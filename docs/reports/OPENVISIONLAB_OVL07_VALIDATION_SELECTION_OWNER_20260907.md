# OpenVisionLab OVL-07 Validation Set selection/projection owner 분리

작성일: 2026-09-07 KST

## 의미와 범위

이번 실행은 OVL-07에서 독립적으로 검증 가능한 한 가지 작업으로,
Local Validation Set의 option, 선택된 set, PinArrayGap Train/Validation/Test
선택, image-row와 선택된 image-row 상태를
OpenVisionShellHostRecipeCommandSurface의 mutable field에서
OpenVisionRecipeValidationSetSelectionOwner로 이동했다.

Shell은 기존 XAML binding/command façade, OnPropertyChanged, 상태 메시지,
command 재평가, Variant 입력 projection을 계속 담당한다. 문서 mutation과
validation-sets.xml 저장은 이전 slice의
OpenVisionRecipeValidationSetDocumentOwner가 계속 담당하고, Validation Set
실행은 OpenVisionRecipeValidationSetRunner가 담당한다.

Recipe/XML schema·경로·정규화·identity/Variant 계약, Preview/Run 명시 실행,
입력/출력 Layer 규칙은 변경하지 않았다. 신규 기능, UI 전면 개편, 의존성 변경,
schema 변경은 포함하지 않았다.

## 구조 변화

| 항목 | 이전 | 현재 |
| --- | --- | --- |
| option 목록과 선택된 set | Shell backing field와 SetProperty | OpenVisionRecipeValidationSetSelectionOwner.Options/Selected |
| PinArrayGap split 선택 | Shell backing field | selection owner의 Train/Validation/Test 상태 |
| image-row projection | Shell backing field와 Presenter 직접 호출 | selection owner가 document owner를 통해 Presenter projection을 보유 |
| 선택 변경 부수효과 | field setter와 refresh가 상태와 projection을 함께 변경 | selection owner는 상태/selection만 변경하고 Shell은 binding/status/command side effect를 유지 |
| 문서 mutation/persistence | document owner 경계 | 변경 없음. selection owner는 문서를 복사하거나 저장하지 않음 |

WPF ComboBox/list의 ItemsSource 갱신 중 발생할 수 있는 일시적인
null source update는, 유효한 option/row가 이미 존재하면 selection owner가
무시한다. 빈 목록에서는 Clear/refresh가 명시적으로 null 상태를 만든다.
따라서 Release 화면에서 첫 이미지 추가 후 선택이 사라져 다음 등록이 막히던
회귀를 방지하면서 사용자 선택 계약은 유지한다.

## 호출 경로와 상태 소유권

WPF binding/command → Shell facade → SelectionOwner → DocumentOwner →
기존 ValidationSet Presenter/Storage 계약

- selection owner는 Window, UserControl, System.Windows, Shell 타입을
  참조하지 않는 concrete C# type이다.
- selection owner는 option/image selection과 refresh projection만 소유한다.
- document owner는 mutable document, set/image CRUD, load/save, catalog import를
  소유한다.
- 실행 중/중지 상태는 기존 OpenVisionRecipeExecutionSessionViewModel과
  OpenVisionRecipeValidationSetRunner가 소유한다.
- Shell은 기존 public binding 이름과 side effect 순서를 보존한다.

## 변경 파일

- src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetSelectionOwner.cs
  - option, pinned split, image-row selection 상태와 refresh/clear를 추가했다.
  - WPF TwoWay binding의 transient null 보호를 포함한다.
- src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetDocumentOwner.cs
  - selection owner가 기존 Presenter projection을 통해 사용할 image-selection 진입점을 추가했다.
- src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs
  - 기존 selection backing field를 제거하고 selection owner façade로 연결했다.
- src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs
  - recipe 전환 시 pinned selection reset을 selection owner에 위임했다.
- src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs
  - refresh/row projection을 selection owner 호출과 기존 UI 상태 통지로 바꿨다.
- tools/VisionRecipeRunnerSmoke/Program.cs
  - document/selection owner 무창 계약에 set/image projection과 transient-null 보호 검사를 추가했다.
- tools/PipelineViewerScreenshotSmoke/Program.cs
  - local validation smoke의 실패 진단 메시지를 선택 상태와 row/status까지 포함하도록 보완했다.
