# OpenVisionLab First-Time Operator Journey Audit

Updated: 2026-07-29 KST

## Decision

Status: **Complete**

The current Dev build exposes a coherent guided path from public sample selection
through explicit result review and, for trained operators, local validation and
Qualified Recipe Snapshot creation. This audit did not reproduce a current-source
operator blocker or regression, so it selects no product implementation.

The remaining evidence gap is external: no independent first-time user has yet
completed the workflow without developer guidance. The product can therefore be
described as usable for guided sample-backed work, but not as independently
proven self-teaching or field-ready.

## Product Boundary

OpenVisionLab is an OpenCvSharp4 rule-based vision recipe workbench. Its product
core is direct PropertyGrid teaching, explicit Preview/Run, image and drawing
evidence, deterministic OK/NG judgment, N-sample validation, and saved recipe
evidence. The LLM XML surface remains optional and frozen in maintenance mode.

Commercial products teach OpenVisionLab to retain:

- intent-oriented setup and an obvious next action;
- image-centered recipe and result review;
- per-object and per-step evidence rather than one final Boolean;
- named recipes, explicit validation, and immutable qualification evidence.

Camera, lighting, PLC, I/O, accounts, deployment, MES, controller runtime, and
production-equipment qualification remain out of scope.

## Audit Scope

Included:

- current product-direction, stable-contract, current-handoff, and commercial-gap
  documents;
- current WPF source for sample, Learn, Recipe Manager, Pipeline Review, local
  Validation Set, and Qualified Recipe Snapshot entry points;
- a fresh Debug build;
- five fresh current-source WPF view captures and their embedded smoke checks;
- a bounded independent-operator trial protocol.

Excluded:

- source or runtime behavior changes;
- a new algorithm, dataset campaign, parameter-tuning campaign, or LLM campaign;
- an actual human usability study;
- installer, support, equipment, and field qualification.

## First-Time Operator Journey

| Stage | Operator question | Current product answer | Current evidence | Audit result |
| --- | --- | --- | --- | --- |
| 1. Choose evidence | “What am I inspecting, and what should Good/Bad look like?” | Sample Catalog shows purpose, tool family, expected result, Good/Bad pair, comparison metric, guide, input image, and Pipeline before opening the sample. | `wpf_shell_host_workspace_sample_picker` | Pass. The entry screen communicates intent and the next explicit action. |
| 2. Learn and teach | “Which tool and values do I change?” | Learn topics connect concepts to a related Tool View or Pipeline Review. Tool configuration remains PropertyGrid-based. Guidance tells the operator to click Preview or Run Review explicitly and compare overlay plus metrics. | `OpenVisionWorkspaceSamplePickerViewModel`, `OpenVisionLearnWindow`, readiness contracts | Pass at guided-content level. Learner comprehension has not been measured. |
| 3. Enter the recipe | “Where do I continue?” | Recipe Manager Summary keeps lifecycle commands separate and promotes `Next: Open Pipeline` as the primary action. Current work sample and latest same-recipe result are shown separately. | `wpf_shell_host_recipe_manager_summary` | Pass. No navigation rewrite is justified. |
| 4. Execute and explain | “Did the rule run, why did it pass, and what should I inspect next?” | Pipeline Review exposes explicit `Run Review`, readiness cards, Step route, input/output images, metric gate, Good/Bad evidence, drawings, object rows, `Learn Tool`, and `Return to Recipe`. | `wpf_shell_host_workspace_sample_pipeline_review_metrics` | Pass. The current capture shows accepted and rejected Blob candidates with exact measurements and reject reason. |
| 5. Validate more than one image | “Does the frozen rule agree with expected OK/NG across a set?” | Advanced Recipe review owns named local Validation Sets, expected roles, missing-path repair, explicit suite execution, batch history, and explicit expected/actual/judgment evidence. | `wpf_shell_host_recipe_local_validation_set` | Pass as an advanced operator workflow. It is intentionally denser than the novice summary and should not be moved into the default entry screen without observed user failure. |
| 6. Preserve qualified evidence | “Can I freeze exactly what passed, inspect it later, and make a separate working copy?” | Run History qualification binds one completed matching Local Validation Set run to explicit scope and note, preflight, immutable content-addressed evidence, verification, working copy, supersede, and revoke actions. | `wpf_shell_host_recipe_qualified_snapshot` and its evidence contract | Pass as a trained review workflow. This proves local evidence preservation, not production or field qualification. |

## What A User Can Learn Today

The current product can teach a user to:

1. begin with an inspection intent and a known Good/Bad pair rather than an
   arbitrary prompt;
2. identify the responsible tool family and edit its explicit parameters;
3. understand that loading, selecting, navigating, and changing visibility do
   not execute the inspection;
4. compare the Step input, output, overlay, metric, gate, and rejected-object
   reason after an explicit run;
5. distinguish one-sample inspection evidence from N-sample validation;
6. distinguish a mutable working recipe from an immutable qualified snapshot.

It does not yet prove that an unaided beginner correctly understands those
concepts. It also does not teach a complete OpenCV curriculum, camera/lighting
engineering, certified metrology, or production acceptance.

## Observed Friction And Triage

### No reproduced blocker

All five current-source smoke targets passed their internal layout, text, and
state checks. Visual review found no clipped primary action, overlapping content,
or broken transition that prevents the audited tasks.

### Advanced-screen density

The Validation Set and Qualified Snapshot panels carry many technical fields and
small evidence rows. This is visible density, but the screen is entered through
Advanced Review and serves trained validation/qualification work. Static visual
density alone is not evidence for a new wizard, top-level navigation rewrite, or
automatic action.

### External evidence gap

