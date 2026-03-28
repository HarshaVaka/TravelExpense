using UserService.Api.Dtos;
using UserService.Api.Models;

namespace UserService.Api.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(RegisterDto user);
    Task<User?> GetByIdAsync(Guid id);
}
