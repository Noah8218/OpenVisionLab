# OpenVisionLab OVL-50 — Validation Set evidence `PropertyChanged` projection

Status: **Complete** for one independently verifiable notification-lifetime
fix. The broader Shell validation and Step Edit refactoring program remains
active.

## Confirmed defect and smallest change

The Validation Set selection setter already owns the WPF projection boundary:
it selects the mutable option, refreshes the selected image row, raises the
selection and summary bindings, raises the four evidence bindings, and then
refreshes command state. `RefreshValidationSetImageRows()` also raised the
same four evidence bindings. Selecting another set therefore delivered two
`PropertyChanged` notifications for each evidence binding.

The helper now refreshes image rows and the pending Variant contract only.
`SelectedValidationSetOption` remains the single evidence-notification owner.
No new presenter, interface, wrapper, partial, callback, or message bus was
introduced.

## Ownership and call path

| Boundary | Current owner | Intended owner | Evidence |
| --- | --- | --- | --- |
| Validation Set document/XML | `OpenVisionRecipeValidationSetDocumentOwner` -> existing storage | unchanged | `TryLoad`/`TrySave` remain in the document owner |
| Selected set and image-row mutable state | `OpenVisionRecipeValidationSetSelectionOwner` | unchanged | `SelectSet`/`RefreshImageRows` remain the only state writers |
| Evidence policy text | `OpenVisionRecipeValidationEvidenceOwner` | unchanged | WPF-free `Build` result contract remains in use |
| WPF binding notification projection | `OpenVisionShellHostRecipeCommandSurface` | unchanged | `SelectedValidationSetOption` setter raises each evidence property once |

The actual path is:

```text
SelectedValidationSetOption setter
  -> ValidationSetSelectionOwner.SelectSet
  -> RefreshValidationSetImageRows
  -> ValidationSetSelectionOwner.RefreshImageRows
  -> LoadSelectedValidationVariantContract
  -> Shell PropertyChanged for rows/selected/pending state
  -> Shell PropertyChanged for selection summaries and four evidence bindings
  -> RefreshCommandState
```

The mutable-state write owner is the selection owner; the Shell owns only the
binding-facing notification order. The existing evidence owner still supplies
acceptance/calibration policy text, and Recipe/XML persistence remains outside
the Shell projection.

## Public and binding contract

The existing bindings remain unchanged:

- `ValidationSetExpectedText`
- `ValidationSetAcceptanceText`
- `ValidationSetCalibrationText`
- `ValidationSetNextActionText`

One selected-set change now raises each of those names exactly once. Selection,
summary, Variant, Preview, Run, and Recipe/XML contracts are otherwise
unchanged. The change does not execute Preview or Run.

## Verification

The focused Runner contract created two disk-backed Validation Sets in an
isolated D: data root, constructed the existing Shell command surface, selected
the second set, and counted the four binding notifications:

```text
PASS|one Validation Set selection raises each evidence binding exactly once
CONTRACT|validation-set-evidence-notification|passed=1|failed=0
```

Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl50-validation-set-evidence-notification-contract-debug-20260909-run1\validation-set-evidence-notification-contract.txt`

The readiness gate also asserts that the row-refresh helper does not raise the
evidence notification and that the selected-set setter retains the single
projection call. Runner and application Debug/Release builds, readiness, the
refactor audit, documentation-index validation, JSON parsing, and
`git diff --check` are recorded in the current handoff after this slice.

## Developer reading order

1. `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs` —
   `SelectedValidationSetOption` setter.
2. `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs` —
   `RefreshValidationSetImageRows` and Validation Set document operations.
3. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetSelectionOwner.cs` —
   selection mutable state.
4. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationEvidenceOwner.cs` —
   acceptance/calibration result policy.
5. `tools/VisionRecipeRunnerSmoke/ValidationSetEvidenceNotificationContract.cs` —
   one-selection notification regression.
6. `tools/OpenVisionReadinessCheck/Program.cs` — static ownership gate.

One search for `NotifyValidationSetEvidenceChanged` reaches the projection
owner and its callers.

## Boundary lock

OVL-50 closes the duplicate evidence-notification path. Other models, agents,
and automations must not recreate, move, split, or wrap this projection or the
OVL-17/46/49 owners unless a new reproducible defect, explicit binding or
Recipe contract change, or dependency boundary conflict is recorded first.
File length, model preference, and repeated continuation requests are not
reopening reasons.

Runtime Shell EXE selection, binding, theme/layout, DPI/monitor, popup, and
full Preview/Run UI were not exercised in this source/contract slice; the
remaining runtime scope is `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.
