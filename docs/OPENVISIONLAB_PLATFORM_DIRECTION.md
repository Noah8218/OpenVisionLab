# OpenVisionLab Platform Direction

OpenVisionLab is a rule-based vision workbench. Its purpose is to make OpenCV/OpenCvSharp inspection development faster, easier to validate, and easier to reuse outside the UI.

## Product Position

OpenVisionLab should not stop at being an OpenCV demo application. It should become a Vision Recipe Platform:

- Load an image and inspect pixels, ROI, layers, and coordinates.
- Tune OpenCV-based tools with immediate preview.
- Compose tools into a pipeline.
- Validate each step with metrics, overlays, and acceptance rules.
- Save the approved pipeline as XML.
- Execute the same XML from UI, AI Recipe import, batch validation, and an external runner/DLL.

## Current Capabilities

- Image display and layer-based result management.
- Main viewer status for layer, image size, coordinates, zoom, GV, and RGB.
- Tool forms using the common `RunVisionStep` path.
- Threshold, Morphology, Filter, EdgeDetection, Blob, Contour, LineGauge, Matching, Mean, and FeatureMatching pipeline support.
- Pipeline step tree with editable properties, input/output layers, step preview, result table, and run log.
- Step metrics, acceptance rules, overlay preview, and image viewer zoom/pan.
- Pipeline XML save/load/import and active pipeline persistence.
- Sample set storage for layers.
- Run report storage with result images and overlay images.
- AI Recipe import dialog with XML validation, sample recipe load, image preview run, and overlay display.
- UI screenshot smoke checks for major forms.
- Recipe XML compatibility check.
- `VisionRecipeRunner` for UI-free XML + image execution.

## Core Contracts

Every tool should satisfy the same contract:

- Input: one source `Mat`.
- Parameters: serializable key/value pairs using invariant culture.
- Output: `VisionToolResult`.
- Result image: `VisionToolResult.ResultImage`.
- Metrics: `VisionToolResult.Metrics`.
- Overlays: `VisionToolResult.Overlays`.
- Status: success, message, elapsed time, and exception when failed.

Every pipeline step should preserve:

- `Name`
- `ToolType`
- `Enabled`
- `InputLayer`
- `OutputLayer`
- `Parameters`
- Acceptance criteria when used

## Near-Term Development Priorities

1. Finish tool standardization.
   - Keep every inspection form on `RunVisionStep`.
   - Ensure every pipeline-capable tool maps property -> step -> tool consistently.
   - Keep parameter names stable for XML and AI Recipe usage.

2. Harden pipeline reliability.
   - Save/load must round-trip all step metadata and parameters.
   - Sample images should be explicit sample sets, not hidden side effects.
   - Step previews and run reports should remain reviewable after restart.

3. Standardize result output.
   - Use the same metric names everywhere.
   - Return overlay count and overlay summaries from runner output.
   - Keep acceptance status visible in UI, logs, reports, and runner output.
   - Use `docs/VISION_TOOL_RESULT_CONTRACT.md` as the minimum tool result standard.

4. Expand AI Recipe workflow.
   - Maintain a strict XML contract.
   - Provide LLM prompt templates and sample recipes.
   - Treat AI output as a first-pass recipe that must be validated by OpenVisionLab.

5. Prepare the external runner/DLL path.
   - Keep runtime code independent from WinForms UI.
   - Expose `VisionRecipeRunner` as the first stable API.
   - Use smoke tests to prove that UI-created XML works outside the UI.

## Validation Baseline

Use this command as the platform-level smoke check:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunVisionPlatformPrecheck.ps1 -FailOnUiWarn
```

This checks:

- OpenVisionLab build.
- XML compatibility and sample XML deserialization.
- `VisionRecipeRunner` with `Sample\Contour.jpg` and the runnable contour sample recipes.
- UI screenshot smoke checks unless `-SkipUi` is used.

## AI Recipe Direction

The LLM should not directly own the final inspection result. The LLM should generate a conservative first-pass XML recipe. OpenVisionLab should validate, run, display overlays, allow tuning, and save the approved recipe.

Recommended flow:

```text
User image + inspection goal
  -> LLM suggests VisionPipeline XML
  -> OpenVisionLab validates XML
  -> OpenVisionLab runs preview with real image
  -> User reviews overlays and metrics
  -> User tunes parameters
  -> OpenVisionLab saves approved recipe
  -> VisionRecipeRunner executes approved XML outside the UI
```

## Definition Of Done For New Tools

A new tool is platform-ready only when:

- It runs from its form through `RunVisionStep`.
- It can be added as a pipeline step.
- It can be serialized to XML and loaded again.
- It produces a result image or a clear no-image result.
- It exposes metrics when the result needs judgment.
- It exposes overlays when the result needs review.
- It has at least one sample XML or smoke coverage.
