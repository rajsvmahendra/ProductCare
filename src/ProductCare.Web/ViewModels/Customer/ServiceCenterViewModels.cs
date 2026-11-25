namespace ProductCare.Web.ViewModels.Customer;

public class ServiceCenterListViewModel
{
    public List<ServiceCenterCardViewModel> Centers { get; set; } = new();

    public string? SearchTerm { get; set; }
    public string? FilterCity { get; set; }
    public string? FilterLocality { get; set; }
    public string? FilterBrand { get; set; }
    public string? FilterCategory { get; set; }

    public List<string> AvailableCities { get; set; } = new();
    public List<string> AvailableLocalities { get; set; } = new();
    public int TotalCount => Centers.Count;
}

public class ServiceCenterCardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Locality { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? Country { get; set; }
    public string SupportedBrands { get; set; } = string.Empty;
    public string SupportedCategories { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string OpeningHours { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? MapUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public string? Notes { get; set; }
    public int OpenTicketCount { get; set; }
}
