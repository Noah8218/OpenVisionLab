# OpenVisionLab OVL-51 — Validation Set projection error branch

Status: **Complete** for one independently verifiable error-state projection
fix. The broader Shell validation and Step Edit refactoring program remains
active.

## Confirmed defect and smallest change

`RefreshValidationSetOptions` clears the existing Validation Set selection when
`validation-sets.xml` cannot be loaded. The error branch raised option, image,
Variant, identity, evidence, and command-state notifications, but omitted
`ValidationSetSelectionSummaryText`. A bound summary could therefore keep the
previous successful selection text after a load failure.

The error branch now raises `ValidationSetSelectionSummaryText` immediately
after the cleared row selection. The existing Shell projection remains the
owner; no new service, interface, wrapper, partial, or presenter was added.

## Ownership and call path

| Boundary | Current owner | Intended owner | Evidence |
| --- | --- | --- | --- |
| Validation Set document/XML | `OpenVisionRecipeValidationSetDocumentOwner` -> existing storage | unchanged | `TryLoad` reports the failure and clears the document state |
| Selection and image rows | `OpenVisionRecipeValidationSetSelectionOwner` | unchanged | `Clear` owns mutable reset state |
| Error-state binding projection | `OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs` | unchanged | error branch now raises the selection summary binding |
| Evidence policy text | `OpenVisionRecipeValidationEvidenceOwner` and existing presenter | unchanged | policy and user-facing text remain outside the error projection change |

Actual path:

```text
RefreshValidationSetOptions
  -> ValidationSetDocumentOwner.TryLoad
  -> (failure) SelectionOwner.Clear
  -> Shell PropertyChanged for options/selection/rows/summary/Variant
  -> status, identity, evidence, and command-state projection
```

The document owner writes `StorageReady`; the selection owner writes cleared
mutable state; the Shell owns WPF notification order. Recipe/XML persistence,
binding names, Preview/Run explicit execution, and successful refresh behavior
remain unchanged.

## Binding contract

The existing `ValidationSetSelectionSummaryText` binding now receives exactly
one notification when a previously valid document becomes unreadable. Its
getter returns the existing localized `validation-sets.xml could not be read`
guidance, and the option list is empty. No automatic repair, overwrite, or
Preview/Run action is introduced.

## Verification

The focused Runner contract created a valid disk-backed Validation Set, loaded
the existing Shell command surface, replaced its XML with malformed content,
called `RefreshOptions`, and verified one summary notification plus the cleared
option state:

```text
PASS|validation-set load failure refreshes the selection summary binding
CONTRACT|validation-set-projection-error|passed=1|failed=0
```

Debug evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl51-validation-set-projection-error-contract-debug-20260909-run1\validation-set-projection-error-contract.txt`

Release evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl51-validation-set-projection-error-contract-release-20260909-run1\validation-set-projection-error-contract.txt`

The existing OVL-50 valid-selection notification contract also passed in
Release (`passed=1|failed=0`). Runner and embedded application Debug/Release
builds had 0 warnings/0 errors; readiness and the refactor audit passed after
the source change. After the final documentation update, the documentation
index, JSON parse, and `git diff --check` also passed.

## Developer reading order

1. `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs` —
   `RefreshValidationSetOptions` error branch.
2. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetDocumentOwner.cs` —
   storage-ready and document ownership.
3. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetSelectionOwner.cs` —
   cleared selection state.
4. `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs` —
   `ValidationSetSelectionSummaryText` binding property.
5. `tools/VisionRecipeRunnerSmoke/ValidationSetProjectionErrorContract.cs` —
   malformed-XML regression.
6. `tools/OpenVisionReadinessCheck/Program.cs` — static error-branch gate.

One search for `RefreshValidationSetOptions` reaches the load owner, reset owner,
and Shell projection.

## Boundary lock

OVL-51 closes the missing error-state summary notification. Other models,
agents, and automations must not recreate, move, split, or wrap this projection
or the OVL-17/46/49/50 owners without a new reproducible defect, explicit
binding or Recipe contract change, or dependency boundary conflict. File
length, model preference, and repeated continuation requests are not reopening
reasons.

Runtime Shell EXE malformed-file recovery, binding rendering, themes, layouts,
DPI/monitor, popup, and full Preview/Run UI were not exercised; the remaining
runtime scope is `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.
