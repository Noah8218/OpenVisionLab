# OpenVisionLab — 2D Inspection Workbench

Windows에서 OpenCvSharp 4와 C#으로 규칙 기반 2D 검사 Recipe를 구성하고 검증하는 도구입니다.

[![Release](https://img.shields.io/github/v/release/Noah8218/OpenVisionLab?include_prereleases)](https://github.com/Noah8218/OpenVisionLab/releases)
![Windows](https://img.shields.io/badge/platform-Windows%2010%2F11%20x64-0078D4) ![.NET](https://img.shields.io/badge/.NET-8-512BD4) [![License](https://img.shields.io/badge/license-Apache--2.0-green)](LICENSE) [![CI](https://github.com/Noah8218/OpenVisionLab/actions/workflows/ci.yml/badge.svg)](https://github.com/Noah8218/OpenVisionLab/actions/workflows/ci.yml)

[English](README.md) | 한국어

![Recipe 생성, 영상 불러오기와 검사 툴 티칭](docs/assets/demo/readme-workflows/recipe-teaching.gif)

## 3단계로 시작하기

1. [Releases](https://github.com/Noah8218/OpenVisionLab/releases/tag/v2.1.0-rc.3)에서 `win-x64` ZIP을 받습니다. [.NET 8 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/8.0)가 필요합니다.
2. 새 폴더에 ZIP을 모두 풀고 `OpenVisionLab.exe`를 실행합니다.
3. Sample Catalog에서 공개 샘플을 열고 Preview 또는 Run을 실행합니다.

공개 ZIP은 `v2.1.0-rc.3`이며, 이 문서에서 보여 주는 소스의 작업 흐름보다 이전 버전입니다.
같은 작업 흐름을 사용하려면 저장소를 clone하고 솔루션을 빌드한 뒤 F5로 실행합니다. [소스 빌드](#소스에서-빌드)를 참고하세요.

현재 버전: `v2.2.0-dev.11` · [변경 이력](CHANGELOG.md)

## 지원 작업

- PropertyGrid에서 툴 파라미터, ROI와 판정 범위를 설정합니다.
- 툴을 Pipeline으로 연결하고 Step·파라미터·Layer 연결을 Recipe XML로 저장합니다.
- 원본·중간·출력 Layer와 검출 표시, 측정값, OK/NG 근거를 비교합니다.
- 같은 Recipe로 Good/Bad 샘플을 Batch inspection하고 결과를 확인합니다.
- LLM/XML 보조 기능은 선택 사항입니다. 티칭과 검사 실행에 필수 조건이 아닙니다.

## Recipe 구성과 확인

1. **Recipe Manager**에서 `Shape Count`를 만들고 **Open work with selected inspection**을 선택한 뒤 영상을 엽니다.
2. **Threshold**를 열고 **Run on change**를 끕니다. 임계값을 `150`으로 설정한 뒤 **Run tool (F8)**을 누릅니다. 원본과 이진화 영상을 비교하고 **Add to pipeline → Add at end**로 추가합니다.
3. **Contour**를 열고 입력이 `Threshold_1`인지 확인합니다. **Use threshold**를 끄고 다섯 형상을 포함하도록 검사 ROI를 그린 뒤 **Apply**를 선택하거나 **Enter**를 누릅니다.
4. **Acceptance**를 켜고 **Detection count**를 선택합니다. **Lower**와 **Upper**를 체크해 모두 `5`로 설정한 뒤 **F8**을 누릅니다. 형상 `5`개가 검출됐는지 확인하고 **Ctrl+Enter**로 Step을 추가합니다.
5. **F5**로 Pipeline을 실행합니다. **Results**에서 결과를 확인하고 한 패널·두 패널·네 패널 뷰어에서 원본·중간·출력 Layer를 비교합니다.
6. **Ctrl+S**로 저장합니다. 다른 Recipe를 선택했다가 돌아와 저장한 Step·파라미터·ROI·Layer 연결을 다시 불러옵니다.
7. Good/Bad 영상을 추가하고 같은 Recipe로 **Batch inspection**을 실행합니다. 각 영상의 측정값과 OK/NG 결과를 확인합니다.

**Run on change**가 꺼져 있으면 파라미터를 바꿔도 툴을 실행하기 전까지 결과는 대기 상태입니다.
Recipe를 불러올 때는 검사 실행 없이 설정만 복원합니다. 저장과 실행은 별도 동작입니다.

활성 Step에 판정 조건이 없으면 실행을 마쳐도 OK/NG 판정이 없을 수 있습니다.
실행 오류는 NG와 구분합니다. 입력이나 파라미터가 바뀌면 보관된 결과는 이전 결과로 표시합니다.
현재 설정의 결과를 보려면 다시 실행합니다. Batch inspection은 모든 중간 영상을 보관하지 않습니다.
중간 영상이 필요하면 해당 영상을 선택해 실행합니다.

저장한 `Shape Count` Recipe를 원본과 형상 누락 영상에 적용합니다.
**Good**과 **Bad** 라벨을 지정한 뒤 **Batch inspection**을 실행합니다.
위의 ROI와 개수 조건에서 원본은 **검출 5개 / OK**, 형상 누락 영상은 **검출 2개 / NG**입니다.
일괄 검사 결과는 **OK 1장·NG 1장·오류 0장**입니다.

![Recipe 재로드와 Good/Bad 검사 결과 비교](docs/assets/demo/readme-workflows/recipe-validation.gif)

Good/Bad 라벨은 기대 분류를, OK/NG는 검사 결과를 나타냅니다.
영상을 선택해 원본·처리 영상·오버레이·판정 범위를 비교합니다.
**Page Up**을 누르면 이전 영상으로 돌아갑니다.

## 한 영상에서 모든 검사 단계 확인

![Filter, Threshold, Morphology, Blob, Contour 결과 비교](docs/assets/demo/readme-workflows/inspection-stages.png)

네 패널 뷰어에서 원본·필터·이진화·Morphology 출력 영상을 비교합니다.
공개 타공판 Recipe는 `Filter -> Threshold -> Morphology`의 출력 Layer를 Blob과 Contour에 각각 연결합니다.
두 단계 모두 판정 조건을 만족한 구멍 34개를 검출하며 **Results**에 표시합니다.
같은 면적 조건으로 영상 전체를 둘러싼 배경 윤곽선은 제외합니다.

[예제 Pipeline](docs/samples/public/product/Product_Field_PerforatedPlate_Inspection.pipeline.xml)을 열거나
Sample Catalog에서 `Product_Field_PerforatedPlate_HoleArray`를 선택합니다.

중간 영상을 비교하면서 Step별 파라미터를 조정하고 Pipeline Review에서 최종 검출 표시와 측정값을 확인합니다.
Preview와 Run은 사용자가 실행을 요청할 때 동작합니다.

## 툴 설정과 결과

메인 작업 공간에서 Matching, Line, Blob, Contour의 설정과 결과를 차례로 확인하는 영상입니다.

![툴 설정과 결과 확인](docs/assets/demo/readme-workflows/tool-settings.gif)

[Tool View MP4 보기](docs/assets/demo/readme-workflows/tool-settings.mp4)

- Matching: 불러온 템플릿과 매칭 점수, 검출 위치를 확인합니다.
- Line: Edge·Distance·Intersection 모드를 선택합니다. 영상에서는 핀 간 거리를 측정합니다.
- Blob: 입자 검출 결과와 개수 판정 범위를 확인합니다.
- Contour: 검사 ROI와 개수 판정 범위를 설정하고 검출된 형상을 확인합니다.

선택한 툴은 **Run step (F8)**, 전체 검사 순서는 **Run pipeline (F5)**로 실행합니다.

## 연속 처리

전처리와 검출 툴을 Pipeline으로 연결합니다. 아래 영상에서는 세 Step으로 구성된 두 Pipeline의
중간 영상과 결과를 비교하고 Morphology의 연산과 커널 설정을 확인합니다.

- `Filter -> Threshold -> Contour`
- `Threshold -> Morphology -> Contour`

![전처리 Step 연결과 결과 비교](docs/assets/demo/readme-workflows/preprocessing-chains.gif)

[연속 처리 MP4 보기](docs/assets/demo/readme-workflows/preprocessing-chains.mp4)

Pipeline Review에서 Step을 선택하면 해당 중간 영상을 확인할 수 있습니다.
마지막 Step에서는 검출한 객체와 검출 표시를 확인합니다.
개별 툴은 **F8**, Pipeline은 **F5**로 실행합니다.
**Run on change**가 있는 툴은 파라미터가 바뀔 때도 실행할 수 있습니다.

다음 예제는 `Filter → Threshold → Contour`와 `Threshold → Blob`을 연결합니다.
입출력 영상을 비교한 뒤 검출 개수와 판정 범위를 확인합니다.

![전처리와 입자 개수 검사](docs/assets/demo/readme-workflows/sample-inspections.gif)

## 주요 기능

| 영역 | 설명 |
| --- | --- |
| Main Workspace | 영상 불러오기, Layer 관리, 확대·축소, 이동과 픽셀 값 확인 |
| Layer Docking | 탭 또는 분할 보기에서 원본·중간·최종 영상 비교 |
| Tool View | Threshold, Filter, Morphology, Blob, Contour, Matching, FeatureMatching, EdgeBasedMatching, Line/Length, Mean 등 툴 설정 |
| PropertyGrid | 공통 속성 편집 화면에서 툴 파라미터 설정 |
| Preview / Run | 툴 또는 전체 Pipeline 실행. 지원하는 툴에서는 Run on change로 파라미터 변경에 따른 실행 제어 |
| Pipeline / Recipe | 툴을 순서 있는 Step으로 연결하고 XML로 저장 |
| Pipeline Review | Step별 입출력·측정값·검출 표시·로그·OK/NG 근거 확인 |
| Sample Catalog | 샘플 영상·기본 Pipeline·기대 결과·Good/Bad 쌍 조회 |
| Good/Bad Pair | 같은 측정값으로 정상·불량 샘플 비교 |
| Learn Mode | Matching, Blob, Contour, Threshold, Mean, FeatureMatching, EdgeBasedMatching, Line 안내 열기 |
| 언어 | 한국어와 영어 전환 |

Pipeline Review에서는 연결과 분기, 보관된 결과 영상, 판정 범위와 측정값, 조정할 파라미터도 확인합니다.
측정값에는 `ResultCount`, `AreaMax`, `ScoreMax`, `LineAngleAvg`가 있습니다.
[작업 공간 안내](docs/runbooks/BUILD.md#workspace-reference)와
[Pipeline Review 화면](docs/runbooks/DEVELOPMENT.md#pipeline-review)을 참고하세요.

## 공개 샘플

[공개 샘플](docs/samples/public/README.md)은 영상·권장 Pipeline·측정값·Good/Bad 기대값으로 구성됩니다.
[공개 샘플 목록](docs/samples/OpenVisionLab.PublicSampleCatalog.csv)과
[제품 샘플 목록](docs/samples/OpenVisionLab.ProductSampleCatalog.csv)에서 찾을 수 있습니다.
루트 `Sample/` 폴더는 로컬 또는 서드파티 데이터용이며 공개 샘플 배포에 포함되지 않습니다.

![공개 샘플 목록](docs/assets/demo/readme-workflows/sample-catalog.png)

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

## 소스에서 빌드

Windows 10/11 x64와 **.NET desktop development** 워크로드가 설치된 Visual Studio 2022 17.8+가 필요합니다.
Git이 PATH에 있어야 하며 최초 NuGet 복원에는 인터넷 연결이 필요합니다.

1. `https://github.com/Noah8218/OpenVisionLab.git`을 짧은 경로에 clone합니다.
2. `OpenVisionLab.sln`을 열고 **OpenVisionLab**을 시작 프로젝트로 지정합니다.
3. **Debug / Any CPU**를 선택하고 솔루션을 빌드한 뒤 **F5**로 실행합니다.

[전체 설치 조건, 명령줄 빌드와 Windows Sandbox 안내](docs/runbooks/BUILD.md).

## 문서

**Guide**를 선택하면 현재 UI 언어의 오프라인 사용 설명서가 열립니다.
작업 절차와 17개 툴별 설명에는 화면 사진, 번호별 설명, 조작 순서와 확인할 결과가 있습니다.

[사용 설명서](docs/manual/README.md) · [튜토리얼](docs/learn/OPENVISIONLAB_TUTORIAL.md) · [Learn Mode](docs/learn/README.md)
[빌드](docs/runbooks/BUILD.md) · [개발·검증](docs/runbooks/DEVELOPMENT.md) · [변경 이력](CHANGELOG.md) · [문서 목차](docs/README.md)

## 현재 상태와 한계

소스 버전은 `v2.2.0-dev.11`이며, 공개 ZIP은 이전 프리릴리스 `v2.1.0-rc.3`입니다.
LLM/XML 보조 기능은 선택 사항이며 검사 실행에 포함되지 않습니다.
실제 검사에 사용하기 전에는 해당 제품의 영상으로 Recipe를 검증해야 합니다.
UI 변경은 해당 빌드에서 확인해야 합니다. 기존 영상은 이후 변경의 동작 근거가 아닙니다.

## OpenVisionLab 프로젝트

| 프로젝트 | 하는 일 |
| --- | --- |
| [2D Workbench](https://github.com/Noah8218/OpenVisionLab) | 규칙 기반 2D 검사 Recipe |
| [3D Studio](https://github.com/Noah8218/OpenVisionLab-3D-Studio) | 3D 데이터 티칭, Thickness와 Warpage 측정 |
| [Labeling Studio](https://github.com/Noah8218/OpenVisionLab-Labeling-Studio) | 라벨링, 학습과 모델 비교 |
| [Machine Studio](https://github.com/Noah8218/OpenVisionLab-Machine-Studio) | 실물 장비 준비 전 시뮬레이션 |
| [Vision SDK](https://github.com/Noah8218/OpenVisionLab-Vision-SDK) | 공통 C# 검사 라이브러리 |

## 라이선스

OpenVisionLab은 [Apache License 2.0](LICENSE)으로 배포합니다.

```text
This project includes software developed by 최노아(Noah-Choi).
Copyright (c) 2026 최노아(Noah-Choi).
```

라이선스 조건에 따라 상업적으로 이용하거나 수정·재배포할 수 있습니다.
소프트웨어의 복사본 또는 상당 부분에는 `LICENSE`, [NOTICE](NOTICE),
저작권과 저작자 표시 고지를 유지해야 합니다.
