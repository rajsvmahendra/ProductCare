namespace ProductCare.Web.Models.Entities;

/// <summary>
/// Represents an authorized or demonstration service centre providing diagnostics, repairs, and warranty servicing.
/// </summary>
public class ServiceCenter
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Locality { get; set; }

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string? State { get; set; }

    public string? Country { get; set; } = "India";

    /// <summary>
    /// Comma-separated list of supported brands (e.g., "Apple, Dell, HP, Lenovo, ASUS").
    /// </summary>
    public string SupportedBrands { get; set; } = string.Empty;

    /// <summary>
    /// Comma-separated list of supported categories (e.g., "Laptops, Computers, Tablets, Monitors").
    /// </summary>
    public string SupportedCategories { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string OpeningHours { get; set; } = string.Empty;

    public string? Website { get; set; }

    public string? MapUrl { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indicates if this is a verified partner or a sample/demonstration center.
    /// Sample centres are clearly badge-labeled to avoid misleading users.
    /// </summary>
    public bool IsVerified { get; set; } = false;

    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
}
