# OpenVisionLab P1 View 책임 경계 리팩토링

## 작업 기준

- 작업일: 2026-09-13 KST
- 저장소: `C:\Git\2D\Dev`
- 브랜치: `codex/public-sample-ux-docs`
- 기준 커밋: `176eec95 refactor: complete junior discoverability and reliability cleanup`
- 실행 프로젝트: `src/OpenVisionLab/OpenVisionLab.csproj`
- 대상 프레임워크: `.NET 8 / WPF / net8.0-windows7.0`
- 작업 ledger: `.proofline/issues/PL-0027.json`
- 자동화: `openvisionlab-2d-1-5`, 5분 heartbeat, 한 실행당 한 milestone

이 기록은 현재 사용자의 P1 후보 세 곳과 기존 Partial 59개 재검증을 한
작업 단위로 추적합니다. 이미 완료된 Shell/Pipeline Review/RecipeCommandSurface
owner는 파일 길이만으로 다시 분리하지 않습니다. 새 책임 경계가 실제 호출·상태·수명
증거로 확인될 때만 다음 단계에서 수정합니다.

## 사용자 문제와 흐름

처음 솔루션을 여는 개발자가 `Image → Layer → Tool → Inspection → Pipeline →
Recipe → Result → Review`를 따라갈 때 View가 파일 I/O, native 이미지, Recipe 저장
정책까지 직접 소유하면 실제 상태 owner와 종료 책임을 찾기 어렵습니다. 이번 범위는
화면 계약과 기존 Recipe/SDK 동작을 유지하면서 다음 세 흐름의 변경 이유를 분리하는
것입니다.

```text
Recipe run evidence 선택
  -> Evidence View 상태/표시
  -> 기존 Bitmap preview owner의 decode
  -> Layer viewer clone/Dispose

Line Tool property/preset/sample 변경
  -> Line View 입력 adapter
  -> LineToolPresenter persistence boundary
  -> 기존 Recipe property session store
```

RoiImageCanvasViewModel과 Partial 59개는 먼저 현재 caller·mutable state·native
lifetime을 대조한 뒤 독립 owner가 증명되는 경우에만 이동합니다.

## 범위와 완료 기준

### 이번에 구현

1. Recipe evidence View의 file-backed Bitmap decode를 기존 concrete image preview
   factory로 이동하고, View의 상태·viewer lifecycle을 보존합니다.
2. Line Tool View의 직접 native property 저장 호출을 기존 조합부에서 주입하는
   Presenter 경계로 이동하고 Line A/B key와 실패 계약을 보존합니다.
3. RoiImageCanvasViewModel의 추가 독립 경계 여부를 source/caller/lifetime 증거로
   판정합니다.
4. 현재 Partial 선언을 전수 재분류해 generated/framework/cohesive 경계를
   기계적으로 삭제하거나 다시 쪼개지 않았음을 증명합니다.

### 제외

- XAML 이름·binding/public Window 계약 변경
- 새 `I...Service`, Manager, Provider, forwarding wrapper 또는 편의용 partial
- 카메라/SDK/GPU, 실제 WPF theme/DPI/monitor/input, 장시간 native runtime 자격
- `C:\Git\2D\Original` 수정, commit, push, tag, release, deploy

## Milestone 상태

| 단계 | 상태 | 완료 근거 |
| --- | --- | --- |
| M1 Recipe evidence 이미지 경계 | **VERIFIED** | 기존 `OpenVisionBitmapImagePreviewFactory.LoadBitmap(path, role)` 사용, View direct decode 제거, 4/4 source/runtime contract, Debug build 0 warning/error |
| M2 Line Tool 저장 경계 | **VERIFIED** | `LineToolPresenter.PersistProperties`에 기존 store Action 주입, View direct store 제거, 3/3 source contract, Debug build 0 warning/error |
| M3 RoiImageCanvasViewModel 경계 | **VERIFIED (no-change proof)** | 입력·dialog·context·path·load/save owner와 5개 소비자 Dispose 경로를 확인; 추가 독립 state/lifetime seam 없음 |
| M4 Partial 59 구조 재검증 | **VERIFIED (one concrete owner slice + row-level review)** | 59개 선언 행 단위 owner/call-path/lifetime 판정, ROI/template 파일 디코드 책임을 기존 이미지 factory로 이동, no-cosmetic-change 및 retained-boundary 근거 |
| M5 최종 owner map·audit·first-contributor review | **VERIFIED** | owner map/Start Here/partial responsibility report 갱신, RefactorAudit·DocumentationIndex·diff check, Solution/Smoke Debug·Release build와 M1~M4 계약 재실행 |

## M1 — Recipe run evidence 이미지 경계

