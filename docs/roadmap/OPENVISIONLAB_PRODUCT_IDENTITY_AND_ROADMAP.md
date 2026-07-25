# OpenVisionLab Product Identity And Roadmap

Updated: 2026-06-30

이 문서는 도킹 레이아웃 안정화 이후 OpenVisionLab의 제품 정체성, 현재까지 완료된 기반, 앞으로 진행할 과제를 한 화면에서 파악하기 위한 상태 문서입니다.

## 1. 제품 정체성

OpenVisionLab은 단순 이미지 뷰어가 아니라 **레이어 기반 rule-based vision workbench**입니다.

사용자는 이미지를 `Main`, `Preview`, `Output`, `Result` 같은 명명된 레이어로 관리하고, 각 툴의 입력/출력 레이어를 명시적으로 선택하며, 결과를 비교한 뒤 검증된 단계만 Pipeline/Recipe로 확정합니다.

제품의 핵심 방향은 다음과 같습니다.

- **명시적 레이어 흐름**: Output 레이어가 생겼다고 Input 레이어가 자동 변경되면 안 됩니다.
- **Preview와 Publish 분리**: Preview는 검토 상태이고, 실제 workspace/pipeline 반영은 사용자가 명시적으로 결정해야 합니다.
- **PropertyGrid 기반 teaching**: 알고리즘 툴은 모델 property를 `PropertyGrid.SelectedObject`에 넣어 UI가 생성되는 구조를 유지합니다.
- **검사 결과의 설명 가능성**: OK/NG, score, count, box, angle, tact, metric failure reason을 사용자가 확인할 수 있어야 합니다.
- **WPF/MVVM 구조 정리**: View code-behind는 view wiring만 담당하고, 업무 로직은 ViewModel, Controller, Presenter, Behavior, Converter, Runtime/Service로 분리합니다.
- **실제 EXE 검증 우선**: UX는 빌드 성공만으로 완료 판단하지 않고, 실제 실행 smoke와 화면 증거를 기준으로 안정화합니다.

## 2. 안정 계약

아래 동작은 현재 제품 방향의 기반입니다. 새 작업에서 임의로 되돌리면 안 됩니다.

- Blob, Contour, Line, Matching, EdgeBasedMatching, FeatureMatching은 PropertyGrid 기반 구조를 유지합니다.
- Boolean visibility toggle은 child row 표시/숨김만 담당하고 Preview/Run을 실행하지 않습니다.
- Matching은 기본 manual preview이며 `AUTO_PREVIEW=true`일 때만 명시적으로 자동 preview 정책을 탑니다.
- ROI/template editor는 활성 WPF Shell display context를 사용해야 합니다.
- Viewer zoom, pan, drag, ROI overlay, output click sync는 유지해야 합니다.
- Layer comparison docking은 중앙 workspace의 주요 사용성 기능입니다.
- Docking에서 global bottom은 전체 workspace 하단을 가져가고, pane-local bottom은 대상 pane 내부 하단만 가져갑니다.
- Docked layer tab click만으로 docking guide가 뜨면 안 됩니다.
- Layer dragging 중 큰 AvalonDock native floating document preview가 workspace를 가리면 안 됩니다.
- AvalonDock 패키지 소유권은 `Library/OpenVisionLab.Docking.Controls`에 둡니다. `OpenVisionLab.csproj`에 직접 `Dirkster.AvalonDock`을 추가하지 않습니다.

상세 계약은 `docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`를 우선합니다.

## 3. 현재까지 완료된 기반

### UX 방향 정리

- 공식 경쟁 제품 자료를 기준으로 메인 워크벤치와 도킹 Tool View UX를 자체평가했습니다.
- 상세 내용은 `docs/OPENVISIONLAB_UX_COMPETITOR_REVIEW.md`에 정리했습니다.
- 결론은 PropertyGrid 기반 구조를 유지하되, 초보자용 `검증 흐름`, `결과 해석`, `다음 행동`을 Tool View에 보강하는 방향입니다.
- 첫 구현 대상은 Matching 도킹 Tool View가 적합합니다. Matching은 template, ROI, score/count/angle/scale, result overlay, Pipeline 추가까지 핵심 UX 요소를 모두 포함합니다.

### WPF Shell / MVVM 구조

- WPF Shell이 제품의 active UI 경로가 되었습니다.
- ShellHost의 많은 직접 이벤트/명령 처리가 command surface, controller, presenter, runtime factory로 이동했습니다.
- Native WPF tool registry/factory/runtime 경로가 생겨, 새 WPF tool 추가 시 반복 wiring을 줄일 수 있습니다.
- Tool floating/docking inspector는 명시적 operator action으로 유지됩니다.

### PropertyGrid 기반 Tool UX

- Blob, Contour, Line, Matching, FeatureMatching, EdgeBasedMatching 계열에서 PropertyGrid 기반 teaching 구조가 안정화되었습니다.
- RangeEditor min/max, duplicate companion row hiding, transient numeric typing, command-time commit이 안정 계약으로 정리되었습니다.
- Matching PropertyGrid UX는 사용자 검증 완료 영역입니다.
- Template editor, ROI editor, result review, manual/auto preview 정책이 문서화되었습니다.

