# OpenVisionLab Product Target And Main Views

Updated: 2026-07-06 15:04 KST

This is the short product-direction document for future sessions. Read this first when continuing OpenVisionLab work so the goal, view structure, completed areas, and next priorities do not need to be rediscovered.

## Final Product Shape

OpenVisionLab is an LLM-assisted OpenCvSharp4 rule-based vision recipe workbench.

It is not intended to become a camera, lighting, PLC, or I/O integration platform. Its primary value is the workflow before equipment integration:

1. Load or choose sample images.
2. Describe the inspection target and detection points.
3. Use GPT, Gemini, Claude, or another LLM to draft OpenVisionLab XML recipes/pipelines.
4. Load and validate the XML inside OpenVisionLab.
5. Verify the pipeline with OpenCvSharp4 rule-based tools.
6. Review Good/Bad samples, failed steps, metrics, layers, ROI, templates, and parameters.
7. Save a validated recipe for learning, review, and later integration by another system.

One-line product definition:

> OpenVisionLab is a desktop workbench for creating and validating OpenCvSharp4 rule-based inspection recipes from sample images, operator intent, and LLM-generated XML.

## Product Boundaries

Keep these boundaries stable:

- Algorithm tools stay PropertyGrid-based.
- Preview and Run stay explicit user actions.
- Boolean visibility toggles must not run Preview or Run.
- Layer create/delete/load-image actions must not auto-run tools.
- Creating an output layer must not silently change the selected input layer.
- Viewer zoom/pan/drag, ROI overlay, template editor, layer comparison, and docking remain product features.
- LLM assistance generates, explains, validates, and imports XML; it does not silently execute or auto-accept recipes.
- Camera/lighting/PLC/I/O screens are out of scope unless the user explicitly changes product direction.
- Top-level account/session UI is out of scope unless real login, user profile, permissions, or audit requirements are added. Keep operator guidance inside recipe/review workflows instead.
- Main window minimize, maximize/restore, and close controls are required window chrome and must remain visible even when account/session UI is removed.

## Main Views To Build Toward

### 1. Image Workbench View

Purpose: the main image/layer working surface.

Responsibilities:

- Load images into named layers.
- Create, delete, rename, and compare layers.
- Show viewer zoom/pan/drag, ROI overlays, pixel/GV status, and result layers.
- Keep image load/layer operations separate from Preview/Run.

Already stable enough to avoid broad rediscovery:

- Core viewer, layer tab, docking, layer comparison, zoom/pan/drag, ROI overlay, and no-auto-run contracts are already protected by stable feature contracts and focused smokes.

### 2. Tool Editor View

Purpose: edit and verify one rule-based algorithm tool.

Responsibilities:

- Keep PropertyGrid-generated parameter UI.
- Show compact teaching guides, result explanations, and next actions.
- Run Preview/Run only through explicit commands or documented auto-preview settings.
- Add verified steps to Pipeline.

Already stable enough to avoid broad rediscovery:

- Matching, EdgeBasedMatching, FeatureMatching, Blob, Contour, Line, Threshold, Filter, Morphology, Arithmetic, and SimplePreprocess have existing PropertyGrid/runtime/controller patterns.
- Tool code-behind cleanup has already moved repeated text/event/runtime responsibility into presenters/controllers/shared bases in several tool families, including single-input custom shells, Blob/Contour/Line single-input PropertyGrid shells, Matching-family single-input PropertyGrid shells, and the double-input Arithmetic custom shell.

### 3. Recipe / Pipeline Manager View

Purpose: manage recipes and inspect pipeline structure.

Responsibilities:

- List, search, create, duplicate, rename, delete recipes.
- Import/export XML.
- List pipelines within a recipe and activate/duplicate/rename/delete them.
- Show pipeline step list, step comparison table, selected-step route, acceptance gate, and full parameters.
- Make failed-step navigation and operator review obvious.

Already completed enough to avoid redoing:

- Recipe manager has searchable list, create/duplicate/rename/delete, XML import/export, draggable title area, close affordance, and a workbench-sized overlay layout with recipe library, review workspace, and command strip zones.
- Pipeline tab is split into review/history/XML-Step sub-tabs.
- Duplicate from sample, LLM XML validation report, structured validation issue rows, pipeline preview step list, step comparison table, selected-step detail panel, selected-step operator context, selected-step input/output layer thumbnail cards with click navigation, selected-step PropertyGrid parameter review with explicit XML apply-back and corrected-output review, selected Step branch/output comparison rows, and Good/Bad role failed-Step drill-down exist.
- Multi-step flow focus now exists: the Recipe Manager header shows the selected Step position, tool, route, previous context, and next context; the XML/Step tab also has a Step flow focus strip with explicit Previous/Next Step navigation that does not run Preview.
- Failed-Step rerun/comparison action strip now exists in the Review tab: selected failed Step text, input/output comparison route, direct output/input layer navigation, parameter review, and Good/Bad rerun.
- Operator decision board now exists in the Review tab: XML/Step, selected sample, Good/Bad, and next action are summarized above the longer operator review text.
- Operator handoff report now exists as a Pipeline review sub-tab, summarizing current recipe/pipeline/XML/sample/Good-Bad/failure-Step/next-action state for review and next-session transfer; it also has an explicit copy action and a compact result-channel board for `Inspection.Status`, `Inspection.FailedStep`, `Inspection.Evidence`, `Inspection.Benchmark`, and `Inspection.NextAction`.
- Run History now has an explicit selected-review copy action for sharing the selected saved run interpretation without rerunning checks.
- The Recipe Manager header now has an explicit guided next-action button that names and runs the current next existing action from the guided setup state without adding automatic Preview/Run.
- Recipe combo crash and old/private recipe cleanup were handled before these latest commits.

Next development focus:

- Continue density/layout polish only when current screenshots show actual clipping, overlap, or workflow friction.
- Make branch/output comparison broader only when real recipes expose multiple independent candidate correction paths beyond the current selected-step producer/consumer map.

### 4. LLM XML Assistant View

Purpose: use LLMs to draft and review OpenVisionLab XML safely.

Responsibilities:

- Build prompts from the selected sample, intended inspection goal, detection points, and tool templates.
- Load LLM XML drafts.
- Validate XML structure, tool names, layer routes, dependencies, and sample compatibility.
- Explain errors, warnings, missing files, and safe import behavior.
- Import only after explicit operator action.

Already completed enough to avoid redoing:

- LLM assistant fields, prompt creation/copy, XML starter creation, XML draft load, clipboard paste, validation, import draft, reference image, dependency copy report, dependency/path action hints, dependency path drill-down rows, draft import review, before/after diff review, inline validation report, and LLM correction review-bundle copy exist. The correction bundle includes selected Step operator context and failed-Step review text.
- LLM result-channel contract now exists in the in-app prompt/review/validation flow: `Inspection.Status`, `Inspection.FailedStep`, `Inspection.Evidence`, `Inspection.Benchmark`, and `Inspection.NextAction` are derived after validation and explicit runs, not emitted as XML nodes.

Next development focus:

- Expand dependency files and unresolved paths further only when real failure examples require more than the current row-level drill-down.
- Keep the LLM workflow explicit: validate, review diff/dependencies, then import. It must not run Preview or silently accept recipes.

### 5. Sample Review / Validation View

Purpose: prove recipes against real sample intent.

Responsibilities:

- Catalog product samples.
- Run Good/Bad pair checks.
- Show expected metrics, actual metrics, OK/NG status, failed Step, and likely correction path.
- Support counterpart sample switching.

Already completed enough to avoid redoing:

- Public product sample catalog policy, Good/Bad sample pairs, expected metric gates, product sample runner, quality audit, and sample review smokes exist.
- Recipe review shows Good/Bad role result cards after pair check, and a failed role can focus the failed Step with correction guidance without opening the saved run history first.
- Recipe review also exposes immediate failed-Step rerun/comparison actions after focus: output layer, input layer, parameters, and Good/Bad rerun.
- Product sample catalog quality has passed recent gates. Do not add more samples until current UX/runtime changes are stabilized.

Next development focus:

- Improve corrected-output review after a failed Step parameter edit.
- Make rerun and comparison paths more visible from Recipe/Pipeline Manager.

### 6. Template / ROI Editor View

Purpose: edit image regions and templates used by rule-based tools.

Responsibilities:

- Edit ROI, template, mask, and search regions.
- Keep ROI/template state attached to the PropertyGrid-backed tool model and XML.
- Show overlays in tool preview and image workbench.

Already stable enough to avoid broad rediscovery:

- ROI editor, template editor, active WPF Shell display context, and overlay behavior are protected by stable feature contracts.
- Recipe/Pipeline selected Step detail now shows ROI/template metadata, an explicit tool entry button, and an embedded PropertyGrid review/apply path for Step parameters.

Next development focus:

- Tighten native tool round-trip only when a real workflow needs edits made in the separate tool window to be pulled back into the selected pipeline Step. The current authoritative apply-back path is the Recipe Manager embedded PropertyGrid and explicit XML apply command.

## Current Strengths

- Clear product identity: rule-based OpenCvSharp4 workbench, not a full equipment platform.
- PropertyGrid-based tools preserve recipe/model compatibility.
- LLM XML workflow is a unique differentiator against common commercial equipment software.
- Good/Bad sample catalog and metric gates make recipe validation explainable.
- Viewer/layer/docking/ROI/template foundations are already in place.
- Explicit Preview/Run contracts reduce accidental state changes.

