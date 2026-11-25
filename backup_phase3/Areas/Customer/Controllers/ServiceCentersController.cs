using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCare.Web.Data;
using ProductCare.Web.ViewModels.Customer;

namespace ProductCare.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
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
    public async Task<IActionResult> Index(string? search, string? city, string? locality, string? brand, string? category)
    {
        var query = _context.ServiceCenters
            .AsNoTracking()
            .Include(sc => sc.ServiceRequests)
            .Where(sc => sc.IsActive);

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(sc => sc.City == city);
        }

        if (!string.IsNullOrWhiteSpace(locality))
        {
            query = query.Where(sc => sc.Locality == locality);
        }

        if (!string.IsNullOrWhiteSpace(brand))
        {
            query = query.Where(sc => sc.SupportedBrands.Contains(brand));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(sc => sc.SupportedCategories.Contains(category));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(sc =>
                sc.Name.Contains(term) ||
                sc.Locality!.Contains(term) ||
                sc.Address.Contains(term) ||
                sc.City.Contains(term) ||
                sc.SupportedBrands.Contains(term) ||
                sc.SupportedCategories.Contains(term));
        }

        var centers = await query
            .OrderByDescending(sc => sc.IsVerified)
            .ThenBy(sc => sc.Name)
            .ToListAsync();

        var cities = await _context.ServiceCenters
            .Where(sc => sc.IsActive)
            .Select(sc => sc.City)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        var localities = await _context.ServiceCenters
            .Where(sc => sc.IsActive && sc.Locality != null)
            .Select(sc => sc.Locality!)
            .Distinct()
            .OrderBy(l => l)
            .ToListAsync();

        var cardModels = centers.Select(c => new ServiceCenterCardViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Locality = c.Locality,
            Address = c.Address,
            City = c.City,
            State = c.State,
            Country = c.Country,
            SupportedBrands = c.SupportedBrands,
            SupportedCategories = c.SupportedCategories,
            Phone = c.Phone,
            Email = c.Email,
            OpeningHours = c.OpeningHours,
            Website = c.Website,
            MapUrl = c.MapUrl,
            IsActive = c.IsActive,
            IsVerified = c.IsVerified,
            Notes = c.Notes,
            OpenTicketCount = c.ServiceRequests.Count(sr => sr.Status != Models.Enums.RequestStatus.Resolved && sr.Status != Models.Enums.RequestStatus.Closed && sr.Status != Models.Enums.RequestStatus.Rejected)
        }).ToList();

        var model = new ServiceCenterListViewModel
        {
            Centers = cardModels,
            SearchTerm = search,
            FilterCity = city,
            FilterLocality = locality,
            FilterBrand = brand,
            FilterCategory = category,
            AvailableCities = cities,
            AvailableLocalities = localities
        };

        return View(model);
    }
}
