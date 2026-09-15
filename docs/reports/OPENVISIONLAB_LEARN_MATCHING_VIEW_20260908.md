# Learn Matching View boundary — 2026-09-08

## Work contract

Complete one user-authorized structural refactor in `C:\Git\2D\Dev`, preserving
the dirty worktree at `codex/public-sample-ux-docs`,
`d875559577c85984d54900df973a6fb35fb20146`.

Included: Matching/EdgeBasedMatching and FeatureMatching XAML, cell rendering,
control wiring, topic-local Tool links, timer ownership and lifetime. Reuse the
completed presenters and simulation model. Keep the Window's public test facade,
topic catalog, outer navigation, shared visual definitions and observable UI.
Excluded: new lessons, restyling, algorithms, Recipe/XML/SDK changes, other topic
extractions, Original, commit/push/release/deployment.

| Boundary | Current owner | Intended owner |
| --- | --- | --- |
| Two topic panels, named controls, cell collections | Learn Window | `MatchingLearnView.xaml(.cs)` |
| Matching-family timers and event handlers | Learn Window | Matching View, stopped at host Close and View unload |
| Simulation decisions and stages | Existing presenters | Same independent presenters |
| Learn style/brush definitions | Window resources | One `LearnResources.xaml`, reused by Window and View |
| Reused small-cell drawing primitives | Window private methods | Shared `LearnCellVisuals`, used by Window and View |
| Topic selection, explicit external Tool callback | Window | Window passes topic/action into View; no View -> Window dependency |

