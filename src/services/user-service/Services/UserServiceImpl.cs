using Microsoft.AspNetCore.Identity;
using UserService.Api.Dtos;
using UserService.Api.Models;
using UserService.Api.Repositories;

namespace UserService.Api.Services;

public class UserServiceImpl(IUserRepository repo, IPasswordHasher<User> hasher) : IUserService
{
    private readonly IUserRepository _repo = repo;
    private readonly IPasswordHasher<User> _hasher = hasher;

    public async Task<User> CreateUserAsync(RegisterDto register)
    {
        User user = new()
        {
            Id = Guid.NewGuid(),
            UserName = register.UserName,
            Email = register.Email,
            PasswordHash = register.Password
        };
        if (!string.IsNullOrEmpty(user.PasswordHash))
        {
            user.PasswordHash = _hasher.HashPassword(user, user.PasswordHash);
        }

        return await _repo.CreateAsync(user);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _repo.GetByIdAsync(id);
    }
}
