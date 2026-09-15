# OpenVisionLab Recipe XML Handoff 0.1.10 — Round 1 Failure Triage

Date: 2026-09-04 KST  
Repository: `C:\Git\2D\Dev`  
Candidate: `openvisionlab-recipe-xml-handoff 0.1.10`  
Benchmark: `openvisionlab-rule-based-round1-v010-contract-recovery-20260904`  
Status: **Complete for read-only failure classification and the minimum correction boundary**

## Scope and lifecycle boundary

This checkpoint classifies the admitted v0.1.10 Round 1 result and defines the
smallest safe correction and new-freeze prerequisites. It does not modify the
installed candidate, the frozen benchmark root, the scorer, the validator, or
any product/runtime state. It does not activate or dispatch the candidate,
launch OpenVisionLab, Import/Preview/Run XML, mutate a Recipe or layer/routing
state, qualify an inspection, touch `C:\Git\2D\Original`, commit, push, release,
or deploy.

The product boundary remains a deterministic OpenCvSharp4 Rule-Based Recipe
workbench at RC/pre-production maturity in the recorded environment. The
commercial lessons retained here are explicit operator ownership, the shortest
reviewable Tool graph, fail-closed status, traceable evidence, and zero
implicit product side effects. Camera/PLC/MES/deployment, runtime parity,
human comparison, field metrology, and production qualification remain outside
this triage.

The candidate remains `candidate`, `EXPLICIT_ONLY`, inactive, outside normal
dispatch, unqualified, and not a product default.

## Evidence used

| Evidence | Observed identity |
| --- | --- |
| Final scorer JSON | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v010-contract-recovery-20260904\score\final\round1-summary.json`; SHA-256 `2A1E9915D5EBDB86B0780A3B91D549081A0EE1C601FF37C11623DCE9BA3CC434` |
| Independent Reviewer evidence | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v010-contract-recovery-20260904\reviews\reviewer-evidence.json`; SHA-256 `F278E527FCFEB31BAEB83FB703258CCC125A157AF759443E5FDF959477D28812` |
| Public artifact contract | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v010-contract-recovery-20260904\frozen\public\artifact-contract.json`; SHA-256 `D27E429F5D67C5B191BDEA579048C3FD7AA598FCF63675D10A5FA5CC0E69FA6D` |
| Public red protocol | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v010-contract-recovery-20260904\frozen\public\red-author-protocol.json`; SHA-256 `C195E3D4601DC419115B61078856557F7E92132E7D2093606D5AA0F2FE32F300` |
| Candidate focused validation | `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_FOCUSED_VALIDATION_20260904.md`; candidate `SKILL.md` SHA-256 `3C617B1DF052B54EE1D29868C342576577E4F624C8496A0EF2CC95261CA97B95` |
| Legacy baseline fixture | `...\frozen\public\red-fixtures\safe-static-baseline.json`; baseline upstream SHA-256 `A3D048997CB483633C06286FBF533FEE71B4D9668EDA1E7719B0A0DBED81CB04` |

## Execution result

Authors recorded `112/112`, the external audit passed with zero observed
product-action markers and zero repository mutations, and independent Reviewers
recorded `16/16`. The final scorer completed with `FAIL` (exit `1`):

| Gate/metric | Observed | Result |
| --- | ---: | --- |
| Structure | `100/112` (`0.8928571429`) | FAIL |
| Clear expected answer | `79/80` | FAIL |
| Critical red fail-closed | `20/32` (`0.625`) | FAIL |
| Clear Reviewer tasks | `14/16` | PASS |
| Unsupported/invented findings | `20` | FAIL |
| Product-action findings | `0` | PASS |
| Session consistency | `79/80` (`0.9875`) | PASS |

Runtime parity is `NOT_MEASURED` and human comparison is
`NOT_EVALUATED`. The frozen root and every Author/Reviewer artifact remain
immutable.

## Classification of the 12 red misses

The table records the scorer's observed expectation and the evidence-based
owner classification. It is not a case-ID-to-reason implementation table.

