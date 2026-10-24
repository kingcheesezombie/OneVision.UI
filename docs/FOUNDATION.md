# OVP-000 – OneVision Application Foundation

**Contract version:** 1.3  
**Status:** Proposed on `feature/presentation-profiles` pending build/review  
**Authority:** `OneVision.UI`

`OVP-000` is the root UI contract for every OneVision application. It is adopted before screen-by-screen component or pattern migration and establishes the governed baseline that all `OVC-*` components, `OVP-*` patterns and application compositions sit on top of.

## Design principle

OneVision is designed around **operational clarity for complex NHS administration** while allowing user-facing experiences to be more expressive.

The foundation therefore has two governed presentation profiles over the same component and pattern contracts:

- **Manager** — compact, restrained, data-dense and operational. It prioritises scanning, predictable controls, tables, forms, navigation and efficient administration.
- **Experience** — more spacious and expressive for user-facing or guided journeys. It keeps the OneVision identity while using stronger hierarchy, richer blue treatment, larger typography, softer surfaces and more generous spacing.

These are presentation profiles, not separate component libraries. A shared `OVC-*` component keeps the same behaviour contract unless a genuinely different reusable interaction is required.

## Adoption contract

An application adopting `OVP-000` must:

1. reference an approved `OneVision.UI` package version;
2. register the library through `AddOneVisionUI()` at the application composition root;
3. load `_content/OneVision.UI/onevision.css` as the governed shared stylesheet;
4. use a governed OneVision presentation profile and shared providers required by the library rather than defining a competing global MudBlazor theme;
5. use `OVP-001` / `OVApplicationShell` for shared application chrome unless an explicitly approved exception applies;
6. use the governed page-layout patterns where they fit the screen composition;
7. treat OneVision.UI as the owner of shared typography, tokens, density, common states and reusable interaction behaviour;
8. use approved `OVC-*` components and `OVP-*` patterns where they exist;
9. keep domain rules, data, permissions and app-specific composition in the application;
10. document narrowly scoped compatibility exceptions while an application is being migrated rather than recreating a second global design system.

The first adoption state for an existing application is therefore:

> **OVP-000 adopted at the application root, including the governed application shell and presentation profile.**

Temporary visual fallout may expose legacy application overrides that conflict with the governed foundation. Those conflicts should be resolved deliberately. Critical application workflows must remain usable, so a narrow compatibility override is acceptable during migration where required.

## Governed foundation scope

The foundation owns the shared baseline for:

- Segoe UI-led typography and governed type scales;
- NHS-style blue primary treatment and shared colour tokens;
- profile-specific spacing and density;
- common control defaults;
- selected, disabled, read-only, validation and focus states;
- shared overlay/dialog/provider expectations;
- standard action and icon treatment;
- shared accessibility behaviour;
- reusable responsive behaviour where explicitly governed;
- application shell composition and visual treatment;
- common page-layout structure, spacing and surface treatment;
- the common visual assumptions used by OneVision components and patterns.

MudBlazor remains an implementation detail. Applications should not depend on its defaults as their UI contract.

## Presentation profiles

### Manager

`OneVisionTheme.Manager` is the default profile and `OneVisionTheme.Default` remains a compatibility alias to it.

Manager is intended for EAM/HDM/KPI/IDM manager and administration surfaces. Its visual rules favour:

- approximately 10pt body text;
- compact controls and page spacing;
- white working surfaces over a very light cool background;
- restrained borders and shadows;
- NHS blue reserved for action, selection and meaningful emphasis;
- low decorative overhead and minimal unnecessary card nesting;
- dense lists, tables, builders and forms that remain easy to scan.

### Experience

`OneVisionTheme.Experience` is intended for user-facing, guided and more communicative experiences. It uses the same OneVision contracts but allows:

- larger typography and stronger page hierarchy;
- more generous spacing and control heights;
- stronger blue/cyan identity treatment;
- softer, more rounded working surfaces;
- more visible elevation where it helps separate content;
- richer visual feedback and room for graphical/illustrative content.

Applications select the profile that matches the experience. They must not duplicate components solely to obtain a different visual treatment.

## Governed shell and layouts

The approved foundation patterns are:

| Reference | Implementation | Purpose |
| --- | --- | --- |
| `OVP-001` | `OVApplicationShell` | Governed application header, identity/logo region, navigation, content host and footer. |
| `OVP-010` | `OVStandardPage` | Standard title/description/actions/content page with governed spacing. |
| `OVP-011` | `OVListPage` | Search/filter/action toolbar, results surface and pagination. |
| `OVP-012` | `OVDetailPage` | Detail/edit page with status, actions, governed content surface and footer actions. |
| `OVP-013` | `OVBuilderPage` | Two/three-pane builder workspace with navigation, canvas and optional inspector. |

These patterns govern shared structure and behaviour while their presentation responds to the selected profile. The application continues to own its domain-specific content, data and rules.

### Visual baseline

The v1.3 baseline replaces the single undifferentiated visual treatment with deliberate presentation profiles:

- Manager remains the default operational baseline;
- Experience provides a governed expressive treatment for user-facing work;
- both profiles share OneVision identity, accessibility rules and component behaviour;
- profile selection is a presentation decision, not a reason to fork OVC/OVP contracts;
- global tokens drive typography, colour, spacing, density, radii, surfaces and shell treatment before component-specific overrides are considered.

Applications must not replace these globally with competing shell/page conventions. App-specific screens compose inside them unless a deliberate OneVision.UI gap is identified.

## Contract identity

The library exposes `OneVision.UI.Foundation.OneVisionApplicationFoundation` so consuming applications and contract tests can identify the adopted baseline:

```csharp
OneVisionApplicationFoundation.Reference       // OVP-000
OneVisionApplicationFoundation.ContractVersion // 1.3
OneVisionApplicationFoundation.DefaultPresentationProfile // Manager
```

The same type exposes the approved shell/page references and presentation profile names so application tests can verify adoption without relying on appearance.

## Versioning rule

`OVP-000` is a living, versioned contract.

A normal OneVision.UI implementation change does **not** automatically change the contract version. The contract version advances when an externally observable foundation rule changes, including changes to required root integration, governed global appearance, shared state behaviour, required providers/assets, required shell composition or other baseline expectations that consuming applications must account for.

For every foundation-affecting OneVision.UI change:

1. perform a contract-impact assessment;
2. update the implementation;
3. update this contract when the governed rule changes;
4. update the demo/gallery representation;
5. update relevant verification/contract tests where present;
6. advance the `OVP-000` contract version only when the externally observable contract changes;
7. complete the normal source → build/tests → demo publish → public demo repository → GitHub Pages verification path.

The authoritative contract always lives in `OneVision.UI`. `OneVision.UI.Demo` demonstrates and publishes it; the public demo repository is compiled output only.

## Application migration model

The expected migration path is:

`legacy app → OVP-000 foundation + presentation profile + OVP-001 shell adopted → legacy exceptions identified → OVC/OVP adoption → genuine library gaps resolved → exceptions removed → fully governed app`

A screen does not need to be modelled in the UI library before it can be implemented. App-specific screens remain application composition built from the governed foundation and existing reusable contracts.
