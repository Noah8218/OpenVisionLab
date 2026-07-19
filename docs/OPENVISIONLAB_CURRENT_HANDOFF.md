# OpenVisionLab Current Project Handoff

Updated: 2026-07-19 KST

This is the current continuation brief for a new OpenVisionLab chat. Read it after `AGENTS.md` and before choosing implementation work. It is a status and priority document; it does not override stable behavioral contracts in `AGENTS.md` or `docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`.

## Start Here

Work in Dev first. Do not touch the original repository unless the user explicitly asks to reflect Dev work there.

```powershell
cd C:\Git\OpenVisionLab_Dev
git status --short
git log --oneline -5

Get-Content docs\OPENVISIONLAB_CURRENT_HANDOFF.md -Raw
Get-Content docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md -Raw
Get-Content docs\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md -Raw
```

Then read the LLM guide/catalog and the public-sample, external-reference, and release policies when the next task touches those areas. Use `docs/OPENVISIONLAB_DOCUMENTATION_MAP.md` for the exact reading order and authority rules.

Before any command, code change, or documentation change, state:

1. Current product identity.
2. Immediate priority and remaining project priority.
3. The evidence that supports the choice.
4. What remains out of scope.

## Repository Snapshot At This Handoff

This snapshot was recorded before the documentation-consolidation edits in this session.

| Repository | Branch | Head | Remote state | Meaning |
| --- | --- | --- | --- | --- |
| `C:\Git\OpenVisionLab_Dev` | `codex/public-sample-ux-docs` | `46333dd2` | `origin/codex/public-sample-ux-docs`, `0/0` ahead/behind | Current Dev implementation baseline. |
| `C:\Git\OpenVisionLab` | `main` | `fe8edc9` | `origin/main`, `0/0` ahead/behind | Reviewed import of the Dev baseline. |

- At this snapshot the two repositories contained the same 1,392 tracked files with identical blob hashes.
- Dev had one intentionally untracked user attachment folder: `.codex-remote-attachments/`. Do not stage or delete it as part of normal project work.
- The documentation changes described by this file are Dev-only until the user explicitly requests a new original-repository import, commit, and push.
- The Dev baseline consists of `28695b04` (`Organize vision workflows and source ownership`) followed by `46333dd2` (documentation whitespace cleanup). The original import commit is `fe8edc9`.

## Product Identity

OpenVisionLab is an LLM-assisted OpenCvSharp4 rule-based vision recipe workbench.

Its operator workflow is:

1. Choose or load a sample image.
2. Describe the inspection target, ROI/measurement region, and OK/NG condition.
3. Configure PropertyGrid-based tools directly, or ask an LLM for constrained `VisionPipeline` XML.
4. Validate XML before import.
5. Run Preview or Run only through an explicit user action.
6. Compare layers, metrics, Good/Bad results, failed steps, ROI, templates, and output evidence.
7. Save a reusable recipe and its validation references.

The product is not a camera, lighting, PLC, I/O, account, deployment, MES, or industrial-controller platform. Commercial vision software is useful as a reference for guided configuration, visual result evidence, named recipe management, and explicit validation workflows. It is not a reason to expand into equipment integration.

## Evidence-Based Maturity Statement

Do not use a single percentage as the current release judgement. Older 62-66%, 98%, or other percentages in historical documents are scoped historical estimates, not current release claims.

| User goal | Current evidence | Current limitation | Status |
| --- | --- | --- | --- |
| Operate the workbench | WPF shell, tool rail, PropertyGrid tools, layers, explicit Preview/Run, Pipeline Review, Recipe Manager summary/advanced flow, public samples, and current-EXE smokes are present. | No independent novice usability study or production support workflow has been completed. | Usable for guided sample-backed work. |
| Learn OpenCvSharp concepts | Separate Learn surface covers image/GV, Mat, Point/Rect/ROI, brightness/histogram, arithmetic, filtering, geometry, edge, HSV, Threshold/Morphology, Blob/Contour, matching, pipeline, metrics, and XML authoring. | Learning content is tool-oriented rather than a complete OpenCV course; real learner comprehension has not been measured. | Broad practical curriculum, not a certified training course. |
| Understand rule-based vision | Learn topics, public samples, Tool Views, output layers, metric gates, Good/Bad review, and result explanation connect concepts to observable evidence. | Industrial variability, illumination variation, calibration, and fixture tolerance are not broadly covered by real datasets. | Strong for deterministic examples; partial for field understanding. |
| Review industrial images | Explicit layer routing, ROI/template tools, measurement metrics, fixture translation v1, sample validation, and result review are implemented. | No field-qualified camera/calibration dataset or acceptance campaign; mm results require real calibration evidence. | Constrained verification workbench, not production qualification. |
| Use LLM assistance | XML guide/catalog, local validation/import gates, correction-packet support, prompt packets, and real manually transferred evidence packages exist. | One-shot generation is not reliable enough to promise automatic inspection authoring; cross-provider correction-loop coverage is incomplete. | Guided authoring assistant, not autonomous recipe creation. |

Overall: OpenVisionLab is a feature-rich internal learning and verification workbench with strong deterministic sample evidence. It is not yet evidence-qualified as a production inspection application.

## Completed Product Capabilities

### Core workbench and tool behavior

- Algorithm tools remain PropertyGrid-based. The parameter object assigned to `PropertyGrid.SelectedObject` remains the editing contract.
- The existing viewer capabilities remain: zoom, pan, drag, ROI overlays, template editor, layer comparison, docking, and main-window minimize/maximize/close.
- Pipeline owns Step order, input/output layer routes, acceptance gates, and explicit Preview/Run. Tool Views configure one algorithm. Pipeline Review explains evidence. Recipe Manager organizes reusable recipe units.
- Output-layer creation does not select or rewrite the input layer. Layer visibility, load/create/delete, and output-layer creation do not auto-run Preview or Run.
- Tool rail readiness, compact icon rail behavior, search, Learn/sample links, and bounded Guided Setup entry points have current-source smoke coverage.

### Learn mode and beginner flow

- Learn is separate from working Tool Views. Tool Views remain focused editors and may offer a compact entry to the relevant Learn topic.
- The current curriculum follows the OpenVisionLab tool chain rather than copying a book: image/GV and Mat basics; coordinates and ROI; intensity and histogram; arithmetic; filtering; geometry; edge; color/HSV; binary/morphology; Blob/Contour; template/edge/feature matching; pipeline routing; metrics/Good-Bad validation; XML authoring.
- Learn interactions do not automatically create layers, alter routes, alter recipe values, Preview, or Run. Any apply/open/run action must be explicit.
- P105 kept the Blob topic's repeated `Practice workflow` panel collapsed by default. At the default 1040x700 Learn window, this keeps the `Blob Tool Open` action and its PropertyGrid location guidance fully visible; the learner can still expand the workflow instructions explicitly.

### Recipe Manager and Pipeline Review

- Recipe Manager has a deliberately separated Summary and Advanced Review mode. Summary is the operator entry point; detailed Pipeline, XML/Step, LLM, history, report, validation-set, import/export, and review functions remain available in Advanced Review.
- The primary novice route is explicit and reversible: Recipe Manager summary -> Open Pipeline -> explicit Run Review -> Return to Recipe.
- Guided Setup contains bounded intent starters. It does not silently execute or mutate workspace state.
- Run History supports persisted sample timing analytics and compatible-baseline comparisons only where suite/sample timing evidence is compatible.
- Good/Bad sample matrix, local validation sets, operator decision summary, failed-step navigation, LLM XML validation, and branch/output review are present behind explicit review surfaces.
- P104 made the Summary action visibly state `Next: Open Pipeline` / `다음: 파이프라인 열기` and made it the readable primary action. Current-code EXE evidence is under `artifacts\p104_recipe_summary_next_action_20260717\final_rebuilt_exe_recipe_manager_tabs`.

### Source ownership and MVVM progress

- The 2026-07-17 source organization pass moved cohesive Core, Shell, Docking, Native Tool, Recipe, Pipeline Review, and Vision Test files into explicit ownership folders without changing their public behavior.
- P95-P103 extracted Recipe Manager presentation responsibilities from the large Shell Host into named presenters/builders. P101 owns Guided Workflow presentation; P102 owns lifecycle validation text; P103 owns stored-pipeline XML validation report composition.
- The post-P103 structural audit found no direct C#/XAML files in `1. Core`, `0. UI\0) MENU`, `0. UI\6) Vision Test`, or `0. UI\6) Vision Test\Wpf`. The `0. UI\0) MENU\Wpf` root intentionally retains only the approved Host composition boundary: `OpenVisionShellHostRecipeCommandSurface.cs`, `OpenVisionShellHostView.xaml`, and its code-behind.
- Do not split or move the remaining Host files merely to reduce line count. Extract only a cohesive, independently testable Presenter, Controller, ViewModel, or helper when a real responsibility boundary exists.

### Public samples and deterministic evidence

- The public sample contract currently reports `CatalogRows=30`, `ManifestAssets=229`, and `Pipelines=15`.
- Public sample assets are project-authored/synthetic or otherwise policy-approved. SDK/legacy/private material must not re-enter public catalog, Learn, README, or evidence paths.
- Current validation includes explicit Good/Bad conditions, metric gates, and layer/result evidence rather than success text alone.

### LLM XML authoring and real evidence

