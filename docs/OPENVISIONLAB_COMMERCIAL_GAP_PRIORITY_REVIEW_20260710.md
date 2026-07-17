# OpenVisionLab Commercial Gap Priority Review

Updated: 2026-07-14 KST

This review compares OpenVisionLab against public official material for commercial machine-vision workbench products. The goal is not to expand OpenVisionLab into a camera/PLC/controller platform. The goal is to identify the operating features OpenVisionLab still needs so it can be useful as a rule-based OpenCvSharp4 recipe workbench even without LLM assistance.

## Current Product Identity

OpenVisionLab is an LLM-assisted OpenCvSharp4 rule-based vision recipe workbench.

Core workflow:

1. Load or choose sample images.
2. Choose or describe an inspection intent.
3. Build or import a rule-based XML recipe.
4. Validate layer routes, parameters, dependencies, and metrics.
5. Run Preview/Run Review only through explicit user actions.
6. Compare layers, Good/Bad samples, failed steps, metrics, ROI, templates, and parameters.
7. Save a validated recipe for learning, review, and later integration by another system.

Still out of scope:

- Camera acquisition setup
- Lighting control
- PLC/I/O
- Account/session platform
- Production deployment runtime
- Controller terminal software

Current maturity estimate from `docs/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`:

- Commercial equipment platform parity: about 25-30%.
- Intended LLM-assisted rule-based recipe workbench: about 62-66%.

This review keeps that estimate. Recent Learn Mode and MVVM cleanup work improves maintainability and operator learning, but it does not yet close the largest commercial workflow gaps.

## Official Sources Checked

- Cognex In-Sight EasyBuilder application steps and Inspect step:
  - https://docs.cognex.com/is_611/web/EN/ise/Content/EasyBuilder/AppSteps.htm
  - https://docs.cognex.com/isvs_2610/web/EN/InSight_EZ/Content/Topics/EZB/ezb-ui-inspect-step.htm
  - https://docs.cognex.com/is_574/web/EN/ezb/Content/EasyBuilder/Locate_AddDeleteEditTool.htm
- MVTec MERLIC product and recipe documentation:
  - https://www.mvtec.com/products/merlic
  - https://www.mvtec.com/doc/merlic/5.6/manual/en-us/Content/Process_integration/Recipes/merlic_recipes.html
  - https://www.mvtec.com/doc/merlic/5.5/manual/en-us/Content/Process_integration/Recipes/creating_recipe_file.html
- NI Vision Builder AI tutorial:
  - https://download.ni.com/support/manuals/373379k.pdf
- Zebra Aurora Vision Studio:
  - https://www.zebra.com/us/en/products/oem/software/aurora-vision-studio.html
- KEYENCE XG-X and CV-X official pages:
  - https://www.keyence.com/products/vision/vision-sys/xg-x/
  - https://www.keyence.com/support/user/cv-x/code/

## Commercial Patterns To Emulate

### 1. Guided Inspection Setup

Commercial tools do not begin with a raw tool list. They guide the operator through a job structure such as image setup, locate/fixture, inspect, pass/fail, and run/review.

OpenVisionLab should emulate this as a local recipe-building guide:

- Choose intent: presence/absence, count, defect blob, contour shape, pin gap/pitch, template match, feature match, brightness/mean, color/HSV.
- Choose sample and optional Good/Bad counterpart.
- Choose ROI/template/measurement region when required.
- Lock the recommended tool family.
- Generate a starter pipeline from built-in templates.
- Show required metrics and Good/Bad gates before the user runs.

Do not emulate camera connection, I/O, or live deployment steps.

### 2. Image-Centered Parameter Teaching

MERLIC and Cognex-style workflows keep the image central while parameters and regions are set directly against visible image evidence.

OpenVisionLab already has viewer, ROI, template editor, overlays, and PropertyGrid tools. The gap is the "why this parameter matters" layer:

- Measurement tools need calibration/scale context before metric gates are trusted.
- Blob/Contour tools need clearer count/area/bounds failure explanations across Good/Bad samples.
- Matching-family tools need a stronger template/ROI/readiness checklist before score tuning.
- Learn topics should link directly to sample, tool, metric, and explicit run review.

### 3. PASS/FAIL Result Review With Reason

NI Vision Builder AI separates configuration and inspection interfaces and reports per-step PASS/FAIL, measurements, comments, and inspection statistics.

OpenVisionLab has Recipe Manager review, failed-step focus, Good/Bad cards, run history, and result channels. The gap is the consolidated operating view:

- One result board should show final OK/NG, failed step, expected/actual metric, likely parameter family, and next action.
- Run history should summarize repeated Good/Bad runs with pass rate, runtime, and recent failures.
- The operator should not need to inspect multiple tabs to understand why a recipe failed.

### 4. Recipe Parameter Sets And Dependency Visibility

MERLIC recipes can reuse an application with different parameter settings. OpenVisionLab has recipes, pipelines, XML import/export, and dependency checks, but still needs clearer variant management:

- Recipe variants should clearly show which parameters differ.
- Dependency status should be visible before import/run: template path, sample path, missing image, expected catalog row.
- A recipe package/export should include XML plus dependency manifest, not just the raw XML.

This remains a workbench packaging feature, not production deployment.

### 5. Tool Palette Search And Readiness

Cognex and Aurora-style tools emphasize searchable tools/filters and ready-made inspection blocks.

