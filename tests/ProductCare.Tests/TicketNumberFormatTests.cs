using System.Text.RegularExpressions;
using Xunit;

namespace ProductCare.Tests;

public class TicketNumberFormatTests
{
    private static readonly Regex TicketPattern = new(@"^PC-\d{8}-[A-Z0-9]{4}$", RegexOptions.Compiled);

    [Theory]
    [InlineData("PC-20261116-A7B2", true)]
    [InlineData("PC-20260101-9999", true)]
    [InlineData("PC-20251231-ZZZZ", true)]
    [InlineData("PC-2026111-A7B2", false)]  // 7 digits date
    [InlineData("PC-20261116-A7B", false)]   // 3 chars suffix
    [InlineData("XX-20261116-A7B2", false)]  // Wrong prefix
    [InlineData("PC-20261116-a7b2", false)]  // Lowercase suffix
    public void TicketNumber_MatchesExpectedEnterprisePattern(string ticketNumber, bool shouldMatch)
    {
        var isMatch = TicketPattern.IsMatch(ticketNumber);
        Assert.Equal(shouldMatch, isMatch);
    }
}
