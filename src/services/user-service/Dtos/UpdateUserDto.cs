using System;

namespace UserService.Api.Dtos;

public record UpdateUserDto(string? UserName, string? Email, string? FirstName, string? LastName, string? Bio, string? AvatarUrl);
