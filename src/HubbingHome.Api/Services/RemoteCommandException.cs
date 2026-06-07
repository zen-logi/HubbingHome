using HubbingHome.Api.Clients;
using HubbingHome.Shared.RemoteControl;

namespace HubbingHome.Api.Services;

/// <summary>
/// リモコン操作サービスの検証失敗
/// </summary>
public sealed class RemoteCommandValidationException(string message) : Exception(message);

/// <summary>
/// リモコン操作で外部連携に失敗した
/// </summary>
public sealed class RemoteCommandDependencyException(
    string message,
    RemoteCommandResultDto result,
    HomeAssistantFailureKind failureKind,
    HomeAssistantClientException innerException)
    : Exception(message, innerException)
{
    /// <summary>
    /// クライアントへ返せる失敗結果
    /// </summary>
    public RemoteCommandResultDto Result { get; } = result;

    /// <summary>
    /// Home Assistant失敗分類
    /// </summary>
    public HomeAssistantFailureKind FailureKind { get; } = failureKind;
}
