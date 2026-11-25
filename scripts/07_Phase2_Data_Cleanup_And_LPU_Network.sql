-- ============================================================================
-- PRODUCTCARE: Phase 2 Master Redesign Database Migration & Cleanup
-- Course: INT402: Modern Web Programming Tools and Techniques
-- Target: ProductCareDb on (localdb)\ProductCareTest
-- ============================================================================

USE [ProductCareDb];
GO

-- 1. Add Locality, State, Country columns to ServiceCenters if not exists
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.ServiceCenters') AND name = 'Locality')
BEGIN
    ALTER TABLE [dbo].[ServiceCenters] ADD [Locality] NVARCHAR(100) NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.ServiceCenters') AND name = 'State')
BEGIN
    ALTER TABLE [dbo].[ServiceCenters] ADD [State] NVARCHAR(50) NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.ServiceCenters') AND name = 'Country')
BEGIN
    ALTER TABLE [dbo].[ServiceCenters] ADD [Country] NVARCHAR(50) NULL;
END
GO

-- 2. Add IsArchived, ArchivedAtUtc columns to Products if not exists
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Products') AND name = 'IsArchived')
BEGIN
    ALTER TABLE [dbo].[Products] ADD [IsArchived] BIT NOT NULL CONSTRAINT DF_Products_IsArchived DEFAULT 0;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Products') AND name = 'ArchivedAtUtc')
BEGIN
    ALTER TABLE [dbo].[Products] ADD [ArchivedAtUtc] DATETIME2 NULL;
END
GO

-- 3. Safely clean up unwanted QA duplicates and junk test entries from rajasvmahendra@gmail.com
-- Preserve product 4005 (which has service ticket 3004 / PC-20261121-UAJJ)
-- Remove only confirmed unreferenced QA duplicates 1004, 1005, 2005, 3005
DECLARE @PersonalUserId NVARCHAR(450) = (SELECT TOP 1 [Id] FROM [dbo].[AspNetUsers] WHERE [Email] = 'rajasvmahendra@gmail.com');

IF @PersonalUserId IS NOT NULL
BEGIN
    -- Delete junk and unreferenced QA duplicates
    DELETE FROM [dbo].[Products]
    WHERE [UserId] = @PersonalUserId
      AND [Id] IN (1004, 1005, 2005, 3005)
      AND NOT EXISTS (SELECT 1 FROM [dbo].[ServiceRequests] sr WHERE sr.[ProductId] = [Products].[Id]);

    -- Verify Product 4005 details are clean and verified
    UPDATE [dbo].[Products]
    SET [Name] = 'MacBook Pro 16-inch M3 Max',
        [Brand] = 'Apple',
        [Category] = 9, -- Laptops
        [SubCategory] = 'Ultrabook',
        [ModelNumber] = 'A2485',
        [SerialNumber] = 'C02G73276936',
        [PurchaseDate] = '2026-01-15',
        [WarrantyDurationMonths] = 24,
        [WarrantyExpiryDate] = '2028-01-15',
        [PurchaseStore] = 'Apple Store Online',
        [PurchasePrice] = 2499.00,
        [Notes] = '16-inch Liquid Retina XDR display, Apple M3 Max with 16-core CPU and 40-core GPU, 36GB Unified Memory, 1TB SSD.'
    WHERE [Id] = 4005 AND [UserId] = @PersonalUserId;

    -- Add iPhone 15 Pro Max (Expiring soon) if not exists
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [UserId] = @PersonalUserId AND [SerialNumber] = 'F2LX7789P9')
    BEGIN
        INSERT INTO [dbo].[Products]
            ([UserId], [Name], [Brand], [Category], [SubCategory], [ModelNumber], [SerialNumber], [PurchaseDate], [WarrantyDurationMonths], [WarrantyExpiryDate], [PurchaseStore], [PurchasePrice], [Notes], [CreatedAtUtc], [IsArchived])
        VALUES
            (@PersonalUserId, 'iPhone 15 Pro Max', 'Apple', 11, 'Smartphones', 'A2849', 'F2LX7789P9', '2025-12-10', 12, '2026-12-10', 'Apple Store Online', 1199.00, 'Natural Titanium 256GB, A17 Pro chip, Super Retina XDR display with ProMotion.', SYSUTCDATETIME(), 0);
    END

    -- Add Sony WH-1000XM5 (Expired) if not exists
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [UserId] = @PersonalUserId AND [SerialNumber] = 'SN-SONY-662288')
    BEGIN
        INSERT INTO [dbo].[Products]
            ([UserId], [Name], [Brand], [Category], [SubCategory], [ModelNumber], [SerialNumber], [PurchaseDate], [WarrantyDurationMonths], [WarrantyExpiryDate], [PurchaseStore], [PurchasePrice], [Notes], [CreatedAtUtc], [IsArchived])
        VALUES
            (@PersonalUserId, 'Sony WH-1000XM5 Noise Canceling', 'Sony', 16, 'Over-Ear Headphones', 'WH1000XM5', 'SN-SONY-662288', '2024-05-15', 12, '2025-05-15', 'Croma Electronics', 349.00, 'Industry-leading noise cancellation with two processors and eight microphones. Integrated 3.5mm cable and hard travel case.', SYSUTCDATETIME(), 0);
    END
