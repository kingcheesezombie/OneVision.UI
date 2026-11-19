# OVP-CALCULATION-BUILDER — Calculation and independent score formatting

**Contract version:** 1.0 · **OneVision.UI release:** 0.1.57 · **OVP-000:** unchanged.

## Governed behaviour
One shared `OVCalculationDefinition` holds stable, ordered typed tokens. `OVCalculationTier` is a **semantic, application-owned editing permission**, not a visual setting:

| Tier | Available editing controls |
|---|---|
| Simple (1) | Core numeric operands, literals, arithmetic + − × ÷, parentheses and token reorder |
| Extended (2) | Tier 1 plus application-supplied external operand categories (such as referenced scores, geography or other measures) |
| Advanced (3) | Tier 2 plus application-approved aggregation/function tokens such as SUM, AVG, MIN, MAX and COUNT |

A lower-tier view of a higher-tier expression **preserves and displays the original definition read-only**. It must not strip advanced tokens, downgrade semantics or allow clearing them. Tier 3 functions are not independently enabled in Tier 2.

`OVCalculationFormat` and `OVCalculationFormatOption` form a separate format-selection contract. Score format applies **after calculation** and does not alter expression tokens or numeric results. Applications supply supported output masks, formatting policies and preview values. Formatting is available at all tiers and can be omitted from the composed builder where a separate settings surface is used.

`OVCalculationSyntax.Validate` performs structural UI validation only. It cannot validate function semantics, source eligibility, division by zero, KPI dependency cycles, data types, aggregation context or production-time rules. Those checks and **all runtime evaluation and final formatting belong to the consuming application**. `OVCalculationPreview` is a read-only view of the application's preview result and loading/error state.

## Typed integration
The app provides `Definition`/`DefinitionChanged`, `Tier`, core `OVCalculationOperand` records, optional `ExternalCategories` and `Functions`, and independently `Format`/`FormatChanged`, `FormatOptions` and `Preview`. Persist stable operand keys and token IDs; do not persist labels. The application decides whether and how a source resolves at runtime.

Token manipulation supports click-to-add, pointer drag/drop, keyboard Alt+Left/Right reorder and removal. Busy/read-only states block edits. Unknown/unavailable operands remain represented by their original tokens, flagged by validation, rather than silently removed.

## Demo and ownership
The public gallery demonstrates each tier, an advanced expression viewed at a lower tier, independent format choices and a synthetic application-supplied preview. It contains no operational KPI data or credentials.

OneVision.UI owns the controls, editing interaction and structural diagnostics. OneVision.KPI (or other consumers) own source catalogues, aggregation resolution, actual expression execution, formatting semantics, validation, storage and permission enforcement. This release does not modify OneVision.KPI.
