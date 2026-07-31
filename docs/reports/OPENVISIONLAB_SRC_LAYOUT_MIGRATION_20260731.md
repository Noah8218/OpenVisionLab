# OpenVisionLab `src` Layout Migration — 2026-07-31

Status: Complete

## Scope

- Move the main WPF application project and its owned source/configuration to
  `src/OpenVisionLab/`.
- Move the independent internal library projects to `src/Libraries/`.
- Preserve namespaces, runtime behavior, root `bin/` output compatibility,
  PropertyGrid behavior, explicit Preview/Run, layer routing, and release data
  ownership.
- Update the solution, ProjectReference/HintPath/link paths, source-check tools,
  build/publish scripts, and current navigation/contract documentation.
- Keep historical evidence documents historical where their numbered paths
  describe the layout that existed at the time.

Excluded:

- No feature, algorithm, XML semantics, UI layout, or namespace refactor.
- No change to `C:\Git\OpenVisionLab`.
- No staging, commit, push, or package publication.

## Resulting ownership

```text
OpenVisionLab_Dev/
├─ src/
│  ├─ OpenVisionLab/
│  │  ├─ OpenVisionLab.csproj
│  │  ├─ Program.cs
│  │  ├─ App.config
│  │  ├─ App/ Common/ Core/ Property/ Properties/ UI/ Vision/
│  │  └─ UI/Menu/Wpf/Viewer/CViewer.cs
│  └─ Libraries/
│     └─ 12 independent internal library projects
├─ tools/
├─ docs/
├─ scripts/
├─ dll/
└─ OpenVisionLab.sln
```

`CViewer.cs` belongs to the main application despite its former location under
the old `Library/` root. It therefore moved to the application viewer owner,
not to `src/Libraries/`.

## Implementation notes

- `OpenVisionLab.sln` now names `src\OpenVisionLab\OpenVisionLab.csproj` and
  all 12 `src\Libraries\...` projects.
- The main project uses sibling ProjectReferences under `src/Libraries/`.
- Main output paths remain rooted at repository `bin/` so existing runtime and
  verification contracts keep their expected EXE locations.
- The previous broad `Compile Remove` exclusions for root `Library/` and
  `tools/` are gone. The physical project boundary now prevents accidental
  compilation.
- The optional embedded direct-smoke source is an explicit linked Compile item;
  it is absent from the default main build.
- `OpenVisionLab.ImageCanvas` resolves repository DLL roots through shared
  MSBuild properties instead of assumptions about the old project depth.
- Tool project references, linked sources, readiness checks, localization
  scanning, clean-runtime build paths, and sample runners use the new roots.
- The migration exposed and corrected two verification-tool defects: the
  localization checker still scanned the old roots, and one generated HTML CSS
  string had been damaged by a mechanical `UI\` path substitution.

## Acceptance criteria

| Criterion | Result | Evidence |
| --- | --- | --- |
| Main application and libraries have explicit `src` owners | Pass | `src/OpenVisionLab`, `src/Libraries` |
| Moved content is preserved | Pass | 734-file manifest; 732 unchanged hashes and exactly 2 intentional project-file edits |
| Old active root source/project items are absent | Pass | 0 old root items |
| Solution paths resolve | Pass | 15/15 projects |
| ProjectReferences resolve | Pass | 31/31 references |
| Main compile ownership is bounded | Pass | 504 items; 0 library violations; 0 external-tool violations; `CViewer` 1 |
| Debug solution build | Pass | 0 warnings, 0 errors |
| Release solution build | Pass | 0 warnings, 0 errors |
| Readiness contracts | Pass | 13/13 |
| Localization catalog | Pass | 2,551 entries; 106 direct keys |
| Public assets | Pass | 33 catalog rows; 229 assets; 17 pipelines |
| Public sample execution | Pass | `GateStatus=OK`; 33/33 rows |
| Dev clean-runtime path | Pass | current project path produced a runnable output manifest |
| Release publish path | Pass | framework-dependent win-x64 publish; 55 files; all four required host files present |
| Supported source-build verifier | Pass | locked restore, Debug/Release, readiness, vendored DLLs, expected EXEs |
| JSON and diff hygiene | Pass | current catalog/evidence JSON parsed; `git diff --check` has no errors |

## Verification

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet build "OpenVisionLab.sln" -c Release -p:Platform="Any CPU"
dotnet run --no-build --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"
dotnet run --project "tools\LocalizationCatalogCheck\LocalizationCatalogCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"
powershell -NoProfile -ExecutionPolicy Bypass -File "tools\TestPublicSampleAssets.ps1"
powershell -NoProfile -ExecutionPolicy Bypass -File "tools\RunVisionSampleCatalog.ps1" -CatalogPath "docs\samples\OpenVisionLab.PublicSampleCatalog.csv" -OutputDir "artifacts\src_layout_migration_20260731\public_sample_catalog_r2"
powershell -NoProfile -ExecutionPolicy Bypass -File "tools\BuildCleanRuntime.ps1" -Mode Dev -Configuration Debug -Platform AnyCPU -OutputDir "artifacts\src_layout_migration_20260731\clean_runtime_debug"
dotnet publish "src\OpenVisionLab\OpenVisionLab.csproj" -c Release -p:Platform=AnyCPU -r win-x64 --self-contained false -p:PublishDir=<absolute task artifact path>
powershell -NoProfile -ExecutionPolicy Bypass -File "tools\VerifySourceBuild.ps1"
git diff --check
```

## Evidence

- `artifacts/src_layout_migration_20260731/before_move_file_manifest.csv`
- `artifacts/src_layout_migration_20260731/after_move_hash_verification.csv`
- `artifacts/src_layout_migration_20260731/project_reference_verification.csv`
- `artifacts/src_layout_migration_20260731/project_graph_summary.json`
- `artifacts/src_layout_migration_20260731/path_ownership_verification.json`
- `artifacts/src_layout_migration_20260731/mechanical_path_updates.txt`
- `artifacts/src_layout_migration_20260731/public_sample_catalog_r2/sample_catalog_summary.json`
- `artifacts/src_layout_migration_20260731/clean_runtime_debug/clean_runtime_manifest.json`
- `artifacts/src_layout_migration_20260731/release_publish_inventory.csv`
- `artifacts/source_build_verification_20260731_192635/source_build_summary.json`

## Durable closure

Status: Complete

Scope: Main application and internal libraries now have explicit
`src/OpenVisionLab` and `src/Libraries` ownership with all active build/tool
paths migrated.

Acceptance criteria: All criteria in the table above passed.

Verification: Debug/Release builds, readiness, localization, public assets,
33-row sample replay, Dev clean runtime, Release publish, project graph, hashes,
JSON parsing, and diff checks passed.

Evidence: `artifacts/src_layout_migration_20260731` and this report.

Boundary / next dependency: This proves structural and build/runtime
compatibility, not a new feature, algorithm qualification, UI change, or
commercial release. UI screenshots were not required because no visible UI or
workflow behavior changed. Further restructuring requires a concrete ownership
or operator-workflow defect; file length or visual tidiness alone is not a
trigger.
