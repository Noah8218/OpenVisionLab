# OpenVisionLab 2D Partial 구조화 계획

Updated: 2026-09-11 KST

Status: **COMPLETE — D5 protected-boundary closure verified**

## 사용자 목표

프로젝트 전체의 `partial` 선언을 전수 조사하고, 주니어 개발자가 실제 책임과
호출 경로를 따라갈 수 있도록 구조화한다. 동작하는 기능, XAML/generated 계약,
공개 API, 직렬화, SDK 계약, 기존 수명·Dispose 순서는 보존한다.

## 현재 기준

| 항목 | 값 |
| --- | --- |
| Repository | `C:\Git\2D\Dev` |
| Branch | `codex/public-sample-ux-docs` |
| Commit at plan start | `0a77e60e` |
| Target Framework | `net8.0-windows7.0` |
| Static audit | `REFACTOR_AUDIT=PASS` |
| Current inventory | C# 815, XAML 60, partial declarations 59, literal text matches 2, project cycles 0 |
| Worktree | Existing user/Dev changes are dirty and must be preserved |

현재 `Invoke-RefactorAudit.ps1 -Verify`는 `ShellStorageCalls=0`, project cycle
0, partial declaration 80을 확인했다. 계획 시작 시점의 110개 raw token에서
수동 partial sprawl을 실제 소유자에 통합한 결과이며, 남은 partial을 모두
삭제해야 한다는 의미는 아니다.

## 구조화 원칙

모든 partial을 다음 세 가지로 분류한다.

1. **Generated/framework 계약**: XAML, Settings, designer 또는 프레임워크가
   요구하는 partial. 삭제하거나 일반 클래스에 합치지 않는다.
2. **동일 객체 수명 조합**: OpenGL drawing, docking workspace, View의 event/test
   hook처럼 같은 private state와 동일한 생성·해제 수명을 공유하는 cohesive
   partial. 파일 수를 줄이기 위해 기계적으로 합치지 않는다.
3. **독립 책임 후보**: 별도 state/dependency/lifetime을 소유할 수 있고 호출자가
   snapshot/result 또는 명시적 계약으로 연결할 수 있는 partial. 이 경우에만
   기존 concrete owner 또는 새 concrete owner로 최소 이동한다.

다음 변경은 금지한다.

- 파일 길이만을 이유로 partial을 새로 만들거나 삭제
- partial을 다른 partial로 옮기는 이름 변경만의 구조화
- one-implementation interface, forwarding wrapper, generic manager/factory
  추가
- XAML binding, public/test facade, serialization, SDK contract 변경
- `C:\Git\2D\Original` 수정, commit, push, tag, release, deployment
- 사용자 입력, 카메라, 하드웨어 또는 실제 UI 조작을 무기한 대기

## 단계별 작업

| 단계 | 목적 | 완료 조건 |
| --- | --- | --- |
| P1 | 전체 partial inventory | 파일, 타입, project, namespace, 줄 수, 분류, caller, mutable-state writer, lifetime/dispose, binding/public contract를 CSV와 요약 문서로 기록 |
| P2 | 독립 책임 후보 검증 | 후보마다 current owner, intended owner, call path, dependency direction, state owner, observable contract, focused proof를 기록. 근거가 없는 후보는 no-change로 남김 |
| P3 | 첫 concrete 경계 이동 | 가장 작은 독립 책임 하나를 기존 owner로 이동하고 old coupling 제거, Debug/Release build와 관련 contract를 통과 |
| P4 | 남은 후보 반복 | 한 heartbeat slice마다 하나의 책임만 처리. 각 slice 후 stale path/name, event subscription, Dispose order, XAML/public contract를 확인 |
| P5 | 최종 구조 검수 | partial 분류 결과, 새 owner/call path, 잔여 partial의 이유, build/test/audit, unverified runtime 범위를 문서화 |

## 검증 계약

각 slice는 다음을 수행한다.

