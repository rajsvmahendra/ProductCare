using ProductCare.Web.Models.Entities;

namespace ProductCare.Web.ViewModels.Admin;

public class AdminReportsViewModel
{
    public DateTime? FilterStartDate { get; set; }
    public DateTime? FilterEndDate { get; set; }

    // Stored procedure output or equivalent aggregation
    public List<ServiceTicketStatisticsDto> TicketStatistics { get; set; } = new();

    // SQL View output or equivalent projection
    public List<ProductWarrantySummaryView> WarrantySummaries { get; set; } = new();

    // Summary totals
    public int TotalTicketsCount { get; set; }
    public int ActiveWarrantiesCount { get; set; }
    public int ExpiringSoonWarrantiesCount { get; set; }
    public int ExpiredWarrantiesCount { get; set; }
}
