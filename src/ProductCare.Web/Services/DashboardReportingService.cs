using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductCare.Web.Data;
using ProductCare.Web.Models.Entities;
using ProductCare.Web.Models.Enums;
using ProductCare.Web.ViewModels.Admin;
using ProductCare.Web.ViewModels.Customer;

namespace ProductCare.Web.Services;

/// <summary>
/// Operational reporting service that computes customer and administrative intelligence.
/// Interacts with database tables, SQL views, and stored procedures.
/// </summary>
public class DashboardReportingService : IDashboardReportingService
{
    private readonly ApplicationDbContext _context;
    private readonly IWarrantyCalculatorService _warrantyCalculator;
    private readonly ILogger<DashboardReportingService> _logger;

    public DashboardReportingService(
        ApplicationDbContext context,
        IWarrantyCalculatorService warrantyCalculator,
        ILogger<DashboardReportingService> logger)
    {
        _context = context;
        _warrantyCalculator = warrantyCalculator;
        _logger = logger;
    }

    public async Task<CustomerDashboardViewModel> GetCustomerDashboardAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        var customerName = user?.FullName ?? "Customer";

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var expiringSoonThreshold = today.AddDays(30);

        var products = await _context.Products
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderBy(p => p.WarrantyExpiryDate)
            .ToListAsync(cancellationToken);

        var totalProducts = products.Count;
        var activeCount = 0;
        var expiringSoonCount = 0;
        var expiredCount = 0;

        var expiringSoonList = new List<ProductSummaryItem>();

