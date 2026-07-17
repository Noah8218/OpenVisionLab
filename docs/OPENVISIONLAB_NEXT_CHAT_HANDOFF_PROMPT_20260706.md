# OpenVisionLab Next Chat Handoff Prompt

Updated: 2026-07-16 KST

Use this document when moving the work to a new Codex chat. It summarizes the current state, the active constraints, completed work that should not be rediscovered, current dirty files, verification evidence, and a paste-ready prompt.

## Current Workspace

- Dev repo: `C:\Git\OpenVisionLab_Dev`
- Original repo: `C:\Git\OpenVisionLab`
- Work in Dev first.
- Do not touch, prepare, stage, commit, or push the original repo unless the user explicitly asks.
- Do not run `git push` unless the user explicitly requests PUSH.
- Do not bulk-copy Dev into the original repo.

## Product Direction

OpenVisionLab is an LLM-assisted OpenCvSharp4 rule-based vision recipe workbench.

It is not a camera, lighting, PLC, I/O, account, or production deployment platform. Its value is:

1. Load or choose sample images.
2. Describe inspection intent and detection points.
3. Use GPT/Gemini/Claude or another LLM to draft OpenVisionLab XML.
4. Load and validate XML in OpenVisionLab.
5. Verify rule-based OpenCvSharp4 tools.
6. Review Good/Bad samples, failed steps, metrics, layers, ROI, templates, and parameters.
7. Save validated recipes for learning, review, and later integration by another system.

Stable contracts:

- Algorithm tools remain PropertyGrid-based.
- Preview/Run are explicit user actions only.
- Layer create/delete/load-image, visibility toggles, and output layer creation must not auto-run tools.
- Output layer creation must not automatically change the selected input layer.
- Keep viewer zoom/pan/drag, ROI overlay, template editor, layer comparison, and docking.
- Keep main window minimize, maximize/restore, and close controls visible.
- UI/UX changes require fresh current-build before/after screenshots shown directly in chat.
- Smoke tests that launch `OpenVisionLab.exe` must use the latest updated build output. Build first, or verify the EXE timestamp/path matches current source changes before screenshots or smoke reporting.

## Current Git State Observed Before This Handoff

`git status --short` in `C:\Git\OpenVisionLab_Dev` showed:

```text
 M "0. UI/0) MENU/Wpf/OpenVisionShellHostRecipeCommandSurface.cs"
 M "0. UI/0) MENU/Wpf/OpenVisionShellHostView.xaml"
 M AGENTS.md
 M OpenVisionLabDirectSmokeRunner.cs
 M docs/OPENVISIONLAB_NEXT_SESSION_HANDOFF.md
 M docs/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md
 M tools/PipelineViewerScreenshotSmoke/Program.cs
?? docs/OPENVISIONLAB_LLM_TOOL_CATALOG.json
?? docs/OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md
?? docs/OPENVISIONLAB_NEXT_CHAT_HANDOFF_PROMPT_20260706.md
```

Notes:

- `AGENTS.md` was updated to require latest-build EXE usage for smoke tests.
- The two LLM docs are intentionally new and untracked at this moment.
- The Recipe Manager and smoke runner changes were already in progress before this handoff. Do not revert them.
- `git log --oneline -5` failed in this restricted sandbox pass because of a read ACL/helper issue. Use `git log --oneline -5` in the next chat if available.

## Completed Work Not To Rediscover

Use these docs as the first orientation sources:

- `AGENTS.md`
- `docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`
- `docs\OPENVISIONLAB_NEXT_SESSION_HANDOFF.md`
- `docs\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`
- `docs\OPENVISIONLAB_PUBLIC_SAMPLE_ASSET_POLICY.md`
- `docs\OPENVISIONLAB_EXTERNAL_REFERENCE_POLICY.md`
- `docs\OPENVISIONLAB_RELEASE_VERSION_POLICY.md`
- `docs\OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md`
- `docs\OPENVISIONLAB_LLM_TOOL_CATALOG.json`

Already completed enough to avoid redoing:

- Recipe Manager CRUD/import/export baseline.
- Recipe Manager workbench-sized overlay, draggable title bar, close affordance, and compact command strip.
- Recipe library and pipeline list filtering with visible/total count.
- Large recipe library smoke with 100 temporary long recipes.
- Large pipeline list smoke with 100 temporary long pipelines in one recipe.
- LLM prompt/draft/load/validate/import baseline.
- LLM XML validation issue rows, before/after diff, dependency/path hints, and dependency drill-down rows.
- LLM result-channel contract: `Inspection.Status`, `Inspection.FailedStep`, `Inspection.Evidence`, `Inspection.Benchmark`, `Inspection.NextAction` are derived review channels, not XML nodes.
- LLM XML authoring guide and machine-readable tool catalog.
- LLM failure/correction direct smoke coverage for malformed XML, missing input layer, unsupported ToolType, missing dependency path, invalid parameter values, matching score percentage misuse, missing Arithmetic `InputLayerB`, correction bundle copy, and corrected draft import.
- Recipe Manager XML/Step tab, inline Step list, compact Step rows, Step comparison, selected Step detail, operator context, input/output thumbnails, ROI/template metadata, embedded PropertyGrid apply-back, corrected-output review, and branch/output comparison.
- Real multi-branch verification with `BentPin_TopBottom_Overlay.pipeline.xml`.
- Real 3+ fan-out verification with `Contour_AllSymbolsAndFaint_LLM.pipeline.xml`.
- Good/Bad role failed-Step drill-down, rerun/comparison actions, and selected-run review copy.
- Top-level account/operator chrome was removed and should not return unless real account/session requirements are introduced.
- Main window minimize/maximize/close controls were restored/verified and must remain.
- Tool View shared bases already exist for Blob/Contour/Line single-input PropertyGrid shells, Matching-family single-input PropertyGrid shells, and double-input Arithmetic custom shell.

## Latest UI Evidence

Recipe Manager sample summary density:

- Before/current: `artifacts\recipe_manager_current_density_review_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- After: `artifacts\recipe_manager_density_after_sample_summary_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- After smoke command:

```powershell
dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_density_after_sample_summary_20260706_r1
```

Result: `layout=0`, `text=0`, `internal=0`.

