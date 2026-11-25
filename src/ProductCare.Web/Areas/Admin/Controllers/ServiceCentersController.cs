using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCare.Web.Data;
using ProductCare.Web.Models.Entities;
using ProductCare.Web.Services;
using ProductCare.Web.ViewModels.Admin;

namespace ProductCare.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = DbInitializerService.RoleAdmin)]
public class ServiceCentersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ServiceCentersController> _logger;

    public ServiceCentersController(ApplicationDbContext context, ILogger<ServiceCentersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.ServiceCenters
            .AsNoTracking()
            .Include(sc => sc.ServiceRequests)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(sc =>
                sc.Name.Contains(term) ||
                sc.City.Contains(term) ||
                sc.SupportedBrands.Contains(term) ||
                sc.SupportedCategories.Contains(term));
        }

        var centers = await query
            .OrderByDescending(sc => sc.IsActive)
            .ThenByDescending(sc => sc.IsVerified)
            .ThenBy(sc => sc.Name)
            .Select(sc => new AdminServiceCenterItemViewModel
            {
                Id = sc.Id,
                Name = sc.Name,
                Address = sc.Address,
                City = sc.City,
                Locality = sc.Locality,
                State = sc.State,
                Country = sc.Country,
                Phone = sc.Phone,
                Email = sc.Email,
                OpeningHours = sc.OpeningHours,
                SupportedBrands = sc.SupportedBrands,
                SupportedCategories = sc.SupportedCategories,
                IsActive = sc.IsActive,
                IsVerified = sc.IsVerified,
                ActiveTicketCount = sc.ServiceRequests.Count(r => r.Status != Models.Enums.RequestStatus.Resolved && r.Status != Models.Enums.RequestStatus.Closed && r.Status != Models.Enums.RequestStatus.Rejected),
                CreatedAtUtc = sc.CreatedAtUtc
            })
            .ToListAsync();

        var model = new AdminServiceCenterListViewModel
        {
            Centers = centers,
            SearchTerm = search
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new AdminServiceCenterCreateEditViewModel
        {
            IsActive = true,
            IsVerified = false,
            OpeningHours = "Mon-Fri: 9:00 AM - 6:00 PM, Sat: 10:00 AM - 2:00 PM"
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminServiceCenterCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var center = new ServiceCenter
        {
            Name = model.Name.Trim(),
            Address = model.Address.Trim(),
            City = model.City.Trim(),
            Locality = model.Locality?.Trim(),
            State = model.State?.Trim() ?? "Punjab",
            Country = model.Country?.Trim() ?? "India",
            SupportedBrands = model.SupportedBrands.Trim(),
            SupportedCategories = model.SupportedCategories.Trim(),
            Phone = model.Phone.Trim(),
            Email = model.Email.Trim(),
            OpeningHours = model.OpeningHours.Trim(),
            Website = model.Website?.Trim(),
            MapUrl = model.MapUrl?.Trim(),
            IsActive = model.IsActive,
            IsVerified = model.IsVerified,
            Notes = model.Notes?.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.ServiceCenters.Add(center);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Service center '{center.Name}' added to directory successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var center = await _context.ServiceCenters.FindAsync(id);
        if (center == null)
        {
            return NotFound();
        }

        var model = new AdminServiceCenterCreateEditViewModel
        {
            Id = center.Id,
            Name = center.Name,
            Address = center.Address,
            City = center.City,
            Locality = center.Locality,
            State = center.State,
            Country = center.Country,
            SupportedBrands = center.SupportedBrands,
            SupportedCategories = center.SupportedCategories,
            Phone = center.Phone,
            Email = center.Email,
            OpeningHours = center.OpeningHours,
            Website = center.Website,
            MapUrl = center.MapUrl,
            IsActive = center.IsActive,
            IsVerified = center.IsVerified,
            Notes = center.Notes
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AdminServiceCenterCreateEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var center = await _context.ServiceCenters.FindAsync(id);
        if (center == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        center.Name = model.Name.Trim();
        center.Address = model.Address.Trim();
        center.City = model.City.Trim();
        center.Locality = model.Locality?.Trim();
        center.State = model.State?.Trim() ?? "Punjab";
        center.Country = model.Country?.Trim() ?? "India";
        center.SupportedBrands = model.SupportedBrands.Trim();
        center.SupportedCategories = model.SupportedCategories.Trim();
        center.Phone = model.Phone.Trim();
        center.Email = model.Email.Trim();
        center.OpeningHours = model.OpeningHours.Trim();
        center.Website = model.Website?.Trim();
        center.MapUrl = model.MapUrl?.Trim();
        center.IsActive = model.IsActive;
        center.IsVerified = model.IsVerified;
        center.Notes = model.Notes?.Trim();

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Service center '{center.Name}' updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var center = await _context.ServiceCenters.FindAsync(id);
        if (center == null)
        {
            return NotFound();
        }

        center.IsActive = !center.IsActive;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Service center '{center.Name}' is now {(center.IsActive ? "Active" : "Inactive")}.";
        return RedirectToAction(nameof(Index));
    }
}
