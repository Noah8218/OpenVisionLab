# OpenVisionLab OVL-48 Validation Set save-failure status owner

Status: Complete for one independently verifiable Validation Set save-failure status boundary. The wider refactoring program remains active.

## Scope

`TrySaveValidationSetDocument`는 기존 `OpenVisionRecipeValidationSetDocumentOwner`
저장 결과를 받아 실패 시 `RefreshValidationSetOptions`로 디스크 기준 상태를
복원하고 Shell binding에 오류를 투영합니다. 이번 audit에서 이 복원·선택·
`PropertyChanged`·evidence 갱신은 Shell의 mutable UI state owner라서 별도
recovery 객체로 분리할 독립 seam이 없음을 확인했습니다.

대신 저장 실패 문구의 조합만 기존
`OpenVisionRecipeValidationSetPresenter.BuildSaveErrorStatus`로 이동했습니다.
저장 호출, 실패 후 refresh 순서, Validation Set document/XML schema와 명시적
Preview/Run 계약은 변경하지 않았습니다.

## Refactor proof

### Before

- `OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs`가 저장 실패 후
  `operation + " ERROR: " + error`를 직접 조합했습니다.
- 같은 메서드가 persistence 호출, refresh orchestration, binding 상태 투영을
  함께 이어 주었습니다.

### After

- `OpenVisionRecipeValidationSetPresenter.BuildSaveErrorStatus`가 저장 실패
  문구 조합을 소유합니다.
- Shell은 `validationSetDocumentOwner.TrySave`를 호출하고 실패 후
  `RefreshValidationSetOptions`를 실행한 뒤 presenter 결과를
  `ValidationSuiteStatusText`에 대입합니다.
- `OpenVisionRecipeValidationSetStorage`와
  `OpenVisionRecipeValidationSetDocumentOwner`는 각각 파일 persistence와
  mutable document를 계속 소유합니다. 새 interface, wrapper, partial, timer,
  callback, dispose owner는 추가하지 않았습니다.

## Responsibility map and call path

| 항목 | canonical owner | OVL-48 결과 |
| --- | --- | --- |
| validation-sets.xml 저장/검증 | `OpenVisionRecipeValidationSetStorage` | 유지 |
| document mutable state와 `TrySave` 위임 | `OpenVisionRecipeValidationSetDocumentOwner` | 유지 |
| 저장 실패 후 디스크 상태 재로드, 선택 복원, `PropertyChanged`/evidence 갱신 | `OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs` | Shell에 유지; 독립 owner 분리하지 않음 |
| 저장 실패 문구 정책 | `OpenVisionRecipeValidationSetPresenter` | `BuildSaveErrorStatus`로 명시화 |
| Validation Set binding 및 Preview/Run routing | `OpenVisionShellHostRecipeCommandSurface` | 유지 |

```text
Create/Delete/Add/Variant/Repair/Remove command
  -> TrySaveValidationSetDocument(operation)
  -> ValidationSetDocumentOwner.TrySave
  -> (failure) RefreshValidationSetOptions
  -> ValidationSetPresenter.BuildSaveErrorStatus
  -> Shell ValidationSuiteStatusText binding
```

오류 복원 경계는 Shell이 `RefreshValidationSetOptions` 안에서 선택 owner,
PinArrayGap identity 상태, evidence 알림과 command state를 함께 갱신하기
때문에 순수 WPF-free owner로 이동할 수 없습니다. 이를 새 클래스로 감싸면
동일 책임이 복제되고 호출 경계가 흐려집니다.

## Developer reading order

