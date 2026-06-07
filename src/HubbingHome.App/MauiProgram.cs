using HubbingHome.App.Services.Api;
using HubbingHome.App.Services.Configuration;
using Microsoft.Extensions.Logging;

namespace HubbingHome.App;

/// <summary>
/// MAUI アプリケーション構成
/// </summary>
public static class MauiProgram
{
    /// <summary>
    /// MAUI アプリケーションの作成
    /// </summary>
    /// <returns>構成済み MAUI アプリケーション</returns>
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddHubbingHomeClient();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
