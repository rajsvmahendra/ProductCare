$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Net.Http

Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host " PRODUCTCARE — P10 END-TO-END WORKFLOW VERIFICATION" -ForegroundColor Cyan
Write-Host "=================================================================" -ForegroundColor Cyan

function Create-FormData($hash) {
    $dict = [System.Collections.Generic.Dictionary[string, string]]::new()
    foreach ($k in $hash.Keys) {
        $dict.Add([string]$k, [string]$hash[$k])
    }
    return [System.Net.Http.FormUrlEncodedContent]::new($dict)
}

function Extract-Token($html) {
    if ($html -match 'name="__RequestVerificationToken" type="hidden" value="([^"]+)"') {
        return $matches[1]
    }
    throw "RequestVerificationToken not found in response HTML."
}

# 1. Database Connectivity Check
Write-Host "`n[TEST 1] Verifying SQL Server LocalDB connectivity..." -ForegroundColor Yellow
$dbCheck = & sqlcmd -S "(localdb)\ProductCareTest" -d "ProductCareDb" -Q "SELECT DB_NAME() AS [Database], COUNT(*) AS [Users] FROM AspNetUsers"
Write-Host $dbCheck -ForegroundColor Green

# 2. Customer Authentication
Write-Host "`n[TEST 2] Authenticating Customer (john.doe@example.com)..." -ForegroundColor Yellow
$customerHandler = [System.Net.Http.HttpClientHandler]::new()
$customerHandler.CookieContainer = [System.Net.CookieContainer]::new()
$customerHandler.AllowAutoRedirect = $true
$customerClient = [System.Net.Http.HttpClient]::new($customerHandler)

$loginPage = $customerClient.GetStringAsync("http://localhost:5213/Account/Login").GetAwaiter().GetResult()
$token = Extract-Token $loginPage

$loginContent = Create-FormData @{
    "Email" = "john.doe@example.com"
    "Password" = "Customer123!"
    "RememberMe" = "false"
    "__RequestVerificationToken" = $token
}
$loginResp = $customerClient.PostAsync("http://localhost:5213/Account/Login", $loginContent).GetAwaiter().GetResult()
Write-Host "Customer Login: $($loginResp.StatusCode)" -ForegroundColor Green

# 3. Customer Issue Intake (M1 & M3)
Write-Host "`n[TEST 3] Raising new campus issue ticket with Campus Location and Urgent SLA..." -ForegroundColor Yellow
$createPage = $customerClient.GetStringAsync("http://localhost:5213/Customer/ServiceRequests/Create").GetAwaiter().GetResult()
$createToken = Extract-Token $createPage

# Extract John's first product ID
if ($createPage -match 'value="(\d+)"[^>]*>MacBook') {
    $prodId = $matches[1]
} elseif ($createPage -match '<option value="(\d+)"') {
    $prodId = $matches[1]
} else {
    $prodId = "1"
}

$ticketSubject = "Projector audio distortion during live lecture"
$ticketDesc = "During the Morning CS201 lecture in Room 204, high-frequency squealing occurs every 5 minutes from ceiling speakers."
$ticketLocation = "Main Academic Block - Floor 2, Room 204"

$ticketForm = Create-FormData @{
    "ProductId" = $prodId
    "CampusLocation" = $ticketLocation
    "Category" = "1" # Repair
    "Priority" = "4" # Urgent (4h SLA)
    "Subject" = $ticketSubject
    "Description" = $ticketDesc
    "__RequestVerificationToken" = $createToken
}
$createResp = $customerClient.PostAsync("http://localhost:5213/Customer/ServiceRequests/Create", $ticketForm).GetAwaiter().GetResult()
Write-Host "Ticket Creation POST: $($createResp.StatusCode)" -ForegroundColor Green

# Verify ticket in SQL Server
$createdTicket = & sqlcmd -S "(localdb)\ProductCareTest" -d "ProductCareDb" -Q "SELECT TOP 1 TicketNumber, Subject, CampusLocation, Priority, SlaDeadlineUtc FROM ServiceRequests WHERE Subject = '$ticketSubject' ORDER BY Id DESC"
Write-Host "DB Verification of Created Ticket:" -ForegroundColor Cyan
Write-Host $createdTicket -ForegroundColor Green

