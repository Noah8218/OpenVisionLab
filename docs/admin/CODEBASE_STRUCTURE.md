# OpenVisionLab Codebase Structure

Updated: 2026-09-15 KST

이 문서는 현재 owner와 가장 짧은 코드 읽기 순서를 기록합니다. PL-0056까지의 상세
owner chronology는
[`CODEBASE_STRUCTURE_HISTORY_20260914.md`](archive/CODEBASE_STRUCTURE_HISTORY_20260914.md)에
보존했습니다. 파일 길이 또는 과거 계획만으로 완료 owner를 다시 나누지 않습니다.

## Start Here: solution and projects

| 역할 | 프로젝트 | 출력/진입점 |
| --- | --- | --- |
| 제품 실행 앱 | `src/OpenVisionLab/OpenVisionLab.csproj` | `Program.Main`; Windows x64 WPF `WinExe` |
| UI/표시 라이브러리 | `OpenVisionLab.Display.Core`, `ImageCanvas`, `Docking.Controls`, `Logging.Controls`, `Pipeline.Controls`, `WpfPropertyGridBridge` | 앱이 참조하는 View/adapter/control |
| 순수·공유 라이브러리 | `ImageSpace.Core`, `Localization`, `Logging`, `History`, `Mvvm`, `PropertyGrid.Abstractions` | UI와 독립 가능한 상태/계약/정책 |
| 화면 없는 검증 | `OpenVisionReadinessCheck` | source/contract readiness |
| 외부 consumer 검증 | `ImageCanvasExternalConsumerSmoke` | ImageCanvas public dependency/lifetime 계약 |
| 보조 실행 도구 | `OpenVisionLab.ImageCompare` | 별도 Image Compare 실행 도구 |

`OpenVisionLab.sln`에는 위 16개 프로젝트가 있고 프로젝트 참조 cycle은 없어야 합니다.
앱이 모든 12개 제품 라이브러리를 조합합니다. x86 구성은 지원하지 않습니다.

## Product startup call path

```text
Program.Main
  -> AppPathService.Initialize / OpenVisionLanguageService.ConfigureDataDirectory
  -> OpenVisionLabApplication.Run
  -> ApplicationRuntimeContext.CreateDefault
  -> Global.RestoreLastRecipe / ApplyLogConfig
  -> OpenVisionShellHostWindow
  -> OpenVisionShellHostView composition
```

- `Program`은 경로 초기화와 embedded smoke dispatch만 소유합니다.
- `OpenVisionLabApplication`은 single-instance, WPF application/window, runtime context,
  startup/shutdown lifetime을 소유합니다.
- `ApplicationRuntimeContext`가 process-scoped dependencies를 만들고 bootstrap이
  `DisplayManager`를 최종 해제합니다.

## Main workflow owners

| 책임 | 현재 owner | 읽기 시작점 |
| --- | --- | --- |
| Shell 화면 조합/가시성 | `OpenVisionShellHostView`와 책임별 Shell controller | `UI/Menu/Wpf/OpenVisionShellHostView.xaml.cs` |
| Shell binding 상태 | Shell ViewModel/state presenter | `UI/Menu/Wpf/Shell/State` |
| Recipe 편집·명령 상태 | `RecipeCommandSurface`와 기존 session/controller owners | `UI/Menu/Wpf/Recipe/CommandSurface/RecipeCommandSurface.cs` |
| Recipe/Pipeline 저장 | Recipe/Pipeline lifecycle·storage owner | `Core/Recipe`, `Core/Pipeline`, Shell Recipe controller |
| Pipeline Review 조합 | `OpenVisionPipelineReviewDocument` | `UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs` |
| Pipeline 실행/취소 | Pipeline Review execution controller → `VisionPipelineExecutionService` | `Core/Pipeline/Execution` |
| Pipeline 출력 할당 preflight | `VisionPipelineOutputAllocationGuard` | `Core/Pipeline/Validation/VisionPipelineOutputAllocationGuard.cs` |
| 결과·drawing 표시 | result/display owner와 Pipeline Review ViewModel/View | `UI/Menu/Wpf/Recipe/Review`, `OpenVisionLab.Display.Core` |
| Native Tool 편집 | Tool ViewModel + Property adapter + interaction/preview controller | `UI/VisionTest/Wpf/ToolViews`에서 해당 Tool 검색 |
| Learn | Learn Window composition + topic presenter | `UI/VisionTest/Wpf/Learn` |
| 이미지 lifetime | 생성한 owner가 명시적으로 Dispose; View는 WPF presentation resource 해제 | 해당 `*ImageResourceOwner`, `DisplayManager`, ImageCanvas owner |

## Recipe/Pipeline reading order

```text
OpenVisionShellHostView
  -> RecipeCommandSurface command
  -> Recipe execution/review session
  -> OpenVisionPipelineReviewDocument
  -> VisionPipelineExecutionService
  -> concrete VisionPipeline*Tool
  -> VisionPipelineRunResult / drawing evidence
  -> review ViewModel binding
```

`Teach`, `Preview`, `Run`은 서로 암묵적으로 호출하지 않는 명시적 계약입니다. 설정 복원,
화면 열기, 언어 전환은 Preview/Run, layer mutation, route mutation을 일으키면 안 됩니다.

## WPF/MVVM boundary

- View code-behind는 XAML namescope, control event adapter, focus/drag/popup/dialog host,
  framework lifetime과 렌더링 plumbing만 소유합니다.
- ViewModel은 binding state, command state, 사용자 의도와 결과 projection을 소유하며
  `Window`, `UserControl`, concrete control 또는 visual tree를 참조하지 않습니다.
