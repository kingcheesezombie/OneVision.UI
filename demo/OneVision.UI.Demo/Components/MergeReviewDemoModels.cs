using System.Globalization;
using OneVision.UI.Components;

namespace OneVision.UI.Demo.Components;

internal static class MergeReviewDemoData
{
    public static List<MergeReviewGroup> BuildGroups() =>
    [
        new("MG-001", "Group 001", 92, "Ready", "2 plausible identities from 3 candidates", "Exact identity + demographic corroboration",
            "EAM entity links from two source systems; no asset is shared across multiple Persons.", "Synthetic matching diagnostics",
            [
                new("P-100184", "Amelia", "Hart", "14 Mar 1987", "999 000 1234", "Female", "B15 2TT", "Primary feed", "2 allocations", "Name, DOB and NHS number agree"),
                new("P-101906", "Amelia", "Hart", "14 Mar 1987", "999 000 1234", "Female", "B15 2TU", "Secondary feed", "1 allocation", "Name, DOB and NHS number agree; postcode differs"),
                new("P-104211", "Amelia", "Harte", "14 Mar 1987", "999 000 1234", "Female", "B15 2TT", "Legacy import", "1 allocation", "DOB and NHS number agree; surname differs")
            ], "P-100184", true),
        new("MG-002", "Group 002", 86, "Ambiguous", "one source asset plausibly matches two Persons", "Demographic match + ambiguous asset",
            "One synthetic asset is plausibly associated with more than one Person and requires review.", "Synthetic matching diagnostics",
            [
                new("P-110022", "Lewis", "Morgan", "02 Nov 1976", "999 000 4567", "Male", "B30 1AA", "Primary feed", "1 allocation", "Name and DOB agree"),
                new("P-112203", "Lewis", "Morgan", "02 Nov 1976", "999 000 4567", "Male", "B30 1AA", "Secondary feed", "1 allocation", "Shared asset creates ambiguity")
            ], "P-110022", false),
        new("MG-003", "Group 003", 95, "Ready", "2 candidates; low ambiguity", "Exact identity match",
            "Distinct source allocations corroborate the same proposed identity.", "Synthetic matching diagnostics",
            [
                new("P-120341", "Noah", "Evans", "21 Jun 1991", "999 000 8890", "Male", "B1 4QA", "Primary feed", "3 allocations", "All configured identity fields agree"),
                new("P-120955", "Noah", "Evans", "21 Jun 1991", "999 000 8890", "Male", "B1 4QA", "Secondary feed", "2 allocations", "All configured identity fields agree")
            ], "P-120341", false)
    ];

    public static string PersonName(MergeReviewCandidate candidate) => $"{candidate.Forename} {candidate.Surname}";

    public static List<MergeReviewCandidate> IncludedCandidates(MergeReviewGroup group)
        => group.Candidates.Where(candidate => !candidate.Removed).ToList();

    public static MergeReviewCandidate? PrimaryCandidate(MergeReviewGroup group)
        => group.Candidates.FirstOrDefault(candidate => !candidate.Removed && candidate.PersonCode == group.PrimaryPersonCode);

    public static IReadOnlyList<MergeReviewEvidence> EvidenceRows(IReadOnlyList<MergeReviewCandidate> included)
        =>
        [
            Evidence("Forename", included.Select(candidate => candidate.Forename)),
            Evidence("Surname", included.Select(candidate => candidate.Surname)),
            Evidence("Date of birth", included.Select(candidate => candidate.DateOfBirth)),
            Evidence("Postcode", included.Select(candidate => candidate.Postcode))
        ];

    public static IReadOnlyList<OVMetadataItem> EvidenceMetadata(MergeReviewGroup group, IReadOnlyList<MergeReviewCandidate> included)
        =>
        [
            new("Matching rule", group.Rule, "Proposal evidence"),
            new("Confidence", $"{group.Confidence:0}%", "Proposal evidence"),
            new("Ambiguity", group.AmbiguityText, "Proposal evidence"),
            new("Included people", included.Count.ToString(CultureInfo.InvariantCulture), "Proposal evidence", OVMetadataValueKind.Number),
            new("Allocation evidence", group.AllocationEvidence, "Source evidence", SecondaryText: "Multiple legitimate assets for one Person are not themselves a conflict."),
            new("Evidence source", group.EvidenceSource, "Source evidence")
        ];

    public static IReadOnlyList<OVMetadataItem> ResultMetadata(MergeReviewCandidate primary, IReadOnlyList<MergeReviewCandidate> included)
    {
        var surnameCount = included.Select(candidate => candidate.Surname).Distinct(StringComparer.OrdinalIgnoreCase).Count();
        var postcodeCount = included.Select(candidate => candidate.Postcode).Distinct(StringComparer.OrdinalIgnoreCase).Count();
        return
        [
            new("Surviving Person", PersonName(primary), "Identity", SecondaryText: "Selected primary"),
            new("IDM Person Code", primary.PersonCode, "Identity", Copyable: true, Monospace: true),
            new("Forename", primary.Forename, "Result"),
            new("Surname", surnameCount == 1 ? primary.Surname : "Unresolved by UI composition", "Result", SecondaryText: surnameCount == 1 ? null : "Application merge rules must supply the resulting value."),
            new("DOB", primary.DateOfBirth, "Result"),
            new("Postcode", postcodeCount == 1 ? primary.Postcode : "Unresolved by UI composition", "Result", SecondaryText: postcodeCount == 1 ? null : "Application merge rules must supply the resulting value."),
            new("Included source records", included.Count.ToString(CultureInfo.InvariantCulture), "Result", OVMetadataValueKind.Number)
        ];
    }

    private static MergeReviewEvidence Evidence(string label, IEnumerable<string> rawValues)
    {
        var values = rawValues.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var agrees = values.Length == 1;
        return new(label, string.Join(" / ", values), agrees ? "Agree" : "Conflict", agrees ? OVPillTone.Success : OVPillTone.Warning);
    }
}

internal sealed class MergeReviewGroup(
    string id, string label, double confidence, string status, string ambiguityText, string rule,
    string allocationEvidence, string evidenceSource, List<MergeReviewCandidate> candidates,
    string primaryPersonCode, bool hasPostcodeConflict)
{
    public string Id { get; } = id;
    public string Label { get; } = label;
    public double Confidence { get; } = confidence;
    public string Status { get; } = status;
    public string AmbiguityText { get; } = ambiguityText;
    public string Rule { get; } = rule;
    public string AllocationEvidence { get; } = allocationEvidence;
    public string EvidenceSource { get; } = evidenceSource;
    public List<MergeReviewCandidate> Candidates { get; } = candidates;
    public string PrimaryPersonCode { get; set; } = primaryPersonCode;
    public bool HasPostcodeConflict { get; } = hasPostcodeConflict;
}

internal sealed class MergeReviewCandidate(
    string personCode, string forename, string surname, string dateOfBirth, string nhsNumber,
    string sex, string postcode, string source, string allocationSummary, string evidenceSummary)
{
    public string PersonCode { get; } = personCode;
    public string Forename { get; } = forename;
    public string Surname { get; } = surname;
    public string DateOfBirth { get; } = dateOfBirth;
    public string NhsNumber { get; } = nhsNumber;
    public string Sex { get; } = sex;
    public string Postcode { get; } = postcode;
    public string Source { get; } = source;
    public string AllocationSummary { get; } = allocationSummary;
    public string EvidenceSummary { get; } = evidenceSummary;
    public bool Removed { get; set; }
}

internal sealed record MergeReviewEvidence(string Label, string Detail, string Result, OVPillTone Tone);
