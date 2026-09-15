# GPT Pro 분석 기반 Dev 리팩토링 준비

작성일: 2026-09-07 KST

## 요청과 완료 기준

사용자 요청은 첨부 분석을 참고한 코드 리팩토링 **준비**다. 이번 범위는 현재 Dev 소스 대조, 10개 제안의 적용 판단, 책임 경계, 실행 순서와 검증 계획 작성이다. 첨부 문서의 구현·독립 커밋·CI 변경 지시는 참고 제안이며 이번 요청의 실행 권한으로 해석하지 않는다.

준비 완료 기준:

1. 첨부 기준과 현재 Dev의 차이 및 미커밋 변경 경계를 식별한다.
2. OVL-01~10을 빠짐없이 대조하고 확인 사실, 조건부 위험, 미검증을 구분한다.
3. 각 후보의 최소 변경, 보존 계약, 선행 조건과 합격 증거를 정한다.
4. 준비 문서를 저장하고 문서 경로·색인 및 기존 변경 보존을 확인한다.

제품 코드·테스트·SDK·CI 수정, 실행 검증, Original 작업, 커밋·push는 이번 범위에 포함하지 않는다. 구현 완료나 Runtime UI 합격을 주장하지 않는다.

## 기준과 기존 작업 경계

| 항목 | 첨부 분석 | 이번에 읽은 Dev |
| --- | --- | --- |
| 위치 | Noah8218/OpenVisionLab, main | `C:\Git\2D\Dev`, `codex/public-sample-ux-docs` |
| HEAD | `604c7fb6fc5247198afd666b3e3f37241ac069ba` | `d875559577c85984d54900df973a6fb35fb20146` + 기존 미커밋 변경 |
| 앱 버전 | 2.2.0-dev | `src/OpenVisionLab/OpenVisionLab.csproj`: 2.1.0 |
| Target | net8.0-windows7.0 | net8.0-windows7.0 |
| SDK manifest commit | `ba0055b713e0bf434b9d0a7fd3f4b0e445c1f982` | `f4f0c0dc8bee5b7a849ae6eb66a5307bed4b8a6b` |
| Integration 패키지 | 이 준비에서 원격 상태 미조회 | Contracts `[0.2.0-alpha.3]`, Transport.Tcp `[0.1.0-alpha.3]` |

입력: `C:\Users\USER\Downloads\OpenVisionLab_Implementation_Tasks_604c7fb.md`.
입력 SHA-256: `814EAAAB2459AC776B85C233F11B7093760894541ED3DD7A973398919E47B3CD`.
SDK 값은 manifest에 기록된 값이며 이번 작업에서 DLL 해시 일치나 빌드 재현성을 검사한 결과가 아니다. 서로 다른 커밋의 선후 관계도 추정하지 않는다.

기존 변경에는 Shell XAML/code-behind, SampleCheck/N-image 서비스, screenshot/recipe/direct smoke runner, 문서·빌드 스크립트 등이 포함된다. 미추적 `OpenVisionTcpIntegrationExeSmoke.cs`도 있다. 구현 시작 시 이 파일들의 현재 diff를 읽고 사용할 기반을 정해야 한다. 기존 변경을 정리·되돌리거나 한 커밋에 묶지 않는다. 이번에는 새 보고서와 색인의 해당 route만 편집한다.

현재 제품은 OpenCvSharp4 기반 규칙 기반 비전 레시피 워크벤치다. 현재 핸드오프의 RC/사전 운영 평가는 과거 기록의 적용 환경에 한정되며 새 빌드 합격이나 상용 GA를 뜻하지 않는다. 상용 도구에서 유지할 교훈은 명시적 실행, 시각적 결과 근거, 반복 가능한 티칭·저장 흐름이다. 카메라·조명·PLC/I/O·MES·계정·장비 제어 플랫폼 확장과 LLM 기능 확장은 제외한다. 기존 TCP 경로의 무결성 검토는 새 플랫폼 기능 추가와 구분한다.

과거 `OPENVISIONLAB_STRUCTURAL_REFACTORING_COMPLETION_20260726.md`는 선제적 구조 정리 종료와 기존 책임 추출을 기록한다. 이번 요청은 새로운 준비 요청이므로 검토를 열지만, 과거 완료된 추출을 실패로 재분류하거나 다시 구현하지 않는다. 현재 남은 수명 문제와 정책 결합만 후보로 삼는다. 별도 스킬 벤치마크는 사용자 승인, CVR-00은 독립 초보자 3명의 관찰 자료가 필요하며 여기서 재개하지 않는다.

