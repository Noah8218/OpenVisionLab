# OpenVisionLab

[English](README.md)

OpenVisionLab은 **OpenCvSharp 4로 구현한 규칙 기반 머신비전 개발 도구**입니다.

검사 순서를 구성하고 툴 파라미터를 조정하며, 각 OK/NG 판정의 근거가 되는 영상과 측정값을 확인할 수 있습니다.

## 버전

현재 버전: `v2.2.0-dev.9`

이 프로젝트는 명시적인 버전 번호로 관리합니다. `dev`가 붙은 버전은 개발 빌드입니다.

### 최근 버전 이력

#### `v2.2.0-dev.9` (2026-10-17)

- 오프라인 매뉴얼의 버전을 애플리케이션과 맞추고 Git 체크아웃 후에도 Guide 해시가 일치하도록 수정했습니다.

#### `v2.2.0-dev.8` (2026-10-17)

- 영상 선택, 툴 편집기 열기와 로그 필터 변경 시 중복 갱신을 줄였습니다.
- 메인 툴 편집, 레시피 전환과 검사 결과 확인 동작을 개선했습니다.
- 전체 한국어 README를 추가하고 화면 사진과 작업 흐름 영상을 교체했습니다.

#### `v2.2.0-dev.7` (2026-10-06)

- Recipe Manager의 선택과 명시적인 작업 열기를 개선하고, 취소하거나 저장에 실패해도 편집 내용을 보존합니다.
- 공통 컨트롤과 아홉 UI 영역을 정비하고, ROI 대화상자 배치·긴 툴 오류 표시·입출력 레이어 이름을 개선합니다.
- 출력 레이어 대상 동기화를 수정하고 메인 작업 공간, 네이티브 Tool 양식, Recipe 저장, Pipeline 실행과 LLM/XML 검토를 갱신합니다.

#### `v2.2.0-dev.6` (2026-09-27)

- 레시피 작업 공간 소스와 연동 alpha.4 패키지를 포함합니다.
- 같은 의존성 버전으로 복원하도록 NuGet 잠금 파일을 추가합니다.
- 소스 빌드에 필요한 Visual Studio 워크로드, Desktop SDK와 Git 요구사항을 명확히 합니다.

#### `v2.2.0-dev.5` (2026-09-17)

- 저장소 간 스모크 실행 전에 checkout 커밋과 진입 어셈블리의 바이트를 각각 대조하고, 일치하지 않으면 거부하는 2D 런타임 적격성 검사를 추가합니다.
- 별도 EXE 연동 근거에 프로필, 소스·레시피 식별 정보, 원본 영상 크기와 스크린샷 보고를 추가합니다.

#### `v2.2.0-dev.4` (2026-09-16)

- 재시도, 오류 주입, 입력 식별과 출력 할당을 확인하는 2D 연동 검사를 추가합니다.

#### `v2.2.0-dev.3` (2026-09-16)

- Pipeline Review, Recipe 전환, 다중 영상 검증, 도킹, 툴 조작, 작업 공간 복원, 저장소 복구와 실행 출처 정보를 보강해 결정적 작업 흐름을 더 명확히 합니다.
- 2D 연동 검사와 출력 할당 사전 검사 가드를 추가합니다. LLM/XML 보조 기능은 선택 사항으로 유지합니다.

#### `v2.2.0-dev.2` (2026-09-10)

- Docking.Controls 소스를 contracts, models, documents, workspace, guides, layer docking, converters와 Views로 정리합니다.
- 네임스페이스, XAML 바인딩, ResourceDictionary 동작과 기존 Image → Layer → Tool → Inspection → Pipeline → Recipe → Result → Review 흐름을 유지합니다.

#### `v2.2.0-dev.1` (2026-09-09)

- 애플리케이션·설명서·연동 식별에 사용하는 버전을 하나로 맞춥니다.
- 검증된 ImageCanvas, Image Compare, Recipe, Shell, PropertyGrid, Learn과 네임스페이스 소유권 경계를 반영합니다.

## 개요

OpenVisionLab은 샘플 영상으로 규칙 기반 검사 레시피를 구성하고 검증하는 Windows 데스크톱 애플리케이션입니다.

