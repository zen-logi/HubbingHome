using Microsoft.Extensions.Options;

namespace HubbingHome.Api.Options;

/// <summary>
/// API保護設定を検証
/// </summary>
public sealed class SecurityOptionsValidator : IValidateOptions<SecurityOptions>
{
    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, SecurityOptions options)
    {
        var tokens = options.DeviceTokens
            .Where(token => !string.IsNullOrWhiteSpace(token))
            .ToArray();

        var failures = new List<string>();
        if (tokens.Length == 0)
        {
            failures.Add("Security:DeviceTokens must contain at least one token.");
        }

        if (tokens.Any(IsPlaceholder))
        {
            failures.Add("Security:DeviceTokens must not contain sample or placeholder tokens.");
        }

        if (tokens.Distinct(StringComparer.Ordinal).Count() != tokens.Length)
        {
            failures.Add("Security:DeviceTokens must not contain duplicates.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }

    private static bool IsPlaceholder(string value) =>
        value.StartsWith("REPLACE_ME", StringComparison.OrdinalIgnoreCase)
        || value.Contains("change-me", StringComparison.OrdinalIgnoreCase);
}