- `Invoke-RefactorAudit.ps1 -Verify`
- 변경 project의 Debug/Release build
- 관련 contract/smoke 또는 호출 경로 검색
- old owner/import/path/direct call 부재 확인
- `git diff --check`
- D: 증거 디렉터리에 source proof와 결과 기록

실제 WPF theme/layout/DPI/monitor/input, camera/SDK, 장시간 native, 강제 종료
검증은 별도 환경 경계이며 소스 검토만으로 PASS라고 기록하지 않는다.

## 읽기 및 소유권 기준

전체 경로는 다음 순서로 유지한다.

```text
Program.Main
 -> OpenVisionLabApplication.Run
 -> OpenVisionShellHostWindow
 -> OpenVisionShellHostView
 -> concrete controller/presenter/document/workspace owner
 -> ViewModel/domain/service
```

구조 변경은 반드시 현재 owner가 더 이상 이동한 책임을 직접 소유하지 않는지,
새 owner가 실제 호출되는지, mutable state와 Dispose 책임이 어디에 남는지로
완료를 판정한다.

## P1 inventory 결과

현재 source와 tools에서 `bin`·`obj`를 제외하고 실제 선언 108개를 확인했다.
정적 감사의 raw token 110개 중 2개는 smoke contract의 문자열 검사였으며
실제 partial 선언으로 세지 않았다.

| 분류 | 개수 | 판단 |
| --- | ---: | --- |
| Generated/framework 또는 XAML contract | 58 | 유지 |
| 동일 state/lifetime을 공유하는 cohesive composition | 50 | 독립 경계가 증명되기 전 유지 |
| 문자열 false-positive | 2 | 코드 선언 아님 |

주요 cohesive family는 `OpenVisionDockWorkspaceController`(9),
`OpenGlDrawing`(9), `OpenVisionShellHostRecipeCommandSurface`(10),
`OpenVisionShellHostDockedLayerOrchestrator`(5),
`OpenVisionLayerDockWorkspaceView`(4), `ImageCanvasControl`(3),
`RoiImageCanvasViewModel`(3), `OpenVisionShellHostView`(3)이다.
각 family의 current owner, call path, mutable-state writer,
lifetime/release owner, contract와 보존/추출 판단은 다음 D: 증거에 기록했다.

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-p1-20260911\partial-inventory.csv`
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-p1-20260911\partial-groups.csv`
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-p1-20260911\partial-owner-map.csv`
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-p1-20260911\summary.txt`

P1 inventory만으로는 독립 책임 경계가 입증되지 않았다. 따라서 다음 단계는
파일 수를 줄이는 작업이 아니라, 위 cohesive family 중 실제로 snapshot/result와
독립 lifetime을 가질 수 있는 후보가 있는지 P2 proof를 수행하는 것이다.

## P2 owner proof 결과

