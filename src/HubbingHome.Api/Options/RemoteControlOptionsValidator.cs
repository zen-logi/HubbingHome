using Microsoft.Extensions.Options;

namespace HubbingHome.Api.Options;

/// <summary>
/// リモコン構成設定を検証
/// </summary>
public sealed class RemoteControlOptionsValidator : IValidateOptions<RemoteControlOptions>
{
    private static readonly HashSet<string> AllowedDataKeys = ["device", "num_repeats", "delay_secs"];

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, RemoteControlOptions options)
    {
        var failures = new List<string>();

        ValidateUniqueIds(
            options.Rooms,
            room => room.Id,
            "RemoteControl:Rooms",
            failures);

        foreach (var room in options.Rooms)
        {
            Require(room.Id, $"Room '{room.Name}' Id", failures);
            Require(room.Name, $"Room '{room.Id}' Name", failures);
            Require(room.RemoteEntityId, $"Room '{room.Id}' RemoteEntityId", failures);

            ValidateUniqueIds(
                room.Devices,
                device => device.Id,
                $"RemoteControl:Rooms:{room.Id}:Devices",
                failures);

            foreach (var device in room.Devices)
            {
                Require(device.Id, $"Device in room '{room.Id}' Id", failures);
                Require(device.Name, $"Device '{device.Id}' Name", failures);
                Require(device.Kind, $"Device '{device.Id}' Kind", failures);

                ValidateUniqueIds(
                    device.Commands,
                    command => command.Id,
                    $"RemoteControl:Rooms:{room.Id}:Devices:{device.Id}:Commands",
                    failures);

                foreach (var command in device.Commands)
                {
                    Require(command.Id, $"Command in device '{device.Id}' Id", failures);
                    Require(command.Name, $"Command '{command.Id}' Name", failures);
                    Require(command.Kind, $"Command '{command.Id}' Kind", failures);
                    Require(command.HomeAssistantCommand, $"Command '{command.Id}' HomeAssistantCommand", failures);

                    if (command.ServiceDomain != "remote" || command.ServiceName != "send_command")
                    {
                        failures.Add($"Command '{command.Id}' must use remote.send_command.");
                    }

                    if (command.Data.ContainsKey("entity_id") || command.Data.ContainsKey("command"))
                    {
                        failures.Add($"Command '{command.Id}' data must not override entity_id or command.");
                    }

                    foreach (var dataKey in command.Data.Keys)
                    {
                        if (!AllowedDataKeys.Contains(dataKey))
                        {
                            failures.Add($"Command '{command.Id}' data key '{dataKey}' is not allowed.");
                        }
                    }
                }
            }
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }

    private static void Require(string value, string label, List<string> failures)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            failures.Add($"{label} is required.");
        }
    }

    private static void ValidateUniqueIds<T>(
        IEnumerable<T> items,
        Func<T, string> idSelector,
        string path,
        List<string> failures)
    {
        var duplicateIds = items
            .Select(idSelector)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .GroupBy(id => id, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key);

        foreach (var duplicateId in duplicateIds)
        {
            failures.Add($"{path} contains duplicate id '{duplicateId}'.");
        }
    }
}
