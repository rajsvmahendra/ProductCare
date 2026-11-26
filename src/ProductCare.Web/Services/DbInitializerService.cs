using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductCare.Web.Data;
using ProductCare.Web.Models.Entities;
using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.Services;

/// <summary>
/// Handles one-time database role initialization, secure administrator provisioning,
/// and optional demonstration dataset seeding.
/// </summary>
public class DbInitializerService : IDbInitializerService
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DbInitializerService> _logger;

    public const string RoleAdmin = "Admin";
    public const string RoleCustomer = "Customer";

    public DbInitializerService(
        ApplicationDbContext context,
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger<DbInitializerService> logger)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task EnsureRolesExistAsync()
    {
        string[] roles = { RoleAdmin, RoleCustomer };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
                _logger.LogInformation("Created Identity role: {Role}", role);
            }
        }
    }

    public async Task<string> ProvisionAdminUserAsync(string? email = null, string? password = null)
    {
        await EnsureRolesExistAsync();

        var adminEmail = email
            ?? _configuration["AdminSetup:Email"]
            ?? Environment.GetEnvironmentVariable("PRODUCTCARE_ADMIN_EMAIL")
            ?? "admin@productcare.local";

        var adminUser = await _userManager.FindByEmailAsync(adminEmail);
        var generatedPassword = false;

        if (adminUser == null)
        {
            var adminPassword = password
                ?? _configuration["AdminSetup:Password"]
                ?? Environment.GetEnvironmentVariable("PRODUCTCARE_ADMIN_PASSWORD");

            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                adminPassword = GenerateSecurePassword();
                generatedPassword = true;
            }

            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(adminUser, adminPassword);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to provision admin user: {errors}");
            }

            await _userManager.AddToRoleAsync(adminUser, RoleAdmin);

            _logger.LogInformation("Administrator user provisioned successfully for {Email}", adminEmail);

            return generatedPassword ? adminPassword : "[Password configured via environment/secrets]";
        }

        // Ensure user has Admin role
        if (!await _userManager.IsInRoleAsync(adminUser, RoleAdmin))
        {
            await _userManager.AddToRoleAsync(adminUser, RoleAdmin);
            _logger.LogInformation("Assigned Admin role to existing user {Email}", adminEmail);
        }

        return "[Administrator account already exists]";
    }

    public async Task EnsureDatabaseObjectsCreatedAsync()
    {
        try
        {
            // Create or alter view vw_ProductWarrantySummary
            await _context.Database.ExecuteSqlRawAsync(@"
CREATE OR ALTER VIEW dbo.vw_ProductWarrantySummary
AS
SELECT 
    p.Id AS ProductId,
    p.Name AS ProductName,
    p.Brand,
    p.ModelNumber,
    p.SerialNumber,
    CASE p.Category
        WHEN 1 THEN 'Electronics'
        WHEN 2 THEN 'Computers'
        WHEN 3 THEN 'Mobile Devices'
        WHEN 4 THEN 'Home Appliances'
        WHEN 5 THEN 'Kitchen Appliances'
        WHEN 6 THEN 'Home Entertainment'
        WHEN 7 THEN 'Wearables'
        WHEN 8 THEN 'Office Equipment'
        ELSE 'Other'
    END AS CategoryName,
    p.UserId,
    u.FullName AS CustomerName,
    ISNULL(u.Email, 'N/A') AS CustomerEmail,
    p.PurchaseDate,
    p.WarrantyDurationMonths,
    p.WarrantyExpiryDate,
    DATEDIFF(day, CAST(GETUTCDATE() AS DATE), p.WarrantyExpiryDate) AS DaysRemaining,
    CASE 
        WHEN CAST(GETUTCDATE() AS DATE) > p.WarrantyExpiryDate THEN 'Expired'
        WHEN DATEDIFF(day, CAST(GETUTCDATE() AS DATE), p.WarrantyExpiryDate) <= 30 THEN 'Expiring Soon'
        ELSE 'Active'
    END AS WarrantyStatusText
FROM dbo.Products p
INNER JOIN dbo.AspNetUsers u ON p.UserId = u.Id;");

            // Create or alter stored procedure sp_GetServiceTicketStatistics
            await _context.Database.ExecuteSqlRawAsync(@"
CREATE OR ALTER PROCEDURE dbo.sp_GetServiceTicketStatistics
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CASE sr.Category
            WHEN 1 THEN 'Repair'
            WHEN 2 THEN 'Installation'
            WHEN 3 THEN 'Maintenance'
            WHEN 4 THEN 'Warranty Claim'
            ELSE 'Other'
        END AS CategoryName,
        COUNT(sr.Id) AS TotalTickets,
        SUM(CASE WHEN sr.Status = 1 THEN 1 ELSE 0 END) AS SubmittedCount,
        SUM(CASE WHEN sr.Status = 2 THEN 1 ELSE 0 END) AS UnderReviewCount,
        SUM(CASE WHEN sr.Status = 3 THEN 1 ELSE 0 END) AS AssignedCount,
        SUM(CASE WHEN sr.Status = 4 THEN 1 ELSE 0 END) AS InProgressCount,
        SUM(CASE WHEN sr.Status = 5 THEN 1 ELSE 0 END) AS WaitingForCustomerCount,
        SUM(CASE WHEN sr.Status = 6 THEN 1 ELSE 0 END) AS ResolvedCount,
        SUM(CASE WHEN sr.Status = 7 THEN 1 ELSE 0 END) AS ClosedCount,
        SUM(CASE WHEN sr.Status = 8 THEN 1 ELSE 0 END) AS RejectedCount
    FROM dbo.ServiceRequests sr
    WHERE (@StartDate IS NULL OR sr.CreatedAtUtc >= @StartDate)
      AND (@EndDate IS NULL OR sr.CreatedAtUtc <= @EndDate)
    GROUP BY sr.Category
    ORDER BY TotalTickets DESC;
END;");

            _logger.LogInformation("Database views and stored procedures ensured successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not ensure views/stored procedures automatically; fallback LINQ queries will be used.");
        }
    }

    public async Task SeedDemoDataAsync()
    {
        await EnsureRolesExistAsync();

        // 1. Provision Admin
        await ProvisionAdminUserAsync("admin@productcare.local", "Admin123!");
        var adminUser = await _userManager.FindByEmailAsync("admin@productcare.local");
        var adminUserId = adminUser?.Id ?? string.Empty;

        // 2. Provision Demo Customer 1 (John Doe)
        var johnEmail = "john.doe@example.com";
        var johnUser = await _userManager.FindByEmailAsync(johnEmail);
        if (johnUser == null)
        {
            johnUser = new ApplicationUser
            {
                UserName = johnEmail,
                Email = johnEmail,
                FullName = "John Doe",
                EmailConfirmed = true,
                Address = "742 Evergreen Terrace",
                City = "Springfield",
                PostalCode = "97477",
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow.AddMonths(-6)
            };
            await _userManager.CreateAsync(johnUser, "Customer123!");
            await _userManager.AddToRoleAsync(johnUser, RoleCustomer);
        }

        // 3. Provision Demo Customer 2 (Sarah Connor)
        var sarahEmail = "sarah.connor@example.com";
        var sarahUser = await _userManager.FindByEmailAsync(sarahEmail);
        if (sarahUser == null)
        {
            sarahUser = new ApplicationUser
            {
                UserName = sarahEmail,
                Email = sarahEmail,
                FullName = "Sarah Connor",
                EmailConfirmed = true,
                Address = "1200 Techwood Drive",
                City = "Atlanta",
                PostalCode = "30318",
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow.AddMonths(-4)
            };
            await _userManager.CreateAsync(sarahUser, "Customer123!");
            await _userManager.AddToRoleAsync(sarahUser, RoleCustomer);
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // 4. Seed Products for John Doe if not exists
        if (!await _context.Products.AnyAsync(p => p.UserId == johnUser.Id))
        {
            var p1 = new Product
            {
                UserId = johnUser.Id,
                Name = "MacBook Pro 16-inch M3 Max",
                Brand = "Apple",
                ModelNumber = "A2485",
                SerialNumber = "C02G1001MD6R",
                Category = ProductCategory.Computers,
                PurchaseDate = today.AddMonths(-5),
                WarrantyDurationMonths = 36,
                WarrantyExpiryDate = today.AddMonths(31), // Active
                PurchaseStore = "Apple Store Online",
                PurchasePrice = 2499.00m,
                Notes = "3-Year AppleCare+ coverage registered.",
                CreatedAtUtc = DateTime.UtcNow.AddMonths(-5)
            };

            var p2 = new Product
            {
                UserId = johnUser.Id,
                Name = "Sony WH-1000XM5 Noise Canceling",
                Brand = "Sony",
                ModelNumber = "WH1000XM5/B",
                SerialNumber = "SN-SONY-998811",
                Category = ProductCategory.Electronics,
                PurchaseDate = today.AddMonths(-11).AddDays(-20),
                WarrantyDurationMonths = 12,
                WarrantyExpiryDate = today.AddDays(10), // Expiring Soon (10 days)
                PurchaseStore = "Best Buy Electronics",
                PurchasePrice = 399.99m,
                Notes = "Daily commuting headphones.",
                CreatedAtUtc = DateTime.UtcNow.AddMonths(-11)
            };

            var p3 = new Product
            {
                UserId = johnUser.Id,
                Name = "Samsung Galaxy S23 Ultra",
                Brand = "Samsung",
                ModelNumber = "SM-S918B",
                SerialNumber = "SM-S23-445566",
                Category = ProductCategory.MobileDevices,
                PurchaseDate = today.AddMonths(-15),
                WarrantyDurationMonths = 12,
                WarrantyExpiryDate = today.AddMonths(-3), // Expired
                PurchaseStore = "Samsung Direct",
                PurchasePrice = 1199.99m,
                Notes = "Phantom Black 512GB model.",
                CreatedAtUtc = DateTime.UtcNow.AddMonths(-15)
            };

            _context.Products.AddRange(p1, p2, p3);
            await _context.SaveChangesAsync();

            // Add Service Requests for John's products
            var sr1 = new ServiceRequest
            {
                TicketNumber = "PC-20261110-A1B2",
                ProductId = p2.Id,
                UserId = johnUser.Id,
                Category = RequestCategory.Repair,
                Priority = PriorityLevel.High,
                Status = RequestStatus.InProgress,
                Subject = "Right ear speaker distortion during ANC mode",
                Description = "When active noise cancellation is switched on, a high-pitch crackling noise is heard in the right ear cup. Firmware is updated.",
                AssignedTechnician = "Alex Rivera (Audio Specialist)",
                AdminRemarks = "Diagnostic confirmed driver membrane vibration fault. Replacement acoustic transducer ordered under warranty.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-5),
                UpdatedAtUtc = DateTime.UtcNow.AddDays(-1)
            };

            var h1_1 = new ServiceRequestHistory
            {
                ServiceRequest = sr1,
                FromStatus = RequestStatus.Submitted,
                ToStatus = RequestStatus.Submitted,
                ChangedByUserId = johnUser.Id,
                ChangedByUserName = "John Doe",
                Remarks = "Service ticket submitted by customer.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-5)
            };

            var h1_2 = new ServiceRequestHistory
            {
                ServiceRequest = sr1,
                FromStatus = RequestStatus.Submitted,
                ToStatus = RequestStatus.UnderReview,
                ChangedByUserId = adminUserId,
                ChangedByUserName = "System Administrator",
                Remarks = "Acknowledged and scheduled for diagnostic inspection.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-4)
            };

            var h1_3 = new ServiceRequestHistory
            {
                ServiceRequest = sr1,
                FromStatus = RequestStatus.UnderReview,
                ToStatus = RequestStatus.Assigned,
                ChangedByUserId = adminUserId,
                ChangedByUserName = "System Administrator",
                Remarks = "Assigned to Alex Rivera.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-3)
            };

            var h1_4 = new ServiceRequestHistory
            {
                ServiceRequest = sr1,
                FromStatus = RequestStatus.Assigned,
                ToStatus = RequestStatus.InProgress,
                ChangedByUserId = adminUserId,
                ChangedByUserName = "System Administrator",
                Remarks = "Replacement parts ordered from manufacturer.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-1)
            };

            _context.ServiceRequests.Add(sr1);
            _context.ServiceRequestHistories.AddRange(h1_1, h1_2, h1_3, h1_4);
            await _context.SaveChangesAsync();
        }

        // 5. Seed Products for Sarah Connor if not exists
        if (!await _context.Products.AnyAsync(p => p.UserId == sarahUser.Id))
        {
            var p4 = new Product
            {
                UserId = sarahUser.Id,
                Name = "LG C3 55-inch OLED 4K TV",
                Brand = "LG",
                ModelNumber = "OLED55C3PUA",
                SerialNumber = "LG-OLED-554433",
                Category = ProductCategory.HomeEntertainment,
                PurchaseDate = today.AddMonths(-6),
                WarrantyDurationMonths = 24,
                WarrantyExpiryDate = today.AddMonths(18), // Active
                PurchaseStore = "Costco Wholesale",
                PurchasePrice = 1499.99m,
                Notes = "Living room home theater setup.",
                CreatedAtUtc = DateTime.UtcNow.AddMonths(-6)
            };

            var p5 = new Product
            {
                UserId = sarahUser.Id,
                Name = "Dell XPS 15 9520",
                Brand = "Dell",
                ModelNumber = "XPS-9520",
                SerialNumber = "DELL-XPS-774411",
                Category = ProductCategory.Computers,
                PurchaseDate = today.AddMonths(-11).AddDays(-15),
                WarrantyDurationMonths = 12,
                WarrantyExpiryDate = today.AddDays(15), // Expiring Soon (15 days)
                PurchaseStore = "Dell Online Direct",
                PurchasePrice = 1899.00m,
                Notes = "Work development laptop.",
                CreatedAtUtc = DateTime.UtcNow.AddMonths(-11)
            };

            _context.Products.AddRange(p4, p5);
            await _context.SaveChangesAsync();

            var sr2 = new ServiceRequest
            {
                TicketNumber = "PC-20261112-C3D4",
                ProductId = p4.Id,
                UserId = sarahUser.Id,
                Category = RequestCategory.Installation,
                Priority = PriorityLevel.Medium,
                Status = RequestStatus.Resolved,
                Subject = "Wall mounting and color profile calibration",
                Description = "Requesting professional technician visit to wall mount display and configure Dolby Vision inputs.",
                AssignedTechnician = "David Miller (Display Field Tech)",
                AdminRemarks = "Completed wall mount installation with heavy-duty bracket. Tested HDR and eARC audio passthrough.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-4),
                UpdatedAtUtc = DateTime.UtcNow.AddDays(-1),
                ResolvedAtUtc = DateTime.UtcNow.AddDays(-1)
            };

            var h2_1 = new ServiceRequestHistory
            {
                ServiceRequest = sr2,
                FromStatus = RequestStatus.Submitted,
                ToStatus = RequestStatus.Submitted,
                ChangedByUserId = sarahUser.Id,
                ChangedByUserName = "Sarah Connor",
                Remarks = "Installation request submitted by customer.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-4)
            };

            var h2_2 = new ServiceRequestHistory
            {
                ServiceRequest = sr2,
                FromStatus = RequestStatus.Submitted,
                ToStatus = RequestStatus.Assigned,
                ChangedByUserId = adminUserId,
                ChangedByUserName = "System Administrator",
                Remarks = "Assigned to David Miller.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-3)
            };

            var h2_3 = new ServiceRequestHistory
            {
                ServiceRequest = sr2,
                FromStatus = RequestStatus.Assigned,
                ToStatus = RequestStatus.InProgress,
                ChangedByUserId = adminUserId,
                ChangedByUserName = "System Administrator",
                Remarks = "Technician dispatched to customer site.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
            };

            var h2_4 = new ServiceRequestHistory
            {
                ServiceRequest = sr2,
                FromStatus = RequestStatus.InProgress,
                ToStatus = RequestStatus.Resolved,
                ChangedByUserId = adminUserId,
                ChangedByUserName = "System Administrator",
                Remarks = "Installation inspected and verified.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-1)
            };

            _context.ServiceRequests.Add(sr2);
            _context.ServiceRequestHistories.AddRange(h2_1, h2_2, h2_3, h2_4);
            await _context.SaveChangesAsync();
        }

        // 5b. Seed Service Centers if not exists
        if (!await _context.ServiceCenters.AnyAsync())
        {
            var sc1 = new ServiceCenter
            {
                Name = "Metro Tech Care & Repairs",
                Address = "142 Commerce Boulevard, Suite 300",
                City = "Springfield",
                SupportedBrands = "Apple, Dell, HP, Lenovo, ASUS",
                SupportedCategories = "Laptops, Computers, Tablets, Monitors",
                Phone = "+1 (555) 234-5678",
                Email = "support@metrotechcare.demo",
                OpeningHours = "Mon-Fri: 9:00 AM - 6:00 PM, Sat: 10:00 AM - 3:00 PM",
                Website = "https://metrotechcare.example.com",
                MapUrl = "https://maps.google.com/?q=Metro+Tech+Care+Springfield",
                IsActive = true,
                IsVerified = false,
                Notes = "Sample demonstration repair hub for laptop diagnostics, motherboard repair, and battery replacement."
            };

            var sc2 = new ServiceCenter
            {
                Name = "Apex Authorized Electronics Hub",
                Address = "880 Precision Way, Technology Park",
                City = "Springfield",
                SupportedBrands = "Sony, Samsung, LG, Bose, JBL",
                SupportedCategories = "AudioDevices, Televisions, HomeEntertainment, Wearables",
                Phone = "+1 (555) 876-5432",
                Email = "service@apexhub.demo",
                OpeningHours = "Mon-Sat: 10:00 AM - 7:00 PM",
                Website = "https://apexhub.example.com",
                MapUrl = "https://maps.google.com/?q=Apex+Electronics+Hub+Springfield",
                IsActive = true,
                IsVerified = true,
                Notes = "Authorized manufacturer diagnostic facility for display calibration, audio driver replacement, and board service."
            };

            var sc3 = new ServiceCenter
            {
                Name = "Campus Express Device Lab",
                Address = "Student Union Building, Ground Floor, Room 12",
                City = "University District",
                SupportedBrands = "Apple, Samsung, Google, Dell, HP, Sony",
                SupportedCategories = "MobilePhones, Laptops, Tablets, Smartwatches, GamingConsoles",
                Phone = "+1 (555) 345-6789",
                Email = "express@campusdevicelab.demo",
                OpeningHours = "Mon-Fri: 8:30 AM - 7:00 PM, Sat: 9:00 AM - 1:00 PM",
                Website = "https://campusdevicelab.example.com",
                MapUrl = "https://maps.google.com/?q=Campus+Express+Device+Lab",
                IsActive = true,
                IsVerified = false,
                Notes = "Sample on-campus rapid triage kiosk offering screen repairs, thermal re-pasting, and OS recovery."
            };

            var sc4 = new ServiceCenter
            {
                Name = "Precision Camera & Optical Service",
                Address = "45 Studio Lane, Creative Quarter",
                City = "Downtown",
                SupportedBrands = "Sony, Canon, Nikon, Fujifilm, DJI",
                SupportedCategories = "Cameras, OtherElectronics",
                Phone = "+1 (555) 456-7890",
                Email = "contact@precisioncamera.demo",
                OpeningHours = "Tue-Sat: 10:00 AM - 5:30 PM",
                Website = "https://precisioncamera.example.com",
                MapUrl = "https://maps.google.com/?q=Precision+Camera+Service",
                IsActive = true,
                IsVerified = false,
                Notes = "Sample optical diagnostics lab for sensor cleaning, shutter mechanism rebuilds, and lens calibration."
            };

            _context.ServiceCenters.AddRange(sc1, sc2, sc3, sc4);
            await _context.SaveChangesAsync();
        }

        // 6. Seed Campus Network Quality Reports (EduRev P10)
        if (!await _context.NetworkQualityReports.AnyAsync())
        {
            var adminUserRef = await _userManager.FindByEmailAsync("admin@productcare.local");
            var adminId = adminUserRef?.Id ?? johnUser.Id;

            var r1 = new NetworkQualityReport
            {
                UserId = johnUser.Id,
                CampusZone = "Central Library",
                NetworkType = "Campus Wi-Fi (Enterprise)",
                DownloadSpeedMbps = 84.5,
                LatencyMs = 19,
                IsClientMeasured = true,
                Notes = "Measured at main reading desk. Low jitter.",
                MeasuredAtUtc = DateTime.UtcNow.AddHours(-3)
            };

            var r2 = new NetworkQualityReport
            {
                UserId = johnUser.Id,
                CampusZone = "Hostel Block A",
                NetworkType = "Campus Wi-Fi (Enterprise)",
                DownloadSpeedMbps = 12.8,
                LatencyMs = 88,
                IsClientMeasured = false,
                Notes = "Intermittent drops during evening peak hours.",
                MeasuredAtUtc = DateTime.UtcNow.AddHours(-6)
            };

            var r3 = new NetworkQualityReport
            {
                UserId = sarahUser.Id,
                CampusZone = "Computer Science Lab 3",
                NetworkType = "Lab Gigabit Ethernet",
                DownloadSpeedMbps = 112.4,
                LatencyMs = 11,
                IsClientMeasured = true,
                Notes = "Excellent throughput on developer workstations.",
                MeasuredAtUtc = DateTime.UtcNow.AddHours(-1)
            };

            var r4 = new NetworkQualityReport
            {
                UserId = sarahUser.Id,
                CampusZone = "Auditorium & Student Wing",
                NetworkType = "Campus Wi-Fi (Guest)",
                DownloadSpeedMbps = 38.0,
                LatencyMs = 35,
                IsClientMeasured = true,
                Notes = "Stable signal during student orientation seminar.",
                MeasuredAtUtc = DateTime.UtcNow.AddHours(-12)
            };

            _context.NetworkQualityReports.AddRange(r1, r2, r3, r4);
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("Demonstration data seeded successfully.");
    }

    private static string GenerateSecurePassword()
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnopqrstuvwxyz";
        const string digits = "23456789";
        const string specials = "!@#$%^&*";

        var chars = new char[16];
        chars[0] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
        chars[1] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
        chars[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        chars[3] = specials[RandomNumberGenerator.GetInt32(specials.Length)];

        var allChars = upper + lower + digits + specials;
        for (var i = 4; i < chars.Length; i++)
        {
            chars[i] = allChars[RandomNumberGenerator.GetInt32(allChars.Length)];
        }

        return new string(chars.OrderBy(_ => RandomNumberGenerator.GetInt32(1000)).ToArray());
    }
}
