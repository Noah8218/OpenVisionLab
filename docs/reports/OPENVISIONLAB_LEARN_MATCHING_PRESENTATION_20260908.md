# Learn Matching presentation boundary — 2026-09-08

## Work contract

Continue the user-requested junior-readable structure refactoring, completing one
independently verifiable boundary in this run. Source: Dev
`codex/public-sample-ux-docs`, `d875559577c85984d54900df973a6fb35fb20146`, including
the existing dirty worktree. The previously completed Recipe execution slice is
preserved and is not reopened.

Included: Matching/EdgeBasedMatching and FeatureMatching teaching inputs, derived
explanations, candidate decisions, and animation-step state. Excluded: XAML/View
extraction, layout/style changes, new lessons, algorithms, Recipe/XML/SDK changes,
Tool execution/routing changes, Original, commit, push, and deployment.

| Responsibility | Before | Intended owner and proof |
| --- | --- | --- |
| Template/edge topic state, text, score acceptance | Learn Window controls and fields | `MatchingLearnPresenter`, executable without a Window |
| Feature good-match explanation and step state | Learn Window controls and fields | `FeatureMatchingLearnPresenter`, executable without a Window |
| Matching simulation values and score calculation | Existing SimulationModel | Same model, byte-identical source |
| Timer start/stop, drawing cells, Window lifecycle, Tool links | Learn Window | Same View owner; focused runtime checks |

Call path: UI event -> Presenter input/state update -> existing SimulationModel ->
Presenter explanation/result -> Window control rendering. Presenters reference no
WPF control, timer, Shell, DisplayManager, or recipe service. The two presenters
have independent state; they are concrete topic owners, not partials or wrappers
around Window private fields.

Acceptance: no Matching-family evaluation calls, simulation-derived explanation
logic, or mutable stage fields remain in Window; preserved formulas, labels, stages and UI control
contracts; focused nonvisual and UI regressions pass; before/after source and
render evidence plus developer navigation are retained.

## Baseline and evidence scope

