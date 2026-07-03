# Next Codex Prompt

```text
작업 위치는 C:\Git\OpenVisionLab_Dev 입니다.

먼저 아래 문서를 읽고 현재 상태를 파악한 뒤 작업을 시작해 주세요.

1. C:\Git\OpenVisionLab_Dev\CODEX_RECOVERY.md
2. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md
3. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_PRODUCT_IDENTITY_AND_ROADMAP.md
4. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_UX_COMPETITOR_REVIEW.md

현재 OpenVisionLab의 제품 정체성은 레이어 기반 rule-based vision workbench입니다.

반드시 지킬 방향:

- View code-behind에는 업무 로직을 최소화하고 ViewModel, Controller, Presenter, Behavior, Converter, Runtime/Service로 분리합니다.
- 알고리즘 툴은 PropertyGrid 기반 구조를 유지합니다. 모델 Property를 PropertyGrid SelectedObject에 넣으면 UI가 자동 생성되는 구조가 제품 방향입니다.
- Output 레이어 생성이 Input 레이어를 자동 변경하면 안 됩니다.
- Boolean visibility toggle만으로 Preview/Run이 실행되면 안 됩니다.
- Viewer zoom/pan/drag, ROI overlay, template editor, 레이어 비교/도킹 기능을 제거하지 마십시오.
- 이미 안정 계약 문서에 기록된 동작은 임의로 바꾸지 마십시오.
- Dirkster.AvalonDock PackageReference를 OpenVisionLab.csproj에 직접 추가하지 마십시오.
- AvalonDock 패키지 소유권은 Library\OpenVisionLab.Docking.Controls\OpenVisionLab.Docking.Controls.csproj에 둡니다.

현재 도킹 상태:

- 도킹 레이아웃의 핵심 UX는 1차 완료로 판단합니다.
- ShellHost는 wrapper인 OpenVisionLayerDockWorkspaceView를 사용합니다.
- AvalonDock raw 세부 구현은 Library\OpenVisionLab.Docking.Controls 쪽으로 이동 중입니다.
- Layer comparison document는 native floating preview가 workspace를 가리지 않도록 CanFloat=false를 사용합니다.
- Docking movement는 wrapper-owned gesture/guide/drop path가 담당합니다.
- tab click만으로 guide가 뜨면 안 됩니다.
- drag 중 WPF Drop 이벤트가 누락되면 wrapper가 현재 cursor 위치 기준으로 dock command를 finalize할 수 있습니다.
- global bottom은 전체 workspace 하단 split, pane-local bottom은 대상 pane 내부 하단 split입니다.
- 도킹 변경 후에는 tools\RunDockingVerification.ps1를 우선 검증으로 사용합니다.

현재 완료로 취급할 영역:

- Matching PropertyGrid UX
- PropertyGrid editor 기본 정책
- Preview/Run visibility toggle 분리
- Startup empty workspace prompt
- Docked layer top tab/header 구조
- Docking guide on-click suppression
- Native floating preview suppression
- Pipeline preview/publish separation
- Sample catalog 기본 검증 구조
- AI Recipe safe-fix 기본 정책

현재 UX 방향:

- 도킹 레이아웃 핵심 UX는 1차 완료로 판단합니다.
- 다음 UX 개선은 도킹 구조 자체가 아니라 `도킹된 Tool View에서 이미지를 보며 검증하는 흐름`입니다.
- PropertyGrid 기반은 유지하되, Tool View에 초보자용 검증 흐름, 결과 해석, 다음 행동 안내를 보강합니다.
- Matching 도킹 Tool View를 첫 기준 화면으로 삼습니다.
- Matching result review에는 1차로 Preview OK/NG, Criteria, reason, next action guidance가 추가됐습니다.
- `wpf_shell_host_matching_tool_docked_verification` smoke가 추가됐고 통과했습니다.
- Main View는 이미지 없음/첫 실행 상태에서 이미지 열기, 샘플 열기, 최근 Recipe, 튜토리얼 시작 진입점을 더 명확히 해야 합니다.

다음 우선순위:

1. Matching 도킹 Tool View UX 개선 2차
   - PropertyGrid는 계속 유지합니다.
   - result guidance 1차는 완료됐으므로 다음은 teach/template 영역과 beginner 핵심 파라미터 요약을 더 명확히 합니다.
   - 화면 smoke는 `wpf_shell_host_matching_tool_docked_verification`을 우선 사용합니다.

2. Main View beginner workflow 개선
   - 이미지 없음 상태에서 샘플/이미지/최근 Recipe/튜토리얼 진입점을 중앙 workspace에 명확히 표시합니다.
   - 하단 Log는 기본 요약/접힘 상태를 우선하고 오류/경고가 있을 때 상세 확장을 유도합니다.

3. 문서/트래커 정리
   - CODEX_RECOVERY, progress/completed tracker, NEXT_CODEX_PROMPT의 오래된 지시와 깨진 인코딩 문구를 최신 상태 기준으로 정리합니다.

4. ShellHost MVVM 경계 정리
   - ShellHost 생성자 주변 의존성 조립을 app-local factory/builder로 더 이동합니다.
   - language changed 처리, refresh coordinator, command surface wiring을 controller/coordinator로 분리합니다.
   - ShellHost는 view wiring과 wrapper 연결 위주로 줄입니다.

5. Tool View code-behind 축소
   - PropertyGrid 기반은 유지합니다.
   - 단일 입력 PropertyGrid tool의 반복 preview scheduling, parameter commit, summary update 흐름을 공통 runtime/controller/template로 정리합니다.

6. Algorithm sample-backed reliability 확장
   - pin, die-pad, surface defect, line/measurement Good/Bad pair를 추가합니다.
   - 각 sample은 score/count/bounds/angle/length/mean 같은 metric gate를 가져야 합니다.

7. Pipeline/Recipe operator review UX
   - step별 input/output image, branch reason, expected metric, actual metric을 더 명확히 보여줍니다.

검증 기준:

1. 기본 빌드:
   dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"

2. 도킹 관련 변경:
   powershell -ExecutionPolicy Bypass -File tools\RunDockingVerification.ps1

3. PropertyGrid/tool view 관련 변경:
   powershell -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 -WpfTools -FailOnWarn

4. Pipeline/sample/runner 관련 변경:
   powershell -ExecutionPolicy Bypass -File tools\RunVisionPlatformPrecheck.ps1

작업 완료 후 한국어로 아래 항목을 보고해 주세요.

- 변경한 파일
- 구조 변경 내용
- 검증 결과
- 남은 위험/후속 작업
- 다음 우선순위
```

## 2026-06-30 readable status update

- Matching Tool View UX first pass is complete: result review shows Preview OK/NG, criteria, reason, and next action.
- Matching Tool View UX second pass is complete: compact verification guide shows `���� �帧`, Preview OK/NG, `�հ� ����`, and `����` action above the PropertyGrid.
- Both `wpf_shell_host_matching_tool` and `wpf_shell_host_matching_tool_docked_verification` passed after the guide assertion was added.
- Next practical priority: Main View beginner workflow. Before changing UI/UX, capture a before screenshot artifact, then produce an after artifact from the same scenario and compare them in the final report.