Acceptance: Window no longer owns Matching controls/rendering/timers; the real
XAML uses the new View; existing public test entry points forward without exposing
child controls. Focused builds and presenter/UI regressions pass. Fresh before/
after frames prove the exercised visual output; independent View hosting,
unload/reload and explicit Tool-link checks prove the new boundary.

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-learn-matching-view-20260908`.
Generated outputs and TEMP/TMP remain on D:. Existing bin/obj junction targets
were inspected and remain D-backed. Source snapshots and worktree hashes precede
implementation. Checkpoints: current baseline -> extraction -> focused runtime
proof -> developer navigation and durable closure.

Known baseline issue: the previous EdgeBasedMatching smoke reaches its UI checks
then fails an unrelated learner-document copy gate (`must not` in the unchanged
`LEARN_EDGE_BASED_MATCHING.md`). Do not delete, skip or weaken that expectation.
Full theme/DPI/mouse-state qualification is not inferred from source inspection
or frame comparison; record the exact exercised states and unverified limits.

## User-directed continuation boundary

The current user explicitly requested that this task include all necessary code
comments, documentation and completion evidence, then stop. After closure, no
model, agent or scheduled run is authorized to continue refactoring, cleanup,
comments, documentation updates, revalidation or another priority automatically.
Historical “continue” instructions and “next task” entries are not authorization.
Only a new explicit user request authorizes its named scope. Resuming the schedule
requires a separate explicit request. The existing `openvisionlab-2d` heartbeat
was updated through the app tool and verified `PAUSED`; its 15-minute interval
and target task were retained. No new agent was delegated.

## Structural result and verification

- Window XAML now composes `MatchingLearnView`. Both topic panels and their
  namescopes moved; Window no longer owns their controls, cell collections,
  presenters, timer fields, rendering methods or topic-local Tool responses.
- Call path: Window topic/action -> View -> existing Presenter/SimulationModel ->
  View drawing. Window Close -> View StopAnimations; View Unloaded -> timer stop
  and Tick detach; Loaded -> Tick attach, with no implicit playback.
- The 29 moved rendering/input/Tool-link methods match baseline text after only
  replacing the Window topic-list lookup with explicit topic input. Both panel
  XAML trees match except their former outer `Grid.Row` attachment. Resource
  definitions match exactly as parsed XML. `structure-proof.txt` records checks.
- `LearnCellVisuals` owns the two genuinely shared drawing methods. The gray
  brush takes an integer; clamping that integer to 0..255 preserves the old
  double/round helper result without making the View depend on Window.
- The new partial is the required XAML-generated `MatchingLearnView` composition,
  a separate type with its own state and lifetime, not another Window partial.
- Window C#: 4,611 -> 4,021 lines. Window XAML: 3,862 -> 3,443 lines.
  Counts are navigation information; ownership and runtime checks prove the split.
- Existing source BOMs and unchanged line bytes were preserved. Window already
  had mixed line endings at baseline; new files use consistent CRLF. No bulk
  formatting or namespace migration was performed.

| Check actually run | Result / evidence |
| --- | --- |
| Current baseline UI-runner Release build | PASS, 0 warnings/errors; `baseline-build.log` |
| Baseline family presentation, Matching, FeatureMatching UI targets | 3 PASS; `baseline-ui/stdout.log` |
| Changed application / UI runner Release build | PASS; existing CS8600 at Program.cs:10732 remains, 0 errors; `focused-build.log` |
| Presenter contract runner Release build and contract | 4 PASS, 0 failures; `unit-build.log`, `unit/learn-matching-presentation-contract.txt` |
| Changed family presentation, Matching, FeatureMatching UI targets | 3 PASS; `after-ui/stdout.log` |
| New independently hosted Matching View contract | PASS: standalone resources, all 3 exact Tool callbacks, no implicit callback, 2 unload/reload cycles, retained inputs, no autoplay or duplicate Tick, independent host Close; `after-ui/wpf_openvision_learn_matching_view.contract.txt` |
| Shared visual consumers: Threshold and Brightness | 2 PASS; `after-ui/stdout.log` |
| Existing EdgeBasedMatching UI/document target | FAIL at the same unrelated document-copy gate, after UI checks; `after-edge-document-check/stderr.log`. Document SHA-256 equals task baseline. No test or expectation was weakened. |
| Deterministic rendered frame parity | 16/16 identical SHA-256; `frame-parity.csv` |
| Debug x64 application build | PASS, 0 warnings/errors; `app-debug-build.log` |
| Documentation index | PASS, 188 indexed paths / 13 routes / 102 redirects; `documentation-index.log` |
| Final diff/status | `git diff --check` PASS; branch/HEAD unchanged; `diff-check-final.log`, `git-status-final.txt`, `task-changes.csv` |

The UI target family uses actual rendered controls and routed Click events,
slider changes, topic navigation, reset/steps, real timer play/pause and Close.
The independent View was rendered at 96 DPI (100%). Shared resources were tested
in the existing Learn appearance at 1040x700, Brightness at 1040x900, and the
independent host at 760x700. Monitor selection/actual rectangles are recorded in
`after-ui/monitor-placement.json`: smaller-left `\\.\DISPLAY2`, bounds
`(-1920,365,1920,1080)`. Current before/after EdgeBasedMatching frames were opened
and displayed in the task.

No actual pointer-down/hover/focus/keyboard, alternative product theme,
Wide/Compact, or 125/150/175/200% DPI matrix was run. These remain unverified;
the evidence establishes the moved boundary and exercised visual states only.
Other Learn topics, production algorithm accuracy, real equipment and full
application EXE/clean-runtime qualification are outside this task.

Commands (TEMP/TMP point into the D evidence root):

```powershell
dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Release -p:Platform=x64 -p:WpgCustomBuildEnabled=false -p:UseAppHost=false -m:1 -nr:false --no-restore
dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Release -p:Platform=x64 -p:WpgCustomBuildEnabled=false -p:UseAppHost=false -m:1 -nr:false --no-restore
dotnet exec tools/VisionRecipeRunnerSmoke/bin/x64/Release/net8.0-windows7.0/VisionRecipeRunnerSmoke.dll --learn-matching-presentation-contract D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-learn-matching-view-20260908/unit
dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false --no-restore
```

The D-only `run-baseline-ui.ps1`, `run-after-ui.ps1` and
`run-edge-document-check.ps1` retain exact `dotnet exec --target` arguments,
process output and monitor evidence. `verify-structure.py` checks ownership,
method/panel/resource equivalence and untouched presenters/model/document.

## Closure

Status: Complete

Scope: Matching-family View/XAML/rendering/lifetime extraction, developer
navigation, necessary ownership/lifetime comments, evidence and user-directed
automatic-continuation stop boundary.

Acceptance criteria: real new View in XAML and old ownership removed -> PASS;
covered UI output preserved -> 16/16 identical frames; existing Presenter
behavior -> 4 contracts PASS; independent View lifetime and explicit Tool action
boundary -> PASS; focused build/UI gates -> PASS, with the proven unrelated
unchanged document-copy precondition separately retained above.

Changed files: six production files (Window XAML/code-behind, Matching View
XAML/code-behind, LearnResources, LearnCellVisuals), two test files
(`LearnMatchingPresentationSmoke.cs`, runner `Program.cs`), and five instruction/
documentation files (AGENTS, structure guide, current handoff, index, this report).
Recipe/XML/SDK and previous user changes are preserved. No Original, reset/clean,
commit, push, merge, release or deployment action was performed.

The worktree hash comparison found these 13 task files plus a concurrent change
to `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_13_STRICT_CORPUS_PREFLIGHT_20260908.md`.
This task did not edit that separate report; it was preserved and is excluded
from the refactor's file count and completion claim. No baseline file was deleted.
The whole HEAD diff includes earlier user work and is not this task's change set.
Root `bin` was also verified to target the D-drive project test root.

Boundary / next dependency: no authorized automatic next task. The user must
explicitly request a new scope; schedule resumption needs a separate request.
Do not have another model add comments/docs, revalidate or reopen this completed
boundary solely to accumulate more evidence. The remaining UI matrix and old
learner-document issue are disclosed limits, not automatic follow-up assignments.
