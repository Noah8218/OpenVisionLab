# OpenVisionLab OVL-38 smoke fixture resources owner

Status: Complete for this independently verifiable slice. The wider
refactoring program remains active.

## Scope

`tools/PipelineViewerScreenshotSmoke/Program.cs` no longer owns the synthetic
Bitmap, template-file, representative-image, streaming-hash, or best-effort
temporary-file helper implementations. The single concrete owner is
`SmokeFixtureResources.cs`. Existing target names, call order, Bitmap/path
contracts, Recipe/XML behavior, explicit Preview/Run behavior, Layer routing,
and WPF capture timing are unchanged.

The following groups remain in `Program.cs` by design: WPF target composition,
capture callbacks and timing, OpenGL diagnostics, and
`WithDockingStateFileBackup`. They were not part of this independent resource
boundary and were not moved by this slice.

## Responsibility and call path

- Current owner before the change: private static fixture helpers in
  `Program`.
- Intended owner: internal static `SmokeFixtureResources`, with no WPF or
  OpenVision product dependency.
- Call path: `Program.Main` -> selected smoke target -> static fixture factory
  -> existing caller owns the returned `Bitmap` or path -> existing caller
  disposes/deletes it in its existing scope and `finally` blocks.
- Mutable state: the owner keeps no fields or caches. Bitmap and temporary-file
  ownership stays with each caller, preserving the former lifetime behavior.
- Public/binding contract: no public type, XAML binding, Recipe/XML type, SDK
  contract, or product application entry point changed. `Program` uses a static
  import so existing helper call names remain stable.

The implementation body moved without logic-token changes. After normalizing
whitespace and changing only the required `private` to `internal` visibility,
the before/after fixture implementation hash is
`71962931406069898943373989cbcf87986c47d8bf2db75f64305e8b9132af04`.

## Shortest reading order

1. `tools/PipelineViewerScreenshotSmoke/Program.cs` — target selection and one
   representative call site.
2. `tools/PipelineViewerScreenshotSmoke/SmokeFixtureResources.cs` — all 14
   fixture/resource methods and their direct `System.Drawing`/file dependencies.
3. `tools/PipelineViewerScreenshotSmoke/SmokeFixtureResourcesContract.cs` —
   WPF-free ownership, shape, hash, template, file, and cleanup checks.
4. `docs/admin/CODEBASE_STRUCTURE.md` section 9.30 — the durable owner map and
   no-duplicate boundary rule.

## Verification evidence

All generated test output was written under `D:\OpenVisionLab-TestData`.

- x64 Debug build:
  `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false` — 0 errors; one existing `CS8600` warning remains at the nullable runtime-packet load in `Program.cs`.
- x64 Release build:
  `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Release -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false --no-restore` — 0 errors; the same existing warning remains.
- Debug contract:
  `PipelineViewerScreenshotSmoke.dll --smoke-fixture-resources-contract D:\OpenVisionLab-TestData\OpenVisionLab_Dev\smoke-fixture-resources-contract-rerun-20260909` — `SMOKE_FIXTURE_RESOURCES_CONTRACT=PASS`, 9/9. Report: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\smoke-fixture-resources-contract-rerun-20260909\smoke-fixture-resources-contract.txt`.
- Release contract:
  `PipelineViewerScreenshotSmoke.dll --smoke-fixture-resources-contract D:\OpenVisionLab-TestData\OpenVisionLab_Dev\smoke-fixture-resources-contract-release-rerun-20260909` — `SMOKE_FIXTURE_RESOURCES_CONTRACT=PASS`, 9/9. Report: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\smoke-fixture-resources-contract-release-rerun-20260909\smoke-fixture-resources-contract.txt`.
- OVL-22 command-line regression: Debug and Release `--command-line-contract` — 10/10 in each run. Reports: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl22-regression-after-ovl38-debug-20260909` and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl22-regression-after-ovl38-release-20260909`.
- `git diff --check` — passed for the source change.

The first `dotnet run --no-build` invocation did not locate the x64 apphost;
the built x64 DLL was then invoked directly and passed in both configurations.

## Runtime boundary and next work

This contract is intentionally window-free, so no desktop EXE, monitor,
theme, layout, DPI, or visual state was exercised for this slice. Existing
WPF screenshot targets remain covered only by their previously recorded
evidence.

Do not let another model, agent, or scheduled run recreate, rename, re-split,
or move this owner without a newly reproduced fixture defect, changed explicit
contract, or demonstrated responsibility/dependency conflict. The next single
priority is to inspect the remaining `WithDockingStateFileBackup` persisted UI
state/cleanup boundary only if its independent ownership can be demonstrated.
Recommended model: `gpt-5.6-terra`; reasoning effort: `high`.
