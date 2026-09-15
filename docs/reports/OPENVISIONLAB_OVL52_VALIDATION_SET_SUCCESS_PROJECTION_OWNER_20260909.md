# OpenVisionLab OVL-52 — Validation Set success projection

Status: **Complete** for one independently verifiable success-state projection
fix. The broader Shell validation and Step Edit refactoring program remains
active.

## Confirmed defect and smallest change

`RefreshValidationSetOptions` successfully refreshes the selected Validation
Set and then calls `RefreshCommandState`. The success branch also raised
`ValidationSuiteSummaryText` directly, while `RefreshCommandState` raised the
same binding again. A successful refresh therefore delivered two summary
notifications from one branch.

The direct success-branch notification was removed. `RefreshCommandState`
remains the shared command-state owner and now emits the summary notification
once for this branch. No new service, interface, wrapper, partial, or public
API was added.

## Ownership and call path

| Boundary | Current owner | Intended owner | Evidence |
| --- | --- | --- | --- |
| Validation Set document/XML | `OpenVisionRecipeValidationSetDocumentOwner` -> existing storage | unchanged | `TryLoad` still owns the document read |
| Selection and image rows | `OpenVisionRecipeValidationSetSelectionOwner` | unchanged | `Refresh` still owns mutable selection state |
| Selection/evidence binding projection | `OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs` | unchanged | success branch raises options, selection, rows, summary, and evidence bindings |
| Shared suite-summary command-state projection | `OpenVisionShellHostRecipeCommandSurface.Handlers.cs` | unchanged | `RefreshCommandState` is the single `ValidationSuiteSummaryText` notifier |

Actual path:

```text
RefreshValidationSetOptions
  -> ValidationSetDocumentOwner.TryLoad
  -> SelectionOwner.Refresh
  -> Shell options/selection/split/row notifications
  -> LoadSelectedValidationVariantContract
  -> RefreshPinArrayGapValidationIdentityState
  -> NotifyValidationSetEvidenceChanged
  -> ValidationSetSelectionSummaryText notification
  -> RefreshCommandState (ValidationSuiteSummaryText once)
```

The document owner writes `StorageReady`; the selection owner writes mutable
selection and row state; the Shell owns WPF notification order. Recipe/XML
persistence, binding names, Preview/Run explicit execution, and error-branch
behavior remain unchanged.

## Binding contract

For one successful `RefreshValidationSetOptions` call, the existing bindings
receive one `ValidationSetSelectionSummaryText` notification, one
`ValidationSuiteSummaryText` notification, and one notification for each of
the four Validation Set evidence bindings. The contract invokes the existing
private method through reflection only to isolate this branch; production code
gains no reflection or test hook.

## Verification

The focused Runner contract created a valid disk-backed Validation Set,
constructed the existing Shell command surface, invoked the existing success
branch, and counted the binding notifications:

```text
PASS|successful Validation Set refresh emits one summary and one evidence projection
CONTRACT|validation-set-success-projection|passed=1|failed=0
```

Debug evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl52-validation-set-success-projection-contract-debug-20260909-run2\validation-set-success-projection-contract.txt`

Release evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl52-validation-set-success-projection-contract-release-20260909-run1\validation-set-success-projection-contract.txt`

The OVL-50 evidence-notification and OVL-51 error-projection contracts also
passed in Release (`passed=1|failed=0` for each). Runner and embedded
application Debug/Release builds had 0 warnings/0 errors; readiness and the
refactor audit passed after the source change. After the final documentation
update, the documentation index, JSON parse, and `git diff --check` also
passed.

## Developer reading order

1. `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs` —
   `RefreshValidationSetOptions` success branch.
2. `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs` —
   `RefreshCommandState` shared summary notifier.
3. `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs` —
   `ValidationSetSelectionSummaryText` and `ValidationSuiteSummaryText` binding properties.
4. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetSelectionOwner.cs` —
   selection mutable state.
5. `tools/VisionRecipeRunnerSmoke/ValidationSetSuccessProjectionContract.cs` —
   successful-refresh regression.
6. `tools/OpenVisionReadinessCheck/Program.cs` — static single-owner gate.

One search for `RefreshValidationSetOptions` reaches the selection owner,
binding projection, and shared command-state owner.

## Boundary lock

OVL-52 closes the direct success-branch summary duplication. Other models,
agents, and automations must not recreate, move, split, or wrap this
projection or the OVL-17/46/49/50/51 owners without a new reproducible defect,
explicit binding or Recipe contract change, or dependency boundary conflict.
The separate `RefreshOptions` composite caller and its nested refresh sequence
are a future audit boundary; they are not part of this completed slice.

Runtime Shell EXE refresh, binding rendering, themes, layouts, DPI/monitor,
popup, and full Preview/Run UI were not exercised; the remaining runtime scope
is `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.