Change: Recipe Manager left library pane baseline width changed to 320px, and sample acceptance summary shortens displayed sample id while preserving full text in tooltip. This is layout/readability only and does not add Preview/Run behavior.

## Latest Verification Evidence

Most recent reported checks passed:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_density_after_sample_summary_20260706_r1
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1
git diff --check
```

Observed results:

- Build: PASS, 0 warnings, 0 errors.
- Screenshot smoke: PASS, `layout=0`, `text=0`, `internal=0`.
- Readiness check: PASS.
- External references: PASS.
- Public sample assets: PASS.
- `git diff --check`: PASS with CRLF warnings only.
- `git diff --check -- AGENTS.md`: PASS with CRLF warning only.

Latest current evidence update on 2026-07-15 21:42 KST:

- Smoke/capture evidence must come from the latest updated EXE or a current-source view generated after the latest relevant source changes. Do not show older artifact images as current UI; label them as historical/baseline only.
- Latest build: `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- Latest Debug EXE `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe` was rebuilt at `2026-07-15 21:36:33 KST` with 0 warnings and 0 errors.
- Latest current-source Validation Set before/after captures are under `artifacts\validation_set_path_repair_20260715\before` and `after_screen`; `wpf_shell_host_recipe_local_validation_set` passed with `layout=0`, `text=0`, and `internal=0` after covering duplicate rejection, expected/notes preservation, and suite re-enable.
- P4 recipe-local Validation Sets are complete at the bounded scope: explicit files, top-level folders, expected OK/NG, notes, missing-file blocking, existing suite/history execution, and operator-selected repair. Recursive search and automatic path rewriting remain prohibited.
- Readiness, external-reference, public-sample (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), and `git diff --check` passed after the Validation Set slices.
- The manual external-GPT packet now uses the public-safe `Line_Pins_Synthetic_OK.png` and `Line_Pins_Synthetic_WidePin_NG.png` pair. Send both images plus the complete contents of `llm_prompt_packets\pin_gap_distance\COPY_THIS_TO_GPT.md`; do not send the other packet files on round 1.
- Verified starter contract: four edge-to-edge gap ROIs (`108,170,65,120`; `204,170,65,120`; `300,170,65,120`; `396,170,65,120`), `PIXELPERMM=0.006`, `DistanceMmAvg=0.14..0.17`, and `DistanceMmRange<=0.02`. Current-source runner evidence is under `artifacts\llm_gpt_packet_public_pin_20260715`.
- A real manually transferred GPT round-1 response is now preserved unchanged under `artifacts\llm_transcripts\raw\20260715_pin_gap_gpt_round1`; SHA-256 is `D2944FF344CFECC9CA90F09EEEDD0006B1D7E85A3D79669E7EA2AD4F960EBF3E`. The exact GPT model/version was not provided.
- Recipe Manager validation/import passed with 9 Steps, 0 errors, 0 warnings, and no image run during import. Explicit nominal execution passed 9/9 Steps; explicit wide-pin execution produced the expected NG at `DistanceMmAvg 0.116 < 0.14`.
- Privacy/public-asset review passed. The unchanged prompt/XML and public-sample result images are now a sanitized candidate under `artifacts\llm_transcripts\sanitized\20260715_pin_gap_gpt_round1_direct_success`; raw and sanitized prompt/response hashes match.
- Fresh current-build `recipe-manager-tabs` evidence under `artifacts\p13_recipe_manager_llm_ux_baseline_20260715` passed. The full-window LLM XML capture shows no clipped controls, hidden input text, or incoherent overlap, and the report confirms visible validate/import/sample-run guidance; no speculative UI edit was made.
- The real P12 GPT recipe exposed and closed one branch-review gap: `OverlayMerge.SourceLayers` was previously ignored. Latest `OpenVisionLab.exe` evidence under `artifacts\p15_real_gpt_branch_review_20260715\final_exe_retry1` shows all four source-consumer and overlay-producer relations, with no Preview/Run or active-layer side effect. The full EXE Recipe Manager regression under `recipe_manager_regression_exe` also passed.
- Recipe Manager information architecture was separated after fresh EXE review. Summary now contains recipe library/search, one selected-recipe overview, and lifecycle CRUD; advanced review hides those outer controls, opens Pipeline review at full width, and provides `Back to summary` plus only technical tabs and XML/review transfer actions. Latest EXE before evidence is under `artifacts\p16_recipe_manager_structure_20260715\before_exe`; final after evidence and the passing regression are under `after_exe_final_200617`.
- Direct smoke scenarios now delete reserved generated `Smoke_<scenario>_<12 hex>` recipe workspaces in `finally`; after the latest EXE regression the runtime recipe root contained only `Default`.
- P17 operator-name validation passed with recipe `배터리 벤트 검사` and pipeline `벤트 영역 이진화`. Latest actual-EXE evidence under `artifacts\p17_recipe_manager_operator_flow_20260715\actual_exe_final_202115` proves summary -> Pipeline Review -> recipe summary -> advanced review -> summary with Preview/Run `0`, layer count `1`, and routing unchanged. No additional layout edit was justified by this real task.
- P18 audited the sanitized GPT direct-success candidate. Privacy, public-input provenance, hashes, metadata, validation/import, nominal PASS, and expected negative NG passed current-build review. After the publication gate was presented, the user approved continuation and the five-file package was added to the Dev worktree at `docs\evidence\llm\20260715_pin_gap_gpt_direct_success`. It has not been staged, committed, pushed, or copied to Original. Latest-EXE evidence is under `artifacts\p18_llm_transcript_publication_review_20260715`.
- P19 replayed the five-file package directly from `docs/evidence/llm/20260715_pin_gap_gpt_direct_success`. Content/disclosure/hash checks passed; latest-EXE validate/import passed with no image run, nominal execution passed 9/9, the wide-pin image produced the expected NG at `0.116 < 0.14`, result hashes were byte-identical, and no process/generated recipe remained. Evidence is under `artifacts/p19_llm_evidence_package_20260715`.
- P20 moved Threshold Learn-window lifecycle ownership from `ThresholdToolWpfView` to `ThresholdToolLearnWindowController`. The View now delegates open/dispose, while the Controller owns create/activate/apply forwarding/detach/reopen state. After explicitly rebuilding the screenshot-smoke project, the enhanced current-source shell smoke verified single-window activation, close/reopen, applied value transfer, and Preview/Run count unchanged. Build and policy checks passed; valid before evidence is under `artifacts/p20_threshold_learn_controller_20260715/before_current_source`, and verified after evidence is under `after_visual_verified` and `after_shell_verified`.
- A first image-viewer read temporarily showed black regions in P20 captures, but direct PNG pixel inspection and repeat reads proved the files were normally painted. This was an evidence-viewer presentation issue, not a PNG defect; do not add a render-completeness guard from that false observation. The legacy `colors=64` and `flat=0%` placeholders remain non-authoritative report fields.
- P21 moved the remaining common Tool-header Learn-window creation out of `VisionToolSingleInputPropertyToolShell` and `VisionToolDoubleInputCustomToolShell` into `VisionToolLearnWindowController`. Repeated clicks now reactivate one window, closing then reopening creates a fresh window at the same topic, and the strengthened single/double-input smokes keep Preview/Run unchanged. Current-source before/after evidence is under `artifacts/p21_common_learn_window_controller_20260715`; build, readiness, external-reference, public-sample, and diff checks passed.
- P22 used a fresh current-EXE `recipe-manager-tabs` task to find one real novice-facing LLM/Guided Setup issue: the Korean UI still displayed English operator guidance such as `Required inputs`, `Starter XML creation only`, `READY`, and the average-only warning. `OpenVisionShellHostRecipeCommandSurface` now localizes the title, summary, required-input help, readiness/missing states, and pin-gap calibration explanation while preserving technical identifiers such as `MM-READY`, `PX-ONLY`, `SCORE_MIN`, and metric names. The direct EXE regression passed; the strengthened `wpf_shell_host_recipe_guided_setup` smoke passed with `layout=0`, `text=0`, and `internal=0`. Valid before/after evidence is under `artifacts/p22_current_recipe_llm_audit_20260715/before_current_exe` and `after_current_exe`, with the final current-source assertion capture under `after_current_source_guided_final`.
- P23 fixed a fresh Guided Setup mismatch: changing the intent or required inputs could leave the previous valid Starter XML visible with no stale indication. The prior XML is now preserved but marked with an amber `설정이 변경되었습니다. Starter XML을 다시 만들어 주세요.` warning, Import readiness is cleared, and only an explicit successful `Starter XML 만들기` clears the warning. Focused smoke proves byte preservation, Import blocking, explicit regeneration, and Preview/Run unchanged. Valid before/after current-source evidence is under `artifacts/p23_guided_setup_stale_draft_20260715/before_current_source` and `after_current_source_visible`; latest-build actual-EXE `recipe-manager-tabs` passed under `actual_exe`.
- P24 added the next self-contained manual GPT task under `llm_prompt_packets/blob_particle_count`: attach its nominal and sparse-negative PNGs, then paste `COPY_THIS_TO_GPT.txt` unchanged. The packet tests `Threshold -> Blob` and `ResultCount 8..14`; its copied image hashes and 11 prompt-contract tokens passed. No GPT response exists yet. When the user returns one, preserve it unchanged and test it before using `PASTE_VALIDATION_NG_BACK_TO_GPT.txt` for a real correction round.
- P25 preserved and replayed the user's XML-only GPT response for the Blob packet. Latest-build Recipe Manager validation/import passed with 2 Steps, 0 errors/warnings, and no image run; explicit nominal execution passed with `ResultCount=12`, while the sparse negative produced the expected NG at `ResultCount=3 < 8`. Raw evidence is under `artifacts/llm_transcripts/raw/20260715_blob_particle_gpt_round1`. Classify it as manually transferred GPT direct success, exact model unknown, non-API, zero correction rounds; do not use its correction prompt.
- P26 added the next self-contained manual GPT task under `llm_prompt_packets/matching_die_pad`. Attach its nominal, no-target negative, and template PNGs together, then paste `COPY_THIS_TO_GPT.txt` unchanged. It tests a one-Step `Matching` recipe, exact repository-relative template paths, normalized `SCORE_MIN=0.6`, `MAGNIFIATION=4`, and `ResultCount=3`. All copied image hashes and prompt-contract tokens passed. Current-build baseline replay passed nominal `ResultCount=3` and rejected the no-target image at `ResultCount=0`; evidence is under `artifacts/p26_matching_packet_baseline_20260715`. No GPT response exists yet; preserve the first response unchanged before any validation or correction.
- P27 preserved and validated the user's first Matching GPT response. XML syntax/schema/routing passed, but Recipe Manager naturally blocked Import because the initial packet's `docs\samples\...` paths did not resolve from current `bin\Debug` StartupPath. This is a host prompt-contract defect, not GPT disobedience. Raw evidence and the complete report are under `artifacts/llm_transcripts/raw/20260715_matching_die_pad_gpt_round1`. Paste `round2_prompt.txt` into the same GPT conversation and preserve the next response unchanged; correction-loop success is not yet proven.
- P28 completed that real GPT correction loop. The unchanged round 2 response changed exactly `TemplatePath` and `PATTERN_PATH`; Recipe Manager validation/import passed with dependencies copied and no image run during import. Explicit nominal execution passed at `ResultCount=3`, and the no-target image produced expected NG at `ResultCount=0 < 3`. Raw evidence is complete under `artifacts/llm_transcripts/raw/20260715_matching_die_pad_gpt_round1`. Exact model/version remains unknown, transfer was manual, no API was used, and the one correction round was caused by the initial packet's host-relative path defect. Do not publish raw local-path reports directly.
- P29 prepared and audited a seven-file sanitized candidate at `artifacts/llm_transcripts/sanitized/20260715_matching_die_pad_gpt_correction_loop`. Byte identity, four-occurrence reversible root sanitization, privacy scan, public-asset provenance, PNG metadata, current-build round 1/round 2 replay, and result hashes passed. Its initial `CONDITIONAL GO / CURRENTLY HOLD` decision is historical and was superseded by P30.
- P30 followed the user's explicit approval and added the seven-file package to the Dev worktree at `docs/evidence/llm/20260715_matching_die_pad_gpt_correction_loop`. Tracked-path replay reproduced round 1 dependency NG, round 2 validation/import PASS without image execution, nominal `ResultCount=3` PASS, and no-target `ResultCount=0 < 3` expected NG; both result image hashes match the package. Build, readiness, external-reference, public-sample, package/hash/privacy, cleanup, and diff checks passed. `docs/OPENVISIONLAB_LLM_MATCHING_CORRECTION_PUBLICATION_REVIEW_20260715.md` now records `GO / ADDED TO THE DEV WORKTREE`. Nothing was staged, committed, pushed, or copied to Original.
- P31 added a six-file self-contained EdgeBasedMatching GPT packet at `llm_prompt_packets/edge_fiducial_matching`. Its one-Step contract uses the verified StartupPath-relative template path and distinguishes normalized `SCORE_MIN=0.70` from the `ScoreMax=70..100` acceptance scale. Recipe Manager validation/import passed without image execution; explicit current-build replay passed the L-fiducial nominal image at `ResultCount=1`, `ScoreMax=99.598` and rejected the wrong T shape at `ResultCount=0`, `BestScore=61.052`. Packet, XML-contract, path, placeholder, and copied-image hash checks passed. No external GPT response exists yet.
- P32 preserved and replayed the user's unchanged XML-only response for P31 under `artifacts/llm_transcripts/raw/20260716_edge_fiducial_gpt_round1`. Latest-EXE Recipe Manager validation/import passed with 1 Step, 0 errors/warnings, two dependencies copied, and no image run during import. Explicit nominal execution passed at `ResultCount=1`, `ScoreMax=99.598`; the wrong T image produced expected NG at `ResultCount=0`, `BestScore=61.052`. This is manually transferred GPT direct success, exact model unknown, non-API, zero corrections; do not use the correction prompt.
- P33 created and audited a six-file artifact-only candidate at `artifacts/llm_transcripts/sanitized/20260716_edge_fiducial_gpt_round1_direct_success`. Four evidence files are byte-identical to raw, privacy/path scans returned zero findings, PNG metadata checks passed, all three inputs are registered public synthetic assets, and latest-EXE replay reproduced validation/import PASS, nominal `ResultCount=1`/`ScoreMax=99.598`, expected wrong-T NG, and byte-identical result hashes. Repository inclusion is not approved; the candidate remains ignored under `artifacts`.
- P34 followed the user's explicit approval and added the six-file package to `docs/evidence/llm/20260716_edge_fiducial_gpt_direct_success`. Package-wide privacy/path and PNG metadata checks passed. Fresh latest-EXE tracked-path replay under `artifacts/p34_edge_fiducial_tracked_package_20260716` reproduced validation/import PASS without image execution, nominal `ResultCount=1`/`ScoreMax=99.598`, expected wrong-T `ResultCount=0`/`BestScore=61.052` NG, and byte-identical result hashes. `docs/OPENVISIONLAB_LLM_EDGE_FIDUCIAL_PUBLICATION_REVIEW_20260716.md` records `GO / ADDED TO THE DEV WORKTREE`. P34 is not committed or pushed yet; Original was not touched.

