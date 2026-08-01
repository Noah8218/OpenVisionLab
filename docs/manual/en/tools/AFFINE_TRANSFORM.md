# Affine Transform: normalize coordinates from three points

## Purpose

Use three non-collinear physical point correspondences to create a 2D transform
that can include rotation, translation, scale, and shear. The operator owns the
physical identity and ordering of the points.

## Steps

1. Select three stable, separated Source points.
2. Select the corresponding Destination points in the same order.
3. Open `Affine Transform`.
4. Set input/output Layers and output size.
5. Enter the three Source/Destination pairs.
6. Set triangle-area and minimum-valid-pixel gates.
7. Enable detected Point references only when needed.
8. Select `Run Preview`.
9. Inspect source/destination triangles, transformed frame, and 2x3 matrix.
10. Check the valid-pixel ratio and clipping.
11. Teach downstream fixed ROIs in the normalized output.
12. Save locators and Affine so they execute in the same Run.

Check point order, duplicate or collinear points, coordinate range, output size,
valid pixels, then locator success/ambiguity. This is not Homography or camera
calibration.
