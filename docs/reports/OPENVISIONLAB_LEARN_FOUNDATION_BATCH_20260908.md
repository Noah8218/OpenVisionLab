# Learn Foundation 화면·상태 책임 분리 — 2026-09-08

## 작업 계약

Status: Complete (Foundation 구조·행동 보존 범위).

Scope: Learn topic0의 Point/Size/Rect/ROI/RotatedRect 및 Mat/채널 학습을
Window에서 독립적인 Presenter·View로 분리했다. 입력은 Dev의
`codex/public-sample-ux-docs`, `d875559577c85984d54900df973a6fb35fb20146`
및 시작 시 dirty worktree다. 이번 사용자의 새 요청으로 이 범위를 수행했다.
기존 Grayscale, Binary/Line, Matching, LayerRecipe, Metrics 및 계산 Model을
다시 나누지 않았다. 사용자 변경과 별도 작업의 문서는 그대로 보존한다.

현재 제품은 규칙 기반 Recipe 워크벤치이며 Handoff상 기록된 환경에서 RC 수준이다.
명시적 실행, 판정 근거, 반복 가능한 검사 흐름을 유지한다. Recipe/XML/SDK,
Preview/Run, Layer 규칙, Geometry/HSV 구현, 의존성·버전, 플랫폼 기능은
변경하지 않았다. 이 기록은 제품 전체 UI 또는 출시 품질 완료를 뜻하지 않는다.

작업 전 계약·입력 SHA·원본은 D: 증거 폴더의 `work-contract.md`,
`worktree-before.csv`, `before/`, `git-status-before.txt`에 보관한다.

## 실제 소유권과 호출 경로

| 책임 | 이전 | 현재 |
| --- | --- | --- |
| 두 학습 단계, 48셀/12셀 ROI 선택, 표시 역할, 안내 문구 | Window 필드·메서드 | `FoundationLearnPresenter` |
| 전체 topic0 XAML, 시각 요소·렌더링, 520/620ms 타이머 | Window | `FoundationLearnView.xaml(.cs)` |
| Topic·공통 문서·창 닫기·콜백 주입·기존 API | Window | Window 유지, 새 View에 전달 |

호출은 `Window -> FoundationLearnView -> FoundationLearnPresenter`다.
Presenter는 Window/WPF/타이머 없이 검사할 수 있고, View는 실제 단독 호스트에서
검사한다. 새 인터페이스·Factory·별도 partial 파일은 없다. View의 partial은
XAML 생성 코드 조합이다. Window의8개 필드·16개 메서드를 제거하고 기존 공개
접근점28개는 같은 이름·타입으로 전달한다. Window C#1582→1318,
XAML1383→873이며 줄 수가 아닌 상태·정책·의존성 이동으로 완료를 판단했다.

일반 Topic 숨김은 기존처럼 재생을 유지한다. View 제거는 타이머를 멈추고
Tick을 해제하며 재호스팅은 단계·Tool 안내를 보존하고 자동 재생하지 않는다.
Tool 콜백 성공 후 안내를 변경하고 예외는 전달한다. 기존 Threshold 성공 시
숨겨진 Foundation 안내 갱신도 같은 View 메서드로 연결한다.
공통 Foundation 문서 버튼과 public Threshold Apply sender는 Window에 남긴다.

## 완료 기준과 검증

| 기준 | 실제 결과·근거 |
| --- | --- |
| 새 소유자가 실제 호출되고 이전 결합 제거 | `structure-proof.py`:117 PASS. 기존 API·원문 패널 XML·28전달·79무관 메서드·완료 Learn 소스28개 SHA 보존 |
| 학습 정책을 Window 없이 검증 | console `--learn-foundation-presentation-contract`:4그룹 PASS. 전체 단계·정확한 ROI 셀·채널 역할·문구·독립 인스턴스·Tool 안내 |
| 실제 화면·수명·외부 동작 계약 | Foundation contract/View 두 target PASS. 두 타이머 각각2회 unload/reload, 최초 tick1회, no-autoplay, 숨김 재생, Tool3개/예외/비활성, Threshold 연결, keyboard focus, Close |
| 기존 화면과 상태 보존 | 상태/DPI19기록 동일.12캡처의 이동 패널 픽셀 동일; 전체 프레임6/12 동일. 나머지6개는 변경하지 않은 공통 Expander 전환 영역만 다름 |
| 범위 빌드·인접 회귀 | 아래4빌드 PASS, Grayscale contract·Geometry UI PASS. 기존 불일치2건은 다음 절에 별도 기록 |
| 사용자 변경·완료 기록 보존 | `final-scope.txt`, `task.diff`, `git-status-after.txt`, `git-diff-check.txt`; 기존 파일의 변경 전후 SHA와 실제 허용 목록 확인 |

실행 명령(Dev root; TEMP/TMP는 이번 D: evidence/temp):

```powershell
dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Release -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false --no-restore
dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Release -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false --no-restore
dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false --no-restore
dotnet build tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj -c Release -p:Platform=x64 -m:1 -nr:false --no-restore
dotnet tools/VisionRecipeRunnerSmoke/bin/x64/Release/net8.0-windows7.0/VisionRecipeRunnerSmoke.dll --learn-foundation-presentation-contract <evidence>/unit
```

