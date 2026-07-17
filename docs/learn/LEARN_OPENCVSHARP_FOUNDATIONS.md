# Learn OpenCvSharp Foundations

Updated: 2026-07-08

This page explains the OpenCvSharp concepts used while tuning OpenVisionLab rule-based tools: pixels, coordinates, ROI, image matrices, and layers.

Read the term first, choose a related sample, find the same value in the PropertyGrid, and compare the input and output after Preview.

## Operator Mental Model

OpenVisionLab work usually moves through this chain:

`image -> Mat/layer -> ROI -> tool parameters -> output layer -> metric -> Good/Bad decision`

If one term in that chain is unclear, the tool may still run, but the operator cannot explain why the result is OK or NG.

## Pixel, GV, And Channels

| Concept | Meaning in OpenVisionLab | Where it appears |
| --- | --- | --- |
| Pixel | One image cell at an X/Y position | Viewer cursor, edge point, contour point |
| GV | Gray Value. In an 8-bit grayscale image, 0 is black and 255 is white | Threshold, Mean, Histogram, EdgeDetection |
| Channel | One component of a color image, such as B/G/R or H/S/V | HSV, Histogram, color drift review |
| Binary pixel | A pixel forced to 0 or 255 | Threshold output, Morphology input, Blob/Contour input |

Good/Bad samples are useful only when the metric changes for the intended reason. For example, a dark Bad sample should change Mean or Threshold output because of brightness, not because the ROI moved.

## Coordinate System

OpenVisionLab uses image coordinates:

- X grows left to right.
- Y grows top to bottom.
- The top-left pixel is near `X=0, Y=0`.
- Width and height are measured in pixels unless a pixel/mm scale is explicitly applied.

This matters for `CvROI`, edge scan direction, match center, contour bounds, line endpoints, and distance measurement.

## Core Data Types

| Type | Simple meaning | Typical OpenVisionLab use |
| --- | --- | --- |
| `Point` | `X,Y` location | edge point, keypoint, match center, line endpoint |
| `Size` | `Width,Height` | image size, kernel size, template size, output size |
| `Rect` | `X,Y,Width,Height` rectangle | ROI, bounding box, template crop |
| `RotatedRect` | center, size, angle | rotated match result or rotated template review |
| `Scalar` | one or more numeric channel values | draw color, threshold value, fill value |
| `Mat` | image data stored like a matrix | every input layer, output layer, ROI slice, and intermediate result |

## Mat As An Image Matrix

Think of `Mat` as rows, columns, and channels:

`Mat[Row=Y, Column=X, Channel] -> pixel value`

That is why many tool parameters look like matrix operations:

- Filter uses a kernel over neighboring pixels.
- Morphology uses a kernel over binary neighborhoods.
- RotateScale creates a transformed output matrix.
- Histogram counts how often each GV or channel value appears.

## ROI And Layer Flow

`Rect ROI = X,Y,Width,Height`

An ROI is a deliberate restriction of where the tool looks. It should be small enough to remove unrelated background and large enough to contain the full expected variation.

Read a layer route in this order:

- `InputLayer` is the source layer for the step.
- `OutputLayer` is the result created by the step.
- To continue processing, select the previous step's `OutputLayer` as the next step's `InputLayer`.
- Compare the original, input, and output layers to locate where the image changed.

## How This Maps To PropertyGrid

| PropertyGrid field | Foundation concept | Check before running |
| --- | --- | --- |
| `CvROI` | `Rect` | Does the rectangle cover the intended target only? |
| `Threshold` | GV | Does the value split object and background? |
| `KernelSize` | `Size` / matrix neighborhood | Is the kernel big enough to clean noise without deleting the target? |
| `TemplatePath` / template ROI | `Rect`, `Mat` crop | Is the template tight and representative? |
| `PIXELPERMM` | calibration scale | Does the metric need pixels or mm? |
| `InputLayer` / `OutputLayer` | layer route | Is the step reading from the intended layer and writing to a new reviewable layer? |

## Practice Path

Use the provided Good/Bad samples:

1. Open Topic 1 Brightness / Histogram and a `mean` Good/Bad pair.
2. Confirm how GV changes the Mean metric.
3. Open Topic 2 Threshold and a `preprocess` sample.
4. Change Threshold only after checking object/background GV.
5. Open Topic 8 LineDistance when distance needs pixels/mm and outlier range gates.
6. Open Topic 9 Matching when the target is a template, not a binary region.

Name the image concept first, choose the matching tool family, then prove the result with Preview/Run output and a Good/Bad metric.
