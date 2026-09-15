# OpenVisionLab OVL-07 Pipeline Review stale callback atomic application boundary — 2026-09-08

## Scope

This slice closes the narrow race in which a Pipeline Review UI callback had
already passed `IsCurrentRun` while Reset or Close could clear the Review
summary/cache state before the callback finished applying its observable
updates. The existing execution controller remains the state owner; the
callback validation, summary/cache mutation, and `StepUpdated` notification now
share one `executionSync` boundary. Completion application uses the same
boundary before `CompleteRun` publishes final summaries and output images.

The change preserves the existing execution-generation and cancellation model,
Recipe/XML exchange, Layer routing, explicit Preview/Run behavior, and the
preceding image snapshot/cache ownership contracts. No new generation service,
message bus, interface, or UI owner was introduced.

## Responsibility and call-path change

| Concern | Before | Current |
| --- | --- | --- |
| Step callback validation/application | `IsCurrentRun` and summary/cache/event updates could be observed as separate operations | `OnStepExecutionUpdated` validates the stamp and applies summary, cache, and `StepUpdated` under `executionSync` |
| Final result application | Completion callback entered `CompleteRun` after an independent current-run check | Completion callback checks the stamp and calls `CompleteRun` under `executionSync` |
| Reset/Close interaction | Reset/Close could race an already-entered callback between validation and mutation | Reset/Close wait for an entered callback boundary; callbacks entering after invalidation fail closed |
| Focused proof | Existing queued stale-callback coverage did not hold Reset against an entered callback | New contract holds `StepUpdated`, starts Reset concurrently, then verifies Reset waits and final summary/cache state is empty |

The resulting path is:

`Execution worker -> invokeOnUi -> executionSync -> stamp validation -> summary/cache update -> StepUpdated`

`Execution worker -> invokeOnUi -> executionSync -> stamp validation -> CompleteRun`

## Changed files

- `src/OpenVisionLab/UI/Menu/Wpf/PipelineReview/Execution/OpenVisionPipelineReviewExecutionController.cs`
  - Serializes current-run validation with step summary/cache updates and the
    step notification; serializes final result application with the same lock.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - Adds `--pipeline-review-stale-callback-contract` and deterministic
    entered-callback/Reset and entered-callback/Close race probes. The probes
    start the controller on a worker because the direct test dispatcher
    executes callbacks synchronously.
- `docs/reports/OPENVISIONLAB_OVL07_PIPELINE_REVIEW_STALE_CALLBACK_20260908.md`
  - Records the bounded refactor, proof, and verification evidence.
- `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`
  - Records completion and the next single priority.
- `docs/LLM_DOCUMENT_INDEX.json`
  - Routes Pipeline/ownership work to this report.

## Preserved contracts

- A current Review step still exposes the same localized summary and output
  layer through the existing Document and View path.
- Reset and Close still invalidate the active generation, cancel the active
  run, and retire Review summaries/output cache through the existing controller
  lifecycle.
- A callback that enters after Reset/Close invalidation still fails closed and
  cannot repopulate summary or cache state.
- Recipe/XML serialization, Layer names/routing, explicit Preview/Run, and the
  existing image/Layer snapshot and cache-owner contracts remain unchanged.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-stale-callback-20260908`

- OpenVisionLab app Debug build passed with 0 warnings and 0 errors.
- OpenVisionLab solution Release build passed with 0 warnings and 0 errors.
- VisionRecipeRunnerSmoke Debug and Release builds passed sequentially with 0
  warnings and 0 errors.
- `--pipeline-review-stale-callback-contract` passed in Debug and Release.
  Both reports record that Reset and Close waited for the entered callback
  boundary and that final summary/cache state is empty; final reports are at
  `contract-debug-final4/pipeline-review-stale-callback-contract.txt` and
  `contract-release-final4/pipeline-review-stale-callback-contract.txt` under
  the evidence root.
- Existing `--pipeline-review-execution-contract`,
  `--pipeline-review-cache-lifetime-contract`, and
  `--pipeline-review-layer-image-owner-contract` passed in Release after the
  controller change.
- `OpenVisionReadinessCheck` Release passed all 13/13 checks.
- `PipelineViewerScreenshotSmoke` x64 Release build completed with 0 warnings
  and 0 errors.
- Release `wpf_shell_host_pipeline_review` and
  `wpf_shell_host_pipeline_review_ng` UI smoke targets passed with
  `check=OK`, `layout=0|text=0|internal=0`, and `1600x900`. Fresh normal and
  acceptance-NG captures under `ui-release-final` were visually inspected;
  OpenGL sidecars report one
  shell host, one layer viewer, one tile, and `DockHeadersReady=True`.
- Dynamic monitor evidence recorded one `\\.\DISPLAY2`, bounds
  `0,0,1920x1080`, working area `0,0,1920x1032`; no smoke target process
  remained after capture.
- Static ownership proof passed at
  `static-proof.txt`, covering callback/completion/reset lock order, snapshot
  provider wiring, focused contract dispatch, and removal of the raw cache
  resolver.
- Documentation-index validation passed with `IndexedPaths=183`, `Routes=13`,
  and `RootRedirects=102`. Final scoped `git diff --check` and untracked-target
  whitespace checks passed.

The first pre-fix contract harness invocation was intentionally stopped after
it exposed that a synchronous callback blocked the test before the caller could
start Reset. The harness now starts the same controller call on a worker; this
was a test-harness correction, not a production behavior change. No source
reset, clean, checkout, commit, push, merge, release, or deployment was
performed.

The full theme/layout/DPI (100/125/150/175/200%), multi-monitor, hover/pressed/
focus, resize, keyboard, and long-run UI matrix remains unrun. A separate
before-baseline capture was not available for this slice.

## Refactor proof

- **Current owner:** `OpenVisionPipelineReviewExecutionController` owns
  `stepResultSummaries`, `reviewLayerImages`, execution generation, and
  cancellation. The defect was the gap between callback stamp validation and
  observable state/event application.
- **Intended owner:** the same controller owns one synchronization boundary for
  validation plus all callback-visible updates. Reset/Close uses that boundary
  when invalidating and retiring state.
- **Dependency direction:** Document -> execution controller -> UI dispatcher;
  the controller remains independent of Window/UserControl and no new owner was
  added.
- **State/data owner:** summary and review-cache dictionaries remain in the
  controller; no mutable result state moved into the Document or View.
- **Observable contract:** current-run output, stale queued callback rejection,
  Reset/Close retirement, Recipe/XML, Layer routing, and explicit Preview/Run
  behavior are preserved by focused contracts and representative UI smoke.

Status: Complete
Scope: OVL-07 Pipeline Review stale callback atomic application boundary.
Acceptance criteria: an entered UI callback cannot be interleaved by Reset/Close
between validation and summary/cache/event application; completion application
uses the same boundary; existing execution/cache/owner contracts remain stable;
focused Debug/Release and representative UI verification pass.
Verification: app/solution/runner builds, stale-callback contract in Debug and
Release, existing Pipeline Review execution/cache/layer-owner contracts,
readiness, static proof, documentation-index validation, and Release normal+NG
UI smoke.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-stale-callback-20260908`.
Boundary / next dependency: this proves the controller callback boundary; it
does not prove every Document-level result projection revision or the full UI
matrix. Next single priority: `OVL-07 Pipeline Review Document result
projection revision guard` | Recommended model: `gpt-5.6-terra` | Reasoning effort:
`high`.
