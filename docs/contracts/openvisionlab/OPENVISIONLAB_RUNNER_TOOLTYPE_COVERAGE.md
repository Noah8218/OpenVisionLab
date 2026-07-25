# OpenVisionLab Runner ToolType Coverage

This document separates pipeline-runner tools from form-only or demo-only features.
Use it when creating LLM recipes, sample catalog rows, or automated validation scenarios.

## Runner-Supported ToolTypes

These values are valid for `VisionPipeline` XML and can be executed by the recipe runner:

| ToolType | Main use |
|---|---|
| `Threshold` | Binary/adaptive threshold preprocessing |
| `Morphology` | Open, close, erode, dilate, gradient cleanup |
| `Filter` | Blur, Gaussian, median, bilateral preprocessing |
| `EdgeDetection` | Canny/Sobel/Scharr/Laplacian edge image generation |
| `Blob` | Blob candidate detection and area/count metrics |
| `Contour` | Contour candidate detection, boxes, area/count metrics |
| `LineGauge` | Edge/line detection and line length/angle metrics |
| `RotateScale` | Image rotation and scaling |
| `Matching` | Template matching and score metrics |
| `Mean` | Brightness/intensity measurement |
| `FeatureMatching` | Feature/template style matching and score metrics |
| `EdgeBasedMatching` | Edge template matching and contour-based candidate/score metrics |
| `OverlayMerge` | Merge branch overlays into one final review image |

Aliases accepted by validation/factory logic include `Line`, `TemplateMatching`,
`RotateAndScale`, `Feature`, `Sift`, `EdgeTemplateMatching`, `EdgeTemplate`, `EdgeBased`,
`ResultMerge`, and `MergeResult`.
Prefer the canonical names above in new XML.

## Form-Only Or Demo-Only Features

These features may exist as UI forms, sample folders, or future directions, but they
must not be emitted as `ToolType` values in pipeline XML until runner support exists:

| Feature name | Current recipe handling |
|---|---|
| `HSV` | Use supported candidate detection only, or leave as manual form work |
| `Histogram` | Use `Mean` or supported preprocessing where possible |
| `Arithmetic` | Leave as manual form work unless a runner wrapper is added |
| `Color` / `EasyColor` | Use candidate detection only; no color classifier contract yet |
| `Barcode` / `EasyBarCode` | Use candidate detection only; no decoder contract yet |
| `QR` / `EasyQRCode` | Use candidate detection only; no decoder contract yet |
| `OCR` / `EasyOcr` | Use candidate detection only; no OCR contract yet |

## LLM Recipe Rule

An LLM recipe should generate only runner-supported steps. If the user asks for a
decoder, classifier, or form-only capability, the recipe summary should say that
OpenVisionLab can currently generate candidate regions and preview them, but the
semantic decoder/classifier still needs a runner-backed tool.

For branched recipes, the final answer should be one `OverlayMerge` output layer.
Users should not need to inspect several branch images to decide whether the recipe
worked.
