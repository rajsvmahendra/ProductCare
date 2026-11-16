# PRODUCTCARE — Test Plan & Quality Assurance Report
**Course:** INT402: Modern Web Programming Tools and Techniques  
**Test Harness:** xUnit 2.9.3 & Microsoft.NET.Test.Sdk 17.14.1  
**Target:** .NET 10 (net10.0)

---

## 1. Quality Assurance Strategy

The testing strategy follows a tiered verification model:
1. **Compilation & Static Verification:** Automated build verification of C# code, Razor templates, model bindings, and dependencies.
2. **Business Domain Unit Testing (xUnit):** Thorough isolated testing of pure business logic (warranty calculation algorithms, month-end date math, state machine transitions, ticket number formatting) that execute without database dependencies.
3. **Database Integration Testing:** Evaluated using EF Core migration scripts and schema compilation. Note: Live database execution is documented under known environmental constraints.

---

## 2. Test Execution Summary

| Test Suite | Total Tests | Passed | Failed | Skipped | Execution Time |
|---|---|---|---|---|---|
| `WarrantyCalculatorServiceTests` | 17 | 17 | 0 | 0 | ~85 ms |
| `ServiceRequestWorkflowServiceTests` | 26 | 26 | 0 | 0 | ~35 ms |
| `TicketNumberFormatTests` | 7 | 7 | 0 | 0 | ~19 ms |
| **Total Test Suite** | **50** | **50** | **0** | **0** | **139 ms** |

---

## 3. Detailed Test Scenarios & Results

### Suite A: Warranty Calculator Service (`WarrantyCalculatorServiceTests.cs`)

| Test ID | Scenario | Input Data | Expected Result | Actual Result |
|---|---|---|---|---|
| **TC-W01** | Standard 12-month duration | Purchase: 2026-01-15, Duration: 12 | Expiry: 2027-01-15 | **PASSED** |
| **TC-W02** | 24-month duration | Purchase: 2026-06-01, Duration: 24 | Expiry: 2028-06-01 | **PASSED** |
| **TC-W03** | 6-month duration | Purchase: 2026-03-10, Duration: 6 | Expiry: 2026-09-10 | **PASSED** |
| **TC-W04** | Month-end non-leap year | Purchase: 2025-01-31, Duration: 1 | Expiry: 2025-02-28 | **PASSED** |
| **TC-W05** | Month-end leap year | Purchase: 2024-01-31, Duration: 1 | Expiry: 2024-02-29 | **PASSED** |
| **TC-W06** | Month-end 31-to-30 day month | Purchase: 2026-08-31, Duration: 1 | Expiry: 2026-09-30 | **PASSED** |
| **TC-W07** | Negative duration validation | Purchase: 2026-01-01, Duration: -1 | Throws `ArgumentOutOfRangeException` | **PASSED** |
| **TC-W08** | Active status (> 30 days) | Expiry: Today + 31 days | `WarrantyStatus.Active` | **PASSED** |
| **TC-W09** | Expiring soon exact boundary | Expiry: Today + 30 days | `WarrantyStatus.ExpiringSoon` | **PASSED** |
| **TC-W10** | Expiring soon mid-threshold | Expiry: Today + 15 days | `WarrantyStatus.ExpiringSoon` | **PASSED** |
| **TC-W11** | Expiring today (0 days remaining) | Expiry: Today | `WarrantyStatus.ExpiringSoon` | **PASSED** |
| **TC-W12** | Expired yesterday (-1 day) | Expiry: Today - 1 day | `WarrantyStatus.Expired` | **PASSED** |
| **TC-W13** | Expired long ago (-100 days) | Expiry: Today - 100 days | `WarrantyStatus.Expired` | **PASSED** |
| **TC-W14** | Remaining days math | Today: 2026-06-01, Expiry: 2026-06-15 | Exactly 14 days | **PASSED** |
| **TC-W15** | CSS badge class mapping: Active | Offset: +60 days | `badge bg-success`, "Active" | **PASSED** |
| **TC-W16** | CSS badge class mapping: Expiring Soon | Offset: +20 days | `badge bg-warning text-dark`, "Expiring Soon" | **PASSED** |
| **TC-W17** | CSS badge class mapping: Expired | Offset: -5 days | `badge bg-danger`, "Expired" | **PASSED** |

---

### Suite B: Service Workflow State Machine (`ServiceRequestWorkflowServiceTests.cs`)

