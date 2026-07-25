# OpenVisionLab Competitor Priority Review - 2026-07-01

This review compares OpenVisionLab against current public positioning from common machine-vision workbench products and fixes the next UX priority after MainView completion.

## Sources Checked

- Cognex In-Sight EasyBuilder: step-by-step job setup, tool add/edit/rearrange, I/O, and HMI configuration.
  - https://docs.cognex.com/isvs_2530/web/EN/InSight_EZ/Content/Topics/GettingStarted/getstarted_ez.htm
- MVTec MERLIC: no-code, image-centered configuration, standardized tools, traceable steps, all-in-one development/runtime, and industry integration.
  - https://www.mvtec.com/products/merlic
  - https://www.mvtec.com/products/merlic/features-tools
- NI Vision Builder AI: configuration/inspection mode split, inspection steps, PASS/FAIL results panel, display window, and inspection statistics.
  - https://download.ni.com/support/manuals/373379k.pdf
- Zebra Aurora Vision Studio: no-code graphical environment, ready-made filters, drag-and-drop workflow, custom HMI, create/integrate/monitor positioning.
  - https://www.zebra.com/us/en/products/oem/software/aurora-vision-studio.html
  - https://cdn.graftek.com/wp-content/uploads/2021/11/03164757/aurora-vision-studio.pdf

## Competitive Pattern

1. The strongest products make the inspection flow explicit: acquire/load image, add tools, set criteria, review pass/fail, deploy/run.
2. The image remains the center of the work; tool lists, parameters, logs, and result cards support the image instead of replacing it.
3. Samples are not just demos. They are benchmark references that explain what the recipe should accept or reject.
4. Beginner UX does not remove advanced tools. It layers guidance, pass/fail explanation, and next actions over the existing tool model.
5. Runtime/operator views and development views are conceptually separated, even when they live in the same product.

## OpenVisionLab Position

OpenVisionLab should continue as a layer-based, rule-based vision workbench backed by OpenCvSharp4 algorithms and model-driven PropertyGrid tools.

Current strengths:

- MainView beginner entry is now stable: empty/image-ready/sample-ready/tool-selected states are display-only and localized.
- Tool View guides cover Matching, EdgeBasedMatching, FeatureMatching, Blob, Contour, and Line without replacing PropertyGrid ownership.
- Pipeline Review shows step route, branch reason, OK/NG result, localized acceptance NG reason, run log, and input/output previews.
- Docking and layer comparison now follow a Visual Studio-style workspace model closely enough to leave in watch mode.

Current competitive gap:

- The sample catalog still reads partly like a picker. Competitor products position sample/inspection setup as a benchmark loop: what reference is loaded, what criteria will be judged, and what OK/NG behavior the operator should expect.

## Priority Decision

P1: Make the Sample Catalog read as a verification benchmark entry point.

Scope:

- Add a compact benchmark strip to the selected sample detail area.
- Show whether the selected sample is an OK reference, NG reference, or generic OK criteria sample.
- Show the acceptance criteria before the operator opens the sample.
- Preserve the current contract: opening a sample prepares `Main` and `Sample_` pipeline only; it must not run Preview/Run, open a tool, create output layers, or change routing by display alone.

Why this is first:

- MainView is complete, so the next beginner gap is not another home screen change.
- Pipeline Review already has OK/NG execution explanation; the missing piece is explaining the reference/benchmark intent before entering the review loop.
- This is low-risk because it affects display text in the sample picker and a focused smoke target already exists.

## Watch Items

- Do not turn sample opening into implicit Preview/Run.
- Do not replace PropertyGrid tools with wizard-only controls.
- Do not hide image/pipeline paths, expected metrics, Good/Bad pair context, check guidance, or NG fix guidance.
- Keep before/after screenshots for UI changes.

## 2026-07-01 Beginner Direction Update

The next UX direction is not more decorative MainView work. It is a beginner learning loop over the existing inspection model:

1. Sample-centered Learn Mode:
   - `Matching 배우기`
   - `Blob으로 얼룩/입자 찾기`
   - `Line으로 거리/각도 측정하기`
2. Tool presets:
   - `기본 검사`
   - `빠른 검사`
   - `정밀 검사`
   - Presets must update PropertyGrid-backed model properties through explicit commands.
3. Result explanation:
   - Translate score/count/angle/area metrics into OK/NG reasons and candidate-risk text.
4. Failure cause guidance:
   - Point to likely parameter families such as template crop, ROI, threshold range, score, edge count, and morphology size.
5. Good/Bad pair expansion:
   - Add only repeatable public pairs with clear metric margins.
6. Recipe context switching:
   - Support multiple explicit recipe contexts so different inspections can use different recipes without hidden global state mutation.

Detailed implementation plan: `docs\OPENVISIONLAB_BEGINNER_LEARN_MODE_AND_RECIPE_CONTEXT_20260701.md`.
