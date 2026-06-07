namespace HubbingHome.Api.Clients;

/// <summary>
/// Home Assistant呼び出し失敗の分類
/// </summary>
public enum HomeAssistantFailureKind
{
    /// <summary>
    /// Home Assistant接続設定が不足している
    /// </summary>
    Configuration,

    /// <summary>
    /// 要求がタイムアウトした
    /// </summary>
    Timeout,

    /// <summary>
    /// ネットワークまたは接続の失敗
    /// </summary>
    Transport,

    /// <summary>
    /// Home Assistantが失敗ステータスを返した
    /// </summary>
    HttpStatus,

    /// <summary>
    /// 分類できない失敗
    /// </summary>
    Unknown,
}