The unresolved question is not whether commands exist. It is whether an
independent first-time operator can form the intended mental model and complete
the core path without coaching. Only a human trial can answer that.

## Independent First-Time Operator Trial Protocol

The reusable study materials are separated to prevent answer leakage:

- participant-visible task only:
  `OPENVISIONLAB_CVR00_PARTICIPANT_TASK_SHEET_20260729.md`;
- facilitator setup, raw observation, runtime identity, and decision gate:
  `OPENVISIONLAB_CVR00_FACILITATOR_PACKET_20260729.md`.

Do not give this audit report to a participant because the journey table and
verification sections disclose the intended product answers.

### Prerequisite

Recruit at least three participants who have not used OpenVisionLab and are not
given an implementation walkthrough. Use the latest built Dev executable or a
clean packaged runtime that is verified against the same source revision.

No model or implementation work should start merely to prepare for this trial.
Prepare only isolated temporary Recipe names and the existing public samples.

### Core task: required for every participant

Use the existing `Public_Blob_Particles_Good` /
`Public_Blob_Particles_Sparse_Bad` pair and its existing sample Pipeline.

1. From Sample Catalog, state the intended inspection, expected Good/Bad
   difference, and comparison metric.
2. Open the guide and sample.
3. Locate the Pipeline, run review explicitly, and explain the final OK/NG
   result using one overlay and one metric gate.
4. Select one accepted and one rejected Blob candidate and explain the table or
   drawing evidence.
5. Open the related Learn topic, identify where the Blob parameters are edited,
   and return without unintentionally running or changing routing.
6. Return to the Recipe summary and identify the next action.

### Advanced task: run after the core task

Use a temporary duplicate Recipe so the participant cannot alter retained
evidence.

1. Find the local Validation Set surface.
2. Explain expected outcome, actual outcome, judgment correctness, and execution
   error as separate concepts.
3. Register the existing Good/Bad pair with explicit roles and run the suite.
4. Open the saved run and identify any failed or rejected row.
5. State why a Qualified Snapshot requires the exact completed set run, scope,
   operator note, and clean saved Pipeline.
6. If preflight is green, create and verify a temporary Snapshot, then create a
   working copy and explain why the copy is not itself qualified.

### Facilitator rules

- Do not name the next button or tab.
- Answer only safety questions until the participant is blocked for two minutes.
- Record the first place assistance is required and the participant's stated
  mental model before explaining the product model.
- Do not count a successful click as understanding; require an evidence-based
  explanation.
- Do not reuse or modify a production Recipe, qualified archive, or external
  image folder.

### Observation sheet

| Field | Record |
| --- | --- |
| Participant ID / prior vision experience | |
| Runtime path, source revision, and date | |
| Core task completed without help | Yes / No |
| First assistance point | |
| Unintended Preview/Run or route/layer change | |
| Correctly explained Good/Bad intent | Yes / No / Partial |
| Correctly used overlay plus metric gate | Yes / No / Partial |
| Correctly distinguished expected/actual/judgment/error | Yes / No / Partial |
| Correctly distinguished working Recipe/qualified Snapshot | Yes / No / Partial |
| First blocking defect or confusing label | Exact screen, action, screenshot, and words used |
| Completion time | |
| Proposed smallest corrective change | Leave blank until results are compared |

### Implementation trigger

- Fix immediately when the current build produces a reproducible crash,
  inaccessible action, data loss, unintended execution, routing/layer mutation,
  or incorrect evidence.
- Select a bounded UX correction when at least two of the first three
  independent participants fail the same transition or form the same incorrect
  safety/evidence model without prompting.
- Treat one isolated hesitation or preference as observation, not feature
  authorization.
- Re-run only the affected task and the explicit Preview/Run, layer, routing,
  evidence, and accessibility contracts after a correction.

## Verification

Command:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
```

Result: PASS, 0 warnings, 0 errors.

Fresh current-source screenshot-smoke targets:

```text
wpf_shell_host_workspace_sample_picker=OK
wpf_shell_host_recipe_manager_summary=OK
wpf_shell_host_workspace_sample_pipeline_review_metrics=OK
wpf_shell_host_recipe_local_validation_set=OK
wpf_shell_host_recipe_qualified_snapshot=OK
```

Each target reported `check=OK`, `layout=0`, `text=0`, and `internal=0`.
The qualification evidence also retained:

```text
PayloadIntegrityValid=True
RuntimeFingerprintMatches=True
WorkingCopyHasQualification=false
CancelledSupersedeNoChange=true
PreviewRunCountUnchanged=true
LayerCountUnchanged=true
WorkspaceLayerUnchanged=true
RoutesUnchanged=true
```

Evidence folder:

`artifacts\first_time_operator_journey_audit_20260727\current`

## Completion Record

Status: Complete

Scope: Current-source first-time operator journey audit plus a reusable
independent participant protocol; no product implementation.

Acceptance criteria:

- Product identity, commercial lessons, and platform exclusions are explicit:
  pass.
- Sample -> Learn/teach -> Recipe -> explicit Run Review -> Validation Set ->
  Qualified Snapshot path is traced to current source and current captures:
  pass.
- Current build and the five relevant UI smokes pass: pass.
- A concrete implementation is selected only if a blocker is reproduced: pass,
  no blocker reproduced and no implementation selected.
- The remaining novice-comprehension evidence gap has an executable protocol:
  pass.

Verification: Debug solution build and five current-source WPF screenshot smokes
listed above.

Evidence: this report and
`artifacts\first_time_operator_journey_audit_20260727\current`.

Boundary / next dependency: independent first-time participants. Until their
observations reproduce a blocker, the project has no active feature priority.
