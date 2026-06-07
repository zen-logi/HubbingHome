using HubbingHome.Api.Clients;
using HubbingHome.Api.Options;
using HubbingHome.Api.Security;
using HubbingHome.Api.Services;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace HubbingHome.Api;

/// <summary>
/// APIのDI登録とHTTPパイプライン構成
/// </summary>
public sealed class Startup(IConfiguration configuration, IWebHostEnvironment environment)
{
    /// <summary>
    /// DIへアプリケーションサービスを登録
    /// </summary>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();
        services
            .AddAuthentication(DeviceTokenAuthenticationDefaults.AuthenticationScheme)
            .AddScheme<AuthenticationSchemeOptions, DeviceTokenAuthenticationHandler>(
                DeviceTokenAuthenticationDefaults.AuthenticationScheme,
                _ => { });
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(DeviceTokenAuthenticationDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    GetClientPartitionKey(context),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 60,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                    }));
        });

        services.AddSingleton<IValidateOptions<HomeAssistantOptions>, HomeAssistantOptionsValidator>();
        services.AddSingleton<IValidateOptions<RemoteControlOptions>, RemoteControlOptionsValidator>();
        services.AddSingleton<IValidateOptions<SecurityOptions>, SecurityOptionsValidator>();
        services.AddOptions<HomeAssistantOptions>()
            .Bind(configuration.GetSection("HomeAssistant"))
            .ValidateOnStart();
        services.AddOptions<RemoteControlOptions>()
            .Bind(configuration.GetSection("RemoteControl"))
            .ValidateOnStart();
        services.AddOptions<SecurityOptions>()
            .Bind(configuration.GetSection("Security"))
            .ValidateOnStart();

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IRemoteControlStateService, RemoteControlStateService>();
        services.AddScoped<IRemoteControlCatalogService, RemoteControlCatalogService>();
        services.AddScoped<IRemoteCommandService, RemoteCommandService>();

        services.AddHttpClient<IHomeAssistantClient, HomeAssistantClient>((serviceProvider, httpClient) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<HomeAssistantOptions>>().Value;
            httpClient.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
            httpClient.Timeout = TimeSpan.FromSeconds(options.RequestTimeoutSeconds);
        });
    }

    /// <summary>
    /// HTTP要求処理パイプラインを構成
    /// </summary>
    public void Configure(WebApplication app)
    {
        if (environment.IsDevelopment())
        {
            app.MapOpenApi().AllowAnonymous();
        }
        else
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(new { message = "An unexpected error occurred" });
                });
            });
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseRateLimiter();
        app.UseAuthorization();
        app.MapControllers();
    }

    /// <summary>
    /// レート制限用のクライアント識別子を作成
    /// </summary>
    private static string GetClientPartitionKey(HttpContext context)
    {
        if (context.User.Identity is { IsAuthenticated: true }
            && !string.IsNullOrWhiteSpace(context.User.Identity.Name))
        {
            return $"device:{context.User.Identity.Name}";
        }

        return $"ip:{context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";
    }
}
