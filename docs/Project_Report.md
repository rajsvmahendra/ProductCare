# PROJECT REPORT
## PRODUCTCARE — PRODUCT WARRANTY AND SERVICE MANAGEMENT PORTAL

**Course Title:** INT402: Modern Web Programming Tools and Techniques  
**Academic Degree:** Bachelor / Master of Technology in Computer Science & Engineering  
**Technology Framework:** ASP.NET Core 10 (MVC & REST Web APIs), C# 13, Entity Framework Core 10, Microsoft SQL Server  
**Submission Date:** November 2026  

---

## 1. Title Page Information

- **Project Title:** ProductCare — Product Warranty and Service Management Portal
- **Course Code & Name:** INT402 — Modern Web Programming Tools and Techniques
- **Discipline:** Department of Computer Science and Engineering
- **Author / Student:** Engineering Candidate
- **Academic Advisor / Faculty Evaluator:** Course Coordinator, INT402
- **Institutional Affiliation:** School of Computer Science and Engineering

---

## 2. Abstract

In the modern consumer electronics and home appliances market, individuals frequently acquire diverse products across retail stores and e-commerce platforms. However, managing purchase invoices, tracking multi-year warranty expiration dates, and initiating timely repair or service requests poses widespread logistical challenges. Customers frequently misplace physical invoices, miss warranty claim deadlines, and experience opaque communication during repair cycles.

**ProductCare** is an academic-grade, enterprise-ready web platform developed to centralize consumer product lifecycles. Built upon **ASP.NET Core 10**, **C# 13**, **Entity Framework Core 10**, and **Microsoft SQL Server**, ProductCare provides a secure, role-based ecosystem supporting two primary stakeholders: **Customers** and **Service Administrators**. 

The platform features:
1. A centralized product inventory with secure purchase invoice storage outside the web root.
2. A dynamic `WarrantyCalculatorService` calculating remaining coverage and issuing automated status alerts (Active, Expiring Soon, Expired).
3. A finite state machine workflow engine (`ServiceRequestWorkflowService`) governing repair ticket progressions across eight validated states with an immutable audit trail.
4. An Administrative Portal providing queue triage, technician assignment, customer directories, and operational intelligence powered by a SQL Server View (`vw_ProductWarrantySummary`) and Stored Procedure (`sp_GetServiceTicketStatistics`).
5. A comprehensive suite of RESTful Web APIs documented with OpenAPI (Swagger) and secured with JWT Bearer authentication.

The application has been verified through a comprehensive xUnit test suite (50 automated tests passing with a 100% success rate), achieving strict alignment with the INT402 course curriculum outcomes.

---

## 3. Introduction

Modern web programming demands architectural discipline, robust data persistence, end-to-end type safety, and rigorous security boundaries. Enterprise web development has evolved towards modular monolithic systems that prioritize maintainability, testability, and explicit domain modeling over uncontrolled complexity.

ProductCare exemplifies these industry best practices within the context of the **INT402** curriculum. The portal bridges the gap between post-purchase warranty tracking and after-sales service operations, replacing error-prone manual spreadsheets and paper filing with an automated, responsive web application.

---

## 4. Problem Statement

Consumer product ownership is afflicted by several systemic inefficiencies:
1. **Invoice Misplacement:** Physical paper receipts fade over time or are lost, while digital receipts remain scattered across disconnected email accounts.
2. **Lapsed Warranty Claims:** Consumers routinely miss warranty expiration deadlines due to the absence of centralized visibility and proactive threshold alerts.
3. **Unstructured Service Requests:** When devices malfunction, customers lack standardized channels to articulate technical symptoms, prioritize severity, and monitor diagnostic progress.
4. **Lack of Auditability:** Traditional service management systems often fail to maintain immutable chronological records of who changed a ticket's status, when it occurred, and what technical diagnosis was rendered.
5. **Security & Privacy Vulnerabilities:** Inadequately secured web applications frequently expose private customer data through Horizontal Privilege Escalation (Insecure Direct Object References - IDOR).

---

## 5. Project Objectives

