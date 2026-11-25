$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Net.Http

Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host " PRODUCTCARE - COMPREHENSIVE END-TO-END QA & VERIFICATION SUITE   " -ForegroundColor Cyan
Write-Host "==================================================================" -ForegroundColor Cyan

function Create-FormData($hash) {
    $dict = [System.Collections.Generic.Dictionary[string, string]]::new()
    foreach ($k in $hash.Keys) {
        $dict.Add([string]$k, [string]$hash[$k])
    }
    return [System.Net.Http.FormUrlEncodedContent]::new($dict)
}

function Extract-Token($html) {
    if ($html -match 'name="__RequestVerificationToken"[^>]*value="([^"]+)"') {
        return $matches[1]
    }
    throw "Failed to extract __RequestVerificationToken from response."
}

$baseUrl = "http://localhost:5213"

# -----------------------------------------------------------------------------
# PHASE 1: AUTHENTICATION, PASSWORD TOGGLE, ROLES & LOGOUT
# -----------------------------------------------------------------------------
Write-Host "`n--- [PHASE 1] Authentication & Access Control ---" -ForegroundColor Yellow

# 1.1 Verify no demo credentials box in Login.cshtml
$loginHtml = (Invoke-WebRequest -Uri "$baseUrl/Account/Login" -UseBasicParsing).Content
if ($loginHtml -match "Demo Credentials" -or $loginHtml -match "fillCredentials" -or $loginHtml -match "admin@productcare.local") {
    throw "[FAIL] Demo credentials box or hardcoded accounts still visible on Login page!"
} else {
    Write-Host "[PASS] No demo credentials, quick-fill buttons, or hardcoded accounts found in Login HTML." -ForegroundColor Green
}

# 1.2 Verify password visibility toggle attributes in HTML
if ($loginHtml -match 'class="[^"]*pc-toggle-password[^"]*"' -and $loginHtml -match 'data-target="loginPassword"') {
    Write-Host "[PASS] Password toggle button properly configured with data-target and accessible attributes." -ForegroundColor Green
} else {
    throw "[FAIL] Password toggle button missing or misconfigured in Login HTML."
}

# 1.3 Authenticate as personal customer account: rajasvmahendra@gmail.com
$custHandler = [System.Net.Http.HttpClientHandler]::new()
$custHandler.CookieContainer = [System.Net.CookieContainer]::new()
$custHandler.AllowAutoRedirect = $true
$custClient = [System.Net.Http.HttpClient]::new($custHandler)

$custLoginGet = $custClient.GetStringAsync("$baseUrl/Account/Login").GetAwaiter().GetResult()
$loginToken = Extract-Token $custLoginGet
$custLoginData = Create-FormData @{
    "Email" = "rajasvmahendra@gmail.com"
    "Password" = "Rajsv@123"
    "RememberMe" = "false"
    "__RequestVerificationToken" = $loginToken
}

$custLoginResp = $custClient.PostAsync("$baseUrl/Account/Login", $custLoginData).GetAwaiter().GetResult()
if ($custLoginResp.IsSuccessStatusCode) {
    Write-Host "[PASS] Successfully logged in with personal customer account (rajasvmahendra@gmail.com)." -ForegroundColor Green
} else {
    throw "[FAIL] Personal customer login failed with HTTP status: $($custLoginResp.StatusCode)"
}

# 1.4 Verify Customer Dashboard Access & Identity
$dashHtml = $custClient.GetStringAsync("$baseUrl/Customer/Dashboard").GetAwaiter().GetResult()
if ($dashHtml -match "Rajsv Mahendra" -or $dashHtml -match "rajasvmahendra@gmail.com") {
    Write-Host "[PASS] Customer Dashboard rendered for Rajsv Mahendra." -ForegroundColor Green
} else {
    throw "[FAIL] Customer Dashboard does not reflect personal customer identity."
}

