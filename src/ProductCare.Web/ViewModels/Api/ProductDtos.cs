using System.ComponentModel.DataAnnotations;
using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.ViewModels.Api;

public class ProductDto
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
    public int RemainingDays { get; set; }
    public WarrantyStatus WarrantyStatus { get; set; }
    public string WarrantyStatusText { get; set; } = string.Empty;
    public string? PurchaseStore { get; set; }
    public decimal? PurchasePrice { get; set; }
    public bool HasInvoice { get; set; }
    public string? InvoiceOriginalFileName { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class CreateProductDto
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ModelNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string SerialNumber { get; set; } = string.Empty;

    [Required]
    public ProductCategory Category { get; set; } = ProductCategory.Electronics;

    [Required]
    public DateOnly PurchaseDate { get; set; }

    [Required]
    [Range(1, 120)]
    public int WarrantyDurationMonths { get; set; } = 12;

    [StringLength(150)]
    public string? PurchaseStore { get; set; }

    [Range(0, 1000000)]
    public decimal? PurchasePrice { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}

public class UpdateProductDto
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ModelNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string SerialNumber { get; set; } = string.Empty;

    [Required]
    public ProductCategory Category { get; set; }

    [Required]
    public DateOnly PurchaseDate { get; set; }

    [Required]
    [Range(1, 120)]
    public int WarrantyDurationMonths { get; set; }

    [StringLength(150)]
    public string? PurchaseStore { get; set; }

    [Range(0, 1000000)]
    public decimal? PurchasePrice { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
