# OVL-31 Validation dataset configuration owner — 2026-09-09

## Status

Complete for one independently verifiable smoke-runner configuration boundary.

## Scope

`CaptureShellHostRecipeLocalValidationDataset` previously mixed environment
variable reads, dataset folder discovery, per-role limits, default Matching
baseline paths, pipeline-name/suite/boundary defaults, and baseline XML
substitution with WPF Recipe execution and Run History review. Those inputs now
belong to the concrete WPF-free `ValidationDatasetSmokeConfiguration` owner.

`Program` calls `LoadFromEnvironment` and keeps the existing Recipe workspace
write, explicit validation-set create/add/run sequence, progress file, summary
loading, artifact writer call, review-queue checks, and drawing-evidence viewer
checks. The configuration object carries `DatasetRoot`, `OkFolder`, `NgFolder`,
`TemplatePath`, `PipelinePath`, `PipelineName`, `PipelineXml`, `SuiteName`,
`Boundary`, `MaximumPerRole`, and the default-baseline flag. No Recipe/XML,
Preview/Run, Layer/ImageSpace, PropertyGrid, or product UI contract changed.

## Structural proof

The call path is now:

`target -> Program.CaptureShellHostRecipeLocalValidationDataset -> ValidationDatasetSmokeConfiguration.LoadFromEnvironment -> existing RecipeWorkspaceService/Shell validation commands`.

The former method no longer reads `OPENVISIONLAB_VALIDATION_*`, resolves OK/NG
folders, selects default baseline files, or rewrites baseline XML. The new owner
has no `Program`, `System.Windows`, or Shell dependency. This is a state-owner
change, not a partial-file split: configuration data is created once, passed to
the execution composition, and is not retained across smoke targets.

## Focused contract

`ValidationDatasetSmokeConfigurationContract` passed 7/7 in both Debug and
Release. It verifies Program delegation and removal of the old preparation
signals, WPF-free ownership, caller-supplied nested `all_images` folder and
pipeline behavior, maximum-per-role clamping, default Matching baseline XML and
path substitution, and fail-closed missing-dataset handling.

Evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl31-validation-dataset-configuration-contract-debug-20260909`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl31-validation-dataset-configuration-contract-release-20260909`

The existing OVL-22 through OVL-30 contracts also passed in Debug and Release
under the `ovl31-regression-*` evidence directories.

## Build and audit

`PipelineViewerScreenshotSmoke.csproj` Debug and Release builds completed with
zero errors. Each retained the existing single `CS8600` warning in
`Program.cs` (currently line 10395); the new owner and contract introduced no
additional warning. `Invoke-RefactorAudit.ps1 -Verify` passed with
`CSharpFiles=805`, `XamlFiles=59`, `PartialDeclarations=108`,
`ViewModelUiIoFiles=1`, `ProjectCycles=0`, and `ShellStorageCalls=0`.

## Junior developer assessment

PASS for this slice. Dataset input preparation is discoverable in one named
owner, while `Program` visibly starts at Recipe persistence and explicit
validation execution. The remaining target-specific UI review and evidence
composition is intentionally still in `Program` and is the next investigation
area; it was not split by file size.

## No-repeat boundary

Do not recreate, rename, or re-split `ValidationDatasetSmokeConfiguration`, or
move OVL-27/30 artifact output, without a newly reproduced configuration defect,
changed explicit smoke contract, or demonstrated responsibility conflict.

## Dev checkpoint

Selective Dev commit `[2.1.0]` `fd759948`
(`fd759948f21d6593202206dab6c5d2bba3ef4eaf`) was pushed to
`origin/codex/public-sample-ux-docs`. Existing dirty worktree changes were
preserved. Original, tags, release publication, and deployment are unchanged.

## Next priority

Inspect one remaining validation dataset execution or summary/UI owner and
complete only one independently verifiable boundary. Alternate WPF
theme/DPI/topology runtime rows and final program-wide gates remain separate
environment-bound work.
