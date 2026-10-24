# OVP-000 – OneVision Application Foundation

**Contract version:** 1.0  
**Status:** Approved baseline  
**Authority:** `OneVision.UI`

`OVP-000` is the root UI contract for every OneVision application. It is adopted before screen-by-screen component or pattern migration and establishes the governed baseline that all `OVC-*` components, `OVP-*` patterns and application compositions sit on top of.

## Adoption contract

An application adopting `OVP-000` must:

1. reference an approved `OneVision.UI` package version;
2. register the library through `AddOneVisionUI()` at the application composition root;
3. load `_content/OneVision.UI/onevision.css` as the governed shared stylesheet;
4. use the OneVision theme and shared providers required by the library rather than defining a competing global MudBlazor theme;
5. treat OneVision.UI as the owner of shared typography, tokens, density, common states and reusable interaction behaviour;
6. use approved `OVC-*` components and `OVP-*` patterns where they exist;
7. keep domain rules, data, permissions and app-specific composition in the application;
8. document narrowly scoped compatibility exceptions while an application is being migrated rather than recreating a second global design system.

The first adoption state for an existing application is therefore:

> **OVP-000 adopted at the application root.**

Temporary visual fallout may expose legacy application overrides that conflict with the governed foundation. Those conflicts should be resolved deliberately. Critical application workflows must remain usable, so a narrow compatibility override is acceptable during migration where required.

## Governed foundation scope

The foundation owns the shared baseline for:

- typography and base sizing;
- design tokens and shared colours;
- spacing and administrative density;
- common control defaults;
- selected, disabled, read-only, validation and focus states;
- shared overlay/dialog/provider expectations;
- standard action and icon treatment;
- shared accessibility behaviour;
- reusable responsive behaviour where explicitly governed;
- the common visual assumptions used by OneVision components and patterns.

MudBlazor remains an implementation detail. Applications should not depend on its defaults as their UI contract.

## Contract identity

The library exposes `OneVision.UI.Foundation.OneVisionApplicationFoundation` so consuming applications and contract tests can identify the adopted baseline:

```csharp
OneVisionApplicationFoundation.Reference       // OVP-000
OneVisionApplicationFoundation.ContractVersion // 1.0
```

Applications should be able to surface or test this value so that foundation adoption is explicit rather than inferred from appearance.

## Versioning rule

`OVP-000` is a living, versioned contract.

A normal OneVision.UI implementation change does **not** automatically change the contract version. The contract version advances when an externally observable foundation rule changes, including changes to required root integration, governed global appearance, shared state behaviour, required providers/assets or other baseline expectations that consuming applications must account for.

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

`legacy app → OVP-000 foundation adopted → legacy exceptions identified → OVC/OVP adoption → genuine library gaps resolved → exceptions removed → fully governed app`

A screen does not need to be modelled in the UI library before it can be implemented. App-specific screens remain application composition built from the governed foundation and existing reusable contracts.
