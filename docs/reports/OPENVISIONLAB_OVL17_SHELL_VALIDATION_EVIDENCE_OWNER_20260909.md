# OpenVisionLab OVL-17 Shell validation evidence owner — 2026-09-09

Status: **Complete for the single Shell validation-evidence projection slice**.

## Scope

`OpenVisionShellHostRecipeCommandSurface.Handlers.cs` still loaded the selected
Pipeline XML and owned the acceptance-gate and `PIXELPERMM` calibration rules
used by the Validation Set panel. That mixed Shell binding state, file access,
and validation policy in one call path.

The new
`OpenVisionRecipeValidationEvidenceOwner` now owns that one boundary:

```text
Shell selected Recipe/Pipeline state
  -> OpenVisionRecipeValidationEvidenceOwner.Build
  -> RecipeWorkspaceService path + VisionPipelineStorage XML load
  -> enabled acceptance-step and mm-calibration policy
  -> OpenVisionRecipeValidationEvidence text result
  -> Shell binding properties and PropertyChanged notifications
```

The owner is Window-free and does not execute Preview or Run. The Shell keeps
the existing `ValidationSetAcceptanceText`, `ValidationSetCalibrationText`,
and notification contract. Existing Korean/English messages, gate ordering,
two-gate truncation, and the fail-closed missing-`PIXELPERMM` message were
preserved.

## Changed files

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationEvidenceOwner.cs`
  - New concrete owner and immutable result for Pipeline-read validation evidence.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs`
  - Composes the existing owner.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs`
  - Delegates acceptance/calibration text to the owner; removed the former
    Shell XML-read and evidence-policy helpers.
- `tools/VisionRecipeRunnerSmoke/ValidationEvidenceOwnerContract.cs`
  - D-drive focused contract for success, missing calibration, no selection,
    and missing XML paths.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - Adds `--validation-evidence-owner-contract` dispatch.

No XAML, Recipe/XML schema, Preview/Run command, Layer/ImageSpace, or
PropertyGrid policy changed.

## Verification

The focused owner contract passed in both configurations:

```text
dotnet .../VisionRecipeRunnerSmoke.dll --validation-evidence-owner-contract <D: evidence>
CONTRACT|validation-evidence-owner|passed=4|failed=0
```

- Debug evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\shell-validation-evidence-owner-20260909-run2\validation-evidence-owner-contract.txt`
- Release evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\shell-validation-evidence-owner-20260909-release\validation-evidence-owner-contract.txt`
- OpenVisionLab Debug build: 0 warnings / 0 errors.
- VisionRecipeRunnerSmoke Debug build: 0 warnings / 0 errors.
- OpenVisionLab Release build: 0 warnings / 0 errors.
- VisionRecipeRunnerSmoke Release build: 0 warnings / 0 errors.

Static proof after the change confirms the Shell calls
`validationEvidenceOwner.Build`, while the removed helper names no longer
exist in the Shell. The contract verifies the new owner directly without
constructing a Window.

## Junior readability assessment

**PASS for this slice.** A maintainer can follow the validation panel path from
the Shell facade to one named owner, then to the existing workspace/storage
services. The owner has no WPF dependency, and the result separates the
acceptance and calibration text channels while retaining the existing UI
binding names.

Do not let another model or agent repeat this owner, recreate its contract, or
split the same acceptance/calibration path again without a newly reproduced
defect, a changed explicit contract, or a proven responsibility conflict.

## Boundary and next priority

This slice does not prove full WPF theme/layout/DPI runtime coverage or close
the remaining Shell CommandSurface responsibilities. The next independent
slice is the residual Shell **Step Edit preparation/navigation** call path,
after confirming that the existing Step Edit loader/apply owners are not
duplicated.

Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.
