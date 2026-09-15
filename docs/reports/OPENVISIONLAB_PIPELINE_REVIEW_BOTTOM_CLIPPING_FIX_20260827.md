# OpenVisionLab Pipeline Review Bottom-Clipping Fix

Date: 2026-08-27 KST
Repository: `C:\Git\OpenVisionLab_Dev`
Branch: `codex/public-sample-ux-docs`
Repository HEAD at verification (UI changes uncommitted):
`54f219acf0006c565688780bcc9175b6d3376c2f`

## Status

`Complete` for the bounded Pipeline Review layout fix.

This closes the reproduced bottom-content visibility defect in the current
Pipeline Review surface. It does not claim full theme/DPI qualification,
English visual qualification, or release/deployment readiness.

## User-visible problem

The lower Pipeline Review detail area used a fixed `130` pixel default row
(`180`/`160` for some compact diagnostic tabs). At the supported review
content density, the right-side `Validation`, `Result`, and `Run Log` sections
could extend below that row and be clipped by the surrounding layout. The
Object Results tab had the same bounded-height pressure: a long object table
could consume the available row and leave the metric distribution plot only
partially reachable.

The left `Step 흐름` rail already owns a separate vertical scroll path. This
fix targets the reproduced right-side lower detail card and the Object Results
content; the rail was not structurally changed.

The closest before-state capture is the current-source baseline captured before
this fix:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\vision-sdk-candidate-f4f0c0d\ui-object-parity-r4-direct\wpf_shell_host_pipeline_review.png`

The baseline is retained as a before-state reference, not as current after
evidence.

## Implemented scope

### Step Details

`src/OpenVisionLab/UI/Menu/Wpf/Views/OpenVisionPipelineReviewView.xaml.cs`

- Raises the review detail row to `240` pixels in the normal default case.
- Keeps the matcher and circle evidence tabs at `300`/`280` pixels in normal
  layout, and `240`/`220` pixels in compact layout.
- Uses `220` pixels for the compact default/object case so the two image
  previews retain useful height instead of being consumed by the detail card.

`src/OpenVisionLab/UI/Menu/Wpf/Views/OpenVisionPipelineReviewView.xaml`

- Gives the right-side result card a `MinHeight="280"` content surface.
- Wraps the result card in a named vertical `ScrollViewer` so the operator can
  reach the complete run log without shrinking the input/output previews.

### Object Results

`src/OpenVisionLab/UI/Menu/Wpf/Views/OpenVisionPipelineReviewView.xaml`

- Wraps the Object Results content in a named vertical/horizontal
  `ScrollViewer`.
- Sets the inner content to `MinWidth="880"` and `MinHeight="320"` so the
  table and distribution plot retain a deliberate minimum presentation area;
  compact windows expose a real horizontal scroll range instead of clipping
  the plot.
- Caps the object `DataGrid` at `300` pixels so large candidate sets scroll in
  the table and do not expand the plot's star row to an unbounded height.

No Pipeline execution, layer creation/deletion/selection, active-layer,
routing, recipe persistence, or result semantics were changed.

## Acceptance and evidence

| Check | Result | Evidence |
| --- | --- | --- |
| Normal Pipeline Review (OK), `1600x900` | Pass | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-bottom-final-20260827-final2\normal\wpf_shell_host_pipeline_review.png` |
| Pipeline Review acceptance failure (NG), `1600x900` | Pass | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-bottom-final-20260827-final2\ng\wpf_shell_host_pipeline_review_ng.png` |
| Object Results metric review, `1600x900` | Pass | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-bottom-final-20260827-final2\object\wpf_shell_host_workspace_sample_pipeline_review_metrics.png` |
| Step Details compact scroll-to-end, `1280x800` | Pass | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-bottom-final-20260827\step-compact-scroll-end\wpf_shell_host_pipeline_review.png` |
| Object Results compact scroll-to-end/right-end, `1280x800` | Pass | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-bottom-final-20260827\object-compact-scroll-end\wpf_shell_host_workspace_sample_pipeline_review_metrics.png` |

The compact checks used a verification-only local harness override to exercise
`1280x800`, assert the named scroll ranges, and capture the bottom/right state.
The override was removed and the smoke source was rebuilt before the final
`1600x900` captures.

The compact runtime measurements were:

- Step Details: extent `190.34 x 299.52`, viewport `195.25 x 157`, run-log
  element `195.25 x 111.72`.
- Object Results: extent `1296.86 x 347.26`, viewport `728 x 183`, metric plot
  `670.86 x 284.7`.

## Commands run

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" --nologo
dotnet build "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU" --nologo --verbosity:minimal
```

Both builds completed with `0` warnings and `0` errors. The final direct
current executable was:

`C:\Git\OpenVisionLab_Dev\tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.exe`

Final direct target results:

```text
wpf_shell_host_pipeline_review=OK ... size=1600x900
wpf_shell_host_pipeline_review_ng=OK ... size=1600x900
wpf_shell_host_workspace_sample_pipeline_review_metrics=OK ... size=1600x900
```

`git diff --check` produced no whitespace errors.

The repository wrapper `tools\RunUiPrecheck.ps1 -FailOnWarn` was also tried
with the three changed-area targets as one process. Its solution and smoke
build stages passed and its stdout recorded the normal and NG targets as
`OK`, but the combined process timed out at 120 seconds before the Object
Results target completed. The Object Results target was then rerun alone
through the same wrapper (`-SkipSolutionBuild -SkipRestore`) and passed in
`8.079` seconds. The three direct invocations above also pass in separate
processes. The combined-process timeout is retained as a smoke-harness
sequencing boundary, not hidden as a product pass:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-bottom-final-20260827-ui-precheck\ui_precheck_stdout.txt`

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-bottom-final-20260827-ui-precheck-object-only\ui_precheck_report.md`

## Regression boundary

The current evidence covers Korean, normal `1600x900`, compact `1280x800`,
OK/NG review state, object metric results, vertical scrolling, and the
Object Results horizontal scroll path. The following remain unverified by this
bounded fix and must not be inferred from these captures:

- English visual capture;
- dark/light theme matrix and hover/pressed/focused/disabled/popup states;
- Windows DPI `125%`, `150%`, `175%`, and `200%`;
- minimum-size/maximize/monitor-move matrix;
- every diagnostic tab (matcher, circle, fixture, geometry, and scale) at every
  compact size;
- EXE launch smoke, original-repository mutation, commit/push, release,
  installation, rollout, and deployment.

Those are separate verification or release decisions, not prerequisites for
this bounded source/UI fix.
