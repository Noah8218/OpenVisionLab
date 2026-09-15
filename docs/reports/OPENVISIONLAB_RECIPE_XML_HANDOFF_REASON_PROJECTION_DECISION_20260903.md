# OpenVisionLab Recipe XML Handoff Reason Projection Decision

Date: 2026-09-03 KST  
Repository: `C:\Git\2D\Dev`  
Status: **Complete for the new corpus input contract; candidate activation and benchmark admission remain separate decisions**

## Scope

The v0.1.6 reconciliation preflight confirmed three hidden-key values that
still use historical Matching-specialist aliases while the installed
`openvisionlab-recipe-xml-handoff 0.1.7` contract rejects those aliases. The
operator's continuation approval is applied here to the recommended
generic-public-vocabulary direction for a new, separately identified corpus.
This record does not change the installed candidate, the v0.1.6 frozen root,
the product, the Original repository, or any benchmark result.

## Decision

The public `openvisionlab-recipe-xml-handoff-v1` decision-evidence field keeps
the current generic vocabulary. Historical `AUTO_REJECTED_*` values are not
emitted, accepted as v1 aliases, or used as a case-ID lookup table.

| Request/evidence pattern | Public handoff `reasonCode` |
| --- | --- |
| Per-image template, ROI, angle, threshold, or acceptance substitution | `PER_IMAGE_OVERRIDE_FORBIDDEN` |
| Missing retained competitor or an automatic candidate/graph composition that requires review | `UPSTREAM_GRAPH_REVIEW_REQUIRED` |
| Direct layer or routing mutation | `LAYER_MUTATION_NOT_AUTHORIZED` |
| Other patterns | The most specific existing generic code in the supplied request/evidence contract |

The three historical values are therefore projected by pattern in the new
corpus as follows:

| Historical specialist value | New public expectation | Projection basis |
| --- | --- | --- |
| `AUTO_REJECTED_PER_IMAGE_OVERRIDE` | `PER_IMAGE_OVERRIDE_FORBIDDEN` | per-image selection/override request |
| `AUTO_REJECTED_AMBIGUOUS` | `UPSTREAM_GRAPH_REVIEW_REQUIRED` | missing retained competitor and automatic selection request |
| `AUTO_REJECTED_REFERENCE_DRIFT` | `PER_IMAGE_OVERRIDE_FORBIDDEN` | locked source/ROI replacement request |

This is a contract projection, not a new reason field. Specialist packets may
retain their own internal diagnostic status, but the v1 handoff exposes only
the generic public code. Introducing a namespaced specialist field or a formal
alias subset would require a separately versioned schema and focused
regression; it is not silently added here.

## Corpus consequences

The new corpus identity must:

1. remove the three historical aliases from the public artifact-contract
   vocabulary;
2. update the private red-team expected reasons to the three generic values
   above;
3. retain every red-team safe policy, status set, prohibited-success list,
   and case request unchanged;
4. use a strict local safe-static baseline whose evidence entries are exact
   `{path, sha256}` pairs and whose `PENDING` packets use
   `packetSchemaId=PENDING`, `packetPath=PENDING`, and
   `packetSha256=UNKNOWN`; and
5. use the corrected runner/scorer fixture with the assigned attempt-root
   `cwd`/scan-root and scoped intentional hash-mismatch handling.

No benchmark execution is implied by preparing this identity. A future
`112 + 16` run must be admitted separately after the new freeze record and all
preflight checks pass.

## Acceptance and evidence

Acceptance criteria for this decision are met when the new identity's public
contract contains only the installed candidate's public reason vocabulary,
the three expected reasons match the pattern table, and the unchanged
candidate validator accepts representative preserved-baseline and blocked
handoffs. Evidence is recorded in the new corpus freeze/preflight report and
its D-drive machine-readable artifacts.

Status: **Complete**  
Scope: generic public reason projection for a new corpus input contract.  
Acceptance criteria: no public `AUTO_REJECTED_*` emission; pattern-based
generic mapping; no candidate/frozen-root/product mutation — **met for the
decision record**.  
Verification: prior reconciliation evidence, installed v0.1.7 public reason
set, and the focused candidate regression were inspected; new-identity
preflight is a subsequent checkpoint.  
Evidence: this report and the new corpus's `reason-projection.json`.  
Boundary / next dependency: strict-baseline corpus creation and preflight are
now complete in the linked v0.1.7 report; a separate benchmark admission is
required before any `112 + 16` execution. Candidate activation, product
execution, qualification, release, deployment, and Original-repository work
remain unapproved.
