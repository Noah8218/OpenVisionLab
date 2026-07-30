# OpenVisionLab Simulated Novice Self-Trial

Date: 2026-07-29 KST

## Decision

Status: **Complete**

Two current-build, actual-EXE walkthroughs were completed. The first ran from
the empty workspace through visible public-sample selection, explicit Good
review, paired Bad selection, and explicit Bad review. The second selected the
same image, opened the direct Blob Tool View, applied the Basic preset, changed
the threshold from 100 to 150, and used explicit Preview to teach the bright
particle target without running the prepared Pipeline.

The Good image returned `ResultCount=12` and OK. The paired sparse image returned
`ResultCount=3` and NG, with the visible explanation that the result was five
below the lower bound. Opening the paired sample did not execute the Pipeline;
the operator still had to press `Run Review`.

The direct Teaching Preview detected 12 bright circular particles and excluded
the oval border from the accepted target set. Preset selection and threshold
editing did not execute automatically; the result remained pending until the
operator pressed Preview.

This is an agent-operated self-trial with prior project knowledge. It is useful
as a current-EXE walkthrough and a facilitator rehearsal, but it does **not**
complete CVR-00 and does not count as an independent first-time participant.

## Product And Trial Boundary

OpenVisionLab remains an OpenCvSharp4 rule-based recipe workbench centered on
PropertyGrid teaching, explicit Preview/Run, deterministic evidence, N-sample
validation, and saved recipe evidence. This trial did not test camera, lighting,
PLC/I/O, MES, deployment, certified metrology, production robustness, or field
qualification.

Included:

- fresh Debug build of the current Dev source;
- actual `OpenVisionLab.exe`;
- empty-workspace start;
- visible sample catalog and target selection;
- natural mouse movement and clicks;
- explicit Good and Bad Pipeline Review;
- direct Blob preset, threshold edit, and explicit Preview;
- video, timeline, run identity, contact sheet, and selected full-resolution
  frames;
- visual review of success and beginner-comprehension risks.

Excluded:

- a genuinely independent human participant;
- creating a new algorithm or tuning a new recipe from an arbitrary image;
- product UI or runtime behavior changes;
- production, unseen-data, installer, support, and field evidence.

## Selected Image And Inspection Target

| Item | Selection |
| --- | --- |
| Good image | `Public_Blob_Particles_Good` / `Blob_Particles_Synthetic_OK.png` |
| Bad image | `Public_Blob_Particles_Sparse_Bad` / `Blob_Particles_Synthetic_Sparse_NG.png` |
| Visible intent | Count bright synthetic particles |
| Pipeline | `Public_Blob_Particles.pipeline.xml` |
| Acceptance | `ResultCount 8..14` |
| Expected Good evidence | Bright particles remain inside the accepted count range |
| Expected Bad evidence | Sparse image falls below the accepted count range |

The catalog showed the purpose, image, pipeline, Good/Bad pair, expected metric,
and next action before the sample was opened. This made the simple inspection
target understandable without reading XML.

## Actual Walkthrough

| Stage | Actual result |
| --- | --- |
| Empty workspace | The public sample action was visible in the beginner card. |
| Select evidence | Search exposed one public Blob Good sample with image and `ResultCount 8..14`. |
| Open sample | The Main image showed 12 visible bright particles and a prepared two-Step Pipeline. |
| Execute Good | Explicit `Run Review` returned OK; selected Blob evidence showed `ResultCount=12`, 12 accepted objects, object rows, and metric distribution. |
| Open counterpart | The paired Bad action loaded the sparse image directly into Pipeline Review but left the result pending. |
| Execute Bad | A second explicit `Run Review` returned NG; selected Blob evidence showed `ResultCount=3` and the exact five-object shortage from the lower bound. |

## Video Review Findings

### What worked well

1. The catalog connected image, target, metric range, Pipeline, and Good/Bad pair
   before execution.
2. Opening a sample prepared state only. It did not silently Preview or Run.
3. The workspace kept the image central and presented `Pipeline 보기` as the
   next action.
4. Pipeline Review exposed route, Step state, input/output evidence, object rows,
   distribution, expected range, actual count, and judgment.
5. Switching to the Bad counterpart did not reuse the prior result and did not
   execute automatically.
6. The Bad explanation was actionable: actual 3, target 8–14, five below the
   minimum, plus the parameters an operator would inspect.

### Beginner-comprehension risks

1. **Known-name bias:** the automation searched the exact technical identifier
   `Public_Blob_Particles_Good`. A real beginner would not know that name and
   would need to discover the sample through categories, image, or intent.
