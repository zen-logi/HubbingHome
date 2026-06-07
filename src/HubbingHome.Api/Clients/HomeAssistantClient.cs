using System.Net.Http.Headers;
using System.Net.Http.Json;
using HubbingHome.Api.Options;
using Microsoft.Extensions.Options;

namespace HubbingHome.Api.Clients;

/// <summary>
/// REST API経由でHome Assistantサービスを呼び出すクライアント
/// </summary>
public sealed class HomeAssistantClient(HttpClient httpClient, IOptions<HomeAssistantOptions> options) : IHomeAssistantClient
{
    /// <inheritdoc />
    public async Task CallServiceAsync(HomeAssistantServiceRequest request, CancellationToken cancellationToken)
    {
        var configuredOptions = options.Value;
        if (string.IsNullOrWhiteSpace(configuredOptions.AccessToken))
        {
            throw CreateException(
                HomeAssistantFailureKind.Configuration,
                request,
                "Home Assistant access token is not configured");
        }

        using var message = new HttpRequestMessage(
            HttpMethod.Post,
            $"api/services/{Uri.EscapeDataString(request.Domain)}/{Uri.EscapeDataString(request.Service)}");

        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", configuredOptions.AccessToken);
        message.Content = JsonContent.Create(CreatePayload(request));

        try
        {
            using var response = await httpClient.SendAsync(message, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            throw CreateException(
                HomeAssistantFailureKind.HttpStatus,
                request,
                $"Home Assistant returned HTTP {(int)response.StatusCode}",
                response.StatusCode,
                message.RequestUri,
                await ReadResponseSnippetAsync(response, cancellationToken));
        }
        catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            throw CreateException(
                HomeAssistantFailureKind.Timeout,
                request,
                "Home Assistant request timed out",
                requestUri: message.RequestUri,
                innerException: exception);
        }
        catch (HttpRequestException exception)
        {
            throw CreateException(
                HomeAssistantFailureKind.Transport,
                request,
                "Home Assistant request failed",
                exception.StatusCode,
                message.RequestUri,
                innerException: exception);
        }
    }

    /// <summary>
    /// Home Assistantサービス呼び出しペイロードを作成
    /// </summary>
    private static Dictionary<string, object?> CreatePayload(HomeAssistantServiceRequest request)
    {
        var payload = new Dictionary<string, object?>(request.Data)
        {
            ["entity_id"] = request.EntityId,
        };

        return payload;
    }

    /// <summary>
    /// Home Assistant例外を作成
    /// </summary>
    private static HomeAssistantClientException CreateException(
        HomeAssistantFailureKind failureKind,
        HomeAssistantServiceRequest request,
        string message,
        System.Net.HttpStatusCode? statusCode = null,
        Uri? requestUri = null,
        string? responseSnippet = null,
        Exception? innerException = null) =>
        new(
            failureKind,
            request.Domain,
            request.Service,
            request.EntityId,
            message,
            statusCode,
            requestUri,
            responseSnippet,
            innerException);

    /// <summary>
    /// ログ向けに応答本文を切り詰める
    /// </summary>
    private static async Task<string?> ReadResponseSnippetAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        content = content.ReplaceLineEndings(" ");
        const int maxLength = 512;
        return content.Length <= maxLength
            ? content
            : content[..(maxLength - 3)] + "...";
    }
}
