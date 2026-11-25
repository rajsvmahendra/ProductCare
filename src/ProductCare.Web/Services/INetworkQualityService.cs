using ProductCare.Web.Models.Entities;

namespace ProductCare.Web.Services;

public record CampusZoneNetworkSummaryDto(
    string CampusZone,
    int TotalReports,
    double AverageDownloadSpeedMbps,
    double AverageLatencyMs,
    int ClientMeasuredCount,
    int ManualSampleCount,
    string HealthStatus,
    string HealthBadgeCssClass
);

/// <summary>
/// Service interface for logging and aggregating campus network performance telemetry.
/// EduRev P10 — Campus Network Quality Management.
/// </summary>
public interface INetworkQualityService
{
    Task<NetworkQualityReport> SubmitReportAsync(NetworkQualityReport report, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NetworkQualityReport>> GetRecentReportsAsync(int limit = 20, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CampusZoneNetworkSummaryDto>> GetZoneSummariesAsync(CancellationToken cancellationToken = default);
}
