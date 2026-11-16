using Microsoft.AspNetCore.Identity;

namespace ProductCare.Web.Models.Entities;

/// <summary>
/// Application user entity extending ASP.NET Core IdentityUser.
/// Represents both Customers and Administrators.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? PostalCode { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    // Navigation properties
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();

    public virtual ICollection<ServiceRequestHistory> ServiceRequestHistories { get; set; } = new List<ServiceRequestHistory>();
}
