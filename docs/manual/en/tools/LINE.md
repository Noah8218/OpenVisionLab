# Line: edge, distance, and intersection

## Select Purpose first

- Edge: fit and inspect one line in one ROI.
- Measure: measure the distance between Line A and Line B.
- Intersection: find the intersection of fitted Line A and Line B.

## Steps

1. Define the physical boundaries and directions.
2. Open `Line`.
3. Select `Purpose`.
4. Confirm input/output Layers.
5. Select Line A and set ROI, scan direction, polarity, and contrast.
6. For Measure or Intersection, configure Line B independently.
7. Set scan interval and scan angle when required.
8. Apply a verified `mm per pixel` value only when physical units are needed.
9. Select `Run Preview`.
10. Check that edge points and fitted lines sit on physical boundaries.
11. For Measure review `DistancePxAvg/Range`; for Intersection review the point.
12. Save only when varied direction/polarity samples select the same boundaries.

Check Purpose, A/B ROI, scan direction, polarity, contrast, interval, angle, then
scale. A correct average distance on the wrong structure is invalid.
