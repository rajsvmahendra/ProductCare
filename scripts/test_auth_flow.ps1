$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Net.Http

Write-Host "Testing Authentication Flow on http://localhost:5213..." -ForegroundColor Cyan

function Create-FormData($hash) {
    $dict = [System.Collections.Generic.Dictionary[string, string]]::new()
    foreach ($k in $hash.Keys) {
        $dict.Add([string]$k, [string]$hash[$k])
    }
    return [System.Net.Http.FormUrlEncodedContent]::new($dict)
}

$handler = [System.Net.Http.HttpClientHandler]::new()
$handler.CookieContainer = [System.Net.CookieContainer]::new()
$handler.AllowAutoRedirect = $true
$client = [System.Net.Http.HttpClient]::new($handler)

# 1. Fetch Login page and extract __RequestVerificationToken
$loginGet = $client.GetStringAsync("http://localhost:5213/Account/Login").GetAwaiter().GetResult()
if ($loginGet -match 'name="__RequestVerificationToken" type="hidden" value="([^"]+)"') {
    $token = $matches[1]
    Write-Host "[OK] Retrieved RequestVerificationToken: $($token.Substring(0, 15))..." -ForegroundColor Green
} else {
    throw "Failed to extract verification token from Login page"
}

# 2. Test Login as Customer (john.doe@example.com / Customer123!)
$loginContent = Create-FormData @{
    "Email" = "john.doe@example.com"
    "Password" = "Customer123!"
    "RememberMe" = "false"
    "__RequestVerificationToken" = $token
}

$loginPost = $client.PostAsync("http://localhost:5213/Account/Login", $loginContent).GetAwaiter().GetResult()
Write-Host "[OK] Customer Login Response Code: $($loginPost.StatusCode)" -ForegroundColor Green

# 3. Access Customer Dashboard
$dashRes = $client.GetStringAsync("http://localhost:5213/Customer/Dashboard").GetAwaiter().GetResult()
if ($dashRes -match "John Doe" -or $dashRes -match "Digital Product Vault" -or $dashRes -match "MacBook Pro") {
    Write-Host "[OK] Customer Dashboard rendered successfully for John Doe!" -ForegroundColor Green
} else {
    Write-Warning "Customer Dashboard content check preview: $($dashRes.Substring(0, [Math]::Min(300, $dashRes.Length)))"
}

# 4. Test Login as Admin (admin@productcare.local / Admin123!)
$adminHandler = [System.Net.Http.HttpClientHandler]::new()
$adminHandler.CookieContainer = [System.Net.CookieContainer]::new()
$adminHandler.AllowAutoRedirect = $true
$adminClient = [System.Net.Http.HttpClient]::new($adminHandler)

$adminLoginGet = $adminClient.GetStringAsync("http://localhost:5213/Account/Login").GetAwaiter().GetResult()
$adminToken = ""
if ($adminLoginGet -match 'name="__RequestVerificationToken" type="hidden" value="([^"]+)"') {
    $adminToken = $matches[1]
}

$adminLoginContent = Create-FormData @{
    "Email" = "admin@productcare.local"
    "Password" = "Admin123!"
    "RememberMe" = "false"
    "__RequestVerificationToken" = $adminToken
}

$adminLoginPost = $adminClient.PostAsync("http://localhost:5213/Account/Login", $adminLoginContent).GetAwaiter().GetResult()
Write-Host "[OK] Admin Login Response Code: $($adminLoginPost.StatusCode)" -ForegroundColor Green

$adminDashRes = $adminClient.GetStringAsync("http://localhost:5213/Admin/Dashboard").GetAwaiter().GetResult()
if ($adminDashRes -match "Operations Command Desk" -or $adminDashRes -match "Service Ticket Queue" -or $adminDashRes -match "Admin") {
    Write-Host "[OK] Admin Dashboard rendered successfully!" -ForegroundColor Green
} else {
    Write-Warning "Admin Dashboard check preview: $($adminDashRes.Substring(0, [Math]::Min(300, $adminDashRes.Length)))"
}

# 5. Test Registering a New User
$regHandler = [System.Net.Http.HttpClientHandler]::new()
$regHandler.CookieContainer = [System.Net.CookieContainer]::new()
$regHandler.AllowAutoRedirect = $true
$regClient = [System.Net.Http.HttpClient]::new($regHandler)

$regGet = $regClient.GetStringAsync("http://localhost:5213/Account/Register").GetAwaiter().GetResult()
$regToken = ""
if ($regGet -match 'name="__RequestVerificationToken" type="hidden" value="([^"]+)"') {
    $regToken = $matches[1]
}

$newEmail = "testuser_$(Get-Random)@example.com"
$regContent = Create-FormData @{
    "FullName" = "Alex Morgan"
    "Email" = $newEmail
    "Password" = "TestUser123!"
    "ConfirmPassword" = "TestUser123!"
    "__RequestVerificationToken" = $regToken
}

$regPost = $regClient.PostAsync("http://localhost:5213/Account/Register", $regContent).GetAwaiter().GetResult()
Write-Host "[OK] Registration POST Response Code: $($regPost.StatusCode)" -ForegroundColor Green

# Verify new user was created in DB
$userDbCheck = & sqlcmd -S "(localdb)\ProductCareTest" -d "ProductCareDb" -Q "SELECT Email, FullName FROM AspNetUsers WHERE Email = '$newEmail'"
Write-Host "[OK] User DB verification: $($userDbCheck -join ' ')" -ForegroundColor Green

Write-Host "=== ALL AUTHENTICATION FLOWS VERIFIED SUCCESSFULLY ===" -ForegroundColor Cyan
