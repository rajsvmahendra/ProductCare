$ErrorActionPreference = "Stop"

$workspaceRoot = "c:\Users\rajsv\Desktop\ProductCare_INT402"
Set-Location $workspaceRoot

$edge = "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
if (-not (Test-Path $edge)) {
    $edge = "C:\Program Files\Microsoft\Edge\Application\msedge.exe"
}

$artifactDir = "C:\Users\rajsv\.gemini\antigravity\brain\25cf6cdf-be18-462b-951d-e31c02c41e31"
$screenshotDir = "C:\Users\rajsv\Desktop\ProductCare_INT402\docs\screenshots"
if (-not (Test-Path $screenshotDir)) {
    New-Item -ItemType Directory -Path $screenshotDir -Force | Out-Null
}

Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host " TESTING PRODUCTCARE HERO VIDEO & LIQUID GLASS VISUAL INTEGRATION " -ForegroundColor Cyan
Write-Host "==================================================================" -ForegroundColor Cyan

# 1. Stop any running web app
Get-Process -Name "ProductCare.Web" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

# 2. Start Web Application
Write-Host "`n[STEP 1] Starting Web Application on http://localhost:5213..." -ForegroundColor Yellow
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
    } catch {
        # continue waiting
    }
}

if (-not $serverReady) {
    if (-not $webProc.HasExited) { $webProc.Kill() }
    throw "Web application failed to start on http://localhost:5213 within 30 seconds."
}
Write-Host "  -> Server is responsive on http://localhost:5213!" -ForegroundColor Green

# 3. Verify HTML markup
Write-Host "`n[STEP 2] Verifying Hero Video Markup in Landing Page HTML..." -ForegroundColor Yellow
$homeHtml = (Invoke-WebRequest -Uri "http://localhost:5213/" -UseBasicParsing).Content

if ($homeHtml -match 'id="heroVideoPrimary"' -and $homeHtml -match 'id="pcHeroVideoCard"' -and $homeHtml -match 'src="/videos/productcare_hero.mp4"') {
    Write-Host "  [PASS] Hero video element with correct source and card ID found in HTML." -ForegroundColor Green
} else {
    throw "Hero video markup verification failed in landing page HTML."
}

if ($homeHtml -match 'id="heroVideoSecondary"' -and $homeHtml -match 'id="pcHeroVideoGlaze"') {
    Write-Host "  [PASS] Seamless looping crossfade secondary video and glaze layer present." -ForegroundColor Green
} else {
    throw "Secondary video / crossfade markup missing."
}

if ($homeHtml -notmatch 'pc-poster-main-card' -and $homeHtml -notmatch 'MacBook Pro 16" M3 Max' -and $homeHtml -notmatch 'Invoice Proof #9842') {
    Write-Host "  [PASS] Obsolete fake dashboard card and floating badges cleanly replaced." -ForegroundColor Green
} else {
    throw "Obsolete fake card or badges still found in landing page HTML."
}

# 4. Capture Visuals across Viewports
Write-Host "`n[STEP 3] Capturing High-Fidelity Screenshots of New Hero Visual..." -ForegroundColor Yellow
$tempUserData = Join-Path $env:TEMP "EdgeHeroQA_$([Guid]::NewGuid())"

$viewports = @(
    @{ Name = "hero_video_desktop_1440x900.png"; Width = 1440; Height = 900; Desc = "Desktop (1440x900)" },
    @{ Name = "hero_video_desktop_1920x1080.png"; Width = 1920; Height = 1080; Desc = "Full HD (1920x1080)" },
    @{ Name = "hero_video_tablet_768x1024.png"; Width = 768; Height = 1024; Desc = "Tablet Portrait (768x1024)" },
    @{ Name = "hero_video_mobile_390x844.png"; Width = 390; Height = 844; Desc = "Mobile Portrait (390x844)" }
)

foreach ($vp in $viewports) {
    $destDocs = Join-Path $screenshotDir $vp.Name
    $destArtifact = Join-Path $artifactDir $vp.Name

    $edgeArgs = @(
        "--headless=new",
        "--user-data-dir=$tempUserData",
        "--window-size=$($vp.Width),$($vp.Height)",
        "--autoplay-policy=no-user-gesture-required",
        "--virtual-time-budget=3500",
        "--screenshot=$destDocs",
        "http://localhost:5213/"
    )

    $proc = Start-Process -FilePath $edge -ArgumentList $edgeArgs -PassThru -Wait
    if (Test-Path $destDocs) {
        Copy-Item -Path $destDocs -Destination $destArtifact -Force
        $len = (Get-Item $destDocs).Length
        Write-Host "  [OK] $($vp.Desc): $destDocs ($len bytes)" -ForegroundColor Green
    } else {
        Write-Host "  [WARN] Failed to capture $($vp.Desc)" -ForegroundColor Yellow
    }
}

# 5. Clean Teardown
Write-Host "`n[STEP 4] Stopping Web Application..." -ForegroundColor Yellow
if ($webProc -and -not $webProc.HasExited) {
    Stop-Process -Id $webProc.Id -Force -ErrorAction SilentlyContinue
}
Get-Process -Name "ProductCare.Web" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Write-Host "  -> Web application stopped cleanly." -ForegroundColor Green

Write-Host "`n==================================================================" -ForegroundColor Green
Write-Host " HERO VIDEO QA CAPTURE COMPLETE!                                 " -ForegroundColor Green
Write-Host "==================================================================" -ForegroundColor Green
