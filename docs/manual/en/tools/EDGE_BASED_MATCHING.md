# Edge Based Matching: locate an outline Template

## Purpose

Use Edge Based Matching when object outlines are more stable than brightness.
Confirm the Template's physical identity; repeated outlines can be ambiguous.

## Steps

1. Select a Template ROI with a distinctive outline.
2. Open `Edge Based Matching`.
3. Confirm input/output Layers.
4. Register the Template.
5. Review Canny, minimum gradient, and maximum points under `Edge Model`.
6. Set Search ROI, minimum Score, and Count.
7. Run Preview at fixed pose first.
8. Enable angle/scale, coarse search, refine, or hybrid verify only when needed.
9. Select `Run Preview`.
10. Check that edge bounds and model points sit on the physical outline.
11. Review score, pose, ambiguity, and alternative results.
12. Verify fail-closed behavior on repeated structure and no-target samples.

Check Template ROI, Canny/gradient, Search ROI, Score/Count, repeated outlines,
pose, then refinement. Speed options are explicit choices, not accuracy defaults.
