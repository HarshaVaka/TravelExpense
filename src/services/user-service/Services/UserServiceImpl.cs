using Microsoft.AspNetCore.Identity;
using FluentValidation;
using UserService.Api.Dtos;
using UserService.Api.Models;
using UserService.Api.RabbitMq;
using UserService.Api.Repositories;

namespace UserService.Api.Services;

public class UserServiceImpl(IUserRepository repo, IPasswordHasher<User> hasher, IValidator<RegisterDto> validator,IRabbitMqPublisher publisher) : IUserService
{
    private readonly IUserRepository _repo = repo;
    private readonly IPasswordHasher<User> _hasher = hasher;
    private readonly IValidator<RegisterDto> _validator = validator;
    private readonly IRabbitMqPublisher _publisher = publisher;

    public async Task CreateUserAsync(RegisterDto register)
    {
        // validate using FluentValidation
        var validationResult = await _validator.ValidateAsync(register);
        if (!validationResult.IsValid)
        {
            // throw FluentValidation's ValidationException with failures
            throw new ValidationException(validationResult.Errors);
        }

        // check uniqueness
        if (await _repo.ExistsByEmailAsync(register.Email))
            throw new ValidationException([new FluentValidation.Results.ValidationFailure("Email", "Email is already registered")
            ]);

        if (await _repo.ExistsByUserNameAsync(register.UserName))
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure("UserName", "Username is already taken")
            ]);
        }

        User user = new()
        {
            UserName = register.UserName,
            Email = register.Email,
            PasswordHash = register.Password
        };

        if (!string.IsNullOrEmpty(user.PasswordHash))
        {
            user.PasswordHash = _hasher.HashPassword(user, user.PasswordHash);
        }

        var updatedUser =  await _repo.CreateAsync(user);
        
        //Write logic to send mail to validate
        _publisher.PublishUserRegistered(new
        {
            userId = updatedUser.Id,
            email = updatedUser.Email,
            userName = updatedUser.UserName
        });
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<UserProfileDto?> GetProfileAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null) return null;
        return new UserProfileDto(user.Id, user.UserName, user.Email, user.FirstName, user.LastName, user.Bio, user.AvatarUrl, user.CreatedAt, user.UpdatedAt);
    }

    public async Task<UserProfileDto?> UpdateProfileAsync(int id, UpdateUserDto update)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null) return null;

        // apply allowed updates
        if (!string.IsNullOrEmpty(update.UserName)) user.UserName = update.UserName;
        if (!string.IsNullOrEmpty(update.Email)) user.Email = update.Email;
        if (!string.IsNullOrEmpty(update.FirstName)) user.FirstName = update.FirstName;
        if (!string.IsNullOrEmpty(update.LastName)) user.LastName = update.LastName;
        if (!string.IsNullOrEmpty(update.Bio)) user.Bio = update.Bio;
        if (!string.IsNullOrEmpty(update.AvatarUrl)) user.AvatarUrl = update.AvatarUrl;

        var updated = await _repo.UpdateAsync(user);
        if (updated == null) return null;
        return new UserProfileDto(updated.Id, updated.UserName, updated.Email, updated.FirstName, updated.LastName, updated.Bio, updated.AvatarUrl, updated.CreatedAt, updated.UpdatedAt);
    }
}
