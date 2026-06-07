namespace HubbingHome.App.WinUI;

/// <summary>
/// Windows アプリケーション
/// </summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>
    /// Windows アプリケーションの初期化
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    protected override MauiApp CreateMauiApp()
    {
        return HubbingHome.App.MauiProgram.CreateMauiApp();
    }
}
