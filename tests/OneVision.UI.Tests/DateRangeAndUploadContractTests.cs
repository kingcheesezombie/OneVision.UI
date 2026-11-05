using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class DateRangeAndUploadContractTests
{
    [Fact]
    public void Date_range_validation_is_inclusive_and_date_only()
    {
        var min = new DateTime(2026, 1, 1, 23, 0, 0);
        var max = new DateTime(2026, 12, 31, 0, 0, 0);
        Assert.True(OVDateRangeValidation.Evaluate(
            new DateRange(min.Date, max.Date), min, max).IsValid);

        Assert.False(OVDateRangeValidation.Evaluate(
            new DateRange(max, min), min, max).IsValid);
        Assert.Contains("End date", OVDateRangeValidation.Evaluate(
            new DateRange(max, min)).ErrorMessage);
    }

    [Fact]
    public void Date_range_checks_partial_required_and_bounds_without_inventing_end_dates()
    {
        var start = new DateTime(2026, 2, 2);
        var openEnded = new DateRange(start, null);
        Assert.True(OVDateRangeValidation.Evaluate(openEnded).IsValid);
        Assert.False(OVDateRangeValidation.Evaluate(openEnded, requireComplete: true).IsValid);
        Assert.True(OVDateRangeValidation.Evaluate(null).IsValid);
        Assert.False(OVDateRangeValidation.Evaluate(null, required: true).IsValid);

        Assert.False(OVDateRangeValidation.Evaluate(openEnded, minDate: start.AddDays(1)).IsValid);
        Assert.False(OVDateRangeValidation.Evaluate(new DateRange(start, start.AddDays(10)),
            maxDate: start.AddDays(9)).IsValid);
        Assert.False(OVDateRangeValidation.Evaluate(openEnded,
            minDate: start.AddDays(10), maxDate: start.AddDays(1)).IsValid);
    }

    [Fact]
    public async Task Date_range_wrapper_passes_constraints_and_exposes_governed_error()
    {
        await using var ctx = NewContext();
        var cut = ctx.Render<OVDateRangePicker>(p => p
            .Add(x => x.Label, "Effective period")
            .Add(x => x.MinDate, new DateTime(2026, 1, 1))
            .Add(x => x.MaxDate, new DateTime(2026, 12, 31))
            .Add(x => x.Value, new DateRange(new DateTime(2026, 10, 10), new DateTime(2026, 10, 1))));

        var picker = cut.FindComponent<MudDateRangePicker>().Instance;
        Assert.True(picker.Error);
        Assert.Contains("End date", picker.ErrorText);
        Assert.Equal(new DateTime(2026, 1, 1), picker.MinDate);
        Assert.Equal(new DateTime(2026, 12, 31), picker.MaxDate);
    }

    [Fact]
    public async Task Date_range_change_reports_validation_without_rejecting_application_owned_value()
    {
        await using var ctx = NewContext();
        var range = new DateRange(new DateTime(2026, 8, 1), null);
        DateRange? supplied = null;
        OVDateRangeValidationResult? checkedValue = null;

        var cut = ctx.Render<OVDateRangePicker>(p => p
            .Add(x => x.Label, "Dates")
            .Add(x => x.RequireComplete, true)
            .Add(x => x.ValueChanged, value => supplied = value)
            .Add(x => x.ValidationChanged, value => checkedValue = value));

        await cut.InvokeAsync(() => cut.FindComponent<MudDateRangePicker>()
            .Instance.DateRangeChanged.InvokeAsync(range));

        Assert.Same(range, supplied);
        Assert.NotNull(checkedValue);
        Assert.False(checkedValue!.IsValid);
    }

    [Fact]
    public async Task File_upload_can_remove_one_of_two_identically_named_files_by_identity()
    {
        await using var ctx = NewContext();
        var first = new StubFile("same.csv");
        var second = new StubFile("same.csv");
        IBrowserFile? removed = null;

        var cut = ctx.Render<OVFileUpload>(p => p
            .Add(x => x.SelectedFiles, new IBrowserFile[] { first, second })
            .Add(x => x.RemoveFileRequested, file => removed = file));

        var actions = cut.FindAll("button[aria-label^='Remove same.csv']");
        Assert.Equal(2, actions.Count);
        Assert.NotEqual(actions[0].GetAttribute("aria-label"), actions[1].GetAttribute("aria-label"));

        actions[1].Click();
        Assert.Same(second, removed);
    }

    [Fact]
    public async Task File_upload_read_only_keeps_file_list_but_suppresses_mutation()
    {
        await using var ctx = NewContext();
        var file = new StubFile("sample.json");
        var cut = ctx.Render<OVFileUpload>(p => p
            .Add(x => x.SelectedFiles, new IBrowserFile[] { file })
            .Add(x => x.ReadOnly, true)
            .Add(x => x.RemoveRequested, _ => { }));

        Assert.Contains("sample.json", cut.Markup);
        Assert.NotNull(cut.Find("input[type=file]").GetAttribute("disabled"));
        Assert.Empty(cut.FindAll("button[aria-label^='Remove']"));
    }

    [Fact]
    public async Task Legacy_file_removal_callback_remains_valid()
    {
        await using var ctx = NewContext();
        string? removed = null;
        var cut = ctx.Render<OVFileUpload>(p => p
            .Add(x => x.SelectedFiles, new IBrowserFile[] { new StubFile("old.csv") })
            .Add(x => x.RemoveRequested, name => removed = name));

        cut.Find("button[aria-label='Remove old.csv']").Click();
        Assert.Equal("old.csv", removed);
    }

    private static BunitContext NewContext()
    {
        var ctx = new BunitContext();
        ctx.Services.AddMudServices();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        return ctx;
    }

    private sealed class StubFile(string name) : IBrowserFile
    {
        public string Name => name;
        public DateTimeOffset LastModified => DateTimeOffset.UnixEpoch;
        public long Size => 8;
        public string ContentType => "text/plain";
        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
            => new MemoryStream(new byte[8]);
    }
}
