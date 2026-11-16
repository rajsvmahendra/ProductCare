# PRODUCTCARE — Database Design & Schema Documentation
**Course:** INT402: Modern Web Programming Tools and Techniques  
**Database System:** Microsoft SQL Server (LocalDB / Azure SQL / Dedicated SQL Server)  
**ORM:** Entity Framework Core 10 (Code First)

---

## 1. Relational Database Overview

ProductCare uses Microsoft SQL Server as its persistent relational data store. The schema is configured using EF Core Fluent API configurations (`IEntityTypeConfiguration<T>`) to guarantee explicit table definitions, data types, indexes, and constraints.

---

## 2. Table Specifications

### 2.1 AspNetUsers (ApplicationUser)
Extends ASP.NET Core Identity's standard user store.

| Column Name | SQL Server Data Type | Nullable | Description |
|---|---|---|---|
| `Id` | `NVARCHAR(450)` | No | Primary Key (GUID string) |
| `FullName` | `NVARCHAR(100)` | No | Customer or Administrator full name |
| `Address` | `NVARCHAR(250)` | Yes | Optional street address |
| `City` | `NVARCHAR(100)` | Yes | Optional city of residence |
| `PostalCode` | `NVARCHAR(20)` | Yes | Optional postal code |
| `IsActive` | `BIT` | No | Account status flag (1=active, 0=deactivated) |
| `CreatedAtUtc` | `DATETIME2` | No | Timestamp of account registration in UTC |
| `UpdatedAtUtc` | `DATETIME2` | Yes | Timestamp of last account modification in UTC |
| `UserName` | `NVARCHAR(256)` | Yes | Login username (email) |
| `NormalizedUserName` | `NVARCHAR(256)` | Yes | Uppercase normalized username |
| `Email` | `NVARCHAR(256)` | Yes | User email address |
| `NormalizedEmail` | `NVARCHAR(256)` | Yes | Normalized email index |
| `PasswordHash` | `NVARCHAR(MAX)` | Yes | PBKDF2 with SHA-256 hashed password |
| `SecurityStamp` | `NVARCHAR(MAX)` | Yes | Identity security stamp |
| `ConcurrencyStamp` | `NVARCHAR(MAX)` | Yes | Optimistic concurrency token |

---

### 2.2 Products
Stores customer registered appliances, gadgets, computers, and electronics.

| Column Name | SQL Server Data Type | Nullable | Description |
|---|---|---|---|
| `Id` | `INT` | No | Primary Key, `IDENTITY(1,1)` |
| `UserId` | `NVARCHAR(450)` | No | Foreign Key to `AspNetUsers.Id` |
| `Name` | `NVARCHAR(150)` | No | Name or model description of product |
| `Brand` | `NVARCHAR(100)` | No | Manufacturer or brand name |
| `ModelNumber` | `NVARCHAR(100)` | No | Product model number |
| `SerialNumber` | `NVARCHAR(100)` | No | Hardware serial number |
| `Category` | `INT` | No | Product category enum identifier (1 to 8, 99) |
| `PurchaseDate` | `DATE` | No | Purchase date |
| `WarrantyDurationMonths`| `INT` | No | Warranty duration in months (1–120) |
| `WarrantyExpiryDate` | `DATE` | No | Calculated expiry date (`PurchaseDate + Months`) |
| `PurchaseStore` | `NVARCHAR(150)` | Yes | Retailer or vendor |
| `PurchasePrice` | `DECIMAL(18,2)` | Yes | Purchase price |
| `InvoiceFileName` | `NVARCHAR(255)` | Yes | Secure unique disk filename outside wwwroot |
| `InvoiceOriginalFileName`| `NVARCHAR(255)`| Yes | Original client filename |
| `InvoiceContentType` | `NVARCHAR(100)` | Yes | MIME type (e.g. application/pdf) |
| `InvoiceFileSize` | `BIGINT` | Yes | File size in bytes |
| `Notes` | `NVARCHAR(1000)` | Yes | Customer notes or warranty terms |
| `CreatedAtUtc` | `DATETIME2` | No | Creation timestamp (UTC) |
| `UpdatedAtUtc` | `DATETIME2` | Yes | Last update timestamp (UTC) |

**Constraints & Indexes:**
- Primary Key: `PK_Products` on `Id`
- Foreign Key: `FK_Products_AspNetUsers_UserId` &rarr; Cascade on delete
- Index: `IX_Products_UserId` on `UserId`
- Index: `IX_Products_WarrantyExpiryDate` on `WarrantyExpiryDate` (optimizes expiry queries)
- Unique Index: `UX_Products_UserId_SerialNumber` on `(UserId, SerialNumber)` (prevents duplicate registrations of the same serial number by the same customer)

---

### 2.3 ServiceRequests
Stores repair, installation, maintenance, and warranty claim tickets.

