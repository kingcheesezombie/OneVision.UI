namespace OneVision.UI.Components;

/// <summary>
/// Inclusive, date-only bounds for a single selected calendar day.
/// Any business-specific date rules remain with the consuming application.
/// </summary>
public static class OVDateValidation
{
    public static OVDateValidationResult Evaluate(
        DateTime? value,
        DateTime? minDate = null,
        DateTime? maxDate = null,
        bool required = false)
    {
        if (minDate.HasValue && maxDate.HasValue && minDate.Value.Date > maxDate.Value.Date)
            return new(false, "The minimum date cannot be after the maximum date.");

        if (!value.HasValue)
            return required
                ? new(false, "A date is required.")
                : new(true, null);

        var date = value.Value.Date;
        if (minDate.HasValue && date < minDate.Value.Date)
            return new(false, "The selected date cannot be before the minimum date.");
        if (maxDate.HasValue && date > maxDate.Value.Date)
            return new(false, "The selected date cannot be after the maximum date.");

        return new(true, null);
    }
}

public sealed record OVDateValidationResult(bool IsValid, string? ErrorMessage);