## Current Blockers / Risks

- The corpus now contains real manually transferred GPT direct-success evidence and one naturally occurring report-driven Matching correction loop. Provider/model breadth and broad intent reliability are still missing; exact GPT model/version remains unknown.
- Current environment check found missing `OPENAI_API_KEY`, `ANTHROPIC_API_KEY`, `CLAUDE_API_KEY`, `GEMINI_API_KEY`, and `GOOGLE_API_KEY`.
- Do not fabricate "real" external LLM transcript examples. If keys or manually exported transcripts become available, store raw prompts/responses under `artifacts\llm_transcripts\raw`, sanitize them, then decide what can be committed.
- User-provided/operator-repaired XML replay cases belong under `artifacts\llm_transcripts\manual`; they can prove validation behavior but must not be reported as real GPT/Gemini/Claude transcript evidence.
- Current manual replay evidence: pin distance intent with a Contour/Threshold draft is blocked by the intent contract. Latest direct EXE smoke `recipe-manager-llm-intent-skills` in `artifacts\llm_manual_replay_contract_after_20260707_r1` passed with `PinGapContourMismatch: blocked by intent contract` and `PreviewRunCountUnchanged: 0`.
- Recipe Manager after screenshot still shows some dense run-summary text. It is not a clipping error in the latest smoke, but can be compacted later if a fresh screenshot shows actual workflow friction. P22 removed the proven Korean/English guidance mix and P23 marks outdated Starter XML explicitly; do not reopen another copy pass without fresh evidence.
- The P13 direct-success and P30 correction-loop packages are present only in the Dev worktree. Do not describe either as remotely published until a later explicit commit/push request succeeds.
- The P32 EdgeBasedMatching response is preserved only under raw artifacts. Its reports contain local paths; do not place it under `docs/evidence` or describe it as publishable before a separate privacy/sanitization and publication decision.
- P34 closes the Edge-fiducial publication gate and was pushed in Dev commit `7a7dc51f`. Do not copy it to Original without a separate explicit request.
- P35 proves the public FeatureMatching Good/Wrong baseline and adds a self-contained manual GPT packet under `llm_prompt_packets/feature_matching_card`. P35 itself is not committed or pushed yet.
- P36 preserves and replays a real manually transferred GPT FeatureMatching direct-success response under raw artifacts. Exact model/version remains unknown, no API was used, and correction rounds are zero.
- P37 created and replayed a six-file artifact-only sanitized FeatureMatching candidate. P38 records the user's explicit Dev-worktree inclusion decision; the approved six-file package is now under `docs/evidence/llm/20260716_feature_card_gpt_direct_success` with a publication review in `docs/OPENVISIONLAB_LLM_FEATURE_CARD_PUBLICATION_REVIEW_20260716.md`. It is still uncommitted and unpushed; Original remains untouched.
- P39 clarified the LLM XML next action with a `검사 설정` entry into Build inspection and replaced visible developer-contract wording with setup -> draft -> validate -> import guidance. P39 is uncommitted and unpushed; Original remains untouched.
- P40 clarified Run History's no-baseline instruction: rerun the same validation suite, then select the earlier saved run as baseline. It is uncommitted and unpushed; Original remains untouched.
- P41 adds the public-baseline-backed `Feature Matching` Guided Setup intent: Feature template, full-image scope, Ratio, RANSAC px, and ScoreMax minimum produce one `FeatureMatching` starter Step. XML validation rejects FeatureMatching drafts that use `ResultCount` as the only acceptance metric. Fresh current-source baseline/after evidence is under `artifacts\p41_feature_guided_setup_before_20260716` and `artifacts\p41_feature_guided_setup_after_20260716_r2`; P41 is uncommitted and unpushed, and Original remains untouched.
- P42 adds the public Edge Fiducial-backed `Edge Based Matching` Guided Setup intent: Edge template, full-image scope, minimum score, search count, Canny low/high, and ScoreMax minimum produce one `EdgeBasedMatching` starter Step. XML validation rejects EdgeBasedMatching drafts that use `ResultCount` as the only acceptance metric. Fresh closest baseline/after evidence is under `artifacts\p42_edge_guided_setup_before_20260716` and `artifacts\p42_edge_guided_setup_after_20260716`; P42 is uncommitted and unpushed, and Original remains untouched.
- P43 closes the latest-EXE coverage gap for P41/P42. The broad `recipe-manager-tabs` smoke now verifies Feature Matching and Edge Based Matching template blocking, ScoreMax-gated Starter XML, visible controls, and unchanged Preview/Run count. Latest-EXE report/screenshots are under `artifacts\p43_guided_setup_direct_exe_20260716`; P43 is uncommitted and unpushed, and Original remains untouched.
- P44 makes the public HSV ColorPatch Good/Bad pair available for constrained external XML authoring. `HSV`, `HsvMask`, `ColorHSV`, and `ColorMask` are now in the LLM catalog/guide with `MaskPixelRatio` gates and the circular `HueMin=170`, `HueMax=10` red-wrap rule. The validator now matches the runner by allowing hue wrap while retaining all bounds and saturation/value ordering checks. Latest-EXE validation/import passed with one Step and no image run; explicit replay passed nominal `MaskPixelRatio=0.058` and produced expected missing-patch NG at `0.015 < 0.05`. The self-contained first-round packet is `llm_prompt_packets\hsv_color_patch`; no real GPT response has been received yet.

