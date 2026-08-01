# Feature Matching: locate from feature geometry

## Purpose

Use Feature Matching for a textured target with distinctive grayscale features,
using Ratio and RANSAC geometry. It may not suit flat or repeated patterns.

## Steps

1. Select a Template with distinguishable texture.
2. Open `Feature Matching`.
3. Confirm input/output Layers.
4. Register the feature Template.
5. Start with default detector/descriptor settings.
6. Review Ratio and RANSAC tolerance.
7. Set Search ROI when needed.
8. Select `Run Preview`.
9. Check that feature links and inliers connect the same physical parts.
10. Check that result geometry is not distorted or mirrored.
11. Review score/inlier results and reject reason.
12. Replay on blur, lighting, and no-target samples before saving.

Check Template texture, input blur, ROI, Ratio, RANSAC, then repeated patterns.
Many lines with physically wrong geometry still mean failure.
