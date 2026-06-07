namespace HubbingHome.Api.Options;

/// <summary>
/// LAN内API保護設定
/// </summary>
public sealed class SecurityOptions
{
    /// <summary>
    /// 端末ごとに発行するAPIトークン一覧
    /// </summary>
    public IList<string> DeviceTokens { get; init; } = [];
}

