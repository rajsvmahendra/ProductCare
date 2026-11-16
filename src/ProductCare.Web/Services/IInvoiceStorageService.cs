using Microsoft.AspNetCore.Http;

namespace ProductCare.Web.Services;

public record InvoiceUploadResult(
    bool Success,
    string? StoredFileName = null,
    string? OriginalFileName = null,
    string? ContentType = null,
    long? FileSize = null,
    string? ErrorMessage = null
);

public interface IInvoiceStorageService
{
    /// <summary>
    /// Validates file size, extensions, MIME types, and magic bytes, and securely saves the file outside wwwroot.
    /// </summary>
    Task<InvoiceUploadResult> SaveInvoiceAsync(IFormFile? file, CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens a read-only stream for an authorized invoice download.
    /// </summary>
    Task<Stream?> GetInvoiceStreamAsync(string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an invoice file from disk when a product is removed or invoice replaced.
    /// </summary>
    Task<bool> DeleteInvoiceAsync(string? fileName, CancellationToken cancellationToken = default);
}