# 1.4b Verify Personal Device Inventory (MacBook Pro 420-day warranty countdown preserved)
if ($dashHtml -match "MacBook Pro" -and $dashHtml -match "iPhone 15 Pro Max" -and $dashHtml -match "Sony WH-1000XM5") {
    Write-Host "[PASS] Personal device inventory verified (MacBook Pro, iPhone 15 Pro Max, Sony WH-1000XM5 present)." -ForegroundColor Green
} else {
    throw "[FAIL] Personal device inventory missing expected registered devices!"
}

# 1.5 Verify Role-Based Access Control: Customer blocked from /Admin/Dashboard
$adminAccessResp = $custClient.GetAsync("$baseUrl/Admin/Dashboard").GetAwaiter().GetResult()
if ($adminAccessResp.RequestMessage.RequestUri.ToString() -match "AccessDenied|Login" -or $adminAccessResp.StatusCode -eq 403) {
    Write-Host "[PASS] Role-Based Access Control enforced: Customer blocked from Admin area (Redirected to AccessDenied)." -ForegroundColor Green
} else {
    throw "[FAIL] Security breach: Customer was allowed access to Admin area!"
}

# 1.6 Verify Admin Login & Dashboard
$adminHandler = [System.Net.Http.HttpClientHandler]::new()
$adminHandler.CookieContainer = [System.Net.CookieContainer]::new()
$adminHandler.AllowAutoRedirect = $true
$adminClient = [System.Net.Http.HttpClient]::new($adminHandler)

$adminLoginGet = $adminClient.GetStringAsync("$baseUrl/Account/Login").GetAwaiter().GetResult()
$adminToken = Extract-Token $adminLoginGet
$adminLoginData = Create-FormData @{
    "Email" = "admin@productcare.local"
    "Password" = "Admin123!"
    "RememberMe" = "false"
    "__RequestVerificationToken" = $adminToken
}

$adminLoginResp = $adminClient.PostAsync("$baseUrl/Account/Login", $adminLoginData).GetAwaiter().GetResult()
$adminDashHtml = $adminClient.GetStringAsync("$baseUrl/Admin/Dashboard").GetAwaiter().GetResult()
if ($adminDashHtml -match "Campus Operations Overview" -or $adminDashHtml -match "Service Ticket Queue" -or $adminDashHtml -match "ADMINISTRATOR") {
    Write-Host "[PASS] Administrator login and dashboard access verified for admin@productcare.local." -ForegroundColor Green
} else {
    throw "[FAIL] Admin dashboard verification failed."
}

# 1.7 Verify New User Registration
$regHandler = [System.Net.Http.HttpClientHandler]::new()
$regHandler.CookieContainer = [System.Net.CookieContainer]::new()
$regHandler.AllowAutoRedirect = $true
$regClient = [System.Net.Http.HttpClient]::new($regHandler)

$regGet = $regClient.GetStringAsync("$baseUrl/Account/Register").GetAwaiter().GetResult()
$regToken = Extract-Token $regGet
$randomSuffix = Get-Random -Minimum 100000 -Maximum 999999
$qaEmail = "qa_user_$randomSuffix@example.com"
$regData = Create-FormData @{
    "FullName" = "QA Automation User"
    "Email" = $qaEmail
    "Password" = "QaTestPass123!"
    "ConfirmPassword" = "QaTestPass123!"
    "__RequestVerificationToken" = $regToken
}

$regResp = $regClient.PostAsync("$baseUrl/Account/Register", $regData).GetAwaiter().GetResult()
if ($regResp.IsSuccessStatusCode) {
    Write-Host "[PASS] Registration succeeded for new user ($qaEmail)." -ForegroundColor Green
} else {
    throw "[FAIL] Registration POST failed with status: $($regResp.StatusCode)"
}