### Layer / Viewer / Docking

- 중앙 workspace는 live layer를 AvalonDock 기반 dock workspace에 mirrored tab으로 표시합니다.
- Layer comparison pane은 top-aligned tab/header를 사용합니다.
- Global split, pane-local split, center/tab merge, nested restore, repeated move restore가 actual EXE gate로 검증됩니다.
- Native floating preview suppression이 적용되어 drag 중 큰 detached image window가 workspace를 가리지 않습니다.
- Docking 검증 기준은 `tools/RunDockingVerification.ps1`입니다.

### ImageCanvas / Viewer / Large Image

- OpenGL 기반 viewer path가 workspace, docked layer, popout, tool preview에 걸쳐 정리되었습니다.
- 16K large-image viewer path는 tile 기반으로 실사용 가능한 수준까지 최적화되었습니다.
- ROI overlay, zoom/pan/drag, pixel/GV status, layer viewer compact mode가 주요 안정 영역입니다.

### Pipeline / Recipe / Runner

- Pipeline Preview와 Publish가 분리되었습니다.
- Pipeline step input/output/branch 흐름이 명시적으로 정리되었습니다.
- XML recipe, sample catalog, external runner/DLL 방향이 문서화되고 smoke 기반 검증이 추가되었습니다.
- Tool result contract가 `VisionToolResult`, `ResultStatus`, `ErrorCode`, metrics, overlays 중심으로 정리되었습니다.

### AI Recipe / Sample Catalog

- AI Recipe prompt/safe-fix/revert/apply-preview 흐름이 기본 계약을 갖췄습니다.
- Good/Bad sample pair catalog와 expected metric 기반 검증이 도입되었습니다.
- 자동 튜닝은 기본값이 아니며, operator confirmation과 sample-backed contract가 필요합니다.

## 4. 현재 완료로 판단할 수 있는 항목

아래 항목은 구체적인 회귀 증거가 없으면 일반 리팩토링 과제로 다시 열지 않습니다.

- Matching PropertyGrid UX
- PropertyGrid editor 기본 정책
- Preview/Run visibility toggle 분리
- WPF tool floating placement 및 dock/float cycle 기본 동작
- Layer docking layout의 핵심 UX
- Startup empty workspace prompt
- Docked layer top tab/header 구조
- Docking guide on-click suppression
- Native floating preview suppression
- Pipeline preview/publish separation
- Sample catalog 기본 검증 구조
- AI Recipe safe-fix 기본 정책

## 5. 아직 남은 주요 과제

### 1. 문서/트래커 정리

현재 `CODEX_RECOVERY.md`, `NEXT_CODEX_PROMPT.md`, tracker 문서 일부에 과거 내용과 깨진 한글 인코딩이 섞여 있습니다.

남은 작업:

- 최신 제품 방향을 기준으로 인계 문서를 간결하게 재정렬합니다.
- 완료된 항목은 `OPENVISIONLAB_COMPLETED_TRACKER.md`로 이동하고, 진행 중 항목은 `OPENVISIONLAB_PROGRESS_TRACKER.md`에 남깁니다.
- 오래된 도킹 지시처럼 현재 안정 계약과 충돌하는 문구를 제거합니다.

### 2. ShellHost MVVM 경계 마무리

도킹 wrapper는 안정화됐지만 ShellHost는 아직 조립 책임이 큽니다.

남은 작업:

- app-local factory/builder로 ShellHost 생성자 주변 의존성 조립을 더 줄입니다.
- language changed 처리, refresh coordinator, command surface wiring을 더 명확한 coordinator/service로 분리합니다.
- ShellHost는 view wiring과 wrapper 연결만 남기는 방향으로 줄입니다.

### 3. Tool View code-behind 축소

PropertyGrid 구조는 유지하되, tool view code-behind에 남은 반복 controller/presenter wiring을 정리해야 합니다.

남은 작업:

- 단일 입력 PropertyGrid tool의 공통 runtime/template 재사용 범위를 넓힙니다.
- Matching/Blob/Contour/Line 계열의 반복 preview scheduling, summary update, parameter commit 흐름을 공통 controller로 유지합니다.
- 단, PropertyGrid를 hand-coded parameter panel로 바꾸지 않습니다.

### 3-1. Tool-docked verification UX 개선

오른쪽 또는 옆에 도킹한 Tool View에서 이미지를 보며 검증하는 흐름은 제품의 핵심 사용성입니다.

남은 작업:

