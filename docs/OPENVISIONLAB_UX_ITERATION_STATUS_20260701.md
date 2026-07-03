# OpenVisionLab UX Iteration Status - 2026-07-01

This note records the before-08:00 self-iteration status for the current beginner-friendly rule-based vision workbench UX direction.

## Completed

- MainView image-ready next-action bar.
  - Generic image load now shows the next action directly under the workspace image.
  - Sample pipeline workflow remains higher priority and replaces the generic bar.
  - Before: `artifacts\mainview_before_20260701\wpf_shell_host_workspace_image_load.png`
  - After: `artifacts\mainview_after_20260701_r3\wpf_shell_host_workspace_image_load.png`
- MainView top status banner synchronization.
  - Empty, image-ready, and sample-ready states now show matching top banner wording.
  - The banner remains display-only and does not open tools, run Preview, create output layers, or change routing.
  - Before: `artifacts\mainview_after_20260701_r3\wpf_shell_host_workspace_image_load.png`
  - After: `artifacts\mainview_status_after_20260701_r2\wpf_shell_host_workspace_image_load.png`
- MainView quick action click smoke.
  - Threshold, Matching, and Line buttons now have a smoke target that executes the actual commands.
  - The smoke verifies tool open, `Main` input routing, and no automatic Preview.
  - After: `artifacts\mainview_quick_actions_20260701\wpf_shell_host_workspace_quick_actions.png`
- MainView empty/image/tool-selected microcopy localization.
  - Empty workflow, image-ready quick actions, sample-ready top status, and tool-selected status now use localization-backed wording.
  - Korean mode uses `미리보기 확인` instead of mixed `Preview 확인`.
  - Korean/English language switching refreshes visible MainView text without opening tools, running Preview/Run, creating layers, or changing routes.
  - Before: `artifacts\mainview_microcopy_before_20260701\wpf_shell_host_workspace_empty.png`, `artifacts\mainview_microcopy_before_20260701\wpf_shell_host_workspace_image_load.png`, `artifacts\mainview_microcopy_before_20260701\wpf_shell_host_workspace_quick_actions.png`
  - After: `artifacts\mainview_microcopy_after_20260701\wpf_shell_host_workspace_empty.png`, `artifacts\mainview_microcopy_after_20260701\wpf_shell_host_workspace_image_load.png`, `artifacts\mainview_microcopy_after_20260701\wpf_shell_host_workspace_quick_actions.png`
- P1 Contour compact verification guide.
  - Display-only guide/result guidance added without replacing PropertyGrid.
  - Before: `artifacts\ux_contour_guide_before_20260701\wpf_shell_host_contour_tool.png`
  - After: `artifacts\ux_area_guides_after_20260701\wpf_shell_host_contour_tool_docked_verification.png`
- P2 Blob compact verification guide.
  - Reuses `VisionToolAreaVerificationGuidePresenter` and `VisionToolAreaVerificationCriteriaText`.
  - Before: `artifacts\ux_blob_guide_before_20260701\wpf_shell_host_blob_tool.png`
  - After: `artifacts\ux_area_guides_after_20260701\wpf_shell_host_blob_tool_docked_verification.png`
- P2 Line compact verification guide.
  - Uses `LineToolVerificationGuidePresenter`.
  - Displays verification state in the shared summary strip to preserve docked PropertyGrid height.
  - After: `artifacts\ux_line_guide_after_20260701_r9\wpf_shell_host_line_tool_docked_verification.png`
- Shell status synchronization after docked tool reselection.
  - Reselecting a docked Blob tool now restores the Shell top `OK` result state when the cached native document still has a displayable output layer.
  - Cross-tool docked reselection `Blob -> Contour -> Blob` restores the Blob result in-place without opening a duplicate floating window.
  - Large PropertyGrid tools also reopen above their usable editor height even if an older saved floating-window bound is too short.
  - Before: `artifacts\ux_area_guides_after_20260701\wpf_shell_host_blob_tool_docked_verification.png`
  - After: `artifacts\status_reselect_after_20260701\wpf_shell_host_blob_tool.png`
- Pipeline Review beginner guide strip.
  - Adds a display-only selected-step review strip for review position, current route, next check, and decision.
  - Run Review remains the only execution path; selecting Pipeline Review or steps does not run Preview/Review.
  - Before: `artifacts\pipeline_review_before_20260701\wpf_shell_host_pipeline_review.png`
  - After: `artifacts\pipeline_review_after_20260701\wpf_shell_host_pipeline_review.png`
- Pipeline Review multi-step readability.
  - The smoke sample now uses a 3-step pipeline with a real branch route so beginners can see normal sequential flow and branch flow side by side.
  - The guide adds a localized detail row and explicit previous/next step selection.
  - Korean/English language changes recalculate the guide text through localization keys without reopening the selected Pipeline Review document.
  - Previous/next selection and language refresh remain display/selection operations only; Review still requires the explicit Run Review command.
  - Before: `artifacts\pipeline_review_readability_before_20260701\wpf_shell_host_pipeline_review.png`
  - After: `artifacts\pipeline_review_readability_after_20260701\wpf_shell_host_pipeline_review.png`
