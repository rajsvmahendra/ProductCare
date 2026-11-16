-- ============================================================================
-- PRODUCTCARE: Comprehensive Sample Data Seed Script
-- Course: INT402: Modern Web Programming Tools and Techniques
-- Purpose: Populates sample customers, products, warranties, service tickets,
--          and status history for realistic portal demonstration in SQL Server.
-- ============================================================================

SET NOCOUNT ON;

PRINT 'Starting ProductCare sample data population...';

-- 1. Ensure Roles Exist
IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE Name = 'Admin')
BEGIN
    INSERT INTO dbo.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES ('role-admin-guid-0001', 'Admin', 'ADMIN', NEWID());
END

IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE Name = 'Customer')
BEGIN
    INSERT INTO dbo.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES ('role-customer-guid-0002', 'Customer', 'CUSTOMER', NEWID());
END

-- 2. Seed Sample Customers and Admin Users
-- Default Demo Password for accounts: DemoPass123!
-- PBKDF2 with HMAC-SHA256 hash representation
DECLARE @DefaultPasswordHash NVARCHAR(MAX) = 'AQAAAAIAAYagAAAAEIeB3d3k1cQ4UqV8j4F9z9vY5q8K5L9wX2m4P7r0S3u6V9y2A5d8G1j4M7p0R3t6';

-- Admin User
IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUsers WHERE Email = 'admin@productcare.local')
BEGIN
    INSERT INTO dbo.AspNetUsers (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
        PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed,
        TwoFactorEnabled, LockoutEnabled, AccessFailedCount, FullName, Address, City, PostalCode, IsActive, CreatedAtUtc
    )
    VALUES (
        'usr-admin-0001', 'admin@productcare.local', 'ADMIN@PRODUCTCARE.LOCAL',
        'admin@productcare.local', 'ADMIN@PRODUCTCARE.LOCAL', 1,
        @DefaultPasswordHash, NEWID(), NEWID(), 0,
        0, 0, 0, 'System Administrator', '100 IT Park Blvd', 'Tech City', '98052', 1, GETUTCDATE()
    );

    INSERT INTO dbo.AspNetUserRoles (UserId, RoleId)
    VALUES ('usr-admin-0001', 'role-admin-guid-0001');
END

-- Customer 1: John Doe
IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUsers WHERE Email = 'john.doe@example.com')
BEGIN
    INSERT INTO dbo.AspNetUsers (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
        PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed,
        TwoFactorEnabled, LockoutEnabled, AccessFailedCount, FullName, Address, City, PostalCode, IsActive, CreatedAtUtc
    )
    VALUES (
        'usr-john-0002', 'john.doe@example.com', 'JOHN.DOE@EXAMPLE.COM',
        'john.doe@example.com', 'JOHN.DOE@EXAMPLE.COM', 1,
        @DefaultPasswordHash, NEWID(), NEWID(), 0,
        0, 0, 0, 'John Doe', '742 Evergreen Terrace', 'Springfield', '97477', 1, GETUTCDATE()
    );

    INSERT INTO dbo.AspNetUserRoles (UserId, RoleId)
    VALUES ('usr-john-0002', 'role-customer-guid-0002');
END

-- Customer 2: Sarah Connor
IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUsers WHERE Email = 'sarah.connor@example.com')
BEGIN
    INSERT INTO dbo.AspNetUsers (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
        PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed,
        TwoFactorEnabled, LockoutEnabled, AccessFailedCount, FullName, Address, City, PostalCode, IsActive, CreatedAtUtc
    )
    VALUES (
        'usr-sarah-0003', 'sarah.connor@example.com', 'SARAH.CONNOR@EXAMPLE.COM',
        'sarah.connor@example.com', 'SARAH.CONNOR@EXAMPLE.COM', 1,
        @DefaultPasswordHash, NEWID(), NEWID(), 0,
        0, 0, 0, 'Sarah Connor', '1200 Techwood Drive', 'Atlanta', '30318', 1, GETUTCDATE()
    );

    INSERT INTO dbo.AspNetUserRoles (UserId, RoleId)
    VALUES ('usr-sarah-0003', 'role-customer-guid-0002');
END

