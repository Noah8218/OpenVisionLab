# Rule-Based Skill Static Development Worklist

Updated: 2026-09-08 KST

Status: Complete
Priorities 1–5 retain their completed evidence. Follow-up 6 adds actual
general-to-XML authoring for shared-mask Blob and dark-seal LineDistance plans.

## Decision and scope

The user chose to defer the full benchmark and requested a prioritized list
with continued skill development. This superseded the pending quiet-interval
question in the two 0.1.13 interruption reports; at that point no new benchmark
identity was started from this worklist. The later, explicitly authorized
identity/freeze continuation is recorded below.

Priorities 1/2 closed at `0.1.14`, priority 3 at `0.1.15` and priority 4 at
`0.1.16`. The new continuation addresses the producer instructions needed by
that new consumer profile: general teaching `1.0.2` and XML candidate `0.1.17`.
Matching remains `0.2.1`; completed validator behavior is preserved.
Follow-up 6 verifies these same versions; it makes no skill/source change.
The product remains a deterministic OpenCvSharp4 recipe workbench with
RC/pre-production evidence, not commercial GA. Preserve explicit Preview/Run,
operator-owned policy, traceable evidence and the shortest sample-to-recipe
workflow. Camera, lighting, PLC/I/O, MES and deployment platforms are excluded.

## Selected priorities

| Order | Work | Current state | Required result | Recommended model | Reasoning effort |
| --- | --- | --- | --- | --- | --- |
| 1 | Propagate retained upstream blocking states into the XML boundary | Complete | A nonblocked handoff cannot pass with upstream WAIT/REJECTED or stage/gate WAIT/FAIL; existing blocked, baseline and measurement paths remain valid | gpt-5.6-terra | high |
| 2 | Reject ambiguous or nonfinite serialized JSON | Complete | Duplicate keys, NaN/Infinity, numeric overflow and malformed UTF-8 produce structured failure at root and linked-input boundaries, including baseline upstream JSON | gpt-5.6-terra | medium |
| 3 | Bind stages to Tool layers and coordinate frames without cardinality bypass | Complete | Ordered primary paths, full Tool coverage, cross-stage frame consistency and actual-Tool evidence/owner checks; grouped/interleaved/overlapping/reordered declarations preserved | gpt-5.6-terra | high |
| 4 | Require evidence for OPERATOR_SELECTED claims | Complete | Empty evidence cannot establish operator selection; validate the evidence's claimed ownership and retain valid explicit selections | gpt-5.6-terra | medium |
| 5 | General teaching to XML selection-evidence transfer | Complete | Conditional canonical-profile routing, supplied authority preserved, missing binding remains WAIT, reviewed 1.0.2 accepted without weakening guards | gpt-5.6-terra | medium |
| 6 | Independent general-to-XML static integration | Complete | Shared Threshold/two Blob and pixel LineDistance plans preserve caller policy and pass actual XML/static handoff validation | gpt-5.6-terra | medium |
| 7 | Separate standalone XML and supplied bundle finalization | Complete | Candidate 0.1.18 states the base handoff path and contract-bound bundle audit as conditional modes without changing validator semantics | gpt-5.6-terra | low |
| 8 | Reject orphan bundle identity options | Complete | Candidate 0.1.19 rejects standalone `attempt_id`/`case_id` inputs instead of silently ignoring them; complete bundle and standalone paths remain valid | gpt-5.6-terra | medium |
| 9 | Require the bundle identity pair | Complete | Candidate 0.1.20 rejects contract-bound audits that omit `attempt_id` or `case_id`; supplied identities remain cross-checked against the manifest | gpt-5.6-terra | medium |

Priority 4 now requires a caller-owned binding for each fresh operator
selection; its five scoped acceptance criteria passed. The project-wide remaining
skill qualification gate is a valid full benchmark; its prerequisite is an
operator-coordinated interval without concurrent product execution or Dev
writes throughout the Author audit. No Author/Reviewer tokens are scheduled
until that prerequisite and a separate admission decision are available.

Automatic issue registration for priorities 3 and 4 could not proceed:
the existing ledger validator rejects `PL-0010.json` for a terminal-state
`state.next_action`, which also invalidates the `PL-0011#M4` reference.
The CLI refuses creation while those errors exist. No issue ID was issued and
`.proofline` was not changed. This worklist is the durable record for the selected
items; ledger repair is a separate scope. Details: `ledger-registration-note.md`
under the evidence root.

## Standalone finalization follow-up — candidate 0.1.18

