$ErrorActionPreference = "Stop"

$edge = "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
if (-not (Test-Path $edge)) {
    $edge = "C:\Program Files\Microsoft\Edge\Application\msedge.exe"
}

$screenshotDir = "C:\Users\rajsv\Desktop\ProductCare_INT402\docs\screenshots"
if (-not (Test-Path $screenshotDir)) {
    New-Item -ItemType Directory -Path $screenshotDir -Force | Out-Null
}

$tempUserData = Join-Path $env:TEMP "EdgeQA_$([Guid]::NewGuid())"

# Fetch personal ticket ID from SQL Server
$ticketIdLines = & sqlcmd -S "(localdb)\ProductCareTest" -d "ProductCareDb" -h -1 -W -Q "SET NOCOUNT ON; SELECT TOP 1 Id FROM ServiceRequests WHERE UserId = (SELECT Id FROM AspNetUsers WHERE Email = 'rajasvmahendra@gmail.com') ORDER BY Id DESC"
$ticketId = ($ticketIdLines | Where-Object { $_ -match '^\d+$' } | Select-Object -First 1)
if ($ticketId) { $ticketId = $ticketId.Trim() } else { $ticketId = "2004" }

# Fetch personal product ID (MacBook Pro)
$prodIdLines = & sqlcmd -S "(localdb)\ProductCareTest" -d "ProductCareDb" -h -1 -W -Q "SET NOCOUNT ON; SELECT TOP 1 Id FROM Products WHERE UserId = (SELECT Id FROM AspNetUsers WHERE Email = 'rajasvmahendra@gmail.com') AND IsArchived = 0 ORDER BY Id ASC"
$prodId = ($prodIdLines | Where-Object { $_ -match '^\d+$' } | Select-Object -First 1)
if ($prodId) { $prodId = $prodId.Trim() } else { $prodId = "4005" }

$targets = @(
    @{ Url = "http://localhost:5213/"; BaseName = "final_01_landing_hero"; Desc = "Public Landing Page" },
    @{ Url = "http://localhost:5213/Account/Login"; BaseName = "final_02_login_portal"; Desc = "Authentication Portal (Sign In State)" },
    @{ Url = "http://localhost:5213/Account/Register"; BaseName = "final_02b_register_portal"; Desc = "Authentication Portal (Get Started State)" },
    @{ Url = "http://localhost:5213/Customer/Dashboard?devUser=rajasvmahendra@gmail.com"; BaseName = "final_03_customer_dashboard"; Desc = "Customer Overview (Compact Icon Rail 58px)" },
    @{ Url = "http://localhost:5213/Customer/Dashboard?devUser=rajasvmahendra@gmail.com&scrollBottom=true"; BaseName = "final_03_customer_dashboard_bottom"; Desc = "Customer Overview (Scrolled to Bottom)" },
    @{ Url = "http://localhost:5213/Customer/Dashboard?devUser=rajasvmahendra@gmail.com&expanded=true"; BaseName = "final_03_customer_dashboard_expanded"; Desc = "Customer Overview (Expanded Rail Overlay)" },
    @{ Url = "http://localhost:5213/Customer/Products?devUser=rajasvmahendra@gmail.com"; BaseName = "final_04_product_cards"; Desc = "Product Ownership Cards (Personal Inventory)" },
    @{ Url = "http://localhost:5213/Customer/Products?devUser=rajasvmahendra@gmail.com&scrollBottom=true"; BaseName = "final_04_product_cards_bottom"; Desc = "Product Ownership Cards (Scrolled to Bottom)" },
    @{ Url = "http://localhost:5213/Customer/Warranty?devUser=rajasvmahendra@gmail.com"; BaseName = "final_13_warranty_timeline"; Desc = "Warranty Status & Coverage Timeline" },
    @{ Url = "http://localhost:5213/Customer/Warranty?devUser=rajasvmahendra@gmail.com&scrollBottom=true"; BaseName = "final_13_warranty_timeline_bottom"; Desc = "Warranty Status & Coverage Timeline (Scrolled to Bottom)" },
    @{ Url = "http://localhost:5213/Customer/ServiceRequests?devUser=rajasvmahendra@gmail.com"; BaseName = "final_14_service_requests"; Desc = "Customer Service Requests Queue" },
    @{ Url = "http://localhost:5213/Customer/ServiceRequests?devUser=rajasvmahendra@gmail.com&scrollBottom=true"; BaseName = "final_14_service_requests_bottom"; Desc = "Customer Service Requests Queue (Scrolled to Bottom)" },
    @{ Url = "http://localhost:5213/Customer/Products/Create?devUser=rajasvmahendra@gmail.com"; BaseName = "final_05_voice_registration"; Desc = "Smart Product Registration 3-Step Guided Form" },
    @{ Url = "http://localhost:5213/Customer/Products/Details/${prodId}?devUser=rajasvmahendra@gmail.com"; BaseName = "final_06_product_details"; Desc = "Product Details & Warranty Countdown" },
    @{ Url = "http://localhost:5213/Customer/Products/Edit/${prodId}?devUser=rajasvmahendra@gmail.com"; BaseName = "final_06b_product_edit"; Desc = "Product Edit View" },
    @{ Url = "http://localhost:5213/Customer/ServiceCenters?devUser=rajasvmahendra@gmail.com"; BaseName = "final_07_service_centres"; Desc = "Find a Service Centre Directory" },
    @{ Url = "http://localhost:5213/Customer/ServiceRequests/Details/${ticketId}?devUser=rajasvmahendra@gmail.com"; BaseName = "final_08_repair_lifecycle"; Desc = "5-Stage Repair Journey & Assigned Centre" },
    @{ Url = "http://localhost:5213/Admin/Dashboard?devUser=admin@productcare.local"; BaseName = "final_09_admin_dashboard"; Desc = "Admin Command Dashboard & Centre Status" },
    @{ Url = "http://localhost:5213/Admin/ServiceCenters?devUser=admin@productcare.local"; BaseName = "final_10_admin_centres_mgmt"; Desc = "Admin Service Centre Directory Management" },
    @{ Url = "http://localhost:5213/#showcase"; BaseName = "final_11_interactive_showcase"; Desc = "Interactive Capability Showcase" },
    @{ Url = "http://localhost:5213/#journey"; BaseName = "final_12_product_journey"; Desc = "5-Stage Product Lifecycle Journey" }
)

