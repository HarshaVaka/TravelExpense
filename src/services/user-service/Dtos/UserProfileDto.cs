using System;

namespace UserService.Api.Dtos;

public record UserProfileDto(int Id, string? UserName, string? Email, string? FirstName, string? LastName, string? Bio, string? AvatarUrl, DateTime CreatedAt, DateTime? UpdatedAt);
