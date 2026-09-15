# OVL-33 Validation dataset review-queue evidence owner — 2026-09-09

## Status

Complete for one independently verifiable local validation dataset review-queue
evidence boundary. The overall refactoring program remains active.

## Scope

`CaptureShellHostRecipeLocalValidationDataset` previously mixed persisted
summary review-queue assertions with the Shell filter toggle, workspace
side-effect checks, panel visibility, saved-summary copying, and
`review_queue_contract.txt` output. Those responsibilities now belong to the
concrete `ValidationDatasetReviewQueueEvidence` owner.

The owner preserves the existing Pitch metric and SHA-256 identity checks,
`ShowRecentBatchReviewQueueOnly` filter behavior, Preview/Layer/route
invariants, panel `BringIntoView` sequence, and saved summary copy. Its
`BuildContractLines` method keeps the persisted review-queue projection
testable without constructing the Shell. `Program` retains target composition,
the existing visual-tree lookup, Recipe execution, summary/artifact setup, and
drawing evidence. Recipe/XML, Preview/Run, Layer/ImageSpace, PropertyGrid, and
product UI contracts are unchanged.

## Structural proof

The call path is now:

`local validation target -> Program.CaptureShellHostRecipeLocalValidationDataset -> ValidationDatasetReviewQueueEvidence.VerifyAndWrite -> existing Shell RecipeCommands and review-queue state`.

The target method no longer owns the review-queue filter state, status/hash
assertions, workspace invariants, panel visibility sequence, saved-summary
copy, or review-queue contract-file projection. The visual-tree lookup remains
at the composition boundary because it reuses the smoke runner's existing
`FindVisualChildren` helper. This is a responsibility and state-flow move, not
a partial-file split.

## Focused contract

`ValidationDatasetReviewQueueEvidenceContract` passed 6/6 in both Debug and
Release. It verifies the target call path, removal of the old block, owner
ownership of filter/workspace/panel/artifact behavior, the pure persisted
projection, and exact artifact contents.

Evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl33-validation-dataset-review-queue-evidence-contract-debug-20260909`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl33-validation-dataset-review-queue-evidence-contract-release-20260909`

OVL-22 through OVL-32 contracts also passed in Debug and Release under the
`ovl33-regression-*` directories in the same D: test root.

## Build and audit

`PipelineViewerScreenshotSmoke.csproj` Debug and Release builds completed with
zero errors. Each retained the existing single nullable warning in
`Program.cs`; the OVL-33 owner and contract introduced no new warning.
`Invoke-RefactorAudit.ps1 -Verify` passed with `CSharpFiles=809`,
`XamlFiles=59`, `PartialDeclarations=108`, `ProjectCycles=0`, and
`ShellStorageCalls=0`. `git diff --cached --check` passed before the code
checkpoint.

## Junior developer assessment

PASS for this slice. The validation target now shows a short, explicit
review-queue composition call while the named owner documents the filter,
identity, workspace-safety, visibility, and artifact responsibilities. The
remaining selected-run drawing viewer sequence is still downstream and has
not been hidden in this owner.

## No-repeat boundary

Do not recreate, rename, or re-split `ValidationDatasetReviewQueueEvidence`, or
move OVL-27/30 artifact output or OVL-31/32 owners, without a newly reproduced
review-queue defect, changed explicit smoke contract, or demonstrated
responsibility/dependency conflict. Do not split the remaining drawing-evidence
UI by file size alone.

## Dev checkpoint

Selective Dev commit `[2.1.0]` `cb41ce0a`
(`cb41ce0aa6c79f36b6b138019a1a2e07a5964ccc`) was pushed to
`origin/codex/public-sample-ux-docs`; local and remote SHA match. The
pre-existing dirty worktree was preserved. `Original`, tags, release
publication, and deployment remain unchanged.

## Next priority

Inspect one remaining validation dataset selected-run drawing-evidence UI
composition boundary and complete only one independently verifiable owner.
Alternate WPF theme/DPI/topology runtime rows and final program-wide gates
remain separate environment-bound work.
