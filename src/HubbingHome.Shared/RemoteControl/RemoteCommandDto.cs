namespace HubbingHome.Shared.RemoteControl;

/// <summary>
/// リモコン操作コマンドの表示情報
/// </summary>
public sealed record RemoteCommandDto(
    string Id,
    string Name,
    string Kind,
    string? Icon,
    IReadOnlyDictionary<string, string>? Parameters);

