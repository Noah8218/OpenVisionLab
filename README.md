# OpenVisionLab

OpenVisionLab은 **OpenCvSharp4 기반의 룰베이스 비전 검사 워크벤치**입니다.

이미지를 불러와서 Tool을 적용하는 데서 끝나는 프로그램이 아니라, 사용자가 검사 흐름을 직접 만들고 결과가 왜 OK/NG인지 확인할 수 있게 만드는 것이 목표입니다.

## 1분 요약

OpenVisionLab은 카메라/PLC 장비 플랫폼이 아니라, 이미지 샘플에서 rule-based 검사 recipe를 만들고 검증하는 데 집중한 WPF 워크벤치입니다.

- 핵심 흐름: 이미지 로드 -> Layer 확인 -> Tool 파라미터 조정 -> 명시적 Preview/Run -> Pipeline/Recipe 저장 -> Good/Bad 샘플 검증
- 주요 툴: Threshold, Blob, Contour, Matching, FeatureMatching, EdgeBasedMatching, Line/Length, Mean 등
- LLM 방향: 이미지를 한 번에 이해해 자동 완성하는 기능이 아니라, operator intent와 샘플 evidence를 기반으로 XML recipe 초안/검증/수정 루프를 돕는 보조 기능입니다.
- 현재 개발 초점: Recipe Manager, Pipeline Review, Sample Catalog, LLM XML authoring, layer/result 비교, 검증 smoke입니다.

## 설치/실행 방법

필수 전제:

- Windows 개발 환경
- .NET SDK 8.0.x. 이 저장소의 `global.json`은 `8.0.421`을 고정합니다.
- WPF/Windows Desktop 빌드가 가능한 SDK
- 저장소의 `dll/` 아래 vendored runtime DLL 유지

빌드 후 실행:

```powershell
cd C:\Git\OpenVisionLab_Dev
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
.\bin\Debug\OpenVisionLab.exe
```

![OpenVisionLab main workspace walkthrough](docs/assets/tutorial/annotated/main_workspace_callouts.png)

처음 화면은 위 번호 순서로 보면 됩니다.

1. `Tool List`: 왼쪽에서 사용할 Tool을 고릅니다.
2. `Layer Input`: 현재 기준으로 볼 Layer를 확인합니다.
3. `Image View`: 원본, 전처리, 결과 이미지를 직접 확인합니다.
4. `Run Status`: 선택한 Tool의 실행 상태와 예상 경로를 봅니다.
5. `Quick Actions`: 현재 Layer에서 바로 실행할 수 있는 주요 Tool과 Pipeline 추가 흐름을 확인합니다.

## 프로그램 방향

OpenVisionLab은 카메라, 조명, PLC를 직접 제어하는 장비 통합 프로그램이 아닙니다.
현재의 핵심은 **이미지 기반 검사 알고리즘을 학습하고, 튜닝하고, 검증하는 개발 환경**입니다.

룰베이스 비전 검사를 만들 때 처음 막히는 지점은 보통 다음과 같습니다.

- 지금 보고 있는 이미지가 원본인지, 전처리 결과인지 헷갈린다.
- Tool 결과가 다음 Tool의 입력으로 제대로 연결되었는지 확인하기 어렵다.
- Score, Count, Area 같은 값이 나와도 그 값으로 OK/NG를 설명하기 어렵다.
- Preview에서 본 결과와 저장된 Recipe 결과가 달라질 수 있다.
- 정상/불량 샘플을 바꿔가며 같은 기준으로 검증하는 과정이 번거롭다.

OpenVisionLab은 이 문제를 줄이기 위해 Layer, Tool, Pipeline, Recipe, Pipeline Review, Sample Catalog를 하나의 흐름으로 묶습니다.

```text
이미지 로드
  -> Layer 확인
  -> Tool 선택
  -> PropertyGrid에서 파라미터 조정
  -> Preview / Run
  -> Output Layer 확인
  -> Pipeline Step 추가
  -> Pipeline Review에서 OK/NG와 metric 확인
  -> Recipe XML 저장
  -> Good/Bad 샘플로 재검증
```

## 주요 기능

