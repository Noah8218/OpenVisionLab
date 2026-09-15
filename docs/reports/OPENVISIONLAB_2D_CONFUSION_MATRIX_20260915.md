# OpenVisionLab 2D-027 — Evaluation Confusion Matrix (2026-09-15)

## Status

Complete for the source/contract scope. The existing Run History surface now
projects the matrix; actual WPF rendering remains an explicit unverified
boundary.

## User outcome

Validation results must not make a quality error look like a model NG, or make
an interrupted run look like a completed evaluation. A saved result therefore
reports the four quality cells and keeps execution errors, not-run samples, and
unknown expected labels outside that matrix.

`OK` is the positive acceptance class:

| Cell | Expected | Actual | Meaning |
| --- | --- | --- | --- |
| TP | OK | OK | correct accept |
| TN | NG | NG | correct reject |
| FP | NG | OK | false accept / missed defect |
| FN | OK | NG | false reject / over-reject |

## Owner and call path

The existing outcome owner remains canonical:

```text
ValidationSetRunner
  -> VisionPipelineBatchRunSummaryStorage.Save(inputSampleCount)
  -> VisionPipelineBatchOutcomeContract.BuildConfusionMatrix
  -> OpenVisionRecipeBatchRunOption.ConfusionMatrix
  -> OpenVisionRecipeRunHistoryPresenter.BuildConfusionMatrixText
  -> existing RecentBatchRunComparisonSummaryText binding
```

No new result store, evaluation database, screen, qualification state, or
runner was introduced. Raw `VisionPipelineBatchSampleRunResult` rows and the
legacy `TotalCount`/`PassCount`/`FailCount` fields remain available.

## Contract

- `InputSampleCount` is persisted on new batch summaries. Local Validation Set
  runs pass the registered image count, including when the operator stops the
  run early. Other callers default it to the observed result count.
- `TP`, `TN`, `FP`, and `FN` require a resolvable expected and actual outcome
  from a completed row.
- A non-completed outcome is `ExecutionErrorCount`, not a quality cell.
- `NotRunCount` is `InputSampleCount - observed result rows` for partial runs.
- A completed row without a resolvable expected label is `UnknownLabelCount`.
- `AccountedCount` must equal `InputSampleCount`; legacy summaries without the
  new field fall back to the observed row count.
- Accuracy, false-accept rate, and false-reject rate use explicit denominators
  and return `N/A` when the denominator is zero.

## Verification

Evidence is stored under `D:\OpenVisionLab-TestData\OpenVisionLab_Dev`:

- Debug build:
  `dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug --no-restore -p:Platform="Any CPU" -p:WpgCustomBuildEnabled=false`
  passed with 0 errors; the 16 nullable warnings are pre-existing in unrelated
  smoke contracts.
- Debug contract:
  `2d027-confusion-matrix-20260915-final\confusion-matrix-contract.txt`
  passed 3/3.
- Release build:
  the same command with `-c Release` passed with 0 errors; the 16 nullable
  warnings are pre-existing in unrelated smoke contracts.
- Release contract:
  `2d027-confusion-matrix-20260915-release-final\confusion-matrix-contract.txt`
  passed 3/3.
- The contract covers four TP/TN/FP/FN rows, an execution error, two not-run
  rows, an unknown label, zero-sample `N/A`, and save/reload projection.

## Boundaries

- Full product/validation catalog execution was not rerun.
- Actual desktop WPF rendering, selected-run navigation, themes, compact/wide
  layouts, DPI, monitor placement, and keyboard/mouse states remain
  `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.
- Hardware, long-running timeout, and release/deployment behavior remain out
  of scope.

## Completion record

Status: Complete

Scope: Persisted input denominator and existing Run History confusion-matrix
projection for labelled batch results.

Acceptance criteria: TP/TN/FP/FN and separate execution-error/not-run/unknown
counts pass; accounted count equals input count; zero denominators show `N/A`;
legacy save/reload remains compatible.

Verification: Debug and Release smoke builds completed with 0 errors; focused
confusion-matrix contract passed 3/3 in both configurations.

Boundary / next dependency: Runtime WPF visual verification is still required
for a UI completion claim. The next autonomous source/contract slice is
2D-028 (non-contiguous Mat/stride/ROI view ownership).
