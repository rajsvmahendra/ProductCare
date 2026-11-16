using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.Models.Entities;

/// <summary>
/// Immutable audit log entry recording every status transition on a service ticket.
/// </summary>
public class ServiceRequestHistory
{
    public int Id { get; set; }

    public int ServiceRequestId { get; set; }

    public RequestStatus FromStatus { get; set; }

    public RequestStatus ToStatus { get; set; }

    public string ChangedByUserId { get; set; } = string.Empty;

    public string ChangedByUserName { get; set; } = string.Empty;

    public string? Remarks { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ServiceRequest ServiceRequest { get; set; } = null!;

    public virtual ApplicationUser ChangedByUser { get; set; } = null!;
}
