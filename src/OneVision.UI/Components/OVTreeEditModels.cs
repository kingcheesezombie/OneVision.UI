namespace OneVision.UI.Components;

public sealed record OVTreeAddRequest(OVTreeAddPosition Position, OVTreeNode? RelativeTo = null);

public sealed record OVTreeMoveRequest(OVTreeNode Node, OVTreeNode Target, OVTreeDropPosition Position);
