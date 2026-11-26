IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [FullName] nvarchar(100) NOT NULL,
        [Address] nvarchar(250) NULL,
        [City] nvarchar(100) NULL,
        [PostalCode] nvarchar(20) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE TABLE [ServiceTicketStatistics] (
        [CategoryName] nvarchar(max) NOT NULL,
        [TotalTickets] int NOT NULL,
        [SubmittedCount] int NOT NULL,
        [UnderReviewCount] int NOT NULL,
        [AssignedCount] int NOT NULL,
        [InProgressCount] int NOT NULL,
        [WaitingForCustomerCount] int NOT NULL,
        [ResolvedCount] int NOT NULL,
        [ClosedCount] int NOT NULL,
        [RejectedCount] int NOT NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE TABLE [Products] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Brand] nvarchar(100) NOT NULL,
        [ModelNumber] nvarchar(100) NOT NULL,
        [SerialNumber] nvarchar(100) NOT NULL,
        [Category] int NOT NULL,
        [PurchaseDate] date NOT NULL,
        [WarrantyDurationMonths] int NOT NULL,
        [WarrantyExpiryDate] date NOT NULL,
        [PurchaseStore] nvarchar(150) NULL,
        [PurchasePrice] decimal(18,2) NULL,
        [InvoiceFileName] nvarchar(255) NULL,
        [InvoiceOriginalFileName] nvarchar(255) NULL,
        [InvoiceContentType] nvarchar(100) NULL,
        [InvoiceFileSize] bigint NULL,
        [Notes] nvarchar(1000) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [UpdatedAtUtc] datetime2 NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Products_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE TABLE [ServiceRequests] (
        [Id] int NOT NULL IDENTITY,
        [TicketNumber] nvarchar(30) NOT NULL,
        [ProductId] int NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [Category] int NOT NULL,
        [Subject] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NOT NULL,
        [Priority] int NOT NULL,
        [Status] int NOT NULL,
        [AssignedTechnician] nvarchar(100) NULL,
        [AdminRemarks] nvarchar(2000) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [ResolvedAtUtc] datetime2 NULL,
        CONSTRAINT [PK_ServiceRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ServiceRequests_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ServiceRequests_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE TABLE [ServiceRequestHistories] (
        [Id] int NOT NULL IDENTITY,
        [ServiceRequestId] int NOT NULL,
        [FromStatus] int NOT NULL,
        [ToStatus] int NOT NULL,
        [ChangedByUserId] nvarchar(450) NOT NULL,
        [ChangedByUserName] nvarchar(100) NOT NULL,
        [Remarks] nvarchar(1000) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_ServiceRequestHistories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ServiceRequestHistories_AspNetUsers_ChangedByUserId] FOREIGN KEY ([ChangedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ServiceRequestHistories_ServiceRequests_ServiceRequestId] FOREIGN KEY ([ServiceRequestId]) REFERENCES [ServiceRequests] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Products_UserId] ON [Products] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Products_WarrantyExpiryDate] ON [Products] ([WarrantyExpiryDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [UX_Products_UserId_SerialNumber] ON [Products] ([UserId], [SerialNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ServiceRequestHistories_ChangedByUserId] ON [ServiceRequestHistories] ([ChangedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ServiceRequestHistories_CreatedAtUtc] ON [ServiceRequestHistories] ([CreatedAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ServiceRequestHistories_ServiceRequestId] ON [ServiceRequestHistories] ([ServiceRequestId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ServiceRequests_CreatedAtUtc] ON [ServiceRequests] ([CreatedAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ServiceRequests_ProductId] ON [ServiceRequests] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ServiceRequests_Status] ON [ServiceRequests] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ServiceRequests_UserId] ON [ServiceRequests] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [UX_ServiceRequests_TicketNumber] ON [ServiceRequests] ([TicketNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116110739_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261116110739_InitialCreate', N'10.0.15');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    ALTER TABLE [ServiceRequests] ADD [AttachmentContentType] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    ALTER TABLE [ServiceRequests] ADD [AttachmentFileName] nvarchar(255) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    ALTER TABLE [ServiceRequests] ADD [AttachmentOriginalFileName] nvarchar(255) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    ALTER TABLE [ServiceRequests] ADD [CampusLocation] nvarchar(255) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    ALTER TABLE [ServiceRequests] ADD [IsEscalated] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    ALTER TABLE [ServiceRequests] ADD [ResolutionProofFileName] nvarchar(255) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    ALTER TABLE [ServiceRequests] ADD [ServiceCenterId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    ALTER TABLE [ServiceRequests] ADD [SlaDeadlineUtc] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    ALTER TABLE [Products] ADD [ArchivedAtUtc] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    ALTER TABLE [Products] ADD [IsArchived] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    ALTER TABLE [Products] ADD [SubCategory] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    CREATE TABLE [NetworkQualityReports] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [CampusZone] nvarchar(150) NOT NULL,
        [NetworkType] nvarchar(100) NOT NULL,
        [DownloadSpeedMbps] float NOT NULL,
        [LatencyMs] int NOT NULL,
        [IsClientMeasured] bit NOT NULL,
        [Notes] nvarchar(1000) NULL,
        [MeasuredAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_NetworkQualityReports] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NetworkQualityReports_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    CREATE TABLE [ServiceCenters] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [Locality] nvarchar(100) NULL,
        [Address] nvarchar(300) NOT NULL,
        [City] nvarchar(100) NOT NULL,
        [State] nvarchar(50) NULL,
        [Country] nvarchar(50) NULL,
        [SupportedBrands] nvarchar(500) NOT NULL,
        [SupportedCategories] nvarchar(500) NOT NULL,
        [Phone] nvarchar(50) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [OpeningHours] nvarchar(200) NOT NULL,
        [Website] nvarchar(255) NULL,
        [MapUrl] nvarchar(500) NULL,
        [IsActive] bit NOT NULL,
        [IsVerified] bit NOT NULL,
        [Notes] nvarchar(1000) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_ServiceCenters] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    CREATE INDEX [IX_ServiceRequests_ServiceCenterId] ON [ServiceRequests] ([ServiceCenterId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    CREATE INDEX [IX_Products_UserId_IsArchived] ON [Products] ([UserId], [IsArchived]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    CREATE INDEX [IX_NetworkQualityReports_CampusZone] ON [NetworkQualityReports] ([CampusZone]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    CREATE INDEX [IX_NetworkQualityReports_MeasuredAtUtc] ON [NetworkQualityReports] ([MeasuredAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    CREATE INDEX [IX_NetworkQualityReports_UserId] ON [NetworkQualityReports] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    CREATE INDEX [IX_ServiceCenters_City] ON [ServiceCenters] ([City]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    CREATE INDEX [IX_ServiceCenters_IsActive] ON [ServiceCenters] ([IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    ALTER TABLE [ServiceRequests] ADD CONSTRAINT [FK_ServiceRequests_ServiceCenters_ServiceCenterId] FOREIGN KEY ([ServiceCenterId]) REFERENCES [ServiceCenters] ([Id]) ON DELETE SET NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261125104521_AddServiceCenterAndNetworkQuality'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261125104521_AddServiceCenterAndNetworkQuality', N'10.0.15');
END;

COMMIT;
GO

-- ============================================================================
-- PRODUCTCARE: Database Views & Stored Procedures
-- ============================================================================

CREATE OR ALTER VIEW dbo.vw_ProductWarrantySummary
AS
SELECT 
    p.Id AS ProductId,
    p.Name AS ProductName,
    p.Brand,
    p.ModelNumber,
    p.SerialNumber,
    CASE p.Category
        WHEN 1 THEN 'Electronics'
        WHEN 2 THEN 'Computers'
        WHEN 3 THEN 'Mobile Devices'
        WHEN 4 THEN 'Home Appliances'
        WHEN 5 THEN 'Kitchen Appliances'
        WHEN 6 THEN 'Home Entertainment'
        WHEN 7 THEN 'Wearables'
        WHEN 8 THEN 'Office Equipment'
        ELSE 'Other'
    END AS CategoryName,
    p.UserId,
    u.FullName AS CustomerName,
    ISNULL(u.Email, 'N/A') AS CustomerEmail,
    p.PurchaseDate,
    p.WarrantyDurationMonths,
    p.WarrantyExpiryDate,
    DATEDIFF(day, CAST(GETUTCDATE() AS DATE), p.WarrantyExpiryDate) AS DaysRemaining,
    CASE 
        WHEN CAST(GETUTCDATE() AS DATE) > p.WarrantyExpiryDate THEN 'Expired'
        WHEN DATEDIFF(day, CAST(GETUTCDATE() AS DATE), p.WarrantyExpiryDate) <= 30 THEN 'Expiring Soon'
        ELSE 'Active'
    END AS WarrantyStatusText
FROM dbo.Products p
INNER JOIN dbo.AspNetUsers u ON p.UserId = u.Id;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetServiceTicketStatistics
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CASE sr.Category
            WHEN 1 THEN 'Repair'
            WHEN 2 THEN 'Installation'
            WHEN 3 THEN 'Maintenance'
            WHEN 4 THEN 'Warranty Claim'
            ELSE 'Other'
        END AS CategoryName,
        COUNT(sr.Id) AS TotalTickets,
        SUM(CASE WHEN sr.Status = 1 THEN 1 ELSE 0 END) AS SubmittedCount,
        SUM(CASE WHEN sr.Status = 2 THEN 1 ELSE 0 END) AS UnderReviewCount,
        SUM(CASE WHEN sr.Status = 3 THEN 1 ELSE 0 END) AS AssignedCount,
        SUM(CASE WHEN sr.Status = 4 THEN 1 ELSE 0 END) AS InProgressCount,
        SUM(CASE WHEN sr.Status = 5 THEN 1 ELSE 0 END) AS WaitingForCustomerCount,
        SUM(CASE WHEN sr.Status = 6 THEN 1 ELSE 0 END) AS ResolvedCount,
        SUM(CASE WHEN sr.Status = 7 THEN 1 ELSE 0 END) AS ClosedCount,
        SUM(CASE WHEN sr.Status = 8 THEN 1 ELSE 0 END) AS RejectedCount
    FROM dbo.ServiceRequests sr
    WHERE (@StartDate IS NULL OR sr.CreatedAtUtc >= @StartDate)
      AND (@EndDate IS NULL OR sr.CreatedAtUtc <= @EndDate)
    GROUP BY sr.Category
    ORDER BY TotalTickets DESC;
END;
GO

