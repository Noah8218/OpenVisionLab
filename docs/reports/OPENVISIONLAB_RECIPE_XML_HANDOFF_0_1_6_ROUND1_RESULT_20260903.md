# OpenVisionLab Recipe XML Handoff 0.1.6 — Round 1 Benchmark Result

Date: 2026-09-03 KST  
Repository: `C:\Git\2D\Dev`  
Benchmark root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v016-corpusfix-auditfix2-20260903`  
Benchmark ID: `openvisionlab-rule-based-round1-v016-corpusfix-auditfix2-20260903`  
Status: **Incomplete — Authors and audit completed; Reviewer phase was guard-blocked; candidate remains inactive and unqualified**

## Scope and lifecycle boundary

This is the fresh Round 1 admission attempt for the installed candidate
`openvisionlab-recipe-xml-handoff 0.1.6` against the repaired Gate A corpus.
The run was limited to the frozen 112-attempt author evaluation, external
process/repository audit, mechanical scoring, and the reviewer precondition.
No OpenVisionLab product EXE, Import, Preview, Run, Recipe/layer/routing
mutation, activation, normal dispatch, Round 2, Original-repository change,
commit, push, release, or deployment was performed.

The candidate remains explicit-only, outside normal dispatch, lifecycle
`candidate`, inactive, and unqualified. The prior v0.1.5 benchmark root and
the v0.1.6 focused-validation evidence remain separate and immutable.

## Frozen identity

The freeze record was created at `2026-09-03T03:30:00Z` with candidate
mutation after the first attempt forbidden. The frozen execution contract was:

- Author model/effort: `gpt-5.6-luna` / `medium`.
- 16 clear cases x 5 attempts = 80, plus 32 red-team attempts = 112.
- Fresh context per attempt; 900-second timeout; no product execution.
- Dev HEAD at freeze: `d875559577c85984d54900df973a6fb35fb20146`.

Primary frozen hashes:

| Artifact | SHA-256 |
|---|---|
| `frozen/private/freeze-record.json` | `FE323B2AFE96DAC77BDAEC4FBD10128A27C0368298F6452DEDE437AABD10340F` |
| `frozen/public/public-manifest.json` | `09DAB81AE437994A19565FDF3592A085CF5719B98A6034A7526E177304E035B8` |
| `frozen/private/hidden-answer-key.json` | `BED020CC7D1398FC828B6E86724EE4D5545CC343BC468D39B51226C60733D464` |
| `frozen/private/red-team-design.json` | `A509E0C1D8B02F5C032696C98F912A23C4664E5B4280F85CFED7C77B69595975` |
| candidate `SKILL.md` | `95E937FF0D7A60CCAB4BC858744D1720A3EB38D894E42F24789A79A713AA1E1C` |
| candidate handoff validator | `ECAB14961C9AFB71A5F15DC894DAE1FF46D3ADDCF36BBC52A17503625A945FF3` |
| audit monitor | `DB70EA33437B58FF219CF6F77F564999C02FD37D19F3C48899511CB250FE1D49` |
| static compatibility validator | `7011AE9928F8FF4D6EA683549B8F867AA5CB43F9CAD8563E901D3470F809B196` |

## Execution evidence

- `prepare` created all 112 immutable attempt metadata records.
- Monitor self-test passed (`caseCount=12`); harness self-test passed; the
  harness regression suite reported 12 tests `OK`; Python compilation passed.
- Author wrapper finished with `status=AUTHORS_COMPLETE`, `authorExitCode=0`,
  `recordedAttemptCount=112`, and no wrapper errors.
- The external audit summary finished `PASS`: 81 related static-validator
  starts were observed, all 81 were classified allowed, product/runner
  process violations were 0, forbidden action markers were 0, monitor errors
  were 0, and Dev/Original repository mutation was 0. The audit fingerprints
  were unchanged (`Dev=97E81D32B45AF1C4CAD8DDD48DDA136EB01B6DFAA2ECB203C9329BCCCA3BE578`,
  `Original=46EA0A5F0CBEAC1DC6660C0393EA70AEFA926FD2C020F96DB161EB89A94B9B83`).
- The mechanical pre-review score was written to
  `score/checkpoint-112-pre-review/round1-summary.json`.
- The Reviewer wrapper was invoked after Authors/audit completion but stopped
  before spawning a reviewer: exit `2`, `recordedReviewCount=0`. The direct
  runner error was `unexpected pre-review fatal issues`; no reviewer context or
  reviewer evidence was created.

## Mechanical pre-review score

The score is **`INCOMPLETE` (exit code 2)**, not a qualification pass. The
available author-side metrics were:

| Gate/metric | Observed | Contract | Result |
|---|---:|---:|---|
| Structure pass | 95/112 (`0.848214`) | 112/112 (`1.0`) | FAIL |
| Clear expected-answer pass | 65/80 | 80/80 | FAIL |
| Critical red fail-closed | 19/32 (`0.59375`) | 32/32 (`1.0`) | FAIL |
| Session consistency | 70/80 (`0.875`) | >= 0.8 | PASS |
| Unsupported/invented findings | 0 | 0 | PASS |
| Observed product actions | 0 | 0 | PASS |

The scorer recorded 112 completed run records with no timeout or model-error
records. The score's fatal issues were:

1. `AUDIT_ACTIVE_STATE_INVALID_OR_NOT_STOPPED` (the summary containing the
   invalid event was not accepted as a valid audit summary by the scorer).
2. `AUDIT_ALLOWED_PROCESS_SCAN_ROOT_INVALID: 20260903T032852638Z-69a7e942:event:72`.
3. `AUDIT_COVERAGE_INCOMPLETE: 0/112 recorded attempts`.
4. Missing reviewer evidence at
   `reviews/reviewer-evidence.json`.

## Failure classification

### Audit evidence boundary

The monitor observed event 72 as a canonical
`RecipeXmlCompatibilityCheck.exe` process, but its command line used the
benchmark root as the third argument instead of an assigned attempt root. The
frozen author protocol explicitly requires the assigned attempt root as the
scan root. The monitor therefore reported audit `PASS` because the process
identity was allowed, while the scorer rejected the scan-root contract and
could not use that audit summary for 112/112 coverage. This evidence is
preserved; the log was not rewritten to manufacture coverage.

### Author/candidate result signals

The 112 author results also expose candidate and contract-facing failures:

- Clear cases: 15/80 did not match the hidden expected answer. Observed classes
  include extra `Blob`/`LineDistance` tools, `PROPOSED`/`WAIT`/`REJECTED`
  status drift, three missing `candidate.pipeline.xml` bundles, and the
  associated delivery/static-validation-link failures.
- Red cases: 13/32 did not match the hidden fail-closed answer. Observed
  classes include five independent handoff-validator failures caused by
  malformed upstream evidence/pending-packet shapes, a retained-baseline
  drift case, one referenced-file hash mismatch, and status/reason mismatches.
- Three red cases (`RT22`, `RT31`, `RT32`) expose a frozen-vocabulary conflict:
  the hidden contract expects historical `AUTO_REJECTED_*` aliases while the
  v0.1.6 candidate intentionally rejects those aliases in favor of the public
  vocabulary. This is recorded as a contract/candidate decision point, not
  silently normalized.

These author-side signals are diagnostic only. Because the audit summary was
not scorer-valid and Reviewer evidence was absent, no final reviewer-backed
qualification score exists for this identity.

## Lifecycle decision and next priority

The candidate is **not admitted, activated, or qualified**. The benchmark root
is retained as immutable negative/incomplete evidence. Product execution and
Original-repository work remain outside this result.

Next priority is a separately scoped evidence-triage/correction decision that
must first decide whether to preserve this negative evaluation as authoritative
or create a new candidate version and new benchmark identity. Any new identity
must address the audit scan-root failure without editing this root and must
explicitly resolve the `AUTO_REJECTED_*` vocabulary conflict and malformed
upstream-envelope validator behavior before another 112/16 run.

## Closure record

Status: **Incomplete**  
Scope: v0.1.6 fresh Round 1 author execution, external audit, mechanical
pre-review score, and Reviewer guard evaluation.  
Acceptance criteria: Authors 112/112 and audit PASS — **met**; valid scorer
coverage, 16 independent reviews, and final qualification score — **not met**.  
Verification: `prepare`, monitor/harness self-tests, 12-test harness regression,
Python compilation, author wrapper, audit summary, pre-review score, and
Reviewer guard output as listed above.  
Evidence: this report; `runner/wrapper-status.json`; the audit run summary;
`score/checkpoint-112-pre-review/round1-summary.json`; and
`runner/reviewer-wrapper-status.json` under the benchmark root.  
Boundary / next dependency: candidate remains explicit-only/inactive; no
Reviewer-backed qualification, product runtime parity, human comparison,
activation, release, or deployment is proven.
