using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.Services;

/// <summary>
/// Service Level Agreement (SLA) calculation and escalation engine.
/// EduRev P10 — M3 SLA & Escalation.
/// </summary>
public interface ISlaCalculationService
{
    /// <summary>
    /// Calculates target resolution deadline based on submission time and priority level.
    /// Urgent: 4 hours, High: 24 hours, Medium: 48 hours, Low: 72 hours.
    /// </summary>
    DateTime CalculateSlaDeadline(DateTime createdAtUtc, PriorityLevel priority);

    /// <summary>
    /// Evaluates whether an issue has breached its SLA target.
    /// </summary>
    bool IsOverdue(DateTime? slaDeadlineUtc, DateTime? resolvedAtUtc = null);

    /// <summary>
    /// Checks if the issue is within 4 hours of breaching SLA and not yet resolved.
    /// </summary>
    bool IsApproachingBreach(DateTime? slaDeadlineUtc, DateTime? resolvedAtUtc = null);

    /// <summary>
    /// Returns remaining time span until deadline or overdue delta.
    /// </summary>
    TimeSpan? GetRemainingTime(DateTime? slaDeadlineUtc);

    /// <summary>
    /// Returns formatted human-readable badge text and CSS class for SLA status.
    /// </summary>
    (string Text, string BadgeCssClass) GetSlaStatusBadge(DateTime? slaDeadlineUtc, DateTime? resolvedAtUtc = null);

    /// <summary>
    /// Suggests responsible team or technician based on category and campus facility.
    /// EduRev P10 — M2 Routing and Assignment.
    /// </summary>
    string GetRecommendedTechnician(RequestCategory category, string? campusLocation);
}
