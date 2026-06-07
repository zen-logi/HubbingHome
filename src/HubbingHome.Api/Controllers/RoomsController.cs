using HubbingHome.Api.Services;
using HubbingHome.Shared.RemoteControl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HubbingHome.Api.Controllers;

/// <summary>
/// 部屋と操作可能コマンドを返すController
/// </summary>
[ApiController]
[Authorize]
[Route("api/rooms")]
public sealed class RoomsController(IRemoteControlCatalogService catalogService) : ControllerBase
{
    /// <summary>
    /// 操作可能な部屋一覧を取得
    /// </summary>
    [HttpGet]
    public ActionResult<IReadOnlyList<RemoteRoomDto>> GetRooms() => Ok(catalogService.GetRooms());
}
