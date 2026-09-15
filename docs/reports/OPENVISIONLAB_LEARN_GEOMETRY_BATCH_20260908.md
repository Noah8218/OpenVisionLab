# Learn Geometry Transform 책임 분리 — 2026-09-08

## 작업 계약

Status: Complete (Geometry Transform topic 15 구조·행동 보존 범위).

이번 배치는 dirty 상태의 Dev checkout에서 Learn topic15의 실제 책임 경계
하나만 이동했다. `OpenVisionLearnWindow`는 Topic 선택, 공통 문서/실습,
창 수명, 기존 public facade와 callback 전달을 유지하고, Geometry의 단계·각도·
배율·수식·상태·렌더링·타이머·관련 Tool 안내를 `GeometryLearnView`와
`GeometryLearnPresenter`로 옮겼다. Foundation, Grayscale, Binary/Line,
Matching, LayerRecipe, Metrics, Recipe 실행 소유자는 다시 나누지 않았다.

제품 정체성은 OpenCvSharp 기반 deterministic rule-based vision Recipe
workbench이며, Handoff 기준 RC/pre-production 수준이다. 상업 도구에서 유지할
교훈은 짧은 명시적 실행 경로, OK/NG 근거와 drawing/metric 비교, bounded
validation, deterministic replay다. 하드웨어·카메라·PLC/I/O·MES·계정·배포는
이번 범위에서 제외했다.

시작 입력과 dirty 상태는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-learn-geometry-20260908`
의 `work-contract.md`, `worktree-before.csv`, `before/`,
`git-status-before.txt`에 보관했다. 브랜치는
`codex/public-sample-ux-docs`, 시작 HEAD는
`d875559577c85984d54900df973a6fb35fb20146`이다.

## 소유권과 호출 경로

| 책임 | 이전 | 현재 |
| --- | --- | --- |
| Angle/Scale 입력, OutputSize 수식, 0~3 단계, 의미·상태·색상 역할 | Window 필드·메서드 | `GeometryLearnPresenter`의 WPF 없는 정책 |
| Geometry topic XAML, transform 렌더링, slider/button 이벤트, 520ms timer와 Loaded/Unloaded | Window | `GeometryLearnView.xaml(.cs)` |
| Topic 선택, 공통 문서/실습, 창 종료, callback 주입과 기존 public API | Window | Window 유지; Geometry View로 전달 |

실제 경로는 `OpenVisionLearnWindow -> GeometryLearnView ->
GeometryLearnPresenter`다. 새 Interface/Factory/Wrapper/production partial은
추가하지 않았다. View 제거는 timer를 정지하고 Tick 구독을 해제하며, 재호스팅은
단계·설정을 보존하고 자동 재생하지 않는다. Topic 숨김은 기존처럼 View를
unload하지 않아 재생을 유지한다. Tool callback 성공 뒤에만 안내를 바꾸고,
예외는 기존처럼 호출자에게 전달한다. Preview/Run·Recipe/XML·Layer 계약은
호출하지 않았다.

## 검증 결과

| 기준 | 결과 |
| --- | --- |
| Presenter 정책을 Window 없이 검증 | `--learn-geometry-presentation-contract`: 4/4 PASS. 단계/문구, OutputSize, 문화권 독립성, reset/restart, 역할, 인스턴스 독립성, Tool 안내 |
| 기존 Window 호환 | `wpf_openvision_learn_geometry_contract`: PASS. 기존 public facade, slider/formula/status, topic 왕복, pause, hidden playback, callback/예외, explicit Preview/Run 문구 |
| 새 View 단독 수명·상호작용 | `wpf_openvision_learn_geometry_view`: PASS. 두 unload/reload cycle, timer 중복 방지, no-autoplay, hidden playback, Tool callback/예외/비활성, keyboard focus, Close |
| 기존 Geometry 화면 회귀 | `wpf_openvision_learn_geometry`: PASS |
| 구조 이동 증명 | `structure-proof.py`: 44/44 PASS. Window 전용 Geometry 상태/메서드/XAML 제거, 새 owner 사용, 완료 Learn owner 8개 SHA 보존, baseline/after 상태·계약 증거 동일 |
| 빌드 | PipelineViewerScreenshotSmoke Release PASS (기존 CS8600 경고 1, 오류 0), VisionRecipeRunnerSmoke Release PASS, OpenVisionLab Debug PASS, OpenVisionReadinessCheck Release PASS |
| 동적 모니터 UI | `DISPLAY2`의 작은 왼쪽 working area `(-1920,365,1920,1032)`, 96 DPI. 모든 after 창이 선택 영역과 교차했고 `monitor-placement.json`에 기록 |

Baseline과 after의 Geometry contract `.states.txt` 및 `.contract.txt`는
byte-identical이다. PNG 최종 contract frame도 동일 SHA-256이며, 단계별 PNG는
렌더 시점/스크롤 상태 차이가 있어 구조 동일성의 근거로 상태 계약과 최종
프레임을 사용했다. 새 View 단독 캡처는
`after/wpf_openvision_learn_geometry_view.png`다.

실행하지 않은 검증은 실제 제품 EXE qualification, 전체 Readiness 성공,
Recipe/XML round trip, 모든 테마/Wide/Compact 레이아웃, 125/150/175/200% DPI,
실제 pointer hover/pressed/mouse-leave와 물리 키보드 순환, 장시간 실행이다.
Readiness 전체 실행은 이전 Matching/Layer/Metrics 추출에 남은 정적 토큰
불일치로 exit 1이며 Geometry 관련 검사는 통과했다. 해당 불일치를 이번
Geometry 범위에서 수정하거나 기대값을 완화하지 않았다.

## 변경 파일

- `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/GeometryLearnPresenter.cs`
- `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/GeometryLearnView.xaml`
- `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/GeometryLearnView.xaml.cs`
- `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/OpenVisionLearnWindow.xaml(.cs)`
- `tools/PipelineViewerScreenshotSmoke/LearnGeometrySmoke.cs`
- `tools/PipelineViewerScreenshotSmoke/LearnGeometryViewSmoke.cs`
- `tools/PipelineViewerScreenshotSmoke/Program.cs`
- `tools/VisionRecipeRunnerSmoke/LearnGeometryPresentationContract.cs`
- `tools/VisionRecipeRunnerSmoke/Program.cs`
- `tools/OpenVisionReadinessCheck/Program.cs`
- `AGENTS.md`, `docs/LLM_DOCUMENT_INDEX.json`, `docs/admin/CODEBASE_STRUCTURE.md`,
  `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`, 이 보고서

## 종료 경계

이번 Geometry 경계의 남은 구현 작업은 없다. 기존 Readiness 불일치와 미실행
환경은 별도 증거 없이는 재개하지 않는다. 다음 프로젝트 우선순위와 상업적
범위는 Handoff의 기존 결정(P256/Rule-Based Skill 및 XML 후보 게이트 포함)을
그대로 따르며, 이 완료가 자동 후속 작업을 승인하지 않는다.

`openvisionlab-2d` 15분 heartbeat는 PAUSED 상태를 유지한다. 다른 모델·에이전트·
예약 실행은 새 명시적 사용자 요청 없이 코드, 주석, 문서, 재검증 또는 같은
경계를 다시 나누지 않는다. Original 변경, stage, commit, push, merge,
release, deployment는 수행하지 않았다.
