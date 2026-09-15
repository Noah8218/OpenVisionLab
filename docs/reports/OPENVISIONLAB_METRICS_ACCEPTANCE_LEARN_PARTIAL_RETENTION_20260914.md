# Metrics Acceptance Learn View Partial retention — 2026-09-14

## Status

Complete for the PL-0053 no-change structural audit. `MetricsAcceptanceLearnView.xaml.cs`
remains a required XAML/presentation adapter; no production split was justified.

## Current and intended owners

- Current owner: `MetricsAcceptanceLearnView` owns the WPF namescope, sample-cell
  construction, brush/text projection, Play/Step/Reset button state, and the
  520 ms `DispatcherTimer` attach/stop/unsubscribe lifetime.
- Intended owner: the same View remains the presentation adapter. The existing
  `MetricsAcceptanceLearnPresenter` owns fixed samples, average/range/maximum,
  average/outlier gate decisions, animation stage, formulas, status text, and
  numeric sample text.
- `OpenVisionLearnWindow` owns topic visibility, initial/changed refresh calls,
  the public test facade, and child close lifetime. Existing smoke/contract
  owners remain the verification boundary.
- Mutable-state writer: `MetricsAcceptanceLearnPresenter.AnimationStep`; the
  View only requests `ResetAnimation`/`AdvanceAnimation` and projects results.
- Lifetime owner: the View owns its `DispatcherTimer`; `Loaded` subscribes,
  `Unloaded` stops and unsubscribes, and the Window stops it during close.

## Call path and contracts

```text
OpenVisionLearnWindow constructor / topic update
  -> metricsAcceptanceLearnView.RefreshFrame / Visibility
MetricsAcceptanceLearnView Play/Step/Reset/timer
  -> MetricsAcceptanceLearnPresenter.ResetAnimation/AdvanceAnimation
  -> sample cells, formula, status, and button projection
OpenVisionLearnWindow.OnClosed
  -> MetricsAcceptanceLearnView.StopAnimation
  -> MetricsAcceptanceLearnView.Unloaded -> timer stop + Tick unsubscribe
```

The binding/public contract is the existing named XAML controls and automation
IDs (`metricsAcceptanceSampleGrid`, Play/Step/Reset, gate cheat sheet and
animation status), plus the Window `MetricsAcceptance*ForTest` facade. The
matching tests are `LearnMetricsAcceptanceContract` (presenter invariants) and
`LearnMetricsAcceptanceSmoke` (Window and standalone View lifetime/UI flow).

## Why no production split was made

The View contains no persistence, file-dialog, OpenCV, Recipe execution,
Tool-factory, or algorithm-lifetime coupling. The presenter is WPF-free and
already owns the only lesson policy/state. Moving button/timer/brush projection
into another service or Partial would create a forwarding wrapper and split a
single visual lifetime without reducing dependency direction or adding an
independently testable owner. The correct structural action for this slice is
to retain the Partial and protect the seam with a focused source contract.

## Developer reading order

1. `MetricsAcceptanceLearnView.xaml` — namescope, automation IDs, and input
   events.
2. `MetricsAcceptanceLearnView.xaml.cs` — WPF projection and timer lifetime.
3. `MetricsAcceptanceLearnPresenter.cs` — samples, gates, stages, and text.
4. `OpenVisionLearnWindow.xaml.cs` — topic selection, refresh, test facade, and
   close lifetime.
5. `LearnMetricsAcceptanceContract.cs` and `LearnMetricsAcceptanceSmoke.cs` —
   non-visual invariants and runtime/UI contract.

## Verification evidence

- `MetricsAcceptanceLearnPartialBoundaryContract`: 12/12 in Debug and Release.
- `LearnMetricsAcceptanceContract`: 3/3 in Debug and Release.
- `VisionRecipeRunnerSmoke` Debug build: 0 errors, 2 existing MSB3270
  processor-architecture warnings; Release build: 0 warnings, 0 errors.
- `RunUiPrecheck.ps1` Debug and Release: both
  `wpf_openvision_learn_metrics_acceptance_contract` and
  `wpf_openvision_learn_metrics_acceptance_view` returned `OK`, with layout,
  text, and internal-error counts at zero and fresh 1040x700/760x700 PNGs.
- `OpenVisionReadinessCheck` Debug and Release: all 13 readiness contracts
  returned `OK`.
- `Invoke-RefactorAudit.ps1 -Verify`: `REFACTOR_AUDIT=PASS` with zero project
  cycles and zero Shell storage calls; `TestDocumentationIndex.ps1` passed with
  322 indexed paths, 17 routes, and 102 redirects; the LLM index JSON parsed
  with the `start_or_continue` route and PL-0053 reference present; and
  `git diff --check` exited 0.
- Dynamic monitor probes selected the smaller-left `\\.\DISPLAY2` from two
  detected monitors (`X=-1920,Y=365`, `1920x1080`). Debug and Release windows
  were explicitly moved there; `MoveSucceeded=true`,
  `IntersectingWindowCount=1`, `IntersectsSelectedMonitor=true`, and the smoke
  contract files contained only PASS lines.

Evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\metrics-acceptance-learn-partial-retention-20260914`.

## Boundary / not verified

This slice does not prove all theme variants, Wide/Compact layouts, pointer
hover/pressed/focus/keyboard paths, 125/150/175/200% DPI, native Tool/file
association, camera/SDK/GPU, real Recipe execution/persistence, or long-running
native runtime. The older legacy `wpf_openvision_learn_metrics_acceptance`
target has a documented wording expectation mismatch in the 2026-09-08 report;
this slice used the focused contract and View-boundary targets and did not
silently weaken or delete that legacy check.

## Durable completion record

```text
Status: Complete
Scope: PL-0053 Metrics Acceptance Learn View Partial owner audit and focused evidence.
Acceptance criteria: owner/call path/state/lifetime/binding/test map recorded; no direct external coupling; focused Debug/Release contracts, builds, WPF smoke, and monitor placement passed.
Verification: MetricsAcceptanceLearnPartialBoundaryContract 12/12; LearnMetricsAcceptanceContract 3/3; Debug/Release WPF precheck; dynamic monitor probes.
Evidence: this report, .proofline/issues/PL-0053.json, and the D: evidence root above.
Boundary / next dependency: broader WPF visual/input/DPI and native/Recipe runtime remain unverified; do not reopen without a new defect, requirement, failed criterion, or changed dependency/lifetime boundary.
```
