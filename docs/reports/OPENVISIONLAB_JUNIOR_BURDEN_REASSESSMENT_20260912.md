# OpenVisionLab 2D first-time developer burden reassessment

Updated: 2026-09-12 KST  
Repository: `C:\Git\2D\Dev`  
Branch: `codex/public-sample-ux-docs`  
Source HEAD: `176eec95` (`refactor: complete junior discoverability and reliability cleanup`)  
Target framework: `.NET 8 / net8.0-windows7.0`, WPF x64  
Product version: `2.2.0-dev.2`

## Scope and closure

This is the final reassessment for the five-minute, five-area discoverability schedule registered as PL-0018. The schedule kept one executor, did not wait for optional user or hardware input, and reopened an owner only when a reproducible defect or independent lifetime/state boundary was demonstrated.

The five slices are complete. M1, M2, and M3 were bounded source audits with no new production owner justified. M4 made one documentation-only navigation improvement. M5 performed the final source-wide MVVM, partial, folder, project-graph, image-lifetime, and readability audit. No product C# or XAML was changed in this schedule.

## Before and after assessment

| Burden area | Current owner and route | Change in this schedule | Result for a first-time contributor |
| --- | --- | --- | --- |
| 1. Shell composition | `Program.Main -> OpenVisionLabApplication.Run -> OpenVisionShellHostWindow -> OpenVisionShellHostView`; Shell phases and existing PL-0014 owner map | Rechecked the existing phase markers and ownership route; no new Manager/Factory/partial | The composition order is searchable. The constructor remains large, but ownership is explicit and should be read as a composition root. |
| 2. Recipe CommandSurface | `RecipeCommandSurface` binding state plus existing execution, step-edit, Pipeline, validation, review, snapshot, and workspace owners | Confirmed that the remaining 10,076-line surface is a shared binding/callback composition boundary; no safe mechanical split | Regions and concrete child owners explain where to continue. A file-length split would make shared state and callback order harder to follow. |
| 3. Pipeline Review | `Document -> RevisionGate -> ExecutionController -> VisionPipelineExecutionService -> ViewModel/View`; `LayerImageOwner` and cache own image leases | Rechecked execution, cancellation, stale-callback, result-cache, and Bitmap ownership; no defect reproduced | Run/revision/image state has named owners. View code-behind remains display and interaction code; runtime theme/DPI/device rows are still separate verification work. |
| 4. Smoke tools versus product code | `src/` product owners; `VisionRecipeRunnerSmoke` headless contracts; `PipelineViewerScreenshotSmoke` WPF target runner; conditional DirectSmokeRunner | Added a short product-versus-verification route to `docs/README.md`; preserved the opt-in embedded `--smoke` seam | Large smoke files are clearly identified as verification orchestration. A developer starts at the product owner, then its contract, then WPF smoke evidence. |
| 5. MVVM, partials, folders, modules | 27 projects / 34 references / cycle 0; responsibility folders under `Common`, `Core`, `UI`, and libraries | Re-ran the whole-repository static audit; no manual business partial or root-folder move met the extraction threshold | Remaining partials are XAML/generated/framework composition or cohesive tool views. Folder density is mostly feature cohesion, not an undifferentiated class dump. |

## Final structure findings

