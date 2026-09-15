# OpenVisionLab OVL-18 Shell Step preview navigation owner — 2026-09-09

Status: **Complete for the single Shell Step preview navigation slice**.

## Scope

`OpenVisionShellHostRecipeCommandSurface.Handlers.cs` still owned the
preview-list policy used by Step Edit and failure review: matching a failed
Step reference, finding the previous/next preview, and deciding whether two
preview objects represented the same Step. The Shell now composes
`OpenVisionRecipeStepPreviewNavigationOwner` for those rules and keeps only
the selected Recipe summary, current selection, dirty-edit transition, and
status/PropertyChanged work.

```text
Shell preview list + selected preview
  -> OpenVisionRecipeStepPreviewNavigationOwner
  -> failure-reference match / previous-next boundary / identity comparison
  -> Shell selection and existing StepEdit loader/apply flow
```

The existing `OpenVisionRecipeStepEditLoader` remains the owner of persisted
Pipeline XML reload and PropertyGrid projection. The existing apply owner
remains the owner of XML save and round-trip validation. No Preview/Run,
Recipe/XML schema, Layer/ImageSpace, or PropertyGrid policy changed.

## Changed files

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeStepPreviewNavigationOwner.cs`
  - New Window-free owner for preview matching, offset navigation, and Step
    identity semantics.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs`
  - Composes the navigation owner.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs`
  - Delegates preview matching, offset lookup, and identity comparison; the
    former matching/normalization loop is removed from the Shell.
- `tools/VisionRecipeRunnerSmoke/StepPreviewNavigationOwnerContract.cs`
  - D-drive contract covering matching, normalization, boundaries, identity,
    and missing-input behavior.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - Adds `--step-preview-navigation-owner-contract` dispatch.

## Verification

- OpenVisionLab Debug build: 0 warnings / 0 errors.
- VisionRecipeRunnerSmoke Debug build: 0 warnings / 0 errors.
- OpenVisionLab Release build: 0 warnings / 0 errors.
- VisionRecipeRunnerSmoke Release build: 0 warnings / 0 errors.
- New owner contract: 5/5 passed in Debug and Release.
- Existing Step Edit loader, apply-owner, and apply-projection contracts:
  passed in Debug.
- Static proof confirms the Shell calls the new owner and the former
  `StepMatches`, `TryExtractStepIndex`, and `NormalizeStepMatchText` helpers
  no longer exist in the CommandSurface partials.

Evidence directories:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\step-preview-navigation-owner-debug-20260909-run2`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\step-preview-navigation-owner-release-20260909-run2`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl18-regression-step-edit-loader-20260909`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl18-regression-step-edit-apply-20260909`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl18-regression-step-edit-projection-20260909`

## Junior readability assessment

**PASS for this slice.** A maintainer can now follow preview navigation from
one named owner with a small input/output surface. The owner has no WPF or
mutable Shell state; loader/apply responsibilities remain visibly separate.

Do not let another model or agent recreate this owner, move the same matching
loop again, or split the same Step Edit navigation path without a newly
reproduced defect, a changed explicit contract, or a proven responsibility
conflict.

## Boundary and next priority

This slice does not qualify WPF runtime themes, layouts, DPI, or desktop
window behavior. The residual Shell CommandSurface Step Edit orchestration
and tool-menu mapping remain in the existing owner. The next independent
slice is a Shell XAML vertical slice only after its View/ViewModel boundary
and runtime evidence target are identified.

Recommended model: `gpt-6-astra` | Reasoning effort: `high`.
