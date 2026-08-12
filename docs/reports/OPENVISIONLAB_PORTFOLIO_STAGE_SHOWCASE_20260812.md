# Portfolio stage showcase — 2026-08-12

## Scope

Current-EXE portfolio and README evidence for one-source, multi-Layer rule-based
inspection. The captures contain no Computer Use cursor or overlay.

## Reproducible public recipes

- `Product_Field_PerforatedPlate_Inspection.pipeline.xml`
  - `Filter -> Threshold -> Morphology`
  - `Hole_Clean -> Blob`, 34 accepted objects
  - `Hole_Clean -> Contour`, 34 accepted objects
  - shared area gate: 600–6000 px²; the full-image background is excluded
- `Product_Field_ShaftPitting_Inspection.pipeline.xml`
  - `Filter -> ThresholdInv -> Morphology -> Contour`
  - reviewed ROI `730,300,200,180`
  - `Threshold=100`, 3x3 morphology Open, and `MIN_AREA=2 px^2`
  - 18 retained dark-pit candidates in the current NG evidence run
  - defect-free acceptance contract `ResultCount=0`; the current sample's
    18 candidates therefore produce an explicit NG instead of a broad
    `1..20` PASS
- `Product_Semiconductor_LeadWidth_Distance.pipeline.xml`
  - one reviewed measurement ROI
  - 16 distance scans, average 42.012 px / 0.252 mm in the retained run
- `Public_EdgeDetection_Shapes.pipeline.xml`
  - `EdgeDetection -> Morphology -> Contour`
  - four expected shape boundaries and four accepted contour results

## Actual EXE evidence

Evidence root:

`D:\OpenVisionLab-TestData\OpenVisionLab\portfolio_stage_showcase_20260812\delivery`

Retuned shaft-pitting evidence root:

`D:\OpenVisionLab-TestData\OpenVisionLab\shaft_pitting_retune_20260812\actual_exe_final`

Accepted captures:

- Korean hole-array stage grid:
  `hole_ko\01_stage_grid_actual_exe.png`
- English hole-array stage grid and Pipeline Review:
  `hole_en\01_stage_grid_actual_exe.png`
  `hole_en\02_pipeline_review_actual_exe.png`
- Korean shaft-pitting stage grid:
  `01_stage_grid_actual_exe.png`
- Korean shaft-pitting Pipeline Review with explicit NG reason:
  `02_pipeline_review_actual_exe.png`
- English lead-width stage grid and Pipeline Review:
  `lead_en\01_stage_grid_actual_exe.png`
  `lead_en\02_pipeline_review_actual_exe.png`
- English edge-processing stage grid and Pipeline Review:
  `edge_en\01_stage_grid_actual_exe.png`
  `edge_en\02_pipeline_review_actual_exe.png`

Every accepted capture report records:

- actual desktop EXE path and SHA-256;
- dynamically selected monitor bounds and intersecting window bounds;
- expected docked Layer and pane count;
- source image and Pipeline SHA-256;
- localized language display;
- completed Pipeline Review state.

The original five showcase reports identify this captured managed assembly:

`80C9C96C10B945F8FBD8779BB2FE2B88382EE9344F751E555C9FC9FF2B13165A`

The retuned shaft report identifies the current captured managed assembly:

`EAD689D0154B8C23BC2ECC69D6B9FC01546FF802EDCF2664DC0A9D22F1BBE40E`

The tracked README image is an exact copy of the final English hole-array
stage-grid capture. Its SHA-256 is:

`080E9CC9DEA5285419069E3CB20851A553B3F76511FF9999B03BCF3D98B177BB`

## Visual review result

- No clipped panel titles, controls, or image content were observed.
- The focused 1600x900 Pipeline Review smoke shows complete localized object
  table headers and values after including the document-docking host in the
  smoke visual-root search.
- Korean and English shell menus follow the requested application language.
- Blob, Contour, and LineDistance drawings are rendered on the source image,
  not on the binary preprocessing image.
- The hole-array result shows only the 34 intended holes.
- The shaft result is confined to the reviewed pitting ROI and rendered as NG.
  It preserves 18 dark-pit candidates, including smaller point-like candidates
  removed by the previous `Threshold=90` / `MIN_AREA=8 px^2` setup.
- The lead-width result shows a visible ROI and 16 repeated measurement lines.

Boundary: these examples prove the retained public sample/recipe executions and
the recorded UI workflow. The shaft sample has no independent pixel-level
ground truth, so its 18 retained candidates demonstrate the configured runtime
selection rather than recall, accuracy, or production calibration. None of the
examples proves field robustness for a different camera, lens, part, or dataset.

## Verification

- Embedded-smoke application build: 0 warnings, 0 errors.
- Retuned shaft `VisionRecipeRunnerSmoke`: expected NG with 18 runtime
  overlays and `ResultCount 18 > 0`.
- Retuned shaft actual desktop EXE capture scenario: PASS; five panes,
  Korean Pipeline Review, explicit NG reason, and monitor intersection were
  recorded.
- Focused Pipeline Review metrics smoke: PASS, 1600x900, layout/text/internal
  issue counts all zero.
- The source image, every generated stage image, Pipeline identity, source and
  Pipeline hashes, actual-EXE captures, and report are retained together under
  each capture folder.
