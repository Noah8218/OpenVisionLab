# Learn Metrics / Acceptance 책임 분리 — 2026-09-08

## 작업 계약

사용자의 새 “이어서 진행” 요청으로 독립 검증 가능한 다음 리팩토링 1건을
진행한다. 이전 Matching·Layer/Recipe 완료 범위는 재작업하지 않는다.
이번 범위의 구현·검증·주석·문서 갱신을 끝내면 자동 후속 작업을 중지하며,
기존 15분 예약 `openvisionlab-2d`는 `PAUSED`를 유지한다.

입력: `C:\Git\2D\Dev`, branch `codex/public-sample-ux-docs`,
HEAD `d875559577c85984d54900df973a6fb35fb20146`와 기존 dirty worktree.
증거: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-learn-metrics-acceptance-20260908`.
수정 전 파일 snapshot·SHA-256 목록·git status를 보존한다. 테스트 출력과
TEMP/TMP는 D:에 두며 기존 build-output junction을 사용한다.

| 현재 소유자 | 새 소유자 | 이동 책임 |
| --- | --- | --- |
| Learn Window | `MetricsAcceptanceLearnPresenter` | 고정 학습 샘플, 평균·범위·최대값, 단계·판정·설명·강조 결정 |
| Learn Window | `MetricsAcceptanceLearnView.xaml(.cs)` | 주제 컨트롤·namescope·셀 렌더링·타이머·Loaded/Unloaded |
| Learn Window | 기존 Window | 주제 선택과 창 종료; View 갱신·정지를 전달 |
| LearnResources / LearnCellVisuals | 기존 소유자 유지 | 스타일·브러시·셀 생성 재사용 |

호출 흐름: Window 주제 선택 -> View 갱신 -> Presenter 결과 -> View 렌더링.
Step/Reset은 Presenter 상태를 바꾸고 View가 표시한다. 타이머와 이벤트 수명은
View가 소유한다. Presenter는 WPF·실제 검사 실행·Recipe 저장에 의존하지 않는다.
새 인터페이스·Factory·Wrapper·Message Bus·Window partial은 추가하지 않는다.

완료 조건: 실제 새 소유자 사용과 이전 결합 제거, 기존 0..3단계 계산·문구·강조·
초기 완료 상태·Reset/Play/Pause/재시작 보존, 독립 View 수명 검증, 현재 빌드와
변경 전후 화면 비교, 개발자 탐색 문서와 종료 기록. 새로운 UI·수업 내용·알고리즘·
Recipe/XML/SDK 변경, Original·commit·push·merge·배포는 범위 밖이다.

기존 Metrics UI smoke는 영문 `Range/Max gate` 상태 문구를 기대하지만 현재
제품은 한국어 문구를 표시한다. 수정 전에 실행해 실제 실패를 확인하고,
변경 후 같은 실패인지 확인한다. 기대값 완화나 테스트 삭제로 통과시키지 않는다.

## 구현 결과

Window에서 학습 데이터·계산·판정·설명·단계 소유권을 Presenter로 옮겼다.
View는 Presenter 결과를 셀·텍스트로 표시하며, 실제 Window XAML이 새 View를
생성한다. Window의 기존 공개 테스트 진입점은 새 View로 전달한다.
기존 XAML 컨트롤명·AutomationId·문구·스타일은 유지했다. 새 partial은 독립
UserControl의 XAML 생성 코드 결합용이며, 기존 Window partial 분할이 아니다.

View는 Loaded에서 타이머를 구독하고 Unloaded에서 정지·구독 해제한다.
다시 붙이면 학습 단계와 펼친 설명 패널을 유지하지만 자동 재생하지 않는다.
Window Close도 View 정지를 호출한다. 수명 소유권은 코드 주석으로 설명했다.
`CODEBASE_STRUCTURE.md` 5.5에는 소유자 표·호출 흐름과
“평균 OK인데 최종 NG인 이유를 어디서 찾는가”라는 탐색 예제를 추가했다.

`structure-proof.txt`의 60개 검사가 새 소유자 사용, Window의 이전 상태·메서드·
컨트롤 참조 제거, Presenter의 WPF/실행 의존성 부재, View의 계산·판정 상수 제거,
XAML 및 설명 literal 보존을 확인한다. Window C#은 3,818 -> 3,671줄,
XAML은 3,293 -> 3,198줄이다. 줄 수 감소가 아닌 책임과 의존성 이동을 검증했다.
기존 Matching·Layer/Recipe View/Presenter와 공유 LearnResources/LearnCellVisuals는
시작 snapshot의 SHA-256과 동일하다.

## 변경 파일

이 작업의 파일은 아래 14개다. 경로는 `C:\Git\2D\Dev` 기준이다.

| 파일 | 변경 |
| --- | --- |
| `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/MetricsAcceptanceLearnPresenter.cs` | 학습 상태·계산·판정·설명 소유자 추가 |
| 같은 폴더의 `MetricsAcceptanceLearnView.xaml`, `MetricsAcceptanceLearnView.xaml.cs` | 독립 화면·컨트롤·타이머 수명 추가 |
| 같은 폴더의 `OpenVisionLearnWindow.xaml`, `OpenVisionLearnWindow.xaml.cs` | 실제 View 연결과 기존 소유권 제거 |
| `tools/VisionRecipeRunnerSmoke/LearnMetricsAcceptanceContract.cs`, `Program.cs` | 비시각 계약 3그룹과 선택 실행 진입점 |
| `tools/PipelineViewerScreenshotSmoke/LearnMetricsAcceptanceSmoke.cs`, `Program.cs` | 변경 전에도 실행 가능한 화면 계약, 독립 View 수명 검사와 선택 실행 진입점 |
| `AGENTS.md`, `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md` | 이번 완료 범위와 자동 후속 작업 금지 |
| `docs/admin/CODEBASE_STRUCTURE.md`, `docs/LLM_DOCUMENT_INDEX.json` | 개발자 탐색 경로 |
| 이 보고서 | 작업 계약·검증·종료 기록 |

시작 이후 별도 변경된 Skill Registry, Skill 개발 Work Contract,
`OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_13_RETRY2_RESULT_20260908.md`는 이 작업에서
수정하지 않았다. 현재 인계 문서와 색인의 별도 XML 평가 기록도 보존했다.
`task-changes.csv`는 시간 구간 내 모든 변경을 담으므로 그 외부 변경까지 포함한다.
이 보고서는 별도 XML 평가의 결과나 저장소 독점 사용을 주장하지 않는다.

## 실제 검증

아래 증거 경로는 위 D: 증거 폴더 기준이다. 각 build 명령은 저장소 루트에서
`-p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false --no-restore`를 사용했다.

| 실행 | 결과 | 증거 |
| --- | --- | --- |
| 변경 전 `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Release` | 통과, 기존 CS8600 경고 1개 | `baseline-build.log` |
| 변경 전 화면 계약 | 통과: 단계·수식·표시값·강조색·설명 펼침·재생·주제 복귀·종료 | `baseline/stdout.log`, `.contract.txt` |
| 변경 후 같은 Release build | 통과, 기존 CS8600 경고 1개 | `after-ui-build.log` |
| `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Release` | 통과, 경고/오류 0 | `unit-build.log` |
| `dotnet exec tools/VisionRecipeRunnerSmoke/bin/x64/Release/net8.0-windows7.0/VisionRecipeRunnerSmoke.dll --learn-metrics-acceptance-contract <증거>/unit` | 3/3 통과: 고정 샘플 계산·판정, 단계·Reset·재시작, 인스턴스 독립성·숫자 표기 | `unit.log`, `unit/learn-metrics-acceptance-contract.txt` |
| `dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Debug` | 통과, 경고/오류 0 | `app-debug-build.log` |
| 변경 후 `wpf_openvision_learn_metrics_acceptance_contract` | 통과: 초기 3, 0..3 단계, 평균 OK/범위·최대값 NG, 셀 색상·문구, Play/Pause/완료/재시작, 주제 복귀, Close | `after/stdout.log`, 해당 `.contract.txt` |
| 변경 후 `wpf_openvision_learn_metrics_acceptance_view` | 통과: 독립 View 생성, 두 번 분리/재결합, 단계·펼침 유지, 자동 재생·중복 Tick 없음, 독립 host Close | `after/` View `.contract.txt` |
| 변경 후 `wpf_openvision_learn_layer_recipe_contract` | 통과: 인접 주제 회귀 | `after/stdout.log` |
| `verify-structure.py` | 구조 60/60 통과, 화면 5/5 SHA-256 일치 | `structure-proof.txt`, `frame-parity.csv` |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools/TestDocumentationIndex.ps1 -RepoRoot C:\Git\2D\Dev` | 통과: 192 paths, 13 routes, 102 redirects | `documentation-index.log` |

