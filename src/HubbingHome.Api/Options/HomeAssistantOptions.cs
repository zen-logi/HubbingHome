namespace HubbingHome.Api.Options;

/// <summary>
/// Home Assistant接続設定
/// </summary>
public sealed class HomeAssistantOptions
{
    /// <summary>
    /// Home AssistantのベースURL
    /// </summary>
    public string BaseUrl { get; init; } = string.Empty;

    /// <summary>
    /// Long-Lived Access Token
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// 要求タイムアウト秒数
    /// </summary>
    public int RequestTimeoutSeconds { get; init; } = 10;

    /// <summary>
    /// HTTP接続を明示的に許可
    /// </summary>
    public bool AllowInsecureHttp { get; init; }
}
