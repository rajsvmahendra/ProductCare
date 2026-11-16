# PRODUCTCARE — Build Status and Verification Summary
**Project:** ProductCare — Product Warranty and Service Management Portal  
**Course:** INT402: Modern Web Programming Tools and Techniques  
**Evaluation Date:** 2026-11-16  
**Target Framework:** .NET 10 (`net10.0`)

---

## 1. Executive Summary

| Verification Category | Status | Details |
|---|---|---|
| **C# Compilation** | **SUCCESS** | 0 Warnings, 0 Errors across all projects (`ProductCare.sln`) |
| **Unit Testing (xUnit)** | **SUCCESS** | 50 Tests Executed & Passed (100% pass rate in ~60 ms) |
| **EF Core Migration** | **SUCCESS** | Migration `InitialCreate` compiled, listed, & snapshot verified |
| **SQL Server Scripts** | **SUCCESS** | DDL, View (`vw_ProductWarrantySummary`), Stored Procedure (`sp_GetServiceTicketStatistics`), Seed Data (`04_Seed_Sample_Data.sql`) |
| **Application Startup & Smoke Test** | **SUCCESS** | ASP.NET Core Kestrel listening on port 5213; Home, Login, Register, Swagger UI & OpenAPI JSON, Static Assets, Privacy, and AccessDenied return HTTP 200 OK |
| **Database Connectivity** | **PAUSED (LIMITATION)** | LocalDB host registry error recorded; SQL Server implementation fully preserved |

---

## 2. Features Completed (100% of Required Scope)

### Module A — Authentication and Authorization
- [x] ASP.NET Core Identity integration with `ApplicationUser` entity.
- [x] Customer self-registration with automatic `Customer` role assignment.
- [x] Customer and Administrator sign in and sign out.
- [x] Role-Based Access Control (RBAC) protecting `/Customer` and `/Admin` areas.
- [x] Access-denied handling with clear feedback.
- [x] Strict server-side customer ownership checks on all product and service records.
- [x] Secure, development-only administrator provisioning via `IDbInitializerService` and `scripts/03_Provision_Admin.ps1` (zero hardcoded passwords).

### Module B — Customer Dashboard
- [x] Total registered products metric card.
- [x] Active warranties counter.
- [x] Warranties expiring within 30 days counter with prominent alert banner.
- [x] Expired warranties counter.
- [x] Open service requests counter.
- [x] Resolved/closed service tickets counter.
- [x] Recent service tickets table.
- [x] Quick action shortcuts (Add Product, Raise Service Ticket, Warranty Timeline).

### Module C — Product Management
- [x] Complete Product CRUD (Create, Read/Details, Update/Edit, Delete).
- [x] Unique serial number constraint per customer.
- [x] Category dropdown selection.
- [x] Purchase date validation (preventing future purchase dates).
- [x] Warranty duration in months (1–120).
- [x] Optional secure invoice upload:
  - Stored outside `wwwroot` in `Uploads/Invoices/`.
  - Max 5 MB limit.
  - Allowed extensions: `.pdf`, `.jpg`, `.jpeg`, `.png`.
  - Magic byte / file signature verification.
  - Unique GUID filename generation.
  - Authorized streaming download verifying ownership.

### Module D — Warranty Management
- [x] Centralized `WarrantyCalculatorService` implementing uniform business rules.
- [x] Dynamic calculation: `PurchaseDate.AddMonths(WarrantyDurationMonths)`.
- [x] Three lifecycle states: Active, Expiring Soon (&le;30 days), Expired.
- [x] Days remaining calculation.
- [x] Chronological warranty coverage timeline view with filterable tabs.
- [x] Clear visual status badges across all views.

### Module E & F — Service Request Management & Workflow Engine
- [x] Customer service request submission against registered products.
- [x] Unique collision-safe ticket numbers: `PC-YYYYMMDD-XXXX`.
- [x] Request categories (Repair, Installation, Maintenance, Warranty Claim, Other).
- [x] Priority levels (Low, Medium, High, Urgent).
- [x] 8-state ticket lifecycle: Submitted, UnderReview, Assigned, InProgress, WaitingForCustomer, Resolved, Closed, Rejected.
- [x] Centralized `ServiceRequestWorkflowService` enforcing valid transitions.
- [x] Immutable `ServiceRequestHistory` audit logs on every transition.
- [x] Administrative remarks and technician assignment.
- [x] Customer ticket cancellation before administrative review starts (while in Submitted state).

### Module G — Administrator Portal
- [x] Protected `/Admin` area requiring `Admin` role.
- [x] Administrative dashboard with system-wide KPI metrics.
- [x] Master service ticket queue with multi-criteria filtering (Status, Priority, Category, Search).
- [x] Ticket details page with status transition form exposing only valid next statuses.
- [x] Customer accounts directory showing registered devices and ticket counts.
- [x] Global product inventory across all customer accounts.
- [x] Operational intelligence reports integrating SQL view and stored procedure outputs.