- 작업 흐름: 영상 불러오기 -> 레이어 선택 -> 툴 구성 -> Preview 또는 Run -> Pipeline/Recipe 저장 -> Good/Bad 샘플 검증
- 주요 툴: Threshold, Filter, Morphology, Blob, Contour, Matching, FeatureMatching, EdgeBasedMatching, Line/Length, Mean
- 현재 중점: PropertyGrid 기반 Tool View, Pipeline Review, 중간 레이어 비교, 공개 샘플과 다중 영상 검증
- LLM/XML 보조 기능은 선택 사항이며 애플리케이션 사용에 필수적이지 않습니다.

## 레시피 구성과 확인

![레시피 생성, 영상 불러오기와 검사 툴 티칭](docs/assets/demo/readme-workflows/recipe-teaching.gif)

1. **Recipe Manager**에서 `Shape Count`를 만들고 **Open work with selected inspection**을 선택한 뒤 영상을 엽니다.
2. **Threshold**를 엽니다. **Run on change**를 끄고 임계값을 `150`으로 설정한 뒤 **Run tool (F8)** 버튼을 선택합니다.
   원본과 이진화 영상을 비교한 다음 **Add to pipeline → Add at end**를 선택합니다.
3. **Contour**를 열고 입력이 `Threshold_1`인지 확인합니다. **Use threshold**를 끄고 다섯 형상을 포함하도록 검사 영역(ROI)을 그린 뒤 **Apply**를 선택하거나 **Enter**를 누릅니다.
4. **Acceptance**를 켜고 **Detection count**를 선택합니다. **Lower**와 **Upper**를 체크하고 두 값을 모두 `5`로 설정한 뒤 **F8**을 누릅니다.
   검출된 형상과 개수 `5`를 확인한 다음 **Ctrl+Enter**로 단계를 추가합니다.
5. **F5**로 파이프라인을 실행합니다. **Results** 탭에서 결과를 확인하고 한 패널·두 패널·네 패널 뷰어에서 원본·중간·출력 레이어를 비교합니다.
6. **Ctrl+S**로 저장합니다. 다른 레시피를 선택했다가 돌아와 저장한 단계·파라미터·ROI·레이어 연결을 다시 불러옵니다.
7. Good과 Bad 영상을 추가하고 같은 레시피로 **Batch inspection**을 실행합니다. 각 영상의 측정값과 OK/NG 결과를 확인합니다.

**Run on change**가 꺼져 있으면 파라미터를 바꿔도 툴을 실행하기 전까지 결과는 대기 상태로 남습니다.
레시피를 불러오면 검사를 실행하지 않고 설정을 복원합니다. 저장과 실행은 별도 동작입니다.

활성 단계에 판정 조건이 없으면 실행을 마쳐도 판정없음으로 표시될 수 있습니다.
실행 오류와 검사 NG는 별개입니다. 입력이나 파라미터가 바뀌면 남아 있는 결과는 이전 결과로 표시되며, 현재 결과를 얻으려면 다시 실행해야 합니다.
일괄 처리에서는 모든 중간 영상을 보관하지 않습니다. 해당 출력을 확인하려면 선택한 영상을 명시적으로 실행합니다.

## 한 영상에서 모든 검사 단계 확인

![OpenVisionLab의 Filter, Threshold, Morphology, Blob, Contour 비교](docs/assets/demo/readme-workflows/inspection-stages.png)

네 패널 뷰어에서 원본·필터·이진화·Morphology 출력 영상을 비교합니다.
이 공개 타공판 레시피는 `Filter -> Threshold -> Morphology`를 실행한 뒤 정리된 레이어를 Blob과 Contour로 분기합니다.
두 단계 모두 구멍 34개를 검출하며, **Results** 탭에서 개수와 판정을 확인할 수 있습니다. 같은 면적 조건을 적용해 전체 영상의 배경 윤곽선은 제외합니다.

[재현 가능한 공개 Pipeline 열기](docs/samples/public/product/Product_Field_PerforatedPlate_Inspection.pipeline.xml)
또는 샘플 목록에서 `Product_Field_PerforatedPlate_HoleArray`를 선택합니다.

