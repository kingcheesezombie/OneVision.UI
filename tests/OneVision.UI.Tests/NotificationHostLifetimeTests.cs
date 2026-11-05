using System.Reflection;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class NotificationHostLifetimeTests
{
    [Fact]
    public async Task Pausing_preserves_remaining_time_instead_of_restarting_the_full_timeout()
    {
        await using var context = Context(out var service, out var clock);
        var cut = Host(context, clock);
        service.Show(new(OVNotificationTone.Information, "Timed notice", Timeout: TimeSpan.FromSeconds(10)));
        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(".ov-notification-host__item")));

        clock.Advance(TimeSpan.FromSeconds(6));
        await cut.InvokeAsync(() => cut.Find(".ov-notification-host__item").TriggerEvent("onmouseenter", new MouseEventArgs()));

        // While hovered, advancing beyond the original expiry must do nothing.
        clock.Advance(TimeSpan.FromSeconds(20));
        Assert.Single(cut.FindAll(".ov-notification-host__item"));

        await cut.InvokeAsync(() => cut.Find(".ov-notification-host__item").TriggerEvent("onmouseleave", new MouseEventArgs()));
        clock.Advance(TimeSpan.FromSeconds(3));
        Assert.Single(cut.FindAll(".ov-notification-host__item"));

        // Four seconds, not ten, remain when the pointer leaves.
        clock.Advance(TimeSpan.FromSeconds(5));
        cut.WaitForAssertion(() => Assert.Empty(cut.FindAll(".ov-notification-host__item")), TimeSpan.FromSeconds(3));
    }

    [Fact]
    public async Task Leaving_pointer_does_not_resume_when_keyboard_focus_remains_inside()
    {
        await using var context = Context(out var service, out var clock);
        var cut = Host(context, clock);
        service.Show(new(OVNotificationTone.Warning, "Review required", Timeout: TimeSpan.FromSeconds(5)));
        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(".ov-notification-host__item")));

        await cut.InvokeAsync(() => cut.Find(".ov-notification-host__item").TriggerEvent("onmouseenter", new MouseEventArgs()));
        await cut.InvokeAsync(() => cut.Find(".ov-notification-host__item").TriggerEvent("onfocusin", new FocusEventArgs()));
        await cut.InvokeAsync(() => cut.Find(".ov-notification-host__item").TriggerEvent("onmouseleave", new MouseEventArgs()));

        clock.Advance(TimeSpan.FromSeconds(20));
        Assert.Single(cut.FindAll(".ov-notification-host__item"));

        await cut.InvokeAsync(() => cut.Find(".ov-notification-host__item").TriggerEvent("onfocusout", new FocusEventArgs()));
        clock.Advance(TimeSpan.FromSeconds(6));
        cut.WaitForAssertion(() => Assert.Empty(cut.FindAll(".ov-notification-host__item")));
    }

    [Fact]
    public async Task Replacement_key_preserves_the_existing_queue_and_avoids_stale_timers()
    {
        await using var context = Context(out var service, out var clock);
        var cut = Host(context, clock, maximumVisible: 1);

        service.Show(new(OVNotificationTone.Information, "Initial", Persistent: true, ReplacementKey: "same"));
        service.Show(new(OVNotificationTone.Information, "Queued", Persistent: true));
        service.Show(new(OVNotificationTone.Success, "Replacement", Persistent: true, ReplacementKey: "same"));

        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("Initial", cut.Markup);
            Assert.Contains("Replacement", cut.Markup);
            Assert.DoesNotContain("Queued", cut.Markup);
            Assert.Single(cut.FindAll(".ov-notification-host__item"));
        });

        cut.Find("button[aria-label='Dismiss notification']").Click();
        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("Replacement", cut.Markup);
            Assert.Contains("Queued", cut.Markup);
        });
    }

    [Fact]
    public async Task Disposal_cancels_timers_and_ignores_later_service_events()
    {
        await using var context = Context(out var service, out var clock);
        var cut = Host(context, clock);
        service.Show(new(OVNotificationTone.Success, "Before dispose", Timeout: TimeSpan.FromSeconds(2)));
        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(".ov-notification-host__item")));

        await context.DisposeAsync();
        clock.Advance(TimeSpan.FromSeconds(30));
        service.Show(new(OVNotificationTone.Success, "After dispose"));

        // Disposing the host must unsubscribe its event handler. No new timer
        // or render callback may be registered after this point.
        var handlers = typeof(OVNotificationService)
            .GetField("NotificationRaised", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(service);
        Assert.Null(handlers);
    }

    private static BunitContext Context(out OVNotificationService service, out ManualClock clock)
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        service = new OVNotificationService();
        context.Services.AddSingleton(service);
        clock = new ManualClock();
        return context;
    }

    private static IRenderedComponent<OVNotificationHost> Host(BunitContext context, TimeProvider clock, int maximumVisible = 4) =>
        context.Render<OVNotificationHost>(parameters => parameters
            .Add(host => host.Clock, clock)
            .Add(host => host.MaximumVisible, maximumVisible)
            .Add(host => host.MinimumTimeout, TimeSpan.FromMilliseconds(10))
            .Add(host => host.MaximumTimeout, TimeSpan.FromMinutes(1)));

    /// <summary>
    /// Monotonic fake clock with manually fired one-shot timers.
    /// Keeps expiry regression tests deterministic without real sleeping.
    /// </summary>
    private sealed class ManualClock : TimeProvider
    {
        private long _ticks;
        private readonly List<ManualTimer> _timers = [];

        public override long TimestampFrequency => TimeSpan.TicksPerSecond;
        public override long GetTimestamp() => _ticks;
        public override DateTimeOffset GetUtcNow() => DateTimeOffset.UnixEpoch.AddTicks(_ticks);

        public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
        {
            var timer = new ManualTimer(this, callback, state, dueTime, period);
            _timers.Add(timer);
            return timer;
        }

        public void Advance(TimeSpan duration)
        {
            _ticks += duration.Ticks;
            foreach (var timer in _timers.ToArray())
                if (!timer.Disposed && timer.DueAt <= _ticks)
                    timer.Fire();
        }

        private sealed class ManualTimer : ITimer
        {
            private readonly ManualClock _owner;
            private readonly TimerCallback _callback;
            private readonly object? _state;
            private TimeSpan _period;

            public ManualTimer(ManualClock owner, TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
            {
                _owner = owner;
                _callback = callback;
                _state = state;
                Change(dueTime, period);
            }

            public bool Disposed { get; private set; }
            public long DueAt { get; private set; }

            public bool Change(TimeSpan dueTime, TimeSpan period)
            {
                if (Disposed) return false;
                _period = period;
                DueAt = dueTime == Timeout.InfiniteTimeSpan ? long.MaxValue : _owner._ticks + dueTime.Ticks;
                return true;
            }

            public void Fire()
            {
                if (Disposed) return;
                if (_period == Timeout.InfiniteTimeSpan)
                    Disposed = true;
                else
                    DueAt = _owner._ticks + _period.Ticks;
                _callback(_state);
            }

            public void Dispose() => Disposed = true;

            public ValueTask DisposeAsync()
            {
                Dispose();
                return ValueTask.CompletedTask;
            }
        }
    }
}
