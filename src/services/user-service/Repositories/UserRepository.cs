using Microsoft.EntityFrameworkCore;
using UserService.Api.Data;
using UserService.Api.Models;

namespace UserService.Api.Repositories;

public class UserRepository(UserDbContext dbContext) : IUserRepository
{
    private readonly UserDbContext _dbContext = dbContext;

    public async Task AddAsync(User user)
    {
        // simple implementation; adjust as needed
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> UpdateAsync(User user)
    {
        var existing = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        if (existing == null) return null;

        existing.UserName = user.UserName;
        existing.Email = user.Email;
        // profile fields
        existing.FirstName = user.FirstName;
        existing.LastName = user.LastName;
        existing.Bio = user.Bio;
        existing.AvatarUrl = user.AvatarUrl;
        existing.UpdatedAt = DateTime.UtcNow;
        // do not update PasswordHash here unless explicitly intended elsewhere

        _dbContext.Users.Update(existing);
        await _dbContext.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email)) return false;
        return await _dbContext.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByUserNameAsync(string userName)
    {
        if (string.IsNullOrEmpty(userName)) return false;
        return await _dbContext.Users.AnyAsync(u => u.UserName == userName);
    }
}
