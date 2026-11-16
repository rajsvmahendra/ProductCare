namespace ProductCare.Web.Models.Enums;

/// <summary>
/// Lifecycle statuses for service tickets managed via the ServiceRequestWorkflowService.
/// </summary>
public enum RequestStatus
{
    Submitted = 1,
    UnderReview = 2,
    Assigned = 3,
    InProgress = 4,
    WaitingForCustomer = 5,
    Resolved = 6,
    Closed = 7,
    Rejected = 8
}
