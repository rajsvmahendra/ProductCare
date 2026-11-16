namespace ProductCare.Web.ViewModels.Admin;

public class AdminDashboardViewModel
{
    // Aggregate system metrics
    public int TotalCustomers { get; set; }
    public int TotalProducts { get; set; }
    public int TotalServiceRequests { get; set; }
    public int PendingRequests { get; set; }      // Submitted, UnderReview
    public int InProgressRequests { get; set; }   // Assigned, InProgress, WaitingForCustomer
    public int ResolvedRequests { get; set; }     // Resolved
    public int ClosedRequests { get; set; }       // Closed
    public int RejectedRequests { get; set; }     // Rejected

    public List<AdminRecentTicketItem> RecentTickets { get; set; } = new();
}

public class AdminRecentTicketItem
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string CategoryText { get; set; } = string.Empty;
    public string PriorityText { get; set; } = string.Empty;
    public string PriorityBadgeCssClass { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
    public string StatusBadgeCssClass { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