- docs/LLM_DOCUMENT_INDEX.json
  - recipe/structure route에 이 scoped report를 추가했다.
- docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md
  - 완료 내용, 증거, 다음 단일 우선순위를 기록했다.

## 검증

- app x64 Debug/Release build:
  - build-app-debug-guard.log
  - build-app-release-guard.log
  - 각각 경고 0개, 오류 0개.
- VisionRecipeRunnerSmoke x64 Debug/Release build:
  - build-runner-debug-guard.log
  - build-runner-release-guard.log
  - 각각 경고 0개, 오류 0개.
- PipelineViewerScreenshotSmoke x64 Debug/Release build:
  - build-smoke-debug-guard.log
  - build-smoke-release-guard.log
  - 각각 경고 0개, 오류 0개.
- Window 없는 --validation-set-document-owner-contract Debug/Release:
  - set create/save/reload, duplicate rejection, image add/projection,
    remove/delete와 selection-owner의 transient-null 보호가 모두 PASS.
  - 결과: selection-contract-debug-final/validation_set_document_owner_contract.txt,
    selection-contract-release-final/validation_set_document_owner_contract.txt.
- wpf_shell_host_recipe_local_validation_set Debug/Release:
  - 기존 set 생성, OK/NG 이미지와 top-level folder 등록, duplicate/update,
    missing path repair, Variant, full/partial run과 re-enable 경로가
    check=OK, layout=0, text=0, internal=0, size=1600x900으로 PASS.
  - 결과 로그와 PNG: wpf-local-validation-debug-final/,
    wpf-local-validation-release-final/.
- 실행 직전 동적 monitor 조회:
  - monitor-topology-debug-final.log, monitor-topology-release-final.log
  - 단일 DISPLAY1, bounds 0,0,1920x1080, working area
    0,0,1920x1032; 해당 화면에서 smoke를 실행했다.
- 정적 구조 검사:
  - static-selection-owner-check.log PASS.
  - selection owner의 WPF/Shell 참조 없음, Shell delegation, stale backing field
    제거, command surface의 document mutation 제거를 확인했다.
- git diff --check:
  - git-diff-check.log, exit 0. Git의 기존 LF→CRLF 안내만 표시됐다.
- TestDocumentationIndex.ps1:
  - documentation-index.log에 DocumentationIndex=PASS, IndexedPaths=167,
    Routes=13, RootRedirects=102를 기록했다.

모든 실행 증거는
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-validation-selection-owner-20260907
에 있다.

## 범위 경계

이번 완료는 Validation Set selection/projection 상태 owner와 그 WPF binding
경계에 한정한다. Step Edit, Recipe workspace, Review History, 추가 suite
orchestration, Layer/이미지 lifetime, 전체 theme/layout/DPI/multi-monitor
행렬은 별도 작업이며 이번 실행에서 검증하지 않았다. Original repository,
commit, push, merge, 배포는 수행하지 않았다.

## Refactor proof

- Current owner: Shell의 selection backing field, setter, Presenter 호출.
- Intended owner: OpenVisionRecipeValidationSetSelectionOwner concrete module.
- Dependency direction: Shell facade → selection owner → document owner →
  existing Presenter; mutation은 document owner에만 남는다.
- State owner: option 목록/선택 set/pinned split/image rows/selected row는
  selection owner, mutable XML document는 document owner, 실행 상태는 기존
  execution-session/runner가 소유한다.
- Observable contract: public binding names, command behavior, XML schema/path,
  Preview/Run explicitness와 Layer routing은 유지된다.

Status: Complete
Scope: OVL-07 Validation Set selection/projection state owner 분리.
Acceptance criteria: Shell selection field 제거, WPF 비참조 concrete owner,
기존 document/runner 경계 유지, transient-null regression 방지, Debug/Release
무창 계약·WPF smoke·정적 구조·문서 인덱스 검증 통과.
Verification: 위 build, owner contract, WPF smoke, monitor, static, diff-check,
documentation-index 검사.
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-validation-selection-owner-20260907.
Boundary / next dependency: OVL-07 Step Edit 책임 경계 분리는 다음 단일 작업이며,
전체 UI 행렬과 Original 저장소는 이번 결과가 증명하지 않는다.