- `docs/OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md` and `docs/OPENVISIONLAB_LLM_TOOL_CATALOG.json` define the validated XML contract. The catalog currently names 18 canonical tool families, including the bounded `ReferenceDifference` family added by P144.
- There are 12 self-contained prompt-packet folders under `llm_prompt_packets` and 11 public-safe evidence packages under `docs/evidence/llm`.
- The public evidence inventory contains one manually transferred GPT correction-loop package (`20260715_matching_die_pad_gpt_correction_loop`), nine GPT direct-success packages, and one Gemini direct-success package.
- Manual-transfer packages must state what is unknown: exact provider model/version, API evidence, and a full conversation export were not supplied unless the package says otherwise. Do not invent missing transcript details.
- A direct-success response with zero correction rounds is useful evidence, but it does not prove LLM reliability or a correction loop. Never manufacture a failure/correction round merely to fill the corpus.
- P106 added a fresh user-authorized interactive GPT direct-success trial in the `룰베이스 LLM 연동` project. The actual first response is preserved at `artifacts\p106_gpt_blob_correction_loop_20260717\gpt_first_response.xml`; the current Debug application validated/imported it, accepted the nominal sample at `ResultCount=12`, and rejected the sparse negative sample at `ResultCount=3 < 8`. This is intentionally recorded as direct success only: there was no XML validation failure and therefore no genuine correction response.

- P107 repeated the public nominal/sparse Blob trial in a new user-authorized `룰베이스 LLM 연동` conversation using a natural-language request with no fixed XML parameter values. The first response is at `artifacts\p107_gpt_blob_natural_prompt_20260717\gpt_first_response.xml`; current Debug validation/import and the nominal run passed at its generated exact `ResultCount=12` gate, while the sparse negative correctly returned `ResultCount=3 < 12`. This is direct-success evidence only: no initial XML/Good-Bad failure existed and no correction message was sent.

- P108 added a new user-authorized natural-language GPT RotateScale trial. The raw response at `artifacts\p108_gpt_rotate_scale_natural_prompt_20260717\gpt_first_response.xml` validated/imported, accepted the public nominal image at `ResultImageWidth=286`, and rejected the wide public negative at `ResultImageWidth=320 > 286`. It is direct-success evidence only.
- P109 added a new user-authorized natural-language GPT Matching trial and exposed a Smoke Runner defect. Its raw first response at `artifacts\p109_gpt_matching_natural_prompt_20260717\gpt_first_response.xml` used the correct startup-relative public template path. An initial harness report falsely ran the raw XML after import rather than the imported dependency-rewritten pipeline; the actual GPT correction response is preserved, but it must not be counted as an LLM correction-loop success or an authoring failure. After the runner fix, the unchanged first response validates/imports, passes the nominal image at `ResultCount=3`, and gives the no-target public negative the intended `ResultCount=0 < 3` NG.

- P112 added a user-authorized natural-language GPT HSV trial in a new `룰베이스 LLM 연동` project conversation, independent from P111. Only `HSV_ColorPatch_Synthetic_OK.png` and `HSV_ColorPatch_Synthetic_Missing_NG.png` were sent. The raw first response at `artifacts\p112_gpt_hsv_natural_prompt_20260718\gpt_first_response.xml` validated/imported in the current Debug application, accepted the nominal public image at `MaskPixelRatio=0.058`, and returned the intended missing-target NG at `MaskPixelRatio=0.015 < 0.05`. This is direct-success evidence only: no initial failure existed and no correction message was sent.

- P113 added a user-authorized natural-language GPT FeatureMatching trial in a new project conversation, independent from P111/P112. Only the public feature-card nominal, wrong-card, and template PNGs were sent. The raw first response at `artifacts\p113_gpt_feature_matching_natural_prompt_20260718\gpt_first_response.xml` validated/imported, relocated its two template references, accepted the nominal image at `ScoreMax=96.7`, and returned the imported-pipeline wrong-card NG at `ScoreMax=26.7 < 80`. P114 added expected-NG support to that imported-pipeline smoke route, so both outcomes now report PASS. The raw-only runner's template-not-loaded result is a pre-import relative-path limitation, not a model error. P113 is direct-success evidence only; no correction message was sent.

- P115 added a user-authorized natural-language GPT EdgeBasedMatching trial in a new project conversation, independent from P111-P113. Only public edge-fiducial nominal, wrong-fiducial, and template PNGs were sent. The raw first response at `artifacts\p115_gpt_edge_based_natural_prompt_20260718\gpt_first_response.xml` validated/imported, relocated both template references, accepted the nominal image at `ScoreMax=99.598`, and returned the intended wrong-fiducial NG with `BestScore=61.052 < SCORE_MIN=0.70`. This is direct-success evidence only; no correction message was sent.

- P116 added a user-authorized natural-language GPT Fixture Pad trial in a new project conversation. Its first raw XML named a missing template and omitted the requested fixture-frame behavior, producing a real current-Debug pre-import validation failure. The first correction added the correct three-step fixture workflow but still used startup-unresolvable `docs\...` paths. The second correction changed only both template paths to the current Debug-relative `..\..\docs\...` form; it validated/imported, passed the shifted-pad nominal with fixture evidence, and returned the expected shifted missing-pad NG. This is a real GPT correction-loop success for this current-Debug public workflow, with explicit path-layout limits.

- P117 added a user-authorized natural-language GPT Filter Denoise trial in a new project conversation. Only the public nominal/missing PNG pair was transferred and no XML was supplied. The actual first XML response validated/imported, accepted the nominal at `ResultCount=4`, and returned the intended missing-target NG at `ResultCount=2 < 4`; no correction message was sent. The provider UI showed project-file-library/repository XML retrieval, and the response is semantically identical to the tracked public pipeline after normalizing its two name values. It is therefore project-chat path direct-success evidence, not independent tool-selection or provider-reliability evidence.

- P118 added a user-authorized GPT Morphology Ellipse recovery-correction trial with only the public morphology-cleanup nominal/missing PNG pair. The first raw response used a custom non-OpenVisionLab XML schema and failed current-Debug import with 16 validation errors. The actual same-conversation correction request remained in a provider loading state without correction text; a new recovery project conversation received that exact failed draft and validation report. Its actual response validated/imported, passed nominal `ResultCount=4`, and returned the intended missing-target NG `ResultCount=2 < 4` while preserving `Shape=Ellipse` and `Operator=Open`. This is real cross-conversation recovery-correction evidence, not a same-conversation correction-loop success, independent authoring proof, provider reliability claim, or production-quality proof.

- P119 added a user-authorized GPT Arithmetic Bitwise NOT -> Mean trial with only the public dark-field nominal/bright-field NG PNG pair. The first raw response used a custom XML schema and failed current-Debug import with 12 errors. Its first same-conversation repair validated/imported and passed nominal `MeanValueAvg=208`, but it put `190,230` inside Parameters so the bright negative incorrectly passed at `MeanValueAvg=76.7`; that actual replay result triggered a second repair in the same conversation. The second repair added a Step-level `MeanValueAvg` 190..230 acceptance gate, passed nominal, and returned intended bright-NG `76.7 < 190`. P119 is real same-conversation GPT correction-loop evidence with two actual repairs, not a reliability or production-quality claim.

- P120 added a user-authorized Gemini Arithmetic Bitwise NOT -> Mean trial with the same public dark-field nominal/bright-field NG pair. Gemini's first raw response used custom `Layers`/`Workflow` XML and current-Debug import failed because no Steps were recognized. The actual same-conversation correction used the required child-Step schema and `MeanValueAvg` 190..230 acceptance gate, passed nominal `MeanValueAvg=208`, and returned the intended bright-NG `76.7 < 190`. This is real Gemini same-conversation correction-loop evidence for one public synthetic workflow, not an independent-authoring, reliability, or production-quality claim.

- P121 audited the latest Debug EXE Recipe Manager/LLM XML route after Claude validation was deferred. A focused intent-contract NG showed only `LLM draft validation: NG` in the first visible report area; its mismatch and `Next` guidance required scrolling. The LLM validation panel now gives the first report more height, and intent-contract failure lines precede general result-channel detail. Current-EXE focused and full Recipe Manager smokes passed; no XML execution, import readiness, Preview/Run, layer, or routing behavior changed.

- P122 used an ignored local industrial pin pair only for a non-public `OverlayMerge.SourceSteps` diagnostic. Both inputs ran 5/5 Steps and merged two contour overlays, while the first current-EXE review showed neither of the two declared Step-source producer/consumer relations. The review DTO and Presenter now expose `SourceSteps` alongside `SourceLayers`; the rebuilt EXE shows both relationships, preserves Preview count and active layer, and retains the existing public `SourceLayers` branch review. No local SDK/vendor image was copied, cataloged, documented as a public asset, or sent to an LLM.

- P123 started the next authorized Gemini evidence case with only the public synthetic LineDistance nominal/Wide-Pin pair. Gemini's actual first XML used custom `Configuration`/`PipelineLayers`/`Layer` nodes and current Debug rejected it with `Pipeline has no steps` before import or execution. The exact local failure and constrained child-Step repair contract were sent in the same conversation. Across five actual correction-generation attempts—the initial repair, two visible retries, one concise same-chat retry with no image transfer, and a user-directed 3.5 Flash retry—Gemini rendered no XML; the concise attempt remained text-empty for 80 seconds before it was explicitly stopped, and the verified Flash retry displayed `대답이 중지되었습니다` within 15 seconds. **Status: Blocked** — an actual Gemini correction response is the external prerequisite; no local repair, import, or Good/Bad result was fabricated. Evidence: `artifacts\p123_gemini_line_distance_same_chat_20260718\README.md`.

- **User operating decision (2026-07-18 22:04 KST):** Pause Gemini provider actions until 2026-07-19 01:04 KST after its repeated no-response state. Do not retry or transmit a Gemini prompt/asset before that time unless the user overrides it. Use GPT as the primary provider for the next bounded public-sample validation; Claude remains deferred. The full three-phase plan and provider stop rules are in `docs\OPENVISIONLAB_3_PHASE_DELIVERY_PLAN_20260718.md`.

