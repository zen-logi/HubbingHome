namespace HubbingHome.Api.Clients;

/// <summary>
/// Home Assistantサービス呼び出し要求
/// </summary>
public sealed record HomeAssistantServiceRequest(
    string Domain,
    string Service,
    string EntityId,
    IReadOnlyDictionary<string, object?> Data);

