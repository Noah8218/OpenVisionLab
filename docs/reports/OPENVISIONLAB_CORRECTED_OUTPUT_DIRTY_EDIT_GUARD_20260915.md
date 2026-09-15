# OpenVisionLab Corrected-Output Dirty-Edit Guard

Date: 2026-09-15 KST  
Status: Complete  
Issue: `PL-0058`  
Repository: `C:\Git\2D\Dev`  
Evidence root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\PL-0058_dirty-rerun-20260915-final2`

## User problem and bounded workflow

The Recipe Manager already exposed a corrected-output panel after a failed Step
parameter edit. Its text required `Apply to XML` before rerun, but the rerun
command remained executable while the PropertyGrid edit session was dirty. The
operator could therefore rerun the previously applied pipeline and mistake that
result for evidence from the pending edit.

The bounded workflow is now:

```text
failed Step -> load parameters -> edit PropertyGrid -> rerun disabled
           -> Apply to XML (or discard) -> output review -> explicit rerun
```

XML apply still does not execute Preview/Run, change layers, or change input/output
routing.

## Owner map and contract

- Current owner: `RecipeCommandSurface` owns selected Step edit state, command
  enablement, rerun scope, and user-facing text.
- Mutable-state writer: `OpenVisionRecipeStepEditSessionViewModel` writes
  `IsDirty`; `RecipeCommandSurface.OnSelectedStepEditSessionPropertyChanged`
  refreshes the command state.
- View contract: `OpenVisionShellHostView.xaml` binds
  `HostRecipeCorrectedOutputRerunButton` to
  `RecipeCommands.RerunCorrectedOutputCommand`, its text, and its tooltip.
- Call path: PropertyGrid edit -> `MarkSelectedStepEditDirty` ->
  `CanRerunCorrectedOutput` / `RerunCorrectedOutput` -> existing Good/Bad or
  Local Validation Set runner. Successful XML apply marks the session clean and
  restores the existing scope-specific rerun command.
- No new service, interface, partial, ViewModel, or dialog boundary was added.

## Change

`RecipeCommandSurface` now:

- returns `false` from `CanRerunCorrectedOutput` while `IsSelectedStepEditDirty`;
- changes the button label to `Apply before rerun` / `XML 반영 후 재검사`;
- explains that the pending edit must be applied or discarded in the tooltip; and
- keeps a defensive guard in `RerunCorrectedOutput` for direct command invocation.

The focused WPF smoke now asserts both the rendered button state and
`ICommand.CanExecute` while dirty, captures a fresh pending-state image, then
applies XML and verifies the existing Good/Bad rerun path is enabled and completes.

## Acceptance and verification

| Criterion | Evidence |
| --- | --- |
| Dirty Step edit disables corrected-output rerun for the existing pair path | `PipelineViewerScreenshotSmoke` target passed; runtime assertion checks `Button.IsEnabled == false` and `CanExecute == false`. |
| Dirty guidance tells the operator to apply or discard before rerun | Runtime assertion checks the label and tooltip; pending-state PNG is captured. |
| XML apply restores the existing rerun scope without implicit execution | The same smoke verifies clean state, persisted edited `MIN_AREA`, unchanged preview/layer/route state before explicit rerun, then completes Good/Bad rerun. |
| Build and UI evidence are reusable | Solution and focused smoke builds report 0 warnings/0 errors; fresh pending/applied PNGs and logs are under the evidence root. |

Commands actually run:

```text
dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform="Any CPU" --nologo
dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU" --nologo
PipelineViewerScreenshotSmoke --target wpf_shell_host_fixture_step_edit_apply_rerun <evidence-root>
PipelineViewerScreenshotSmoke --target p252_contextual_correction_rerun <local-set-regression-root>
git diff --check -- src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/RecipeCommandSurface.cs tools/PipelineViewerScreenshotSmoke/Program.cs
```

Runtime result: `wpf_shell_host_fixture_step_edit_apply_rerun=OK`, `check=OK`,
`layout=0`, `text=0`, `internal=0`, `1600x900` capture.
The existing Local Validation Set regression target also passed:
`p252_contextual_correction_rerun=OK`, `check=OK`, `layout=0`, `text=0`,
`internal=0`, with its log under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\PL-0058_local-set-regression-20260915`.

Fresh visual evidence:

- `fixture-step-edit-pending.png` — dirty edit, apply-before-rerun guidance,
  disabled rerun affordance.
- `fixture-step-edit-applied-corrected-output.png` — clean edit, corrected-output
  guidance, existing explicit rerun affordance.
- `wpf-shell-host-fixture-step-edit-apply-rerun.log` — runtime exit `0`.
- `solution-build.log` — solution build exit `0`, warnings `0`, errors `0`.

## Boundary

This proves the current Recipe Manager workflow in the focused source-built WPF
smoke at the exercised default runtime scale. It does not qualify every theme,
100/125/150/175/200% DPI state, keyboard/mouse state matrix, long-run behavior,
field participants, hardware integration, packaging, or release readiness.
