using System;
using UserService.Api.Models;

namespace UserService.Api.Services;

public interface IJwtService
{
    // generate access token (JWT) for the provided user
    string GenerateAccessToken(User user);

    // generate a new refresh token value (opaque token)
    string GenerateRefreshToken();

    // optional: validate refresh token format
    bool ValidateRefreshToken(string token);
}
