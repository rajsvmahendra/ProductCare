-- ============================================================================
-- PRODUCTCARE: P10 Campus Issue, Maintenance & Network Quality Management Migration
-- Course: INT402: Modern Web Programming Tools and Techniques
-- Target: ProductCareDb on (localdb)\ProductCareTest
-- ============================================================================

USE [ProductCareDb];
GO

-- 1. Add P10 columns to ServiceRequests if they don't already exist
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.ServiceRequests') AND name = 'CampusLocation')
BEGIN
    ALTER TABLE [dbo].[ServiceRequests]
    ADD [CampusLocation] NVARCHAR(255) NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.ServiceRequests') AND name = 'SlaDeadlineUtc')
BEGIN
    ALTER TABLE [dbo].[ServiceRequests]
    ADD [SlaDeadlineUtc] DATETIME2 NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.ServiceRequests') AND name = 'IsEscalated')
BEGIN
    ALTER TABLE [dbo].[ServiceRequests]
    ADD [IsEscalated] BIT NOT NULL CONSTRAINT DF_ServiceRequests_IsEscalated DEFAULT 0;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.ServiceRequests') AND name = 'AttachmentFileName')
BEGIN
    ALTER TABLE [dbo].[ServiceRequests]
    ADD [AttachmentFileName] NVARCHAR(255) NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.ServiceRequests') AND name = 'AttachmentOriginalFileName')
BEGIN
    ALTER TABLE [dbo].[ServiceRequests]
    ADD [AttachmentOriginalFileName] NVARCHAR(255) NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.ServiceRequests') AND name = 'AttachmentContentType')
BEGIN
    ALTER TABLE [dbo].[ServiceRequests]
    ADD [AttachmentContentType] NVARCHAR(100) NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.ServiceRequests') AND name = 'ResolutionProofFileName')
BEGIN
    ALTER TABLE [dbo].[ServiceRequests]
    ADD [ResolutionProofFileName] NVARCHAR(255) NULL;
END
GO

-- 2. Create NetworkQualityReports table if not exists
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'NetworkQualityReports')
BEGIN
    CREATE TABLE [dbo].[NetworkQualityReports] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] NVARCHAR(450) NOT NULL,
        [CampusZone] NVARCHAR(150) NOT NULL,
        [NetworkType] NVARCHAR(100) NOT NULL,
        [DownloadSpeedMbps] FLOAT NOT NULL,
        [LatencyMs] INT NOT NULL,
        [IsClientMeasured] BIT NOT NULL,
        [Notes] NVARCHAR(1000) NULL,
        [MeasuredAtUtc] DATETIME2 NOT NULL,
        CONSTRAINT [PK_NetworkQualityReports] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_NetworkQualityReports_AspNetUsers_UserId] FOREIGN KEY ([UserId])
            REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION
    );

    CREATE NONCLUSTERED INDEX [IX_NetworkQualityReports_CampusZone]
        ON [dbo].[NetworkQualityReports] ([CampusZone] ASC);

    CREATE NONCLUSTERED INDEX [IX_NetworkQualityReports_MeasuredAtUtc]
        ON [dbo].[NetworkQualityReports] ([MeasuredAtUtc] ASC);
END
GO

-- 3. Populate sample SLA deadlines and campus locations on existing service requests
UPDATE [dbo].[ServiceRequests]
SET 
    [CampusLocation] = CASE 
        WHEN [CampusLocation] IS NOT NULL THEN [CampusLocation]
        WHEN [TicketNumber] = 'PC-20261110-A1B2' THEN 'Hostel Block B - Room 314'
        WHEN [TicketNumber] = 'PC-20261112-C3D4' THEN 'Main Academic Block - Seminar Hall 2'
        ELSE 'Main Academic Block - Floor 2'
    END,
    [SlaDeadlineUtc] = CASE
        WHEN [SlaDeadlineUtc] IS NOT NULL THEN [SlaDeadlineUtc]
        WHEN [TicketNumber] = 'PC-20261110-A1B2' THEN DATEADD(hour, 24, [CreatedAtUtc])
        WHEN [TicketNumber] = 'PC-20261112-C3D4' THEN DATEADD(hour, 48, [CreatedAtUtc])
        ELSE DATEADD(hour, 48, [CreatedAtUtc])
    END
WHERE [CampusLocation] IS NULL OR [SlaDeadlineUtc] IS NULL;
GO