고위험 cohesive family를 실제 호출자, 공유 mutable state, dependency 방향,
생성·해제 수명, 공개/XAML/test contract 기준으로 재확인했다. 상세 증거는
`docs/reports/OPENVISIONLAB_PARTIAL_STRUCTURE_PROOF_20260911.md`와 다음 D:
evidence에 기록했다.

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-p2-20260911\p2-owner-proof.md`

검토 결과 독립 책임 경계를 증명할 수 있는 partial은 0개였다.

- `OpenVisionShellHostRecipeCommandSurface`: 기존 R17 증거와 동일하게 command
  binding, recipe selection, execution session, callback, shared state가 한 Shell
  수명을 공유한다.
- `OpenVisionDockWorkspaceController`: 모든 partial이 같은 AvalonDock
  `DockingManager`와 `primaryPane`를 직접 변경한다.
- `OpenVisionPipelineReviewDocument`: 이벤트 partial이 document의 pipeline,
  view, execution/image owner, revision gate, validation state를 공유하고 root가
  구독 해제를 소유한다.
- `OpenVisionShellHostDockedLayerOrchestrator`: 모든 partial이 하나의 composition을
  전달하며 독립 mutable state가 없다.
- `OpenVisionShellHostView`와 `TestHooks`: XAML/UI/test contract와 여러 private
  owner를 공유하므로 별도 forwarding API 없이 이동할 수 없다.
- `RoiImageCanvasViewModel`, `ImageCanvasControl`, docking gesture/view,
  `OpenGlDrawing`, 두 Tool shell은 각각 native/UI/rendering 수명과 현재 concrete
  owner가 일치한다.

따라서 P3의 concrete extraction은 현재 근거로는 수행하지 않는다. partial 수를
줄이기 위한 파일 병합, 이름 변경, forwarding wrapper, one-implementation
interface는 구조 개선으로 인정하지 않는다. 새 요구사항·재현 결함·실패한 완료
조건·dependency/lifetime 경계 변경이 생길 때만 해당 family를 다시 연다.

## 사용자 요구 재개 이후 P3/P4 실제 적용 결과

사용자가 “기존 partial을 전수조사하고 구조화”하도록 범위를 명시적으로 다시
열었으므로, P2의 no-change 결론을 기계적으로 유지하지 않고 각 family를 다시
소스 기준으로 확인했다. 독립 서비스나 forwarding wrapper를 추가하지 않고, 같은
객체의 수명과 상태를 유지하면서 탐색 비용을 줄일 수 있는 수동 partial만 본체로
통합했다.

| 기존 family | 실제 변경 | 현재 owner와 호출 경로 | 유지한 계약 |
| --- | --- | --- | --- |
| `OpenVisionShellHostDockedLayerOrchestrator` (5 files) | Commands/Events/Guide/State를 `OpenVisionShellHostDockedLayerOrchestrator.cs`에 통합하고 Fields/Commands/Gesture/State/Notifications 영역으로 정리 | `OpenVisionShellHostView -> OpenVisionShellHostDockedLayerOrchestrator -> ShellDockedLayerWorkspaceComposition` | 이벤트 순서, composition ownership, Dispose 순서 |
| `OpenVisionDockWorkspaceController` (9 files) | Documents/Layout/Move/Native/Normalize/Cleanup/State를 concrete root에 통합 | `OpenVisionLayerDockWorkspaceView -> OpenVisionDockWorkspaceController` | AvalonDock tree, document ownership, layout restore/cleanup |
| `OpenVisionLayerDockingGestureController` (2 files) | Source resolver를 root에 통합 | Docking view/controller가 같은 manager와 callback을 사용 | drag source와 event lifetime |
| `RoiImageCanvasViewModel` (3 files) | Commands/Refresh를 root에 통합 | `RoiImageCanvasView -> RoiImageCanvasViewModel` | Mat/native ownership, command names, Dispose |
| `OpenVisionPipelineReviewDocument` / `View` event parts | event partial을 기존 XAML/document root에 통합 | `ShowPipelineReview -> document/view` | XAML binding, revision/event contract |
| `OpenVisionShellHostView` interactions | UI interaction partial을 XAML code-behind root에 통합; TestHooks는 별도 test contract로 유지 | `OpenVisionShellHostWindow -> OpenVisionShellHostView -> concrete owners` | XAML generated contract와 test hooks |
| Single/Double Input Tool Shell | nested layout controller partial을 XAML root의 private concrete owner로 통합 | 각 Tool Shell이 직접 layout controller를 생성·해제 | PropertyGrid control lifetime |
| `OpenVisionShellHostRecipeCommandSurface` | flat Wpf root에서 `Recipe/CommandSurface`로 물리적 책임 폴더 이동, 파일명을 `Commands.cs`, `Handlers.cs`, `PipelineExchange.cs` 등으로 정리 | Shell View/Recipe owner가 같은 command surface를 생성하고 XAML binding이 사용 | type/namespace/binding/serialization contract |

이 변경으로 수동 source partial 선언은 110 raw token에서 80개로 줄었다. 남은
`OpenGlDrawing` 9개는 OpenGL context와 static drawing assumptions를 공유하는
하나의 rendering API이고, Recipe CommandSurface 9개는 binding-facing state,
callbacks, execution session을 한 Shell 수명에서 공유한다. 이 두 family를
또 다른 wrapper/interface로 분리하면 상태 공유가 숨겨지고 호출 경로가 더
길어지므로 이번 범위에서는 현재 책임 폴더와 명시적 영역을 유지한다.

`ImageCanvasControl.designer.cs`, XAML code-behind, Settings designer,
`OpenVisionShellHostView.TestHooks.cs`, smoke contract partial은 생성 코드·UI
계약·테스트 계약으로 남겼다. 이들은 generated/public/test boundary를 깨지 않고
별도 owner로 옮길 수 있다는 증거가 없으므로 삭제하지 않았다.

최신 선언 분류는 XAML contract 56, generated/framework 2, native control
composition 1, rendering composition 9, Recipe command surface 9, test contract
1, smoke contract 2이다. 별도 `Manual review` 잔여 항목은 없다.

현재 소유권 증거와 최신 inventory는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-p3-20260911`
에 있다. 주요 파일은 `partial-inventory-after.csv`,
`partial-owner-map-after.csv`, `partial-groups-after.txt`,
`owner-proof-after.md`, smoke/build/audit 로그다.

