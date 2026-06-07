namespace HubbingHome.App.Models;

/// <summary>
/// ダッシュボードアクティビティ
/// </summary>
/// <param name="Title">表示タイトル</param>
/// <param name="Timestamp">発生日時</param>
public sealed record DashboardActivity(string Title, DateTimeOffset Timestamp);
