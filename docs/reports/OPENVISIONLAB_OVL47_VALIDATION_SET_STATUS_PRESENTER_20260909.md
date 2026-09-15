# OpenVisionLab OVL-47 Validation Set status presenter owner

Status: Complete for one independently verifiable Validation Set folder-registration status boundary. The wider refactoring program remains active.

## Scope

`AddValidationSetFolder`의 파일 열거, 문서 변경, 저장, 상태 문구가 한 Shell
호출 경로에 이어져 있었습니다. 이번 slice는 기존
`OpenVisionRecipeValidationSetPresenter`를 재사용해 폴더 등록 오류, 빈 폴더,
이미지 수량 결과의 localized 문구 조합을 그 presenter가 소유하도록 이동했습니다.

파일 열거는 기존 `OpenVisionRecipeValidationSetStorage.TryGetTopLevelImagePaths`,
문서의 mutable 변경은 기존 `OpenVisionRecipeValidationSetDocumentOwner.TryAddImages`,
저장은 기존 `TrySaveValidationSetDocument`가 계속 소유합니다. Recipe/XML,
Validation Set schema, 명시적 Preview/Run 및 화면 routing은 변경하지 않았습니다.

## Refactor proof

### Before

- `OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs`가 폴더 열거 결과와
  이미지 추가 결과를 직접 localized 문자열로 조합했습니다.
- 같은 호출 경로에서 storage/document owner 호출과 `ValidationSuiteStatusText`
  projection이 함께 보였습니다.

### After

- `OpenVisionRecipeValidationSetPresenter.BuildFolderImageRegistrationError`가
  폴더 열거 실패 prefix와 오류를 조합합니다.
- `BuildEmptyFolderImageRegistrationStatus`가 빈 폴더 상태를, 
  `BuildImageRegistrationStatus`가 추가/갱신/건너뜀 수량을 조합합니다.
- Shell은 storage/document/save 호출 결과를 받아 기존
  `ValidationSuiteStatusText`에 대입하는 역할만 수행합니다. 파일 수집과 문서
  변경 owner는 이동하거나 복제하지 않았습니다.

## Responsibility map and call path

| 항목 | 현재 canonical owner | 이번 경계 |
| --- | --- | --- |
| 지원 이미지 파일 열거 | `OpenVisionRecipeValidationSetStorage` | 유지 |
| Validation Set 이미지 mutable 변경 | `OpenVisionRecipeValidationSetDocumentOwner` | 유지 |
| 저장 및 refresh 순서 | `OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs` | 유지 |
| 폴더/수량 결과 문구 | `OpenVisionRecipeValidationSetPresenter` | OVL-47에서 명시적 owner |
| binding 상태와 명시적 Preview/Run routing | `OpenVisionShellHostRecipeCommandSurface` | 유지 |
| lifetime/callback/dispose | 기존 Shell/document owner | 변경 없음 |

```text
AddValidationSetFolder
  -> OpenVisionRecipeValidationSetStorage.TryGetTopLevelImagePaths
  -> OpenVisionRecipeValidationSetDocumentOwner.TryAddImages
  -> TrySaveValidationSetDocument
  -> RefreshValidationSetOptions
  -> OpenVisionRecipeValidationSetPresenter.BuildImageRegistrationStatus
  -> Shell ValidationSuiteStatusText binding
```

오류와 빈 폴더 경로는 각각 presenter의 전용 builder를 거쳐 Shell 상태에
투영됩니다. 새 presenter 메서드는 WPF, Window, 파일 시스템, 네트워크를
참조하지 않는 순수 문자열 정책이며, mutable state와 release owner를
추가하지 않습니다.

## Developer reading order

