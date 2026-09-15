# OpenVisionLab 2D ExpectedFailure contract audit — 2026-09-15

Status: Complete for the bounded 2D-026 execution-error/quality-NG
classification contract. The existing sample catalog, runner, metric gates,
artifact gates, and batch result owners remain in place; this slice adds the
smallest typed guard at their existing boundaries.

## Scope and product boundary

The goal was to prevent a crash, timeout, invalid tool, or unknown tool error
from being counted as a normal `ExpectedFailure` quality NG. A normal quality
NG must come from a tool-successful step whose acceptance evaluation failed.
Controlled no-result errors remain supported only when they are typed as one of
the existing no-result diagnostics; three product rows now demonstrate the
strict optional contract. Rows without the optional fields remain compatible,
but are reported as `Legacy` rather than as strong validation.

The product boundary is unchanged: Windows x64/.NET 8 OpenCvSharp rule-based
recipe execution. No new runner, qualification database, dataset service,
Labeling integration, release, deployment, camera/PLC/I/O platform, or
Original-repository change was introduced.

## Owner map and shortest reading order

| Responsibility | Current owner and call path |
| --- | --- |
| Catalog schema and row identity | `VisionPipelineSampleCatalogItem.LoadRunnable` reads optional `ExpectedOutcome`, `ExpectedError`, and `ExpectedFailedStep` columns. |
| ExpectedFailure policy | `VisionPipelineExpectedFailureContract.Evaluate` classifies the first failed typed step as `QualityNG`, `ControlledNoResult`, or an execution error. |
| In-process sample validation | `VisionPipelineSampleCheckService.RunSampleCheckAsync` reuses the contract, keeps metric checks active, rejects non-finite values, and marks unclassified tool errors as `ExecutionCompleted=false`/`ERROR`. |
| CLI/catalog validation | `tools/RunVisionSampleCatalog.ps1` parses `Success`, step status, typed error, optional contract fields, metrics, and artifacts without replacing the existing runner. |
| Batch result state | Existing `VisionPipelineBatchOutcomeContract` continues to consume the sample result's execution state; no second batch state owner was added. |
| Focused proof | `tools/VisionRecipeRunnerSmoke/ExpectedFailureContract.cs` dispatches from `Program.cs` through the friend assembly and writes D: evidence. |
| Mutable state and lifetime | The sample runner remains the execution/lifetime owner. This slice only projects typed result state; it does not own images, native tools, or persisted reports. |

Shortest code-reading order:

1. This report and the `ExpectedFailure` column contract below.
2. `VisionPipelineExpectedFailureContract.cs` — normalization, safe error
   allow-list, quality-NG/controlled-no-result evaluation.
3. `VisionPipelineSampleCheckService.cs` — call path, metric/artifact-facing
   result state, and execution completion projection.
4. `VisionPipelineSampleCatalog.cs` — CSV optional-field loading.
5. `tools/RunVisionSampleCatalog.ps1` — independent CLI assertion and report
   projection.
6. `tools/VisionRecipeRunnerSmoke/ExpectedFailureContract.cs` — focused cases
   and generated evidence.

## Findings before the change

The runner already exposed the required typed evidence: `ToolSuccess`,
`AcceptanceEvaluated`, `AcceptancePassed`, `Status`, `ErrorCode`, and
`ErrorName`. `VisionPipelineResultSummaryService` already distinguishes a
tool-successful acceptance `NG` from `ERROR`/`TIMEOUT`/`CANCEL`. The defect was
the layer above it: `VisionPipelineSampleCheckService` used only
`!result.Success`, while `RunVisionSampleCatalog.ps1` used only a nonzero exit
code for `ExpectedFailure`. Therefore an invalid tool or timeout could satisfy
the same expected-failure branch as a quality NG.

The existing metric and artifact checks were retained. A metric map value or
expected bound that is `NaN` or infinite now fails closed in both the in-process
service and the catalog script.

## Optional catalog contract

The three columns are appended to all sample catalog headers and are optional:

| Column | Values | Rule |
| --- | --- | --- |
| `ExpectedOutcome` | `QualityNG`, `ControlledNoResult` (aliases `NG`, `NoResult`) | Required when either of the other two optional fields is present. |
| `ExpectedError` | Existing typed error name, for example `MatchingNoResult` | For `ControlledNoResult`, it must match the first failed typed step. `QualityNG` may not declare a tool error. |
| `ExpectedFailedStep` | First failed step name or 1-based index | If supplied, it must match the first failed step. |

If all three fields are blank, the row uses the compatibility path: the typed
quality/no-result allow-list still rejects arbitrary errors, but the result is
marked `Legacy` and must not be described as a strict contract.

## Bounded implementation

- Added `VisionPipelineExpectedFailureContract` as the single in-process policy
  owner. It requires a tool-successful acceptance failure for `QualityNG` and
  limits `ControlledNoResult` to existing no-result diagnostics (`MatchingNoResult`,
  `ContourNoResult`, `BlobNoResult`, Feature no-result variants, and LineGauge
  no-result variants). `ToolFactoryFailed`, `StepTimeout`, `StepCanceled`, ROI,
  input, template, and unknown errors do not pass this branch.
