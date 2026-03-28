using Microsoft.AspNetCore.Mvc;
using UserService.Api.Dtos;
using UserService.Api.Models;
using UserService.Api.Services;

namespace UserService.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    // POST api/v1.0/user/register
    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] RegisterDto user)
    {
        var created = await _userService.CreateUserAsync(user);
        return CreatedAtAction(nameof(GetUser), new { id = created.Id }, created);
    }

    // GET api/v1.0/user/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        // Implement login logic here, e.g., validate credentials, generate JWT, etc.
        // For now, just return a placeholder response
        return Ok(new { Message = "Login endpoint not implemented yet." });
    }   
}
