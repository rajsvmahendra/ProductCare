# PRODUCTCARE — Product Warranty and Service Management Portal
**INT402: Modern Web Programming Tools and Techniques**  
**Technology Framework:** ASP.NET Core 10 (MVC & REST APIs) &middot; C# 13 &middot; Entity Framework Core 10 &middot; Microsoft SQL Server &middot; Bootstrap 5

[![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen)]()
[![Tests](https://img.shields.io/badge/xUnit%20Tests-50%2F50%20Passed-success)]()
[![Target Framework](https://img.shields.io/badge/.NET-10.0-blue)]()
[![Course Mapping](https://img.shields.io/badge/INT402-CO1--CO6%20Complete-teal)]()

---

## 1. Project Overview & Problem Statement

People frequently lose track of product purchase records, warranty periods, paper invoices, and after-sales service history. When a device or appliance develops a problem, consumers struggle to verify whether it remains under warranty, locate proof of purchase, or follow up on repair progress.

**ProductCare** centralizes this entire lifecycle in a single, modern web portal. It provides customers with an organized digital product vault, dynamic warranty status calculations, and real-time repair ticket tracking, while equipping service administrators with a structured operational portal to manage workloads and triage repair requests through a governed state machine workflow.

---

## 2. Key Features

### Customer Features
- **Account Registration & Security:** Self-service registration automatically assigned the `Customer` role; secure password hashing with ASP.NET Core Identity.
- **Product Inventory Management:** Full CRUD operations for purchased products (brand, model, serial number, purchase date, store, price, notes).
- **Secure Invoice Uploads:** PDF and image receipts uploaded and saved outside the web root (`Uploads/Invoices/`) with MIME type, extension, and magic byte file signature verification.
- **Dynamic Warranty Calculator:** Centralized business service calculating remaining coverage days and classifying warranties as `Active`, `Expiring Soon` (&le;30 days), or `Expired`.
- **Chronological Warranty Timeline:** Dedicated view showing expiration milestones across all registered devices.
- **Service Request Tracking:** Unique collision-safe ticket numbers (`PC-YYYYMMDD-XXXX`), category, priority, symptom descriptions, and an immutable status history audit timeline.
- **Customer Cancellation:** Ability to cancel tickets while still in the `Submitted` state before administrative processing begins.

### Administrator Features
- **Admin Dashboard:** System-wide key metrics: total customers, products, service requests, pending actions, and resolved counts.
- **Master Ticket Queue:** Search tickets by number, customer, or product; filter by status, priority, or category.
- **State Machine Workflow Management:** Advance tickets strictly through permissible transitions (`Submitted` &rarr; `UnderReview` &rarr; `Assigned` &rarr; `InProgress` &rarr; `WaitingForCustomer` &rarr; `Resolved` &rarr; `Closed` / `Rejected`).
- **Technician Assignment & Admin Remarks:** Assign technicians and record diagnostic remarks visible to the customer.
- **Customer Directory & Global Product Inventory:** Comprehensive lookup of customer accounts and registered devices.
- **Reports & Operational Intelligence:** Analytics aggregated via SQL Server Stored Procedure (`sp_GetServiceTicketStatistics`) and Database View (`vw_ProductWarrantySummary`).

### RESTful Web APIs & Integration
- Complete REST API controllers for Products and Service Requests.
- OpenAPI 3.0 / Swagger UI documentation at `/swagger`.
- JWT Bearer Authentication (`POST /api/auth/login`) for API consumers and Postman.
- Pre-built Postman collection: `postman/ProductCare.postman_collection.json`.

---

## 3. Technology Stack

| Layer | Technology |
|---|---|
| Programming Language | C# 13 |
| Web Framework | ASP.NET Core 10 (MVC + REST APIs) |
| Target Framework | .NET 10 (`net10.0`) |
| Frontend Rendering | Razor Views with TagHelpers |
| Styling & UI | Bootstrap 5.3 + Custom SaaS Theme (`wwwroot/css/site.css`) |
| Object-Relational Mapper | Entity Framework Core 10 |
| Database Engine | Microsoft SQL Server (LocalDB / Dedicated SQL Server) |
| Authentication & Identity | ASP.NET Core Identity (PBKDF2 Password Hashing) |
| API Authentication | JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`) |
| API Documentation | Swagger / OpenAPI (`Swashbuckle.AspNetCore`) |
| Unit Testing | xUnit 2.9.3 + Microsoft.NET.Test.Sdk |
| Database Migrations | EF Core Code First Migrations |

---

## 4. Solution Structure

```text
ProductCare_INT402/
│
├── ProductCare.sln                      # Master Solution File
├── Directory.Build.props                # Roslyn compiler & build settings
├── .gitignore                           # Git ignore configuration
├── .editorconfig                        # Code style & formatting rules
│
├── src/
│   └── ProductCare.Web/                 # ASP.NET Core 10 MVC Web Application
│       ├── Areas/
│       │   ├── Admin/                   # Administrative Area (Controllers & Views)
│       │   └── Customer/                # Customer Area (Controllers & Views)
│       ├── Controllers/                 # Account, Home, and REST API Controllers
│       ├── Data/                        # DbContext, Configurations, EF Migrations
│       ├── Middleware/                  # Correlation ID and Security Headers
│       ├── Models/                      # Entities and Domain Enums
│       ├── Services/                    # Domain Services (Warranty, Workflow, Invoices, Reports, JWT)
│       ├── ViewModels/                  # Strongly-typed UI ViewModels and API DTOs
│       ├── Views/                       # Shared layouts, Home, Account views
│       ├── wwwroot/                     # Bootstrap, jQuery, CSS, and JS assets
│       ├── appsettings.json             # Configuration & connection strings
│       └── Program.cs                   # Application entry point & DI pipeline
│
├── tests/
│   └── ProductCare.Tests/               # xUnit Unit Testing Project (50 Tests)
│       ├── WarrantyCalculatorServiceTests.cs
│       ├── ServiceRequestWorkflowServiceTests.cs
│       └── TicketNumberFormatTests.cs
│
├── docs/                                # Technical & Academic Documentation
│   ├── ARCHITECTURE.md                  # System architecture & data flow
│   ├── DATABASE.md                      # Database design, tables, view, stored proc
│   ├── ER_DIAGRAM.md                    # Entity-Relationship diagram (Mermaid)
│   ├── API_GUIDE.md                     # REST API guide & endpoint reference
│   ├── INT402_COURSE_MAPPING.md         # Syllabus mapping (CO1 to CO6)
│   ├── TEST_PLAN.md                     # QA test plan & execution outcomes
│   ├── BUILD_STATUS.md                  # Comprehensive build & verification log
│   └── Project_Report.md                # Full academic project report draft
│
├── scripts/                             # Database & Provisioning Scripts
│   ├── 00_InitialDatabase.sql           # Complete idempotent SQL Server DDL
│   ├── 01_Create_Views.sql              # SQL View: vw_ProductWarrantySummary
│   ├── 02_Create_Stored_Procedures.sql  # Stored Proc: sp_GetServiceTicketStatistics
│   ├── 03_Provision_Admin.ps1           # Secure admin provisioning script
│   └── 04_Seed_Sample_Data.sql          # Sample SQL test data script
│
├── postman/
│   └── ProductCare.postman_collection.json # Postman API Test Collection
│
└── README.md                            # Primary project guide
```

---

## 5. Prerequisites & Environment

- **Operating System:** Windows 10/11, Linux, or macOS.
- **.NET SDK:** .NET 10 SDK (`10.0.404` or higher installed).
- **Database:** Microsoft SQL Server (LocalDB instance `(localdb)\MSSQLLocalDB` or another SQL Server instance).
- **Browser:** Any modern web browser (Edge, Chrome, Firefox).

---

## 6. Configuration & Connection String

The database connection string is located in `src/ProductCare.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ProductCareDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

To target a remote or local named SQL Server instance (e.g. `localhost\SQLEXPRESS` or Docker SQL Server), update the `DefaultConnection` string accordingly.

---

## 7. Database Setup & Migration Execution

### Option A: Using EF Core CLI (Recommended)
Once your SQL Server or LocalDB instance is active, apply the migrations:
```powershell
dotnet ef database update --project src/ProductCare.Web --startup-project src/ProductCare.Web
```

### Option B: Using Idempotent SQL Scripts
Execute the generated SQL scripts via SQL Server Management Studio (SSMS), Azure Data Studio, or `sqlcmd`:
```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -d ProductCareDb -i scripts/00_InitialDatabase.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -d ProductCareDb -i scripts/01_Create_Views.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -d ProductCareDb -i scripts/02_Create_Stored_Procedures.sql
```

---

## 8. Secure Administrator Provisioning

To prevent credential leakage, administrator credentials are **never hardcoded** in source files or repository commits.

Provision the system administrator account using the secure provisioning script:
```powershell
.\scripts\03_Provision_Admin.ps1
```

Or execute directly with the application's provisioning CLI switch:
```powershell
dotnet run --project src/ProductCare.Web --no-build -- --provision-admin
```

This routine:
1. Ensures the `Admin` and `Customer` Identity roles exist in the database.
2. Checks if an administrator account already exists.
3. If not present, generates a cryptographically strong 16-character temporary password and displays it once in the local terminal.

### Optional: Seeding Demonstration Sample Data
To populate the database with realistic sample customers, registered products, and service tickets across various lifecycle stages, run:
```powershell
dotnet run --project src/ProductCare.Web --no-build -- --seed-demo-data
```
Or execute the SQL script:
```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -d ProductCareDb -i scripts/04_Seed_Sample_Data.sql
```

---

## 9. Launching the Application

Run the web application using the .NET CLI:
```powershell
dotnet run --project src/ProductCare.Web
```

The application will bind to:
- **HTTP:** `http://localhost:5000`
- **HTTPS:** `https://localhost:5001`
- **OpenAPI / Swagger UI:** `https://localhost:5001/swagger`

---

## 10. Customer & Administrator Demonstration Workflow

### Workflow 1: Customer Self-Service Experience
1. Open `https://localhost:5001` in your browser.
2. Click **Register** in the top navigation bar. Fill in your name, email, and password (e.g. `john@example.com` / `Customer123!`).
3. Upon registration, you are automatically signed in and redirected to **My Dashboard**.
4. Click **+ Add Product** (`/Customer/Products/Create`). Fill in product details (e.g., Apple MacBook Pro, S/N: `C02G1234MD6R`, 12 months warranty) and optionally attach a PDF invoice.
5. View the registered product in **My Products** and inspect the dynamic warranty status badge and remaining days.
6. Open **Warranty Coverage Overview** (`/Customer/Warranty`) to view the chronological warranty timeline.
7. Click **Raise Service Ticket** (`/Customer/ServiceRequests/Create`). Select your product, choose category `Repair`, set priority `High`, enter a subject and description, and submit.
8. Track the ticket lifecycle and review the chronological audit trail.

### Workflow 2: Administrator Operations Experience
1. Sign in with the provisioned administrator credentials.
2. The navbar displays an **Admin** badge and links to the **Admin Dashboard** (`/Admin/Dashboard`).
3. Click **Tickets Queue** (`/Admin/ServiceRequests`) to view all customer submissions across the organization.
4. Open the customer's submitted ticket (`/Admin/ServiceRequests/Details/{id}`).
5. Advance the ticket through the state machine: select `Under Review`, assign a technician, enter administrative remarks, and submit.
6. Advance again to `Assigned` &rarr; `In Progress` &rarr; `Resolved`.
7. View **Customer Accounts Directory** (`/Admin/Customers`) and **Global Product Inventory** (`/Admin/Products`).
8. Open **Reports & Intelligence** (`/Admin/Reports`) to inspect ticket statistics generated by stored procedure `sp_GetServiceTicketStatistics` and warranty data from view `vw_ProductWarrantySummary`.

---

## 11. Testing and Verification

Run the automated xUnit unit test suite:
```powershell
dotnet test tests/ProductCare.Tests/ProductCare.Tests.csproj --no-build
```

**Verification Results:**
- **Total Tests:** 50
- **Passed:** 50 (100% pass rate)
- **Failed:** 0
- **Execution Duration:** ~139 ms

Test suites cover:
- `WarrantyCalculatorServiceTests`: 17 tests (standard durations, leap years, month-end date math, boundary days, status mapping).
- `ServiceRequestWorkflowServiceTests`: 26 tests (valid and invalid state transitions, terminal states, next permissible statuses).
- `TicketNumberFormatTests`: 7 tests (regex pattern matching for `PC-YYYYMMDD-XXXX`).

---

## 12. Troubleshooting & FAQ

- **LocalDB Connection Error on Startup:** If SQL Server LocalDB encounters an operating system startup error, the application's `DashboardReportingService` executes resilient LINQ fallbacks for all reporting queries, allowing the portal to run without crashing.
- **Port Conflict:** If port 5001 is already in use, customize the URLs in `src/ProductCare.Web/Properties/launchSettings.json`.
- **Swagger Bearer Token Testing:** Call `POST /api/auth/login` to retrieve a JWT token. In Swagger UI, click **Authorize** at the top right, paste `Bearer <your-token>`, and click Authorize.

---

## 13. Academic Declaration

This application and documentation have been developed in compliance with the **INT402 Modern Web Programming Tools and Techniques** curriculum. All course outcomes (CO1–CO6) are fully realized and documented in `docs/INT402_COURSE_MAPPING.md`.
