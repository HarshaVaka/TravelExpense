using Microsoft.AspNetCore.Mvc;
using UserService.Api.Dtos;
using UserService.Api.Models;
using UserService.Api.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace UserService.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    // POST api/v1.0/user/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto user)
    {
        var created = await _userService.CreateUserAsync(user);
        return CreatedAtAction(nameof(GetMyProfile), new { id = created.Id }, created);
    }
    // GET api/v1.0/user/me
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetMyProfile()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (idClaim == null) return Unauthorized();
        if (!int.TryParse(idClaim.Value, out var id)) return Unauthorized();

        var profile = await _userService.GetProfileAsync(id);
        if (profile == null) return NotFound();
        return Ok(profile);
    }

    // PUT api/v1.0/user/me
    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserDto update)
    {
    var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
    if (idClaim == null) return Unauthorized();
    if (!int.TryParse(idClaim.Value, out var id)) return Unauthorized();

    var updated = await _userService.UpdateProfileAsync(id, update);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        // Implement login logic here, e.g., validate credentials, generate JWT, etc.
        // For now, just return a placeholder response
        return Ok(new { Message = "Login endpoint not implemented yet." });
    }   
}
