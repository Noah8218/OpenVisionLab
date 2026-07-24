# OpenVisionLab Affine Transform v1 Contract

Date: 2026-07-23

## Outcome

Add a deterministic two-dimensional affine image transform that is calculated and
executed by the separately built Library-Noah `Lib.OpenCV.dll`, then exposed in
OpenVisionLab as a PropertyGrid tool, Pipeline/XML Step, and Geometry Learn path.

The operator teaches three non-collinear source points and their three destination
points. The tool computes the `2 x 3` affine matrix, warps the image into the requested
output size, draws reviewable destination/frame geometry, and publishes matrix,
geometry, and valid-pixel metrics.

## Included Scope

- Library-Noah owns:
  - point and output validation;
  - affine matrix calculation;
  - `WarpAffine` execution;
  - valid-pixel calculation;
  - matrix/decomposition metrics;
  - result drawing primitives and fail-closed error codes.
- OpenVisionLab owns:
  - a PropertyGrid-based `Affine Transform` Tool View;
  - explicit Preview and Pipeline execution only;
  - Pipeline/XML create, validate, save, load, and selected-Step edit round trip;
  - localized operator wording;
  - Geometry Learn guidance and tool navigation;
  - public sample XML and current-build evidence.
- Canonical Pipeline/XML tool name: `AffineTransform`.
- Accepted compatibility aliases: `Affine`, `AffineMatrix`.
- Coordinates and distances are pixel-only in v1.

## Parameters

| Group | Parameters | Contract |
| --- | --- | --- |
| Source points | `SourcePoint1X/Y` through `SourcePoint3X/Y` | Three non-collinear pixel points in the input coordinate system |
| Destination points | `DestinationPoint1X/Y` through `DestinationPoint3X/Y` | Three non-collinear pixel points in the output coordinate system |
| Output | `OutputWidth`, `OutputHeight` | `0` keeps the input dimension; positive values set an explicit dimension |
| Sampling | `Interpolation`, `BorderType`, `BorderValue` | OpenCV interpolation and border policy with a scalar border value |
| Gates | `MinimumSourceTriangleArea`, `MinimumDestinationTriangleArea`, `MinimumValidPixelRatio` | Fail-closed geometry and retained-source coverage gates |

The default points are an identity mapping:

```text
Source:      (0,0), (100,0), (0,100)
Destination: (0,0), (100,0), (0,100)
```

Points may lie outside the image. This is intentional: the affine mapping remains
well-defined, and the valid-pixel gate owns cropping/coverage rejection.

## Result Metrics

- `AffineM11`, `AffineM12`, `AffineM13`
- `AffineM21`, `AffineM22`, `AffineM23`
- `AffineDeterminant`
- `AffineScaleX`, `AffineScaleY`
- `AffineRotationDeg`
- `AffineShearCosine`
- `AffineTranslationX`, `AffineTranslationY`
- `AffineSourceTriangleArea`, `AffineDestinationTriangleArea`
- `AffineValidPixelRatio`
- standard result image width, height, and channel metrics

`AffineRotationDeg`, scales, and shear are review aids derived from the matrix. They
do not replace the authoritative six matrix coefficients.

## Drawings

Every successful execution retains current-run geometry in output coordinates:

- the three taught destination points;
- the three destination-triangle edges;
- the four transformed input-frame edges.

The drawing is evidence of which mapping was executed. It is not a claim that the
operator selected the correct physical features.

## Failure Contract

The tool fails closed for:

- an empty input image;
- non-finite point, gate, or border values;
- collinear source or destination points, even when the configured minimum is zero;
- source or destination triangle area below its configured minimum;
- output dimensions outside the supported range;
- unsupported interpolation or border policy;
- an invalid matrix or transform execution;
- `AffineValidPixelRatio` below `MinimumValidPixelRatio`.

Failure must expose a stable error code and diagnostic message. A coverage failure may
retain the warped image, metrics, and drawings so the operator can correct the teaching
without inventing a pass.

## Explicit UI Contract

- Opening the Tool View, changing a PropertyGrid value, changing layer selection, or
  adding a Pipeline Step does not run Preview or Pipeline execution.
- Preview and Run remain explicit operator actions.
- Writing the output creates or updates the selected output layer without changing the
  selected input layer.
- The Tool View uses the shared WPG-based PropertyGrid host and current Tool View theme.

## Learning Contract

Geometry Learn teaches this sequence:

1. Select three stable, well-separated, non-collinear physical points.
2. Enter their input coordinates and the desired output coordinates.
3. Review source/destination triangle areas before execution.
4. Run Preview explicitly.
5. Review transformed-frame/destination drawings and `AffineValidPixelRatio`.
6. Freeze the Step and replay representative samples before using downstream fixed ROIs.

v1 does not add an image-click point editor. Numeric PropertyGrid point teaching is the
bounded operator path; a graphical three-point editor requires separate evidence and
approval.

## Excluded Scope

- camera or lens calibration;
- homography/perspective correction;
- automatic keypoint correspondence;
- Matching-driven per-image pose estimation;
- per-image automatic ROI movement;
- calibrated mm units;
- industrial field-robustness or accuracy qualification;
- LLM prompt/provider/skill expansion.

## Acceptance Criteria

| Criterion | Required evidence |
| --- | --- |
| Library source ownership | Library-Noah source and standalone build with no OpenVisionLab implementation of the affine calculation |
| Numeric correctness | A known synthetic transform recovers the expected six coefficients within tolerance and produces the expected output geometry |
| Fail-closed behavior | Collinear source/destination teaching and insufficient valid-pixel coverage fail with the named errors |
| Vendored consumption | OpenVisionLab builds and executes against the copied `dll\Library-Noah\Lib.OpenCV.dll`; source version and DLL hash are recorded |
| PropertyGrid workflow | Current-build Tool View exposes all v1 parameters and does not run until explicit Preview |
| Pipeline/XML round trip | Canonical and alias tool names import, execute, save/load, and selected-Step edit without parameter loss |
| Drawing/metric evidence | Fresh result image shows destination points/triangle and transformed input frame; matrix and coverage metrics match the run |
| Learn path | Current-build Geometry Learn explains three-point teaching, coverage review, downstream fixed-ROI use, and opens the Affine Tool View |
| Regression boundary | OpenVisionLab solution build, focused smokes, readiness check, and the existing RotateScale path pass |

## Risks

- Incorrect point ordering can create a valid but semantically mirrored/sheared result.
- A small triangle amplifies noise; area gates reduce but cannot eliminate poor teaching.
- Output size and destination points can crop the source; the valid-pixel gate exposes,
  but does not infer, the intended crop.
- A public API addition requires a Library-Noah minor version increase and exact DLL
  provenance.

## Completion Record

```text
Status: Complete
Scope: Library-Noah three-point 2D Affine runtime plus OpenVisionLab PropertyGrid, explicit Preview, Pipeline/XML, drawings/metrics, public sample, and Geometry Learn integration.
Acceptance criteria: Library ownership, known matrix, collinear/coverage fail-closed gates, vendored DLL identity, PropertyGrid workflow, XML aliases/round trip, drawings/metrics, Learn path, and RotateScale regression all passed.
Verification: Library-Noah build and 57/57 smoke; OpenVisionLab zero-warning build; focused Affine contract/public-sample/UI smokes; readiness and public-sample checks.
Evidence: artifacts\p218_affine_transform_v1_20260723
Boundary / next dependency: Known-matrix synthetic evidence only. A real use case requires operator-approved corresponding point triplets, downstream reference ROI, representative samples, and acceptance criteria.
```
