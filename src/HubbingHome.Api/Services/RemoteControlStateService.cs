using HubbingHome.Shared.RemoteControl;

namespace HubbingHome.Api.Services;

/// <summary>
/// メモリ上に最終操作履歴を保持する状態サービス
/// </summary>
public sealed class RemoteControlStateService : IRemoteControlStateService
{
    private const int MaxHistoryCount = 30;
    private readonly Lock syncRoot = new();
    private readonly LinkedList<RemoteCommandResultDto> recentCommands = [];

    /// <inheritdoc />
    public RemoteStateDto GetState()
    {
        lock (syncRoot)
        {
            return new RemoteStateDto(recentCommands.ToArray());
        }
    }

    /// <inheritdoc />
    public void Record(RemoteCommandResultDto result)
    {
        lock (syncRoot)
        {
            recentCommands.AddFirst(result);

            while (recentCommands.Count > MaxHistoryCount)
            {
                recentCommands.RemoveLast();
            }
        }
    }
}

