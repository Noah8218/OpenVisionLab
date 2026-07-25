# Recipe Command Surface Validation Set Refactor Proof (2026-07-25)

Status: Complete

## Scope

Move the local Validation Set lifecycle from the broad recipe command-handler file into a dedicated partial class. The scope is limited to creation, deletion, image/folder registration, missing-path repair, persistence, and Validation Set option/image-row projection.

## Intended responsibility boundary

- `OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs` owns local Validation Set CRUD, storage persistence, and selection projections.
- `OpenVisionShellHostRecipeCommandSurface.Handlers.cs` retains recipe execution, Preview/Run, layer routing, and unrelated recipe workflows.
- Existing command bindings and test-only entry points keep the same method names and behavior.

## Acceptance criteria

- The moved methods have one implementation, in the Validation Sets partial.
- Validation Set command bindings compile without behavior or API changes.
- A focused Validation Set smoke and full solution build pass from the current Dev workspace.
- The repository readiness check passes.

## Verification

- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- `dotnet run --no-build --project tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj -c Debug -- C:\Git\OpenVisionLab_Dev` passed after the explicit WPF composition-file allow list was updated for the new partial.
- Current-source `wpf_shell_host_recipe_local_validation_set` smoke passed. It exercised set creation, direct image and top-level-folder registration, missing-path rejection and repair, persisted metadata, the explicit Validation Set run, and the invariant that registration does not alter Preview/Run count, layers, workspace, or routing.
- A source search confirms that the extracted lifecycle methods now have implementations only in `OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs`; the original handler retains only call sites that refresh Validation Set options.

## Evidence

- `artifacts\\maintenance_recipe_validation_set_refactor_20260725\\wpf_shell_host_recipe_local_validation_set.png\\wpf_shell_host_recipe_local_validation_set.png`
- `UI\\Menu\\Wpf\\OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs`

## Boundary

This is a responsibility split only. It does not change Validation Set storage, run criteria, Pipeline behavior, Preview/Run semantics, layer routing, or any inspection algorithm.
