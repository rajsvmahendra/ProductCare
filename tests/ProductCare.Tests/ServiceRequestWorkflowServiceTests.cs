using Microsoft.Extensions.Logging.Abstractions;
using ProductCare.Web.Models.Enums;
using ProductCare.Web.Services;
using Xunit;

namespace ProductCare.Tests;

public class ServiceRequestWorkflowServiceTests
{
    private readonly ServiceRequestWorkflowService _workflowService;

    public ServiceRequestWorkflowServiceTests()
    {
        // Null context is safe because IsTransitionValid and GetAllowedNextStatuses do not touch the DB
        _workflowService = new ServiceRequestWorkflowService(null!, NullLogger<ServiceRequestWorkflowService>.Instance);
    }

    [Theory]
    [InlineData(RequestStatus.Submitted, RequestStatus.UnderReview, true)]
    [InlineData(RequestStatus.Submitted, RequestStatus.Rejected, true)]
    [InlineData(RequestStatus.Submitted, RequestStatus.Closed, true)]
    [InlineData(RequestStatus.Submitted, RequestStatus.Resolved, false)] // Cannot skip directly to Resolved
    [InlineData(RequestStatus.Submitted, RequestStatus.InProgress, false)] // Cannot skip UnderReview/Assigned
    [InlineData(RequestStatus.UnderReview, RequestStatus.Assigned, true)]
    [InlineData(RequestStatus.UnderReview, RequestStatus.WaitingForCustomer, true)]
    [InlineData(RequestStatus.UnderReview, RequestStatus.Rejected, true)]
    [InlineData(RequestStatus.UnderReview, RequestStatus.Resolved, false)]
    [InlineData(RequestStatus.Assigned, RequestStatus.InProgress, true)]
    [InlineData(RequestStatus.Assigned, RequestStatus.WaitingForCustomer, true)]
    [InlineData(RequestStatus.Assigned, RequestStatus.Rejected, true)]
    [InlineData(RequestStatus.InProgress, RequestStatus.Resolved, true)]
    [InlineData(RequestStatus.InProgress, RequestStatus.WaitingForCustomer, true)]
    [InlineData(RequestStatus.InProgress, RequestStatus.Rejected, true)]
    [InlineData(RequestStatus.WaitingForCustomer, RequestStatus.InProgress, true)]
    [InlineData(RequestStatus.WaitingForCustomer, RequestStatus.UnderReview, true)]
    [InlineData(RequestStatus.WaitingForCustomer, RequestStatus.Closed, true)]
    [InlineData(RequestStatus.Resolved, RequestStatus.Closed, true)]
    [InlineData(RequestStatus.Resolved, RequestStatus.InProgress, true)] // Permitted if reopened
    [InlineData(RequestStatus.Resolved, RequestStatus.UnderReview, false)]
    [InlineData(RequestStatus.Closed, RequestStatus.InProgress, true)] // Admin reopen permitted
    [InlineData(RequestStatus.Closed, RequestStatus.UnderReview, false)]
    [InlineData(RequestStatus.Rejected, RequestStatus.Submitted, false)] // Terminal state
    [InlineData(RequestStatus.Rejected, RequestStatus.InProgress, false)] // Terminal state
    [InlineData(RequestStatus.Submitted, RequestStatus.Submitted, false)] // Same status invalid
    public void IsTransitionValid_EnforcesWorkflowStateMachineRules(RequestStatus current, RequestStatus target, bool expected)
    {
        var isValid = _workflowService.IsTransitionValid(current, target);
        Assert.Equal(expected, isValid);
    }

    [Fact]
    public void GetAllowedNextStatuses_ForSubmitted_ReturnsPermittedStatuses()
    {
        var next = _workflowService.GetAllowedNextStatuses(RequestStatus.Submitted);

        Assert.Contains(RequestStatus.UnderReview, next);
        Assert.Contains(RequestStatus.Rejected, next);
        Assert.Contains(RequestStatus.Closed, next);
        Assert.DoesNotContain(RequestStatus.Resolved, next);
    }

    [Fact]
    public void GetAllowedNextStatuses_ForRejected_ReturnsEmpty()
    {
        var next = _workflowService.GetAllowedNextStatuses(RequestStatus.Rejected);
        Assert.Empty(next);
    }
}
