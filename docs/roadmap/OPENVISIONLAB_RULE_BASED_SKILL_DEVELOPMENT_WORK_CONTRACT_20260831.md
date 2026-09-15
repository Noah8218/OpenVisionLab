# OpenVisionLab Rule-Based Skill Development Work Contract

Date: 2026-08-31 KST
Repository: `C:\Git\2D\Dev`
Status: **Approved governance baseline — Recipe XML handoff `0.1.5` Round 1 benchmark FAIL; v0.1.6/v0.1.7/v0.1.8 admitted runs include incomplete historical evidence and a completed v0.1.8 backend-recovery FAIL; v0.1.10 focused correction and strict preflight PASS with admitted Round 1 FAIL; v0.1.11 focused correction and strict corpus preflight PASS with admitted Round 1 FAIL; v0.1.12 failure-derived focused correction PASS; candidate remains inactive and unqualified**

This document is the reusable work contract for creating or changing the
OpenVisionLab rule-based Codex skill suite. It defines admission, ownership,
verification, lifecycle, and rollback. It does not itself authorize a skill,
product, Recipe, release, or deployment mutation.

## 2026-09-08 Prompt Alignment Checkpoint

The user supplied the 20-section Rule-Based Vision prompt and requested
continuation. The bounded work updates the existing general teaching owner to
`1.0.1` and the explicit-only XML candidate to `0.1.13` solely to accept both
reviewed teaching versions `1.0.0` and `1.0.1` under the unchanged v1 schema.
The earlier benchmark records below remain historical; this checkpoint does
not admit a new corpus, Round 1 run, specialist or activation. Current checks
and the source-to-guidance mapping are recorded in
`docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_PROMPT_ALIGNMENT_20260908.md`.

## Goal And Current Decision

The goal is to grow the skill suite only when real repeated work proves an
independent responsibility, then preserve deterministic evidence and explicit
operator control through the shortest safe workflow.

Current installed baseline:

- `openvisionlab-rule-based-teaching` `1.0.2`, `active`: general composition
  owner;
- `openvisionlab-matching-teaching` `0.2.1`, `active`: admitted Matching-family
  specialist;
- `OPENVISIONLAB_RULE_BASED_SKILL_REGISTRY.json`: installed-suite governance
  snapshot and machine-readable ownership boundary.

Current admission result: **one bounded non-algorithm candidate**. Blob,
Contour, and Line work has not shown the repeated independent responsibility,
stable evidence packet, or existing-skill insufficiency required for an
algorithm-family specialist. After reviewing the exact three-round research
design, the user explicitly authorized documentation and development start on
2026-08-31. That decision admits
`openvisionlab-recipe-xml-handoff 0.1.1 candidate` only; activation remains a
separate decision. The operator subsequently authorized the compatible
`0.1.2`, `0.1.3`, focused `0.1.4`, focused `0.1.5`, `0.1.6`, `0.1.7`, and
  `0.1.8`, `0.1.9`, and `0.1.10` candidate correction lineage
without changing that lifecycle boundary. The v0.1.3 implementation/full
Round 1 result, v0.1.4 benchmark/triage, v0.1.5 focused correction, and fresh
v0.1.5 Round 1 benchmark evidence are recorded in their dated reports.

The candidate owns the stable boundary from an already reviewed
`openvisionlab-rule-based-teaching-envelope-v1` to one `VisionPipeline` XML
draft plus `openvisionlab-recipe-xml-handoff-v1`. It does not reinterpret raw
images, choose datum/ROI/Tool policy, duplicate Matching packets, execute or
import XML, mutate a Recipe, or qualify inspection results. The detailed
research and candidate contract is
`docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_RESEARCH_PLAN_20260831.md`.

## Non-Negotiable Requirements

- Repository contracts remain authoritative for product names, Tool behavior,
  frames, metrics, XML, and execution boundaries.
- Keep one general composition owner and add a specialist only for a proven
  independent responsibility.
- Skills may plan, review, and emit evidence packets. They must not implicitly
  run Preview/Run, mutate Recipe/XML/layers/routing, choose per-image values,
  auto-evolve, or pull remote updates.
- Missing, stale, ambiguous, wrong-frame, non-finite, or hash-mismatched
  evidence fails closed as `WAIT` or `REJECTED`.
- Selection does not imply qualification, release, or deployment readiness.
- Candidate creation and `candidate -> active` promotion require separate,
  explicit operator approvals.
- A candidate is explicit-invocation-only. Registry status and dispatch are
  governance declarations, not runtime enablement controls; use
  `agents/openai.yaml` with `policy.allow_implicit_invocation: false` and keep
  the candidate out of registry dispatch until activation.
- Existing user changes in Dev are preserved. `C:\Git\2D\Original`, commit,
  push, tag, release, and deployment remain separate authorization boundaries.

## Ownership Architecture

| Owner | Owns | Must not own |
| --- | --- | --- |
| Repository contracts | Product behavior, supported Tools/results, coordinate and execution contracts | Installed skill lifecycle |
| Skill registry | Governance routing declarations, owner, version, status, resources, relations, permissions, evaluation pointer | Runtime enablement, candidate isolation, or product runtime state |
| General skill | Intent, datum, minimal Tool chain, ROI/frame handoff, stage order, generic envelope | Matching registration or duplicated specialist policy |
| Admitted specialist | One stable family-specific workflow and its versioned evidence packet | General composition or unrelated Tool-family policy |
| Product/operator | Explicit Preview/Run, Recipe mutation, qualification and release decisions | Implicit skill-side execution |

Each admitted specialist publishes a versioned evidence packet. The general
skill composes it only by exact path and SHA-256, does not duplicate or
reinterpret specialist-owned fields, and cannot report downstream success from
an upstream `WAIT` or `REJECTED`.

The current suite-wide registry validator stays in the Matching skill bundle.
Move generic governance checks to a repository-owned tool only after a second
specialist is admitted or another suite-wide validation rule creates a real
independent ownership boundary.

## Candidate Admission Gate

A proposal becomes a skill candidate only when every item passes.

1. The same responsibility appears in at least two independent real tasks.
2. The repeated manual cost or observed error is meaningful and recorded.
3. Inputs, outputs, exit condition, and fail-closed states are stable.
4. Existing skills, repository scripts, tests, rules, and simple commands were
   checked and are insufficient.
5. The responsibility has one owner and does not duplicate the general skill
   or an admitted specialist.
6. ROI, frame, state, parameter, evidence, and write ownership are explicit.
7. A versioned packet or equivalent review artifact can preserve inputs,
   hashes, decisions, failures, and qualification boundary.
8. Deterministic structural or contract checks and a realistic independent
   behavioral-forward evaluation are possible.
9. The operator approves the exact candidate name, scope, inspected sources,
   writes, checks, stop condition, report format, and risks.

If any item fails, record `No candidates` or route the work to the existing
owner. File length, one example, one Tool name, or speculative future reuse is
not admission evidence.

## Development And Lifecycle Flow

