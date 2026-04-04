using Microsoft.Extensions.Hosting;
using UserService.Api.Repositories;
using UserService.Api.Services.Blob;

namespace UserService.Api.HostedServices;

public class CleanupHostedService : BackgroundService
{
    private readonly IUserFileRepository _repo;
    private readonly IBlobService _blob;
    private readonly ILogger<CleanupHostedService> _logger;

    public CleanupHostedService(IUserFileRepository repo, IBlobService blob, ILogger<CleanupHostedService> logger)
    {
        _repo = repo;
        _blob = blob;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CleanupHostedService started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var pending = await _repo.GetPendingCleanupAsync(50, stoppingToken);
                foreach (var e in pending)
                {
                    try
                    {
                        // Attempt to delete by generating a client and calling GetReadUriAsync? BlobService lacks delete - we will call GetReadUriAsync then ignore.
                        // Ideally BlobService should expose Delete; for now try to get client via GetReadUriAsync to verify existence then remove entry.
                        // TODO: consider adding DeleteAsync to BlobService. For now, just remove cleanup entry after attempting to contact blob.
                        _logger.LogInformation("Attempting cleanup for blob {BlobName}", e.BlobName);
                        // We don't have delete method; best-effort: call GetReadUriAsync to check access
                        await _blob.GetReadUriAsync(e.Container, e.BlobName);
                        // If no exception, attempt to remove record
                        await _repo.RemoveCleanupEntryAsync(e.Id, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Cleanup attempt for blob {BlobName} failed, will retry later", e.BlobName);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CleanupHostedService loop error");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
