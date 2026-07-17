# AGENTS.md

This file defines the working agreement for Codex in this repository.

## Work Location

- Primary implementation and verification work starts in `C:\Git\OpenVisionLab_Dev`.
- `C:\Git\OpenVisionLab` is the original OpenVisionLab repository that receives reviewed, stabilized changes from Dev.
- Do not bulk-copy Dev over the original repository. Move changes by reviewed patch, cherry-pick, or import.
- Do not run `git push` unless the user explicitly requests `PUSH`.

## Product Identity

- OpenVisionLab is an LLM-assisted OpenCvSharp4-based rule-based vision recipe workbench.
- It is for learning, verifying, and composing image-processing inspection recipes with tools such as Threshold, Blob, Contour, Line/Length, Matching, EdgeBasedMatching, and Feature/Shape-style workflows.
- The main workflow is sample image and operator intent -> LLM XML draft -> validation/correction/import -> explicit Preview/Run -> layer/result comparison -> saved recipe.
- It is not a camera, lighting, PLC, or I/O integration platform.

## LLM-Assisted Recipe Skill Direction

- Do not position OpenVisionLab as a one-shot "LLM looks at an image and creates the correct rule-based inspection automatically" product. That expectation is currently inaccurate and leads to weak product decisions.
- Treat LLM support as a guided recipe authoring assistant. The human operator provides inspection intent, target ROI/measurement region, OK/NG tolerance, and sample evidence; OpenVisionLab constrains tool choice, generates or validates XML, runs explicit Preview/Run checks, and exposes correction evidence.
- Develop LLM workflows as reusable inspection-intent skills inside OpenVisionLab: intent -> required user inputs -> locked tool family -> XML starter -> required metrics -> acceptance gates -> correction-loop evidence.
- These are OpenVisionLab recipe-wizard/intent-template skills, similar in spirit to a Codex skill such as Ponytail, but they are product features and documentation contracts, not external Codex plugins or global agent skills unless the user explicitly asks to create those.
- For measurement intents such as pin gap, pitch, width, or clearance, do not rely on average distance alone. Require consistency/outlier gates such as `DistancePxRange`, `DistanceMmRange`, `DistancePxMax`, or `DistanceMmMax` so a visually wrong long measurement line cannot pass through `DistancePxAvg` or `DistanceMmAvg`.
- When continuing this direction, prefer building the next concrete inspection-intent wizard or template only after identifying the user-visible workflow, required inputs, generated tool family, validation metrics, and smoke evidence.

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
- Read the current handoff and contract documents before choosing the next task:
  - `AGENTS.md`
  - `docs\OPENVISIONLAB_NEXT_CHAT_HANDOFF_PROMPT_20260706.md`
  - `docs\OPENVISIONLAB_NEXT_SESSION_HANDOFF.md`
  - `docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`
  - `docs\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`
  - `docs\OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md`
  - `docs\OPENVISIONLAB_LLM_TOOL_CATALOG.json`
  - `docs\OPENVISIONLAB_PUBLIC_SAMPLE_ASSET_POLICY.md`
  - `docs\OPENVISIONLAB_EXTERNAL_REFERENCE_POLICY.md`
  - `docs\OPENVISIONLAB_RELEASE_VERSION_POLICY.md`
- When the user asks about current status, product direction, commercial comparison, or "what is next", also check the status/comparison history docs when present:
  - `docs\OPENVISIONLAB_PRODUCT_IDENTITY_AND_ROADMAP.md`
  - `docs\OPENVISIONLAB_STATUS_AND_NEXT_STEPS.md`
  - `docs\OPENVISIONLAB_UX_COMPETITOR_REVIEW_20260701.md`
  - `docs\OPENVISIONLAB_COMPETITOR_PRIORITY_REVIEW_20260701.md`
  - `docs\OPENVISIONLAB_SELF_EVALUATION_20260703.md`
