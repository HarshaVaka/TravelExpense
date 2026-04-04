using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace UserService.Api.Services.Blob;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _client;
    private readonly ILogger<BlobService> _logger;
    private readonly IConfiguration _config;

    public BlobService(BlobServiceClient client, IConfiguration config, ILogger<BlobService> logger)
    {
        _client = client;
        _logger = logger;
        _config = config;
    }

    public async Task<Uri> UploadUserFileAsync(int userId, string fileName, Stream content, string contentType, CancellationToken ct = default)
    {
        // container per user for simple isolation
        var containerName = _config.GetValue<string>("Blob:ContainerPrefix") ?? "user-files";
        var containerClient = _client.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();

        // consider putting under user/{userId}/filename
        var blobName = $"user/{userId}/{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        var blobClient = containerClient.GetBlobClient(blobName);

        var headers = new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = contentType };

        _logger.LogInformation("Uploading blob {BlobName} to container {Container}", blobName, containerName);
        await blobClient.UploadAsync(content, headers, cancellationToken: ct);

        // If an account key is provided in config we can generate a short-lived SAS so the URL is accessible
        var accountKey = _config.GetValue<string?>("Blob:AccountKey");
        var accountName = _config.GetValue<string?>("Blob:AccountName");

        if (!string.IsNullOrEmpty(accountKey) && !string.IsNullOrEmpty(accountName))
        {
            var credential = new StorageSharedKeyCredential(accountName, accountKey);
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
                Resource = "b"
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sas = sasBuilder.ToSasQueryParameters(credential).ToString();
            var uri = new UriBuilder(blobClient.Uri) { Query = sas }.Uri;
            return uri;
        }

        // otherwise return the blob URI (may require container to be public)
        return blobClient.Uri;
    }

    public async Task<string> UploadStreamAsync(string container, string blobName, Stream content, string contentType, CancellationToken ct = default)
    {
        var containerName = _config.GetValue<string>("Blob:ContainerPrefix") ?? "user-files";
        var containerClient = _client.GetBlobContainerClient(container);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        var blobClient = containerClient.GetBlobClient(blobName);
        var headers = new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = contentType };
        await blobClient.UploadAsync(content, headers, cancellationToken: ct);
        return blobName;
    }

    public Task<Uri> GetReadUriAsync(string container, string blobName, int expiryHours = 1)
    {
        var containerClient = _client.GetBlobContainerClient(container);
        var blobClient = containerClient.GetBlobClient(blobName);

        var accountKey = _config.GetValue<string?>("Blob:AccountKey");
        var accountName = _config.GetValue<string?>("Blob:AccountName");

        if (!string.IsNullOrEmpty(accountKey) && !string.IsNullOrEmpty(accountName))
        {
            var credential = new StorageSharedKeyCredential(accountName, accountKey);
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = container,
                BlobName = blobName,
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(expiryHours),
                Resource = "b"
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sas = sasBuilder.ToSasQueryParameters(credential).ToString();
            var uri = new UriBuilder(blobClient.Uri) { Query = sas }.Uri;
            return Task.FromResult(uri);
        }

        return Task.FromResult(blobClient.Uri);
    }
}
