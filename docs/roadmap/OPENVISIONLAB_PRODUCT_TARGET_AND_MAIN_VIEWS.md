# OpenVisionLab Product Target And Main Views

Updated: 2026-07-29 KST

> **Live status and next priority:** Read `docs\OPENVISIONLAB_CURRENT_HANDOFF.md` first. This document owns stable product direction and main-view responsibilities; it does not replace current git/test evidence.

This is the short product-direction document for future sessions. Read this first when continuing OpenVisionLab work so the goal, view structure, completed areas, and next priorities do not need to be rediscovered.

## Final Product Shape

OpenVisionLab is an OpenCvSharp4 rule-based vision recipe workbench. Direct deterministic teaching and repeatable evidence are the product core; the existing LLM XML assistant is optional and frozen in maintenance mode.

It is not intended to become a camera, lighting, PLC, or I/O integration platform. Its primary value is the workflow before equipment integration:

1. Load or choose sample images.
2. Teach the inspection target, ROI, template, measurement region, and tolerance through PropertyGrid-based tools.
3. Compose and validate the Pipeline and its layer routes.
4. Run Preview or Run explicitly with OpenCvSharp4 rule-based tools.
5. Review Good/Bad samples, failed steps, metrics, layers, ROI, templates, parameters, and current-run drawings.
6. Replay a frozen recipe on N samples and review its deterministic queue.
7. Save a validated recipe for learning, review, and later integration by another system.

The preserved LLM Assistant may optionally draft or validate XML at the composition step. GPT, Gemini, Claude, browser automation, or API credentials are not required for the core workflow.

One-line product definition:

> OpenVisionLab is a desktop workbench for directly teaching, executing, and validating OpenCvSharp4 rule-based inspection recipes from sample images and operator-owned inspection intent.

## LLM Maintenance-Mode Product Contract

P196 freezes planned LLM expansion. The existing assistant remains a supported optional authoring surface, but current development must not add providers, browser automation, prompt families, new intent skills, or transcript campaigns. Work on it only for a demonstrated regression, unsafe XML acceptance, broken compatibility, or an explicit user decision to reopen the track.

The frozen evidence below remains useful as a compatibility and safety contract; it is not the active roadmap:

The product bet remains valid only as **guided initial recipe setup**, not autonomous inspection discovery from an arbitrary image. The primary LLM development unit is a reusable OpenVisionLab inspection-intent skill: operator intent and required inputs -> locked verified tool family -> starter XML -> explicit execution -> N-sample drawings/metrics/error table -> genuine correction evidence -> held-out completion gate.

- Phase 1 proves that the named intent and required inputs produce valid, importable starter XML. XML validity is not recipe-quality proof.
- Phase 2 proves the recipe on multiple samples with exact runtime drawings, metrics, and an operator-readable error table. Successful execution without semantic drawing evidence is not a pass.
- Phase 3 preserves a genuine failed first draft, uses its validator/runtime evidence for an LLM correction, and replays the correction on held-out data without hiding regressions.
- Complete one skill before starting another. Do not create a new algorithm family merely because one sample or draft fails when an existing bounded tool family can implement the intent.
- The first LLM pilot is `Pin row gap / pitch consistency`, with its v1 Guided Setup locked to `PinArrayGap` adjacent edge-to-edge clearance. P201 separately adds and verifies direct deterministic dark-pin, pixel-only `CenterPitch`; this does not reopen the frozen LLM skill. Edge gap, center pitch, pixel results, and calibrated units must remain explicit and separate.
- `docs/OPENVISIONLAB_PIN_ROW_GAP_INTENT_SKILL.md` is the approved v1 contract. Its user-visible supported intent is `Pin row edge-gap consistency`; measurement-only XML remains visibly unjudged until an explicit range gate and Train/Validation/Test evidence exist.
- P168 completes bounded Phase 2 for this pilot in Dev: three existing Local Validation Sets can be frozen with the exact skill/XML/set identity including image-content hashes, the unchanged two-row P148 recipe replays with sample error rows and current-run drawings, and the viewer retains a SHA-256-verified run-time source snapshot plus every executed row drawing. This is adjacent edge-gap evidence only, not center-pitch, calibrated-unit, or other-defect classification evidence.
- P169 freezes a new non-overlapping 72-image held-out Test split and preserves a fresh judged GPT response. That response passed strict validation and the allowed Train/Validation replay directly, so there was no legitimate correction step and the Test remains unexecuted. Do not repeat prompts to force a failure.
- P170 freezes target-bearing working Train/Validation manifests for the next natural Phase 3 attempt: Train Good 178 / `pitch_error` 26 and Validation Good 36 / `pitch_error` 12, with zero path/content overlap against the P169 target Test. Every working row was previously observed, so this improves pre-Test coverage without becoming blind evidence or Phase 3 completion.
- `OuterCornerIntersection` remains experimental and is not a default LLM skill or recommendation. Its current card evidence does not prove that the fitted lower line is supported by the operator-intended physical card-bottom edge.
- Manual prompt copy/XML paste through free web accounts is a supported optional transport. Consumer-web automation and API credentials are not hard dependencies of the skill workflow.