| Column Name | SQL Server Data Type | Nullable | Description |
|---|---|---|---|
| `Id` | `INT` | No | Primary Key, `IDENTITY(1,1)` |
| `TicketNumber` | `NVARCHAR(30)` | No | Unique human-readable ticket (e.g. `PC-20261116-A7B2`) |
| `ProductId` | `INT` | No | Foreign Key to `Products.Id` |
| `UserId` | `NVARCHAR(450)` | No | Foreign Key to `AspNetUsers.Id` |
| `Category` | `INT` | No | Request category (Repair, Installation, Maintenance, Claim, Other) |
| `Subject` | `NVARCHAR(200)` | No | Issue headline |
| `Description` | `NVARCHAR(2000)`| No | Detailed issue description |
| `Priority` | `INT` | No | Priority level (1=Low, 2=Medium, 3=High, 4=Urgent) |
| `Status` | `INT` | No | Workflow status (1 to 8) |
| `AssignedTechnician` | `NVARCHAR(100)` | Yes | Assigned technician name |
| `AdminRemarks` | `NVARCHAR(2000)`| Yes | Administrative notes and diagnosis |
| `CreatedAtUtc` | `DATETIME2` | No | Submission timestamp (UTC) |
| `UpdatedAtUtc` | `DATETIME2` | Yes | Last status change timestamp (UTC) |
| `ResolvedAtUtc` | `DATETIME2` | Yes | Resolution timestamp (UTC) |

**Constraints & Indexes:**
- Primary Key: `PK_ServiceRequests` on `Id`
- Unique Index: `UX_ServiceRequests_TicketNumber` on `TicketNumber`
- Foreign Key: `FK_ServiceRequests_Products_ProductId` &rarr; Restrict on delete (prevents deleting a product that has active service history)
- Foreign Key: `FK_ServiceRequests_AspNetUsers_UserId` &rarr; Restrict on delete
- Index: `IX_ServiceRequests_Status` on `Status`
- Index: `IX_ServiceRequests_CreatedAtUtc` on `CreatedAtUtc`

---

### 2.4 ServiceRequestHistories
Immutable audit trail recording every status change on a ticket.

| Column Name | SQL Server Data Type | Nullable | Description |
|---|---|---|---|
| `Id` | `INT` | No | Primary Key, `IDENTITY(1,1)` |
| `ServiceRequestId` | `INT` | No | Foreign Key to `ServiceRequests.Id` |
| `FromStatus` | `INT` | No | Pre-transition status |
| `ToStatus` | `INT` | No | Post-transition status |
| `ChangedByUserId` | `NVARCHAR(450)` | No | User ID who executed the transition |
| `ChangedByUserName` | `NVARCHAR(100)` | No | User display name or email snapshot |
| `Remarks` | `NVARCHAR(1000)` | Yes | Optional transition remarks |
| `CreatedAtUtc` | `DATETIME2` | No | Transition timestamp in UTC |

**Constraints & Indexes:**
- Primary Key: `PK_ServiceRequestHistories` on `Id`
- Foreign Key: `FK_ServiceRequestHistories_ServiceRequests_ServiceRequestId` &rarr; Cascade on ticket delete
- Foreign Key: `FK_ServiceRequestHistories_AspNetUsers_ChangedByUserId` &rarr; Restrict on delete
- Index: `IX_ServiceRequestHistories_ServiceRequestId`
- Index: `IX_ServiceRequestHistories_CreatedAtUtc`

---

## 3. SQL Server Database Objects

### 3.1 SQL View: `dbo.vw_ProductWarrantySummary`
- **File:** `scripts/01_Create_Views.sql`
- **Purpose:** Joins `Products` with `AspNetUsers` and dynamically computes `DaysRemaining` and `WarrantyStatusText` (`Active`, `Expiring Soon`, `Expired`) using SQL Server date functions (`DATEDIFF`, `GETUTCDATE()`).
- **Mapped in C#:** Keyless entity `ProductWarrantySummaryView` queried via `_context.ProductWarrantySummaries`.

```sql
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
```

---

### 3.2 Stored Procedure: `dbo.sp_GetServiceTicketStatistics`
- **File:** `scripts/02_Create_Stored_Procedures.sql`
- **Purpose:** Aggregates ticket volume by category and status counters with optional `@StartDate` and `@EndDate` parameters.
- **Mapped in C#:** Keyless entity `ServiceTicketStatisticsDto` queried via `_context.ServiceTicketStatistics.FromSqlInterpolated($"EXEC dbo.sp_GetServiceTicketStatistics {startDate}, {endDate}")`.

```sql
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
```

---

## 4. Migration & Deployment Commands

The project includes pre-compiled EF Core migrations in `src/ProductCare.Web/Data/Migrations/`.

To apply migrations against a live SQL Server instance:
```powershell
dotnet ef database update --project src/ProductCare.Web --startup-project src/ProductCare.Web
```

Or apply the complete idempotent SQL script directly via SQL Server Management Studio (SSMS) or `sqlcmd`:
```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -d ProductCareDb -i scripts/00_InitialDatabase.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -d ProductCareDb -i scripts/01_Create_Views.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -d ProductCareDb -i scripts/02_Create_Stored_Procedures.sql
```
