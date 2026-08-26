# OpenVisionLab Pipeline Execution Provenance

Date: 2026-08-25 KST  
Repository: `C:\Git\OpenVisionLab_Dev`  
Issue: `PL-0008`  
Status: Complete in Dev

## Scope

Preserve the operator-authored Pipeline definition during execution and retain
enough evidence to distinguish the original definition from the effective
definition that was normalized for a run. This change covers the deterministic
execution core, Pipeline Review, sample-validation reports, Batch summaries,
and qualified-snapshot preflight compatibility. It does not add an
operator-facing Apply-normalization action and does not address the external
DLL license/provenance blocker in `PL-0005`.

## Implementation

- Added `VisionPipelineExecutionPlan` as the serializable boundary between the
  caller/source XML and execution. It loads a separate Pipeline object,
  normalizes that object once, serializes the effective copy, and computes both
  identities before execution.
- `VisionRecipeRunner` preserves exact source-file bytes when the input is a
  file and keeps object callers unchanged. `VisionPipelineExecutionService`
  retains a compatibility entry point but delegates execution to
  `RunPreparedAsync`; the execution body no longer normalizes its input.
- Pipeline Review creates the same execution plan before building its review
  context, so result/Step callbacks refer to the effective copy without
  mutating the review caller's Pipeline.
- Normalization changes are recorded at property level, including step index,
  Step name, change kind, original value, effective value, and the existing
  operator-readable message.
- Run Reports now store schema version, separate original/effective snapshots,
  both SHA-256 values, application identity, Vision SDK identity, SDK manifest
  identity/hash, and structured normalization changes. Batch summaries retain
  the same identity/change set while keeping their existing single snapshot
  layout backward-compatible.
- Qualified snapshot preflight resolves the original snapshot for the frozen
  Pipeline definition and independently verifies the effective snapshot hash.
  The archive semantic check follows the original snapshot for manifest
  identity, while both snapshots remain inside copied evidence.

## Acceptance criteria

| Criterion | Result | Evidence |
|---|---|---|
| C1. Original serialized Pipeline remains unchanged across direct Runner, repeated Runner, file Runner, Pipeline Review, sample validation, and Batch paths. | PASS | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0008_current_20260825_r3\pipeline_provenance_contract.txt` and `completion.txt` |
| C2. Normalization occurs once on an effective copy and the current chain routing/preprocessing/result/drawing path remains executable. | PASS | Source search leaves `NormalizeForRun` in the execution-plan boundary only; focused contract records the effective chain and four structured changes. |
| C3. Saved evidence records original/effective identity, structured changes, application identity, and SDK/manifest identity without saving the original Recipe. | PASS | `...\report.xml`, `pipeline.original.xml`, `pipeline.xml`, and `...\summary.xml` under the r3 evidence directory. |
| C4. Old reports remain readable, new report/batch data round-trips, qualified snapshot compatibility is preserved, and Pipeline Review does not change display selection/layers. | PASS | `legacy-report.xml`, r3 contract observation, and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0008_qualified_snapshot_20260825_r1\SMOKE_RESULT.txt`. |

The r3 report contained schema `1`, distinct original/effective SHA-256 values,
four property-level normalization entries, application identity
`OpenVisionLab;AppVersion=2.1.0;AssemblyVersion=2.1.0.0`, Vision SDK assembly
identity `3.0.0`, and the manifest identity/hash. The effective snapshot shows
the normalized `Blob` input route and preprocessing values; the original
snapshot retains the caller values.

## Verification

Commands run from the Dev repository:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` — passed
  with 0 warnings and 0 errors.
- `dotnet build "tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj" -c Debug` — passed with 0 warnings and 0 errors.
- `dotnet run --project "tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj" -c Debug -- --pipeline-provenance-contract "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0008_current_20260825_r3"` — passed.
- `dotnet run --project "tools\QualifiedRecipeSnapshotSmoke\QualifiedRecipeSnapshotSmoke.csproj" -c Debug -- "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0008_qualified_snapshot_20260825_r1"` — passed.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"` — passed.
- `powershell -NoProfile -ExecutionPolicy Bypass -File "tools\TestExternalReferences.ps1"` — passed inventory gate; pre-existing `BLOCKED` external-reference rows remain evidence for `PL-0005`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File "tools\TestPublicSampleAssets.ps1"` — passed (`33` catalog rows, `229` manifest assets, `17` Pipelines).
- `git diff --check` — no whitespace errors; Git emitted only existing LF/CRLF normalization notices for the dirty worktree.

## Evidence paths

- Focused contract: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0008_current_20260825_r3\pipeline_provenance_contract.txt`
- Completion record: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0008_current_20260825_r3\completion.txt`
- Current Run Report: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0008_current_20260825_r3\data\RECIPE\PL0008_Provenance\VISION\PipelineRuns\PL0008 Provenance Pipeline\20260825_155609657\report.xml`
- Current Batch Summary: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0008_current_20260825_r3\data\RECIPE\PL0008_Provenance\VISION\PipelineBatchRuns\PL0008 Provenance Pipeline\20260825_155616827\summary.xml`
- Qualified snapshot regression: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0008_qualified_snapshot_20260825_r1\SMOKE_RESULT.txt`

## Boundary

This is a Dev implementation and evidence closure for `PL-0008`. It does not
prove DLL licensing or provenance for blocked third-party binaries, does not
make release or deployment decisions, and does not authorize changes to the
original repository, commits, tags, pushes, or publication.
