using UserService.Api.Dtos;
using UserService.Api.Models;

namespace UserService.Api.Services.Files;

public interface IUserFileService
{
    Task<UserFile> UploadAsync(int userId, IFormFile file, CancellationToken ct = default);
    Task<List<UserFile>> ListAsync(int userId, int skip, int take, CancellationToken ct = default);
    Task<UserFile?> GetAsync(int id, int userId, CancellationToken ct = default);
    Task<Uri> GetDownloadUriAsync(UserFile file, int expiryHours = 1);
}
