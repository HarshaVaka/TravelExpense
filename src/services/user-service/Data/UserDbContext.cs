using Microsoft.EntityFrameworkCore;

namespace UserService.Api.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options), DbContext
{
    public DbSet<User> Users { get; set; } = null!;
}

public class User
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
}
