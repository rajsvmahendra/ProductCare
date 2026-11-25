using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCare.Web.Models.Entities;
using ProductCare.Web.Services;
using ProductCare.Web.ViewModels.Customer;

namespace ProductCare.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize(Roles = DbInitializerService.RoleCustomer)]
public class NetworkQualityController : Controller
{
    private readonly INetworkQualityService _networkService;
    private readonly ILogger<NetworkQualityController> _logger;

    public NetworkQualityController(
        INetworkQualityService networkService,
        ILogger<NetworkQualityController> logger)
    {
        _networkService = networkService;
        _logger = logger;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var summaries = await _networkService.GetZoneSummariesAsync();
        var recent = await _networkService.GetRecentReportsAsync(15);

        var model = new NetworkQualityViewModel
        {
            ZoneSummaries = summaries.ToList(),
            RecentReports = recent.Select(r => new NetworkQualityReportItem
            {
                Id = r.Id,
                ReporterName = r.User?.FullName ?? "Campus Reporter",
                CampusZone = r.CampusZone,
                NetworkType = r.NetworkType,
                DownloadSpeedMbps = r.DownloadSpeedMbps,
                LatencyMs = r.LatencyMs,
                IsClientMeasured = r.IsClientMeasured,
                Notes = r.Notes,
                MeasuredAtUtc = r.MeasuredAtUtc
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(NetworkQualityViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var summaries = await _networkService.GetZoneSummariesAsync();
            var recent = await _networkService.GetRecentReportsAsync(15);
            model.ZoneSummaries = summaries.ToList();
            model.RecentReports = recent.Select(r => new NetworkQualityReportItem
            {
                Id = r.Id,
                ReporterName = r.User?.FullName ?? "Campus Reporter",
                CampusZone = r.CampusZone,
                NetworkType = r.NetworkType,
                DownloadSpeedMbps = r.DownloadSpeedMbps,
                LatencyMs = r.LatencyMs,
                IsClientMeasured = r.IsClientMeasured,
                Notes = r.Notes,
                MeasuredAtUtc = r.MeasuredAtUtc
            }).ToList();

            return View("Index", model);
        }

        var report = new NetworkQualityReport
        {
            UserId = CurrentUserId,
            CampusZone = model.CampusZone.Trim(),
            NetworkType = model.NetworkType.Trim(),
            DownloadSpeedMbps = Math.Round(model.DownloadSpeedMbps, 1),
            LatencyMs = model.LatencyMs,
            IsClientMeasured = model.IsClientMeasured,
            Notes = model.Notes?.Trim(),
            MeasuredAtUtc = DateTime.UtcNow
        };

        await _networkService.SubmitReportAsync(report);

        TempData["SuccessMessage"] = $"Network quality report for '{report.CampusZone}' recorded successfully ({report.DownloadSpeedMbps} Mbps, {report.LatencyMs}ms latency).";
        return RedirectToAction(nameof(Index));
    }
}
