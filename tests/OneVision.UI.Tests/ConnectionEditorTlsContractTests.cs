using System.Text.Json;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class ConnectionEditorTlsContractTests
{
    [Fact]
    public void Sql_settings_have_secure_governed_defaults()
    {
        var settings = new OVSqlConnectionSettings();

        Assert.True(settings.Encrypt);
        Assert.False(settings.TrustServerCertificate);

        var value = new OVConnectionEditorValue();
        Assert.True(value.SqlSettings.Encrypt);
        Assert.False(value.SqlSettings.TrustServerCertificate);
    }

    [Fact]
    public void Historical_sql_json_without_tls_properties_uses_governed_defaults()
    {
        const string json = """
            {
              "Server": "SQL01",
              "Database": "OneVision",
              "Authentication": 0
            }
            """;

        var settings = JsonSerializer.Deserialize<OVSqlConnectionSettings>(json);

        Assert.NotNull(settings);
        Assert.True(settings!.Encrypt);
        Assert.False(settings.TrustServerCertificate);
        Assert.Equal("SQL01", settings.Server);
        Assert.Equal("OneVision", settings.Database);
    }

    [Fact]
    public async Task Sql_editor_renders_encrypt_and_trust_controls_with_certificate_validation_explanation()
    {
        await using var context = CreateContext();
        var cut = context.Render<OVConnectionEditor>(parameters => parameters
            .Add(component => component.Value, SqlValue())
            .Add(component => component.ShowActions, false));

        var toggles = cut.FindComponents<OVToggle>();
        var encrypt = Assert.Single(toggles, toggle => toggle.Instance.Label == "Encrypt connection");
        var trust = Assert.Single(toggles, toggle => toggle.Instance.Label == "Trust server certificate");

        Assert.True(encrypt.Instance.Value);
        Assert.False(trust.Instance.Value);
        Assert.False(trust.Instance.Disabled);
        Assert.Contains("Accept the SQL Server certificate without validating its certificate chain.", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Trust_server_certificate_is_disabled_when_encryption_is_off()
    {
        await using var context = CreateContext();
        var value = SqlValue() with
        {
            Sql = SqlValue().SqlSettings with { Encrypt = false, TrustServerCertificate = true }
        };
        var cut = context.Render<OVConnectionEditor>(parameters => parameters
            .Add(component => component.Value, value)
            .Add(component => component.ShowActions, false));

        var trust = cut.FindComponents<OVToggle>()
            .Single(toggle => toggle.Instance.Label == "Trust server certificate");

        Assert.True(trust.Instance.Value);
        Assert.True(trust.Instance.Disabled);
    }

    [Fact]
    public async Task Changing_each_tls_setting_updates_the_governed_editor_value()
    {
        await using var context = CreateContext();
        OVConnectionEditorValue? changed = null;
        var value = SqlValue();
        var cut = context.Render<OVConnectionEditor>(parameters => parameters
            .Add(component => component.Value, value)
            .Add(component => component.ValueChanged, updated => { changed = updated; })
            .Add(component => component.ShowActions, false));

        var encrypt = cut.FindComponents<OVToggle>()
            .Single(toggle => toggle.Instance.Label == "Encrypt connection");
        await cut.InvokeAsync(() => encrypt.Instance.ValueChanged.InvokeAsync(false));

        Assert.NotNull(changed);
        Assert.False(changed!.SqlSettings.Encrypt);
        Assert.False(changed.SqlSettings.TrustServerCertificate);

        value = value with { Sql = value.SqlSettings with { TrustServerCertificate = false } };
        cut.Render(parameters => parameters
            .Add(component => component.Value, value)
            .Add(component => component.ValueChanged, updated => { changed = updated; })
            .Add(component => component.ShowActions, false));

        var trust = cut.FindComponents<OVToggle>()
            .Single(toggle => toggle.Instance.Label == "Trust server certificate");
        await cut.InvokeAsync(() => trust.Instance.ValueChanged.InvokeAsync(true));

        Assert.NotNull(changed);
        Assert.True(changed!.SqlSettings.Encrypt);
        Assert.True(changed.SqlSettings.TrustServerCertificate);
    }

    [Fact]
    public async Task Test_connection_receives_current_sql_tls_values()
    {
        await using var context = CreateContext();
        OVConnectionEditorValue? tested = null;
        var value = SqlValue() with
        {
            Sql = SqlValue().SqlSettings with { Encrypt = true, TrustServerCertificate = true }
        };
        var cut = context.Render<OVConnectionEditor>(parameters => parameters
            .Add(component => component.Value, value)
            .Add(component => component.TestRequested, testedValue => { tested = testedValue; }));

        await cut.FindAll("button")
            .Single(button => button.TextContent.Contains("Test connection", StringComparison.Ordinal))
            .ClickAsync();

        Assert.NotNull(tested);
        Assert.True(tested!.SqlSettings.Encrypt);
        Assert.True(tested.SqlSettings.TrustServerCertificate);
    }

    [Theory]
    [InlineData(OVConnectionType.File)]
    [InlineData(OVConnectionType.Ldap)]
    [InlineData(OVConnectionType.Json)]
    public async Task Non_sql_editors_do_not_render_sql_tls_controls(OVConnectionType type)
    {
        await using var context = CreateContext();
        var cut = context.Render<OVConnectionEditor>(parameters => parameters
            .Add(component => component.Value, SqlValue() with { Type = type })
            .Add(component => component.ShowActions, false));

        Assert.DoesNotContain(cut.FindComponents<OVToggle>(), toggle => toggle.Instance.Label == "Encrypt connection");
        Assert.DoesNotContain(cut.FindComponents<OVToggle>(), toggle => toggle.Instance.Label == "Trust server certificate");
        Assert.DoesNotContain("Transport security", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Embedded_editor_keeps_test_action_but_delegates_save_and_cancel_to_overlay()
    {
        await using var context = CreateContext();
        OVConnectionEditorValue? tested = null;
        var cut = context.Render<OVConnectionEditor>(parameters => parameters
            .Add(component => component.Value, SqlValue())
            .Add(component => component.EmbeddedInOverlay, true)
            .Add(component => component.ShowActions, false)
            .Add(component => component.ShowTestAction, true)
            .Add(component => component.TestRequested, value => { tested = value; }));

        var buttons = cut.FindAll("button");
        Assert.Single(buttons.Where(button => button.TextContent.Contains("Test connection", StringComparison.Ordinal)));
        Assert.DoesNotContain(buttons, button => button.TextContent.Trim() is "Save" or "Cancel");
        await buttons.Single(button => button.TextContent.Contains("Test connection", StringComparison.Ordinal)).ClickAsync();
        Assert.NotNull(tested);
        Assert.Equal("Operational SQL", tested!.Name);
    }

    private static OVConnectionEditorValue SqlValue()
        => new(
            Name: "Operational SQL",
            Type: OVConnectionType.Sql,
            Sql: new OVSqlConnectionSettings("SQL01", "OneVision", OVConnectionAuthentication.IntegratedWindows));

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }
}
