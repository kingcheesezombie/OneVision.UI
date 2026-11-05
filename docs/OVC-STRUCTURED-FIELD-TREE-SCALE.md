# OVC-STRUCTURED-FIELD-TREE — scalable source lookup

## Observed cost

The existing structured tree wraps source nodes in the shared OVTree and resolves incoming source-node keys in drag/drop and selection-related callbacks.

Previously Find(key) recursively enumerated the entire source tree on **each lookup**. When the same lookup was evaluated for multiple visible nodes (for example, drag/drop eligibility), this could approach quadratic work for large metadata trees.

## Compatible remediation

- Rebuild a dictionary mapping source Key to OVStructuredFieldNode only when the already-governed tree-content signature changes.
- All row/event handlers use an O(1) dictionary lookup instead of a repeated complete walk.
- The index uses an iterative depth-first preorder traversal so it does not introduce additional recursive stack usage.
- Preserve legacy behavior when duplicate keys occur: the **first preorder occurrence wins**.
- Replacing the source metadata with different content rebuilds both the shared tree nodes and the lookup together. No cached source survives a semantic signature change.
- Drag/drop permissions, record-collection selection, read-only behavior, presentation, accessibility and API callbacks remain unchanged.

## Shared OVTree depth resilience

The shared OVTree also had recursive helper traversals for flattening selected nodes and preventing a structure-edit move into a descendant. Both traversals now use iterative depth-first preorder stacks.

This preserves existing selection order and cycle-prevention rules while avoiding stack overflow **in those specific helper paths** for very deep input trees. Other functions, including tree-item construction, matching/expansion, and rendering, are not certified for 10,000 nested levels; no broad scalability claim follows from these narrow fixes.

## Verification

The unit contract checks a 10,000-entry structured input and verifies first/last lookup, missing keys, duplicate-key precedence, stale-source removal and empty inputs. Separate shared OVTree tests exercise 10,000-level flatten and descendant checks.

This is a targeted algorithmic L4 remediation. It does **not** certify rendering 10,000 expanded nodes in a real browser. Virtualisation, 200%/400% zoom, deep nesting and consumer-level dataset sizes must be evaluated separately before a full 4.5/5 certificate.

## Governance

No new user-facing component, application rule, data acquisition contract or foundation change. Applications still supply source metadata and own remote metadata loading; OVTree retains presentation and interaction governance.
