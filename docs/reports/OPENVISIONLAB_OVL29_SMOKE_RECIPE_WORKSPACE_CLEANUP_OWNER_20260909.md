# OpenVisionLab OVL-29 Smoke Recipe Workspace Cleanup Owner

Status: Complete for one independently verifiable smoke-runner workspace
cleanup policy boundary.

## Scope

`PipelineViewerScreenshotSmoke/Program.cs` previously owned the repeated
`CleanupTransientRecipeWorkspaces` policy and the call into
`RecipeWorkspaceService`. The policy now belongs to
`SmokeRecipeWorkspaceCleanup`:

- `SelectTransientRecipeNames` keeps `Default` and caller-supplied names,
  compares them case-insensitively, and selects only `Smoke_`/`Recipe_`
  workspaces.
- `DeleteTransient` obtains the current Recipe names and deletes the selected
  workspaces through the existing `RecipeWorkspaceService` boundary.
- `Program` keeps the target-specific ordering and calls the owner at the same
  points as before. All 21 former call sites now resolve through the concrete
  owner; the old private helper declaration was removed.

No Recipe XML schema, Preview/Run command, Layer/ImageSpace route, PropertyGrid
policy, product UI, or public API changed. The existing `Default` protection,
keep-name behavior, prefix filtering, and deletion order are preserved.

## Structural proof

Current owner: `PipelineViewerScreenshotSmoke/Program.cs` owned workspace-name
selection and deletion in a private helper used by 21 target paths.

Intended owner: `SmokeRecipeWorkspaceCleanup` owns only transient workspace
selection and deletion. Its pure selection method accepts snapshots for
contract testing; its deletion method retains `RecipeWorkspaceService` as the
existing persistence boundary. `Program` owns target composition and no longer
owns the cleanup policy implementation.

The owner has no WPF or smoke entry-point dependency. No interface, factory,
wrapper, or partial type was added. The completed OVL-28
`RecipeContextFixture` owner and all earlier owners remain unchanged.

## Verification

- Debug build:
  `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform='Any CPU' -m:1 -nr:false --nologo`
  — 0 errors; the pre-existing `CS8600` warning remains at the shifted
  `Program.cs:10497`.
- Release build: same command with `-c Release` — 0 errors; the same existing
  `CS8600` warning remains.
- Debug focused contract:
  `SMOKE_RECIPE_WORKSPACE_CLEANUP_CONTRACT=PASS|checks=6`, evidence under
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl29-smoke-recipe-workspace-cleanup-contract-20260909-run2`.
- Release focused contract:
  `SMOKE_RECIPE_WORKSPACE_CLEANUP_CONTRACT=PASS|checks=6`, evidence under
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl29-smoke-recipe-workspace-cleanup-contract-20260909-release`.
- Existing smoke contracts reran in Debug and Release: OVL-22 10/10,
  OVL-23 7/7, OVL-24 8/8, OVL-25 7/7, OVL-26 10/10, OVL-27 7/7, and OVL-28
  6/6. Evidence is under the `ovl29-regression-*` directories in
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev`.
- Refactor audit:
  `REFACTOR_AUDIT=PASS|CSharpFiles=803|XamlFiles=59|PartialDeclarations=108|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`
  with `-Verify`, evidence under
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl29-refactor-audit-20260909`.
- Documentation index:
  `DocumentationIndex=PASS IndexedPaths=248 Routes=13 RootRedirects=102`.
- `git diff --check` completed with no whitespace errors. Git reported the
  repository's existing LF/CRLF normalization warnings for the dirty files.

## Boundary and next work

Do not recreate or re-split this cleanup owner without a newly reproduced
workspace-lifecycle defect, changed explicit smoke contract, or proven
responsibility conflict. Remaining smoke fixture execution, summary/UI
orchestration, and other product boundaries remain separate slices.