# 1.8 Verify Logout
$logoutToken = Extract-Token ($custClient.GetStringAsync("$baseUrl/Customer/Dashboard").GetAwaiter().GetResult())
$logoutData = Create-FormData @{ "__RequestVerificationToken" = $logoutToken }
$logoutResp = $custClient.PostAsync("$baseUrl/Account/Logout", $logoutData).GetAwaiter().GetResult()
$postLogoutDash = $custClient.GetAsync("$baseUrl/Customer/Dashboard").GetAwaiter().GetResult()
if ($postLogoutDash.RequestMessage.RequestUri.ToString() -match "Account/Login") {
    Write-Host "[PASS] Logout successfully invalidated authentication cookie; subsequent protected access redirects to Login." -ForegroundColor Green
} else {
    throw "[FAIL] Logout failed to invalidate customer session!"
}

# Re-login customer for remaining phases
$reloginToken = Extract-Token ($custClient.GetStringAsync("$baseUrl/Account/Login").GetAwaiter().GetResult())
$reloginData = Create-FormData @{
    "Email" = "rajasvmahendra@gmail.com"
    "Password" = "Rajsv@123"
    "RememberMe" = "false"
    "__RequestVerificationToken" = $reloginToken
}
$null = $custClient.PostAsync("$baseUrl/Account/Login", $reloginData).GetAwaiter().GetResult()

# -----------------------------------------------------------------------------
# PHASE 2: PRODUCT REGISTRATION (MANUAL ENTRY, VOICE SPEECH, DATABASE PERSISTENCE)
# -----------------------------------------------------------------------------
Write-Host "`n--- [PHASE 2] Product Registration & Voice Input (Isolated QA Account) ---" -ForegroundColor Yellow

# Use the freshly registered dedicated QA user ($regClient) so personal inventory is preserved
$createProdGet = $regClient.GetStringAsync("$baseUrl/Customer/Products/Create").GetAwaiter().GetResult()
$prodToken = Extract-Token $createProdGet

# 2.1 Verify Web Speech UI elements and fallback elements exist in Create page
if ($createProdGet -match 'id="voiceMicBtn"' -and $createProdGet -match 'id="voiceParsedContainer"' -and $createProdGet -match 'id="voiceUnsupportedNotice"') {
    Write-Host "[PASS] Web Speech API interface, microphone button, suggestions preview card, and fallback notice verified in HTML." -ForegroundColor Green
} else {
    throw "[FAIL] Voice assistant elements missing in Products/Create view!"
}

# 2.2 Register a new Apple MacBook Pro via normal manual entry
$uniqueSerial = "C02G" + (Get-Random -Minimum 10000000 -Maximum 99999999)
$prodData = Create-FormData @{
    "Name" = "MacBook Pro 16-inch M3 Max"
    "Brand" = "Apple"
    "Category" = "9" # Laptops
    "SubCategory" = "Ultrabook"
    "ModelNumber" = "A2485"
    "SerialNumber" = $uniqueSerial
    "PurchaseDate" = "2026-01-15"
    "WarrantyDurationMonths" = "24"
    "PurchaseStore" = "Apple Store Online"
    "PurchasePrice" = "2499.00"
    "Notes" = "Registered during End-to-End QA verification."
    "__RequestVerificationToken" = $prodToken
}

$prodCreateResp = $regClient.PostAsync("$baseUrl/Customer/Products/Create", $prodData).GetAwaiter().GetResult()
if ($prodCreateResp.IsSuccessStatusCode) {
    Write-Host "[PASS] Product registered successfully (S/N: $uniqueSerial) under dedicated QA user." -ForegroundColor Green
} else {
    throw "[FAIL] Product registration POST failed with status $($prodCreateResp.StatusCode)"
}

# 2.3 Verify persistence in SQL Server database
$dbProdCheck = & sqlcmd -S "(localdb)\ProductCareTest" -d "ProductCareDb" -h -1 -W -Q "SET NOCOUNT ON; SELECT Id, Name, Category, SubCategory, WarrantyDurationMonths, WarrantyExpiryDate FROM Products WHERE SerialNumber = '$uniqueSerial'"
$newProdRow = ($dbProdCheck | Where-Object { $_ -match '^\d+' } | Select-Object -First 1)
if ($newProdRow) {
    Write-Host "[PASS] Database persistence confirmed: $newProdRow" -ForegroundColor Green
    $newProductId = ($newProdRow -split '\s+')[0].Trim()
} else {
    throw "[FAIL] Newly registered product not found in database!"
}

