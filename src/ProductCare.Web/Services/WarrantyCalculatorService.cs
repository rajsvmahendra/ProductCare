using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.Services;

/// <summary>
/// Centralized service for warranty duration, expiration date, and lifecycle status evaluations.
/// Ensures consistent business rules across controllers, views, background queries, and reports.
/// </summary>
public class WarrantyCalculatorService : IWarrantyCalculatorService
{
    private const int ExpiringSoonThresholdDays = 30;

    public DateOnly CalculateExpiryDate(DateOnly purchaseDate, int durationMonths)
    {
        if (durationMonths < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(durationMonths), "Warranty duration cannot be negative.");
        }

        return purchaseDate.AddMonths(durationMonths);
    }

    public WarrantyStatus EvaluateStatus(DateOnly expiryDate, DateOnly? asOfDate = null)
    {
        var today = asOfDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var remainingDays = expiryDate.DayNumber - today.DayNumber;

        if (remainingDays < 0)
        {
            return WarrantyStatus.Expired;
        }

        if (remainingDays <= ExpiringSoonThresholdDays)
        {
            return WarrantyStatus.ExpiringSoon;
        }

        return WarrantyStatus.Active;
    }

    public int CalculateRemainingDays(DateOnly expiryDate, DateOnly? asOfDate = null)
    {
        var today = asOfDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        return expiryDate.DayNumber - today.DayNumber;
    }

    public WarrantyDetails GetWarrantyDetails(DateOnly purchaseDate, int durationMonths, DateOnly? asOfDate = null)
    {
        var expiryDate = CalculateExpiryDate(purchaseDate, durationMonths);
        return GetWarrantyDetails(expiryDate, asOfDate);
    }

    public WarrantyDetails GetWarrantyDetails(DateOnly expiryDate, DateOnly? asOfDate = null)
    {
        var today = asOfDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var remainingDays = expiryDate.DayNumber - today.DayNumber;
        var status = EvaluateStatus(expiryDate, today);

        var (badgeClass, text) = status switch
        {
            WarrantyStatus.Active => ("badge bg-success", "Active"),
            WarrantyStatus.ExpiringSoon => ("badge bg-warning text-dark", "Expiring Soon"),
            WarrantyStatus.Expired => ("badge bg-danger", "Expired"),
            _ => ("badge bg-secondary", "Unknown")
        };

        return new WarrantyDetails(expiryDate, remainingDays, status, badgeClass, text);
    }
}