Frozen maturity: P167 completes Phase 1 authoring/strict validation and P168 completes bounded Phase 2 Train/Validation/Test identity, replay, error-row, and multi-row drawing evidence for the first `Pin row edge-gap consistency` skill. P195 separately completes Phase 1 for the bounded hybrid relative-ROI authoring contract. Phase 3 still lacks a genuine natural failure -> correction -> one-time held-out replay, but that missing evidence is intentionally deferred rather than an active blocker for rule-based development.

Reopen planned LLM work only after an explicit user decision and all three conditions hold: the same target can be configured without LLM assistance; its deterministic tool family has stable metrics/drawings and frozen N-sample evidence; and the XML contract is stable enough that the LLM composes verified capabilities instead of compensating for missing rule-based behavior.

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

## User-Centered Workflow And Persisted Setup Direction

Future development starts from the operator's goal and the shortest safe normal
workflow. Internal view, dialog, component, class, and storage boundaries must
not make one durable task require repeated configuration in several places.

When related settings belong to one reusable workflow:

1. expose one coherent first-use setup or option surface;
2. require explicit operator confirmation;
3. persist at the narrowest correct Tool, Recipe, project, workspace, or user
   scope;
4. restore the setup visibly and editably on the next equivalent use;
5. provide an explicit reset/default path;
6. fail closed with a direct stale/incompatible-state explanation;
7. restore configuration without Preview/Run, layer, active-layer, or routing
   side effects.

Every implementation must verify first use, save, close/reload/reopen, exact
restoration, visible reset, incompatible-state handling where applicable, and
zero unintended execution/layer/routing mutation. Do not silently share
inspection-specific ROI, tolerance, template, dependency, or coordinate-frame
state across unrelated Recipes or workspaces.

The reusable admission template, commercial-video rationale, and current
evidence-gated priority order are in
`docs/reports/OPENVISIONLAB_USER_CENTERED_WORKFLOW_DIRECTION_20260729.md`.

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

### 3. Recipe Manager And Pipeline Review Views

Purpose: keep recipe lifecycle management separate from pipeline authoring and execution review.

Responsibilities:

- Recipe Manager lists, searches, creates, duplicates, renames, and deletes reusable recipes.
- Recipe Manager summarizes the selected recipe, its active pipeline, selected validation sample, and latest result, then opens the existing Pipeline surface for step-level work.
- Pipeline Review shows the owning recipe and provides an explicit return to that recipe summary after review; navigation in either direction must not execute the pipeline implicitly.
- Pipeline owns inspection-step order, layer routing, acceptance configuration, explicit Preview/Run, and step/output comparison.
- Tool Views own one algorithm's PropertyGrid parameters and explicit Step creation.
- Detailed XML, validation-set, history, LLM, and report functions remain available only through an explicit advanced-review mode instead of competing with the default task.
- Summary and advanced review are separate layouts rather than one additive screen: summary shows recipe library/search, one selected-recipe overview, and lifecycle commands; advanced review hides those outer controls, opens Pipeline review at full width, and provides an explicit return to summary.
- Recipe Manager actions must not run Preview/Run, create layers, or change input/output routing implicitly.

Already completed enough to avoid redoing:

