using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCare.Web.Services;

namespace ProductCare.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize(Roles = DbInitializerService.RoleCustomer)]
public class DashboardController : Controller
{
    private readonly IDashboardReportingService _reportingService;

    public DashboardController(IDashboardReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var dashboardData = await _reportingService.GetCustomerDashboardAsync(userId);
        return View(dashboardData);
    }
}
