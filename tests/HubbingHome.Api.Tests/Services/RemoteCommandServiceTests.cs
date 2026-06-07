using HubbingHome.Api.Clients;
using HubbingHome.Api.Options;
using HubbingHome.Api.Services;
using HubbingHome.Shared.RemoteControl;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace HubbingHome.Api.Tests.Services;

/// <summary>
/// リモコン操作サービスのテスト
/// </summary>
public sealed class RemoteCommandServiceTests
{
    [Test]
    public async Task ExecuteAsync_許可済みコマンドをHomeAssistantへ送信して状態を記録()
    {
        var client = new RecordingHomeAssistantClient();
        var stateService = new RemoteControlStateService();
        var service = CreateService(client, stateService);

        var result = await service.ExecuteAsync(
            new RemoteCommandRequestDto("living", "light", "on"),
            CancellationToken.None);

        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(client.LastRequest?.Data["command"]).IsEqualTo("living_light_on");
        await Assert.That(client.LastRequest?.EntityId).IsEqualTo("remote.living_broadlink");
        await Assert.That(stateService.GetState().RecentCommands.Count).IsEqualTo(1);
    }

    [Test]
    public async Task ExecuteAsync_未許可コマンドは拒否()
    {
        var service = CreateService(new RecordingHomeAssistantClient(), new RemoteControlStateService());

        await Assert.That(async () =>
            await service.ExecuteAsync(
                new RemoteCommandRequestDto("living", "light", "unknown"),
                CancellationToken.None))
            .Throws<RemoteCommandValidationException>();
    }

    [Test]
    public async Task ExecuteAsync_HomeAssistant失敗時も失敗結果を記録()
    {
        var stateService = new RemoteControlStateService();
        var service = CreateService(
            new FailingHomeAssistantClient(HomeAssistantFailureKind.Timeout),
            stateService);

        await Assert.That(async () =>
            await service.ExecuteAsync(
                new RemoteCommandRequestDto("living", "light", "on"),
                CancellationToken.None))
            .Throws<RemoteCommandDependencyException>();

        var state = stateService.GetState();
        await Assert.That(state.RecentCommands.Count).IsEqualTo(1);
        await Assert.That(state.RecentCommands[0].Succeeded).IsFalse();
        await Assert.That(state.RecentCommands[0].RoomId).IsEqualTo("living");
    }

    /// <summary>
    /// テスト用サービスを作成
    /// </summary>
    private static RemoteCommandService CreateService(
        IHomeAssistantClient client,
        IRemoteControlStateService stateService)
    {
        return new RemoteCommandService(
            Microsoft.Extensions.Options.Options.Create(CreateOptions()),
            client,
            stateService,
            TimeProvider.System,
            NullLogger<RemoteCommandService>.Instance);
    }

    /// <summary>
    /// テスト用リモコン構成を作成
    /// </summary>
    private static RemoteControlOptions CreateOptions() =>
        new()
        {
            Rooms =
            [
                new RemoteRoomOptions
                {
                    Id = "living",
                    Name = "リビング",
                    RemoteEntityId = "remote.living_broadlink",
                    Devices =
                    [
                        new RemoteDeviceOptions
                        {
                            Id = "light",
                            Name = "照明",
                            Kind = "light",
                            Commands =
                            [
                                new RemoteCommandOptions
                                {
                                    Id = "on",
                                    Name = "点灯",
                                    Kind = "power",
                                    HomeAssistantCommand = "living_light_on",
                                },
                            ],
                        },
                    ],
                },
            ],
        };

    /// <summary>
    /// Home Assistant要求を記録するテスト用クライアント
    /// </summary>
    private sealed class RecordingHomeAssistantClient : IHomeAssistantClient
    {
        /// <summary>
        /// 最後に送信された要求
        /// </summary>
        public HomeAssistantServiceRequest? LastRequest { get; private set; }

        /// <inheritdoc />
        public Task CallServiceAsync(HomeAssistantServiceRequest request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Home Assistant失敗を返すテスト用クライアント
    /// </summary>
    private sealed class FailingHomeAssistantClient(HomeAssistantFailureKind failureKind) : IHomeAssistantClient
    {
        /// <inheritdoc />
        public Task CallServiceAsync(HomeAssistantServiceRequest request, CancellationToken cancellationToken)
        {
            throw new HomeAssistantClientException(
                failureKind,
                request.Domain,
                request.Service,
                request.EntityId,
                "failed");
        }
    }
}
