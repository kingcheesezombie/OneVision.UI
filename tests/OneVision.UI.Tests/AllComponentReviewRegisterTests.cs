using Xunit;

namespace OneVision.UI.Tests;

/// <summary>
/// Governance guard: every runtime component must retain a visible remediation
/// entry. This checks tracker coverage, not quality certification.
/// </summary>
public sealed class AllComponentReviewRegisterTests
{
    [Fact]
    public void Every_runtime_component_is_in_the_governed_quality_tracker_exactly_once()
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root is not null && !File.Exists(Path.Combine(root.FullName, "OneVision.UI.sln")))
            root = root.Parent;

        Assert.NotNull(root);

        var components = Directory.GetFiles(Path.Combine(root!.FullName, "src", "OneVision.UI", "Components"), "OV*.razor")
            .Select(path => Path.GetFileNameWithoutExtension(path))
            .Order(StringComparer.Ordinal)
            .ToArray();
        var register = File.ReadAllLines(Path.Combine(root.FullName, "docs", "QUALITY-REMEDIATION-TRACKER-20261105.md"))
            .Where(line => line.StartsWith("| " + (char)96 + "OV", StringComparison.Ordinal))
            .Select(line => line.Split('|')[1].Trim().Trim((char)96))
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.True(components.Length >= 92, "The baseline contains 92 runtime components.");
        Assert.Equal(components, register);
    }
}