| Phase | Trigger and output | Exit gate | Recommended model | Reasoning effort |
| --- | --- | --- | --- | --- |
| 0. Admission review | Repeated-work evidence is supplied; complete the gate above | All nine items pass, otherwise `No candidates` | `gpt-5.6-luna` | low |
| 1. Candidate work contract | Freeze name, owner, scope, I/O, permissions, checks, stop condition, and exclusions | Operator approves this exact candidate only | `gpt-5.6-luna` | low |
| 2. Minimal candidate | Modify the existing owner, or create `SKILL.md` plus explicit-only `agents/openai.yaml`; add only proven references/scripts | No duplicated responsibility, placeholder resource, implicit invocation, or candidate dispatch | `gpt-5.6-terra` | medium |
| 3. Deterministic validation | Run structural, registry, focused regression, documentation, and diff checks | Every applicable check passes | `gpt-5.6-luna` | medium |
| 4. Independent forward evaluation | Exercise a realistic in-scope request and nearest out-of-scope request when routing changed | Correct owner, fail-closed decisions, and zero implicit product side effects | `gpt-5.6-terra` | medium |
| 5. Activation | Present exact version, hashes, checks, evidence, dispatch, and invocation-policy change for a separate decision | Explicit operator approval, then registry/policy update and revalidation | `gpt-5.6-luna` | low |
| 6. Rollback or deprecation | A candidate fails or an active behavior regresses | Evidence preserved; prior safe behavior restored without rewriting a published version | `gpt-5.6-luna` | medium |

Lifecycle rules:

- `proposed`: outside the registry while admission evidence is reviewed;
- `candidate`: registered and installed only after exact candidate approval,
  omitted from registry dispatch, and marked
  `policy.allow_implicit_invocation: false`; bounded evaluation invokes the
  exact `$skill` or skill path directly;
- `active`: independently evaluated and explicitly activated by the operator;
  only this transition may add normal dispatch and restore implicit invocation;
- `deprecated`: removed from dispatch and marked explicit-only after
  consumer/compatibility review; evidence and version history remain traceable.

The registry validator does not enforce these invocation-policy or lifecycle
transition rules. Record and review them manually until a repeated failure
justifies a narrow validator extension.

Use SemVer for each skill. Use `PATCH` for a compatible correction, wording or
validator synchronization; `MINOR` for a compatible capability or field; and
`MAJOR` for an incompatible packet, dispatch, permission, or behavior change.
A new specialist starts at `0.1.0 candidate`. Never overwrite, decrement, or
reuse an externally recorded version.

## Verification Matrix

| Change | Required checks |
| --- | --- |
| Any changed skill | `python -B C:\Users\USER\.codex\skills\.system\skill-creator\scripts\quick_validate.py <skill-folder>` |
| Registry snapshot, resources, dispatch, relation, version, status, or permission | `validate_skill_registry.py <registry> <skills-root> <repository-root> --json` |
| Matching packet version or minimum manifest example | `test_skill_registry.py` |
| Matching registration manifest | `test_template_registration_manifest.py` and its validator |
| Candidate/active/deprecated transition | Manual evidence review of exact version/hash, status, dispatch membership, `allow_implicit_invocation`, approval, and evaluation pointer |
| New deterministic parser or validator | One small standard-library regression test that covers pass and fail-closed cases |
| New skill, dispatch/owner change, packet contract change, observed misrouting, or material model change | Independent behavioral-forward evaluation |
| Indexed documentation change | `tools/TestDocumentationIndex.ps1` |
| Every approved file set | scoped `git diff --check` and final diff review |

Test-only outputs, evaluator requests, outputs, verdicts, and hashes belong
under `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-<date>`.
Route test-process `TEMP` and `TMP` there when practical and use `python -B` to
avoid repository bytecode artifacts. A skill-only change does not require a
full product build or WPF run. If product source or runtime behavior must
change, stop and obtain a separate product work contract and authorization.

The independent evaluator receives the skill path, realistic request, and
minimum raw evidence, but not the intended answer, suspected defect, or
proposed fix. Preserve request, actual output, separate verdict, and SHA-256.
Evaluate decisions and side-effect boundaries, not matching prose.

## Candidate Registration Template

Copy and complete this block before requesting candidate approval:

```text
Candidate name:
User goal:
Repeated real tasks and evidence:
Meaningful manual cost or observed error:
Existing skills/scripts/tests/rules checked:
Independent responsibility owner:
Inputs and authority:
Outputs and packet/schema:
ROI/frame/state/parameter/evidence owners:
Files or sources it may inspect:
Files it may write:
Exact checks:
Invocation policy while candidate (must be explicit-only):
Dispatch change requested at activation:
Stop and fail-closed conditions:
Report format and evidence root:
Risks:
Explicit exclusions:
Candidate-creation approval: PENDING
Activation approval: NOT REQUESTED
```

## Conditional Maintenance Queue

These items are not active work. Open one only when its trigger exists.

| Conditional item | Trigger | Recommended model | Reasoning effort |
| --- | --- | --- | --- |
| General envelope version guard | Next approved general-skill version or observed registry/envelope drift | `gpt-5.6-luna` | low |
| Suite validator ownership extraction | Second specialist admission or another suite-wide rule | `gpt-5.6-terra` | medium |
| Matching `AUTO_SELECTION` / Variant B validation | Authorized frozen artifact exposes a validator gap or repeated manual cost | `gpt-5.6-luna` | medium |
| Blob, Contour, or Line specialist review | All admission evidence and exact candidate-review approval exist | `gpt-5.6-terra` | high |

Until a trigger exists, do not spend model or engineering effort on these
items and do not ask the operator to invent missing production requirements.

## Acceptance And Rollback

A candidate can be called complete only when its exact owner and boundary are
observable, the former owner no longer duplicates the moved responsibility,
all applicable checks pass, independent evaluation proves correct routing and
fail-closed behavior, hashes match the reviewed files, and the completion
record names what remains unqualified.

On failure:

- before activation, keep the candidate explicit-only and outside dispatch,
  then preserve the failed evidence and record whether the candidate remains;
- on installed-file hash mismatch, restore the last reviewed snapshot and
  rerun the focused validators;
- after activation, restore prior safe behavior through a new version rather
  than rewriting or reusing the recorded version;
- deprecate only after dispatch and packet-consumer compatibility are reviewed;
  never delete evidence merely because a candidate failed.

## Explicit Exclusions

This contract does not authorize new product algorithms, WPF UI, runtime
detectors, per-image parameter/template switching, Recipe/XML mutation,
Preview/Run, plugin/marketplace/API publication, automatic evolution, CI or a
new test framework, Original-repository work, commit, push, release, or
deployment.

## Publication Closure Record

