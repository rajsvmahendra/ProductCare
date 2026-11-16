using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCare.Web.Data;
using ProductCare.Web.Models.Enums;
using ProductCare.Web.Services;
using ProductCare.Web.ViewModels.Admin;
using ProductCare.Web.ViewModels.Customer;

namespace ProductCare.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = DbInitializerService.RoleAdmin)]
public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWarrantyCalculatorService _warrantyCalculator;

    public ProductsController(ApplicationDbContext context, IWarrantyCalculatorService warrantyCalculator)
    {
        _context = context;
        _warrantyCalculator = warrantyCalculator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(ProductCategory? category, WarrantyStatus? status, string? search)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.ServiceRequests)
            .AsQueryable();

        if (category.HasValue)
        {
            query = query.Where(p => p.Category == category.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p =>
                p.Name.Contains(term) ||
                p.Brand.Contains(term) ||
                p.ModelNumber.Contains(term) ||
                p.SerialNumber.Contains(term) ||
                p.User.FullName.Contains(term) ||
                p.User.Email!.Contains(term));
        }

        var products = await query
            .OrderBy(p => p.WarrantyExpiryDate)
            .ToListAsync();

        var list = new List<AdminProductListItem>();

        foreach (var p in products)
        {
            var warranty = _warrantyCalculator.GetWarrantyDetails(p.WarrantyExpiryDate, today);

            if (status.HasValue && warranty.Status != status.Value)
            {
                continue;
            }

            list.Add(new AdminProductListItem
            {
                Id = p.Id,
                Name = p.Name,
                Brand = p.Brand,
                ModelNumber = p.ModelNumber,
                SerialNumber = p.SerialNumber,
                Category = p.Category,
                CategoryName = p.Category.ToString(),
                CustomerId = p.UserId,
                CustomerName = p.User?.FullName ?? "N/A",
                CustomerEmail = p.User?.Email ?? "N/A",
                PurchaseDate = p.PurchaseDate,
                WarrantyExpiryDate = p.WarrantyExpiryDate,
                RemainingDays = warranty.RemainingDays,
                WarrantyStatus = warranty.Status,
                StatusBadgeCssClass = warranty.StatusBadgeCssClass,
                StatusDisplayText = warranty.StatusDisplayText,
                ServiceRequestsCount = p.ServiceRequests.Count
            });
        }

        var model = new AdminProductListViewModel
        {
            Products = list,
            FilterCategory = category,
            FilterStatus = status,
            SearchTerm = search
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.ServiceRequests)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var warranty = _warrantyCalculator.GetWarrantyDetails(product.WarrantyExpiryDate, today);

        var serviceRequestItems = product.ServiceRequests
            .OrderByDescending(sr => sr.CreatedAtUtc)
            .Select(sr => new ProductServiceRequestItem
            {
                Id = sr.Id,
                TicketNumber = sr.TicketNumber,
                Subject = sr.Subject,
                Category = sr.Category,
                Priority = sr.Priority,
                Status = sr.Status,
                StatusBadgeCssClass = GetStatusBadgeClass(sr.Status),
                PriorityBadgeCssClass = GetPriorityBadgeClass(sr.Priority),
                CreatedAtUtc = sr.CreatedAtUtc
            })
            .ToList();

        var model = new AdminProductDetailsViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Brand = product.Brand,
            ModelNumber = product.ModelNumber,
            SerialNumber = product.SerialNumber,
            Category = product.Category,
            CategoryName = product.Category.ToString(),
            PurchaseDate = product.PurchaseDate,
            WarrantyDurationMonths = product.WarrantyDurationMonths,
            WarrantyExpiryDate = product.WarrantyExpiryDate,
            PurchaseStore = product.PurchaseStore,
            PurchasePrice = product.PurchasePrice,
            Notes = product.Notes,
            CreatedAtUtc = product.CreatedAtUtc,
            CustomerId = product.UserId,
            CustomerName = product.User?.FullName ?? "N/A",
            CustomerEmail = product.User?.Email ?? "N/A",
            CustomerPhone = product.User?.PhoneNumber,
            CustomerCity = product.User?.City,
            Warranty = warranty,
            InvoiceFileName = product.InvoiceFileName,
            InvoiceOriginalFileName = product.InvoiceOriginalFileName,
            InvoiceContentType = product.InvoiceContentType,
            InvoiceFileSize = product.InvoiceFileSize,
            ServiceRequests = serviceRequestItems
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
