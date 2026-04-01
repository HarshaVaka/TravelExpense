using System;

namespace UserService.Api.Models;

public class RefreshToken
{
    public Guid Id { get; set; }
    // the token string
    public string Token { get; set; } = null!;
    // which user this belongs to (matches User.Id)
    public int UserId { get; set; }
    public User? User { get; set; }
    // when it was created
    public DateTime CreatedAt { get; set; }
    // optional revocation time; null = active
    public DateTime? RevokedAt { get; set; }
    // expiration
    public DateTime ExpiresAt { get; set; }
}