Status: **Complete**
Scope: reusable governance plus admission of
`openvisionlab-recipe-xml-handoff 0.1.1 candidate` only
Acceptance criteria: exact owner and exclusions recorded; explicit-only policy;
normal dispatch exclusion; separate activation boundary
Verification: use the current candidate report for the executed structural,
registry, forward, XML compatibility, documentation, and diff checks
Evidence: this file, the current registry, the approved research plan, and
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_CANDIDATE_20260831.md`
Boundary / next dependency: activation requires a separate user decision;
Round 1 benchmark completion, XML Import/Preview/Run, Recipe qualification,
human-superiority claims, product work, release, and deployment remain outside
this closure

## Current Candidate Evaluation Extension — 2026-09-01

`openvisionlab-recipe-xml-handoff 0.1.4` was the installed candidate for this
2026-09-01 checkpoint. Its focused and bounded forward validation passed; the
separately admitted fresh v0.1.4 Round 1 benchmark completed with status `FAIL`
(112 authors, 16 reviewers, audit `PASS`). It remained explicit-only, outside
normal dispatch, inactive, and unqualified. Evidence:

- `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_3_FOCUSED_VALIDATION_20260901.md`
- `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_3_ROUND1_RESULT_20260901.md`
- `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_FOCUSED_VALIDATION_20260901.md`
- `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_ROUND1_RESULT_20260902.md`

This extension does not authorize activation, Round 2, product execution,
Original-repository work, commit, push, release, or deployment.

## Current Candidate Evaluation Extension — 2026-09-02

The candidate-only C1–C6 implementation checkpoint for
`openvisionlab-recipe-xml-handoff 0.1.5` is complete. The installed
SKILL/reference contract now freezes finalization before prose, records the
case/upstream/specialist/catalog/validator owner matrix, preserves opaque
baselines, routes public reasons without case-specific mappings, locks the
Matching/NormalizeImage frame relation, and rejects unknown parameters exposed
by a non-empty catalog `commonParameters` list. Fifteen focused regression
tests and an independent fresh positive/blocked forward probe passed. Registry
status remains `candidate`, explicit-only, outside normal dispatch, inactive,
and unqualified. The v0.1.4 full benchmark remains `FAIL`; a fresh v0.1.5
benchmark was subsequently admitted under a separate identity and is recorded
below.

Evidence: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_FOCUSED_VALIDATION_20260902.md`.

## v0.1.5 Round 1 Benchmark — 2026-09-02

The separately admitted fresh identity
`openvisionlab-rule-based-round1-v015-20260902` completed with 112/112 author
attempts, author audit `PASS`, 16/16 independent reviews, and final scorer
`FAIL`: structure `73/112`, unsupported/invented findings `69`, critical
red fail-closed `20/32`, clear reviewer pass `10/16`, product actions `0`, and
session consistency `78/80`. The candidate remains explicit-only, outside
normal dispatch, inactive, and unqualified. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_ROUND1_RESULT_20260902.md`.

The separately scoped evidence-triage/correction decision is complete in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_TRIAGE_20260902.md`. It
separates the frozen Matching nested-hash and safe-baseline pointer defects
(corpus-owned) from the remaining candidate-owned finalization, closed-world
projection, and public reason-routing failures. The v0.1.5 frozen root remains
immutable. Gate A has now repaired and re-frozen a new benchmark corpus
identity; Gate B implements only the candidate-side v0.1.6 C7–C10 correction
after that corpus gate. Neither gate authorizes activation, Round 2, product
execution, or release.

## v0.1.5 Failure Triage And Correction Decision — 2026-09-02

The v0.1.5 Round 1 failure triage is complete and is authoritative for the
correction boundary. The report verifies that the v0.1.5 Matching packet
references stale nested registration/operator-lock hashes, while the v0.1.4
packet references the same frozen bytes correctly. It also verifies that the
safe-static-baseline wrapper and nested handoff disagree on versioned pointer
paths even though the referenced bytes and hashes match. These are frozen
corpus integrity defects, not reasons to weaken the candidate's fail-closed
behavior or to reinterpret S4 results.

The report retains the v0.1.5 candidate-owned residuals as the minimum
candidate-only scope: C7 executable artifact-bundle finalization, C8
closed-world/evidence-owner projection, C9 exact public status/reason and
baseline precedence, and C10 focused regression. No new Tool family, handoff
schema, product action, dispatch rule, or activation is included.

Gate A is complete in a new benchmark identity with repaired Matching
transitive hashes, one canonical local safe-baseline pointer set, and fresh
freeze/public/expected records. The exact scope and evidence are recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_CORPUS_REFREEZE_20260903.md`.
Only after that corpus integrity gate passed may Gate B install a
candidate-only v0.1.6 correction and run focused checks. The frozen v0.1.5
benchmark, candidate installation, dispatch, activation, Round 2, product
execution, Original-repository work, commit, push, release, and deployment
remain unchanged and unauthorized.

## Gate A Corpus Re-freeze — 2026-09-03

The corpus-only checkpoint passed without changing the installed candidate or
the historical v0.1.5 root. The new identity is
`openvisionlab-rule-based-round1-v015-corpusfix-20260903` under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v015-corpusfix-20260903`.
Matching registration/lock/packet references, S4 and RT04 links, red-fixture
hashes, and the safe-static-baseline wrapper/nested handoff pointers are
internally consistent. Harness `prepare` created 112 immutable metadata
records; `self-test`, `validate_freeze_record`, `load_contract`, recursive
case-reference checks, the unchanged candidate baseline probe, and static XML
compatibility all passed. No author/reviewer benchmark, product EXE, Import,
  Preview/Run, or qualification action was performed. Gate B candidate-only
  C7–C10 work is complete in the focused checkpoint recorded below.

## Gate B Candidate-only v0.1.6 — 2026-09-03

The installed `openvisionlab-recipe-xml-handoff 0.1.6` candidate implements
only the approved C7–C10 correction after Gate A: clear/red artifact
finalization, one-XML/static-report and first-validator binding, blocked
`PENDING/UNKNOWN` reference projection, inline `PROPOSED` precedence, explicit
upstream owner/evidence shape, safe-baseline pointer-byte verification, and
public reason-alias rejection. The v1 handoff schema, explicit-only policy,
registry status `candidate`, dispatch exclusion, and no-product-action
boundary are unchanged.

