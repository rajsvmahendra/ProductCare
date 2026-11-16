using ProductCare.Web.Models.Enums;
using ProductCare.Web.Services;
using Xunit;

namespace ProductCare.Tests;

public class WarrantyCalculatorServiceTests
{
    private readonly WarrantyCalculatorService _service = new();

    [Theory]
    [InlineData("2026-01-15", 12, "2027-01-15")]
    [InlineData("2026-06-01", 24, "2028-06-01")]
    [InlineData("2026-03-10", 6, "2026-09-10")]
    public void CalculateExpiryDate_WithStandardDuration_CalculatesCorrectDate(string purchaseDateStr, int durationMonths, string expectedExpiryStr)
    {
        // Arrange
        var purchaseDate = DateOnly.Parse(purchaseDateStr);
        var expectedExpiry = DateOnly.Parse(expectedExpiryStr);

        // Act
        var result = _service.CalculateExpiryDate(purchaseDate, durationMonths);

        // Assert
        Assert.Equal(expectedExpiry, result);
    }

    [Fact]
    public void CalculateExpiryDate_WithMonthEndDate_HandlesMonthEndCorrectly()
    {
        // Jan 31 + 1 month in non-leap year 2025 -> Feb 28
        var purchaseDate2025 = new DateOnly(2025, 1, 31);
        var result2025 = _service.CalculateExpiryDate(purchaseDate2025, 1);
        Assert.Equal(new DateOnly(2025, 2, 28), result2025);

        // Jan 31 + 1 month in leap year 2024 -> Feb 29
        var purchaseDate2024 = new DateOnly(2024, 1, 31);
        var result2024 = _service.CalculateExpiryDate(purchaseDate2024, 1);
        Assert.Equal(new DateOnly(2024, 2, 29), result2024);

        // Aug 31 + 1 month -> Sep 30
        var purchaseDateAug = new DateOnly(2026, 8, 31);
        var resultSep = _service.CalculateExpiryDate(purchaseDateAug, 1);
        Assert.Equal(new DateOnly(2026, 9, 30), resultSep);
    }

    [Fact]
    public void CalculateExpiryDate_WithNegativeDuration_ThrowsArgumentOutOfRangeException()
    {
        var purchaseDate = new DateOnly(2026, 1, 1);
        Assert.Throws<ArgumentOutOfRangeException>(() => _service.CalculateExpiryDate(purchaseDate, -1));
    }

    [Fact]
    public void EvaluateStatus_WhenMoreThan30DaysRemaining_ReturnsActive()
    {
        var today = new DateOnly(2026, 6, 1);
        var expiryDate = today.AddDays(31); // 31 days away

        var status = _service.EvaluateStatus(expiryDate, today);

        Assert.Equal(WarrantyStatus.Active, status);
    }

    [Fact]
    public void EvaluateStatus_WhenExactly30DaysRemaining_ReturnsExpiringSoon()
    {
        var today = new DateOnly(2026, 6, 1);
        var expiryDate = today.AddDays(30); // Boundary threshold

        var status = _service.EvaluateStatus(expiryDate, today);

        Assert.Equal(WarrantyStatus.ExpiringSoon, status);
    }

    [Fact]
    public void EvaluateStatus_When15DaysRemaining_ReturnsExpiringSoon()
    {
        var today = new DateOnly(2026, 6, 1);
        var expiryDate = today.AddDays(15);

        var status = _service.EvaluateStatus(expiryDate, today);

        Assert.Equal(WarrantyStatus.ExpiringSoon, status);
    }

    [Fact]
    public void EvaluateStatus_WhenExpiringToday_ReturnsExpiringSoon()
    {
        var today = new DateOnly(2026, 6, 1);
        var expiryDate = today; // 0 remaining days; today is the last valid warranty date

        var status = _service.EvaluateStatus(expiryDate, today);

        Assert.Equal(WarrantyStatus.ExpiringSoon, status);
    }

    [Fact]
    public void EvaluateStatus_WhenExpiredYesterday_ReturnsExpired()
    {
        var today = new DateOnly(2026, 6, 2);
        var expiryDate = new DateOnly(2026, 6, 1); // 1 day in the past

        var status = _service.EvaluateStatus(expiryDate, today);

        Assert.Equal(WarrantyStatus.Expired, status);
    }

    [Fact]
    public void EvaluateStatus_WhenExpiredLongAgo_ReturnsExpired()
    {
        var today = new DateOnly(2026, 6, 1);
        var expiryDate = today.AddDays(-100);

        var status = _service.EvaluateStatus(expiryDate, today);

        Assert.Equal(WarrantyStatus.Expired, status);
    }

    [Fact]
    public void CalculateRemainingDays_ReturnsExactDifference()
    {
        var today = new DateOnly(2026, 6, 1);
        var expiryDate = new DateOnly(2026, 6, 15);

        var remaining = _service.CalculateRemainingDays(expiryDate, today);

        Assert.Equal(14, remaining);
    }

    [Theory]
    [InlineData(60, WarrantyStatus.Active, "Active", "badge bg-success")]
    [InlineData(20, WarrantyStatus.ExpiringSoon, "Expiring Soon", "badge bg-warning text-dark")]
    [InlineData(-5, WarrantyStatus.Expired, "Expired", "badge bg-danger")]
    public void GetWarrantyDetails_ReturnsCorrectCssBadgeAndText(int offsetDays, WarrantyStatus expectedStatus, string expectedText, string expectedClass)
    {
        var today = new DateOnly(2026, 6, 1);
        var expiryDate = today.AddDays(offsetDays);

        var details = _service.GetWarrantyDetails(expiryDate, today);

        Assert.Equal(expectedStatus, details.Status);
        Assert.Equal(expectedText, details.StatusDisplayText);
        Assert.Equal(expectedClass, details.StatusBadgeCssClass);
        Assert.Equal(offsetDays, details.RemainingDays);
    }
}