- Use the freshest product-target/status document as the current source of truth. Treat older readiness percentages as historical or scoped estimates unless the latest docs confirm them.
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
- Keep business logic out of View code-behind. Move it to ViewModel, Controller, Presenter, Behavior, Converter, Runtime, or Service classes.
- Creating an output layer must not automatically change the input layer.
- Boolean visibility toggles must not trigger Preview or Run.
- Layer create/delete/load-image actions must not auto-run tools.
- Pipeline Review must distinguish a genuinely absent input image from a downstream input that an earlier enabled Step will produce. Show the former as `입력 없음` and keep the latter in the explicit-run `WAIT` state; selecting either Step must not trigger Preview/Run.
- Run History batch analytics must derive from persisted per-sample elapsed values, keep correctness (`failure rate`) separate from performance (`average`, `median`, nearest-rank `p95`, `maximum`), and remain read-only. Do not claim per-Step batch analytics until the batch path persists and links structured Step run reports.
- Run History baseline timing may compare only runs with the same suite kind, suite name, and exact sample-image multiset, and only when every sample has a valid positive elapsed value. Outcome rows may still be compared independently; a different or incomplete set must show that performance comparison was skipped rather than imply a timing regression.
- Do not remove viewer zoom/pan/drag, ROI overlay, template editor, layer comparison, or docking features.
- Do not remove the main window title-bar minimize, maximize/restore, and close controls.
- Do not add `Dirkster.AvalonDock` directly to `OpenVisionLab.csproj`; AvalonDock ownership belongs in `Library\OpenVisionLab.Docking.Controls`.
- Do not reintroduce SDK sample assets into public sample paths.
- Do not reintroduce `dll\Library-Noah\OpenCvSharpExtern.dll`.

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
- For EXE smoke tests, prefer a fresh `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` first. `dotnet run --no-build --project OpenVisionLab.csproj ...` is allowed only when that build already completed in the same turn and no source files changed afterward.
- For screenshot smoke tools that instantiate WPF views directly, run the tool from the current Dev workspace after the latest relevant source changes and report it as a current-source view capture, not as an EXE smoke. If the user asks for EXE evidence, launch the latest built EXE instead.
- Before showing any UI image in chat, verify the image path belongs to the current artifact folder, and that the folder was produced by the command just run for this task. Do not show images from earlier artifact folders unless explicitly marked as before/baseline evidence.
- UI/UX changes require current-build before/after evidence. Do not reuse old screenshots.
- When reviewing UI screenshots, explicitly inspect visible controls for clipped text, clipped icons, hidden button content, combo box text visibility, input text visibility, and incoherent overlap.
- When UI/UX work is done, render the relevant before/after screenshots directly in the chat whenever the chat surface supports local image display. Do not report only file paths.
- Image paths may be included as supporting evidence, but they do not replace direct in-chat image display.

## Priority Direction