OpenVisionLab has many tools and Learn topics, but the operator still needs to know which tool to pick. Missing:

- Searchable tool palette by intent keyword.
- Recent/favorite tools.
- Recommended next tool from sample/intent context.
- Readiness badges: needs image, needs ROI, needs template, needs second input, ready to preview.

### 6. Flow Debug And Offline Simulation

KEYENCE and NI emphasize flowchart/debug/simulation concepts. OpenVisionLab should not become a controller simulator, but it should improve recipe debug:

- Step graph should show ready/wait/failed/disabled state.
- Dry validation should show which step cannot run before executing.
- Branch/fan-out should be visually understandable from the selected step.
- A failed step should expose "rerun this step with current parameter edit" and "compare before/after output" as first-class actions.

Some of this already exists; the missing part is a single predictable debug surface.

## Current Strengths

- Product boundary is clear: rule-based recipe workbench, not equipment platform.
- PropertyGrid-based tool editing is preserved.
- Viewer/layer/docking/ROI/template foundations exist.
- Explicit Preview/Run and no-auto-run contracts are documented and smoke-protected.
- Recipe Manager already has XML validation, issue rows, dependency hints, diff review, Good/Bad review, failed-step focus, selected-step details, and branch/output comparison.
- Public-safe synthetic sample catalogs and Good/Bad gates exist.
- LLM XML authoring guide/catalog exist, though real external transcripts are still missing.

## Current Gaps

1. The initial non-LLM guided setup contract is implemented for all five starter intents.
   - A separate Guided setup tab now lets the operator choose an intent, see the selected sample and readiness text, create a deterministic Starter XML draft, and inspect the draft without an LLM.
   - Pin gap/pitch, Blob count, Contour shape, Matching target presence, and Mean brightness now expose their actual input fields plus `READY`/`MISSING` validation in the standalone tab.
   - Remaining P1 refinement is evidence-driven linkage to existing Learn/sample/template-editor surfaces, not another starter intent or a second XML generator.

2. Measurement calibration now has a bounded Guided setup contract and public Pin gap sample proof.
   - Pin gap now exposes `MM-READY`, `PX-ONLY`, and `MISSING` states, including mm-to-px gate conversion and px-only XML without a physical-unit claim.
   - Latest-EXE smoke proves Good OK / Bad NG parity between generated mm and px-only pipelines on the public synthetic pin pair. Do not expand this to other measurement entry points until a real workflow exposes the same gap.

3. Result review now has a report-first summary baseline.
   - The Report tab places final status + failed step, expected/actual metric evidence + parameter/layer route, and the next explicit action in one top summary band.
   - Existing checklist, result-channel cards/list, failure review, and copyable report remain available below it; further P3 work should require a real long-text or multi-sample failure case.

4. Recipe variants and dependency packaging remain incomplete.
   - Active-versus-selected variant comparison and parameter diffs are now visible in Recipe Manager without activating or running the selected variant.
   - Dependency manifest packaging still needs a clear, explicit review/handoff workflow beyond raw XML export.

5. Tool discovery is still too expert-oriented.
   - Learn Mode helps, but tool search/recommendation/readiness is not yet commercial-grade.

6. Flow debug is present but not yet unified.
   - Branch/output comparison and failed-step focus exist, but the operator does not yet get one integrated debug graph/state board.

## Reassessment 2026-07-11

The following official product documentation was rechecked against the current Dev build:

- Cognex In-Sight EasyBuilder's Inspect step describes a job-centered tool list where tools can be added, edited, deleted, reordered, and copied: https://docs.cognex.com/isvs_2530/web/EN/InSight_EZ/Content/Topics/EZB/ezb-ui-inspect-step.htm
- MVTec MERLIC documents recipe files as reusable parameter sets that can be imported, switched, and validated before runtime use: https://www.mvtec.com/doc/merlic/5.7/manual/en-us/Content/RTE/Setup/Recipes/merlic_recipes.html and https://www.mvtec.com/doc/merlic/5.7/manual/en-us/Content/RTE/Setup/Recipes/import_recipe.html
- NI Vision Builder AI describes a menu-driven environment for configuration, benchmarking, and deployment; its training outline also calls out results viewing, step-over/breakpoints, disabled steps, image/data logging, and performance measurement: https://www.ni.com/en/shop/electronic-test-instrumentation/application-software-for-electronic-test-and-instrumentation-category/what-is-vision-builder-for-automated-inspection.html and https://www.ni.com/en/shop/services/education-services/customer-education-courses/developing-machine-vision-systems-with-vbai-course-overview.html
- KEYENCE CV-X describes an icon-driven tool catalog/application navigator, stored-image retesting through simulator software, and a Vision Database for long-term image/result storage: https://www.keyence.com/products/vision/vision-sys/ and https://www.keyence.com/support/user/cv-x/code/

Current evidence-based interpretation:

- OpenVisionLab already covers the review evidence that matters for this workbench: explicit Preview/Run, Good/Bad checks, failed-step links, layer navigation, branch/output comparison, run history, and catalog benchmark.
- The first five minutes without an LLM now have deterministic starter coverage for Pin gap, Blob, Contour, Matching, and Mean. The next product-fit gap is calibration confidence for measurement recipes.
- The next commercial lesson is therefore not camera, PLC, controller, or deployment parity. It is a deterministic setup flow that makes tool choice, required inputs, and readiness visible before XML validation and explicit execution.
- Simulator/database features are useful only as offline sample replay, benchmark history, and evidence export inside this workbench. Hardware acquisition, production runtime, and controller terminal behavior remain out of scope.

