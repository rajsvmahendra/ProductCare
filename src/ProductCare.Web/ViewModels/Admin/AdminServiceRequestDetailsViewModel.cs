using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductCare.Web.Models.Enums;
using ProductCare.Web.ViewModels.Customer;

namespace ProductCare.Web.ViewModels.Admin;

public class AdminServiceRequestDetailsViewModel
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public RequestCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public PriorityLevel Priority { get; set; }
    public string PriorityBadgeCssClass { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    public string StatusBadgeCssClass { get; set; } = string.Empty;
    public string StatusDisplayText { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string? CurrentAssignedTechnician { get; set; }
    public string? CurrentAdminRemarks { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }

    // Customer details
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public string? CustomerCity { get; set; }

    // Associated Product details
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductBrand { get; set; } = string.Empty;
    public string ProductModelNumber { get; set; } = string.Empty;
    public string ProductSerialNumber { get; set; } = string.Empty;
    public DateOnly ProductWarrantyExpiryDate { get; set; }
    public string ProductWarrantyStatusBadge { get; set; } = string.Empty;
    public string ProductWarrantyStatusText { get; set; } = string.Empty;

    // Status transition action form
    [Display(Name = "Update Status To")]
    public RequestStatus? NewStatus { get; set; }

    [Display(Name = "Assign Technician")]
    [StringLength(100, ErrorMessage = "Technician name cannot exceed 100 characters.")]
    public string? NewAssignedTechnician { get; set; }

    [Display(Name = "Administrative Remarks / Notes")]
    [StringLength(2000, ErrorMessage = "Remarks cannot exceed 2000 characters.")]
    public string? NewRemarks { get; set; }

    public List<SelectListItem> AllowedNextStatuses { get; set; } = new();

    // Chronological status history list
    public List<ServiceRequestHistoryItem> StatusHistory { get; set; } = new();
}
