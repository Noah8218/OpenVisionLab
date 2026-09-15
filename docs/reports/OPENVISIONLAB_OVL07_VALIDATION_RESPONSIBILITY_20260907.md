# OpenVisionLab OVL-07 Validation 실행 책임 1차 분리

작성일: 2026-09-07 KST

## 의미와 범위

이번 OVL-07 slice는 `Local Validation Set`의 실제 이미지 목록 실행 책임을
`OpenVisionShellHostRecipeCommandSurface`에서 독립적인 구체 runner로 이동한
작업이다. Validation Set 문서 생성·선택·CRUD 전체를 한 번에 옮기지 않고,
실행 중 상태와 기존 UI façade는 유지하면서 실행 경계를 먼저 분리했다.

보존한 계약은 Recipe/Pipeline XML 형식, Pipeline 저장 경로, frozen identity
검사, 기존 sample-check 서비스 호출, Preview/Run·레이어·라우팅 동작, 명시적
중지와 부분 결과 저장, 기존 XAML binding 및 command 이름이다.

## 현재 구조와 의도한 구조

| 항목 | 이전 | 현재 |
| --- | --- | --- |
| 실행 책임 owner | `OpenVisionShellHostRecipeCommandSurface.Handlers.cs`의 `RunLocalValidationSetAsync`가 XML 읽기, frozen identity 확인, 이미지 순회, sample/result 변환, outcome 판정, 부분 저장, 집계를 모두 소유 | `OpenVisionRecipeValidationSetRunner`가 위 실행 흐름과 결과 계약을 소유하고, `OpenVisionRecipeValidationRunSupport`가 sample/result 변환과 공통 판정 보조를 소유 |
| 호출 경로 | WPF command → Shell handler의 직접 실행 루프 | WPF command → Shell façade의 session 시작/상태 투영 → `OpenVisionRecipeValidationSetRunner.RunAsync` → 기존 storage/sample-check 서비스 |
| 의존성 방향 | Shell handler가 실행 정책과 결과 변환을 함께 보유 | Shell → Validation runner → 기존 Storage/SampleCheck/Outcome 계약. runner는 Shell/WPF 타입을 참조하지 않음 |
| 상태/데이터 owner | 실행 상태는 기존 `OpenVisionRecipeExecutionSessionViewModel`이 소유하지만 handler가 실행 중 변경 가능한 흐름도 함께 소유 | 실행 상태는 기존 `OpenVisionRecipeExecutionSessionViewModel` 하나가 계속 소유한다. Shell은 선택된 Set의 실행용 이미지 snapshot과 UI callback만 전달하고 runner는 결과를 반환한다 |

## 변경 파일

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetRunner.cs`
  - 명시적 request/result 계약과 비-WPF 실행 runner를 추가했다.
  - pipeline XML 읽기, frozen identity preflight, 이미지 순회, 중지 감지,
    결과 저장과 부분 실행 표식을 이동했다.
- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationRunSupport.cs`
  - local sample 생성, batch 결과 매핑, expected outcome 판정, 부분 실행
    note 생성을 새 owner로 이동했다. Selected/Pair/Catalog의 공통 결과 매핑도
    같은 owner를 사용한다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs`
  - runner를 한 번 생성해 사용하는 façade 의존성을 추가했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs`
  - local 실행의 핵심 loop를 runner 호출로 대체하고, session lifecycle,
    status projection, history refresh, 오류/종료 처리는 Shell에 남겼다.

## 구조 증거

- Handler에서 기존 `RunLocalValidationSetAsync`의 직접 이미지 loop,
  `TryValidateFrozenIdentity`, 부분 저장, 집계 helper 정의가 제거됐다.
- Handler는 `validationSetRunner.RunAsync(new OpenVisionRecipeValidationSetRunRequest(...))`
  를 호출하고 runner 결과의 count/path만 화면 상태로 투영한다.
- `OpenVisionRecipeValidationSetRunner.cs`와
  `OpenVisionRecipeValidationRunSupport.cs`에는
  `OpenVisionShellHostRecipeCommandSurface` 또는 `System.Windows` 참조가 없다.
