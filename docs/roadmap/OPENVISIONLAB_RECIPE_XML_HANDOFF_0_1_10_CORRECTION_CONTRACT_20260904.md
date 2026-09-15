# OpenVisionLab Recipe XML Handoff 0.1.10 — Public Correction Contract

Date: 2026-09-04 KST  
Repository: `C:\Git\2D\Dev`  
Parent candidate: `openvisionlab-recipe-xml-handoff 0.1.10`  
Status: **Draft — operator approval required; not installed**

## Purpose and decision boundary

This document turns the v0.1.10 Round 1 failure triage into a public,
pattern-based correction contract for a future candidate-only patch and a new
benchmark identity. It does not add a case-ID lookup table, expose hidden
answers, or change the installed candidate, the v0.1.10 freeze, the validator,
the scorer, or any product/runtime state.

The evidence base is
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_TRIAGE_20260904.md`
and its linked D-drive result, Reviewer, public artifact-contract, and public
red-protocol files. The existing candidate remains explicit-only, inactive,
outside normal dispatch, and unqualified until a separately approved patch and
new freeze pass all gates.

## Non-negotiable requirements

- Keep the `openvisionlab-recipe-xml-handoff-v1` schema and the existing
  `qualification: false`/no-product-action boundary.
- Use request/evidence patterns, never `RTxx`/`Sxx` branches or hidden scorer
  values.
- Treat case/authority records, reviewed upstream envelopes, specialist
  packets, repository contracts, and validators as separate owners.
- Preserve valid immutable baselines byte-for-byte. A legacy baseline that
  fails the current validator is a corpus compatibility defect, not permission
  to rewrite the baseline or weaken `Main -> SourceFrame`.
- Do not emit a completed XML handoff for `WAIT` or `REJECTED`; do not emit an
  XML file for a blocked red attempt.
- Pixel/intensity measurements remain `MEASURE_ONLY` without calibration when
  no physical unit is requested. Never serialize `PIXELPERMM: 0` as unknown.
- Candidate activation, normal dispatch, Import, Preview/Run, Recipe/layer/
  routing mutation, qualification, release, deployment, Original-repository
  work, commit, and push remain outside this contract.

## Public status/reason patterns

The following table is a public contract for evidence patterns. It is not a
case-to-answer table. The specialized reason codes already exist in the public
artifact vocabulary; this draft defines when they take precedence.

| Supplied pattern | Required status/reason | XML delivery |
| --- | --- | --- |
| Catalog, upstream, specialist, or XML identity/hash/schema mismatch | `REJECTED` with the matching public hash/schema reason, such as `TOOL_CATALOG_HASH_MISMATCH`, `UPSTREAM_HASH_MISMATCH`, `SPECIALIST_PACKET_HASH_MISMATCH`, `XML_VALIDATION_HASH_DRIFT`, or `HANDOFF_SCHEMA_DRIFT` | No completed XML |
| Inline or unhashed XML that has not become a file-backed artifact | `PROPOSED / INLINE_HASH_NOT_RETAINED` | Inline/proposed only; never a completed file-backed handoff |
| Explicit operator ROI/CvROI is required but absent or pending | `WAIT / OPERATOR_ROI_REQUIRED` | No XML |
| Ordered source/destination correspondences are required but absent or incomplete | `WAIT / AFFINE_CORRESPONDENCE_REQUIRED` | No XML |
| Required datum/geometry identity is absent without a more specific typed rule | `WAIT / OPERATOR_DATUM_FEATURES_REQUIRED` | No XML |
| Geometry identity is supplied, but a requested physical-unit conversion lacks calibration | `WAIT / CALIBRATION_REQUIRED` | No XML |
| A valid retained upstream envelope contains a stage `WAIT` | `WAIT / UPSTREAM_WAIT_PROPAGATED` before downstream prerequisite reasoning | No downstream XML |
| A valid retained upstream envelope contains a stage `FAIL` | `REJECTED / UPSTREAM_STAGE_FAIL` before graph-review reasoning | No downstream XML |
| Capability is merely unavailable and the request does not ask to bypass the catalog | `WAIT / WAIT_ALGORITHM_GAP` | No XML |
| Request explicitly invents or bypasses an unlisted ToolType or semantic detector | `REJECTED / UNSUPPORTED_SEMANTIC_CLAIM` | No XML |
| Per-image source/template/ROI/angle/threshold substitution | `REJECTED / PER_IMAGE_OVERRIDE_FORBIDDEN` | No XML |
| Per-image area/score/tolerance/Good-Bad acceptance substitution | `REJECTED / PER_IMAGE_ACCEPTANCE_MUTATION_FORBIDDEN` | No XML |
| Graph recomposition or automatic candidate selection without an observed upstream failure | `REJECTED / UPSTREAM_GRAPH_REVIEW_REQUIRED` | No XML |
| Direct layer/routing mutation without graph recomposition | `REJECTED / LAYER_MUTATION_NOT_AUTHORIZED` | No XML |
| Valid file-backed XML, retained first-validator `PASS`, and no acceptance fields | `MEASURE_ONLY` | One validated XML may be emitted |

`PRODUCT_IMPORT_NOT_AUTHORIZED`, `PRODUCT_EXECUTION_NOT_AUTHORIZED`,
`RECIPE_MUTATION_NOT_AUTHORIZED`, `OUT_OF_SCOPE_PLATFORM_REQUEST`, and the
remaining existing public codes retain their current request-pattern meaning.
This draft does not introduce a new reason code.

## Precedence and projection rules

Evaluate a supplied request in this order, stopping at the first applicable
blocking condition:

1. **Identity and integrity.** Validate schema, path, length, and SHA-256
   before interpreting capability or semantic wording. A catalog hash mismatch
   cannot be reclassified as an unsupported semantic claim.
2. **Immutable baseline.** Validate the baseline pointer set, then preserve its
   status, qualification, upstream/XML/validation pointers, overrides, and
   product-action flags exactly. An unauthorized action changes only the
   explanation and the specific public reason.
3. **Upstream terminal state.** Propagate a valid upstream `WAIT`; report a
   valid upstream stage `FAIL` before graph review. A declared `PASS` requires
   non-empty, hash-valid evidence.
4. **Unauthorized mutation.** Apply the specific per-image, acceptance,
   graph, layer, product-import, product-execution, or Recipe-mutation rule.
5. **Typed prerequisites.** Prefer explicit ROI and affine-correspondence
   reasons over generic datum reasoning; use calibration only when geometry is
   already identified and physical-unit conversion is requested.
6. **Capability boundary.** Distinguish an unavailable catalog capability from
   an explicit request to invent or bypass an unlisted ToolType/semantic.
7. **Artifact finalization.** A file-backed XML becomes `MEASURE_ONLY` only
   after the exact first validator passes, the manifest is complete, and all
   projections agree. Inline/unhashed XML remains `PROPOSED`.

Every emitted raw response, XML, upstream envelope, handoff, validator output,
manifest, and decision-evidence file must project one transient tuple:

```text
status
xmlArtifact.state / delivery
validation.state / validator
reasonCode
ordered Tool/layer graph
frame, ROI, parameter, and owner records
baseline mode and protected pointers
gate states and evidence references
qualification and product-action flags
```

## Closed-world serialization locks

- Copy the reviewed Tool count, order, layer names, frames, and parameters.
  Do not add helper stages or infer a Tool from image appearance.
- Keep `Main` inputs in `SourceFrame` and preserve non-transforming Tool frames.
  Only an explicitly evidenced transforming packet may introduce a different
  frame.
- A source PNG can establish an image observation only. It cannot establish an
  operator lock, calibration, acceptance policy, or specialist packet value.
- `PIXELPERMM` is omitted when no positive calibration is owned. A zero value
  is never an unknown sentinel.
- `FIXTURE_MIN_VALID_PIXEL_RATIO` is required for a NormalizeImage consumer
  only when its value and owner are explicit. The guide's bounded `0.25`
  example is not a global default.
- `USE_GAP_EDGE_PAIR` and its dark-band parameters are opt-in for a reviewed
  dark-band intent. They must not be copied into a bright-tab facing-edge
  LineDistance plan.
- Projection fields such as `VER_PRJ_DIR`, ROI ownership, and frame names must
  be byte/value-consistent across raw response, envelope, XML, handoff, and
  decision evidence.

## Required focused coverage after approval

Before any new benchmark admission, the candidate-only patch must add or retain
focused checks for:

1. inline/unhashed XML remaining `PROPOSED`;
2. catalog hash integrity preceding capability classification;
3. typed operator ROI and affine-correspondence waits;
4. upstream `WAIT` propagation and upstream `FAIL` precedence;
5. unavailable capability versus explicit catalog bypass;
6. pixel-only measurement with omitted `PIXELPERMM`;
7. no zero calibration sentinel;
8. bright-tab facing-edge graph without dark-band starter parameters;
9. owner/evidence separation for ROI and policy values; and
10. explicit NormalizeImage ratio ownership.

The existing v0.1.10 focused suite, Skill Creator validation, and Python
compilation must remain green. A focused pass does not activate the candidate or
qualify a Recipe.

## Operator approval checklist

The following decisions are intentionally left open for the operator:

- [ ] Approve the public pattern table and precedence order above.
- [ ] Approve publication of the specialized reason mappings without any
  case-ID or hidden-answer mapping.
- [ ] Approve the pixel-only rule that omits `PIXELPERMM` unless a positive,
  owner-supplied calibration exists.
- [ ] Choose and document an explicit owner/value for
  `FIXTURE_MIN_VALID_PIXEL_RATIO` in the affected S4 clear cases, or remove
  those cases from the clear set until the value is supplied. `0.25` may be
  used only as an explicitly approved case value, not as a silent default.
- [ ] Approve a new corpus identity after the candidate-only focused patch;
  the v0.1.10 root remains immutable.

## Closure boundary

Status: **Draft**  
Scope: public reason/precedence and serialization correction contract for a
future candidate-only patch.  
Evidence: v0.1.10 Round 1 result and triage report listed above.  
Not claimed: candidate implementation, benchmark rerun, activation, product
execution, qualification, runtime parity, human comparison, release,
deployment, or Original-repository work.

Recommended model for the approval review: `gpt-5.6-terra`  
Reasoning effort: `high`
