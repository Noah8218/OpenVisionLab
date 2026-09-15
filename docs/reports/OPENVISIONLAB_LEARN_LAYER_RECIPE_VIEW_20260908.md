# Learn Layer / Pipeline / Recipe boundary — 2026-09-08

## Work contract

The user explicitly requested the next refactoring task and asked that it include
all necessary comments, documentation, verification and closure, then stop.
This new request authorizes one independently verifiable Learn topic boundary;
it does not reopen the completed Matching View extraction or resume scheduling.

Source: `C:\Git\2D\Dev`, branch `codex/public-sample-ux-docs`, HEAD
`d875559577c85984d54900df973a6fb35fb20146`, including the existing dirty worktree.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-learn-layer-recipe-20260908`.
Source snapshots and worktree SHA-256 inventory precede implementation; generated
outputs and process TEMP/TMP remain on D:. Existing build-output junctions remain.

| Responsibility | Current owner | Intended owner |
| --- | --- | --- |
| Four teaching routes, selected step, explanation, highlighted layer decisions | Window private fields and methods | `LayerRecipeLearnPresenter`, no WPF/runtime Recipe dependency |
| Topic XAML, cell rendering, slider synchronization and timer lifetime | Window | `LayerRecipeLearnView.xaml(.cs)` |
| Theme definitions and shared cells | Existing LearnResources / LearnCellVisuals | Same owners, reused unchanged |
| Topic selection and Window Close | Window | Same owner, delegates refresh/stop to child View |

Call path: Window topic selection -> View refresh -> Presenter step/route state
-> View text/cell rendering. Slider/Step/Reset/Play stay local to this simulation;
no real Preview/Run, layer, recipe or persistence action is added. The View owns
its timer and UI slider re-entry guard; Presenter owns the teaching state.

Preserve XAML appearance/names/automation IDs and public Window test entry points.
In particular, Reset clears the animation/highlights while retaining the prior
slider value, formula and explanation; returning to the topic refreshes from the
slider. Preserve this existing distinction rather than simplifying it away.

Checkpoints and acceptance: capture current baseline; implement real Presenter
and View ownership; focused nonvisual and rendered control regressions; compare
before/after frames and moved XAML; build and update developer navigation/closure.
Scope excludes other topic/Shell extractions, UI redesign, new lessons, algorithms,
Recipe/XML/SDK changes, Original, commit/push/merge/release/deployment.

Initial source inspection shows the legacy Layer Recipe screenshot target still
expects English instructional text while the current XAML contains Korean copy.
Run it before/after and retain any proven pre-existing mismatch; do not weaken
its expectations or change learner copy as part of this structural extraction.

## Continuation boundary

After this task closes, no other model, agent or scheduled run may automatically
continue refactoring, cleanup, code comments, documentation updates, revalidation
or another priority. Historical continuation requests and next-task lists are
not authorization. Only a new explicit user request authorizes its named scope.
The existing `openvisionlab-2d` schedule was inspected as `PAUSED`; retain that
state and require a separate explicit request to resume it. No delegation is used.

## Implemented boundary

The new Presenter owns teaching state and route/highlight decisions. The new
UserControl owns its controls, rendering and timer; Window actually composes it
and delegates refresh/stop. No interface, factory, message bus or Window partial
was introduced. The new View partial is required by XAML-generated composition.

The View subscribes on Loaded, stops/unsubscribes on Unloaded, and does not
automatically resume after rehosting. Window Close also stops the View. Timer
updates retain the existing slider re-entry guard so a programmatic step does
not trigger the manual-selection pause behavior. Comments explain that guard,
the timer lifetime owner and Reset's intentionally retained selection.

`structure-proof.txt` proves the old Window no longer owns the route data,
animation state, timer, cell collections or ten topic methods. Five unchanged
UI handlers/helpers were compared directly with the starting snapshot. Every
formula/explanation literal from the former update method is preserved, including
the existing `Binary_Output` explanation. Parsed panel XAML is identical after
moving only outer Grid.Row/Visibility to the hosting UserControl. Existing control
names and automation IDs now belong to the View's namescope. Shared resources,
cell helpers and completed Matching View/Presenters are byte-identical.

Window C#: 4,021 -> 3,818 lines. Window XAML: 3,443 -> 3,293 lines. These counts
are navigation evidence; independent ownership and its actual call path are
the acceptance criteria.

## Changed files

All paths are relative to `C:\Git\2D\Dev`. This task owns 14 files:

| Files | Change |
| --- | --- |
| `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/LayerRecipeLearnPresenter.cs` | New independent teaching-state owner |
| `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/LayerRecipeLearnView.xaml` and `.xaml.cs` | New topic View and deterministic timer lifetime |
| `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/OpenVisionLearnWindow.xaml` and `.xaml.cs` | Compose View; remove moved responsibilities; retain public facade |
| `tools/VisionRecipeRunnerSmoke/LearnLayerRecipeContract.cs` and `Program.cs` | New four-group contract suite and focused dispatch |
| `tools/PipelineViewerScreenshotSmoke/LearnLayerRecipeSmoke.cs` and `Program.cs` | Baseline-compatible lesson contract, independent View test and focused dispatch |
| `AGENTS.md` and `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md` | Current completion scope and explicit no-follow-up boundary |
| `docs/admin/CODEBASE_STRUCTURE.md` | Section 5.4 ownership map, call path and debugging examples |
| `docs/LLM_DOCUMENT_INDEX.json` | Add this report to the structure route |
| This report | Work contract, evidence and closure |

The task-start inventory comparison also detected three externally changed
files: `docs/contracts/openvisionlab/OPENVISIONLAB_RULE_BASED_SKILL_REGISTRY.json`,
`docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_DEVELOPMENT_WORK_CONTRACT_20260831.md`,
and `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_13_ROUND1_RESULT_20260908.md`.
They were not edited by this task. The independently added XML execution section
in the shared current handoff was preserved; only its top refactoring closure
was replaced. This report does not claim an isolated worktree or validate that
separate XML evaluation. Existing user changes remain intact.

## Verification actually run

All evidence paths below are relative to the D-drive evidence root above.
Build/test process TEMP/TMP was directed to that root. No full regression suite,
production Recipe run or external service was needed for this lesson boundary.

| Check / command | Result | Evidence |
| --- | --- | --- |
| Baseline Release screenshot runner build | Pass, 0 errors | `baseline-build.log` |
| Baseline `wpf_openvision_learn_layer_recipe_contract` on the actual pre-change app | Pass | `baseline-ui/stdout.log`, `.contract.txt` |
| Debug app: `dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false --no-restore` | Pass, 0 warnings/errors | `app-debug-build.log` |
| Release x64 `PipelineViewerScreenshotSmoke.csproj` build | Pass, 0 errors; existing CS8600 in runner Program.cs | `settled-ui-build.log` |
| Release x64 `VisionRecipeRunnerSmoke.csproj` build | Pass, 0 warnings/errors | `unit-build-final.log` |
| `dotnet exec tools/VisionRecipeRunnerSmoke/bin/x64/Release/net8.0-windows7.0/VisionRecipeRunnerSmoke.dll --learn-layer-recipe-contract <evidence>/unit` | 4/4 pass: exact routes/highlights, Reset/restart, rounding/clamping, instance/culture independence | `unit.log`, `unit/learn-layer-recipe-contract.txt` |
| `run-after-settled.ps1`: `wpf_openvision_learn_layer_recipe_contract` | Pass: initial step 2, retained Reset selection/formula, frames 0..4, wrap, real timer, Play/Pause, manual slider pause, topic return, Close and zero Tool/sample/apply callbacks | `after-settled/stdout.log`, lesson `.contract.txt` |
| Same wrapper: `wpf_openvision_learn_layer_recipe_view` | Pass: standalone View/resources, two unload/reload cycles, no autoplay/duplicate Tick, host Close | `after-settled/` View `.contract.txt` |
| Same wrapper: `wpf_openvision_learn_matching_presentation` | Pass: adjacent topic composition regression | `after-settled/stdout.log` |
| `verify-structure.py` | Pass: actual new-owner use, removed Window coupling, unchanged XAML/shared files and legacy test | `structure-proof.txt` |
| Settled baseline/current PNG SHA-256 comparison | 5/5 identical stage frames | `frame-parity-settled.csv`, `baseline-settled/`, `after-settled/` |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools/TestDocumentationIndex.ps1 -RepoRoot C:\Git\2D\Dev` | Pass: 190 indexed paths, 13 routes, 102 redirects | `documentation-index.log` |

