# OpenVisionLab 2D 프로젝트 준비도 및 문서 기반 리팩터링 계약

작성일: 2026-09-13 KST  
저장소: C:/Git/2D/Dev  
기준 HEAD: 176eec95  
작업 원장: PL-0029  
구조 리팩터링 원장: PL-0028

## 1. 목적과 범위

이 문서는 처음 프로젝트를 여는 개발자의 관점에서 수행한 전체 분석을
현재 작업 순서와 검증 기준으로 고정한다. 세부 감사 내용을 복사하지 않고
다음 문서를 현재 근거로 연결한다.

- JUNIOR_WHOLE_REPOSITORY_REVIEW_20260911: 전체 저장소·탐색 경로
- JUNIOR_BURDEN_REASSESSMENT_20260912: 첫 참여자 부담과 빌드/readiness
- WPF_VIEW_MVVM_PORTABILITY_AUDIT_20260913: View 가족별 MVVM 및 이동성
- PARTIAL_RESPONSIBILITY_REVIEW_20260913: Partial owner/lifetime 판단
- PARTIAL_STRUCTURAL_ELIMINATION_PLAN_20260913: PL-0028 구조 제거 순서
- CURRENT_HANDOFF: 진행 중인 자동 작업과 최신 작업 경계

이번 계약의 사용자 목표는 다음과 같다.

1. 프로그램 문서와 코드의 실제 진입 경로를 고정한다.
2. Visual Studio에서 열고 빌드하고 실행하는 경로를 재현한다.
3. MVVM, View, Presenter, Controller, native/image lifetime 경계를 구분한다.
4. DLL 의존성·재사용·재배포 가능성을 기술적 근거와 제3자 권리로 분리한다.
5. 불필요 문서·코드·패키지 후보를 정리하되, 호환 문서와 증거는 보존한다.
6. 이 문서를 기준으로 하나씩 검증 가능한 리팩터링을 진행하고, 마지막에
   최초 요청 전체를 다시 대조한다.

## 2. 현재 제품과 비협상 조건

제품은 OpenCvSharp 기반의 결정론적 2D 검사 레시피 워크벤치다.
정상 흐름은 다음과 같다.

Image sample -> Teaching/configuration -> explicit Preview/Run
-> result/layer comparison -> N-sample validation -> Recipe save

카메라·조명·PLC/I/O·계정 플랫폼·클라우드·배포 플랫폼을 이번 기능 범위에
추가하지 않는다. 기존 XAML 이름, binding, 공개 생성자, Recipe/XML,
Preview/Run, PropertyGrid, image/native Dispose와 종료 순서를 보존한다.

현재 작업 트리는 PL-0028이 doing 상태로 계속 움직이고 있다. 문서와 소스
사이에 Partial 수가 53과 54로 엇갈리는 관찰이 있었고, 이 문서가 최종
기준선이라고 주장하지 않는다. 먼저 안정된 체크포인트를 만든 뒤 다음
경계를 연다.

## 3. 진입 경로와 프로젝트 지도

### Start Here

1. AGENTS.md
2. docs/README.md와 docs/LLM_DOCUMENT_INDEX.json
3. OpenVisionLab.sln
4. 시작 프로젝트 src/OpenVisionLab/OpenVisionLab.csproj
5. src/OpenVisionLab/Program.cs
6. App/Bootstrap/OpenVisionLabApplication.cs
7. OpenVisionShellHostWindow
8. OpenVisionShellHostView
9. 목적별 concrete owner와 focused contract

실행 경로:

Program.Main -> OpenVisionLabApplication.Run -> OpenVisionShellHostWindow
-> OpenVisionShellHostView -> Image/Layer/Tool -> Pipeline execution
-> Recipe -> result/review

현재 솔루션은 16개 프로젝트(실행 1, 라이브러리 12, 검증 도구 3)이며
저장소 전체에는 27개 csproj와 34개 ProjectReference가 있다. 프로젝트
순환은 확인되지 않았다. 솔루션 밖 검증 도구는 제품 프로젝트와 섞지 않고
제품 owner -> 해당 contract -> WPF smoke 순서로 읽는다.

## 4. 현재 증거 요약

### 문서

- docs 전체 1,319개 파일, 약 105MB
- 텍스트 계열 911개 읽기/파싱
- Markdown 722개 내용·제목·상태 검사
- JSON 13개 파싱 오류 0
- XML/XSD 143개 중 오류 1: 실제 XSD가 아닌 Markdown 이동 안내
- 이미지 405개 디코드 성공
- MP4 3개 메타데이터·접촉시트 확인
- Markdown 링크 304개 검사 결과 깨진 링크 0

