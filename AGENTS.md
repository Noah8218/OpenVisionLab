# AGENTS.md

This file defines the working agreement for Codex in this repository.

## Work Location

- Primary implementation and verification work starts in `C:\Git\OpenVisionLab_Dev`.
- `C:\Git\OpenVisionLab` is the original OpenVisionLab repository that receives reviewed, stabilized changes from Dev.
- Do not bulk-copy Dev over the original repository. Move changes by reviewed patch, cherry-pick, or import.
- Do not run `git push` unless the user explicitly requests `PUSH`.

## Product Identity

- OpenVisionLab is an OpenCvSharp4-based rule-based vision recipe workbench. Rule-based teaching, execution, drawings, and repeatable validation are the product core.
- LLM support is an optional, maintenance-mode XML authoring aid. It is not the primary product direction and must not be a prerequisite for operating the workbench.
- It is for learning, verifying, and composing image-processing inspection recipes with tools such as Threshold, Blob, Contour, Line/Length, Matching, EdgeBasedMatching, and Feature/Shape-style workflows.
- The main workflow is sample image -> direct PropertyGrid teaching and Pipeline composition -> explicit Preview/Run -> layer/result comparison -> N-sample validation -> saved recipe. Existing LLM XML draft/validation/import may optionally accelerate the composition step.
- It is not a camera, lighting, PLC, or I/O integration platform.

## Localized User Manual Contract

- The application `Guide` command must select the offline manual from
  `OpenVisionLanguageService.CurrentLanguage`: Korean UI opens only the Korean
  manual and English UI opens only the English manual.
- Do not silently fall back to a manual in a different language. A missing,
  damaged, duplicated, or hash-mismatched selected-language entry must fail
  closed with the localized Guide error.
- Package all supported manual languages beside the EXE under `Guide`, and
  validate each exact language/file/SHA-256 mapping through
  `Guide\guide-manifest.json` before opening it.
- Keep Korean and English manuals structurally equivalent: the same supported
  workflow chapters, Tool coverage, reference scope, numbered callouts, and
  explicit Preview/Run contracts. A language-specific manual must use current
  UI captures from that same application language wherever UI text is visible.
- A language change or Guide open must not run Preview/Run, create/delete/select
  layers, change the active layer, or mutate Pipeline routing.
- Verify both language selections from the current build and a copied clean
  runtime. Check exact HTML language markers, manifest hashes, missing-language
  failure, and that switching Korean -> English -> Korean changes only the
  selected manual path and localized presentation state.

## User-Centered Workflow And Persisted Setup

- Design each feature from the operator's goal and the shortest safe normal
  workflow, not from internal screen, component, class, or storage boundaries.
- When one task requires related settings across several views, dialogs, or
  buttons, treat that friction as a design problem. Consolidate settings that
  belong to one durable workflow into one coherent first-use option or setup
  surface.
- Persist reusable setup only after explicit operator confirmation and at the
  narrowest correct Tool, Recipe, project, workspace, or user scope. Restore
  it on the next equivalent use so the operator does not repeat the same setup.
- Keep restored values visible and editable, provide an explicit reset/default
  path, validate them before reuse, and explain stale or incompatible state.
- Do not silently share task-specific settings across unrelated Recipes,
  projects, workspaces, or users. Do not remember destructive,
  security-sensitive, or safety-critical choices implicitly.
- Restoring configuration must not execute Preview/Run, create/delete/select
  layers, change the active layer, or mutate Pipeline routing.
- Verify reusable setup through save, close/reload/reopen, exact restoration,
  visible reset, stale-state rejection where applicable, and zero unintended
  execution/layer/routing side effects.
- Use
  `docs/reports/OPENVISIONLAB_USER_CENTERED_WORKFLOW_DIRECTION_20260729.md`
  as the reusable admission template and future development direction.

## LLM Maintenance Mode And Preserved Evidence

