using Microsoft.EntityFrameworkCore;

namespace NotificationService.Api.Data;

public class NotificationDbContext(DbContextOptions<NotificationDbContext> options) : DbContext(options)
{
    public DbSet<Notification> Notifications { get; set; } = null!;
}

public class Notification
{
    public int Id { get; set; }
    public string? Message { get; set; }
    public DateTime SentAt { get; set; }
}
