# OpenVisionLab ImageCanvas debug output cleanup — 2026-09-11

## Status

Complete for the bounded production debug-output cleanup. The change removes
two active timing prints from the existing ImageCanvas ViewModel and leaves
image loading, ownership, persistence, and UI contracts unchanged.

## Baseline and change

| Item | Value |
| --- | --- |
| Repository | `C:\Git\2D\Dev` |
| Branch | `codex/public-sample-ux-docs` |
| Commit before local change | `0a77e60e444b12757565ee977216a994446b53a6` |
| Target framework | `net8.0-windows7.0` |
| Changed file | `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs` |

`OpenLoadImage` previously wrote `LoadMatFromFile` and `LoadImage` elapsed
milliseconds to `Console`. Those messages were development instrumentation,
not the product logging contract. The change removes the two writes and the
now-unused `System.Diagnostics` import. The existing call path remains:

```text
ImageCanvas View command
  -> RoiImageCanvasViewModel.OpenLoadImage
  -> ImageDialogHost.ShowOpenImageDialog
  -> CanvasImageLoader.LoadMatFromFile
  -> RoiImageCanvasViewModel.LoadImage
  -> ImageCanvasDirectoryPolicy.RememberImagePath
```

The ViewModel remains the image-state owner, `CanvasImageLoader` remains the
Mat creation owner, and the existing `using` scope still releases the loaded
Mat. No new logger, interface, service, partial, or wrapper was added.

## Existing and changed behavior

- Existing behavior: canceling the dialog returns; a selected file is loaded,
  projected to the canvas, and remembered by the existing directory policy.
- Changed behavior: the two development timing lines are no longer written to
  stdout. No UI, image, Recipe, binding, command, or file-format contract
  changed.

## Verification

Evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-review-recheck-20260911\debug-output-cleanup`.

- OpenVisionLab solution Debug build: 0 warnings, 0 errors.
- OpenVisionLab solution Release build: 0 warnings, 0 errors.
- `OpenVisionReadinessCheck` Release: `OpenVisionLab readiness contract passed.`
- Production source scan: no active `Console.WriteLine`/`Console.Write` calls
  remain under `src`; smoke-tool result output remains under `tools`.
- The concurrent Machine Studio test process was not touched. After the
  verification commands, no `OpenVisionLab`, PipelineViewer, VisionRecipeRunner,
  or ReadinessCheck process remained.

## Junior contributor assessment

The ImageCanvas load command no longer emits unexplained timing lines while a
developer is debugging an unrelated workflow. The ownership path remains
direct and unchanged, so a contributor can read the command, loader, state
owner, and release scope without following a new abstraction.

## Completion record

```text
Status: Complete
Scope: Remove two active ImageCanvas production debug timing writes
Acceptance criteria: debug output removed -> pass; image load call path preserved -> pass; solution Debug/Release build and readiness -> pass
Verification: dotnet build OpenVisionLab.sln -c Debug/Release --no-restore -m:1; OpenVisionReadinessCheck Release; source console-write scan
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-review-recheck-20260911\debug-output-cleanup
Boundary / next dependency: full ImageCanvas interactive UI/runtime matrix remains covered by existing WPF qualification boundaries; this change was not a new UI feature
```
