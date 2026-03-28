namespace UserService.Api.Models;

public class User
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    // Store the password hash (not plain text)
    public string? PasswordHash { get; set; }
}
