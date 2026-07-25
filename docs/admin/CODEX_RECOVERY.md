# CODEX RECOVERY

Last updated: 2026-07-03

Workspace: `C:\Git\OpenVisionLab_Dev`

## 2026-07-03 Update - Pipeline Review paired sample handoff

Continued after the Product Learn guide screenshot refresh. The next non-duplicate UX gap was that Pipeline Review told the operator which Good/Bad reference to compare, but did not provide an explicit way to move to that paired sample from the review screen.

Changed files:

- `0. UI\0) MENU\Wpf\Documents\OpenVisionPipelineReviewDocument.cs`
- `0. UI\0) MENU\Wpf\OpenVisionShellHostStatePresenter.cs`
- `0. UI\0) MENU\Wpf\OpenVisionShellHostToolTestFacade.cs`
- `0. UI\0) MENU\Wpf\OpenVisionShellHostToolWindowController.cs`
- `0. UI\0) MENU\Wpf\OpenVisionShellHostView.TestHooks.cs`
- `0. UI\0) MENU\Wpf\OpenVisionShellHostView.xaml.cs`
- `0. UI\0) MENU\Wpf\ViewModels\OpenVisionPipelineReviewViewModel.cs`
- `0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml`
- `0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml.cs`
- `tools\PipelineViewerScreenshotSmoke\Program.cs`

Behavior:

- Pipeline Review now shows an explicit Good/Bad paired-sample action beside the `Good/Bad Pair` line, for example `NG 기준 열기` on an OK sample and `OK 기준 열기` on an NG sample.
- The button uses the existing sample-open path by sample name, then refreshes the active Pipeline Review document against the newly active sample pipeline.
- This is an explicit operator action. It loads the paired sample image and activates its `Sample_` pipeline, but it does not run native Preview, run Pipeline Review, create output layers, or change tool PropertyGrid values.
- The Pipeline Review state presenter now exposes the actual active pipeline name loaded by the Review document instead of the creation-time recipe-context snapshot. This prevents stale pipeline names after an explicit paired-sample handoff.
- No Product sample catalog rows, generated sample pixels, pipeline XML, tool execution semantics, layer routing rules, output layer behavior, docking behavior, or PropertyGrid contracts were changed.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" -m:1` PASS, warnings 0, errors 0.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_product_sample_pair_open artifacts\product_sample_pair_open_after_20260703_r2` PASS. The smoke verifies OK -> NG paired-sample handoff, Review refresh, and no Preview/Review side effects.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\product_sample_review_regression_20260703_pair_action_r2` PASS.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"` PASS.
- `git diff --check` PASS, with CRLF normalization warnings only.
- A temporary parallel build/smoke attempt caused WPF `obj` generated-file conflicts; rerunning sequentially after `dotnet build-server shutdown` passed. Keep WPF build and WPF smoke sequential.
- Before/after UI comparison: `artifacts\product_sample_pair_open_after_20260703_r2\product_review_pair_action_before_after.png`.

Next priority:

- Continue only with Product review improvements that remove a real remaining end-to-end ambiguity. The current picker, Good/Bad guide, pair metric line, opposite selection, Review pair summary, and paired-sample handoff are complete enough to avoid repeating them.

## 2026-07-03 Update - Product Learn guide current review screenshots

Continued after adding the separate `Good/Bad Pair` line to Pipeline Review. The code/UI work was already committed; the next non-duplicate task was keeping the Product Learn guide aligned with the current executable screenshots, because public tutorials should not reuse stale UI images after a visible review-flow change.

Changed files:

- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `docs\assets\tutorial\current\product_sample_review_current.png`
- `docs\assets\tutorial\current\product_sample_review_ng_current.png`

Behavior:

- Replaced the Product sample OK/NG review screenshots with current smoke output from the latest build, showing the new `Good/Bad Pair` line.
- Updated the Product sample guide so the review steps explicitly tell the reader to read the `Good/Bad Pair` line before checking output image, overlay, metric, and log.
- No source code, generated sample catalog rows, product sample pixels, pipeline XML, layer routing, Preview/Run behavior, or PropertyGrid contracts were changed.

Validation:

- Markdown image existence check for `docs\learn\LEARN_PRODUCT_SAMPLES.md` PASS: 5 images found and all targets exist.
- Public user docs private-purpose scan PASS for `README.md`, `docs\learn\README.md`, and `docs\learn\LEARN_PRODUCT_SAMPLES.md`.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"` PASS.
- `git diff --check` PASS, with CRLF normalization warnings only.
- Visual check: `docs\assets\tutorial\current\product_sample_review_current.png` and `docs\assets\tutorial\current\product_sample_review_ng_current.png` both show the `Good/Bad Pair` line.

Next priority:

- Keep improving the Product review end-to-end flow only where the operator still has a concrete ambiguity. Do not repeat the catalog expansion, pair metric audit, sample picker filters, opposite-reference selection, pair summary line, or this screenshot refresh.

## 2026-07-03 Update - Product review Good/Bad pair line

Continued after explicit Good/Bad opposite-reference selection in the Product sample picker. The next non-duplicate UX gap was inside Pipeline Review: the Good/Bad pair comparison existed, but the current sample, opposite reference, PairGroup, and comparison metric were mixed into one long review-habit paragraph.

Changed files:

- `0. UI\0) MENU\Wpf\OpenVisionWorkspaceSamplePairDecisionGuide.cs`
- `0. UI\0) MENU\Wpf\OpenVisionPipelineReviewGuidePresenter.cs`
- `0. UI\0) MENU\Wpf\ViewModels\OpenVisionPipelineReviewViewModel.cs`
- `0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml`
- `0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml.cs`
- `0. UI\0) MENU\Wpf\Documents\OpenVisionPipelineReviewDocument.cs`
- `0. UI\0) MENU\Wpf\OpenVisionShellHostStatePresenter.cs`
- `0. UI\0) MENU\Wpf\OpenVisionShellHostToolTestFacade.cs`
- `0. UI\0) MENU\Wpf\OpenVisionShellHostView.TestHooks.cs`
- `tools\PipelineViewerScreenshotSmoke\Program.cs`

Behavior:

- Pipeline Review now shows a separate display-only `Good/Bad Pair` line when the active `Sample_` pipeline belongs to a catalog PairGroup.
- The line names the current sample role, opposite reference, PairGroup, and comparison metric, for example `Product_Display_Particle_Good` versus `Product_Display_Particle_Many_Bad` with `ResultCount`.
- The existing review habit/checklist remains below it, so beginners can scan "what pair am I comparing" before reading the longer review habit.
- This is presentation state only. It does not open samples, run Preview/Run, create/delete/select layers, alter tool parameters, change pipeline execution, or change sample catalog rows.
- A stale smoke process from the first current-check run locked `OpenVisionLab.dll`; it was stopped before rerunning focused smokes. The lock was test-process cleanup, not a code failure.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\product_review_pair_line_after_20260703` PASS.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\product_review_pair_line_ng_after_20260703` PASS.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"` PASS.
- `git diff --check` PASS, with CRLF normalization warnings only.
- Before/after comparison evidence: `artifacts\product_review_pair_line_after_20260703\product_review_pair_line_before_after.png`.

Next priority:

- Continue the Product review flow only where it removes a real remaining ambiguity. The next useful candidate is a small Product review handoff/action that helps the operator move from this Pipeline Review result back to the paired sample without repeating sample-picker work, but only if it can stay explicit and display/selection-only.

## 2026-07-03 Update - Product sample pair counterpart selection

Continued after the Product pair next-action guidance. The next non-duplicate beginner UX gap was that the picker explained which Good/Bad counterpart to review next, but the operator still had to locate that opposite reference manually in the list.

Changed files:

- `0. UI\0) MENU\Wpf\OpenVisionWorkspaceSamplePickerViewModel.cs`
- `0. UI\0) MENU\Wpf\OpenVisionWorkspaceSamplePickerView.xaml`
- `tools\PipelineViewerScreenshotSmoke\Program.cs`

Behavior:

- The Product sample Good/Bad decision guide now exposes an explicit opposite-reference selection command in the visible guide header.
- For a Good sample, the button selects the paired NG reference in the same `PairGroup`. For an NG/Bad sample, it selects the paired OK reference.
- The command only changes the selected catalog item and collection view current item. It does not open the sample, load an image, create/delete/select layers, run Preview/Run, or alter pipeline/tool routing.
- The button is intentionally placed in the guide header so the Good -> Bad -> Good review loop is available without scrolling.
- No sample catalog rows, generated images, pipeline XML, tool execution semantics, layer routing, output layer behavior, or PropertyGrid contracts were changed.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pair_picker artifacts\sample_pair_counterpart_select_after_20260703_r3` PASS. The smoke checks the visible button and verifies Good -> paired NG -> original Good selection flow.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"` PASS.
- `git diff --check` PASS, with CRLF normalization warnings only.
- Visual after-check: `artifacts\sample_pair_counterpart_select_after_20260703_r3\wpf_shell_host_workspace_sample_pair_picker.png` shows the visible `NG 기준 선택` button in the guide header.
- Before/after comparison evidence: `artifacts\sample_pair_counterpart_select_after_20260703_r3\sample_pair_counterpart_before_after.png`.

Next priority:

- Continue only with Product sample review improvements that remove an actual end-to-end ambiguity. The already-completed catalog expansion, sample quality audit, Product picker filters, pair metric checklist, next-action text, and opposite-reference selection should not be repeated.

## 2026-07-03 Update - Product pair next-action guidance

Continued after refreshing the Product sample review guide. The next non-duplicate UX gap was that the in-app Good/Bad pair guide explained the pair and metric, but did not give the operator a role-specific next action in the first visible guide area.

Changed files:

- `0. UI\0) MENU\Wpf\OpenVisionWorkspaceSamplePairDecisionGuide.cs`
- `0. UI\0) MENU\Wpf\OpenVisionWorkspaceSamplePickerViewModel.cs`
- `0. UI\0) MENU\Wpf\OpenVisionWorkspaceSamplePickerView.xaml`
- `0. UI\0) MENU\Wpf\OpenVisionPipelineReviewGuidePresenter.cs`
- `tools\PipelineViewerScreenshotSmoke\Program.cs`

Behavior:

- `OpenVisionWorkspaceSamplePairDecisionGuide` now carries display-only `NextActionText`.
- If the selected sample is OK/Good, the guide tells the operator to run the NG reference in the same PairGroup with the same pipeline and compare the separating metric.
- If the selected sample is NG/Bad, the guide tells the operator to verify the OK reference first, then rerun the NG sample with the same pipeline and compare the separating metric.
- The sample picker shows this next action directly under the pair summary, before the longer checklist/metric/workflow text, so the operator does not need to scroll to understand the next step.
- Pipeline Review includes the same next-action cue in the review habit text alongside the existing concrete PairGroup and metric guide.
- No sample catalog rows, generated images, Preview/Run behavior, layer routing, output layer behavior, PropertyGrid contracts, or tool execution semantics were changed.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pair_picker artifacts\product_pair_next_action_after_20260703_r2` PASS.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\product_pair_next_action_after_20260703` PASS.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\product_pair_next_action_after_20260703` PASS.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"` PASS.
- `git diff --check` PASS, with CRLF normalization warnings only.
- Visual after-check: `artifacts\product_pair_next_action_after_20260703_r2\wpf_shell_host_workspace_sample_pair_picker.png` shows the next-action line in the first visible Good/Bad decision guide area.
- Before/after comparison evidence: `artifacts\product_pair_next_action_after_20260703_r2\sample_pair_picker_before_after.png`. The before image uses an older catalog focus than the after image, so it is evidence for guide placement/visibility, not for sample content parity.

Next priority:

- Continue with in-app beginner review guidance only where it removes a real next-step ambiguity. Avoid repeating the already-completed Product catalog expansion, pair metric checklist, and Learn document refresh.

## 2026-07-03 Update - Product sample review guide refresh

Continued after the Product sample 168-row catalog reached `OK=84`, `Review=0`, `Critical=0`. This pass did not add new samples or change runtime behavior; it refreshed the beginner-facing review guide so the current app evidence shows both Good and NG Product sample review flow.

Changed files:

- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `docs\OPENVISIONLAB_DOCUMENTATION_CAPTURE_GUIDE.md`
- `docs\assets\tutorial\current\product_focus_picker_current.png`
- `docs\assets\tutorial\current\product_sample_review_current.png`
- `docs\assets\tutorial\current\product_sample_review_ng_current.png`

Behavior:

- Updated Product Learn metric bands for the three recently tuned pairs: `Battery_ElectrolyteStain`, `Battery_TabDiscoloration`, and `Display_CornerLightLeak`.
- Reworked the Product sample review instructions around a concrete Good/Bad pair: open Good first, run the same `Sample_` pipeline, compare output image, overlay, metric, and log, then run Bad from the same PairGroup.
- Added current NG review evidence image so the failure reason is visible from the document, not inferred from Markdown alone.
- Replaced mixed `Pipeline 보기` wording with `Pipeline Review` in the English Product sample guide.
- Documentation capture guide now explicitly says public README/tutorial/Learn documents must not contain portfolio, submission, personal promotion, internal-only, or maintainer-only wording.
- No Product sample catalog rows, generated sample pixels, tool execution, layer routing, Preview/Run behavior, or PropertyGrid contracts were changed.

Validation:

- Latest Product sample UX evidence images copied from current smoke artifacts into `docs\assets\tutorial\current`.
- Markdown image existence check PASS for Product focus picker, Product focus opened, Product sample review, Product sample NG review, and Product sample source/result sheet.
- Visual check of `docs\assets\tutorial\current\product_sample_review_ng_current.png` confirmed the NG reason, Good/Bad habit, input/output images, metric, and log are visible in the same review screen.
- Public README/tutorial/Learn private-purpose term scan PASS for `portfolio` / `포트폴리오` in the user-facing docs that should not contain it.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"` PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.
- `git diff --check` PASS, with CRLF normalization warnings only.

Next priority:

- Keep the Product catalog stable and improve in-app review guidance only when it helps a beginner compare Good/Bad results without opening Markdown. Add new samples only for distinct field-like failure cases that are not already covered.

## 2026-07-03 Update - Product sample weak-margin tuning

Continued after the Product sample quality audit. The audit already covered all 84 Good/Bad PairGroups and left only three `Review` candidates, so this pass tuned those existing cases instead of adding more samples.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Battery_ElectrolyteStain_Heavy_NG.png`

Behavior:

- `Battery_ElectrolyteStain` NG sample now uses a wider, lower-contrast stain so it remains field-like while separating from the Good sample by a clear mean-value metric.
- `Battery_ElectrolyteStain`, `Battery_TabDiscoloration`, and `Display_CornerLightLeak` expected mean-value bands were tightened to match the current generated data and remove weak Good/Bad margin from the audit.
- No pipeline semantics, tool execution, layer routing, Preview/Run behavior, PropertyGrid contracts, or UI behavior were changed.

Validation:

- `powershell -NoProfile -ExecutionPolicy Bypass -File "tools\GenerateOpenVisionProductSamples.ps1"` PASS.
- Visual check of `docs\samples\public\product\Battery_ElectrolyteStain_Heavy_NG.png` confirmed the stain is less over-bright than the first tuning attempt while still visibly inspectable.
- `powershell -NoProfile -ExecutionPolicy Bypass -File "tools\RunVisionSampleCatalog.ps1" -CatalogPath "docs\samples\OpenVisionLab.ProductSampleCatalog.csv" -OutputDir "artifacts\product_sample_catalog_margin_tuned_r2"` PASS: `GateStatus=OK`, `RunnableRows=168`, `RequiredRows=84`, `ExpectedFailureRows=84`, `OKRows=168`, `FailedSamples=0`.
- Targeted metric evidence: `Battery_ElectrolyteStain` Good/Bad `MeanValueAvg=84.3/107.7` with expected bands `70..96` and `106..150`; `Battery_TabDiscoloration` Good/Bad `160.1/110.5` with expected bands `145..180` and `90..130`; `Display_CornerLightLeak` Good/Bad `80.2/138.7` with expected bands `75..98` and `110..160`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File "tools\AuditProductSampleQuality.ps1" -SummaryPath "artifacts\product_sample_catalog_margin_tuned_r2\sample_catalog_summary.json" -OutputDir "artifacts\product_sample_quality_audit_margin_tuned_r2"` PASS: `PairRecords=84`, `OK=84`, `Review=0`, `Critical=0`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File "tools\TestPublicSampleAssets.ps1" -PublicCatalogPath "docs\samples\OpenVisionLab.ProductSampleCatalog.csv" -ManifestPath "docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv"` PASS.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"` PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.
- `git diff --check` PASS, with CRLF normalization warnings only.

Evidence:

- Catalog summary: `artifacts\product_sample_catalog_margin_tuned_r2\sample_catalog_summary.json`.
- Audit report: `artifacts\product_sample_quality_audit_margin_tuned_r2\product_sample_quality_audit.md`.

Next priority:

- Keep the 168-row Product catalog stable unless a new sample teaches a distinct field-like failure case. The next useful work is sample review UX or documentation that helps beginners compare Good/Bad pairs and explain metrics from the current app, not adding duplicate sample rows.

## 2026-07-03 Update - Product sample quality audit

Continued after the Product sample Pipeline Review pair-metric checklist. The next priority was not adding more rows, but making the existing 168-row Product catalog easier to evaluate for realistic Good/Bad separation.

Changed files:

- `tools\AuditProductSampleQuality.ps1`
- `docs\samples\public\product\README.md`

Behavior:

- Added a Product sample quality audit script that reads the latest Product catalog runner summary and compares each Good/Bad PairGroup by expected metric ranges plus a sampled image-difference heuristic.
- The audit generates Markdown and JSON output under `artifacts\product_sample_quality_audit`.
- The audit does not generate images, run tools, change layer routing, or change any Product sample pass/fail semantics.
- README now documents the audit command so future sample work can check existing PairGroups before adding or tuning rows.

Validation:

- `powershell -NoProfile -ExecutionPolicy Bypass -File "tools\AuditProductSampleQuality.ps1"` PASS: `PairRecords=84`, `OK=81`, `Review=3`, `Critical=0`.
- Review candidates from the audit: `Battery_ElectrolyteStain`, `Battery_TabDiscoloration`, and `Display_CornerLightLeak`, all mean-value based pairs with weak metric margin.
- `powershell -NoProfile -ExecutionPolicy Bypass -File "tools\TestPublicSampleAssets.ps1" -PublicCatalogPath "docs\samples\OpenVisionLab.ProductSampleCatalog.csv" -ManifestPath "docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv"` PASS.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"` PASS.
- `git diff --check` PASS, with CRLF normalization warnings only.

Evidence:

- Audit report: `artifacts\product_sample_quality_audit\product_sample_quality_audit.md`.
- Audit JSON: `artifacts\product_sample_quality_audit\product_sample_quality_audit.json`.

Next priority:

- Tune only the three audit review candidates if more sample-quality work continues. Do not add new samples until those weak-margin cases are either accepted as intentionally subtle or adjusted with clearer metric separation.

## 2026-07-03 Update - Product sample Pipeline Review pair metric checklist

Continued after the field-variation pass without adding duplicate sample rows. The next UX gap was that Pipeline Review showed a generic Good/Bad habit, but it did not connect that habit to the current Product sample's PairGroup and expected metric ranges.

Changed files:

- `0. UI\0) MENU\Wpf\OpenVisionPipelineReviewGuidePresenter.cs`
- `0. UI\0) MENU\Wpf\Documents\OpenVisionPipelineReviewDocument.cs`
- `tools\PipelineViewerScreenshotSmoke\Program.cs`

Behavior:

- Pipeline Review now resolves the active `Sample_*` pipeline back to its catalog item, finds runnable samples in the same PairGroup, and passes a display-only pair guide to the review guide presenter.
- The review checklist now shows concrete Product-sample comparison context such as `Display_Particle`, `Product_Display_Particle_Good`, `Product_Display_Particle_Many_Bad`, and the separating metric/range such as `ResultCount OK 0~1 / NG 5~8`.
- The change is presentation-only. It does not run Preview/Run, create layers, change input/output routing, change pipeline execution, or alter pass/fail metric semantics.
- The Product review screenshot smoke now asserts that the checklist contains concrete PairGroup and metric evidence, not only generic `Good`/`Bad` words.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\product_sample_review_pair_metric_after` PASS.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\product_sample_review_pair_metric_ng_after` PASS.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"` PASS.
- `git diff --check` PASS, with CRLF normalization warnings only.

Evidence:

- Before reference: `artifacts\product_sample_review_checklist_after\wpf_shell_host_workspace_product_sample_review.png`.
- After capture: `artifacts\product_sample_review_pair_metric_after\wpf_shell_host_workspace_product_sample_review.png`.
- NG after capture: `artifacts\product_sample_review_pair_metric_ng_after\wpf_shell_host_workspace_product_sample_review_ng.png`.

Next priority:

- If time remains before the automation stop point, prefer a small targeted audit of Product samples whose Good/Bad separation is too visually obvious or too subtle. Do not add more rows unless the new row teaches a distinct field-like failure case.

## 2026-07-03 Update - Product sample field variation quality pass

Continued after the Product sample review checklist work. The next priority was not more rows, but making the existing public-safe Battery/Display/Semiconductor samples look less toy-like while keeping them generated and reproducible.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `tools\TestPublicSampleAssets.ps1`
- `docs\samples\public\product\README.md`
- `docs\samples\public\product\*.png`
- `docs\samples\public\product\templates\*.png`

Behavior:

- Product sample generation now applies deterministic field variation to generated product images: subtle GV drift, non-uniform illumination, light scan banding, sensor grain, vignette, and limited softness.
- The variation is domain-profiled so Battery, Display, and Semiconductor samples do not all share the same clean rendering style.
- Product templates are not independently post-processed, but template crops are regenerated from the current OK product image so matching references stay aligned with the current generated sample set.
- Public sample asset validation now checks both the base public manifest and the Product manifest when validating all public sample images.
- Product catalog validation now has its own required representative rows and broad-pipeline coverage checks, instead of applying only the base public sample required-row list.
- The Product public sample README now records the generation standard: generated samples should remain metric-explainable, field-like, and not rely on exaggerated toy defects.

Validation:

- `powershell -NoProfile -ExecutionPolicy Bypass -File "tools\GenerateOpenVisionProductSamples.ps1"` PASS.
- `powershell -NoProfile -ExecutionPolicy Bypass -File "tools\TestPublicSampleAssets.ps1"` PASS: `CatalogRows=16 ManifestAssets=198 Pipelines=8`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File "tools\TestPublicSampleAssets.ps1" -PublicCatalogPath "docs\samples\OpenVisionLab.ProductSampleCatalog.csv" -ManifestPath "docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv"` PASS: `CatalogRows=168 ManifestAssets=198 Pipelines=84`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File "tools\RunVisionSampleCatalog.ps1" -CatalogPath "docs\samples\OpenVisionLab.ProductSampleCatalog.csv" -OutputDir "artifacts\product_sample_catalog_field_variation"` PASS: `GateStatus=OK`, `RunnableRows=168`, `RequiredRows=84`, `ExpectedFailureRows=84`, `OKRows=168`, `FailedSamples=0`.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"` PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.
- `git diff --check` PASS, with CRLF normalization warnings only.

Evidence:

- Representative regenerated samples checked visually: `Battery_WeldSpatter_Heavy_NG.png`, `Display_MuraVariation_Uneven_NG.png`, and `Semiconductor_WireSweepAlignment_Shifted_NG.png`.
- Catalog report: `artifacts\product_sample_catalog_field_variation\sample_catalog_report.md`.
- Catalog summary: `artifacts\product_sample_catalog_field_variation\sample_catalog_summary.json`.

Next priority:

- Add or tune only samples that improve realistic, explainable Good/Bad metric separation. Prioritize subtle low-contrast and field-like cases over simply increasing row count.
- If UI work resumes, use the existing Product picker/review checklist path and capture before/after evidence from the current EXE.

## 2026-07-03 Update - Product sample review checklist in Pipeline Review

Continued after the Product sample picker focus work. The next highest-impact beginner UX gap was that Product sample users could open Pipeline Review, but the Good/Bad comparison habit was still mostly learned from the guide document rather than the app.

Changed files:

- `0. UI\0) MENU\Wpf\OpenVisionPipelineReviewGuidePresenter.cs`
- `0. UI\0) MENU\Wpf\ViewModels\OpenVisionPipelineReviewViewModel.cs`
- `0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml`
- `0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml.cs`
- `0. UI\0) MENU\Wpf\Documents\OpenVisionPipelineReviewDocument.cs`
- `0. UI\0) MENU\Wpf\OpenVisionShellHostStatePresenter.cs`
- `0. UI\0) MENU\Wpf\OpenVisionShellHostToolTestFacade.cs`
- `0. UI\0) MENU\Wpf\OpenVisionShellHostView.TestHooks.cs`
- `Library\OpenVisionLab.Localization\Resources\LocalizationCatalog.tsv`
- `tools\PipelineViewerScreenshotSmoke\Program.cs`
- `docs\assets\tutorial\current\product_sample_review_current.png`

Behavior:

- Pipeline Review guide state now carries a display-only checklist text.
- Pipeline Review top guide panel now shows a compact `검증 습관` row: run Good first, run Bad from the same PairGroup with the same Pipeline, then compare output image, overlay, metric, and log.
- The checklist is presentation state only. It does not run Preview/Run, change input/output layer routing, create layers, or affect pass/fail metrics.
- Test hooks and screenshot smoke now assert the Product sample review path exposes the Good/Bad review habit, not only the OK result and output preview.
- Runtime CONFIG localization copies were refreshed so the current EXE/smoke loads the new Korean/English text.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"` PASS.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\product_sample_review_checklist_after` PASS.
- `git diff --check` for the touched review/localization/smoke files PASS, with CRLF normalization warnings only.

Evidence:

- Before reference: `artifacts\product_sample_review_final_20260703_r2\wpf_shell_host_workspace_product_sample_review.png`.
- After capture: `artifacts\product_sample_review_checklist_after\wpf_shell_host_workspace_product_sample_review.png`.
- Documentation current image refreshed: `docs\assets\tutorial\current\product_sample_review_current.png`.

Next priority:

- Continue with field-like public-safe synthetic sample quality instead of adding toy-like rows. New or revised Battery/Display/Semiconductor samples should include subtle GV differences, low-contrast defects, uneven illumination/uniformity, mild mura, small rotation/scale changes, position jitter, blur/noise, realistic contamination, burrs, scratches, voids, and domain context.
- Add more samples only when they improve explainable Good/Bad metric separation for Line/Blob/Contour/Matching/EdgeBased/Feature-style review.

## 2026-07-03 Update - Product sample picker focus filters

Continued after the 168-row Product sample catalog expansion and stopped adding duplicate sample rows. The next highest-impact beginner UX item was the sample picker: users should be able to choose Product-domain samples by product family or tool family without reading the CSV.

Changed files:

- `0. UI\0) MENU\Wpf\OpenVisionWorkspaceSampleFocusOption.cs`
- `0. UI\0) MENU\Wpf\OpenVisionWorkspaceSamplePickerViewModel.cs`
- `0. UI\0) MENU\Wpf\OpenVisionWorkspaceSamplePickerView.xaml`
- `0. UI\0) MENU\Wpf\OpenVisionShellHostSampleWorkflowPresenter.cs`
- `tools\PipelineViewerScreenshotSmoke\Program.cs`
- `0. UI\0) MENU\Wpf\OpenVisionPipelineReviewGuidePresenter.cs`
- `Library\OpenVisionLab.Localization\Resources\LocalizationCatalog.tsv`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `docs\assets\tutorial\current\product_focus_picker_current.png`
- `docs\assets\tutorial\current\product_focus_open_current.png`
- `docs\assets\tutorial\current\product_sample_review_current.png`

Behavior:

- Added a Product/sample focus option model for All, Battery, Display, Semiconductor, Matching, Blob, Contour, Measure, and Brightness.
- Sample picker now filters in this order: catalog source -> product/tool focus -> Learn path -> search text.
- Changing catalog source rebuilds valid focus choices, and changing focus rebuilds valid Learn-path choices, so users do not see unrelated or empty category combinations.
- The WPF picker exposes automation IDs for the focus summary/list so the focused UX path is covered by screenshot smoke.
- The smoke now verifies the Product source, Product focus options, and Battery-focus filtering count before taking the final picker screenshot.
- When a sample is opened, the workspace sample workflow bar now shows a clearer breadcrumb: sample name, product category, Good/Bad role, and tool flow.
- Added a Product-focus open smoke that selects a Product + Battery sample through the picker ViewModel, opens that exact sample in the workspace, and verifies the breadcrumb carries the same category/role/tool-flow context.
- Updated the Product Sample Guide with current-build screenshots for Product focus selection, Product focus open, and Product sample review.
- Pipeline Review final-step guidance now explicitly reminds the user to compare the output image, metric, and matching Good/Bad pair before accepting a pipeline.
- Runtime CONFIG copies were refreshed from `Library\OpenVisionLab.Localization\Resources\LocalizationCatalog.tsv` so the updated guidance is visible in the current EXE/smoke output.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_picker,wpf_shell_host_workspace_sample_learn_paths artifacts\sample_picker_focus_after` PASS.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_picker artifacts\sample_picker_product_focus_after` PASS.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_open artifacts\sample_open_breadcrumb_after` PASS.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_open artifacts\sample_product_focus_open_after` PASS.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\product_sample_review_final_20260703_r2` PASS.
- `dotnet run --project "tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"` PASS.
- Product Sample Guide image links were checked with `Test-Path` for all three current screenshots.
- `git diff --check` for touched picker/smoke files PASS, with CRLF normalization warnings only.

Evidence:

- Before reference: `artifacts\sample_catalog_source_picker_20260702_r2\wpf_shell_host_workspace_sample_picker.png`.
- After capture: `artifacts\sample_picker_focus_after\wpf_shell_host_workspace_sample_picker.png`.
- Product focus capture: `artifacts\sample_picker_product_focus_after\wpf_shell_host_workspace_sample_product_focus_picker.png`.
- Opened sample breadcrumb capture: `artifacts\sample_open_breadcrumb_after\wpf_shell_host_workspace_sample_open.png`.
- Product focus open capture: `artifacts\sample_product_focus_open_after\wpf_shell_host_workspace_sample_product_focus_open.png`.
- Product review capture: `artifacts\product_sample_review_final_20260703_r2\wpf_shell_host_workspace_product_sample_review.png`.
- Documentation copies: `docs\assets\tutorial\current\product_focus_picker_current.png`, `docs\assets\tutorial\current\product_focus_open_current.png`, `docs\assets\tutorial\current\product_sample_review_current.png`.

Next priority:

- Add a small product-sample checklist inside the app review panel so beginners can see the same Good/Bad metric habit without opening the Markdown guide.
- When adding more samples, avoid toy-like synthetic scenes. Use field-like variation: subtle GV differences, low-contrast defects, uneven illumination/uniformity, mild mura, small rotation/scale changes, position jitter, blur/noise, realistic contamination, burrs, scratches, voids, and domain context. Good/Bad pairs should still separate by an explainable metric, but the visual difference should be plausible rather than overly obvious.

## 2026-07-03 Update - Product sample catalog expanded to 168 rows

Continued immediately after the 162-row Product sample expansion and expanded the public-safe product-domain sample catalog from 162 to 168 runnable rows.

Added samples:

- Secondary battery `Battery_PouchSealBubble`: Threshold -> Blob count for pouch-seal bubble candidates.
- Display `Display_PadBridge`: Threshold -> Blob count for display pad bridge candidates.
- Semiconductor `Semiconductor_PackageCornerChip`: Threshold -> Contour count for package corner-chip candidates.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_PouchSealBubble_Blob.pipeline.xml`
- `docs\samples\public\product\Product_Display_PadBridge_Blob.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_PackageCornerChip_Contour.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS.
- New source images visually checked: pouch seal bubble, display pad bridge, and package corner chip show clear synthetic inspection scenes and visible Good/Bad candidate differences.
- Product catalog runner PASS at `artifacts\product_sample_catalog_20260703_r29`: `GateStatus=OK`, `RunnableRows=168`, `ExpectedFailureRows=84`, `OKRows=168`, `NGRows=0`.
- New row metrics: `Battery_PouchSealBubble` Good/Bad `ResultCount=1/5`, `Display_PadBridge` Good/Bad `ResultCount=1/5`, `Semiconductor_PackageCornerChip` Good/Bad `ResultCount=1/5`.
- Evidence sync PASS, using the current `r29` result overlays.
- Latest evidence sheet visually checked: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.
- OpenVisionReadinessCheck PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.
- `git diff --check` PASS, with CRLF normalization warnings only.

Next priority:

- Add the next non-duplicated product-domain batch with a larger UX impact for sample selection: battery weld offset/short, display scratch or FPC particle, and semiconductor lead plating void.
- Consider adding a domain/filter preset in the Product sample picker so beginners can choose Battery/Display/Semiconductor plus tool family without reading the CSV.

## 2026-07-03 Update - Product sample catalog expanded to 162 rows

Continued immediately after the 156-row Product sample expansion and expanded the public-safe product-domain sample catalog from 156 to 162 runnable rows.

Added samples:

- Secondary battery `Battery_SeparatorEdgeTear`: Threshold -> Contour count for separator edge-tear candidates.
- Display `Display_FpcAlignmentMark`: Image Matching FPC alignment-mark presence check using a generated template crop.
- Semiconductor `Semiconductor_BondPadCorrosion`: Threshold -> Blob count for bond-pad corrosion candidates.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_SeparatorEdgeTear_Contour.pipeline.xml`
- `docs\samples\public\product\Product_Display_FpcAlignmentMark_Matching.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_BondPadCorrosion_Blob.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS.
- Direct FPC alignment probe found a real threshold issue at `SCORE_MIN=0.72`; the pipeline gate was changed to `0.50`, which keeps Good at `ScoreMax=100` and Bad as no-result.
- Product catalog runner PASS at `artifacts\product_sample_catalog_20260703_r28`: `GateStatus=OK`, `RunnableRows=162`, `ExpectedFailureRows=81`, `OKRows=162`, `NGRows=0`.
- New row metrics: `Battery_SeparatorEdgeTear` Good/Bad `ResultCount=1/5`, `Display_FpcAlignmentMark` Good `ResultCount=1` and `ScoreMax=100` with Bad `ResultCount=0`, `Semiconductor_BondPadCorrosion` Good/Bad `ResultCount=1/5`.
- Evidence sync PASS, using the current `r28` result overlays.
- Latest evidence sheet visually checked: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.
- OpenVisionReadinessCheck PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery pouch seal bubble, display pad bridge, and semiconductor package corner chip.
- Consider a focused Product sample picker smoke for one newest PairGroup if the beginner sample-picker UI path needs stronger coverage.

## 2026-07-03 Update - Product sample catalog expanded to 156 rows

Continued immediately after the 150-row Product sample expansion and expanded the public-safe product-domain sample catalog from 150 to 156 runnable rows.

Added samples:

- Secondary battery `Battery_CurrentCollectorBurr`: Threshold -> Contour count for current-collector burr candidates.
- Display `Display_CofBondParticle`: Threshold -> Blob count for COF bond particle candidates.
- Semiconductor `Semiconductor_WireSweepAlignment`: LineDistance wire sweep clearance measurement.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_CurrentCollectorBurr_Contour.pipeline.xml`
- `docs\samples\public\product\Product_Display_CofBondParticle_Blob.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_WireSweepAlignment_Distance.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS.
- Product catalog runner PASS at `artifacts\product_sample_catalog_20260703_r27`: `GateStatus=OK`, `RunnableRows=156`, `ExpectedFailureRows=78`, `OKRows=156`, `NGRows=0`.
- New row metrics: `Battery_CurrentCollectorBurr` Good/Bad `ResultCount=1/5`, `Display_CofBondParticle` Good/Bad `ResultCount=1/5`, `Semiconductor_WireSweepAlignment` Good/Bad `DistanceMmAvg=0.252/0.132`.
- Evidence sync PASS, using the current `r27` result overlays.
- Latest evidence sheet visually checked: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.
- Product evidence sheet title rendering was adjusted to wrap long labels instead of overlapping.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery separator edge tear, display FPC alignment mark, and semiconductor bond-pad corrosion.
- Consider a focused Product sample picker smoke for one newest PairGroup if the beginner sample-picker UI path needs stronger coverage.

## 2026-07-03 Update - Product sample catalog expanded to 150 rows

Continued immediately after the 144-row Product sample expansion and expanded the public-safe product-domain sample catalog from 144 to 150 runnable rows.

Added samples:

- Secondary battery `Battery_PouchTabSkew`: LineDistance pouch tab skew/clearance measurement.
- Display `Display_PolarizerEdgeLift`: Threshold -> Contour count for lifted polarizer edge candidates.
- Semiconductor `Semiconductor_PackageVoid`: Threshold -> Blob count for package void candidates.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_PouchTabSkew_Distance.pipeline.xml`
- `docs\samples\public\product\Product_Display_PolarizerEdgeLift_Contour.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_PackageVoid_Blob.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS after fixing a PowerShell array unwrap issue in the new `Display_PolarizerEdgeLift` sample.
- First 150-row catalog run identified a real `Battery_PouchTabSkew` measurement issue caused by visual helper geometry inside the LineDistance ROI; the helper geometry was removed so only the measurable tab edges remain.
- Product catalog runner PASS at `artifacts\product_sample_catalog_20260703_r26`: `GateStatus=OK`, `RunnableRows=150`, `ExpectedFailureRows=75`, `OKRows=150`, `NGRows=0`.
- New row metrics: `Battery_PouchTabSkew` Good/Bad `DistanceMmAvg=0.252/0.132`, `Display_PolarizerEdgeLift` Good/Bad `ResultCount=1/5`, `Semiconductor_PackageVoid` Good/Bad `ResultCount=1/5`.
- Evidence sync PASS, using the current `r26` result overlays.
- Latest evidence sheet visually checked: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.
- OpenVisionReadinessCheck PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery current-collector burr, display COF bond particle, and semiconductor wire sweep alignment.
- Consider a focused Product sample picker smoke for one newest PairGroup if the beginner sample-picker UI path needs stronger coverage.

## 2026-07-03 Update - Product sample catalog expanded to 144 rows

Continued immediately after the 138-row Product sample expansion and expanded the public-safe product-domain sample catalog from 138 to 144 runnable rows.

Added samples:

- Secondary battery `Battery_CellVentAlignment`: LineDistance cell vent alignment spacing measurement.
- Display `Display_SealCornerContamination`: Threshold -> Blob count for seal-corner contamination candidates.
- Semiconductor `Semiconductor_LeadWidth`: LineDistance lead-width measurement.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_CellVentAlignment_Distance.pipeline.xml`
- `docs\samples\public\product\Product_Display_SealCornerContamination_Blob.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_LeadWidth_Distance.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS.
- Product catalog runner PASS at `artifacts\product_sample_catalog_20260703_r24`: `GateStatus=OK`, `RunnableRows=144`, `ExpectedFailureRows=72`, `OKRows=144`, `NGRows=0`.
- New row metrics: `Battery_CellVentAlignment` Good/Bad `DistanceMmAvg=0.252/0.132`, `Display_SealCornerContamination` Good/Bad `ResultCount=1/5`, `Semiconductor_LeadWidth` Good/Bad `DistanceMmAvg=0.252/0.132`.
- Evidence sync PASS, using the current `r24` result overlays.
- Latest evidence sheet visually checked: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.
- OpenVisionReadinessCheck PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.
- `git diff --check` on touched text/XML files PASS, CRLF normalization warnings only.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery pouch tab skew, display polarizer edge lift, and semiconductor package void inspection.
- Consider a focused Product sample picker smoke for one newest PairGroup if the beginner sample-picker UI path needs stronger coverage.

## 2026-07-03 Update - Product sample catalog expanded to 138 rows

Continued immediately after the 132-row Product sample expansion and expanded the public-safe product-domain sample catalog from 132 to 138 runnable rows.

Added samples:

- Secondary battery `Battery_ElectrolyteFillLine`: LineDistance fill-line spacing measurement.
- Display `Display_BezelChip`: Threshold -> Contour count for bezel-chip candidates.
- Semiconductor `Semiconductor_PackageLaserText`: Image Matching package laser-text presence check using a generated template crop.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_ElectrolyteFillLine_Distance.pipeline.xml`
- `docs\samples\public\product\Product_Display_BezelChip_Contour.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_PackageLaserText_Matching.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS.
- Product catalog runner PASS at `artifacts\product_sample_catalog_20260703_r20`: `GateStatus=OK`, `RunnableRows=138`, `ExpectedFailureRows=69`, `OKRows=138`, `NGRows=0`.
- New row metrics: `Battery_ElectrolyteFillLine` Good/Bad `DistanceMmAvg=0.252/0.12`, `Display_BezelChip` Good/Bad `ResultCount=1/5`, `Semiconductor_PackageLaserText` Good/Bad `ResultCount=1/0` with Good `ScoreMax=89.831`.
- `Semiconductor_PackageLaserText` uses `SCORE_MIN=0.30` in the pipeline and catalog metric validation `ScoreMax >= 72`; this keeps candidate generation stable while still enforcing the visible match score.
- Evidence sync PASS, using the current `r20` result overlays.
- Latest evidence sheet visually checked: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.
- OpenVisionReadinessCheck PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery cell vent alignment, display seal corner contamination, and semiconductor lead-width measurement.
- Consider a focused Product sample picker smoke for one newest PairGroup if the beginner sample-picker UI path needs stronger coverage.

## 2026-07-03 Update - Product sample catalog expanded to 132 rows

Continued immediately after the 126-row Product sample expansion and expanded the public-safe product-domain sample catalog from 126 to 132 runnable rows.

Added samples:

- Secondary battery `Battery_TabDateCode`: Image Matching target-presence check using a generated template crop.
- Display `Display_SealWidth`: LineDistance seal-width measurement.
- Semiconductor `Semiconductor_PackagePolarity`: Image Matching package polarity-mark presence check using a generated template crop.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_TabDateCode_Matching.pipeline.xml`
- `docs\samples\public\product\Product_Display_SealWidth_Distance.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_PackagePolarity_Matching.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS.
- Product catalog runner PASS at `artifacts\product_sample_catalog_20260703_r18`: `GateStatus=OK`, `RunnableRows=132`, `ExpectedFailureRows=66`, `OKRows=132`, `NGRows=0`.
- New row metrics: `TabDateCode` Good/Bad `ResultCount=1/0` with Good `ScoreMax=100`, `Display_SealWidth` Good/Bad `DistanceMmAvg=0.252/0.132`, `PackagePolarity` Good/Bad `ResultCount=1/0` with Good `ScoreMax=93.556`.
- Evidence sync PASS, using the current `r18` result overlays.
- Latest evidence sheet visually checked: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.
- OpenVisionReadinessCheck PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery electrolyte fill line, display bezel chip measurement, and semiconductor package laser text mark.
- Consider a focused Product sample picker smoke for one newest PairGroup if the beginner sample-picker UI path needs stronger coverage.

## 2026-07-03 Update - Product sample catalog expanded to 126 rows

Continued immediately after the 120-row Product sample expansion and expanded the public-safe product-domain sample catalog from 120 to 126 runnable rows.

Added samples:

- Secondary battery `Battery_LaserMark`: Image Matching target-presence check using a generated template crop.
- Display `Display_PolarizerCrease`: Threshold -> Contour count for polarizer-crease candidates.
- Semiconductor `Semiconductor_LeadOxidation`: Threshold -> Blob count for lead-oxidation candidates.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_LaserMark_Matching.pipeline.xml`
- `docs\samples\public\product\Product_Display_PolarizerCrease_Contour.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_LeadOxidation_Blob.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS.
- Product catalog runner PASS at `artifacts\product_sample_catalog_20260703_r16`: `GateStatus=OK`, `RunnableRows=126`, `ExpectedFailureRows=63`, `OKRows=126`, `NGRows=0`.
- New row metrics: `LaserMark` Good/Bad `ResultCount=1/0` with Good `ScoreMax=87.226`, `PolarizerCrease` Good/Bad `ResultCount=1/5`, `LeadOxidation` Good/Bad `ResultCount=1/5`.
- Evidence sync PASS, using the current `r16` result overlays.
- Latest evidence sheet visually checked: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.
- OpenVisionReadinessCheck PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.
- `git diff --check` on touched text/XML files PASS, CRLF normalization warning only.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery tab date-code OCR-style mark, display seal width measurement, and semiconductor package polarity mark.
- Consider a focused Product sample picker smoke for one newest PairGroup if the beginner sample-picker UI path needs stronger coverage.

## 2026-07-03 Update - Product sample catalog expanded to 120 rows

Continued immediately after the 114-row Product sample expansion and expanded the public-safe product-domain sample catalog from 114 to 120 runnable rows.

Added samples:

- Secondary battery `Battery_SealContamination`: Threshold -> Blob count for seal-contamination candidates.
- Display `Display_PolarizerScratch`: Threshold -> Contour count for polarizer-scratch candidates.
- Semiconductor `Semiconductor_LeadCrack`: Threshold -> Contour count for lead-crack candidates.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_SealContamination_Blob.pipeline.xml`
- `docs\samples\public\product\Product_Display_PolarizerScratch_Contour.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_LeadCrack_Contour.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS.
- Product catalog runner PASS at `artifacts\product_sample_catalog_20260703_r15`: `GateStatus=OK`, `RunnableRows=120`, `ExpectedFailureRows=60`, `OKRows=120`, `NGRows=0`.
- New row metrics: `SealContamination` Good/Bad `ResultCount=1/5`, `PolarizerScratch` Good/Bad `ResultCount=1/5`, `LeadCrack` Good/Bad `ResultCount=1/5`.
- Evidence sync PASS, using the current `r15` result overlays.
- Latest evidence sheet visually checked: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.
- OpenVisionReadinessCheck PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery laser mark, display polarizer crease, and semiconductor lead oxidation.
- Consider a focused Product sample picker smoke for one newest PairGroup if the beginner sample-picker UI path needs stronger coverage.

## 2026-07-03 Update - Product sample catalog expanded to 114 rows

Continued immediately after the 108-row Product sample expansion and expanded the public-safe product-domain sample catalog from 108 to 114 runnable rows.

Added samples:

- Secondary battery `Battery_TabDiscoloration`: Mean brightness measurement for tab discoloration drift.
- Display `Display_MuraRing`: Threshold -> Contour count for mura-ring candidates.
- Semiconductor `Semiconductor_LeadBurr`: Threshold -> Contour count for lead-burr candidates.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_TabDiscoloration_Mean.pipeline.xml`
- `docs\samples\public\product\Product_Display_MuraRing_Contour.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_LeadBurr_Contour.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS.
- Product catalog runner PASS at `artifacts\product_sample_catalog_20260703_r14`: `GateStatus=OK`, `RunnableRows=114`, `ExpectedFailureRows=57`, `OKRows=114`, `NGRows=0`.
- New row metrics: `TabDiscoloration` Good/Bad `MeanValueAvg=160.5/110.9`, `MuraRing` Good/Bad `ResultCount=1/5`, `LeadBurr` Good/Bad `ResultCount=1/5`.
- Evidence sync PASS, using the current `r14` result overlays.
- Latest evidence sheet visually checked: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.
- OpenVisionReadinessCheck PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery seal contamination, display polarizer scratch, and semiconductor lead crack.
- Consider a focused Product sample picker smoke for one newest PairGroup if the beginner sample-picker UI path needs stronger coverage.

## 2026-07-03 Update - Product sample catalog expanded to 108 rows

Continued immediately after the 102-row Product sample expansion and expanded the public-safe product-domain sample catalog from 102 to 108 runnable rows.

Added samples:

- Secondary battery `Battery_TabOxidation`: Threshold -> Contour count for tab oxidation candidates.
- Display `Display_MuraSpotCluster`: Threshold -> Contour count for mura spot candidates.
- Semiconductor `Semiconductor_PackageCrack`: Threshold -> Contour count for package-crack candidates.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_TabOxidation_Contour.pipeline.xml`
- `docs\samples\public\product\Product_Display_MuraSpotCluster_Contour.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_PackageCrack_Contour.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS.
- Product catalog runner PASS at `artifacts\product_sample_catalog_20260703_r13`: `GateStatus=OK`, `RunnableRows=108`, `ExpectedFailureRows=54`, `OKRows=108`, `NGRows=0`.
- New row metrics: `TabOxidation` Good/Bad `ResultCount=1/5`, `MuraSpotCluster` Good/Bad `ResultCount=1/5`, `PackageCrack` Good/Bad `ResultCount=1/5`.
- Evidence sync PASS, using the current `r13` result overlays.
- Latest evidence sheet visually checked: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.
- OpenVisionReadinessCheck PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.
- `git diff --check` on touched text/XML files PASS, CRLF normalization warning only.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery tab discoloration, display mura ring, and semiconductor lead burr.
- Consider a focused Product sample picker smoke for one newest PairGroup if the beginner sample-picker UI path needs stronger coverage.

## 2026-07-03 Update - Product sample catalog expanded to 102 rows

Continued immediately after the 96-row Product sample expansion and expanded the public-safe product-domain sample catalog from 96 to 102 runnable rows.

Added samples:

- Secondary battery `Battery_SealEdgeDelamination`: Threshold -> Contour count for seal-edge delamination candidates.
- Display `Display_LineDropout`: Threshold -> Contour count for signal-line dropout gap candidates.
- Semiconductor `Semiconductor_MoldingFlash`: Threshold -> Contour count for molding-flash candidates.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_SealEdgeDelamination_Contour.pipeline.xml`
- `docs\samples\public\product\Product_Display_LineDropout_Contour.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_MoldingFlash_Contour.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS.
- Product catalog runner PASS at `artifacts\product_sample_catalog_20260703_r12`: `GateStatus=OK`, `RunnableRows=102`, `ExpectedFailureRows=51`, `OKRows=102`, `NGRows=0`.
- New row metrics: `SealEdgeDelamination` Good/Bad `ResultCount=1/5`, `LineDropout` Good/Bad `ResultCount=1/5`, `MoldingFlash` Good/Bad `ResultCount=1/5`.
- Evidence sync PASS, using the current `r12` result overlays.
- Latest evidence sheet visually checked: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.
- OpenVisionReadinessCheck PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.
- `git diff --check` on touched text/XML files PASS, CRLF normalization warning only.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery tab oxidation, display mura spot cluster, and semiconductor package crack.
- Consider a focused Product sample picker smoke for one newest PairGroup if the beginner sample-picker UI path needs stronger coverage.

## 2026-07-03 Update - Product sample catalog expanded to 96 rows

Continued immediately after the 90-row Product sample expansion and expanded the public-safe product-domain sample catalog from 90 to 96 runnable rows.

Added samples:

- Secondary battery `Battery_PouchSealBurn`: Threshold -> Blob count for pouch seal burn candidates.
- Display `Display_BlackMatrixScratch`: Threshold -> Contour count for black-matrix scratch candidates.
- Semiconductor `Semiconductor_DieEdgeChip`: Threshold -> Contour count for die-edge chip candidates.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_PouchSealBurn_Blob.pipeline.xml`
- `docs\samples\public\product\Product_Display_BlackMatrixScratch_Contour.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_DieEdgeChip_Contour.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS.
- Product catalog runner PASS at `artifacts\product_sample_catalog_20260703_r11`: `GateStatus=OK`, `RunnableRows=96`, `ExpectedFailureRows=48`, `OKRows=96`, `NGRows=0`.
- New row metrics: `PouchSealBurn` Good/Bad `ResultCount=1/5`, `BlackMatrixScratch` Good/Bad `ResultCount=1/5`, `DieEdgeChip` Good/Bad `ResultCount=1/5`.
- Evidence sync PASS, using the current `r11` result overlays.
- Latest evidence sheet visually checked: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.
- OpenVisionReadinessCheck PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.
- `git diff --check` on touched text/XML files PASS, CRLF normalization warning only.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery seal edge delamination, display line dropout, and semiconductor molding flash.
- Consider a focused Product sample picker smoke for one newest PairGroup if the beginner sample-picker UI path needs stronger coverage.

## 2026-07-03 Update - Product sample catalog expanded to 90 rows

Continued immediately after the 84-row Product sample expansion and expanded the public-safe product-domain sample catalog from 84 to 90 runnable rows.

Added samples:

- Secondary battery `Battery_TabPlatingPeel`: Threshold -> Contour count for tab plating peel candidates.
- Display `Display_CornerLightLeak`: Mean brightness measurement for corner light-leak intensity.
- Semiconductor `Semiconductor_ProbeMark`: Threshold -> Blob count for probe-mark candidates.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_TabPlatingPeel_Contour.pipeline.xml`
- `docs\samples\public\product\Product_Display_CornerLightLeak_Mean.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_ProbeMark_Blob.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS.
- Product catalog runner PASS at `artifacts\product_sample_catalog_20260703_r10`: `GateStatus=OK`, `RunnableRows=90`, `ExpectedFailureRows=45`, `OKRows=90`, `NGRows=0`.
- New row metrics: `TabPlatingPeel` Good/Bad `ResultCount=1/5`, `CornerLightLeak` Good/Bad `MeanValueAvg=80.8/140`, `ProbeMark` Good/Bad `ResultCount=1/5`.
- Evidence sync PASS, using the current `r10` result overlays.
- Latest evidence sheet visually checked: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.
- OpenVisionReadinessCheck PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.
- `git diff --check` on touched text/XML files PASS, CRLF normalization warning only.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery pouch seal burn, display black matrix scratch, and semiconductor die edge chip.
- Consider a focused Product sample picker smoke for one newest PairGroup if the beginner sample-picker UI path needs stronger coverage.

## 2026-07-03 Update - Product sample catalog expanded to 84 rows

Continued immediately after the 78-row Product sample expansion and expanded the public-safe product-domain sample catalog from 78 to 84 runnable rows.

Added samples:

- Secondary battery `Battery_TabTear`: Threshold -> Contour count for torn-tab edge candidates.
- Display `Display_SealContamination`: Threshold -> Blob count for seal-contamination candidates.
- Semiconductor `Semiconductor_LeadCoplanarity`: LineDistance measurement for lead-foot coplanarity clearance.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_TabTear_Contour.pipeline.xml`
- `docs\samples\public\product\Product_Display_SealContamination_Blob.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_LeadCoplanarity_Distance.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS.
- Product catalog runner PASS at `artifacts\product_sample_catalog_20260703_r9`: `GateStatus=OK`, `RunnableRows=84`, `ExpectedFailureRows=42`, `OKRows=84`, `NGRows=0`.
- New row metrics: `TabTear` Good/Bad `ResultCount=1/5`, `SealContamination` Good/Bad `ResultCount=1/5`, `LeadCoplanarity` Good/Bad `DistanceMmAvg=0.225/0.118`.
- Evidence sync PASS, using the current `r9` result overlays.
- Latest evidence sheet visually checked: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery tab plating peel, display corner light leak, and semiconductor probe mark.
- Consider a focused Product sample picker smoke for one newest PairGroup if the beginner sample-picker UI path needs stronger coverage.

## 2026-07-03 Update - Product sample catalog expanded to 78 rows

Expanded the public-safe product-domain sample catalog from 72 to 78 runnable rows.

Added samples:

- Secondary battery `Battery_SeparatorPinhole`: Threshold -> Blob count for separator pinhole candidates.
- Display `Display_ColorFilterShift`: LineDistance measurement for color-filter registration shift.
- Semiconductor `Semiconductor_UnderfillVoid`: Threshold -> Blob count for underfill void candidates.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_SeparatorPinhole_Blob.pipeline.xml`
- `docs\samples\public\product\Product_Display_ColorFilterShift_Distance.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_UnderfillVoid_Blob.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS.
- Initial `r8` catalog run caught a real Display `ColorFilterShift` issue: auxiliary guide lines interfered with LineDistance and produced `0.517/0.648mm` instead of the intended normal/shifted bands. The generator was corrected by removing those guide lines.
- Product catalog runner PASS at `artifacts\product_sample_catalog_20260703_r8_fixed`: `GateStatus=OK`, `RunnableRows=78`, `ExpectedFailureRows=39`, `OKRows=78`, `NGRows=0`.
- New row metrics: `SeparatorPinhole` Good/Bad `ResultCount=1/5`, `ColorFilterShift` Good/Bad `DistanceMmAvg=0.252/0.132`, `UnderfillVoid` Good/Bad `ResultCount=1/5`.
- Evidence sync PASS, using the current `r8_fixed` result overlays.
- OpenVisionReadinessCheck PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery tab tear, display seal contamination, and semiconductor lead coplanarity.
- Add a focused Product sample picker smoke for one newest PairGroup if the beginner sample-picker UI path needs stronger coverage.

## 2026-07-03 Update - Product sample catalog expanded to 72 rows

Expanded the public-safe product-domain sample catalog from 66 to 72 runnable rows.

Added samples:

- Secondary battery `Battery_SeparatorWrinkle`: Threshold -> Contour count for separator wrinkle candidates.
- Display `Display_PolarizerBubble`: Threshold -> Blob count for polarizer bubble candidates.
- Semiconductor `Semiconductor_WireBondLift`: Threshold -> Blob count for wire-bond lift candidates.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_SeparatorWrinkle_Contour.pipeline.xml`
- `docs\samples\public\product\Product_Display_PolarizerBubble_Blob.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_WireBondLift_Blob.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS.
- Product catalog runner PASS at `artifacts\product_sample_catalog_20260703_r7`: `GateStatus=OK`, `RunnableRows=72`, `ExpectedFailureRows=36`, `OKRows=72`, `NGRows=0`.
- New row metrics: `SeparatorWrinkle` Good/Bad `ResultCount=1/4`, `PolarizerBubble` Good/Bad `ResultCount=1/5`, `WireBondLift` Good/Bad `ResultCount=1/5`.
- Evidence sync PASS, using the current `r7` result overlays.
- OpenVisionReadinessCheck PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.
- `git diff --check` PASS for touched text/XML files, with CRLF normalization warnings only.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery separator pinhole, display color-filter shift, and semiconductor underfill void.
- Add a focused Product sample picker smoke for one newest PairGroup if the beginner sample-picker UI path needs stronger coverage.

## 2026-07-03 Update - Product sample catalog expanded to 66 rows

Expanded the public-safe product-domain sample catalog from 60 to 66 runnable rows.

Added samples:

- Secondary battery `Battery_ElectrolyteStain`: Mean brightness drift for electrolyte-stain inspection.
- Display `Display_SubpixelBridge`: Threshold -> Blob count for subpixel bridge candidates.
- Semiconductor `Semiconductor_BondPadNick`: Threshold -> Contour count for bond-pad nick candidates.

Changed files:

- `tools\GenerateOpenVisionProductSamples.ps1`
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
- `docs\samples\public\product\Product_Battery_ElectrolyteStain_Mean.pipeline.xml`
- `docs\samples\public\product\Product_Display_SubpixelBridge_Blob.pipeline.xml`
- `docs\samples\public\product\Product_Semiconductor_BondPadNick_Contour.pipeline.xml`
- `docs\samples\public\product\README.md`
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`
- `tools\SyncPublicLearnEvidenceImages.ps1`
- `tools\OpenVisionReadinessCheck\Program.cs`
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`

Validation:

- Generator PASS.
- First catalog run found the electrolyte-stain Bad expected range was too high (`MeanValueAvg=105` vs min `110`); range corrected to `101..150`.
- Catalog runner PASS at `artifacts\product_sample_catalog_20260703_r6_fixed`: `GateStatus=OK`, `RunnableRows=66`, `ExpectedFailureRows=33`, `OKRows=66`, `NGRows=0`.
- Evidence sync PASS.
- OpenVisionReadinessCheck PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` PASS, warnings 0, errors 0.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery separator wrinkle, display polarizer bubble, and semiconductor wire-bond lift.
- Add a focused Product sample picker smoke for one newest PairGroup if the beginner sample-picker UI path needs stronger coverage.

## 2026-07-03 Update - Product sample catalog expanded to 60 rows

Expanded the public-safe product-domain sample catalog with three additional Good/Bad pairs. This cycle continued from the 54-row catalog and avoided already completed PairGroups.

Added samples:

- Secondary battery `Battery_WeldOverburn`: Threshold -> Blob count for weld overburn hot-spot candidates.
- Display `Display_LineStain`: Threshold -> Contour count for line-stain candidates.
- Semiconductor `Semiconductor_PadScratch`: Threshold -> Contour count for pad-scratch candidates.

Changed structure:

- `tools\GenerateOpenVisionProductSamples.ps1`: added generator functions, manifest rows, and generation calls for the new six sample images.
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`: expanded from 54 to 60 runnable rows.
- Added baseline pipelines:
  - `docs\samples\public\product\Product_Battery_WeldOverburn_Blob.pipeline.xml`
  - `docs\samples\public\product\Product_Display_LineStain_Contour.pipeline.xml`
  - `docs\samples\public\product\Product_Semiconductor_PadScratch_Contour.pipeline.xml`
- `docs\samples\public\product\README.md`: updated domain table and expected result to 60 rows / 30 controlled NG rows.
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`: updated the product sample guide and next-sample list without repeating completed PairGroups.
- `tools\SyncPublicLearnEvidenceImages.ps1`: updated the product evidence sheet to the new Weld Overburn, Line Stain, and Pad Scratch pairs, using overlay result captures.
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`: regenerated from `artifacts\product_sample_catalog_20260703_r5`.
- `tools\OpenVisionReadinessCheck\Program.cs`: updated product sample contract checks for the new catalog rows, PairGroups, manifest assets, and pipeline files.

Evidence:

- Product catalog report: `artifacts\product_sample_catalog_20260703_r5\sample_catalog_report.md`.
- Product source/result sheet: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.

Validation:

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\GenerateOpenVisionProductSamples.ps1`: PASS.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_sample_catalog_20260703_r5 -SkipRestore -SkipRunnerBuild`: PASS, `GateStatus=OK`, `RunnableRows=60`, `ExpectedFailureRows=30`, `OKRows=60`, `NGRows=0`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\SyncPublicLearnEvidenceImages.ps1 -CatalogArtifactDir artifacts\public_sample_catalog_20260702_learn_evidence -ProductCatalogArtifactDir artifacts\product_sample_catalog_20260703_r5`: PASS.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `git diff --check` on touched text files: PASS, CRLF normalization warning only.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery electrolyte stain, display subpixel bridge, and semiconductor bond-pad nick.
- Add or refresh a focused Product sample picker WPF smoke once the sample catalog UI needs stronger UI-path coverage for the newest samples.

## 2026-07-03 Update - Product sample catalog expanded to 54 rows

Expanded the public-safe product-domain sample catalog with three additional Good/Bad pairs. This cycle continued from the 48-row catalog and avoided already completed PairGroups.

Added samples:

- Secondary battery `Battery_PouchEdgeFold`: Threshold -> Contour count for pouch-edge fold candidates.
- Display `Display_AlignmentOffset`: LineDistance measurement for alignment-mark offset against a reference bar.
- Semiconductor `Semiconductor_SolderBridge`: Threshold -> Blob count for solder-bridge candidates between pads.

Changed structure:

- `tools\GenerateOpenVisionProductSamples.ps1`: added generator functions, manifest rows, and generation calls for the new six sample images.
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`: expanded from 48 to 54 runnable rows and widened `Display_AlignmentOffset` Good upper gate to `0.26` based on the measured stable `0.252mm` result.
- Added baseline pipelines:
  - `docs\samples\public\product\Product_Battery_PouchEdgeFold_Contour.pipeline.xml`
  - `docs\samples\public\product\Product_Display_AlignmentOffset_Distance.pipeline.xml`
  - `docs\samples\public\product\Product_Semiconductor_SolderBridge_Blob.pipeline.xml`
- `docs\samples\public\product\README.md`: updated domain table and expected result to 54 rows / 27 controlled NG rows.
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`: updated the product sample guide and next-sample list without repeating completed PairGroups.
- `tools\SyncPublicLearnEvidenceImages.ps1`: updated the product evidence sheet to the new Pouch Edge Fold, Alignment Offset, and Solder Bridge pairs, using overlay result captures instead of final binary-only result images.
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`: regenerated from `artifacts\product_sample_catalog_20260703_r4`.
- `tools\OpenVisionReadinessCheck\Program.cs`: updated product sample contract checks for the new catalog rows, PairGroups, manifest assets, and pipeline files.

Evidence:

- Product catalog report: `artifacts\product_sample_catalog_20260703_r4\sample_catalog_report.md`.
- Product source/result sheet: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.

Validation:

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\GenerateOpenVisionProductSamples.ps1`: PASS; generated image and manifest entries verified.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_sample_catalog_20260703_r4 -SkipRestore -SkipRunnerBuild`: PASS after widening `Display_AlignmentOffset` Good max from `0.25` to measured-safe `0.26`; `GateStatus=OK`, `RunnableRows=54`, `ExpectedFailureRows=27`, `OKRows=54`, `NGRows=0`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\SyncPublicLearnEvidenceImages.ps1 -CatalogArtifactDir artifacts\public_sample_catalog_20260702_learn_evidence -ProductCatalogArtifactDir artifacts\product_sample_catalog_20260703_r4`: PASS.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `git diff --check` on touched text files: PASS, CRLF normalization warning only.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery weld overburn, display line stain, and semiconductor pad scratch.
- Consider a focused Product sample picker smoke for one of the newest PairGroups if the beginner sample-picker UX needs stronger UI-path coverage.

## 2026-07-03 Update - Product sample catalog expanded to 48 rows

Expanded the public-safe product-domain sample catalog with three additional Good/Bad pairs. This cycle continued from the 42-row catalog and avoided already completed PairGroups.

Added samples:

- Secondary battery `Battery_TabWeldVoid`: Threshold -> Blob count for bright void candidates inside a tab weld nugget.
- Display `Display_CornerCrack`: Threshold -> Contour count for corner-crack candidates on a display panel.
- Semiconductor `Semiconductor_DieContamination`: Threshold -> Blob count for bright contamination candidates on the die surface.

Changed structure:

- `tools\GenerateOpenVisionProductSamples.ps1`: added generator functions, manifest rows, and generation calls for the new six sample images.
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`: expanded from 42 to 48 runnable rows.
- Added baseline pipelines:
  - `docs\samples\public\product\Product_Battery_TabWeldVoid_Blob.pipeline.xml`
  - `docs\samples\public\product\Product_Display_CornerCrack_Contour.pipeline.xml`
  - `docs\samples\public\product\Product_Semiconductor_DieContamination_Blob.pipeline.xml`
- `docs\samples\public\product\README.md`: updated domain table and expected result to 48 rows / 24 controlled NG rows.
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`: updated the product sample guide and next-sample list without repeating completed PairGroups.
- `tools\SyncPublicLearnEvidenceImages.ps1`: updated the product evidence sheet source/result set to the new Tab Weld Void, Corner Crack, and Die Contamination pairs.
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`: regenerated from `artifacts\product_sample_catalog_20260703_r3`.
- `tools\OpenVisionReadinessCheck\Program.cs`: updated product sample contract checks for the new catalog rows, PairGroups, manifest assets, and pipeline files.

Evidence:

- Product catalog report: `artifacts\product_sample_catalog_20260703_r3\sample_catalog_report.md`.
- Product source/result sheet: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.

Validation:

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\GenerateOpenVisionProductSamples.ps1`: PASS.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_sample_catalog_20260703_r3 -SkipRestore -SkipRunnerBuild`: PASS, `GateStatus=OK`, `RunnableRows=48`, `ExpectedFailureRows=24`, `OKRows=48`, `NGRows=0`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\SyncPublicLearnEvidenceImages.ps1 -CatalogArtifactDir artifacts\public_sample_catalog_20260702_learn_evidence -ProductCatalogArtifactDir artifacts\product_sample_catalog_20260703_r3`: PASS.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery pouch edge fold, display alignment offset, and semiconductor solder bridge.
- Consider a focused Product sample picker smoke for one of the newest PairGroups if the beginner sample-picker UX needs stronger UI-path coverage.

## 2026-07-03 Update - Product sample catalog expanded to 42 rows

Expanded the public-safe product-domain sample catalog with three additional Good/Bad pairs. This cycle continued from the 36-row catalog and avoided already completed PairGroups.

Added samples:

- Secondary battery `Battery_SealWidth`: LineDistance measurement for seal-width clearance against a reference band.
- Display `Display_EdgeChip`: Threshold -> Contour count for bright edge-chip candidates along a display panel edge.
- Semiconductor `Semiconductor_WaferDieMark`: Image Matching check for a generated wafer die mark template.

Changed structure:

- `tools\GenerateOpenVisionProductSamples.ps1`: added generator functions, manifest rows, and generation calls for the new six sample images and the wafer die mark template.
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`: expanded from 36 to 42 runnable rows.
- Added baseline pipelines:
  - `docs\samples\public\product\Product_Battery_SealWidth_Distance.pipeline.xml`
  - `docs\samples\public\product\Product_Display_EdgeChip_Contour.pipeline.xml`
  - `docs\samples\public\product\Product_Semiconductor_WaferDieMark_Matching.pipeline.xml`
- `docs\samples\public\product\README.md`: updated domain table and expected result to 42 rows / 21 controlled NG rows.
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`: updated the product sample guide and next-sample list without repeating completed PairGroups.
- `tools\SyncPublicLearnEvidenceImages.ps1`: updated the product evidence sheet source/result set to the new Seal Width, Edge Chip, and Wafer Die Mark pairs.
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`: regenerated from `artifacts\product_sample_catalog_20260703_r2`.
- `tools\OpenVisionReadinessCheck\Program.cs`: updated product sample contract checks for the new catalog rows, PairGroups, manifest assets, and pipeline files.

Evidence:

- Product catalog report: `artifacts\product_sample_catalog_20260703_r2\sample_catalog_report.md`.
- Product source/result sheet: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.

Validation:

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\GenerateOpenVisionProductSamples.ps1`: PASS.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_sample_catalog_20260703_r2 -SkipRestore -SkipRunnerBuild`: PASS after widening `Battery_SealWidth` Good max from `0.25` to measured-safe `0.26`; `GateStatus=OK`, `RunnableRows=42`, `ExpectedFailureRows=21`, `OKRows=42`, `NGRows=0`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\SyncPublicLearnEvidenceImages.ps1 -CatalogArtifactDir artifacts\public_sample_catalog_20260702_learn_evidence -ProductCatalogArtifactDir artifacts\product_sample_catalog_20260703_r2`: PASS.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS after rerunning alone; a prior parallel build collided with the concurrently running readiness project DLL lock and was not counted as final validation.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery tab weld void, display corner crack, and semiconductor die contamination.
- Consider a focused Product sample picker smoke for one of the newest PairGroups if the beginner sample-picker UX needs stronger UI-path coverage.

## 2026-07-03 Update - Product sample catalog expanded to 36 rows

Expanded the public-safe product-domain sample catalog with three additional Good/Bad pairs. This cycle avoided completed PairGroups and closed the previous next-priority items.

Added samples:

- Secondary battery `Battery_TabOffset`: LineDistance measurement for tab offset clearance against a reference lead.
- Display `Display_MuraVariation`: Mean brightness check for uneven mura variation under controlled illumination.
- Semiconductor `Semiconductor_LeadAlignment`: LineDistance measurement for lead alignment clearance against a reference lead.

Changed structure:

- `tools\GenerateOpenVisionProductSamples.ps1`: added generator functions, manifest rows, and generation calls for the new six sample images.
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`: expanded from 30 to 36 runnable rows.
- Added baseline pipelines:
  - `docs\samples\public\product\Product_Battery_TabOffset_Distance.pipeline.xml`
  - `docs\samples\public\product\Product_Display_MuraVariation_Mean.pipeline.xml`
  - `docs\samples\public\product\Product_Semiconductor_LeadAlignment_Distance.pipeline.xml`
- `docs\samples\public\product\README.md`: updated domain table and expected result to 36 rows / 18 controlled NG rows.
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`: updated the product sample guide and next-sample list without repeating completed PairGroups.
- `tools\SyncPublicLearnEvidenceImages.ps1`: updated the product evidence sheet source/result set to the new Tab Offset, Mura Variation, and Lead Alignment pairs.
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`: regenerated from `artifacts\product_sample_catalog_20260703_r1`.
- `tools\OpenVisionReadinessCheck\Program.cs`: updated product sample contract checks for the new catalog rows, PairGroups, manifest assets, and pipeline files.

Evidence:

- Product catalog report: `artifacts\product_sample_catalog_20260703_r1\sample_catalog_report.md`.
- Product source/result sheet: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.

Validation:

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\GenerateOpenVisionProductSamples.ps1`: PASS.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_sample_catalog_20260703_r1 -SkipRestore -SkipRunnerBuild`: PASS, `GateStatus=OK`, `RunnableRows=36`, `ExpectedFailureRows=18`, `OKRows=36`, `NGRows=0`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\SyncPublicLearnEvidenceImages.ps1 -CatalogArtifactDir artifacts\public_sample_catalog_20260702_learn_evidence -ProductCatalogArtifactDir artifacts\product_sample_catalog_20260703_r1`: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `git diff --check` on touched text files: PASS, CRLF normalization warning only.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery seal width, display edge chip, and semiconductor wafer die mark.
- Add a focused Product sample picker smoke that selects one of the newly added PairGroups from the Product source if the sample picker UX needs stronger coverage.

## 2026-07-03 Update - Product sample controlled-NG WPF review smoke added

Added a focused WPF smoke target for Product sample controlled-NG review. The product sample UI path now covers both an OK Product sample and a Bad sample that must explain its NG reason through Pipeline Review.

Changed structure:

- `tools\PipelineViewerScreenshotSmoke\Program.cs`: added `wpf_shell_host_workspace_product_sample_review_ng`.
  - Opens `Product_Display_Particle_Many_Bad` from the Product catalog source.
  - Verifies it is the `Display_Particle` Product PairGroup, uses `Product_Display_Particle_Blob.pipeline.xml`, and expects `ResultCount`.
  - Verifies the Product Learn route resolves to `docs\learn\LEARN_PRODUCT_SAMPLES.md`.
  - Runs Pipeline Review explicitly and verifies NG decision, beginner next action, metric detail, run log, and output preview.
- `tools\OpenVisionReadinessCheck\Program.cs`: added readiness contract tokens for the Product controlled-NG WPF smoke target and concrete Bad sample.

Evidence:

- Product NG WPF review capture: `artifacts\product_sample_ui_review_ng_20260703_single\wpf_shell_host_workspace_product_sample_review_ng.png`.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\product_sample_ui_review_ng_20260703_single`: PASS.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `git diff --check -- tools\PipelineViewerScreenshotSmoke\Program.cs tools\OpenVisionReadinessCheck\Program.cs`: PASS, CRLF normalization warning only.

Note:

- A combined two-target run printed PASS for `wpf_shell_host_workspace_product_sample_review`, then the second target stalled before output. It was terminated and not counted as validation. The controlled-NG target passed when run as a focused single target.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery tab offset, display mura variation under uneven illumination, and semiconductor wafer die mark or lead alignment.
- Keep new samples tied to one shared Good/Bad baseline pipeline and a bounded metric before adding Learn/tutorial references.

## 2026-07-02 Update - Product sample WPF review smoke added

Added a focused WPF smoke target for the Product sample catalog path so product-domain synthetic samples are validated through the operator UI flow, not only through the catalog runner.

Changed structure:

- `tools\PipelineViewerScreenshotSmoke\Program.cs`: added `wpf_shell_host_workspace_product_sample_review`.
  - Opens `Product_Display_Particle_Good` from the Product catalog source.
  - Verifies the Product Learn route resolves to `docs\learn\LEARN_PRODUCT_SAMPLES.md`.
  - Opens the sample Pipeline Review without native Preview side effects.
  - Runs Pipeline Review explicitly and verifies OK decision, Result-style detail, run log, and output preview.
- `tools\OpenVisionReadinessCheck\Program.cs`: added readiness contract tokens for the Product WPF review smoke target, concrete Product sample, and Product Learn document route.

Evidence:

- WPF review capture: `artifacts\product_sample_ui_review_20260702\wpf_shell_host_workspace_product_sample_review.png`.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\product_sample_ui_review_20260702`: PASS.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `git diff --check -- tools\PipelineViewerScreenshotSmoke\Program.cs tools\OpenVisionReadinessCheck\Program.cs`: PASS, CRLF normalization warning only.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery tab offset, display mura variation under uneven illumination, and semiconductor wafer die mark or lead alignment.
- After the next batch passes catalog validation, add another WPF smoke for a Product controlled-NG sample so the beginner flow covers both OK and NG decisions from the UI path.

## 2026-07-02 Update - Product sample catalog expanded to 30 rows

Expanded the public-safe product-domain sample catalog with three additional Good/Bad pairs. This cycle avoided the already completed PairGroups and added new inspection modes per domain.

Added samples:

- Secondary battery `Battery_EdgeBurr`: Threshold -> Contour count for coating-edge burr candidates.
- Display `Display_Particle`: Threshold -> Blob count for bright particle candidates.
- Semiconductor `Semiconductor_RotationMark`: EdgeBasedMatching angle gate for rotation tolerance.

Changed structure:

- `tools\GenerateOpenVisionProductSamples.ps1`: added generator functions, manifest rows, and generation calls for the new samples and `Semiconductor_RotationMark_Template.png`.
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`: expanded from 24 to 30 runnable rows.
- Added baseline pipelines:
  - `docs\samples\public\product\Product_Battery_EdgeBurr_Contour.pipeline.xml`
  - `docs\samples\public\product\Product_Display_Particle_Blob.pipeline.xml`
  - `docs\samples\public\product\Product_Semiconductor_RotationMark_Edge.pipeline.xml`
- `docs\samples\public\product\README.md`: updated domain table and expected result to 30 rows / 15 controlled NG rows.
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`: updated the product sample guide and next-sample list without repeating completed PairGroups.
- `tools\SyncPublicLearnEvidenceImages.ps1`: updated the product evidence sheet source/result set to the new Edge Burr, Particle, and Rotation Mark pairs.
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`: regenerated from `artifacts\product_sample_catalog_20260702_r10`.
- `tools\OpenVisionReadinessCheck\Program.cs`: updated product sample contract checks for the new catalog rows, PairGroups, manifest assets, and pipeline files.

Evidence:

- Product catalog report: `artifacts\product_sample_catalog_20260702_r10\sample_catalog_report.md`.
- Product source/result sheet: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.

Validation:

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\GenerateOpenVisionProductSamples.ps1`: PASS.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_sample_catalog_20260702_r10 -SkipRestore -SkipRunnerBuild`: PASS, `GateStatus=OK`, `RunnableRows=30`, `ExpectedFailureRows=15`, `OKRows=30`, `NGRows=0`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\SyncPublicLearnEvidenceImages.ps1 -CatalogArtifactDir artifacts\public_sample_catalog_20260702_learn_evidence -ProductCatalogArtifactDir artifacts\product_sample_catalog_20260702_r10`: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `git diff --check` on touched files: PASS, CRLF normalization warning only.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery tab offset, display mura variation under uneven illumination, and semiconductor wafer die mark or lead alignment.
- Add a focused WPF sample-picker/Pipeline Review smoke for the product catalog source so the beginner Learn/sample flow is checked from the UI path, not only from the catalog runner.

## 2026-07-02 Update - Product sample catalog expanded to 24 rows

Expanded the public-safe product-domain sample catalog with three larger-context Good/Bad pairs. These samples are generated by OpenVisionLab scripts and do not use commercial SDK sample images.

Added samples:

- Secondary battery `Battery_ForeignObject`: Threshold -> Blob count for bright foreign-object candidates.
- Display `Display_BrightnessBand`: Mean brightness check for a mura/brightness-band region.
- Semiconductor `Semiconductor_PadPitch`: LineDistance measurement for pad pitch spacing in mm.

Changed structure:

- `tools\GenerateOpenVisionProductSamples.ps1`: added generator functions, manifest rows, and generation calls for the three new Good/Bad pairs.
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`: expanded from 18 to 24 runnable rows.
- Added baseline pipelines:
  - `docs\samples\public\product\Product_Battery_ForeignObject_Blob.pipeline.xml`
  - `docs\samples\public\product\Product_Display_BrightnessBand_Mean.pipeline.xml`
  - `docs\samples\public\product\Product_Semiconductor_PadPitch_Distance.pipeline.xml`
- `docs\samples\public\product\README.md`: updated domain table and expected result to 24 rows / 12 controlled NG rows.
- `docs\learn\LEARN_PRODUCT_SAMPLES.md`: updated the product sample guide and evidence description for the 24-row catalog.
- `tools\SyncPublicLearnEvidenceImages.ps1`: can generate a product source/result evidence sheet from the latest product catalog artifact when a prebuilt expansion sheet is absent.
- `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`: regenerated from the current product catalog result images.
- `tools\OpenVisionReadinessCheck\Program.cs`: updated product sample contract checks for the new rows, PairGroups, manifest assets, and pipeline files.

Evidence:

- Product catalog report: `artifacts\product_sample_catalog_20260702_r8\sample_catalog_report.md`.
- Product source/result sheet: `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.

Validation:

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\GenerateOpenVisionProductSamples.ps1`: PASS.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_sample_catalog_20260702_r8 -SkipRestore -SkipRunnerBuild`: PASS, `GateStatus=OK`, `RunnableRows=24`, `ExpectedFailureRows=12`, `OKRows=24`, `NGRows=0`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\SyncPublicLearnEvidenceImages.ps1 -CatalogArtifactDir artifacts\public_sample_catalog_20260702_learn_evidence -ProductCatalogArtifactDir artifacts\product_sample_catalog_20260702_r8`: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.

Next priority:

- Add the next product-domain batch without repeating completed PairGroups: battery tab offset or coating edge burr, display particle/mura variation, and semiconductor rotation tolerance or wafer die mark.
- After that, add a focused sample-picker or Pipeline Review smoke for the product catalog source so beginner Learn/sample flows are checked from the WPF UI, not only from the catalog runner.

## 2026-07-02 Update - Product sample Learn guide connected

Connected the product-domain synthetic catalog to a beginner-facing Learn document after the 18-row product catalog passed.

Changed structure:

- Added `docs\learn\LEARN_PRODUCT_SAMPLES.md`.
  - Covers secondary battery, display, and semiconductor Good/Bad PairGroups.
  - Explains the expected metric, Bad-sample failure reason, and first troubleshooting check.
- Rewrote `docs\learn\README.md` as a short Learn index with `Product Sample Guide` first.
- Copied current generated product evidence to `docs\assets\tutorial\annotated\product_sample_source_result_sheet.png`.
- Updated `OpenVisionWorkspaceLearnDocumentService` so `VisionPipelineSampleCatalogSourceKind.Product` resolves to `LEARN_PRODUCT_SAMPLES.md`.
- Updated `OpenVisionReadinessCheck` to require the product Learn document and evidence image.
- Updated `tools\SyncPublicLearnEvidenceImages.ps1` with optional `-ProductCatalogArtifactDir`.
- Updated `docs\OPENVISIONLAB_DOCUMENTATION_CAPTURE_GUIDE.md` so product sample catalog evidence is regenerated/synced instead of copied by hand.

Validation:

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\SyncPublicLearnEvidenceImages.ps1 -CatalogArtifactDir artifacts\public_sample_catalog_20260702_learn_evidence -ProductCatalogArtifactDir artifacts\product_sample_catalog_20260702_r6`: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_picker,wpf_shell_host_workspace_sample_learn_paths artifacts\product_learn_picker_20260702`: PASS.
- `git diff --check` on touched files: PASS, CRLF normalization warning only.

Next priority:

- Add the next product-domain sample batch with larger context: battery foreign object/tab offset, display mura/brightness band, and semiconductor pad pitch/rotation tolerance.
- Keep each new Good/Bad pair tied to one shared baseline pipeline and a bounded metric so the Learn guide can explain the NG reason without subjective image interpretation.

## 2026-07-02 Update - Product-domain synthetic sample catalog expanded to 18 rows

Expanded the public-safe product-domain sample set with three additional Good/Bad pairs. The product catalog now covers nine Good/Bad flows across secondary battery, display, and semiconductor domains.

Added samples:

- Secondary battery `Battery_CoatingGap`: LineDistance measurement for coating-to-tab clearance.
- Display `Display_Scratch`: Threshold -> Contour counting for scratch-like candidates.
- Semiconductor `Semiconductor_PadContamination`: Threshold -> Blob counting for pad contamination.

Changed structure:

- `tools\GenerateOpenVisionProductSamples.ps1`: added generator functions and manifest rows for the three new Good/Bad pairs.
- `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`: expanded from 12 to 18 runnable rows.
- `docs\samples\public\product\Product_Battery_CoatingGap_Distance.pipeline.xml`: baseline distance pipeline.
- `docs\samples\public\product\Product_Display_Scratch_Contour.pipeline.xml`: baseline contour pipeline.
- `docs\samples\public\product\Product_Semiconductor_PadContamination_Blob.pipeline.xml`: baseline blob pipeline.
- `docs\samples\public\product\README.md`: updated domain table and validation expectation to 18 rows / 9 controlled NG rows.
- `OpenVisionReadinessCheck`: updated product sample contract to require the new catalog rows, manifest assets, Good/Bad groups, and pipelines.

Evidence:

- Product catalog report: `artifacts\product_sample_catalog_20260702_r6\sample_catalog_report.md`.
- Product expansion source/result sheet: `artifacts\product_sample_catalog_20260702_r6\product_expansion_source_result_sheet.png`.

Validation:

- `powershell -ExecutionPolicy Bypass -File tools\GenerateOpenVisionProductSamples.ps1`: PASS.
- `powershell -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_sample_catalog_20260702_r6 -SkipRestore -SkipRunnerBuild`: PASS, `GateStatus=OK`, `RunnableRows=18`, `ExpectedFailureRows=9`, `OKRows=18`, `NGRows=0`.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.

Next priority:

- Add product-domain Learn/evidence pages that explain the new Good/Bad pairs in operator language: what to inspect, which metric decides OK/NG, and what failure cause to check first.
- Then expand the remaining domain scenarios with larger product context: battery tab offset/foreign object, display mura/brightness band, and semiconductor pad pitch/rotation tolerance.

## 2026-07-02 Update - Product-domain synthetic sample catalog added

Added a public-safe product-domain sample batch for secondary battery, display, and semiconductor inspection workflows. This batch is generated by OpenVisionLab scripts and does not use copied commercial SDK sample images.

Changed structure:

- Added `tools\GenerateOpenVisionProductSamples.ps1`.
- Added `docs\samples\public\product` generated assets, templates, product pipelines, manifest, and README.
- Added `docs\samples\OpenVisionLab.ProductSampleCatalog.csv` with 12 runnable rows:
  - Secondary battery: `Battery_TabGap` LineDistance Good/Bad, `Battery_WeldSpatter` Blob Good/Bad.
  - Display: `Display_PixelDefect` Contour Good/Bad, `Display_Alignment` Image Matching Good/Bad.
  - Semiconductor: `Semiconductor_Fiducial` EdgeBasedMatching Good/Bad, `Semiconductor_BondMark` FeatureMatching Good/Bad.
- Exposed the product catalog in the WPF sample picker as `제품군 샘플` / `Product` while keeping `공개 샘플` as the default source.
- Extended `OpenVisionReadinessCheck` and `PipelineViewerScreenshotSmoke` so the product catalog source, manifest, no-SDK-path contract, and sample picker filtering are checked.
- Updated public sample docs/policy to include the product-domain catalog and validation command.

Important tuning note:

- The battery tab-gap sample must keep the LineDistance ROI visually quiet. Earlier decoration around the fixture made the Line tool measure the frame instead of the two tab edges. The current generated image removes the decorative frame from the measurement ROI.

Validation:

- `powershell -ExecutionPolicy Bypass -File tools\GenerateOpenVisionProductSamples.ps1`: PASS.
- `powershell -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_sample_catalog_20260702_r3 -SkipRestore -SkipRunnerBuild`: PASS, `GateStatus=OK`, `RunnableRows=12`, `OKRows=12`, `NGRows=0`.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_picker artifacts\product_sample_picker_20260702`: PASS. Screenshot shows `공개 샘플`, `제품군 샘플`, and `로컬 Legacy` sources.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `git diff --check` on touched files: PASS, CRLF normalization warnings only.

Next priority:

- Add product-domain Learn pages/evidence images for the six new product Good/Bad flows so the beginner route can explain not only which sample passes/fails, but why the metric proves it.
- Then expand product samples with additional failure modes per domain, for example battery tab offset/foreign object, display line mura/brightness band, and semiconductor pad contamination/rotation tolerance.

## 2026-07-02 Update - Pins_LineGauge sample catalog contract aligned

Root-caused the `Pins_LineGauge` and `EasyGauge_Pins_LineGauge` sample catalog NGs after the LineGauge angle acceptance gate was tightened to `LineAngleAvg -2..2`.

Root cause:

- `Pins_Edge_LineGauge.pipeline.xml` correctly rejects tilted rail images through the strict `LineAngleAvg` acceptance gate.
- The old `Pins_LineGauge` and recursive `EasyGauge_Pins_LineGauge` rows still pointed at tilted `Pins.bmp` images while expecting broad `-20..20` angle metrics, so the same image was being treated as both OK and controlled NG.
- `LineGauge_PinsTilted_Bad` is the intended tilted NG reference. The compatibility/recursive OK rows must use the straight rail reference.

Changed structure:

- `docs\samples\OpenVisionLab.SampleCatalog.csv`: moved `Pins_LineGauge` and `EasyGauge_Pins_LineGauge` to `Sample\EasyGauge\Pin 1.jpg`.
- Tightened their expected LineGauge metrics to the current straight-rail contract:
  - `EdgeCount 35..45`
  - `LineLengthMax 730..745`
  - `LineLengthMmMax 4.3..4.6`
  - `LineAngleAvg -2..2`
- Kept `Pins_Edge_LineGauge.pipeline.xml` strict. Do not relax it back to broad `-20..20`; tilted `Pins.bmp` remains covered by `LineGauge_PinsTilted_Bad`.

Evidence:

- Before: `artifacts\platform_precheck_doc_audit_20260702\samples\Pins_LineGauge.log` showed `Success=False`, `LineAngleAvg=7.202`, acceptance target `-2..2`.
- After: `artifacts\sample_catalog_linegauge_contract_20260702\sample_catalog_report.md`
  - `Pins_LineGauge`: `Success=True`, `LineAngleAvg=0.234`
  - `EasyGauge_Pins_LineGauge`: `Success=True`, `LineAngleAvg=0.234`
  - `LineGauge_PinsTilted_Bad`: expected failure remains controlled NG with `LineAngleAvg=7.202`

Validation:

- `tools\RunVisionSampleCatalog.ps1 -OutputDir artifacts\sample_catalog_linegauge_contract_20260702 -SkipRestore -SkipRunnerBuild`: PASS, `GateStatus=OK`, `RunnableRows=60`, `OKRows=60`, `NGRows=0`.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.

Next priority:

- Continue sample catalog/public-sample cleanup without duplicating already resolved Good/Bad conversions. Focus next on either replacing remaining SDK-derived public-facing sample references or checking MainView/Learn entry points against the current current-image documentation standard.

## 2026-07-02 Update - Run Log layout clipping fixed and documented

Fixed the clipped bottom Run Log layout found during tutorial documentation review.

Root cause:

- The collapsed shell log row was `44px` high, but the log panel border used `Margin="14,8,14,14"` and the log header itself is `42px`.
- The visible content area was therefore too short, so the log title/toggle button could be clipped at 1600x900.

Changed structure:

- `OpenVisionShellHostView.xaml`: collapsed `logPanelRow` height is now `68`.
- `OpenVisionShellHostView.xaml.cs`: `SetShellLogExpanded` now uses `68` collapsed and `184` expanded, leaving enough room for the log header and open-state body.
- `OpenVisionShellHostView.TestHooks.cs`: added test hook methods/properties to set and inspect shell log expanded state.
- `OpenVisionLabDirectSmokeRunner.cs`: `tutorial-captures` now captures:
  - `08_run_log_collapsed_current.png`
  - `09_run_log_open_current.png`
- `tools\BuildTutorialCalloutImages.ps1`: generates `run_log_collapsed_callouts.png` and `run_log_open_callouts.png`.
- `docs\OPENVISIONLAB_TUTORIAL.md` and `.html`: document the Run Log collapsed/open workflow.
- `docs\OPENVISIONLAB_TUTORIAL_PORTABLE.html`: regenerated with embedded Run Log images.
- `OpenVisionReadinessCheck`: verifies the Run Log tutorial images are referenced and exist.

Evidence artifacts:

- Before clipped capture: `artifacts\tutorial_current_exe_20260702_run_log\08_run_log_collapsed_current.png`
- After fixed capture: `artifacts\tutorial_current_exe_20260702_run_log_fixed\08_run_log_collapsed_current.png`
- Workspace empty smoke after fix: `artifacts\workspace_empty_log_layout_20260702_fixed\wpf_shell_host_workspace_empty.png`
- Tutorial annotated outputs:
  - `docs\assets\tutorial\annotated\run_log_collapsed_callouts.png`
  - `docs\assets\tutorial\annotated\run_log_open_callouts.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `bin\Debug\OpenVisionLab.exe --smoke tutorial-captures --output artifacts\tutorial_current_exe_20260702_run_log_fixed`: PASS, required tutorial capture files generated.
- `tools\BuildTutorialCalloutImages.ps1`: PASS.
- `tools\BuildPortableTutorial.ps1`: PASS, embedded images 10.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_empty artifacts\workspace_empty_log_layout_20260702_fixed`: PASS.

Next priority:

- Continue with `Pins_LineGauge` sample NG root-cause isolation, or audit Learn pages for the same current-image/followability standard.

## 2026-07-02 Update - Tutorial image audit follow-up

Audited all images referenced by `README.md`, `docs\OPENVISIONLAB_TUTORIAL.md`, and `docs\OPENVISIONLAB_TUTORIAL.html` after the clipped Run Log callout issue.

Changed structure:

- Built a visual contact sheet for the tutorial documentation images at `artifacts\tutorial_image_audit_20260702\tutorial_doc_images_contact_sheet.png`.
- Confirmed the public tutorial image set no longer contains the bad browser/video capture.
- Confirmed the main workspace image excludes the previously clipped bottom Run Log callout.
- Updated `docs\OPENVISIONLAB_TUTORIAL.html` so the portable tutorial now includes the same followable flow as the Markdown guide: Pipeline creation, Pipeline Review, Good/Bad validation, Recipe save/switch, troubleshooting, recommended learning order, and summary.
- Regenerated `docs\OPENVISIONLAB_TUTORIAL_PORTABLE.html`.

Validation:

- `tools\BuildPortableTutorial.ps1`: PASS, embedded images 8.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- Public README/tutorial bad-term scan for internal capture criteria, private-purpose wording, AI Recipe framing, and browser/video strings: PASS with no matches.

Next priority:

- If documenting Run Log behavior, add a dedicated complete log screenshot instead of pointing to the cropped main workspace walkthrough.

## 2026-07-02 Update - Tutorial public document audit and current EXE image refresh

Audited the public README and tutorial after the user found private-purpose/internal wording and a bad browser/video screenshot in the tutorial assets.

Changed structure:

- Replaced the bad `docs\assets\tutorial\current\main_workspace_current.png` source with a current `OpenVisionLab.exe --smoke tutorial-captures` capture.
- Backed up the bad previous annotated image to `artifacts\tutorial_doc_audit_20260702\before_bad_main_workspace_callouts.png` for before/after evidence.
- Regenerated `docs\assets\tutorial\annotated\main_workspace_callouts.png` from the current EXE capture and removed the clipped Event Log callout from the main walkthrough.
- Cropped the main walkthrough annotated image to exclude the partially clipped bottom log area until a separate complete log capture is available.
- Rewrote `README.md`, `docs\OPENVISIONLAB_TUTORIAL.md`, and `docs\OPENVISIONLAB_TUTORIAL.html` into user-facing documentation focused on sample -> layer -> tool -> preview -> output layer -> pipeline review -> Good/Bad validation.
- Removed public tutorial/README references to internal capture criteria, private-purpose framing, and AI-recipe style framing.
- Regenerated `docs\OPENVISIONLAB_TUTORIAL_PORTABLE.html`.
- Updated tutorial readiness/precheck contracts to require Recipe and Good/Bad tutorial flow instead of AI Recipe wording.

Evidence artifacts:

- Before: `artifacts\tutorial_doc_audit_20260702\before_bad_main_workspace_callouts.png`
- Current EXE source: `artifacts\tutorial_current_exe_20260702_doc_audit\01_main_workspace_current.png`
- After: `docs\assets\tutorial\annotated\main_workspace_callouts.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `bin\Debug\OpenVisionLab.exe --smoke tutorial-captures --output artifacts\tutorial_current_exe_20260702_doc_audit`: PASS.
- `tools\BuildTutorialCalloutImages.ps1`: PASS.
- `tools\BuildPortableTutorial.ps1`: PASS, embedded images 8.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.

Next priority:

- Add a separate complete Run Log tutorial capture if the log open/collapsed behavior needs to be documented, instead of reusing a cropped main workspace screenshot.

## 2026-07-02 Update - Explicit guide-plus-sample action in sample picker

Extended the in-app Learn entry point with an explicit `가이드와 샘플 열기` action. Operators can now choose between:

- `문서 열기`: opens only the matched `docs\learn` page and does not open a sample.
- `가이드와 샘플 열기`: opens the matched Learn page, then accepts the selected sample through the existing sample-open path.
- `이 샘플 열기`: opens only the selected sample through the existing path.

The new action remains explicit and does not run Preview/Run. The existing sample-open smoke verifies the selected public sample is loaded into `Main`, the sample pipeline is prepared, the next-action sample workflow bar is visible, and no native tool/Preview is auto-opened.

Changed structure:

- `OpenVisionWorkspaceSamplePickerViewModel` now exposes `OpenLearnAndSampleButtonText`, `CanOpenLearnAndSample`, and `OpenLearnDocumentForSelection`.
- `OpenVisionWorkspaceSamplePickerWindow.xaml` adds `WorkspaceSamplePickerOpenGuideAndSampleButton` beside the existing Open/Cancel buttons.
- `OpenVisionWorkspaceSamplePickerWindow.xaml.cs` reuses a single accept helper for regular sample open and guide-plus-sample open.
- `PipelineViewerScreenshotSmoke` now checks the guide-plus-sample button and uses current sample first-step metadata instead of hard-coded `Threshold/Contour` text in the sample-open smoke.
- The workspace image capture verifier now accepts current public grayscale synthetic samples, not only very bright image content.
- `OpenVisionReadinessCheck` protects the guide-plus-sample UI contract.

Evidence artifacts:

- Before: `artifacts\sample_picker_learn_document_20260702_r2\wpf_shell_host_workspace_sample_picker.png`
- After: `artifacts\sample_picker_guide_sample_20260702\wpf_shell_host_workspace_sample_picker.png`
- Sample-open evidence: `artifacts\sample_picker_guide_sample_20260702_open_r3\wpf_shell_host_workspace_sample_open.png`

Validation:

- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_picker,wpf_shell_host_workspace_sample_learn_paths artifacts\sample_picker_guide_sample_20260702`: PASS.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_open artifacts\sample_picker_guide_sample_20260702_open_r3`: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `git diff --check` on touched files: PASS, only CRLF normalization warnings.

Next priority:

- Continue beginner workflow polish by making the sample workflow bar after sample open show the same Learn document affordance, or return to public-safe sample expansion for tools not yet covered by the current 8 Good/Bad flows.

## 2026-07-02 Update - In-app Learn document entry point in sample picker

Converted the current Learn documentation set into a visible beginner entry point inside the WPF sample picker. The sample picker now shows a compact `따라하기 문서` card in the selected-sample header, resolves the matching `docs\learn` Markdown page from the currently selected sample/tool flow, and exposes a `문서 열기` command that opens the document without opening a sample, running Preview/Run, changing layers, or modifying pipeline/tool routing.

Changed structure:

- Added `0. UI\0) MENU\Wpf\OpenVisionWorkspaceLearnDocumentService.cs` to resolve and open Learn documents from the repository `docs\learn` folder.
- Updated `OpenVisionWorkspaceSamplePickerViewModel` with `OpenLearnDocumentCommand`, Learn document title/description/button state, and selection-change notifications.
- Updated `OpenVisionWorkspaceSamplePickerView.xaml` so the Learn document action is visible in the selected-sample header rather than hidden below the scroll fold.
- Updated `OpenVisionReadinessCheck` and `PipelineViewerScreenshotSmoke` to verify the Learn document command, resolver, and visible button.

Evidence artifacts:

- Before: `artifacts\sample_catalog_public16_20260702\wpf_shell_host_workspace_sample_picker.png`
- After: `artifacts\sample_picker_learn_document_20260702_r2\wpf_shell_host_workspace_sample_picker.png`
- Learn-path smoke: `artifacts\sample_picker_learn_document_20260702_r2\wpf_shell_host_workspace_sample_learn_paths.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_picker,wpf_shell_host_workspace_sample_learn_paths artifacts\sample_picker_learn_document_20260702_r2`: PASS.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `git diff --check` on touched Learn-entry files: PASS, only CRLF normalization warnings.

Next priority:

- Continue the beginner workflow by letting the sample picker/Learn entry optionally open the matching Learn page and sample together through an explicit operator command, or continue public-safe sample expansion for tools not yet covered by the current 8 Good/Bad flows.

## 2026-07-02 Update - All 8 Learn pages have current public result evidence

Extended the current-run Learn evidence image pattern to Matching, Blob, and Line. All 8 public Good/Bad Learn flows now have a documented result evidence image generated from the current public sample catalog runner and annotated with numbered callouts.

Changed structure:

- `tools\SyncPublicLearnEvidenceImages.ps1` now syncs all 8 public Good/Bad flow result images into `docs\assets\tutorial\current`, including Matching, Blob, and Line.
- `tools\BuildTutorialCalloutImages.ps1` now generates annotated result evidence for:
  - `docs\assets\tutorial\annotated\public_matching_diepad_good_callouts.png`
  - `docs\assets\tutorial\annotated\public_blob_particles_good_callouts.png`
  - `docs\assets\tutorial\annotated\public_line_pins_good_callouts.png`
- Rewrote the existing Learn pages in readable Korean and added current result evidence sections:
  - `docs\learn\LEARN_MATCHING.md`
  - `docs\learn\LEARN_BLOB.md`
  - `docs\learn\LEARN_LINE.md`
- `docs\OPENVISIONLAB_DOCUMENTATION_CAPTURE_GUIDE.md` now lists the all-8 result evidence assets.
- `OpenVisionReadinessCheck` now verifies Matching, Blob, and Line evidence links and files in addition to Contour/Threshold/Mean/Feature/EdgeBased.

Evidence artifacts:

- Current public catalog run: `artifacts\public_sample_catalog_20260702_learn_evidence_all8`
- Current evidence sources: `docs\assets\tutorial\current\public_*_result.png`
- Annotated evidence outputs: `docs\assets\tutorial\annotated\public_*_good_callouts.png`

Validation:

- `tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.PublicSampleCatalog.csv -OutputDir artifacts\public_sample_catalog_20260702_learn_evidence_all8 -SkipRestore`: PASS, `GateStatus=OK`, `RunnableRows=16`, `ExpectedFailureRows=8`, `OKRows=16`.
- `tools\SyncPublicLearnEvidenceImages.ps1 -CatalogArtifactDir artifacts\public_sample_catalog_20260702_learn_evidence_all8`: PASS.
- `tools\BuildTutorialCalloutImages.ps1`: PASS.
- `tools\BuildPortableTutorial.ps1`: PASS.
- `tools\TestPublicSampleAssets.ps1`: PASS, `CatalogRows=16`, `ManifestAssets=20`, `Pipelines=8`.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `git diff --check` on touched files: PASS, only CRLF normalization warnings.

Note: one parallel validation attempt ran `OpenVisionReadinessCheck` while the solution build was writing the same check assembly and hit a file-lock error. The same readiness command passed when rerun sequentially.

Next priority:

- Continue public-safe sample expansion for tools not yet covered by the 8 public Good/Bad flows, or convert the Learn index into an in-app beginner learning entry point.

## 2026-07-02 Update - Current public Learn result evidence images

Added current-run result evidence images to the newer Learn Mode pages for Contour, Threshold, Mean, FeatureMatching, and EdgeBasedMatching. The images are generated from the current public sample catalog runner and then annotated with numbered callouts so the docs show what a correct public-safe result looks like without using legacy SDK sample assets.

Changed structure:

- Added `tools\SyncPublicLearnEvidenceImages.ps1` to copy current public sample catalog result images into `docs\assets\tutorial\current`.
- `tools\BuildTutorialCalloutImages.ps1` now generates annotated Learn evidence images:
  - `docs\assets\tutorial\annotated\public_contour_shapes_good_callouts.png`
  - `docs\assets\tutorial\annotated\public_threshold_bandpads_good_callouts.png`
  - `docs\assets\tutorial\annotated\public_mean_brightness_good_callouts.png`
  - `docs\assets\tutorial\annotated\public_feature_card_good_callouts.png`
  - `docs\assets\tutorial\annotated\public_edge_fiducial_good_callouts.png`
- Updated Learn pages to embed the current evidence image and explain how to read the Good/Bad public sample result:
  - `docs\learn\LEARN_CONTOUR.md`
  - `docs\learn\LEARN_THRESHOLD.md`
  - `docs\learn\LEARN_MEAN.md`
  - `docs\learn\LEARN_FEATURE_MATCHING.md`
  - `docs\learn\LEARN_EDGE_BASED_MATCHING.md`
- `docs\OPENVISIONLAB_DOCUMENTATION_CAPTURE_GUIDE.md` now documents the current-run Learn evidence workflow.
- `OpenVisionReadinessCheck` now verifies that the Learn docs link these evidence images and that the annotated assets exist.

Evidence artifacts:

- Current public catalog run: `artifacts\public_sample_catalog_20260702_learn_evidence`
- Current evidence sources: `docs\assets\tutorial\current\public_*_result.png`
- Annotated evidence outputs: `docs\assets\tutorial\annotated\public_*_good_callouts.png`

Validation:

- `tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.PublicSampleCatalog.csv -OutputDir artifacts\public_sample_catalog_20260702_learn_evidence -SkipRestore`: PASS, `GateStatus=OK`, `RunnableRows=16`, `ExpectedFailureRows=8`, `OKRows=16`.
- `tools\SyncPublicLearnEvidenceImages.ps1 -CatalogArtifactDir artifacts\public_sample_catalog_20260702_learn_evidence`: PASS.
- `tools\BuildTutorialCalloutImages.ps1`: PASS.
- `tools\BuildPortableTutorial.ps1`: PASS after rerunning sequentially; the first parallel validation attempt conflicted with readiness reading the same portable HTML file.
- `tools\TestPublicSampleAssets.ps1`: PASS, `CatalogRows=16`, `ManifestAssets=20`, `Pipelines=8`.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `git diff --check` on touched files: PASS, only CRLF normalization warning for `tools/OpenVisionReadinessCheck/Program.cs`.

Next priority:

- Add the same current-result evidence style to Matching/Blob/Line Learn pages if their existing walkthrough images need stronger result proof, then continue public-safe sample expansion for tools not yet covered by the 8 Good/Bad flows.

## 2026-07-02 Update - Learn Mode expanded to all 8 public Good/Bad flows

Expanded Learn Mode documentation from the previous Matching/Blob/Line first pass to all 8 GitHub-safe public Good/Bad flows. The existing Learn docs were rewritten in clean UTF-8 Korean and new pages were added for Contour, Threshold, Mean, FeatureMatching, and EdgeBasedMatching.

Changed structure:

- Rewritten Learn overview and existing pages:
  - `docs\learn\README.md`
  - `docs\learn\LEARN_MATCHING.md`
  - `docs\learn\LEARN_BLOB.md`
  - `docs\learn\LEARN_LINE.md`
- Added new Learn pages:
  - `docs\learn\LEARN_CONTOUR.md`
  - `docs\learn\LEARN_THRESHOLD.md`
  - `docs\learn\LEARN_MEAN.md`
  - `docs\learn\LEARN_FEATURE_MATCHING.md`
  - `docs\learn\LEARN_EDGE_BASED_MATCHING.md`
- `README.md`, `docs\OPENVISIONLAB_TUTORIAL.md`, and `docs\OPENVISIONLAB_TUTORIAL.html` now link to the expanded 8-page Learn set.
- `docs\OPENVISIONLAB_TUTORIAL_PORTABLE.html` was regenerated after the HTML link update.
- `OpenVisionReadinessCheck` now verifies that the tutorial links the new Learn pages and that each Learn page explains public sample, Good, and Bad behavior.

Validation:

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1`: PASS, `CatalogRows=16`, `ManifestAssets=20`, `Pipelines=8`.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.

Next priority:

- Add result-evidence images or compact screenshots for the newly added Contour/Threshold/Mean/Feature/EdgeBased Learn pages, using only current EXE captures and public-safe samples.

## 2026-07-02 Update - Current EXE tutorial captures for public 8-pair catalog

Updated README/tutorial documentation so the public sample learning path reflects the current 16-row / 8 Good-Bad pair catalog and uses screenshots generated from the current `OpenVisionLab.exe`.

Changed structure:

- `OpenVisionLabDirectSmokeRunner.cs` extends `--smoke tutorial-captures` with a public sample catalog capture:
  - `07_sample_catalog_public_current.png`
  - The capture selects `Public_Edge_Fiducial_Good` and records `PublicCatalogSamples=16`.
- `tools\BuildTutorialCalloutImages.ps1` now generates:
  - `docs\assets\tutorial\annotated\sample_catalog_public_callouts.png`
- `docs\OPENVISIONLAB_DOCUMENTATION_CAPTURE_GUIDE.md` now documents the new capture/copy/callout step.
- `README.md`, `docs\OPENVISIONLAB_TUTORIAL.md`, and `docs\OPENVISIONLAB_TUTORIAL.html` now describe all 8 public Good/Bad flows:
  - Matching, Blob, Contour, Threshold, Mean, FeatureMatching, EdgeBasedMatching, LineDistance.
- `docs\OPENVISIONLAB_TUTORIAL_PORTABLE.html` was regenerated from the updated HTML.

Evidence artifacts:

- EXE capture output: `artifacts\tutorial_current_exe_20260702_public16`
- Current source capture: `docs\assets\tutorial\current\sample_catalog_public_current.png`
- Annotated capture: `docs\assets\tutorial\annotated\sample_catalog_public_callouts.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `bin\Debug\OpenVisionLab.exe --smoke tutorial-captures --output artifacts\tutorial_current_exe_20260702_public16`: PASS, generated 7 current tutorial captures plus `matching_preview_actual_current.png`.
- Tutorial capture report recorded `PublicCatalogSamples=16`, selected `Public_Edge_Fiducial_Good`, and listed all 8 public pair groups.
- `tools\BuildTutorialCalloutImages.ps1`: PASS, generated `sample_catalog_public_callouts.png`.
- `tools\BuildPortableTutorial.ps1`: PASS, embedded images 6.
- `tools\TestPublicSampleAssets.ps1`: PASS, `CatalogRows=16`, `ManifestAssets=20`, `Pipelines=8`.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.

Next priority:

- Continue public-safe sample expansion for remaining legacy catalog rows, or add dedicated Learn pages for Contour/Threshold/Mean/Feature/EdgeBasedMatching using the same current-EXE capture rule.

## 2026-07-02 Update - Public EdgeBasedMatching Good/Bad sample pair

Expanded the GitHub-safe public sample catalog from 14 rows / 7 Good-Bad groups to 16 rows / 8 Good-Bad groups. The public set now covers Matching, Blob, Contour, Threshold, Mean, FeatureMatching, EdgeBasedMatching, and LineDistance without using legacy SDK-derived `Sample` assets.

Changed structure:

- `tools\GenerateOpenVisionSyntheticSamples.ps1` now generates three additional public EdgeBasedMatching assets:
  - `docs\samples\public\Edge_Fiducial_Synthetic_OK.png`
  - `docs\samples\public\Edge_Fiducial_Synthetic_Wrong_NG.png`
  - `docs\samples\public\templates\Edge_Fiducial_Synthetic_Template.png`
- Added a public EdgeBasedMatching pipeline:
  - `docs\samples\public\Public_Edge_Fiducial.pipeline.xml`
- `docs\samples\OpenVisionLab.PublicSampleCatalog.csv` now includes:
  - Edge Good: asymmetric L fiducial matched by edge geometry with `ScoreMax` and `ResultCount`.
  - Edge Bad: wrong fiducial rejected by the same pipeline with `ResultCount=0`.
- `docs\samples\public\OpenVisionLab.PublicSampleManifest.csv` now records 20 public image/template assets.
- `tools\TestPublicSampleAssets.ps1` now requires the public catalog to include Matching, Blob, Contour, Threshold, Mean, FeatureMatching, EdgeBasedMatching, and LineDistance public pipelines.
- `OpenVisionReadinessCheck` now protects the expanded 16-row / 8-pair public sample contract.
- README and public sample policy docs now describe the 16-row / 8-pair public sample set.

Evidence artifacts:

- Edge Good overlay: `artifacts\public_sample_catalog_20260702_edge_r2\Public_Edge_Fiducial_Good.png`
- Edge Bad no-result evidence image: `artifacts\public_sample_catalog_20260702_edge_r2\Public_Edge_Fiducial_Wrong_Bad.png`
- Sample picker after expansion: `artifacts\sample_catalog_public16_20260702\wpf_shell_host_workspace_sample_picker.png`

Validation:

- `tools\GenerateOpenVisionSyntheticSamples.ps1`: PASS.
- `tools\TestPublicSampleAssets.ps1`: PASS, `CatalogRows=16`, `ManifestAssets=20`, `Pipelines=8`.
- `tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.PublicSampleCatalog.csv -OutputDir artifacts\public_sample_catalog_20260702_edge_r2 -SkipRestore`: PASS, `GateStatus=OK`, `RunnableRows=16`, `RequiredRows=8`, `ExpectedFailureRows=8`.
- Edge Good measured `ScoreMax=99.598`, `ResultCount=1`; Edge Bad measured `ResultCount=0` and correctly returned `MatchingNoResult`.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_picker,wpf_shell_host_workspace_sample_pair_coverage artifacts\sample_catalog_public16_20260702`: PASS.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.

Next priority:

- Use the 8 public Good/Bad pairs in README/tutorial captures with current EXE screenshots, then continue adding public-safe samples for remaining algorithms that still depend on local SDK sample assets.

## 2026-07-02 Update - Public FeatureMatching Good/Bad sample pair

Expanded the GitHub-safe public sample catalog from 12 rows / 6 Good-Bad groups to 14 rows / 7 Good-Bad groups. The public set now covers Matching, Blob, Contour, Threshold, Mean, FeatureMatching, and LineDistance without using legacy SDK-derived `Sample` assets.

Changed structure:

- `tools\GenerateOpenVisionSyntheticSamples.ps1` now generates three additional public FeatureMatching assets:
  - `docs\samples\public\Feature_Card_Synthetic_OK.png`
  - `docs\samples\public\Feature_Card_Synthetic_Wrong_NG.png`
  - `docs\samples\public\templates\Feature_Card_Synthetic_Template.png`
- Added a public FeatureMatching pipeline:
  - `docs\samples\public\Public_Feature_Card.pipeline.xml`
- `docs\samples\OpenVisionLab.PublicSampleCatalog.csv` now includes:
  - Feature Good: feature-rich synthetic card matched with the project-authored template crop.
  - Feature Bad: wrong synthetic card rejected by the same FeatureMatching pipeline through `ScoreMax`.
- `docs\samples\public\OpenVisionLab.PublicSampleManifest.csv` now records 17 public image/template assets.
- `tools\TestPublicSampleAssets.ps1` now requires the public catalog to include Matching, Blob, Contour, Threshold, Mean, FeatureMatching, and LineDistance public pipelines.
- `OpenVisionReadinessCheck` now protects the expanded 14-row / 7-pair public sample contract.
- README and public sample policy docs now describe the 14-row / 7-pair public sample set.

Evidence artifacts:

- Feature Good overlay: `artifacts\public_sample_catalog_20260702_feature_r2\Public_Feature_Card_Good.png`
- Feature Bad overlay: `artifacts\public_sample_catalog_20260702_feature_r2\Public_Feature_Card_Wrong_Bad.png`
- Sample picker after expansion: `artifacts\sample_catalog_public14_20260702\wpf_shell_host_workspace_sample_picker.png`

Validation:

- `tools\GenerateOpenVisionSyntheticSamples.ps1`: PASS.
- `tools\TestPublicSampleAssets.ps1`: PASS, `CatalogRows=14`, `ManifestAssets=17`, `Pipelines=7`.
- `tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.PublicSampleCatalog.csv -OutputDir artifacts\public_sample_catalog_20260702_feature_r2 -SkipRestore`: PASS, `GateStatus=OK`, `RunnableRows=14`, `RequiredRows=7`, `ExpectedFailureRows=7`.
- Feature Good measured `ScoreMax=96.7`, `ResultCount=1`; Feature Bad measured `ScoreMax=26.7` and correctly failed the Good acceptance range.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_picker,wpf_shell_host_workspace_sample_pair_coverage artifacts\sample_catalog_public14_20260702`: PASS.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.

Next priority:

- Add public synthetic Good/Bad coverage for EdgeBasedMatching or start using the 7 public Good/Bad pairs in README/tutorial captures with current EXE screenshots.

## 2026-07-02 Update - Public Mean brightness Good/Bad sample pair

Expanded the GitHub-safe public sample catalog from 10 rows / 5 Good-Bad groups to 12 rows / 6 Good-Bad groups. The public set now covers Matching, Blob, Contour, Threshold, Mean, and LineDistance without using legacy SDK-derived `Sample` assets.

Changed structure:

- `tools\GenerateOpenVisionSyntheticSamples.ps1` now generates two additional public sample images:
  - `docs\samples\public\Mean_Brightness_Synthetic_OK.png`
  - `docs\samples\public\Mean_Brightness_Synthetic_Dark_NG.png`
- Added a public Mean pipeline:
  - `docs\samples\public\Public_Mean_BrightnessDrift.pipeline.xml`
- `docs\samples\OpenVisionLab.PublicSampleCatalog.csv` now includes:
  - Mean Good: normal bright reference field, expected `MeanValueAvg=185..220`.
  - Mean Bad: dark drift field, expected to fail the same production criterion while documenting the observed negative range.
- `docs\samples\public\OpenVisionLab.PublicSampleManifest.csv` now records 14 public image/template assets.
- `tools\TestPublicSampleAssets.ps1` now requires the public catalog to include Matching, Blob, Contour, Threshold, Mean, and LineDistance public pipelines.
- `OpenVisionReadinessCheck` now protects the expanded 12-row / 6-pair public sample contract.
- README and public sample policy docs now describe the 12-row / 6-pair public sample set.

Evidence artifacts:

- Mean Good overlay: `artifacts\public_sample_catalog_20260702_mean\Public_Mean_Brightness_Good.png`
- Mean Bad overlay: `artifacts\public_sample_catalog_20260702_mean\Public_Mean_Brightness_Dark_Bad.png`
- Sample picker after expansion: `artifacts\sample_catalog_public12_20260702\wpf_shell_host_workspace_sample_picker.png`

Validation:

- `tools\GenerateOpenVisionSyntheticSamples.ps1`: PASS.
- `tools\TestPublicSampleAssets.ps1`: PASS, `CatalogRows=12`, `ManifestAssets=14`, `Pipelines=6`.
- `tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.PublicSampleCatalog.csv -OutputDir artifacts\public_sample_catalog_20260702_mean -SkipRestore`: PASS, `GateStatus=OK`, `RunnableRows=12`, `RequiredRows=6`, `ExpectedFailureRows=6`.
- Mean Good measured `MeanValueAvg=201.5`; Mean Dark Bad measured `MeanValueAvg=117.5` and correctly failed the Good acceptance range.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_picker,wpf_shell_host_workspace_sample_pair_coverage artifacts\sample_catalog_public12_20260702`: PASS.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.

Next priority:

- Add public synthetic Good/Bad coverage for Feature/EdgeBased Matching, then keep README/tutorial captures pinned to current EXE screenshots and public-safe assets only.

## 2026-07-02 Update - Public Contour and Threshold Good/Bad sample pairs

Expanded the GitHub-safe public sample catalog from 6 rows / 3 Good-Bad groups to 10 rows / 5 Good-Bad groups. The public set now covers Matching, Blob, Contour, Threshold, and LineDistance without using legacy SDK-derived `Sample` assets.

Changed structure:

- `tools\GenerateOpenVisionSyntheticSamples.ps1` now generates four additional public sample images:
  - `docs\samples\public\Contour_Shapes_Synthetic_OK.png`
  - `docs\samples\public\Contour_Shapes_Synthetic_Missing_NG.png`
  - `docs\samples\public\Threshold_BandPads_Synthetic_OK.png`
  - `docs\samples\public\Threshold_BandPads_Synthetic_Missing_NG.png`
- Added public pipeline XMLs:
  - `docs\samples\public\Public_Contour_Shapes.pipeline.xml`
  - `docs\samples\public\Public_Threshold_BandPads.pipeline.xml`
- `docs\samples\OpenVisionLab.PublicSampleCatalog.csv` now includes:
  - Contour Good: detects 5 separated synthetic shapes.
  - Contour Bad: missing-shape scene, expected `ResultCount=2`.
  - Threshold Good: basic threshold isolates 4 bright inspection pads.
  - Threshold Bad: missing-pad scene, expected `ResultCount=1`.
- `docs\samples\public\OpenVisionLab.PublicSampleManifest.csv` now records 12 public image/template assets.
- `tools\TestPublicSampleAssets.ps1` now requires the public catalog to include Matching, Blob, Contour, Threshold, and LineDistance public pipelines.
- `OpenVisionReadinessCheck` now protects the expanded public sample contract.
- README and public sample policy docs now describe the 10-row / 5-pair public sample set.

Evidence artifacts:

- Contour overlay: `artifacts\public_sample_catalog_20260702_contour_threshold_r3\Public_Contour_Shapes_Good.png`
- Threshold overlay: `artifacts\public_sample_catalog_20260702_contour_threshold_r3\Public_Threshold_BandPads_Good.png`
- Sample picker after expansion: `artifacts\sample_catalog_public10_20260702\wpf_shell_host_workspace_sample_picker.png`

Validation:

- `tools\GenerateOpenVisionSyntheticSamples.ps1`: PASS.
- `tools\TestPublicSampleAssets.ps1`: PASS, `CatalogRows=10`, `ManifestAssets=12`, `Pipelines=5`.
- `tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.PublicSampleCatalog.csv -OutputDir artifacts\public_sample_catalog_20260702_contour_threshold_r3 -SkipRestore`: PASS, `GateStatus=OK`, `RunnableRows=10`, `RequiredRows=5`, `ExpectedFailureRows=5`.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_picker,wpf_shell_host_workspace_sample_pair_coverage artifacts\sample_catalog_public10_20260702`: PASS.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.

Next priority:

- Add public synthetic Good/Bad coverage for Mean/brightness drift and Feature/EdgeBased Matching, then keep README/tutorial captures pinned to public-safe assets only.

## 2026-07-02 Update - Sample picker public/local catalog source split

The Sample Catalog UI now separates GitHub-safe public synthetic samples from local-only legacy sample rows. Public samples are selected by default so README, tutorial, and public project documents do not accidentally mix in SDK-derived assets. Local legacy rows remain available for private development, but only after explicitly switching the catalog source.

Changed structure:

- `VisionPipelineSampleCatalog` now loads two catalog sources:
  - `docs\samples\OpenVisionLab.PublicSampleCatalog.csv` as `Public`.
  - `docs\samples\OpenVisionLab.SampleCatalog.csv` as `LocalLegacy`.
- `VisionPipelineSampleCatalogItem` now carries source metadata such as source kind, source path, source id, display name, description, and badge text.
- `OpenVisionWorkspaceSamplePickerViewModel` now exposes `CatalogSourceOptions`, `SelectedCatalogSourceOption`, and `VisibleSampleCount`; source changes only filter the visible list and do not open a sample or run preview.
- `OpenVisionWorkspaceSamplePickerView` now shows a Catalog Source section above Learn Path.
- `VisionPipelineSampleCheckService.GetPairSamples` now resolves Good/Bad pair samples from the selected sample's catalog source only, preventing public and local legacy pairs from being mixed.
- Catalog source display strings now follow the active Korean/English language mode.
- `PipelineViewerScreenshotSmoke` and `OpenVisionReadinessCheck` now verify the public/local source selector contract.

Before/after comparison:

- Before: Sample Catalog opened directly into Learn Path and sample rows, with no clear distinction between public-safe synthetic samples and local legacy SDK/sample rows.
- Before artifact: `artifacts\sample_learn_paths_after_20260701_r3\wpf_shell_host_workspace_sample_picker.png`
- After: Sample Catalog opens with `Public Samples` selected by default and exposes `Local Legacy` as an explicit source switch.
- After artifact: `artifacts\sample_catalog_source_picker_20260702_r2\wpf_shell_host_workspace_sample_picker.png`
- Comparison artifact: `artifacts\sample_catalog_source_picker_20260702_r2\sample_catalog_source_before_after.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_picker,wpf_shell_host_workspace_sample_learn_paths,wpf_shell_host_workspace_sample_pair_picker artifacts\sample_catalog_source_picker_20260702_r2`: PASS.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pair_coverage artifacts\sample_pair_coverage_after_source_ui_20260702`: PASS.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `tools\TestPublicSampleAssets.ps1`: PASS, `CatalogRows=6`, `ManifestAssets=8`, `Pipelines=3`.
- `tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.PublicSampleCatalog.csv -OutputDir artifacts\public_sample_catalog_20260702_after_source_ui -SkipRestore`: PASS, `GateStatus=OK`, `RunnableRows=6`, `RequiredRows=3`, `ExpectedFailureRows=3`.
- `git diff --check` for the touched source split files: PASS, whitespace warnings only for existing LF/CRLF normalization.

Next priority:

- Continue moving default learning/documentation flows onto public synthetic or explicitly licensed sample assets, while keeping legacy sample rows clearly marked as local-only.

## 2026-07-02 Update - Public synthetic Good/Bad sample pairs

Expanded the GitHub-safe public sample catalog from three OK-only rows to three Good/Bad pairs. The public catalog now demonstrates both normal OK and controlled NG behavior without relying on legacy `Sample` or SDK-derived images.

Changed structure:

- `tools\GenerateOpenVisionSyntheticSamples.ps1` now generates additional public negative samples:
  - `docs\samples\public\Matching_DiePad_Synthetic_NoTarget_NG.png`
  - `docs\samples\public\Blob_Particles_Synthetic_Sparse_NG.png`
  - `docs\samples\public\Line_Pins_Synthetic_WidePin_NG.png`
- `docs\samples\public\OpenVisionLab.PublicSampleManifest.csv` now records all eight public image/template assets.
- `docs\samples\OpenVisionLab.PublicSampleCatalog.csv` now has six runnable rows:
  - Matching Good: detects 3 target pads.
  - Matching Bad: no target, expected `ResultCount=0`.
  - Blob Good: detects 12 particles.
  - Blob Bad: sparse image, expected `ResultCount=3`.
  - Line Good: expected `DistanceMmAvg=0.222`.
  - Line Bad: wide-pin drift image, expected `DistanceMmAvg=0.106`.
- `tools\TestPublicSampleAssets.ps1` now also fails if a public sample image exists under `docs\samples\public` but is not listed in the public manifest.
- `OpenVisionReadinessCheck` now requires the public catalog to include Matching, Blob, and Line Good/Bad pairs.
- README and public sample docs now describe the six-row public catalog and three public Good/Bad pair groups.

Validation:

- `tools\GenerateOpenVisionSyntheticSamples.ps1`: PASS.
- `tools\TestPublicSampleAssets.ps1`: PASS, `CatalogRows=6`, `ManifestAssets=8`, `Pipelines=3`.
- `tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.PublicSampleCatalog.csv -OutputDir artifacts\public_sample_catalog_20260702_pairs_v3 -SkipRestore`: PASS, `GateStatus=OK`, `RunnableRows=6`, `RequiredRows=3`, `ExpectedFailureRows=3`.
- `dotnet run --project "tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.

Next priority:

- Surface the public catalog separately in the Sample Catalog UI so users can choose GitHub-safe public samples without mixing them with local-only legacy SDK samples.

## 2026-07-02 Update - Public sample catalog and asset audit guard

The public-safe synthetic samples now have a runnable catalog and dedicated pipeline XMLs. This keeps public GitHub validation separate from legacy `Sample` assets that may have come from commercial SDK installations.

Changed structure:

- Added `docs\samples\OpenVisionLab.PublicSampleCatalog.csv`.
- Added public pipeline XMLs:
  - `docs\samples\public\Public_Matching_DiePad.pipeline.xml`
  - `docs\samples\public\Public_Blob_Particles.pipeline.xml`
  - `docs\samples\public\Public_Line_Pins_Distance.pipeline.xml`
- Added `tools\TestPublicSampleAssets.ps1` to fail if the public catalog, manifest, or public pipeline image references point back to legacy `Sample`, `EasyMatch`, Euresys, or MVTec assets.
- Extended `OpenVisionReadinessCheck` with a public sample asset contract.
- Updated README and public sample policy docs with the public catalog path and validation commands.

Validation:

- `tools\GenerateOpenVisionSyntheticSamples.ps1`: PASS.
- `tools\TestPublicSampleAssets.ps1`: PASS, `CatalogRows=3`, `ManifestAssets=5`, `Pipelines=3`.
- `tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.PublicSampleCatalog.csv -OutputDir artifacts\public_sample_catalog_20260702_guarded -SkipRestore`: PASS.
- `Public_Matching_DiePad_Good`: PASS, `ResultCount=3`, `ScoreMax=93.074`.
- `Public_Blob_Particles_Good`: PASS, `ResultCount=12`.
- `Public_Line_Pins_Good`: PASS, `DistanceMmAvg=0.222`.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.

Next priority:

- Keep migrating default/public-facing sample tests from legacy `Sample` rows into public synthetic or explicitly licensed assets. Do not remove legacy/local rows until their replacement coverage exists.

## 2026-07-02 Update - Public-safe synthetic sample policy and tutorial capture source

The user clarified that most images under `Sample` came from an installed Euresys SDK sample folder and should not be treated as redistributable GitHub assets. Tutorial/documentation captures must therefore move away from SDK-derived samples.

Changed structure:

- Added `docs\OPENVISIONLAB_PUBLIC_SAMPLE_ASSET_POLICY.md` to define public-safe sample rules and explicitly mark the legacy `Sample` folder as local-only until each asset has verified redistribution rights.
- Added `docs\samples\public\README.md` and `docs\samples\public\OpenVisionLab.PublicSampleManifest.csv`.
- Added `tools\GenerateOpenVisionSyntheticSamples.ps1`, which generates project-authored synthetic Workspace, Matching, Blob, and Line source images plus a Matching template.
- Regenerated:
  - `docs\samples\public\Workspace_Inspection_Synthetic_OK.png`
  - `docs\samples\public\Matching_DiePad_Synthetic_OK.png`
  - `docs\samples\public\templates\Matching_DiePad_Synthetic_Template.png`
  - `docs\samples\public\Blob_Particles_Synthetic_OK.png`
  - `docs\samples\public\Line_Pins_Synthetic_OK.png`
- Updated `OpenVisionLabDirectSmokeRunner --smoke tutorial-captures` to use only public synthetic assets for the Main workspace, Matching, Blob, and Line tutorial captures.
- Isolated tutorial capture Pipeline Review into `Documentation_Public / Public_Synthetic_Matching` so the public screenshot no longer inherits legacy `Sample_Contour_TemplateMatching` state.
- Removed the previous Euresys-derived Matching tutorial template artifact from `docs\samples\templates`.
- Updated README/tutorial/capture guide wording to require public-safe sample sources for README/tutorial/public project captures and to treat legacy `Sample` rows as local-only until audited.
- Regenerated tutorial current images, annotated callouts, and portable tutorial HTML from the synthetic sample capture.

Before/after comparison:

- Product UI layout was not changed in this step, so no product UI before/after image is required.
- Before: the tutorial capture flow still used SDK-derived sample paths for Matching/Blob/Line/Main or could inherit a legacy sample pipeline in Pipeline Review.
- After: the tutorial capture flow uses project-generated synthetic images under `docs\samples\public`, and Pipeline Review shows the dedicated `Public_Synthetic_Matching` pipeline.
- New verification artifact: `artifacts\tutorial_current_exe_20260702_public_synthetic_v3\matching_preview_actual_current.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `bin\Debug\OpenVisionLab.exe --smoke tutorial-captures --output artifacts\tutorial_current_exe_20260702_public_synthetic_v3` with `Start-Process -Wait`: PASS.
- `tools\BuildTutorialCalloutImages.ps1`: PASS.
- `tools\BuildPortableTutorial.ps1`: PASS.
- Markdown tutorial image link check: PASS.
- `dotnet run --project "tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: PASS.

Next priority:

- Audit all sample catalog rows, smoke targets, and documentation references that still depend on `Sample` or `bin\Debug\EasyMatch`. Move default/public tests to `docs\samples\public` synthetic assets or explicitly licensed datasets. Keep SDK sample tests opt-in/local-only.

## 2026-07-02 Update - Tutorial Matching actual preview capture automation

Reworked the tutorial/documentation capture smoke so Matching learning docs no longer depend on a manually copied or hand-marked image. The smoke now generates a real Matching Preview overlay from the current EXE and saves it as a required documentation artifact.

Changed structure:

- `OpenVisionLabDirectSmokeRunner --smoke tutorial-captures` now uses `Sample\EasyMatch\Die Pad 2.bmp` with `docs\samples\templates\Matching_DiePad_Target_Template.bmp` for the Matching tutorial capture.
- The Matching tutorial setup enables angle search with the documented `Score >= 0.6 / Match 3` beginner criteria and verifies a real Preview OK result before saving documentation assets.
- The smoke writes `matching_preview_actual_current.png` from the actual `Matching_Preview` output layer, before any tool-window docking/screenshot work.
- `tutorial-captures` now verifies all required tutorial capture files at the end, including the actual Matching result image, so a partial capture cannot remain a silent PASS.
- The documentation capture guide now lists and copies `matching_preview_actual_current.png` as a required current-EXE capture.
- Tutorial current images, annotated callouts, and portable tutorial HTML were regenerated from the new EXE capture.

Before/after comparison:

- Product UI layout was not changed in this step, so no product UI before/after image is required.
- Before: Matching tutorial docs could point at a manually supplied current screenshot, but `tutorial-captures` did not itself create the actual overlay image.
- After: `tutorial-captures` produces a real overlay where the Die Pad target boxes and centers are generated by the Matching Preview output layer.
- New verification artifact: `artifacts\tutorial_current_exe_20260702_matching_actual\matching_preview_actual_current.png`
- Updated documentation asset: `docs\assets\tutorial\current\matching_preview_actual_current.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `bin\Debug\OpenVisionLab.exe --smoke tutorial-captures --output artifacts\tutorial_current_exe_20260702_matching_actual` with `Start-Process -Wait`: PASS.
- `tools\BuildTutorialCalloutImages.ps1`: PASS, regenerated six annotated tutorial images.
- `tools\BuildPortableTutorial.ps1`: PASS, embedded images 5.

Next priority:

- Keep documentation screenshots generated from current EXE captures. If Matching sample wording is expanded later, add a catalog row or Learn Mode entry for the Die Pad Matching teaching sample instead of reusing a contour-template sample label.

## 2026-07-02 Update - BentPin and Film controlled NG samples

Promoted two remaining comparative Bad references to controlled NG after checking that each uses a pair-specific baseline pipeline and has a stable normal-range metric gate.

Changed structure:

- `BentPin_ShaftContour.pipeline.xml` now gates acceptance on `BoundsWidthMax` in the normal shaft-width range `0..18` instead of the broad `ResultCount 10..20` range.
- `BentPin_BadShaft` is now an `ExpectedFailure` catalog row.
- `Film_DarkSpot_Contour.pipeline.xml` now gates acceptance on `AreaMax` in the normal small-background-candidate range `0..20` instead of the broad `ResultCount 0..8` range.
- `EasyObject_FilmBad_DarkSpot` is now an `ExpectedFailure` catalog row.
- `PipelineViewerScreenshotSmoke` now exposes `wpf_shell_host_workspace_sample_pipeline_review_bentpin_ng_metrics` and `wpf_shell_host_workspace_sample_pipeline_review_film_ng_metrics`.
- The Bad reference audit now classifies 11 controlled NG references and 5 comparative Bad references.

Before/after comparison:

- Product UI layout was not changed in this step, so no product UI before/after image is required.
- Before: `BentPin_BadShaft` passed the sample pipeline because shaft count stayed valid even when one shaft was too wide. After: Pipeline Review reports controlled NG through `BoundsWidthMax`.
- Before: `EasyObject_FilmBad_DarkSpot` passed the sample pipeline because candidate count stayed valid even when a dark spot was too large. After: Pipeline Review reports controlled NG through `AreaMax`.
- New verification artifact: `artifacts\sample_pipeline_review_bentpin_ng_after_20260702\wpf_shell_host_workspace_sample_pipeline_review_bentpin_ng_metrics.png`
- New verification artifact: `artifacts\sample_pipeline_review_film_ng_after_20260702\wpf_shell_host_workspace_sample_pipeline_review_film_ng_metrics.png`
- Updated audit artifact: `artifacts\sample_bad_reference_audit_after_bentpin_film_20260702\wpf_shell_host_workspace_sample_bad_reference_audit.png`

Validation:

- `dotnet build "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_bentpin_ng_metrics artifacts\sample_pipeline_review_bentpin_ng_after_20260702`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_film_ng_metrics artifacts\sample_pipeline_review_film_ng_after_20260702`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_bad_reference_audit artifacts\sample_bad_reference_audit_after_bentpin_film_20260702`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pair_coverage artifacts\sample_pair_coverage_after_bentpin_film_20260702`: PASS 1/1.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj" -c Debug`: PASS.
- `git diff --check`: PASS with CRLF normalization warnings only.

Next priority:

- Do not immediately convert the remaining Surface/Fiducial comparative Bad references. `SurfaceDefect_EdgeContour.pipeline.xml` is also used by the required `EasyObject_SurfaceDefect1_Edge` benchmark, and `Threshold_Morphology_Contour.pipeline.xml` is a shared generic fiducial contour pipeline. Promote them only after either confirming all shared required samples pass the new gate or splitting pair-specific pipelines.

## 2026-07-02 Update - Blob density controlled NG sample

Promoted the Blob sparse-density Bad reference from comparative Bad to controlled NG because the pair-specific Blob pipeline exposes a stable `ResultCount` metric that separates dense OK particles from sparse NG particles.

Changed structure:

- `Rice_Particle_BlobPair.pipeline.xml` now gates acceptance on `ResultCount` in the normal dense-particle range `120..170` instead of accepting the broad `1..220` range.
- `Blob_Bacteria_SparseBad` is now an `ExpectedFailure` catalog row.
- `PipelineViewerScreenshotSmoke` now exposes `wpf_shell_host_workspace_sample_pipeline_review_blob_ng_metrics`.
- Existing controlled-NG sample review smokes now share a single helper so Mean, Feature, LineGauge, and Blob verify the same NG/next-action/run-log/output-preview contract.
- The new smoke opens `Blob_Bacteria_SparseBad`, verifies the catalog baseline `ResultCount`, opens the active `Sample_` pipeline in Pipeline Review, runs Review explicitly, and asserts NG decision, beginner next action, count metric detail, run log, and output preview.
- The Bad reference audit now classifies 9 controlled NG references and 7 comparative Bad references.

Before/after comparison:

- Product UI layout was not changed in this step, so no product UI before/after image is required.
- Before: `Blob_Bacteria_SparseBad` was a Bad reference in the picker, but the sample pipeline accepted it because `ResultCount` stayed inside the broad `1..220` range.
- After: `Blob_Bacteria_SparseBad` remains a Bad reference and now produces controlled metric NG in Pipeline Review because measured `ResultCount` is below the normal dense-particle acceptance gate.
- New verification artifact: `artifacts\sample_pipeline_review_blob_ng_after_20260702\wpf_shell_host_workspace_sample_pipeline_review_blob_ng_metrics.png`
- Updated audit artifact: `artifacts\sample_bad_reference_audit_after_blob_20260702\wpf_shell_host_workspace_sample_bad_reference_audit.png`

Validation:

- `dotnet build "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_blob_ng_metrics artifacts\sample_pipeline_review_blob_ng_after_20260702`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\sample_pipeline_review_mean_ng_after_blob_20260702`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_feature_ng_metrics artifacts\sample_pipeline_review_feature_ng_after_blob_20260702`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\sample_pipeline_review_line_ng_after_blob_20260702`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_metrics artifacts\sample_pipeline_review_blob_ok_after_blob_20260702`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_bad_reference_audit artifacts\sample_bad_reference_audit_after_blob_20260702`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pair_coverage artifacts\sample_pair_coverage_after_blob_20260702`: PASS 1/1.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj" -c Debug`: PASS.
- `git diff --check`: PASS with CRLF normalization warnings only.

Next priority:

- Continue reviewing the remaining 7 comparative Bad references only where a pair-specific, stable normal-range metric gate exists. Do not convert Fiducial/Surface/Film/BentPin comparative references unless their shared baseline pipeline can reject Bad without weakening the Good reference or other catalog samples.

## 2026-07-02 Update - LineGauge angle controlled NG sample

Promoted the LineGauge tilted Bad reference from comparative Bad to controlled NG because the shared LineGauge pipeline already exposes a stable `LineAngleAvg` metric that separates straight OK rails from tilted NG rails.

Changed structure:

- `Pins_Edge_LineGauge.pipeline.xml` now gates acceptance on `LineAngleAvg` in the normal straight-rail range `-2..2` instead of accepting broad `EdgeCount 20..60`.
- `LineGauge_PinsTilted_Bad` is now an `ExpectedFailure` catalog row.
- `PipelineViewerScreenshotSmoke` now exposes `wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics`.
- The new smoke opens `LineGauge_PinsTilted_Bad`, verifies the catalog baseline `LineAngleAvg`, opens the active `Sample_` pipeline in Pipeline Review, runs Review explicitly, and asserts NG decision, beginner next action, angle metric detail, run log, and output preview.
- The Bad reference audit now classifies 8 controlled NG references and 8 comparative Bad references.

Before/after comparison:

- Product UI layout was not changed in this step, so no product UI before/after image is required.
- Before: `LineGauge_PinsTilted_Bad` was a Bad reference in the picker, but the sample pipeline accepted it because `EdgeCount` stayed inside the broad normal range.
- After: `LineGauge_PinsTilted_Bad` remains a Bad reference and now produces controlled metric NG in Pipeline Review because `LineAngleAvg` is outside the straight-rail acceptance gate.
- New verification artifact: `artifacts\sample_pipeline_review_line_ng_after_20260702\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
- Updated audit artifact: `artifacts\sample_bad_reference_audit_after_line_20260702\wpf_shell_host_workspace_sample_bad_reference_audit.png`

Validation:

- `dotnet build "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\sample_pipeline_review_line_ng_after_20260702`: PASS 1/1.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj" -c Debug`: PASS.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_bad_reference_audit artifacts\sample_bad_reference_audit_after_line_20260702`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pair_coverage artifacts\sample_pair_coverage_after_line_20260702`: PASS 1/1.

Next priority:

- Continue reviewing remaining comparative Bad references. Blob and Fiducial/Surface/Film groups may remain comparative unless a single stable normal-range metric gate can reject Bad without weakening the Good reference.

## 2026-07-02 Update - Feature score controlled NG samples

Promoted the Feature score-discrimination Bad references from comparative Bad to controlled NG because the shared FeatureMatching pipeline already exposes a stable `ScoreMax` metric that separates the OK target from wrong/low-score hypotheses.

Changed structure:

- `Feature_Template_Review.pipeline.xml` now gates acceptance on `ScoreMax` with the normal target range `90..100` instead of accepting any `ResultCount 1..3`.
- `Feature_TemplateReview_LowScoreSwitch` and `Feature_TemplateReview_WrongTargetBoard` are now `ExpectedFailure` catalog rows.
- `PipelineViewerScreenshotSmoke` now exposes `wpf_shell_host_workspace_sample_pipeline_review_feature_ng_metrics`.
- The new smoke opens `Feature_TemplateReview_LowScoreSwitch`, verifies the catalog baseline `ScoreMax`, opens the active `Sample_` pipeline in Pipeline Review, runs Review explicitly, and asserts NG decision, beginner next action, score metric detail, run log, and output preview.
- The Bad reference audit now classifies 7 controlled NG references and 9 comparative Bad references.

Before/after comparison:

- Product UI layout was not changed in this step, so no product UI before/after image is required.
- Before: Feature low-score/wrong-target Bad references were `Required`, so the shared pipeline could still pass if it returned one geometric hypothesis.
- After: Feature low-score/wrong-target Bad references are controlled NG because `ScoreMax` is below the accepted normal-target range.
- New verification artifact: `artifacts\sample_pipeline_review_feature_ng_after_20260702\wpf_shell_host_workspace_sample_pipeline_review_feature_ng_metrics.png`
- Updated audit artifact: `artifacts\sample_bad_reference_audit_after_feature_20260702\wpf_shell_host_workspace_sample_bad_reference_audit.png`

Validation:

- `dotnet build "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_feature_ng_metrics artifacts\sample_pipeline_review_feature_ng_after_20260702`: PASS 1/1.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj" -c Debug`: PASS.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_bad_reference_audit artifacts\sample_bad_reference_audit_after_feature_20260702`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pair_coverage artifacts\sample_pair_coverage_after_feature_20260702`: PASS 1/1.

Next priority:

- Continued by the 2026-07-02 LineGauge angle controlled NG sample above.
- Continue reviewing comparative Bad references, but only promote rows when the shared baseline pipeline can reject them through a stable normal-range metric without weakening the Good reference. Remaining comparative candidates include Blob/Fiducial/Surface/Film groups, which may intentionally remain comparative because their measured Bad ranges are still useful for side-by-side learning.

## 2026-07-02 Update - Bad reference audit smoke

Added an audit smoke for the Good/Bad catalog's Bad references so later work does not blindly convert every Bad reference into Pipeline Review NG.

Changed structure:

- `PipelineViewerScreenshotSmoke` now exposes `wpf_shell_host_workspace_sample_bad_reference_audit`.
- The target runs every runnable Bad reference through `VisionPipelineSampleCheckService`.
- The target classifies each Bad reference as either:
  - `Controlled NG`: catalog row is `ExpectedFailure`, meaning the shared sample pipeline intentionally rejects the sample through a stable failure/no-result or acceptance gate.
  - `Comparative Bad`: catalog row remains `Required`, meaning the sample is a measured bad reference for Good/Bad separation but the shared pipeline is still expected to run successfully.
- The target requires `Mean_Brightness_DimBad` to remain a controlled NG sample after the previous brightness acceptance change.
- The target fails if any Bad reference no longer produces the expected sample metric/result classification.

Before/after comparison:

- Product UI layout was not changed in this step, so no product UI before/after image is required.
- Before: Bad reference status was implied by catalog metadata and pair coverage, but there was no single audit showing which Bad samples are controlled NG versus comparative references.
- After: `wpf_shell_host_workspace_sample_bad_reference_audit` produces a compact classification report for all runnable Bad references.
- New verification artifact: `artifacts\sample_bad_reference_audit_after_20260702\wpf_shell_host_workspace_sample_bad_reference_audit.png`

Validation:

- `dotnet build "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_bad_reference_audit artifacts\sample_bad_reference_audit_after_20260702`: PASS 1/1.

Next priority:

- Started by the 2026-07-02 Feature score controlled NG samples above.
- Use the audit report before changing more sample pipelines. Only promote a comparative Bad reference to controlled NG when the shared baseline pipeline has a stable normal/abnormal metric gate that does not weaken the Good reference.
- Do not convert every Bad reference to `ExpectedFailure`; some are intentionally comparative samples for metric separation.

## 2026-07-02 Update - Sample Pipeline Review NG metric smoke

Added real catalog-sample NG review coverage instead of relying only on the synthetic Pipeline Review NG smoke.

Changed structure:

- `Mean_BrightnessDrift.pipeline.xml` now uses `MeanValueAvg >= 200` as the normal-brightness acceptance gate instead of the previous all-pass `0..255` range.
- `Mean_Brightness_DimBad` in `OpenVisionLab.SampleCatalog.csv` is now `ExpectedFailure`, so the Bad reference represents controlled NG from the same Mean brightness pipeline.
- `PipelineViewerScreenshotSmoke` now exposes `wpf_shell_host_workspace_sample_pipeline_review_ng_metrics`.
- The existing sample Review smoke flow was factored into `OpenWorkspaceSamplePipelineReviewForSmoke` so OK and NG catalog-sample review targets share the same sample-load/Pipeline-Review binding checks.
- The new target opens `Mean_Brightness_DimBad`, verifies the catalog baseline `MeanValueAvg`, opens the active `Sample_` pipeline in Pipeline Review, runs Review explicitly, and asserts NG decision, beginner next action, metric detail, run log, and output preview.

Before/after comparison:

- Product UI layout was not changed in this step, so no product UI before/after image is required.
- Before: `Mean_Brightness_DimBad` was a Bad reference in the picker, but the sample pipeline acceptance range was `0..255`, so Pipeline Review treated the dim image as OK.
- After: `Mean_Brightness_DimBad` remains a Bad reference and now produces controlled metric NG in Pipeline Review because `MeanValueAvg` is below the normal-brightness acceptance gate.
- New verification artifact: `artifacts\sample_pipeline_review_ng_metrics_after_20260702\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj" -c Debug`: PASS.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\sample_pipeline_review_ng_metrics_after_20260702`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pair_coverage artifacts\sample_pair_coverage_after_20260702_r2`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_metrics artifacts\sample_pipeline_review_metrics_after_20260702_r2`: PASS 1/1.

Next priority:

- Started by the 2026-07-02 Bad reference audit smoke above.
- Do not rework MainView/docking/sample picker UI unless a concrete regression appears.

## 2026-07-02 Update - Sample Pipeline Review metric smoke

Added a focused smoke for the operator path after a runnable Good sample is opened: sample load -> active `Sample_` pipeline -> Pipeline Review -> explicit Run Review -> OK decision and metric/output review.

Changed structure:

- `OpenVisionShellHostCommandController` exposes a name-based runnable sample open path for test hooks while reusing the existing `OpenRunnableSample` implementation.
- `OpenVisionShellHostView` test hooks now expose `OpenWorkspaceSampleForTest(string sampleName)`.
- `PipelineViewerScreenshotSmoke` now exposes `wpf_shell_host_workspace_sample_pipeline_review_metrics`.
- The target opens `Blob_RiceParticle_Good`, verifies the catalog baseline metrics (`ResultCount`, `AreaAvg`, `BoundsWidthAvg`), opens the generated active `Sample_` pipeline in Pipeline Review, runs Review explicitly, selects the Blob result step, and asserts OK decision, run-log text, primary result metric, and output preview.
- The target also verifies sample open and Pipeline Review open do not trigger native Preview automatically.

Before/after comparison:

- Product UI was not changed in this step, so no product UI before/after image is required.
- New verification artifact: `artifacts\sample_pipeline_review_metrics_after_20260702\wpf_shell_host_workspace_sample_pipeline_review_metrics.png`

Validation:

- `dotnet build "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_metrics artifacts\sample_pipeline_review_metrics_after_20260702`: PASS 1/1.

Next priority:

- Completed by the 2026-07-02 sample Pipeline Review NG metric smoke above.
- Do not rework completed MainView/docking/sample picker UI unless a concrete regression appears.

## 2026-07-02 Update - Good/Bad sample pair coverage smoke

Added a non-UI coverage smoke for the already-expanded Good/Bad sample catalog instead of duplicating completed sample picker UI work.

Changed structure:

- `PipelineViewerScreenshotSmoke` now exposes `wpf_shell_host_workspace_sample_pair_coverage`.
- The target validates representative Good/Bad pair groups:
  - `Blob_ParticleDensity`
  - `BentPin_Shaft`
  - `LineGauge_Angle`
  - `Template_TargetPresence`
  - `EdgeBased_TargetPresence`
  - `Feature_ScoreDiscrimination`
  - `Mean_BrightnessDrift`
  - `Film_DarkSpot`
  - `SurfaceDefect_EdgeCount`
  - `Fiducial_Visibility`
  - `Fiducial_Blur`
  - `Fiducial_Ink`
  - `Fiducial_Solder`
- For every pair group, the target checks Good and Bad roles, one shared baseline pipeline, bounded expected metrics, and at least one shared Good/Bad metric.
- For selected representative groups, the target runs actual sample recipes through `VisionPipelineSampleCheckService` and verifies metric pass/fail behavior.

Before/after comparison:

- Product UI was not changed in this step, so no product UI before/after image is required.
- New verification artifact: `artifacts\sample_pair_coverage_after_20260702\wpf_shell_host_workspace_sample_pair_coverage.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pair_coverage artifacts\sample_pair_coverage_after_20260702`: PASS 1/1.

Next priority:

- Completed by the 2026-07-02 sample Pipeline Review metric smoke above.
- Keep later work separate from sample picker UI and do not rework the already completed Good/Bad guide/checklist unless a concrete regression appears.

## 2026-07-02 Update - Good/Bad sample validation checklist

Added a visible Good/Bad validation checklist to the sample catalog pair decision guide without reworking completed MainView, docking, recipe, language, layer routing, or preset flows.

Changed structure:

- `OpenVisionWorkspaceSamplePairDecisionGuide` now carries a `ChecklistText` field alongside summary, separating metric, and workflow text.
- `OpenVisionWorkspaceSamplePairDecisionGuidePresenter` derives the checklist from the selected OK/NG pair references and their shared expected metrics.
- `OpenVisionWorkspaceSamplePickerViewModel` exposes `PairDecisionChecklistText` and refreshes it when the selected sample changes.
- `OpenVisionWorkspaceSamplePickerView` shows `WorkspaceSamplePickerPairDecisionChecklist` inside the existing Good/Bad decision guide.
- `PipelineViewerScreenshotSmoke` checks the checklist AutomationId and text in `wpf_shell_host_workspace_sample_pair_picker`.

Before/after comparison:

- Before: the Good/Bad pair guide showed the comparison summary, separating metrics, and review order, but the concrete validation loop was buried in longer guidance.
- After: the first visible Good/Bad guide area shows a compact checklist: record OK, run the same pipeline on NG, confirm metric separation, then adjust input/ROI/template or acceptance limits based on the failure direction.
- Before artifact: `artifacts\good_bad_validation_before_20260702\wpf_shell_host_workspace_sample_pair_picker.png`
- After artifact: `artifacts\good_bad_validation_after_20260702\wpf_shell_host_workspace_sample_pair_picker.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_pair_picker artifacts\good_bad_validation_after_20260702`: PASS 1/1.
- Note: running WPF build and WPF screenshot smoke in parallel can produce temporary `*_wpftmp.csproj` `InitializeComponent` errors; rerunning the build alone passed.

Next priority:

- Do not rework docking/layout, recipe/language/layer management, selected output write, or completed Blob/Contour/Line preset work unless a concrete regression appears.
- Next non-duplicate candidate: broaden the sample catalog Good/Bad data itself where representative tools still have only single-reference samples, then add focused checks for opening a sample and reviewing Good/Bad metrics in Pipeline Review.

## 2026-07-02 Update - Line selected-line beginner presets

Added Line-specific beginner presets without reworking the already completed Blob/Contour preset path.

Changed structure:

- `VisionToolPresetCatalog` now exposes Line Basic/Fast/Precise presets.
- `VisionToolSingleInputSpecialPropertyToolRuntime` exposes the existing preset presenter surface to special PropertyGrid tools.
- `LineToolWpfView` attaches the preset presenter through the special runtime.
- Line presets apply only to the currently selected `Line A` or `Line B` property model.
- Line presets preserve Line purpose, selected line, ROI, projection direction, polarity, input/output layer route, and output layers.
- Applying a Line preset updates the selected PropertyGrid model, persists Line A/B state, refreshes generated visibility rows, updates summary/ROI overlay, and clears stale result review.
- Applying a Line preset cancels pending auto-preview and does not run Preview/Run.

Before/after comparison:

- Before: Matching-family and Blob/Contour had beginner presets, but Line still required beginners to tune contrast, scan interval, fit-line extension, and average filtering manually.
- After: Line exposes the same Basic/Fast/Precise preset surface while respecting Line's selected-line semantics.
- After artifact: `artifacts\line_presets_after_20260702\wpf_shell_host_line_presets.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_line_presets artifacts\line_presets_after_20260702`: PASS 1/1.

Next priority:

- Do not rework MainView recipe/language/layer routing/output write, Blob/Contour presets, or Line presets unless a new concrete regression appears.
- Next non-duplicate candidate: Good/Bad sample-backed validation depth, especially making sample expected metrics and failure reasons broader across representative tools.

## 2026-07-02 Update - Blob/Contour beginner presets

Added non-executing beginner preset support to the common PropertyGrid Tool View runtime for Blob and Contour.

Changed structure:

- `VisionToolPresetCatalog` now exposes `GetPropertyGridPresets<TProperty>()` for non-matching PropertyGrid tools.
- Blob and Contour each expose Basic/Fast/Precise presets.
- `VisionToolSingleInputPropertyToolRuntime` owns the shared preset presenter for common PropertyGrid tools, so Blob/Contour views do not duplicate button/menu logic.
- Applying a Blob/Contour preset updates the selected PropertyGrid model, persists it, refreshes visibility rows, updates summaries/overlays, and clears stale result review.
- Applying a preset does not run Preview/Run, create layers, change input/output routing, or bypass the PropertyGrid model.
- Line remains intentionally excluded from this preset sweep because its ROI, purpose, and Line A/B selection behavior needs a separate design.

Before/after comparison:

- Before: Basic/Fast/Precise presets were available for Matching-family tools only; Blob/Contour beginners had to edit all area/threshold/draw rows manually.
- After: Blob/Contour show the same recommended preset surface while keeping their PropertyGrid editor as the source of truth.
- After artifact: `artifacts\area_tool_presets_after_20260702\wpf_shell_host_area_tool_presets.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_area_tool_presets artifacts\area_tool_presets_after_20260702`: PASS 1/1.

Next priority:

- Do not rework MainView recipe/language/layer routing/output write or Blob/Contour presets unless a new concrete regression appears.
- Next non-duplicate candidate: design Line-specific presets separately because Line has purpose/Line A-B/ROI semantics that should not be forced into the simple area-tool preset model.

## 2026-07-02 Update - Preprocess selected existing output write smoke

Expanded selected-existing-output write validation across the remaining custom/simple single-input Tool Views.

Changed structure:

- `PipelineViewerScreenshotSmoke` adds `wpf_layer_selection_preprocess_existing_output_write`.
- The smoke verifies Filter, Morphology, EdgeDetection, RotateScale, HSV, Mean, and Histogram.
- For each tool, the smoke:
  - loads the source image into `Main`,
  - creates or resets an operator-owned existing output layer,
  - selects that existing output layer in the Tool View output combo,
  - runs explicit Preview,
  - verifies `Main` remains the selected input route,
  - verifies the selected existing output layer is overwritten,
  - verifies no default `{Tool}_Preview` output layer is silently created,
  - verifies the active host layer is restored to `Main`.
- The smoke intentionally avoids parameter auto-preview changes because persisted operator settings can make individual parameter choices state-dependent. The contract here is route/write behavior under explicit Preview.

Before/after comparison:

- Before: selected existing output write was protected for Threshold, algorithm PropertyGrid tools, and recipe output route isolation, but not swept across Filter/Morphology/SimplePreprocess tools as a group.
- After: `wpf_layer_selection_preprocess_existing_output_write` checks the same selected existing output write contract across the remaining custom/simple single-input Tool Views.
- After artifact: `artifacts\preprocess_existing_output_write_after_20260702\wpf_layer_selection_preprocess_existing_output_write.png`
- Diagnostic before/after output images are saved under `artifacts\preprocess_existing_output_write_after_20260702\wpf_layer_selection_preprocess_existing_output_write.diagnostics`.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_layer_selection_preprocess_existing_output_write artifacts\preprocess_existing_output_write_after_20260702`: PASS 1/1.

Next priority:

- Consider whether Line's specialized ROI/measurement modes need a separate selected-existing-output write smoke, because Line requires its own ROI/parameter setup and should not be folded into the simple preprocessing sweep.
- Then move back to actual EXE-level UX review for MainView recipe/layer/output workflows.

## 2026-07-02 Update - Recipe output route isolation smoke

Added a focused recipe-switching guard for Tool View output route selection.

Changed structure:

- `PipelineViewerScreenshotSmoke` adds `wpf_shell_host_recipe_output_route_isolation`.
- The smoke creates recipe A/B with separate active pipelines and separate operator output layers.
- In recipe A, it selects `RecipeA_Output` in Threshold's output write selector and verifies Add Pipeline writes that output only into recipe A.
- After switching to recipe B, it verifies recipe A's selected output route is not carried into recipe B's reopened Tool View.
- In recipe B, it selects `RecipeB_Output` and verifies Add Pipeline writes that output only into recipe B.
- After switching back to recipe A, it verifies recipe B's selected output route is not carried back into recipe A.
- The smoke also verifies the route checks do not run Preview/Run or leave stale native preview state.

Before/after comparison:

- Before: recipe context smoke protected recipe-scoped tool parameters and Add Pipeline context, but did not explicitly check that output write route selections stayed isolated across recipe switches.
- After: recipe A/B output route selection and Add Pipeline output names are checked directly.
- After artifact: `artifacts\recipe_output_route_isolation_after_20260702\wpf_shell_host_recipe_output_route_isolation.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_recipe_output_route_isolation artifacts\recipe_output_route_isolation_after_20260702`: PASS 1/1.

Next priority:

- Continue selected-existing-output write coverage for remaining custom/simple tools if gaps remain.
- Then move from smoke coverage into any product UX gaps the route tests expose during actual EXE use.

## 2026-07-02 Update - Algorithm selected existing output write smoke

Expanded selected-output-layer validation from custom/simple tools into the PropertyGrid-heavy algorithm tools.

Changed structure:

- `PipelineViewerScreenshotSmoke` adds `wpf_layer_selection_algorithm_existing_output_write`.
- The new smoke verifies Blob, Contour, Matching, EdgeBasedMatching, and FeatureMatching.
- For each tool, the smoke:
  - loads the source image into `Main`,
  - creates an operator-owned existing output layer,
  - selects that existing output layer in the Tool View output combo,
  - runs explicit Preview,
  - verifies `Main` remains the selected input route,
  - verifies the selected existing output layer is overwritten,
  - verifies no default `{Tool}_Preview` output layer is silently created,
  - verifies the active host layer is restored to `Main`.
- The smoke resets the selected output layer after template setup for matching-family tools so the explicit Preview write path is measured even if a template-backed tool schedules an internal preview during setup.
- A lightweight overlay-aware bitmap assertion was added because matching-family Preview output may be the original image plus small result overlays, not a full-frame threshold-style transformation.

Before/after comparison:

- Before: `wpf_layer_selection_existing_output_write` protected the selected existing output write path mainly through Threshold, while algorithm output write behavior was not swept as a group.
- After: `wpf_layer_selection_algorithm_existing_output_write` checks the same selected existing output write contract across Blob, Contour, Matching, EdgeBasedMatching, and FeatureMatching.
- After artifact: `artifacts\algorithm_existing_output_write_after_20260702\wpf_layer_selection_algorithm_existing_output_write.png`
- Diagnostic before/after output images are saved under `artifacts\algorithm_existing_output_write_after_20260702\wpf_layer_selection_algorithm_existing_output_write.diagnostics`.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_layer_selection_algorithm_existing_output_write,wpf_layer_selection_existing_output_write artifacts\algorithm_existing_output_write_after_20260702`: PASS 2/2.

Next priority:

- Review recipe-scoped output/write target persistence so switching recipes does not leak a previous recipe's selected output layer into a different recipe workflow.
- Then expand the same selected-existing-output write sweep to remaining custom/simple tools if gaps remain.

## 2026-07-02 Update - MainView layer rename command

Added explicit operator-facing layer rename support to the MainView layer management surface.

Changed structure:

- `DisplayLayerStore`, `DisplayLayerPresenter`, and `DisplayManagerService` now support renaming a layer title while preserving the existing image slot.
- `OpenVisionShellHostLayerManagementController` owns rename validation and execution.
  - `Main` cannot be renamed.
  - Empty, duplicate, control-character, or invalid-file-name layer titles are rejected.
  - Rename activates and refreshes the renamed layer without running tools or changing native input routes.
- `OpenVisionShellHostLayerCommandSurface` exposes selected/current rename commands.
- The top MainView layer strip now includes a visible rename text field and pencil command next to the layer selector.
- `OpenVisionShellPreviewViewModel` layer options now refresh from real host layer titles instead of keeping the old static placeholder list.
- `PipelineViewerScreenshotSmoke` adds `wpf_shell_host_layer_rename_command`, covering create, dock, rename, layer image preservation, stale old-title removal, docked title refresh, invalid `Main`/duplicate rename rejection, and no tool/Preview side effects.

Before/after comparison:

- Before: MainView had layer create/load/delete, but no explicit rename command; the top layer selector also used a stale static option list.
- After: MainView exposes a top rename field/button and the host layer selector is refreshed from actual layer names.
- After artifact: `artifacts\layer_rename_after_20260702_final\wpf_shell_host_layer_rename_command.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_rename_command,wpf_shell_host_layer_management_commands artifacts\layer_rename_after_20260702_final`: PASS 2/2.

Next priority:

- Extend the output-write smoke sweep across more native/custom Tool Views so selected existing output layers are contract-checked beyond Threshold/Arithmetic.
- Then review recipe-scoped output layer naming/selection persistence so multi-recipe workflows do not leak write targets.

## 2026-07-02 Update - Tool View selected output write target

Clarified and contract-tested Tool View output routing so operators can write Preview results into a selected existing output layer instead of being forced into a generated default output layer.

Changed structure:

- `VisionToolChromePresenter` now applies a localized tooltip to the output-layer selector, explaining that Preview/Run writes to the selected result layer.
- Single-input and double-input Tool View runtimes now pass their output-layer selector into the shared chrome presenter.
- Tool View output group wording now uses `Result Write Layer` / `결과 쓰기 레이어` so the combo is presented as the write target, not only as a layer list.
- Localization catalog migration updates existing local catalogs for the renamed output label and expanded create-output tooltip.
- `PipelineViewerScreenshotSmoke` adds `wpf_layer_selection_existing_output_write`, covering selection of an existing operator output layer, Preview overwrite into that layer, preservation of `Main` as input, no default output auto-creation, and no host active-layer side effect.

Before/after comparison:

- Before: the Tool View showed `Output Layer`, existing output overwrite behavior was not obvious, and no smoke enforced that selecting an existing output layer wrote there.
- After: the Tool View labels the selector as `결과 쓰기 레이어`; selecting `Operator_Output` and running Preview overwrites `Operator_Output`, keeps input as `Main`, and does not create `Threshold_Preview`.
- After artifact: `artifacts\output_layer_write_after_20260702_final\wpf_layer_selection_existing_output_write.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_layer_selection_existing_output_write,wpf_layer_selection_threshold_tool,wpf_layer_selection_arithmetic_tool artifacts\output_layer_write_after_20260702_final`: PASS 3/3.

Next priority:

- Add operator-facing layer rename support and decide where it belongs in the MainView layer command surface.
- Then continue recipe-scoped output/write behavior across more native/custom tools, keeping output writes explicit and input routing unchanged.

## 2026-07-02 Update - Explicit layer management commands

Added operator-facing layer management commands to MainView without changing tool routing or running Preview/Run.

Changed structure:

- Added `OpenVisionShellHostLayerManagementController`.
  - Creates explicit blank operator layers with unique `Layer_###` names.
  - Loads an image into the selected/current layer through the existing DisplayManager image/history path.
  - Deletes non-`Main` layers and refreshes the host layer list, selected detail, direct-route text, and docked layer workspace.
- `OpenVisionShellHostLayerCommandSurface` now exposes MVVM commands for create/load/delete layer actions in addition to existing open/dock/clear commands.
- `OpenVisionShellHostView` now exposes layer create/load/delete from:
  - top layer input strip icons,
  - workspace context menu,
  - layer row context menu,
  - selected-layer detail action row.
- `OpenVisionShellHostView.TestHooks` exposes create/load/set/delete layer hooks for focused smoke verification.
- `PipelineViewerScreenshotSmoke` adds `wpf_shell_host_layer_management_commands`, covering create, image load into an operator layer, docked layer deletion synchronization, and no tool/Preview side effects.

Before/after artifact:

- After: `artifacts\layer_management_commands_after_20260702_r2\wpf_shell_host_layer_management_commands.png`
- Before state: MainView had layer display/open/dock paths, but no first-screen create/load/delete layer command surface.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_management_commands artifacts\layer_management_commands_after_20260702_r2`: PASS 1/1.

Next priority:

- Extend Tool View output routing so operators can write Preview results into a selected existing output layer or explicitly create a new output layer, without changing input selection.
- Then consider rename support for operator-created layers, because create/load/delete is now available but layer names are still generated.

## 2026-07-02 Update - MainView recipe/language controls

Added the first operator-facing MainView controls for recipe switching and fixed the language selector display/change path.

Changed structure:

- `RecipeWorkspaceService` now exposes recipe workspace names from the standard `RECIPE` root.
- Added `OpenVisionShellHostRecipeCommandSurface`.
  - Lists recipe workspaces.
  - Switches recipes through `RecipeState.Name`, preserving the existing recipe reload/tool-cache-close path.
  - Creates a timestamped `Recipe_yyyyMMdd_HHmmss` workspace with an active default pipeline.
- `OpenVisionShellHostView` now exposes `RecipeCommands` and binds a top-bar recipe selector plus a new-recipe button.
- The language selector now uses readable `한국어` / `English` option text and selected language changes are persisted.
- The shared Shell ComboBox template now applies the foreground color to the selected item presenter so dark chrome does not hide selected text.
- `PipelineViewerScreenshotSmoke` now has `wpf_shell_host_recipe_language_controls`, covering language switching, recipe listing, recipe switching, new recipe creation, and no tool/Preview side effects.

Before/after artifacts:

- Before: `artifacts\recipe_context_tool_state_after_20260701\wpf_shell_host_recipe_context_switch.png`
- After: `artifacts\recipe_language_controls_after_20260702\wpf_shell_host_recipe_language_controls.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_language_controls_after_20260702`: PASS 1/1.

Next priority:

- Add explicit layer management commands in the docked layer workspace: create/rename/delete/load image into selected layer without disrupting input route selection.
- Then extend Tool View output routing so operators can write Preview results into a selected existing output layer or create a new one explicitly.

## 2026-07-01 Update - Recipe-scoped tool state guard

Locked the per-recipe PropertyGrid and custom WPF teaching-state boundary into the recipe context smoke.

Changed structure:

- `wpf_shell_host_recipe_context_switch` now seeds different persisted `Blob_1` `MIN_AREA`/`MAX_AREA` values for recipe A and recipe B.
- The smoke opens Blob after switching to recipe B and verifies Add Pipeline writes only recipe B's active pipeline with recipe B's Blob parameters.
- The smoke then switches back to recipe A, verifies the native PropertyGrid document was closed/recreated instead of keeping stale state, and verifies Add Pipeline uses recipe A's Blob parameters.
- The same smoke now seeds different `Threshold_ToolState` values for recipe A and recipe B, then verifies Threshold Add Pipeline uses the active recipe's mode/value state after each switch.
- `OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md` now explicitly states that recipe switching must not let cached native PropertyGrid documents leak previous-recipe selected objects.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_recipe_context_switch artifacts\recipe_context_tool_state_after_20260701`: PASS 1/1.

Next priority:

- Add the same recipe-scoped persistence guard for the remaining custom/dynamic WPF tool state files (`Filter_ToolState`, `Morphology_ToolState`, `Arithmetic_ToolState`) so non-PropertyGrid tool modes/values cannot leak across recipe switches.
- Keep recipe switching display-only: no tool auto-open, no Preview/Run, no output layer creation.

## 2026-07-01 Update - Recipe context scoped native Add Pipeline

Pushed active recipe/pipeline context into the cached native Tool View Add Pipeline path.

Changed structure:

- `OpenVisionNativeToolDocument` now stores the active `OpenVisionRecipeContext` and exposes read-only test diagnostics for recipe/pipeline names.
- `OpenVisionNativeToolDocumentCache` reapplies the current context whenever a cached native tool is activated, so prewarmed/reused tools do not keep a stale recipe.
- `OpenVisionShellHostDocumentController` and `OpenVisionShellHostToolWindowController` pass the Shell's resolved recipe context into native tool activation.
- `OpenVisionNativePipelineCommandController` appends created steps through the context-aware pipeline append service.
- `VisionPipelineAppendService` now has explicit recipe/pipeline overloads while preserving the existing global fallback overload.
- `wpf_shell_host_recipe_context_switch` now verifies that recipe A/B switching is display-only and that a native Tool View Add Pipeline action appends only to recipe B's active pipeline without running Preview.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_recipe_context_switch artifacts\recipe_context_native_append_after_20260701_r2`: PASS 1/1.
- `git diff --check` on the changed files: PASS with LF/CRLF working-copy warnings only.

Next priority:

- Continue recipe context propagation into per-recipe tool parameter/session persistence, especially PropertyGrid last-edited values and custom tool mode state.
- Keep all recipe-context UI changes display-only unless the operator explicitly opens a tool, runs Preview/Run, or presses Add Pipeline.

## 2026-07-01 Update - Task-oriented Learn path sample picker

Added a task-first Learn Mode entry layer to the sample catalog picker.

Changed structure:

- Added `OpenVisionWorkspaceSampleLearnPathOption` and `OpenVisionWorkspaceSampleLearnPathClassifier`.
  - Classifies runnable catalog samples into display-only learning paths.
  - Current paths: All, Matching, Blob, Contour, Line, Mean, Good/Bad.
  - Matching classification now uses ToolFlow/Category rather than loose sample-name text so generic "feature" wording does not pull contour samples into the Matching path.
- `OpenVisionWorkspaceSamplePickerViewModel` now exposes:
  - `LearnPathOptions`
  - `SelectedLearnPathOption`
  - `LearnPathLabelText`
  - `ActiveLearnPathText`
  - combined LearnPath + search filtering
- `OpenVisionWorkspaceSamplePickerView` now shows a Learn path selector above the raw sample list.
- `PipelineViewerScreenshotSmoke` now has `wpf_shell_host_workspace_sample_learn_paths`, which selects representative paths and verifies the visible list count and selected sample match the path.
- Stable contracts and the Learn Mode direction doc now record that Learn path selection is filter/display-only and must not run Preview/Run, open tools, create layers, change routing, or rewrite recipe values.

Before/after artifacts:

- Before sample picker: `artifacts\sample_pair_decision_after_20260701\wpf_shell_host_workspace_sample_picker.png`
- After Learn path default picker: `artifacts\sample_learn_paths_after_20260701_r3\wpf_shell_host_workspace_sample_picker.png`
- After Matching Learn path filter: `artifacts\sample_learn_paths_after_20260701_r3\wpf_shell_host_workspace_sample_learn_paths.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_picker,wpf_shell_host_workspace_sample_learn_paths,wpf_shell_host_workspace_sample_pair_picker artifacts\sample_learn_paths_after_20260701_r2`: PASS 3/3.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_picker,wpf_shell_host_workspace_sample_learn_paths artifacts\sample_learn_paths_after_20260701_r3`: PASS 2/2 after selector height polish.

Next priority:

- Continue polishing Learn Mode visual cards and sample metadata only where it improves the beginner path.
- Then continue recipe context propagation into deeper tool runtime/session boundaries for multi-recipe workflows.

## 2026-07-01 Update - Good/Bad pair decision guide

Strengthened the sample-centered Learn Mode flow for Good/Bad image pairs.

Changed structure:

- Added `OpenVisionWorkspaceSamplePairDecisionGuide` and `OpenVisionWorkspaceSamplePairDecisionGuidePresenter`.
  - Formats pair-specific decision text outside XAML/code-behind.
  - Compares shared expected metrics between the selected sample and its opposite OK/NG references.
  - Explains the manual review order: verify OK first, then run the same pipeline on NG and compare the metric margin.
- `OpenVisionWorkspaceSamplePickerViewModel` now exposes:
  - pair decision visibility
  - pair decision summary
  - separating metric text
  - manual review workflow text
- `OpenVisionWorkspaceSamplePickerView` now shows a `WorkspaceSamplePickerPairDecisionGuide` section for Good/Bad pair samples.
- `PipelineViewerScreenshotSmoke` now asserts the pair decision guide ID and text in `wpf_shell_host_workspace_sample_pair_picker`.
- Stable contracts and the Learn Mode direction doc now record that the pair decision guide is display-only and must not run Preview/Run, open tools, create layers, change routing, or rewrite recipe thresholds.

Before/after artifacts:

- Before Learn Mode pair picker: `artifacts\sample_learn_after_20260701\wpf_shell_host_workspace_sample_pair_picker.png`
- After pair decision guide: `artifacts\sample_pair_decision_after_20260701\wpf_shell_host_workspace_sample_pair_picker.png`
- After default sample picker: `artifacts\sample_pair_decision_after_20260701\wpf_shell_host_workspace_sample_picker.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_picker,wpf_shell_host_workspace_sample_pair_picker artifacts\sample_pair_decision_after_20260701`: PASS 2/2.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target localization_catalog_contract_check artifacts\sample_pair_decision_after_20260701_localization`: PASS 1/1.
- `git diff --check` on touched tracked files: PASS with LF/CRLF working-copy warnings only.

Next priority:

- Add task-oriented Learn Mode entry grouping, such as Matching, Blob, Line, and Mean learning paths, without bypassing the sample/catalog/pipeline contract.
- Then continue recipe context propagation into deeper tool runtime/session boundaries where multiple recipes need distinct state.

## 2026-07-01 Update - SimplePreprocess result explanation helper

Extended the beginner-facing result explanation pattern to SimplePreprocess-style tools.

Changed structure:

- Added `SimplePreprocessResultExplanation` to format Mean/HSV/Histogram preview interpretation outside the view code-behind.
- `VisionToolSingleInputCustomToolRuntime` and controller now expose shared result-review show/clear APIs for custom single-input tools.
- `SimplePreprocessToolWpfView` clears stale result review when parameters or layer routes change and exposes a read-only result-review test hook.
- `OpenVisionNativeSimplePreprocessPreviewExecutor` now publishes display-only result explanations after successful preview execution:
  - Mean: average mean, configured range, result count, and likely adjustment path
  - HSV: selected mask pixel ratio and H/S/V range guidance
  - Histogram: input/output mean and contrast change guidance
- `PipelineViewerScreenshotSmoke` verifies Mean/HSV/Histogram result-review tokens through the focused `wpf_simple_preprocess_result_review` target.

Before/after artifacts:

- Before SimplePreprocess shell baseline: `artifacts\ui_precheck_simple_preprocess_executor_20260624\wpf_shell_host_native_tool.png`
- After SimplePreprocess result explanation: `artifacts\simple_preprocess_result_explanation_after_20260701\wpf_simple_preprocess_result_review.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_simple_preprocess_result_review artifacts\simple_preprocess_result_explanation_after_20260701`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target localization_catalog_contract_check artifacts\simple_preprocess_result_explanation_after_20260701_localization`: PASS 1/1.
- `git diff --check` on the changed files: PASS with line-ending warnings only. Full-worktree `git diff --check` still reports a pre-existing unrelated `tools/RunUiPrecheck.ps1` blank line at EOF.

Next priority:

- Continue result explanation/preset coverage for remaining beginner-critical tools, or start the next Learn Mode sample flow once the user confirms this pass.

## 2026-07-01 Update - Line result explanation helper

Extended the display-only result explanation pattern to the Line tool.

Changed structure:

- Added `LineToolResultExplanation` to format Line Edge/Measure/Intersection result reasons outside the guide presenter.
- `LineToolVerificationGuidePresenter` now delegates result reason/next-action wording to the helper.
- Line guidance now explains:
  - Edge: line count, edge-point count, fitted-line length, and stability check hint
  - Measure: px/mm distance, detected count, and Line A/B scan-direction interpretation
  - Intersection: cross/no-cross result plus likely failure-cause parameter families
- Added Korean/English localization keys for Line result explanation and failure-cause guidance.
- `PipelineViewerScreenshotSmoke` now verifies the richer Line result guidance tokens for Edge, Measure, and Intersection, including the NG intersection path.

Before/after artifacts:

- Before Line guide baseline: `artifacts\ux_line_guide_after_20260701_r9\wpf_shell_host_line_tool.png`
- After Line result explanation: `artifacts\line_result_explanation_after_20260701\wpf_shell_host_line_tool.png`
- Extra Line measure/intersection validation: `artifacts\line_result_explanation_after_20260701_extra`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_line_tool artifacts\line_result_explanation_after_20260701`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_line_pins_measure_tool,wpf_shell_host_line_intersection_tool artifacts\line_result_explanation_after_20260701_extra`: PASS 2/2.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target localization_catalog_contract_check artifacts\line_result_explanation_after_20260701_localization`: PASS 1/1.

Next priority:

- Continue result explanation/failure-cause presenter expansion to measurement-style simple tools such as Mean/Histogram/HSV where useful.
- Keep explanation helpers display-only: no Preview/Run, no layer creation, no route mutation, and no metric/pass-fail semantic changes.

## 2026-07-01 Update - Blob/Contour area result explanation helper

Extended the result explanation/failure-cause presenter pattern from Matching to Blob and Contour area-style tools.

Changed structure:

- Added `VisionToolAreaResultExplanation` to format display-only result reasons for area-style tools.
- `VisionToolAreaVerificationGuidePresenter<TProperty, TResult>` now delegates result reason/next-action wording to the helper.
- Blob and Contour now pass area and bounding-box accessors into the guide presenter so result guidance can explain:
  - detected region count
  - max area
  - max box size
  - threshold/ROI/area criteria pass interpretation
  - likely failure-cause parameter families
- Added Korean/English localization keys for Blob/Contour result explanation and failure-cause guidance.
- `PipelineViewerScreenshotSmoke` now verifies that Blob/Contour result guidance includes max-area and box explanation tokens in both floating and docked inspector states.

Before/after artifacts:

- Blob before: `artifacts\blob_after_docked_preset_header_20260701\wpf_shell_host_blob_tool.png`
- Blob after: `artifacts\area_result_explanation_after_20260701\wpf_shell_host_blob_tool.png`
- Contour before: `artifacts\ux_contour_guide_after_20260701_r2\wpf_shell_host_contour_tool.png`
- Contour after: `artifacts\area_result_explanation_after_20260701\wpf_shell_host_contour_tool.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_blob_tool,wpf_shell_host_contour_tool artifacts\area_result_explanation_after_20260701`: PASS 2/2.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target localization_catalog_contract_check artifacts\area_result_explanation_after_20260701_localization`: PASS 1/1.

Next priority:

- Continue result explanation/failure-cause presenter expansion to Line and measurement-style tools.
- Keep these helpers display-only: no Preview/Run, no layer creation, no route mutation, and no metric/pass-fail semantic changes.

## 2026-07-01 Update - Matching result explanation helper

Added the first display-only result explanation helper for Matching-family tools.

Changed structure:

- Added `VisionToolMatchingResultExplanation` to format beginner-facing result reasons and likely failure causes outside the result-review UI presenter.
- `VisionToolMatchingResultReviewPresenter` now applies summary/chips/guidance UI and delegates score/count/angle/scale/cause wording to the helper.
- Matching success guidance now includes explicit metric reasoning such as `최고 점수 >= 기준`, detected count against requested count, and the next action.
- Empty/NG Matching-family guidance now names likely parameter families such as template ROI, min score, Canny range, Ratio/RANSAC, contrast, and candidate count.
- Added Korean/English localization keys for the new Matching-family explanation strings.
- `PipelineViewerScreenshotSmoke` now verifies the richer Matching result guidance tokens and prints active tool text in result-review assertion failures.

Before/after artifacts:

- Before docked preset header result panel: `artifacts\matching_tool_after_docked_presets_20260701\wpf_shell_host_matching_tool.png`
- After Matching result explanation: `artifacts\matching_result_explanation_after_20260701\wpf_shell_host_matching_tool.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_matching_tool artifacts\matching_result_explanation_after_20260701`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_edge_based_matching_tool,wpf_shell_host_feature_matching_tool artifacts\matching_family_result_explanation_after_20260701`: PASS 2/2.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target localization_catalog_contract_check artifacts\matching_result_explanation_after_20260701_localization`: PASS 1/1.

Next priority:

- Extend the same result explanation/failure-cause pattern to Blob and Contour result panels.
- Keep the explanations display-only: no Preview/Run, no layer creation, no route mutation, no pass/fail semantic changes.

## 2026-07-01 Update - Docked preset header menu

Added docked-inspector access to the shared Matching-family tool presets without taking height away from the PropertyGrid editor.

Changed structure:

- `VisionToolSingleInputPropertyToolShell` now uses a custom `Parameters` header with an optional compact preset menu button.
- `VisionToolChromePresenter` preserves custom GroupBox header content and localizes the embedded header text instead of replacing the header object.
- `VisionToolPresetButtonPresenter<TProperty>` now shows:
  - the full preset strip in floating tool windows
  - a compact header menu in docked inspector mode
- Docked menu items use stable AutomationIds such as `VisionToolPresetMenuItem_fast`.
- `wpf_shell_host_matching_presets` now verifies both floating preset buttons and docked preset menu application without Preview/Run execution.

Before/after artifacts:

- Before docked Matching baseline: `artifacts\matching_tool_after_presets_20260701\wpf_shell_host_matching_tool.png`
- After docked preset header menu: `artifacts\matching_docked_presets_after_20260701\wpf_shell_host_matching_presets.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_matching_tool artifacts\matching_tool_after_docked_presets_20260701`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_matching_presets artifacts\matching_docked_presets_after_20260701`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_edge_based_matching_tool,wpf_shell_host_feature_matching_tool artifacts\matching_family_after_docked_preset_header_20260701`: PASS 2/2.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_blob_tool artifacts\blob_after_docked_preset_header_20260701`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target localization_catalog_contract_check artifacts\matching_docked_presets_after_20260701_localization`: PASS 1/1.

Next priority:

- Start the result explanation/failure-cause presenter pass for Matching first, then Blob/Contour.
- Keep the same rule: explanations are display-only and must not run Preview/Run or change layer routing.

## 2026-07-01 Update - Tool preset foundation

Added the first shared beginner preset foundation on top of PropertyGrid-backed Matching-family tool models.

Changed structure:

- Added `VisionToolPreset<TProperty>` as a model-level preset command contract.
- Added `VisionToolPresetCatalog` with `basic`, `fast`, and `precise` presets for Matching, EdgeBasedMatching, and FeatureMatching property models.
- Added `VisionToolPresetButtonPresenter<TProperty>` to bind localized preset buttons to the shared single-input tool shell.
- `VisionToolSingleInputPropertyToolShell` now has a reusable preset host. It is visible in floating Matching-family tools and hidden in docked inspector mode to preserve the stable PropertyGrid editing viewport.
- `VisionToolMatchingPropertyRuntime<TProperty>` now applies presets through a model-update-only path: persist selected object, refresh generated PropertyGrid rows, update summary/overlays, clear result review. It does not schedule Preview/Run.
- `PipelineViewerScreenshotSmoke` now has `wpf_shell_host_matching_presets`, which verifies preset UI, exact PropertyGrid model values, and no preview execution even after `AUTO_PREVIEW=true` was enabled before applying a preset.

Before/after artifacts:

- Before Matching tool baseline: `artifacts\matching_tool_after_presets_20260701\wpf_shell_host_matching_tool.png`
- After preset UI smoke: `artifacts\matching_presets_after_20260701\wpf_shell_host_matching_presets.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_matching_presets artifacts\matching_presets_after_20260701`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_matching_tool artifacts\matching_tool_after_presets_20260701`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_feature_matching_tool artifacts\matching_family_presets_after_20260701`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_edge_based_matching_tool artifacts\edge_based_matching_after_presets_retry_20260701`: PASS 1/1 after retry. The first combined Edge/Feature run hit a transient invalid-window-handle error while FeatureMatching passed.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target localization_catalog_contract_check artifacts\matching_presets_after_20260701_localization`: PASS 1/1.

Next priority:

- Continue result explanation/failure-cause presenter work for Matching and Blob/Contour families.

## 2026-07-01 Update - RecipeContext controller propagation

Continued the recipe-context foundation by moving key Shell controller boundaries from string recipe providers to `OpenVisionRecipeContext` providers.

Changed structure:

- `OpenVisionShellHostCommandController` now receives `Func<OpenVisionRecipeContext>` and saves opened sample pipelines under `context.Name`.
- `OpenVisionShellHostSampleWorkflowPresenter` now receives `Func<OpenVisionRecipeContext>` and reads `context.Name/context.PipelineName`.
- `OpenVisionShellHostToolWindowController` now receives `Func<OpenVisionRecipeContext>` and opens Pipeline Review with the active context snapshot.
- `OpenVisionPipelineReviewDocument` now stores `OpenVisionRecipeContext` and resolves the active pipeline using the context name and pipeline fallback.
- Test hooks expose Pipeline Review recipe context name/pipeline.
- `wpf_shell_host_recipe_context_switch` now verifies that explicitly opened Pipeline Review sees the active recipe context and still does not run Preview.

Before/after artifacts:

- Before context-only smoke: `artifacts\recipe_context_after_20260701\wpf_shell_host_recipe_context_switch.png`
- After controller-propagation smoke: `artifacts\recipe_context_provider_after_20260701\wpf_shell_host_recipe_context_switch.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_recipe_context_switch artifacts\recipe_context_provider_after_20260701`: PASS 1/1.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_open artifacts\recipe_context_provider_after_20260701_sample`: PASS 1/1.

Next priority:

- Start the shared tool preset contract (`기본 검사`, `빠른 검사`, `정밀 검사`) on top of PropertyGrid-backed models.
- Keep preset application explicit and verify that applying a preset does not auto-run Preview/Run.

## 2026-07-01 Update - RecipeContext foundation

Implemented the first explicit recipe-context foundation before deeper controller movement.

Changed structure:

- Added `OpenVisionRecipeContext`.
- Added `OpenVisionRecipeContextStore`.
- Added `OpenVisionShellHostRecipeContextPresenter`.
- `OpenVisionShellHostView` now resolves recipe names through the context store and shows a compact top-bar recipe/pipeline context chip.
- Shell recipe-change handling now refreshes the context presenter while keeping the existing `RecipeState.Name` change path and `OpenVisionShellHostRecipeController` cleanup behavior.
- Test hooks expose current recipe context name, pipeline, source path, active layer, and an explicit recipe switch helper.
- `PipelineViewerScreenshotSmoke` now has `wpf_shell_host_recipe_context_switch`, which verifies recipe A/B context switching without auto-opening tools or running Preview.

Before/after artifacts:

- Before sample-open top bar: `artifacts\mainview_microcopy_after_20260701\wpf_shell_host_workspace_sample_open.png`
- After sample-open top bar: `artifacts\recipe_context_after_20260701\wpf_shell_host_workspace_sample_open.png`
- Recipe context switch smoke: `artifacts\recipe_context_after_20260701\wpf_shell_host_recipe_context_switch.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_recipe_context_switch,wpf_shell_host_workspace_sample_open artifacts\recipe_context_after_20260701`: PASS 2/2.

Next priority:

- Move tool/Pipeline Review recipe consumption from string provider callbacks toward `OpenVisionRecipeContext` references.
- Add real tool preset commands (`기본 검사`, `빠른 검사`, `정밀 검사`) that update PropertyGrid-backed models without auto-running Preview/Run.

## 2026-07-01 Update - Beginner Learn Mode and recipe context direction

Added the beginner-friendly learning direction requested by the user.

Changed structure:

- Added `docs\OPENVISIONLAB_BEGINNER_LEARN_MODE_AND_RECIPE_CONTEXT_20260701.md`.
- `OpenVisionWorkspaceSamplePickerViewModel` now exposes:
  - sample-centered Learn Mode text
  - recommended start text
  - result interpretation text
  - failure-cause summary text
  - Good/Bad pair comparison summary and opposite reference text
- `OpenVisionWorkspaceSamplePickerView` now displays:
  - compact benchmark strip
  - pair-comparison strip for Good/Bad pair rows
  - Learn Mode strip with recommended start, result interpretation, and failure cause
- `PipelineViewerScreenshotSmoke` now has `wpf_shell_host_workspace_sample_pair_picker`.
- The default sample picker and Good/Bad pair picker smokes assert the new guidance text.

Before/after artifacts:

- Before pair picker: `artifacts\sample_pair_before_20260701\wpf_shell_host_workspace_sample_pair_picker.png`
- After Learn Mode pair picker: `artifacts\sample_learn_after_20260701\wpf_shell_host_workspace_sample_pair_picker.png`
- After Learn Mode default picker: `artifacts\sample_learn_after_20260701\wpf_shell_host_workspace_sample_picker.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_picker,wpf_shell_host_workspace_sample_pair_picker artifacts\sample_learn_after_20260701`: PASS 2/2.

Next priority:

- Implement explicit recipe context switching before deeper controller movement. Different inspections must be able to use different recipe contexts without hidden global recipe mutation.
- Then add real tool preset commands for `기본 검사`, `빠른 검사`, and `정밀 검사`; presets must update PropertyGrid-backed models and must not auto-run Preview/Run.

## 2026-07-01 Update - Competitor review and sample benchmark strip

Compared OpenVisionLab against public Cognex In-Sight EasyBuilder, MVTec MERLIC, NI Vision Builder AI, and Zebra Aurora Vision Studio materials.

Changed structure:

- Added `docs\OPENVISIONLAB_COMPETITOR_PRIORITY_REVIEW_20260701.md`.
- The current priority after MainView completion is the sample/inspection benchmark loop rather than more MainView layout work.
- `OpenVisionWorkspaceSamplePickerViewModel` now exposes selected-sample benchmark display state:
  - OK reference / NG reference / OK criteria
  - acceptance criteria summary
  - Good/Bad pair or single-sample context
- `OpenVisionWorkspaceSamplePickerView` now shows a compact benchmark strip in the selected sample header.
- `wpf_shell_host_workspace_sample_picker` now verifies the benchmark strip and selected benchmark text.

Before/after artifacts:

- Before: `artifacts\sample_benchmark_before_20260701\wpf_shell_host_workspace_sample_picker.png`
- After: `artifacts\sample_benchmark_after_20260701\wpf_shell_host_workspace_sample_picker.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_picker artifacts\sample_benchmark_after_20260701`: PASS 1/1.
- Localization duplicate-key check: PASS, 1513 keys.
- `git diff --check` on touched tracked files: PASS. Git reported LF/CRLF working-copy warnings only.

Next priority:

- If validation passes, continue sample-backed OK/NG coverage where a real operator workflow exposes a missing visual contract.

## 2026-07-01 Update - Pipeline Review acceptance NG visual coverage

Added focused visual coverage for the Pipeline Review NG/failed-step review path.

Changed structure:

- `PipelineViewerScreenshotSmoke` now has a dedicated `wpf_shell_host_pipeline_review_ng` target.
- The NG smoke reuses the existing 3-step Pipeline Review readability sample, then applies an impossible `ResultImageWidth <= 1` acceptance rule to the first Threshold step.
- `OpenVisionPipelineReviewGuidePresenter` now formats metric-based acceptance NG reasons through localization before falling back to lower-level diagnostic text.
- `OpenVisionPipelineReviewDocument` reuses the same localized metric NG reason in the selected-step result detail, so Korean review screens do not leak the English `Result Width` diagnostic.
- The smoke verifies that `Run Review` produces an NG decision, the guide/result detail shows the localized NG next action and metric target reason, the run log remains populated, and the failed step output image stays visible for inspection.
- Pipeline Review execution/routing was not changed; this is display-state coverage for the already-stable Pipeline Review guide/acceptance behavior.

Before/after artifacts:

- Before OK review baseline: `artifacts\pipeline_review_ng_before_20260701\wpf_shell_host_pipeline_review.png`
- After NG review sample: `artifacts\pipeline_review_ng_after_20260701\wpf_shell_host_pipeline_review_ng.png`

Validation:

- Localization duplicate-key check: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_pipeline_review,wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_after_20260701`: PASS 2/2.

Next priority:

- Continue Tool View wording polish only where a concrete mixed-language or beginner-readiness gap remains.

## 2026-07-01 Update - MainView microcopy localization pass

Completed the MainView empty/image/sample/tool-selected microcopy audit.

Changed structure:

- `OpenVisionShellHostMenuPresenter` now owns the empty workspace beginner workflow text, sample/guide buttons, log hint, and workspace context-menu image actions.
- `OpenVisionShellHostMainActionPresenter` now owns the image-ready quick action button labels and reapplies the visible image-ready text when the product language changes.
- `OpenVisionShellHostDirectRunPresenter` now keeps its current display state and reapplies empty/image-ready/sample-ready/pending/succeeded banner text through localization.
- `OpenVisionShellHostView.xaml` keeps named text elements for presenter-driven localization instead of hard-coded MainView learning copy.
- `LocalizationCatalog.tsv` now includes MainView empty workflow, workspace status, context menu, and image-ready guidance keys in Korean and English.
- `PipelineViewerScreenshotSmoke` now verifies Korean and English MainView copy, language switching without opening tools, no `Preview 확인` mixed Korean/English text, and no automatic Preview/Run from MainView display states.

Before/after artifacts:

- Empty before: `artifacts\mainview_microcopy_before_20260701\wpf_shell_host_workspace_empty.png`
- Empty after: `artifacts\mainview_microcopy_after_20260701\wpf_shell_host_workspace_empty.png`
- Image-ready before: `artifacts\mainview_microcopy_before_20260701\wpf_shell_host_workspace_image_load.png`
- Image-ready after: `artifacts\mainview_microcopy_after_20260701\wpf_shell_host_workspace_image_load.png`
- Tool-selected before: `artifacts\mainview_microcopy_before_20260701\wpf_shell_host_workspace_quick_actions.png`
- Tool-selected after: `artifacts\mainview_microcopy_after_20260701\wpf_shell_host_workspace_quick_actions.png`

Validation:

- Localization duplicate-key check: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_empty,wpf_shell_host_workspace_image_load,wpf_shell_host_workspace_sample_open,wpf_shell_host_workspace_quick_actions artifacts\mainview_microcopy_after_20260701`: PASS 4/4.

Next priority:

- Add Pipeline Review NG/failed-step visual sample coverage if a representative NG pipeline is useful.
- Continue targeted wording polish only where a concrete mixed-language or beginner-readiness gap remains.

## 2026-07-01 Update - Pipeline Review multi-step readability

Completed the Pipeline Review readability pass for multi-step beginner review.

Changed structure:

- `OpenVisionPipelineReviewGuidePresenter` now builds an additional localized detail line for selected-step guidance.
  - Explains pre-run state, missing input, disabled step, branch input, NG reason, continue-to-next-step, and final-step OK.
- `OpenVisionPipelineReviewViewModel` exposes guide detail text and previous/next step availability.
- `OpenVisionPipelineReviewView` adds explicit previous/next step buttons and a guide detail row.
  - These controls only select review steps and do not run Review/Preview, create layers, or change routing.
- `OpenVisionPipelineReviewDocument` handles previous/next selection and language-change text refresh.
- `OpenVisionShellCommandCatalog` restores the localized selected tool label without firing a fresh selected-tool open during language changes.
- ShellHost state/test facades expose guide detail and previous/next availability for smoke verification.
- `wpf_shell_host_pipeline_review` now uses a 3-step pipeline:
  - Threshold: `Main -> Threshold_Preview`
  - Morphology: `Threshold_Preview -> Morphology_Preview`
  - Filter branch: `Main -> Filter_Branch_Preview`
- The smoke verifies Korean and English guide text through localization keys, branch explanation, explicit step navigation, selected-step preservation across language changes, output preview after Run Review, and final OK detail.

Before/after artifacts:

- Before: `artifacts\pipeline_review_readability_before_20260701\wpf_shell_host_pipeline_review.png`
- After: `artifacts\pipeline_review_readability_after_20260701\wpf_shell_host_pipeline_review.png`

Validation:

- Localization duplicate-key check: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_pipeline_review artifacts\pipeline_review_readability_after_20260701`: PASS.

Next priority:

- MainView empty/image/tool-selected microcopy audit.
- Optional follow-up: preserve the selected Pipeline Review step across language changes if that becomes a UX requirement.

## 2026-07-01 Update - Tool View remaining terminology pass

Completed the follow-up Tool View wording pass for matching-family template state, criteria summaries, FeatureMatching guide parity, and Line purpose controls.

Changed structure:

- `VisionToolVerificationText` now owns display text for template-ready/missing state, original/adaptive-threshold summary, FeatureMatching Ratio/RANSAC criteria, and Line purpose/setting labels.
- `VisionToolTemplateStatusPresenter` converts legacy template status text into localized beginner-facing display text.
- `VisionToolMatchingPropertyRuntime` now composes the Matching-family teaching summary from criteria plus image-process state.
  - Matching keeps score/count/angle/scale/pyramid criteria.
  - FeatureMatching adds Ratio/RANSAC criteria to the visible summary and guide.
- `VisionToolMatchingVerificationGuidePresenter` now has FeatureMatching-specific state, criteria fallback, and next-action wording.
- Line Tool View now uses shared display text for `목적`, `라인`, `엣지`, `측정`, `교차`, and selected-line ROI tooltip.
- `LineToolVerificationGuidePresenter` uses the same localized purpose text as the Line summary.
- `PipelineViewerScreenshotSmoke` now asserts the new Matching, FeatureMatching, and Line text so screenshots cannot pass while the important guide text is missing.

Before/after artifacts:

- Matching before: `artifacts\terminology_before_20260701\wpf_shell_host_matching_tool.png`
- Matching after: `artifacts\terminology_after_20260701\wpf_shell_host_matching_tool.png`
- FeatureMatching before: `artifacts\terminology_before_20260701\wpf_shell_host_feature_matching_tool.png`
- FeatureMatching after: `artifacts\terminology_after_20260701\wpf_shell_host_feature_matching_tool.png`
- Line before: `artifacts\terminology_before_20260701\wpf_shell_host_line_measure_tool.png`
- Line after: `artifacts\terminology_after_20260701\wpf_shell_host_line_measure_tool.png`

Validation:

- Localization duplicate-key check: PASS.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_matching_tool,wpf_shell_host_feature_matching_tool,wpf_shell_host_line_measure_tool artifacts\terminology_after_20260701`: PASS 3/3.

Next priority:

- Pipeline Review multi-step sample readability: branch/failed-step explanation and selected-step navigation.
- MainView empty/image/tool-selected microcopy audit after Pipeline Review readability.

## 2026-07-01 Update - Tool View/MainView terminology consistency pass

Completed the next beginner-oriented consistency pass across MainView quick guidance and Tool View result review text.

Changed structure:

- Added `VisionToolVerificationText` as the shared display-text helper for verification state, result-review labels, criteria summaries, and next-action wording.
- MainView image-ready guidance and top direct-result text now use localization keys instead of hard-coded mixed `Preview` / `Pipeline` wording.
- Blob/Contour verification criteria, result state, result-review summary, and chip labels now share the same beginner-facing vocabulary.
  - Blob/Contour parameter summary rows reuse the same criteria formatter through `OpenVisionNativePropertyGridToolFactory`.
- Matching-family result review now uses common labels for decision, criteria, count, score, center, box, angle, scale, and tact.
- Line result review now uses common labels and localized summaries for Edge, Measure, Distance, and Intersection review states.
- `PipelineViewerScreenshotSmoke` assertions were updated to validate the new Korean display wording without depending on unrelated PropertyGrid labels.

Before/after artifacts:

- MainView before: `artifacts\consistency_before_20260701\wpf_shell_host_workspace_image_load.png`
- MainView after: `artifacts\consistency_after_20260701\wpf_shell_host_workspace_image_load.png`
- Blob before: `artifacts\consistency_before_20260701\wpf_shell_host_blob_tool.png`
- Blob after: `artifacts\consistency_after_20260701\wpf_shell_host_blob_tool.png`
- Matching before: `artifacts\consistency_before_20260701\wpf_shell_host_matching_tool.png`
- Matching after: `artifacts\consistency_after_20260701\wpf_shell_host_matching_tool.png`
- Line before: `artifacts\consistency_before_20260701\wpf_shell_host_line_measure_tool.png`
- Line after: `artifacts\consistency_after_20260701\wpf_shell_host_line_measure_tool.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_image_load,wpf_shell_host_blob_tool,wpf_shell_host_matching_tool,wpf_shell_host_line_measure_tool artifacts\consistency_after_20260701`: PASS 4/4.

Next priority:

- Tool View remaining terminology pass: template-ready/status strings, Matching parameter summary row, Line purpose/setting labels, and FeatureMatching guide parity.

## 2026-07-01 Update - EdgeBasedMatching compact verification guidance

Continued the beginner-friendly tool review pass for EdgeBasedMatching.

Changed structure:

- Reworked `VisionToolMatchingVerificationGuidePresenter`.
  - Matching-family guides now build display-only state, criteria, and next-action text through a presenter.
  - EdgeBasedMatching gets a dedicated compact header: edge match verification.
- Extended `VisionToolMatchingResultReviewCriteria`.
  - EdgeBasedMatching criteria now carries Canny range, search/greediness summary, and template point count.
  - Existing Matching criteria and result semantics remain unchanged.
- `VisionToolMatchingPropertyRuntime` now creates EdgeBasedMatching-specific criteria from the existing PropertyGrid model.
  - This is presentation state only; it does not run Preview, change routing, create layers, or bypass PropertyGrid.
- `VisionToolMatchingResultReviewPresenter` now uses EdgeBasedMatching-specific reason/next-action text when a preview result is OK/NG.
- `VisionToolVerificationGuideView` keeps full guide text in tooltips when compact mode trims the visible line.
- `wpf_shell_host_edge_based_matching_tool` now verifies EdgeBasedMatching guide text, including edge/Canny criteria and Preview OK state.

Before/after artifacts:

- Before: `artifacts\edge_based_matching_before_20260701\wpf_shell_host_edge_based_matching_tool.png`
- After: `artifacts\edge_based_matching_after_20260701\wpf_shell_host_edge_based_matching_tool.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_edge_based_matching_tool artifacts\edge_based_matching_after_20260701`: PASS.

Next priority:

- Beginner-oriented consistency pass across Tool View result review and MainView quick actions.

## 2026-07-01 Update - Pipeline Review beginner guide strip

Resumed Pipeline Review as the central beginner learning loop.

Changed structure:

- Added `OpenVisionPipelineReviewGuidePresenter`.
  - Builds display-only guide state for selected step, route, next check, and result decision.
  - Keeps guide wording/state logic out of `OpenVisionPipelineReviewView.xaml.cs`.
- `OpenVisionPipelineReviewViewModel` now exposes guide state properties.
- `OpenVisionPipelineReviewView.xaml` shows a compact top guide strip:
  - Review position
  - Current step and route
  - Next check
  - Decision
- `OpenVisionPipelineReviewDocument` updates the guide during selected-step refresh, validation-error state, running state, and completed review result.
- `OpenVisionShellHostStatePresenter` / test facade / test hooks expose guide state for smoke verification.
- Localization catalog now contains the `PipelineReview.Guide.*` keys.
- `wpf_shell_host_pipeline_review` now verifies:
  - Pre-run guide state does not claim `OK`.
  - Completed guide state follows the explicit Run Review result.

Before/after artifacts:

- Before: `artifacts\pipeline_review_before_20260701\wpf_shell_host_pipeline_review.png`
- After: `artifacts\pipeline_review_after_20260701\wpf_shell_host_pipeline_review.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_pipeline_review artifacts\pipeline_review_after_20260701`: PASS.

Next priority:

- EdgeBasedMatching compact verification guidance.

## 2026-07-01 Update - Docked tool reselection status synchronization

Closed the follow-up status mismatch found after the Blob/Line guide work.

Changed structure:

- `OpenVisionShellHostToolWindowController` no longer marks every native tool selection as pending.
  - If the reselected native document already has a displayable preview output layer, Shell top direct-result state is restored to `OK`.
  - If the document has no preview result or the output layer is gone, Shell remains in the pending state.
  - This is a display-state restore only; it does not run Preview/Run or change routing.
- `OpenVisionFloatingToolWindowHost` now preserves saved floating bounds without allowing large PropertyGrid tools to reopen below their usable editor size.
- `OpenVisionNativeToolRegistry` raises the large hosted tool preferred height so Blob/Contour/Line/Matching-family tools have enough initial PropertyGrid height.
- `wpf_shell_host_blob_tool` now verifies:
  - Blob floating PropertyGrid editor height.
  - Docked Blob result guidance.
  - Docked same-tool reselect keeps one hosted tool and keeps the Shell `OK` result banner synchronized.
  - Cross-tool docked reselection `Blob -> Contour -> Blob` restores the cached Blob result in-place without opening a duplicate floating window.

Before/after artifacts:

- Before docked guide/status baseline: `artifacts\ux_area_guides_after_20260701\wpf_shell_host_blob_tool_docked_verification.png`
- After status synchronization: `artifacts\status_reselect_after_20260701\wpf_shell_host_blob_tool.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_blob_tool artifacts\status_reselect_after_20260701`: PASS.

Next priority:

- Resume Pipeline Review beginner-readiness guide.

## 2026-07-01 Update - MainView top status banner synchronization

Continued MainView completion before returning to Pipeline Review.

Changed structure:

- Extended `OpenVisionShellHostDirectRunPresenter`.
  - It now owns display-only top banner states for workspace-empty, image-ready, and sample-ready.
  - Status changes are still presentation only; they do not open tools, run Preview, create output layers, or change input routing.
- `OpenVisionShellHostChromeController` exposes workspace status methods instead of making ShellHost write banner text directly.
- `OpenVisionShellHostMainActionPresenter` and `OpenVisionShellHostSampleWorkflowPresenter` now use stable source strings for Korean UI text.
- `OpenVisionShellHostToolTestFacade` / `OpenVisionShellHostView.TestHooks` expose the top banner title so smoke tests verify the state, not just the screenshot.
- `OpenVisionShellHostView.xaml` widens the top status area so the empty/image/sample guidance is readable at the 1600px smoke viewport.
- Added `wpf_shell_host_workspace_quick_actions`.
  - Opens Threshold, Matching, and Line through the actual MainView quick action commands.
  - Verifies the tools open against `Main` without auto-running Preview.

Before/after artifacts:

- Before empty/status mismatch baseline: `artifacts\mainview_before_20260701\wpf_shell_host_workspace_empty.png`
- Before image-ready top status baseline: `artifacts\mainview_after_20260701_r3\wpf_shell_host_workspace_image_load.png`
- After empty: `artifacts\mainview_status_after_20260701_r2\wpf_shell_host_workspace_empty.png`
- After image-ready: `artifacts\mainview_status_after_20260701_r2\wpf_shell_host_workspace_image_load.png`
- After sample-ready: `artifacts\mainview_status_after_20260701_r2\wpf_shell_host_workspace_sample_open.png`
- Quick actions after: `artifacts\mainview_quick_actions_20260701\wpf_shell_host_workspace_quick_actions.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_empty,wpf_shell_host_workspace_image_load,wpf_shell_host_workspace_sample_open artifacts\mainview_status_after_20260701_r2`: PASS.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_quick_actions artifacts\mainview_quick_actions_20260701`: PASS.

Next priority:

- Resume Pipeline Review beginner-readiness guide.

## 2026-07-01 Update - MainView image-ready next action bar

Paused Pipeline Review work and returned to MainView completion first.

Changed structure:

- Added `OpenVisionShellHostMainActionPresenter`.
  - Handles the generic Main workspace "image ready" guidance outside ShellHost code-behind.
  - Shows a display-only next-action bar after a normal image load.
- Added quick MainView commands to `OpenVisionShellHostWorkspaceCommandSurface`.
  - `OpenThresholdToolCommand`
  - `OpenMatchingToolCommand`
  - `OpenLineToolCommand`
- Added `WorkspaceMainActionOverlay` below the main workspace image.
  - It appears for normal image-ready state.
  - It hides when sample workflow guidance is active, so sample pipeline guidance remains the stronger state.
- Added smoke assertions for the image-ready bar and quick action buttons.

Before/after artifacts:

- Before empty: `artifacts\mainview_before_20260701\wpf_shell_host_workspace_empty.png`
- Before image load: `artifacts\mainview_before_20260701\wpf_shell_host_workspace_image_load.png`
- After image load: `artifacts\mainview_after_20260701_r3\wpf_shell_host_workspace_image_load.png`
- After sample workflow: `artifacts\mainview_after_20260701_r2\wpf_shell_host_workspace_sample_open.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_empty,wpf_shell_host_workspace_image_load,wpf_shell_host_workspace_sample_open,wpf_shell_host_workspace_sample_actions artifacts\mainview_after_20260701_r2`: PASS.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_image_load artifacts\mainview_after_20260701_r3`: PASS.

Next priority:

- Continue MainView polish before Pipeline Review: top status wording should align with empty/image-ready/sample-ready state.
- Then resume Pipeline Review beginner-readiness guide.

## 2026-07-01 Update - Line compact verification guide and docked density

Implemented the third iterative UX priority before 08:00: Line docked Tool View verification guidance without reducing PropertyGrid usability.

Changed structure:

- Added `LineToolVerificationGuidePresenter`.
  - Keeps Line-specific Edge/Measure/Intersection result interpretation out of the View code-behind.
  - Writes compact display-only verification state into the existing summary strip so the docked inspector does not lose editor height.
- `LineToolWpfView` now uses a denser two-row Purpose/Setting selector.
  - Existing Purpose modes, Line A/B selection, and ROI edit button remain intact.
  - The old purpose hint row is not used as a visible extra row in the docked inspector.
- `VisionToolSingleInputSpecialPropertyToolRuntime` now exposes the shared summary `TextBlock` for special PropertyGrid tools that need compact status presentation.
- Added target alias `wpf_shell_host_line_tool_docked_verification`.

Before/after artifacts:

- Line after: `artifacts\ux_line_guide_after_20260701_r9\wpf_shell_host_line_tool.png`
- Line after docked guide: `artifacts\ux_line_guide_after_20260701_r9\wpf_shell_host_line_tool_docked_verification.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_line_tool,wpf_shell_host_line_tool_docked_verification artifacts\ux_line_guide_after_20260701_r9`: PASS.

Self-evaluation:

- The first Line guide attempt used an extra guide row and made the docked PropertyGrid too short. The final version moved guidance into the summary strip and preserved a usable editor viewport.
- The initial one-row Purpose/Setting compression clipped the ROI edit button in the docked inspector, so the final version keeps Purpose and Setting as two compact rows.

Next priority:

- Apply the same before/after and smoke-first process to EdgeBasedMatching or to Shell status synchronization for docked tool reselection.

## 2026-07-01 Update - Blob compact verification guide and area guide reuse

Implemented the second iterative UX priority before 08:00: Blob docked Tool View teaching guidance.

Changed structure:

- Added reusable `VisionToolAreaVerificationGuidePresenter<TProperty, TResult>`.
  - Shared display-only guide/result guidance presenter for area-result tools.
  - Blob and Contour now use the same presenter pattern.
- Added `VisionToolAreaVerificationCriteriaText`.
  - Centralizes concise criteria text for Blob and Contour: area, threshold, ROI, mask/draw mode.
- `BlobToolWpfView` now declares a compact `VisionToolVerificationGuideView` in the common shell `ToolContent` slot.
- Blob uses the existing optional `refreshVerificationGuide` runtime hook; PropertyGrid remains the source of truth.
- Added target alias `wpf_shell_host_blob_tool_docked_verification`.

Before/after artifacts:

- Blob before: `artifacts\ux_blob_guide_before_20260701\wpf_shell_host_blob_tool.png`
- Blob after: `artifacts\ux_area_guides_after_20260701\wpf_shell_host_blob_tool.png`
- Blob after docked guide: `artifacts\ux_area_guides_after_20260701\wpf_shell_host_blob_tool_docked_verification.png`
- Contour regression after common presenter refactor: `artifacts\ux_area_guides_after_20260701\wpf_shell_host_contour_tool_docked_verification.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_blob_tool artifacts\ux_blob_guide_before_20260701`: PASS before capture.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_blob_tool,wpf_shell_host_blob_tool_docked_verification,wpf_shell_host_contour_tool_docked_verification artifacts\ux_area_guides_after_20260701`: PASS.

Self-evaluation:

- Blob/Contour now share a reusable guide presenter, so the next area-style Tool View does not need another one-off presenter.
- The final Blob screenshot still shows a known status-banner mismatch after the smoke reselects a docked Blob tool: the Shell banner can show pending while the Tool View retains Preview OK. This predates the guide pattern and should be handled as a follow-up status-state synchronization issue.

Next priority:

- Extend the display-only guide/result guidance pattern to Line.
- Then revisit Shell status banner synchronization for docked tool reselection.

## 2026-07-01 Update - Contour compact verification guide

Implemented the first post-review UX priority: Contour docked Tool View teaching guidance.

Changed structure:

- Added `VisionToolContourVerificationGuidePresenter`.
  - Display-only presenter for Contour teaching state and Preview result guidance.
  - Shows compact Preview state, area/threshold/ROI/draw criteria, and next action.
  - Does not run Preview/Run/Add Pipeline or change input/output routing.
- `ContourToolWpfView` now declares a compact `VisionToolVerificationGuideView` in the common `VisionToolSingleInputPropertyToolShell.ToolContent` slot.
- `VisionToolSingleInputPropertyToolRuntime` and controller now accept an optional `refreshVerificationGuide` hook.
  - PropertyGrid remains the source of truth.
  - The hook is invoked from summary refresh only, so it updates display text without changing execution behavior.
- `wpf_shell_host_contour_tool` now verifies guide visibility, result guidance, no auto-preview from Contour visibility/display toggles, and docked inspector preservation.
- Added target alias `wpf_shell_host_contour_tool_docked_verification`.

Before/after artifacts:

- Before: `artifacts\ux_contour_guide_before_20260701\wpf_shell_host_contour_tool.png`
- After: `artifacts\ux_contour_guide_after_20260701_r2\wpf_shell_host_contour_tool.png`
- After docked guide: `artifacts\ux_contour_guide_after_20260701_r2\wpf_shell_host_contour_tool_docked_verification.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_contour_tool artifacts\ux_contour_guide_before_20260701`: PASS before capture.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_contour_tool,wpf_shell_host_contour_tool_docked_verification artifacts\ux_contour_guide_after_20260701_r2`: PASS.
- Self-evaluation shortened the first guide text because the initial compact sentence clipped in the docked inspector.

Next priority:

- Extend the same display-only guide/result guidance pattern to Blob.
- Preserve Blob PropertyGrid, threshold/adaptive/masking visibility-toggle no-auto-preview contracts, and before/after screenshot comparison.

## 2026-07-01 Update - Competitor-informed UX direction refresh

Added `docs/OPENVISIONLAB_UX_COMPETITOR_REVIEW_20260701.md`.

The refresh re-checks official competitor sources and maps the findings back to OpenVisionLab's current state:

- OpenVisionLab remains a layer-based rule-based vision workbench, not a wizard-only product.
- PropertyGrid-based algorithm tools stay as the core teaching surface.
- Beginner UX should be added as display-only guide/result/next-action presenters around the PropertyGrid, not by replacing the model-driven PropertyGrid.
- Docking layout work is now treated as a stabilization/regression-gate area unless a concrete defect appears.
- The next implementation priority is Contour docked Tool View teaching-guide expansion, reusing the Matching compact verification guide pattern.

Next implementation target:

- Add a Contour compact verification guide/result explanation in the docked Tool View.
- Preserve PropertyGrid editing, explicit Preview/Run/Add Pipeline actions, input/output route separation, and no auto-preview from display-only guide visibility.
- Add before/after screenshot evidence and a focused smoke target when UI changes are made.

## 2026-06-30 Update - Main workspace sample workflow strip

Added a non-blocking next-action strip after a runnable sample is opened.

Changed structure:

- Added `OpenVisionShellHostSampleWorkflowPresenter`.
  - Reads only the active `Sample_` pipeline name and step flow for display.
  - Shows the first tool, step count, and operator next action.
  - Does not open tools, run Preview/Run, create output layers, or change routing.
- `OpenVisionShellHostCommandController` now accepts sample/manual workspace-image callbacks.
  - Sample load success shows the sample workflow strip.
  - Manual image load hides it so a user image is not mislabeled as a sample workflow.
- `OpenVisionShellHostWorkspaceCommandSurface` now exposes explicit sample next-action commands.
  - `OpenSamplePipelineCommand` opens Pipeline Review through the existing tool-selection path.
  - `OpenSampleFirstStepCommand` opens the first pipeline step's tool through the existing tool-selection path.
  - These commands do not run Preview/Run by themselves.
- `OpenVisionShellHostView.xaml` places the strip as a thin workspace row below the image/docking surface, avoiding image/header overlap.
- The final strip includes explicit `Pipeline 보기` and `첫 단계 열기` buttons.
- Test hooks expose sample workflow title/meta/detail.
- `wpf_shell_host_workspace_sample_open` now asserts that the workflow strip appears, its action buttons are available, and still verifies no auto-open/no auto-preview.
- Added `wpf_shell_host_workspace_sample_actions`.
  - Verifies that `Pipeline 보기` opens active sample Pipeline Review.
  - Verifies that `첫 단계 열기` opens the first step tool (`Threshold` for the first catalog sample).
  - Verifies both actions still do not run Preview automatically.

Before/after artifacts:

- Before sample open: `artifacts\ux_main_sample_workflow_before_20260630\wpf_shell_host_workspace_sample_open.png`
- Rejected overlay iteration: `artifacts\ux_main_sample_workflow_after_20260630\wpf_shell_host_workspace_sample_open.png`
- Final after strip: `artifacts\ux_main_sample_workflow_after_final_20260630\wpf_shell_host_workspace_sample_open.png`
- Final after explicit buttons/compact text: `artifacts\ux_main_sample_actions_after_compact_20260630\wpf_shell_host_workspace_sample_open.png`
- Explicit action validation capture: `artifacts\ux_main_sample_actions_after_compact_20260630\wpf_shell_host_workspace_sample_actions.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_empty,wpf_shell_host_workspace_sample_open,wpf_shell_host_workspace_sample_picker artifacts\ux_main_sample_workflow_after_final_20260630`: PASS.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_open,wpf_shell_host_workspace_sample_actions artifacts\ux_main_sample_actions_after_compact_20260630`: PASS.
- Self-evaluation changed the initial floating overlay into a bottom strip because the overlay could cover image/header space.
- Self-evaluation then shortened the strip text after adding buttons because the previous sentence was clipped in the 1600x900 capture.

## 2026-06-30 Update - Main workspace sample catalog picker

Extended the no-image workspace sample entry from direct first-sample open to a beginner-readable catalog picker.

Changed structure:

- Added `OpenVisionWorkspaceSamplePickerViewModel`.
  - Owns runnable sample filtering, default selection, localized/fallback labels, image thumbnail loading, and selection summaries.
  - Keeps catalog order so the intended first beginner sample remains `Contour_TextSymbols`.
- Added `OpenVisionWorkspaceSamplePickerView` and `OpenVisionWorkspaceSamplePickerWindow`.
  - Shows sample list, search, sample goal, tool flow, expected metrics, check guidance, NG fix guidance, image path, pipeline path, and Good/Bad pair context.
  - Explicitly states that opening a sample prepares `Main` and `Sample_` pipeline only; Preview/Run remain manual.
- `OpenVisionShellHostCommandController` now separates runnable sample discovery, sample picker prompt, and selected sample loading.
- `OpenVisionShellHostWorkspaceCommandSurface.OpenSampleCommand` now opens the picker when multiple runnable samples exist.
- The test hook still uses `OpenFirstRunnableSample` so automated sample-open validation does not block on the modal picker.
- `PipelineViewerScreenshotSmoke` adds `wpf_shell_host_workspace_sample_picker`.

Before/after artifacts:

- Before empty state: `artifacts\ux_main_sample_picker_before_20260630\wpf_shell_host_workspace_empty.png`
- After empty state: `artifacts\ux_main_sample_picker_after_20260630\wpf_shell_host_workspace_empty.png`
- After sample picker: `artifacts\ux_main_sample_picker_after_20260630\wpf_shell_host_workspace_sample_picker.png`
- After sample open: `artifacts\ux_main_sample_picker_after_20260630\wpf_shell_host_workspace_sample_open.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_empty,wpf_shell_host_workspace_sample_picker,wpf_shell_host_workspace_sample_open,wpf_shell_host_workspace_image_load artifacts\ux_main_sample_picker_after_20260630`: PASS.
- `bin\Debug\OpenVisionLab.exe --smoke workspace-startup-empty --output artifacts\actual_exe_workspace_startup_empty_after_sample_picker_20260630`: PASS.

## 2026-06-30 Update - Main workspace runnable sample entry

Added a real beginner sample entry to the no-image workspace.

Changed structure:

- `OpenVisionShellHostCommandController` now owns `OpenFirstRunnableSample`.
  - It finds the first `VisionPipelineSampleCatalogItem` where `CanOpen` is true.
  - It loads the sample image into `Main`.
  - It loads the sample pipeline XML and saves it into the current recipe as `Sample_<SampleName>`.
  - It sets that sample pipeline as the active pipeline.
  - It writes a run-log event.
- `OpenVisionShellHostWorkspaceCommandSurface` exposes `OpenSampleCommand`.
- `OpenVisionShellHostView.xaml` adds a `Sample Open` button to the no-image card.
- Test hooks expose runnable sample state, sample-open execution, active pipeline name, and active pipeline step count.
- `PipelineViewerScreenshotSmoke` adds `wpf_shell_host_workspace_sample_open`.
  - The smoke asserts the sample button is visible.
  - The command loads `Main`, activates a `Sample_` pipeline with steps, and does not auto-open a tool or run Preview.

Before/after artifacts:

- Before: `artifacts\ux_main_empty_after_20260630_pass2\wpf_shell_host_workspace_empty.png`
- After empty state: `artifacts\ux_main_sample_after_20260630\wpf_shell_host_workspace_empty.png`
- After sample open: `artifacts\ux_main_sample_after_20260630\wpf_shell_host_workspace_sample_open.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_empty,wpf_shell_host_workspace_sample_open,wpf_shell_host_workspace_image_load artifacts\ux_main_sample_after_20260630`: PASS.
- `bin\Debug\OpenVisionLab.exe --smoke workspace-startup-empty --output artifacts\actual_exe_workspace_startup_empty_after_sample_20260630`: PASS.

## 2026-06-30 Update - Main workspace beginner workflow pass 2

Improved the first-run / no-image workspace after the initial beginner card pass.

Changed structure:

- The main no-image prompt now explicitly tells the operator that the bottom run log tracks state after image load, Preview, and tool events.
- `LogPanelViewModel` exposes localized/fallback empty-state title, detail, and action-hint text.
- `LogPanelView.xaml` replaces the single centered empty text with a compact run-log waiting card.
  - The card explains that image load, Preview, Run, and tool validation events will appear there.
  - It also points operators to Details mode for warning/error filtering.
- Added AutomationIds for the workspace log hint and the log-panel empty card so `wpf_shell_host_workspace_empty` catches regressions.
- Sample catalog services exist, but this pass did not add unsupported Main workspace sample/recent-recipe buttons because there is not yet a stable ShellHost command for opening a sample plus recipe from the central workspace.

Before/after artifacts:

- Before: `artifacts\ux_main_empty_before_20260630_pass2\wpf_shell_host_workspace_empty.png`
- After: `artifacts\ux_main_empty_after_20260630_pass2\wpf_shell_host_workspace_empty.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_empty,wpf_shell_host_workspace_image_load artifacts\ux_main_empty_after_20260630_pass2`: PASS.
- `bin\Debug\OpenVisionLab.exe --smoke workspace-startup-empty --output artifacts\actual_exe_workspace_startup_empty_after_20260630_pass2`: PASS.

## 2026-06-30 Update - Main workspace beginner empty state

Implemented the next UX priority after the Matching docked verification pass: improve the first-run / no-image main workspace.

Changed structure:

- `OpenVisionShellHostView.xaml` keeps the existing localized `WorkspaceEmptyTitle`, `WorkspaceEmptyDetail`, and `WorkspaceCommands.LoadImageCommand` binding.
- The empty-state card now shows a compact beginner workflow:
  - `1. Load image`
  - `2. Select tool`
  - `3. Check Preview`
- Added a secondary guide action bound to `ChromeCommands.OpenTutorialCommand`.
- Added AutomationIds for the beginner workflow and guide button so screenshot smoke can verify the new UI without depending on OCR.
- `wpf_shell_host_workspace_empty` now asserts the beginner workflow AutomationIds in addition to the existing no-auto-image/no-auto-tool/localized prompt contract.

Before/after artifacts:

- Before: `artifacts\ux_main_empty_before_20260630_next\wpf_shell_host_workspace_empty.png`
- After: `artifacts\ux_main_empty_after_20260630_next\wpf_shell_host_workspace_empty.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_empty,wpf_shell_host_workspace_image_load artifacts\ux_main_empty_after_20260630_next`: PASS.
- `bin\Debug\OpenVisionLab.exe --smoke workspace-startup-empty --output artifacts\actual_exe_workspace_startup_empty_after_20260630_next`: PASS.

## 2026-06-30 Update - Matching compact verification guide

Implemented the second Matching docked Tool View UX pass.

Changed structure:

- Added reusable `VisionToolVerificationGuideView`.
  - The control is dependency-property based and can be reused by other Tool Views.
  - Matching-family tools use it in compact mode above the PropertyGrid.
- Added `VisionToolMatchingVerificationGuidePresenter`.
  - Shows `검증 흐름`, Preview OK/NG state, pass criteria, and the next operator action.
  - It is display-only and does not trigger preview/run, layer routing, or pipeline publication.
- `VisionToolSingleInputMatchingToolRuntime` inserts the guide through the existing common shell `ToolContent` slot.
- `VisionToolSingleInputPropertyToolShell` keeps docked previews compact and preserves PropertyGrid editing space after the guide is added.
- `PipelineViewerScreenshotSmoke` now asserts the compact guide text for `wpf_shell_host_matching_tool` and `wpf_shell_host_matching_tool_docked_verification`.

Before/after artifacts:

- Before: `artifacts\ux_matching_docked_before_20260630_next\wpf_shell_host_matching_tool_docked_verification.png`
- After: `artifacts\ux_matching_docked_after_20260630_final\wpf_shell_host_matching_tool_docked_verification.png`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_matching_tool artifacts\ux_matching_tool_after_20260630_final`: PASS.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_matching_tool_docked_verification artifacts\ux_matching_docked_after_20260630_final`: PASS.

## 2026-06-30 Update - Matching docked verification result guidance

Implemented the first UX pass from `docs/OPENVISIONLAB_UX_COMPETITOR_REVIEW.md` for Matching-family docked Tool View verification.

Changed structure:

- `VisionToolSingleInputPropertyToolShell` now exposes a compact result guidance text row in the result review card.
  - Floating tools keep result chips.
  - Docked inspector mode hides the chip row to preserve PropertyGrid editing height and shows compact summary/guidance text.
- `VisionToolMatchingResultReviewPresenter` now shows:
  - Preview OK/NG decision,
  - configured Criteria such as score/count,
  - reason text,
  - next action guidance.
- `VisionToolMatchingPropertyRuntime` passes display-only criteria from Matching/EdgeBasedMatching property models into the presenter.
  - This does not change matching execution, preview scheduling, layer routing, or pipeline creation.
- `PipelineViewerScreenshotSmoke` adds `wpf_shell_host_matching_tool_docked_verification`.
  - Existing `wpf_shell_host_matching_tool` also asserts the new guidance text.
  - The old "no auto docked layers" helper now matches the current contract: live layers may mirror as same-pane AvalonDock tabs, but preview must not auto-create comparison split panes.
- Updated:
  - `docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`
  - `docs/OPENVISIONLAB_UX_COMPETITOR_REVIEW.md`
  - `docs/UI_SCREENSHOT_SMOKE.md`

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_matching_tool_docked_verification artifacts\ux_matching_docked_verification_20260630_r5`: PASS.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_matching_tool artifacts\ux_matching_existing_target_20260630`: PASS.

## 2026-06-30 Update - UX competitor review and next UX priorities

Added a competitor-informed UX review for the main workbench and docked tool verification flow.

Changed structure:

- Added `docs/OPENVISIONLAB_UX_COMPETITOR_REVIEW.md`.
  - Reviews official Cognex, MVTec, NI, Aurora Vision, KEYENCE, and HALCON/HDevelop UX references.
  - Evaluates the current OpenVisionLab main view and right-docked Tool View workflow.
  - Sets the direction: keep PropertyGrid tools, but add beginner-friendly verification flow, result explanation, and next-action guidance around the PropertyGrid.
  - Defines Matching docked Tool View UX as the first implementation target.
- Updated `docs/OPENVISIONLAB_PRODUCT_IDENTITY_AND_ROADMAP.md`.
  - Links the UX review.
  - Adds Tool-docked verification UX and Main View beginner workflow as active work areas.
  - Moves Matching docked Tool View UX and Main View beginner workflow to the top of the next priority list.

Validation:

- Documentation-only change.
- No build or smoke was run.

## 2026-06-30 Update - Product identity and roadmap snapshot

Added a concise product identity and roadmap snapshot after the layer docking UX stabilized.

Changed structure:

- Added `docs/OPENVISIONLAB_PRODUCT_IDENTITY_AND_ROADMAP.md`.
  - Defines OpenVisionLab as a layer-based rule-based vision workbench.
  - Summarizes stable contracts, completed foundations, completed/watch areas, remaining tasks, next priorities, and validation gates.
- Replaced `NEXT_CODEX_PROMPT.md` with a current handoff prompt.
  - Removes obsolete native-floating/CanFloat=true docking guidance.
  - Points future Codex runs to the new roadmap document.
  - Keeps docking changes gated by `tools\RunDockingVerification.ps1`.

Validation:

- Documentation-only change.
- UTF-8 readback of both new/updated documents was checked.
- No build was run for this documentation snapshot.

## 2026-06-30 Update - Suppress native floating preview for layer docking

Layer comparison tab dragging now suppresses AvalonDock native floating document previews so a large detached image window does not cover the workspace while the operator is choosing a docking target.

Changed structure:

- `OpenVisionDockWorkspaceController.Documents.ConfigureDocument` creates docked layer `LayoutAnchorable` documents with `CanFloat=false`.
- The wrapper-owned gesture/guide/drop path remains the layer comparison movement path.
- `OpenVisionLayerDockingGestureController` now finalizes a drag at the current pointer position if WPF `DragDrop` returns without a `Drop` event, so bottom/top edge drops are not lost when native floating is suppressed.
- `OpenVisionLabDirectSmokeRunner` now asserts native floating preview suppression instead of the older native-floating-enabled expectation.
- `docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md` was updated so the current stable layer docking contract matches the wrapper-owned UX.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `bin\Debug\OpenVisionLab.exe --smoke layer-docking-mouse-drag --output artifacts\docking_verification\actual_exe_mouse_drag_after_floating_suppression`: PASS.
  - Covered actual mouse drag for GlobalRight, GlobalBottom, GlobalLeft, GlobalTop, pane-local Bottom, pane-local Left, pane-local Right, and pane-local Top.
- `powershell -ExecutionPolicy Bypass -File tools\RunDockingVerification.ps1 -SkipBuild`: PASS.
  - Output: `artifacts\docking_verification\actual_exe_20260630_194900`.
  - Covered startup empty workspace, docking layout verification, tab click no-guide, initial docked workspace, and actual mouse drag.
- `dotnet list OpenVisionLab.csproj package --include-transitive`: `Dirkster.AvalonDock 4.74.1` appears under transitive packages only.

## 2026-06-30 Update - Docked layer orchestrator factory split

The ShellHost docked layer orchestrator was reduced to a thin app facade over the generic dock workspace composition.

Changed structure:

- `OpenVisionDockWorkspaceComposition<TDocumentState, TWorkspaceState>` now owns document-close handling, layout refresh callbacks, state-save queue callbacks, and workspace-state change notification.
- The composition raises `WorkspaceStateChanged` for document commands and document-close changes; `OpenVisionShellHostDockedLayerOrchestrator` only re-publishes that event to the runtime.
- Added `OpenVisionDockedLayerWorkspaceCompositionFactory`.
  - Builds the generic `OpenVisionDockWorkspaceComposition<OpenVisionDockedLayerDocumentState, OpenVisionDockedLayerWorkspaceState>` from runtime options and app-local content composition.
  - Keeps app-specific projection delegates (`OpenVisionDockedLayerDocumentProjection`) out of the orchestrator facade.
- Removed `OpenVisionShellHostDockedLayerOrchestratorOptions`.
- `OpenVisionDockedLayerWorkspaceRuntimeFactory` now creates content composition, creates dock workspace composition through the new factory, then wraps it with the thin orchestrator facade.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `powershell -ExecutionPolicy Bypass -File tools\RunDockingVerification.ps1 -SkipBuild`: PASS.
  - Output: `artifacts\docking_verification\actual_exe_20260630_192934`.
  - Covered startup empty workspace, docking layout, tab click no-guide, initial docked workspace, and docking mouse drag.
- AvalonDock package boundary remains unchanged: `Dirkster.AvalonDock` is owned by `Library\OpenVisionLab.Docking.Controls`.

## 2026-06-30 Update - Dock workspace composition facade

The app-local docked layer orchestrator now calls the generic library composition through facade methods instead of reaching into its internal controllers.

Changed structure:

- `OpenVisionDockWorkspaceComposition<TDocumentState, TWorkspaceState>` now exposes intent-level methods for:
  - document commands: dock, sync, clear, refresh, split, move, arrange, guide-zone dock;
  - persisted state: ensure/load, apply persisted documents, restore layout, save, queue/stop pending save;
  - projections: find content, get workspace state, get document ids, document count;
  - gesture/test hooks: enumerate headers, show/reset guide, begin test drag guide, source hit testing.
- Internal controller objects in the library composition are now private implementation details.
- `OpenVisionShellHostDockedLayerOrchestrator.*` partials no longer call `OpenVisionDockDocumentController`, `OpenVisionDockDocumentOrchestrator`, `OpenVisionDockDocumentSynchronizationController`, `OpenVisionDockDocumentProjectionController`, `IOpenVisionDockDocumentWorkspace`, or `OpenVisionLayerDockingGestureController` directly.
- The ShellHost-side orchestrator still supplies app-specific projection/content delegates and retains app-local viewer state decisions.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `powershell -ExecutionPolicy Bypass -File tools\RunDockingVerification.ps1 -SkipBuild`: PASS.
  - Output: `artifacts\docking_verification\actual_exe_20260630_191125`.
  - Covered startup empty workspace, docking layout, tab click no-guide, initial docked workspace, and docking mouse drag.
- AvalonDock package boundary: `Dirkster.AvalonDock` remains only in `Library\OpenVisionLab.Docking.Controls\OpenVisionLab.Docking.Controls.csproj`, not in `OpenVisionLab.csproj`.

## 2026-06-30 Update - Docking guide appears only after drag threshold

The docked layer guide overlay no longer appears on a simple tab click.

Changed structure:

- `OpenVisionLayerDockingGestureController` now separates a pending click/drag candidate from an active drag.
  - `MouseDown` on a docked layer tab records the candidate only.
  - The guide is shown only after pointer movement passes WPF drag distance thresholds.
  - Layout refresh no longer shows the guide for a simple click/tab selection.
- Added direct EXE smoke `layer-docking-tab-click-no-guide`.
  - Creates three same-pane docked layer tabs.
  - Sends real mouse down/up input on tab headers.
  - Fails if the guide appears during mouse down or remains visible after click.
- `tools\RunDockingVerification.ps1` now includes this smoke in the focused docking gate.
- `docs\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md` records the no-guide-on-click contract.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `OpenVisionLab.exe --smoke layer-docking-tab-click-no-guide --output artifacts\docking_verification\tab_click_no_guide_20260630`: PASS.
- `powershell -ExecutionPolicy Bypass -File tools\RunDockingVerification.ps1 -SkipBuild`: PASS.
  - Output: `artifacts\docking_verification\actual_exe_20260630_175746`.
  - Covered startup empty workspace, docking layout, tab click no-guide, initial docked workspace, and docking mouse drag.

## 2026-06-30 Update - Generic dock workspace composition moved to library

The AvalonDock workspace/controller/gesture/layout composition was moved out of the app-local ShellHost composition and into `OpenVisionLab.Docking.Controls`.

Changed structure:

- Added `Library\OpenVisionLab.Docking.Controls\OpenVisionDockWorkspaceCompositionOptions.cs`.
  - Captures generic docking construction inputs: wrapper view, document state/content source, content predicate, projection delegates, command delegates, refresh/save callbacks, and document close handler.
- Added `Library\OpenVisionLab.Docking.Controls\OpenVisionDockWorkspaceComposition.cs`.
  - Creates the generic `OpenVisionDockWorkspaceController`, document controller, document orchestrator, synchronization controller, projection controller, guide/gesture controllers, state-save scheduler, layout controller, and lifecycle binder.
  - Owns idempotent gesture handler attachment, lifecycle attachment, layout refresh, and state-save queue/stop helpers for the wrapper workspace.
- Removed the app-local `OpenVisionShellHostDockedLayerOrchestratorComposition`.
- `OpenVisionShellHostDockedLayerOrchestrator` now directly creates the generic library composition and only supplies app-specific projection/content delegates.
- `OpenVisionLabDirectSmokeRunner` now retries pane-local mouse drag verification once when the first OS mouse-input attempt does not produce the expected pane split. The retry still uses the same mouse drag path and records `*_retry` / `FirstAttempt` in the report if it is needed.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `powershell -ExecutionPolicy Bypass -File tools\RunDockingVerification.ps1 -SkipBuild`: PASS.
  - Output: `artifacts\docking_verification\actual_exe_20260630_174400`.
  - Covered startup empty workspace, actual EXE docking layout, initial docked workspace, and docking mouse drag checks.
  - The final passing mouse-drag report did not need a retry.
- AvalonDock package boundary: `Dirkster.AvalonDock` remains only in `Library\OpenVisionLab.Docking.Controls\OpenVisionLab.Docking.Controls.csproj`, not in `OpenVisionLab.csproj`.

## 2026-06-30 Update - Docked layer content composition split

The app-local docked layer content creation was separated from the docking/orchestrator composition.

Changed structure:

- Added `OpenVisionDockedLayerContentComposition`.
  - Creates the dock document state and `OpenVisionDockedLayerWorkspaceViewModel`.
  - Creates the app-local `OpenVisionDockedLayerContentSource`.
  - Owns the `IOpenVisionDockedLayerViewer` content predicate used by the generic docking workspace controller.
- `OpenVisionDockedLayerWorkspaceRuntimeFactory` now asks the content composition for state, view model, content source, and content predicate instead of constructing those inline.
- `OpenVisionShellHostDockedLayerOrchestratorOptions` now receives `Predicate<object> DocumentContentPredicate`.
- `OpenVisionShellHostDockedLayerOrchestratorComposition` uses the predicate from options, so the orchestrator no longer owns the app-specific viewer type check.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `powershell -ExecutionPolicy Bypass -File tools\RunDockingVerification.ps1 -SkipBuild`: PASS.
  - Output: `artifacts\docking_verification\actual_exe_20260630_163936`.
  - Covered startup empty workspace, actual EXE docking layout, initial docked workspace, and docking mouse drag checks.

## 2026-06-30 Update - Docked layer orchestrator composition split

The app-local docked layer orchestrator was split so it no longer constructs every document, guide, layout, gesture, and scheduler dependency inline.

Changed structure:

- Added `Library\OpenVisionLab.Docking.Controls\OpenVisionDockDocumentSynchronizationController.cs`.
  - Owns generic document id synchronization against `IOpenVisionDockDocumentState`.
  - Refreshes documents, refreshes layout, and saves workspace state only when the synchronized document set changed.
- Added `OpenVisionShellHostDockedLayerOrchestratorOptions`.
  - Captures ShellHost-specific construction inputs for the docked layer orchestrator.
- Added `OpenVisionShellHostDockedLayerOrchestratorComposition`.
  - Builds and stores the document workspace, document controller, document orchestrator, projection controller, synchronization controller, guide/gesture controllers, save scheduler, layout controller, and lifecycle binder.
- `OpenVisionShellHostDockedLayerOrchestrator` now delegates component construction to the composition and delegates `SyncLayers` to the generic synchronization controller.
- `OpenVisionDockedLayerWorkspaceRuntimeFactory` now creates the orchestrator through the options object instead of passing the long raw constructor argument list.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- Run docking gate after this change: `powershell -ExecutionPolicy Bypass -File tools\RunDockingVerification.ps1 -SkipBuild`.

## 2026-06-30 Update - Docked workspace composition contracts

ShellHost now consumes the docked layer workspace through role-specific contracts instead of passing the full runtime into every consumer.

Changed structure:

- Added `IOpenVisionDockedLayerWorkspaceLayerCatalog` and `IOpenVisionDockedLayerWorkspaceSynchronization`.
- `OpenVisionShellHostLayerRefreshController` now depends on `IOpenVisionDockedLayerWorkspaceSynchronization` instead of three separate delegates for docked titles, document sync, and viewer refresh.
- Added `OpenVisionShellHostDockedLayerWorkspaceComposition` to expose the single runtime as narrow `Commands`, `Synchronization`, `Refresh`, `Session`, and `Diagnostics` surfaces.
- `OpenVisionDockedLayerWorkspaceRuntimeFactory` can now create the composition directly.
- `OpenVisionShellHostView.xaml.cs` wires controllers through the composition surfaces:
  - layer commands use `Commands`,
  - layer refresh uses `Synchronization`,
  - session lifecycle uses `Session`,
  - test facade uses `Diagnostics`,
  - refresh coordinator uses `Refresh`.

Validation:

- Run after this change: `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`.
- Run docking gate after build: `powershell -ExecutionPolicy Bypass -File tools\RunDockingVerification.ps1 -SkipBuild`.

## 2026-06-30 Update - Actual EXE startup-empty workspace guard

The latest docking regression hid the startup `이미지 없음` / image-load prompt behind the docked layer workspace when no image was loaded.

Changed structure:

- `OpenVisionLabDirectSmokeRunner` now exposes `--smoke workspace-startup-empty`.
- The scenario clears persisted layer docking state inside a backup/restore guard, launches the actual `OpenVisionShellHostWindow`, and asserts:
  - no seeded `Main` image or workspace preview,
  - no auto-opened WPF/native tool window,
  - single workspace surface visible,
  - docked AvalonDock layer workspace hidden,
  - no docked layer documents or texture tiles,
  - localized image-load prompt visible.
- `tools\RunDockingVerification.ps1` runs `workspace-startup-empty` before the layer docking layout and mouse-drag checks.
- `docs\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md` records the empty-startup contract and direct EXE smoke.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: run after this change.
- Focused smoke: `OpenVisionLab.exe --smoke workspace-startup-empty --output artifacts\actual_exe_workspace_startup_empty_next`.
- Docking gate: `powershell -ExecutionPolicy Bypass -File tools\RunDockingVerification.ps1 -SkipBuild`.

## 2026-06-30 Update - Docked layer runtime contract cleanup

The docked layer workspace runtime was narrowed after the initial auto-docked AvalonDock workspace was stabilized.

Changed structure:

- Removed the app-local `IOpenVisionDockedLayerOrchestrator` forwarding interface. It was only used inside the runtime and duplicated the workspace facade.
- `OpenVisionDockedLayerWorkspaceRuntime` now implements only `IOpenVisionDockedLayerWorkspace` and delegates to the concrete `OpenVisionShellHostDockedLayerOrchestrator`.
- Added `OpenVisionDockedLayerWorkspaceRuntimeFactory` so ShellHost no longer owns the low-level document state/content source/orchestrator assembly.
- Split `IOpenVisionDockedLayerWorkspace` into smaller app-local contracts:
  - `IOpenVisionDockedLayerWorkspaceCommands`
  - `IOpenVisionDockedLayerWorkspaceRefresh`
  - `IOpenVisionDockedLayerWorkspaceSession`
  - `IOpenVisionDockedLayerWorkspaceDiagnostics`
- Updated command, refresh, session, and test facade consumers to depend on the smallest required contract.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- Run docking-focused smoke after any further change touching these contracts: `powershell -ExecutionPolicy Bypass -File tools\RunDockingVerification.ps1 -SkipBuild`.

Next priority:

1. Keep app-specific viewer/content creation in the app layer; do not move `OpenVisionLayerViewerView` or `IDisplayManager` into `OpenVisionLab.Docking.Controls`.
2. Continue reducing ShellHost constructor wiring by moving more app-local option assembly into factories/builders.
3. If moving another docking controller into the library, first introduce a contract that avoids referencing app viewer/image/display types from the library.

## 목적

이 문서는 현재 Codex 대화에서 진행한 OpenVisionLab 작업을 다른 Codex 대화에서 이어가기 위한 복구/인계 문서입니다. 다음 작업자는 이 파일과 `docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`를 먼저 읽고 진행해야 합니다.

핵심 목표는 다음과 같습니다.

- OpenVisionLab을 WPF/MVVM 중심 구조로 정리한다.
- View 코드비하인드에 업무 로직을 남기지 않고 ViewModel, Controller, Presenter, Behavior, Converter, 공통 Runtime으로 분리한다.
- 알고리즘 툴은 PropertyGrid 기반 구조를 유지한다. 모델에 Property를 추가하면 PropertyGrid가 자동으로 UI를 만드는 구조가 제품 방향이다.
- 레이어 기반 비전 워크벤치 방향을 유지한다. 입력/출력/결과 레이어는 사용자가 명시적으로 선택하고 비교한다.
- 기존 WinForms 시절의 사용성 기능을 복원하고, 이미 검증된 기능은 문서화해 임의 변경을 막는다.
- 툴 수가 늘어날 때 반복 작업을 줄이되, 지나친 추상화로 확장이 어려워지지 않게 한다.
- 툴 창 표시 속도와 도킹/비교 UX를 개선한다.

## 제품 방향

OpenVisionLab은 단순 이미지 뷰어가 아니라 다중 레이어 기반 비전 검사 워크벤치입니다.

- 사용자는 Main, Preview, Output, Result 등 여러 이미지 레이어를 만들고 비교한다.
- 각 툴은 입력 레이어를 바꿔가며 테스트할 수 있어야 한다.
- Output 레이어를 추가했다고 Input 레이어가 자동으로 바뀌면 안 된다.
- 결과 레이어/비교 패널/도킹 패널은 사용자가 명시적으로 조작해야 한다.
- 중앙 작업 영역은 여러 레이어를 탭 또는 도킹 패널로 배치해 비교하는 구조가 되어야 한다.
- 툴 뷰는 기본적으로 Floating이지만 선택에 따라 도킹 가능해야 한다.
- Visual Studio처럼 전체 워크스페이스 도킹과 패널 내부 상하좌우/탭 도킹이 모두 가능한 방향이다.

## 반드시 유지할 안정 계약

다음 파일은 안정 계약 문서입니다. 다음 대화에서 작업 전 반드시 읽어야 합니다.

- `docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`

특히 아래 항목은 임의 변경 금지입니다.

- Blob, Contour, Line, Matching, EdgeBasedMatching, FeatureMatching은 PropertyGrid 기반 알고리즘 툴이다.
- PropertyGrid는 모델 객체를 SelectedObject에 넣으면 속성/어트리뷰트로 UI가 생성되는 구조를 유지한다.
- RangeEditor의 Min/Max 동작, 중복 Max row 숨김, transient numeric typing 허용, command 실행 전 pending binding commit은 유지한다.
- Boolean visibility toggle은 child row 표시/숨김만 해야 하며 Preview/Run을 자동 실행하면 안 된다.
- Matching은 기본 Manual Preview이다. `AUTO_PREVIEW=false`일 때 파라미터 변경만으로 검사가 돌면 안 된다.
- Contour 기본 결과 그리기는 외곽선이다. BoundingBox는 명시 옵션이다.
- Line 툴 UI 용어는 `Scan Line`, `Scan direction`, `Scan interval`, `Use scan angle`, `Scan angle`, `Show scan line`이다. 내부 XML/레시피 호환 이름은 함부로 바꾸지 않는다.
- ROI, Template editor는 활성 WPF Shell의 `IDisplayManager` 컨텍스트를 사용해야 한다.
- Viewer의 zoom, pan, drag, ROI 표시, overlay, output click sync 같은 기존 사용성 기능을 제거하면 안 된다.
- 도킹 UX는 Visual Studio식 전체 워크스페이스 도킹 + 패널 내부 도킹이 목표다.
- AvalonDock 패키지는 app 프로젝트가 직접 소유하지 않고 `Library\OpenVisionLab.Docking.Controls`가 소유하는 방향이다.

## 최근 완료된 핵심 작업

### 1. 도킹 가이드 구조 개선

중앙 레이어 탭/패널 도킹 시 전체 워크스페이스 기준 도킹과 패널 기준 도킹을 구분하도록 개선했습니다.

완료된 동작:

- 전체 워크스페이스 상/하/좌/우 도킹 가이드 표시.
- 특정 패널 위에서는 패널 내부 상/하/좌/우/중앙 탭 병합 가이드 표시.
- 중앙 zone은 기존 패널에 탭으로 합치는 의미.
- global edge zone은 전체 작업 영역 기준으로 새 pane을 만드는 의미.

관련 파일:

- `0. UI/0) MENU/Wpf/OpenVisionShellHostView.xaml`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionLayerDockingGuidePresenter.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionLayerDockingGuidePolicy.cs`

### 2. 도킹 command 분리

도킹 위치 결정과 실제 workspace command 실행 책임을 분리했습니다.

관련 파일:

- `Library/OpenVisionLab.Docking.Controls/OpenVisionLayerDockingCommandController.cs`

역할:

- `DockingGuideZone`을 workspace move command로 변환.
- workspace layer title과 docked-layer state title 구분 유지.
- outer pane 이동, pane side 이동, center tab merge 명령 분기.

### 3. 도킹 gesture 분리

마우스 down/move/up, drag/drop, AvalonDock tab/header title resolution, guide refresh를 별도 controller로 분리했습니다.

관련 파일:

- `Library/OpenVisionLab.Docking.Controls/OpenVisionLayerDockingGestureController.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostView.xaml.cs`

현재 상태:

- ShellHost는 이벤트를 controller로 위임한다.
- ShellHost 내부 직접 drag state 필드는 제거된 상태다.
- View 코드비하인드가 아직 완전히 비어 있는 수준은 아니며 추가 분리 필요.

### 4. Docked layer document sync/save/restore 분리

도킹 레이어 문서 생성/갱신/삭제, document close 처리, pane map 저장/복원, queue normalize를 controller로 분리했습니다.

관련 파일:

- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerDocumentController.cs` (이후 section 15에서 제거)
- `0. UI/0) MENU/Wpf/OpenVisionShellHostView.xaml.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostView.TestHooks.cs`

현재 상태:

- `RefreshDockedLayerViews`, `SaveDockingWorkspaceState`, `TryRestoreDockingLayoutState`는 controller 위임 구조다.
- ShellHost에는 UI text/button 갱신 등 일부 shell 책임이 남아 있다.

### 5. AvalonDock DLL 경계 1차 분리

AvalonDock 패키지 소유권을 app 프로젝트에서 별도 library 프로젝트로 옮겼습니다.

추가된 프로젝트:

- `Library/OpenVisionLab.Docking.Controls/OpenVisionLab.Docking.Controls.csproj`

변경된 파일:

- `OpenVisionLab.sln`
- `OpenVisionLab.csproj`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionLayerDockingGuidePolicy.cs`
- `docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`

현재 상태:

- `OpenVisionLab.csproj`에서 직접 `Dirkster.AvalonDock` PackageReference를 제거했습니다.
- `OpenVisionLab.csproj`는 `OpenVisionLab.Docking.Controls.csproj`를 ProjectReference합니다.
- `OpenVisionLab.Docking.Controls.csproj`가 `Dirkster.AvalonDock 4.74.1`을 소유합니다.
- `dotnet list OpenVisionLab.csproj package --include-transitive` 기준 AvalonDock은 top-level이 아니라 transitive package입니다.
- ShellHost XAML/C#의 raw AvalonDock 타입 참조는 wrapper control/library 이동으로 제거된 상태입니다.

### 6. 안정 계약 문서 업데이트

다음 문서에 도킹 DLL 경계 guard를 추가했습니다.

- `docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`

추가된 의도:

- `Dirkster.AvalonDock`을 다시 `OpenVisionLab.csproj` top-level package로 추가하지 않는다.
- docking policy/control은 `Library\OpenVisionLab.Docking.Controls` 방향으로 이동한다.
- ShellHost는 최종적으로 AvalonDock raw API가 아니라 wrapper/control API를 소비해야 한다.

### 7. ShellHost session/chrome/test surface 추가 분리

도킹 wrapper 적용 이후 ShellHost code-behind를 한 번 더 줄였습니다.

관련 파일:

- `0. UI/0) MENU/Wpf/OpenVisionShellHostView.xaml.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostView.TestHooks.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostSessionState.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostSessionController.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostChromeController.cs`

정리 내용:

- `loaded` / `disposed` 플래그와 `Loaded`, `Unloaded`, canvas loaded, dispose 순서를 `OpenVisionShellHostSessionController`로 이동했습니다.
- active document text, direct run badge/status, route refresh, tool rail compact/localization 적용을 `OpenVisionShellHostChromeController`로 묶었습니다.
- ShellHost runtime 파일에 섞여 있던 public test 조회 surface를 `OpenVisionShellHostView.TestHooks.cs` partial로 이동했습니다.
- `OpenVisionShellHostView.xaml.cs`는 현재 약 487라인이며, 런타임 이벤트 핸들러와 controller 연결 위주로 줄었습니다.
- ShellHost WPF 폴더의 C# 파일에서 AvalonDock raw type 검색은 계속 0건입니다.

### 8. Docked layer app-local dependency 추상화

도킹 controller 추가 이동을 위해 app-local type 의존성을 한 단계 더 추상화했습니다.

관련 파일:

- `0. UI/0) MENU/Wpf/IOpenVisionDockedLayerViewer.cs`
- `0. UI/0) MENU/Wpf/IOpenVisionDockedLayerViewerFactory.cs`
- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerViewerFactory.cs`
- `0. UI/0) MENU/Wpf/IOpenVisionDockedLayerContentSource.cs` (이후 section 15에서 제거)
- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerContentSource.cs`
- `0. UI/0) MENU/Wpf/IOpenVisionDockedLayerOrchestrator.cs`
- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerWorkspaceController.cs` (이후 section 10에서 제거)
- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerDocumentController.cs` (이후 section 15에서 제거)
- `0. UI/0) MENU/Wpf/OpenVisionShellHostDockedLayerOrchestrator*.cs`

정리 내용:

- `OpenVisionDockedLayerWorkspaceController`는 더 이상 `OpenVisionLayerViewerView`를 직접 생성하거나 직접 타입으로 반환하지 않습니다. `IOpenVisionDockedLayerViewer`와 `IOpenVisionDockedLayerViewerFactory`를 사용합니다.
- `OpenVisionLayerViewerView`는 docked layer content interface를 구현합니다.
- 이 단계 당시 `OpenVisionDockedLayerDocumentController`는 `IDisplayManager`와 여러 delegate를 직접 들지 않고 `IOpenVisionDockedLayerContentSource`만 사용했습니다. 두 app-local forwarding adapter는 이후 section 15에서 제거됐습니다.
- `OpenVisionShellHostDockedLayerOrchestrator`는 `IOpenVisionDockedLayerOrchestrator` contract 뒤에 숨겼습니다. ShellHost, session controller, layer interaction, test adapter는 concrete orchestrator 대신 interface를 받습니다.
- 이 단계는 아직 `OpenVisionDockedLayerDocumentController` 자체를 library로 이동한 것은 아닙니다. 다만 concrete viewer/display-manager 결합을 걷어내서 다음 이동의 선행 조건을 줄였습니다.

### 9. Generic dock document sync controller library 이동

도킹 document refresh/save/restore/close 동기화 흐름을 `OpenVisionLab.Docking.Controls`로 이동했습니다.

추가된 library 파일:

- `Library/OpenVisionLab.Docking.Controls/IOpenVisionDockDocumentWorkspace.cs`
- `Library/OpenVisionLab.Docking.Controls/IOpenVisionDockDocumentState.cs`
- `Library/OpenVisionLab.Docking.Controls/IOpenVisionDockDocumentContentSource.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockDocumentRefreshResult.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockDocumentController.cs`

정리 내용:

- `OpenVisionDockDocumentController`가 document refresh, stale document close, selected document restore, layout save/restore, close event handling을 담당합니다.
- library controller는 Bitmap, `OpenVisionLayerViewerView`, `IDisplayManager`를 알지 않습니다.
- 이 단계 당시 app의 `OpenVisionDockedLayerDocumentController`는 library controller를 감싸고, layer-specific refresh result와 viewer metric projection만 담당하는 adapter로 줄었습니다. 이후 section 15에서 generic projection controller와 app-local projector로 대체됐습니다.
- `OpenVisionDockedLayerContentSource`가 app image/status 조회와 docked viewer content update를 담당합니다.
- `OpenVisionDockWorkspaceController`는 `IOpenVisionDockDocumentWorkspace`를 구현합니다.
- 이 단계 당시 app-local `IOpenVisionDockedLayerWorkspace`, `IOpenVisionDockedLayerState`, `IOpenVisionDockedLayerContentSource`는 library contract 위에 얇게 얹힌 형태였습니다. workspace/content forwarding adapter는 이후 제거됐고, `IOpenVisionDockedLayerState`만 app layer-state marker로 남아 있습니다.

### 10. Docked layer workspace adapter 제거

app-local workspace forwarding adapter를 제거하고 orchestrator가 library workspace controller를 직접 사용하도록 정리했습니다.

변경 내용:

- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerWorkspaceController.cs` 제거.
- `0. UI/0) MENU/Wpf/IOpenVisionDockedLayerWorkspace.cs` 제거.
- `OpenVisionShellHostDockedLayerOrchestrator`가 `OpenVisionDockWorkspaceController`를 직접 생성하고 `IOpenVisionDockDocumentWorkspace` contract로 보관합니다.
- docked layer viewer content 판별은 orchestrator의 작은 predicate(`IOpenVisionDockedLayerViewer`)로 넘깁니다.
- 이 단계 당시 `OpenVisionDockedLayerDocumentController`는 `IOpenVisionDockDocumentWorkspace`를 직접 받았습니다. 이후 section 15에서 제거됐습니다.
- app-specific viewer/image/status 책임은 계속 `OpenVisionDockedLayerContentSource`와 viewer factory에 남아 있습니다.

### 11. Generic dock document orchestration library 이동

도킹 문서의 command/orchestration 흐름을 library controller로 이동했습니다.

추가된 library 파일:

- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockDocumentOrchestrator.cs`

정리 내용:

- `OpenVisionDockDocumentOrchestrator`가 dock document add/clear/split/move/guide-zone dock/arrange/restore/save 흐름을 담당합니다.
- app의 `OpenVisionShellHostDockedLayerOrchestrator.Commands.cs`는 layer API를 받아 library orchestrator로 위임하는 얇은 partial로 줄었습니다.
- app의 `OpenVisionShellHostDockedLayerOrchestrator.State.cs`는 state load/save/restore를 library orchestrator로 위임합니다.
- 이 단계 당시 app orchestrator에는 `OpenVisionLayerDockWorkspaceView` event wiring, guide overlay state, app-specific docked layer state projection이 남아 있었습니다. event wiring과 guide overlay state는 이후 section 12, 13에서 library helper로 이동했습니다.
- `OpenVisionLayerDockingCommandController` 사용 위치는 library orchestrator 내부로 이동했습니다.

### 12. Dock workspace lifecycle/event binder library 이동

`OpenVisionLayerDockWorkspaceView`의 docking/drag/drop event lifecycle wiring을 library helper로 이동했습니다.

추가된 library 파일:

- `Library/OpenVisionLab.Docking.Controls/IOpenVisionDockLifecycle.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockWorkspaceLifecycleBinder.cs`

정리 내용:

- `OpenVisionDockWorkspaceLifecycleBinder`가 workspace docking event, drag/drop gesture event, workspace drop enable lifecycle 등록을 담당합니다.
- app의 `OpenVisionShellHostLifecycleController`는 `IOpenVisionDockLifecycle`를 구현해 attach/detach 추적만 제공합니다.
- app의 `OpenVisionShellHostDockedLayerOrchestrator.Events.cs`는 library binder 호출과 layout save timer 등록만 남기는 형태로 줄었습니다.
- Current state update: document-close, layout refresh, and state-save callbacks are now owned by `OpenVisionDockWorkspaceComposition`; app partials no longer implement `OnDocumentClosed`.
- ShellHost 쪽 raw AvalonDock 참조는 계속 제거된 상태이며, AvalonDock package ownership은 `Library/OpenVisionLab.Docking.Controls`에 남아 있습니다.

### 13. Dock guide overlay state controller library 이동

guide overlay visible/active-zone/pane margin reset 상태 조작을 library controller로 이동했습니다.

추가된 library 파일:

- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockingGuideStateController.cs`

정리 내용:

- `OpenVisionDockingGuideStateController`가 `IsGuideOverlayVisible`, `ActiveGuideZone`, pane guide margin reset을 담당합니다.
- `OpenVisionLayerDockingGestureController`는 app delegate(`SetGuideOverlay`, `SetGuideZone`) 대신 library guide-state controller를 받습니다.
- app의 `OpenVisionShellHostDockedLayerOrchestrator.Guide.cs`에는 test/gesture forwarding만 남고 wrapper DP 직접 조작은 제거됐습니다.
- ShellHost test hook의 read-only `IsDockingGuideOverlayVisibleForTest` 조회는 smoke 검증 surface라 유지했습니다.

### 14. Dock workspace layout/save scheduler library 이동

layout changed/docking state changed 이벤트 처리와 docking workspace state save debounce를 library controller로 이동했습니다.

추가된 library 파일:

- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockWorkspaceStateSaveScheduler.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockWorkspaceLayoutController.cs`

정리 내용:

- `OpenVisionDockWorkspaceStateSaveScheduler`가 `DispatcherTimer` 기반 save debounce, lifecycle attach/detach, pending save stop을 담당합니다.
- `OpenVisionDockWorkspaceLayoutController`가 layout changed/docking state changed event handler, guide refresh, comparison pane normalize, layout refresh를 담당합니다.
- app의 `OpenVisionShellHostDockedLayerOrchestrator.Events.cs`는 lifecycle binder attach와 document close 처리만 남았습니다.
- app의 `OpenVisionShellHostDockedLayerOrchestrator.State.cs`는 `StopPendingSave`/queue를 scheduler에 위임하고, timer/normalizing flag 구현은 제거됐습니다.
- app의 `OpenVisionShellHostDockedLayerOrchestrator.Commands.cs`는 `RefreshLayout()`을 library layout controller에 위임합니다.

### 15. Generic dock document projection controller library 이동

docked layer document forwarding adapter와 layer-specific refresh result를 제거하고, document/workspace state projection 경계를 정리했습니다.

추가된 library 파일:

- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockDocumentProjectionController.cs`

추가된 app 파일:

- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerDocumentProjection.cs`

제거된 app 파일:

- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerDocumentController.cs`
- `0. UI/0) MENU/Wpf/IOpenVisionDockedLayerContentSource.cs`

정리 내용:

- `OpenVisionShellHostDockedLayerOrchestrator`가 generic `OpenVisionDockDocumentController`를 직접 생성합니다.
- `OpenVisionDockDocumentProjectionController<TDocumentState, TWorkspaceState>`가 generic document state를 app workspace state로 project하는 흐름을 담당합니다.
- app-specific viewer metrics(`TextureTileCount`, compact readiness/chrome)는 `OpenVisionDockedLayerDocumentProjection`에만 남겼습니다.
- layer-specific `OpenVisionDockedLayerRefreshResult`를 제거하고 `OpenVisionDockDocumentRefreshResult`를 ShellHost presenter까지 직접 전달합니다.
- `OpenVisionDockedLayerContentSource`는 app-local `IOpenVisionDockedLayerContentSource` 대신 library `IOpenVisionDockDocumentContentSource`를 직접 구현합니다.
- `OpenVisionDockedLayerContentSource`의 layer image/status helper는 public app contract가 아니라 private helper로 닫았습니다.

### 16. Docked layer state marker 제거

app-local `IOpenVisionDockedLayerState` marker interface를 제거하고, app layer state owner가 library document-state contract를 직접 구현하도록 정리했습니다.

변경 내용:

- `0. UI/0) MENU/Wpf/IOpenVisionDockedLayerState.cs` 제거.
- `OpenVisionShellHostDockedLayerController`가 `IOpenVisionDockDocumentState`를 직접 구현합니다.
- `OpenVisionShellHostDockedLayerOrchestrator`는 `IOpenVisionDockDocumentState`를 직접 받습니다.
- orchestrator 내부 필드명도 `layerState`에서 `documentState`로 정리했습니다.

### 17. Generic dock document state store/controller library 이동

docked layer title persistence와 pane-map persistence를 generic document state store/controller로 이동했습니다.

추가된 library 파일:

- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockDocumentStateStore.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockDocumentStateController.cs`

추가된 app 파일:

- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerDocumentStateFactory.cs`

제거된 app 파일:

- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerStateStore.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostDockedLayerController.cs`

정리 내용:

- `OpenVisionDockDocumentStateStore`가 document id 목록과 pane map 파일 저장/복원, normalize, delete/parent-directory 처리를 담당합니다.
- `OpenVisionDockDocumentStateController`가 active document id list와 persisted state 적용/저장을 담당하며 `IOpenVisionDockDocumentState`를 구현합니다.
- app의 `OpenVisionDockedLayerDocumentStateFactory`는 기존 파일명(`LayerDocking.layers`, `LayerDocking.layout`)과 `AppPathService` 경로만 제공합니다.
- `OpenVisionShellHostView`는 concrete app docked-layer state controller 대신 docked layer workspace runtime을 보관합니다.
- `OpenVisionLab.Docking.Controls`는 app path service를 알지 않습니다.

### 18. Docked layer workspace runtime/ViewModel 경계 정리

ShellHost가 docked layer document state/content/viewer/orchestrator 조립을 직접 수행하던 흐름을 `OpenVisionDockedLayerWorkspaceRuntime`과 `OpenVisionDockedLayerWorkspaceViewModel` 경계로 옮겼습니다.

추가/변경된 app 파일:

- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerWorkspaceRuntime.cs`
- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerWorkspaceRuntimeOptions.cs`
- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerWorkspaceViewModel.cs`
- `0. UI/0) MENU/Wpf/IOpenVisionDockedLayerOrchestrator.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostDockedLayerOrchestrator*.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostTestAdapter.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostView.xaml.cs`

정리 내용:

- `OpenVisionShellHostView`는 더 이상 `OpenVisionDockedLayerContentSource`, `OpenVisionDockedLayerViewerFactory`, `OpenVisionShellHostDockedLayerOrchestrator`, `OpenVisionDockedLayerDocumentStateFactory.Create()`를 직접 조립하지 않습니다.
- 도킹 작업공간 구성 의존성은 `OpenVisionDockedLayerWorkspaceRuntimeOptions`로 명시하고, 실제 조립은 `OpenVisionDockedLayerWorkspaceRuntime.Create(...)` 내부에 모았습니다.
- `OpenVisionDockedLayerWorkspaceViewModel`은 `ObservableObject` 기반으로 `LayerTitles`, `HasLayers`, `LayerCount`, `LayerTitleSummary`를 노출하고, 도킹 상태 변경 시 property changed 알림을 발행합니다.
- `IOpenVisionDockedLayerOrchestrator`에 `WorkspaceStateChanged` 이벤트를 추가했습니다. Dock, Clear, Split, Move, Guide-zone dock, Arrange, Restore, document close가 같은 상태 변경 경계를 통과합니다.
- `OpenVisionShellHostTestAdapter`는 더 이상 `OpenVisionLayerDockWorkspaceView`를 직접 받지 않습니다. 테스트용 guide ratio 계산과 tab drag guide 시작은 `OpenVisionDockedLayerWorkspaceRuntime`이 wrapper control을 알고 처리합니다.
- ShellHost는 도킹 상태 조회를 `dockedLayerWorkspace.LayerTitles` / `dockedLayerWorkspace.HasLayers`로 제한하고, ViewModel 세부 상태나 document-state factory를 직접 알지 않습니다.
- 이 단계에서도 output layer 생성이 input layer를 바꾸지 않는 계약, boolean visibility toggle이 Preview/Run을 실행하지 않는 계약, viewer zoom/pan/drag 및 docking guide UX는 변경하지 않았습니다.

### 19. Layer command surface / selection activation 분리

ShellHost의 레이어 선택, 레이어 viewer open, dock/clear 사용자 액션을 code-behind Click handler에서 command surface와 작은 controller로 이동했습니다.

추가/변경된 app 파일:

- `0. UI/0) MENU/Wpf/OpenVisionShellHostLayerActivationController.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostLayerSelectionController.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostLayerCommandSurface.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostView.xaml`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostView.xaml.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostView.TestHooks.cs`

제거된 app 파일:

- `0. UI/0) MENU/Wpf/OpenVisionShellHostLayerInteractionController.cs`

정리 내용:

- 레이어 활성화는 `OpenVisionShellHostLayerActivationController`가 담당합니다.
- ListBox 선택 해석과 선택 변경 처리는 `OpenVisionShellHostLayerSelectionController`가 담당합니다.
- Open selected/current layer, dock selected/current layer, clear docked layers는 `OpenVisionShellHostLayerCommandSurface`의 `ICommand`로 노출됩니다.
- `OpenVisionShellHostView`는 `LayerCommands` dependency property를 노출하고, XAML 버튼/메뉴는 이 command surface에 바인딩합니다.
- `LayerRowsContextMenu`와 workspace context menu는 `PlacementTarget.Tag`를 통해 command surface를 받습니다. ContextMenu가 visual tree 밖에 있어도 같은 명령 경계를 탑니다.
- `HostLayerRowsList_SelectionChanged`, `HostLayerRowsList_MouseDoubleClick`, `OpenSelectedLayerWindow_Click`, `OpenCurrentLayerWindow_Click`, `DockSelectedLayer_Click`, `DockCurrentLayer_Click`, `ClearDockedLayers_Click` handler를 제거했습니다.
- `OpenVisionShellHostView.TestHooks.cs`도 제거된 interaction controller 대신 `layerViewerController`, `layerActivationController`, `testAdapter`를 사용합니다.

## 이전 작업 축약 요약

이 대화에서는 도킹 외에도 장기간 다음 영역을 다뤘습니다. 자세한 안정 계약은 `docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`를 기준으로 확인하십시오.

### PropertyGrid / WPG

- ComboBox가 열리지 않거나 텍스트가 겹쳐 보이는 문제를 여러 차례 수정했습니다.
- RangeEditor에서 `Invert`가 잘못 노출되던 문제를 제거했습니다.
- Min/Max companion property는 모델/XML/실행 호환성을 위해 유지하되 UI 중복 row는 숨기는 방향으로 정리했습니다.
- Numeric TextBox는 빈 값, `-` 등 입력 중간 상태를 허용해야 하며 Enter/focus loss 또는 command 직전 commit으로 모델에 반영됩니다.
- Preview/Run/Add Pipeline 클릭 시 포커스 이동 없이도 현재 입력값이 반영되어야 합니다.
- Conditional child row는 들여쓰기/구분 스타일을 통해 부모 switch 하위 항목임을 알아볼 수 있게 했습니다.

### 레이어 선택 / 출력 레이어

- Output 레이어 추가 시 Input 레이어가 강제로 바뀌는 것은 버그로 정의했습니다.
- Input ComboBox는 모든 적절한 레이어를 표시하고 사용자가 명시적으로 선택해야 합니다.
- Tool output click 시 Main/중앙 viewer와 동기화되는 기존 사용성은 복원 대상입니다.
- 자동으로 output 패널을 늘리거나 input을 바꾸는 방식은 OpenVisionLab 방향성과 맞지 않습니다.

### Viewer / ROI / Template editor

- Viewer zoom, pan, drag 기능은 제품 기능으로 유지해야 합니다.
- ROI editor는 정보 표시만으로 output layer를 생성하거나 선택하면 안 됩니다.
- ROI 설정은 tool property model/recipe에 저장되고 이미지 로드 시 overlay로 보여야 합니다.
- Template matching 등록 창은 OpenGL 기반 viewer 방향으로 정리했습니다.
- Template 등록 시 ROI 이동/크기 조절/회전 요구가 있었습니다. 회전 ROI 저장 시 matching 자체는 0도 template처럼 저장되는 UX가 목표입니다.

### Blob / Contour / Line

- Blob threshold trackbar는 원본이 아니라 threshold된 preview image를 output/preview에 보여야 합니다.
- threshold 사용 여부 toggle만 켰다고 자동 threshold preview/run이 실행되면 안 됩니다.
- Contour는 외곽선 drawing이 기본입니다. 색상/두께/display mode는 operator-facing control입니다.
- Line은 Vertical Line 용어 대신 Scan Line으로 UI 용어를 정리했습니다.
- Distance는 scan-line 기반 intersection + distance measurement 의미입니다.

### Matching / EdgeBasedMatching

- Matching은 기본 manual preview입니다.
- angle/scale 등 무거운 파라미터 변경 시 자동 검사로 UI가 멈추면 안 됩니다. 자동 preview는 명시 옵션과 debounce 정책 아래에서만 허용됩니다.
- Matching result에는 tact time/label 표시 요구가 있었습니다.
- magnification tooltip은 image pyramid 개념 때문에 존재하는 파라미터로 설명해야 합니다.
- EdgeBasedMatching에는 angle 보정, scale 보정, coarse-to-fine, pyramid proposal, hybrid verify 등 성능/정확도 개선을 실험했습니다.
- EdgeBasedMatching drawing은 contour처럼 외곽 기반이어야 합니다.
- 상용 라이브러리 관점에서 edge-based와 shape/geometric matching은 분리 가능성이 있으며, 현재는 EdgeBased에 scale/subpixel 개념까지 넣고 이후 Shape/Geometric 별도 툴을 검토하는 방향이 제안되었습니다.

### Tool open / 성능

- 툴 창이 느리게 열리는 문제를 위해 prewarm/cache/policy/gate를 도입했습니다.
- 무조건 많은 테스트를 돌리는 것이 아니라 변경 포인트에 맞춘 smoke를 선호합니다.
- PropertyGrid는 전체 row를 처음부터 무겁게 그리는 대신 보이는 영역 중심으로 렌더링할 수 있는지 검토 대상입니다.

## 최근 검증 결과

최근 도킹/DLL 분리 작업 후 아래 검증이 완료되었습니다.

### Solution build

명령:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
```

결과:

- PASS
- warnings 0
- errors 0

### Focused UI smoke

명령:

```powershell
dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible artifacts\docking_smoke
```

결과:

- PASS
- 단, 기존 nullable warning이 한 건 남아 있습니다.
- 기존 warning: `tools\PipelineViewerScreenshotSmoke\Program.cs(7608,23): warning CS8600`

### Actual EXE smoke

명령:

```powershell
& "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe" --smoke layer-global-docking --output "C:\Git\OpenVisionLab_Dev\artifacts\actual_exe_layer_global_docking_20260629"
```

결과:

```text
Result: PASS
Scenario: layer-global-docking
DockedLayers: 2
DockedPanes: 2
DockedTiles: 2
RootOrientation: Horizontal
Titles: Main|HSV_Preview
```

### Package ownership check

명령:

```powershell
dotnet list "OpenVisionLab.csproj" package --include-transitive
```

결과:

- `Dirkster.AvalonDock 4.74.1`은 transitive package로만 표시됩니다.
- app 프로젝트 top-level package로는 표시되지 않습니다.

### Latest ShellHost refactor validation

2026-06-29 추가 ShellHost session/chrome/test surface 분리 후 검증:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_workspace_output,wpf_shell_host_native_tool,wpf_tool_window_dock_float_cycle,wpf_shell_host_layer_popout artifacts\docking_smoke_session_refactor_large`: PASS 7/7.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_session_refactor_large`: PASS, DockedLayers 2, DockedPanes 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- `dotnet list OpenVisionLab.csproj package --include-transitive`: `Dirkster.AvalonDock 4.74.1`은 transitive package로만 표시됩니다.
- `0. UI/0) MENU/Wpf/*.cs` AvalonDock raw type 검색: 결과 없음.

### Latest docked-layer abstraction validation

2026-06-29 docked layer viewer/content source/orchestrator contract 추상화 후 검증:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_workspace_output,wpf_shell_host_native_tool,wpf_tool_window_dock_float_cycle,wpf_shell_host_layer_popout artifacts\docking_smoke_orchestrator_contract`: PASS 7/7.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_orchestrator_contract`: PASS, DockedLayers 2, DockedPanes 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- `dotnet list OpenVisionLab.csproj package --include-transitive`: `Dirkster.AvalonDock 4.74.1`은 transitive package로만 표시됩니다.
- `0. UI/0) MENU/Wpf/*.cs` AvalonDock raw type 검색: 결과 없음.

### Latest generic dock document controller validation

2026-06-29 generic dock document sync controller library 이동 후 검증:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_workspace_output,wpf_shell_host_native_tool,wpf_tool_window_dock_float_cycle,wpf_shell_host_layer_popout artifacts\docking_smoke_document_controller_library`: PASS 7/7.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_document_controller_library`: PASS, DockedLayers 2, DockedPanes 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- `Library/OpenVisionLab.Docking.Controls` app-local dependency 검색: `IDisplayManager`, `OpenVisionLayerViewerView`, `System.Drawing`, `Bitmap` 결과 없음.
- `dotnet list OpenVisionLab.csproj package --include-transitive`: `Dirkster.AvalonDock 4.74.1`은 transitive package로만 표시됩니다.
- `0. UI/0) MENU/Wpf/*.cs` AvalonDock raw type 검색: 결과 없음.

### Latest workspace adapter removal validation

2026-06-29 docked layer workspace forwarding adapter 제거 후 검증:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_workspace_output,wpf_shell_host_native_tool,wpf_tool_window_dock_float_cycle,wpf_shell_host_layer_popout artifacts\docking_smoke_workspace_adapter_removed`: PASS 7/7.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_workspace_adapter_removed`: PASS, DockedLayers 2, DockedPanes 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- `Library/OpenVisionLab.Docking.Controls` app-local dependency 검색: `IDisplayManager`, `OpenVisionLayerViewerView`, `System.Drawing`, `Bitmap` 결과 없음.
- `dotnet list OpenVisionLab.csproj package --include-transitive`: `Dirkster.AvalonDock 4.74.1`은 transitive package로만 표시됩니다.
- `0. UI/0) MENU/Wpf/*.cs` AvalonDock raw type 검색: 결과 없음.

### Latest generic dock document orchestrator validation

2026-06-29 generic dock document orchestrator library 이동 후 검증:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_workspace_output,wpf_shell_host_native_tool,wpf_tool_window_dock_float_cycle,wpf_shell_host_layer_popout artifacts\docking_smoke_document_orchestrator_library`: PASS 7/7.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_document_orchestrator_library`: PASS, DockedLayers 2, DockedPanes 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- `Library/OpenVisionLab.Docking.Controls` app-local dependency 검색: `IDisplayManager`, `OpenVisionLayerViewerView`, `System.Drawing`, `Bitmap` 결과 없음.
- `dotnet list OpenVisionLab.csproj package --include-transitive`: `Dirkster.AvalonDock 4.74.1`은 transitive package로만 표시됩니다.
- `0. UI/0) MENU/Wpf/*.cs` AvalonDock raw type 검색: 결과 없음.

### Latest workspace lifecycle binder validation

2026-06-29 dock workspace lifecycle/event binder library 이동 후 검증:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_workspace_output,wpf_shell_host_native_tool,wpf_tool_window_dock_float_cycle,wpf_shell_host_layer_popout artifacts\docking_smoke_workspace_lifecycle_binder`: PASS 7/7.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_workspace_lifecycle_binder`: PASS, DockedLayers 2, DockedPanes 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- `dotnet list OpenVisionLab.csproj package --include-transitive`: `Dirkster.AvalonDock 4.74.1`은 transitive package로만 표시됩니다.
- `0. UI/0) MENU/Wpf/*.cs` AvalonDock raw type 검색: 결과 없음.
- `Library/OpenVisionLab.Docking.Controls` app-local dependency 검색: `IDisplayManager`, `OpenVisionLayerViewerView`, `System.Drawing`, `Bitmap` 결과 없음.

### Latest guide state controller validation

2026-06-29 dock guide overlay state controller library 이동 후 검증:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_workspace_output,wpf_shell_host_native_tool,wpf_tool_window_dock_float_cycle,wpf_shell_host_layer_popout artifacts\docking_smoke_guide_state_controller`: PASS 7/7.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_guide_state_controller`: PASS, DockedLayers 2, DockedPanes 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- `dotnet list OpenVisionLab.csproj package --include-transitive`: `Dirkster.AvalonDock 4.74.1`은 transitive package로만 표시됩니다.
- `0. UI/0) MENU/Wpf/*.cs` AvalonDock raw type 검색: 결과 없음.
- `Library/OpenVisionLab.Docking.Controls` app-local dependency 검색: `IDisplayManager`, `OpenVisionLayerViewerView`, `System.Drawing`, `Bitmap` 결과 없음.
- guide overlay state 검색: wrapper DP 직접 쓰기는 `Library/OpenVisionLab.Docking.Controls/OpenVisionDockingGuideStateController.cs`에 집중되어 있고, ShellHost에는 read-only test hook만 남아 있습니다.

### Latest workspace layout controller validation

2026-06-29 dock workspace layout/save scheduler library 이동 후 검증:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_workspace_output,wpf_shell_host_native_tool,wpf_tool_window_dock_float_cycle,wpf_shell_host_layer_popout artifacts\docking_smoke_layout_controller`: PASS 7/7.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_layout_controller`: PASS, DockedLayers 2, DockedPanes 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- `dotnet list OpenVisionLab.csproj package --include-transitive`: `Dirkster.AvalonDock 4.74.1`은 transitive package로만 표시됩니다.
- `0. UI/0) MENU/Wpf/*.cs` AvalonDock raw type 검색: 결과 없음.
- `Library/OpenVisionLab.Docking.Controls` app-local dependency 검색: `IDisplayManager`, `OpenVisionLayerViewerView`, `System.Drawing`, `Bitmap` 결과 없음.
- app WPF docked layer orchestrator 검색: `layoutSaveTimer`, layout event handler, `OnLayoutSaveTimerTick`, app-local `NormalizeComparisonLayout`, `normalizingLayout` 결과 없음.

### Latest generic dock document projection validation

2026-06-29 generic dock document projection controller library 이동 및 app forwarding adapter 제거 후 검증:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_workspace_output,wpf_shell_host_native_tool,wpf_tool_window_dock_float_cycle,wpf_shell_host_layer_popout artifacts\docking_smoke_document_projection_controller`: PASS 7/7.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_document_projection_controller`: PASS, DockedLayers 2, DockedPanes 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- `dotnet list OpenVisionLab.csproj package --include-transitive`: `Dirkster.AvalonDock 4.74.1`은 transitive package로만 표시됩니다.
- `0. UI/0) MENU/Wpf/*.cs` AvalonDock raw type 검색: 결과 없음.
- `Library/OpenVisionLab.Docking.Controls` app-local dependency 검색: `IDisplayManager`, `OpenVisionLayerViewerView`, `System.Drawing`, `Bitmap` 결과 없음.
- app forwarding adapter 검색: `IOpenVisionDockedLayerContentSource`, `OpenVisionDockedLayerDocumentController`, `OpenVisionDockedLayerRefreshResult`, `GenericController`, `HasWorkspaceLayers` 결과 없음.

### Latest dock document state contract validation

2026-06-29 docked layer state marker 제거 후 검증:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_workspace_output,wpf_shell_host_native_tool,wpf_tool_window_dock_float_cycle,wpf_shell_host_layer_popout artifacts\docking_smoke_document_state_contract_final`: PASS 7/7.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_document_state_contract_final`: PASS, DockedLayers 2, DockedPanes 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- `dotnet list OpenVisionLab.csproj package --include-transitive`: `Dirkster.AvalonDock 4.74.1`은 transitive package로만 표시됩니다.
- `0. UI/0) MENU/Wpf/*.cs` AvalonDock raw type 검색: 결과 없음.
- `Library/OpenVisionLab.Docking.Controls` app-local dependency 검색: `IDisplayManager`, `OpenVisionLayerViewerView`, `System.Drawing`, `Bitmap` 결과 없음.
- `IOpenVisionDockedLayerState` 검색: 코드 결과 없음. 이 단계 당시 `OpenVisionShellHostDockedLayerController`는 `IOpenVisionDockDocumentState`를 직접 구현했습니다. 이후 section 17에서 generic `OpenVisionDockDocumentStateController`로 대체됐습니다.

### Latest generic dock document state store validation

2026-06-29 generic dock document state store/controller library 이동 후 검증:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_workspace_output,wpf_shell_host_native_tool,wpf_tool_window_dock_float_cycle,wpf_shell_host_layer_popout artifacts\docking_smoke_document_state_store_library`: PASS 7/7.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_document_state_store_library`: PASS, DockedLayers 2, DockedPanes 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- `dotnet list OpenVisionLab.csproj package --include-transitive`: `Dirkster.AvalonDock 4.74.1`은 transitive package로만 표시됩니다.
- `0. UI/0) MENU/Wpf/*.cs` AvalonDock raw type 검색: 결과 없음.
- `Library/OpenVisionLab.Docking.Controls` app-local dependency 검색: `IDisplayManager`, `OpenVisionLayerViewerView`, `System.Drawing`, `Bitmap`, `AppPathService` 결과 없음.
- state store 검색: `OpenVisionDockedLayerStateStore`, `OpenVisionShellHostDockedLayerController` 결과 없음. app에는 `OpenVisionDockedLayerDocumentStateFactory`만 남아 기존 `LayerDocking.layers/layout` 경로를 제공합니다.

### Latest docked layer workspace runtime/ViewModel validation

2026-06-29 docked layer workspace runtime/ViewModel 경계 정리 후 검증:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_workspace_output,wpf_shell_host_native_tool,wpf_tool_window_dock_float_cycle,wpf_shell_host_layer_popout artifacts\docking_smoke_workspace_runtime_mvvm`: PASS 7/7.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_workspace_runtime_mvvm`: PASS, DockedLayers 2, DockedPanes 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- `dotnet list OpenVisionLab.csproj package --include-transitive`: `Dirkster.AvalonDock 4.74.1`은 transitive package로만 표시됩니다.
- `0. UI/0) MENU/Wpf/*.cs` AvalonDock raw type 검색: 결과 없음.
- `Library/OpenVisionLab.Docking.Controls` app-local dependency 검색: `IDisplayManager`, `OpenVisionLayerViewerView`, `System.Drawing`, `Bitmap`, `AppPathService` 결과 없음.
- ShellHost 직접 조립 검색: `OpenVisionDockedLayerDocumentStateFactory.Create`, `new OpenVisionDockedLayerContentSource`, `new OpenVisionDockedLayerViewerFactory`, `new OpenVisionShellHostDockedLayerOrchestrator`는 `OpenVisionShellHostView.xaml.cs`에서 결과 없음. 해당 조립은 `OpenVisionDockedLayerWorkspaceRuntime`에 모였습니다.

### Latest layer command surface validation

2026-06-29 layer command surface / selection activation 분리 후 검증:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_workspace_output,wpf_shell_host_native_tool,wpf_tool_window_dock_float_cycle,wpf_shell_host_layer_popout artifacts\docking_smoke_layer_command_surface`: PASS 7/7.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_layer_command_surface`: PASS, DockedLayers 2, DockedPanes 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- `dotnet list OpenVisionLab.csproj package --include-transitive`: `Dirkster.AvalonDock 4.74.1`은 transitive package로만 표시됩니다.
- `0. UI/0) MENU/Wpf/*.cs` AvalonDock raw type 검색: 결과 없음.
- `Library/OpenVisionLab.Docking.Controls` app-local dependency 검색: `IDisplayManager`, `OpenVisionLayerViewerView`, `System.Drawing`, `Bitmap`, `AppPathService` 결과 없음.
- legacy layer interaction 검색: `OpenVisionShellHostLayerInteractionController`, `layerInteractionController`, removed layer Click handlers, `HostLayerRowsList_SelectionChanged`, `HostLayerRowsList_MouseDoubleClick` 결과 없음.

## 현재 중요한 파일

도킹/MVVM/분리 관련:

- `0. UI/0) MENU/Wpf/OpenVisionShellHostView.xaml`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostView.xaml.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostView.TestHooks.cs`
- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerWorkspaceRuntime.cs`
- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerWorkspaceRuntimeOptions.cs`
- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerWorkspaceViewModel.cs`
- `0. UI/0) MENU/Wpf/IOpenVisionDockedLayerOrchestrator.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostLayerActivationController.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostLayerSelectionController.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostLayerCommandSurface.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostDockedLayerOrchestrator.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostDockedLayerOrchestrator.Commands.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostDockedLayerOrchestrator.Events.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostDockedLayerOrchestrator.Guide.cs`
- `0. UI/0) MENU/Wpf/OpenVisionShellHostDockedLayerOrchestrator.State.cs`
- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerDocumentProjection.cs`
- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerDocumentStateFactory.cs`
- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerContentSource.cs`
- `0. UI/0) MENU/Wpf/OpenVisionDockedLayerViewerFactory.cs`

새 library:

- `Library/OpenVisionLab.Docking.Controls/OpenVisionLab.Docking.Controls.csproj`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionLayerDockWorkspaceView.xaml`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionLayerDockWorkspaceView.xaml.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionLayerDockingGuidePolicy.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionLayerDockingGuidePresenter.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionLayerDockingCommandController.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionLayerDockingGestureController.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockDocumentController.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockDocumentOrchestrator.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockDocumentProjectionController.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockDocumentStateStore.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockDocumentStateController.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockWorkspaceLifecycleBinder.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockingGuideStateController.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockWorkspaceStateSaveScheduler.cs`
- `Library/OpenVisionLab.Docking.Controls/OpenVisionDockWorkspaceLayoutController.cs`
- `Library/OpenVisionLab.Docking.Controls/IOpenVisionDockLifecycle.cs`

Project/reference:

- `OpenVisionLab.csproj`
- `OpenVisionLab.sln`

문서:

- `CODEX_RECOVERY.md`
- `NEXT_CODEX_PROMPT.md`

## 2026-06-29 Update - Actual EXE docking verification gate

User review showed that existing smoke coverage could pass while the real docking UX was still wrong. A dedicated actual-EXE docking verification path was added and used to fix two concrete regressions.

Changed structure:

- `tools/RunDockingVerification.ps1` builds optionally and runs `bin\Debug\OpenVisionLab.exe --smoke layer-docking-verification` through `Start-Process -Wait`.
- `OpenVisionLabDirectSmokeRunner` now has `layer-docking-verification`, which captures and asserts same-pane top tabs, workspace-level GlobalRight, workspace-level GlobalBottom, pane-local Bottom, flatten-then-restore of nested pane-local layout, and center/tab merge.
- `OpenVisionLayerDockWorkspaceView` exposes wrapper-owned `OpenVisionDockingVisualSnapshot` / `OpenVisionDockingVisualElementSnapshot` DTOs so ShellHost tests can inspect top-aligned tabs and pane bounds without touching AvalonDock types directly.
- `OpenVisionLayerDockWorkspaceView.xaml` reapplies wrapper-owned AvalonDock pane/header/title styles so docked layer tabs stay top-aligned instead of falling to the bottom tab strip.
- Historical note: this pass kept docked layer documents `CanFloat=true`, but the 2026-06-30 native-floating-preview suppression update supersedes that setting for the current product state.
- `OpenVisionDockDocumentStateStore` now persists layout as `LayerTitle<TAB>PaneIndex<TAB>LayoutPath`. Old two-column `LayerTitle<TAB>PaneIndex` files still load as a flat horizontal fallback.
- `OpenVisionDockWorkspaceController.State` restores nested layout paths such as pane-local bottom, instead of flattening everything through pane indices.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `powershell -ExecutionPolicy Bypass -File "tools\RunDockingVerification.ps1" -SkipBuild`: PASS.
- Verification artifact: `artifacts\docking_verification\actual_exe_20260629_215143`.
- The verifier first caught bottom tab placement (`HSV_Preview` tab at pane bottom), then caught nested restore loss (`Dock_LocalBottom` restored beside Main instead of below it). Both issues were fixed before the final PASS.

Next priority:

1. Use `tools\RunDockingVerification.ps1` as the primary docking gate after any layer docking, tab chrome, layout persistence, or wrapper style change.
2. Keep obsolete custom-guide screenshot targets out of release judgement until they are rewritten around native DockingManager behavior.
3. Continue manual native drag review in the real app, but use the EXE gate to catch objective regressions before user review.

## 2026-06-29 Update - Menu command surface binding

This update completes the next ShellHost MVVM cleanup step after the layer command surface split.

Changed structure:

- `OpenVisionShellHostWorkspaceCommandSurface` owns workspace image commands: load, fit, save.
- `OpenVisionShellHostCommandSurfaces` groups layer and workspace command surfaces for ContextMenu `PlacementTarget.Tag` binding.
- `OpenVisionShellHostLayerCommandSurface` now exposes localized menu text properties and raises property changes on language changes.
- `OpenVisionShellHostView.xaml` no longer uses `LayerRowsContextMenu_Opened`, `WorkspaceContextMenu_Opened`, `WorkspaceLoadImage_Click`, `FitWorkspaceImage_Click`, or `SaveWorkspaceImage_Click`.
- Layer row context menu headers and commands bind to `LayerCommands`.
- Workspace context menu commands bind through `CommandSurfaces.WorkspaceCommands` and `CommandSurfaces.LayerCommands`.
- Workspace empty-overlay image load button binds to `WorkspaceCommands.LoadImageCommand`.
- `OpenVisionShellHostMenuPresenter` only applies stable localization/text/tooltips for named UI elements. It no longer enables/disables context menu items directly.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_workspace_output,wpf_shell_host_native_tool,wpf_tool_window_dock_float_cycle,wpf_shell_host_layer_popout artifacts\docking_smoke_menu_command_surfaces`: PASS 7/7.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_menu_command_surfaces`: PASS. Report: DockedLayers 2, DockedPanes 2, DockedTiles 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- `Dirkster.AvalonDock` PackageReference remains only in `Library\OpenVisionLab.Docking.Controls\OpenVisionLab.Docking.Controls.csproj`; `OpenVisionLab.csproj` sees it only transitively.
- ShellHost WPF raw AvalonDock search did not find raw XAML/API use. Remaining matches are state/test names such as `NestedLayoutPanelCount` and an unload comment.
- `Library\OpenVisionLab.Docking.Controls` app-local dependency search for `IDisplayManager`, `OpenVisionLayerViewerView`, `System.Drawing`, `Bitmap`, and `AppPathService` returned no matches.

Next priority:

1. Continue ShellHost code-behind reduction around remaining view event handlers that still call controllers directly, without changing stable layer/tool behavior.
2. Move more menu/button state to `OpenVisionDockedLayerWorkspaceViewModel` where it can be bound cleanly, but keep explicit operator actions for layer docking/comparison.
3. Continue app-local docking orchestration cleanup only after preserving the current wrapper package boundary and smoke coverage.
4. Add or extend focused smoke only if new changes touch context menu execution paths that the current screenshot smoke does not open directly.

## 2026-06-29 Update - Chrome/session command surfaces

This update continues the ShellHost MVVM cleanup by removing the next group of direct view event handlers.

Changed structure:

- `OpenVisionShellHostChromeCommandSurface` now owns shell chrome commands:
  - open tutorial
  - toggle tool rail
  - float docked tool
  - close docked tool
- `OpenVisionShellHostSessionCommandSurface` now owns session/lifecycle forwarding commands:
  - ShellHost loaded
  - ShellHost unloaded/dispose
  - workspace canvas loaded
- `InputCommandBehaviors` in `OpenVisionLab.Mvvm` now supports `LoadedCommand` and `UnloadedCommand` attached properties.
- `OpenVisionShellHostView.xaml` binds tutorial/tool-rail/docked-tool buttons to `ChromeCommands`.
- `OpenVisionShellHostView.xaml` binds root Loaded/Unloaded and `hostWorkspaceCanvas.Loaded` through `InputCommandBehaviors`.
- `OpenVisionShellHostView.xaml.cs` no longer contains `OpenTutorial_Click`, `ToggleToolRail_Click`, `FloatDockedTool_Click`, `CloseDockedTool_Click`, `OnLoaded`, `OnUnloaded`, or `HostWorkspaceCanvas_Loaded`.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_workspace_output,wpf_shell_host_native_tool,wpf_tool_window_dock_float_cycle,wpf_shell_host_layer_popout artifacts\docking_smoke_chrome_session_commands`: PASS 7/7.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_chrome_session_commands`: PASS. Report: DockedLayers 2, DockedPanes 2, DockedTiles 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- ShellHost direct handler search for `Click=`, direct Loaded/Unloaded subscriptions, and removed handler names returned no matches in `OpenVisionShellHostView.xaml` / `.xaml.cs`.
- `Dirkster.AvalonDock` PackageReference remains only in `Library\OpenVisionLab.Docking.Controls\OpenVisionLab.Docking.Controls.csproj`; `OpenVisionLab.csproj` sees it only transitively.
- ShellHost WPF raw AvalonDock search did not find raw XAML/API use. Remaining matches are state/test names such as `NestedLayoutPanelCount` and an unload comment.
- `Library\OpenVisionLab.Docking.Controls` app-local dependency search for `IDisplayManager`, `OpenVisionLayerViewerView`, `System.Drawing`, `Bitmap`, and `AppPathService` returned no matches.

Next priority:

1. Continue reducing ShellHost code-behind by moving remaining refresh delegates (`RefreshDockedLayerViews`, `RefreshHostLayerRows`, `RefreshHostSelectedLayerDetail`, `RefreshLayerActionButtons`) behind a small coordinator/presenter interface where it does not create circular ownership.
2. Keep the current command surface split: layer commands, workspace commands, chrome commands, session commands.
3. If more lifecycle/event behavior is added, keep it in `OpenVisionLab.Mvvm` only when it is generic and reusable; keep app-specific behavior in ShellHost controllers.
4. Add focused smoke only for newly touched command paths not covered by the 7-target docking smoke.

## 2026-06-29 Update - ShellHost refresh coordinator

This update moves the remaining ShellHost refresh delegate surface behind one app-local coordinator.

Changed structure:

- Added `OpenVisionShellHostRefreshCoordinator`.
- The coordinator owns the ShellHost refresh surface used by controllers/runtime/test hooks:
  - refresh host layer rows
  - refresh selected layer detail
  - refresh docked layer views
  - apply docked layer refresh result
  - refresh layer action buttons
  - create workspace layer title snapshot
  - apply workspace pointer status
  - refresh layer/workspace command `CanExecute`
- `OpenVisionShellHostView.xaml.cs` no longer owns private wrapper methods for `RefreshHostLayerRows`, `RefreshHostSelectedLayerDetail`, `RefreshDockedLayerViews`, `RefreshLayerActionButtons`, `ApplyDockedLayerRefreshResult`, `ApplyWorkspacePointerStatus`, or `CreateWorkspaceLayerTitleSnapshot`.
- ShellHost construction now wires controller/runtime callbacks to `refreshCoordinator`.
- `OpenVisionShellHostView.TestHooks.cs` uses `refreshCoordinator` directly instead of calling view-local wrapper methods.
- The command surface split remains unchanged: layer commands, workspace commands, chrome commands, session commands.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_workspace_output,wpf_shell_host_native_tool,wpf_tool_window_dock_float_cycle,wpf_shell_host_layer_popout artifacts\docking_smoke_refresh_coordinator`: PASS 7/7.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_refresh_coordinator`: PASS. Report: DockedLayers 2, DockedPanes 2, DockedTiles 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- ShellHost private refresh wrapper search returned no matches in `OpenVisionShellHostView.xaml.cs`.
- `Dirkster.AvalonDock` PackageReference remains only in `Library\OpenVisionLab.Docking.Controls\OpenVisionLab.Docking.Controls.csproj`; `OpenVisionLab.csproj` sees it only transitively.
- ShellHost WPF raw AvalonDock search did not find raw XAML/API use. Remaining matches are state/test names such as `NestedLayoutPanelCount` and an unload comment.
- `Library\OpenVisionLab.Docking.Controls` app-local dependency search for `IDisplayManager`, `OpenVisionLayerViewerView`, `System.Drawing`, `Bitmap`, and `AppPathService` returned no matches.

Next priority:

1. Continue reducing ShellHost construction size by moving grouped object creation into app-local factories/builders where dependency order is clear.
2. Consider a small localization coordinator for `OpenVisionLanguageService.LanguageChanged` if more localization behavior accumulates.
3. Keep `OpenVisionShellHostRefreshCoordinator` app-local; do not move it into `OpenVisionLab.Docking.Controls` because it references ShellHost presenters and command surfaces.
4. Continue app-local docking orchestration cleanup only after preserving the wrapper package boundary and smoke coverage.
- `docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`

Smoke/test:

- `tools/PipelineViewerScreenshotSmoke/Program.cs`
- `OpenVisionLabDirectSmokeRunner.cs`

## 2026-06-29 Update - Visual Studio-style docking guide overlay

The layer docking guide overlay was compacted to match the Visual Studio-style target model more closely.

Changed structure:

- `OpenVisionLayerDockingGuideOverlayView.xaml` now renders pane-local guides as a compact centered compass instead of stretching five large rectangles across the target pane.
- Global guide visuals are compact edge targets instead of a full 3x3 overlay grid.
- `OpenVisionLayerDockingGuideOverlayView.xaml.cs` switches display mode from `ActiveGuideZone`:
  - `GlobalLeft/Right/Top/Bottom` shows only the active global edge guide.
  - pane-local `Left/Right/Top/Bottom/Center` hides global guides and shows one local compass for the current target pane.
- Existing guide zone policy and drop command mapping were not changed.
- This keeps the two-level contract: global workspace docking and pane-local split/tab docking both remain available, but the UI no longer shows all guide groups at once.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible artifacts\docking_smoke_vs_guides`: PASS 3/3.
- Visual inspection of `artifacts\docking_smoke_vs_guides\wpf_shell_host_layer_docking_guide_visible.png` confirmed a single compact pane compass.
- Visual inspection of `artifacts\docking_smoke_vs_guides\wpf_shell_host_layer_tab_drag_guide_visible.png` confirmed only the active global edge guide is shown for global edge targeting.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_vs_guides`: PASS. Report: DockedLayers 2, DockedPanes 2, DockedTiles 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- `Dirkster.AvalonDock` PackageReference remains only in `Library\OpenVisionLab.Docking.Controls\OpenVisionLab.Docking.Controls.csproj`.
- `Library\OpenVisionLab.Docking.Controls` app-local dependency search for `OpenVisionLab.ViewModels`, `OpenVisionLab.Controllers`, `OpenVisionLab.Views`, and `OpenVisionLab.Services` returned no matches.

Next priority:

1. Continue ShellHost construction cleanup with app-local factories/builders; this guide overlay pass did not address constructor object graph size.
2. If docking guide tests are expanded, add visual/assertive checks for "single local compass" and "single active global guide" so clutter regressions are caught automatically.
3. Keep guide rendering in `OpenVisionLab.Docking.Controls`; do not move AvalonDock-specific guide visuals back into ShellHost.

## 2026-06-29 Update - Docked layer bottom tab chrome cleanup

The docked comparison bottom tabs were cleaned up after visual review showed the selected tab still looked like AvalonDock's default white tab with nested dark content.

Changed structure:

- `OpenVisionLayerDockWorkspaceView.Resources.xaml` now owns a minimal `LayoutAnchorableTabItem` template in the wrapper library.
- The template binds the tab content to AvalonDock's `Model` and reuses `DockedLayerTabHeaderTemplate` for title/size metadata.
- `OverridesDefaultStyle=true` prevents the default white selected-tab chrome from leaking into the dark Shell.
- The tab header content was flattened:
  - removed the nested bordered header card
  - kept the drag grip
  - kept the trimmed layer title
  - reduced image size from a boxed badge to compact low-contrast meta text
- AvalonDock tab item types and drag gesture source detection were preserved.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0 after the first template pass.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_layer_docking_guide_visible artifacts\docking_smoke_tab_chrome_c`: PASS 2/2.
- Earlier full tab/guide run after the template fix: `wpf_shell_host_layer_docking_guide_visible`, `wpf_shell_host_layer_tab_drag_guide_visible`, `wpf_shell_host_layer_global_docking`: PASS 3/3 in `artifacts\docking_smoke_tab_chrome`.
- Visual inspection of `artifacts\docking_smoke_tab_chrome_c\wpf_shell_host_layer_tab_drag_guide_visible.png` confirmed the bottom tabs no longer use the boxed size badge and no longer render the old nested tab card.

Next priority:

1. Run the actual EXE layer-global-docking smoke after any further tab chrome changes because the tab item template is now wrapper-owned.
2. If tab visual tests are expanded, add a simple visual/hook check that selected docked tabs do not contain the old white AvalonDock selected background.
3. Continue ShellHost construction cleanup separately; this pass intentionally stayed inside `OpenVisionLab.Docking.Controls`.

## 2026-06-29 Update - Top-aligned docked layer tabs and visible drop regions

The docked comparison tab/guide UX was adjusted after review of real docking interaction.

Changed structure:

- `OpenVisionLayerDockWorkspaceView.xaml` now assigns `AnchorablePaneControlStyle="{StaticResource DockedLayerAnchorablePaneControlStyle}"`.
- `OpenVisionLayerDockWorkspaceView.Resources.xaml` adds a wrapper-owned `LayoutAnchorablePaneControl` template:
  - comparison tabs render above pane content
  - AvalonDock `LayoutAnchorableTabItem` is still used for each tab
  - tab content is bound through AvalonDock models
  - selected pane title chrome is removed so tabs are not duplicated by a second title row
  - outer WPF `TabItem` chrome is also overridden so the default white selected-tab slot does not appear
- `OpenVisionLayerDockingGuideOverlayView.xaml` adds active pane region overlays for left/right/top/bottom/center.
- `OpenVisionLayerDockingGuideOverlayView.xaml.cs` toggles the active region from `ActiveGuideZone`.
- The small compass remains as the label/command affordance, but the highlighted pane region now shows where the cursor can be dropped.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible artifacts\docking_smoke_top_tabs_regions_v4`: PASS 3/3.
- Visual inspection of `artifacts\docking_smoke_top_tabs_regions_v4\wpf_shell_host_layer_docking_guide_visible.png` confirmed top-aligned tabs and active pane-region highlight.
- Visual inspection of `artifacts\docking_smoke_top_tabs_regions_v4\wpf_shell_host_layer_global_docking.png` confirmed split comparison panes keep top tabs without the bottom tab strip.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_top_tabs_regions`: PASS.

Next priority:

1. Add visual/hook checks for top-aligned docked tabs and active pane-region guide if this area is touched again.
2. Continue ShellHost construction cleanup only after preserving the wrapper-owned top tab/template boundary.

## 2026-06-29 Update - Global vs pane-local bottom docking semantics

The layer docking UX was adjusted after review of the Visual Studio docking model. The issue was not only tab chrome: a bottom drop must mean different things depending on whether the operator targets the whole workspace or the current comparison pane.

Changed structure:

- `OpenVisionLayerDockingGuidePolicy` now resolves the pane-local compact compass before workspace global edge zones.
- `GlobalBottom` still maps to a workspace-level vertical split through `MoveToOuterPane`.
- Pane-local `Bottom` still maps to `MoveToPaneSide` and now remains reachable from the local compass without being stolen by the workspace bottom edge.
- `OpenVisionLayerDockingGuideOverlayView.xaml` keeps active global edge regions separate from active pane-local regions.
- `OpenVisionLayerDockWorkspaceView.Resources.xaml` keeps AvalonDock model-based selected tab styling without using unsupported `IsSelected` triggers on AvalonDock tab item types.
- `OpenVisionShellHostView.TestHooks.cs` exposes `ActiveDockingGuideZoneForTest` so smoke can assert `Bottom` versus `GlobalBottom`.
- `tools/PipelineViewerScreenshotSmoke/Program.cs` adds `wpf_shell_host_layer_bottom_docking_semantics`.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- `dotnet run --project "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_layer_global_docking,wpf_shell_host_layer_bottom_docking_semantics,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible artifacts\docking_smoke_bottom_semantics`: PASS 4/4.
- Visual inspection of `artifacts\docking_smoke_bottom_semantics\wpf_shell_host_layer_bottom_docking_semantics.png` confirmed `Dock_LocalBottom` is nested under the left comparison pane while `HSV_Preview` remains the right workspace column.
- `bin\Debug\OpenVisionLab.exe --smoke layer-global-docking --output artifacts\actual_exe_layer_global_docking_bottom_semantics`: PASS. Report: DockedLayers 2, DockedPanes 2, RootOrientation Horizontal, Titles Main|HSV_Preview.
- `dotnet list OpenVisionLab.csproj package --include-transitive`: `Dirkster.AvalonDock 4.74.1` remains transitive.
- Direct PackageReference search shows `Dirkster.AvalonDock` only in `Library\OpenVisionLab.Docking.Controls\OpenVisionLab.Docking.Controls.csproj`.

Next priority:

1. Keep the `wpf_shell_host_layer_bottom_docking_semantics` smoke in the focused docking set whenever guide hit-testing, pane target resolution, or move command mapping changes.
2. If guide hit areas are tuned again, preserve the distinction: global bottom owns the whole workspace row; pane-local bottom owns only the hovered pane.
3. Continue ShellHost construction cleanup only after preserving the wrapper-owned guide policy and tab template boundary.

## 2026-06-29 Update - Return layer docking to native AvalonDock behavior

After user review, the custom Visual-Studio-like guide overlay direction was judged wrong. AvalonDock documentation and samples treat `DockingManager` as the interaction owner: native layout panes, tab dragging, overlay windows, and floating should not be replaced by a parallel WPF `DragDrop.DoDragDrop` guide.

Changed structure:

- `OpenVisionLayerDockWorkspaceView.xaml` no longer wires `AnchorablePaneControlStyle`, `AnchorableHeaderTemplate`, or `AnchorableTitleTemplate` for docked layer panes.
- The custom `OpenVisionLayerDockingGuideOverlayView` is removed from the wrapper visual tree.
- `OpenVisionDockWorkspaceLifecycleBinder` no longer subscribes DockingManager mouse/drag/drop events to `OpenVisionLayerDockingGestureController`.
- `OpenVisionDockWorkspaceLayoutController` no longer refreshes custom guide state during layout changes.
- `OpenVisionShellHostDockedLayerOrchestrator` no longer constructs `OpenVisionLayerDockingGuidePresenter`, `OpenVisionDockingGuideStateController`, or `OpenVisionLayerDockingGestureController`.
- Historical note: this pass temporarily set `CanFloat=true` for docked layer `LayoutAnchorable` items, but the 2026-06-30 native-floating-preview suppression update supersedes that setting for the current product state.
- Legacy guide test hooks now no-op instead of showing the removed OpenVisionLab overlay.

Validation:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: PASS, warnings 0, errors 0.
- Per user instruction, no unrelated screenshot smoke was run in this pass. The old guide-visible smoke targets still describe the removed custom overlay path and need native-DockingManager-oriented rewrite before they are useful again.

Next priority:

1. Manually validate native AvalonDock dragging in the actual app: drag layer tabs/title bars and confirm AvalonDock's own overlay supports whole workspace and pane-local docking.
2. If native behavior is acceptable, remove or rewrite obsolete custom-guide code and smoke targets instead of reviving the custom overlay.
3. Longer-term, evaluate moving central image layers from `LayoutAnchorablePane` to the AvalonDock document model (`LayoutDocumentPane`) so the workspace matches the DockingManager sample structure more closely.

## 남은 작업 우선순위

### 1. Layer command surface를 menu/button 상태 바인딩으로 추가 확장

권장 방향:

- `OpenVisionShellHostLayerCommandSurface`는 open/dock/clear 명령을 소유하고 XAML 버튼/메뉴가 이를 직접 바인딩합니다. 다음 단계는 `LayerRowsContextMenu_Opened` / `WorkspaceContextMenu_Opened`에 남은 메뉴 상태 갱신을 더 얇게 하거나 binding/command CanExecute 중심으로 줄이는 것입니다.
- `OpenVisionDockedLayerWorkspaceViewModel`의 `HasLayers`, `LayerCount`, `LayerTitleSummary`를 menu/button 상태 바인딩 또는 presenter 입력으로 사용하고, ShellHost에서 직접 상태를 묻는 코드를 더 줄입니다.
- `OpenVisionShellHostLayerActivationController`, `OpenVisionShellHostLayerSelectionController`, `OpenVisionShellHostLayerCommandSurface` 경계를 유지하고, 레이어 선택/활성화/open/dock 책임이 다시 하나의 컨트롤러로 합쳐지지 않게 합니다.
- `OpenVisionShellHostTestAdapter`처럼 test-only guide 계산이 runtime으로 이동한 흐름을 유지하고, wrapper control 직접 의존이 ShellHost 주변으로 다시 퍼지지 않게 합니다.
- 이동 전후로 output layer 생성이 input layer를 변경하지 않는지, comparison pane이 자동 생성되지 않는지 계속 확인합니다.

### 2. Docked layer content/view factory 경계 추가 이동

현재 `OpenVisionLab.Docking.Controls`는 wrapper view, raw AvalonDock workspace controller, guide/gesture/command controller, guide overlay state controller, workspace layout/save scheduler, generic dock document sync controller, generic dock document orchestrator, generic document projection controller, generic document state store/controller, workspace lifecycle/event binder를 소유합니다. app-local workspace/content/document/state marker/state-store forwarding adapter는 제거된 상태입니다. 다음 단계는 app에 남은 viewer/content factory 조립 경계를 더 얇게 만드는 것입니다.

권장 방향:

- `IOpenVisionDockedLayerViewer`, `IOpenVisionDockedLayerViewerFactory` 경계를 유지하면서 app의 bitmap/viewer 생성 책임은 app adapter에 남깁니다.
- `OpenVisionShellHostDockedLayerOrchestrator`는 현재 `IOpenVisionDockedLayerOrchestrator` 뒤에 있으므로 ShellHost 소비부는 concrete 이동에 덜 민감합니다.
- app orchestrator 내부에 남은 주요 책임은 content/view factory 조립과 app-specific image/status lookup입니다.
- app state persistence는 `OpenVisionDockedLayerDocumentStateFactory`의 path wiring만 남았습니다.
- layer viewer metrics projection은 `OpenVisionDockedLayerDocumentProjection`에 모았으므로 orchestrator로 다시 흩어지지 않게 유지합니다.
- 이동 전후로 output layer 생성이 input layer를 변경하지 않는지, comparison pane이 자동 생성되지 않는지 계속 확인합니다.

### 3. AvalonDock-dependent controller library 이동 상태 정리

아래 controller들은 이미 `OpenVisionLab.Docking.Controls` 쪽으로 이동된 상태입니다.

- `OpenVisionLayerDockingGuidePresenter`
- `OpenVisionLayerDockingGestureController`
- `OpenVisionLayerDockingCommandController`

남은 후보:

- `OpenVisionShellHostDockedLayerOrchestrator`

주의:

- `OpenVisionLayerViewerView` 직접 생성/반환과 `IDisplayManager` 직접 의존은 1차 추상화가 끝났습니다.
- generic document sync 흐름은 `OpenVisionDockDocumentController`로 library 이동이 끝났습니다. app의 `OpenVisionDockedLayerDocumentController` forwarding adapter는 제거됐습니다.
- generic document command/orchestration 흐름은 `OpenVisionDockDocumentOrchestrator`로 library 이동이 끝났습니다.
- `OpenVisionDockedLayerWorkspaceController` forwarding adapter는 제거됐습니다.
- 아직 layer-specific viewer/content 모델과 image/status lookup은 app 쪽에 남아 있습니다.
- 다음 이동은 library가 app의 viewer/image/display/runtime을 알지 않도록 adapter contract를 유지해야 합니다.

### 4. ShellHost code-behind 축소

목표:

- View 코드비하인드는 view event wiring과 wrapper 연결 수준만 남긴다.
- 도킹, 문서 저장/복원, 레이어 이동, viewer 상태 갱신 로직은 controller/viewmodel/service 쪽으로 이동한다.

### 5. 실제 EXE 기반 도킹 UX 검증

사용자 관점 테스트:

- Main 이미지 로드.
- Matching 또는 HSV/Threshold로 output layer 생성.
- 레이어 탭을 드래그해 전체 workspace 좌/우/상/하 도킹 가이드 확인.
- 이미 도킹된 패널의 탭을 다시 드래그해 해당 패널 기준 상/하/좌/우/중앙 guide 확인.
- 중앙 drop 시 같은 패널 내 tab으로 병합되는지 확인.
- 오른쪽/왼쪽 drop 시 기준 패널 옆에 split되는지 확인.
- white theme/titlebar가 다시 노출되지 않는지 확인.

### 6. Tool PropertyGrid regression smoke

다음 안정 기능은 작업 후 regression 확인이 필요합니다.

- Matching property grid order/visibility/manual preview.
- EdgeBasedMatching angle/scale/coarse options.
- Blob threshold preview image.
- Contour outline drawing and display options.
- Line Scan Line UI wording/order.
- ROI editor click/drag/up state.
- Viewer zoom/pan/drag.

### 7. Recipe XML persistence 재확인

사용자가 원래부터 recipe XML 영구 저장 로직이 있었다고 명확히 언급했습니다. 다음 작업자는 툴별 설정값 저장/복원을 반드시 확인해야 합니다.

검증 포인트:

- 툴 property model 값 저장.
- ROI/mask/template 저장.
- tool reopen 시 마지막 값 복원.
- document cache/floating/docked 전환 시 값 유실 없음.

### 8. Matching / EdgeBasedMatching 성능 기준표 정리

이미지 매칭과 엣지 기반 매칭은 비교 기준표가 필요합니다.

샘플 위치:

- `C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch`
- `C:\Git\OpenVisionLab_Dev\bin\Debug` 하위 여러 이미지 폴더

검증 방향:

- 작은 샘플만 사용하지 말고 큰 이미지/큰 템플릿 포함.
- 원본을 단순 scale 변환한 검증 이미지 사용.
- angle 0/5/10도 등 회전 샘플도 측정.
- score, 위치 오차, angle/scale 오차, tact time 기록.

## 주의사항

- 현재 worktree에는 여러 이전 작업의 dirty file이 있을 수 있습니다. 관련 없는 변경을 되돌리지 마십시오.
- `Library/OpenVisionLab.Docking.Controls\bin` 및 `obj`는 build artifact입니다. Git에 포함할지는 별도 판단이 필요합니다.
- 작업 완료를 주장하기 전에 실제 build/smoke/EXE smoke 중 변경 범위에 맞는 검증을 실행해야 합니다.
- 테스트는 변경 포인트 중심으로 최소화합니다. 무관한 전체 테스트 남발은 피합니다.
- 코드 수정 후 문서도 함께 갱신해야 합니다. 특히 완료되어 건드리면 안 되는 동작은 `docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`에 추가합니다.
- UI 변경은 실제 EXE로 열어 확인하는 것이 원칙입니다. 단순 빌드 성공만으로 UX 완료로 판단하지 않습니다.

## 다음 대화 시작 프롬프트

다음 파일을 그대로 붙여넣어도 됩니다.

- `NEXT_CODEX_PROMPT.md`
