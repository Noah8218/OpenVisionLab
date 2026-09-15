# OpenVisionLab OVL-42 Smoke Task waiter owner

Status: Complete for one independently verifiable shared Smoke task-wait
boundary. The wider refactoring program remains active.

## Scope

`OpenVisionLabDirectSmokeRunner` and `PipelineViewerScreenshotSmoke.Program`
each contained a pump-based `WaitForTaskWithPump` loop. The loops had the same
responsibility but different caller contracts: Direct requires a non-null task,
uses a 20-second wait and its existing timeout message; Pipeline keeps its
null-is-no-op wrapper, accepts a per-call timeout, and keeps its timeout message.

The common loop is now the WPF-free `SmokeTaskWaiter` owner. The two wrappers
still own their public scenario shape, null behavior, timeout values, timeout
text, and caller-owned `Pump(4)` callback. Recipe/XML, explicit Preview/Run,
Layer/ImageSpace, PropertyGrid, product UI, and scenario names are unchanged.

## Refactor proof

### Before

- Direct implemented the task completion loop with `Stopwatch`, 20 ms delay,
  20-second timeout, and `did not complete within 20 seconds.` text.
- Pipeline implemented a second loop with a caller timeout, 10 ms delay,
  minimum one-second timeout, and `timed out.` text.

### After

- `SmokeTaskWaiter.Wait(Task, string, Action, TimeSpan, TimeSpan, string)` owns
  the pump/delay/timeout loop and final `GetAwaiter().GetResult()` propagation.
- Direct and Pipeline wrappers pass their existing policy values and retain
  their null and timeout-message contracts.
- The application links the owner only when
  `OpenVisionLabEnableEmbeddedSmokeRunner=true`.

## Responsibility and call path

- Direct: scenario -> `WaitForTaskWithPump` -> `SmokeTaskWaiter.Wait` ->
  caller `Pump(4)` and task result.
- Pipeline: scenario -> `WaitForTaskWithPump` -> `SmokeTaskWaiter.Wait` ->
  caller `Pump(4)` and task result.

The owner has no fields or global state. The caller owns dispatcher affinity,
task creation, null policy, timeout values, and scenario lifetime. The owner
owns only the synchronous wait policy and exception propagation. It has no
WPF, Shell, Recipe, file-system, or product dependency.

## Developer reading order

1. `tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs` —
   Direct wrapper and preserved required-task contract.
2. `tools/PipelineViewerScreenshotSmoke/Program.cs` — Pipeline wrapper and
   preserved optional-task contract.
3. `tools/PipelineViewerScreenshotSmoke/SmokeTaskWaiter.cs` — shared WPF-free
   wait owner.
4. `tools/PipelineViewerScreenshotSmoke/SmokeTaskWaiterContract.cs` —
   structural, timeout, pump, exception, and input-boundary checks.
5. `src/OpenVisionLab/OpenVisionLab.csproj` — conditional embedded link.
6. `docs/admin/CODEBASE_STRUCTURE.md` section 9.34 — durable owner map and
   closed-scope rule.

## Verification evidence

All contract and build artifacts were written under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl42-smoke-task-waiter-contract-20260909`.

- Pipeline Smoke x64 Debug/Release builds — 0 errors; the pre-existing
  nullable `CS8600` warning remains at `Program.cs:10169`.
- Embedded OpenVisionLab x64 Debug/Release builds with
  `OpenVisionLabEnableEmbeddedSmokeRunner=true` — 0 warnings, 0 errors.
- `SMOKE_TASK_WAITER_CONTRACT=PASS` 8/8 in Debug and Release.
- OVL-41 clipboard contract — 7/7 in Debug and Release.
- OVL-40 Direct screenshot owner contract — 6/6 in Debug and Release.
- OVL-22 command-line contract — 10/10 in Debug and Release.
- `Invoke-RefactorAudit.ps1 -Verify` —
  `REFACTOR_AUDIT=PASS|CSharpFiles=824|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
- `TestDocumentationIndex.ps1` —
  `DocumentationIndex=PASS IndexedPaths=261 Routes=13 RootRedirects=102`.

Scoped `git diff --check` (exit 0), JSON parsing, and staged-file/unmerged-path
checks (0/0) passed at close-out; no Original or external push was performed.

## Runtime boundary and remaining work

The focused contract uses deterministic in-process tasks and a supplied pump;
it does not claim physical desktop UI, monitor topology, theme, DPI, or real
Clipboard behavior. Those environment-bound checks remain outside this slice.

Developer discoverability assessment: **PASS for this boundary**. There is one
searchable owner for the shared wait loop, while caller-specific policy remains
at the wrappers. Another model, agent, or automation must not recreate, rename,
re-split, or move this owner without a new task-wait defect, changed timeout
contract, or demonstrated dependency conflict.

Next priority: perform a fresh residual Direct Smoke responsibility audit and
select one owner only when its call path and mutable-state boundary are proven;
completed OVL-01/02/03/04/05/06a/07/08/10/12-42 owners stay closed.
Recommended model: `gpt-6-astra`; reasoning effort: `high`.
