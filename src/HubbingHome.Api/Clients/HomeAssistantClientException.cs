using System.Net;

namespace HubbingHome.Api.Clients;

/// <summary>
/// Home Assistantサービス呼び出し失敗
/// </summary>
public sealed class HomeAssistantClientException : Exception
{
    public HomeAssistantClientException(
        HomeAssistantFailureKind failureKind,
        string domain,
        string service,
        string entityId,
        string message,
        HttpStatusCode? statusCode = null,
        Uri? requestUri = null,
        string? responseSnippet = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        FailureKind = failureKind;
        Domain = domain;
        Service = service;
        EntityId = entityId;
        StatusCode = statusCode;
        RequestUri = requestUri;
        ResponseSnippet = responseSnippet;
    }

    /// <summary>
    /// 失敗分類
    /// </summary>
    public HomeAssistantFailureKind FailureKind { get; }

    /// <summary>
    /// Home Assistantサービスドメイン
    /// </summary>
    public string Domain { get; }

    /// <summary>
    /// Home Assistantサービス名
    /// </summary>
    public string Service { get; }

    /// <summary>
    /// 対象entity_id
    /// </summary>
    public string EntityId { get; }

    /// <summary>
    /// Home Assistant応答ステータス
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>
    /// Bearer tokenを含まない要求URI
    /// </summary>
    public Uri? RequestUri { get; }

    /// <summary>
    /// 調査用に切り詰めた応答本文
    /// </summary>
    public string? ResponseSnippet { get; }
}
