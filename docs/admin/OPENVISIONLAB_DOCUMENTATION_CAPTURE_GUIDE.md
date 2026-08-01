# OpenVisionLab 문서 캡처 가이드

Updated: 2026-07-02

README, 튜토리얼, Learn 문서처럼 공개 배포되는 UI 이미지는 항상 현재 빌드의 `OpenVisionLab.exe`에서 다시 캡처합니다.
예전 이미지를 그대로 쓰면 실제 UI와 설명이 어긋나므로, 문서를 수정하기 전 아래 절차를 먼저 실행합니다.

## 원칙

- 문서 이미지는 수동으로 잘라 붙인 예전 캡처를 재사용하지 않습니다.
- 기준 실행 파일은 현재 workspace에서 빌드한 `bin/Debug/OpenVisionLab.exe`입니다.
- 캡처에 보이는 샘플 이미지는 `docs/OPENVISIONLAB_PUBLIC_SAMPLE_ASSET_POLICY.md`를 만족해야 합니다.
- 상용 SDK 설치 폴더나 기존 `Sample` 폴더에서 가져온 이미지는 공개 README, 튜토리얼, Learn 문서 캡처에 사용하지 않습니다.
- 공개 README, 튜토리얼, Learn 문서에는 작성자 개인 목적, 제출 목적, 내부 사정, 작업자만 알면 되는 메모를 쓰지 않습니다. 문서는 제품 정체성, 기능, 사용 절차, 검증 근거만 설명합니다.
- 공개 README, 튜토리얼, Learn 문서에는 `portfolio`, `포트폴리오`, 채용/제출/개인 홍보 목적 같은 문구를 쓰지 않습니다. 그런 의도는 공개 사용자 문서가 아니라 작업 메모나 회복 문서에서만 다룹니다.
- 원본 캡처는 `docs/assets/tutorial/current`에 저장합니다.
- 번호와 화살표가 들어간 이미지는 `docs/assets/tutorial/annotated`에 생성합니다.
- README와 튜토리얼은 `annotated` 이미지를 우선 사용합니다.
- UI를 수정했다면 문서 캡처를 다시 만들고, 이전/이후 비교에는 최신 이미지를 사용합니다.

## 공개 샘플 생성

튜토리얼 캡처는 `docs/samples/public`의 synthetic 샘플을 사용합니다.
캡처 전에 아래 명령으로 샘플을 재생성합니다.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File "tools\GenerateOpenVisionSyntheticSamples.ps1"
```

생성되는 주요 파일:

- `docs/samples/public/Workspace_Inspection_Synthetic_OK.png`
- `docs/samples/public/Matching_DiePad_Synthetic_OK.png`
- `docs/samples/public/templates/Matching_DiePad_Synthetic_Template.png`
- `docs/samples/public/Blob_Particles_Synthetic_OK.png`
- `docs/samples/public/Contour_Shapes_Synthetic_OK.png`
- `docs/samples/public/Threshold_BandPads_Synthetic_OK.png`
- `docs/samples/public/Mean_Brightness_Synthetic_OK.png`
- `docs/samples/public/Feature_Card_Synthetic_OK.png`
- `docs/samples/public/templates/Feature_Card_Synthetic_Template.png`
- `docs/samples/public/Edge_Fiducial_Synthetic_OK.png`
- `docs/samples/public/templates/Edge_Fiducial_Synthetic_Template.png`
- `docs/samples/public/Line_Pins_Synthetic_OK.png`
- `docs/samples/public/OpenVisionLab.PublicSampleManifest.csv`

## 캡처 생성

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" `
  -p:OpenVisionLabEnableEmbeddedSmokeRunner=true

$process = Start-Process `
  -FilePath "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe" `
  -ArgumentList @(
    "--smoke",
    "tutorial-captures",
    "--output",
    "C:\Git\OpenVisionLab_Dev\artifacts\tutorial_current_exe_YYYYMMDD"
  ) `
  -PassThru `
  -Wait
