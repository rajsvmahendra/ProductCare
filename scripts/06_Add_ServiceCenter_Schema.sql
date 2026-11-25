-- ============================================================================
-- PRODUCTCARE: Service Center Directory & Smart Product Category Migration
-- Course: INT402: Modern Web Programming Tools and Techniques
-- Target: ProductCareDb on (localdb)\ProductCareTest
-- ============================================================================

USE [ProductCareDb];
GO

-- 1. Add SubCategory column to Products if not exists
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Products') AND name = 'SubCategory')
BEGIN
    ALTER TABLE [dbo].[Products]
    ADD [SubCategory] NVARCHAR(100) NULL;
END
GO

-- 2. Create ServiceCenters table if not exists
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ServiceCenters')
BEGIN
    CREATE TABLE [dbo].[ServiceCenters] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(200) NOT NULL,
        [Address] NVARCHAR(300) NOT NULL,
        [City] NVARCHAR(100) NOT NULL,
        [SupportedBrands] NVARCHAR(500) NOT NULL,
        [SupportedCategories] NVARCHAR(500) NOT NULL,
        [Phone] NVARCHAR(50) NOT NULL,
        [Email] NVARCHAR(150) NOT NULL,
        [OpeningHours] NVARCHAR(200) NOT NULL,
        [Website] NVARCHAR(255) NULL,
        [MapUrl] NVARCHAR(500) NULL,
        [IsActive] BIT NOT NULL CONSTRAINT DF_ServiceCenters_IsActive DEFAULT 1,
        [IsVerified] BIT NOT NULL CONSTRAINT DF_ServiceCenters_IsVerified DEFAULT 0,
        [Notes] NVARCHAR(1000) NULL,
        [CreatedAtUtc] DATETIME2 NOT NULL CONSTRAINT DF_ServiceCenters_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        CONSTRAINT [PK_ServiceCenters] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE NONCLUSTERED INDEX [IX_ServiceCenters_City]
        ON [dbo].[ServiceCenters] ([City] ASC);

    CREATE NONCLUSTERED INDEX [IX_ServiceCenters_IsActive]
        ON [dbo].[ServiceCenters] ([IsActive] ASC);
END
GO

-- 3. Add ServiceCenterId to ServiceRequests if not exists
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.ServiceRequests') AND name = 'ServiceCenterId')
BEGIN
    ALTER TABLE [dbo].[ServiceRequests]
    ADD [ServiceCenterId] INT NULL;

    ALTER TABLE [dbo].[ServiceRequests]
    ADD CONSTRAINT [FK_ServiceRequests_ServiceCenters_ServiceCenterId]
        FOREIGN KEY ([ServiceCenterId]) REFERENCES [dbo].[ServiceCenters] ([Id])
        ON DELETE SET NULL;

    CREATE NONCLUSTERED INDEX [IX_ServiceRequests_ServiceCenterId]
        ON [dbo].[ServiceRequests] ([ServiceCenterId] ASC);
END
GO

-- 4. Seed initial Service Centers if table is empty
IF NOT EXISTS (SELECT 1 FROM [dbo].[ServiceCenters])
BEGIN
    INSERT INTO [dbo].[ServiceCenters]
        ([Name], [Address], [City], [SupportedBrands], [SupportedCategories], [Phone], [Email], [OpeningHours], [Website], [MapUrl], [IsActive], [IsVerified], [Notes])
    VALUES
    (
        N'Metro Tech Care & Repairs',
        N'142 Commerce Boulevard, Suite 300',
        N'Springfield',
        N'Apple, Dell, HP, Lenovo, ASUS',
        N'Laptops, Computers, Tablets, Monitors',
        N'+1 (555) 234-5678',
        N'support@metrotechcare.demo',
        N'Mon-Fri: 9:00 AM - 6:00 PM, Sat: 10:00 AM - 3:00 PM',
        N'https://metrotechcare.example.com',
        N'https://maps.google.com/?q=Metro+Tech+Care+Springfield',
        1,
        0,
        N'Sample demonstration repair hub for laptop diagnostics, motherboard repair, and battery replacement.'
    ),
    (
        N'Apex Authorized Electronics Hub',
        N'880 Precision Way, Technology Park',
        N'Springfield',
        N'Sony, Samsung, LG, Bose, JBL',
        N'AudioDevices, Televisions, HomeEntertainment, Wearables',
        N'+1 (555) 876-5432',
        N'service@apexhub.demo',
        N'Mon-Sat: 10:00 AM - 7:00 PM',
        N'https://apexhub.example.com',
        N'https://maps.google.com/?q=Apex+Electronics+Hub+Springfield',
        1,
        1,
        N'Authorized manufacturer diagnostic facility for display calibration, audio driver replacement, and board service.'
    ),
    (
        N'Campus Express Device Lab',
        N'Student Union Building, Ground Floor, Room 12',
        N'University District',
        N'Apple, Samsung, Google, Dell, HP, Sony',
        N'MobilePhones, Laptops, Tablets, Smartwatches, GamingConsoles',
        N'+1 (555) 345-6789',
        N'express@campusdevicelab.demo',
        N'Mon-Fri: 8:30 AM - 7:00 PM, Sat: 9:00 AM - 1:00 PM',
        N'https://campusdevicelab.example.com',
        N'https://maps.google.com/?q=Campus+Express+Device+Lab',
        1,
        0,
        N'Sample on-campus rapid triage kiosk offering screen repairs, thermal re-pasting, and OS recovery.'
    ),
    (
        N'Precision Camera & Optical Service',
        N'45 Studio Lane, Creative Quarter',
        N'Downtown',
        N'Sony, Canon, Nikon, Fujifilm, DJI',
        N'Cameras, OtherElectronics',
        N'+1 (555) 456-7890',
        N'contact@precisioncamera.demo',
        N'Tue-Sat: 10:00 AM - 5:30 PM',
        N'https://precisioncamera.example.com',
        N'https://maps.google.com/?q=Precision+Camera+Service',
        1,
        0,
        N'Sample optical diagnostics lab for sensor cleaning, shutter mechanism rebuilds, and lens calibration.'
    );
END
GO

-- 5. Link existing sample service requests to appropriate service centres
UPDATE [dbo].[ServiceRequests]
SET [ServiceCenterId] = (SELECT TOP 1 [Id] FROM [dbo].[ServiceCenters] WHERE [Name] = N'Metro Tech Care & Repairs')
WHERE [TicketNumber] IN ('PC-20261110-A1B2', 'PC-20261111-B2C3') AND [ServiceCenterId] IS NULL;

UPDATE [dbo].[ServiceRequests]
SET [ServiceCenterId] = (SELECT TOP 1 [Id] FROM [dbo].[ServiceCenters] WHERE [Name] = N'Apex Authorized Electronics Hub')
WHERE [TicketNumber] IN ('PC-20261112-C3D4') AND [ServiceCenterId] IS NULL;

UPDATE [dbo].[ServiceRequests]
SET [ServiceCenterId] = (SELECT TOP 1 [Id] FROM [dbo].[ServiceCenters] WHERE [Name] = N'Campus Express Device Lab')
WHERE [TicketNumber] IN ('PC-20261113-D4E5', 'PC-20261114-E5F6') AND [ServiceCenterId] IS NULL;
GO