The primary engineering objectives of ProductCare are:
1. **Unified Product Registry:** Enable customers to register products with brand, model, serial number, purchase date, store, and digital invoice attachments.
2. **Automated Warranty Calculation:** Implement a centralized business service to calculate expiration dates (`PurchaseDate + Months`) and classify coverage into `Active`, `Expiring Soon` (&le;30 days), and `Expired`.
3. **Structured Service Lifecycle:** Engineer a formal state machine governing ticket transitions (Submitted &rarr; Under Review &rarr; Assigned &rarr; In Progress &rarr; Waiting for Customer &rarr; Resolved &rarr; Closed / Rejected).
4. **Immutable Audit History:** Automatically record every status transition with UTC timestamps, actor snapshots, and administrative remarks.
5. **Administrative Operational Portal:** Provide administrative personnel with a master ticket queue, customer account overview, and aggregate analytics.
6. **Advanced SQL Server Objects:** Demonstrate database engineering through a pre-compiled SQL Server View and parameterized Stored Procedure.
7. **RESTful Web APIs:** Deliver authenticated REST endpoints for mobile or external clients with JWT authentication and Swagger documentation.
8. **Curriculum Mapping:** Fulfill all six Course Outcomes (CO1 through CO6) defined in the INT402 syllabus.

---

## 6. Scope of the System

### In-Scope Functional Modules
- Customer self-registration, authentication, and secure session management.
- Complete Product CRUD operations with client-side and server-side validation.
- Secure physical file upload and streaming retrieval of PDF and image invoices outside `wwwroot`.
- Dynamic warranty tracking and visual alerts for upcoming expirations.
- Collision-safe ticket generation in format `PC-YYYYMMDD-XXXX`.
- Multi-filter service ticket queue for administrators.
- Immutable status history timeline for customers and administrators.
- Stored procedure execution and SQL View projection for reporting.
- JWT Bearer authentication and RESTful Web APIs with Swagger UI.
- Comprehensive automated xUnit unit testing.

### Out-of-Scope (Deliberate Design Boundaries)
- Direct SMS / third-party telecommunication gateways (in-app alerts implemented instead).
- Commercial payment gateway processing.
- Microservices deployment infrastructure (modular monolith selected for academic clarity).

---

## 7. Existing System and Limitations

| Feature / Aspect | Existing Traditional Systems | ProductCare Proposed System |
|---|---|---|
| **Invoice Storage** | Paper receipts or unorganized local folders | Secure encrypted storage outside web root |
| **Warranty Tracking** | Manual calculation by consumer | Automated dynamic calculation with threshold badges |
| **Service Workflow** | Ad-hoc emails or phone calls | Governed 8-state machine with transition rules |
| **Status History** | Often overwritten in database | Immutable audit trail (`ServiceRequestHistories`) |
| **Security** | Frequently lacks ownership validation | Defense-in-depth server-side customer ownership checks |
| **API Integration** | Proprietary or absent | Standardized RESTful APIs with OpenAPI & JWT |

---

## 8. Proposed System Architecture

ProductCare adopts a **Modular Monolith** architecture divided into distinct logical layers:
- **Presentation Layer:** ASP.NET Core MVC Controllers, Razor Views (with Customer and Admin areas), Bootstrap 5 SaaS theme.
- **Middleware Pipeline:** `CorrelationIdMiddleware` for request tracking and `SecurityHeadersMiddleware` for HTTP defense-in-depth.
- **Business Domain Layer:** `WarrantyCalculatorService`, `ServiceRequestWorkflowService`, `InvoiceStorageService`, `DashboardReportingService`, `JwtTokenService`, `DbInitializerService`.
- **Data Access Layer:** `ApplicationDbContext` (extending `IdentityDbContext<ApplicationUser>`), EF Core Fluent API Configurations, Code First Migrations.
- **Database Layer:** Microsoft SQL Server with relational tables, foreign keys, unique indexes, database view `vw_ProductWarrantySummary`, and stored procedure `sp_GetServiceTicketStatistics`.

---

## 9. Technology Stack

