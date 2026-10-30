using System.Reflection;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class CapabilityAssignmentCompositionContractTests
{
    [Fact]
    public void Capability_assignment_uses_existing_workspace_identity_and_settings_contracts()
    {
        var detail = ParametersOf<OVDetailPage>();
        Assert.Contains(nameof(OVDetailPage.Summary), detail);
        Assert.Contains(nameof(OVDetailPage.Busy), detail);
        Assert.Contains(nameof(OVDetailPage.StickyFooter), detail);
        Assert.Contains(nameof(OVDetailPage.FooterContent), detail);
        Assert.Contains(nameof(OVDetailPage.FooterActions), detail);

        var text = ParametersOf<OVTextField>();
        Assert.Contains(nameof(OVTextField.ReadOnly), text);
        Assert.Contains(nameof(OVTextField.Disabled), text);
        Assert.Contains(nameof(OVTextField.ValueChanged), text);

        var toggle = ParametersOf<OVToggle>();
        Assert.Contains(nameof(OVToggle.ReadOnly), toggle);
        Assert.Contains(nameof(OVToggle.Disabled), toggle);
        Assert.Contains(nameof(OVToggle.Saving), toggle);
        Assert.Contains(nameof(OVToggle.ValueChanged), toggle);
    }

    [Fact]
    public void Capability_cards_expose_accessible_assignment_and_guarded_state_contracts()
    {
        var card = ParametersOf<OVCard>();
        foreach (var required in new[]
        {
            nameof(OVCard.Selected),
            nameof(OVCard.Disabled),
            nameof(OVCard.Busy),
            nameof(OVCard.Actionable),
            nameof(OVCard.SelectedAction),
            nameof(OVCard.AriaLabel),
            nameof(OVCard.Status),
            nameof(OVCard.Metadata)
        })
            Assert.Contains(required, card);

        var collection = ParametersOf<OVCardCollection>();
        Assert.Contains(nameof(OVCardCollection.Loading), collection);
        Assert.Contains(nameof(OVCardCollection.ErrorMessage), collection);
        Assert.Contains(nameof(OVCardCollection.Empty), collection);
    }

    [Fact]
    public void Card_collection_owns_stretch_alignment_for_card_rows()
    {
        var css = ReadRepositoryFile("src/OneVision.UI/Components/OVCardCollection.razor.css");
        Assert.Contains("align-items:stretch", css, StringComparison.Ordinal);
    }

    [Fact]
    public void Capability_assignment_composition_does_not_override_collection_alignment()
    {
        var css = ReadRepositoryFile("demo/OneVision.UI.Demo/Components/CapabilityAssignmentWorkspaceDemo.razor.css");
        Assert.DoesNotContain(".ov-capability-demo__group ::deep .ov-card-collection{align-items:", css, StringComparison.Ordinal);
        Assert.DoesNotContain(".ov-capability-demo__group ::deep .ov-card-collection {align-items:", css, StringComparison.Ordinal);
        Assert.DoesNotContain("align-items:start;gap:8px", css, StringComparison.Ordinal);
    }

    [Fact]
    public void Worker_operational_context_uses_summary_and_process_status_contracts()
    {
        var summary = ParametersOf<OVContextSummary>();
        Assert.Contains(nameof(OVContextSummary.Items), summary);
        Assert.Contains(nameof(OVContextSummary.Process), summary);

        var process = ParametersOf<OVProcessStatus>();
        Assert.Contains(nameof(OVProcessStatus.State), process);
        Assert.Contains(nameof(OVProcessStatus.Display), process);
        Assert.Contains(nameof(OVProcessStatus.Phase), process);
        Assert.Contains(nameof(OVProcessStatus.Message), process);
        Assert.Contains(nameof(OVProcessStatus.LastRunAt), process);
    }

    private static string ReadRepositoryFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var solution = Path.Combine(directory.FullName, "OneVision.UI.sln");
            if (File.Exists(solution))
                return File.ReadAllText(Path.Combine(directory.FullName, relativePath));

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Unable to locate OneVision.UI repository root from test output directory.");
    }

    private static HashSet<string> ParametersOf<T>()
        => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);
}
