using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.Models.Entities;

/// <summary>
/// Represents a repair, maintenance, or warranty claim request raised against a registered product.
/// </summary>
public class ServiceRequest
{
    public int Id { get; set; }

    /// <summary>
    /// Unique collision-safe ticket number in format PC-YYYYMMDD-XXXX.
    /// </summary>
    public string TicketNumber { get; set; } = string.Empty;

    public int ProductId { get; set; }

    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Optional assigned or selected service centre for repair intake.
    /// </summary>
    public int? ServiceCenterId { get; set; }

    public RequestCategory Category { get; set; } = RequestCategory.Repair;

    public string Subject { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

    public RequestStatus Status { get; set; } = RequestStatus.Submitted;

    public string? AssignedTechnician { get; set; }

    public string? AdminRemarks { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    public DateTime? ResolvedAtUtc { get; set; }

    /// <summary>
    /// P10 Campus Location / Facility where the issue is situated.
    /// E.g. "Main Academic Block, Floor 2, Room 204", "Hostel Block B", "Central Library".
    /// </summary>
    public string? CampusLocation { get; set; }

    /// <summary>
    /// P10 SLA deadline calculated from priority level and submission timestamp.
    /// </summary>
    public DateTime? SlaDeadlineUtc { get; set; }

    /// <summary>
    /// P10 SLA escalation flag indicating high-priority breach or supervisor escalation.
    /// </summary>
    public bool IsEscalated { get; set; } = false;

    /// <summary>
    /// Attached photo or diagnostic log (safe magic-byte storage).
    /// </summary>
    public string? AttachmentFileName { get; set; }

    public string? AttachmentOriginalFileName { get; set; }

    public string? AttachmentContentType { get; set; }

    /// <summary>
    /// Resolution proof photograph or inspection certificate submitted upon completion.
    /// </summary>
    public string? ResolutionProofFileName { get; set; }

    // Navigation properties
    public virtual Product Product { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;

    public virtual ServiceCenter? ServiceCenter { get; set; }

    public virtual ICollection<ServiceRequestHistory> StatusHistories { get; set; } = new List<ServiceRequestHistory>();
}