## 현재 소스와 분석의 대조

아래 경로는 Dev 루트 기준이다. 행 번호는 이번에 읽은 소스의 위치로, 구현 전에 다시 확인한다. 위험은 런타임 재현 결과와 구분한다.

| 항목 | 현재 근거 | 적용 판단 |
| --- | --- | --- |
| OVL-01 / E01 | `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Documents/OpenVisionNativeToolDocument.cs:114,560,868`: 초기화 구독, Dispose 해제, 성공 Preview마다 추가 구독. `src/Libraries/OpenVisionLab.Localization/OpenVisionLanguageService.cs:457`은 static event. | **소스 결함 확인.** 초기화가 이미 구독하므로 추가 초기화가 아니라 성공 경로의 중복 구독 제거가 첫 후보다. 실제 콜백 수·GC 수거 미측정. |
| OVL-02 / E02 | `src/OpenVisionLab/Core/Integration/TwoDIntegrationExchange.cs:179,236,329,370`의 공개 ACK/Run은 `LoadQualifiedTargetIdentity` 호출. `TwoDIntegrationBuildIdentity.cs`는 실제 ApplicationAssembly와 expectedIdentity를 패키지 verifier에 전달. 결과 존재 검사는 `:385`. | **분석 일부가 현재 Dev와 다름.** 요청 신원을 그대로 신뢰한다고 확정할 수 없다. 패키지 내부 동작 및 공개 기본 경로의 실패 차단은 미실행. 결과 파일 존재 검사만으로 동시 Run 1회 진입은 증명되지 않는다. UI IsBusy는 존재하므로 UI 중복 클릭과 공용 API 동시 호출도 구분한다. |
| OVL-03 / E05 | `src/Libraries/OpenVisionLab.Logging/Model/RuntimeLogSink.cs`는 무제한 큐와 전체 drain. `RuntimeLogStream.cs`는 합친 문자열을 다시 split. `src/Libraries/OpenVisionLab.Logging.Controls/ViewModel/LogPanelViewModel.cs:27,69,312`는 3,000행 상한과 250ms 소비 timer. | **수집 큐·한 번의 소비량 경계가 후보.** UI 표시 행 상한은 이미 있으므로 재구현하지 않는다. OOM이나 현장 지연을 관측한 것은 아니다. |
| OVL-04 / E04 | `src/Libraries/OpenVisionLab.ImageSpace.Core/ImageSpaceService.cs:80,98,140`: 원본 반환과 lease 공존, 이름 조회 lock과 index 제거 lock 분리. Native Preview와 Review는 `GetLayerImage`로 입력을 읽는다. | **소유권 및 조건부 경쟁 확인.** 동시 교체 시점의 disposed 접근은 재현 테스트 필요. Lease를 먼저 재사용하고 clone 제거는 별도 계측 후 판단한다. |
| OVL-05 / E03 | `src/OpenVisionLab/Core/Pipeline/Execution/VisionPipelineExecutionService.cs:455`는 timeout/cancel 뒤 worker를 drain. `src/OpenVisionLab/UI/Menu/Wpf/PipelineReview/Execution/OpenVisionPipelineReviewExecutionController.cs:91,97,146`은 Reset cache 삭제, CancellationToken.None 실행, Dispose 뒤 결과 적용 차단 부재. 실행계획 생성은 try 밖. TCP controller `:565`에 동기 DisposeAsync 대기. | **Review 세대·종료 경계 후보.** drain 자체는 안정 계약상 필수다. 무조건 유한 시간 반환으로 바꾸면 자원 수명을 깨뜨린다. UI의 실행 상태 회복과 늦은 결과 차단부터 분리한다. |
| OVL-06 / E06 | `src/OpenVisionLab/Common/SerializeHelper.cs:177,236,240`에 임시 파일/Replace/Move. `src/OpenVisionLab/Core/Pipeline/Storage/VisionPipelineStorage.cs`에 journal·복구·실패 주입. `.github/workflows/ci.yml:28`은 SkipLaunch, artifact upload는 수동 실행 조건. | **검증 보강 후보.** 저장 원자성 기능과 기존 실패 주입 테스트를 새로 만드는 일이 아니다. 프로세스 kill/restart, 항상 남는 테스트 증거, 필요 시 전수 계측을 별도 단계로 둔다. |
| OVL-07 / E07 | `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface*.cs`는 10개, 현재 합계 512,448 bytes. Validation 문서·선택·CRUD가 surface에 남아 있다. 기존 `OpenVisionRecipeExecutionSessionViewModel`, ValidationSetPresenter, StepEditSessionViewModel, Workspace/Pipeline UseCase가 이미 존재한다. | **잔여 Validation 소유권부터 검토.** 첨부 454,851 bytes는 현재 값이 아니다. 파일 byte 수는 참고 계측이며 AST/논리 타입 전수 감사가 아니다. 기존 상태 owner를 복제하지 않는다. |
| OVL-08 / E08 | `src/Libraries/WpfPropertyGridBridge/WpfPropertyGridAdapter.cs:209,224,239,1746`에 Threshold/Canny/Affine 이름 정책. `src/Libraries/PropertyGrid.Abstractions/PropertyGridContracts.cs`에 기존 display options와 browsable 계약. | **공용 UI에 들어간 업무 정책 분리 후보.** 기존 계약을 먼저 활용한다. 정적 TypeDescriptor 등록도 존재하므로 인스턴스 정책 분리와 전역 등록 수명을 별도로 조사한다. |
| OVL-09 / E09 | `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/OpenVisionLearnWindow.xaml.cs`: 208,402 bytes, XAML 271,381 bytes, private DispatcherTimer 선언 17개. BasicGrayscale/Binary/Line/Matching SimulationModel 존재. | **주제 단위 추출 후보.** 타이머 수는 누수 증거가 아니다. 이미 추출된 계산을 다시 만들지 않는다. `docs/admin/CODEBASE_STRUCTURE.md`의 “프리뷰 실행 또는 자동 프리뷰”는 명시적 실행 권위와 정합성 검토가 필요하다. |
| OVL-10 / E10 | `src/OpenVisionLab/Core/Integration/TwoDIntegrationExchange.cs:673`에서 totalMilliseconds는 ms, 각 Step metric은 일괄 unitless. | **단위 전달 의미 손실 경로 확인.** 현재 metric 숫자·이름은 유지한다. 허용 단위, unknown 표현, 좌표 문맥은 계약/consumer fixture 확인 전 확정하지 않는다. |

