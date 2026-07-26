# OpenVisionLab Current Project Handoff

Updated: 2026-07-24 KST

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

This is the observed pre-publication state for the 2026-07-24 handoff
consolidation. Always rerun `git status -sb` and `git log --oneline -5`; do not
treat the hashes below as a substitute for current Git state.

| Repository | Branch | Observed head | Observed worktree | Meaning |
| --- | --- | --- | --- | --- |
| `C:\Git\OpenVisionLab_Dev` | `codex/public-sample-ux-docs` | `dc187c27` | P196-P235-era product, evidence, contract, smoke, and documentation changes accumulated in the working tree. | Authoritative source for the requested publication. |
| `C:\Git\OpenVisionLab` | `main` | `0e02609` | Clean and tracking `origin/main`. | Reviewed target; import Dev by Git patch/commit, never by bulk folder copy. |

- The user explicitly requested that the consolidated Dev state be committed
  and pushed, then imported into the original repository and committed/pushed
  there as well.
- At the prerequisite check, GitHub CLI `2.96.0` was installed but
  `gh auth status` reported no authenticated GitHub host. Publication therefore
  requires `gh auth login` before a push can be claimed.
- `.codex-remote-attachments/` is user-supplied input and must not be staged,
  deleted, or copied to the original repository.
- `.proofline/` contains the project-local resolved issue ledger and dashboard
  used by the accumulated work. It is project evidence, unlike the attachment
  folder.
- Exact publication commits belong in Git history and the close-out report.
  Do not embed a guessed final commit hash in this document.

## Incremental Work Update (2026-07-25)

- Ran `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` after MVVM 구조 분해 Slice의 1개 부분 작업을 적용해 PASS(0 오류, 0 경고).
- `UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs`의 이벤트/스텝 제어 경로를
  `UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.Events.cs`로 partial 분리해 가독성을 개선했습니다.
  동작 계약(명시적 Preview/Run, 라우팅/레이어 유지)은 기존과 동일하게 유지했습니다.

- OpenVisionShellHostRecipeCommandSurface additionally received a second refactor slice: StepNavigation methods were extracted to OpenVisionShellHostRecipeCommandSurface.StepNavigation.cs (preview step matching/selection, run-evidence resolve, run/sampling helpers, unique name helpers, and command-state refresh).
- Verification: dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" passed (0 warnings, 0 errors).
## Product Identity

OpenVisionLab is an OpenCvSharp4 rule-based vision recipe workbench. Direct deterministic teaching and repeatable validation are the product core; the existing LLM XML assistant is optional and frozen in maintenance mode by P196.

Its operator workflow is:

1. Choose or load a sample image.
2. Teach the inspection target, ROI/template/measurement region, and OK/NG condition through PropertyGrid-based tools.
3. Compose and validate the Pipeline and its layer routes.
4. Run Preview or Run only through an explicit user action.
5. Compare layers, metrics, Good/Bad results, failed steps, ROI, templates, and current-run drawings.
6. Replay a frozen recipe on N samples and inspect its deterministic review queue.
7. Save a reusable recipe and its validation references. Existing LLM XML draft/validation/import may optionally assist composition but is not required.

The product is not a camera, lighting, PLC, I/O, account, deployment, MES, or industrial-controller platform. Commercial vision software is useful as a reference for guided configuration, visual result evidence, named recipe management, and explicit validation workflows. It is not a reason to expand into equipment integration.

## Current State Ledger For The Next Chat

The detailed P1-P235 chronology remains in
`docs\OPENVISIONLAB_NEXT_SESSION_HANDOFF.md`. This ledger is the compact current
truth; a completed engineering slice does not imply field qualification.

| Track | Current state | Durable result | Remaining boundary |
| --- | --- | --- | --- |
| Workbench, Learn, Tool Views, Pipeline, Pipeline Review, Recipe Manager, public samples | Complete for the intended local learning/verification scope. | PropertyGrid teaching, explicit Preview/Run, layers/routes, drawings, result review, saved recipes, validation sets, run history, public samples, and clean-runtime evidence are connected. | No independent novice study, installer/support lifecycle, or production-equipment qualification. |
| LLM XML authoring and inspection-intent skills | Preserved; planned expansion frozen by P196. | Guide/catalog, strict validation/import, prompt/evidence packages, Pin Phase 1/2, and hybrid relative-ROI Phase 1 remain compatible. | Natural Pin Phase 3 failure/correction and broad GPT/Gemini/Claude reliability are incomplete and intentionally not active backlog. |
| Deterministic fixture, measurement, object review, and calibration UI (P197-P217) | Completed bounded product slices. | Relative-ROI workflow, LineDistance drawing/persistence, CenterPitch, Object Results, Fixture Review, GeometryMeasure/CircleGauge, two-point scale, and Blob/Contour dimension filters have focused runtime/UI evidence. | Pixel/synthetic or selected-corpus evidence is not certified metrology, unseen robustness, or industrial truth. Missing-pin candidates P205/P207/P209 were correctly rejected rather than tuned indefinitely. |
| Three-point affine and detected-point fixture (P218-P221) | Core and product wiring complete; one strict pilot remains incomplete. | Library-Noah Affine, PropertyGrid/Pipeline/XML/Learn, typed Point x3 binding, and a 12/12 coarse fixed-ROI linkage exist. | P220 remains 10/12 at its frozen `<=3 px` residual gate; the separate accepted `<=5 px` coarse ROI does not retroactively pass it. No homography, lens calibration, or field proof. |
| Auto MPoint and unique edge matching (P222-P231) | Training UI/reporting complete; qualification is intentionally selective. | Deterministic candidate ranking, explicit apply, ambiguity states, representative-image evaluation, HTML export, and one frozen Die Pad 1 full-stratum qualification exist. | The card `R` anchor was rejected; other corpus strata are not qualified. Score alone cannot prove durable physical-feature identity or authorize automatic template selection. |
| Shared Tool View N-image verification and Recipe promotion (P232-P235) | Complete for sequential quick verification and locator handoff. | Thirteen eligible single-input Tool Views share frozen-Step N-image execution/reporting; P234 proves one real 24-image folder path; P235 promotes an exact hash-locked locator expected-success set into Recipe Manager. | Execution is sequential, sample roles are not inferred, promoted `Expected OK` is locator success rather than defect truth, and no field robustness is claimed. |
| Release/runtime evidence | Complete for the approved bounded contract. | Timestamped clean Dev runtimes and `dist\OpenVisionLab` Release output avoid stale `bin\Debug` evidence. | This is not an installer, deployment platform, equipment runtime, or production support contract. |

### Explicitly Incomplete, Rejected, Deferred, Or Out Of Scope

1. **Incomplete but preserved:** P220's card affine pilot has two rows outside
   the frozen `<=3 px` residual gate. Do not relabel it complete.
2. **Rejected candidates:** the tuned-raw missing-pin paths and card `R` unique
   locator failed semantic/drawing review. Their completed audits are evidence
   to stop those candidates, not unfinished tuning requests.
3. **Deferred implementation:** concurrent N-image workers do not exist.
   Consider isolated `1/2/4` worker equivalence only after a measured
   sequential bottleneck and an explicit user request.
4. **Frozen work:** provider expansion, browser/API automation, new LLM prompt
   families, and repeated transcript/correction campaigns remain closed unless
   the user explicitly reopens the LLM direction.
5. **Experimental only:** `OuterCornerIntersection` remains outside default
   recommendation paths until independent physical-boundary evidence exists.
6. **Not qualified:** broad real-production variation, camera/lens calibration,
   certified dimensional accuracy, unseen-data robustness, and field acceptance
   have not been proven.
7. **Out of product scope:** camera, lighting, PLC/I/O, MES, accounts,
   deployment orchestration, and industrial controller functionality.

## Evidence-Based Maturity Statement

Do not use a single percentage as the current release judgement. Older 62-66%, 98%, or other percentages in historical documents are scoped historical estimates, not current release claims.

| User goal | Current evidence | Current limitation | Status |
| --- | --- | --- | --- |
| Operate the workbench | WPF shell, tool rail, PropertyGrid tools, layers, explicit Preview/Run, Pipeline Review, Recipe Manager summary/advanced flow, public samples, and current-EXE smokes are present. | No independent novice usability study or production support workflow has been completed. | Usable for guided sample-backed work. |
| Learn OpenCvSharp concepts | Separate Learn surface covers image/GV, Mat, Point/Rect/ROI, brightness/histogram, arithmetic, filtering, geometry, edge, HSV, Threshold/Morphology, Blob/Contour, matching, pipeline, metrics, and XML authoring. | Learning content is tool-oriented rather than a complete OpenCV course; real learner comprehension has not been measured. | Broad practical curriculum, not a certified training course. |
| Understand rule-based vision | Learn topics, public samples, Tool Views, output layers, metric gates, Good/Bad review, and result explanation connect concepts to observable evidence. | Industrial variability, illumination variation, calibration, and fixture tolerance are not broadly covered by real datasets. | Strong for deterministic examples; partial for field understanding. |
| Review industrial images | Explicit layer routing, ROI/template tools, measurement metrics, fixture translation v1, bounded Matching-driven full-image normalization, sample validation, result review, the P182 coordinate-correct C9 pixel Gap path, the P183 fail-closed gate, and the P184 500-image top-left replay/report are implemented. | Black-strip OK/NG truth, independent tolerance/calibration, unseen/all-direction robustness, and a field-qualified acceptance campaign do not exist; P183 thresholds and P184 results remain C9/top-left bounded. | Constrained verification workbench, not production qualification. |
| Use LLM assistance | XML guide/catalog, local validation/import gates, correction-packet support, prompt packets, one bounded `PinArrayGap` skill through Phase 2, a fresh P169 judged GPT direct-success replay with a frozen unused Test split, and P170 target-bearing working Train/Validation manifests exist. | One-shot generation is not reliable enough to promise automatic inspection authoring; P170 Validation is previously observed rather than blind, and the first skill still lacks a genuine naturally failed GPT draft, evidence-backed correction, and one-time replay of the reserved held-out Test. | Guided authoring assistant, not autonomous recipe creation. |

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

- The final P236 public-sample check reports `CatalogRows=33`,
  `ManifestAssets=229`, and `Pipelines=17`. Older counts inside P-number
  chronology entries remain the observed values at those historical checkpoints.
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

- **Historical scoped LLM workflow milestone (P125, 2026-07-18):** Level 4 of 5 for that bounded LineDistance delivery plan only. It is not the current overall product or `PinArrayGap` skill maturity. P125 closes the planned LineDistance first-response failure -> same-web-conversation repair -> current-Debug Good/Bad replay. This does not prove provider reliability, autonomous authoring, field robustness, calibration, deployment, or production readiness.

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

## P146/P147 Batch Drawing Evidence And Pin_1 GPT XML Audit (2026-07-20)

- P146 adds an explicit Run History `도면 보기` action. It opens the selected sample's stored original and persisted detection drawing side by side, without rerunning Preview/Run or changing layers or input/output routing. Batch sample checks now retain one review overlay per sample: the failed Step when one exists, otherwise the last relevant overlay-bearing Step. The implementation deliberately keeps one persisted review image instead of every intermediate Step image.
- P147 reran the user-supplied local Pin_1 corpus with the saved third GPT correction XML and made no XML, ROI, metric-gate, or algorithm adjustment. The 500 rows produced 49 correct accepts, 201 false rejects, 37 false accepts, and 213 correct rejects: 52.40% accuracy, 19.60% OK recall, 85.20% NG recall, and 2.268 ms average elapsed time.
- Every false classification was copied before the reserved Smoke workspace cleanup. `artifacts\p147_pin1_gpt_full_batch_drawing_audit_20260720\misclassification_evidence` contains 238 indexed folders, each with its original image, persisted drawing, and Run Report; an integrity pass opened every copied original/drawing and found `Failures=0`. The current-source WPF comparison capture is `artifacts\p147_pin1_gpt_full_batch_drawing_audit_20260720\wpf_shell_host_recipe_local_validation_drawing_evidence.png`.
- This is a negative recipe-quality result, not a reason to tune against the same undifferentiated set. The current GPT XML is usable as an auditable draft, but is not acceptable for this 500-image corpus. Treat the corpus as local evaluation evidence until the operator defines inspection intent, ROI, acceptance criteria, and an explicit Train/Validation/Test split.

## P148 Dynamic PinArrayGap And Frozen Pin_1 Pitch Evidence (2026-07-20)

- P148 preserved the user-supplied local Pin_1 split exactly: Train 356, Validation 72, Test 72. The synthetic/augmented dataset was not sent to a provider, copied into public samples, or used for a deployment/production claim.
- The all-pair static `LineDistance` experiment could not support safe tuning: fixed ROI completed only 42/356 Train images and translation-only fixture ROI completed 31/356. That is a fixed-coordinate limitation under the supplied variation, not a basis for fitting gates from invalid measurements.
- `PinArrayGap` is now a validated/importable XML ToolType (alias `AdjacentPinGap`). In one reviewed row ROI it finds dark vertical pin runs, measures every adjacent edge-to-edge clearance dynamically, returns count/min/max/average/range metrics, and supplies pin/gap drawing overlays. The LLM XML guide/catalog documents the boundary: one row ROI, edge clearance rather than center pitch, and pixel-only criteria until an actual scale is supplied.
- The measurement-only recipe completed 356/356 Train images. Expected-Good range was at most 6 px in both rows, so `DistancePxRange <= 6` was frozen for top and bottom before Validation/Test. Current-source results: Train Expected Good 178/178 accepted and pitch-error 38/38 rejected; Validation Expected Good 36/36 accepted (it contains no pitch-error label); Test Expected Good 36/36 accepted and pitch-error 12/12 rejected. Other defect classes are cross-defect observations, not a complete Pin_1 classifier.
- Evidence and exact boundary: `artifacts\p148_pin1_all_pitch_measurement_20260720\README.md`. The dynamic measurement starter and frozen gate XML plus Train/Validation/Test CSVs are preserved there.
- Final verification passed: Debug solution build (0 warnings/errors), rebuilt `VisionRecipeRunnerSmoke`, `OpenVisionReadinessCheck`, external-reference policy, public-sample policy (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), JSON catalog parse/PinArrayGap contract check, and `git diff --check` (line-ending notices only). A current-build expected-Good smoke passed and a current-build pitch-error smoke returned the expected range-gate NG with the pin-array-specific next action.

## P149 Card Intersection And Curved-Band Measurement Evidence (2026-07-20)

- P149 used only the user-approved local `card_original` and `device_left` datasets. No source image, label, XML, or result was sent to an LLM/provider or placed in a public-sample path.
- `LineIntersection` completed all 500 `card_original` split rows, but operator review later showed that its broad candidate ROIs selected a text/diagonal pair rather than the required card lower/right edges. Treat that P149 card probe as rejected diagnostic evidence only; P150 supersedes it with the intended corner geometry.
- Added `CurveBandProfile` (alias `DarkBandCurve`) as a validated/importable XML runtime tool. Within an explicit ROI it selects the leftmost eligible dark component, draws its outer/inner curve and sampled widths, and reports profile/arc-length metrics. Factory, XML validation, known metrics, diagnostics, runner CSV, LLM catalog, and authoring guide were updated together.
- The fixed `CurveBandProfile` recipe completed all 500 `device_left` split rows. Train/Validation/Test center-arc means were 191.659/191.061/191.016 px. It correctly follows the reviewed curved band after lateral movement, but the supplied expected-OK and expected-NG metric ranges overlap and the reviewed NG boxes describe independent central defects. No curve-quality acceptance gate or millimetre claim was added.
- Evidence and exact recipes: `artifacts\p149_card_intersection_device_curve_measurement_20260720\README.md`. It retains split CSVs, four current-build overlays, and the two XML probes.
- Final verification passed: Debug solution build (0 warnings/errors), `VisionRecipeRunnerSmoke` build, `OpenVisionReadinessCheck`, external-reference policy, public-sample policy (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), JSON catalog parse, and `git diff --check` (line-ending notices only).

## P150 Dynamic Card Bottom-Right Outer-Corner Evidence (2026-07-20)

- P150 corrects the rejected P149 card probe. The intended feature is the virtual sharp corner made by the card's lower outer edge and right outer edge, not any text, scratch, or arbitrary fitted-line intersection.
- Added `OuterCornerIntersection` (alias `BrightObjectCorner`), a validated/importable XML runtime tool. It first uses a non-frame-touching bright outer component, then dynamic bottom/right intensity-transition and direction-constrained Canny/Hough fallback only when the component is unreliable. It rejects implausible top/left intersections and draws both selected outer edges to their common corner.
- Finalized local-only split results were Train 350/350, Validation 75/75, and Test 75/75. The 500 visible intersections ranged X=334.883..632.715 and Y=317.528..473.586; average elapsed time was 4.608 ms. This is geometry-execution evidence, not an OK/NG defect classifier or calibrated measurement result.
- Current overlays and final CSVs are retained at `artifacts\p150_card_bottom_right_intersection_20260720\README.md`. The earlier fixed-ROI attempt completed only 297/350 Train, 56/75 Validation, and 63/75 Test rows; it is explicitly rejected rather than tuned further.
- Factory, XML validator/schema, known metrics (`IntersectionX`, `IntersectionY`), diagnostics, LLM XML tool catalog, and authoring guide were updated together. Final verification passed: Debug solution build (0 warnings/errors), runner build, the three split batches above, `OpenVisionReadinessCheck`, external-reference policy, public-sample policy (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), JSON catalog parse, and `git diff --check` (line-ending notices only).

## P151 GPT PinArrayGap Direct-Success Evidence (2026-07-20)

- In the user-authorized `룰베이스 LLM 연동` GPT project, only public bundled `Sample\EasyGauge\Pin 1.jpg` was attached. The exact request, first XML document content, and conversation URL are retained in `artifacts\p151_gpt_pinarraygap_direct_success_20260720\README.md`.
- GPT's first response used exactly one `PinArrayGap` Step with `Main -> Top_Pin_Clearance`, upper-row ROI `0,90,768,170`, and pixel-only parameters. The unchanged XML validated/imported through the current Debug LLM XML route (`ValidationOk=True`, `ImportEnabled=True`, `Imported=True`, 0 errors/warnings), then explicit image run returned `Success=True`, 14 adjacent edge-to-edge gaps, and a 43..44 px gap range in 49.653 ms. Its drawing confirms the upper row rather than the lower row was selected.
- No first-response validation or runtime failure occurred. Do not call P151 a correction loop and do not manufacture a correction prompt. It is one real GPT direct-success path only; it does not establish natural-authoring or correction reliability across providers or samples.
- Final verification passed: Debug solution build (0 warnings/errors), `OpenVisionReadinessCheck`, public-sample policy (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), XML parse, and `git diff --check` (line-ending notices only).

## P152 Card-Corner Acceptance Evidence Preparation (2026-07-20)

