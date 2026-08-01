# Histogram: inspect brightness distribution and contrast

## Purpose

Use Histogram to compare brightness distribution, mean, and contrast in an
image or ROI and to review equalization effects. Use Mean for a single average
acceptance value.

## Steps

1. Select the image and ROI.
2. Open `Histogram`.
3. Confirm the input and separate output Layer.
4. Select the channel or gray conversion.
5. Apply equalization, CLAHE, or normalize options one at a time.
6. Select `Run Preview`.
7. Compare original and result mean and contrast.
8. Check for dark or bright saturation.
9. Compare downstream Threshold or Matching results.
10. Save only when the effect persists across repeated samples.

Check input channel, ROI, processing mode, then clip/grid/normalize options. Do
not use a large distribution change if it removes the target boundary or feature.