## 상태

- P1: VERIFIED — 전체 inventory, 분류, owner/call-path baseline을 D: evidence로 생성
- P2: VERIFIED — 고위험 family owner proof 및 no-change 근거를 기록
- P3: VERIFIED — concrete root 통합과 old partial path 제거, 관련 smoke/build 통과
- P4: VERIFIED — Recipe CommandSurface 물리 폴더 정리와 남은 protected family 재확인
- P5: VERIFIED — 최신 inventory/owner proof, Debug/Release, Readiness, focused
  smoke, documentation index, refactor audit, stale-path search, and diff check
  evidence are recorded

이 문서는 모든 partial을 기계적으로 제거하겠다는 약속이 아니라, 실제 구조적
경계가 입증된 partial만 concrete owner로 정리하겠다는 실행 계약이다.

## D2 결과 — Shell TestSurface 추출 (2026-09-11)

설계된 첫 구현 slice를 적용했다. `OpenVisionShellHostView.TestHooks.cs` 수동
partial은 제거했고, 기존 검사 동작은
`Shell/Support/OpenVisionShellHostViewTestSurface.cs`의 concrete owner로
이동했다. Surface는 `OpenVisionShellHostViewTestSurfaceBindings`를 통해
기존 Shell/Layer/Tool/Recipe owner를 참조하며 상태를 복제하지 않는다.

Shell View는 surface 생성과 UI/lifetime 연결을 소유하고, 기존 `*ForTest`
public/internal 계약은 root의 얇은 forwarding compatibility contract로
보존했다. 따라서 Smoke 호출 경로는 이름을 바꾸지 않고
`OpenVisionShellHostView -> testSurface -> 기존 concrete owner`로 추적할 수
있다.

