using Bunit;
using MudBlazor.Services;
using Microsoft.Extensions.DependencyInjection;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class CalculationBuilderContractTests
{
    private static readonly IReadOnlyList<OVCalculationOperand> Core =
        [new("num", "Numerator"), new("den", "Denominator")];
    private static readonly IReadOnlyList<OVCalculationOperand> External =
        [new("population", "Population", OVCalculationOperandSource.External)];
    private static readonly IReadOnlyList<OVCalculationFunction> Functions = [new("COUNT", "COUNT")];

    private static OVCalculationToken Token(OVCalculationTokenKind kind, string? key = null, decimal? number = null)
        => new(Guid.NewGuid().ToString("N"), kind, key, number);

    [Fact]
    public void Tier1_arithmetic_structure_is_valid_and_independent_of_format()
    {
        var definition = new OVCalculationDefinition(
            [Token(OVCalculationTokenKind.Operand, "num"), Token(OVCalculationTokenKind.Operator, "/"),
             Token(OVCalculationTokenKind.Operand, "den")]);
        Assert.True(OVCalculationSyntax.Validate(definition, OVCalculationTier.Simple, Core).IsValid);
        Assert.Equal(OVCalculationTier.Simple, OVCalculationSyntax.RequiredTier(definition, Core));
        var format = new OVCalculationFormat("percentage", "0.0%");
        Assert.Equal(3, definition.Tokens.Count);
        Assert.Equal("percentage", format.FormatId);
    }

    [Fact]
    public void Tier2_allows_external_operands_but_does_not_enable_functions()
    {
        var definition = new OVCalculationDefinition(
            [Token(OVCalculationTokenKind.Operand, "population")]);
        Assert.Equal(OVCalculationValidationStatus.UnsupportedTier,
            OVCalculationSyntax.Validate(definition, OVCalculationTier.Simple, [..Core, ..External]).Status);
        Assert.True(OVCalculationSyntax.Validate(definition, OVCalculationTier.Extended, [..Core, ..External]).IsValid);

        var function = new OVCalculationDefinition(
            [Token(OVCalculationTokenKind.Function, "COUNT"), Token(OVCalculationTokenKind.OpenParenthesis),
             Token(OVCalculationTokenKind.Operand, "population"), Token(OVCalculationTokenKind.CloseParenthesis)]);
        Assert.Equal(OVCalculationValidationStatus.UnsupportedTier,
            OVCalculationSyntax.Validate(function, OVCalculationTier.Extended, [..Core, ..External], Functions).Status);
        Assert.True(OVCalculationSyntax.Validate(function, OVCalculationTier.Advanced, [..Core, ..External], Functions).IsValid);
    }

    [Fact]
    public void Malformed_or_unavailable_expressions_are_not_silently_normalised()
    {
        var missing = new OVCalculationDefinition([Token(OVCalculationTokenKind.Operand, "unmapped")]);
        Assert.Equal(OVCalculationValidationStatus.MissingOperand,
            OVCalculationSyntax.Validate(missing, OVCalculationTier.Advanced, Core).Status);
        var incomplete = new OVCalculationDefinition(
            [Token(OVCalculationTokenKind.Operand, "num"), Token(OVCalculationTokenKind.Operator, "*")]);
        Assert.Equal(OVCalculationValidationStatus.Incomplete,
            OVCalculationSyntax.Validate(incomplete, OVCalculationTier.Simple, Core).Status);
    }

    [Fact]
    public async Task Lower_tier_renders_advanced_expression_read_only_without_stripping_tokens()
    {
        await using var ctx = new BunitContext();
        ctx.Services.AddMudServices();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        var advanced = new OVCalculationDefinition(
            [Token(OVCalculationTokenKind.Function, "COUNT"), Token(OVCalculationTokenKind.OpenParenthesis),
             Token(OVCalculationTokenKind.Operand, "population"), Token(OVCalculationTokenKind.CloseParenthesis)]);
        var changes = 0;
        var cut = ctx.Render<OVCalculationBuilder>(p => p
            .Add(x => x.Tier, OVCalculationTier.Simple)
            .Add(x => x.Definition, advanced)
            .Add(x => x.CoreOperands, Core)
            .Add(x => x.ExternalCategories,
                new[] { new OVCalculationOperandCategory("geo", "GEO Measures", External) })
            .Add(x => x.Functions, Functions)
            .Add(x => x.DefinitionChanged, _ => changes++));

        Assert.Contains("COUNT", cut.Markup);
        Assert.Contains("Population", cut.Markup);
        Assert.Contains("shown without modification", cut.Markup);
        Assert.Equal(0, cut.FindAll("button.ov-calculation-builder__remove").Count);
        Assert.Equal(0, changes);
        Assert.Equal(4, advanced.Tokens.Count);
    }
}
