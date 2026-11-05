using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class DateAndNumberQualityTests
{
    [Fact]
    public void Date_validator_uses_inclusive_calendar_day_bounds()
    {
        var date = new DateTime(2026, 11, 5, 22, 47, 0);
        var minimum = new DateTime(2026, 11, 5, 8, 0, 0);
        var maximum = new DateTime(2026, 11, 5, 10, 0, 0);
        Assert.True(OVDateValidation.Evaluate(date, minimum, maximum).IsValid);
        Assert.False(OVDateValidation.Evaluate(date, maxDate: maximum.AddDays(-1)).IsValid);
        Assert.False(OVDateValidation.Evaluate(date, minDate: minimum.AddDays(1)).IsValid);
    }

    [Fact]
    public void Date_validator_reports_missing_required_and_invalid_configured_bounds()
    {
        Assert.True(OVDateValidation.Evaluate(null).IsValid);
        Assert.Equal("A date is required.", OVDateValidation.Evaluate(null, required: true).ErrorMessage);
        Assert.Contains("minimum date", OVDateValidation.Evaluate(
            new DateTime(2026, 1, 1),
            minDate: new DateTime(2026, 12, 31),
            maxDate: new DateTime(2026, 1, 1)).ErrorMessage);
    }

    [Fact]
    public async Task Date_picker_propagates_limits_and_flags_values_outside_them()
    {
        await using var context = Context();
        var date = new DateTime(2026, 9, 15);
        var cut = context.Render<OVDatePicker>(parameters => parameters
            .Add(x => x.Label, "Effective date")
            .Add(x => x.Value, date)
            .Add(x => x.MinDate, new DateTime(2026, 10, 1))
            .Add(x => x.MaxDate, new DateTime(2026, 10, 31)));

        var mud = cut.FindComponent<MudDatePicker>().Instance;
        Assert.True(mud.Error);
        Assert.Contains("before the minimum date", mud.ErrorText);
        Assert.Equal(new DateTime(2026, 10, 1), mud.MinDate);
        Assert.Equal(new DateTime(2026, 10, 31), mud.MaxDate);
    }

    [Fact]
    public async Task Date_picker_change_exposes_validation_and_preserves_attempted_value()
    {
        await using var context = Context();
        DateTime? incoming = null;
        OVDateValidationResult? result = null;

        var cut = context.Render<OVDatePicker>(parameters => parameters
            .Add(x => x.Label, "Date")
            .Add(x => x.Required, true)
            .Add(x => x.ValueChanged, d => incoming = d)
            .Add(x => x.ValidationChanged, v => result = v));

        await cut.InvokeAsync(() => cut.FindComponent<MudDatePicker>().Instance.DateChanged.InvokeAsync(null));
        Assert.Null(incoming);
        Assert.NotNull(result);
        Assert.False(result!.IsValid);
        Assert.Contains("required", result.ErrorMessage);
    }

    [Fact]
    public async Task Date_picker_read_only_and_disabled_guard_change_callbacks()
    {
        await using var context = Context();
        var invoked = false;
        var cut = context.Render<OVDatePicker>(parameters => parameters
            .Add(x => x.Label, "Locked")
            .Add(x => x.ReadOnly, true)
            .Add(x => x.ValueChanged, _ => invoked = true));

        await cut.InvokeAsync(() => cut.FindComponent<MudDatePicker>().Instance.DateChanged.InvokeAsync(new DateTime(2026, 1, 1)));
        Assert.False(invoked);

        var disabled = context.Render<OVDatePicker>(parameters => parameters
            .Add(x => x.Label, "Disabled")
            .Add(x => x.Disabled, true)
            .Add(x => x.ValueChanged, _ => invoked = true));
        await disabled.InvokeAsync(() => disabled.FindComponent<MudDatePicker>().Instance.DateChanged.InvokeAsync(new DateTime(2026, 1, 1)));
        Assert.False(invoked);
    }

    [Fact]
    public async Task Numeric_field_has_optional_typed_inclusive_minimum_and_maximum()
    {
        await using var context = Context();
        var cut = context.Render<OVNumberField<int?>>(parameters => parameters
            .Add(x => x.Label, "Threshold")
            .Add(x => x.Value, (int?)25)
            .Add(x => x.Min, (int?)0)
            .Add(x => x.Max, (int?)100));

        var mud = cut.FindComponent<MudNumericField<int?>>().Instance;
        Assert.Equal((int?)0, mud.Min);
        Assert.Equal((int?)100, mud.Max);
        Assert.Equal((int?)25, mud.Value);
    }

    [Fact]
    public async Task Numeric_field_retains_original_value_and_validation_contract()
    {
        await using var context = Context();
        var cut = context.Render<OVNumberField<decimal?>>(parameters => parameters
            .Add(x => x.Label, "Amount")
            .Add(x => x.Value, (decimal?)1.5m)
            .Add(x => x.Error, true)
            .Add(x => x.ErrorText, "Application constraint failed.")
            .Add(x => x.ReadOnly, true));

        var mud = cut.FindComponent<MudNumericField<decimal?>>().Instance;
        Assert.Equal((decimal?)1.5m, mud.Value);
        Assert.True(mud.ReadOnly);
        Assert.True(mud.Error);
        Assert.Equal("Application constraint failed.", mud.ErrorText);
    }

    private static BunitContext Context()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }
}
