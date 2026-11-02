namespace OneVision.UI.Components;

public enum OVConnectionType
{
    Sql,
    File,
    Ldap,
    Json
}

public enum OVConnectionAuthentication
{
    IntegratedWindows,
    UsernamePassword
}

public enum OVConnectionTestState
{
    Untested,
    Testing,
    Success,
    Error
}

public sealed record OVSqlConnectionSettings(
    string Server = "",
    string Database = "",
    OVConnectionAuthentication Authentication = OVConnectionAuthentication.IntegratedWindows,
    string? Username = null,
    string? Password = null)
{
    public bool Encrypt { get; init; } = true;
    public bool TrustServerCertificate { get; init; }
}

public sealed record OVFileConnectionSettings(
    string Path = "",
    string SearchPattern = "*.*",
    bool IncludeSubdirectories = false);

public sealed record OVLdapConnectionSettings(
    string Host = "",
    int Port = 389,
    string BaseDn = "",
    OVConnectionAuthentication Authentication = OVConnectionAuthentication.IntegratedWindows,
    string? Username = null,
    string? Password = null);

public sealed record OVJsonConnectionSettings(
    string Source = "",
    string RootPath = "$");

public sealed record OVConnectionEditorValue(
    string Name = "",
    string? Description = null,
    bool Enabled = true,
    OVConnectionType Type = OVConnectionType.Sql,
    OVSqlConnectionSettings? Sql = null,
    OVFileConnectionSettings? File = null,
    OVLdapConnectionSettings? Ldap = null,
    OVJsonConnectionSettings? Json = null)
{
    public OVSqlConnectionSettings SqlSettings => Sql ?? new();
    public OVFileConnectionSettings FileSettings => File ?? new();
    public OVLdapConnectionSettings LdapSettings => Ldap ?? new();
    public OVJsonConnectionSettings JsonSettings => Json ?? new();
}
