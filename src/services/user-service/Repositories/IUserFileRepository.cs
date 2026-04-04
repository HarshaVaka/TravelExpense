using UserService.Api.Models;

namespace UserService.Api.Repositories;

public interface IUserFileRepository
{
    Task<UserFile> AddAsync(UserFile file, CancellationToken ct = default);
    Task<UserFile?> GetByIdAsync(int id, int userId, CancellationToken ct = default);
    Task<List<UserFile>> ListByUserAsync(int userId, int skip, int take, CancellationToken ct = default);
    Task AddCleanupEntryAsync(CleanupEntry entry, CancellationToken ct = default);
    Task<List<CleanupEntry>> GetPendingCleanupAsync(int max, CancellationToken ct = default);
    Task RemoveCleanupEntryAsync(int id, CancellationToken ct = default);
}
