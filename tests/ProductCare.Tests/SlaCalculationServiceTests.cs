using ProductCare.Web.Models.Enums;
using ProductCare.Web.Services;
using Xunit;

namespace ProductCare.Tests;

public class SlaCalculationServiceTests
{
    private readonly SlaCalculationService _service = new();

    [Fact]
    public void CalculateSlaDeadline_Urgent_Returns4Hours()
    {
        var now = new DateTime(2026, 11, 18, 10, 0, 0, DateTimeKind.Utc);
        var deadline = _service.CalculateSlaDeadline(now, PriorityLevel.Urgent);

        Assert.Equal(now.AddHours(4), deadline);
    }

    [Fact]
    public void CalculateSlaDeadline_High_Returns24Hours()
    {
        var now = new DateTime(2026, 11, 18, 10, 0, 0, DateTimeKind.Utc);
        var deadline = _service.CalculateSlaDeadline(now, PriorityLevel.High);

        Assert.Equal(now.AddHours(24), deadline);
    }

    [Fact]
    public void CalculateSlaDeadline_Medium_Returns48Hours()
    {
        var now = new DateTime(2026, 11, 18, 10, 0, 0, DateTimeKind.Utc);
        var deadline = _service.CalculateSlaDeadline(now, PriorityLevel.Medium);

        Assert.Equal(now.AddHours(48), deadline);
    }

    [Fact]
    public void CalculateSlaDeadline_Low_Returns72Hours()
    {
        var now = new DateTime(2026, 11, 18, 10, 0, 0, DateTimeKind.Utc);
        var deadline = _service.CalculateSlaDeadline(now, PriorityLevel.Low);

        Assert.Equal(now.AddHours(72), deadline);
    }

    [Fact]
    public void IsOverdue_ActiveTicketPastDeadline_ReturnsTrue()
    {
        var pastDeadline = DateTime.UtcNow.AddHours(-2);
        Assert.True(_service.IsOverdue(pastDeadline));
    }

    [Fact]
    public void IsOverdue_ActiveTicketBeforeDeadline_ReturnsFalse()
    {
        var futureDeadline = DateTime.UtcNow.AddHours(12);
        Assert.False(_service.IsOverdue(futureDeadline));
    }

    [Fact]
    public void IsOverdue_ResolvedTicketBeforeDeadline_ReturnsFalse()
    {
        var deadline = new DateTime(2026, 11, 18, 18, 0, 0, DateTimeKind.Utc);
        var resolvedAt = new DateTime(2026, 11, 18, 14, 0, 0, DateTimeKind.Utc);

        Assert.False(_service.IsOverdue(deadline, resolvedAt));
    }

    [Fact]
    public void IsOverdue_ResolvedTicketAfterDeadline_ReturnsTrue()
    {
        var deadline = new DateTime(2026, 11, 18, 12, 0, 0, DateTimeKind.Utc);
        var resolvedAt = new DateTime(2026, 11, 18, 15, 0, 0, DateTimeKind.Utc);

        Assert.True(_service.IsOverdue(deadline, resolvedAt));
    }

    [Fact]
    public void IsApproachingBreach_Within4Hours_ReturnsTrue()
    {
        var deadline = DateTime.UtcNow.AddHours(2);
        Assert.True(_service.IsApproachingBreach(deadline));
    }

    [Fact]
    public void IsApproachingBreach_Over4Hours_ReturnsFalse()
    {
        var deadline = DateTime.UtcNow.AddHours(10);
        Assert.False(_service.IsApproachingBreach(deadline));
    }

    [Theory]
    [InlineData(RequestCategory.Repair, "Hostel Block B", "Hostel")]
    [InlineData(RequestCategory.Maintenance, "Computer Science Lab 3", "Hardware")]
    [InlineData(RequestCategory.Installation, "Main Academic Block", "Infrastructure")]
    public void GetRecommendedTechnician_ReturnsSensibleDispatchUnit(RequestCategory category, string location, string expectedSubstring)
    {
        var recommendation = _service.GetRecommendedTechnician(category, location);
        Assert.Contains(expectedSubstring, recommendation, StringComparison.OrdinalIgnoreCase);
    }
}
