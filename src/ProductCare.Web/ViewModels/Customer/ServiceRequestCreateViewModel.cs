using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.ViewModels.Customer;

public class ServiceRequestCreateViewModel
{
    [Required(ErrorMessage = "Please select a registered product.")]
    [Display(Name = "Product")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Please select a request category.")]
    [Display(Name = "Request Category")]
    public RequestCategory Category { get; set; } = RequestCategory.Repair;

    [Required(ErrorMessage = "Please select a priority level.")]
    [Display(Name = "Priority Level")]
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

    [Required(ErrorMessage = "Subject is required.")]
    [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters.")]
    [Display(Name = "Subject / Issue Summary")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Detailed description of the issue is required.")]
    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    [Display(Name = "Issue Description")]
    public string Description { get; set; } = string.Empty;

    // Dropdown list items populated from the customer's products
    public List<SelectListItem> AvailableProducts { get; set; } = new();
}
