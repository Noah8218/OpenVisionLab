# OpenVisionLab OVL-24 — Screenshot bitmap assertion owner

- **Status:** Complete for one independently verifiable smoke evidence responsibility.
- **Scope:** `tools/PipelineViewerScreenshotSmoke/Program.cs` no longer owns bitmap presence, diagnostic save, visual-difference, preview-overlay, source-background, binary-like, grayscale, or expected-color assertions. `ScreenshotBitmapAssertions` now owns all eight methods and their thresholds/error messages.
- **Intentional boundary:** WPF window lifecycle, RenderTargetBitmap/CopyFromScreen capture, OpenGL diagnostics, and fixture bitmap creation remain in `Program`. The extracted owner receives only `System.Drawing.Bitmap`/`Color`, names, and output paths.
- **Dependency direction:** `Program` -> `ScreenshotBitmapAssertions`; the owner has no WPF, OpenVision application, Recipe, Layer, or PropertyGrid dependency and is exercised before any WPF application is created.
- **Observable contract:** Existing method names, sampling stride, thresholds, diagnostic directory naming, PNG format, and exception messages are preserved. Callers still decide when to capture and which image represents a Preview/Run result.
- **Duplicate-work rule:** A repository-wide search found these assertion/save methods only in `PipelineViewerScreenshotSmoke/Program.cs`. Do not recreate or further split this owner unless a new image-evidence defect, changed threshold contract, or proven responsibility conflict appears. Remaining WPF capture helpers are a separate boundary and must not be split by file size alone.

## Focused proof

`ScreenshotBitmapAssertionsContract` verifies:

1. Every moved operation is called through the new owner.
2. `Program` no longer declares the moved methods.
3. The owner is WPF-free and retains PNG/pixel inspection behavior.
4. Invalid and identical images fail with context.
5. Changed, overlay, source-background, binary, grayscale, and color cases preserve their existing acceptance rules.
6. Diagnostic PNG output remains under the requested evidence directory.

Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl24-screenshot-bitmap-assertions-contract-20260909\screenshot-bitmap-assertions-contract.txt` and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl24-screenshot-bitmap-assertions-release-contract-20260909\screenshot-bitmap-assertions-contract.txt`.

## Verification

- Debug build: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform='Any CPU' -m:1 -nr:false --nologo` — 0 errors, 1 existing `CS8600` warning at `Program.cs:10712`.
- Debug focused contract: `--screenshot-bitmap-assertions-contract` — `SCREENSHOT_BITMAP_ASSERTIONS_CONTRACT=PASS|checks=8`.
- Release build: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Release -p:Platform='Any CPU' -m:1 -nr:false --nologo` — 0 errors, 1 existing `CS8600` warning at `Program.cs:10712`.
- Release focused contract: `--screenshot-bitmap-assertions-contract` — `SCREENSHOT_BITMAP_ASSERTIONS_CONTRACT=PASS|checks=8`.
- OVL-23 Learn documentation contract rerun in Debug/Release — `LEARN_DOCUMENT_COPY_POLICY_CONTRACT=PASS|checks=7`.
- OVL-22 target-runner contract rerun in Debug/Release — `SMOKE_TARGET_RUNNER_CONTRACT=PASS|checks=10`.
- `Invoke-RefactorAudit.ps1 -Verify -OutputDirectory D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl24-refactor-audit-20260909` — `REFACTOR_AUDIT=PASS|CSharpFiles=793|XamlFiles=59|PartialDeclarations=108|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
- `TestDocumentationIndex.ps1` — `DocumentationIndex=PASS IndexedPaths=243 Routes=13 RootRedirects=102`.

## Junior readability review

**PASS for this boundary.** The owner name describes the evidence responsibility, its inputs are ordinary bitmap/color values, and the contract demonstrates the acceptance and rejection examples without opening a desktop window. Capture lifecycle and fixture creation remain visible at their existing call sites.

## Next slice

Inspect the remaining WPF capture lifecycle helpers (`CaptureWindowWithContent`, `CaptureStandaloneWindow`, `CaptureElement`, and PNG writers) for one concrete owner only if its state and cleanup boundary can be passed explicitly. Do not repeat OVL-12 through OVL-24 or split by line count.