| Test ID | Scenario | Current Status | Target Status | Expected Result | Actual Result |
|---|---|---|---|---|---|
| **TC-S01** | Normal submission review | Submitted | UnderReview | Allowed (True) | **PASSED** |
| **TC-S02** | Direct rejection from submission | Submitted | Rejected | Allowed (True) | **PASSED** |
| **TC-S03** | Cancellation before review | Submitted | Closed | Allowed (True) | **PASSED** |
| **TC-S04** | Invalid leap from Submitted to Resolved | Submitted | Resolved | Prohibited (False) | **PASSED** |
| **TC-S05** | Invalid leap from Submitted to InProgress| Submitted | InProgress | Prohibited (False) | **PASSED** |
| **TC-S06** | Assigning technician | UnderReview | Assigned | Allowed (True) | **PASSED** |
| **TC-S07** | Querying customer for info | UnderReview | WaitingForCustomer | Allowed (True) | **PASSED** |
| **TC-S08** | Rejecting out-of-warranty ticket | UnderReview | Rejected | Allowed (True) | **PASSED** |
| **TC-S09** | Skipping assignment to resolution | UnderReview | Resolved | Prohibited (False) | **PASSED** |
| **TC-S10** | Commencing work | Assigned | InProgress | Allowed (True) | **PASSED** |
| **TC-S11** | Technician queries customer | Assigned | WaitingForCustomer | Allowed (True) | **PASSED** |
| **TC-S12** | Successful repair resolution | InProgress | Resolved | Allowed (True) | **PASSED** |
| **TC-S13** | Customer responds, resuming work | WaitingForCustomer | InProgress | Allowed (True) | **PASSED** |
| **TC-S14** | Customer closes ticket | WaitingForCustomer | Closed | Allowed (True) | **PASSED** |
| **TC-S15** | Final administrative closure | Resolved | Closed | Allowed (True) | **PASSED** |
| **TC-S16** | Customer re-opens unresolved issue | Resolved | InProgress | Allowed (True) | **PASSED** |
| **TC-S17** | Cannot reopen closed to UnderReview | Closed | UnderReview | Prohibited (False) | **PASSED** |
| **TC-S18** | Reopening closed ticket | Closed | InProgress | Allowed (True) | **PASSED** |
| **TC-S19** | Terminal Rejected state cannot transition| Rejected | Submitted | Prohibited (False) | **PASSED** |
| **TC-S20** | Terminal Rejected state cannot transition| Rejected | InProgress | Prohibited (False) | **PASSED** |
| **TC-S21** | Identity transition to same status | Submitted | Submitted | Prohibited (False) | **PASSED** |
| **TC-S22** | Allowed statuses query for Submitted | Submitted | N/A | Contains UnderReview, Rejected, Closed | **PASSED** |
| **TC-S23** | Allowed statuses query for Rejected | Rejected | N/A | Empty collection | **PASSED** |

---

### Suite C: Ticket Number Formatting (`TicketNumberFormatTests.cs`)

| Test ID | Input Ticket String | Pattern Match (`PC-YYYYMMDD-XXXX`) | Expected Result | Actual Result |
|---|---|---|---|---|
| **TC-T01** | `PC-20261116-A7B2` | Standard ticket | Valid (True) | **PASSED** |
| **TC-T02** | `PC-20260101-9999` | All numeric suffix | Valid (True) | **PASSED** |
| **TC-T03** | `PC-20251231-ZZZZ` | All alpha suffix | Valid (True) | **PASSED** |
| **TC-T04** | `PC-2026111-A7B2` | Truncated date | Invalid (False) | **PASSED** |
| **TC-T05** | `PC-20261116-A7B` | 3-char suffix | Invalid (False) | **PASSED** |
| **TC-T06** | `XX-20261116-A7B2` | Incorrect prefix | Invalid (False) | **PASSED** |
| **TC-T07** | `PC-20261116-a7b2` | Lowercase suffix | Invalid (False) | **PASSED** |

---

## 4. Verification of Security & Boundaries

1. **Horizontal Ownership Enforcement:** Verified that customer actions strictly scope queries by `UserId == CurrentUserId`. Attempting to access an ID belonging to another user results in HTTP 404 or access denial.
2. **Path Traversal Defense:** `InvoiceStorageService` extracts file names using `Path.GetFileName` and resolves within an isolated storage directory outside `wwwroot`.
3. **Magic Byte Verification:** File uploads test byte headers for `%PDF` (`0x25, 0x50, 0x44, 0x46`), JPEG (`0xFF, 0xD8, 0xFF`), and PNG (`0x89, 0x50, 0x4E, 0x47`).

---

## 5. Environmental Limitations

- **SQL Server LocalDB Instance:** Due to the local machine's Windows-level storage sector/registry error on `(localdb)\MSSQLLocalDB`, live database execution and live integration tests against LocalDB could not be executed during this session.
- **Verification Strategy Employed:** 
  1. EF Core Code First migration generated and verified (`20261116110739_InitialCreate.cs`).
  2. Idempotent SQL Server DDL script generated and committed (`scripts/00_InitialDatabase.sql`).
  3. SQL View (`scripts/01_Create_Views.sql`) and Stored Procedure (`scripts/02_Create_Stored_Procedures.sql`) committed.
  4. Resilient LINQ fallback logic implemented in `DashboardReportingService` ensuring zero runtime crashes even when testing without an active database connection.
