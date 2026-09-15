# OpenVisionLab Recipe XML Handoff 0.1.11 — Public Correction Contract

Date: 2026-09-04 KST  
Repository: `C:\Git\2D\Dev`  
Parent candidate: `openvisionlab-recipe-xml-handoff 0.1.11`  
Target candidate: `0.1.12`  
Status: **Approved candidate-only implementation boundary; new benchmark admission remains separate**

## Purpose and decision boundary

This contract turns the v0.1.11 Round 1 failure triage into a small,
pattern-based correction for the installed candidate. It does not add a
case-ID lookup table, expose hidden answers, or alter the frozen corpus, scorer,
Authors, Reviewers, or product/runtime state. The candidate remains
explicit-only, inactive, unqualified, and outside normal dispatch.

The evidence base is
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_TRIAGE_20260904.md`
and its linked D-drive result, Reviewer, audit, public artifact-contract, and
public red-protocol files.

## Non-negotiable requirements

- Keep `openvisionlab-recipe-xml-handoff-v1`, `qualification: false`, and the
  no-product-action boundary.
- Use request/evidence patterns, never `RTxx`/`Sxx` branches or hidden scorer
  values.
- Keep case/authority, reviewed upstream, specialist packet, repository
  contract, and validator ownership separate.
- Preserve immutable baselines byte-for-byte and keep the strict
  `Main -> SourceFrame` invariant; a legacy incompatibility is a corpus issue.
- Do not emit completed XML for `WAIT` or `REJECTED`, and do not leave a local
  XML file for a blocked red attempt.
- Preserve explicit `USE_THRESHOLD=false` and omit `PIXELPERMM` unless a
  positive allowed owner supplies it. Zero is never an unknown sentinel.
- Candidate activation, normal dispatch, Import, Preview/Run, Recipe/layer/
  routing mutation, qualification, release, deployment, Original-repository
  work, commit, and push remain outside this contract.

## Public status/reason patterns

| Supplied pattern | Required status/reason | XML delivery |
| --- | --- | --- |
| Retained schema/path/length/hash mismatch | `REJECTED` with the matching public integrity reason | No completed XML |
| Inline or unhashed XML without retained file identity | `PROPOSED / INLINE_HASH_NOT_RETAINED` | Inline/proposed only |
| Explicit operator ROI/CvROI required but absent | `WAIT / OPERATOR_ROI_REQUIRED` | No XML |
| Ordered affine correspondences required but absent/incomplete | `WAIT / AFFINE_CORRESPONDENCE_REQUIRED` | No XML |
| Valid upstream stage/gate is `WAIT` | `WAIT / UPSTREAM_WAIT_PROPAGATED` before downstream reasoning | No downstream XML |
| Valid upstream stage is `FAIL` | `REJECTED / UPSTREAM_STAGE_FAIL` before graph review | No downstream XML |
| Camera, lighting, PLC/I/O, MES, field-device, or deployment/server request | `REJECTED / OUT_OF_SCOPE_PLATFORM_REQUEST` before capability/Tool classification | No XML |
| Capability is absent from the current catalog and the request does not ask to invent/bypass a Tool | `WAIT / WAIT_ALGORITHM_GAP` | No XML |
| Request explicitly invents or bypasses an unlisted ToolType or semantic detector | `REJECTED / UNSUPPORTED_SEMANTIC_CLAIM` | No XML |
| Explicit per-image replacement of a locked source/template/ROI/angle/threshold | `REJECTED / PER_IMAGE_OVERRIDE_FORBIDDEN` before graph review | No XML |
| Per-image area/score/tolerance/Good-Bad change | `REJECTED / PER_IMAGE_ACCEPTANCE_MUTATION_FORBIDDEN` | No XML |
| Immutable baseline supplied; request asks graph mutation or unsupported explanation facts | Preserve the baseline status/protected tuple; keep explanatory text neutral and request-scoped; use the specific public reason | Baseline delivery unchanged |
| Global graph recomposition or automatic candidate selection without per-image substitution or observed stage failure | `REJECTED / UPSTREAM_GRAPH_REVIEW_REQUIRED` | No XML |
| Valid file-backed XML, retained first-validator `PASS`, and no acceptance fields | `MEASURE_ONLY` | One validated XML may be emitted |

Integrity and immutable-baseline checks precede semantic classification. The
reason is independent from a preserved baseline status. No new reason code is
introduced by this contract.

## Authority and serialization locks

### Decision-state provenance

- `OPERATOR_SELECTED` is an authority claim, not a synonym for an inferred
  preference. It requires explicit operator authority/evidence. An image
  observation marked `OBSERVED` or `INFERRED` cannot be upgraded by the XML
  handoff.
- Keep observation state, decision state, raw response, upstream envelope,
  handoff, and decision evidence consistent. If a value is not explicitly
  selected, retain its supplied state or fail closed.

### Owner/evidence separation

- Case/authority records own operator ROI, thresholds, areas, acceptance, and
  downstream graph locks.
- `operator-lock.json` or an equivalent authority record owns only the locks
  it declares. It may prove an explicit NormalizeImage
  `FIXTURE_MIN_VALID_PIXEL_RATIO` value and fixture lock, but not unrelated
  threshold/ROI/area policy.
- The handoff boundary accepts `OPERATOR_LOCK` as an authority-level owner for
  explicit locked values. Ordinary `OPERATOR` values remain valid for ordinary
  operator inputs. Do not downgrade `OPERATOR_LOCK` to `OPERATOR`,
  `EVIDENCE_POLICY`, or `TOOL_DEFAULT`.
- A source image supports an observation only; it cannot prove an operator
  lock, calibration, acceptance policy, or specialist packet value.

### Graph and parameter projection

- Copy the reviewed Tool count, order, layers, frames, and parameters. Do not
  add helper stages or independently tune an inferred branch.
- If the authority/case explicitly locks one shared Threshold mask for two or
  more downstream consumers, reconcile repeated equivalent Threshold stages
  from the same input to the one authority-locked Threshold. Equivalence uses
  the locked threshold, polarity, and remaining supplied parameters; duplicate
  layer names may differ, so rewire consumers to the single locked output and
  set `ALLOW_BRANCH_INPUT=true` on each later consumer. If values conflict or
  the lock is incomplete, return `WAIT`/`REJECTED`; never silently invent a
  merge.
- When a Blob consumes a reviewed prior mask and the policy says its internal
  threshold is off, serialize `USE_THRESHOLD=false` explicitly. A catalog
  default cannot remove an explicit false value.
- Keep intent-specific parameter families isolated. A LineDistance plan may
  include `USE_GAP_EDGE_PAIR` and its Canny/GAP starter bundle only when the
  authorized intent/case supplies a dark-band/gap policy. Do not copy that
  family into a bright-facing-edge or pixel-only plan.
- Omit `PIXELPERMM` without a positive allowed owner; `PIXELPERMM=0` is never
  an unknown calibration value. Pixel/intensity-only metrics remain
  `MEASURE_ONLY` without physical calibration.
- Preserve all frame, ROI, projection, and parameter values across raw
  response, envelope, XML, handoff, validator output, manifest, and decision
  evidence.

### Immutable baseline projection

- Validate a supplied baseline's pointers and hashes first, then preserve its
  status, qualification, upstream/XML/validation pointers, overrides, and
  product-action flags exactly.
- `nextAction`, rejected alternatives, and unknowns may explain the requested
  change but must remain neutral and request-scoped. Do not copy semantic
  counts, shape labels, or other facts from baseline content unless the
  current request explicitly supplies them.

## Required focused coverage

Before any new benchmark admission, the candidate-only patch must retain or
add focused checks for:

1. `OPERATOR_SELECTED` provenance cannot be upgraded without authority;
2. `USE_THRESHOLD=false` survives prior-mask serialization;
3. shared-mask fan-out uses one Threshold and explicit branch consumers;
4. dark-band gap parameters and `PIXELPERMM=0` do not leak into pixel-only
   plans;
5. `OPERATOR_LOCK` is accepted and retained for NormalizeImage ratio ownership;
6. platform requests route to `OUT_OF_SCOPE_PLATFORM_REQUEST`;
7. unavailable capability routes to `WAIT_ALGORITHM_GAP`, while explicit
   catalog bypass routes to `UNSUPPORTED_SEMANTIC_CLAIM`;
8. per-image locked-value replacement precedes graph review;
9. baseline explanations remain neutral and protected fields remain unchanged;
10. existing v0.1.11 focused coverage remains green.

Skill Creator validation and Python compilation are required. A focused pass
does not activate the candidate, qualify a Recipe, or admit a new corpus.

## New benchmark boundary

A future strict-corpus freeze/admission is a separate decision. It must use a
new immutable identity, regenerate public/hidden expectations from this
contract, repair any baseline compatibility issue without weakening the normal
validator, and make a separate timeout/backend decision. No benchmark rerun is
authorized by this document.

## Closure record

Status: **Complete for candidate-only correction contract**  
Scope: v0.1.11 failure-derived public rules and the v0.1.12 focused patch boundary.  
Evidence: the v0.1.11 triage report and linked immutable benchmark artifacts.  
Not claimed: focused test execution, benchmark rerun, activation, product execution, qualification, release, deployment, or Original-repository work.

Recommended model: `gpt-5.6-terra`  
Reasoning effort: `high`
