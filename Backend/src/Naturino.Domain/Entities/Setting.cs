using Naturino.Domain.Enums;

namespace Naturino.Domain.Entities;

public class Setting
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string GroupName { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
    public SettingValueType ValueType { get; set; } = SettingValueType.String;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
