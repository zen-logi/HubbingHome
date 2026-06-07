namespace HubbingHome.App.Services.Configuration;

/// <summary>
/// 自宅サーバAPI接続設定
/// </summary>
public sealed record ApiClientSettings(string ServerAddress, string DeviceToken)
{
    /// <summary>
    /// 未設定状態
    /// </summary>
    public static ApiClientSettings Empty { get; } = new(string.Empty, string.Empty);
}

