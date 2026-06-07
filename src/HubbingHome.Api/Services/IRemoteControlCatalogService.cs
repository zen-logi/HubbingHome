using HubbingHome.Shared.RemoteControl;

namespace HubbingHome.Api.Services;

/// <summary>
/// リモコン構成カタログサービス
/// </summary>
public interface IRemoteControlCatalogService
{
    /// <summary>
    /// 操作可能な部屋一覧を取得する
    /// </summary>
    IReadOnlyList<RemoteRoomDto> GetRooms();
}

