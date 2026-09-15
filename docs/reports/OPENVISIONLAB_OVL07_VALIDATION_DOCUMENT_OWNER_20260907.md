# OpenVisionLab OVL-07 Validation Set 문서 owner 분리

작성일: 2026-09-07 KST

## 의미와 범위

이번 실행은 OVL-07의 독립 검증 가능한 한 가지 작업으로,
`Local Validation Set` 문서의 mutable 상태·저장·생성·삭제·이미지 변경 책임을
`OpenVisionShellHostRecipeCommandSurface`에서 구체적인 문서 owner로 이동했다.
Shell은 XAML binding, command, 선택 상태, 상태 메시지와 화면용 projection을
facade로 계속 소유한다. Validation Set 실행 loop는 이전 OVL-07 slice의
`OpenVisionRecipeValidationSetRunner`가 계속 소유한다.

Recipe/XML 호환성과 Preview/Run의 명시적 실행 계약은 변경하지 않았다. 이번
slice에서는 새 기능, schema 변경, UI 전면 개편을 포함하지 않았다.

## 구조 변화

| 항목 | 이전 | 현재 |
| --- | --- | --- |
| 문서 상태 owner | Shell의 `validationSetDocument`와 `validationSetStorageReady` 필드 | `OpenVisionRecipeValidationSetDocumentOwner`가 문서와 storage-ready 상태를 소유 |
| 생성·삭제·이미지 CRUD | `OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs`가 DTO와 Storage를 직접 변경 | Shell command가 owner의 `TryCreateSet`, `TryDeleteSet`, `TryAddImages`, `TryApplyVariantContract`, `TryRepairMissingImagePath`, `TryRemoveImage`를 호출 |
| 저장·로드 | Shell이 `OpenVisionRecipeValidationSetStorage.TryLoad/TrySave`를 직접 호출 | owner가 기존 Storage 계약을 호출하고 Shell은 결과·상태만 투영 |
| 카탈로그 쌍 가져오기 | Shell이 문서를 직접 `Import`에 전달 | owner가 기존 `OpenVisionRecipeCatalogPairValidationSetService`를 호출 |
| 선택/화면 projection | Shell과 기존 Presenter | owner가 문서 기반 option selection을 만들고 Shell은 image-row projection과 binding을 유지 |
| 실행 책임 | 기존 OVL-07 runner slice | 변경 없음. 실행 상태는 기존 `OpenVisionRecipeExecutionSessionViewModel`이 단일 owner로 유지 |

## 호출 경로와 경계

`WPF command → Shell facade → OpenVisionRecipeValidationSetDocumentOwner →`
`기존 Storage/CatalogPair 서비스 → Shell의 status/binding refresh`

owner는 `Window`, `UserControl`, `System.Windows`, Shell 타입을 참조하지
않는다. XML DTO, schema version 1, `VISION/ValidationSets/validation-sets.xml`
경로, set/image limits, identity-lock 및 Variant 계약 검증은 기존
`OpenVisionRecipeValidationSetStorage`에 위임했다. 폴더 선택을 위한
`TryGetTopLevelImagePaths` 호출은 파일 목록을 수집하는 UI 입력 경계이므로 Shell에
남겼고 문서 mutation은 owner를 통해서만 수행한다.

## 변경 파일

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetDocumentOwner.cs`
  - 문서 상태, 저장/로드, set 생성·삭제, 이미지 추가·수정·복구·삭제와 option
    selection 진입점을 소유하는 concrete owner를 추가했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs`
  - Shell의 문서 DTO와 storage-ready 필드를 owner dependency로 교체했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs`
  - command guard가 owner의 storage-ready 상태를 사용하도록 연결했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs`
  - 문서 직접 mutation/storage 호출을 owner 호출과 기존 UI 상태 투영으로 바꿨다.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - Window 없이 owner를 생성하고 create/save/reload/add/remove/delete 및 normalized
    image projection을 확인하는 `--validation-set-document-owner-contract`를 추가했다.

## 구조 증거

- Shell과 handler에서 이전 `validationSetDocument`, `validationSetStorageReady`
  필드가 제거됐다.
- `OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs`에는 문서의
  `TryLoad`, `TrySave`, `AddOrUpdateImages`, Variant/repair Storage 직접 호출과
  Catalog `Import` 직접 호출이 남아 있지 않다.