The 20-case standard-library regression, UTF-8 Skill Creator validation,
Python compilation, and an independent D-drive positive/blocked forward probe
all pass. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_FOCUSED_VALIDATION_20260903.md`.
The fresh benchmark identity
`openvisionlab-rule-based-round1-v016-corpusfix-auditfix2-20260903` then
completed 112/112 Authors and external audit `PASS`, but mechanical pre-review
scoring was `INCOMPLETE`: structure `95/112`, critical red fail-closed `19/32`,
session consistency `70/80`, with one invalid validator scan-root event causing
the scorer to reject audit coverage. The Reviewer runner stopped at its
precondition (`0/16` contexts and no reviewer evidence). Detailed evidence is
recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_ROUND1_RESULT_20260903.md`.
The evidence-triage checkpoint classifies the scan-root, scorer, frozen-fixture,
candidate-validator, reason-routing, and upstream-authoring boundaries and
records the minimum correction scope for a new identity in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_TRIAGE_20260903.md`.
The candidate remains explicit-only, outside normal dispatch, inactive, and
unqualified; activation, product execution, qualification, Original-repository
work, commit, push, release, and deployment are not authorized by this result.

## Candidate-only v0.1.7 Focused Correction — 2026-09-03

The installed `openvisionlab-recipe-xml-handoff 0.1.7` candidate retains the
v0.1.6 C7–C10 finalization, trust-boundary, baseline, and public-vocabulary
guards. It adds the candidate-owned generic ROI authority-lock correction:
`operatorLocks[].kind == "ROI"` matches the explicit `USE_ROI=true` and
`CvROI` pair on any enabled consumer instead of assuming `LineDistance`. The
SKILL/reference contract also makes inline `PROPOSED`, passed file-backed
measurement-only `MEASURE_ONLY`, immutable-baseline, repository-contract,
locator-frame, and upstream-graph reason precedence explicit. Historical
`AUTO_REJECTED_*` aliases and the frozen artifact-contract/corpus conflicts
remain unchanged and unresolved.

The 21-case standard-library regression, UTF-8 Skill Creator validation,
Python compilation, and independent D-drive positive/blocked forward probe
all pass. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_FOCUSED_VALIDATION_20260903.md`.
The candidate remains explicit-only, outside normal dispatch, inactive, and
unqualified. This focused checkpoint does not admit a new benchmark identity
or authorize activation, Round 2, product execution, qualification,
Original-repository work, commit, push, release, or deployment.

## Round 1 Runner/Scorer Correction — 2026-09-03

The proven v0.1.6 audit event `72` used the benchmark root as the static
validator scan root. The isolated protocol correction pins the author prompt,
process `cwd`, and all retries to the assigned attempt root; preserves the
scorer's fail-closed rejection of any unassigned root; permits only
case-declared red-team path/provided-hash mismatches; and scopes the static
validator cache by scan root. Fifteen harness tests, one runner test, Python
compilation, a retained RT03 before/after probe, and a retained audit-event
diagnosis pass in the D-drive fixture. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_ROUND1_RUNNER_SCORER_CORRECTION_20260903.md`.

This was a benchmark-protocol correction, not a skill-version change. At that
checkpoint it did not rewrite the frozen v0.1.6 root, alter the hidden key or
reason vocabulary, activate a candidate, or authorize product,
Original-repository, commit, push, release, or deployment work. The subsequent
read-only reconciliation preflight did not reproduce S3-02 or S4-01 as
mechanical blockers; it confirmed the strict legacy-baseline incompatibility
and the public reason-vocabulary decision point. The owner-approved projection
and strict-baseline identity were then admitted and are reported below.

## Generic Reason Projection And Strict-Baseline Preflight — 2026-09-03

The contract-owner direction for the new corpus keeps the v1 public
`reasonCode` vocabulary generic and excludes historical `AUTO_REJECTED_*`
aliases. The projection is pattern-based: per-image substitution maps to
`PER_IMAGE_OVERRIDE_FORBIDDEN`, missing retained competitors or automatic
candidate/graph composition maps to `UPSTREAM_GRAPH_REVIEW_REQUIRED`, and
direct layer/routing mutation maps to `LAYER_MUTATION_NOT_AUTHORIZED`. The
decision record is
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_REASON_PROJECTION_DECISION_20260903.md`.
It does not add a handoff field or silently broaden the installed candidate.

Using that input contract and the corrected runner/scorer fixture, a new
D-drive-only identity is frozen for preflight:

```text
openvisionlab-rule-based-round1-v017-strictbaseline-20260903
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v017-strictbaseline-20260903
```

The strict local baseline contains 17 exact evidence pairs and uses
`PENDING/UNKNOWN` for unexecuted packet identity. Freeze, contract, recursive
case-reference, direct and wrapper handoff-validator, static XML compatibility,
15 harness-test, one runner-test, compile, self-test, and `prepare` checks all
pass; `prepare` creates metadata for 80 clear + 32 red-team = 112 planned
attempts only. The complete evidence is
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_STRICT_BASELINE_PREFLIGHT_20260903.md`.

The preflight closed the input-contract and corpus gates. The separately
admitted static execution then completed Authors `112/112` with audit
`PASS`, but the mechanical scorer returned `INCOMPLETE` (structure `103/112`,
clear expected-answer `76/80`, critical red fail-closed `25/32`, session
consistency `76/80`); the Reviewer runner guard-blocked at `0/16`. Candidate
activation, normal dispatch, Round 2, product execution, qualification,
Original-repository work, commit, push, release, and deployment remain
outside this checkpoint. The result and sequencing defect are recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_ROUND1_RESULT_20260903.md`;
the frozen input root is not rewritten. | Recommended model:
`gpt-5.6-terra` | Reasoning effort: `high` for a new-identity correction/admission review.

## v0.1.7 Strict-Baseline Round 1 Result — 2026-09-03

The admitted identity `openvisionlab-rule-based-round1-v017-strictbaseline-20260903`
completed all 112 Author records (80 clear + 32 red-team) with no timeout or
model-error records. The external audit was `PASS` with 75 allowed
static-validator starts, 0 violations, 0 forbidden-action markers, 0 monitor
errors, and 0 repository mutations. Mechanical scoring was `INCOMPLETE`
(structure `103/112`, clear `76/80`, red fail-closed `25/32`, session
consistency `76/80`); every attempt metadata record carried the old
freeze-record hash `F1785DA94DF9E2B9D42060B45C739571BDC47EECC33ED7A3D1980E53A18FE65D`
instead of the current frozen-record hash. The Reviewer guard stopped before
spawning any of 16 contexts. The complete result is recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_ROUND1_RESULT_20260903.md`.

The candidate remains explicit-only, outside normal dispatch, inactive, and
unqualified. A future benchmark needs a new owner-approved identity with
freeze-record finalization before `prepare`, red status/reason reconciliation,
and a clear-case regression check; the immutable root must not be rewritten.

## Candidate-only v0.1.8 Correction And Strict-Baseline Preflight — 2026-09-03

The installed `openvisionlab-recipe-xml-handoff 0.1.8` candidate closes the
candidate-owned projection boundary identified by the v0.1.7 result. It
validates and preserves the immutable baseline tuple first, keeps passed
file-backed XML without acceptance as `MEASURE_ONLY` when upstream is
`PROPOSED`, and applies the generic fail-closed order for per-image, graph,
layer/routing, datum, algorithm, and explicit repository-contract failures.
The 22-case focused suite, Skill Creator validation, and Python compilation
pass. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_FOCUSED_VALIDATION_20260903.md`.

A new D-drive-only identity,
`openvisionlab-rule-based-round1-v018-preflight-20260903-r1`, completes the
freeze-before-`prepare` gate. Freeze/contract/case checks, direct v0.1.7
baseline validation, v0.1.8 baseline projection, static XML compatibility,
harness/runner regressions, and `prepare` all pass; all 112 metadata records
match the final freeze hash and no author/reviewer artifact exists. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_STRICT_BASELINE_PREFLIGHT_20260903.md`.

This is a candidate-only and preflight checkpoint. The v0.1.7 failed run stays
immutable, and the 112 + 16 Authors/Reviewers benchmark, activation, normal
dispatch, product execution, qualification, Original-repository work,
commit, push, release, and deployment remain separately authorized. |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high` for the separate
benchmark admission review.