2. **Validation versus judgment terminology:** the Bad screen simultaneously
   showed `검증 OK` and `결과 NG`. The first means the Pipeline/Step definition
   is valid, while the second is the inspection decision. A beginner may read
   them as contradictory.
3. **Candidate-count interpretation:** the Good screen showed a large total
   candidate count with 12 accepted and many excluded candidates. Without a
   short label explaining that rejected rows include threshold/noise candidates,
   a beginner may think the image contains hundreds of actual particles.
4. **Technical density:** Pipeline Review provides strong evidence, but the
   Step route, Good/Bad explanation, candidate table, distribution, scale tab,
   and navigation compete on one screen. The main result remains visible; this
   trial did not prove the density is a blocker.
5. **Agent prior knowledge:** the task and expected metric were known before the
   run. The successful path therefore proves execution clarity for a rehearsed
   simple task, not unaided task discovery.

## Direct Blob Teaching Self-Trial

### Actual teaching path

| Stage | Actual result |
| --- | --- |
| Select image | The same public Blob Good image was selected from the empty workspace. |
| Select target | The intended target was defined as the bright circular particles inside the oval, excluding the border and noise. |
| Open tool | The operator opened the left-side Blob Tool View instead of running the prepared Pipeline. |
| Apply starting setup | The `기본` preset was applied. No Preview occurred. |
| Edit parameter | Threshold changed visibly from 100 to 150. The result remained pending. |
| Preview | Explicit Preview created `Blob_Preview`, drew 12 yellow accepted boxes, and published detection count 12. |
| Next action | The Tool View exposed explicit `파이프라인 추가` and `N장 검증` actions after the result. Neither was invoked in this bounded trial. |

### Teaching strengths

1. Input and output are visible side by side.
2. `기본`, `빠른`, and `정밀` provide starting configurations without hiding
   the PropertyGrid.
3. Threshold has a slider and exact numeric editor.
4. Editing a preset or parameter does not silently execute.
5. Preview output uses clear yellow boxes on all 12 intended particles.
6. Count, maximum area, center, and bounding box metrics appear immediately
   after Preview.
7. The next durable actions, Pipeline add and N-image validation, remain
   explicit.

### Teaching comprehension risks

1. **Threshold-selection bias:** the agent already knew to try 150. The Tool View
   explains how to change the value, but not how a beginner should derive the
   first value from this image.
2. **ROI zero-rectangle ambiguity:** `ROI 사용` was checked while the displayed
   ROI was `(x:0 y:0 width:0 height:0)`, and Preview behaved like a full-image
   ROI. A beginner may interpret zero width/height as no inspection region.
3. **Preset tradeoffs:** `기본`, `빠른`, and `정밀` are easy to find, but the
   visible surface does not explain the concrete speed/evidence tradeoff before
   selection.
4. **Persisted-result mental model:** Preview success is clear, but a beginner
   must still understand that Preview is temporary evidence and
   `파이프라인 추가` is the action that composes the current setup into the
   recipe workflow.
5. **Pending-label transition wording:** the first Teaching recording attempt
   encountered a valid unsaved-label decision with `저장 후 종료`,
   `저장하지 않고 종료`, and `계속 작업`. In the context of opening a new
   sample, `종료` can sound like application exit rather than ending the prior
   image/label session. The successful retry started after that pending state
   was cleared.

## CVR-00 Handling

Do not alter the participant-only task sheet or coach participants with these
findings. During the real three-person trial, record whether participants
independently:

- find a sample without knowing its identifier;
- explain why `검증 OK` can coexist with `결과 NG`;
- distinguish total candidates, accepted objects, and rejected candidates;
- understand that opening the paired sample does not execute it;
- cite actual count, expected range, and final judgment.

A product change should still require the existing CVR-00 trigger: the same
task-critical blocker or interpretation error reproduced by at least two of
three independent participants. This self-trial alone does not authorize a UI
rewrite.

### Subsequent Development Decision

After this report was written, the user explicitly authorized bounded feature
development using automated verification plus current-build actual-EXE
before/after recordings while real participants are unavailable. This
authorization changes the development gate, not the evidence label:

- CVR-00 remains incomplete and deferred external validation.
- Agent-operated recordings may admit and verify a bounded UX correction, but
  must not be reported as independent novice-user evidence.
- Algorithm claims still require runtime drawings, metrics, gates, and
  proportionate sample evidence.
- The first completed slice is recorded in
  `docs/reports/OPENVISIONLAB_VIDEO_GATED_UI_CLARITY_20260729.md`.

## Verification

