# OpenVisionLab CVR-00 Facilitator Packet

Date: 2026-07-29 KST

## Current Status

Status: **Blocked**

The product, public samples, participant-only task sheet, observation template,
and current-build screen baseline are ready. CVR-00 cannot complete until at
least three independent first-time participants perform the tasks and their raw
observations are retained.

Do not show this facilitator packet, the prior journey audit, smoke screenshots,
expected metric values, or product-answer tables to a participant. Show only:

`docs/reports/OPENVISIONLAB_CVR00_PARTICIPANT_TASK_SHEET_20260729.md`

## Study Question

Can an independent first-time operator use only the product's visible guidance
to complete the core Sample -> Recipe -> explicit result review path, explain
the evidence correctly, and then distinguish multi-image validation from a
qualified immutable snapshot?

This is a product-comprehension study, not a participant skill test.

## Participant Eligibility

Recruit at least three participants who:

- have not used OpenVisionLab;
- have not received an implementation or UI walkthrough;
- have not seen the facilitator packet, expected answers, or baseline captures;
- may have no vision experience or may have general machine-vision experience,
  but that prior experience must be recorded.

Use participant IDs such as `P01`, `P02`, and `P03`. Do not place personal names
in repository evidence. Obtain the participant's permission before recording
screen, voice, or identifiable information.

## Frozen Readiness Baseline

The 2026-07-29 preparation used:

```text
Repository: C:\Git\OpenVisionLab_Dev
HEAD: e64a9d0ba8f3d210be47ba7fb43143f56e321158
Observed worktree entry count: 58
Observed worktree-status SHA-256: 997922FB37637F41FFE5A8E2CFF2C485D5AD4D74D37A952134E61BB55BF0ECCC
Runtime: C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe
Runtime SHA-256: DB8D70F81B0DE5D961A60D68C11E9020C6C53E6C53C3F28FAADB8278B96D8B42
OpenVisionLab.dll SHA-256: B12E3BE75C36D418E59FA5A14F1708F286D3794155ED5F1E59A4CC2C3D254DBA
```

The worktree is intentionally recorded as dirty and must not be identified by
the Git commit alone. At every participant session, record the actual runtime
path and SHA-256 again. If the runtime hash changes, create a new study baseline
and do not silently mix the observations.

Frozen public evidence:

| File | SHA-256 |
| --- | --- |
| `Blob_Particles_Synthetic_OK.png` | `E4D9283FF041F8BCAA0B61E005CEB135B23DF2E87C3BA45C05C68CAA49920387` |
| `Blob_Particles_Synthetic_Sparse_NG.png` | `1293C3BF4C3439A158DA2EEE943DE7C68C467C66C872E88F6C067CFB911D4D26` |
| `Public_Blob_Particles.pipeline.xml` | `C246F7355D58387A3806F5B615FC2D672AEECDEAD04758D7CD3BB1C2D5B9C85F` |
| `OpenVisionLab.PublicSampleCatalog.csv` | `8F973B1AC27D0FB2EEA8793200638E51B1376283ABE356FA692F5E87ED836162` |

Current-source screen readiness evidence:

`artifacts/cvr00_participant_study_readiness_20260729/current_source`

The following individual targets passed with `check=OK`, `layout=0`, `text=0`,
and `internal=0`:

- `wpf_shell_host_workspace_sample_picker`;
- `wpf_shell_host_recipe_manager_summary`;
- `wpf_shell_host_workspace_sample_pipeline_review_metrics`;
- `wpf_shell_host_recipe_local_validation_set`;
- `wpf_shell_host_recipe_qualified_snapshot`.

## Pre-Session Setup

1. Verify the runtime path and SHA-256.
2. Verify the four frozen public evidence hashes.
3. Start from a closed application and a clean temporary session state.
4. Prepare one temporary duplicate Recipe named
   `CVR00_<ParticipantId>_<YYYYMMDD>`.
5. Do not pre-open the target sample, Learn topic, Recipe screen, Pipeline
   screen, Validation Set, or Snapshot screen.
6. Do not preload an expected metric or selected object row.
7. Keep production Recipes, qualified archives, and external image folders out
   of the session.
8. Give the participant only the participant task sheet.
9. Start a monotonic timer when the participant first controls the product.

## Facilitation Rules