- `OpenVisionRecipeExecutionSessionViewModel`의 `IsLocalValidationSetRunning`,
  `StopRequested`, `StartValidationSuite`, `RequestStop`,
  `CompleteValidationSuite`가 기존 실행 상태 owner로 유지되어 두 번째 mutable
  실행 상태 복사본을 만들지 않았다.
- runner는 Shell이 만든 이미지 목록 snapshot을 읽기 전용으로 소비하며
  Validation Set 문서의 별도 mutable 복사본을 만들지 않는다.

## 검증

- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Debug -p:Platform=x64 --no-restore -m:1 -nr:false`
  - 0 warnings / 0 errors.
- `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform=x64 --no-restore -m:1 -nr:false`
  - 0 errors. 기존 dirty smoke 코드의 `Program.cs:10717` nullable warning 1개가
    남아 있으며 이번 runner 변경에서 발생한 warning은 아니다.
- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Release -p:Platform=x64 --no-restore -m:1 -nr:false`
  - 0 warnings / 0 errors.
- `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Release -p:Platform=x64 --no-restore -m:1 -nr:false`
  - 0 errors. 동일한 기존 nullable warning 1개.
- `wpf_shell_host_recipe_local_validation_set` Debug/Release runtime smoke
  - local Set 생성·OK/NG 이미지와 폴더 등록·중복/누락/경로 복구·variant
    contract·명시적 full run·stop partial save·재활성화를 통과했다.
  - Debug 결과: `check=OK`, `size=1600x900`.
  - Release 결과: `check=OK`, `size=1600x900`.
- 정적 구조 검사
  - runner/support의 Shell/WPF 참조 없음, façade의 runner 호출 존재,
    이전 helper 정의 제거를 확인했다.
- `git diff --check` 대상 코드 파일 통과. LF→CRLF 변환 안내만 Git이 표시했다.
- 실행 직전 Windows monitor topology를 동적으로 조회했다. 독립 모니터 1개
  (`\\.\DISPLAY1`, bounds `0,0,1920x1080`, working area `0,0,1920x1032`)라
  기존 화면을 그대로 사용했다.

모든 실행 로그와 화면 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-validation-responsibility-20260907`
에 있다.

## 범위 경계

이번 완료는 OVL-07의 첫 실행 책임 slice에 한정한다. Validation Set 문서
생성·선택·CRUD, Workspace/Step Edit/Review History/optional LLM 책임,
Selected/Pair/Catalog 전체 orchestration은 기존 Shell에 남아 있다. 전체
테마·Wide/Compact·125/150/175/200% DPI·다중 모니터·장시간 UI 행렬은 이번
실행에서 검증하지 않았다. XML schema나 저장 경로를 바꾸지 않았으므로
마이그레이션은 필요하지 않다.

## Refactor proof

- **Before:** Shell handler가 local validation 실행 정책과 결과 변환을 직접 소유.
- **After:** `OpenVisionRecipeValidationSetRunner`가 실행 정책을 소유하고
  Shell은 façade/session/status만 소유.
- **Dependency direction:** Shell → runner → 기존 storage/sample-check 계약.
- **State owner:** `OpenVisionRecipeExecutionSessionViewModel` 단일 owner 유지.
- **Observable contract:** 기존 command, binding, 저장 kind
  (`LocalValidationSet`/`LocalValidationSetPartial`), partial note, 결과 집계
  의미를 유지.

Status: Complete
Scope: OVL-07 첫 slice — Local Validation Set 실행 책임과 공통 batch 결과 변환 분리.
Acceptance criteria: 기존 실행 계약 유지, Shell/WPF 비참조 concrete runner, 기존 실행 상태 owner 단일 유지, façade 호출 경로와 old helper 제거 증명, Debug/Release build 및 local validation runtime smoke 통과.
Verification: 위 Debug/Release x64 build, 정적 구조 검사, `wpf_shell_host_recipe_local_validation_set` Debug/Release smoke, `git diff --check`.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-validation-responsibility-20260907`.
Boundary / next dependency: 전체 OVL-07 Validation 문서/선택/CRUD와 나머지 suite orchestration은 별도 slice다. `C:\Git\2D\Original`은 변경하지 않았다.
