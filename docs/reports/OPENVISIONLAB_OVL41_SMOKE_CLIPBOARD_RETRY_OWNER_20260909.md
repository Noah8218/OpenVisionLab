# OpenVisionLab OVL-41 Smoke clipboard retry owner

Status: Complete for one independently verifiable shared Smoke clipboard
retry boundary. The wider refactoring program remains active.

## Scope

`OpenVisionLabDirectSmokeRunner` and `PipelineViewerScreenshotSmoke.Program`
each owned the same COM clipboard retry loop: up to 40 attempts, retry only
`0x800401D0`, pump the caller's dispatcher four times, and delay between
attempts. The loop is now the WPF-free `SmokeClipboardRetry` owner. Existing
get/set wrappers keep owning `System.Windows.Clipboard` access and pass their
existing `Pump(4)` callback.

Recipe/XML, explicit Preview/Run, Layer/ImageSpace, PropertyGrid, product UI,
and clipboard text contracts are unchanged.

## Refactor proof

### Before

- Direct contained `GetClipboardTextWithRetry`,
  `SetClipboardTextWithRetry`, and `RunClipboardActionWithRetry`.
- Pipeline Smoke contained a second implementation with the same retry
  HRESULT, attempt limit, delay, and pump behavior.

### After

- `SmokeClipboardRetry.Run<T>(Func<T>, Action)` owns the retry policy,
  target-HRESULT filter, delay, and final exception propagation.
- Direct and Pipeline wrappers retain Clipboard access and delegate the loop,
  so UI-thread pumping remains visible at each caller.
- The application links the same owner only when
  `OpenVisionLabEnableEmbeddedSmokeRunner=true`.

## Responsibility and call path

- Direct: scenario -> `GetClipboardTextWithRetry` or
  `SetClipboardTextWithRetry` -> `SmokeClipboardRetry.Run` ->
  `System.Windows.Clipboard` action; `Pump(4)` remains the caller callback.
- Pipeline: scenario -> existing Clipboard wrapper ->
  `SmokeClipboardRetry.Run` -> `System.Windows.Clipboard` action; `Pump(4)`
  remains the caller callback.

The owner has no fields or global state. The caller owns Clipboard access and
dispatcher affinity; the owner owns only retry control flow. A target COM
failure is retried, an unrelated COM failure is rethrown unchanged, and the
last target failure is propagated after the existing attempt limit.

## Developer reading order

1. `tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs` —
   existing scenario Clipboard wrappers and their `Pump(4)` callback.
2. `tools/PipelineViewerScreenshotSmoke/Program.cs` — the second caller and
   preserved wrapper contract.
3. `tools/PipelineViewerScreenshotSmoke/SmokeClipboardRetry.cs` — shared
   WPF-free retry owner.
4. `tools/PipelineViewerScreenshotSmoke/SmokeClipboardRetryContract.cs` —
   structural and retry/exception boundary checks.
5. `src/OpenVisionLab/OpenVisionLab.csproj` — conditional embedded link.
6. `docs/admin/CODEBASE_STRUCTURE.md` section 9.33 — durable owner map and
   closed-scope rule.

## Verification evidence

All contract and build artifacts were written under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl41-smoke-clipboard-retry-contract-20260909`.

- Pipeline Smoke x64 Debug/Release builds — 0 errors; the pre-existing
  nullable `CS8600` warning remains at `Program.cs:10163`. Logs:
  `pipeline-debug-final.txt`, `pipeline-release-final.txt`.
- Embedded OpenVisionLab x64 Debug/Release builds with
  `OpenVisionLabEnableEmbeddedSmokeRunner=true` — 0 warnings, 0 errors. Logs:
  `embedded-debug-final.txt`, `embedded-release-final.txt`.
- `SMOKE_CLIPBOARD_RETRY_CONTRACT=PASS` 7/7 in Debug and Release.
- OVL-22 command-line contract — `SMOKE_TARGET_RUNNER_CONTRACT=PASS` 10/10
  in Debug and Release.
- OVL-40 Direct screenshot owner contract —
  `DIRECT_SMOKE_SCREENSHOT_WRITER_CONTRACT=PASS` 6/6 in Debug and Release.
- `Invoke-RefactorAudit.ps1 -Verify` —
  `REFACTOR_AUDIT=PASS|CSharpFiles=822|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
- `TestDocumentationIndex.ps1` —
  `DocumentationIndex=PASS IndexedPaths=260 Routes=13 RootRedirects=102`;
  JSON parse, scoped `git diff --check`, and unmerged-path check — passed.
- Build, audit, and index logs are in the evidence directory named above.

## Runtime boundary and remaining work

The focused contract simulates the clipboard HRESULT and validates the retry
and exception paths without touching the system clipboard. Physical desktop
clipboard ownership, monitor topology, theme, and DPI-matrix behavior remain
environment-bound and were not claimed here.

Developer discoverability assessment: **PASS for this boundary**. The retry
policy has one searchable owner while Clipboard access and UI pumping stay at
the scenario call sites. Another model, agent, or automation must not recreate,
rename, re-split, or move this owner without a new clipboard defect, changed
retry contract, or demonstrated dependency conflict.

Next priority: perform a fresh residual Direct Smoke responsibility audit and
select one owner only when its call path and mutable-state boundary are proven;
completed OVL-01/02/03/04/05/06a/07/08/10/12-41 owners stay closed.
Recommended model: `gpt-6-astra`; reasoning effort: `high`.