- `Invoke-RefactorAudit.ps1 -Verify` reports `CSharpFiles=816`, `XamlFiles=60`, `PartialDeclarations=59`, `PartialTextMatches=2`, `ViewModelUiIoFiles=1`, `ProjectCycles=0`, and `ShellStorageCalls=0`.
- The 59 compiled partial declarations are XAML/framework composition, generated `ImageCanvasControl` code, or cohesive WPF tool views. The audit found no separate manual business partial family that should be merged or moved in this schedule.
- The seven files at or above 3,000 lines are dominated by smoke orchestration (`PipelineViewerScreenshotSmoke`, `DirectSmokeRunner`, `VisionRecipeRunnerSmoke`), the Recipe command composition surface, readiness tooling, and the PropertyGrid bridge. Size alone did not prove a new state or lifetime boundary.
- Five ViewModel files have direct file/path signals and one known UI/IO coupling signal is retained in `RoiImageCanvasViewModel`. Existing `OpenVisionWorkspaceSamplePickerViewModel`, `ImageCompareViewModel`, `VisionToolPropertySummaryViewModel`, and Recipe validation model path checks were reviewed as display/path state rather than new persistence owners. This remains a follow-up boundary, not a reason to add wrappers during this schedule.
- Existing `ImageSpaceFrame` Borrow/TakeOwnership, `SerializeHelper` atomic save, Pipeline execution plan/service, Review revision/cache owners, Recipe dialog adapter, and contract/smoke projects remain the canonical implementations.

## Recommended reading order

1. `AGENTS.md` and `docs/README.md`.
2. `src/OpenVisionLab/Program.cs`.
3. `src/OpenVisionLab/App/Bootstrap/OpenVisionLabApplication.cs`.
4. `src/OpenVisionLab/UI/Menu/Wpf/Windows/OpenVisionShellHostWindow.xaml.cs`.
5. `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.xaml.cs` and the named Shell phase owners.
6. `src/OpenVisionLab/Core/Pipeline/Execution/VisionPipelineExecutionService.cs`.
7. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/RecipeCommandSurface.cs` only after locating its concrete child owner.
8. `src/OpenVisionLab/UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs` -> `PipelineReview/Execution/OpenVisionPipelineReviewExecutionController.cs` -> `PipelineReview/Presenters/OpenVisionPipelineReviewLayerImageOwner.cs`.
9. `tools/VisionRecipeRunnerSmoke/*Contract.cs` for a window-free contract, then `tools/PipelineViewerScreenshotSmoke/ScreenshotSmokeTargetRunner.cs` for a WPF target.

This order follows `Image -> Layer -> Tool -> Inspection -> Pipeline -> Recipe -> Result -> Review` while keeping composition, execution, persistence, and presentation distinct.

## Remaining technical debt and verification boundary

- `RoiImageCanvasViewModel` still has a known UI/IO coupling signal. It should be reopened only with a concrete replacement owner, call path, and focused contract.
- The Shell constructor and Recipe CommandSurface remain large composition surfaces. No additional split is justified without an independent state/lifetime/test boundary or a reproduced defect.
- Full WPF alternate theme, Wide/Compact layout, 125/150/175/200% DPI, pointer/keyboard matrix, camera/SDK/GPU execution, permanent native hang, and long-run shutdown remain `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.
- This schedule did not change public API, Recipe/XML format, SDK contract, or user Preview/Run behavior.

## Verification evidence

- Solution Debug build: `dotnet build OpenVisionLab.sln --configuration Debug --no-restore -m:1 -nr:false` — 0 warnings, 0 errors.
- Solution Release build: `dotnet build OpenVisionLab.sln --configuration Release --no-restore -m:1 -nr:false` — 0 warnings, 0 errors.
- Readiness Debug and Release: all 13 checks `OK`; `OpenVisionLab readiness contract passed.`
- `TestDocumentationIndex.ps1`: initial M4 documentation check `DocumentationIndex=PASS IndexedPaths=293 Routes=16 RootRedirects=102`; final re-run after this report/index entry `DocumentationIndex=PASS IndexedPaths=294 Routes=16 RootRedirects=102`.
- `Invoke-RefactorAudit.ps1 -Verify`: `REFACTOR_AUDIT=PASS|CSharpFiles=816|XamlFiles=60|PartialDeclarations=59|PartialTextMatches=2|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
- PL-0018 issue ledger: `PL-0018: valid v2` after M1–M5 evidence updates.
- Evidence root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\junior-burden-1-5-20260912`.

The schedule evidence is complete for source, ownership, documentation, Debug/Release build, and readiness checks. Runtime rows listed above remain explicitly unverified rather than inferred from source inspection.
