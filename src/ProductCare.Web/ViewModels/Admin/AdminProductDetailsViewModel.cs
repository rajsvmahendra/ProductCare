using ProductCare.Web.Models.Enums;
using ProductCare.Web.Services;
using ProductCare.Web.ViewModels.Customer;

namespace ProductCare.Web.ViewModels.Admin;

public class AdminProductDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string ModelNumber { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public ProductCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public DateOnly PurchaseDate { get; set; }
    public int WarrantyDurationMonths { get; set; }
    public DateOnly WarrantyExpiryDate { get; set; }
    public string? PurchaseStore { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    // Customer owner info
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public string? CustomerCity { get; set; }

    // Calculated warranty details
    public WarrantyDetails Warranty { get; set; } = null!;

    // Invoice info
    public bool HasInvoice => !string.IsNullOrEmpty(InvoiceFileName);
    public string? InvoiceFileName { get; set; }
    public string? InvoiceOriginalFileName { get; set; }
    public string? InvoiceContentType { get; set; }
    public long? InvoiceFileSize { get; set; }

    // Service requests for this product
    public List<ProductServiceRequestItem> ServiceRequests { get; set; } = new();
}
