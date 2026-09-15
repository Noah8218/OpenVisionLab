# OpenVisionLab Recipe XML Handoff 0.1.4 — Correction Contract

Date: 2026-09-01 KST  
Repository: `C:\Git\2D\Dev`  
Parent candidate: `openvisionlab-recipe-xml-handoff 0.1.3`  
Candidate version: `0.1.4` (`PATCH`, installed as an inactive candidate)  
Status: **Implemented — focused correction and forward checks PASS; fresh benchmark not run**

## User goal

Correct the v0.1.3 XML-handoff candidate so an already reviewed OpenVisionLab
teaching envelope is serialized only when its values are evidence-owned and
internally consistent, while preserving explicit operator control and the
existing `openvisionlab-recipe-xml-handoff-v1` schema.

This contract is the direct follow-up to the frozen v0.1.3 failure triage in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_3_TRIAGE_20260901.md`.

## Non-negotiable requirements

- Keep the candidate explicit-only, outside normal registry dispatch,
  `qualification: false`, and all product-action flags `false`.
- Do not reinterpret raw images, choose a datum/ROI/Tool policy, add a Tool
  family, or replace upstream specialist ownership.
- Preserve the existing machine-readable schema and exact product spelling.
- Never emit a completed XML handoff when the canonical decision ledger,
  validator result, required XML, artifact manifest, or evidence ownership is
  inconsistent.
- Preserve an explicitly supplied immutable safe baseline byte-for-byte and do
  not infer its contents or staleness from pointers alone.
- Use only the public reason vocabulary and the most specific reason supported
  by the request and evidence.
- No Import, Preview, Run, Recipe save/activation, layer/routing mutation,
  release, deployment, Original-repository work, commit, or push.

## Included correction scope

### C1. Canonical decision ledger

Add a transient authoring ledger (not a new public schema field unless a later
review proves one is required) with this minimum tuple:

```text
status
xmlArtifact.state / delivery
validation.state / validator
reasonCode (when a decision-evidence artifact is required)
tool signature and ordered layer graph
frame / ROI / parameter ownership
baseline mode and immutable pointer set
gate states and evidence references
product-action flags
```

Each non-null value must name one owner: operator input, reviewed upstream
envelope, retained specialist packet, repository contract, authority manifest,
or validator evidence. The handoff, XML, raw response, and decision evidence
must be projections of the same tuple. A disagreement is a failed attempt, not
a reason to pick the most convenient text.

### C2. Closed-world preflight

Before XML serialization, check every planned Tool, layer, frame, ROI,
parameter, semantic statement, and gate claim:

- exact source/evidence owner exists;
- frame and layer names are copied verbatim;
- no helper stage, Tool substitution, optional default, plausible value, or
  semantic image label has been added;
- a policy change returns to the upstream owner;
- missing required evidence returns `WAIT`; stale, mismatched, unsupported, or
  ambiguous evidence returns `REJECTED` or the exact existing algorithm-gap
  state.

The preflight must be neutral about image appearance. `input image`, `source
frame`, and supplied ROI are allowed; unsupported color/object/geometry labels
are not.

### C3. Validator and artifact hard stop

For file-backed output, the first handoff validator must pass on the exact
retained bytes before a completed handoff is described. Missing XML, failed
validator output, invalid static-compatibility links, or a manifest-set mismatch
must stop finalization. A blocked attempt must use the existing `WAIT` or
`REJECTED` shape and must not fabricate a candidate XML or a static pass.

The second bundle audit remains separate from the manifest-listed first
validator output to avoid a hash cycle.

### C4. Evidence-gated PASS

An upstream stage or gate may be `PASS` only when its exact evidence list is
non-empty and the referenced evidence is allowed, hash-valid, and sufficient
for that state. Empty evidence yields `NOT_REVIEWED` for an unreviewed claim or
`WAIT` when the missing evidence is a required prerequisite. No Preview/Run
means no runtime drawing/metric PASS.

### C5. Baseline-preserving action decisions

When a safe-static baseline is supplied, load and preserve its exact status,
qualification, upstream pointer, repository contracts, XML pointer, validation
pointer, overrides, and product-action flags. Do not describe pointer/content
drift unless an explicit comparison artifact proves it. An unauthorized action
changes only the explanation and uses the specific existing reason code.

### C6. Exact public reason vocabulary

Use the following request-pattern table as a public contract, not as a hidden
case-answer table:

| Request/evidence pattern | Required direction |
| --- | --- |
| Missing acceptance policy | `ACCEPTANCE_POLICY_NOT_SUPPLIED` and preserve `MEASURE_ONLY` when a valid baseline is supplied |
| Unknown required algorithm capability | `WAIT_ALGORITHM_GAP` |
| Per-image runtime override | `PER_IMAGE_OVERRIDE_FORBIDDEN` |
| Import request | `PRODUCT_IMPORT_NOT_AUTHORIZED` |
| Preview/Run request | `PRODUCT_EXECUTION_NOT_AUTHORIZED` |
| Recipe save/activation request | `RECIPE_MUTATION_NOT_AUTHORIZED` |
| Layer/routing mutation request | `LAYER_MUTATION_NOT_AUTHORIZED` |
| Upstream graph requires review rather than downstream XML repair | `UPSTREAM_GRAPH_REVIEW_REQUIRED` |

The reason must agree with the canonical status and decision evidence. Do not
invent a local synonym such as `AUTO_REJECTED_PER_IMAGE_OVERRIDE`.

## Files allowed to change in the implementation checkpoint

Only the installed candidate resources and their focused regression tests may
change:

- `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\SKILL.md`
- `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\references\recipe-xml-handoff-contract.md`
- `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\scripts\validate_recipe_xml_handoff.py`
- `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\scripts\test_recipe_xml_handoff.py`

The registry remains candidate/explicit-only/outside normal dispatch. It may
move from `0.1.3` to `0.1.4` only after the installed resource hashes and the
focused/forward evidence below are recorded; activation is still a separate
approval.

## Explicit exclusions

- no new Blob/Contour/Line or semantic detector skill;
- no hidden answer/case-specific mapping or benchmark-only exception;
- no raw image inspection or image-derived facts;
- no product source, WPF, EXE, XML import, Preview/Run, Recipe qualification,
  calibrated metrology, release, deployment, or Original repository;
- no automatic dispatch, implicit invocation, marketplace/plugin publication,
  remote update, or new test framework;
- no fresh 112-author/16-reviewer benchmark before focused correction checks
  pass and a new benchmark identity is explicitly admitted.

## Acceptance criteria

### A. Focused implementation checks

1. Skill Creator `quick_validate.py` passes for the installed skill.
2. Existing focused regression tests pass, with new standard-library tests for:
   - status/XML/validation canonical-tuple mismatch;
   - empty-evidence `PASS` rejection;
   - invented frame/Tool/parameter rejection or fail-closed routing;
   - safe-baseline opaque preservation;
   - the eight public reason patterns above.
3. A file-backed positive measurement-only handoff passes the independent
   validator and exact bundle audit.
4. A failed-validator/missing-XML case cannot be presented as complete.
5. An immutable safe-baseline case preserves every protected pointer and state.

### B. Forward and boundary checks

Run one fresh realistic forward request and the nearest unsafe/out-of-scope
requests. Record raw request, actual output, verdict, and SHA-256 under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-v014-<date>`.
The evaluator must not receive the intended answer or this proposed fix.