The subsequent v0.1.8 admission used the same frozen identity. The Authors
wrapper recorded `112/112` outcomes and external audit `PASS`, but 72 contexts
ended with Codex backend HTTP 503/404 connection errors, leaving 40 valid
author evidence sets. The scorer is `INCOMPLETE` (structure `40/112`, clear
`40/80`, red `0/32`) and the Reviewer phase was not launched. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_ROUND1_RESULT_20260903.md`.
The identity is immutable incomplete evidence; a retry needs backend
availability and a new freeze/preflight identity. | Recommended model:
`gpt-5.6-terra` | Reasoning effort: `high`.

The first 2026-09-04 ephemeral Codex CLI health call failed with HTTP `404`,
but a later probe using the current `codex-cli 0.153.0-alpha.5` binary returned
the exact `HEALTHCHECK_OK` response with exit `0`. The health report preserves
both probes and the D-drive evidence:
`docs/reports/OPENVISIONLAB_CODEX_BACKEND_HEALTH_20260904.md`.

## Candidate-only v0.1.9 Correction And Backend-Recovery Result — 2026-09-04

The installed `openvisionlab-recipe-xml-handoff 0.1.9` candidate adds only
failure-derived serialization and ownership guards: evidence-language
integrity, `Main -> SourceFrame` and non-transforming frame preservation,
explicit graph exclusions, case/image/specialist owner provenance, Contour
`DetectMode=External` projection, per-input observation discipline, final-state
projection, and public reason-contract conflict handling. The 24-case focused
suite, Skill Creator validation, and Python compilation pass. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_9_FOCUSED_VALIDATION_20260904.md`.

With the successful current-binary health probe, the separately identified
`openvisionlab-rule-based-round1-v018-backend-recovery-20260904-r2` run
completed all `112/112` Authors, external audit `PASS`, and `16/16` Reviews.
The final scorer is a complete `FAIL`: structure `108/112`, unsupported or
invented findings `34`, critical red fail-closed `28/32`, clear reviewer tasks
`11/16`, product actions `0`, and session consistency `79/80`. This result is
recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_BACKEND_RECOVERY_ROUND1_RESULT_20260904.md`;
the frozen D-drive root is immutable.

The public reason-contract/corpus decision for the four red expectation
conflicts is recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_REASON_CONTRACT_CONFLICT_REVIEW_20260904.md`
and is materialized only in the new v0.1.10 freeze/preflight identity recorded
below. At this checkpoint the next skill priority was the separately admitted
v0.1.10 Authors and Reviewers run; its completed result is recorded in the
latest execution section below.
Candidate activation, implicit dispatch, XML Import, Preview/Run, Recipe
qualification, runtime parity, human comparison, Original-repository work,
commit, push, release, and deployment remain separately unauthorized. |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## v0.1.3 Failure Triage And Implemented v0.1.4 — 2026-09-01

The frozen v0.1.3 result is a complete negative evaluation. Its failure
classification is recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_3_TRIAGE_20260901.md`.
The patch-level correction contract and focused implementation are
`docs/roadmap/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_CORRECTION_CONTRACT_20260901.md`.

The triage found recurring canonical decision/state drift, unsupported
closed-world values, validator/artifact delivery claims after failure or
missing XML, empty-evidence `PASS` gates, non-opaque safe-baseline claims, and
public reason-code drift. The proposed correction adds only a transient
decision ledger, evidence-owned preflight, validator/artifact hard stop,
evidence-gated `PASS`, opaque baseline preservation, and exact public reason
selection. It does not add an algorithm-family specialist or change the v1
handoff schema.

`0.1.4` is installed and registered as a candidate, but remains outside normal
dispatch and inactive. Focused and bounded forward checks pass, while the
admitted fresh full benchmark is a complete negative evaluation:
structure 84/112, unsupported/invented findings 58, critical red fail-closed
16/32, clear reviewer pass 12/16, product actions 0, and session consistency
74/80. The next step is a separately scoped failure-triage/correction decision;
  this extension does not authorize activation, Round 2, product execution,
  Original-repository work, commit, push, release, or deployment.

## v0.1.4 Failure Triage — 2026-09-02

The separate failure-triage checkpoint is complete in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_TRIAGE_20260902.md`.
It records the observed structure/projection, evidence-ownership,
status/reason-routing, and S4 frame/parameter failures and defines the
candidate-only C1–C6 minimum correction scope. No installed skill resource was
changed by the triage. The next checkpoint is a proposed v0.1.5 implementation
followed by focused and bounded-forward checks; a fresh benchmark identity is
not admitted until those checks pass and is separately authorized.

## Candidate-only v0.1.10 Correction And Strict-Baseline Preflight — 2026-09-04

The installed `openvisionlab-recipe-xml-handoff 0.1.10` candidate makes the
public reason precedence explicit after the v0.1.8 backend-recovery conflicts:
datum/geometry versus calibration, unprovided capability versus unsupported
semantic invention, per-image override versus acceptance mutation, and an
observed upstream stage `FAIL` versus graph-review. The correction retains the
v1 handoff schema, explicit-only invocation, inactive/unqualified lifecycle,
and the existing owner boundary. Its 25-case focused regression, Skill Creator
validation, and Python compilation pass. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_FOCUSED_VALIDATION_20260904.md`.

The approved contract/corpus decision was materialized only in the new identity
`openvisionlab-rule-based-round1-v010-contract-recovery-20260904`. Its final
freeze precedes `prepare`; contract/self-test, direct v0.1.7 baseline,
v0.1.10 projection, static compatibility, harness/runner checks, and metadata
freeze all pass. The 112 metadata records are 80 clear + 32 red-team, each
bound to the final freeze hash, and the attempt roots contain no author or
review artifacts. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_STRICT_BASELINE_PREFLIGHT_20260904.md`.

This remains a candidate-only and preflight checkpoint. The prior v0.1.8
backend-recovery root remains immutable. Authors/Reviewers execution,
qualification, activation, normal dispatch, product execution, runtime parity,
human comparison, Original-repository work, commit, push, release, and
deployment remain separately authorized. | Recommended model:
`gpt-5.6-terra` | Reasoning effort: `high` for the separate benchmark admission
review.

## v0.1.10 Round 1 Execution Result — 2026-09-04