if ($process.ExitCode -ne 0) {
  throw "tutorial-captures failed with exit code $($process.ExitCode)."
}
```

성공하면 output 폴더에 아래 파일이 생성되어야 합니다.

- `01_main_workspace_current.png`
- `02_pipeline_review_current.png`
- `03_layer_docking_current.png`
- `04_matching_tool_current.png`
- `matching_preview_actual_current.png`
- `05_blob_tool_current.png`
- `06_line_tool_current.png`
- `07_sample_catalog_public_current.png`
- `report.txt`

`report.txt`의 `Result`가 `PASS`인지 확인합니다.

## 문서 자산 반영

캡처가 성공하면 최신 캡처를 문서 자산 폴더로 복사합니다.

```powershell
$capture = "C:\Git\OpenVisionLab_Dev\artifacts\tutorial_current_exe_YYYYMMDD"
$current = "C:\Git\OpenVisionLab_Dev\docs\assets\tutorial\current"

New-Item -ItemType Directory -Force -Path $current | Out-Null
Copy-Item "$capture\01_main_workspace_current.png" "$current\main_workspace_current.png" -Force
Copy-Item "$capture\02_pipeline_review_current.png" "$current\pipeline_review_current.png" -Force
Copy-Item "$capture\03_layer_docking_current.png" "$current\layer_docking_current.png" -Force
Copy-Item "$capture\04_matching_tool_current.png" "$current\matching_tool_current.png" -Force
Copy-Item "$capture\matching_preview_actual_current.png" "$current\matching_preview_actual_current.png" -Force
Copy-Item "$capture\05_blob_tool_current.png" "$current\blob_tool_current.png" -Force
Copy-Item "$capture\06_line_tool_current.png" "$current\line_tool_current.png" -Force
Copy-Item "$capture\07_sample_catalog_public_current.png" "$current\sample_catalog_public_current.png" -Force
```

Learn 문서에 들어가는 알고리즘 결과 근거 이미지는 UI smoke 캡처가 아니라 현재 public sample catalog 실행 결과에서 가져옵니다.
아래 순서로 현재 코드 기준 결과를 만들고 `docs/assets/tutorial/current`에 동기화합니다.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File "tools\RunVisionSampleCatalog.ps1" `
  -CatalogPath "docs\samples\OpenVisionLab.PublicSampleCatalog.csv" `
  -OutputDir "artifacts\public_sample_catalog_YYYYMMDD_learn_evidence" `
  -SkipRestore

powershell -NoProfile -ExecutionPolicy Bypass -File "tools\RunVisionSampleCatalog.ps1" `
  -CatalogPath "docs\samples\OpenVisionLab.ProductSampleCatalog.csv" `
  -OutputDir "artifacts\product_sample_catalog_YYYYMMDD_learn_evidence" `
  -SkipRestore

powershell -NoProfile -ExecutionPolicy Bypass -File "tools\SyncPublicLearnEvidenceImages.ps1" `
  -CatalogArtifactDir "artifacts\public_sample_catalog_YYYYMMDD_learn_evidence" `
  -ProductCatalogArtifactDir "artifacts\product_sample_catalog_YYYYMMDD_learn_evidence"
```

동기화되는 주요 결과 이미지:

- `docs/assets/tutorial/current/public_matching_diepad_good_result.png`
- `docs/assets/tutorial/current/public_blob_particles_good_result.png`
- `docs/assets/tutorial/current/public_contour_shapes_good_result.png`
- `docs/assets/tutorial/current/public_threshold_bandpads_good_result.png`
- `docs/assets/tutorial/current/public_mean_brightness_good_result.png`
- `docs/assets/tutorial/current/public_feature_card_good_result.png`
- `docs/assets/tutorial/current/public_edge_fiducial_good_result.png`
- `docs/assets/tutorial/current/public_line_pins_good_result.png`
- `docs/assets/tutorial/annotated/product_sample_source_result_sheet.png`

