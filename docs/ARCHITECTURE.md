# PRODUCTCARE — Solution Architecture Document
**Course:** INT402: Modern Web Programming Tools and Techniques  
**System Name:** ProductCare — Product Warranty and Service Management Portal  
**Target Runtime:** .NET 10 (ASP.NET Core 10, C# 13, EF Core 10)

---

## 1. Architectural Philosophy and Style

ProductCare is designed following the **Modular Monolith** architectural style. It avoids the premature network latency, serialization overhead, and deployment complexity of microservices, while enforcing strict separation of concerns, domain model isolation, and maintainability.

```
+---------------------------------------------------------------------------------+
|                               Presentation Layer                                |
|  - Razor Views (Customer Area, Admin Area, Public Views)                        |
|  - ASP.NET Core REST API Controllers (Swagger / OpenAPI v1)                     |
+---------------------------------------------------------------------------------+
                                      |
                                      v
+---------------------------------------------------------------------------------+
|                       Cross-Cutting Middleware Pipeline                         |
|  - CorrelationIdMiddleware (X-Correlation-ID tracing & logging scope)           |
|  - SecurityHeadersMiddleware (nosniff, SAMEORIGIN, XSS-Protection)              |
|  - Cookie & JWT Bearer Hybrid Authentication Handler                            |
+---------------------------------------------------------------------------------+
                                      |
                                      v
+---------------------------------------------------------------------------------+
|                              Core Business Layer                                |
|  - WarrantyCalculatorService (Dynamic expiration & days calculation)            |
|  - ServiceRequestWorkflowService (State machine transition engine)              |
|  - InvoiceStorageService (Physical storage outside wwwroot + signature check)  |
|  - DashboardReportingService (Operational intelligence & KPI calculation)       |
|  - JwtTokenService (HMAC-SHA256 bearer token issuance)                          |
+---------------------------------------------------------------------------------+
                                      |
                                      v
+---------------------------------------------------------------------------------+
|                           Data Access Layer (EF Core 10)                        |
|  - ApplicationDbContext : IdentityDbContext<ApplicationUser>                    |
|  - Fluent API Configurations (ProductConfiguration, ServiceRequestConfiguration)|
|  - SQL Server Views (vw_ProductWarrantySummary)                                 |
|  - SQL Server Stored Procedures (sp_GetServiceTicketStatistics)                 |
+---------------------------------------------------------------------------------+
                                      |
                                      v
+---------------------------------------------------------------------------------+
|                             Microsoft SQL Server                                |
|  - Relational Database Tables, Foreign Keys, Unique Indexes, Constraints        |
+---------------------------------------------------------------------------------+
```

---

## 2. Solution Structure and Physical Organization

The physical file layout separates presentation, business services, data access, tests, and documentation:

```text
ProductCare_INT402/
├── ProductCare.sln                          # Master Visual Studio Solution
├── Directory.Build.props                    # Build optimization settings
├── src/
│   └── ProductCare.Web/
│       ├── Areas/
│       │   ├── Admin/                       # Administrative Subsystem (Role = Admin)
│       │   │   ├── Controllers/             # Dashboard, ServiceRequests, Customers, Products, Reports
│       │   │   └── Views/                   # Administrative Razor Views
│       │   └── Customer/                    # Customer Subsystem (Role = Customer)
│       │       ├── Controllers/             # Dashboard, Products, Warranty, ServiceRequests
│       │       └── Views/                   # Customer Razor Views
│       ├── Controllers/
│       │   ├── AccountController.cs         # Registration, Login, Logout, AccessDenied
│       │   ├── HomeController.cs            # Landing page, Privacy, Error
│       │   └── Api/                         # REST API Controllers
│       │       ├── AuthController.cs        # JWT Login endpoint
│       │       ├── ProductsApiController.cs # CRUD API for products
│       │       └── ServiceRequestsApiController.cs # Ticket workflow API
│       ├── Data/
│       ├── Models/
│       ├── ViewModels/
│       ├── Services/
│       ├── Middleware/
│       └── wwwroot/
├── tests/
│   └── ProductCare.Tests/                   # xUnit Unit Testing Project
├── docs/                                    # Academic & Technical Documentation
├── scripts/                                 # SQL Scripts & Provisioning Utilities
└── postman/                                 # Postman API Collection
```

---

## 3. Data Flow and Request Lifecycle

### 3.1 MVC Web Request Flow
1. **Inbound HTTP Request:** Enters Kestrel web server.
2. **Middleware Pipeline:**
   - `CorrelationIdMiddleware` reads or generates an `X-Correlation-ID`, sets `HttpContext.TraceIdentifier`, and enriches the logger scope.
   - `SecurityHeadersMiddleware` appends security headers.
   - Routing matches the URL to an Area, Controller, and Action.
   - Authentication decrypts the application cookie.
   - Authorization verifies role policies (`[Authorize(Roles = "Customer")]` or `[Authorize(Roles = "Admin")]`).
3. **Controller Execution:**
   - Controller extracts the authenticated user ID (`User.FindFirstValue(ClaimTypes.NameIdentifier)`).
   - Server-side ownership validation ensures customers cannot query or mutate records belonging to other users.
   - Controller calls domain services (`WarrantyCalculatorService`, `ServiceRequestWorkflowService`).
4. **Service Execution & Data Access:**
   - Queries `ApplicationDbContext` asynchronously using compiled EF Core expressions.
   - Maps domain entities to strongly typed ViewModels.
5. **View Rendering:**
   - Razor view engine compiles the `.cshtml` view with Layout `_Layout.cshtml`, generating responsive HTML5.

### 3.2 REST API Request Flow
1. Client sends HTTP request with header `Authorization: Bearer <token>`.
2. `JwtBearerHandler` validates signature, expiration, issuer, and audience using HMAC-SHA256.
3. API controller returns a standardized `ApiResponse<T>` JSON envelope.
4. If unauthorized, returns HTTP 401/403 JSON instead of HTML login redirects.

---

## 4. Key Business Services

### 4.1 WarrantyCalculatorService
- **Responsibility:** Centralizes all warranty calculation logic.
- **Rules:**
  - `ExpiryDate = PurchaseDate.AddMonths(WarrantyDurationMonths)`.
  - `RemainingDays = ExpiryDate - Today`.
  - Status is evaluated as:
    - **Active:** `RemainingDays > 30`
    - **Expiring Soon:** `0 <= RemainingDays <= 30`
    - **Expired:** `RemainingDays < 0`

### 4.2 ServiceRequestWorkflowService
- **Responsibility:** Enforces the service ticket lifecycle state machine.
- **Supported Statuses:** Submitted, UnderReview, Assigned, InProgress, WaitingForCustomer, Resolved, Closed, Rejected.
- **Audit Logging:** Every transition creates an immutable `ServiceRequestHistory` record with UTC timestamp, performer identity, and remarks.

### 4.3 InvoiceStorageService
- **Responsibility:** Secure handling of uploaded customer invoice files.
- **Security Features:**
  - Files stored outside `wwwroot` in `Uploads/Invoices/` to prevent direct unauthenticated HTTP access.
  - File extension and MIME type allowlisting (`.pdf`, `.jpg`, `.jpeg`, `.png`).
  - Magic byte / file signature validation.
  - Maximum size restriction (5 MB).
  - Cryptographically unique filenames (`Guid:N + ext`).
  - Downloads streamed strictly after verifying product customer ownership.

---

## 5. Security Architecture

1. **Authentication:** ASP.NET Core Identity with PBKDF2 password hashing (SHA-256).
2. **Role-Based Access Control (RBAC):** Distinct roles `Admin` and `Customer`. New self-service registrations receive `Customer` role only.
3. **Cross-Site Request Forgery (CSRF):** `[ValidateAntiForgeryToken]` enforced on all state-altering MVC POST actions.
4. **Defense-in-Depth Ownership Checks:** Every database read and write explicitly filters by `UserId == CurrentUserId` unless the caller possesses the `Admin` role.
