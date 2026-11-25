using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.ViewModels.Customer;

public class ServiceRequestListViewModel
{
    public List<CustomerServiceRequestListItem> Requests { get; set; } = new();

    public RequestStatus? FilterStatus { get; set; }
    public PriorityLevel? FilterPriority { get; set; }
    public RequestCategory? FilterCategory { get; set; }
    public string? SearchTerm { get; set; }

    public int TotalCount => Requests.Count;
}

public class CustomerServiceRequestListItem
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProductBrand { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public RequestCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public PriorityLevel Priority { get; set; }
    public string PriorityBadgeCssClass { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    public string StatusBadgeCssClass { get; set; } = string.Empty;
    public string StatusDisplayText { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? CampusLocation { get; set; }
    public string? ServiceCenterName { get; set; }
    public DateTime? SlaDeadlineUtc { get; set; }
    public bool IsOverdue { get; set; }
    public bool IsEscalated { get; set; }
    public string SlaBadgeText { get; set; } = string.Empty;
    public string SlaBadgeCssClass { get; set; } = "badge bg-secondary";
}
