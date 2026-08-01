# Threshold: separate bright and dark regions

## Purpose

Use Threshold to separate target and background brightness into a binary image.
The usual input is `Main` and output is `Threshold_Preview`. Consider HSV first
when color, rather than brightness, defines the target.

## Steps

1. Decide whether the target or background should become white.
2. Open `Threshold`.
3. Confirm input `Main` and output `Threshold_Preview`.
4. Start with the `Basic` preset.
5. Select `Binary` or `BinaryInv` polarity.
6. Set the threshold and maximum values.
7. Set an ROI when only part of the image matters.
8. Select `Run Preview`.
9. Check that the white target is continuous and background noise is limited.
10. If correct, select `Add and save to Pipeline`.

## Check the result

Review binary polarity, holes or breaks inside the target, white background
noise, ROI placement, and the output Layer.

## Failure order

Check input Layer, binary polarity, threshold, ROI, then adaptive options. Use
adaptive Threshold only when lighting variation requires it. Replay the same
values on Bad and additional Good samples.
