using System.Reflection;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class CatalogueManagementContractTests
{
    [Fact]
    public void Catalogue_manager_exposes_governed_list_search_state_and_pagination_contract()
    {
        var parameters = ParametersOf<OVCatalogueManager>();
        foreach (var required in new[]
        {
            nameof(OVCatalogueManager.Title), nameof(OVCatalogueManager.Description), nameof(OVCatalogueManager.ItemName),
            nameof(OVCatalogueManager.ReadOnly), nameof(OVCatalogueManager.AddRequested), nameof(OVCatalogueManager.SearchValue),
            nameof(OVCatalogueManager.SearchValueChanged), nameof(OVCatalogueManager.Filters), nameof(OVCatalogueManager.Loading),
            nameof(OVCatalogueManager.ErrorMessage), nameof(OVCatalogueManager.Empty), nameof(OVCatalogueManager.NoResults),
            nameof(OVCatalogueManager.GridContent), nameof(OVCatalogueManager.Page), nameof(OVCatalogueManager.TotalPages),
            nameof(OVCatalogueManager.TotalCount), nameof(OVCatalogueManager.PageSize), nameof(OVCatalogueManager.PageChanged)
        }) Assert.Contains(required, parameters);

        Assert.Equal(25, new OVCatalogueManager().PageSize);
    }

    [Fact]
    public void Catalogue_manager_composes_existing_governed_primitives()
    {
        var source = ReadRepositoryFile("src/OneVision.UI/Components/OVCatalogueManager.razor");
        Assert.Contains("<OVListPage", source, StringComparison.Ordinal);
        Assert.Contains("<OVSearchBox", source, StringComparison.Ordinal);
        Assert.Contains("<OVDataGrid", source, StringComparison.Ordinal);
        Assert.Contains("<OVPagination", source, StringComparison.Ordinal);
        Assert.Contains("<OVEmptyState", source, StringComparison.Ordinal);
        Assert.Contains("<OVProgress", source, StringComparison.Ordinal);
        Assert.Contains("<OVAlert", source, StringComparison.Ordinal);
    }

    [Fact]
    public void Catalogue_editor_exposes_semantic_add_edit_validation_and_dirty_state_contract()
    {
        var parameters = ParametersOf<OVCatalogueEditor>();
        foreach (var required in new[]
        {
            nameof(OVCatalogueEditor.Open), nameof(OVCatalogueEditor.OpenChanged), nameof(OVCatalogueEditor.Mode),
            nameof(OVCatalogueEditor.Title), nameof(OVCatalogueEditor.Description), nameof(OVCatalogueEditor.ChildContent),
            nameof(OVCatalogueEditor.IsDirty), nameof(OVCatalogueEditor.IsBusy), nameof(OVCatalogueEditor.IsValid),
            nameof(OVCatalogueEditor.ReadOnly), nameof(OVCatalogueEditor.SaveRequested), nameof(OVCatalogueEditor.ValidationSummary)
        }) Assert.Contains(required, parameters);

        var source = ReadRepositoryFile("src/OneVision.UI/Components/OVCatalogueEditor.razor");
        Assert.Contains("<OVOverlay", source, StringComparison.Ordinal);
        Assert.Contains("Placement=\"OVOverlayPlacement.RightSide\"", source, StringComparison.Ordinal);
        Assert.Contains("SaveDisabled=\"@(!IsValid || ReadOnly)\"", source, StringComparison.Ordinal);
    }

    [Fact]
    public void Catalogue_demo_uses_governed_toggle_overflow_and_domain_neutral_reusable_components()
    {
        var demo = ReadRepositoryFile("demo/OneVision.UI.Demo/Components/CatalogueManagementDemo.razor");
        Assert.Contains("<OVCatalogueManager", demo, StringComparison.Ordinal);
        Assert.Contains("<OVCatalogueEditor", demo, StringComparison.Ordinal);
        Assert.Contains("<OVToggle", demo, StringComparison.Ordinal);
        Assert.Contains("<OVOverflowMenu", demo, StringComparison.Ordinal);

        var manager = ReadRepositoryFile("src/OneVision.UI/Components/OVCatalogueManager.razor");
        var editor = ReadRepositoryFile("src/OneVision.UI/Components/OVCatalogueEditor.razor");
        Assert.DoesNotContain("Standard Detail", manager, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Standard Detail", editor, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("IDM", manager, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("IDM", editor, StringComparison.OrdinalIgnoreCase);
    }

    private static HashSet<string> ParametersOf<T>()
        => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

    private static string ReadRepositoryFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "OneVision.UI.sln")))
                return File.ReadAllText(Path.Combine(directory.FullName, relativePath));
            directory = directory.Parent;
        }
        throw new DirectoryNotFoundException("Unable to locate OneVision.UI repository root.");
    }
}
