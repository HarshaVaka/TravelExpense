using System;

namespace UserService.Api.Dtos;

public record RegisterDto(string UserName, string Email, string Password);
