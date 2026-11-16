namespace ProductCare.Web.Models.Enums;

/// <summary>
/// Dynamic warranty states determined by the centralized WarrantyCalculatorService.
/// </summary>
public enum WarrantyStatus
{
    Active = 1,
    ExpiringSoon = 2,
    Expired = 3
}
