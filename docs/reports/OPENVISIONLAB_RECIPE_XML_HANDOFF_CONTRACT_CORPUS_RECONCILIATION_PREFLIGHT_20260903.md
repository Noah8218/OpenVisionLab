# OpenVisionLab Recipe XML Handoff — Contract/Corpus Reconciliation Preflight

Date: 2026-09-03 KST  
Repository: `C:\Git\2D\Dev`  
Parent evidence: `OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_TRIAGE_20260903.md`  
Status: **Complete for read-only reconciliation preflight; follow-up reason projection and strict-baseline identity are recorded separately**

## Scope and safety boundary

This preflight rechecks the triage claims against the frozen v0.1.6 public
cases, hidden expected answers, retained author attempts, the installed
candidate contract, the public Tool Catalog, and the product source contract.
It does not rewrite the frozen v0.1.6 root, change the hidden key or reason
vocabulary, change the installed candidate, create a benchmark identity, run
Authors/Reviewers, execute the product, or touch the Original repository.

Follow-up state: the generic public reason projection is recorded in
`OPENVISIONLAB_RECIPE_XML_HANDOFF_REASON_PROJECTION_DECISION_20260903.md`, and
the strict-baseline identity created from this preflight is recorded in
`OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_STRICT_BASELINE_PREFLIGHT_20260903.md`.

Machine-readable evidence is retained at:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-contract-reconciliation-preflight-20260903\contract-reconciliation-preflight.json`

Length: `3,610` bytes  
SHA-256: `6D25581B2028B988538CAFC507CC11A47C3A4A762B301BE1A056B074F3E1E7EE`

## Reconciliation results

| Area | Direct evidence | Decision boundary |
| --- | --- | --- |
| `S3-02` vertical projection | Public case has `verticalProjection=R_TO_L`; the installed handoff contract maps `R_TO_L -> X_RTOL`; the representative `S3-02-01` XML contains `VER_PRJ_DIR=X_RTOL`, handoff/static checks pass, and the product property is the general `FormulaUtil.PROJECTION_DIR`. | The earlier triage statement that this is a proven `VER_PRJ_DIR` axis conflict is **not reproduced** by the retained mechanical evidence. Do not rewrite `S3-02` on that claim; a separate geometric/semantic review may still be performed. |
| `S4-01` NormalizeImage ratio | The public Tool Catalog requires `FIXTURE_MIN_VALID_PIXEL_RATIO`; the hidden expected policy is `0.25`; representative `S4-01-01` emits `0.25` and both handoff/static checks pass. | The earlier statement that the case lacks the required ratio is **not a confirmed blocker**. The catalog is an allowed public authority and supplies the parameter. No corpus repair is justified for this item. |
| Legacy safe-static baseline | Retained scorer data shows independent handoff-validator failures for `RT13`, `RT24`, `RT25`, `RT26`, and `RT28`: legacy string evidence entries and `PENDING` stages do not satisfy the current exact `{path, sha256}` and `PENDING/UNKNOWN` identity rules. | Confirmed corpus/compatibility defect. Preserve the old root and prepare a strict local baseline bundle in a new corpus identity, or explicitly approve a compatibility contract. The safer default is a new strict bundle aligned with the installed validator. |
| Public reason vocabulary | Retained scorer data shows three exact hidden-key mismatches: `RT22` expects `AUTO_REJECTED_PER_IMAGE_OVERRIDE` vs `PER_IMAGE_OVERRIDE_FORBIDDEN`; `RT31` expects `AUTO_REJECTED_AMBIGUOUS` vs `UPSTREAM_GRAPH_REVIEW_REQUIRED`; `RT32` expects `AUTO_REJECTED_REFERENCE_DRIFT` vs `PER_IMAGE_OVERRIDE_FORBIDDEN`. The handoff candidate explicitly forbids these historical aliases, while the Matching packet contract still uses them as specialist states. | Confirmed contract-owner decision point. Do not silently broaden or shrink the public vocabulary. Choose whether handoff `reasonCode` stays generic with a separate specialist-status projection, or whether specialist aliases are formally admitted and the candidate/contract/tests are updated together. |

## Recommended next decision

1. Retain the v0.1.7 generic handoff vocabulary as the default because it is
   already enforced by the installed candidate and its focused regression, but
   explicitly define how a reviewed Matching specialist state is projected
   without losing its exact source status. This requires the contract owner to
   approve either a namespaced specialist-status field or a formally admitted
   alias subset; no case-ID mapping is acceptable.
2. Create a new corpus-only identity from the immutable source with a strict
   local safe-static baseline bundle and regenerated transitive hashes. Do not
   modify the v0.1.6 root or the installed candidate during that operation.
3. Re-run the focused validator/scorer probes against the new copy, then seek a
   separate admission decision before any `112 + 16` execution.

The `S3-02` and `S4-01` findings should remain as semantic-review notes, not
automatic corpus edits. The runner/scorer correction is already available in
the separate isolated fixture and must be used for any future benchmark setup.

## Verification

- Frozen v0.1.6 score data was read for representative S3/S4 attempts and the
  RT03/RT13/RT22/RT24–RT26/RT28/RT31/RT32 signals.
- Public S3/S4 case JSON, hidden expected answer entries, public Tool Catalog,
  installed handoff skill mapping, `LineGaugeProperty.VER_PRJ_DIR`, and the
  pipeline parameter schema were inspected.
- `contract-reconciliation-preflight.json` records the exact values and the
  four conclusions above; its length and SHA-256 are fixed above.

## Closure record

Status: **Complete**  
Scope: read-only reclassification of the proposed S3-02/S4-01 conflicts and
confirmation of the legacy-baseline and reason-vocabulary blockers.  
Acceptance criteria: every triage claim is either reproduced with direct
evidence or marked as unconfirmed; no frozen source is rewritten — **met**.  
Verification: retained score/attempt artifacts, public/private case contracts,
installed candidate rules, public catalog, product parameter source, and the
machine-readable preflight evidence were inspected.  
Evidence: the JSON artifact above and this report.  
Boundary / next dependency: the follow-up projection and strict-baseline
identity are complete; a separate benchmark admission is still required before
Authors/Reviewers execution. Candidate activation, product execution,
qualification, release, deployment, and Original-repository work remain
unapproved.