UI 실행은 증거 폴더의 `run-baseline.ps1`, `run-after.ps1`가 현재 smoke DLL에
`--target <선택 대상> <출력 폴더> --quiet`를 전달했다. 프로덕션 EXE가 아닌
현재 소스를 빌드한 WPF View capture다. 실행 전 모니터를 동적으로 탐지해 작은
왼쪽 `\\.\DISPLAY2`(bounds `-1920,365,1920,1080`, working area `-1920,365,1920,1032`)에
테스트 PID의 창만 배치했고 실제 창 영역을 확인했다. 각 `monitor-placement.json`에
기록했다. 단위 테스트에는 모니터 제약을 적용하지 않았다.

이번 변경 전 실제 Release 소스에서 새로 캡처한 단계 0..3 및 펼친 설명 패널
5장을 변경 후 캡처와 비교했고 모두 byte-identical이다. 변경 전후 3단계 이미지를
직접 표시·검토했다. 범례, 샘플값·판정 문구, 버튼, 설명 패널은 비교 화면에서
동일하다. 캡처는 기존 Expander 전환이 안정된 뒤 수행했다.

### 기존 실패와 검증 한계

기존 `wpf_openvision_learn_metrics_acceptance`는 변경 전후 모두
`Metrics / Acceptance topic did not expose the animated outlier gate.`로 실패했다.
기대하는 `Range/Max gate`가 현재 한국어 상태 문구에 없기 때문이다.
각 `stderr.log`와 변경 전 source snapshot이 근거이며, 기존 테스트 메서드와
기대값 전체가 그대로임을 구조 검사로 확인했다. 새 화면 계약은 실제 기존 동작을
추가 검증하며 기존 실패를 삭제·대체·완화하지 않았다. 따라서 legacy 대상을 포함한
두 UI 실행의 전체 종료 코드는 1이다. 전체 smoke 통과를 주장하지 않는다.

