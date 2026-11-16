using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.Services;

public record WarrantyDetails(
    DateOnly ExpiryDate,
    int RemainingDays,
    WarrantyStatus Status,
    string StatusBadgeCssClass,
    string StatusDisplayText
);

public interface IWarrantyCalculatorService
{
    /// <summary>
    /// Calculates the exact warranty expiry date using PurchaseDate.AddMonths(durationMonths).
    /// </summary>
    DateOnly CalculateExpiryDate(DateOnly purchaseDate, int durationMonths);

    /// <summary>
    /// Determines the warranty lifecycle status (Active, ExpiringSoon, Expired) relative to the asOf date.
    /// </summary>
    WarrantyStatus EvaluateStatus(DateOnly expiryDate, DateOnly? asOfDate = null);

    /// <summary>
    /// Calculates remaining days until warranty expiration (negative if already expired).
    /// </summary>
    int CalculateRemainingDays(DateOnly expiryDate, DateOnly? asOfDate = null);

    /// <summary>
    /// Computes full warranty details including display text and Bootstrap CSS badge class.
    /// </summary>
    WarrantyDetails GetWarrantyDetails(DateOnly purchaseDate, int durationMonths, DateOnly? asOfDate = null);

    /// <summary>
    /// Computes full warranty details directly from an existing expiry date.
    /// </summary>
    WarrantyDetails GetWarrantyDetails(DateOnly expiryDate, DateOnly? asOfDate = null);
}