- **Status: Complete** — P124 extracted the pure LLM default-template `VisionPipeline` construction from `OpenVisionShellHostRecipeCommandSurface` into `Recipe\IntentSkills\OpenVisionRecipeLlmTemplateDraftBuilder`; Host now delegates only the selected template, reference image path, and pin-gap ROI text. A focused smoke verifies LineDistance, Blob, Contour, EdgeBasedMatching, Mean, and Matching starters (including Matching reference parameters) plus the existing validation/dependency/correction-bundle and zero-auto-Preview/Run contracts. Fresh Debug build, focused current-EXE smoke, readiness, and diff check passed. No UI or runtime behavior was intentionally changed. Evidence: `artifacts\p124_llm_template_draft_builder_20260718\README.md`.

- **Status: Complete** — P125 used the user-authorized logged-in GPT web project chat, not an API, with only the public synthetic `Line_Pins_Synthetic_OK.png` and `Line_Pins_Synthetic_WidePin_NG.png` pair. The first rendered `VisionPipeline` XML arrived after a displayed 3-minute-57-second interval and current Debug validated/imported it, but its `MinValue`/`MaxValue` fields did not activate the Step acceptance gates: nominal passed at `DistancePxAvg=28.667`, while Wide-Pin incorrectly passed at `DistancePxAvg=18.417`. The exact Good/Bad evidence and required Step-level acceptance child elements were sent back in the same web conversation. GPT's one repair arrived after a displayed 3-minute-12-second interval, used `UseAcceptanceMetricMinimum/Maximum`, and added a separate `DistancePxMax` guard. Fresh current-Debug replay passed nominal and returned the intended Wide-Pin NG at `DistancePxAvg=18.417 < 24`. This is a real same-conversation GPT correction loop for one public pixel-only LineDistance workflow, not a provider-reliability, calibration, field, or production claim. Evidence: `artifacts\p125_gpt_line_distance_phase1_20260718\README.md`.

- **User product-direction decision (2026-07-18):** OpenVisionLab's next LLM surface should use a no-API browser-assist route: the operator uses a provider web account, then explicitly copies an OpenVisionLab prompt/review packet and pastes XML back for local validation/import. The app must not store credentials, bypass provider account/limit rules, or automate logged-in page input/output. An embedded WebView is optional after this explicit handoff works with an external-browser fallback. Details: `docs\OPENVISIONLAB_3_PHASE_DELIVERY_PLAN_20260718.md`.

- **Status: Complete** — P126 implemented the first no-API Browser Assist slice in Recipe Manager Advanced Review. The new `웹 보조` tab exposes explicit `ChatGPT 열기`, `외부 브라우저`, `프롬프트 복사`, and `XML 붙여넣기` controls. The embedded WebView2 host uses a transient profile and `https://chatgpt.com/` navigates only after the explicit open click; default external-browser fallback is present. It does not automate sign-in, upload, send, response capture, XML import, Preview, or Run. Fresh Debug build (0 warnings/errors) and the current-build smoke passed, including ChatGPT navigation and `PreviewRunCountUnchanged: 0`. Current UI evidence and full boundary: `artifacts\p126_browser_assist_20260718\README.md`.

- **Status: Complete** — P127 closed the first Phase 2 measurement-path check without a product change. Fresh current-Debug `recipe-manager-tabs` verified LineDistance Guided Setup MM-READY and PX-ONLY contracts, invalid-scale blocking, explicit starter creation, and public `Line_Pins_Synthetic_OK/WidePin_NG` Good/Bad outcomes in both modes. The reported unit parity was `DistanceMmAvg=0.224` and `DistancePxAvg=37.263`; the existing range gate prevented an average-only pass. Recipe Manager Good/Bad review, failed-Step navigation, and explicit PropertyGrid apply remained covered. Evidence: `artifacts\p127_phase2_line_distance_operator_path_20260718\README.md`.

- **Status: Complete** — P128 closed the second Phase 2 count-path check without a product change. The public Threshold + Blob pipeline validated/imported, accepted nominal at `ResultCount=12`, and returned the intended sparse NG at `ResultCount=3 < 8`; its report preserves `Blob_Binary -> Blob_Preview`. The separate Blob Learn practice opened the correct Tool View with `PreviewRunCount=0` and `LayerCount=0`. Evidence: `artifacts\p128_phase2_blob_operator_path_20260718\README.md`.

- **Status: Complete** — P129 closed the third Phase 2 Matching path without a product change. The public catalog runtime accepted the Die-Pad nominal image at `ResultCount=3` / `ScoreMax=93.074` and returned the NoTarget NG at `ResultCount=0 < 3`. The valid Debug-relative LLM draft copied both template dependencies into the saved recipe, then passed the same nominal/NG pair; the explicit Matching Tool Preview remained `0 -> 1` and published `Matching_Preview` with three overlays. The catalog XML's raw `docs\...` template path is correctly blocked in the current Debug LLM draft validator, whose relative base is `bin\Debug`; that is the recorded portability boundary, not a resolver change. Evidence: `artifacts\p129_phase2_matching_operator_path_20260718\README.md`.

- **Current LLM workflow maturity:** Level 4 of 5 bounded workflow proof. P125 closes the planned LineDistance first-response failure -> same-web-conversation repair -> current-Debug Good/Bad replay. This does not prove provider reliability, autonomous authoring, field robustness, calibration, deployment, or production readiness.

## Stable Contracts That Must Not Regress

Read the full stable-contract document before touching these areas. At minimum preserve:

- PropertyGrid-based algorithm configuration.
- Explicit Preview/Run only.
- No automatic route/input mutation when output layers are created.
- No execution side effects from visibility, image/layer lifecycle, selection, Recipe Manager navigation, or Learn navigation.
- Viewer, ROI, template, comparison, docking, and main-window controls.
- Recipe Manager/Pipeline/Pipeline Review role separation.
- Public-sample and external-DLL policies.
- `Library\OpenVisionLab.Docking.Controls` ownership of AvalonDock; do not add `Dirkster.AvalonDock` directly to `OpenVisionLab.csproj`.

## Latest Verification Baseline

The latest code baseline was verified after the P105 current-EXE novice workflow audit and its bounded Blob Learn layout correction.

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1
```

Results recorded on 2026-07-17:

- Dev final code build: 0 warnings, 0 errors.
- Latest Debug EXE path: `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`.
- P105 `public-fixture-review` actual-EXE smoke: PASS. It checks the Public Fixture sample, Pipeline Review, explicit pair rerun, XML step edit handoff, and opening Blob Learn without Preview, layer, routing, parameter, or review-state changes.
- P105 `recipe-pipeline-roundtrip` actual-EXE smoke: PASS. It confirms Summary -> Open Pipeline -> explicit Review -> Return without Preview/Run, layer, active-layer, or recipe-route changes.
- P106 current-Debug GPT XML evidence: `llm-xml-draft-file` PASS under `artifacts\p106_gpt_blob_correction_loop_20260717\first_response_nominal`; it proves local XML validation, Recipe Manager import, and nominal `ResultCount=12` acceptance. `llm-xml-image-run --expect-run-success false` PASS under `artifacts\p106_gpt_blob_correction_loop_20260717\first_response_sparse_negative`; it proves the same unmodified XML gives the sparse public negative a genuine inspection NG (`ResultCount=3 < 8`).
- P105 visual audit found one real 1040x700 issue: the initial Blob Learn image showed the related Tool View action partially below the viewport. The collapsed repeated workflow exposes the complete action and guidance in `artifacts\p105_novice_workflow_audit_20260717_214517\after_public_fixture\public_fixture_blob_learn_current_exe.png`; the fresh Recipe Manager summary evidence is in `artifacts\p105_novice_workflow_audit_20260717_214517\after_roundtrip\OpenVisionLab_RecipeManager_Operator_Summary.png`.
- P107 current-Debug natural-prompt GPT XML evidence: `llm-xml-draft-file` PASS under `artifacts\p107_gpt_blob_natural_prompt_20260717\first_response_nominal`; it proves the raw XML validates/imports and accepts the nominal public sample at `ResultCount=12`. `llm-xml-image-run --expect-run-success false` PASS under `artifacts\p107_gpt_blob_natural_prompt_20260717\first_response_sparse_negative`; it proves the same XML gives the sparse public negative the intended inspection NG (`ResultCount=3 < 12`).
- P108 current-Debug natural-prompt GPT XML evidence: `llm-xml-draft-file` PASS under `artifacts\p108_gpt_rotate_scale_natural_prompt_20260717\first_response_nominal`; it validates/imports and accepts the nominal public image at `ResultImageWidth=286`. `llm-xml-image-run --expect-run-success false` PASS under `artifacts\p108_gpt_rotate_scale_natural_prompt_20260717\first_response_wide_negative`; it gives the wide public image the intended NG (`ResultImageWidth=320 > 286`).
- P109 runner-corrected current-Debug Matching evidence: after `OpenVisionLabDirectSmokeRunner.cs` was changed to execute the selected imported pipeline, the unchanged GPT first response passed nominal validation/import/run at `ResultCount=3` under `artifacts\p109_gpt_matching_natural_prompt_20260717\first_response_nominal_after_runner_fix`. The no-target replay at `first_response_negative_after_runner_fix` intentionally causes the nominal-success smoke command to exit nonzero, but its report proves the imported pipeline's intended product NG (`ResultCount=0 < 3`). The unmodified raw-response failure report and subsequent GPT correction response are retained as confounded provenance only.
- Readiness: PASS.
- External references: PASS.
- Public sample assets: PASS, `30/229/15`.
- `git diff --check`: PASS after the active handoff update.
- Original import verification after sync: 0 warnings, 0 errors; readiness, external-reference, and public-sample checks passed; original and Dev tracked trees were equal.

For any later UI work, build after source changes and capture new before/after evidence in a new artifact folder. Do not present historical P104 images as the UI after a later source change.

## P110 Real GPT Pin-Gap Correction Attempt (2026-07-17)

- P110 used only public nominal/wide-gap pin images in a user-authorized GPT project chat. The natural first XML validated/imported and returned the expected partial Good/Bad results, but it created only four executable LineDistance Steps (Gaps 1-2) and replaced Gaps 3-7 with a literal placeholder comment. That is a real operator-intent scope failure.
- The exact missing-coverage evidence was returned to GPT in the same conversation. Two requests ended in provider `thinking failed`; a final 5m 39s attempt produced a real 14-Step correction XML with Average and `DistanceMmRange` gates for all seven gaps.
- The correction validated/imported, but the nominal current-Debug run failed at `12 Pin Gap 6 Range`: `DistanceMmRange=0.024 > 0.02`. Evidence: `artifacts\p110_gpt_pin_gap_natural_prompt_20260717\corrected_nominal\report.txt`.
- P110 is real correction-attempt failure evidence, not a successful correction loop. Preserve both raw responses and do not score a correction success from it.

## P111 Real GPT Pin-Gap Gate-Repair Correction Success (2026-07-18)

- P111 continued the same user-authorized P110 GPT project conversation without transferring any new images or private files. The exact current-Debug P110 nominal failure was supplied: `12 Pin Gap 6 Range` rejected `DistanceMmRange=0.024` against a `0.02` maximum.
- The actual XML-only response is `artifacts\p111_gpt_pin_gap_gate_repair_20260718\gpt_second_correction_response.xml`. It retains 14 executable LineDistance Steps (Average plus `DistanceMmRange` for each of seven adjacent gaps) and no placeholder. Its only XML delta from the prior 14-Step correction is the affected range maximum `0.02 -> 0.03`.
- After a fresh `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" -v:q` passed with 0 warnings and 0 errors, current Debug `llm-xml-draft-file` validation/import and nominal replay passed. The final Step was `14 Pin Gap 7 Range`; Step 12 accepted `DistanceMmRange=0.024`.
- Current Debug `llm-xml-image-run --expect-run-success false` passed for the supplied wide-pin public negative. The product returned expected NG at `01 Pin Gap 1 Average`: `DistanceMmAvg=0.116 < 0.14`.
- P111 is a real GPT correction-loop success for this one public synthetic workflow: first-response scope failure, a first correction that failed nominal replay, exact local failure evidence, second correction response, and current-Debug nominal/NG replay. It is not a provider reliability or production-quality claim. Full provenance: `artifacts\p111_gpt_pin_gap_gate_repair_20260718\README.md`.

