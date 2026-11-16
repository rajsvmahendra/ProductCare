namespace ProductCare.Web.Models.Entities;

/// <summary>
/// Keyless entity mapped to the SQL Server view 'vw_ProductWarrantySummary'.
/// Provides aggregated warranty intelligence combining Product and User records.
/// </summary>
public class ProductWarrantySummaryView
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string ModelNumber { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateOnly PurchaseDate { get; set; }
    public int WarrantyDurationMonths { get; set; }
    public DateOnly WarrantyExpiryDate { get; set; }
    public int DaysRemaining { get; set; }
    public string WarrantyStatusText { get; set; } = string.Empty;
}
