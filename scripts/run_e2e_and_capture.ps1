$ErrorActionPreference = "Stop"

Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host " PRODUCTCARE: COMPLETE E2E QA VERIFICATION & VISUAL CAPTURE RUNNER " -ForegroundColor Cyan
Write-Host "==================================================================" -ForegroundColor Cyan

$workspaceRoot = "c:\Users\rajsv\Desktop\ProductCare_INT402"
Set-Location $workspaceRoot

# 1. Build Verification
Write-Host "`n[STEP 1] Building Solution..." -ForegroundColor Yellow
$buildOutput = dotnet build ProductCare.sln -m:1 --no-restore 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host $buildOutput
    throw "Build failed with exit code $LASTEXITCODE"
}
$buildSummary = $buildOutput | Select-String "0 Warning\(s\)", "0 Error\(s\)"
Write-Host "  -> Build Success: $buildSummary" -ForegroundColor Green

# 2. Unit & Integration Test Suite
Write-Host "`n[STEP 2] Running Unit & Integration Tests..." -ForegroundColor Yellow
$testOutput = dotnet test tests/ProductCare.Tests/ProductCare.Tests.csproj --no-build 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host $testOutput
    throw "Tests failed with exit code $LASTEXITCODE"
}
$testSummary = $testOutput | Select-String "Passed!", "Failed: 0", "Passed:"
Write-Host "  -> Tests Success: $($testSummary -join ' | ')" -ForegroundColor Green

# 3. Start Web Server in Background
Write-Host "`n[STEP 3] Launching Web Application on http://localhost:5213..." -ForegroundColor Yellow

# Kill any existing process on port 5213 or named ProductCare.Web
Get-Process -Name "ProductCare.Web" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

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

# Wait for server to become responsive
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
Write-Host "  -> Web application is live and responsive at http://localhost:5213!" -ForegroundColor Green

try {
    # 4. Run E2E Verification
    Write-Host "`n[STEP 4] Executing End-to-End Functional QA Test Suite..." -ForegroundColor Yellow
    try {
        & "$workspaceRoot\scripts\e2e_qa_verification.ps1"
    } catch {
        Write-Host "Error in e2e_qa_verification: $_" -ForegroundColor Red
        Write-Host $_.ScriptStackTrace -ForegroundColor Red
        throw
    }

    # 5. Capture Visuals
    Write-Host "`n[STEP 5] Capturing High-Fidelity Screenshots (Desktop & Mobile)..." -ForegroundColor Yellow
    try {
        & "$workspaceRoot\scripts\capture_qa_visuals.ps1"
    } catch {
        Write-Host "Error in capture_qa_visuals: $_" -ForegroundColor Red
        Write-Host $_.ScriptStackTrace -ForegroundColor Red
        throw
    }
}
finally {
    Write-Host "`n[TEARDOWN] Stopping Background Web Application..." -ForegroundColor Yellow
    if ($webProc -and -not $webProc.HasExited) {
        try {
            Stop-Process -Id $webProc.Id -Force -ErrorAction SilentlyContinue
        } catch {}
    }
    Get-Process -Name "ProductCare.Web" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
    Write-Host "  -> Web Application stopped cleanly." -ForegroundColor Green
}

Write-Host "`n==================================================================" -ForegroundColor Green
Write-Host " ALL QUALITY ASSURANCE & VERIFICATION STEPS COMPLETED 100%!       " -ForegroundColor Green
Write-Host "==================================================================" -ForegroundColor Green