- Prefer large, user-visible product improvements before minor polish when the user asks to continue priorities.
- Prioritize complete workflow upgrades such as recipe/sample review, pipeline operator review, validation summaries, and tool runtime structure before small label, spacing, or wording tweaks.
- Keep small UI polish scoped to the large workflow currently being improved instead of spending cycles on isolated cosmetic fixes first.
- After orientation or handoff review, select the next priority from the current evidence and tell the user before any implementation, documentation edit, or command-driven follow-up work. Do not skip this for small or specific follow-up requests; if the user gives a specific task, state that task as the immediate priority and also name the remaining project priority.
- For the current product direction, the default next-priority order is:
  1. Prove the LLM-assisted recipe-workbench differentiator with real GPT/Gemini/Claude XML correction-loop transcripts when actual API keys or manual transcripts are available.
  2. If real transcripts are unavailable, inspect current-build Recipe Manager and LLM Assistant UX evidence and fix only visible clipping, overlap, or unclear next-action friction.
  3. Extend branch/output comparison only when a real multi-branch recipe exposes a gap beyond the existing smoke corpus.
  4. Clean Tool View code-behind only where the existing base/controller pattern naturally fits.

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
- Use this target layout incrementally. Do not create empty folder trees merely to mirror the target:
  - `1. Core\State`: application, recipe, data, and system state objects plus runtime context.
  - `1. Core\Recipe`: recipe workspace and recipe persistence services.
  - `1. Core\Display`: display-layer state, snapshots, history, synchronization, and presenters.
  - `1. Core\Pipeline\Definition`: pipeline model normalization, step construction, parameter schemas, and tool factories.
  - `1. Core\Pipeline\Execution`: pipeline execution, fixtures, result summaries, reports, and runtime notifications.
  - `1. Core\Pipeline\Validation`: known metrics, metric enrichment, diagnostic rules, and validation.
  - `1. Core\Pipeline\Storage`: pipeline manifests, sample sets, run reports, and batch storage.
  - `1. Core\Pipeline\Tools`: non-WPF algorithm adapters owned by the pipeline runtime.
  - `0. UI\0) MENU\Wpf\Shell\Chrome`, `Commands`, `Layers`, `Session`, `Tooling`, and `Workspace`: main-shell chrome, command routing, layer workspace, session state, tool-window orchestration, and image-workspace behavior respectively. Put hosted-document lifetime in `Shell\Documents`, host-wide display state in `Shell\State`, recipe navigation in `Shell\Recipe`, and lifecycle/test adapters in `Shell\Support`.
  - Keep the five `OpenVisionShellHostDockedLayerOrchestrator` partial files together in `Shell\Layers\Orchestration`; do not split a partial type across ownership folders.
  - `0. UI\0) MENU\Wpf\Recipe\Context`, `IntentSkills`, `Models`, `Review`, and `Validation`: current recipe context, deterministic recipe starters plus standalone LLM prompt/intent/correction-packet contracts and Guided Setup required-input/readiness presentation, presentation/report DTOs, recipe review/export plus LLM dependency scan/copy execution and pure LLM draft/variant comparison, selected-step/branch-output review, Good/Bad sample-matrix presentation, local validation-set/dashboard and Validation Suite summary presentation, Guided Setup intent latest-run/calibration feedback, guided-workflow next-action, and recipe/pipeline lifecycle validation presentation, operator run-review/next-action, Run History filter/baseline/comparison/performance presentation, decision-board, and handoff presentation, and local validation-set persistence plus pure LLM XML draft validation rules, stored-pipeline XML report composition, and request/result orchestration.
  - `0. UI\0) MENU\Wpf\PipelineReview`: Pipeline Review presenters, readiness state, ViewModel, View, document ownership, and the explicit `Execution` controller/result contracts. Keep pipeline execution, review-only result caches, display-layer execution context construction, and result-image disposal in `PipelineReview\Execution`; keep View event wiring, selected-Step navigation, and rendered text/image presentation in the document/ViewModel/presenters.
  - `0. UI\0) MENU\Wpf\Workspace`: sample-picker, Learn-document, sample-pair, and catalog-focus support.
  - `0. UI\0) MENU\Wpf\NativeTools`: native Tool View document, preview, route, PropertyGrid, session, registry, and prewarm support.
  - `0. UI\0) MENU\Wpf\Docking`, `Viewer`, and `Windows`: docked-layer runtime, viewer-specific support, and reusable floating/title-bar window behavior. Put docking interfaces in `Docking\Contracts` and docking-only smoke/test facades in `Docking\TestSupport`.
  - `0. UI\0) MENU\Wpf\Views`, `ViewModels`, and `Documents`: only generic or shared visual artifacts that do not have a stronger domain owner.
  - `0. UI\6) Vision Test\Composition`, `Contracts`, `Services`, and `ViewModels`: shared Tool View composition, bridge contracts, non-visual tool support, and tool-facing view models. Put pipeline sample catalog/storage and sample execution in `1. Core\Pipeline\Storage` and `1. Core\Pipeline\Execution`, not in the Vision Test UI root.
  - `0. UI\6) Vision Test\Wpf\Tooling\Contracts`, `SingleInput`, `DoubleInput`, `Preview`, `PropertyGrid`, `Presentation`, `Review`, `Presets`, `Layers`, and `Interaction`: Tool View interfaces; single- and double-input controller/runtime/binder/shell families; shared preview, PropertyGrid, presentation/theme, result-review, preset, layer, and explicit user-action support. Keep each input-family controller/runtime/view-base chain together.
  - `0. UI\6) Vision Test\Wpf\ToolViews`: concrete algorithm Tool View XAML/code-behind pairs. `0. UI\6) Vision Test\Wpf\Learn`: Learn topic catalog, Learn window XAML/code-behind, and Tool-to-Learn window controllers. Keep existing `Behaviors` and `ViewModels` folders as their current explicit owners.