D2 evidence는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d2-20260911`
에 있다. Debug solution build, ReadinessCheck, runtime stability contract와
declaration/text 분리 audit가 PASS했다. 현재 compiled partial declaration은
77개이고 literal smoke text match는 2개로 별도 보고된다. Release build와
같은 bounded source 변경의 Debug/Release solution build는 PASS했다. 실제 GUI
runtime은 이 slice에서 실행하지 않았으므로 unverified로 남긴다.

다음 단계는 D3 `OpenGlDrawing`의 concrete renderer 경계 검증이다. D4 Recipe
Command Surface와 D5 protected set 최종 검수 전까지 PL-0013은 완료로 닫지 않는다.

## D3 결과 — OpenGL renderer concrete owner (2026-09-11)

설계된 9개 OpenGL partial family를 concrete owner로 이동했다. 기존
`OpenGlDrawing` 공개 호출면은 `OpenGlDrawing.cs` compatibility façade로
유지하고, 83개 public static method 시그니처·`ZoomFactor` field·
`FontGlyphCount` reflection contract를 보존했다.

구현 파일은 `OpenGlColorConverter`, `OpenGlTextureRenderer`,
`OpenGlShapeRenderer`, `OpenGlPenRenderer`, `OpenGlTextRenderer`,
`OpenGlMeasurementRenderer`, `OpenGlOverlayRenderer`,
`OpenGlOverlayTextRenderer`, `OpenGlDrawingState`다. Renderer는 OpenGL
context를 소유하지 않는다. `ImageCanvasControl`/`OpenGlOverlayExtensions`의
기존 texture와 display-list release owner도 유지했다. partial 간 private helper
였던 `ConvertDotInfoToPoints`는 실제 호출 책임자인 Pen renderer로 옮겼다.

D3 evidence는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d3-20260911`
와 설계 inventory
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d3-design-20260911`
에 있다. ImageCanvas 및 solution Debug/Release build, Smoke build/runtime
stability, ReadinessCheck, public signature comparison, refactor audit가
통과했다. 현재 compiled partial은 68개, literal smoke match는 2개다.
실제 GPU/desktop visual rendering은 실행하지 않아 unverified로 남긴다.

다음 단계는 D4 Recipe CommandSurface shared-state 경계를 concrete façade와
기존 workflow owner에 적용하는 것이다.

## 사용자 재개 요청에 따른 완료 설계 재개 — 2026-09-11

이전 P5 기록은 당시 증거로 `OpenGlDrawing`, Recipe Command Surface,
`OpenVisionShellHostView.TestHooks`를 protected family로 닫았다. 사용자가
“기존에 만들어진 partial을 전부 전수조사해서 구조화”를 다시 명시했으므로,
이 세 family를 이번에 다시 열어 **설계를 먼저 고정한 뒤 구현**한다. 이전
P1~P5 evidence와 실제로 적용된 same-owner consolidation은 역사적 증거로
보존하며, 남은 manual family를 닫은 근거로 재사용하지 않는다.

현재 source-only 재확인 결과는 실제 compiled declaration 78개와 smoke
contract 문자열 match 2개다. 문자열 2개는 선언이 아니므로 다음 audit부터
`PartialDeclarations`와 `PartialTextMatches`를 분리한다. 기대하는 최종
수치는 XAML 56개, `ImageCanvasControl` designer 조합 2개, `Settings` 1개인
보호 partial 59개와 manual partial 0개다.

구체적 완료 설계와 단계별 owner/call-path/검증 기준은
`docs/reports/OPENVISIONLAB_PARTIAL_STRUCTURE_COMPLETION_DESIGN_20260911.md`
에 기록했다. 현재 단계의 다음 작업은 D0 계약 inventory 교정과 D1 smoke
false-positive 분리이며, 설계 slice에서는 source implementation을 시작하지
않는다.

## D0/D1 진행 결과 — 2026-09-11

D0/D1은 설계 다음의 첫 번째 bounded slice로 완료했다. 변경한 production
behavior는 없고, `tools/RefactorAudit/Invoke-RefactorAudit.ps1`의 partial
검색만 실제 declaration line에 고정했다. 감사 결과는 이제
`PartialDeclarations=78;PartialTextMatches=2`로 분리된다. 두 text match는
smoke contract가 source code-behind 계약을 확인하기 위해 보유한 문자열이며
compiled partial이 아니다.

정정된 D: evidence는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d1-20260911`
에 있다. `partial-inventory-declarations.csv`와
`partial-owner-map-declarations.csv`는 smoke 문자열 2개를 제외한 78개
declaration을 담고, `partial-text-matches.csv`는 문자열 2개의 위치를
별도로 기록한다.

