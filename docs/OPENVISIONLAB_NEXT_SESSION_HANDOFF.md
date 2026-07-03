# OpenVisionLab Next Session Handoff

Updated: 2026-07-03 20:01 KST

This document is the minimum handoff needed to continue without re-discovering the current state. Work starts in `C:\Git\OpenVisionLab_Dev`; only reviewed and stabilized changes are imported into the original repo at `C:\Git\OpenVisionLab`. Do not run `git push` unless the user explicitly requests `PUSH`.

## Product Direction

- OpenVisionLab is an OpenCvSharp4-based rule-based vision workbench.
- Its purpose is image-based algorithm learning, verification, and recipe composition.
- It is not a camera, lighting, PLC, or I/O integration platform.
- Algorithm tools must stay PropertyGrid-based.
- Preview/Run must be explicit user actions. Layer create/delete/load-image, visibility toggles, and output layer creation must not auto-run tools.
- Viewer zoom/pan/drag, ROI overlay, template editor, layer comparison, and docking features must be preserved.

## Latest Original Repo Commits

`C:\Git\OpenVisionLab` is clean at these latest stable commits:

- `6ca54d3 Add public sample review smoke runner`
- `dabf398 Localize pipeline review result count metric`
- `9b20cec Record latest sample catalog audit`
- `2ed377a Move line test configuration into controller`
- `61466b0 Update handoff with final review smokes`
- `031c347 Update OpenVisionLab self evaluation evidence`
- `811c2b2 Update handoff after final catalog check`
- `4278e43 Trim unused filter morphology usings`
- `da392e8 Update handoff after pipeline copy polish`
- `5f76663 Shorten pipeline review next-action copy`
- `853da22 Update handoff after review smoke restore`
- `0a2e026 Restore filter morphology layout smoke`
- `567fefc Share kernel preset click handling`
- `8c6c992 Update OpenVisionLab handoff evidence`
- `01c7aa4 Fix Korean duplicate key detection`
- `bab969e Clear stale contour review before teaching preview`
- `9966a81 Update handoff after sample review checks`
- `b0da050 Allow product sample NG review smoke`
- `b011ee2 Expose pair review hint in sample workflow`
- `667e454 Update OpenVisionLab handoff status`
- `e98a0b2 Clarify NG review fix guidance`

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

## Verification Evidence

- Dev build:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed.
- Original build:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed.
- Readiness:
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab"` passed.
- Reference and sample policy checks:
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed in Dev and Original.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed in Dev and Original.
  - Re-run at 2026-07-03 19:27 KST passed in Dev and Original.
- Product sample full catalog:
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\original_product_catalog_full_20260703_1919` passed in Original.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_final_20260703_1920` passed in Original.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_after_line_controller_cleanup_20260703_1935` passed in Original.
- Sample review UI smoke runner:
  - Dev current-flow evaluation: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_user_flow_eval_20260703_1959` passed.
  - Dev: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_script_after_auditfix_20260703_1918` passed.
  - Original: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\original_sample_review_ui_smoke_script_after_auditfix_20260703_1919` passed.
  - Original re-run after layout smoke restore: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_after_layout_guard_restore_20260703_1903` passed.
  - Original re-run after Pipeline Review copy shortening: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_after_pipeline_copy_short_20260703_1915` passed.
  - Original final re-run: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_final_20260703_1930` passed.
  - Product sample NG after `ResultCount` wording: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\product_sample_review_ng_metric_display_after_original_20260703_1948` passed.
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

## Next Priorities

1. Pipeline/Recipe operator review UX polish
   - Current OK/NG smoke passes and top-card copy now fits. Next value is step-level clarity: keep failed metric, expected/actual range, suggested fix, and relevant parameter location close together.
2. Tool View code-behind cleanup
   - Filter/Morphology preset handling and Line test configuration have moved into controllers. Continue only where existing controller/runtime patterns already fit; avoid broad base-class or interface refactors.
3. MainView/Product sample review flow
   - Current six-target sample review smoke passes. Re-check actual screenshots before changing UI copy or layout.
4. Product sample catalog quality
   - Current 84-pair audit and final 168-row catalog gate are PASS, including the post-Line-cleanup rerun. More samples are lower priority than improving explanation and review affordance.

## Cautions

- UI/UX changes require fresh current-build before/after screenshots. Do not reuse old screenshots.
- `PipelineViewerScreenshotSmoke` can hang when multiple WPF targets are run in one process. Use `tools\RunSampleReviewUiSmokes.ps1` or single-target runs.
- Do not run WPF smoke targets in parallel; `OpenCvSharpExtern.dll` lock warnings can appear.
- Do not bulk-copy Dev into Original.
- Do not restore GitHub Desktop stashes unless the user explicitly asks.
- Do not reintroduce SDK sample assets or `dll\Library-Noah\OpenCvSharpExtern.dll` into public paths.
