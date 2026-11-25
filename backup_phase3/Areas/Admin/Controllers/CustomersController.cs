using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCare.Web.Data;
using ProductCare.Web.Models.Enums;
using ProductCare.Web.Services;
using ProductCare.Web.ViewModels.Admin;

namespace ProductCare.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = DbInitializerService.RoleAdmin)]
public class CustomersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWarrantyCalculatorService _warrantyCalculator;

    public CustomersController(ApplicationDbContext context, IWarrantyCalculatorService warrantyCalculator)
    {
        _context = context;
        _warrantyCalculator = warrantyCalculator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search)
    {
        var openStatuses = new[]
        {
            RequestStatus.Submitted,
            RequestStatus.UnderReview,
            RequestStatus.Assigned,
            RequestStatus.InProgress,
            RequestStatus.WaitingForCustomer
        };

        var query = _context.Users
            .AsNoTracking()
            .Include(u => u.Products)
            .Include(u => u.ServiceRequests)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(u =>
                u.FullName.Contains(term) ||
                u.Email!.Contains(term) ||
                (u.City != null && u.City.Contains(term)));
        }

        var customers = await query
            .OrderByDescending(u => u.CreatedAtUtc)
            .Select(u => new AdminCustomerListItem
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? "N/A",
                PhoneNumber = u.PhoneNumber,
                City = u.City,
                RegisteredProductsCount = u.Products.Count,
                TotalServiceRequestsCount = u.ServiceRequests.Count,
                OpenServiceRequestsCount = u.ServiceRequests.Count(sr => openStatuses.Contains(sr.Status)),
                CreatedAtUtc = u.CreatedAtUtc
            })
            .ToListAsync();

        var model = new AdminCustomerListViewModel
        {
            Customers = customers,
            SearchTerm = search
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var customer = await _context.Users
            .AsNoTracking()
            .Include(u => u.Products)
            .Include(u => u.ServiceRequests)
                .ThenInclude(sr => sr.Product)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (customer == null)
        {
            return NotFound();
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var productItems = customer.Products.Select(p =>
        {
            var warranty = _warrantyCalculator.GetWarrantyDetails(p.WarrantyExpiryDate, today);
            return new AdminCustomerProductItem
            {
                Id = p.Id,
                Name = p.Name,
                Brand = p.Brand,
                ModelNumber = p.ModelNumber,
                SerialNumber = p.SerialNumber,
                CategoryName = p.Category.ToString(),
                PurchaseDate = p.PurchaseDate,
                WarrantyExpiryDate = p.WarrantyExpiryDate,
                RemainingDays = warranty.RemainingDays,
                StatusBadgeCssClass = warranty.StatusBadgeCssClass,
                StatusDisplayText = warranty.StatusDisplayText,
                HasInvoice = !string.IsNullOrEmpty(p.InvoiceFileName)
            };
        }).ToList();

        var ticketItems = customer.ServiceRequests
            .OrderByDescending(sr => sr.CreatedAtUtc)
            .Select(sr => new AdminCustomerTicketItem
            {
                Id = sr.Id,
                TicketNumber = sr.TicketNumber,
                ProductName = sr.Product?.Name ?? "N/A",
                Subject = sr.Subject,
                Category = sr.Category,
                Priority = sr.Priority,
                PriorityBadgeCssClass = GetPriorityBadgeClass(sr.Priority),
                Status = sr.Status,
                StatusBadgeCssClass = GetStatusBadgeClass(sr.Status),
                StatusDisplayText = sr.Status.ToString(),
                CreatedAtUtc = sr.CreatedAtUtc
            }).ToList();

        var model = new AdminCustomerDetailsViewModel
        {
            Id = customer.Id,
            FullName = customer.FullName,
            Email = customer.Email ?? "N/A",
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            City = customer.City,
            PostalCode = customer.PostalCode,
            IsActive = customer.IsActive,
            CreatedAtUtc = customer.CreatedAtUtc,
            Products = productItems,
            ServiceRequests = ticketItems
        };

        return View(model);
    }

    private static string GetPriorityBadgeClass(PriorityLevel priority) => priority switch
    {
        PriorityLevel.Low => "badge bg-secondary",
        PriorityLevel.Medium => "badge bg-info text-dark",
        PriorityLevel.High => "badge bg-warning text-dark",
        PriorityLevel.Urgent => "badge bg-danger",
        _ => "badge bg-light text-dark"
    };

    private static string GetStatusBadgeClass(RequestStatus status) => status switch
    {
        RequestStatus.Submitted => "badge bg-primary",
        RequestStatus.UnderReview => "badge bg-info text-dark",
        RequestStatus.Assigned => "badge bg-secondary",
        RequestStatus.InProgress => "badge bg-warning text-dark",
        RequestStatus.WaitingForCustomer => "badge bg-dark",
        RequestStatus.Resolved => "badge bg-success",
        RequestStatus.Closed => "badge bg-secondary",
        RequestStatus.Rejected => "badge bg-danger",
        _ => "badge bg-light text-dark"
    };
}
