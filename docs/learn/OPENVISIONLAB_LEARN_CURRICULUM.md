# OpenVisionLab Learn Curriculum

Updated: 2026-07-07

This curriculum follows the concepts needed to understand and verify rule-based inspection in OpenVisionLab. It starts with pixels, coordinates, ROI, and layers, then connects preprocessing, detection, measurement, matching, and Good/Bad decisions to the actual tools.

Tool Views stay as editors. Learn content belongs in the separate Learn window, Learn tab, Sample Picker guide path, or documentation.

## Learning Flow

Every topic should connect these five pieces:

`concept -> visual explanation -> public sample -> OpenVisionLab tool -> Preview/Run result -> Good/Bad metric`

Read the concept first, open its public sample, locate the same parameter in the PropertyGrid, then compare the result image and metric after Preview or Run Review.

## Foundation Map

Read [Learn OpenCvSharp Foundations](LEARN_OPENCVSHARP_FOUNDATIONS.md) before tool-specific topics when coordinates, ROI, or image-matrix terms are unclear. These foundation topics are allowed and expected because they explain OpenVisionLab parameters, overlays, metrics, and XML:

| Topic | Why it matters in OpenVisionLab | First Learn surface |
| --- | --- | --- |
| Pixel and GV | Threshold, Mean, Histogram, EdgeDetection all start from pixel values | Learn window chapter 0-1 |
| Channel values | HSV and color range checks depend on channel values | Learn window topic 16 |
| Coordinate system | ROI, edge scan direction, match center, contour bounds all use X/Y coordinates | `LEARN_OPENCVSHARP_FOUNDATIONS.md` |
| `Point` | edge points, keypoints, match centers, line endpoints | `LEARN_OPENCVSHARP_FOUNDATIONS.md` |
| `Size` | image size, kernel size, template size, output dimensions | `LEARN_OPENCVSHARP_FOUNDATIONS.md` |
| `Rect` | ROI, bounding box, template crop, sample region | Learn window chapter 0 and `LEARN_OPENCVSHARP_FOUNDATIONS.md` |
| `RotatedRect` | rotated template ROI, rotated matching result, angle review | `LEARN_OPENCVSHARP_FOUNDATIONS.md` |
| `Mat` | image as matrix, row/column access, channel count, ROI slice | `LEARN_OPENCVSHARP_FOUNDATIONS.md` |
| Matrix operation | filter kernel, morphology kernel, affine/rotate/scale transform | `LEARN_OPENCVSHARP_FOUNDATIONS.md` and chapters 6-8 |

## OpenVisionLab Chapter Map

| Chapter | Topic | OpenVisionLab focus | Learn connection |
| --- | --- | --- | --- |
| 0 | OpenCvSharp foundation | pixel/GV, channel, coordinate, `Point`, `Size`, `Rect`, `RotatedRect`, `Scalar`, `Mat`, ROI, layer | Learn topic 0 and `LEARN_OPENCVSHARP_FOUNDATIONS.md` |
| 1 | Computer vision and inspection workflow | image, layer, tool, metric, OK/NG reason | Learn topic 0 |
| 2 | Image structure | grayscale/color, coordinate system, ROI, matrix-style image data | Learn topic 0 |
| 3 | OpenCvSharp data types for operators | `Point`, `Size`, `Rect`, `RotatedRect`, `Scalar`, `Mat`, ROI slice | `LEARN_OPENCVSHARP_FOUNDATIONS.md` |
| 4 | Workspace and layer basics | Main layer, output layer, Step connection, layer comparison | Learn topic 11 and `LEARN_PIPELINE_LAYER_ROUTING.md` |
| 5 | Brightness, contrast, histogram | mean brightness, contrast spread, histogram drift | Learn topic 1 and `LEARN_MEAN.md` |
| 6 | Arithmetic and logical operations | add/subtract/absdiff/bitwise operations, mask-style reasoning | Learn topic 14 and `LEARN_ARITHMETIC.md` |
| 7 | Filtering | average/gaussian/median-style smoothing, edge preparation, noise cleanup | Learn topic 3 and `LEARN_FILTER.md` |
| 8 | Geometric transforms | rotate, scale, ROI coordinate change | Learn topic 15 and `LEARN_GEOMETRY_TRANSFORM.md` |
| 9 | Edge and line detection | Canny/edge threshold, scan direction, line fit, distance | Learn topics 7-8, `LEARN_EDGE_DETECTION.md`, and `LEARN_LINE.md` |
| 10 | Color image processing | HSV, color range segmentation, channel thinking, color drift | Learn topic 16 and `LEARN_COLOR_HSV.md` |
| 11 | Threshold and morphology | binary/adaptive threshold, erosion, dilation, open, close | Learn topics 2 and 4 |
| 12 | Labeling and contour | connected components, Blob count/area, contour outline/box/draw mode | Learn topics 5-6 |
| 13 | Object matching | template matching, edge-based template matching, score/count gates | Learn topics 9 and 12 |
| 14 | Feature detection and matching | keypoints, descriptors, ratio, RANSAC, score gate | Learn topic 10 |
| 15 | Pipeline and layer routing | InputLayer, OutputLayer, fan-out, OverlayMerge review | Learn topic 11 |
| 16 | Metrics and acceptance gates | ResultCount, ScoreMax, AreaAvg, DistanceMmAvg, DistanceMmRange | Learn topic 13 |
| 17 | Good/Bad validation | public pairs, controlled NG, metric separation | `LEARN_PRODUCT_SAMPLES.md` |

## Recommended Learning Order

1. Learn pixel/GV, Point/Size/Rect/Mat, ROI, and Layer in topic 0.
2. Practice image preparation with Brightness, Threshold, Filter, Morphology, Arithmetic, Geometry, and HSV.
3. Practice candidate detection and measurement with Blob, Contour, Edge, and LineDistance.
4. Compare targets with Matching, EdgeBasedMatching, and FeatureMatching.
5. Follow the Pipeline route and prove Good/Bad with bounded metrics.
