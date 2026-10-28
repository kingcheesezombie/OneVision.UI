namespace OneVision.UI.Components;

public enum OVMetadataValueKind
{
    Text,
    Number,
    DateTime,
    Boolean,
    Structured
}

public sealed record OVMetadataItem(
    string Label,
    string? Value,
    string? Group = null,
    OVMetadataValueKind Kind = OVMetadataValueKind.Text,
    bool Copyable = false,
    bool Monospace = false,
    object? RawValue = null,
    string? SecondaryText = null,
    OVDateTimeDisplay DateTimeDisplay = OVDateTimeDisplay.DateTime);
