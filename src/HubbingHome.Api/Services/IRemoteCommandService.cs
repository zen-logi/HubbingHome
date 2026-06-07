using HubbingHome.Shared.RemoteControl;

namespace HubbingHome.Api.Services;

/// <summary>
/// リモコン操作実行サービス
/// </summary>
public interface IRemoteCommandService
{
    /// <summary>
    /// 許可済みリモコン操作を実行する
    /// </summary>
    Task<RemoteCommandResultDto> ExecuteAsync(RemoteCommandRequestDto request, CancellationToken cancellationToken);
}

