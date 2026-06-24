using Naturino.Application.DTOs.Auth;

namespace Naturino.Application.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(string email, string password, string? ipAddress, CancellationToken ct = default);
    Task<LoginResponseDto> RefreshAsync(string refreshToken, string? ipAddress, CancellationToken ct = default);
    Task LogoutAsync(string refreshToken, CancellationToken ct = default);
}
