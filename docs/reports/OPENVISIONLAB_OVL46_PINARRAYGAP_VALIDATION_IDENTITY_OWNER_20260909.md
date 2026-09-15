# OpenVisionLab OVL-46 PinArrayGap validation identity owner

Status: Complete for one independently verifiable Shell validation identity
boundary. The wider refactoring program remains active.

## Scope

`OpenVisionShellHostRecipeCommandSurface.Handlers.cs`의 PinArrayGap 2단계
검증 기준 고정/상태 갱신 경로는 선택 Pipeline XML 원문을 읽고 기존
`OpenVisionRecipePinArrayGapValidationRecordStorage`에 저장·조회·비교하는
책임을 함께 가지고 있었습니다. 이번 slice는 그 orchestration을
`OpenVisionRecipePinArrayGapValidationIdentityOwner` concrete owner로
이동했습니다.

기존 Train/Validation/Test 불변성·파일 해시·Pipeline XML 해시,
`PHASE 2 FROZEN`/`STALE` 상태, Recipe/XML 형식, 명시적 Preview/Run과
Validation Set 화면 routing은 변경하지 않았습니다. Shell은 선택 상태,
`CanExecute`, localized status, `IsPinArrayGapValidationIdentityFrozen`와
기존 명시적 실행 callback을 계속 소유합니다.

## Refactor proof

### Before

- `FreezePinArrayGapValidationIdentity`가 `RecipeWorkspaceService` 경로,
  `File.Exists`/`File.ReadAllText`, record storage 저장을 직접 조합했습니다.
- `RefreshPinArrayGapValidationIdentityState`가 같은 XML 읽기를 반복하고,
  record load와 `TryMatchesCurrent` 호출 및 missing/stale 분기를 직접
  조합했습니다.
- 이 두 경로는 Shell의 mutable selection/status state와 raw XML/storage
  orchestration을 한 partial에 함께 보유했습니다.

### After

- `OpenVisionRecipePinArrayGapValidationIdentityOwner.Freeze`가 선택
  Pipeline XML 원문을 읽고 기존 record storage에 identity를 저장합니다.
- `OpenVisionRecipePinArrayGapValidationIdentityOwner.Evaluate`가 같은
  원문을 읽어 저장 record와 현재 Train/Validation/Test·Pipeline을 비교하고
  `OpenVisionRecipePinArrayGapValidationIdentityResult`를 반환합니다.
- CommandSurface는 결과를 기존 status 문자열과 frozen flag에 투영하며,
  Validation Set 선택·편집·저장과 명시적 실행 경계는 그대로 유지합니다.

## Responsibility map

| 항목 | 현재 owner | 의도한 owner / 경계 |
| --- | --- | --- |
| PinArrayGap 선택/실행 가능 상태와 localized status | `OpenVisionShellHostRecipeCommandSurface.Handlers.cs` | Shell이 기존 binding/command 상태를 계속 소유 |
| Pipeline XML 원문 읽기와 Freeze/Evaluate orchestration | 기존 Shell handlers | `OpenVisionRecipePinArrayGapValidationIdentityOwner` |
| XML 구조·Row·split hash 생성과 record persistence | `OpenVisionRecipePinArrayGapValidationRecordStorage` | 기존 storage owner 유지 |
| Train/Validation/Test mutable selection | `OpenVisionRecipeValidationSetDocumentOwner` 및 `OpenVisionRecipeValidationSetSelectionOwner` | 변경 없음 |
| Preview/Run 및 Validation Set 화면 routing | 기존 CommandSurface callback/runner | 변경 없음 |
| lifetime | owner는 호출별 결과만 반환 | dispose/callback/timer 변경 없음 |

실제 call path:

```text
Freeze command
  -> FreezePinArrayGapValidationIdentity
  -> PinArrayGapValidationIdentityOwner.Freeze
  -> RecipeWorkspaceService + raw Pipeline XML read
  -> PinArrayGapValidationRecordStorage.TrySave
  -> Shell frozen status/command-state projection

Selection/Pipeline refresh
  -> RefreshPinArrayGapValidationIdentityState
  -> PinArrayGapValidationIdentityOwner.Evaluate
  -> raw Pipeline XML read + record TryLoad/TryMatchesCurrent
  -> Shell NOT FROZEN / REVIEW / STALE / FROZEN status projection
```

Dependency direction은 `Shell handler -> identity owner -> existing
RecipeWorkspaceService/PinArrayGapValidationRecordStorage`입니다. 새 owner는
WPF, Window, Clipboard, execution session, Layer/ImageSpace를 참조하지
않습니다. XML 원문을 그대로 읽어 기존 hash 계약을 보존하며, Pipeline을
재직렬화해 hash를 바꾸지 않습니다.

## Developer reading order

