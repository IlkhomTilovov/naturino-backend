using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Naturino.Application.DTOs.Auth;
using Naturino.Application.Services;
using Naturino.Domain.Entities;
using Naturino.Domain.Exceptions;
using Naturino.Infrastructure.Persistence;

namespace Naturino.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponseDto> LoginAsync(string email, string password, string? ipAddress, CancellationToken ct = default)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user is null || !_passwordHasher.Verify(password, user.PasswordHash))
        {
            throw new UnauthorizedException("Email yoki parol noto'g'ri.");
        }

        if (!user.IsActive)
        {
            throw new ForbiddenException("Foydalanuvchi bloklangan.");
        }

        user.LastLoginAt = DateTimeOffset.UtcNow;
        var response = await IssueTokensAsync(user, ipAddress, ct);
        await _context.SaveChangesAsync(ct);

        return response;
    }

    public async Task<LoginResponseDto> RefreshAsync(string refreshToken, string? ipAddress, CancellationToken ct = default)
    {
        var tokenHash = _jwtTokenService.HashToken(refreshToken);

        var existing = await _context.RefreshTokens
            .Include(rt => rt.User).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

        if (existing is null || existing.RevokedAt is not null || existing.ExpiresAt < DateTimeOffset.UtcNow)
        {
            throw new ForbiddenException("Refresh token noto'g'ri yoki muddati o'tgan.");
        }

        existing.RevokedAt = DateTimeOffset.UtcNow;

        var response = await IssueTokensAsync(existing.User, ipAddress, ct);
        existing.ReplacedByHash = _jwtTokenService.HashToken(response.RefreshToken);

        await _context.SaveChangesAsync(ct);

        return response;
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        var tokenHash = _jwtTokenService.HashToken(refreshToken);
        var existing = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

        if (existing is not null && existing.RevokedAt is null)
        {
            existing.RevokedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync(ct);
        }
    }

    private async Task<LoginResponseDto> IssueTokensAsync(User user, string? ipAddress, CancellationToken ct)
    {
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _jwtTokenService.HashToken(refreshTokenValue),
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),
            CreatedByIp = ipAddress
        });

        return new LoginResponseDto
        {
            AccessToken = accessToken.Token,
            RefreshToken = refreshTokenValue,
            ExpiresAt = accessToken.ExpiresAt,
            User = new CurrentUserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles
            }
        };
    }
}
