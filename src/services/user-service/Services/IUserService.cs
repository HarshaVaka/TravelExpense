using UserService.Api.Dtos;
using UserService.Api.Models;

namespace UserService.Api.Services;

public interface IUserService
{
    Task CreateUserAsync(RegisterDto user);
    Task<User?> GetByIdAsync(int id);

    // profile helpers
    Task<UserProfileDto?> GetProfileAsync(int id);
    Task<UserProfileDto?> UpdateProfileAsync(int id, UpdateUserDto update);
}
