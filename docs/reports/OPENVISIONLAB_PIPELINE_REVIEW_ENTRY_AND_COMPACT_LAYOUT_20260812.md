# Pipeline Review Entry, Startup Feedback, And Compact Layout

Date: 2026-08-12 KST

## Outcome

The Dev workspace now gives immediate themed feedback while the last Recipe and
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
Scope: Dev and original startup feedback, Pipeline Review entry prewarm/cache reuse, compact responsive layout, localized guide toggle, and stale-result invalidation
Acceptance criteria: Startup feedback before main window -> pass at 1666 ms versus 4937 ms prior no-window interval; startup popup closes only after completion -> pass; Startup/Recipe-switch/after-Tool cached Pipeline entry -> pass at 5/6/47 ms; explicit-action contract -> pass with 0 Preview runs and unchanged layer counts; 1280x800 image usability -> pass at 208x156 collapsed and 185x138 guide-expanded; current EXE visual review -> pass
Verification: Dev and original Debug builds 0 warnings/errors; both repositories passed loading feedback smokes 2/2, readiness 13/13, external references, and public assets; Dev focused Pipeline Review UI/performance smokes 2/2; 26/26 promoted Git object hashes match
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab\pipeline-startup-feedback-20260812 plus D:\OpenVisionLab-TestData\OpenVisionLab\pipeline-review-compact-performance-20260812\final_actual_exe_v2
Boundary / next dependency: commit and push remain; this does not publish a tagged Release or prove hardware/field performance
```
