# OpenVisionLab OVL-12 Image Compare Resource Owner

Updated: 2026-09-08 KST
Status: Complete for the single Image Compare file/Bitmap lifetime boundary.

## Scope

This slice moves decoded image resource ownership out of
`ImageCompareSlotViewModel` while preserving the existing public slot surface,
`ImageCompareWindow.LoadImages` flow, Recipe/XML contracts, and Preview/Run
behavior. It does not change Image Compare layout, zoom policy, pixel lookup,
or the remaining Recipe/Shell boundaries.

## Structural change

Before this slice, `ImageCompareSlotViewModel` owned binding state, file
decoding, `System.Drawing.Bitmap` creation/disposal, WPF `BitmapSource`
creation, and PNG/BMP format parsing in one type.

The new `ImageCompareImageResource` is the concrete resource owner. It creates
the `Bitmap`, creates an `OnLoad` and frozen `BitmapSource`, resolves the
format text, and disposes the native bitmap idempotently. The slot now owns
binding state and projects the resource properties through the unchanged
`Bitmap`, `Source`, `Width`, `Height`, `IsLoaded`, `Load`, and `Dispose` members.
Replacing a slot first disposes the previous resource; a failed or missing
path leaves the slot empty as before.

Call path:

```text
ImageCompareViewModel.LoadImages
  -> ImageCompareSlotViewModel.Load
  -> ImageCompareImageResource.Load
  -> slot binding projection
```

The former decoding and metadata helper methods no longer exist in the slot
ViewModel. Static ownership search confirms they are present only in
`ImageCompareImageResource`.

## Changed files

- `src/OpenVisionLab/UI/Popup/Wpf/ImageCompare/ImageCompareImageResource.cs`
- `src/OpenVisionLab/UI/Popup/Wpf/ViewModels/ImageCompareViewModel.cs`
- `tools/VisionRecipeRunnerSmoke/ImageCompareResourceContract.cs`
- `tools/VisionRecipeRunnerSmoke/Program.cs`

## Verification evidence

All generated test data and logs are on D::
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\image-compare-refactor-20260908`.

- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Release -p:Platform=x64 -p:WpgCustomBuildEnabled=false -p:UseAppHost=false -m:1 -nr:false --no-restore` — passed, 0 errors and 0 warnings. Log: `build-release.log`.
- `dotnet tools\VisionRecipeRunnerSmoke\bin\x64\Release\net8.0-windows7.0\VisionRecipeRunnerSmoke.dll --image-compare-resource-contract <evidence>` — passed. The contract covers dimensions, frozen WPF source, PNG/BMP metadata, replacement disposal, invalid-path reset, and repeated disposal. Evidence: `image_compare_resource_contract.txt`.
- `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Release -p:Platform=x64 -p:WpgCustomBuildEnabled=false -p:UseAppHost=false -m:1 -nr:false --no-restore` — passed with 0 errors and one existing nullable warning at `Program.cs:10792`. Log: `pipeline-viewer-build-release.log`.
- Existing `wpf_image_compare` target — passed: `wpf_image_compare=OK`, `1280x760`, `elapsed=392ms`. Evidence: `runtime-image-compare/pipeline-viewer.stdout.log` and `wpf_image_compare.png`.
- Dynamic monitor verification — passed. Windows reported two independent monitors; the smaller left monitor (`DISPLAY2`, work area `-1920,365–0,1397`) was selected. The Image Compare window rectangle was `-1920,365–-640,1125` and intersected the selected monitor. Evidence: `runtime-image-compare/monitor-topology.txt`, `monitor-run-summary.txt`, and `window-placement.txt`.
- `Invoke-RefactorAudit.ps1 -Verify` — passed after the change: `CSharpFiles=775`, `XamlFiles=58`, `PartialDeclarations=106`, `ViewModelUiIoFiles=1`, `ProjectCycles=0`, `ShellStorageCalls=0`. Evidence: `module-audit`.
- `rg` ownership search — passed: old decode/parser methods are absent from the slot ViewModel and resolve only in the resource owner.
- `git diff --check` — exit 0. Existing line-ending normalization warnings were reported by Git; no whitespace error was returned.

The screenshot confirms the two loaded images, synchronized display, and pixel
status remain visible. Full alternate theme and 125/150/175/200% DPI matrices
were not run for this source/resource-only change; those environment-bound
rows remain outside this slice.

## Junior readability assessment

Pass for this boundary. A new contributor can follow one explicit owner:
`ImageCompareImageResource` owns decoded resources and disposal, while
`ImageCompareSlotViewModel` owns binding state and delegates loading. The
public facade and the existing window call path remain unchanged.

## Completion record

```text
Status: Complete
Scope: Image Compare decoded image and Bitmap/BitmapSource lifetime owner
Acceptance criteria:
  - Concrete resource owner exists and is used: PASS (source search and build)
  - Previous resource is disposed before replacement: PASS (resource contract)
  - Invalid path and repeated disposal remain safe: PASS (resource contract)
  - Existing Image Compare UI contract remains runnable: PASS (WPF smoke)
  - Recipe/XML and Preview/Run contracts changed: PASS (no changes in those paths)
Verification: focused Release builds, resource contract, WPF smoke, monitor placement, audit, static ownership search, diff check
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\image-compare-refactor-20260908
Boundary / next dependency: Recipe execution session ownership is the next independent slice; do not repeat or re-split this completed owner without a newly reproduced defect, changed contract, or proven responsibility conflict.
```

## Next priority

Recipe execution session ownership around
`OpenVisionRecipeExecutionSessionViewModel` and its existing controller/service
path. Preserve explicit Preview/Run semantics, cancellation, stale-result
guards, and Recipe/XML compatibility. One independently verifiable slice only.

Recommended model: `gpt-5.6-terra`
Reasoning effort: `high`
