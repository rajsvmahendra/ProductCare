using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCare.Web.Services;

namespace ProductCare.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = DbInitializerService.RoleAdmin)]
public class DashboardController : Controller
{
    private readonly IDashboardReportingService _reportingService;

    public DashboardController(IDashboardReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    public async Task<IActionResult> Index()
    {
        var dashboardData = await _reportingService.GetAdminDashboardAsync();
        return View(dashboardData);
    }
}
