# Pipeline Review Flow Presenter Refactor Proof (2026-07-26)

## Status

Complete.

## Scope

- Move Pipeline Review Step flow projection out of
  `OpenVisionPipelineReviewDocument`.
- Preserve enabled/disabled, branch, produced-input, missing-input,
  waiting/loaded, execution-result, and acceptance-NG display behavior.

## Excluded

- No Pipeline execution, validation, layer, image, selection, fixture,
  Preview/Run, route, or visible layout change.
- No new interface, factory, or command-surface partial.

## Structural Change

- Previous owner: `OpenVisionPipelineReviewDocument` directly resolved previous
  enabled output, branch input, upstream producer availability, missing input,
  flow status/text, and constructed every `PipelineFlowStepItem`.
- Current owner: `OpenVisionPipelineReviewFlowPresenter` owns both full-list
  `CreateItems` and selected-Step `CreateStepProjection`.
- Current call path:
  Document reads layer-image/execution-summary state -> Flow Presenter creates
  projection -> existing View/ViewModel receives the result.
- Dependency direction:
  the Presenter depends only on Pipeline models, flow control models, supplied
  state callbacks, and localization. It does not depend on the Document, View,
  display manager, or execution controller.

## Acceptance Criteria

1. The Document no longer owns flow branch/status/input-missing algorithms.
2. One non-partial Presenter creates both list and selected-Step projections.
3. Existing normal, input-state, and NG Pipeline Review smokes pass from the
   current source.
4. Debug build and readiness check pass.

## Verification

- Source search confirmed the old Document flow-owner methods were removed and
  the new Presenter owns `CreateItems`, `CreateStepProjection`,
  `ResolveExpectedInputLayer`, `ResolveStatusText`, and `ResolveFlowSummary`.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed
  with 0 warnings and 0 errors.
- Current-source UI smokes passed:
  - `wpf_shell_host_pipeline_review`
  - `wpf_shell_host_pipeline_review_input_state`
  - `wpf_shell_host_pipeline_review_ng`
- Artifacts:
  `artifacts/mvvm_pipeline_review_flow_presenter_20260726`.

## Boundary

This proves a real state-projection responsibility and call-path change. It
does not claim that the whole Pipeline Review Document is MVVM-complete or
requalify runtime inspection semantics.