- Recipe manager has searchable list, create/duplicate/rename/delete, XML import/export, draggable title area, close affordance, and a workbench-sized overlay layout with recipe library, review workspace, and command strip zones.
- Recipe Manager now opens on a compact selected-recipe summary. The summary provides one explicit `Open Pipeline` action, while the existing detailed tabs are hidden behind `Advanced review`; reopening the manager returns to the summary without running Preview/Run.
- Advanced review now removes the outer recipe library, search, and create/duplicate/rename/delete controls instead of leaving them visible beside technical content. Its top-level choices are `Build inspection`, `Pipeline review`, `LLM XML`, and `Step preview`; XML import/export/review-bundle actions remain in a compact transfer strip.
- The novice round trip is now complete: `Recipe summary -> Open Pipeline -> explicit Run Review -> Return to Recipe`. Pipeline Review shows the recipe context, and the return path restores the same summary while preserving native Preview count, layer count, active layer, and recipe/pipeline routing.
- The summary now labels the catalog selection as the current work sample and shows a latest execution only when the sample-run result belongs to the same recipe and selected pipeline. An automatically selected catalog sample no longer appears as recipe validation evidence before a sample check.
- Recipe library filtering now shows visible/total count and is smoke-verified with 100 temporary long recipe names.
- Pipeline list filtering now shows visible/total count and is smoke-verified with 100 temporary long pipeline names in one recipe.
- Pipeline inventory now excludes PropertyGrid/tool-state XML, `pipeline.active.xml`, malformed XML, and unrelated metadata by requiring an exact no-namespace `VisionPipeline` document root. Excluded files are preserved unchanged.
- The Recipe Manager library/sample column now uses a wider 320px baseline and shortens the displayed sample id in the sample acceptance summary while keeping the full text in the tooltip.
- Pipeline tab is split into review/history/XML-Step sub-tabs.
- Duplicate from sample, LLM XML validation report, structured validation issue rows, pipeline preview step list, step comparison table, selected-step detail panel, selected-step operator context, selected-step input/output layer thumbnail cards with click navigation, selected-step PropertyGrid parameter review with explicit XML apply-back and corrected-output review, selected Step branch/output comparison rows, and Good/Bad role failed-Step drill-down exist.
- Multi-step flow focus remains in the XML/Step technical tab with explicit Previous/Next Step navigation that does not run Preview. The repeated Step/status/guided text was removed from the global Recipe Manager header.
- The XML/Step tab now keeps the inline Step list directly under the flow focus strip, before branch/output and detail panels, so the actual Step rows are visible on the first 1600x900 workbench view instead of being pushed below dense review content.
- Recipe library rows and XML/Step inline Step rows now use predictable single-line ellipsis with tooltips for long recipe names, long routes, and long parameter previews instead of allowing dense rows to grow unpredictably.
- Branch/output comparison is now smoke-verified against the real `BentPin_TopBottom_Overlay` multi-branch sample, including same-input rows, input-producer rows, and multiple output consumers from one intermediate layer.
- Branch/output comparison is also smoke-verified against the real `Contour_AllSymbolsAndFaint_LLM` 3+ fan-out sample, including three same-input alternatives and one output consumer from the selected Step.
- Branch/output comparison now resolves declared `OverlayMerge.SourceLayers`. The real GPT four-branch pin-gap recipe shows each range-evidence Step's review-merge consumer and the final overlay's four source producers instead of classifying the overlay as an unrelated same-input alternative.
- Failed-Step rerun/comparison action strip now exists in the Review tab: selected failed Step text, input/output comparison route, direct output/input layer navigation, parameter review, and Good/Bad rerun.
- Operator decision board now exists in the Review tab: XML/Step, selected sample, Good/Bad, and next action are summarized above the longer operator review text.
- Operator handoff report now exists as a Pipeline review sub-tab, summarizing current recipe/pipeline/XML/sample/Good-Bad/failure-Step/next-action state for review and next-session transfer; it also has an explicit copy action and a compact result-channel board for `Inspection.Status`, `Inspection.FailedStep`, `Inspection.Evidence`, `Inspection.Benchmark`, and `Inspection.NextAction`.
- Run History now has an explicit selected-review copy action for sharing the selected saved run interpretation without rerunning checks, plus a compact linked-report Step bottleneck list with coverage, average, p95, and maximum.
- Guided starter actions remain inside the dedicated `Build inspection` tab instead of being repeated in the global Recipe Manager header.
- Recipe Manager now has a separate `Review bundle` export. The first bounded ZIP schema contains `pipeline.xml` plus `review-manifest.json`, including validation, ToolType/Step/acceptance summaries, and referenced dependency/sample path, size, and SHA-256 evidence. Referenced files are not copied, and export does not import or run Preview/Run.
- XML import and XML/bundle load recognize `.review.zip` as a review-only source. They verify the two-entry schema, XML hash/size, package policy, manifest/XML dependency consistency, and deterministic adjacent relocation candidates, then open the existing `LLM XML` review tab without importing, copying, or running anything.
- Relocation candidates remain explicit evidence: the operator must update the XML path and validate again. Validation NG keeps `Import` disabled; changing the XML or selected inspection intent invalidates prior import readiness.
- Recipe combo crash and old/private recipe cleanup were handled before these latest commits.