## Priority Decision

### P1. Non-LLM Guided Inspection Setup

Build a `Guided Inspection Setup` surface that creates starter recipes from built-in intent templates without using an external LLM.

Why first:

- It directly answers the core product risk: "Can OpenVisionLab be useful without LLM?"
- It borrows the best part of commercial tools, the guided setup flow, without expanding into hardware.
- It reuses existing PropertyGrid tools, sample catalog, Recipe Manager, and Learn topics.

Completed first bounded slice (2026-07-11):

- Added a separate `Guided setup` tab to Recipe Manager.
- Reused the existing deterministic starter pipeline builder instead of adding a second XML generator.
- Exposed inspection intent, selected sample, readiness guidance, Starter XML creation, draft text, and the no-auto-run boundary in one view.
- Added current-source screenshot smoke assertions and kept Pipeline as the default tab so existing Recipe Manager entry behavior is preserved.

Completed second bounded slice (2026-07-11):

- Added standalone Pin gap inputs for ROI samples, nominal min/max distance, range gate, and mm/px scale.
- Added standalone Blob inputs for ROI, threshold, count range, and area range.
- Added `READY`/`MISSING` validation and disabled Starter XML creation while required inputs are invalid.
- Routed the shared Guided setup action to the existing Pin gap or Blob generator and verified that neither path triggers Preview/Run.
- Added current-source Pin gap/Blob state checks and latest-EXE direct smoke evidence.

Completed third bounded slice (2026-07-11):

- Added standalone Contour ROI, threshold, count range, and area range inputs.
- Added `READY`/`MISSING` validation and routed the shared action to the existing Threshold + Contour ResultCount/AreaMax generator.
- Verified invalid area input blocking and no Preview/Run side effects in current-source and latest-EXE smoke paths.

Completed fourth bounded slice (2026-07-11):

- Added standalone Matching template path, search ROI, `SCORE_MIN`, and expected count inputs.
- Added `READY`/`MISSING` validation for an existing template dependency, ROI format, 0..1 score, and positive expected count.
- Added a deterministic Matching target-presence skill that uses `SCORE_MIN` for candidate filtering and exact `ResultCount` acceptance.
- Kept Starter XML creation draft-only and added current-source/latest-EXE checks proving no Preview/Run side effects.

Completed fifth bounded slice (2026-07-11):

- Added standalone Mean optional ROI, `Mean`/`MeanStdDev` type, and Min/Max GV inputs.
- Added `READY`/`MISSING` validation for optional ROI syntax, supported mean type, 0..255 GV bounds, and Min <= Max ordering.
- Added a deterministic Mean brightness-drift skill with `MeanValueAvg` acceptance gates and explicit full-image behavior when ROI is blank.
- Completed current-source/latest-EXE regression coverage for all five non-LLM Starter intents without Preview/Run side effects.

Completed sixth bounded slice (2026-07-11):

- Added `MM-READY`, `PX-ONLY`, and `MISSING` Pin gap calibration states directly in standalone Guided setup.
- Positive mm/px produces `DistanceMmAvg` + `DistanceMmRange` gates and shows px equivalents; blank scale produces `DistancePxAvg` + `DistancePxRange` gates with `PIXELPERMM=0` and no physical-unit claim.
- Invalid nonblank scale and mm acceptance metrics with zero calibration are blocked, while Starter XML creation still does not trigger Preview/Run.

Next bounded slice:

- Completed: expose active-versus-selected pipeline step/routing/parameter diffs in the Pipeline Review tab without activation or Preview/Run.
- Next: audit an explicit XML review-bundle/dependency-manifest export action that reuses existing dependency scanning; do not silently add files to normal XML export and do not build deployment packaging.
- Keep the current LLM XML surface as the optional prompt/correction path.
- Keep the guide document and in-app plan for 5 starter intents:
  - Threshold + Blob count
  - Threshold + Contour shape
  - LineDistance pin gap/pitch
  - Matching target presence
  - Mean brightness drift
- For each intent define:
  - required user inputs
  - locked tool family
  - starter XML shape
  - required metrics
  - Good/Bad sample route
  - explicit Preview/Run Review action

Implementation should be template-backed and deterministic. It must not call LLM, auto-run tools, or mutate layers by itself.

### P2. Measurement Calibration And Unit Review

Make scale and measurement confidence visible before LineDistance/LineGauge recipes are accepted.

Needed work:

- Add a calibration/scale review card for `PIXELPERMM`.
- Show px and mm metric gates together.
- Require consistency gates for pin gap/pitch/clearance: `DistancePxRange`, `DistanceMmRange`, `DistancePxMax`, or `DistanceMmMax`.
- Add Learn + sample flow for measurement setup.

Completed baseline (2026-07-11):

- Guided Pin gap setup exposes `MM-READY`, `PX-ONLY`, and `MISSING` without adding a hardware calibration wizard.
- mm gates show px equivalents; px-only XML uses `PIXELPERMM=0` and does not emit mm metrics.
- Public Pin Good/Bad direct smoke proves unit parity and expected outcomes in both modes.

### P3. Consolidated Result Board

