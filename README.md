# OneVision.UI

Shared UI design system and component library for OneVision applications.

## Purpose

`OneVision.UI` provides the common visual language, interaction patterns, and reusable components used by OneVision applications. MudBlazor supplies the underlying Blazor component framework; OneVision.UI owns the public component contracts, styling, design tokens, and higher-level application patterns.

Applications should consume `OneVision.UI` rather than use MudBlazor components directly unless a documented exception is required.

## Application foundation

Every OneVision application starts by adopting `OVP-000 – OneVision Application Foundation` at its composition root. This is the versioned baseline contract for shared typography, tokens, density, global states, required assets/providers and other governed foundation behaviour before screen-by-screen `OVC-*`/`OVP-*` migration.

The authoritative living contract is `docs/FOUNDATION.md`. Its machine-readable identity is exposed by `OneVision.UI.Foundation.OneVisionApplicationFoundation`, allowing consuming applications and contract tests to prove which foundation version they implement.

A OneVision.UI change that affects the foundation must include a contract-impact assessment. The `OVP-000` contract version advances only when externally observable foundation rules change; implementation, contract documentation and gallery representation are updated together.

## Projects

- `src/OneVision.UI` — Razor Class Library containing OneVision components, theme, tokens, foundation contract and static assets.
- `demo/OneVision.UI.Demo` — interactive component gallery used to design and review OneVision controls and the current foundation contract.
- `docs` — design-system guidance and component/pattern catalogue.

## Public component gallery

The compiled Blazor WebAssembly gallery is published to the public `kingcheesezombie/OneVision.UI.Demo` repository. The private source repository remains authoritative; the public repository contains compiled gallery output only.

The gallery exposes only canonical first-class component and pattern contracts in its global search. Internal pattern regions remain part of their owning pattern and are not independent library items.

## Technology baseline

- .NET 9
- Blazor
- MudBlazor 9.11.0

## Build

```powershell
dotnet restore
dotnet build OneVision.UI.sln -c Release
dotnet run --project demo/OneVision.UI.Demo
```

## Package

The library is configured to produce the `OneVision.UI` NuGet package. Consuming applications should reference a released package version rather than a source-project reference in production.

## Status

The library now has an explicit canonical OVC component catalogue plus OVP foundation/page/pattern contracts. Retired aliases, duplicate gallery routes and obsolete compatibility wrappers are not retained unless a deliberate compatibility requirement is agreed.
