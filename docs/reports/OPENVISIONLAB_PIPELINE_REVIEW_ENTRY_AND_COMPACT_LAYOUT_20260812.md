# Pipeline Review Entry, Startup Feedback, And Compact Layout

Date: 2026-08-12 KST

## 2026-08-13 Actual Desktop EXE Recheck After Operator Report

The earlier `46 ms` result came from a direct WPF regression runner. It did
not prove the operator's reported ten-second delay in the installed desktop
workflow, so the earlier completion claim was too broad.

The current Dev Debug EXE was rebuilt and driven as a desktop process through
the main no-image `Pipeline 열기` command. The application now writes
`[PipelineOpenTrace]` records for command entry/return, Tool selection return,
WPF render priority, application idle, cache state, and the existing internal
selection phases.

| Actual EXE path | Workflow | Click return | Command return | Render | UI idle |
| --- | --- | ---: | ---: | ---: | ---: |
| Dev | Main no-image `Pipeline 열기` | 101 ms | 19 ms | 26 ms | 47 ms |
| Dev | Threshold Tool -> close/open main `Pipeline 열기` | 110 ms | 25 ms | 29 ms | 57 ms |
| Original before promotion | Main no-image `Pipeline 열기` | 108 ms | not instrumented | not instrumented | not instrumented |
| Original before promotion | Threshold Tool -> main `Pipeline 열기` | 396 ms | not instrumented | not instrumented | not instrumented |
| Original after promotion | Main no-image `Pipeline 열기` | 102 ms | 18 ms | 23 ms | 40 ms |
| Original after promotion | Threshold Tool -> main `Pipeline 열기` | 98 ms | 5 ms | 12 ms | 29 ms |

The Computer Use state-capture operation took about `3.1 s`; it is excluded
from application latency because it occurred after the click returned and the
in-process trace had already reached WPF application idle. The ten-second delay
was not reproduced on this workstation. This does not disprove the operator's
observation.

Before promotion, the original source and EXE did not contain the Dev-only
change that keeps the prepared Pipeline Review document cached across native
Tool selection. After explicit approval, the reviewed patch was applied and
the original EXE was rebuilt. Both original runs recorded
`CachedBefore=True`; the after-Tool application-idle time was `29 ms`.

Current actual-EXE evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab\pipeline-main-button-actual-exe-20260813`.
It contains the Dev trace, EXE SHA-256 values and timestamps, measured paths,
the actual Pipeline Review screen, and the selected-monitor intersection
record.

Original promotion and actual-EXE evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab\pipeline-main-button-original-promotion-20260813`.
It contains the original build/focused-smoke outputs, exact EXE identity,
direct and after-Tool screenshots, and the complete original trace.

The current focused regression also passed after trace insertion. Startup,
Recipe switch, and immediate Tool-plus-Step-add selection completed in
`36/13/46 ms`; Preview remained `0 -> 0`, Layer count remained `1 -> 1`, the
active Layer remained `Main`, and the immediate route remained
`Main -> Threshold_Preview`. Readiness passed all 13 contracts.

The promoted original independently passed the same focused regression at
`33/14/39 ms` selection and retained the same Preview, Layer, active-Layer,
and routing invariants. Its Debug build passed with zero warnings and errors,
and readiness passed all 13 contracts.

## 2026-08-13 Corrected Main-Button Scope

The earlier after-Tool number waited for the 800 ms idle prewarm and therefore
did not represent an operator who opens a Tool, adds a Step, and immediately
clicks the main no-image `Pipeline 열기` button. The corrected regression now
executes `OpenSamplePipelineCommand`, adds a Threshold Step, performs no idle
wait, and also checks Preview count, Layer count, active Layer, and Tool route.

With a 40-Step Pipeline, the previous commit discarded the prepared review
when the native Tool opened: `CachedBefore=False`, document activation
`199 ms`, and internal open `293 ms`. The corrected implementation keeps the
same Recipe/Pipeline document alive, refreshes it immediately when Add Pipeline
saves a Step, and then opens through the exact main button at `46 ms`
(`CachedBefore=True`). The full smoke ready value was `542 ms`; this includes
the runner's fixed dispatcher pumping after the 46 ms command and is not the
button's synchronous work.

Current evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab\pipeline-main-button-speed-20260813\large-pipeline-final\wpf_pipeline_review_entry_perf.perf.txt`.
Baseline evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab\pipeline-main-button-speed-20260813\large-pipeline-baseline`.

## Outcome

The Dev and original workspaces now give immediate themed feedback while the last Recipe and
Pipeline Review are prepared, keeps Pipeline Review outside the operator's
click path, and uses a denser review layout that gives the input and output
images the largest practical share of the window. The normal workflow remains
explicit: opening or restoring Pipeline Review does not run the Pipeline,
create a Layer, or change routing.

## Implemented Scope

- Show a non-dismissible localized startup window before synchronous Recipe
  restore begins; close it only after the main shell and Pipeline Review cache
  are ready.
- Prepare the first Pipeline Review while that startup feedback remains visible.
- Do not warm all native Tool Views during initial launch; their existing
  post-selection idle warmup remains available without freezing first paint.
- Show a themed, input-blocking Pipeline loading overlay when the selected
  Recipe/Pipeline does not yet have a matching cached review document.
- Resume the prewarm after Recipe selection without extending the Recipe
  loading-popup lifetime.
- Reuse the cached Pipeline Review document after a native Tool View closes.
- Refresh a cached document only when its Pipeline file changed; update current
  Layer readiness separately.
- Invalidate old review results when the Main image or native Tool output
  changes, without reacting recursively to Pipeline Review's own execution
  events.
