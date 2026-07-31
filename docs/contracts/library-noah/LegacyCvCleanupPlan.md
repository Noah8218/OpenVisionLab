# OpenVisionLab Legacy CV Cleanup Plan

## Scope

Legacy classes still present in Library-Noah:

- `CVContour`
- `CVCorner`
- `CVLineGuage`
- `CVMatching`
- `CVMean`
- `CVSIFT`
- `COpenCVAlgorithmBase`
- `COpenCVHelper`

These classes are not removed in this pass. They may preserve old behavior, timing assumptions, or compatibility paths. Cleanup must happen only after the replacement Tool path has a passing contract.

## Replacement Coverage

Current replacement paths covered by smoke contracts:

- `ContourTool`
  - sample: `Contour_TextSymbols`
  - synthetic: rectangle geometry, square contract
- `LineGaugeTool`
  - sample: `Pins_LineGauge`
  - synthetic: direction, polarity, ROI geometry
- `MatchingTool`
  - sample: `Contour_TemplateMatching`
  - synthetic: preprocessing, multi ROI, angle/scale, rotated fixture
- `MeanTool`
  - pipeline validation and metric contract coverage exists
- `SiftTool`
  - synthetic: positive feature match, no false positive, no-detection message

## Cleanup Rules

1. Do not remove a legacy class until its replacement has sample coverage and synthetic edge-case coverage.
2. Do not rename public compatibility classes in the same step as behavior changes.
3. Preserve CVBlob DLL usage. Do not upgrade or replace the external blob DLL.
4. For each removal candidate, first search src/OpenVisionLab/UI/forms and recipe code for direct usage.
5. If a class is still referenced by a form, migrate that form to the Tool path first.
6. If old behavior is faster, keep the behavior and only isolate naming/adapter compatibility.

## Recommended Order

1. `CVMean`
   - Smallest surface area.
   - Replace only after Mean sample/contract is explicit enough.
2. `CVContour`
   - Replacement coverage is already broad, but old ROI/multi ROI behavior must be compared.
3. `CVMatching`
   - Keep until rotated fixture and template path behavior stay stable.
4. `CVSIFT`
   - Keep until failure-reason and low-feature behavior is stable.
5. `CVLineGuage`
   - Last candidate because direction/polarity/edge-thickness behavior is easy to regress.
6. `CVCorner`
   - Decide whether it becomes `CornerTool` coverage or remains unsupported legacy.

## Current Decision

The current development pass documents the cleanup boundary only. No legacy class is deleted yet.