## P112 Independent GPT HSV Natural-Authoring Direct Success (2026-07-18)

- P112 used a new user-authorized GPT project chat, independent from the P111 pin-gap conversation. Only the public nominal/missing-target HSV color-patch PNG pair was attached; the prompt requested a complete HSV `VisionPipeline` with a measurable `MaskPixelRatio` gate and did not provide XML, source, private images, or API credentials.
- The raw XML-only first response is `artifacts\p112_gpt_hsv_natural_prompt_20260718\gpt_first_response.xml`. After a fresh zero-warning/zero-error Debug build, current Debug `llm-xml-draft-file` validated/imported it and accepted the nominal image at `MaskPixelRatio=0.058`. Evidence: `nominal_first\report.txt`.
- Current Debug `llm-xml-image-run --expect-run-success false` passed for the public missing-target image: the product returned the intended NG at `01 Red Color Coverage`, `MaskPixelRatio=0.015 < 0.05`. Evidence: `missing_negative_expected_ng\report.txt`.
- P112 is an independent real GPT direct success in a different tool family, not a correction loop. No correction prompt was manufactured or sent because the first response had no XML, import, nominal, or intended-negative failure. Provider model/version, API evidence, and a complete provider export remain unknown.

## P113 Independent GPT FeatureMatching Natural-Authoring Direct Success (2026-07-18)

- P113 used a new user-authorized GPT project chat, independent from P111 and P112. Only public nominal/wrong-card/template feature-card PNGs were attached; the natural prompt requested one FeatureMatching `VisionPipeline` with a template dependency and a measurable `ScoreMax` acceptance range. No XML, source, private image, or API credential was sent.
- The raw XML-only first response is `artifacts\p113_gpt_feature_matching_natural_prompt_20260718\gpt_first_response.xml`. On the fresh current Debug build, `llm-xml-draft-file` validated/imported it, copied both relative template references into the smoke recipe, and accepted the nominal image at `ScoreMax=96.7`. P114 report: `artifacts\p114_imported_expected_ng_runner_20260718\p113_nominal\report.txt`.
- The imported-pipeline wrong-card replay produced the intended NG at `01 Synthetic Feature Card Match`, `ScoreMax=26.7 < 80`. P114 adds `--expect-run-success false` to that same import route, so the expected product NG now reports PASS. Evidence: `artifacts\p114_imported_expected_ng_runner_20260718\p113_wrong_expected_ng\report.txt`.
- `llm-xml-image-run --expect-run-success false` still does not import/rewrite the raw relative template path first and therefore reports template-not-loaded rather than the semantic wrong-card result. This is a runner-path limitation, not a GPT authoring failure; it was not supplied to GPT and must not be counted as a correction trigger. Use the P114 imported-pipeline route for template-dependent LLM XML Good/Bad evidence.
- P113 is independent real GPT direct-success evidence in a template-dependent tool family, not a correction loop. No correction prompt was sent. Provider model/version, API evidence, and a complete provider export remain unknown.

## P114 Imported LLM-Draft Expected-NG Smoke Support (2026-07-18)

- P113 exposed an evidence-only runner mismatch: `llm-xml-draft-file` correctly imports/re-writes relative template dependencies, but before P114 it treated every image NG as a smoke failure; `llm-xml-image-run` accepts expected NG but runs the raw draft without importing/re-writing those dependencies.
- `OpenVisionLabDirectSmokeRunner.cs` now accepts `--expect-run-success true|false` for `llm-xml-draft-file`, defaulting to `true`. With an image it reports expected and actual run success while still requiring XML validation and Recipe Manager import. No operator product behavior, UI, XML format, layer, routing, Preview, or Run behavior changed.
- Fresh zero-warning/zero-error Debug build passed. The raw P113 GPT XML now passes the imported nominal replay (`ExpectedRunSuccess=True`, `ActualRunSuccess=True`, `ScoreMax=96.7`) and imported expected-NG replay (`ExpectedRunSuccess=False`, `ActualRunSuccess=False`, `ScoreMax=26.7 < 80`). Evidence: `artifacts\p114_imported_expected_ng_runner_20260718\README.md`.

## P115 Independent GPT EdgeBasedMatching Natural-Authoring Direct Success (2026-07-18)

- P115 used a new user-authorized GPT project chat, independent from P111-P113. Only public edge-fiducial nominal/wrong/template PNGs were attached; the natural prompt requested one EdgeBasedMatching `VisionPipeline` with a template dependency and a measurable `ScoreMax` acceptance range. No XML, source, private image, or API credential was sent.
- The raw XML-only first response is `artifacts\p115_gpt_edge_based_natural_prompt_20260718\gpt_first_response.xml`. After a fresh zero-warning/zero-error Debug build, current Debug `llm-xml-draft-file` validated/imported it, relocated both template references into the smoke recipe, and accepted the nominal image at `ScoreMax=99.598`. Evidence: `nominal_after_import\report.txt`.
- The imported-pipeline wrong-fiducial replay with `--expect-run-success false` returned the intended NG: no result exceeded `SCORE_MIN=0.70` and the measured best score was `61.052`. It recorded `ExpectedRunSuccess=False`, `ActualRunSuccess=False`, and `Result: PASS`. Evidence: `wrong_expected_ng_after_import\report.txt`.
- P115 is independent real GPT direct-success evidence in the EdgeBasedMatching tool family, not a correction loop. No correction prompt was sent because the first response had no XML/import/nominal/intended-negative failure. Provider model/version, API evidence, and a complete provider export remain unknown.

## P116 Real GPT Fixture Pad Correction-Loop Success (2026-07-18)

- P116 used a new user-authorized GPT project chat and only public shifted-pad nominal/missing PNGs plus the public locator template. The natural request required locator-based fixture handling, pad inspection in the part-relative region, a combined review output, nominal acceptance, and missing-pad rejection.
- The raw first response is `artifacts\p116_gpt_fixture_pad_natural_prompt_20260718\gpt_first_response.xml`. Current Debug `llm-xml-draft-file` rejected it before import because both template references named non-existent `Locator_Template.png`. Its Matching step also did not declare a fixture frame and its pad inspection used only a static ROI, so it missed the requested shifted part-relative intent. Evidence: `first_nominal\report.txt`.
- The exact validation/intent evidence was sent to GPT in the same conversation. Its first correction supplied the fixture-aware public workflow but still failed the current Debug validator because `docs\...` relative dependencies are resolved from `bin\Debug`. The first correction is a real failed intermediate response, not a success. Evidence: `correction_nominal\report.txt`.
- The second GPT correction changed only `TemplatePath` and `PATTERN_PATH` to `..\..\docs\samples\public\templates\Fixture_Locator_Synthetic_Template.png`. Current Debug validation/import and nominal replay passed: locator fixture offset `(80,55)`, effective pad ROI `(400,235)`, and three accepted Matching/Blob/OverlayMerge Steps ending at `FixtureReview`. Evidence: `second_correction_nominal\report.txt`.
- The unchanged second correction also passed `--expect-run-success false` for the public shifted missing-pad sample. Matching passed, then `02 Inspect Fixture Pad` returned the intended `ResultCount=0 < 1` NG. Evidence: `second_correction_missing_expected_ng\report.txt`.
- P116 is a real GPT correction-loop success for one public synthetic current-Debug workflow: initial XML/intent failure, actual correction response, actual failed intermediate correction, second correction response, and current Debug nominal/NG replay. The final `..\..\docs` dependency path is specific to the current Debug startup layout; do not generalize it to a portable deployment path or provider reliability claim.

## P117 GPT Filter Denoise Project-Chat Direct Success (2026-07-18)