- owner에는 위 mutation/persistence 메서드가 있고 Shell/WPF 참조가 없다.
- Shell은 기존 command/binding/selection/status façade와 image-row projection을
  유지하며, 별도 mutable 문서 복사본이나 실행 상태 복사본을 만들지 않는다.
- 정적 검사 결과는
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-validation-document-owner-20260907\static-document-owner-check.log`에 기록했다.

## 검증

- Debug app build:
  `dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Debug -p:Platform=x64 --no-restore -m:1 -nr:false`
  - 0 warnings / 0 errors.
- Debug `VisionRecipeRunnerSmoke` build:
  - 0 warnings / 0 errors.
- Release app build:
  - 0 warnings / 0 errors.
- Release `PipelineViewerScreenshotSmoke` build:
  - 0 warnings / 0 errors.
- Release `VisionRecipeRunnerSmoke` build:
  - 0 warnings / 0 errors.
- `--validation-set-document-owner-contract` Debug/Release:
  - owner를 Window 없이 생성하고 validation-sets.xml 저장·재로드,
    duplicate 거부, 이미지 add/projection, remove, set delete를 모두 PASS했다.
  - 결과 파일: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-validation-document-owner-20260907\document-owner-contract-final-debug\validation_set_document_owner_contract.txt`,
    `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-validation-document-owner-20260907\document-owner-contract-final-release\validation_set_document_owner_contract.txt`
- `wpf_shell_host_recipe_local_validation_set` Debug/Release runtime:
  - 기존 Set 생성·OK/NG 이미지/폴더 등록·중복/누락/경로 복구/Variant/full run/
    partial save/re-enable 경로가 `check=OK`, `size=1600x900`으로 PASS했다.
- `git diff --check` 대상 코드·문서 파일:
  - whitespace 오류 없음. Git의 LF→CRLF 안내만 표시됐다.
- 실행 직전 monitor topology를 동적으로 조회했다. 독립 모니터 1개
  (`\\.\DISPLAY1`, bounds `0,0,1920x1080`, working area `0,0,1920x1032`)로
  보고되어 해당 화면에서 WPF smoke를 실행했다.
- 문서 인덱스 검사는 `TestDocumentationIndex.ps1`로 실행하며 결과를 같은
  evidence directory의 `documentation-index.log`에 남겼다 (`DocumentationIndex=PASS`,
  `IndexedPaths=166`, `Routes=13`, `RootRedirects=102`).

모든 실행 로그와 화면 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-validation-document-owner-20260907`
에 있다.

## 범위 경계

이번 완료는 Validation Set 문서 상태 owner와 그 mutation/persistence 경계에
한정한다. Selection/projection의 추가 독립화, Step Edit, Recipe workspace,
Review History, optional LLM orchestration은 별도 slice다. 전체 테마·Wide/Compact·
125/150/175/200% DPI·다중 모니터·장시간 UI 행렬은 이번 실행에서 검증하지 않았다.
Original 저장소, commit, push, merge, 배포는 수행하지 않았다.

## Refactor proof

- **Current owner:** Shell의 문서 field와 ValidationSets 직접 mutation.
- **Intended owner:** `OpenVisionRecipeValidationSetDocumentOwner` concrete module.
- **Dependency direction:** Shell facade → document owner → 기존 Storage/CatalogPair 계약.
- **State owner:** mutable Validation Set document는 owner 단일 인스턴스가 소유하고,
  실행 상태는 기존 execution-session owner가 계속 소유한다.
- **Observable contract:** 기존 command/binding 이름, XML schema/path, set/image
  normalization과 identity/Variant 정책, Preview/Run 명시성 유지.

Status: Complete
Scope: OVL-07 Validation Set 문서 상태·저장·생성·삭제·이미지 mutation owner 분리.
Acceptance criteria: Shell 직접 문서 mutation 제거, WPF 비참조 concrete owner, 기존 XML/Storage 계약 유지, 무창 owner 계약 및 Debug/Release/WPF 회귀 검증 통과.
Verification: 위 Debug/Release build, owner contract, `wpf_shell_host_recipe_local_validation_set`, 정적 구조 검사, `git diff --check`, 문서 인덱스 검사.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-validation-document-owner-20260907`.
Boundary / next dependency: OVL-07의 selection/projection·Step Edit·나머지 suite orchestration은 별도 작업이며 전체 UI 행렬은 미검증이다. `C:\Git\2D\Original`은 변경하지 않았다.
