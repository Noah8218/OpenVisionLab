# OpenVisionLab 2D View 책임 경계 리팩토링 — 2026-09-12

## 범위와 기준

이번 작업은 새 기능을 만드는 작업이 아니라, 처음 저장소를 여는 개발자가
Shell과 Pipeline Review의 UI 수명·상태·이미지 소유권을 추적하기 어렵다는
문제를 줄이는 작업이었다. 현재 기준은 `codex/public-sample-ux-docs`,
`176eec95e0081502dc07d89ddaee666abac8123d`, `net8.0-windows7.0` WPF/x64다.

정상 동작하던 XAML 이름, binding, public View event, Preview/Run 흐름,
Recipe/Document/Pipeline owner, 이미지 clone 계약을 유지했다. 파일 길이만으로
새 계층을 만들지 않았고, 독립 상태·수명·호출 경계가 확인된 경우에만 concrete
owner를 추가했다.

## 실제로 바뀐 책임

| 단계 | 기존 owner | 현재 owner와 호출 경로 | 보존한 계약 |
| --- | --- | --- | --- |
| M1 Pipeline Review layout | `OpenVisionPipelineReviewView.xaml.cs` | `OpenVisionPipelineReviewView`의 XAML handler → `OpenVisionPipelineReviewLayoutController` | Details/Step Flow 상태, row sizing, visibility, tooltip, toggle visual 결과 |
| M2 Pipeline Review image lifetime | `OpenVisionPipelineReviewView.xaml.cs`의 Bitmap/state 필드와 `OnUnloaded` | `SetSelectedStep/SetObjectResults/SetCircleEvidence/SetMatcherDiagnostics/SetScaleCalibrationState` → `OpenVisionPipelineReviewImageResourceOwner` → 기존 Presenter/Render/ViewModel | 입력 Bitmap clone, 이전 resource dispose, matcher diagnostic preview, ViewModel BitmapImage 변환 |
| M3 Recipe Manager panel drag | `OpenVisionShellHostView.xaml.cs` | XAML mouse handler → `OpenVisionShellHostRecipePanelDragController`; 기존 lifecycle이 Dispose | pointer capture, drag offset clamp, test callback, Recipe toggle/tab/close 동작 |

Shell의 Recipe persistence, Pipeline execution, Layer/Workspace state, Recipe
command policy는 기존 concrete owner에 남겼다. `OpenVisionShellHostView`는
여전히 조합 계층이며, 모든 controller를 다시 Factory/Manager로 포장하지 않았다.

## 최종 구조 감사

- 시작 경로는 `Program.Main → OpenVisionLabApplication.Run →
  OpenVisionShellHostWindow → OpenVisionShellHostView`다.
- Pipeline Review 읽기 경로는 `Shell tool selection → document restore/create →
  OpenVisionPipelineReviewDocument → OpenVisionPipelineReviewView →
  LayoutController/ImageResourceOwner → ViewModel/Render service`다.
- C# 819개, XAML 60개, 프로젝트 27개(솔루션 등록 18개), project reference
  34개, project cycle 0개를 확인했다.
- partial 선언 59개와 문자열 매치 2개는 XAML/generated/framework 또는
  cohesive control composition으로 분류했다. 수동 partial을 추가하지 않았고,
  파일 길이만 줄이기 위한 partial도 만들지 않았다.
- View side-effect 검색에서 남은 Bitmap/File 경계는 ROI editor, template editor,
  result-evidence viewer처럼 기존 Window/파일 계약을 가진 화면과 display image
  adapter다. Shell/Pipeline Review에는 직접 Recipe storage·pipeline execution
  경로가 남아 있지 않다.
- `RoiImageCanvasViewModel`의 WPF/path coupling과 대형 smoke runner는 이번
  View owner 범위 밖의 후속 기술 부채로 남겼다.

## 첫 기여자 관점의 결과

처음에는 Pipeline Review View 안에서 layout state와 여러 Bitmap의 Dispose
경로를 함께 읽어야 했지만, 이제 각각 `LayoutController`와
`ImageResourceOwner`로 바로 이동할 수 있다. Shell의 Recipe Manager drag도
별도 파일에서 pointer state와 종료를 확인할 수 있다. 따라서 화면 동작의
작은 수정은 이전보다 탐색 범위가 줄었다.

여전히 `OpenVisionShellHostView` 생성자는 여러 기존 owner를 조합하고, Recipe
navigation과 document/tool window 연결은 Shell composition에서 확인해야 한다.
이는 새 결함이 아니라 현재 Shell의 실제 조합 책임이며, 파일 크기만으로 다시
분리하지 않았다. 다음 변경은 반드시 기존 owner map과 이 읽기 순서를 출발점으로
삼아야 한다.

## 검증 근거

증거는 `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\view-owner-refactor-20260912`
아래에 저장했다.

- M1: `m1-layout-owner/phase-summary.txt` 및 Pipeline Review focused screenshot
- M2: `m2-image-lifetime/phase-summary.txt` 및 image lifetime source check
- M3: `m3-shell-ui/phase-summary.txt` 및 Recipe Manager/Shell Chrome screenshots
- M4: `m4-final-audit/`

실행한 검증은 Solution Debug/Release build(각각 경고 0, 오류 0),
`TestDocumentationIndex.ps1`, `VisionUiContractCheck`, screenshot runner
contract 10/10, `OpenVisionReadinessCheck`, `Invoke-RefactorAudit.ps1 -Verify`,
`git diff --check`, 그리고 Pipeline Review/Recipe Manager focused WPF screenshot
smoke다. focused smoke 결과는 layout/text/internal 오류 0이었다.

전체 WPF theme/DPI/monitor/keyboard matrix, 모든 카메라·SDK·GPU 경로,
장시간 native hang/종료와 실제 생산 하드웨어는 실행하지 않았으므로
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`로 남긴다.
