using System;
using UserService.Api.Models;

namespace UserService.Api.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task RevokeAsync(Guid id, DateTime revokedAt);
}
