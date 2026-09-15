# OpenVisionLab Dev/Original version alignment

Status: Complete
Date: 2026-09-09 KST

## Scope

This record closes the approved `2.2.0-dev.1` promotion of the verified Dev
refactor snapshot. The same source and dependency candidate was committed on
Dev first and then promoted to the public Original `main` branch. Recipe/XML
compatibility, explicit Preview/Run actions, layer routing, and the existing
SDK/integration contracts remain the observable compatibility boundary.

The candidate includes the already completed ownership work for ImageCanvas
input/dialog/save lifetime, runtime log buffering and shutdown, ImageSpace and
ImageCompare resource ownership, Recipe execution/session/validation/step-edit/
run-history projections, Pipeline Review execution/revision/projection owners,
Shell command-surface coordination, Learn topic views/presenters, PropertyGrid
policy/metadata/value-change adapters, 2D integration identity/metric policy,
and the namespace move for `TemplateImageExtraction`. The ImageCompare tool
project now links its shared image-resource owner explicitly, and readiness and
structural audit checks follow the resulting owners.

## Version and commits

| Boundary | Base | Verified commit | Remote ref |
| --- | --- | --- | --- |
| Dev | `5386db1b739bf06d3ba5132c00cfb45803a2a6df` | source `99c048f70409b3a4bd26651fc7fd56dac55a7109` + evidence `e018ec349d87d65180ac9e7860903c0ade719f30` | `origin/codex/public-sample-ux-docs` = `e018ec349d87d65180ac9e7860903c0ade719f30` |
| Original `main` | `604c7fb6fc5247198afd666b3e3f37241ac069ba` | code `9a977b457fddd5d2bcc53cd0dfb3c614707f50e9` + README correction `bd4659b713a88b7c6d335606a231c510174524fc` | `origin/main` = `bd4659b713a88b7c6d335606a231c510174524fc` |

The public version is `2.2.0-dev.1`.
A one-line README-only correction followed the code promotion to close the version-history Markdown span; it did not change source, dependencies, or runtime behavior.
It is a new development candidate after
the already published `2.2.0-dev`; it is not a stable release. The canonical
surfaces are `src/OpenVisionLab/OpenVisionLab.csproj`,
`src/OpenVisionLab/Core/State/GlobalState.cs`, the shell preview caption,
`tools/TwoDIntegrationTcpSmoke/Program.cs`, the manual manifests/generated
manual, `README.md`, and `CHANGELOG.md`. The vendored SDK DLLs and
`sdk-manifest.json` were synchronized because the refactored object-candidate
contracts require the verified SDK set.

## Structural ownership evidence

| Boundary | Current owner and call path | Mutable-state owner | Existing contract and shortest reading order |
| --- | --- | --- | --- |
| ImageCanvas | `RoiImageCanvasView` -> `RoiImageCanvasViewModel` -> `RoiImageCanvasMouseInputController` / `RoiImageCanvasKeyboardInputController` / `CanvasImageSaver` / `RoiImageCanvasDialogHost` | `RoiImageCanvasViewModel` owns selected image/ROI state; controllers translate input | Existing XAML bindings remain on the ViewModel. Read the View, ViewModel, then the controller named by the event. |
| Recipe execution and validation | `OpenVisionShellHostRecipeCommandSurface` -> `OpenVisionRecipeExecutionSessionViewModel` -> the selected execution, validation, step-edit, workspace, and run-history owner | The execution session owns run state and cancellation; projection owners return snapshots | Preview/Run and Recipe/XML contracts remain explicit. Read the session, then the owner matching the command path, then its focused contract. |
| Pipeline Review | Review document -> execution controller -> revision gate -> result/layer/guide/domain projection owners | Review document/controller owns current revision and cache lifetime | Existing review bindings and result status are preserved. Read the document, controller, revision gate, then projection owner. |
| Shell and Learn | Shell command controller -> `OpenVisionShellHostLearnWindowController` -> `OpenVisionLearnWindow` -> topic View + presenter + `OpenVisionLearnTopicPresentationPolicy` | Topic presenter/view owns presentation state; shell controller owns window lifetime | Existing command and Window contracts remain. Read controller, Learn Window, topic View, presenter, policy. |
| PropertyGrid | `VisionToolPropertyGridHost` -> `WpfPropertyGridAdapter` -> `PropertyGridMetadataAdapters` / `PropertyGridPropertyValueChangeSubscription`; inspection policy is `PropertyGridToolPolicy` | Adapter owns UI subscription lifetime; tool policy owns domain acceptance rules | Existing property binding and value-change contract remain. Read host, adapter, metadata adapter, then policy. |
| ImageCompare and namespace | ImageCompare tool -> linked `ImageCompareImageResource`; application code imports `OpenVisionLab.Property.TemplateImageExtraction` | Resource owner controls bitmap lifetime; Property namespace owns extraction | Tool project compile contract and namespace boundary contract passed. Read the project file, resource owner, then call sites. |

