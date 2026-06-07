using Android.App;
using Android.Runtime;

namespace HubbingHome.App;

/// <summary>
/// Android アプリケーション
/// </summary>
[Application]
public sealed class MainApplication(IntPtr handle, JniHandleOwnership ownership) : MauiApplication(handle, ownership)
{
    /// <inheritdoc />
    protected override MauiApp CreateMauiApp()
    {
        return MauiProgram.CreateMauiApp();
    }
}