## 제안 실행 순서와 합격 조건

아래는 구현 후보이며 실행 승인이나 완료 목록이 아니다. OVL-06은 관련 테스트를 각 변경에 붙이는 06a와 별도 광역 검증 06b로 나눈다. 전체 CI/계측 완성을 모든 수정의 선행 조건으로 만들지 않는다.

| 순서 | 최소 범위와 보존 계약 | 필요한 합격 증거 | 모델 |
| --- | --- | --- | --- |
| 1. OVL-01 + 06a | 기존 Document 초기화/Dispose owner 유지, 성공 Preview의 추가 구독 제거. 캐시·도킹 수명과 언어 리소스 보존. | 동일 문서 100회 성공 Preview 후 언어 콜백 1회, 최종 Dispose 후 0회. 캐시 재열기와 실제 eviction 구분, 중복 Dispose, WeakReference 수거. 언어 전환의 Preview/Run·레이어·라우팅 부작용 0. | Recommended model: gpt-5.6-luna / Reasoning effort: medium |
| 2. OVL-03 + 06a | Sink의 항목/문자 상한과 bounded batch. 기존 UI 3,000행 제한·영속 로그 보존 활용. 정책 수치는 구현 시 메시지 크기/소비 증거로 제안. | 병렬 producer, 소비 정지/재개, 큰 메시지, overflow 수, Dispose 경쟁. 메모리 제한과 1회 소비 제한을 각각 검사. 중요한 오류의 파일 보존 확인. | Recommended model: gpt-5.6-terra / Reasoning effort: medium |
| 3. OVL-02 검증 후 필요 수정 | 기존 assembly verifier 재사용. 공개 ACK/Run 기본 overload의 누락·변조·불일치 실패 차단부터 검사. 공용 API의 transaction 진입 소유자를 정한다. | 정상 ACK→Run→Result, version/SHA/source-state 불일치, manifest 누락/변조, 같은 ID 동시 Run의 실제 진입 1회, 완료 ID 재요청. 단일/다중 프로세스 지원 범위 확정 전 보장 확대 금지. | Recommended model: gpt-5.6-terra / Reasoning effort: high |
| 4. OVL-10 + 06a | 앱 결과 변환 경계에서 명시적 metric metadata 활용. mm/px/deg/ms/ratio/unknown을 계약에 맞춰 처리. | 숫자 불변, JSON 왕복, unknown 진단, 좌표 Layer/변환 문맥 보존, 소비 측 호환 fixture. 실제 교차 저장소 실행에는 명시적 대상과 검증 입력이 필요. | Recommended model: gpt-5.6-terra / Reasoning effort: high |
| 5. OVL-04 + 06a | ImageSpace lease→독립 실행 snapshot→작업 drain→해제. 이름 조회/삭제는 같은 임계영역, Release는 밖. Preview consumer부터 전환. | barrier로 Acquire→Replace/Delete→Read, RemoveByName/Insert 교차, 이중 Dispose 없음, snapshot 불변. Arithmetic B/Fixture 입력 보존. 복사 byte 추정과 실제 peak 측정 분리. | Recommended model: gpt-5.6-terra / Reasoning effort: high |
| 6. OVL-05 | 04 후 Review Run/Reset/Dispose부터. 이후 TCP 종료, Recipe 전환을 별도 단위로 검토. 네이티브 drain 계약 유지. | 지연 fake step의 A/B 교차 완료, Reset/Close 뒤 cache 재생성 없음, 계획 생성 예외 후 IsRunning 복구, 이중 Run 차단, 취소/timeout/오류 구분, dispatcher 종료. 비협조 worker 종료 보장은 주장하지 않음. | Recommended model: gpt-5.6-terra / Reasoning effort: high |
| 7. OVL-07 | 05와 관련 baseline 후 Validation 문서/선택/CRUD 소유권 한 단위. Workspace→Step Edit→History→optional LLM은 각각 독립 경계가 입증될 때만 후속 채택. | 한 상태의 변경 owner 1개, 기존 binding/command façade 유지, Dirty·Apply·Save·Cancel·전환·실행 중 변경 차단, XML 저장/재로드. 기존 use case 호출 유지와 surface의 옛 직접 상태 변경 제거 증명. | Recommended model: gpt-5.6-terra / Reasoning effort: high |
| 8. OVL-08 | 06a 및 Step Edit 편집 계약 확인 후 앱 정책→기존 abstraction/options→공용 bridge로 전달. | Threshold/Canny/Affine 분류가 bridge에서 제거, 두 편집 문맥 격리, descriptor 표시/순서/숨김/검증/키보드 동등, public namespace/attribute/extern alias 유지. 등록 해제 계약 확인. | Recommended model: gpt-5.6-terra / Reasoning effort: high |
| 9. OVL-09 | 06a 후 Threshold/기본 영상 주제 하나. 08 전체 완료에 의존하지 않음. 주제 View에 timer/Focus/animation, 기존 모델에 계산 유지. | 다른 주제의 상태 접근 감소, 주제 전환·닫기 후 tick 중지, 명시적 Apply만 Tool 변경, 주제 ID/샘플/언어/automation 연결 보존. 신규 파라미터 추가 예제와 실제 소유권 문서 갱신. | Recommended model: gpt-5.6-terra / Reasoning effort: high |
| 10. OVL-06b | 관련 slice에 필요한 만큼 kill/restart harness, 실패 시 증거 업로드, 전수 계측을 확장. 기존 RC gate 유지. | journal 단계별 새 프로세스 복구 old-or-new 일관성, 실패 로그 보존, SHA/OS/SDK/입력 hash/명령/ExitCode 기록. 계측은 partial을 완전한 타입명으로 합산, 생성 코드 분리, 소스 전후 동일. | Recommended model: gpt-5.6-terra / Reasoning effort: high |