The first unit build found missing `System.IO` and unnamed tuple element access
in the new test harness. Those compile errors were corrected; assertions were
not removed or weakened. The failed `unit-build.log` remains alongside the
passing final build.

### Visual baseline identity and capture timing

The original before/after comparison had differences in frames 0..2 limited to
the **outer host practice Expander animation**, y=251..315. Frames 3..4 already
matched. `frame-pixel-difference.txt` retains this finding. The harness now waits
for that existing opening animation to settle before taking stage frames; no
production behavior or expected value changed.

The settled reference is explicitly a **reconstructed baseline**. A retained
pre-change Debug app DLL/PDB was copied into a D-drive runtime with the updated
capture harness. DLL CodeView/PDB GUIDs match, and both Window `.xaml` and `.cs`
PDB source SHA-256 values match this task's pre-change snapshots.
`baseline-identity.json` and `prepare-reconstructed-baseline.ps1` record that
proof. No repository source was restored or overwritten to reconstruct it.
`run-baseline-settled.ps1` passed, then all three current targets passed.

The resulting frames 0..4 are byte-identical to the current Release app captures.
The settled before/after step-3 screenshots were also viewed. This proves the
captured presentation at the recorded state/size, not every possible UI state.

### Known failure and unverified scope

The unchanged legacy `wpf_openvision_learn_layer_recipe` target fails **both
before and after** because it requires `Routing safety checklist` while the
starting XAML already contains Korean lesson copy. `baseline-legacy/stderr.log`
and `after-legacy/stderr.log` preserve the same failure. The original target and
all its expectations are byte-preserved; the new baseline-compatible contract
adds behavior coverage without replacing or masking that failure. This report
does not claim the full existing smoke suite is green.

