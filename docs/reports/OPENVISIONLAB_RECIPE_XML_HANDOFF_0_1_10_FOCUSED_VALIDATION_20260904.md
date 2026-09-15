# OpenVisionLab Recipe XML Handoff 0.1.10 — Focused Validation

Date: 2026-09-04 KST  
Repository: `C:\Git\2D\Dev`  
Installed candidate: `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff`  
Status: **Complete for the candidate-only contract correction; benchmark qualification and activation remain separate decisions**

## Scope and boundary

This checkpoint installs candidate `0.1.10` after the v0.1.8 backend-recovery
Round 1 exposed four public-reason expectation conflicts and two related
precedence ambiguities. The correction is limited to the handoff contract and
its deterministic regression suite. It does not alter the OpenVisionLab
product, execute Import/Preview/Run, mutate a Recipe or layer/routing state,
activate the candidate, add normal dispatch, touch `C:\Git\2D\Original`, or
authorize release/deployment.

The public precedence is now explicit:

- missing datum/geometry -> `OPERATOR_DATUM_FEATURES_REQUIRED`;
- supplied geometry without physical-unit calibration -> `CALIBRATION_REQUIRED`;
- an otherwise unprovided capability -> `WAIT_ALGORITHM_GAP`;
- an explicit invented/bypass ToolType or semantic detector ->
  `UNSUPPORTED_SEMANTIC_CLAIM`;
- per-image source/template/ROI/angle/threshold mutation ->
  `PER_IMAGE_OVERRIDE_FORBIDDEN`;
- per-image area/score/tolerance/Good/Bad acceptance mutation ->
  `PER_IMAGE_ACCEPTANCE_MUTATION_FORBIDDEN`;
- an observed retained upstream stage `FAIL` -> `UPSTREAM_STAGE_FAIL` before
  graph-review; graph-review is `UPSTREAM_GRAPH_REVIEW_REQUIRED` only when no
  observed stage failure exists.

## Candidate resources

The machine-readable schema remains `openvisionlab-recipe-xml-handoff-v1`.
The candidate remains `candidate`, explicit-only, inactive, unqualified, and
outside normal dispatch.

| Resource | SHA-256 | Length |
| --- | --- | ---: |
| `SKILL.md` | `3C617B1DF052B54EE1D29868C342576577E4F624C8496A0EF2CC95261CA97B95` | 34104 |
| `references/recipe-xml-handoff-contract.md` | `D98890799E313C718E7912E03DADFA952D86D9FC4A56653EEE6B2F4502825B45` | 17729 |
| `scripts/validate_recipe_xml_handoff.py` | `563C34999274206CF93C7C85BAB6CBBB8E855B7A72EFC66F30B865B8CA9067F6` | 75786 |
| `scripts/test_recipe_xml_handoff.py` | `2A6ED3DCF27E6530D3CCAEF01EAF680E917A2F4304354D1CBEEFDF0D28F0A379` | 47178 |

Integrity record:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v010-focused-20260904\candidate-integrity.json`

`agents/openai.yaml` retains `allow_implicit_invocation: false` (explicit-only).

## Verification

All test-only output is under the D-drive test root:

| Check | Evidence | Result |
| --- | --- | --- |
| Focused validator regression | `recipe-xml-handoff-v010-focused-20260904/outputs/focused-tests.txt` | `25` tests, `OK`, exit `0` |
| Skill Creator structural validation | `recipe-xml-handoff-v010-focused-20260904/outputs/quick-validate.txt` | `Skill is valid!`, exit `0` |
| Python compilation | `recipe-xml-handoff-v010-focused-20260904/outputs/pycompile-exit-code.txt` | exit `0` |

The new regression covers the complete public-reason precedence matrix above
and keeps the prior frame/layer, owner-provenance, evidence-language,
Contour/projection, baseline, artifact-binding, and catalog-allowlist guards.

## Closure record

Status: **Complete**  
Scope: installed v0.1.10 candidate correction and focused validation only.  
Acceptance criteria: candidate resources share version `0.1.10`, the four
conflict-derived precedence boundaries are explicit, 25 focused tests pass,
Skill Creator validation passes, Python compilation passes, and explicit-only
policy remains — **met**.  
Verification: D-drive focused regression, `quick_validate.py`, `py_compile`,
and candidate integrity hash record.  
Evidence: the integrity record and transcripts listed above.  
Boundary / next dependency: the separate v0.1.10 freeze/preflight is recorded
independently; Authors/Reviewers execution, qualification, activation,
runtime parity, product execution, Original mutation, release, and deployment
remain unapproved.
