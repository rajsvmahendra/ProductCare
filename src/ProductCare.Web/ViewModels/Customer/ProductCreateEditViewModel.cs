using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.ViewModels.Customer;

public class ProductCreateEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(150, ErrorMessage = "Product name cannot exceed 150 characters.")]
    [Display(Name = "Product Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Brand / Manufacturer is required.")]
    [StringLength(100, ErrorMessage = "Brand cannot exceed 100 characters.")]
    public string Brand { get; set; } = string.Empty;

    [Required(ErrorMessage = "Model number is required.")]
    [StringLength(100, ErrorMessage = "Model number cannot exceed 100 characters.")]
    [Display(Name = "Model Number")]
    public string ModelNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Serial number is required.")]
    [StringLength(100, ErrorMessage = "Serial number cannot exceed 100 characters.")]
    [Display(Name = "Serial Number")]
    public string SerialNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a product category.")]
    [Display(Name = "Category")]
    public ProductCategory Category { get; set; } = ProductCategory.Electronics;

    [Display(Name = "Subcategory / Form Factor")]
    [StringLength(100, ErrorMessage = "Subcategory cannot exceed 100 characters.")]
    public string? SubCategory { get; set; }

    [Required(ErrorMessage = "Purchase date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Purchase Date")]
    public DateOnly PurchaseDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [Required(ErrorMessage = "Warranty duration is required.")]
    [Range(1, 120, ErrorMessage = "Warranty duration must be between 1 and 120 months.")]
    [Display(Name = "Warranty Duration (Months)")]
    public int WarrantyDurationMonths { get; set; } = 12;

    [StringLength(150, ErrorMessage = "Store name cannot exceed 150 characters.")]
    [Display(Name = "Store / Vendor")]
    public string? PurchaseStore { get; set; }

    [Range(0, 1000000, ErrorMessage = "Purchase price must be a positive value.")]
    [Display(Name = "Purchase Price")]
    [DataType(DataType.Currency)]
    public decimal? PurchasePrice { get; set; }

    [Display(Name = "Invoice Attachment (PDF, JPG, PNG - Max 5 MB)")]
    public IFormFile? InvoiceFile { get; set; }

    public string? ExistingInvoiceOriginalFileName { get; set; }

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
    public string? Notes { get; set; }
}