Window topology was detected before each desktop launch. The selected smaller
left monitor was `\\.\DISPLAY2`, bounds `(-1920,365,1920,1080)`, working area
`(-1920,365,1920,1032)`. Only the launched test PID's windows were placed, and
their rectangles were verified inside that area in each `monitor-placement.json`.
Runtime evidence covers 96 DPI (100%), a 1040x700 Learn Window and a 760x700
standalone host. It exercises control values, rendered text, routed Button Click
events, timing and visibility/lifetime transitions. It does not prove physical
pointer-down/hover/focus/keyboard paths, every theme, named Wide/Compact layouts,
125/150/175/200% DPI, all resize/maximize cases, or commercial UI qualification.
The prior Matching document-copy failure was not rerun or reopened in this slice.

## Durable closure

Status: Complete

Scope: The Learn Layer / Pipeline / Recipe Presenter/View ownership boundary,
its behavior-preserving Window composition, focused tests, comments and
developer navigation only.

Acceptance criteria:
- Independent state and View owners actually used -> pass, structure proof and standalone View test.
- Existing selection, routes, Reset, timer and Close behavior -> pass, four unit groups and focused rendered contracts.
- Captured presentation and adjacent Matching composition preserved -> pass, five identical settled frames and adjacent UI target.
- Relevant builds and documentation navigation -> pass, logs above.
- Completion and no automatic continuation recorded -> pass, this report, AGENTS and current handoff; schedule retained PAUSED.

Evidence: D-drive logs, scripts, captures and `task-changes.csv` above; final
Git scope/status evidence is recorded beside them. Branch and HEAD remain the
stated input identity. No baseline files were removed. No commit, push, merge,
Original mutation, schedule resumption or delegated task was performed.

Boundary / next dependency: No next task is authorized. Another model or
scheduled run must not perform further code, comments, documents or verification
after this closure. A new explicit user request is required for a named scope;
resuming the 15-minute schedule requires separate explicit authorization.
Known legacy-copy failure and unrun UI matrix are limitations of the recorded
evidence, not implicitly assigned follow-up work.
