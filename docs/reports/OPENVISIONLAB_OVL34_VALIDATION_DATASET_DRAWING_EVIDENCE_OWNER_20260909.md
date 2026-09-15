# OVL-34 Validation dataset selected-run drawing evidence owner — 2026-09-09

## Status

Complete for one independently verifiable selected-run drawing-evidence UI
composition boundary. The overall refactoring program remains active.

## Scope

`CaptureShellHostRecipeLocalValidationDataset` previously mixed persisted
selected-Run evidence resolution, source snapshot verification, expected
PinArrayGap drawing checks, synthetic executed-failure persistence coverage,
viewer selector probing, report-artifact copying, floating-window opening, and
workspace side-effect assertions. Those responsibilities now belong to the
concrete `ValidationDatasetDrawingEvidence` owner.

`Program` keeps the `openDrawingEvidence` decision and composes the existing
Recipe configuration, source path, artifact directory, and UI pump callback.
The owner preserves `OpenVisionRecipeRunEvidence` resolution, SHA-256 source
verification, the two-row PinArrayGap drawing contract, stable failed-Step
selection, read-only viewer behavior, and Layer/Preview/route/active-layer
invariants. It writes `drawing_evidence_contract.txt` beside the copied saved
Run Report. Recipe/XML, explicit Preview/Run, Layer/ImageSpace, PropertyGrid,
and product UI contracts are unchanged.

## Structural proof

The call path is now:

`local validation target -> Program.CaptureShellHostRecipeLocalValidationDataset
-> ValidationDatasetDrawingEvidence.VerifyAndWrite -> existing Shell
RecipeCommands/OpenVisionRecipeRunEvidence/viewer`.

The target method no longer owns stored drawing resolution, source hash
verification, drawing-selector state, saved-report copying, floating-window
lookup, workspace invariants, or the executed-failure persisted-drawing probe.
The `Program` composition boundary still decides whether the target requests
drawing evidence and supplies the existing UI pump; it does not own the moved
policy.

## Focused contract

`ValidationDatasetDrawingEvidenceContract` passed 7/7 in both Debug and
Release. It verifies the new owner call path, removal of the old target block
and helper, source/viewer/artifact/workspace ownership, retention of the
executed-failure drawing proof, pure contract projection, and exact artifact
contents.

The OVL-22 through OVL-33 regression contracts passed in both configurations.
The OVL-32 and OVL-33 contract readers were updated only to use the next
stable method boundary after the executed-failure helper moved; their covered
behavior and assertions are unchanged.

Evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl34-validation-dataset-drawing-evidence-contract-debug-20260909`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl34-validation-dataset-drawing-evidence-contract-release-20260909`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl34-regression-*`

## Build, audit, and runtime evidence

`PipelineViewerScreenshotSmoke.csproj` Debug and Release builds completed with
zero errors. The existing single nullable warning remains in `Program.cs`
(`CS8600`); this slice introduced no new warning.

`Invoke-RefactorAudit.ps1 -Verify` passed with
`CSharpFiles=811|XamlFiles=59|PartialDeclarations=108|ProjectCycles=0|ShellStorageCalls=0`.

The focused EXE target
`wpf_shell_host_recipe_local_validation_drawing_evidence` passed in Debug and
Release using a D-drive synthetic OK/NG dataset. The workstation reported one
monitor, `\\.\DISPLAY2`, bounds `1920x1080`, working area `1920x1032`; no
two-monitor placement fallback was needed. Both captures were `1420x760` and
showed the stored source image, detection drawing, selector, and evidence
status without rerunning Preview or Run:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl34-runtime-drawing-evidence-debug2-20260909\wpf_shell_host_recipe_local_validation_drawing_evidence.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl34-runtime-drawing-evidence-release2-20260909\wpf_shell_host_recipe_local_validation_drawing_evidence.png`

Only the current one-monitor runtime row was exercised. Alternate themes,
Wide/Compact layouts, and 100/125/150/175/200% DPI rows remain environment
bound and are not claimed by this slice.

## Junior developer assessment

**PASS for this boundary.** A new contributor can follow the short target
composition call to a named owner, then read one owner for stored evidence,
selector behavior, artifact copying, floating-window display, and workspace
safety. The owner uses explicit inputs and the existing product evidence/viewer
types; it does not introduce an interface, factory, wrapper, or message bus.

## No-repeat boundary

Do not recreate, rename, or re-split `ValidationDatasetDrawingEvidence`, or
move OVL-27/30/31/32/33 behavior into it, without a newly reproduced drawing
defect, changed explicit smoke contract, or demonstrated responsibility or
dependency conflict. Do not split this owner by file size alone. The remaining
Shell XAML vertical slice, generic PropertyGrid adapter internals,
compatibility-safe namespace/project review, and residual smoke-runner groups
remain separate priorities.

## Dev checkpoint

Selective Dev commit `[2.1.0]` `fc0b2b8d29a94eac737d8fa9ac316390559cacfd`
was pushed to
`origin/codex/public-sample-ux-docs`; local and remote SHA matched at push.
The pre-existing dirty worktree was preserved. `Original`, tags, release
publication, and deployment remain unchanged.

## Next priority

Inspect one Shell XAML vertical slice with a concrete presentation owner and
focused runtime proof. Do not reopen the completed drawing, review-queue,
execution-progress, summary-artifact, or configuration owners without new
evidence | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.
