using Microsoft.EntityFrameworkCore;
using Naturino.Application.Services;
using Naturino.Domain.Entities;
using Naturino.Infrastructure.Persistence;

namespace Naturino.Infrastructure.Repositories;

public class SettingService : ISettingService
{
    private readonly ApplicationDbContext _context;

    public SettingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Dictionary<string, string>> GetGroupAsync(string groupName, CancellationToken ct = default)
    {
        var settings = await _context.Settings.Where(s => s.GroupName == groupName).ToListAsync(ct);
        return settings.ToDictionary(s => s.Key, s => s.Value ?? string.Empty);
    }

    public async Task<Dictionary<string, string>> UpdateGroupAsync(string groupName, Dictionary<string, string> values, CancellationToken ct = default)
    {
        var existing = await _context.Settings.Where(s => s.GroupName == groupName).ToListAsync(ct);

        foreach (var (key, value) in values)
        {
            var setting = existing.FirstOrDefault(s => s.Key == key);
            if (setting is null)
            {
                setting = new Setting { GroupName = groupName, Key = key };
                _context.Settings.Add(setting);
            }

            setting.Value = value;
            setting.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await _context.SaveChangesAsync(ct);
        return await GetGroupAsync(groupName, ct);
    }
}
