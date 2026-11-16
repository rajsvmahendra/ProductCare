namespace ProductCare.Web.ViewModels.Admin;

public class AdminCustomerListViewModel
{
    public List<AdminCustomerListItem> Customers { get; set; } = new();
    public string? SearchTerm { get; set; }
    public int TotalCustomers => Customers.Count;
}

public class AdminCustomerListItem
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? City { get; set; }
    public int RegisteredProductsCount { get; set; }
    public int TotalServiceRequestsCount { get; set; }
    public int OpenServiceRequestsCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
