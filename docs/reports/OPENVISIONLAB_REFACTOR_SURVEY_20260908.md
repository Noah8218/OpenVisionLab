# OpenVisionLab 전수 조사 및 단계적 리팩토링 순서 — 2026-09-08

> 이 문서는 Run History orchestration slice 직전의 구조 기준선이다. 현재
> 전수 계측·주니어 모듈화 판정·완료 기준·자동화와 갱신된 우선순위는
> `OPENVISIONLAB_REFACTOR_PROGRAM_AUDIT_20260908.md`를 따른다. 아래의
> 당시 “다음 리팩토링 순서”는 역사적 조사 근거이며 현재 작업을 중복
> 승인하지 않는다.

## 조사 범위와 권위

이번 조사는 `C:\Git\2D\Dev`의 현재 소스·프로젝트·기록을 기준으로 수행했다. 브랜치와 HEAD는 `codex/public-sample-ux-docs` / `d875559577c85984d54900df973a6fb35fb20146`이며, 조사 시작 시 worktree는 의도적으로 dirty 상태였다(`status --short` 227줄). 기존 변경은 보존하고 이 작업의 대상에서 제외했다.

사용자가 첨부한 `C:\Users\USER\Downloads\OpenVisionLab_Implementation_Tasks_604c7fb.md`는 SHA-256 `814EAAAB2459AC776B85C233F11B7093760894541ED3DD7A973398919E47B3CD`인 과거 개발 명세다. 문서의 OVL-01~10 후보·검증 제안은 현재 구현의 증거가 아니므로, 현재 `AGENTS.md`, `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`, 안정 계약, 현재 호출 경로와 새 검증 결과로 재확인했다.

15분 자동화 `openvisionlab-2d`는 조사 시점에도 `PAUSED`이며, 이번 작업에서 재개하지 않았다. 자동 모델·에이전트·예약 실행이 후속 리팩토링을 이어가도록 승인하지 않았다.

## 코드 계측 결과

| 항목 | 현재 계측값 | 의미 |
| --- | ---: | --- |
| C# 소스 | 761개 / 268,055줄 / 12,209,708 bytes | 제품·라이브러리·도구가 함께 있는 대규모 코드베이스; 조사 시작 기준은 759개 / 267,740줄 |
| XAML 소스 | 58개 / 24,624줄 / 1,752,829 bytes | WPF 화면과 템플릿 비중이 큼 |
| `partial` 선언 | 106개 | 부분 파일 수 자체를 결함으로 판단하지 않고 책임 경계로 재검토해야 함 |
| 1,000줄 이상 C# 타입 | 28개 | 큰 타입이 남아 있으나 파일 길이만으로 분리하지 않음 |
| 2,000줄 이상 C# 타입 | 12개 | 독립 상태·의존성·호출 경로가 입증되는 경우에만 다음 분리 후보 |
| 3,000줄 이상 C# 타입 | 8개 | `PipelineViewerScreenshotSmoke` 등 도구가 포함되어 있어 제품 책임과 구분 필요 |
| Shell 중심 파일 집합 | 10개 / 10,391줄 / 481,747 bytes | `OpenVisionShellHostRecipeCommandSurface`가 남은 가장 큰 제품 경계 |
| 프로젝트 순환 참조 | 발견되지 않음 | 앱·12개 라이브러리·도구 간 현재 참조 방향은 유지 가능 |

패턴 검색 수는 결함 수가 아니라 다음 조사 위치를 찾기 위한 넓은 계측이다. 최종 계측은 Native Tool lifetime 177/36 files, Runtime log 58/5, ImageSpace/Layer 53/15, execution lifetime 154/9, Recipe CommandSurface 1,493/77, PropertyGrid 5,418/194, Learn/Shell 471/42, metric 62/3이다. execution lifetime 수에는 이번 gate와 독립 계약이 포함된다.

## 이전 후보의 현재 상태

| 후보 | 현재 상태 | 근거와 중복 작업 방지 판단 |
| --- | --- | --- |
| OVL-01 Native Tool 이벤트 lifetime | 범위 닫힘 | 구독·해제 쌍과 기존 계약이 `OPENVISIONLAB_OVL01...`에 기록됨. 새 결함 재현 전 재분리하지 않음 |
| OVL-02 TCP identity/중복 entry | 범위 닫힘 | 기존 identity·disposal 계약과 보고서가 현재 호출 경로에 남아 있음 |
| OVL-03 Runtime log queue | 범위 닫힘 | bounded queue/read batch 구현과 계약이 존재함 |
| OVL-04 Image Lease/Layer 원자성 | 범위 닫힘 | ImageSpace Lease/snapshot 및 Pipeline Review image owner가 분리되어 있음 |
| OVL-05 실행 세대·취소·종료 | 범위 닫힘 | controller generation/cancellation/close 계약이 존재함. 이번 Document gate는 controller 경계를 재분리하지 않고 문서 투영만 보강함 |
| OVL-06 회귀·복구 | 06a 범위 닫힘, 06b 미완료 | process recovery 계약은 있으나 전수 정량 감사 계측은 별도 작업 |
| OVL-07 Recipe/Review CommandSurface | 진행 중 | Validation·Step Edit·Recipe manager·Review projection owner가 분리됨. 이번 실행에서 Document result projection revision gate를 추가로 완료했으며 History/Validation orchestration 잔여가 남음 |
| OVL-08 PropertyGrid 정책 | 범위 닫힘 | 공용 adapter와 검사 정책 owner가 분리되고 계약이 기록됨 |
| OVL-09 Learn/Shell | 주제별 범위 닫힘, Shell 잔여 | Foundation/Grayscale/Binary-Line/Matching/Geometry 등 완료 owner를 재분할하지 않음. Window 공통 topic selection과 남은 주제는 별도 evidence 필요 |
| OVL-10 metric 단위 | 범위 닫힘 | 단위·표시 계약과 보고서가 존재함 |