- P45 corrected the LLM Intent direct smoke so it captures the real Advanced Review state. Do not use the earlier basic-mode LLM capture as current Recipe Manager UX evidence. The verified latest-EXE capture is `artifacts\p45_recipe_manager_llm_audit_after_verified_current_exe_20260716\OpenVisionLab_RecipeManager_LlmIntentSkills_TemplateMatching.png`; it hides the outer library and basic lifecycle controls and keeps the explicit return-to-summary action.

- P46-P47 replaced the Korean learner-facing `Starter XML` action with `초안 XML 만들기` / `Create draft XML` and widened the Guided Setup inspection-intent selector from 240px to 300px while reducing its adjacent label column from 130px to 120px. Latest-EXE proof is `artifacts\p46_llm_draft_labels_after_current_exe_20260716\OpenVisionLab_RecipeManager_LlmIntentSkills_TemplateMatching.png` and `artifacts\p47_guided_setup_intent_width_after_current_exe_20260716\OpenVisionLab_RecipeManager_GuidedSetup.png`. Do not revert to raw `Starter XML` user labels or reintroduce the clipped Pin gap intent.

- P48 preserved and replayed the user's XML-only GPT HSV ColorPatch response under `artifacts\llm_transcripts\raw\20260716_hsv_color_patch_gpt_round1`. Exact GPT model/version and API evidence remain unknown; do not infer them. Latest-EXE validation/import passed with one HSV Step and `ImageRun: SKIPPED`. After correcting the generic image smoke to preserve color channels and declare expected Good/NG outcomes, nominal passed at `MaskPixelRatio=0.058` within `0.05..0.07`, and the missing-patch image produced the expected NG at `0.015 < 0.05`; both source images had 3 channels. This is a real manual direct-success transcript with zero correction rounds. Do not use the correction prompt or publish raw local-path reports without separate sanitization and explicit inclusion approval.

