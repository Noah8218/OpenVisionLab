# OVL-35 Shell Recipe Validation Suite view boundary — 2026-09-09

## Status

Complete for one independently verifiable Shell XAML presentation boundary. The overall refactoring program remains active.

## Scope

The Shell Recipe `PipelineRunHistory` tab previously contained the complete Validation Suite presentation: scope selection, explicit Run/Stop actions, evidence summary, local validation-set editor, image rows, and the suite summary. That cohesive presentation surface now belongs to `OpenVisionRecipeValidationSuiteView.xaml`.

`OpenVisionShellHostView.xaml` composes the new UserControl at the same tab position. The existing `RecipeCommands` bindings, AutomationIds, inherited Shell `DataContext`, explicit Preview/Run commands, Recipe/XML persistence, Layer/ImageSpace routing, and validation state remain in their existing owners. The code-behind contains only `InitializeComponent`. The extracted view has a local `BooleanToVisibilityConverter` resource because the former resource lived in the Shell panel's resource scope; this preserves the same binding behavior after the namescope boundary.

`OpenVisionRecipeBasicLifecycleView` remains a separate presentation owner. CommandSurface validation, execution, persistence, and Step Edit policy were not moved into the View.

## Structural proof

The call path is now:

`OpenVisionShellHostView -> PipelineRunHistory TabItem -> OpenVisionRecipeValidationSuiteView -> inherited DataContext -> RecipeCommands bindings/commands`.

The old `HostRecipeValidationSuitePanel` block and its 30 validation AutomationIds were removed from the Shell XAML. The new view owns each validation AutomationId exactly once, while the qualified-snapshot surface remains the next Shell sibling. The extracted block's whitespace-stripped token hash is unchanged (`c121eab1b491f1595528941b420bfb4c45cbf2e78a8c72d66812ce7410214fbe`, 15,538 non-whitespace characters), and both XAML files parse as XML.

`RecipeValidationSuiteViewContract` passed four checks in Debug and Release:

- Shell composes the dedicated view and retains the qualified-snapshot sibling.
- The extracted view owns the validation AutomationId surface with no duplicates in Shell.
- RecipeCommands bindings and the inherited DataContext boundary are retained.
- Code-behind is a presentation-only UserControl with no extra workflow logic.

## Build, focused contracts, and runtime evidence

`OpenVisionLab.csproj` Debug and Release builds completed with zero warnings and zero errors. `PipelineViewerScreenshotSmoke.csproj` Debug and Release builds completed with zero errors and the existing single `CS8600` warning in `Program.cs`; this slice introduced no new warning.

`Invoke-RefactorAudit.ps1 -Verify` passed with `CSharpFiles=813`, `XamlFiles=60`, `PartialDeclarations=110`, `ViewModelUiIoFiles=1`, `ProjectCycles=0`, and `ShellStorageCalls=0`. The audit artifacts are under `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl35-refactor-audit-final-20260909`.

The new view contract passed `4/4` in both configurations with `OwnerAutomationIdCount=30`. Focused OVL-31 configuration (`7/7`), OVL-32 execution-progress (`7/7`), OVL-33 review-queue evidence (`6/6`), and OVL-34 drawing-evidence (`7/7`) regression contracts also passed in Debug and Release. Evidence directories are under `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl35-validation-suite-view-contract-debug4-20260909`, `ovl35-validation-suite-view-contract-release4-20260909`, and the `ovl35-regression-*` directories.

The focused EXE target `wpf_shell_host_recipe_local_validation_dataset` passed in Debug and Release with the Validation Suite visible. Captures are:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl35-runtime-validation-suite-view-debug5-20260909\wpf_shell_host_recipe_local_validation_dataset.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl35-runtime-validation-suite-view-release-20260909\wpf_shell_host_recipe_local_validation_dataset.png`

The workstation reported one monitor, `\\.\DISPLAY2`, bounds `1920x1080`, and working area `1920x1032`. The observed test windows were `1600x900` and intersected the selected monitor. Fresh captures show the scope controls, evidence board, local set editor, image list, and summary in the extracted view. Alternate themes, Wide/Compact layouts, and 100/125/150/175/200% DPI rows remain environment-bound and are not claimed by this slice.

## Junior developer assessment

**PASS for this boundary.** A new contributor can follow the Shell composition to one named View, then follow its bindings to the existing `RecipeCommands` owner. No new interface, factory, wrapper, message bus, or duplicate command policy was introduced.

## No-repeat boundary

Do not let another model, agent, or scheduled run recreate, rename, re-split, or move command/state policy into `OpenVisionRecipeValidationSuiteView` without a newly reproduced UI defect, changed explicit contract, or demonstrated responsibility/dependency conflict. Do not split this view by file size alone. Keep `OpenVisionRecipeBasicLifecycleView`, Shell CommandSurface, and prior OVL-31/32/33/34 smoke owners separate.

## Dev checkpoint

Selective Dev commits `[2.1.0]` `890ad48b551f56d722b5aee68de0e0256e41586e` and `0830cdb4` were pushed to `origin/codex/public-sample-ux-docs`; the latter completes the 30-id contract coverage. Existing dirty changes were preserved. `Original`, tags, release publication, and deployment remain unchanged.

## Next priority

Inspect the generic internals of `WpfPropertyGridAdapter` for a concrete reusable/policy dependency boundary. Reuse the completed `PropertyGridToolPolicy` and OVL-20 subscription owner; reopen or split only with current code evidence | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.
