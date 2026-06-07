namespace HubbingHome.Shared.RemoteControl;

/// <summary>
/// アプリ表示用の最終操作状態
/// </summary>
public sealed record RemoteStateDto(IReadOnlyList<RemoteCommandResultDto> RecentCommands);