Next development focus:

- Continue density/layout polish only when current screenshots show actual clipping, overlap, or workflow friction.
- Make branch/output comparison broader only when a real recipe exposes a relationship not represented by direct `InputLayer` or declared `SourceLayers`.
- P3 review-bundle export plus import-side dry validation/path-relocation review is complete at the reference-only scope. Do not add recursive file search or silent asset copy.
- Recipe-local user-defined Validation Sets now support named explicit image lists, bounded top-level folder registration, per-image expected OK/NG and notes, missing-file blocking, operator-selected path repair, and explicit execution through the existing result/history/failure-review flow. They are stored outside the pipeline XML directory and do not enter the public catalog.
- Recipe Manager pipeline inventory pollution is fixed: only actual `VisionPipeline` documents are listed even though tool-state XML remains in the same legacy `VISION` directory.
- P4 Validation Sets are complete at the bounded local workflow scope. Keep recursive search, inferred replacement, automatic path rewriting, and a second runner out of scope.

### 4. LLM XML Assistant View

Purpose: preserve an optional maintenance-mode surface for drafting and reviewing OpenVisionLab XML safely.

Responsibilities:

- Build prompts from the selected sample, intended inspection goal, detection points, and tool templates.
- Load LLM XML drafts.
- Validate XML structure, tool names, layer routes, dependencies, and sample compatibility.
- Explain errors, warnings, missing files, and safe import behavior.
- Import only after explicit operator action.

Already completed enough to avoid redoing:

- LLM assistant fields, prompt creation/copy, XML starter creation, XML draft load, clipboard paste, validation, import draft, reference image, dependency copy report, dependency/path action hints, dependency path drill-down rows, draft import review, before/after diff review, inline validation report, and LLM correction review-bundle copy exist. The correction bundle includes selected Step operator context and failed-Step review text.
- LLM result-channel contract now exists in the in-app prompt/review/validation flow: `Inspection.Status`, `Inspection.FailedStep`, `Inspection.Evidence`, `Inspection.Benchmark`, and `Inspection.NextAction` are derived after validation and explicit runs, not emitted as XML nodes.
- External LLM prompt-side API guidance now exists at `docs\contracts\openvisionlab\OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md` and `docs\contracts\openvisionlab\OPENVISIONLAB_LLM_TOOL_CATALOG.json`. Use these only for maintenance-mode XML compatibility work so the LLM learns the actual OpenVisionLab XML contract instead of guessing.

Next development focus:

- No planned feature expansion. Preserve current behavior and fix only evidence-backed regressions, unsafe validation/import behavior, or compatibility defects.
- Do not request new provider transcripts, add browser automation, or create another intent skill while maintenance mode is active.
- Keep the workflow explicit: validate, review diff/dependencies, then import. It must not run Preview or silently accept recipes.

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
- The preserved LLM XML workflow is an optional convenience layer over the deterministic workbench, not its readiness claim.
- Good/Bad sample catalog and metric gates make recipe validation explainable.
- Viewer/layer/docking/ROI/template foundations are already in place.
- Explicit Preview/Run contracts reduce accidental state changes.

