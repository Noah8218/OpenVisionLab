# Contour: inspect outlines and shape candidates

## Purpose

Use Contour to find binary outlines and compare shape data such as area, bounds,
and angle. Blob is more direct for simple connected-region counts.

## Steps

1. Define the outer boundary and whether internal holes matter.
2. Open `Contour`.
3. Confirm input/output Layers.
4. Set threshold polarity and value.
5. Select Retrieval and Approximation modes.
6. Set ROI and Min/Max Area.
7. Set Width/Height ranges when needed.
8. Set contour drawing mode, color, and thickness.
9. Select `Run Preview`.
10. Check that drawings follow the physical outline.
11. Review accepted/rejected rows and `ResultCount`.
12. Save when the same shape meaning holds for Good and Bad.

Check binary polarity, ROI, Retrieval, area/size, weak boundaries, then drawing
mode. Very small pixel noise may be omitted from object rows.
