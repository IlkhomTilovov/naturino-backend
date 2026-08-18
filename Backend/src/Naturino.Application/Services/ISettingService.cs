namespace Naturino.Application.Services;

public interface ISettingService
{
    Task<Dictionary<string, string>> GetGroupAsync(string groupName, CancellationToken ct = default);
    Task<Dictionary<string, string>> UpdateGroupAsync(string groupName, Dictionary<string, string> values, CancellationToken ct = default);
}
