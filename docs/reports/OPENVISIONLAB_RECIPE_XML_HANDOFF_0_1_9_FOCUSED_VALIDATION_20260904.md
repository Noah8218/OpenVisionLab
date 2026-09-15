# OpenVisionLab Recipe XML Handoff 0.1.9 — Focused Validation

Date: 2026-09-04 KST  
Repository: `C:\Git\2D\Dev`  
Installed candidate: `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff`  
Status: **Complete for the candidate-only correction; fresh benchmark admission and activation remain separate decisions**

## Scope and boundary

This checkpoint updates the installed `openvisionlab-recipe-xml-handoff`
candidate from `0.1.8` to `0.1.9` after the backend-recovery Round 1 review.
The correction is limited to evidence-language integrity, frame/layer
invariants, explicit graph exclusions, case/specialist ownership separation,
Contour/projection completeness, per-input observation discipline, final-state
projection, and public reason-contract conflict handling. The handoff validator
now also checks `Main -> SourceFrame`, non-transforming frame preservation,
synthetic pending frames, evidence-state overclaim, specialist default ownership,
operator parameter ownership against supplied locks, and Contour/Morphology/Edge
policy locks.

No product EXE, Import, Preview/Run, Recipe or layer/routing mutation, candidate
activation, normal dispatch, Original checkout change, commit, push, release,
deployment, or new full benchmark was performed. The v0.1.8 backend-recovery
Round 1 root remains immutable and is reported separately.

## Candidate resources

The machine-readable schema remains `openvisionlab-recipe-xml-handoff-v1`, and
the candidate remains `candidate`, `EXPLICIT_ONLY`, inactive, unqualified, and
outside normal dispatch.

| Resource | SHA-256 | Length |
| --- | --- | ---: |
| `SKILL.md` | `1D9B82E8618F3EC4111DFA8A81C3711E258E751631D212BB3D0585346E0115E1` | 31767 |
| `references/recipe-xml-handoff-contract.md` | `9DFD5137FD3B6FE6E88CF216C55217B780BC3D32A4982DE6F671531AB35F2C3C` | 16571 |
| `scripts/validate_recipe_xml_handoff.py` | `07FFB0C26793BB5E53DB57EFD120DA8895D0BEE89334EF557679B09F24559919` | 75785 |
| `scripts/test_recipe_xml_handoff.py` | `2A872184A7F27C5D986972D0648763166225CEDA2971CDA71B974C7A9B1A42A8` | 46050 |

Machine-readable integrity record:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v019-focused-20260904\candidate-integrity.json`

## Verification

All test-only output was written under the D-drive test root:

| Check | Evidence | Result |
| --- | --- | --- |
| Focused validator regression | `recipe-xml-handoff-v019-focused-20260904/focused-tests.txt` | `24` tests, `OK`, exit `0` |
| Skill Creator structural validation | `recipe-xml-handoff-v019-focused-20260904/quick-validate.txt` | `Skill is valid!`, exit `0` |
| Python compilation | `recipe-xml-handoff-v019-focused-20260904/py-compile.txt` | exit `0` |
| Explicit-only invocation policy | installed `agents/openai.yaml` | `allow_implicit_invocation: false` |

The new regression coverage proves:

- non-transforming Tools cannot invent a frame transition or use a synthetic
  frame, and a `Main` input must remain `SourceFrame`;
- a supplied `CONTOUR_POLICY` lock requires `DetectMode=External`, exact
  approximation, and exact area values; and
- existing baseline, inline, public-reason, authority-lock, specialist-frame,
  status-projection, artifact-binding, and catalog-allowlist checks remain green.

## Decision and next dependency

The candidate-only correction and its focused validation are complete. This
does not prove that an LLM will pass the full 112-attempt benchmark, runtime
parity, semantic detection, human comparison, Recipe qualification, or product
readiness. The next research gate is a new owner-approved freeze/preflight
identity and fresh Round 1 admission using v0.1.9; the frozen v0.1.8 identity
must not be repaired or resumed. The four RT12/RT17/RT23/RT30 public-reason
expectation conflicts remain a contract/corpus decision boundary and must not
be solved by hidden case mapping.

## Closure record

Status: **Complete**  
Scope: installed v0.1.9 candidate correction and focused validation only.  
Acceptance criteria: candidate resources share version `0.1.9`, new failure-derived boundaries are documented, 24 focused tests pass, Skill Creator validation passes, Python compilation passes, and explicit-only policy remains — **met**.  
Verification: D-drive focused regression, `quick_validate.py`, `py_compile`, and candidate integrity hash record.  
Evidence: the integrity record and transcripts listed above.  
Boundary / next dependency: fresh benchmark admission, activation, runtime parity, qualification, release, deployment, and Original-repository work remain unapproved.
