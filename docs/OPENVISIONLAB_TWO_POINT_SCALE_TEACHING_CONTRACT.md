# OpenVisionLab Two-Point Scale Teaching Contract

Status: Approved for bounded implementation (P214)

## Outcome

Teach one uniform image scale from two typed Point results produced by the same explicit Pipeline Review Run, preserve the exact source-image evidence, and apply the resulting millimeters-per-pixel value only to an operator-selected compatible measurement Step.

## Included Scope

1. The operator selects Point A and Point B from P213 typed geometry results.
2. Both points must come from the same coordinate layer and image dimensions in the same current Run.
3. The operator enters one known distance in `mm`, `µm`, or `inch`.
4. OpenVisionLab calculates:

   `millimeters per pixel = known distance in millimeters / point distance in pixels`

5. The calculation record stores both point identities and coordinates, pixel distance, entered value/unit, converted millimeters, derived `mm/px`, coordinate layer, image dimensions, source-image SHA-256, and creation time.
6. The record is saved separately from the pipeline before it can be applied.
7. Apply is an explicit action against exactly one selected compatible Step. Repeating Apply is required for another Step.
8. Apply writes the legacy `PIXELPERMM` parameter because existing runtime and XML depend on that key. The actual value semantics are **millimeters per pixel (`mm/px`)**. `LineDistance` also receives its existing `LeftPIXELPERMM` and `RightPIXELPERMM` values.

## Fail-Closed Rules

- Point A and Point B must be finite, distinct Point results.
- Coordinate layer, image width, and image height must match.
- The current coordinate-layer image must exist and match those dimensions.
- Known distance and derived scale must be finite and greater than zero.
- Apply must reject a changed source-image SHA-256, changed coordinate layer/dimensions, incompatible target tool, or target Step whose input layer differs from the calibration coordinate layer.
- Calculate, save, and apply must not invoke Preview or Run and must not change layer selection, routing, or output-layer creation.

## Compatible Step Families

- `Line` / `LineGauge`
- `LineDistance` / `LineDistanceGauge`
- `PinArrayGap` / `AdjacentPinGap`
- `GapEdgePair`
- `CurveBandProfile` / `DarkBandCurve`
- `CircleGauge`
- `GeometryMeasure` / `GeometricMeasurement`

## Acceptance Checklist

- [ ] A 3-4-5 pixel pair taught as 10 mm produces exactly 2 mm/px within numeric tolerance.
- [ ] Equivalent `10 mm`, `10000 µm`, and `0.3937007874 inch` inputs produce the same scale within tolerance.
- [ ] Same-point, cross-layer, dimension-mismatch, missing-image, zero/negative-distance, changed-image, and incompatible-target cases fail closed.
- [ ] The saved record round-trips with exact identities, coordinates, source hash, and scale.
- [ ] Explicit Apply changes only the selected compatible Step and pipeline persistence round-trips.
- [ ] Existing pipelines that already contain `PIXELPERMM` continue to load and execute unchanged.
- [ ] Calibrated P213 distance/radius metrics are published in millimeters only when the positive legacy scale parameter exists.
- [ ] Current-build UI evidence shows Point A/B, their connecting drawing, pixel distance, entered real distance, derived mm/px, source hash, and selected target Step.

## Excluded Scope

- Camera/lens calibration, distortion correction, perspective calibration, non-uniform X/Y scale, world/robot coordinates, or calibration-board detection.
- Automatic point detection or automatic target-Step selection.
- Per-image scale adaptation, LLM inference, dataset campaigns, acceptance tolerance invention, or industrial calibration certification.
- Renaming or migrating the legacy `PIXELPERMM` XML key.

## Operator Example

After an explicit Run, select `DatumA/End` and `DatumB/End`. If their drawing distance is `125 px` and the certified physical distance is `5 mm`, save evidence at `0.04 mm/px`. Then choose `PinPitch` and press **Apply to selected Step**. Run Review again explicitly to obtain calibrated metrics; Apply itself does not execute the pipeline.