### 빌드와 실행

- dotnet restore --locked-mode: 통과
- Solution Debug 빌드: 16/16, 경고·오류 0
- Visual Studio 2022 전체 빌드 재실행: 16/16 통과
- 시작 로딩 smoke와 깨끗한 1600x900 최초 실행: 통과
- 일반 bin/Debug 실행은 기존 3D 패널과 1,096개 레거시 런타임 파일을 복원
- dotnet test --list-tests: 종료 코드는 0이나 발견 테스트 0개
- 감사 당시 마지막 재감사는 D: 여유 공간 0바이트로 증거 기록이 중단되었으나,
  공간 회복 후 R1 focused evidence를 새 경로에 기록함

실행 증거는 D:/OpenVisionLab-TestData/OpenVisionLab_Dev/
junior-project-audit-20260913 아래에 있다. D:가 가득 찬 동안 기존 증거를
삭제하거나 덮어쓰지 않는다.

## 5. 문서 정리 기준

| 판정 | 대상 | 조치 |
| --- | --- | --- |
| 삭제 후보 | docs/VISION_PIPELINE_RECIPE_SCHEMA.xsd | 실제 XSD가 아닌 5줄 Markdown 이동 안내. 정식 스키마 확인 후 삭제 |
| 축약/보관 | CURRENT_HANDOFF, CODEBASE_STRUCTURE, 완료된 PL-0028 중간 보고서 | 현재 상태·owner·검증·다음 경계만 남기고 누적 이력은 archive |
| 역사적 유지 | root redirect 102개, 공개 샘플·증거 중복 | 호환 링크와 증거 목적이 있으므로 일괄 삭제 금지 |
| 검토 후 삭제 | 미참조 이미지 21개, 약 2.73MB | Git 이력·매니페스트·공개 샘플 사용 여부 확인 후 개별 판정 |
| 유지 | 생성된 사용자 매뉴얼 HTML, 워크플로 영상, 계약·스키마 | csproj/매뉴얼/계약에서 참조하므로 유지 |
| 최신성 수정 | 18개 솔루션이라고 기록한 오래된 보고서 | 실제 16개와 맞지 않으므로 superseded 표기 |

삭제는 현재 PL-0028 변경이 안정된 뒤 문서 인덱스와 함께 수행한다.

## 6. MVVM과 책임 경계

현재 구조는 순수 View -> ViewModel -> Service가 아니라 다음 혼합 구조다.

Shell composition View
  + MVVM binding ViewModel
  + Tool Presenter/Controller
  + PropertyGrid/custom-control contract
  + Window/Dispatcher/Bitmap/native lifetime adapter

이 혼합 자체는 결함이 아니다. View가 업무 정책·영속성·통신·검사 규칙을
소유하는 경우만 책임 이동 대상으로 삼는다.

현재 실제 우선순위:

1. RecipeDialogAdapter의 깨진 한국어와 한국어 분기의 영어 메시지
2. OpenVisionRecipeRunEvidenceViewerView에서 파일/Bitmap decode 책임이
   existing image factory를 통하는지 현재 소스를 기준으로 재확인
3. LineToolWpfView의 persistence가 기존 presenter/session owner를 통하는지
   현재 소스를 기준으로 재확인
4. RoiImageCanvasViewModel의 WPF control, KeyEventArgs, native Mat,
   파일/대화상자 경계. 새 interface를 먼저 만들지 말고 consumer와 수명부터
   고정한다.

Shell composition root, Pipeline Review layout/image owner, Docking controls,
ImageCanvas native host, Learn presenter, generated/designer Partial은 파일
길이만으로 다시 분리하지 않는다.

## 7. DLL 재사용·재배포 판단

기술적으로 관리 DLL은 복사·역컴파일할 수 있다. strong-name은 복사 방지나
소스 보호가 아니다. 다만 앱 DLL 하나만 떼어서는 실행되지 않으며 Core,
Vision2D, OpenCvSharp managed/native, .NET runtime, WPF/native 의존성의
폐쇄가 필요하다.

소스의 Apache 2.0 조건과 제3자 바이너리 조건은 별도다.

- Apache License 2.0: https://apache.org/licenses/LICENSE-2.0.html
- OpenCvSharp: https://github.com/shimat/opencvsharp
- OpenVisionLab Vision SDK: https://github.com/Noah8218/OpenVisionLab-Vision-SDK

