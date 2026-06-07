using HubbingHome.Api.Services;
using HubbingHome.Shared.RemoteControl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HubbingHome.Api.Controllers;

/// <summary>
/// アプリ表示用状態を返すController
/// </summary>
[ApiController]
[Authorize]
[Route("api/state")]
public sealed class StateController(IRemoteControlStateService stateService) : ControllerBase
{
    /// <summary>
    /// 最終操作状態を取得
    /// </summary>
    [HttpGet]
    public ActionResult<RemoteStateDto> GetState() => Ok(stateService.GetState());
}
