using HubbingHome.Shared.RemoteControl;

namespace HubbingHome.Api.Services;

/// <summary>
/// リモコン操作状態サービス
/// </summary>
public interface IRemoteControlStateService
{
    /// <summary>
    /// 表示用の最終操作状態を取得する
    /// </summary>
    RemoteStateDto GetState();

    /// <summary>
    /// 操作結果を記録する
    /// </summary>
    void Record(RemoteCommandResultDto result);
}

