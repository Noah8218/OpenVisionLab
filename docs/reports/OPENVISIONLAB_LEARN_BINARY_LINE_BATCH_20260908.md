# Learn Binary / Line 개발 묶음과 중복 리팩토링 방지 — 2026-09-08

## 작업 계약

사용자는 대규모 개발을 이어서 진행하되, 이미 리팩토링한 구조는 확실한 사유가
없으면 다시 나누지 말고 중복 작업 방지 기준을 문서화하도록 요청했다.
이번 묶음은 남은 Binary 3개(Morphology/Blob/Contour), Line 2개(Edge/LineDistance)
주제의 책임 이동과 그 검증이다. 이전의 단일 소규모 실행 제한은 이번 명시적
대규모 요청 범위에서 확대한다. 작업 종료 후 자동 후속 실행 금지와 예약 PAUSED는 유지한다.

입력: `C:\Git\2D\Dev`, `codex/public-sample-ux-docs`,
`d875559577c85984d54900df973a6fb35fb20146` 및 기존 dirty worktree.
증거: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-learn-binary-line-20260908`.
기존 파일의 SHA-256 목록과 수정 전 snapshot을 먼저 보존했다.

현재 제품은 규칙 기반 검사 Recipe 워크벤치이며, 기록된 검증 환경에서 RC 수준이다.
명시적 실행·판정 근거·Recipe 재사용이라는 상용 도구의 장점을 유지한다.
하드웨어·계정·클라우드·배포 확장은 범위 밖이다. LLM/XML 평가는 재개하지 않는다.

| 경계 | 변경 사유와 새 소유자 | 보존할 기존 소유자 |
| --- | --- | --- |
| Binary 학습 주제 3개 | Window가 세 주제의 UI 외에 선택값·단계·설명·표시 판정을 직접 소유함. `BinaryLearnPresenter`와 `BinaryLearnView`가 담당 | `OpenVisionLearnBinarySimulationModel` |
| Edge/Line 학습 주제 2개 | Window가 두 주제의 상태·설명·판정과 UI 타이머를 혼합함. `LineLearnPresenter`와 `LineLearnView`가 담당 | `OpenVisionLearnLineSimulationModel` |
| 완료된 경계 | 현재 호출과 완료 기록에 새 결함이나 책임 충돌 없음. 재분리하지 않음 | Matching/LayerRecipe/Metrics Presenter·View, 공유 Learn resources/cells, 기존 Recipe/Validation/실행 owners |

흐름: Window 주제 선택·Tool callback -> 각 View의 명시적 진입점 -> Presenter 상태와
기존 SimulationModel 계산 -> View 렌더링. Window는 주제 제목·부제·Focus와 창 수명,
기존 공개 테스트 facade를 유지한다. Presenter에 WPF/실행/Recipe 저장 의존성을 추가하지 않는다.
View가 타이머 생성·Loaded 구독·Unloaded 정지/해제를 소유한다.

완료 기준:
1. 두 도메인의 실제 독립 소유자가 사용되고 Window의 해당 정책·상태·컨트롤 결합이 제거된다.
2. 기존 계산 모델·완료된 경계·문구·XAML·Recipe/XML/SDK 계약을 보존한다.
3. Morphology 재선택/모드 변경의 완료 단계 복원, Blob MIN_AREA 변경 시 재생 유지,
   다른 모드/슬라이더 변경의 정지 규칙, topic 숨김 중 타이머 동작을 보존한다.
4. 변경 전후 집중 UI·설정·상태·frame 비교, 독립 Presenter·View·재결합 수명·Tool 호출 검증,
   관련 빌드와 구조·문서 검증 결과를 기록한다. 기존 실패는 삭제하거나 완화하지 않는다.
5. AGENTS와 구조 안내에 완료된 경계 및 근거 기반 재개 기준을 명시한다.

두 보조 에이전트는 각각 분리된 D: draft 폴더만 사용한다. 저장소 통합과 빌드/UI
실행·공용 문서는 주 에이전트만 처리한다. 같은 파일의 동시 편집과 같은 검증의 중복 실행을 피한다.
실제 소스 소유권이 명확하지 않은 영역은 파일 크기만으로 분리하지 않는다.

## 완료 경계 재개 기준

완료 기록이 있는 경계는 기본적으로 그대로 재사용한다. 새 모델의 선호, 파일 길이,
이름/폴더 취향, 남은 토큰, “더 깔끔해 보임”은 재분리 사유가 아니다.
변경 전에 다음 기록을 남길 수 있을 때만 기존 완료 범위를 다시 연다.

```text
기존 완료 기록 / 소유자:
새로 확인한 실패 또는 변경된 요구사항:
현재 코드·재현 절차·실패 검증:
기존 소유자 안의 수정만으로 해결되지 않는 이유:
최소 변경 경계 / 유지할 계약:
완료를 증명할 집중 검증:
```

이번 작업은 기존 SimulationModel의 재분리가 아니라, 아직 Window에 남아 있는
다른 책임을 처음 옮긴다. 새로 옮긴 경계도 완료 후 동일한 재개 기준을 적용한다.

## 완료 기록

Status: Complete
Scope: Learn Binary/Line 다섯 주제의 상태·표시 정책과 View 수명 경계, 중복 작업 방지 규칙.

| 완료 기준 | 실제 증거 |
| --- | --- |
| 새 소유자가 실제 사용되고 Window 책임 제거 | `structure-proof.txt`: 93 PASS; Window 필드·메서드 제거, facade/주제/Tool/Close 위임과 WPF 없는 Presenter |
| 기존 동작과 계산 소유자 보존 | 5개 패널 XML·문구·AutomationId·이벤트 동등, 완료/shared 소스16개 SHA-256 동일, 무관한 Window 메서드123개 동일 |
| 단계·설정·재생·외부 실행 계약 보존 | 독립 계약8그룹, UI8대상 PASS; 68개 상태 기록 완전 일치, 32개 안정화된 화면 완전 일치 |
| 관련 빌드·집중 검증 | 아래 네 빌드 성공; View 재결합/Tool 실패 순서와 인접 Metrics 계약 통과 |
| 완료 경계와 재개 근거 문서화 | AGENTS의 중복 작업 방지, 구조 안내0/5.6, 현재 Handoff, 문서 index route |

### 변경 파일과 책임

생산 코드 루트: `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/`.

- `BinaryLearnPresenter.cs`, `LineLearnPresenter.cs`: 단계·샘플·설명·셀 표시 판단의 독립 소유자.
- `BinaryLearnView.xaml(.cs)`, `LineLearnView.xaml(.cs)`: 기존5패널과 컨트롤·브러시·렌더링·Timer/Loaded/Unloaded.
- `OpenVisionLearnWindow.xaml(.cs)`: 실제 두 View 배치, 기존 주제/Tool/Close 호출 전달; public facade 유지.
- `tools/VisionRecipeRunnerSmoke/LearnBinaryPresentationContract.cs`, `LearnLinePresentationContract.cs`, `Program.cs`: 독립 계약8그룹과 명시적 실행 target.
- `tools/PipelineViewerScreenshotSmoke/LearnBinaryLineSmoke.cs`, `Program.cs`: 단계/설정 baseline 기록과 독립 View 수명·Tool 계약. 기존5개 legacy test 본문은 유지.
- `tools/OpenVisionReadinessCheck/Program.cs`: Binary/Line 계산 호출과 이동한 panel 검사를 새 실제 소유자 경로로 연결. 인접 완료 영역의 기존 stale assertion은 수정하지 않음.
- `AGENTS.md`, `docs/admin/CODEBASE_STRUCTURE.md`, `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`, `docs/LLM_DOCUMENT_INDEX.json`, 이 보고서: 완료된 소유자·탐색 경로·재개 사유·종료 경계.

Window C# 3,671 -> 2,369줄, XAML 3,198 -> 2,206줄. 줄 수는 이동 규모일 뿐
완료 기준이 아니다. 새 partial은 XAML 생성 코드와의 조합에만 사용하며,
Presenter는 Window/Recipe/Tool 실행에 의존하지 않는 concrete type이다.
새 Interface/Factory/Message Bus, SDK/Framework 버전 변경은 없다.

### 실제 실행한 검증

모든 출력과 테스트 TEMP/TMP는 위 D: 증거 루트에 두었다. 기존 빌드 출력
junction도 D: 대상임을 시작 시 확인했다. 아래 빌드는 `--no-restore -m:1
-nr:false`를 사용했다. UI/앱/계약 runner는 `-p:WpgCustomBuildEnabled=false`.

| 명령/대상 | 결과·로그 |
| --- | --- |
| `dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Debug -p:Platform=x64` | PASS, 경고0/오류0; `debug-build.log` |
| `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Release -p:Platform=x64` | baseline/변경 후 PASS; 기존 CS8600 경고1; `baseline-build.log`, `after-build-fixed.log` |
| `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Release -p:Platform=x64` | PASS, 경고0/오류0; `unit-build.log` |
| `dotnet build tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj -c Release -p:Platform=x64` | PASS, 경고0/오류0; `readiness-build.log` |
| runner DLL `--learn-binary-presentation-contract <evidence>/unit-binary` | 4 PASS / 0 FAIL |
| runner DLL `--learn-line-presentation-contract <evidence>/unit-line` | 4 PASS / 0 FAIL |
| `powershell -NoProfile -ExecutionPolicy Bypass -File <evidence>/run-baseline.ps1` | 수정 전 소스 UI6대상 PASS, exit0 |
| 동일 `run-after.ps1` | UI8대상 PASS, exit0; `after/stdout.log`, `monitor-placement.json` |
| `python <evidence>/structure-proof.py` | 93 PASS / 0 FAIL |
| `python <evidence>/compare-evidence.py` | 상태68개/안정화 캡처32개 동일; 무관 메서드123개 동일 |

UI 대상: `wpf_openvision_learn_morphology`, `..._blob`, `..._contour`,
`..._edge_line`, `..._line_distance`, `..._binary_line_contract`,
`..._binary_line_views`, `..._metrics_acceptance_contract`.

각 주제의 모든 단계, 설정 경계값, topic 왕복, 실제 Timer의 Play/Pause,
숨겨진 주제 재생, Close를 확인했다. 독립 View별2회 분리/재결합은 단계와
설정 보존, 자동 재생 없음, 중복 Tick 없음, 종료 후 갱신 없음을 검증한다.
6개 Tool 링크의 enum·안내·명시적 클릭1회·callback 예외 전파/안내 미갱신도 통과했다.
실제 Tool/Preview/Recipe 저장 실행은 이 테스트에서 수행하지 않는다.

### 실패 기록과 증거의 한계

- 첫 통합 빌드는 인접 `OpenVisionLearnThresholdApplyEventArgs` 선언을 제거한
  편집 범위 오류로 실패했다(`after-build.log`). 메서드 끝 경계 처리를 수정해
  원래 선언을 복원했다. 이후 빌드, 공개 API 비교와 전체 집중 회귀가 통과했다.
- 전체 PNG38장 원본 일치 탐색은36장 일치/2장 차이였다. 두 legacy 캡처
  Contour/EdgeLine의 차이는 변경하지 않은 공통 실습 Expander에만 있다
  (`legacy-image-difference.txt`: 각각 bounds 296,246–1007,335 및
  296,280–1007,347). 화면 검토상 전환 시점 차이이며, 안정화 후32개
  비교 캡처는 모두 byte-identical이다. 원본38장 전체 일치를 주장하지 않는다.
- baseline/after 캡처는 기존 Window viewport의 문맥 비교다. 스크롤 밖 셀의
  모든 시각 상태를 입증하지 않는다. 셀별 text/background/border는 상태68개에
  포함하며, `after/*-standalone.png`는 분리한 실제 컨트롤을 보여준다.
- 실행 환경은 current-source WPF test host, 96 DPI, 동적으로 선정한 작은
  왼쪽 `\\.\DISPLAY2`, bounds `(-1920,365,1920,1080)`. 실제 창 교차를 기록했다.
- 실제 OpenVisionLab EXE, 전체 회귀/Readiness, Recipe/XML round trip, 모든 테마,
  Wide/Compact, DPI125/150/175/200%, 실제 pointer-down/hover/키보드/focus 전체
  검증은 미실행이다. 레이아웃·스타일 변경이나 제품 전체 UI 검증 완료가 아니다.
- Readiness에는 앞선 Matching/Layer/Metrics 이동을 반영하지 않은 기존
  Window 문자열 assertion이 남아 있다. 소스에서 확인했으며 이번에는 관련
  Binary/Line owner assertion과 컴파일만 검증했다. 기존 테스트를 삭제하거나
  기대값을 완화하지 않았고 전체 readiness 성공을 주장하지 않는다.

### 종료 경계

기존 사용자 변경을 보존했고 Original·stage·commit·push·merge·배포는 하지 않았다.
이번 변경은19개 파일이며, 최종 SHA 비교에서 기존2,129개 파일은 동일했다.
동시에 별도 작업에서 수정한 스킬 문서4개와 Handoff의 새 Skill Static Development
항목은 그대로 보존하고 이번 작업 diff에서 제외했다. 관련 경로는 `final-scope.txt`와
`independent-handoff-preserved.md`에 기록했다.
최종 범위·문서 검증은 `final-scope.txt`, diff와 status는 `task.diff`,
`git-status-after.txt`에 남긴다. 두 보조 에이전트는 disjoint draft/읽기 전용
검토만 수행했으며 추가 구현을 이어가지 않는다.

다음 작업: 자동 후속 작업 없음. 새로운 명시적 요청이 오기 전에는 코드·주석·문서·
재검증을 시작하지 않는다. 이번 Binary/Line 소유자도 완료 경계 목록에 등록했으며,
재분리는 위 재개 템플릿의 확실한 근거가 있을 때만 한다. 15분 예약은 PAUSED를
유지한다. 대기 중인 모델 실행이나 별도 후속 개발을 등록하지 않았다.
