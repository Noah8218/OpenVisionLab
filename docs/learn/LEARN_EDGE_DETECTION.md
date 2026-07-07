# Edge Detection Review

Updated: 2026-07-07

Edge detection turns brightness changes into edge pixels. It is usually a preparation step, not the final inspection decision.

OpenVisionLab has an `EdgeDetection` tool view. Use it when the next step needs clearer boundaries for Contour, LineGauge, LineDistance, or EdgeBasedMatching-style reasoning.

## Core Concepts

| Concept | Meaning in OpenCvSharp/OpenVisionLab |
| --- | --- |
| Gradient | How quickly GV changes from one pixel to the next |
| Canny | Common edge detector with threshold-style control |
| Sobel/Scharr | Directional gradient operators for edge strength |
| Laplacian | Highlights rapid intensity changes in multiple directions |
| Edge map | Output layer where strong boundaries become visible pixels |

## EdgeDetection Is Not Line Measurement

| Question | Prefer |
| --- | --- |
| Where are boundary pixels? | `EdgeDetection` |
| Is this shape/fiducial present by edge geometry? | `EdgeBasedMatching` |
| What is the fitted line angle or rail line? | `LineGauge` |
| What is the gap, pitch, width, or clearance between two edges? | `LineDistance` |
| How many defect regions exist after edge cleanup? | `EdgeDetection -> Morphology -> Contour` |

## Public Practice Samples

| Role | SampleName | What to verify |
| --- | --- | --- |
| Good | `Public_EdgeDetection_Shapes_Good` | `ResultCount=4` after `EdgeDetection -> Morphology -> Contour` |
| Bad | `Public_EdgeDetection_Shapes_Missing_Bad` | `ResultCount=2` with the same pipeline |

Use `Public_EdgeDetection_Shapes.pipeline.xml` to practice this topic. The EdgeDetection step creates the edge map, but the final pass/fail evidence comes from the downstream Contour metric.

## What To Check

1. Confirm the input layer is the original or filtered image intended for edge extraction.
2. Run Preview explicitly.
3. Check whether the output edge layer contains only useful boundaries, not the whole background texture.
4. If the next step is Contour, check `ResultCount` and area metrics after cleanup.
5. If the next step is LineGauge or LineDistance, check ROI, scan direction, polarity, and metric names.

## Common Failures

| Symptom | Likely Cause | First Fix |
| --- | --- | --- |
| Too many edges | Noise, low threshold, or texture background | Add Filter or tighten edge threshold |
| Important edge missing | Threshold too high or contrast too low | Lower threshold or adjust preprocessing |
| Contour selects the whole surface | Edge map is not cleaned before region extraction | Add Morphology or ROI |
| Distance line appears in the wrong place | EdgeDetection was used, but LineDistance ROI/direction is wrong | Fix LineDistance ROI and scan direction |

## Completion Standard

EdgeDetection is useful only when the next tool produces a measurable result. A recipe should end with a metric such as count, area, score, distance, angle, or output size.

Opening this guide or changing EdgeDetection parameters must not run Preview/Run automatically.

## Beginner path handoff

- Previous topic: Filter. Use EdgeDetection when boundary pixels, not filled regions, are the useful evidence.
- This topic goal: prove that Canny edge output can feed a downstream count or measurement step.
- Practice Samples path: `preprocess`.
- Public sample pair: `Public_EdgeDetection_Shapes_Good` / `Public_EdgeDetection_Shapes_Missing_Bad`.
- Explicit action: run EdgeDetection Preview manually, then run downstream Morphology/Contour manually and compare `ResultCount`.
- Next topic: use LineDistance when the question is distance, pitch, width, or clearance between edges.