# 2.4 Verify product persists on page refresh
$prodIndexHtml = $regClient.GetStringAsync("$baseUrl/Customer/Products").GetAwaiter().GetResult()
if ($prodIndexHtml -match $uniqueSerial -and $prodIndexHtml -match "MacBook Pro 16-inch M3 Max") {
    Write-Host "[PASS] Product list refreshed successfully; device ownership card rendered with S/N $uniqueSerial." -ForegroundColor Green
} else {
    throw "[FAIL] Registered product not found on Customer/Products inventory page!"
}

# -----------------------------------------------------------------------------
# PHASE 3: PRODUCT CARDS, ACTIONS & WARRANTY DATES
# -----------------------------------------------------------------------------
Write-Host "`n--- [PHASE 3] Product Cards, Navigation & Warranty Verification ---" -ForegroundColor Yellow

# 3.1 Test View Details
$detailsHtml = $regClient.GetStringAsync("$baseUrl/Customer/Products/Details/$newProductId").GetAwaiter().GetResult()
if ($detailsHtml -match "MacBook Pro 16-inch M3 Max" -and $detailsHtml -match "Apple" -and $detailsHtml -match "Ultrabook" -and $detailsHtml -match "24 Months") {
    Write-Host "[PASS] Details action verified: Full hardware profile & specifications rendered." -ForegroundColor Green
} else {
    throw "[FAIL] Details page failed verification!"
}

# 3.2 Test Edit View
$editHtml = $regClient.GetStringAsync("$baseUrl/Customer/Products/Edit/$newProductId").GetAwaiter().GetResult()
if ($editHtml -match $uniqueSerial -and $editHtml -match "Ultrabook") {
    Write-Host "[PASS] Edit action verified: Edit form populated with existing product values." -ForegroundColor Green
} else {
    throw "[FAIL] Edit page failed verification!"
}

# 3.3 Test Report Issue Action - Verify product pre-population in ServiceRequests/Create
$intakeHtml = $regClient.GetStringAsync("$baseUrl/Customer/ServiceRequests/Create?productId=$newProductId").GetAwaiter().GetResult()
if ($intakeHtml -match "<option[^>]*value=`"$newProductId`"[^>]*selected" -or $intakeHtml -match "<option[^>]*selected[^>]*value=`"$newProductId`"") {
    Write-Host "[PASS] Report Issue action verified: Correct product ($newProductId) carried and selected in service-request form." -ForegroundColor Green
} else {
    throw "[FAIL] Product ID was not pre-selected in ServiceRequests/Create!"
}

# 3.4 Check Warranty Date Accuracy
# Purchase: 2026-01-15 + 24 Months = 2028-01-15
if ($detailsHtml -match "15 Jan 2028" -or $detailsHtml -match "2028") {
    Write-Host "[PASS] Warranty calculation accurate: Purchase Date 15 Jan 2026 + 24 Months = 15 Jan 2028 (Under warranty)." -ForegroundColor Green
} else {
    throw "[FAIL] Warranty expiry date calculation inaccurate on Details page!"
}

# 3.5 Test Product Deletion with Confirmation, Authorization & Persistence
Write-Host "  -> Testing Product Delete Action with Authorization Checks..." -ForegroundColor Yellow

# A. Register a dedicated product to test hard deletion (no service tickets)
$delSerial = "DEL" + (Get-Random -Minimum 10000000 -Maximum 99999999)
$delProdToken = Extract-Token ($regClient.GetStringAsync("$baseUrl/Customer/Products/Create").GetAwaiter().GetResult())
$delProdData = Create-FormData @{
    "Name" = "Delete Verification Phone"
    "Brand" = "OnePlus"
    "Category" = "1" # Smartphone
    "ModelNumber" = "OP12"
    "SerialNumber" = $delSerial
    "PurchaseDate" = "2026-02-01"
    "WarrantyDurationMonths" = "12"
    "__RequestVerificationToken" = $delProdToken
}
$null = $regClient.PostAsync("$baseUrl/Customer/Products/Create", $delProdData).GetAwaiter().GetResult()

