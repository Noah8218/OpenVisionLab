# Mean: decide from average brightness

## Purpose

Use Mean to compare average brightness in one ROI for simple bright/dark,
missing-part, or contamination checks.

## Steps

1. Define the physical region to measure.
2. Open `Mean`.
3. Confirm input and result Layers.
4. Select Mean mode and channel.
5. Place the ROI inside the target.
6. Record `MeanValueAvg` and its range from several Good images.
7. Set the normal minimum and maximum.
8. Select `Run Preview`.
9. Review the ROI drawing and mean/range metrics.
10. Confirm that Bad evidence leaves the same range before saving.

Check input Layer, ROI, channel/Mean mode, lighting variation, then acceptance
range. An ROI that includes background or reflection may not represent the
inspection intent.
