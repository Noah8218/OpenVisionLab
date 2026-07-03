# AGENTS.md

This file defines the working agreement for Codex in this repository.

## Work Location

- Primary implementation and verification work starts in `C:\Git\OpenVisionLab_Dev`.
- `C:\Git\OpenVisionLab` is the original OpenVisionLab repository that receives reviewed, stabilized changes from Dev.
- Do not bulk-copy Dev over the original repository. Move changes by reviewed patch, cherry-pick, or import.
- Do not run `git push` unless the user explicitly requests `PUSH`.

## Product Identity

- OpenVisionLab is an OpenCvSharp4-based rule-based vision workbench.
- It is for learning, verifying, and composing image-processing inspection recipes with tools such as Threshold, Blob, Contour, Line/Length, Matching, EdgeBasedMatching, and Feature/Shape-style workflows.
- It is not a camera, lighting, PLC, or I/O integration platform.

## Stable Contracts

- Keep algorithm tools PropertyGrid-based.
- A model property object assigned to PropertyGrid `SelectedObject` should generate the parameter UI.
- Keep business logic out of View code-behind. Move it to ViewModel, Controller, Presenter, Behavior, Converter, Runtime, or Service classes.
- Creating an output layer must not automatically change the input layer.
- Boolean visibility toggles must not trigger Preview or Run.
- Layer create/delete/load-image actions must not auto-run tools.
- Do not remove viewer zoom/pan/drag, ROI overlay, template editor, layer comparison, or docking features.
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
- UI/UX changes require current-build before/after evidence. Do not reuse old screenshots.

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
