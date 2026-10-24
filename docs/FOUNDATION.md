# OVP-000 – OneVision Application Foundation

**Contract version:** 1.5  
**Status:** Current  
**Authority:** `OneVision.UI`

`OVP-000` is the root UI contract for every OneVision application. It is adopted before screen-by-screen component or pattern migration and establishes the governed baseline that all `OVC-*` components, `OVP-*` patterns and application compositions sit on top of.

## Design principle

OneVision is designed around **operational clarity for complex NHS administration** while allowing user-facing experiences to be materially more expressive.

The foundation has two governed presentation profiles over the same component and behaviour contracts:

- **Manager** — compact, restrained, data-dense and operational.
- **Experience** — spacious, guided, expressive and progression-oriented for user-facing journeys.

Both profiles use a Fluent-inspired visual language and Fluent System Icons. MudBlazor remains the implementation layer rather than the application-level contract.

## Adoption contract

An application adopting `OVP-000` must:

1. reference an approved `OneVision.UI` package version;
2. register the library through `AddOneVisionUI()` at the application composition root;
3. load `_content/OneVision.UI/onevision.css`;
4. load `_content/OneVision.UI/onevision-fluent.css` immediately after it;
5. use a governed OneVision presentation profile rather than defining a competing global MudBlazor theme;
6. use `OVP-001` / `OVApplicationShell` for shared application chrome unless an approved exception applies;
7. use governed page-layout patterns where they fit the screen composition;
8. use OneVision semantic iconography (`OneVisionIcon` / `OVIcon`) rather than arbitrary glyph selection;
9. keep domain rules, data, permissions and app-specific behaviour in the application;
10. preserve reduced-motion and focus treatment supplied by the foundation.

## Typography

OneVision now follows a Fluent-style typography hierarchy rather than using Segoe UI Light as a global body treatment.

### Manager

- Segoe UI / Segoe UI Variable Text.
- 13px body with approximately 20px line height.
- regular 400 weight for body copy and dense data.
- 600 weight for headings, selected navigation, table headers and important labels.
- 12px captions/metadata where appropriate.

### Experience

- Segoe UI / Segoe UI Variable Text.
- 15px body with approximately 22px line height.
- 14px secondary body text.
- 22px section headings and 32px+ page/hero titles.
- stronger whitespace and hierarchy rather than making all text simply larger.

## Profile tokens

The foundation governs profile-specific:

- typography scale;
- spacing scale;
- control height/density;
- surface roles;
- radii;
- elevation;
- focus states;
- responsive behaviour;
- motion and reduced-motion behaviour.

Manager defaults to compact controls and low elevation. Experience defaults to comfortable controls, larger spacing, softer geometry and more prominent layered surfaces.

## Colour and surfaces

NHS-style blue remains the OneVision brand anchor.

Manager uses blue primarily for action, selection and meaningful emphasis over neutral white/grey surfaces.

Experience additionally owns governed expressive accents and tinted surfaces:

- cyan/aqua;
- teal;
- violet/indigo;
- warm orange/coral;
- pale blue, cyan, violet and warm supporting surfaces.

These are expressive/shared colours, not replacements for semantic success, warning or danger colours. Applications must not invent arbitrary profile colours.

## Motion and accessibility

Experience may use subtle hover lift, surface transitions, smooth expansion/collapse and overlay entrance where useful. Motion must remain functional and restrained. `prefers-reduced-motion` disables non-essential transitions and animation.

Both profiles retain clear keyboard focus treatment and governed selected/disabled/read-only states.

## Presentation profiles

### Manager

`OneVisionTheme.Manager` is the default profile and `OneVisionTheme.Default` remains a compatibility alias.

Manager favours compact forms, tables, lists and builders; white work surfaces; low elevation; small radii; strong information density; and minimal decorative treatment.

### Experience

`OneVisionTheme.Experience` is not merely Manager with larger padding. It allows profile-specific composition where user-facing guidance benefits from it:

- hero/summary regions;
- stronger visual hierarchy;
- tinted and raised task surfaces;
- richer selection and progress treatment;
- contextual help panels;
- prominent completion/success treatment;
- grouped cards, timelines and guided flows where those better fit the task;
- progressive disclosure of advanced detail;
- more adaptive responsive behaviour.

The same data and domain behaviour can therefore be composed differently by Manager and Experience while using shared OVC primitives and shared behavioural contracts.

## Iconography

OneVision uses **Microsoft Fluent System Icons** through the semantic OneVision layer. Applications reference meanings such as `Search`, `Filter`, `Edit`, `Delete`, `Settings` and `Success`; `OVIcon` resolves the governed glyph.

Important/destructive actions retain text labels unless the icon is universally understood in context.

## Governed shell and layouts

| Reference | Implementation | Purpose |
| --- | --- | --- |
| `OVP-001` | `OVApplicationShell` | Governed application header, identity/logo region, navigation, content host and footer. |
| `OVP-010` | `OVStandardPage` | Standard title/description/actions/content page. |
| `OVP-011` | `OVListPage` | Search/filter/action toolbar, results surface and pagination. |
| `OVP-012` | `OVDetailPage` | Detail/edit page with status, actions, content surface and footer actions. |
| `OVP-013` | `OVBuilderPage` | Two/three-pane builder workspace with navigation, canvas and optional inspector. |
| `OVP-014` | `OVGuidedLandingPage` | Guided landing composition with hero, actions, visual/summary region and staged content; especially suited to Experience. |

## v1.5 contract change

v1.5 changes the externally observable foundation by:

- replacing the former approximately-10pt/light-weight default with a Fluent-style Segoe UI regular/semibold hierarchy;
- introducing explicit profile typography, spacing and control-density tokens;
- making Experience materially more expressive with governed shared accents, tinted surfaces, larger geometry and stronger elevation;
- adding motion/reduced-motion rules;
- allowing profile-specific composition rather than requiring identical DOM/layout treatment;
- adding `OVP-014 / OVGuidedLandingPage` as the first governed guided Experience-oriented composition;
- retaining Manager as the compact operational default.

## Contract identity

```csharp
OneVisionApplicationFoundation.Reference       // OVP-000
OneVisionApplicationFoundation.ContractVersion // 1.5
OneVisionApplicationFoundation.DefaultPresentationProfile // Manager
OneVisionApplicationFoundation.GuidedLandingPageReference // OVP-014
OneVisionApplicationFoundation.Iconography     // Fluent System Icons
```

## Versioning rule

The contract version advances only when an externally observable foundation rule changes. For foundation-affecting changes follow:

`change → contract impact assessment → implementation/tests → contract/demo update → publish → public Pages verification`

## Application migration model

`legacy app → OVP-000 foundation + presentation profile + OVP-001 shell → legacy exceptions identified → OVC/OVP adoption → genuine library gaps resolved → exceptions removed`