### Owner map

| 항목 | 현재/의도 owner |
| --- | --- |
| Evidence 선택·status·XAML event | `OpenVisionRecipeRunEvidenceViewerView.xaml.cs` |
| file-backed Bitmap decode | `OpenVisionBitmapImagePreviewFactory.LoadBitmap(path, role)` |
| 표시 이미지 clone·이전 이미지 Dispose | `OpenVisionLayerViewerView.xaml.cs` |
| View 생성·owner Window·floating lifecycle | `OpenVisionRecipeRunEvidenceViewerController.cs` |

호출 경로:

```text
OpenVisionShellHostView
  -> OpenVisionRecipeRunEvidenceViewerController.Open
  -> OpenVisionRecipeRunEvidenceViewerView.TrySetEvidence/TrySetDrawing
  -> OpenVisionBitmapImagePreviewFactory.LoadBitmap
  -> OpenVisionLayerViewerView.SetLayer
```

변경 파일:

- `src/OpenVisionLab/UI/Menu/Wpf/Viewer/OpenVisionBitmapImagePreviewFactory.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeRunEvidenceViewerView.xaml.cs`
- `tools/VisionRecipeRunnerSmoke/RecipeRunEvidenceImageBoundaryContract.cs`
- `tools/VisionRecipeRunnerSmoke/Program.cs`

기존 role-specific error text, read-only evidence behavior, selection/status binding,
local Bitmap disposal 순서를 유지했습니다. 새 interface나 image loader를 만들지
않았습니다.

검증:

- `dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj --configuration Debug --no-restore` — 0 warning / 0 error
- `dotnet run --project tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj --configuration Debug --no-build -- --recipe-run-evidence-image-boundary-contract <D:\...\m1-recipe-evidence>` — `PASS|checks=4`

## M2 — Line Tool 저장 경계

### Owner map

| 항목 | 현재/의도 owner |
| --- | --- |
| PropertyGrid/preset/sample UI 입력 | `LineToolWpfView.xaml.cs` |
| ViewModel facade와 저장 호출 seam | `LineToolPresenter.PersistProperties()` |
| Line A/B property 조회·저장 Action 조합 | `OpenVisionNativeCustomToolFactory.CreateLine` |
| Recipe별 key·파일·실패 event·cache | `OpenVisionNativeToolPropertySessionStore` |

호출 경로:

```text
OpenVisionNativeCustomToolFactory.CreateLine
  -> LineToolPresenter(viewModel, persistProperties)
  -> LineToolWpfView property/preset/sample change
  -> LineToolPresenter.PersistProperties
  -> OpenVisionNativeToolPropertySessionStore.Save("Line(L)_1" / "Line(R)_1")
```

변경 파일:

- `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Documents/OpenVisionNativeCustomToolFactory.cs`
- `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/Presentation/LineToolPresenter.cs`
- `src/OpenVisionLab/UI/VisionTest/Wpf/ToolViews/LineToolWpfView.xaml.cs`
- `tools/VisionRecipeRunnerSmoke/LineToolPersistenceBoundaryContract.cs`
- `tools/VisionRecipeRunnerSmoke/Program.cs`

View는 native store를 직접 참조하지 않고 Presenter 호출만 합니다. 기존 PropertyGrid
변경 callback, preset, sample pair, ROI 적용 경로와 Line A/B key를 그대로 사용합니다.

검증:

- 동일한 `VisionRecipeRunnerSmoke` Debug build — 0 warning / 0 error
- `dotnet run --project tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj --configuration Debug --no-build -- --line-tool-persistence-boundary-contract <D:\...\m2-line-persistence>` — `PASS|checks=3`

## M3 — RoiImageCanvasViewModel 경계 결정

`RoiImageCanvasViewModel`은 ImageCanvas control, WPF keyboard/mouse/context adapter,
dialog host, OpenCvSharp Mat과 path/save 흐름을 한 facade에서 연결하지만, 각 정책은
이미 concrete owner로 나뉘어 있습니다. 소스 기준으로 다음 경계를 확인했습니다.

1. `RoiImageCanvasView`가 DataContext attach/detach와 dialog/context host를 연결합니다.
2. ViewModel은 ROI/mode/snapshot 상태, current Mat clone, image name/size,
   ImageCanvasControl과 reshape timer의 단일 lifetime owner입니다.
3. WinForms/WPF keyboard와 mouse 정책은 세 controller가 구독·해제하고 explicit
   callback으로 ViewModel 상태를 변경합니다.
4. `ImageCanvasDirectoryPolicy`, `CanvasImageLoader`, `CanvasImageSaver`, dialog
   host가 경로·파일·modal 정책을 소유합니다.
