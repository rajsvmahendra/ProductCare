using ProductCare.Web.Models.Entities;
using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.Services;

public record WorkflowResult(
    bool Success,
    string? ErrorMessage = null,
    ServiceRequest? Request = null
);

public interface IServiceRequestWorkflowService
{
    /// <summary>
    /// Checks whether transitioning from currentStatus to newStatus is permissible under workflow rules.
    /// </summary>
    bool IsTransitionValid(RequestStatus currentStatus, RequestStatus newStatus);

    /// <summary>
    /// Gets all permissible destination statuses from the specified current status.
    /// </summary>
    IReadOnlyList<RequestStatus> GetAllowedNextStatuses(RequestStatus currentStatus);

    /// <summary>
    /// Validates and executes a status change on a service request, recording an immutable audit history record.
    /// </summary>
    Task<WorkflowResult> TransitionStatusAsync(
        int requestId,
        RequestStatus newStatus,
        string changedByUserId,
        string changedByUserName,
        string? remarks = null,
        string? assignedTechnician = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Allows a customer to cancel their own ticket if it is still in the Submitted state.
    /// </summary>
    Task<WorkflowResult> CancelTicketByCustomerAsync(
        int requestId,
        string customerUserId,
        string reason,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Flags a ticket as escalated under SLA policy with recorded audit trail and supervisor remarks.
    /// EduRev P10 — M3 SLA & Escalation.
    /// </summary>
    Task<WorkflowResult> EscalateTicketAsync(
        int requestId,
        string changedByUserId,
        string changedByUserName,
        string reason,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Generates a collision-safe, human-readable ticket number in format PC-YYYYMMDD-XXXX.
    /// </summary>
    Task<string> GenerateTicketNumberAsync(CancellationToken cancellationToken = default);
}
