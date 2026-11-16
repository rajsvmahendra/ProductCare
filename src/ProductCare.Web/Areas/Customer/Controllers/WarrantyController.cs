using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCare.Web.Data;
using ProductCare.Web.Models.Enums;
using ProductCare.Web.Services;
using ProductCare.Web.ViewModels.Customer;

namespace ProductCare.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize(Roles = DbInitializerService.RoleCustomer)]
public class WarrantyController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWarrantyCalculatorService _warrantyCalculator;

    public WarrantyController(ApplicationDbContext context, IWarrantyCalculatorService warrantyCalculator)
    {
        _context = context;
        _warrantyCalculator = warrantyCalculator;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task<IActionResult> Index(WarrantyStatus? filter)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var products = await _context.Products
            .AsNoTracking()
            .Where(p => p.UserId == CurrentUserId)
            .OrderBy(p => p.WarrantyExpiryDate)
            .ToListAsync();

        var activeCount = 0;
        var expiringSoonCount = 0;
        var expiredCount = 0;

        var timelineItems = new List<WarrantyTimelineItem>();

        foreach (var p in products)
        {
            var warranty = _warrantyCalculator.GetWarrantyDetails(p.WarrantyExpiryDate, today);

            switch (warranty.Status)
            {
                case WarrantyStatus.Active:
                    activeCount++;
                    break;
                case WarrantyStatus.ExpiringSoon:
                    expiringSoonCount++;
                    break;
                case WarrantyStatus.Expired:
                    expiredCount++;
                    break;
            }

            if (filter.HasValue && warranty.Status != filter.Value)
            {
                continue;
            }

            timelineItems.Add(new WarrantyTimelineItem
            {
                ProductId = p.Id,
                ProductName = p.Name,
                Brand = p.Brand,
                ModelNumber = p.ModelNumber,
                SerialNumber = p.SerialNumber,
                CategoryName = p.Category.ToString(),
                PurchaseDate = p.PurchaseDate,
                WarrantyDurationMonths = p.WarrantyDurationMonths,
                WarrantyExpiryDate = p.WarrantyExpiryDate,
                RemainingDays = warranty.RemainingDays,
                Status = warranty.Status,
                StatusBadgeCssClass = warranty.StatusBadgeCssClass,
                StatusDisplayText = warranty.StatusDisplayText
            });
        }

        var model = new WarrantyOverviewViewModel
        {
            ActiveCount = activeCount,
            ExpiringSoonCount = expiringSoonCount,
            ExpiredCount = expiredCount,
            TotalProductsCount = products.Count,
            FilterStatus = filter,
            WarrantyItems = timelineItems
        };

        return View(model);
    }
}
