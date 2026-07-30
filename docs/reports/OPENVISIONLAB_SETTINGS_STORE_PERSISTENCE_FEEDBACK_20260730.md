# OpenVisionLab Settings Store Persistence Feedback

Date: 2026-07-30 KST<br>
Priority: P271<br>
Status: Complete

## Outcome

The P270 follow-up audit reproduced the same persistence-trust defect in the
separate `OpenVisionNativeToolSettingsStore`.

The affected Direct Tool views were Threshold, Filter, Morphology, Arithmetic,
EdgeDetection, RotateScale, Mean, HSV, and Histogram. Their saved settings
previously:

- silently opened defaults after a load exception;
- discarded the XML loader's invalid-file replacement result;
- swallowed a disk-save failure;
- published an undifferentiated saved event that looked successful.

P271 brings this store to the same operator-visible contract as P269/P270
without changing algorithm behavior or adding another dialog.

## Persisted-Settings Contract

### Missing and valid configuration

- A first-use missing file is created from visible editable defaults.
- A valid file restores normally.
- Neither state shows a warning.

### Invalid or incompatible configuration

- The original is retained as
  `<name>.invalid-<timestamp>.xml`.
- A default configuration is created.
- The active Tool status identifies default substitution, tells the operator
  not to assume that prior teaching was restored, and includes the backup path
  and cause.

### Unreadable configuration or load exception

- The Tool opens with in-memory defaults.
- The status names Tool and Recipe, review requirement, cause, and the fact
  that the saved file was not changed.

### Save failure and recovery

- A failed save keeps the current teaching values in memory.
- The status explicitly warns that they may be lost after reopening.
- The next successful save for the same Tool/Recipe reports recovery once and
  clears both retained load and save failures.
- Later ordinary successful saves do not repeatedly replace useful Tool
  status.

Recipe changes and shell initialization clear tracked transient failure state,
so one Recipe cannot leak a warning into another.

## User Workflow And UI

The existing bottom Tool status is reused.

- No modal dialog, overlay, extra settings screen, or new teaching step was
  added.
- The one-line status is ellipsized when needed.
- The complete text is available through Tooltip and accessibility HelpText.
- Korean and English messages passed.
- Loading, warning display, saving, and recovery do not Preview/Run,
  create/delete/select layers, or mutate Pipeline routing.

The Tool families' initialization paths were checked as part of the contract:
Threshold, Filter, and Morphology suppress automatic save while applying
loaded settings; Arithmetic and SimplePreprocess attach their settings-save
event after initial application or suppress the application refresh. The
retained load warning is therefore not accidentally cleared by initialization.

## Actual EXE Evidence

The same current Debug EXE and `920 x 660` Threshold Tool layout were used.

Before:

- forced settings load and save failures left the status empty;
- defaults and memory-only values looked normally restored/saved.

After:

- load failure is visible in the existing bottom status;
- save failure explicitly identifies memory-only state and reopen-loss risk;
- the first successful save reports disk persistence recovery;
- full text remains accessible while no teaching control, image viewer,
  Pipeline action, or explicit Preview button is covered.

All current-EXE runs retained:

- `PreviewRunCount: 0`
- `LayerCount: 0`
- `Main -> Threshold_Preview`

Evidence:

- `artifacts\p271_settings_persistence_feedback_20260730\actual_exe_before`
- `artifacts\p271_settings_persistence_feedback_20260730\actual_exe_after`

## Verification

- Missing configuration -> normal default creation without warning: passed
- Second valid load -> normal restore without warning: passed
- Malformed configuration -> exact invalid-file backup and default
  replacement: passed
- Load failure retained until explicit successful save: passed
- First successful save reports recovery and clears the failure: passed
- Second ordinary successful save does not repeat recovery: passed
- Forced disk-save failure reports memory-only/reopen-loss state: passed
- Korean/English localization: passed
- Full Tooltip/accessibility HelpText: passed
- Threshold actual current Debug EXE before/after: passed
- P269 property save failure/recovery actual-EXE regression: passed
- P270 property load/backup/recovery actual-EXE regression: passed
- Threshold, Filter/Morphology, Arithmetic, EdgeDetection, RotateScale, Mean,
  HSV, and Histogram focused current-source UI regressions: passed
- Parameter Guide and localization regressions: passed
- Readiness contract: passed
- `git diff --check`: no whitespace errors; existing line-ending notices only
- Full standard Debug build: zero warnings, zero errors

Evidence:

`artifacts\p271_settings_persistence_feedback_20260730`

## Reassessment Decision

P271 closes both saved-setting stores' currently reproduced load/save feedback
gaps. Do not add schema/version migration or another settings UI without a
reproducible stale-but-deserializable file case.

The next bounded priority is a static persistence audit of the higher-impact
Recipe/Pipeline boundaries, beginning with `VisionPipelineStorage` and
`RecipeDataStorage`, because their current `LoadOrCreateXmlFile` calls may also
collapse invalid saved state into a default. Audit first; implement only when
an operator-visible silent fallback is reproduced.
The admitted P272 scope, R1-R10 reproduction matrix, A1-A12 acceptance
criteria, implementation gate, and ordered later priorities are recorded in
`docs\reports\OPENVISIONLAB_NEXT_DEVELOPMENT_DECISION_20260730.md`.

Recommended model: `gpt-5.6-terra` | Reasoning effort: `medium`.

CVR-00 remains deferred until three independent first-time participants and
their raw observations exist.

## Known Boundary

P271 detects missing files, XML/deserialization incompatibility, read
exceptions, and disk-save exceptions. It does not identify syntactically valid
but semantically stale settings when the serializer still accepts every value.
That requires an explicit schema/version or semantic-validation contract.

## Completion Record

Status: Complete<br>
Scope: `OpenVisionNativeToolSettingsStore` missing/valid/invalid/load-exception
state preservation, invalid-file backup, save-failure feedback, and one-time
explicit-save recovery for the named Direct Tool families.<br>
Acceptance criteria: normal first use and valid restore remain warning-free;
invalid files are backed up and identified; unreadable load and failed save
state are explicit; full Korean/English status is non-obstructing and
accessible; successful save clears failure once; zero
Preview/Run/layer/route side effects.<br>
Verification: current Debug EXE before/after, missing/valid/invalid/backup/
recovery file contract, P269/P270 actual-EXE regressions, focused Tool-family
and localization smokes, readiness, standard Debug build, and diff check.<br>
Evidence:
`artifacts\p271_settings_persistence_feedback_20260730` and this report.<br>
Boundary / next dependency: syntactically valid semantic staleness remains
undetectable without a schema/version rule. Recipe/Pipeline persistence is a
separate higher-impact audit, not proven defective or corrected by P271.
