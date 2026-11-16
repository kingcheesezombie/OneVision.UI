namespace OneVision.UI.Components;

/// <summary>Access management is UI-only. Consuming applications own authority, routes and persistence.</summary>
public enum OVAccessMode { Manage, InspectOnly, Bootstrap, Recovery }
public enum OVAccessPrincipalType { User, Group, Other }
public enum OVAccessGrantOrigin { Direct, Group, Baseline, Other }
public enum OVAccessEvaluationQuality { Complete, Partial, Stale, Unavailable }
public enum OVAccessOutcome { Granted, Denied, Unavailable }
public enum OVAccessLifecycleState
{
    NotConfigured, BootstrapEligible, BootstrapIneligible, BootstrapInProgress,
    Configured, RecoveryRequired, RecoveryEligible, RecoveryIneligible, Failed
}
public enum OVAccessMutationCode
{
    Success, AlreadyAssigned, AlreadyRevoked, Conflict, Forbidden, PrincipalUnavailable,
    StateChanged, LastAdministratorProtected, Invalid, Failed
}

public sealed record OVAccessFeatures(
    bool AllowRoleAssignment = true,
    bool AllowRoleRevocation = true,
    bool ShowEffectivePermissions = false,
    bool ShowAuditHistory = false,
    bool SupportGroupPrincipals = true,
    bool SupportBootstrap = false,
    bool SupportRecovery = false);

/// <summary>Feature support is not the user's authority. Application-supplied permissions are a separate input.</summary>
public sealed record OVAccessAvailability(bool Allowed, string? Reason = null)
{
    public static OVAccessAvailability Permit { get; } = new(true);
    public static OVAccessAvailability Deny(string reason) => new(false, reason);
}

public sealed record OVAccessPrincipal(
    string PrincipalId,
    string DisplayName,
    OVAccessPrincipalType Type,
    string? SecondaryLabel = null,
    string? Provider = null,
    string? Status = null,
    IReadOnlyList<OVAccessAssignment>? Assignments = null,
    OVAccessAvailability? AssignmentAvailability = null);

public sealed record OVAccessPermission(string PermissionId, string Name, string? Description = null, string? Group = null);

public sealed record OVAccessRole(
    string RoleId, string Name, string? Description = null,
    IReadOnlyList<OVAccessPermission>? Permissions = null,
    OVAccessAvailability? Assignability = null,
    bool IsImplicit = false);

public sealed record OVAccessAssignment(
    string AssignmentId, string PrincipalId, string RoleId,
    string RoleName, string? Revision = null, string? Status = null,
    OVAccessAvailability? Revocability = null, string? RevokeImpact = null);

public sealed record OVAccessQuery(string? Search, int Page = 1, int PageSize = 25);
public sealed record OVAccessPage<T>(
    IReadOnlyList<T> Items, int Page, int PageSize, long? TotalCount = null,
    bool HasNextPage = false, DateTimeOffset? AsOf = null);

public sealed record OVAccessGrantRequest(
    string PrincipalId, string RoleId, string? OperationId = null);

public sealed record OVAccessRevokeRequest(
    string AssignmentId, string? ExpectedRevision = null, string? OperationId = null);

public sealed record OVAccessMutationResult(OVAccessMutationCode Code, string? Message = null)
{
    public bool Succeeded => Code == OVAccessMutationCode.Success;
    public static OVAccessMutationResult Success { get; } = new(OVAccessMutationCode.Success);
}

public sealed record OVAccessProvenance(
    OVAccessGrantOrigin Origin, string Description,
    string? RoleId = null, string? AssignmentId = null, string? Lineage = null);

public sealed record OVAccessPermissionResult(
    string PermissionId, string PermissionName, OVAccessOutcome Outcome,
    IReadOnlyList<OVAccessProvenance> Sources, string? Explanation = null);

public sealed record OVAccessEvaluation(
    OVAccessEvaluationQuality Quality, IReadOnlyList<OVAccessPermissionResult> Permissions,
    DateTimeOffset? EvaluatedAt = null, string? Scope = null, string? Message = null);

public sealed record OVAccessAuditRecord(
    string EventId, DateTimeOffset At, string Actor, string Action,
    string Principal, string? Role, string Outcome, string? Detail = null);

public sealed record OVAccessAuditPage(
    OVAccessPage<OVAccessAuditRecord> Page, string? CompletenessNotice = null);

/// <summary>
/// The application provides the authenticated identity and server-evaluated lifecycle state.
/// Access to an application-owned bootstrap route must not depend on a pre-existing admin role.
/// </summary>
public sealed record OVAccessLifecycle(
    OVAccessLifecycleState State,
    string AuthenticatedPrincipal,
    OVAccessAvailability OperationAvailability,
    string? Explanation = null, string? RecoveryReason = null);
