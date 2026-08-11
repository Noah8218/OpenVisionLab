# Recipe Switch And Result Canvas Correction

Date: 2026-08-11 KST
Promotion verification: 2026-08-12 KST
Repositories: `C:\Git\OpenVisionLab_Dev` and `C:\Git\OpenVisionLab`
Source baseline: `8a2efe0f8684f8563652ab965bfd38cd928d8929`

## Scope

- Remove full Pipeline Review WPF document construction from the recipe-change
  critical path. Recipe readiness remains bound to repository state, layers,
  routes, and command-surface rebinding; Pipeline Review stays lazy and opens
  only on explicit operator demand.
- Render explicit Blob and Contour Preview detection drawings on a color copy
  of the routed source image. Keep the separate internal-threshold teaching
  Preview binary so the operator can still inspect the mask deliberately.
- Add focused image assertions that reject a Blob or Contour result which
  replaces most of the source background with a processed mask.

## Acceptance Evidence

- Actual EXE recipe switch `Default -> FieldPilot_BentPin` changed from
  `1168.2 ms` before to `577.6 ms` and `579.5 ms` after on the same selected
  monitor, a comparable reduction of about 50.5%. The first reverse switch was
  `972.3 ms` while disposing the startup-prewarmed review document; subsequent
  switches in both directions were `577.6-589.7 ms` and the process remained
  responsive.
- Actual EXE Contour Preview completed in `462 ms`, reported one detection,
  maximum area `239249`, center `286,210`, and box `572x420`. The result viewer
  retained the original shape image and overlaid the selected detection marks.
- Current-source Blob and Contour diagnostic captures retain the source image.
  The Contour threshold-teaching capture remains binary and separate from the
  explicit detection result.
- A detached D-drive build of the exact source baseline reproduced the former
  UI: the Blob result viewer and Contour result image replaced the grid/source
  background with a white/black binary mask. The current build retains the
  grid and unselected source geometry while preserving detection overlays.
- Actual EXE main and Contour windows were fully inside selected monitor
  `\\.\DISPLAY2`; monitor placement is recorded with the captures.

## Verification

- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU" --no-restore`
  - PASS, 0 warnings, 0 errors.
- `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug --no-restore`
  - PASS, 0 warnings, 0 errors.
- Focused screenshot smoke targets:
  `wpf_shell_host_blob_tool`, `wpf_shell_host_contour_tool`,
  `wpf_shell_host_recipe_change_safety`, and
  `wpf_shell_host_recipe_context_switch`
  - PASS; every target reported layout `0`, text `0`, and internal `0` failures.
- `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- C:\Git\OpenVisionLab_Dev`
  - PASS, all 13 readiness contracts.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1`
  - PASS.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1`
  - PASS, 33 catalog rows, 229 manifest assets, 17 pipelines.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestDocumentationIndex.ps1`
  - PASS, 59 indexed paths, 12 routes, 101 root redirects.
- `git diff --check`
  - PASS.
- Original repository independent verification on 2026-08-12:
  - Debug solution and ScreenshotSmoke builds: PASS, 0 warnings, 0 errors.
  - The same four focused screenshot smoke targets: PASS with layout `0`,
    text `0`, and internal `0` failures.
  - Readiness 13/13, external references, public assets, and documentation
    index: PASS.
  - All six promoted Git object hashes: equal to Dev.

## Evidence

- Root:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\artifacts\recipe_contour_review_20260811`
- Before actual-EXE timing:
  `before_actual_exe\before_timing.json`
- Reproduced source-baseline UI:
  `focused_before\wpf_shell_host_blob_tool.png` and
  `focused_before\wpf_shell_host_contour_tool.diagnostics\contour-tool-draw-result.png`
- After actual-EXE timing:
  `after_actual_exe\after_timing.json`
- Actual-EXE Contour result:
  `after_actual_exe\after_contour_original_overlay_actual_exe.png`
- Current-source focused Blob/Contour images:
  `focused_after\wpf_shell_host_blob_tool.diagnostics\blob-tool-draw-result.png`
  and
  `focused_after\wpf_shell_host_contour_tool.diagnostics\contour-tool-draw-result.png`

## Boundary

- This correction changes native Tool View explicit Preview presentation and
  recipe-switch preparation only. It does not change Pipeline execution,
  detection parameters, acceptance gates, output-layer routing, or the
  threshold-teaching Preview contract.
- Work is complete and independently verified in Dev and original. The
  unrelated original-only untracked `Temp.txt` is outside scope and unchanged.

Status: Complete