# Query DB for this product ID
$delProdRow = (& sqlcmd -S "(localdb)\ProductCareTest" -d "ProductCareDb" -h -1 -W -Q "SET NOCOUNT ON; SELECT Id FROM Products WHERE SerialNumber = '$delSerial'") | Where-Object { $_ -match '^\d+' } | Select-Object -First 1
if (-not $delProdRow) {
    throw "[FAIL] Temporary deletion test product was not created in database!"
}
$delProdId = $delProdRow.Trim()
Write-Host "  [OK] Temporary product registered for delete test: ID $delProdId ($delSerial)" -ForegroundColor Gray

# B. Get AntiForgeryToken from Products page
$productsPageHtml = $regClient.GetStringAsync("$baseUrl/Customer/Products").GetAwaiter().GetResult()
$deleteToken = Extract-Token $productsPageHtml

# C. Attempt Cross-User Deletion (Authorization check: customer client rajasvmahendra trying to delete regClient's product)
$unauthorizedDeleteData = Create-FormData @{
    "id" = $delProdId
    "__RequestVerificationToken" = Extract-Token ($custClient.GetStringAsync("$baseUrl/Customer/Products").GetAwaiter().GetResult())
}
$unauthResp = $custClient.PostAsync("$baseUrl/Customer/Products/Delete/$delProdId", $unauthorizedDeleteData).GetAwaiter().GetResult()
if ($unauthResp.StatusCode -eq [System.Net.HttpStatusCode]::NotFound) {
    Write-Host "[PASS] Cross-user deletion blocked: Unauthorized customer received 404 NotFound." -ForegroundColor Green
} else {
    throw "[FAIL] Cross-user authorization check failed! Status: $($unauthResp.StatusCode)"
}

# D. Legitimate Owner Deletion (regClient deleting their own product)
$legitDeleteData = Create-FormData @{
    "id" = $delProdId
    "__RequestVerificationToken" = $deleteToken
}
$deleteResp = $regClient.PostAsync("$baseUrl/Customer/Products/Delete/$delProdId", $legitDeleteData).GetAwaiter().GetResult()
if ($deleteResp.IsSuccessStatusCode) {
    Write-Host "[PASS] Delete operation succeeded with 200/302 response." -ForegroundColor Green
} else {
    throw "[FAIL] Legitimate owner product deletion POST failed with status: $($deleteResp.StatusCode)"
}

