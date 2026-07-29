# OpenVisionLab Validation Variant v1 Contract

Date: 2026-07-29  
Queue item: CVR-19  
Status: Complete

## Purpose

One recipe and one immutable Pipeline may validate several explicitly named
product styles without changing Pipeline parameters between image rows.

## Operator workflow

1. Select a Local Validation Set image.
2. Review or enter `Variant`, `Expected metric`, `Min`, and `Max` in the one
   Variant setup row.
3. Select `Apply selected` to persist the contract on that image.
4. Use `Reset` to restore the legacy Default Variant with no metric gate.
5. Explicitly run the Validation Set when ready.

Selecting, editing, applying, and resetting this setup must not Preview, Run,
create/delete/select a layer, or change Pipeline routing.

## Persisted data

Each `OpenVisionRecipeValidationSetImage` owns:

- `VariantId`; blank legacy data is displayed and compared as `Default`;
- `ExpectedMetricName`;
- `ExpectedMetricMinimum`;
- `ExpectedMetricMaximum`.

The image-set SHA-256 includes the ordered image path/hash and all four Variant
contract fields. The same fields are retained by the batch result, TSV, Run
History, deterministic review queue, Qualified Snapshot validation-set source,
and Qualified Snapshot evidence row.

## Validation

- Variant ID: at most 80 characters and no control characters.
- Metric name: at most 100 characters and no control characters.
- A metric name requires at least one finite numeric bound.
- A bound requires a metric name.
- When both bounds exist, minimum must not exceed maximum.
- Invalid values fail before save or execution.
- Missing attributes remain backward-compatible Default/no-gate behavior.
- Qualified Snapshot schema v2 binds Variant fields into its idempotency
  identity while verification still accepts schema v1 with its original
  canonical identity. Legacy review-queue v2 strata can still be rebuilt.

## Execution and comparison

- The existing deterministic Pipeline executes unchanged for every row.
- The existing sample-check metric evaluator applies the row's expected metric
  range after execution.
- The raw Pipeline outcome remains the actual outcome; the metric/role contract
  contributes to judgment correctness.
- Review-queue hash audit strata are `Variant + expected role`.
- Run comparison requires the same suite, source identity, expected outcome,
  Variant ID, metric name, and bounds. An incompatible contract is not merged.
- Qualified Snapshot preflight rebuilds the image-set hash and review queue and
  rejects a row whose Variant contract differs from the selected set.

## Approved v1 evidence

Both variants retain the existing Pipeline
`docs\samples\public\product\Product_Field_DarkFeature_Contour.pipeline.xml`.

| Variant | Metric | Range |
| --- | --- | --- |
| `Product_Field_FilmStripe_SurfaceReview` | `ResultCount` | `3..8` |
| `Product_Field_TexturedRoller_SurfaceReview` | `ResultCount` | `1..4` |

The current-source smoke loaded these exact catalog rows and replayed both
through the unchanged deterministic Pipeline and catalog gates successfully.

## Boundary

v1 supports one expected metric range per validation image. It does not mutate
Pipeline parameters by Variant, add accounts/electronic signatures/regulatory
claims, qualify production robustness, or turn an Explore sample into certified
inspection evidence.
