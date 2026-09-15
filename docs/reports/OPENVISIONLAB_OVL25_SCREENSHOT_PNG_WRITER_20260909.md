# OpenVisionLab OVL-25 — Screenshot PNG writer owner

- **Status:** Complete for one independently verifiable WPF screenshot output responsibility.
- **Scope:** `tools/PipelineViewerScreenshotSmoke/Program.cs` no longer owns the three PNG output operations. `ScreenshotPngWriter` now owns `WriteScreenPng`, `WriteElementPng`, and `WriteVisibleElementPng`, including WPF render/readback, screen copy, encoder, directory creation, and PNG stream writing.
- **Intentional boundary:** Window creation/show/activate/close, floating-tool selection, dispatcher pumping, verification callbacks, OpenGL diagnostics, fixture bitmap creation, and capture timing remain in `Program`. `CaptureWindowWithContent`, `CaptureStandaloneWindow`, and `CaptureElement` remain the lifecycle/orchestration boundary for a later independently scoped slice.
- **Dependency direction:** `Program` and the existing Learn smoke modules call `ScreenshotPngWriter`; the writer depends only on WPF rendering, System.Drawing screen capture, and file output. It does not reference `Program`, OpenVision application types, Recipe/XML, Layer/ImageSpace, or PropertyGrid.
- **Observable contract:** Existing PNG dimensions, `RenderTargetBitmap` format, 96 DPI, screen-coordinate calculation, diagnostic directory behavior, and caller-selected capture timing are preserved. Recipe/XML, Preview/Run, Layer/ImageSpace, PropertyGrid, and user-facing UI behavior are unchanged.
- **Duplicate-work rule:** The repository search found the three moved PNG writer methods only in `PipelineViewerScreenshotSmoke/Program.cs`; all call sites now target the single writer. Do not recreate or re-split this owner without a newly reproduced capture-format defect, changed rendering contract, or proven responsibility conflict. The remaining lifecycle and fixture groups are separate and must not be split by file size alone.

## Focused proof

`ScreenshotPngWriterContract` verifies:

1. Every screen, arranged-element, and visible-element PNG call uses the new owner.
2. `Program` no longer declares the moved writer methods.
3. The owner contains the WPF render, screen-copy, PNG encoder, and PNG-format operations.
4. The owner has no dependency on the smoke entry point or product modules.
5. Real WPF `FrameworkElement` renders produce the requested `48x32` PNG dimensions for both element paths.
6. The screen-capture path retains `PointToScreen` and `CopyFromScreen` ownership.

Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl25-screenshot-png-writer-contract-20260909\screenshot-png-writer-contract.txt` and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl25-screenshot-png-writer-release-contract-20260909\screenshot-png-writer-contract.txt`.

## Verification

- Debug build: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform='Any CPU' -m:1 -nr:false --nologo` — 0 errors, the existing `CS8600` warning at `Program.cs:10718` remains.
- Debug focused contract: `--screenshot-png-writer-contract` — `SCREENSHOT_PNG_WRITER_CONTRACT=PASS|checks=7`.
- OVL-24 bitmap assertion contract rerun in Debug — `SCREENSHOT_BITMAP_ASSERTIONS_CONTRACT=PASS|checks=8`.
- OVL-23 Learn documentation contract rerun in Debug — `LEARN_DOCUMENT_COPY_POLICY_CONTRACT=PASS|checks=7`.
- OVL-22 target-runner contract rerun in Debug — `SMOKE_TARGET_RUNNER_CONTRACT=PASS|checks=10`.
- Release build: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Release -p:Platform='Any CPU' -m:1 -nr:false --nologo` — 0 errors, the existing `CS8600` warning at `Program.cs:10718` remains.
- Release focused contract: `--screenshot-png-writer-contract` — `SCREENSHOT_PNG_WRITER_CONTRACT=PASS|checks=7`.
- OVL-24/OVL-23/OVL-22 regression contracts rerun in Release — `8/8`, `7/7`, and `10/10` PASS respectively.
- `tools\RefactorAudit\Invoke-RefactorAudit.ps1 -Verify -OutputDirectory D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl25-refactor-audit-20260909` — `REFACTOR_AUDIT=PASS|CSharpFiles=795|XamlFiles=59|PartialDeclarations=108|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
- `tools\TestDocumentationIndex.ps1` — `DocumentationIndex=PASS IndexedPaths=244 Routes=13 RootRedirects=102`.
- WPF desktop runtime matrix was not run; this slice changes only the smoke utility's rendering owner and has no product UI source change.

## Junior readability review

**PASS for this boundary.** The owner name describes PNG rendering/output, the three methods have direct WPF/file inputs, and the contract exercises both element paths with concrete dimensions. Window lifecycle and verification policy remain visible in `Program`, so a reader can distinguish rendering from orchestration without tracing a wrapper chain.

## Next slice

Inspect `CaptureWindowWithContent`, `CaptureStandaloneWindow`, and `CaptureElement` for one explicit window state/cleanup owner. Extract only if the temporary-window, floating-window, dispatcher, content-disposal, and close-order dependencies can be passed explicitly; otherwise record an audit and leave the lifecycle intact. Do not repeat OVL-12 through OVL-25 or split by line count.
