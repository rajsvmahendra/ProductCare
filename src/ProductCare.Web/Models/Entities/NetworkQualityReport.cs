namespace ProductCare.Web.Models.Entities;

/// <summary>
/// Represents a campus Wi-Fi / mobile network quality measurement or complaint.
/// EduRev P10 — Campus Issue, Maintenance and Network Quality Management Platform.
/// </summary>
public class NetworkQualityReport
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Campus zone or facility (e.g. "Hostel Block A", "Central Library", "Academic Block 1", "Auditorium & Student Center", "Sports Complex").
    /// </summary>
    public string CampusZone { get; set; } = string.Empty;

    /// <summary>
    /// Network connection type (e.g. "Campus Wi-Fi (Enterprise)", "Campus Wi-Fi (Guest)", "Mobile 5G/4G", "Lab Ethernet").
    /// </summary>
    public string NetworkType { get; set; } = "Campus Wi-Fi (Enterprise)";

    /// <summary>
    /// Download speed in Mbps.
    /// </summary>
    public double DownloadSpeedMbps { get; set; }

    /// <summary>
    /// Network round-trip latency in milliseconds.
    /// </summary>
    public int LatencyMs { get; set; }

    /// <summary>
    /// Honest distinction: True if measured live via browser timing/probe;
    /// False if manually entered sample by reporter.
    /// </summary>
    public bool IsClientMeasured { get; set; }

    /// <summary>
    /// Optional reporter observation notes (e.g., "Frequent drops during lectures", "High packet loss").
    /// </summary>
    public string? Notes { get; set; }

    public DateTime MeasuredAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual ApplicationUser User { get; set; } = null!;
}