1. `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs` — `TrySaveValidationSetDocument`와 실패 후 `RefreshValidationSetOptions`
2. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeValidationSetPresenter.cs` — `BuildSaveErrorStatus`와 기존 상태 builder
3. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetDocumentOwner.cs` — `TrySave`와 document state
4. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetStorage.cs` — XML validation/persistence
5. `tools/VisionRecipeRunnerSmoke/ValidationSetStatusPresenterContract.cs` — 한국어/영어 저장 실패 문구 계약
6. `tools/OpenVisionReadinessCheck/Program.cs` — Shell의 직접 저장 오류 조합 제거 정적 gate

한 번의 검색은 `BuildSaveErrorStatus`입니다. 저장 실패 문구 owner와
refresh/mutable state owner를 분리해서 확인할 수 있습니다.

## Focused verification

모든 test evidence는 D:에 저장했습니다.

- `dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform="Any CPU" --no-restore -m:1 -nr:false` — 0 warnings, 0 errors.
- 같은 Runner Release 빌드 — 0 warnings, 0 errors.
- `VisionRecipeRunnerSmoke.dll --validation-set-status-presenter-contract` — Debug `passed=2|failed=0`, evidence `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl48-validation-set-status-presenter-contract-debug-20260909-run1`.
- 같은 Release 계약 — `passed=2|failed=0`, evidence `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl48-validation-set-status-presenter-contract-release-20260909-run1`.
- `dotnet build src\OpenVisionLab\OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:OpenVisionLabEnableEmbeddedSmokeRunner=true --no-restore -m:1 -nr:false` — 0 warnings, 0 errors.
- 같은 OpenVisionLab Release 빌드 — 0 warnings, 0 errors.
- `OpenVisionReadinessCheck` — pass; Presenter의 save-error builder와 Shell delegation, 직접 문자열 조합 제거를 확인했습니다.
- `Invoke-RefactorAudit.ps1 -Verify` — `REFACTOR_AUDIT=PASS|CSharpFiles=833|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`; evidence `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl48-refactor-audit-20260909-run1`.
- `TestDocumentationIndex.ps1` — `DocumentationIndex=PASS IndexedPaths=267 Routes=13 RootRedirects=102`; JSON parse에서 13 route/55 reference와 OVL-48 status-route 포함을 확인했습니다.
- `git diff --check` — pass; 기존 작업 트리의 LF/CRLF 변환 경고만 출력되었습니다.

## Runtime boundary and discoverability assessment

이번 source/contract slice는 실제 Shell EXE의 저장 실패 유도, 폴더 선택,
binding, theme/layout, DPI/monitor와 전체 Validation Set Preview/Run UI를
실행하지 않았습니다. 따라서 `소스 코드 기준 검토 완료 / 실제 Runtime UI
검증 필요`입니다.

Developer discoverability self-assessment: **PASS for this boundary**.
저장 실패 문구는 Presenter에서 찾고, refresh와 mutable state는 Shell에서,
XML persistence는 기존 DocumentOwner/Storage에서 순서대로 읽을 수 있습니다.

이 save-status owner와 기존 OVL-47 presenter owner는 새 결함·명시적 문구/계약
변경·입증된 dependency 충돌 없이는 다른 모델·agent·automation이 재생성·이동·
재분할·복제하지 않습니다. 파일 길이, 모델 선호, 반복 요청은 재개 사유가
아닙니다.

## Closure record

Status: Complete

Scope: Validation Set save-failure message policy moved to the existing presenter;
Shell refresh/mutable-state recovery remains the canonical application owner.

Acceptance criteria: save-error wording has one concrete presenter owner; Shell
direct concatenation is removed; storage/document/refresh/binding and
Recipe/XML/Preview/Run contracts remain active; focused Debug/Release contract,
app builds, readiness, and audit pass.

Verification: Runner Debug/Release builds and 2/2 contracts, embedded app
Debug/Release builds, readiness, refactor audit, documentation index/JSON parse,
and `git diff --check`.

Evidence: the two `ovl48-validation-set-status-presenter-contract-*` D: folders
and `ovl48-refactor-audit-20260909-run1`.

Boundary / next dependency: `RefreshValidationSetOptions` remains an application
projection boundary and is not split without a new independently testable result
contract or reproducible defect. OVL-01/02/03/04/05/06a/07/08/10/12-48 completed
owners remain closed. Original promotion, commit, and push remain outside this
execution.

Recommended model for the next slice: `gpt-5.6-terra`; reasoning effort: `high`.