5. External session, OpenGL preview adapter, Bitmap canvas presenter, template
   editor가 각각 ViewModel.Dispose를 호출합니다.

따라서 현재 source에는 Mat/control/timer를 snapshot으로 넘겨 독립적으로 소유할
실제 state/lifetime/test seam이 없습니다. 추가 wrapper·interface·partial을 만들면
Dispose와 caller compatibility가 분산되므로 **production no-change**를 결정했습니다.
이 결정은 파일 길이가 아니라 caller와 native lifetime proof에 근거합니다.

검증:

- `RoiImageCanvasBoundaryContract` — `PASS|checks=6`
- `VisionRecipeRunnerSmoke` Debug build — 0 warning / 0 error
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\p1-view-boundaries-20260913\m3-imagecanvas`

## M4 — Partial 59 구조 재검증 및 실제 책임 이동

기존 `OPENVISIONLAB_PARTIAL_STRUCTURE_PLAN_20260911.md`와 proof를 출발점으로
삼되, 보호 목록을 그대로 승인하지 않고 최신 source의 59개 선언을 다시 열어
caller, mutable-state writer, binding/public/serialization contract,
Dispose/lifetime을 행 단위로 대조했습니다. 결과는
[`OPENVISIONLAB_PARTIAL_RESPONSIBILITY_REVIEW_20260913.md`](OPENVISIONLAB_PARTIAL_RESPONSIBILITY_REVIEW_20260913.md)와
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\p1-view-boundaries-20260913\m4-partial-audit\partial-structure-review.csv`에 기록했습니다.

실제 책임 충돌이 확인된 ROI/template editor 두 곳의 file-backed pattern
decode는 기존 `OpenVisionBitmapImagePreviewFactory.LoadBitmap`으로 이동했습니다.
XAML partial 자체는 유지하고 Window가 ROI interaction, preview assignment,
source Bitmap Dispose를 계속 소유합니다. 생성/디자이너 partial, Shell
composition root, Pipeline Review control projection, Learn presenter boundary,
ImageCanvas native host boundary는 독립 state/lifetime seam이 없거나 기존
owner가 이미 있어 기계적으로 병합하지 않았습니다.

검증:

- `Invoke-RefactorAudit.ps1 -Verify` — `PASS`, 59 declarations, 2 literal text matches, 0 project cycles.
- `VisionRecipeRunnerSmoke` Debug build — 0 warnings / 0 errors.
- `--template-editor-image-boundary-contract` — `PASS|checks=3`.
- 실제 WPF visual/theme/DPI/monitor/input, GPU/camera/SDK, 장시간 native shutdown은 미검증.

## M5 — 최종 검증 기준

- `Invoke-RefactorAudit.ps1 -Verify`
- 문서 index 검사
- `git diff --check`
- 변경된 smoke contract와 Debug build
- owner map/Start Here/first-contributor reading order 갱신
- 소스 기준과 실제 WPF runtime 미검증 범위 분리

실행 결과:

- `Invoke-RefactorAudit.ps1 -Verify` — `PASS` (`CSharpFiles=826`, `XamlFiles=60`, `PartialDeclarations=59`, `PartialTextMatches=2`, `ProjectCycles=0`, `ShellStorageCalls=0`).
- `tools/TestDocumentationIndex.ps1` — `PASS` (`IndexedPaths=295`, `Routes=16`, `RootRedirects=102`).
- `git diff --check` — exit 0 (Git line-ending conversion warning만 출력).
- `dotnet build OpenVisionLab.sln --configuration Debug --no-restore` — 0 warning / 0 error.
- `dotnet build OpenVisionLab.sln --configuration Release --no-restore` — 0 warning / 0 error.
- `VisionRecipeRunnerSmoke` Debug/Release build — 0 warning / 0 error.
- M1~M4 focused contracts를 Debug/Release에서 모두 재실행했고 전부 PASS: evidence 4/4, Line persistence 3/3, RoiImageCanvas 6/6, ROI/template image 3/3.
- 최종 증거: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\p1-view-boundaries-20260913\m5-final\phase-summary.txt` 및 `m5-final-release`.

현재 상태:

```text
Status: Verified
Acceptance criteria: C1/M1 PASS, C2/M2 PASS, C3/M3 PASS (no-change proof), C4/M4 PASS, C5 PASS
Verification: M1~M4 focused contracts and VisionRecipeRunnerSmoke Debug/Release builds passed; Solution Debug/Release builds, RefactorAudit, DocumentationIndex, and diff check passed
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\p1-view-boundaries-20260913
Boundary: WPF visual/theme/DPI/monitor/input, camera/SDK/GPU, and long-running native behavior remain unverified
```
