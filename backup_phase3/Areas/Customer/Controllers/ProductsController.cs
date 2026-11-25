using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCare.Web.Data;
using ProductCare.Web.Models.Entities;
using ProductCare.Web.Models.Enums;
using ProductCare.Web.Services;
using ProductCare.Web.ViewModels.Customer;

namespace ProductCare.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize(Roles = DbInitializerService.RoleCustomer)]
public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWarrantyCalculatorService _warrantyCalculator;
    private readonly IInvoiceStorageService _invoiceStorage;
    private readonly IProductCatalogService _productCatalog;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        ApplicationDbContext context,
        IWarrantyCalculatorService warrantyCalculator,
        IInvoiceStorageService invoiceStorage,
        IProductCatalogService productCatalog,
        ILogger<ProductsController> logger)
    {
        _context = context;
        _warrantyCalculator = warrantyCalculator;
        _invoiceStorage = invoiceStorage;
        _productCatalog = productCatalog;
        _logger = logger;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public IActionResult Suggest(string? query)
    {
        var suggestions = _productCatalog.SearchSuggestions(query, maxResults: 8);
        return Json(suggestions);
    }

    [HttpGet]
    public async Task<IActionResult> Index(ProductCategory? category, WarrantyStatus? status, string? search, string? sort)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var query = _context.Products
            .AsNoTracking()
            .Where(p => p.UserId == CurrentUserId && !p.IsArchived);

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
                p.SerialNumber.Contains(term));
        }

        var products = await query
            .OrderBy(p => p.WarrantyExpiryDate)
            .Include(p => p.ServiceRequests)
            .ToListAsync();

        var list = new List<ProductListItemViewModel>();

        foreach (var p in products)
        {
            var warranty = _warrantyCalculator.GetWarrantyDetails(p.WarrantyExpiryDate, today);

            if (status.HasValue && warranty.Status != status.Value)
            {
                continue;
            }

            var activeRequestsCount = p.ServiceRequests.Count(sr =>
                sr.Status != RequestStatus.Resolved &&
                sr.Status != RequestStatus.Closed &&
                sr.Status != RequestStatus.Rejected);

            list.Add(new ProductListItemViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Brand = p.Brand,
                ModelNumber = p.ModelNumber,
                SerialNumber = p.SerialNumber,
                Category = p.Category,
                CategoryName = p.Category.ToString(),
                SubCategory = p.SubCategory,
                PurchaseDate = p.PurchaseDate,
                WarrantyDurationMonths = p.WarrantyDurationMonths,
                WarrantyExpiryDate = p.WarrantyExpiryDate,
                RemainingDays = warranty.RemainingDays,
                WarrantyStatus = warranty.Status,
                StatusBadgeCssClass = warranty.StatusBadgeCssClass,
                StatusDisplayText = warranty.StatusDisplayText,
                HasInvoice = !string.IsNullOrEmpty(p.InvoiceFileName),
                ActiveServiceRequestsCount = activeRequestsCount
            });
        }

        list = sort switch
        {
            "expiry_desc" => list.OrderByDescending(p => p.WarrantyExpiryDate).ToList(),
            "name_asc" => list.OrderBy(p => p.Name).ToList(),
            "purchase_desc" => list.OrderByDescending(p => p.PurchaseDate).ToList(),
            _ => list.OrderBy(p => p.WarrantyExpiryDate).ToList()
        };

        var model = new ProductListViewModel
        {
            Products = list,
            FilterCategory = category,
            FilterStatus = status,
            SearchTerm = search,
            Sort = sort
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.ServiceRequests)
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == CurrentUserId && !p.IsArchived);

        if (product == null)
        {
            _logger.LogWarning("Unauthorized or non-existent product details request: ProductId {Id}, User {UserId}", id, CurrentUserId);
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

        var model = new ProductDetailsViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Brand = product.Brand,
            ModelNumber = product.ModelNumber,
            SerialNumber = product.SerialNumber,
            Category = product.Category,
            CategoryName = product.Category.ToString(),
            SubCategory = product.SubCategory,
            PurchaseDate = product.PurchaseDate,
            WarrantyDurationMonths = product.WarrantyDurationMonths,
            WarrantyExpiryDate = product.WarrantyExpiryDate,
            PurchaseStore = product.PurchaseStore,
            PurchasePrice = product.PurchasePrice,
            Notes = product.Notes,
            CreatedAtUtc = product.CreatedAtUtc,
            Warranty = warranty,
            InvoiceFileName = product.InvoiceFileName,
            InvoiceOriginalFileName = product.InvoiceOriginalFileName,
            InvoiceContentType = product.InvoiceContentType,
            InvoiceFileSize = product.InvoiceFileSize,
            ServiceRequests = serviceRequestItems
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ProductCreateEditViewModel
        {
            PurchaseDate = DateOnly.FromDateTime(DateTime.UtcNow),
            WarrantyDurationMonths = 12
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateEditViewModel model)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (model.PurchaseDate > today)
        {
            ModelState.AddModelError(nameof(model.PurchaseDate), "Purchase date cannot be in the future.");
        }

        // Check for duplicate serial number for this customer
        var serialExists = await _context.Products.AnyAsync(p =>
            p.UserId == CurrentUserId &&
            p.SerialNumber == model.SerialNumber.Trim());

        if (serialExists)
        {
            ModelState.AddModelError(nameof(model.SerialNumber), "You have already registered a product with this serial number.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var expiryDate = _warrantyCalculator.CalculateExpiryDate(model.PurchaseDate, model.WarrantyDurationMonths);

        string? storedInvoiceName = null;
        string? originalInvoiceName = null;
        string? invoiceContentType = null;
        long? invoiceFileSize = null;

        if (model.InvoiceFile != null && model.InvoiceFile.Length > 0)
        {
            var uploadResult = await _invoiceStorage.SaveInvoiceAsync(model.InvoiceFile);
            if (!uploadResult.Success)
            {
                ModelState.AddModelError(nameof(model.InvoiceFile), uploadResult.ErrorMessage ?? "Invoice upload failed.");
                return View(model);
            }

            storedInvoiceName = uploadResult.StoredFileName;
            originalInvoiceName = uploadResult.OriginalFileName;
            invoiceContentType = uploadResult.ContentType;
            invoiceFileSize = uploadResult.FileSize;
        }

        var product = new Product
        {
            UserId = CurrentUserId,
            Name = model.Name.Trim(),
            Brand = model.Brand.Trim(),
            ModelNumber = model.ModelNumber.Trim(),
            SerialNumber = model.SerialNumber.Trim(),
            Category = model.Category,
            SubCategory = model.SubCategory?.Trim(),
            PurchaseDate = model.PurchaseDate,
            WarrantyDurationMonths = model.WarrantyDurationMonths,
            WarrantyExpiryDate = expiryDate,
            PurchaseStore = model.PurchaseStore?.Trim(),
            PurchasePrice = model.PurchasePrice,
            InvoiceFileName = storedInvoiceName,
            InvoiceOriginalFileName = originalInvoiceName,
            InvoiceContentType = invoiceContentType,
            InvoiceFileSize = invoiceFileSize,
            Notes = model.Notes?.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Product '{product.Name}' registered successfully!";
        return RedirectToAction(nameof(Details), new { id = product.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == CurrentUserId && !p.IsArchived);

        if (product == null)
        {
            return NotFound();
        }

        var model = new ProductCreateEditViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Brand = product.Brand,
            ModelNumber = product.ModelNumber,
            SerialNumber = product.SerialNumber,
            Category = product.Category,
            SubCategory = product.SubCategory,
            PurchaseDate = product.PurchaseDate,
            WarrantyDurationMonths = product.WarrantyDurationMonths,
            PurchaseStore = product.PurchaseStore,
            PurchasePrice = product.PurchasePrice,
            ExistingInvoiceOriginalFileName = product.InvoiceOriginalFileName,
            Notes = product.Notes
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductCreateEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == CurrentUserId && !p.IsArchived);

        if (product == null)
        {
            return NotFound();
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (model.PurchaseDate > today)
        {
            ModelState.AddModelError(nameof(model.PurchaseDate), "Purchase date cannot be in the future.");
        }

        var serialExists = await _context.Products.AnyAsync(p =>
            p.UserId == CurrentUserId &&
            p.SerialNumber == model.SerialNumber.Trim() &&
            p.Id != id);

        if (serialExists)
        {
            ModelState.AddModelError(nameof(model.SerialNumber), "Another registered product already has this serial number.");
        }

        if (!ModelState.IsValid)
        {
            model.ExistingInvoiceOriginalFileName = product.InvoiceOriginalFileName;
            return View(model);
        }

        // Handle replacement invoice file if uploaded
        if (model.InvoiceFile != null && model.InvoiceFile.Length > 0)
        {
            var uploadResult = await _invoiceStorage.SaveInvoiceAsync(model.InvoiceFile);
            if (!uploadResult.Success)
            {
                ModelState.AddModelError(nameof(model.InvoiceFile), uploadResult.ErrorMessage ?? "Invoice upload failed.");
                model.ExistingInvoiceOriginalFileName = product.InvoiceOriginalFileName;
                return View(model);
            }

            // Remove old invoice from disk
            if (!string.IsNullOrEmpty(product.InvoiceFileName))
            {
                await _invoiceStorage.DeleteInvoiceAsync(product.InvoiceFileName);
            }

            product.InvoiceFileName = uploadResult.StoredFileName;
            product.InvoiceOriginalFileName = uploadResult.OriginalFileName;
            product.InvoiceContentType = uploadResult.ContentType;
            product.InvoiceFileSize = uploadResult.FileSize;
        }

        product.Name = model.Name.Trim();
        product.Brand = model.Brand.Trim();
        product.ModelNumber = model.ModelNumber.Trim();
        product.SerialNumber = model.SerialNumber.Trim();
        product.Category = model.Category;
        product.SubCategory = model.SubCategory?.Trim();
        product.PurchaseDate = model.PurchaseDate;
        product.WarrantyDurationMonths = model.WarrantyDurationMonths;
        product.WarrantyExpiryDate = _warrantyCalculator.CalculateExpiryDate(model.PurchaseDate, model.WarrantyDurationMonths);
        product.PurchaseStore = model.PurchaseStore?.Trim();
        product.PurchasePrice = model.PurchasePrice;
        product.Notes = model.Notes?.Trim();
        product.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Product '{product.Name}' updated successfully!";
        return RedirectToAction(nameof(Details), new { id = product.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products
            .Include(p => p.ServiceRequests)
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == CurrentUserId && !p.IsArchived);

        if (product == null)
        {
            return NotFound();
        }

        // If there are service requests, soft-delete / archive to preserve ticket audit history
        if (product.ServiceRequests.Any())
        {
            product.IsArchived = true;
            product.ArchivedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Product '{product.Name}' has been archived from your active inventory. Service history remains preserved for auditing.";
            return RedirectToAction(nameof(Index));
        }

        // Hard delete when no service requests reference this product
        if (!string.IsNullOrEmpty(product.InvoiceFileName))
        {
            await _invoiceStorage.DeleteInvoiceAsync(product.InvoiceFileName);
        }

        var productName = product.Name;
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Product '{productName}' has been removed from your inventory.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> DownloadInvoice(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == CurrentUserId);

        if (product == null || string.IsNullOrEmpty(product.InvoiceFileName))
        {
            return NotFound();
        }

        var stream = await _invoiceStorage.GetInvoiceStreamAsync(product.InvoiceFileName);
        if (stream == null)
        {
            _logger.LogError("Physical invoice file missing on disk for Product {Id}: {FileName}", id, product.InvoiceFileName);
            return NotFound("Invoice file could not be located on server storage.");
        }

        var contentType = product.InvoiceContentType ?? "application/octet-stream";
        var downloadName = product.InvoiceOriginalFileName ?? $"Invoice-{product.Id}.pdf";

        return File(stream, contentType, downloadName);
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
