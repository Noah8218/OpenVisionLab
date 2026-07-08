# OpenVisionLab Learn Curriculum

Updated: 2026-07-07

This curriculum is inspired by common machine-vision learning order, but it is rewritten for OpenVisionLab. It is not an OpenCV installation or camera-programming textbook. It should still teach enough OpenCvSharp concepts for an operator to understand what the tools are doing.

Tool Views stay as editors. Learn content belongs in the separate Learn window, Learn tab, Sample Picker guide path, or documentation.

## Learning Rule

Every topic should connect these five pieces:

`concept -> visual explanation -> public sample -> OpenVisionLab tool -> explicit Preview/Run or validation result`

Opening a guide must not run Preview/Run, create layers, change input/output routing, or edit tool parameters. Applying a suggested value must be an explicit action.

## Foundation Map

Read [Learn OpenCvSharp Foundations](LEARN_OPENCVSHARP_FOUNDATIONS.md) before tool-specific topics when coordinates, ROI, or image-matrix terms are unclear. These foundation topics are allowed and expected because they explain OpenVisionLab parameters, overlays, metrics, and XML:

| Topic | Why it matters in OpenVisionLab | First Learn surface |
| --- | --- | --- |
| Pixel and GV | Threshold, Mean, Histogram, EdgeDetection all start from pixel values | Learn window chapter 0-1 |
| Channel values | HSV and color range checks depend on channel values | Planned color chapter |
| Coordinate system | ROI, edge scan direction, match center, contour bounds all use X/Y coordinates | `LEARN_OPENCVSHARP_FOUNDATIONS.md` |
| `Point` | edge points, keypoints, match centers, line endpoints | `LEARN_OPENCVSHARP_FOUNDATIONS.md` |
| `Size` | image size, kernel size, template size, output dimensions | `LEARN_OPENCVSHARP_FOUNDATIONS.md` |
| `Rect` | ROI, bounding box, template crop, sample region | Learn window chapter 0 and `LEARN_OPENCVSHARP_FOUNDATIONS.md` |
| `RotatedRect` | rotated template ROI, rotated matching result, angle review | `LEARN_OPENCVSHARP_FOUNDATIONS.md` |
| `Mat` | image as matrix, row/column access, channel count, ROI slice | `LEARN_OPENCVSHARP_FOUNDATIONS.md` |
| Matrix operation | filter kernel, morphology kernel, affine/rotate/scale transform | `LEARN_OPENCVSHARP_FOUNDATIONS.md` and chapters 6-8 |

## OpenVisionLab Chapter Map

