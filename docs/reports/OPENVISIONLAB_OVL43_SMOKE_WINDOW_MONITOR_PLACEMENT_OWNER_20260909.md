# OpenVisionLab OVL-43 Smoke window monitor placement owner

Status: Complete for one independently verifiable Direct Smoke monitor
placement boundary. The wider refactoring program remains active.

## Scope

`OpenVisionLabDirectSmokeRunner` had a Win32 monitor placement implementation
inside its scenario class. Twenty Direct Smoke call sites selected the
leftmost monitor, centered the EXE window in its work area, moved it with
`SetWindowPos`, and verified that the resulting window intersected the selected
monitor. This was a separate environment adapter from the already-closed PNG,
clipboard, task-wait, and docking owners.

The implementation is now `SmokeWindowMonitorPlacement`. The Direct runner
keeps its existing wrapper and scenario call sites, error text, returned
monitor evidence, Window lifetime, and Dispatcher ownership. No Recipe/XML,
Preview/Run, layer routing, or product behavior changed.

## Refactor proof

### Before

- `OpenVisionLabDirectSmokeRunner` declared the monitor Win32 imports,
  callback, native rectangles, monitor-info structure, selection order,
  centered-position arithmetic, `SetWindowPos`, and intersection check.
- The scenario class therefore owned both inspection workflows and the
  monitor/environment adapter.

### After

- `SmokeWindowMonitorPlacement.PlaceOnLeftmostMonitor(Window)` owns the
  monitor enumeration, leftmost/top tie-break, native window rectangle read,
  centered placement, post-move rectangle read, intersection guard, and exact
  evidence/error text.
- `CalculateCenteredPosition` and `Intersects` are deterministic owner helpers
  used by the contract without launching a desktop EXE.
- The Direct wrapper delegates to the owner; its 20 existing callers and
  caller-owned Window/Dispatcher lifetime remain unchanged.
- The owner is conditionally linked into the application only when
  `OpenVisionLabEnableEmbeddedSmokeRunner=true`.

## Responsibility and call path

- Current owner before this slice: `OpenVisionLabDirectSmokeRunner`.
- Intended/current owner: `SmokeWindowMonitorPlacement`.
- Direct path: scenario -> `PlaceWindowOnLeftmostMonitor` wrapper ->
  `SmokeWindowMonitorPlacement.PlaceOnLeftmostMonitor` -> Win32 monitor/window
  adapter -> returned monitor evidence.
- Mutable state: the per-call monitor list and native rectangles are local to
  the owner; no static mutable state is retained.
- Window creation, visibility, close order, Dispatcher affinity, and scenario
  timing remain caller-owned. The owner only moves an already-created Window.
- Existing public/observable contract: Direct scenario names, wrapper name,
  error messages, evidence format, and the `Window` argument are preserved.
- Dependency direction: the owner depends only on WPF `Window`/interop and
  Win32 user32 APIs; it has no Shell, Recipe, file-system, SDK, or product
  dependency.

## Developer reading order

1. Search `PlaceWindowOnLeftmostMonitor` in
   `tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs`
   to see the unchanged scenario wrapper and its callers.
2. Read `tools/PipelineViewerScreenshotSmoke/SmokeWindowMonitorPlacement.cs`
   for monitor selection, placement, geometry, and failure contracts.
3. Read `tools/PipelineViewerScreenshotSmoke/SmokeWindowMonitorPlacementContract.cs`
   for the structural and deterministic geometry checks.
4. Read the conditional Link in `src/OpenVisionLab/OpenVisionLab.csproj` when
   the embedded Direct runner composition is relevant.
5. Use this report and `docs/admin/CODEBASE_STRUCTURE.md` section 9.35 for the
   closed owner boundary and verification evidence.

## Verification evidence

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl43-smoke-window-monitor-placement-contract-debug-20260909-run1`.

Release evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl43-smoke-window-monitor-placement-contract-release-20260909-run1`.

- PipelineViewerScreenshotSmoke x64 Debug build: 0 errors; the existing
  nullable `CS8600` warning at `Program.cs:10175` remains.
- PipelineViewerScreenshotSmoke x64 Release build: 0 errors; the same
  pre-existing nullable `CS8600` warning remains.
- `SMOKE_WINDOW_MONITOR_PLACEMENT_CONTRACT=PASS` with 9/9 checks in both Debug
  and Release. The contract
  covers Direct delegation and old Win32-code removal, conditional composition,
  owner dependency isolation, normal/negative/oversized geometry,
  intersection, and evidence formatting.
- Embedded OpenVisionLab x64 Debug and Release builds with
  `OpenVisionLabEnableEmbeddedSmokeRunner=true`: 0 warnings, 0 errors.
- `Invoke-RefactorAudit.ps1 -Verify`:
  `REFACTOR_AUDIT=PASS|CSharpFiles=826|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
- `TestDocumentationIndex.ps1`:
  `DocumentationIndex=PASS IndexedPaths=262 Routes=13 RootRedirects=102`.
- JSON parse and scoped `git diff --check` passed; no staged or unmerged paths
  were present at close-out.
- No physical EXE monitor placement was launched in this source/contract
  slice; monitor topology, DPI, theme, input, and desktop rendering remain
  runtime-unverified.

## Boundary lock

This owner is complete for the current monitor placement contract. Another
model, agent, or automation must not recreate, rename, re-split, or move this
owner without a new monitor-placement defect, changed monitor-selection or
window-placement contract, or demonstrated dependency conflict. File length,
new model preference, or a request to continue refactoring is not sufficient.

Next priority: fresh residual Direct Smoke responsibility audit, then return to
the recorded Shell/Recipe roadmap only when a single call path and mutable-state
boundary is proven. Recommended model: `gpt-6-astra`; reasoning effort:
`high`.