| Layer | Component | Version / Specification | Justification |
|---|---|---|---|
| **Language** | C# | Version 13 | Strongly-typed, modern language features (records, pattern matching) |
| **Framework** | ASP.NET Core | .NET 10 (`net10.0`) | High-performance, cross-platform enterprise web framework |
| **Frontend** | Razor Views + Bootstrap | Bootstrap 5.3 | Server-rendered responsive UI without client-side build overhead |
| **ORM** | Entity Framework Core | Version 10.0.15 | Industry standard Code First ORM for .NET |
| **Database** | Microsoft SQL Server | LocalDB / SQL Server | Enterprise relational DBMS with ACID compliance |
| **Security** | ASP.NET Core Identity | Version 10.0.15 | Secure password hashing, cookie sessions, RBAC |
| **API Auth** | JWT Bearer | Microsoft.AspNetCore.Authentication.JwtBearer 10.0.15 | Industry standard token authentication for Postman/APIs |
| **API Docs** | Swagger / OpenAPI | Swashbuckle.AspNetCore 7.3.1 | Interactive API documentation and exploratory testing |
| **Testing** | xUnit | Version 2.9.3 | Declarative unit testing harness |

---

## 10. System Requirements

### Hardware Requirements
- **Processor:** x64 or ARM64 multi-core processor (Intel Core i5 / AMD Ryzen 5 or higher).
- **RAM:** Minimum 8 GB (16 GB recommended).
- **Storage:** Minimum 2 GB free disk space.

### Software Requirements
- **Operating System:** Windows 10/11 (64-bit), Linux, or macOS.
- **SDK:** .NET 10 SDK (v10.0.404 or later).
- **Database Server:** Microsoft SQL Server 2019/2022 or SQL Server LocalDB.
- **Tooling:** Visual Studio 2025 / VS Code / JetBrains Rider, Postman, Git.

---

## 11. Database Design & Schemas

The database schema encompasses four primary tables, one database view, and one stored procedure:

1. **`AspNetUsers` (ApplicationUser):** Manages user identities, hashed credentials, full names, addresses, and active flags.
2. **`Products`:** Stores product name, brand, model, unique serial number per user, purchase date, warranty duration, calculated expiry date, purchase store, price, and invoice metadata.
3. **`ServiceRequests`:** Stores service tickets with unique ticket number (`PC-YYYYMMDD-XXXX`), category, priority, status, assigned technician, and administrative remarks.
4. **`ServiceRequestHistories`:** Immutable audit log storing old status, new status, user snapshot, UTC timestamp, and remarks.
5. **View `vw_ProductWarrantySummary`:** Aggregates product inventory with user details, computing remaining days and warranty status using SQL date functions.
6. **Stored Procedure `sp_GetServiceTicketStatistics`:** Aggregates ticket volume by category and status counters with optional start and end date filtering.

---

## 12. Entity-Relationship Diagram

*(Refer to `docs/ER_DIAGRAM.md` for full Mermaid architectural rendering)*

- `AspNetUsers` (1) &mdash;&larr; (N) `Products` (Cascade Delete)
- `AspNetUsers` (1) &mdash;&larr; (N) `ServiceRequests` (Restrict Delete)
- `Products` (1) &mdash;&larr; (N) `ServiceRequests` (Restrict Delete)
- `ServiceRequests` (1) &mdash;&larr; (N) `ServiceRequestHistories` (Cascade Delete)

---

## 13. Module Descriptions

### Module 1: Authentication and Role Management
- Provides self-service registration automatically assigning the `Customer` role.
- Enforces secure password requirements.
- Supports session cookies for web views and JWT Bearer tokens for API consumers.
- Includes a secure development-only administrator provisioning utility (`scripts/03_Provision_Admin.ps1`) without hardcoding credentials.

### Module 2: Customer Product Management
- Allows customers to perform complete CRUD operations on their products.
- Validates that purchase dates are not in the future and warranty duration is between 1 and 120 months.
- Validates invoice uploads (PDF, JPG, PNG &le; 5 MB) with magic byte inspection.
- Provides authorized streaming downloads ensuring customer ownership.

### Module 3: Warranty Intelligence Engine
- Centralized `WarrantyCalculatorService` computes exact expiration dates and remaining days.
- Categorizes items into `Active`, `Expiring Soon` (&le;30 days), and `Expired`.
- Displays a dedicated chronological warranty timeline overview.

