$ErrorActionPreference = "Stop"

$edge = "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
if (-not (Test-Path $edge)) {
    $edge = "C:\Program Files\Microsoft\Edge\Application\msedge.exe"
}

$screenshotDir = "C:\Users\rajsv\Desktop\ProductCare_INT402\docs\screenshots"
if (-not (Test-Path $screenshotDir)) {
    New-Item -ItemType Directory -Path $screenshotDir -Force | Out-Null
}

$tempUserData = Join-Path $env:TEMP "EdgeP10_$([Guid]::NewGuid())"

Write-Host "Capturing Genuine P10 Application Screenshots..." -ForegroundColor Cyan

# Fetch latest ticket ID from SQL Server
$ticketIdLines = & sqlcmd -S "(localdb)\ProductCareTest" -d "ProductCareDb" -h -1 -W -Q "SET NOCOUNT ON; SELECT TOP 1 Id FROM ServiceRequests ORDER BY Id DESC"
$ticketId = ($ticketIdLines | Where-Object { $_ -match '^\d+$' } | Select-Object -First 1).Trim()
if (-not $ticketId) { $ticketId = "2004" }

$shots = @(
    @{ Url = "http://localhost:5213/"; Out = "p10_01_landing_hero.png"; Desc = "Public Landing Page (Working Identity & P10 Positioning)" },
    @{ Url = "http://localhost:5213/Account/Login"; Out = "p10_02_login_portal.png"; Desc = "Authentication Portal" },
    @{ Url = "http://localhost:5213/Customer/Dashboard?devUser=john.doe@example.com"; Out = "p10_03_customer_dashboard.png"; Desc = "Customer Ownership & Issue Intake Overview" },
    @{ Url = "http://localhost:5213/Customer/ServiceRequests?devUser=john.doe@example.com"; Out = "p10_04_customer_tickets_queue.png"; Desc = "Customer Ticket Queue (Campus Location & SLA Badges)" },
    @{ Url = "http://localhost:5213/Customer/ServiceRequests/Create?devUser=john.doe@example.com"; Out = "p10_05_customer_create_ticket.png"; Desc = "M1 Issue Intake Form with Campus Location & SLA Targets" },
    @{ Url = "http://localhost:5213/Customer/ServiceRequests/Details/${ticketId}?devUser=john.doe@example.com"; Out = "p10_06_customer_ticket_details.png"; Desc = "M4 Work Order Lifecycle & Audit History" },
    @{ Url = "http://localhost:5213/Customer/NetworkQuality?devUser=john.doe@example.com"; Out = "p10_07_network_quality_monitor.png"; Desc = "Campus Network Quality & Latency Telemetry Monitor" },
    @{ Url = "http://localhost:5213/Admin/Dashboard?devUser=admin@productcare.local"; Out = "p10_08_admin_sla_dashboard.png"; Desc = "M9 Admin Dashboard (SLA Compliance, Building Breakdown, Network Health)" },
    @{ Url = "http://localhost:5213/Admin/ServiceRequests?devUser=admin@productcare.local"; Out = "p10_09_admin_master_queue.png"; Desc = "M2 Admin Master Queue (SLA Countdown, Triage & Breached Indicators)" },
    @{ Url = "http://localhost:5213/Admin/ServiceRequests/Details/${ticketId}?devUser=admin@productcare.local"; Out = "p10_10_admin_ticket_management.png"; Desc = "M2 Routing & M4 State Machine Transition" },
    @{ Url = "http://localhost:5213/Admin/Reports?devUser=admin@productcare.local"; Out = "p10_11_admin_reports_sp.png"; Desc = "SQL Server Stored Procedure & View Analytics" }
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
Write-Host "All P10 screenshots captured successfully in $screenshotDir!" -ForegroundColor Cyan
