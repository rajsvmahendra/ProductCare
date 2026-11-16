-- ============================================================================
-- PRODUCTCARE: Database View Definition
-- View: dbo.vw_ProductWarrantySummary
-- Course: INT402: Modern Web Programming Tools and Techniques
-- Purpose: Aggregates product inventory with customer details and dynamically
--          computes remaining warranty days and warranty lifecycle status.
-- ============================================================================

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
