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
- For the first implementation phase, build the Threshold Learn topic as a separate Learn screen with a table of contents, GV/Threshold/Binary/BinaryInv/MaxValue explanations, sandbox animation, and an explicit apply-to-tool action that changes only the tool parameters.

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

## Goal-Driven Execution

- Convert broad requests into concrete success criteria.
- Work toward passing checks and preserving stable behavior, not toward producing a long explanation.
- Keep reports grounded in changed files and command results.

## Reasoning Effort

- Use low reasoning effort for simple formatting, documentation-only edits, and narrow bug fixes.
- Use higher reasoning effort for architecture, MVVM refactors, docking behavior, sample catalog design, performance issues, and cross-module changes.
- Increase verification rigor as the blast radius grows.