현재 sdk-manifest.json과 NOTICE가 서로 다른 SDK 커밋을 가리키며, Blob,
IPPICV, ittnotify의 정확한 재배포 근거가 정리되지 않았다. 따라서 내부
개발 사용은 가능하나 공개·상업 재배포 승인은 보류한다. 권리 자료와 배포
책임자의 결정이 생기기 전에는 DLL 삭제나 패키징을 추측으로 진행하지 않는다.

## 8. 문서 기반 리팩터링 순서

### R0. 기준선 고정

- 현재 owner: PL-0028
- 의도한 owner: PL-0028의 기존 concrete owner/retained reason
- 호출 경로: issue ledger와 CURRENT_HANDOFF
- mutable-state writer: 각 Partial row owner
- 완료 조건: 53/54 불일치 제거, 최신 source survey·문서·원장 일치
- 중복 금지: PL-0028과 같은 Partial 경계를 이 작업에서 다시 구현하지 않음

### R1. 사용자 문자열 경계

- 현재 owner: RecipeDialogAdapter
- 호출 경로: OpenVisionShellHostView -> RecipeCommandSurface -> dialog delegates
- mutable state: 없음; MessageBox/FileDialog 결과만 반환
- lifetime owner: WPF Window owner provider와 dialog
- public/binding contract: delegate signatures 유지
- 변경: 깨진 한국어 문자열을 기존 제품 용어로 복구하고, 한국어 분기의
  English pipeline/delete title을 수정
- proof: source mojibake search, Korean/English focused contract, Debug build,
  dialog smoke 가능 여부 기록

R1 결과(2026-09-13):

- RecipeDialogAdapter의 깨진 한국어 문자열과 한국어 삭제 제목을 복구했다.
- ConfirmDeleteRecipe, ConfirmDeletePipeline, ConfirmDeleteValidationSet의
  delegate signature와 Window owner provider는 변경하지 않았다.
- 새 RecipeDialogLocalizationContract가 5/5 통과했다.
- VisionRecipeRunnerSmoke Debug 빌드가 경고 0·오류 0으로 통과했다.
- 증거: D:/OpenVisionLab-TestData/OpenVisionLab_Dev/
  project-readiness-refactor-20260913/r1-recipe-dialog/
- 남은 범위: 실제 MessageBox/FileDialog를 각 언어와 DPI에서 여는 UI 검증은
  아직 실행하지 않았다.

### R2. Debug runtime data 격리

- 상태: 현재 소스·계약 재확인 완료. 이번 R1 배치에서는 추가 변경하지 않는다.
- 현재 owner: AppPathService
- 현재 동작: DEBUG 기본값은 `InstallationRootDirectory`(빌드 출력 루트)이며,
  같은 루트일 때 Release용 legacy migration을 실행하지 않는다. `OPENVISIONLAB_DATA_ROOT`
  절대 경로 override와 traversal 경계는 `AppPathBoundaryContract`가 검사한다.
- 근거: `OPENVISIONLAB_RUNTIME_DATA_ROOT_V1_CONTRACT.md`의 Debug 격리 규칙과
  `BuildCleanRuntime.ps1`의 Dev manifest 설명이 현재 구현과 일치한다.
- 재개 조건: 새 fresh-F5 증거에서 stale state 자동 복사 또는 의도하지 않은
  restore가 재현될 때에만 AppPathService owner를 다시 연다. 재현 전에는
  일반적인 경로 변경이나 새 abstraction을 추가하지 않는다.
- 현재 미검증: 이 재확인은 소스·계약 기준이며, 별도 clean Debug F5의
  save/reload/reopen 및 migration-count runtime 증거는 아직 실행하지 않았다.

### R3. ROI ImageCanvas 경계

- 현재 owner: RoiImageCanvasViewModel과 View/native adapter가 혼재
- 먼저 기록할 것: consumer, Mat/Bitmap writer, timer/callback release owner,
  key/mouse/dialog contract
- 변경 조건: 독립 state/lifetime/test seam이 확인된 경우에만 기존 concrete
  owner로 이동

### R4. 죽은 코드·패키지·문서 정리

- EventCommandBehavior, unreferenced InspectionAlgorithm/BitmapDrawing,
  unused package, stale App.config, unused CAccountManager를 각각 caller와
  public API 검사 후 작은 배치로 처리
- 한 배치마다 build와 직접 영향 contract를 실행

## 9. 완료 기준

1. 이 문서와 세부 감사 문서가 docs index에서 탐색된다.
2. PL-0028 안정 기준선과 현재 source가 일치한다.
3. R1 문자열 계약이 한국어/영어에서 깨지지 않는다.
4. 각 structural change는 current/intended owner, caller path, state writer,
   lifetime, binding/public contract, focused proof를 남긴다.
