Add-Type -AssemblyName System.Drawing

$srcPath = "c:\Users\rajsv\Desktop\ProductCare_INT402\src\ProductCare.Web\wwwroot\images\productcare-logo.png"
$backupPath = "c:\Users\rajsv\Desktop\ProductCare_INT402\src\ProductCare.Web\wwwroot\images\productcare-logo-orig.png"

if (-not (Test-Path $backupPath)) {
    Copy-Item $srcPath $backupPath
}

$src = [System.Drawing.Bitmap]::FromFile($backupPath)

# Content is X=190..3040 (width 2850), Y=260..885 (height 625)
# Crop tightly with 20px padding: X=170, Y=240, Width=2890, Height=665
$cropRect = [System.Drawing.Rectangle]::new(170, 240, 2890, 665)
$dest = [System.Drawing.Bitmap]::new($cropRect.Width, $cropRect.Height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($dest)
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
$g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
$g.DrawImage($src, 0, 0, $cropRect, [System.Drawing.GraphicsUnit]::Pixel)

$g.Dispose()
$src.Dispose()

$dest.Save($srcPath, [System.Drawing.Imaging.ImageFormat]::Png)
$dest.Dispose()

Write-Host "Cropped logo saved! New dimensions: 2890 x 665 (clean, crisp, no empty margins)" -ForegroundColor Green