- `OuterCornerIntersection` already returned lower/right line angle metrics, but the batch CSV retained only X/Y. `VisionRecipeRunnerSmoke` now records read-only `LineAngleMin`, `LineAngleMax`, and `LineAngleAvg` beside each row's intersection coordinates; no tool parameter, runtime selection, or acceptance gate changed.
- The finalized P150 XML reran over all local splits with the new columns: Train 350/350, Validation 75/75, Test 75/75, zero missing images. The operator decision table at `artifacts\p150_card_bottom_right_intersection_20260720\P150_OPERATOR_ACCEPTANCE_SPEC.md` contains split/OK-NG coordinate and angle distributions plus explicit fields for allowed movement, X/Y band, angular deviation, and out-of-frame handling.
- The supplied defect labels do not identify the card corner or rotation. Their X/Y and angle distributions overlap, so P152 deliberately adds no automatic card judgement gate. The next dependency remains an operator-approved inspection specification or geometric ground truth.
- Final verification passed: Debug solution build (0 warnings/errors), runner build, all three angle-CSV row/metric checks, `OpenVisionReadinessCheck`, external-reference policy, and public-sample policy (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`).

## P153 Algorithm Drawing-Evidence Contract And Card-Corner Visual Replay (2026-07-20)

- `AGENTS.md` now requires every algorithm image-validation claim to retain and show current-run visual evidence, not only CSV/PASS metrics. The required evidence is the exact executed source image, XML/recipe, runtime overlay with the selected ROI/geometry/final detection, result metrics, visual inspection, and direct in-chat rendering. Batch reports require representative ordinary and difficult/boundary/failure overlays when those cases exist.
- P150's finalized `OuterCornerIntersection` XML was rebuilt and rerun on a normal OK sample, the operator-reviewed NG_0066 sample, and a lower-position NG_0246 sample. `artifacts\p153_card_corner_visual_evidence_20260720\README.md` contains copied inputs, exact XML, runtime result drawings, all-overlay drawings, coordinates, angles, and the executable command. The red bottom/right edges and green intersection are actual runtime output; no manual annotation was substituted.
- All three visual replays passed with `ResultCount=1`, `EdgeCount=2`, and `IntersectionCross=1`. They prove the selected geometry is the intended lower/right card outer corner on these cases, but do not add an OK/NG defect, position, or angle acceptance gate.

> **Superseded by P154:** the P153 NG_0066 profile result was rejected by later operator visual review. Do not use P153's 500-row completion or NG_0066 drawing as semantic card-edge correctness evidence.

## P154 Card-Corner False-Positive Repair And LLM Visual-Review Contract (2026-07-20)

- The user rejected P153's NG_0066 profile drawing. The prior algorithm accepted a dense bright-component profile whenever its fitted-line intersection was in frame; it did not prove that both lines were adjacent sides of the card outer contour. P154 first uses the selected bright contour's rotated outer rectangle, taking the lower/right adjacent sides and their common virtual corner. NG_0066 changed from `(551.136, 354.271)` to `(534.113, 357.586)` and its current result drawing is `artifacts\p154_card_corner_false_positive_repair_20260720\result_ng_0066_outer_contour_verified_current.png`.
- Runtime now publishes `CornerOuterContourVerified`: `1` for adjacent outer-contour sides and `0` for profile/edge fallback. The batch CSV records it. The LLM authoring guide/catalog say that a `0` requires a preserved overlay review/correction and cannot supply a position/angle gate. The Recipe Manager LLM validator adds a visible `Outer-corner: WAIT` instruction without blocking import, because XML syntax alone cannot decide image semantics. Current-source WPF smoke confirms the message is fully visible and makes no Preview/Run or layer change: `artifacts\p154_card_corner_false_positive_repair_20260720\llm_review_ui_current\wpf_shell_host_outer_corner_llm_review.png`.
- Current measurement-only rerun still completed Train `350/350`, Validation `75/75`, Test `75/75`, but `CornerOuterContourVerified=1` occurred on only `5/500`; the other `495/500` used reviewed fallback. A strict `CornerOuterContourVerified >= 1` gate therefore passed only `5/500` and is intentionally not the default card gate. This records a real limitation, not a 500-row semantic-correctness claim.
- P154 is **Incomplete** until the operator supplies ground-truth corner/edge marks or accepted pixel coordinates for NG_0066 and representative fallback cases. The supplied defect labels/masks do not identify the intended card corner.

## P155 Card-Corner Operator-Mark Comparison And Honest LLM Review (2026-07-20)

- The user's blue-mark screenshot was matched against all 500 local `card_original` images. `card_original_NG_0066.jpg` was the top normalized-grayscale correlation match (`0.941901`; next `0.854018`). Its displayed 640 x 480 source rectangle maps the blue centre to approximately `(530, 391)` with a freehand-mark uncertainty of about +/-18 px. This turns the supplied visual criticism into traceable local evidence rather than a guessed target.
- `OuterCornerIntersection` now fits lower/right support points from the selected bright contour before using the existing profile/edge fallback. On NG_0066 it selected 129 support points and returned `(531.274, 352.716)`. `CornerOuterContourVerified=1` now means only that both fitted lines have outer-contour support; it does **not** assert agreement with the operator target. The measured difference to the blue mark is `(+1.274, -38.284)` px (38.305 px total), so this remains an incorrect target result rather than a completed repair.
- Evidence is under `artifacts\p155_card_corner_contour_tangent_repair_20260720`: `result_ng_0066_before.png` and `result_ng_0066_after.png` are exact runtime drawings; `comparison_ng_0066_operator_mark_vs_after.png` adds the separately labelled blue operator mark; `P155_OPERATOR_MARK_COMPARISON.md` records mapping, numbers, and limits. The local measurement-only split replay still completed Train `350/350`, Validation `75/75`, Test `75/75`, with only `4/500` rows reporting outer-contour support.
- The LLM XML validator, authoring guide, and catalog now say to compare red edges/green corner with an operator mark **before** a coordinate/angle gate. Fresh current-source WPF before/after captures are `llm_review_before\wpf_shell_host_outer_corner_llm_review.png` and `llm_review_after_final\wpf_shell_host_outer_corner_llm_review.png`; the after text is fully visible and the smoke confirms no layout, text, Preview/Run, or layer side effect.
- P155 is **Incomplete**. One marked NG_0066 sample exposes a 38 px Y disagreement, but does not define whether the intended virtual point is a tangent intersection, a rounded-corner offset, or a fiducial. Do not tune a new default from one sample or add a card position/angle gate.

## P156 Card-Corner Operator-Mark Packet (2026-07-20)

- Prepared the missing translated-mark packet without guessing new geometry: `artifacts\p156_card_corner_operator_mark_packet_20260720`. It contains source-only 50 px grids, exact current runtime result/all-overlay drawings, XML, and raw run logs for left-shifted `NG_0207` and lower/right-shifted `NG_0246`.
- `NG_0207` currently returns `(369.509,389.785)` through the fallback and visibly sends its vertical red candidate through printed content; `NG_0246` returns `(601.540,444.077)` through the fallback. Both report `CornerOuterContourVerified=0`, so the packet makes neither a coordinate/angle gate candidate. Their role is to collect the same lower/right segment plus virtual-corner interpretation as P155's recovered NG_0066 mark.
- `P156_OPERATOR_MARK_PACKET.md` supplies the reusable coordinate template. Runner build passed with 0 warnings/errors; both replays returned `Success=True`, `ResultCount=1`, `EdgeCount=2`, and `IntersectionCross=1`; all current source/grid/result drawings were visually inspected.

## P157 Card-Corner Manual-Tolerance Fallback Repair (2026-07-20)

- The user's two freehand marks were treated as geometric intent with a +/-20 px source-coordinate tolerance, not one-pixel ground truth. The grid maps `NG_0207` to `(546,390)` and `NG_0246` to `(601,441)`. Current comparison drawings, copied operator screenshots, exact XML, sources, logs, and three split CSVs are under `artifacts\p157_card_corner_manual_tolerance_repair_20260720\P157_MANUAL_TOLERANCE_REPAIR.md`.
- The demonstrated `NG_0207` error was a fallback-order defect: projection fit accepted printed content at `(369.509,389.785)` before the available long lower/right Hough outer-edge pair. `OuterCornerIntersection` now tries the direction-constrained Hough pair first and labels the visible source as `hough`, `projection`, or `outer`. Current exact runtime returns `NG_0207=(547,389)`, a `(1,-1)` px difference from the approximate mark; `NG_0246=(601.540,444.077)` remains within its mark tolerance. `CornerOuterContourVerified=0` remains a mandatory visual-review condition for both fallback labels.
- Fresh latest Runner DLL execution completed all `500/500` local card rows (Train `350/350`, Validation `75/75`, Test `75/75`) without missing images. This is execution stability evidence only. The prior `NG_0066` outer-contour result remains `(531.274,352.716)`, 38.305 px from its recovered `(530,391)` mark, and is explicitly retained as a semantic mismatch.
- The LLM authoring guide/catalog now name `hough` versus `projection` fallback so an XML author must compare the actual red/green drawing and operator mark before enabling a coordinate/angle gate. P157 is **Incomplete** until card-corner semantics are defined for the remaining `NG_0066` disagreement; no automatic coordinate/angle gate was added.

## P158 LLM Assistant Outer-Corner Correction Contract (2026-07-20)

- The P157 image-validation result now reaches the actual Recipe Manager LLM XML review path. Any `OuterCornerIntersection`/`BrightObjectCorner` draft reports `Corner WAIT: run; red/green + hough/projection/outer vs mark; no coordinate gate if fallback.` This turns the runtime drawing/source label into an explicit correction-loop input instead of leaving it only in a guide.
- The warning remains advisory: XML validation/import never executes Preview/Run, creates layers, or changes routing, and cannot infer semantic correctness from XML alone. It therefore does not claim `CornerOuterContourVerified=1`, hough/projection fallback, or an LLM draft itself is a valid coordinate/angle gate.
- Fresh current-source 1600 x 900 before/after captures are `artifacts\p158_llm_outer_corner_correction_contract_20260720\llm_review_before\wpf_shell_host_outer_corner_llm_review.png` and `llm_review_after\wpf_shell_host_outer_corner_llm_review.png`. The final compact line is visible without clipping; focused smoke passed with layout/text/internal checks `0` and no execution side effect. P158 is **Complete** for the LLM Assistant review-contract change. P157 `NG_0066` geometry remains separately incomplete.

## P159 NG_0066 Card-Corner Interpretation Packet (2026-07-20)

- Rebuilt and reran the current Debug Runner on `NG_0066`. The exact runtime outer-tangent remains `(531.274,352.716)` with `CornerOuterContourVerified=1` and 129 support points; it is 38.305 px from the recovered operator mark `(530,391) +/-20 px`. The new current artifact folder `artifacts\p159_card_corner_interpretation_packet_20260720` contains the exact XML/source/runtime drawing, a separately labelled operator-mark comparison, raw run log, and reusable LLM correction-packet text.
- The evidence establishes that an LLM must not invent a coordinate, offset, or gate. It now presents the only three meaningful contracts: true outer-edge tangent, deliberate rounded/cropped-corner offset, or a separate fiducial. The authoring guide records the same no-guessing rule. P159 is **Complete** as a decision/recovery packet; it does not resolve the external geometry decision.

## P160 Same-Image Card-Corner Validation Correction (2026-07-20)

- The operator clarified that the blue freehand mark previously mapped to `NG_0066` may belong to a different card image. Because translated/rotated cards legitimately have different `IntersectionX`/`IntersectionY`, P155/P159's 38.305 px cross-image comparison is invalidated and must not be used for an XML change, tolerance, gate, or LLM correction request. Preserve P159's exact runtime drawing only as historical execution evidence.
- LLM Assistant validation now says `Corner WAIT: same image; red/green + hough/projection/outer vs mark; no gate if fallback.` This requires same-source provenance before visual correction and explicitly forbids fixed-coordinate comparison across cards. The LLM guide/catalog carry the same contract.
- Fresh current-source 1600 x 900 before/after evidence is at `artifacts\p160_same_image_corner_validation_20260720\llm_review_before\wpf_shell_host_outer_corner_llm_review.png` and `llm_review_after\wpf_shell_host_outer_corner_llm_review.png`. The final line is fully visible; focused smoke passed with layout/text/internal checks `0` and no execution side effect. P160 is **Complete**. The next card-corner evidence must use same-image marks/overlays for representative translated samples.

## P161 Same-Image Card-Corner Review Packet (2026-07-20)

- P161 turns the P160 provenance rule into a reusable visual-review packet instead of another cross-image coordinate comparison. `artifacts\p161_card_corner_same_image_review_packet_20260720` binds six current-run cases to copied exact source images, the unchanged card-corner XML, raw runner logs, actual runtime overlays, source-grid/runtime-overlay pairs, and SHA-256 bindings. The left cyan grid is only an operator coordinate aid; the right lime lines/cross are runtime output from that same copied source.
- Cases cover an ordinary OK image, the P157 Hough/projection investigation images, the `CornerOuterContourVerified=1` bright-contour case, a left-shifted card, and a high-in-frame corner. Their runtime intersections are recorded in `P161_SAME_IMAGE_REVIEW_PACKET.md`; no coordinate is treated as expected for another image. The current 500-row measurement replay completed Train `350/350`, Validation `75/75`, and Test `75/75`, with zero missing images.
- P161 is **Complete** as evidence preparation. It intentionally adds no tool change, XML gate, or LLM/provider claim. The remaining semantic card-gate dependency is an operator mark made on one or more of these exact P161 source grids, retained with its paired overlay and case ID; P162 separately records a mechanical false positive that must be repaired first.

## P162 Confirmed Hough Frame False Positive (2026-07-20)

- The operator correctly rejected P161 `05_left_shift_ng`. The exact current runtime drawing `artifacts\p161_card_corner_same_image_review_packet_20260720\cases\05_left_shift_ng\runtime_result.png` is labelled `hough` and chooses the image-bottom/frame candidate `(12,473) -> (346.319,473)` and a diagonal `(418,24) -> (346.319,473)`, rather than the card's adjacent lower/right outer edges. It is a confirmed false selection, not a valid card-corner measurement.
- Root cause is bounded and reproducible in `TryFindHoughCorner`: a horizontal candidate may be as low as `source.Height - 7`, `IsPlausibleBottomRight` accepts `y < source.Height - 6`, and the pair score rewards a lower/larger intersection plus longer lines. Thus the frame row at `y=473` in the 480 px source can outrank card geometry. This is not repaired by P161 and its 500/500 run must remain execution-stability evidence only.
- P162 identified the repair acceptance criteria. It is resolved by P163; preserve its false `05_left_shift_ng` drawing only as the before baseline. Do not add a simplistic fixed bottom margin: a valid card may occur close to an image edge.

## P163 Card-Boundary-Supported Hough Repair (2026-07-20)

- P163 repairs P162 in `1. Core\Pipeline\Tools\VisionPipelineOuterCornerIntersectionTool.cs`. When the configured bright threshold merges a card with the frame, the runtime finds a detached large bright candidate at progressively higher thresholds. Hough/projection candidates must be supported by that candidate's lower-right contour region; Hough horizontal candidates also require repeated inside-above/outside-below contour support. The threshold support is component-relative, not a fixed image-bottom exclusion.
- The exact user-rejected `05_left_shift_ng` changed from Hough frame corner `(346.319,473.000)` to outer-contour corner `(534.118,379.121)`. P163 visual review also exposed and repaired `06_low_corner_ng`'s internal horizontal-band selection, from `(532.311,276.524)` to `(530.321,392.620)`. The prior reviewed Hough `02_hough_ng=(547,389)` and low-contrast projection `NG_0172=(520.283,427.555)` remain executable. Current source/result pairs are under `artifacts\p163_card_boundary_support_repair_20260720`.
- Latest current-build measurement replay completed Train `350/350`, Validation `75/75`, Test `75/75`, zero missing rows. `CornerOuterContourVerified=1` occurs on `59/500` rows; that is only a line-source metric, never a semantic corner/gate claim. P163 is **Complete** for the concrete frame/internal-line false-positive repair. Same-image target marks remain necessary before a card judgement gate.

## P164 Card Virtual-Corner Definition (2026-07-20)

- A subsequent operator review invalidated P164's semantic interpretation. The actual requirement was to prove that the fitted lower line came from the physical card-bottom edge; changing the label to `Virtual corner` did not prove that boundary ownership and did not repair the selection.
- The exact replay remains useful execution evidence only: `(534.118,379.121)`, `CornerOuterContourVerified=1`, `ResultCount=1`, and `EdgeCount=2`. `CornerOuterContourVerified=1` means support from the selected threshold contour, not proof that the contour segment is the operator-intended card-bottom edge.
- P164 is **Incomplete** and superseded by the P165 strategy decision. Preserve `artifacts\p164_card_virtual_corner_definition_20260720` as a rejected interpretation baseline; do not cite it as correct card geometry or continue image-by-image tuning.

## P165 Inspection-Intent Skill Strategy (2026-07-20)

- The LLM-assisted recipe concept remains the product direction, but only as guided initial setup and evidence-backed correction. OpenVisionLab will not promise arbitrary image plus prompt -> autonomous correct inspection. The operator owns intent, ROI/measurement region, tolerance, and sample evidence; the LLM drafts constrained XML; deterministic tools, batch evidence, and explicit review own acceptance.
- Reusable in-product inspection-intent skills are now the primary development unit: intent -> required inputs -> locked existing tool family -> starter XML -> required metrics/gates -> explicit N-sample drawings/error table -> genuine correction packet -> held-out completion gate. These are product recipe-wizard/template contracts, not Codex plugins.
- Evidence supporting the decision is mixed but actionable: P151 proves one real GPT `PinArrayGap` direct-success draft; P147 proves that valid/runnable GPT XML can still score only 52.40% on an undifferentiated 500-image corpus; P148 proves that a narrowly named edge-clearance/pitch-consistency signal can pass frozen Train/Validation/Test evidence; P149-P164 prove that repeated generic card-corner tuning can execute without establishing the intended physical boundary.
- The first pilot is `Pin row gap / pitch consistency`, locked initially to `PinArrayGap` adjacent edge-to-edge clearance. Required inputs are a reviewed row ROI, pin polarity, edge-gap versus center-pitch intent, pixel-only versus calibrated units, expected tolerance, and an explicit sample split. Center-to-center pitch is not claimed until a separately verified runtime metric exists.
- At the P165 decision point, Phase 1 XML authoring/validation/import infrastructure was broadly present, Phase 2 still needed a complete skill workflow, and Phase 3 was limited. P167 and P168 subsequently complete Phase 1 and the bounded Phase 2 pilot. P169 now reserves a fresh held-out Test, but Phase 3 natural failure -> correction -> one-time held-out replay remains pending. `OuterCornerIntersection` stays experimental and outside the default LLM skill/recommendation priority.
- P165 is **Complete** as a durable product-priority decision. The global and project `AGENTS.md`, product target, current/chronological handoffs, next-chat prompt, LLM authoring guide, and machine-readable tool catalog carry the same rule. JSON catalog parsing, `git diff --check`, and `OpenVisionReadinessCheck` passed. No algorithm, UI, Preview/Run, layer, or routing behavior changed in this documentation slice.

## P166 Pin Row Edge-Gap Skill V1 Design (2026-07-20)

- `docs\OPENVISIONLAB_PIN_ROW_GAP_INTENT_SKILL.md` is the approved executable product contract for the first inspection-intent skill. The supported user-visible promise is now `Pin row edge-gap consistency`: one or more independently reviewed single-row ROIs, roughly vertical dark pins, adjacent edge-to-edge pixel clearances, and an explicit `DistancePxRange` maximum. Center pitch, bright pins, physical mm without calibration provenance, and unrelated pin defects remain WAIT/out of scope.
- The contract separates `MEASURE READY/MEASURED` from `JUDGEMENT READY/VALIDATION READY/COMPLETE`. A valid or runnable XML file without explicit acceptance fields is a measurement draft, not a judged recipe. This addresses P151: its unchanged GPT response is real direct-success measurement evidence, but it contains no acceptance gate even though a generic report line said explicit judgement existed.
- The design keeps the existing `Pin gap / edge distance (LineDistance)` workflow intact and adds a separate exact-match `PinArrayGap` template. It reuses existing ROI parsing, Local Validation Sets, batch outcome/error rows, Pipeline Review, Run History, and runtime drawing review. Three existing Validation Sets represent Train, Validation, and Test; no dataset database or new runner is introduced.
- P148 remains the two-row synthetic regression baseline: 356/72/72 split, frozen `DistancePxRange <= 6`, Test Good 36/36 accepted, and Test `pitch_error` 12/12 rejected. The value/ROIs are not universal defaults, a single top row does not cover every labelled pitch defect, and the already reviewed Test set cannot be presented as new blind Phase 3 evidence.
- P166 is **Complete** as the v1 design slice. It changes documentation only; the dedicated Guided Setup template, strict intent validator, UI state, and product-integrated N-sample evidence path are not yet implemented. Verification record: contract/reference scan, `git diff --check`, and `OpenVisionReadinessCheck` after final documentation edits.

## P167 Pin Row Edge-Gap Skill Phase 1 (2026-07-20)

- P167 implements the separate `Pin row edge-gap consistency (PinArrayGap)` Guided Setup path. It preserves the existing LineDistance intent and adds reviewed row ROI input, `Dark`/`Bright` polarity choice, edge-gap/center-pitch choice, optional range maximum, and the existing five PinArrayGap detection settings.
- Blank Range produces an importable measurement draft with no acceptance fields and visible `MEASURE READY / NOT JUDGED`. A positive Range produces one `PinArrayGap` Step per reviewed row with the same `DistancePxRange` maximum and visible `JUDGED XML READY / VALIDATION PENDING`. Bright pins and center-to-center pitch remain non-runnable WAIT states; v1 remains px-only.
- The deterministic starter and LLM prompt use the same locked contract. Strict validation now compares the returned tool family, enabled row count/order, exact ROI values, locked detection parameters, unique outputs, branch-input declaration, and judged maximum with the current Guided Setup state. It rejects unequal row maxima, a mixed tool family, missing ROI/detection fields, or a missing gate when the current state is judged.
- Focused current-source smoke proves measurement/judged creation, three negative XML mutations, Bright/center-pitch blocking, Import readiness, 11 visible controls, and unchanged Preview/Run/layer/routing/active-layer state. Before/after evidence and the completion record are under `artifacts\p167_pinarraygap_intent_skill_20260720`.
- P167 is **Complete** for Phase 1 authoring/validation only. It is not algorithm-image validation or multi-sample recipe-quality proof. Its recorded next step was Phase 2 integration with the existing Local Validation Set, error-table, Pipeline Review, and exact runtime drawings; P168 below completes that bounded step.

## P168 Pin Row Edge-Gap Skill Phase 2 (2026-07-20)

- P168 connects the P167 skill to three existing Local Validation Set selectors, freezes a versioned identity for the judged XML and Train/Validation/Test lists including each image's SHA-256, reports later selection/path/file-content drift as stale, and routes `Open explicit runs` into the existing Run History local-set path. Selecting, freezing, refreshing, or opening these surfaces does not run Preview/Run or change layers, routes, active layer, or preview results.
- The frozen P148 two-row XML was replayed unchanged from `artifacts\p148_pin1_all_pitch_measurement_20260720\pin1_dynamic_pitch_range_gate_train_frozen.xml` (SHA-256 `9F8F60E615B9F90CA9D010BE0EC43C0C897BDB3BE5BA0333CF810E0DE139A4F2`). It uses row ROIs `0,120,768,130` and `0,330,768,130`, with `DistancePxRange <= 6` on both `PinArrayGap` Steps.
- The exact pairwise-disjoint source split-list file identities are Train 356 / `4BD979B72B5AB6E61689C0609C05DB570658B77AC05AE4859D92914ED133F20E`, Validation 72 / `80D7B1895491459C909FB1565396EBB5F8DC4A463E7B3EF41DEEA00A9CF8747D`, and Test 72 / `F4F483C5FE01B54191D1FD2C1F6DA53D58D27437714D41F110D3F72057D6A3EC`. These are not the product record's separate canonical set-content hashes.
- Current-source replay had zero image-load/runtime errors. Train accepted expected Good 178/178 and rejected `pitch_error` 38/38; Validation accepted expected Good 36/36 and contains no `pitch_error`; frozen Test accepted expected Good 36/36 and rejected `pitch_error` 12/12. Full rows are under `artifacts\p168_pinarraygap_phase2_20260720\current_runner`.
- Cross-defect observations remain outside the skill: Train rejected bend 34/38, missing 35/38, short 2/38, and bridge 0/26; Validation rejected bend 11/12, missing 12/12, and bridge 0/12; Test rejected short 0/12 and bridge 0/12. These counts must not be presented as a whole-pin or other-defect classifier.
- Runtime/result evidence now draws the reviewed ROI, selected pin rectangles, every adjacent gap, and row metrics. Run Report storage keeps a SHA-256-verified run-time `source.png` and every executed `PinArrayGap` row drawing for the viewer instead of pairing a mutable external path with one collapsed result image. Representative exact-run overlays are under `artifacts\p168_pinarraygap_phase2_20260720\representative_overlays`; the final current-source multi-row viewer evidence is under `artifacts\p168_pinarraygap_phase2_20260720\multistep_current_source_verified`.
- P168 is **Complete** for Phase 2 of the dark-pin, pixel, adjacent edge-gap skill only. It does not prove center-to-center pitch, calibrated mm, or bend/missing/short/bridge/contamination classification. P148 Test was already reviewed and cannot serve as previously unused Phase 3 evidence.

## P169 Phase 3 Prerequisite And GPT Direct Success (2026-07-21)

- A new Phase 3 candidate corpus was selected without using P148's reviewed Test split: `D:\라벨테스트\Pin_2_Bad_Bent_500_OK_NG\Pin_2_Bad_Bent`. It contains 500 unique synthetic/augmented 768x576 grayscale images with native pairwise-disjoint Train 356 / Validation 72 / Test 72 lists. The split-file SHA-256 values are Train `1FE966E3C756EA17AA175A2B2E2ACD375175DF1B503596D0E2073C351F5A63F0`, Validation `972E7197CCE048A780C1EB0717817FD1AE17909FE471B70EED9689646C18BCDD`, and Test `B4679F515995DD654D667A5CEFB321FF3A5F709863AA1314D0A8ED093851DB84`.
- The 72-image Test list is frozen with 72 non-empty, unique per-image content hashes in `artifacts\p169_pin2_phase3_prerequisite_20260721\reserved_test_manifest.csv` (SHA-256 `60FD1EA7820919816EA168B6CC31F3C5932750F5DD75D831293381E9C12F06B6`). It contains Good 36, `pitch_error` 12, `short_pin` 12, and `bridge_contamination` 12; only Good plus `pitch_error` are the 48 target-intent rows. The Test list has zero image overlap with P148, and none of its 72 images appears in any P169 executed CSV. It was hashed and reserved, not visually reviewed or executed.
- A new ChatGPT conversation in the user's existing project received the product-constrained judged prompt only. No local path or image was transmitted. The exact prompt SHA is `4292A3A485BF361828D2F7802E73FFB1BB5F59628EBFDA1658C6CF21B9C5E3DE`; the unchanged first response SHA is `CB6BB116DCDD9572F6A3BB8D913ECB93881EB308B0F6FEF188037B45F2943F6B`. The conversation is `https://chatgpt.com/g/g-p-6a57516431548191a9e4f7c95505200c-rulbeiseu-llm-yeondong/c/6a5e9ded-1d64-83ee-9c6a-8195c2111326`. The web UI did not expose a reliable model identifier, so the model is recorded as unknown rather than inferred.
- The first response used the requested two dark-pin row ROIs `0,120,768,130` and `0,330,768,130`, the locked detection values, and `DistancePxRange <= 6` on both `PinArrayGap` Steps. Current product strict validation passed with zero errors and one non-blocking multi-result/OverlayMerge review warning. There were no external dependencies.
- Permitted Train/Validation replay used the unchanged response. Train had zero load/runtime errors, accepted Good 178/178, and rejected target `pitch_error` 38/38. Validation had zero load/runtime errors and accepted Good 36/36; its native split contains no `pitch_error`. Other defect outcomes are observations only and are not claims for this skill. Exact CSV rows and visually checked ROI/pin/gap drawings are under `artifacts\p169_pin2_phase3_prerequisite_20260721`.
- The response therefore succeeded directly. No genuine failure existed to correct, no correction prompt was sent, and the reserved Test was not run. P169 is **Blocked** for Phase 3 by the missing natural first-draft failure, not by missing held-out data. Repeating equivalent prompts until one fails would manufacture evidence and is prohibited.

## P170 Target-Bearing Working Validation Readiness (2026-07-21)

- P169's native Validation contains Good 36 but no `pitch_error`, so it cannot reject a future correction candidate that misses all target pitch defects. P170 closes only this evidence-design gap; it does not call an LLM, alter XML, run an algorithm, or open/execute the reserved Test images.
- `artifacts\p170_pin2_target_validation_readiness_20260721\working_train_target_manifest.csv` freezes Good 178 / `pitch_error` 26, with file SHA-256 `D3A35087CFB2AFA26D5A1D9EB67FE72A224F7BFC6B86FADBFCD87CCFC8D02745`.
- `artifacts\p170_pin2_target_validation_readiness_20260721\working_validation_target_manifest.csv` freezes Good 36 / `pitch_error` 12, with file SHA-256 `952BAEA1038C0A8AD77524D685E6F69A5CA60E3D539F4CF817147E9EAF30B90B`.
- The 12 working-Validation pitch rows are the deterministic lowest content hashes from P169 native Train, with `RelativePath` as tie-break; they are removed from Working Train. The two working manifests cover all 252 in-scope non-Test rows exactly once, have zero path/content overlap with each other, and have zero path/content overlap with the 48 target rows in the frozen P169 Test manifest.
- All 252 working rows were already executed during P169, so this is explicitly `previously observed working Validation`, not blind or unused evidence. It may check a future naturally failed response and its correction before Test, but it cannot retroactively turn P169 into a correction loop or replace the one-time held-out Test gate.
- P170 is **Complete** for split readiness only. The first skill's Phase 3 remains dependent on a future judged first response that fails naturally during normal use.

## P171 Local Validation Set Provisioning Audit (2026-07-21)

- Audited whether the two P170 CSV manifests are already usable through the recipe-local Validation Set path. All 252 non-Test rows resolve to existing supported images, keep valid OK/NG roles, fit the existing 64-set/5,000-image limits, and retain zero duplicate or cross-set path overlap.
- They are not directly selectable today. The UI supports multi-file selection or one top-level folder assigned wholly OK or NG, while P170 contains selective Train/Validation subsets drawn from the same physical OK/NG folders. There is no CSV/manifest parser or import command, and the evidence-logical `datasets/Pin_2_Bad_Bent/...` path requires an explicit reviewed mapping to the local corpus root.
- No general CSV importer was added. A single blocked Phase 3 staging artifact does not yet justify a product-wide path-mapping contract, no target recipe exists in the retained Debug workspace, and adding UI now would be speculative. When a natural judged failure and target recipe exist, perform a narrow dry-run merge using explicit manifest, corpus root, recipe name, and set names; verify all paths/roles/hashes/overlap before writing only the two working sets. Do not register or execute the reserved Test at that stage.
- P171 is **Complete** as a no-change readiness audit. Current-source baseline capture and the reusable decision record are under `artifacts\p171_validation_set_manifest_import_20260721`. The project priority remains the natural first-response failure; this audit must not be reopened unless a target recipe exists or the same manifest-import need recurs in another skill.

## P172 Device Top-Left Black-Band Gap Measurement (2026-07-21)

- The operator selected the vertical pixel thickness between the upper and lower physical edges of the long black horizontal strip in `device_top_left`. This is a one-Step `LineDistance` intent, not `PinArrayGap`, white clearance, or a card corner. No calibration, nominal thickness, or tolerance was supplied, so the recipe remains pixel-only and measurement-only.
- A real GPT first response and same-conversation correction were preserved without transmitting a local image, path, dataset, or private file. The first response parsed and ran but its broad ROI produced a genuine semantic drawing failure: 25/63 reference lines reached unrelated lower hardware. The correction used only that runtime evidence plus the operator-reviewed reference ROI `20,200,510,60`.
- After a fresh 0-warning/0-error full build, the corrected XML passed current `bin\Debug` validation/import and explicit reference execution. Its 64 vertical lines joined the inspected strip edges with `DistancePxMin=22`, `Max=38`, `Avg=28.219`, and `Range=16`. Source, XML, reports, and opened runtime drawings are under `artifacts\p172_device_top_left_black_band_gap_20260721`.
- Both XML variants were also replayed over the corpus' 350/75/75 lists. The first response mechanically succeeded on 461/500 images; the fixed reviewed-ROI correction succeeded on 382/500. Representative drawings prove that mechanical success is not semantic success and that large pose changes move the strip outside the fixed ROI. The corpus OK/NG labels describe local synthetic defects, not strip-thickness truth, so these counts are not accuracy claims and no split remains blind after this diagnostic.
- P172 is **Complete** only for the bounded reference-pose GPT failure -> correction loop and the fixed-ROI corpus boundary audit. It changes no production source and does not complete the first `PinArrayGap` skill's Phase 3. A 500-image-capable strip recipe requires either pose-stable acquisition or a separately verified rotation/scale-aware fixture before further LLM tuning; Good/NG and millimetre claims additionally require tolerance and calibration.

## P173 Device Top-Left Similarity-Fixture Contract (2026-07-21)

- The operator approved continuing the P172 pose decision without asserting a fixed-acquisition setup. A deterministic 24-image audit selected four filename-quantile OK and four NG images from each Train/Validation/Test split. Reviewed audit overlays observed strip center Y `45.55..361.23 px`, angle `-2.544..+2.154 deg`, visible length `435..640 px`, and outer thickness `36.70..78.66 px`. The 315.68 px vertical movement rejects one fixed narrow ROI; the rotation/scale range also rejects treating X/Y-only translation as the final metrology contract.
- Current source remains translation-only: Matching fixture frames do not publish scale, angle is only a fail-closed gate, and runtime moves only a cloned common `CvROI.X/Y`. Pipeline Matching scale-search round trip, LineDistance fixture PropertyGrid support, and coordinate-correct per-Step report drawings are not implemented.
- P173 selects a bounded three-Step design: `Matching` locates one operator-reviewed rigid feature, fixture-driven `RotateScale` applies the inverse similarity transform into a reference-coordinate `DeviceAligned` layer, then the unchanged `LineDistance` ROI measures that aligned layer. Rotated-ROI enclosing boxes, generic affine frame graphs, perspective correction, per-image tuning, OK/NG claims, and uncalibrated millimetre claims are excluded.
- The reusable design is `docs\OPENVISIONLAB_MATCHING_SIMILARITY_FIXTURE_V2_SPEC.md`; dataset evidence and the work contract are under `artifacts\p173_device_top_left_similarity_fixture_contract_20260721`. The audit drawings are explicitly heuristic decision evidence, not current OpenVisionLab runtime output.
- P173 is **Complete as a design/decision slice only**. No production source, LLM guide, or tool catalog changed because the proposed XML contract is not implemented. At P173 closure, runtime work was blocked on a stable locator and reviewed pose limits; P174/P175 subsequently resolved C9 selection, reference pose, and the observed angle/scale search envelope. A deployable score/ambiguity gate remains open, but it does not block the isolated Pipeline/XML round-trip plumbing slice. This is a bounded P172 follow-up, not completion of a second inspection-intent skill and not `PinArrayGap` Phase 3.

## P174 Device Top-Left Locator Candidate Audit (2026-07-21)

- Image registration clarifies that the operator-marked screenshot comes from `device_top_left_OK_0001.jpg` (SHA-256 `4EDD5C5B36ACE3053066AD810E2F5CF75C0E5EFA5C5EC2F047289D74B65C5241`), while P172's current-build runtime replay used the separate corpus `source.png` copy (SHA-256 `30766834777142F2DBA57265A27E591EDF926A324D5BA546EC74E9F2D468483C`). They are not byte-identical, so P174 teaches the locator on the exact operator-marked OK_0001 reference and keeps P172 runtime claims bound to `source.png`.
- The initial `P0=130,260,200,35` locator matched the deterministic 24-row prototype, but the all-500 audit found NG mask overlap on 82/250. In fully visible `NG_0248`, 1,531 defect pixels entered P0 and both normalized/fixed matches jumped about 274/300 px to the wrong region. P0 is rejected and retained as failure evidence.
- The recommended replacement is `C9=240,270,65,60`, a compact rigid joint 10 px below the measurement ROI with zero intersection area. Its frozen 24-row OpenCV-Python multiscale prototype had scores `0.815127..1.0`, center-consensus error `0.092..1.776 px`, and 24/24 visually reviewed correct-locator drawings. This is prototype evidence, not `Lib.OpenCV.MatchingTool`, XML, Pipeline, or EXE evidence.
- Independent all-500 registration reports C9 fully visible on 491/500 and at least 90% visible on 494/500; every fully visible row passed the pose-normalized heuristic (`491/491`). The nine remaining rows are crop/visibility boundaries. C9 intersects 43/250 NG masks, so the result is not universal or production-qualified; fixed-scale matching passed only 404/500 and has a reviewed 202.5 px wrong-region case, confirming the need for scale-aware location.
- Evidence is under `artifacts\p174_device_top_left_locator_candidate_20260721`, including the exact references/templates/configs, rejected P0 run, all-500 candidate tables, every 24-row overlay, three split contact sheets, and representative/boundary/failure drawings. No production source or supported XML contract changed.
- The 24-row prototype script is retained and rerunnable. The independent all-500 audit was executed as inline Python and its exact script was not retained; the artifact preserves method settings, all 500-row transforms/tables, hashes, and drawings but must not call that audit exactly replayable.
- The user approved proceeding with C9 as the locator feature. P174 is therefore **Complete for candidate selection**; P175 owns the separate native qualification. This approval does not itself approve Similarity Fixture V2 or an OK/NG gate.

## P175 Device Top-Left C9 Native Matching Qualification (2026-07-21)

- Added the focused `matching-c9-batch` scenario to the existing actual-EXE direct smoke runner. It opens the real Matching Tool View, requires explicit evidence inputs and a new/empty output, applies one frozen PropertyGrid configuration with `AUTO_PREVIEW=false`, replaces `Main`, invokes one explicit Preview per case, and preserves the actual `Matching_Preview` layer, review values, source/config/runtime hashes, and comparison drawings. The reserved `Smoke_MatchingC9_<12 hex>` workspace is deleted in `finally`; no normal product UI, routing, layer, Preview, or Pipeline behavior changed.
- The current EXE (`E24315F0...BE8A`), current entry assembly (`84D0CF7E...7793`), and vendored `Lib.OpenCV.dll` (`E7F662C7...59D4`) passed three synthetic field-semantics cases: `0.8/-3`, `1.0/0`, and `1.8/+3`, with center error `0,0,1 px`. Across calibration plus dataset, image loads caused zero Preview increments and explicit Preview caused exactly one increment with a fresh native result/file in `27/27`; no obsolete or reserved Smoke recipe remained after exit.
- The exact P174 24-row observed set passed `24/24`. Minimum score was `80.358`, maximum independent-center error `2.032 px`, minimum box/polygon IoU `0.895`, maximum scale error `0.05691`, and maximum P173 local strip-angle error `0.92995 deg`. All Train/Validation/Test contact-sheet drawings were opened; every native box selected the intended C9 joint.
- Whole-device ORB rotation is retained only as a diagnostic because it differs from the local black-strip tangent being normalized. Its maximum difference was `2.17368 deg`; the same row's physically relevant local-strip error was `0.59277 deg`. The failed r2 global-angle gate is superseded rather than hidden.
- r1 failed report production, r2 used the wrong whole-device angle oracle, and r3 passed algorithm geometry but failed later harness-hygiene review. Evidence: `artifacts\p175_device_top_left_c9_native_matching_20260721_r4`. P175 is **Complete for current native Matching Tool View qualification only**. `SCORE_MIN=0` was evidence-only, so no deployable score threshold, Pipeline XML scale round trip, fixture normalization, black-strip Gap result, OK/NG classification, calibrated unit, unseen-data robustness, or production readiness is claimed.

## P176 Die Pad 1 Native Matching Batch Qualification (2026-07-21)

- The operator correctly rejected the `device_top_left` C9 fixture patch as a representative general Matching benchmark. C9 remains bounded to the P172 black-strip Gap fixture workflow. P176 instead uses the dedicated synthetic `EasyMatch_Die_Pad_500` corpus and freezes only the `Die Pad 1.bmp` source family so one template identity is not mixed across four source variants.
- Added the test-only `matching-die-pad-batch` actual-EXE scenario. It selects all 122 metadata rows for `Die Pad 1.bmp` (Train 82 / Validation 27 / Test 13; role labels OK 62 / NG 60), crops template ROI `90,150,270,220` from Train/OK `die_pad_001_ok.jpg`, applies one frozen native PropertyGrid configuration, and saves the exact source, `Matching_Preview`, parsed result, source/config/runtime hashes, and evidence overlay for every row. Loading an image remains non-executing and each explicit Preview increments the run count exactly once.
- Current r3 passed `122/122`; minimum/average/maximum displayed score was `89.689 / 95.271 / 99.347`. The explicit-run contract passed `122/122`, every expected source/preview/overlay existed, and no reserved `Smoke_MatchingDiePad_<12 hex>` workspace remained. The current EXE SHA-256 is `E24315F0...BE8A`, entry assembly `93844BE1...1B99`, native `Lib.OpenCV.dll` `E7F662C7...59D4`, and template `E4C5D8F9...7B97`.
- The final source state passed `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` with 0 warnings/0 errors and the readiness checker with every contract `OK`. An independent artifact audit re-hashed all 122 source files against metadata and found zero missing/empty preview or overlay files.
- All Train/Validation/Test contact sheets, the representative/boundary sheet, and the minimum-score `train_NG_die_pad_109_ng` overlay were opened. The runtime yellow boxes and green centers remain on the same intended pad/trace corner despite synthetic defects; no wrong-region drawing was accepted. r1/r2 are retained as rejected command-line quoting failures that selected zero rows; neither ran Matching.
- P176 is **Complete for current native Matching Tool View single-target locator qualification on this one synthetic source family only**. Evidence: `artifacts\p176_die_pad_1_native_matching_20260721_r3`. Corpus OK/NG are defect-role labels rather than Matching pass/fail truth. The generator does not persist per-image transform matrices, so this result does not claim exact center/angle/scale error, four-source generalization, real-camera robustness, defect classification, Pipeline XML round trip, or production readiness.

## P177 Operator-Approved Zero-Degree Die Pad Matching Template (2026-07-21)

- The operator rejected treating P176's broad `90,150,270,220` context crop as the final feature-point template and selected the lower-right pad/trace/outer-corner feature. P177 freezes the approved ROI as `190,220,175,145`; it contains the right two pads and asymmetric holes, stepped traces, and the outer bottom/right L corner. The old P176 artifact remains a comparison baseline.
- The reference Train/OK image is visibly tilted. The new `zero-reference` profile detects the longest approved horizontal outer baseline inside search ROI `170,320,210,60`, measured `-1.789910608°`, rotates the full 512x512 reference about the approved ROI center by the same `-1.789910608°`, then re-detects a `0.000°` residual before cropping the template. The evidence drawing preserves before/after images and lines. This zero definition is horizontal baseline rectification only; it does not remove the source's mild perspective or claim physical calibration.
- A fresh full Debug build passed with 0 warnings/0 errors before the current EXE run. The same actual Matching Tool View, one frozen 0° template, and explicit Preview path passed `122/122`: Train 82/82, Validation 27/27, Test 13/13, OK role 62/62, NG role 60/60. Score min/average/max was `85.554 / 96.426 / 99.629`; reported angle range was `-2.5..+2.5 deg` and scale range `0.9..1.15`. Every load caused zero Preview increments and every explicit Preview exactly one (`122/122`).
- Independent audit found 122/122 source MD5 matches and zero missing source/native-preview/overlay files. All Train/Validation/Test sheets, the zero-degree before/after drawing, the template ROI drawing, the representative/boundary sheet, and minimum-score `val_NG_die_pad_198_ng` overlay were opened. Every runtime yellow box and green center remained on the approved feature; the minimum row stayed correct despite a synthetic defect crossing the left part of the template.
- Evidence: `artifacts\p177_die_pad_1_zero_degree_matching_20260721_r1`. P177 is **Complete for the operator-approved 0° single-target native Matching template and this 122-image synthetic source family only**. It does not classify defects, provide per-image transform ground truth, qualify the other three Die Pad source families, prove real-camera robustness, or implement Pipeline/XML pose/scale round trip.

## P178 Object-Bounded Zero-Degree Die Pad Matching Template (2026-07-21)

- The operator correctly rejected P177's residual uniform area below the physical feature. P178 preserves P177 and adds the separate `object-only-zero-reference` profile with ROI `190,220,175,130`. The 15 px height reduction keeps the right two pads/holes, stepped traces, and outer bottom/right L boundary while leaving only a small boundary margin instead of teaching the broad lower background.
- The same reference baseline measured `-1.789910608 deg`, the full reference was rectified by the same angle, and the residual was `0.000 deg` before the 175x130 template was cropped. This remains horizontal-baseline rectification, not perspective correction or calibration.
- A fresh full Debug build passed with 0 warnings/0 errors. The current EXE then passed 122/122 actual Matching Tool View runs: Train 82/82, Validation 27/27, Test 13/13; role labels OK 62/62 and NG 60/60. Score minimum/average/maximum was `84.731 / 96.272 / 99.568`; every load caused zero Preview increments and every explicit Preview exactly one.
- Independent audit found zero source MD5 mismatch, zero Preview-contract violation, and zero missing source/native-preview/overlay files. The exact template, ROI drawing, representative sheet, and minimum-score `val_NG_die_pad_198_ng` runtime overlay were opened. Every reviewed runtime box selected the intended object; the minimum row remained correctly localized at center `(266,261)`, angle `+1.5 deg`, and scale `0.95`.
- Evidence: `artifacts\p178_die_pad_1_object_only_zero_degree_matching_20260721_r1`. P178 supersedes P177 as the operator-approved generic Matching template. Its existing matcher is rectangular rather than alpha-masked, so dark pixels inside the tight object bounding rectangle remain part of the template. It does not classify defects, provide exact pose truth, qualify other source families, or implement Pipeline/XML pose/scale round trip.

## P179 Matching Pose/Scale Pipeline/XML Round Trip (2026-07-21)

- The existing native `MatchingProperty` angle and uniform-scale search fields now survive Pipeline builder -> Recipe Manager PropertyGrid -> XML save/load -> apply-back -> app tool factory without introducing a second matcher. Validation requires positive scale min/max/step with minimum not above maximum, and fixture reference scale must be positive.
- A fixture-producing Matching Step now publishes `FixtureCenterX/Y`, `FixtureAngle`, `FixtureScale`, and `FixtureScaleRatio`. The external overlay contract has bounds and angle but no scale field, so the app derives uniform scale from the runtime match bounds versus the resolved template dimensions, verifies X/Y consistency, and snaps to the configured native scale-search grid. Startup-relative template paths use the same resolver as the app tool factory.
- Pipeline Review's explicit `Save as reference` now persists center, angle, and scale, invalidates stale results, and still performs zero Preview/Run. The fixture consumer remains translation-only: it uses only X/Y offsets and does not rotate or scale a downstream ROI.
- Fresh UI evidence shows `기준 배율` in the existing PropertyGrid and the reviewed/saved pose text includes scale. The focused smokes preserve the scale-search fields and reference scale through PropertyGrid/XML round trip and assert unchanged Preview count.
- A current Debug EXE executed one startup-relative XML on the rectified nominal image and three exact P178 corpus rows. All four validated with zero errors/warnings and ran successfully. The three shared P178 rows reproduced native Tool View geometry: `die_pad_109_ng` center `(320,269)`, angle `+3`, scale `0.90`; `die_pad_081_ok` `(259,282)`, `-2.5`, `0.95`; `die_pad_199_ng` `(278,278)`, `+2.5`, `1.15`. Current-run rotated boxes were opened and confirmed on the operator-approved two-pad/stepped-trace/outer-corner object.
- Evidence: `artifacts\p179_matching_pose_scale_pipeline_roundtrip_20260721`. P179 is **Complete for Matching pose/scale Pipeline/XML round trip and evidence publication only**. It does not preserve reference image dimensions, normalize an image, transform a measurement, establish an operating score threshold, or prove Gap/OK/NG/calibrated behavior.

## P180 Matching Report Angle Convention And Host PropertyGrid Theme (2026-07-21)

- The operator correctly challenged the P179 `+2.5 deg` report drawing. The native Matching Tool View uses image coordinates, where its positive-angle polygon transform is `X = Cx + x*cos + y*sin`, `Y = Cy - x*sin + y*cos`. The persisted report renderer had applied the mathematical/System.Drawing sign instead, so the reported rectangle visibly tilted opposite to the native yellow rectangle even though center, score, angle, scale, and bounds metrics were correct.
- Report rendering now negates the published Matching angle before rotating the rectangle, matching the native polygon convention. A short yellow local-X direction line from the center makes the applied orientation auditable. The same unchanged P179 XML and exact `die_pad_199_ng` image replayed at center `(278,278)`, angle `+2.5 deg`, scale `1.15`, score `97.064`; the corrected corners are approximately `(174.824,207.433)`, `(374.633,198.709)`, `(381.176,348.567)`, `(181.367,357.291)`.
- The WPG-CUSTOM-derived light PropertyGrid remains the default for all algorithm Tool Views. The bridge now exposes an instance-scoped `Default`/`Dark` theme variant, and only Recipe Manager's dark Step editor opts into `Dark`. It recolors rows, names, editors, range controls, search feedback, and dialog buttons without rebuilding or modifying `C:\Git\WPG-CUSTOM` or globally replacing the vendor resources.
- Focused current-source UI smokes assert Recipe Manager uses `Dark`, Matching Tool View remains `Default`, expected properties remain browsable, and parameter load/theme selection does not trigger Preview/Run. Fresh before/after captures and the corrected current-build runtime overlay are under `artifacts\p180_matching_angle_and_property_grid_host_theme_20260721`.
- P180 is **Complete for report-angle drawing consistency and the two verified PropertyGrid host variants only**. It does not normalize the source image, rotate/scale a downstream ROI, add arbitrary host palettes, or change Matching detection. The next project priority remains reviewed reference dimensions plus fail-closed inverse-similarity `NormalizeImage`.

## P181 Matching Similarity NormalizeImage (2026-07-21)

- Pipeline Review reference teach now saves the reviewed source width/height together with Matching center, angle, and scale. The Recipe Manager PropertyGrid and XML round trip preserve those producer fields plus `RotateScale` fixture mode, frame name, minimum valid-pixel ratio, and explicit branch intent without triggering Preview/Run.
- The existing `RotateScale` family now has a bounded `FIXTURE_APPLY_MODE=NormalizeImage` path. It applies the inverse current-to-reference center/angle/uniform-scale transform to the full unannotated source and writes a new reference-sized layer. Fixed Angle/Scale behavior is unchanged when fixture mode is off.
- Runtime and validator paths fail closed on missing reference dimensions, source/reference size mismatch, invalid pose/scale/angle, an unavailable/wrong-source frame, ROI/masks on the normalization Step, or an invalid/insufficient valid-pixel ratio. Runtime metrics and overlays expose reference/result dimensions, valid coverage, applied correction, valid boundary, reference axes, and reference center.
- The focused current-build pipeline/XML smoke passed identity, angle `-5/+5 deg`, and uniform scale `0.8/1.2`. Reviewed-region mean absolute differences versus the reference were `0 / 2.225 / 2.208 / 2.990 / 2.016`. Missing dimensions, mismatched dimensions, and an invalid coverage gate all failed closed; fixed `RotateScale` compatibility passed.
- Final verification passed: full Debug build (`0` warnings/errors), `OpenVisionFixtureSmoke`, the two focused `PipelineViewerScreenshotSmoke` targets, `OpenVisionReadinessCheck`, `TestExternalReferences.ps1`, `TestPublicSampleAssets.ps1`, catalog JSON parsing, and `git diff --check`. Fresh before/after PropertyGrid and reference-teach captures plus every current-run source, clean normalized image, Matching overlay, normalized overlay, XML, and report are under `artifacts\p181_matching_similarity_normalize_image_20260721`; the final UI captures are in `after_verified`. P181 is **Complete only for reviewed dimensions and the bounded `Matching -> NormalizeImage` slice**. It does not prove C9 strip-edge selection, `LineDistance`, OK/NG, calibration, all-500 behavior, or field robustness.

## P182 C9 Normalized LineDistance Coordinate Evidence (2026-07-21)

- The exact P175 24-row Train/Validation/Test manifest, frozen C9 template/reference pose, and unchanged P172 pixel ROI `20,200,510,60` were replayed without per-image tuning. The supported route is `Main -> Matching`, the same clean `Main -> RotateScale NormalizeImage -> DeviceAligned`, then `DeviceAligned -> LineDistance`.
- Pipeline `LineDistance` now preserves raw edge-point intersections by default. When the existing paired `USE_EXTEND_FIT_LINE=true` mode is selected, it connects scan lines to the two fitted boundaries, discards endpoints outside the source image or configured ROI, and publishes runtime overlays for the measurement ROI, both fitted edges, detected edge points, and final distance lines.
- The normalized path produced mechanical/ROI-valid measurements on `24/24`; the identical LineDistance parameters on unnormalized `Main` executed on only `18/24`. Normalized `DistancePxAvg` was `38.5..50.5`, maximum `DistancePxRange` was `23`, minimum Matching score was `80.367`, and minimum valid-pixel ratio was `0.309`. These are observed metrics, not operating gates or Gap truth.
- Every row retains the exact source copy, clean normalized image, Matching overlay, normalization overlay, measurement overlay, raw-control overlay, source hash, and both XML hashes. Train/Validation/Test contact sheets plus minimum-average, maximum-average, maximum-range, high-scale/minimum-count, and raw-failure rows were opened and visually checked. The final post-build evidence root is `artifacts\p182_c9_normalized_gap_20260721_r10`; r8/r9 are matching pre-final passes, while rejected r1-r7 are not completion evidence.
- P182 is **Complete for coordinate-correct C9 pixel measurement and drawing evidence on the exact observed 24-row set only**. It does not establish score/ambiguity/pose operating thresholds, black-strip OK/NG labels, calibration, all-500 coverage, unseen robustness, or production readiness.

## P183 C9 Fail-Closed Pre-Measurement Gate (2026-07-21)

- Added `ScoreMargin` for `Matching`/`TemplateMatching` only when `NUM_MATCH=2`. It is best minus second-best score in percentage points; when only one candidate survives `SCORE_MIN`, the missing second candidate contributes zero. The C9 pipeline uses a separate two-candidate preflight with `SCORE_MIN=0.8` and `ScoreMargin >= 10`, then preserves the fixture producer's existing `NUM_MATCH=1` contract.
- Added optional paired fixture parameters `FIXTURE_MIN_SCALE_RATIO` and `FIXTURE_MAX_SCALE_RATIO`. Both must be present with `0 < minimum <= maximum`; older recipes without either parameter retain prior behavior. C9 freezes `0.8..1.8`, keeps absolute angle delta `<= 5.25` degrees, and keeps NormalizeImage valid-pixel ratio `>= 0.25`. PropertyGrid mapping, XML round trip, parameter schema, known metrics/preset, LLM guide, and tool catalog expose the contract.
- The exact P175 24-row observed set passed the three-Step pre-measurement gate `24/24`; minimum best score was `80.367`. Deliberate no-target and exact-duplicate cases failed at Step 1, an 8-degree case failed at Step 2's angle gate, a 1.9x case failed at Step 2's scale gate, and a `0.227` valid-pixel case failed at Step 3. The angle diagnostic widened search to ±10 degrees only so the downstream ±5.25-degree gate could be reached; the coverage diagnostic widened scale to 2.1 only so the downstream 0.25 coverage gate could be reached. Neither diagnostic override is part of the operating XML.
- Fixture-publish failures now preserve the just-produced Matching metrics and rotated rectangle in the failed Step result instead of discarding that visual evidence. The current-run source, XML/hash, failure reason, and overlay are retained together. Final evidence is `artifacts\p183_c9_fail_closed_thresholds_20260721\gate_r6`; earlier `gate_r1` through `gate_r5` are diagnostic/pre-final runs and are not completion evidence.
- P183 is **Complete for the bounded C9/P175 starter operating policy and deliberate pre-measurement failure gates only**. It is not a general Matching default, Gap OK/NG truth, all-500 or unseen robustness, calibration, production tolerance, or field qualification.

## P184 Device Top-Left Full-Corpus Guarded Gap Replay (2026-07-21)

- Added one focused `--c9-gap-corpus` evidence path to `OpenVisionFixtureSmoke`. It freezes the P183 ambiguity/pose/coverage gates, appends the unchanged P182 `LineDistance` ROI `20,200,510,60`, saves one XML identity, and replays it without per-image tuning. It is a verification harness, not a new algorithm or product fallback.
- The exact corpus contains 500 unique 640x480 images (`OK` label-only 250, `NG` label-only 250). The P175 reference hash matches `device_top_left_OK_0001.jpg`; the manifest SHA-256 is `1A103450773D9E0242BA2EAAD51F6EC6744EDFF32DCC218DBF08E74E7755DEEA`, and the executed XML SHA-256 is `8963A7528EBDEF493541C5CF6E781BB4F7A5ABCD04E92C7B802A5D86D8D1E1CB`.
- The frozen chain measured 487/500 rows and rejected 13 before or at fixture publication: 10 `Gate1_NoTargetOrLowScore` and 3 `Gate2_Scale`. There were no ambiguity, angle, coverage, measurement, unclassified, load, or thrown runtime failures. Measured rows were 244 OK-label-only and 243 NG-label-only; rejected rows were 6 and 7 respectively. These are execution counts, not defect-classification accuracy.
- Successful pixel results had `DistancePxAvg` min/median/max `20.308 / 46 / 51.512`, `DistancePxRange` min/median/max `0 / 7 / 37`, angle delta `-3..3` degrees, scale ratio `0.8..1.8`, and valid-pixel ratio `0.292..1`. Every successful final distance line remained inside the reviewed ROI. The lowest-count row had 13 measurements; the maximum-range row had 42 measurements, range 37, scale 1.7, and coverage 0.334.
- Visual review opened the ordinary reference, minimum score/margin, angle/scale/coverage boundaries, minimum/maximum Gap, maximum range, both gate-failure types, and the paged clusters containing the largest ranges. The majority selected the intended black-strip edges. Large-range and low-count rows coincide with severe pose/crop/occlusion conditions and remain explicit review evidence; without independent Gap tolerance truth they are not reclassified or used to tune the frozen gate. No repeatable runtime/reporting defect was established.
- Evidence is `artifacts\p184_c9_full_corpus_gap_20260721_r1`: 500 source copies and hashes, 500 runtime results, 1,964 current-run executed-Step overlays, 22 contact/representative sheets, frozen XML/reference/template/manifest identities, `rows.csv`, `representatives.csv`, `VISUAL_REVIEW.md`, and the verification record. P184 is **Complete for the full supplied `device_top_left` corpus and this reviewed pixel ROI only**. It is not OK/NG truth, a production tolerance, calibrated units, unseen robustness, another direction, or field qualification.

## P185 Device Top-Right Gap/Locator Approval Candidate (2026-07-22)

- Audited the separate `device_top_right` corpus before XML authoring. The canonical reference is `source.png`, SHA-256 `870DA834B70EF17143E7E097E92A66B58E40C882DF026D104D10F005E956A018`; it is not byte-identical to `OK_0001` and no top-left coordinate/template was mirrored or reused.
- Three Gap widths and three non-overlapping locator regions were compared with SIFT/RANSAC projection, screen visibility, reference texture, and NG defect-mask overlap. The first audit incorrectly accepted several degenerate `0.002x/172-degree` homographies from eight inliers; it is rejected. The corrected audit requires at least eight inliers, scale `0.5..3.0`, and absolute angle `<=10` degrees and accepts 492/500 rows for design projection.
- The provisional approval candidate is green Gap ROI `330,245,260,40` over the long black strip and magenta locator ROI `460,286,70,52` over the center joint. Their area overlap is zero. Of 492 accepted projections, at least 90% of the Gap ROI was visible in 435 rows and at least 90% of the locator in 440 rows. The alternatives had lower locator visibility: left texture 344 and right joint 323 rows at the same 90% criterion.
- Opened the canonical approval drawing, locator crop, and selected-only variant sheet containing ordinary, minimum/maximum scale, maximum angle, lowest locator visibility, minimum-inlier, and mask-overlap cases. Labels are visible and the green region follows the intended black strip in the reviewed projections; the magenta region follows the center joint when it remains in frame. These are manually reviewed design projections, not runtime detections.
- Evidence is `artifacts\p185_device_top_right_contract_candidate_20260722_r5`; config SHA-256 is `B5D85A4833CFF2565A2E95B5A237EC79C792916EE12A8BDDC5D50E2D4258DB32`, audit-row SHA-256 is `5F16A6FF7E36473C8F3894ED78ACF342D80283A9B653F6053E7D07EDBEA2397C`, and the selected approval drawings have SHA-256 `8E6C9B4E7406ECD81B5CB935472D62A608D93E40A4D86927483D562AB3EB3063` and `2A16E88FE5C51C1741ECB9863F6B3E5C8766F9672C2794FDE6BCC640D01C64F1`. This r5 locator proposal is historical and superseded by the operator's Gap-only correction below; do not revive it.

## P186 Device Top-Right Gap-Only Correction And Small-Split Evidence (2026-07-22)

- The operator confirmed that the target is only the vertical thickness of the long dark strip marked as the upper-plate/lower-plate Gap and explicitly rejected the locator/Matching concept. The corrected contract therefore contains one `LineDistance` Step on raw ROI `330,245,260,40`; it contains no Matching, locator, template, NormalizeImage, hidden pose correction, acceptance tolerance, or calibration claim.
- The latest-built runner executed the canonical source plus ten representative raw images and retained copied sources, hashes, logs, result metrics, and current-run overlays. The ten-image batch completed all rows with five runtime successes and five `LineGaugeEdgeNotFound` fail-closed outcomes.
- Visual review prevents a false completion claim. The canonical source measures the intended pair at `DistancePxAvg=22.485`, `DistancePxRange=3`, and `NG_0250` visibly follows the strip, but `OK_0001` returns only `3..6 px`, `OK_0024` loses the upper boundary, and `OK_0111`/`OK_0186` measure a different lower structure after the intended strip moves above the fixed ROI. Failure drawings now retain the exact XML ROI and visibly show the miss.
- Three larger raw-coordinate ROIs mechanically executed 10/10 but connected unrelated distant edges. Their maximum within-row `DistancePxRange` values were `113`, `196`, and `189` px, so all are rejected; higher execution count is not semantic Gap evidence.
- The smoke evidence renderer now falls back to the loaded pipeline Step definition when a failed runtime summary omits parameters, ensuring a configured ROI remains visible on failure without changing algorithm execution or acceptance.
- Evidence is `artifacts\p185_device_top_right_gap_only_20260722_r6`. Status is **Incomplete** because successful semantic edge selection across the representative split failed. Do not run all 500 and do not restore Matching. The next candidate must remain a bounded direct-Gap mode inside the existing `LineDistance` family: operator coarse ROI plus expected separation/orientation/support/uniqueness gates and candidate/selected-line drawings.

## P187 Device Top-Right Direct Dark-Band Gap Edge Pair (2026-07-22)

- Added an opt-in `USE_GAP_EDGE_PAIR=true` path to the existing `LineDistance` family. The legacy pair-projection path is unchanged when the flag is absent. The new path requires one reviewed coarse ROI, collects near-horizontal Canny/Hough candidates, merges collinear fragments, and selects one pair through separation, angle delta, shared support, local dark coverage, band darkness, and distinct-candidate margin gates. It uses no Matching, locator, template, NormalizeImage, hidden ROI movement, or per-image coordinate.
- PropertyGrid round trip, XML parameter typing/validation, known metrics, LLM tool catalog, and authoring guide expose the bounded mode together. The current P187 XML uses ROI `100,80,530,230`, pixel-only `12..60` candidate separation, maximum line angle `8 deg`, parallel delta `4 deg`, minimum shared support `0.26`, local dark contrast `8`, dark coverage `0.25`, and distinct score margin `0.05`. These are observed-set starter values, not general defaults or OK/NG tolerances.
- P187's executions completed mechanically, but the user later invalidated the canonical drawing: the magenta line joined a farther lower structure and the reported `25 px` was not the intended black-band thickness. Treat `artifacts\p187_gap_edge_pair_20260722` as historical failed semantic evidence, not current completion evidence. P189 below replaces it.

## P188 Dark-Band Gap Inspection-Intent Skill Contract (2026-07-22)

- Added the separate Guided Setup intent `Dark band thickness / Gap (LineDistance)`. It accepts exactly one operator-reviewed coarse ROI and generates exactly one px-only, measurement-only `LineDistance` Step with the frozen P187 `USE_GAP_EDGE_PAIR=true` parameters. It does not add Matching, locator, normalization, template teaching, Blob, Contour, acceptance, or calibration.
- The LLM prompt carries the same tool/parameter boundary and requires candidate/selected-edge drawings plus distance, stage-count, support, dark-coverage, and ambiguity metrics. Strict validation accepts the generated starter and rejects a changed ROI, Matching substitution, or an unapproved acceptance gate. The drawing requirement remains `WAIT` until the user explicitly runs the recipe.
- PropertyGrid labels now describe expected physical thickness, maximum edge tilt, shared support, local dark evidence, and distinct-pair score margin rather than presenting these settings as generic line values.
- The persisted generated starter round-trips through the runtime file loader. P188 completes the Phase 1 XML/UI contract, but its original `25 px` canonical runtime claim is superseded by P189.
- The reusable contract is `docs\OPENVISIONLAB_DARK_BAND_GAP_INTENT_SKILL.md`. Current-build UI, generated prompt/XML, positive validation, and three negative validation reports are retained under `artifacts\p188_dark_band_gap_skill_20260722`.

## P189 Nearest Same-Band Lower-Edge Correction (2026-07-22)

- The user correctly rejected P188's magenta lower line. Pixel review showed that the intended first lower transition was near `y=278` at `x=250`, while the fitted magenta line was near `y=291`; the former implementation could reward a farther continuous edge because the wider region was still dark on average.
- `USE_GAP_EDGE_PAIR` now keeps the supported upper Hough candidate but derives the lower boundary from the nearest sustained bright transition after that upper edge's immediately following dark core. It robustly fits only those traced points and fails closed when their count or horizontal span is below the existing support gate. No new XML parameter, Matching, hidden ROI movement, acceptance, or calibration was added; legacy LineDistance behavior remains unchanged when the flag is absent.
- On the unchanged P188 XML, the canonical selected lower line moved from the wrong `x=100..518`, `y=294.7..279.3` structure to `x=219..629`, `y=278.5..266.5`, following the actual black-band bottom. The measurement changed from invalid `DistancePxAvg=25`, range `4`, to `14.4`, range `2`.
- The exact P186 ten-row split completed `10/10` with pixel averages `19.4..49.4`; the current contact sheet was opened and the canonical plus meaningful extreme/boundary rows were inspected full size. A white no-band diagnostic failed closed with `GapCandidatePairCount=0` and `LineGaugeEdgeNotFound`.
- Evidence is `artifacts\p189_gap_lower_edge_correction_20260722`, including before/after canonical drawings, unchanged XML/hash, all ten source copies/hashes/logs/drawings, batch and runtime summaries, contact sheet, no-band failure drawing/log, and completion record. This is corrected small-split measurement evidence only; it does not prove all 500, label accuracy, OK/NG tolerance, mm calibration, other directions, unseen robustness, or field qualification.

## P190 Full-Corpus Scalable Dark-Band Gap Audit (2026-07-22)

- The unchanged P189/P188 XML was executed on all 500 unique `device_top_right` images without per-image tuning. Baseline accounting is 448 measurements, 52 named fail-closed outcomes, and zero missing inputs. Every row retains a same-run source snapshot, source/result SHA-256, metrics, and runtime drawing.
- The deterministic review queue contained all 52 failures plus measurement/stage extremes and 15 hash-selected rows from each `OK`/`NG` folder stratum: 128 rows, queue SHA-256 `F5E5A0A113495023EBBF26CFDE320686A25D2C62F13C491884D5397CD830B71F`. All 12 contact sheets were opened. Folder names were not treated as Gap truth.
- Visual review rejected the mechanical-success count as semantic success. Repeated rows selected lower secondary structures when the intended upper long band was clipped, weak, or outside the useful ROI region.
- One bounded correction changed only minimum shared support from `0.26` to `0.60`. Its full replay produced 329 measurements and 171 fail-closed outcomes; the 239-row queue and all 21 sheets were reviewed. It safely rejected some short structures but still wrong-passed long lower structures, so the candidate is rejected and numeric tuning stops.
- Decision: `Keep with documented limits`. The direct raw-ROI skill is supported only when one reviewed coarse ROI contains exactly one complete intended long dark band and no competing long band. It is rejected as a general solution for this variable corpus. Evidence: `artifacts\p190_dark_band_gap_full_corpus_20260722`.

## P191 Deterministic Run History Review Queue (2026-07-22)

- New saved batch summaries now persist the exact v1 queue policy, canonical queue SHA-256, selected result indices, and operator-readable reasons. The queue is frozen at save time rather than rebuilt differently whenever Run History is opened.
- The generic v1 policy includes all runtime failures, labelled false accepts/false rejects, missing or unreadable source/report/drawing evidence, minimum and maximum rows for every varying finite Step metric, and three content-hash-ordered audit rows per declared role stratum. Invariant metrics do not produce artificial extrema.
- The existing Run History surface now has a mutually exclusive `검토 큐만` filter. A selected queued row uses the existing retained-drawing action; filtering and selection do not run Preview/Run, create layers, or change input/output routing. Historical summaries without a saved queue are shown as unavailable and must be rerun instead of silently recomputed.
- Current-source smoke persisted and reopened an exact policy/hash, retained a labelled false accept, resolved and loaded its saved drawing, included evidence-gap/runtime-failure probes, verified metric min/max behavior, and confirmed that a 500-row two-stratum bounded probe selects six hash-audit rows when no other risk category varies. Evidence is `artifacts\p191_run_history_review_queue_20260722`.
- This completes the reusable queue/navigation workflow in Dev. It does not benchmark 10,000-image execution, prove that a particular queue is sufficient for every safety risk, or convert unlabelled metric/drawing evidence into semantic accuracy.

## P192/P193 Approved Hybrid Locator And Relative-ROI Gap Candidate (2026-07-22)

- The user explicitly approved the bounded hybrid product direction: deterministic Matching detects the reference pose, NormalizeImage maps the current image back to the reviewed reference coordinates, and a locked rule-based tool inspects a fixed reference-coordinate ROI. The LLM may author this constrained XML but is not the production detector and may not move the ROI per image.
- P192 reused only existing `Matching -> RotateScale/NormalizeImage -> LineDistance` families. The frozen candidate first audits two-candidate `ScoreMargin >= 10`, then publishes a bounded pose with score/angle/scale gates, requires normalized valid-pixel ratio `>= 0.15`, and measures the dark-band edge pair in reference ROI `330,210,260,100`. On the exact ten-row split, four rows measured the intended upper band and six failed closed at ambiguity. All drawings were reviewed.
- P193 replayed the unchanged XML on the exact frozen 500-row `device_top_right` list. Accounting is 356 measurements, 144 named fail-closed outcomes, and zero missing images. Terminal failures are 139 at the ambiguity audit, four at pose publication, and one at Gap edge selection.
- The deterministic review queue contains all 144 failures plus 106 measured metric/stage extreme and hash-audit rows. All 42 contact sheets (`18` measured and `24` fail-closed) were opened. The reviewed measurements did not repeat P190's confirmed lower-secondary-structure wrong-pass group; the failed rows visibly showed ambiguous, weak, or absent locator evidence and did not proceed to measurement.
- Measurement evidence remains bounded: `DistancePxAvg=12.8..18.4`, `DistancePxRange=0..3`, selected support `0.269230769231..0.996153846154`, and dark coverage `0.969696969697..1`. Some successes use short support. Folder `OK`/`NG` roles are review strata, not Gap tolerance truth.
- Final verification passed after the evidence-tool and documentation changes: `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` and the focused `VisionRecipeRunnerSmoke` build completed with zero warnings/errors; readiness, external-reference, public-sample, Python syntax, and `git diff --check` checks passed. The line-ending notices from `git diff --check` are conversion warnings, not whitespace errors.
- Decision: `Hybrid candidate`. Keep the architecture because it removes the observed raw-coordinate failure mode in the reviewed queue, but do not call the present small center-joint locator product-ready or weaken its gates merely to increase the 71.2% measurement coverage. Contract: `docs\OPENVISIONLAB_HYBRID_LOCATOR_RELATIVE_ROI_INTENT_SKILL.md`. Evidence: `artifacts\p192_top_right_hybrid_gap_20260722`, `artifacts\p193_top_right_hybrid_gap_full_corpus_20260722`.

## P195 Hybrid Relative-ROI Guided Setup/LLM Skill Phase 1 (2026-07-22)

- Added a separate `Locator-aligned Gap (NormalizeImage)` Guided Setup intent without changing the direct raw-ROI dark-band skill or adding an algorithm family.
- The UI collects the cropped locator template, search ROI, reviewed reference pose and image dimensions, reference-coordinate measurement ROI, score/margin/angle/scale/valid-pixel gates, and the pixel-only/no-judgement boundary.
- Starter generation emits the locked four-Step `Matching audit -> Matching fixture publisher -> RotateScale NormalizeImage -> DarkBandGap LineDistance` sequence. The LLM prompt and strict validator share the same contract.
- Current-source smoke accepted the generated starter, round-tripped it through Pipeline loading, preserved explicit Preview/Run and layer/routing counts, and rejected a changed normalization tool, changed measurement ROI, and weakened ScoreMargin. The readiness state is `LOCATION GATED / MEASURE READY / NOT JUDGED`; runtime drawings remain `WAIT` until explicit Run.
- Updated stale PropertyGrid and authoring/spec text that previously implied all fixture consumers were translation-only. V1 ROI translation remains supported separately; V2 NormalizeImage is the bounded angle/scale path.
- Evidence: `artifacts\p195_hybrid_relative_roi_phase1_20260722`. P195 completes Phase 1 only. It adds no new semantic runtime claim beyond P192/P193 and does not make the current locator product-ready.

## P196 Rule-Based-First Direction And LLM Maintenance Mode (2026-07-22)

- The user approved a product-direction change after reviewing the cumulative LLM work: planned LLM expansion is frozen, while the deterministic rule-based workbench becomes the active development track.
- This is a maintenance-mode decision, not deletion. Preserve the current LLM Assistant, Guided Setup/XML generation, guide/catalog, strict validation/import, P168 Pin evidence, and P195 hybrid authoring workflow. Fix concrete regressions or unsafe compatibility defects only.
- Do not add GPT/Gemini/Claude providers, consumer-web automation, API dependencies, prompt families, intent skills, or repeated transcript/correction campaigns unless the user explicitly reopens the track.
- The missing natural Pin Phase 3 failure, frozen P169 Test, and the ambiguous P193 locator remain documented evidence boundaries but are no longer active next-priority blockers.
- Product identity is now rule-based first: direct PropertyGrid teaching, Pipeline composition, explicit Preview/Run, layer and drawing review, frozen N-sample execution, and reusable recipes must work without an LLM account or generated XML.
- Commercial lessons to emulate remain deterministic teaching, fixture coordinates, Caliper/EdgePair-style metrology, segmentation, drawings, recipe management, and repeatable validation. Camera, lighting, PLC/I/O, MES, account, deployment, and industrial-controller scope remain excluded.
- Reopen planned LLM development only by explicit user decision after the equivalent non-LLM workflow is usable, the selected rule-based tool family has stable drawings/metrics and frozen N-sample evidence, and its XML contract is stable.

## P197 Non-LLM Matching Normalization And Reference-ROI Workflow (2026-07-22)

- Audited the existing operator path before adding code. Recipe Manager/Pipeline Review already expose the Matching fixture fields and `RotateScale` `NormalizeImage` fields through PropertyGrid; explicit reviewed-pose save already persists reference center/angle/scale/image dimensions while preserving downstream ROI/routes and requiring another explicit Review.
- Closed the actual product gap with one public rule-based example instead of a new algorithm or wizard: `Public_Matching_NormalizeImage_RelativeRoi.pipeline.xml` freezes `Matching -> RotateScale NormalizeImage -> Threshold -> Blob` and keeps the inspection `CvROI=320,180,60,50` in reference coordinates.
- The tracked Good sample publishes center `(200,155)` / offset `(80,55)`, normalizes to `572x420` at valid-pixel ratio `0.748`, and returns one accepted Blob. The paired missing-pad sample passes localization/normalization/preprocessing and fails at the fixed ROI with `ResultCount=0`; the catalog treats that result as the controlled expected failure.
- The current-source focused Pipeline Review smoke passes with four explicit Steps and current-run Good/Bad overlays. Public policy now reports `CatalogRows=32`, `ManifestAssets=229`, and `Pipelines=16`. Evidence and frozen hashes: `artifacts\p197_rule_based_fixture_workflow_20260722`.
- Final verification passed: full Debug solution build with zero warnings/errors; rebuilt screenshot and recipe-runner smoke projects; focused `wpf_shell_host_workspace_sample_normalize_fixture_review` at `1180x890` with zero layout/text/internal findings; readiness, external-reference, public-sample, and four-Step/two-row XML/catalog structure checks; and `git diff --check` with line-ending notices only.
- Status: Complete for one synthetic translated Good/Bad pair. It does not newly prove rotation/scale search robustness, other downstream tools, unseen images, industrial truth, calibration, or field qualification.

### P198 LineDistance / Gap Caliper Audit And Distinct-ROI Drawing Correction

- Audited the existing metrology family before adding another tool. General `LineDistance` already supports independent Line A/B ROI, polarity, scan direction/angle, fitted-edge distance, pixel/mm outputs, and distance distribution. The opt-in `GapEdgePair` path remains a specialized selector for one reviewed long near-horizontal dark band; do not present it as a general polarity/orientation Caliper.
- Reproduced one concrete evidence defect with public `Line_Pins_Synthetic_OK.png`: distinct Line A/B ROIs were both used by measurement, but only Line A's ROI was retained in runtime overlays. The runtime now keeps the existing single `Measurement ROI` when A/B are equal and emits separately labelled `Line A ROI` / `Line B ROI` overlays when they differ.
- The drawing-only change preserved the frozen OK result at `ResultCount=22`, `DistancePxAvg=37.014`, and `DistancePxRange=1.999`. The same XML measured the Wide-Pin comparison at `DistancePxAvg=18.300` and `DistancePxRange=3.994`, with both ROI drawings; because the audit XML is measurement-only, this mechanical success is not an OK classification.
- Existing shared-ROI public LineDistance replay retained one compact `Measurement ROI`. The P189 dark-band canonical remained `DistancePxAvg=14.4` and `DistancePxRange=2` with its candidate/selected-edge evidence. Current-source Line Tool smokes passed at `1600x900` and `1180x890` with zero layout/text/internal findings.
- Final verification passed: full Debug solution build with zero warnings/errors; exact distinct-ROI OK and Wide-Pin replays; shared-ROI public LineDistance and specialized P189 Gap regressions; current-source Line Tool screenshot smokes; readiness, external-reference, public-sample (`CatalogRows=32`, `ManifestAssets=229`, `Pipelines=16`), and `git diff --check` with line-ending notices only.
- Status: Complete for the audit, distinct-ROI drawing correction, and named focused regressions. It does not prove arbitrary industrial edge selection, physical calibration, all orientations/polarities, or broad-corpus semantic correctness. Evidence: `artifacts\p198_line_caliper_audit_20260722`.

### P199 Line Pair PropertyGrid A/B Round-Trip Fidelity

- A deliberately asymmetric `LineDistance` Step reproduced an actual no-edit Recipe Manager apply defect. Right ROI `485,170,70,145`, `BTOW` polarity, `Y_BTOT` vertical projection, manual-angle disabled, and angle `-7` were overwritten with the corresponding Line A values.
- Corrected only `PipelineLinePairProperty`: Line A/B ROI, projection direction, polarity, vertical projection, and manual-angle settings are independently labelled and serialized. Loaded per-line baselines preserve parameters not represented by the compact editor; changing a compact shared field such as Contrast still applies it to both lines.
- Direct round trip preserved 20 frozen asymmetric parameters. A direct Line B edit changed Right ROI/angle without changing Left ROI/angle, while a shared Contrast edit produced `21` on both. The actual Recipe Manager `XML apply` command saved and reloaded the asymmetric Pipeline without loss and with `NativePreviewRunCount=0`.
- Current-source UI visibly shows separate `Line A ROI=430,170,55,145` and `Line B ROI=485,170,70,145` plus `Round-trip validation passed`. ReferenceDifference, Fixture, Line measure, and Line pins measure regression smokes passed with zero layout/text/internal findings. The P198 runtime replay remained `ResultCount=22`, `DistancePxAvg=37.014`, `DistancePxRange=1.999`, and both ROI overlays.
- Final verification passed: full Debug solution and screenshot-smoke builds with zero warnings/errors, focused and related UI smokes, exact LineDistance runtime replay, readiness, external references, public samples (`32` rows / `229` assets / `16` pipelines), and `git diff --check` with line-ending notices only.
- Status: Complete for the reproduced A/B edit/persistence defect. It does not prove semantic edge selection across orientations/polarities, calibration, or broad-corpus robustness. Evidence: `artifacts\p199_line_pair_property_roundtrip_20260722`.

### P200 LineDistance Caliper Orientation/Polarity Matrix

- Froze one project-authored synthetic physical intent as four exact variants: rightmost pin to right datum rail clearance and its 90-degree clockwise equivalent, each with bright and dark object polarity.
- Final horizontal configurations use `X_LTOR/X_RTOL`; final vertical configurations use transformed A/B ROIs plus `Y_TTOB/Y_BTOT`. All four final cases returned `ResultCount=22`, `DistancePxAvg=37.000..37.014`, and `DistancePxRange=1.999..2.000`.
- Exact current-run drawings were opened and confirm separate A/B ROIs, both selected edge sets, and 22 final lines between the same transformed pin/datum boundaries. Bright/dark variants retain identical per-orientation metrics.
- Initial missing `USE_THRESHOLD=false` and vertical X-direction configurations failed closed with `703:LineGaugeEdgeNotFound`; correcting the recipe contract resolved them. No repeated runtime defect was found and no product source was changed.
- Status: Complete for the four-case synthetic matrix. It does not prove adjacent pin gap, center pitch, calibration, unseen variation, or field robustness. Evidence: `artifacts\p200_line_caliper_matrix_20260722`.

### P201 PinArrayGap Center-Pitch Semantic Extension

- The audit proved that the existing runtime measured only adjacent empty edge clearance, the strict v1 Guided Setup rejected center pitch, and Recipe Manager selected-Step PropertyGrid could not load a `PinArrayGap` editor.
- Added optional `MeasurementMode=CenterPitch` to the existing family. Missing mode remains legacy `EdgeGap`; center pitch uses adjacent dark-pin run centers and publishes distinct `PitchCount` / `PitchPxMin` / `PitchPxMax` / `PitchPxAvg` / `PitchPxRange` metrics. It does not publish `DistancePx*` or mm pitch metrics.
- Current-run drawings retain the row ROI and detected pin rectangles, add visible center points, and label every center-to-center line `P1..Pn`. EdgeGap keeps its `G1..Gn` drawings and existing metric contract.
- Recipe Manager now exposes Measurement, row ROI, dark detection, and acceptance fields for `PinArrayGap`. Direct mapping and actual Apply-to-XML/save/reload preserve the mode, ROI, represented fields, and an unrepresented `ALLOW_BRANCH_INPUT`; neither load nor apply triggers Preview/Run.
- Frozen semantic matrix results: uniform 20px pins at 60px centers -> `PitchPxAvg=60`, `PitchPxRange=0`; varied pin widths at the same centers -> the same pitch metrics while EdgeGap range becomes non-zero; one shifted center -> `PitchPxRange=12` and expected rejection by `PitchPxRange <= 2`. Missing `MeasurementMode` and explicit `EdgeGap` drawings are SHA-256 identical.
- Fresh current-source UI evidence is `artifacts\p201_pin_center_pitch_20260722\ui\before\wpf_shell_host_recipe_pinarraygap_properties.png` versus `artifacts\p201_pin_center_pitch_20260722\ui\precheck\wpf_shell_host_recipe_pinarraygap_properties.png`. The first shows `Unsupported step tool: PinArrayGap`; the final image shows `Measurement mode = CenterPitch` and the successful round-trip status.
- Final verification passed: full Debug solution and focused x64 runner builds with zero warnings/errors; frozen EdgeGap Guided Setup intent smoke; the six-run semantic/legacy matrix plus three-row Pitch batch CSV; focused UI precheck with zero warning/layout/text/internal findings; catalog JSON contract; readiness; external references; public samples (`CatalogRows=32`, `ManifestAssets=229`, `Pipelines=16`); and `git diff --check` with line-ending notices only.
- Status: Complete for the pixel-only dark-pin, one-row synthetic contract and UI persistence path. It does not prove bright pins, calibration, real-corpus robustness, semantic OK/NG truth, or a non-tuned N-sample completion. The frozen LLM Pin Guided Setup v1 remains EdgeGap-only. Evidence: `artifacts\p201_pin_center_pitch_20260722`.

### P202 PinArrayGap CenterPitch N-Sample Validation

- Reused the reviewed P168 top/bottom row ROIs and the P170 target-bearing Working Train/Validation manifests for a direct deterministic CenterPitch replay. The input audit found 252/252 files present, unique, and SHA-256 matched; every image is 768x576 8-bit grayscale. The reserved P169 Test was not listed, opened, copied, or executed.
- Train measurement froze the image score as the maximum top/bottom `PitchPxRange`. The highest Good was 12 px and the lowest `pitch_error` was 13.5 px, so the judged XML was frozen at `PitchPxRange <= 12` before Validation.
- The unchanged two-row XML accepted Good 178/178 and rejected `pitch_error` 26/26 on Working Train; it accepted 36/36 and rejected 12/12 on Working Validation. There were zero missing files, load/runtime errors, or misclassifications.
- All 252 same-run result drawings exist and match their recorded hashes. The 44-row deterministic review queue includes varying Pitch metric extrema, three content-hash audits per split/class, and explicit Good-max/NG-min boundary rows. All 11 queue sheets were opened; detected rectangles, centers, and adjacent pitch lines remained on the intended dark pins in the reviewed ROIs.
- A concrete evidence-tool defect was corrected: `VisionRecipeRunnerSmoke --batch-evidence` had stored only the final Step result image, hiding the top row on successful multi-Step recipes. It now saves one combined image with every executed Step overlay. Production runtime and CenterPitch measurement were unchanged.
- Status: Complete; decision `Keep with documented limits`. Evidence: `artifacts\p202_pin_center_pitch_nsamples_20260722`. This is pixel-only, dark-pin-only evidence on one synthetic/augmented and previously observed corpus. It is not independent blind Test, calibration, real production variation, or field robustness.

### P203 CenterPitch Saved-Validation And Run-History Workflow

- Staged exactly the P170 Working Validation rows into a task-local OK/NG folder layout after verifying every source SHA-256: 48 unique inputs, 36 OK and 12 NG. The reserved P169 Test was not listed, opened, copied, or executed.
- The existing Recipe Manager local Validation Set path executed the frozen P202 two-row XML and saved all 48 results. Judgement remained 36 correct accepts and 12 correct rejects, with zero false accepts, false rejects, load errors, or runtime errors.
- The product UI exposed both `PitchPxRange <= 12` gates. Stored evidence retained both PinArrayGap row drawings; one selected NG row reported `PitchPxMin=45.5`, `PitchPxMax=65`, `PitchPxAvg=56.792`, `PitchPxRange=19.5`, and `PitchCount=12`, and rejected `19.5 > 12` without rerunning Preview/Run.
- Saved Run History generated a 24/48 deterministic review queue. Policy and SHA-256 identity persisted as `196A8EF87728A867F4542F0A09D0AEEFB9C803E6041544893EB089770589E21F`; queue reasons include varying Pitch metric extrema and content-hash audits. Queue-only filtering did not change Preview/Run count, layers, or routes.
- No product integration loss was reproduced. Product runtime/UI code therefore did not change; only a screenshot-smoke target was added to retain the queue-view assertions and current-source image.
- Status: Complete for saved-validation/Run History integration. Evidence: `artifacts\p203_center_pitch_product_workflow_20260722`. This does not extend the P202 semantic boundary or prove independent blind Test, bright polarity, calibration, real production variation, or field robustness.

### P204 Missing-Pin / Row-Count Data And Tool Audit

- Selected `D:\라벨테스트\Pins_500_OK_NG\Pins` because it directly names the approved pin-missing intent and supplies images, masks, YOLO boxes, and split lists. The 250 NG rows contain five balanced classes; global class 30 identifies exactly 50 `Pins:missing_pin` rows, so the other 200 NG rows are not inferred as missing-pin truth.
- The frozen audit scope is 250 OK plus 50 class-30 missing-pin images. All 300 image hashes are unique and all images are 768x576 8-bit grayscale. Every OK label is empty and mask is zero; every missing-pin row has exactly one class-30 box and one non-empty mask, with the supplied box covering the mask bounds in all 50 rows.
- Provided split accounting is Train 178 OK/38 missing, Validation 36/12, and Test 36/0. The supplied Test is not target-bearing and cannot qualify missing-pin rejection.
- Tool decision: select the existing `Threshold -> Blob` path for the first semantic matrix. Its one reviewed ROI, binary threshold, area interval, `ResultCount`, and count gate match a bright connected-pin row without a new algorithm. Current `PinArrayGap` is a dark-run edge-gap/center-pitch tool and is rejected for this intent; Contour is retained only as a fallback if Blob filtering or drawings prove insufficient.
- Status: Complete for data/tool selection only. Evidence: `artifacts\p204_missing_pin_count_audit_20260722`. This does not freeze a ROI, threshold, area interval, count gate, Train/Validation candidate, or independent Test result.

### P205 Missing-Pin Fixed-Raw-ROI Blob Semantic Matrix

- Drew and reviewed raw ROI `0,40,768,360` on three hash-selected OK and three class-30 missing-pin images. It contains the upper bright pin row and supplied missing locations but also intersects competing slanted bright rails.
- Actual product measurement at Binary threshold 150 and Blob area `200..5000` returned 11 for all three OK rows, 10 for two missing rows, and 12 for `Pins_NG_0001`; current-run drawings proved two lower-rail fragments entered that row's Blob result.
- One bounded correction changed area to `1700..3000` and expected OK count to exactly 11. Numerical outcomes became OK 3/3 accepted and missing 3/3 rejected at count 10, but drawing review invalidated `Pins_NG_0001`: one horizontal rail fragment remained counted while the right border pin was missed. The correct-looking count was coincidental.
- Stop numeric tuning and reject the fixed raw rectangular ROI candidate. The defect is geometric separation, not another threshold/area value. Contour shares the same raw-ROI geometry problem and is not justified as a substitute.
- Corrected one evidence-harness defect: batch evidence now clones the source before execution, so the combined ROI/component drawing uses the original image rather than a Threshold-mutated binary Mat. Product Blob runtime was unchanged.
- Status: Incomplete because the required all-physical-pin drawing criterion failed. Evidence: `artifacts\p205_missing_pin_blob_semantic_matrix_20260722`. Do not run Train/Validation from this candidate.

### P206 Fixed-Rectification Missing-Pin Blob Semantic Matrix

- Reused only existing `RotateScale -> Threshold -> Blob` on the exact P205 six rows. An actual product sign probe showed `+10 deg` straightens the shared slanted pin row, while `-10 deg` increases the slant.
- The first aligned ROI still touched bright rails. Geometry correction 1 raised its bottom to `y=298`, removing all rail Blobs. One OK row then exposed an unstable source-border-truncated pin, so geometry correction 2 froze the common fully visible interior ROI `40,140,660,158` rather than lowering the area gate.
- Frozen candidate: `+10 deg`, 100% scale, Binary threshold 150, Blob area `200..5000`, aligned interior ROI `40,140,660,158`, and exact `ResultCount=9` acceptance. It intentionally judges nine stable interior slots and excludes truncated source-border pins.
- Final raw-source three-Step replay accepted all three OK rows at count 9 and rejected all three missing rows at count 8. Every aligned-stage current-run drawing was opened at original resolution and contains physical pins only; no rail fragment, noise component, or truncated boundary pin is counted.
- Status: Complete for the bounded six-row interior-slot candidate. Source/aligned/candidate hashes and all drawings are retained under `artifacts\p206_missing_pin_rectified_blob_matrix_20260722`. This is not pose-extreme, full Train/Validation, independent Test, unseen-data, lighting, or field qualification.

### P207 Frozen Missing-Pin Pose/Border Extreme Matrix

- Froze an eight-row matrix before candidate execution: per role, one unique minimum/maximum pin-base fit-angle row and minimum aligned left/right-margin row. All P205/P206 hashes were excluded; source hashes were verified; target NG rows were eligible only when the supplied missing-label center mapped inside the P206 aligned ROI.
- Replayed the byte-identical P206 candidate SHA-256 `A74AE17F44F2076F7277DBF92106DE2BE869D6E1456FD4908FCB6DF982204BE8` from the current build. Six rows were semantically correct.
- `Pins_OK_0243` admitted a right boundary pin and false-rejected at count 10. `Pins_NG_0111` admitted the same boundary pin, offset its true interior missing pin, and false-accepted at count 9. All eight aligned-coordinate drawings were opened at original resolution and the two failures are marked red in the final sheet.
- Decision: `Reject` the fixed `+10 deg` plus fixed aligned-ROI candidate. Its two correction cycles were already consumed; do not tune it again or run remaining Train/Validation.
- Status: Complete for the extreme-matrix audit and Reject decision. Evidence: `artifacts\p207_missing_pin_pose_border_extremes_20260722`. The next bounded option is a reviewed stable-locator proposal before any existing `Matching -> NormalizeImage -> Threshold -> Blob` audit, not another fixed-ROI adjustment.

### P208 Missing-Pin Locator Proposal Review

- Marked three non-runtime physical locator proposals on the exact P207 eight-row matrix and opened both the full overlay sheet and focused candidate crops. No Matching, Preview, Run, recipe edit, runtime change, or LLM work occurred.
- Candidate A, the long carrier rail, is useful only as secondary angle/Y support because it is horizontally repetitive. Candidate C, the carrier seam/corner, is clipped at the image borders and absent in two rows, so it is rejected as a sole locator.
- Candidate B, the central asymmetric curved machining mark on the lower rail, is outside the judged pins and visually appears in all eight crops. It may provide X/Y/angle, but image evidence cannot establish whether it is the same durable physical feature rather than glare, dirt, or changing surface texture.
- Status: Complete for the proposal task, with runtime work blocked on explicit operator confirmation of Candidate B. Evidence: `artifacts\p208_missing_pin_locator_proposals_20260722`.

### P209 Missing-Pin Hybrid Locator Audit

- Used the approved P208 Candidate B template with existing `Matching audit -> Matching fixture publisher -> NormalizeImage -> Threshold -> Blob` on the exact P207 eight source hashes. Product runtime and LLM surfaces were unchanged.
- Full-image Matching failed the frozen `ScoreMargin >= 10` ambiguity gate on all eight (`1.51..4.82` points); drawings showed a second similar lower-rail position.
- One bounded correction applied only the pre-approved Candidate B box union as coarse search ROI `220,350,260,220`. It cleared the ambiguity gate but left two OK false rejects at unchanged `SCORE_MIN=0.8` and one missing-pin false accept.
- `Pins_NG_0186` counted eight physical pins plus one lower horizontal rail fragment as 9, so the missing pin was numerically cancelled. Operational classification was 5/8. All full-image and coarse-ROI drawings were opened at original resolution.
- Decision: `Reject` and stop this missing-pin/count intent for the current framing/composition. Do not lower gates, retune Blob, or run Train/Validation. Evidence: `artifacts\p209_missing_pin_hybrid_locator_audit_20260722`.

### P210 Repeated-Validation Closure And Rule-Based UI Gap Audit

- The user closed repeated image inspection, dataset switching, parameter tuning, and LLM XML/provider validation as active priorities. Do not resume any of them until the user explicitly requests a named validation task.
- A static audit inventoried the current 21 canonical tool families, metrics, PropertyGrid editors, ROI/template surfaces, Pipeline Review, saved evidence, and Run History without executing Preview, Run, a batch, or an LLM workflow.
- Official Cognex, MVTec MERLIC, KEYENCE, and Zebra material was compared only for workbench-relevant UI patterns. The selected gaps are: (1) per-object Blob/Contour result rows with accepted/rejected reasons and click-to-highlight drawings, (2) one visual Fixture/relative-ROI designer over the existing Matching/NormalizeImage path, and (3) a reusable circle/point/line geometric measurement workspace.
- OCR/barcode, deep learning, 3D, hardware integration, and another Pipeline graph engine are not current priorities. Detailed audit: `docs\OPENVISIONLAB_RULE_BASED_UI_GAP_AUDIT_20260723.md`.
- Status: Complete for static inventory, commercial comparison, priority selection, and the repeated-validation stop decision. No product code or UI changed.

### P211 Object Results Inspector

- Pipeline Review now presents Blob/Contour results as stable object rows: object number, accepted/rejected state, area, center X/Y, bounds W/H, angle, and the active area-filter reject reason.
- Selecting a table row highlights that exact object on the retained result drawing. Clicking an object in either result preview selects the matching row. These review actions reuse same-run coordinates and do not execute Preview/Run or change layers/routing.
- The Step summary, direct Pipeline report, and saved recipe-run report contracts retain the object rows. A focused report round trip preserved one accepted row, one rejected row, and `MIN_AREA` reject evidence.
- Blob preserves the complete area-audit candidate set produced by the shared tool preprocessing. Contour preserves accepted objects plus near-filter candidates at or above 25% of configured `MIN_AREA`; this deliberate floor prevents pixel-noise contour floods and is not an exhaustive list of every tiny rejected contour.
- Current-build focused UI smokes passed for the public Blob NG and Contour NG samples. Blob exposed `253 / accepted 3 / rejected 250`; Contour exposed `2 / accepted 2 / rejected 0`. Both table selection and image-hit selection preserved Preview count, layer count, active layer, and routing.
- Status: Complete for the bounded Object Results Inspector slice. Before/after UI and report evidence: `artifacts\p211_object_results_inspector_20260723`.

### P212 Fixture And Relative-ROI Designer

- Pipeline Review now detects one existing named `Matching fixture producer -> NormalizeImage consumer -> reachable downstream CvROI Step` chain and presents it in a dedicated `Fixture / 상대 ROI` tab. The relationship stays read-only; PropertyGrid remains the authoritative editor.
- The tab shows the template, search ROI, taught reference pose/image size, current Matching pose, score, same-template preflight margin when available, and normalized valid-pixel ratio. It does not infer a missing margin from an unrelated Matching Step.
- The saved downstream reference ROI is drawn as a transformed magenta polygon on the current source image and as a green reference-coordinate rectangle on the normalized image. Before an explicit Review, the source remains visible but transformed/normalized evidence stays in the run-required state.
- Explicit actions route to the existing reference teach, Fixture Matching Step editor, downstream ROI Step editor, and Run Review. No locator/normalization/measurement algorithm, automatic ROI, automatic run, or LLM surface was added.
- Current-build verification passed the full solution build, focused designer UI, legacy translation-Fixture reference teach, selected-Step editor handoff, and Fixture PropertyGrid round trip with zero warnings/errors. Selecting the designer preserved Preview/Run count, layers, active layer, and routing.
- Status: Complete for this bounded UI-consolidation slice. Before/after and command evidence: `artifacts\p212_fixture_relative_roi_designer_20260723`.

### P213 General Geometric Measurement Workspace

- The bounded workspace is complete. Successful existing `Line` Steps publish typed `Segment`, `Start`, `End`, and `Midpoint` rows; `CircleGauge` publishes `Circle` and `Center`; and a later `GeometryMeasure` Step resolves exact earlier `SourceStep + FeatureName` identities in the same explicit run.
- The seven frozen pixel-only modes are `PointPointDistance`, `PointLineDistance`, `SegmentSegmentDistance`, `LineLineDistance`, `LineLineAngle`, `LineLineIntersection`, and `CircleSegmentClearance`. Runtime/validator gates reject wrong kinds, failed or acceptance-NG sources, coordinate mismatch, non-finite/out-of-image values, degenerate segments, invalid ROI, non-parallel/parallel misuse, and excessive intersection extension.
- Recipe Manager PropertyGrid provides compatible earlier-feature Source A/B dropdowns. Applying and saving retains source identities without Preview/Run. Pipeline Review adds a read-only Geometry Review with table-to-drawing highlight and drawing-to-row selection. Direct and recipe Run Reports preserve typed rows and provenance.
- Full and focused builds passed with zero warnings/errors. Synthetic checks passed all seven positive modes plus bounded negative gates, Circle full/partial success and wrong-polarity/no-edge/support/residual rejects, report round trips, fresh PropertyGrid/Geometry Review captures, and legacy Line/LineDistance/LineIntersection regressions.
- Status: Complete. Evidence: `artifacts\p213_general_geometric_measurement_workspace_20260723` and `docs\OPENVISIONLAB_GENERAL_GEOMETRIC_MEASUREMENT_WORKSPACE_CONTRACT.md`.
- Boundary: this is pixel-only synthetic algorithm/UI evidence. It does not prove calibration, industrial semantic accuracy, unseen robustness, field qualification, automatic arbitrary feature selection, or `OuterCornerIntersection` physical-boundary correctness.

### P214 Two-Point Scale Teaching

- The bounded uniform-scale slice is complete. Pipeline Review now exposes a dedicated Scale Calibration tab that consumes two distinct same-run P213 Point rows, draws A/B and their connecting pixel distance, accepts a certified real distance in mm/µm/inch, and stores the exact point identities/coordinates, coordinate layer, dimensions, image SHA-256, unit conversion, and derived millimeters per pixel.
- Calculation saves evidence before recipe mutation. Apply is a second explicit action against one selected compatible Line/LineDistance/PinArrayGap/Gap/Curve/Circle/Geometry Step. It preserves the legacy `PIXELPERMM` XML key because the existing runtime depends on it, while UI and contract wording state its real runtime meaning: mm per pixel. LineDistance also receives its existing Left/Right scale values.
- Calculate and Apply do not invoke Preview/Run and do not change layers or routing. The actual Pipeline Review integration smoke proved that explicit Run supplied the points, while subsequent calculate/apply preserved execution state, layer count, and source image hash and persisted only the chosen Step.
- Fail-closed checks cover same identity/coincident points, cross-layer or dimension mismatch, missing/changed source image, invalid known distance, incompatible target, and target input-layer mismatch. Calibrated P213 `GeometryDistanceMm`, `GeometrySignedClearanceMm`, `CircleRadiusMm`, and `CircleDiameterMm` are added only when a positive legacy scale exists.
- Verification passed: current Debug solution and focused smoke builds with zero warnings/errors; 3-4-5 geometry and mm/µm/inch equivalence; record/pipeline round trips; changed-image, same-identity, cross-layer, and incompatible-target rejects; actual Pipeline Review Run/calculate/apply isolation; current-source visual capture; and P213 geometry/Line/LineDistance/LineIntersection regression smoke.
- Status: Complete. Evidence: `artifacts\p214_two_point_scale_teaching_20260723` and `docs\OPENVISIONLAB_TWO_POINT_SCALE_TEACHING_CONTRACT.md`.
- Boundary: this is one uniform image-plane scale and synthetic/UI integration evidence. It does not calibrate lens distortion, perspective, separate X/Y scales, camera/world/robot coordinates, uncertainty, traceability, or industrial metrology accuracy.

### P215 Post-P214 Commercial UI/Tool Gap Reassessment

- Completed a static source/catalog and official commercial-workflow reassessment after P211-P214. No image, Preview, Run, batch, recipe tuning, or LLM provider workflow was executed.
- The current machine-readable catalog contains 23 canonical tool families and 42 accepted ToolType names/aliases. P211 already persists Blob/Contour object rows with area, center, bounds, angle, accepted/rejected state, and drawing selection.
- The concrete remaining mismatch is narrower than the older audit: Blob/Contour PropertyGrid, Pipeline/XML mapping, factory, and per-object runtime acceptance still expose only `MIN_AREA`/`MAX_AREA`, even though object width and height are already calculated and visible. Aggregate Step acceptance such as `BoundsWidthMax` exists, but it cannot filter the accepted object set or its `ResultCount`.
- Selected exactly one bounded next slice: optional bounding-width and bounding-height minimum/maximum gates for Blob/Contour, with backward-compatible area-only behavior, exact P211 reject reasons, and saved-report round trip.
- Deliberately deferred full region-feature evaluation, angle/aspect/circularity/holes/gray gates, easyTouch-style automatic suggestions, tool-navigation reorganization, OCR/barcode, region algebra, graph-engine work, datasets, and LLM campaigns.
- Status: Complete for the audit and priority selection only. Evidence: `docs\OPENVISIONLAB_RULE_BASED_UI_GAP_AUDIT_20260723.md`.

### P216 Blob/Contour Object Dimension Filters v1

- Added optional axis-aligned pixel bounding-box gates `MIN_WIDTH`, `MAX_WIDTH`, `MIN_HEIGHT`, and `MAX_HEIGHT` to Blob and Contour PropertyGrid, Pipeline builder/mapper/factory, parameter schema, validation, deterministic runtime, summaries, object evidence, and XML/catalog guidance.
- Missing new XML keys restore `0..1000000`, so existing area-only recipes keep their accepted set. Reversed ranges fail strict validation; PropertyGrid range editors keep their pairs ordered.
- Runtime filtering now occurs before `ResultCount`, accepted-object metrics, bounds metrics, drawings, and Step acceptance. P211 still retains nearby rejected candidates and records exact reasons such as `Width 52 > MAX_WIDTH 30` or `Height 8 < MIN_HEIGHT 16`.
- The deterministic five-shape matrix passed for both Blob and Contour: one object remained accepted, four failed the named width/height gates, and the same pipelines without the four new keys retained all five. Actual Run History XML preserved the four reasons.
- Fresh current-build evidence shows the new PropertyGrid range, W/H summary, unchanged explicit Preview/Run state, accepted/rejected drawings, and existing Blob/Contour/Pipeline Review regression UI.
- Status: Complete. Evidence: `artifacts\p216_object_dimension_filters_20260723`.
- Boundary: axis-aligned pixel bounds only. This does not add rotated size, aspect ratio, circularity, holes, gray features, automatic suggestions, semantic classification, operator-dataset evidence, industrial robustness, or field qualification.

### P217 Post-P216 Deterministic Workflow Reassessment

- Completed a source/document-only reassessment of the current operator path. No image, Preview, Run, batch validation, recipe tuning, UI change, algorithm change, or LLM provider workflow was performed.
- Current source connects PropertyGrid teaching and the selected-Step edit handoff to explicit `Run Review`; Pipeline Review exposes the completed Object Results, Fixture/relative-ROI, Geometry Review, and Scale Calibration surfaces.
- Recipe storage has an explicit round-trip check; Run Reports preserve per-Step drawings and object evidence; saved batch summaries preserve the deterministic review queue; Run History resolves those saved artifacts without silently rerunning Preview or Run.
- P211-P216 complete the bounded commercial-workflow shortlist selected by P210. The remaining descriptor, suggestion, navigation, OCR/barcode, region-algebra, and new-algorithm candidates have no named operator task plus reproducible current blocker, so none was selected.
- Status: Complete. Decision: close proactive feature expansion. Evidence: `artifacts\p217_post_p216_workflow_reassessment_20260723` and `docs\OPENVISIONLAB_RULE_BASED_UI_GAP_AUDIT_20260723.md`.
- Boundary / next dependency: implementation may resume only from a concrete operator-blocking workflow or verified regression with current-source reproduction. Product limitations and field-qualification boundaries remain documented limits, not automatic feature backlog.

### P218 Affine Transform v1

- The user explicitly reopened one named deterministic tool gap: a three-point 2D Affine transform implemented in `C:\Git\Library-Noah`, built there, and consumed through the OpenVisionLab vendored DLL boundary.
- Library-Noah now owns finite-point/output/sampling/gate checks, non-collinear source/destination enforcement, `GetAffineTransform` plus `WarpAffine`, retained-source coverage, matrix/decomposition/triangle metrics, stable errors, and ten review drawings.
- OpenVisionLab adds the PropertyGrid `Affine Transform` Tool View, explicit Preview, result card, Pipeline/XML canonical name `AffineTransform`, aliases `Affine`/`AffineMatrix`, strict validator/mapper round trip, public sample, and Geometry Learn teaching. Irrelevant inherited Threshold/ROI/masking/pixel-mm rows remain hidden.
- Exact DLL provenance is fixed: Library-Noah source, Dev vendor, and current build output use assembly `2.1.0.0`, file `2.8.0.0`, SHA-256 `B128CA282C0CD02C36F5CCF0C78C69C6F4834C3376158E8667EEAA7DE494A08B`.
- The known public matrix `[0.9 0.1 20; 0.05 0.9 10]` replayed at determinant `0.805`, valid-pixel ratio `0.805`, output `572x420`, and ten drawings. Canonical/alias execution, PropertyGrid/XML round trip, collinear rejection with a zero configured area gate, and coverage-failure evidence retention passed.
- Library-Noah build and 57/57 smoke, OpenVisionLab zero-warning build, focused contract/sample/current-source UI smokes, readiness, public-sample checks, and RotateScale regression passed.
- Status: Complete for v1. Evidence: `artifacts\p218_affine_transform_v1_20260723` and `docs\OPENVISIONLAB_AFFINE_TRANSFORM_V1_CONTRACT.md`.
- Boundary: this is known-matrix synthetic algorithm/integration/UI evidence. It does not prove automatic correspondence, homography, camera/lens calibration, calibrated units, industrial accuracy, unseen robustness, or field qualification. LLM remains in maintenance mode.

### P219 Detected-Point Affine Fixture

- P218 fixed numeric source points remain the default. Optional `USE_DETECTED_SOURCE_POINTS=true` resolves three ordered earlier typed `Point` results into the existing Library-Noah source coordinates only for the current explicit Run.
- Successful single-result Matching now publishes `Center`; Line endpoints/midpoint, CircleGauge center, and GeometryMeasure Point outputs use the same picker contract. Sources must be earlier, enabled, accepted, distinct, finite, inside the image, and in the Affine input layer and dimensions.
- Recipe Manager selected-Step PropertyGrid exposes the enable toggle plus three constrained Point pickers. Apply/XML round trip does not invoke Preview/Run.
- The actual representative pipeline `Matching x3 -> AffineTransform -> Threshold -> fixed-ROI Blob` passed. The independently calculated matrix matched, the saved `CvROI=170,120,70,60` did not move, and the final result was exactly one normalized object. Matching, Affine destination/frame, and fixed-ROI Blob drawings are retained.
- Duplicate Point references are rejected by definition validation and fail closed if direct execution bypasses it. There is no fallback to the fixed source coordinates after detected-point mode is enabled.
- Status: Complete. Evidence: `artifacts\p219_dynamic_affine_fixture_20260723` and `docs\OPENVISIONLAB_AFFINE_DETECTED_POINT_FIXTURE_CONTRACT.md`.
- Boundary: this is deterministic same-run source wiring and synthetic integration evidence. It does not prove automatic correspondence, locator stability, homography, calibration, industrial accuracy, unseen robustness, or field qualification. LLM remains in maintenance mode.

### P220 Operator-Approved Card Affine Pilot

- The operator approved three distinct, non-collinear card features: the `R` glyph, `5` glyph, and lower expiry mark. Their template ROIs and destination centers were frozen from `card_original_OK_0001.jpg` before Matching execution.
- A preselected 12-row pilot used six evenly spaced OK and six evenly spaced NG rows. NG is an input stratum only; this pilot did not attempt defect classification.
- The first fixed run passed 8/12. Drawing review proved two `5` omissions came from a too-narrow coarse search ROI and one weak `R` case matched the left `P`.
- One geometry-only r2 correction excluded `P` from the `R` search region and widened the `5` search region. Score `0.55`, angle `-8..8°`, scale `0.9..1.1`, Affine gates, and the independent `<=3 px` normalized-center gate remained unchanged.
- r2 produced all three typed Points and Affine output on 12/12. Ten rows retained `0..2 px` maximum center residual. `OK_0051` remained `5.00 px` and `NG_0150` remained `4.12 px`.
- Status: Incomplete at the frozen `<=3 px` gate. Evidence: `artifacts\p220_affine_fixture_point_candidates_20260723` and `docs\OPENVISIONLAB_CARD_AFFINE_PILOT_20260723.md`.
- Boundary: do not lower the gate, run all 500, add Homography, or switch features until the downstream fixed ROI/inspection and allowable registration error are supplied.

### P221 Card Affine Fixed-ROI Linkage

- The operator accepted the current registration result for one coarse downstream ROI. P220 remains historically incomplete at `<=3 px`; P221 separately freezes the accepted observed envelope at `<=5 px`.
- The same P220 12 rows and unchanged Matching x3/Affine path now execute one existing `Mean` Step on `CardReference` with exact `CvROI=250,315,190,80`. XML round trip retains the input layer and ROI; the Step has no acceptance judgement.
- All 12 rows completed, published finite `MeanValueAvg=111.4..170.1`, met normalized-template score `>=0.65` and residual `<=5 px`, and retained exact current-run ROI drawings over the `10/05` date area.
- Status: Complete for this bounded fixed-coordinate linkage. Evidence: `artifacts\p221_card_affine_fixed_roi_20260723_r2` and `docs\OPENVISIONLAB_CARD_AFFINE_FIXED_ROI_20260723.md`.
- Boundary: this is not OK/NG classification, a Mean tolerance, unseen-data qualification, Homography, or production-locator proof.

### P222 Auto MPoint Library Core

- The operator explicitly requested one new Library-Noah teaching tool that finds
  fixed-size image regions suitable for matching instead of requiring manual
  region selection alone.
- `AutoMPointTool` is a training-time suggestion engine, not a Pipeline Step. It
  ranks contrast/edge-distribution candidates, overlap-suppresses finalists, then
  reuses `EdgeBasedTemplateMatchingTool` for self-location, strongest-alternative
  uniqueness, three known synthetic replays, precision, and measured runtime.
- Results retain the authored rectangle-center MPoint, the native edge-model center
  and offset, exact accepted/rejected reasons, metrics, result drawings, and
  overlays. No template, recipe, layer route, Preview, or Run is changed.
- Library-Noah Release build passed with zero warnings/errors and the full smoke
  suite passed 60/60. The unique pattern ranked first at `64,64,64,64`; identical
  patterns both failed `UniquenessMargin 0 < 0.1`; invalid ROI/pattern definitions
  failed closed; repeat ranking/drawing pixels were identical.
- The current source-library `Lib.OpenCV.dll` is assembly `2.1.0.0`, file
  `2.8.0.0`, SHA-256
  `3D7A0B5D392B096DB3C14091D08E52BBB840772C1BDD1B30BEB15475ABAE28D9`.
  P223 later copied and verified this exact build in OpenVisionLab Dev.
- Status: Complete for Library-Noah V1. Evidence:
  `C:\Git\Library-Noah\artifacts\auto_mpoint_v1_20260724` and
  `docs\OPENVISIONLAB_AUTO_MPOINT_V1_CONTRACT.md`.
- Boundary: operator confirmation on real images, automatic size selection, and
  field qualification remain pending. P223 completes the UI/DLL integration only.

### P223 Auto MPoint Teaching UI And Matcher Direction Review

- Integrated Library-Noah `AutoMPointTool` into the existing Edge Based Matching
  Tool View rather than adding a Pipeline Step.
- Added PropertyGrid settings, explicit `Analyze candidates`, a `Suggested`
  candidate list with ROI/uniqueness/pose-error/runtime evidence, the library
  drawing, and explicit `Use this pattern`.
- Property edits, analysis, row selection, and pattern apply preserve Preview/Run
  count, layers, active layer, and routing. Applying a candidate uses the existing
  template save path and still requires an explicit Matching Preview.
- Source, vendored, and current Debug `Lib.OpenCV.dll` remain assembly `2.1.0.0`,
  file `2.8.0.0`, SHA-256
  `3D7A0B5D392B096DB3C14091D08E52BBB840772C1BDD1B30BEB15475ABAE28D9`.
- The operator-provided GPT Pro research was checked against current source and
  official HALCON/Cognex documentation. Keep the current matcher. Its ambiguity
  diagnostics do not yet reject a runtime result, its subpixel adjustment is
  independent X/Y five-score interpolation, model reduction is sequential, and
  hybrid selection evidence is not exposed through `MatchingResult`.
- Direction: implement an opt-in unique-match gate first; then prove it on a
  frozen fixed ROI; only then compare a translation-only joint refinement.
  Adaptive window growth, ODB/CAD, global anchors, and Homography are deferred.
- Status: Complete for the bounded teaching integration and direction review.
  Evidence: `artifacts\p223_auto_mpoint_ui_20260724` and
  `docs\OPENVISIONLAB_AUTO_MPOINT_V1_CONTRACT.md`.
- Boundary: this is one-image suggestion/UI evidence, not runtime uniqueness,
  production repeatability, field robustness, or commercial-library parity.

### P224 Edge-Based Unique Match Runtime

- Added an opt-in Library-Noah unique-result gate to the existing
  `EdgeBasedTemplateMatchingTool`; no new matcher family was created.
- Missing keys preserve legacy behavior (`USE_UNIQUE_MATCH_VALIDATION=false`,
  `UNIQUE_MATCH_MIN_SCORE_MARGIN=0.03`). Enabled mode requires `NUM_MATCH=1`,
  `USE_MULTI_ROI=false`, and a finite normalized margin.
- Internal candidate retention is at least Top 8 even though the external result
  count is one. Runtime returns exactly one result only for `Success`; `NoMatch`
  and `Ambiguous` return no `MatchingResult` with separate error codes, state,
  score/margin/alternative metrics, and an exact reject reason.
- OpenVisionLab exposes the option and margin in the existing PropertyGrid and
  XML/Pipeline round trip, validates the one-result/one-search-region contract,
  and preserves the ambiguity reason in Pipeline diagnostics. Edits do not
  auto-run Preview.
- Library-Noah Release build passed and the complete smoke suite passed 64/64.
  The focused matrix covers legacy repeated-pattern success, unique distinct
  success, repeated-pattern `MatchingAmbiguous`, and absent-pattern
  `MatchingNoResult`.
- Source, vendored, and current Debug `Lib.OpenCV.dll` are assembly `2.1.0.0`,
  file `2.8.0.0`, SHA-256
  `000C75A7D0E796E166DF6F24C95F264FC001927881B1ED7DE7BAE31913099F6D`.
- Status: Complete for the bounded runtime/XML/UI contract. Evidence:
  `artifacts\p224_unique_match_runtime_20260724` and
  `docs\OPENVISIONLAB_EDGE_BASED_UNIQUE_MATCH_V1_CONTRACT.md`.
- Boundary: the synthetic matrix does not qualify a physical template, search
  ROI, margin, pose error, repeatability, false-accept rate, or field robustness.

### P225 Card R Fixed-ROI Edge Matching Matrix

- Reused the exact operator-approved P220/P221 card `R` anchor rather than
  selecting or tuning a new target. The reference/template, 12 source hashes,
  prior reviewed `R` centers, angle `-8..8°`, scale `0.9..1.1`, score `0.45`,
  unique margin `0.03`, and accepted prior-center error `<=5 px` were frozen
  before execution.
- The three modes were reviewed search ROI plus unique validation, original broad
  search ROI plus legacy behavior, and that same broad ROI plus unique
  validation. No setting changed after results were observed.
- Reviewed-ROI unique returned `0/12` correct accepts, two accepted centers more
  than 5 px from the frozen P220/P221 center, two ambiguity rejects, and eight
  no-match rejects. Broad legacy returned one correct accept and two wrong
  accepts; broad unique returned no correct accepts and retained the same two
  wrong accepts.
- Exact current-run drawings and separate baseline-comparison drawings were
  opened. One reviewed-ROI wrong accept selected the `T` glyph instead of `R`
  with score `74.237` and no plausible alternative. This is direct evidence that
  a large uniqueness margin does not establish semantic or physical-feature
  identity.
- Pipeline integration now retains the already exposed EdgeBased scale,
  subpixel, and pyramid settings in builder/factory paths and publishes one
  successful EdgeBased result as typed `Center`; the 36-row matrix asserts those
  settings and the center handoff.
- Status: Complete audit; candidate decision `Reject fixed candidate`. Evidence:
  `artifacts\p225_edge_unique_card_r_matrix_20260724`.
- Boundary: the expected centers are previously reviewed P220/P221 Matching
  centers, not independent metrology ground truth. Do not lower gates, retune
  the `R` template/ROI per image, run a larger card set, or start joint pose
  refinement. Refinement cannot repair eight no-matches or selection of the wrong
  glyph.

### P226 Public EasyMatch Auto MPoint Candidate Presentation

- Used the existing Library-Noah engine through a focused evidence command on
  five public reference images: `BOARD.JPG`, `Die Pad 1.bmp`, `Floppies.jpg`,
  `Frame 1.tif`, and `Switch1.tif`.
- Froze the current product defaults before execution: `96x96` candidate
  windows, `16 px` stride, eight exact finalists, at most five displayed
  suggestions, feature quality `>=0.15`, matching score `>=0.75`, uniqueness
  `>=0.05`, and synthetic position error `<=2.5 px`.
- Retained five current-run drawings, 40 evaluated finalist rows, exact source
  hashes, and 20 displayed candidate crops. Twenty-eight finalists passed the
  internal gates; the UI-equivalent rank cap exposed 20 suggestions on four of
  the five images.
- `Frame 1.tif` rejected all eight finalists because uniqueness margins
  `0.0011..0.0054` were below `0.05`. This is the intended fail-closed result for
  its repeated frame pattern.
- `Floppies.jpg` still suggested five disk-hub windows. The repeated hubs have
  different fixed orientations, so the one-image angle-disabled comparison can
  call them numerically distinct. Drawing review therefore confirms again that
  Auto MPoint score/uniqueness is not a durable physical-feature or semantic
  identity decision.
- No setting changed after results, no suggestion was applied, and no
  cross-image Matching, Affine, inspection, or OK/NG run occurred.
- Status: Complete candidate-presentation slice. Evidence:
  `artifacts\p226_auto_mpoint_easymatch_candidates_20260724_r2`.
- Boundary: the operator must still name one sample/rank/ROI and confirm that it
  is the same stable physical feature in representative images before a
  cross-image qualification matrix is allowed.

### P227 Six-Corpus Auto MPoint Pilot And Report

- Audited the six operator-provided EasyMatch packages under `D:\라벨테스트`
  as 3,000 unique `all_images` rows, 250 OK and 250 NG per package. The package
  formats contain duplicate copies for detection/segmentation/anomaly tasks, so
  this pilot intentionally used only `all_images`.
- Split the corpus into 16 independent `source_file` strata. Each stratum used
  its first OK as the frozen Auto MPoint teaching image and a deterministic
  MD5-spread pilot of four OK plus four NG rows. Different source images were
  never treated as one template family.
- The first product-path execution found a real contract defect rather than a
  candidate-quality result: 88/104 rows returned
  `MatchingTemplateInvalid` because Auto MPoint verified grayscale windows but
  a new `EdgeBasedMatchingProperty` inherited
  `USE_THRESHOLD=true / THRESHOLD=0`, which erased most template edges.
- New EdgeBasedMatching properties and explicit Auto MPoint application now use
  grayscale-edge matching by default. A recipe that explicitly stores
  `USE_THRESHOLD=true` still restores that value. PropertyGrid, explicit apply,
  Pipeline round trip, and the current UI smoke passed without automatic
  Preview/Run, layer, or route changes.
- The corrected frozen replay generated suggestions on 13/16 strata and ran 104
  pilot rows with zero runtime or integrity errors. Twelve strata met the
  minimal mechanical rule of at least three successful OK rows.
- Every one of the 13 matching contact sheets and the 16-stratum candidate
  sheet was opened. Drawing review stopped Frame 1/2/3 (no suggestion), Frame 4
  (OK 2/4), Die1 (repeated grid intersection), and Die2 (image-frame boundary).
  Ten strata remain `expansion candidates`; none is a qualified locator.
- The report contains the candidate overview, per-stratum machine table,
  advisory drawing decision/reason, all current-run contact sheets, raw result
  CSV, review CSV, and completion record:
  `artifacts\p227_auto_mpoint_six_corpus_pilot_20260724_r4\OPENVISIONLAB_AUTO_MPOINT_SIX_CORPUS_REPORT.md`.
- Status: Complete for the bounded pilot/report and threshold-contract repair.
  Boundary: generated pose ground-truth coordinates are absent, the images are
  synthetic/augmented rather than field captures, and no 500-row stratum replay
  was run. The operator must approve one named expansion candidate before that
  larger replay.

### P228 Self-Contained HTML Report Export

- Extended the existing P227 six-corpus command so its primary operator output
  is a self-contained HTML report rather than Markdown.
- The HTML embeds the 16-stratum candidate overview and all 13 available
  Matching contact sheets as data URIs. It includes the full decision table,
  Korean drawing-review decisions and reasons, fixed conditions, honest limits,
  and relative links to the companion CSV evidence.
- The report has no web dependency and includes an `인쇄 / PDF 저장` button
  backed by the browser's standard print dialog.
- A fresh replay retained 3,000 metadata rows, 16 strata, 13 suggestions, 104
  executions, zero runtime/integrity errors, ten expansion candidates, and six
  stopped strata. Logical result comparison with P227 r4 was zero-delta.
- Structural verification found 14 embedded images and zero missing companion
  links. The current HTML was rendered in Chrome at 1440×1200 and visually
  checked for Korean text, summary cards, warnings, controls, and the first
  evidence image.
- Primary operator document:
  `artifacts\p227_auto_mpoint_six_corpus_pilot_20260724_r5_html\OPENVISIONLAB_AUTO_MPOINT_SIX_CORPUS_REPORT.html`.
- Status: Complete for HTML export and current-result rendering. Boundary: this
  changes report presentation only; it does not qualify a locator or authorize
  the full 500-row replay.

### P229 Representative-Image Automatic Best Pattern

- Extended Library-Noah AutoMPoint with an optional representative-image overload;
  the legacy one-image API and behavior remain unchanged.
- Every one-image-accepted finalist is replayed with the existing edge matcher on
  all supplied same-size representative images. Candidates below the configured
  success rate are rejected; survivors are ranked by representative success rate,
  minimum uniqueness margin, mean score, then original score.
- The existing Edge Based Matching teaching panel now accepts multiple
  representative images, shows their count, marks rank one as `BEST`, and selects
  it automatically. Apply remains a separate explicit action. Analysis and
  selection do not Preview, Run, mutate layers, or change routing.
- The frozen `Die Pad 1.bmp` pilot used one canonical OK image, four OK plus four
  NG representative images, and a disjoint four OK plus four NG held-out set.
  With `96x96`, stride `16`, score `0.75`, uniqueness `0.05`, angle `-8..8`, and
  scale `0.9..1.1`, the tool selected ROI `128,256,96,96`, the same ROI retained
  by P227. Representative and held-out replay both passed `8/8`; runtime and
  integrity errors were zero. Drawings consistently locate the same central
  pad/trace feature.
- The first run produced no candidate because the verification command omitted the
  already established angle/scale envelope and clustered its rows. The correction
  restored the existing matcher envelope and deterministic spread selection; no
  score, uniqueness, or success threshold was lowered.
- Library-Noah Release build and 66/66 smoke passed. Source, vendored, and current
  Debug `Lib.OpenCV.dll` retain assembly `2.1.0.0`, file `2.8.0.0`, SHA-256
  `B456BE7AFC002BA1535A5892092B746FB44560300961BD71342AAC0E7741B180`.
- Primary operator report:
  `artifacts\p229_auto_mpoint_representative_best_20260724\die_pad_1_r3_current\OPENVISIONLAB_AUTO_MPOINT_REPRESENTATIVE_BEST_REPORT.html`.
- Status: Complete for bounded automatic selection, UI integration, and split
  replay. Boundary: this is one synthetic/augmented same-source stratum, not
  semantic identity, automatic size, all-500, real-capture, production, or field
  qualification.

### P230 Frozen Die Pad 1 Full-Stratum Qualification

- Froze the operator-approved P229 template ROI `128,256,96,96` and replayed the
  unchanged score `0.75`, uniqueness `0.05`, angle `-8..8`, and scale `0.9..1.1`
  contract on all 122 rows belonging to source stratum `Die Pad 1.bmp`.
- Runtime result: 62/62 OK and 60/60 NG matched; ambiguous 0, no-match 0, runtime
  errors 0, hash-integrity errors 0, and current-run drawings 122/122.
- Opened the deterministic 35-row decision queue and all nine supplied-defect-mask
  overlap drawings. Every green runtime result stayed on the same central
  pad/trace feature; no image-frame, adjacent-pad, or wrong-location selection was
  observed.
- Nine NG masks intersect the 96x96 template bounds. This is retained as a
  production-variation risk, so the bounded decision is `Keep with documented
  limits`, not broad or field qualification.
- The first report incorrectly made any mask overlap fatal. Only the report
  decision semantics were corrected; no source, template, parameter, drawing, or
  runtime outcome changed.
- Primary operator report:
  `artifacts\p230_auto_mpoint_die_pad_1_full_stratum_20260724_r2\OPENVISIONLAB_AUTO_MPOINT_FULL_STRATUM_REPORT.html`.
- Status: Complete for this source stratum. Do not rerun or retune the same 122
  rows. Die Pad 2-4 are distinct source strata and require separate candidates if
  the operator later requests their qualification.

### P231 Product-UI Auto MPoint N-Image Report

- Audited the actual Edge Based Matching Tool View. Representative-image
  selection, multi-image `BEST` ranking, and explicit apply were present, but
  self-contained HTML export existed only in the P230 validation tool.
- Added an explicit `N-이미지 보고서 저장` action after representative analysis.
  It exports the selected candidate's retained results without rerunning matching.
- The HTML contains source/template identity, complete N-row file/hash/outcome/
  score/uniqueness/pose/runtime evidence, and a deterministic review gallery.
  Every failure is retained; larger sets add metric/pose/time extremes and
  SHA-256-spread rows, while N <= 24 shows every drawing.
- Export fails closed when source, settings, representative file identity, or
  result count changed. It does not apply a template, Preview/Run, create/select
  layers, or change input/output routing.
- Current-source UI smoke passed with 3/3 rows, four embedded PNGs, no external
  image links, and unchanged Preview/layer/routing state. The rendered HTML and
  before/current UI evidence are under
  `artifacts\p231_auto_mpoint_operator_html_report_20260724`.
- Status: Complete for the AutoMPoint representative-image teaching/report path.
  This does not replace Recipe Manager OK/NG Validation Set and Run History.

### P232 Tool View N-Image Verification Design

- Audited the current product instead of adding another batch runner. Recipe
  Manager Local Validation Sets already register up to 5,000 images, execute the
  selected Pipeline across every row, save one linked Step report/drawing per
  image, write XML/TSV batch summaries, and expose Run History analytics plus a
  deterministic failure/extreme/hash review queue.
- Current Tool Views remain single-image teaching surfaces. The common
  single-input shell exposes explicit Preview and Add Pipeline only. AutoMPoint's
  P231 representative-image report is a specialized exception, not a reusable
  Pipeline batch surface.
- The current Local Validation Set, pair, and catalog paths use ordered
  `for`/`foreach` loops with an awaited sample run. N-image batch execution is
  present; simultaneous parallel execution is not.
- Selected design: add one capability-driven `N장 검증` entry to eligible Tool
  Views, open one shared verification window, freeze the current parameters
  through the existing Add Pipeline Step adapter into a transient one-Step
  Pipeline, execute sequentially, and build drawings/table/review queue/HTML
  from retained results only. Do not copy a batch panel into every concrete Tool
  View.
- Thirteen current single-input Tool Views already have Step adapters and fit
  Phase 1. Arithmetic needs an explicit A/B image-pairing policy. HSV and
  Histogram have no current native Step adapter. Pipeline-only tool families
  continue to use Recipe Manager.
- Status: Complete as design/audit only. No production source, UI, execution, or
  dataset run changed. Design:
  `docs\OPENVISIONLAB_TOOL_VIEW_N_IMAGE_VERIFICATION_DESIGN.md`.

### P233 Shared Tool View N-Image Verification Phase 1

- Implemented the accepted P232 design as one capability-driven action in the
  shared single-input Tool View shell. The thirteen current one-Step-adapter
  Tool Views use the same modal verification window; unsupported and exceptional
  tools retain their existing paths.
- The window accepts multiple files or one top-level folder up to 5,000 images.
  Explicit Run creates the current Step exactly once, freezes its XML/SHA-256
  and ordered image list, and executes a transient `Main -> NImageResult`
  one-Step Pipeline sequentially. Stop is honored after the current row.
- Execution uses the same channel-normalization policy as the native Tool View
  Preview while retaining the original source snapshot. Every completed row
  saves its source SHA-256, run report, drawing, metrics, message, and elapsed
  time; the batch saves XML/TSV summary, Pipeline snapshot, and deterministic
  review queue.
- The self-contained HTML report is generated only from retained evidence and
  does not rerun the tool. Selection/export/open/close actions do not change
  Preview/Run count, layers, active layer, or input/output routing.
- The 30-row EdgeBasedMatching report rendered with all rows, six embedded
  review images, and no page-level horizontal overflow after long SHA-256 values
  were made to wrap inside their summary cards.
- Threshold, Blob, Line, Matching, EdgeBasedMatching, and AffineTransform each
  passed 30/30 rows with once-only Step creation, complete evidence, and exact
  direct-run status/metric equivalence.
- Status: Complete for Phase 1. Evidence:
  `artifacts\p233_tool_view_n_image_verification_20260724` and
  `docs\OPENVISIONLAB_TOOL_VIEW_N_IMAGE_VERIFICATION_DESIGN.md`.
- Boundary: this is quick execution evidence without inferred expected OK/NG
  roles or accuracy. Recipe Manager remains the formal labelled-validation
  path. Parallel workers are not implemented.

### P234 First Real-Folder Acceptance Of P233

- Reused the exact frozen P230 `Die Pad 1` EdgeBasedMatching template and
  parameters without tuning. The Step SHA-256 is
  `7CEAEC5D50259ED1337AB912F0F0A63C673F4A74E692DCDEA01BAA14FC25658F`.
- Selected a deterministic MD5-spread 12 OK + 12 NG copy from the
  operator-supplied `EasyMatch_Die_Pad_500(1)` corpus and registered that
  task-local top-level folder through the same folder helper used by P233.
- Folder registration, once-only Step creation, execution, and retained
  drawings passed 24/24. Retained source SHA-256 and decoded pixels matched the
  actual loaded inputs.
- `ScoreMax` reproduced the P230 baseline within `0.068` percentage points for
  every row under the frozen `<=0.1` integration-equivalence gate. This gate
  accommodates the already documented native Tool View input-loading path; it
  is not an inspection tolerance.
- The minimum-score row (`die_pad_240_ok.jpg`, `83.76%`) and the
  maximum-baseline-delta row (`die_pad_089_ng.jpg`, `-0.068` percentage points)
  were opened. Both runtime drawings kept the rotated rectangle and center on
  the approved central pad/trace feature.
- Status: Complete. Operator-readable report:
  `artifacts\p234_tool_n_image_real_folder_acceptance_20260724\P234_DIE_PAD_REAL_FOLDER_REPORT.html`.
  Full evidence:
  `artifacts\p234_tool_n_image_real_folder_acceptance_20260724`.
- Boundary: P234 proves one real folder-to-report integration path for an
  already qualified-with-limits locator. It does not infer OK/NG truth, add a
  defect classifier, qualify other Die Pad strata, prove parallel execution,
  or establish field robustness.

### P235 Hash-Locked Locator Validation Promotion

- Added one explicit `위치검출 세트 승격` action to completed `Matching`,
  `EdgeBasedMatching`, and `FeatureMatching` N-image sessions. It is enabled
  only when the retained session is complete, not cancelled, and every row
  succeeded.
- Promotion saves the exact one-Step Pipeline text/name without activating it,
  plus the Step SHA-256, template/dependency hashes, ordered original-file
  hashes, and one image-set SHA-256. Each row is saved as `Expected OK` for
  locator execution; source-corpus OK/NG labels are deliberately not copied as
  defect judgments.
- The retained execution source remains authoritative: its PNG hash must match
  the report, and its decoded pixels must equal the current original image.
  The current original file bytes are then separately hash-locked at promotion.
- Repeating the same promotion reuses the same deterministic Validation Set.
  A conflicting same-name Pipeline/set fails closed. Hash-locked sets disable
  add/remove/repair, require the linked Pipeline selection, and recheck
  Pipeline/dependency/image hashes before any image is executed.
- Exact P234 replay-free promotion preserved Step SHA-256
  `7CEAEC5D50259ED1337AB912F0F0A63C673F4A74E692DCDEA01BAA14FC25658F`
  and all 24 rows as locator-expected-success. Save/reload, idempotent repeat,
  wrong-Pipeline blocking, tamper rejection, zero auto-run, Recipe Manager UI,
  legacy manual OK/NG Validation Set, and N-image entry/window smokes passed.
- Status: Complete. Evidence:
  `artifacts\p235_n_image_locator_validation_promotion_20260724`.
- Boundary: this creates durable locator-stability validation ownership. It
  does not rerun or requalify P234, classify defects, infer industrial truth,
  add parallel execution, or prove field robustness.

### P236 Current-State Handoff Consolidation

- Consolidated the next-chat source of truth without adding product behavior.
  The current handoff now separates completed product slices, completed audits
  that rejected a candidate, explicit incomplete gates, frozen work, unverified
  production claims, and out-of-scope platform areas.
- Replaced the stale repository snapshot and removed reliance on historical
  readiness percentages. The older 62-66%, 98%, and similar values remain
  historical only; no new percentage was invented.
- Replaced the stale/mojibake next-chat prompt with a short restart contract
  that points first to `AGENTS.md`, this handoff, and the documentation map.
- The chronological handoff remains the detailed P1-P235 evidence index; the
  documentation map remains the authority/reading-order guide.
- Status: Complete for documentation consolidation once the documented files
  pass structure/link/diff checks. Repository publication is a separate Git
  operation and may be blocked by missing GitHub authentication.

### Maintenance: PropertyGrid Mapper Basic Image Adapter (2026-07-25)

- User-requested source/MVVM readability maintenance moved the independent
  Threshold, Morphology, Filter, and EdgeDetection PropertyGrid mapping family
  out of the root `VisionPipelineStepPropertyMapper` switch/apply chain into
  `VisionPipelineStepPropertyMapper.BasicImage.cs`.
- The root mapper now dispatches the family only; the adapter keeps the
  existing parameter defaults, aliases, metadata, and
  `VisionPipelineStepBuilder` serialization path unchanged.
- Current-source `wpf_shell_host_pipeline_step_edit_handoff` smoke, Debug
  solution build, and readiness check passed. Evidence:
  `artifacts\maintenance_property_mapper_basic_image_adapter_20260725` and
  `docs\admin\OPENVISIONLAB_PROPERTY_MAPPER_BASIC_IMAGE_ADAPTER_PROOF_20260725.md`.
- Boundary: this is a structural maintenance slice, not a new algorithm,
  inspection validation, or LLM work. The next mapper extraction must retain
  the same explicit Preview/Run and XML contracts.

### Maintenance: Validation Set Command Surface Boundary (2026-07-25)

- User-requested readability maintenance moved local Validation Set lifecycle
  responsibility out of the broad recipe handler into
  `OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs`: set creation and
  deletion, image/folder registration, path repair, persistence, and option/
  image-row projection.
- Command bindings and test entry points retain their original names. Recipe
  execution, explicit Preview/Run, layer routing, and pipeline behavior remain
  outside this slice and were not changed.
- Current-source `wpf_shell_host_recipe_local_validation_set` smoke exercised
  registration, missing-path repair, persisted metadata, explicit suite run,
  and no Preview/Run/layer/routing side effect. Debug builds passed with zero
  warnings or errors. Evidence:
  `artifacts\\maintenance_recipe_validation_set_refactor_20260725` and
  `docs\\admin\\OPENVISIONLAB_RECIPE_COMMAND_VALIDATION_SET_REFACTOR_PROOF_20260725.md`.
- Boundary: structural maintenance only; it does not qualify an inspection,
  modify Validation Set data contracts, or reopen LLM work.

### Maintenance: LLM XML Draft Workflow Boundary (2026-07-25)

- Moved the already-supported XML draft load, review-bundle dry-run,
  validation/dependency inspection, import, and import-readiness ownership into
  `OpenVisionShellHostRecipeCommandSurface.LlmXmlDraftWorkflow.cs`.
- Guided Setup template creation and prompt composition remain outside this
  partial. This is a frozen-LLM maintenance refactor, not provider or prompt
  expansion; command names, test entry points, XML validation, dependency-copy,
  explicit Preview/Run, and layer-routing contracts are unchanged.
- Current-source `wpf_shell_host_recipe_manager_summary` smoke and Debug builds
  passed with zero warnings or errors. Evidence:
  `artifacts\\maintenance_llm_xml_draft_workflow_refactor_20260725` and
  `docs\\admin\\OPENVISIONLAB_LLM_XML_DRAFT_WORKFLOW_REFACTOR_PROOF_20260725.md`.

### Maintenance: Object Inspection Property Mapper Boundary (2026-07-25)

- Moved the Blob/Contour PropertyGrid family into
  `VisionPipelineStepPropertyMapper.ObjectInspection.cs`: creation with existing
  defaults, Blob fixture parameter serialization, and the two specialized
  PropertyGrid models.
- The root mapper now delegates that family and retains common step persistence.
  XML keys, existing area/width/height filters, acceptance metadata, and
  explicit Preview/Run behavior were not changed.
- Current-source `p216_object_dimension_filters_property_grid` smoke and Debug
  builds passed with zero warnings or errors. Evidence:
  `artifacts\\maintenance_property_mapper_object_inspection_adapter_20260725` and
  `docs\\admin\\OPENVISIONLAB_PROPERTY_MAPPER_OBJECT_INSPECTION_ADAPTER_PROOF_20260725.md`.

### Maintenance: Line Pair Property Mapper Boundary (2026-07-25)

- Moved the LineDistance/LineIntersection two-line PropertyGrid model into
  `VisionPipelineStepPropertyMapper.LinePair.cs`: Left/Right baseline restore,
  pair construction, serialization, and the public pair projection.
- The root mapper retains dispatch and common step flow. The single LineGauge
  mapping, measurement algorithm, XML semantics, calibration, drawings, and
  explicit Preview/Run were not changed.
- Current-source `wpf_shell_host_recipe_line_pair_properties` smoke confirmed
  independent Line A/B ROI fields and XML round trip; Debug builds passed with
  zero warnings or errors. Evidence:
  `artifacts\\maintenance_property_mapper_line_pair_adapter_20260725` and
  `docs\\admin\\OPENVISIONLAB_PROPERTY_MAPPER_LINE_PAIR_ADAPTER_PROOF_20260725.md`.

### Maintenance: Matching Property Mapper Boundary (2026-07-25)

- Moved general `Matching`/`TemplateMatching` PropertyGrid construction,
  fixture-frame publication serialization, and its specialized PropertyGrid
  model into `VisionPipelineStepPropertyMapper.Matching.cs`.
- The root mapper retains family dispatch and common persistence. Score/angle/
  scale behavior, template ownership, fixture semantics, XML defaults,
  EdgeBasedMatching, FeatureMatching, and explicit Preview/Run were not
  changed.
- Current-source `wpf_shell_host_recipe_fixture_properties` smoke and Debug
  builds passed with zero warnings or errors. Evidence:
  `artifacts\\maintenance_property_mapper_matching_adapter_20260725` and
  `docs\\admin\\OPENVISIONLAB_PROPERTY_MAPPER_MATCHING_ADAPTER_PROOF_20260725.md`.

## Known Gaps And Honest Limits

Large-corpus skill validation now follows `docs\OPENVISIONLAB_SCALABLE_SKILL_VALIDATION_PROTOCOL.md`. OpenVisionLab executes every declared row but does not treat 500/10,000 successful executions or LLM visual opinion as semantic truth. XML/corpus identity is frozen before execution; a small operator-approved semantic gold set owns physical meaning; review is limited to a deterministic queue of all failures/misclassifications, confidence and measurement extremes, declared strata, and a hash-seeded random audit sample. Per-image tuning is prohibited, correction is limited to two bounded cycles, and every skill must close as `Keep`, `Keep with documented limits`, `Hybrid candidate`, or `Reject`.

1. **Real correction-loop breadth is incomplete and intentionally deferred.** P136 adds one clean-Dev same-conversation GPT dependency-path correction with public assets, but this does not prove broad or cross-provider authoring reliability. P196 places the LLM track in maintenance mode, so Gemini/Claude/provider breadth and the missing natural Pin Phase 3 failure are not active backlog.
2. **Cross-install template relocation is now proven only for the three template tool families.** P137 proves moved-package execution for Matching, EdgeBasedMatching, and FeatureMatching when templates reside under the moved package's `RECIPE` tree. It does not qualify installer behavior, arbitrary external dependencies, updates, signing, or deployment support.
3. **Industrial validation is incomplete.** P144 proves a held-out 500-image synthetic Die Pad corpus, but public/local synthetic samples still do not establish robustness across real production part variation, camera noise, lighting, fixturing, calibration drift, or operator error.
4. **P214 records scale evidence but does not certify calibration.** Pixel results remain usable without scale. P214 can derive and hash-lock one uniform mm/px value from two reviewed points and a user-supplied real distance, but the operator remains responsible for the physical standard, imaging geometry, uncertainty, and field verification. Do not imply certified physical accuracy from a positive `PIXELPERMM` alone.
5. **No novice usability measurement exists.** The flow has guided surfaces and visual evidence, but no independent beginner study verifies that a first-time user understands the workflow without help.
6. **No blanket branch-comparison gap is proven.** Existing review handles direct `InputLayer`, declared `SourceLayers`, and declared `SourceSteps` relations. Extend it only after a real multi-branch recipe demonstrates a missing relationship outside those contracts.
7. **Host cleanup has a stop condition.** The remaining composition files are not automatic debt. Further cleanup must be driven by a real responsibility boundary, not folder appearance.
8. **Release readiness is not field readiness.** Build/policy checks and release policies exist; no signed installer, deployment support program, or production acceptance campaign is claimed.
9. **The retained `bin\Debug` folder is not a clean runtime or deployment artifact.** P133 records the approved contract: use a new timestamped Dev runtime under `artifacts` for current EXE evidence and a new `dist\OpenVisionLab` package for release evidence. Preserve the retained local workspace without automatic deletion or migration. P134/P137 separately verify copied-template behavior in and after a package move.
10. **The P147 Pin_1 GPT recipe is not validated for the supplied corpus.** Its 52.40% accuracy remains an immutable baseline. P148/P168 separately prove only the frozen two-row pixel adjacent edge-gap consistency signal; they do not establish a whole Pin_1 defect classifier, center-pitch measurement, calibration result, or field qualification.
11. **Matching-driven normalized measurement is bounded, not broadly qualified.** P182 proves the reviewed coordinate path, P183 adds the bounded fail-closed gate, and P184 replays that exact guarded pixel measurement on all 500 supplied top-left images with same-run drawings/hashes. LLMs may author this guarded contract, but must not promote observed distances to acceptance thresholds, calibration, other directions, or general robustness.
12. **C9 has top-left full-corpus evidence, not broad qualification.** P175 qualified native Matching on the observed set, P181 proved the transform, P182 proved coordinate-correct measured edges, P183 froze the starter gates, and P184 classified all 500 supplied top-left executions as a pixel measurement or a named fail-closed outcome. Unseen/all-direction robustness, black-strip truth, production tolerances, calibration, and field qualification remain open.
13. **Dedicated Matching evidence is still bounded.** P178 proves 122/122 current-EXE localization for one synthetic `Die Pad 1.bmp` family; P179 proves the pose/scale Pipeline path on four representative cases and exactly reproduces three P178 native rows. The template is a tight rectangle, not an alpha mask. Zero degrees means the selected horizontal baseline is rectified; perspective remains. Neither result supplies generator transform ground truth, mixes the four source variants under one template, proves multi-target ambiguity handling, or replaces real captured validation.
14. **Center pitch now has bounded N-sample and product-workflow evidence, not independent qualification.** P201 separates the deterministic metric and PropertyGrid contract; P202 freezes a two-row `<=12 px` candidate with perfect Working Train/Validation classification and reviewed drawings on one synthetic/augmented corpus; P203 proves that exact Validation candidate through Recipe Manager saved validation, stored drawings, and deterministic Run History queue identity. The Validation images were previously observed by the older edge-gap workflow, no independent non-P169 Test was consumed, and bright polarity, calibration, real production variation, and field qualification remain unproved. The frozen LLM EdgeGap Guided Setup still does not support center pitch.
15. **The selected missing-pin corpus has target-bearing Train/Validation but not Test.** P204 isolates 50 class-30 missing-pin rows without contaminating the intent with four other NG classes. The provided split places 38 target rows in Train, 12 in Validation, and zero in Test. A future product candidate may be developed on Train/Validation, but it cannot be called independently Test-qualified without an operator-approved content-hash-disjoint target-bearing split.
16. **A correct count without a correct drawing is not missing-pin evidence.** P205's fixed raw ROI candidate numerically separated its six selected rows after one area correction, but one missing row balanced a false lower-rail Blob against a missed border pin. Treat the candidate as rejected and solve row/rail geometry before counting; do not promote 6/6 numerical outcomes or continue threshold/area tuning.
17. **P206 judges nine stable interior slots, not every visible border fragment.** The fixed rectification candidate excludes source-border-truncated pins because their visible area varies independently of the missing-pin target. Its six-row success permits one frozen extreme replay only; it does not prove whole-row counting or justify changing the ROI/count gate per image.
18. **P207 supersedes P206's candidate success with a Reject decision.** The frozen border-extreme replay proved one right boundary pin can enter the fixed aligned ROI, causing both an OK false reject and a missing-pin false accept by numerical cancellation. Do not quote P206's 6/6 result as the current candidate state, retune its fixed ROI, or advance it to Train/Validation.
19. **P208 is a proposal, not locator evidence.** Candidate B is visible in all eight reviewed crops, but no Matching run or physical-feature provenance exists. Do not call it stable, teach it, or tune gates until the operator confirms it is the same durable fixture feature.
20. **P209 closes the current missing-pin/count attempt as Reject.** Candidate B's coarse ROI can localize some rows, but two OK rows fail the unchanged score gate and one target NG false-accepts through rail-fragment cancellation. Do not spend more cycles on score/ROI/threshold/area/count tuning or claim that location normalization solves the semantic count problem.
21. **P211 makes current Blob/Contour objects inspectable; P216 later adds bounded width/height filtering.** Contour's rejected-candidate list remains deliberately bounded to candidates at or above 25% of `MIN_AREA`; do not describe it as every threshold speck or as semantic defect classification.
22. **P212 visualizes an existing coordinate contract; it does not qualify the locator or recipe.** The source/normalized ROI pair is computed from the saved reference and current reviewed pose for operator inspection. It is not new rotation/scale, unseen-data, calibration, tolerance, or field-robustness evidence.
23. **P213 is complete only within its frozen pixel-only synthetic/UI boundary.** Do not promote it to calibrated measurement, industrial semantic accuracy, unseen-data robustness, field qualification, automatic feature selection, or experimental `OuterCornerIntersection` correctness.
24. **P214 is a uniform scale teaching aid, not a camera calibration system.** Its source hash prevents silently applying saved evidence to changed pixels, but it does not correct distortion/perspective, prove the user's known distance, estimate uncertainty, or qualify any downstream inspection tolerance.
25. **P215 was selection evidence only and is superseded by P216 runtime support.** Do not cite P215 alone as implementation evidence.
26. **P216 filters axis-aligned pixel bounds, not physical or semantic object shape.** A passing width/height rectangle does not prove the object is the intended part, and no operator dataset was run. Do not infer rotated dimensions, aspect/circularity, defect class, calibrated size, industrial robustness, or field qualification.
27. **P219 wires detected Points; it does not choose or qualify them.** The operator owns three stable physical features, their ordered correspondence, locator gates, destination frame, and downstream inspection truth. A passing synthetic chain is not evidence that arbitrary Matching templates remain stable under production variation.
28. **P220 does not qualify the approved Matching-center fixture at `<=3 px`.** The bounded r2 removed coarse-ROI omissions and the `P` wrong locator, but two of twelve normalized rows retained `4.12/5.00 px` center residual. Do not hide this with a looser gate or a larger replay. The downstream inspection must define whether that registration error is acceptable.
29. **P221 accepts only one coarse fixed-ROI linkage at the observed `<=5 px` envelope.** The date-area Mean Step proves that an unchanged reference-coordinate ROI can execute and draw on the same 12 rows. Its unjudged brightness values do not define an inspection, tolerance, defect class, unseen robustness, or field qualification.
30. **P222/P223 suggest and apply matching candidates; they do not qualify them.** A passing one-image synthetic score, uniqueness gate, known-transform replay, and UI apply path make a candidate reviewable, not production-stable. OpenVisionLab labels it `Suggested`, requires explicit apply and Matching Preview, and keeps runtime uniqueness/N-image qualification separate.
31. **P224 rejects bounded synthetic ambiguity; it does not qualify an anchor.** Internal Top-K and explicit `NoMatch`/`Success`/`Ambiguous` semantics close the hidden-second-candidate contract, but the default margin and spatial separation are not production tolerances. A fixed physical ROI, motion envelope, pose-error gate, and representative repeat/false-accept evidence remain prerequisites.
32. **P225 rejects the approved card `R` as the first EdgeBased fixed-ROI candidate.** Unique validation rejected some ambiguous rows but did not remove the two broad-ROI wrong accepts, and one reviewed-ROI result uniquely selected `T` instead of `R`. Do not call uniqueness a semantic identity check or advance this candidate to pose refinement.
33. **P226 produces reviewable suggestions, not a qualified locator.** The public EasyMatch drawings demonstrate both useful rejection of repeated frame windows and false semantic confidence on rotated repeated floppy hubs. Do not auto-apply the highest score or call 20 suggestions matcher qualification; physical-feature approval and cross-image evidence remain separate gates.
34. **P227 narrows the six corpora to ten expansion candidates; it does not qualify them.** The corrected 104-row pilot removed the grayscale/threshold contract defect and retained current-run drawings, but three strata produced no suggestion, one failed the OK gate, one selected a repeated grid intersection, and one selected an image-frame boundary. Do not run all 500 rows or call the remaining ten production-ready until the operator approves one named physical feature from the report.
35. **P228 makes the evidence easier to review; it does not strengthen the algorithm result.** The self-contained HTML and browser print/PDF path preserve the same P227 classifications and limits. Do not treat the polished presentation as additional locator qualification.
36. **P229 automatically selects from actual cross-image evidence; it still does not know physical meaning.** The selected Die Pad candidate wins on representative success and worst-case uniqueness and replays on a disjoint held-out slice, but only the operator can confirm that the central pad/trace is a durable locator outside the judged defect region. Do not equate `BEST` with `Qualified`.
37. **P230 qualifies one source stratum with limits, not the complete 500-image package.** All 122 `Die Pad 1.bmp` rows located the same feature, but nine supplied NG masks intersect the pattern bounds. Keep this as an explicit variation risk and do not reuse the template for distinct Die Pad 2-4 sources or claim real-capture/field robustness.
38. **P231 exports retained representative-image evidence; it is not a classification report.** The green box and red center are rendered from the exact retained runtime pose without rerunning matching. Use Recipe Manager Validation Set/Run History when OK/NG roles, recipe acceptance, or saved inspection history are required.
39. **N-image and parallel execution are different capabilities.** Recipe Manager already runs N images, but current loops are sequential. P232 deliberately keeps the first shared Tool View quick-verification slice sequential; parallel workers require isolated tool/template/Mat state and sequential-equivalence proof.
40. **P233 N-image success is not automatic OK/NG accuracy.** The shared Tool View surface preserves the frozen Step's actual execution/acceptance result, metrics, source snapshot, and drawing, but it does not guess sample roles or semantic truth. Use Recipe Manager Validation Set/Run History for labelled qualification, and do not claim parallel execution from the sequential Phase 1 runner.
41. **P234 closes first real-folder integration, not locator semantics.** The 24/24 result proves that one frozen P230 locator survives P233 folder registration, execution, evidence retention, and HTML export. The OK/NG labels were used only for balanced sampling; no classification gate was applied. Do not rerun or tune this corpus, transfer the template to Die Pad 2-4, or call it parallel/field qualification.
42. **P235 preserves locator identity, not defect truth.** Its 24 Expected OK rows mean the frozen locator is expected to execute successfully on every registered image. They do not mean the original OK/NG defect labels were reclassified, and they do not upgrade the P230/P234 semantic or field-robustness boundary.

## Current Next Priority Order

### Maintenance: EdgeBasedMatching Property Mapper Boundary (2026-07-25)

- `VisionPipelineStepPropertyMapper.EdgeBasedMatching.cs` now owns the EdgeBasedMatching aliases' PropertyGrid defaults and metadata model. The root mapper retains only the tool-family dispatch.
- The existing opt-in unique-match/Top-K contract, XML/default keys, explicit Preview/Run behavior, layers, and routes were unchanged.
- Verification passed: Debug build (0 warnings/0 errors), current-source `wpf_shell_host_edge_based_matching_tool` smoke, and visual review of `artifacts\maintenance_property_mapper_edge_based_matching_adapter_20260725\wpf_shell_host_edge_based_matching_tool.png`.
- Evidence: `docs\admin\OPENVISIONLAB_PROPERTY_MAPPER_EDGE_BASED_MATCHING_ADAPTER_PROOF_20260725.md`.

### Maintenance: FeatureMatching Property Mapper Boundary (2026-07-25)

- `VisionPipelineStepPropertyMapper.FeatureMatching.cs` now owns the Feature/FeatureMatching/SIFT PropertyGrid defaults and metadata model. The root mapper retains only the tool-family dispatch.
- The existing score, RANSAC reprojection, template-path, common OpenCV parameter, explicit Preview/Run, layer, and route contracts were unchanged.
- Verification passed: Debug build (0 warnings/0 errors), current-source `wpf_shell_host_feature_matching_tool` smoke, and visual review of `artifacts\maintenance_property_mapper_feature_matching_adapter_20260725\wpf_shell_host_feature_matching_tool.png`.
- Evidence: `docs\admin\OPENVISIONLAB_PROPERTY_MAPPER_FEATURE_MATCHING_ADAPTER_PROOF_20260725.md`.

### Maintenance: Recipe Workspace Command Boundary (2026-07-25)

- `OpenVisionShellHostRecipeCommandSurface.RecipeWorkspace.cs` now owns recipe creation, named creation, duplication, rename, deletion, command enablement, and the post-create workspace switch. The generic handler partial no longer owns those methods.
- Existing workspace storage calls, confirmation gates, option refresh, explicit Preview/Run, layers, and routes were unchanged.
- Verification passed: Debug build (0 warnings/0 errors), current-source `wpf_shell_host_recipe_manager_summary` smoke, and visual review of `artifacts\maintenance_recipe_workspace_command_refactor_20260725\wpf_shell_host_recipe_manager_summary.png`.
- Evidence: `docs\admin\OPENVISIONLAB_RECIPE_WORKSPACE_COMMAND_REFACTOR_PROOF_20260725.md`.

### Maintenance: Pipeline Lifecycle Command Boundary (2026-07-25)

- `OpenVisionShellHostRecipeCommandSurface.PipelineLifecycle.cs` now owns pipeline activation, duplication, rename, deletion, and sample-pipeline duplication. The generic handler partial no longer owns those methods.
- Existing storage calls, confirmation gates, selected-pipeline refresh, active-pipeline switching, explicit Preview/Run, layers, and routes were unchanged.
- Verification passed: Debug build (0 warnings/0 errors) and current-source `wpf_shell_host_recipe_context_switch` smoke. Artifact: `artifacts\maintenance_pipeline_lifecycle_command_refactor_20260725_context\wpf_shell_host_recipe_context_switch.png`.
- Separate recorded risk: `wpf_shell_host_recipe_language_controls` reproduced a later LLM dependency-report Korean token assertion failure after its recipe/pipeline lifecycle checks. Do not classify that as pipeline lifecycle evidence; diagnose only if LLM maintenance is explicitly reopened.
- Evidence: `docs\admin\OPENVISIONLAB_PIPELINE_LIFECYCLE_COMMAND_REFACTOR_PROOF_20260725.md`.

### Maintenance: Pipeline Exchange Command Boundary (2026-07-25)

- `OpenVisionShellHostRecipeCommandSurface.PipelineExchange.cs` now owns XML import/export, review-bundle export, and reference collection. The generic handler partial no longer owns those methods.
- Existing selected-recipe guards, storage, review-bundle dry-run routing, reference collection, explicit Preview/Run, layers, and routes were unchanged.
- Verification passed: Debug build (0 warnings/0 errors), current-source `wpf_shell_host_recipe_review_bundle_import`, and Recipe context smoke. Artifact: `artifacts\maintenance_pipeline_exchange_command_refactor_20260725_import\wpf_shell_host_recipe_review_bundle_import.png`.
- Separate recorded risk: `wpf_shell_host_recipe_review_bundle` exported the bundle, then expected an advanced XML button while the current manager stayed in its default summary view. Treat this as stale smoke UI precondition maintenance, not an exchange-command regression.
- Evidence: `docs\admin\OPENVISIONLAB_PIPELINE_EXCHANGE_COMMAND_REFACTOR_PROOF_20260725.md`.

### Maintenance: Run History Command Boundary (2026-07-25)

- `OpenVisionShellHostRecipeCommandSurface.RunHistory.cs` now owns recent batch-history refresh, baseline selection, and default sample-result selection. The generic handler partial no longer owns those methods.
- The three-run history limit, previous selection retention, automatic baseline choice, review-queue/NG default order, explicit Preview/Run, layers, and routes were unchanged.
- Verification passed: Debug build (0 warnings/0 errors) and current-source `wpf_shell_host_recipe_local_validation_set` smoke. Artifact: `artifacts\maintenance_run_history_command_refactor_20260725\wpf_shell_host_recipe_local_validation_set.png`.
- Evidence: `docs\admin\OPENVISIONLAB_RUN_HISTORY_COMMAND_REFACTOR_PROOF_20260725.md`.

### Maintenance: Pipeline Exchange Application-Service Boundary (2026-07-25)

- `OpenVisionRecipePipelineExchangeUseCase` now owns pipeline XML import/export, review-bundle creation, and XML serialization with explicit inputs/results. `OpenVisionShellHostRecipeCommandSurface.PipelineExchange.cs` is only the selected-recipe/UI adapter; the earlier partial split is therefore not treated as the completed architecture by itself.
- Existing selected-recipe guards, review-bundle dry-run routing, selected-reference collection, explicit Preview/Run, layers, and routes remain unchanged.
- Verification passed: Debug build (0 warnings/0 errors), current-source `wpf_shell_host_recipe_review_bundle_import`, and readiness check. Artifact: `artifacts\mvvm_pipeline_exchange_usecase_20260725\wpf_shell_host_recipe_review_bundle_import.png`.
- Evidence: `docs\admin\OPENVISIONLAB_PIPELINE_EXCHANGE_USECASE_REFACTOR_PROOF_20260725.md`.

### Maintenance: Recipe Workspace Application-Service Boundary (2026-07-25)

- `OpenVisionRecipeWorkspaceUseCase` now owns recipe creation, duplicate-name generation, workspace duplicate/rename/delete, fallback workspace preparation, and default-pipeline preparation. `OpenVisionShellHostRecipeCommandSurface.RecipeWorkspace.cs` is the command/UI adapter; the earlier partial split is not treated as the completed architecture by itself.
- Existing command enablement, delete confirmation, selected-option fallback choice, recipe switch, explicit Preview/Run, layers, and routes remain unchanged.
- Verification passed: Debug build (0 warnings/0 errors), current-source `wpf_shell_host_recipe_manager_summary`, and readiness check. Artifact: `artifacts\mvvm_recipe_workspace_usecase_20260725\wpf_shell_host_recipe_manager_summary.png`.
- Evidence: `docs\admin\OPENVISIONLAB_RECIPE_WORKSPACE_USECASE_REFACTOR_PROOF_20260725.md`.

### Maintenance: Run History Presenter Boundary (2026-07-25)

- `OpenVisionRecipeRunHistoryPresenter` now owns recent-run projection, retained selection, baseline/automatic baseline choice, and batch/pair default result selection. `OpenVisionShellHostRecipeCommandSurface.RunHistory.cs` is the storage/UI adapter; the earlier partial split is not treated as the completed architecture by itself.
- Existing persisted run data, execution, reports, drawings, explicit Preview/Run, layers, and routes remain unchanged.
- Verification passed: Debug build (0 warnings/0 errors), current-source `wpf_shell_host_recipe_local_validation_set`, and readiness check. Artifact: `artifacts\mvvm_run_history_presenter_20260725\wpf_shell_host_recipe_local_validation_set.png`.
- Evidence: `docs\admin\OPENVISIONLAB_RUN_HISTORY_PRESENTER_REFACTOR_PROOF_20260725.md`.

### Maintenance: Pipeline Lifecycle Application-Service Boundary (2026-07-25)

- `OpenVisionRecipePipelineLifecycleUseCase` now owns activation, duplicate-name generation, duplicate, rename, delete/fallback resolution, and sample-pipeline import/activation. `OpenVisionShellHostRecipeCommandSurface.PipelineLifecycle.cs` is the command/UI adapter; the earlier partial split is not treated as the completed architecture by itself.
- Existing command guards, delete confirmation, active-state refresh, explicit Preview/Run, layers, and routes remain unchanged.
- Verification passed: Debug build (0 warnings/0 errors), current-source `wpf_shell_host_recipe_context_switch`, and readiness check. Artifact: `artifacts\mvvm_pipeline_lifecycle_usecase_20260725\wpf_shell_host_recipe_context_switch.png`.
- Evidence: `docs\admin\OPENVISIONLAB_PIPELINE_LIFECYCLE_USECASE_REFACTOR_PROOF_20260725.md`.

### Maintenance: Validation Set Presenter Boundary (2026-07-26)

- `OpenVisionRecipeValidationSetPresenter` now owns Validation Set option/image-row projection and retained selection resolution. `ValidationSets.cs` remains the storage/WPF adapter.
- Verification passed: Debug build (0 warnings/0 errors), current-source `wpf_shell_host_recipe_local_validation_set`, and readiness check. Artifact: `artifacts\mvvm_validation_set_presenter_20260726\wpf_shell_host_recipe_local_validation_set.png`.
- Evidence: `docs\admin\OPENVISIONLAB_VALIDATION_SET_PRESENTER_REFACTOR_PROOF_20260726.md`.

### Maintenance: Next Structural Boundary Audit (2026-07-26)

- Do not add another command-surface partial. LLM command work remains frozen; Pipeline Review Document needs a later state/execution boundary rather than a file move.
- The next selected structural candidate is one remaining `VisionPipelineStepPropertyMapper` ToolType family with explicit Create/Apply XML adapter behavior. Evidence: `docs\admin\OPENVISIONLAB_NEXT_STRUCTURAL_BOUNDARY_AUDIT_20260726.md`.

### Maintenance: Transform Property Adapter Boundary (2026-07-26)

- `VisionPipelineTransformPropertyAdapter` now owns RotateScale/AffineTransform
  alias recognition, PropertyGrid parameter/default projection, Step creation,
  fixture-consumer parameters, and detected Point binding parameters.
- The root mapper no longer contains transform ToolType cases or direct
  transform builder calls. This is a non-partial mapping boundary; shared Step
  metadata/copy remains in the root mapper.
- Verification passed: Debug build (0 warnings/0 errors), Affine transform
  contract, and current-source RotateScale/Affine/P219 PropertyGrid UI smokes.
  Artifact: `artifacts\refactor_transform_adapter_20260726`.
- Evidence:
  `docs\admin\OPENVISIONLAB_TRANSFORM_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`.

### Maintenance: Pipeline Review Flow Presenter Boundary (2026-07-26)

- `OpenVisionPipelineReviewFlowPresenter` now owns previous-output, branch,
  upstream-producer, missing-input, status/text, full-list, and selected-Step
  flow projection.
- `OpenVisionPipelineReviewDocument` now supplies layer-image and execution
  summary state and applies the returned projection. The Presenter does not
  depend on the Document, View, display manager, or execution controller.
- Verification passed: Debug build (0 warnings/0 errors) and current-source
  normal/input-state/NG Pipeline Review UI smokes. Artifact:
  `artifacts\mvvm_pipeline_review_flow_presenter_20260726`.
- Evidence:
  `docs\admin\OPENVISIONLAB_PIPELINE_REVIEW_FLOW_PRESENTER_REFACTOR_PROOF_20260726.md`.

### Maintenance: Step Edit Session ViewModel Boundary (2026-07-26)

- `OpenVisionRecipeStepEditSessionViewModel` now owns the selected Step edit
  object, dirty flag, status text, corrected-output review text, and their
  Load/Dirty/Clean/Clear transitions.
- The Shell retains XML lookup/save, tool-session seeding, and a property
  notification adapter for existing XAML bindings. The four old mutable Shell
  fields were removed.
- Verification passed: extended current-source selected-Step handoff,
  Fixture edit/apply/rerun, old-field absence, Debug build, and readiness.
  Artifacts: `artifacts\mvvm_step_edit_session_viewmodel_20260726_r3` and
  `artifacts\mvvm_step_edit_session_viewmodel_20260726_r4`.
- The Fixture smoke's pre-existing hidden-button failure was fixed by selecting
  its Step Details tab before clicking the edit action.
- Evidence:
  `docs\admin\OPENVISIONLAB_STEP_EDIT_SESSION_VIEWMODEL_REFACTOR_PROOF_20260726.md`.

### Maintenance: Validation Run Session ViewModel Boundary (2026-07-26, consolidated)

- `OpenVisionRecipeExecutionSessionViewModel` now owns Validation Suite
  running, Local Validation Set running, stop-requested, and status-text state
  plus their Start/RequestStop/Complete/SetStatus transitions.
- The Shell retains explicit execution, image iteration, frozen-identity
  validation, judgment, report persistence, Run History refresh, and an
  existing-binding notification adapter. The four old mutable Shell fields
  were removed.
- Verification passed: old-field absence, current-source Local Validation Set
  complete run and stop/partial-save path, unchanged Preview/Run/layers/
  workspace/routes, Debug build, focused smoke build, and readiness.
  Artifact:
  `artifacts\mvvm_validation_run_session_viewmodel_20260726\wpf_shell_host_recipe_local_validation_set.png`.
- Evidence:
  `docs\admin\OPENVISIONLAB_VALIDATION_RUN_SESSION_VIEWMODEL_REFACTOR_PROOF_20260726.md`.

### Maintenance: Recipe Execution Session ViewModel Boundary (2026-07-26)

- The existing validation-session owner was consolidated rather than followed
  by another small ViewModel. `OpenVisionRecipeExecutionSessionViewModel` now
  owns Validation Suite, Local Validation Set, selected-sample, Good/Bad pair,
  and Catalog running state plus validation stop/status transitions.
- The Shell no longer owns the sample/pair/catalog running fields. It retains
  exact command guards, explicit execution, result summaries, iteration,
  judgment, storage, Run History, and the existing-binding notification
  adapter.
- Verification passed: six old running-field absence, current-source Local
  Validation complete/stop/partial-save, real Good/Bad pair rerun, unchanged
  Preview/Run/layers/workspace/routes, Debug build, and readiness.
  Artifacts:
  `artifacts\mvvm_recipe_execution_session_viewmodel_20260726`.
- Evidence:
  `docs\admin\OPENVISIONLAB_RECIPE_EXECUTION_SESSION_VIEWMODEL_REFACTOR_PROOF_20260726.md`.
- Remaining structural priority: audit the root
  `VisionPipelineStepPropertyMapper` direct Create/Apply families and select at
  most one real non-partial adapter boundary. Recommended model:
  gpt-5.6-terra | Reasoning effort: medium.

### Maintenance: ReferenceDifference Property Adapter Boundary (2026-07-26)

- `VisionPipelineReferenceDifferencePropertyAdapter` now owns ToolType
  recognition, current parameter/default projection, legacy `ReferencePaths`
  fallback, the PropertyGrid model, canonical Step reconstruction, and metric
  owner identification for `ReferenceDifference`.
- The root mapper no longer owns a direct `referencedifference` switch case,
  private property model, `ToStep`, or reference-path helper. It retains adapter
  dispatch and shared metadata/parameter copying.
- The readiness contract now checks the new owner and root dispatch separately.
- Verification passed: Debug build (0 warnings/0 errors), current-source
  `wpf_shell_host_recipe_reference_difference_properties`, visual inspection,
  and readiness. Artifact:
  `artifacts\refactor_reference_difference_adapter_20260726\wpf_shell_host_recipe_reference_difference_properties.png`.
- Evidence:
  `docs\admin\OPENVISIONLAB_REFERENCE_DIFFERENCE_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`.
- Remaining structural priority: re-audit remaining root mapper families and
  select one only when a dedicated round-trip regression can prove its
  behavior. Recommended model: gpt-5.6-terra | Reasoning effort: medium.

### Maintenance: PinArrayGap Property Adapter Boundary (2026-07-26)

- `VisionPipelinePinArrayGapPropertyAdapter` now owns PinArrayGap/
  AdjacentPinGap alias recognition, parameter/default projection, the
  PropertyGrid model, baseline parameter preservation, Step reconstruction,
  and metric-owner identification.
- The root mapper no longer owns the direct aliases, private property model, or
  `ToStep`. It retains adapter dispatch and shared metadata/parameter copying.
- The focused smoke now also proves `AdjacentPinGapTool` alias/default/baseline
  round trip while retaining the existing `ALLOW_BRANCH_INPUT`, PropertyGrid,
  saved-recipe, and zero-Preview/Run checks.
- Verification passed: Debug build (0 warnings/0 errors), current-source
  `wpf_shell_host_recipe_pinarraygap_properties`, visual inspection, and
  readiness. Artifact:
  `artifacts\refactor_pinarraygap_adapter_20260726_r2\wpf_shell_host_recipe_pinarraygap_properties.png`.
- Evidence:
  `docs\admin\OPENVISIONLAB_PIN_ARRAY_GAP_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`.
- The Line Pair partial remains coupled to the GeometryMeasure/CircleGauge base
  in the same file and was deliberately not changed in this one-family slice.
  Remaining structural priority: design its smallest clean independent
  boundary and require both Line Pair and geometry regressions before
  implementation. Recommended model: gpt-5.6-terra | Reasoning effort:
  medium.

### Maintenance: Line Pair Property Adapter Boundary (2026-07-26)

- The old `VisionPipelineStepPropertyMapper.LinePair.cs` partial was removed.
  `VisionPipelineLinePairPropertyAdapter` now owns LineDistance/
  LineIntersection recognition, prefixed Line A/B projection, the PropertyGrid
  model, Step reconstruction, Tool View LineGauge pair handoff, and metric
  identification.
- The root mapper retains create/apply/metric dispatch, shared parameter
  helpers, and a thin public `TryCreateLineGaugePair` compatibility forwarder.
- The misplaced `PipelineGeometryPropertyBase` moved beside its only derived
  root models, GeometryMeasure and CircleGauge. It is no longer owned by the
  Line Pair adapter.
- Verification passed: Debug build (0 warnings/0 errors), current-source Line
  Pair PropertyGrid including `LineIntersectionTool` alias/default round trip,
  P213 Geometry PropertyGrid, P213 Geometry Review/core, visual inspection, and
  readiness. Artifacts:
  `artifacts\refactor_line_pair_adapter_20260726_r2` and
  `artifacts\refactor_line_pair_adapter_20260726`.
- Evidence:
  `docs\admin\OPENVISIONLAB_LINE_PAIR_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`.
- Remaining structural priority: audit GeometryMeasure/CircleGauge as one
  cohesive adapter candidate while keeping both P213 regressions as mandatory
  completion evidence. Recommended model: gpt-5.6-terra | Reasoning effort:
  medium.

### Maintenance: Geometry Property Adapter Boundary (2026-07-26)

- `VisionPipelineGeometryPropertyAdapter` now owns GeometryMeasure/
  GeometricMeasurement/CircleGauge recognition, shared baseline/acceptance
  state, both PropertyGrid models, typed feature selection, reference parsing,
  ROI formatting, Step reconstruction, and metric identification.
- The root mapper no longer owns direct geometry ToolType/apply/metric cases or
  the geometry base/models/converter/helpers. It retains adapter dispatch and
  existing shared parameter/metadata/final-copy infrastructure.
- No new interface, factory, or duplicate parameter codec was added.
- Verification passed: Debug build (0 warnings/0 errors), current-source P213
  Geometry PropertyGrid and Geometry Review/core, all seven GeometryMeasure
  modes, CircleGauge gates, GeometricMeasurementTool/CircleGaugeTool alias
  round trips, visual inspection, and readiness. Artifact:
  `artifacts\refactor_geometry_adapter_20260726`.
- Evidence:
  `docs\admin\OPENVISIONLAB_GEOMETRY_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`.
- Remaining structural priority: re-audit direct and partial mapper families
  and select none unless a dedicated current round-trip regression can prove
  the boundary. Recommended model: gpt-5.6-terra | Reasoning effort: medium.

1. **No active implementation priority after P235.**
   - The explicitly requested exact locator-session promotion passed without a product blocker. Wait for a concrete operator workflow blocker or verified current-build regression. Until then, do not spend model tokens or start another semantic image campaign. Recommended model: none until evidence exists | Reasoning effort: none until evidence exists.

2. **Audit isolated-worker equivalence only after a measured bottleneck and explicit request.**
   - Prove per-image Pipeline/tool/template/Mat isolation and identical sequential versus `1/2/4` worker status, metrics, drawings, hashes, order, cancellation, and partial reports. Recommended model: gpt-5.6-sol | Reasoning effort: high.

Repeated dataset inspection, `short_pin` auditing, recipe tuning, and LLM validation are not active priorities and require a new explicit user request.

## Historical Next Priority Order (superseded by P148)

1. **Classify and split the user-supplied Pin_1 corpus before recipe changes.**
   - Prerequisite: operator-approved inspection target/ROI and Good/NG acceptance definition, plus an explicit Train/Validation/Test split. Keep P147's 500-row result as an immutable baseline; do not tune and re-score the same undifferentiated set. Recommended model: 해당 없음 (operator/data decision required) | Reasoning effort: 해당 없음.

2. **Acquire and approve real captured Die Pad Good/NG variation before further algorithm work.**
   - Prerequisite: multiple real captured Good and defect images under representative pose, illumination, focus, and part variation, plus operator-approved labels and inspection region. P144 already passes the synthetic Train/Validation/frozen-Test contract; do not spend model tokens retuning it against the same corpus or present it as field-ready. Recommended model: 해당 없음 (데이터 확보 전 모델 작업 불필요) | Reasoning effort: 해당 없음.

3. **Resume non-GPT provider correction coverage only after the provider is usable and the user authorizes it.**
   - Prerequisite: Gemini must recover from the P140 no-response state after the user's required pause, or Claude access must be explicitly resumed. Use public samples only and preserve the first response before validation. No model recommendation until that condition exists.

4. **Test an installer/update path only when installation behavior becomes an explicit product requirement.**
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
