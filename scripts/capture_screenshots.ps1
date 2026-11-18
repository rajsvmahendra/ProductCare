$edge = "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
if (-not (Test-Path $edge)) {
    $edge = "C:\Program Files\Microsoft\Edge\Application\msedge.exe"
}

$screenshotDir = "c:\Users\rajsv\Desktop\ProductCare_INT402\docs\screenshots"
if (-not (Test-Path $screenshotDir)) {
    New-Item -ItemType Directory -Path $screenshotDir -Force | Out-Null
}

$tempUserData = Join-Path $env:TEMP "EdgeHeadless_$([Guid]::NewGuid())"

Write-Host "Starting ProductCare web server..."
$server = Start-Process -FilePath "dotnet" -ArgumentList "run --project src/ProductCare.Web --no-build --urls http://127.0.0.1:5213" -PassThru

Start-Sleep -Seconds 5

try {
    # Probe endpoints
    $homeRes = Invoke-WebRequest -Uri "http://127.0.0.1:5213/" -UseBasicParsing
    Write-Host "GET / => HTTP $($homeRes.StatusCode)"

    $loginRes = Invoke-WebRequest -Uri "http://127.0.0.1:5213/Account/Login" -UseBasicParsing
    Write-Host "GET /Account/Login => HTTP $($loginRes.StatusCode)"

    $regRes = Invoke-WebRequest -Uri "http://127.0.0.1:5213/Account/Register" -UseBasicParsing
    Write-Host "GET /Account/Register => HTTP $($regRes.StatusCode)"

    $shots = @(
        @{ Url = "http://127.0.0.1:5213/"; Out = "01_landing_desktop.png"; W = 1440; H = 920 },
        @{ Url = "http://127.0.0.1:5213/"; Out = "02_landing_mobile.png"; W = 390; H = 844 },
        @{ Url = "http://127.0.0.1:5213/#demonstration"; Out = "03_landing_demonstration.png"; W = 1440; H = 900 },
        @{ Url = "http://127.0.0.1:5213/Account/Register"; Out = "04_register_desk.png"; W = 1440; H = 900 },
        @{ Url = "http://127.0.0.1:5213/Account/Login"; Out = "05_login_desk.png"; W = 1440; H = 900 },
        @{ Url = "http://127.0.0.1:5213/Account/Register"; Out = "06_register_mobile.png"; W = 390; H = 844 },
        @{ Url = "http://127.0.0.1:5213/Home/Privacy"; Out = "07_privacy_desktop.png"; W = 1440; H = 900 },
        @{ Url = "http://127.0.0.1:5213/Account/AccessDenied"; Out = "08_access_denied.png"; W = 1440; H = 900 },
        @{ Url = "http://127.0.0.1:5213/swagger/index.html"; Out = "09_swagger_api.png"; W = 1440; H = 900 }
    )

    foreach ($s in $shots) {
        $dest = Join-Path $screenshotDir $s.Out
        $args = @(
            "--headless=new",
            "--disable-gpu",
            "--user-data-dir=$tempUserData",
            "--window-size=$($s.W),$($s.H)",
            "--screenshot=$dest",
            $s.Url
        )
        $p = Start-Process -FilePath $edge -ArgumentList $args -PassThru -Wait
        $len = (Get-Item $dest).Length
        Write-Host "Captured $($s.Out) ($len bytes, $($s.W)x$($s.H))"
    }
}
finally {
    Write-Host "Stopping ProductCare server..."
    if ($server -and -not $server.HasExited) {
        Stop-Process -Id $server.Id -Force -ErrorAction SilentlyContinue
    }
    # Also stop any child dotnet process
    Get-Process | Where-Object { $_.ProcessName -like "*ProductCare*" } | Stop-Process -Force -ErrorAction SilentlyContinue
    Remove-Item -Recurse -Force $tempUserData -ErrorAction SilentlyContinue
}
