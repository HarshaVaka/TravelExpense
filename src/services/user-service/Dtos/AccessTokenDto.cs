using System;

namespace UserService.Api.Dtos;

public record AccessTokenDto(string Token, DateTime ExpiresUtc);