- Do not name the next button, tab, screen, or expected value.
- Ask the participant to continue thinking aloud when they become silent.
- Answer safety questions, but do not answer workflow or product-model
  questions before the blocked threshold.
- Record the first visible hesitation and the participant's exact words.
- Treat a participant as blocked after two continuous minutes without
  meaningful progress.
- At the blocked threshold, record the current screen, action attempted, exact
  words, and elapsed time before giving the smallest possible hint.
- Record every hint verbatim.
- A successful click is not understanding. Require the evidence explanation in
  the task sheet.
- Do not correct terminology until the participant has stated their current
  mental model.

## Raw Observation Record

Create one unchanged copy of this section per participant.

```text
Participant ID:
Session date/time and timezone:
Prior vision experience:
Prior OpenVisionLab use: No
Recording permission and retained media:

Runtime absolute path:
Runtime SHA-256:
Repository HEAD:
Worktree-status SHA-256:
Temporary Recipe name:

Core start time:
Core completion time:
Core completed without help: Yes / No
First hesitation timestamp:
First assistance timestamp:
First assistance screen/action:
First assistance exact participant words:
Hint given verbatim:

Good/Bad intent explanation:
Overlay evidence explanation:
Metric/gate explanation:
Accepted-object explanation:
Rejected-object explanation:
Blob teaching/edit location explanation:
Returned to Recipe summary without help: Yes / No

Unintended Preview/Run:
Unintended layer create/delete/select:
Unintended active-layer change:
Unintended Pipeline route change:
Crash, data loss, inaccessible action, or false evidence:

Advanced start time:
Advanced completion time:
Advanced completed without help: Yes / No
Expected/actual/judgment/error explanation:
Working Recipe versus Qualified Snapshot explanation:
Snapshot/working-copy action outcome:

Repeated-setting request, exact words:
Most confusing label, exact words:
First blocking defect:
Screenshots or recording paths:
Facilitator factual notes only:
Proposed correction: LEAVE BLANK UNTIL THREE RECORDS ARE COMPARED
```

Preserve the raw record before writing a summary. Do not rewrite a failed task
as successful after assistance.

## Evidence Folder

Use a new task-local folder for the actual sessions:

```text
artifacts/cvr00_participant_study_<YYYYMMDD>/
  baseline/
    runtime-identity.txt
    sample-identity.txt
  P01/
    raw-observation.md
    captures/
  P02/
    raw-observation.md
    captures/
  P03/
    raw-observation.md
    captures/
  comparison-after-three.md
```

Do not copy personal names, unrelated desktop content, credentials, private
images, or production Recipes into the evidence folder.

## Decision After Three Participants

1. Preserve all three raw records before analysis.
2. Separate safety/integrity defects from comprehension friction.
3. Fix a reproducible crash, data loss, inaccessible primary action, unintended
   execution, route/layer mutation, or false evidence immediately.
4. Admit one bounded UX correction only when at least two of the first three
   participants fail the same transition or form the same incorrect
   safety/evidence model without prompting.
5. When repeated setup friction is causal, apply the persisted-setup contract:
   coherent first-use surface, explicit confirmation, narrowest persistence
   scope, visible restoration, reset/default, stale-state validation, and zero
   Preview/Run/layer/routing side effects.
6. Treat one isolated hesitation or personal preference as an observation, not
   feature authorization.
7. Re-run only the affected participant task and stable contracts after a
   correction.

## Durable Completion Record

```text
Status: Blocked
Scope: CVR-00 unbiased participant task sheet, facilitator protocol, raw observation template, frozen runtime/sample identity, and five-screen readiness evidence.
Acceptance criteria: Current build and five screen contracts pass; participant and facilitator material are separated; exact raw-observation and 2-of-3 decision gates are reusable.
Verification: Debug solution build passed with 0 warnings and 0 errors; five individual WPF screenshot-smoke targets passed; runtime and public sample SHA-256 identities were recorded.
Evidence: docs/reports/OPENVISIONLAB_CVR00_PARTICIPANT_TASK_SHEET_20260729.md; docs/reports/OPENVISIONLAB_CVR00_FACILITATOR_PACKET_20260729.md; artifacts/cvr00_participant_study_readiness_20260729/current_source
Boundary / next dependency: At least three real independent first-time participants and their unedited raw observations. No model synthesis or product implementation is authorized before those observations exist.
```
