using System;
using System.Security.Cryptography;
using UserService.Api.Models;

namespace UserService.Api.Services;

// Skeleton Jwt service - implement token creation logic yourself
public class JwtService(IConfiguration config) : IJwtService
{
    private readonly IConfiguration _config = config;

    public string GenerateAccessToken(User user)
    {
        // TODO: implement JWT creation using configuration (signing key, claims, expiry)
        // Returning placeholder token for now
        return "ACCESS_TOKEN_PLACEHOLDER";
    }

    public string GenerateRefreshToken()
    {
        // generate a secure random string
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    public bool ValidateRefreshToken(string token)
    {
        // placeholder - you can add format/length checks, TTL checks etc.
        return !string.IsNullOrWhiteSpace(token);
    }
}