The dependency direction is View/command -> owner -> domain/service. No new
message bus or wrapper was introduced. The previous owners no longer retain
the extracted workflow responsibility; partial files remain only where the
existing framework composition requires them.

## Verification evidence

Dev candidate and Original promotion both passed the following focused checks.
All generated test output was written under `D:\OpenVisionLab-TestData`.

- `dotnet build OpenVisionLab.sln -c Debug -p:Platform=x64 --nologo` — pass,
  0 warnings/errors on Dev and Original.
- `dotnet build OpenVisionLab.sln -c Release -p:Platform=x64 --nologo` — pass,
  0 warnings/errors on Dev and Original.
- `dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj`
  in Debug and Release — pass, 0 warnings/errors on both repositories.
- `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj`
  in Debug and Release — pass, 0 errors; the existing nullable warning at
  `Program.cs:10140` remains in both configurations and was not changed.
- `OpenVisionReadinessCheck` — pass with all 13 readiness contracts on both
  repositories.
- `Invoke-RefactorAudit.ps1 -Verify` — pass:
  `CSharpFiles=815`, `XamlFiles=60`, `PartialDeclarations=110`,
  `ViewModelUiIoFiles=1`, `ProjectCycles=0`, `ShellStorageCalls=0`.
- `VisionRecipeRunnerSmoke` focused owner/lifetime/Recipe/Pipeline Review/
  Learn/PropertyGrid/namespace contracts — Debug and Release pass; the
  copied-runtime Recipe execution session contract passed `12 passed, 0 failed`.
- `PipelineViewerScreenshotSmoke` 13 contract targets — Debug and Release
  pass (command line, Learn document copy, bitmap/PNG/capture lifecycle,
  validation dataset artifacts/progress/evidence, recipe fixture/cleanup/view).
- Manual builder — Korean and English pass, 26 sections and 17 tools per
  language, with hashes `197FF1495861FEBE581871BECE2EA4F5A68B1BF6AAFEB243995A422D7E3F7F60`
  and `02F4D802AC8A224AA264C664D47893FE49ACE847F43ED29D2738ACF39327E025`.
- `git diff --check` — pass for the staged promotion changes.
- Dev and Original remote branch checks — each remote ref equals the commit
  listed above.

Evidence directories include:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\current-pipeline-contracts-debug-20260909`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\current-pipeline-contracts-release-20260909`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\promotion-original-contracts-debug-20260909`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\promotion-original-contracts-release-20260909`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\promotion-original-execution-contract-20260909`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\current-refactor-audit-20260909`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\promotion-original-refactor-audit-20260909`

## Discoverability and duplicate-work boundary

A developer can start at `OpenVisionLab.sln`, open
`src/OpenVisionLab/OpenVisionLab.csproj` (`WinExe`, `net8.0-windows7.0`), then
follow the project references and the owner reading order above. The mutable
state, binding/public contract, and focused proof are named for each moved
boundary so a file-length-only split is not needed.

The owners completed by this record are canonical. Another model or scheduled
run must not reimplement, retest, recomment, or re-document these same
responsibilities. Reopen a completed owner only when a new requirement, a
reproducible defect, a failed completion criterion, or a changed dependency
boundary is recorded with focused proof that the owner cannot satisfy it.
Otherwise continue only with the next unfinished boundary in the current
handoff; do not split, move, rename, or wrap a completed owner for style or
file length.

## Boundary and next priority

Actual desktop EXE interaction, all supported themes/layouts, DPI 100/125/150/
175/200%, monitor placement, and a full release-package launch were not run in
this promotion. Source-level and in-process contract evidence is complete;
those UI/runtime states remain the next qualification boundary.

Next priority: perform one fresh UI runtime qualification slice for the
representative Shell/Learn/PropertyGrid flow, capture monitor/DPI/theme evidence,
and update the current handoff only if a reproducible gap is found. Do not
reopen any owner above without one of the recorded reopening conditions.

Rollback references are Dev `5386db1b739bf06d3ba5132c00cfb45803a2a6df` and
Original `604c7fb6fc5247198afd666b3e3f37241ac069ba`. No tag, release, or
deployment was created.