| 영역 | 내용 |
| --- | --- |
| Main Workspace | 이미지 로드, Layer 관리, zoom/pan, pixel/GV/RGB 상태 확인 |
| Layer Docking | 원본, 중간 결과, 최종 결과를 탭/분할 형태로 비교 |
| Tool View | Threshold, Filter, Morphology, Blob, Contour, Matching, FeatureMatching, Line, Mean 등 |
| PropertyGrid 기반 설정 | Tool 모델의 property를 그대로 편집 UI로 사용 |
| Preview / Run | 사용자가 명시적으로 실행할 때만 결과 생성 |
| Pipeline / Recipe | 여러 Tool을 Step으로 연결하고 XML로 저장 |
| Pipeline Review | Step별 input/output, metric, OK/NG, log, result image 확인 |
| Sample Catalog | 샘플 이미지와 baseline pipeline, expected metric 관리 |
| Good/Bad Pair | 정상/불량 샘플을 같은 metric으로 비교 |
| Learn Mode | Matching, Blob, Contour, Threshold, Mean, FeatureMatching, EdgeBasedMatching, Line 학습 문서 |
| Localization | 한국어/영어 전환 구조 |

## 샘플 데이터

처음에는 직접 이미지를 고르기보다 공개용 synthetic 샘플로 시작하는 것이 좋습니다.
공개 샘플은 `docs/samples/public` 아래에 있고, 각 샘플은 이미지, 추천 Pipeline, expected metric, Good/Bad 기준을 함께 제공합니다.

공개 배포 기준 catalog는 `docs/samples/OpenVisionLab.PublicSampleCatalog.csv`와
`docs/samples/OpenVisionLab.ProductSampleCatalog.csv`입니다.
루트 `Sample/` 폴더는 로컬/vendor 샘플 보관 영역이며 공개 배포 대상이 아닙니다.

![Public sample catalog walkthrough](docs/assets/tutorial/annotated/sample_catalog_public_callouts.png)

대표 흐름은 다음과 같습니다.

| 배우고 싶은 내용 | Good 샘플 | Bad 샘플 | 보는 metric |
| --- | --- | --- | --- |
| Matching으로 대상 찾기 | `Public_Matching_DiePad_Good` | `Public_Matching_DiePad_NoTarget_Bad` | `ResultCount`, `ScoreMax` |
| Blob으로 여러 입자 세기 | `Public_Blob_Particles_Good` | `Public_Blob_Particles_Sparse_Bad` | `ResultCount` |
| Contour로 모양 개수 보기 | `Public_Contour_Shapes_Good` | `Public_Contour_Shapes_Missing_Bad` | `ResultCount` |
| Threshold로 밝은 pad 분리 | `Public_Threshold_BandPads_Good` | `Public_Threshold_BandPads_Missing_Bad` | `ResultCount` |
| Mean으로 밝기 drift 보기 | `Public_Mean_Brightness_Good` | `Public_Mean_Brightness_Dark_Bad` | `MeanValueAvg` |
| FeatureMatching 점수 비교 | `Public_Feature_Card_Good` | `Public_Feature_Card_Wrong_Bad` | `ScoreMax`, `ResultCount` |
| EdgeBasedMatching 형상 비교 | `Public_Edge_Fiducial_Good` | `Public_Edge_Fiducial_Wrong_Bad` | `ScoreMax`, `ResultCount` |
| Line으로 거리 보기 | `Public_Line_Pins_Good` | `Public_Line_Pins_WidePin_Bad` | `DistanceMmAvg` |

## Tool 설계

OpenVisionLab의 알고리즘 Tool은 PropertyGrid 기반 구조를 유지합니다.

```text
Tool Property Model
  -> PropertyGrid SelectedObject
  -> Preview / Run
  -> VisionToolResult
  -> Metrics / Overlays / Logs
  -> Pipeline Step XML
```

이 구조를 유지하는 이유는 Tool이 늘어나도 설정 UI와 Recipe 저장 구조를 일관되게 유지하기 위해서입니다.
초보자는 Basic/Fast/Precise 같은 preset과 결과 해석 문구로 시작하고, 숙련자는 PropertyGrid에서 세부 파라미터를 직접 조정할 수 있습니다.

## Pipeline Review

Pipeline Review는 OpenVisionLab에서 가장 중요한 검증 화면 중 하나입니다.

여기서는 Step별로 다음 정보를 확인합니다.

- 현재 Step이 읽는 input layer
- 현재 Step이 쓰는 output layer
- 이전 Step과 연결된 chain인지, 의도적인 branch인지
- 결과 image와 overlay
- ResultCount, AreaMax, ScoreMax, LineAngleAvg 같은 metric
- acceptance 기준과 실제 측정값
- OK/NG 판단 이유
- 다음에 확인해야 할 파라미터 방향

![Pipeline review walkthrough](docs/assets/tutorial/annotated/pipeline_matching_review_callouts.png)

## 빌드

기본 빌드:

```powershell
cd C:\Git\OpenVisionLab_Dev
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
```

실행 파일은 Debug 기준으로 다음 위치에 생성됩니다.

```powershell
.\bin\Debug\OpenVisionLab.exe
```

## 검증

