# PRODUCTCARE — INT402 Course Syllabus Mapping
**Course:** INT402: Modern Web Programming Tools and Techniques  
**Academic Level:** Bachelor / Master of Technology (Computer Science & Engineering)  
**Project:** ProductCare — Product Warranty and Service Management Portal

---

## Comprehensive Course Outcome (CO) Mapping

| Course Outcome | Academic Domain | Realized in Project Files / Classes |
|---|---|---|
| **CO1** | .NET and C# Fundamentals | .NET 10 SDK, CLI commands, Project SDKs, `Directory.Build.props`, `Program.cs` |
| **CO2** | C# Programming and OOP | Domain models, Encapsulation, Interface contracts, Generics, Enums |
| **CO3** | ASP.NET Core MVC | Controllers, Razor Views, Model Binding, ViewModels, Data Annotations |
| **CO4** | Middleware, DI, and Security | Custom Middleware, IoC container, ASP.NET Core Identity, RBAC, Ownership checks |
| **CO5** | SQL Server Integration | EF Core 10, Relational Tables, Fluent API, SQL View, Stored Procedure |
| **CO6** | RESTful Web APIs | API Controllers, HTTP Verbs, DTOs, OpenAPI / Swagger, JWT Auth, Postman |

---

## Detailed Evidence and Implementation Manifest

### CO1 — .NET and C# Fundamentals
* **.NET 10 Runtime & SDK:** Solution targets `net10.0` using C# 13 language features (`required` properties, file-scoped namespaces, pattern matching, record types).
  * Evidence: `src/ProductCare.Web/ProductCare.Web.csproj`, `tests/ProductCare.Tests/ProductCare.Tests.csproj`.
* **.NET CLI Tooling:** Fully manageable and verifiable via `dotnet build`, `dotnet test`, `dotnet ef`, and `dotnet run`.
  * Evidence: Local tool manifest `dotnet-tools.json` registering `dotnet-ef` 10.0.15.
* **Modern Build Configuration:**
  * Evidence: `Directory.Build.props` setting Roslyn compiler flags (`UseSharedCompilation=false`).

---

### CO2 — C# Programming and Object-Oriented Principles
* **Classes and Objects:**
  * Domain entities encapsulating state and behavior: `Product.cs`, `ServiceRequest.cs`, `ApplicationUser.cs`, `ServiceRequestHistory.cs`.
* **Inheritance:**
  * `ApplicationUser` inherits from `IdentityUser`.
  * `ApplicationDbContext` inherits from `IdentityDbContext<ApplicationUser>`.
* **Interfaces and Polymorphism:**
  * Loose coupling via abstractions:
    * `IWarrantyCalculatorService` &rarr; `WarrantyCalculatorService.cs`
    * `IServiceRequestWorkflowService` &rarr; `ServiceRequestWorkflowService.cs`
    * `IInvoiceStorageService` &rarr; `InvoiceStorageService.cs`
    * `IDashboardReportingService` &rarr; `DashboardReportingService.cs`
    * `IJwtTokenService` &rarr; `JwtTokenService.cs`
    * `IDbInitializerService` &rarr; `DbInitializerService.cs`
* **Strongly-Typed Enums & Records:**
  * `ProductCategory.cs`, `WarrantyStatus.cs`, `RequestCategory.cs`, `PriorityLevel.cs`, `RequestStatus.cs`.
  * C# Records: `WarrantyDetails`, `WorkflowResult`, `InvoiceUploadResult`.
* **Generics:**
  * Standardized generic wrapper: `ApiResponse<T>` in `src/ProductCare.Web/ViewModels/Common/ApiResponse.cs`.

---

### CO3 — ASP.NET Core MVC Architecture
* **Controllers:**
  * Account Controller: `src/ProductCare.Web/Controllers/AccountController.cs`
  * Customer Area Controllers: `DashboardController.cs`, `ProductsController.cs`, `WarrantyController.cs`, `ServiceRequestsController.cs`.
  * Administrator Area Controllers: `DashboardController.cs`, `ServiceRequestsController.cs`, `CustomersController.cs`, `ProductsController.cs`, `ReportsController.cs`.
* **Razor Views & Layout Hierarchy:**
  * Master Layout: `src/ProductCare.Web/Views/Shared/_Layout.cshtml`
  * Area ViewStarts & Imports: `_ViewStart.cshtml`, `_ViewImports.cshtml`.
  * Views for registration, login, dashboards, product catalog, edit forms, details, warranty timelines, and ticket queues.