- Remove the redundant shell Layer-row rebuild from the Pipeline selection
  path.
- Compress the title, Recipe/Pipeline identity, readiness strip, Step summary,
  spacing, and detail allocation.
- Collapse the detailed review guide by default behind a localized themed
  toggle.
- Below a 650-pixel review-view height, hide the duplicate Step summary and use
  the space for the two image panes. When the guide is expanded at that size,
  temporarily hide the lower detail tabs so the image panes remain useful.

## Performance Evidence

Focused current-source entry smoke:

| Path | Cached before click | Select | Ready | Preview runs | Layers |
| --- | ---: | ---: | ---: | ---: | ---: |
| Startup | yes | 5 ms | 11 ms | 0 -> 0 | 1 -> 1 |
| Recipe switch | yes | 6 ms | 63 ms | 0 -> 0 | 1 -> 1 |
| After native Tool | yes | 47 ms | 136 ms | 0 -> 0 | 1 -> 1 |

Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab\pipeline-startup-feedback-20260812\pipeline_perf\wpf_pipeline_review_entry_perf.perf.txt`.

The actual desktop EXE portfolio scenario exercised the main-window
`Pipeline 열기` command after idle prewarm. Its measured selection path was
12 ms, with 8 ms recorded inside Tool selection. The earlier instrumented
version of the same scenario exposed a 1,448 ms selection path before the
redundant shell Layer-row refresh was removed.

## Startup And Loading Feedback Evidence

The current pre-change EXE exposed no application window for 4,937 ms while it
restored the last Recipe and constructed the shell. The updated actual EXE
showed its startup feedback at 1,666 ms, before the main window appeared at
5,025 ms. This is a responsiveness/feedback correction, not a claim that all
startup work completes in 1.666 seconds.

- Actual application lifecycle probe:
  `D:\OpenVisionLab-TestData\OpenVisionLab\pipeline-startup-feedback-20260812\after_probe`.
- Current actual-EXE Korean/English loading UI and close-lock verification:
  `D:\OpenVisionLab-TestData\OpenVisionLab\pipeline-startup-feedback-20260812\focused\startup`.
- Current actual-EXE Pipeline overlay and empty-workspace contract:
  `D:\OpenVisionLab-TestData\OpenVisionLab\pipeline-startup-feedback-20260812\focused\workspace`.

The startup smoke verified both languages and proved that a normal Close call
cannot dismiss the startup window before the explicit completion signal. The
Pipeline overlay covers and blocks the shell until document preparation ends.

## Current Actual-EXE Layout Evidence

Artifact root:
`D:\OpenVisionLab-TestData\OpenVisionLab\pipeline-review-compact-performance-20260812\final_actual_exe_v2`.

- 1920 x 1500 full state:
  `02_pipeline_review_actual_exe.png`
- 1600 x 900 wide state:
  `03_pipeline_review_wide_actual_exe.png`
- 1280 x 800 compact state:
  `04_pipeline_review_compact_actual_exe.png`
- 1280 x 800 compact state with review guide expanded:
  `05_pipeline_review_compact_guide_expanded_actual_exe.png`
- Exact executable, managed assembly, input image, Pipeline hashes, monitor
  placement, timing, and preview dimensions: `report.txt`

Measured 1280 x 800 image content:

- Guide collapsed: input 208 x 156, output 208 x 156.
- Guide expanded: input 185 x 138, output 185 x 138.

The current captures were inspected for clipped text or icons, overlapping
content, hidden button labels, and theme-default white leaks. No such defect
was visible in the captured states. The EXE was placed on `\\.\DISPLAY2`; its
recorded window rectangle intersected that monitor. Capture did not use a
Computer Use overlay or cursor visualization.

## Verification

- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"`
  - PASS, 0 warnings, 0 errors.
- `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review,wpf_pipeline_review_entry_perf <artifact>`
  - PASS, two focused targets; layout/text/internal issue counts all zero.
- Current embedded-EXE `startup-loading-feedback` and
  `workspace-startup-empty` smokes
  - PASS; Korean/English startup copy, completion-only close, themed Pipeline
    overlay, empty workspace, zero auto-open Tool, and zero docked Layers.
- `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- C:\Git\OpenVisionLab_Dev`
  - PASS, readiness contract.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1`
  - PASS.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1`
  - PASS, 33 catalog rows, 229 manifest assets, 17 Pipelines.
- `git diff --check`
  - PASS; only configured LF-to-CRLF worktree notices were emitted.

## Durable Closure

```text
Status: Complete
Scope: Dev and original Pipeline Review cache reuse, exact main-button tracing, original promotion, startup feedback, compact responsive layout, localized guide toggle, and stale-result invalidation
Acceptance criteria: Dev actual EXE direct/after-Tool idle -> pass at 47/57 ms; original actual EXE direct/after-Tool idle -> pass at 40/29 ms with click return 102/98 ms and CachedBefore=True; explicit-action contract -> pass in both focused smokes with 0 Preview runs, unchanged layer counts, Main active, and unchanged routes; original build/readiness -> pass at 0 warnings/0 errors and 13/13
Verification: Dev and original Debug builds passed; both focused Pipeline-entry regressions passed; both actual desktop EXEs were clicked on the exact main no-image command and retained in-process PipelineOpenTrace evidence; original window was fully inside the selected left DISPLAY2
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab\pipeline-main-button-actual-exe-20260813 and D:\OpenVisionLab-TestData\OpenVisionLab\pipeline-main-button-original-promotion-20260813
Boundary / next dependency: The originally reported ten-second observation was not reproduced on this workstation; reopen only if the current original EXE produces a new trace showing the delay. Commit, push, and tagged Release publication remain separate actions.
```