TCP가 실제 검사에 사용되는 배경이 확인되면 3번을 1번 다음으로 올린다. 현재 사용 여부는 미확인이다. 그 정보가 없어도 이벤트·이미지·로그 준비는 진행할 수 있다. 단위 처리도 단순 책임 이동을 넘어 잘못된 의미를 바로잡는 행동 변경이므로 구조 리팩토링과 별도 검증 단위로 둔다.

## 구조 변경의 실제 경계

| 후보 | 현재 owner / 호출·데이터 흐름 | 의도한 owner / 방향 | 완료를 입증할 구조 증거 |
| --- | --- | --- | --- |
| Validation | Shell binding→CommandSurface가 validationSetDocument·선택·CRUD를 변경. 실행 상태는 기존 ExecutionSessionViewModel, 표시 projection은 기존 ValidationSetPresenter. | Shell façade→Validation의 구체 owner→기존 storage/presenter. recipe/pipeline 식별과 선택 결과를 명시적으로 전달. 실행 상태는 기존 owner 하나를 공유. | Shell이 같은 문서/선택의 변경 가능한 복사본을 보유하지 않음. 기존 presenter를 우회하지 않음. 새 owner는 Shell Window 없이 행동 테스트 가능. 클래스명/추출 크기는 전체 호출자 조사 후 확정. |
| PropertyGrid | 공용 bridge가 검사 속성명을 보고 표시 정책 결정. 앱은 factory/adapter를 통해 grid 구성. | 앱의 Tool 속성 정책→PropertyGrid.Abstractions의 최소 표현→bridge의 표시/editor. 공용 라이브러리→앱 참조를 만들지 않음. | bridge의 도구명 기반 분기 제거, 두 grid 옵션 격리. 새 interface/provider는 기존 options로 표현 불가능한 실제 경계가 있을 때만 도입. |
| Learn | Window가 여러 주제 UI/timer/상태를 조정하고 이미 분리된 SimulationModel을 사용. | Learn Window는 주제 선택·탐색, 한 주제 View는 자신의 UI/timer, 기존 모델은 계산. 명시적 Apply 결과가 기존 Tool 연결로 전달. | 대상 주제 상태/timer에 대한 이전 Window의 직접 접근 제거, inactive/Closed 수명 검증. 이름만 바꾸거나 partial 추가로 완료 처리하지 않음. |
| Review 수명 | Document→ExecutionController→ExecutionService→native worker. cache와 run 상태는 controller. | 같은 controller가 실행 세대·취소 요청·cache 적용 권한을 소유. 서비스는 worker drain과 입력 수명 소유. UI callback은 유효 세대의 결과만 적용. | Reset/Dispose 이후 callback이 상태를 재생성하지 않음. UI 요청 종료와 실제 worker 종료를 구분. 새 범용 runner/context 가방 불필요. |