* **Model Binding & ViewModels:**
  * Form inputs bound strictly to ViewModels rather than database entities:
    * `RegisterViewModel.cs`, `LoginViewModel.cs`
    * `ProductCreateEditViewModel.cs`, `ProductDetailsViewModel.cs`, `ProductListViewModel.cs`
    * `ServiceRequestCreateViewModel.cs`, `ServiceRequestDetailsViewModel.cs`
    * `AdminDashboardViewModel.cs`, `AdminReportsViewModel.cs`
* **Server-Side & Client-Side Validation:**
  * DataAnnotations: `[Required]`, `[StringLength]`, `[EmailAddress]`, `[Compare]`, `[Range]`, `[DataType]`.
  * Cross-field validation (e.g., preventing future purchase dates, serial duplication).
* **Data Passing Mechanisms:**
  * Strongly-typed models (`@model`)
  * `TempData` for post-redirect-get flash alerts (`TempData["SuccessMessage"]`, `TempData["ErrorMessage"]`)
  * Route parameters (`asp-route-id`, `asp-route-productId`).

---

### CO4 — Middleware, Dependency Injection, and Security
* **Custom Middleware Pipeline:**
  * `CorrelationIdMiddleware.cs`: Implements request tracking with `X-Correlation-ID` header and logger scoping.
  * `SecurityHeadersMiddleware.cs`: Appends defense-in-depth headers (`X-Content-Type-Options: nosniff`, `X-Frame-Options: SAMEORIGIN`, `X-XSS-Protection: 1; mode=block`).
* **Dependency Injection (DI):**
  * Configured in `Program.cs` registering services with appropriate lifecycles:
    * `AddSingleton`: `IWarrantyCalculatorService`, `IJwtTokenService`.
    * `AddScoped`: `IServiceRequestWorkflowService`, `IInvoiceStorageService`, `IDashboardReportingService`, `IDbInitializerService`.
* **Authentication & Authorization (Security):**
  * ASP.NET Core Identity authentication with secure cookie configuration.
  * Role-Based Access Control (`[Authorize(Roles = "Admin")]`, `[Authorize(Roles = "Customer")]`).
  * Server-side ownership verification preventing Horizontal Privilege Escalation (IDOR attacks).
  * Anti-forgery tokens (`[ValidateAntiForgeryToken]`).
  * Secure physical invoice storage outside `wwwroot` with magic byte file signature verification.

---

### CO5 — SQL Server Integration and Database Engineering
* **Entity Framework Core 10:**
  * Code First migrations with full snapshot: `Data/Migrations/20261116110739_InitialCreate.cs`.
  * Design-time factory: `Data/ApplicationDbContextFactory.cs`.
* **Relational Schema Design:**
  * Primary keys, foreign keys with explicit delete behaviors (`Cascade`, `Restrict`).
  * Unique indexes (`UX_ServiceRequests_TicketNumber`, `UX_Products_UserId_SerialNumber`).
  * Lookup indexes on `UserId`, `WarrantyExpiryDate`, `Status`.
* **SQL Server Database View:**
  * `scripts/01_Create_Views.sql` creating `dbo.vw_ProductWarrantySummary`.
  * Mapped in C# as keyless entity `ProductWarrantySummaryView`.
* **SQL Server Stored Procedure:**
  * `scripts/02_Create_Stored_Procedures.sql` creating `dbo.sp_GetServiceTicketStatistics`.
  * Mapped in C# via `FromSqlInterpolated` to `ServiceTicketStatisticsDto`.

---

### CO6 — RESTful Web APIs and API Integration
* **REST API Controllers:**
  * `src/ProductCare.Web/Controllers/Api/AuthController.cs`: `POST /api/auth/login`
  * `src/ProductCare.Web/Controllers/Api/ProductsApiController.cs`: Full CRUD endpoints (`GET`, `POST`, `PUT`, `DELETE`).
  * `src/ProductCare.Web/Controllers/Api/ServiceRequestsApiController.cs`: Ticket lifecycle and cancellation endpoints.
* **HTTP Verbs and Status Codes:**
  * `200 OK`, `201 Created` (with `CreatedAtAction`), `400 BadRequest`, `401 Unauthorized`, `403 Forbidden`, `404 NotFound`.
* **API Documentation:**
  * OpenAPI 3.0 / Swagger UI integration in `Program.cs` (`/swagger`).
* **API Testing Tooling:**
  * Full Postman test collection: `postman/ProductCare.postman_collection.json`.
  * Automated token extraction in test script saving `{{authToken}}` variable.