The separately admitted v0.1.10 identity
`openvisionlab-rule-based-round1-v010-contract-recovery-20260904` completed
Authors `112/112` (wrapper exit `0`), external audit `PASS`, and independent
Reviews `16/16` (wrapper exit `0`). The final scorer completed with exit `1` and
status `FAIL`: structure `100/112`, critical red fail-closed `20/32`, clear
Reviewer tasks `14/16`, unsupported/invented findings `20`, product actions `0`,
and session consistency `79/80`. Evidence and hashes are recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_ROUND1_RESULT_20260904.md`.

The run is a complete negative quality evaluation, not a backend-blocked
attempt. The failed D-drive root, Authors, audit, Reviewer, and scorer evidence
remain immutable. The candidate stays explicit-only, outside normal dispatch,
inactive, and unqualified. The next step is candidate-only failure triage and a
separately approved new freeze identity; activation, Round 2, product
execution, runtime parity, human comparison, Original-repository work, commit,
push, release, and deployment remain unauthorized. | Recommended model:
`gpt-5.6-terra` | Reasoning effort: `high`.

## v0.1.10 Round 1 Failure Triage — 2026-09-04

The v0.1.10 Round 1 failure-triage checkpoint is complete in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_TRIAGE_20260904.md`.
All 12 red misses, all 20 unsupported/invented findings, and the two failed
clear Reviewer tasks are classified. The evidence separates candidate-owned
status/projection and semantic-family residuals from public-contract gaps and
the legacy safe-static-baseline/validator frame incompatibility. No installed
candidate resource or frozen benchmark root was changed.

The minimum next correction is not a hidden-answer fit: first approve and
publish the missing public reason/NormalizeImage-value rules, then create a new
strict corpus identity with a SourceFrame-compatible baseline and explicit
`FIXTURE_MIN_VALID_PIXEL_RATIO` ownership. Only after that may a candidate-only
patch, focused validation, and a separately admitted `112 + 16` run proceed.
Candidate activation, normal dispatch, product execution, qualification,
Round 2, Original-repository work, commit, push, release, and deployment remain
separate approvals. | Recommended model: `gpt-5.6-terra` | Reasoning effort:
`high`.

## v0.1.10 Public Correction Contract and Corpus Repair Draft — 2026-09-04

The next planning checkpoint is prepared, but not approved or installed. The
public pattern/precedence contract is in
`docs/roadmap/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_CORRECTION_CONTRACT_20260904.md`;
the replacement-corpus and strict-freeze specification is in
`docs/roadmap/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_CORPUS_REPAIR_SPEC_20260904.md`.
Both are derived from the v0.1.10 triage and intentionally leave the
NormalizeImage ratio decision, public publication, candidate patch, new
benchmark ID, and admission as explicit operator decisions.

The contract preserves the no-hidden-mapping rule, explicit-only lifecycle,
pixel-only measurement boundary, strict `Main -> SourceFrame` baseline, and
zero product-side effects. No installed candidate, frozen root, Original
repository, or product runtime was changed. The next implementation checkpoint
is allowed only after those drafts are approved. | Recommended model:
`gpt-5.6-terra` | Reasoning effort: `high`.

## v0.1.11 Candidate-only Correction — 2026-09-04

The operator authorized continuation to the installed candidate-only patch
under the conservative policy that an unowned
`FIXTURE_MIN_VALID_PIXEL_RATIO` returns `WAIT` rather than adopting the guide's
`0.25` example. Candidate `openvisionlab-recipe-xml-handoff` is now version
`0.1.11`; its skill, handoff reference, validator, and focused regression
resources are synchronized. The patch adds integrity-first routing, typed ROI /
affine / upstream-WAIT precedence, pixel-only calibration boundaries,
image-versus-policy ownership, intent/projection isolation, and strict baseline
compatibility. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_FOCUSED_VALIDATION_20260904.md`.

The focused suite ran `27` tests with `OK`; Python compilation and UTF-8 Skill
Creator `quick_validate` also passed. The registry keeps the candidate
explicit-only, inactive, and unqualified. The v0.1.10 failed root remains
immutable; the public correction and corpus-repair documents remain Draft, and
no new corpus identity, benchmark admission, activation, product execution,
Original-repository change, commit, push, release, or deployment was made. The
next gate from this checkpoint was to build a new strict SourceFrame-compatible
corpus identity; its completed preflight is recorded in the section below. |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## v0.1.11 Strict Corpus Identity And Preflight — 2026-09-04

The candidate-only continuation created the new immutable D-drive identity
`openvisionlab-rule-based-round1-v011-strict-corpus-20260904`. The retained
baseline was repaired to strict `Main -> SourceFrame` compatibility; S1/S3
intent and projection constraints were made lossless; and S4-01..04 each carry
one explicit operator-owned `FIXTURE_MIN_VALID_PIXEL_RATIO=0.25` lock. No
validator weakening, hidden-answer mapping, product execution, or failed-root
repair was used.

The new root passed contract load, self-test, direct and wrapper-projection
handoff validation, static XML compatibility, harness/runner regression,
five-file Python compilation, `prepare`, and the exact 112/112 metadata
freeze-hash gate with no attempt artifacts. The full evidence is
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_STRICT_CORPUS_PREFLIGHT_20260904.md`.

This is a preflight checkpoint only: Authors/Reviewers execution, benchmark
admission, activation, runtime parity, qualification, and Original-repository
work remain separate approvals. | Recommended model: `gpt-5.6-terra` |
Reasoning effort: `high`.

## v0.1.11 Round 1 Execution Result — 2026-09-04

The separately admitted immutable identity
`openvisionlab-rule-based-round1-v011-strict-corpus-20260904` completed Authors
`112/112` (wrapper exit `0`), external audit `PASS`, and independent Reviews
`16/16` (wrapper exit `0`). Final scoring completed with exit `1` and status
`FAIL`: structure `108/112`, unsupported/invented findings `16`, critical red
fail-closed `29/32`, clear Reviewer tasks `14/16`, product actions `0`, and
session consistency `77/80`. Evidence and hashes are recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_ROUND1_RESULT_20260904.md`.

The run is a complete negative quality evaluation, not a backend-blocked
attempt. The frozen D-drive root, Author/Reviewer artifacts, and prior failed
v0.1.10 root remain immutable. The candidate stays explicit-only, outside
normal dispatch, inactive, and unqualified. The next checkpoint is a separate
evidence-triage and candidate-only correction decision for the timeout, public
reason-code residuals, shared-mask/graph projection, and NormalizeImage
ownership failures. Activation, Round 2, product execution, runtime parity,
human comparison, Original-repository work, commit, push, release, and
deployment remain unauthorized. | Recommended model: `gpt-5.6-terra` |
Reasoning effort: `high`.

## v0.1.11 Failure Triage — 2026-09-04

The v0.1.11 Round 1 triage is complete in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_TRIAGE_20260904.md`.
It classifies the single S2 timeout as backend execution variance and assigns
the remaining provenance, shared-mask, prior-mask, intent-isolation,
owner/evidence, and public reason-precedence residuals to the candidate-only
correction boundary. The frozen corpus, scorer, Authors, Reviewers, product,
and Original checkout remain unchanged.

The triage does not admit a rerun. A separate strict-corpus repair/freeze and
Round 1 admission decision remains required after the candidate patch. |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## v0.1.12 Candidate-only Failure-derived Correction — 2026-09-04