5. Debug/Release 또는 해당 변경의 최소 build와 focused contract가 통과한다.
6. 실제 UI에서 실행하지 않은 theme/DPI/input/hardware/long-run 범위를
   완료로 오인하지 않는다.
7. 모든 문서화·구현·검증 후 최초 요청(전체 문서, Visual Studio 실행,
   MVVM, DLL, 삭제 후보)을 다시 대조하고, 그 결과를 이 문서에 추가한다.
8. 최종 재검토가 끝나면 PL-0029 heartbeat를 중지하고 새 예약을 만들지
   않는다.

## 9.1 최종 M4 감사와 최초 요청 재대조 (2026-09-13)

PL-0028 M4와 PL-0029 M4를 닫기 전에 현재 소스·문서·계약을 다시 실행했다.
전체 결과와 재현 경로는
`D:/OpenVisionLab-TestData/OpenVisionLab_Dev/partial-structural-elimination-20260913/m4-final-audit/phase-summary.txt`
에 기록했다.

- `Invoke-RefactorAudit.ps1 -Verify`는 C# 823개, XAML 57개, 컴파일된 Partial
  54개, Partial 문자열 일치 0개, 프로젝트 순환 0개, Shell storage 호출 0개로
  통과했다. 최초 59행 기준은 삭제된 dead/중복 행과, 빈 UserControl smoke 결함
  재현 후 복원한 Sample Picker child `InitializeComponent()` 계약을 반영해
  현재 54개로 정합화했다.
- Solution Debug/Release와 `VisionRecipeRunnerSmoke` Debug/Release가 모두
  경고 0·오류 0으로 통과했다. `OpenVisionReadinessCheck` Debug/Release,
  `TestDocumentationIndex`(IndexedPaths=298, Routes=17, RootRedirects=102),
  PL-0028/PL-0029 원장 검증, 대상 `git diff --check`도 통과했다.
- ColorHsvLearnPresenter, Shell recipe lifecycle, ROI ImageCanvas, recipe-run
  evidence image, LineTool persistence, RecipeDialog localization, ImageCompare
  directory/resource, AppPath focused contract를 Debug/Release 범위에서
  재실행했다. 각각 5/5, 7/7, 6/6, 4/4, 3/3, 5/5, PASS, PASS, 8/8이다.
- `wpf_shell_preview`, `wpf_shell_host_window_chrome`,
  `wpf_shell_host_workspace_sample_picker`, `wpf_shell_host_pipeline_review`,
  `wpf_image_compare` UI smoke가 모두 통과했고 layout/text/internal 진단은
  모두 0이었다. 이 smoke는 대표 경로 증거이며 전체 theme/DPI/input/monitor
  행렬의 실행을 의미하지 않는다.

최초 요청을 다시 대조한 결과는 다음과 같다. 문서 전체 inventory·routing·정리
후보는 통합 문서와 문서 index에 남겼고, fake XSD·stale 보고서·archive 후보·
미참조 이미지 21개는 삭제 후보로만 기록했다. Visual Studio/solution 시작 경로와
실행 증거는 project graph, Debug/Release build, readiness, focused contract,
대표 WPF smoke로 확인했다. 구조는 순수 MVVM이 아니라 View/ViewModel과 WPF
framework/native lifetime adapter가 결합된 혼합 구조이며, 정책·mutable state는
확인된 ViewModel/controller/presenter/service owner로 이동하고 View에는 필요한
XAML·framework·native adapter만 남겼다. DLL은 기술적으로 복사·역컴파일 가능하지만
단일 DLL 실행은 불가능하고 managed/native 의존성 폐쇄·라이선스·NOTICE·SDK
commit 불일치가 해소되지 않아 공개/상업 재배포는 승인하지 않았다.

결론은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`이다. alternate
theme, Wide/Compact, 125/150/175/200% DPI, 전체 입력·resize, camera/GPU/SDK,
장시간 native shutdown, monitor-visible EXE, 제3자 DLL 재배포 권리는 미검증으로
남긴다. 이 최종 대조 후 예약 실행은 중지하며, 새 예약은 만들지 않는다.

## 10. 현재 미검증

- WPF alternate theme, Wide/Compact, 125/150/175/200% DPI
- 전체 pointer/keyboard/focus/pressed/disabled/resize matrix
- 실제 카메라·GPU·SDK·장시간 native shutdown
- 제3자 DLL 공개·상업 재배포 권리
- D: 공간 확보 이후의 최신 전체 build와 새 증거 기록

UI 결론은 소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요다.
