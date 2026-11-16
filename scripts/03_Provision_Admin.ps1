<#
.SYNOPSIS
    Secure development administrator provisioning script for ProductCare.
.DESCRIPTION
    Provisions or verifies the system administrator account without committing hardcoded credentials.
    Reads optional parameters or environment variables; otherwise generates a cryptographically strong temporary password.
.PARAMETER Email
    Optional admin email address (default: admin@productcare.local)
.PARAMETER Password
    Optional admin password. If omitted, a secure 16-character password is generated automatically.
#>

[CmdletBinding()]
param (
    [string]$Email = "admin@productcare.local",
    [string]$Password = ""
)

$ErrorActionPreference = "Stop"

Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host " PRODUCTCARE — Administrator Provisioning Utility" -ForegroundColor Cyan
Write-Host "=================================================================" -ForegroundColor Cyan

if (-not [string]::IsNullOrWhiteSpace($Email)) {
    $env:PRODUCTCARE_ADMIN_EMAIL = $Email
}

if (-not [string]::IsNullOrWhiteSpace($Password)) {
    $env:PRODUCTCARE_ADMIN_PASSWORD = $Password
}

$repoRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repoRoot "src\ProductCare.Web\ProductCare.Web.csproj"

Write-Host "Executing provisioning routine via ASP.NET Core CLI..." -ForegroundColor Yellow

# Execute the application with explicit provisioning CLI switch
dotnet run --project $projectPath --no-build -- --provision-admin

# Clean up sensitive environment variables from current session
Remove-Item Env:\PRODUCTCARE_ADMIN_PASSWORD -ErrorAction SilentlyContinue
Remove-Item Env:\PRODUCTCARE_ADMIN_EMAIL -ErrorAction SilentlyContinue

Write-Host "Provisioning script execution complete." -ForegroundColor Green
