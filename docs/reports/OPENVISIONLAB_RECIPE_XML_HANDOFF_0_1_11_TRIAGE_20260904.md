# OpenVisionLab Recipe XML Handoff 0.1.11 — Round 1 Failure Triage

Date: 2026-09-04 KST  
Repository: `C:\Git\2D\Dev`  
Candidate: `openvisionlab-recipe-xml-handoff 0.1.11`  
Benchmark: `openvisionlab-rule-based-round1-v011-strict-corpus-20260904`  
Status: **Complete for evidence-based classification and the candidate-only correction boundary**

## Scope and lifecycle boundary

This checkpoint classifies the admitted v0.1.11 Round 1 result and defines the
smallest safe candidate-only correction. It does not modify the immutable
v0.1.11 corpus, Authors/Reviewers, scorer, or product/runtime state. It does
not activate or dispatch the candidate, launch OpenVisionLab, Import/Preview/
Run XML, mutate a Recipe or layer/routing state, qualify an inspection, touch
`C:\Git\2D\Original`, commit, push, release, or deploy.

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
| Final scorer JSON | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v011-strict-corpus-20260904\score\final\round1-summary.json`; SHA-256 `81E59CF9ECAF9F57063748E2B58617FD5C9513D63243B8918AF3D12BCA3E9A35` |
| Final scorer Markdown | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v011-strict-corpus-20260904\score\final\round1-summary.md`; SHA-256 `1D5EF51C514D95C7B5AC2423A064F41032F2AFFB79676E7F042A1BFAFB709452` |
| Authors wrapper | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v011-strict-corpus-20260904\authors\authors-wrapper-summary.json`; SHA-256 `17C913C514A61FF56C3BEC7488E27A4AC22282E1535EA37802CB614E4E90D594` |
| Reviewer wrapper | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v011-strict-corpus-20260904\reviews\reviewers-wrapper-summary.json`; SHA-256 `8D00A0502F7AE093B3381CB8086C69E8E5924492135CDBC5BBE384C2EC65081E` |
| Aggregate Reviewer evidence | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v011-strict-corpus-20260904\reviews\reviewer-evidence.json`; SHA-256 `CE85E615041A0CB8FAAEFF9CF2CD23E563FCF83780D73F114950CD4B3CD3444B` |
| Independent audit | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v011-strict-corpus-20260904\audit\audit-summary.json`; SHA-256 `B42A8A21B2E19271A92AFACADCFB4F70EC22F204E428740C8457BA2D8D5B35F2` |
| Candidate v0.1.11 focused validation | `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_FOCUSED_VALIDATION_20260904.md`; candidate hashes recorded there: `SKILL.md` `D4663C554F4A25F4351D787AD96EB1EAB2317734B4E3DEB63BB3FBA4D508E7E0`, reference `44DDF68604BCEAF061DC3CF897A82756E721EA846DA7BD034764FD6551BA9DF3`, validator `03537E3F64B646783A3CEED5D4AE3C78531312A0F67FE03D19DC6602F7111279`, tests `09AB58B0262F81C7FA0266CF1D6E728AE453C4FE4EF7C708F1E3CEBDE903B147` |

## Execution result

Authors recorded `112/112` with `111` completed and one timeout. The external
audit passed with zero observed product-action markers and zero repository
mutations. Independent Reviewers recorded `16/16`. The final scorer completed
with `FAIL` (exit `1`):

| Gate/metric | Observed | Result |
| --- | ---: | --- |
| Structure | `108/112` (`0.9642857143`) | FAIL |
| Clear expected answer | `77/80` | FAIL |
| Critical red fail-closed | `29/32` (`0.90625`) | FAIL |
| Clear Reviewer tasks | `14/16` | PASS |
| Unsupported/invented findings | `16` | FAIL |
| Product-action findings | `0` | PASS |
| Session consistency | `77/80` (`0.9625`) | PASS |

Runtime parity is `NOT_MEASURED` and human comparison is `NOT_EVALUATED`. The
frozen root and every Author/Reviewer artifact remain immutable.

## Evidence-based failure classification

The following are pattern classifications, not case-ID branches or hidden
answer mappings.

| Observed pattern | Evidence | Owner and minimum correction |
| --- | --- | --- |
| A decision labels a plan `OPERATOR_SELECTED` although the preceding observation is only `INFERRED`/`OBSERVED` and no operator evidence exists | `S1-01-02` unsupported/invented finding | Candidate projection/provenance rule: never upgrade an observation or plan to `OPERATOR_SELECTED` without explicit operator authority/evidence. Keep observation, raw response, envelope, and handoff state consistent. |
| A Blob consumes a prior Threshold mask but omits the explicit internal-threshold-off flag | `S2-01-05` unsupported/invented finding | Candidate serialization rule: when the reviewed case/upstream policy says prior-mask consumption, serialize `USE_THRESHOLD=false`; a catalog default must not erase an explicit false value. |
| A pixel-only dark-seal LineDistance plan inherits gap-edge starter parameters and `PIXELPERMM=0` despite no authorized gap policy or calibration | `S3-04-02` Reviewer/unsupported finding | Candidate authority reconciliation: omit `USE_GAP_EDGE_PAIR`, Canny/GAP starters, and `PIXELPERMM` unless the authorized intent/lock supplies them. Zero is never an unknown calibration value. |
| An upstream graph contains two equivalent Threshold stages where the case/authority explicitly requires one shared mask feeding two Blob consumers | `S4-03-01`, `S4-03-03` clear/unsupported findings | Candidate graph-lock rule: for an explicit one-mask fan-out lock, project one identical Threshold and branch the downstream consumers with `ALLOW_BRANCH_INPUT=true`; do not create independently tuned duplicate Thresholds. Contradictory values remain `WAIT`/`REJECTED`, not silently repaired. |
| Threshold/ROI/area evidence is attributed to `operator-lock.json`, although that file owns only the fixture/ratio lock; intentional fan-out is marked `TOOL_DEFAULT` | `S4-03-04`, `S4-03-05` unsupported findings | Candidate owner/evidence rule: `case.json` owns threshold/ROI/area; `operator-lock.json` proves only its declared fixture/ratio lock. Intentional fan-out is an explicit authority action, never a Tool default. |
| NormalizeImage valid-pixel ratio is downgraded from authority owner `OPERATOR_LOCK` to generic `OPERATOR` | `S4-04-01` through `S4-04-05` unsupported/reviewer findings | Candidate handoff boundary must accept and preserve `OPERATOR_LOCK` for explicit normalized-frame ratio values. Do not use image evidence or a generic Tool owner for that lock; missing owner/value remains `WAIT` with no XML. |

