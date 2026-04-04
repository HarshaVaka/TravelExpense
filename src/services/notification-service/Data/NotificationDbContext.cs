using Microsoft.EntityFrameworkCore;
using NotificationService.Api.Models;

namespace NotificationService.Api.Data;

public class NotificationDbContext(DbContextOptions<NotificationDbContext> options) : DbContext(options)
{
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<NotificationTemplate> Templates { get; set; } = null!;
    public DbSet<EmailQueue> EmailQueue { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Notification>(b =>
        {
            b.HasKey(n => n.Id);
            b.Property(n => n.Channel).HasMaxLength(50);
            b.Property(n => n.Status).HasMaxLength(50);
            b.Property(n => n.Recipient).HasMaxLength(500);
        });

        modelBuilder.Entity<NotificationTemplate>(b =>
        {
            b.HasKey(t => t.Id);
            b.Property(t => t.Name).IsRequired().HasMaxLength(200);
            b.Property(t => t.Subject).IsRequired().HasMaxLength(500);
        });

        modelBuilder.Entity<EmailQueue>(b =>
        {
            b.HasKey(q => q.Id);
            b.Property(q => q.To).IsRequired().HasMaxLength(500);
            b.Property(q => q.Subject).IsRequired().HasMaxLength(500);
            b.Property(q => q.Status).HasMaxLength(50);
        });
    }
}
