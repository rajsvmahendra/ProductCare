using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductCare.Web.Data;
using ProductCare.Web.Models.Entities;
using ProductCare.Web.Models.Enums;
using ProductCare.Web.Services;
using ProductCare.Web.ViewModels.Customer;

namespace ProductCare.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize(Roles = DbInitializerService.RoleCustomer)]
public class ServiceRequestsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IServiceRequestWorkflowService _workflowService;
    private readonly IWarrantyCalculatorService _warrantyCalculator;
    private readonly ISlaCalculationService _slaService;
    private readonly IInvoiceStorageService _storageService;
    private readonly ILogger<ServiceRequestsController> _logger;

    public ServiceRequestsController(
        ApplicationDbContext context,
        IServiceRequestWorkflowService workflowService,
        IWarrantyCalculatorService warrantyCalculator,
        ISlaCalculationService slaService,
        IInvoiceStorageService storageService,
        ILogger<ServiceRequestsController> logger)
    {
        _context = context;
        _workflowService = workflowService;
        _warrantyCalculator = warrantyCalculator;
        _slaService = slaService;
        _storageService = storageService;
        _logger = logger;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private string CurrentUserName => User.Identity?.Name ?? "Customer";

    [HttpGet]
    public async Task<IActionResult> Index(
        RequestStatus? status,
        PriorityLevel? priority,
        RequestCategory? category,
        string? search)
    {
        var query = _context.ServiceRequests
            .AsNoTracking()
            .Include(sr => sr.Product)
            .Include(sr => sr.ServiceCenter)
            .Where(sr => sr.UserId == CurrentUserId);

        if (status.HasValue)
        {
            query = query.Where(sr => sr.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(sr => sr.Priority == priority.Value);
        }

        if (category.HasValue)
        {
            query = query.Where(sr => sr.Category == category.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(sr =>
                sr.TicketNumber.Contains(term) ||
                sr.Subject.Contains(term) ||
                sr.Product.Name.Contains(term) ||
                sr.Product.Brand.Contains(term));
        }

        var rawTickets = await query
            .OrderByDescending(sr => sr.CreatedAtUtc)
            .ToListAsync();

        var tickets = rawTickets.Select(sr =>
        {
            var isOverdue = _slaService.IsOverdue(sr.SlaDeadlineUtc, sr.ResolvedAtUtc);
            var (slaText, slaBadge) = _slaService.GetSlaStatusBadge(sr.SlaDeadlineUtc, sr.ResolvedAtUtc);
            return new CustomerServiceRequestListItem
            {
                Id = sr.Id,
                TicketNumber = sr.TicketNumber,
                ProductName = sr.Product.Name,
                ProductBrand = sr.Product.Brand,
                Subject = sr.Subject,
                Category = sr.Category,
                CategoryName = sr.Category.ToString(),
                Priority = sr.Priority,
                PriorityBadgeCssClass = GetPriorityBadgeClass(sr.Priority),
                Status = sr.Status,
                StatusBadgeCssClass = GetStatusBadgeClass(sr.Status),
                StatusDisplayText = FormatStatusText(sr.Status),
                CampusLocation = sr.CampusLocation,
                ServiceCenterName = sr.ServiceCenter?.Name,
                SlaDeadlineUtc = sr.SlaDeadlineUtc,
                IsOverdue = isOverdue,
                IsEscalated = sr.IsEscalated,
                SlaBadgeText = slaText,
                SlaBadgeCssClass = slaBadge,
                CreatedAtUtc = sr.CreatedAtUtc,
                UpdatedAtUtc = sr.UpdatedAtUtc
            };
        }).ToList();

        var model = new ServiceRequestListViewModel
        {
            Requests = tickets,
            FilterStatus = status,
            FilterPriority = priority,
            FilterCategory = category,
            SearchTerm = search
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var request = await _context.ServiceRequests
            .AsNoTracking()
            .Include(sr => sr.Product)
            .Include(sr => sr.ServiceCenter)
            .Include(sr => sr.StatusHistories.OrderBy(h => h.CreatedAtUtc))
            .FirstOrDefaultAsync(sr => sr.Id == id && sr.UserId == CurrentUserId);

        if (request == null)
        {
            _logger.LogWarning("Unauthorized or non-existent ticket request: TicketId {Id}, User {UserId}", id, CurrentUserId);
            return NotFound();
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var productWarranty = _warrantyCalculator.GetWarrantyDetails(request.Product.WarrantyExpiryDate, today);

        var historyItems = request.StatusHistories.Select(h => new ServiceRequestHistoryItem
        {
            Id = h.Id,
            FromStatus = h.FromStatus,
            FromStatusText = FormatStatusText(h.FromStatus),
            ToStatus = h.ToStatus,
            ToStatusText = FormatStatusText(h.ToStatus),
            ToStatusBadgeCssClass = GetStatusBadgeClass(h.ToStatus),
            ChangedByUserName = h.ChangedByUserName,
            Remarks = h.Remarks,
            CreatedAtUtc = h.CreatedAtUtc
        }).ToList();

        var isOverdue = _slaService.IsOverdue(request.SlaDeadlineUtc, request.ResolvedAtUtc);
        var isApproaching = _slaService.IsApproachingBreach(request.SlaDeadlineUtc, request.ResolvedAtUtc);
        var (slaText, slaBadge) = _slaService.GetSlaStatusBadge(request.SlaDeadlineUtc, request.ResolvedAtUtc);

        var model = new ServiceRequestDetailsViewModel
        {
            Id = request.Id,
            TicketNumber = request.TicketNumber,
            Category = request.Category,
            CategoryName = request.Category.ToString(),
            Priority = request.Priority,
            PriorityBadgeCssClass = GetPriorityBadgeClass(request.Priority),
            Status = request.Status,
            StatusBadgeCssClass = GetStatusBadgeClass(request.Status),
            StatusDisplayText = FormatStatusText(request.Status),
            Subject = request.Subject,
            Description = request.Description,
            CampusLocation = request.CampusLocation,
            SlaDeadlineUtc = request.SlaDeadlineUtc,
            IsOverdue = isOverdue,
            IsApproachingBreach = isApproaching,
            IsEscalated = request.IsEscalated,
            SlaBadgeText = slaText,
            SlaBadgeCssClass = slaBadge,
            AttachmentFileName = request.AttachmentFileName,
            ResolutionProofFileName = request.ResolutionProofFileName,
            AssignedTechnician = request.AssignedTechnician,
            AdminRemarks = request.AdminRemarks,
            CreatedAtUtc = request.CreatedAtUtc,
            UpdatedAtUtc = request.UpdatedAtUtc,
            ResolvedAtUtc = request.ResolvedAtUtc,
            ProductId = request.ProductId,
            ProductName = request.Product.Name,
            ProductBrand = request.Product.Brand,
            ProductModelNumber = request.Product.ModelNumber,
            ProductSerialNumber = request.Product.SerialNumber,
            ProductWarrantyExpiryDate = request.Product.WarrantyExpiryDate,
            ProductWarrantyStatusBadge = productWarranty.StatusBadgeCssClass,
            ProductWarrantyStatusText = productWarranty.StatusDisplayText,
            ServiceCenterId = request.ServiceCenterId,
            ServiceCenterName = request.ServiceCenter?.Name,
            ServiceCenterAddress = request.ServiceCenter?.Address,
            ServiceCenterCity = request.ServiceCenter?.City,
            ServiceCenterPhone = request.ServiceCenter?.Phone,
            ServiceCenterIsVerified = request.ServiceCenter?.IsVerified,
            StatusHistory = historyItems
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? productId, int? centerId)
    {
        var products = await _context.Products
            .AsNoTracking()
            .Where(p => p.UserId == CurrentUserId)
            .OrderBy(p => p.Name)
            .ToListAsync();

        if (!products.Any())
        {
            TempData["ErrorMessage"] = "You must register at least one product before raising a service request.";
            return RedirectToAction("Create", "Products");
        }

        var centers = await _context.ServiceCenters
            .AsNoTracking()
            .Where(sc => sc.IsActive)
            .OrderBy(sc => sc.Name)
            .ToListAsync();

        var model = new ServiceRequestCreateViewModel
        {
            ProductId = productId ?? products.First().Id,
            ServiceCenterId = centerId,
            AvailableProducts = products.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Name} ({p.Brand} - S/N: {p.SerialNumber})",
                Selected = productId.HasValue && p.Id == productId.Value
            }).ToList(),
            AvailableServiceCenters = centers.Select(sc => new SelectListItem
            {
                Value = sc.Id.ToString(),
                Text = $"{sc.Name} — {sc.City} ({(sc.IsVerified ? "Verified Partner" : "Sample Hub")})",
                Selected = centerId.HasValue && sc.Id == centerId.Value
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceRequestCreateViewModel model)
    {
        // Enforce product ownership
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == model.ProductId && p.UserId == CurrentUserId);

        if (product == null)
        {
            ModelState.AddModelError(nameof(model.ProductId), "Selected product was not found in your registered inventory.");
        }

        if (!ModelState.IsValid)
        {
            var userProducts = await _context.Products
                .AsNoTracking()
                .Where(p => p.UserId == CurrentUserId)
                .OrderBy(p => p.Name)
                .ToListAsync();

            var centers = await _context.ServiceCenters
                .AsNoTracking()
                .Where(sc => sc.IsActive)
                .OrderBy(sc => sc.Name)
                .ToListAsync();

            model.AvailableProducts = userProducts.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Name} ({p.Brand} - S/N: {p.SerialNumber})"
            }).ToList();

            model.AvailableServiceCenters = centers.Select(sc => new SelectListItem
            {
                Value = sc.Id.ToString(),
                Text = $"{sc.Name} — {sc.City} ({(sc.IsVerified ? "Verified Partner" : "Sample Hub")})",
                Selected = model.ServiceCenterId.HasValue && sc.Id == model.ServiceCenterId.Value
            }).ToList();

            return View(model);
        }

        var ticketNumber = await _workflowService.GenerateTicketNumberAsync();
        var utcNow = DateTime.UtcNow;
        var slaDeadline = _slaService.CalculateSlaDeadline(utcNow, model.Priority);
        var suggestedTech = _slaService.GetRecommendedTechnician(model.Category, model.CampusLocation);

        string? attachmentFileName = null;
        string? attachmentOriginalFileName = null;
        string? attachmentContentType = null;

        if (model.Attachment != null && model.Attachment.Length > 0)
        {
            var saveResult = await _storageService.SaveInvoiceAsync(model.Attachment);
            if (saveResult.Success)
            {
                attachmentFileName = saveResult.StoredFileName;
                attachmentOriginalFileName = saveResult.OriginalFileName;
                attachmentContentType = saveResult.ContentType;
            }
        }

        var request = new ServiceRequest
        {
            TicketNumber = ticketNumber,
            ProductId = model.ProductId,
            UserId = CurrentUserId,
            ServiceCenterId = model.ServiceCenterId,
            Category = model.Category,
            Priority = model.Priority,
            Status = RequestStatus.Submitted,
            Subject = model.Subject.Trim(),
            Description = model.Description.Trim(),
            CampusLocation = model.CampusLocation.Trim(),
            SlaDeadlineUtc = slaDeadline,
            AssignedTechnician = suggestedTech,
            AttachmentFileName = attachmentFileName,
            AttachmentOriginalFileName = attachmentOriginalFileName,
            AttachmentContentType = attachmentContentType,
            CreatedAtUtc = utcNow
        };

        // Create initial audit log entry
        var initialHistory = new ServiceRequestHistory
        {
            ServiceRequest = request,
            FromStatus = RequestStatus.Submitted,
            ToStatus = RequestStatus.Submitted,
            ChangedByUserId = CurrentUserId,
            ChangedByUserName = CurrentUserName,
            Remarks = "Service ticket submitted by customer.",
            CreatedAtUtc = utcNow
        };

        _context.ServiceRequests.Add(request);
        _context.ServiceRequestHistories.Add(initialHistory);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Customer {UserId} raised service ticket {TicketNumber}", CurrentUserId, ticketNumber);

        TempData["SuccessMessage"] = $"Service ticket #{ticketNumber} raised successfully!";
        return RedirectToAction(nameof(Details), new { id = request.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id, string? reason = null)
    {
        var result = await _workflowService.CancelTicketByCustomerAsync(id, CurrentUserId, reason ?? "Cancelled by customer.");

        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["SuccessMessage"] = "Your service request has been cancelled successfully.";
        return RedirectToAction(nameof(Details), new { id });
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

    private static string FormatStatusText(RequestStatus status) => status switch
    {
        RequestStatus.Submitted => "Submitted",
        RequestStatus.UnderReview => "Under Review",
        RequestStatus.Assigned => "Assigned",
        RequestStatus.InProgress => "In Progress",
        RequestStatus.WaitingForCustomer => "Waiting for Customer",
        RequestStatus.Resolved => "Resolved",
        RequestStatus.Closed => "Closed",
        RequestStatus.Rejected => "Rejected",
        _ => status.ToString()
    };
}
