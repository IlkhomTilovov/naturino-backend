using System.Text;
using System.Text.RegularExpressions;

namespace Naturino.Infrastructure.Common;

public static class SlugHelper
{
    public static string Generate(string value)
    {
        var normalized = value.ToLowerInvariant().Trim();
        normalized = Regex.Replace(normalized, @"[^a-z0-9\s-]", "");
        normalized = Regex.Replace(normalized, @"\s+", "-");
        normalized = Regex.Replace(normalized, "-+", "-").Trim('-');

        return string.IsNullOrWhiteSpace(normalized)
            ? Guid.NewGuid().ToString("N")[..8]
            : normalized;
    }
}
