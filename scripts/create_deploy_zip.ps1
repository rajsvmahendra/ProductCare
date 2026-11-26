Add-Type -AssemblyName "System.IO.Compression.FileSystem"
$sourceDir = "C:\Users\rajsv\Desktop\ProductCare_INT402\publish"
$zipPath = "C:\Users\rajsv\Desktop\ProductCare_INT402\ProductCare_Production_Deploy.zip"

if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

[System.IO.Compression.ZipFile]::CreateFromDirectory($sourceDir, $zipPath, [System.IO.Compression.CompressionLevel]::Optimal, $false)

$item = Get-Item $zipPath
Write-Host "ZIP created successfully: $($item.FullName) ($($item.Length) bytes, $([math]::Round($item.Length / 1MB, 2)) MB)"

$zip = [System.IO.Compression.ZipFile]::OpenRead($zipPath)
$outerCount = ($zip.Entries | Where-Object { $_.FullName.StartsWith("publish/") -or $_.FullName.StartsWith("publish\") }).Count
$dllCount = ($zip.Entries | Where-Object { $_.FullName -eq "ProductCare.Web.dll" }).Count
$webConfigCount = ($zip.Entries | Where-Object { $_.FullName -eq "web.config" }).Count
$appsettingsCount = ($zip.Entries | Where-Object { $_.FullName -eq "appsettings.json" }).Count
$prodSettingsCount = ($zip.Entries | Where-Object { $_.FullName -eq "appsettings.Production.json" }).Count
$videoCount = ($zip.Entries | Where-Object { $_.FullName -eq "wwwroot/videos/productcare_hero.mp4" -or $_.FullName -eq "wwwroot\videos\productcare_hero.mp4" }).Count

Write-Host "`nZIP Structural Validation:"
Write-Host "  Total archive entries: $($zip.Entries.Count)"
Write-Host "  Outer 'publish/' folder entries: $outerCount (Expected: 0)"
Write-Host "  ProductCare.Web.dll at root: $dllCount (Expected: 1)"
Write-Host "  web.config at root: $webConfigCount (Expected: 1)"
Write-Host "  appsettings.json at root: $appsettingsCount (Expected: 1)"
Write-Host "  appsettings.Production.json at root: $prodSettingsCount (Expected: 1)"
Write-Host "  wwwroot/videos/productcare_hero.mp4: $videoCount (Expected: 1)"

$zip.Dispose()
