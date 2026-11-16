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

    // Navigation properties
    public virtual Product Product { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;

    public virtual ICollection<ServiceRequestHistory> StatusHistories { get; set; } = new List<ServiceRequestHistory>();
}