- Keep `1. Core` independent of WPF presentation types for new work. A legacy compatibility dependency must not be copied into a new Core service; put presentation adaptation in WPF or a dedicated adapter instead.
- Keep algorithm parameter objects and PropertyGrid ownership with the relevant tool/runtime. Do not create a generic `Common`, `Utils`, `Helpers`, or `Legacy` folder as a dumping ground.
- Put top-level recipe validation, review, sample-run, and batch-result DTOs in `Wpf\Recipe\Models`. Keep command execution, callbacks, selection changes, and ViewModel state coordination in the command surface or a named Controller/Presenter; a DTO must not become a second command surface.
- A View code-behind may own visual lifecycle, control wiring, and framework-only behavior. Move command decisions, text derivation, file access, validation, and pipeline state changes to a ViewModel, Presenter, Controller, Runtime, or Service with a named responsibility.
- File length is a review signal, not an automatic split. Split a file only when it contains independently testable responsibilities, repeated state derivation, or an existing natural service/presenter boundary; do not split only to reduce line count.
- Move one clean cohesive file group at a time. Do not combine physical moves with behavior changes, namespace rewrites, formatting sweeps, or unrelated refactors. Do not move a file that is already dirty unless the user explicitly asks to include that work.
- Preserve namespaces during a physical-only move unless a namespace change is required for correctness and reviewed as a separate behavior-neutral step. For XAML, preserve `x:Class`, resource, and automation contracts.
- Move an XAML file and its code-behind together. Do not physically move a large dirty partial class, especially `OpenVisionShellHostView` or `OpenVisionShellHostRecipeCommandSurface`, merely to make the tree look tidier; first establish a real Presenter, Controller, or ViewModel boundary.
- Do not add new production `.cs` files directly under `0. UI\0) MENU`; place them under `Wpf` by responsibility. Leave only an explicitly retained legacy archive or OS metadata at that top level. The `Wpf` root may temporarily retain an XAML/code-behind pair, a dirty file, or an unassigned new file until its natural owner is verified; record that exception in the handoff instead of forcing a speculative move.
- The `0. UI\0) MENU\Wpf` root is an explicit temporary Shell composition boundary only for `OpenVisionShellHostView.xaml`, its code-behind, and `OpenVisionShellHostRecipeCommandSurface.cs`. Before moving either large Host file, extract a cohesive Presenter, Controller, ViewModel, or static helper with a focused test; do not perform a cosmetic move of those files.
- When a source-check tool names a moved file path, update the check to the new explicit owner path in the same change. Do not add an unbounded file-search fallback that would conceal a misplaced source file.
- Before a move, confirm the project includes the destination path and that the source files are tracked and clean. After each move group, run the smallest meaningful build and affected smoke/check; use full build and readiness checks when the group crosses Core/WPF boundaries.
- New folders must have at least two coherent files or a clear near-term owner. A single exceptional file may stay at the current level until its companion responsibility exists.

## Goal-Driven Execution

- Convert broad requests into concrete success criteria.
- Work toward passing checks and preserving stable behavior, not toward producing a long explanation.
- Keep reports grounded in changed files and command results.

## Reasoning Effort

- Use low reasoning effort for simple formatting, documentation-only edits, and narrow bug fixes.
- Use higher reasoning effort for architecture, MVVM refactors, docking behavior, sample catalog design, performance issues, and cross-module changes.
- Increase verification rigor as the blast radius grows.