- OpenVisionLab is a deterministic rule-based vision recipe workbench. LLM support is an optional maintenance-mode XML authoring aid and must not be required for normal operation.
- Do not start provider integrations, consumer-web automation, new prompt families, inspection-intent skills, transcript campaigns, or repeated dataset tuning unless the user explicitly reopens a named task.
- Preserve existing LLM Assistant, Guided Setup, XML guide/catalog, validation/import gates, and recorded correction evidence. Fix only a reproduced regression, data-loss risk, invalid XML acceptance, or compatibility break inside that frozen scope.
- Provider transport remains optional. Manual prompt copy/XML paste may remain available; API credentials and consumer-web automation must not become product dependencies.
- Do not use an LLM as a per-image production detector, tune a recipe per image, weaken frozen gates to raise coverage, or infer semantic correctness from execution count alone.
- Keep `OuterCornerIntersection` experimental and outside default recommendations until independent physical-boundary evidence qualifies it.
- For an explicitly authorized image-validation task, freeze the recipe and corpus first, retain drawings/metrics/hashes/fail-closed reasons, use the deterministic review queue, bound correction cycles, and reserve held-out data until the candidate is frozen.
- Historical P-number decisions and scoped evidence belong in `docs/reports`, `docs/admin/OPENVISIONLAB_NEXT_SESSION_HANDOFF.md`, and the archived handoff. Find them through `docs/LLM_DOCUMENT_INDEX.json`; do not duplicate them in this file.
- Live product status and the absence or presence of an active priority belong only to `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`.

## Rule-Based-First Development Order

1. Do not resume repeated inspection or LLM validation until the user explicitly names and authorizes that validation task.
2. Prefer src/OpenVisionLab/UI/result improvements over new algorithms: expose per-object results, click-to-highlight evidence, and clear accepted/rejected reasons from existing tools first.
3. Make the existing deterministic `Matching -> NormalizeImage -> reference-coordinate ROI -> inspection` path fully teachable and reviewable without LLM assistance through one visual fixture/relative-ROI workflow.
4. Add a new algorithm family only after existing tools cannot express an approved operator intent and the PropertyGrid inputs, result model, drawings, metrics, and failure contract are defined first.
5. When the user later authorizes algorithm validation, use a frozen recipe and bounded evidence; do not reopen already completed campaigns merely to accumulate execution count.

## OpenVisionLab Learn Mode Direction

- Prioritize making OpenVisionLab usable as a rule-based vision workbench without LLM assistance. LLM support is optional assistance; the core program must teach and run the existing tools clearly on its own.
- Keep algorithm Tool Views as working editors. Do not turn Threshold, Blob, Contour, LineDistance, Matching, or other Tool Views into long textbook pages.
- Put tool learning content in a separate Learn surface, tab, option, or window. Tool Views may expose only a compact `Learn` entry point that opens the relevant Learn topic.
- Structure Learn content around OpenCvSharp concepts that explain OpenVisionLab's actual tools and workflows. Include operator-facing basics such as coordinate systems, `Point`, `Size`, `Rect`, `RotatedRect`, `Mat`, pixel/channel values, matrix-style image storage, ROI slicing, and how those concepts appear in PropertyGrid parameters and result metrics.
- Do not build OpenCV installation, camera/video capture, generic file I/O, event handling, machine learning, DNN, or deployment chapters unless the product direction explicitly changes.
- Organize the separate Learn surface like a machine-vision curriculum, but rewrite the outline for OpenVisionLab instead of copying a book table of contents. The intended chapter flow is OpenCvSharp/image basics -> Point/Rect/Mat/ROI/layers -> brightness/contrast/histogram -> arithmetic/logical operations -> filtering -> geometry transforms -> edge/line -> color/HSV -> Threshold/Morphology -> Blob/Contour -> Matching/EdgeBasedMatching/FeatureMatching -> pipeline/layer routing -> metrics/Good-Bad validation -> LLM XML authoring.
- The Learn roadmap should cover the useful equivalent of OpenCV learning chapters 5-14 first: brightness/contrast/histogram, arithmetic/logical operations, filtering, geometry transforms, edge/Hough-style line concepts where supported, color/HSV, threshold/morphology, labeling/blob/contour, template/object matching, and feature matching. If a chapter has no current OpenVisionLab tool, record the gap and add a PropertyGrid-based tool only after defining the operator workflow, parameters, metrics, samples, and smoke evidence.
- Each Learn topic should connect concept -> visual explanation or animation -> sample image/recipe -> relevant Tool View entry -> explicit Preview/Run or validation step. Learn interactions must not auto-run Preview/Run, create layers, change routing, or modify recipe values unless the user explicitly clicks an apply/open/run action.
- Keep engineering contracts such as no-auto-run rules, routing invariants, smoke/readiness evidence, scope exclusions, and backlog state in `AGENTS.md`, engineering documents, and regression checks. Do not expose those developer/user working agreements as learner-facing copy. Learn UI and `docs/learn` content must instead explain positively what concept is being learned, what action the operator should take, and what image, layer, parameter, or metric should be compared.
- For the first implementation phase, build the Threshold Learn topic as a separate Learn screen with a table of contents, GV/Threshold/Binary/BinaryInv/MaxValue explanations, sandbox animation, and an explicit apply-to-tool action that changes only the tool parameters.

