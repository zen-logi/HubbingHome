namespace HubbingHome.Shared.RemoteControl;

/// <summary>
/// クライアントから送信されるリモコン操作要求
/// </summary>
public sealed record RemoteCommandRequestDto(
    string RoomId,
    string DeviceId,
    string CommandId,
    IReadOnlyDictionary<string, string>? Parameters = null);
