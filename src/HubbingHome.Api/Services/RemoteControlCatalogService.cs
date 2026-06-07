using HubbingHome.Api.Options;
using HubbingHome.Shared.RemoteControl;
using Microsoft.Extensions.Options;

namespace HubbingHome.Api.Services;

/// <summary>
/// 設定値からリモコン構成を返すカタログサービス
/// </summary>
public sealed class RemoteControlCatalogService(IOptions<RemoteControlOptions> options) : IRemoteControlCatalogService
{
    /// <inheritdoc />
    public IReadOnlyList<RemoteRoomDto> GetRooms() =>
        options.Value.Rooms
            .Select(room => new RemoteRoomDto(
                room.Id,
                room.Name,
                room.Devices.Select(device => new RemoteDeviceDto(
                    device.Id,
                    device.Name,
                    device.Kind,
                    device.Commands.Select(command => new RemoteCommandDto(
                        command.Id,
                        command.Name,
                        command.Kind,
                        command.Icon,
                        command.Parameters is null
                            ? null
                            : new Dictionary<string, string>(command.Parameters))).ToArray())).ToArray()))
            .ToArray();
}