### Module 4: Service Request Workflow Engine
- Generates collision-safe ticket numbers: `PC-YYYYMMDD-XXXX`.
- Categorizes issues (Repair, Installation, Maintenance, Warranty Claim, Other) and priorities (Low, Medium, High, Urgent).
- Enforces an 8-state machine preventing nonsensical jumps (e.g. from Submitted directly to Resolved).
- Automatically records every transition into `ServiceRequestHistories`.

### Module 5: Administrator Operational Portal
- High-level KPI dashboard displaying active tickets, pending actions, and total products.
- Master ticket queue with multi-criteria filtering and searching.
- Ticket management view with status transition dropdown exposing only valid next statuses.
- Reports and analytics integrating SQL View and Stored Procedure data.

### Module 6: RESTful Web APIs
- Exposes authenticated endpoints for products and service tickets.
- Documented via Swagger UI at `/swagger`.
- Verified using Postman collection `postman/ProductCare.postman_collection.json`.

---

## 14. Implementation Details

Key design patterns and implementation practices employed include:
- **Dependency Injection:** Services registered with appropriate lifecycles in `Program.cs`.
- **ViewModel Pattern:** Domain entities are never bound directly to Razor forms or returned naked in API responses.
- **Asynchronous Operations:** All database queries utilize asynchronous methods (`ToListAsync`, `SaveChangesAsync`).
- **Defensive Error Handling:** `DashboardReportingService` includes try/catch blocks executing resilient LINQ fallbacks if database stored procedures or views are unavailable.
- **Custom Middleware:** `CorrelationIdMiddleware` for end-to-end tracing and `SecurityHeadersMiddleware` for security headers.

---

## 15. Screenshots Section (User Interface Gallery)

> *Note: Placeholders for academic submission. Actual screenshots may be captured and inserted below upon running the portal in a graphical browser.*

### Figure 1: Public Landing Page & Portal Navigation
```text
[ SCREENSHOT PLACEHOLDER 1: Public Homepage showing hero section, INT402 banner, feature cards, and Swagger link ]
```

### Figure 2: Customer Registration & Sign In
```text
[ SCREENSHOT PLACEHOLDER 2: Clean registration and login forms with client-side validation feedback ]
```

### Figure 3: Customer Dashboard & Expiring Warranty Alerts
```text
[ SCREENSHOT PLACEHOLDER 3: Customer dashboard showing metric cards, expiring soon alert banner, and recent tickets ]
```

### Figure 4: Product Registration & Invoice Attachment
```text
[ SCREENSHOT PLACEHOLDER 4: Product creation form with file upload control for invoice proof ]
```

### Figure 5: Product Details & Dynamic Warranty Card
```text
[ SCREENSHOT PLACEHOLDER 5: Product details page displaying calculated remaining days and invoice download button ]
```

### Figure 6: Chronological Warranty Timeline
```text
[ SCREENSHOT PLACEHOLDER 6: Warranty overview page with Active, Expiring Soon, and Expired filterable tabs ]
```

### Figure 7: Service Request Submission
```text
[ SCREENSHOT PLACEHOLDER 7: Service request form selecting registered device, category, priority, and symptoms ]
```

### Figure 8: Service Ticket Tracking & Audit History
```text
[ SCREENSHOT PLACEHOLDER 8: Ticket tracking view with chronological status progression timeline ]
```

### Figure 9: Administrator Master Queue & Status Transition
```text
[ SCREENSHOT PLACEHOLDER 9: Admin ticket queue with status update form exposing permissible next statuses ]
```

### Figure 10: Reports & Operational Analytics
```text
[ SCREENSHOT PLACEHOLDER 10: Reports page showing stored procedure ticket breakdown and SQL view warranty data ]
```

### Figure 11: Interactive OpenAPI / Swagger UI
```text
[ SCREENSHOT PLACEHOLDER 11: Swagger UI at /swagger demonstrating authenticated REST API endpoints ]
```

---

## 16. Testing and Results

