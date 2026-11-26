Add-Type -AssemblyName "System.IO.Compression.FileSystem"
$zip = [System.IO.Compression.ZipFile]::OpenRead("C:\Users\rajsv\Desktop\ProductCare_INT402\ProductCare_Production_Deploy.zip")
$foundLeak = $false

foreach ($entry in $zip.Entries) {
    if ($entry.Name.EndsWith(".json") -or $entry.Name.EndsWith(".config")) {
        $reader = New-Object System.IO.StreamReader($entry.Open())
        $content = $reader.ReadToEnd()
        $reader.Dispose()

        if ($content.Contains("localdb") -or $content.Contains("ProductCare_Development_Secret") -or $content.Contains("hD@37+nAbB!6")) {
            Write-Host "[LEAK DETECTED] in $($entry.FullName)" -ForegroundColor Red
            $foundLeak = $true
        }
    }
}

if (-not $foundLeak) {
    Write-Host "[PASS] No database credentials, LocalDB strings, or development secrets found inside ZIP." -ForegroundColor Green
}
$zip.Dispose()
