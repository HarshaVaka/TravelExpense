using UserService.Api.Models;
using UserService.Api.Repositories;
using UserService.Api.Services.Blob;
using Microsoft.AspNetCore.Http;

namespace UserService.Api.Services.Files;

public class UserFileService : IUserFileService
{
    private readonly IBlobService _blob;
    private readonly IUserFileRepository _repo;
    private readonly ILogger<UserFileService> _logger;

    public UserFileService(IBlobService blob, IUserFileRepository repo, ILogger<UserFileService> logger)
    {
        _blob = blob;
        _repo = repo;
        _logger = logger;
    }

    public async Task<UserFile> UploadAsync(int userId, IFormFile file, CancellationToken ct = default)
    {
        var container = "user-files";
        var blobName = $"user/{userId}/{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

        // Upload blob first
        try
        {
            using var stream = file.OpenReadStream();
            await _blob.UploadStreamAsync(container, blobName, stream, file.ContentType ?? "application/octet-stream", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob upload failed for user {UserId} file {FileName}", userId, file.FileName);
            throw; // bubble up
        }

        // Create DB record; if DB persist fails => attempt to delete blob and record cleanup if delete fails
        var userFile = new UserFile
        {
            UserId = userId,
            Container = container,
            BlobName = blobName,
            DisplayName = file.FileName,
            ContentType = file.ContentType ?? "application/octet-stream",
            Size = file.Length,
            UploadedAt = DateTimeOffset.UtcNow
        };

        try
        {
            var created = await _repo.AddAsync(userFile, ct);
            return created;
        }
        catch (Exception dbEx)
        {
            _logger.LogError(dbEx, "DB insert failed for uploaded blob {BlobName}, attempting cleanup", blobName);
            // attempt to delete blob as compensation
            try
            {
                // Instead of direct delete (not present), enqueue cleanup entry to try later
                await _repo.AddCleanupEntryAsync(new CleanupEntry { Container = container, BlobName = blobName }, ct);
            }
            catch (Exception cleanupEx)
            {
                _logger.LogError(cleanupEx, "Failed to enqueue cleanup entry for blob {BlobName}", blobName);
            }

            throw; // bubble original DB exception
        }
    }

    public Task<List<UserFile>> ListAsync(int userId, int skip, int take, CancellationToken ct = default)
    {
        return _repo.ListByUserAsync(userId, skip, take, ct);
    }

    public Task<UserFile?> GetAsync(int id, int userId, CancellationToken ct = default)
    {
        return _repo.GetByIdAsync(id, userId, ct);
    }

    public Task<Uri> GetDownloadUriAsync(UserFile file, int expiryHours = 1)
    {
        return _blob.GetReadUriAsync(file.Container, file.BlobName, expiryHours);
    }
}
