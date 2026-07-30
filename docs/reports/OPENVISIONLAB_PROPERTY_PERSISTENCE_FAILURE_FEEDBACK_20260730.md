# OpenVisionLab Property Persistence Failure Feedback

Date: 2026-07-30 KST<br>
Priority: P269<br>
Status: Complete

## Outcome

The post-Parameter-Guide user-flow reassessment found one concrete data-loss
risk and selected only that bounded correction.

Direct PropertyGrid Tools persist their current settings after edits, presets,
template changes, and Pipeline creation. The previous persistence path kept
the in-memory value when disk save failed, swallowed every exception, and
still raised an undifferentiated `PropertySaved` event. The Tool therefore
looked normal even though the value could disappear after reopening.

The current behavior now:

- retains the in-memory teaching value;
- does not trigger Preview/Run or change layers/routing;
- publishes an explicit failed persistence result;
- shows the active Tool and Recipe scope;
- explains that the value is memory-only and can be lost after reopening;
- includes the failure cause;
- publishes one recovery result when the next save succeeds;
- does not replace the Tool status on every ordinary successful save.

## User-Centered Contract

### Failure

The active Tool status states:

> Settings could not be saved for Tool / Recipe. The current values remain in
> memory but may be lost after reopening.

The Korean and English messages name the exact Tool and Recipe. The visible
one-line status uses ellipsis when necessary, while the complete message is
available through the status Tooltip and accessibility HelpText.

The failure does not undo the current edit. This keeps the operator's current
teaching work available for correction or retry without pretending it is
durably saved.

### Recovery

The persistence store remembers failed Tool/Recipe keys. The next successful
save for the same key emits a one-time recovery result and the active Tool
states that the current values are now persisted. Later ordinary saves do not
repeatedly replace the Tool's more useful inspection status.

Changing Recipe context clears the tracked failure state so one Recipe does
not leak save status into another scope.

## Actual EXE Evidence

The same current Debug EXE and `920 x 660` Blob Tool were used.

Before:

- a forced disk-save failure produced an empty status;
- the operator could not distinguish durable state from memory-only state.

After:

- the same forced failure is visible in the existing bottom Tool status;
- no dialog or overlay covers PropertyGrid teaching, images, Pipeline actions,
  or explicit Preview;
- the full message remains accessible by Tooltip and accessibility HelpText;
- the next successful save reports recovery exactly once.

Evidence:

- `artifacts\p269_property_persistence_feedback_20260730\actual_exe_before`
- `artifacts\p269_property_persistence_feedback_20260730\actual_exe_after`

Both actual-EXE runs retained:

- `PreviewRunCount: 0`
- `LayerCount: 0`
- unchanged input/output routes

## Verification

- P269 focused Korean/English failure/recovery smoke: passed
- Failed result event:
  - `Succeeded=false`
  - `RecoveredFromFailure=false`
- Next success:
  - `Succeeded=true`
  - `RecoveredFromFailure=true`
- Later ordinary success:
  - no repeated Tool-status replacement
- Full Tooltip and accessibility HelpText: passed
- P254 Direct teaching -> Pipeline persistence regression: passed
- P257 contextual guide regression: passed
- P268 EdgeBasedMatching guide regression: passed in isolated current-build
  run; one combined multi-target process inherited conditional visibility
  state and failed, while immediate isolated replay passed
- Blob Tool explicit Preview regression: passed
- Localization catalog contract: passed
- Actual Debug EXE before/after: passed
- Full Debug build: zero warnings, zero errors
- Readiness contract: passed

Evidence:

`artifacts\p269_property_persistence_feedback_20260730`

## Reassessment Decision

No algorithm, dataset, navigation rewrite, automatic tuning, or new help
surface is admitted from P269. The connected core workflow remains:

`image -> PropertyGrid teaching -> explicit Preview -> add/save Pipeline ->
Run Review/N-image evidence -> saved Recipe`.

The one selected defect was persistence trust, not inspection capability.

## Next Bounded Priority

Audit the paired load path before implementation. Current source still catches
saved-setting load exceptions and silently returns defaults. Determine whether
the operator can distinguish:

- no saved configuration;
- valid saved configuration;
- stale or incompatible saved configuration;
- unreadable/corrupt configuration.

Implement only if a reproducible current-source path silently substitutes
defaults for a previously saved Tool/Recipe configuration.

Recommended model: `gpt-5.6-terra` | Reasoning effort: `medium`.

CVR-00 remains deferred until three independent first-time participants and
their raw observations exist.

## Completion Record

Status: Complete<br>
Scope: Static post-guide workflow reassessment and one bounded Direct Tool
property-save failure/recovery feedback correction.<br>
Acceptance criteria: failed disk save preserves current memory state but
publishes failure; active Tool/Recipe and reopen-loss risk are visible;
complete message is tooltip/accessibility-readable; next success reports
recovery once; ordinary saves add no status noise; Korean/English work; zero
Preview/Run/layer/route side effects.<br>
Verification: Focused failure/recovery event and UI smoke, actual current Debug
EXE before/after, Direct teaching persistence, contextual guide,
EdgeBasedMatching guide isolated regression, Blob Tool, localization, full
Debug build, readiness, and diff checks.<br>
Evidence:
`artifacts\p269_property_persistence_feedback_20260730` and this report.<br>
Boundary / next dependency: This corrects property-save trust only. It does not
yet distinguish missing, stale, corrupt, incompatible, or unreadable saved
configuration during load; that paired load-path audit is the next dependency.
