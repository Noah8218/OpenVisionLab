# OpenVisionLab Workspace Root Cleanup

Updated: 2026-07-31 KST

## Completion Record

Status: Complete

Scope: Dev repository workspace-root hygiene and documentation alignment only.
No production source ownership, runtime behavior, UI, Pipeline, PropertyGrid,
Preview/Run, layer, or route behavior changed.

Source version:

- Repository: `C:\Git\OpenVisionLab_Dev`
- Baseline HEAD: `6c14651`
- Branch: `codex/public-sample-ux-docs`

Acceptance criteria:

- One canonical generated-evidence entry remains at `artifacts/`: Pass.
  The retired `.artifacts` junction target
  `D:\OpenVisionLab_Data\Dev\.artifacts` remains intact with all 37 top-level
  entries.
- Codex temporary data no longer appears as an untracked workspace change:
  Pass. `.codex-temp/` is ignored without deleting its contents.
- Machine-local and unused root debris is removed: Pass.
  The tracked root and `Connected Services` `desktop.ini` files, unused
  `scripts/sql/dbo.Table.sql`, empty root `RECIPE`, and empty stray
  `wpf_shell_host_edge_based_matching_auto_mpoint` directory were removed.
- Current structure documentation names current responsibility roots: Pass.
  `CODEBASE_STRUCTURE.md` no longer uses the retired numbered source paths.
- Maintenance-mode LLM prompt packets are owned by documentation evidence:
  Pass. All 64 files moved from the retired repository-root packet folder to
  `docs/evidence/llm/prompt-packets/`, all 64 Git blob hashes match, and all
  checked documentation references use the new path.
- Current build and deterministic verification remain valid: Pass.

Verification:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --no-build --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"
powershell -NoProfile -ExecutionPolicy Bypass -File "tools\TestPublicSampleAssets.ps1"
powershell -NoProfile -ExecutionPolicy Bypass -File "tools\RunVisionSampleCatalog.ps1" -CatalogPath "docs\samples\OpenVisionLab.PublicSampleCatalog.csv" -OutputDir "artifacts\workspace_root_cleanup_20260731\public_sample_catalog"
git diff --check
```

Results:

- Debug solution build: 0 warnings, 0 errors.
- Readiness: all 13 contracts passed.
- Public sample asset check:
  `CatalogRows=33`, `ManifestAssets=229`, `Pipelines=17`.
- Public sample execution:
  `GateStatus=OK`, `RunnableRows=33`, `OKRows=33`, `NGRows=0`.
- Prompt-packet move: 64 expected, 64 present, 64 hash matches.
- Old repository-root prompt-packet path references: 0.
- Retired numbered paths in the current structure guide: 0.

Evidence:

- `artifacts/workspace_root_cleanup_20260731/legacy_dot_artifacts_retirement.json`
- `artifacts/workspace_root_cleanup_20260731/legacy_dot_artifacts_top_level_manifest.csv`
- `artifacts/workspace_root_cleanup_20260731/prompt_packet_move_hashes.csv`
- `artifacts/workspace_root_cleanup_20260731/public_sample_catalog/sample_catalog_summary.json`
- `artifacts/workspace_root_cleanup_20260731/public_sample_catalog/sample_catalog_report.md`

Boundary / next dependency:

- The preserved external legacy artifact target was not deleted or modified.
- Local/vendor `Sample/`, `.codex/`, `.codex-temp/`, build outputs, and user
  caches were not deleted.
- The main project and libraries were outside this report's original scope.
  The separately authorized follow-on migration is now complete; see
  `docs/reports/OPENVISIONLAB_SRC_LAYOUT_MIGRATION_20260731.md`.
- The original repository `C:\Git\OpenVisionLab` was not touched.
- No files were staged, committed, pushed, or published.
