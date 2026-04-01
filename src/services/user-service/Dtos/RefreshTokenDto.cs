using System;

namespace UserService.Api.Dtos;

public record RefreshTokenDto(string Token, DateTime ExpiresUtc);