Follow-up 7 is Complete. Candidate 0.1.17's validator already accepted a
file-backed handoff without artifact-bundle options, but its finalization prose
could be read as requiring the benchmark clear/red artifact set for every
request. Candidate 0.1.18 now states the standalone base-validator path and
reserves the artifact manifest, attempt/case identity and second audit for a
caller-supplied artifact contract.

The machine-readable schema, XML/authority/frame/graph checks, public reasons,
bundle validator and explicit-only policy are unchanged. Fifty-two focused
tests, Skill Creator, registry and documentation-index checks passed. No new
benchmark identity, driver, artifact contract, product execution or skill
qualification was introduced.

Evidence: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_18_STANDALONE_FINALIZATION_20260908.md`.
Physical logs: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-integration-20260908`.

## Orphan bundle identity follow-up — candidate 0.1.19

Follow-up 8 is Complete. Candidate 0.1.18 documented standalone handoff as
having no benchmark/attempt/case identity, but the validator silently ignored
`attempt_id` or `case_id` when no artifact contract, manifest, and kind were
supplied. Candidate 0.1.19 includes those options in the bundle presence check
and returns `E_ARTIFACT_OPTIONS` for an orphan identity option. Standalone
validation and complete clear/red bundle checks remain valid.

The XML schema, handoff semantics, explicit-only lifecycle, and qualification
boundary are unchanged. Fifty-three focused tests, Skill Creator, registry and
documentation-index checks passed. No benchmark identity, product execution or
qualification was introduced.

Evidence: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_19_ORPHAN_BUNDLE_IDENTITY_20260908.md`.
Physical logs: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-integration-20260908`.

## Bundle identity follow-up — candidate 0.1.20

Follow-up 9 is Complete. Candidate 0.1.19 rejected orphan `attempt_id` or
`case_id` options, but a contract-bound audit could still omit either identity
value. Candidate 0.1.20 now requires both values before bundle validation and
continues to compare supplied values with the retained manifest. Standalone
validation and complete clear/red bundle checks remain valid.

The XML schema, handoff semantics, explicit-only lifecycle, and qualification
boundary are unchanged. Fifty-three focused tests, Skill Creator, registry and
documentation-index checks passed. No benchmark identity, product execution or
qualification was introduced.

Evidence: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_20_BUNDLE_IDENTITY_REQUIRED_20260908.md`.
Physical logs: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-integration-20260908`.

## Qualification admission readiness preflight — 2026-09-08

The read-only preflight for candidate `0.1.20` is complete, but benchmark
admission remains `BLOCKED`. Candidate version/source alignment, the 53-test
focused regression, Skill Creator, registry, documentation index and source
manifest all pass. The retained strict corpus is frozen for `0.1.13`, not
`0.1.20`, so a new identity and freeze are required. The snapshot also found no
exclusive quiet interval is not established: the final snapshot had no active
`dotnet`/MSBuild process, but 237 dirty worktree status lines remain, and a
point-in-time idle snapshot cannot prove a full-audit quiet interval. The prior retry's 4/112 incomplete result
and preservation PASS remain immutable evidence; its backend PASS is historical.

No new benchmark identity or Author/Reviewer run was started. The next
admission requires the operator-coordinated quiet interval, a fresh `0.1.20`
freeze, fresh backend probes and a separate admission decision.

Evidence: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_20_ADMISSION_READINESS_20260908.md`.
Physical snapshot: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-admission-readiness-20260908`.

## New 0.1.20 identity and freeze preflight — 2026-09-08

This explicitly authorized continuation is Complete for identity preparation
and static preflight. The new D-drive benchmark identity is
`openvisionlab-rule-based-round1-v0_1_20-strict-corpus-20260908`, copied from
the preserved `0.1.13` source root and rebound to the current candidate
`0.1.20` snapshots. The final freeze SHA-256 is
`91CB69791282DA6083F6A3303590425AEA7FAD9AE28C8766DA26FEFF154F1394`.

The freeze preceded preparation of exactly 112 attempt metadata records. The
target has no Author outcomes, Reviewer outputs, scorer results or product-run
artifacts. Its final gate passed all 20 checks with zero failures: source-root
immutability, current skill/validator/runtime snapshots, direct and projected
validation, static compatibility, 53 focused candidate tests, Skill Creator,
harness/runner regressions, self-test, clear/red/baseline preservation and 273
recursive path/hash pairs.

This preflight did not run Authors, Reviewers, a product EXE, Import,
Preview/Run, or a fresh backend probe. Point-in-time tasklist snapshots had no
matching `dotnet` process, but the operator-coordinated quiet interval required
for a full Author audit is not established. Runtime parity is
`NOT_MEASURED`; the candidate remains inactive and unqualified, and a separate
admission decision is required before token spend.

