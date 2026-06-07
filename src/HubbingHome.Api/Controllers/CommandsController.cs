using HubbingHome.Api.Clients;
using HubbingHome.Api.Services;
using HubbingHome.Shared.RemoteControl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HubbingHome.Api.Controllers;

/// <summary>
/// リモコン操作を実行するController
/// </summary>
[ApiController]
[Authorize]
[Route("api/commands")]
public sealed class CommandsController(IRemoteCommandService commandService) : ControllerBase
{
    /// <summary>
    /// 許可済みリモコン操作を実行
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RemoteCommandResultDto>> ExecuteAsync(
        RemoteCommandRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await commandService.ExecuteAsync(request, cancellationToken));
        }
        catch (RemoteCommandValidationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (RemoteCommandDependencyException exception)
        {
            var statusCode = exception.FailureKind switch
            {
                HomeAssistantFailureKind.Timeout => StatusCodes.Status504GatewayTimeout,
                HomeAssistantFailureKind.Configuration => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status502BadGateway,
            };

            return StatusCode(statusCode, exception.Result);
        }
    }
}
