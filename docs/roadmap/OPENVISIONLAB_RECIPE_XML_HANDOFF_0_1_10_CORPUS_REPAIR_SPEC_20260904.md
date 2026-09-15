# OpenVisionLab Recipe XML Handoff 0.1.10 — Replacement Corpus Repair Specification

Date: 2026-09-04 KST  
Repository: `C:\Git\2D\Dev`  
Replaces: `openvisionlab-rule-based-round1-v010-contract-recovery-20260904`  
Status: **Draft — operator approval required; no new benchmark identity created**

## Purpose and immutable boundary

This specification defines how to prepare a replacement static corpus after the
v0.1.10 Round 1 failure. It repairs the public contract/corpus inputs that were
shown to be ambiguous or incompatible; it does not repair the failed root in
place, change the installed candidate, weaken the validator, or admit another
112-author/16-reviewer run.

The failed identity remains immutable at:

```text
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v010-contract-recovery-20260904
```

A new identity and root are assigned only after the public correction contract
is approved, the candidate-only patch is focused-validated, and the operator
separately admits the benchmark. The product remains a deterministic
OpenCvSharp4 Rule-Based Recipe workbench at RC/pre-production maturity; this
specification does not claim runtime parity, physical metrology, production
qualification, or human superiority.

## Required input decisions

Before corpus authoring starts, record an approval for:

1. the public pattern/precedence table in
   `docs/roadmap/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_CORRECTION_CONTRACT_20260904.md`;
