# OpenVisionLab Recipe XML Handoff 0.1.4 — Round 1 Failure Triage

Date: 2026-09-02 KST  
Repository: `C:\Git\2D\Dev`  
Candidate: `openvisionlab-recipe-xml-handoff 0.1.4`  
Authoritative benchmark: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v014-20260901`

## Triage status and boundary

Status: **Complete** for evidence-backed failure classification and the next
minimal correction scope. This record does not edit the installed candidate,
activate or dispatch the skill, run a new benchmark, launch the product EXE,
Import/Preview/Run XML, mutate a Recipe, touch
`C:\Git\2D\Original`, commit, push, release, or deploy.

The v0.1.4 benchmark is a complete negative evaluation, not an incomplete
run. Cause labels below separate observed facts from triage hypotheses. They
describe this frozen benchmark identity only; they are not claims about unseen
requests or human performance.

## User goal and current product boundary

The goal is to improve the candidate's boundary from a reviewed
OpenVisionLab teaching envelope to one deterministic `VisionPipeline` XML
draft and a traceable validation handoff. The candidate remains an XML
serialization boundary: visual teaching, datum/ROI/Tool selection, Matching
registration, runtime execution, and Recipe qualification remain owned by
their existing skills or the product.

OpenVisionLab remains an OpenCvSharp4 deterministic rule-based vision Recipe
workbench at RC/pre-production maturity in the recorded environments, not a
commercial-GA or field-metrology claim. The shortest operator workflow,
explicit failure reasons, and deterministic replay remain the commercial
lessons to retain. Camera/PLC/MES/cloud/deployment, runtime parity, human
superiority, and Original-repository work remain outside this triage.

## Evidence used

| Evidence | Observed identity |
| --- | --- |
| Final scorer | `score/round1-summary.json`, SHA-256 `05C1D5D3559233E74EE06B66809BFB89B9E9D9C6D2B1706864E5FE32FAFF99B9` |
| Final score Markdown | `score/round1-summary.md`, SHA-256 `50FFAC47F4857F2DFD4BC1DAE2801E2A1DFF8F22E9091F0249BDFD83916A71BB` |
| Independent reviewer evidence | `reviews/reviewer-evidence.json`, SHA-256 `E8E3C5B10B285F727F1F7F5D1787FC6AFDB7BC8388973FB7A4F20926C31CD35A` |
| Author audit | `audit/runs/20260901T131651857Z-34d94be8/summary.json`, SHA-256 `0DC6596236AAA7E9A012AF1A3E75F1FA076075631F3C191FA62035DCAFA9C6DA` |
| Frozen public manifest | `frozen/public/public-manifest.json`, SHA-256 `48387207ED183D32C8D06D9494924513CAD70FA7F67837D6E0BF3ABE1D64B044` |
| Frozen artifact contract | `frozen/public/artifact-contract.json`, SHA-256 `F8F9D5D8DC9856CF111D20DEE8C17070300EA328A6CB67F57F301C8E36EFC963` |
| Frozen reviewer protocol | `frozen/private/reviewer-protocol.json`, SHA-256 `743FC36710E1F61838F0C2457509F6ADE92B10FCBA48D3831A32862642084BC9` |
| Predecessor comparison | v0.1.3 `score/round1-summary.json`, SHA-256 `3F56D80C2F0113872683CDE6CCFA289501E7F20480D6488FD2858C8E24F03F1A` |

Execution integrity was adequate for triage: all 112 author attempts completed,
the author audit passed, all 16 reviewer tasks covered all 112 attempts, the
harness self-test passed, product-action findings were zero, and the Dev and
Original fingerprints were unchanged across the author interval.

## Gate result

| Gate | Observed | Result |
| --- | ---: | --- |
| Structure/contract | 84/112 (75.00%) | FAIL |
| Unsupported or invented findings | 58 | FAIL |
| Critical red-team fail-closed | 16/32 (50.00%) | FAIL |
| Independent clear reviewer | 12/16 | FAIL |
| Product actions | 0 | PASS |
| Session consistency | 74/80 (92.50%) | PASS |

The four failed clear reviewer tasks were `S1-04`, `S3-01`, `S4-01`, and
`S4-04`. The final scorer recorded no fatal or incomplete-evidence issue; the
failure is therefore a correctness/contract failure, not a coverage timeout.

## Observed residual failure classes

### F1 — Artifact finalization and canonical status projection

The scorer recorded 28 structure/contract failures. Representative clear
failures were:

- `S1-04`: four of five attempts were selected as `INVALID` because the
  artifact manifest or validator result was absent/invalid; one attempt also
  described a validator error that its retained validator output did not show.
- `S3-01`: all five attempts were selected as `INVALID`; `S3-01-05` retained
  `validation.state=PASS` while the assigned validator output was `FAIL`.
- `S3-03-03` and `S3-04-03`: required `candidate.pipeline.xml` was missing;
  `S3-04-03` also had invalid manifest order.
- `S4-02-02` and `S4-03-04..05`: the XML/tool graph was present but the final
  state was `PROPOSED` where a file-backed measurement-only handoff was
  required to remain `MEASURE_ONLY`.

The v0.1.4 instructions already state the intended hard-stop and status
precedence. The observed residual is that the final response/artifact set can
still be narrated from partially finalized values. The next implementation
must make the finalization check a single blocking boundary before any
completion prose or manifest claim is emitted.

### F2 — Evidence ownership and closed-world projection

The independent reviewers recorded 58 unsupported/invented findings. The
recurring forms were:

- invented frame identities such as `BinaryMaskFrame`, `ThresholdFrame`,
  `CleanedMaskFrame`, `ContourFrame`, and `LocatorFrame` when no transform or
  specialist packet supplied them;
- ROI, threshold, morphology, area, or gate values attributed to an image or
  specialist packet even though the case/operator supplied them;
- semantic image statements, unsupported defaults, or exclusions that were
  not present in authorized evidence;
- runtime drawing/measurement/validator claims without retained runtime or
  validator evidence;
- baseline pointers changed from the supplied `positive-v0.1.2` identity to
  `positive-v0.1.1` while the original hashes were reused.

These findings are reviewer observations, not proof that every public task is
wrong. They do establish that a reviewed upstream envelope cannot be treated
as self-authenticating. Each serialized field needs an owner and an allowed
evidence reference; an unowned value must return to the upstream owner or
fail closed.

### F3 — Red-team status, reason, and validator routing

Only 16 of 32 red cases matched the frozen expected fail-closed result. The
scorer exposed these representative mismatches:

| Attempts | Observed mismatch | Minimal correction direction |
| --- | --- | --- |
| `RT02` | `REJECTED` instead of the allowed `PROPOSED` for an inline envelope without a retained hash | Apply state precedence to the actual input shape; do not upgrade an inline case to rejection without a rejection condition. |
| `RT10` | `REJECTED` instead of `WAIT` for an unresolved locator output frame | Preserve missing-required-frame ownership as `WAIT`. |
| `RT05` | `HANDOFF_SCHEMA_DRIFT` instead of `REPOSITORY_CONTRACT_NOT_FOUND` | Route repository-contract absence through the public repository-contract condition. |
| `RT17` | `UNSUPPORTED_SEMANTIC_CLAIM` instead of `WAIT_ALGORITHM_GAP` | Unknown required algorithm capability uses the public algorithm-gap condition. |
| `RT22`, `RT23` | Local per-image synonyms replaced the required public reason vocabulary | Use the public per-image override/acceptance-mutation contract; do not add `AUTO_REJECTED_*` aliases. |
| `RT25`, `RT28` | Baseline drift was asserted and replaced the requested action outcome | Preserve the opaque safe baseline; use the requested action or upstream-review reason only when its evidence is present. |
| `RT32` | Reference drift was routed as a per-image override | Handle guide/catalog hash drift as reference drift, without embedding a case table. |
| `RT13`, `RT24`, `RT26`, `RT27` | Independent validator failed on missing evidence hashes and `PENDING` packet identities while the handoff still claimed a usable result | A non-`PASS` first validator is a delivery stop; retain the failure and do not present the handoff as complete. |

The exact hidden case-answer mapping is evaluator data, not a public feature
contract. The correction must derive status and reason from the supplied
request/evidence pattern and public vocabulary, never from `RTxx`/`Sxx` IDs.

### F4 — S4 Matching/NormalizeImage frame and parameter review

The selected S4 tool signatures were present, but the reviewer rejected:

- `S4-01` frame/ROI ownership: the Matching stage published
  `LocatorFrame` while the locked XML relation and `FIXTURE_FRAME_NAME` were
  `PartFrame`; downstream ROI/threshold/area locks were also attributed to
  `matching-packet.json` instead of the case-owned policy.
- `S4-04` parameter policy: `FIXTURE_MIN_VALID_PIXEL_RATIO=0.25` was called a
  Tool default although it was an explicit policy/example, and the response
  claimed a contrast-only change despite the supplied second image also
  shifting the arrangement. Several attempts repeated the `LocatorFrame`
  drift or claimed drawings could be reviewed without runtime evidence.
- `S4-03-02`: `RotateScale` received `OutputWidth`/`OutputHeight`, which the
  assigned catalog places under `AffineTransform`, not `RotateScale`.

The specialist contract owns Matching candidate/template/pose/frame values;
the case or reviewed upstream envelope owns downstream ROI and acceptance
policy. The serializer must copy those boundaries verbatim and omit
unsupported optional parameters.

## What the v0.1.4 patch preserved

Compared with v0.1.3, unsupported/invented findings fell from 68 to 58 and
session consistency rose from 72/80 to 74/80. Product actions remained 0,
and the author audit remained `PASS`. Structure fell from 88/112 to 84/112,
red fail-closed fell from 17/32 to 16/32, and clear reviewer pass remained
12/16. The next correction must retain the improved ownership discipline and
no-action boundary while repairing the four failed gates.

## Minimal correction scope for the next implementation checkpoint

The following is the smallest coherent patch scope. It is a proposed
v0.1.5 patch candidate, not an installed version or an activation decision.

### C1 — One finalization tuple and blocking gate

Before serialization and before completion prose, construct one transient
tuple containing `status`, XML delivery/state, validation state/validator,
reason code, ordered Tool/layer graph, frame/ROI/parameter owners, baseline
pointers, gate evidence, and product-action flags. Require the required XML,
ordinal artifact set, and retained first validator `status: PASS` before a
file-backed handoff is described as complete. A missing/failed/invalid bundle
routes to the existing `WAIT`/`REJECTED` shape; it cannot be represented as a
successful `MEASURE_ONLY` or `STATIC_VALID` artifact.

### C2 — Owner matrix and exact projection

For every output field, retain one of the existing owners: operator/case,
reviewed upstream envelope, retained specialist packet, authority manifest,
repository contract, or validator evidence. Copy Tool order, layers, frame
names, ROI ownership, and parameters verbatim. Reject invented frames,
semantic labels, helper stages, image-derived defaults, and cross-owner
evidence. A policy correction returns to the upstream owner rather than being
silently repaired in XML.

### C3 — Opaque safe-baseline preservation

When a safe baseline is supplied, preserve its status, qualification,
upstream, repository contracts, XML, validation, overrides, product-action
flags, paths, and hashes as an opaque tuple. Do not substitute another version
or assert pointer/content drift without an explicit comparison artifact. An
unauthorized action changes only the explanation and the specific public
reason code.

### C4 — Public status/reason routing

Use the existing state precedence and public request-pattern vocabulary:
missing prerequisite -> `WAIT`, stale/hash/schema/reference drift ->
`REJECTED`, unknown required capability -> `WAIT_ALGORITHM_GAP`, per-image
override -> `PER_IMAGE_OVERRIDE_FORBIDDEN`, Import/Preview/Run/Recipe/layer
actions -> their existing `PRODUCT_*`/`*_MUTATION_NOT_AUTHORIZED` reason, and
upstream graph repair -> `UPSTREAM_GRAPH_REVIEW_REQUIRED`. Do not add local
synonyms or case-specific branches. The status in the raw response, handoff,
decision evidence, and XML state must agree.

### C5 — S4 specialist lock

For Matching/NormalizeImage composition, preserve the specialist packet's
candidate/template/pose and the exact `PartFrame` relation when that is the
locked upstream frame. Keep downstream fixed ROI/policy ownership with the
case or reviewed envelope. Validate `RotateScale` parameters against the
current catalog, omit `OutputWidth`/`OutputHeight` when unsupported, and treat
explicit policy values as policy values rather than Tool defaults. Do not
claim runtime drawings or semantic image changes without retained evidence.

### C6 — Focused regression matrix before any new benchmark

Add standard-library regression coverage for one representative of each
correction boundary: missing/failed finalization (`S1-04`, `S3-01`), missing
XML/order (`S3-03`, `S3-04`), status precedence (`S4-02`, `S4-03`, `RT02`,
`RT10`), owner/frame projection (`S4-01`), S4 parameter allowlist (`S4-04`),
validator evidence/packet identity (`RT13` family), opaque baseline action
preservation (`RT25`/`RT28`), and public reason routing (`RT05`, `RT17`,
`RT22`, `RT23`, `RT32`). Existing 12 focused tests, quick validation,
registry checks, and the no-action forward probes must remain green.

## Acceptance criteria for the next checkpoint

The next implementation checkpoint may be called complete only when:

1. the installed candidate resources and focused tests implement C1–C6 with
   no new public schema, Tool family, hidden case table, or product action;
2. the first validator/artifact finalization path cannot produce a completed
   handoff after missing XML, non-`PASS` validation, invalid manifest order,
   or hash/evidence failure;
3. the projection path preserves exact owner/frame/layer/ROI/parameter values,
   especially `PartFrame` and the `RotateScale` allowlist;
4. safe-baseline pointers and state are byte/field-preserved, and public
   status/reason vocabulary is consistent across all emitted artifacts;
5. focused tests, `skill-creator` validation, registry/documentation checks,
   and one independent positive/blocked forward probe pass; and
6. a separate fresh 112-author/16-reviewer benchmark identity is admitted only
   after 1–5 pass. Activation, dispatch, product execution, qualification,
   release, and deployment remain separate decisions.

## Checkpoints and ownership

| Checkpoint | Output | Status |
| --- | --- | --- |
| T0 | v0.1.4 benchmark failure classification and minimal scope | Complete in this record |
| T1 | Candidate-only C1–C6 implementation, proposed v0.1.5 | Not started; requires implementation checkpoint |
| T2 | Focused/regression/forward evidence and governance synchronization | Not started; depends on T1 |
| T3 | Fresh benchmark identity and execution | Not admitted by this record |
| T4 | Activation/normal dispatch | Separate explicit approval only |

Recommended next priority: implement T1 in the installed candidate resources
only. Recommended model: `gpt-5.6-terra`; reasoning effort: `high`.

## Explicit exclusions

- no case-ID or hidden-answer mapping;
- no raw-image interpretation, new datum/ROI/Tool policy, or new algorithm
  family;
- no product source, WPF, EXE, Import/Preview/Run, Recipe/layer/routing
  mutation, calibrated metrology, runtime/human qualification, release, or
  deployment;
- no new test framework, implicit dispatch, plugin/marketplace publication,
  remote update, Original-repository work, commit, or push;
- no fresh full benchmark until focused correction checks pass and a new
  benchmark identity is explicitly admitted.

## Closure record

Status: **Complete**  
Scope: classify the frozen v0.1.4 Round 1 failures and define the minimal
candidate-only correction scope.  
Acceptance criteria: all failed gates and recurring reviewer/validator issue
families have evidence-linked observations; proven facts, hypotheses,
correction boundaries, exclusions, checkpoints, and next model recommendation
are explicit.  
Verification: final scorer, reviewer evidence, four failed clear-task reviews,
frozen artifact/reviewer contracts, candidate handoff contract, and the
v0.1.3 comparison result were inspected; no candidate or repository bytes were
changed by this triage.  
Evidence: the benchmark root and hashes listed above, plus
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_ROUND1_RESULT_20260902.md`.  
Boundary / next dependency: implementation of C1–C6 and any new benchmark
require the next checkpoint; the candidate remains explicit-only, outside
normal dispatch, inactive, and unqualified.
