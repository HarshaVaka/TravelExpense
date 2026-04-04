using System;

namespace NotificationService.Api.Models;

// Templates for email (or other channels). Keep simple for now: subject + body (could be HTML).
public class NotificationTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = null!; // e.g. "WelcomeEmail"
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!; // template body with placeholders e.g. {{UserName}}
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