Report: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_20_NEW_IDENTITY_PREFLIGHT_20260908.md`.
Physical evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-round1-v020-preflight-20260908`.

## Fresh backend prerequisite probe — candidate 0.1.20

This bounded continuation is Complete for a fresh minimal backend availability
probe, while benchmark admission remains `NOT_GRANTED`. The planned Author
(`gpt-5.6-luna` / `medium`) and Reviewer (`gpt-5.6-sol` / `high`) configurations
both completed the exact `HEALTHCHECK_OK` prompt with exit `0`, no event errors
and no observed tool calls under Codex CLI `0.153.4`.

The sequential probes used isolated ephemeral D-drive roots and did not execute
the product, benchmark harness, Authors, Reviewers or repository operations.
Before and after `tasklist` snapshots each contained 11 `dotnet.exe` processes,
so the operator-coordinated quiet interval remains `NOT_ESTABLISHED`. This
probe does not alter the immutable freeze-time `backendHealth: NOT_TESTED`
field, final-gate result or freeze hash, and it does not constitute the
separate admission decision required before token spend.

Report: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_20_BACKEND_ADMISSION_PREFLIGHT_20260908.md`.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\codex-backend-health-20260908-v020\backend-admission.json`.

## Midterm evaluation and development goals — 2026-09-08

The current static contract is strong enough for a controlled qualification
attempt, but the skill is not qualified. General teaching `1.0.2` and Matching
`0.2.1` remain active; XML handoff `0.1.20` is a candidate with focused/static
PASS, a frozen identity and a fresh minimal backend prerequisite PASS. Full
Round 1 quality remains unproven, the quiet interval is not established, and
runtime parity is `NOT_MEASURED`.

The next plan is evidence-gated: establish a quiet interval and separate
admission decision, run the existing 112-Author/16-Reviewer contract, apply
only failure-derived candidate corrections under a new freeze, and consider
promotion only after every existing scorer gate passes. The plan does not add
hardware, product runtime, per-image mutation, or release scope.

Canonical evaluation and goal contract:
`docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_MIDTERM_EVALUATION_AND_GOALS_20260908.md`.

Latest all-goal verification: G0 `PASS`; G1 `BLOCKED`; G2/G4 `NOT_STARTED`;
G3 `CONDITIONAL`; G5 `NOT_MEASURED`. Evidence:
`docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_GOALS_VERIFICATION_20260908.md`.

G1 admission preflight recheck: the repaired current tasklist snapshot observed
`dotnet.exe=0` and `MSBuild.exe=0`, while the retained post-static snapshot
observed `23/11`. The point-in-time improvement does not establish the required
operator-coordinated interval, and no separate admission decision was found in
the frozen target. Report:
`docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_G1_ADMISSION_PREFLIGHT_RECHECK_20260908.md`.
Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-g1-admission-preflight-20260908\g1-admission-preflight.json`.

Prompt alignment recheck: the supplied 20-section rule-based vision prompt was
matched in order to the current teaching skill `1.0.2` and its conditional
references. Static coverage passed; this does not replace image, benchmark,
runtime or field qualification. Report:
`docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_PROMPT_ALIGNMENT_RECHECK_20260908.md`.
Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-prompt-alignment-recheck-20260908\prompt-alignment-recheck.json`.

## Independent static integration follow-up — unchanged 1.0.2 / 0.1.17

Follow-up 6 is Complete. Two independent producer contexts and two independent
XML contexts retained caller policy, one fixed graph, SourceFrame and unqualified
runtime states. Both actual XML files passed the headless compatibility checker
and the existing handoff validator. The parent compared branch, ROI/area, false
threshold flags, all LineDistance parameters and absent calibration/acceptance.

