namespace HubbingHome.Api.Options;

/// <summary>
/// リモコン構成の設定値
/// </summary>
public sealed class RemoteControlOptions
{
    /// <summary>
    /// 部屋一覧
    /// </summary>
    public IList<RemoteRoomOptions> Rooms { get; init; } = [];
}

/// <summary>
/// 部屋ごとのスマートリモコン設定
/// </summary>
public sealed class RemoteRoomOptions
{
    /// <summary>
    /// 部屋ID
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// 部屋名
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Home Assistant上のBroadLink remote entity
    /// </summary>
    public string RemoteEntityId { get; init; } = string.Empty;

    /// <summary>
    /// 部屋内の機器一覧
    /// </summary>
    public IList<RemoteDeviceOptions> Devices { get; init; } = [];
}

/// <summary>
/// 操作対象機器の設定
/// </summary>
public sealed class RemoteDeviceOptions
{
    /// <summary>
    /// 機器ID
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// 機器名
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// 機器種別
    /// </summary>
    public string Kind { get; init; } = string.Empty;

    /// <summary>
    /// 許可済みコマンド一覧
    /// </summary>
    public IList<RemoteCommandOptions> Commands { get; init; } = [];
}

/// <summary>
/// Home Assistantへ送信する許可済みコマンド設定
/// </summary>
public sealed class RemoteCommandOptions
{
    /// <summary>
    /// コマンドID
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// 表示名
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// コマンド種別
    /// </summary>
    public string Kind { get; init; } = string.Empty;

    /// <summary>
    /// 表示用アイコン名
    /// </summary>
    public string? Icon { get; init; }

    /// <summary>
    /// 表示用パラメータ
    /// </summary>
    public IDictionary<string, string>? Parameters { get; init; }

    /// <summary>
    /// Home Assistantサービスドメイン
    /// </summary>
    public string ServiceDomain { get; init; } = "remote";

    /// <summary>
    /// Home Assistantサービス名
    /// </summary>
    public string ServiceName { get; init; } = "send_command";

    /// <summary>
    /// BroadLinkへ送信するコマンド名
    /// </summary>
    public string HomeAssistantCommand { get; init; } = string.Empty;

    /// <summary>
    /// リクエストパラメータからHome Assistantコマンド名を受け取るキー
    /// </summary>
    public string? HomeAssistantCommandParameter { get; init; }

    /// <summary>
    /// リクエスト由来Home Assistantコマンド名の許可パターン
    /// </summary>
    public string? HomeAssistantCommandPattern { get; init; }

    /// <summary>
    /// Home Assistantコマンド名を生成する方式
    /// </summary>
    public string? HomeAssistantCommandGenerator { get; init; }

    /// <summary>
    /// Daikinエアコン赤外線コード生成設定
    /// </summary>
    public DaikinAirConditionerCodeOptions? DaikinAirConditionerCode { get; init; }

    /// <summary>
    /// Home Assistantサービスへ渡す追加データ
    /// </summary>
    public IDictionary<string, object?> Data { get; init; } = new Dictionary<string, object?>();
}

/// <summary>
/// Daikinエアコン赤外線コード生成設定
/// </summary>
public sealed class DaikinAirConditionerCodeOptions
{
    /// <summary>
    /// 冷房時の第1フレーム設定バイト
    /// </summary>
    public int CoolFirstFrameModeByte { get; init; } = 0x20;

    /// <summary>
    /// 暖房時の第1フレーム設定バイト
    /// </summary>
    public int HeatFirstFrameModeByte { get; init; } = 0x50;
}