- P117 used a new user-authorized GPT project chat and transferred only `Filter_Denoise_Synthetic_OK.png` and `Filter_Denoise_Synthetic_Missing_NG.png`. The natural request specified the operator intent (remove small bright noise, then count four large targets), required a sequential Filter -> Threshold -> Contour route, and supplied neither XML, source, private assets, API credentials, nor template paths.
- The actual first XML-only response is `artifacts\p117_gpt_filter_denoise_natural_prompt_20260718\gpt_first_response.xml`. The provider's `응답 복사` text and saved file match character-for-character. The provider reported `3m 4s` and its UI showed project-library/repository XML retrieval before returning the XML; exact provider model/version, API evidence, and a complete provider export remain unknown.
- After a fresh zero-warning/zero-error Debug build, `llm-xml-draft-file` validated/imported the first response and passed the nominal image: Filter, Threshold, and Contour all accepted, with final `Filter_Denoise_Preview` and `ResultCount=4`. Evidence: `first_nominal\report.txt`.
- The unchanged first response passed `--expect-run-success false` on the public missing-target image. Filter and Threshold accepted; the final Contour returned intended NG `ResultCount=2 < 4`. Evidence: `first_missing_expected_ng\report.txt`.
- Canonical XML comparison is identical to `docs\samples\public\Public_Filter_Denoise.pipeline.xml` after normalizing only pipeline name `Filter_Denoise_Inspection -> Public_Filter_Denoise` and its Contour `Name` parameter `GPT_Filter_Denoise -> Public_Filter_Denoise`. No correction was requested because there was no real XML/import/nominal/intended-negative failure. P117 is real project-chat direct-success path evidence, not independent authoring, provider reliability, or production-quality proof. Full provenance: `artifacts\p117_gpt_filter_denoise_natural_prompt_20260718\README.md`.

## P118 GPT Morphology Ellipse Recovery-Correction Evidence (2026-07-18)

- P118 used a new user-authorized GPT project-chat workflow and transferred only `Morphology_Cleanup_Synthetic_OK.png` and `Morphology_Cleanup_Synthetic_Missing_NG.png`. The natural prompt required Threshold -> elliptical-kernel Morphology Open -> Contour count, nominal four-target acceptance, missing-target NG rejection, and prohibited use of project/repository XML. No XML, source, private asset, API credential, template path, or hardware data was sent.
- The actual first response is `artifacts\p118_gpt_morphology_ellipse_natural_prompt_20260718\gpt_first_response.txt`. It used `InputLayers`/`OutputLayers`/`BinaryThreshold`/`ConnectedComponents`/`AcceptanceGate` custom XML rather than OpenVisionLab child Step fields. Current Debug reported `ValidationOk=False`, `ImportEnabled=False`, and `Imported=False`, with 16 schema errors; image execution was skipped. This is a real initial provider XML-schema failure, not an expected NG-sample result. Evidence: `first_nominal\report.txt`.
- The actual same-conversation failure report was sent back using `gpt_correction_prompt.txt`, but the provider UI remained in a loading state for about five minutes and returned no correction text. Record this as an unreceived provider UI/hang observation, not an absent model response or a correction failure.
- A new recovery project conversation received the exact initial draft plus actual validation report through `gpt_retry_correction_prompt.txt`. Its completed response is preserved at `gpt_retry_correction_response.xml` (the artifact appends only one terminal newline because the provider copy action exposed no browser clipboard payload). It declares Threshold -> Morphology -> Contour, `Shape=Ellipse`, `Operator=Open`, and a final `ResultCount` minimum/maximum of four.
- On the fresh current Debug build, the recovery response validated/imported and passed the nominal sample: three Steps accepted and the final Contour returned `ResultCount=4`. Evidence: `retry_correction_nominal\report.txt`. The unchanged recovery response passed `--expect-run-success false` on the public missing-target image: the final Contour returned intended `ResultCount=2 < 4`, with `ExpectedRunSuccess=False`, `ActualRunSuccess=False`, and smoke `Result: PASS`. Evidence: `retry_correction_missing_expected_ng\report.txt`.
- P118 is real GPT recovery-correction evidence across two project conversations: actual initial response, actual local validation failure, actual recovery correction response, and current-Debug nominal/NG replay. It is not a pure same-conversation correction-loop completion and must not be generalized to independent authoring, provider reliability, or production quality. Full provenance: `artifacts\p118_gpt_morphology_ellipse_natural_prompt_20260718\README.md`.

## P119 GPT Arithmetic/Mean Same-Conversation Correction Evidence (2026-07-18)

- P119 used a new user-authorized GPT project chat and transferred only `Arithmetic_Invert_Synthetic_OK.png` and `Arithmetic_Invert_Synthetic_Bright_NG.png`. The natural request required `Arithmetic` Bitwise NOT followed by `Mean`, a distinct intermediate output layer, nominal acceptance after inversion, bright-image NG rejection after inversion, and no project/repository XML retrieval. No XML, source, private asset, API credential, template path, or hardware data was sent.
- The actual first rendered response, `artifacts\p119_gpt_arithmetic_mean_same_chat_20260718\gpt_first_response.xml`, used a custom `Layers`/`ImageLayer`/`BitwiseInvert`/`AverageBrightness`/`AcceptanceGate` schema. Fresh current-Debug validation reported `ValidationOk=False`, `ImportEnabled=False`, and `Imported=False` with 12 child-Step-contract errors; image execution was skipped. This is a real provider XML-schema failure, not an expected sample NG. Evidence: `first_nominal\report.txt`.
- The actual failure report was returned in the same provider conversation using `gpt_correction_prompt.txt`. The first repair, `gpt_correction_response.xml`, used the correct two-Step XML shape and passed nominal import/run at `MeanValueAvg=208`, but it put `MeanValueAvg=190,230` into `Parameters`. The bright expected-NG replay therefore incorrectly passed at `MeanValueAvg=76.7` (`ExpectedRunSuccess=False`, `ActualRunSuccess=True`). This is an actual acceptance-contract defect, not a schema or harness failure. Evidence: `correction_nominal\report.txt` and `correction_bright_expected_ng\report.txt`.
- That exact current-Debug bright replay result was sent back in the same conversation through `gpt_second_correction_prompt.txt`. The completed second repair, `gpt_second_correction_response.xml`, retained `Arithmetic` Bitwise NOT -> `Mean`, removed the range parameter, and added `UseAcceptance`, `AcceptanceMetricName=MeanValueAvg`, and a 190..230 Step-level range.
- On the fresh current Debug build, the second repair validated/imported and passed the nominal sample at `MeanValueAvg=208`. The unchanged repair also passed `--expect-run-success false` on the bright public image: `MeanValueAvg=76.7 < 190`, `ExpectedRunSuccess=False`, `ActualRunSuccess=False`, and smoke `Result: PASS`. Evidence: `second_correction_nominal\report.txt` and `second_correction_bright_expected_ng\report.txt`.
- P119 is real GPT same-conversation correction-loop evidence: actual first provider response, actual local validation failure, actual first repair with a real Good/Bad acceptance defect, actual second repair, and fresh current-Debug nominal/NG replay. Provider model/version, API transcript, and full provider export remain unknown; rendered-response capture adds only one terminal newline. It is not a general provider-reliability, independent-authoring, deployment-portability, or production-quality claim. Full provenance: `artifacts\p119_gpt_arithmetic_mean_same_chat_20260718\README.md`.

## P120 Gemini Arithmetic/Mean Same-Conversation Correction Evidence (2026-07-18)

- P120 used the user-authorized logged-in Gemini chat and transferred only `Arithmetic_Invert_Synthetic_OK.png` and `Arithmetic_Invert_Synthetic_Bright_NG.png`. The in-app browser cannot use native file-picker upload and clipboard attachments share the name `clipboard.png`, so the nominal image was sent first with an explicit no-XML staging request, then the bright image and actual XML request were sent in the immediately following turn. This delivery detail is preserved in `gemini_nominal_staging_prompt.txt` and `gemini_first_prompt.txt`; it does not add source, XML, private asset, credential, template path, or hardware data.
- The actual initial rendered response, `artifacts\p120_gemini_arithmetic_mean_same_chat_20260718\gemini_first_response.xml`, used custom `Layers`/`Layer`/`Workflow` and attribute-Step nodes. Current Debug reported `ValidationOk=False`, `ImportEnabled=False`, and `Imported=False`; the only schema error was `Pipeline has no steps`, so image execution was skipped. This is a real provider XML-schema failure, not an expected sample NG. Evidence: `first_nominal\report.txt`.
- The exact current-Debug failure result and child-Step repair contract were returned in the same Gemini conversation using `gemini_correction_prompt.txt`. The completed correction, `gemini_correction_response.xml`, declares `Arithmetic` Bitwise NOT from `Main` to `InvertedOutputLayer`, then `Mean` to `MeanOutputLayer`, and a Step-level `MeanValueAvg` 190..230 acceptance range.
- On the fresh current Debug build, that correction validated/imported and passed nominal at `MeanValueAvg=208`. The unchanged correction passed `--expect-run-success false` on the bright public image: final Mean returned intended NG `MeanValueAvg=76.7 < 190`, `ExpectedRunSuccess=False`, `ActualRunSuccess=False`, and smoke `Result: PASS`. Evidence: `correction_nominal\report.txt` and `correction_bright_expected_ng\report.txt`.
- P120 is real Gemini same-conversation correction-loop evidence: actual first provider response, actual local validation failure, actual same-conversation correction response, and fresh current-Debug nominal/NG replay. The repair prompt supplied exact child-Step and range contracts; this is correction-path evidence for one public synthetic workflow, not independent tool-selection, general provider reliability, deployment portability, or production-quality evidence. The visible provider mode label was `Gemini Pro`; exact provider model/version, API transcript, and full provider export remain unknown. Full provenance: `artifacts\p120_gemini_arithmetic_mean_same_chat_20260718\README.md`.

