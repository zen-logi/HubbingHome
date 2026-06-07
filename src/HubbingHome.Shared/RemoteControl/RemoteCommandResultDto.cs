namespace HubbingHome.Shared.RemoteControl;

/// <summary>
/// リモコン操作要求の実行結果
/// </summary>
public sealed record RemoteCommandResultDto(
    bool Succeeded,
    string RoomId,
    string DeviceId,
    string CommandId,
    DateTimeOffset ExecutedAt,
    string Message);

