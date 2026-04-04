using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserService.Api.Services.Files;
using System.Security.Claims;

namespace UserService.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IUserFileService _files;

    public FilesController(IUserFileService files)
    {
        _files = files;
    }

    [HttpPost("me/files")]
    [Authorize]
    public async Task<IActionResult> UploadMyFile([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest(new { message = "File is required" });

        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (idClaim == null) return Unauthorized();
        if (!int.TryParse(idClaim.Value, out var id)) return Unauthorized();

        var created = await _files.UploadAsync(id, file);
        return CreatedAtAction(nameof(GetFile), new { id = created.Id }, new { id = created.Id, created.DisplayName, created.UploadedAt, created.Size });
    }

    [HttpGet("me/files")]
    [Authorize]
    public async Task<IActionResult> ListMyFiles([FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (idClaim == null) return Unauthorized();
        if (!int.TryParse(idClaim.Value, out var id)) return Unauthorized();

        var list = await _files.ListAsync(id, skip, take);
        return Ok(list);
    }

    [HttpGet("me/files/{id}")]
    [Authorize]
    public async Task<IActionResult> GetFile(int id)
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (idClaim == null) return Unauthorized();
        if (!int.TryParse(idClaim.Value, out var userId)) return Unauthorized();

        var file = await _files.GetAsync(id, userId);
        if (file == null) return NotFound();

        var uri = await _files.GetDownloadUriAsync(file);
        return Ok(new { id = file.Id, file.DisplayName, file.Size, file.ContentType, url = uri.ToString(), file.UploadedAt });
    }
}
