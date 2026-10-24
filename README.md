# OneVision.UI

Shared UI design system and component library for OneVision applications.

## Purpose

`OneVision.UI` provides the common visual language, interaction patterns, and reusable components used by OneVision applications. MudBlazor supplies the underlying Blazor component framework; OneVision.UI owns the public component contracts, styling, design tokens, and higher-level application patterns.

Applications should consume `OneVision.UI` rather than use MudBlazor components directly unless a documented exception is required.

## Projects

- `src/OneVision.UI` — Razor Class Library containing OneVision components, theme, tokens, and static assets.
- `demo/OneVision.UI.Demo` — interactive component gallery used to design and review OneVision controls.
- `docs` — design-system guidance and component/pattern catalogue.

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

Initial design-system scaffold. The first controls are intentionally small while the OneVision visual language is established in the demo application.
