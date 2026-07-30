# OpenVisionLab Next Development Decision — P272

Date: 2026-07-30 KST
Status: Complete — reproduced defects admitted and corrected
Immediate priority: Recipe/Pipeline persistence failure audit
Recommended model: `gpt-5.6-terra`
Reasoning effort: `medium`

## Decision

The next bounded OpenVisionLab development task is:

> **P272 — Recipe/Pipeline Persistence Failure Audit and, only when a
> current operator-visible defect is reproduced, the smallest fail-closed
> feedback correction.**

P272 begins with `VisionPipelineStorage` and `RecipeDataStorage`. It is an
audit-first task, not permission to implement a generic persistence framework,
schema migration system, new settings UI, or broad project rewrite.

If the audit does not reproduce silent fallback, reopen loss, wrong evidence,
or another operator-visible storage defect, P272 closes as an audit report
without changing production code.

## Decision Basis

This decision incorporates the operator-provided
`OpenVisionLab_룰베이스_상용성_분석_및_Codex_감사_개발_프롬프트.docx`,
reviewed in full on 2026-07-30. That review used public/original main
`2586e92c762e4cf16cc9f688bdf7b9529fe8c7f8`, which matched the original
repository HEAD at review time. Dev was ahead at
`f0abe8385a734fc2762a265b15b17a541c279ffd` and already contained the P271
completion evidence.

The current Dev source, current handoff, stable contracts, and current runtime
evidence take precedence if a later audit conflicts with the supplied report.

## Why This Is The Immediate Priority

OpenVisionLab's product value is the repeatable operator workflow:

`sample -> PropertyGrid teaching -> explicit Preview/Run -> result evidence
-> Good/Bad or N-image validation -> Run History -> Qualified Snapshot`.

That workflow depends on the saved Recipe and Pipeline being the same
definition the operator taught, reviewed, reopened, and qualified. A silent
default Pipeline, lost Recipe data, or save result that appears successful
when only memory changed can invalidate downstream evidence even when every
algorithm executes correctly.

P269 through P271 already reproduced and corrected this class of defect in
the Direct PropertyGrid and native Tool settings stores. P272 checks the
higher-impact Recipe/Pipeline boundaries for the same risk before selecting
another algorithm or UI feature.

## Current Product Maturity Context

This decision does not treat commercial readiness as one percentage:

| Area | Current evidence-based state | Priority consequence |
| --- | --- | --- |
| Product identity and local workbench | Strong: the sample, PropertyGrid teaching, explicit Run, review, validation, history, and Snapshot lifecycle is coherent. | Preserve this workflow instead of pursuing generic SDK breadth. |
| Recipe/Pipeline review and validation evidence | Strong, but its credibility depends on exact persistence identity. | Audit the saved definition before adding another inspection feature. |
| Beginner independence | Internally rehearsed but externally unproven. | Keep CVR-00 blocked on three real participants; agent recordings do not close it. |
| Matching, fixture, and metrology | Useful bounded pixel/synthetic/public-sample workflows exist; production and calibration claims remain limited. | Admit expansion only from a named physical task and evidence packet. |
| Release, recovery, performance, and SDK delivery | Partial and not equivalent to a supported commercial distribution. | Open a separate Productionization track only after an explicit distribution decision. |
| Camera, lighting, PLC, MES, controller, and account platform | Deliberately outside the product boundary. | Do not use those gaps to reorder the current workbench priority. |

P272 therefore improves the trust boundary of the product OpenVisionLab
already is. It does not attempt to make OpenVisionLab a commercial SDK,
industrial controller, or certified metrology platform.

## Current Source Evidence

The current source proves an audit candidate, not a completed defect
reproduction:

- `Core/Pipeline/Storage/VisionPipelineStorage.cs`
  - `Load` calls `SerializeHelper.LoadOrCreateXmlFile(..., out _)` and discards
    the load disposition.
  - `Save` writes through `SerializeHelper.SaveXmlFile`, while its many callers
    do not share one operator-visible save/recovery contract.
- `Core/Recipe/RecipeDataStorage.cs`
  - `Load` also discards the load disposition.
  - `Save` does not itself publish memory-only or reopen-loss state.
