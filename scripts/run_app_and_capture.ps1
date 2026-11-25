$ErrorActionPreference = "Stop"

Write-Host "Starting ProductCare on http://localhost:5213..." -ForegroundColor Cyan

$process = Start-Process -FilePath "dotnet" -ArgumentList "run --project src/ProductCare.Web/ProductCare.Web.csproj --urls http://localhost:5213 --no-build" -PassThru

try {
    $started = $false
    $retries = 25
    while ($retries -gt 0 -and -not $started) {
        Start-Sleep -Seconds 1
        try {
            $resp = Invoke-WebRequest -Uri "http://localhost:5213/" -UseBasicParsing -TimeoutSec 2
            if ($resp.StatusCode -eq 200) {
                $started = $true
                Write-Host "ProductCare is online and responding at http://localhost:5213!" -ForegroundColor Green
            }
        } catch {
            Write-Host "Waiting for server to initialize ($retries attempts remaining)..." -ForegroundColor Yellow
        }
        $retries--
    }

    if (-not $started) {
        throw "ProductCare web server failed to start within 25 seconds."
    }

    Write-Host "Running screenshot capture..." -ForegroundColor Cyan
    & powershell -ExecutionPolicy Bypass -File "scripts/capture_product_transformation_screenshots.ps1"
}
finally {
    Write-Host "Shutting down ProductCare web server process (PID $($process.Id))..." -ForegroundColor Cyan
    Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
    # Also clean up any child dotnet processes
    Get-Process -Name "ProductCare.Web" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
    Write-Host "Clean shutdown complete." -ForegroundColor Green
}