- 파일·네트워크·device/native side effect는 기존 adapter/service/controller owner에 둡니다.
- 새 interface/service/helper는 실제 교체 seam, 독립 lifetime, reuse 또는 test seam이
  있을 때만 추가합니다.
- `partial`은 XAML/generated/framework 조합 또는 기록된 cohesive 경계에서만 유지합니다.

## ImageCanvas owner and reading order

```text
RoiImageCanvasView 또는 ImageCanvasExternalConsumerSession
  -> RoiImageCanvasPresentation
  -> ImageCanvasControl / input controllers / render events / refresh timer
  -> SharpGL + OpenCvSharp managed/native runtime
```

- 현재 owner: `RoiImageCanvasPresentation`이 concrete `ImageCanvasControl`을 생성하고
  입력 adapter, render subscription, refresh timer, texture/overlay surface와 native Dispose를
  소유합니다.
- View owner: `RoiImageCanvasView`가 presentation을 만들고 `WindowsFormsHost`에 control을
  배치하며, DataContext attach/detach와 presentation 해제를 소유합니다.
- 외부 consumer owner: `ImageCanvasExternalConsumerSession`이 자체 presentation/host 수명을
  소유하고 UI thread에서 해제합니다.
- mutable-state writer: `RoiImageCanvasViewModel`이 binding, command, ROI interaction, 현재
  이미지 `Mat` 상태를 소유합니다. 외부 session은 public overlay metadata를 소유합니다.
- 기존 contract: XAML binding 이름과 ViewModel의 image/ROI/view-state public operation은
  유지되며 concrete `ImageCanvasControl`은 ViewModel public surface에 노출되지 않습니다.
- 가장 짧은 판독 순서: `RoiImageCanvasView.xaml.cs` →
  `Presentation/RoiImageCanvasPresentation.cs` → `RoiImageCanvasViewModel.cs` →
  `External/ImageCanvasExternalConsumerSession.cs`.
- focused proof: `RoiImageCanvasBoundaryContract`, `RoiImageCanvasPathPolicyContract`, 실제
  external-consumer 100회 create/dispose smoke와 제품 EXE Line smoke입니다.
- 미검증 범위: 125~200% DPI, 모든 theme/keyboard state, GPU/driver 조합과 장시간 반복은
  이번 source/runtime 범위에서 검증하지 않았습니다.

## Completed boundary lock

| 보호 범위 | canonical evidence |
| --- | --- |
| Shell/Recipe/Pipeline Review owner 분리 | `PL-0013`~`PL-0018`, `PL-0021`~`PL-0029` |
| 이미지 decode/resource lifetime 후속 | `PL-0030`~`PL-0035` |
| Tool/Learn/XAML Partial 검토 | `PL-0036`~`PL-0056` |
| ImageCanvas concrete control/lifetime owner 이동 | `PL-0057`, `docs/reports/OPENVISIONLAB_PREDEVELOPMENT_PRIORITIES_1_TO_5_20260915.md` |
| 전체 상세 owner/call-path/test 기록 | `docs/admin/archive/CODEBASE_STRUCTURE_HISTORY_20260914.md`와 각 issue/report |

위 owner는 새 요구사항, 재현 defect, 실패한 완료 criterion, 변경된 dependency/lifetime
경계가 없으면 이동·분할·rename·wrapper 추가 대상이 아닙니다.

## Verification owners

| 검증 | owner/명령 |
| --- | --- |
| solution build | `dotnet build OpenVisionLab.sln -c <Debug|Release> -p:Platform="Any CPU"` |
| source/MVVM/refactor audit | `tools/Invoke-RefactorAudit.ps1 -Verify` |
| readiness | `tools/OpenVisionReadinessCheck` |
| 문서 route/redirect | `tools/TestDocumentationIndex.ps1` |
| 외부 DLL hash/allowlist | `tools/TestExternalReferences.ps1` |
| NOTICE | `tools/TestThirdPartyNoticeCoverage.ps1` |
| sample catalog/assets | `tools/TestPublicSampleAssets.ps1` 및 public catalog runner |
| WPF UI | `tools/RunUiPrecheck.ps1` → `PipelineViewerScreenshotSmoke` |
| actual product EXE | opt-in embedded `OpenVisionLab.DirectSmokeRunner` |
| ImageCanvas external consumer | `tools/ImageCanvasExternalConsumerSmoke` |

검증 도구는 제품 정책을 소유하지 않습니다. 실패를 통과시키기 위해 assertion, fixture,
expected result를 완화하지 않습니다.

## Before changing structure

1. owning feature/module, current caller, matching test를 찾습니다.
2. mutable-state writer와 create/release owner를 기록합니다.
3. 기존 concrete owner 재사용, 최소 in-place 변경, 새 owner가 실제 대안인지 비교합니다.
4. 변경 후 intended call path가 실제 사용되는지와 old coupling 제거를 검색합니다.
5. focused build/test와 미검증 runtime 범위를 completion record에 남깁니다.

## Search shortcuts

```powershell
rg -n "class <Type>|<Method>\(" src tools
rg -n "DataContext|Command=|Binding " src -g "*.xaml" -g "*.cs"
rg -n "ProjectReference" src tools -g "*.csproj"
rg -n "Dispose\(|event |\+=|-=|Timer" src -g "*.cs"
node C:\Users\USER\.codex\skills\proofline-issue-ledger\scripts\issue-ledger.js show PL-0000 --root C:\Git\2D\Dev
```
