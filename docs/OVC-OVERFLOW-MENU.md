# OVC-OVERFLOW-MENU — Overflow Menu

## Purpose

`OVOverflowMenu` is the governed compact secondary/destructive action menu for rows, cards and other dense OneVision administrative surfaces.

The trigger remains in the calling composition. The open menu surface is rendered through the OneVision.UI anchored overlay/popover layer so it is not constrained by the caller's DOM clipping context.

## Public contract

The existing public API remains:

- `Items`
- `ItemSelected`
- `AriaLabel`
- `Disabled`
- `AlignRight`
- `Class`

Applications do not need to change existing `OVOverflowMenu` usage to receive clipping-safe positioning.

## Overlay and clipping contract

When open, the menu surface is temporarily portalled outside the trigger hierarchy and positioned in viewport coordinates against the trigger. This allows it to escape legitimate `overflow: hidden`, `overflow: auto`, scrolling table/list wrappers, cards, panels, builders, drawers and other clipping ancestors.

Callers must not change governed containers to `overflow: visible` merely to accommodate an overflow menu, and must not add application-specific popup hosts or z-index overrides.

The anchored popover layer is a lightweight extension of the existing OneVision.UI overlay infrastructure. It is not a second application popup framework.

## Positioning

- `AlignRight = true` remains the default and aligns the menu to the trigger's right edge.
- `AlignRight = false` aligns the menu to the trigger's left edge.
- If there is insufficient useful space below the trigger, the menu opens above it.
- Horizontal positioning is constrained to the viewport so the menu does not avoidably overflow the left/right edge or create page-level horizontal scrolling.
- Resize and scroll events reposition the open menu.
- If the trigger is no longer visible enough to anchor meaningfully, the menu closes rather than remaining detached.

## Interaction

- Only one governed anchored overflow menu is active at a time.
- Clicking/tapping outside the trigger and menu closes it.
- `Escape` closes it.
- Selecting an enabled item closes the menu before/while invoking `ItemSelected`.
- Disabled items remain visible and non-actionable.
- Opening moves focus to the first enabled menu item where possible; closing restores focus to the trigger where the trigger still exists.

## Accessibility

The trigger retains `aria-haspopup="menu"` and an accurate `aria-expanded` state. The popup uses `role="menu"`; actions use `role="menuitem"`. Existing accessible labels, destructive presentation, icons, ordering and disabled behaviour are preserved.

## Governed layering

OneVision.UI currently reserves the following interaction stack:

- normal application content: below governed overlay layers;
- `OVOverlay`: `2000`;
- `OVDialog`: `2050`;
- anchored popovers such as `OVOverflowMenu`: `2080`;
- `OVConfirmDialog`: `2100`.

This ordering allows a menu opened from an overlay/dialog to remain usable while preserving destructive confirmation as the highest governed modal layer. Applications must not compete with this stack through local z-index escalation.

## Ownership

OneVision.UI owns menu rendering, anchoring, collision handling, overlay teardown, focus restoration, outside-click/Escape handling, accessibility and visual density. Applications own the menu items supplied and the domain actions performed by `ItemSelected`.
