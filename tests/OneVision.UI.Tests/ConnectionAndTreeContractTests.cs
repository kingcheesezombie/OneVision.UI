using System.Reflection;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class ConnectionAndTreeContractTests
{
    [Fact]
    public void Connection_editor_exposes_governed_connection_types_and_test_lifecycle()
    {
        Assert.Equal(new[] { OVConnectionType.Sql, OVConnectionType.File, OVConnectionType.Ldap, OVConnectionType.Json }, Enum.GetValues<OVConnectionType>());
        Assert.Contains(OVConnectionTestState.Testing, Enum.GetValues<OVConnectionTestState>());
        Assert.Contains(OVConnectionTestState.Success, Enum.GetValues<OVConnectionTestState>());
        Assert.Contains(OVConnectionTestState.Error, Enum.GetValues<OVConnectionTestState>());

        var parameters = ParametersOf<OVConnectionEditor>();
        foreach (var required in new[]
        {
            nameof(OVConnectionEditor.Value), nameof(OVConnectionEditor.ValueChanged), nameof(OVConnectionEditor.ReadOnly),
            nameof(OVConnectionEditor.TestState), nameof(OVConnectionEditor.TestMessage), nameof(OVConnectionEditor.TestRequested),
            nameof(OVConnectionEditor.SaveRequested), nameof(OVConnectionEditor.CancelRequested)
        })
            Assert.Contains(required, parameters);
    }

    [Fact]
    public void Connection_editor_value_keeps_each_transport_settings_separate()
    {
        var value = new OVConnectionEditorValue();
        Assert.NotNull(value.SqlSettings);
        Assert.NotNull(value.FileSettings);
        Assert.NotNull(value.LdapSettings);
        Assert.NotNull(value.JsonSettings);
        Assert.Equal(389, value.LdapSettings.Port);
        Assert.Equal("$", value.JsonSettings.RootPath);
    }

    [Fact]
    public void Tree_structure_edit_exposes_explicit_add_move_and_node_action_contracts()
    {
        Assert.Contains(OVTreeMode.StructureEdit, Enum.GetValues<OVTreeMode>());
        Assert.Equal(new[] { OVTreeDropPosition.Before, OVTreeDropPosition.Child, OVTreeDropPosition.After }, Enum.GetValues<OVTreeDropPosition>());

        var parameters = ParametersOf<OVTree>();
        foreach (var required in new[]
        {
            nameof(OVTree.Mode), nameof(OVTree.AllowAddRoot), nameof(OVTree.CanAddChild), nameof(OVTree.CanAddSibling),
            nameof(OVTree.CanMove), nameof(OVTree.CanMoveTo), nameof(OVTree.AddRequested), nameof(OVTree.MoveRequested),
            nameof(OVTree.RenameRequested), nameof(OVTree.DeleteRequested), nameof(OVTree.PropertiesRequested)
        })
            Assert.Contains(required, parameters);
    }

    private static HashSet<string> ParametersOf<T>()
        => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);
}
