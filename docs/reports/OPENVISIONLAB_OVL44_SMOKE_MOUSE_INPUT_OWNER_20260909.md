# OpenVisionLab OVL-44 Smoke mouse input owner

Status: Complete for one independently verifiable Direct Smoke mouse-input
boundary. The wider refactoring program remains active.

## Scope

`OpenVisionLabDirectSmokeRunner` mixed WPF docking assertions with low-level
Win32 cursor/button injection. The class declared `SetCursorPos` and
`mouse_event`, repeated left-button transitions, converted screen coordinates,
and owned two background drag-thread wait loops.

The low-level input responsibility is now owned by `SmokeMouseInput`. Direct
scenario methods still calculate WPF element coordinates, pump the Dispatcher,
assert docking state, and choose the gesture scenario. Existing drag timings,
thread names, 8-second timeout, exception messages, button flags, and screen
pixel rounding are preserved. Recipe/XML, Preview/Run, Layer routing, and
product UI behavior are unchanged.

## Refactor proof

### Before

- `OpenVisionLabDirectSmokeRunner` declared the cursor and mouse-event Win32
  imports and constants.
- It implemented left-button release/press calls inline in docking scenarios.
- It owned coordinate rounding, cursor failure conversion, drag interpolation,
  background thread creation, Dispatcher pumping, timeout detection, joining,
  and drag exception translation.

### After

- `SmokeMouseInput` owns Win32 cursor/button injection, screen-pixel rounding,
  drag interpolation, and the background input-thread lifetime.
- `OpenVisionLabDirectSmokeRunner` calls the concrete owner for every low-level
  mouse operation. It retains WPF coordinate discovery, `Pump` callbacks,
  visual-state assertions, scenario order, and the existing click/drag timing
  around those assertions.
- The owner is conditionally linked into the application only when
  `OpenVisionLabEnableEmbeddedSmokeRunner=true`.
- No interface, factory, wrapper chain, or duplicate mouse owner was added.

## Responsibility and call path

- Current owner before this slice: `OpenVisionLabDirectSmokeRunner`.
- Intended/current owner: `SmokeMouseInput`.
- Drag path: Direct docking scenario -> WPF element/target coordinate lookup ->
  `SmokeMouseInput.DragViaPointOnBackgroundThread(..., () => Pump(1))` ->
  owner-created input thread -> `SetCursorPos`/`mouse_event` -> joined result.
- Click path: Direct docking scenario -> `SmokeMouseInput.ReleaseLeftButton` /
  `PressLeftButton` and `SetCursorPosOrThrow` -> existing WPF state assertion.
- Mutable state: WPF controls, docking documents, layer selection, and route
  state remain Direct-owned. The owner retains only per-call thread,
  exception, and coordinate locals; it has no static mutable state.
- Lifetime: `SmokeMouseInput` creates each background thread, pumps through the
  caller callback while it is alive, detects the existing 8-second deadline,
  calls `Join`, and translates input failures. Window creation, Dispatcher
  affinity, and scenario cleanup remain caller-owned.
- Existing public/observable contract: Direct scenario names, gesture points,
  mouse flags, timing constants, thread names, timeout/error messages, and
  `SetCursorPos` failure text are preserved.
- Dependency direction: the owner depends on WPF `Point`, user32, and base
  threading/exception types. It does not depend on Shell, Recipe, SDK, file
  storage, or product modules. Direct remains the only composition caller.

## Developer reading order

1. Search `SmokeMouseInput.` in
   `tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs`
   to see the existing docking scenarios and their WPF state/pump ownership.
2. Read `tools/PipelineViewerScreenshotSmoke/SmokeMouseInput.cs` for the
   concrete Win32 input and thread lifetime owner.
3. Read `tools/PipelineViewerScreenshotSmoke/SmokeMouseInputContract.cs` for
   delegation, old-code removal, timing, dependency, and deterministic rounding
   checks.
4. Read the conditional Link in `src/OpenVisionLab/OpenVisionLab.csproj` when
   embedded Direct Smoke composition is relevant.
5. Use this report and `docs/admin/CODEBASE_STRUCTURE.md` section 9.36 for the
   closed owner boundary and evidence.

## Verification evidence

Evidence root (Debug):
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl44-smoke-mouse-input-contract-debug-20260909-run3`.

Evidence root (Release):
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl44-smoke-mouse-input-contract-release-20260909-run2`.

- PipelineViewerScreenshotSmoke x64 Debug build: 0 errors; the existing
  nullable `CS8600` warning at `Program.cs:10181` remains.
- PipelineViewerScreenshotSmoke x64 Release build: 0 errors; the same
  pre-existing nullable warning remains.
- `SMOKE_MOUSE_INPUT_CONTRACT=PASS` with 10/10 checks in Debug and Release.
  The contract covers Direct delegation, removal of Direct Win32 declarations,
  preserved button flags/timing/thread errors, WPF pump/state ownership,
  conditional composition, dependency isolation, and deterministic rounding.
- Embedded OpenVisionLab x64 Debug and Release builds with
  `OpenVisionLabEnableEmbeddedSmokeRunner=true`: 0 warnings, 0 errors.
- Adjacent regressions passed in Debug and Release: monitor placement contract
  9/9, task waiter contract 8/8, and command-line target contract 10/10.
- `Invoke-RefactorAudit.ps1 -Verify`:
  `REFACTOR_AUDIT=PASS|CSharpFiles=828|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
- `TestDocumentationIndex.ps1`:
  `DocumentationIndex=PASS IndexedPaths=263 Routes=13 RootRedirects=102`.
- JSON parse and scoped `git diff --check` passed; no staged or unmerged paths
  were present at close-out.
- A first concurrent Debug build attempt produced unrelated WPF generated-file
  errors while the app and smoke projects wrote shared intermediates. The
  required sequential Debug and Release builds then passed; no generated-file
  workaround or expectation change was applied.
- Runtime desktop cursor/drag input, monitor topology, DPI/theme matrix, and
  full visual docking interaction were not launched in this source/contract
  slice and remain unverified.

## Boundary lock

This owner is complete for the current Direct Smoke mouse-input contract.
Another model, agent, or automation must not recreate, rename, re-split, move,
or duplicate this owner without a new mouse-input defect, changed gesture or
timeout contract, or demonstrated dependency conflict. File length, model
preference, or a repeated continuation request is not sufficient.

Next priority: perform a fresh residual Direct Smoke responsibility audit, then
return to the recorded Shell/Recipe roadmap only when one call path and
mutable-state boundary are proven. Completed OVL-01/02/03/04/05/06a/07/08/10,
OVL-12 through OVL-43 owners remain closed. Recommended model: `gpt-6-astra`;
reasoning effort: `high`.