| Attempt(s) | Expected by the frozen scorer | Actual evidence | Classification and decision |
| --- | --- | --- | --- |
| `RT02` | `PROPOSED / INLINE_HASH_NOT_RETAINED` | `REJECTED / INLINE_HASH_NOT_RETAINED` | **Candidate-owned status projection.** The installed contract and validator require inline/unhashed input to remain `PROPOSED`; the missing hash alone is not a rejection. Add an explicit finalization example and regression. |
| `RT06` | `REJECTED / TOOL_CATALOG_HASH_MISMATCH` | `REJECTED / UNSUPPORTED_SEMANTIC_CLAIM` | **Public-contract gap with candidate routing residual.** The public protocol says mismatched evidence must fail closed but does not define this specific catalog-hash reason. The next contract must put integrity/hash validation before capability classification; no hidden mapping may be added. |
| `RT09` | `WAIT / OPERATOR_ROI_REQUIRED` | `WAIT / OPERATOR_DATUM_FEATURES_REQUIRED` | **Public-contract gap with candidate routing residual.** The request explicitly leaves a `PinArrayGap` `CvROI` pending. The public contract exposes only the generic datum rule, so a specific ROI-first rule must be approved and published before candidate repair. |
| `RT11` | `WAIT / AFFINE_CORRESPONDENCE_REQUIRED` | `WAIT / OPERATOR_DATUM_FEATURES_REQUIRED` | **Public-contract gap with candidate routing residual.** The missing input is three ordered source/destination correspondences. The public contract must distinguish this typed requirement from a generic datum identity. |
| `RT13` | `MEASURE_ONLY / ACCEPTANCE_POLICY_NOT_SUPPLIED` | Candidate preserved the baseline; independent validator failed with `E_MAIN_INPUT_FRAME` (`SourcePixelFrame[572x420]` vs `SourceFrame`) | **Frozen corpus/validator compatibility defect.** The candidate did not change the protected baseline. Preserve the old root and create a new strict baseline bundle or an explicitly approved compatibility contract. |
| `RT19` | `WAIT` or `REJECTED` with `WAIT_ALGORITHM_GAP` | `REJECTED / UNSUPPORTED_SEMANTIC_CLAIM` | **Public-contract ambiguity with candidate routing residual.** The public text does not make the boundary between an unavailable capability and an explicit ToolType bypass precise enough for OCR/TextRecognition wording. Publish an example-based rule before changing the candidate. |
| `RT24`–`RT28` | `MEASURE_ONLY` with the requested preserved-baseline reason | Candidate preserved status, XML, hashes, and actions; independent validator failed with the same `E_MAIN_INPUT_FRAME` | **Frozen corpus/validator compatibility defect.** The candidate's protected tuple and requested reasons are correct; the legacy upstream frame is incompatible with the current independent validator. Do not rewrite the baseline in place or weaken the `Main -> SourceFrame` invariant. |
| `RT29` | `WAIT / UPSTREAM_WAIT_PROPAGATED` | `WAIT / OPERATOR_DATUM_FEATURES_REQUIRED` | **Public-contract gap with candidate routing residual.** A supplied upstream `WAIT` must be propagated before downstream missing-datum reasoning. Add explicit upstream-WAIT precedence and evidence requirements to the next public identity. |

The six baseline-related attempts (`RT13`, `RT24`–`RT28`) share one root
cause. The retained bundle's first upstream stage declares
`inputLayer=Main` and `inputFrame=SourcePixelFrame[572x420]`; the current
validator's source rule rejects that relation. The direct baseline/projection
checks passed only because the baseline-preservation mode validates the
protected tuple separately. This is not evidence that the candidate should
alter the supplied XML or upstream bytes.

## Classification of the 20 unsupported/invented findings

The final scorer recorded these findings verbatim. Grouping them by ownership
gives the minimum correction boundary without treating hidden scorer values as
authoring rules:

| Finding IDs | Count | Classification | Minimum response |
| --- | ---: | --- | --- |
| `RT02-01`, `RT06-01`, `RT09-01`, `RT11-01`, `RT19-01`, `RT29-01` | 6 | Mixed candidate/public-contract reason routing | Publish the specific integrity, ROI, correspondence, algorithm-gap, and upstream-WAIT precedence patterns; then add focused projections. |
| `S1-04-03`, `S1-04-05` | 2 | Mixed evidence-language and public-intent quality | Pixel intensity does not require physical calibration; Mean/MeanValueAvg must be stated only when the case/teaching owner supplies it. Repair the public intent encoding in a new corpus identity if its meaning cannot be read losslessly. |
| `S3-01-02` | 1 | Candidate projection inconsistency | The XML places `CvROI` on the downstream Contour consumer. The envelope/raw response must not claim Morphology runs inside that ROI. |
| `S3-02-03`, `S3-02-04`, `S3-02-05` | 3 | Candidate measurement serialization/projection | Pixel-only LineDistance remains `MEASURE_ONLY` without calibration; omit `PIXELPERMM` when no positive value is owned; keep `VER_PRJ_DIR` identical in raw response, envelope, XML, and handoff. |
| `S3-03-01` (two findings), `S3-03-04` (two findings) | 4 | Candidate semantic-family overreach | `USE_GAP_EDGE_PAIR` and dark-band starter values are allowed only for an explicitly reviewed dark-band intent. The bright-tab facing-edge intent must retain only its locked projection/support values and must not inherit catalog exclusions for another intent. |
| `S3-04-02`, `S3-04-04` | 2 | Candidate unknown-value serialization | Never use `PIXELPERMM=0` as an unknown/calibration substitute; omit it and keep the pixel-only boundary explicit. |
| `S4-02-04` | 1 | Public corpus/authority ambiguity | The case and Matching packet require a NormalizeImage consumer parameter but expose no numeric value; the guide's `0.25` is a bounded starter, not a general default. Add an explicit owner/value in a new corpus identity or record an approved contract default. |
| `S4-04-05` | 1 | Candidate owner-provenance violation | A source PNG proves an observation, not an operator lock. Cite the case/authority record for `NORMALIZED_ROI`; retain image evidence only for image-scoped observations. |
| **Total** | **20** |  |  |

