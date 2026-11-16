using ProductCare.Web.Models.Enums;
using ProductCare.Web.Services;

namespace ProductCare.Web.ViewModels.Admin;

public class AdminProductListViewModel
{
    public List<AdminProductListItem> Products { get; set; } = new();
    public ProductCategory? FilterCategory { get; set; }
    public WarrantyStatus? FilterStatus { get; set; }
    public string? SearchTerm { get; set; }
    public int TotalProducts => Products.Count;
}

public class AdminProductListItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string ModelNumber { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public ProductCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateOnly PurchaseDate { get; set; }
    public DateOnly WarrantyExpiryDate { get; set; }
    public int RemainingDays { get; set; }
    public WarrantyStatus WarrantyStatus { get; set; }
    public string StatusBadgeCssClass { get; set; } = string.Empty;
    public string StatusDisplayText { get; set; } = string.Empty;
    public int ServiceRequestsCount { get; set; }
}
