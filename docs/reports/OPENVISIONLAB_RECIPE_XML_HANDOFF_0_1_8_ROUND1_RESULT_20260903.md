# OpenVisionLab Recipe XML Handoff 0.1.8 — Round 1 Admission Result

Date: 2026-09-03 KST  
Repository: `C:\Git\2D\Dev`  
Benchmark root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v018-preflight-20260903-r1`  
Benchmark ID: `openvisionlab-rule-based-round1-v018-preflight-20260903-r1`  
Status: **Incomplete — Authors wrapper recorded 112 attempts and audit PASS, but only 40 attempts produced valid author evidence; 72 attempts failed with Codex backend connection errors; Reviewer phase was not run**

## Scope and lifecycle boundary

This is the separately admitted static Round 1 execution for the installed
candidate `openvisionlab-recipe-xml-handoff 0.1.8`. It ran the frozen Authors
contract and external process/repository audit, then ran the mechanical scorer.
The scorer did not admit the Reviewer phase because the author evidence was
incomplete.

No OpenVisionLab product EXE, Import, Preview, Run, Recipe/layer/routing
mutation, activation, normal dispatch, Round 2, Original-repository change,
commit, push, release, or deployment was performed. The candidate remains
explicit-only, outside normal dispatch, inactive, and unqualified. The frozen
input and metadata set is retained; this root is not to be repaired in place.

## Frozen identity

- Freeze record was finalized before `prepare`; the preflight metadata gate
  recorded `112/112` final freeze-hash matches.
- Author contract: `gpt-5.6-luna` / `medium`, 16 clear cases x 5 attempts
  (80) plus 32 red-team attempts (112), fresh context per attempt,
  900-second timeout.
- Reviewer contract: `gpt-5.6-sol` / `high`; no Reviewer context was created.
- Candidate lifecycle at freeze: `candidate` / `EXPLICIT_ONLY`; mutation after
  the first attempt was forbidden.

Primary frozen hashes:

| Artifact | SHA-256 |
| --- | --- |
| `frozen/private/freeze-record.json` | `CBEDBF8491CAAD956A7F41DBBB4FE237CA5935B7EAD079ED5C6953A059CFED70` |
| `frozen/public/public-manifest.json` | `DBC2FB39B4DD26DC35BC313E76DD9931C5F5DE3E483AD8AF477013164C8F5A3B` |
| `frozen/private/hidden-answer-key.json` | `DF66681E3B7511BD36497F07F050A273FDC4ECD116AD9550E721EB925151C3A8` |
| candidate `SKILL.md` | `3EAD10B814C1C66DAB327AAA2BE910B7E065C8AE9C1B6E7A1FF936818503ED34` |
| candidate handoff validator | `C93D9C6D1F5E7E935BA04E9B424032F134D774FA52A3EE55DE4B9B42761D0CAE` |
| scorer harness | recorded in the frozen `SCORER_HARNESS` role |

## Author and audit execution

The Authors wrapper finished with `AUTHORS_COMPLETE`, wrapper exit `0`, and
`recordedAttemptCount=112`. The external audit finished `PASS`:

- related process starts: `39` observed, `39` allowed, `0` violations;
- forbidden-action markers: `0`;
- repository mutations: `0`;
- monitor errors: `0`;
- Dev and Original repository fingerprints were unchanged.

The wrapper's recorded outcomes are:

| Attempt group | Outcome | Count |
| --- | --- | ---: |
| `S1-01` through `S2-04` clear attempts | `COMPLETED` | 40 |
| `S3-01` through `S4-04` clear attempts | `MODEL_ERROR` | 40 |
| `RT01` through `RT32` red-team attempts | `MODEL_ERROR` | 32 |

All 72 failed attempts ended with Codex CLI return code `1`. Their stderr and
event logs show repeated Codex backend connection failures: initial HTTP
`503 Service Unavailable` responses followed by HTTP `404 Not Found` for
`/backend-api/codex/responses`. These are execution-environment failures, not
author artifact mismatches, because those 72 contexts did not create the
required artifact sets.

## Mechanical scorer result

The scorer result is **`INCOMPLETE` (exit code `2`)**. It retains all 112
attempts in the denominator.

| Gate/metric | Observed | Contract | Result |
| --- | ---: | ---: | --- |
| Recorded attempts | 112/112 | 112/112 | PASS |
| Valid completed evidence | 40/112 | 112/112 | FAIL |
| Structure pass | 40/112 (`0.3571428571`) | 112/112 (`1.0`) | FAIL |
| Clear expected-answer pass | 40/80 | 80/80 | FAIL; S3/S4 contexts unavailable |
| Critical red fail-closed | 0/32 | 32/32 | FAIL; all red contexts unavailable |
| Unsupported/invented findings | 0 | 0 | PASS |
| Observed product actions | 0 | 0 | PASS |
| Session consistency | 80/80 (`1.0`) | >= 0.80 | PASS, not a completeness signal |
| Reviewer tasks | 0/16 | 16/16 | NOT EVALUATED |

The 40 valid attempts are the S1/S2 clear cases and all passed the scorer's
structure, expected status, expected Tool-family signature, handoff validator,
and static compatibility checks. No valid author evidence exists for S3/S4 or
the 32 red cases in this run.

## Reviewer boundary

The Reviewer wrapper was not launched. The scorer has a fatal incomplete
condition because `reviews/reviewer-evidence.json` does not exist, and the
author evidence is not complete enough to form the required 16 blinded review
packets. Creating synthetic reviewer evidence or rerunning only failed attempts
in this root would violate the frozen-attempt contract.

## Evidence paths

- Author wrapper status:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v018-preflight-20260903-r1\runner\wrapper-status.json`
- Audit summary:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v018-preflight-20260903-r1\audit\runs\20260903T134509321Z-25036435\summary.json`
- Scorer JSON:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v018-preflight-20260903-r1\score\checkpoint-112-pre-review\round1-summary.json`
- Scorer Markdown:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v018-preflight-20260903-r1\score\checkpoint-112-pre-review\round1-summary.md`
- Scorer stdout/stderr:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v018-preflight-20260903-r1\preflight\scorer.stdout.txt` and `scorer.stderr.txt`

## Lifecycle decision and next dependency

This identity is an **incomplete static benchmark result**, not a candidate
qualification or promotion signal. The root and its evidence remain immutable.
The 112 + 16 benchmark must not be resumed by rewriting this root or by
substituting synthetic outputs. A future attempt requires a new identity after
the Codex backend is available and a fresh freeze/preflight gate passes.

Candidate activation, implicit dispatch, product execution, runtime parity,
human comparison, qualification, Original-repository work, commit, push,
release, and deployment remain separate approvals.

## Closure record

Status: **Incomplete**  
Scope: v0.1.8 fresh Round 1 Authors execution, external audit, mechanical
pre-review scoring, and Reviewer admission guard.  
Acceptance criteria: Authors wrapper/audit — **met**; 112 valid author evidence
sets, all scorer gates, 16 independent reviews, and final qualification score —
**not met**.  
Verification: freeze/metadata preflight, Authors wrapper, audit summary,
scorer JSON/Markdown, and per-attempt Codex event/stderr logs.  
Evidence: the D-drive paths listed above.  
Boundary / next dependency: external Codex backend availability and a new
benchmark identity are required; this result does not prove candidate
qualification, activation, runtime parity, human comparison, or product
behavior.
