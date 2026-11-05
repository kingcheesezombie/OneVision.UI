# OVC-DATE-PICKER and OVC-NUMBER-FIELD — bounded input quality

## OVC-DATE-PICKER

`OVDateValidation.Evaluate` compares calendar dates inclusively, without timezone conversion or artificial date endpoints.

- `MinDate` and `MaxDate` are optional; both are passed to the native picker and independently applied to programmatically supplied values.
- If the configured minimum is after the maximum, a reusable validation error is reported, never a silently swapped limit.
- Empty selection is allowed unless `Required=true`; an unset required date is an error.
- An app-supplied `Error`/`ErrorText` takes precedence when present.
- The consumer keeps ownership of `Value` and `ValueChanged`; optional `ValidationChanged(OVDateValidationResult)` reports the attempted value's reusable validity without enforcing business-specific save policy.
- Disabled and read-only stop change notifications while still showing the existing value.

## OVC-NUMBER-FIELD

- New optional typed `Min` and `Max` parameters propagate inclusive bounds to the existing `MudNumericField<T>` wrapper.
- Numeric input remains strongly typed; no hidden string conversion, default rounding, units, calculation, domain-specific field validation or backend contract is introduced.
- `Value`, `ValueChanged`, `Required`, `ReadOnly`, `Disabled` and external errors remain as before.
- Applications own any additional constraints, such as non-negative financial amounts, available stock, clinical reference ranges or business rule exceptions.

## Gallery and regression

The live workbench now exercises date required/empty/out-of-bounds states, bounded numeric states, read-only inputs, and independent limit/validation switches.

Unit tests cover inclusive date-only comparisons, optional and invalid constraints, wrapper errors, change-callback semantics and strongly typed numeric bounds. Browser tests exercise actual date and number inputs and state/capability combinations.

This change is a compatible OVC-quality improvement and **does not change OVP-000**. The library release/build/browser checks support L0/L1/L5 review, but application L3 adoption, 200%/400% zoom and stress checks remain separate.