| Chapter | Topic | OpenVisionLab focus | Status |
| --- | --- | --- | --- |
| 0 | OpenCvSharp foundation | pixel/GV, channel, coordinate, `Point`, `Size`, `Rect`, `RotatedRect`, `Scalar`, `Mat`, ROI, layer | Done: Learn window topic 0 with operator cards and `LEARN_OPENCVSHARP_FOUNDATIONS.md` |
| 1 | Computer vision and inspection workflow | image, layer, tool, metric, OK/NG reason | Planned |
| 2 | Image structure | grayscale/color, coordinate system, ROI, matrix-style image data | Partial: foundation guide covers operator-facing image structure; deeper channel split/merge remains a tool gap |
| 3 | OpenCvSharp data types for operators | `Point`, `Size`, `Rect`, `RotatedRect`, `Scalar`, `Mat`, ROI slice | Done for operator use: `LEARN_OPENCVSHARP_FOUNDATIONS.md`; no generic API tutorial planned |
| 4 | Workspace and layer basics | Main layer, output layer, layer comparison, no-auto-run rule | Planned |
| 5 | Brightness, contrast, histogram | mean brightness, contrast spread, histogram drift, stretch/equalize concepts | Partial: `LEARN_MEAN.md`; tools: Mean, Histogram; gap: contrast/stretch/equalize tool |
| 6 | Arithmetic and logical operations | add/subtract/absdiff/bitwise operations, mask-style reasoning | Done: Learn window topic 14 and `LEARN_ARITHMETIC.md`; tool: Arithmetic |
| 7 | Filtering | average/gaussian/median-style smoothing, edge preparation, noise cleanup | Done: `LEARN_FILTER.md`; tool: Filter |
| 8 | Geometric transforms | rotate, scale, affine/perspective concepts, ROI coordinate change | Partial: Learn window topic 15 and `LEARN_GEOMETRY_TRANSFORM.md`; tool: RotateScale; gap: affine/perspective tool |
| 9 | Edge and line detection | Canny/edge threshold, scan direction, line fit, Hough concept where needed | Partial: Learn window topic 7, `LEARN_EDGE_DETECTION.md`, and `LEARN_LINE.md`; tools: EdgeDetection, LineGauge, LineDistance; gap: public EdgeDetection Good/Bad pair and Hough line/circle tool |
| 10 | Color image processing | HSV, color range segmentation, channel thinking, color drift | First-pass done: Learn window topic 16, `LEARN_COLOR_HSV.md`, `HSV` VisionPipeline ToolType, `MaskPixelRatio`, and public HSV Good/Bad samples; remaining gap: channel split/merge guide/tool for deeper color debugging |
| 11 | Threshold and morphology | binary/adaptive threshold, erosion, dilation, open, close | Done: `LEARN_THRESHOLD.md`, `LEARN_MORPHOLOGY.md` |
| 12 | Labeling and contour | connected components, Blob count/area, contour outline/box/draw mode | Done: `LEARN_BLOB.md`, `LEARN_CONTOUR.md` |
| 13 | Object matching | template matching, edge-based template matching, score/count gates | Done: `LEARN_MATCHING.md`, `LEARN_EDGE_BASED_MATCHING.md` |
| 14 | Feature detection and matching | keypoints, descriptors, ratio, RANSAC, score gate, homography concept | Done: `LEARN_FEATURE_MATCHING.md`; gap: standalone corner/keypoint detector guide/tool |
| 15 | Pipeline and layer routing | InputLayer, OutputLayer, fan-out, OverlayMerge review | Done: `LEARN_PIPELINE_LAYER_ROUTING.md` |
| 16 | Metrics and acceptance gates | ResultCount, ScoreMax, AreaAvg, DistanceMmAvg, DistanceMmRange | Done: `LEARN_METRICS_ACCEPTANCE.md` |
| 17 | Good/Bad validation | public pairs, controlled NG, metric separation | Partial: `LEARN_PRODUCT_SAMPLES.md` |
| 18 | Recipe Manager review | step list, selected step, dependencies, failed-step review | Planned |
| 19 | LLM XML authoring | prompt packet, XML validation, correction loop, import gate | Planned |
| 20 | Troubleshooting cookbook | symptom -> likely cause -> tool setting -> validation evidence | Planned |

## Tool Gap Backlog

Do not add all missing tools at once. Add them one bounded workflow at a time, keeping PropertyGrid-based tools, explicit Preview/Run, and sample-backed metrics.

| Gap | Candidate tool | Why it may be needed | First verification sample |
| --- | --- | --- | --- |
| Contrast stretch/equalize | `BrightnessContrast` or `HistogramNormalize` | Chapter 5 needs contrast/histogram operations beyond Mean/Histogram review | synthetic low-contrast mark |
| Affine/perspective transform | extend `RotateScale` or add `GeometryTransform` | Chapter 8 needs transform concepts beyond rotate/scale | synthetic tilted rectangle/mark |
| Hough line/circle | `HoughLineCircle` | Chapter 9 line/circle detection concept not covered by LineDistance alone | synthetic line/circle target |
| Channel split/merge | `ChannelSplit` or HSV extension | Chapter 10 channel education and color debugging | synthetic RGB/HSV patches |
| Standalone corner/keypoint review | `CornerFeature` | Chapter 14 needs visible keypoint teaching before FeatureMatching | synthetic corner/card target |

## Excluded From The Main Curriculum

These are useful OpenCV programming topics, but they are not OpenVisionLab product goals right now:

- OpenCV installation, CMake, or HelloCV project setup tutorials.
- Camera, video capture, lighting, PLC, I/O, deployment, account, or audit workflows.
- Generic source-code API coverage that does not explain an OpenVisionLab parameter, overlay, layer, metric, or XML field.
- Machine learning, DNN, face detection, pedestrian detection, or generic AI chapters.
- Full OpenCV source build, Linux setup, or external SDK walkthroughs.

## First Implementation Order

1. Keep the current individual tool guides working.
2. Add a visible curriculum/index entry in the Learn surface.
3. Fill planned chapters with short operator-facing pages, not long textbook chapters.
4. Add one current-source screenshot smoke per new visible Learn surface change.
5. Use public-safe samples only.