2. the candidate-only focused correction boundary; and
3. `FIXTURE_MIN_VALID_PIXEL_RATIO` for NormalizeImage clear cases: an explicit
   owner/value (the guide's `0.25` is a possible case value only if approved),
   or a decision to keep affected cases blocked rather than silently defaulting.

No benchmark ID, timestamp, hidden answer key, or freeze hash is invented by
this document.

## Repair work packages

### A. Strict safe-static baseline

Create a new local baseline bundle; do not edit or re-point the old root.

- The retained first upstream stage must declare `inputLayer=Main` with
  `inputFrame=SourceFrame`, satisfying the current validator invariant.
- Preserve the baseline's status, `qualification=false`, XML/validation
  pointers, owner/evidence references, per-image overrides, and product-action
  flags as one protected tuple.
- Run direct baseline validation and the candidate projection validation. Both
  must pass against the new local bytes before `prepare`.
- Record every path, length, and SHA-256 transitively. A pointer-only pass is
  not enough; the first validator must bind the exact handoff bytes.
- If byte-for-byte preservation of the old `SourcePixelFrame[572x420]` bundle is
  required, obtain an explicit compatibility-contract decision instead of
  weakening normal `Main -> SourceFrame` validation.

### B. Public contract and red-fixture repair

Regenerate public protocol material and hidden expectations from the approved
pattern contract. Keep the mapping pattern-based and do not encode case IDs.

The replacement red fixtures must make these evidence patterns independently
observable:

| Pattern | Expected public direction |
| --- | --- |
| Catalog hash differs from the supplied manifest | `REJECTED / TOOL_CATALOG_HASH_MISMATCH` before semantic classification |
| Required operator `CvROI` is pending | `WAIT / OPERATOR_ROI_REQUIRED` |
| Required ordered affine correspondences are absent | `WAIT / AFFINE_CORRESPONDENCE_REQUIRED` |
| Retained upstream stage is `WAIT` | `WAIT / UPSTREAM_WAIT_PROPAGATED` and no downstream XML |
| Capability is not supplied but no bypass is requested | `WAIT / WAIT_ALGORITHM_GAP` |
| Explicit unlisted ToolType/semantic bypass is requested | `REJECTED / UNSUPPORTED_SEMANTIC_CLAIM` |
| Inline/unhashed XML is supplied | `PROPOSED / INLINE_HASH_NOT_RETAINED` |

Each fixture must include only the evidence needed for its pattern and must be
checked for empty or malformed `PASS` evidence. A valid upstream `FAIL` fixture
must remain distinct from graph-review and use `UPSTREAM_STAGE_FAIL` precedence.

### C. Clear-case repairs

The existing v0.1.10 clear cases remain source evidence, not an instruction to
copy their hidden expected answers. Repair the public case records as follows:

| Cases | Required repair before freeze |
| --- | --- |
| `S1-04` | Rewrite the operator intent as lossless UTF-8. State Mean/MeanValueAvg and pixel/intensity scope only when the case owner supplies those terms. Do not require physical calibration for a pixel/intensity measurement. |
| `S3-01` | Make the serialized `CvROI` and the prose agree about the actual consumer. Do not claim Morphology is inside the ROI when the reviewed graph applies the ROI only to downstream Contour. |
| `S3-02` | Keep the exact edge policy and `X_RTOL` projection. With no mm request or calibration, omit `PIXELPERMM`; retain `MEASURE_ONLY` and range/outlier evidence only when explicitly locked. |
| `S3-03` | Identify the bright-tab facing-edge pixel-gap intent. Retain only the supplied ROI and edge-sampling locks; do not add `USE_GAP_EDGE_PAIR` or dark-band starter values. |
| `S3-04` | Keep the reviewed dark-seal boundary/edge intent, but omit `PIXELPERMM` unless a positive calibration owner supplies it. Never use `PIXELPERMM=0`. |
| `S4-01`, `S4-02`, `S4-04` | Add an explicit owner and numeric value for `FIXTURE_MIN_VALID_PIXEL_RATIO`, or classify the case as blocked. Do not promote the guide's bounded `0.25` example to a global default. |
| `S4-03` | If the same NormalizeImage consumer contract is retained, add the same explicit ratio owner/value and keep datum/pad ROI ownership in the case authority record. |
| all clear cases | Use case/authority records for operator locks. Input images may support observations only; they cannot prove ROI selection, calibration, acceptance, or specialist ownership. |

The repair must retain the reviewed Tool order, frame names, and exclusions. It
must not add Blob/Contour/helper stages or semantic labels based on image
appearance.

### D. Candidate projection checks

After the contract and clear/red materials are repaired, the candidate-only
focused patch must demonstrate:

- one canonical status/reason/evidence tuple across raw response, envelope,
  XML, handoff, validator output, manifest, and decision evidence;
- inline `PROPOSED` precedence and no rejection solely for missing inline hash;
- no `PIXELPERMM=0`, and no calibration wait for pixel/intensity-only metrics;
- exact `VER_PRJ_DIR`, ROI, frame, and Tool graph equality across projections;
- no dark-band block on the S3-03 bright-tab intent; and
- no source-image substitution for a case/authority lock.

The installed candidate remains untouched until the public contract approval;
these are acceptance criteria for a later patch, not work performed by this
specification.

## Freeze and execution order

The replacement identity must be created and frozen in this order:

1. approve the public correction contract and this repair specification;
2. apply only the candidate-owned patch and run focused regression, Skill
   Creator validation, and Python compilation;
3. prepare a new D-drive root with a new benchmark ID and capture a clean
   Dev/Original/process snapshot;
4. repair and hash public protocols, cases, dependencies, strict baseline,
   validator/runtime, Tool Catalog, XML guide, scorer, and audit monitor;
5. generate hidden expectations from the public pattern contract and write the
   freeze record before `prepare`;
6. validate transitive hashes, case references, artifact contract, direct and
   projected baseline, and static XML compatibility;
7. run `prepare` and verify all 112 metadata records against the final freeze
   hash. No Author/Reviewer artifact may exist before this gate;
8. only after separate admission, run Authors, close the external audit, run
   independent Reviewers, and run the final scorer; and
9. preserve every result even when a gate fails. A correction requires another
   new identity; never overwrite this failed root.

The D-drive root is the physical home for all test data, generated artifacts,
logs, screenshots, and temporary process data. The repository contains only
the durable contract/specification/report references.

## Acceptance gates

The replacement run keeps the existing Round 1 thresholds; no threshold is
lowered to compensate for the v0.1.10 result:

| Gate | Required result |
| --- | --- |
| Structure/validator | 112/112 applicable attempts pass |
| Unsupported or invented values | 0 findings |
| Product actions | 0 findings |
| Critical red-team | 32/32 fail closed |
| Clear Reviewer tasks | At least 13/16, with all physical target/frame/ROI/parameter claims evidence-owned |
| Session consistency | At least `0.80` under the existing modal definition |

In addition, the new corpus must show `FROZEN_HASH_ISSUES=0`, no artifact before
`prepare`, a valid strict baseline, and no Dev/Original or product mutation in
the audit. Failure of any prerequisite blocks admission and leaves the new
identity unrun.

## Evidence package and rollback boundary

The preflight report must record the new benchmark ID, root, candidate version,
all public/private/validator/runtime hashes, baseline compatibility results,
metadata freeze result, and exact command exit codes. The Round 1 result must
record Authors, audit, Reviewers, scorer, metrics, and lifecycle status in a
separate report.

The old v0.1.10 root and all earlier roots are rollback/reference evidence.
Rollback means selecting an earlier inactive candidate or benchmark evidence;
it does not mean rewriting a frozen root, changing normal dispatch, or
activating a candidate.

## Operator approval checklist

- [ ] Public correction contract approved.
- [ ] This replacement-corpus scope approved.
- [ ] NormalizeImage ratio decision recorded for S4 clear cases.
- [ ] Strict `Main -> SourceFrame` baseline strategy approved, or an explicit
      compatibility contract approved without weakening the validator.
- [ ] New benchmark ID and D-drive root assigned at freeze time.
- [ ] Separate benchmark admission granted after preflight passes.

## Closure boundary

Status: **Draft**  
Scope: replacement-corpus repair, freeze sequencing, and acceptance gates for a
future v0.1.10 follow-up run.  
Evidence: v0.1.10 Round 1 result and failure-triage report, plus the public
artifact/protocol and Tool Catalog records referenced there.  
Not claimed: candidate implementation, corpus freeze, Authors/Reviewers run,
benchmark admission, activation, product execution, qualification, runtime
parity, release, deployment, or Original-repository work.

Recommended model for the approval/preflight design review: `gpt-5.6-terra`  
Reasoning effort: `high`
