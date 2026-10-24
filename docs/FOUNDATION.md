# OVP-000 – OneVision Application Foundation

**Contract version:** 1.4  
**Status:** Proposed on `feature/fluent-visual-language` pending build/review  
**Authority:** `OneVision.UI`

`OVP-000` is the root UI contract for every OneVision application. It is adopted before screen-by-screen component or pattern migration and establishes the governed baseline that all `OVC-*` components, `OVP-*` patterns and application compositions sit on top of.

## Design principle

OneVision is designed around **operational clarity for complex NHS administration** while allowing user-facing experiences to be more expressive.

The foundation has two governed presentation profiles over the same component and pattern contracts:

- **Manager** — compact, restrained, data-dense and operational.
- **Experience** — more spacious and expressive for user-facing and guided journeys.

Both profiles now use a **Fluent-inspired visual language**: Segoe UI-led typography, restrained Fluent-style surfaces, interaction/focus treatment, compact radii, subtle elevation, and semantic Fluent System Icons. OneVision retains its NHS-style blue identity and its existing OVC/OVP contracts.

MudBlazor remains the component implementation layer. Fluent is the governed visual language, not a second component contract.

## Adoption contract

An application adopting `OVP-000` must:

1. reference an approved `OneVision.UI` package version;
2. register the library through `AddOneVisionUI()` at the application composition root;
3. load `_content/OneVision.UI/onevision.css`;
4. load `_content/OneVision.UI/onevision-fluent.css` immediately after it;
5. use a governed OneVision presentation profile rather than defining a competing global MudBlazor theme;
6. use `OVP-001` / `OVApplicationShell` for shared application chrome unless an explicitly approved exception applies;
7. use the governed page-layout patterns where they fit the screen composition;
8. use OneVision semantic iconography (`OneVisionIcon` / `OVIcon`) instead of selecting arbitrary Material or Fluent glyphs in applications;
9. use approved `OVC-*` components and `OVP-*` patterns where they exist;
10. keep domain rules, data, permissions and app-specific composition in the application.

## Governed foundation scope

The foundation owns:

- Segoe UI / Segoe UI Variable typography and type scales;
- NHS-style blue primary identity;
- Fluent-inspired neutral palette, spacing, radii, elevation and focus states;
- profile-specific density and expressiveness;
- common control defaults;
- selected, disabled, read-only, validation and focus states;
- dialogs, overlays and common surfaces;
- semantic Fluent System Icons;
- application shell and page-layout treatment;
- shared accessibility and responsive behaviour.

## Presentation profiles

### Manager

`OneVisionTheme.Manager` is the default profile and `OneVisionTheme.Default` remains a compatibility alias.

Manager favours:

- approximately 10pt body text;
- compact controls and page spacing;
- white working surfaces over Fluent-style neutral backgrounds;
- subtle borders and low elevation;
- NHS blue reserved for action, selection and meaningful emphasis;
- dense tables, lists, builders and forms;
- minimal decorative icon use.

### Experience

`OneVisionTheme.Experience` uses the same contracts while allowing:

- larger typography and stronger page hierarchy;
- more generous spacing and control heights;
- stronger blue treatment and expressive surfaces;
- slightly larger radii and elevation;
- more visible semantic iconography and graphical content;
- richer user feedback without becoming visually inconsistent with Manager.

Applications must not duplicate components solely to obtain the alternate presentation.

## Iconography

OneVision uses **Microsoft Fluent System Icons** through a semantic OneVision layer.

Applications reference meanings such as `OneVisionIcon.Search`, `OneVisionIcon.Filter`, `OneVisionIcon.Edit`, `OneVisionIcon.Delete`, `OneVisionIcon.Settings` and `OneVisionIcon.Success`; `OVIcon` resolves the governed glyph. This prevents application code from depending on a specific icon library or choosing inconsistent metaphors.

Important/destructive actions should retain text labels unless the icon is universally understood in context. Colour remains semantic rather than decorative.

## Governed shell and layouts

| Reference | Implementation | Purpose |
| --- | --- | --- |
| `OVP-001` | `OVApplicationShell` | Governed application header, identity/logo region, navigation, content host and footer. |
| `OVP-010` | `OVStandardPage` | Standard title/description/actions/content page. |
| `OVP-011` | `OVListPage` | Search/filter/action toolbar, results surface and pagination. |
| `OVP-012` | `OVDetailPage` | Detail/edit page with status, actions, content surface and footer actions. |
| `OVP-013` | `OVBuilderPage` | Two/three-pane builder workspace with navigation, canvas and optional inspector. |

## v1.4 contract change

v1.4 changes the externally observable foundation by:

- adopting Fluent-inspired visual treatment across both presentation profiles;
- adding the required `onevision-fluent.css` layer;
- making Fluent System Icons the governed icon family;
- introducing semantic `OneVisionIcon` / `OVIcon` usage;
- preserving MudBlazor as an implementation detail rather than replacing the component architecture.

## Contract identity

```csharp
OneVisionApplicationFoundation.Reference       // OVP-000
OneVisionApplicationFoundation.ContractVersion // 1.4
OneVisionApplicationFoundation.DefaultPresentationProfile // Manager
OneVisionApplicationFoundation.Iconography     // Fluent System Icons
```

## Versioning rule

The contract version advances only when an externally observable foundation rule changes. For foundation-affecting changes follow:

`change → contract impact assessment → implementation/tests → contract/demo update → publish → public Pages verification`

## Application migration model

`legacy app → OVP-000 foundation + presentation profile + OVP-001 shell → legacy exceptions identified → OVC/OVP adoption → genuine library gaps resolved → exceptions removed`