Create one operator-facing board for final recipe judgement.

Needed work:

- Final OK/NG
- Failed step
- Expected metric vs actual metric
- Likely parameter family to inspect
- Layer/output evidence
- Next explicit action
- Copyable operator report

This should reuse existing Recipe Manager result-channel data rather than adding a new result model.

Completed baseline (2026-07-11):

- Added one report-first summary band backed by the existing `Inspection.Status`, `Inspection.FailedStep`, metric evidence, and next-action properties.
- Kept the detailed validation checklist, five result channels, failure review, and copyable operator report unchanged.
- Current-source screenshot smoke verifies final NG, failed step, expected/actual metric, parameter family, layer route, and next action without clipping or overlap.

### P4. Recipe Variant And Dependency Manifest

Improve recipe operational handling without becoming a deployment platform.

Needed work:

- Parameter-set diff between recipe variants. Completed first operator-visible slice on 2026-07-11.
- Dependency manifest for XML export.
- Missing template/image dependency checklist before import/run.
- Recipe package folder export for review and handoff.

### P5. Tool Palette Search And Readiness Badges

Improve tool discovery after the guided setup path exists.

Needed work:

- Search tools by keyword and intent.
- Recent/favorite tools.
- Readiness badges: image, ROI, template, second input, output layer.
- Link each tool to its Learn topic and public sample path.

### P6. Unified Flow Debug Board

Improve debug visibility once P1-P3 are in place.

Needed work:

- Step state: ready, missing input, disabled, failed, passed.
- Branch/fan-out graph from selected step.
- Step-level rerun and before/after comparison affordance.
- Runtime/tact summary and repeated-run statistics.

## Deferred Or Out Of Scope

Do not prioritize these unless the product direction explicitly changes:

- Camera setup UI
- Lighting control
- PLC/I/O mapping
- Controller simulation
- HMI/runtime deployment
- Account/user permission model
- Deep learning training platform
- Full HALCON/VisionPro-scale algorithm breadth

## Next Development Recommendation

Next concrete development should start with P1.

Recommended first implementation slice:

1. Create `docs/OPENVISIONLAB_GUIDED_INSPECTION_SETUP_SPEC.md`.
2. Define the 5 initial non-LLM starter intents.
3. Add readiness checks ensuring each starter intent maps to an existing supported ToolType, Learn document, sample path, and known metric.
4. Only after the spec/checks pass, add a small Recipe Manager or Learn Mode entry point that opens the guided setup surface.

This avoids building a broad wizard before the intent contracts are stable.

## Reassessment 2026-07-14

This reassessment uses the current dirty Dev workspace rather than the 2026-07-10 baseline. The solution builds with 0 warnings and 0 errors, readiness/external-reference/public-sample checks pass, and the public catalog reports 28 rows, 226 manifest assets, and 14 pipelines. The broader product catalog still contains 184 rows.

The maturity estimate remains:

- About 25-30% versus broad commercial equipment platforms, intentionally.
- About 62-66% versus the intended LLM-assisted rule-based recipe workbench.

Recent work closes more of guided setup, Learn-to-tool navigation, Matching-family semantics, report-first review, validation-suite history, and Run History next-action visibility. It does not justify a higher overall estimate because several core workbench capabilities are still absent.

### Current Commercial Lessons

- Cognex In-Sight describes fixture-based location as the prerequisite when part position or orientation changes, and its current Job Validation flow compares expected and actual results on stored Good/Bad images.
- MERLIC 5.8 exposes recipe, referenced-app, image-source, parameter, result, referenced-file, and custom-tool information together and reports missing referenced components at the affected item.
- NI Vision Builder AI supports explicit pass/fail image-batch validation and per-inspection/per-step timing benchmarks.
- Aurora Vision Studio provides task-oriented and advanced filter search, explicit Run/Iterate/Step Over/Step Into, output data previews, and breakpoints. Zebra Aurora Design Assistant adds runtime statistics, profiling, and project change validation.
- KEYENCE VisionDatabase links stored images and results for search, replay, setting adjustment, backup, and analysis.

OpenVisionLab should emulate the offline authoring, validation, evidence, and recovery behaviors only. Camera setup, lighting, PLC/I/O, controller runtime, HMI deployment, account, and production traceability-server scope remain excluded.

### Revised Major Development Backlog

#### P1. Fixture And Coordinate-Frame Workflow

Current source has Matching, ROI, RotateScale, and overlays, but no named fixture/coordinate-frame contract that transforms downstream ROI, template, line, or measurement geometry when a part translates or rotates.

Required product outcome:

- A PropertyGrid-based locate/fixture tool or profile that publishes X/Y/angle and optional scale.
- Downstream steps explicitly select a fixture frame; the original image and input routing remain unchanged.
- ROI and measurement overlays are transformed for display and execution without silently rewriting recipe values.
- Shifted/rotated Good/Bad samples verify position error, angle error, downstream metric stability, and failure behavior.
- Preview/Run remains explicit.

Do not implement this as a generic geometry framework first. Define one Matching-based fixture workflow, parameters, metrics, XML shape, public sample, and smoke before generalizing.

First runtime proof completed on 2026-07-14:

