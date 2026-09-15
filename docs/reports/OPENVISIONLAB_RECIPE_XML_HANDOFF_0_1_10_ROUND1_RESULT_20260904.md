# OpenVisionLab Recipe XML Handoff 0.1.10 — Round 1 Result

Date: 2026-09-04 KST  
Repository: `C:\Git\2D\Dev`  
Benchmark root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v010-contract-recovery-20260904`  
Benchmark ID: `openvisionlab-rule-based-round1-v010-contract-recovery-20260904`  
Status: **INCOMPLETE — Authors, audit, Reviewers, and final scoring completed; quality gates failed**

## Scope and lifecycle boundary

This is the separately admitted static Round 1 execution of the installed
`openvisionlab-recipe-xml-handoff 0.1.10` candidate. The frozen corpus and
metadata were not changed during execution. The run covered 112 Authors, the
external process/repository audit, pre-review scoring, 16 independent Reviewer
tasks, and final scoring.

No OpenVisionLab product EXE, Import, Preview, Run, Recipe/layer mutation,
activation, normal dispatch, Round 2, Original-repository change, commit, push,
release, deployment, runtime-parity evaluation, or human comparison was
performed. The candidate remains `candidate`, `EXPLICIT_ONLY`, inactive,
outside normal dispatch, and unqualified.

## Admission and frozen identity

- Codex binary: `C:\Users\USER\AppData\Local\OpenAI\Codex\bin\994e8469124a0d31\codex.exe`
  (`codex-cli 0.153.0-alpha.5`). The D-drive-only probe returned exactly
  `HEALTHCHECK_OK` with exit code `0` before execution.
- Strict preflight passed before `prepare`: contract/self-test, direct and
  projected baseline, static compatibility, harness/runner checks, Python
  compilation, and `112/112` final-hash-bound metadata records.
- Frozen corpus: 16 clear cases × 5 attempts (`80`) plus 32 red-team attempts
  (`112` total). The freeze root is immutable.

Primary frozen hashes:

| Artifact | SHA-256 |
| --- | --- |
| `frozen/private/freeze-record.json` | `06B66F9C349FD615FB8F9108BF28E0C44D32F3717DBFCACD6825F7308F1C74BD` |
| `frozen/public/public-manifest.json` | `00B579FE3D30AFB82CC2CC60753B6F59FA0CA9021ADB52A7CBD2E70BF4895DDE` |
| `frozen/private/hidden-answer-key.json` | `7C4D1889FA400BED2C59D976E106BDAF99E3264FA6D016DDE0AF29A5076EC332` |
| `frozen/public/red-author-protocol.json` | `C195E3D4601DC419115B61078856557F7E92132E7D2093606D5AA0F2FE32F300` |
| `frozen/public/artifact-contract.json` | `D27E429F5D67C5B191BDEA579048C3FD7AA598FCF63675D10A5FA5CC0E69FA6D` |
| `frozen/public/dependencies/fixture-locator/matching-packet.json` | `F4B22E7804D7B3886523B1FB09FDDCBAFCDF4FACB0D021A7328504B58499D2B4` |

## Authors and audit

The Authors wrapper completed with `AUTHORS_COMPLETE`, exit code `0`, and
`recordedAttemptCount=112`. The external audit completed `PASS`: 82 allowed
related process starts, zero violations, zero forbidden action markers, zero
monitor errors, and zero repository mutations. Dev and Original fingerprints
were unchanged.

Evidence:

- Authors wrapper: `runner/wrapper-status.json`
- Authors wrapper SHA-256: `147B0997759DDD223EBC46BD63B91C737A6C00B7F5349681D6A8CA2A858D2BA2`
- Audit summary: `audit/runs/20260903T223135115Z-75c10b6e/summary.json`
- Audit summary SHA-256: `73D1BE09581EEB6764255E03BDE85EF94E450AFE60EA2BB543C90A52F62E87D5`

## Pre-review and Reviewer phases

The pre-review scorer returned the expected evidence-gated `INCOMPLETE` exit
code `2` because Reviewer evidence did not yet exist. It recorded all 112
completed attempts and raised only `REVIEW_EVIDENCE_MISSING` for the reviewer
evidence path, so the Reviewer guard admitted the run.

The independent Reviewer wrapper then completed with `REVIEWS_COMPLETE`, exit
code `0`, and `recordedReviewCount=16`. Reviewer evidence is blinded and
independent; its SHA-256 is
`F278E527FCFEB31BAEB83FB703258CCC125A157AF759443E5FDF959477D28812`.
The final scorer counted 14/16 clear Reviewer tasks as passing; `S2-01` and
`S3-03` did not pass the overall Reviewer judgment.

## Final mechanical score

The final scorer returned `FAIL` with exit code `1`, after all execution
evidence was present:

| Gate/metric | Observed | Result |
| --- | ---: | --- |
| Recorded/completed attempts | `112/112` | PASS |
| Structure pass | `100/112` (`0.8928571429`) | FAIL |
| Clear expected-answer pass | `79/80` | FAIL |
| Critical red fail-closed | `20/32` (`0.625`) | FAIL |
| Clear Reviewer task pass | `14/16` | PASS |
| Unsupported/invented findings | `20` | FAIL |
| Product-action findings | `0` | PASS |
| Session consistency | `79/80` (`0.9875`) | PASS |

The 12 red-case misses were `RT02`, `RT06`, `RT09`, `RT11`, `RT13`, `RT19`,
`RT24`–`RT29`. They consist of public reason/status precedence mismatches and
an independent handoff-validator `SourceFrame` versus
`SourcePixelFrame[572x420]` input-frame mismatch. The clear artifact failure is
`S4-02-04`, which omitted the required `candidate.pipeline.xml` and therefore
did not produce the expected tool signature.

The 20 unsupported/invented findings include calibration/ROI/datum ownership,
algorithm-gap and semantic-family claims, graph/frame/parameter projection,
and unprovided `PIXELPERMM`/fixture values. These are recorded verbatim in
`score/final/round1-summary.json`; no product-action findings were observed.

Score evidence:

- JSON: `score/final/round1-summary.json`
- JSON SHA-256: `2A1E9915D5EBDB86B0780A3B91D549081A0EE1C601FF37C11623DCE9BA3CC434`
- Markdown: `score/final/round1-summary.md`
- Markdown SHA-256: `BFC3A07127D5415B54A5A40C5E3D50406228737AE9BF78ADAD20A705FCB6631C`
- Final scorer exit record: `preflight/final-score-exit-code.txt` (`1`)
- Consolidated execution state: `preflight/final-status.json`
- Consolidated execution state SHA-256: `DB0C46DE1D82CD440C3C20FD2C941AB7055B048C321BDC126D183045605B075D`

The scorer gates are: structure `FAIL`, unsupported/invented `FAIL`, observed
product actions `PASS`, critical red fail-closed `FAIL`, clear Reviewer tasks
`PASS`, and session consistency `PASS`. Runtime parity is `NOT_MEASURED` and
human comparison is `NOT_EVALUATED`.

## Decision and next dependency

This is a complete execution with a negative quality-gate result, not a
backend-blocked run. The frozen root and all Author/Reviewer evidence remain
immutable. Do not activate, dispatch, qualify, or promote v0.1.10. A future
correction must first perform evidence-based triage of the observed clear
artifact/Reviewer failures, red reason/status precedence, independent frame
validation, and unsupported/invented claims, then receive a separately approved
new frozen benchmark identity. Hidden case-to-reason lookup tables and in-place
repair of this root remain disallowed.

## Closure record

Status: **Incomplete**  
Scope: v0.1.10 Round 1 Authors, audit, independent Reviewers, and final scoring.  
Acceptance criteria: `112/112` Authors, audit `PASS`, and `16/16` Reviews — **met**; all quality gates and candidate qualification — **not met**.  
Verification: D-drive health probe, strict preflight, Authors wrapper, audit summary, Reviewer wrapper/evidence, and final scorer JSON/Markdown.  
Evidence: the benchmark root and hashes listed above.  
Boundary / next dependency: candidate-only failure triage and a new owner-approved freeze are required before any rerun; activation, runtime parity, qualification, release, deployment, and Original-repository work remain unapproved.
