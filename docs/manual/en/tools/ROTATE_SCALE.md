# Rotate / Scale: normalize angle and size

## Purpose

Use known angle and scale values to transform an image or establish reference
coordinates for later Steps. This is not automatic localization or calibration.

## Steps

1. Define the rotation center and output size.
2. Open `Rotate / Scale`.
3. Select the input and a separate output Layer.
4. Set Angle.
5. Set X/Y scale.
6. Check output size and border fill.
7. Select `Run Preview`.
8. Check for clipped targets or empty borders inside the inspection ROI.
9. Confirm that downstream ROIs sit on the transformed targets.
10. Save only when a fixed transform is sufficient.

Check angle sign, center, X/Y scale, output size, border, then downstream ROI.
If position varies per image, review whether a qualified locator/fixture is
required instead of retuning a fixed transform.
