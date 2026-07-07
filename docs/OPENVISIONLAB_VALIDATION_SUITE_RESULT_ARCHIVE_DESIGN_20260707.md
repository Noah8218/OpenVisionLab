# OpenVisionLab Validation Suite And Result Archive Design

Updated: 2026-07-07 KST

## Decision

Build the next operating feature as a recipe-local validation suite inside the existing Recipe Manager `Runs` workflow.

Do not create a new hardware/runtime platform. Do not add camera, lighting, PLC, I/O, account, scheduler, deployment, or background auto-run scope.

## Why This Is Priority 1

Commercial vision tools are stronger at proving an inspection job before production use: validation cases, result history, baseline comparison, saved evidence, and operator reports.

OpenVisionLab already has most of the raw pieces:

- `VisionPipelineSampleCheckService` runs a sample against a pipeline and returns status, metric text, failed step, final layer, overlay count, action summary, and elapsed time.
- `VisionPipelineBatchRunSummaryStorage` saves batch summaries as `summary.xml` and `summary.tsv`.
- `VisionPipelineRunReportStorage` can save a pipeline snapshot, step reports, result images, overlay images, metrics, and parameters.
- Recipe Manager already shows recent batch runs, baseline comparison rows, selected sample results, failure-step actions, and copyable review text in the `Runs` tab.

The missing product behavior is not another algorithm tool. It is a clear recipe-local validation flow:

1. Choose the sample cases that prove this recipe.
2. Run them explicitly.
3. Save enough evidence to reopen what passed or failed.
4. Compare the latest run against a baseline.
5. Export/copy a compact review report.

## Non-Goals

- No camera acquisition.
- No lighting control.
- No PLC/I/O.
- No account, login, role, or server audit.
- No deployment runtime.
- No automatic Preview/Run.
- No replacement for PropertyGrid-based tool editing.
- No new database unless XML/TSV files become insufficient.

## Phase 1 Scope

Use the existing `Runs` tab and add a validation-suite section above the existing run history.

The first version should support:

- Current selected sample.
- Current Good/Bad pair group when available.
- Current Product catalog benchmark path that already exists.
- Explicit `Run suite` command.
- Saved run summary with recipe, pipeline, suite type, started/finished time, pass/fail count, sample results, failed step, metric text, final layer, overlay count, action summary, sample image path, and summary path.
- Baseline comparison using the existing recent batch run comparison model.
- Selected sample failure actions that already exist: focus failed step, load sample image to input layer, view input, view output, copy review.

Do not add custom arbitrary image lists in Phase 1. That is useful later, but the existing sample catalog and Good/Bad pair flow are enough to prove the operating model.

## Phase 1 Data Shape

Reuse `VisionPipelineBatchRunSummary` where possible. Add only fields that make archived runs understandable:

- `SuiteName`
- `SuiteKind`
- `PipelineSnapshotFile`
- `Notes`

Extend `VisionPipelineBatchSampleRunResult` with:

- `SampleImagePath`
- `PairGroup`
- `PairRole`
- `ExpectedText`
- `MetricText`
- `MetricReviewText`
- `FinalLayer`
- `OverlayCount`
- `ActionSummary`
- `RunReportPath`

Keep old XML readable by using defaults for missing fields.

Phase 1 can write `RunReportPath` only when a per-sample report is available. If saving full per-step images for every sample is too heavy, save the full report for failed samples first and keep summary-only evidence for OK samples.

## UI Shape

Add a compact validation section at the top of `Runs`:

```text
Recipe Manager / Runs

Validation Suite
  Active: <Recipe> / <Pipeline>       [Run suite] [Set baseline] [Copy report] [Open folder]
  Scope: (Selected sample) (Good/Bad pair) (Product catalog)

  Cases                         Latest Result Matrix                    Evidence Inspector
  ---------------------------   ------------------------------------    ------------------------------
  OK  Good sample               Sample | Expected | Actual | Step       Run summary
  NG  Bad sample                Good   | OK       | OK     | -          Selected sample
  WAIT Product category rows    Bad    | NG       | NG     | 02 Blob    Metrics / gate review
                                                                      Failed step / next action
                                                                      Input/output actions

Run History
  Baseline selector
  Regression / Recovered / Still NG comparison
  Recent run list
  Selected run review
```

The UI should feel like the existing Recipe Manager: dense, workbench-oriented, no landing-page styling, no nested card layout.

## Operator Flow

1. Select recipe and pipeline.
2. Choose suite scope.
3. Click `Run suite`.
4. Review pass/fail count and matrix.
5. Select NG sample.
6. Use existing failed-step actions to inspect input/output and parameters.
7. Mark or choose a baseline run.
8. Compare a later run against baseline.
9. Copy report for handoff.

Every run action stays explicit. Selecting a sample, toggling suite scope, loading an image path, or selecting a history row must not run Preview/Run.

## Implementation Checkpoints

1. Current-source before capture of the existing `Runs` tab.
2. Data extension with backward-compatible XML serialization.
3. ViewModel properties for suite scope, run status, matrix rows, selected evidence row, and baseline summary.
4. XAML addition inside the existing `Runs` tab.
5. Direct smoke updates for:
   - suite controls visible;
   - `Run suite` creates summary;
   - selecting rows does not increment Preview/Run count;
   - baseline comparison still detects regression.
6. Current-build after capture.
7. Required checks:
   - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`
   - focused Recipe Manager smoke;
   - readiness check;
   - external reference check;
   - public sample asset check;
   - `git diff --check`

## Open Questions

Recommended default for Phase 1:

- Suite scope default: `Good/Bad pair` if the selected sample has a pair, otherwise `Selected sample`.
- Full per-step image report: save for failed samples first.
- Custom ad-hoc image suite: defer until the catalog/pair workflow proves useful.

