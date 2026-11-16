using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductCare.Web.Data;
using ProductCare.Web.Models.Entities;
using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.Services;

/// <summary>
/// Domain service governing service ticket lifecycle transitions and audit trail generation.
/// Enforces business state rules and prevents unauthorized or nonsensical status progressions.
/// </summary>
public class ServiceRequestWorkflowService : IServiceRequestWorkflowService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ServiceRequestWorkflowService> _logger;

    // Explicit state transition matrix
    private static readonly Dictionary<RequestStatus, HashSet<RequestStatus>> ValidTransitions = new()
    {
        [RequestStatus.Submitted] = new()
        {
            RequestStatus.UnderReview,
            RequestStatus.Rejected,
            RequestStatus.Closed
        },
        [RequestStatus.UnderReview] = new()
        {
            RequestStatus.Assigned,
            RequestStatus.WaitingForCustomer,
            RequestStatus.Rejected
        },
        [RequestStatus.Assigned] = new()
        {
            RequestStatus.InProgress,
            RequestStatus.WaitingForCustomer,
            RequestStatus.Rejected
        },
        [RequestStatus.InProgress] = new()
        {
            RequestStatus.Resolved,
            RequestStatus.WaitingForCustomer,
            RequestStatus.Rejected
        },
        [RequestStatus.WaitingForCustomer] = new()
        {
            RequestStatus.InProgress,
            RequestStatus.UnderReview,
            RequestStatus.Closed
        },
        [RequestStatus.Resolved] = new()
        {
            RequestStatus.Closed,
            RequestStatus.InProgress // Permitted if customer reports issue persists
        },
        [RequestStatus.Closed] = new()
        {
            RequestStatus.InProgress // Admin reopen for edge cases
        },
        [RequestStatus.Rejected] = new()
        {
            // Terminal state
        }
    };

    public ServiceRequestWorkflowService(
        ApplicationDbContext context,
        ILogger<ServiceRequestWorkflowService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public bool IsTransitionValid(RequestStatus currentStatus, RequestStatus newStatus)
    {
        if (currentStatus == newStatus)
        {
            return false;
        }

        return ValidTransitions.TryGetValue(currentStatus, out var allowed) && allowed.Contains(newStatus);
    }

    public IReadOnlyList<RequestStatus> GetAllowedNextStatuses(RequestStatus currentStatus)
    {
        if (ValidTransitions.TryGetValue(currentStatus, out var allowed))
        {
            return allowed.ToList();
        }

        return Array.Empty<RequestStatus>();
    }

    public async Task<WorkflowResult> TransitionStatusAsync(
        int requestId,
        RequestStatus newStatus,
        string changedByUserId,
        string changedByUserName,
        string? remarks = null,
        string? assignedTechnician = null,
        CancellationToken cancellationToken = default)
    {
        var request = await _context.ServiceRequests
            .Include(sr => sr.Product)
            .Include(sr => sr.User)
            .FirstOrDefaultAsync(sr => sr.Id == requestId, cancellationToken);

        if (request == null)
        {
            return new WorkflowResult(false, $"Service request #{requestId} was not found.");
        }

        var currentStatus = request.Status;

        if (!IsTransitionValid(currentStatus, newStatus))
        {
            _logger.LogWarning(
                "Invalid status transition requested for Ticket #{TicketNumber}: {CurrentStatus} -> {NewStatus}",
                request.TicketNumber, currentStatus, newStatus);
            return new WorkflowResult(false, $"Transition from '{currentStatus}' to '{newStatus}' is not allowed.");
        }

        var utcNow = DateTime.UtcNow;

        // Create immutable audit history record
        var history = new ServiceRequestHistory
        {
            ServiceRequestId = request.Id,
            FromStatus = currentStatus,
            ToStatus = newStatus,
            ChangedByUserId = changedByUserId,
            ChangedByUserName = changedByUserName,
            Remarks = remarks,
            CreatedAtUtc = utcNow
        };

        // Update request properties
        request.Status = newStatus;
        request.UpdatedAtUtc = utcNow;

        if (newStatus == RequestStatus.Resolved)
        {
            request.ResolvedAtUtc = utcNow;
        }

        if (!string.IsNullOrWhiteSpace(assignedTechnician))
        {
            request.AssignedTechnician = assignedTechnician.Trim();
        }

        if (!string.IsNullOrWhiteSpace(remarks))
        {
            request.AdminRemarks = remarks.Trim();
        }

        _context.ServiceRequestHistories.Add(history);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Ticket #{TicketNumber} transitioned from {CurrentStatus} to {NewStatus} by {UserName}",
            request.TicketNumber, currentStatus, newStatus, changedByUserName);

        return new WorkflowResult(true, null, request);
    }

    public async Task<WorkflowResult> CancelTicketByCustomerAsync(
        int requestId,
        string customerUserId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        var request = await _context.ServiceRequests
            .FirstOrDefaultAsync(sr => sr.Id == requestId && sr.UserId == customerUserId, cancellationToken);

        if (request == null)
        {
            return new WorkflowResult(false, "Service request was not found or access is denied.");
        }

        if (request.Status != RequestStatus.Submitted)
        {
            return new WorkflowResult(false, "Tickets can only be cancelled while in 'Submitted' status before review starts.");
        }

        var cancellationRemarks = string.IsNullOrWhiteSpace(reason)
            ? "Cancelled by customer before processing."
            : $"Cancelled by customer: {reason}";

        return await TransitionStatusAsync(
            requestId,
            RequestStatus.Closed,
            customerUserId,
            "Customer",
            cancellationRemarks,
            cancellationToken: cancellationToken
        );
    }

    public async Task<string> GenerateTicketNumberAsync(CancellationToken cancellationToken = default)
    {
        const string chars = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ"; // Ambiguity-free alphanumeric
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");

        for (var attempt = 0; attempt < 10; attempt++)
        {
            var randomBytes = new byte[4];
            RandomNumberGenerator.Fill(randomBytes);
            var randomChars = new char[4];
            for (var i = 0; i < 4; i++)
            {
                randomChars[i] = chars[randomBytes[i] % chars.Length];
            }

            var candidate = $"PC-{datePart}-{new string(randomChars)}";

            var exists = await _context.ServiceRequests
                .AnyAsync(sr => sr.TicketNumber == candidate, cancellationToken);

            if (!exists)
            {
                return candidate;
            }
        }

        // Fallback with GUID segment
        return $"PC-{datePart}-{Guid.NewGuid().ToString("N")[..4].ToUpperInvariant()}";
    }
}