- Matching을 기준 화면으로 삼아 Tool View에 `입력/출력`, `가르치기`, `핵심 파라미터`, `고급 PropertyGrid`, `결과 해석`, `다음 행동` 순서를 명확히 만듭니다.
- PropertyGrid는 유지하되, 초보자가 먼저 조정할 핵심 파라미터와 고급 파라미터를 시각적으로 구분합니다.
- Preview 후 결과 카드가 OK/NG 이유, threshold 대비 actual value, count/score/box/angle/scale/tact, 다음 조정 후보를 보여주게 합니다.
- Tool View는 이미지 workspace를 과도하게 가리지 않아야 하며, 실제 EXE 또는 focused screenshot smoke로 확인합니다.

### 3-2. Main View beginner workflow 개선

메인뷰는 초보자가 첫 실행부터 검사 흐름을 이해할 수 있어야 합니다.

남은 작업:

- 이미지 없음 상태에서 `이미지 열기`, `샘플 열기`, `최근 Recipe`, `튜토리얼 시작` 진입점을 중앙 workspace에 명확히 표시합니다.
- 왼쪽 Tool List에 검색, 즐겨찾기, 최근 사용, 추천 순서를 검토합니다.
- 하단 Log는 기본 요약/접힘 상태를 우선하고, 오류/경고가 있을 때 상세 확장을 유도합니다.
- 상단 상태 배너는 실행 결과만 간결히 표시하고, 상세 해석은 Tool View의 result explanation에 둡니다.

### 4. Algorithm reliability 확장

현재 샘플 기반 검증은 기반이 있으나, 실제 검사 품질은 더 넓은 sample pair가 필요합니다.

남은 작업:

- pin, die-pad, surface defect, line/measurement 계열 Good/Bad pair를 확장합니다.
- 각 sample은 score/count/bounds/angle/length/mean 같은 설명 가능한 metric gate를 가져야 합니다.
- Matching/EdgeBasedMatching 성능 옵션은 sample-backed benchmark 없이는 default로 승격하지 않습니다.

### 5. Pipeline / Recipe UX 고도화

Pipeline은 기능 기반은 있으나 operator가 긴 recipe를 빠르게 이해하는 UX는 더 다듬어야 합니다.

남은 작업:

- Step별 input/output image, branch reason, expected metric, actual metric을 더 명확히 보여줍니다.
- Add Pipeline 시 recommended input은 도와주되, branch input은 명시 확인을 요구합니다.
- Recipe XML persistence는 ROI/template/mask/tool state까지 계속 검증해야 합니다.

### 6. Release / external dependency 품질

외부 runner와 package/version 정책은 문서화되어 있지만, 실제 배포 기준은 더 엄격히 잠가야 합니다.

남은 작업:

- 새 PC에서 external roots, source references, prepared DLL fallback을 빠르게 진단하는 preflight를 강화합니다.
- release artifact와 tag/commit/version evidence를 일관되게 남깁니다.

## 6. 다음 우선순위

권장 순서는 다음과 같습니다.

1. **Matching 도킹 Tool View UX 개선**
   - 이유: 현재 사용자가 가장 많이 검증하는 흐름이며, 이미지와 Tool View를 함께 보며 parameter/result/pipeline을 판단하는 제품 핵심 UX입니다.

2. **Main View beginner workflow 개선**
   - 이유: 초보자가 첫 실행부터 이미지 로드, 샘플, 툴 선택, Preview, Pipeline 추가의 차이를 이해해야 합니다.

3. **문서 정리와 최신 인계 프롬프트 확정**
   - 이유: 오래된 도킹 지시와 깨진 문서가 다음 작업자를 잘못된 방향으로 유도할 수 있습니다.

4. **ShellHost MVVM 경계 정리**
   - 이유: 도킹 wrapper가 안정화됐으므로 이제 ShellHost 조립 책임과 code-behind 의존성을 줄이는 것이 맞습니다.

5. **Tool View 공통 runtime/template 정리**
   - 이유: PropertyGrid 기반 제품 방향은 유지하되, 반복 view code를 줄여 새 tool 추가 비용을 낮춰야 합니다.

6. **Algorithm sample-backed reliability 확장**
   - 이유: 제품 완성도는 UI 구조만으로 끝나지 않고 실제 검사 품질과 metric 설명력으로 결정됩니다.

7. **Pipeline/Recipe operator review UX**
   - 이유: 사용자가 recipe 단계별 입력/출력/판정 이유를 빠르게 이해해야 실제 현장 사용이 가능합니다.

## 7. 검증 기준

변경 범위별 최소 검증은 다음을 기준으로 합니다.

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
```

도킹 관련 변경:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunDockingVerification.ps1
```

PropertyGrid/tool view 관련 변경:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 -WpfTools -FailOnWarn
```

Pipeline/sample/runner 관련 변경:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunVisionPlatformPrecheck.ps1
```

## 8. 작업 원칙

- 안정 계약 문서에 있는 동작은 먼저 보호합니다.
- UI/UX 변경은 실제 EXE 또는 focused screenshot smoke로 확인합니다.
- 관련 없는 dirty file은 되돌리지 않습니다.
- 새 abstraction은 반복을 줄이거나 책임 경계를 명확히 할 때만 추가합니다.
- 문서와 검증이 없는 완료 주장은 하지 않습니다.
