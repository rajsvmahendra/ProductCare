using ProductCare.Web.Models.Entities;

namespace ProductCare.Web.ViewModels.Customer;

public class CustomerDashboardViewModel
{
    public string CustomerName { get; set; } = string.Empty;

    // Metric counters
    public int TotalProducts { get; set; }
    public int ActiveWarranties { get; set; }
    public int ExpiringSoonWarranties { get; set; }
    public int ExpiredWarranties { get; set; }
    public int OpenServiceRequests { get; set; }
    public int ResolvedServiceRequests { get; set; }

    // Lists
    public List<ProductSummaryItem> RecentProducts { get; set; } = new();
    public List<ProductSummaryItem> ExpiringSoonProducts { get; set; } = new();
    public List<ServiceRequestSummaryItem> RecentServiceRequests { get; set; } = new();
}

public class ProductSummaryItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string ModelNumber { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public DateOnly ExpiryDate { get; set; }
    public int RemainingDays { get; set; }
    public string StatusBadgeCssClass { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
}

public class ServiceRequestSummaryItem
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? ServiceCenterName { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string PriorityText { get; set; } = string.Empty;
    public string PriorityBadgeCssClass { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
    public string StatusBadgeCssClass { get; set; } = string.Empty;
    public string NextActionText { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