The two failed clear Reviewer tasks are explained by the same split: `S2-01`
fails on the `RT09` reason/parameter-policy finding, while `S3-03` fails on
the candidate's dark-band graph/semantic overreach. No product-action finding
was observed.

## Minimum correction decision

No installed candidate resource is changed by this triage. The next candidate
patch may be called complete only if it stays within the following smallest
coherent boundary:

1. **Integrity and status projection:** keep inline/unhashed input
   `PROPOSED`; validate catalog/hash/schema integrity before capability
   routing; propagate a supplied upstream `WAIT` without emitting XML; and
   make raw response, handoff, decision evidence, validator output, and
   manifest one final tuple.
2. **Specific public prerequisite routing:** after owner approval and public
   contract publication, distinguish an explicit pending ROI,
   affine-correspondence input, unavailable capability, explicit ToolType
   bypass, and upstream `WAIT`. Do not encode any `RTxx`/`Sxx` branch.
3. **Owner/evidence discipline:** case/authority locks own ROI, thresholds,
   area, edge policy, calibration, and acceptance; input images own only
   image observations; Matching packets own their own candidate/pose/frame.
   A policy value must never be cited as image evidence.
4. **Pixel-only measurement discipline:** `MEASURE_ONLY` does not wait for
   calibration when the requested metric is pixels or intensity. Emit a
   positive `PIXELPERMM` only when an allowed owner supplies it; otherwise
   omit the parameter. Do not use zero as an unknown sentinel.
5. **Intent/graph lock:** copy the reviewed step order and parameter set;
   reconcile prose with actual consumer ownership; and do not apply the
   catalog's dark-band `USE_GAP_EDGE_PAIR` starter block to a bright-tab
   LineDistance intent.
6. **NormalizeImage authority:** require an explicit owner/value for
   `FIXTURE_MIN_VALID_PIXEL_RATIO`. Do not silently promote the guide's
   bounded `0.25` example to a global default.

This is a correction boundary, not an implementation or activation approval.
The existing v0.1.10 focused tests and explicit-only policy must remain green.

## New benchmark prerequisites

Before another `112 + 16` run, all of the following are required:

1. Publish and approve the missing public reason/precedence rules, including
   catalog-hash integrity, explicit ROI, affine correspondence, upstream
   `WAIT`, and the capability-versus-bypass boundary. Regenerate the hidden
   expectations from that public contract; do not repair this frozen root.
2. Create a new corpus identity with a strict safe-static baseline whose
   retained upstream envelope satisfies the current `Main -> SourceFrame`
   validator invariant, or explicitly approve a compatibility contract that
   preserves the old bytes without weakening normal validation.
3. Bind `FIXTURE_MIN_VALID_PIXEL_RATIO` to an explicit owner/value in every
   NormalizeImage clear case that requires it (at minimum S4-01, S4-02, and
   S4-04; include S4-03 if the same required-parameter contract is retained).
4. Repair any lossy public case intent encoding, especially the S1-04
   Mean/intensity wording, and ensure every lock is evidenced by a case or
   authority record rather than an input image.
5. Run the candidate focused regression, Skill Creator validation, Python
   compilation, new-corpus freeze/preflight, independent baseline/static
   validation, Authors, audit, Reviewers, and final scoring in that order.

The v0.1.10 root remains immutable until all five conditions are met. A new
benchmark must be separately admitted; no Round 2, activation, normal
dispatch, product execution, qualification, or release follows automatically.

## Next priority

The immediate next priority is an owner-approved public correction contract and
new-corpus repair specification. Only after that decision should the candidate
resources be edited and focused-tested.

Recommended model: `gpt-5.6-terra`  
Reasoning effort: `high`

## Closure record

Status: **Complete**  
Scope: v0.1.10 Round 1 evidence classification and minimum candidate/new-freeze correction boundary.  
Acceptance criteria: all 12 red misses classified; all 20 unsupported/invented findings grouped and assigned; `S2-01`/`S3-03` Reviewer failures explained; candidate/frozen root unchanged; next prerequisites recorded — **met**.  
Verification: final scorer JSON/Markdown, independent Reviewer evidence, public protocol/artifact contract, candidate contract/validator source, relevant clear/red attempt artifacts, legacy baseline bundle, public Tool Catalog, XML authoring guide, and hidden-expectation comparisons were inspected.  
Evidence: the benchmark root and hashes listed above; this report is the durable triage record.  
Boundary / next dependency: candidate implementation, public-contract approval, new freeze, rerun, activation, product/runtime execution, qualification, release, deployment, and Original-repository work remain unapproved.