## Current Weaknesses

- Recipe/Pipeline Manager now has a larger workbench overlay, wider Step review column, and compact footer command strip; remaining density work should be driven only by fresh screenshots that show actual clipping or workflow friction.
- Commercial-style guided workflow is still intentionally narrower than equipment software, but Recipe Manager now exposes a compact guided setup strip for sample readiness, XML validation, Step count, sample run, Good/Bad run, and next action.
- LLM XML validation now has issue rows, before/after diff review, dependency/path action hints, and dependency drill-down rows, but real unresolved-path examples may still expose edge cases.
- Sample review now links into failed-Step focus, selected Step flow context, rerun/comparison actions, corrected-output review after XML apply, and selected Step branch/output comparison.
- Commercial tools are still ahead in guided setup, deployment/runtime packaging, recipe management maturity, and operator-ready polish.

## Commercial Comparison Summary

Official sources rechecked on 2026-07-06:

- Cognex In-Sight EasyBuilder: stronger guided setup and equipment/job workflow. Official EasyBuilder help describes Inspect as the step for assembling and configuring inspection tools, and Cognex positions In-Sight Explorer as setup/configuration/management software for In-Sight vision systems. OpenVisionLab should not clone equipment integration; it should improve guided recipe-building and review.
  - Sources: https://docs.cognex.com/is_631/web/EN/ezb/Content/EasyBuilder/Inspect_Home.htm and https://support.cognex.com/en/products/in-sight-explorer-software
- MVTec MERLIC: stronger no-code, image-centered recipe/runtime management. MERLIC recipes are separate `.mrcp` files that reference MVApps and predefined parameter sets, and MERLIC can switch recipes by loading parameter settings. OpenVisionLab should learn from centralized recipe information, dependency visibility, and operator-friendly recipe switching.
  - Sources: https://www.mvtec.com/doc/merlic/5.5/manual/en-us/Content/Process_integration/Recipes/merlic_recipes.html and https://www.mvtec.com/doc/merlic/5.5/manual/en-us/Content/Process_integration/Recipes/creating_recipe_file.html
- NI Vision Builder AI: stronger configure/benchmark/deploy scope. NI describes Vision Builder AI as menu-driven software for configuring cameras, customizing image analysis, benchmarking inspections, interfacing with automation hardware, and deploying inspection systems. OpenVisionLab should keep a narrower learning/verification scope and improve benchmark/sample review evidence.
  - Sources: https://www.ni.com/en/shop/electronic-test-instrumentation/application-software-for-electronic-test-and-instrumentation-category/what-is-vision-builder-for-automated-inspection.html and https://www.ni.com/pdf/manuals/375131g.html
- KEYENCE CV-X: stronger controller/simulator/terminal ecosystem. KEYENCE positions CV-X around cameras/lighting/high-volume inspection and provides PC simulator/terminal software for configuring settings and manipulating controller screens. OpenVisionLab should not compete as a controller platform; its value is local image-based recipe design and LLM-assisted XML generation.
  - Sources: https://www.keyence.com/products/vision/vision-sys/ and https://www.keyence.com/support/user/cv-x/code/

Current completion estimate:

- Versus commercial equipment platforms: about 25-30%.
- Versus the intended LLM-assisted rule-based recipe workbench: about 62-66%.

Current self-evaluation:

- Strongest differentiated area: LLM-generated XML prompt/draft/load/validate/import review, because common commercial equipment tools do not center GPT/Gemini/Claude-style XML generation as the core authoring loop.
- Strongest matured area: Recipe Manager review flow, including Good/Bad role drill-down, selected Step review, explicit XML apply-back, corrected-output review, operator report, and run-history review copy.
- Weakest product-fit area: real-world LLM draft correction coverage. The first replayable failure/correction corpus now blocks malformed XML, missing input layers, unsupported tools, missing dependency paths, invalid parameter values, matching score percentage misuse, and missing Arithmetic second inputs, then proves a bad-parameter draft can be corrected, validated, and explicitly imported. The next quality jump is external LLM transcript examples beyond these known gaps, not another generic layout pass.
- Weakest commercial-parity area: deployment/runtime packaging, camera/lighting/PLC/I/O, account/audit, and production controller workflows. These remain intentionally out of scope unless the product direction changes.

## Do Not Re-Review From Scratch

Future sessions should not spend time re-discovering these unless a regression is reported:

- Product identity and out-of-scope camera/PLC/I/O boundary.
- PropertyGrid-based algorithm-tool direction.
- Explicit Preview/Run and no layer/visibility auto-run rules.
- Viewer/layer/docking/ROI/template stable contracts.
- Public sample asset policy.
- Good/Bad sample catalog quality gate.
- Existing Recipe Manager CRUD/import/export baseline.
- Existing Recipe Manager workbench-sized overlay layout.
- Existing Good/Bad role failed-Step drill-down in Recipe Manager.
- Existing LLM XML prompt/draft/load/validate/import baseline and structured validation issue rows.
- Existing LLM prompt copy action in the LLM XML tab; do not add another prompt handoff surface unless the current tab is insufficient.
- Existing LLM review-bundle copy action in the LLM XML tab, including selected Step operator context and failed-Step review text; extend the bundle only when real LLM correction examples need additional fields.
- Existing LLM XML clipboard paste action in the LLM XML tab; keep it as an explicit draft-edit action and do not make paste validate, import, Preview, or Run.
- Existing LLM XML before/after diff review and dependency/path action hints.
- Existing LLM XML missing-dependency import block; drafts with unresolved template/image dependency paths must validate NG and must not import.
- Existing LLM XML result-channel contract: `Inspection.*` values are logical operator outputs derived from validation/run evidence. Do not add a separate XML node model for them unless a real export/import workflow requires it.
- Existing LLM XML failure/correction corpus smoke coverage for malformed XML, missing input layer, unsupported ToolType, missing dependency path, invalid parameter values, matching score percentage misuse, missing Arithmetic InputLayerB, correction-bundle copy, and corrected-draft explicit import.
- Existing Pipeline XML/Step tab, step comparison table, selected-step detail panel, selected-step operator context, and input/output thumbnail cards with click navigation.
- Existing multi-step selected-Step flow focus in the Recipe Manager header and XML/Step tab.
- Existing failed Step rerun/comparison action strip in the Recipe Manager Review tab.
- Existing corrected-output review after selected Step XML apply.
- Existing selected Step branch/output comparison rows for multi-step Recipe Manager review.
- Existing LLM XML dependency path drill-down rows.
- Existing selected Step ROI/template metadata card and explicit tool entry button.
- Existing selected Step PropertyGrid parameter review and explicit XML apply-back inside Recipe Manager.
- Existing Recipe Manager behavior where the selected Step PropertyGrid is hidden until parameters are explicitly loaded, and stale edit status is cleared when selected Step changes.
- Existing Recipe Manager compact footer where the selected recipe name editor and recipe CRUD/XML commands stay visible together on 1600x900.
- Existing Recipe Manager guided setup strip in the detail header; do not rebuild it as a separate wizard unless a real workflow requires a full-screen guided mode.
- Existing Recipe Manager guided next-action button; extend the action routing only when current EXE evidence shows a real next action is missing.
- Existing Recipe Manager operator decision board in the Review tab; do not replace it with a separate wizard unless current screenshots prove the compact board is insufficient.
- Existing Recipe Manager operator handoff Report tab, compact result-channel board, detailed result-channel list, and copy action; extend this report only with real missing review fields instead of adding another reporting surface.
- Existing Run History selected-review copy action; do not add a second history export path unless a real operator report format is required.
- Existing single-input custom tool base, Blob/Contour/Line single-input PropertyGrid tool base, Matching-family single-input PropertyGrid tool base, and double-input Arithmetic custom tool base; do not re-extract those shell/event forwarding paths.
- Removed top-level account/operator chrome; do not reintroduce it without real account/session requirements.
- Existing main window minimize, maximize/restore, and close controls; do not remove or hide these with account/session cleanup.

## Next Priority Order

1. Continue LLM XML correction-loop coverage
   - Highest-value next feature: add external LLM transcript examples only when they expose gaps beyond the current failure/correction corpus.
   - Improve guided setup/operator review only where current screenshots show unclear next action; do not add equipment integration.

2. Recipe Manager density/layout follow-up
   - Use current-build screenshots to fix only actual clipping, overlap, or workflow friction.

3. Pipeline/Recipe operator review follow-up
   - Expand branch/output comparison only when a real multi-branch sample needs more than the current selected-step producer/consumer map.

4. Tool View code-behind cleanup continuation
   - Continue only where established presenters/controllers/shared bases fit naturally.

## Session Start Checklist

```powershell
cd C:\Git\OpenVisionLab_Dev
git status --short
git log --oneline -5

cd C:\Git\OpenVisionLab
git fetch origin
git status --short
git log --oneline -5
```

## Verification Checklist

For code changes, run the smallest meaningful focused check plus the required policy checks when practical:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1
```

For UI/UX changes:

- Capture fresh current-build before and after screenshots.
- Store them under a clearly named `artifacts\...` folder.
- Show the images directly in the chat when reporting.
- Do not reuse older screenshots as before evidence.