- P49 followed the user's explicit inclusion approval. The six-file HSV package is now `docs\evidence\llm\20260716_hsv_color_patch_gpt_direct_success`, with publication review `docs\OPENVISIONLAB_LLM_HSV_COLOR_PATCH_PUBLICATION_REVIEW_20260716.md`. Payload scans and PNG metadata checks passed. Latest-EXE tracked-path replay passed import without image execution, nominal at `MaskPixelRatio=0.058`, and expected missing-patch NG at `0.015 < 0.05`; included result PNGs match replay byte-for-byte. P49 is uncommitted and unpushed; Original remains untouched.

- P50 selected Mean Brightness as the next bounded GPT authoring task. The existing public Good/Dark baseline passes at `MeanValueAvg=201.5` and rejects at `117.5 < 185`. `llm_prompt_packets\mean_brightness` has a self-contained five-file packet that requires exactly one full-image Mean Step and the `185..220` MeanValueAvg gate. The reference XML passed latest-EXE import, nominal, and expected negative replay. No external GPT response has been received yet.

- P51 preserved and replayed the user's XML-only GPT Mean Brightness response under `artifacts\llm_transcripts\raw\20260716_mean_brightness_gpt_round1`. Exact GPT model/version and API evidence remain unknown; do not infer them. A 21-field structured comparison found zero differences from the P50 reference. Latest actual EXE validation/import passed with one Mean Step and `ImageRun: SKIPPED`; nominal passed at `MeanValueAvg=201.5`, while dark image produced expected NG at `117.5 < 185`, both with 3 source channels. This is a real manual direct-success transcript with zero correction rounds. Do not use the correction prompt or publish raw local-path reports without separate sanitization and explicit inclusion approval.

- P52 received that explicit approval. The sanitized six-file Mean evidence package is `docs\evidence\llm\20260716_mean_brightness_gpt_direct_success`, with publication review `docs\OPENVISIONLAB_LLM_MEAN_BRIGHTNESS_PUBLICATION_REVIEW_20260716.md`. Payload scans and PNG metadata checks passed. A fresh-build actual-EXE tracked replay passed package import without image execution, nominal at `MeanValueAvg=201.5`, and expected dark NG at `117.5 < 185`; included result PNGs match replay byte-for-byte. P52 is uncommitted and unpushed; Original remains untouched.

