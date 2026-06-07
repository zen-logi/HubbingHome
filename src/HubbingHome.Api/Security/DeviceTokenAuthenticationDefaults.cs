namespace HubbingHome.Api.Security;

/// <summary>
/// 端末トークン認証の既定値
/// </summary>
public static class DeviceTokenAuthenticationDefaults
{
    /// <summary>
    /// 認証スキーム名
    /// </summary>
    public const string AuthenticationScheme = "DeviceToken";

    /// <summary>
    /// 端末トークンを受け取るHTTPヘッダー名
    /// </summary>
    public const string HeaderName = "X-HubbingHome-Device-Token";
}
