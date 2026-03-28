using Microsoft.EntityFrameworkCore;
using UserService.Api.Data;
using UserService.Api.Models;

namespace UserService.Api.Repositories;

public class RefreshTokenRepository(UserDbContext dbContext) : IRefreshTokenRepository
{
    private readonly UserDbContext _dbContext = dbContext;

    public async Task AddAsync(RefreshToken token)
    {
        _dbContext.RefreshTokens.Add(token);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _dbContext.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
    }

    public async Task RevokeAsync(Guid id, DateTime revokedAt)
    {
        var t = await _dbContext.RefreshTokens.FirstOrDefaultAsync(r => r.Id == id);
        if (t == null) return;
        t.RevokedAt = revokedAt;
        await _dbContext.SaveChangesAsync();
    }
}
