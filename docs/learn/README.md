# OpenVisionLab Learn Mode

Updated: 2026-07-07

Learn Mode is for learning rule-based vision inspection with public sample assets. The important flow is:

`image -> layer -> tool -> metric -> OK/NG reason`

Open the Good sample first, then open the Bad sample from the same PairGroup and run the same baseline pipeline. A sample is useful only when the result can be explained by image, overlay, metric, and log together.

## Curriculum First

Start with [OpenVisionLab Learn Curriculum](OPENVISIONLAB_LEARN_CURRICULUM.md) when you want a chapter-style learning path. Use [Learn OpenCvSharp Foundations](LEARN_OPENCVSHARP_FOUNDATIONS.md) first when `Point`, `Rect`, `Mat`, ROI, layer routing, or pixel/GV terms are unclear. The guides stay separate from the Tool Views and map machine-vision chapters 5-14 to actual OpenVisionLab tools or explicit tool gaps.

## Learn Window Topic Map

The Learn window exposes the topics below. Detail documents in the Tool Guides table are reference pages unless they are listed here as a Learn window topic.

| Topic | Learn window topic | Topic Docs file | Practice Samples path |
| --- | --- | --- | --- |
| 0 | Curriculum / image basics | `OPENVISIONLAB_LEARN_CURRICULUM.md` plus `LEARN_OPENCVSHARP_FOUNDATIONS.md` | `all` |
| 1 | Brightness / Histogram | `LEARN_MEAN.md` | `mean` |
| 2 | Threshold | `LEARN_THRESHOLD.md` | `preprocess` |
| 3 | Filtering | `LEARN_FILTER.md` | `preprocess` |
| 4 | Morphology | `LEARN_MORPHOLOGY.md` | `preprocess` |
| 5 | Blob | `LEARN_BLOB.md` | `blob` |
| 6 | Contour | `LEARN_CONTOUR.md` | `contour` |
| 7 | Edge / Line | `LEARN_EDGE_DETECTION.md` | `preprocess` |
| 8 | LineDistance | `LEARN_LINE.md` | `line` |
| 9 | Matching | `LEARN_MATCHING.md` | `template-matching` |
| 10 | Feature Matching | `LEARN_FEATURE_MATCHING.md` | `feature-matching` |
| 11 | Layer / Pipeline / Recipe | `LEARN_PIPELINE_LAYER_ROUTING.md` | `all` |
| 12 | EdgeBasedMatching | `LEARN_EDGE_BASED_MATCHING.md` | `edge-matching` |
| 13 | Metrics / Acceptance | `LEARN_METRICS_ACCEPTANCE.md` | `all` |
| 14 | Arithmetic / Logic | `LEARN_ARITHMETIC.md` | `preprocess` |
| 15 | Geometry Transform | `LEARN_GEOMETRY_TRANSFORM.md` | `geometry` |
| 16 | Color / HSV | `LEARN_COLOR_HSV.md` | `mean` |

Opening Topic Docs or Practice Samples must not run Preview/Run. Practice Samples only opens the sample picker on the selected Learn path.

Color / HSV currently uses the `mean` Practice Samples path as a temporary brightness bridge. Treat those Mean samples as brightness/channel drift evidence, not as HSV color-classification evidence.

## Tool Guides

| Order | Guide | What to learn |
| --- | --- | --- |
| 0 | [OpenVisionLab Learn Curriculum](OPENVISIONLAB_LEARN_CURRICULUM.md) | Chapter map for the separate Learn surface |
| 1 | [Learn OpenCvSharp Foundations](LEARN_OPENCVSHARP_FOUNDATIONS.md) | Pixel/GV, coordinates, `Point`, `Size`, `Rect`, `RotatedRect`, `Scalar`, `Mat`, ROI, layers |
| 2 | [Product Sample Guide](LEARN_PRODUCT_SAMPLES.md) | Battery, display, and semiconductor Good/Bad pairs |
| 3 | [Learn Matching](LEARN_MATCHING.md) | Template target presence, `ScoreMax`, `ResultCount` |
| 4 | [Learn Blob](LEARN_BLOB.md) | Particle/stain count and area checks |
| 5 | [Learn Contour](LEARN_CONTOUR.md) | Shape/region count and area checks |
| 6 | [Learn Threshold](LEARN_THRESHOLD.md) | Bright/dark region separation before another tool |
| 7 | [Learn Arithmetic](LEARN_ARITHMETIC.md) | Add/subtract, absdiff, and bitwise mask reasoning |
| 8 | [Learn Geometry Transform](LEARN_GEOMETRY_TRANSFORM.md) | Rotate/scale, output size, and ROI coordinate review |
| 9 | [Learn Filter](LEARN_FILTER.md) | Noise reduction and edge preparation before detection |
| 10 | [Learn Morphology](LEARN_MORPHOLOGY.md) | Binary cleanup before Blob or Contour |
| 11 | [Learn Mean](LEARN_MEAN.md) | Brightness drift with one metric |
| 12 | [Learn Color / HSV](LEARN_COLOR_HSV.md) | HSV ranges, channel thinking, Mean, and Histogram review |
| 13 | [Learn Edge Detection](LEARN_EDGE_DETECTION.md) | Edge maps, gradient thinking, and downstream tool choice |
| 14 | [Learn Feature Matching](LEARN_FEATURE_MATCHING.md) | Feature score discrimination |
| 15 | [Learn Edge Based Matching](LEARN_EDGE_BASED_MATCHING.md) | Edge geometry target checks |
| 16 | [Learn Line](LEARN_LINE.md) | Distance, angle, and line measurement |
| 17 | [Learn Pipeline / Layers](LEARN_PIPELINE_LAYER_ROUTING.md) | Input/output layers, branches, and explicit execution |
| 18 | [Learn Metrics / Acceptance](LEARN_METRICS_ACCEPTANCE.md) | OK/NG gates, metric names, and Good/Bad comparison |

## Rules While Practicing

- Use `docs/samples/public` and `docs/samples/public/product` for public documentation and tutorials.
- Do not use commercial SDK sample folders in public-facing material.
- Opening a guide or sample must not run Preview or Run.
- Output layers and input layers must stay explicit.
- Good/Bad decisions should be checked by bounded metrics, not only by how the image looks.

## Recommended Review Order

1. Open a Good sample.
2. Read the expected metric range.
3. Run the prepared pipeline manually.
4. Check the result image and metric.
5. Open the Bad sample from the same PairGroup.
6. Run the same pipeline manually.
7. Confirm which metric explains NG.

If the Bad sample passes, tighten the ROI, threshold, score, area, count, or distance gate before adding more samples.
