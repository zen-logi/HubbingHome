using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using HubbingHome.Api.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace HubbingHome.Api.Security;

/// <summary>
/// 端末トークンヘッダーを検証する認証ハンドラー
/// </summary>
public sealed class DeviceTokenAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    IOptionsMonitor<SecurityOptions> securityOptions,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    /// <inheritdoc />
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var providedToken = Request.Headers[DeviceTokenAuthenticationDefaults.HeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(providedToken))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var tokenIndex = FindTokenIndex(providedToken);
        if (tokenIndex is null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid device token"));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, $"device-{tokenIndex.Value}"),
            new Claim(ClaimTypes.Name, $"device-{tokenIndex.Value}"),
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    /// <inheritdoc />
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Response.WriteAsJsonAsync(new { message = "Device token is required" });
    }

    private int? FindTokenIndex(string providedToken)
    {
        var configuredTokens = securityOptions.CurrentValue.DeviceTokens;
        for (var index = 0; index < configuredTokens.Count; index++)
        {
            var configuredToken = configuredTokens[index];
            if (!string.IsNullOrWhiteSpace(configuredToken)
                && FixedTimeEquals(providedToken, configuredToken))
            {
                return index;
            }
        }

        return null;
    }

    private static bool FixedTimeEquals(string left, string right)
    {
        var leftBytes = Encoding.UTF8.GetBytes(left);
        var rightBytes = Encoding.UTF8.GetBytes(right);

        return leftBytes.Length == rightBytes.Length
            && CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }
}
