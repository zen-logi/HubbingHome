using Foundation;

namespace HubbingHome.App;

/// <summary>
/// Mac Catalyst アプリケーションデリゲート
/// </summary>
[Register("AppDelegate")]
public sealed class AppDelegate : MauiUIApplicationDelegate
{
    /// <inheritdoc />
    protected override MauiApp CreateMauiApp()
    {
        return MauiProgram.CreateMauiApp();
    }
}
