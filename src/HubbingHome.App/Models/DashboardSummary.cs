using HubbingHome.App.Components.Shared;

namespace HubbingHome.App.Models;

/// <summary>
/// ダッシュボード概要
/// </summary>
public sealed record DashboardSummary
{
    /// <summary>
    /// デバイス数
    /// </summary>
    public int DeviceCount { get; init; }

    /// <summary>
    /// アクティブデバイス数
    /// </summary>
    public int ActiveDeviceCount { get; init; }

    /// <summary>
    /// 部屋数
    /// </summary>
    public int RoomCount { get; init; }

    /// <summary>
    /// 未対応アラート数
    /// </summary>
    public int OpenAlertCount { get; init; }

    /// <summary>
    /// ヘルステキスト
    /// </summary>
    public string HealthText { get; init; } = "正常";

    /// <summary>
    /// ヘルス表示種別
    /// </summary>
    public StatusBadgeKind HealthKind { get; init; } = StatusBadgeKind.Success;

    /// <summary>
    /// API 接続先
    /// </summary>
    public string ApiEndpoint { get; init; } = string.Empty;

    /// <summary>
    /// 最終同期日時
    /// </summary>
    public DateTimeOffset LastSyncedAt { get; init; }

    /// <summary>
    /// 最近のアクティビティ
    /// </summary>
    public IReadOnlyList<DashboardActivity> RecentActivities { get; init; } = [];
}