각 단계를 조정하고, 중간 영상을 비교하고, 최종 표시와 결과 지표를 확인한 뒤 Pipeline Review에서 같은 근거를 확인하는 흐름이 OpenVisionLab의 중심 작업입니다.
Preview와 Run은 사용자가 명시적으로 실행하는 동작으로 유지됩니다.

## 사용 설명서

애플리케이션에서 `Guide`를 선택하면 오프라인 사용 설명서가 열립니다.
같은 원본 문서는 [docs/manual](docs/manual/README.md)에서 관리합니다. 모든 작업 흐름과 17개 툴 장에는 현재 OpenVisionLab UI 캡처, 번호가 붙은 설명과 해당 클릭·검토 순서가 포함되어 있습니다.

## 현재 EXE의 툴 뷰

아래 영상에서는 메인 작업대에서 Matching, Line, Blob, Contour의 설정과 결과를 차례로 확인합니다.

![OpenVisionLab 툴 뷰 시연](docs/assets/demo/readme-workflows/tool-settings.gif)

[툴 뷰 MP4 보기](docs/assets/demo/readme-workflows/tool-settings.mp4)

- Matching: 불러온 템플릿과 매칭 점수, 검출 위치를 확인합니다.
- Line: Edge·Distance·Intersection 모드를 선택합니다. 영상에서는 핀 간 거리를 측정합니다.
- Blob: 입자 검출 결과와 개수 판정 범위를 확인합니다.
- Contour: 검사 ROI와 개수 판정 범위를 설정하고 검출된 형상을 확인합니다.

선택한 툴의 결과는 **Run step (F8)**, 전체 검사 결과는 **Run pipeline (F5)** 버튼으로 확인합니다.

## 연속 처리

OpenVisionLab에서는 전처리와 검출 툴을 Pipeline으로 연결할 수 있습니다.
아래 영상에서는 세 단계로 구성된 두 Pipeline의 중간 영상과 결과를 비교하고, Morphology의 연산과 커널 설정을 확인합니다.

- `Filter -> Threshold -> Contour`
- `Threshold -> Morphology -> Contour`

![OpenVisionLab 전처리 연결 시연](docs/assets/demo/readme-workflows/preprocessing-chains.gif)

[연속 처리 MP4 보기](docs/assets/demo/readme-workflows/preprocessing-chains.mp4)

Pipeline Review에서 각 단계를 선택해 중간 영상을 확인하고, 마지막 단계의 검출 객체와 표시를 검토할 수 있습니다.

개별 툴은 **F8**, 파이프라인은 **F5**로 실행합니다.
**Run on change** 옵션이 있는 툴은 파라미터가 바뀔 때도 실행할 수 있습니다.

다음 예제는 `Filter → Threshold → Contour`와 `Threshold → Blob`을 연결합니다.
입출력 영상을 비교한 뒤 검출 개수와 판정 범위를 확인합니다.

![전처리와 입자 개수 검사](docs/assets/demo/readme-workflows/sample-inspections.gif)

## 시작하기: 프로젝트 선택

일반 개발에서는 `OpenVisionLab.sln`을 엽니다. **OpenVisionLab**을 시작 프로젝트로 지정하고 `Debug` / `Any CPU`를 사용합니다.
제품은 `src/OpenVisionLab/Program.cs`에서 시작해 `src/OpenVisionLab/App/Bootstrap/OpenVisionLabApplication.cs`를 거쳐 `OpenVisionShellHostWindow`로 이어집니다.
검증 도구를 F5 시작 프로젝트로 선택하지 마세요.

솔루션에는 제품과 공통 라이브러리 프로젝트가 포함되어 있습니다.
다른 `.csproj` 파일 중 `tools/` 아래에 있는 파일은 독립된 검증 진입점이므로, 특정 검사가 필요할 때 해당 프로젝트 경로로 열거나 실행합니다.

