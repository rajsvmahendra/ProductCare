using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.ViewModels.Customer;

public class ServiceRequestDetailsViewModel
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public RequestCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public PriorityLevel Priority { get; set; }
    public string PriorityBadgeCssClass { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    public string StatusBadgeCssClass { get; set; } = string.Empty;
    public string StatusDisplayText { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string? AssignedTechnician { get; set; }
    public string? AdminRemarks { get; set; }

    public string? CampusLocation { get; set; }
    public DateTime? SlaDeadlineUtc { get; set; }
    public bool IsOverdue { get; set; }
    public bool IsApproachingBreach { get; set; }
    public bool IsEscalated { get; set; }
    public string SlaBadgeText { get; set; } = string.Empty;
    public string SlaBadgeCssClass { get; set; } = "badge bg-secondary";
    public string? AttachmentFileName { get; set; }
    public string? ResolutionProofFileName { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }

    // Associated Product details
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductBrand { get; set; } = string.Empty;
    public string ProductModelNumber { get; set; } = string.Empty;
    public string ProductSerialNumber { get; set; } = string.Empty;
    public DateOnly ProductWarrantyExpiryDate { get; set; }
    public string ProductWarrantyStatusBadge { get; set; } = string.Empty;
    public string ProductWarrantyStatusText { get; set; } = string.Empty;

    // Associated Service Centre (Optional)
    public int? ServiceCenterId { get; set; }
    public string? ServiceCenterName { get; set; }
    public string? ServiceCenterAddress { get; set; }
    public string? ServiceCenterCity { get; set; }
    public string? ServiceCenterPhone { get; set; }
    public bool? ServiceCenterIsVerified { get; set; }

    // Permissions
    public bool CanCancel => Status == RequestStatus.Submitted;

    // Chronological status audit history
    public List<ServiceRequestHistoryItem> StatusHistory { get; set; } = new();
}

public class ServiceRequestHistoryItem
{
    public int Id { get; set; }
    public RequestStatus FromStatus { get; set; }
    public string FromStatusText { get; set; } = string.Empty;
    public RequestStatus ToStatus { get; set; }
    public string ToStatusText { get; set; } = string.Empty;
    public string ToStatusBadgeCssClass { get; set; } = string.Empty;
    public string ChangedByUserName { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
