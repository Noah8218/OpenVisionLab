# OpenVisionLab OVL-49 PinArrayGap selection-restoration owner

Status: Complete for one independently verifiable PinArrayGap selection
restoration boundary. The wider refactoring program remains active.

## Scope

`RefreshValidationSetOptions`는 Validation Set XML을 다시 읽은 뒤 선택된
Train/Validation/Test 세트가 비어 있을 때 고정된 PinArrayGap 기록에서 이름을
복원합니다. 이번 재감사에서 이 경로가 이미 완료된
`OpenVisionRecipePinArrayGapValidationIdentityOwner`를 우회해
`OpenVisionRecipePinArrayGapValidationRecordStorage.TryLoad`를 Shell에서 직접
호출하는 잔여 결합을 확인했습니다.

이번 slice는 기록 읽기와 split-name projection만 기존 identity owner로
이동했습니다. Validation Set document/selection mutable state, WPF binding,
`PropertyChanged`, PinArrayGap identity status, evidence/command state,
Recipe/XML 형식, 그리고 명시적 Preview/Run 계약은 변경하지 않았습니다.

## Refactor proof

### Before

- `OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs`의
  `RefreshValidationSetOptions`가
  `OpenVisionRecipePinArrayGapValidationRecordStorage.TryLoad`를 직접
  호출했습니다.
- OVL-46에서 Freeze/Evaluate XML·record orchestration은 identity owner로
  이동했지만, 선택 복원만 Shell이 저장소를 우회해 읽고 있었습니다.

### After

- `OpenVisionRecipePinArrayGapValidationIdentityOwner.TryGetFrozenSelectionNames`
  가 기존 record storage를 읽고 Train/Validation/Test 이름만 반환합니다.
- Shell은 `TryGetFrozenSelectionNames` 결과를 기존
  `previousTrainName`/`previousValidationName`/`previousTestName`에 투영한
  뒤, 동일한 `OpenVisionRecipeValidationSetSelectionOwner.Refresh`와
  `PropertyChanged` 순서를 실행합니다.
- Shell의 직접 record-storage 호출은 제거되었고, 새 interface, wrapper,
  partial, timer, callback, dispose owner는 추가하지 않았습니다.

## Responsibility map and call path

| 항목 | canonical owner | OVL-49 결과 |
| --- | --- | --- |
| `pin-row-edge-gap-v1.xml` 읽기/shape 검증 | `OpenVisionRecipePinArrayGapValidationRecordStorage` | 기존 owner 유지; identity owner 뒤로 숨김 |
| Freeze/Evaluate와 frozen split-name projection | `OpenVisionRecipePinArrayGapValidationIdentityOwner` | `TryGetFrozenSelectionNames` 추가 |
| Validation Set document mutable state와 XML 저장 | `OpenVisionRecipeValidationSetDocumentOwner` / 기존 storage | 유지 |
| 선택·이미지 row mutable state | `OpenVisionRecipeValidationSetSelectionOwner` | 유지 |
| 선택 복원 orchestration과 WPF binding 알림 | `OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs` | 이름 projection 후 기존 순서 유지 |
| PinArrayGap status/evidence/command state | Shell과 기존 identity/evidence owners | 유지 |

```text
RefreshValidationSetOptions
  -> ValidationSetDocumentOwner.TryLoad
  -> PinArrayGapValidationIdentityOwner.TryGetFrozenSelectionNames
  -> ValidationSetSelectionOwner.Refresh(previous split names)
  -> Shell PropertyChanged / variant / identity / evidence / command projection
```

Mutable state write owner는 document 변경을
`OpenVisionRecipeValidationSetDocumentOwner`, selection 변경을
`OpenVisionRecipeValidationSetSelectionOwner`, WPF-facing projection을
Shell이 계속 담당합니다. 새 owner는 WPF 상태나 파일 경로를 노출하지 않고
기존 record의 세 split 이름만 반환합니다.

## Developer reading order

