using Xunit;

namespace OneVision.UI.Tests;

public sealed class MappingPairCssIsolationContractTests
{
    [Fact]
    public void Dynamic_mapping_descendants_are_anchored_through_static_mapping_pair_scope()
    {
        var css = ReadRepositoryFile("src/OneVision.UI/Components/OVMappingPair.razor.css");

        foreach (var selector in new[]
        {
            ".ov-mapping-pair__target-row",
            ".ov-mapping-pair__target-row--standard-detail",
            ".ov-mapping-pair__target-info",
            ".ov-mapping-pair__standard-detail-identity",
            ".ov-mapping-pair__standard-detail-selector",
            ".ov-mapping-pair__validation",
            ".ov-mapping-pair__drop-zone",
            ".ov-mapping-pair__drop-placeholder",
            ".ov-mapping-pair__mapped-value",
            ".ov-mapping-pair__actions"
        })
        {
            Assert.Contains($".ov-mapping-pair ::deep {selector}", css, StringComparison.Ordinal);
        }

        Assert.Contains(
            ".ov-mapping-pair ::deep .ov-mapping-pair__standard-detail-identity .ov-mapping-pair__standard-detail-change",
            css,
            StringComparison.Ordinal);
    }

    [Fact]
    public void Static_mapping_structure_remains_component_isolated_without_global_stylesheet_escape()
    {
        var css = ReadRepositoryFile("src/OneVision.UI/Components/OVMappingPair.razor.css");

        Assert.Contains(".ov-mapping-pair__layout{", css, StringComparison.Ordinal);
        Assert.Contains(".ov-mapping-pair__target-section{", css, StringComparison.Ordinal);
        Assert.Contains(".ov-mapping-pair__section-header{", css, StringComparison.Ordinal);
        Assert.Contains(".ov-mapping-pair__target-list{", css, StringComparison.Ordinal);
        Assert.Contains(".ov-mapping-pair__standard-empty{", css, StringComparison.Ordinal);
        Assert.DoesNotContain("b-", css, StringComparison.Ordinal);
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
}