- `Common/SerializeHelper.cs`
  - distinguishes `Loaded`, `CreatedDefaultForMissingFile`, and
    `ReplacedInvalidFile`;
  - moves an invalid original to an exact timestamped backup before writing a
    default;
  - writes through a temporary file and replaces or moves it into place.
- `Core/Recipe/RecipeRuntimeStorage.cs` and `Core/State/RecipeState.cs`
  - are part of the Recipe switch/load/save path and must be observed for
    error propagation, current in-memory state, and operator feedback.

The static risk is that a higher-level caller can receive a default object
without knowing whether this was normal first use or replacement of an
existing invalid definition. The audit must prove the actual current-build
operator effect before any correction is admitted.

## Concrete Outcome

P272 is complete only when one of these two outcomes is recorded:

1. **No defect reproduced**
   - every required case is executed;
   - current behavior and evidence are recorded;
   - production source remains unchanged;
   - the next priority returns to an external or named admission gate.
2. **Defect reproduced and corrected**
   - the exact operator path and storage owner are identified;
   - only the reproduced boundary is corrected;
   - current before/after evidence and focused regressions pass;
   - the operator cannot mistake substituted defaults or memory-only state for
     a successful persisted Recipe/Pipeline.

## Included Scope

- canonical Pipeline XML used by Recipe Manager, Pipeline Review, Direct
  teaching append/save, application reopen, and explicit Run Review;
- Recipe `DataState` load/save during Recipe selection, reopen, and explicit
  persistence;
- existing active Recipe/Pipeline/Step identity and pending-edit transition;
- invalid-source backup, default substitution, save failure, and subsequent
  recovery feedback;
- Korean and English operator text when a visible correction is required;
- Tooltip and accessibility HelpText for any ellipsized status;
- exact no-side-effect checks for Preview/Run, layers, active layer, workspace
  selection, and Pipeline routes.

## Excluded Scope

- new algorithm families or parameter tuning;
- OCR, Barcode, calibration, deformable matching, Region Algebra, or generic
  expression engines;
- camera, lighting, PLC, I/O, MES, account, cloud, or controller runtime;
- LLM provider, prompt, or browser-automation expansion;
- generic persistence engine, database, autosave daemon, or new modal dialog;
- broad MVVM/folder restructuring;
- schema/version migration unless a syntactically valid but semantically stale
  payload is separately reproduced and a bounded compatibility rule is
  approved;
- changing explicit Preview/Run or layer/route contracts.

## Required Reproduction Matrix

| ID | Case | Required observation |
| --- | --- | --- |
| R1 | Missing file, normal first use | Distinguish intended default creation from loss of an existing definition; no warning noise for a valid first-use path. |
| R2 | Valid file | Restore exact Recipe/Pipeline/Step identity, order, parameters, routes, and applicable DataState. |
| R3 | Malformed XML | Record whether the original is preserved, whether a backup is exact, whether a default is substituted, and what the operator sees. |
| R4 | Deserialization-incompatible XML | Separate a parseable-but-wrong contract from normal first use and prevent a substituted definition from appearing qualified. |
| R5 | Unreadable or permission-denied source | Record whether the source changes, which in-memory state remains, and whether the operator receives the full cause and path. |
| R6 | Disk-full or save exception | Distinguish memory state from disk state and state the reopen-loss risk without claiming save success. |
| R7 | Partial or truncated existing file | Verify invalid backup/default behavior and ensure partial content is not silently accepted as the taught definition. |
| R8 | Existing-file replace failure | Verify that a previously valid file remains usable and is not damaged by a failed temporary-file replace. |
| R9 | Successful save after failure | Clear retained failure state only after a verified successful save and report recovery once, not on every ordinary save. |
| R10 | Syntactically valid but semantically stale payload | Record the current detectability boundary; do not invent schema/version semantics to make this case pass. |

For every case, retain:

- source and resulting file identity or SHA-256 where meaningful;
- exact backup path and hash when a backup is created;
- whether the existing file changed;
- loaded in-memory identity versus persisted disk identity;
- default-substitution state and operator-visible message;
- whether Import, Save, Preview, Run, or reopen remains allowed;
- Preview/Run count;
- layer count and active layer;
- workspace selection and input/output routes;
- selected Recipe, Pipeline, and Step identity;
- pending-edit state;
- next successful save and recovery state.

