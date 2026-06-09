using HubbingHome.Api.Clients;
using HubbingHome.Api.Options;
using HubbingHome.Shared.RemoteControl;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace HubbingHome.Api.Services;

/// <summary>
/// 許可済み設定に基づいてHome Assistantへリモコン操作を送信するサービス
/// </summary>
public sealed class RemoteCommandService(
    IOptions<RemoteControlOptions> options,
    IHomeAssistantClient homeAssistantClient,
    IRemoteControlStateService stateService,
    TimeProvider timeProvider,
    ILogger<RemoteCommandService> logger) : IRemoteCommandService
{
    /// <inheritdoc />
    public async Task<RemoteCommandResultDto> ExecuteAsync(RemoteCommandRequestDto request, CancellationToken cancellationToken)
    {
        var (room, device, command) = FindAllowedCommand(request);
        var data = CreateServiceData(command, request);

        logger.LogInformation(
            "Remote command requested. RoomId={RoomId}, DeviceId={DeviceId}, CommandId={CommandId}",
            room.Id,
            device.Id,
            command.Id);

        try
        {
            await homeAssistantClient.CallServiceAsync(
                new HomeAssistantServiceRequest(
                    command.ServiceDomain,
                    command.ServiceName,
                    room.RemoteEntityId,
                    data),
                cancellationToken);
        }
        catch (HomeAssistantClientException exception)
        {
            var failedResult = new RemoteCommandResultDto(
                false,
                room.Id,
                device.Id,
                command.Id,
                timeProvider.GetUtcNow(),
                "Home Assistant連携に失敗しました");

            stateService.Record(failedResult);

            logger.LogWarning(
                exception,
                "Home Assistant command failed. FailureKind={FailureKind}, StatusCode={StatusCode}, RequestUri={RequestUri}, Domain={Domain}, Service={Service}, EntityId={EntityId}, ResponseSnippet={ResponseSnippet}",
                exception.FailureKind,
                exception.StatusCode,
                exception.RequestUri,
                exception.Domain,
                exception.Service,
                exception.EntityId,
                exception.ResponseSnippet);

            throw new RemoteCommandDependencyException(
                "Home Assistant連携に失敗しました",
                failedResult,
                exception.FailureKind,
                exception);
        }

        var result = new RemoteCommandResultDto(
            true,
            room.Id,
            device.Id,
            command.Id,
            timeProvider.GetUtcNow(),
            $"{room.Name}/{device.Name}/{command.Name} を送信");

        stateService.Record(result);

        logger.LogInformation(
            "Remote command sent. RoomId={RoomId}, DeviceId={DeviceId}, CommandId={CommandId}",
            room.Id,
            device.Id,
            command.Id);

        return result;
    }

    /// <summary>
    /// allowlistに存在するコマンドを検索
    /// </summary>
    private (RemoteRoomOptions Room, RemoteDeviceOptions Device, RemoteCommandOptions Command) FindAllowedCommand(
        RemoteCommandRequestDto request)
    {
        var room = options.Value.Rooms.SingleOrDefault(x => x.Id == request.RoomId)
            ?? throw new RemoteCommandValidationException("Unknown room id");

        var device = room.Devices.SingleOrDefault(x => x.Id == request.DeviceId)
            ?? throw new RemoteCommandValidationException("Unknown device id");

        var command = device.Commands.SingleOrDefault(x => x.Id == request.CommandId)
            ?? throw new RemoteCommandValidationException("Unknown command id");

        return (room, device, command);
    }

    /// <summary>
    /// Home Assistantへ送信するサービスデータを作成
    /// </summary>
    private static Dictionary<string, object?> CreateServiceData(
        RemoteCommandOptions command,
        RemoteCommandRequestDto request)
    {
        var homeAssistantCommand = ResolveHomeAssistantCommand(command, request);
        var data = new Dictionary<string, object?>(command.Data)
        {
            ["command"] = homeAssistantCommand,
        };

        return data;
    }

    /// <summary>
    /// 固定設定または許可済みリクエストパラメータからHome Assistantコマンド名を解決
    /// </summary>
    private static string ResolveHomeAssistantCommand(
        RemoteCommandOptions command,
        RemoteCommandRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(command.HomeAssistantCommandParameter))
        {
            return command.HomeAssistantCommand;
        }

        if (request.Parameters is null ||
            !request.Parameters.TryGetValue(command.HomeAssistantCommandParameter, out var requestedCommand) ||
            string.IsNullOrWhiteSpace(requestedCommand))
        {
            throw new RemoteCommandValidationException("Home Assistant command parameter is required");
        }

        if (string.IsNullOrWhiteSpace(command.HomeAssistantCommandPattern) ||
            !Regex.IsMatch(
                requestedCommand,
                command.HomeAssistantCommandPattern,
                RegexOptions.CultureInvariant,
                TimeSpan.FromMilliseconds(100)))
        {
            throw new RemoteCommandValidationException("Home Assistant command parameter is not allowed");
        }

        if (command.HomeAssistantCommandGenerator == "daikin_air_conditioner")
        {
            return DaikinAirConditionerBroadlinkCommandGenerator.Generate(
                requestedCommand,
                command.DaikinAirConditionerCode ?? new DaikinAirConditionerCodeOptions());
        }

        return requestedCommand;
    }
}