## Current Weaknesses

- Recipe Manager now has a novice-first summary and a physically separated full-width advanced-review workspace. The advanced Pipeline/XML/LLM/history surfaces remain intentionally technical; move or consolidate another function only when a real workflow proves that it duplicates the dedicated Pipeline surface.
- Commercial-style guided workflow is still intentionally narrower than equipment software. The default Recipe Manager no longer presents its detailed guided strip as the primary task; step setup and execution guidance belong in Pipeline and Tool workflows.
- LLM XML validation now has issue rows, before/after diff review, dependency/path action hints, and dependency drill-down rows, but real unresolved-path examples may still expose edge cases.
- Sample review now links into failed-Step focus, selected Step flow context, rerun/comparison actions, corrected-output review after XML apply, and selected Step branch/output comparison.
- Commercial tools are still ahead in guided setup, deployment/runtime packaging, recipe management maturity, and operator-ready polish.
- P216 completes the P215-selected per-object Blob/Contour bounding-width/height slice. The four optional pixel-bound keys filter individual accepted objects and `ResultCount`, preserve legacy missing-key behavior, and publish exact P211/Run History reject reasons. Full region-feature evaluation, rotated dimensions, automatic parameter suggestion, OCR/barcode, and navigation rewrites remain deferred.

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

Historical completion estimate recorded before the P165 inspection-intent-skill decision (not current status; use `docs\OPENVISIONLAB_CURRENT_HANDOFF.md`):

- Versus commercial equipment platforms: about 25-30%.
- Versus the intended LLM-assisted rule-based recipe workbench: about 62-66%.

