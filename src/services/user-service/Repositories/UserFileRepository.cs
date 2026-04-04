using Microsoft.EntityFrameworkCore;
using UserService.Api.Data;
using UserService.Api.Models;

namespace UserService.Api.Repositories;

public class UserFileRepository : IUserFileRepository
{
    private readonly UserDbContext _db;

    public UserFileRepository(UserDbContext db)
    {
        _db = db;
    }

    public async Task<UserFile> AddAsync(UserFile file, CancellationToken ct = default)
    {
        var ent = (await _db.UserFiles.AddAsync(file, ct)).Entity;
        await _db.SaveChangesAsync(ct);
        return ent;
    }

    public async Task<UserFile?> GetByIdAsync(int id, int userId, CancellationToken ct = default)
    {
        return await _db.UserFiles.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId && !f.IsDeleted, ct);
    }

    public async Task<List<UserFile>> ListByUserAsync(int userId, int skip, int take, CancellationToken ct = default)
    {
        return await _db.UserFiles
            .Where(f => f.UserId == userId && !f.IsDeleted)
            .OrderByDescending(f => f.UploadedAt)
            .Skip(skip).Take(take)
            .ToListAsync(ct);
    }

    public async Task AddCleanupEntryAsync(CleanupEntry entry, CancellationToken ct = default)
    {
        await _db.CleanupEntries.AddAsync(entry, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<CleanupEntry>> GetPendingCleanupAsync(int max, CancellationToken ct = default)
    {
        return await _db.CleanupEntries.OrderBy(c => c.CreatedAt).Take(max).ToListAsync(ct);
    }

    public async Task RemoveCleanupEntryAsync(int id, CancellationToken ct = default)
    {
        var e = await _db.CleanupEntries.FindAsync(new object[] { id }, ct);
        if (e != null)
        {
            _db.CleanupEntries.Remove(e);
            await _db.SaveChangesAsync(ct);
        }
    }
}