END
GO

-- 4. Reconstruct ServiceCenters table to focus on Lovely Professional University, Phagwara & Jalandhar, Punjab
-- Remove old Springfield / US mock data
DELETE FROM [dbo].[ServiceCenters];
GO

-- Insert authentic LPU, Phagwara, Jalandhar service hubs with truthful sample listing labels
SET IDENTITY_INSERT [dbo].[ServiceCenters] ON;

INSERT INTO [dbo].[ServiceCenters]
    ([Id], [Name], [Locality], [Address], [City], [State], [Country], [SupportedBrands], [SupportedCategories], [Phone], [Email], [OpeningHours], [Website], [MapUrl], [IsActive], [IsVerified], [Notes])
VALUES
(
    1,
    N'LPU Uni-Mall Device Care & Triage Kiosk',
    N'LPU Campus Area',
    N'Uni-Mall, Ground Floor, Central Plaza, Lovely Professional University',
    N'Phagwara',
    N'Punjab',
    N'India',
    N'Apple, Dell, HP, Lenovo, ASUS, Samsung',
    N'Laptops, MobilePhones, Tablets, AudioDevices, Smartwatches',
    N'+91 1824 517000',
    N'campuscare@unimall.sample.lpu.in',
    N'Mon-Sat: 9:00 AM - 7:30 PM, Sun: 11:00 AM - 4:00 PM',
    N'https://www.lpu.in',
    N'https://maps.google.com/?q=Lovely+Professional+University+Uni+Mall+Phagwara',
    1,
    0, -- Sample Listing
    N'Sample demonstration repair kiosk on LPU campus for rapid student diagnostics, thermal servicing, OS restoration, and screen inspection.'
),
(
    2,
    N'Phagwara GT Road Multi-Brand Hardware Service Hub',
    N'Guru Hargobind Nagar / GT Road',
    N'Shop 14, Opposite Main Bus Stand, Grand Trunk Road',
    N'Phagwara',
    N'Punjab',
    N'India',
    N'Dell, HP, Lenovo, Acer, ASUS, MSI',
    N'Laptops, DesktopComputers, Monitors, GamingConsoles',
    N'+91 1824 260100',
    N'support@phagwaracare.sample.in',
    N'Mon-Sat: 9:30 AM - 6:30 PM',
    NULL,
    N'https://maps.google.com/?q=Grand+Trunk+Road+Phagwara+Punjab',
    1,
    0, -- Sample Listing
    N'Sample demonstration regional electronics lab providing motherboard repairs, battery replacements, and display panel calibration.'
),
(
    3,
    N'Jalandhar Model Town Electronics Care Centre',
    N'Model Town',
    N'88 Circular Road, Near Model Town Central Market',
    N'Jalandhar',
    N'Punjab',
    N'India',
    N'Apple, Sony, Samsung, Bose, LG, OnePlus',
    N'Laptops, MobilePhones, AudioDevices, Televisions, Tablets',
    N'+91 181 2450800',
    N'care@jalandharhub.sample.in',
    N'Mon-Sat: 10:00 AM - 7:00 PM',
    NULL,
    N'https://maps.google.com/?q=Model+Town+Jalandhar+Punjab',
    1,
    0, -- Sample Listing
    N'Sample demonstration urban service centre specializing in audio driver replacement, TV backlight repairs, and certified multi-brand hardware triage.'
),
(
    4,
    N'Law Gate Student Device Assistance Depot',
    N'Law Gate / Maheru Market',
    N'Shop 22, University Market Road, Maheru',
    N'Phagwara',
    N'Punjab',
    N'India',
    N'Apple, Samsung, Xiaomi, OnePlus, Realme, HP',
    N'MobilePhones, Laptops, Smartwatches, AudioDevices',
    N'+91 1824 501234',
    N'contact@lawgatedepot.sample.in',
    N'Mon-Sun: 10:00 AM - 9:00 PM',
    NULL,
    N'https://maps.google.com/?q=Law+Gate+Maheru+Phagwara+Punjab',
    1,
    0, -- Sample Listing
    N'Sample demonstration walk-in assistance depot located near LPU Law Gate, offering quick diagnostics, charging port cleaning, and accessory tests.'
);

SET IDENTITY_INSERT [dbo].[ServiceCenters] OFF;
GO

-- 5. Link existing service requests to LPU Uni-Mall Device Care & Triage Kiosk (ID 1)
UPDATE [dbo].[ServiceRequests]
SET [ServiceCenterId] = 1
WHERE [ServiceCenterId] IS NULL OR [ServiceCenterId] NOT IN (1, 2, 3, 4);
GO
