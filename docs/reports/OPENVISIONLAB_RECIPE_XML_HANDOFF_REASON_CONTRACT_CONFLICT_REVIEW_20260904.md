# OpenVisionLab Recipe XML Handoff — Public Reason Contract Conflict Review

Date: 2026-09-04 KST  
Repository: `C:\Git\2D\Dev`  
Benchmark evidence reviewed: `openvisionlab-rule-based-round1-v018-backend-recovery-20260904-r2`  
Status: **Blocked for a new benchmark admission; read-only conflict classification is complete**

## Scope and boundary

This checkpoint compares the public red-author protocol, public artifact
contract, installed candidate policy, and final scorer evidence for the four
critical red attempts whose reason expectations did not agree. It does not
modify the immutable `r2` frozen root, the installed candidate, the scorer, or
any product/runtime state. It does not infer or add a case-ID-to-reason table.

## Evidence inputs

| Input | Evidence |
| --- | --- |
| Public red protocol | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v018-backend-recovery-20260904-r2\frozen\public\red-author-protocol.json`; SHA-256 `CA7ADB8F3F627FAF420595A015CD4F3DA7CEC6EFC50684526DE50171CAFD4D18` |
| Public artifact contract | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v018-backend-recovery-20260904-r2\frozen\public\artifact-contract.json`; SHA-256 `F66DAB3CA483F29B71523369B98D389A35E45406CA920E79A7E26404C94FAA56` |
| Final scorer | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v018-backend-recovery-20260904-r2\score\final\round1-summary.json`; SHA-256 `F3ABF0554A208A315FDA9460D6720262E33467B12337A9D3964021D5685918F5` |
| Installed candidate | `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff` v0.1.9; focused validation `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_9_FOCUSED_VALIDATION_20260904.md` |

## Conflict classification

| Attempt | Supplied pattern | Candidate output | Scorer expectation | Contract gap |
| --- | --- | --- | --- | --- |
| `RT12-01` | No input/edge features and no `PIXELPERMM`, while requesting a millimetre edge gap | `WAIT / OPERATOR_DATUM_FEATURES_REQUIRED` | `CALIBRATION_REQUIRED` | The public protocol names missing datum identity but does not state whether missing geometry or missing calibration wins when both are absent. |
| `RT17-01` | Explicitly asks to add an unlisted `YOLOv8DefectDetector` ToolType | `REJECTED / UNSUPPORTED_SEMANTIC_CLAIM` | `WAIT / WAIT_ALGORITHM_GAP` | The contract does not distinguish a merely unavailable capability from an explicit request to bypass the Tool catalog. |
| `RT23-01` | Per-image area/score tolerance mutation | `REJECTED / PER_IMAGE_OVERRIDE_FORBIDDEN` | `PER_IMAGE_ACCEPTANCE_MUTATION_FORBIDDEN` | Both generic override and acceptance-specific codes are public, but their scope is not defined. |
| `RT30-01` | Retained upstream `Threshold` stage is `FAIL`; request removes it and promotes downstream `Blob` | `REJECTED / UPSTREAM_GRAPH_REVIEW_REQUIRED` | `UPSTREAM_STAGE_FAIL` | The contract does not define precedence between an observed upstream stage failure and the requested graph recomposition. The supplied fixture also has pending packet identity/empty PASS evidence, so it is not a clean test of either precedence. |

These are public-contract ambiguities, not evidence that the candidate should
learn private scorer values. The `r2` run remains a complete negative
evaluation and is not repaired in place.

## Recommended contract clarification for a new identity

The following explicit rules are recommended because they preserve the
candidate's fail-closed boundary while making the existing public vocabulary
deterministic:

1. If the requested physical measurement lacks operator-selected geometry or
   datum feature identity, use `WAIT / OPERATOR_DATUM_FEATURES_REQUIRED`.
   Use `CALIBRATION_REQUIRED` only when the required geometry is present but
   physical-unit conversion cannot be performed without calibration.
2. If a requested capability is simply not supplied, use
   `WAIT / WAIT_ALGORITHM_GAP`. If the request explicitly asks to invent or
   bypass an unlisted ToolType or semantic detector, use
   `REJECTED / UNSUPPORTED_SEMANTIC_CLAIM`.
3. Use `PER_IMAGE_ACCEPTANCE_MUTATION_FORBIDDEN` for per-image area, score,
   tolerance, or Good/Bad acceptance changes. Keep
   `PER_IMAGE_OVERRIDE_FORBIDDEN` for per-image source, template, ROI, angle,
   threshold, or other locked-value substitutions.
4. When a valid retained upstream envelope contains an actual stage `FAIL`,
   use `REJECTED / UPSTREAM_STAGE_FAIL` before interpreting a request to remove
   or recompose that failed stage. Use `UPSTREAM_GRAPH_REVIEW_REQUIRED` when
   graph recomposition is the blocking pattern without an observed stage
   failure. A new fixture must have valid packet identity and non-empty PASS
   evidence for every stage declared `PASS`.

These rules must be written into the public protocol and candidate contract,
covered by focused regression, and then used to generate a new hidden key. The
old frozen files and their hashes must remain unchanged.

## Admission boundary

No v0.1.9 Round 1 identity is admitted by this review. Before the next
`112 + 16` execution, the operator must approve the four precedence rules,
the corrected RT30 fixture, and a new freeze/preflight identity. The fresh
identity must bind the installed candidate version, public protocol, artifact
contract, expected reasons, and all transitive hashes before `prepare`.

## Closure record

Status: **Blocked**  
Scope: read-only classification of the four public reason-contract/scorer conflicts.  
Acceptance criteria: each conflict identified with source evidence, no frozen-root mutation, and a concrete contract clarification — **met**; owner approval for a new contract/corpus identity — **not yet recorded**.  
Verification: public protocol, artifact contract, installed v0.1.9 policy, four attempt artifacts, independent validator output, and final scorer JSON were inspected.  
Evidence: the D-drive paths and hashes listed above.  
Boundary / next dependency: explicit operator approval is required before editing the public contract, candidate, or creating a fresh v0.1.9/vNext freeze; activation, runtime parity, qualification, release, deployment, and Original-repository work remain out of scope.
