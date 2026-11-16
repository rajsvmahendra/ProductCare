namespace ProductCare.Web.Models.Entities;

/// <summary>
/// Result DTO returned by the stored procedure 'sp_GetServiceTicketStatistics'.
/// Aggregates ticket volume by category and operational statuses.
/// </summary>
public class ServiceTicketStatisticsDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int TotalTickets { get; set; }
    public int SubmittedCount { get; set; }
    public int UnderReviewCount { get; set; }
    public int AssignedCount { get; set; }
    public int InProgressCount { get; set; }
    public int WaitingForCustomerCount { get; set; }
    public int ResolvedCount { get; set; }
    public int ClosedCount { get; set; }
    public int RejectedCount { get; set; }
}