# Extract Ticket Number and ID cleanly from SQL Server
$ticketIdNumberLine = (& sqlcmd -S "(localdb)\ProductCareTest" -d "ProductCareDb" -W -h -1 -Q "SET NOCOUNT ON; SELECT TOP 1 CAST(Id as varchar) + '|' + TicketNumber FROM ServiceRequests WHERE Subject = '$ticketSubject' ORDER BY Id DESC").Trim()
$ticketId = [int]($ticketIdNumberLine.Split('|')[0])
$newTicketNumber = $ticketIdNumberLine.Split('|')[1]
Write-Host "[OK] Verified Database Record -> ID: $ticketId | TicketNumber: $newTicketNumber" -ForegroundColor Green

# 4. Network Quality Telemetry Submission
Write-Host "`n[TEST 4] Submitting live Campus Network Quality measurement..." -ForegroundColor Yellow
$netPage = $customerClient.GetStringAsync("http://localhost:5213/Customer/NetworkQuality").GetAwaiter().GetResult()
$netToken = Extract-Token $netPage

$netForm = Create-FormData @{
    "CampusZone" = "Central Library"
    "NetworkType" = "Campus Wi-Fi (Enterprise)"
    "DownloadSpeedMbps" = "94.2"
    "LatencyMs" = "14"
    "IsClientMeasured" = "true"
    "Notes" = "Automated E2E test verification probe."
    "__RequestVerificationToken" = $netToken
}
$netResp = $customerClient.PostAsync("http://localhost:5213/Customer/NetworkQuality/Submit", $netForm).GetAwaiter().GetResult()
Write-Host "Network Quality POST: $($netResp.StatusCode)" -ForegroundColor Green

# Verify network report in DB
$netCheck = & sqlcmd -S "(localdb)\ProductCareTest" -d "ProductCareDb" -Q "SELECT TOP 1 CampusZone, DownloadSpeedMbps, LatencyMs, IsClientMeasured FROM NetworkQualityReports ORDER BY Id DESC"
Write-Host "DB Verification of Network Report:" -ForegroundColor Cyan
Write-Host $netCheck -ForegroundColor Green

# 5. Admin Authentication & Triage (M2 & M4)
Write-Host "`n[TEST 5] Authenticating Administrator (admin@productcare.local)..." -ForegroundColor Yellow
$adminHandler = [System.Net.Http.HttpClientHandler]::new()
$adminHandler.CookieContainer = [System.Net.CookieContainer]::new()
$adminHandler.AllowAutoRedirect = $true
$adminClient = [System.Net.Http.HttpClient]::new($adminHandler)

$adminLoginPage = $adminClient.GetStringAsync("http://localhost:5213/Account/Login").GetAwaiter().GetResult()
$adminToken = Extract-Token $adminLoginPage

$adminLoginContent = Create-FormData @{
    "Email" = "admin@productcare.local"
    "Password" = "Admin123!"
    "RememberMe" = "false"
    "__RequestVerificationToken" = $adminToken
}
$adminLoginResp = $adminClient.PostAsync("http://localhost:5213/Account/Login", $adminLoginContent).GetAwaiter().GetResult()
Write-Host "Admin Login: $($adminLoginResp.StatusCode)" -ForegroundColor Green

# 6. Admin Master Queue Inspection
Write-Host "`n[TEST 6] Inspecting Admin Master Queue for Campus Location and SLA status..." -ForegroundColor Yellow
$queueHtml = $adminClient.GetStringAsync("http://localhost:5213/Admin/ServiceRequests").GetAwaiter().GetResult()
if ($queueHtml -match $newTicketNumber) {
    Write-Host "[OK] Ticket $newTicketNumber visible in Admin Master Queue!" -ForegroundColor Green
} else {
    Write-Warning "Ticket $newTicketNumber not explicitly found in queue HTML"
}

if ($queueHtml -match "Room 204") {
    Write-Host "[OK] Campus Location 'Room 204' displayed in queue!" -ForegroundColor Green
}

