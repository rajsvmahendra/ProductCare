using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.Services;

/// <summary>
/// Implements deterministic SLA calculations, countdown telemetry, and automated campus dispatch routing.
/// EduRev P10 — M2 Routing & Assignment and M3 SLA & Escalation.
/// </summary>
public class SlaCalculationService : ISlaCalculationService
{
    // SLA Targets by priority level
    public static readonly TimeSpan SlaUrgent = TimeSpan.FromHours(4);
    public static readonly TimeSpan SlaHigh = TimeSpan.FromHours(24);
    public static readonly TimeSpan SlaMedium = TimeSpan.FromHours(48);
    public static readonly TimeSpan SlaLow = TimeSpan.FromHours(72);

    private static readonly TimeSpan ApproachingBreachThreshold = TimeSpan.FromHours(4);

    public DateTime CalculateSlaDeadline(DateTime createdAtUtc, PriorityLevel priority) => priority switch
    {
        PriorityLevel.Urgent => createdAtUtc.Add(SlaUrgent),
        PriorityLevel.High => createdAtUtc.Add(SlaHigh),
        PriorityLevel.Medium => createdAtUtc.Add(SlaMedium),
        PriorityLevel.Low => createdAtUtc.Add(SlaLow),
        _ => createdAtUtc.Add(SlaMedium)
    };

    public bool IsOverdue(DateTime? slaDeadlineUtc, DateTime? resolvedAtUtc = null)
    {
        if (!slaDeadlineUtc.HasValue)
        {
            return false;
        }

        if (resolvedAtUtc.HasValue)
        {
            return resolvedAtUtc.Value > slaDeadlineUtc.Value;
        }

        return DateTime.UtcNow > slaDeadlineUtc.Value;
    }

    public bool IsApproachingBreach(DateTime? slaDeadlineUtc, DateTime? resolvedAtUtc = null)
    {
        if (!slaDeadlineUtc.HasValue || resolvedAtUtc.HasValue)
        {
            return false;
        }

        var utcNow = DateTime.UtcNow;
        if (utcNow > slaDeadlineUtc.Value)
        {
            return false; // Already overdue
        }

        return (slaDeadlineUtc.Value - utcNow) <= ApproachingBreachThreshold;
    }

    public TimeSpan? GetRemainingTime(DateTime? slaDeadlineUtc)
    {
        if (!slaDeadlineUtc.HasValue)
        {
            return null;
        }

        return slaDeadlineUtc.Value - DateTime.UtcNow;
    }

    public (string Text, string BadgeCssClass) GetSlaStatusBadge(DateTime? slaDeadlineUtc, DateTime? resolvedAtUtc = null)
    {
        if (!slaDeadlineUtc.HasValue)
        {
            return ("No SLA Target", "badge bg-secondary");
        }

        if (resolvedAtUtc.HasValue)
        {
            if (resolvedAtUtc.Value <= slaDeadlineUtc.Value)
            {
                return ("Resolved within SLA", "badge bg-success");
            }

            return ("Resolved after SLA Breach", "badge bg-warning text-dark");
        }

        var remaining = slaDeadlineUtc.Value - DateTime.UtcNow;

        if (remaining <= TimeSpan.Zero)
        {
            var overdue = remaining.Duration();
            var overdueText = overdue.TotalHours >= 24
                ? $"Overdue by {(int)overdue.TotalDays}d {overdue.Hours}h"
                : $"Overdue by {(int)overdue.TotalHours}h {overdue.Minutes}m";

            return (overdueText, "badge bg-danger");
        }

        if (remaining <= ApproachingBreachThreshold)
        {
            return ($"Approaching SLA ({(int)remaining.TotalHours}h {remaining.Minutes}m left)", "badge bg-warning text-dark");
        }

        var remainingText = remaining.TotalHours >= 24
            ? $"Within SLA ({(int)remaining.TotalDays}d {remaining.Hours}h left)"
            : $"Within SLA ({(int)remaining.TotalHours}h {remaining.Minutes}m left)";

        return (remainingText, "badge bg-success");
    }

    public string GetRecommendedTechnician(RequestCategory category, string? campusLocation)
    {
        var loc = campusLocation?.ToLowerInvariant() ?? string.Empty;

        return category switch
        {
            RequestCategory.Repair => loc.Contains("hostel")
                ? "Campus Hostel Facilities Team (Lead: David Miller)"
                : "Campus Facilities & Electrical Team",

            RequestCategory.Installation => "Campus Infrastructure & Audio/Visual Team",

            RequestCategory.Maintenance => loc.Contains("lab") || loc.Contains("library")
                ? "Campus IT & Lab Hardware Team"
                : "Campus General Maintenance Unit",

            RequestCategory.WarrantyClaim => "Central Asset Management & Vendor Liaison",

            _ => "Campus Facilities Help Desk"
        };
    }
}
