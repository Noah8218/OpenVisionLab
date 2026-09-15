# Learn Grayscale 네 주제 구조 개선 — 2026-09-08

## 작업 계약과 변경 사유

사용자의 새 대규모 리팩토링 요청 범위에서 밝기/Histogram(1), Threshold(2),
Filtering(3), Arithmetic(14)을 한 묶음으로 완료한다. 기존 Binary/Line,
Matching, LayerRecipe, Metrics 경계는 완료 상태이며 재분리하지 않는다.
입력은 `C:\Git\2D\Dev`, `codex/public-sample-ux-docs`,
`d875559577c85984d54900df973a6fb35fb20146` 및 시작 시 dirty worktree다.
증거 루트는 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-learn-grayscale-20260908`.

현재 제품은 규칙 기반 검사 Recipe 워크벤치이며 Handoff의 기록된 환경에서
RC 수준이다. 명시적 실행, 판정 근거, 반복 가능한 Recipe를 보존한다.
CVR-00은 독립적인 초보 사용자3명의 관찰 자료가 필요한 별도 과제다.
하드웨어·계정·클라우드·배포·새 알고리즘·LLM/XML 검증은 이번 범위 밖이다.

| 항목 | 현재 -> 의도한 구조 |
| --- | --- |
| 계산 소유자 | 기존 `OpenVisionLearnBasicGrayscaleSimulationModel` 그대로 재사용 |
| 상태·표시 판단 | Window의 네 주제 단계/Threshold 왕복 방향/공식/설명/셀 의미 -> `GrayscaleLearnPresenter` |
| 화면·수명 | Window 컨트롤/그리기/타이머 -> `GrayscaleLearnView.xaml(.cs)` |
| 호출과 의존성 | Window 주제/초기값 -> View -> Presenter -> 기존 Model; 결과를 View가 렌더링 |
| 외부 동작 | View Apply/Close/Threshold Tool 완료 -> Window의 기존 이벤트 sender·Close·Foundation 안내 유지 |

Window에 세 단계 상태 머신, Threshold 왕복 방향과 MaxValue, 셀/히스토그램
표시 판단이 직접 남아 있으므로 독립 테스트 가능한 실제 책임 이동이다.
기존 계산 Model은 이미 올바른 소유자이며 파일 크기나 취향으로 재분리하지 않는다.
새 Interface/Factory/Wrapper/Message Bus, 의존성 버전 변경을 추가하지 않는다.

완료 조건과 순서:
1. 수정 전 현재 소스 baseline UI와 상태를 D:에 기록한다.
2. 네 주제 실제 소유권을 옮기고 Window의 기존 public facade·이벤트·문구를 유지한다.
3. 기존 Model/완료 경계는 SHA-256 동일, XAML 구조·ID·문구는 부착 행/새 host 외 동일하게 보존한다.
4. Presenter 계약, 기존 집중 UI, 안정화된 화면/상태 비교, View 분리/재결합과
   Threshold Apply/Close/Tool 순서, Debug 앱·Release runner·변경된 검사 도구를 검증한다.
5. 소유자 목록·Handoff·문서 경로에 실제 결과와 미검증 범위를 기록하고 종료한다.

알려진 주의점: Threshold는80ms마다 슬라이더가25..230 사이를5씩 왕복하며,
수동 설정/Invert는 재생을 멈추지 않는다. 나머지 세 주제의 수동 설정은
완료 단계3으로 이동 후 정지한다. 주제 숨김은 재생 유지, View 분리는 정지한다.
Threshold Tool은 기존 Foundation 안내를 갱신하는 숨은 경로가 있으므로 보존한다.
Threshold 탭과 하단 Apply/Close는 원래 서로 다른 Grid 행에 있어 host 배치와
키보드 이동을 집중 확인한다. 타이머 동작은 UI에 남기고 규칙은 Presenter가 소유한다.

두 보조 에이전트는 각각 D: Presenter/View 초안만 작성한다. 실제 저장소 통합,
테스트 실행, 공용 문서는 주 에이전트만 수정한다. 기존 사용자 변경은 보존한다.

## 중복 작업 방지와 종료 경계

완료 기록과 현재 호출 경로를 먼저 확인한다. 새로 재현된 결함, 명시적인 요구 변경,
입증된 책임 충돌이 없으면 기존 소유자를 재분리·이동·래핑하지 않는다.
재개 시 기존 완료 기록, 새 증거, 기존 소유자 내부 수정으로 해결할 수 없는 이유,
최소 경계, 보존 계약과 집중 검증을 먼저 기록한다. 이번 Grayscale도 완료 후 동일하다.
이번 요청은 예약 재개 승인이 아니다. 15분 예약은 PAUSED를 유지하고 종료 후
추가 코드·주석·문서·재검증이나 다른 모델의 자동 후속 작업을 시작하지 않는다.

## 완료 기록

Status: Complete
Scope: Learn Grayscale 네 주제의 상태·표시 정책·View 수명 경계와 기존 계약 보존.

| 완료 기준 | 관찰한 결과 |
| --- | --- |
| 실제 새 소유권/호출 경로 | `structure-proof.txt`:136 PASS. Window에서 관련26필드/43메서드 제거;35 facade와 public eventargs 유지 |
| 기존 경계·계산·화면 계약 | 기존 Learn 소스25개 SHA-256 동일, 무관 Window 메서드89개 동일,5패널 XML/문구/ID/이벤트 동등(외곽 Grid 행만 재배치) |
| 동작 보존 | Presenter6그룹/현재 UI9대상 PASS; 상태50개와 안정화 화면41개 byte-identical |
| 관련 검증과 기록 | Debug/Release 관련 빌드4개 PASS; 아래 실패·미실행 경계 명시 |
| 중복 작업 방지 | AGENTS 현재 범위, 구조 안내0/5.7 완료 소유자, 현재 Handoff와 index route 갱신 |

### 변경 파일

- `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/GrayscaleLearnPresenter.cs`: 단계·공식·설명·표시 역할과 Threshold 방향/MaxValue.
- 같은 폴더 `GrayscaleLearnView.xaml(.cs)`: 기존 네 주제의 컨트롤/renderer/timer. XAML 생성 조합 외 partial 분할은 없다.
- 같은 폴더 `OpenVisionLearnWindow.xaml(.cs)`: 실제 View 배치/초기화/주제 선택과35개 public facade 전달. Apply는 기존 Window 발신자, Close는 Window 소유.
- `tools/VisionRecipeRunnerSmoke/LearnGrayscalePresentationContract.cs`, `Program.cs`: 독립 계약6그룹과 전용 target.
- `tools/PipelineViewerScreenshotSmoke/LearnGrayscaleSmoke.cs`, `Program.cs`: baseline 상태/화면, 독립 View 재결합, Tool/Apply/Close 계약.
- `tools/OpenVisionReadinessCheck/Program.cs`: 계산 호출4개 검사 경로를 새 Presenter로 연결하고 기존 Threshold 탭의 실제 View 경로 확인. 이전 완료 영역의 stale 검사는 유지.
- `AGENTS.md`, `docs/admin/CODEBASE_STRUCTURE.md`, `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`, `docs/LLM_DOCUMENT_INDEX.json`, 이 보고서.

Window는 C#2,369 ->1,582줄, XAML2,206 ->1,383줄이다. 줄 수 자체를 완료 근거로
삼지 않는다. 기존 픽셀 계산 Model과 공유 resource를 재사용하며 정책/상태와
UI 이벤트·렌더링의 소유자가 실제로 달라진 것을 구조 검사로 확인했다.

### 실제 검증

출력/테스트 TEMP/TMP는 모두 위 D: 증거 루트다. 기존 앱/runner bin·obj는
D: 대상 junction임을 확인했다. 빌드는 모두 `--no-restore -m:1 -nr:false
-p:Platform=x64`이며, 앱·두 runner는 `-p:WpgCustomBuildEnabled=false`를 사용했다.

| 실행 명령/대상 | 결과와 증거 |
| --- | --- |
| `dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Debug` | PASS, 경고0/오류0; `debug-build.log` |
| `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Release` | baseline/변경 후 PASS; 기존 CS8600 경고1; `baseline-build.log`, `after-build.log` |
| `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Release` | PASS, 경고0/오류0; `unit-build.log` |
| `dotnet build tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj -c Release` | PASS, 경고0/오류0; `readiness-build.log` |
| `dotnet tools/VisionRecipeRunnerSmoke/bin/x64/Release/net8.0-windows7.0/VisionRecipeRunnerSmoke.dll --learn-grayscale-presentation-contract <evidence>/unit` |6 PASS/0 FAIL; `unit/learn-grayscale-presentation-contract.txt` |
| `powershell -NoProfile -ExecutionPolicy Bypass -File <evidence>/run-baseline.ps1` | 변경 전 UI7대상 PASS, exit0 |
| 같은 명령의 `run-after.ps1` | 변경 후 UI9대상 PASS, exit0; `after/stdout.log`, `monitor-placement.json` |
| `python <evidence>/structure-proof.py` |136 PASS/0 FAIL; `structure-proof.txt` |
| `python <evidence>/compare-evidence.py` | 상태50개와 안정화 캡처41개 일치; `baseline-comparison.txt` |

UI 대상은 `wpf_openvision_learn_brightness`, `..._filtering`, `..._arithmetic`,
`..._threshold`, `..._threshold_animation`, `..._threshold_apply`,
`..._grayscale_contract`, `..._grayscale_view`, `..._binary_line_contract`다.
기존 legacy6개와 인접 Binary/Line 계약은 변경하지 않았다.

단계0..3, 각 모드와 설정 경계값, Threshold0/127/255·반전·MaxValue255/64/비정상값
정규화, 수동 설정 중 재생 정책, 주제 숨김 중 진행, Pause/Close를 검사했다.
독립 View의 각 타이머2회 분리/재연결은 상태 유지·자동 재생 없음·중복 callback
없음을 검사했다. 실제5개 Tool 링크의 enum과 성공 후 안내, callback 실패 시
안내/이벤트 미갱신, Apply payload와 Window sender, 예외 전파, 명시적 Close도 통과했다.
기존 Threshold UI 검사에서 실제 키보드 Tab/역방향 순서를 검증했다.

### 차이와 미검증 범위

원본 PNG48장 중44장은 완전 동일하다. 나머지4장은 변경하지 않은 공통 실습
Expander의 전환 영역에서만 차이가 있었다(`legacy-image-difference.txt`).
범위는 각각(296,228)-(1007,283), (296,228)-(1007,312),
(296,230)-(1007,330), (296,230)-(1007,316)이다. 실제 Threshold 컨트롤과
하단 버튼의 차이는 관찰하지 못했다. 안정화된41개 비교는 모두 통과했으며,
원본48장 전체가 동일하다고 주장하지 않는다.

독립 검토도 기존 Window event sender/해제/Foundation fallback과3개 standalone
화면에 새 회귀를 발견하지 못했다. 화면 일부 하단은 기존 ScrollViewer viewport
밖에 있으므로 이 캡처만으로 모든 콘텐츠의 가시성을 입증하지 않는다.

현재 증거는96 DPI current-source WPF host rendering이다. 실제 desktop test 창을
동적으로 선택한 작은 왼쪽 `\\.\DISPLAY2` (bounds -1920,365,1920,1080)에
배치하고 창 사각형 교차를 기록했다. 실제 OpenVisionLab EXE, 전체 회귀와
Readiness 실행, Recipe/XML round trip, 모든 테마/Wide/Compact, DPI125/150/175/200%,
실제 hover/pressed/mouse-leave 전 상태 검증은 미실행이다. 제품 전체 UI/출시
검증 완료를 뜻하지 않는다.

최초 구조 검사 초안은 새 XAML의 명시적 `Grid.Row="0"`을 행 생략으로 가정했고,
추가한 Threshold XAML owner assertion을 허용 범위에서 빠뜨려5개 항목이 실패했다.
원래 설계의 명시적 행0과 해당 실제 assertion을 정확히 검사하도록 초안을
수정해136개가 통과했다. 제품/기존 테스트를 삭제하거나 기대값을 완화하지 않았다.

### 종료와 재개 조건

완료된 Grayscale 경계도 기존 소유자 목록에 등록했다. 확실한 새 증거 없이
재분리하지 않는다. 다음 자동 작업은 없다. 별도 명시적 요청 전에는 다른 모델,
에이전트 또는 예약 실행이 코드·주석·문서·재검증을 이어가지 않는다.
기존15분 예약은 PAUSED를 유지한다. Original·stage·commit·push·merge·배포를
수행하지 않았다. 최종 변경 목록/해시 확인은 `final-scope.txt`, 이번 변경 diff는
`task.diff`, git 상태는 `git-status-after.txt`에 기록한다.

이번 변경은15개 파일이다. 별도 작업의 스킬 문서4개와 Handoff의 Skill Static
Development 완료 갱신은 보존하고 이번 diff에서 제외했다. 해당 경로와 내용은
`final-scope.txt`, `independent-handoff-preserved.md`로 구분했다.
