using System;

namespace NotificationService.Api.Models;

public class Notification
{
    public int Id { get; set; }
    public string? Message { get; set; }
    public DateTime SentAt { get; set; }
    // channel could be Email, SMS, Push, etc.
    public string Channel { get; set; } = "Email";
    // optional recipient identifier (email address, phone, user id)
    public string? Recipient { get; set; }
    // delivery status: Pending, Sent, Failed
    public string Status { get; set; } = "Pending";
}