기본 release/readiness 검증:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --project "tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.PublicSampleCatalog.csv -OutputDir artifacts\public_sample_catalog
```

현재 소스 기준 WPF view screenshot smoke:

```powershell
dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\smoke\recipe_language_controls
```

최신 빌드 EXE 기준 direct smoke:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
.\bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs --output artifacts\smoke\recipe_manager_tabs
```

UI/UX를 수정했다면 현재 빌드 또는 현재 소스 view에서 before/after 캡처를 새로 남겨야 합니다. 이전 smoke artifact는 현재 UI 증거로 쓰지 않습니다.

## CI

GitHub Actions workflow는 `.github/workflows/ci.yml`에 있습니다.
현재 CI는 Windows에서 build, readiness, external reference policy, public sample asset policy를 확인하는 최소 게이트입니다.

WPF screenshot smoke와 실제 EXE UI smoke는 로컬 current-build 증거가 필요한 경우가 많아, CI 기본 게이트에는 넣지 않고 변경 범위에 맞춰 수동 실행합니다.

## 릴리즈 노트

- 변경 이력: [CHANGELOG.md](CHANGELOG.md)
- release/version gate: [docs/OPENVISIONLAB_RELEASE_VERSION_POLICY.md](docs/OPENVISIONLAB_RELEASE_VERSION_POLICY.md)

릴리즈 후보는 최소한 build, readiness, external refs, public sample assets, 필요한 UI/EXE smoke 증거를 함께 남겨야 합니다.

## 로드맵

현재 제품 방향과 다음 우선순위는 다음 문서가 기준입니다.

- [제품 목표와 메인 뷰](docs/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md)
- [제품 정체성과 로드맵](docs/OPENVISIONLAB_PRODUCT_IDENTITY_AND_ROADMAP.md)
- [현재 상태와 다음 작업](docs/OPENVISIONLAB_STATUS_AND_NEXT_STEPS.md)
- [Validation Suite / Result Archive 설계](docs/OPENVISIONLAB_VALIDATION_SUITE_RESULT_ARCHIVE_DESIGN_20260707.md)

현재 다음 큰 작업은 Recipe Manager 안에서 recipe-local Validation Suite와 Result Archive를 제공하는 것입니다.

## Known Limitations

- OpenVisionLab은 카메라, 조명, PLC, I/O, account, deployment 플랫폼이 아닙니다.
- LLM 기능은 one-shot 자동 영상처리가 아니라 guided XML authoring/correction-loop 보조 기능입니다.
- 실제 GPT/Gemini/Claude transcript evidence는 사용자가 제공한 API key 또는 수동 transcript가 있을 때만 확보할 수 있습니다.
- `Sample/` 루트는 로컬/vendor 샘플 영역이며 공개 배포 샘플 기준은 `docs/samples/public`과 catalog CSV입니다.
- Recipe-local Validation Suite / Result Archive는 설계 문서가 먼저 정리된 상태이며 구현은 다음 우선순위입니다.
- UI/UX 변경은 문서나 설명만으로 완료 처리하지 않고 current-build/current-source before/after 캡처가 필요합니다.

## 문서

- [튜토리얼 가이드](docs/OPENVISIONLAB_TUTORIAL.md)
- [Learn Mode 가이드](docs/learn/README.md)
- [안정 기능 계약](docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md)
- [현재 상태와 다음 작업](docs/OPENVISIONLAB_STATUS_AND_NEXT_STEPS.md)
- [공개용 샘플 카탈로그](docs/samples/OpenVisionLab.PublicSampleCatalog.csv)
- [제품형 샘플 카탈로그](docs/samples/OpenVisionLab.ProductSampleCatalog.csv)

## 라이선스와 저작권 고지

OpenVisionLab은 루트의 [LICENSE](LICENSE) 파일에 적힌 Apache License 2.0을 따릅니다.

이 프로젝트에는 최노아(Noah-Choi)가 개발한 소프트웨어가 포함되어 있습니다.

```text
This project includes software developed by 최노아(Noah-Choi).
Copyright (c) 2026 최노아(Noah-Choi).
```

상업적 사용, 수정, 재배포는 라이선스 조건에 따라 가능합니다.
다만 소프트웨어의 복사본이나 중요한 부분을 재배포할 때는 `LICENSE`, `NOTICE`, 저작권 표시, 저작자 표시를 제거하거나 가리지 말아야 합니다.

## 정리

OpenVisionLab의 방향은 명확합니다.
초보자는 샘플을 따라 하면서 룰베이스 비전의 흐름을 배우고, 숙련자는 같은 구조 안에서 실제 검사 Recipe를 빠르게 만들고 검증할 수 있는 프로그램입니다.
