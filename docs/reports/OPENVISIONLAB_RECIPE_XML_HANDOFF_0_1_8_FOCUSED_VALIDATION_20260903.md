# OpenVisionLab Recipe XML Handoff 0.1.8 — Focused Validation

Date: 2026-09-03 KST  
Repository: `C:\Git\2D\Dev`  
Status: **Complete for the candidate-only correction; benchmark admission and activation remain separate decisions**

## Scope and boundary

This checkpoint updates the installed `openvisionlab-recipe-xml-handoff` candidate
from `0.1.7` to `0.1.8` in the user skill directory. The historical installed
snapshot is retained at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v017-installed-snapshot-20260903`.
No product EXE, Import, Preview/Run, Recipe or layer mutation, Original checkout,
commit, push, release, deployment, or full Authors/Reviewers execution was
performed.

## Correction boundary

- Finalization validates and preserves the immutable baseline tuple before
  projecting a new status or reason.
- A file-backed XML with retained validator `PASS` and no acceptance fields is
  `MEASURE_ONLY` even when upstream is `PROPOSED`; inline/unhashed or unvalidated
  XML remains `PROPOSED`.
- Multi-block requests fail closed in a fixed order: per-image locked
  substitution, graph recomposition/automatic candidate selection, direct
  layer/routing mutation, missing datum identity, then missing algorithm
  capability. The public reasons are the generic v1 vocabulary; historical
  `AUTO_REJECTED_*` aliases are not added.
- The candidate remains `EXPLICIT_ONLY`, `candidate`, outside normal dispatch,
  inactive, and unqualified.

## Candidate evidence

The machine-readable integrity record is:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v018-focused-20260903\candidate-integrity.json`

| Resource | SHA-256 | Length |
| --- | --- | ---: |
| `SKILL.md` | `3EAD10B814C1C66DAB327AAA2BE910B7E065C8AE9C1B6E7A1FF936818503ED34` | 26430 |
| `references/recipe-xml-handoff-contract.md` | `5384EE42E83B086D73451BF475AE29FAA2736FED4B0441548D937A01D442CDC8` | 15517 |
| `scripts/validate_recipe_xml_handoff.py` | `C93D9C6D1F5E7E935BA04E9B424032F134D774FA52A3EE55DE4B9B42761D0CAE` | 68864 |
| `scripts/test_recipe_xml_handoff.py` | `73F6090A433318644D72D9FE9173B13DE1E9803C7FA1875F1D1B8CD02560BD51` | 43549 |

## Verification

| Check | Evidence | Result |
| --- | --- | --- |
| Focused regression suite | `recipe-xml-handoff-v018-focused-20260903/focused-tests.txt` | 22/22 `OK` |
| Skill Creator structural validation | `recipe-xml-handoff-v018-focused-20260903/quick-validate.txt` | `Skill is valid!` |
| Python compilation | `recipe-xml-handoff-v018-focused-20260903/py-compile.txt` | exit `0` |
| Explicit-only policy | installed `agents/openai.yaml` | `allow_implicit_invocation: false` |

The added regression is
`test_upstream_proposed_does_not_override_passed_measure_only_xml`; it proves
the v0.1.8 status projection at the validator boundary. The existing baseline,
inline, public-reason, authority-lock, specialist-frame, and artifact-binding
regressions remain green.

## Closure record

Status: **Complete**  
Scope: installed v0.1.8 candidate correction and focused validation only.  
Acceptance criteria: candidate resources are internally versioned, explicit-only,
public alias vocabulary is unchanged, the new status projection is covered by a
regression, and Skill Creator/compile checks pass — **met**.  
Verification: 22 focused tests, Skill Creator `quick_validate.py`, and Python
`py_compile`, with transcripts on D:.  
Evidence: candidate integrity JSON and the four installed candidate resources
listed above.  
Boundary / next dependency: this does not prove full benchmark quality, runtime
parity, semantic detection, qualification, activation, or release. A separate
admission decision is required before using the new 112-attempt identity.
