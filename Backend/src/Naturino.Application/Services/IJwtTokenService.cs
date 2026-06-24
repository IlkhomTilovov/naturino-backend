using Naturino.Domain.Entities;

namespace Naturino.Application.Services;

public class GeneratedAccessToken
{
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
}

public interface IJwtTokenService
{
    GeneratedAccessToken GenerateAccessToken(User user, IEnumerable<string> roles);
    string GenerateRefreshToken();
    string HashToken(string token);
}
