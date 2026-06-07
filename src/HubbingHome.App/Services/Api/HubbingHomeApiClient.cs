using System.Net.Http.Json;
using HubbingHome.App.Services.Configuration;
using HubbingHome.Shared.RemoteControl;

namespace HubbingHome.App.Services.Api;

/// <inheritdoc />
public sealed class HubbingHomeApiClient(HttpClient httpClient, IApiClientSettingsService settingsService) : IHubbingHomeApiClient
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<RemoteRoomDto>> GetRoomsAsync(CancellationToken cancellationToken = default)
    {
        using var request = await CreateRequestAsync(HttpMethod.Get, "rooms");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var rooms = await response.Content.ReadFromJsonAsync<IReadOnlyList<RemoteRoomDto>>(cancellationToken);
        return rooms ?? [];
    }

    /// <inheritdoc />
    public async Task<RemoteStateDto> GetStateAsync(CancellationToken cancellationToken = default)
    {
        using var request = await CreateRequestAsync(HttpMethod.Get, "state");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var state = await response.Content.ReadFromJsonAsync<RemoteStateDto>(cancellationToken);
        return state ?? new RemoteStateDto([]);
    }

    /// <inheritdoc />
    public async Task<RemoteCommandResultDto> ExecuteCommandAsync(
        RemoteCommandRequestDto request,
        CancellationToken cancellationToken = default)
    {
        using var message = await CreateRequestAsync(HttpMethod.Post, "commands");
        message.Content = JsonContent.Create(request);

        using var response = await httpClient.SendAsync(message, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<RemoteCommandResultDto>(cancellationToken);
        return result ?? new RemoteCommandResultDto(
            false,
            request.RoomId,
            request.DeviceId,
            request.CommandId,
            DateTimeOffset.Now,
            "APIから操作結果を取得できない");
    }

    /// <summary>
    /// 現在の設定からHTTP要求を作成
    /// </summary>
    private async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string relativePath)
    {
        var settings = await settingsService.GetCurrentAsync();
        var baseAddress = await settingsService.GetApiBaseAddressAsync();
        var request = new HttpRequestMessage(method, new Uri(baseAddress, relativePath));

        if (!string.IsNullOrWhiteSpace(settings.DeviceToken))
        {
            request.Headers.Add("X-HubbingHome-Device-Token", settings.DeviceToken);
        }

        return request;
    }
}