- P53 selected the first public sequential multi-Step candidate: `Threshold -> Morphology(Open) -> Contour`, with route `Main -> Morphology_Binary -> Morphology_Clean -> Morphology_Cleanup_Preview`. `llm_prompt_packets\morphology_cleanup` is a five-file self-contained GPT packet. Its latest-EXE public baseline passed import (3 Steps, 0 errors/warnings), nominal `ResultCount=4`, and expected missing-target NG `ResultCount=2 < 4`; public image copies and prompt-contract checks passed. No external GPT response exists yet. The baseline review is `docs\OPENVISIONLAB_LLM_MORPHOLOGY_CLEANUP_BASELINE_REVIEW_20260716.md`; do not invent a response or correction round.

- P54 rechecked Recipe Manager with a fresh current EXE. The first `recipe-manager-tabs` run transiently reported a Pin-gap unit Good/Bad mismatch, but the public LineDistance baseline, exact Intent Skill-style 16/6 sampling diagnostics, and three further independent current-EXE Recipe Manager runs all passed. The cause was not reproduced. The smoke now preserves all four Pin-gap result PNGs before assertions and reports mm/px average/range metrics on a future failure; it does not retry-to-pass or change any operator behavior. Current-EXE Summary and LLM XML screenshots showed no new clipping, overlap, or unclear next action requiring a UI change. Build after this diagnostic-only change passed with 0 warnings/errors; current evidence is `artifacts\p54_pin_gap_diagnostic_after_20260716`.

- P55 selected `Filter(MedianBlur) -> Threshold -> Contour` as the next public-safe external-authoring candidate. Its existing public baseline passed a fresh build/latest-EXE import with 3 Steps, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`; nominal passed `ResultCount=4` and missing-target input produced expected NG `ResultCount=2 < 4`. `llm_prompt_packets\filter_denoise` is a five-file self-contained GPT packet with byte-identical public Good/NG image copies and a locked 3-Step route/gate. The exact packet reference XML replayed successfully in the latest EXE. Filter catalog/guide coverage now explicitly distinguishes odd `MedianKernelSize` for `MedianBlur` from other kernel/bilateral parameters. No external GPT response exists yet; do not invent one or a correction round. Evidence: `docs\OPENVISIONLAB_LLM_FILTER_DENOISE_BASELINE_REVIEW_20260716.md` and `artifacts\p55_filter_denoise_packet_20260716`.

- P56 received a real manually transferred GPT XML response for the P55 Filter packet. The unchanged response/prompt are under `artifacts\llm_transcripts\raw\20260716_filter_denoise_gpt_round1` with SHA-256 `4E684E91...D6855E` and `654BC48D...07B6C1`. XML-only and structured reference comparisons passed with zero differences. After a fresh 0-warning/0-error build, actual-EXE import passed with 3 Steps, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`; nominal passed `ResultCount=4`, while missing-target input produced expected NG `ResultCount=2 < 4`. This is a direct success with zero correction rounds. Exact GPT model/version, API evidence, and a full conversation export are unavailable. Do not manufacture a correction round. Raw reports contain local paths and require a separate sanitization and explicit inclusion decision before `docs/evidence` publication.

- P57 created an ignored six-file Filter Denoise sanitized candidate at `artifacts\llm_transcripts\sanitized\20260716_filter_denoise_gpt_round1_direct_success`. The unchanged prompt/response and current-EXE nominal/negative result PNGs are byte-identical to P56 source evidence; payload/manifest path and credential scans found zero findings, and both PNGs contain only `IHDR`/`IDAT`/`IEND` chunks. It intentionally excludes raw reports and local paths. Repository inclusion is not approved: do not copy it into `docs/evidence`, stage, commit, or publish it without a separate explicit user decision.

- P58 followed the user's explicit approval and added the six-file Filter Denoise package to `docs/evidence/llm/20260716_filter_denoise_gpt_direct_success`. Package-wide text and PNG metadata checks passed. Fresh latest-EXE tracked-path replay under `artifacts/p58_filter_denoise_tracked_package_20260716` passed import without image execution, nominal at `ResultCount=4`, and expected missing-target NG at `ResultCount=2 < 4`; included result PNGs match replay byte-for-byte. `docs/OPENVISIONLAB_LLM_FILTER_DENOISE_PUBLICATION_REVIEW_20260716.md` records `GO / ADDED TO THE DEV WORKTREE`. P58 is uncommitted and unpushed; Original remains untouched.

- P59 received the user's actual XML-only P53 Morphology responses, identified as GPT and Gemini. Both raw responses were separately preserved, parsed as XML-only, and matched the locked `Threshold -> Morphology(Open) -> Contour` route/parameter/ResultCount gate with zero required differences. Their only difference is the free pipeline name. Fresh latest-EXE imports passed with 3 Steps and no image run; both nominal paths passed `ResultCount=4` and both missing-target paths produced expected `ResultCount=2 < 4` NG. Result PNGs are byte-identical across providers because the executable contract is identical. Exact provider model/version/API evidence is unavailable, so this is not a provider benchmark or an unconstrained authoring claim.

- P60 created ignored six-file sanitized candidates for both providers at `artifacts\llm_transcripts\sanitized\20260716_morphology_cleanup_gpt_round1_direct_success` and `artifacts\llm_transcripts\sanitized\20260716_morphology_cleanup_gemini_round1_direct_success`. Each passed immutable hash, payload/manifest text, PNG metadata, and ignored-artifact checks. Repository inclusion is not approved: do not copy either candidate into `docs/evidence`, stage, commit, or publish it without a separate explicit user decision.

- P61 followed the user's explicit approval and added two six-file Morphology packages to `docs/evidence/llm`: `20260716_morphology_cleanup_gpt_direct_success` and `20260716_morphology_cleanup_gemini_direct_success`. Package-wide text/privacy/PNG/file-set checks passed. Fresh latest-EXE tracked-path replays under `artifacts/p61_morphology_gpt_gemini_tracked_packages_20260716` passed import without image execution, nominal at `ResultCount=4`, and expected missing-target NG at `ResultCount=2 < 4` for both packages; included result PNGs match replay byte-for-byte. The paired GPT/Gemini publication reviews record `GO / ADDED TO THE DEV WORKTREE`. P61 is uncommitted and unpushed; Original remains untouched.

