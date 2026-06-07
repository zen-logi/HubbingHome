namespace HubbingHome.Shared.RemoteControl;

/// <summary>
/// 部屋に配置された操作対象機器
/// </summary>
public sealed record RemoteDeviceDto(
    string Id,
    string Name,
    string Kind,
    IReadOnlyList<RemoteCommandDto> Commands);

