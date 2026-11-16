-- ============================================================================
-- PRODUCTCARE: Stored Procedure Definition
-- Procedure: dbo.sp_GetServiceTicketStatistics
-- Course: INT402: Modern Web Programming Tools and Techniques
-- Purpose: Generates operational ticket volume statistics aggregated by
--          service category, with optional date filtering for reporting.
-- ============================================================================

CREATE OR ALTER PROCEDURE dbo.sp_GetServiceTicketStatistics
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CASE sr.Category
            WHEN 1 THEN 'Repair'
            WHEN 2 THEN 'Installation'
            WHEN 3 THEN 'Maintenance'
            WHEN 4 THEN 'Warranty Claim'
            ELSE 'Other'
        END AS CategoryName,
        COUNT(sr.Id) AS TotalTickets,
        SUM(CASE WHEN sr.Status = 1 THEN 1 ELSE 0 END) AS SubmittedCount,
        SUM(CASE WHEN sr.Status = 2 THEN 1 ELSE 0 END) AS UnderReviewCount,
        SUM(CASE WHEN sr.Status = 3 THEN 1 ELSE 0 END) AS AssignedCount,
        SUM(CASE WHEN sr.Status = 4 THEN 1 ELSE 0 END) AS InProgressCount,
        SUM(CASE WHEN sr.Status = 5 THEN 1 ELSE 0 END) AS WaitingForCustomerCount,
        SUM(CASE WHEN sr.Status = 6 THEN 1 ELSE 0 END) AS ResolvedCount,
        SUM(CASE WHEN sr.Status = 7 THEN 1 ELSE 0 END) AS ClosedCount,
        SUM(CASE WHEN sr.Status = 8 THEN 1 ELSE 0 END) AS RejectedCount
    FROM dbo.ServiceRequests sr
    WHERE (@StartDate IS NULL OR sr.CreatedAtUtc >= @StartDate)
      AND (@EndDate IS NULL OR sr.CreatedAtUtc <= @EndDate)
    GROUP BY sr.Category
    ORDER BY TotalTickets DESC;
END;
GO
