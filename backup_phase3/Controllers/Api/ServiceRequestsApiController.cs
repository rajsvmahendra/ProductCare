using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCare.Web.Data;
using ProductCare.Web.Models.Entities;
using ProductCare.Web.Models.Enums;
using ProductCare.Web.Services;
using ProductCare.Web.ViewModels.Api;
using ProductCare.Web.ViewModels.Common;

namespace ProductCare.Web.Controllers.Api;

[ApiController]
[Route("api/servicerequests")]
[Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},Identity.Application")]
[Produces("application/json")]
public class ServiceRequestsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IServiceRequestWorkflowService _workflowService;
    private readonly ILogger<ServiceRequestsApiController> _logger;

    public ServiceRequestsApiController(
        ApplicationDbContext context,
        IServiceRequestWorkflowService workflowService,
        ILogger<ServiceRequestsApiController> logger)
    {
        _context = context;
        _workflowService = workflowService;
        _logger = logger;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private string CurrentUserName => User.Identity?.Name ?? "API Consumer";
    private bool IsAdmin => User.IsInRole(DbInitializerService.RoleAdmin);

    /// <summary>
    /// Retrieves service requests (customer sees own requests; administrator sees all).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ServiceRequestDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] RequestStatus? status,
        [FromQuery] RequestCategory? category,
        [FromQuery] PriorityLevel? priority)
    {
        var query = _context.ServiceRequests
            .AsNoTracking()
            .Include(sr => sr.Product)
            .Include(sr => sr.User)
            .AsQueryable();

        if (!IsAdmin)
        {
            query = query.Where(sr => sr.UserId == CurrentUserId);
        }

        if (status.HasValue)
        {
            query = query.Where(sr => sr.Status == status.Value);
        }

        if (category.HasValue)
        {
            query = query.Where(sr => sr.Category == category.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(sr => sr.Priority == priority.Value);
        }

        var tickets = await query
            .OrderByDescending(sr => sr.CreatedAtUtc)
            .Select(sr => MapToDto(sr))
            .ToListAsync();

        return Ok(ApiResponse<List<ServiceRequestDto>>.Ok(tickets));
    }

    /// <summary>
    /// Retrieves a single service request by ID, enforcing ownership or administrative role.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ServiceRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var ticket = await _context.ServiceRequests
            .AsNoTracking()
            .Include(sr => sr.Product)
            .Include(sr => sr.User)
            .FirstOrDefaultAsync(sr => sr.Id == id);

        if (ticket == null || (!IsAdmin && ticket.UserId != CurrentUserId))
        {
            return NotFound(ApiResponse<object>.Fail($"Service request #{id} was not found."));
        }

        return Ok(ApiResponse<ServiceRequestDto>.Ok(MapToDto(ticket)));
    }

    /// <summary>
    /// Raises a new service request against a registered product owned by the authenticated customer.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ServiceRequestDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateServiceRequestDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == dto.ProductId && p.UserId == CurrentUserId);

        if (product == null)
        {
            return BadRequest(ApiResponse<object>.Fail("Selected product does not exist or does not belong to your account."));
        }

        var ticketNumber = await _workflowService.GenerateTicketNumberAsync();
        var utcNow = DateTime.UtcNow;

        var request = new ServiceRequest
        {
            TicketNumber = ticketNumber,
            ProductId = dto.ProductId,
            UserId = CurrentUserId,
            Category = dto.Category,
            Priority = dto.Priority,
            Status = RequestStatus.Submitted,
            Subject = dto.Subject.Trim(),
            Description = dto.Description.Trim(),
            CreatedAtUtc = utcNow
        };

        var history = new ServiceRequestHistory
        {
            ServiceRequest = request,
            FromStatus = RequestStatus.Submitted,
            ToStatus = RequestStatus.Submitted,
            ChangedByUserId = CurrentUserId,
            ChangedByUserName = CurrentUserName,
            Remarks = "Ticket submitted via REST API.",
            CreatedAtUtc = utcNow
        };

        _context.ServiceRequests.Add(request);
        _context.ServiceRequestHistories.Add(history);
        await _context.SaveChangesAsync();

        request.Product = product;
        var user = await _context.Users.FindAsync(CurrentUserId);
        request.User = user!;

        return CreatedAtAction(nameof(GetById), new { id = request.Id }, ApiResponse<ServiceRequestDto>.Ok(MapToDto(request), "Ticket created successfully."));
    }

    /// <summary>
    /// Updates service ticket status adhering to state machine transition validation rules (Admin only).
    /// </summary>
    [HttpPut("{id:int}/status")]
    [Authorize(Roles = DbInitializerService.RoleAdmin)]
    [ProducesResponseType(typeof(ApiResponse<ServiceRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTicketStatusDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var result = await _workflowService.TransitionStatusAsync(
            id,
            dto.NewStatus,
            CurrentUserId,
            CurrentUserName,
            dto.Remarks,
            dto.AssignedTechnician);

        if (!result.Success)
        {
            return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? "Status transition failed."));
        }

        return Ok(ApiResponse<ServiceRequestDto>.Ok(MapToDto(result.Request!), "Ticket status transitioned successfully."));
    }

    /// <summary>
    /// Cancels a submitted service request (Customer only, allowed only while in Submitted state).
    /// </summary>
    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel(int id, [FromQuery] string? reason)
    {
        var result = await _workflowService.CancelTicketByCustomerAsync(id, CurrentUserId, reason ?? "Cancelled via API.");

        if (!result.Success)
        {
            return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage ?? "Cancellation failed."));
        }

        return Ok(ApiResponse<object>.Ok(new { TicketId = id, Status = "Closed" }, "Ticket cancelled successfully."));
    }

    private static ServiceRequestDto MapToDto(ServiceRequest sr)
    {
        return new ServiceRequestDto
        {
            Id = sr.Id,
            TicketNumber = sr.TicketNumber,
            ProductId = sr.ProductId,
            ProductName = sr.Product?.Name ?? "N/A",
            ProductBrand = sr.Product?.Brand ?? "N/A",
            CustomerId = sr.UserId,
            CustomerName = sr.User?.FullName ?? "N/A",
            Category = sr.Category,
            CategoryName = sr.Category.ToString(),
            Priority = sr.Priority,
            Status = sr.Status,
            StatusText = sr.Status.ToString(),
            Subject = sr.Subject,
            Description = sr.Description,
            AssignedTechnician = sr.AssignedTechnician,
            AdminRemarks = sr.AdminRemarks,
            CreatedAtUtc = sr.CreatedAtUtc,
            UpdatedAtUtc = sr.UpdatedAtUtc,
            ResolvedAtUtc = sr.ResolvedAtUtc
        };
    }
}