- Pipeline Review acceptance NG visual coverage.
  - Added a focused `wpf_shell_host_pipeline_review_ng` smoke target for the case where a tool executes successfully but fails its acceptance metric.
  - Metric-based acceptance NG reasons now use Pipeline Review localization before falling back to lower-level diagnostics.
  - The target verifies NG decision text, localized metric-target reason, next-action guidance, run-log context, and retained failed-step output preview.
  - Before: `artifacts\pipeline_review_ng_before_20260701\wpf_shell_host_pipeline_review.png`
  - After: `artifacts\pipeline_review_ng_after_20260701\wpf_shell_host_pipeline_review_ng.png`
- EdgeBasedMatching compact verification guidance.
  - EdgeBasedMatching now shows a display-only compact guide with edge-specific state, Canny/search/point criteria, Preview OK/NG, and next action.
  - PropertyGrid remains the parameter source; the guide does not trigger Preview/Run/Add Pipeline.
  - Before: `artifacts\edge_based_matching_before_20260701\wpf_shell_host_edge_based_matching_tool.png`
  - After: `artifacts\edge_based_matching_after_20260701\wpf_shell_host_edge_based_matching_tool.png`
- Tool View/MainView terminology consistency pass.
  - MainView image-ready guidance now uses beginner-facing `미리보기` / `파이프라인` wording consistently.
  - Blob/Contour/Matching/Line result review summaries and chips use shared labels for decision, criteria, count, score, center, box, distance, length, and next action.
  - Blob/Contour parameter summary rows reuse the same display criteria formatter as the compact guide.
  - Before: `artifacts\consistency_before_20260701\wpf_shell_host_workspace_image_load.png`, `artifacts\consistency_before_20260701\wpf_shell_host_blob_tool.png`, `artifacts\consistency_before_20260701\wpf_shell_host_matching_tool.png`, `artifacts\consistency_before_20260701\wpf_shell_host_line_measure_tool.png`
  - After: `artifacts\consistency_after_20260701\wpf_shell_host_workspace_image_load.png`, `artifacts\consistency_after_20260701\wpf_shell_host_blob_tool.png`, `artifacts\consistency_after_20260701\wpf_shell_host_matching_tool.png`, `artifacts\consistency_after_20260701\wpf_shell_host_line_measure_tool.png`
- Tool View remaining terminology pass.
  - Matching template state now displays localized `템플릿 준비` wording instead of mixed English status text.
  - Matching-family summary rows now combine pass criteria with image-process state such as `원본` and `전체 이미지`.
  - FeatureMatching now has the same compact guide parity as Matching/EdgeBasedMatching, including Ratio/RANSAC criteria.
  - Line purpose controls now display `목적`, `라인`, `엣지`, `측정`, and `교차` while retaining the existing Line A/B internal/test identifiers.
  - Before: `artifacts\terminology_before_20260701\wpf_shell_host_matching_tool.png`, `artifacts\terminology_before_20260701\wpf_shell_host_feature_matching_tool.png`, `artifacts\terminology_before_20260701\wpf_shell_host_line_measure_tool.png`
  - After: `artifacts\terminology_after_20260701\wpf_shell_host_matching_tool.png`, `artifacts\terminology_after_20260701\wpf_shell_host_feature_matching_tool.png`, `artifacts\terminology_after_20260701\wpf_shell_host_line_measure_tool.png`

- Competitor review and sample benchmark strip.
  - Checked Cognex In-Sight EasyBuilder, MVTec MERLIC, NI Vision Builder AI, and Zebra Aurora Vision Studio public materials.
  - Priority changed from more MainView layout work to the sample/inspection benchmark loop.
  - Sample Catalog selected-sample details now show whether the sample is an OK reference, NG reference, or generic OK criteria case before opening it.
  - The strip also shows the expected criteria and Good/Bad pair or single-sample context.
  - Opening a sample remains preparation-only: no Preview, Run, tool open, output layer creation, or route mutation is triggered by the display change.
  - Before: `artifacts\sample_benchmark_before_20260701\wpf_shell_host_workspace_sample_picker.png`
  - After: `artifacts\sample_benchmark_after_20260701\wpf_shell_host_workspace_sample_picker.png`
