using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class AccessManagementContractTests
{
    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }

    [Fact]
    public async Task Bootstrap_is_renderable_without_admin_catalogue_or_admin_route()
    {
        await using var context = CreateContext();
        var cut = context.Render<OVAccessManagement>(p => p
            .Add(x => x.Mode, OVAccessMode.Bootstrap)
            .Add(x => x.Features, new OVAccessFeatures(SupportBootstrap: true))
            .Add(x => x.Lifecycle, new OVAccessLifecycle(OVAccessLifecycleState.BootstrapEligible,
                "synthetic:first-admin", OVAccessAvailability.Permit))
            .Add(x => x.BootstrapAdministrator, () => Task.FromResult(OVAccessMutationResult.Success)));

        Assert.Contains("Initial administrator setup", cut.Markup);
        Assert.Contains("synthetic:first-admin", cut.Markup);
        Assert.Contains("Set up initial administrator", cut.Markup);
        Assert.DoesNotContain("Assigned application access", cut.Markup);
    }

    [Fact]
    public async Task Bootstrap_denied_by_server_state_has_no_action()
    {
        await using var context = CreateContext();
        var cut = context.Render<OVAccessManagement>(p => p
            .Add(x => x.Mode, OVAccessMode.Bootstrap)
            .Add(x => x.Features, new OVAccessFeatures(SupportBootstrap: true))
            .Add(x => x.Lifecycle, new OVAccessLifecycle(OVAccessLifecycleState.BootstrapIneligible,
                "synthetic:operator", OVAccessAvailability.Deny("Not authorised"))));

        Assert.DoesNotContain("Set up initial administrator", cut.Markup);
        Assert.Contains("Not authorised", cut.Markup);
    }

    [Fact]
    public async Task Read_only_mode_does_not_expose_mutations()
    {
        await using var context = CreateContext();
        var cut = context.Render<OVAccessManagement>(p => p
            .Add(x => x.Mode, OVAccessMode.InspectOnly)
            .Add(x => x.Roles, new[] { new OVAccessRole("reader", "Reader") })
            .Add(x => x.LoadPrincipals, (OVAccessQuery q, CancellationToken token) =>
                Task.FromResult(new OVAccessPage<OVAccessPrincipal>(
                    [new("opaque-id", "Synthetic Operator", OVAccessPrincipalType.User,
                        Assignments: [new("immutable-assignment-id", "opaque-id", "reader", "Reader")])], 1, 25, 1))));

        cut.WaitForAssertion(() => Assert.Contains("Synthetic Operator", cut.Markup));
        Assert.DoesNotContain("Assign role", cut.Markup);
        Assert.DoesNotContain("Revoke Reader", cut.Markup);
    }

    [Fact]
    public async Task Role_and_baseline_are_not_conflated_with_direct_assignment()
    {
        await using var context = CreateContext();
        var cut = context.Render<OVAccessManagement>(p => p
            .Add(x => x.Mode, OVAccessMode.InspectOnly)
            .Add(x => x.Roles, new[] {
                new OVAccessRole("baseline", "Authenticated", IsImplicit: true),
                new OVAccessRole("reader", "Reader")
            })
            .Add(x => x.LoadPrincipals, (OVAccessQuery q, CancellationToken token) =>
                Task.FromResult(new OVAccessPage<OVAccessPrincipal>(
                    [new("group-x", "Synthetic Group", OVAccessPrincipalType.Group,
                        Assignments: [new("direct-x", "group-x", "reader", "Reader")])], 1, 25, 1))));

        cut.WaitForAssertion(() => Assert.Contains("Synthetic Group", cut.Markup));
        Assert.Contains("Baseline", cut.Markup);
        Assert.Contains("Reader", cut.Markup);
        Assert.DoesNotContain("Revoke Authenticated", cut.Markup);
    }
    [Fact]
    public async Task Protected_assignment_is_not_actionable_even_when_revocation_is_supported()
    {
        await using var context = CreateContext();
        var protectedAssignment = new OVAccessAssignment("protected-assignment", "opaque-admin", "admin", "Administrator",
            Revocability: OVAccessAvailability.Deny("Final administrator protection"));
        var cut = context.Render<OVAccessManagement>(p => p
            .Add(x => x.Features, new OVAccessFeatures())
            .Add(x => x.Roles, new[] { new OVAccessRole("admin", "Administrator") })
            .Add(x => x.RevokeRole, (OVAccessRevokeRequest request) =>
                Task.FromResult(OVAccessMutationResult.Success))
            .Add(x => x.LoadPrincipals, (OVAccessQuery q, CancellationToken token) =>
                Task.FromResult(new OVAccessPage<OVAccessPrincipal>(
                    [new("opaque-admin", "Protected Example", OVAccessPrincipalType.User,
                        Assignments: [protectedAssignment])], 1, 25, 1))));

        cut.WaitForAssertion(() => Assert.Contains("Protected: Final administrator protection", cut.Markup));
        var menu = cut.FindComponent<OVOverflowMenu>();
        Assert.Contains(menu.Instance.Items, item => item.Label == "Revoke Administrator" && item.Disabled);
    }

    [Fact]
    public async Task Bootstrap_failed_server_result_never_reports_success()
    {
        await using var context = CreateContext();
        var attempts = 0;
        var cut = context.Render<OVAccessManagement>(p => p
            .Add(x => x.Mode, OVAccessMode.Bootstrap)
            .Add(x => x.Features, new OVAccessFeatures(SupportBootstrap: true))
            .Add(x => x.Lifecycle, new OVAccessLifecycle(OVAccessLifecycleState.BootstrapEligible,
                "synthetic:first-admin", OVAccessAvailability.Permit))
            .Add(x => x.BootstrapAdministrator, () =>
            {
                attempts++;
                return Task.FromResult(new OVAccessMutationResult(OVAccessMutationCode.Forbidden, "Not authorised"));
            }));

        var buttons = cut.FindAll("button");
        var start = buttons.First(b => b.TextContent.Contains("Set up initial administrator"));
        await start.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());
        var confirmation = cut.FindComponents<OVConfirmDialog>().Single(d => d.Instance.Open);
        await cut.InvokeAsync(() => confirmation.Instance.Confirmed.InvokeAsync());
        Assert.Equal(1, attempts);
        Assert.Contains("Not authorised", cut.Markup);
        Assert.DoesNotContain("Operation confirmed by the application.", cut.Markup);
    }

}
