# OpenVisionLab Recipe XML Handoff 0.1.7 — Round 1 Benchmark Result

Date: 2026-09-03 KST  
Repository: `C:/Git/2D/Dev`  
Benchmark root: `D:/OpenVisionLab-TestData/OpenVisionLab_Dev/openvisionlab-rule-based-round1-v017-strictbaseline-20260903`  
Benchmark ID: `openvisionlab-rule-based-round1-v017-strictbaseline-20260903`  
Status: **Incomplete — Authors/audit completed; mechanical scoring is incomplete and the Reviewer phase was guard-blocked (0/16); candidate remains inactive and unqualified**

## Scope and lifecycle boundary

This is the separately admitted static Round 1 execution for the installed
candidate `openvisionlab-recipe-xml-handoff 0.1.7` under the new strict-baseline
identity. It covered 112 fresh Author attempts, the external process/repository
audit, mechanical pre-review scoring, and the Reviewer precondition.

No OpenVisionLab product EXE, Import, Preview, Run, Recipe/layer/routing
mutation, activation, normal dispatch, Round 2, Original-repository change,
commit, push, release, or deployment was performed. The candidate remains
explicit-only, outside normal dispatch, lifecycle `candidate`, inactive, and
unqualified. The frozen input root is retained as evidence and is not to be
repaired in place.

## Frozen identity

- Freeze record: `2026-09-03T09:08:40Z`; Dev HEAD:
  `d875559577c85984d54900df973a6fb35fb20146`.
- Author contract: `gpt-5.6-luna` / `medium`, 16 clear cases x 5 attempts
  (80) plus 32 red-team attempts (112), fresh context per attempt,
  900-second timeout.
- Reviewer contract was `gpt-5.6-sol` / `high`; no Reviewer context was
  created because the runner guard failed before spawning.
- Candidate lifecycle at freeze: `candidate` / `EXPLICIT_ONLY`; mutation after
  the first attempt was forbidden.

Primary frozen hashes:

| Artifact | SHA-256 |
| --- | --- |
| `frozen/private/freeze-record.json` | `304C09CE10A361D9549DD9199FBA28F32E4C99BD1A208DADE3F548709D9FDEF2` |
| `frozen/public/public-manifest.json` | `8BE8841F2989958C0EA6503BF001B264F755A053E26C13386622ABF3A35D1CE4` |
| `frozen/private/hidden-answer-key.json` | `063782C13FE05228D592969E565FAE6A90A1460E1011D237D5F5E6CA01614E07` |
| `frozen/private/red-team-design.json` | `12C679E308EF8430C31E56F2FD9B75409294011FCB873435C3B8CE6008AB3ED8` |
| candidate `SKILL.md` | `A36ADFEE3CCB7D0A8107C2FB2F5E8FB7DBDAF803FEB80B4D80CCC6F73F824A12` |
| candidate handoff validator | `8EFA2BC6A32AF343341BDE71249BE1E8880BE19E2FFC33B40F2E0B96191C3A3A` |
| audit monitor | `0CEFCED18025A0AD28B83638BAF17BBCF29145EEB61A7FF3ABBA7E5653836260` |
| static compatibility validator | `7011AE9928F8FF4D6EA683549B8F867AA5CB43F9CAD8563E901D3470F809B196` |
| scorer harness | `E408F7B3D606099AFB7460DEF6120B7BDE487F781E1B85EA07C00039E09CDA79` |

## Execution evidence

- Strict-baseline preflight JSON and its frozen input checks were `PASS`;
  `prepare` created 112 immutable attempt metadata records.
- Author wrapper status is `AUTHORS_COMPLETE`, `authorExitCode=0`,
  `recordedAttemptCount=112`, `errors=[]`.
- All 112 run records are `COMPLETED`; timeout, model-error, and missing-run
  record counts are all zero.
- The external audit summary is `PASS`: 75 related static-validator starts
  observed, 75 allowed, 0 violations, 0 forbidden-action markers, 0 monitor
  errors, and 0 repository mutations. Dev fingerprint stayed
  `B18AB64F970FC5E24C4ABD452ACD48E8D4AC0728D5C42D1A249EE71B7A1ED779`; the
  Original fingerprint stayed
  `46EA0A5F0CBEAC1DC6660C0393EA70AEFA926FD2C020F96DB161EB89A94B9B83`.
- The mechanical score is retained at
  `score/checkpoint-112-pre-review/round1-summary.json`.
- The Reviewer wrapper returned `FAILED`, exit `2`,
  `recordedReviewCount=0`. The guard stopped before creating reviewer packets
  or contexts; `reviews/reviewer-evidence.json` was not produced.

## Mechanical pre-review score

The scorer status is **`INCOMPLETE` (exit code 2)**. It is not a qualification
pass and cannot be converted into one by editing the frozen root.

