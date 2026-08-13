# OpenVisionLab Pipeline F5 Dock-Mode Performance Correction

Date: 2026-08-13
Status: Complete in Dev

## Scope

This correction covers the main no-image workspace's explicit `Pipeline 열기`
button when OpenVisionLab is launched from Visual Studio 2022 with F5. It does
not change Pipeline execution, Recipe contents, layer routing, Preview/Run,
algorithm parameters, or native Tool View docking behavior.

The investigation used the same persisted context for every measured click:

- Recipe: `Documentation_Public`
- Pipeline: `Public_Synthetic_Matching`
- Pipeline Review cache: `CachedBefore=True`
- Input state: no Main image

## Reproduction And Root Cause

The current Dev source reproduced the operator's delay in three F5 processes:

| Run | Command return | UI idle |
| --- | ---: | ---: |
| F5 before 1 | 10,139 ms | 10,181 ms |
| F5 before 2 | 10,119 ms | 10,185 ms |
| F5 instrumented | 9,971 ms | 10,003 ms |

The instrumented run isolated 9,947 ms to
`OpenVisionToolDockModeHelper.Apply(content, false)` while attaching Pipeline
Review to the central document workspace. That helper walks and applies
templates across a hosted Tool View tree to find Tool-specific dock shells.
Pipeline Review is a document, contains no such Tool shell, and does not need
that traversal. Visual Studio's WPF debugger/Live Visual Tree made the needless
walk approximately ten seconds; the direct EXE path happened to finish it
quickly, which is why the earlier direct-only verification missed the defect.

## Correction

- `OpenVisionDockedDocumentWorkspaceController` no longer applies the
  Tool-specific dock-mode traversal to general documents.
- The Pipeline Review floating path also skips that Tool-specific traversal.
- Native algorithm Tool Views retain the existing dock-mode application.
- Document attach phase timing remains in the performance log so a future
  regression can be isolated without guessing.

## Measured Result

Three fresh F5 processes after the correction produced:

| Run | Command return | Render priority | UI idle |
| --- | ---: | ---: | ---: |
| F5 after 1 | 23 ms | 33 ms | 60 ms |
| F5 after 2 | 19 ms | 27 ms | 47 ms |
| F5 after 3 | 19 ms | 34 ms | 54 ms |

The median command-return time changed from 10,119 ms to 19 ms. A fresh direct
EXE actual-button run returned in 15 ms, reached render priority in 20 ms, and
reached application idle in 39 ms. All after traces retained
`CachedBefore=True` and recorded zero-millisecond central document attach
phases with no `DockedDocumentApplyDockMode` phase.

## Verification

Commands executed from `C:\Git\OpenVisionLab_Dev` with `TEMP` and `TMP` routed
under the task's D-drive artifact root:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" -m:1 -nodeReuse:false
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review,wpf_pipeline_review_entry_perf,wpf_tool_window_dock_float_cycle D:\OpenVisionLab-TestData\OpenVisionLab\pipeline-open-f5-fix-20260813\focused-smoke-final
dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"
```

Results:

- Debug solution build: 0 warnings, 0 errors.
- Pipeline Review, entry-performance, and dock/float focused smokes: PASS;
  layout/text/internal issue counts are all zero.
- Readiness: 13/13 PASS.
- Actual F5 button: three fresh processes PASS.
- Actual direct-EXE button: PASS. The final capture was made inside the selected
  left monitor (`DISPLAY2`, bounds `-1920,365,1920x1080`, work area
  `-1920,365,1920x1032`); the captured window rectangle was
  `-1760,431,1280x720`.

Evidence:

- Runtime log:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\bin\Debug\Log\2026\08\13\2026-08-13_ALL.log`
- Current direct-EXE capture:
  `D:\OpenVisionLab-TestData\OpenVisionLab\pipeline-open-f5-fix-20260813\after\02-direct-exe-pipeline-open-display2.png`
- Focused current-source captures and reports:
  `D:\OpenVisionLab-TestData\OpenVisionLab\pipeline-open-f5-fix-20260813\focused-smoke-final`

## Boundary

This task changes and verifies Dev only. The original repository was not
modified, committed, or pushed. The earlier original direct-EXE evidence does
not prove the Visual Studio F5 path; original F5 equivalence requires an
explicit promotion and independent verification request.

```text
Status: Complete
Scope: Dev main no-image Pipeline open F5 dock-mode performance correction
Acceptance criteria: exact F5 delay reproduced and isolated -> pass; same-context F5 actual button under 100 ms in three fresh processes -> pass; direct EXE actual button and dock/float contracts preserved -> pass
Verification: Debug build 0 warnings/0 errors; three focused WPF smokes pass; readiness 13/13 pass; F5 23/19/19 ms; direct EXE 15 ms
Evidence: current runtime log, current DISPLAY2 EXE capture, and focused-smoke-final folder listed above
Boundary / next dependency: original repository was not changed; original F5 equivalence needs explicit promotion and independent verification
```