## P121 LLM Assistant Failure Next-Action Visibility (2026-07-18)

- With no newly authorized provider transcript and Claude deliberately deferred by the user, P121 audited the current Debug EXE Recipe Manager LLM XML route rather than fabricating another LLM interaction. The focused `Pin gap / edge distance` intent mismatch correctly blocked a Contour draft, but its first visible `LLM 초안 검증: NG` panel ended before the actual error and `Next` guidance. The operator could see failure but had to scroll to learn the required `LineDistance` correction.
- `OpenVisionShellHostView.xaml` now assigns a larger first row to the LLM draft validation/dependency review area; detailed draft review, diff, stored validation, and issue-list areas remain in the same explicitly scrollable review surface. `OpenVisionRecipeLlmDraftValidationService` now writes intent-contract error/tool-type/next-action lines before general result-channel explanation. No XML parsing, validation result, import gate, tool choice, Preview/Run, layer, routing, or recipe mutation behavior changed.
- Fresh Debug build completed with 0 warnings and 0 errors. The after focused EXE screenshot visibly shows `Error: Intent contract mismatch`, `Draft enabled ToolTypes`, and `Next: Use ToolType=LineDistance...` without scrolling. Evidence: `artifacts\p121_llm_assistant_ux_audit_20260718\focused_exe_llm_intent_skills\OpenVisionLab_RecipeManager_LlmIntentSkills_PinGapContourMismatch.png` and `after_final_exe_llm_intent_skills\OpenVisionLab_RecipeManager_LlmIntentSkills_PinGapContourMismatch.png`.
- The same current Debug EXE passed `recipe-manager-llm-intent-skills` and the full `recipe-manager-tabs` smoke. The latter preserves normal LLM XML validation/import review, guided intent skills, blocked invalid paths/tools/parameters, correction bundle, corrected-draft import, and explicit Preview/Run behavior. Normal after evidence: `after_final_exe_recipe_manager_tabs\OpenVisionLab_RecipeManager_LlmXml.png`. Full provenance: `artifacts\p121_llm_assistant_ux_audit_20260718\README.md`.

## P122 Local SourceSteps Branch-Comparison Evidence (2026-07-18)

- P122 used the ignored root `Sample` directory only as a local industrial-image diagnostic; its images were not copied into `artifacts`, a public catalog, documentation capture, GitHub-bound file, or any LLM/provider prompt. A five-Step local Pin pipeline had two Contour branches and a final `OverlayMerge` whose `SourceSteps` names were `03 Pin Top Contour;04 Pin Bottom Contour`.
- The current Debug runner executed both local inputs successfully with `Steps=5/5`, `MergeSourceCount=2`, and `MergeOverlayCount=2`. The first current-EXE Recipe Manager branch review imported the same XML but reported `SourceConsumerRelationsVisible: 0/2` and `OverlaySourceProducersVisible: 0/2`; it treated the merge as a same-input candidate rather than its two declared Step-source relationships.
- `OpenVisionRecipePipelineStepPreview` now parses `SourceSteps`, and the branch/output Presenter resolves declared Step names alongside declared output layers for same-input exclusion, input producers, output consumers, review merges, and overlay sources. The rebuilt EXE reports `2/2` for both source-consumer and overlay-source relations. Preview count stayed unchanged and the active layer stayed unchanged. Before/after EXE evidence: `artifacts\p122_local_sourcesteps_pin_branch_20260718\before_exe_sourcesteps\OpenVisionLab_PipelineReview_SourceConsumer.png` and `after_exe_sourcesteps\OpenVisionLab_PipelineReview_SourceConsumer.png`.
- Existing public `SourceLayers` behavior remains current-EXE verified at `2/2` producer/consumer relations with `BentPin_TopBottom_Overlay.pipeline.xml`; full `recipe-manager-tabs`, readiness, external-reference, public-sample, and diff checks passed. Full provenance and local-asset boundary: `artifacts\p122_local_sourcesteps_pin_branch_20260718\README.md`.

## P131 Approved Local Bent-Pin Field-Pilot Result (2026-07-18)

- The user explicitly approved the local Bent Pin contract in this chat: bent-pin shaft-width inspection, ROI `20,65,728,175`, pixel-only `BoundsWidthMax <= 18` gate, and the selected Good/bent-NG labels. No mm, calibration, public-sample, provider, or production-deployment claim is included.
- The current Debug executable workspace now contains the saved local recipe `FieldPilot_BentPin` with active pipeline `BentPin_ShaftContour`, a two-row recipe-local validation set, local result/overlay evidence, and a field-pilot handoff note. The saved pipeline SHA-256 is `B817FC09093AF30A77AB7AA5A96436FA8884D79ACD3FDE7DD6089D3E11E46D82`, matching the approved source recipe.
- Fresh replay used the saved recipe file itself. The approved Good row returned `Success=True`, `ResultCount=13`, `BoundsWidthMax=14`; the approved bent NG row returned the controlled expected `Success=False` because `BoundsWidthMax=26 > 18`. Both final layers contained 13 overlays. Local record: `bin\\Debug\\RECIPE\\FieldPilot_BentPin\\FIELD_PILOT_HANDOFF.md` and its `EVIDENCE` folder.
- This completes Phase 3 only for the agreed local two-image workbench scope. It does not establish production robustness, physical-unit accuracy, equipment integration, or broad model reliability.

## P132 Clean Runtime Direct-Contour Replay (2026-07-19)

- P132 repeated the failure before changing code: the retained `bin\Debug\OpenVisionLab.exe --smoke llm-xml-image-run` and `dotnet bin\Debug\OpenVisionLab.dll` both terminated with `-1073741819` in `OpenCvSharp.NativeMethods.imgproc_findContours1_vector`; no smoke report was written. Moving image execution to a worker, creating a WPF Application, and temporarily hiding `opencv_world430.dll` did not repair it, so none of those speculative changes was retained.
- The decisive comparison was execution output, not source or vendored DLL content. `OpenCvSharp.dll` and `OpenCvSharpExtern.dll` hashes matched the passing tool outputs, but the retained `bin\Debug` folder held many legacy SDK/runtime files. The same current `OpenVisionLab.exe` copied into the clean `VisionRecipeRunnerSmoke` output passed immediately. A new empty `OutputPath` build also passed.
- Added `tools\BuildCleanRuntime.ps1`. It only builds into a new directory under `artifacts`, rejects an existing or out-of-artifacts destination, verifies the required app/OpenCV/PropertyGrid files, and writes `clean_runtime_manifest.json` with SHA-256 values. It does not delete, move, or rewrite the retained `bin\Debug` workspace or its local recipe evidence.
- The script-built current EXE passed `llm-xml-image-run` against the approved saved Bent Pin recipe: Good returned `ActualRunSuccess=True`, `ResultCount=13`, `BoundsWidthMax=14`; the bent input returned the expected inspection NG `ActualRunSuccess=False`, `BoundsWidthMax=26 > 18`, while both smoke commands returned `Result: PASS`. Current-source WPF Contour Tool and Pipeline Review controls also passed. Evidence: `artifacts\p132_direct_smoke_contour_host_20260719\clean_runtime_script_final\clean_runtime_manifest.json`, `script_runtime_good\report.txt`, `script_runtime_bad_expected_ng\report.txt`, and `control_wpf_contour_current_source\wpf_shell_host_contour_tool.png`.
- `PL-0001` is resolved for current LLM XML replay evidence through the clean-runtime builder. P132's output-location decision was completed by the user-approved P133 contract below.

## P133 Approved Dev/Release Runtime Output Contract (2026-07-19)

- The user approved this contract without a `bin\Debug` migration: Dev evidence uses a new `artifacts\openvisionlab_clean_runtime_<timestamp>` directory; the release package uses a new `dist\OpenVisionLab` directory; the retained `bin\Debug` folder remains a local recipe workspace.
- `tools\BuildCleanRuntime.ps1` now has explicit `Dev` and `Release` modes. Dev builds a timestamped Debug runtime under `artifacts`; Release publishes a Release runtime only to `dist\OpenVisionLab`. Both reject an existing destination, and Dev rejects a release-root destination.
- The P133 Dev runtime manifest is `artifacts\openvisionlab_clean_runtime_20260719_004600\clean_runtime_manifest.json`. The P133 Release manifest is `dist\OpenVisionLab\clean_runtime_manifest.json`; each records the required managed/native runtime hashes.
- The Dev and Release EXEs both validated and replayed the approved saved Bent Pin XML. Good returned `ActualRunSuccess=True`, `ResultCount=13`, `BoundsWidthMax=14`; bent input returned the expected inspection NG `ActualRunSuccess=False`, `BoundsWidthMax=26 > 18`. Both smoke commands reported `Result: PASS`.
- The Release EXE also passed `recipe-manager-tabs`, including Summary/Advanced Review, validation-suite, LLM review/import guards, branch/output review, and explicit Preview/Run behavior. Evidence is under `artifacts\p133_clean_runtime_output_contract_20260719`.
- The retained local recipe file remains unchanged at SHA-256 `B817FC09093AF30A77AB7AA5A96436FA8884D79ACD3FDE7DD6089D3E11E46D82`. `PL-0002` is resolved for the approved runtime-output contract. This does not prove that startup-relative LLM template paths work from the release package.

## P134 Release Template-Dependency Import Contract (2026-07-19)

