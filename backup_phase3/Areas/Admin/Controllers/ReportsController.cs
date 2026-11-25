using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCare.Web.Services;

namespace ProductCare.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = DbInitializerService.RoleAdmin)]
public class ReportsController : Controller
{
    private readonly IDashboardReportingService _reportingService;

    public ReportsController(IDashboardReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
    {
        var model = await _reportingService.GetFullReportsAsync(startDate, endDate);
        return View(model);
    }
}
