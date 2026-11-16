using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ProductCare.Web.Services;

/// <summary>
/// Secure physical storage provider for customer purchase invoices.
/// Files are stored outside wwwroot and served exclusively via authorized controller actions.
/// </summary>
public class InvoiceStorageService : IInvoiceStorageService
{
    private readonly string _storageFolder;
    private readonly ILogger<InvoiceStorageService> _logger;

    private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

    private static readonly Dictionary<string, (string[] MimeTypes, byte[][] MagicBytes)> AllowedFormats = new(StringComparer.OrdinalIgnoreCase)
    {
        [".pdf"] = (
            new[] { "application/pdf" },
            new[] { new byte[] { 0x25, 0x50, 0x44, 0x46 } } // %PDF
        ),
        [".jpg"] = (
            new[] { "image/jpeg", "image/pjpeg" },
            new[] { new byte[] { 0xFF, 0xD8, 0xFF } }
        ),
        [".jpeg"] = (
            new[] { "image/jpeg", "image/pjpeg" },
            new[] { new byte[] { 0xFF, 0xD8, 0xFF } }
        ),
        [".png"] = (
            new[] { "image/png" },
            new[] { new byte[] { 0x89, 0x50, 0x4E, 0x47 } }
        )
    };

    public InvoiceStorageService(IWebHostEnvironment environment, ILogger<InvoiceStorageService> logger)
    {
        _logger = logger;
        _storageFolder = Path.Combine(environment.ContentRootPath, "Uploads", "Invoices");

        if (!Directory.Exists(_storageFolder))
        {
            Directory.CreateDirectory(_storageFolder);
        }
    }

    public async Task<InvoiceUploadResult> SaveInvoiceAsync(IFormFile? file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return new InvoiceUploadResult(false, ErrorMessage: "No file was uploaded.");
        }

        if (file.Length > MaxFileSizeInBytes)
        {
            return new InvoiceUploadResult(false, ErrorMessage: "File exceeds maximum permitted size of 5 MB.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !AllowedFormats.TryGetValue(extension, out var formatRule))
        {
            return new InvoiceUploadResult(false, ErrorMessage: "Only PDF, JPEG, and PNG files are allowed.");
        }

        if (!formatRule.MimeTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            return new InvoiceUploadResult(false, ErrorMessage: "Content type header does not match file extension.");
        }

        // Validate magic signature bytes
        await using (var stream = file.OpenReadStream())
        {
            var headerBytes = new byte[8];
            var bytesRead = await stream.ReadAsync(headerBytes.AsMemory(0, 8), cancellationToken);

            var matchesSignature = false;
            foreach (var magic in formatRule.MagicBytes)
            {
                if (bytesRead >= magic.Length && headerBytes.Take(magic.Length).SequenceEqual(magic))
                {
                    matchesSignature = true;
                    break;
                }
            }

            if (!matchesSignature)
            {
                _logger.LogWarning("File signature validation failed for file {FileName}", file.FileName);
                return new InvoiceUploadResult(false, ErrorMessage: "File format verification failed. The file appears corrupted or altered.");
            }
        }

        // Generate safe unique filename to prevent path traversal or collision attacks
        var safeFileName = $"{Guid.NewGuid():N}{extension}";
        var targetFilePath = Path.Combine(_storageFolder, safeFileName);

        await using (var targetStream = new FileStream(targetFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await file.CopyToAsync(targetStream, cancellationToken);
        }

        _logger.LogInformation("Stored invoice file {SafeFileName} ({Bytes} bytes)", safeFileName, file.Length);

        return new InvoiceUploadResult(
            Success: true,
            StoredFileName: safeFileName,
            OriginalFileName: Path.GetFileName(file.FileName),
            ContentType: file.ContentType,
            FileSize: file.Length
        );
    }

    public Task<Stream?> GetInvoiceStreamAsync(string fileName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Task.FromResult<Stream?>(null);
        }

        // Prevent path traversal attacks
        var cleanFileName = Path.GetFileName(fileName);
        var fullPath = Path.Combine(_storageFolder, cleanFileName);

        if (!File.Exists(fullPath))
        {
            return Task.FromResult<Stream?>(null);
        }

        Stream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult<Stream?>(fileStream);
    }

    public Task<bool> DeleteInvoiceAsync(string? fileName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Task.FromResult(false);
        }

        try
        {
            var cleanFileName = Path.GetFileName(fileName);
            var fullPath = Path.Combine(_storageFolder, cleanFileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("Deleted physical invoice file {FileName}", cleanFileName);
                return Task.FromResult(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting invoice file {FileName}", fileName);
        }

        return Task.FromResult(false);
    }
}