-- 3. Seed Products for John Doe
DECLARE @JohnId NVARCHAR(450) = 'usr-john-0002';
DECLARE @SarahId NVARCHAR(450) = 'usr-sarah-0003';

-- Product 1: MacBook Pro (Active)
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE UserId = @JohnId AND SerialNumber = 'C02G1001MD6R')
BEGIN
    INSERT INTO dbo.Products (
        UserId, Name, Brand, ModelNumber, SerialNumber, Category,
        PurchaseDate, WarrantyDurationMonths, WarrantyExpiryDate, PurchaseStore, PurchasePrice, Notes, CreatedAtUtc
    )
    VALUES (
        @JohnId, 'MacBook Pro 16-inch M3 Max', 'Apple', 'A2485', 'C02G1001MD6R', 2,
        '2026-01-15', 36, '2029-01-15', 'Apple Store Online', 2499.00,
        '3-Year AppleCare+ coverage registered.', GETUTCDATE()
    );
END

-- Product 2: Sony Headphones (Expiring Soon - 10 days left)
DECLARE @ExpiringSoonDate DATE = DATEADD(day, 10, CAST(GETUTCDATE() AS DATE));
DECLARE @ExpiringSoonPurchase DATE = DATEADD(month, -12, @ExpiringSoonDate);

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE UserId = @JohnId AND SerialNumber = 'SN-SONY-998811')
BEGIN
    INSERT INTO dbo.Products (
        UserId, Name, Brand, ModelNumber, SerialNumber, Category,
        PurchaseDate, WarrantyDurationMonths, WarrantyExpiryDate, PurchaseStore, PurchasePrice, Notes, CreatedAtUtc
    )
    VALUES (
        @JohnId, 'Sony WH-1000XM5 Noise Canceling', 'Sony', 'WH1000XM5/B', 'SN-SONY-998811', 1,
        @ExpiringSoonPurchase, 12, @ExpiringSoonDate, 'Best Buy Electronics', 399.99,
        'Daily commuting headphones. Soft ear cushions.', GETUTCDATE()
    );
END

-- Product 3: Samsung Galaxy S23 (Expired 60 days ago)
DECLARE @ExpiredDate DATE = DATEADD(day, -60, CAST(GETUTCDATE() AS DATE));
DECLARE @ExpiredPurchase DATE = DATEADD(month, -12, @ExpiredDate);

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE UserId = @JohnId AND SerialNumber = 'SM-S23-445566')
BEGIN
    INSERT INTO dbo.Products (
        UserId, Name, Brand, ModelNumber, SerialNumber, Category,
        PurchaseDate, WarrantyDurationMonths, WarrantyExpiryDate, PurchaseStore, PurchasePrice, Notes, CreatedAtUtc
    )
    VALUES (
        @JohnId, 'Samsung Galaxy S23 Ultra', 'Samsung', 'SM-S918B', 'SM-S23-445566', 3,
        @ExpiredPurchase, 12, @ExpiredDate, 'Samsung Direct', 1199.99,
        'Phantom Black 512GB model.', GETUTCDATE()
    );
END

-- Product 4: LG OLED TV for Sarah Connor (Active)
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE UserId = @SarahId AND SerialNumber = 'LG-OLED-554433')
BEGIN
    INSERT INTO dbo.Products (
        UserId, Name, Brand, ModelNumber, SerialNumber, Category,
        PurchaseDate, WarrantyDurationMonths, WarrantyExpiryDate, PurchaseStore, PurchasePrice, Notes, CreatedAtUtc
    )
    VALUES (
        @SarahId, 'LG C3 55-inch OLED 4K TV', 'LG', 'OLED55C3PUA', 'LG-OLED-554433', 6,
        '2026-05-10', 24, '2028-05-10', 'Costco Wholesale', 1499.99,
        'Home theater living room display.', GETUTCDATE()
    );
END

-- 4. Seed Service Requests
DECLARE @HeadphonesId INT = (SELECT TOP 1 Id FROM dbo.Products WHERE SerialNumber = 'SN-SONY-998811');
DECLARE @TvId INT = (SELECT TOP 1 Id FROM dbo.Products WHERE SerialNumber = 'LG-OLED-554433');

