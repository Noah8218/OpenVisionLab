# Root Artifact Map (2026-07-24, current)

Root path: `C:\Git\OpenVisionLab_Dev`

## Root-level artifacts

- `AGENTS.md`
- `README.md`
- `CHANGELOG.md`
- `OpenVisionLab.sln`
- `Directory.Build.props`
- `global.json`
- `LICENSE`
- `NOTICE`

## Source ownership areas

Current high-level ownership layout is:

- `src/OpenVisionLab/`
  Main WPF application project root. It owns `OpenVisionLab.csproj`,
  `Program.cs`, `App.config`, `log4net.config`, `lens.ico`, and the application
  source trees listed below.
- `src/OpenVisionLab/App/Bootstrap/`
  App boot sequence, exception policy, and single-instance coordination.
- `src/OpenVisionLab/Core/`
  application, recipe, pipeline, storage, and state services.
- `src/OpenVisionLab/UI/`
  WPF shell, recipe, pipeline review, tool presenters/controllers, popups, and tool views.
- `src/OpenVisionLab/Common/`
  shared helpers and non-UI domain helpers moved out of legacy numeric folders.
- `src/OpenVisionLab/Vision/`
  OpenCV property models and helper conversions.
- `src/OpenVisionLab/Property/`
  low-level property containers.
- `src/Libraries/`
  independent internal libraries.
- `tools/`
  smoke runners, checks, generators, and utility executables.
- `docs/`
  contracts, completion logs, and operational runbooks.
- `scripts/`
  release/publish entry scripts. Verification runners remain under `tools/`.

## Generated and local-only areas

- `artifacts/`
  the single canonical generated-evidence root. In the Dev workspace this may
  be a junction to external storage.
- `bin/`, `obj/`, `dist/`
  generated build and publish output.
- `.codex/`, `.codex-temp/`, `.vs/`, `tmp/`
  ignored tool-owned caches and temporary work.
- `Sample/`
  ignored local/vendor sample material. Public samples belong under
  `docs/samples/`.
- Runtime `RECIPE`, `CONFIG`, `Log`, and other writable operator data belong
  to the selected OpenVisionLab data root, not the repository root.

## Important path migrations completed in this refactor wave

- On 2026-07-31 the main application moved to `src\OpenVisionLab\` and the
  internal library projects moved to `src\Libraries\`. The solution, project
  references, smoke tools, scripts, and current path documentation now use
  those ownership roots. See
  `docs/reports/OPENVISIONLAB_SRC_LAYOUT_MIGRATION_20260731.md`.
- `OpenVisionLabDirectSmokeRunner.cs` was moved from repository root to:
  `tools\OpenVisionLab.DirectSmokeRunner\OpenVisionLabDirectSmokeRunner.cs`.
- `OpenVisionLabApplication.cs`, `OpenVisionLabUnhandledExceptionPolicy.cs`,
  `OpenVisionLabSingleInstanceGuard.cs` were moved from `src\OpenVisionLab\Core\Runtime` to:
  `src\OpenVisionLab\App\Bootstrap\`.
- `Core`, `UI`, `Common`, `Vision`, and `Property` no longer use legacy numeric
  root prefixes and now share the `src\OpenVisionLab\` application owner.
- Tooling project references and linked source paths now point to
  `src\OpenVisionLab\*` and `src\Libraries\*`.

## Quick-doc index for operators

- `docs/admin/CODEX_RECOVERY.md`
  Recovery notes and historical cleanup context.
- `docs/admin/NEXT_CODEX_PROMPT.md`
  Next-turn summary for continuation.
- `docs/admin/ROOT_ARTIFACT_MAP_20260724.md`
  This map.
- `docs/admin/OPENVISIONLAB_SOURCE_STRUCTURE_REFACTOR_DESIGN_20260724.md`
  Refactor design and sequencing evidence.
- `docs/reports/OPENVISIONLAB_WORKSPACE_ROOT_CLEANUP_20260731.md`
  Canonical record for the root cleanup, prompt-packet move, and verification.
- `docs/reports/OPENVISIONLAB_SRC_LAYOUT_MIGRATION_20260731.md`
  Canonical record for the application/library source-root migration and verification.