따라서 현재 성숙도는 “전체 제품 완성률”이 아니라, 안정성·리소스·주요 Learn 주제·Recipe Review의 독립 owner와 계약 증거가 넓게 구축된 RC/pre-production 단계로 판단한다. installer/signing, 다중 PC·하드웨어, 현장 metrology, 전체 WPF theme/layout/DPI/input 행렬은 아직 전수 입증되지 않았다.

## 이번 실행에서 선택한 한 slice

핸드오프가 지목한 최상위 미해결 결합은 `OpenVisionPipelineReviewDocument`의 결과 투영 경계였다. controller 내부에는 실행 세대와 revision 검사가 있었지만 Document의 completion, exception, `StepUpdated`, finally 경로는 `disposed`만 확인했다. Reset·입력 교체·Recipe 교체 뒤 오래된 continuation이 현재 View 상태를 덮을 수 있는 구조였다.

이번 실행은 다음 한 경계만 변경했다.

- `OpenVisionPipelineReviewDocumentRevisionGate`가 input/recipe/run generation과 disposal invalidation을 소유한다.
- Document refresh/run/dispose가 gate를 통해 revision을 갱신·캡처한다.
- controller의 `StepUpdated` event args가 input/recipe revision을 전달한다.
- Document의 completion/failure/StepUpdated projection이 current stamp를 확인하고, finally는 현재 controller가 idle일 때만 busy를 해제한다.
- Recipe/XML, Preview/Run 명시성, Layer routing, ImageSpace ownership, controller의 stale callback atomic boundary는 변경하지 않았다.

독립 계약은 gate의 recipe/input invalidation, 새 run generation, disposal idempotence와 실제 Document/controller/event source call path를 함께 검사한다. 계획과 소유권은 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-survey-20260908\refactor-proof-plan.md`에 남겼다.

## 다음 리팩토링 순서

1. **OVL-07 Recipe CommandSurface의 남은 History/Validation orchestration 경계** | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`  
   현재 큰 Shell/CommandSurface 집합에서 실제 남은 파일 I/O·상태 조합을 재현하고, 기존 Validation/Recipe execution owner를 재사용한 최소 경계만 분리한다.
2. **OVL-06b 전수 정량 감사 계측 보강** | Recommended model: `gpt-5.6-luna` | Reasoning effort: `low`  
   현재 06a 복구 증거를 다시 구현하지 않고, 누락된 전수 지표와 재현 가능한 출력만 보강한다.
3. **OVL-09 Learn Window/Shell 잔여 topic ownership** | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`  
   이미 닫힌 Presenter/View owner는 유지하고, Window에 남은 실제 공통 선택·문서 책임 중 독립 경계가 증명되는 부분만 조사한다.
4. **WPF 대표 화면의 theme/layout/DPI/pressed/focus runtime 행렬** | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`  
   소스 토큰 검사가 아니라 현재 빌드에서 대표 Pipeline Review와 Shell 상태를 동적 모니터·DPI·키보드 경로로 검증한다.
5. **OVL-01/02/03/04/05/08/10 및 완료 Learn owner 재검토** | Recommended model: `gpt-5.6-luna` | Reasoning effort: `low`  
   새 defect, 명시 요구사항 변경 또는 책임 충돌이 재현될 때만 다시 연다. 파일 길이·새 모델 선호만으로 재분할하지 않는다.

### 범위 밖

카메라·조명·PLC/I/O·MES·계정·배포·새 LLM 기능·신규 알고리즘·Framework/의존성 버전 변경은 이번 순서와 조사 범위에 포함하지 않는다.

## 재현 증거

- 조사 계측: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-survey-20260908\source-survey.json`, `source-survey-summary.txt`
- 기준선: `status-initial-preserved.txt`(조사 시작 226줄), `branch-head.txt`, `baseline\build-runner-debug.log`, `baseline\stale\stdout.log`, `baseline\execution\stdout.log`
- 구조 계획: `refactor-proof-plan.md`
- 새 slice 계약: `revision-contract-debug`, `revision-contract-release`, `revision-stale-debug`, `revision-stale-release`, `revision-execution-debug`, `revision-execution-release`
- Release/WPF 검증: `revision-build-main-release.log`, `revision-build-smoke-release.log`, `ui-after-normal-r6`, `ui-after-ng-r1`, `documentation-index-check.log`, `git-diff-check.log`
