using System.ComponentModel.DataAnnotations;
using ProductCare.Web.Services;

namespace ProductCare.Web.ViewModels.Customer;

public class NetworkQualityViewModel
{
    [Required(ErrorMessage = "Please select or specify the campus zone.")]
    [Display(Name = "Campus Zone / Building")]
    public string CampusZone { get; set; } = "Central Library";

    [Required]
    [Display(Name = "Network Connection Type")]
    public string NetworkType { get; set; } = "Campus Wi-Fi (Enterprise)";

    [Range(0.1, 10000.0, ErrorMessage = "Speed must be a positive number.")]
    [Display(Name = "Download Speed (Mbps)")]
    public double DownloadSpeedMbps { get; set; } = 42.5;

    [Range(1, 5000, ErrorMessage = "Latency must be between 1ms and 5000ms.")]
    [Display(Name = "Round-Trip Latency (ms)")]
    public int LatencyMs { get; set; } = 28;

    /// <summary>
    /// Distinguishes live client probe from manual estimate.
    /// </summary>
    [Display(Name = "Measured via In-Browser Network Probe")]
    public bool IsClientMeasured { get; set; } = true;

    [StringLength(500, ErrorMessage = "Observation notes cannot exceed 500 characters.")]
    [Display(Name = "User Observations & Notes")]
    public string? Notes { get; set; }

    // Summary data for display
    public List<CampusZoneNetworkSummaryDto> ZoneSummaries { get; set; } = new();

    public List<NetworkQualityReportItem> RecentReports { get; set; } = new();
}

public class NetworkQualityReportItem
{
    public int Id { get; set; }
    public string ReporterName { get; set; } = string.Empty;
    public string CampusZone { get; set; } = string.Empty;
    public string NetworkType { get; set; } = string.Empty;
    public double DownloadSpeedMbps { get; set; }
    public int LatencyMs { get; set; }
    public bool IsClientMeasured { get; set; }
    public string? Notes { get; set; }
    public DateTime MeasuredAtUtc { get; set; }
}
