namespace OneVision.UI.Components;

public enum OVScheduleType
{
    Day,
    Week,
    Month,
    Interval
}

public enum OVScheduleMonthMode
{
    DayOfMonth,
    OrdinalWeekday
}

public sealed record OVScheduleItem(string Key, string Label, string? Description = null, bool Disabled = false);

public sealed record OVScheduleBuilderValue
{
    public string Name { get; init; } = string.Empty;
    public OVScheduleType Type { get; init; } = OVScheduleType.Week;
    public int RepeatEvery { get; init; } = 1;
    public TimeSpan RunTime { get; init; } = new(8, 0, 0);
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public IReadOnlyCollection<DayOfWeek> DaysOfWeek { get; init; } = new[] { DayOfWeek.Monday };
    public OVScheduleMonthMode MonthMode { get; init; } = OVScheduleMonthMode.DayOfMonth;
    public int MonthDay { get; init; } = 1;
    public int MonthOrdinal { get; init; } = 1;
    public DayOfWeek MonthWeekday { get; init; } = DayOfWeek.Monday;
    public int IntervalMinutes { get; init; } = 30;
    public TimeSpan? IntervalWindowStart { get; init; } = new TimeSpan(8, 0, 0);
    public TimeSpan? IntervalWindowEnd { get; init; } = new TimeSpan(18, 0, 0);
    public string? ItemTypeKey { get; init; }
    public IReadOnlyCollection<string> CommittedItemKeys { get; init; } = Array.Empty<string>();
}