## Required Operator Workflows

### Workflow A — Existing Pipeline reopen

1. Save a non-default multi-Step Pipeline with distinctive names, parameters,
   and routes.
2. Close and reopen the application or reload the Recipe.
3. Verify exact WAIT-state restoration before explicit Run.
4. Replace the stored Pipeline with each required failure case.
5. Observe Recipe Manager, Pipeline Review, and Run eligibility without
   changing the file again.

### Workflow B — Direct teaching append/save

1. Teach one existing Tool through PropertyGrid.
2. Add and save it to a named Pipeline.
3. Force the storage failure at the actual save boundary.
4. Verify that the in-memory Step and disk Pipeline are visibly distinguished.
5. Reopen and confirm whether the Step was retained or lost.

### Workflow C — Recipe DataState

1. Save distinctive Recipe data that can be checked exactly after reopen.
2. Exercise valid, invalid, unreadable, and save-failure cases.
3. Verify that Recipe selection does not present substituted defaults as a
   successful restoration.
4. Verify that unrelated Tool settings and the selected Recipe identity do not
   hide the Recipe-data failure.

### Workflow D — Recovery

1. Start from a retained load or save failure.
2. Correct only the external file-system condition.
3. Perform one explicit successful save.
4. Confirm one recovery message and exact reopen restoration.
5. Confirm that later ordinary saves do not repeat stale recovery warnings.

## Implementation Admission Rule

A production correction is admitted only when all of the following are true:

1. a current source/build reproduces an operator-visible silent fallback,
   memory-only save, reopen loss, wrong evidence identity, or unintended side
   effect;
2. the exact storage owner and UI consumer are identified;
3. the smallest correction can preserve existing first-use and valid-file
   behavior;
4. the correction has an explicit current-build acceptance test;
5. it does not broaden product scope or change Preview/Run/layer/route
   contracts.

Static suspicion alone is not enough.

## Minimum Correction Contract If A Defect Is Reproduced

- Normal missing-file first use remains a quiet, visible, editable default.
- Valid files restore without warning.
- An invalid or incompatible existing file retains an exact backup before any
  replacement.
- Default substitution is named explicitly; it cannot look like the operator's
  prior taught/qualified Pipeline.
- Run/import/qualification paths fail closed until the substituted definition
  is explicitly reviewed or replaced, when the reproduced workflow requires
  that gate.
- Unreadable-load feedback states that the saved file was not changed when
  that is true.
- Save failure retains the current in-memory edit but names the disk state and
  reopen-loss risk.
- A failed replace does not damage the previous valid file.
- The next verified successful save clears the failure and reports recovery
  once.
- Ordinary successful loads/saves do not flood the UI.
- Full Korean/English reason, path, Recipe/Pipeline context, Tooltip, and
  accessibility HelpText remain available.
- Feedback and recovery cause zero Preview/Run, layer, active-layer,
  workspace-selection, or route changes.

The result should be owned by the Recipe/Pipeline persistence domain and
presented through existing status surfaces. Do not create a new framework or
modal dialog solely to mirror P269-P271.

## Verification Gate

Audit evidence:

- all R1-R10 rows executed against both storage boundaries where applicable;
- source/disk/in-memory identity table;
- current behavior report and decision;
- exact commands, exit codes, and artifact paths;
- no production diff when no defect is reproduced.

Additional evidence when code or UI changes:

- focused storage unit/smoke coverage for every reproduced state;
- current Debug solution build:
  `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`;
- current readiness check;
- current Debug EXE before/after evidence for visible feedback;
- Korean/English, Tooltip, and accessibility assertions;
- Recipe switch, Pipeline reopen, Direct teaching append/save, Run Review, and
  pending-edit regressions;
- zero Preview/Run/layer/route/workspace side effects;
- final tracked diff review.

Planned evidence root:

`artifacts\p272_recipe_pipeline_persistence_audit_20260730`

## Acceptance Criteria

