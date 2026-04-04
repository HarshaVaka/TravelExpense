namespace UserService.Api.Models;

public class CleanupEntry
{
    public int Id { get; set; }
    public string Container { get; set; } = "user-files";
    public string BlobName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public int RetryCount { get; set; } = 0;
}
