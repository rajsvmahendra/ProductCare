using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.ViewModels.Customer;

public class WarrantyOverviewViewModel
{
    public int ActiveCount { get; set; }
    public int ExpiringSoonCount { get; set; }
    public int ExpiredCount { get; set; }
    public int TotalProductsCount { get; set; }

    public WarrantyStatus? FilterStatus { get; set; }

    public List<WarrantyTimelineItem> WarrantyItems { get; set; } = new();
}

public class WarrantyTimelineItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string ModelNumber { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public DateOnly PurchaseDate { get; set; }
    public int WarrantyDurationMonths { get; set; }
    public DateOnly WarrantyExpiryDate { get; set; }
    public int RemainingDays { get; set; }
    public WarrantyStatus Status { get; set; }
    public string StatusBadgeCssClass { get; set; } = string.Empty;
    public string StatusDisplayText { get; set; } = string.Empty;
}
