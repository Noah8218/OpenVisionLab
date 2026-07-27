# OpenVisionLab Validation Outcome Contract Implementation

Date: 2026-07-27
Source commit: `9d7fa796ed94d90e50d840607b441a2954278947`
Workspace: `C:\Git\OpenVisionLab_Dev`

## Result

The Qualified Recipe Snapshot prerequisite for unambiguous validation outcomes
is complete.

New batch rows persist these meanings separately:

- whether execution completed;
- the raw Pipeline outcome;
- the operator-owned expected outcome;
- whether an expected-outcome judgment exists;
- whether that judgment is correct.

The legacy `Success` field remains the aggregate sample-validation result so
older non-judgment batch behavior and pass/fail statistics remain compatible.
New judgment consumers do not infer actual OK/NG from that field.

## Audit Correction

The preceding design audit initially identified the Local Validation Set
handler's expected-NG Boolean inversion as the defect. Deeper tracing during
implementation showed the full flow:

1. `VisionRecipeRunResult.Success` is the raw Pipeline result.
2. `VisionPipelineSampleCheckService.Success` applies expected-failure and
   expected-metric validation.
3. The former Local handler inversion recovered the raw Pipeline result for
   Run History.

The persisted Local result was not proven wrong by that line alone. The real
defect was an implicit, layer-dependent meaning for `Success`, plus no distinct
execution-error state. This implementation removes that ambiguity instead of
preserving the audit's initial causal explanation.

## Implemented Contract

### Execution result

`VisionPipelineSampleCheckResult` now exposes:

| Field | Meaning |
| --- | --- |
| `ExecutionCompleted` | The sample reached a completed Pipeline result. |
| `ActualSuccess` | Raw Pipeline OK/NG before sample-role and expected-metric checks. |
| `Success` | Legacy aggregate validation pass after expected-failure and metric checks. |

### Persisted batch row

`VisionPipelineBatchSampleRunResult` outcome schema v1 adds:

| Field | Meaning |
| --- | --- |
| `OutcomeSchemaVersion` | `1` for an explicit outcome row; `0` for legacy. |
| `ExecutionState` | `Completed` or `Error`. |
| `HasJudgment` | Whether expected versus actual is an authoritative row contract. |
| `ExpectedOutcome` | `OK` or `NG` when `HasJudgment=true`. |
| `ActualOutcome` | Raw `OK` or `NG`; empty when execution did not complete. |
| `JudgmentCorrect` | Stored judgment result, never inferred from `Success` for v1 rows. |

`VisionPipelineBatchOutcomeContract` is the single owner for applying and
resolving these fields, legacy fallback, and false-accept/false-reject
classification.

### Batch summary

`VisionPipelineBatchRunSummary` schema v2 adds:

- `JudgmentCount`;
- `JudgmentCorrectCount`;
- `FalseAcceptCount`;
- `FalseRejectCount`;
- `ExecutionErrorCount`;
- `LegacyAmbiguousCount`.

`summary.tsv` exposes all new row fields. A future Qualified Recipe Snapshot
preflight can reject any batch with legacy ambiguous rows, execution errors, or
incorrect judgments without reparsing display text.

### Run History and review queue

- Run History renders explicit actual outcomes for judgment rows.
- Correct rejects remain actual NG while their aggregate validation result is
  pass.
- Judgment filters use `JudgmentCorrect`, not raw/legacy `Success`.
- Execution errors are visible separately and cannot become false accepts or
  false rejects.
- Review queue policy v2 records `execution-error`, misclassification,
  evidence-gap, metric extrema, and bounded hash-audit reasons.
- Legacy summaries remain readable through the previous `ExpectedActual:`
  role/text fallback.

## Four-Outcome Matrix

| Expected | Actual | Stored judgment | Run History |
| --- | --- | --- | --- |
| OK | OK | correct | Correct accept |
| OK | NG | incorrect | False reject |
| NG | OK | incorrect | False accept |
| NG | NG | correct | Correct reject |

An additional explicit execution-error probe verified:

- no actual outcome is published;
- judgment is incorrect;
- neither false-accept nor false-reject is assigned;
- the review queue contains `execution-error`.

