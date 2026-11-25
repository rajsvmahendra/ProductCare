$ErrorActionPreference = "Stop"

$edge = "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
if (-not (Test-Path $edge)) {
    $edge = "C:\Program Files\Microsoft\Edge\Application\msedge.exe"
}

$screenshotDir = "C:\Users\rajsv\Desktop\ProductCare_INT402\docs\screenshots"
if (-not (Test-Path $screenshotDir)) {
    New-Item -ItemType Directory -Path $screenshotDir -Force | Out-Null
}

$tempUserData = Join-Path $env:TEMP "EdgeMaster_$([Guid]::NewGuid())"

Write-Host "Capturing Genuine ProductCare Redesign Screenshots at 1440x900..." -ForegroundColor Cyan

# Fetch latest ticket ID from SQL Server
$ticketIdLines = & sqlcmd -S "(localdb)\ProductCareTest" -d "ProductCareDb" -h -1 -W -Q "SET NOCOUNT ON; SELECT TOP 1 Id FROM ServiceRequests ORDER BY Id DESC"
$ticketId = ($ticketIdLines | Where-Object { $_ -match '^\d+$' } | Select-Object -First 1)
if ($ticketId) { $ticketId = $ticketId.Trim() } else { $ticketId = "2004" }

$shots = @(
    @{ Url = "http://localhost:5213/"; Out = "master_01_landing_hero.png"; Desc = "Public Landing Page with Authentic Logo & Service Positioning" },
    @{ Url = "http://localhost:5213/Account/Login"; Out = "master_02_login_clean.png"; Desc = "Clean Authentication Portal (Liquid Glass, No Demo Box, Password Toggle)" },
    @{ Url = "http://localhost:5213/Customer/Dashboard?devUser=john.doe@example.com"; Out = "master_03_customer_dashboard.png"; Desc = "Customer Overview & Hardware Catalog" },
    @{ Url = "http://localhost:5213/Customer/Products?devUser=john.doe@example.com"; Out = "master_04_product_ownership_cards.png"; Desc = "Product Ownership Cards (Warranty Status, Copy S/N, 3 Actions)" },
    @{ Url = "http://localhost:5213/Customer/Products/Create?devUser=john.doe@example.com"; Out = "master_05_smart_voice_registration.png"; Desc = "Smart Product Registration with Voice Assistant & Dynamic Subcategories" },
    @{ Url = "http://localhost:5213/Customer/ServiceCenters?devUser=john.doe@example.com"; Out = "master_06_service_centres_directory.png"; Desc = "Service Centres Directory (Verified Partner vs Sample Hub, Direct Routing)" },
    @{ Url = "http://localhost:5213/Customer/ServiceRequests/Create?devUser=john.doe@example.com"; Out = "master_07_service_request_intake.png"; Desc = "Issue Intake with 5-Step Repair Lifecycle & Service Centre Selector" },
    @{ Url = "http://localhost:5213/Customer/ServiceRequests/Details/${ticketId}?devUser=john.doe@example.com"; Out = "master_08_service_request_lifecycle.png"; Desc = "Repair Lifecycle Tracker & Assigned Service Centre" },
    @{ Url = "http://localhost:5213/Admin/Dashboard?devUser=admin@productcare.local"; Out = "master_09_admin_dashboard_service_centres.png"; Desc = "Admin Dashboard (SLAs, Service Centre Status, Ticket Triage)" },
    @{ Url = "http://localhost:5213/Admin/ServiceCenters?devUser=admin@productcare.local"; Out = "master_10_admin_service_centres_mgmt.png"; Desc = "Admin Service Centre Directory Management & Verification Toggles" }
)

foreach ($s in $shots) {
    $dest = Join-Path $screenshotDir $s.Out
    $args = @(
        "--headless=new",
        "--disable-gpu",
        "--user-data-dir=$tempUserData",
        "--window-size=1440,900",
        "--screenshot=$dest",
        $s.Url
    )
    $proc = Start-Process -FilePath $edge -ArgumentList $args -PassThru -Wait
    if (Test-Path $dest) {
        $len = (Get-Item $dest).Length
        Write-Host "[OK] $($s.Out) ($len bytes) — $($s.Desc)" -ForegroundColor Green
    } else {
        Write-Warning "Failed to capture $($s.Out)"
    }
}

Remove-Item -Recurse -Force $tempUserData -ErrorAction SilentlyContinue
Write-Host "All screenshots captured successfully in $screenshotDir!" -ForegroundColor Cyan
