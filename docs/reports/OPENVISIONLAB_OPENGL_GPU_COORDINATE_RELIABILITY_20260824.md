# OpenVisionLab OpenGL/GPU/Viewer Coordinate Reliability Completion

Updated: 2026-08-24 KST  
State: Complete

## Scope

Dev-only cleanup and verification for the existing SharpGL/ImageCanvas path:
OpenGL exceptional resource retirement, per-process GPU allocation plateau,
and native Viewer pixel/region coordinate edges. No renderer replacement, GPU
framework, algorithm, concurrency, external-repository, release, or deployment
work was performed.

Source identity was the dirty Dev worktree at `HEAD 827a22e9` with unrelated
pre-existing changes preserved. The focused source changes are in
`OpenGlRenderer`, `ImageCanvasControl`, `ImageCanvasControl.ViewState`, the
OpenGL overlay allocation paths, and the focused smoke project.

## Acceptance criteria

- AC1: Pass. The CP1 owner/failure matrix names creator, owner, success/failure
  cleanup, context requirement, and reproducer for Texture, FBO, RBO, PBO,
  display list, Bitmap/BitmapData lock, binding, render context, and timer:
  `D:\OpenVisionLab-TestData\OpenVisionLab\opengl-gpu-coordinate-20260824-114354\focused\cp1-resource-owner-failure-matrix.md`.
- AC2: Pass for the admitted native failure paths. FBO/RBO/PBO/Bitmap lock and
  texture cleanup now use allocation IDs/flags and local `try/finally` paths;
  cleanup warnings do not replace the primary exception. The smoke forces the
  render callback exception, preserves its message, and renders successfully
  afterward in the same Viewer context. The 4512 lifetime path also passed
  repeat close/delete/dispose cycles.
- AC3: Pass. The latest current-DLL gate recorded PID `34648`, NVIDIA GeForce
  GTX 1060 3GB, driver `32.0.15.8228` / `nvidia-smi 582.28`, 14 valid late
  samples, dedicated late range `19.0 MB`, shared late range `0.0 MB`, and
  both plateau decisions below the predeclared `155.3 MB` delta ceiling.
- AC4: Pass. Native FBO/PBO/readback matched all 16 fixture pixels, four
  corners, last row/column, 1x1, full, edge, interior, and clamped regions;
  negative, `x == width`, and `y == height` inputs were rejected. Half-open
  `[1,3) x [1,3)` restore changed exactly four pixels and no neighbor.
- AC5: Pass in the current 4512 lifetime and full focused suite: active Main
  layer, live viewer count, explicit no-auto-run path, and route/layer evidence
  remained stable; no Preview/Run or Pipeline-routing behavior was added.
- AC6: Pass. The current-source ten-target focused suite and the required
  build, HistoryContract, readiness, external-reference, public-sample, and
  documentation-index gates passed.
- AC7: Pass. Current artifacts retain source raw/PNG fixtures and SHA-256,
  native bitmap/readback output, coordinate report, GPU CSV/report/adapter
  identity, smoke output, and command logs under the D-drive artifact root.
  The final native fixture hashes are raw RGBA
  `DCBE51BC3EA5BA42F48D57FCFB5204F0C801BF402E02E748E53AF7EE95426AEF` and PNG
  `4D24164C2586938CD57AF2F2BCB506ED5892360641F8C8B03072702F8359B3C1`.

## Verification

Focused and repository commands actually run:

```text
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" -> 0 warnings, 0 errors
dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform="Any CPU" -> 0 warnings, 0 errors
tools\RunUiPrecheck.ps1 -Targets lifetime,native-readback,reliability,workspace/layer/large-image/owned-Mat/template -> all 10 OK
dotnet run --project tools\HistoryContractCheck\HistoryContractCheck.csproj -c Debug -> HistoryContract=OK
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev" -> passed
tools\TestExternalReferences.ps1 -> passed
tools\TestPublicSampleAssets.ps1 -> PASS (33 catalog rows, 229 assets, 17 pipelines)
tools\TestDocumentationIndex.ps1 -> PASS (64 indexed paths, 12 routes, 101 redirects)
tools\RunOpenGlGpuAllocationGate.ps1 -> Result=PASS; dedicated/shared plateau PASS
```

## Evidence

Artifact root:
`D:\OpenVisionLab-TestData\OpenVisionLab\opengl-gpu-coordinate-20260824-114354`

- Native coordinate report and source identity:
  `coordinates\native-readback-final\wpf_opengl_native_readback.coordinate.txt`,
  `native_readback_source.rgba`, `native_readback_source.png`, and
  `native_readback_bitmap.png`.
- Current full focused-suite summary:
  `focused\final-full-suite-current\ui_precheck_summary.json` and
  `focused\final-full-suite-current\ui_precheck_report.md`.
- Current GPU report, CSV, adapter identity, and smoke output:
  `gpu\formal-4512-lifetime-final-source\gpu-process-memory.txt`,
  `gpu-process-memory.csv`, `gpu-adapter.txt`, and `gpu-smoke.stdout.txt`.
- Current production source and smoke build logs:
  `focused\final-solution-build-after-docs.txt`,
  `focused\build-final-fixture.txt`.

## Boundary / next dependency

The GPU counter collector retained one invalid Windows performance-counter
sample message; 46 total samples and 14 valid late samples remained available,
and both plateau checks passed. The WPF `RenderTargetBitmap` host capture may
show hosted OpenGL as dark/blank, so the native bitmap and native readback are
the pixel proof. No actual `OpenVisionLab.exe` desktop launch was performed;
the dynamic monitor rule is therefore not claimed as exercised. This slice
does not prove every driver, monitor/DPI/theme, multi-PC, arbitrary-duration,
field, camera/PLC/I/O, original-repository, release, or deployment boundary.
An older composite smoke ordering with `reliability` before `lifetime` still
reproduced a lifetime `NullReferenceException`; the final current-source suite
uses lifetime-first ordering and all ten targets pass. This is recorded as a
smoke-order precondition, not as an all-order guarantee.
The remaining project priority is P256's bounded four-Step route-clarity
walkthrough; CVR-00 remains deferred pending three independent first-time
participants.