- Added a translation-only named frame published by one `Matching` result with `NUM_MATCH=1`.
- Added explicit downstream opt-in that translates one runtime `CvROI` without rewriting the saved recipe value or changing input routing.
- Added fixture pose/offset/effective-ROI metrics and fail-closed rules for angle drift, missing/duplicate frames, source-layer mismatch, multi-ROI, masks, and invalid effective ROI.
- Added `tools/OpenVisionFixtureSmoke`; its shifted image is NG without fixture and OK with fixture while the saved ROI remains unchanged.
- Added Matching-producer and Blob-consumer Recipe Manager PropertyGrid fields with Korean labels, XML parameter round trip, unchanged ROI/routing assertions, and current-source before/after evidence in `artifacts/fixture_property_grid_roundtrip_20260714`.
- The bounded P1 v1 now also has a public shifted Good/Bad sample, current-EXE Recipe Manager/Pipeline Review proof, and an explicit post-review reference-pose save action. The next evidence gate is a real operator pass on the chosen reference image; rotation/scale compensation remains deferred until a real failing sample proves the translation-only contract insufficient.

#### P2. Tool Finder And Readiness Map

The compact Tool rail and Learn links work. A first bounded readiness state now exists, but tool search and tool-specific prerequisite states remain incomplete.

Required product outcome:

- Search by intent, tool name, parameter term, and expected result, using the existing canonical ToolTypes and aliases.
- Task-oriented categories such as preprocess, locate, inspect, measure, compare, and review.
- Readiness states such as image missing, ROI optional/required, template missing, second input missing, calibration missing, output target ready, and ready to Preview.
- Direct links to the existing Tool View, Learn topic, public sample, and Guided setup intent.

Defer favorites and recommendation scoring until real usage shows search alone is insufficient.

First bounded readiness slice completed on 2026-07-14:

- The existing 15 image-processing/algorithm Tool rail items show `입력 없음` when `Main` has no image and `설정 가능` after a Main image is loaded.
- This is descriptive state only. Tool selection remains available, compact icons remain clickable, and readiness changes do not run Preview/Run, create layers, or change routing.
- `설정 가능` deliberately does not mean `ready to Preview`; template, second-input, ROI, calibration, and output-target states still need canonical per-tool metadata.
- Current-source before/after evidence and focused smoke are under `artifacts/tool_readiness_20260714`; the latest Debug EXE `workspace-startup-empty` smoke also passed there.
- The second bounded slice now marks `Matching` with a canonical template prerequisite and reads the existing recipe-owned `MatchingProperty` template status. With `Main` ready and no valid loaded template, the row shows `템플릿 필요`; registering or clearing the template refreshes the row through the existing PropertyGrid save path.
- The transition is descriptive only and is smoke-proven not to increase Preview/Run count. Current-source before/after evidence is under `artifacts/matching_template_readiness_20260714`.
- The third bounded slice applies the same contract to `EdgeBasedMatching` and `FeatureMatching`. All three template-backed matching rows now distinguish missing templates, and Edge/Feature save-event transitions are smoke-proven without opening tools or increasing Preview/Run count. Evidence is under `artifacts/matching_family_template_readiness_20260714`.
- The fourth bounded slice applies the existing Arithmetic execution contract to Tool rail readiness. Operations that require B show `B 입력 필요` until a second non-placeholder image layer exists, while unary, constant-input, and Offset settings remain configurable. Settings and layer transitions are smoke-proven not to open a tool or increase Preview/Run count. Evidence is under `artifacts/arithmetic_second_input_readiness_20260714`.
- The fifth bounded slice applies the existing Pin gap unit contract to Line readiness. Recipe-owned Line A/B values now distinguish `px 전용`, matching positive scales such as `mm 0.006`, and `보정 확인` for missing, invalid, negative, or inconsistent values. A matching positive value is not presented as proof of physical calibration.
- The Line scale transitions are smoke-proven through the existing PropertyGrid save notification without opening the tool or increasing Preview/Run count. Current-source before/after evidence is under `artifacts/line_scale_readiness_20260714`.
- Required-ROI audit found no generic Tool rail prerequisite that could be shown truthfully. Blob/Contour support full-image direct Preview, Line supplies its full-image default only after an explicit Preview action, and Matching-family readiness is template-based. Fixture-consuming pipeline Steps remain the real required-ROI case and already fail closed in pipeline validation, so no misleading generic ROI badge was added.
- The sixth bounded slice adds inline Tool rail search over canonical bilingual tool, inspection-intent, PropertyGrid parameter, and result-metric terms. Multi-token queries use AND matching, preserve the existing item/readiness instances, and show the visible result count plus an explicit clear action.
- Search is display-only and is smoke-proven not to open a Tool View, run Preview/Run, create layers, or change workspace/input/output routing. The compact icon rail hides the search row and remains usable. Current-source evidence is under `artifacts/tool_finder_search_20260714`.
- The seventh bounded slice exposes a book button beside found tools and maps all 16 Tool rail menus to existing canonical Learn topics. It opens or reuses the Learn window at the requested topic and is smoke-proven not to select/open the Tool View, run Preview/Run, create layers, or change workspace/input/output routing. Current-source evidence is under `artifacts/tool_finder_learn_link_20260714`.
- The eighth bounded slice exposes an image button beside found tools and reuses each Learn topic's existing `PracticePathId` to open the current Sample Picker. All 16 mappings are asserted, the Line action is proven to select `line` with visible public samples, and cancelling is proven not to load a sample, open a Tool View, run Preview/Run, create layers, or change workspace/input/output routing. Current-source evidence is under `artifacts/tool_finder_sample_link_20260714`.
- The ninth bounded slice exposes Guided Setup only for the five tools with an existing starter-intent contract: Line, Blob, Contour, Matching, and Mean. It selects the mapped intent in the existing Recipe Manager Guided Setup tab without creating XML or changing Tool View, Preview/Run, layer, workspace, or route state; the other 11 tools expose no button. Search mode hides the readiness badge so all three direct actions remain inside the Tool rail, and the smoke checks the button's real bounds. Current-source evidence is under `artifacts/tool_finder_guided_setup_link_20260714`.
- P2 Tool Finder is complete at the current bounded scope. Favorites and recommendation scoring remain deferred until search usage proves they are needed. P3 now has a first explicit Recipe review bundle and dependency manifest built from existing validation/review data.