그 다음 번호와 화살표가 들어간 이미지를 다시 생성합니다.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File "tools\BuildTutorialCalloutImages.ps1"
powershell -NoProfile -ExecutionPolicy Bypass -File "tools\BuildPortableTutorial.ps1"
```

생성 결과:

- `docs/assets/tutorial/annotated/main_workspace_callouts.png`
- `docs/assets/tutorial/annotated/layer_docking_callouts.png`
- `docs/assets/tutorial/annotated/tool_matching_form_callouts.png`
- `docs/assets/tutorial/annotated/tool_blob_form_callouts.png`
- `docs/assets/tutorial/annotated/tool_line_form_callouts.png`
- `docs/assets/tutorial/annotated/sample_catalog_public_callouts.png`
- `docs/assets/tutorial/annotated/public_matching_diepad_good_callouts.png`
- `docs/assets/tutorial/annotated/public_blob_particles_good_callouts.png`
- `docs/assets/tutorial/annotated/public_contour_shapes_good_callouts.png`
- `docs/assets/tutorial/annotated/public_threshold_bandpads_good_callouts.png`
- `docs/assets/tutorial/annotated/public_mean_brightness_good_callouts.png`
- `docs/assets/tutorial/annotated/public_feature_card_good_callouts.png`
- `docs/assets/tutorial/annotated/public_edge_fiducial_good_callouts.png`
- `docs/assets/tutorial/annotated/public_line_pins_good_callouts.png`
- `docs/assets/tutorial/annotated/pipeline_matching_review_callouts.png`
- `docs/learn/OPENVISIONLAB_TUTORIAL_PORTABLE.html`

## 작성 체크리스트

문서를 수정하기 전:

1. `tools\GenerateOpenVisionSyntheticSamples.ps1`로 public synthetic 샘플을 재생성합니다.
2. 현재 코드로 `dotnet build`를 통과했는지 확인합니다.
3. `tutorial-captures` smoke가 `PASS`인지 확인합니다.
4. `docs/assets/tutorial/current`가 최신 캡처로 갱신되었는지 확인합니다.
5. Learn 문서에 결과 근거 이미지를 넣는 경우 `RunVisionSampleCatalog.ps1`와 `SyncPublicLearnEvidenceImages.ps1`를 먼저 실행합니다.
6. `tools/BuildTutorialCalloutImages.ps1`를 실행해 `annotated` 이미지를 다시 만듭니다.
7. Matching 문서에 들어가는 실제 검출 이미지는 `matching_preview_actual_current.png`처럼 Preview/Run이 만든 overlay 결과만 사용합니다.
8. README와 튜토리얼의 번호 설명이 이미지 안의 라벨과 같은지 확인합니다.
9. 오래된 static 이미지가 다시 walkthrough로 들어가지 않았는지 확인합니다.

문서를 수정한 후:

1. Markdown 이미지 경로가 실제로 존재하는지 확인합니다.
2. HTML/portable 문서를 사용하는 경우 다시 생성합니다.
3. UI 변경 전/후 비교에는 현재 EXE에서 다시 찍은 이미지를 사용합니다.

## 사용하지 말아야 할 방식

- 예전 `docs/assets/tutorial/*.png`를 최신 walkthrough 이미지처럼 재사용
- 브라우저 이미지 뷰어에서 임의로 자른 수동 캡처만 사용
- UI를 수정했는데 README나 튜토리얼 이미지를 그대로 두는 방식
- 이미지 안 번호와 본문 설명이 서로 다른 상태로 문서 배포
- 기존 `Sample` 폴더나 상용 SDK 예제 이미지를 공개 문서 캡처에 노출

OpenVisionLab 문서는 실제 프로그램을 따라가기 위한 자료입니다.
따라서 문서 이미지는 항상 현재 실행 파일의 화면이어야 합니다.