| 작업 내용 | 열 프로젝트 또는 폴더 | 먼저 읽을 코드 |
| --- | --- | --- |
| 제품 작업 흐름·Shell·Recipe·Pipeline 실행 또는 변경 | `src/OpenVisionLab/OpenVisionLab.csproj` | `Program.cs` -> `App/Bootstrap/OpenVisionLabApplication.cs` -> `UI/Menu/Wpf/` 또는 `Core/Recipe/` / `Core/Pipeline/` |
| 영상 레이어·lease·레이어 표시 상태 변경 | `src/Libraries/OpenVisionLab.ImageSpace.Core/`와 `src/Libraries/OpenVisionLab.Display.Core/` | `ImageSpaceService.cs`와 `docs/admin/CODEBASE_STRUCTURE.md`에 기록된 표시 담당 코드 |
| 영상 캔버스·ROI 입력·캔버스 저장 변경 | `src/Libraries/OpenVisionLab.ImageCanvas/` | `View/`, `ViewModel/`, `RoiInteraction/` |
| 공통 PropertyGrid 계약 또는 WPF 연결 변경 | `src/Libraries/PropertyGrid.Abstractions/` 또는 `src/Libraries/WpfPropertyGridBridge/` | `PropertyGridContracts.cs` -> `WpfPropertyGridAdapter.cs` |
| 공통 Pipeline·Docking·Logging·Localization 컨트롤 변경 | `src/Libraries/` 아래의 해당 프로젝트 | 해당 `View/`, `ViewModel/`, `Services/` 담당 코드 |
| 구조 또는 기능 검사 실행 | `tools/` 아래의 해당 프로젝트 | `tools/OpenVisionReadinessCheck/`, `tools/VisionRecipeRunnerSmoke/`, `tools/PipelineViewerScreenshotSmoke/` |

코드를 가장 짧게 읽는 순서는 `AGENTS.md` -> `docs/README.md` -> `docs/admin/CODEBASE_STRUCTURE.md` -> 해당 기능의 안정 계약 -> 구조 문서에 기록된 담당 코드입니다.
PowerShell의 기본 명령은 다음과 같습니다.

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --project "tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj" -c Debug -- "$PWD"
```

## 새 GitHub 클론에서 빌드하기

### 권장 방법: Visual Studio 2022만 사용

요구사항:

- Windows 10 또는 Windows 11 x64
- Visual Studio 2022 17.8 이상
- Visual Studio Installer에서 `.NET desktop development` 워크로드 선택
- 최초 NuGet 패키지 복원을 위한 인터넷 연결

이 워크로드는 MSBuild, WPF 개발 도구와 해당 .NET SDK를 제공합니다.
빌드 과정에서는 Git도 직접 호출합니다. 개발에 사용할 터미널에서 `git --version`을 확인하세요.
실행되지 않으면 PATH 지원을 포함해 Git for Windows를 설치한 뒤 Visual Studio를 다시 시작합니다. GitHub Desktop이 있다는 사실만으로 이 확인을 대신할 수는 없습니다.

1. **Visual Studio Installer**를 엽니다.
2. Visual Studio 2022에서 **Modify**를 선택하고 **.NET desktop development**를 선택한 뒤 변경 사항을 적용합니다.
3. Visual Studio를 시작하고 **Clone a repository**를 선택합니다.
4. `https://github.com/Noah8218/OpenVisionLab.git`을 입력하고 `C:\src\OpenVisionLab` 또는 `D:\src\OpenVisionLab`처럼 짧은 로컬 경로를 선택합니다.
5. `main` 브랜치를 선택하고 `OpenVisionLab.sln`을 엽니다. `OpenVisionLab`을 시작 프로젝트로 지정하고 `Debug`와 `Any CPU`를 선택한 뒤 **Build > Build Solution**을 실행합니다.
6. **F5**를 누르거나 `OpenVisionLab` 시작 버튼을 선택합니다.

워크로드 없이 Visual Studio를 설치했다면 **Tools > Get Tools and Features**에서 추가합니다.
Visual Studio 2022 17.8 이전 버전은 이 `net8.0` 솔루션을 빌드할 수 없습니다.

### 명령줄 대안

요구사항:

- Windows 10 또는 Windows 11 x64
- Git과 Windows PowerShell
- .NET SDK `8.0.100` 이상 `8.x` 계열 또는 .NET SDK `9.x`
- SDK `9.x`를 명령줄용으로 설치한 경우 .NET 8 Desktop Runtime
- 최초 NuGet 패키지 복원을 위한 인터넷 연결