Historical self-evaluation from the same pre-P165 snapshot:

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
- Existing LLM XML authoring guide and machine-readable tool catalog; do not ask external LLMs to draft OpenVisionLab XML without these references unless the experiment is intentionally measuring unguided failure behavior.
- Existing LLM XML failure/correction corpus smoke coverage for malformed XML, missing input layer, unsupported ToolType, missing dependency path, invalid parameter values, matching score percentage misuse, missing Arithmetic InputLayerB, correction-bundle copy, and corrected-draft explicit import.
- Existing Pipeline XML/Step tab, inline Step list placement before branch/output/detail panels, compact ellipsis Step rows, step comparison table, selected-step detail panel, selected-step operator context, and input/output thumbnail cards with click navigation.
- Existing multi-step selected-Step flow focus in the Recipe Manager header and XML/Step tab.
- Existing failed Step rerun/comparison action strip in the Recipe Manager Review tab.
- Existing corrected-output review after selected Step XML apply.
- Existing selected Step branch/output comparison rows for multi-step Recipe Manager review, including real BentPin multi-branch and Contour_AllSymbolsAndFaint 3+ fan-out smoke coverage.
- Existing LLM XML dependency path drill-down rows.
- Existing selected Step ROI/template metadata card and explicit tool entry button.
- Existing selected Step PropertyGrid parameter review and explicit XML apply-back inside Recipe Manager.
- Existing Recipe Manager behavior where the selected Step PropertyGrid is hidden until parameters are explicitly loaded, and stale edit status is cleared when selected Step changes.
- Existing Recipe Manager split footer: summary shows name editing and recipe lifecycle commands, while advanced review shows only XML import/export and review-bundle transfer commands.
- Existing Recipe Manager recipe library filter count for large libraries; do not replace it with a separate browser until a real workflow needs grouping, tags, or paging beyond search.
- Existing Recipe Manager pipeline list filter count for large recipes; do not replace it with a separate pipeline browser until a real workflow needs grouping, tags, or paging beyond search.
- Existing Recipe Manager compact summary and explicit advanced-review switch; keep recipe selection/lifecycle as the default task and do not expose every detailed review surface at once again.
- Existing Guided Setup content is contained in the advanced `Build inspection` tab; do not repeat guided status/actions in the global header or summary.
- Existing Recipe Manager operator decision board remains an advanced review aid; do not duplicate it in another Recipe Manager summary card.
- Existing Recipe Manager operator handoff Report tab, compact result-channel board, detailed result-channel list, and copy action; extend this report only with real missing review fields instead of adding another reporting surface.
- Existing Run History selected-review copy action; do not add a second history export path unless a real operator report format is required.
- Existing single-input custom tool base, Blob/Contour/Line single-input PropertyGrid tool base, Matching-family single-input PropertyGrid tool base, double-input Arithmetic custom tool base, and Arithmetic interaction-controller event ownership; do not re-extract those shell/event forwarding paths or move Arithmetic parameter events back into view code-behind.
- Removed top-level account/operator chrome; do not reintroduce it without real account/session requirements.
- Existing main window minimize, maximize/restore, and close controls; do not remove or hide these with account/session cleanup.
- Existing Pipeline Review contextual `Learn Tool` entry for supported selected Steps; it reuses the Learn topic catalog and has no Preview/Run, layer, routing, parameter, or review-state side effects.
- Existing Pipeline Review `설정 수정` handoff for the selected Step; it opens the same recipe/pipeline/Step in Recipe Manager `Advanced > Pipeline > XML/Step`, makes the established PropertyGrid editor visible, and preserves explicit XML apply plus explicit Preview/Run semantics.
- Existing catalog-sample edit alignment: when the selected pipeline is an exact `Sample_<catalog sample name>` workspace copy, Recipe Manager selects that same work sample before any explicit Good/Bad rerun; unrelated recipe pipelines do not change work-sample selection.
- Existing `PinArrayGap` repeated-pin teaching now has two explicit deterministic meanings: legacy/default `EdgeGap` publishes `DistancePx*`, while `CenterPitch` publishes `PitchPx*` from adjacent dark-pin centers. Recipe Manager PropertyGrid can edit both without automatic execution. The frozen LLM Pin Guided Setup v1 remains EdgeGap-only.
- Existing Pipeline Review Fixture/relative-ROI designer detects one named `Matching -> NormalizeImage -> downstream CvROI` chain and shows template/search ROI, taught and current pose, score/margin/valid-pixel evidence, and paired source/normalized ROI drawings. Its actions reuse reference teach, Recipe Manager PropertyGrid, and explicit Run Review; it is not a locator algorithm or qualification claim.
- Completed P213 General Geometric Measurement Workspace keeps editing PropertyGrid-based and adds reusable same-run point/segment/circle evidence, radial CircleGauge, seven GeometryMeasure relations, and a read-only Geometry Review with two-way drawing/table selection. Its evidence is pixel-only and synthetic/UI-bounded.
- Completed P214 Two-Point Scale Teaching adds a separate Pipeline Review calibration tab that hash-locks two same-run points and a user-supplied known distance, derives one uniform mm/px value, and applies it explicitly to one compatible measurement Step without Preview/Run or layer/routing mutation. It preserves the legacy `PIXELPERMM` key for compatibility and is not lens/camera or certified metrology calibration.
- Completed P216 Blob/Contour object-dimension filtering adds PropertyGrid min/max ranges for axis-aligned pixel width and height, filters before accepted-object metrics/`ResultCount`, and preserves exact rejected-object evidence in P211 and Run History. Missing keys remain area-only.
- P217 statically rechecked the end-to-end deterministic operator path after P216 and selected no additional feature. PropertyGrid teaching, explicit Run Review, Object/Fixture/Geometry/Scale review, recipe round trip, saved drawings/object rows, batch history, and the deterministic review queue are connected; no remaining commercial candidate has a named blocked operator task and current reproduction.
- P218 responds to a later explicit operator request with one bounded deterministic extension: Library-Noah calculates and executes a three-point pixel Affine transform, while OpenVisionLab provides PropertyGrid teaching, explicit Preview, XML/Pipeline round trip, drawings/metrics, result review, a public sample, and Geometry Learn. It is not automatic correspondence, homography, or camera calibration.
- P219 connects three earlier deterministic typed Point results to that same Library-Noah Affine source triangle. Matching can publish one accepted `Center`; Recipe Manager provides ordered source pickers; the normalized output feeds unchanged fixed-coordinate downstream ROIs. This is explicit same-run correspondence wiring, not automatic point selection or per-image ROI motion.
- P220 applies the operator-approved card `R`/`5`/expiry-mark centers to a frozen 12-row real-image pilot. After one search-ROI-only correction, all 12 reached Affine output and 10 met the pre-frozen `<=3 px` normalized-center gate; two retained `4.12/5.00 px`. The current Matching-center candidate is therefore not qualified for a `<=3 px` downstream inspection.
- P221 records the operator's separate acceptance of the observed `<=5 px` envelope for one coarse date-area ROI. The unchanged Matching x3/Affine result feeds exact `CardReference` ROI `250,315,190,80` into the existing unjudged Mean tool on all 12 rows, with finite metrics and runtime drawings. This proves fixed-coordinate linkage only, not defect classification or locator qualification.
- P222 implements the separately requested Library-Noah Auto MPoint core as a one-image, fixed-size matching-candidate suggestion engine. It reuses the existing edge matcher for uniqueness, synthetic pose replay, precision, runtime, and drawings; it is not a Pipeline Step or automatic recipe mutation.
- P223 integrates that core into the existing Edge Based Matching Tool View. PropertyGrid teaching settings, explicit `Analyze candidates`, candidate rows/drawing, and explicit `Use this pattern` preserve Preview/Run count, layers, active layer, and routing. The source/vendored/current-build Library-Noah DLL identity is retained. The UI labels candidates `Suggested`, not `Qualified`.
- The P223 GPT Pro research review confirmed that the current matcher was the correct base and selected unique-result acceptance before least-squares refinement, adaptive pattern sizing, ODB, or multi-anchor expansion.
- P224 completes that optional acceptance slice. Library-Noah now retains an internal Top-8 candidate pool independently of external `NUM_MATCH=1`, distinguishes `NoMatch`, `Success`, and `Ambiguous`, and returns no result with an exact reason for either failure state. OpenVisionLab exposes the backward-compatible fields through the existing PropertyGrid/XML path without automatic Preview and retains normalized review metrics.
- P225 rejects the first real-image fixed-ROI candidate. The approved card `R` anchor produced no correct accepts in reviewed-ROI unique mode and retained wrong accepts in broad unique mode; one exact drawing shows a unique high-score selection on `T`. The unique gate remains useful for ambiguity rejection, but it is not a semantic identity test. EdgeBased single-result `Center` and existing scale/refinement Pipeline settings are now wired for future Affine/relative-ROI use only after a different candidate qualifies.
- P226 presents current Auto MPoint suggestions on five diverse public EasyMatch sources without applying them. Four sources produced 20 displayed suggestions, while the repeated `Frame 1` pattern rejected all eight finalists. Repeated floppy hubs could still rank because their fixed orientations differed, so the operator must approve physical identity before any cross-image qualification.

