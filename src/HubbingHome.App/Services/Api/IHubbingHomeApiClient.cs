using HubbingHome.Shared.RemoteControl;

namespace HubbingHome.App.Services.Api;

/// <summary>
/// HubbingHome API クライアント
/// </summary>
public interface IHubbingHomeApiClient
{
    /// <summary>
    /// 操作可能な部屋一覧の取得
    /// </summary>
    /// <param name="cancellationToken">キャンセル通知</param>
    /// <returns>操作可能な部屋一覧</returns>
    Task<IReadOnlyList<RemoteRoomDto>> GetRoomsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 表示用状態の取得
    /// </summary>
    /// <param name="cancellationToken">キャンセル通知</param>
    /// <returns>表示用状態</returns>
    Task<RemoteStateDto> GetStateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// リモコン操作の実行
    /// </summary>
    /// <param name="request">操作要求</param>
    /// <param name="cancellationToken">キャンセル通知</param>
    /// <returns>操作結果</returns>
    Task<RemoteCommandResultDto> ExecuteCommandAsync(
        RemoteCommandRequestDto request,
        CancellationToken cancellationToken = default);
}
