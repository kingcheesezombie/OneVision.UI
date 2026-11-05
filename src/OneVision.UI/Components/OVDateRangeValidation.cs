using MudBlazor;

namespace OneVision.UI.Components;

/// <summary>
/// Domain-neutral validation of a date interval. Dates are inclusive and compared
/// by calendar day, not by time-of-day or server timezone.
/// </summary>
public static class OVDateRangeValidation
{
    public static OVDateRangeValidationResult Evaluate(
        DateRange? range,
        DateTime? minDate = null,
        DateTime? maxDate = null,
        bool required = false,
        bool requireComplete = false)
    {
        if (minDate.HasValue && maxDate.HasValue && minDate.Value.Date > maxDate.Value.Date)
            return new(false, "The minimum date cannot be after the maximum date.");

        var start = range?.Start?.Date;
        var end = range?.End?.Date;

        if (!start.HasValue && !end.HasValue)
            return required ? new(false, "A start and end date are required.") : new(true, null);

        if ((!start.HasValue || !end.HasValue) && (required || requireComplete))
            return new(false, "Select both a start and an end date.");

        if (start.HasValue && end.HasValue && end.Value < start.Value)
            return new(false, "End date cannot be before start date.");

        if ((start.HasValue && minDate.HasValue && start.Value < minDate.Value.Date) ||
            (end.HasValue && minDate.HasValue && end.Value < minDate.Value.Date))
            return new(false, "The selected dates cannot be before the minimum date.");

        if ((start.HasValue && maxDate.HasValue && start.Value > maxDate.Value.Date) ||
            (end.HasValue && maxDate.HasValue && end.Value > maxDate.Value.Date))
            return new(false, "The selected dates cannot be after the maximum date.");

        return new(true, null);
    }
}

public sealed record OVDateRangeValidationResult(bool IsValid, string? ErrorMessage);
