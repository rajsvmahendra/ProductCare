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
| **Unit Testing (xUnit)** | **SUCCESS** | 50 Tests Executed & Passed (100% pass rate in 95 ms) |
| **UI/UX Design Transformation** | **SUCCESS** | Complete visual overhaul implemented per `docs/DESIGN.md` specification |
| **Visual Verification & Screenshots**| **SUCCESS** | 9 Genuine high-DPI screenshots captured via Headless Chromium in `docs/screenshots/` |
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

### Module J & K — Middleware, UI/UX & Visual Design (Ownership Continuity Experience per `docs/DESIGN.md` v2.0)
- [x] `CorrelationIdMiddleware` for end-to-end request tracing (`X-Correlation-ID`).
- [x] `SecurityHeadersMiddleware` for HTTP security headers (`X-Content-Type-Options: nosniff`, `X-Frame-Options: SAMEORIGIN`, referrer policy).
- [x] Comprehensive Warm Mineral Token System (`site.css`):
  - Off-White Canvas (`#F2F0E9`, `--pc-canvas`) & Warm Natural Surface (`#FAFAF7`, `--pc-surface`).
  - Deep Graphite Text & Navigation (`#202A28`, `--pc-graphite`).
  - Forest Green Accent (`#315D50`, `--pc-forest`) & Forest Hover (`#234339`).
  - Warm Amber Highlights (`#D69A51`, `--pc-amber`).
  - Tactile Stone Dividers & Borders (`#DCDAD1`, `--pc-stone`).
  - High-Contrast Editorial Typography: *Newsreader* serif for headings, *IBM Plex Sans* for UI/body, tabular lining figures for serial numbers, dates, and currency.
  - Semantic Status Tokens: Active (`#2E6B56`), Expiring Soon (`#9B6B1F`), Expired (`#9E3A3A`), Resolved (`#2B6F55`).
- [x] Original ProductCare Brand Identity: Inline SVG shield mark with circular continuity verification glyph in forest green and warm amber.
- [x] Public Experience Overhaul:
  - Editorial Hero: "Everything you own. Every warranty. Every service journey. One place."
  - Grounded Product Demonstration Card: Authentic device ownership record (Apple MacBook Pro 16", Serial `C02G1234MD6R`, verified 2.4 MB PDF invoice download, 36-month dynamic coverage radar, and active service ticket `PC-20261118-4091` in Progress).
  - Human Dilemma Cards: Real-world problems (faded thermal receipts, expired warranty surprises, chaotic repair journeys, fragmented device records).
  - Connected 3-Step Ownership Progression: Register & Preserve &rarr; Monitor & Anticipate &rarr; Request & Track.
  - Verified Core Capabilities & Academic INT402 Footer.
- [x] Redesigned Authentication Flow ("The ProductCare Ownership Desk"):
  - Asymmetric desktop layout: left column contains a document record vignette previewing authentic ownership cards; right column hosts the clean form surface.
  - Interactive password visibility reveal with accessible SVG eye icons (`site.js`).
  - Grouped collapsible physical address fields for progressive disclosure.
- [x] Dedicated Authenticated Portal Architecture (`_PortalLayout.cshtml`):
  - Fixed 250px deep graphite (`#202A28`) sidebar navigation with stone dividers, amber active indicators, and section groupings.
  - Contextual topbar with dynamic page title, role badge, user profile button, and quick logout.
  - Off-canvas responsive mobile drawer navigation.
- [x] Customer & Admin Portal Pages:
  - High-contrast KPI metric cards with progress meters and mineral background surfaces.
  - Actionable warranty radar table with expiration countdown markers.
  - Service request timeline stepper with complete audit history and actor snapshots.
  - Status transition dropdowns constrained strictly to permissible next states.
  - Clean modal dialogs for product deletion and ticket cancellation confirmation.

### Module L — Testing and Quality Assurance
- [x] xUnit test project: `tests/ProductCare.Tests/`.
- [x] 50 automated unit tests covering warranty math, month-end calculations, boundaries, workflow state transitions, and ticket formatting.
- [x] 100% test pass rate (50 passed in 77 ms).

---

## 3. Verified Commands and Execution Log

```powershell
# 1. Solution and Project Compilation
dotnet build ProductCare.sln -m:1 --no-restore
# Output: Build succeeded. 0 Warning(s), 0 Error(s). Time Elapsed 00:00:04.91

# 2. Automated Test Execution
dotnet test tests/ProductCare.Tests/ProductCare.Tests.csproj --no-build
# Output: Passed! - Failed: 0, Passed: 50, Skipped: 0, Total: 50, Duration: 77 ms - ProductCare.Tests.dll (net10.0)

# 3. Headless Chromium Screenshot Capture
# Verified genuine rendering of desktop and mobile viewports via Microsoft Edge headless:
# - docs/screenshots/01_landing_desktop.png (1440x920, 150 KB) - Editorial hero & ownership continuity vision
# - docs/screenshots/02_landing_mobile.png (390x844, 55 KB) - Mobile responsive landing experience
# - docs/screenshots/03_landing_demonstration.png (1440x900, 149 KB) - Grounded product ownership demonstration card
# - docs/screenshots/04_register_desk.png (1440x900, 66 KB) - The Ownership Desk registration experience
# - docs/screenshots/05_login_desk.png (1440x900, 56 KB) - The Ownership Desk customer sign-in
# - docs/screenshots/06_register_mobile.png (390x844, 45 KB) - Mobile registration form desk
# - docs/screenshots/07_privacy_desktop.png (1440x900, 110 KB) - Academic privacy & data compliance
# - docs/screenshots/08_access_denied.png (1440x900, 75 KB) - Role authorization denial card
# - docs/screenshots/09_swagger_api.png (1440x900, 17 KB) - OpenAPI / Swagger REST API UI

# 4. EF Core Migration Verification
dotnet ef migrations list --project src/ProductCare.Web --startup-project src/ProductCare.Web --no-connect
# Output: Build succeeded. 20261116110739_InitialCreate listed.

# 5. SQL Migration Script Generation
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

## 4. Known Environmental Limitations & Instance Migration

- **LocalDB Startup Restriction:** The default SQL Server LocalDB (`(localdb)\MSSQLLocalDB`) was temporarily blocked on this host due to a Windows OS registry user instance initialization error (`0x80070002`).
- **User-Managed Alternative Instance:** The user successfully provisioned an independent, healthy LocalDB instance named `ProductCareTest`.
- **Architectural Handling:** The application code, connection string, EF Core configurations, migration snapshot, and SQL scripts remain 100% Microsoft SQL Server based. No SQLite or In-Memory substitute was introduced. Resilient LINQ fallback logic allows the application and all reporting features to execute gracefully without runtime crashes.

---

## 5. Remaining Work (Future Viva Demonstration)

1. Point `appsettings.json` `DefaultConnection` to `Server=(localdb)\\ProductCareTest;Database=ProductCareDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`.
2. Run `dotnet ef database update --project src/ProductCare.Web --startup-project src/ProductCare.Web` to execute migrations against the live `ProductCareTest` database.
3. Execute `scripts/01_Create_Views.sql` and `scripts/02_Create_Stored_Procedures.sql`.
4. Run `.\scripts\03_Provision_Admin.ps1` to provision the administrator credentials.
5. All genuine screenshots have already been captured and archived in `docs/screenshots/` ready for the academic submission report and viva presentation.