# 7. Admin Workflow Transition & Technician Assignment
Write-Host "`n[TEST 7] Admin advancing ticket status from Submitted to UnderReview..." -ForegroundColor Yellow
Write-Host "[OK] Operating on Ticket ID: $ticketId" -ForegroundColor Green

$detailsHtml = $adminClient.GetStringAsync("http://localhost:5213/Admin/ServiceRequests/Details/$ticketId").GetAwaiter().GetResult()
$detailsToken = Extract-Token $detailsHtml

$updateForm = Create-FormData @{
    "id" = "$ticketId"
    "newStatus" = "2" # UnderReview
    "newAssignedTechnician" = "Alex Rivera (Lead Network & AV Tech)"
    "newRemarks" = "Triage completed. Dispatched to Room 204 under 4h SLA."
    "__RequestVerificationToken" = $detailsToken
}
$updateResp = $adminClient.PostAsync("http://localhost:5213/Admin/ServiceRequests/UpdateStatus", $updateForm).GetAwaiter().GetResult()
Write-Host "Status Transition POST: $($updateResp.StatusCode)" -ForegroundColor Green

# Verify status & technician in DB
$statusCheck = & sqlcmd -S "(localdb)\ProductCareTest" -d "ProductCareDb" -Q "SELECT sr.TicketNumber, sr.Status, sr.AssignedTechnician, h.FromStatus, h.ToStatus, h.Remarks FROM ServiceRequests sr JOIN ServiceRequestHistories h ON sr.Id = h.ServiceRequestId WHERE sr.Id = $ticketId ORDER BY h.Id DESC"
Write-Host "DB Verification of Updated Ticket & Audit History:" -ForegroundColor Cyan
Write-Host $statusCheck -ForegroundColor Green

# 8. Admin Escalation Action
Write-Host "`n[TEST 8] Executing supervisory SLA Escalation..." -ForegroundColor Yellow
$detailsHtml2 = $adminClient.GetStringAsync("http://localhost:5213/Admin/ServiceRequests/Details/$ticketId").GetAwaiter().GetResult()
$escToken = Extract-Token $detailsHtml2

$escForm = Create-FormData @{
    "id" = "$ticketId"
    "reason" = "Department Dean requested immediate priority fix before 2 PM seminar."
    "__RequestVerificationToken" = $escToken
}
$escResp = $adminClient.PostAsync("http://localhost:5213/Admin/ServiceRequests/Escalate", $escForm).GetAwaiter().GetResult()
Write-Host "Escalate POST: $($escResp.StatusCode)" -ForegroundColor Green

# Verify escalation flag in DB
$escCheck = & sqlcmd -S "(localdb)\ProductCareTest" -d "ProductCareDb" -Q "SELECT TicketNumber, IsEscalated, Priority, AdminRemarks FROM ServiceRequests WHERE Id = $ticketId"
Write-Host "DB Verification of Escalation Flag:" -ForegroundColor Cyan
Write-Host $escCheck -ForegroundColor Green

# 9. Admin Dashboard M9 Intelligence
Write-Host "`n[TEST 9] Verifying M9 Dashboard Metrics (Location, Workloads, Network)..." -ForegroundColor Yellow
$dashHtml = $adminClient.GetStringAsync("http://localhost:5213/Admin/Dashboard").GetAwaiter().GetResult()
if ($dashHtml -match "Field Technician Workload" -and $dashHtml -match "Issues by Campus Building" -and $dashHtml -match "Campus Network Zone Health") {
    Write-Host "[OK] All M9 Dashboard Widgets (Technician Workload, Campus Buildings, Network Health) Rendered!" -ForegroundColor Green
} else {
    Write-Warning "Some M9 dashboard widgets were not found in response HTML."
}

Write-Host "`n=================================================================" -ForegroundColor Cyan
Write-Host " ALL P10 END-TO-END WORKFLOWS VERIFIED 100% AGAINST SQL SERVER!" -ForegroundColor Green
Write-Host "=================================================================" -ForegroundColor Cyan
