# OpenVisionLab Video-Gated Pipeline Review Clarity

Date: 2026-07-29 KST

## Outcome

Status: **Complete**

The first user-approved video-gated operator-UX slice separates three meanings
that were visually easy to conflate:

- `Pipeline 구성`: whether the selected Pipeline definition is structurally
  valid;
- `검사 결과`: whether the explicit current Run passed its acceptance gates;
- `검출 후보`: Blob/Contour rows created by segmentation or contour extraction,
  which may differ from the number of physical objects.

The object summary now reads `검출 후보 / 검사 대상 / 필터 제외`, and its
guide explains the green/red row meaning. Runtime execution, acceptance,
Preview/Run, layers, active layer, and routing were not changed.

## Scope

Included:

- Pipeline Review summary and Step-detail labels;
- Object Results tab title, count wording, and explanatory guide;
- migration of unchanged saved default translations to the new defaults;
- focused Korean UI assertions;
- same-scenario actual-EXE before/after recording and frame comparison.

Excluded:

- Blob/Contour detection or acceptance behavior;
- automatic Preview/Run;
- layer or route mutation;
- ROI semantics, preset guidance, threshold selection, or algorithm
  qualification;
- completion of CVR-00.

## Acceptance Criteria And Evidence

1. Pipeline validity and inspection judgment use distinct labels: pass.
   The Bad replay visibly shows `Pipeline 구성 OK` beside `검사 결과 NG`.
2. Object rows are presented as candidates rather than physical-object count:
   pass. The focused UI shows `검출 후보 5 / 검사 대상 2 / 필터 제외 3` and
   the physical-object caveat.
3. Existing saved catalog defaults migrate without overwriting customized
   values: pass. Migration applies only when both saved Korean and English
   values exactly match the former shipped defaults; missing guide text is
   added through the existing catalog merge.
4. Good/Bad behavior remains explicit and unchanged: pass. The after timeline
   records one explicit Good review as OK and one explicit Bad review as NG;
   loading the paired Bad sample is still separate from execution.
5. Current build and focused WPF checks pass: pass.
6. Fresh actual-EXE before and after evidence exists: pass.

## Verification

```text
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
```

Result: 0 warnings, 0 errors.

Focused current-source UI checks:

```text
cvr05_object_metric_distribution=OK
wpf_shell_host_workspace_sample_pipeline_review_metrics=OK
wpf_shell_host_workspace_sample_pipeline_review_blob_ng_metrics=OK
```

The object-metric smoke also asserts the exact Korean labels, candidate counts,
candidate explanation, retained rows, distribution series, range markers, and
evidence identity.

Actual-EXE after replay:

```text
Status=Complete
EXE=bin/Debug/OpenVisionLab.exe
EXESHA256=239C081C7B05BA2E825F661FB2F468FA98E207480CA8331FA3793068629F010E
Good review=OK / 10.6 ms
Bad review=NG / 7.6 ms
Video=1920x1080, 30 fps, 64.67 seconds
```

Elapsed time is observational and is not used as a semantic equivalence gate.

## Evidence

- Before video:
  `artifacts/video_gated_ui_clarity_20260729/before_r3/novice-blob-self-trial.mp4`
  (`05DA760265047F85B093AC54CA10B70864F7D198D0C5CCBBC84251C1BC29F625`)
- After video:
  `artifacts/video_gated_ui_clarity_20260729/after/novice-blob-self-trial.mp4`
  (`799F566A1FD51A48AC7C395706633D5501BEB0BAFA7FD80C936C2E29C5BA0C4A`)
- Before/after Bad frame comparison:
  `artifacts/video_gated_ui_clarity_20260729/comparison/bad_before_after.png`
- Focused object-candidate UI:
  `artifacts/video_gated_ui_clarity_20260729/ui_smoke_cvr05_r3/cvr05_object_metric_distribution.png`
- Current-source Good and Bad review captures:
  `artifacts/video_gated_ui_clarity_20260729/ui_smoke_good` and
  `artifacts/video_gated_ui_clarity_20260729/ui_smoke_bad`
- Timelines and EXE identity:
  `artifacts/video_gated_ui_clarity_20260729/before_r3` and
  `artifacts/video_gated_ui_clarity_20260729/after`

## Boundary And Next Priority

This proves the recorded public Blob Good/Bad workflow and the visible wording
contract only. It is agent-operated evidence, not an independent novice study,
and CVR-00 remains deferred until three real first-time participants are
available.

Next priority: audit the direct Teaching state `USE_ROI=true` with
`ROI=0,0,0,0`. If current-source evidence confirms that it is misleading,
clarify the full-image fallback without changing runtime semantics or causing
Preview/Run. Recommended model: `gpt-5.6-terra` | Reasoning effort: `medium`.

## Durable Completion Record

```text
Status: Complete
Scope: Pipeline-definition versus inspection-result terminology, detection-candidate terminology/guide, saved-default migration, focused UI assertions, and same-scenario actual-EXE before/after evidence.
Acceptance criteria: distinct state labels -> pass; candidate meaning -> pass; saved default migration without custom-value overwrite -> pass; explicit Good OK and Bad NG replay -> pass; current build and focused UI checks -> pass; before/after media retained -> pass.
Verification: Debug solution build passed with 0 warnings/errors; three focused UI targets passed; actual-EXE after replay completed at 1920x1080/30 fps; timelines and comparison frames reviewed.
Evidence: artifacts/video_gated_ui_clarity_20260729; docs/reports/OPENVISIONLAB_VIDEO_GATED_UI_CLARITY_20260729.md
Boundary / next dependency: Agent-operated workflow evidence only; CVR-00 still requires three independent first-time participants. ROI zero-size/full-image semantics are a separate next audit.
```
