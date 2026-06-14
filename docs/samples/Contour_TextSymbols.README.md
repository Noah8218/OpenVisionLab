# Contour_TextSymbols Sample

This sample is a reference pipeline for detecting text, numbers, and simple symbols in `Sample/Contour.jpg`.

## Pipeline

1. `01 Text Symbol Binary`
   - Tool: `Threshold`
   - Input: `Main`
   - Output: `TextSymbol_Binary`
   - Purpose: invert bright background into a binary candidate mask.

2. `02 Text Symbol Clean`
   - Tool: `Morphology`
   - Input: `TextSymbol_Binary`
   - Output: `TextSymbol_Clean`
   - Purpose: remove small noise before contour extraction.

3. `03 Text Symbol Contour`
   - Tool: `Contour`
   - Input: `TextSymbol_Clean`
   - Output: `TextSymbol_Contour`
   - Purpose: detect glyph and symbol candidate boxes from the cleaned mask.

## Acceptance

The contour step uses:

- `AcceptanceMetricName`: `ResultCount`
- `AcceptanceMetricMinimum`: `35`
- `AcceptanceMetricMaximum`: `80`
- `MaxElapsedMilliseconds`: `1000`

This range is intentionally loose. It is meant to catch a broken recipe while still allowing minor OpenCV/platform differences.

## Review

After running the sample, open the pipeline preview viewer for `03 Text Symbol Contour`.
Use the overlay list, mouse wheel zoom, drag pan, and Up/Down keys to review individual boxes.

The saved result image is the contour step image. Detection boxes are reviewed through the OpenVisionLab preview overlay, not burned into the sample PNG.

The sample should produce visible preview overlays and a non-zero `ResultCount` for the text/symbol regions.
