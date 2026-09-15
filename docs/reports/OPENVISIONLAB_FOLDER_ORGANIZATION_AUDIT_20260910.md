# OpenVisionLab 폴더·클래스 배치 감사 및 정리

Updated: 2026-09-10 KST

Status: Complete for the bounded Docking.Controls physical source
reorganization and its focused verification. The remaining `Common` folder
candidate was subsequently completed by R15 in
`OPENVISIONLAB_COMMON_FOLDER_REORGANIZATION_20260910.md`. Desktop UI/runtime
qualification remains outside this source/build scope.

## 기준선과 범위

- Repository: `C:\Git\2D\Dev`
- Remote: `origin = https://github.com/Noah8218/OpenVisionLab_Dev.git`
- Working branch at inspection start: `codex/public-sample-ux-docs`
- Inspection start commit: `65e2fd8b77989172d68d0aad81d489df94baf474`
- Dev refactor baseline aligned before this change: `origin/codex/public-sample-ux-docs`
  (`19d7ab0b93649d0a0d178aa11108bcbe3f52adf1`)
- Target framework: `net8.0-windows7.0` (`src/OpenVisionLab/OpenVisionLab.csproj`)
- Product version for this single refactoring change set: `2.2.0-dev.2`

The survey covered the solution, all 27 projects, source, XAML, ViewModels,
services, SDK boundaries, smoke/contract tools, and the existing refactor
reports. Algorithm behavior, Recipe/XML format, SDK contracts, explicit
Preview/Run behavior, and equipment integration were not rewritten.

## 정량 감사 결과

The repeatable source audit reported:

| 항목 | 정리 전 | 정리 후 | 의미 |
| --- | ---: | ---: | --- |
| C# source files | 844 | 844 | 파일 수와 구현은 보존 |
| XAML files | 60 | 60 | View 자산은 보존 |
| partial declarations | 110 | 110 | 기존 generated/framework composition 유지 |
| project references | 34 | 34 | dependency graph 변경 없음 |
| project cycles | 0 | 0 | 순환 없음 |
| 프로젝트 루트의 10개 초과 C# 파일 | 3 | 2 | Docking.Controls 루트 혼잡 해소 |

정리 전 root-class 후보는 `OpenVisionLab.Docking.Controls`(49개),
`PipelineViewerScreenshotSmoke`(47개), `VisionRecipeRunnerSmoke`(33개)였습니다.
두 smoke 프로젝트는 독립 실행 harness로서 command dispatcher, contract,
fixture, report가 같은 실행 경계에 있고 `.csproj`와 embedded compile 목록이
경로를 직접 참조하므로 이번 배치에서 파일을 기계적으로 분할하지 않았습니다.

`src/OpenVisionLab/Common`은 당시 23개 파일과 세 namespace가 섞인 조사 후보였지만,
공개/역사 문서와 readiness 검사에 기존 경로가 다수 기록되어 있고 공통 helper의
실제 수명 경계가 아직 하나로 합의되지 않았습니다. 따라서 이번 버전에서는
분류와 후속 검토 대상으로 기록하고 물리 이동하지 않았습니다. 이 후보는
R15에서 namespace/type 계약을 보존한 책임별 physical folder 이동으로 후속
완료했습니다.

## 적용한 폴더 배치

`src/Libraries/OpenVisionLab.Docking.Controls`의 root C#·XAML을 다음 책임별
폴더로 이동했습니다. 모든 namespace, type name, access modifier, XAML
`x:Class`, binding key는 그대로 유지했습니다.

| 폴더 | 소유 책임 | 대표 파일 |
| --- | --- | --- |
| `Contracts/` | Shell과 docking library 사이의 공개 계약 | `IOpenVisionDockDocumentWorkspace`, `IOpenVisionDockLifecycle` |
| `Models/` | document/workspace 상태·레이아웃·snapshot 데이터 | `OpenVisionDockDocumentState`, `OpenVisionDockWorkspaceHandle` |
| `Converters/` | XAML 표시용 값 변환 | `OpenVisionCountToVisibilityConverter` |
| `Documents/` | document 생성·projection·state 동기화 | `OpenVisionDockDocumentController`, `OpenVisionDockDocumentStateStore` |
| `Workspace/` | workspace composition, layout, move, cleanup, save lifecycle | `OpenVisionDockWorkspaceController` 및 cohesive partial 구성 |
| `Guides/` | docking guide 정책, parser, presenter, 진단 | `OpenVisionLayerDockingGuidePresenter` |
| `LayerDocking/` | layer docking command/gesture 입력 | `OpenVisionLayerDockingGestureController` |
| `Views/` | WPF View, code-behind, view resources | `OpenVisionLayerDockWorkspaceView.xaml` |

`Views/OpenVisionLayerDockWorkspaceView.xaml`의 pack URI만
`Views/OpenVisionLayerDockWorkspaceView.Resources.xaml`로 갱신했습니다. View
code-behind는 UI lifecycle/event 연결만 유지하고 비즈니스·검사·Recipe 정책을
추가하지 않았습니다. 별도 링크 프로젝트인
`tools/OpenVisionLab.ImageCompare/OpenVisionLab.ImageCompare.csproj`에는 이동된
`ImageCompareDirectoryPolicy.cs`를 명시적으로 포함해 외부 소비자 빌드 경계를
보존했습니다.

## 소유권과 호출 경로

```text
OpenVisionShellHostWindow
  -> OpenVisionDockWorkspaceComposition
  -> OpenVisionDockWorkspaceController
  -> Documents/* / Workspace/* / LayerDocking/*
  -> OpenVisionLayerDockWorkspaceView (WPF binding and visual events)
  -> Guides/* and Views/* overlay presentation
```

