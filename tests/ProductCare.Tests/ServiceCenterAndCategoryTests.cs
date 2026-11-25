using ProductCare.Web.Models.Entities;
using ProductCare.Web.Models.Enums;
using Xunit;

namespace ProductCare.Tests;

public class ServiceCenterAndCategoryTests
{
    [Fact]
    public void ProductCategory_PreservesLegacyIntegerCodes()
    {
        Assert.Equal(1, (int)ProductCategory.Electronics);
        Assert.Equal(2, (int)ProductCategory.Computers);
        Assert.Equal(3, (int)ProductCategory.MobileDevices);
        Assert.Equal(4, (int)ProductCategory.HomeAppliances);
        Assert.Equal(5, (int)ProductCategory.KitchenAppliances);
        Assert.Equal(6, (int)ProductCategory.HomeEntertainment);
        Assert.Equal(7, (int)ProductCategory.Wearables);
        Assert.Equal(8, (int)ProductCategory.OfficeEquipment);
        Assert.Equal(99, (int)ProductCategory.Other);
    }

    [Fact]
    public void ProductCategory_SupportsExpandedGranularCategories()
    {
        Assert.Equal(9, (int)ProductCategory.Laptops);
        Assert.Equal(10, (int)ProductCategory.DesktopComputers);
        Assert.Equal(11, (int)ProductCategory.MobilePhones);
        Assert.Equal(12, (int)ProductCategory.Tablets);
        Assert.Equal(13, (int)ProductCategory.Smartwatches);
        Assert.Equal(14, (int)ProductCategory.Televisions);
        Assert.Equal(15, (int)ProductCategory.Monitors);
        Assert.Equal(16, (int)ProductCategory.AudioDevices);
        Assert.Equal(17, (int)ProductCategory.Cameras);
        Assert.Equal(18, (int)ProductCategory.GamingConsoles);
        Assert.Equal(19, (int)ProductCategory.OtherElectronics);
    }

    [Fact]
    public void ServiceCenter_DefaultValues_SafeAndConsistent()
    {
        var sc = new ServiceCenter
        {
            Name = "Campus Device Lab",
            Address = "Student Union Building",
            City = "University District",
            SupportedBrands = "Apple, Dell",
            SupportedCategories = "Laptops, MobilePhones",
            Phone = "+1 555 1234",
            Email = "lab@campus.edu",
            OpeningHours = "Mon-Fri 9-5"
        };

        Assert.True(sc.IsActive);
        Assert.False(sc.IsVerified); // Sample / demo by default to avoid misleading claims
        Assert.NotNull(sc.ServiceRequests);
        Assert.Empty(sc.ServiceRequests);
    }

    [Fact]
    public void Product_SupportsSubCategoryProperty()
    {
        var product = new Product
        {
            Name = "MacBook Pro 16",
            Brand = "Apple",
            ModelNumber = "A2485",
            SerialNumber = "C02G1001",
            Category = ProductCategory.Laptops,
            SubCategory = "Ultrabook"
        };

        Assert.Equal("Ultrabook", product.SubCategory);
        Assert.Equal(ProductCategory.Laptops, product.Category);
    }
}
