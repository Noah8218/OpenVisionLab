# Card Three-Point Affine Pilot

Updated: 2026-07-23

## Purpose

Qualify one operator-approved real-image candidate for the P219 deterministic
path:

```text
Matching R + Matching 5 + Matching expiry mark
-> typed Point x3
-> Library-Noah AffineTransform
-> fixed 640x480 card reference frame
```

This is a fixture-normalization pilot. It does not classify the supplied OK/NG
defects and does not qualify any downstream inspection ROI.

## Approved Physical Features

Reference image:

`card_original/images/OK/card_original_OK_0001.jpg`

| Point | Template ROI | Destination center |
| --- | --- | --- |
| A: `R` glyph | `100,38,68,126` | `134.0,101.0` |
| B: `5` glyph | `320,35,75,125` | `357.5,97.5` |
| C: expiry mark | `165,333,85,55` | `207.5,360.5` |

The destination triangle area is about `29,128 px²`. The three features are
visually distinct, non-collinear, and inside the physical card. The operator
approved this set before any Matching execution.

## Frozen Pilot

The 12 inputs were selected by fixed numeric spacing before outcomes were read:

- OK: `0026`, `0051`, `0101`, `0150`, `0200`, `0250`
- NG: `0026`, `0051`, `0101`, `0150`, `0200`, `0250`

Matching starter settings:

- `CCoeffNormed`
- score minimum `0.55`
- one result
- angle `-8..8°`, step `1°`
- scale `0.9..1.1`, step `0.05`
- one coarse search ROI per approved feature

The independent normalized-output check retained the original three templates
and fixed these gates before execution:

- minimum post-normalization template score `>= 0.65`
- maximum post-normalization center residual `<= 3 px`

The manifest records every source path and SHA-256. The saved Pipeline XML,
reference image, and templates are hash-retained under the evidence folder.

## Actual Result

The first frozen run passed `8/12`.

- Two rows stopped because the `5` moved outside the original coarse search ROI.
- `OK_0150` showed that the `R` search region also admitted the visually similar
  left `P`, producing a semantically wrong locator.
- No score, angle, scale, Affine, or residual gate was changed.

One bounded geometry-only correction excluded `P` from the `R` search region
and widened the `5` search region to include the observed physical feature.
The r2 run passed `10/12`:

- Ten rows retained maximum center residual `0..2 px`.
- `OK_0051` failed at `5.00 px`; its `R` match was visibly on the intended glyph
  but weaker and the normalized glyph correlation remained offset.
- `NG_0150` failed at `4.12 px`; the normalized image was visually coherent, but
  the frozen `3 px` gate remained exceeded.
- Runtime errors, missing sources, and missing drawings were not hidden.

Decision: `Incomplete at <= 3 px`.

Do not lower the gate, run all 500, add Homography, or replace the approved
features automatically. First obtain the downstream fixed ROI/inspection and
its required registration tolerance. If that inspection requires `<= 3 px`,
reject the current Matching-center fixture and select a more stable physical
Point-producing method. If it safely tolerates at least the observed `5 px`,
define that engineering margin before any larger replay.

Follow-up: P221 records the operator's later acceptance of the observed
`<=5 px` envelope for one coarse fixed date-area ROI. That separate completion
is documented in `OPENVISIONLAB_CARD_AFFINE_FIXED_ROI_20260723.md` and does not
change this P220 result.

## Evidence

- Candidate approval drawing:
  `artifacts\p220_affine_fixture_point_candidates_20260723\p220_card_affine_three_point_candidates.png`
- Immutable first run:
  `artifacts\p220_affine_fixture_point_candidates_20260723\pilot`
- Geometry-corrected r2:
  `artifacts\p220_affine_fixture_point_candidates_20260723\pilot_r2`
- r2 contact sheet:
  `pilot_r2\p220_normalized_recheck_contact_sheet.png`
- Per-row source, Matching drawings, Affine drawing, normalized image, and fixed
  point recheck:
  `pilot_r2\runs`
- Exact selected inputs and hashes:
  `pilot_r2\p220_input_manifest.csv`
- Exact metrics:
  `pilot_r2\p220_results.csv`
- Pipeline:
  `pilot_r2\p220_card_matching_x3_affine.pipeline.xml`

Focused command:

```powershell
dotnet "tools\VisionRecipeRunnerSmoke\bin\Any CPU\Debug\net8.0-windows7.0\VisionRecipeRunnerSmoke.dll" --affine-card-pilot "<card_original root>" "artifacts\p220_affine_fixture_point_candidates_20260723\pilot_r2"
```

Status: Incomplete

Scope: operator-approved real card Matching centers driving the existing P219
Affine path on a frozen 12-row pilot.

Acceptance criteria: three typed Points and Affine output are present on every
row -> pass; maximum normalized center residual `<= 3 px` on every row -> fail
at `5.00 px` and `4.12 px`; drawings and identities retained -> pass.

Verification: focused smoke project builds with zero warnings/errors; r1 and r2
executed from current source; all r2 current-run drawings and the contact sheet
were generated and reviewed.

Evidence: `artifacts\p220_affine_fixture_point_candidates_20260723`.

Boundary / next dependency: downstream reference-coordinate inspection ROI and
its allowable registration error.