- Beginner Learn Mode and Good/Bad pair comparison pass.
  - Sample Catalog now surfaces Learn Mode, recommended start, result interpretation, and failure-cause guidance for the selected sample.
  - Good/Bad pair rows now show a compact pair-comparison strip with OK/NG counts and the opposite reference sample.
  - Added `docs\OPENVISIONLAB_BEGINNER_LEARN_MODE_AND_RECIPE_CONTEXT_20260701.md` to capture Learn Mode, presets, result explanation, failure causes, Good/Bad pair expansion, and multi-recipe context switching as implementation priorities.
  - This pass is display-only: no Preview, Run, tool open, output layer creation, or route mutation is triggered.
  - Before: `artifacts\sample_pair_before_20260701\wpf_shell_host_workspace_sample_pair_picker.png`
  - After: `artifacts\sample_learn_after_20260701\wpf_shell_host_workspace_sample_pair_picker.png`

## Verification

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS.
- Localization duplicate-key check: PASS.
- MainView focused smoke: PASS at `artifacts\mainview_after_20260701_r2` and `artifacts\mainview_after_20260701_r3`.
- MainView top status focused smoke: PASS at `artifacts\mainview_status_after_20260701_r2`.
- MainView quick action focused smoke: PASS at `artifacts\mainview_quick_actions_20260701`.
- MainView microcopy localization smoke: PASS at `artifacts\mainview_microcopy_after_20260701`.
- Blob/Contour focused smoke: PASS at `artifacts\ux_area_guides_after_20260701`.
- Line focused smoke: PASS at `artifacts\ux_line_guide_after_20260701_r9`.
- Blob/Contour/Line docked guide regression smoke: PASS at `artifacts\ux_guides_regression_20260701`.
- Docked Blob reselection/status smoke: PASS at `artifacts\status_reselect_after_20260701`.
- Pipeline Review beginner guide smoke: PASS at `artifacts\pipeline_review_after_20260701`.
- Pipeline Review multi-step readability smoke: PASS at `artifacts\pipeline_review_readability_after_20260701`.
- Pipeline Review acceptance NG smoke: PASS at `artifacts\pipeline_review_ng_after_20260701`.
- EdgeBasedMatching compact guide smoke: PASS at `artifacts\edge_based_matching_after_20260701`.
- MainView/Tool View terminology consistency smoke: PASS at `artifacts\consistency_after_20260701`.
- Tool View remaining terminology smoke: PASS at `artifacts\terminology_after_20260701`.
- Sample benchmark strip smoke: PASS at `artifacts\sample_benchmark_after_20260701`.
- Sample Learn Mode and pair comparison smoke: PASS at `artifacts\sample_learn_after_20260701`.

## Self-Evaluation

- The guide pattern now covers Matching, Contour, Blob, and Line without replacing model-driven PropertyGrid editing.
- Area-style tools share one presenter path; Line remains separate because its Edge/Measure/Intersection modes have different result semantics.
- Line initially failed docked density because an extra guide row reduced PropertyGrid height. The final design uses the summary strip and keeps Purpose/Setting controls compact.
- The Shell top status now follows the active native document after docked tool reselection. The fix is intentionally state-only and does not treat tab/tool selection as an implicit Preview command.
- MainView microcopy now follows the same localized display-only rule as the tool guides. The smoke explicitly flips Korean/English while empty/image-ready states are visible and confirms no tool window opens.
- Pipeline Review now explains the selected step's route and post-run decision without replacing the existing flow/preview/result panels. The guide is state-only, and the smoke explicitly checks that pre-run state does not present itself as OK.
- Pipeline Review now covers the more realistic case where a later step reads from a branch input instead of the previous output. The UI makes that route visible in the flow, the guide detail row, and validation/result panels without treating branch selection or language switching as an execution command.
- Pipeline Review now has explicit visual coverage for acceptance NG. The sample keeps the failed output image visible and checks that the guide tells the operator which metric target failed in the active language instead of showing only a generic NG state.
- EdgeBasedMatching now tells beginners what is being judged in edge terms instead of using only generic matching wording. The visible compact line stays dense, while full criteria remain available through tooltip text.
- The consistency pass reduced mixed English/Korean in the primary learning path without changing PropertyGrid ownership, Preview execution, routing, or pipeline creation.
- The remaining terminology pass closes the most visible mixed-language gaps in Matching, FeatureMatching, and Line without altering model properties, route selection, layer creation, or Preview triggers.
- The competitor review shows OpenVisionLab is strongest when it keeps the image and PropertyGrid tools intact while making the inspection benchmark loop explicit. The sample picker now tells beginners what reference they are about to load before they enter Pipeline Review.
- The new Learn Mode strip addresses the beginner gap directly: the sample now explains what to learn, where to start, how to read the metric, and what failure family to inspect before the user opens the sample. Full tool presets and multi-recipe context are intentionally left as explicit follow-up work because they need model/service contracts, not only UI labels.

## Next Priority

1. Define and implement recipe context switching so different inspections can use different recipe contexts without hidden global state mutation.
2. Add explicit tool preset contracts for `기본 검사`, `빠른 검사`, and `정밀 검사` while preserving PropertyGrid ownership and manual Preview/Run.
3. Expand sample-backed OK/NG coverage where a real operator workflow exposes a missing visual contract.