```powershell
New-Item -ItemType Directory -Force C:\src | Out-Null
Set-Location C:\src
git clone https://github.com/Noah8218/OpenVisionLab.git
cd OpenVisionLab
powershell -NoProfile -ExecutionPolicy Bypass -File tools\VerifySourceBuild.ps1
.\bin\Debug\OpenVisionLab.exe
```

Windows에서 긴 경로 지원이 꺼져 있다면 `C:\src\OpenVisionLab`이나 `D:\src\OpenVisionLab`처럼 짧은 checkout 경로를 사용합니다.
WPF/MSBuild 릴리스 빌드는 긴 임시 파일명을 만듭니다. checkout 경로가 이를 지원하지 못하면 `VerifyReleaseCandidate.ps1`이 경로 길이 오류를 명확히 표시하고 초기에 중단합니다.

`VerifySourceBuild.ps1`은 잠금 파일 기준의 패키지 복원, Debug·Release 솔루션 빌드, 저장소 준비 상태 검사와 포함된 외부 DLL 검사를 수행합니다.
설치 프로그램을 만들거나 공개 샘플 검사를 실행하지 않으며, Recipe/Layer 상태를 변경하지 않습니다.

저장소의 `dll\OpenVisionLab-Vision-SDK`에는 검증된 OpenVisionLab Vision SDK 3.0 런타임이 이미 포함되어 있습니다.
OpenVisionLab을 빌드하거나 실행하기 위해 SDK 소스를 별도로 복제할 필요는 없습니다.
`sdk-manifest.json`에는 SDK 소스의 정확한 커밋과 포함된 모든 SDK DLL의 SHA-256이 기록되어 있습니다.