1. `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs` — `AddValidationSetFolder`, `AddValidationSetImages`, `TrySaveValidationSetDocument`와 기존 binding projection
2. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeValidationSetPresenter.cs` — `BuildFolderImageRegistrationError`, `BuildEmptyFolderImageRegistrationStatus`, `BuildImageRegistrationStatus`
3. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetStorage.cs` — `TryGetTopLevelImagePaths`
4. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetDocumentOwner.cs` — `TryAddImages`
5. `tools/VisionRecipeRunnerSmoke/ValidationSetStatusPresenterContract.cs` — Korean/English message contract
6. `tools/OpenVisionReadinessCheck/Program.cs` — presenter delegation과 Shell literal 제거 정적 gate

한 번의 검색은 `BuildImageRegistrationStatus`입니다. 상태 문구의 owner와
파일/문서 변경 owner를 구분해서 확인할 수 있습니다.

## Focused verification

모든 test evidence는 D:에 저장했습니다.

- `dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform="Any CPU" --no-restore -m:1 -nr:false` — 0 warnings, 0 errors.
- 같은 Runner Release 빌드 — 0 warnings, 0 errors.
- `VisionRecipeRunnerSmoke.dll --validation-set-status-presenter-contract` — Debug `passed=2|failed=0`, evidence `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl47-validation-set-status-presenter-contract-debug-20260909-run2`.
- 같은 Release 계약 — `passed=2|failed=0`, evidence `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl47-validation-set-status-presenter-contract-release-20260909-run2`.
- `dotnet build src\OpenVisionLab\OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:OpenVisionLabEnableEmbeddedSmokeRunner=true --no-restore -m:1 -nr:false` — 0 warnings, 0 errors.
- 같은 OpenVisionLab Release 빌드 — 0 warnings, 0 errors.
- `OpenVisionReadinessCheck` — pass; Shell의 폴더/빈 폴더/수량 문구 직접 조합이 제거되고 presenter delegation이 확인됨.
- `Invoke-RefactorAudit.ps1 -Verify` — `REFACTOR_AUDIT=PASS|CSharpFiles=833|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`; evidence `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl47-refactor-audit-20260909-run1`.
- `TestDocumentationIndex.ps1` — `DocumentationIndex=PASS IndexedPaths=266 Routes=13 RootRedirects=102`; JSON parse에서 13 route/54 reference와 OVL-47 status-route 포함을 확인했습니다.
- `git diff --check` — pass.

## Runtime boundary and junior assessment

이번 source/contract slice는 실제 Shell EXE의 폴더 선택 click, binding,
theme/layout, DPI/monitor, popup 및 전체 Validation Set Preview/Run UI를
실행하지 않았습니다. 따라서 `소스 코드 기준 검토 완료 / 실제 Runtime UI
검증 필요`입니다. 기존 runtime evidence와 이번 WPF-free contract가 확인한
범위 밖의 UI 회귀는 남은 위험입니다.

Junior developer self-assessment: **PASS for this boundary**. 한 이름의
presenter에서 상태 문구를 찾고, Shell에서 storage/document/save 호출 순서를
따라가며, 각 mutable owner를 별도로 읽을 수 있습니다.

이 status presenter owner는 새 결함, 명시적 문구/계약 변경, 또는 입증된
dependency 충돌 없이는 다른 모델·agent·automation이 재생성·이동·재분할·
복제하지 않습니다. 파일 길이, 모델 선호, 반복 요청은 재개 사유가 아닙니다.

## Closure record

Status: Complete

Scope: Validation Set folder-registration error, empty-folder, and image-count
status wording owner separation while preserving storage, document mutation,
save, Recipe/XML, and Preview/Run contracts.

Acceptance criteria: Existing presenter owns all three message families; Shell
delegates without duplicated localized literals; storage/document/save owners and
bindings remain active; Debug/Release focused contract, app builds, readiness,
and refactor audit pass.

Verification: Runner Debug/Release builds and 2/2 contracts, embedded app
Debug/Release builds, readiness, refactor audit, documentation index/JSON parse,
and `git diff --check`.

Evidence: the two `ovl47-validation-set-status-presenter-contract-*` D: folders
and `ovl47-refactor-audit-20260909-run1`.

Boundary / next dependency: This slice does not move `TrySaveValidationSetDocument`
or `RefreshValidationSetOptions`; their failure/status transition remains the next
single audit boundary. OVL-01/02/03/04/05/06a/07/08/10/12-47 completed owners
remain closed without new evidence. Original promotion, commit, and push remain
outside this execution.

Recommended model for the next slice: `gpt-5.6-terra`; reasoning effort: `high`.
