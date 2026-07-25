# Root Artifact Map (2026-07-24, current)

Root path: `C:\Git\OpenVisionLab_Dev`

## Root-level artifacts

- `AGENTS.md`
- `README.md`
- `CHANGELOG.md`
- `OpenVisionLab.csproj`
- `OpenVisionLab.sln`
- `Program.cs`
- `App.config`
- `Directory.Build.props`
- `global.json`
- `log4net.config`
- `LICENSE`
- `NOTICE`
- `desktop.ini`

## Source ownership areas

Current high-level ownership layout is:

- `App/Bootstrap/`
  App boot sequence, exception policy, and single-instance coordination.
- `Core/`
  application, recipe, pipeline, storage, and state services.
- `UI/`
  WPF shell, recipe, pipeline review, tool presenters/controllers, popups, and tool views.
- `Common/`
  shared helpers and non-UI domain helpers moved out of legacy numeric folders.
- `Vision/`
  OpenCV property models and helper conversions.
- `Property/`
  low-level property containers.
- `Library/`
  independent internal libraries.
- `tools/`
  smoke runners, checks, generators, and utility executables.
- `docs/`
  contracts, completion logs, and operational runbooks.
- `scripts/`
  environment/setup helper scripts.
- `tmp/`
  temporary files.

## Important path migrations completed in this refactor wave

- `OpenVisionLabDirectSmokeRunner.cs` was moved from repository root to:
  `tools\OpenVisionLab.DirectSmokeRunner\OpenVisionLabDirectSmokeRunner.cs`.
- `OpenVisionLabApplication.cs`, `OpenVisionLabUnhandledExceptionPolicy.cs`,
  `OpenVisionLabSingleInstanceGuard.cs` were moved from `Core\Runtime` to:
  `App\Bootstrap\`.
- `Core`, `UI`, `Common`, `Vision`, and `Property` no longer use legacy numeric root prefixes.
- Tooling project references that depended on `0. UI` and `1. Core` roots now point
  to the migrated paths in `UI\*`, `Core\*`, and `App\Bootstrap`.

## Quick-doc index for operators

- `docs/admin/CODEX_RECOVERY.md`
  Recovery notes and historical cleanup context.
- `docs/admin/NEXT_CODEX_PROMPT.md`
  Next-turn summary for continuation.
- `docs/admin/ROOT_ARTIFACT_MAP_20260724.md`
  This map.
- `docs/OPENVISIONLAB_SOURCE_STRUCTURE_REFACTOR_DESIGN_20260724.md`
  Refactor design and sequencing evidence.