## Current Next Priority Order

1. P274 is complete for Runtime Data Root v1. Release now separates immutable installation files from per-user or administrator-selected writable data, preserves legacy portable data through copy-only migration, restores the same scope on reopen, and leaves the copied installation inventory unchanged. Two independent clean clones of commit `823d2d8acb87a269b79c602d29316e0908081ab0` produced the same 75-file framework-dependent ZIP SHA-256 `807747DB316FE115E48728DF930F224F7CFB289CD597BDD0F5774B253CC123BD`; Debug/Release, readiness 13/13, all 33 public sample rows, package launch/reopen, focused persistence, and zero-side-effect checks passed. See `docs\reports\OPENVISIONLAB_RUNTIME_DATA_ROOT_V1_20260730.md`. This is not commercial GA.
2. Approve the distribution/installation model, publisher/signing identity and certificate, update channel, and machine/per-user policy. Prerequisite: these business and deployment inputs; do not spend implementation tokens before they exist. Recommended model: none until prerequisites; `gpt-5.6-sol` afterward | Reasoning effort: none until prerequisites; high afterward.
3. Implement and verify installer, signed binary/payload, update/rollback, uninstall, retained-data choices, and migration recovery against the approved model. Recommended model: `gpt-5.6-sol` | Reasoning effort: high.
4. Generate and review dependency/SBOM/license evidence, then add an operator support bundle and bounded startup/run performance criteria. Recommended model: `gpt-5.6-terra` | Reasoning effort: medium.
5. Collect raw observations from at least three independent novice users through the existing `CVR-00` protocol. Prerequisite: real participants and unedited observations. Agent recordings remain facilitator/development rehearsal only. Recommended model: none before observations; `gpt-5.6-terra` for synthesis afterward | Reasoning effort: none before observations; low afterward.
6. Do not activate OCR/Barcode, calibration, deformable/anisotropic matching, Region descriptors/algebra, derived expressions, or another algorithm without a named operator task, reproducible current-tool failure, Good/Bad/held-out evidence, metrics, acceptance, and physical tolerance ownership. Recommended model: none before an admission packet; `gpt-5.6-sol` for an approved high-risk matching/metrology/calibration task | Reasoning effort: none before the packet; high afterward.