## Recipe, Pipeline, And Manager Responsibilities

- Keep the responsibility boundary explicit: Tool Views configure one algorithm and add a Step; Pipeline owns Step order, input/output layer routing, acceptance gates, and explicit Preview/Run; Pipeline Review owns Step/result/failure analysis; Recipe groups reusable Pipeline and sample-validation references; Recipe Manager owns recipe library and lifecycle operations.
- Recipe Manager must not become a second Pipeline editor or an always-visible container for every XML, Step, report, history, LLM, and debug surface.
- The default Recipe Manager view should be a compact recipe summary: recipe identity, active Pipeline, Pipeline/Step count, XML readiness, current work sample, recipe-specific current check status, and a direct entry to the existing Pipeline Review.
- Keep Guided Setup, detailed Pipeline review, LLM XML, raw XML/Step, branch/output comparison, report/history, import/export, and review bundles available through an explicit advanced-review mode instead of giving them equal prominence on first entry.
- Treat Recipe Manager summary and advanced review as separate workspace states. Summary shows recipe search/library, one selected-recipe overview, and lifecycle commands. Advanced review hides the outer recipe library/search and create/duplicate/rename/delete controls, uses the detail area at full width, opens on Pipeline review, and provides an explicit return to summary. Do not restore the previous additive layout where advanced controls were layered on top of the library screen.
- Direct and screenshot smoke runs must clean up their reserved `Smoke_<scenario>_<12 hex>` recipe workspaces. Internal smoke recipes must not accumulate in or be presented as the operator's recipe library; cleanup must match a reserved prefix and exact generated suffix rather than deleting arbitrary user recipes.
- Opening Recipe Manager, switching basic/advanced review, selecting a recipe, or opening Pipeline Review must not run Preview/Run, create layers, or change input/output routing.
- Keep the novice round trip explicit and reversible: Recipe Manager summary -> Open Pipeline -> explicit Run Review -> Return to Recipe. Pipeline Review must show the owning recipe, and Return to Recipe must reopen that same recipe summary without rerunning, creating/removing layers, changing the active layer, or changing recipe/pipeline routing.
- Keep workspace sample selection separate from recipe validation evidence. An automatically selected catalog sample may be shown as the current work sample, but it must not appear as a result for the selected recipe/pipeline until that same recipe/pipeline has actually run the sample check.
- Learn teaches concepts, Tool Views tune algorithms, Pipeline composes and executes the inspection, Pipeline Review explains evidence, and Recipe Manager organizes reusable recipe units. Do not blur these roles in learner-facing copy or navigation.

## Project Orientation and Status Review

At the start of a new OpenVisionLab chat, after a handoff, or whenever the user asks to continue project work, do not jump directly into narrow code or UI fixes. First rebuild the product context from current evidence.

- Work in `C:\Git\OpenVisionLab_Dev`.
- Run `git status --short` and `git log --oneline -5` before interpreting the current state.
- Start from the LLM-oriented document entrypoint and its machine-readable routes:
  - `AGENTS.md`
  - `docs\README.md`
  - `docs\LLM_DOCUMENT_INDEX.json`
  - `docs\admin\OPENVISIONLAB_CURRENT_HANDOFF.md`
  - `docs\roadmap\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`
  - `docs\contracts\openvisionlab\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`
- Use the matching `routes[].read` list in `docs\LLM_DOCUMENT_INDEX.json` for task-specific contracts, reports, policies, and historical evidence. Do not load the full chronological handoff or LLM documents when the current task does not involve them.
- When the user asks about current status, product direction, commercial comparison, or "what is next", also check the status/comparison history docs when present:
  - `docs\roadmap\OPENVISIONLAB_PRODUCT_IDENTITY_AND_ROADMAP.md`
  - `docs\roadmap\OPENVISIONLAB_STATUS_AND_NEXT_STEPS.md`
  - `docs\research\OPENVISIONLAB_UX_COMPETITOR_REVIEW_20260701.md`
  - `docs\research\OPENVISIONLAB_COMPETITOR_PRIORITY_REVIEW_20260701.md`
  - `docs\reports\OPENVISIONLAB_SELF_EVALUATION_20260703.md`
