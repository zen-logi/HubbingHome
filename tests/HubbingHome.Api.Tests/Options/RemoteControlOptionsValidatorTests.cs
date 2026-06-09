using HubbingHome.Api.Options;

namespace HubbingHome.Api.Tests.Options;

/// <summary>
/// リモコン構成検証のテスト
/// </summary>
public sealed class RemoteControlOptionsValidatorTests
{
    [Test]
    public async Task Validate_動的HomeAssistantコマンドはパターン必須()
    {
        var result = new RemoteControlOptionsValidator().Validate(
            null,
            CreateOptions(new RemoteCommandOptions
            {
                Id = "temperature_up",
                Name = "温度 +0.5",
                Kind = "temperature",
                HomeAssistantCommandParameter = "homeAssistantCommand",
            }));

        await Assert.That(result.Failed).IsTrue();
        await Assert.That(result.Failures).Contains("Command 'temperature_up' HomeAssistantCommandPattern is required.");
    }

    [Test]
    public async Task Validate_無効なHomeAssistantコマンドパターンは拒否()
    {
        var result = new RemoteControlOptionsValidator().Validate(
            null,
            CreateOptions(new RemoteCommandOptions
            {
                Id = "temperature_up",
                Name = "温度 +0.5",
                Kind = "temperature",
                HomeAssistantCommandParameter = "homeAssistantCommand",
                HomeAssistantCommandPattern = "[",
            }));

        await Assert.That(result.Failed).IsTrue();
        await Assert.That((result.Failures ?? []).Any(failure =>
            failure.StartsWith(
                "Command 'temperature_up' HomeAssistantCommandPattern is invalid:",
                StringComparison.Ordinal))).IsTrue();
    }

    [Test]
    public async Task Validate_静的HomeAssistantコマンドはコマンド名必須()
    {
        var result = new RemoteControlOptionsValidator().Validate(
            null,
            CreateOptions(new RemoteCommandOptions
            {
                Id = "on",
                Name = "点灯",
                Kind = "power",
            }));

        await Assert.That(result.Failed).IsTrue();
        await Assert.That(result.Failures).Contains("Command 'on' HomeAssistantCommand is required.");
    }

    [Test]
    public async Task Validate_動的HomeAssistantコマンド設定を許可()
    {
        var result = new RemoteControlOptionsValidator().Validate(
            null,
            CreateOptions(new RemoteCommandOptions
            {
                Id = "temperature_up",
                Name = "温度 +0.5",
                Kind = "temperature",
                HomeAssistantCommandParameter = "homeAssistantCommand",
                HomeAssistantCommandPattern = "^cool_25_[05]$",
                Data = new Dictionary<string, object?>
                {
                    ["device"] = "air_conditioner",
                },
            }));

        await Assert.That(result.Succeeded).IsTrue();
    }

    private static RemoteControlOptions CreateOptions(RemoteCommandOptions command) =>
        new()
        {
            Rooms =
            [
                new RemoteRoomOptions
                {
                    Id = "living",
                    Name = "リビング",
                    RemoteEntityId = "remote.living",
                    Devices =
                    [
                        new RemoteDeviceOptions
                        {
                            Id = "aircon",
                            Name = "エアコン",
                            Kind = "airConditioner",
                            Commands = [command],
                        },
                    ],
                },
            ],
        };
}
