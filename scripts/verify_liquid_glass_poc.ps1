$ErrorActionPreference = "Stop"

Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host " PRODUCTCARE: LIQUID GLASS WEBGL PROOF-OF-CONCEPT VERIFIER       " -ForegroundColor Cyan
Write-Host "==================================================================" -ForegroundColor Cyan

$workspaceRoot = "c:\Users\rajsv\Desktop\ProductCare_INT402"
Set-Location $workspaceRoot

$edge = "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
if (-not (Test-Path $edge)) {
    $edge = "C:\Program Files\Microsoft\Edge\Application\msedge.exe"
}

$screenshotDir = "C:\Users\rajsv\Desktop\ProductCare_INT402\docs\screenshots"
if (-not (Test-Path $screenshotDir)) {
    New-Item -ItemType Directory -Path $screenshotDir -Force | Out-Null
}

$tempUserData = Join-Path $env:TEMP "EdgePoc_$([Guid]::NewGuid())"

# Kill any existing ProductCare.Web process
Get-Process -Name "*ProductCare.Web*" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 1

# Launch Web Application on port 5213
Write-Host "`n[1] Starting ProductCare on http://localhost:5213..." -ForegroundColor Yellow
$startInfo = New-Object System.Diagnostics.ProcessStartInfo
$startInfo.FileName = "dotnet"
$startInfo.Arguments = "run --project src/ProductCare.Web/ProductCare.Web.csproj --urls http://localhost:5213 --no-build"
$startInfo.WorkingDirectory = $workspaceRoot
$startInfo.EnvironmentVariables["ASPNETCORE_ENVIRONMENT"] = "Development"
$startInfo.UseShellExecute = $false
$startInfo.RedirectStandardOutput = $true
$startInfo.RedirectStandardError = $true
$startInfo.CreateNoWindow = $true

$webProc = [System.Diagnostics.Process]::Start($startInfo)

$serverReady = $false
for ($i = 0; $i -lt 30; $i++) {
    Start-Sleep -Seconds 1
    try {
        $resp = Invoke-WebRequest -Uri "http://localhost:5213/" -UseBasicParsing -TimeoutSec 2
        if ($resp.StatusCode -eq 200) {
            $serverReady = $true
            break
        }
    } catch {}
}

if (-not $serverReady) {
    if (-not $webProc.HasExited) { $webProc.Kill() }
    throw "Web application failed to start on http://localhost:5213 within 30 seconds."
}
Write-Host "  -> Server is live and responding at http://localhost:5213!" -ForegroundColor Green

try {
    # 2. Check HTML output for liquid-glass markup
    Write-Host "`n[2] Verifying Liquid Glass Markup & Scripts in Responses..." -ForegroundColor Yellow
    $homeHtml = (Invoke-WebRequest -Uri "http://localhost:5213/" -UseBasicParsing).Content
    if ($homeHtml -match '<liquid-glass[^>]*class="pc-nav-liquid-scene"') {
        Write-Host "  [PASS] <liquid-glass> custom element tag found in public navbar!" -ForegroundColor Green
    } else {
        throw "Failed: <liquid-glass> element tag missing from public navbar HTML!"
    }

    if ($homeHtml -match 'liquid-glass\.js') {
        Write-Host "  [PASS] liquid-glass.js module script referenced in document!" -ForegroundColor Green
    } else {
        Write-Host "HTML preview near bottom:" -ForegroundColor Red
        $homeHtml.Substring([Math]::Max(0, $homeHtml.Length - 1000))
        throw "Failed: liquid-glass.js module script not referenced in HTML!"
    }

    $pocHtml = (Invoke-WebRequest -Uri "http://localhost:5213/Home/LiquidGlassPoc" -UseBasicParsing).Content
    if ($pocHtml -match '<liquid-glass id="pocAuroraScene"') {
        Write-Host "  [PASS] Standalone POC view rendered with <liquid-glass> scene!" -ForegroundColor Green
    } else {
        throw "Failed: POC view did not return expected liquid-glass elements!"
    }

    # 3. Capture Screenshots with Edge Headless
    Write-Host "`n[3] Capturing Proof-of-Concept Screenshots with Edge Headless..." -ForegroundColor Yellow
    
    $pocTargets = @(
        @{ Url = "http://localhost:5213/"; BaseName = "poc_01_liquid_glass_navbar_desktop"; W = 1440; H = 900; Desc = "Public Navbar with Liquid Glass Engine (Desktop)" },
        @{ Url = "http://localhost:5213/"; BaseName = "poc_01_liquid_glass_navbar_mobile"; W = 390; H = 844; Desc = "Public Navbar with Liquid Glass Engine (Mobile)" },
        @{ Url = "http://localhost:5213/Home/LiquidGlassPoc"; BaseName = "poc_02_liquid_glass_standalone_desktop"; W = 1440; H = 950; Desc = "Standalone Liquid Glass WebGL Proof-of-Concept Stage" }
    )

    foreach ($t in $pocTargets) {
        $dest = Join-Path $screenshotDir "$($t.BaseName).png"
        $args = @(
            "--headless=new",
            "--disable-gpu",
            "--virtual-time-budget=3000",
            "--user-data-dir=$tempUserData",
            "--window-size=$($t.W),$($t.H)",
            "--screenshot=$dest",
            $t.Url
        )
        $proc = Start-Process -FilePath $edge -ArgumentList $args -PassThru -Wait
        if (Test-Path $dest) {
            $len = (Get-Item $dest).Length
            Write-Host "  [OK] $($t.BaseName).png ($len bytes) - $($t.Desc)" -ForegroundColor Green
        } else {
            Write-Host "  [WARN] Failed to capture $($t.BaseName).png" -ForegroundColor Red
        }
    }
}
finally {
    Write-Host "`n[4] Stopping Background Web Server..." -ForegroundColor Yellow
    if ($webProc -and -not $webProc.HasExited) {
        try { Stop-Process -Id $webProc.Id -Force -ErrorAction SilentlyContinue } catch {}
    }
    Get-Process -Name "*ProductCare.Web*" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
    Remove-Item -Recurse -Force $tempUserData -ErrorAction SilentlyContinue
    Write-Host "  -> Background Web Server stopped cleanly." -ForegroundColor Green
}

Write-Host "`n==================================================================" -ForegroundColor Green
Write-Host " LIQUID GLASS PROOF-OF-CONCEPT VERIFICATION COMPLETE!             " -ForegroundColor Green
Write-Host "==================================================================" -ForegroundColor Green
