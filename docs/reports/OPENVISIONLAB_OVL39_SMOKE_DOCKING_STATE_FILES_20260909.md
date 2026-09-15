# OpenVisionLab OVL-39 smoke docking state files owner

Status: Complete for this independently verifiable responsibility slice. The
wider refactoring program remains active.

## Scope

The persisted docking state file lifecycle used by the screenshot smoke and
embedded direct smoke entry points now has one concrete owner:
`tools/PipelineViewerScreenshotSmoke/SmokeDockingStateFiles.cs`.
`LayerDocking.layers` and `LayerDocking.layout` backup/restore, clear, and
selected evidence-copy behavior moved out of the runner implementations.
Existing target names, callback order, file names, exception propagation,
best-effort restore cleanup, Recipe/XML behavior, explicit Preview/Run
behavior, Layer routing, and WPF capture timing are unchanged.

The owner is a WPF-free file lifecycle module. It does not create windows,
inspect Shell state, or decide when a docking operation runs. The caller still
resolves the application data directory and owns the WPF callback. The
embedded application links the same source only when
`OpenVisionLabEnableEmbeddedSmokeRunner` is enabled.

## Refactor proof

### Before

- `PipelineViewerScreenshotSmoke.Program` owned `WithDockingStateFileBackup`.
- `OpenVisionLabDirectSmokeRunner` separately owned backup, clear, and evidence
  copy implementations for the same two files.
- Each implementation constructed the same paths and performed its own byte
  snapshot and restore loop.

### After

- `SmokeDockingStateFiles` owns the stable file names, path construction,
  byte snapshots, restore/delete cleanup, clear operation, and selected-file
  evidence copy.
- `Program` calls `SmokeDockingStateFiles.RunWithBackup` directly.
- Direct smoke keeps its existing private method names as thin wrappers that
  delegate to `SmokeDockingStateFiles`; existing scenario call sites remain
  unchanged.
- The app project links the owner under the existing embedded-smoke condition,
  so the product build has no unconditional smoke dependency.

## Responsibility and call path

- Pipeline path: `Program.Main` -> selected docking-persistence target ->
  `SmokeDockingStateFiles.RunWithBackup` -> WPF callback -> owner restores the
  two files in `finally` -> existing `CaptureResult` returns to the target
  runner.
- Embedded direct path: direct scenario -> existing
  `WithDockingStateFileBackup`/`ClearCurrentDockingStateFiles`/
  `CopyCurrentDockingStateFile` wrappers -> `SmokeDockingStateFiles` -> direct
  scenario continues with its existing window and evidence flow.
- Mutable state is invocation-local: the owner keeps no fields, caches, or
  static mutable state. The callback retains WPF window/layer ownership.
- No public type, XAML binding, Recipe/XML type, SDK contract, or product entry
  point changed.

## Junior developer reading order

1. `tools/PipelineViewerScreenshotSmoke/Program.cs` — target registration and
   the docking-persistence call site.
2. `tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs` —
   existing direct-scenario wrapper names and their unchanged call sites.
3. `tools/PipelineViewerScreenshotSmoke/SmokeDockingStateFiles.cs` — one owner
   for file names, backup/restore, clear, and evidence copy.
4. `tools/PipelineViewerScreenshotSmoke/SmokeDockingStateFilesContract.cs` —
   WPF-free structural and byte-level lifecycle checks.
5. `src/OpenVisionLab/OpenVisionLab.csproj` — conditional embedded-source link.
6. `docs/admin/CODEBASE_STRUCTURE.md` section 9.31 — durable owner map and
   no-duplicate boundary rule.

## Verification evidence

All generated output was written under `D:\OpenVisionLab-TestData`.

- Pipeline x64 Debug build:
  `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform=x64 -p:WpfCustomBuildEnabled=false -m:1 -nr:false --no-restore` — 0 errors; one pre-existing `CS8600` warning remains at `Program.cs:10153`.
- Pipeline x64 Release build: same command with `-c Release` — 0 errors; the
  same pre-existing `CS8600` warning remains.
- Embedded app x64 Debug build:
  `dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:OpenVisionLabEnableEmbeddedSmokeRunner=true -p:WpfCustomBuildEnabled=false -m:1 -nr:false --no-restore` — 0 warnings, 0 errors.
- Embedded app x64 Release build: the same command with `-c Release` — 0
  warnings, 0 errors.
- Docking-state contract in Debug and Release:
  `--smoke-docking-state-files-contract` — `SMOKE_DOCKING_STATE_FILES_CONTRACT=PASS`, 9/9 in each configuration.
  Reports: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\smoke-docking-state-files-contract-debug-rerun-20260909\smoke-docking-state-files-contract.txt` and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\smoke-docking-state-files-contract-release-rerun-20260909\smoke-docking-state-files-contract.txt`.
- OVL-22 command-line target regression in Debug and Release —
  `SMOKE_TARGET_RUNNER_CONTRACT=PASS`, 10/10 in each configuration.
- `git diff --check` — passed in the isolated worktree.

The same changed sources were revalidated in the actual Dev checkout after
the safe patch was applied:

- Embedded app x64 Release build — 0 warnings, 0 errors.
- `SMOKE_DOCKING_STATE_FILES_CONTRACT=PASS`, 9/9 in x64 Debug and Release.
  Reports: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl39-actual-docking-contract-debug-20260909\smoke-docking-state-files-contract.txt` and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl39-actual-docking-contract-release-20260909\smoke-docking-state-files-contract.txt`.
- `SMOKE_TARGET_RUNNER_CONTRACT=PASS`, 10/10 in x64 Debug and Release.
  Reports: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl22-regression-after-ovl39-actual-debug-20260909\screenshot-smoke-target-runner-contract.txt` and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl22-regression-after-ovl39-actual-release-20260909\screenshot-smoke-target-runner-contract.txt`.
- `Invoke-RefactorAudit.ps1 -Verify` — `REFACTOR_AUDIT=PASS|CSharpFiles=819|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
- `TestDocumentationIndex.ps1` — `DocumentationIndex=PASS IndexedPaths=258 Routes=13 RootRedirects=102`.
- `docs/LLM_DOCUMENT_INDEX.json` parse — passed.
- Scoped `git diff --check` — passed; only existing line-ending normalization warnings were reported by Git.

The contract covers owner delegation, old implementation removal, conditional
app linking, existing-file restoration, absent-file deletion, exception
rethrow after cleanup, clear, and evidence copy. The code checkpoint is local
commit `7a804859191e5ac21c51997fd6b1c7a700d5cf99` and has not been pushed.

## Runtime boundary and remaining work

This slice changes a WPF-free file lifecycle owner and does not claim fresh
monitor, theme, DPI, or desktop-window evidence. Existing docking persistence
runtime evidence remains the regression baseline. A new UI defect or changed
visual contract is required before reopening the docking View/Controller
owners.

Junior developer self-assessment: **PASS for this boundary**. The file policy
has one searchable owner, while the existing WPF scenarios remain readable at
their call sites. No second owner should be introduced for these two files.
Another model, agent, or scheduled run must not recreate, rename, re-split, or
move this owner without a newly reproduced defect, changed explicit contract,
or demonstrated responsibility/dependency conflict.

Next priority: perform a fresh residual smoke-scenario boundary audit and
select one independent owner only if its call path and mutable-state owner can
be proven; completed OVL-01/02/03/04/05/06a/07/08/10/12-39 owners stay closed.
Recommended model: `gpt-6-astra`; reasoning effort: `high`.
