namespace HubbingHome.App.Services.Configuration;

/// <summary>
/// 自宅サーバAPI接続設定サービス
/// </summary>
public interface IApiClientSettingsService
{
    /// <summary>
    /// 設定変更通知
    /// </summary>
    event Action? Changed;

    /// <summary>
    /// 現在の接続設定を取得する
    /// </summary>
    Task<ApiClientSettings> GetCurrentAsync();

    /// <summary>
    /// 接続設定が完了しているか取得する
    /// </summary>
    Task<bool> IsConfiguredAsync();

    /// <summary>
    /// APIベースアドレスを取得する
    /// </summary>
    Task<Uri> GetApiBaseAddressAsync();

    /// <summary>
    /// 接続設定を保存する
    /// </summary>
    Task SaveAsync(string serverAddress, string deviceToken);
}
