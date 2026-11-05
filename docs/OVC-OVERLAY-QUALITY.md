# Overlay/dialog/overflow quality — cross-pattern assessment

No replacement overlay or application-specific popup convention is authorised. The governed building blocks are `OVOverlay`, `OVDialog`, `OVConfirmDialog` and `OVOverflowMenu`, with established focus containment, nested overlay priority, dirty-confirmation and clipping-safe overflow behaviour.

## Level 1 findings

The existing overlay and overflow tests already exercise focus, scroll locking, Escape, nested close and menu clipping. This review found no sufficiently evidenced new library gap to justify changing those primitives. Consuming patterns should use their existing `EmbeddedInOverlay`/footer-ownership parameters rather than duplicate heading or action bars. A busy save must not silently discard the selected entity/context.

## Level 2 references

`/quality/workflows` exercises target-first allocation creation inside `OVOverlay`, subsequent `OVEntityInspector`, and hierarchy node editing inside a contained overlay with context restored on return. Existing overlay component workbenches remain authoritative for standalone states.

The synthetic reference does not validate the containing application's IIS routing, permission model or mobile host. Do not use local z-index overrides or independent app-specific modal shells. See `docs/GOVERNED-QUALITY-COVERAGE-L1-L2.md`.