- P62 selected the public `Arithmetic(Bitwise_NOT) -> Mean` workflow as the next distinct LLM evidence candidate. A fresh 0-warning/0-error build and latest actual EXE proved the public baseline Good (`MeanValueAvg=208`) and Bright-NG (`76.7 < 190`) outcomes, then proved the exact packet reference XML with the same results. `llm_prompt_packets\arithmetic_invert` is a five-file self-contained GPT packet with two byte-identical public images, normal operator intent, locked unary `Bitwise_NOT` routing, and a `190..230` MeanValueAvg gate.

- P63 received a real manually transferred XML-only response identified by the user as GPT for the P62 Arithmetic packet. Raw prompt/response evidence is under `artifacts\llm_transcripts\raw\20260716_arithmetic_invert_gpt_round1`; exact model/version, API evidence, and full provider-chat export are unavailable. XML-only and structured reference checks passed with zero differences across 34 fields. After a fresh 0-warning/0-error build, actual `bin/Debug/OpenVisionLab.exe` import passed with 2 Steps, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`; nominal passed `MeanValueAvg=208`, while Bright-NG produced expected product NG `MeanValueAvg=76.7 < 190`. This is a constrained direct success with zero correction rounds. Do not manufacture a correction round. Raw reports require separate sanitization and explicit inclusion approval before any `docs/evidence` publication. P63 is uncommitted and unpushed; Original remains untouched.

- P64 created the ignored six-file Arithmetic sanitized candidate at `artifacts\llm_transcripts\sanitized\20260716_arithmetic_invert_gpt_round1_direct_success`. Its copied prompt, XML, nominal result PNG, and Bright-NG result PNG match P63 evidence byte-for-byte. Payload privacy scans found zero disallowed path/contact/credential/private-asset findings; each 572x420 result PNG contains only `IHDR`/`IDAT`/`IEND` chunk types. It deliberately excludes raw reports and local evidence locations. Repository inclusion is not approved: do not copy it into `docs/evidence`, stage, commit, or publish it without a separate explicit user decision.

- P65 followed the user's explicit approval and added the six-file Arithmetic package to `docs/evidence/llm/20260716_arithmetic_invert_gpt_direct_success`. Package-wide file-set, privacy, public-asset, PNG metadata, and immutable-hash checks passed. Fresh latest-EXE tracked-path replay under `artifacts/p65_arithmetic_invert_tracked_package_20260716` passed import without image execution, nominal at `MeanValueAvg=208`, and expected Bright-NG at `MeanValueAvg=76.7 < 190`; included result PNGs match replay byte-for-byte. `docs/OPENVISIONLAB_LLM_ARITHMETIC_INVERT_PUBLICATION_REVIEW_20260716.md` records `GO / ADDED TO THE DEV WORKTREE`. P65 is uncommitted and unpushed; Original remains untouched.

- P66 selected `EdgeDetection(Canny) -> Morphology(Close) -> Contour` as the next distinct public-safe external-authoring candidate. Its existing public baseline passed a fresh build/latest-EXE import with 3 Steps, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`; nominal passed `ResultCount=4` and missing-shape input produced expected NG `ResultCount=2 < 4`. `llm_prompt_packets/edge_detection_shapes` is a five-file self-contained GPT packet with byte-identical public Good/Missing-NG image copies and a locked Canny/Close/Contour route/gate. The exact packet reference XML replayed successfully in the latest EXE. No external GPT response exists yet; do not invent one or a correction round. Evidence: `docs/OPENVISIONLAB_LLM_EDGE_DETECTION_BASELINE_REVIEW_20260716.md` and `artifacts/p66_edge_detection_baseline_20260716`.

- P67 received a real manually transferred XML-only response in the P66 GPT packet workflow. Raw prompt/response evidence is under `artifacts/llm_transcripts/raw/20260716_edge_detection_shapes_gpt_round1`; exact model/version, API evidence, and full provider-chat export are unavailable. XML-only and structured reference checks passed with zero differences across 53 fields. After a fresh 0-warning/0-error build, actual `bin/Debug/OpenVisionLab.exe` import passed with 3 Steps, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`; nominal passed `ResultCount=4`, while Missing-NG produced expected product NG `ResultCount=2 < 4`. This is a constrained direct success with zero correction rounds. Do not manufacture a correction round. Raw reports require separate sanitization and explicit inclusion approval before any `docs/evidence` publication. P67 is uncommitted and unpushed; Original remains untouched.

- P68 created the ignored six-file Edge Detection sanitized candidate at `artifacts/llm_transcripts/sanitized/20260716_edge_detection_shapes_gpt_round1_direct_success`. Its copied prompt, XML, nominal result PNG, and Missing-NG result PNG match P67 evidence byte-for-byte. Payload privacy scans found zero disallowed path/contact/credential/private-asset findings; each 572x420 result PNG contains only `IHDR`/`IDAT`/`IEND` chunk types. It deliberately excludes raw reports and local evidence locations. P69 records the subsequent explicitly approved Dev-worktree inclusion.

- P69 followed the user's explicit approval and added the six-file Edge Detection package to `docs/evidence/llm/20260716_edge_detection_shapes_gpt_direct_success`. Package-wide file-set, privacy, public-asset, PNG metadata, and immutable-hash checks passed. Fresh latest-EXE tracked-path replay under `artifacts/p69_edge_detection_tracked_package_20260716` passed import without image execution, nominal at `ResultCount=4` in 33.914 ms, and expected Missing-NG at `ResultCount=2 < 4` in 30.826 ms; included result PNGs match replay byte-for-byte. `docs/OPENVISIONLAB_LLM_EDGE_DETECTION_PUBLICATION_REVIEW_20260716.md` records `GO / ADDED TO THE DEV WORKTREE`. P69 is uncommitted and unpushed; Original remains untouched.

- P70 selected the public `RotateScale` geometry workflow as the next distinct external-authoring candidate. The exact one-Step `Main -> Geometry_ResizeHalf_Result` contract uses Angle 0, ScaleX/Y 50, Linear/Constant, and `ResultImageWidth=286..286`. Fresh actual-EXE import passed with 1 Step, no errors/warnings/dependencies, `ImageRun: SKIPPED`, and the corrected `Inspection.Evidence: OK - explicit judgement criteria are present.` Nominal passed `572x420 -> 286x210` in 3.428 ms; Wide-NG produced expected `640x420 -> 320x210`, `ResultImageWidth=320 > 286` NG in 3.18 ms. `llm_prompt_packets/rotate_scale_geometry` is a five-file self-contained GPT packet with byte-identical public Good/Wide-NG copies, a locked route/parameter/gate, one correction placeholder, and a passing privacy/hash audit. No external response exists yet; preserve the first real response unchanged. Evidence: `docs/OPENVISIONLAB_LLM_ROTATE_SCALE_BASELINE_REVIEW_20260716.md` and `artifacts/p70_rotate_scale_baseline_20260716`.

## Next Priority

1. Send `Geometry_RotateScale_Synthetic_OK.png`, `Geometry_RotateScale_Synthetic_Wide_NG.png`, and the complete `llm_prompt_packets/rotate_scale_geometry/COPY_THIS_TO_GPT.txt` to GPT. Preserve the first complete XML response unchanged before validation or correction.
2. Preserve a correction-loop transcript only when a real GPT/Gemini/Claude response naturally fails validation or Good/NG execution. Do not manufacture correction loops.
3. Keep P65, P69, P70, and other approved or locked evidence baselines unchanged unless a fresh tracked replay detects hash or behavior drift; commit/push only on an explicit request.
4. Select another distinct public-safe tool family only after P70 either receives a real response or is explicitly deferred. Do not add a tool merely to broaden the corpus.
5. If the Pin-gap unit contract fails again, retain the generated `PinGapUnit_*` images and detailed mm/px metrics, then diagnose the actual divergence. Do not add a retry-to-pass workaround or change LineDistance values without new deterministic evidence.
6. Expand branch/output comparison only when another real recipe exposes a relationship not represented by direct `InputLayer` or declared `SourceLayers`; the P12 four-branch GPT recipe is now covered.
7. For every smoke/capture report, build first or verify the latest EXE/current-source view, and show only images generated in the current artifact folder unless explicitly labeled historical/baseline.

## Paste-Ready Prompt For The Next Chat

Copy the block below into the next Codex chat:

```text
작업 위치는 C:\Git\OpenVisionLab_Dev 입니다.

