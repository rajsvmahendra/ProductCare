# PRODUCTCARE — Entity-Relationship (ER) Diagram
**Course:** INT402: Modern Web Programming Tools and Techniques  
**System:** ProductCare — Product Warranty and Service Management Portal

---

## 1. Entity-Relationship Diagram (Mermaid)

```mermaid
erDiagram
    AspNetUsers ||--o{ Products : "registers (1:N)"
    AspNetUsers ||--o{ ServiceRequests : "submits (1:N)"
    AspNetUsers ||--o{ ServiceRequestHistories : "transitions (1:N)"
    Products ||--o{ ServiceRequests : "has (1:N)"
    ServiceRequests ||--o{ ServiceRequestHistories : "audited_by (1:N)"

    AspNetUsers {
        nvarchar(450) Id PK
        nvarchar(100) FullName
        nvarchar(256) Email
        nvarchar(256) UserName
        nvarchar(max) PasswordHash
        nvarchar(250) Address
        nvarchar(100) City
        nvarchar(20) PostalCode
        bit IsActive
        datetime2 CreatedAtUtc
        datetime2 UpdatedAtUtc
    }

    Products {
        int Id PK
        nvarchar(450) UserId FK
        nvarchar(150) Name
        nvarchar(100) Brand
        nvarchar(100) ModelNumber
        nvarchar(100) SerialNumber
        int Category
        date PurchaseDate
        int WarrantyDurationMonths
        date WarrantyExpiryDate
        nvarchar(150) PurchaseStore
        decimal(18_2) PurchasePrice
        nvarchar(255) InvoiceFileName
        nvarchar(255) InvoiceOriginalFileName
        nvarchar(100) InvoiceContentType
        bigint InvoiceFileSize
        nvarchar(1000) Notes
        datetime2 CreatedAtUtc
        datetime2 UpdatedAtUtc
    }

    ServiceRequests {
        int Id PK
        nvarchar(30) TicketNumber UK
        int ProductId FK
        nvarchar(450) UserId FK
        int Category
        nvarchar(200) Subject
        nvarchar(2000) Description
        int Priority
        int Status
        nvarchar(100) AssignedTechnician
        nvarchar(2000) AdminRemarks
        datetime2 CreatedAtUtc
        datetime2 UpdatedAtUtc
        datetime2 ResolvedAtUtc
    }

    ServiceRequestHistories {
        int Id PK
        int ServiceRequestId FK
        int FromStatus
        int ToStatus
        nvarchar(450) ChangedByUserId FK
        nvarchar(100) ChangedByUserName
        nvarchar(1000) Remarks
        datetime2 CreatedAtUtc
    }
```

---

## 2. Cardinality and Referential Integrity Rules

1. **User to Product (1 : N):**
   - One `ApplicationUser` owns zero or more `Products`.
   - Each `Product` belongs to exactly one `ApplicationUser`.
   - Delete Behavior: `Cascade`. If a user profile is purged, their registered inventory is removed.
   - Unique Constraint: `(UserId, SerialNumber)` ensures a customer cannot register the same serial number multiple times.

2. **Product to ServiceRequest (1 : N):**
   - One `Product` has zero or more `ServiceRequests`.
   - Each `ServiceRequest` references exactly one `Product`.
   - Delete Behavior: `Restrict`. A product cannot be deleted if there is an active service ticket associated with it.

3. **User to ServiceRequest (1 : N):**
   - One `ApplicationUser` submits zero or more `ServiceRequests`.
   - Each `ServiceRequest` is owned by exactly one customer.
   - Delete Behavior: `Restrict`.

4. **ServiceRequest to ServiceRequestHistory (1 : N):**
   - One `ServiceRequest` has one or more `ServiceRequestHistory` audit entries.
   - Each history record represents an immutable state transition.
   - Delete Behavior: `Cascade`. Deleting a ticket purges its history logs.
