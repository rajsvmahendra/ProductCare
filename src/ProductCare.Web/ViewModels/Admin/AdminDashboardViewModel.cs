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
    public int OverdueRequests { get; set; }      // P10 M3: Breached SLA target

    public List<AdminRecentTicketItem> RecentTickets { get; set; } = new();

    // P10 M9 Metrics
    public List<TechnicianWorkloadItem> TechnicianWorkloads { get; set; } = new();
    public List<LocationIssueItem> LocationSummaries { get; set; } = new();
    public List<NetworkZoneHealthItem> NetworkZoneSummaries { get; set; } = new();
    public List<AdminServiceCenterSummaryItem> ServiceCenterSummaries { get; set; } = new();
}

public class AdminServiceCenterSummaryItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string SupportedBrands { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public int ActiveTicketCount { get; set; }
}

public class TechnicianWorkloadItem
{
    public string TechnicianName { get; set; } = string.Empty;
    public int ActiveTicketCount { get; set; }
}

public class LocationIssueItem
{
    public string Location { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public int OpenCount { get; set; }
    public int OverdueCount { get; set; }
}

public class NetworkZoneHealthItem
{
    public string Zone { get; set; } = string.Empty;
    public double AvgSpeedMbps { get; set; }
    public double AvgLatencyMs { get; set; }
    public int ReportsCount { get; set; }
    public string HealthStatus { get; set; } = string.Empty;
    public string BadgeClass { get; set; } = string.Empty;
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
    public string? CampusLocation { get; set; }
    public bool IsOverdue { get; set; }
    public string SlaBadgeText { get; set; } = string.Empty;
    public string SlaBadgeCssClass { get; set; } = "badge bg-secondary";
    public DateTime CreatedAtUtc { get; set; }
}
