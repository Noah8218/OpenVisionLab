# OVL-32 Validation dataset execution progress owner — 2026-09-09

## Status

Complete for one independently verifiable local validation dataset execution
progress boundary. The overall refactoring program remains active.

## Scope

`CaptureShellHostRecipeLocalValidationDataset` previously combined the
`RunValidationSuiteCommand` call with progress-file initialization, periodic
status writes, UI pumping, and the saved Run History completion wait. Those
execution-lifetime concerns now belong to the concrete
`ValidationDatasetExecutionProgress` owner.

The owner receives explicit callbacks for command execution, command state,
status text, saved-run detection, UI pumping, and clocks. It writes the
registration line, appends the existing two-second status checkpoints, and
stops on the existing ten-minute deadline or when the command is executable and
a saved run exists. `Program` still owns validation-set creation and image
registration, summary loading, artifact output, Run History review, and
drawing evidence. Recipe/XML, Preview/Run, Layer/ImageSpace, PropertyGrid, and
product UI contracts are unchanged.

## Structural proof

The call path is now:

`local validation target -> Program.CaptureShellHostRecipeLocalValidationDataset -> ValidationDatasetExecutionProgress.Run -> existing Shell command/status callbacks`.

The old method no longer owns `DateTime` deadline state, progress append I/O,
or the polling loop. The new owner is independent of `System.Windows` and the
concrete Shell host; UI affinity remains at the `pump` callback boundary. This
is a state and responsibility move, not a partial-file split.

## Focused contract

`ValidationDatasetExecutionProgressContract` passed 7/7 in both Debug and
Release. It verifies the target call path, discoverable command usage, removal
of the old polling implementation from the target method, explicit callback
dependencies, WPF/Shell independence, successful early completion, and
deadline-bounded progress retention.

Evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl32-validation-dataset-execution-progress-contract-debug3-20260909`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl32-validation-dataset-execution-progress-contract-release3-20260909`

The existing OVL-22 through OVL-31 contracts also passed in Debug and Release;
the combined regression evidence is under the `ovl32-regression-*` directories
in the same D: test root. The first regression invocation used an obsolete
command name and was immediately rerun with the supported
`--command-line-contract` entry point; the corrected run passed 10/10 in both
configurations.

## Build and audit

`PipelineViewerScreenshotSmoke.csproj` Debug and Release builds completed with
zero errors. Each retained the existing single nullable warning in
`Program.cs` (now line 10390); the OVL-32 owner and contract introduced no new
warning. `Invoke-RefactorAudit.ps1 -Verify` passed with
`CSharpFiles=807`, `XamlFiles=59`, `PartialDeclarations=108`,
`ProjectCycles=0`, and `ShellStorageCalls=0`. `git diff --cached --check`
passed before each checkpoint commit.

## Junior developer assessment

PASS for this slice. The validation target now presents Recipe setup and
dataset registration followed by a named execution-progress owner. The
callback names make UI-thread pumping and completion detection visible without
requiring a junior developer to trace the Shell object inside a timing loop.
Summary loading and Run History/drawing review remain visibly downstream and
are intentionally separate responsibilities.

## No-repeat boundary

Do not recreate, rename, or re-split `ValidationDatasetExecutionProgress`, or
move OVL-27/30 artifact output or OVL-31 configuration, without a newly
reproduced execution-lifetime defect, a changed explicit smoke contract, or a
demonstrated responsibility/dependency conflict. Do not split the remaining
summary/UI review by file size alone.

## Dev checkpoint

Selective Dev commit `[2.1.0]` `bc64b6bd`
(`bc64b6bdfad004bf6940abaa4195dfb0956a2f63`) was pushed to
`origin/codex/public-sample-ux-docs`; local and remote SHA match. The
pre-existing dirty worktree was preserved. `Original`, tags, release
publication, and deployment remain unchanged.

## Next priority

Inspect one remaining validation dataset summary/UI composition boundary and
complete only one independently verifiable owner. Alternate WPF theme/DPI/
topology runtime rows and final program-wide gates remain separate
environment-bound work.