#### P3. Explicit Recipe Review Bundle And Dependency Manifest

OpenVisionLab validates draft dependencies and copies an LLM review bundle, but source search found no exportable dependency manifest or recipe review package.

Required product outcome:

- One explicit export command separate from normal XML export.
- XML, dependency manifest, referenced template/sample list, relative-path status, file size/hash, ToolType/version summary, acceptance metrics, and validation status.
- Optional explicit copy of redistributable referenced files into a review folder; never silently package private/local assets.
- Import-side dry validation and relocation review before explicit import.

This is a local review/handoff package, not production deployment packaging.

Current bounded implementation on 2026-07-14:

- Recipe Manager exposes an explicit `.review.zip` export beside, but separate from, normal XML export.
- Schema v1 includes only `pipeline.xml` and `review-manifest.json`.
- The manifest includes application/schema version, XML size/SHA-256, validation state, ToolType counts, Step routes, acceptance metrics, and referenced pipeline/sample/reference path kind, existence, size, and SHA-256.
- Referenced files remain `ReferencedOnly`; no private/local asset is copied and export does not import, Preview/Run, create layers, or change routing.
- Focused ZIP/content/no-side-effect smoke and current-source 1600x900 Recipe Manager evidence are under `artifacts\recipe_review_bundle_20260714`.
- Import-side dry validation now consumes `.review.zip` in the existing `LLM XML` review tab. It rejects extra/duplicate/oversize entries, unsupported schema/policy, XML size/SHA mismatch, and manifest/XML dependency mismatch without extracting files.
- Missing absolute dependencies expose only one deterministic bundle-adjacent candidate when its size/SHA evidence matches. The candidate remains review-only; XML is not rewritten and validation stays NG until the operator explicitly changes the path and validates again.
- Validation NG disables `Import`. Bundle load/import selection does not save or activate a pipeline, copy a dependency, run Preview/Run, create a layer, open a Tool View, or change workspace/routing.
- Focused before/after evidence and tamper/no-side-effect smoke are under `artifacts\recipe_review_bundle_import_20260714`; latest-build direct EXE `recipe-manager-tabs` passed under `direct_exe_final`.
- P3 is complete at the reference-only review/handoff scope. Optional explicit redistributable-file copy remains deferred and must be designed around opt-in asset classification; P4 user-defined local Validation Sets is the next large operator workflow.

#### P4. User-Defined Local Validation Sets

Validation Suite already supports selected sample, Good/Bad pair, full catalog, saved history, NG filtering, and baseline regression comparison. Do not rebuild that flow.

Required product outcome:

- Register a local image folder or explicit image list as a recipe-local validation set.
- Assign expected OK/NG and optional notes without adding assets to the public catalog.
- Reuse the existing explicit suite runner, result matrix, failure-step actions, and summary storage.
- Preserve missing-file diagnostics and make moved local paths repairable.

Synthetic brightness/rotation/noise variation must be labeled as a stress test and must not replace real sample evidence.

Completed first operator slice (2026-07-14):

- Recipe Manager `Pipeline > Runs` now creates named recipe-local sets and registers explicit multi-selected image files as expected OK or NG with optional notes.
- Schema v1 is isolated under `VISION\ValidationSets`; it does not add local files to public/product catalogs or expose the metadata XML as a pipeline.
- Missing files remain listed and block suite execution. An invalid metadata XML is preserved and blocks edits instead of being overwritten.
- Explicit Local set execution reuses the selected pipeline, sample-check service, batch history, NG filtering, failed-step actions, and baseline comparison. Registration and metadata edits preserve Preview/Run, layers, workspace, and routing.
- `wpf_shell_host_recipe_local_validation_set` proves one OK/one NG registration, missing-file fail-closed behavior, restored-path execution, saved suite metadata/results, no runtime side effects, and non-overlapping current-source UI. Latest Debug EXE `recipe-manager-tabs` proves the controls in the actual app.
- Pipeline inventory reliability follow-up on 2026-07-14 filters the shared legacy `VISION` directory by the exact `VisionPipeline` document root, so tool-state and malformed XML no longer appear as pipelines. The files remain untouched; focused and latest-EXE smokes cover storage and visible UI inventory.
- The second operator slice on 2026-07-15 adds explicit top-level folder registration as expected OK or NG. Supported root images are registered in deterministic order, unsupported files are ignored, subfolders are excluded, and registering the same paths again updates their role/notes without duplicates.
- Folder registration reuses the same schema, suite/history runner, and no-side-effect contract as explicit file registration. `wpf_shell_host_recipe_local_validation_set` covers top-level filtering, role updates, missing-file blocking, suite results, and current-source layout; latest-build direct EXE `recipe-manager-tabs` records the same file/folder and no-side-effect contract under `artifacts\validation_set_folder_registration_20260715\direct_exe`.
- The third operator slice on 2026-07-15 adds repair for one selected missing image. The operator chooses one existing supported replacement; an already-registered replacement is rejected, and successful repair changes only the path while preserving expected OK/NG and notes.
- Focused before/after evidence is under `artifacts\validation_set_path_repair_20260715\before` and `after_screen`. Latest-build direct EXE `recipe-manager-tabs` records file/folder/repair controls plus preserved metadata, Preview/Run, layers, and routing under `artifacts\validation_set_path_repair_20260715\direct_exe`.
- P4 is complete at the bounded recipe-local Validation Set scope. Recursive search, inferred replacement, automatic path rewriting, and a second runner remain prohibited.

