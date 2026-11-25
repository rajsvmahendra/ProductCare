using System.ComponentModel.DataAnnotations;

namespace ProductCare.Web.ViewModels.Admin;

public class AdminServiceCenterListViewModel
{
    public List<AdminServiceCenterItemViewModel> Centers { get; set; } = new();
    public string? SearchTerm { get; set; }
    public int TotalCount => Centers.Count;
}

public class AdminServiceCenterItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Locality { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string OpeningHours { get; set; } = string.Empty;
    public string SupportedBrands { get; set; } = string.Empty;
    public string SupportedCategories { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public int ActiveTicketCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class AdminServiceCenterCreateEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Center name is required.")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    [Display(Name = "Service Center Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Street address is required.")]
    [StringLength(300, ErrorMessage = "Address cannot exceed 300 characters.")]
    [Display(Name = "Street Address")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required.")]
    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
    public string City { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Locality cannot exceed 100 characters.")]
    [Display(Name = "Locality / Campus Hub")]
    public string? Locality { get; set; }

    [StringLength(100, ErrorMessage = "State cannot exceed 100 characters.")]
    public string? State { get; set; } = "Punjab";

    [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters.")]
    public string? Country { get; set; } = "India";

    [Required(ErrorMessage = "Supported brands list is required.")]
    [StringLength(500, ErrorMessage = "Brands cannot exceed 500 characters.")]
    [Display(Name = "Supported Brands (comma-separated)")]
    public string SupportedBrands { get; set; } = string.Empty;

    [Required(ErrorMessage = "Supported categories list is required.")]
    [StringLength(500, ErrorMessage = "Categories cannot exceed 500 characters.")]
    [Display(Name = "Supported Categories (comma-separated)")]
    public string SupportedCategories { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [StringLength(50, ErrorMessage = "Phone cannot exceed 50 characters.")]
    [Phone]
    [Display(Name = "Telephone / Hotline")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
    [EmailAddress]
    [Display(Name = "Support Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Operating hours are required.")]
    [StringLength(200, ErrorMessage = "Operating hours cannot exceed 200 characters.")]
    [Display(Name = "Operating Hours")]
    public string OpeningHours { get; set; } = "Mon-Fri: 9:00 AM - 6:00 PM";

    [StringLength(255, ErrorMessage = "Website URL cannot exceed 255 characters.")]
    [Url]
    [Display(Name = "Website URL (Optional)")]
    public string? Website { get; set; }

    [StringLength(500, ErrorMessage = "Map URL cannot exceed 500 characters.")]
    [Display(Name = "Google Maps / Directions Link (Optional)")]
    public string? MapUrl { get; set; }

    [Display(Name = "Active for New Tickets")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Officially Verified Partner")]
    public bool IsVerified { get; set; } = false;

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
    [Display(Name = "Internal Operational Notes")]
    public string? Notes { get; set; }
}