No skill/source correction was required by these two outcomes. The shared-mask
plan retains finer parameter ownership in its existing purpose/observation/
review fields; optional typed USE_THRESHOLD ownership remains a known profile
limit, not a new lock supported by this result. All nine selected scopes are closed.
Evidence: `docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_STATIC_INTEGRATION_20260908.md`.
Physical root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-integration-20260908`.

## Producer transfer follow-up — 1.0.2 / 0.1.17

This is a new producer/consumer compatibility requirement after the 0.1.16
profile, not a reopening of its completed validator acceptance criteria.

1. Route explicitly requested XML preparation to the canonical selection
   profile; ordinary inspection design still needs no XML authority manifest.
2. Retain supplied statements, evidence and authority bytes; never manufacture
   an operator binding, relabel actual selection to evade a gate, or mark a
   missing binding ready. Keep authority outside the unchanged v1 envelope.
3. Accept reviewed teaching 1.0.2 with older reviewed versions and every existing
   semantic/schema/invocation gate preserved.
4. Pass independent supplied-selection and missing-binding authoring requests,
   focused compatibility/regression, skill/registry/index and scoped proof.

Example: the exact operator statement plus request-file evidence and a supplied
matching selection row can proceed to explicit XML review. The same request
file without that row remains a teaching WAIT artifact with the actual
OPERATOR_SELECTED provenance intact; it is not a validated XML input.

Evidence: `docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_PRODUCER_HANDOFF_20260908.md`.
Physical root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-producer-handoff-20260908`.
Follow-up 5 is closed; all five selected static scopes retain completed evidence.

## Completed 0.1.14 acceptance criteria

1. Preserve a source snapshot and reproduce selected failures on 0.1.13.
2. Reject actual blocking upstream states without changing public reason
   precedence or reinterpreting protected immutable baselines.
3. Reject duplicate keys and nonfinite numbers through the common JSON reader,
   including hash-valid linked evidence; malformed UTF-8 returns structured
   validation diagnostics.
4. Preserve finite JSON, UTF-8 BOM support, teaching versions 1.0.0/1.0.1,
   PROPOSED/MEASURE_ONLY behavior and existing baseline regression cases.
5. Pass focused tests and independent review; synchronize the skill, reference,
   validator, registry, navigation and reusable evidence record.

Example negative input: a valid handoff containing
`"run": true, "run": false` in the same JSON object must fail parsing; the
last value must not hide the first. A retained upstream stage in `FAIL` must
not become a `MEASURE_ONLY` XML handoff merely because static XML validation
passed. A planned `NOT_REVIEWED` runtime gate alone is not that failure case.

## Completed 0.1.15 acceptance criteria

1. Retain and reproduce the 0.1.14 mapping failures before editing.
2. Check the complete primary Tool input order and bind each stage to a unique,
   ordered layer path, with coverage for every Tool.
3. Check Main/synthetic frames, shared-layer frame agreement, preserving-Tool
   equality and evidence/owner guards independently of stage counts/positions.
4. Preserve grouped/interleaved/overlapping paths, reordered stage declarations,
   explicit transforms and immutable baseline behavior.
5. Pass focused and independent checks, update registry/navigation and retain
   the evidence in the focused report.

Example: `Main -> Mask`, an interleaved `Main -> BranchResult`, then
`Mask -> Result` may use one `Main -> Result` stage plus the branch stage.
Grouping must not let `Mask` change coordinate frames or leave a Tool without
a stage. Per-Tool stages must not hide cycles or forward references in an
inline draft.

0.1.15 report:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_15_FOCUSED_VALIDATION_20260908.md`.
0.1.15 evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-stage-mapping-20260908`.

## Completed 0.1.16 acceptance criteria

1. Retain the 0.1.15 source and reproduce unsupported operator claims passing.
2. Reject blank statements, empty evidence, absent/ambiguous selection authority,
   invalid owners and mismatched statement/evidence bindings.
3. Accept caller-supplied OPERATOR/OPERATOR_LOCK selections with exact retained
   evidence; preserve other provenance states and immutable baseline semantics.
4. Pass focused regression and independent contract/implementation checks.
5. Align candidate, reference, registry, worklist, navigation and retained proof.

Example: an OPERATOR_SELECTED observation stating `Use the full image as the
inspection region.` must match exactly one caller-owned OBSERVATION_SELECTION
row with the same statement and nonempty path/hash evidence. An image or an
unrelated ROI lock alone cannot establish that selection. Evidence order may
differ; paths/hashes use existing normalization and duplicate counts must agree.
The skill consumes authority without creating or approving it. Static PASS does
not authenticate the caller or prove the natural-language claim's truth.

Current report: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_16_FOCUSED_VALIDATION_20260908.md`.
Current evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-operator-evidence-20260908`.

## Earlier 0.1.14 evidence and boundaries

Focused report:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_14_FOCUSED_VALIDATION_20260908.md`.

Physical evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-static-hardening-20260908`.

The two stopped 0.1.13 benchmark identities retain their separate INCOMPLETE
results. This work does not combine or rerun them, change frozen inputs,
activate the candidate, qualify inspection accuracy, change product code/UI,
touch Original, or authorize commit/push/release.