| ID | Acceptance |
| --- | --- |
| A1 | Missing, valid, malformed, incompatible, unreadable, save-failure, and recovery states are distinguishable. |
| A2 | An invalid original is preserved by an exact backup or an equally strong verified recovery mechanism before replacement. |
| A3 | Save failure states the in-memory-only and reopen-loss risk in the correct Tool/Recipe/Pipeline context. |
| A4 | Successful recovery is reported once and ordinary success does not overwrite a useful warning. |
| A5 | Failed temporary write or replace does not corrupt the previous valid file. |
| A6 | Load, restore, and error feedback cause zero Preview/Run. |
| A7 | Layer count, active layer, workspace selection, and routes remain unchanged. |
| A8 | Recipe/Pipeline/Step identity and pending-edit guards remain intact. |
| A9 | Korean/English message, Tooltip, and accessibility HelpText provide the full reason. |
| A10 | Current Debug build, focused smokes, readiness, and actual-EXE evidence pass when a visible correction is made. |
| A11 | Semantically stale but syntactically accepted data is documented as undetectable when no schema/version rule exists. |
| A12 | If no defect is reproduced, production code remains unchanged and the audit report closes the task. |

## Ordered Priorities After P272

1. **CVR-00 independent novice validation**
   - Prerequisite: three real independent first-time participants and their
     unedited raw observations.
   - Agent-operated recordings remain facilitator/development rehearsal only.
   - Recommended model: none before observations; `gpt-5.6-terra` for
     synthesis afterward.
   - Reasoning effort: none before observations; `low` afterward.
2. **Productionization track, only after an explicit distribution decision**
   - Reproducible Release build and full CI gate first; then atomic recovery,
     installer/signing/update/uninstall, diagnostic support bundle,
     performance criteria, and SBOM/licensing.
   - Recommended model: none before the product/distribution decision;
     `gpt-5.6-terra` for the first bounded audit afterward.
   - Reasoning effort: none before the decision; `medium` afterward.
3. **Algorithm expansion, only after a named admission packet**
   - Requires a named operator task, reproducible current-tool failure,
     Good/Bad/held-out evidence, metrics, acceptance, and physical tolerance
     ownership.
   - OCR/Barcode, calibration, deformable/anisotropic matching, Region
     descriptors/algebra, and derived expressions are not automatically active.
   - Recommended model: none before the packet; `gpt-5.6-sol` for an approved
     high-risk matching/metrology/calibration task.
   - Reasoning effort: none before the packet; `high` afterward.

## Commercial Lessons And Product Boundary

OpenVisionLab should continue to emulate:

- Cognex: visible Tool/image/result/fixture relationships and reusable setup;
- Euresys: prototype-to-integration clarity and measured runtime;
- HALCON: explicit signal, residual, frame, unit, and failure diagnostics;
- MERLIC: bounded operator assistance without automatic execution;
- Aurora/MIL: release provenance, compatibility, performance, and support
  discipline.

It should not claim or automatically pursue:

- parity with a broad commercial Vision SDK;
- camera, lighting, PLC, MES, controller, or account platform scope;
- certified calibration or production metrology from pixel/uniform-scale
  evidence;
- autonomous beginner usability without CVR-00;
- production robustness without physical data and field qualification;
- a new algorithm only because a commercial product contains it.

## Completion Record

Status: Complete
Scope: P272 reproduced and corrected silent active-Pipeline default
substitution, unreadable Recipe-data load propagation, memory-only save
feedback, failed-replace integrity, fail-closed Recipe execution, and
one-time recovery.
Acceptance criteria: A1-A11 passed. A12 is not applicable because current
operator-visible defects were reproduced and satisfied the implementation
admission rule. R10 remains explicitly undetectable without a schema/version
rule.
Verification: R1-R10 Pipeline/Recipe-data matrix, current Debug EXE
Korean/English before/after, Tooltip/accessibility, Direct Tool save
failure/retry, Recipe Pipeline/pending-edit round trip, P269-P271 regressions,
standard Debug build, readiness, and patch hygiene.
Evidence:
`artifacts\p272_recipe_pipeline_persistence_20260730` and
`docs\reports\OPENVISIONLAB_RECIPE_PIPELINE_PERSISTENCE_FEEDBACK_20260730.md`.
Boundary / next dependency: CVR-00 requires three independent first-time
participants. Productionization requires an explicit distribution decision;
algorithm expansion requires a named admission packet.
