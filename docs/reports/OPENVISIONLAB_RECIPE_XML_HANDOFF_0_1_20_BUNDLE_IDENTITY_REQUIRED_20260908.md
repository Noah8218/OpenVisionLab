# OpenVisionLab Recipe XML Handoff 0.1.20 — Bundle Identity Required

Date: 2026-09-08 KST  
Status: Complete  
Scope: require both `attempt_id` and `case_id` when a contract-bound bundle
audit is requested.

## Evidence and correction

The handoff contract's bundle command includes `--attempt-id` and
`--case-id`, and the audit claims benchmark/attempt/case identity. Candidate
0.1.19 rejected orphan identity options but still allowed a contract,
manifest, and artifact kind with either identity omitted. That left the
command-level identity binding weaker than the documented bundle mode.

Candidate 0.1.20 requires both identity values once the contract, manifest, and
kind select bundle mode. Each value is checked against the retained manifest.
Standalone validation remains complete with no bundle options, and existing
clear/red bundle validation remains valid when the identity pair is supplied.

The XML schema, handoff status/reason vocabulary, authority/frame/graph rules,
explicit-only policy, product-action flags, qualification boundary, and artifact
set/hash/order checks are unchanged.

## Verification

- Candidate focused regression: **53 tests passed**.
- Skill Creator format validation: **PASS**.
- Registry validation: **PASS**, no errors.
- Documentation index: **PASS** after adding this report to the governance
  route.
- Clear-bundle regression passes with both identity values and fails closed when
  either `attempt_id` or `case_id` is omitted.

Evidence root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-integration-20260908`.  
Focused test log: `candidate-0.1.20-focused-tests.log`.  
Format log: `candidate-0.1.20-skill-format.log`.  
Registry log: `candidate-0.1.20-registry-validation.log`.  
Documentation-index log: `candidate-0.1.20-documentation-index.log`.  
Source before/after hashes: `candidate-0.1.20-source-manifest.json`.

Boundary: this is a candidate-only bundle-option correction. It does not
execute images, start a product runtime, create or admit a benchmark identity,
activate or qualify the candidate, or authorize Import/Preview/Run.

## Closure record

Status: **Complete**  
Scope: candidate 0.1.20 contract-bound attempt/case identity guard.  
Acceptance: both identities are required for bundle mode; standalone and
complete clear/red paths remain valid; focused and governance checks pass.  
Verification: 53 focused tests, Skill Creator, registry and documentation-index
checks passed.  
Evidence: this report and the D-drive evidence root above.  
Boundary / next dependency: full benchmark admission remains separately
deferred and requires an operator-coordinated quiet interval.
