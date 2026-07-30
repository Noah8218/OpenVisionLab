# OpenVisionLab Property Load Recovery Feedback

Date: 2026-07-30 KST<br>
Priority: P270<br>
Status: Complete

## Outcome

The P269 follow-up audit reproduced a second persistence-trust defect.

Direct PropertyGrid Tool configuration loading previously collapsed several
different states into the same default-valued PropertyGrid:

- no configuration file;
- a valid configuration;
- an invalid or deserialization-incompatible configuration that had just been
  moved to an `.invalid-<timestamp>.xml` backup;
- an unreadable configuration or another load exception.

The XML loader already backed up invalid files and created a new default file,
but its result was discarded. Load exceptions were also caught and returned as
defaults. The Tool therefore looked like a normal restored setup.

P270 preserves the loader's existing recovery behavior and carries an explicit
load result through both Direct Tool load paths:

- session-store loading, including `AffineTransform`;
- Recipe repository preloading used by Blob, Contour, Line, Matching,
  EdgeBasedMatching, and FeatureMatching.

## Load-State Contract

### No saved configuration

The first use remains normal:

- the default configuration file is created;
- the Tool opens with visible editable defaults;
- no warning is shown.

### Valid saved configuration

The configuration restores normally and no warning is shown.

### Invalid or deserialization-incompatible configuration

The existing file is preserved as
`<name>.invalid-<timestamp>.xml`, a default configuration is created, and the
Tool status says:

- saved settings were invalid or incompatible;
- the Tool opened with default values;
- prior teaching must not be assumed restored;
- values must be reviewed;
- the exact backup path and cause.

### Unreadable configuration or load exception

The Tool opens with in-memory defaults and the status says:

- saved settings could not be loaded;
- prior teaching must not be assumed restored;
- values must be reviewed;
- the saved file was not changed;
- the failure cause.

### Explicit save recovery

The load warning is retained by Tool/Recipe key. A later successful explicit
property save clears it and reuses P269's one-time recovery result. A failed
save keeps both the load warning and memory-only save warning state.

Recipe context changes clear tracked failures so one Recipe cannot leak status
into another.

## UI Contract

The warning uses the existing bottom Tool status rather than a modal dialog or
an overlay.

- PropertyGrid teaching remains available.
- The visible line is ellipsized when needed.
- The complete text is available through Tooltip and accessibility HelpText.
- Korean and English are supported.
- Merely loading or showing the warning does not Preview/Run, create/delete
  layers, change the active layer, or mutate routing.

## Actual EXE Evidence

The same current Debug EXE and `920 x 660` Blob Tool layout were used.

Before:

- a forced load exception silently opened defaults;
- the status area remained absent;
- the operator could not distinguish restored settings from fallback values.

After:

- the same failure is visible in the existing bottom status;
- it identifies Tool/Recipe, default substitution, review requirement, file
  mutation boundary, and cause;
- the status does not cover PropertyGrid, image viewers, Pipeline actions, or
  explicit Preview.

Evidence:

- `artifacts\p270_property_load_feedback_20260730\actual_exe_before`
- `artifacts\p270_property_load_feedback_20260730\actual_exe_after`

Both actual-EXE runs retained:

- `PreviewRunCount: 0`
- `LayerCount: 0`
- `Main -> Blob_Preview`

## Verification

- Missing file -> `CreatedDefaultForMissingFile`: passed
- Second load of that file -> `Loaded`: passed
- Malformed file -> `ReplacedInvalidFile`: passed
- Invalid original retained at exact backup path: passed
- Recipe-repository preloaded invalid result retained by Direct Tool session
  store: passed
- Successful save cleared the retained load failure and reported recovery:
  passed
- Forced unreadable/load-exception status: passed
- Korean/English localization: passed
- Full message Tooltip/accessibility HelpText: passed
- P269 save failure/recovery regression: passed
- P268 EdgeBasedMatching Parameter Guide regression: passed
- Blob Tool explicit Preview regression: passed
- Localization catalog contract: passed
- Actual current Debug EXE after capture: passed
- Full standard Debug build: zero warnings, zero errors
- Readiness contract: passed
- Localization duplicate keys: zero
- `git diff --check`: no whitespace errors; existing line-ending notices only

Evidence:

`artifacts\p270_property_load_feedback_20260730`

## Reassessment Decision

P270 does not add an algorithm, dataset, automatic tuning, navigation rewrite,
or new help surface.

The next persistence boundary is
`OpenVisionNativeToolSettingsStore`. Threshold, Filter, Morphology,
Arithmetic, and SimplePreprocess families use that separate store, whose save
and load results remain undifferentiated. Audit it before implementing parity;
do not assume the P269/P270 PropertyGrid contract already covers those Tools.

Recommended model: `gpt-5.6-terra` | Reasoning effort: `medium`.

CVR-00 remains deferred until three independent first-time participants and
their raw observations exist.

## Known Boundary

P270 detects XML read failures and structural/deserialization incompatibility.
It cannot label a syntactically valid older file as semantically stale when
the current serializer still accepts every value. That requires an explicit
future schema/version or validation contract; no version was invented in this
bounded correction.

## Completion Record

Status: Complete<br>
Scope: Direct PropertyGrid Tool saved-setting load result preservation,
invalid-file backup/default-substitution feedback, unreadable-load feedback,
and successful-save recovery.<br>
Acceptance criteria: missing configuration remains warning-free; valid
configuration restores; invalid configuration is backed up and identified;
read exceptions state that the saved file was not changed; repository-preload
and session-load paths retain the result; full Korean/English message is
non-obstructing and accessible; successful save clears the warning; zero
Preview/Run/layer/route side effects.<br>
Verification: Current Debug EXE before/after, missing/valid/invalid file
contract, repository-preload backup and save recovery, P269, P268, Blob Tool,
localization, full Debug build, readiness, duplicate-key, and diff checks.<br>
Evidence:
`artifacts\p270_property_load_feedback_20260730` and this report.<br>
Boundary / next dependency: Syntactically valid semantic staleness remains
undetectable without a schema/version rule. The separate
`OpenVisionNativeToolSettingsStore` persistence path is the next bounded
audit.
