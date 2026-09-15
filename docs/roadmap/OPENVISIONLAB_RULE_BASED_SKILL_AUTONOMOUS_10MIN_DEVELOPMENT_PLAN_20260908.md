# OpenVisionLab Rule-Based Skill Autonomous 10-Minute Development Plan

Date: 2026-09-08 KST  
Status: **Active plan; automatic benchmark admission remains blocked; no recurring automation is active**  
Cadence: none; the former 10-minute heartbeat was deleted on 2026-09-09  
Automation: **OpenVisionLab 2D Rule-Based Skill 10분 개발** (`DELETED`)

## Purpose

This plan gives the recurring Codex heartbeat a fixed development order and a
small, reviewable result for every run. It continues the installed rule-based
skill in `C:\Git\2D\Dev` and keeps static development, benchmark admission,
product runtime qualification and release as separate boundaries.

The heartbeat must check whether another test, benchmark, product process or
workspace task is active before changing anything. If work is active, it
reports `WORKING / DEFERRED` and does not overlap or restart it. If the
workspace is idle, it advances only the highest incomplete phase and records
the phase result and evidence.

## Ordered phases

### Phase 1 — Static contract and prompt alignment

Goal: keep the teaching, Matching and Recipe XML handoff contracts internally
consistent with the supplied 20-section rule-based vision prompt.

Acceptance:

- current skill versions, registry and conditional references agree;
- focused regression, Skill Creator and documentation-index checks pass;
- prompt sections `1..20` remain covered by the current teaching skill;
- no unsupported Tool, parameter, coordinate, tolerance or runtime claim is
  added; and
- the evidence and report identify the exact source hashes and boundary.

Current result: **Complete**. The current alignment evidence is recorded in
`docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_PROMPT_ALIGNMENT_RECHECK_20260908.md`.

### Phase 2 — Admission readiness and collision-safe execution

Goal: prepare one exact candidate identity for a controlled benchmark without
starting it until the workspace is safe and an operator has made a separate
admission decision.

Acceptance:

- the candidate freeze and D-drive evidence remain immutable;
- each heartbeat checks active test/product processes and current repository
  writes before proceeding;
- the admission packet records the reserved quiet interval, start/end process
  and repository snapshots, audit monitor evidence and operator decision; and
- no Author/Reviewer token is spent while the packet is incomplete.

Current result: **Blocked**. Candidate `0.1.20` is frozen and preflighted, but
the operator-coordinated quiet interval and separate admission decision are
not recorded. The current packet is
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-g1-admission-preflight-20260908`.

### Phase 3 — Qualification and failure-derived correction

Goal: after Phase 2 is explicitly admitted, run the existing frozen
112-Author / 16-Reviewer contract, show the result for that phase, and make a
new candidate identity only when a measured gate failure requires a correction.

Acceptance:

- Author, audit, Reviewer, scorer and preservation evidence all retain their
  fixed denominator and model/reasoning contract;
- each quality gate is reported separately, including structure, unsupported /
  invented findings, product actions, critical red handling, Reviewer coverage
  and session consistency;
- a correction is smallest, failure-derived, focused-tested and frozen under a
  new identity before any rerun; and
- runtime parity, field qualification, activation, Original, commit/push,
  release and deployment remain separate decisions.

Current result: **Not started**. Phase 3 cannot begin before Phase 2 closes.

## Ten-minute run contract

Every heartbeat run follows this order:

1. Read the current handoff, this plan, the canonical midterm goals, the
   latest phase report and the D-drive evidence manifest.
2. Detect active benchmark/test/product work and repository writes. If active,
   report the observed state, preserve the current work, and stop the run.
3. Select the first phase whose acceptance is not complete. Do not skip a
   blocker, repeat a completed slice, or create a second benchmark identity.
4. Perform one bounded development or verification action only.
5. Record the phase outcome, exact evidence paths, remaining blocker and next
   action. Send a concise Korean status report for that 10-minute run.

The heartbeat may pause or stop itself only after all three phases and their
required evidence are complete, or when an external prerequisite is blocked.
It must never infer an admission decision from an ordinary user message,
single idle snapshot or backend health probe.

## Non-negotiable boundaries

- Dev checkout only; preserve unrelated dirty work and do not reset, clean,
  stage, commit or push.
- No Original/Public mutation, release, deployment, product EXE, Import,
  Preview/Run or field/hardware qualification under this plan.
- No parallel agent/model or overlapping heartbeat work.
- No automatic Author/Reviewer token spend before the explicit Phase 2 packet.
- Static evidence does not become runtime or production evidence by repetition.

## Closure record

Status: **Complete**  
Scope: recurring three-phase development order and 10-minute reporting
contract.  
Acceptance: phases, gates, collision handling, per-run reporting and explicit
boundaries are defined; current Phase 1/2/3 states are recorded — **met**.  
Verification: aligned with the current handoff, midterm goals, G1 packet and
prompt-alignment evidence.  
Evidence: this plan plus the linked D-drive packets and phase reports.  
Boundary / next dependency: Phase 2 remains blocked until the operator
quiet-interval and separate admission record exists.
