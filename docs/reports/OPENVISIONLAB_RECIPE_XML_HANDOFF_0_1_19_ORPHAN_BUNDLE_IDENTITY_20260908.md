# OpenVisionLab Recipe XML Handoff 0.1.19 — Orphan Bundle Identity Guard

Date: 2026-09-08 KST  
Status: Complete  
Scope: fail closed when standalone validation receives `attempt_id` or
`case_id` without the complete artifact-bundle options.

## Evidence and correction

Candidate 0.1.18 documented that standalone handoff does not carry benchmark,
attempt, or case identity. The validator still treated `attempt_id` and
`case_id` as optional comparison fields that were ignored when no artifact
contract, manifest, and kind were supplied. A direct probe showed both orphan
options returned `errors: []` for an otherwise valid standalone handoff.

Candidate 0.1.19 includes those identity options in the bundle-option presence
check. A standalone handoff remains valid with no bundle options; a supplied
`attempt_id` or `case_id` now returns `E_ARTIFACT_OPTIONS` unless the complete
bundle option group is present. Full clear/red bundle validation remains
unchanged.

The XML schema, handoff status/reason vocabulary, authority/frame/graph rules,
explicit-only policy, product-action flags, qualification boundary, and bundle
artifact semantics are unchanged.

## Verification

- Candidate focused regression: **53 tests passed**.
- Skill Creator format validation: **PASS**.
- Registry validation: **PASS**, no errors.
- Documentation index: **PASS** after adding this report to the governance
  route.
- Existing standalone, clear-bundle, red-bundle, manifest-order, and public
  reason tests remain green.

Evidence root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-integration-20260908`.  
Focused test log: `candidate-0.1.19-focused-tests.log`.  
Format log: `candidate-0.1.19-skill-format.log`.  
Registry log: `candidate-0.1.19-registry-validation.log`.  
Documentation-index log: `candidate-0.1.19-documentation-index.log`.  
Source before/after hashes: `candidate-0.1.19-source-manifest.json`.

Boundary: this is a candidate-only validator boundary correction. It does not
execute images, start a product runtime, create a benchmark identity, admit a
full benchmark, activate or qualify the candidate, or authorize Import/
Preview/Run.

## Closure record

Status: **Complete**  
Scope: candidate 0.1.19 orphan bundle-identity routing guard.  
Acceptance: orphan `attempt_id`/`case_id` options fail closed; standalone and
complete bundle paths retain their prior behavior; focused and governance
checks pass.  
Verification: 53 focused tests, Skill Creator, registry and documentation-index
checks passed.  
Evidence: this report and the D-drive evidence root above.  
Boundary / next dependency: full benchmark admission remains separately
deferred and requires an operator-coordinated quiet interval.
