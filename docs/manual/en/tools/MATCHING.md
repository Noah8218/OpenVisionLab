# Matching: locate a grayscale Template

## Purpose

Use Matching to locate one object with a reasonably stable brightness pattern
and report score, angle, and scale. Repeated patterns require tighter search ROI
and unique-result review.

## Steps

1. Select a Template region that preserves one physical identity.
2. Open `Matching`.
3. Confirm the input and `Matching_Preview`.
4. Register the Template ROI and confirm `Template Ready`.
5. Set the Search ROI.
6. Set minimum Score and required Count.
7. Start with angle/scale search disabled at fixed pose.
8. Enable narrow angle/scale ranges only for observed variation.
9. Select `Run Preview`.
10. Check that bounds and center sit on the physical Template object.
11. Review `ScoreMax`, Count, angle, scale, and alternative margin.
12. Verify that wrong repeated patterns do not pass on Bad evidence.

Check Template file/ROI, input, Search ROI, Score, Count, contrast, angle, then
scale. A high score does not prove physical feature identity.
