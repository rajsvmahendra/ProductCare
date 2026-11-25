namespace ProductCare.Web.Models.Enums;

/// <summary>
/// Broad categorization of consumer products supported by ProductCare.
/// Preserves legacy category codes 1-8 and 99 while expanding granular categories.
/// </summary>
public enum ProductCategory
{
    Electronics = 1,
    Computers = 2,
    MobileDevices = 3,
    HomeAppliances = 4,
    KitchenAppliances = 5,
    HomeEntertainment = 6,
    Wearables = 7,
    OfficeEquipment = 8,
    Laptops = 9,
    DesktopComputers = 10,
    MobilePhones = 11,
    Tablets = 12,
    Smartwatches = 13,
    Televisions = 14,
    Monitors = 15,
    AudioDevices = 16,
    Cameras = 17,
    GamingConsoles = 18,
    OtherElectronics = 19,
    Other = 99
}
