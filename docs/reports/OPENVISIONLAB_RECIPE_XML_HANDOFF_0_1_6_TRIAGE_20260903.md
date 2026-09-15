# OpenVisionLab Recipe XML Handoff 0.1.6 — Round 1 Failure Triage

Date: 2026-09-03 KST  
Repository: `C:\Git\2D\Dev`  
Candidate: `openvisionlab-recipe-xml-handoff 0.1.6`  
Benchmark root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v016-corpusfix-auditfix2-20260903`  
Status: **Complete for evidence triage; the underlying Round 1 admission remains Incomplete**

## Scope and decision boundary

This record classifies the observed v0.1.6 Round 1 failures before any
candidate correction or new benchmark identity is created. It does not rewrite
the frozen root, change the installed candidate, activate or dispatch a skill,
run the product, modify a Recipe/layer/routing, change the Original checkout,
commit, push, release, or deploy.

The evidence sources are the frozen benchmark contract and hidden key, the
112-attempt score, audit run `20260903T032852638Z-69a7e942`, retained attempt
artifacts, the installed candidate `SKILL.md` and validator, and the v0.1.6
Round 1 result report.

## Admission-blocking chain

1. Authors completed `112/112` and the wrapper exited `0`.
2. The external monitor reported `PASS`, but audit event `72` invoked the
   canonical static validator with the benchmark root instead of the assigned
   attempt root as its scan root.
3. The scorer therefore rejected the audit summary as contract-invalid,
   exposed `AUDIT_COVERAGE_INCOMPLETE: 0/112`, and returned `INCOMPLETE`.
4. The Reviewer runner correctly stopped at its precondition: `0/16` contexts
   and no reviewer evidence were created.

The monitor's process-identity classification and the scorer's scan-root
contract are different checks; the former being `PASS` does not make the
latter valid.

## Failure ownership

### Benchmark protocol, scorer, or frozen-corpus compatibility

- **Static-validator scan root:** event `72` is an attempt/protocol execution
  defect. A new run must pass the exact assigned attempt root and must retain
  the event as a violation if it does not.
- **Deliberate hash-mismatch red case:** `RT03` emits the hidden-key reason
  `UPSTREAM_HASH_MISMATCH`, but the scorer also adds
  `REFERENCED_FILE_HASH_MISMATCH` through its generic linked-reference check.
  The scorer needs an explicit expected-mismatch exception or a separate
  representation for this red case; the old attempt must not be rewritten.
- **Historical safe-static baseline shape:** the retained v0.1.2 baseline
  upstream envelope stores evidence entries as strings and non-materialized
  stages with legacy shapes. The v0.1.6 validator requires exact
  `{path, sha256}` references and `PENDING/PENDING/UNKNOWN` packet identity.
  Independent validation consequently reports repeated
  `E_EVIDENCE_REFERENCE_SHAPE` and `E_PENDING_PACKET_IDENTITY` errors for
  `RT13`, `RT24`, `RT25`, `RT26`, and `RT28`. This is a frozen fixture versus
  strict-validator compatibility decision, not evidence to weaken the current
  closed-world rule silently.
- **Reason vocabulary:** the artifact contract and hidden red key still list
  `AUTO_REJECTED_AMBIGUOUS`, `AUTO_REJECTED_PER_IMAGE_OVERRIDE`, and
  `AUTO_REJECTED_REFERENCE_DRIFT`, while the installed v0.1.6 candidate
  explicitly forbids those aliases. The contract owner must choose one public
  vocabulary before another admission run.
- **Case-contract contradictions:** `S3-02` locks `verticalProjection: R_TO_L`
  even though the product `VER_PRJ_DIR` axis accepts `Y_TTOB`/`Y_BTOT`; the
  hidden key nevertheless expects a `MEASURE_ONLY` XML. `S4-01` expects the
  complete Matching/NormalizeImage/Threshold/Blob graph while the case does
  not supply the required `FIXTURE_MIN_VALID_PIXEL_RATIO`. These require a
  new, explicitly reconciled corpus identity or a changed expected answer.

### Candidate-owned validator and projection behavior

- `_compare_authority_locks` currently maps a generic `ROI` lock only to
  `LineDistance` steps. This rejects the correct `Mean` ROI handoff in
  `S1-04-02` with `E_OPERATOR_LOCK_PARAMETER_DRIFT`.
- The candidate contract says an inline, unhashed upstream remains
  `PROPOSED`, but `RT02` emitted `REJECTED`.
- A file-backed XML with no acceptance fields and a passed static validator
  must be `MEASURE_ONLY`; `S3-04-02` emitted `PROPOSED` instead.
- Safe-baseline precedence requires preserving the baseline tuple while
  refusing the requested product mutation. `RT27` changed the retained
  baseline projection to `REJECTED` instead of preserving `MEASURE_ONLY`.
- Public reason routing needs explicit precedence for missing repository
  contracts (`RT05`), a missing locator output frame (`RT10`), and an
  upstream graph/layer request against a retained baseline (`RT28`). The
  current outputs choose different valid-looking reasons from the hidden
  mapping.

### Upstream teaching/authoring behavior

Ten clear attempts selected a Tool-family signature different from the frozen
case: extra `Blob`/`LineDistance` stages or a `Threshold`/`Blob` graph where
`Mean` or `Contour` was required. The XML handoff candidate must not repair
Tool selection because that ownership belongs to the reviewed teaching
envelope. `S3-01-02` also treated an unrequested `LineDistance` helper as a
validator requirement; that is an author/protocol interpretation error.

## Correction boundary for a new identity

The smallest safe next boundary is:

1. Reconcile the frozen public vocabulary, legacy baseline fixture shape,
   `S3-02` projection lock, and `S4-01` NormalizeImage prerequisite. Record
   whether each change is corpus-owned or candidate-compatible; do not alter
   the v0.1.6 root.
2. Correct the candidate's generic ROI owner matching, inline and baseline
   status precedence, and specific reason routing. Add focused regression cases
   for each observed failure.
3. Correct the author runner/scorer contract for assigned scan roots and
   deliberate red hash mismatches. Keep audit evidence and scorer evidence
   independent.
4. Re-run focused checks and the independent positive/blocked forward probe.
   Only after those checks pass, freeze a new candidate version and benchmark
   identity, then repeat the `112 + 16` admission sequence.

No new candidate version, corpus rewrite, or benchmark rerun is authorized by
this triage record alone.

## Acceptance prerequisites for the next admission attempt

- audit summary accepted with `112/112` attempt coverage and no scan-root
  violation;
- scorer exits without fatal or incomplete-evidence issues;
- 16 independent Reviewer contexts cover all 112 attempts exactly once;
- structure `112/112`, red fail-closed `32/32`, clear Reviewer `>=13/16`,
  unsupported/invented findings `0`, product actions `0`, and session
  consistency `>=0.80`.

## Closure record

Status: **Complete** (evidence-triage checkpoint)  
Scope: classification of the v0.1.6 Round 1 audit, scorer, candidate, upstream,
and frozen-corpus failure boundaries.  
Acceptance criteria: every observed blocker is assigned to a correction owner
or an explicit contract decision; no frozen evidence is rewritten — **met**.  
Verification: existing Authors/audit/score artifacts, retained attempt files,
candidate validator source, frozen case/key/contract data, and the v0.1.6
result report were inspected.  
Evidence: this report and the benchmark root listed above.  
Boundary / next dependency: the benchmark remains `INCOMPLETE`; candidate
activation, product execution, and a new version require a separately approved
correction boundary.
