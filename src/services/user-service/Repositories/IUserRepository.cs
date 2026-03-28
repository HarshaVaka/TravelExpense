using UserService.Api.Models;

namespace UserService.Api.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User> CreateAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
}