보존할 동작은 XML 필드/기본값/저장 경로, Preview/Run 명시성, 레이어 생성·삭제·선택·입출력 라우팅, 기존 Good/Bad 의미, 수치와 그림, 문서 캐시/도킹 계약이다. 검증 실패를 숨기는 기본값·재시도·호환 우회는 추가하지 않는다. WPF ViewModel에 Window/Control/ShowDialog를 넣지 않는다.

## 기존 검증 재사용과 부족한 증거

실행하지 않은 아래 항목은 향후 계획이다. 테스트 이름이 존재한다는 사실은 합격이나 모든 요구의 coverage를 뜻하지 않는다.

| 대상 | 현재 존재를 확인한 진입점 | 추가할 핵심 증거 |
| --- | --- | --- |
| 이미지/실행 수명 | `tools/VisionRecipeRunnerSmoke/Program.cs`: `--runtime-stability-contract`, `--bitmap-converter-contract` | lease 동시 교체 barrier, Review generation/Dispose/계획 예외 |
| 저장/호환성 | 같은 runner: `--recipe-storage-path-contract`, `--pipeline-provenance-contract`, `--pipeline-persistence-recovery-contract` | 실제 자식 프로세스 kill/restart. 실패 주입 예외와 전원 차단 내구성은 구분 |
| TCP/identity | `tools/VisionRecipeRunnerSmoke/TwoDIntegrationSmoke.cs`, `tools/TwoDIntegrationTcpSmoke/Program.cs` | 기존 missing/identity/hash tamper case 재사용. 기본 overload, 같은 ID 동시 진입, 종료 경쟁 추가 |
| Tool/Review | `tools/PipelineViewerScreenshotSmoke/Program.cs`: `wpf_shell_host_pipeline_review`, `wpf_shell_host_pipeline_review_input_state` | 언어 이벤트 정확한 콜백 수/Dispose 수거는 전용 행동 검사 추가 필요 |
| Validation/PropertyGrid | 같은 screenshot runner: `wpf_shell_host_recipe_local_validation_set`, `p219_affine_point_binding_property_grid`, `wpf_property_grid_matching_combo` | Dirty/selection/바인딩 왕복, 두 문맥, 현재 변경에 영향을 받는 대표 소비 화면 |
| Learn | 같은 screenshot runner: `wpf_openvision_learn_threshold`, `wpf_openvision_learn_threshold_animation`, `wpf_openvision_learn_threshold_apply`, `wpf_openvision_learn_reopen_focus` | 한 주제의 활성/비활성/닫기 수명과 모델 수치 동등 |