Required observed boundaries: no product action, no implicit dispatch, no
case-specific hidden mapping, and exact owner/status/reason consistency.

### C. Documentation and governance checks

- update the candidate-focused report, registry pointer, handoff, plan, and
  documentation map/index only after the installed candidate actually changes;
- run registry validation, registry regression, documentation-index validation,
  JSON parse, scoped `git diff --check`, and final diff review;
- keep the candidate explicit-only and outside dispatch;
- do not claim promotion, qualification, or benchmark success from focused
  checks.

### D. Optional subsequent benchmark gate

Only after A–C pass and the operator explicitly admits a fresh identity, run a
new 112-author/16-reviewer Round 1 evaluation with the same frozen gate
thresholds. The benchmark must separately report structure, unsupported /
invented findings, product actions, critical red-team fail-closed, reviewer
task pass, and session consistency. The prior v0.1.3 root remains immutable
comparison evidence.

## Checkpoints and approval boundaries

| Checkpoint | Output | Status |
| --- | --- | --- |
| P0 | v0.1.3 triage and this contract | Complete in this turn |
| P1 | installed 0.1.4 resource edit and focused tests | Complete; 12 regression tests and Skill Creator validation PASS |
| P2 | focused/forward evidence and governance synchronization | Complete; local positive/blocked forward probes PASS and records synchronized |
| P3 | fresh benchmark identity and execution | Not admitted by this contract |
| P4 | candidate activation/dispatch | Separate explicit approval only |

## Recommended execution model

- P1 localized skill/validator edits: `gpt-5.6-terra`, reasoning `medium`.
- P2 documentation, registry, and command verification: `gpt-5.6-luna`,
  reasoning `low`.
- P3 benchmark orchestration, only if explicitly admitted: `gpt-5.6-terra`,
  reasoning `medium`.

## Closure record

Status: **Complete** for the v0.1.4 focused correction checkpoint.  
Scope: implement the bounded patch-level correction in the installed candidate,
exercise the validator/regression and forward boundaries, and synchronize
candidate governance records.  
Acceptance criteria: C1–C6 are represented in the skill/reference contract;
empty-evidence PASS, retained-validator failure, blocked XML, baseline drift,
exact projections, and public reason vocabulary have focused regression
coverage; positive and blocked forward probes preserve the no-action boundary.  
Verification: Skill Creator `quick_validate.py` PASS; 12 standard-library
regression tests PASS; Python compilation PASS; forward probe PASS for one
positive and one blocked request; registry, documentation-index, JSON, and
diff checks recorded in the focused report.  
Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_FOCUSED_VALIDATION_20260901.md`
and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-v014-20260901\forward-probe.json`.  
Boundary / next dependency: the candidate remains explicit-only,
outside normal dispatch, inactive, and unqualified. No fresh 112-author/
16-reviewer benchmark, promotion, activation, product execution, or
Original-repository work is claimed; a new benchmark requires separate
operator admission and identity.