```text
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
Result: PASS, 0 warnings, 0 errors

powershell -NoProfile -ExecutionPolicy Bypass -File
  tools\OperatorWalkthroughCapture\Record-OperatorWalkthrough.ps1
  -Scenario novice-blob-self-trial
  -OutputDirectory artifacts\novice_self_trial_20260729\raw_r2
Result: Status=Complete

powershell -NoProfile -ExecutionPolicy Bypass -File
  tools\OperatorWalkthroughCapture\Record-OperatorWalkthrough.ps1
  -Scenario novice-blob-teaching-self-trial
  -OutputDirectory artifacts\novice_self_trial_20260729\teaching_raw_r2
Result: Status=Complete

Pipeline Review video: 1920x1080, 30 fps, 57.8 seconds
Teaching video: 1920x1080, 30 fps, 43.9 seconds
EXE SHA-256:
239C081C7B05BA2E825F661FB2F468FA98E207480CA8331FA3793068629F010E
Pipeline Review video SHA-256:
3E521843FE0401B90A8B1BCD0B094A2805CC1C35EA999FED1FA08148196F4142
Teaching video SHA-256:
AF30F33FF971053DE497F37DE6ED752524D863641A4113405BAFAD450D2B81D8
```

The first recording attempt is retained under `raw`. It reached the Good result
and loaded the Bad sample, then stopped because the older capture helper expected
an intermediate sample-picker window. Frame review proved the current product
had already loaded the Bad sample directly and was waiting for explicit review.
The helper was corrected to the current UI contract, and `raw_r2` completed.

## Evidence

- Successful video:
  `artifacts\novice_self_trial_20260729\raw_r2\novice-blob-self-trial.mp4`
- Timeline:
  `artifacts\novice_self_trial_20260729\raw_r2\novice-blob-self-trial.timeline.tsv`
- Run identity:
  `artifacts\novice_self_trial_20260729\raw_r2\novice-blob-self-trial.run.txt`
- Contact sheet:
  `artifacts\novice_self_trial_20260729\review_r2\contact_sheet.png`
- Target-selection frame:
  `artifacts\novice_self_trial_20260729\review_r2\frame_00_00_12.png`
- Loaded-image frame:
  `artifacts\novice_self_trial_20260729\review_r2\frame_00_00_19.png`
- Good evidence frame:
  `artifacts\novice_self_trial_20260729\review_r2\frame_00_00_34.png`
- Bad evidence frame:
  `artifacts\novice_self_trial_20260729\review_r2\frame_00_00_54.png`
- Retained incomplete first attempt:
  `artifacts\novice_self_trial_20260729\raw`
- Successful direct Teaching video:
  `artifacts\novice_self_trial_20260729\teaching_raw_r2\novice-blob-teaching-self-trial.mp4`
- Direct Teaching timeline:
  `artifacts\novice_self_trial_20260729\teaching_raw_r2\novice-blob-teaching-self-trial.timeline.tsv`
- Direct Teaching run identity:
  `artifacts\novice_self_trial_20260729\teaching_raw_r2\novice-blob-teaching-self-trial.run.txt`
- Direct Teaching contact sheet:
  `artifacts\novice_self_trial_20260729\teaching_review_r2\contact_sheet.png`
- Threshold-edited, pre-Preview frame:
  `artifacts\novice_self_trial_20260729\teaching_review_r2\frame_00_00_34.png`
- Direct Teaching result frame:
  `artifacts\novice_self_trial_20260729\teaching_review_r2\frame_00_00_39.png`
- Retained incomplete Teaching attempt and unsaved-label transition:
  `artifacts\novice_self_trial_20260729\teaching_raw` and
  `artifacts\novice_self_trial_20260729\teaching_review_r1`

## Durable Completion Record

```text
Status: Complete
Scope: Current-build agent-operated public Blob sample selection, explicit Good/Bad Pipeline Review, and direct Basic-preset/threshold/Preview Teaching, with separate videos and frame review.
Acceptance criteria: sample and target visibly selected -> pass; Good actual/range/judgment visible -> pass; paired Bad requires explicit execution -> pass; Bad actual/range/judgment visible -> pass; direct preset and threshold edits do not auto-run -> pass; explicit Preview detects the intended 12 particles -> pass; both video/current EXE identities retained -> pass; beginner risks reviewed -> pass.
Verification: Debug solution build passed with 0 warnings/errors; successful 57.8-second Pipeline Review and 43.9-second direct Teaching actual-EXE recordings completed; timelines, video metadata/hashes, contact sheets, and key frames checked.
Evidence: artifacts/novice_self_trial_20260729 and docs/reports/OPENVISIONLAB_SIMULATED_NOVICE_SELF_TRIAL_20260729.md
Boundary / next dependency: This is not independent human evidence and does not complete CVR-00. The prerequisite remains at least three real independent first-time participants with unedited observations.
```