## Current Local Validation Set Evidence

The focused product-path run stored four rows:

```text
Summary schema: 2
Outcome schema: 1
Rows: 4
Aggregate validation pass/fail: 3/1
Judgments correct: 3
False accept: 1
False reject: 0
Execution error: 0
Legacy ambiguous: 0
```

The expected-NG probe was explicitly retained as:

```text
Success=false
ExecutionState=Completed
ExpectedOutcome=NG
ActualOutcome=OK
JudgmentCorrect=false
Review reason=false-accept
```

It did not receive `runtime-failure` or `execution-error`.

## Side-Effect Contract

The two focused current-source UI smokes verified that validation registration,
execution review, filtering, and review-queue selection did not unexpectedly
change:

- Preview/Run count outside the explicit suite run;
- layer count;
- active layer;
- input/output routes.

This change does not add automatic qualification or automatic Pipeline
execution.

## Verification

Solution build:

```text
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" /nr:false -m:1 /p:UseSharedCompilation=false
PASS: 0 warnings, 0 errors
```

Screenshot runner build:

```text
dotnet build "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug /nr:false -m:1 /p:UseSharedCompilation=false
PASS: 0 warnings, 0 errors
```

Focused current-source UI:

```text
wpf_shell_host_recipe_run_history_review_queue=OK
wpf_shell_host_recipe_local_validation_set=OK
layout=0, text=0, internal=0, 1600x900
```

Readiness:

```text
dotnet run --project "tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"
PASS: OpenVisionLab readiness contract passed
```

Artifacts:

- `artifacts\qualified_recipe_outcome_contract_20260727\final\wpf_shell_host_recipe_run_history_review_queue.png`
- `artifacts\qualified_recipe_outcome_contract_20260727\final\wpf_shell_host_recipe_local_validation_set.png`
- `artifacts\qualified_recipe_outcome_contract_20260727\final\wpf_shell_host_recipe_run_history_review_queue.evidence\summary.xml`
- `artifacts\qualified_recipe_outcome_contract_20260727\final\wpf_shell_host_recipe_run_history_review_queue.evidence\summary.tsv`
- `artifacts\qualified_recipe_outcome_contract_20260727\final\wpf_shell_host_recipe_run_history_review_queue.evidence\review-queue-contract.txt`

No true task-before screenshot was captured because the initial change was
treated as a storage-contract correction before its visible Run History impact
was recognized. The closest historical current-source baseline is
`artifacts\mvvm_validation_set_presenter_20260726\wpf_shell_host_recipe_local_validation_set.png`;
it is retained as historical context and is not represented as a fresh before
capture.

## Changed Ownership

- `Core\Pipeline\Execution\VisionPipelineSampleCheckService.cs`
- `Core\Pipeline\Storage\VisionPipelineBatchOutcomeContract.cs`
- `Core\Pipeline\Storage\VisionPipelineBatchRunSummaryStorage.cs`
- `UI\Menu\Wpf\OpenVisionShellHostRecipeCommandSurface.Handlers.cs`
- `UI\Menu\Wpf\Recipe\Models\OpenVisionRecipeSampleRunModels.cs`
- `UI\Menu\Wpf\Recipe\Review\OpenVisionRecipeRunHistoryPresenter.cs`
- `tools\PipelineViewerScreenshotSmoke\Program.cs`

## Completion Record

Status: Complete
Scope: Validation sample actual-outcome, expected-outcome, judgment, execution
state, batch summary, Run History, review queue, TSV, legacy-read fallback, and
focused regression evidence
Acceptance criteria: Four OK/NG outcomes and an execution-error case are
unambiguous; current Local Validation Set persistence and presentation use the
explicit schema; legacy rows still render; execution errors and
misclassifications remain separate; UI side effects are unchanged
Verification: Debug solution and screenshot-runner builds passed with zero
warnings/errors; two current-source UI smokes and the full readiness contract
passed
Evidence: `artifacts\qualified_recipe_outcome_contract_20260727\final` and this
report
Boundary / next dependency: Qualified Recipe Snapshot archive creation,
integrity verification, lifecycle events, and Recipe Manager qualification UI
are not implemented. They are now the next bounded project priority.
