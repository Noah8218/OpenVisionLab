# OpenVisionLab OVL-40 Direct Smoke Screenshot PNG owner

Status: Complete for one independently verifiable Direct Smoke screenshot
output boundary. The wider refactoring program remains active.

## Scope

`OpenVisionLabDirectSmokeRunner` had three local PNG implementations for a
DPI-aware WPF window render, a single-window screen copy, and a visible-window
union screen copy. The existing `ScreenshotPngWriter` is now the canonical
output owner for all three operations:
`WriteDpiAwareWindowPng`, `WriteWindowScreenPng`, and
`WriteWindowsScreenPng`.

The Direct runner keeps its existing private method names and scenario call
sites. Those wrappers retain window activation, layout update, visibility
filtering, and dispatcher-pump timing, then delegate pixel geometry and PNG
encoding to `ScreenshotPngWriter`. The OpenVisionLab app links the existing
writer only when `OpenVisionLabEnableEmbeddedSmokeRunner` is enabled.

Recipe/XML contracts, explicit Preview/Run behavior, Layer/ImageSpace state,
product UI behavior, and the existing screenshot file names are unchanged.

## Refactor proof

### Before

- `OpenVisionLabDirectSmokeRunner.SaveWindowScreenshot` owned DPI transform,
  `RenderTargetBitmap`, and PNG encoding.
- `SaveWindowScreenScreenshot` owned screen coordinates, `CopyFromScreen`, and
  PNG encoding.
- `SaveWindowsScreenScreenshot` owned visible-window filtering, union bounds,
  `CopyFromScreen`, and PNG encoding.
- `ScreenshotPngWriter` owned the equivalent Pipeline Smoke output contract,
  but the embedded Direct runner could not consume it.

### After

- `ScreenshotPngWriter` owns DPI-aware window rendering, single-window screen
  capture, multi-window union geometry, and PNG output.
- Direct wrappers keep orchestration that is specific to the scenario:
  `BringWindowToFront`, `UpdateLayout`, `Pump`, and visible-window validation.
- `OpenVisionLabDirectSmokeRunner` no longer contains `RenderTargetBitmap`,
  `PngBitmapEncoder`, `CopyFromScreen`, or `Rect.Union` inside those wrappers.
- The existing OVL-25 owner remains canonical. Its new Direct caller is a
  changed dependency boundary, so the owner was extended rather than creating
  a second screenshot writer.

## Responsibility and call path

- Window render: Direct scenario -> `SaveWindowScreenshot` wrapper ->
  `BringWindowToFront`/`Pump` -> `ScreenshotPngWriter.WriteDpiAwareWindowPng`.
- Single screen capture: Direct scenario -> `SaveWindowScreenScreenshot` ->
  `BringWindowToFront`/`Pump` -> `ScreenshotPngWriter.WriteWindowScreenPng`.
- Union screen capture: Direct scenario -> visible-window filter and activation
  -> `Pump(24)` -> `ScreenshotPngWriter.WriteWindowsScreenPng`.
- The writer keeps no mutable state. It receives the window sequence and output
  path for one invocation; Direct retains window lifetime and scenario timing.

## Junior developer reading order

1. `tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs` —
   existing scenario names and the three short compatibility wrappers.
2. `tools/PipelineViewerScreenshotSmoke/ScreenshotPngWriter.cs` — DPI-aware,
   single-screen, multi-window, and element PNG output owner.
3. `tools/PipelineViewerScreenshotSmoke/DirectSmokeScreenshotWriterContract.cs` —
   delegation, old-code removal, conditional app link, and WPF render proof.
4. `src/OpenVisionLab/OpenVisionLab.csproj` — embedded link condition.
5. `docs/admin/CODEBASE_STRUCTURE.md` section 9.32 — owner map and boundary
   lock.

## Verification evidence

All generated output was written under `D:\OpenVisionLab-TestData`.

- Pipeline Smoke x64 Debug/Release builds — 0 errors; the existing nullable
  `CS8600` warning remains in `Program.cs` only.
- Embedded OpenVisionLab x64 Debug/Release builds with
  `OpenVisionLabEnableEmbeddedSmokeRunner=true` — 0 warnings, 0 errors.
- Direct screenshot writer contract — `DIRECT_SMOKE_SCREENSHOT_WRITER_CONTRACT=PASS`,
  6/6 in Debug and Release. Actual Dev reports:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl40-actual-validation-20260909\direct-smoke-screenshot-writer-contract-debug\direct-smoke-screenshot-writer-contract.txt` and
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl40-actual-validation-20260909\direct-smoke-screenshot-writer-contract-release\direct-smoke-screenshot-writer-contract.txt`.
- Existing PNG writer contract — `SCREENSHOT_PNG_WRITER_CONTRACT=PASS`, 7/7
  in Debug and Release. Actual Dev evidence is under
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl40-actual-validation-20260909\screenshot-png-writer-contract-debug` and
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl40-actual-validation-20260909\screenshot-png-writer-contract-release`.
- Existing capture lifecycle contract —
  `SCREENSHOT_CAPTURE_LIFECYCLE_CONTRACT=PASS`, 10/10 in Debug and Release.
  Actual Dev evidence is under
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl40-actual-validation-20260909\screenshot-capture-lifecycle-contract-debug` and
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl40-actual-validation-20260909\screenshot-capture-lifecycle-contract-release`.
- OVL-22 command-line contract — `SMOKE_TARGET_RUNNER_CONTRACT=PASS`, 10/10
  in Debug and Release. Actual Dev evidence is under
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl40-actual-validation-20260909\command-line-contract-debug` and
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl40-actual-validation-20260909\command-line-contract-release`.
- `Invoke-RefactorAudit.ps1 -Verify` —
  `REFACTOR_AUDIT=PASS|CSharpFiles=820|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
  Actual Dev log: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl40-actual-validation-20260909\refactor-audit-verify.txt`.
- Actual Dev `TestDocumentationIndex.ps1` —
  `DocumentationIndex=PASS IndexedPaths=259 Routes=13 RootRedirects=102`;
  `docs/LLM_DOCUMENT_INDEX.json` parse and scoped `git diff --check` — passed.
  Index log: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl40-actual-validation-20260909\documentation-index.txt`.

The isolated worktree needed one initial NuGet restore because its generated
assets were absent; the subsequent builds and contracts used `--no-restore`.

## Runtime boundary and remaining work

The contract exercises a real WPF window for DPI-aware element rendering. It
does not claim fresh physical desktop monitor, theme, topology, or DPI-matrix
evidence for the Direct screen-copy paths. Existing Direct scenario runtime
evidence remains the baseline.

Junior developer self-assessment: **PASS for this boundary**. Screenshot output
now has one searchable owner, while scenario-specific activation and pump
ordering remain visible at the Direct call site. Another model, agent, or
scheduled run must not recreate, rename, re-split, or move this owner without a
new output defect, changed Direct capture contract, or demonstrated dependency
conflict.

Next priority: perform a fresh residual Direct Smoke responsibility audit and
select one owner only when its call path and mutable-state boundary are proven;
completed OVL-01/02/03/04/05/06a/07/08/10/12-40 owners stay closed.
Recommended model: `gpt-6-astra`; reasoning effort: `high`.
