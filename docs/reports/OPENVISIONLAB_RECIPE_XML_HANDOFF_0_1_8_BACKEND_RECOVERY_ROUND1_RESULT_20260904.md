# OpenVisionLab Recipe XML Handoff 0.1.8 — Backend-Recovery Round 1 Result

Date: 2026-09-04 KST  
Repository: `C:\Git\2D\Dev`  
Benchmark root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v018-backend-recovery-20260904-r2`  
Benchmark ID: `openvisionlab-rule-based-round1-v018-backend-recovery-20260904-r2`  
Status: **FAIL — all 112 author attempts, audit, and 16 blinded reviews completed; final quality gates did not pass**

## Scope and lifecycle boundary

This is the fresh, separately identified static Round 1 execution of the
installed `openvisionlab-recipe-xml-handoff 0.1.8` candidate after the Codex
backend health prerequisite recovered. The frozen corpus was copied into a new
identity, reconciled before freeze, and treated as immutable for execution.
The run covered Authors, the external process/repository audit, the mechanical
pre-review score, and the independent Reviewer phase.

No OpenVisionLab product EXE, Import, Preview, Run, Recipe or layer/routing
mutation, activation, normal dispatch, Round 2, Original-repository change,
commit, push, release, or deployment was performed. Runtime parity and human
comparison were not evaluated. The candidate remains `candidate`,
`EXPLICIT_ONLY`, inactive, unqualified, and outside normal dispatch.

## Admission and frozen identity

- The current Codex binary was `C:\Users\USER\AppData\Local\OpenAI\Codex\bin\994e8469124a0d31\codex.exe` (`codex-cli 0.153.0-alpha.5`). A D-drive-only health check passed before this identity was prepared.
- Strict preflight passed before `prepare`: freeze-before-prepare ordering,
  contract validation, direct/projected/static baseline validation, harness and
  runner checks, Python compilation, and `112/112` final-hash-bound metadata.
- Authors used `gpt-5.6-luna` / `medium`, 16 clear cases × 5 attempts plus 32
  red-team attempts, fresh context per attempt, and a 900-second timeout.
- Reviewers used `gpt-5.6-sol` / `high`, read-only, with 16 independent clear
  task reviews.

Primary frozen hashes:

| Artifact | SHA-256 |
| --- | --- |
| `frozen/private/freeze-record.json` | `EE04BECDA9528969C5825C8FA9D42E865D2163665F643A135685D76789B17E52` |
| `frozen/public/public-manifest.json` | `5D473419E77D401F8D040222CF64B482741C8C8E064BBADA04FECF3FEDCD7C37` |
| `frozen/private/hidden-answer-key.json` | `92F7488D910DD36AFD5E23D842420DB5E518930A2B2967ACE6BBA32AEA6C243C` |
| `frozen/public/red-author-protocol.json` | `CA7ADB8F3F627FAF420595A015CD4F3DA7CEC6EFC50684526DE50171CAFD4D18` |
| `frozen/public/artifact-contract.json` | `F66DAB3CA483F29B71523369B98D389A35E45406CA920E79A7E26404C94FAA56` |
| `frozen/public/dependencies/fixture-locator/matching-packet.json` | `68CA2333030DD6EDB550612F23363B8258521819FE1B64D022FFCFCED745B00A` |

The v0.1.8 candidate resources used by this run were the pre-correction
snapshot recorded in the v0.1.8 strict-preflight report. They must not be
confused with the subsequently installed v0.1.9 candidate.

## Authors and audit

The Authors wrapper completed with `AUTHORS_COMPLETE`, exit code `0`, and
`recordedAttemptCount=112`. The external audit completed `PASS` with no
forbidden actions or repository mutation. There were no timeout or backend
model errors in this recovered run.

Evidence:

- wrapper: `runner/wrapper-status.json`
- audit summary: `audit/runs/20260903T174546079Z-dd1fdc5b/summary.json`
- wrapper SHA-256: `0D55313D2CB17AA188E4ADCB4BD1C19BC9893B61ADFDB430626E026BFC13F137`
- audit summary SHA-256: `18EFAFC536D2881F9673F002B933C0A66F75CE52BE1DA8680A408DE3EE5F4AB8`

## Reviewer phase

The independent Reviewer wrapper completed with `REVIEWS_COMPLETE`, exit code
`0`, and `recordedReviewCount=16`. Review evidence is retained at
`reviews/reviewer-evidence.json` with SHA-256
`FF3D6B29217D8618AC46B112F83353A28A8F2CA73C30016D2DC19EBA7A7FB6DF`.

Five clear tasks did not receive an overall reviewer pass (`S1-04`, `S2-01`,
`S2-04`, `S3-03`, and `S4-03`). The reviewers identified 30 candidate-owned
unsupported or invented claims, principally:

- invented binary/threshold/mask/source-image frames and frame changes without
  a transforming Tool;
- extra Blob/Contour/component-count work beyond an explicit mask-only graph;
- image paths used as owners of case policy, or specialist packet values
  labelled as Tool defaults;
- `External` contour prose without serialized `DetectMode=External`;
- inconsistent projection axes, intermediate status prose, or `REVIEWED`
  evidence claims while stages remained `NOT_REVIEWED`; and
- proposed runtime drawings/measurements in a bundle whose execution was not
  authorized.

Four red attempts also exposed a public-contract/scorer expectation conflict:
the final scorer expected a different reason spelling for RT12, RT17, RT23, and
RT30 than the supplied public protocol and the installed candidate's generic
reason policy. RT30 additionally retained an upstream fixture with pending
packet identity and an empty-evidence `PASS` stage, so the independent validator
correctly rejected that fixture. These conflicts require a separately scoped
corpus/contract decision; the candidate must not learn a hidden case-to-reason
table or emit local aliases.

## Final mechanical score

The final scorer returned `FAIL` (exit code `1`), not an incomplete execution:

| Gate/metric | Observed | Result |
| --- | ---: | --- |
| Recorded/completed attempts | `112/112` | PASS |
| Structure pass | `108/112` (`0.9642857143`) | FAIL |
| Clear expected-answer pass | `79/80` | FAIL |
| Critical red fail-closed | `28/32` (`0.875`) | FAIL |
| Clear reviewer task pass | `11/16` | FAIL |
| Unsupported/invented findings | `34` | FAIL |
| Product-action findings | `0` | PASS |
| Session consistency | `79/80` (`0.9875`) | PASS |

The scorer gates were: structure `FAIL`, unsupported/invented `FAIL`, observed
product actions `PASS`, critical red fail-closed `FAIL`, clear reviewer tasks
`FAIL`, and session consistency `PASS`. Runtime parity was `NOT_MEASURED` and
human comparison was `NOT_EVALUATED`.

Score evidence:

- JSON: `score/final/round1-summary.json`
- Markdown: `score/final/round1-summary.md`
- JSON SHA-256: `F3ABF0554A208A315FDA9460D6720262E33467B12337A9D3964021D5685918F5`
- Markdown SHA-256: `5E39F615711FAC703C094809EB89742511B0B8F59279B823046587A5BF0A184C`

## Decision and next dependency

This is a complete negative evaluation of the v0.1.8 candidate, not a partial
run. The frozen root and all author/reviewer evidence remain immutable. The
candidate is not activated, qualified, or promoted.

The candidate-only v0.1.9 correction is separately focused on the observed
unsupported-claim and serialization-boundary failures. Its installed-skill
focused validation is recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_9_FOCUSED_VALIDATION_20260904.md`.
A new Round 1 admission requires a new owner-approved freeze/preflight identity
after that focused checkpoint. The public reason vocabulary must remain
pattern-based until the RT12/17/23/30 contract conflict is explicitly resolved.

## Closure record

Status: **Incomplete**  
Scope: v0.1.8 backend-recovery Round 1 Authors, audit, independent Reviews, and final scoring.  
Acceptance criteria: `112/112` Authors, audit `PASS`, and `16/16` Reviews — **met**; all quality gates and candidate qualification — **not met**.  
Verification: strict preflight, Authors wrapper, audit summary, Reviewer wrapper/evidence, and final scorer JSON/Markdown.  
Evidence: the D-drive paths and hashes listed above.  
Boundary / next dependency: v0.1.9 focused correction is validated, but a fresh owner-approved benchmark admission is required; activation, runtime parity, qualification, release, deployment, and Original-repository work remain unapproved.
