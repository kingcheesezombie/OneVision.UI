# OVC-DATE-RANGE and OVC-FILE-UPLOAD — input quality acceptance

## OVC-DATE-RANGE

A date-only inclusive range is validated through `OVDateRangeValidation.Evaluate`. No time-zone conversion, fabricated end date, or application-owned business rule is applied.

- A range with both dates is invalid when End is before Start, even if no optional constraints were configured.
- `MinDate` and `MaxDate` are inclusive bounds, passed to the underlying calendar, with the same limits independently checked for supplied values. An invalid minimum/maximum configuration is reported rather than silently swapped.
- A fully empty range is allowed unless `Required=true`.
- A partial/open-ended range is allowed by default; `RequireComplete=true` requires both dates once any date is present.
- `Required=true` requires both dates even for a completely empty range.
- External `Error`/`ErrorText` still take precedence when explicitly supplied.
- `ValueChanged` always sends the attempted value for application-controlled state, while optional `ValidationChanged` reports the reusable result. The library displays semantic invalid-state feedback; the consuming application owns whether Save is permitted.
- Disabled/read-only prevent change callbacks; they retain selected dates for inspection.

Visual and keyboard tests use the actual `MudDateRangePicker` wrapper; the workbench shows open-ended, required, incomplete, reversed and bounded intervals, with independently combined options.

## OVC-FILE-UPLOAD

- Existing `RemoveRequested(string filename)` remains source-compatible.
- Preferred `RemoveFileRequested(IBrowserFile)` identifies the selected file object and disambiguates duplicate filenames. It takes precedence if both callbacks exist.
- Duplicate filenames receive individually accessible, position-qualified remove action labels; ordinary unique filenames keep their existing label.
- `ReadOnly=true` retains the chosen file list, validation feedback and progress, while preventing selection/removal. `Disabled` retains its existing interaction guard.
- Applications still own file validation, accepted extensions/types, streaming, persistence and duplicate-file policy. The library must not silently deduplicate legitimate files by filename.
- Maximum file-count feedback and partial rejection continue to follow the previous callback contract. No new upload backend, file caching or preview is introduced.

The gallery uses real `InputFile`, exact-object removal and independently supported read-only, disabled, multi-file and upload-progress switches.

## Regression gate

1. Unit tests: date-only inclusive bounds, reversed dates, absent/partial dates, callback compatibility, duplicate-file identity and read-only.
2. Browser tests: invalid/required/open-ended range state transitions, actual duplicate-file selection/removal, read-only suppression and accessibility.
3. Whole-solution build, unit and browser suites, then source-to-compiled-gallery and package publish verification.

## Ownership and foundation

All capabilities extend existing components and default to the previous permissive behaviour except validation of an intrinsically reversed interval. No OVP-000 foundation contract change is required. Real application adoption and domain-specific date/file policies remain L3 verification tasks.