먼저 아래 문서를 읽고 현재 상태를 파악해 주세요.

1. C:\Git\OpenVisionLab_Dev\AGENTS.md
2. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_NEXT_CHAT_HANDOFF_PROMPT_20260706.md
3. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_NEXT_SESSION_HANDOFF.md
4. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md
5. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md
6. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md
7. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_LLM_TOOL_CATALOG.json
8. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_PUBLIC_SAMPLE_ASSET_POLICY.md
9. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_EXTERNAL_REFERENCE_POLICY.md
10. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_RELEASE_VERSION_POLICY.md

반드시 지킬 방향:

- OpenVisionLab은 LLM-assisted OpenCvSharp4 rule-based vision recipe workbench입니다.
- 카메라/조명/PLC/I/O/account/deployment 플랫폼으로 확장하지 마세요.
- 알고리즘 툴은 PropertyGrid 기반 구조를 유지합니다.
- Preview/Run은 명시적 사용자 액션이어야 합니다.
- Output 레이어 생성이 Input 레이어를 자동 변경하면 안 됩니다.
- Boolean visibility toggle, layer create/delete/load-image, output layer creation이 Preview/Run을 자동 유발하면 안 됩니다.
- Viewer zoom/pan/drag, ROI overlay, template editor, layer comparison, docking, main window minimize/maximize/close를 제거하지 마세요.
- UI/UX 수정 시 현재 빌드 기준 before/after 캡처를 새로 남기고, 채팅에 이미지를 직접 보여주세요.
- 스모크 테스트에 쓰는 OpenVisionLab.exe는 최신 업데이트된 빌드 산출물이어야 합니다. 빌드하거나 EXE timestamp/path가 현재 소스와 맞는지 확인한 뒤 smoke/capture를 보고하세요.
- Dev에서 작업하고, 사용자가 명시적으로 원본 repo 반영을 요청하기 전에는 C:\Git\OpenVisionLab 원본 repo를 건드리거나 반영 준비하지 마세요.
- git push는 사용자가 명시적으로 PUSH를 요청할 때만 실행하세요.

먼저 실행할 확인:

cd C:\Git\OpenVisionLab_Dev
git status --short
git log --oneline -5

현재 알려진 dirty 상태:

- 0. UI/0) MENU/Wpf/OpenVisionShellHostRecipeCommandSurface.cs
- 0. UI/0) MENU/Wpf/OpenVisionRecipeReviewBundleExporter.cs
- 0. UI/0) MENU/Wpf/OpenVisionRecipeReviewBundleInspector.cs
- 0. UI/0) MENU/Wpf/OpenVisionShellHostView.xaml
- AGENTS.md
- OpenVisionLabDirectSmokeRunner.cs
- docs/OPENVISIONLAB_NEXT_SESSION_HANDOFF.md
- docs/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md
- tools/PipelineViewerScreenshotSmoke/Program.cs
- docs/OPENVISIONLAB_LLM_TOOL_CATALOG.json
- docs/OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md
- docs/OPENVISIONLAB_NEXT_CHAT_HANDOFF_PROMPT_20260706.md

현재 다음 우선순위:

1. P13 정제 GPT 직접 성공 증거는 `docs/evidence/llm/20260715_pin_gap_gpt_direct_success`에 Dev 작업 트리 패키지로 포함됐습니다. 사용자가 별도로 commit/push를 요청하기 전에는 원본 repo나 원격 저장소에 반영하지 마세요.
2. 실제 GPT/Gemini/Claude 초안이 자연스럽게 validation/execution NG가 될 때만 correction-loop transcript를 확보하세요. 실패 응답을 꾸며내거나 정상 XML을 고의로 손상하지 마세요.
3. 현재 EXE/current build 기준 Recipe Manager UX에서 실제 잘림/겹침/불명확한 next action이 보일 때만 추가 개선하세요.
4. 실제 multi-branch recipe에서 현재 branch/output 비교가 부족한 경우에만 branch/output 비교 UX를 확장하세요.
5. Tool View code-behind 정리는 실제 중복 창이나 수명주기 결함과 기존 controller owner가 함께 확인될 때만 진행하세요.

최근 검증은 build, screenshot smoke, readiness, external refs, public sample assets, git diff check가 통과했습니다. 다음 작업도 완료는 말이 아니라 실제 명령 통과로 판단해 주세요.
```
