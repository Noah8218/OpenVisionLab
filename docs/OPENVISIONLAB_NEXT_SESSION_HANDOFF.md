# OpenVisionLab Next Session Handoff

Updated: 2026-07-16 KST

This document is the minimum handoff needed to continue without re-discovering the current state. Work starts in `C:\Git\OpenVisionLab_Dev`; only reviewed and stabilized changes are imported into the original repo at `C:\Git\OpenVisionLab`. Do not run `git push` unless the user explicitly requests `PUSH`.

## Read First

- Product target, final program shape, main view architecture, stable areas that should not be rediscovered, and current development priorities are summarized in `docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`.
- Use that document as the first orientation source for future sessions before starting UI/Recipe/LLM/sample work.
- If this handoff conflicts with `docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`, prefer the newer target/main-view document and then verify against source, tests, and screenshots.

## Latest Recipe Manager Responsibility Split On 2026-07-15

- User review identified that Recipe Manager had become a second Pipeline editor: Pipeline, XML, LLM, validation, history, reports, and guided actions competed at the same level and obscured the first task.
- The corrected ownership is now explicit: Tool View configures one algorithm, Pipeline owns Step order/layer routing/acceptance/explicit Preview-Run, Pipeline Review owns run evidence, Recipe is the reusable package, and Recipe Manager owns library/lifecycle/entry-point work.
- Recipe Manager now opens on a compact selected-recipe summary with active pipeline, current work sample, recipe-specific current check result, and one explicit `Open Pipeline` action.
- Existing Guided Setup, Pipeline, LLM XML, history, report, validation-set, import/export, and detailed review functions are preserved behind an explicit `Advanced review` switch. Closing/reopening returns to the summary.
- `Open Pipeline` reuses the existing Pipeline Review surface. The screenshot smoke asserts that opening it does not run Preview/Run, create a result, or change layer routing.
- The change is intentionally a responsibility-first UI slice, not a destructive migration. The next evidence gate is the real novice route `select recipe -> open Pipeline -> explicit review/run -> return`; move more detailed functions only when that route proves a concrete duplication.
- Current-source before/after evidence is stored under `artifacts\recipe_manager_responsibility_split_20260715`.
- Focused current-source smokes passed for the summary, Guided Setup, operator decision board, local Validation Set, and broad language/CRUD/import/export/history flow with `layout=0`, `text=0`, and `internal=0`.
- The full solution build passed with zero warnings/errors. Readiness, external-reference, public-sample (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), and `git diff --check` passed.
- Latest EXE proof used `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, built at `2026-07-15 09:38:59 KST`. Direct `recipe-manager-tabs` passed under `artifacts\recipe_manager_responsibility_split_20260715\direct_exe_verified` and now checks the default summary before opening advanced review.

## Latest Matching Fixture Operator Workflow Slice On 2026-07-14

- The commercial-gap reassessment keeps intended-workbench maturity at about 62-66% and selects fixture/coordinate-frame handling as the first major missing workflow.
- `docs\OPENVISIONLAB_MATCHING_FIXTURE_WORKFLOW_SPEC.md` defines the first bounded contract: one `Matching` result, `NUM_MATCH=1`, one named frame, and one downstream axis-aligned `CvROI` translated on the same source layer.
- `VisionPipelineFixtureFrameService` publishes the reviewed Matching center/angle, computes X/Y offset, clones the consumer step, and changes only the clone's effective `CvROI`. Saved XML and input routing remain unchanged.
- Translation-only v1 rejects excessive angle change, missing/duplicate frames, different source layers, missing ROI, multi-ROI, masks, and out-of-image effective ROI instead of applying a partial or misleading transform.
- Metrics now include `FixtureCenterX/Y`, `FixtureAngle`, `FixtureOffsetX/Y`, `FixtureAngleDelta`, and effective ROI X/Y.
- `tools\OpenVisionFixtureSmoke` proves reference OK, translated-without-fixture NG, translated-with-fixture OK, `(70,40)` offset recovery, and unchanged saved `CvROI=170,80,50,50`.
- Recipe Manager now exposes the Matching producer fields and the first Blob consumer fields through the existing PropertyGrid. `VisionPipelineStepPropertyMapper` preserves the fixture keys through load/apply while keeping `CvROI` and layer routing unchanged.
- Korean localization labels the workflow as `기준 좌표`; no separate settings window or code-behind workflow was added.
- `wpf_shell_host_recipe_fixture_properties` verifies the producer/consumer descriptors, XML parameter round trip, visible PropertyGrid rows, and unchanged Preview/Run count. Current-source before/after evidence is under `artifacts\fixture_property_grid_roundtrip_20260714`.
- The public pair `Public_Fixture_Pad_Good` / `Public_Fixture_Pad_Missing_Bad` now proves a shifted `(80,55)` locator, effective Blob ROI `(400,235)`, Good OK, and locator-present/pad-missing Bad NG with the same pipeline.
- The same shifted Good image fails when Fixture use is explicitly disabled, and the saved consumer `CvROI=320,180,60,50` remains unchanged.
- Pipeline Review now shows the selected consumer's read-only runtime evidence as `Fixture Delta X,Y | ROI X,Y`; this does not run Preview or change recipe values.
- Current-source sample-picker before/after and Pipeline Review evidence is under `artifacts\public_fixture_sample_20260714`. Latest EXE proof is `latest_exe\public_fixture_pipeline_review_current_exe.png` plus `report.txt`.
- Public catalog verification passed 30/30 runnable rows; public asset policy passed with `CatalogRows=30`, `ManifestAssets=229`, and `Pipelines=15`.
- Pipeline Review now exposes `참조 자세 저장` only for a fixture-producing Matching Step after an explicit successful Review. It copies the reviewed `FixtureCenterX/Y/Angle` into the three reference parameters and persists the active pipeline.
- Saving the reference immediately invalidates the prior review result and asks for another explicit Review. It does not launch Preview/Run, create or select a layer, change input/output routing, or rewrite any consumer parameter including `CvROI`.
- `wpf_shell_host_workspace_sample_fixture_teach` executes the command and asserts `120,100,0 -> 200,155,0`, unchanged consumer parameters/routes/layers/native Preview count, and the post-save `run review required` state. Current-source before/after evidence is under `artifacts\fixture_pose_teach_20260714`.
- Fresh solution and screenshot-tool builds passed with zero warnings/errors. Fixture runtime, localization catalog, readiness, external-reference, public-sample (`30/229/15`), and `git diff --check` passed. The latest Debug EXE timestamp is 2026-07-14 12:47:51 KST; direct `recipe-manager-tabs` smoke passed under `artifacts\fixture_pose_teach_20260714\direct_exe`.
- This remains translation-only. The next evidence gate is a real operator pass on the chosen reference image to determine whether explicit consumer-ROI reteaching guidance is sufficient. Do not auto-rewrite consumer ROI, and do not add rotation/scale compensation without a real failing sample.

## Latest Tool Rail Readiness Slice On 2026-07-14

- P2 Tool Finder/readiness started with the smallest verified operator-visible state instead of adding a second tool browser.
- The existing 15 image-processing/algorithm Tool rail items now show `입력 없음` when `Main` has no image and `설정 가능` after a `Main` image is loaded. Pipeline remains a `흐름` surface rather than pretending to be an image Tool View.
- `설정 가능` means only that the PropertyGrid tool can be opened and configured. It does not claim that template, second input, ROI, calibration, or other tool-specific Preview prerequisites are complete.
- The readiness state reuses `OpenVisionShellNavItem` and the existing Main-layer refresh path. It does not add a new window, disable tool selection, open a tool, run Preview/Run, create a layer, or change routing.
- Compact Tool rail remains icon-only and clickable; its existing tooltip now carries the current readiness description.
- Current-source before/after captures are under `artifacts\tool_readiness_20260714\before` and `artifacts\tool_readiness_20260714\after`. Focused smoke passed for empty workspace, image-loaded workspace, and compact Tool rail with `layout=0`, `text=0`, and `internal=0`.
- A fresh solution build passed with zero warnings/errors. The latest Debug EXE timestamp is 2026-07-14 13:32:26 KST; direct `workspace-startup-empty` smoke passed under `artifacts\tool_readiness_20260714\direct_exe` and shows the new empty-input badges.
- Localization catalog, readiness, external-reference, public-sample (`30/229/15`), and `git diff --check` passed.
- `Matching` now adds the first real tool-specific prerequisite state: when `Main` is ready but its existing `MatchingProperty` has no loaded template, only the Matching row shows `템플릿 필요`. Registering or clearing the template through the PropertyGrid persistence path refreshes the row immediately.
- The transition smoke proves `템플릿 필요 -> 설정 가능 -> 템플릿 필요` without increasing the native Preview/Run count. Tool selection, layer state, and routing remain unchanged by readiness refresh itself.
- Fresh current-source before/after evidence is under `artifacts\matching_template_readiness_20260714\before` and `artifacts\matching_template_readiness_20260714\after`. Empty workspace, image-loaded workspace, and compact Tool rail regression captures passed with `layout=0`, `text=0`, and `internal=0`.
- The solution build passed with zero warnings/errors. The latest Debug EXE is `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-14 13:55:56 KST`. Readiness, external-reference, public-sample (`30/229/15`), and `git diff --check` passed.
- The same template prerequisite now covers `Matching`, `EdgeBasedMatching`, and `FeatureMatching`. Each row reads the first recipe-owned PropertyGrid property and shows `템플릿 필요` until its path exists and the template image loads successfully.
- EdgeBasedMatching and FeatureMatching registration/clear transitions are exercised through the shared property-save notification without opening a Tool View or increasing Preview/Run count. The smoke resets its three template properties first so repeated runs cannot inherit prior smoke configuration.
- Fresh current-source before/after evidence is under `artifacts\matching_family_template_readiness_20260714\before` and `artifacts\matching_family_template_readiness_20260714\after`; empty workspace, image-loaded workspace, and compact Tool rail regression passed with `layout=0`, `text=0`, and `internal=0`.
- Focused and full solution builds passed with zero warnings/errors. The latest Debug EXE is `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-14 14:08:51 KST`.
- Arithmetic now reuses the execution contract for `Operation`/`Offset`, `Bitwise_NOT`/`ABS`, and constant-input behavior. With only one non-placeholder image layer, a setting that needs B shows `B 입력 필요`; unary, constant, and Offset settings remain `설정 가능`.
- The settings-save and layer-refresh paths update this badge without opening Arithmetic or increasing Preview/Run count. Adding a B image changes the row to `설정 가능`; deleting it restores `B 입력 필요`. The state remains advisory and does not claim that selected routes or image sizes are compatible.
- Fresh current-source before/after evidence is under `artifacts\arithmetic_second_input_readiness_20260714\before` and `artifacts\arithmetic_second_input_readiness_20260714\final`. The image-load, empty-workspace, and compact Tool rail smokes passed with `layout=0`, `text=0`, and `internal=0`.
- Focused and full solution builds passed with zero warnings/errors. The latest Debug EXE is `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-14 14:33:15 KST`.
- The image-ready Threshold, Matching, and Line quick actions now reuse `TopSecondaryCommandButtonStyle`; their text and icons inherit the button foreground so the dark background, hover, and pressed states remain readable without a second palette.
- The workspace image-load smoke now enforces text/background contrast of at least 3.0 for all three quick actions while retaining the existing no-automatic-tool/Preview/Run assertions.
- Fresh current-source before/after evidence is under `artifacts\quick_tool_button_contrast_20260714\before` and `artifacts\quick_tool_button_contrast_20260714\after_visible`. Focused and full solution builds passed with zero warnings/errors. The latest Debug EXE is `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-14 14:50:56 KST`.
- Line now reads the first recipe-owned Line A/B `PIXELPERMM` values and distinguishes `px 전용`, matching positive scale such as `mm 0.006`, and `보정 확인` for missing, invalid, negative, or inconsistent values.
- A matching positive value means only that the configured A/B scale agrees; the tooltip explicitly requires real calibration evidence before mm results are trusted. `px 전용` remains a valid pixel-measurement mode and must not make a physical-unit claim.
- The transition smoke proves `px 전용 -> 보정 확인 -> mm 0.006` through existing PropertyGrid persistence notifications without opening Line, increasing Preview/Run count, creating a layer, or changing routing. Empty-workspace and compact Tool rail regressions also passed with `layout=0`, `text=0`, and `internal=0`.
- Fresh current-source before/after evidence is under `artifacts\line_scale_readiness_20260714\before` and `artifacts\line_scale_readiness_20260714\final_visible`. Focused and full solution builds passed with zero warnings/errors. The latest Debug EXE is `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-14 15:21:47 KST`.
- Required-ROI audit found no truthful generic Tool rail badge target. Blob and Contour already support full-image direct Preview, Line initializes an empty ROI to the full image only when the operator explicitly runs Preview, and Matching-family Preview is blocked by template readiness rather than ROI. The one real required-ROI case is a fixture-consuming pipeline Step, which already fails closed in `VisionPipelineFixtureFrameService` validation. Do not add a misleading generic `ROI needed` badge.
- The expanded Tool rail now has one inline search box that filters the existing items by all whitespace-separated terms. Canonical bilingual metadata covers tool names, inspection intents such as pin gap and defect count, PropertyGrid terms such as `InputLayerB`, and result metrics such as `DistanceMmRange` and `ScoreMax`.
- Search changes item/group visibility only. It preserves selected/readiness state and is smoke-proven not to open tools, run Preview/Run, create layers, change the visible workspace layer, or change input/output routing. The compact icon rail hides the search row and remains clickable.
- Fresh current-source before/after evidence is under `artifacts\tool_finder_search_20260714\before` and `artifacts\tool_finder_search_20260714\after`. The dedicated search, image-load readiness, and compact Tool rail targets passed with `layout=0`, `text=0`, and `internal=0`.
- Found tools now expose a book button only while search is active. All 16 Tool rail menus map to an existing canonical Learn topic, and the command opens or reuses the Learn window at that topic without selecting a Tool View or changing Preview/Run, layer, workspace, or route state.
- Fresh current-source before/after evidence for the Learn shortcut is under `artifacts\tool_finder_learn_link_20260714\before` and `artifacts\tool_finder_learn_link_20260714\after`; the dedicated target also verifies all tool-topic mappings and Learn-window reuse.
- Found tools now also expose an image button that opens the existing Sample Picker at the mapped Learn path. All 16 Tool rail menus have explicit path assertions; the Line search action opens `line` with visible public samples, and cancelling the Picker preserves Preview/Run, layer, workspace, route, and Tool View state.
- Fresh current-source before/after evidence for the sample shortcut is under `artifacts\tool_finder_sample_link_20260714\before` and `artifacts\tool_finder_sample_link_20260714\after`. The same folder contains image-load, compact-rail, and Sample Picker regression captures.
- Found tools now expose a Guided Setup button only for the five existing starter-intent contracts: Line pin gap/pitch, Blob count, Contour shape/count, Matching target presence, and Mean brightness. The action opens Recipe Manager at the existing Guided Setup tab with the mapped intent selected; it does not create Starter XML or change Tool View, Preview/Run, layer, workspace, or route state. The other 11 tools expose no such button.
- Search mode hides the readiness badge so Learn, public sample, and Guided Setup actions remain inside the 220 px Tool rail. The smoke now checks the Guided Setup button's actual bounds and verifies an unsupported Threshold result has no button.
- Fresh current-source before/after evidence is under `artifacts\tool_finder_guided_setup_link_20260714\before` and `artifacts\tool_finder_guided_setup_link_20260714\after`; image-load, compact-rail, and existing Guided Setup regressions are under the same folder's `regression` directory. Build, localization (`1695/77`), readiness, external-reference, and public-sample (`30/229/15`) checks passed. The latest Debug EXE is `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-14 16:41:46 KST`, newer than the latest touched app source.
- P2 Tool Finder is complete at the bounded search/readiness/direct-link scope. Do not add favorites or recommendation scoring until usage evidence shows search is insufficient. P3 has started with the first explicit Recipe review bundle; the next P3 slice is import-side dry validation and path-relocation review using its manifest.

## Latest Recipe Review Bundle Slice On 2026-07-14

- Recipe Manager now exposes `Review bundle` separately from normal XML export and writes a `.review.zip` package.
- Schema v1 contains exactly `pipeline.xml` and `review-manifest.json`. The manifest records application/schema version, XML size/SHA-256, validator errors/warnings, ToolType and enabled counts, Step routes, acceptance gates, pipeline dependency metadata, and selected sample/reference metadata.
- Dependency/reference rows record source/path kind, resolved existence, size, SHA-256, and read errors. The package records `ReferencedOnly`; it does not copy referenced files or private local assets.
- Export is review-only. It does not import, run Preview/Run, create/select a layer, open a Tool View, or change workspace/input/output routing.
- Focused smoke `wpf_shell_host_recipe_review_bundle` opens the current Recipe Manager, exports and reopens the ZIP, verifies its two-entry boundary and hashes/metrics/policy, then checks no runtime or route state changed. Current-source before/final evidence is under `artifacts\recipe_review_bundle_20260714`.
- Regression exposed a real rename defect: Tool readiness could reload against the previous RecipeContext during recipe-change callbacks and recreate the old workspace after `Directory.Move`. `OnRuntimeRecipeChanged` now refreshes RecipeContext before nested layer/readiness refreshes; the broad Recipe Manager language/CRUD/import/export/history smoke passes through rename without retaining the old recipe.
- The run-history baseline ComboBox now carries its existing localized label as automation metadata, preserving keyboard-input identification without adding a visible UI or audio-validation feature.
- Verification passed: solution and screenshot-tool builds reported 0 warnings/0 errors; focused review-bundle, Guided Setup, LLM dependency, operator decision-board, and broad Recipe Manager language/CRUD/history smokes passed with `layout=0`, `text=0`, and `internal=0`; localization was `1695/77`; readiness, external-reference, public-sample (`30/229/15`), and `git diff --check` passed.
- Latest EXE proof used `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll`, built at `2026-07-14 17:27:42 KST`, with `recipe-manager-tabs` reporting `Result: PASS` under `artifacts\recipe_review_bundle_20260714\direct_exe`.
- Import-side dry validation is now implemented. `.review.zip` selection verifies the exact two-entry schema, bounded sizes, package policy, XML size/SHA-256, summary counts, and manifest/XML dependency consistency before loading the XML into the existing `LLM XML` review tab.
- A missing absolute dependency can show one SHA-matched file beside the bundle as `재배치 후보`; OpenVisionLab does not rewrite the XML path or copy the file. Validation remains NG and `Import` remains disabled until the operator explicitly corrects the XML and validates again.
- Tampered XML is rejected before draft exposure. Dependency content changed since export is also blocked from copy/import.
- `wpf_shell_host_recipe_review_bundle_import` proves tamper rejection, relocation evidence, disabled import, and unchanged pipeline/Preview/Run/layer/workspace/routing state. Fresh before/final evidence is under `artifacts\recipe_review_bundle_import_20260714`.
- The latest Debug EXE/DLL was built at `2026-07-14 18:29:07 KST`; direct `recipe-manager-tabs` reports `Result: PASS` under `artifacts\recipe_review_bundle_import_20260714\direct_exe_final`.
- P3 is complete at the reference-only review/handoff scope. Optional redistributable-asset copy stays deferred and must never silently include private/local assets.
- P4 local Validation Sets now have their first executable slice: named recipe-local explicit image lists, expected OK/NG, per-image notes, missing-file blocking, explicit suite execution, and existing run-history reuse. Metadata lives under `VISION\ValidationSets`, outside pipeline enumeration.
- Registration/editing has no Preview/Run, layer, workspace, Tool View, or routing side effects. Focused current-source smoke is `wpf_shell_host_recipe_local_validation_set`; latest Debug EXE `recipe-manager-tabs` also verifies the actual controls and no-side-effect registration.
- Final current-source evidence is `artifacts\local_validation_sets_20260714\final_render\wpf_shell_host_recipe_local_validation_set.png`. Final latest-build EXE evidence and report are under `artifacts\local_validation_sets_20260714\final_exe`; the built EXE/DLL timestamp is `2026-07-14 19:37:58 KST`.
- Verification passed with 0 build warnings/errors, focused local-set smoke `layout=0|text=0|internal=0`, direct EXE `Result: PASS`, readiness, external-reference policy, public-sample policy (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), and `git diff --check`.
- Pipeline inventory pollution found in that current-source evidence is fixed. `RecipeWorkspaceService.GetVisionPipelineNames` now lists only exact no-namespace `VisionPipeline` XML roots while preserving tool-state, active-name, malformed, and unrelated files unchanged.
- Fresh before/after evidence is under `artifacts\pipeline_inventory_filter_20260714\before` and `after_retry`. The latest EXE/DLL was rebuilt at `2026-07-14 19:54:03 KST`; `artifacts\pipeline_inventory_filter_20260714\final_exe\report.txt` is `Result: PASS` and records `PipelineInventory: valid VisionPipeline XML only`.
- P4 top-level folder registration is implemented with explicit OK/NG roles. It ignores unsupported files, excludes subfolders, updates duplicate paths deterministically, and preserves the existing no-side-effect registration contract.
- Current-source before/after evidence is under `artifacts\validation_set_folder_registration_20260715\before` and `after`; the focused smoke reports `layout=0|text=0|internal=0`.
- The full solution build passed with 0 warnings/errors. Latest Debug EXE `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe` was built at `2026-07-15 06:28:56 KST`; `recipe-manager-tabs` reports `Result: PASS` under `artifacts\validation_set_folder_registration_20260715\direct_exe` and records top-level folder filtering plus unchanged Preview/Run, layers, and routing.
- Readiness, external-reference policy, public-sample policy (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), and `git diff --check` passed after the folder slice.
- P4 missing-path repair now applies only to one selected missing row and one operator-selected existing supported image. Duplicate replacement paths are rejected; expected OK/NG and notes are preserved; no recursive or inferred search occurs.
- Current-source before/after evidence is under `artifacts\validation_set_path_repair_20260715\before` and `after_screen`; the focused smoke covers duplicate rejection, metadata preservation, suite re-enable, no runtime side effects, and `layout=0|text=0|internal=0`.
- The first focused run exposed a too-short smoke-only suite wait at progress `2/4`; its wait ceiling was increased without changing product runtime behavior, and repeated focused runs passed.
- Latest Debug EXE `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe` was built at `2026-07-15 07:30:54 KST`. Direct `recipe-manager-tabs` under `artifacts\validation_set_path_repair_20260715\direct_exe` reports `Result: PASS` and records file/folder/repair controls plus preserved metadata, Preview/Run, layers, and routing.
- P4 is complete at the bounded recipe-local Validation Set scope. Recursive search, inferred replacement, automatic path rewriting, and a second runner remain prohibited.

## Product Direction

- OpenVisionLab is an LLM-assisted OpenCvSharp4-based rule-based vision recipe workbench.
- Its purpose is image-based algorithm learning, verification, LLM-assisted XML recipe generation, and recipe composition.
- It is not a camera, lighting, PLC, or I/O integration platform.
- Algorithm tools must stay PropertyGrid-based.
- Preview/Run must be explicit user actions. Layer create/delete/load-image, visibility toggles, and output layer creation must not auto-run tools.
- Viewer zoom/pan/drag, ROI overlay, template editor, layer comparison, and docking features must be preserved.
- Main window title-bar minimize, maximize/restore, and close controls must remain visible and verified. These are window controls, not account/session UI.
- Smoke tests and UI screenshots must use the latest updated EXE or a current-source view generated after the latest relevant source changes. Do not show old artifact images as current UI evidence; label them as historical/baseline only.

## Current Dev Baseline On 2026-07-05

Recent Dev commits on `codex/public-sample-ux-docs` include:

- `487106f Show selected recipe step details`
- `646dce5 Add recipe step comparison grid`
- `8bea861a Explain failed recipe history samples`
- `3d5767ec Show recipe step parameter previews`
- `c1a16bb5 Split recipe review panel into tabs`
- `7e4cd81 Document OpenVisionLab target views`
- `e76a440 Show LLM XML validation issue rows`
- `53fbfc3d Add selected step layer navigation`
- `eeb47e69 Show Good Bad pair role cards`

Current Recipe Manager baseline:

- Searchable recipe list, create, duplicate, rename, delete, XML import/export are already present.
- Recipe Manager is now a workbench-sized overlay with a dedicated recipe library pane, review workspace header, and command strip. It is no longer treated as a small floating settings panel.
- Pipeline review is split into Review, Runs, and XML/Step sub-tabs.
- `Duplicate from sample`, LLM XML validation report, structured LLM XML validation issue rows, LLM XML before/after diff review, actionable dependency/path scan hints and dependency path drill-down rows, pipeline preview step list, Step comparison table, selected Step detail panel, selected Step input/output layer thumbnail cards with click navigation, selected Step ROI/template metadata, selected Step PropertyGrid parameter review with explicit XML apply-back and corrected-output review, branch/output comparison rows for selected multi-step correction paths, Good/Bad role result cards with failed-Step drill-down, failed Step rerun/comparison action strip, and failed-history explanation are already present.
- Top account/operator chrome has been reviewed and removed from Shell Host/Shell Preview. It was only an `Account` icon plus `OperatorText`, with no login/profile/permission command behind it. Keep operator review wording inside Recipe/Pipeline Manager, but do not bring back top-level account UI unless real account/session features are intentionally added.
- Do not re-spend the next session re-evaluating these from scratch unless a regression is reported.

Latest P4 recipe-variant comparison slice:

- Recipe Manager `Pipeline > Review` now shows a read-only comparison between the active pipeline and the selected pipeline variant.
- The report reuses the LLM draft diff engine and exposes step-count and dependency-path deltas, added/removed/changed steps, routing changes, and representative parameter before/after values.
- Selecting a variant still does not activate it and does not trigger Preview/Run. Direct EXE smoke asserts the preview/run counter remains unchanged.
- Before capture: `artifacts\p4_pipeline_variant_diff_before_20260711_01\OpenVisionLab_RecipeManager_PipelineFilter.png`.
- After capture from the current Debug EXE: `artifacts\p4_pipeline_variant_diff_after_20260711_04\OpenVisionLab_RecipeManager_PipelineVariantComparison.png`.
- Verification: `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors; direct EXE `recipe-manager-tabs` passed and records `PipelineVariantComparison: active/selected diff visible without Preview/Run`.
- `OpenVisionReadinessCheck`, `TestExternalReferences.ps1`, and `TestPublicSampleAssets.ps1` also passed; the public sample check reported `CatalogRows=28`, `ManifestAssets=226`, and `Pipelines=14`.
- P4 is not complete. The next bounded priority is to audit an explicit XML review-bundle/dependency-manifest export action that reuses the existing dependency scanner. Do not silently create sidecar files during normal XML export and do not build deployment packaging.

Latest sample catalog and Shell readability slice on 2026-07-12:

- Sample workflow action buttons now use a dark secondary command surface with high-contrast white text/icons; screenshot smoke checks the rendered text/background contrast.
- The default Shell Host bottom bar no longer shows a fake `C:` capacity meter. It mirrors the current recipe, workspace layer, selected tool/task, and operation status.
- `OpenVisionWorkspaceSamplePickerWindow` uses the shared custom OpenVisionLab title bar and keeps minimize, maximize/restore, close, drag, and resize behavior.
- Workspace and Recipe Manager sample selectors exclude `LocalLegacy`; the lower-level loader remains for old recipe/history compatibility.
- Learn document actions now convert all `docs\learn\*.md` files into linked, styled local HTML under `%LOCALAPPDATA%\OpenVisionLab\LearnHtml\v1` and open the selected `.html` in the default browser. Relative tutorial images resolve against the repository Learn source folder.
- Current-source before captures:
  - `artifacts\sample_catalog_shell_before_20260712_01\wpf_shell_host_workspace_sample_open.png`
  - `artifacts\sample_catalog_picker_before_20260712_01\wpf_shell_host_workspace_sample_picker.png`
- Current-source after captures:
  - `artifacts\sample_catalog_shell_after_20260712_02\wpf_shell_host_workspace_sample_open.png`
  - `artifacts\sample_catalog_picker_after_20260712_01\wpf_shell_host_workspace_sample_picker.png`
- Focused smokes assert no auto Preview/Run behavior changed. Final verification commands must still be rerun after this documentation update before reporting completion.

Latest Shell maximize and compact Tool rail slice on 2026-07-12:

- `OpenVisionShellHostWindow` now handles `WM_GETMINMAXINFO` against the current monitor work area. Maximized content stops above the Windows taskbar instead of hiding the bottom status/log controls.
- Compact Tool rail remains 56 px wide and shows the existing tool icons with tooltips and working commands. Clicking a compact Threshold icon selects the tool without triggering Preview/Run.
- The redundant read-only `Scope: {pipeline}` chip is hidden. The adjacent recipe selector is the single operator-facing recipe context; internal recipe/pipeline routing context is unchanged.
- Current-source before captures:
  - `artifacts\shell_chrome_before_20260712_01\wpf_shell_host_tool_rail_compact.png`
  - `artifacts\shell_chrome_before_20260712_01\wpf_shell_host_workspace_empty.png`
- Current-source/current-build after captures:
  - `artifacts\shell_chrome_final_debug_20260712_01\wpf_shell_host_window_maximized.png`
  - `artifacts\shell_chrome_final_debug_20260712_01\wpf_shell_host_tool_rail_compact.png`
  - `artifacts\shell_chrome_final_debug_20260712_01\wpf_shell_host_workspace_empty.png`
  - `artifacts\shell_chrome_final_debug_20260712_01\wpf_shell_host_recipe_context_switch.png`
- Focused screenshot smokes require maximized bounds to remain inside the Windows work area, at least ten visible compact tool buttons, a working Threshold icon command, no compact-icon Preview/Run side effect, no visible `HostRecipeContext`, and no visible `범위:` text.

Latest Color/HSV Learn-to-tool slice on 2026-07-13:

- The Color/HSV Learn topic now exposes `HSV Tool 열기` and names the Hue/Saturation/Value, ROI, and OutputLayer locations to inspect in the existing PropertyGrid Tool View.
- The action only opens/selects HSV. It must not run Preview/Run, create a result layer, or change input routing.
- Focused coverage is in `wpf_openvision_learn_color_hsv` and the Shell-backed `wpf_shell_host_learn_entry` target.
- Current-source before: `artifacts\learn_color_tool_link_before_20260713_01\wpf_openvision_learn_color_hsv.png`.
- Current-source after: `artifacts\learn_color_tool_link_after_20260713_04\wpf_openvision_learn_color_hsv.png`.
- Verification: solution build passed with 0 warnings and 0 errors; `wpf_openvision_learn_color_hsv`, `wpf_shell_host_learn_entry`, `wpf_simple_preprocess_tool_learn_button`, readiness, external references, and public sample assets passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Color/HSV Split/Merge Learn slice on 2026-07-13:

- The existing Color/HSV animation now runs four explicit stages: BGR input, `Cv2.Split` into B/G/R `CV_8UC1` Mats, `Cv2.Merge` plus BGR-to-HSV conversion, and HSV gate/mask review.
- The visual sample keeps BGR `(25,185,105)`, split values B=25/G=185/R=105, merged BGR `(25,185,105)`, and converted HSV `(45,221,185)` aligned with an actual one-pixel OpenCvSharp smoke assertion.
- Split/Merge remains Learn-only. No new ToolType was added because there is no separate operator workflow, metric contract, or public sample requiring a channel tool.
- Current-source before: `artifacts\learn_color_split_merge_before_20260713_01\wpf_openvision_learn_color_hsv.png`.
- Current-source after: `artifacts\learn_color_split_merge_after_20260713_04\wpf_openvision_learn_color_hsv.png`.
- Verification: solution build passed with 0 warnings and 0 errors; `wpf_openvision_learn_color_hsv`, `wpf_shell_host_learn_entry`, `wpf_simple_preprocess_tool_learn_button`, readiness, external references, and public sample assets passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Brightness/Histogram Learn-to-tool slice on 2026-07-13:

- The Brightness/Histogram Learn topic now exposes `Mean Tool 열기` and `Histogram Tool 열기` using the existing Shell related-tool callback.
- Mean points operators to `Mean Type`, `Min Mean`, and `Max Mean`; Histogram points to `Type`, `Clip Limit`, `Tile Grid`, and `Normalize Alpha/Beta`.
- Standalone Learn keeps both buttons disabled. Shell actions only open/select the Tool View and must not run Preview/Run, create a result layer, or change input routing.
- Current-source before: `artifacts\learn_brightness_tool_links_before_20260713_01\wpf_openvision_learn_brightness.png`.
- Current-source after: `artifacts\learn_brightness_tool_links_after_20260713_06\wpf_openvision_learn_brightness.png`.
- Focused coverage is in `wpf_openvision_learn_brightness` and the Shell-backed `wpf_shell_host_learn_entry` target.

Latest Arithmetic Learn-to-tool slice on 2026-07-13:

- The Arithmetic Learn topic now exposes `Arithmetic Tool 열기` using the existing Shell related-tool callback.
- The location guide names the actual double-input route and parameter areas: `Input A`, `Input B`, `Output Layer`, `Mode`, `Arithmetic Type`, and `Input B Source`.
- Standalone Learn keeps the button disabled. The Shell action only opens/selects Arithmetic and must not run Preview/Run, create a result layer, or change either input route.
- Current-source before: `artifacts\learn_arithmetic_tool_link_before_20260713_01\wpf_openvision_learn_arithmetic.png`.
- Current-source after: `artifacts\learn_arithmetic_tool_link_after_20260713_02\wpf_openvision_learn_arithmetic.png`.
- Focused coverage is in `wpf_openvision_learn_arithmetic`, `wpf_shell_host_learn_entry`, and the existing reverse `wpf_arithmetic_tool_learn_button` target.
- Verification: solution and screenshot-tool builds passed with 0 warnings and 0 errors; all three focused targets, readiness, external references, and public sample assets passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Geometry Learn-to-tool slice on 2026-07-13:

- The Geometry Learn topic now exposes `Rotate / Scale Tool 열기` using the existing Shell related-tool callback.
- The location guide names the shared input/output route and actual `Angle`, `Scale X`, and `Scale Y` fields, and explains that `OutputSize` is an explicit Preview result rather than another input.
- Standalone Learn keeps the button disabled. The Shell action only opens/selects RotateScale and must not run Preview/Run, create a result layer, or change routing.
- Current-source before: `artifacts\learn_geometry_tool_link_before_20260713_01\wpf_openvision_learn_geometry.png`.
- Current-source after: `artifacts\learn_geometry_tool_link_after_20260713_02\wpf_openvision_learn_geometry.png`.
- Focused coverage is in `wpf_openvision_learn_geometry`, `wpf_shell_host_learn_entry`, and the existing reverse `wpf_simple_preprocess_tool_learn_button` target.
- Verification: solution and screenshot-tool builds passed with 0 warnings and 0 errors; all three focused targets, readiness, external references, and public sample assets passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Filtering Learn-to-tool slice on 2026-07-13:

- The Filtering Learn topic now exposes `Filter Tool 열기` using the existing Shell related-tool callback.
- The location guide names the shared route, `Filter Type`, `Border Type`, Kernel `Width/Height`, and the type-specific Median/Bilateral fields.
- Standalone Learn keeps the button disabled. The Shell action only opens/selects Filter and must not run Preview/Run, create a result layer, or change routing.
- Current-source before: `artifacts\learn_filter_tool_link_before_20260713_01\wpf_openvision_learn_filtering.png`.
- Current-source after: `artifacts\learn_filter_tool_link_after_20260713_02\wpf_openvision_learn_filtering.png`.
- Focused coverage is in `wpf_openvision_learn_filtering`, `wpf_shell_host_learn_entry`, and the existing reverse Filter header Learn assertion in `wpf_filter_morphology_layout_guard`.
- Verification: solution and screenshot-tool builds passed with 0 warnings and 0 errors; all three focused targets, readiness, external references, and public sample assets passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Morphology Learn-to-tool slice on 2026-07-13:

- The Morphology Learn topic now exposes `Morphology Tool 열기` using the existing Shell related-tool callback.
- The location guide names the shared input/output route, `Operation`, Kernel `Width/Height`, `3 x 3`/`5 x 5`/`7 x 7` presets, and `Rect`/`Ellipse`/`Cross` Shape choices.
- Standalone Learn keeps the button disabled. The Shell action only opens/selects Morphology and must not run Preview/Run, create a result layer, or change routing.
- Current-source before: `artifacts\learn_morphology_tool_link_before_20260713_01\wpf_openvision_learn_morphology.png`.
- Current-source after: `artifacts\learn_morphology_tool_link_after_20260713_02\wpf_openvision_learn_morphology.png`.
- Focused coverage is in `wpf_openvision_learn_morphology`, `wpf_shell_host_learn_entry`, and the existing reverse Morphology header Learn assertion in `wpf_filter_morphology_layout_guard`.
- Verification: solution and screenshot-tool builds passed with 0 warnings and 0 errors; all three focused targets, readiness, external references, and public sample assets passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Blob Learn-to-tool slice on 2026-07-13:

- The Blob Learn topic now exposes `Blob Tool 열기` using the existing Shell related-tool callback.
- The location guide names the actual Blob PropertyGrid fields: `Use ROI`, `ROI`, Blob Parameter `Min area`, and `Max area`; it separates those setup values from the post-run `ResultCount`, `AreaMin/AreaMax`, and `BoundsWidth/BoundsHeight` metrics.
- Standalone Learn keeps the button disabled. The Shell action only opens/selects Blob and must not run Preview/Run, create a result layer, or change routing.
- Current-source before: `artifacts\learn_blob_tool_link_before_20260713_01\wpf_openvision_learn_blob.png`.
- Current-source after: `artifacts\learn_blob_tool_link_after_20260713_02\wpf_openvision_learn_blob.png`.
- Focused coverage is in `wpf_openvision_learn_blob`, `wpf_shell_host_learn_entry`, and the existing reverse `wpf_shell_host_blob_tool` target.
- Verification: solution and screenshot-tool builds passed with 0 warnings and 0 errors; all three focused targets, readiness, external references, and public sample assets passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Contour Learn-to-tool slice on 2026-07-13:

- The Contour Learn topic now exposes `Contour Tool 열기` using the existing Shell related-tool callback.
- The location guide names the actual Contour PropertyGrid fields: `컨투어 표시`, `Retrieval mode`, `Min area`, `Max area`, plus optional approximation and drawing fields; it separates those setup values from the post-run `ResultCount`, `AreaMax`, `BoundsWidthMax`, and `BoundsHeightMax` metrics.
- Standalone Learn keeps the button disabled. The Shell action only opens/selects Contour and must not run Preview/Run, create a result layer, or change routing.
- Current-source before: `artifacts\learn_contour_tool_link_before_20260713_01\wpf_openvision_learn_contour.png`.
- Current-source after: `artifacts\learn_contour_tool_link_after_20260713_02\wpf_openvision_learn_contour.png`.
- Focused coverage is in `wpf_openvision_learn_contour`, `wpf_shell_host_learn_entry`, and the existing reverse `wpf_shell_host_contour_tool` target.
- Verification: solution and screenshot-tool builds passed with 0 warnings and 0 errors; all three focused targets, readiness, external references, and public sample assets passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Edge/Line Learn-to-tool slice on 2026-07-13:

- The Edge/Line Learn topic now exposes separate `EdgeDetection Tool 열기` and `Line Tool 열기` actions using the existing Shell related-tool callback.
- The role guide keeps the tools distinct: EdgeDetection creates an edge-map layer from Canny/Sobel/Scharr/Laplacian settings; Line performs ROI-based edge/fit-line work using Purpose, Line A/B, ROI, edge polarity/direction/contrast/thickness, and scan settings.
- Standalone Learn keeps both buttons disabled. Shell actions only open/select the requested Tool View and must not run Preview/Run, create a result layer, or change routing.
- The existing Line Tool header Learn route remains topic 8 LineDistance because the same Tool View owns Measure and Intersection purposes; the Edge/Line topic does not replace that route.
- Current-source before: `artifacts\learn_edge_line_tool_links_before_20260713_01\wpf_openvision_learn_edge_line.png`.
- Current-source after: `artifacts\learn_edge_line_tool_links_after_20260713_02\wpf_openvision_learn_edge_line.png`.
- Focused coverage is in `wpf_openvision_learn_edge_line`, `wpf_shell_host_learn_entry`, `wpf_simple_preprocess_tool_learn_button`, and `wpf_shell_host_line_tool`.

Latest LineDistance Learn-to-tool slice on 2026-07-14:

- The LineDistance Learn topic now exposes `Line Tool 열기` and points to the existing Line Tool's `Purpose > Measure`, Line A/B, ROI, `Pixel / mm`, edge/scan fields, and average plus range/max result review.
- The action opens/selects the Line Tool View only. It deliberately does not auto-select Measure, mutate parameters, run Preview/Run, create an output layer, or change input routing.
- Current-source before: `artifacts\learn_line_distance_tool_link_before_20260714_02\wpf_openvision_learn_line_distance.png`.
- Current-source after: `artifacts\learn_line_distance_tool_link_after_20260714_02\wpf_openvision_learn_line_distance.png`.
- Focused coverage: `wpf_openvision_learn_line_distance`, Shell-backed `wpf_shell_host_learn_entry`, and reverse `wpf_shell_host_line_tool`.
- Verification: solution and screenshot-smoke builds passed with 0 warnings and 0 errors; all three focused smoke targets, repository readiness, external-reference, and public-sample checks passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Matching Learn-to-tool slice on 2026-07-14:

- The Matching Learn topic now exposes `Matching Tool 열기` and names the actual operator locations: Tool Shell `Template Ready`, PropertyGrid `Pattern path`, `Matching > Min score`, `Match count`, ROI, and optional angle/scale search.
- Result guidance keeps setup and evidence distinct: the operator explicitly runs Preview or Run Review, checks overlay position, and interprets `ScoreMax` together with `ResultCount`.
- The action opens/selects the Matching Tool View only. It does not register a template, mutate parameters, run Preview/Run, create an output layer, or change input routing.
- Current-source before: `artifacts\learn_matching_tool_link_before_20260714_01\wpf_openvision_learn_matching.png`.
- Current-source after: `artifacts\learn_matching_tool_link_after_20260714_04\wpf_openvision_learn_matching.png`.
- Focused coverage: `wpf_openvision_learn_matching`, Shell-backed `wpf_shell_host_learn_entry`, and reverse `wpf_shell_host_matching_tool`.
- Verification: solution and screenshot-smoke builds passed with 0 warnings and 0 errors; all three focused smoke targets, repository readiness, external-reference, and public-sample checks passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest FeatureMatching Learn-to-tool slice on 2026-07-14:

- The FeatureMatching Learn topic now exposes `FeatureMatching Tool 열기` and names only the fields the current PropertyGrid actually owns: Tool Shell `Template Ready`, `Feature template path`, `Matching > Ratio threshold`, `RANSAC tolerance`, and ROI.
- The serialized key remains `SCORE_MIN`, but the PropertyGrid and Learn text identify it as the Lowe Ratio threshold: smaller values are stricter. Learn keeps GoodMatches as a concept, while explicit Preview or Run Review evidence uses overlay position, `ScoreMax`, and `ResultCount`.
- The action opens/selects the FeatureMatching Tool View only. It does not register a template, mutate parameters, run Preview/Run, create an output layer, or change input routing.
- Current-source before: `artifacts\learn_feature_matching_tool_link_before_20260714_01\wpf_openvision_learn_feature_matching.png`.
- Current-source after: `artifacts\learn_feature_matching_tool_link_after_20260714_02\wpf_openvision_learn_feature_matching.png`.
- Focused coverage: `wpf_openvision_learn_feature_matching`, Shell-backed `wpf_shell_host_learn_entry`, and reverse `wpf_shell_host_feature_matching_tool`.
- Verification: solution and screenshot-smoke builds passed with 0 warnings and 0 errors; all three focused smoke targets, repository readiness, external-reference, and public-sample checks passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest EdgeBasedMatching Learn-to-tool slice on 2026-07-14:

- The shared Matching animation panel now switches its related action to `EdgeBasedMatching Tool 열기` for the EdgeBasedMatching topic and opens the actual EdgeBasedMatching PropertyGrid Tool View.
- The location guide names the current fields: Tool Shell `Template Ready`, `Pattern path`, `Matching > Min score / Match count`, `Edge Model > Canny range / Max template points`, `Search > Search step`, ROI, and optional angle/scale search.
- The action opens/selects the Tool View only. It does not register a template, mutate parameters, run Preview/Run, create an output layer, or change input routing.
- Current-source before: `artifacts\learn_edge_based_matching_tool_link_before_20260714_01\wpf_openvision_learn_edge_based_matching.png`.
- Current-source after: `artifacts\learn_edge_based_matching_tool_link_after_20260714_02\wpf_openvision_learn_edge_based_matching.png`.
- Focused coverage: `wpf_openvision_learn_edge_based_matching`, Shell-backed `wpf_shell_host_learn_entry`, and reverse `wpf_shell_host_edge_based_matching_tool`.
- Verification: solution and screenshot-smoke builds passed with 0 warnings and 0 errors; all three focused smoke targets, repository readiness, external-reference, and public-sample checks passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Matching-family semantics audit on 2026-07-14:

- Runtime inspection confirmed that FeatureMatching `SCORE_MIN` is used as the Lowe descriptor-ratio threshold (`best.Distance < SCORE_MIN * second.Distance`), so smaller values are stricter. The serialized key remains unchanged for recipe compatibility.
- FeatureMatching PropertyGrid, preset descriptions, Learn guidance, localization defaults/migration, XML authoring guide, and tool catalog now use the same Ratio/RANSAC meaning. Exact-default localization migration updates the previous built-in text without overwriting user-customized translations.
- FeatureMatching practice guidance now separates setup from evidence: configure Ratio/RANSAC, explicitly run Preview or Run Review, then compare `GoodMatches`, `ScoreMax`, and overlay position.
- Current-source before: `artifacts\matching_family_semantics_before_20260714_01\wpf_openvision_learn_feature_matching.png` and `artifacts\matching_family_semantics_before_20260714_01\wpf_shell_host_feature_matching_tool.png`.
- Current-source after: `artifacts\matching_family_audit_20260714_03_feature\wpf_openvision_learn_feature_matching.png` and `artifacts\matching_family_audit_20260714_01\wpf_shell_host_feature_matching_tool.png`.
- Verification: solution and screenshot-smoke builds passed with 0 warnings and 0 errors; Matching, EdgeBasedMatching, and FeatureMatching Learn targets, Shell Learn entry, and all three reverse Tool targets passed with `layout=0`, `text=0`, and `internal=0`. Repository readiness, external-reference, and public-sample checks passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Recipe Manager Run History next-action visibility slice on 2026-07-14:

- Fresh current-EXE evidence showed that the selected run review could grow with its content and placed the operator's `Next` guidance after run summary, sample, result, and linked-Step lines. On the 1600x900 workbench the next action was only partially visible near the footer.
- `HostRecipeSelectedRunReview` now keeps a stable 72-88px scrollable height. The review order is `Run -> Sample -> Result -> Next -> Linked step -> Summary`, so the action appears in the first four lines while full detail remains available through the internal scrollbar and copy command.
- Direct smoke now asserts both the bounded review height and first-four-lines next-action contract. It does not add or trigger Preview/Run, layer creation, input routing changes, or recipe mutation.
- Current-build before: `artifacts\recipe_manager_llm_ux_audit_before_20260714_01\OpenVisionLab_RecipeManager_RunHistory.png`.
- Current-build after: `artifacts\recipe_manager_run_history_after_20260714_04\OpenVisionLab_RecipeManager_RunHistory.png`.
- Verification: fresh solution build passed with 0 warnings and 0 errors; `OpenVisionLab.exe --smoke recipe-manager-tabs` passed from the latest Debug build with the existing LLM validation/import blocks, Good/Bad review, Pipeline review, and explicit execution contracts intact. Repository readiness, external-reference, and public-sample checks passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Current priority order:

1. If a real API key or manual transcript is available, collect one GPT/Gemini/Claude XML correction-loop transcript using `docs\OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md` and `docs\OPENVISIONLAB_LLM_TOOL_CATALOG.json`, then validate/import it in Recipe Manager. Do not fabricate transcript evidence.
2. If no real transcript is available, continue Recipe Manager/LLM Assistant UX work only where fresh current-EXE/current-source screenshots show actual clipping, overlap, unclear next action, or workflow friction.
3. Expand branch/output comparison only when a real multi-branch recipe exceeds the current selected-step producer/consumer model and the existing BentPin plus Contour_AllSymbolsAndFaint coverage.
4. Continue Tool View code-behind cleanup only where established controller/presenter/base patterns fit; current test hooks and preview command paths are in use and should not be removed just to reduce line count. The double-input Arithmetic shell, Blob/Contour/Line single-input PropertyGrid shell, and Matching-family single-input PropertyGrid shell now have shared bases, so do not recreate those extractions.

Latest UI evidence for pin-gap intent ROI suggestion:

- Scope: pin-gap intent skills now treat unmarked requests as whole pin-array inspection samples, not one arbitrary pair, and expose a `Sample ROI` button beside the ROI sample field.
- Product contract: `Sample ROI` is a starter ROI helper. It scales the existing whole-array ROI samples from the selected sample/reference image size. It does not claim automatic visual understanding, does not create a run result, and does not trigger Preview/Run.
- Before: `artifacts\pin_gap_roi_suggest_before_20260707_r1\OpenVisionLab_RecipeManager_LlmIntentSkills_PinGap.png`.
- After: `artifacts\pin_gap_roi_suggest_after_20260707_r1\OpenVisionLab_RecipeManager_LlmIntentSkills_PinGap.png`.
- Current-build UI smoke: `dotnet run --no-build --project OpenVisionLab.csproj -c Debug -- --smoke recipe-manager-llm-intent-skills artifacts\pin_gap_roi_suggest_after_20260707_r1` passed after a fresh Debug build.
- Current-build direct smoke: `artifacts\pin_gap_roi_suggest_recipe_manager_tabs_20260707_r1\report.txt` passed with `LlmPinGapRoiSuggest: selected sample image suggested multi-sample ROI without Preview/Run`.
- Current-build XML/image run: `artifacts\pin_gap_roi_suggest_generated_xml_image_run_20260707_r1\report.txt` passed on `Sample\EasyGauge\Pin 1.jpg` with final layer `PinArray_Review` and `MergeOverlayCount=24`.

Latest LLM prompt evidence for pin-gap GPT handoff:

- Scope: when `Pin gap / edge distance (LineDistance)` is selected, Recipe Manager `Build prompt` now embeds a self-contained GPT task packet for pin gap/pitch/edge-to-edge distance XML. It includes whole-array default scope, ROI samples, DistanceMmAvg and DistanceMmRange gates, mm/px, LineDistance-only constraints, OverlayMerge review, and XML-only response format.
- Product contract: this reduces manual document hunting for the operator. The preferred user flow is Recipe Manager -> LLM XML -> select pin-gap intent -> set ROI/spec fields -> `Build prompt` -> `Copy prompt` -> paste into GPT with the image. File packets under `llm_prompt_packets\pin_gap_distance` remain as fallback/reference material.
- Before: `artifacts\pin_gap_prompt_packet_before_20260707_r1\OpenVisionLab_RecipeManager_LlmIntentLineDistance.png`.
- After: `artifacts\pin_gap_prompt_packet_after_20260707_r1\OpenVisionLab_RecipeManager_LlmIntentLineDistance.png`.
- Current-build direct smoke: `artifacts\pin_gap_prompt_packet_after_20260707_r1\report.txt` passed with `LlmPinGapPromptPacket: copy-ready GPT XML-only packet copied`.
- Current-build XML/image run: `artifacts\pin_gap_prompt_packet_generated_xml_image_run_20260707_r1\report.txt` passed on `Sample\EasyGauge\Pin 1.jpg` with final layer `PinArray_Review`.

Latest UI evidence for corrected-output review after Step XML apply:

- Before: `artifacts\corrected_output_review_before_20260705_r1\OpenVisionLab_RecipeManager_StepPropertyGrid.png`
- After: `artifacts\corrected_output_review_after_20260705_r1\OpenVisionLab_RecipeManager_StepPropertyGrid.png`
- Direct EXE smoke: `artifacts\corrected_output_review_after_20260705_r1\report.txt` with `Result: PASS`, `CorrectedOutputReview: visible after XML apply`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.
- Structure note: `HostRecipeCorrectedOutputReviewPanel` appears under the embedded Step PropertyGrid status. It reuses existing explicit output navigation and Good/Bad rerun commands; XML apply still does not run Preview/Run.

Latest UI evidence for LLM dependency path drill-down rows:

- Before: `artifacts\corrected_output_review_after_20260705_r1\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_dependency_drilldown_after_20260705_r2\OpenVisionLab_RecipeManager_LlmXml.png`
- Direct EXE smoke: `artifacts\llm_dependency_drilldown_after_20260705_r2\report.txt` with `Result: PASS`, `LlmDependencyRows: 1`, and `LlmXmlDiff: visible`.
- Structure note: `LlmXmlDraftDependencyRows` exposes row-level status, step, parameter, path, and action. The text dependency report remains for copy/paste review.

Latest UI evidence for Recipe Manager internal workbench density adjustment:

- Before: `artifacts\llm_dependency_drilldown_after_20260705_r2\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\recipe_manager_workbench_layout_after_20260705_r2\OpenVisionLab_RecipeManager_Pipeline.png`
- Direct EXE smoke: `artifacts\recipe_manager_workbench_layout_after_20260705_r2\report.txt` with `Result: PASS`, `CorrectedOutputReview: visible after XML apply`, `LlmDependencyRows: 1`, and `MovedTo: -64.0,18.0`.
- Structure note: Pipeline tab internal management column is narrower so the Step review/PropertyGrid area gets more horizontal room on large workbench screens.

Latest UI evidence for branch/output comparison rows:

- True before note: `artifacts\branch_output_comparison_before_20260705_r1\wpf_shell_host_recipe_language_controls.png` was captured from the current smoke target but Visual Studio was in front, so it is not a clean true-before UI capture. Treat the immediately previous Recipe Manager layout capture as the closest baseline: `artifacts\recipe_manager_workbench_layout_after_20260705_r2\OpenVisionLab_RecipeManager_Pipeline.png`.
- After full-window capture: `artifacts\branch_output_comparison_after_20260705_r5_screenshot_smoke\wpf_shell_host_recipe_language_controls.png`
- Direct EXE smoke: `artifacts\branch_output_comparison_after_20260705_r2\report.txt` with `Result: PASS` and `BranchOutputComparison: 2`.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\branch_output_comparison_after_20260705_r5_screenshot_smoke` passed with `layout=0`, `text=0`, and `internal=0`.
- Structure note: `HostRecipeBranchOutputComparisonPanel` shows selected Step, same-input candidates, input producers, and output consumers for the selected multi-step route. Step navigation still does not run Preview/Run.

Latest UI evidence for selected Step operator context:

- Before: `artifacts\operator_step_review_before_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- After: `artifacts\operator_step_context_after_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- Direct EXE smoke: `artifacts\operator_step_context_after_20260706_r1_direct\report.txt` passed and now checks `PipelineSelectedStepOperatorContextText`.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\operator_step_context_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.
- Structure note: `HostRecipePipelineSelectedStepOperatorContext` sits inside the selected Step detail panel and summarizes selected Step, route, Good/Bad or run-history failure link, and next action. It is read-only guidance and does not run Preview/Run.

Latest UI evidence for Recipe Manager XML/Step list density:

- Before: `artifacts\recipe_manager_density_current_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- After: `artifacts\recipe_manager_density_after_step_list_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_density_after_step_list_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.
- Structure note: `HostRecipePipelineInlinePreviewStepList` now appears immediately after the XML/Step flow focus strip, before branch/output and selected-Step detail panels. On a 1600x900 workbench screenshot the Step rows are visible in the first view instead of only the list title being pushed near the footer. This is a layout-only change and does not add Preview/Run triggers.

Latest UI evidence for compact Recipe Manager recipe/Step rows:

- Before: `artifacts\recipe_manager_compact_step_rows_before_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- After: `artifacts\recipe_manager_compact_step_rows_after_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_compact_step_rows_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.
- Direct EXE smoke: `artifacts\recipe_manager_compact_step_rows_after_20260706_r1_direct\report.txt` passed with `Result: PASS`, `BranchOutputComparison: 2`, and `ActualMultiBranchComparison: 7`.
- Structure note: the recipe library list now stretches item content so long names use ellipsis and tooltips predictably. The XML/Step inline Step list now uses single-line ellipsis rows for Display, Route, and Parameters instead of multi-line wrapping, preserving workbench density for long routes and parameter previews.

Latest UI evidence for large Recipe Manager library filtering:

- Before: `artifacts\recipe_large_library_before_20260706_r1\wpf_shell_host_recipe_large_library.png`
- After: `artifacts\recipe_large_library_after_20260706_r1\wpf_shell_host_recipe_large_library.png`
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_large_library artifacts\recipe_large_library_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.
- Direct EXE smoke: `artifacts\recipe_large_library_after_20260706_r1_direct\report.txt` passed with `Result: PASS`, `ActualMultiBranchComparison: 7`, and `ActualThreeWayBranchComparison: 5`.
- Structure note: `RecipeLibrarySummaryText` now shows total or filtered/total recipe count, for example `레시피 라이브러리 (10/101)`, above the recipe list. The screenshot smoke creates 100 long temporary recipe names, filters to `Category_07`, verifies 10 visible matches, then cleans the temporary workspaces.

Latest UI evidence for large Recipe Manager pipeline filtering:

- Before: `artifacts\recipe_large_pipeline_list_before_20260706_r1\wpf_shell_host_recipe_large_pipeline_list.png`
- After: `artifacts\recipe_large_pipeline_list_after_20260706_r1\wpf_shell_host_recipe_large_pipeline_list.png`
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_large_pipeline_list artifacts\recipe_large_pipeline_list_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.
- Direct EXE smoke: `artifacts\recipe_large_pipeline_list_after_20260706_r1_direct\report.txt` passed with `Result: PASS`, `ActualMultiBranchComparison: 7`, and `ActualThreeWayBranchComparison: 5`.
- Structure note: `PipelineListSummaryText` now shows total or filtered/total pipeline count, for example `파이프라인 (10/109)`, above the pipeline list. The screenshot smoke creates one temporary recipe with 100 long pipeline names, filters to `Group_07`, verifies 10 visible matches, then deletes the temporary workspace.

Latest actual multi-branch branch/output coverage:

- Real sample scan found `docs\samples\BentPin_TopBottom_Overlay.pipeline.xml` with `Main` fan-out and `BentPin_Clean` multi-consumer routes; `docs\samples\Contour_AllSymbolsAndFaint_LLM.pipeline.xml` also has `Main` fan-out.
- Direct EXE smoke now imports `docs\samples\BentPin_TopBottom_Overlay.pipeline.xml` and verifies both output consumers from `BentPin_Clean` plus the same-input and input-producer rows around the selected multi-branch Steps.
- Evidence: `artifacts\recipe_manager_density_after_step_list_20260706_r2_direct\report.txt` passed with `ActualMultiBranchComparison: 7`.
- Structure note: this expands verification of the existing branch/output comparison model for a real multi-branch recipe. It does not create a new comparison surface.

Latest actual 3+ branch fan-out coverage:

- Real sample scan found only one 3+ branch candidate in the current sample set: `docs\samples\Contour_AllSymbolsAndFaint_LLM.pipeline.xml`, where `Main` feeds 4 enabled Steps.
- Screenshot smoke target `wpf_shell_host_recipe_multibranch_comparison` imports that pipeline, selects `Main -> TextSymbol_Binary`, and verifies 3 same-input alternatives plus one output consumer row.
- UI evidence: `artifacts\recipe_multibranch_comparison_after_20260706_r1\wpf_shell_host_recipe_multibranch_comparison.png`.
- Direct EXE smoke evidence: `artifacts\recipe_multibranch_comparison_after_20260706_r1_direct\report.txt` passed with `ActualThreeWayBranchComparison: 5`.
- Decision: no new branch/output UI surface was added. The current selected-step producer/consumer list shows all rows for this real 3+ branch sample on the 1600x900 workbench screenshot, so broader UI expansion should wait for a real sample that exceeds this map.

Latest LLM XML authoring reference:

- Added `docs\OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md` as the external GPT/Gemini/Claude prompt-side API guide for OpenVisionLab `VisionPipeline` XML authoring and correction-loop transcript collection.
- Added `docs\OPENVISIONLAB_LLM_TOOL_CATALOG.json` as the machine-readable tool/parameter/metric catalog for LLM prompt packets.
- Source evidence checked before writing: `OpenVisionShellHostRecipeCommandSurface.cs`, `VisionPipelineValidation.cs`, `VisionPipelineStepParameterSchema.cs`, `VisionPipelineKnownMetrics.cs`, `VisionPipelineArithmeticStep.cs`, `docs\samples\*.pipeline.xml`, and LLM direct smoke cases in `OpenVisionLabDirectSmokeRunner.cs`.
- Usage rule: when collecting real LLM transcripts, give the LLM the guide and JSON catalog first, ask for one `VisionPipeline` XML only, validate inside Recipe Manager, then feed back the validation/dependency report for repair. Do not commit raw transcripts until private names/assets are scrubbed.
- Corpus rule: store real external LLM prompt/response under `artifacts\llm_transcripts\raw`, user-provided/operator-repaired XML replay cases under `artifacts\llm_transcripts\manual`, and sanitized replay candidates under `artifacts\llm_transcripts\sanitized`. Manual replay is validation evidence, not real GPT/Gemini/Claude transcript evidence.
- Validation rules now documented for external LLMs include supported ToolType names, `Main`/previous-output layer routing, `ALLOW_BRANCH_INPUT`, dependency path safety, `Inspection.*` review channel handling, 0..1 matching score parameters, gray-value ranges, Arithmetic `InputLayerB`, OverlayMerge final review intent, and acceptance metric use.

Latest LLM intent-contract replay evidence:

- User-provided pin image XML replay is local-only under `artifacts\llm_transcripts\manual\20260706_user_pins_marked_roi_length_measure.xml`. It is a manual draft/replay artifact, not a real external GPT/Gemini/Claude transcript.
- Product lesson: when the selected intent is pin-to-pin, edge-to-edge, gap, pitch, width, or clearance, OpenVisionLab must lock the tool family to `LineDistance`. `Contour` plus `BoundsHeightAvg` does not satisfy a distance intent.
- Source behavior: `OpenVisionShellHostRecipeCommandSurface.AppendLlmIntentContractValidation` blocks drafts whose selected `Pin gap / edge distance (LineDistance)` intent lacks an enabled `LineDistance` or accepted `LineDistanceGauge` step.
- Latest direct EXE smoke: `dotnet run --no-build --project OpenVisionLab.csproj -c Debug -- --smoke recipe-manager-llm-intent-skills artifacts\llm_manual_replay_contract_after_20260707_r1` passed with `Result: PASS`, `PinGapContourMismatch: blocked by intent contract`, and `PreviewRunCountUnchanged: 0`.
- Validation evidence: `artifacts\llm_manual_replay_contract_after_20260707_r1\PinGapContourMismatchValidation.txt` reports `Error: Intent contract mismatch. Selected intent 'Pin gap / edge distance' requires ToolType=LineDistance.` and `Draft enabled ToolTypes: Contour, Threshold`.
- UI evidence: `artifacts\llm_manual_replay_contract_after_20260707_r1\OpenVisionLab_RecipeManager_LlmIntentSkills_PinGapContourMismatch.png`.
- Next transcript work remains unchanged: capture one real GPT/Gemini/Claude correction-loop transcript when an API key or manually exported transcript is available. Do not treat this manual replay as that evidence.

Latest Recipe Manager sample summary density evidence:

- Before/current capture: `artifacts\recipe_manager_current_density_review_20260706_r1\wpf_shell_host_recipe_language_controls.png`.
- After capture: `artifacts\recipe_manager_density_after_sample_summary_20260706_r1\wpf_shell_host_recipe_language_controls.png`.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_density_after_sample_summary_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.
- Structure note: the Recipe Manager left library pane now uses a 320px baseline width, and the sample acceptance summary shortens the displayed sample id while preserving the full summary as a tooltip. This is a layout/readability change only and does not add Preview/Run behavior.

Latest Tool View code-behind candidate review:

- Search evidence: `rg -n "SetTemplatePathForTest|ConfigurePropertyForTest|ApplyPresetForTest|ResultReviewTextForTest|ConsumeThresholdTeachingPreviewRequest" .` shows these hooks are used by native tool document, preview executor, and smoke paths.
- Decision: no code-behind deletion was made in this pass. Removing these forwarding/test hooks would be higher risk than value until a natural controller/base extraction target appears.

Latest Tool View code-behind cleanup for Blob/Contour single-input PropertyGrid base:

- Added `VisionToolSingleInputPropertyToolViewBase` and `IVisionToolSingleInputPropertyToolController` so Blob/Contour no longer duplicate source/destination layer events, preview image command events, selected layer getters, preview setters, status setter, and controller disposal.
- `BlobToolWpfView.xaml` and `ContourToolWpfView.xaml` now use `VisionToolSingleInputPropertyToolViewBase` as the XAML root.
- `BlobToolWpfView.xaml.cs` and `ContourToolWpfView.xaml.cs` now keep only tool-specific presenter setup, threshold teaching preview state, property creation, and area result review.
- Code-behind reduction: repeated forwarding removal reduced Blob and Contour from roughly 160 lines each to roughly 75 lines each.
- UI evidence: `artifacts\tool_view_property_base_smoke_20260705_r1\wpf_shell_host_blob_tool.png` and `artifacts\tool_view_property_base_smoke_20260705_r1\wpf_shell_host_contour_tool.png`.
- Verification:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunUiScreenshotSmoke.ps1 -Targets "wpf_shell_host_blob_tool,wpf_shell_host_contour_tool" -OutputDir "artifacts\tool_view_property_base_smoke_20260705_r1"` passed with `layout=0`, `text=0`, and `internal=0` for both targets.
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
  - `git diff --check` passed with CRLF warnings only.

Latest Tool View code-behind cleanup for Matching-family single-input PropertyGrid base:

- Reused `VisionToolSingleInputPropertyToolViewBase` for `MatchingToolWpfView`, `EdgeBasedMatchingToolWpfView`, and `FeatureMatchingToolWpfView`.
- `VisionToolSingleInputMatchingToolController<TProperty>` now implements the same shared controller bridge used by Blob/Contour, so the Matching-family Views no longer duplicate source/destination layer events, preview image command events, selected layer getters, preview setters, status setter, and controller disposal.
- The Matching-family Views still own only tool-specific construction, template/test hooks, property creation, and matching result review.
- Code-behind line counts after cleanup: Matching 58, EdgeBasedMatching 53, FeatureMatching 49.
- Smoke stability note: the screenshot smoke matching template files now use unique temp names instead of a fixed `OpenVisionLab_matching_smoke_template.png`, preventing serial target cleanup/recreate interference during multi-target UI verification.
- UI evidence:
  - Before: `artifacts\matching_tool_base_before_20260706_r1\wpf_shell_host_matching_tool.png`, `artifacts\matching_tool_base_before_20260706_r1\wpf_shell_host_edge_based_matching_tool.png`, `artifacts\matching_tool_base_before_20260706_r1\wpf_shell_host_feature_matching_tool.png`.
  - After: `artifacts\matching_tool_base_after_20260706_r7\wpf_shell_host_matching_tool.png`, `artifacts\matching_tool_base_after_20260706_r7\wpf_shell_host_edge_based_matching_tool.png`, `artifacts\matching_tool_base_after_20260706_r7\wpf_shell_host_feature_matching_tool.png`.
- Verification:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunUiScreenshotSmoke.ps1 -Targets "wpf_shell_host_matching_tool,wpf_shell_host_edge_based_matching_tool,wpf_shell_host_feature_matching_tool,wpf_property_grid_matching_combo" -OutputDir "artifacts\matching_tool_base_after_20260706_r7"` passed with `layout=0`, `text=0`, and `internal=0` for all targets.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_algorithm_output_preview_flow artifacts\matching_tool_base_after_20260706_r3_route` passed with `layout=0`, `text=0`, and `internal=0`.

Latest Tool View code-behind cleanup for Line single-input special PropertyGrid base:

- Reused `VisionToolSingleInputPropertyToolViewBase` for `LineToolWpfView`.
- `VisionToolSingleInputSpecialPropertyToolController` now implements `IVisionToolSingleInputPropertyToolController`, while keeping the Line-specific input-preview callback path for ROI overlay refresh.
- `LineToolWpfView.xaml` now uses `VisionToolSingleInputPropertyToolViewBase` as the XAML root.
- `LineToolWpfView.xaml.cs` now keeps Line-specific purpose/line selection, ROI editing, preset, result review, and test hooks; repeated source/destination layer events, preview image events, selected layer getters, layer list/output/status setters, and controller disposal moved to the shared base.
- Code-behind reduction: `LineToolWpfView.xaml.cs` went from 323 lines to 263 lines.
- UI evidence:
  - Before: `artifacts\line_tool_base_before_20260706_r1\wpf_shell_host_line_tool.png`.
  - After: `artifacts\line_tool_base_after_20260706_r1\wpf_shell_host_line_tool.png`.
- Verification:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_tool artifacts\line_tool_base_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_algorithm_output_preview_flow artifacts\line_tool_base_after_20260706_r1_route` passed with `layout=0`, `text=0`, and `internal=0`.

Latest Tool View shared-base stability recheck:

- Rechecked the current Dev build after Matching-family and Line shared-base cleanup.
- Verification: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunUiScreenshotSmoke.ps1 -Targets "wpf_shell_host_matching_tool,wpf_shell_host_edge_based_matching_tool,wpf_shell_host_feature_matching_tool,wpf_shell_host_line_tool" -OutputDir "artifacts\tool_view_shared_base_recheck_20260706_r1"` passed for all four targets with `layout=0`, `text=0`, and `internal=0`.
- UI evidence:
  - `artifacts\tool_view_shared_base_recheck_20260706_r1\wpf_shell_host_matching_tool.png`
  - `artifacts\tool_view_shared_base_recheck_20260706_r1\wpf_shell_host_edge_based_matching_tool.png`
  - `artifacts\tool_view_shared_base_recheck_20260706_r1\wpf_shell_host_feature_matching_tool.png`
  - `artifacts\tool_view_shared_base_recheck_20260706_r1\wpf_shell_host_line_tool.png`
- Decision: do not continue deleting Tool View code-behind just for line count. Next Tool View work should start only from a visible bug, duplicated owner path, or already-established controller/base pattern.

Latest Tool View code-behind cleanup for double-input custom tool base:

- Changed `ArithmeticToolWpfView` from direct `UserControl` inheritance to `VisionToolDoubleInputCustomToolViewBase`.
- Added `VisionToolDoubleInputCustomToolViewBase` to own double-input event forwarding, preview-image command forwarding, layer preview setters, status setter, and controller disposal.
- `ArithmeticToolWpfView.xaml.cs` now focuses on arithmetic-specific interaction/settings/text behavior. Code-behind reduced from 276 lines to 172 lines; shared base is 164 lines.
- UI evidence: `artifacts\double_input_custom_tool_base_refactor_20260705_r1\wpf_layer_selection_arithmetic_tool.png`
- Verification:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\double_input_custom_tool_base_refactor_20260705_r1` passed with `layout=0`, `text=0`, and `internal=0`.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_algorithm_output_preview_flow artifacts\double_input_custom_tool_base_refactor_20260705_r1_route` passed with `layout=0`, `text=0`, and `internal=0`.
  - `git diff --check -- "0. UI/6) Vision Test/Wpf/ArithmeticToolWpfView.xaml" "0. UI/6) Vision Test/Wpf/ArithmeticToolWpfView.xaml.cs" "0. UI/6) Vision Test/Wpf/VisionToolDoubleInputCustomToolViewBase.cs"` passed with CRLF warnings only.

Latest current-build evidence rule and Arithmetic event-owner cleanup:

- `AGENTS.md` and global `C:\Users\user\.codex\AGENTS.md` now require smoke/capture evidence to use the latest updated EXE or a current-source view generated after the latest relevant source changes. Old artifacts must not be shown as current UI.
- `ArithmeticToolInteractionController` now owns Arithmetic parameter event attach/detach for operation mode, source mode, constant/offset text changes, and numeric input filtering. `ArithmeticToolWpfView.xaml` no longer wires those events to code-behind forwarding methods, and `ArithmeticToolWpfView.xaml.cs` no longer contains those forwarding methods.
- This is a narrow Tool View cleanup only; it does not change Preview/Run behavior, Arithmetic A/B input routing, output layer creation, or docked/floating layout.
- Latest build: `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors on 2026-07-06 21:57 KST.
- Latest direct EXE smoke: `dotnet run --no-build --project OpenVisionLab.csproj -c Debug -- --smoke recipe-manager-tabs artifacts\current_exe_recipe_manager_tabs_20260706_r2_direct` passed. Report includes `Result: PASS`, `LlmCorrectedDraftImport: imported`, `BranchOutputComparison: 2`, `ActualMultiBranchComparison: 7`, and `ActualThreeWayBranchComparison: 5`.
- Latest current-source view capture: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\current_source_arithmetic_tool_20260706_r2` passed with `layout=0`, `text=0`, and `internal=0`.
- Current UI evidence from this turn:
  - `artifacts\current_exe_recipe_manager_tabs_20260706_r2_direct\OpenVisionLab_RecipeManager_LlmXml.png`
  - `artifacts\current_source_arithmetic_tool_20260706_r2\wpf_layer_selection_arithmetic_tool.png`

Latest UI evidence for main window title-bar controls:

- Before/current check: `artifacts\main_window_chrome_before_20260705_r1\wpf_shell_host_window_chrome.png`
- After: `artifacts\main_window_chrome_after_20260705_r1\wpf_shell_host_window_chrome.png`
- Structure note: `OpenVisionWindowTitleBar` keeps minimize, maximize/restore, and close controls. `OpenVisionWindowTitleBar.xaml` now exposes `OpenVisionWindowMinimizeButton`, `OpenVisionWindowMaximizeRestoreButton`, and `OpenVisionWindowCloseButton` automation IDs.
- Verification: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_window_chrome artifacts\main_window_chrome_after_20260705_r1` passed with `layout=0`, `text=0`, and `internal=0`, and the smoke asserts all three window controls are visible.

Latest UI evidence for Recipe Manager density/status cleanup:

- Before: `artifacts\recipe_manager_density_before_20260705_r1\wpf_shell_host_recipe_language_controls.png`
- After: `artifacts\recipe_manager_density_after_20260705_r3\wpf_shell_host_recipe_language_controls.png`
- Direct EXE smoke: `artifacts\recipe_manager_density_after_20260705_r3_direct\report.txt` with `Result: PASS`, `StepPropertyGridApply: explicit XML apply without Preview/Run`, and `BranchOutputComparison: 2`.
- Structure note: `HostRecipeSelectedStepPropertyGridHost` is now visible only after selected Step parameters are explicitly loaded. Changing selected Step clears stale edit status text, so the view no longer shows an old Step 2 XML apply status while Step 1 is selected.
- Verification: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_density_after_20260705_r3` passed with `layout=0`, `text=0`, and `internal=0`.

Latest UI evidence for Recipe Manager footer density:

- Before: `artifacts\recipe_manager_density_before_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- After: `artifacts\recipe_manager_density_after_20260706_r2\wpf_shell_host_recipe_language_controls.png`
- Structure note: `HostRecipeManagerNameStrip` and `HostRecipeManagerCommandStrip` now share one compact footer row. The long recipe name editor remains visible while create/duplicate/rename/delete/XML import/export buttons stay inside the 1600x900 workbench viewport.
- Verification: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_density_after_20260706_r2` passed with `layout=0`, `text=0`, and `internal=0`.

Latest commercial-comparison guided workflow improvement:

- Commercial comparison basis checked during the 2026-07-06 loop:
  - Cognex In-Sight EasyBuilder emphasizes a step workflow such as image setup, location, inspection, and result/output review.
  - MVTec MERLIC recipe documentation emphasizes recipe files as parameter sets for reusable app variants.
  - NI Vision Builder AI documentation emphasizes state/step review.
  - KEYENCE CV-X simulator material emphasizes PC-side configuration/review and generated operating material.
- Dev scope decision: do not add camera/PLC/runtime integration. Add the useful part only: an in-app guided setup strip in Recipe Manager showing sample readiness, XML validation, Step count, sample run, Good/Bad run, and next action.
- Structure note: `HostRecipeGuidedSetupStrip` is now shown inside the Recipe Manager detail header and bound to `RecipeCommands.RecipeGuidedSetupText`.
- UI evidence: `artifacts\recipe_manager_guided_setup_after_20260706_r2\wpf_shell_host_recipe_language_controls.png`.
- Verification:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_guided_setup_after_20260706_r2` passed with `layout=0`, `text=0`, and `internal=0`; the smoke now asserts `HostRecipeGuidedSetupStrip`.

Latest commercial-comparison self-evaluation:

- Official sources were rechecked on 2026-07-06:
  - Cognex EasyBuilder Inspect help and In-Sight Explorer product page: guided inspect/configuration/management workflow.
  - MVTec MERLIC recipe docs: `.mrcp` recipe files, MVApp references, and predefined parameter sets.
  - NI Vision Builder AI pages/readme: configure, benchmark, deploy, camera/image analysis, automation hardware.
  - KEYENCE CV-X product/software pages: camera/lighting/controller ecosystem plus PC simulator/terminal software.
- Product conclusion: OpenVisionLab should not chase camera, lighting, PLC/I/O, controller simulator, deployment runtime, account/session, or production audit features. Its differentiator is local image-based recipe design plus GPT/Gemini/Claude-style LLM XML generation, validation, and explicit OpenCvSharp4 rule-based verification.
- Completion estimate updated in `docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`: about 25-30% versus broad commercial equipment platforms by design, and about 62-66% versus the intended LLM-assisted rule-based recipe workbench.
- Next highest-value development target: real LLM XML failure examples and replayable validation scenarios for bad paths, wrong layers, wrong parameters, and unsafe imports. Do this before adding another generic Recipe Manager panel.

Latest LLM XML bad-route validation scenario:

- Added a direct EXE smoke case that creates a valid `VisionPipeline` XML draft with `InputLayer="Missing_Input_Layer"` and verifies validation blocks it as a route/layer error.
- The smoke asserts the malformed-XML case still reports a line/position fix, and the bad-route case reports the missing input layer without marking draft review/diff as ready.
- Direct EXE evidence: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_bad_route_validation_20260706_r1_direct"` passed with `Result: PASS` and `LlmBadRouteValidation: blocked`.

Latest LLM XML unsupported-tool/import-block scenario:

- Added a direct EXE smoke case that creates a valid `VisionPipeline` XML draft with `ToolType="ImaginaryLlmTool"` and verifies validation blocks it as an unsupported tool.
- The smoke then attempts the explicit import command when available and verifies the selected pipeline does not change and validation context is preserved. This covers the unsafe-import failure path without adding auto Preview/Run.
- Direct EXE evidence: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_unsupported_tool_validation_20260706_r1_direct"` passed with `Result: PASS`, `LlmBadRouteValidation: blocked`, and `LlmUnsupportedToolImport: blocked`.

Latest LLM XML failure corpus expansion:

- Missing dependency paths now block validation/import. `BuildDependencyReport(...)` reports the missing count back into LLM XML validation, so an XML draft with a missing template/image path is not importable even when schema/routing is otherwise valid.
- Added replayable direct EXE smoke cases for:
  - missing template dependency path on a Matching draft;
  - invalid parameter values such as `Threshold=bright` and `USE_ROI=sometimes`;
  - missing Arithmetic `InputLayerB` for a two-input operation.
- Each smoke case validates the draft, attempts explicit import when the command is available, and verifies the selected pipeline does not change.
- Direct EXE evidence: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_failure_corpus_20260706_r2_direct"` passed with `Result: PASS`, `LlmMissingDependencyImport: blocked`, `LlmBadParameterImport: blocked`, and `LlmMissingInputBImport: blocked`.

Latest LLM XML correction-loop scenario:

- The LLM review bundle now includes explicit correction rules: return only OpenVisionLab VisionPipeline XML, use `Main` or previous enabled `OutputLayer`, use supported ToolTypes and PropertyGrid-compatible values, fix missing dependency paths before import, and do not add equipment/Preview/Run instructions.
- The same review bundle now includes selected Step operator context and failed-Step review text, so GPT/Gemini/Claude-style correction requests carry the current Step, route, failure link, and next action without adding another UI surface.
- Added a direct EXE smoke path for bad draft -> correction bundle copy -> corrected XML validation -> explicit import.
- The corrected draft uses `Threshold=128` and `USE_ROI=False`, validates OK, imports as a new selected pipeline, and then the smoke restores the previous pipeline selection so the remaining Recipe Manager checks stay stable.
- Direct EXE evidence: `dotnet run --no-build --project OpenVisionLab.csproj -c Debug -- --smoke recipe-manager-tabs artifacts\llm_step_context_bundle_after_20260706_r1_direct` passed with `Result: PASS`, `LlmCorrectionBundle: copied`, and `LlmCorrectedDraftImport: imported`.
- Screenshot smoke evidence: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\llm_step_context_bundle_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.

Latest LLM XML tool-parameter compatibility guard:

- `VisionPipelineValidator` now treats 0..1 score/weight parameters as validation errors when LLM output uses percentage-style values. Current guarded keys: `SCORE_MIN`, `GREEDINESS`, and `HYBRID_VERIFY_IMAGE_WEIGHT`.
- Matching/feature scale and tolerance parameters now have basic compatibility guards: `MAGNIFIATION`, `RANSAC_REPROJ_THRESHOLD`, and `COARSE_ANGLE_STEP` must be positive; `FIND_ANGLE_MIN` must not exceed `FIND_ANGLE_MAX`.
- Direct EXE smoke now includes `Direct_LLM_BadScoreRange` with `SCORE_MIN=80` and verifies it cannot import.
- Direct EXE evidence: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_parameter_compat_20260706_r1_direct"` passed with `Result: PASS` and `LlmBadScoreRangeImport: blocked`.

Latest LLM prompt/contract alignment:

- The in-app LLM prompt now tells GPT/Gemini/Claude-style assistants to use score/weight parameters as `0..1` decimals, keep angle min/max ordered, use positive matching/feature tolerance values, and avoid unresolved template/image dependency paths.
- `docs\VISION_PIPELINE_LLM_PROMPT_TEMPLATE.md` and `docs\VISION_PIPELINE_LLM_RECIPE_CONTRACT.md` now match the in-app import path: direct OpenVisionLab import expects complete `VisionPipeline` XML only, not extra prose.
- Direct EXE smoke now asserts the copied LLM prompt includes the new score, angle, and dependency-path rules.
- Direct EXE evidence: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_prompt_contract_20260706_r1_direct"` passed with `Result: PASS`.

Latest LLM XML result-channel contract:

- The in-app LLM prompt, review bundle, validation report, and Recipe Manager Report tab now define the operator result channels: `Inspection.Status`, `Inspection.FailedStep`, `Inspection.Evidence`, `Inspection.Benchmark`, and `Inspection.NextAction`.
- These are logical outputs derived from XML validation and explicit sample runs. LLM drafts must not emit custom `Inspection.*` XML nodes or parameters.
- Contract docs were updated in `docs\VISION_PIPELINE_LLM_RECIPE_CONTRACT.md`, `docs\VISION_PIPELINE_LLM_PROMPT_TEMPLATE.md`, and `docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`.
- Direct EXE smoke evidence from the implementation step: `artifacts\llm_result_channel_requirements_after_20260706_r2\report.txt` passed with prompt/review/validation checks for `Inspection.Status` and `Inspection.Evidence`.

Latest UI evidence for Recipe Manager result-channel board:

- Before: `artifacts\result_channel_board_before_20260706_r3_direct\OpenVisionLab_RecipeManager_Report.png`
- After: `artifacts\result_channel_board_after_20260706_r3_direct\OpenVisionLab_RecipeManager_Report.png`
- Structure note: `HostRecipeOperatorResultChannelBoard` now shows compact cards for `Inspection.Status`, `Inspection.FailedStep`, `Inspection.Evidence`, `Inspection.Benchmark`, and `Inspection.NextAction` above the detailed result-channel list in the Recipe Manager Report tab.
- Direct EXE smoke: `artifacts\result_channel_board_after_20260706_r3_direct\report.txt` passed with `Result: PASS`; the smoke asserts the board automation id and the Status/Evidence rows.

Latest LLM XML `Inspection.*` misuse block:

- `Inspection.*` names are now treated as logical review channels only. If an LLM draft emits `Inspection.Status` or another `Inspection.*` name inside XML, validation is NG and import keeps the previous pipeline selection.
- Direct EXE smoke: `artifacts\llm_custom_inspection_block_after_20260706_r4_direct\report.txt` passed with `LlmCustomInspectionImport: blocked`.
- Screenshot smoke: `artifacts\llm_custom_inspection_block_after_20260706_r2\wpf_shell_host_recipe_language_controls.png` passed with `layout=0`, `text=0`, and `internal=0`.

Latest Dev verification checkpoint at 2026-07-06 09:04 KST:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors after the LLM prompt/contract alignment.
- `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_prompt_contract_20260706_r1_direct"` passed with `Result: PASS`, `LlmBadScoreRangeImport: blocked`, `LlmCorrectionBundle: copied`, and `LlmCorrectedDraftImport: imported`.
- `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
- `git diff --check` passed with CRLF warnings only.
- Original repo was not touched.

Latest UI evidence for Recipe Manager guided next action:

- Before/current baseline: `artifacts\llm_clipboard_paste_after_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\guided_next_action_after_20260706_r2_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- Structure note: The Recipe Manager guided setup strip now exposes `HostRecipeGuidedNextActionButton`. It routes one explicit user click to the current next existing action, such as Validate XML, Duplicate from sample, Activate pipeline, Run check, Run Good/Bad, load selected Step parameters, or open the selected Step tool. It does not add automatic Preview/Run.
- Direct EXE smoke: `artifacts\guided_next_action_after_20260706_r2_direct\report.txt` with `Result: PASS`, `FailedStepRerunComparison: visible`, `StepPropertyGridApply: explicit XML apply without Preview/Run`, and the smoke asserts the guided next action command is enabled during failed-Step review.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\guided_next_action_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it asserts `HostRecipeGuidedNextActionButton`.

Latest UI evidence for Recipe Manager guided next action label:

- Before: `artifacts\guided_next_action_after_20260706_r2_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\guided_next_label_after_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- Structure note: `RecipeGuidedNextActionText` now shows the concrete next action instead of a generic "Run next" label. Failed-Step review shows `도구 열기`/`Open tool`; other states can show Validate XML, Duplicate sample, Activate, Run check, Load params, Run Good/Bad, or Complete.
- Direct EXE smoke: `artifacts\guided_next_label_after_20260706_r1_direct\report.txt` with `Result: PASS`, `FailedStepRerunComparison: visible`, `StepPropertyGridApply: explicit XML apply without Preview/Run`, and the smoke asserts the failed-Step guided action label includes tool/도구.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\guided_next_label_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.

Latest UI evidence for Run History selected-review copy action:

- Before: `artifacts\run_history_copy_before_20260706_r1_direct\OpenVisionLab_RecipeManager_RunHistory.png`
- After: `artifacts\run_history_copy_after_20260706_r1_direct\OpenVisionLab_RecipeManager_RunHistory.png`
- Structure note: The Run History tab now exposes `HostRecipeCopySelectedRunReviewButton`. It copies the selected run review text to the clipboard and shows inline status. It does not rerun checks, change layers, import XML, or run Preview.
- Direct EXE smoke: `artifacts\run_history_copy_after_20260706_r2_direct\report.txt` with `Result: PASS`, `SelectedRunReview: linked failed step`, `SelectedRunReviewCopy: copied`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`; the smoke executes the selected run review copy command and checks the success status. The command is enabled only when a saved run with `SummaryPath` is selected.
- Clipboard payload smoke: `artifacts\clipboard_payload_smoke_20260706_r1_direct\report.txt` passed after checking copied clipboard text for operator handoff report, selected run review, LLM prompt, and LLM review bundle.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\run_history_copy_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it also executes the selected run review copy command.
- Current-build Recipe Manager recheck: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_current_recheck_20260706_r2` passed with `layout=0`, `text=0`, and `internal=0`. The screenshot smoke now treats selected-run review copy as enabled only when a saved run with `SummaryPath` is selected.
- Direct EXE recheck: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\recipe_manager_current_recheck_20260706_r2_direct"` passed with `Result: PASS` and `SelectedRunReviewCopy: copied`, so the saved-run copy path remains covered.
- Follow-up screenshot recheck after smoke cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_current_recheck_20260706_r3` passed with `layout=0`, `text=0`, and `internal=0`. Visual inspection of `artifacts\recipe_manager_current_recheck_20260706_r3\wpf_shell_host_recipe_language_controls.png` did not show a new control clipping/overlap issue that justifies another UI change in this loop.

Latest UI evidence for Recipe Manager operator decision board:

- Before: `artifacts\recipe_manager_guided_setup_after_20260706_r3_direct\OpenVisionLab_RecipeManager_RoleDrilldown.png`
- After: `artifacts\operator_review_board_after_20260706_r1_direct\OpenVisionLab_RecipeManager_RoleDrilldown.png`
- Structure note: Review tab now shows `HostRecipeOperatorDecisionBoard` with XML/Step, selected sample, Good/Bad, and next-action cards above the existing long operator review text. It reuses existing sample/pair/pipeline state and does not add Preview/Run triggers.
- Direct EXE smoke: `artifacts\operator_review_board_after_20260706_r1_direct\report.txt` with `Result: PASS`, `PairRoleCards: 2`, `FailedStepRerunComparison: visible`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\operator_review_board_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.

Latest UI evidence for Recipe Manager operator handoff report:

- Before: `artifacts\operator_review_board_after_20260706_r1_direct\OpenVisionLab_RecipeManager_RoleDrilldown.png`
- After: `artifacts\operator_report_tab_after_20260706_r1_direct\OpenVisionLab_RecipeManager_Report.png`
- Structure note: Pipeline review now has a `Report` tab (`HostRecipePipelineReportTab`) with `HostRecipeOperatorHandoffReport`. The report summarizes recipe, pipeline, active pipeline, XML/Step status, selected sample result, Good/Bad result, next action, selected role, review Step, route, and first LLM XML validation line.
- Direct EXE smoke: `artifacts\operator_report_tab_after_20260706_r1_direct\report.txt` with `Result: PASS`, `PairRoleCards: 2`, `FailedStepRerunComparison: visible`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\operator_report_tab_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.

Latest UI evidence for Recipe Manager operator report copy action:

- Before: `artifacts\operator_report_copy_before_20260706_r1_direct\OpenVisionLab_RecipeManager_Report.png`
- After: `artifacts\operator_report_copy_after_20260706_r1_direct\OpenVisionLab_RecipeManager_Report.png`
- Structure note: The Pipeline review `Report` tab now exposes `HostRecipeCopyOperatorHandoffReportButton`. It copies the generated operator report to the clipboard and shows an inline success/failure status without running Preview or changing layers.
- Direct EXE smoke: `artifacts\operator_report_copy_after_20260706_r1_direct\report.txt` with `Result: PASS`, `PairRoleCards: 2`, `FailedStepRerunComparison: visible`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`; the smoke now executes the copy command and checks the success status.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\operator_report_copy_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it also executes the copy command.

Latest UI evidence for LLM prompt copy action:

- Before: `artifacts\operator_report_copy_before_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_prompt_copy_after_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- Structure note: The LLM XML tab now exposes `HostRecipeCopyLlmPromptButton`. It copies the generated prompt to the clipboard and shows an inline success/failure status. It does not validate/import XML and does not run Preview.
- Direct EXE smoke: `artifacts\llm_prompt_copy_after_20260706_r1_direct\report.txt` with `Result: PASS`, `LlmValidationIssues: visible`, `LlmDependencyRows: 1`, and `LlmXmlDiff: visible`; the smoke now builds the prompt, executes the copy command, and checks the success status.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\llm_prompt_copy_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it also executes the copy command.

Latest UI evidence for LLM review bundle copy action:

- Before: `artifacts\llm_review_bundle_before_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_review_bundle_after_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- Structure note: The LLM XML tab now exposes `HostRecipeCopyLlmReviewBundleButton`. It copies a correction bundle containing recipe/pipeline context, validation report, dependency report, draft import review, diff review, and current XML draft. It does not validate/import XML and does not run Preview.
- Direct EXE smoke: `artifacts\llm_review_bundle_after_20260706_r1_direct\report.txt` with `Result: PASS`, `LlmValidationIssues: visible`, `LlmDependencyRows: 1`, and `LlmXmlDiff: visible`; the smoke now executes the review bundle copy command and checks the success status.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\llm_review_bundle_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it also executes the review bundle copy command.

Latest UI evidence for LLM XML clipboard paste action:

- Before: `artifacts\llm_clipboard_paste_before_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_clipboard_paste_after_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- Structure note: The LLM XML tab now exposes `HostRecipePasteLlmXmlDraftButton`. It pastes clipboard XML text into the draft editor and shows an inline status. It does not validate, import, run Preview, or change layers; the operator still must press Validate and Import explicitly.
- Direct EXE smoke: `artifacts\llm_clipboard_paste_after_20260706_r1_direct\report.txt` with `Result: PASS`, `LlmValidationIssues: visible`, `LlmDependencyRows: 1`, and `LlmXmlDiff: visible`; the smoke now sets clipboard XML, executes the paste command, and checks the pasted draft/status.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\llm_clipboard_paste_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it also executes the paste command.

Latest UI evidence for top account/operator chrome removal:

- Before: `artifacts\account_header_before_20260705_r1\wpf_shell_host_layer_management_commands.png`
- After: `artifacts\account_header_after_20260705_r1\wpf_shell_host_layer_management_commands.png`
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_layer_management_commands artifacts\account_header_after_20260705_r1` passed.
- Structure evidence: `rg -n 'OperatorText|Shell\.Operator|Kind="Account"' -g '*.xaml' -g '*.cs' "0. UI/0) MENU/Wpf"` returns no matches.
- Product decision: account/session UI is not part of the current OpenVisionLab workbench scope.

Latest UI evidence for failed Step rerun/comparison action strip:

- True before note: this action strip was implemented before a fresh before capture was taken. Closest baseline is the immediately prior Recipe Manager role drill-down capture: `artifacts\llm_xml_diff_after_20260705_r1\OpenVisionLab_RecipeManager_RoleDrilldown.png`.
- After: `artifacts\failure_rerun_comparison_after_20260705_r1\OpenVisionLab_RecipeManager_RoleDrilldown.png`
- WPF screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\failure_rerun_comparison_after_20260705_r2_screenshot_smoke` passed.
- Direct EXE smoke: `artifacts\failure_rerun_comparison_after_20260705_r1\report.txt` with `Result: PASS`, `FailedStepRerunComparison: visible`, `RoleDrilldown: Bad -> 01 Battery Cell Vent Alignment Distance`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.
- Structure note: Review tab now shows `HostRecipeFailureRerunComparisonPanel` after a failed Step is selected, with direct output/input layer navigation, Step parameter review, and Good/Bad rerun actions. It reuses existing explicit commands and does not introduce auto Preview/Run.

Latest UI evidence for top layer command icon stabilization:

- Before: `artifacts\top_layer_icon_before_20260705_r1\wpf_shell_host_layer_management_commands.png`
- After: `artifacts\top_layer_icon_after_20260705_r2\wpf_shell_host_layer_management_commands.png`
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_layer_management_commands artifacts\top_layer_icon_after_20260705_r2` passed.
- Structure note: top layer create/load/delete icon buttons now share a fixed 28x26 centered style, and the smoke asserts visible button size/order so the white icons do not drift, clip, or disappear under header pressure.

Latest UI evidence for LLM XML diff review and dependency path action hints:

- Before: `artifacts\llm_xml_diff_before_20260705_r1\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_xml_diff_after_20260705_r1\OpenVisionLab_RecipeManager_LlmXml.png`
- WPF screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\llm_xml_diff_after_20260705_r1_screenshot_smoke` passed.
- Direct EXE smoke: `artifacts\llm_xml_diff_after_20260705_r1\report.txt` with `Result: PASS`, `LlmXmlDiff: visible`, `LlmValidationIssues: visible`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.
- Structure note: LLM XML tab now separates draft validation, dependency scan/copy report, draft import review, LLM XML diff review, and validation issue rows. The diff compares the active pipeline with the draft before import and reports step count, dependency count, added/removed/changed steps, and parameter changes without running Preview.

Latest UI evidence for selected Step PropertyGrid parameter review and explicit XML apply-back:

- Before: `artifacts\recipe_step_parameter_apply_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\recipe_step_parameter_apply_after_20260705_r6\OpenVisionLab_RecipeManager_StepPropertyGrid.png`
- Direct EXE smoke: `artifacts\recipe_step_parameter_apply_after_20260705_r6\report.txt` with `Result: PASS`, `StepPropertyGridApply: explicit XML apply without Preview/Run`, and `StepToolEntry: 도구 열기: LineDistance`.
- Structure note: Recipe Manager owns the embedded Step PropertyGrid review/apply path. Opening the native tool seeds repository-backed tool sessions for inspection, but XML apply-back is still an explicit Recipe Manager action.

Latest UI evidence for Recipe Manager workbench layout:

- Before: `artifacts\recipe_workbench_layout_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\recipe_workbench_layout_after_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After with Step PropertyGrid loaded: `artifacts\recipe_workbench_layout_after_20260705_r1\OpenVisionLab_RecipeManager_StepPropertyGrid.png`
- Direct EXE smoke: `artifacts\recipe_workbench_layout_after_20260705_r1\report.txt` with `Result: PASS`, `StepPropertyGridApply: explicit XML apply without Preview/Run`, and `MovedTo: -64.0,18.0`.

Latest UI evidence for Good/Bad role failed-Step drill-down:

- Before: `artifacts\sample_role_drilldown_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\sample_role_drilldown_after_20260705_r3\OpenVisionLab_RecipeManager_RoleDrilldown.png`
- Direct EXE smoke: `artifacts\sample_role_drilldown_after_20260705_r3\report.txt` with `Result: PASS`, `RoleDrilldown: Bad -> 01 Battery Cell Vent Alignment Distance`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.

Latest UI evidence for multi-step pipeline flow focus:

- Baseline note: the first current-build before capture used the wrong recipe-context screenshot target, so it is only a closest reproducible baseline for the shell state, not a true Recipe Manager before view.
- Closest baseline: `artifacts\multi_step_flow_before_20260705_r1\wpf_shell_host_recipe_context_switch.png`
- After full-window Recipe Manager capture: `artifacts\multi_step_flow_after_20260705_r3_recipe_manager\wpf_shell_host_recipe_language_controls.png`
- After Recipe Manager panel crop: `artifacts\multi_step_flow_after_20260705_r3_recipe_manager\wpf_shell_host_recipe_language_controls.diagnostics\recipe-manager-panel.png`
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\multi_step_flow_after_20260705_r3_recipe_manager` passed.
- Structure note: Recipe Manager now exposes current selected Step flow in the header (`HostRecipePipelineHeaderStepFlow`), adds an XML/Step flow focus strip with Previous/Next commands, and verifies next-Step navigation does not trigger Preview/Run.

Latest UI evidence for structured LLM XML validation rows:

- Before: `artifacts\llm_xml_validation_rows_before_20260705_r1\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_xml_validation_rows_after_20260705_r5\OpenVisionLab_RecipeManager_LlmXml.png`
- Direct EXE smoke: `artifacts\llm_xml_validation_rows_after_20260705_r5\report.txt` with `Result: PASS` and `LlmValidationIssues: visible`.

Latest UI evidence for selected Step input/output layer cards:

- Before: `artifacts\step_layer_navigation_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\step_layer_navigation_after_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- Direct EXE smoke: `artifacts\step_layer_navigation_after_20260705_r1\report.txt` with `Result: PASS` and `StepLayerCards: visible`.

Latest UI evidence for selected Step thumbnail cards and click navigation:

- Before: `artifacts\step_layer_click_nav_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\step_layer_click_nav_after_20260705_r4\OpenVisionLab_RecipeManager_Pipeline.png`
- Direct EXE smoke: `artifacts\step_layer_click_nav_after_20260705_r4\report.txt` with `Result: PASS` and `StepLayerNavigation: Battery_CellVentAlignment_Preview -> Main`.

Latest UI evidence for Good/Bad role result cards:

- Before: `artifacts\sample_review_drilldown_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\sample_review_drilldown_after_20260705_r2\OpenVisionLab_RecipeManager_Pipeline.png`
- Direct EXE smoke: `artifacts\sample_review_drilldown_after_20260705_r2\report.txt` with `Result: PASS` and `PairRoleCards: 2`.

Latest UI evidence for selected Step ROI/template metadata and tool entry:

- Before: `artifacts\recipe_step_roi_template_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\recipe_step_roi_template_after_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- Direct EXE smoke: `artifacts\recipe_step_roi_template_after_20260705_r1\report.txt` with `Result: PASS`, `StepRoiTemplate: ROI: 172,166,116,136 | Template: 없음`, and `StepToolEntry: 도구 열기: LineDistance`.

## Latest Original Repo Commits

Key latest stable code/workflow commits in `C:\Git\OpenVisionLab`:

- `9c2bbe1 Show pipeline review parameter focus hints`
- `c90d60a Record pipeline review parameter location hints`
- `2371b37 Add pipeline review parameter location hints`
- `bc42e0e Record pipeline review label polish`
- `71ecc21 Localize pipeline review guide labels`
- `b8a95cf Record catalog audit after metric cleanup`

## Completed On 2026-07-03

- Product sample catalog/native runner gate is stable.
  - Dev evidence: `artifacts\self_evaluation_product_catalog_20260703_1750\sample_catalog_summary.json`
  - Original full evidence: `artifacts\original_product_catalog_full_20260703_1919\sample_catalog_summary.json`
  - Original final evidence: `artifacts\product_catalog_final_20260703_1920\sample_catalog_summary.json`
  - Original after Line cleanup evidence: `artifacts\product_catalog_after_line_controller_cleanup_20260703_1935\sample_catalog_summary.json`
  - Original full result: `GateStatus=OK`, `RunnableRows=168`, `RequiredRows=84`, `ExpectedFailureRows=84`, `OKRows=168`, `NGRows=0`
  - Original final result: `GateStatus=OK`, `RunnableRows=168`, `RequiredRows=84`, `ExpectedFailureRows=84`, `OKRows=168`, `NGRows=0`, `DurationSeconds=81.234`
  - Original after Line cleanup result: `GateStatus=OK`, `RunnableRows=168`, `RequiredRows=84`, `ExpectedFailureRows=84`, `OKRows=168`, `NGRows=0`, `DurationSeconds=70.815`
  - Quality audit: `ProductSampleQualityAudit=PASS | PairRecords=84 OK=84 Review=0 Critical=0`
  - Latest quality audit after Line cleanup: `ProductSampleQualityAudit=PASS | PairRecords=84 OK=84 Review=0 Critical=0`
- Original repo reviewed import, Product Field Explore samples and picker affordance:
  - Imported the reviewed Dev Field Explore sample bundle into `C:\Git\OpenVisionLab` without bulk-copying the repo.
  - Original data/assets added:
    - 16 PNGs under `C:\Git\OpenVisionLab\docs\samples\public\product\field\`
    - `C:\Git\OpenVisionLab\docs\samples\public\product\Product_Field_DarkFeature_Contour.pipeline.xml`
    - `C:\Git\OpenVisionLab\docs\samples\public\product\Product_Field_BrightFeature_Contour.pipeline.xml`
    - `C:\Git\OpenVisionLab\docs\samples\public\product\Product_Field_SurfaceMean.pipeline.xml`
    - 16 `Product_Field_*` Explore rows in `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
    - 16 field provenance rows in `docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv`
  - Original UI/runtime imported:
    - `OpenVisionWorkspaceSampleFocusOption` now exposes a `field` focus only for `ValidationMode=Explore` field-style samples.
    - `OpenVisionWorkspaceSamplePickerViewModel` shows `Explore sample`, reference metric copy, and `ExploratoryGuideText` for Explore rows.
    - `OpenVisionWorkspaceSamplePickerView.xaml` displays `WorkspaceSamplePickerExploreGuide`.
    - `VisionPipelineSampleCatalog` Product source copy now describes Good/Bad plus Field Explore samples.
    - `PipelineViewerScreenshotSmoke` has `wpf_shell_host_workspace_sample_product_field_focus_picker`.
  - Before/after Original UI evidence:
    - Before Field affordance: `C:\Git\OpenVisionLab\artifacts\product_field_explore_original_before_ui_20260703_01\wpf_shell_host_workspace_sample_product_focus_picker.png`
    - After Field affordance: `C:\Git\OpenVisionLab\artifacts\product_field_explore_original_after_20260703_02\wpf_shell_host_workspace_sample_product_field_focus_picker.png`
    - Existing Product focus after: `C:\Git\OpenVisionLab\artifacts\product_field_explore_original_after_20260703_02\wpf_shell_host_workspace_sample_product_focus_picker.png`
  - Original verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_field_focus_picker artifacts\product_field_explore_original_after_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_picker artifacts\product_field_explore_original_after_20260703_02` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed: `PublicSampleAssetCheck=PASS | CatalogRows=184 ManifestAssets=214 Pipelines=87`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_184_original_after_field_import_20260703_01 -SkipRunnerBuild` passed: `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=76.231`.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
    - `rg -n "ChatGPT|C:\\Git\\새 폴더|새 폴더|DALL|OpenAI" docs\samples\public\product docs\samples\OpenVisionLab.ProductSampleCatalog.csv tools\GenerateOpenVisionProductSamples.ps1` returned no matches.
  - Source-target evidence:
    - Field PNG hashes match Dev: 16 files, 0 mismatches.
    - Field pipeline hashes match Dev: 3 files, 0 mismatches.
    - Dev/Original text equality confirmed for Field focus/view/viewmodel/catalog CSV/manifest/README/generator and `VisionPipelineSampleCatalog`.
    - Known deviation: `tools\PipelineViewerScreenshotSmoke\Program.cs` differs from Dev by one non-Field line, `resultCountMetricText`, which was left outside this import scope.
- Dev Tool View code-behind cleanup, text presenter extraction:
  - Added small text presenters so View code-behind no longer owns static localization assignments for Threshold, Filter, and Morphology:
    - `C:\Git\OpenVisionLab_Dev\0. UI\6) Vision Test\Wpf\ThresholdToolTextPresenter.cs`
    - `C:\Git\OpenVisionLab_Dev\0. UI\6) Vision Test\Wpf\FilterToolTextPresenter.cs`
    - `C:\Git\OpenVisionLab_Dev\0. UI\6) Vision Test\Wpf\MorphologyToolTextPresenter.cs`
  - Updated Views:
    - `ThresholdToolWpfView.xaml.cs` now delegates Threshold parameter/mode labels to `ThresholdToolTextPresenter`.
    - `FilterToolWpfView.xaml.cs` now delegates operation/kernel labels to `FilterToolTextPresenter`.
    - `MorphologyToolWpfView.xaml.cs` now delegates operation/kernel/shape labels to `MorphologyToolTextPresenter`, while keeping operation/shape button state in `VisionToolMorphologyInteractionController`.
  - Structure evidence:
    - `rg -n "OpenVisionLanguageService\.T\("` over the three View code-behind files returns no matches; localization calls remain in the presenter classes.
    - Responsibility moved from View code-behind to text presenter layer; existing tool runtime/controller event paths were not changed.
  - Dev verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\FilterToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\FilterToolTextPresenter.cs" "0. UI\6) Vision Test\Wpf\MorphologyToolTextPresenter.cs" "0. UI\6) Vision Test\Wpf\ThresholdToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\ThresholdToolTextPresenter.cs"` passed with CRLF warnings only.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard artifacts\tool_text_presenters_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_basic_tool artifacts\tool_text_presenters_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_tool artifacts\tool_text_presenters_dev_20260703_01` passed.
  - Dev screenshot evidence:
    - `C:\Git\OpenVisionLab_Dev\artifacts\tool_text_presenters_dev_20260703_01\wpf_filter_morphology_layout_guard.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\tool_text_presenters_dev_20260703_01\wpf_shell_host_threshold_basic_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\tool_text_presenters_dev_20260703_01\wpf_shell_host_threshold_tool.png`
- Dev Tool View code-behind cleanup, Line text presenter and Filter/Morphology event attach:
  - Added `C:\Git\OpenVisionLab_Dev\0. UI\6) Vision Test\Wpf\LineToolTextPresenter.cs`.
  - `LineToolWpfView.xaml.cs` now delegates Line purpose labels, ROI tooltip, purpose hint, and summary text composition to `LineToolTextPresenter`.
  - `VisionToolKernelSizeController` now attaches/detaches kernel text, lock, and preset button events directly.
  - `VisionToolFilterInteractionController` now attaches/detaches Filter type and border type selection events directly.
  - `VisionToolMorphologyInteractionController` now attaches/detaches operation button and shape radio events directly.
  - Removed direct XAML event handler attributes from `FilterToolWpfView.xaml` and `MorphologyToolWpfView.xaml` for those controller-owned paths.
  - Structure evidence:
    - `LineToolWpfView.xaml.cs` no longer contains `VisionToolVerificationText`, `VisionToolChromePresenter.ApplyTooltip`, or direct `presenter.CreateSummary(...)` usage.
    - `FilterToolWpfView.xaml` and `MorphologyToolWpfView.xaml` no longer contain controller-owned `SelectionChanged`, `TextChanged`, `Checked`, `Unchecked`, or `Click` handler attributes, except normal `IsChecked` state.
  - Dev verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\LineToolTextPresenter.cs"` passed with CRLF warnings only.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\Behaviors\VisionToolKernelSizeController.cs" "0. UI\6) Vision Test\Wpf\Behaviors\VisionToolFilterInteractionController.cs" "0. UI\6) Vision Test\Wpf\Behaviors\VisionToolMorphologyInteractionController.cs" "0. UI\6) Vision Test\Wpf\FilterToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\FilterToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml.cs"` passed with CRLF warnings only.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_tool artifacts\line_text_presenter_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_text_presenter_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_text_presenter_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard artifacts\filter_morph_controller_event_attach_dev_20260703_01` passed.
  - Dev screenshot evidence:
    - `C:\Git\OpenVisionLab_Dev\artifacts\line_text_presenter_dev_20260703_01\wpf_shell_host_line_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\line_text_presenter_dev_20260703_01\wpf_shell_host_line_measure_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\line_text_presenter_dev_20260703_01\wpf_shell_host_line_intersection_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\filter_morph_controller_event_attach_dev_20260703_01\wpf_filter_morphology_layout_guard.png`
- Dev Pipeline Review result presenter extraction:
  - Added `C:\Git\OpenVisionLab_Dev\0. UI\0) MENU\Wpf\OpenVisionPipelineReviewResultPresenter.cs`.
  - `OpenVisionPipelineReviewDocument` now delegates selected-step run log, result summary/detail, Good/Bad pair action text, and pair metric comparison text to the presenter.
  - The document keeps pipeline execution, layer image cache, validation, sample-pair resolution, and View update orchestration.
  - `ResultCount` display now keeps the public-smoke `Result` token while retaining localized operator text, for example `Result (결과 수)` in Korean.
  - Structure evidence:
    - `rg -n "private .*FormatRunLog|private .*FormatResultSummary|private .*FormatResultDetails|private .*ResolvePairMetricComparisonText|private .*ResolvePairActionText|FormatPrimaryMetricText|FormatMetricName|LocalText\(" "0. UI\0) MENU\Wpf\Documents\OpenVisionPipelineReviewDocument.cs"` returns no matches.
    - `OpenVisionPipelineReviewDocument.cs` calls `OpenVisionPipelineReviewResultPresenter.FormatRunLog`, `FormatResultSummary`, `FormatResultDetails`, `ResolvePairActionText`, and `ResolvePairMetricComparisonText`.
  - Dev verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_metrics artifacts\pipeline_review_result_presenter_dev_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_result_presenter_dev_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\pipeline_review_result_presenter_dev_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\pipeline_review_result_presenter_dev_20260703_02` passed.
  - Dev screenshot evidence:
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_result_presenter_dev_20260703_02\wpf_shell_host_workspace_sample_pipeline_review_metrics.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_result_presenter_dev_20260703_02\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_result_presenter_dev_20260703_02\wpf_shell_host_workspace_product_sample_review.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_result_presenter_dev_20260703_02\wpf_shell_host_workspace_product_sample_review_ng.png`
- Dev Pipeline Review operator NG triage UX:
  - Added NG-only operator triage fields to `OpenVisionPipelineReviewGuideState` and `OpenVisionPipelineReviewViewModel`.
  - `OpenVisionPipelineReviewGuidePresenter` now supplies separate cause, adjustment, and rerun texts for NG/acceptance-NG steps.
  - `OpenVisionPipelineReviewView.xaml` shows the triage strip inside the guide detail area only when NG triage text exists.
  - `OpenVisionShellHostStatePresenter`, `OpenVisionShellHostToolTestFacade`, and `OpenVisionShellHostView.TestHooks.cs` expose the new triage texts for smoke evidence.
  - Added localization keys:
    - `PipelineReview.Guide.TriageFailure`
    - `PipelineReview.Guide.TriageAdjustment`
    - `PipelineReview.Guide.TriageRerun`
    - `PipelineReview.Guide.TriageRerunPair`
  - Reduced the lower detail row height in Pipeline Review from `250` to `210` so the input/output preview area remains visible after the triage strip is shown.
  - Before/after Dev UI evidence:
    - Before: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_operator_review_ux_before_dev_20260703_01\wpf_shell_host_workspace_product_sample_review_ng.png`
    - After: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_operator_review_ux_after_dev_20260703_02\wpf_shell_host_workspace_product_sample_review_ng.png`
  - Dev verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\pipeline_operator_review_ux_after_dev_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\pipeline_operator_review_ux_after_dev_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_operator_review_ux_after_dev_20260703_02` passed.
    - `git diff --check --` over the touched Pipeline Review/localization/smoke files passed with CRLF warnings only.
  - Structure evidence:
    - `rg -n "PipelineReviewGuideTriage(Failure|Adjustment|Rerun)|ReviewGuideTriage(Failure|Adjustment|Rerun)|HasReviewGuideTriage|TriageRerunPair" "0. UI\0) MENU\Wpf" "Library\OpenVisionLab.Localization\Resources\LocalizationCatalog.tsv" "tools\PipelineViewerScreenshotSmoke\Program.cs"` shows the ViewModel, View, document/test hook, localization, and smoke assertion path.
- Dev MainView/Product sample counterpart affordance:
  - Current-flow evaluation screenshots were refreshed before changing the MainView workflow strip:
    - Picker before/evaluation: `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_eval_dev_20260703_01\wpf_shell_host_workspace_sample_product_focus_picker.png`
    - Open before/evaluation: `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_eval_dev_20260703_01\wpf_shell_host_workspace_sample_product_focus_open.png`
  - Added a direct counterpart sample button to the bottom sample workflow strip:
    - Good samples show `NG 기준 열기`.
    - Bad/NG samples show `OK 기준 열기`.
  - `OpenVisionShellHostSampleWorkflowPresenter` now resolves the opposite Good/Bad sample in the same PairGroup and exposes `CounterpartSampleName`.
  - `OpenVisionShellHostWorkspaceCommandSurface` adds `OpenSampleCounterpartCommand`, reusing `OpenRunnableSampleByName` so the action swaps the sample image/pipeline only and does not run Preview or open a tool.
  - `OpenVisionShellHostView.TestHooks.cs` exposes `CanOpenSampleCounterpartForTest` and `OpenSampleCounterpartForTest`.
  - Added smoke target `wpf_shell_host_workspace_sample_product_counterpart_open`.
  - After Dev UI evidence:
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_after_dev_20260703_01\wpf_shell_host_workspace_sample_product_focus_open.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_after_dev_20260703_01\wpf_shell_host_workspace_sample_product_counterpart_open.png`
  - Dev verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_open artifacts\mainview_product_flow_after_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_counterpart_open artifacts\mainview_product_flow_after_dev_20260703_01` passed.
    - `git diff --check --` over the touched MainView workflow/smoke files passed with CRLF warnings only.
  - Stable-contract evidence:
    - The counterpart command smoke asserts no active WPF tool, no active native document, no native Preview result, and unchanged `NativePreviewRunCount` after switching samples.
- Self-evaluation document was added.
  - File: `docs\OPENVISIONLAB_SELF_EVALUATION_20260703.md`
  - Conclusion: target-product maturity `4.0/5`; industrial integrated-platform maturity `2.0/5`.
  - Keep the product advantage focused on PropertyGrid tools, transparent layer routes, Preview/Pipeline separation, and sample-backed review.
- MainView/Product sample workflow was improved.
  - After opening a sample, the bottom workflow strip exposes the hint `Pipeline Review에서 NG/OK 기준 열기`.
  - Product group labels are shortened to `Secondary Battery`, `Display`, and `Semiconductor` so the review hint stays visible.
  - Original commit: `b011ee2`
- Pipeline Review operator guide was improved.
  - Final OK no longer implies a misleading "next step"; it points to output/support-layer review and Good/Bad pair comparison.
  - NG review now shows tool-type-specific `우선 확인:` guidance.
  - Original commits: `95ed902`, `e98a0b2`
- Product sample NG review smoke was fixed.
  - `wpf_shell_host_workspace_product_sample_review_ng` now accepts Product catalog samples instead of asserting Public source kind.
  - Original commit: `b0da050`
- Contour teaching preview stale review was fixed.
  - `ContourToolWpfView.RequestThresholdTeachingPreview()` now clears stale result review before teaching preview, matching Blob behavior.
  - Original commit: `bab969e`
- Korean duplicate-key detection was fixed.
  - `WpfPropertyGridAdapter` now detects `같은 키` in duplicate-key messages instead of a mojibake string.
  - Original commit: `01c7aa4`
- Public/product sample review smoke coverage was consolidated.
  - New script: `tools\RunSampleReviewUiSmokes.ps1`
  - The script runs single WPF targets sequentially to avoid the previously observed multi-target suite hang.
  - Required pair coverage now uses public/product representative groups instead of legacy root-only sample groups.
  - Bad-reference audit now requires controlled NG samples and treats legacy comparative bad references as optional/private.
  - Original commit: `6ca54d3`
- Tool View code-behind cleanup continued.
  - `VisionToolKernelSizeController` now owns shared kernel preset click parsing for Filter and Morphology.
  - Filter/Morphology views now use the same Tag-based preset click path instead of separate 3/5/7 handlers.
  - Original commit: `567fefc`
- Filter/Morphology layout smoke was restored.
  - `SelectComboBoxItemText` now accepts the already-selected value as a valid selection.
  - The layout guard now clicks Filter and Morphology kernel preset buttons, so the shared preset handler is covered by smoke.
  - Original commit: `0a2e026`
- Pipeline Review top-card next-action copy was shortened.
  - The long final OK/NG next-action strings now fit the top summary card while the detailed guide still carries the longer explanation.
  - Existing runtime `CONFIG\localization_catalog.tsv` files migrate from the previous default strings to the shorter defaults.
  - Original commit: `5f76663`
- Filter/Morphology code-behind was trimmed.
  - Unused imaging, IO, and OpenCvSharp morphology usings were removed after the shared preset controller extraction.
  - Original commit: `4278e43`
- Self-evaluation evidence was refreshed after the final catalog and UI smoke passes.
  - The self-evaluation conclusion remains unchanged: OpenVisionLab should stay a rule-based OpenCvSharp4 PropertyGrid-centered workbench, not a hardware integration platform.
  - Original commit: `031c347`
- Line tool code-behind cleanup continued.
  - Test-only selected-line configuration now lives in `LineToolInteractionController`; `LineToolWpfView` keeps the public test hooks as thin wrappers.
  - Original commit: `2ed377a`
- Pipeline Review metric wording was clarified.
  - `ResultCount` now displays as `결과 수` in Korean Pipeline Review NG guidance.
  - The Product sample NG review smoke now asserts localized metric display text in the guide detail.
  - Original commit: `dabf398`
- MainView/Product sample user-flow was re-evaluated with current Dev build screenshots.
  - The bottom workflow strip shows product group, Good/Bad direction, NG/OK counterpart action, Pipeline Review, and first-step action.
  - Pipeline Review shows Good/Bad pair context, metric check, checklist, and explicit counterpart-open action.
  - No additional UI change was made in this pass.
- Pipeline Review sample metric explanations were localized.
  - `ResultCount`, `MeanValueAvg`, and `DistanceMmAvg` now use localized display names in Pipeline Review result detail, Good/Bad pair text, metric check, and checklist text.
  - Mean NG fix detail no longer repeats raw `AcceptanceMessage`; it points the operator to input layer, ROI, Mean type, lighting/brightness drift, and target range.
  - `PipelineViewerScreenshotSmoke` now rejects raw expected metric keys in localized guide/detail/pair text.
  - Original commit: `470f863`
- Product sample catalog was re-audited after metric explanation cleanup.
  - Dev and Original audits both passed: `ProductSampleQualityAudit=PASS | PairRecords=84 OK=84 Review=0 Critical=0`.
  - The catalog still has 84 Good rows and 84 Bad rows, with 84 PairGroups and one shared baseline pipeline per pair.
  - No new product samples are warranted before improving review/explanation UX further.
- Tool View code-behind candidate review was performed.
  - `git diff --no-index --ignore-space-at-eol` between Original and Dev WPF Tool View files produced no semantic diff.
  - Current Dev Tool View diff is mostly Dev baseline/line-ending noise; Original already has the reviewed controller/runtime cleanup.
  - No Tool View code change was made in this pass.
- Pipeline Review guide labels were localized.
  - `Good/Bad Pair` now displays as `Good/Bad 쌍`.
  - `Metric Check` now displays as `지표 확인`.
  - Original commit: `71ecc21`
- Pipeline Review NG parameter-location hints were added.
  - NG detail now keeps the reason, first check, and `조정 위치:` in the same operator guide line.
  - Tool-type-specific hints point to the PropertyGrid parameter panel areas for Threshold, Blob, Contour, Line, Mean, Matching/Feature, and generic steps.
  - `PipelineViewerScreenshotSmoke` now requires the localized parameter-location prefix and `파라미터 패널` text in NG guide detail.
  - Original commit: `2371b37`
- Pipeline Review parameter panel focus hints were added.
  - The lower Parameters panel now repeats the selected NG step's `조정 위치:` hint directly above the parameter list.
  - The hint is data-bound from `OpenVisionPipelineReviewGuideState.ParameterFocusText`; it does not trigger Preview or Run.
  - ShellHost test hooks and `PipelineViewerScreenshotSmoke` now verify the same focus text.
  - Original commit: `9c2bbe1`
- Pipeline Review metric gap explanation was added in Dev.
  - Acceptance NG text now keeps the localized metric name, measured value, target range, and adds the target gap such as `511` over max or `67.5` under min.
  - The change is centralized in `OpenVisionPipelineReviewGuidePresenter.FormatAcceptanceMetricNgReason`, so Pipeline Review detail and result detail use the same wording.
  - Current-build before/after screenshots were captured for the generic NG Pipeline Review path.

## Verification Evidence

- Dev build:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed.
- Original build:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed.
- Smoke tool build:
  - Dev: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed.
  - Original: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed.
- Readiness:
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab"` passed.
- Reference and sample policy checks:
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed in Dev and Original.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed in Dev and Original.
  - Re-run at 2026-07-03 19:27 KST passed in Dev and Original.
  - Re-run at 2026-07-03 21:16 KST passed in Dev and Original after parameter-location hints.
  - Re-run at 2026-07-03 21:48 KST passed in Dev and Original after parameter focus hints.
- Product sample full catalog:
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\original_product_catalog_full_20260703_1919` passed in Original.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_final_20260703_1920` passed in Original.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_after_line_controller_cleanup_20260703_1935` passed in Original.
  - Dev quality audit after metric cleanup: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -SummaryPath artifacts\self_evaluation_product_catalog_20260703_1750\sample_catalog_summary.json -OutputDir artifacts\product_quality_after_metric_cleanup_20260703_2018 -FailOnCritical` passed.
  - Original quality audit after metric cleanup: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -SummaryPath artifacts\product_catalog_after_line_controller_cleanup_20260703_1935\sample_catalog_summary.json -OutputDir artifacts\product_quality_after_metric_cleanup_20260703_2018 -FailOnCritical` passed.
- Sample review UI smoke runner:
  - Dev current-flow evaluation: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_user_flow_eval_20260703_1959` passed.
  - Dev: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_script_after_auditfix_20260703_1918` passed.
  - Original: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\original_sample_review_ui_smoke_script_after_auditfix_20260703_1919` passed.
  - Original re-run after layout smoke restore: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_after_layout_guard_restore_20260703_1903` passed.
  - Original re-run after Pipeline Review copy shortening: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_after_pipeline_copy_short_20260703_1915` passed.
  - Original final re-run: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_final_20260703_1930` passed.
  - Product sample NG after `ResultCount` wording: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\product_sample_review_ng_metric_display_after_original_20260703_1948` passed.
  - Dev Product OK after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\metric_display_product_ok_after_dev2_20260703_2030` passed.
  - Dev Product NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\metric_display_product_ng_after_dev2_20260703_2031` passed.
  - Dev Mean NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\metric_display_mean_after_dev6_20260703_2031` passed.
  - Dev Line NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\metric_display_line_after_dev3_20260703_2031` passed.
  - Original Product OK after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\metric_display_product_ok_after_original_20260703_2038` passed.
  - Original Product NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\metric_display_product_ng_after_original_20260703_2038` passed.
  - Original Mean NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\metric_display_mean_after_original_20260703_2039` passed.
  - Original Line NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\metric_display_line_after_original_20260703_2039` passed.
  - Dev Pipeline Review label localization: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_labels_after_dev_20260703_2027` passed.
  - Original Pipeline Review label localization: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_labels_after_original_20260703_2028` passed.
  - Dev Mean NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_parameter_location_after_dev_20260703_2105` passed.
  - Dev Line NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\pipeline_review_line_parameter_location_after_dev_20260703_2105` passed.
  - Dev generic NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_parameter_location_after_dev_20260703_2105` passed.
  - Original Mean NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_parameter_location_after_original_20260703_2115` passed.
  - Original Line NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\pipeline_review_line_parameter_location_after_original_20260703_2115` passed.
  - Original generic NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_parameter_location_after_original_20260703_2115` passed.
  - Dev Mean NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_parameter_focus_after_dev_20260703_2135` passed.
  - Dev Line NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\pipeline_review_line_parameter_focus_after_dev_20260703_2135` passed.
  - Dev generic NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_parameter_focus_after_dev_20260703_2135` passed.
  - Original Mean NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_parameter_focus_after_original_20260703_2145` passed.
  - Original Line NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\pipeline_review_line_parameter_focus_after_original_20260703_2145` passed.
  - Original generic NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_parameter_focus_after_original_20260703_2145` passed.
  - Dev metric gap before: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_metric_gap_before_dev_20260703_01` passed.
  - Dev metric gap after: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_metric_gap_after_dev_20260703_01` passed.
  - Dev sample NG gap after: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_metric_gap_sample_ng_after_dev_20260703_01` passed.
  - Dev metric gap build: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed.
- Filter/Morphology guard:
  - Dev: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard artifacts\filter_morphology_layout_guard_after_dev_20260703_1903` passed.
  - Original: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard artifacts\filter_morphology_layout_guard_after_original_20260703_1908` passed.
- Pipeline Review OK/NG:
  - Original OK: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review artifacts\pipeline_review_ok_after_smoke_restore_20260703_1906` passed.
  - Original NG: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_after_smoke_restore_20260703_1906` passed.
  - Original OK after copy shortening: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review artifacts\pipeline_review_top_card_short_after_original_20260703_1917` passed.
  - Original NG after copy shortening: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_top_card_short_ng_after_original_20260703_1917` passed.
  - Original final OK: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review artifacts\pipeline_review_ok_final_20260703_1931` passed.
  - Original final NG: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_final_20260703_1931` passed.
- Line tool controller cleanup:
  - Dev: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_controller_test_hook_after_dev_20260703_1932` passed.
  - Dev: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_controller_intersection_after_dev_20260703_1932` passed.
  - Original: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_controller_test_hook_after_original_20260703_1934` passed.
  - Original: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_controller_intersection_after_original_20260703_1935` passed.

## Screenshot Evidence

- Product sample focus after:
  - Dev: `C:\Git\OpenVisionLab_Dev\artifacts\sample_workflow_pair_hint_after2_20260703_1821\wpf_shell_host_workspace_sample_product_focus_open.png`
  - Original: `C:\Git\OpenVisionLab\artifacts\original_sample_workflow_pair_hint_after_20260703_1825\wpf_shell_host_workspace_sample_product_focus_open.png`
- Product sample NG review after:
  - Dev: `C:\Git\OpenVisionLab_Dev\artifacts\operator_review_pair_flow_after_fix_20260703_1833\wpf_shell_host_workspace_product_sample_review_ng.png`
  - Original: `C:\Git\OpenVisionLab\artifacts\original_product_sample_review_ng_after_fix_20260703_1836\wpf_shell_host_workspace_product_sample_review_ng.png`
- Contour teaching preview clear after:
  - Dev: `C:\Git\OpenVisionLab_Dev\artifacts\contour_teaching_clear_after_20260703_1849\wpf_shell_host_contour_tool.png`
  - Original: `C:\Git\OpenVisionLab\artifacts\original_contour_teaching_clear_after_20260703_1851\wpf_shell_host_contour_tool.png`
- PropertyGrid duplicate-key smoke after:
  - Dev: `C:\Git\OpenVisionLab_Dev\artifacts\property_grid_duplicate_key_string_after_20260703_1855\wpf_property_grid_matching_combo.png`
  - Original: `C:\Git\OpenVisionLab\artifacts\original_property_grid_duplicate_key_string_after_20260703_1857\wpf_property_grid_matching_combo.png`
- Filter/Morphology layout guard after:
  - Dev: `C:\Git\OpenVisionLab_Dev\artifacts\filter_morphology_layout_guard_after_dev_20260703_1903\wpf_filter_morphology_layout_guard.png`
  - Original: `C:\Git\OpenVisionLab\artifacts\filter_morphology_layout_guard_after_original_20260703_1908\wpf_filter_morphology_layout_guard.png`
- Pipeline review OK/NG after:
  - OK: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ok_after_smoke_restore_20260703_1906\wpf_shell_host_pipeline_review.png`
  - NG: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ng_after_smoke_restore_20260703_1906\wpf_shell_host_pipeline_review_ng.png`
- Pipeline review next-action copy after:
  - OK: `C:\Git\OpenVisionLab\artifacts\pipeline_review_top_card_short_after_original_20260703_1917\wpf_shell_host_pipeline_review.png`
  - NG: `C:\Git\OpenVisionLab\artifacts\pipeline_review_top_card_short_ng_after_original_20260703_1917\wpf_shell_host_pipeline_review_ng.png`
- Final review smoke after:
  - Sample runner: `C:\Git\OpenVisionLab\artifacts\sample_review_ui_smoke_final_20260703_1930`
  - Pipeline OK: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ok_final_20260703_1931\wpf_shell_host_pipeline_review.png`
  - Pipeline NG: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ng_final_20260703_1931\wpf_shell_host_pipeline_review_ng.png`
- Product sample NG metric wording after:
  - Before: `C:\Git\OpenVisionLab\artifacts\sample_review_ui_smoke_final_20260703_1930\wpf_shell_host_workspace_product_sample_review_ng.png`
  - Dev after: `C:\Git\OpenVisionLab_Dev\artifacts\product_sample_review_ng_metric_display_after_dev_20260703_1946\wpf_shell_host_workspace_product_sample_review_ng.png`
  - Original after: `C:\Git\OpenVisionLab\artifacts\product_sample_review_ng_metric_display_after_original_20260703_1948\wpf_shell_host_workspace_product_sample_review_ng.png`
- MainView/Product sample current-flow evaluation:
  - Dev Product focus: `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_user_flow_eval_20260703_1959\wpf_shell_host_workspace_sample_product_focus_open.png`
  - Dev Product pair handoff: `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_user_flow_eval_20260703_1959\wpf_shell_host_workspace_product_sample_pair_open.png`
- Pipeline Review metric explanation cleanup:
  - Mean before: `C:\Git\OpenVisionLab\artifacts\metric_display_mean_before_original_20260703_2002\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Line before: `C:\Git\OpenVisionLab\artifacts\metric_display_line_before_original_20260703_2002\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Dev Mean after: `C:\Git\OpenVisionLab_Dev\artifacts\metric_display_mean_after_dev6_20260703_2031\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev Line after: `C:\Git\OpenVisionLab_Dev\artifacts\metric_display_line_after_dev3_20260703_2031\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Original Mean after: `C:\Git\OpenVisionLab\artifacts\metric_display_mean_after_original_20260703_2039\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original Line after: `C:\Git\OpenVisionLab\artifacts\metric_display_line_after_original_20260703_2039\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Original Product OK after: `C:\Git\OpenVisionLab\artifacts\metric_display_product_ok_after_original_20260703_2038\wpf_shell_host_workspace_product_sample_review.png`
  - Original Product NG after: `C:\Git\OpenVisionLab\artifacts\metric_display_product_ng_after_original_20260703_2038\wpf_shell_host_workspace_product_sample_review_ng.png`
- Pipeline Review guide label localization:
  - Before: `C:\Git\OpenVisionLab\artifacts\metric_display_mean_after_original_20260703_2039\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_labels_after_dev_20260703_2027\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_labels_after_original_20260703_2028\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
- Pipeline Review parameter-location hints:
  - Original Mean before: `C:\Git\OpenVisionLab\artifacts\pipeline_review_parameter_location_before_original_20260703_2110\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev Mean after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_parameter_location_after_dev_20260703_2105\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original Mean after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_parameter_location_after_original_20260703_2115\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev Line after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_line_parameter_location_after_dev_20260703_2105\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Original Line after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_line_parameter_location_after_original_20260703_2115\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Original generic NG before: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ng_parameter_location_before_original_20260703_2110\wpf_shell_host_pipeline_review_ng.png`
  - Dev generic NG after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_ng_parameter_location_after_dev_20260703_2105\wpf_shell_host_pipeline_review_ng.png`
  - Original generic NG after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ng_parameter_location_after_original_20260703_2115\wpf_shell_host_pipeline_review_ng.png`
- Pipeline Review parameter focus hints:
  - Dev Mean before: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_parameter_focus_before_dev_20260703_2130\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original Mean before: `C:\Git\OpenVisionLab\artifacts\pipeline_review_parameter_focus_before_original_20260703_2140\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev Mean after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_parameter_focus_after_dev_20260703_2135\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original Mean after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_parameter_focus_after_original_20260703_2145\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev Line after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_line_parameter_focus_after_dev_20260703_2135\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Original Line after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_line_parameter_focus_after_original_20260703_2145\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Dev generic NG after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_ng_parameter_focus_after_dev_20260703_2135\wpf_shell_host_pipeline_review_ng.png`
  - Original generic NG after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ng_parameter_focus_after_original_20260703_2145\wpf_shell_host_pipeline_review_ng.png`
- Pipeline Review metric gap explanation:
  - Dev generic NG before: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_metric_gap_before_dev_20260703_01\wpf_shell_host_pipeline_review_ng.png`
  - Dev generic NG after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_metric_gap_after_dev_20260703_01\wpf_shell_host_pipeline_review_ng.png`
  - Dev sample Mean NG after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_metric_gap_sample_ng_after_dev_20260703_01\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
- Product field-style sample import:
  - Imported 16 project-authored field-style PNGs into `docs\samples\public\product\field` with clean product/inspection names and 960px max dimension.
  - Added 16 `Product_Field_*` Explore rows to `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`.
  - Added field sample provenance rows to `docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` and the product sample generator manifest output.
  - Added three exploratory baseline pipelines: `Product_Field_DarkFeature_Contour.pipeline.xml`, `Product_Field_BrightFeature_Contour.pipeline.xml`, and `Product_Field_SurfaceMean.pipeline.xml`.
  - Added a `Field` product/tool focus option in the sample picker so field-style samples are not buried in the Product catalog list.
  - Contact sheet: `C:\Git\OpenVisionLab_Dev\artifacts\user_sample_import_review_20260703\imported_field_sample_contact_sheet.png`.
  - Current overlay contact sheet after metric tuning: `C:\Git\OpenVisionLab_Dev\artifacts\field_sample_catalog_20260703_03\field_overlay_contact_sheet.png`.
  - Field rows now carry expected metric ranges based on current runner output: `ResultCount` for contour pipelines and `MeanValueAvg` for the surface mean pipeline.
  - Self-evaluation: keep these 16 samples as `Explore` rows for now. They are useful, more field-like recipe setup examples, but several overlays are intentionally broad and should not be promoted to controlled Good/Bad pairs without tighter per-sample pipelines.
  - UI before/after captures:
    - Before Original: `C:\Git\OpenVisionLab\artifacts\field_sample_focus_before_original_20260703\wpf_shell_host_workspace_sample_product_focus_picker.png`
    - After Dev: `C:\Git\OpenVisionLab_Dev\artifacts\field_sample_focus_after_dev_20260703\wpf_shell_host_workspace_sample_product_focus_picker.png`
  - Verification:
    - `dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform="Any CPU"` passed.
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed in Dev and Original for sample picker capture.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed.
    - Representative runner smoke passed for dark contour, bright contour, and surface mean pipelines under `artifacts\field_sample_smoke_20260703`.
    - Field-only catalog gate after expected metric tuning: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath artifacts\field_sample_catalog_20260703_03\field_sample_catalog.csv -OutputDir artifacts\field_sample_catalog_20260703_03 -SkipRunnerBuild -FailOnExplore` passed with `GateStatus=OK`, `RunnableRows=16`, `OKRows=16`, `NGRows=0`.
    - Full Product catalog after field metric tuning: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_184_after_field_metric_gap_dev_20260703_01 -SkipRunnerBuild` passed with `GateStatus=OK`, `RunnableRows=184`, `OKRows=184`, `NGRows=0`.
    - Public sample policy after metric tuning: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
    - Source/privacy check after rename/import: `rg -n "ChatGPT|C:\\Git\\새 폴더|새 폴더|DALL|OpenAI" docs\samples\public\product docs\samples\OpenVisionLab.ProductSampleCatalog.csv tools\GenerateOpenVisionProductSamples.ps1` returned no matches.

- Tool View code-behind cleanup, Line tool:
  - Moved Line ROI/default-ROI mutation from `0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs` into `0. UI\6) Vision Test\Wpf\Behaviors\LineToolInteractionController.cs`.
  - The View now delegates `EnsureDefaultRoi`, `ApplySelectedLineRoi`, and `SetRoiForTest` to the controller. Existing `VisionToolPropertyChangeController.RefreshAfterExternalUpdate` behavior remains the single path for summary/overlay/preview policy updates.
  - Preserved PropertyGrid-based Line parameter editing and did not add broad base classes or new runtime abstractions.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\LineToolInteractionController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_tool artifacts\line_tool_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_tool_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_tool_controller_refactor_dev_20260703_01` passed.
- Pipeline/Recipe operator review UX, NG next action focus:
  - `OpenVisionPipelineReviewGuidePresenter.ResolveNextActionText` now keeps the existing generic NG instruction but prefixes it with the failed acceptance metric or tool-specific focus area.
  - Example after: `평균 밝기(Mean) 기준 확인 / 파라미터/라우트 조정 후 재리뷰`.
  - `PipelineViewerScreenshotSmoke` now asserts the NG next action contains the localized failed metric name for sample NG and generic acceptance NG review targets.
  - UI before/after captures:
    - Before Dev: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_operator_review_before_dev_20260703_01\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
    - After Dev: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_operator_review_after_dev_20260703_01\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
    - Generic NG after Dev: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_operator_review_after_dev_20260703_01\wpf_shell_host_pipeline_review_ng.png`
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\0) MENU\Wpf\OpenVisionPipelineReviewGuidePresenter.cs" "tools\PipelineViewerScreenshotSmoke\Program.cs" "0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\LineToolInteractionController.cs" "docs\OPENVISIONLAB_NEXT_SESSION_HANDOFF.md"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_operator_review_after_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_operator_review_after_dev_20260703_01` passed.
- Tool View code-behind cleanup, SimplePreprocess settings restore:
  - `SimplePreprocessParameterController.ApplySettings` now preserves the previous suppress state using its existing `isSuppressed` dependency.
  - `SimplePreprocessToolWpfView.ApplyPersistedSettings` now delegates suppression to the controller and only replays the lightweight `ParameterChanged` path after restore.
  - This keeps dynamic SimplePreprocess parameter restore behavior out of View code-behind without changing the generated parameter UI contract.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\SimplePreprocessParameterController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_simple_preprocess_result_review artifacts\simple_preprocess_controller_refactor_dev_20260703_01` passed.
- MainView/Product sample review, Field Explore affordance:
  - Fixed Field focus filtering in `OpenVisionWorkspaceSampleFocusOption.Matches`; `field` now requires `ValidationMode=Explore` and field-style product tokens instead of falling through the generic LearnPath fallback.
  - Added Field focus smoke target `wpf_shell_host_workspace_sample_product_field_focus_picker`.
  - For Explore samples, `OpenVisionWorkspaceSamplePickerViewModel` now shows `Explore 샘플`, formats expected ranges as reference metrics rather than fixed OK/NG criteria, and exposes a short guide explaining that the sample is for recipe setup rather than a controlled Good/Bad decision pair.
  - `OpenVisionWorkspaceSamplePickerView.xaml` displays the Explore guide in the benchmark strip via `WorkspaceSamplePickerExploreGuide`.
  - Before/after evidence:
    - Initial Field target before focus fix failed: selected `Product_Battery_TabGap_Good` with `Mode=Required` after choosing Field.
    - Field after focus fix, before Explore guide: `C:\Git\OpenVisionLab_Dev\artifacts\product_field_explore_guide_before_dev_20260703_02\wpf_shell_host_workspace_sample_product_field_focus_picker.png`
    - Field after Explore guide: `C:\Git\OpenVisionLab_Dev\artifacts\product_field_explore_guide_after_dev_20260703_01\wpf_shell_host_workspace_sample_product_field_focus_picker.png`
    - Existing Product focus after: `C:\Git\OpenVisionLab_Dev\artifacts\product_field_explore_guide_after_dev_20260703_01\wpf_shell_host_workspace_sample_product_focus_picker.png`
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\0) MENU\Wpf\OpenVisionWorkspaceSampleFocusOption.cs" "0. UI\0) MENU\Wpf\OpenVisionWorkspaceSamplePickerView.xaml" "0. UI\0) MENU\Wpf\OpenVisionWorkspaceSamplePickerViewModel.cs" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_field_focus_picker artifacts\product_field_explore_guide_after_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_picker artifacts\product_field_explore_guide_after_dev_20260703_01` passed.
- Original repo reviewed patch import, Pipeline Review metric guidance:
  - Imported only the reviewed Pipeline Review metric-gap/NG next-action focus change into `C:\Git\OpenVisionLab`; did not bulk-copy Dev.
  - Original touched files:
    - `C:\Git\OpenVisionLab\0. UI\0) MENU\Wpf\OpenVisionPipelineReviewGuidePresenter.cs`
    - `C:\Git\OpenVisionLab\tools\PipelineViewerScreenshotSmoke\Program.cs`
  - Deferred Field Explore sample/UI import into Original because Original does not yet contain the Product Field sample assets/catalog rows. Importing only the Field UI would expose dead or untestable affordance.
  - Original after capture:
    - `C:\Git\OpenVisionLab\artifacts\pipeline_operator_review_original_after_20260703_01\wpf_shell_host_pipeline_review_ng.png`
    - `C:\Git\OpenVisionLab\artifacts\pipeline_operator_review_original_after_20260703_01\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\0) MENU\Wpf\OpenVisionPipelineReviewGuidePresenter.cs" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_operator_review_original_after_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_operator_review_original_after_20260703_01` passed.
- Product sample catalog quality follow-up, Dev 184-row gate:
  - Re-ran the full Product catalog after the Field Explore import and current review UX changes.
  - Full catalog summary: `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=72.891`.
  - Product sample quality audit passed with `PairRecords=84`, `OK=84`, `Review=0`, `Critical=0`.
  - Evidence:
    - Catalog run output: `C:\Git\OpenVisionLab_Dev\artifacts\product_catalog_quality_followup_dev_20260703_01\sample_catalog_summary.json`
    - Catalog run report: `C:\Git\OpenVisionLab_Dev\artifacts\product_catalog_quality_followup_dev_20260703_01\sample_catalog_report.md`
    - Quality audit report: `C:\Git\OpenVisionLab_Dev\artifacts\product_catalog_quality_followup_audit_dev_20260703_01\product_sample_quality_audit.md`
  - Verification:
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_quality_followup_dev_20260703_01 -SkipRunnerBuild` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -SummaryPath artifacts\product_catalog_quality_followup_dev_20260703_01\sample_catalog_summary.json -OutputDir artifacts\product_catalog_quality_followup_audit_dev_20260703_01 -FailOnCritical` passed.
  - Self-evaluation: no additional product sample generation is needed before stabilizing/importing the current Dev UX and Tool View changes. The remaining sample risk is not quantity, but whether the 16 Field Explore samples should later receive tighter per-sample pipelines before promotion to controlled Good/Bad pairs.
- Tool View code-behind cleanup, single-input PropertyGrid shell layout:
  - Moved docked/floating density and layout mutation out of `VisionToolSingleInputPropertyToolShell.xaml.cs` into `VisionToolSingleInputPropertyToolShell.DockedInspectorLayoutController.cs`.
  - The original shell file now keeps dependency properties, exposed controls, and the `DockedInspectorModeChanged` event path; `ApplyDockedInspectorMode` delegates to the layout controller.
  - `VisionToolSingleInputPropertyToolShell.xaml.cs` is now 189 lines; the extracted layout controller is 150 lines.
  - This is behavior-preserving: no algorithm tool PropertyGrid contract was changed, and no Preview/Run trigger path was added.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\VisionToolSingleInputPropertyToolShell.xaml.cs" "0. UI\6) Vision Test\Wpf\VisionToolSingleInputPropertyToolShell.DockedInspectorLayoutController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_blob_tool artifacts\single_input_property_shell_layout_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_contour_tool_docked_verification artifacts\single_input_property_shell_layout_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_tool artifacts\single_input_property_shell_layout_refactor_dev_20260703_01` passed.
- Tool View code-behind cleanup, double-input custom shell layout:
  - Moved docked/floating preview-card density, input-B visibility, and offset action row layout out of `VisionToolDoubleInputCustomToolShell.xaml.cs` into `VisionToolDoubleInputCustomToolShell.DockedInspectorLayoutController.cs`.
  - The shell file now keeps dependency properties, exposed controls, and public layout commands that delegate to the controller.
  - `VisionToolDoubleInputCustomToolShell.xaml.cs` is now 107 lines; the extracted layout controller is 121 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\VisionToolDoubleInputCustomToolShell.xaml.cs" "0. UI\6) Vision Test\Wpf\VisionToolDoubleInputCustomToolShell.DockedInspectorLayoutController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\double_input_shell_layout_refactor_dev_20260703_01` passed.
- Tool View code-behind cleanup, Line result review controller:
  - Added `LineToolReviewController` to coordinate Line result chips and verification/failure guide updates.
  - `LineToolWpfView.xaml.cs` now delegates Line, Distance, and Intersection result review presentation plus teaching-summary reset to the controller.
  - `LineToolWpfView.xaml.cs` is now 407 lines; the extracted review controller is 102 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\LineToolReviewController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_tool artifacts\line_tool_review_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_tool_review_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_tool_review_controller_refactor_dev_20260703_01` passed.
- Tool View code-behind cleanup, Arithmetic settings restore:
  - `ArithmeticToolInteractionController` now receives the current suppress state and preserves it while applying operation lists or persisted settings.
  - `ArithmeticToolWpfView.ApplyPersistedSettings` now delegates directly to the controller.
  - `ArithmeticToolWpfView.xaml.cs` is now 290 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\ArithmeticToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\ArithmeticToolInteractionController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\arithmetic_controller_suppression_refactor_dev_20260703_01` passed.
- Pipeline/Recipe operator review UX, Step Flow operator focus:
  - Added a small `PipelineReviewStepFlowOperatorFocus` strip inside the Step Flow panel.
  - It reuses `ReviewGuideParameterFocusText`, so the selected NG step shows the operator/parameter location where the user is already choosing the step.
  - This does not add a Tool View launch command; no command surface is available there yet, so the safer improvement is clearer operator focus near the selected step.
  - Before capture: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_operator_focus_before_dev_20260703_01\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - After captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_operator_focus_after_dev_20260703_02\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_operator_focus_after_dev_20260703_02\wpf_shell_host_workspace_product_sample_review.png`
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_operator_focus_after_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\pipeline_review_operator_focus_after_dev_20260703_02` passed.
- MainView/Product sample review current-flow recheck:
  - Rechecked the current Dev build after Tool View and Pipeline Review changes.
  - The product sample workflow strip still shows the explicit counterpart/sample review actions and does not auto-run Preview/Run during open/counterpart switching.
  - Current captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260703_02\wpf_shell_host_workspace_sample_product_focus_picker.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260703_02\wpf_shell_host_workspace_sample_product_focus_open.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260703_02\wpf_shell_host_workspace_sample_product_counterpart_open.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260703_02\wpf_shell_host_workspace_sample_product_field_focus_picker.png`
  - Verification:
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_picker artifacts\mainview_product_flow_recheck_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_open artifacts\mainview_product_flow_recheck_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_counterpart_open artifacts\mainview_product_flow_recheck_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_field_focus_picker artifacts\mainview_product_flow_recheck_dev_20260703_02` passed.
  - Self-evaluation: no further Product sample workflow UI work is needed before stabilization. The next value is import/readiness review or additional Tool View cleanup, not more visible copy.
- Tool View code-behind cleanup, Threshold test configuration:
  - `VisionToolThresholdInteractionController` now owns the Basic/Invert test configuration path and preserves the previous suppress state while changing coupled radio buttons.
  - `ThresholdToolWpfView.ConfigureBasicInvertForTest` delegates to the controller.
  - `ThresholdToolWpfView.xaml.cs` is now 221 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\ThresholdToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\VisionToolThresholdInteractionController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_basic_tool artifacts\threshold_controller_test_config_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_tool artifacts\threshold_controller_test_config_refactor_dev_20260703_01` passed.
- Tool View code-behind cleanup, Morphology kernel binding flush:
  - Added `VisionToolKernelSizeController.FlushParameterBindings`.
  - `MorphologyToolWpfView` now delegates width/height binding flush to the kernel controller before creating properties or refreshing the summary.
  - `MorphologyToolWpfView.xaml.cs` is now 195 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\VisionToolKernelSizeController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard artifacts\morphology_kernel_flush_controller_refactor_dev_20260703_01` passed.
- Dev stabilization checkpoint after Tool View/Pipeline Review/MainView loop:
  - Verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
- Pipeline/Recipe operator review UX, first NG step navigation:
  - Added a manual `NG Step` button to Pipeline Review next to Previous/Next.
  - The button selects the first enabled step whose review result is NG after explicit `Run Review`; it does not trigger Preview/Run.
  - The button is disabled for OK-only review results and is exposed through shell-host test hooks.
  - The multi-step NG smoke now selects a later OK step, clicks the visible `btnFirstIssueStep` button, verifies that the first NG Threshold step is selected, and confirms no native Preview/Run count increase.
  - Before first-issue button capture:
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_operator_focus_after_dev_20260703_02\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - After captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_first_issue_after_dev_20260703_02\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_first_issue_after_dev_20260703_02\wpf_shell_host_workspace_product_sample_review.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_first_issue_navigation_dev_20260704_01\wpf_shell_host_pipeline_review_ng.png`
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_first_issue_after_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\pipeline_review_first_issue_after_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_first_issue_navigation_dev_20260704_01` passed.
    - `git diff --check -- "0. UI\0) MENU\Wpf\Documents\OpenVisionPipelineReviewDocument.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostStatePresenter.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostToolTestFacade.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostView.TestHooks.cs" "0. UI\0) MENU\Wpf\ViewModels\OpenVisionPipelineReviewViewModel.cs" "0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml" "0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml.cs" "Library\OpenVisionLab.Localization\Resources\LocalizationCatalog.tsv" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
- Tool View code-behind cleanup, Line preview controller:
  - Added `LineToolPreviewController` to own Line tool debounced auto-preview scheduling, threshold teaching preview requests, and input ROI overlay refresh.
  - `LineToolWpfView.xaml.cs` now delegates preview/ROI state to the controller and is reduced from 407 lines to 369 lines.
  - This is behavior-preserving: Line still uses explicit Preview/Run requests, and property changes only schedule through the existing `VisionToolPropertyPreviewPolicy`.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_tool artifacts\line_preview_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_preview_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_preview_controller_refactor_dev_20260703_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\LineToolPreviewController.cs"` passed with CRLF warnings only.
- Tool View code-behind cleanup, Arithmetic preview controller:
  - Added `ArithmeticToolPreviewController` to own debounced auto-preview scheduling and the Offset-mode vs normal Preview request split.
  - `ArithmeticToolWpfView.xaml.cs` now delegates preview scheduling to the controller and is reduced from 290 lines to 276 lines.
  - Behavior is unchanged: Offset mode still uses `Run Offset`, normal mode still uses `Run Preview`, and parameter changes go through the existing `VisionToolParameterChangeController`.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\arithmetic_preview_controller_refactor_dev_20260703_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\ArithmeticToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\ArithmeticToolPreviewController.cs"` passed with CRLF warnings only.
- Tool View code-behind cleanup, Threshold schedule wrapper removal:
  - Removed the View-local `ScheduleAutoPreview` wrapper from `ThresholdToolWpfView`.
  - Scheduling now goes directly through `VisionToolParameterChangeController` and `VisionToolDebouncedPreviewScheduler`, which already own suppress and loaded-state checks.
  - `ThresholdToolWpfView.xaml.cs` is now 212 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_basic_tool artifacts\threshold_schedule_simplification_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_tool artifacts\threshold_schedule_simplification_dev_20260703_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\ThresholdToolWpfView.xaml.cs"` passed with CRLF warnings only.
- Tool View code-behind cleanup, SimplePreprocess apply-settings ownership:
  - `SimplePreprocessParameterController.ApplySettings` now owns the post-restore `RefreshProgrammatic(notifyChanged: true)` path.
  - `SimplePreprocessToolWpfView.ApplyPersistedSettings` no longer manually raises `ParameterChanged`; the View is reduced from 285 lines to 283 lines.
  - `PipelineViewerScreenshotSmoke` combo/slider auto-preview mutations now choose a value different from the current persisted setting, so the smoke is not dependent on local settings store state.
  - Verification:
    - Initial `wpf_preprocess_output_preview_flow` run failed because the smoke re-selected an already persisted Filter/RotateScale value and did not trigger a change event.
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors after the smoke fix.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_preprocess_output_preview_flow artifacts\simple_preprocess_apply_settings_controller_dev_20260703_03` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_simple_preprocess_result_review artifacts\simple_preprocess_apply_settings_controller_dev_20260703_03` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_preprocess_existing_output_write artifacts\simple_preprocess_layer_contract_20260704_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\SimplePreprocessParameterController.cs" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
- Tool View code-behind cleanup, SimplePreprocess parameter facade removal:
  - `SimplePreprocessToolWpfView` now exposes its existing `SimplePreprocessParameterController` through an internal `Parameters` property instead of forwarding every `Add*`, `Get*`, visibility, settings capture, and settings restore method.
  - SimplePreprocess configurator/property/preview/factory code now uses `view.Parameters` directly, so parameter generation and mapping stay in the controller/runtime path.
  - `SimplePreprocessToolWpfView.xaml.cs` is now 180 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_preprocess_output_preview_flow artifacts\simple_preprocess_parameter_facade_refactor_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_simple_preprocess_result_review artifacts\simple_preprocess_parameter_facade_refactor_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_preprocess_existing_output_write artifacts\simple_preprocess_parameter_facade_refactor_20260704_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessDocumentFactory.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessViewConfigurator.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessPropertyFactory.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessPreviewExecutor.cs"` passed with CRLF warnings only.
- Arithmetic route smoke stabilization:
  - `wpf_layer_selection_arithmetic_tool` exposed a WPF `PopupControlService` stale HWND exception while docking the floating Arithmetic tool after combo popup checks.
  - The smoke runner now closes the Arithmetic combo popups before docking and ignores only Win32 error 1400 during dispatcher pump cleanup; behavioral assertions still fail normally from the verification action.
  - Verification:
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\arithmetic_popup_cleanup_stabilized_20260704_01` passed.
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessDocumentFactory.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessViewConfigurator.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessPropertyFactory.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessPreviewExecutor.cs" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
- Dev stabilization checkpoint after first-issue navigation and Tool View preview-controller refactors:
  - Verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
- Self-evaluation document refresh:
  - `docs\OPENVISIONLAB_SELF_EVALUATION_20260703.md` now references the current Dev product catalog result: `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`.
  - The Product sample catalog score was adjusted from `4.2 / 5` to `4.3 / 5`.
  - Verification:
    - `git diff --check -- "docs\OPENVISIONLAB_SELF_EVALUATION_20260703.md"` passed.
- Product field-style sample catalog follow-up:
  - Confirmed `C:\Git\새 폴더` source images are represented in Dev as 16 renamed field-style images under `docs\samples\public\product\field`.
  - Current repo sample names/catalog rows do not include `ChatGPT` or `OpenAI` markers.
  - Visual contact sheet for review:
    - `C:\Git\OpenVisionLab_Dev\artifacts\field_sample_quality_review_20260704_01\field_sample_contact_sheet.png`
  - Verification:
    - `rg -n "ChatGPT|OpenAI|generated by" "docs\samples\public\product" "docs\samples\OpenVisionLab.ProductSampleCatalog.csv"` found only the README policy line describing deterministic/project-authored samples; no ChatGPT/OpenAI marker is present in catalog/image names.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -SummaryPath artifacts\product_catalog_quality_followup_dev_20260703_01\sample_catalog_summary.json -FailOnCritical` passed with `PairRecords=84`, `OK=84`, `Review=0`, `Critical=0`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_full_recheck_20260704_01 -SkipRunnerBuild` completed with summary `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=62.722`.
- MainView/Product sample review current-flow recheck:
  - Rechecked current Dev build after Pipeline Review first-issue navigation, Tool View cleanup, and sample catalog follow-up.
  - Current captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260704_01\wpf_shell_host_workspace_sample_product_focus_picker.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260704_01\wpf_shell_host_workspace_sample_product_focus_open.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260704_01\wpf_shell_host_workspace_sample_product_counterpart_open.png`
  - Verification:
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_picker artifacts\mainview_product_flow_recheck_dev_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_open artifacts\mainview_product_flow_recheck_dev_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_counterpart_open artifacts\mainview_product_flow_recheck_dev_20260704_01` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_20260704_01` passed with `Targets=6`.
- Latest Dev stabilization checkpoint after 2026-07-04 00:00 changes:
  - Verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
- Final Dev recheck on 2026-07-04 before the 02:00 handoff:
  - `git diff --check` passed with CRLF warnings only.
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_final_20260704_01` passed with `Targets=6`.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\final_ui_recheck_dev_20260704_01` passed; capture: `C:\Git\OpenVisionLab_Dev\artifacts\final_ui_recheck_dev_20260704_01\wpf_shell_host_pipeline_review_ng.png`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_full_final_20260704_01 -SkipRunnerBuild` completed with summary `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=61.536`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -SummaryPath artifacts\product_catalog_full_final_20260704_01\sample_catalog_summary.json -FailOnCritical` passed with `PairRecords=84`, `OK=84`, `Review=0`, `Critical=0`.
- Post-00:23 continuation, Pipeline Review progress summary:
  - Added a compact header progress line for Pipeline Review: `OK x / NG y / 대기 z`, plus `OFF z` when disabled steps exist.
  - The progress text is owned by `OpenVisionPipelineReviewDocument.FormatReviewProgressText`, surfaced through the Pipeline Review view/viewmodel, and exposed through ShellHost test hooks for smoke assertions.
  - During current-build visual verification, the first after-capture showed `실행 중...` still present after completion. The run completion path now recalculates progress after `isRunningReview=false` in the common `finally` block.
  - Current before/after captures:
    - Before: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_progress_before_20260704_01\wpf_shell_host_pipeline_review_ng.png`
    - After: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_progress_after_20260704_03\wpf_shell_host_pipeline_review_ng.png`
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_progress_after_20260704_03` passed and now asserts the progress text directly.
    - `git diff --check -- "0. UI\0) MENU\Wpf\Documents\OpenVisionPipelineReviewDocument.cs" "0. UI\0) MENU\Wpf\ViewModels\OpenVisionPipelineReviewViewModel.cs" "0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml" "0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostStatePresenter.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostToolTestFacade.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostView.TestHooks.cs" "Library\OpenVisionLab.Localization\Resources\LocalizationCatalog.tsv" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
- Post-00:44 continuation, Blob/Contour area review controller cleanup:
  - Added `VisionToolThresholdTeachingPreviewController` for the shared threshold-teaching preview request flag used by Blob and Contour.
  - Added a `VisionToolSingleInputPropertyToolController<TProperty>.ShowAreaResultReview(...)` overload that pairs verification guide update with the existing area result review presenter.
  - `BlobToolWpfView.xaml.cs` and `ContourToolWpfView.xaml.cs` now delegate the duplicated result-list filtering and teaching-preview state to shared runtime/controller code.
  - Code-behind line counts after cleanup: Blob 161, Contour 159.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_blob_tool artifacts\area_review_controller_refactor_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_contour_tool artifacts\area_review_controller_refactor_20260704_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\BlobToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\ContourToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\VisionToolSingleInputPropertyToolController.cs"` passed with CRLF warnings only.
    - `Select-String -LiteralPath "0. UI\6) Vision Test\Wpf\VisionToolThresholdTeachingPreviewController.cs" -Pattern '[ \t]+$'` found no trailing whitespace.
- Post-tool-review stabilization checkpoint:
  - Verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_post_tool_review_20260704_01 -SkipRunnerBuild` completed with summary `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=68.489`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -SummaryPath artifacts\product_catalog_post_tool_review_20260704_01\sample_catalog_summary.json -FailOnCritical` passed with `PairRecords=84`, `OK=84`, `Review=0`, `Critical=0`.
- MainView/Product sample review post-progress recheck:
  - Rechecked the sample-selection-to-product-review flow after the Pipeline Review progress summary change.
  - Current captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_ui_smoke_post_progress_20260704_01\wpf_shell_host_workspace_sample_product_focus_open.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_ui_smoke_post_progress_20260704_01\wpf_shell_host_workspace_product_sample_review.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_ui_smoke_post_progress_20260704_01\wpf_shell_host_workspace_product_sample_review_ng.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_ui_smoke_post_progress_20260704_01\wpf_shell_host_workspace_product_sample_pair_open.png`
  - Visual check: the header progress line does not overlap the Good/Bad pair guide; it shows `미실행` before review and `OK 1 / NG 1 / 대기 0` after the controlled NG product review run.
  - Verification:
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_post_progress_20260704_01` passed with `Targets=6`.
- Product sample review progress assertion hardening:
  - Added `AssertPipelineReviewProgressText` in `PipelineViewerScreenshotSmoke` and wired it into Product sample review OK/NG paths.
  - The smoke now verifies the visible progress summary counts and also checks that `실행 중...` does not remain after review completion.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\product_review_progress_assertion_20260704_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\product_review_progress_assertion_20260704_02` passed.
    - `git diff --check -- tools\PipelineViewerScreenshotSmoke\Program.cs` passed with CRLF warnings only.
- Matching-family result review title cleanup:
  - `VisionToolSingleInputMatchingToolController<TProperty>` now owns the result review title supplied at attach time.
  - Matching, EdgeBasedMatching, and FeatureMatching views no longer pass the same title string on every `SetResultReview` call.
  - Code-behind line counts after cleanup: Matching 142, EdgeBasedMatching 137, FeatureMatching 133.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_matching_tool artifacts\matching_review_title_controller_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_edge_based_matching_tool artifacts\matching_review_title_controller_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_feature_matching_tool artifacts\matching_review_title_controller_20260704_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\MatchingToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\EdgeBasedMatchingToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\FeatureMatchingToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\VisionToolSingleInputMatchingToolController.cs"` passed with CRLF warnings only.
- Post-matching-cleanup stabilization checkpoint:
  - Verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `git diff --check` passed with CRLF warnings only.
- Single-input custom tool shell base refactor:
  - Added `VisionToolSingleInputCustomToolViewBase` to own the repeated single-input custom tool shell/event/status/preview-image command forwarding.
  - Switched `ThresholdToolWpfView`, `FilterToolWpfView`, `MorphologyToolWpfView`, and `SimplePreprocessToolWpfView` XAML roots from `UserControl` to the shared base so the generated WPF partial classes share the same controller plumbing.
  - Removed repeated forwarding code from the four code-behind files while keeping each tool's parameter UI, presenter/controller setup, and explicit Preview/Run path unchanged.
  - Code-behind line counts after this cleanup: Filter 121, Morphology 116, Threshold 129, SimplePreprocess 86. Shared base is 165 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard,wpf_shell_host_threshold_tool,wpf_simple_preprocess_result_review artifacts\custom_tool_base_refactor_20260704_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_threshold_tool,wpf_preprocess_output_preview_flow,wpf_layer_selection_preprocess_existing_output_write artifacts\custom_tool_base_refactor_route_20260704_01` passed.
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `git diff --check` passed with CRLF warnings only.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\VisionToolSingleInputCustomToolViewBase.cs" "0. UI\6) Vision Test\Wpf\FilterToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\FilterToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\ThresholdToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\ThresholdToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml.cs"` passed with CRLF warnings only.
  - Current captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_20260704_02\wpf_filter_morphology_layout_guard.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_20260704_02\wpf_shell_host_threshold_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_20260704_02\wpf_simple_preprocess_result_review.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_route_20260704_01\wpf_layer_selection_threshold_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_route_20260704_01\wpf_preprocess_output_preview_flow.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_route_20260704_01\wpf_layer_selection_preprocess_existing_output_write.png`
- Pipeline Review flow status badge improvement:
  - `OpenVisionPipelineReviewDocument.ResolveFlowStatus` now maps completed successful review steps to `Passed` and NG/acceptance-NG review steps to `Failed`.
  - This uses the existing `PipelineFlowView` OK/NG badge colors instead of leaving completed review rows as generic `LOAD/WAIT`, making the left Step Flow usable as an operator review map.
  - UI evidence:
    - Before capture with the old LOAD/WAIT mapping: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_flow_status_before_20260704_01\wpf_shell_host_pipeline_review_ng.png`
    - After capture with the NG flow badge visible: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_flow_status_after_20260704_01\wpf_shell_host_pipeline_review_ng.png`
  - Verification:
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_flow_status_before_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_flow_status_after_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\product_review_flow_status_after_20260704_01` passed.
  - Product sample review recheck capture:
    - `C:\Git\OpenVisionLab_Dev\artifacts\product_review_flow_status_after_20260704_01\wpf_shell_host_workspace_product_sample_review_ng.png`
- Product catalog recheck after Tool/Pipeline UX changes:
  - Verification:
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_after_base_and_flow_20260704_01 -SkipRunnerBuild` passed.
    - Summary: `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=63.456`, `ArtifactIssueCount=0`, `MetadataIssueCount=0`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -SummaryPath artifacts\product_catalog_after_base_and_flow_20260704_01\sample_catalog_summary.json -FailOnCritical` passed with `PairRecords=84`, `OK=84`, `Review=0`, `Critical=0`.
- Final Dev verification checkpoint at 2026-07-04 01:21 +09:00:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
  - `git diff --check` passed with CRLF warnings only.
- Custom tool extension guide update:
  - `docs\VISION_TOOL_NATIVE_WPF_EXTENSION_GUIDE.md` now names `VisionToolSingleInputCustomToolViewBase`, `AttachToolController(...)`, and the rule that single-input custom UI views must not copy event/status/preview forwarding or call `VisionToolSingleInputCustomToolRuntime` directly.
  - Verification:
    - `git diff --check -- docs\VISION_TOOL_NATIVE_WPF_EXTENSION_GUIDE.md` passed with CRLF warnings only.
- Original repo recheck at 2026-07-04 01:21 +09:00:
  - `git fetch origin` completed in `C:\Git\OpenVisionLab`.
  - Original remains dirty with a subset of earlier sample/catalog/Pipeline Review/Tool View changes and field sample files.
  - Latest original commits remain:
    - `e11b724 Record pipeline review parameter focus hints`
    - `9c2bbe1 Show pipeline review parameter focus hints`
    - `c90d60a Record pipeline review parameter location hints`
    - `2371b37 Add pipeline review parameter location hints`
    - `bc42e0e Record pipeline review label polish`
  - No Dev-to-Original import was performed for the latest custom tool base refactor or Pipeline Review flow status badge change.
- Original repo status check:
  - `C:\Git\OpenVisionLab` is already dirty with a subset of sample/catalog/Pipeline Review/Tool View changes.
  - No bulk copy from Dev was performed in this checkpoint.
  - `git fetch origin` completed with no output.
  - Dev changes not yet present in Original include Pipeline Review first-issue navigation and the latest Tool View preview-controller cleanup. Import these later as reviewed patch groups, not as a bulk folder copy.
  - Verification in `C:\Git\OpenVisionLab`:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
- Original repo recheck after the post-00:23 Dev continuation:
  - `git fetch origin` completed.
  - `C:\Git\OpenVisionLab` remains dirty with a subset of earlier sample/catalog/Pipeline Review changes and field sample files.
  - Latest original commits:
    - `e11b724 Record pipeline review parameter focus hints`
    - `9c2bbe1 Show pipeline review parameter focus hints`
    - `c90d60a Record pipeline review parameter location hints`
    - `2371b37 Add pipeline review parameter location hints`
    - `bc42e0e Record pipeline review label polish`
  - No Dev-to-Original import was performed for the latest Pipeline Review progress summary, Blob/Contour cleanup, Product review progress smoke assertion, or Matching-family title cleanup.
  - Import only by reviewed patch groups after choosing the target group; do not copy the Dev tree over Original.

## Dev To Original Import Groups

Do not import the whole Dev tree. Review and move the current Dev changes in small groups:

1. Product field sample catalog
   - Candidate files: `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`, `docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv`, `docs\samples\public\product\field\*`, `docs\samples\public\product\Product_Field_*.pipeline.xml`, `0. UI\6) Vision Test\VisionPipelineSampleCatalog.cs`, and sample generation/policy docs.
   - Required checks after import: product asset policy, full product catalog gate, sample quality audit.
2. MainView/Product sample review UX
   - Candidate files: `OpenVisionShellHostSampleWorkflowPresenter`, sample picker view/viewmodel, sample focus/pair decision helpers, shell command surface, and related UI smoke updates.
   - Required checks after import: `tools\RunSampleReviewUiSmokes.ps1` and current-build screenshots.
3. Pipeline Review operator UX
   - Candidate files: Pipeline Review document/view/viewmodel, guide/result presenters, localization keys, shell host test hooks, and PipelineViewerScreenshotSmoke updates.
   - Required checks after import: `wpf_shell_host_pipeline_review_ng`, product sample review target, and no Preview/Run count increase on `NG Step`.
4. Tool View controller cleanup
   - Candidate files: preview/review/text/controller classes and the touched Tool View code-behind files.
   - Required checks after import: focused Tool View WPF smokes for Threshold, Line, Arithmetic, SimplePreprocess, Filter/Morphology.
5. Policy/runtime cleanup
   - Candidate files: external reference/readiness scripts, project references, native DLL placement, and policy docs.
   - Required checks after import: solution build, readiness, external references, public sample assets.

## Start Checklist

```powershell
cd C:\Git\OpenVisionLab_Dev
git status --short
git log --oneline -5

cd C:\Git\OpenVisionLab
git fetch origin
git status --short
git log --oneline -5
```

## 2026-07-14 Learn User-Copy Audit

- Audited the common Learn header, all 17 Learn topics, the OpenCvSharp foundations visualization, and every user-openable file under `docs\learn`.
- Removed learner-facing engineering agreements such as no-auto-run/routing invariants, smoke/readiness wording, runtime-contract wording, and scope/backlog notes. Those rules remain in `AGENTS.md`, engineering contracts, and regression checks.
- Rewrote the common practice panel around the operator workflow: open a Good/Bad pair, open the related tool, run Preview or Pipeline Review, then compare the input, output, overlay, and metric.
- Reworked the previously unclear foundations view into two explicit visual sequences:
  - BGR pixel -> B/G/R channels -> Gray GV.
  - Point/Size -> Rect/RotatedRect -> ROI and PropertyGrid values.
- Updated the readiness contract so it checks positive learner guidance and rejects internal engineering phrases instead of requiring those phrases in user documentation.
- Added the same forbidden-copy regression across all 17 topic windows and their resolved Learn documents in `PipelineViewerScreenshotSmoke`.
- Current-source UI evidence:
  - Before: `C:\Git\OpenVisionLab_Dev\artifacts\learn_copy_audit_20260714\before_shell\wpf_shell_host_learn_entry.png`.
  - After, same shell target: `C:\Git\OpenVisionLab_Dev\artifacts\learn_copy_audit_20260714\after_shell\wpf_shell_host_learn_entry.png`.
  - After, latest full BGR/Gray foundations detail: `C:\Git\OpenVisionLab_Dev\artifacts\learn_copy_audit_20260714\after_final_curriculum\wpf_openvision_learn_curriculum.png`.
  - A final shell rerun also passed all assertions, but its `RenderTargetBitmap` intermittently saved partially black regions. The invalid `after_final_shell` PNG is excluded from visual evidence; this is a screenshot-tool capture issue, not a reproduced product-window defect.
- Verification:
  - `wpf_openvision_learn_curriculum`: `OK`, `layout=0`, `text=0`, `internal=0`, `1040x980`.
  - `wpf_shell_host_learn_entry`: `OK`, `layout=0`, `text=0`, `internal=0`, `1040x980`.
  - Learn user-facing internal-copy audit: `PASS`, matches `0`.
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: passed with 0 warnings and 0 errors.
  - `dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"`: passed.
  - `tools\TestExternalReferences.ps1`: passed.
- `tools\TestPublicSampleAssets.ps1`: passed with `CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`.
- Latest current-workspace EXE: `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-15 10:06:25 KST`.

## 2026-07-15 Recipe/Pipeline Responsibility Round Trip

- Recipe Manager remains the recipe library and summary surface; Pipeline Review remains the explicit execution and evidence surface.
- Pipeline Review now shows the owning recipe, keeps `Run Review` explicit, and provides `Return to Recipe` in its header.
- Returning closes Pipeline Review and reopens the same Recipe Manager summary. It does not run native Preview, create/remove layers, change the active layer, or change recipe/pipeline routing.
- Current-source UI evidence:
  - Before: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_recipe_roundtrip_20260715\before\wpf_shell_host_workspace_sample_pipeline_review_metrics.png`.
  - After Pipeline Review: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_recipe_roundtrip_20260715\after_pipeline_retry\wpf_shell_host_workspace_sample_pipeline_review_metrics.png`.
  - After returned Recipe Manager summary: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_recipe_roundtrip_20260715\after_summary\wpf_shell_host_recipe_manager_summary.png`.
- Latest-EXE evidence:
  - Pipeline Review: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_recipe_roundtrip_20260715\direct_exe_roundtrip_retry\OpenVisionLab_PipelineReview_Roundtrip.png`.
  - Returned Recipe Manager summary: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_recipe_roundtrip_20260715\direct_exe_roundtrip_retry\OpenVisionLab_RecipeManager_Roundtrip_Return.png`.
  - Report: `Result: PASS`, explicit review `OK`, native Preview runs `0`, layer count preserved at `1`.
- Verification:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: passed with 0 warnings and 0 errors.
  - Direct EXE `recipe-pipeline-roundtrip`: passed.
  - Existing direct EXE `recipe-manager-tabs`: passed after the new flow was added.
  - Current-source `wpf_shell_host_recipe_manager_summary`: passed with `layout=0`, `text=0`, `internal=0`; the assertion also docks Pipeline Review before using `Return to Recipe`, covering the docked close path.
  - Current-source `wpf_shell_host_workspace_sample_pipeline_review_metrics`: passed with `layout=0`, `text=0`, `internal=0`.

## 2026-07-15 Recipe Manager Sample Evidence Semantics

- The Recipe Manager sample selector is a workspace-global public/product catalog selection and defaults to the first runnable sample. It is not a persisted link to every selected recipe.
- The summary now labels that value `현재 작업 샘플` and explains that a sample check is required before it becomes recipe evidence.
- `OpenVisionRecipeSampleRunSummary` now keeps the recipe and pipeline used by the sample execution. The summary displays the compact result only when both match the selected recipe and pipeline; otherwise it shows `아직 검사하지 않음`.
- Opening Pipeline Review, running isolated Review, and returning to Recipe Manager do not create sample-check evidence or cause the global sample name to appear as the recipe's latest result.
- Current-source evidence is under `artifacts\recipe_manager_sample_semantics_20260715`: `before` reproduces the false implication and `after_final` shows the corrected card. The focused smoke passed with `layout=0`, `text=0`, and `internal=0`.
- Latest EXE proof used `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-15 11:07:05 KST`. `direct_exe_final\report.txt` passed with `RecipeSampleExecution=False`, `RecipeSampleResult=아직 검사하지 않음`, native Preview runs `0`, and layer count `1`; `direct_exe_regression_final\report.txt` also passed the full Recipe Manager regression.

## 2026-07-15 Pipeline Review Input-State Semantics

- Pipeline Review no longer labels every unexecuted Step as the same condition. An enabled Step with no input image and no earlier enabled producer now shows `입력 없음`.
- If an earlier enabled Step will produce the selected input layer, the downstream Step remains `WAIT` and the guide says explicit Review will create that input.
- The new status is read-only. Step selection did not increase native Preview runs or create/change layers and routing.
- Current-source before/after evidence is under `artifacts\pipeline_review_input_state_20260715\before_final` and `after_final_valid`. Both focused captures passed with `layout=0`, `text=0`, and `internal=0`; `producer_wait_regression` passed the existing three-Step execution path.
- Latest EXE proof used `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-15 11:29:05 KST`. `direct_exe\report.txt` passed with `PendingProducedInput=WAIT / 리뷰 실행으로 이전 Step 입력 생성`, review OK, native Preview runs `0`, and layer count `1`.
- The first full `recipe-manager-tabs` attempt failed while locating the pipeline-filter TextBox before any Pipeline input-state assertion. Immediate `direct_exe_regression_retry` rerun passed the full scenario, so keep the one-off result as direct-smoke UI timing sensitivity rather than product evidence.

## Next Priorities

1. Real external LLM XML correction-loop evidence, only when an API key or manually exported transcript is available. Do not fabricate evidence or spend model work while the prerequisite is absent.
2. Add one compact contextual Learn entry from the selected Pipeline Review tool when a matching Learn topic exists. Opening Learn must not change parameters, create layers, alter routing, or run Preview/Run.
3. Recheck Recipe Manager only when a real operator recipe or fresh current-build capture proves clipping, overlap, duplicated responsibility, or unclear next action.
4. Broaden pipeline branch/output review only when a real recipe exceeds the current comparison coverage.
5. Resume Tool View code-behind cleanup only when a visible defect or established controller/runtime owner justifies it; do not refactor for line-count reduction.

## 2026-07-15 P6 Run History Batch Analytics

- Audited the existing batch summary, Validation Suite, Run History, baseline comparison, and structured run-report paths before implementation.
- Batch rows already persist per-sample `TotalMilliseconds`. The selected saved run now derives judgement failure rate plus performance average, median, nearest-rank p95, and maximum without changing the saved XML/TSV schema or adding telemetry infrastructure.
- The existing `Benchmark 회귀 비교` summary shows the analytics as one compact read-only line above the outcome comparison. Correctness and performance labels remain separate.
- At this checkpoint the audit found that `VisionPipelineBatchSampleRunResult.RunReportPath` was not populated by the sample-suite path. The later Step-report persistence slice below closes that evidence gap.
- Current-source before: `artifacts\p6_benchmark_analytics_20260715\before\wpf_shell_host_recipe_local_validation_set.png`.
- Current-source after: `artifacts\p6_benchmark_analytics_20260715\after_final\wpf_shell_host_recipe_local_validation_set.png`; focused smoke passed with `layout=0`, `text=0`, and `internal=0`.
- Latest EXE proof used `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-15 12:05:58 KST`. Direct `recipe-manager-tabs` passed under `artifacts\p6_benchmark_analytics_20260715\direct_exe_final` and reported `RunHistoryAnalytics: 판정 실패율 50.0% | 성능 평균 2.8 ms · 중앙 2.8 ms · p95 3.1 ms · 최대 3.1 ms`.
- The first two attempted after captures ran a stale screenshot-tool copy because `--no-build` was used after only the solution build; they are not valid evidence. The screenshot-tool project was then rebuilt explicitly before the valid `after_final` evidence.

## 2026-07-15 P6 Compatible Baseline Timing

- Selected-sample, Good/Bad pair, Catalog, and Local Validation Set saves now carry distinct suite kinds.
- Run History compares baseline-to-current average and p95 only when suite kind, suite name, and the sorted sample-image multiset match and both runs have complete positive timings.
- Outcome comparison is intentionally independent. Switching to a different-suite baseline still produced `Still NG`, but the summary explicitly skipped timing; restoring the compatible baseline restored `Regression` and `+0.3 ms` average/p95 deltas.
- No new panel, storage schema, regression gate, Preview/Run action, layer mutation, or routing change was added.
- Fresh latest-EXE before: `artifacts\p6_baseline_timing_comparison_20260715\before_exe_01\OpenVisionLab_RecipeManager_RunHistory.png`.
- Fresh latest-EXE after: `artifacts\p6_baseline_timing_comparison_20260715\after_exe_04\OpenVisionLab_RecipeManager_RunHistory.png`.
- `after_exe_01` and `after_exe_03` passed behavior checks but their Run History images were mostly black from WPF composition timing; do not use them as UI evidence. `after_exe_02` rendered correctly but predates the final empty-suite guard.
- Latest EXE/DLL timestamp is `2026-07-15 12:26:00 KST`. Direct `recipe-manager-tabs` passed and reported `RunHistoryPerformanceComparison` with average `2.5 -> 2.8 ms (+0.3)` and p95 `2.8 -> 3.1 ms (+0.3)`.
- Current-source local Validation Set smoke passed with `layout=0`, `text=0`, and `internal=0` under `artifacts\p6_baseline_timing_comparison_20260715\after_current_source_02`.
- The next P6 evidence gate at this checkpoint was the real `RunReportPath` persistence path. The later Step-report persistence slice below closes it.

## 2026-07-15 P6 Structured Step-Report Link

- Added metadata-only `VisionRecipeRunResult` persistence through the existing `VisionPipelineRunReport` schema, including the pipeline snapshot and per-Step elapsed time/metrics without duplicating images.
- Selected-sample, Good/Bad pair, Catalog, and Local Validation Set suite executions now save one structured report per sample and copy its path into `VisionPipelineBatchSampleRunResult.RunReportPath`.
- The plain single `Run check` path remains non-persisting, so the change adds no hidden history side effect to the smallest explicit check.
- Latest solution build passed with 0 warnings and 0 errors. Latest direct `recipe-manager-tabs` smoke passed under `artifacts/p6_step_report_link_20260715/direct_exe_current_build` and preserved `validation-suite-step-report/report.xml` plus `pipeline.xml`; the report contained one linked Step and the existing Run History analytics/performance-comparison assertions also passed.
- The following P6 per-Step bottleneck slice completed this next action.

## 2026-07-15 Commercial Vision UI Reference Review

- Captured official public authoring UI evidence for MVTec MERLIC Creator, Cognex VisionPro QuickBuild, Zebra Aurora Vision Studio, NI Vision Builder AI, and KEYENCE XG VisionEditor under `artifacts/commercial_vision_ui_reference_20260715`.
- The reusable comparison and source links are recorded in `docs/OPENVISIONLAB_COMMERCIAL_UI_REFERENCE_REVIEW_20260715.md`.
- Common useful pattern: keep the flow, selected Step parameters, image evidence, result meaning, and execution timing coherent. OpenVisionLab already has the structural surfaces; the next value is linked Step bottleneck evidence, not another Recipe Manager workspace.
- Explicitly rejected scope: camera/lighting/controller/PLC/I/O/deployment/HMI/account features, auto-execution, and replacing the ordered PropertyGrid Pipeline with a free-form graph.

## 2026-07-15 P6 Per-Step Run History Bottleneck

- `VisionPipelineBatchRunSummaryStorage.CalculateStepTimingAnalysis` loads linked reports only when every batch row has a path and readable file, recipe/pipeline identity matches, and Step index/name/tool/enabled/input/output definitions agree.
- The existing Run History now shows one compact `Step 병목` list ordered by p95. Each row contains Step index/name/tool, timing coverage, average, p95, and maximum; incomplete or incompatible coverage shows a reason and no partial rows.
- The analysis and selection path is read-only. It does not trigger Preview/Run, load images, create layers, or change routing.
- Current-source before: `artifacts/p6_step_bottleneck_20260715/before/wpf_shell_host_recipe_local_validation_set.png`.
- Current-source after: `artifacts/p6_step_bottleneck_20260715/after_final/wpf_shell_host_recipe_local_validation_set.png`; focused smoke passed with `layout=0`, `text=0`, and `internal=0` and verified both complete 4/4 report coverage and missing-path rejection.
- Latest-build direct `recipe-manager-tabs` passed under `artifacts/p6_step_bottleneck_20260715/direct_exe`; the smoke verified one linked selected-sample Step aggregate and rejected synthetic batch history without `RunReportPath`.
- The following P7 selected-Step coherence audit completed this next action.

## 2026-07-15 P7 Selected-Step Workspace Coherence

- Audited the real three-Step `Public_Matching_FixturePad` sample after explicit Pipeline Review execution with Step 2 selected.
- Step identity, Blob tool, `Main -> FixturePadBlob` route, branch explanation against `FixtureMatch`, both previews, `CvROI`/Fixture frame parameters, Fixture result metrics, and elapsed time were coherent.
- The visible gap was duplicate ordinal text (`02 02 Inspect Fixture Pad`) plus a narrow Step summary. The document/presenter now normalize an already-prefixed Step name and the existing summary grid gives the Step card enough width; no new panel or command was added.
- Current-source before: `artifacts/p7_selected_step_coherence_20260715/before/wpf_shell_host_workspace_sample_fixture_review.png`.
- Current-source after: `artifacts/p7_selected_step_coherence_20260715/after_r4/wpf_shell_host_workspace_sample_fixture_review.png`; focused smoke passed with `layout=0`, `text=0`, and `internal=0` and asserts Step/tool/route/previews/parameters/result/timing coherence.
- Latest-build `OpenVisionLab.exe` smoke: `artifacts/p7_selected_step_coherence_20260715/direct_exe_final_r2`; report passed and recorded `Selected Step: 02 Inspect Fixture Pad / Main -> FixturePadBlob` plus Fixture metrics and elapsed time.
- Two current-source capture attempts and the first final EXE capture rendered mostly black despite passing behavior checks; repeated captures produced normal frames. Treat those files as invalid screenshot evidence, not as current UI.
- The following P8 contextual Learn entry completed this next action.

## 2026-07-15 P8 Pipeline Review Contextual Learn Entry

- Pipeline Review now shows one compact `도구 배우기` command when the selected Step ToolType has a matching Learn topic. Unsupported review-only Steps such as `OverlayMerge` do not receive a misleading Learn entry.
- The command reuses the existing Learn window and topic catalog. Selecting the Fixture sample's Blob Step opens topic `5. Blob / 영역 검출`; no duplicate Learn surface or new review panel was added.
- View code-behind only forwards the click. Topic availability belongs to the Learn catalog, review selection state belongs to the Pipeline Review ViewModel/Document, and the shell command controller owns opening/reusing Learn.
- Opening and closing Learn does not run Preview/Run, add/delete layers, change the active layer or native routing, change Step parameters, alter the selected Step, or reset the completed review result.
- Fresh current-source before: `artifacts/p8_pipeline_contextual_learn_20260715/before/wpf_shell_host_workspace_sample_fixture_review.png`.
- Fresh current-source after: `artifacts/p8_pipeline_contextual_learn_20260715/after_r3/wpf_shell_host_workspace_sample_fixture_review.png`; the focused smoke passed with `layout=0`, `text=0`, and `internal=0`, opened Blob Learn, rejected the unsupported OverlayMerge entry, and restored the same selected Step. The first `after` frame was mostly black from WPF composition timing and is excluded from visual evidence.
- Latest-build EXE proof: `artifacts/p8_pipeline_contextual_learn_20260715/direct_exe`; `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe` opened the Blob Learn topic and returned to the same completed review state. The report and both actual-EXE screenshots passed.
- Next evidence-based priority: audit the failed/selected Step -> Tool View adjustment handoff. Implement a direct entry only if the current Step parameters can be loaded with clear edit/apply semantics and without changing routing or running Preview automatically.

## 2026-07-15 P9 Selected-Step Parameter Edit Handoff

- The Pipeline Review selected-Step parameter panel now provides one compact `설정 수정` command.
- The command closes Pipeline Review and opens the same recipe, pipeline, and 1-based Step in Recipe Manager `Advanced > Pipeline > XML/Step`, with the existing PropertyGrid editor visible in the current viewport.
- Recipe Manager remains the authoritative existing-Step edit/apply surface. The separate Tool View remains a detached tool session and is not presented as though it automatically writes settings back into a pipeline Step.
- Opening the edit handoff does not run Preview/Run, create/delete/load layers, change the active layer, or alter pipeline routing. Applying the edited parameters remains an explicit Recipe Manager XML action.
- Fresh current-source evidence:
  - Before: `artifacts/p9_pipeline_step_edit_handoff_20260715/before/wpf_shell_host_workspace_sample_fixture_review.png`.
  - After review command: `artifacts/p9_pipeline_step_edit_handoff_20260715/final_current_source_review/wpf_shell_host_workspace_sample_fixture_review.png`.
  - After handoff destination: `artifacts/p9_pipeline_step_edit_handoff_20260715/final_current_source/wpf_shell_host_pipeline_step_edit_handoff.png`.
- Latest-EXE evidence:
  - Pipeline Review: `artifacts/p9_pipeline_step_edit_handoff_20260715/final_direct_exe/OpenVisionLab_PipelineReview_Roundtrip.png`.
  - Recipe Manager Step editor: `artifacts/p9_pipeline_step_edit_handoff_20260715/final_direct_exe/OpenVisionLab_RecipeManager_Step_Edit_Handoff.png`.
  - Direct `recipe-pipeline-roundtrip` report passed with `StepEditHandoff: 2 / Threshold`, native Preview runs `0`, layer count `1`, and no recipe sample execution side effect.
- The intended-workbench maturity estimate remains 62-66%. This is a bounded workflow-clarity improvement, not evidence for a percentage increase.
- Next evidence-based priority: validate `설정 수정 -> PropertyGrid edit -> explicit XML apply -> explicit rerun` with one real operator recipe and inspect whether any correction evidence is still unclear. Do not add a second editor or automatic apply path without that evidence.

## 2026-07-15 P10 Fixture Step Edit, Apply, And Explicit Rerun

- The real public `Public_Fixture_Pad_Good` workflow exposed one context defect: entering selected-Step edit correctly opened the Fixture Blob PropertyGrid, but Recipe Manager could retain an unrelated prior/default work sample. The following Good/Bad command then executed that unrelated pair.
- `FocusPipelineStepForEdit` now aligns `SelectedSampleOption` only when the requested pipeline exactly matches a runnable catalog workspace pipeline name (`Sample_<catalog sample name>`). Ordinary recipe pipelines with no exact catalog match keep their current work-sample selection.
- The verified operator round trip is `Pipeline Review Step 2 Blob -> 설정 수정 -> MIN_AREA 700 to 750 -> explicit XML 반영 -> explicit Good/Bad 재검사`.
- Typing and XML apply do not run native Preview/Run, create or remove layers, change the active layer, or change input/output routing. The Good/Bad pair runs only after the explicit rerun command.
- Current-source before: `artifacts/p10_fixture_step_edit_roundtrip_20260715/before/wpf_shell_host_pipeline_step_edit_handoff.png`.
- Current-source after: `artifacts/p10_fixture_step_edit_roundtrip_20260715/current_source_final_current_build/wpf_shell_host_fixture_step_edit_apply_rerun.png`; the focused target `wpf_shell_host_fixture_step_edit_apply_rerun` passed with `layout=0`, `text=0`, and `internal=0`.
- Latest EXE proof used `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-15 16:56:36 KST`, under `artifacts/p10_fixture_step_edit_roundtrip_20260715/exe_after_final_current_build`.
- The direct `public-fixture-review` report passed with `MIN_AREA 700 -> 750`, work sample `Public_Fixture_Pad_Good`, and pair result `쌍 검사 OK / Public_Fixture_Pad_Good, Public_Fixture_Pad_Missing_Bad`.
- The latest `recipe-pipeline-roundtrip` regression under `artifacts/p10_fixture_step_edit_roundtrip_20260715/exe_no_catalog_regression` passed with the ordinary `Direct_Recipe_Roundtrip` pipeline retaining its prior work sample and producing no recipe-sample execution side effect.
- The intended-workbench maturity estimate remains 62-66%. This closes a real workflow context defect but does not broaden product scope.
- Next evidence-based priority: use a real external LLM transcript when available. Without one, inspect fresh current-build Recipe Manager/LLM Assistant evidence for actual clipping, overlap, or unclear next action; audit branch/output comparison only when a real multi-branch recipe demonstrates a gap.

## 2026-07-15 P11 Public GPT Pin-Gap Packet

- The manual GPT packet now uses only project-authored public images: `docs/samples/public/Line_Pins_Synthetic_OK.png` and `Line_Pins_Synthetic_WidePin_NG.png`. The local `Sample/EasyGauge/Pin 1.jpg` vendor/legacy path is no longer part of the external-send instructions.
- The first GPT round requires only the two images plus the complete contents of `llm_prompt_packets/pin_gap_distance/COPY_THIS_TO_GPT.md`. README, expanded references, and the correction template are not first-round uploads.
- The intent is fixed to whole-array adjacent-pin edge-to-edge gap measurement with four verified ROI windows: `108,170,65,120`, `204,170,65,120`, `300,170,65,120`, and `396,170,65,120`.
- Current-source runner evidence under `artifacts/llm_gpt_packet_public_pin_20260715` proves the starter contract before any external LLM claim: the nominal image passed all 9 reference Steps with `DistanceMmAvg=0.151..0.159` and `DistanceMmRange=0.006..0.012`; the wide-pin image failed at `0.116 < 0.14`.
- This is prompt/fixture readiness evidence only. No real GPT transcript has been received yet, and no external response may be described as validated until the user's raw response is preserved and run through Recipe Manager validation/correction/import review.
- Immediate next priority: receive the unchanged GPT XML response, preserve the raw prompt/response, validate in Recipe Manager, and use `02_PASTE_VALIDATION_NG_BACK_TO_GPT.md` in the same GPT task if correction is required.

## 2026-07-15 P12 Real GPT Pin-Gap Round 1

- The user manually transferred an actual GPT response. The exact GPT model/version was not provided, so do not infer or add one.
- The unchanged prompt and response are preserved under `artifacts/llm_transcripts/raw/20260715_pin_gap_gpt_round1`. The response SHA-256 is `D2944FF344CFECC9CA90F09EEEDD0006B1D7E85A3D79669E7EA2AD4F960EBF3E`.
- Recipe Manager validation/import smoke passed with XML syntax OK, deserialization OK, 9 Steps, 0 errors, 0 warnings, import enabled, and import completed. Import did not run the image; `ImageRun: SKIPPED` preserves the explicit-run contract.
- An independent contract preflight confirmed 8 `LineDistance` Steps: four `DistanceMmAvg` gates and four `DistanceMmRange` gates, followed by one `OverlayMerge` Step. The generic smoke recipe did not select a strict intent, so its `Intent contract: SKIP` line is not evidence that the tool family was unchecked.
- Explicit nominal-image execution passed all 9 Steps. The four average results were `0.151..0.159 mm`, and the four range results were `0.006..0.012 mm`.
- Explicit negative-image execution produced the expected product NG at the first Step: `DistanceMmAvg 0.116 < 0.14`. The smoke command returned exit code 1 because product NG is represented as a failed image run; this is expected negative-test evidence, not a validation failure.
- The combined validate/import/image smoke path hung and was discarded. Separate current-build validation/import and explicit image-run paths completed and are the accepted evidence for this round.
- This is a real manually transferred GPT direct-success transcript with zero corrections. It is not an NG-to-correction-loop transcript, and no failed round may be invented merely to create one.
- The intended-workbench maturity estimate remains 62-66%. P12 proves one external GPT round against the public pin-gap contract but does not yet establish provider/model breadth or correction-loop reliability.
- Next evidence-based priority: privacy-review and sanitize this direct-success transcript before any promotion outside the raw artifact area. Capture an actual correction round only when a future independent GPT/Gemini/Claude draft naturally fails validation or execution.

## 2026-07-15 P13 Sanitized GPT Direct-Success Candidate

- The P12 prompt and response passed privacy/public-asset review and were copied unchanged to `artifacts/llm_transcripts/sanitized/20260715_pin_gap_gpt_round1_direct_success`.
- Automated checks found zero absolute/user-home paths, email addresses, secret labels, or known private/legacy asset hints in `prompt.md` and `response.xml`.
- The sanitized prompt and response hashes exactly match the preserved raw evidence: prompt `96886DEEDA962E59E653EADA99AE4792CA130B0537018434793A38DFFCC04354`, response `D2944FF344CFECC9CA90F09EEEDD0006B1D7E85A3D79669E7EA2AD4F960EBF3E`.
- The candidate includes only the unchanged prompt/XML, derived nominal/negative result images, a repository-relative manifest, and the privacy review. Raw manifests and smoke reports were deliberately excluded because they contain local workspace or attachment paths.
- Public replay inputs remain `docs/samples/public/Line_Pins_Synthetic_OK.png` and `Line_Pins_Synthetic_WidePin_NG.png`; the XML has no template or external-file dependency.
- Classification remains narrow: real manually transferred GPT direct-success evidence, not API evidence and not a correction-loop transcript. Exact GPT model/version remains unknown.
- The intended-workbench maturity estimate remains 62-66%. The next evidence-based implementation priority is fresh current-build Recipe Manager/LLM Assistant UX inspection using this real draft; change only proven clipping, overlap, or unclear next-action friction. A real correction round remains blocked until an independent external response naturally fails.

## 2026-07-15 P14 Current-Build Recipe Manager LLM UX Recheck

- The unchanged 18:29:44 Debug build was still current because P13 changed only documentation and ignored transcript artifacts. `dotnet bin/Debug/OpenVisionLab.dll --smoke recipe-manager-tabs --output artifacts/p13_recipe_manager_llm_ux_baseline_20260715` passed.
- Fresh `OpenVisionLab_RecipeManager_LlmXml.png` inspection at 1600x900 found no clipped button/icon content, hidden combo/input text, or incoherent control overlap. The advanced LLM surface remains dense but is vertically scrollable and is not the default Recipe Manager summary.
- The smoke report confirms visible whole-array validate/import/sample-run guidance, actual latest-run `DistanceMmAvg`/`DistanceMmRange` feedback, validation issue rows, dependency rows, XML diff, and blocked invalid imports without Preview/Run side effects.
- No UI source was changed because the fresh evidence did not prove clipping, overlap, or an unclear next action. Do not perform another speculative Recipe Manager layout pass from this baseline.
- The WPF-render-only `OpenVisionLab_RecipeManager_LlmPinGapSkill.png` contains a large unpainted area and is not accepted as a full-window visual reference; use the screen-captured LLM XML image and report for this P14 review.
- Next evidence-based priority: run the real P12 four-branch `LineDistance` plus `OverlayMerge` recipe through Pipeline Review and determine whether the existing branch/output comparison explains its producer outputs. Extend comparison UX only if that real recipe exposes a concrete gap. No new external sample or GPT prompt is required for this audit.

## 2026-07-15 P15 Real GPT Overlay Source Review

- The real P12 GPT recipe exposed a concrete comparison defect: Pipeline Review considered only each Step's `InputLayer`, so the final `OverlayMerge.SourceLayers` dependencies appeared as unrelated same-input alternatives. Baseline evidence showed `SourceConsumerRelationsVisible: 0/4` and `OverlaySourceProducersVisible: 0/4`.
- Pipeline review now parses `SourceLayers` and shows the four range-evidence Steps as explicit overlay sources. Selecting a source Step shows its review-merge consumer before same-input alternatives; selecting the final overlay shows only its four declared source producers.
- Latest `OpenVisionLab.exe` evidence under `artifacts/p15_real_gpt_branch_review_20260715/final_exe_retry1` passed with `SourceConsumerRelationsVisible: 4/4`, `OverlaySourceProducersVisible: 4/4`, `PreviewRunCountUnchanged: 0`, and `ActiveLayerUnchanged: True`.
- The full `OpenVisionLab.exe` Recipe Manager regression under `artifacts/p15_real_gpt_branch_review_20260715/recipe_manager_regression_exe` passed, including the existing BentPin multi-branch and Contour 3+ fan-out coverage. The Contour regression now rejects an unrelated `OverlayMerge` as a same-input alternative and verifies the actual declared source route instead.
- Fresh current-source before evidence is under `artifacts/p15_real_gpt_branch_review_20260715/before_current_logic`; latest-EXE after evidence is under `artifacts/p15_real_gpt_branch_review_20260715/final_exe_retry1`. Intermediate captures with large WPF unpainted regions are excluded from visual evidence.
- The intended-workbench maturity estimate remains 62-66%. This closes one real multi-branch explanation defect; it does not prove provider breadth or a correction-loop transcript.
- Next evidence-based priority: keep the sanitized direct-success corpus separate until a deliberate repository-inclusion decision is made. Capture a correction loop only when a future independent external draft naturally fails. Extend branch/output comparison again only when another real recipe exposes a relationship not represented by direct `InputLayer` or declared `SourceLayers`.

## 2026-07-15 P16 Recipe Manager Workspace Separation

- Fresh latest-EXE review confirmed the user's concern was information architecture, not a color-only defect: advanced Pipeline review retained the outer recipe library/search, lifecycle CRUD, repeated global Step/guided text, technical tabs, and transfer commands at the same time.
- Recipe Manager now has two physically separate workspace states. Summary shows recipe search/library, one selected-recipe overview, and create/duplicate/rename/delete lifecycle commands. Advanced review hides those outer controls, opens `Pipeline review` at full width, and exposes `Build inspection`, `Pipeline review`, `LLM XML`, `Step preview`, plus a compact XML/review transfer strip and explicit `Back to summary`.
- The global advanced header now keeps only selected recipe, active pipeline, counts, and XML readiness. Repeated Step flow, status, and guided-next-action text were removed from that header; detailed flow remains in the XML/Step tab.
- Recipe Manager colors now use neutral charcoal surfaces and borders with cyan reserved for selection/primary state and green for readiness, reducing the previous one-note teal hierarchy.
- Direct smoke hygiene now deletes only reserved generated recipe names matching `Smoke_LlmBranch_`, `Smoke_LlmDraft_`, `Smoke_LlmIntentSkills_`, or `Smoke_RecipeManager_` plus an exact 12-hex suffix. Each affected scenario also cleans its current workspace in `finally`. After the latest EXE regression, `bin/Debug/RECIPE` contained only `Default`.
- Latest EXE before evidence: `artifacts/p16_recipe_manager_structure_20260715/before_exe/OpenVisionLab_RecipeManager_Summary.png` and `OpenVisionLab_RecipeManager_Pipeline.png`.
- Latest EXE after evidence: `artifacts/p16_recipe_manager_structure_20260715/after_exe_final_200617/OpenVisionLab_RecipeManager_Summary.png` and `OpenVisionLab_RecipeManager_Pipeline.png`; the matching regression report is `report.txt` in the same folder.
- Current-source focused summary and advanced smokes passed with `layout=0`, `text=0`, and `internal=0`. The full latest-EXE `recipe-manager-tabs` regression passed with the new summary/advanced separation, and the real GPT branch regression still passed `4/4`, `4/4`, Preview/Run delta `0`, active layer unchanged.
- The intended-workbench maturity estimate remains 62-66%. This materially improves novice entry and advanced-workspace hierarchy but does not simplify every technical panel inside advanced Pipeline review.
- Next evidence-based Recipe Manager priority: validate the new summary with a normal named operator recipe rather than a smoke name, then simplify one advanced Pipeline sub-workflow only if that real task still lacks an obvious next action. Do not reintroduce library/lifecycle controls into advanced review.

## 2026-07-15 P17 Operator-Named Recipe Manager Roundtrip

- The latest built `OpenVisionLab.exe` ran `recipe-pipeline-roundtrip` with the operator-style recipe `배터리 벤트 검사` and pipeline `벤트 영역 이진화`; evidence is under `artifacts/p17_recipe_manager_operator_flow_20260715/actual_exe_final_202115`.
- The roundtrip passed `Recipe summary -> Pipeline Review -> Return to Recipe -> Advanced review -> Return to summary`. Native Preview/Run remained `0`, layer count remained `1`, the active layer and recipe routing were unchanged, and the isolated Pipeline Review result was `OK`.
- Fresh screenshots show the normal recipe/pipeline names without clipping. The summary keeps `Open Pipeline` as its primary action, Pipeline Review exposes `Run Review` and `Return to Recipe`, and advanced review exposes `Run check` and `Back to summary`. No additional speculative Recipe Manager layout edit was made because this real task did not expose an unclear next action.
- `recipe-pipeline-roundtrip` now accepts optional `--recipe-name` and `--pipeline-name` text for this repeatable operator-facing check. It refuses to overwrite an existing recipe, deletes only the workspace it created in `finally`, and cleans stale reserved `Smoke_RecipeRoundtrip_<12 hex>` workspaces. The collision guard was verified against `Default`; it returned the expected failure and left `Default` unchanged.
- The intended-workbench maturity estimate remains 62-66%. P17 closes the P16 operator-name validation item but does not reduce every technical row inside advanced review.
- Next evidence-based priority: audit the P13 sanitized direct-success transcript candidate and present a repository-inclusion recommendation. Do not move sanitized evidence into permanent/public paths without explicit user approval.

## 2026-07-15 P18 Sanitized Transcript Publication Audit

- `docs\OPENVISIONLAB_LLM_TRANSCRIPT_PUBLICATION_REVIEW_20260715.md` records the reusable publication gate and the P13 candidate audit.
- The initial decision was `CONDITIONAL GO / CURRENTLY HOLD`. After the publication gate was presented, the user approved continuation and the minimum package was added to the Dev worktree at `docs\evidence\llm\20260715_pin_gap_gpt_direct_success`.
- Raw/sanitized prompt and response hashes match. The XML parses as 9 Steps with no external file/template dependency. Both replay inputs are registered project-authored public synthetic assets. Result PNGs contain no textual metadata.
- Latest-EXE replay under `artifacts/p18_llm_transcript_publication_review_20260715` passed validation/import with 9 Steps, 0 errors, 0 warnings, and `ImageRun: SKIPPED`; the nominal image passed 9/9 Steps, while the negative image produced the expected inspection NG at `DistanceMmAvg 0.116 < 0.14`.
- The current result images are byte-identical to the sanitized candidate images. No process or generated recipe remained after replay.
- The package contains only `README.md`, `prompt.md`, `response.xml`, and the two result images. Its README identifies GPT through user-operated ChatGPT, manual transfer, unknown exact model/version, direct success with zero corrections, AI-generated content, human review, hashes, and replay commands.
- Do not publish raw manifests or reports containing local paths, and do not place transcript evidence under `docs/samples/public`.
- The intended-workbench maturity estimate remains 62-66%. P18 strengthens evidence governance but does not add provider breadth or a correction-loop result.
- The Dev-worktree package has not been staged, committed, pushed, or copied to the Original repository. A real correction-loop transcript remains blocked until a future external response naturally fails; do not fabricate one. The next unblocked priority is a bounded Tool View code-behind cleanup only where an existing base/controller pattern naturally fits.

## 2026-07-15 P19 GPT Evidence Package Inclusion

- After the P18 publication gate was presented, the user approved continuation. The minimum evidence package now exists in the Dev worktree at `docs\evidence\llm\20260715_pin_gap_gpt_direct_success`.
- The package contains exactly five files: one combined disclosure/reproduction `README.md`, the unchanged `prompt.md` and `response.xml`, and byte-identical nominal/negative OpenVisionLab result PNGs. Raw manifests, local-path reports, attachment references, and API/session data remain excluded.
- `README.md` conspicuously identifies the GPT-generated response, user-operated ChatGPT transfer, unknown exact model/version, zero correction rounds, non-API classification, completed human review, publication approval, hashes, and exact replay commands. The result PNGs are identified as rule-based OpenVisionLab outputs rather than model-generated images.
- The package content check passed with five files, 9 XML Steps, matching prompt/response/result hashes, no detected absolute user path, credential token, or email, and a valid link to the P18 audit document.
- Latest-EXE replay from the new documentation path passed under `artifacts/p19_llm_evidence_package_20260715`: validate/import PASS with `ImageRun: SKIPPED`, nominal 9/9 PASS, expected negative NG at `DistanceMmAvg 0.116 < 0.14`, and byte-identical result hashes. No `OpenVisionLab` process or generated recipe directory remained.
- This was documentation/evidence inclusion only; no UI changed, so no before/after UI capture was required. The package has not been staged, committed, pushed, or copied to Original.
- The intended-workbench maturity estimate remains 62-66%. The next unblocked priority is a bounded Threshold Tool cleanup: move only Learn-window creation, activation, apply-event forwarding, unsubscription, and disposal from `ThresholdToolWpfView.xaml.cs` into a focused controller. Keep `VisionToolThresholdInteractionController`, the public test hook, explicit apply behavior, and all Preview/Run contracts unchanged. Existing `wpf_openvision_learn_threshold`, animation, apply, and native Threshold smokes provide direct regression coverage. Real correction-loop evidence still depends on a naturally failing external response.

## 2026-07-15 P20 Threshold Learn Window Controller

- Added `ThresholdToolLearnWindowController` as the owner of Learn-window creation, existing-window activation, apply-event forwarding, event detachment, closed-state reset, and disposal.
- `ThresholdToolWpfView` now owns only the controller reference and calls `Open()` or `Dispose()` from its existing XAML click handler, `OpenThresholdGuideForTest`, and tool cleanup path. Direct `OpenVisionLearnWindow` ownership and Learn event handlers are gone from the View; its code-behind decreased from 179 to 146 lines.
- The data path remains `Learn Apply event -> ThresholdToolLearnWindowController -> VisionToolThresholdInteractionController.ApplyBasicThresholdFromGuide`. Property creation still uses the existing `ThresholdToolPresenter`; no alternative PropertyGrid, parameter model, or automatic execution path was added.
- `wpf_shell_host_threshold_tool` now also verifies a second open activates the same single Learn window, closing clears controller state, a later open creates a new window, and open/apply/reopen does not increment native Preview/Run. The screenshot-smoke project was explicitly rebuilt after the new assertions; the final current-source smoke passed with `layout=0`, `text=0`, and `internal=0`.
- Full build and the explicit screenshot-smoke project build passed with 0 warnings and 0 errors. Readiness, external-reference, public-sample, and `git diff --check` passed.
- Valid pre-edit evidence is `artifacts/p20_threshold_learn_controller_20260715/before_current_source/wpf_openvision_learn_threshold_apply.png`; accepted verified after evidence is `artifacts/p20_threshold_learn_controller_20260715/after_visual_verified/wpf_openvision_learn_threshold_apply.png`; controller-path shell evidence is `artifacts/p20_threshold_learn_controller_20260715/after_shell_verified/wpf_shell_host_threshold_tool.png`.
- Before/after comparison found no layout/content change. Differing pixels were limited to the nondeterministically rendered instruction-text band (`1.142170%`, bounds `296,249-1007,309`).
- A first image-viewer read temporarily presented large black regions, but direct PNG pixel inspection and a repeat read showed normal light-surface pixels in every P20 file. This was a transient evidence-viewer presentation issue, not a corrupt or unpainted PNG. Do not add a screenshot failure rule from that false observation. The `colors=64` and `flat=0%` fields remain legacy placeholders, but replacing them is not a current product priority without a real capture defect.
- The intended-workbench maturity estimate remains 62-66%. P6 Step-report linkage and per-Step bottleneck analytics were already complete in the fresher product-target and stable-contract documents; the common Tool-header lifecycle slice below is the actual next completed priority. Real correction-loop evidence remains blocked on a naturally failing external response.

## 2026-07-15 P21 Common Tool Learn Window Controller

- Source audit found that `VisionToolSingleInputPropertyToolShell` and `VisionToolDoubleInputCustomToolShell` still created a new `OpenVisionLearnWindow` directly for every header Learn click. Repeated clicks could therefore leave duplicate Learn windows open.
- Added the shared `VisionToolLearnWindowController`. Both common Shells now delegate only `Open(LearnTopicIndex)`; the controller owns create, same-topic activation, topic-change replacement, closed-state reset, and window references.
- The common Learn smoke helper now proves that a second click reactivates the same single window, closing and clicking again creates a fresh window at the expected topic, and the entire open/reactivate/reopen sequence leaves Preview/Run unchanged.
- Explicit screenshot-smoke project build and full solution build passed with 0 warnings and 0 errors. `wpf_arithmetic_tool_learn_button` and `wpf_simple_preprocess_tool_learn_button` both passed with `layout=0`, `text=0`, and `internal=0`; readiness, external-reference, public-sample, and `git diff --check` also passed.
- Current-source evidence is under `artifacts/p21_common_learn_window_controller_20260715`: `before_arithmetic`/`after_arithmetic` cover the double-input Arithmetic Shell, and `before_single_input`/`after_single_input` cover the common single-input Shell. The Arithmetic capture used different persisted responsive widths before and after, but both layouts are complete; the equal-size single-input comparison changed only `0.387352%` in the preview-render region.
- No Tool parameter, PropertyGrid, layer, input/output route, or execution path changed. Product maturity remains 62-66%.
- Next priority remains conditional: capture a natural external LLM correction round when available; otherwise change Recipe Manager/LLM or branch comparison only when fresh real evidence exposes a gap. Do not continue code-behind cleanup solely to reduce line count.

## 2026-07-15 P22 Recipe Manager Guided Setup Korean Operator Copy

- A fresh latest-EXE `recipe-manager-tabs` audit exposed one real novice-facing issue: the Korean Recipe Manager still showed English operator guidance in Guided Setup/LLM, including `Guided setup / LLM assistant`, `Required inputs`, `Starter XML creation only`, `READY/MISSING` explanations, and the pin-gap average-only warning.
- `OpenVisionShellHostRecipeCommandSurface` now localizes the visible title, summary, required-input help, ready/missing explanations, and pin-gap calibration advice. Technical identifiers such as `MM-READY`, `PX-ONLY`, `PIXELPERMM`, `DistanceMmAvg`, `DistanceMmRange`, `SCORE_MIN`, and `ResultCount` remain unchanged. The copied external-LLM prompt remains English by design.
- `wpf_shell_host_recipe_guided_setup` now rejects Korean operator guidance that falls back to `Required inputs` or `average-only measurement`, while also requiring the Korean title, summary, readiness, and calibration phrases.
- Valid latest-EXE before/after evidence is `artifacts/p22_current_recipe_llm_audit_20260715/before_current_exe/OpenVisionLab_RecipeManager_GuidedSetup.png` and `after_current_exe/OpenVisionLab_RecipeManager_GuidedSetup.png`. The valid after LLM task view is `after_current_exe/OpenVisionLab_RecipeManager_LlmIntentLineDistance.png`. Final current-source assertion evidence is `after_current_source_guided_final/wpf_shell_host_recipe_guided_setup.png` with `layout=0`, `text=0`, and `internal=0`.
- Some other LLM captures in this run intermittently presented large dark composition regions; they are excluded from visual evidence. The valid Guided Setup and LLM intent captures plus the full direct smoke report are the current evidence.
- Full solution build and explicit screenshot-smoke project build passed with 0 warnings and 0 errors. Latest-EXE `recipe-manager-tabs`, readiness, external-reference, and public-sample checks passed; no OpenVisionLab process remained.
- No XML, PropertyGrid, parameter, layer, routing, Preview/Run, import, or branch/output behavior changed. Product maturity remains 62-66%.
- Next priority remains conditional: preserve a naturally failing external LLM correction round when available. Otherwise change Recipe Manager/LLM or branch comparison only when another fresh real task exposes a concrete gap.

## 2026-07-15 P23 Guided Setup Stale Draft Guard

- Fresh current-source evidence reproduced a concrete safety/clarity defect: the Guided Setup selector showed `Pin gap / edge distance (LineDistance)` while the read-only draft still contained `LLM_MeanBrightnessDrift_Skill` and a `Mean` Step, with no indication that the XML belonged to the previous settings.
- Guided Setup now preserves the previous XML after an intent/input change, marks it stale, disables Import readiness, and changes the draft heading to an amber `설정이 변경되었습니다. Starter XML을 다시 만들어 주세요.` warning. It does not auto-delete the prior draft or auto-generate a replacement.
- A successful explicit `Starter XML 만들기` clears the stale state. The focused smoke proves draft byte preservation, Import blocking, stale-state clearing after explicit regeneration, and unchanged Preview/Run count for both input-value and inspection-intent changes.
- Valid current-source before evidence is `artifacts/p23_guided_setup_stale_draft_20260715/before_current_source/wpf_shell_host_recipe_guided_setup.png`. Valid after warning evidence is `after_current_source_visible/wpf_shell_host_recipe_guided_setup.png`. The first quiet after capture had large unpainted regions and is excluded from visual evidence.
- Full solution build passed with 0 warnings and 0 errors and produced `bin/Debug/OpenVisionLab.exe` and `.dll` at `2026-07-15 21:54:57 KST`. Latest-build actual-EXE `recipe-manager-tabs` passed under `artifacts/p23_guided_setup_stale_draft_20260715/actual_exe`; the report retains all Guided Setup, LLM validation/import, branch comparison, history, and explicit-run contracts.
- Focused current-source smoke passed with `layout=0`, `text=0`, and `internal=0`. Readiness, external-reference, public-sample (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), and `git diff --check` passed. No OpenVisionLab or screenshot-smoke process remained.
- No XML schema, PropertyGrid, tool parameter, layer, route, Preview/Run, or automatic execution behavior changed. Product maturity remains 62-66%.
- Next priority remains conditional: capture a naturally failing external LLM correction round when available. Otherwise perform another current-build operator task and modify Recipe Manager/LLM or branch comparison only when it exposes a new concrete gap.

## 2026-07-15 P24 GPT Blob Particle Count Packet

- Added a self-contained manual GPT packet at `llm_prompt_packets/blob_particle_count` so the user does not need to locate or attach the full authoring guide/tool catalog.
- The packet contains byte-identical copies of the public-safe 572x420 nominal and sparse-negative Blob samples, `COPY_THIS_TO_GPT.txt`, `PASTE_VALIDATION_NG_BACK_TO_GPT.txt`, and one short `README.md` with the exact transfer order.
- The initial prompt constrains the task to `Threshold -> Blob`, preserves explicit Preview/Run behavior, defines `Main -> Particle_Binary -> Particle_Count_Preview`, filters area `200..2000`, and requires a `ResultCount` acceptance band of `8..14`. It requests XML only and forbids unrelated ToolTypes/custom nodes.
- The correction template must be used only after actual OpenVisionLab Validation/Run evidence exists. The user must return the first GPT response unchanged, even if it contains prose, fences, or invalid XML; do not repair it before capture.
- SHA-256 checks proved both packet images are byte-identical to their public sample sources. Prompt contract check passed all 11 required tokens, public-sample policy remained `CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`, and `git diff --check` passed with line-ending warnings only.
- No LLM response or transcript has been claimed yet. The immediate next dependency is the user's unchanged GPT response to `COPY_THIS_TO_GPT.txt`; validate/import/run it only after that response arrives.

## 2026-07-15 P25 GPT Blob Direct-Success Replay

- The user returned one XML-only GPT response for the P24 Blob packet. The unchanged response is preserved under `artifacts/llm_transcripts/raw/20260715_blob_particle_gpt_round1/response.xml`; exact model/version remains unknown, transfer was manual through user-operated ChatGPT, and no API was used.
- The packet prompt was found overwritten with the returned XML in the shared workspace. That exact file content was first preserved as the raw response, then `COPY_THIS_TO_GPT.txt` was restored and copied to raw `prompt.txt`. Final hashes are prompt `FE58976...B1F3` and response `CCB47A4D...3127`.
- Full solution build passed with 0 warnings and 0 errors. Latest current build remains `bin/Debug/OpenVisionLab.exe/.dll`, timestamp `2026-07-15 21:54:57 KST` because no source file changed after P23.
- Recipe Manager `llm-xml-draft-file` passed: XML syntax/deserialization/routing OK, 2 Steps, 0 errors, 0 warnings, Import enabled/completed, selected pipeline `Blob_Particle_Count_Inspection`, and `ImageRun: SKIPPED`.
- Explicit nominal run passed 2/2 Steps with `ResultCount=12` inside `8..14`. Explicit sparse-negative run produced the expected product NG with `ResultCount=3 < 8`; the runner exit code 1 is expected for an NG inspection outcome.
- Raw evidence includes prompt/response, manifest, reports, current-build Recipe Manager screenshot, and nominal/negative result images. This is a real GPT direct-success example with zero correction rounds, not API evidence and not a correction-loop transcript. Do not use the correction prompt or manufacture a failed round.
- Next evidence task should add a different dependency/tool-family challenge, preferably public-safe Matching with a template dependency, rather than repeating another tightly constrained Threshold/Blob prompt.

## 2026-07-15 P26 GPT Matching Die-Pad Packet

- Added the next self-contained manual GPT packet at `llm_prompt_packets/matching_die_pad` for a different tool family and a real template-file dependency.
- The packet contains byte-identical copies of the public-safe nominal image, no-target negative image, and matching template plus `COPY_THIS_TO_GPT.txt`, `PASTE_VALIDATION_NG_BACK_TO_GPT.txt`, and a short `README.md`.
- The initial prompt requires exactly one `Matching` Step, the repository-relative template path in both `TemplatePath` and `PATTERN_PATH`, normalized `SCORE_MIN=0.6`, the product's exact `MAGNIFIATION` spelling, `NUM_MATCH=3`, angle-search settings, and a `ResultCount` acceptance gate of exactly 3.
- SHA-256 checks passed for all three copied images: nominal `EF12511...A68BF`, negative `C01E877...2649`, and template `FE8B979...A5144`. The prompt contract check passed all required Matching, path, score, count, and acceptance tokens.
- Current-build baseline replay passed on the nominal image with `ResultCount=3`, `ScoreMin=80.059`, `ScoreMax=93.074`, and 60.533 ms. The no-target image produced the expected inspection NG with `ResultCount=0 < 3`; evidence is under `artifacts/p26_matching_packet_baseline_20260715`.
- No Matching GPT response has been received or claimed yet. The next dependency is the user's complete unchanged response after attaching all three packet images and pasting `COPY_THIS_TO_GPT.txt` once. Preserve that response before validation; use the correction template only after a natural current-build validation or OK/NG failure.

## 2026-07-15 P27 GPT Matching Round 1 Natural Dependency Failure

- The user returned an XML-only GPT response for P26. The unchanged initial prompt and response are preserved under `artifacts/llm_transcripts/raw/20260715_matching_die_pad_gpt_round1`; exact model/version remains unknown, transfer was manual through user-operated ChatGPT, and no API was used.
- Full solution build passed with 0 warnings and 0 errors. Recipe Manager parsing/deserialization/schema/routing all passed for the one-Step `Matching_DiePad_Inspection`, but dependency review found both `TemplatePath` and `PATTERN_PATH` unresolved, blocked Import, and skipped image execution.
- GPT followed the initial prompt exactly. The failure was caused by the packet incorrectly requiring `docs\samples\...` while Recipe Manager resolves relative dependencies from `AppPathService.StartupPath`, currently `bin\Debug`. The verified current-build relative path is `..\..\docs\samples\public\templates\Matching_DiePad_Synthetic_Template.png` and resolves to the expected template hash.
- Raw round 1 response hash is `479D5D08...ED2FA`; the complete validation report hash is `32BD710A...389B`. The reusable packet path rule was corrected only after the original prompt/response were preserved.
- A ready-to-paste round 2 request with the complete unedited report is `artifacts/llm_transcripts/raw/20260715_matching_die_pad_gpt_round1/round2_prompt.txt`, hash `B38AE291...200CC`. It asks GPT to change only the two dependency paths. No round 2 response exists yet; do not claim correction-loop success before unchanged round 2 validation/import plus nominal PASS and expected no-target NG.

## 2026-07-15 P28 GPT Matching Correction Loop Completed

- The user returned the unchanged round 2 XML from the same GPT conversation. It is preserved as `artifacts/llm_transcripts/raw/20260715_matching_die_pad_gpt_round1/round2_response.xml`, hash `E608A864...66B79`.
- Structured round 1/round 2 comparison found no pipeline-name, Step-count, fixed Step-field, or acceptance-field differences. Exactly two parameter values changed: `TemplatePath` and `PATTERN_PATH` now use the verified StartupPath-relative template path.
- Full solution build passed with 0 warnings and 0 errors. Recipe Manager round 2 validation/import passed with 1 Step, 0 errors/warnings, both dependencies copied, Import completed, and `ImageRun: SKIPPED` during import.
- Explicit nominal execution from the actual `bin\Debug` StartupPath passed with `ResultCount=3`, `ScoreMin=80.059`, `ScoreMax=93.074`, `ScoreAvg=86.444`, and 33.101 ms. Explicit no-target execution produced the expected product NG with `ResultCount=0 < 3` and 25.604 ms.
- This is the first completed real manually transferred GPT correction loop in the current corpus: exact model/version unknown, non-API, one correction round. Preserve the attribution that round 1 failed because the packet supplied the wrong host-relative path; it proves report-driven correction, not independent GPT discovery of the path convention.
- Raw evidence contains local paths and stack traces. Do not publish it directly. The next evidence-governance priority is a separate sanitization/publication audit; keep a minimum package only after confirming hashes, public asset provenance, replay commands, and disclosure wording.

## 2026-07-15 P29 Matching Correction Evidence Publication Audit

- Prepared a seven-file sanitized candidate under `artifacts/llm_transcripts/sanitized/20260715_matching_die_pad_gpt_correction_loop`; no file was placed under `docs/evidence/llm`.
- `prompt_round1.md`, both response XML files, and both result PNGs are byte-identical to raw evidence. `prompt_round2.md` replaces exactly four raw workspace-root occurrences with `<REPO_ROOT>`; reverse substitution reconstructs the raw prompt byte-for-byte.
- Publishable-text scan found no drive/user-home path, credential, email, URL, AppData/Codex attachment path, private/legacy asset, or customer data. Both result PNGs are 572x420 with only `IHDR`/`IDAT`/`IEND` chunks and no text metadata.
- Nominal, negative, and template inputs are registered OpenVisionLab-generated public synthetic assets. Candidate README records disclosure, unknown exact GPT model/version, manual non-API transfer, one correction round, prompt-path failure attribution, hashes, and reproducible commands.
- Fresh current-build replay under `artifacts/p29_matching_correction_publication_review_20260715` reproduced round 1 dependency NG, round 2 import PASS, nominal `ResultCount=3` PASS, no-target `ResultCount=0 < 3` expected NG, and byte-identical result images. Build passed with 0 warnings/errors.
- `docs/OPENVISIONLAB_LLM_MATCHING_CORRECTION_PUBLICATION_REVIEW_20260715.md` initially recorded `CONDITIONAL GO / CURRENTLY HOLD`. That historical gate was superseded by the explicit approval and P30 inclusion below.

## 2026-07-15 P30 Matching Correction Evidence Included

- After the conditional publication decision was presented, the user explicitly approved inclusion of the sanitized copy under `docs`.
- Added exactly seven files under `docs/evidence/llm/20260715_matching_die_pad_gpt_correction_loop`: one disclosure/replay README, two prompts, two unchanged GPT XML responses, and two OpenVisionLab result images. Raw reports, stack traces, runtime paths, and session data remain excluded.
- The included README records approval, unknown exact GPT model/version, manual non-API transfer, one correction round, prompt-path failure attribution, replay commands, public input provenance, and file hashes. Its final SHA-256 is `6C24B58D...7B1924`; the other six files retain their audited candidate hashes.
- Tracked-path replay under `artifacts/p30_matching_correction_tracked_package_20260715` reproduced round 1 dependency NG, round 2 validation/import PASS with both dependencies copied and no image run during import, nominal `ResultCount=3` PASS, and no-target `ResultCount=0 < 3` expected NG. Both result image hashes match the included package exactly.
- Post-inclusion verification passed: full solution build with 0 warnings/errors, readiness, external-reference, public-sample (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), package/hash/privacy checks, runtime cleanup, and `git diff --check` with line-ending warnings only.
- `docs/OPENVISIONLAB_LLM_MATCHING_CORRECTION_PUBLICATION_REVIEW_20260715.md` now records `GO / ADDED TO THE DEV WORKTREE`. The package is not staged, committed, pushed, or copied to the Original repository.
- The intended-workbench maturity estimate remains 62-66%. This closes the first real manually transferred GPT correction-loop evidence item, but does not establish provider/model breadth or broad intent reliability.
- Next evidence priority: select a different public-safe tool family from the current catalog, establish a deterministic OpenVisionLab baseline first, and then prepare one self-contained manual GPT packet. Do not repeat the same Matching path defect merely to manufacture another correction round.

## 2026-07-16 P31 GPT Edge-Fiducial Packet

- Added a six-file self-contained manual GPT packet under `llm_prompt_packets/edge_fiducial_matching`: one README, one first-round prompt, one correction template, and three project-authored public synthetic PNGs. The first round requires only those three PNGs plus the complete contents of `COPY_THIS_TO_GPT.txt`; the full authoring guide and tool catalog are not separate uploads.
- The contract is intentionally narrow: exactly one full-image `EdgeBasedMatching` Step finds the supplied asymmetric L fiducial once in the nominal image and rejects the wrong T-shape image. Both template parameters use the verified Debug StartupPath-relative path `..\..\docs\samples\public\templates\Edge_Fiducial_Synthetic_Template.png`.
- The prompt explicitly separates normalized inputs (`SCORE_MIN=0.70`, `GREEDINESS=0.90`) from the percentage-like `ScoreMax` result gate (`70..100`) and forbids unsupported tools, invented parameters, absolute paths, automatic execution, explanatory prose, and Markdown fences.
- A mechanically derived reference contract passed Recipe Manager validation/import with 1 Step, 0 errors, 0 warnings, both dependencies copied, and `ImageRun: SKIPPED`. Import therefore preserved the explicit Preview/Run contract.
- Explicit current-build execution passed the nominal image at `ResultCount=1` and `ScoreMax=99.598`. The wrong T-shape image produced the expected inspection NG with `ResultCount=0` and `BestScore=61.052`, below `SCORE_MIN=0.70`.
- Packet checks passed with exactly six files, 22 exact Step parameters, no absolute drive path in the prompt, one correction-report placeholder, and byte-identical copied image hashes. Evidence is under `artifacts/p31_edge_based_gpt_packet_20260716`.
- No external GPT response exists yet. Preserve the user's complete first response unchanged before validation. Use `PASTE_VALIDATION_NG_BACK_TO_GPT.txt` only when that actual response produces a natural current-build validation or OK/NG failure.
- The intended-workbench maturity estimate remains 62-66%. This adds a verified EdgeBasedMatching authoring task but does not yet add provider/model breadth or another real transcript.

## 2026-07-16 P32 GPT Edge-Fiducial Direct-Success Replay

- The user returned one XML-only GPT response for the P31 EdgeBasedMatching packet. The unchanged response and exact packet prompt are preserved under `artifacts/llm_transcripts/raw/20260716_edge_fiducial_gpt_round1`; response SHA-256 is `55D63ADF...B3DCD` and prompt SHA-256 is `F51731D2...F1E`.
- Exact GPT model/version was not provided. Transfer was manual through a user-operated ChatGPT conversation, no API was used, and the response was not edited after receipt.
- XML-only format checks passed. The response contains one `EdgeBasedMatching` Step and 22 parameters, and a structured comparison found zero pipeline, fixed-field, parameter, routing, or acceptance differences from the pre-verified P31 reference contract.
- A fresh full solution build passed with 0 warnings and 0 errors. The build was incremental; an independent freshness check found 0 of 598 compile inputs newer than `bin/Debug/OpenVisionLab.exe`.
- Latest-EXE Recipe Manager validation/import passed with 1 Step, 0 errors, 0 warnings, both template dependencies resolved/copied, Import enabled/completed, and `ImageRun: SKIPPED`.
- Explicit nominal execution passed at `ResultCount=1`, `ScoreMin/Max/Avg=99.598`, overlay count 1, and 123.622 ms. Visual review confirmed the L fiducial carries the `#1 99.6` edge overlay.
- Explicit wrong-T execution produced the expected product NG with validation still successful, `ResultCount=0`, `BestScore=61.052`, overlay count 0, and 83.404 ms. The command exit code 1 represents the expected inspection rejection.
- This is a real manually transferred GPT direct-success example with zero correction rounds. Do not send the correction prompt or manufacture a failed round. Raw reports contain local paths; perform a separate privacy/sanitization audit before any `docs/evidence` inclusion decision.
- The intended-workbench maturity estimate remains 62-66%. P32 adds another real tool-family transcript but does not establish provider/model breadth or broad prompt reliability.

## 2026-07-16 P33 Edge-Fiducial Sanitized Candidate

- Created a six-file artifact-only candidate under `artifacts/llm_transcripts/sanitized/20260716_edge_fiducial_gpt_round1_direct_success`: unchanged prompt/XML, unchanged nominal/negative result PNGs, a sanitized manifest, and a privacy review.
- The prompt, response, and two result images are byte-identical to P32 raw evidence. Their SHA-256 values remain `F51731D2...F1E`, `55D63ADF...B3DCD`, `E96C0DAB...23C4C`, and `3A4AC87A...F8ECE`.
- Automated text review found zero absolute drive/user-home/AppData/Codex-attachment paths, emails, URLs, or credential labels. Both 572x420 result PNGs contain only `IHDR`, `IDAT`, and `IEND` chunks with no text or EXIF metadata.
- The nominal, wrong-shape negative, and template hashes match their registered public synthetic assets. No raw report, stack trace, runtime recipe path, generated runtime identifier, or local EXE path was copied into the candidate.
- Latest-EXE candidate replay under `artifacts/p33_edge_fiducial_sanitization_20260716` passed validation/import with no image run, passed nominal at `ResultCount=1` and `ScoreMax=99.598`, and produced expected wrong-T NG at `ResultCount=0` and `BestScore=61.052`. Replayed result PNG hashes are byte-identical to the candidate.
- Candidate manifest SHA-256 is `A41E16D5...D4AA`; privacy review SHA-256 is `34B574B1...8EB`. The directory remains ignored under `artifacts`, is not staged, and is not under `docs/evidence`.
- Dev commit `64d9ade0` was pushed to `origin/codex/public-sample-ux-docs` before this P33 artifact-only candidate and handoff update. P33 itself is not included in that pushed commit. The Original repository was not touched.
- Next gate: obtain a separate explicit repository-inclusion decision before copying this candidate to `docs/evidence`, staging it, or publishing it. If approved, replay the eventual tracked path and verify all copied hashes again.

## 2026-07-16 P34 Edge-Fiducial Evidence Included

- The user explicitly approved repository inclusion of the P33 sanitized EdgeBasedMatching direct-success candidate.
- Added six files under `docs/evidence/llm/20260716_edge_fiducial_gpt_direct_success`: AI-disclosure/replay README, privacy review, unchanged prompt, unchanged GPT XML response, and unchanged nominal/negative OpenVisionLab result PNGs.
- The prompt, response, and two PNGs remain byte-identical to P32 raw evidence and P33 candidate. Final package hashes are recorded in `docs/OPENVISIONLAB_LLM_EDGE_FIDUCIAL_PUBLICATION_REVIEW_20260716.md`.
- Package-wide text scan found 0 absolute drive, user-home, AppData, Codex attachment, email, or URL matches. Both PNGs are 572x420 with only `IHDR`/`IDAT`/`IEND` chunks and no text/EXIF metadata.
- Fresh full build passed with 0 warnings and 0 errors. Latest-EXE tracked-path replay under `artifacts/p34_edge_fiducial_tracked_package_20260716` passed validation/import with `ImageRun: SKIPPED`, passed nominal at `ResultCount=1` and `ScoreMax=99.598`, and produced expected wrong-T NG at `ResultCount=0` and `BestScore=61.052`.
- Replayed nominal and negative result PNG hashes match the package exactly. No OpenVisionLab process or reserved Smoke recipe remains after replay.
- Publication review decision is `GO / ADDED TO THE DEV WORKTREE`. The nine-file P34 scope was committed as `7a7dc51f` (`Add GPT edge fiducial evidence`) and pushed to `origin/codex/public-sample-ux-docs`; local and remote SHAs matched. The Original repository was not touched.
- The intended-workbench maturity estimate remains 62-66%. This increases evidence coverage but does not establish exact-model, provider, or unconstrained authoring reliability.
- Next evidence priority: select a different public-safe tool family only after defining a deterministic nominal/negative baseline. Prefer FeatureMatching if the current implementation can produce stable public synthetic evidence; do not force a transcript around an unstable tool.

## 2026-07-16 P35 FeatureMatching Baseline And GPT Packet

- Audited the existing public-safe `Public_Feature_Card` benchmark before preparing another external LLM task. The registered synthetic nominal, wrong-card negative, template, and one-Step FeatureMatching pipeline already provide the required deterministic baseline.
- A fresh full solution build passed with 0 warnings and 0 errors. The latest EXE is `bin/Debug/OpenVisionLab.exe`, timestamp `2026-07-16 09:39:33 KST`, SHA-256 `53E9223D...B0F85`.
- Five sequential latest-EXE runs per image were stable. Nominal passed 5/5 at `ResultCount=1`, `ScoreMax=96.7`, 210.595-288.316 ms; wrong-card produced expected NG 5/5 at `ResultCount=1`, `ScoreMax=26.7`, 261.463-286.012 ms. Each group produced one unique result-image hash.
- The wrong-card result proves that `ResultCount` alone is unsafe for this FeatureMatching task. The packet and baseline require the final `ScoreMax` gate of `80..100`.
- Added `llm_prompt_packets/feature_matching_card` with exactly six files: README, first-round prompt, correction template, and byte-identical nominal/negative/template PNGs. The prompt requires one full-image `FeatureMatching` Step with `SCORE_MIN=0.85`, `RANSAC_REPROJ_THRESHOLD=4`, and explicit `ScoreMax` acceptance.
- Recipe Manager validation/import resolved and copied both startup-relative template dependencies, imported one Step with 0 errors/warnings, and reported `ImageRun: SKIPPED`. Direct raw replay passed when launched with the real `bin/Debug` startup working directory; launching raw XML from the repository-root working directory cannot resolve the startup-relative path and is retained only as harness-condition evidence.
- Packet audit passed: exactly six files, all three image hashes equal their public sources, all required prompt tokens present, 0 absolute-path matches, one correction-report placeholder, and no external response claimed.
- Detailed evidence is `docs/OPENVISIONLAB_LLM_FEATURE_MATCHING_BASELINE_REVIEW_20260716.md`, `artifacts/p35_feature_matching_sequential_20260716`, and `artifacts/p35_feature_matching_packet_20260716`.
- P35 is not committed or pushed. Its external-response dependency was satisfied by the real P36 direct-success response below. The correction template was not used.

## 2026-07-16 P36 GPT FeatureMatching Direct-Success Replay

- The user returned one XML-only GPT response after using the P35 FeatureMatching packet. The complete visible one-line response is preserved under `artifacts/llm_transcripts/raw/20260716_feature_card_gpt_round1/response.xml`; its SHA-256 is `F06E9259...E245`.
- The exact packet prompt is preserved as `prompt.txt`, SHA-256 `B9F32A8C...BBFE`, and is byte-identical to `llm_prompt_packets/feature_matching_card/COPY_THIS_TO_GPT.txt`.
- Transfer was manual through a user-operated ChatGPT conversation. No API was used. The exact GPT model/version was not supplied and must remain unknown rather than inferred.
- XML-only checks passed: no prose or Markdown fence, one `FeatureMatching` Step, 8 parameters, no duplicate keys, and zero structural differences from the mechanically verified P35 reference contract.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest EXE remained `bin/Debug/OpenVisionLab.exe`, timestamp `2026-07-16 09:39:33 KST`, SHA-256 `53E9223D...B0F85`.
- Latest-EXE Recipe Manager validation/import passed with 1 Step, 0 errors, 0 warnings, Import enabled/completed, and both template dependencies copied. `ImageRun: SKIPPED` confirms import did not execute the inspection.
- Explicit nominal execution passed at `ResultCount=1`, `ScoreMax=96.7`, overlay count 1, and 200.371 ms. Explicit wrong-card execution produced the expected inspection NG at `ResultCount=1`, `ScoreMax=26.7 < 80`, overlay count 1, and 245.15 ms.
- Nominal and negative result-image hashes are byte-identical to the P35 deterministic baseline: `6FD55065...F4914` and `FF663DD3...EC57B`.
- This is a real manually transferred GPT direct-success example with zero correction rounds. The correction prompt was not sent and no failed round was manufactured.
- The raw manifest records prompt/response/report/result hashes, provenance, build evidence, and cleanup. Raw reports contain absolute local paths and a stack trace; do not copy the raw folder directly into `docs/evidence`.
- P36 is artifact evidence plus this handoff update and is not committed or pushed. Its sanitization dependency was completed by the P37 artifact-only candidate below.

## 2026-07-16 P37 FeatureMatching Sanitized Candidate

- Created a six-file artifact-only candidate under `artifacts/llm_transcripts/sanitized/20260716_feature_card_gpt_round1_direct_success`: unchanged prompt/XML, unchanged nominal/negative OpenVisionLab result PNGs, a sanitized manifest, and a privacy review.
- The prompt, response, and two result images are byte-identical to P36 raw evidence. Their SHA-256 values remain `B9F32A8C...BBFE`, `F06E9259...E245`, `6FD55065...F4914`, and `FF663DD3...EC57B`.
- Candidate-wide text review found zero absolute drive, user-home, AppData/Codex attachment, email, URL, or credential-token matches. Prompt/response-specific scans also found zero credential labels and private/legacy asset hints.
- Both result PNGs are 572x420 and contain only `IHDR`, `IDAT`, and `IEND` chunks with no text or EXIF metadata.
- Nominal, wrong-card negative, and template hashes match their registered OpenVisionLab-generated public synthetic assets. No raw report, stack trace, runtime recipe identifier, local EXE path, or Recipe Manager capture was copied into the candidate.
- A fresh full build passed with 0 warnings and 0 errors. Latest-EXE candidate replay under `artifacts/p37_feature_card_sanitization_20260716` passed validation/import with one Step, 0 errors/warnings, two copied dependencies, and `ImageRun: SKIPPED`.
- Candidate replay passed the nominal sample at `ResultCount=1`, `ScoreMax=96.7`, and 216.264 ms. The wrong-card sample produced expected NG at `ResultCount=1`, `ScoreMax=26.7 < 80`, and 224.545 ms.
- Replayed nominal and negative result hashes are byte-identical to the candidate. No OpenVisionLab process or reserved Smoke recipe remained.
- Final sanitized manifest SHA-256 is `C76A9F76...F2636`; privacy review SHA-256 is `3921D49F...2D73`.
- At P37 completion the directory remained ignored under `artifacts` and was not staged. The user supplied the separate inclusion decision at P38; see the following section for the tracked `docs/evidence` package. The P37 artifact itself remains ignored and unchanged.

## 2026-07-16 P38 FeatureMatching Evidence Included In Docs

- The user explicitly approved inclusion of the P37 FeatureMatching direct-success candidate in the Dev worktree. The public six-file package is now `docs/evidence/llm/20260716_feature_card_gpt_direct_success` and the publication review is `docs/OPENVISIONLAB_LLM_FEATURE_CARD_PUBLICATION_REVIEW_20260716.md`.
- The package contains only `README.md`, `privacy_review.md`, `prompt.md`, `response.xml`, `nominal_result.png`, and `negative_result.png`. The prompt, XML, and two result images remain byte-identical to P37 and P36 raw evidence: `B9F32A8C...BBFE`, `F06E9259...E245`, `6FD55065...F4914`, and `FF663DD3...EC57B`.
- Final package README and privacy-review hashes are `68D13992...D5E0` and `2CA546C5...A160`. Package-wide text scans found zero absolute Windows path, user-home, AppData/Codex attachment, email, or URL matches. Prompt/XML payload scans found zero credential labels.
- Both result images are 572x420 and contain `IHDR`, thirteen `IDAT`, and `IEND` chunks only; no text or EXIF metadata exists. The nominal, wrong-card, and template public source hashes match `AA0A0092...3C26`, `3791EC67...703F`, and `75D535A5...A1CD`.
- A fresh full solution build passed with 0 warnings and 0 errors. The latest EXE is `bin/Debug/OpenVisionLab.exe`, timestamp `2026-07-16 09:39:33 KST`, SHA-256 `53E9223D...0F85`.
- Latest-EXE tracked-path replay used the copied `docs/evidence` response XML and is preserved under `artifacts/p38_feature_card_tracked_package_20260716`. Recipe Manager validation/import passed with 1 Step, 0 errors/warnings, 2 copied dependencies, and `ImageRun: SKIPPED`. The nominal sample passed at `ResultCount=1`, `ScoreMax=96.7`, overlay 1, `355.149 ms`; the wrong-card sample produced expected NG at `ResultCount=1`, `ScoreMax=26.7 < 80`, overlay 1, `287.081 ms`, and exit code 1.
- Replayed nominal/negative PNG hashes equal the included package. No OpenVisionLab process or reserved `Smoke_LlmDraft_<12 hex>` directory remained after replay. This inclusion did not stage, commit, or push anything, and did not touch the Original repository.

## 2026-07-16 P39 LLM XML Next-Action Clarity

- Fresh current-source baseline evidence showed the LLM XML tab with three dense lines, including developer-facing execution and result-channel wording, while Template Matching's required fields were only available in the separate Build inspection tab.
- Added the `검사 설정` (`Set up inspection`) command to the LLM XML toolbar. It opens `Advanced > 검사 만들기` so the operator can enter the intent-specific values without searching the Recipe Manager surface.
- Rewrote the visible LLM XML guidance into the operator sequence: choose an inspection intent and required values, create a starter XML or prompt, then validate and import XML. The selected intent/result-channel developer-contract line is no longer rendered in the learner-facing panel.
- Guided Setup action and next-step text now describe create draft -> validate -> import. The former no-auto-run phrase remains only in the private `GuidedSetupStarterXmlNoAutoRunContract` source constant so the regression contract stays enforced without appearing in the UI.
- Fresh current-source before: `artifacts/p39_recipe_llm_ux_before_20260716/llm_xml_initial/wpf_shell_host_llm_dependency_placeholder.png`. Fresh same-summary-state after: `artifacts/p39_recipe_llm_ux_after_20260716_r3/wpf_shell_host_llm_dependency_placeholder.png`. The Build inspection target capture is `artifacts/p39_recipe_llm_ux_after_20260716_r2/guided_setup/wpf_shell_host_recipe_guided_setup.png`.
- Screenshot smoke asserted that `검사 설정` is visible, opens Build inspection, and leaves Preview run count, layer count, preview result, and XML draft unchanged. LLM XML and Guided Setup captures both passed with `layout=0`, `text=0`, and `internal=0`.
- Fresh full solution build passed with 0 warnings and 0 errors. Readiness, external-reference, public-sample, and `git diff --check` validation passed. P39 is uncommitted and unpushed; the Original repository remains untouched.

## 2026-07-16 P40 Run History Baseline Next-Action Clarity

- Fresh current-source Run History evidence showed the no-baseline row using the time-impossible instruction `Run at least one earlier benchmark to enable regression comparison.` The selected saved run already existed, so the wording did not tell the operator what to do next.
- Replaced the no-baseline guidance with: `동일한 검증 세트를 다시 실행한 뒤, 이전 저장 실행을 기준 실행으로 선택하세요.` / `Run the same validation suite again, then select an earlier saved run as the baseline.` It points directly to the existing `Baseline run` selector and does not add an execution, persistence, layer, or routing action.
- Fresh current-source before: `artifacts/p40_run_history_baseline_20260716/wpf_shell_host_recipe_local_validation_set.png`. Fresh after: `artifacts/p40_run_history_after_20260716/wpf_shell_host_recipe_local_validation_set.png`.
- The focused local-validation-set screenshot smoke now checks the no-baseline row when present. It passed with `layout=0`, `text=0`, and `internal=0`. Full solution and screenshot-tool builds passed with 0 warnings and 0 errors. Readiness, external-reference, public-sample, and `git diff --check` validation passed.
- P40 is uncommitted and unpushed; the Original repository remains untouched. Do not repeat a generic Run History wording pass without new current-build evidence.

## 2026-07-16 P41 Feature Matching Guided Setup

- The public Feature Card baseline and the real GPT direct-success package both establish a bounded FeatureMatching contract: full-image search, `SCORE_MIN=0.85`, `RANSAC_REPROJ_THRESHOLD=4`, and `ScoreMax 80..100` acceptance. The wrong-card negative returns `ResultCount=1`, so ResultCount alone is not a valid pass gate.
- Added `Feature Matching` to the existing Build inspection intent catalog and mapped the existing `FeatureMatching` Tool rail action to the same Guided Setup surface. This is not a second Pipeline editor or a new runtime tool.
- The compact operator input block exposes only Feature template path, read-only full-image scope, Ratio minimum, RANSAC pixel tolerance, and ScoreMax minimum. Its starter XML creates one `FeatureMatching` Step (`Main -> Feature_Preview`) with `USE_ROI=false`; it does not validate/import/run or change any layer/routing by itself.
- XML contract validation now requires `ToolType=FeatureMatching`, `SCORE_MIN`, `RANSAC_REPROJ_THRESHOLD`, and a minimum `ScoreMax` acceptance gate. A FeatureMatching draft altered to use only `ResultCount` is rejected with a corrective next action.
- Fresh current-source baseline: `artifacts\p41_feature_guided_setup_before_20260716\wpf_shell_host_recipe_guided_setup.png`. It is the closest reproducible Guided Setup baseline because the Feature Matching input state did not exist before this slice. Fresh after: `artifacts\p41_feature_guided_setup_after_20260716_r2\wpf_shell_host_recipe_guided_setup.png`.
- Fresh solution and screenshot-tool builds passed with 0 warnings and 0 errors. The current-source Guided Setup smoke passed with `layout=0`, `text=0`, and `internal=0`; it verifies missing/invalid inputs, generated XML parameters, the ScoreMax gate, rejection of ResultCount-only acceptance, no Preview/Run, and Korean Feature Matching capture state.
- P41 is uncommitted and unpushed; the Original repository remains untouched. Keep future FeatureMatching Guided Setup changes tied to a deterministic public Good/Bad baseline rather than adding unconstrained fields.

## 2026-07-16 P42 Edge Based Matching Guided Setup

- The included public Edge Fiducial direct-success evidence establishes a bounded EdgeBasedMatching contract: one full-image `EdgeBasedMatching` Step, `SCORE_MIN=0.70`, `NUM_MATCH=1`, `CANNY_LOW/HIGH=30/90`, and `ScoreMax 70..100` acceptance. The wrong T-shape negative has `ResultCount=0` and `BestScore=61.052`, so the workflow keeps count as review evidence and requires ScoreMax for acceptance.
- Added `Edge Based Matching` to the existing Build inspection intent catalog and mapped the existing `EdgeBasedMatching` Tool rail action to it. This remains a Guided Setup starter surface, not a second Pipeline editor or a new runtime tool.
- The compact input block exposes only Edge template path, read-only full-image scope, minimum score, search count, Canny low/high, and ScoreMax minimum. Its Starter XML creates one `EdgeBasedMatching` Step (`Main -> EdgeBased_Preview`) with the public baseline's fixed edge parameters and `USE_ROI=false`; it does not validate/import/run or change any layer/routing by itself.
- XML contract validation now requires `ToolType=EdgeBasedMatching`, `SCORE_MIN`, `NUM_MATCH`, `CANNY_LOW/HIGH`, `USE_ROI=false`, and a minimum `ScoreMax` acceptance gate. A draft altered to use only `ResultCount` is rejected with a corrective next action.
- Fresh closest pre-change baseline: `artifacts\p42_edge_guided_setup_before_20260716\wpf_shell_host_recipe_guided_setup.png`; Edge Based Matching inputs did not exist before this slice. Fresh current-source after: `artifacts\p42_edge_guided_setup_after_20260716\wpf_shell_host_recipe_guided_setup.png`.
- Fresh solution and screenshot-tool builds passed with 0 warnings and 0 errors. The current-source Guided Setup smoke passed with `layout=0`, `text=0`, and `internal=0`; it verifies missing/invalid inputs, Canny ordering, generated XML parameters, the ScoreMax gate, rejection of ResultCount-only acceptance, no Preview/Run, and Korean Edge Based Matching capture state. Readiness, external-reference, and public-sample checks passed (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`).
- P42 is uncommitted and unpushed; the Original repository remains untouched. Keep future EdgeBasedMatching Guided Setup changes tied to the deterministic public fiducial baseline rather than adding opaque matching controls.

## 2026-07-16 P43 Guided Setup Latest-EXE Coverage

- The first P42 latest-EXE `recipe-manager-tabs` replay passed, but its Guided Setup report still named only Pin gap, Blob, Contour, Matching, and Mean. This exposed a verification gap: P41 Feature Matching and P42 Edge Based Matching had current-source coverage but were not exercised by the broad latest-EXE Recipe Manager scenario.
- Extended the existing direct `recipe-manager-tabs` smoke, without changing operator behavior, to select each new intent, verify a missing template blocks Starter XML, create valid public-template Starter XML, require its `ScoreMax` contract, assert the visible intent-specific controls, and keep `NativePreviewRunCount` unchanged.
- The updated latest EXE was built at `2026-07-16 12:56:26 KST` with 0 warnings and 0 errors. Direct `recipe-manager-tabs` passed under `artifacts\p43_guided_setup_direct_exe_20260716`; `report.txt` now records `Pin gap + Blob + Contour + Matching + Feature Matching + Edge Based Matching + Mean Starter XML without Preview/Run`.
- Current latest-EXE Edge Based Matching evidence is `artifacts\p43_guided_setup_direct_exe_20260716\OpenVisionLab_RecipeManager_GuidedSetup_EdgeBasedMatching.png`. It is runtime regression evidence; the P42 before/current-source after pair remains the UI-change comparison record.
- P43 is uncommitted and unpushed; the Original repository remains untouched. Do not extend the direct smoke merely to list every possible control: add a new intent here only when that intent has a dedicated Guided Setup contract.

## 2026-07-16 P44 HSV LLM Authoring Contract And Packet

- The public HSV ColorPatch pair already had a deterministic one-Step pipeline and `MaskPixelRatio` Good/Bad gate, but `HSV` and its aliases were missing from the LLM tool catalog and authoring guide. This made the tool family unavailable for constrained external XML tasks even though the runner supported it.
- Found and fixed one contract mismatch: `VisionPipelineHsvMaskTool` intentionally accepts circular hue ranges such as `HueMin=170`, `HueMax=10` by joining the `170..179` and `0..10` ranges, while generic XML validation rejected the same values. The validator now retains 0..179 bounds but allows hue wrap; saturation/value ordering remains enforced.
- `docs/OPENVISIONLAB_LLM_TOOL_CATALOG.json` now exposes `HSV`, `HsvMask`, `ColorHSV`, and `ColorMask`, their range rules, separate-mask routing, and `MaskPixelCount`/`MaskPixelRatio` metrics. The authoring guide includes a one-Step HSV Color Mask pattern and repair rule. Readiness protects those contracts.
- Fresh latest-EXE `llm-xml-draft-file` validation/import passed for the circular-hue XML with one Step, 0 errors/warnings, and `ImageRun: SKIPPED`; evidence is `artifacts/p44_hsv_llm_contract_after_20260716/direct_validation/report.txt`.
- Explicit current-build replay passed the nominal public image at `MaskPixelRatio=0.058` and produced expected NG for the missing-patch image at `MaskPixelRatio=0.015 < 0.05`. The packet is `llm_prompt_packets/hsv_color_patch`; its two PNGs are byte-identical to public synthetic sources. See `docs/OPENVISIONLAB_LLM_HSV_COLOR_PATCH_BASELINE_REVIEW_20260716.md`.
- No real GPT response has been received or claimed. The next dependency is the user's complete unchanged first response after attaching the two packet images and pasting `COPY_THIS_TO_GPT.txt`. P44 is uncommitted and unpushed; the Original repository remains untouched.

## 2026-07-16 P45 LLM Intent Smoke Advanced-Review State

- A fresh latest-EXE LLM Intent smoke initially showed the outer recipe library and Create/Duplicate/Rename/Delete controls. Source review proved that the normal `OpenRecipeLlmXmlReview` path enables Advanced Review before selecting the LLM XML tab, while this smoke selected only the tab. The capture was therefore a smoke-state evidence defect, not a confirmed operator UI defect.
- Updated `OpenVisionLabDirectSmokeRunner.cs` to enable `recipeAdvancedReviewToggle` before opening `tabRecipeLlmXml`, then assert that `HostRecipeManagerLibraryPane`, `HostRecipeNameEditor`, and `HostRecipeManagerCommandStrip` are not visible.
- Fresh latest-EXE evidence is `artifacts/p45_recipe_manager_llm_audit_after_verified_current_exe_20260716/OpenVisionLab_RecipeManager_LlmIntentSkills_TemplateMatching.png`. It shows the full-width technical review state with only the explicit return-to-summary action; no library, search, or basic lifecycle controls are present.
- The focused direct smoke passed under the same artifact directory. Its report records the intent-skill visibility cases, the intentional PinGap/Contour mismatch block, and `PreviewRunCountUnchanged: 0`.
- Fresh full solution build passed with 0 warnings and 0 errors. Readiness, external-reference, public-sample (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), and `git diff --check` passed; the latter emitted existing line-ending warnings only.
- P45 changes test evidence only. Do not reopen a Recipe Manager layout change unless a real current-build operator route exposes a concrete clipping, overlap, or next-action problem.

## 2026-07-16 P46-P47 LLM Draft Action Clarity And Guided Setup Intent Width

- Fresh latest-EXE evidence showed the learner-facing `Starter XML` action remaining in an otherwise Korean LLM XML and Guided Setup workflow. The action, its surrounding summary/draft wording, and template-missing status messages now use the operator-facing term `초안 XML 만들기` / `Create draft XML`. Internal command names and the private no-auto-run contract keep their existing `StarterXml` identifiers.
- P46 before evidence is `artifacts/p45_recipe_manager_llm_audit_after_verified_current_exe_20260716/OpenVisionLab_RecipeManager_LlmIntentSkills_TemplateMatching.png`; fresh after evidence is `artifacts/p46_llm_draft_labels_after_current_exe_20260716/OpenVisionLab_RecipeManager_LlmIntentSkills_TemplateMatching.png`. The latest-EXE intent-skill smoke passed and retained `PreviewRunCountUnchanged: 0`.
- The P46 Guided Setup capture exposed one actual clipped value: `Pin gap / edge distance (LineDistance)` did not fit in the 240px inspection-intent selector. P47 reduced the adjacent label column from 130px to 120px and widened only that selector column to 300px. The selected value is fully visible while the sample field remains available.
- P47 before evidence is `artifacts/p46_recipe_manager_tabs_after_current_exe_20260716/OpenVisionLab_RecipeManager_GuidedSetup.png`; fresh after evidence is `artifacts/p47_guided_setup_intent_width_after_current_exe_20260716/OpenVisionLab_RecipeManager_GuidedSetup.png`.
- Fresh latest-EXE `recipe-manager-tabs` passed under the P47 artifact directory. It preserved explicit sample loading, no automatic Preview/Run from Guided Setup starter generation, advanced-review full-width behavior, Feature/Edge Guided Setup coverage, and existing branch/output review checks.
- The first Readiness pass correctly caught its stale expected status-message token after P46. The check now expects `Created Guided setup draft XML. Preview/Run was not executed`; the behavioral no-auto-run invariant remains unchanged and Readiness passes again.
- Final P46/P47 verification passed: full solution build with 0 warnings/errors, Readiness, external-reference, public-sample (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), and `git diff --check` after the document update. No commit, push, or Original repository change occurred.

## 2026-07-16 P48 GPT HSV Color-Patch Direct-Success Replay

- The user returned one XML-only GPT response for the self-contained P44 HSV ColorPatch packet. The unchanged response and packet prompt are preserved under `artifacts/llm_transcripts/raw/20260716_hsv_color_patch_gpt_round1`; response SHA-256 is `1B85CA1D...8587` and prompt SHA-256 is `B77AF4DE...B7ACA`.
- Exact GPT model/version was not supplied. Transfer was manual through a user-operated ChatGPT conversation, API evidence was not supplied, and the full chat export was not captured. Do not infer any of those missing facts.
- Latest-EXE Recipe Manager validation/import passed with one HSV Step, 0 errors, 0 warnings, Import enabled/completed, and `ImageRun: SKIPPED`. Import remained separate from explicit image execution.
- The generic LLM image smoke was corrected to load source images with `ImreadModes.Unchanged` rather than grayscale, because grayscale destroys HSV/color input data. It also now accepts `--expect-run-success false` so expected product NG samples can be represented as passing verification cases. The initial grayscale result is not evidence for the GPT XML.
- After the correction and a fresh full build, explicit nominal replay passed at `MaskPixelRatio=0.058` within the `0.05..0.07` gate with 3 source channels. The missing-patch image produced the expected NG at `MaskPixelRatio=0.015 < 0.05`, also with 3 source channels. Both latest-EXE verification commands returned exit code 0 because their expected results were declared explicitly.
- This is a real manually transferred GPT direct-success example with zero correction rounds. Do not send the correction prompt or manufacture a failed round. The raw manifest records provenance, report hashes, and the pre-fix harness exclusion; raw reports contain absolute local paths and must not be copied into `docs/evidence` without a separate sanitization and explicit inclusion decision.
- P48 is not committed or pushed. The Original repository remains untouched.

## 2026-07-16 P49 HSV Color-Patch Evidence Included In Docs

- The user explicitly approved inclusion of the P48 HSV ColorPatch direct-success evidence in the Dev worktree. The public six-file package is now `docs/evidence/llm/20260716_hsv_color_patch_gpt_direct_success` and the publication review is `docs/OPENVISIONLAB_LLM_HSV_COLOR_PATCH_PUBLICATION_REVIEW_20260716.md`.
- The package contains only `README.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged GPT `response.xml`, and unchanged nominal/negative OpenVisionLab result PNGs. The four immutable evidence hashes are `B77AF4DE...B7ACA`, `1B85CA1D...8587`, `C3D021DF...B5A1`, and `06C6D742...9C34`.
- Payload-only text scans found zero absolute Windows/user-home/AppData/Codex attachment paths, emails, URLs, credential-token labels, and known private/legacy asset hints. Both result PNGs are 572x420 with only `IHDR`/`IDAT`/`IEND` chunks and no text/EXIF metadata.
- Fresh latest-EXE tracked-path replay under `artifacts/p49_hsv_tracked_package_20260716` passed validation/import with one Step, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`. The nominal passed at `MaskPixelRatio=0.058`; the missing-patch image produced expected NG at `0.015 < 0.05`. Both source images retained 3 channels, and replay result images are byte-identical to the included package.
- The user-visible package explicitly discloses manual GPT transfer, missing exact model/version/API evidence, zero correction rounds, and the constrained public-synthetic scope. It does not claim broad color robustness or independent tool selection.
- P49 is not committed or pushed. The Original repository remains untouched.

## 2026-07-16 P50 Mean Brightness GPT Packet

- Selected `Mean` as the next bounded external-authoring task because it is not represented by the current real GPT evidence set, has a one-Step operator-facing workflow, and has a deterministic public Good/Dark pair. It teaches full-image brightness judgement without introducing a new tool or expanding platform scope.
- The current public baseline `docs/samples/public/Public_Mean_BrightnessDrift.pipeline.xml` passed the nominal at `MeanValueAvg=201.5` and produced expected dark NG at `117.5 < 185`; both inputs retained 3 channels. The two latest-EXE baseline commands returned exit code 0 through explicit expected-result declarations.
- Added `llm_prompt_packets/mean_brightness` with exactly five files: README, first-round prompt, correction template, and two byte-identical public synthetic PNGs. The packet requires exactly one full-image `Mean` Step, `MEAN_TYPES=Mean`, disabled internal threshold/adaptive/invert/multi-ROI flags, and a `MeanValueAvg` gate of `185..220`.
- Its mechanically derived reference XML passed latest-EXE validation/import with one Step, 0 errors/warnings, and `ImageRun: SKIPPED`; explicit nominal passed at `201.5`, and dark negative produced expected NG at `117.5 < 185`. Packet checks passed: 5 files, all required prompt tokens, no absolute Windows path, one correction placeholder, and the expected one-Step XML structure.
- No external GPT response exists for P50 yet. Send the two packet images and complete `COPY_THIS_TO_GPT.txt` in one new GPT conversation, then preserve the full first response unchanged. Use `PASTE_VALIDATION_NG_BACK_TO_GPT.txt` only after a natural current-build validation or Good/NG failure.

## 2026-07-16 P51 GPT Mean Brightness Direct-Success Replay

- The user returned one XML-only GPT response for the P50 Mean Brightness packet. The unchanged response and exact packet prompt are preserved under `artifacts/llm_transcripts/raw/20260716_mean_brightness_gpt_round1`; response SHA-256 is `19C2EA46...E191` and prompt SHA-256 is `87CF9840...65CC`.
- Exact GPT model/version was not supplied. Transfer was manual through a user-operated ChatGPT conversation, API evidence was not supplied, and the full chat export was not captured. Do not infer any of those missing facts.
- A structured comparison found zero differences across all 21 pipeline, Step, parameter, and acceptance fields against the mechanically verified P50 reference contract.
- Fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` Recipe Manager validation/import passed with one Mean Step, 0 errors, 0 warnings, Import enabled/completed, and `ImageRun: SKIPPED`.
- Explicit nominal execution passed at `MeanValueAvg=201.5`, `ResultCount=1`, source channels 3, and 11.5 ms. Explicit dark execution produced expected product NG at `MeanValueAvg=117.5 < 185`, `ResultCount=1`, source channels 3, and 5.821 ms. Both smoke commands returned exit code 0 because their expected outcomes were declared explicitly.
- This is a real manually transferred GPT direct-success example with zero correction rounds. Do not send the correction prompt or manufacture a failed round. The raw manifest records provenance, contract comparison, report hashes, and result-image hashes. Raw reports contain absolute local paths; do not copy the raw folder directly into `docs/evidence`.
- P51 is not committed or pushed. The Original repository remains untouched. P52 records the separate explicit publication approval and sanitized package.

## 2026-07-16 P52 GPT Mean Brightness Evidence Publication

- The user explicitly approved publication of the P51 Mean direct-success evidence into the Dev worktree. The six-file package is `docs/evidence/llm/20260716_mean_brightness_gpt_direct_success`; the companion decision record is `docs/OPENVISIONLAB_LLM_MEAN_BRIGHTNESS_PUBLICATION_REVIEW_20260716.md`.
- The package contains only `README.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged `response.xml`, and two OpenVisionLab-generated result PNGs. Raw manifests, validation/run reports, local paths, runtime identifiers, stack traces, session data, and Recipe Manager captures remain outside the package.
- The prompt/XML payload scan found zero absolute Windows, user-home, application-data, Codex attachment, email, URL, credential-label, or private/legacy asset-hint matches. Both included 572x420 PNGs contain only `IHDR`, `IDAT`, and `IEND` chunks and are byte-identical to the raw evidence results.
- After a fresh 0-warning/0-error solution build, the latest actual `bin/Debug/OpenVisionLab.exe` replayed the packaged `response.xml`: validation/import PASS with one Step, 0 errors, 0 warnings, and `ImageRun: SKIPPED`; nominal PASS at `MeanValueAvg=201.5`; dark input expected product NG at `MeanValueAvg=117.5 < 185`. Both image inputs retained 3 channels and both smoke commands returned exit code 0 through their explicit expected-result declarations.
- The P52 tracked replay is `artifacts/p52_mean_tracked_package_20260716`; its packaged result PNGs match the actual replay byte-for-byte. The exact GPT model/version, API evidence, and full conversation export remain unavailable; do not infer them. This remains one constrained direct-success example with zero correction rounds.
- P52 is not committed or pushed. The Original repository remains untouched.

## 2026-07-16 P53 Morphology Cleanup GPT Packet

- Selected the first bounded multi-Step external-authoring candidate after the direct single-tool examples: `Threshold -> Morphology(Open) -> Contour`. It exercises sequential layer routing without introducing a branch, template dependency, new tool, or product-scope expansion.
- The existing public baseline `docs/samples/public/Public_Morphology_Cleanup.pipeline.xml` passed fresh latest-EXE validation/import with 3 Steps, 0 errors, 0 warnings, and no external dependencies. Explicit nominal passed `ResultCount=4` with 3 -> 1 -> 1 Step channel transitions and four overlays. Explicit missing-target input produced expected product NG at `ResultCount=2 < 4` with two overlays. Both expected-result commands returned exit code 0.
- Added `llm_prompt_packets/morphology_cleanup` with five files: README, first-round prompt, correction template, and two byte-identical project-authored public synthetic PNGs. The first-round prompt locks Step names, ToolTypes, layer route `Main -> Morphology_Binary -> Morphology_Clean -> Morphology_Cleanup_Preview`, all verified parameters, and a final `ResultCount=4..4` gate.
- Packet checks passed: 5 files, all required tool/route/parameter/acceptance tokens, exactly one correction-report placeholder, no absolute Windows path in the first-round prompt, and both images byte-identical to their public inputs. The baseline review is `docs/OPENVISIONLAB_LLM_MORPHOLOGY_CLEANUP_BASELINE_REVIEW_20260716.md`; latest-EXE evidence is `artifacts/p53_morphology_baseline_20260716`.
- No external GPT response or correction loop exists for P53 yet. The user should send the two packet PNGs and the full first-round prompt to GPT, then return the complete first response unchanged. Use the correction template only after an actual current-build validation or Good/NG failure.
- P53 is not committed or pushed. The Original repository remains untouched.

## 2026-07-16 P54 Recipe Manager Current-EXE Recheck And Pin-Gap Smoke Diagnostics

- A fresh current-EXE `recipe-manager-tabs` run initially stopped in `VerifyPinGapUnitSampleContract` with `MM Good=False, PX Good=True, MM Bad=False, PX Bad=False`. The public one-Step `Public_Line_Pins_Distance.pipeline.xml` replayed independently in the same current build as expected: nominal `DistanceMmAvg=0.222`, expected wide-pin NG `0.106 < 0.2`.
- A no-source-change diagnostic compared the exact Intent Skill-style one-sample route at `SAMPLING_STEP=16` and the public tuned value `6`. Both variants passed nominal and rejected the wide-pin input; the `16` variant measured `DistanceMmAvg=0.224` and the `6` variant `0.222`. This ruled out the sampling-step difference as the observed failure cause.
- Three further independent current-EXE `recipe-manager-tabs` processes passed the same Pin-gap unit contract at `DistanceMmAvg=0.224`, `DistancePxAvg=37.263`, with Good OK in both mm/px modes and Bad NG in both modes. The original failure was not reproduced; do not hide a future recurrence by adding retry-to-pass behavior.
- `OpenVisionLabDirectSmokeRunner.VerifyPinGapUnitSampleContract` now saves the four Pin-gap result PNGs before outcome assertions and includes each run's success flag plus `DistanceMmAvg`, `DistancePxAvg`, `DistanceMmRange`, and `DistancePxRange` in a future failure message. This is smoke diagnostic evidence only; no tool parameter, layer route, Preview/Run, Recipe Manager workflow, or operator-facing UI behavior changed.
- Fresh build after the diagnostic change passed with 0 warnings and 0 errors. Latest `bin/Debug/OpenVisionLab.exe` `recipe-manager-tabs` passed under `artifacts/p54_pin_gap_diagnostic_after_20260716`; the current-EXE Recipe Manager summary and LLM XML views were visually reviewed without a new clipping, overlap, or unclear-next-action defect that justified a UI edit. The one observed limitation remains the un-reproduced first failure, now diagnosable if it recurs.
- P54 is not committed or pushed. The Original repository remains untouched.

## 2026-07-16 P55 Filter Denoise GPT Packet

- Selected `Filter(MedianBlur) -> Threshold -> Contour` as the next independent public-safe authoring candidate. Filter is not covered by the existing real GPT evidence packages, while the existing public Good/NG pair provides a deterministic downstream `ResultCount` decision instead of treating preprocessing output alone as an inspection result.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual-EXE baseline import of `docs/samples/public/Public_Filter_Denoise.pipeline.xml` passed with 3 Steps, 0 errors, 0 warnings, no dependencies, and `ImageRun: SKIPPED`.
- Explicit current-EXE nominal execution passed at `ResultCount=4` with Step channel transitions `3 -> 3 -> 1`, four overlays, and `29.848 ms`. The missing-target image produced expected product NG at `ResultCount=2 < 4`, two overlays, and `27.179 ms`. Both commands used explicit expected-result declarations and returned exit code 0.
- Added the five-file self-contained packet `llm_prompt_packets/filter_denoise`: README, first-round prompt, correction template, and byte-identical copies of the registered Filter public Good/NG assets. The prompt locks `FilterType=MedianBlur`, `MedianKernelSize=5`, `BorderType=Reflect101`, the sequential route `Main -> Filter_Denoised -> Filter_Denoise_Binary -> Filter_Denoise_Preview`, and a final `ResultCount=4..4` gate.
- The exact mechanically derived packet reference XML passed latest-EXE validation/import without image execution, nominal `ResultCount=4`, and expected missing-target NG `ResultCount=2 < 4`; evidence is `artifacts/p55_filter_denoise_packet_20260716`. Packet audit passed: 5 files, copied image hashes match public sources, required prompt tokens and one correction placeholder are present, no private/path token is present, and the reference XML matches the required 3-Step route/gate.
- Updated the LLM tool catalog so Filter lists `MedianKernelSize`, bilateral parameters, and the correct per-filter validation hints. The authoring guide now explains the `MedianBlur -> Threshold -> Contour` route and why the acceptance gate belongs on Contour.
- No external GPT response or correction loop exists for P55 yet. Send the two packet images and the complete first-round prompt in one new GPT conversation, preserve the complete first XML response unchanged, and use the correction template only after a natural current-build validation or Good/NG failure.
- P55 is not committed or pushed. The Original repository remains untouched.

## 2026-07-16 P56 GPT Filter Denoise Direct-Success Replay

- The user returned one XML-only response for the P55 Filter Denoise packet. The visible response was preserved unchanged under `artifacts/llm_transcripts/raw/20260716_filter_denoise_gpt_round1/response.xml`; response SHA-256 is `4E684E91...D6855E`. The copied packet prompt is `prompt.txt`, SHA-256 `654BC48D...07B6C1`.
- Transfer was manual through a user-operated GPT/ChatGPT conversation. Exact model/version, API evidence, and a full chat export were not supplied and must remain unknown rather than inferred.
- XML-only boundary checks passed. A structured comparison found zero differences across the pipeline name, three Steps, routes, parameters, and acceptance fields against the mechanically verified P55 reference contract.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` validation/import passed with 3 Steps, 0 errors, 0 warnings, no dependencies, Import enabled/completed, and `ImageRun: SKIPPED`.
- Explicit nominal execution passed at `ResultCount=4`, four overlays, channel transitions `3 -> 3 -> 1`, and `32.762 ms`. Explicit missing-target execution produced the expected product NG at `ResultCount=2 < 4`, two overlays, the same channel transitions, and `43.106 ms`. Both smoke commands returned exit code 0 because their expected outcomes were declared explicitly.
- This is a real manually transferred GPT direct-success example with zero correction rounds. Do not send the correction prompt or manufacture a failed round. The raw manifest records provenance, report hashes, and result-image hashes. Raw reports contain absolute local paths; do not copy the raw folder directly into `docs/evidence`.
- P56 is raw evidence plus this handoff update only. It is not committed or pushed, and the Original repository remains untouched. The next gate is a sanitized artifact-only candidate and a separate explicit repository-inclusion decision before any tracked `docs/evidence` package.

## 2026-07-16 P57 GPT Filter Denoise Sanitized Candidate

- Created the ignored six-file artifact-only candidate at `artifacts/llm_transcripts/sanitized/20260716_filter_denoise_gpt_round1_direct_success`: `manifest.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged `response.xml`, and current-EXE nominal/negative result PNGs. It is not a tracked `docs/evidence` package.
- The copied prompt, response, nominal PNG, and negative PNG are byte-identical to P56 raw/current-EXE evidence: `654BC48D...07B6C1`, `4E684E91...D6855E`, `F19B25B7...E8D6C`, and `BFCD2853...19DC2` respectively.
- Payload and sanitized-manifest scans found zero absolute Windows/user-home/AppData/Codex attachment paths, URLs, emails, credential-token labels, and known private/legacy asset hints. The two PNGs are 572x420 with only `IHDR`, `IDAT`, and `IEND` chunks. The only full-folder keyword hits are explanatory labels in `privacy_review.md`, not evidence payload content.
- The candidate records only repository-relative public synthetic inputs and replay outcomes. It deliberately excludes the raw manifest and raw validation/run reports because they contain local evidence and EXE paths. Current-EXE replay details remain P56 evidence: import PASS with `ImageRun: SKIPPED`, nominal `ResultCount=4`, and expected missing-target NG `ResultCount=2 < 4`.
- P57 does not grant repository inclusion. Do not copy this candidate to `docs/evidence`, stage it, commit it, or publish it without a separate explicit user decision. No original-repository operation, commit, or push occurred.

## 2026-07-16 P58 GPT Filter Denoise Evidence Included In Docs

- The user approved inclusion of the P57 Filter Denoise direct-success evidence in the Dev worktree. The public six-file package is now `docs/evidence/llm/20260716_filter_denoise_gpt_direct_success`; the inclusion decision is `docs/OPENVISIONLAB_LLM_FILTER_DENOISE_PUBLICATION_REVIEW_20260716.md`.
- The package contains only `README.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged `response.xml`, and OpenVisionLab-generated nominal/negative result PNGs. Raw manifests, validation/run reports, local paths, runtime identifiers, stack traces, and session data remain outside the package.
- Full package text scanning found zero disallowed local-path, contact, URL, credential-marker, or private/legacy asset-hint matches. Both result PNGs are 572x420 with only `IHDR`/`IDAT`/`IEND` chunks and are byte-identical to raw P56 evidence.
- After a fresh 0-warning/0-error solution build, latest actual `bin/Debug/OpenVisionLab.exe` replayed the tracked `response.xml`: validation/import PASS with 3 Steps, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`; nominal PASS at `ResultCount=4`; missing-target input expected product NG at `ResultCount=2 < 4`. Both replay input images retained `3 -> 3 -> 1` channel transitions and replay result PNGs match the included package byte-for-byte.
- The P58 tracked replay is `artifacts/p58_filter_denoise_tracked_package_20260716`. Exact GPT model/version, API evidence, and full conversation export remain unavailable; this is one constrained direct-success case with zero correction rounds. P58 is not committed or pushed, and the Original repository remains untouched.

## 2026-07-16 P59 Morphology Cleanup GPT And Gemini Direct-Success Replay

- The user returned two XML-only responses for the P53 Morphology Cleanup packet and identified the first as GPT and the second as Gemini. The visible XMLs are separately preserved under `artifacts/llm_transcripts/raw/20260716_morphology_cleanup_gpt_round1` and `artifacts/llm_transcripts/raw/20260716_morphology_cleanup_gemini_round1`. Exact model/version, API evidence, and full conversation exports were not supplied and must remain unknown rather than inferred.
- XML-only boundary checks passed for both. The required three-Step `Threshold -> Morphology -> Contour` contract, parameter sets, sequential layer route, and final `ResultCount=4..4` gate had zero differences for both providers. Their free `VisionPipeline/Name` values differ (`Morphology_Cleanup_Inspection` versus `Morphology Cleanup`) because the packet did not constrain that label.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` independently imported both drafts with 3 Steps, 0 errors, 0 warnings, no dependencies, and `ImageRun: SKIPPED`.
- GPT nominal passed `ResultCount=4` in 39.171 ms; its missing-target input produced expected NG `ResultCount=2 < 4` in 24.75 ms. Gemini nominal passed `ResultCount=4` in 33.919 ms; its missing-target input produced expected NG `ResultCount=2 < 4` in 26.502 ms. Both paths retained `3 -> 1 -> 1` channel transitions.
- The provider-specific reports remain separate. Their Good result PNGs are byte-identical, as are their expected-NG PNGs, because the required executable contract is identical. Do not treat this as a provider-quality comparison or as evidence of independent algorithm selection.
- Both are real manually transferred direct-success examples with zero correction rounds. Do not send the correction prompt or manufacture failed rounds. Raw reports contain local paths and must not be copied into `docs/evidence` without separate sanitization and explicit inclusion approval.

## 2026-07-16 P60 Morphology Cleanup Sanitized Candidates

- Created separate ignored six-file artifact-only candidates for GPT and Gemini under `artifacts/llm_transcripts/sanitized/20260716_morphology_cleanup_gpt_round1_direct_success` and `artifacts/llm_transcripts/sanitized/20260716_morphology_cleanup_gemini_round1_direct_success`.
- Each candidate has a manifest, privacy review, unchanged prompt/response, and nominal/negative OpenVisionLab result PNGs. The four immutable evidence files match their raw/current-EXE sources byte-for-byte. Text scans found zero disallowed local-path, contact, URL, credential-marker, or private/legacy-asset matches; each PNG is 572x420 with only `IHDR`/`IDAT`/`IEND` chunks.
- P60 does not grant repository inclusion. Keep both candidates ignored and do not copy either to `docs/evidence`, stage, commit, or publish without a separate explicit user decision. The Original repository remains untouched.

## 2026-07-16 P61 Morphology Cleanup GPT And Gemini Evidence Included In Docs

- The user approved inclusion of both P60 Morphology Cleanup direct-success candidates in the Dev worktree. The public packages are `docs/evidence/llm/20260716_morphology_cleanup_gpt_direct_success` and `docs/evidence/llm/20260716_morphology_cleanup_gemini_direct_success`; the companion decisions are `docs/OPENVISIONLAB_LLM_MORPHOLOGY_CLEANUP_GPT_PUBLICATION_REVIEW_20260716.md` and `docs/OPENVISIONLAB_LLM_MORPHOLOGY_CLEANUP_GEMINI_PUBLICATION_REVIEW_20260716.md`.
- Each package contains only `README.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged provider response XML, and OpenVisionLab-generated nominal/negative result PNGs. Raw manifests, validation/run reports, local paths, runtime identifiers, stack traces, and session data remain outside both packages.
- Both six-file packages passed complete text/path/privacy scans, PNG metadata checks, file-set checks, and publication-review hash audits. Their result PNGs are 572x420 `IHDR`/`IDAT`/`IEND` files and match raw evidence byte-for-byte.
- After a fresh 0-warning/0-error solution build, latest actual `bin/Debug/OpenVisionLab.exe` replayed both tracked package XMLs: each import passed with 3 Steps, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`; each nominal passed at `ResultCount=4`; each missing-target input produced expected product NG at `ResultCount=2 < 4`. GPT replay elapsed values were 27.722/26.648 ms and Gemini replay values were 25.567/27.727 ms for Good/NG respectively. Included result PNGs match replay byte-for-byte.
- The combined tracked replay is `artifacts/p61_morphology_gpt_gemini_tracked_packages_20260716`. Exact provider model/version, API evidence, and full conversation exports remain unavailable. The matching outputs prove only constrained contract adherence, not a provider benchmark or general authoring reliability. P61 is not committed or pushed, and the Original repository remains untouched.

## 2026-07-16 P62 Arithmetic Invert GPT Packet

- Selected the distinct public-safe `Arithmetic(Bitwise_NOT) -> Mean` workflow after the existing public baseline showed a deterministic two-Step Good/Bright-NG split. This adds Arithmetic evidence coverage without introducing a new tool, template dependency, camera path, or platform feature.
- Fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` imported `Public_Arithmetic_Invert.pipeline.xml` with 2 Steps, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`; the nominal image passed at `MeanValueAvg=208`, and the bright-input image produced expected NG at `MeanValueAvg=76.7 < 190`.
- Added the self-contained five-file packet `llm_prompt_packets/arithmetic_invert`: README, first-round prompt, correction template, and byte-identical public Good/Bright-NG image copies. The packet locks the sequential route `Main -> Arithmetic_Invert_Result -> Arithmetic_Invert_Mean`, unary `Bitwise_NOT` without `InputLayerB`, and the `190..230` MeanValueAvg gate.
- The exact packet reference XML replayed in the same latest EXE: import passed with 2 Steps and 0 errors/warnings, nominal passed at `208`, and Bright-NG produced expected product NG at `76.7 < 190`; all three commands returned exit code 0. Evidence is `artifacts/p62_arithmetic_invert_baseline_20260716`.
- No external GPT response or correction loop exists for P62. The next external gate is the user's unchanged first response after attaching the two packet images and sending the packet prompt. Do not invent or publish a response, failure, or correction round before that input exists. P62 is uncommitted and unpushed; the Original repository remains untouched.

## 2026-07-16 P63 Arithmetic Invert GPT Direct-Success Replay

- The user returned one XML-only response identified as GPT for the P62 Arithmetic packet. The manually transferred XML and copied packet prompt are preserved under `artifacts/llm_transcripts/raw/20260716_arithmetic_invert_gpt_round1`. Exact model/version, API evidence, and a full provider-chat export were not supplied and must remain unknown rather than inferred.
- XML-only boundary validation passed. The user-supplied response matched the mechanically verified two-Step Arithmetic reference contract across 34 structured fields with zero differences.
- After a fresh 0-warning/0-error solution build, latest actual `bin/Debug/OpenVisionLab.exe` validation/import passed with 2 Steps, 0 errors, 0 warnings, no dependencies, and `ImageRun: SKIPPED`.
- Explicit nominal execution passed at `MeanValueAvg=208` in 16.243 ms. Explicit Bright-NG execution produced expected product NG at `MeanValueAvg=76.7 < 190` in 10.07 ms. Both smoke commands returned exit code 0 because their expected outcomes were declared explicitly.
- This is a real manually transferred direct-success example with zero correction rounds. Do not send the correction prompt or manufacture a failed round. Raw reports can contain local evidence locations and must not be copied into `docs/evidence` without a separate sanitization and explicit inclusion decision.
- P63 is raw evidence plus handoff updates only. It is not committed or pushed, and the Original repository remains untouched.

## 2026-07-16 P64 Arithmetic Invert GPT Sanitized Candidate

- Created the ignored six-file artifact-only candidate at `artifacts/llm_transcripts/sanitized/20260716_arithmetic_invert_gpt_round1_direct_success`: `manifest.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged `response.xml`, and current-EXE nominal/negative result PNGs. It is not a tracked `docs/evidence` package.
- The prompt, response, nominal PNG, and Bright-NG PNG are byte-identical to P63 raw/current-EXE evidence. Payload scans found zero absolute Windows, user-home, application-data, Codex attachment, URL, email, credential-label, or private/legacy asset-hint matches.
- Both result PNGs are 572x420 and contain only `IHDR`, `IDAT`, and `IEND` chunk types. The candidate excludes raw reports and local evidence locations; its manifest records only repository-relative public inputs, hashes, outcomes, and classification limits.
- P64 does not grant repository inclusion. Do not copy it to `docs/evidence`, stage it, commit it, or publish it without a separate explicit user decision. The Original repository remains untouched.

## 2026-07-16 P65 Arithmetic Invert GPT Evidence Included In Docs

- The user explicitly approved inclusion of the P64 Arithmetic direct-success candidate in the Dev worktree. The public six-file package is `docs/evidence/llm/20260716_arithmetic_invert_gpt_direct_success`; the companion decision record is `docs/OPENVISIONLAB_LLM_ARITHMETIC_INVERT_PUBLICATION_REVIEW_20260716.md`.
- The package contains only `README.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged `response.xml`, and OpenVisionLab-generated nominal/negative result PNGs. Raw manifests, validation/run reports, local paths, runtime identifiers, stack traces, and session data remain outside the package.
- Package-wide file-set, text/privacy, public-asset, PNG metadata, and immutable-hash checks passed. Both 572x420 result PNGs contain only `IHDR`/`IDAT`/`IEND` chunk types and match P63 raw evidence plus the P65 tracked replay byte-for-byte.
- After a fresh 0-warning/0-error solution build, latest actual `bin/Debug/OpenVisionLab.exe` replayed the tracked `response.xml`: validation/import passed with 2 Steps, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`; nominal passed at `MeanValueAvg=208` in 15.558 ms; Bright-NG produced expected product NG at `MeanValueAvg=76.7 < 190` in 5.929 ms. Both smoke commands returned exit code 0 through their explicit expected-result declarations.
- Exact GPT model/version, API evidence, and full provider-chat export remain unavailable. This is one constrained direct-success case with zero correction rounds, not a provider benchmark or independent algorithm-design claim. P65 is not committed or pushed, and the Original repository remains untouched.

## 2026-07-16 P66 Edge Detection Shape Count GPT Packet

- Selected the distinct public-safe `EdgeDetection(Canny) -> Morphology(Close) -> Contour` workflow as the next bounded LLM-authoring candidate. It adds EdgeDetection coverage while retaining the existing rule-based sequential pipeline and final Contour count gate; no new tool, template dependency, camera path, or platform feature was introduced.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` imported `Public_EdgeDetection_Shapes.pipeline.xml` with 3 Steps, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`; nominal passed `ResultCount=4` and missing-shape input produced expected product NG `ResultCount=2 < 4`.
- Added the self-contained five-file packet `llm_prompt_packets/edge_detection_shapes`: README, first-round prompt, correction template, and byte-identical public Good/Missing-NG image copies. The prompt locks Canny `40/120`, `3`, `UseL2Gradient=true`; Morphology Close `3x3`; Contour ROI `90,100,410,95`; route `Main -> EdgeDetection_Edge -> EdgeDetection_EdgeJoin -> EdgeDetection_Shape_Preview`; and a final `ResultCount=4..4` gate.
- The exact packet reference XML replayed in the same latest EXE: import passed with 3 Steps and 0 errors/warnings, nominal passed `ResultCount=4`, and Missing-NG produced expected product NG `ResultCount=2 < 4`; all three commands returned exit code 0. Evidence is `artifacts/p66_edge_detection_baseline_20260716`; packet audit passed with five files, copied public image hashes, required prompt route/parameters/gate, one correction placeholder, and no private/path token.
- No external GPT response or correction loop exists for P66 yet. The user should send the two packet images and the complete first-round prompt in one new GPT conversation, then return the complete first XML response unchanged. Use the correction template only after an actual current-build validation or Good/NG failure. P66 is not committed or pushed; the Original repository remains untouched.

## 2026-07-16 P67 Edge Detection Shape Count GPT Direct-Success Replay

- The user returned one XML-only response in the P66 GPT packet workflow. The manually transferred XML and copied packet prompt are preserved under `artifacts/llm_transcripts/raw/20260716_edge_detection_shapes_gpt_round1`. Exact model/version, API evidence, and a full provider-chat export were not supplied and must remain unknown rather than inferred.
- XML-only boundary validation passed. The user-supplied response matched the mechanically verified three-Step EdgeDetection reference contract across 53 structured fields with zero differences.
- After a fresh 0-warning/0-error solution build, latest actual `bin/Debug/OpenVisionLab.exe` validation/import passed with 3 Steps, 0 errors, 0 warnings, no dependencies, and `ImageRun: SKIPPED`.
- Explicit nominal execution passed at `ResultCount=4` in 34.661 ms. Explicit Missing-NG execution produced expected product NG at `ResultCount=2 < 4` in 33.613 ms. Both smoke commands returned exit code 0 because their expected outcomes were declared explicitly.
- This is a real manually transferred direct-success example with zero correction rounds. Do not send the correction prompt or manufacture a failed round. Raw reports can contain local evidence locations and must not be copied into `docs/evidence` without a separate sanitization and explicit inclusion decision.
- P67 is raw evidence plus handoff updates only. It is not committed or pushed, and the Original repository remains untouched.

## 2026-07-16 P68 Edge Detection Shape Count GPT Sanitized Candidate

- Created the ignored six-file artifact-only candidate at `artifacts/llm_transcripts/sanitized/20260716_edge_detection_shapes_gpt_round1_direct_success`: `manifest.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged `response.xml`, and current-EXE nominal/negative result PNGs. It is not a tracked `docs/evidence` package.
- The prompt, response, nominal PNG, and Missing-NG PNG are byte-identical to P67 raw/current-EXE evidence. Payload scans found zero absolute Windows, user-home, application-data, Codex attachment, URL, email, credential-label, or private/legacy asset-hint matches.
- Both result PNGs are 572x420 and contain only `IHDR`, `IDAT`, and `IEND` chunk types. The candidate excludes raw reports and local evidence locations; its manifest records only repository-relative public inputs, hashes, outcomes, and classification limits.
- P68 was the artifact-only candidate gate. P69 records the user's subsequent explicit Dev-worktree inclusion decision; the Original repository remains untouched.

## 2026-07-16 P69 Edge Detection Shape Count GPT Evidence Included In Docs

- The user approved inclusion of the P68 Edge Detection direct-success candidate in the Dev worktree. The public six-file package is `docs/evidence/llm/20260716_edge_detection_shapes_gpt_direct_success`; the companion decision record is `docs/OPENVISIONLAB_LLM_EDGE_DETECTION_PUBLICATION_REVIEW_20260716.md`.
- The package contains only `README.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged `response.xml`, and OpenVisionLab-generated nominal/negative result PNGs. Raw manifests, validation/run reports, local paths, runtime identifiers, stack traces, and session data remain outside the package.
- Package-wide file-set, text/privacy, public-asset, PNG metadata, and immutable-hash checks passed. Both 572x420 result PNGs contain only `IHDR`/`IDAT`/`IEND` chunk types and match P67 raw evidence plus the P69 tracked replay byte-for-byte.
- After a fresh 0-warning/0-error solution build, latest actual `bin/Debug/OpenVisionLab.exe` replayed the tracked `response.xml`: validation/import passed with 3 Steps, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`; nominal passed at `ResultCount=4` in 33.914 ms; Missing-NG produced expected product NG at `ResultCount=2 < 4` in 30.826 ms. Both smoke commands returned exit code 0 through their explicit expected-result declarations.
- Exact GPT model/version, API evidence, and full provider-chat export remain unavailable. This is one constrained direct-success case with zero correction rounds, not a provider benchmark or independent algorithm-design claim. P69 is not committed or pushed, and the Original repository remains untouched.

## 2026-07-16 P70 RotateScale Geometry Baseline And GPT Packet

- Selected the distinct public-safe `RotateScale` geometry workflow as the next bounded external-authoring candidate. The one-Step contract resizes `Main` to `Geometry_ResizeHalf_Result` with Angle 0, 50 percent X/Y scale, Linear interpolation, and Constant border. It gates `ResultImageWidth=286..286`; it is an output-size check, not object detection, physical calibration, or a new platform feature.
- The first current-EXE baseline exposed a real review-copy gap: a valid `UseAcceptance` metric/range was not treated as explicit judgement evidence. `HasJudgementParameter` now recognizes an enabled acceptance metric with a configured minimum or maximum. This changes only the LLM review explanation; it does not alter XML execution, Preview/Run, layer creation, active-layer selection, or routing.
- After a fresh 0-warning/0-error build, latest actual `bin/Debug/OpenVisionLab.exe` import passed with 1 Step, 0 errors/warnings, no dependencies, `ImageRun: SKIPPED`, and `Inspection.Evidence: OK - explicit judgement criteria are present.` Explicit nominal execution passed `572x420 -> 286x210` at `ResultImageWidth=286` in 3.428 ms. The Wide-NG input produced expected product NG `640x420 -> 320x210`, `ResultImageWidth=320 > 286`, in 3.18 ms.
- Added the self-contained five-file GPT packet at `llm_prompt_packets/rotate_scale_geometry`: README, first-round prompt, correction template, and byte-identical public nominal/Wide-NG images. Packet audit passed: five files, copied public image hashes, locked route/parameters/gate, one correction placeholder, and no private/path token. The baseline review is `docs/OPENVISIONLAB_LLM_ROTATE_SCALE_BASELINE_REVIEW_20260716.md`; the authoring guide now includes a `RotateScale` geometry example.
- No real GPT/Gemini/Claude response exists for P70 yet. Send the two packet images and complete `COPY_THIS_TO_GPT.txt` prompt in one new GPT conversation, preserve the first complete XML response unchanged, and use the correction template only after an actual current-build validation or Good/Wide-NG failure. P70 is not committed or pushed, and the Original repository remains untouched.

## 2026-07-17 P71 Pipeline Review Inspection Readiness

- Added a compact, read-only `검사 준비도` strip to Pipeline Review. It separates input image, enabled Step/route validation, acceptance criteria, Good/Bad evidence, and unit calibration into five visible states before the operator chooses `리뷰 실행`.
- Input readiness preserves the downstream-input contract: an input produced by an earlier enabled Step is not reported as missing. An actual unloaded external input is reported by layer name. Route errors, route warnings, missing OK/NG criteria, missing comparison pairs, and `PIXELPERMM` calibration states remain explanatory and do not invoke Preview/Run or alter layer/routing state.
- The readiness calculation lives in `OpenVisionPipelineReviewReadinessPresenter`; the ViewModel exposes display state only, and the View code-behind only forwards state/localization. Opening Review, language changes, Step selection, and readiness refresh do not change `CanRunReview`, create layers, or execute tools.
- Fresh current-source evidence is under `artifacts/pipeline_review_readiness_20260716`: `before/wpf_shell_host_pipeline_review.png`, `after/wpf_shell_host_pipeline_review.png`, and `after/wpf_shell_host_pipeline_review_input_state.png`. The 1180x660 checks reported zero layout, text, or internal clipping issues. The main review area is clipped to its bounds and the lower detail row is 160px so the added strip does not push the status line outside the compact viewport.
- After a fresh 0-warning/0-error solution build, latest actual `bin/Debug/OpenVisionLab.exe` passed `recipe-pipeline-roundtrip`. Its report records no native Preview runs, one unchanged layer, no recipe sample execution, and an isolated explicit Review result. The actual-EXE Pipeline Review screenshot is `artifacts/pipeline_review_readiness_20260716/direct_exe/OpenVisionLab_PipelineReview_Roundtrip.png`.
- Screenshot smoke now verifies all five readiness controls, Korean/English refresh, actual missing-input wording, no automatic Preview/Run, positive `PIXELPERMM=0.006`, and rejection of non-positive calibration. Readiness, localization-catalog, external-reference, public-sample, and diff checks passed. P71 is not committed or pushed, and the Original repository remains untouched.
- Next product priority after P71 is connecting the 17 remaining Learn concepts to real tool/sample/explicit-run practice, followed by stronger industrial Validation Set evidence labels and measured rotate/scale/metrology reliability work.

## 2026-07-17 P72 Threshold Learn Practice Connection

- Completed the first of the 17 Learn practice connections with Threshold. The topic now names the exact public pair `Public_Threshold_BandPads_Good` / `Public_Threshold_BandPads_Missing_Bad`, the shared `Public_Threshold_BandPads` Pipeline, and the expected `ResultCount` values 4 / 1 before the operator changes Threshold GV.
- Added the dedicated Sample Picker Learn path `threshold`. Its classifier uses sample category/name rather than generic Pipeline flow, so Filter and Morphology Pipelines that also contain a Threshold Step do not leak into this focused pair. The curriculum regression confirms the path contains exactly the two public Threshold samples.
- Added `Threshold Tool 열기` inside the Threshold concept tab. It reuses the existing Shell related-tool callback and opens `ThresholdToolWpfView` only. It does not execute Preview/Run, create a result layer, or change the workspace layer. Preview/Run remains an explicit operator action.
- Fresh current-source before evidence: `artifacts/p72_learn_threshold_practice_20260717/before/wpf_openvision_learn_threshold/wpf_openvision_learn_threshold.png`. Valid after evidence: `artifacts/p72_learn_threshold_practice_20260717/after_r2/wpf_openvision_learn_threshold/wpf_openvision_learn_threshold.png`. The first after render had a transient WPF composition blackout and is excluded.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke learn-threshold-practice` under `artifacts/p72_learn_threshold_practice_20260717/direct_exe_r2`; its report records `PracticePath: threshold`, `PreviewRunCount: 0`, `LayerCount: 0`, and `Tool: ThresholdToolWpfView`. The actual-EXE capture is `OpenVisionLab_Learn_Threshold_Practice.png` in that folder.
- `wpf_shell_host_learn_entry`, `wpf_openvision_learn_curriculum`, readiness, localization catalog (`1733/78`), external-reference, public-sample (`30/229/15`), and diff checks passed. P72 is not committed or pushed, and the Original repository remains untouched.
- Next product priority after P72 is the Filter Learn practice connection using the existing `Public_Filter_Denoise_Good` / `Public_Filter_Denoise_Missing_Bad` pair and explicit `ResultCount=4/2` evidence. Sixteen Learn topics remain after this first completed connection.

## 2026-07-17 P73 Filter Learn Practice Connection

- Completed the second Learn practice connection with Filter. The Filtering topic now names the exact public pair `Public_Filter_Denoise_Good` / `Public_Filter_Denoise_Missing_Bad`, the shared `Public_Filter_Denoise` Pipeline, and the expected `ResultCount` values 4 / 2 before the operator opens the Filter Tool or explicitly runs Preview/Pipeline Review.
- Added the dedicated Sample Picker Learn path `filter`. Its classifier uses Filter category/name data, and the curriculum regression proves it contains exactly the two public Filter samples rather than the broader Threshold/Morphology/EdgeDetection preprocessing set. The focused path resolves `LEARN_FILTER.md` for the beginner-facing document action.
- Added a compact Filter concept-tab practice card. It links the top `실습 샘플` action with the existing `Filter Tool 열기` action and states the comparison sequence: open the pair, inspect Median Kernel, then explicitly Preview or Run Review to compare denoising and final count. No new execution command, layer write, or routing behavior was added.
- The first before-capture attempt exposed a stale smoke assertion: the initial Filter guide already says `Median`, `Bilateral`, and `Preview`, while the test incorrectly expected post-tool-open copy about input/output comparison. The assertion now checks the actual initial guide without changing visible UI behavior. The valid visual baseline is `artifacts/p73_learn_filter_practice_20260717/before_r2/wpf_openvision_learn_filtering/wpf_openvision_learn_filtering.png`; the failed first attempt is not UI evidence.
- Current-source after evidence is `artifacts/p73_learn_filter_practice_20260717/after/wpf_openvision_learn_filtering/wpf_openvision_learn_filtering.png`. It was visually checked for clipped text, icons, combo/input text, and overlap; the new card and existing scroll region remain legible at 1040x900.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke learn-filter-practice` under `artifacts/p73_learn_filter_practice_20260717/direct_exe`; its report records `PracticePath: filter`, `PreviewRunCount: 0`, `LayerCount: 0`, and `Tool: FilterToolWpfView`.
- Current-source `wpf_openvision_learn_filtering`, global `wpf_openvision_learn_curriculum`, and `wpf_shell_host_learn_entry` smokes passed. Readiness, localization catalog (`1733/78`), external-reference, public-sample (`30/229/15`), and `git diff --check` passed. P73 is not committed or pushed, and the Original repository remains untouched.
- Next product priority after P73 is the Morphology Learn practice connection with `Public_Morphology_Cleanup_Good` / `Public_Morphology_Cleanup_Missing_Bad` and explicit `ResultCount=4/2` evidence. Fifteen Learn topics remain after the Threshold and Filter connections.

## 2026-07-17 P74 Morphology Learn Practice Connection

- Completed the third Learn practice connection with Morphology. The Morphology topic now names the exact public pair `Public_Morphology_Cleanup_Good` / `Public_Morphology_Cleanup_Missing_Bad`, the shared `Public_Morphology_Cleanup` Pipeline, and the expected `ResultCount` values 4 / 2. It tells the operator to inspect `Open`, `Rect`, and the 5x5 kernel before an explicit Preview or Pipeline Review run.
- Added the dedicated Sample Picker Learn path `morphology`. Its classifier uses the Morphology category/name data, and the curriculum regression proves it contains exactly the two public Morphology samples rather than all Threshold preprocessing samples. The focused path resolves `LEARN_MORPHOLOGY.md` for the beginner-facing document action.
- Added a compact Morphology concept-tab practice card. It connects the existing top `실습 샘플` action with the existing `Morphology Tool 열기` action and preserves the existing Opening animation. No new execution command, layer write, or routing behavior was added.
- Fresh current-source before evidence is `artifacts/p74_learn_morphology_practice_20260717/before/wpf_openvision_learn_morphology/wpf_openvision_learn_morphology.png`; after evidence is `artifacts/p74_learn_morphology_practice_20260717/after/wpf_openvision_learn_morphology/wpf_openvision_learn_morphology.png`. Both were inspected at 1040x700; the practice card, existing animation, and Tool entry remain legible without clipped text or overlap.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke learn-morphology-practice` under `artifacts/p74_learn_morphology_practice_20260717/direct_exe`; its report records `PracticePath: morphology`, `PreviewRunCount: 0`, `LayerCount: 0`, and `Tool: MorphologyToolWpfView`.
- Current-source `wpf_openvision_learn_morphology`, global `wpf_openvision_learn_curriculum`, and `wpf_shell_host_learn_entry` smokes passed. Readiness, localization catalog (`1733/78`), external-reference, public-sample (`30/229/15`), and `git diff --check` passed. P74 is not committed or pushed, and the Original repository remains untouched.
- Next product priority after P74 is the Blob Learn practice connection with `Public_Blob_Particles_Good` / `Public_Blob_Particles_Sparse_Bad` and explicit `ResultCount=8..14 / 2..4` evidence. Fourteen Learn topics remain after the Threshold, Filter, and Morphology connections.

## 2026-07-17 P75 Blob Learn Practice Connection

- Completed the fourth Learn practice connection with Blob. The Blob topic now names the exact public pair `Public_Blob_Particles_Good` / `Public_Blob_Particles_Sparse_Bad`, the shared `Public_Blob_Particles` Pipeline, and the expected `ResultCount` bands: 8..14 is OK and 2..4 is NG. It directs the operator to inspect `MIN_AREA` and `MAX_AREA` before an explicit Preview or Pipeline Review run.
- Narrowed the existing Sample Picker Learn path `blob` to Blob category/name data. The curriculum regression proves it contains exactly the two public Blob samples instead of broad Particle/Density/Stain-related samples. The existing focused path continues to resolve `LEARN_BLOB.md`.
- Added a compact Blob concept-tab practice card. It connects the existing top `실습 샘플` action with the existing `Blob Tool 열기` action and preserves the existing candidate/area animation. No new execution command, layer write, or routing behavior was added.
- Fresh current-source before evidence is `artifacts/p75_learn_blob_practice_20260717/before/wpf_openvision_learn_blob/wpf_openvision_learn_blob.png`; after evidence is `artifacts/p75_learn_blob_practice_20260717/after/wpf_openvision_learn_blob/wpf_openvision_learn_blob.png`. Both were inspected at 1040x700; the practice card, existing animation, and Tool entry remain legible without clipped text or overlap.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke learn-blob-practice` under `artifacts/p75_learn_blob_practice_20260717/direct_exe`; its report records `PracticePath: blob`, `PreviewRunCount: 0`, `LayerCount: 0`, and `Tool: BlobToolWpfView`.
- Current-source `wpf_openvision_learn_blob`, global `wpf_openvision_learn_curriculum`, and `wpf_shell_host_learn_entry` smokes passed. Readiness, localization catalog (`1733/78`), external-reference, public-sample (`30/229/15`), and `git diff --check` passed. P75 is not committed or pushed, and the Original repository remains untouched.
- Next product priority after P75 is the Contour Learn practice connection with `Public_Contour_Shapes_Good` / `Public_Contour_Shapes_Missing_Bad` and explicit `ResultCount=5/2` evidence. Thirteen Learn topics remain after the Threshold, Filter, Morphology, and Blob connections.

## 2026-07-17 P76 Contour Learn Practice Connection

- Completed the fifth Learn practice connection with Contour. The Contour topic now names the exact public pair `Public_Contour_Shapes_Good` / `Public_Contour_Shapes_Missing_Bad`, the shared `Public_Contour_Shapes` Pipeline, and the expected `ResultCount` values 5 / 2. It directs the operator to inspect Retrieval mode, `MIN_AREA`, `MAX_AREA`, and contour display before an explicit Preview or Pipeline Review run.
- Narrowed the existing Sample Picker Learn path `contour` to Contour category/name data. The curriculum regression proves it contains exactly the two public Contour samples instead of broad Shape/Region/Surface/Fiducial-related samples. The existing focused path continues to resolve `LEARN_CONTOUR.md`.
- Added a compact Contour concept-tab practice card. It connects the existing top `실습 샘플` action with the existing `Contour Tool 열기` action and preserves the existing boundary/shape animation. No new execution command, layer write, or routing behavior was added.
- The first before-capture attempt exposed a stale screenshot-smoke assertion: the existing initial guide says `Approximation` and display color/line thickness, while the test expected a non-visible `Approx epsilon` phrase. The assertion now verifies the actual initial PropertyGrid/result-metric guide without changing visible UI behavior. Valid visual baseline is `artifacts/p76_learn_contour_practice_20260717/before_r2/wpf_openvision_learn_contour/wpf_openvision_learn_contour.png`; the first failed attempt is not UI evidence.
- Current-source after evidence is `artifacts/p76_learn_contour_practice_20260717/after/wpf_openvision_learn_contour/wpf_openvision_learn_contour.png`. The before and after views were inspected at 1040x700; the practice card, existing animation, and Tool entry remain legible without clipped text or overlap.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke learn-contour-practice` under `artifacts/p76_learn_contour_practice_20260717/direct_exe`; its report records `PracticePath: contour`, `PreviewRunCount: 0`, `LayerCount: 0`, and `Tool: ContourToolWpfView`.
- Current-source `wpf_openvision_learn_contour`, global `wpf_openvision_learn_curriculum`, and `wpf_shell_host_learn_entry` smokes passed. Readiness, localization catalog (`1733/78`), external-reference, public-sample (`30/229/15`), and `git diff --check` passed. P76 is not committed or pushed, and the Original repository remains untouched.
- Next product priority after P76 is the EdgeDetection Learn practice connection with `Public_EdgeDetection_Shapes_Good` / `Public_EdgeDetection_Shapes_Missing_Bad` and explicit downstream `ResultCount=4/2` evidence. Twelve Learn topics remain after the Threshold, Filter, Morphology, Blob, and Contour connections.

## 2026-07-17 P77 EdgeDetection Learn Practice Connection

- Completed the sixth Learn practice connection with EdgeDetection. The Edge / Line topic now names the exact public pair `Public_EdgeDetection_Shapes_Good` / `Public_EdgeDetection_Shapes_Missing_Bad`, the shared `Public_EdgeDetection_Shapes` Pipeline, and the expected downstream `ResultCount` values 4 / 2. The card makes the boundary clear: EdgeDetection creates the edge map, while Morphology/Contour supplies the final count; distance or width measurement remains in the following LineDistance topic.
- Added the dedicated Sample Picker Learn path `edge-detection`. Its classifier uses EdgeDetection category/name data, and curriculum regression proves it contains exactly the two public EdgeDetection samples rather than the broad preprocessing set. The focused path resolves `LEARN_EDGE_DETECTION.md`, whose path reference was updated to `edge-detection`.
- Added a compact EdgeDetection concept-tab practice card. It connects the existing top Practice Samples action with the existing EdgeDetection Tool action, asks the operator to inspect Canny Low/High and L2 Gradient, then explicitly Preview or Pipeline Review the edge map and final downstream count. No execution command, layer write, active-layer selection, or routing behavior was added.
- The first two before-capture attempts exposed stale screenshot assertions only: one expected post-tool Canny/Line copy from the initial screen, and one expected the non-visible internal name `LineGauge`. The assertions now check the visible `EdgeDetection` / `LineDistance` role boundary. No visible UI behavior changed during those assertion fixes. Valid before evidence is `artifacts/p77_learn_edge_detection_practice_20260717/before_r3/wpf_openvision_learn_edge_line/wpf_openvision_learn_edge_line.png`; current-source after evidence is `artifacts/p77_learn_edge_detection_practice_20260717/after/wpf_openvision_learn_edge_line/wpf_openvision_learn_edge_line.png`.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke learn-edge-detection-practice` under `artifacts/p77_learn_edge_detection_practice_20260717/direct_exe_final`; its report records `PracticePath: edge-detection`, `PreviewRunCount: 0`, `LayerCount: 0`, and `Tool: SimplePreprocessToolWpfView`.
- Current-source `wpf_openvision_learn_edge_line`, global `wpf_openvision_learn_curriculum`, and `wpf_shell_host_learn_entry` smokes passed. Readiness, localization catalog (`1733/78`), external-reference, public-sample (`30/229/15`), and `git diff --check` passed; the diff check emitted CRLF normalization warnings only. P77 is not committed or pushed, and the Original repository remains untouched.
- Next product priority after P77 is the LineDistance Learn practice connection using `Public_Line_Pins_Good` / `Public_Line_Pins_WidePin_Bad`. It must show the Good/Bad measurement evidence and retain a Range/Max outlier gate beside the average distance. Eleven Learn topics remain after the Threshold, Filter, Morphology, Blob, Contour, and EdgeDetection connections.

## 2026-07-17 P78 LineDistance Learn Practice Connection

- Completed the seventh Learn practice connection with LineDistance. The topic now names `Public_Line_Pins_Good` / `Public_Line_Pins_WidePin_Bad`, the shared `Public_Line_Pins_Distance` Pipeline, and the operator order: check `DistanceMmRange <= 0.03` first, then check `DistanceMmAvg=0.20..0.25` only after the sampling lines are consistent.
- Strengthened the public Line pipeline from an average-only gate to two explicit LineDistance Steps. Step 01 writes `Line_Range_Preview` and gates `DistanceMmRange <= 0.03`; Step 02 explicitly branches from `Main` with `ALLOW_BRANCH_INPUT=true`, writes the existing `Line_Preview`, and retains the nominal average gate. This keeps Preview/Run explicit and does not alter input-layer selection automatically.
- Current runner evidence: Good completed both Steps with `DistanceMmRange=0.012` and `DistanceMmAvg=0.222`. WidePin Bad stopped at the first consistency Step with `DistanceMmRange=0.095 > 0.03`; the public catalog now declares its expected failure metric as `DistanceMmRange=0.08..0.11`. The full 30-row public catalog gate passed with `GateStatus=OK`.
- Narrowed the existing Sample Picker `line` path to the `Public Synthetic / Line` category/name pair. Curriculum and Shell smokes prove it exposes exactly the two public Line samples and opens the `line` path without Preview/Run, layer creation, or routing changes.
- Added a compact LineDistance practice card and clarified the existing tool-location copy to name `DistanceMmAvg` and `DistanceMmRange/Max`. Fresh visual baseline is `artifacts/p78_line_distance_practice_20260717/before_r2/wpf_openvision_learn_line_distance/wpf_openvision_learn_line_distance.png`; current-source after evidence is `artifacts/p78_line_distance_practice_20260717/after/wpf_openvision_learn_line_distance/wpf_openvision_learn_line_distance.png`. Both were inspected at 1040x700 for clipping, overlap, and readable control text.
- The first baseline smoke exposed stale assertion text only: it expected English `Range gate`, `Pixel / mm`, and `Gate rule`, while the visible Korean screen presents `Range`, `Pixel/mm`, and `DISTANCE_RANGE_MAX`. The smoke now verifies the visible terms. No execution behavior changed through that fix.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke learn-line-distance-practice` under `artifacts/p78_line_distance_practice_20260717/direct_exe_final`; its report records `PracticePath: line`, `PreviewRunCount: 0`, `LayerCount: 0`, and `Tool: LineToolWpfView`.
- Current-source `wpf_openvision_learn_line_distance`, global `wpf_openvision_learn_curriculum`, and `wpf_shell_host_learn_entry` smokes passed. Readiness, external-reference, public-sample (`30/229/15`), full public catalog (`30/30`), and `git diff --check` passed; diff check emitted CRLF normalization warnings only. P78 is not committed or pushed, and the Original repository remains untouched.
- Next product priority after P78 is a bounded industrial Validation Set evidence pass: make sample status, expected Good/Bad evidence, acceptance metric, calibration applicability, and the next explicit operator action readable without turning Recipe Manager into a second Pipeline editor. Ten Learn topics remain after Threshold, Filter, Morphology, Blob, Contour, EdgeDetection, and LineDistance.

## 2026-07-17 P79 Validation Set Evidence Board

- Added a compact read-only evidence board above the existing local Validation Set editor. It shows expected OK/NG and missing-file status, the selected Pipeline acceptance gate, whether physical calibration applies, and the next explicit operator action. It is derived from the selected set and Pipeline only; opening or selecting it does not run Preview/Run, create layers, or change routing.
- When the Pipeline has no metric acceptance gate, the board says that the Pipeline Good/Bad result must be compared with the expected Good/Bad role instead of inventing a numeric criterion. When a selected metric uses `Mm`, it checks for a positive `PIXELPERMM`; non-physical gates are explicitly marked as not requiring calibration.
- Fresh baseline evidence is `artifacts/p79_validation_set_evidence_20260717/before/wpf_shell_host_recipe_local_validation_set.png`. Current-source after evidence is `artifacts/p79_validation_set_evidence_20260717/after_current_source_r4/wpf_shell_host_recipe_local_validation_set.png`; the focused screenshot smoke passed with `layout=0`, `text=0`, and `internal=0` after catching and correcting the initial grid-row overlap.
- A fresh full solution build passed with 0 warnings and 0 errors. The latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke recipe-manager-tabs` under `artifacts/p79_validation_set_evidence_20260717/direct_exe_recipe_manager_tabs_retry`; its report confirms local-set file/folder registration and missing-path repair preserve Preview/Run count, layers, and routing.
- Next implementation priority is source organization: define durable folder and responsibility rules, inventory the flat `1. Core` files, and move one clean cohesive group at a time without mixing moves with behavior changes. Do not move already modified files as part of the first cleanup bundle.

## 2026-07-17 P80 Core Source Organization

- Added durable source-organization rules to `AGENTS.md`: folder ownership signals, Core/WPF boundaries, no dumping folders, View code-behind responsibilities, small clean move groups, namespace preservation, and build/smoke verification after every group.
- Replaced the flat Core layout with physical-only responsibility folders while preserving namespaces and public APIs: `State` (5 files), `Display` (9), `Recipe` (5), `Pipeline/Definition` (6), `Pipeline/Execution` (6), `Pipeline/Storage` (5), `Pipeline/Validation` (3), and `Pipeline/Tools` (7). The Core root now contains only `desktop.ini` and the already-modified `VisionPipelineValidation.cs`.
- `VisionPipelineValidation.cs` deliberately remains at the old root path because it was dirty before the organization pass. Do not move it until its current behavior change is stabilized and verified as a separate clean move; this avoids mixing a user-visible validation change with a physical refactor.
- Updated `tools/OpenVisionReadinessCheck` to inspect the new explicit Validation and Tools paths. The check does not retain hidden old-path fallback behavior, so future structure regressions are visible.
- Every move group passed a full Debug solution build with 0 warnings and 0 errors. Latest `bin/Debug/OpenVisionLab.exe` passed `--smoke recipe-pipeline-roundtrip` under `artifacts/p80_core_organization_20260717/direct_exe_recipe_pipeline_roundtrip`; it preserved the no-auto-run/layer/routing contract and the explicit-run `WAIT` state.
- Final readiness, external-reference, and public-sample checks passed; `git diff --check` passed with existing CRLF normalization warnings only. P80 is not committed or pushed, and the Original repository remains untouched.
- Next source-organization priority is conditional: after the current `VisionPipelineValidation.cs` work is stable, move it to `Pipeline/Validation` in a separate clean change. Then inventory the largest WPF files for natural Presenter/ViewModel boundaries without splitting files merely by line count. Product priority remains RotateScale/measurement reliability and the remaining Learn practice connections.

## 2026-07-17 P81 MENU WPF Source Organization

- Applied the same physical-only ownership organization to `0. UI/0) MENU`. The WPF root fell from 141 C# files to 16; the `0) MENU` top level now has no production C# file. The retained top-level `0) MENU.zip` and `desktop.ini` were deliberately not deleted or rewritten.
- Staged 126 clean WPF source moves without namespace, public API, XAML, Preview/Run, layer, routing, or docking behavior changes. The owner layout is: `Docking` (19, including `Contracts` and `TestSupport`), `NativeTools` (36), `Viewer` (6), `Windows` (2), shared `Tooling` (2), `PipelineReview` (2 presenters), `Recipe` (10), `Workspace` (5), and `Shell` (44 across `Chrome`, `Commands`, `Layers`, `Session`, `Tooling`, `Workspace`, `Recipe`, `Documents`, `State`, and `Support`).
- The five `OpenVisionShellHostDockedLayerOrchestrator` partial files moved together under `Shell/Layers/Orchestration`. `MainWorkspaceLayoutState` moved from the MENU top level to `Wpf/Workspace/State`.
- Kept XAML/code-behind pairs, the dirty Shell host/menu/recipe files, dirty Workspace Learn files, dirty Pipeline Review files, and untracked new presenters/intent skills in place. They require a separate ownership review rather than a speculative move mixed with the existing behavior changes.
- Read-only boundary audit of `OpenVisionShellHostRecipeCommandSurface` found 12,089 lines, 54 active diff hunks, and roughly 20 recipe validation/batch/sample row-or-option model classes beginning near line 9,827. One active hunk also falls in that model range, so do not extract `Recipe/Models` yet. After the current dirty feature work is stabilized, first move only clean model classes, then review validation/batch command behavior for Presenter or Controller boundaries.
- Updated `OpenVisionReadinessCheck` explicit paths for moved Recipe intent skills, Workspace sample-picker ViewModel, Shell sample workflow, hosted document controller, state presenter, and command catalog. One initially missed `OpenVisionShellCommandCatalog` path caused a readiness failure; it was corrected immediately. A final preflight confirmed all 15 direct WPF readiness paths exist, so the check has no hidden fallback search.
- Every move bundle passed `dotnet build OpenVisionLab.sln -c Debug -p:Platform=Any CPU` with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke recipe-pipeline-roundtrip` under `artifacts/p81_menu_wpf_organization_20260717/direct_exe_recipe_pipeline_roundtrip`: `NativePreviewRuns=0`, `LayerCount=1`, no recipe-sample execution, and downstream input remains explicit-run `WAIT`.
- Final readiness, external-reference, and public-sample checks passed. P81 is not committed or pushed, and the Original repository remains untouched.
- Next source-organization priority is a behavior-neutral responsibility review of the remaining large dirty Shell host surface. Do not move it for cosmetics; first identify natural Presenter/Controller/ViewModel extraction points. Product priority remains measurement reliability and remaining Learn practice connections.

## 2026-07-17 P82 Recipe Command Surface Model Ownership

- The user explicitly continued the source-cleanup work, so the top-level recipe DTO tail was separated from the already-dirty `OpenVisionShellHostRecipeCommandSurface` without changing names, namespaces, callbacks, command behavior, Preview/Run behavior, layer routing, or UI copy. The command surface now owns command coordination plus the shared `OpenVisionRecipeText` helper; it no longer owns top-level validation/review/sample/batch DTO declarations.
- `OpenVisionShellHostRecipeCommandSurface.cs` was reduced from 12,089 to 9,827 lines. Ten validation/review/operator DTOs now live in `Wpf\Recipe\Models\OpenVisionRecipeValidationReviewModels.cs` (884 lines), and ten sample/pair/batch DTOs now live in `Wpf\Recipe\Models\OpenVisionRecipeSampleRunModels.cs` (1,420 lines). The previously dirty `OpenVisionRecipeBatchRunComparisonRow` change was preserved as part of its exact model extraction.
- Structural proof confirms each of the 20 model declarations exists once, in its new owner file only; the command surface contains no `public sealed class OpenVisionRecipe...` DTO declaration. `OpenVisionRecipeText` intentionally remains alongside the command surface because both it and the extracted models reference the same localized text helper.
- `AGENTS.md` now defines `Recipe\Models` as the owner for top-level recipe validation, review, sample-run, and batch-result DTOs. `OpenVisionReadinessCheck` now requires those exact model paths and rejects their old command-surface declarations, preventing silent regression to the flat layout.
- A fresh 0-warning/0-error solution build passed. Latest actual `bin/Debug/OpenVisionLab.exe` passed both `recipe-manager-tabs` and `recipe-pipeline-roundtrip` under `artifacts/p82_recipe_models_refactor_20260717`; the former exercised Recipe Manager summary/advanced review, validation suite, LLM intent, branch comparison, and explicit XML apply, while the latter preserved the no-auto-run/layer/routing `WAIT` contract.
- Readiness, external-reference, and public-sample checks passed. P82 is not committed or pushed, and the Original repository remains untouched.
- Do not continue slicing command behavior from this dirty surface merely to reduce line count. The next source refactor must identify one complete execution responsibility and prove its call path separately. Product priority remains measurement reliability and remaining Learn practice connections.

## 2026-07-17 P83 Remaining MENU Root Owner Cleanup

- Moved the remaining untracked owner-ready WPF files out of the MENU root without content changes: `OpenVisionPipelineReviewReadinessPresenter` now belongs to `PipelineReview\Presenters`; `OpenVisionRecipeEdgeBasedMatchingIntentSkill` and `OpenVisionRecipeFeatureMatchingIntentSkill` now belong to `Recipe\IntentSkills`. The WPF root now contains only XAML/code-behind pairs and files that are still dirty or require a later responsibility decision.
- Added explicit readiness paths for all three files. The structural contract now verifies that Pipeline Review readiness and the EdgeBasedMatching/FeatureMatching XML starters cannot silently drift back to the root directory.
- A fresh 0-warning/0-error solution build passed. Latest actual `bin/Debug/OpenVisionLab.exe` passed `recipe-pipeline-roundtrip` under `artifacts/p83_menu_root_owner_cleanup_20260717/recipe-pipeline-roundtrip`, preserving no auto Preview/Run, layer count, active-layer, routing, and explicit produced-input `WAIT` behavior.
- The current `recipe-manager-tabs` attempt used the same latest EXE but could not open the Windows clipboard (`CLIPBRD_E_CANT_OPEN`) while pasting the smoke XML draft. This is an external clipboard-lock limitation, not a product assertion failure; the report is retained under `artifacts/p83_menu_root_owner_cleanup_20260717/recipe-manager-tabs`. Do not treat that scenario as current passing evidence until rerun succeeds in an unlocked desktop session.
- Readiness, external-reference, and public-sample checks passed. P83 is not committed or pushed, and the Original repository remains untouched.
- Next priority should leave source cosmetic work alone: either define one fully bounded command-execution ownership refactor with dedicated smoke evidence, or return to the higher product priority of measurement reliability and remaining Learn practice connections.

## 2026-07-17 P84 LineDistance Measurement Evidence And Scale Validation

- Corrected the LineDistance Bad-sample review smoke to use the catalog's real `DistanceMmRange` evidence. The WidePin Bad run now proves the intended explicit-run state: Step 01 fails `DistanceMmRange=0.095 > 0.03`, Step 02 remains `WAIT`, and the review shows `0 OK / 1 NG / 1 WAIT` without selecting a step causing Preview/Run.
- Clarified Good/Bad pair wording in Pipeline Review. The result now separates the active Pipeline gate from the sample-evidence band, for example `Pipeline criterion <= 0.03 / NG sample band 0.08~0.11`, so a Bad sample that correctly matches its expected catalog band is not presented as passing the Pipeline criterion.
- Changed mm-scale readiness from a false completion signal to an advisory reference check. A positive `PIXELPERMM` now means `Verify reference ... mm/px`; px-only acceptance gates are not calibration-required, and mm gates with a missing or non-positive scale remain an explicit check. This does not claim physical calibration and does not add camera, calibration hardware, or platform scope.
- Current-source screenshots: baseline `artifacts/p84_measurement_scale_evidence_20260717/before_calibration/wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`; final `artifacts/p84_measurement_scale_evidence_20260717/after_calibration_final/wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`. The final 1180x660 screenshot smoke passed with `layout=0`, `text=0`, and `internal=0`.
- A fresh 0-warning/0-error solution build and screenshot-tool build passed. Current-source `wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics` and `wpf_shell_host_pipeline_review` passed; the latter explicitly checks configured-mm advisory, px-only N/A, and missing-mm Check states. The latest Debug EXE passed `--smoke recipe-pipeline-roundtrip` under `artifacts/p84_measurement_scale_evidence_20260717/recipe_pipeline_roundtrip_final`.
- Readiness, external-reference, and public-sample checks passed. `git diff --check` passed with pre-existing CRLF normalization warnings only. P84 is not committed or pushed, and the Original repository remains untouched.
- Next product priority is the LineDistance/RotateScale Learn practice connection: teach image-size transformation separately from physical measurement, require a reference-object check before treating mm values as decision evidence, and keep the existing explicit Preview/Run path.

## 2026-07-17 P85 Full Source Ownership Pass

- Completed a source-ownership pass across `0. UI\\6) Vision Test\\Wpf`, `0. UI\\6) Vision Test`, `1. Core`, and the owner-ready portion of `0. UI\\0) MENU\\Wpf`. The direct `Vision Test\\Wpf` root no longer contains C# or XAML source files. Its former 107 direct-root artifacts now have explicit owners under `Tooling`, `ToolViews`, and `Learn`; namespaces, XAML `x:Class`, public names, and behavior were preserved.
- The outer `Vision Test` root is also clear. `VisionPipelineDesignTime` now belongs to `Composition`; sample-catalog metadata belongs to `1. Core\\Pipeline\\Storage`; and sample-check execution belongs to `1. Core\\Pipeline\\Execution`. The former Core-root `VisionPipelineValidation` now belongs to `1. Core\\Pipeline\\Validation`. SHA-256 checks confirmed the four moved non-XAML source files retained identical content.
- MENU owner-ready files moved by responsibility: floating/title-bar windows live in `Wpf\\Windows`, viewer documents in `Wpf\\Viewer`, sample-picker and Learn-sample support in `Wpf\\Workspace\\Samples`, and the host menu presenter in `Wpf\\Shell\\Chrome`. Moved XAML resource URIs were corrected and smoke-tested; do not duplicate the common theme to compensate for a path error.
- Extracted two real independent helpers from the still-large dirty `OpenVisionShellHostRecipeCommandSurface`: `OpenVisionGuidedSetupCatalog` now belongs to `Wpf\\Recipe\\IntentSkills`, and `OpenVisionRecipeText` belongs to `Wpf\\Recipe\\Models`. This is a behavior-neutral responsibility split, not a cosmetic file move.
- `OpenVisionReadinessCheck` now enforces the ownership shape: no direct source files in the Core or Vision Test roots, no direct source files in `Vision Test\\Wpf`, only the intentional three-file Shell composition boundary in MENU Wpf, and no restored Guided Setup/RecipeText helper declarations in the recipe command surface. `AGENTS.md` records the same boundary.
- The only direct source files intentionally retained in `0. UI\\0) MENU\\Wpf` are `OpenVisionShellHostView.xaml`, its code-behind, and `OpenVisionShellHostRecipeCommandSurface.cs`. They remain because moving either large dirty Host artifact without first extracting a complete controller/presenter/view-model responsibility would disguise, rather than reduce, coupling. The next refactor must extract one complete execution responsibility with dedicated smoke evidence.
- Fresh current-source view captures passed after the final source changes: `artifacts/p85_vision_test_wpf_organization_20260717/final_threshold_tool_current_source`, `final_learn_curriculum_current_source`, and `final_workspace_picker_current_source` all report `layout=0`, `text=0`, and `internal=0`.
- A fresh 0-warning/0-error solution build, screenshot-tool build, and readiness check passed. The latest `bin\\Debug\\OpenVisionLab.exe` then passed `recipe-pipeline-roundtrip` under `artifacts/p85_vision_test_wpf_organization_20260717/final_build_exe_recipe_pipeline_roundtrip` and `learn-line-distance-practice` under `final_build_exe_learn_line_distance`; both preserved explicit Preview/Run and no implicit layer changes.
- P85 is not committed or pushed. The Original repository remains untouched.

## 2026-07-17 P86 LLM Prompt And Intent Contract Ownership

- Extracted the independent LLM authoring prompt responsibility from `OpenVisionShellHostRecipeCommandSurface` into `Wpf\\Recipe\\IntentSkills\\OpenVisionRecipeLlmPromptBuilder.cs`. `OpenVisionRecipeLlmPromptRequest` carries the current recipe/pipeline, operator wording, reference image, and pin-gap gate context; the Host now only gathers that state and delegates construction.
- Moved template classification, intent contract wording, result-channel wording, and template guidance into `OpenVisionRecipeLlmIntent` in the same IntentSkills owner. The Host imports that explicit static owner for the remaining guided-setup and validation call sites; it no longer declares the prompt packet, tool-family contract, or template classifier methods. The command surface decreased from 9,827 to 9,553 lines in this bounded extraction.
- Added source-ownership readiness assertions that reject restored prompt-packet/intent-contract methods in the Host and require the new request/builder/explicit-Preview-Run contract in IntentSkills. `AGENTS.md` now names standalone LLM prompt and intent contracts as an IntentSkills responsibility.
- Updated `docs\\OPENVISIONLAB_LLM_TOOL_CATALOG.json` source-evidence paths to the current `1. Core\\Pipeline\\...` ownership layout and parsed the JSON successfully after the update.
- Extended the clipboard-independent `recipe-manager-llm-intent-skills` EXE smoke. It now validates template-matching prompt identity, intent/result-channel text, decimal-score and angle/path constraints, LineDistance XML-only packet text, and `PreviewRunCount=0`; this avoids treating a transient desktop clipboard lock as a product failure.
- Fresh 0-warning/0-error build, readiness, external-reference, and public-sample checks passed. The latest `bin\\Debug\\OpenVisionLab.exe` passed `recipe-manager-llm-intent-skills` under `artifacts/p86_llm_prompt_owner_refactor_20260717/final_build_exe_recipe_manager_llm_intent_skills_contract`; it verified the matching and pin-gap prompt packets without clipboard access, blocked the Contour-only pin-gap draft, and kept Preview/Run unchanged.
- P86 is not committed or pushed. The Original repository remains untouched.
- Next structure priority: extract the LLM XML draft validation/dependency-review execution path as one controller with an explicit request/result boundary. Do not move the whole Recipe Command Surface or Host XAML merely to reduce file length.

## 2026-07-17 P87-P89 LLM Draft Validation And Dependency Review Ownership

- Completed the next real responsibility split from the remaining dirty `OpenVisionShellHostRecipeCommandSurface`. `Recipe\Validation\OpenVisionRecipeLlmDraftValidationRules` now owns pure XML syntax, result-channel, and strict Intent contract rules. The Host calls those rules but no longer declares them.
- Added `Recipe\Review\OpenVisionRecipeDependencyReviewService`. It owns external image/template path classification, path resolution, review-bundle relocation/content-mismatch evidence, scan-versus-copy decisions, recipe dependency copying, and reference-image copying. It returns `OpenVisionRecipeDependencyReviewResult`; the Host only applies the returned report and rows to its display state. Review bundle exporter/inspector now depend on this service rather than Host static helpers.
- Added `Recipe\Validation\OpenVisionRecipeLlmDraftValidationService` with explicit `OpenVisionRecipeLlmDraftValidationRequest` and `OpenVisionRecipeLlmDraftValidationResult`. It owns XML syntax/deserialization, Pipeline schema/routing validation, result-channel and Intent contracts, reference-image evidence, and dependency review composition. The Host `TryBuildLlmDraftPipeline` now only builds the request, applies the result to its UI properties, and returns success.
- The command surface reduced from 9,553 lines after P86 to 8,889 lines. This was a responsibility extraction, not a move-for-size refactor: no Preview/Run, layer, routing, PropertyGrid, or UI workflow contract changed.
- `AGENTS.md` and `OpenVisionReadinessCheck` now enforce the owner boundaries. The readiness check rejects restored prompt, XML syntax, result-channel, Intent, XML-deserialization, dependency-classification, dependency-resolution, or dependency-copy implementation in the Host; it requires the exact IntentSkills, Validation, and Review owner files.
- Extended the clipboard-independent `recipe-manager-llm-intent-skills` EXE smoke with a real temporary PNG template. It validates an existing dependency path as `Found/확인`, while also checking the matching/LineDistance prompt packets, Contour-only pin-gap rejection, and `PreviewRunCount=0`. This avoids the known clipboard-lock limitation in the older all-in-one Recipe Manager smoke.
- Fresh 0-warning/0-error builds passed after P87, P88, and P89. The latest `bin\Debug\OpenVisionLab.exe` passed `recipe-manager-llm-intent-skills` under `artifacts/p89_llm_draft_validation_service_20260717/final_build_exe_recipe_manager_llm_intent_skills`; its report includes `DependencyReview: existing template path found without clipboard` and `PreviewRunCountUnchanged: 0`. Readiness, external-reference, public-sample (`30/229/15`), and `git diff --check` passed; diff check emitted CRLF normalization warnings only.
- P87-P89 are not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: inspect `LLM review bundle load/export` versus `Pipeline Review result/execution state` and extract only a complete request/result or presenter/controller responsibility. Do not move the remaining Host XAML/code-behind or command surface merely for tree appearance. Product priority remains the explicit rule-based learning/verification workflow and real LLM correction evidence.

## 2026-07-17 P90 LLM Correction Packet Ownership

- Extracted the pure correction-packet text construction from `OpenVisionShellHostRecipeCommandSurface` into `Wpf\Recipe\IntentSkills\OpenVisionRecipeLlmReviewBundleBuilder.cs`. `OpenVisionRecipeLlmReviewBundleRequest` carries recipe/pipeline identity, selected intent, operator context, failure review, validation/dependency/diff reports, and the current XML draft. The builder owns the correction rules, result-channel contract, and packet layout; the Host only checks whether copying is currently allowed and supplies state.
- Added the minimal `BuildLlmReviewBundleTextForTest` test hook to the command surface. It exercises the same Host-to-builder wiring without invoking the Windows clipboard, which can be externally locked in desktop smoke sessions.
- Extended the clipboard-independent `recipe-manager-llm-intent-skills` EXE smoke. After it validates a real temporary template path, it now confirms that the correction packet contains the shared Intent contract, `Inspection.Status`, `Direct_LLM_DependencyReview`, and the current dependency path. The latest report records `CorrectionBundle: current validation and dependency context assembled without clipboard` and `PreviewRunCountUnchanged: 0`.
- `OpenVisionReadinessCheck` rejects the correction-packet header in the Host and requires the explicit IntentSkills builder/request/shared Intent contract. `AGENTS.md` now names prompt, Intent, and correction-packet contracts as `Recipe\IntentSkills` ownership.
- A fresh 0-warning/0-error solution build passed. Latest `bin\Debug\OpenVisionLab.exe` passed `recipe-manager-llm-intent-skills` under `artifacts/p90_llm_review_bundle_builder_20260717/final_build_exe_recipe_manager_llm_intent_skills`. Readiness, external-reference, public-sample (`30/229/15`), and `git diff --check` passed; the diff check emitted CRLF normalization warnings only.
- P90 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not create a review-bundle session wrapper solely to move the existing inspection field. Audit Pipeline Review execution/result state first and split only a complete presenter/controller or request/result boundary. Product priority remains the explicit rule-based learning/verification workflow and real LLM correction evidence.

## 2026-07-17 P91 Pipeline Review Execution Ownership

- Audited the current Pipeline Review call path before editing. `OpenVisionPipelineReviewDocument` was simultaneously responsible for View event wiring/selected-Step presentation and the explicit runner call, display-layer execution context, Step-result cache, review output-image cache, and result-image disposal. This was a real runtime boundary, not a file-size split.
- Added `Wpf\PipelineReview\Execution\OpenVisionPipelineReviewExecutionController` and its explicit result/update contracts. The controller now owns `VisionPipelineExecutionService.RunAsync`, copies current display layers into a review-only execution context, caches Step summaries and output images, raises Step update events on the View dispatcher, and disposes runner result images after cache capture. The document now owns only explicit Run Review state text, View refresh/selection, fixture interaction, and displayed guide/result text.
- No Preview/Run path was broadened: the controller is called only from the existing explicit `Run Review` command. Opening Pipeline Review, selecting a Step, refreshing layers, or reading readiness does not execute the pipeline.
- `OpenVisionReadinessCheck` now rejects a restored runner/cache/context implementation in `OpenVisionPipelineReviewDocument` and requires the new controller/result contracts. `AGENTS.md` records `PipelineReview\Execution` as the owner for execution state and review-only image lifetime.
- Fresh full solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` passed `recipe-pipeline-roundtrip` under `artifacts\p91_pipeline_review_execution_controller_20260717\final_build_exe_recipe_pipeline_roundtrip`; the report retained `WAIT`, explicit Run Review, `NativePreviewRuns: 0`, and `LayerCount: 1`. Current-source `wpf_shell_host_pipeline_review` and `wpf_shell_host_pipeline_review_input_state` both passed with `layout=0`, `text=0`, and `internal=0`.
- Readiness, external-reference, public-sample (`30/229/15`), and `git diff --check` passed; diff check emitted existing CRLF normalization warnings only. P91 is not committed or pushed, and the Original repository remains untouched.
- Next source-organization priority: do not split selected-Step presentation or review-bundle session fields unless a new call-path audit identifies a full owner. Return to the higher product priority of real correction-loop evidence or current-build operator workflow friction.

## 2026-07-17 P92 Recipe Run Review Presentation Ownership

- Audited the remaining recipe run-review call path before editing. The Host still derived the operator summary, selected Good/Bad role suffix, saved batch-run review, and ordered next action from recipe/sample/pair/history DTOs. Those methods are reused by the decision board, handoff report, Guided Setup strip, result channels, role drill-down, and Run History, so they form a real presentation-policy boundary.
- Added Wpf\Recipe\Review\OpenVisionRecipeRunReviewPresenter. It accepts existing DTOs and an already-resolved failed Step; it owns formatted operator run review, selected-role suffix, saved-run review, and next-action ordering. The Host retains selected-state lookup, command enablement, clipboard copy, and PropertyChanged coordination.
- No execution behavior changed. The presenter cannot run Preview/Run, create layers, change input/output routing, modify XML, or access WPF controls. OpenVisionReadinessCheck now rejects restoration of the four Host text/policy methods and requires the dedicated presenter methods.
- Fresh solution build passed with 0 warnings/0 errors. Latest bin\Debug\OpenVisionLab.exe passed recipe-manager-tabs under artifacts\p92_recipe_run_review_presenter_20260717\final_build_exe_recipe_manager_tabs. Its report confirms RoleDrilldown, FailedRunLink, SelectedRunReview, SelectedRunReviewCopy, OperatorDecisionSummaryBand, and the existing explicit Preview/Run contracts.
- P92 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: audit the remaining Recipe Manager operator decision-board/report composition. Split only a complete presenter or request/result boundary; do not move or wrap Host fields solely to shorten OpenVisionShellHostRecipeCommandSurface.

## 2026-07-17 P93 Recipe Operator Decision Presentation Ownership

- Audited the remaining operator decision board and handoff report path before editing. The Host was deriving XML/sample/Good-Bad cards, final status, metric evidence, validation rows, result channels, and the handoff report from existing DTOs. Those outputs share one deterministic presentation contract, while Step resolution and clipboard commands remain Host responsibilities.
- Added `Wpf\Recipe\Review\OpenVisionRecipeOperatorDecisionPresenter` with explicit request/result contracts. The presenter owns all decision-board and handoff text derivation; the Host now supplies selected recipe/sample/history state and the already-resolved evidence/handoff Steps.
- No execution behavior changed. The presenter has no WPF, Preview/Run, layer, routing, XML mutation, or recipe mutation dependency. Opening Recipe Manager, selecting rows, reviewing result channels, and copying a handoff report remain read-only with respect to recipe execution.
- OpenVisionReadinessCheck now rejects restoration of the nine former Host composition methods and requires the dedicated `Recipe\Review` request/result/presenter owner. `AGENTS.md` and the ownership proof record the decision-board/handoff boundary.
- Fresh full solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` passed `recipe-manager-tabs` under `artifacts\p93_recipe_operator_decision_presenter_20260717\final_build_exe_recipe_manager_tabs`; the report records `RoleDrilldown`, `FailedRunLink`, `SelectedRunReviewCopy`, `OperatorDecisionSummaryBand`, and the existing explicit Preview/Run contracts.
- P93 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: audit remaining Recipe Manager lifecycle/session coordination only if a complete Controller/Presenter/service boundary exists; otherwise return to real correction-loop evidence or current-build workflow friction.

## 2026-07-17 P94 Recipe Pipeline Comparison Presentation Ownership

- Audited the LLM draft review and active/selected pipeline comparison path before editing. The Host was loading current pipelines and also formatting draft-import review, XML diff, variant comparison, step/parameter changes, and dependency-path deltas. The display portion is a complete pure presentation boundary.
- Added `Wpf\Recipe\Review\OpenVisionRecipePipelineComparisonPresenter`. It accepts supplied active/draft/selected pipelines and owns the read-only comparison strings; the Host retains current recipe/selection lookup and storage reads only.
- No execution behavior changed. The presenter cannot access WPF controls, recipe storage, Preview/Run, layers, routing, XML mutation, or recipe mutation. Selecting a variant remains read-only and does not activate or execute it.
- OpenVisionReadinessCheck now rejects restored Host pipeline-diff helpers and requires the dedicated comparison presenter. `AGENTS.md` and the source-ownership proof record the `Recipe\Review` ownership.
- Fresh full solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` passed `recipe-manager-tabs` under `artifacts\p94_recipe_pipeline_comparison_presenter_20260717\final_build_exe_recipe_manager_tabs`; it recorded `PipelineVariantComparison: active/selected diff visible without Preview/Run` and `LlmXmlDiff: visible`.
- P94 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: return to user-visible evidence or real correction-loop transcripts unless a future Host audit identifies another complete ownership boundary. Do not split validation-set lifecycle or selected-Step UI coordination merely to reduce line count.

## 2026-07-17 P95 Recipe Pipeline Step Review Presentation Ownership

- Audited the selected-Step review call paths before editing. The Host was composing failure links, corrected-output guidance/evidence, Step-flow context, branch/output comparison text and rows, and Step-slot labels from selected DTO state. This is one pure review-presentation boundary.
- Added `Wpf\Recipe\Review\OpenVisionRecipePipelineStepReviewPresenter`. It receives selected Step/result DTOs and derives all review text/rows; the Host keeps selection changes, PropertyGrid/XML apply, tool opening, and layer navigation.
- No execution behavior changed. The presenter has no WPF, Preview/Run, layer/routing mutation, storage, or recipe mutation dependency. Branch/output comparison remains read-only; corrected-output review still requires explicit PropertyGrid/XML apply and explicit review actions.
- OpenVisionReadinessCheck now rejects restored Host step-review composition methods and requires the `Recipe\Review` presenter. `AGENTS.md` and the source-ownership proof record the selected-Step/branch-output ownership.
- Fresh full solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` passed `recipe-manager-tabs` under `artifacts\p95_recipe_pipeline_step_review_presenter_20260717\final_build_exe_recipe_manager_tabs`; it recorded `FailedRunLink`, `CorrectedOutputReview`, `StepComparisonGrid`, `BranchOutputComparison`, and the existing explicit Preview/Run contracts.
- P95 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not split remaining Recipe Manager lifecycle/validation-set persistence or Shell selection coordination without a new complete owner. Return to real correction-loop evidence or a current-build UX issue first.

## 2026-07-17 P96 Recipe Run History Presentation Ownership

- Audited Run History before editing. The Host was deriving the saved-run NG filter, NG-cause summary, baseline resolution, comparison rows/default-row selection, and timing-comparison summary while also owning persisted-run loading and selected UI state. The derivation portion is a complete read-only review boundary.
- Added `Wpf\Recipe\Review\OpenVisionRecipeRunHistoryPresenter`. The Host continues to load persisted current/baseline summaries and coordinate selection/notifications; the presenter receives supplied DTOs/summaries and derives the filter, baseline policy, row comparison, default review row, and correctness/performance summary.
- No execution behavior changed. The presenter has no WPF, storage-load, Preview/Run, layer/routing mutation, XML mutation, or recipe mutation dependency. Existing timing-comparison guards still require matching suite kind/name, exact sample-image multiset, and complete timing coverage; outcome comparison remains available when timing is skipped.
- OpenVisionReadinessCheck rejects restored Host Run History presentation helpers, requires the Presenter boundary, and rejects direct `VisionPipelineBatchRunSummaryStorage.Load` usage in the presenter.
- Fresh solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs` passed under `artifacts\p96_recipe_run_history_presenter_20260717\final_build_exe_recipe_manager_tabs`; its report confirms `BenchmarkBaselineSelection`, `RunHistoryAnalytics`, `RunHistoryPerformanceComparison`, `RunHistoryNgFilter`, and unchanged explicit Preview/Run behavior. Readiness, external-reference, public-sample (`30/229/15`), `P96_SOURCE_OWNERSHIP`, and `git diff --check` passed; diff check emitted existing CRLF normalization warnings only.
- P96 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not split validation-set persistence, Recipe Manager lifecycle orchestration, or Shell selected-state coordination unless a new call-path audit finds a complete owner. Prefer real correction-loop evidence or a current-build operator UX issue next.

## 2026-07-17 P97 Recipe Good/Bad Sample-Matrix Presentation Ownership

- Audited the Good/Bad sample-matrix path before editing. The Host was obtaining same-pair sample rows, mapping the latest pair-run results, preserving/defaulting the selected matrix row, and formatting the matrix summary. These operations consume supplied catalog/run state only and form a complete read-only review boundary.
- Added `Wpf\Recipe\Review\OpenVisionRecipeSampleMatrixPresenter`. The Host retains selected sample/pair state and notifications; the presenter owns matrix rows, selected-row priority, and summary text. Validation Suite execution, persistence, PropertyGrid state, XML, layers, routes, and Preview/Run remain outside this presenter.
- Existing selection policy remains explicit: preserve the prior sample when present; otherwise choose an NG row, then a pending row, then the first available row.
- OpenVisionReadinessCheck rejects restored Host sample-matrix builders, requires the Presenter methods, and rejects pipeline execution from the presenter.
- Fresh solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs` passed under `artifacts\p97_recipe_sample_matrix_presenter_20260717\final_build_exe_recipe_manager_tabs`; its report confirms `PairRoleCards`, `RoleDrilldown`, `FailedRunLink`, `ValidationSuite`, and explicit Preview/Run-free Recipe Manager behavior. Readiness, external-reference, public-sample (`30/229/15`), `P97_SOURCE_OWNERSHIP`, and `git diff --check` passed; diff check emitted existing CRLF normalization warnings only.
- P97 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not split validation-set persistence, Recipe Manager lifecycle orchestration, or Shell selected-state coordination without a separately proven request/result or presenter boundary. Prefer real correction-loop evidence or a current-build operator UX issue next.

## 2026-07-17 P98 Recipe Local Validation-Set Dashboard Presentation Ownership

- Audited the local validation-set path before editing. Validation-set persistence, file/folder registration, missing-path repair, explicit suite execution, and XML acceptance/calibration evidence are still Host/storage responsibilities. Four dashboard outputs only consume already selected DTO/state: expected-role counts, selected-set summary, next-action order, and Validation Suite top summary.
- Added `Wpf\Recipe\Review\OpenVisionRecipeValidationSetPresenter`. The Host retains storage/load/command/selection lifecycle; the presenter receives only selected state and formats the four dashboard outputs. It cannot persist validation sets, load pipeline XML, execute Preview/Run, change layers/routes, or mutate XML/recipes.
- The split deliberately leaves `BuildValidationSetAcceptanceText` and `BuildValidationSetCalibrationText` with the Host because they load the active pipeline and calculate gate/physical-unit evidence. Do not move them as a cosmetic file-length change.
- OpenVisionReadinessCheck rejects restoration of the four Host presentation methods, requires the Presenter methods, and rejects validation-set storage, pipeline XML loading, or execution behavior from the presenter.
- Fresh solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs` passed under `artifacts\p98_recipe_validation_set_presenter_20260717\final_build_exe_recipe_manager_tabs`; its report confirms `LocalValidationSet`, `ValidationSuite`, saved Step report, and explicit Preview/Run-free behavior. Readiness, external-reference, public-sample (`30/229/15`), `P98_SOURCE_OWNERSHIP`, and `git diff --check` passed; diff check emitted existing CRLF normalization warnings only.
- P98 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not split validation-set persistence, Recipe Manager lifecycle orchestration, Shell selection coordination, or acceptance/calibration evidence without a new explicit request/result boundary. Prefer real correction-loop evidence or a current-build UX issue next.

## 2026-07-17 P99 Guided Intent Feedback Presentation Ownership

- Audited Pin-gap, Blob, and Contour Guided Setup feedback before editing. The seven latest-run/calibration/advice methods consume only the selected sample DTO, current gate fields, and Pin-gap unit mode; they do not execute, persist, navigate, change selection, or mutate XML/layers/routes.
- Added `Wpf\Recipe\Review\OpenVisionRecipeIntentFeedbackPresenter`. The Host retains all input state, Starter XML creation, selection, stale-draft coordination, and `PropertyChanged`; the presenter formats Pin-gap latest-run/calibration feedback, Blob `ResultCount`, and Contour `ResultCount`/`AreaMax` feedback.
- Existing localization and metric behavior is retained, including the Pin-gap average-only warning, PX-ONLY/MM-READY calibration wording, and explicit sample-run guidance. This is a behavior-neutral ownership refactor; it adds no automatic Preview/Run or recipe/layer/routing side effects.
- OpenVisionReadinessCheck now rejects restoration of the former Host methods and rejects sample execution, pipeline XML loading, and run-history persistence in the new Presenter. It also follows the moved average-only warning token from the Presenter instead of assuming the Host owns it.
- Fresh full solution build passed with 0 warnings/0 errors. The latest `bin\Debug\OpenVisionLab.dll` built at `2026-07-17 19:40:08 KST` passed `--smoke recipe-manager-tabs` under `artifacts\p99_recipe_intent_feedback_presenter_20260717\final_build_exe_recipe_manager_tabs_final`; the report records Pin-gap `DistanceMmAvg`/`DistanceMmRange`, Blob `ResultCount`, Contour `ResultCount`/`AreaMax`, Validation Suite, review/history, and explicit Preview/Run-free behavior.
- Readiness, external-reference, public-sample (`30/229/15`), `P99_SOURCE_OWNERSHIP`, and `git diff --check` passed. P99 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not split Guided Setup commands, recipe lifecycle/selection coordination, validation-set persistence, or acceptance/calibration evidence without another complete request/result or pure presentation boundary. Prefer real correction-loop evidence or a current-build operator UX issue next.

## 2026-07-17 P100 Guided Setup Readiness Presentation Ownership

- Audited `BuildGuidedSetupReadinessText` and `TryBuildGuidedSetupIntentInputStatus` before editing. Both only classify the selected intent, parse the current Guided Setup field values, check optional reference-template existence, and format the required-input/readiness text; they do not create XML, execute a sample, persist history, navigate, or mutate recipe/layer/routing state.
- Added `Wpf\Recipe\IntentSkills\OpenVisionRecipeGuidedSetupReadinessPresenter` with the explicit `OpenVisionRecipeGuidedSetupReadinessInput` and status result. The Host now owns only current-field mapping, PropertyChanged coordination, and command gating; the Presenter owns LineDistance, Blob, Contour, EdgeBasedMatching, FeatureMatching, Matching, and Mean `READY`/`MISSING` evaluation plus guidance text.
- Existing input semantics and localized text are preserved, including Pin-gap PX-ONLY/MM-READY metrics, invalid min/max and Canny ordering, required template paths, optional Mean ROI, and Matching Search ROI/ResultCount readiness. The change introduces no automatic Preview/Run, recipe import, layer/routing update, XML mutation, or Starter XML creation.
- OpenVisionReadinessCheck now rejects restoration of both former Host methods, requires delegation and the read-only input contract, and rejects sample execution, pipeline XML load, run-history persistence, and starter-pipeline creation in the Presenter. `P100_SOURCE_OWNERSHIP` passed.
- Fresh solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` built at `2026-07-17 20:03:01 KST` passed `--smoke recipe-manager-tabs` under `artifacts\p100_guided_setup_readiness_presenter_20260717\final_build_openvisionlab_exe_recipe_manager_tabs`; the report records all seven Guided Setup Starter XML paths, Pin-gap MM/PX parity, current Recipe Manager review/history behavior, and explicit Preview/Run-free operation.
- Readiness, external-reference, public-sample (`30/229/15`), `P100_SOURCE_OWNERSHIP`, and `git diff --check` passed. P100 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not split Guided Setup command/lifecycle orchestration, Recipe Manager selection/persistence, or acceptance/calibration evidence without a newly proven pure request/result or controller boundary. Return to real correction-loop evidence or current-build novice UX evidence before extracting another Host method.

## 2026-07-17 P101 Guided Workflow Presentation Ownership

- Audited the Guided Setup strip and its next-action flow before editing. The Host duplicated the same ordered conditions for the visible instruction and for choosing the command delegate. This created a real consistency risk while the policy itself used only supplied recipe/sample/pair state and existing command availability.
- Added `Wpf\Recipe\Review\OpenVisionRecipeGuidedWorkflowPresenter`, request contract, and action enum. The presenter owns setup-strip formatting, one ordered next-action decision, and its label. The Host still maps current state, owns `Can...` command predicates, and performs the explicit switch that invokes the existing Validate, Duplicate, Activate, Sample Check, parameter load, pair check, or Tool open command.
- No execution behavior changed: the new presenter cannot access WPF, storage, pipeline creation, XML mutation, recipe mutation, layers/routes, clipboard, or Preview/Run. Opening/selecting Recipe Manager remains read-only; command execution remains explicit.
- OpenVisionReadinessCheck rejects restoration of the three former Host helpers and rejects pipeline execution/storage/create/clipboard APIs in the presenter. `P101_SOURCE_OWNERSHIP` passed.
- Fresh full solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` built at `2026-07-17 20:14:35 KST` passed `recipe-manager-tabs` under `artifacts\p101_guided_workflow_presenter_20260717\final_build_openvisionlab_exe_recipe_manager_tabs`; it retained Guided Setup intent/workflow behavior, operator decision summary, review/history links, and explicit Preview/Run contracts.
- P101 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not split Guided Setup command/lifecycle orchestration, Recipe Manager selection/persistence, acceptance/calibration evidence, or Host command execution without a separately proven controller boundary. First audit remaining lifecycle validation presentation; if it lacks a complete pure boundary, return to real correction-loop evidence or current-build novice UX evidence.

## 2026-07-17 P102 Recipe Lifecycle Validation Presentation Ownership

- Audited the two Recipe Manager name-guidance paths before editing. They only classify current recipe/pipeline selection, blank/invalid/normalized requested names, collision state, and the final localized guidance; lifecycle commands and storage calls are separate.
- Added `Wpf\Recipe\Review\OpenVisionRecipeLifecycleValidationPresenter` with recipe-edit and pipeline-edit request contracts. The Host maps current selection/list state and the pre-existing normalized pipeline name, while the Presenter formats all validation guidance. Workspace/pipeline creation, duplicate, rename, delete, selection refresh, and command execution stay in the Host.
- Existing branch order is preserved, including no selected recipe/pipeline, blank name, invalid name, invalid-character replacement notice, selected-only item guidance, duplicate names, and available-name guidance. `HasSelectedPipelineOption` retains the old option-null behavior without relying on a pipeline-name string.
- The Presenter has no WPF, workspace mutation, pipeline storage, XML/recipe mutation, layer/routing mutation, or Preview/Run execution. It may use the existing Core name-validity rule only. Opening or typing in Recipe Manager still does not run Preview/Run.
- OpenVisionReadinessCheck rejects restored Host methods and lifecycle/storage/execution APIs in the Presenter. `P102_SOURCE_OWNERSHIP` passed.
- Fresh full solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` built at `2026-07-17 20:24:07 KST` passed `recipe-manager-tabs` under `artifacts\p102_recipe_lifecycle_validation_presenter_20260717\final_build_openvisionlab_exe_recipe_manager_tabs`; it retained summary/advanced Recipe Manager modes, lifecycle commands, Guided Setup, review/history, and explicit Preview/Run contracts.
- P102 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not split Recipe Manager lifecycle execution, workspace/pipeline persistence, Host selection coordination, acceptance/calibration evidence, or command predicates without a separately proven Controller/Service boundary. First perform one remaining Host call-path audit; if no cohesive non-mutating boundary remains, end mechanical refactoring and return to product evidence/UX work.

## 2026-07-17 P103 Stored Pipeline XML Validation Report Ownership

- Audited `BuildLlmXmlValidationReport` before editing. It uses already-loaded selected-pipeline XML state only, then formats the XML/load status, assumed input layer, pipeline/Step count, file-name mismatch warning, schema/routing evidence, and bounded error/warning rows. The Host owns the storage load but not the report construction.
- Added `Wpf\Recipe\Validation\OpenVisionRecipeStoredPipelineValidationReportBuilder` with a request contract. The Host now passes pipeline path, XML load status/message, and loaded pipeline; the Builder owns report composition and `VisionPipelineValidator` formatting.
- No execution behavior changed. The Builder has no pipeline storage, WPF, Preview/Run, layer/routing mutation, XML mutation, or recipe mutation dependency. Recipe Manager selection/refresh continues to be read-only.
- OpenVisionReadinessCheck rejects restoration of the Host report method and storage/execution/WPF APIs in the Builder. `P103_SOURCE_OWNERSHIP` passed.
- Fresh full solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` built at `2026-07-17 20:30:10 KST` passed `recipe-manager-tabs` under `artifacts\p103_stored_pipeline_validation_report_builder_20260717\final_build_openvisionlab_exe_recipe_manager_tabs`; it retained Recipe Manager summary/advanced modes, LLM XML evidence, Guided Setup, review/history, and explicit Preview/Run contracts.
- P103 is not committed or pushed. The Original repository remains untouched.
- Post-P103 audit completed: `1. Core`, `0. UI\0) MENU`, `0. UI\6) Vision Test`, and `0. UI\6) Vision Test\Wpf` have no direct C#/XAML files. `0. UI\0) MENU\Wpf` has only the approved Host composition files. The remaining Host methods are genuine selection/session coordination, storage/lifecycle, explicit execution, dialog/clipboard/viewer callbacks, or current-session LLM prompt/review-bundle assembly.
- Do not extract isolated review-reference, catalog-formatting, or ROI lookup helpers solely for line count. No further cohesive non-mutating Host boundary was found in this audit. End mechanical source refactoring here and return to product evidence/UX work unless a future call-path audit proves a Controller/Service boundary.

## 2026-07-17 P104 Recipe Summary Next-Action Visibility

- Current EXE Recipe Manager review found no visible clipping/overlap in Summary, Guided Setup, Pipeline Review, Report, or Run History. The only evidence-based novice friction was that the Summary action `파이프라인 열기` looked like a secondary outline control even though it is the required next step into the existing Pipeline workflow.
- Reused the existing `OpenPipelineReviewCommand`; changed only its localized label to `다음: 파이프라인 열기` / `Next: Open Pipeline` and rendered the existing Summary button as the teal primary action with a localized accessible name. No command routing, Preview/Run, layer, recipe, XML, or input/output behavior changed.
- Fresh before evidence: `artifacts\p103_stored_pipeline_validation_report_builder_20260717\final_build_openvisionlab_exe_recipe_manager_tabs\OpenVisionLab_RecipeManager_Summary.png`. Final after evidence: `artifacts\p104_recipe_summary_next_action_20260717\final_rebuilt_exe_recipe_manager_tabs\OpenVisionLab_RecipeManager_Summary.png`. The after capture shows a visible primary button with no text clipping.
- Fresh full solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` built at `2026-07-17 20:38:36 KST` passed `recipe-manager-tabs` under `artifacts\p104_recipe_summary_next_action_20260717\final_rebuilt_exe_recipe_manager_tabs`; it retained Summary/Advanced Review separation, validation/review/history behavior, and explicit Preview/Run contracts.
- OpenVisionReadinessCheck now requires the localized next-action label and the accessible-name binding. P104 is not committed or pushed. The Original repository remains untouched.
- Next priority: real GPT/Gemini/Claude correction-loop transcript validation when supplied. Without a real transcript, capture a current-build novice workflow only when a visible issue is observed; do not resume mechanical source splitting without a proved boundary.

## Cautions

- UI/UX changes require fresh current-build before/after screenshots. Do not reuse old screenshots.
- `PipelineViewerScreenshotSmoke` can hang when multiple WPF targets are run in one process. Use `tools\RunSampleReviewUiSmokes.ps1` or single-target runs.
- Do not run WPF smoke targets in parallel; `OpenCvSharpExtern.dll` lock warnings can appear.
- Do not bulk-copy Dev into Original.
- Do not restore GitHub Desktop stashes unless the user explicitly asks.
- Do not reintroduce SDK sample assets or `dll\Library-Noah\OpenCvSharpExtern.dll` into public paths.
