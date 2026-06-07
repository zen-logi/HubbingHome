namespace HubbingHome.Shared.RemoteControl;

/// <summary>
/// スマートリモコンを配置した部屋
/// </summary>
public sealed record RemoteRoomDto(
    string Id,
    string Name,
    IReadOnlyList<RemoteDeviceDto> Devices);