- The Release `dist\OpenVisionLab\OpenVisionLab.exe` correctly blocked the public catalog Matching XML when it used `docs\samples\...` dependency values. The report recorded two missing dependencies and disabled Import; this is expected because catalog paths are repository references, not packaged assets.
- The same public Matching workflow passed when its `TemplatePath` and `PATTERN_PATH` values named an existing operator-accessible public template file. Import copied both dependency values into the Release recipe `Template` folder, then the imported pipeline accepted the public Good image at `ResultCount=3`, `ScoreMax=93.074` and returned the intended no-target NG at `ResultCount=0 < 3`.
- The validator and Import service required no code change: they already reject unavailable paths, copy verified image dependencies on Import, and update the imported XML to the copied files. P134 changed only the LLM authoring guide so it no longer presents `docs\samples\...` as a packaged LLM-draft path.
- Evidence: `artifacts\p134_release_template_portability_20260719\before_catalog_relative_path_blocked\report.txt`, `release_operator_path_good\report.txt`, and `release_operator_path_expected_ng\report.txt`.
- Historical P134 boundary: it proved dependency relocation only inside the current Release installation. P137 below supersedes the untested cross-install portion for copied template paths.

## P135/P136/P137 Approved Evidence Expansion (2026-07-19)

- P135 added a second, local-only EdgeBasedMatching validation slice from the approved root `Sample` folder. The L-fiducial Good image passed with `ResultCount=1` and `ScoreMax=99.991`; the explicitly named `NoTarget` image returned the intended NG because its best score was `57.502 < 0.70`. The source images and all derived artifacts remain local under `artifacts\p135_local_edge_fiducial_20260719_100236`; they are not public-sample or provider material.
- P136 is a new, user-authorized same-conversation GPT correction loop using only the public Die Pad image and public template. The actual first GPT XML parsed and met the tool/acceptance shape, but `TemplatePath` and `PATTERN_PATH` used the attachment filename and both dependencies were missing. Current clean-Dev validation therefore returned `ValidationOk=False` and `ImportEnabled=False`. GPT's same-chat correction changed both values to the verified public accessible path; current clean-Dev validation/import copied both dependencies, and explicit execution passed at `ResultCount=3`, `ScoreMax=93.074`. Preserve raw files only under `artifacts\p136_gpt_matching_correction_20260719_103200`; do not publish the local-path correction response without a separate sanitization and inclusion approval.
- P137 makes imported template dependencies portable across a moved package. `OpenVisionRecipeDependencyReviewService` now stores copied template values relative to `AppPathService.StartupPath`; `VisionPipelineAppToolFactory` resolves such values from that root for Matching, EdgeBasedMatching, and FeatureMatching. A freshly published Release package was copied to `artifacts\p137_cross_install_20260719_100614\relocated_install` and ran only package-contained template/sample paths: Matching passed `ResultCount=3`, EdgeBasedMatching passed `ScoreMax=99.598`, and FeatureMatching passed `ScoreMax=96.7`. The copied EXE SHA-256 matched the original Release EXE.
- P137 evidence: `artifacts\p137_cross_install_20260719_100614\import_relative_path_current_dev\report.txt`, `relocated_install\P137_RelocationRun\report.txt`, `relocated_install\P137_RelocationRun_edge\report.txt`, and `relocated_install\P137_RelocationRun_feature\report.txt`.

## P138 Current-EXE LLM Template-Path Guidance Audit (2026-07-19)

- P138 rechecked the LLM Assistant after the real P136 attachment-filename failure without changing product code. A newly built clean Dev EXE reproduced the initial XML's controlled validation failure: two named missing dependency keys, Import disabled, and an explicit instruction to replace paths with verified files before import.
- The dedicated LLM Intent Skills EXE smoke passed. Its current capture shows the analogous intent-contract failure reason and `Next` action in the first validation panel viewport, with no clipped button text or overlapping controls. The targeted missing-dependency runner's final screenshot returned to the shell after recording the report, so that shell-only image is not used as proof of the panel layout.
- No verified visible clipping, overlap, or unclear next action justified a UI change. Evidence: `artifacts\p138_llm_template_path_guidance_audit_20260719_103100\before_current_exe\report.txt` and `current_exe_llm_intent_skills\report.txt`.

## P139 Public Product-Catalog Template-Path Regression Repair (2026-07-19)

- The user's request to use the industrial samples first exposed that the public-safe product catalog is larger than the older README claim: it has 184 current rows (84 Required Good, 84 ExpectedFailure Bad, and 16 Explore field-style rows). The README now states the current total; the product quality audit found all 84 Good/Bad pairs separated without review or critical flags.
- The first current-source P139 catalog run passed 164/184 rows but failed all 20 Matching, EdgeBasedMatching, and FeatureMatching rows with missing template images. This was a regression from P137: the new resolver only searched `AppPathService.StartupPath`, while the developer catalog deliberately uses repository-relative `docs\samples\...` paths from the recipe runner working directory.
- `VisionPipelineAppToolFactory.ResolveTemplatePath` now keeps the portable-package rule first (resolve against `AppPathService.StartupPath` when that file exists) and otherwise preserves the former current-working-directory relative-path behavior. Absolute paths are unchanged. The fixed current-source catalog passed all 184 rows; public-asset and external-reference checks also passed.
- A freshly built clean Dev runtime was copied to a new artifact root and launched from that copied root with only package-contained `RECIPE` templates and test images. Matching, EdgeBasedMatching, and FeatureMatching all passed again; the copied EXE SHA-256 matched the clean-runtime EXE. This is a current Debug-runtime relocation regression check, not a new installer or Release-package qualification.
- Evidence: `artifacts\p139_public_product_catalog_20260719_103941\sample_catalog_summary.json` (initial expected regression), `artifacts\p139_public_product_catalog_fixed_20260719_104336\sample_catalog_summary.json` (184/184 pass), `artifacts\p139_product_sample_quality_20260719_104336\product_sample_quality_audit.md`, and `artifacts\p139_relocated_clean_runtime_20260719_104632\relocated_install\P139_RelocationRun_matching\report.txt` plus the Edge/Feature peer reports.

## P140 Gemini Flash Availability Check (2026-07-19)

- With the user's explicit approval, a signed-in Gemini Flash new chat received only `응답성 확인입니다. READY 한 단어로만 답하세요.` No image, XML, local path, API key, or project data was sent.
- The message appeared in the chat and Gemini remained in its visible response-generating state for an additional 20-second check without returning any response text. This is a provider-availability observation, not a correction-loop result.
- Per the user's operating rule, do not send another Gemini message or any sample for several hours after this stalled state. The live tab remains available for later manual/provider recovery; do not claim Gemini correction coverage from P140.

## P141 Tool View Code-Behind Stop-Condition Audit (2026-07-19)

- With Gemini paused and no labelled field-pilot variation set supplied, P141 audited the clean concrete Tool Views instead of creating a speculative refactor. The scope covered Threshold, Filter, Morphology, Arithmetic, SimplePreprocess, Blob, Contour, Line, Matching, EdgeBasedMatching, and FeatureMatching.
- Threshold, Filter, Morphology, and Arithmetic already delegate parameter binding, event handling, preview scheduling, summary text, and layout policy to the existing Controller/Presenter classes. Blob, Contour, and the Matching family already use the shared single-input PropertyGrid controller/base; Line delegates interaction, preview, review, localization, and preset concerns to named owners. The remaining code-behind methods are WPF construction/lifetime wiring, deliberate public test hooks, or narrow controller forwarding.
- No code was moved or deleted. Moving any remaining forwarding/test hook would add an owner without removing a real responsibility, and could destabilize explicit Preview/Run or test paths. `git diff --check` found no whitespace error (only existing CRLF conversion notices); the inspected Tool View files are clean in the worktree. This is a completed no-change audit, not a claim that all future Tool View work is unnecessary.

## P142 Local Field-Pilot Candidate Inventory (2026-07-19)

- The user-approved, ignored local `Sample` root was inspected read-only. It contains 341 image files across measurement, matching, object, OCR, code-reading, and related vendor/SDK example groups. No local image, path, XML, or screenshot was transmitted to a provider or placed in a public sample path.
- One explicitly named Good/NG pin pair was visually confirmed as a straight-pin reference versus a bent-pin negative. It contains only one image for each outcome. Several larger repeated groups exist, but their filenames do not declare Good/NG semantics and no operator label, ROI, or gate was supplied. They must remain unlabelled candidates rather than inferred validation evidence.
- Result: the local root offers a useful field-pilot starting point, but does not satisfy the required multi-variation labelled dataset. No recipe, catalogue entry, public asset, or product claim was changed. Evidence was the current read-only inventory (341 images), filename label search, image dimensions/SHA comparison, and local visual inspection; the local-only source paths are intentionally not repeated in public-facing documentation.

## P143 Image-List Validation And Die Pad 500 Baseline (2026-07-19)

- Recipe Manager now exposes an explicit `Image list validation` entry from Summary. It opens Advanced Review -> Pipeline -> Runs and selects the local validation-set scope without loading an image, running a pipeline, creating a layer, or changing input/output routing.
- The local validation set accepts separate OK and NG folders, displays a virtualized image list, runs every registered image through the selected saved Pipeline, and offers a safe stop that finishes the current image and persists the partial result. New local runs preserve actual Pipeline acceptance separately from expected/actual judgement and present Correct accept, False reject, False accept, and Correct reject outcomes. Historical rows without the new marker keep their prior interpretation.
- Current-source UI smoke passed at 1600x900 with no layout, text, or internal failure. The language-dependent harness wait was replaced with command/history state, reducing the focused smoke from roughly 65 seconds to 6 seconds. Final evidence: `artifacts\p143_batch_image_list_ui_20260719_170756\final_focused_local_validation_set`.
- A fresh zero-warning/zero-error Debug build and new clean Dev runtime were produced after the final source changes. The first broad direct-EXE `recipe-manager-tabs` attempt reached the advanced review but hit its existing timing-sensitive failed-Step action assertion; an immediate fresh-output rerun completed `Result: PASS`, including local file/folder controls, Run History judgement analytics, failed-Step review, and the no-auto-run sample-to-input contract. Current EXE evidence: `artifacts\p143_batch_image_list_ui_20260719_170756\after_clean_exe_recipe_manager_tabs_final_retry`.
- The user-supplied local Die Pad dataset provided the previously missing multi-variation labels: 250 OK plus 250 NG images, with train/validation/test metadata. A full Matching-only baseline completed all 500 rows and cleaned its reserved smoke workspace. Judgement was 243 correct accepts, 7 false rejects, 243 false accepts, and 7 correct rejects: 50.0% overall; NG rejection was only 2.8%. This proves the list/batch review path and proves that Matching-only is not an adequate varied-defect recipe. Evidence: `artifacts\p143_batch_image_list_ui_20260719_170756\die_pad_500_matching_baseline_final\README.md` and its JSON/CSV/screenshot artifacts.
- Final repository checks passed: OpenVisionLab readiness, vendored external-reference policy, public sample asset policy (`30` catalog rows, `229` manifest assets, `15` pipelines), and `git diff --check` with line-ending notices only.
- P143 does not claim production qualification, calibrated metrology, or robustness. The next recipe work must use train/validation only and freeze the 30 OK plus 30 NG test images until final evaluation.

