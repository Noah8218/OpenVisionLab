# OpenVisionLab OVL-26 — Screenshot capture lifecycle owner

- **Status:** Complete for one independently verifiable temporary-window capture responsibility.
- **Scope:** `tools/PipelineViewerScreenshotSmoke/Program.cs` no longer owns the lifecycle implementation of `CaptureWindowWithContent`, `CaptureStandaloneWindow`, or `CaptureElement`. `ScreenshotCaptureLifecycle` now owns temporary `Window` creation/show/activate, floating-window selection, dispatcher-pump ordering through an explicit callback, verification/capture sequencing, disposable-content cleanup, supplied-window close, and `CaptureResult` timing.
- **Intentional boundary:** `Program` remains the composition point for the target catalog, shared `Pump`, OpenGL diagnostic implementation, visual verification policy, fixture creation, and caller-selected capture options. `ScreenshotPngWriter` remains the single PNG render/output owner. Recipe/XML, Preview/Run, Layer/ImageSpace, PropertyGrid, and product UI behavior are unchanged.
- **Dependency direction:** `Program` delegates to `ScreenshotCaptureLifecycle` and passes `Pump` plus `WriteOpenGlDiagnostics` as explicit callbacks. The lifecycle owner depends on WPF Window/Application state, `ScreenshotPngWriter`, and the tool-local `CaptureResult`; it does not reference `Program` or product modules.
- **State/data owner:** The lifecycle owner owns the temporary Window and cleanup order for each call. The caller still owns the supplied content object until the owner reaches its existing `finally` disposal point; the owner does not retain content after the call.
- **Observable contract:** Existing dimensions, initial/second pump counts, floating-window fallback, screen-versus-element capture choice, diagnostics timing, result timing, content disposal order, and close order are preserved.
- **Duplicate-work rule:** The old lifecycle bodies were found only in `Program`; all three now route through the single concrete owner. Do not recreate or re-split this owner without a newly reproduced cleanup/lifecycle defect, changed capture contract, or proven responsibility conflict. Do not split remaining fixture/reporting helpers by file size.

## Focused proof

`ScreenshotCaptureLifecycleContract` verifies:

1. The three Program wrappers delegate to the lifecycle owner.
2. The owner contains Window creation, Application window selection, disposable-content cleanup, and deterministic close operations.
3. Pump and OpenGL diagnostics cross the boundary as explicit callbacks rather than duplicated implementations.
4. The moved lifecycle body is absent from the Program wrapper region.
5. A temporary content Window invokes verification, pumps before and after verification, writes the requested PNG, sends both roots to diagnostics, disposes content, and closes the Window.
6. A standalone Window verifies, writes, and closes; direct element capture preserves its result and PNG dimensions.

Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl26-screenshot-capture-lifecycle-contract-retry-20260909\screenshot-capture-lifecycle-contract.txt` and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl26-screenshot-capture-lifecycle-release-contract-20260909\screenshot-capture-lifecycle-contract.txt`.

## Verification

- Debug build: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform='Any CPU' -m:1 -nr:false --nologo` — 0 errors, the existing `CS8600` warning at `Program.cs:10724` remains.
- Debug focused contract: `--screenshot-capture-lifecycle-contract` — `SCREENSHOT_CAPTURE_LIFECYCLE_CONTRACT=PASS|checks=10`.
- Debug OVL-25/24/23/22 regression contracts — `7/7`, `8/8`, `7/7`, and `10/10` PASS.
- Release build: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Release -p:Platform='Any CPU' -m:1 -nr:false --nologo` — 0 errors, the existing `CS8600` warning at `Program.cs:10724` remains.
- Release focused contract: `--screenshot-capture-lifecycle-contract` — `SCREENSHOT_CAPTURE_LIFECYCLE_CONTRACT=PASS|checks=10`.
- Release OVL-25/24/23/22 regression contracts — `7/7`, `8/8`, `7/7`, and `10/10` PASS.
- `tools\RefactorAudit\Invoke-RefactorAudit.ps1 -Verify -OutputDirectory D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl26-refactor-audit-20260909` — `REFACTOR_AUDIT=PASS|CSharpFiles=797|XamlFiles=59|PartialDeclarations=108|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
- `tools\TestDocumentationIndex.ps1` — `DocumentationIndex=PASS IndexedPaths=245 Routes=13 RootRedirects=102` after registering this report.
- WPF desktop theme/DPI qualification was not run; this is a smoke-tool lifecycle change without product UI source changes.

## Junior readability review

**PASS for this boundary.** The owner name describes the Window capture lifecycle, its dependencies are two visible callbacks, and the contract exercises cleanup and output with concrete objects. `Program` retains only a short composition wrapper, while rendering and diagnostics remain separate owners.

## Next slice

Inspect the remaining fixture/reporting helpers for one concrete state owner only when their state and call path are independent. Do not repeat OVL-12 through OVL-26 or split `Program.cs` by file size.
