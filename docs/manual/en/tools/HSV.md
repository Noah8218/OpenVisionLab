# HSV: separate a color range

## Purpose

Use Hue, Saturation, and Value ranges when brightness alone cannot separate the
target color. Use multiple samples because lighting changes can move the range.

## Steps

1. Define colors to include and exclude.
2. Open `HSV`.
3. Confirm the input and `HSV_Preview` output Layer.
4. Start with a narrow Hue range.
5. Set Saturation and Value minimum/maximum values.
6. Set ROI and mask options when needed.
7. Select `Run Preview`.
8. Check that the mask covers the physical color region.
9. Review selected-pixel ratio and color distribution.
10. Replay the same range on Good, Bad, and lighting-variation samples.

Check input color channels, Hue wrap, Saturation, Value, ROI, then lighting.
Hue may be unstable in gray or low-saturation regions.