- Use `docs\admin\OPENVISIONLAB_CURRENT_HANDOFF.md` as the current source of truth for live status, completed evidence, known gaps, and next priority. Use the product-target document and stable contracts for product/behavioral authority. Treat older readiness percentages and chronological handoff entries as historical or scoped evidence unless fresh code/tests/screenshots confirm them.
- Before selecting work, explicitly restate:
  - current product identity;
  - current maturity/completeness estimate and its source;
  - what commercial tools teach OpenVisionLab to emulate;
  - what commercial platform scope must remain out of scope;
  - the immediate next priority and the remaining project priority.
- Do not treat a narrow screenshot, smoke test, or UI issue review as a substitute for product status analysis.
- Do not invent LLM transcript evidence. If real API keys or manual transcripts are unavailable, say so and choose the next evidence-based priority.

## Stable Contracts

- Keep algorithm tools PropertyGrid-based.
- A model property object assigned to PropertyGrid `SelectedObject` should generate the parameter UI.
- Keep the WPG-CUSTOM-derived light PropertyGrid presentation as the default for algorithm Tool Views. Dark or otherwise host-specific windows must opt into an instance-scoped bridge theme variant; do not fork, duplicate, or globally replace the vendor DLL merely to recolor one host.
- Keep business logic out of View code-behind. Move it to ViewModel, Controller, Presenter, Behavior, Converter, Runtime, or Service classes.
- Creating an output layer must not automatically change the input layer.
- Boolean visibility toggles must not trigger Preview or Run.
- Layer create/delete/load-image actions must not auto-run tools.
- Pipeline Review must distinguish a genuinely absent input image from a downstream input that an earlier enabled Step will produce. Show the former as `입력 없음` and keep the latter in the explicit-run `WAIT` state; selecting either Step must not trigger Preview/Run.
- Run History batch analytics must derive from persisted per-sample elapsed values, keep correctness (`failure rate`) separate from performance (`average`, `median`, nearest-rank `p95`, `maximum`), and remain read-only. Do not claim per-Step batch analytics until the batch path persists and links structured Step run reports.
- Run History baseline timing may compare only runs with the same suite kind, suite name, and exact sample-image multiset, and only when every sample has a valid positive elapsed value. Outcome rows may still be compared independently; a different or incomplete set must show that performance comparison was skipped rather than imply a timing regression.
- Do not remove viewer zoom/pan/drag, ROI overlay, template editor, layer comparison, or docking features.
- Do not remove the main window title-bar minimize, maximize/restore, and close controls.
- Do not add `Dirkster.AvalonDock` directly to `src/OpenVisionLab/OpenVisionLab.csproj`; AvalonDock ownership belongs in `src\Libraries\OpenVisionLab.Docking.Controls`.
- Do not reintroduce SDK sample assets into public sample paths.
- Do not reintroduce `dll\Library-Noah` or `OpenCvSharp.Extensions.dll`; OpenVisionLab uses the manifest-verified `dll\OpenVisionLab-Vision-SDK` 3.0 managed DLLs and the single shared `dll\OpenCVSharp\OpenCvSharpExtern.dll`.

## Completion Means Commands Pass

Do not mark work complete by explanation alone. Completion requires command evidence.