실행 단계에는 테스트 데이터·TEMP/TMP·로그·화면 증거를 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev` 아래의 작업 전용 폴더에 둔다. 기존 bin/obj junction의 실제 D: 대상을 확인한 뒤 관련 프로젝트만 빌드한다. 파일 이동이나 junction 재설정은 이 계획에 포함하지 않는다. 공개 Recipe 원본은 보존하고 테스트 복사본으로 저장→재로드→명시적 Preview/Run의 수치·그림·라우팅을 비교한다.

WPF 변경에는 `C:\Users\USER\.codex\docs\WPF_UI_UX_RULES.md`를 적용한다. 영향받는 control/container 및 shared style 소비 화면에서 지원 theme, Wide/Compact, 가능한 100/125/150/175/200% DPI, 정상·hover·실제 pressed·focus·selected·disabled/read-only·error·mouse-leave·popup을 검사한다. 비어 있지 않은 입력과 긴 값, 마우스/키보드, CanExecute와 중복 실행, 양방향 binding을 실제 화면에서 확인한다. 새 before/after 증거와 실행하지 못한 상태를 기록한다.

실제 desktop EXE 검증은 현재 빌드로 실행하고 모니터를 동적으로 조회한다. 독립 모니터가 정확히 2개면 왼쪽의 작은 모니터에 창과 dialog를 배치하고 실제 창 사각형 및 monitor bounds를 기록한다. 1개면 그대로, headless면 가상 화면과 그 한계를 기록한다. 직접 in-process WPF 렌더링은 실제 EXE 검증으로 부르지 않는다.

각 slice의 복구 범위는 자신이 만든 변경만으로 한정한다. 기존 dirty 작업에 일괄 checkout/reset을 사용하지 않는다. Recipe schema/SDK를 보존하면 데이터 마이그레이션은 필요 없다. TCP 무결성 실패 시 검증 우회로 복귀하지 않고 해당 실행을 중지한다. 커밋 여부와 시점은 별도 사용자 요청에 따른다.

## 이번 준비의 검증 및 종료 기록

Status: Complete

- Scope: OVL-01~10의 현재 Dev 소스 대조와 단계별 리팩토링 준비 문서.
- Acceptance criteria: 기준 차이와 미커밋 경계 식별, OVL-01~10 대조, 최소 변경·owner·선행 조건·합격 증거 작성, 문서 경로·색인과 기존 변경 보존 확인 모두 충족. Complete는 준비 범위에만 적용한다.
- Verification: `git status --short`, `git log --oneline -5`, branch/HEAD 조회, 대상 `rg`/`Get-Content`, 파일 byte/선언 수와 입력 SHA-256 확인. `powershell -NoProfile -ExecutionPolicy Bypass -File tools/TestDocumentationIndex.ps1` PASS: 156 indexed paths / 13 routes / 102 redirects. `git diff --check -- docs/LLM_DOCUMENT_INDEX.json` PASS: LF→CRLF 안내만 발생. 보고서의 OVL-01~10 포함 및 명시적 저장소 참조 경로 존재 검사 PASS. 기존 dirty 파일 77개의 SHA-256 동일, 새 보고서 이외 git status 동일, 색인 변경은 원본 바이트를 읽어 비교한 정확한 route 추가 1건임을 확인했다.
- Evidence: 이 문서와 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-preparation-20260907`의 `before-edit.json`, `documentation-index.log`, `preparation-checks.json`.
- Unrun checks: 솔루션/프로젝트 빌드, readiness, 코드 행동 테스트, WPF smoke, EXE 실행, 메모리/장시간·DPI/테마·교차 소비자 검증. 준비·문서 작성만 수행했으므로 실행하지 않았으며 과거 PASS를 이번 결과로 재사용하지 않는다.
- Boundary / next dependency: 구현·버그 수정·Runtime UI 합격을 증명하지 않음. 소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요. 후속 구현 요청 시 1번의 현재 기반과 관련 미커밋 변경을 재확인한다.
