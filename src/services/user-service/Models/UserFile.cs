namespace UserService.Api.Models;

public class UserFile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Container { get; set; } = "user-files";
    public string BlobName { get; set; } = string.Empty; // e.g. user/{userId}/{guid}_{filename}
    public string DisplayName { get; set; } = string.Empty; // original file name
    public string ContentType { get; set; } = "application/octet-stream";
    public long Size { get; set; }
    public string? ETag { get; set; }
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsDeleted { get; set; }
    public string? MetadataJson { get; set; }
}
