# OpenVisionLab Learn Mode

Updated: 2026-07-02

Learn Mode is for learning rule-based vision inspection with public sample assets. The important flow is:

`image -> layer -> tool -> metric -> OK/NG reason`

Open the Good sample first, then open the Bad sample from the same PairGroup and run the same baseline pipeline. A sample is useful only when the result can be explained by image, overlay, metric, and log together.

## Start Here

| Order | Guide | What to learn |
| --- | --- | --- |
| 1 | [Product Sample Guide](LEARN_PRODUCT_SAMPLES.md) | Battery, display, and semiconductor Good/Bad pairs |
| 2 | [Learn Matching](LEARN_MATCHING.md) | Template target presence, `ScoreMax`, `ResultCount` |
| 3 | [Learn Blob](LEARN_BLOB.md) | Particle/stain count and area checks |
| 4 | [Learn Contour](LEARN_CONTOUR.md) | Shape/region count and area checks |
| 5 | [Learn Threshold](LEARN_THRESHOLD.md) | Bright/dark region separation before another tool |
| 6 | [Learn Mean](LEARN_MEAN.md) | Brightness drift with one metric |
| 7 | [Learn Feature Matching](LEARN_FEATURE_MATCHING.md) | Feature score discrimination |
| 8 | [Learn Edge Based Matching](LEARN_EDGE_BASED_MATCHING.md) | Edge geometry target checks |
| 9 | [Learn Line](LEARN_LINE.md) | Distance, angle, and line measurement |

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
