using Microsoft.Maui.Storage;

namespace HubbingHome.App;

/// <summary>
/// アプリケーションルート
/// </summary>
public partial class App : Application
{
    private const string WindowWidthKey = "Window.Width";
    private const string WindowHeightKey = "Window.Height";
    private const double DefaultWindowWidth = 520;
    private const double DefaultWindowHeight = 900;
    private const double MinimumSavedWindowWidth = 400;
    private const double MinimumSavedWindowHeight = 640;

    /// <summary>
    /// アプリケーションルートの初期化
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new MainPage())
        {
            Width = Preferences.Default.Get(WindowWidthKey, DefaultWindowWidth),
            Height = Preferences.Default.Get(WindowHeightKey, DefaultWindowHeight)
        };

        window.SizeChanged += (_, _) => SaveWindowSize(window);

        return window;
    }

    /// <summary>
    /// ユーザーが変更したウィンドウサイズを保存
    /// </summary>
    private static void SaveWindowSize(Window window)
    {
        if (window.Width < MinimumSavedWindowWidth || window.Height < MinimumSavedWindowHeight)
        {
            return;
        }

        Preferences.Default.Set(WindowWidthKey, window.Width);
        Preferences.Default.Set(WindowHeightKey, window.Height);
    }
}
