using ProductCare.Web.Models.Enums;
using ProductCare.Web.Services;

namespace ProductCare.Web.ViewModels.Customer;

public class ProductDetailsViewModel
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

    // Warranty details calculated centrally
    public WarrantyDetails Warranty { get; set; } = null!;

    // Invoice details
    public bool HasInvoice => !string.IsNullOrEmpty(InvoiceFileName);
    public string? InvoiceFileName { get; set; }
    public string? InvoiceOriginalFileName { get; set; }
    public string? InvoiceContentType { get; set; }
    public long? InvoiceFileSize { get; set; }

    // Service requests for this product
    public List<ProductServiceRequestItem> ServiceRequests { get; set; } = new();
}

public class ProductServiceRequestItem
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public RequestCategory Category { get; set; }
    public PriorityLevel Priority { get; set; }
    public RequestStatus Status { get; set; }
    public string StatusBadgeCssClass { get; set; } = string.Empty;
    public string PriorityBadgeCssClass { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
