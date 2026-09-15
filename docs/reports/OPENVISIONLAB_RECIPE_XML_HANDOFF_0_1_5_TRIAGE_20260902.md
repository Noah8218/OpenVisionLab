# OpenVisionLab Recipe XML Handoff 0.1.5 — Round 1 Failure Triage and Correction Decision

Date: 2026-09-02 KST
Repository: `C:\Git\2D\Dev`
Candidate: `openvisionlab-recipe-xml-handoff 0.1.5`
Authoritative benchmark: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v015-20260902`
Benchmark ID: `openvisionlab-rule-based-round1-v015-20260902`

## Triage status and boundary

Status: **Complete** for evidence-backed failure classification and the
separate corpus-repair/candidate-correction decision. This record does not
rewrite the frozen benchmark root, change the installed candidate, activate or
dispatch the skill, run another benchmark, launch the product, Import,
Preview, Run, mutate a Recipe/layer/routing, touch
`C:\Git\2D\Original`, commit, push, release, or deploy.

The v0.1.5 Round 1 result is a complete negative evaluation. The classifications
below describe this frozen identity only. They separate observed facts from
the correction decision and do not claim human performance or unseen-request
behavior.

## Project identity and user goal

OpenVisionLab remains an OpenCvSharp4 deterministic rule-based vision Recipe
workbench. Its normal operator path is sample -> PropertyGrid teaching ->
Pipeline composition -> explicit Preview/Run -> drawing/metric/layer review ->
N-sample validation -> saved Recipe. The candidate is only an optional XML
serialization and validation-handoff boundary; it does not own visual
teaching, datum/ROI/Tool policy, Matching qualification, runtime execution, or
Recipe mutation.

The goal of this checkpoint is to determine which v0.1.5 failures are caused
by the benchmark input/contract and which remain candidate-owned. The candidate
stays `candidate`, explicit-only, outside normal dispatch, inactive, and
unqualified.

## Evidence used

| Evidence | Identity / observed result |
| --- | --- |
| Final scorer | `score/round1-summary.json`, SHA-256 `0AF56A91767F739D8DC8F8676BCE63E53D64AC70BCB4AF1A83BF80E4FD9B4981` |
| Final score Markdown | `score/round1-summary.md`, SHA-256 `D6E006E4CE34A2A7F1FF1F836DB0EC75C6E353C2371BB0ED3D3B32A539359009` |
| Independent reviewer evidence | `reviews/reviewer-evidence.json`, SHA-256 `97CA6BCF741AF933C959FEF00770187D5511E8E39ABE02296477A32773805C1B` |
| Author audit | `audit/runs/20260901T224617364Z-7bb80b51/summary.json`, SHA-256 `0251DE5C36B0901BFA22E6EEE7FE28DFC7087DD993BA0E812283908D5B6440F3` |
| Frozen public manifest | `frozen/public/public-manifest.json`, SHA-256 `74DE98C45270201466E0F5888291B4483844543746088A3C65434ED2B953CE7A` |
| Frozen artifact contract | `frozen/public/artifact-contract.json`, SHA-256 `3002E6AF28326FEF7695F11DB9BE77ECBFF175A96EA4CBEC45789BDA100F7D9F` |
| Candidate validator | `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\scripts\validate_recipe_xml_handoff.py`, SHA-256 `9994A4C8042689B2A68130DFE66FCF35C94DD19326FF8ADB9B24277A8B58BC81` |
| Candidate skill text | `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\SKILL.md`, SHA-256 `4E87570693EA5903D75059E19260D8750A8D30EA9146B3170B9173888647E42E` |
| Frozen Matching packet | `frozen/public/dependencies/fixture-locator/matching-packet.json`, SHA-256 `94F829395930AC7BB9C053892CB789817833758D1DB1B6273B220BAD6EE6188C` |

The author wrapper recorded `112/112` completed attempts with no timeout or
model error. The audit was `PASS`, all `16/16` reviewer tasks covered the 112
attempts, and the final scorer had no fatal or incomplete-evidence issue.

## Final benchmark gates

| Gate | Observed | Result |
| --- | ---: | --- |
| Structure/contract | 73/112 (65.18%) | FAIL |
| Unsupported or invented findings | 69 | FAIL |
| Product actions | 0 | PASS |
| Critical red-team fail-closed | 20/32 (62.50%) | FAIL |
| Independent clear reviewer | 10/16 | FAIL |
| Session consistency | 78/80 (97.50%) | PASS |

The final score also recorded `clearExpectedAnswerPass=51/80`,
`completed=112`, `timeout=0`, and `modelError=0`.

## Integrity finding A — frozen Matching packet is internally hash-invalid

The packet file itself has the frozen manifest hash, but its two nested
evidence references do not match the bytes they name:

| Nested evidence | Packet-declared SHA-256 | Actual frozen-byte SHA-256 |
| --- | --- | --- |
| `registration-manifest.json` | `D9FFA5F34FA1825BE972016C35393D07A6483CF4B605280053F915874F3C077C` | `FFFB1B48EC434F08638E1DCFD257216D8BB180791D471E36E4B277982C32B3E3` |
| `operator-lock.json` | `DCAEC961A845862D9610CA798C20B065DF51132E4D24CF675DCDDF948C7A3889` | `1836A0E123EC89CCDEF36EAAD93DF8D9459DBA044F4B2E39617B507089D13614` |

The same registration and lock bytes in the prior v0.1.4 fixture use these
actual hashes in the packet. This makes the v0.1.5 mismatch a copied/frozen
corpus defect, not evidence that the Matching packet content is semantically
wrong.

The harness recursively validates linked file references. Therefore the
mismatch affects all S4 clear attempts and the packet-linked red integrity
case. The candidate's fail-closed response to this evidence is correct, but a
clear case then cannot satisfy both of the frozen rules below:

- `clearRequiredArtifacts` always requires `candidate.pipeline.xml`;
- `blockedArtifactPolicy` requires no local XML, `NOT_EMITTED`, and
  `NOT_RUN` for `WAIT` or `REJECTED`.

This is a conditional fixture-to-contract incompatibility. It is not a reason
to relax the blocked-artifact policy or emit fabricated XML. The v0.1.5 root
must remain immutable; a new benchmark identity must regenerate the packet's
transitive hashes before S4 is scored again.

## Integrity finding B — safe-static-baseline wrapper and nested handoff disagree

The frozen `safe-static-baseline.json` wrapper points to the copied
`positive-v0.1.2` files, while the nested `positive-v0.1.2\handoff.json`
still points to the `positive-v0.1.1` paths. The XML, validation-report, and
upstream hashes are equal, but all three pointer paths differ. The candidate
validator checks wrapper/nested pointer equality; the benchmark scorer's
safe-policy comparison reads the nested handoff; the blinded reviewer sees the
wrapper identity. This produces incompatible evidence expectations for the
RT13/RT24/RT25/RT26/RT27/RT28 baseline family.

The correction is to repair the fixture in a new benchmark copy so the wrapper,
nested handoff, and retained files use one canonical pointer set and matching
hashes. Do not teach the candidate to guess which stale pointer wins, and do
not rewrite the v0.1.5 evidence root.

## Candidate-owned residual failure classes

The following remain candidate behavior after the two fixture defects are
removed. The counts are an attempt-level triage of the 69 reviewer findings,
not a re-scored benchmark: 37 findings are on packet-affected attempts, nine
are on the baseline-fixture family, and 23 are residual clear/red projection
or routing findings.

### F1 — Artifact finalization is still not operationally closed

Thirty-nine attempts failed the contract check. `S1-04` selected an invalid
majority because XML or manifests were missing; `S3-01` selected an invalid
majority and one valid attempt added an unsupported `LineDistance` stage.
Other attempts claimed a static compatibility pass while no candidate XML was
delivered, omitted `artifact-manifest.json`, or disagreed across raw response,
handoff, validator, and XML state.

The v0.1.5 prose describes a finalization gate, but the authoring behavior still
allows a static-validator narrative to be written before the required bundle
is actually closed. This is a candidate correction, not a reason to weaken
the benchmark contract.

### F2 — Closed-world ownership is not consistently projected

The independent reviewers recorded 69 unsupported/invented findings. Beyond
the packet/baseline fixture families, the residual forms include invented
`BinaryFrame`/`CleanedMaskFrame`/`ContourFrame` identities, unowned ROI or
threshold values, unsupported semantic image claims, an added helper
`LineDistance` stage, and runtime or validator claims without retained
evidence. One attempt also serialized `Y_RTOL` where the locked projection is
`R_TO_L -> X_RTOL`.

The candidate must keep the reviewed upstream graph and every field owner
closed-world. An upstream envelope is not self-authenticating merely because
it exists; unowned values must be omitted or returned to the upstream owner.

### F3 — Public state/reason and baseline-action precedence still drift

Twelve red attempts did not match the frozen status/reason outcome. The
residual patterns are:

- treating an inline, un-hashed envelope as a rejection instead of a proposed
  state;
- serializing missing or unallowlisted paths instead of returning a pending
  prerequisite without linking the path;
- using local per-image reason synonyms instead of the exact public vocabulary;
- letting a safe-baseline pointer narrative override the requested
  product-side-effect reason, or changing the baseline state instead of
  preserving it; and
- reporting a hash/schema cause when the request is actually an upstream graph
  or product mutation decision.

Routing must remain request/evidence based and must not use a case-ID table.
When a repaired safe baseline is valid, its protected fields remain opaque and
the requested action changes only the explanation and the existing public
reason code. When it is invalid, the emitted blocked shape must be based on the
validator's actual error, not an inferred stale-version story.

### F4 — S4 semantic results are not yet separable from fixture failure

The six failed clear reviewer tasks were `S1-04`, `S3-01`, `S4-01`, `S4-02`,
`S4-03`, and `S4-04`. The S4 tasks are dominated by the frozen packet hash
defect and the resulting blocked-vs-clear artifact conflict. Their frame,
ROI, and `RotateScale` judgments must be re-run only after the packet and
baseline fixtures are repaired. No additional S4 semantic rule is admitted
from the corrupted run.

## What v0.1.5 did preserve

The candidate-only C1–C6 focused and forward checks remain the usable baseline.
Product actions stayed at zero, session consistency improved to 78/80, and
red fail-closed coverage improved from v0.1.4's 16/32 to 20/32. These gains
must be retained. The full benchmark regressed structure (84/112 -> 73/112),
unsupported findings (58 -> 69), and clear reviewer pass (12/16 -> 10/16),
so a broad rewrite or activation is not justified.

## Correction decision and minimum next checkpoint

The next work is split into two gates.

### Gate A — repair and re-freeze the benchmark corpus (mandatory first)

Create a new benchmark identity; preserve the v0.1.5 root as historical
evidence. Before author contexts run:

1. regenerate the Matching packet's nested registration/lock hashes from the
   exact frozen bytes and recompute every transitive public/freeze/case hash;
2. make the safe-static-baseline wrapper, nested handoff, XML, validation
   report, and upstream envelope use one canonical pointer set, then recompute
   the wrapper and all dependent hashes;
3. assert that every clear dependency is hash-valid before authoring, and
   explicitly check that a clear expected `MEASURE_ONLY` case is not forced
   into the blocked-artifact shape by the corpus itself; and
4. record a new freeze record, public manifest, artifact contract, expected
   attempts, and reviewer protocol. Do not alter the old score or attempt
   artifacts.

Gate A is a benchmark/corpus operation, not a candidate skill patch.

### Gate B — candidate-only v0.1.6 correction proposal

After Gate A is independently clean, the smallest candidate correction is:

- **C7 — executable bundle finalizer:** require the seven clear or five red
  artifacts to exist, bind static output to exactly one candidate XML, retain
  the first handoff-validator `PASS`, and generate the manifest last. A static
  pass with zero XML, missing manifest, wrong ordinal set, or a non-`PASS`
  first validator is never a completed handoff.
- **C8 — trust-boundary projection:** do not serialize missing/unallowlisted
  paths; use `PENDING/UNKNOWN` and the owner prerequisite instead. Make the
  inline/no-hash -> `PROPOSED` distinction explicit, and reject only an actual
  supplied hash/content mismatch. Copy the upstream Tool graph, frame, ROI,
  and parameter owners without semantic or helper-stage additions.
- **C9 — public routing and baseline precedence:** select the existing public
  reason vocabulary from the request/evidence pattern, keep per-image
  synonyms out, and validate a repaired baseline before deciding the action.
  A valid baseline keeps its status and protected pointers/bytes; an
  unauthorized action changes only the explanation/reason.
- **C10 — focused regression expansion:** add standard-library checks for
  missing/unallowlisted references, inline `PROPOSED`, static-pass-without-XML,
  manifest-last closure, repaired baseline alias/pointer preservation, exact
  product/per-image/upstream routing, and the clean S4 packet. Keep the
  existing C1–C6 tests and no-action forward probes.

No new public schema, Tool family, hidden case table, image interpretation,
product action, activation, Round 2, or release stage is included.

## Acceptance criteria for the next checkpoint

The next checkpoint may be called complete only when:

1. Gate A has a new identity with zero transitive hash mismatches and one
   canonical safe-baseline pointer set;
2. the v0.1.6 candidate finalizer cannot close a bundle with missing XML,
   missing manifest, invalid artifact order, unbound static output, or a
   non-`PASS` first validator;
3. missing/unallowlisted references, inline envelopes, exact public reasons,
   and safe-baseline action precedence have focused evidence;
4. the clean S4 packet passes independent frame/ROI/parameter and no-invention
   review; and
5. Skill Creator validation, focused tests, documentation/registry checks, and
   one independent positive/blocked forward probe pass before a separately
   admitted fresh 112-author/16-reviewer benchmark.

Activation, normal dispatch, product execution, qualification, release,
deployment, and Original-repository work remain separate approvals.

## Evidence identity

The authoritative v0.1.5 result and benchmark root are:

- Report: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_ROUND1_RESULT_20260902.md`.
- Evidence root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v015-20260902`.
- Final score: `score/round1-summary.json` SHA-256
  `0AF56A91767F739D8DC8F8676BCE63E53D64AC70BCB4AF1A83BF80E4FD9B4981`.
- Reviewer binder: `reviews/reviewer-evidence.json` SHA-256
  `97CA6BCF741AF933C959FEF00770187D5511E8E39ABE02296477A32773805C1B`.
- Audit summary: `audit/runs/20260901T224617364Z-7bb80b51/summary.json` SHA-256
  `0251DE5C36B0901BFA22E6EEE7FE28DFC7087DD993BA0E812283908D5B6440F3`.
- Matching packet: `frozen/public/dependencies/fixture-locator/matching-packet.json`
  SHA-256 `94F829395930AC7BB9C053892CB789817833758D1DB1B6273B220BAD6EE6188C`.
- Artifact contract: `frozen/public/artifact-contract.json` SHA-256
  `3002E6AF28326FEF7695F11DB9BE77ECBFF175A96EA4CBEC45789BDA100F7D9F`.

## Closure record

Status: **Complete**
Scope: classify the frozen v0.1.5 Round 1 failures, independently verify the
Matching and safe-baseline fixture defects, and define the two-gate corpus /
candidate correction boundary.
Acceptance criteria: execution integrity, final gates, representative failure
families, exact fixture hash mismatches, artifact-policy interaction, retained
v0.1.5 improvements, minimum correction scope, exclusions, and next acceptance
conditions are recorded.
Verification: final score JSON/Markdown, reviewer binder and failed clear-task
reviews, author audit, frozen public manifest/artifact contract, direct
Matching nested-hash recomputation, direct safe-baseline pointer comparison,
candidate skill/validator inspection, and v0.1.4 fixture comparison.
Evidence: the benchmark root and hashes listed above, plus
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_ROUND1_RESULT_20260902.md`.
Boundary / next dependency: Gate A requires a new benchmark identity; Gate B
requires candidate-only implementation and focused evidence. The candidate
remains explicit-only, inactive, outside normal dispatch, and unqualified.
