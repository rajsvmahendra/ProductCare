using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.Models.Entities;

/// <summary>
/// Represents a purchased product registered by a customer.
/// </summary>
public class Product
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Brand { get; set; } = string.Empty;

    public string ModelNumber { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;

    public ProductCategory Category { get; set; } = ProductCategory.Other;

    public DateOnly PurchaseDate { get; set; }

    public int WarrantyDurationMonths { get; set; }

    /// <summary>
    /// Calculated expiry date stored for fast querying and database indexing.
    /// Expiry Date = PurchaseDate + WarrantyDurationMonths.
    /// </summary>
    public DateOnly WarrantyExpiryDate { get; set; }

    public string? PurchaseStore { get; set; }

    public decimal? PurchasePrice { get; set; }

    // Invoice storage metadata (stored safely outside wwwroot)
    public string? InvoiceFileName { get; set; }

    public string? InvoiceOriginalFileName { get; set; }

    public string? InvoiceContentType { get; set; }

    public long? InvoiceFileSize { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;

    public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
}