The installed `openvisionlab-recipe-xml-handoff` candidate is now `0.1.12`.
The patch preserves explicit-only invocation, the v1 handoff shape,
`qualification: false`, and no product actions while adding explicit
`OPERATOR_SELECTED` provenance, `OPERATOR_LOCK` ownership for NormalizeImage
ratio, one-Threshold shared-mask fan-out, prior-mask `USE_THRESHOLD=false`,
intent-specific parameter isolation, platform/capability/per-image reason
precedence, and neutral immutable-baseline explanations. The public contract is
recorded in
`docs/roadmap/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_CORRECTION_CONTRACT_20260904.md`.

The focused suite passed `28` tests; UTF-8 Skill Creator validation and Python
compilation passed. Candidate hashes and checks are recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_12_FOCUSED_VALIDATION_20260904.md`.
No new corpus, benchmark admission, activation, runtime execution,
qualification, Original-repository change, commit, push, release, or
deployment was performed. The remaining gate is a separately approved strict
corpus/freeze and Round 1 admission decision. | Recommended model:
`gpt-5.6-terra` | Reasoning effort: `high`.

## v0.1.13 Strict Corpus Preflight — 2026-09-08

Status: Complete for the new exposed-regression corpus, static preflight and
112 metadata records. Candidate version/dispatch/permissions remain unchanged.
The new identity `openvisionlab-rule-based-round1-v0_1_13-strict-corpus-20260908`
contains recovered public prompts, clarified shared-mask/capability/per-image
requests, consistent red expectations and frozen skill/static-runtime copies.
Source images/numeric locks, all 16 clear expectations, negative supplied hashes,
baseline protected semantics and the old 0.1.11 root remain preserved.

Acceptance evidence: 45 focused tests; three skill-format checks; direct and
baseline-projection handoff checks; 13 XML roots / 1 recipe static compatibility;
five Python syntax checks; harness/contract/prepare; 12 audit-classifier fixtures;
20 final gates including 112/112 metadata, 273 path/hash pairs and 1,800 unchanged
old-root files. No Author/Reviewer artifacts exist in the new attempt root.
Commands, exact freeze and repaired preflight failures are recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_13_STRICT_CORPUS_PREFLIGHT_20260908.md`.

Boundary / next dependency: separate full-run authorization and backend/immutable
input check. The 900-second timeout and missing/error denominator are retained;
changing execution settings requires an explicit pre-execution freeze decision.
No rerun, scoring, activation, runtime qualification, product/Original mutation,
commit, push, release or deployment is implied. Execution management after these
prerequisites: Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## v0.1.13 Admitted Round 1 Interruption — 2026-09-08

Status: Blocked on a shared-workspace quiet interval for a valid new evaluation.
The admitted 0.1.13 run stopped after 4/112 completed Authors when another task
launched the Layer/Recipe UI smoke and changed Dev. All four outputs and 108
missing records remain in the fixed denominator. The audit is `VIOLATION`,
final scorer `INCOMPLETE` (exit 2), and Reviewers were not started (0/16).
Original was unchanged; frozen inputs, execution scripts and 112 metadata hashes
remain intact. Current backend health probes passed before launch.

The candidate remains explicit-only, inactive and unqualified. This is no
quality verdict; the prior 0.1.11 full quality FAIL remains historical evidence.
See `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_13_ROUND1_RESULT_20260908.md`.
The stopped identity is immutable. A new identity and freeze/preflight are
required once no concurrent repository writes or product/runner launches will
occur throughout Author audit. A transient idle snapshot did not ensure that
condition. Do not spend further Author/Reviewer tokens until it is available.
This skill request does not authorize unrelated product work or schedule changes.

## 2026-09-08 Rule-Based XML Retry 2

Status: Blocked on an operator-coordinated quiet interval for the Author audit.
The user requested a retry. A new identity preserved all old evidence, passed
45 focused tests, 12 audit fixtures, two backend probes, 11 independent input
equivalence checks and 11 admission checks, then started 112 fresh Authors.
After the prior product task closed, a newly requested Metrics/Acceptance task
started and launched its UI smoke during audit. The retry stopped with four
completed Authors, 108 missing retained, audit `VIOLATION` and final score
`INCOMPLETE`; Reviewers were not admitted. Original and frozen inputs are intact.
The candidate remains explicit-only, inactive and unqualified.

Both interrupted identities remain immutable and their results are not pooled.
A transient idle check has failed twice. The user has been asked to choose a
skill-only evaluation interval after current product work, or defer evaluation.
Do not start another identity or spend Author/Reviewer tokens until that
coordination decision is available. This does not stop separately authorized
product work or change its schedule. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_13_RETRY2_RESULT_20260908.md`.

## v0.1.14 Static Development Closure — 2026-09-08

Status: Complete for the selected 0.1.14 upstream-state and JSON-input correction.
The user deferred full benchmarking and requested other skill priorities. That
decision supersedes the pending quiet-interval question in Retry 2 below.
Do not start another benchmark identity or schedule Authors/Reviewers from it.

The XML handoff candidate is now 0.1.14: retained upstream WAIT/REJECTED or
stage/gate WAIT/FAIL blocks nonblocked XML handoffs; duplicate JSON keys,
NaN/Infinity, numeric overflow and malformed UTF-8 produce structured failure.
Baseline preservation still validates JSON syntax without reinterpreting its
protected semantics. Thirty-five focused tests, 13 independent probes, Skill
Creator, registry and documentation-index checks passed. The candidate remains
explicit-only, inactive and unqualified; teaching 1.0.1 and Matching 0.2.1 remain.

Selected remaining skill priorities:
1. Stage-to-Tool layer/frame mapping without cardinality bypass | Recommended model: gpt-5.6-terra | Reasoning effort: high.
2. OPERATOR_SELECTED evidence ownership | Recommended model: gpt-5.6-terra | Reasoning effort: medium.

Worklist: `docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_STATIC_DEVELOPMENT_20260908.md`.
Evidence: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_14_FOCUSED_VALIDATION_20260908.md`.
The worklist preserves both queued items. Existing PL-0010 ledger schema errors
prevented automated issue registration; no new issue ID was issued. Product
work, Original and old frozen benchmarks are outside this batch.

## v0.1.15 Stage Route Development — 2026-09-08

Status: Complete — 43 focused regressions, 24 independent contract cases,
Skill Creator, registry and documentation-index checks passed.
The user's continuation request selects worklist priority 3: primary layer
path mapping and frame consistency without a 1:1 stage/Tool assumption.
Acceptance, preserved cases and exclusions are recorded in
`docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_STATIC_DEVELOPMENT_20260908.md`.
Evidence: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_15_FOCUSED_VALIDATION_20260908.md`.
Priority 4 remains queued; full benchmarking remains deferred.

## v0.1.16 Operator Selection Development — 2026-09-08

Status: Complete — 51 focused regressions, 40 independent cases, Skill Creator,
registry and documentation-index checks passed. All five acceptance criteria
are satisfied; all four selected static priorities are closed. Scope: selected worklist
priority 4, exact caller-owned observation selection and retained evidence.
Acceptance and the explicit authority trust boundary are recorded in
`docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_STATIC_DEVELOPMENT_20260908.md`.
Evidence: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_16_FOCUSED_VALIDATION_20260908.md`.
Full benchmarking remains deferred; no activation or product action is implied.

