# OpenVisionLab Next Chat Handoff Prompt

Updated: 2026-07-06 KST

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

Latest current evidence update on 2026-07-06 21:57 KST:

- Smoke/capture evidence must come from the latest updated EXE or a current-source view generated after the latest relevant source changes. Do not show older artifact images as current UI; label them as historical/baseline only.
- Latest build: `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- Latest direct EXE smoke: `dotnet run --no-build --project OpenVisionLab.csproj -c Debug -- --smoke recipe-manager-tabs artifacts\current_exe_recipe_manager_tabs_20260706_r2_direct` passed with `Result: PASS`, `LlmCorrectedDraftImport: imported`, `BranchOutputComparison: 2`, `ActualMultiBranchComparison: 7`, and `ActualThreeWayBranchComparison: 5`.
- Latest current-source Arithmetic view capture: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\current_source_arithmetic_tool_20260706_r2` passed with `layout=0`, `text=0`, and `internal=0`.
- Latest Arithmetic cleanup: `ArithmeticToolInteractionController` owns parameter event attach/detach; `ArithmeticToolWpfView.xaml.cs` no longer keeps those forwarding handlers.

## Current Blockers / Risks

- Actual external LLM transcript corpus is not yet captured.
- Current environment check found missing `OPENAI_API_KEY`, `ANTHROPIC_API_KEY`, `CLAUDE_API_KEY`, `GEMINI_API_KEY`, and `GOOGLE_API_KEY`.
- Do not fabricate "real" external LLM transcript examples. If keys or manually exported transcripts become available, store raw prompts/responses under `artifacts\llm_transcripts\raw`, sanitize them, then decide what can be committed.
- User-provided/operator-repaired XML replay cases belong under `artifacts\llm_transcripts\manual`; they can prove validation behavior but must not be reported as real GPT/Gemini/Claude transcript evidence.
- Current manual replay evidence: pin distance intent with a Contour/Threshold draft is blocked by the intent contract. Latest direct EXE smoke `recipe-manager-llm-intent-skills` in `artifacts\llm_manual_replay_contract_after_20260707_r1` passed with `PinGapContourMismatch: blocked by intent contract` and `PreviewRunCountUnchanged: 0`.
- Recipe Manager after screenshot still shows some dense run-summary text. It is not a clipping error in the latest smoke, but can be compacted later if a fresh screenshot shows actual workflow friction.

## Next Priority

1. If an API key or manually exported transcript is available: collect one real LLM XML correction-loop transcript using `docs\OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md` and `docs\OPENVISIONLAB_LLM_TOOL_CATALOG.json`, then validate it in Recipe Manager.
2. If no real transcript is available: continue current-build Recipe Manager UX review only where fresh screenshots show actual clipping, overlap, unclear next action, or workflow friction.
3. Expand branch/output comparison only when a real multi-branch recipe exceeds the current selected-step producer/consumer comparison model.
4. Continue Tool View code-behind cleanup only when an existing base/controller pattern naturally fits; do not delete test/preview hooks just for line count.
5. For every smoke/capture report, build first or verify the latest EXE/current-source view, and show only images generated in the current artifact folder unless explicitly labeled historical/baseline.

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

1. 실제 API key나 수동 transcript가 있으면, LLM XML authoring guide/catalog를 prompt에 넣고 실제 GPT/Gemini/Claude XML correction-loop transcript 1세트를 확보한 뒤 Recipe Manager에서 validate/import 전까지 검증하세요. 실제 transcript를 꾸며내지 마세요.
2. 실제 transcript가 없으면 현재 EXE/current build 기준 Recipe Manager UX를 캡처해서 실제 잘림/겹침/불명확한 next action이 보이는 곳만 개선하세요.
3. 실제 multi-branch recipe에서 현재 branch/output 비교가 부족한 경우에만 branch/output 비교 UX를 확장하세요.
4. Tool View code-behind cleanup은 기존 base/controller 패턴이 자연스럽게 맞을 때만 진행하세요.

최근 검증은 build, screenshot smoke, readiness, external refs, public sample assets, git diff check가 통과했습니다. 다음 작업도 완료는 말이 아니라 실제 명령 통과로 판단해 주세요.
```