`Invoke-RefactorAudit.ps1 -Verify`는 `CSharpFiles=822`, `XamlFiles=60`,
`PartialDeclarations=78`, `PartialTextMatches=2`, `ProjectCycles=0`,
`ShellStorageCalls=0`으로 PASS했다. 다음 bounded slice는 D2
`OpenVisionShellHostView.TestHooks` concrete TestSurface 추출이다.

## D4 결과 — Recipe CommandSurface concrete facade (2026-09-11)

D4 설계 문서에 기록한 owner/call-path/state/lifetime 기준을 적용했다.
`src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface`의 기존 9개 수동 partial
선언을 `RecipeCommandSurface.cs` 하나의 concrete facade로 통합하고, 기존
본문을 responsibility region으로 보존했다. `Workspace`, `Pipeline lifecycle`,
`Pipeline exchange`, validation, qualification, step edit, run history, LLM/review
동작은 기존 concrete owner가 계속 수행하며 facade는 binding/command/notification
및 callback projection을 유지한다. 새 interface/factory/manager/wrapper는
추가하지 않았다.

Readiness 검사 도구는 삭제된 `Handlers.cs`/`ValidationSets.cs`를 직접 읽지 않고
현재 facade source family를 읽도록 최소 수정했다. D4 통합 전 원본은
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d4-design-20260911`,
구현과 검증은 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d4-20260911`
에 보관했다. source-body SHA-256 비교에서 8개 기존 partial body와 root core가
모두 일치했다.

검증 결과는 Solution 및 VisionRecipeRunnerSmoke Debug/Release build 0 warning/0
error, ReadinessCheck Debug/Release PASS, Debug Recipe focused contract 24/24
PASS(D: 복사 runtime execution session 포함), Release 23/23 PASS, RefactorAudit
`CSharpFiles=815;XamlFiles=60;PartialDeclarations=59;PartialTextMatches=2;
ProjectCycles=0;ShellStorageCalls=0`, DocumentationIndex PASS, `git diff --check`
PASS다. 실제 WPF/GPU/monitor/DPI/camera/SDK 장시간 검증은 실행하지 않았다.

D5는 보호 partial 59개의 file/reason/call path/state/lifetime/contract inventory를
최종 고정하고 PL-0013을 closure하는 단계다.

## D5 최종 결과 — 보호 계약 closure (2026-09-11)

D5 설계대로 남은 compiled partial 59개를 전수 inventory했다. 56개는 WPF/XAML
code-behind와 외부 smoke XAML contract, 2개는 `ImageCanvasControl` native/
designer 조합, 1개는 `Settings.Designer.cs` generated contract다. manual
partial declaration은 0개이며, smoke source 문자열 2개는 literal text match로
분리된다. 모든 row의 owner/caller/mutable-state/lifetime/contract가
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d5-20260911`
아래 CSV와 summary에 기록됐다.

Recipe/OpenGL/TestHooks의 수동 partial 및 이전 파일 경로는 `src`/`tools`에서
검색되지 않았다. RefactorAudit는
`CSharpFiles=815;XamlFiles=60;PartialDeclarations=59;PartialTextMatches=2;
ProjectCycles=0;ShellStorageCalls=0`으로 PASS했다. D4에서 통과한 Solution/
Smoke Debug·Release build, ReadinessCheck, Recipe focused contracts와 D5의
inventory, stale-path, DocumentationIndex, diff check를 합쳐 PL-0013 closure
조건을 만족한다. 실제 WPF/GPU/monitor/DPI/camera/SDK/장시간 native 검증은
환경상 미실행으로 기록한다.

PL-0013의 다음 구현 단계는 없다. 새로운 defect, failed criterion, changed
lifetime/dependency boundary가 생길 때만 별도 issue로 연다.