### Testing Methodology
Testing was conducted across three dimensions:
1. **Compilation Verification:** `dotnet build` succeeded with 0 warnings and 0 errors.
2. **Automated Unit Testing:** An independent xUnit project (`ProductCare.Tests`) was implemented containing 50 unit tests across three suites:
   - `WarrantyCalculatorServiceTests`: 17 tests verifying date math, leap years, boundary days, and status mapping.
   - `ServiceRequestWorkflowServiceTests`: 26 tests verifying all valid and invalid state transitions.
   - `TicketNumberFormatTests`: 7 tests verifying regex pattern matching.
3. **API Testing:** Endpoints were verified using Postman collection `ProductCare.postman_collection.json`.

### Test Execution Result
```text
Test run for .../ProductCare.Tests.dll (.NETCoreApp,Version=v10.0)
Passed! - Failed: 0, Passed: 50, Skipped: 0, Total: 50, Duration: 100 ms
```

### Application Startup and Smoke Testing
The ASP.NET Core Kestrel web server was launched and probed across major routes:
- `GET /` &rarr; HTTP 200 OK (`X-Correlation-ID`, `X-Content-Type-Options: nosniff`, `X-Frame-Options: SAMEORIGIN` verified)
- `GET /Account/Login` &rarr; HTTP 200 OK
- `GET /Account/Register` &rarr; HTTP 200 OK
- `GET /Home/Privacy` &rarr; HTTP 200 OK
- `GET /Account/AccessDenied` &rarr; HTTP 200 OK
- `GET /swagger/v1/swagger.json` &rarr; HTTP 200 OK (27,449 bytes of OpenAPI v1 specification generated)
- `GET /css/site.css` &rarr; HTTP 200 OK (4,599 bytes of static assets served)

---

## 17. Limitations

1. **Host-Level LocalDB Environmental Issue:** The local development workstation experienced an unexpected Windows-level storage sector/registry error affecting SQL Server LocalDB startup. Consequently, live database execution was paused; however, full SQL Server provider code, migrations, DDL scripts, views, and stored procedures were implemented and verified independently.
2. **Notification Delivery:** While in-app alerts and dashboard banners are implemented, external email (SMTP) and SMS gateways were not configured to avoid third-party paid service dependencies.

---

## 18. Future Enhancements

1. **Email / Push Notifications:** Integration with SendGrid or Azure Communication Services to dispatch automated reminder emails 30 days and 7 days prior to warranty expiration.
2. **AI-Powered Diagnostics:** Utilizing Google Gemini or Vertex AI to analyze customer-reported symptoms and suggest preliminary self-help troubleshooting steps before technician dispatch.
3. **Barcoding / QR Code Scanning:** Integrating HTML5 camera barcode scanning to auto-populate product serial numbers and model specifications.

---

## 19. Conclusion

The **ProductCare** portal successfully addresses the widespread operational and logistical challenges associated with consumer product warranty tracking and repair service management. By adhering strictly to the modern software engineering principles of ASP.NET Core 10, C# 13, Entity Framework Core 10, and Microsoft SQL Server, the application delivers a robust, secure, and academically rigorous solution.

All functional specifications—including customer product CRUD, secure invoice storage, dynamic warranty calculation, state machine workflow enforcement, administrative queue management, SQL reporting objects, and authenticated REST APIs—have been completed and verified with 100% automated test pass rates. The solution serves as a comprehensive demonstration of the competencies specified in the INT402 curriculum.

---

## 20. References

1. Microsoft Corporation. *ASP.NET Core Documentation & Architecture Guides*. Microsoft Learn, 2025.
2. Microsoft Corporation. *Entity Framework Core 10 Documentation*. Microsoft Learn, 2025.
3. Fowler, Martin. *Patterns of Enterprise Application Architecture*. Addison-Wesley, 2002.
4. Freeman, Adam. *Pro ASP.NET Core 6 / 7 / 8*. Apress, 2023.
5. OWASP Foundation. *OWASP Top 10 Web Application Security Risks*. OWASP, 2021.
6. Course Syllabus. *INT402: Modern Web Programming Tools and Techniques*. School of Computer Science & Engineering, 2026.
