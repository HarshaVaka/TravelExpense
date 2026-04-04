using Microsoft.AspNetCore.Http;

namespace UserService.Api.Services.Blob;

public interface IBlobService
{
    /// <summary>
    /// Uploads the provided stream as a blob and returns an accessible URI (may include SAS if configured).
    /// </summary>
    Task<Uri> UploadUserFileAsync(int userId, string fileName, Stream content, string contentType, CancellationToken ct = default);

    /// <summary>
    /// Upload a stream to a specific container/blob name and return the blob name (or an identifier).
    /// </summary>
    Task<string> UploadStreamAsync(string container, string blobName, Stream content, string contentType, CancellationToken ct = default);

    /// <summary>
    /// Generate a read URI (SAS) for the given container/blob. If SAS cannot be generated, returns blob URI which may be public.
    /// </summary>
    Task<Uri> GetReadUriAsync(string container, string blobName, int expiryHours = 1);
}
