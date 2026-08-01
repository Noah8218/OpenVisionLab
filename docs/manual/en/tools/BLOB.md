# Blob: count and size connected regions

## Purpose

Use Blob to count thresholded connected regions and filter them by area, center,
and axis-aligned bounds. Use Contour when the outer shape itself matters.

## Steps

1. Define objects to count and objects to reject.
2. Open `Blob`.
3. Confirm the input Layer and `Blob_Preview`.
4. Choose internal Threshold or a previous Threshold Layer.
5. Set threshold polarity and value.
6. Set the ROI.
7. Set Min/Max Area.
8. Set Width/Height ranges when needed.
9. Select `Run Preview`.
10. Review accepted/rejected object boxes, centers, and reasons.
11. Confirm that ResultCount, Area, and Bounds metrics match the drawings.
12. Save, then run Good and Bad through the same Pipeline.

Check input/polarity, ROI, Morphology, Area, Width/Height, then connectivity.
A correct count of the wrong objects is not a valid result.
