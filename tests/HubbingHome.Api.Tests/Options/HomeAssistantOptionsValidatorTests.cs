using HubbingHome.Api.Options;

namespace HubbingHome.Api.Tests.Options;

/// <summary>
/// Home Assistant接続設定検証のテスト
/// </summary>
public sealed class HomeAssistantOptionsValidatorTests
{
    [Test]
    public async Task Validate_ProductionでもAllowInsecureHttpが有効ならHttpを許可()
    {
        var validator = new HomeAssistantOptionsValidator();

        var result = validator.Validate(null, CreateOptions("http://homeassistant.local:8123/", allowInsecureHttp: true));

        await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task Validate_AllowInsecureHttpが無効ならHttpを拒否()
    {
        var validator = new HomeAssistantOptionsValidator();

        var result = validator.Validate(null, CreateOptions("http://homeassistant.local:8123/", allowInsecureHttp: false));

        await Assert.That(result.Failed).IsTrue();
        await Assert.That(result.Failures).Contains("HomeAssistant:BaseUrl must use HTTPS unless AllowInsecureHttp is enabled.");
    }

    [Test]
    public async Task Validate_HttpsはAllowInsecureHttpなしで許可()
    {
        var validator = new HomeAssistantOptionsValidator();

        var result = validator.Validate(null, CreateOptions("https://homeassistant.example.local/"));

        await Assert.That(result.Succeeded).IsTrue();
    }

    [Test]
    public async Task Validate_環境に関係なくAllowInsecureHttpが無効ならHttpを拒否()
    {
        var validator = new HomeAssistantOptionsValidator();

        var result = validator.Validate(null, CreateOptions("http://homeassistant.local:8123/"));

        await Assert.That(result.Failed).IsTrue();
    }

    [Test]
    public async Task Validate_Http以外の非HttpsスキームはAllowInsecureHttpが有効でも拒否()
    {
        var validator = new HomeAssistantOptionsValidator();

        var result = validator.Validate(null, CreateOptions("ftp://homeassistant.local/", allowInsecureHttp: true));

        await Assert.That(result.Failed).IsTrue();
    }

    private static HomeAssistantOptions CreateOptions(
        string baseUrl,
        bool allowInsecureHttp = false) =>
        new()
        {
            BaseUrl = baseUrl,
            AccessToken = "valid-home-assistant-token",
            RequestTimeoutSeconds = 10,
            AllowInsecureHttp = allowInsecureHttp,
        };
}
