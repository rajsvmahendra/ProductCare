using System.ComponentModel.DataAnnotations;
using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.ViewModels.Api;

public class ServiceRequestDto
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductBrand { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public RequestCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public PriorityLevel Priority { get; set; }
    public RequestStatus Status { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? AssignedTechnician { get; set; }
    public string? AdminRemarks { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
}

public class CreateServiceRequestDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public RequestCategory Category { get; set; } = RequestCategory.Repair;

    [Required]
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

    [Required]
    [StringLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;
}

public class UpdateTicketStatusDto
{
    [Required]
    public RequestStatus NewStatus { get; set; }

    [StringLength(100)]
    public string? AssignedTechnician { get; set; }

    [StringLength(2000)]
    public string? Remarks { get; set; }
}