Physical evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-learn-matching-20260908`.
All generated artifacts and process TEMP/TMP use D:. Existing build-output
junctions are retained; no storage migration or source-tree move is performed.

- Current-source baseline build passed (0 warnings, 0 errors).
- Existing `wpf_openvision_learn_matching` and
  `wpf_openvision_learn_feature_matching` passed before production edits.
- Existing `wpf_openvision_learn_edge_based_matching` reached its UI state checks
  and then failed `AssertLearnTopicDocument`: unchanged
  `docs/learn/LEARN_EDGE_BASED_MATCHING.md:73` contains `must not`. This is a
  pre-existing document-copy gate, outside the presentation extraction. Its test,
  expectation, and document are preserved.
- Added `wpf_openvision_learn_matching_presentation` before production edits;
  its baseline passed. It verifies topics 9 -> 12 -> 10 -> 9, slider-to-formula
  refresh, reset, frames 0..3, wrap/restart, changed-input result frame,
  play/pause, and Close of both timers. It raises routed Button Click events;
  it does not claim real pointer-down, keyboard, or full interaction coverage.
  It captures 16 deterministic frame images and verifies no Tool callback.
- Test windows are placed by a D-only PID-scoped wrapper on the dynamically
  selected monitor. Final topology and rectangles belong in the run artifacts.

This refactor preserves the existing XAML and presentation text. It does not
qualify new themes/styles or the full 100/125/150/175/200% DPI, Wide/Compact,
resize, hover/pressed/focus, popup, or field-usage matrix.

## Implementation and checks

- New production owners: `MatchingLearnPresenter.cs` and
  `FeatureMatchingLearnPresenter.cs` in the existing Learn folder.
- Updated `OpenVisionLearnWindow.xaml.cs` delegates input/state transitions and
  explanation derivation. Existing XAML, SimulationModel, UI bindings, public
  test hooks, button order, timer intervals, Tool link callbacks, and Close
  behavior remain intact. Topic-navigation and Tool-location guidance stay in
  Window pending the later View boundary.
- New feature-owned contract files are linked through small entries in both
  existing smoke runners; no new test framework or production interface.
- Release application/contract-runner build: PASS, 0 warnings, 0 errors.
- Debug x64 application build: PASS, 0 warnings, 0 errors
  (`app-debug-build.log`; `dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false --no-restore`).
- Nonvisual contract: 4/4 PASS, covering template candidate scores and inclusive
  threshold, edge/template switching, stage reset/restart, invariant culture,
  feature OK/NG count boundaries, and independent presenter instances.
- Current UI run: new family contract plus existing Matching and FeatureMatching
  targets PASS. All 16 baseline/after frame PNGs have identical SHA-256 values
  (`frame-parity.csv`). Representative EdgeBasedMatching before/after frames were
  opened and displayed in chat.
- Existing EdgeBasedMatching document gate was rerun and failed for the same
  unchanged `must not` text, after reaching its UI checks. It is not a regression
  introduced by the presenters; it is recorded rather than removed or weakened.
- UI runner builds retain the existing CS8600 warning in `Program.cs` (now line
  10727 after dispatch insertion); no warning was suppressed.
- Final UI run selected the smaller left `\\.\DISPLAY2`, bounds
  `(-1920,365,1920,1080)`. Actual test windows `(-1480,531)..(-440,1231)` were
  contained in its working area. See `after-ui/monitor-placement.json`.

Commands actually used (TEMP/TMP were set to the D evidence root):

```powershell
dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Release -p:Platform=x64 -p:WpgCustomBuildEnabled=false -p:UseAppHost=false -m:1 -nr:false --no-restore
dotnet exec tools/VisionRecipeRunnerSmoke/bin/x64/Release/net8.0-windows7.0/VisionRecipeRunnerSmoke.dll --learn-matching-presentation-contract D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-learn-matching-20260908/unit
dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Release -p:Platform=x64 -p:WpgCustomBuildEnabled=false -p:UseAppHost=false -m:1 -nr:false --no-restore
```

The D-only `run-baseline-ui.ps1`, `run-baseline-contract.ps1`, `run-after-ui.ps1`,
and `run-edge-document-check.ps1` record the exact `dotnet exec --target` commands,
process output, and monitor placement. They preserve earlier evidence directories.

## Closure

Status: Complete

Scope: the Matching-family simulation presentation/state boundary and developer
navigation only. The whole Learn Window/View split remains a separate task.

Acceptance criteria: direct evaluation calls in Window 4 -> 0 and mutable
Matching-family stage fields 2 -> 0 -> PASS (`structure-proof.txt`); independent
presenter behavior -> PASS (4 nonvisual contracts); control/state/Close behavior
-> PASS (new family UI contract and two existing targets); unchanged covered
screen output -> PASS (16/16 identical PNGs); navigation/index -> PASS
(`documentation-index.log`, 186 indexed paths, 13 routes, 102 redirects).

Verification: builds, exact target names, failures, and commands are listed above.
`git diff --check` passed. Window decreased from 4,707 to 4,611 lines; the real
change is state and explanation ownership, not a file-size claim. `OnClosed` and
both timer callback bodies are unchanged. New files contain no WPF control/timer
dependency. Tests and source snapshots are stored with the task evidence.

Changed files: three production files (two presenters and Window code-behind),
four test files (two feature suites and two runner entries), and four documents
(this report, structure guide, live handoff, and document index). No unrelated
worktree file changed in the task hash comparison. Existing user work and Recipe
execution changes remain preserved. Branch/HEAD did not change; no Original,
reset/clean, commit, push, merge, or deployment action was performed.

Boundary: the old EdgeBasedMatching learner-document check still fails and is
separately recorded. Full UI theme/DPI/interaction qualification was not run;
the passing frame comparison proves only the exercised current Learn layout and
teaching states. Focus, actual pointer states, and production algorithm accuracy
are not inferred from these structural tests.

## Next boundary

Extract the Matching topic View/XAML and its rendering/lifecycle as one cohesive
UI module, retaining these presenters and the simulation model. First resolve
shared resource lookup and namescope ownership without duplicating theme styles.
Recommended model: `gpt-5.6-terra`; Reasoning effort: `high`.
