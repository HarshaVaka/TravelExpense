using System;

namespace UserService.Api.Dtos;

public record LoginDto(string UserNameOrEmail, string Password);
