# OpenVisionLab Line Inactive And Legacy Controls

Date: 2026-07-30 KST<br>
Priority: P266<br>
Status: Complete

## Outcome

The seven Line properties identified by P265 remain visible for Recipe/XML
compatibility, but no longer look like active teaching controls:

1. Current runtime does not consume:
   `USE_AVERAGE_FILTER`, `AVERAGE_Diff`, `AVERAGE_FILTER_TYPE`
2. Legacy bitmap Draw path only:
   `SHOW_VERTICAL_LINE`, `SHOW_EDGE`, `SHOW_CONTOUR`, `SHOW_FITLINE`

Direct Line Tool and Recipe Manager selected-Step PropertyGrids now place these
values in an explicit compatibility/legacy category. Their editors are
disabled, the rows remain selectable for contextual guidance, and Korean/
English labels state either `호환(미적용)` or `레거시 표시`.

The properties, setters, XML keys, Recipe values, and pair-specific Line A/B
baselines remain intact. No average-filter algorithm was added and mandatory
current WPF Preview/Pipeline Review evidence was not connected to the legacy
drawing flags.

## Compatibility Contract

The shared PropertyGrid bridge now recognizes
`PropertyGridCompatibilityReadOnlyAttribute`.

- Marked rows stay visible and selectable.
- Editor controls are disabled and excluded from tab navigation.
- PropertyGrid mutation APIs reject edits to marked values.
- Contextual parameter guidance still opens for the selected row.

The ordinary .NET `ReadOnlyAttribute` was not used because the current
third-party WPF PropertyGrid hides such rows. Hiding the values would prevent
operators from reviewing saved legacy state and would weaken Recipe
compatibility.

Basic, Fast, and Precise Line presets no longer mutate the seven compatibility
values. Precise also no longer claims to enable average filtering. Existing
non-default Line A/B values survived no-edit apply, save, and reload exactly.

## Actual EXE Evidence

The current Debug EXE used the same `920 x 860` Line Tool before and after.

- Before: `평균 필터 사용` appeared as a normal editable checked field.
- After: the same saved value remains visible under
  `호환(미적용) · 평균 필터`, with a disabled editor and detailed explanation.
- The legacy scan-line row is also visibly disabled and labelled as legacy.
- `ObstructedControls: None`
- `AutomaticShowFocusRetained: True`
- `ExplicitHideReopen: PASS`
- `PreviewRunCount: 0`
- `LayerCount: 0`

Evidence:

- `artifacts\p266_line_inactive_legacy_controls_20260730\actual_exe_before`
- `artifacts\p266_line_inactive_legacy_controls_20260730\actual_exe_after`

The guide remains beside the Tool and does not cover the PropertyGrid, image
viewers, layer selectors, Pipeline actions, or explicit Preview button.

## Verification

- P266 focused smoke: `ReadOnlyProperties=7/7`
- Direct Tool Korean/English compatibility labels: passed
- Direct PropertyGrid bridge mutation rejection: passed
- Basic/Fast/Precise preservation of all seven values: passed
- Direct Line single-property mapping round trip: passed
- Recipe Manager asymmetric Line A/B no-edit apply/save/reload: passed
- P257 shared contextual guide regression: passed
- P265 Line parameter guide regression: passed
- Line Tool, preset, and LineDistance measurement regressions: passed
- Line Signal current-build regression: first combined-process capture hit the
  known overlay-discovery flake; an immediate isolated retry from the same
  build passed
- Full Debug solution build: 0 warnings, 0 errors
- Readiness contract: all checks passed
- Standalone canonical audit remains:
  - Browsable: `318`
  - Detailed: `266`
  - Basic fallback: `52`

Evidence:

`artifacts\p266_line_inactive_legacy_controls_20260730`

## Next Bounded Priority

The remaining 52 Basic entries are EdgeBasedMatching 32 and AffineTransform
20. Before implementing more guide text, audit the 20 AffineTransform entries
against the existing runtime and group them into source/destination
coordinates, output image policy, interpolation/border policy, and fail-closed
geometry/coverage gates. Admit implementation only for semantics supported by
current code and tests.

Recommended model: `gpt-5.6-sol` | Reasoning effort: `medium`.

CVR-00 remains incomplete and deferred until three independent first-time
participants and their raw observations exist.

## Completion Record

Status: Complete<br>
Scope: Non-editable but visible compatibility treatment for the seven inactive
or legacy-only Line properties in Direct Tool and Recipe Manager, plus preset
preservation.<br>
Acceptance criteria: seven rows visibly and accurately labelled; editor and
bridge mutation blocked; existing XML/Recipe/Line A/B values preserved;
Basic/Fast/Precise do not mutate them; contextual guidance remains available;
no Preview/Run/layer/route side effects; actual EXE layout remains
non-obstructing.<br>
Verification: Focused/shared guide smokes, Direct/Recipe PropertyGrid and
preset/measurement regressions, actual Debug EXE before/after, standalone
audit, full Debug solution build, and readiness.<br>
Evidence: `artifacts\p266_line_inactive_legacy_controls_20260730` and this
report.<br>
Boundary / next dependency: This does not implement average filtering, make
legacy drawing flags control current WPF/Pipeline evidence, increase detailed
guide coverage, qualify Line metrology, prove field robustness, integrate
equipment, or complete CVR-00.