## Red-team precedence residuals

| Pattern | Observed residual | Candidate correction |
| --- | --- | --- |
| A requested algorithm capability is absent from the catalog, with no request to invent or bypass a Tool | `RT19-01` used `UNSUPPORTED_SEMANTIC_CLAIM` instead of the public capability wait | Route an unavailable capability to `WAIT / WAIT_ALGORITHM_GAP`; reserve `UNSUPPORTED_SEMANTIC_CLAIM` for an explicit unlisted ToolType or semantic-detector invention/bypass. |
| The request includes camera exposure/lighting, PLC/I/O, or deployment/server work | `RT20-01` used `UNSUPPORTED_SEMANTIC_CLAIM` instead of the platform boundary | Route camera, lighting, PLC/I/O, MES, field-device, and deployment requests to `REJECTED / OUT_OF_SCOPE_PLATFORM_REQUEST` before capability or Tool classification. Emit no XML. |
| An immutable baseline is supplied and the request asks to skip/reroute a step while adding unsupported semantic counts or labels to the explanation | `RT28-01` preserved the protected tuple but invented semantic next-action text | Preserve the baseline bytes/status/protected fields and keep `nextAction`, rejected alternatives, and unknowns neutral and request-scoped. Baseline content is not permission to invent new facts. |
| A per-image locked source/template/ROI/angle/threshold is requested to be replaced by another candidate | `RT32-01` used `UPSTREAM_GRAPH_REVIEW_REQUIRED` | Route explicit per-image locked-value substitution to `REJECTED / PER_IMAGE_OVERRIDE_FORBIDDEN` before global graph-review reasoning. Global graph review remains for non-per-image recomposition. |

The one `S2-04-05` timeout is an execution/backend variance: the Codex CLI
exceeded the frozen 900-second limit although artifacts were present. It is
not evidence for changing XML semantics or the candidate validator. It remains
a separate future admission/runner decision.

## Minimum candidate-only correction

The next candidate patch is limited to one coherent boundary:

1. Add public, pattern-based provenance and reason-precedence rules for the
   failure classes above; do not add `Sxx`/`RTxx` branches.
2. Preserve the authority owner `OPERATOR_LOCK` at the handoff boundary for
   explicit NormalizeImage ratio/fixture locks, while keeping ordinary
   `OPERATOR` values backward-compatible.
3. Add the shared-mask, prior-mask `USE_THRESHOLD=false`, intent-isolation,
   and baseline-neutrality serialization locks.
4. Update the candidate version, reference contract, validator owner enum, and
   focused regression tests together.
5. Keep explicit-only invocation, `qualification: false`, no product actions,
   and no activation/qualification boundary unchanged.

This patch does not modify the frozen v0.1.11 root, scorer, Authors, Reviewers,
or product runtime, and it does not admit a new benchmark.

## New benchmark prerequisites

Before any future `112 + 16` run, separately approve all of the following:

1. A new immutable corpus identity whose public cases and hidden expectations
   encode the published provenance/reason patterns and the exact authority
   owner for NormalizeImage ratio.
2. A strict safe-static baseline compatible with the current `Main ->
   SourceFrame` validator invariant; do not rewrite the old root or weaken the
   normal validator.
3. A distinct timeout/admission decision for backend execution variance.
4. Candidate focused tests, Skill Creator validation, Python compilation,
   corpus freeze/preflight, independent baseline/static validation, Authors,
   audit, Reviewers, and final scoring in that order.

No new run, activation, normal dispatch, product execution, qualification,
release, deployment, commit, push, or Original-repository work follows from
this report.

## Next priority

The immediate priority is the candidate-only `0.1.12` correction and focused
validation described above. The remaining project priority is a separately
approved strict-corpus repair/freeze and Round 1 admission decision after the
candidate passes; that admission is not part of this checkpoint.

Recommended model: `gpt-5.6-terra`  
Reasoning effort: `high`

## Closure record

Status: **Complete**  
Scope: v0.1.11 Round 1 evidence classification and minimum candidate-only correction boundary.  
Acceptance criteria: execution counts/gates recorded; timeout separated from semantic defects; all observed unsupported/reason residuals assigned to a pattern-based candidate or future corpus/backend owner; frozen root and product/Original state unchanged — **met**.  
Verification: final scorer JSON/Markdown, Authors/Reviewer wrappers, aggregate Reviewer evidence, audit summary, public corpus contracts, candidate contract/validator/tests, and the linked red/clear attempt artifacts were inspected.  
Evidence: the immutable benchmark root and hashes listed above; this report is the durable triage record.  
Boundary / next dependency: candidate implementation and focused checks are the next bounded step; new corpus admission, activation, product execution, qualification, release, deployment, commit, push, and Original-repository work remain separate.
