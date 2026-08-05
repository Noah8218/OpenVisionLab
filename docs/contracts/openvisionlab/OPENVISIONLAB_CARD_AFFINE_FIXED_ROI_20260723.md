# Card Affine Fixed-ROI Linkage

Updated: 2026-07-23

## Purpose

Close the exact P220 dependency that the operator resolved by accepting the
observed coarse registration result:

```text
Matching R + Matching 5 + Matching expiry mark
-> typed Point x3
-> OpenVisionLab Vision SDK AffineTransform
-> fixed CardReference ROI
-> existing Mean measurement
```

This is a bounded coordinate-linkage proof. It does not retroactively make the
P220 `<=3 px` gate pass, classify the supplied OK/NG images, or qualify a
production inspection.

## Operator Decision And Frozen Boundary

The operator accepted the current card registration as adequate for this
coarse fixed ROI. P221 therefore uses a separate observed-envelope gate:

- minimum normalized locator-template score: `>=0.65`
- maximum normalized locator-center residual: `<=5 px`
- fixed downstream ROI: `CvROI=250,315,190,80`
- fixed downstream input/output: `CardReference -> CardDateMean`
- existing tool: `Mean`
- judgement: none

The fixed ROI covers the embossed `10/05` date area and is separate from the
three locator template ROIs. It has enough guard area for the accepted
five-pixel observed residual. This is an operator-approved engineering boundary
for the exact 12-row pilot, not a universal registration tolerance.

## Actual Result

The same six OK and six NG rows from P220 were replayed once from the current
source:

- `12/12` completed Matching x3, AffineTransform, and the fixed Mean Step.
- `12/12` retained the exact saved `CvROI=250,315,190,80` after XML round trip.
- `12/12` published finite `MeanValueAvg` values (`111.4..170.1`).
- normalized-template minimum score was `0.836786`.
- maximum center residual was `5.00 px`; the next highest was `4.12 px`.
- `12/12` runtime Mean drawings show the same reference-coordinate ROI over the
  intended date area.

The OK/NG folder role was retained only as an input stratum. No Mean threshold,
defect label, Good/NG tolerance, or acceptance gate was inferred.

## Evidence

- Final current-source evidence:
  `artifacts\p221_card_affine_fixed_roi_20260723_r2`
- Runtime ROI contact sheet:
  `p221_fixed_roi_contact_sheet.png`
- Exact per-row metrics:
  `p221_results.csv`
- Source paths and SHA-256:
  `p221_input_manifest.csv`
- Pipeline/XML:
  `p221_card_matching_x3_affine.pipeline.xml`
- Per-row source, three Matching drawings, Affine drawing, normalized image,
  independent point check, and runtime Mean ROI drawing:
  `runs`

Focused command:

```powershell
dotnet "tools\VisionRecipeRunnerSmoke\bin\Any CPU\Debug\net8.0-windows7.0\VisionRecipeRunnerSmoke.dll" --affine-card-fixed-roi "<card_original root>" "artifacts\p221_card_affine_fixed_roi_20260723_r2"
```

Status: Complete

Scope: user-accepted coarse card Affine registration linked to one unchanged
fixed reference-coordinate Mean ROI on the frozen 12-row pilot.

Acceptance criteria: all five Steps execute on 12/12 -> pass; XML retains exact
Mean input/ROI with no judgement -> pass; normalized score/residual meets the
separate `>=0.65` / `<=5 px` boundary -> pass; finite Mean metric and runtime
ROI drawing exist for every row -> pass.

Verification: focused smoke project built with zero warnings/errors; final r2
execution passed 12/12; the runtime contact sheet and maximum-residual row were
opened and reviewed.

Evidence: `artifacts\p221_card_affine_fixed_roi_20260723_r2`.

Boundary / next dependency: the operator must select the actual rule-based
inspection performed inside a reference ROI and define its physical Good/NG
tolerance. Do not infer it from the unjudged Mean values.
