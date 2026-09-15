# OpenVisionLab Recipe XML Handoff 0.1.5 — Round 1 Result

Date: 2026-09-02 KST  
Repository: `C:\Git\2D\Dev`  
Benchmark: `openvisionlab-rule-based-round1-v015-20260902`  
Evidence root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v015-20260902`  
Candidate: `openvisionlab-recipe-xml-handoff 0.1.5` (`candidate`, explicit-only, outside normal dispatch)

## Outcome

The fresh frozen Round 1 evaluation completed with final scorer status
`FAIL` and exit code `1`. This is complete negative evidence for candidate
`0.1.5`: it is not promotable, activated, or qualified. The result does not
authorize normal dispatch, XML Import, Preview/Run, Recipe or layer/routing
mutation, product qualification, Round 2, Original-repository work, commit,
push, release, or deployment.

## Project identity and scope

OpenVisionLab is an OpenCvSharp4 deterministic rule-based vision Recipe
workbench. Its normal operator path is sample -> PropertyGrid teaching ->
Pipeline composition -> explicit Preview/Run -> drawing/metric/layer review ->
N-sample validation -> saved Recipe. The product evidence supports an
RC/pre-production assessment in the recorded environments, not commercial GA.
Camera/lighting, PLC/I/O, MES, account/cloud control, deployment, calibrated
field metrology, runtime parity, and human-superiority claims were outside this
benchmark.

The prior document-intake evidence remains the frozen corpus authority at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\document-audit-20260831-phase1\document-read-manifest.json`.

## Frozen execution

| Item | Frozen execution |
| --- | --- |
| Clear authoring | 16 cases x 5 fresh contexts = 80 attempts |
| Critical red-team | 32 cases x 1 fresh context = 32 attempts |
| Author model | `gpt-5.6-luna`, reasoning `medium`, timeout 900 seconds |
| Independent review | 16 fresh blinded contexts; one clear case and two red cases per reviewer |
| Product actions | Product EXE, Import, Preview, Run, Recipe/layer/routing mutation forbidden |
| Runtime/human claims | Runtime parity not measured; human comparison not evaluated |

## Integrity and coverage

- Authors: `112/112` recorded and `COMPLETED`; timeout `0`; model error `0`;
  missing run record `0`; author wrapper exit `0`.
- Author audit: run
  `20260901T224617364Z-7bb80b51`, result `PASS`; observed related process
  starts `71/71`, process violations `0`, forbidden action markers `0`, monitor
  errors `0`, and repository mutations `0`. Dev and Original fingerprints were
  unchanged across the author interval: Dev
  `7F71F5DB945FB9BBBABF95EFB13C9D1BB97740E305FC4FD794387128AB17965E` at both
  ends; Original
  `46EA0A5F0CBEAC1DC6660C0393EA70AEFA926FD2C020F96DB161EB89A94B9B83` at both
  ends.
- Reviewers: `16/16` task reviews, exact `112`-attempt coverage, reviewer
  wrapper exit `0`; independent clear-reviewer task pass `10/16`.
- Final scorer: executed after the reviewer evidence binder was present; no
  fatal or incomplete-evidence issue remained in the final score.
- No product EXE or repository mutation was performed by the benchmark.

## Final gates

| Gate | Required | Observed | Result |
| --- | ---: | ---: | --- |
| Structure/contract | 112/112 | 73/112 (65.18%) | FAIL |
| Unsupported or invented findings | 0 | 69 | FAIL |
| Observed product actions | 0 | 0 | PASS |
| Critical red-team fail-closed | 32/32 | 20/32 (62.50%) | FAIL |
| Independent clear reviewer | >=13/16 | 10/16 | FAIL |
| Session consistency | >=80% | 78/80 (97.50%) | PASS |

The final score also recorded `clearExpectedAnswerPass=51/80`,
`completed=112`, `timeout=0`, and `modelError=0`.

## Evidence-backed failure classification

These are observations from this frozen identity, not claims about unseen
requests or human performance.

1. **Artifact finalization and canonical projection remained unstable.**
   Thirty-nine attempts failed the contract check. The most common observed
   forms were missing XML or artifact manifests, artifact-set/order mismatch,
   a final `WAIT`/`REJECTED`/`PROPOSED` state where the clear case expected a
   file-backed measurement-only handoff, and a retained validator result that
   did not agree with the handoff.
2. **The frozen Matching fixture exposed an integrity defect.** The supplied
   `matching-packet.json` retains hashes for `registration-manifest.json` and
   `operator-lock.json` that do not match their frozen bytes. The v0.1.5
   validator correctly fail-closed on this evidence, but clear S4 attempts then
   omitted XML under the blocked-artifact policy while the Round 1 clear
   artifact contract still required a candidate XML. This is a frozen-corpus
   contract/data-integrity finding that must be resolved before another
   benchmark; this run's evidence root is not rewritten.
3. **Closed-world ownership and projection still leaked unsupported claims.**
   Independent reviewers recorded 69 unsupported/invented findings. Recurring
   forms were unsupported frame identities, unowned ROI/threshold/area or gate
   values, semantic image assertions, validator/runtime claims without retained
   evidence, and stale or mismatched evidence pointers.