- 현재 owner: `OpenVisionDockWorkspaceController`가 workspace mutable state와
  cleanup을 소유하고, document state controller/store가 document state를
  소유합니다.
- 의도한 physical owner: 위 책임별 폴더입니다. 파일 이동은 owner나 호출자를
  바꾸지 않았습니다.
- 상태 write owner: workspace controller와 document state controller; View는
  docking visual event를 owner에게 전달합니다.
- 수명/해제 owner: workspace controller의 기존 `Dispose`/cleanup 경로와
  document synchronization/state store의 기존 해제 순서를 유지합니다.
- 공개/바인딩 계약: `OpenVisionLab.Docking.Controls` namespace와 public
  contracts, `OpenVisionLayerDockWorkspaceView`/overlay XAML class names를
  유지했습니다.

## partial과 MVVM 판정

110개 partial은 생성 코드, WPF View composition, AvalonDock workspace의
cohesive file split, OpenGL control composition으로 분류되었습니다. 파일 길이만
줄이기 위해 새 partial을 만들거나 기존 partial을 제거하지 않았습니다.
Docking workspace partial은 동일 private state와 동일 lifecycle owner를
공유하므로 `Workspace/` 안에서 유지하는 것이 호출 흐름을 숨기지 않습니다.

전체 감사에서 ViewModel의 직접 Window/Dialog 생성, `Process.Start`, Shell
storage 호출은 발견되지 않았습니다. 감사 도구의 `RoiImageCanvasViewModel`
신호 1건은 UI 객체가 아니라 `EnumItemType.Window` 비교이며 false-positive로
확인했습니다. Image Compare와 Log Panel의 외부 I/O는 기존 concrete policy
owner를 그대로 사용합니다.

## 확인한 기능 상태

- 구현 완료 및 기존 계약으로 검증됨: Docking document/workspace lifecycle,
  guide overlay, converters, AvalonDock package boundary.
- 구현되었으나 이 배치에서 재실행하지 않은 범위: 실제 desktop docking
  interaction, 모든 WPF theme/DPI 조합, 장시간 native resource soak.
- 문서/계획으로만 남은 범위: smoke 프로젝트의 더 세분화된 physical folder
  배치와 `Common` namespace/responsibility 재정렬.
- 폐기/삭제: 이번 배치에서 기능 코드는 삭제하지 않았고, root 파일의 물리
  위치만 정리했습니다.

## 검증 기록

실행한 정적 검증:

```text
Invoke-RefactorAudit.ps1 -Verify
  REFACTOR_AUDIT=PASS|CSharpFiles=844|XamlFiles=60|PartialDeclarations=110|ProjectCycles=0

folder-inventory.ps1 (before)
  FOLDER_INVENTORY=PASS|Projects=27|SourceFiles=904|RootCandidates=3

folder-inventory.ps1 (after Docking.Controls move)
  FOLDER_INVENTORY=PASS|Projects=27|SourceFiles=904|RootCandidates=2|MixedNamespaceFolders=38
```

실행한 빌드와 계약 검증:

```text
dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU" --no-restore
  warnings=0; errors=0
dotnet build OpenVisionLab.sln -c Release -p:Platform="Any CPU" --no-restore
  warnings=0; errors=0
dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Debug/Release --no-restore
  warnings=0; errors=0 (각 구성)
TestDocumentationIndex.ps1
  DocumentationIndex=PASS IndexedPaths=277 Routes=15 RootRedirects=102
OpenVisionReadinessCheck Debug/Release
  OpenVisionLab readiness contract passed (각 구성)
VisionRecipeRunnerSmoke ImageCompare directory/resource and namespace boundary
  Debug: directory PASS; resource PASS; namespace passed=12, failed=0
  Release: directory PASS; resource PASS; namespace passed=12, failed=0
```

최종 Git diff는 커밋 직전에 `git diff --check`와
`git diff --cached --check`로 재확인합니다. Desktop UI/runtime qualification은
지원 monitor/DPI/theme 조합과 장시간 native resource 조건을 실행하지 않았으므로
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`로 남깁니다.

## 추천 코드 읽기 순서

1. `README.md`의 **Start Here: Choose the Project**
2. `src/OpenVisionLab/Program.cs`
3. `src/OpenVisionLab/App/Bootstrap/OpenVisionLabApplication.cs`
4. `docs/admin/CODEBASE_STRUCTURE.md`의 `Image → Layer → Tool → Inspection → Pipeline → Recipe → Result → Review`
5. 이 보고서의 owner/call path
6. `OpenVisionLab.Docking.Controls/Contracts/`
7. `Documents/` → `Workspace/` → `LayerDocking/` → `Guides/` → `Views/`

Use one search to navigate: `rg -n "OpenVisionDockWorkspaceComposition|OpenVisionDockWorkspaceController|OpenVisionLayerDockWorkspaceView" src tools docs`.

## 후속 우선순위

1. 두 standalone smoke 프로젝트의 command/contract/fixture 경계를 별도
   dependency audit 후 검토합니다.
2. 운영 이미지와 지원 DPI/theme/monitor에서 docking runtime과 Mat/native
   lifetime을 실행 검증합니다.

이 항목들은 이번 Docking/Common physical folder 정리의 완료 조건에 포함되지 않으며,
재현 결함·새 caller·변경된 dependency boundary가 없으면 기존 완료 owner를
다시 분할하지 않습니다.