        foreach (var p in products)
        {
            var warranty = _warrantyCalculator.GetWarrantyDetails(p.WarrantyExpiryDate, today);

            switch (warranty.Status)
            {
                case WarrantyStatus.Active:
                    activeCount++;
                    break;
                case WarrantyStatus.ExpiringSoon:
                    expiringSoonCount++;
                    expiringSoonList.Add(new ProductSummaryItem
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Brand = p.Brand,
                        ModelNumber = p.ModelNumber,
                        ExpiryDate = warranty.ExpiryDate,
                        RemainingDays = warranty.RemainingDays,
                        StatusBadgeCssClass = warranty.StatusBadgeCssClass,
                        StatusText = warranty.StatusDisplayText
                    });
                    break;
                case WarrantyStatus.Expired:
                    expiredCount++;
                    break;
            }
        }

        var tickets = await _context.ServiceRequests
            .AsNoTracking()
            .Include(sr => sr.Product)
            .Where(sr => sr.UserId == userId)
            .OrderByDescending(sr => sr.UpdatedAtUtc ?? sr.CreatedAtUtc)
            .Take(5)
            .ToListAsync(cancellationToken);

        var openStatuses = new[]
        {
            RequestStatus.Submitted,
            RequestStatus.UnderReview,
            RequestStatus.Assigned,
            RequestStatus.InProgress,
            RequestStatus.WaitingForCustomer
        };

        var openTicketsCount = await _context.ServiceRequests
            .AsNoTracking()
            .CountAsync(sr => sr.UserId == userId && openStatuses.Contains(sr.Status), cancellationToken);

        var resolvedTicketsCount = await _context.ServiceRequests
            .AsNoTracking()
            .CountAsync(sr => sr.UserId == userId && (sr.Status == RequestStatus.Resolved || sr.Status == RequestStatus.Closed), cancellationToken);

        var recentTicketItems = tickets.Select(t => new ServiceRequestSummaryItem
        {
            Id = t.Id,
            TicketNumber = t.TicketNumber,
            ProductName = t.Product?.Name ?? "N/A",
            Subject = t.Subject,
            PriorityText = t.Priority.ToString(),
            PriorityBadgeCssClass = GetPriorityBadgeClass(t.Priority),
            StatusText = FormatStatusText(t.Status),
            StatusBadgeCssClass = GetStatusBadgeClass(t.Status),
            CreatedAtUtc = t.CreatedAtUtc,
            UpdatedAtUtc = t.UpdatedAtUtc
        }).ToList();

        return new CustomerDashboardViewModel
        {
            CustomerName = customerName,
            TotalProducts = totalProducts,
            ActiveWarranties = activeCount,
            ExpiringSoonWarranties = expiringSoonCount,
            ExpiredWarranties = expiredCount,
            OpenServiceRequests = openTicketsCount,
            ResolvedServiceRequests = resolvedTicketsCount,
            ExpiringSoonProducts = expiringSoonList,
            RecentServiceRequests = recentTicketItems
        };
    }

    public async Task<AdminDashboardViewModel> GetAdminDashboardAsync(CancellationToken cancellationToken = default)
    {
        var totalCustomers = await _context.Users.CountAsync(cancellationToken);
        var totalProducts = await _context.Products.CountAsync(cancellationToken);
        var totalRequests = await _context.ServiceRequests.CountAsync(cancellationToken);

        var pendingStatuses = new[] { RequestStatus.Submitted, RequestStatus.UnderReview };
        var inProgressStatuses = new[] { RequestStatus.Assigned, RequestStatus.InProgress, RequestStatus.WaitingForCustomer };

        var pendingCount = await _context.ServiceRequests.CountAsync(sr => pendingStatuses.Contains(sr.Status), cancellationToken);
        var inProgressCount = await _context.ServiceRequests.CountAsync(sr => inProgressStatuses.Contains(sr.Status), cancellationToken);
        var resolvedCount = await _context.ServiceRequests.CountAsync(sr => sr.Status == RequestStatus.Resolved, cancellationToken);
        var closedCount = await _context.ServiceRequests.CountAsync(sr => sr.Status == RequestStatus.Closed, cancellationToken);
        var rejectedCount = await _context.ServiceRequests.CountAsync(sr => sr.Status == RequestStatus.Rejected, cancellationToken);

        var recentTickets = await _context.ServiceRequests
            .AsNoTracking()
            .Include(sr => sr.Product)
            .Include(sr => sr.User)
            .OrderByDescending(sr => sr.CreatedAtUtc)
            .Take(8)
            .Select(sr => new AdminRecentTicketItem
            {
                Id = sr.Id,
                TicketNumber = sr.TicketNumber,
                CustomerName = sr.User.FullName,
                CustomerEmail = sr.User.Email ?? "N/A",
                ProductName = sr.Product.Name,
                CategoryText = sr.Category.ToString(),
                PriorityText = sr.Priority.ToString(),
                PriorityBadgeCssClass = GetPriorityBadgeClass(sr.Priority),
                StatusText = FormatStatusText(sr.Status),
                StatusBadgeCssClass = GetStatusBadgeClass(sr.Status),
                CreatedAtUtc = sr.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        return new AdminDashboardViewModel
        {
            TotalCustomers = totalCustomers,
            TotalProducts = totalProducts,
            TotalServiceRequests = totalRequests,
            PendingRequests = pendingCount,
            InProgressRequests = inProgressCount,
            ResolvedRequests = resolvedCount,
            ClosedRequests = closedCount,
            RejectedRequests = rejectedCount,
            RecentTickets = recentTickets
        };
    }

    public async Task<IReadOnlyList<ServiceTicketStatisticsDto>> GetServiceTicketStatisticsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Execute SQL Server Stored Procedure
            var results = await _context.ServiceTicketStatistics
                .FromSqlInterpolated($"EXEC dbo.sp_GetServiceTicketStatistics {startDate}, {endDate}")
                .ToListAsync(cancellationToken);

            if (results.Any())
            {
                return results;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Stored procedure sp_GetServiceTicketStatistics execution failed or db unavailable; executing LINQ fallback.");
        }

        // Resilient in-memory/LINQ fallback for database-agnostic or development scenarios
        var query = _context.ServiceRequests.AsNoTracking().AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(sr => sr.CreatedAtUtc >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(sr => sr.CreatedAtUtc <= endDate.Value);
        }

        var grouped = await query
            .GroupBy(sr => sr.Category)
            .Select(g => new ServiceTicketStatisticsDto
            {
                CategoryName = g.Key.ToString(),
                TotalTickets = g.Count(),
                SubmittedCount = g.Count(sr => sr.Status == RequestStatus.Submitted),
                UnderReviewCount = g.Count(sr => sr.Status == RequestStatus.UnderReview),
                AssignedCount = g.Count(sr => sr.Status == RequestStatus.Assigned),
                InProgressCount = g.Count(sr => sr.Status == RequestStatus.InProgress),
                WaitingForCustomerCount = g.Count(sr => sr.Status == RequestStatus.WaitingForCustomer),
                ResolvedCount = g.Count(sr => sr.Status == RequestStatus.Resolved),
                ClosedCount = g.Count(sr => sr.Status == RequestStatus.Closed),
                RejectedCount = g.Count(sr => sr.Status == RequestStatus.Rejected)
            })
            .OrderByDescending(s => s.TotalTickets)
            .ToListAsync(cancellationToken);

        return grouped;
    }

    public async Task<IReadOnlyList<ProductWarrantySummaryView>> GetWarrantySummaryReportAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Query SQL Server View vw_ProductWarrantySummary
            var viewData = await _context.ProductWarrantySummaries
                .AsNoTracking()
                .OrderBy(v => v.WarrantyExpiryDate)
                .ToListAsync(cancellationToken);

            if (viewData.Any())
            {
                return viewData;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Querying vw_ProductWarrantySummary failed or view not present; executing LINQ fallback.");
        }

        // Resilient LINQ fallback
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var products = await _context.Products
            .AsNoTracking()
            .Include(p => p.User)
            .OrderBy(p => p.WarrantyExpiryDate)
            .ToListAsync(cancellationToken);

        var list = new List<ProductWarrantySummaryView>();
        foreach (var p in products)
        {
            var details = _warrantyCalculator.GetWarrantyDetails(p.WarrantyExpiryDate, today);
            list.Add(new ProductWarrantySummaryView
            {
                ProductId = p.Id,
                ProductName = p.Name,
                Brand = p.Brand,
                ModelNumber = p.ModelNumber,
                SerialNumber = p.SerialNumber,
                CategoryName = p.Category.ToString(),
                UserId = p.UserId,
                CustomerName = p.User?.FullName ?? "N/A",
                CustomerEmail = p.User?.Email ?? "N/A",
                PurchaseDate = p.PurchaseDate,
                WarrantyDurationMonths = p.WarrantyDurationMonths,
                WarrantyExpiryDate = p.WarrantyExpiryDate,
                DaysRemaining = details.RemainingDays,
                WarrantyStatusText = details.StatusDisplayText
            });
        }

        return list;
    }

    public async Task<AdminReportsViewModel> GetFullReportsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var stats = await GetServiceTicketStatisticsAsync(startDate, endDate, cancellationToken);
        var warranties = await GetWarrantySummaryReportAsync(cancellationToken);

        var totalTickets = stats.Sum(s => s.TotalTickets);
        var activeCount = warranties.Count(w => w.WarrantyStatusText == "Active");
        var expiringSoonCount = warranties.Count(w => w.WarrantyStatusText == "Expiring Soon");
        var expiredCount = warranties.Count(w => w.WarrantyStatusText == "Expired");

        return new AdminReportsViewModel
        {
            FilterStartDate = startDate,
            FilterEndDate = endDate,
            TicketStatistics = stats.ToList(),
            WarrantySummaries = warranties.ToList(),
            TotalTicketsCount = totalTickets,
            ActiveWarrantiesCount = activeCount,
            ExpiringSoonWarrantiesCount = expiringSoonCount,
            ExpiredWarrantiesCount = expiredCount
        };
    }

    private static string GetPriorityBadgeClass(PriorityLevel priority) => priority switch
    {
        PriorityLevel.Low => "badge bg-secondary",
        PriorityLevel.Medium => "badge bg-info text-dark",
        PriorityLevel.High => "badge bg-warning text-dark",
        PriorityLevel.Urgent => "badge bg-danger",
        _ => "badge bg-light text-dark"
    };

    private static string GetStatusBadgeClass(RequestStatus status) => status switch
    {
        RequestStatus.Submitted => "badge bg-primary",
        RequestStatus.UnderReview => "badge bg-info text-dark",
        RequestStatus.Assigned => "badge bg-secondary",
        RequestStatus.InProgress => "badge bg-warning text-dark",
        RequestStatus.WaitingForCustomer => "badge bg-dark",
        RequestStatus.Resolved => "badge bg-success",
        RequestStatus.Closed => "badge bg-secondary",
        RequestStatus.Rejected => "badge bg-danger",
        _ => "badge bg-light text-dark"
    };

    private static string FormatStatusText(RequestStatus status) => status switch
    {
        RequestStatus.Submitted => "Submitted",
        RequestStatus.UnderReview => "Under Review",
        RequestStatus.Assigned => "Assigned",
        RequestStatus.InProgress => "In Progress",
        RequestStatus.WaitingForCustomer => "Waiting for Customer",
        RequestStatus.Resolved => "Resolved",
        RequestStatus.Closed => "Closed",
        RequestStatus.Rejected => "Rejected",
        _ => status.ToString()
    };
}
