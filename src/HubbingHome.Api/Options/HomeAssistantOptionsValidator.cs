using Microsoft.Extensions.Options;

namespace HubbingHome.Api.Options;

/// <summary>
/// Home Assistant接続設定を検証
/// </summary>
public sealed class HomeAssistantOptionsValidator(IWebHostEnvironment environment)
    : IValidateOptions<HomeAssistantOptions>
{
    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, HomeAssistantOptions options)
    {
        var failures = new List<string>();

        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseAddress))
        {
            failures.Add("HomeAssistant:BaseUrl must be an absolute URL.");
        }
        else if (baseAddress.Scheme != Uri.UriSchemeHttps
            && !(environment.IsDevelopment() && options.AllowInsecureHttp && baseAddress.Scheme == Uri.UriSchemeHttp))
        {
            failures.Add("HomeAssistant:BaseUrl must use HTTPS unless AllowInsecureHttp is enabled in Development.");
        }

        if (string.IsNullOrWhiteSpace(options.AccessToken)
            || IsPlaceholder(options.AccessToken))
        {
            failures.Add("HomeAssistant:AccessToken must be configured from a secret source.");
        }

        if (options.RequestTimeoutSeconds is < 1 or > 60)
        {
            failures.Add("HomeAssistant:RequestTimeoutSeconds must be between 1 and 60.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }

    private static bool IsPlaceholder(string value) =>
        value.StartsWith("REPLACE_ME", StringComparison.OrdinalIgnoreCase)
        || value.Contains("change-me", StringComparison.OrdinalIgnoreCase);
}