For OpenVisionLab changes, run the smallest meaningful set from the list below:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1
```

- If code tests, linters, or type checks exist for the touched area, run them.
- If a frontend, Python, Rust, or other subproject has its own checks, completion means its relevant command passes, for example `pnpm test`, `pnpm lint`, `pnpm typecheck`, `pytest`, or `cargo test`.
- If a required command is unavailable, report the exact command and the reason it could not run.
- Smoke tests that launch `OpenVisionLab.exe` must use the latest updated build output from the current workspace. Build first, or otherwise verify the EXE timestamp/path corresponds to the current source changes before capturing screenshots or reporting smoke results.
- Do not use old smoke artifacts, old screenshots, or old view captures as evidence for the current state. If an artifact was not generated in the current turn after the latest relevant build/source check, label it as historical or baseline only.
- For EXE smoke tests, prefer a fresh `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` first. `dotnet run --no-build --project src/OpenVisionLab/OpenVisionLab.csproj ...` is allowed only when that build already completed in the same turn and no source files changed afterward.
- For screenshot smoke tools that instantiate WPF views directly, run the tool from the current Dev workspace after the latest relevant source changes and report it as a current-source view capture, not as an EXE smoke. If the user asks for EXE evidence, launch the latest built EXE instead.
- Before showing any UI image in chat, verify the image path belongs to the current artifact folder, and that the folder was produced by the command just run for this task. Do not show images from earlier artifact folders unless explicitly marked as before/baseline evidence.
- src/OpenVisionLab/UI/UX changes require current-build before/after evidence. Do not reuse old screenshots.
- When reviewing UI screenshots, explicitly inspect visible controls for clipped text, clipped icons, hidden button content, combo box text visibility, input text visibility, and incoherent overlap.
- When src/OpenVisionLab/UI/UX work is done, render the relevant before/after screenshots directly in the chat whenever the chat surface supports local image display. Do not report only file paths.
- Image paths may be included as supporting evidence, but they do not replace direct in-chat image display.

## Algorithm Image Validation Evidence

- An algorithm image-validation claim is incomplete when it reports only CSV rows, metrics, PASS/NG counts, or elapsed time. It must also retain and show current-run visual evidence of what the algorithm selected.
- For every algorithm or recipe validation, generate overlays from the exact XML, parameters, and image execution being reported. The overlay must visibly distinguish the relevant ROI (when used), candidate or selected detection geometry, final point/line/contour/template/measurement marks, and the resulting PASS/NG or measurement context.
- Batch validation must retain representative current-run overlays: at minimum one ordinary successful sample and one meaningful boundary, shifted, difficult, or failed sample when the corpus contains one. When the validation concerns a reported extreme or outlier, show that exact row's overlay as well.
- Store the source image, rendered overlay, XML/recipe identity, and reported metrics together under the current task artifact folder. Before reporting, open or otherwise verify the produced images and confirm that the drawn geometry corresponds to the claimed detection; do not substitute manually drawn annotations for runtime output.
- In the final chat response, render the relevant current artifact overlay images directly, explain what each point/line/ROI represents, and state the input image and exact result being demonstrated. File paths alone are insufficient. If a visual cannot be produced, mark the image-validation claim incomplete and explain the concrete limitation.

## Video-Gated Operator Development

- CVR-00 remains an external human-validation task and may be marked complete only after at least three independent first-time participants and their unedited observations exist. An agent-operated recording must never be described as novice-user proof.
- The absence of those participants is not a general development blocker. The user has explicitly authorized bounded operator-UX development from reproducible current-build evidence while CVR-00 remains deferred.
- For each visible workflow slice admitted through this mode, freeze one concrete operator task and acceptance criteria, capture a fresh actual-EXE before video when the prior state can be reproduced safely, implement only the observed bounded correction, then run automated checks and record the same actual-EXE task after the change.
- Review the videos or exact extracted frames for action order, visible state, result interpretation, accidental Preview/Run, layer creation/deletion/selection, active-layer changes, and route changes. Retain the EXE identity, timeline, before/after media, commands, and comparison in one task-local evidence folder.
- A video-gated UX result proves only the recorded workflow and visible contract. Algorithm correctness still requires exact runtime drawings, metrics, gates, and proportionate N-sample or held-out evidence under the Algorithm Image Validation Evidence contract.
- Do not turn every hesitation into a feature. Admit a change only for a reproducible blocker, misleading state, unsafe action, or user-approved workflow friction; preserve an explicit reset/default and persisted-setup rules when reusable configuration is involved.

## Priority Direction

- Prefer large, user-visible product improvements before minor polish when the user asks to continue priorities.
- Prioritize complete workflow upgrades such as recipe/sample review, pipeline operator review, validation summaries, and tool runtime structure before small label, spacing, or wording tweaks.
- Keep small UI polish scoped to the large workflow currently being improved instead of spending cycles on isolated cosmetic fixes first.
- After orientation or handoff review, select the next priority from the current evidence and tell the user before any implementation, documentation edit, or command-driven follow-up work. Do not skip this for small or specific follow-up requests; if the user gives a specific task, state that task as the immediate priority and also name the remaining project priority.
- For the current product direction, the default next-priority order is:
  1. P256 closes the bounded four-Step `Filter -> Threshold -> Morphology -> Blob -> restart -> explicit Run Review` route-clarity walkthrough with no reproduced product blocker. Do not admit another feature from this chain; wait for a named operator task or verified current-build regression. Recommended model: none until evidence exists | Reasoning effort: none until evidence exists.
  2. Keep CVR-00 incomplete and deferred until three independent first-time participants are available. Use their raw observations later to confirm or overturn the video-gated assumptions; do not retroactively call agent recordings participant evidence. Recommended model: none until observations exist | Reasoning effort: none until observations exist.
  3. Only after the operator reports a measured sequential bottleneck and explicitly requests parallelism, audit isolated-worker equivalence and thread safety before offering bounded parallel image execution. Do not implement concurrency merely because sequential N-image execution exists. Recommended model: gpt-5.6-sol | Reasoning effort: high.
- Repeated image inspection, dataset tuning, `short_pin` audit, and LLM validation are closed as active priorities and require a new explicit user request.

## No Guessing

- Do not assume behavior, file ownership, or current state when it can be checked.
- If unsure, open the relevant file, test, log, screenshot, or command output and cite that evidence in the response.
- If evidence conflicts, state the conflict instead of smoothing it over.

## Think Before Coding

Before editing:

- State the concrete goal in executable terms, for example "make this smoke pass" instead of "improve the feature".
- Identify assumptions.
- If an assumption materially changes the implementation and cannot be verified, ask the user.
- If the task becomes confused or contradictory, stop and re-orient before editing.

## Simplicity First

- Prefer the simplest change that satisfies the requested behavior and can be verified.
- Do not add unrequested features, abstractions, fallback paths, or broad error handling.
- Add abstractions only when they reduce real duplication or match an established local pattern.

## Surgical Changes

- Change only the files and behavior needed for the request.
- Do not reformat unrelated files.
- Do not revert unrelated dirty files.
- Preserve existing public contracts unless the user explicitly asks to change them.

## Source Organization And Folder Rules

- Treat folder placement as an ownership signal, not as cosmetic sorting. A file moves only when its runtime responsibility is clear and the move makes the next change easier to find.
- Use this current layout incrementally. Do not create empty folder trees merely to mirror it:
  - `src\OpenVisionLab\Core\State`: application, recipe, data, and system state objects plus runtime context.
  - `src\OpenVisionLab\Core\Recipe`: recipe workspace and recipe persistence services.
  - `src\OpenVisionLab\Core\Display`: display-layer state, snapshots, history, synchronization, and presenters.
  - `src\OpenVisionLab\Core\Pipeline\Definition`: pipeline model normalization, step construction, parameter schemas, and tool factories.
  - `src\OpenVisionLab\Core\Pipeline\Execution`: pipeline execution, fixtures, result summaries, reports, and runtime notifications.
  - `src\OpenVisionLab\Core\Pipeline\Validation`: known metrics, metric enrichment, diagnostic rules, and validation.
  - `src\OpenVisionLab\Core\Pipeline\Storage`: pipeline manifests, sample sets, run reports, and batch storage.
  - `src\OpenVisionLab\Core\Pipeline\Tools`: non-WPF algorithm adapters owned by the pipeline runtime.
  - `src\OpenVisionLab\UI\Menu\Wpf\Shell\Chrome`, `Commands`, `Layers`, `Session`, `Tooling`, and `Workspace`: main-shell chrome, command routing, layer workspace, session state, tool-window orchestration, and image-workspace behavior respectively. Put hosted-document lifetime in `Shell\Documents`, host-wide display state in `Shell\State`, recipe navigation in `Shell\Recipe`, and lifecycle/test adapters in `Shell\Support`.
  - Keep the five `OpenVisionShellHostDockedLayerOrchestrator` partial files together in `Shell\Layers\Orchestration`; do not split a partial type across ownership folders.
  - `src\OpenVisionLab\UI\Menu\Wpf\Recipe\Context`, `IntentSkills`, `Models`, `Review`, and `Validation`: current recipe context, deterministic recipe starters plus standalone LLM prompt/intent/correction-packet contracts and Guided Setup required-input/readiness presentation, presentation/report DTOs, recipe review/export plus LLM dependency scan/copy execution and pure LLM draft/variant comparison, selected-step/branch-output review, Good/Bad sample-matrix presentation, local validation-set/dashboard and Validation Suite summary presentation, Guided Setup intent latest-run/calibration feedback, guided-workflow next-action, and recipe/pipeline lifecycle validation presentation, operator run-review/next-action, Run History filter/baseline/comparison/performance presentation, decision-board, and handoff presentation, and local validation-set persistence plus pure LLM XML draft validation rules, stored-pipeline XML report composition, and request/result orchestration.
  - `src\OpenVisionLab\UI\Menu\Wpf\PipelineReview`: Pipeline Review presenters, readiness state, ViewModel, View, document ownership, and the explicit `Execution` controller/result contracts. Keep pipeline execution, review-only result caches, display-layer execution context construction, and result-image disposal in `PipelineReview\Execution`; keep View event wiring, selected-Step navigation, and rendered text/image presentation in the document/ViewModel/presenters.
  - `src\OpenVisionLab\UI\Menu\Wpf\Workspace`: sample-picker, Learn-document, sample-pair, and catalog-focus support.
  - `src\OpenVisionLab\UI\Menu\Wpf\NativeTools`: native Tool View document, preview, route, PropertyGrid, session, registry, and prewarm support.
  - `src\OpenVisionLab\UI\Menu\Wpf\Docking`, `Viewer`, and `Windows`: docked-layer runtime, viewer-specific support, and reusable floating/title-bar window behavior. Put docking interfaces in `Docking\Contracts` and docking-only smoke/test facades in `Docking\TestSupport`.
  - `src\OpenVisionLab\UI\Menu\Wpf\Views`, `ViewModels`, and `Documents`: only generic or shared visual artifacts that do not have a stronger domain owner.
  - `src\OpenVisionLab\UI\VisionTest\Composition`, `Contracts`, `Services`, and `ViewModels`: shared Tool View composition, bridge contracts, non-visual tool support, and tool-facing view models. Put pipeline sample catalog/storage and sample execution in `src\OpenVisionLab\Core\Pipeline\Storage` and `src\OpenVisionLab\Core\Pipeline\Execution`, not in the Vision Test UI root.
  - `src\OpenVisionLab\UI\VisionTest\Wpf\Tooling\Contracts`, `SingleInput`, `DoubleInput`, `Preview`, `PropertyGrid`, `Presentation`, `Review`, `Presets`, `Layers`, and `Interaction`: Tool View interfaces; single- and double-input controller/runtime/binder/shell families; shared preview, PropertyGrid, presentation/theme, result-review, preset, layer, and explicit user-action support. Keep each input-family controller/runtime/view-base chain together.
  - `src\OpenVisionLab\UI\VisionTest\Wpf\ToolViews`: concrete algorithm Tool View XAML/code-behind pairs. `src\OpenVisionLab\UI\VisionTest\Wpf\Learn`: Learn topic catalog, Learn window XAML/code-behind, and Tool-to-Learn window controllers. Keep existing `Behaviors` and `ViewModels` folders as their current explicit owners.
- Keep `src\OpenVisionLab\Core` independent of WPF presentation types for new work. A legacy compatibility dependency must not be copied into a new Core service; put presentation adaptation in WPF or a dedicated adapter instead.
- Keep algorithm parameter objects and PropertyGrid ownership with the relevant tool/runtime. Do not create a generic `Common`, `Utils`, `Helpers`, or `Legacy` folder as a dumping ground.
- Put top-level recipe validation, review, sample-run, and batch-result DTOs in `Wpf\Recipe\Models`. Keep command execution, callbacks, selection changes, and ViewModel state coordination in the command surface or a named Controller/Presenter; a DTO must not become a second command surface.
- A View code-behind may own visual lifecycle, control wiring, and framework-only behavior. Move command decisions, text derivation, file access, validation, and pipeline state changes to a ViewModel, Presenter, Controller, Runtime, or Service with a named responsibility.
- File length is a review signal, not an automatic split. Split a file only when it contains independently testable responsibilities, repeated state derivation, or an existing natural service/presenter boundary; do not split only to reduce line count.
- Treat the current `OpenVisionShellHostRecipeCommandSurface.*` and `VisionPipelineStepPropertyMapper.*` partial files as a temporary responsibility inventory, not completed MVVM architecture. Do not add another production partial to either family merely to reduce line count.
- For the next MVVM slice, first identify and extract a real owner: ViewModel for binding state and commands, Presenter for display derivation, Controller/Coordinator for UI workflow coordination, or UseCase/Application Service for recipe/pipeline operations. The extracted owner must receive explicit inputs/dependencies, return an explicit result or state change, avoid unrelated private state from the former owner, and have a focused test seam.
- A partial-only move may be reported only as a temporary readability step with its exit criterion; it must not be reported as a completed structural/MVVM refactor. Complete such a refactor only when the former owner no longer owns the behavior and focused evidence proves the new owner.
- Move one clean cohesive file group at a time. Do not combine physical moves with behavior changes, namespace rewrites, formatting sweeps, or unrelated refactors. Do not move a file that is already dirty unless the user explicitly asks to include that work.
- Preserve namespaces during a physical-only move unless a namespace change is required for correctness and reviewed as a separate behavior-neutral step. For XAML, preserve `x:Class`, resource, and automation contracts.
- Move an XAML file and its code-behind together. Do not physically move a large dirty partial class, especially `OpenVisionShellHostView` or `OpenVisionShellHostRecipeCommandSurface`, merely to make the tree look tidier; first establish a real Presenter, Controller, or ViewModel boundary.
- Do not add new production `.cs` files directly under `src\OpenVisionLab\UI\Menu`; place them under `Wpf` by responsibility. The `Wpf` root may temporarily retain an XAML/code-behind pair, a dirty file, or an unassigned new file until its natural owner is verified; record that exception in the handoff instead of forcing a speculative move.
- The `src\OpenVisionLab\UI\Menu\Wpf` root is an explicit temporary Shell composition boundary only for `OpenVisionShellHostView.xaml`, its code-behind, and `OpenVisionShellHostRecipeCommandSurface.cs`. Before moving either large Host file, extract a cohesive Presenter, Controller, ViewModel, or static helper with a focused test; do not perform a cosmetic move of those files.
- When a source-check tool names a moved file path, update the check to the new explicit owner path in the same change. Do not add an unbounded file-search fallback that would conceal a misplaced source file.
- Before a move, confirm the project includes the destination path and that the source files are tracked and clean. After each move group, run the smallest meaningful build and affected smoke/check; use full build and readiness checks when the group crosses `src/OpenVisionLab/Core` and `src/OpenVisionLab/UI` boundaries.
- New folders must have at least two coherent files or a clear near-term owner. A single exceptional file may stay at the current level until its companion responsibility exists.

## Clean Runtime Evidence Contract (2026-07-19)

- Use `tools\BuildCleanRuntime.ps1 -Mode Dev` for current Dev EXE evidence. It creates a new timestamped runtime under `artifacts\openvisionlab_clean_runtime_<timestamp>` and refuses to overwrite an existing directory.
- Use `tools\BuildCleanRuntime.ps1 -Mode Release` for the release package. It publishes a new `Release` runtime only to `dist\OpenVisionLab` and refuses to overwrite an existing package directory.
- The retained `bin\Debug` directory is a local recipe workspace, not current EXE or release evidence. Do not delete, move, migrate, or use it for current-runtime claims unless the user explicitly changes this retention decision.
- The output contract proves a clean runtime can execute the tested XML/workbench flows. It does not by itself establish portable LLM template-dependency paths, installer readiness, or deployment qualification.

## Goal-Driven Execution

- Convert broad requests into concrete success criteria.
- Work toward passing checks and preserving stable behavior, not toward producing a long explanation.
- Keep reports grounded in changed files and command results.

## Reasoning Effort

- Use low reasoning effort for simple formatting, documentation-only edits, and narrow bug fixes.
- Use higher reasoning effort for architecture, MVVM refactors, docking behavior, sample catalog design, performance issues, and cross-module changes.
- Increase verification rigor as the blast radius grows.

## Public README Editorial Contract

- Keep the GitHub README focused on user-visible capabilities, workflows, setup, examples, and validation.
- Do not add camera, lighting, PLC, industrial I/O, account, or deployment non-goal boilerplate to the public README. Those boundaries are an internal product-development agreement, not required introductory material for users.
- Keep scope boundaries in internal project contracts, handoffs, and planning documents.
- Do not reintroduce this exclusion language unless the user explicitly reverses this decision.
- After changing the README, search the full file for equivalent exclusion wording rather than checking only one sentence.
- Canonical decision record: `docs/admin/OPENVISIONLAB_PUBLIC_README_EDITORIAL_POLICY.md`.
