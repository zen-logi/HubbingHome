namespace HubbingHome.Api.Clients;

/// <summary>
/// Home Assistant APIクライアント
/// </summary>
public interface IHomeAssistantClient
{
    /// <summary>
    /// Home Assistantサービスを呼び出す
    /// </summary>
    Task CallServiceAsync(HomeAssistantServiceRequest request, CancellationToken cancellationToken);
}