## Producer Transfer Compatibility — 2026-09-08

Status: Complete — producer transfer acceptance and verification passed.
The new continuation selects general 1.0.2 conditional XML transfer guidance
and candidate 0.1.17 reviewed-version acceptance. The 0.1.16 selection-profile
validator remains closed; its exact semantics are preserved. The v1 envelope
shape, caller-owned authority, ordinary design workflow and existing invocation
policies remain. Two independent producer requests, 51 regression tests and
eight compatibility cases passed. Full benchmarking remains deferred.
Acceptance/evidence: `docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_PRODUCER_HANDOFF_20260908.md`.

## Independent Static XML Integration — 2026-09-08

Status: Complete
Scope: follow-up 6, two independent general-to-XML cases with unchanged
teaching 1.0.2 and candidate 0.1.17. Shared-mask Blob routing/ROI/area/flags
and pixel LineDistance parameters survive real file authoring and static checks.
Acceptance: retained identities, fresh producers, fresh XML authors with actual
compatibility/handoff PASS, independent semantic comparison and durable tracking.
Verification/evidence: `docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_STATIC_INTEGRATION_20260908.md` and its D-drive root.
Boundary / next dependency: synthetic static evidence only; existing runtime
DLLs were identified, not rebuilt. Full benchmark remains deferred; no product
image execution, new typed flag-lock profile, activation or qualification.

## Recipe XML Handoff 0.1.18 Standalone Finalization — 2026-09-08

Status: Complete
Scope: clarify standalone file-backed handoff completion versus the supplied
clear/red artifact-bundle contract. The XML/handoff validator, schema, bundle
checks and explicit-only policy remain unchanged.
Acceptance: standalone mode is documented without invented benchmark identity,
artifact manifest or second audit; supplied-contract mode retains the exact
bundle rules; focused tests and governance checks pass.
Verification/evidence: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_18_STANDALONE_FINALIZATION_20260908.md`.
Boundary: no product execution, benchmark admission, artifact contract creation,
activation, qualification, commit or push.

## Recipe XML Handoff 0.1.19 Orphan Bundle Identity — 2026-09-08

Status: Complete
Scope: fail closed when standalone validation receives `attempt_id` or
`case_id` without the complete artifact-bundle option group. Candidate 0.1.19
preserves the standalone base path and complete clear/red bundle behavior while
rejecting silently ignored identity options with `E_ARTIFACT_OPTIONS`.
Acceptance: orphan identity options fail closed; standalone and full bundle
focused regressions remain valid; candidate, registry and documentation records
are synchronized.
Verification/evidence: 53 focused tests, Skill Creator, registry and
documentation-index checks; `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_19_ORPHAN_BUNDLE_IDENTITY_20260908.md`.
Boundary: no benchmark admission, product execution, activation, qualification,
commit or push.

## Recipe XML Handoff 0.1.20 Bundle Identity — 2026-09-08

Status: Complete
Scope: require both `attempt_id` and `case_id` for contract-bound bundle
audits. Candidate 0.1.20 preserves the standalone path and complete clear/red
bundle behavior while failing closed before artifact checks when either identity
value is omitted.
Acceptance: the identity pair is required and compared with the retained
manifest; standalone and full bundle focused regressions remain valid; candidate,
registry and documentation records are synchronized.
Verification/evidence: 53 focused tests, Skill Creator, registry and
documentation-index checks; `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_20_BUNDLE_IDENTITY_REQUIRED_20260908.md`.
Boundary: no benchmark admission, product execution, activation, qualification,
commit or push.

## Recipe XML Handoff 0.1.20 Admission Readiness Preflight — 2026-09-08

Status: Complete for a read-only admission-readiness review; benchmark
admission is `BLOCKED`.
Scope: cross-check candidate 0.1.20 static/governance evidence and the
prerequisites for a future new Round 1 identity without starting Authors,
Reviewers or product execution.
Acceptance: candidate version/source, focused regression, Skill Creator,
registry, documentation index and retained corpus/preservation evidence pass;
the missing candidate freeze, quiet interval, fresh backend probe and separate
admission decision are recorded as blockers.
Verification/evidence: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_20_ADMISSION_READINESS_20260908.md` and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-admission-readiness-20260908`.
Boundary: no benchmark identity, Author/Reviewer token spend, product process,
activation, qualification, Original mutation, commit or push.

## Recipe XML Handoff 0.1.20 New Identity and Freeze Preflight — 2026-09-08

Status: Complete for candidate identity, freeze and static preflight; benchmark
admission remains `NOT_GRANTED`.
Scope: create a new D-drive `0.1.20` benchmark identity from the preserved
`0.1.13` source, bind current skill/validator snapshots, freeze before metadata
preparation, and run static, harness and integrity gates without Author,
Reviewer or product execution.
Acceptance: new target identity is isolated; source root is immutable; the
final freeze precedes exactly 112 metadata records; no outcomes are copied; all
20 final-gate checks and recorded preflight commands pass — **PASS**.
Verification/evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_20_NEW_IDENTITY_PREFLIGHT_20260908.md`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-round1-v020-preflight-20260908\identity-preflight.json`,
and the target `preflight/final-gate.json`.
Boundary / next dependency: no Authors, Reviewers, product EXE, Import,
Preview/Run, fresh backend probe, activation, qualification, Original mutation,
commit, push, release or deployment occurred. An operator-coordinated quiet
interval, fresh backend probes and a separate admission decision remain
required; runtime parity is `NOT_MEASURED`.

## Recipe XML Handoff 0.1.20 Fresh Backend Prerequisite Probe — 2026-09-08

Status: Complete for the bounded fresh backend prerequisite; benchmark
admission remains `NOT_GRANTED`.
Scope: probe the exact planned Author (`gpt-5.6-luna` / `medium`) and Reviewer
(`gpt-5.6-sol` / `high`) configurations using isolated ephemeral D-drive roots,
without product, benchmark, Author, Reviewer or repository execution.
Acceptance: both probes return exit `0`, exact `HEALTHCHECK_OK`, no event errors
and no observed tool calls — **PASS**.
Verification/evidence: Codex CLI `0.153.4`, sequential 120-second probes,
before/after tasklist inspection and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\codex-backend-health-20260908-v020\backend-admission.json`;
the snapshots each contain 11 `dotnet.exe` processes.
Boundary / next dependency: the quiet interval remains `NOT_ESTABLISHED`; the
freeze-time `backendHealth: NOT_TESTED` field and frozen identity are unchanged,
and a separate admission decision is required before Author/Reviewer token
spend. Report:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_20_BACKEND_ADMISSION_PREFLIGHT_20260908.md`.

## Midterm evaluation and forward goal contract — 2026-09-08

The current static candidate and the remaining qualification boundary are
captured in the canonical goal record:
`docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_MIDTERM_EVALUATION_AND_GOALS_20260908.md`.
It preserves the existing Round 1 thresholds, separates admission from
execution, requires a new freeze for every failure-derived candidate change,
and keeps runtime parity, activation, release and deployment outside this
skill-development scope.
