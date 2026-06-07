using Microsoft.Maui.Storage;

namespace HubbingHome.App.Services.Configuration;

/// <summary>
/// MAUIストレージに自宅サーバAPI接続設定を保存するサービス
/// </summary>
public sealed class ApiClientSettingsService : IApiClientSettingsService
{
    private const string ServerAddressKey = "ApiClient.ServerAddress";
    private const string DeviceTokenKey = "ApiClient.DeviceToken";

    /// <inheritdoc />
    public event Action? Changed;

    /// <inheritdoc />
    public async Task<ApiClientSettings> GetCurrentAsync()
    {
        return new ApiClientSettings(
            Preferences.Default.Get(ServerAddressKey, string.Empty),
            await GetDeviceTokenAsync());
    }

    /// <inheritdoc />
    public async Task<bool> IsConfiguredAsync()
    {
        var settings = await GetCurrentAsync();
        return !string.IsNullOrWhiteSpace(settings.ServerAddress)
            && !string.IsNullOrWhiteSpace(settings.DeviceToken);
    }

    /// <inheritdoc />
    public async Task<Uri> GetApiBaseAddressAsync()
    {
        var settings = await GetCurrentAsync();
        if (string.IsNullOrWhiteSpace(settings.ServerAddress))
        {
            throw new InvalidOperationException("API server address is not configured");
        }

        return BuildApiBaseAddress(settings.ServerAddress);
    }

    /// <inheritdoc />
    public async Task SaveAsync(string serverAddress, string deviceToken)
    {
        Preferences.Default.Set(ServerAddressKey, serverAddress.Trim());
        Preferences.Default.Remove(DeviceTokenKey);

        var normalizedDeviceToken = deviceToken.Trim();
        if (string.IsNullOrWhiteSpace(normalizedDeviceToken))
        {
            SecureStorage.Default.Remove(DeviceTokenKey);
        }
        else
        {
            await SecureStorage.Default.SetAsync(DeviceTokenKey, normalizedDeviceToken);
        }

        Changed?.Invoke();
    }

    /// <summary>
    /// 入力されたサーバアドレスからAPIベースアドレスを生成
    /// </summary>
    private static Uri BuildApiBaseAddress(string serverAddress)
    {
        var normalized = serverAddress.Trim();
        if (!normalized.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            && !normalized.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            normalized = $"https://{normalized}";
        }

        if (!Uri.TryCreate(normalized, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
        {
            throw new InvalidOperationException("API server address must be an absolute HTTP or HTTPS URL");
        }

        normalized = normalized.TrimEnd('/');
        if (!normalized.EndsWith("/api", StringComparison.OrdinalIgnoreCase))
        {
            normalized = $"{normalized}/api";
        }

        return new Uri($"{normalized}/", UriKind.Absolute);
    }

    /// <summary>
    /// SecureStorageから端末トークンを取得し、旧Preferences保存値があれば移行する
    /// </summary>
    private static async Task<string> GetDeviceTokenAsync()
    {
        var deviceToken = await SecureStorage.Default.GetAsync(DeviceTokenKey);
        if (!string.IsNullOrWhiteSpace(deviceToken))
        {
            Preferences.Default.Remove(DeviceTokenKey);
            return deviceToken;
        }

        var legacyDeviceToken = Preferences.Default.Get(DeviceTokenKey, string.Empty);
        if (string.IsNullOrWhiteSpace(legacyDeviceToken))
        {
            return string.Empty;
        }

        var normalizedLegacyDeviceToken = legacyDeviceToken.Trim();
        await SecureStorage.Default.SetAsync(DeviceTokenKey, normalizedLegacyDeviceToken);
        Preferences.Default.Remove(DeviceTokenKey);

        return normalizedLegacyDeviceToken;
    }
}
