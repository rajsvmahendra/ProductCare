using ProductCare.Web.Models.Enums;

namespace ProductCare.Web.Services;

public record ProductSuggestion(
    string Name,
    string Brand,
    ProductCategory Category,
    string SubCategory,
    string ModelNumber,
    string Description
);

public interface IProductCatalogService
{
    IReadOnlyList<ProductSuggestion> SearchSuggestions(string? query, int maxResults = 8);
}

public class ProductCatalogService : IProductCatalogService
{
    private static readonly List<ProductSuggestion> Catalog = new()
    {
        // Apple
        new("MacBook Pro 16-inch (M3 Max)", "Apple", ProductCategory.Laptops, "Ultrabook", "A2485", "Apple M3 Max 16-core CPU, 40-core GPU, Liquid Retina XDR"),
        new("MacBook Pro 14-inch (M3 Pro)", "Apple", ProductCategory.Laptops, "Ultrabook", "A2992", "Apple M3 Pro 11-core CPU, 14-core GPU, Liquid Retina XDR"),
        new("MacBook Air 15-inch (M3)", "Apple", ProductCategory.Laptops, "Ultrabook", "A3114", "Apple M3 8-core CPU, 10-core GPU, Liquid Retina Display"),
        new("MacBook Air 13-inch (M3)", "Apple", ProductCategory.Laptops, "Ultrabook", "A3113", "Apple M3 8-core CPU, 8-core GPU, Fanless Design"),
        new("iPhone 16 Pro Max", "Apple", ProductCategory.MobilePhones, "Smartphone", "A3296", "A18 Pro chip, Grade 5 Titanium, 48MP Fusion Camera"),
        new("iPhone 16 Pro", "Apple", ProductCategory.MobilePhones, "Smartphone", "A3293", "A18 Pro chip, Grade 5 Titanium, Camera Control"),
        new("iPhone 15 Pro Max", "Apple", ProductCategory.MobilePhones, "Smartphone", "A2849", "A17 Pro chip, Titanium Frame, 5x Telephoto"),
        new("iPhone 15", "Apple", ProductCategory.MobilePhones, "Smartphone", "A3090", "A16 Bionic chip, Dynamic Island, 48MP Main Camera"),
        new("iPad Pro 13-inch (M4)", "Apple", ProductCategory.Tablets, "Pro Tablet", "A2925", "Apple M4 chip, Ultra Retina XDR OLED, Apple Pencil Pro"),
        new("iPad Air 11-inch (M2)", "Apple", ProductCategory.Tablets, "Tablet", "A2902", "Apple M2 chip, Liquid Retina display"),
        new("Apple Watch Ultra 2", "Apple", ProductCategory.Smartwatches, "Rugged Smartwatch", "A2986", "49mm Titanium case, Precision dual-frequency GPS"),
        new("Apple Watch Series 10", "Apple", ProductCategory.Smartwatches, "Smartwatch", "A2998", "Wide-angle OLED, Depth and water temperature sensors"),
        new("AirPods Max", "Apple", ProductCategory.AudioDevices, "Over-Ear ANC Headphones", "A2096", "Custom acoustic design, Active Noise Cancellation, Spatial Audio"),
        new("AirPods Pro 2", "Apple", ProductCategory.AudioDevices, "In-Ear ANC Earbuds", "A3048", "H2 chip, Active Noise Cancellation, USB-C MagSafe"),

        // Dell
        new("Dell XPS 15 (9530)", "Dell", ProductCategory.Laptops, "Ultrabook", "XPS9530", "13th Gen Intel Core i7/i9, OLED InfinityEdge, RTX 4070"),
        new("Dell XPS 13 Plus (9320)", "Dell", ProductCategory.Laptops, "Ultrabook", "XPS9320", "Zero-lattice keyboard, capacitive touch function row"),
        new("Dell Alienware m16 R2", "Dell", ProductCategory.Laptops, "Gaming Laptop", "AW-M16R2", "Intel Core Ultra 7/9, NVIDIA GeForce RTX 4070, Cryo-tech"),
        new("Dell Inspiron 16 Plus", "Dell", ProductCategory.Laptops, "All-Round Laptop", "INS-7630", "16-inch 2.5K display, Intel Core i7, 16GB RAM"),
        new("Dell UltraSharp 32 4K Monitor", "Dell", ProductCategory.Monitors, "Professional Monitor", "U3223QE", "IPS Black technology, 4K UHD, 90W USB-C Hub"),

        // HP
        new("HP Spectre x360 14", "HP", ProductCategory.Laptops, "2-in-1 Convertible", "14-ef000", "Intel Core Ultra 7, 2.8K OLED touch, 360-degree hinge"),
        new("HP Envy 16", "HP", ProductCategory.Laptops, "Creator Laptop", "16-h1000", "16-inch 120Hz display, Intel Core i7, NVIDIA RTX 4060"),
        new("HP Omen 16", "HP", ProductCategory.Laptops, "Gaming Laptop", "16-xf000", "AMD Ryzen 7 / Intel Core i7, Tempest Cooling, QHD 240Hz"),
        new("HP Pavilion Plus 14", "HP", ProductCategory.Laptops, "Standard Laptop", "14-ew000", "OLED display, Intel Core i5, Lightweight aluminum chassis"),

        // Lenovo
        new("Lenovo ThinkPad X1 Carbon Gen 11", "Lenovo", ProductCategory.Laptops, "Business Ultrabook", "21HM000", "Carbon-fiber top cover, Intel Core i7 vPro, 16:10 display"),
        new("Lenovo Legion Pro 7i", "Lenovo", ProductCategory.Laptops, "Gaming Laptop", "16IRX8H", "Intel Core i9-13900HX, RTX 4080/4090, Coldfront 5.0"),
        new("Lenovo Yoga 9i Dual Screen", "Lenovo", ProductCategory.Laptops, "Dual-Screen Convertible", "13IRU8", "Twin 13.3-inch 2.8K PureSight OLED screens, Bowers & Wilkins audio"),
        new("Lenovo IdeaPad Slim 5", "Lenovo", ProductCategory.Laptops, "Budget Laptop", "16ABR8", "AMD Ryzen 5, 16GB LPDDR5, military-grade durability"),

        // Samsung
        new("Samsung Galaxy S24 Ultra", "Samsung", ProductCategory.MobilePhones, "Flagship Smartphone", "SM-S928B", "Snapdragon 8 Gen 3, Titanium frame, 200MP camera, S Pen"),
        new("Samsung Galaxy S24+", "Samsung", ProductCategory.MobilePhones, "Smartphone", "SM-S926B", "6.7-inch QHD+ Dynamic AMOLED 2X, Galaxy AI features"),
        new("Samsung Galaxy Z Fold 6", "Samsung", ProductCategory.MobilePhones, "Foldable Smartphone", "SM-F956B", "7.6-inch Dynamic AMOLED main screen, Armor Aluminum frame"),
        new("Samsung Galaxy Tab S9 Ultra", "Samsung", ProductCategory.Tablets, "Flagship Tablet", "SM-X910", "14.6-inch Dynamic AMOLED 2X, S Pen included, IP68 rated"),
        new("Samsung Neo QLED 65-inch 4K TV", "Samsung", ProductCategory.Televisions, "Smart 4K TV", "QN65QN90D", "Quantum Matrix Technology, Neural Quantum Processor 4K"),

        // Sony
        new("Sony WH-1000XM5", "Sony", ProductCategory.AudioDevices, "Over-Ear ANC Headphones", "WH1000XM5", "Auto NC Optimizer, two processors, eight microphones, LDAC"),
        new("Sony WF-1000XM5", "Sony", ProductCategory.AudioDevices, "In-Ear ANC Earbuds", "WF1000XM5", "Dynamic Driver X, Integrated Processor V2, bone conduction sensor"),
        new("Sony Bravia XR A80L OLED 55-inch", "Sony", ProductCategory.Televisions, "Smart OLED TV", "XR-55A80L", "Cognitive Processor XR, Acoustic Surface Audio+, XR OLED Contrast Pro"),
        new("Sony PlayStation 5 Slim", "Sony", ProductCategory.GamingConsoles, "Gaming Console", "CFI-2000", "Ultra-high speed SSD, ray tracing, Tempest 3D AudioTech"),

        // LG
        new("LG C3 55-inch 4K OLED TV", "LG", ProductCategory.Televisions, "OLED Smart TV", "OLED55C3", "Self-lit OLED pixels, a9 AI Processor Gen6, Dolby Vision / Atmos"),
        new("LG UltraGear 27-inch OLED Gaming Monitor", "LG", ProductCategory.Monitors, "Gaming Monitor", "27GR95QE", "240Hz refresh rate, 0.03ms response time, QHD OLED"),

        // ASUS
        new("ASUS ROG Zephyrus G16", "ASUS", ProductCategory.Laptops, "Gaming Laptop", "GU605MY", "Intel Core Ultra 9, RTX 4090, 2.5K 240Hz OLED ROG Nebula display"),
        new("ASUS ZenBook 14 OLED", "ASUS", ProductCategory.Laptops, "Ultrabook", "UX3405MA", "Intel Core Ultra 7, 3K 120Hz ASUS Lumina OLED, 1.2 kg weight"),
        new("ASUS TUF Gaming A15", "ASUS", ProductCategory.Laptops, "Gaming Laptop", "FA507NV", "AMD Ryzen 7, NVIDIA RTX 4060, Military-grade MIL-STD-810H")
    };

    public IReadOnlyList<ProductSuggestion> SearchSuggestions(string? query, int maxResults = 8)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Array.Empty<ProductSuggestion>();
        }

        var normalized = query.Trim().ToLowerInvariant();

        return Catalog
            .Where(p =>
                p.Name.ToLowerInvariant().Contains(normalized) ||
                p.Brand.ToLowerInvariant().Contains(normalized) ||
                p.ModelNumber.ToLowerInvariant().Contains(normalized) ||
                p.SubCategory.ToLowerInvariant().Contains(normalized) ||
                p.Category.ToString().ToLowerInvariant().Contains(normalized))
            .Take(maxResults)
            .ToList();
    }
}