- Added the three optional catalog fields without changing existing row
  meanings. Three representative product rows are strict (`QualityNG` and two
  `ControlledNoResult` rows); all other existing rows remain legacy-compatible.
- `VisionPipelineSampleCheckService` now exposes
  `ExpectedFailureValidation`/`ExpectedFailureClassification`, keeps
  `HasToolError`, and reports unclassified tool errors as `ERROR` with
  `ExecutionCompleted=false`. A declared controlled no-result remains a
  completed, explicitly classified sample result so the existing expected-NG
  workflow remains usable.
- `RunVisionSampleCatalog.ps1` mirrors the same classification from the runner's
  typed output, adds a report/JSON contract column, and preserves metadata,
  metric, result-image, overlay-image, and raw-log assertions.

## Acceptance matrix

| Case | Expected contract | Evidence |
| --- | --- | --- |
| Tool-successful acceptance NG | Strict `QualityNG` passes only with `Status=NG`, `ErrorCode=0`, and a matching optional step when declared. | Debug/Release `expected-failure-contract.txt`; strict service run for `Product_Battery_TabGap_Narrow_Bad`. |
| Controlled no-result | Strict `ControlledNoResult` passes only with an allow-listed typed error and matching error/step. | Debug/Release contract; service and catalog runs for Battery Laser Mark and Display Alignment. |
| Legacy row | Blank optional fields remain compatible but are labeled `Legacy`. | Contract case `legacy quality NG is accepted but marked legacy`; catalog JSON `ExpectedFailureValidation=Legacy` path is preserved. |
| Timeout | `StepTimeout` is not a quality NG and fails the ExpectedFailure contract. | Contract case `timeout is not a quality NG`. |
| Invalid tool / arbitrary execution error | `ToolFactoryFailed` and missing/crash-like runner output fail closed; missing artifacts remain failures. | Contract case `invalid tool is not a controlled no-result`; D: catalog failure fixture with `GateStatus=NG`, nonzero script exit, and missing result/overlay evidence. |
| Wrong error/step | Optional error and step mismatches fail closed. | Contract case `wrong error and step fail closed`. |
| Non-finite metric | `NaN`/`Infinity` metric or expected bounds are rejected rather than range-compared. | `PipelineAcceptanceFiniteContract` existing finite-value contract plus new finite guards in service/script; the isolated CLI fixture did not fabricate a non-finite runner metric. |

## Verification actually run

- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Debug --no-restore -p:Platform="Any CPU" -p:WpgCustomBuildEnabled=false` — pass, 0 errors (existing nullable warnings remain; no new error).
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Release --no-restore -p:Platform="Any CPU" -p:WpgCustomBuildEnabled=false` — pass, 0 errors (existing nullable warnings remain; no new error).
- `VisionRecipeRunnerSmoke.exe --expected-failure-contract` Debug — `passed=8|failed=0`; final evidence is under `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d026-debug-20260915-final\`.
- `VisionRecipeRunnerSmoke.exe --expected-failure-contract` Release — `passed=8|failed=0`; final evidence is under `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d026-release-20260915-final\`.
- `RunVisionSampleCatalog.ps1` bounded passing fixture (three existing product ExpectedFailure rows) — `GateStatus=OK`, `OKRows=3`, artifacts and metric gates all present; output is under `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d026-script-pass-20260915\`.
- `RunVisionSampleCatalog.ps1` invalid/missing-pipeline fixture — expected nonzero script exit, `GateStatus=NG`, contract failure plus missing result/overlay artifacts preserved; output is under `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d026-script-fail-20260915\`.
- PowerShell parse check for `tools/RunVisionSampleCatalog.ps1` — pass.

## Boundary and next dependency

This proves the bounded in-process and CLI classification paths. It does not
prove every catalog row, a fabricated non-finite runner emission through a
real tool, long-running hardware timeout behavior, or a full WPF theme/DPI
matrix. Those are separate risks and are not silently claimed complete.

Completion record:

```text
Status: Complete
Scope: 2D-026 ExpectedFailure execution-error versus quality-NG classification, optional catalog fields, and finite metric guards.
Acceptance criteria: quality NG requires typed acceptance failure PASS; controlled no-result requires allow-listed typed error and optional matches PASS; timeout/invalid/unknown execution errors fail closed PASS; metric/artifact/metadata checks retained PASS; legacy rows labeled PASS.
Verification: Debug/Release builds and ExpectedFailure contract 8/8 PASS; bounded catalog pass fixture 3/3 PASS; invalid fixture correctly failed with GateStatus=NG and preserved artifact/metadata failures.
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d026-debug-20260915-final, D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d026-release-20260915-final, D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d026-script-pass-20260915, D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d026-script-fail-20260915, and this report.
Boundary / next dependency: all-catalog rerun, real non-finite runner emission, hardware/timeout endurance, and WPF visual matrix remain unverified; next scheduled boundary is 2D-027 confusion-matrix separation.
```