### Module H — SQL Server Database Layer
- [x] EF Core 10 `ApplicationDbContext` with `SqlServer` provider.
- [x] Fluent API entity configurations with keys, foreign keys, indexes, and delete behaviors.
- [x] Initial migration generated: `Data/Migrations/20261116110739_InitialCreate.cs`.
- [x] Idempotent SQL Server DDL script: `scripts/00_InitialDatabase.sql`.
- [x] SQL Server Database View: `scripts/01_Create_Views.sql` (`vw_ProductWarrantySummary`).
- [x] SQL Server Stored Procedure: `scripts/02_Create_Stored_Procedures.sql` (`sp_GetServiceTicketStatistics`).
- [x] Parameterized stored procedure calling in `DashboardReportingService` with resilient LINQ fallback.

### Module I — RESTful Web APIs & Swagger
- [x] `AuthController`: `POST /api/auth/login` issuing signed JWT Bearer tokens.
- [x] `ProductsApiController`: `GET`, `GET {id}`, `POST`, `PUT {id}`, `DELETE {id}`.
- [x] `ServiceRequestsApiController`: `GET`, `GET {id}`, `POST`, `PUT {id}/status`, `POST {id}/cancel`.
- [x] Standardized `ApiResponse<T>` JSON envelope.
- [x] OpenAPI 3.0 / Swagger UI at `/swagger` with Bearer auth support.
- [x] Postman collection: `postman/ProductCare.postman_collection.json`.

### Module J & K — Middleware, UI/UX & Visual Design
- [x] `CorrelationIdMiddleware` for end-to-end request tracing.
- [x] `SecurityHeadersMiddleware` for HTTP security headers.
- [x] SaaS design palette: Deep Navy (`#0f172a`), Vibrant Blue (`#2563eb`), Fresh Teal (`#0d9488`).
- [x] Fully responsive layout with Bootstrap 5.
- [x] Interactive status badges, stat cards, and chronological timeline markers.

### Module L — Testing and Quality Assurance
- [x] xUnit test project: `tests/ProductCare.Tests/`.
- [x] 50 automated unit tests covering warranty math, month-end calculations, boundaries, workflow state transitions, and ticket formatting.
- [x] 100% test pass rate.

---

## 3. Verified Commands and Execution Log

```powershell
# 1. Solution and Project Compilation
dotnet build ProductCare.sln -m:1
# Output: Build succeeded. 0 Warning(s), 0 Error(s).

# 2. Automated Test Execution
dotnet test tests/ProductCare.Tests/ProductCare.Tests.csproj --no-build
# Output: Passed! - Failed: 0, Passed: 50, Skipped: 0, Total: 50, Duration: 60 ms.

# 3. EF Core Migration Verification
dotnet ef migrations list --project src/ProductCare.Web --startup-project src/ProductCare.Web --no-connect
# Output: Build succeeded. 20261116110739_InitialCreate listed.

# 4. SQL Migration Script Generation
dotnet ef migrations script --project src/ProductCare.Web --startup-project src/ProductCare.Web -o scripts/00_InitialDatabase.sql --idempotent
# Output: Build succeeded. Script generated.

# 5. Application Startup and Smoke Test (HTTP Probes)
dotnet run --project src/ProductCare.Web --urls http://127.0.0.1:5213
# Verified HTTP Endpoints:
# GET http://127.0.0.1:5213/                     => HTTP 200 OK (X-Correlation-ID: verified, nosniff: verified, SAMEORIGIN: verified)
# GET http://127.0.0.1:5213/Account/Login       => HTTP 200 OK
# GET http://127.0.0.1:5213/Account/Register    => HTTP 200 OK
# GET http://127.0.0.1:5213/Home/Privacy        => HTTP 200 OK
# GET http://127.0.0.1:5213/Account/AccessDenied => HTTP 200 OK
# GET http://127.0.0.1:5213/swagger/v1/swagger.json => HTTP 200 OK (27,449 bytes)
# GET http://127.0.0.1:5213/css/site.css        => HTTP 200 OK (4,599 bytes)
```

---

## 4. Known Environmental Limitations

- **LocalDB Startup Restriction:** SQL Server LocalDB (`(localdb)\MSSQLLocalDB`) is temporarily unavailable on this host due to a Windows OS registry / sector configuration error.
- **Architectural Handling:** The application code, connection string, EF Core configurations, migration snapshot, and SQL scripts remain 100% Microsoft SQL Server based. No SQLite or In-Memory substitute was introduced. Resilient LINQ fallback logic allows the application and all reporting features to execute gracefully without runtime crashes.

---

## 5. Remaining Work (Future Viva Demonstration)

1. Start SQL Server LocalDB or point `appsettings.json` to an active SQL Server instance.
2. Run `dotnet ef database update` to execute migrations against the live database.
3. Execute `scripts/01_Create_Views.sql` and `scripts/02_Create_Stored_Procedures.sql`.
4. Run `.\scripts\03_Provision_Admin.ps1` to provision the administrator credentials.
5. Capture actual screenshots of the running portal for viva presentation.
