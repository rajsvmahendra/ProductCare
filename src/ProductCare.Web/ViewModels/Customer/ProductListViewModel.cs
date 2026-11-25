using ProductCare.Web.Models.Enums;
using ProductCare.Web.Services;

namespace ProductCare.Web.ViewModels.Customer;

public class ProductListViewModel
{
    public List<ProductListItemViewModel> Products { get; set; } = new();

    public ProductCategory? FilterCategory { get; set; }
    public WarrantyStatus? FilterStatus { get; set; }
    public string? SearchTerm { get; set; }
    public string? Sort { get; set; }

    public int TotalCount => Products.Count;
}

public class ProductListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string ModelNumber { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public ProductCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public int WarrantyDurationMonths { get; set; }
    public DateOnly WarrantyExpiryDate { get; set; }
    public int RemainingDays { get; set; }
    public WarrantyStatus WarrantyStatus { get; set; }
    public string StatusBadgeCssClass { get; set; } = string.Empty;
    public string StatusDisplayText { get; set; } = string.Empty;
    public bool HasInvoice { get; set; }
    public int ActiveServiceRequestsCount { get; set; }
}
