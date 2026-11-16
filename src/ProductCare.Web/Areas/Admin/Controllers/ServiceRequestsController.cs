using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductCare.Web.Data;
using ProductCare.Web.Models.Enums;
using ProductCare.Web.Services;
using ProductCare.Web.ViewModels.Admin;
using ProductCare.Web.ViewModels.Customer;

namespace ProductCare.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = DbInitializerService.RoleAdmin)]
public class ServiceRequestsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IServiceRequestWorkflowService _workflowService;
    private readonly IWarrantyCalculatorService _warrantyCalculator;
    private readonly ILogger<ServiceRequestsController> _logger;

    public ServiceRequestsController(
        ApplicationDbContext context,
        IServiceRequestWorkflowService workflowService,
        IWarrantyCalculatorService warrantyCalculator,
        ILogger<ServiceRequestsController> logger)
    {
        _context = context;
        _workflowService = workflowService;
        _warrantyCalculator = warrantyCalculator;
        _logger = logger;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private string CurrentUserName => User.Identity?.Name ?? "Administrator";

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
            .Include(sr => sr.User)
            .AsQueryable();

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
                sr.User.FullName.Contains(term) ||
                sr.User.Email!.Contains(term) ||
                sr.Product.Name.Contains(term) ||
                sr.Product.SerialNumber.Contains(term));
        }

        var tickets = await query
            .OrderByDescending(sr => sr.CreatedAtUtc)
            .Select(sr => new AdminTicketListItem
            {
                Id = sr.Id,
                TicketNumber = sr.TicketNumber,
                CustomerId = sr.UserId,
                CustomerName = sr.User.FullName,
                CustomerEmail = sr.User.Email ?? "N/A",
                ProductId = sr.ProductId,
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
                AssignedTechnician = sr.AssignedTechnician,
                CreatedAtUtc = sr.CreatedAtUtc,
                UpdatedAtUtc = sr.UpdatedAtUtc
            })
            .ToListAsync();

        var pendingStatuses = new[] { RequestStatus.Submitted, RequestStatus.UnderReview };
        var inProgressStatuses = new[] { RequestStatus.Assigned, RequestStatus.InProgress, RequestStatus.WaitingForCustomer };

        var pendingCount = await _context.ServiceRequests.CountAsync(sr => pendingStatuses.Contains(sr.Status));
        var inProgressCount = await _context.ServiceRequests.CountAsync(sr => inProgressStatuses.Contains(sr.Status));
        var resolvedCount = await _context.ServiceRequests.CountAsync(sr => sr.Status == RequestStatus.Resolved);

        var model = new AdminServiceRequestListViewModel
        {
            Tickets = tickets,
            FilterStatus = status,
            FilterPriority = priority,
            FilterCategory = category,
            SearchTerm = search,
            TotalTickets = tickets.Count,
            PendingCount = pendingCount,
            InProgressCount = inProgressCount,
            ResolvedCount = resolvedCount
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var request = await _context.ServiceRequests
            .AsNoTracking()
            .Include(sr => sr.Product)
            .Include(sr => sr.User)
            .Include(sr => sr.StatusHistories.OrderBy(h => h.CreatedAtUtc))
            .FirstOrDefaultAsync(sr => sr.Id == id);

        if (request == null)
        {
            return NotFound();
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var warranty = _warrantyCalculator.GetWarrantyDetails(request.Product.WarrantyExpiryDate, today);

        var allowedNext = _workflowService.GetAllowedNextStatuses(request.Status);
        var allowedListItems = allowedNext.Select(s => new SelectListItem
        {
            Value = s.ToString(),
            Text = FormatStatusText(s)
        }).ToList();

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

        var model = new AdminServiceRequestDetailsViewModel
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
            CurrentAssignedTechnician = request.AssignedTechnician,
            CurrentAdminRemarks = request.AdminRemarks,
            CreatedAtUtc = request.CreatedAtUtc,
            UpdatedAtUtc = request.UpdatedAtUtc,
            ResolvedAtUtc = request.ResolvedAtUtc,
            CustomerId = request.UserId,
            CustomerName = request.User.FullName,
            CustomerEmail = request.User.Email ?? "N/A",
            CustomerPhone = request.User.PhoneNumber,
            CustomerCity = request.User.City,
            ProductId = request.ProductId,
            ProductName = request.Product.Name,
            ProductBrand = request.Product.Brand,
            ProductModelNumber = request.Product.ModelNumber,
            ProductSerialNumber = request.Product.SerialNumber,
            ProductWarrantyExpiryDate = request.Product.WarrantyExpiryDate,
            ProductWarrantyStatusBadge = warranty.StatusBadgeCssClass,
            ProductWarrantyStatusText = warranty.StatusDisplayText,
            AllowedNextStatuses = allowedListItems,
            StatusHistory = historyItems
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(
        int id,
        RequestStatus? newStatus,
        string? newAssignedTechnician,
        string? newRemarks)
    {
        if (!newStatus.HasValue)
        {
            TempData["ErrorMessage"] = "Please select a valid destination status.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var result = await _workflowService.TransitionStatusAsync(
            id,
            newStatus.Value,
            CurrentUserId,
            CurrentUserName,
            newRemarks,
            newAssignedTechnician);

        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["SuccessMessage"] = $"Ticket #{result.Request!.TicketNumber} status updated to '{FormatStatusText(newStatus.Value)}' successfully.";
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