#### P5. Unified Pipeline Debug And Replay Board

Selected-step flow, branch/output comparison, layer navigation, failed-step focus, and rerun actions exist, but they remain distributed across review surfaces.

Required product outcome:

- One pipeline map with ready, missing-input, disabled, passed, failed, and not-run states.
- Selected-step producer/consumer and fixture-frame context.
- Explicit Run selected, Run to step, and rerun-from-step commands only where runtime semantics can be preserved.
- Input/output/layer preview comparison and parameter/metric evidence in the same review flow.
- No breakpoint or selection action may auto-run the pipeline.

Reuse the current Pipeline Review document and branch model instead of adding a second graph subsystem.

2026-07-15 evidence audit:

- The existing Pipeline Review already shows selected-step flow, branch context, input/output previews, validation, per-step run results, first-issue navigation, fixture context, and one explicit full `Run Review` command.
- `OpenVisionPipelineReviewDocument.ResolveFlowStatus` currently maps an enabled Step with no result and no output to `Waiting` regardless of whether its input is available. Therefore a genuinely missing external input and a normal not-yet-run Step are visually indistinguishable.
- The first bounded P5 slice should add a producer-aware `Missing input` state only: an absent input is not missing when an earlier enabled Step is expected to produce that layer. It must remain read-only and must not add partial-run semantics or selection-triggered execution.

First bounded P5 slice completed on 2026-07-15:

- Pipeline Review now shows `입력 없음` only when an enabled Step has no input image and no earlier enabled producer for that layer.
- A downstream input that an earlier enabled Step will produce remains `WAIT`; its guide tells the operator that explicit Review will create the input instead of asking them to load it manually.
- Missing-input and produced-input Step selection remain read-only and preserve native Preview count, layers, and routing.
- Focused before/after evidence is under `artifacts\pipeline_review_input_state_20260715\before_final` and `after_final_valid`; `wpf_shell_host_pipeline_review` also covers the producer-wait path.
- Partial-run, run-to-step, and rerun-from-step semantics remain deferred until execution-state preservation can be proved.

#### P6. Benchmark And Regression Analytics

Current batch summaries store total/pass/fail counts and per-sample elapsed time. They do not provide commercial-style aggregate or per-step performance analysis.

Required product outcome:

- Average, median, p95, maximum, and failure-rate summaries for repeated runs.
- Per-step elapsed-time contribution from existing run reports.
- Same-validation-set comparison between active and selected recipe variants.
- Configurable regression gates for result outcome and tact time.
- Clear separation between correctness gates and performance telemetry.

Completed first bounded P6 slice (2026-07-15):

- Audited `VisionPipelineBatchRunSummaryStorage`, Validation Suite execution, saved Run History options, baseline comparison, and `VisionPipelineRunReportStorage`.
- Confirmed that batch summaries already persist per-sample `TotalMilliseconds`; the selected saved run now derives failure rate, average, median, nearest-rank p95, and maximum without changing the storage schema.
- Reused the existing benchmark comparison summary instead of adding another panel. It labels judgement failure rate separately from performance timing and remains read-only.
- Confirmed that `VisionPipelineBatchSampleRunResult.RunReportPath` is currently not populated by the sample-suite path. Per-Step batch timing is therefore deferred rather than inferred from unlinked reports.
- Current-source evidence is under `artifacts\p6_benchmark_analytics_20260715\before` and `after_final`; latest-EXE evidence is under `artifacts\p6_benchmark_analytics_20260715\direct_exe_final`.

Completed second bounded P6 slice (2026-07-15):

- Selected-sample, Good/Bad pair, Catalog, and Local Validation Set runs now persist distinct suite kinds instead of relying on one generic batch label.
- Average and p95 deltas compare baseline to current only when suite kind/name and the exact sample-image multiset match and every saved row has a valid timing.
- A different suite or sample set explicitly skips performance comparison while preserving independent outcome rows such as `Regression`, `Recovered`, and `Still NG`.
- Reused the existing Run History summary; no new panel, schema, telemetry service, or regression threshold was added.
- Current latest-EXE before/after evidence is under `artifacts\p6_baseline_timing_comparison_20260715\before_exe_01` and `after_exe_04`. `after_exe_01` and `after_exe_03` contain black WPF composition artifacts and are not valid UI evidence; `after_exe_02` predates the final empty-suite guard.
- Per-Step timing remains the next evidence gap because `RunReportPath` is still not populated by the suite execution path.

