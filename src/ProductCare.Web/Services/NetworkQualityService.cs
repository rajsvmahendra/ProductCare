using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductCare.Web.Data;
using ProductCare.Web.Models.Entities;

namespace ProductCare.Web.Services;

/// <summary>
/// Aggregates and records campus network performance metrics.
/// Honest minimum demonstration: clearly separates live browser-probed data from manual sample data.
/// EduRev P10 — Campus Network Quality Management.
/// </summary>
public class NetworkQualityService : INetworkQualityService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NetworkQualityService> _logger;

    public NetworkQualityService(
        ApplicationDbContext context,
        ILogger<NetworkQualityService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<NetworkQualityReport> SubmitReportAsync(NetworkQualityReport report, CancellationToken cancellationToken = default)
    {
        if (report.MeasuredAtUtc == default)
        {
            report.MeasuredAtUtc = DateTime.UtcNow;
        }

        _context.NetworkQualityReports.Add(report);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "New network report logged: Zone={Zone}, Type={Type}, Speed={Speed}Mbps, Latency={Latency}ms, ClientMeasured={ClientMeasured}",
            report.CampusZone, report.NetworkType, report.DownloadSpeedMbps, report.LatencyMs, report.IsClientMeasured);

        return report;
    }

    public async Task<IReadOnlyList<NetworkQualityReport>> GetRecentReportsAsync(int limit = 20, CancellationToken cancellationToken = default)
    {
        return await _context.NetworkQualityReports
            .AsNoTracking()
            .Include(r => r.User)
            .OrderByDescending(r => r.MeasuredAtUtc)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CampusZoneNetworkSummaryDto>> GetZoneSummariesAsync(CancellationToken cancellationToken = default)
    {
        var reports = await _context.NetworkQualityReports
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (!reports.Any())
        {
            return Array.Empty<CampusZoneNetworkSummaryDto>();
        }

        return reports
            .GroupBy(r => r.CampusZone)
            .Select(g =>
            {
                var total = g.Count();
                var avgSpeed = Math.Round(g.Average(r => r.DownloadSpeedMbps), 1);
                var avgLatency = Math.Round(g.Average(r => r.LatencyMs), 1);
                var clientCount = g.Count(r => r.IsClientMeasured);
                var manualCount = total - clientCount;

                var (status, badge) = (avgSpeed, avgLatency) switch
                {
                    ( >= 50.0, <= 30.0) => ("Optimal", "badge bg-success"),
                    ( >= 20.0, <= 70.0) => ("Satisfactory", "badge bg-info text-dark"),
                    ( >= 10.0, <= 120.0) => ("Degraded", "badge bg-warning text-dark"),
                    _ => ("Poor / High Latency", "badge bg-danger")
                };

                return new CampusZoneNetworkSummaryDto(
                    CampusZone: g.Key,
                    TotalReports: total,
                    AverageDownloadSpeedMbps: avgSpeed,
                    AverageLatencyMs: avgLatency,
                    ClientMeasuredCount: clientCount,
                    ManualSampleCount: manualCount,
                    HealthStatus: status,
                    HealthBadgeCssClass: badge
                );
            })
            .OrderByDescending(s => s.TotalReports)
            .ToList();
    }
}