# 1. Capture Standard Desktop (1440x900)
Write-Host "Capturing Standard Desktop (1440x900) Visuals..." -ForegroundColor Cyan
foreach ($t in $targets) {
    $dest = Join-Path $screenshotDir "$($t.BaseName)_desktop.png"
    $args = @(
        "--headless=new",
        "--disable-gpu",
        "--virtual-time-budget=2000",
        "--user-data-dir=$tempUserData",
        "--window-size=1440,900",
        "--screenshot=$dest",
        $t.Url
    )
    $proc = Start-Process -FilePath $edge -ArgumentList $args -PassThru -Wait
    if (Test-Path $dest) {
        $len = (Get-Item $dest).Length
        Write-Host "  [OK] $($t.BaseName)_desktop.png ($len bytes) - $($t.Desc) (1440x900)" -ForegroundColor Green
    }
}

# 2. Capture Laptop Default (1366x768)
Write-Host "`nCapturing Laptop Resolution (1366x768) Key Visuals..." -ForegroundColor Cyan
$laptopTargets = @(
    "final_01_landing_hero",
    "final_02_login_portal",
    "final_02b_register_portal",
    "final_03_customer_dashboard",
    "final_03_customer_dashboard_bottom",
    "final_03_customer_dashboard_expanded",
    "final_04_product_cards",
    "final_04_product_cards_bottom",
    "final_13_warranty_timeline",
    "final_13_warranty_timeline_bottom",
    "final_14_service_requests",
    "final_14_service_requests_bottom",
    "final_05_voice_registration",
    "final_06_product_details",
    "final_06b_product_edit",
    "final_07_service_centres",
    "final_08_repair_lifecycle"
)
foreach ($t in $targets | Where-Object { $laptopTargets -contains $_.BaseName }) {
    $dest = Join-Path $screenshotDir "$($t.BaseName)_1366x768.png"
    $args = @(
        "--headless=new",
        "--disable-gpu",
        "--virtual-time-budget=2000",
        "--user-data-dir=$tempUserData",
        "--window-size=1366,768",
        "--screenshot=$dest",
        $t.Url
    )
    $proc = Start-Process -FilePath $edge -ArgumentList $args -PassThru -Wait
    if (Test-Path $dest) {
        $len = (Get-Item $dest).Length
        Write-Host "  [OK] $($t.BaseName)_1366x768.png ($len bytes) - $($t.Desc) (1366x768)" -ForegroundColor Green
    }
}

# 3. Capture Full HD (1920x1080)
Write-Host "`nCapturing Full HD Resolution (1920x1080) Key Visuals..." -ForegroundColor Cyan
foreach ($t in $targets | Where-Object { $laptopTargets -contains $_.BaseName }) {
    $dest = Join-Path $screenshotDir "$($t.BaseName)_1920x1080.png"
    $args = @(
        "--headless=new",
        "--disable-gpu",
        "--virtual-time-budget=2000",
        "--user-data-dir=$tempUserData",
        "--window-size=1920,1080",
        "--screenshot=$dest",
        $t.Url
    )
    $proc = Start-Process -FilePath $edge -ArgumentList $args -PassThru -Wait
    if (Test-Path $dest) {
        $len = (Get-Item $dest).Length
        Write-Host "  [OK] $($t.BaseName)_1920x1080.png ($len bytes) - $($t.Desc) (1920x1080)" -ForegroundColor Green
    }
}

# 4. Capture Mobile Viewport (390x844 iPhone Viewport)
Write-Host "`nCapturing Mobile Viewport (390x844) Visuals..." -ForegroundColor Cyan
foreach ($t in $targets) {
    $dest = Join-Path $screenshotDir "$($t.BaseName)_mobile.png"
    $args = @(
        "--headless=new",
        "--disable-gpu",
        "--virtual-time-budget=2000",
        "--user-data-dir=$tempUserData",
        "--window-size=390,844",
        "--screenshot=$dest",
        $t.Url
    )
    $proc = Start-Process -FilePath $edge -ArgumentList $args -PassThru -Wait
    if (Test-Path $dest) {
        $len = (Get-Item $dest).Length
        Write-Host "  [OK] $($t.BaseName)_mobile.png ($len bytes) - $($t.Desc) (Mobile)" -ForegroundColor Green
    }
}

Remove-Item -Recurse -Force $tempUserData -ErrorAction SilentlyContinue
Write-Host "`nAll required resolution screenshots captured successfully in $screenshotDir!" -ForegroundColor Cyan
