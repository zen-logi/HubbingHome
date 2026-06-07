namespace HubbingHome.App.Services.Configuration;

/// <summary>
/// QA用のMock接続設定サービス
/// </summary>
public sealed class MockApiClientSettingsService : IApiClientSettingsService
{
    private static readonly ApiClientSettings Settings = new(
        "mock://hubbinghome.local",
        "qa-mock-device-token");

    /// <inheritdoc />
    public event Action? Changed;

    /// <inheritdoc />
    public Task<ApiClientSettings> GetCurrentAsync() => Task.FromResult(Settings);

    /// <inheritdoc />
    public Task<bool> IsConfiguredAsync() => Task.FromResult(true);

    /// <inheritdoc />
    public Task<Uri> GetApiBaseAddressAsync() => Task.FromResult(new Uri("https://mock.hubbinghome.local/api/"));

    /// <inheritdoc />
    public Task SaveAsync(string serverAddress, string deviceToken)
    {
        Changed?.Invoke();
        return Task.CompletedTask;
    }
}