#### P7. Local Recipe Revision And Recovery

XML save uses temporary-file replacement and invalid-XML backup, but there is no operator-facing recipe revision history or last-known-good restore workflow.

Required product outcome:

- Local snapshots on explicit save/import/apply-back operations.
- Revision list with timestamp, operation, validation status, and concise diff.
- Explicit restore to a new current revision, followed by validation but no Preview/Run.
- Unsaved-change guard and crash-recovery draft where practical.

Do not add accounts, permissions, signatures, or server audit scope.

#### P8. Portable Evidence Report

Copyable operator text and run archives exist, but review evidence is not yet a portable beginner-friendly report.

Required product outcome:

- Local HTML report containing recipe/pipeline identity, sample role, final OK/NG, failed step, expected/actual metrics, key input/output images or overlays, tact summary, dependency status, next action, and source artifact paths.
- Optional CSV/TSV data export from the same model.
- No background logging service or production database.

#### P9. Learn Mode Practical Labs

Most tool concepts and Learn-to-tool links now exist. The remaining gap is guided practice, not more static chapters.

Required product outcome:

- Complete the planned inspection workflow, workspace/layer, Good/Bad validation, Recipe Manager, LLM XML, and troubleshooting topics.
- Convert representative topics into `concept -> animation -> sample -> Tool View -> explicit Preview/Run -> metric interpretation` labs.
- Track only local lesson progress/checkpoints; do not add accounts or cloud learning state.
- Prefer real OpenCvSharp visual stages where they improve understanding. Do not add animation as decoration.

#### P10. Sample-Backed Algorithm Expansion

Do not pursue HALCON/VisionPro-scale breadth. Add a tool only when its operator workflow, PropertyGrid parameters, XML shape, metrics, sample pair, and smoke are defined.

Candidate order:

1. Affine/perspective transform and calibration support needed by fixture/metrology workflows.
2. Contrast normalization/equalization for low-contrast rule-based inspection.
3. Golden-template or image-difference defect review.
4. Circle/arc or Hough-style geometric detection when a real measurement sample requires it.
5. Standalone corner/keypoint visualization only if FeatureMatching troubleshooting needs it.

Channel Split/Merge remains Learn-only until a separate operator result and acceptance contract exists.

#### P11. Real LLM Correction-Loop Proof

The in-app draft/validate/diff/dependency/import flow and replayable synthetic failure corpus exist. The missing evidence is one or more real external GPT/Gemini/Claude transcripts using the current authoring guide and tool catalog.

This priority is blocked until a real manual transcript or API execution is available. Do not fabricate evidence and do not spend implementation effort on another generic LLM panel before a transcript exposes a concrete gap.

#### P12. Desktop Release Hardening

CI, version policy, readiness, external-reference checks, public-asset checks, and release-evidence scripts exist.

Remaining productization work:

- Reproducible Release build/package and first-run verification.
- Installer or clearly versioned portable package only if distribution requires it.
- Upgrade/rollback and user-data compatibility checks.
- Crash/log collection instructions and a support bundle that excludes private recipe/sample assets by default.

This is desktop workbench distribution, not production runtime deployment.

### Recommended Execution Order

1. Stabilize and checkpoint the current dirty Dev changes.
2. Design and prove one Matching-based fixture workflow before implementing a general coordinate-frame abstraction.
3. Deliver Tool Finder/readiness using the existing ToolType/Learn/sample metadata.
4. Deliver the explicit dependency manifest/review bundle.
5. Extend the existing Validation Suite with user-defined local sets.
6. Build unified debug/replay and benchmark analytics by reusing existing Pipeline Review and run archives.
7. Add revision recovery and portable HTML evidence.
8. Complete practical Learn labs and add only sample-backed algorithm tools.
9. Run the real LLM transcript track when its prerequisite becomes available.

### Official Sources Rechecked 2026-07-14

- Cognex inspection tool and fixture guidance: https://docs.cognex.com/is_611/web/EN/ise/Content/GettingStarted/BuildJob_SetupTools.htm
- Cognex In-Sight 26.1 Job Validation: https://docs.cognex.com/isvs_2610/web/EN/InSight_EZ/Content/Topics/Spreadsheet/validation_panel.htm
- MVTec MERLIC 5.8 recipe-management improvements: https://www.mvtec.com/doc/merlic/5.8/manual/en-us/Content/About_merlic/whats_new.html
- NI Vision Builder AI benchmarking and validation: https://www.ni.com/en/shop/electronic-test-instrumentation/application-software-for-electronic-test-and-instrumentation-category/what-is-vision-builder-for-automated-inspection/benchmarking-and-testing-inspections-in-vision-builder-for-autom.html
- Aurora Vision Studio running and analysis: https://docs.adaptive-vision.com/current/studio/getting_started/RunningAndAnalysingPrograms.html
- Aurora Vision Studio filter discovery: https://docs.adaptive-vision.com/current/studio/user_interface/FindingFilters.html
- Zebra Aurora Design Assistant utilities: https://www.zebra.com/us/en/software/fact-sheets/machine-vision-and-fixed-industrial-scanning-software/aurora-design-assistant.html
- KEYENCE VisionDatabase: https://www.keyence.com/products/vision/vision-sys/ca-h1db_ca-ad1/