1. `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs` — `RefreshValidationSetOptions`의 선택 복원 호출
2. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipePinArrayGapValidationIdentityOwner.cs` — `TryGetFrozenSelectionNames`, Freeze, Evaluate
3. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipePinArrayGapValidationRecordStorage.cs` — record XML 읽기와 shape 검증
4. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetSelectionOwner.cs` — split/image 선택 mutable state
5. `tools/VisionRecipeRunnerSmoke/PinArrayGapValidationIdentityOwnerContract.cs` — frozen split-name restoration contract
6. `tools/OpenVisionReadinessCheck/Program.cs` — Shell direct storage call 제거 gate

한 번의 검색은 `TryGetFrozenSelectionNames`입니다. 이 검색으로 새 호출
경계와 record storage의 유일한 호출 owner를 함께 확인할 수 있습니다.

## Focused verification

모든 계약 산출물은 D:에 저장했습니다.

- `dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform="Any CPU" --no-restore -m:1 -nr:false` — 0 warnings, 0 errors.
- 같은 Runner Release 빌드 — 0 warnings, 0 errors.
- Runner Debug `--pinarraygap-validation-identity-owner-contract` — `passed=5|failed=0`; evidence `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl49-pinarraygap-validation-identity-owner-contract-debug-20260909-run1`.
- Runner Release 같은 계약 — `passed=5|failed=0`; evidence `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl49-pinarraygap-validation-identity-owner-contract-release-20260909-run1`.
- `dotnet build src\OpenVisionLab\OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:OpenVisionLabEnableEmbeddedSmokeRunner=true --no-restore -m:1 -nr:false` — 0 warnings, 0 errors.
- 같은 OpenVisionLab Release 빌드 — 0 warnings, 0 errors.
- `OpenVisionReadinessCheck` — pass; Validation Set Shell의 직접 PinArrayGap record load 제거와 identity-owner delegation을 확인했습니다.
- `Invoke-RefactorAudit.ps1 -Verify` — `REFACTOR_AUDIT=PASS|CSharpFiles=833|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`; evidence `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl49-refactor-audit-20260909-run1`.
- 선택 복원 호출과 기존 Freeze/Evaluate 호출은 `rg`로 재확인했으며, Shell ValidationSets 파일에 `OpenVisionRecipePinArrayGapValidationRecordStorage.TryLoad`가 남아 있지 않습니다.

## Runtime boundary and discoverability assessment

이번 source/contract slice는 실제 Shell EXE의 Validation Set 선택 변경,
ComboBox transient-null, binding, theme/layout, DPI/monitor, popup 및 전체
Preview/Run UI를 실행하지 않았습니다. 따라서
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`이며, 이 범위 밖의
UI 회귀는 남은 위험입니다.

Developer discoverability self-assessment: **PASS for this boundary**.
기록 저장소는 identity owner 뒤에 있고, split 선택은 selection owner,
WPF 투영은 Shell에서 순서대로 읽을 수 있습니다.

OVL-49는 OVL-46 identity owner의 완료 조건에서 발견된 잔여 직접 호출을
해결한 기록입니다. 새 PinArrayGap 결함·명시적 계약 변경·입증된 dependency
충돌 없이는 다른 모델·agent·automation이 이 owner를 재생성·이동·재분할·
복제하지 않습니다. 파일 길이, 모델 선호, 반복 요청은 재개 사유가 아닙니다.

## Closure record

Status: Complete

Scope: PinArrayGap frozen Train/Validation/Test split-name restoration now
routes through the existing identity owner while preserving Validation Set
selection, binding, Recipe/XML, and explicit Preview/Run behavior.

Acceptance criteria: Shell direct record-storage load removed; concrete
identity-owner call path active; selection restoration contract passes 5/5 in
Debug and Release; Runner/app builds, readiness, and refactor audit pass.

Verification: Debug/Release Runner builds and contracts, embedded app
Debug/Release builds, readiness, refactor audit, and static call-path search.

Evidence: the two `ovl49-pinarraygap-validation-identity-owner-contract-*`
D: folders and `ovl49-refactor-audit-20260909-run1`.

Boundary / next dependency: Shell `PropertyChanged` and evidence projection
remain application-owned because they update the existing binding contract and
command state together. They require a new independently testable result seam
or reproducible defect before another split. Original promotion, commit, push,
and scheduled execution remain outside this execution.

Recommended model for the next slice: `gpt-5.6-terra`; reasoning effort:
`high`.
