using System;

namespace NotificationService.Api.Models;

// EmailQueue holds outbound email messages (useful if you want to retry sending, inspect failures, etc.)
public class EmailQueue
{
    public int Id { get; set; }
    public string To { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
    public DateTime EnqueuedAt { get; set; } = DateTime.UtcNow;
    public int Attempts { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Sent, Failed
    public DateTime? SentAt { get; set; }
}
