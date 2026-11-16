using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.ViewModels.Admin;

public class AdminCustomerDetailsViewModel
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public List<AdminCustomerProductItem> Products { get; set; } = new();
    public List<AdminCustomerTicketItem> ServiceRequests { get; set; } = new();
}

public class AdminCustomerProductItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string ModelNumber { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public DateOnly PurchaseDate { get; set; }
    public DateOnly WarrantyExpiryDate { get; set; }
    public int RemainingDays { get; set; }
    public string StatusBadgeCssClass { get; set; } = string.Empty;
    public string StatusDisplayText { get; set; } = string.Empty;
    public bool HasInvoice { get; set; }
}

public class AdminCustomerTicketItem
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public RequestCategory Category { get; set; }
    public PriorityLevel Priority { get; set; }
    public string PriorityBadgeCssClass { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    public string StatusBadgeCssClass { get; set; } = string.Empty;
    public string StatusDisplayText { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