4. **Red-team routing still mismatched the public state/reason contract.**
   Twelve of 32 red attempts did not match the frozen expected status/reason
   outcome. The observed classes include wrong `WAIT` versus `REJECTED` or
   `PROPOSED` precedence, generic schema/hash routing instead of the public
   reason vocabulary, per-image override/acceptance synonyms, and safe-baseline
   or upstream-evidence drift being treated as a different action.
5. **Reviewer-level semantic/parameter checks failed six clear tasks.** The
   failed tasks were `S1-04`, `S3-01`, `S4-01`, `S4-02`, `S4-03`, and `S4-04`.
   The valid selected tasks otherwise passed their tool-family, physical-target,
   frame/ROI, parameter, and no-invention judgments as recorded in the review
   JSON. The S4 failures were dominated by the frozen Matching hash/frame
   problem and a `RotateScale` parameter/graph projection issue.

## Comparison with v0.1.4

| Metric | v0.1.4 | v0.1.5 | Delta |
| --- | ---: | ---: | ---: |
| Structure pass | 84/112 (75.00%) | 73/112 (65.18%) | -11 attempts, -9.82 pp |
| Unsupported/invented | 58 | 69 | +11 |
| Red fail-closed | 16/32 (50.00%) | 20/32 (62.50%) | +4, +12.50 pp |
| Clear reviewer pass | 12/16 | 10/16 | -2 |
| Product actions | 0 | 0 | unchanged |
| Session consistency | 74/80 (92.50%) | 78/80 (97.50%) | +4, +5.00 pp |

The v0.1.5 changes improved red fail-closed coverage and session consistency,
but regressed structure, unsupported/invented findings, and clear reviewer
pass. None of the four failed promotion gates was met.

## Evidence identity

| Evidence | SHA-256 |
| --- | --- |
| `frozen/public/public-manifest.json` | `74DE98C45270201466E0F5888291B4483844543746088A3C65434ED2B953CE7A` |
| `frozen/private/freeze-record.json` | `76DEF5570F76FF901C28C3CBD2F98B66E4647D573493B50BD87BCB96C0CE024E` |
| `frozen/public/artifact-contract.json` | `3002E6AF28326FEF7695F11DB9BE77ECBFF175A96EA4CBEC45789BDA100F7D9F` |
| `frozen/private/reviewer-protocol.json` | `852337BC83BB44897D800CACB31F47AE3A12F21A107188C3933F7FFE4C64F350` |
| `harness/expected-attempts.json` | `2D512EA904987FB4BAC40407AAFE30EF6E6079E24EDE32061C3B0387304C0562` |
| `score/checkpoint-112-pre-review/round1-summary.json` | `B1293DFBA254D3F29DAEA9E12747C5F6F770DC1EC1FDE00385A07FD1DBEFE91A` |
| `score/round1-summary.json` | `0AF56A91767F739D8DC8F8676BCE63E53D64AC70BCB4AF1A83BF80E4FD9B4981` |
| `score/round1-summary.md` | `D6E006E4CE34A2A7F1FF1F836DB0EC75C6E353C2371BB0ED3D3B32A539359009` |
| `reviews/reviewer-evidence.json` | `97CA6BCF741AF933C959FEF00770187D5511E8E39ABE02296477A32773805C1B` |
| `audit/runs/20260901T224617364Z-7bb80b51/summary.json` | `0251DE5C36B0901BFA22E6EEE7FE28DFC7087DD993BA0E812283908D5B6440F3` |
| `runner/wrapper-status.json` | `CA888F36BA8DB27C3FEB738FE7C82315362F7F5629A28ED0994565EBD91168AB` |
| `runner/reviewer-wrapper-status.json` | `29EA3A36E099B99489CA5F52BEF81A2BBF5E43871772D4201069A9C1DEBF172A` |
| Candidate validator (`0.1.5`) | `9994A4C8042689B2A68130DFE66FCF35C94DD19326FF8ADB9B24277A8B58BC81` |
| `frozen/public/dependencies/fixture-locator/matching-packet.json` | `94F829395930AC7BB9C053892CB789817833758D1DB1B6273B220BAD6EE6188C` |

## Decision and next priority

The candidate remains `candidate`, explicit-only, outside normal dispatch,
inactive, and unqualified. Do not start Round 2 or another full benchmark yet.
The next priority is a separately scoped evidence-triage and correction
decision that first resolves the frozen Matching fixture/hash and clear
artifact-contract contradiction, then addresses the remaining finalization,
ownership, and red-routing failures without changing the benchmark root.

Recommended model: `gpt-5.6-terra`; reasoning effort: `high`.

Activation, normal dispatch, product execution, qualification, release, and
deployment remain separate approvals.

## Closure record

Status: **Complete**  
Scope: frozen v0.1.5 Round 1 authoring/reviewer benchmark execution and
evidence-backed result recording.  
Acceptance criteria: author coverage `112/112`, audit closure `PASS`, review
coverage `16/16` with `112` attempts, final scorer execution, gate metrics,
hashes, comparison, and lifecycle boundary recorded.  
Verification: author wrapper exit `0`; author audit `PASS`; reviewer wrapper
exit `0`; final scorer exit `1`; final score contains no fatal/incomplete
evidence issue.  
Evidence: this report and the D-drive benchmark root above, including frozen,
score, review, audit, and runner artifacts.  
Boundary / next dependency: candidate remains explicit-only, inactive, outside
normal dispatch, and unqualified; the next triage/correction decision is not an
activation or new benchmark authorization.