실제 Runtime 검증은 96 DPI(100%), Learn 1040x700, 독립 host 760x700에서 수행했다.
정상 표시·판정 단계·재생 상태·설명 펼침/접힘·주제 가시성·재결합·종료와
Tool/sample/apply callback 0회를 확인했다. 버튼은 routed Click으로 실행했다.
실제 포인터 hover/pressed/focus/keyboard 경로, 전체 theme/Wide·Compact,
125/150/175/200% DPI, 모든 resize/maximize 경우는 미검증이다. 이번 구조 보존
검증을 전체 UI 사용성·상용 GA 또는 실제 검사 Recipe 정확성으로 확대하지 않는다.

## 종료 기록

Status: Complete

Scope: Metrics / Acceptance 학습 주제의 Presenter/View 책임 분리, 기존 Window
연결·표시·단계 동작 보존, 집중 검증·주석·개발자 문서까지 완료.

Acceptance criteria:
- 새 상태/화면 소유자를 실제 사용하고 이전 결합 제거 -> 구조 60개 및 독립 View 검증 통과.
- 기존 계산·설명·판정·단계·재생·종료 유지 -> 비시각 3그룹 및 화면 계약 통과.
- 기존 화면과 인접 주제 연결 유지 -> 5장 동일 및 Layer/Recipe 회귀 통과.
- 관련 빌드·문서 경로 검증 -> 통과, 위 로그.
- 종료 이후 자동 후속 작업 금지 -> AGENTS·현재 Handoff·이 기록에 명시, 예약 PAUSED 유지.

Verification / Evidence: 위 명령·로그·화면과 최종 `final-git-check.txt`,
`git-status-final.txt`, `task-snapshot.diff`, `task-changes.csv`. 기존 사용자 변경을
보존했고 삭제한 baseline 파일은 없다. 입력 branch/HEAD는 유지했다.
commit·push·merge·Original 변경·배포·예약 재개·에이전트 위임은 하지 않았다.

Boundary / next dependency: 자동으로 이어서 수행할 다음 작업은 없다. 새 사용자
요청 전에는 다른 모델/예약 실행이 코드·정리·주석·문서·재검증을 추가하지 않는다.
예약 재개는 별도 명시적 요청이 필요하다. 기존 smoke 문구 불일치와 미실행 UI
범위도 자동 후속 작업의 승인이 아니다.
