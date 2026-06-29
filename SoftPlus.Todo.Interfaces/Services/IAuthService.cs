using SoftPlus.Todo.Interfaces.DTOs.Auth;

namespace SoftPlus.Todo.Interfaces.Services;

public interface IAuthService
{
    public Task<(bool Success, string? ErrorMessage, AuthResponseDto? Data)> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken = default);

    public Task<(bool Success, string? ErrorMessage, AuthResponseDto? Data)> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken = default);
}