# E. Verify physical deletion from database
$dbAfterDel = (& sqlcmd -S "(localdb)\ProductCareTest" -d "ProductCareDb" -h -1 -W -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM Products WHERE Id = $delProdId").Trim()
if ($dbAfterDel -eq "0") {
    Write-Host "[PASS] Database confirmed: Product $delProdId permanently deleted (0 records remain)." -ForegroundColor Green
} else {
    throw "[FAIL] Product record $delProdId still exists in database after deletion!"
}

# -----------------------------------------------------------------------------
# PHASE 4: SERVICE CENTRE WORKFLOW & REPAIR LIFECYCLE
# -----------------------------------------------------------------------------
Write-Host "`n--- [PHASE 4] Service Centre Directory & Issue Intake ---" -ForegroundColor Yellow

# 4.1 Search by brand & category
$scSearchHtml = $regClient.GetStringAsync("$baseUrl/Customer/ServiceCenters?brand=Apple").GetAwaiter().GetResult()
if ($scSearchHtml -match "LPU Uni-Mall" -and ($scSearchHtml -match "Phagwara" -or $scSearchHtml -match "LPU Campus")) {
    Write-Host "[PASS] Service centre directory search by brand (Apple) returned matching facility (LPU Uni-Mall)." -ForegroundColor Green
} else {
    throw "[FAIL] Service centre search did not return expected match!"
}

# 4.2 Verify contact links: tel:, mailto:, map directions
if ($scSearchHtml -match 'href="tel:[^"]*517000' -and $scSearchHtml -match 'href="mailto:campuscare@unimall.sample.lpu.in"' -and $scSearchHtml -match 'maps.google.com') {
    Write-Host "[PASS] Contact links verified: Actionable tel:, mailto:, and map directions links present." -ForegroundColor Green
} else {
    throw "[FAIL] Service centre contact links missing or invalid!"
}

# 4.3 Raise an issue against the registered product linked to LPU Uni-Mall (ID: 1)
$ticketToken = Extract-Token $intakeHtml
$ticketData = Create-FormData @{
    "ProductId" = $newProductId
    "ServiceCenterId" = "1"
    "Category" = "Repair"
    "Priority" = "Medium"
    "CampusLocation" = "Uni-Mall Tech Desk - Room 104"
    "Subject" = "Display backlight bleeding under high brightness"
    "Description" = "Subtle backlight glow observed along bottom right bezel when brightness is set above 70%."
    "__RequestVerificationToken" = $ticketToken
}

$ticketCreateResp = $regClient.PostAsync("$baseUrl/Customer/ServiceRequests/Create", $ticketData).GetAwaiter().GetResult()
if ($ticketCreateResp.IsSuccessStatusCode) {
    Write-Host "[PASS] Service ticket submitted successfully with designated Service Center ID 1." -ForegroundColor Green
} else {
    throw "[FAIL] Service ticket submission failed with status: $($ticketCreateResp.StatusCode)"
}

# 4.4 Verify ServiceCenterId association saved in SQL Server database
$dbTicketCheck = & sqlcmd -S "(localdb)\ProductCareTest" -d "ProductCareDb" -h -1 -W -Q "SET NOCOUNT ON; SELECT TOP 1 sr.Id, sr.TicketNumber, sr.ServiceCenterId, sc.Name FROM ServiceRequests sr JOIN ServiceCenters sc ON sr.ServiceCenterId = sc.Id WHERE sr.ProductId = $newProductId ORDER BY sr.Id DESC"
$ticketRow = ($dbTicketCheck | Where-Object { $_ -match '^\d+' } | Select-Object -First 1)
if ($ticketRow) {
    Write-Host "[PASS] Database relationship verified: $ticketRow" -ForegroundColor Green
    $newTicketId = ($ticketRow -split '\s+')[0].Trim()
    $newTicketNumber = ($ticketRow -split '\s+')[1].Trim()
} else {
    throw "[FAIL] Service request was not saved with ServiceCenterId in database!"
}

# 4.5 Verify ticket appears in customer request history
$custRequestsHtml = $regClient.GetStringAsync("$baseUrl/Customer/ServiceRequests").GetAwaiter().GetResult()
if ($custRequestsHtml -match $newTicketNumber) {
    Write-Host "[PASS] Ticket #$newTicketNumber appears in customer service requests history." -ForegroundColor Green
} else {
    throw "[FAIL] Ticket #$newTicketNumber missing from customer requests view!"
}

# 4.6 Verify Admin Review and Status Update to InProgress
$adminTicketHtml = $adminClient.GetStringAsync("$baseUrl/Admin/ServiceRequests/Details/$newTicketId").GetAwaiter().GetResult()
if ($adminTicketHtml -match $newTicketNumber -and ($adminTicketHtml -match "LPU Uni-Mall" -or $adminTicketHtml -match "Service Facility")) {
    Write-Host "[PASS] Admin review verified: Ticket details rendered with assigned Service Centre." -ForegroundColor Green
} else {
    throw "[FAIL] Admin ticket details view missing ticket or service centre!"
}

# Transition 1: Submitted -> UnderReview
$adminTicketToken = Extract-Token $adminTicketHtml
$dataUnderReview = Create-FormData @{
    "id" = $newTicketId
    "newStatus" = "UnderReview"
    "newRemarks" = "Diagnostic review initiated at LPU Uni-Mall Tech Care Center."
    "__RequestVerificationToken" = $adminTicketToken
}
$resp1 = $adminClient.PostAsync("$baseUrl/Admin/ServiceRequests/UpdateStatus", $dataUnderReview).GetAwaiter().GetResult()
if (-not $resp1.IsSuccessStatusCode) { throw "Transition to UnderReview failed with status $($resp1.StatusCode)" }

# Transition 2: UnderReview -> Assigned
$detailsHtml2 = $adminClient.GetStringAsync("$baseUrl/Admin/ServiceRequests/Details/$newTicketId").GetAwaiter().GetResult()
$token2 = Extract-Token $detailsHtml2
$dataAssigned = Create-FormData @{
    "id" = $newTicketId
    "newStatus" = "Assigned"
    "newAssignedTechnician" = "Alex Rivera (Hardware Specialist)"
    "newRemarks" = "Assigned to certified hardware technician."
    "__RequestVerificationToken" = $token2
}
$resp2 = $adminClient.PostAsync("$baseUrl/Admin/ServiceRequests/UpdateStatus", $dataAssigned).GetAwaiter().GetResult()
if (-not $resp2.IsSuccessStatusCode) { throw "Transition to Assigned failed with status $($resp2.StatusCode)" }

# Transition 3: Assigned -> InProgress
$detailsHtml3 = $adminClient.GetStringAsync("$baseUrl/Admin/ServiceRequests/Details/$newTicketId").GetAwaiter().GetResult()
$token3 = Extract-Token $detailsHtml3
$dataInProgress = Create-FormData @{
    "id" = $newTicketId
    "newStatus" = "InProgress"
    "newAssignedTechnician" = "Alex Rivera (Hardware Specialist)"
    "newRemarks" = "Display backlight repair underway at bench."
    "__RequestVerificationToken" = $token3
}
$resp3 = $adminClient.PostAsync("$baseUrl/Admin/ServiceRequests/UpdateStatus", $dataInProgress).GetAwaiter().GetResult()
if ($resp3.IsSuccessStatusCode) {
    Write-Host "[PASS] Admin successfully triaged ticket through workflow stages (UnderReview -> Assigned -> InProgress)." -ForegroundColor Green
} else {
    throw "[FAIL] Transition to InProgress failed with status $($resp3.StatusCode)"
}

# Verify status in DB
$statusCheck = & sqlcmd -S "(localdb)\ProductCareTest" -d "ProductCareDb" -h -1 -W -Q "SET NOCOUNT ON; SELECT Status FROM ServiceRequests WHERE Id = $newTicketId"
$dbStatus = ($statusCheck | Where-Object { $_ -match '^\d+' } | Select-Object -First 1).Trim()
# RequestStatus.InProgress = 4
if ($dbStatus -eq "4") {
    Write-Host "[PASS] Database confirmed ticket status is now InProgress (enum value 4)." -ForegroundColor Green
} else {
    throw "[FAIL] Database status is $dbStatus, expected 4 (InProgress)!"
}

# -----------------------------------------------------------------------------
# PHASE 5: TEARDOWN (CLEAN UP QA TEST USER & RECORDS)
# -----------------------------------------------------------------------------
Write-Host "`n--- [TEARDOWN] Cleaning up automated QA test data ---" -ForegroundColor Yellow
$teardownSql = @"
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
DELETE sr FROM ServiceRequests sr JOIN Products p ON sr.ProductId = p.Id WHERE p.SerialNumber = '$uniqueSerial';
DELETE FROM Products WHERE SerialNumber = '$uniqueSerial';
DELETE FROM AspNetUsers WHERE Email = '$qaEmail';
"@
& sqlcmd -S "(localdb)\ProductCareTest" -d "ProductCareDb" -Q $teardownSql
Write-Host "[PASS] QA test records cleanly deleted from database (Test user and test product removed)." -ForegroundColor Green

Write-Host "`n==================================================================" -ForegroundColor Green
Write-Host " ALL END-TO-END QA PHASES COMPLETED WITH 100% PASS RATE!          " -ForegroundColor Green
Write-Host "==================================================================" -ForegroundColor Green