## P144 Die Pad Reference-Difference Recipe And Frozen-Test Result (2026-07-19)

- Existing-tool evidence stopped speculative threshold iteration: the P143 Matching-only baseline reached only 50.0% accuracy and 2.8% NG rejection; Train-only local `ABSDIFF`/Contour and TopHat/BlackHat Mean probes either conflated normal structure with defects or detected only 17.2% of NG while preserving at least 85% of OK.
- Added one bounded rule-based `ReferenceDifference` Pipeline tool. It registers the input against the closest of up to four explicit approved Good references, normalizes grayscale intensity, detects localized difference regions, overlays their bounds, and reports defect/registration metrics. Zero regions is a successful measured result; the explicit Pipeline acceptance gate decides OK with `ResultCount=0`.
- The tool is editable through the existing Step PropertyGrid, validated by the XML contract, available through the runtime factory, and documented in the LLM authoring catalog. `ReferencePath1` through `ReferencePath4` are individually scanned and copied during import; a real clean-runtime import copied four files and ran both a Good and an intended NG sample successfully.
- Fixed Train parameters produced 176 correct accepts, 4 false rejects, 0 false accepts, and 180 correct rejects: 98.89% accuracy, 97.78% OK recall, and 100% NG recall. The unchanged held-out Validation split produced 40/0/1/39: 98.75% accuracy, 100% OK recall, and 97.50% NG recall, exceeding the predeclared 90%/85%/85% gate.
- Only after Validation passed, the frozen Test 30 OK plus 30 NG split was executed once without retuning. It produced 30 correct accepts, 0 false rejects, 1 false accept, and 29 correct rejects: 98.33% accuracy, 100% OK recall, and 96.67% NG recall.
- The Test artifact uses the original supported semicolon-list serialization. The post-Test portability revision only split the same four dependency values into individually copyable parameters; current-source Validation reproduced the same 40/0/1/39 result and the Test split was not rerun.
- A freshly built clean Dev runtime passed current `llm-xml-draft-file` import/copy and execution for one Train Good and one intended Train NG. The retained `bin\Debug` workspace reproduced its previously documented native `FindContours` crash and is not used as success evidence.
- Final solution and screenshot-smoke builds completed with zero warnings/errors. OpenVisionLab readiness, vendored external-reference policy, public sample policy (`30` catalog rows, `229` manifest assets, `15` pipelines), tool-catalog JSON parsing, and `git diff --check` all passed; the latter reported line-ending notices only.
- Full evidence, candidate SHA, split discipline, exact results, current-source PropertyGrid capture, and boundary are in `artifacts\p144_die_pad_multitool_20260719\README.md`. This is strong evidence for the supplied synthetic 500-image corpus, not field qualification or production readiness.

## P145 Golden-reference Defect Guided Setup (2026-07-19)

- Recipe Manager -> Build inspection now includes `Golden-reference defect (ReferenceDifference)`. The operator supplies one to four approved Good reference paths plus difference threshold and minimum/maximum defect area; the deterministic generator creates the proven P144 registration defaults and exact `ResultCount=0` acceptance.
- Reference selection remains explicit. Draft creation does not learn or replace references, import XML, Preview, Run, create layers, change the active layer, or change input/output routing. The normal LLM XML validation/import path discovers all four independent dependencies.
- The selected intent is now a strict validation contract: an enabled `ReferenceDifference` Step with exact `ResultCount=0` is required. Current-source smoke changed the maximum gate to `1`, confirmed validation failure with corrective guidance, restored the original XML, and confirmed import readiness.
- Fresh current-source UI smoke passed at 1600x900 with all layout/text/internal counters at zero. A new clean Dev runtime at `artifacts\openvisionlab_clean_runtime_20260719_210850` passed the focused EXE scenario with four dependency rows, unchanged Preview/Run count, and unchanged layer/route state.
- The older broad `recipe-manager-tabs` clean-runtime scenario stopped earlier at a non-packaged historical `docs` Matching template path and is not used as P145 success evidence. This is a smoke portability boundary, not a ReferenceDifference product failure.
- Final solution build completed with 0 warnings/errors. Readiness, external-reference policy, public-sample policy (`30` catalog rows, `229` manifest assets, `15` pipelines), and `git diff --check` passed; line-ending conversion notices remain informational. Full closure and UI/EXE evidence: `artifacts\p145_reference_difference_guided_setup_20260719\README.md`.

## Known Gaps And Honest Limits

1. **Real correction-loop breadth is incomplete.** P136 adds a current clean-Dev same-conversation GPT dependency-path correction with public assets, alongside the earlier GPT/Gemini examples. It proves one real correction path, not independent authoring reliability, broad natural-failure coverage, or cross-provider equivalence. Gemini remains paused by the recorded operating rule, and Claude remains deferred.
2. **Cross-install template relocation is now proven only for the three template tool families.** P137 proves moved-package execution for Matching, EdgeBasedMatching, and FeatureMatching when templates reside under the moved package's `RECIPE` tree. It does not qualify installer behavior, arbitrary external dependencies, updates, signing, or deployment support.
3. **Industrial validation is incomplete.** P144 proves a held-out 500-image synthetic Die Pad corpus, but public/local synthetic samples still do not establish robustness across real production part variation, camera noise, lighting, fixturing, calibration drift, or operator error.
4. **Calibration is an operator responsibility.** Pixel results are useful without calibration. mm gates require independently verified scale/calibration evidence; do not imply physical accuracy from a positive `PIXELPERMM` alone.
5. **No novice usability measurement exists.** The flow has guided surfaces and visual evidence, but no independent beginner study verifies that a first-time user understands the workflow without help.
6. **No blanket branch-comparison gap is proven.** Existing review handles direct `InputLayer`, declared `SourceLayers`, and declared `SourceSteps` relations. Extend it only after a real multi-branch recipe demonstrates a missing relationship outside those contracts.
7. **Host cleanup has a stop condition.** The remaining composition files are not automatic debt. Further cleanup must be driven by a real responsibility boundary, not folder appearance.
8. **Release readiness is not field readiness.** Build/policy checks and release policies exist; no signed installer, deployment support program, or production acceptance campaign is claimed.
9. **The retained `bin\Debug` folder is not a clean runtime or deployment artifact.** P133 records the approved contract: use a new timestamped Dev runtime under `artifacts` for current EXE evidence and a new `dist\OpenVisionLab` package for release evidence. Preserve the retained local workspace without automatic deletion or migration. P134/P137 separately verify copied-template behavior in and after a package move.

## Next Priority Order

1. **Acquire and approve real captured Die Pad Good/NG variation before further algorithm work.**
   - Prerequisite: multiple real captured Good and defect images under representative pose, illumination, focus, and part variation, plus operator-approved labels and inspection region. P144 already passes the synthetic Train/Validation/frozen-Test contract; do not spend model tokens retuning it against the same corpus or present it as field-ready. Recommended model: 해당 없음 (데이터 확보 전 모델 작업 불필요) | Reasoning effort: 해당 없음.

2. **Resume non-GPT provider correction coverage only after the provider is usable and the user authorizes it.**
   - Prerequisite: Gemini must recover from the P140 no-response state after the user's required pause, or Claude access must be explicitly resumed. Use public samples only and preserve the first response before validation. No model recommendation until that condition exists.

3. **Test an installer/update path only when installation behavior becomes an explicit product requirement.**
   - Prerequisite: a concrete installer, update, or signed-package acceptance requirement. P137 already covers a copied clean package at a different root; no additional relocation work is warranted without that requirement. No model recommendation until that condition exists.

## Handoff Rules For The Next Chat

- Do not claim a feature is complete because the historical handoff says so. Re-run the smallest meaningful current command when work touches it.
- Do not use older screenshots as current evidence. New UI work requires a build/source timestamp check and fresh before/after images.
- Do not commit/push or touch `C:\Git\OpenVisionLab` unless the user explicitly asks in that active chat.
- Preserve user changes and unknown dirty files. Investigate them; do not reset, checkout, or bulk-copy over them.
- Record each completed bounded slice in this file with: changed responsibility, verification command/result, artifact path when UI changed, and the re-ranked next priority.

## Detailed Evidence References

- Current product shape and main view responsibility: `docs/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`
- Stable non-regression contract: `docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`
- Source ownership proof for P95-P104: `docs/OPENVISIONLAB_SOURCE_OWNERSHIP_REFACTOR_PROOF_20260717.md`
- Full chronological engineering evidence: `docs/OPENVISIONLAB_NEXT_SESSION_HANDOFF.md`
- Existing handoff prompt/template: `docs/OPENVISIONLAB_NEXT_CHAT_HANDOFF_PROMPT_20260706.md`
- LLM XML contract and tool catalog: `docs/OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md`, `docs/OPENVISIONLAB_LLM_TOOL_CATALOG.json`
- Public assets, external dependencies, and release rules: the three policy documents listed in `docs/OPENVISIONLAB_DOCUMENTATION_MAP.md`
