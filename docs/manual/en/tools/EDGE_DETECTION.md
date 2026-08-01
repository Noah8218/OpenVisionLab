# Edge Detection: find boundaries

## Purpose

Use Edge Detection to create an image of strong brightness boundaries or to
prepare a Line/shape inspection. Consider Filter first for textured images.

## Steps

1. Define the physical boundary and direction to find.
2. Open `Edge Detection`.
3. Confirm the input and `Edge_Preview` output Layer.
4. Select Canny, Sobel, Scharr, or Laplacian.
5. Set low/high thresholds or Kernel values.
6. Set an ROI that excludes distracting regions.
7. Select `Run Preview`.
8. Check that edges follow the physical boundary.
9. Check for excessive texture and noise edges.
10. Confirm that the downstream Tool uses the same boundary before saving.

Check input contrast, ROI, detector, thresholds, then Filter. Many edge pixels
alone do not prove a correct result.
