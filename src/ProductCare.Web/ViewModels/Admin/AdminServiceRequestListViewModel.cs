using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.ViewModels.Admin;

public class AdminServiceRequestListViewModel
{
    public List<AdminTicketListItem> Tickets { get; set; } = new();

    public RequestStatus? FilterStatus { get; set; }
    public PriorityLevel? FilterPriority { get; set; }
    public RequestCategory? FilterCategory { get; set; }
    public string? FilterLocation { get; set; }
    public bool? FilterOverdueOnly { get; set; }
    public string? SearchTerm { get; set; }

    public int TotalTickets { get; set; }
    public int PendingCount { get; set; }
    public int InProgressCount { get; set; }
    public int ResolvedCount { get; set; }
    public int OverdueCount { get; set; }
}

public class AdminTicketListItem
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public int ProductId { get; set; }
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
    public string? AssignedTechnician { get; set; }
    public string? CampusLocation { get; set; }
    public string? ServiceCenterName { get; set; }
    public DateTime? SlaDeadlineUtc { get; set; }
    public bool IsOverdue { get; set; }
    public bool IsApproachingBreach { get; set; }
    public bool IsEscalated { get; set; }
    public string SlaBadgeText { get; set; } = string.Empty;
    public string SlaBadgeCssClass { get; set; } = "badge bg-secondary";
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