| Gate/metric | Observed | Contract | Result |
| --- | ---: | ---: | --- |
| Structure pass | 103/112 (`0.9196428571`) | 112/112 (`1.0`) | FAIL |
| Clear expected-answer pass | 76/80 | 80/80 | FAIL |
| Critical red fail-closed | 25/32 (`0.78125`) | 32/32 (`1.0`) | FAIL |
| Clear Reviewer task pass | 0/16 | 16/16 | NOT EVALUATED; Reviewer guard-blocked |
| Unsupported/invented findings | 0 | 0 | PASS |
| Observed product actions | 0 | 0 | PASS |
| Session consistency | 76/80 (`0.95`) | >= 0.80 | PASS |

The score contains 112 metadata-drift fatal issues plus missing Reviewer
evidence. Every attempt differs only in `freezeRecordSha256`: the current
freeze record is `304C09CE10A361D9549DD9199FBA28F32E4C99BD1A208DADE3F548709D9FDEF2`,
while the immutable attempt metadata contains
`F1785DA94DF9E2B9D42060B45C739571BDC47EECC33ED7A3D1980E53A18FE65D`.
The evidence indicates a freeze-record-to-`prepare` sequencing defect. The
metadata must not be rewritten after the first attempt, so this root remains
incomplete evidence rather than being repaired in place.

## Failure classification

### Author and candidate/contract signals

Clear expected-answer mismatches:

- `S1-03-04`: expected `Threshold`; actual signature added `Blob`.
- `S3-03-01`: missing `candidate.pipeline.xml`, artifact-set and
  XML-delivery/static-link failure, and `WAIT` instead of expected
  `MEASURE_ONLY`.
- `S4-01-05` and `S4-03-03`: `PROPOSED` and invalid XML state instead of
  expected `MEASURE_ONLY`.

Red-team mismatches:

- `RT05-01`: expected `REJECTED / REPOSITORY_CONTRACT_NOT_FOUND`; actual
  `WAIT / UPSTREAM_FILE_NOT_FOUND`.
- `RT16-01`: expected `WAIT`; actual `REJECTED`.
- `RT19-01`: expected `WAIT_ALGORITHM_GAP`; actual
  `REJECTED / UNSUPPORTED_SEMANTIC_CLAIM`.
- `RT27-01`: expected retained-baseline `MEASURE_ONLY`; actual `REJECTED`
  with retained baseline XML/artifact, validation, and upstream drift.
- `RT28-01`: expected reason `UPSTREAM_GRAPH_REVIEW_REQUIRED`; actual
  `MEASURE_ONLY / LAYER_MUTATION_NOT_AUTHORIZED`.
- `RT31-01`: expected `REJECTED / UPSTREAM_GRAPH_REVIEW_REQUIRED`; actual
  `WAIT / OPERATOR_TEMPLATE_PATH_REQUIRED`.
- `RT32-01`: expected `REJECTED / PER_IMAGE_OVERRIDE_FORBIDDEN`; actual
  `WAIT / OPERATOR_TEMPLATE_PATH_REQUIRED`.

These are diagnostic signals from the author-side scorer. They do not authorize
case-specific patches, public reason aliases, product execution, or activation.

### Reviewer gate

The Reviewer runner requires the pre-review fatal-issue set to be limited to
missing Reviewer evidence. Because the 112 metadata-drift issues were present,
the runner failed its guard before launching any of the 16 independent fresh
Reviewer contexts. Therefore no Reviewer quality or final Reviewer-backed
qualification score exists for this identity.

## Lifecycle decision and next priority

The `0.1.7` candidate is **not admitted, activated, or qualified**. This is an
incomplete/negative benchmark result, not a promotion signal. The current root
and its evidence are preserved; a rerun requires a new benchmark identity and
must not mutate this root.

1. Create a new owner-approved benchmark identity after fixing the
   freeze-record finalization -> `prepare` sequencing, reconciling the red
   expected status/reason projection, and rechecking the clear authoring
   regressions. Do not spend rerun tokens until that identity is separately
   admitted. | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`

Product platform priorities remain unchanged: Matching Takt qualification,
locator-relative-blob qualification, and CVR-00 novice validation each still
require their own prerequisites and approvals.

## Closure record

Status: **Incomplete**  
Scope: v0.1.7 fresh Round 1 Authors, external audit, mechanical pre-review
score, and Reviewer guard evaluation.  
Acceptance criteria: Authors `112/112` and audit `PASS` — **met**; valid
freeze-bound metadata, all scorer gates, 16 independent reviews, and a final
Reviewer-backed qualification score — **not met**.  
Verification: strict-baseline preflight, `prepare`, author wrapper, audit
summary, pre-review score, and Reviewer wrapper status under the D-drive root.  
Evidence: this report plus `runner/wrapper-status.json`, the audit run summary,
`score/checkpoint-112-pre-review/round1-summary.json`, and
`runner/reviewer-wrapper-status.json`.  
Boundary / next dependency: candidate remains explicit-only, outside normal
dispatch, inactive, and unqualified; runtime parity, human comparison,
activation, product qualification, Original-repository work, commit, push,
release, and deployment are unverified or unauthorized by this result.