SDK가 없다는 메시지가 표시되면 위의 Visual Studio 워크로드를 추가하거나 [.NET 다운로드 페이지](https://dotnet.microsoft.com/download)에서 지원되는 SDK를 설치합니다.
저장소는 `8.0.100` 이상 `8.x` SDK를 허용하며 `8.0.300`도 포함됩니다. 현재 Visual Studio 2022에 포함된 `9.x` SDK도 사용할 수 있습니다.
SDK 10은 이 Visual Studio 2022 소스 빌드 경로의 검증 대상이 아닙니다.
독립적으로 설치한 SDK 9로 이 `net8.0-windows` 애플리케이션을 실행하려면 [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)도 필요합니다. 권장 경로인 Visual Studio 워크로드를 사용하면 이 런타임이 함께 설치됩니다.

### 선택 사항: 깨끗한 Windows Sandbox에서 확인

Windows Sandbox를 활성화한 Windows Pro, Enterprise 또는 Education 사용자는 폐기 가능한 VM에서 추가 검사를 실행할 수 있습니다.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\WindowsSandbox\InvokeSourceBuildSandbox.ps1
```

호스트 스크립트는 현재 커밋된 소스를 압축하고 새 작업 전용 근거 폴더만 Sandbox에 연결합니다.
그 안에서 고정된 SDK를 내려받아 같은 소스 빌드 검증기를 실행합니다.
Sandbox는 임시 환경이며, 실행기는 결과를 `artifacts\source_build_sandbox_*`에 기록한 뒤 인스턴스를 닫습니다.
`sandbox-progress.txt`에서 SDK 다운로드·압축 해제·복원·빌드의 현재 단계를 확인할 수 있습니다.
SDK와 NuGet 패키지 다운로드에는 네트워크 연결이 필요합니다.

Windows Sandbox는 선택 사항입니다. GitHub Actions는 이미 깨끗한 Windows checkout에서 이 빌드 검사보다 엄격하고 범위가 넓은 검사를 실행합니다.
OpenVisionLab 빌드만을 위해 Windows 기능이나 가상화 설정을 바꾸지 마세요.

## 요구사항과 애플리케이션 실행

요구사항:

- Windows 개발 환경
- `.NET desktop development`가 있는 Visual Studio 2022 17.8+ 또는 앞서 설명한 지원 대상 명령줄 .NET SDK
- WPF/Windows Desktop 빌드 지원
- 저장소 `dll/` 디렉터리에 포함된 런타임 DLL

저장소 루트에서 실행합니다:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
.\bin\Debug\OpenVisionLab.exe
```

![OpenVisionLab 메인 작업 공간](docs/assets/demo/readme-workflows/main-workspace.png)

메인 작업 공간의 주요 영역은 다음과 같습니다:

1. `Pipeline`: 단계 순서, 레이어 연결과 최종 판정을 확인합니다.
2. `Tools`: 구성할 툴을 선택하고 Pipeline에 추가합니다.
3. `Image viewer`: 한 패널·두 패널·네 패널에서 원본·중간·출력 레이어를 비교합니다.
4. `Tool panel`: 입력 레이어를 확인하고 파라미터·ROI·판정 범위를 설정한 뒤 툴을 실행합니다. 뷰어의 표시 레이어를 바꿔도 툴 입력은 바뀌지 않습니다.
5. `Images` / `Results`: 검사 영상을 관리하고 단계별 측정값·실행 상태·OK/NG 사유를 확인합니다.

## 제품 범위

OpenVisionLab은 영상 검사 규칙을 티칭하고 조정하며 검증하도록 설계되었습니다.

규칙 기반 검사를 구성할 때 흔히 겪는 문제는 다음과 같습니다:

- 표시 중인 영상이 원본인지 중간 결과인지 구분하기 어려움
- 한 툴의 출력이 의도한 다음 툴에 연결됐는지 알기 어려움
- Score, Count, Area 값은 보이지만 OK/NG 판정 근거를 이해하기 어려움
- Preview와 저장한 Recipe의 결과가 달라짐
- Good/Bad 샘플에서 같은 검사를 반복하는 데 시간이 많이 듦

OpenVisionLab은 Layer, Tool, Pipeline, Recipe, Pipeline Review와 Sample Catalog를 하나의 작업 흐름으로 연결합니다:

```text
Load image
  -> Confirm input Layer
  -> Select a Tool
  -> Configure parameters in the PropertyGrid
  -> Preview / Run
  -> Inspect the output Layer
  -> Add a Pipeline Step
  -> Review measurements and OK/NG status
  -> Save the Recipe XML
  -> Validate with Good and Bad samples
```

## 주요 기능

| 영역 | 설명 |
| --- | --- |
| 메인 작업 공간 | 영상 불러오기, 레이어 관리, 확대·축소, 이동과 픽셀 값 확인 |
| 레이어 도킹 | 탭 또는 분할 보기에서 원본·중간·최종 영상 비교 |
| 툴 뷰 | Threshold, Filter, Morphology, Blob, Contour, Matching, FeatureMatching, Line, Mean 등 툴 설정 |
| PropertyGrid | 일관된 속성 인터페이스로 툴 파라미터 편집 |
| Preview / Run | 툴 또는 전체 Pipeline 실행. 지원하는 툴에서는 Run on change로 파라미터 변경에 따른 실행 제어 |
| Pipeline / Recipe | 여러 툴을 순서 있는 단계로 연결하고 XML로 저장 |
| Pipeline Review | 단계별 입출력·측정값·결과 표시·로그·OK/NG 근거 확인 |
| Sample Catalog | 샘플 영상·기본 Pipeline·기대 결과·Good/Bad 쌍 조회 |
| Good/Bad 쌍 | 같은 측정값으로 정상·불량 샘플 비교 |
| Learn 모드 | Matching, Blob, Contour, Threshold, Mean, FeatureMatching, EdgeBasedMatching, Line 안내 열기 |
| 언어 | 한국어와 영어 전환 |

## 공개 샘플

`docs/samples/public`에는 검사 작업 흐름을 익힐 수 있는 예제 영상과 Pipeline이 있습니다.
각 샘플은 영상·권장 Pipeline·확인할 측정값·Good/Bad 기대값으로 구성됩니다.

공개 배포용 목록은 다음과 같습니다:

- `docs/samples/OpenVisionLab.PublicSampleCatalog.csv`
- `docs/samples/OpenVisionLab.ProductSampleCatalog.csv`

루트 `Sample/` 디렉터리는 로컬 또는 서드파티 데이터용이며 공개 샘플 배포에 포함되지 않습니다.

![OpenVisionLab 공개 샘플 목록](docs/assets/demo/readme-workflows/sample-catalog.png)

대표적인 샘플 쌍:

| 검사 | Good 샘플 | Bad 샘플 | 측정값 |
| --- | --- | --- | --- |
| Matching으로 대상 위치 찾기 | `Public_Matching_DiePad_Good` | `Public_Matching_DiePad_NoTarget_Bad` | `ResultCount`, `ScoreMax` |
| Blob으로 입자 개수 세기 | `Public_Blob_Particles_Good` | `Public_Blob_Particles_Sparse_Bad` | `ResultCount` |
| Contour로 형상 개수 세기 | `Public_Contour_Shapes_Good` | `Public_Contour_Shapes_Missing_Bad` | `ResultCount` |
| Threshold로 밝은 패드 분리 | `Public_Threshold_BandPads_Good` | `Public_Threshold_BandPads_Missing_Bad` | `ResultCount` |
| Mean으로 밝기 확인 | `Public_Mean_Brightness_Good` | `Public_Mean_Brightness_Dark_Bad` | `MeanValueAvg` |
| 특징 매칭 점수 비교 | `Public_Feature_Card_Good` | `Public_Feature_Card_Wrong_Bad` | `ScoreMax`, `ResultCount` |
| 에지 기반 형상 비교 | `Public_Edge_Fiducial_Good` | `Public_Edge_Fiducial_Wrong_Bad` | `ScoreMax`, `ResultCount` |
| Line으로 거리 측정 | `Public_Line_Pins_Good` | `Public_Line_Pins_WidePin_Bad` | `DistanceMmAvg` |

저장한 `Shape Count` 레시피를 원본과 형상 누락 영상에 적용합니다.
**Good**과 **Bad** 라벨을 지정한 뒤 **Batch inspection**을 실행합니다.
위의 ROI와 개수 조건을 사용하면 원본은 **검출 5개 / OK**, 형상 누락 영상은 **검출 2개 / NG**가 됩니다.
일괄 검사 결과는 **OK 1장·NG 1장·오류 0장**입니다.

Good/Bad 라벨은 기대 분류를, OK/NG는 검사 결과를 나타냅니다.
영상을 선택해 결과를 확인한 뒤 원본·처리 영상·오버레이·판정 범위를 비교합니다.
**Page Up**을 누르면 이전 영상으로 돌아갑니다.

![레시피 재로드와 Good/Bad 검사 결과 비교](docs/assets/demo/readme-workflows/recipe-validation.gif)

## 툴 구조

알고리즘 툴은 PropertyGrid 기반 설정 모델을 사용합니다:

```text
Tool Property Model
  -> PropertyGrid SelectedObject
  -> Preview / Run
  -> VisionToolResult
  -> Metrics / Overlays / Logs
  -> Pipeline Step XML
```

이 구조는 새 툴을 추가할 때도 툴 설정과 Recipe 직렬화의 일관성을 유지합니다.
Basic, Fast, Precise 같은 프리셋으로 시작한 뒤 PropertyGrid에서 개별 파라미터를 조정할 수 있습니다.

## 파이프라인 검토

Pipeline Review는 각 단계의 실행 순서와 보관된 결과를 보여 줍니다.
다음 내용을 확인할 수 있습니다:

- 선택한 단계의 입력·출력 레이어
- 단계 사이의 연결과 분기
- 결과 영상과 검출 표시
- `ResultCount`, `AreaMax`, `ScoreMax`, `LineAngleAvg` 같은 측정값
- 설정한 판정 범위와 측정값
- OK 또는 NG 판정의 근거
- 조정이 필요할 수 있는 파라미터

![OpenVisionLab Pipeline Review](docs/assets/demo/readme-workflows/pipeline-results.png)

## 빌드

저장소 루트에서 실행합니다:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
```

Debug 실행 파일은 다음 경로에 생성됩니다:

```powershell
.\bin\Debug\OpenVisionLab.exe
```

## 검증

저장소 루트에서 기본 빌드와 준비 상태 검사를 실행합니다:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --project "tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj" -c Debug -- "$PWD"
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.PublicSampleCatalog.csv -OutputDir artifacts\public_sample_catalog
```

현재 소스의 WPF 뷰 검사를 실행합니다:

```powershell
dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\smoke\recipe_language_controls
```

실제 EXE 스모크 시나리오를 실행합니다:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
.\bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs --output artifacts\smoke\recipe_manager_tabs
```

화면 UI나 작업 흐름이 바뀌면 현재 빌드에서 새로운 근거를 캡처합니다.
이전 빌드의 스크린샷을 현재 UI의 근거로 사용할 수는 없습니다.

## 지속적 통합

GitHub Actions 워크플로는 `.github/workflows/ci.yml`에 정의되어 있습니다.

Windows CI 게이트는 다음을 검사합니다:

- 솔루션 빌드
- 준비 상태 계약
- 외부 참조 정책
- 공개 샘플 자산 정책

변경 사항에 현재 빌드의 근거가 필요하면 WPF 스크린샷과 실제 EXE UI 검사를 로컬에서 실행합니다.

## 릴리스

- 변경 이력: [CHANGELOG.md](CHANGELOG.md)
- 릴리스·버전 정책:
  [docs/OPENVISIONLAB_RELEASE_VERSION_POLICY.md](docs/OPENVISIONLAB_RELEASE_VERSION_POLICY.md)

제품 버전의 원본은 `src/OpenVisionLab/OpenVisionLab.csproj`입니다. 위 버전 표시와 변경 이력은 이 값을 따릅니다.

## 로드맵

기능 변경은 검사 작업 흐름과 기존 계약을 기준으로 검토합니다:

- [검사 작업 흐름](docs/manual/README.md)
- [Tool·Recipe 계약](docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md)
- [공개 예제](docs/samples/public/README.md)
- [소스 책임과 구조](docs/admin/CODEBASE_STRUCTURE.md)

툴 변경에는 입력 영상, 파라미터, 출력 레이어와 판정 지표를 포함합니다.
작업 흐름을 바꿀 때는 재현 절차와 영향을 받는 컨트롤의 확인 결과를 함께 남깁니다.

## 범위와 한계

- LLM/XML 보조 기능은 선택 사항이며 검사 실행에 포함되지 않습니다.
- 공개 배포 샘플은 `docs/samples/public`과 목록 CSV 파일에 정의되어 있습니다. `Sample/` 아래의 로컬 또는 서드파티 파일은 공개 배포 자산이 아닙니다.
- 공개 예제의 결과만으로 실제 운영 환경의 견고성을 입증할 수는 없습니다.
- 화면에 보이는 변경 사항은 현재 빌드에서 확인해야 완료로 간주합니다.

## 문서

- [튜토리얼](docs/learn/OPENVISIONLAB_TUTORIAL.md)
- [Learn 모드 가이드](docs/learn/README.md)
- [안정 기능 계약](docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md)
- [문서 목차](docs/README.md)
- [공개 샘플 목록](docs/samples/OpenVisionLab.PublicSampleCatalog.csv)
- [제품 샘플 목록](docs/samples/OpenVisionLab.ProductSampleCatalog.csv)

## 라이선스와 저작권

OpenVisionLab은 [Apache License 2.0](LICENSE)으로 배포합니다.

이 프로젝트에는 최노아(Noah-Choi)가 개발한 소프트웨어가 포함되어 있습니다.

```text
This project includes software developed by 최노아(Noah-Choi).
Copyright (c) 2026 최노아(Noah-Choi).
```

라이선스 조건에 따라 상업적으로 이용하거나 수정·재배포할 수 있습니다.
소프트웨어의 복사본 또는 상당 부분에는 `LICENSE`, `NOTICE`, 저작권과 저작자 표시 고지를 유지해야 합니다.

## 요약

OpenVisionLab은 검사 조건을 구성하고 중간 결과를 확인하며 여러 영상으로 Recipe를 검증하는 규칙 기반 머신비전 개발 도구입니다.
공개 샘플은 시작점으로 사용할 수 있으며, 실제 운영에 사용하려면 해당 용도를 대표하는 데이터와 용도별 검증이 필요합니다.
