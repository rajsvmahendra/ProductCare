# PRODUCTCARE — RESTful Web API Guide
**Course:** INT402: Modern Web Programming Tools and Techniques  
**Base URL:** `https://localhost:5001/api` (or `http://localhost:5000/api`)  
**Specification:** OpenAPI 3.0 / Swagger UI (`/swagger`)

---

## 1. Authentication & Security

The ProductCare REST APIs support **JWT (JSON Web Token) Bearer Authentication** using HMAC-SHA256 signature verification.

### 1.1 Obtain JWT Token
- **Endpoint:** `POST /api/auth/login`
- **Request Body:**
```json
{
  "email": "customer@example.com",
  "password": "Customer123!"
}
```
- **Response (HTTP 200 OK):**
```json
{
  "success": true,
  "message": "Authentication successful.",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "customer@example.com",
    "fullName": "Jane Doe",
    "roles": ["Customer"],
    "expiresAtUtc": "2026-11-16T18:00:00Z"
  },
  "errors": []
}
```

### 1.2 Using the Token
Pass the received token in the standard HTTP `Authorization` request header:
```http
Authorization: Bearer <your-jwt-token>
```

---

## 2. Products API Endpoints

### 2.1 Get All Products
- **Route:** `GET /api/products`
- **Query Parameters:**
  - `category` (optional, integer 1–8, 99)
  - `status` (optional, integer: 1=Active, 2=ExpiringSoon, 3=Expired)
- **Authorization:** Customer (sees own products) or Admin (sees all products)
- **Response (HTTP 200 OK):**
```json
{
  "success": true,
  "message": "Request processed successfully.",
  "data": [
    {
      "id": 1,
      "name": "Dell UltraSharp 27 4K Monitor",
      "brand": "Dell",
      "modelNumber": "U2723QE",
      "serialNumber": "SN-DELL-987654",
      "category": 2,
      "categoryName": "Computers",
      "purchaseDate": "2026-01-10",
      "warrantyDurationMonths": 36,
      "warrantyExpiryDate": "2029-01-10",
      "remainingDays": 785,
      "warrantyStatus": 1,
      "warrantyStatusText": "Active",
      "purchaseStore": "Dell Direct Online",
      "purchasePrice": 579.99,
      "hasInvoice": true,
      "notes": "Purchased with extended 3-year guarantee."
    }
  ],
  "errors": []
}
```

### 2.2 Get Product By ID
- **Route:** `GET /api/products/{id}`
- **Authorization:** Verified Customer Owner or Admin
- **Response (HTTP 200 OK / 404 Not Found)**

### 2.3 Create Product
- **Route:** `POST /api/products`
- **Request Body:**
```json
{
  "name": "Sony WH-1000XM5 Headphones",
  "brand": "Sony",
  "modelNumber": "WH1000XM5/B",
  "serialNumber": "SN-SONY-778899",
  "category": 1,
  "purchaseDate": "2026-05-15",
  "warrantyDurationMonths": 12,
  "purchaseStore": "Best Buy",
  "purchasePrice": 399.99,
  "notes": "Includes travel case and charging adapter."
}
```
- **Response (HTTP 201 Created):** Returns the newly registered product DTO with calculated `warrantyExpiryDate`.

### 2.4 Update Product
- **Route:** `PUT /api/products/{id}`
- **Request Body:** Full `UpdateProductDto` JSON schema.
- **Response (HTTP 200 OK):** Returns updated product DTO.

### 2.5 Delete Product
- **Route:** `DELETE /api/products/{id}`
- **Response (HTTP 200 OK / 400 Bad Request if active tickets exist)**

---

## 3. Service Requests API Endpoints

### 3.1 Get All Service Requests
- **Route:** `GET /api/servicerequests`
- **Query Parameters:** `status`, `category`, `priority`
- **Response (HTTP 200 OK):** Array of `ServiceRequestDto` items.

### 3.2 Get Service Request By ID
- **Route:** `GET /api/servicerequests/{id}`
- **Response (HTTP 200 OK / 404 Not Found)**

### 3.3 Create Service Request
- **Route:** `POST /api/servicerequests`
- **Request Body:**
```json
{
  "productId": 1,
  "category": 1,
  "priority": 3,
  "subject": "Intermittent USB-C power delivery failure",
  "description": "The monitor does not charge the connected laptop via Thunderbolt port."
}
```
- **Response (HTTP 201 Created):**
```json
{
  "success": true,
  "message": "Ticket created successfully.",
  "data": {
    "id": 1,
    "ticketNumber": "PC-20261116-K7X9",
    "productId": 1,
    "productName": "Dell UltraSharp 27 4K Monitor",
    "category": 1,
    "priority": 3,
    "status": 1,
    "statusText": "Submitted",
    "subject": "Intermittent USB-C power delivery failure",
    "createdAtUtc": "2026-11-16T11:45:00Z"
  }
}
```

### 3.4 Advance Ticket Status (Admin Only)
- **Route:** `PUT /api/servicerequests/{id}/status`
- **Authorization:** `Admin` Role Required
- **Request Body:**
```json
{
  "newStatus": 2,
  "assignedTechnician": "Level 2 Hardware Specialist",
  "remarks": "Issue diagnostic scheduled."
}
```

### 3.5 Cancel Ticket (Customer)
- **Route:** `POST /api/servicerequests/{id}/cancel?reason=Resolved`
- **Rule:** Permitted only while ticket status is `Submitted`.

---

## 4. Postman Integration Guide

1. Open Postman.
2. Select **File &rarr; Import**.
3. Choose file: `postman/ProductCare.postman_collection.json`.
4. Ensure the `baseUrl` collection variable matches your running application URL (e.g. `https://localhost:5001`).
5. Execute **1. Authentication &rarr; Login**. The test script automatically captures the returned JWT token into the `{{authToken}}` collection variable.
6. Run any endpoint under **Products** or **Service Requests**.