The user closed repeated image inspection, dataset tuning, and LLM validation as active work. Do not resume them without a new explicit request. See `docs\OPENVISIONLAB_RULE_BASED_UI_GAP_AUDIT_20260723.md`.

## Historical Priority Order (superseded by the first inspection-intent skill)

1. Continue LLM XML correction-loop coverage
   - Highest-value next feature: add external LLM transcript examples only when they expose gaps beyond the current failure/correction corpus.
   - Improve guided setup/operator review only where current screenshots show unclear next action; do not add equipment integration.

2. Continue P6 benchmark and regression analytics when no real transcript is available
   - The first bounded aggregate is complete: saved batch rows now provide failure rate, average, median, nearest-rank p95, and maximum in the existing Run History comparison summary.
   - The compatible-baseline slice is complete: average and p95 deltas appear only for the same suite kind/name and exact image multiset with complete timings. Selected/Pair/Catalog/Local-set executions now persist distinct suite kinds, while outcome comparison remains independent.
   - The Step-report evidence gate is complete: explicit selected-sample, Good/Bad pair, Catalog, and Local Validation Set suite executions save a structured metadata-only Step report and link it through `RunReportPath`; a plain single check still has no persistence side effect.
   - The per-Step bottleneck slice is complete: Run History aggregates only complete compatible linked reports, orders enabled Steps by p95, and shows coverage, average, p95, and maximum. Missing or incompatible coverage shows a reason instead of partial numbers; no telemetry service, database, background run, or new top-level panel was added.
   - The real multi-Step selected-Step coherence audit is complete with `Public_Matching_FixturePad`: Step 2 Blob identity, branch route, input/output previews, Fixture parameters, result metrics, and elapsed time stay aligned. Duplicate ordinal text was removed and the Step summary width was adjusted without adding a panel or automatic execution.
   - The compact contextual Learn entry is complete. It appears only for supported selected Step ToolTypes, reuses the existing Learn window, and keeps review/workspace state unchanged.
   - The selected-Step adjustment handoff is complete. Pipeline Review now routes `설정 수정` to the authoritative Recipe Manager PropertyGrid editor for the exact recipe/pipeline/Step; opening it does not change routing, run Preview/Run, or mutate layers.
   - The real public Fixture round trip is complete. `MIN_AREA 700 -> 750` persists only after explicit XML apply, and the following explicit Good/Bad rerun uses `Public_Fixture_Pad_Good` plus `Public_Fixture_Pad_Missing_Bad` instead of an unrelated prior/default sample.
   - Do not add a second editor, automatic apply, or direct detached Tool View write-back. With no real external LLM transcript, continue only from fresh evidence of Recipe Manager/LLM Assistant friction or a real multi-branch comparison gap.

3. Recipe Manager responsibility follow-up
   - Use only fresh current-EXE screenshots or current-source view captures generated after the latest relevant source changes. Do not use older artifacts as current UI evidence.
   - The synthetic novice round trip and sample-evidence semantics are smoke-verified. Recheck with a real operator recipe before moving more advanced functions.
   - Remove or relocate a detailed function only when it demonstrably duplicates the dedicated Pipeline workflow; preserve existing advanced capabilities meanwhile.

4. Pipeline/Recipe operator review follow-up
   - The real GPT four-branch LineDistance/OverlayMerge recipe is covered through declared `SourceLayers`; expand comparison only when another real recipe exposes a relationship outside direct `InputLayer` and `SourceLayers`.

5. Tool View code-behind cleanup continuation
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
