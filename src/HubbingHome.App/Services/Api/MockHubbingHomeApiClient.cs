using HubbingHome.Shared.RemoteControl;

namespace HubbingHome.App.Services.Api;

/// <summary>
/// QA用のアプリ内Mock APIクライアント
/// </summary>
public sealed class MockHubbingHomeApiClient : IHubbingHomeApiClient
{
    private static readonly RemoteRoomDto[] Rooms =
    [
        new(
            "living",
            "リビング",
            [
                new(
                    "living-ac",
                    "エアコン",
                    "airConditioner",
                    [
                        new("cool-26", "冷房", "mode", "snowflake", new Dictionary<string, string>
                        {
                            ["mode"] = "cool",
                            ["temp"] = "26"
                        }),
                        new("heat-22", "暖房", "mode", "flame", new Dictionary<string, string>
                        {
                            ["mode"] = "heat",
                            ["temp"] = "22"
                        }),
                        new("dry", "除湿", "mode", "droplets", new Dictionary<string, string>
                        {
                            ["mode"] = "dry"
                        }),
                        new("off", "停止", "power", "power", null)
                    ]),
                new(
                    "living-light",
                    "メイン照明",
                    "light",
                    [
                        new("on", "点灯", "power", "lightbulb", null),
                        new("dim", "暗め", "brightness", "moon", new Dictionary<string, string>
                        {
                            ["brightness"] = "40%"
                        }),
                        new("bright", "明るめ", "brightness", "sun", new Dictionary<string, string>
                        {
                            ["brightness"] = "100%"
                        }),
                        new("off", "消灯", "power", "power", null)
                    ]),
                new(
                    "living-tv",
                    "テレビ",
                    "tv",
                    [
                        new("power", "電源", "power", "power", null),
                        new("volume-up", "音量 +", "volume", "volume-2", null),
                        new("volume-down", "音量 -", "volume", "volume-1", null),
                        new("input", "入力切替", "input", "panel-top", null)
                    ])
            ]),
        new(
            "bedroom",
            "寝室",
            [
                new(
                    "bedroom-light",
                    "ベッドライト",
                    "light",
                    [
                        new("on", "点灯", "power", "lightbulb", null),
                        new("night", "常夜灯", "brightness", "moon", new Dictionary<string, string>
                        {
                            ["brightness"] = "10%"
                        }),
                        new("off", "消灯", "power", "power", null)
                    ]),
                new(
                    "bedroom-ac",
                    "寝室エアコン",
                    "airConditioner",
                    [
                        new("cool-27", "冷房 27", "mode", "snowflake", new Dictionary<string, string>
                        {
                            ["temp"] = "27"
                        }),
                        new("off", "停止", "power", "power", null),
                        new("mock-fail", "失敗確認", "diagnostic", "triangle-alert", null)
                    ])
            ]),
        new(
            "workspace",
            "ワークスペース",
            [
                new(
                    "desk-light",
                    "デスクライト",
                    "light",
                    [
                        new("focus", "集中", "scene", "target", new Dictionary<string, string>
                        {
                            ["color"] = "昼白色",
                            ["brightness"] = "80%"
                        }),
                        new("relax", "休憩", "scene", "coffee", new Dictionary<string, string>
                        {
                            ["color"] = "電球色",
                            ["brightness"] = "35%"
                        }),
                        new("off", "消灯", "power", "power", null)
                    ])
            ])
    ];

    private readonly List<RemoteCommandResultDto> recentCommands =
    [
        new(true, "living", "living-light", "on", DateTimeOffset.Now.AddMinutes(-7), "Mock: 点灯コマンドを送信しました"),
        new(true, "bedroom", "bedroom-ac", "cool-27", DateTimeOffset.Now.AddMinutes(-18), "Mock: 冷房 27 を送信しました")
    ];

    /// <inheritdoc />
    public async Task<IReadOnlyList<RemoteRoomDto>> GetRoomsAsync(CancellationToken cancellationToken = default)
    {
        await SimulateLatencyAsync(cancellationToken);
        return Rooms;
    }

    /// <inheritdoc />
    public async Task<RemoteStateDto> GetStateAsync(CancellationToken cancellationToken = default)
    {
        await SimulateLatencyAsync(cancellationToken);
        return new RemoteStateDto(recentCommands.ToArray());
    }

    /// <inheritdoc />
    public async Task<RemoteCommandResultDto> ExecuteCommandAsync(
        RemoteCommandRequestDto request,
        CancellationToken cancellationToken = default)
    {
        await SimulateLatencyAsync(cancellationToken);

        var succeeded = request.CommandId != "mock-fail";
        var result = new RemoteCommandResultDto(
            succeeded,
            request.RoomId,
            request.DeviceId,
            request.CommandId,
            DateTimeOffset.Now,
            succeeded
                ? $"Mock: {request.DeviceId}/{request.CommandId} を送信しました"
                : "Mock: QA用の失敗応答です");

        recentCommands.Insert(0, result);
        if (recentCommands.Count > 10)
        {
            recentCommands.RemoveAt(recentCommands.Count - 1);
        }

        return result;
    }

    /// <summary>
    /// ネイティブQAでローディングと二重タップ抑止を確認しやすくする
    /// </summary>
    private static Task SimulateLatencyAsync(CancellationToken cancellationToken) =>
        Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken);
}
