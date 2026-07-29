# OpenVisionLab User-Centered Workflow Direction

Date: 2026-07-29 KST

## Decision

OpenVisionLab development must start from the operator's inspection goal and
the shortest safe normal workflow. Screen, component, class, or storage
boundaries must not force the operator to configure one durable task through
unrelated views, dialogs, and buttons.

When related settings belong to one reusable workflow, the product must:

1. present them through one coherent first-use setup or option surface;
2. require explicit operator confirmation before they become reusable state;
3. persist them at the narrowest correct scope;
4. restore them visibly on the next equivalent use;
5. provide an explicit reset/default path;
6. reject or explain stale and incompatible saved state;
7. restore configuration only, without executing Preview/Run or mutating
   layers, active-layer selection, or Pipeline routing.

This direction applies to every admitted future feature. It does not activate
a new feature by itself.

## Product Direction

OpenVisionLab remains an OpenCvSharp4 deterministic rule-based vision recipe
workbench. Its normal operator workflow is:

```text
sample/operator image and inspection intent
  -> direct PropertyGrid teaching
  -> coherent reusable setup
  -> explicit Preview/Run
  -> layer/drawing/metric/object review
  -> N-image or labelled Validation Set
  -> Run History and deterministic review queue
  -> saved recipe or Qualified Recipe Snapshot
```

The existing LLM XML authoring path remains optional and frozen in maintenance
mode. It must not become a prerequisite for the workflow above.

## Current Evidence Baseline

The 16-video Cognex, HALCON, and MERLIC review and the post-review CVR work
support the following current direction:

- keep the image, selected Tool/Step, parameters, result, and evidence in one
  coherent context;
- keep locator/Fixture relationships visible;
- explain parameters with retained signal, distribution, model, candidate,
  object, and geometry evidence;
- preserve Good/Bad and N-image review;
- use task-specific suggestions only with explicit operator acceptance;
- keep editing, navigation, and result selection separate from execution.

`CVR-01` through bounded `CVR-11` already cover the selected signal,
distribution, matcher-diagnostic, suggestion, Fixture, multi-instance, and
global-polarity slices. `CVR-12` through `CVR-18` were audited but were not
admitted because their named operator/data packets do not exist.

The unresolved product evidence is not a generic missing-tool list:

- independent novice self-navigation has not been observed;
- CVR-09 and CVR-11 physical-task qualification packets are absent;
- later CVR rows remain conditional on their exact current-source trigger;
- production, field robustness, camera calibration, and equipment integration
  are not proven or are outside the product boundary.

## Persisted Setup Contract

### Scope selection

Choose the narrowest scope that matches the operator's expectation:

| Scope | Use when | Do not use when |
| --- | --- | --- |
| Tool | The setting belongs to one Tool instance or teaching model. | It changes another Tool or Recipe. |
| Recipe | The setup is part of one reusable inspection definition. | It should affect unrelated Recipes. |
| Project/workspace | The setting is intentionally shared by the current workspace. | It contains task-specific ROI, tolerance, template, or evidence identity. |
| User/application | The setting is a harmless personal display preference. | It is destructive, security-sensitive, safety-critical, or inspection-specific. |

### Required behavior

- Restored values remain visible and editable in the owning PropertyGrid or
  setup surface.
- The operator can reset to a documented default without deleting unrelated
  Recipe or workspace state.
- Saved ROI, template, tolerance, coordinate-frame, and dependency identities
  are validated before reuse.
- Stale state explains the exact incompatible source, image, Tool, Recipe,
  version, or dependency instead of silently falling back.
- A restored setup does not claim that Preview/Run evidence is current.
- No task-specific setting is silently shared across unrelated Recipes,
  projects, workspaces, or users.

### Required verification

Every implemented reusable setup must prove:

1. first-use configuration;
2. explicit confirmation;
3. save;
4. close/reload/reopen;
5. exact value restoration at the intended scope;
6. visible edit and reset;
7. stale/incompatible rejection when applicable;
8. zero unintended Preview/Run;
9. zero unintended layer creation/deletion/selection;
10. zero unintended Pipeline route change.

## Feature Admission Template

Use this template before implementing a new workflow or expanding an existing
one:

```text
Operator goal:
Current-source blocker:
Shortest safe normal path:
Related settings:
Current screens/dialogs/buttons:
Proposed coherent first-use surface:
Persistence scope:
Explicit confirmation:
Visible restored state:
Reset/default path:
Stale/incompatible behavior:
Preview/Run side-effect proof:
Layer/routing side-effect proof:
Good/Bad or N-sample evidence:
Completion boundary:
```

A candidate is not admitted when the operator goal, current blocker, or
evidence packet is missing. Commercial parameter parity is not a blocker.

## Current Development Direction

1. Collect independent novice observations through `CVR-00`.
   Prerequisite: at least three real first-time participants and unedited
   observations. No feature implementation is selected before that evidence.
   Recommended model: none before observations; `gpt-5.6-terra` for synthesis
   afterward | Reasoning effort: none before observations; low afterward.

2. Admit one bounded UX correction only if at least two of the first three
   participants fail the same transition or form the same incorrect mental
   model. The correction must use this persisted-setup contract when repeated
   setup friction is causal.
   Recommended model: `gpt-5.6-sol` | Reasoning effort: medium.

3. Qualify CVR-09 or CVR-11 only when its named physical operator/data packet
   exists. Do not create a convenient synthetic replacement.
   Recommended model: none before the packet; `gpt-5.6-sol` afterward |
   Reasoning effort: none before the packet; high afterward.

4. Audit or implement a later CVR row only after its exact trigger exists or
   the user explicitly selects it. Without an earlier admitted packet,
   `CVR-19` is the next conditional queue audit, not an implementation.
   Recommended model: none before a task exists; `gpt-5.6-sol` afterward |
   Reasoning effort: none before a task exists; high afterward.

## Commercial Lessons To Keep

- image-first Tool/Step/result context;
- progressive disclosure from teaching to advanced validation;
- visible Fixture and coordinate-frame relationships;
- retained signal and distribution evidence for parameter teaching;
- Good/Bad and image-sequence review;
- direct failure reason and next-action guidance;
- explicit operator acceptance of suggestions.

## Platform Scope To Keep Out

- camera acquisition and lighting control;
- PLC, I/O, MES, and industrial-controller workflows;
- account, SSO, electronic-signature, and regulatory platforms;
- installer, fleet, cloud, and deployment management;
- HALCON-style general scripting/debugging and tuple languages;
- autonomous AI classification or LLM-controlled inspection;
- arbitrary visualization scripting;
- camera/lens calibration or certified metrology without a separate explicit
  product decision and physical evidence.

## Durable Completion Record

```text
Status: Complete
Scope: User-goal-first workflow, coherent first-use setup, narrow-scope persistence/restoration, reset/stale-state behavior, side-effect verification, and evidence-gated future direction.
Acceptance criteria: Product direction stated; reusable setup contract and checklist recorded; current CVR state and next-priority gates recorded; commercial lessons and excluded platform scope separated.
Verification: Cross-checked against AGENTS.md, Product Target And Main Views, Stable Feature Contracts, the canonical CVR backlog, current handoff, and the 16-video commercial review evidence.
Evidence: docs/reports/OPENVISIONLAB_USER_CENTERED_WORKFLOW_DIRECTION_20260729.md
Boundary / next dependency: This documentation activates no feature. CVR-00 still requires three independent novice participants; physical and later CVR work requires its named packet or explicit user decision.
```
