using HubbingHome.App.Services.Api;
using Microsoft.Extensions.DependencyInjection;

namespace HubbingHome.App.Services.Configuration;

/// <summary>
/// クライアント DI 登録拡張
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// HubbingHome クライアントサービスの登録
    /// </summary>
    /// <param name="services">サービスコレクション</param>
    /// <returns>サービスコレクション</returns>
    public static IServiceCollection AddHubbingHomeClient(this IServiceCollection services)
    {
#if QA_MOCK
        services.AddSingleton<IApiClientSettingsService, MockApiClientSettingsService>();
        services.AddSingleton<IHubbingHomeApiClient, MockHubbingHomeApiClient>();
#else
        services.AddSingleton<IApiClientSettingsService, ApiClientSettingsService>();
        services.AddHttpClient<IHubbingHomeApiClient, HubbingHomeApiClient>();
#endif

        return services;
    }
}
