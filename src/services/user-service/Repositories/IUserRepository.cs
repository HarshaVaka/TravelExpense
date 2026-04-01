using UserService.Api.Models;

namespace UserService.Api.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User> CreateAsync(User user);
    Task<User?> GetByIdAsync(int id);
    Task<User?> UpdateAsync(User user);
    // existence checks
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUserNameAsync(string userName);
}
