# Evidence and limits

## Before accepting a result

These four items must describe the same physical target:

1. Input image and ROI
2. Result drawings
3. Metrics
4. Good/Bad or Validation results

Do not accept a result from execution count or an `OK` label alone.

## Units

- A px result is measured in image pixels.
- A mm result depends on the applied `mm per pixel` value.
- A two-point scale is uniform scale in one image plane, not camera calibration.
- Distortion, perspective, and depth variation need separate physical evidence.

## Product scope

OpenVisionLab is a workbench for teaching, executing, comparing, and validating
rule-based recipes on sample images. This manual does not certify cameras,
lighting, PLC, I/O, MES, production control, or field performance.

LLM Assistant is an optional XML-authoring aid. It is not required for the
normal workflow or for this manual.