UI runner는 `run-baseline.ps1`, `run-baseline-focused.ps1`, `run-after.ps1`,
`run-after-legacy.ps1`에서 해당 DLL의 `--target ... <output> --quiet`를 호출했다.
실제 target과 PID·모니터·창 좌표는 각 폴더 `monitor-placement.json`에 있다.
빌드 로그는 `baseline-build.log`, `baseline-focused-build.log`, `after-build.log`,
`unit-build.log`, `debug-build.log`, `readiness-build.log`다. UI runner 기존
CS8600 경고1개, 나머지 빌드 경고0개·모두 오류0개다.

## 실패·차이·검증 한계

- 기존 `wpf_openvision_learn_curriculum`은 변경 전과 후 모두
  `LEARN_EDGE_BASED_MATCHING.md:73`의 `must not` 문구로 실패했다.
  실제 학습 검증 전에 실행되는 공통 문서 검사다. 문서와 검사 본문을 보존했다.
- 인접 `wpf_openvision_learn_color_hsv`는 한국어 practice 안내에 없는
  `Public_HSV_ColorPatch`, `explicit Run Review`를 기대해 실패했다.
  `OpenVisionLearnTopics.cs`의 안내 SHA, Window의 해당 값 전달 메서드 및
  기존 검사 본문이 시작 상태와 동일함을 확인했다. ColorHSV 전체 검증 PASS로
  주장하지 않는다. 두 불일치는 이 구조 작업의 범위 밖이며 자동 수정하지 않는다.
- 따라서 `run-after.ps1`은4 target PASS/1 ColorHSV FAIL로 exit1이다.
  전체 명령 성공으로 보고하지 않는다. `after-legacy-curriculum`도 exit1이다.
  구조 완료는 직접적인 Foundation 계약/독립 View/비교 및 입증된 이전 결합
  제거에 한정한다. 기존 테스트를 삭제하거나 기대값을 완화하지 않았다.
- 신규 기준 테스트의 최초 실행은 Topic 전환 후 WPF 화면 생성 대기를 빠뜨려
  Threshold 버튼을 찾지 못했다. dispatcher pump를 추가한 동일 기준 테스트가
  제품 변경 전에 통과했다. 실패 로그는 `baseline/`에 남겨 둔다.
- 최초 전체 PNG 동일성 검사는 MatChannel 프레임의 공통 실습 Expander 전환
  차이로 실패했다. `image-differences.json`은 차이가 x296..1007/y197..315의
  변경하지 않은 공통 header 내부임을 확인한다. 전체 프레임 동일성과 이동
  패널 동일성을 `comparison.txt`에 구분했다. 이미지 차이를 지우거나 덮지 않았다.
- fresh 전후 캡처에서 Foundation Tool 버튼 문구 잘림이 기존부터 보인다.
  해당 XAML과 픽셀은 동일하며 이번 변경에서 재설계하지 않았다. 전체 UI 품질
  완료가 아니다. 실제 hover/pressed/mouse-leave·물리 키보드 순환 탐색,
  모든 테마/Wide/Compact, DPI125/150/175/200%, 실제 제품 EXE, 전체 회귀 및
  Readiness 실행, Recipe/XML round trip은 미실행이다.

증거는 current-source WPF test hosting이며 실제 OpenVisionLab EXE 검증이 아니다.
동적 선택된 작은 왼쪽 `\\.\DISPLAY2`, bounds(-1920,365,1920,1080),
96DPI에서 실행했고 실제 창과 모니터 교차를 기록했다. 기존 bin/obj junction
및 테스트 출력/TEMP/TMP는 D:를 사용했다.

Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-learn-foundation-20260908`.
비교 사진: `baseline-focused/Foundation-step-3.png` ↔ `after/Foundation-step-3.png`.

## 변경 파일과 다음 작업 경계

- `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/`: 새 Foundation Presenter·View·XAML,
  기존 `OpenVisionLearnWindow.xaml(.cs)`.
- `tools/PipelineViewerScreenshotSmoke/`: 새 `LearnFoundationSmoke.cs`, Program dispatch.
- `tools/VisionRecipeRunnerSmoke/`: 새 `LearnFoundationPresentationContract.cs`, Program dispatch.
- `tools/OpenVisionReadinessCheck/Program.cs`: 이동한5개 XAML 검사에 명시적 새 경로 적용.
- `AGENTS.md`, `docs/LLM_DOCUMENT_INDEX.json`, `docs/admin/CODEBASE_STRUCTURE.md`,
  현재 Handoff 및 이 보고서: 완료 경계와 탐색 경로.

Boundary / next dependency: 이번 구조 범위의 남은 작업은 없다. 위 미검증/기존
불일치는 전체 품질 주장의 한계이며 자동 후속 작업을 뜻하지 않는다.
새 결함 재현, 명시적 요구 변경 또는 실제 책임 충돌이 없는 한 완료된 Foundation
및 기존 소유자를 다시 분리·이동·래핑하지 않는다. 재개 시 기존 완료 기록,
새 증거, 기존 소유자 내부 수정이 불충분한 이유, 최소 경계와 검증을 먼저 기록한다.

다른 모델·에이전트·예약 실행의 자동 코드/주석/문서/재검증 후속 작업은 없다.
15분 예약은 PAUSED 유지. CVR-00은 독립 첫 사용자3명의 자료가 필요하며 보류한다.
Original·stage·commit·push·merge·배포를 수행하지 않았다.

최종 범위는15개 파일이다. 별도 Skill producer-handoff 작업에서 갱신한 문서4개는
수정하거나 되돌리지 않고 이번 diff에서 제외했다. 정확한 경로와 SHA 범위는
`final-scope.txt`에 기록했다. 공동 Handoff의 다른 본문도 보존했다.
