using Microsoft.EntityFrameworkCore;
using UserService.Api.Models;

namespace UserService.Api.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;

    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<UserFile> UserFiles { get; set; } = null!;
    public DbSet<CleanupEntry> CleanupEntries { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.UserName).HasMaxLength(200);
            b.Property(u => u.Email).HasMaxLength(300);
            // ensure uniqueness at the database level to avoid race conditions
            b.HasIndex(u => u.Email).IsUnique();
            b.HasIndex(u => u.UserName).IsUnique();
        });

        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.HasKey(r => r.Id);
            b.Property(r => r.Token).IsRequired();
            b.HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserFile>(b =>
        {
            b.HasKey(f => f.Id);
            b.Property(f => f.BlobName).IsRequired();
            b.Property(f => f.DisplayName).HasMaxLength(500);
            b.HasIndex(f => new { f.UserId, f.UploadedAt });
        });

        modelBuilder.Entity<CleanupEntry>(b =>
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.BlobName).IsRequired();
        });
    }
}
