# OpenVisionLab Recipe XML Handoff 0.1.11 — Round 1 Result

Date: 2026-09-04 KST  
Repository: `C:\Git\2D\Dev`  
Benchmark root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v011-strict-corpus-20260904`  
Benchmark ID: `openvisionlab-rule-based-round1-v011-strict-corpus-20260904`  
Status: **INCOMPLETE — Authors, audit, Reviewers, and final scoring completed; quality gates failed**

## Scope and lifecycle boundary

This is the separately admitted static Round 1 execution of the installed
`openvisionlab-recipe-xml-handoff 0.1.11` candidate. The frozen corpus,
freeze record, and attempt metadata were not changed during execution. The run
covered 112 Authors, the external process/repository audit, pre-review scoring,
16 independent Reviewer tasks, and final scoring.

No OpenVisionLab product EXE, Import, Preview, Run, Recipe/layer mutation,
activation, normal dispatch, Round 2, Original-repository change, commit, push,
release, deployment, runtime-parity evaluation, or human comparison was
performed. The candidate remains `candidate`, `EXPLICIT_ONLY`, outside normal
dispatch, inactive, and unqualified.

## Admission and frozen identity

- Strict preflight passed before `prepare`: contract/self-test, direct and
  projected baseline, static compatibility, harness/runner checks, Python
  compilation, and exact 112-record metadata freeze gate.
- The frozen corpus contains 16 clear cases × 5 attempts (`80`) plus 32
  red-team attempts (`112` total). The freeze root is immutable.
- The prior failed v0.1.10 root remains present and immutable; its freeze-record
  hash was rechecked as
  `06B66F9C349FD615FB8F9108BF28E0C44D32F3717DBFCACD6825F7308F1C74BD`.

Primary v0.1.11 frozen hashes:

| Artifact | SHA-256 |
| --- | --- |
| `frozen/private/freeze-record.json` | `0DF579083B7F72C473A2C97B5C875FBE97FEB152101E9615421C4CAC308845D8` |
| `frozen/public/public-manifest.json` | `9D5D5745B2C0861EEB389EE17722B23A78AB512E09592F2BF806CB8C19A1A41D` |
| `frozen/private/hidden-answer-key.json` | `975F87D58D6C22CE571403D4A2DC337E203CA6A28FB5FEE877347F30AFAF5533` |
| `frozen/private/red-team-design.json` | `4676706B2CFF399EB628DE227FC79C3D2EF3529F9B9C8CEA0714822AF3210163` |

## Authors and audit

The Authors wrapper completed with `AUTHORS_COMPLETE`, exit code `0`, and
`recordedAttemptCount=112`. Outcomes were `111 COMPLETED` and one frozen
timeout: `S2-04-05`. The external audit completed `PASS`: 92 observed/allowed
related process starts, zero product or runner violations, zero forbidden
action markers, zero monitor errors, and zero repository mutations. Dev and
Original fingerprints were unchanged during the run.

Evidence:

- Wrapper: `runner/wrapper-status.json`
- Wrapper SHA-256: `17C913C514A61FF56C3BEC7488E27A4AC22282E1535EA37802CB614E4E90D594`
- Audit summary: `audit/runs/20260904T054556426Z-58854d98/summary.json`
- Audit summary SHA-256: `B42A8A21B2E19271A92AFACADCFB4F70EC22F204E428740C8457BA2D8D5B35F2`

## Pre-review and Reviewer phases

The pre-review scorer returned the expected evidence-gated `INCOMPLETE` exit
code `2` because the Reviewer aggregate did not yet exist. Its summary covered
all 112 attempt IDs and raised only `REVIEW_EVIDENCE_MISSING`; its SHA-256 is
`37BF9CE51772C6676907D5EDFAA56B038D6612CB1BE9152FC96DDF760EFD4632`.

The independent Reviewer wrapper then completed with `REVIEWS_COMPLETE`, exit
code `0`, and `recordedReviewCount=16`. The aggregate is blinded and independent
with 112 attempt reviews. Reviewer wrapper SHA-256 is
`8D00A0502F7AE093B3381CB8086C69E8E5924492135CDBC5BBE384C2EC65081E`, and
`reviews/reviewer-evidence.json` SHA-256 is
`CE85E615041A0CB8FAAEFF9CF2CD23E563FCF83780D73F114950CD4B3CD3444B`.

The final aggregate passed 14/16 clear Reviewer tasks. The two failed tasks
were `S3-04` and `S4-04`; all 16 reviewer contexts were distinct from Authors
and the final evidence binder accepted the full 112-attempt coverage.

## Final mechanical score

The final scorer returned `FAIL` with exit code `1` after all execution evidence
was present:

| Gate/metric | Observed | Result |
| --- | ---: | --- |
| Recorded attempts | `112/112` | PASS |
| Completed / timeout | `111 / 1` | execution complete; timeout retained |
| Structure pass | `108/112` (`0.9642857143`) | FAIL |
| Clear expected-answer pass | `77/80` | FAIL |
| Critical red fail-closed | `29/32` (`0.90625`) | FAIL |
| Clear Reviewer task pass | `14/16` | PASS |
| Unsupported/invented findings | `16` | FAIL |
| Product-action findings | `0` | PASS |
| Session consistency | `77/80` (`0.9625`) | PASS |

Observed failure classes are:

- Structure: `S2-04-05` timed out; `RT19-01`, `RT20-01`, and `RT32-01` used
  reason codes different from their public evidence pattern.
- Clear expected answers: `S4-03-01` and `S4-03-03` added a second Threshold
  and separate mask path instead of the locked shared `Threshold -> Blob ->
  Blob` graph. The timeout accounts for the remaining clear miss.
- Unsupported/invented findings (16): eight ownership/evidence projections
  (`S1-01-02`, `S4-03-04..05`, `S4-04-01..05`), four parameter/graph
  projections (`S2-01-05`, `S3-04-02`, `S4-03-01`, `S4-03-03`), and four
  red reason/semantic projections (`RT19-01`, `RT20-01`, `RT28-01`,
  `RT32-01`). The scorer records every finding verbatim.
- Reviewer task failures: `S3-04` rejected the parameter policy because its
  XML introduced unsupported GAP/`PIXELPERMM` values; `S4-04` rejected the
  parameter policy and no-invented-claim judgment because the NormalizeImage
  lock owner was projected as `OPERATOR` rather than `OPERATOR_LOCK`.

Score evidence:

- JSON: `score/final/round1-summary.json`
- JSON SHA-256: `81E59CF9ECAF9F57063748E2B58617FD5C9513D63243B8918AF3D12BCA3E9A35`
- Markdown: `score/final/round1-summary.md`
- Markdown SHA-256: `1D5EF51C514D95C7B5AC2423A064F41032F2AFFB79676E7F042A1BFAFB709452`
- Final scorer exit record: `score/final/mechanical-score-exit-code.txt` (`1`)

The final scorer gates are: structure `FAIL`, unsupported/invented `FAIL`,
observed product actions `PASS`, critical red fail-closed `FAIL`, clear
Reviewer tasks `PASS`, and session consistency `PASS`. Runtime parity is
`NOT_MEASURED` and human comparison is `NOT_EVALUATED`.

## Decision and next dependency

This is a complete negative quality evaluation, not a backend-blocked run. The
frozen root and all Author/Reviewer evidence remain immutable. Do not activate,
dispatch, qualify, or promote v0.1.11. The next skill priority is a separately
scoped evidence-triage and candidate-only correction decision for the observed
timeout, public reason-code residuals, shared-mask/graph projection, and
NormalizeImage ownership failures, followed by a new frozen identity if a rerun
is approved. In-place repair of this root, hidden case-to-reason lookup tables,
activation, product execution, Round 2, runtime parity, human comparison,
Original-repository work, commit, push, release, and deployment remain
unauthorized.

## Closure record

Status: **Incomplete**  
Scope: v0.1.11 Round 1 Authors, audit, independent Reviewers, and final scoring.  
Acceptance criteria: `112/112` Authors, audit `PASS`, and `16/16` Reviews — **met**; all quality gates and candidate qualification — **not met**.  
Verification: strict preflight, Authors wrapper, audit summary, pre-review scorer, Reviewer wrapper/evidence, final scorer JSON/Markdown, and frozen-hash rechecks.  
Evidence: the benchmark root and hashes listed above.  
Boundary / next dependency: candidate-only failure triage and a new owner-approved freeze are required before any rerun; candidate activation, runtime parity, qualification, release, deployment, and Original-repository work remain unapproved.