1. `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs` — `FreezePinArrayGapValidationIdentity`, `RefreshPinArrayGapValidationIdentityState`, status projection
2. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipePinArrayGapValidationIdentityOwner.cs` — raw XML read와 Freeze/Evaluate result contract
3. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipePinArrayGapValidationRecordStorage.cs` — Pipeline/row/split hash와 XML record persistence
4. `tools/VisionRecipeRunnerSmoke/PinArrayGapValidationIdentityOwnerContract.cs` — freeze/current/stale/missing XML contract
5. `tools/OpenVisionReadinessCheck/Program.cs` — Shell direct orchestration 제거와 owner delegation 정적 gate

한 번의 검색은 `pinArrayGapValidationIdentityOwner.Freeze` 또는
`pinArrayGapValidationIdentityOwner.Evaluate`입니다. mutable selection/status와
`Preview/Run` 명시 계약은 Shell에서, hash/persistence는 기존 storage에서
확인합니다.

## Verification evidence

모든 계약 산출물은 D:에 저장했습니다.

- `dotnet build tools\\VisionRecipeRunnerSmoke\\VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform="Any CPU" --no-restore -m:1 -nr:false` — 0 warnings, 0 errors.
- `dotnet build tools\\VisionRecipeRunnerSmoke\\VisionRecipeRunnerSmoke.csproj -c Release -p:Platform="Any CPU" --no-restore -m:1 -nr:false` — 0 warnings, 0 errors.
- `VisionRecipeRunnerSmoke --pinarraygap-validation-identity-owner-contract` — Debug `4/4` pass at `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl46-pinarraygap-validation-identity-owner-contract-debug-20260909-run1`; Release `4/4` pass at `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl46-pinarraygap-validation-identity-owner-contract-release-20260909-run1`.
- Contract cases: Freeze persistence, unchanged identity match, changed image stale detection, missing Pipeline XML failure with saved XML unchanged.
- `dotnet build src\\OpenVisionLab\\OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:OpenVisionLabEnableEmbeddedSmokeRunner=true --no-restore -m:1 -nr:false` — 0 warnings, 0 errors.
- `dotnet build src\\OpenVisionLab\\OpenVisionLab.csproj -c Release -p:Platform=x64 -p:OpenVisionLabEnableEmbeddedSmokeRunner=true --no-restore -m:1 -nr:false` — 0 warnings, 0 errors.
- `OpenVisionReadinessCheck` — pass; Shell handler no longer contains direct PinArrayGap XML/record orchestration and the new owner is WPF-independent.
- `Invoke-RefactorAudit.ps1 -Verify` — `REFACTOR_AUDIT=PASS|CSharpFiles=832|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
- `TestDocumentationIndex.ps1 -RepoRoot C:\\Git\\2D\\Dev` — `DocumentationIndex=PASS IndexedPaths=265 Routes=13 RootRedirects=102`; JSON parse and `git diff --check` also passed.

## Runtime boundary and remaining work

이번 source/contract slice는 실제 Shell EXE의 PinArrayGap panel을 띄우는 UI
검증이 아닙니다. Freeze 버튼의 physical click, selected-set binding,
localized theme/layout, alternate DPI/monitor, Validation Set 화면의 전체
Preview/Run 흐름은 이번 실행에서 재실행하지 않았습니다. 기존 runtime
증거와 이번 raw XML/hash 계약이 적용되는 범위만 확인했습니다.

Junior developer self-assessment: **PASS for this boundary**. Freeze와
refresh의 읽기 경로가 한 이름의 owner로 모이고, 기존 record storage와
Shell mutable state가 분리되어 있습니다.

이 owner는 새 PinArrayGap identity 결함, 검토 계약 변경, 또는 입증된
dependency 충돌 없이는 다른 모델·agent·automation이 재생성·이동·재분할·
복제하지 않습니다. 파일 길이, 모델 선호, 반복 요청은 재개 사유가 아닙니다.

## Closure record

Status: Complete
Scope: Shell PinArrayGap Validation Identity raw Pipeline XML read and
Freeze/Evaluate orchestration owner separation.
Acceptance criteria: Shell direct XML/record orchestration removed; concrete
WPF-free owner call path active; Train/Validation/Test and Recipe/XML hash
contracts preserved; Debug/Release contract and app builds pass.
Verification: Debug/Release runner builds and 4/4 contracts, embedded app
Debug/Release builds, readiness, refactor audit, documentation index, JSON
parse, static structure, and `git diff --check`.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl46-pinarraygap-validation-identity-owner-contract-debug-20260909-run1`, `...release-20260909-run1`, and `ovl46-refactor-audit-20260909-run1`.
Boundary / next dependency: the next single slice is a residual Validation Set
image-folder ingestion or status projection boundary only after its mutable
document/selection owner is proven. Completed OVL-01/02/03/04/05/06a/07/08/
10/12-46 owners remain closed; Original and push remain outside this execution.

Recommended model for the next slice: `gpt-5.6-terra`; reasoning effort: `high`.