-- Ticket 1: Headphones (In Progress)
IF @HeadphonesId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.ServiceRequests WHERE TicketNumber = 'PC-20261110-A1B2')
BEGIN
    INSERT INTO dbo.ServiceRequests (
        TicketNumber, ProductId, UserId, Category, Priority, Status,
        Subject, Description, AssignedTechnician, AdminRemarks, CreatedAtUtc, UpdatedAtUtc
    )
    VALUES (
        'PC-20261110-A1B2', @HeadphonesId, @JohnId, 1, 3, 4,
        'Right ear speaker distortion during ANC mode',
        'When active noise cancellation is switched on, a high-pitch crackling noise is heard in the right ear cup. Firmware updated to latest version but problem persists.',
        'Alex Rivera (Audio Specialist)',
        'Diagnostic confirmed driver membrane vibration fault. Replacement acoustic transducer ordered under warranty.',
        DATEADD(day, -5, GETUTCDATE()), DATEADD(day, -1, GETUTCDATE())
    );

    DECLARE @Ticket1Id INT = SCOPE_IDENTITY();

    INSERT INTO dbo.ServiceRequestHistories (ServiceRequestId, FromStatus, ToStatus, ChangedByUserId, ChangedByUserName, Remarks, CreatedAtUtc)
    VALUES 
        (@Ticket1Id, 1, 1, @JohnId, 'John Doe', 'Service ticket submitted by customer.', DATEADD(day, -5, GETUTCDATE())),
        (@Ticket1Id, 1, 2, 'usr-admin-0001', 'System Administrator', 'Ticket acknowledged and prioritized for audio hardware inspection.', DATEADD(day, -4, GETUTCDATE())),
        (@Ticket1Id, 2, 3, 'usr-admin-0001', 'System Administrator', 'Assigned to Audio Specialist Alex Rivera.', DATEADD(day, -3, GETUTCDATE())),
        (@Ticket1Id, 3, 4, 'usr-admin-0001', 'System Administrator', 'Diagnosis completed. Replacement parts in transit.', DATEADD(day, -1, GETUTCDATE()));
END

-- Ticket 2: TV (Resolved)
IF @TvId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.ServiceRequests WHERE TicketNumber = 'PC-20261112-C3D4')
BEGIN
    INSERT INTO dbo.ServiceRequests (
        TicketNumber, ProductId, UserId, Category, Priority, Status,
        Subject, Description, AssignedTechnician, AdminRemarks, CreatedAtUtc, UpdatedAtUtc, ResolvedAtUtc
    )
    VALUES (
        'PC-20261112-C3D4', @TvId, @SarahId, 2, 2, 6,
        'Wall mounting and color profile calibration',
        'Requesting professional technician visit to wall mount 55-inch OLED display and configure Dolby Vision inputs.',
        'David Miller (Display Field Tech)',
        'Completed wall mount installation with heavy-duty bracket. Tested HDR and eARC audio passthrough.',
        DATEADD(day, -4, GETUTCDATE()), DATEADD(day, -1, GETUTCDATE()), DATEADD(day, -1, GETUTCDATE())
    );

    DECLARE @Ticket2Id INT = SCOPE_IDENTITY();

    INSERT INTO dbo.ServiceRequestHistories (ServiceRequestId, FromStatus, ToStatus, ChangedByUserId, ChangedByUserName, Remarks, CreatedAtUtc)
    VALUES 
        (@Ticket2Id, 1, 1, @SarahId, 'Sarah Connor', 'Installation request booked by customer.', DATEADD(day, -4, GETUTCDATE())),
        (@Ticket2Id, 1, 3, 'usr-admin-0001', 'System Administrator', 'Assigned to Field Tech David Miller.', DATEADD(day, -3, GETUTCDATE())),
        (@Ticket2Id, 3, 4, 'usr-admin-0001', 'System Administrator', 'Technician on site for installation.', DATEADD(day, -2, GETUTCDATE())),
        (@Ticket2Id, 4, 6, 'usr-admin-0001', 'System Administrator', 'Installation verified and completed successfully.', DATEADD(day, -1, GETUTCDATE()));
END

PRINT 'Sample data population completed successfully.';
GO
