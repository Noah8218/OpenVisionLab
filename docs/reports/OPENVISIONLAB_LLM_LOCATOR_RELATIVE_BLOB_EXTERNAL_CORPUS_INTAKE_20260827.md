# OpenVisionLab `locator-relative-blob-v1` 외부 샘플 corpus intake

날짜: 2026-08-27 KST
상태: **Complete — bounded pilot intake/replay 경로 구현 및 실행 완료; qualification은 불완전**

## 1. 요청과 범위

사용자가 `E:\라벨테스트`의 다양한 샘플을 활용해 기존 LLM maintenance-mode
skill을 발전시키도록 요청했다. 이 작업의 실행 가능한 목표는 다음으로 제한했다.

1. 외부 폴더의 실제 파일 수·metadata·label/split 구조를 재현 가능한 manifest로
   고정한다.
2. 현재 계약에 직접 맞는 하나의 반복 가능한 sample family를 선택한다.
3. native Matching 실행 결과를 임의 좌표가 아닌 source/template/overlay hash와
   `CandidateId`가 있는 evidence packet으로 연결한다.
4. packet을 deterministic XML로 compile하고, LLM 없이 같은 XML을 explicit replay해
   결과가 일치하는지 확인한다.
5. 부족한 점과 qualification 전제조건을 명시한다.

provider API, browser automation, per-image ROI 추정, autonomous threshold/area
tuning, 새로운 multi-ROI/mask 알고리즘, release/deploy는 이 작업의 범위가 아니다.

## 2. 제품·skill 경계

OpenVisionLab의 제품 정체성은 OpenCvSharp4 기반 rule-based recipe workbench이고,
LLM은 선택적인 XML authoring aid다. 이번 외부 corpus 연결도 다음 고정 그래프를
바꾸지 않는다.

```text
Matching(NUM_MATCH=2)
  -> Matching(NUM_MATCH=1, LocatorFrame)
  -> RotateScale(NormalizeImage)
  -> Threshold
  -> Blob(fixed reference-coordinate ROI)
```

LLM이 선택할 수 있는 값은 hash-verified packet 안의 `CandidateId`뿐이다. source,
template, overlay의 바이트와 SHA-256이 현재 파일과 다르면 packet은 닫힌 상태로
거부된다. OK/NG는 corpus의 `RoleLabelOnly` metadata로만 남기며, Blob 결과 개수나
실행 성공 여부로 label 의미를 대신하지 않는다.

## 3. `E:\라벨테스트` inventory 결과

현재 폴더를 재귀 조사한 결과는 다음 manifest에 고정했다.

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-corpus-inventory-20260827\inventory.json`

| 항목 | 값 |
| --- | ---: |
| Top-level dataset directory | 33 |
| 전체 파일 | 153,988 |
| Raster image (`jpg/png/bmp/tif` 등) | 98,619 |
| 전체 바이트 | 2,299,890,756 (약 2.30 GB decimal) |
| 주요 metadata | `csv`, `json`, `yaml`, `txt`, `md` |

대표 family별 규모와 계약 적합성은 다음과 같다.

| Family | 실제 inventory 규모 | 관찰된 metadata/의미 | `locator-relative-blob-v1` 직접 적합성 |
| --- | --- | --- | --- |
| `EasyMatch_*_500(1)` 6종 | 각 3,252 raster | OK/NG, COCO/YOLO, segmentation, anomaly split; synthetic test 안내 | Die Pad 1만 bounded pilot로 선택 |
| `EasyMatch_Labeling_Dataset_300` | 1,954 raster | 6 family × 50, synthetic NG, test-only | 고정 locator/packet 별도 필요 |
| `Industrial_4NewProducts_500_Each_2000_Full` | 9,011 raster | 4 product × 500, ROI/mask, split/sha256 | 제품별 template/pose 계약 필요 |
| `Industrial_Crosspoint_8Types_500_Each_4000_Full` | 20,000 raster | crosspoint 좌표·orientation·mask | point/cross 계약이므로 현재 Blob skill과 다름 |
| `Uploaded_8Sources_500_Each_4000_Full` | 8,008 raster | 8 source family | source별 locator 검토 필요 |
| multishape v3 10종 | 각 2,501 raster | shape/rotation/background/defect bbox와 pixel hash | multi-shape/segmentation 계약 필요 |
| `multishape_defect_labeling_dataset_v2_300` | 1,501 raster | shape/defect label과 mask | 현재 v1 범위 밖 |
| Pin/Screw/Rule/Slit/Target 8종 | 각 1,002 raster | OK/NG, product별 결함 의미 | gap/pin/target 전용 skill 후보 |
| `Card_Crosspoint_500_Full` | 2,501 raster | junction point/orientation/mask | crosspoint/geometry skill 후보 |
| circular/washer/industry samples | 1,203~1,403 또는 500 raster | 원형 결함·washer·multi-industry | 별도 contour/blob 판정 계약 필요 |

대부분의 폴더는 합성 데이터, labeling/tool 호환성 확인, segmentation/object
annotation 또는 제품별 geometry 검증을 위한 자료다. 따라서 “이미지가 많다”는
사실만으로 현재 skill의 일반성이나 현장 성능을 추론하지 않았다.

## 4. 선택한 첫 corpus와 이유

첫 연결 대상은 다음 하나로 고정했다.

`E:\라벨테스트\EasyMatch_Die_Pad_500(1)\EasyMatch_Die_Pad_500`

그중 `metadata.csv`의 `source_file=Die Pad 1.bmp` 122행을 사용했다.

| split | 행 수 | `RoleLabelOnly=OK` | `RoleLabelOnly=NG` |
| --- | ---: | ---: | ---: |
| train | 82 | 42 | 40 |
| val | 27 | 13 | 14 |
| test | 13 | 7 | 6 |
| 합계 | 122 | 62 | 60 |

이 선택은 이미지가 충분해서가 아니라, 이미 현재 Dev에 있는
`matching-die-pad-batch`가 source MD5, native result, score, center/box/angle/scale,
native overlay, explicit Preview contract를 보존하고 있기 때문이다. README와
`dataset_summary.json`은 이 Die Pad 자료를 synthetic procedural NG와
labeling/tool/pipeline compatibility test로 설명하므로 qualification corpus로
승격하지 않았다.

## 5. 구현한 skill 발전 경로

`tools/LocatorRelativeBlobSkillSmoke/Program.cs`에 다음 실행 모드를 추가했다.

```text
--locator-relative-blob-corpus-pilot <native-batch-evidence> <new-D-drive-output>
```

이 모드는 다음을 순서대로 수행한다.

1. native batch의 `summary.json`/`native_rows.csv`가 예상된 scenario, profile,
   122행, explicit Preview contract인지 확인한다.
2. template ROI, angle/scale gate, template SHA-256을 확인하고 template을 새
   D: artifact로 복사한다.
3. fixed search ROI `0,0,512,512`, inspection ROI `190,220,175,130`, reference
   pose `277.5,285,0,1,512,512`로 reviewed Plan을 만든다.
4. 각 source의 SHA-256과 512×512 decode를 확인하고 5-Step pipeline을 실제로
   실행한다.
5. locator success/ambiguity gate를 통과한 행만 현재 실행의 overlay와 packet을
   생성한다. packet은 source/template/overlay hash와 retained native candidate를
   보존한다.
6. packet reload → `CandidateId`-only compile → 같은 compiled XML explicit replay를
   수행하고 source run과 outcome을 비교한다.
7. 모든 행에 대해 `intake.csv`와 `corpus-freeze-manifest.tsv`를 쓴다. 거부 행도
   누락하지 않고 fail-closed 사유를 기록한다.

실행 중 Preview/Run이 없는 packet compile 경계와 실제 explicit pipeline replay를
분리했으며, 임의의 좌표·threshold·area·OK/NG 판정을 생성하지 않았다.

## 6. 현재 실행 evidence

native batch 입력:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-e-die-pad-20260827`

현재 corpus intake 결과:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-corpus-pilot-20260827-die-pad1-final`

| Check | 결과 |
| --- | --- |
| 입력 행 | 122 |
| packet 생성 | 120 |
| packet compiled replay | 120 |
| replay outcome 동일 | 120/120 |
| locator 후보 없음으로 reject | 2 (`train_NG_die_pad_026_ng`, `val_NG_die_pad_198_ng`) |
| 처리 중 예외 | 0 |
| locator overlay | 120/120 |
| Blob annotated overlay | 120/120 |
| qualification | `false` |

정확한 결과는 `summary.json`, `intake.csv`, `corpus-freeze-manifest.tsv`,
`report.txt`에 기록되어 있다. 생성된 각 row 폴더에는 source copy, native batch
overlay, current locator overlay, Blob overlay, packet, compiled XML이 함께 있다.

현재-run 시각 증거로 다음 세 이미지를 확인했다.

- `test_OK_die_pad_019_ok`의 locator overlay: best/second score margin과
  `LocatorFrame` 후보 표시.
- `test_NG_die_pad_013_ng`의 Blob overlay: fixed ROI 안 `Result Count=3`과
  accepted object 표시.
- `train_NG_die_pad_026_ng`의 native overlay: source는 native batch에서는
  결과가 있었지만 locator-relative 5-Step run에서는 후보가 없어 reject된 경계.

이 이미지들은 위 current artifact folder에서 직접 열어 확인했으며, 이전
스크린샷을 재사용하지 않았다.

## 7. 부족한 점과 수용한 리스크

1. **전체 33 family를 skill에 연결하지 않았다.** Die Pad 1만 현재 fixed template/
   ROI/pose 계약과 연결했으며, 나머지는 point/cross/segmentation/pin/shape 등
   별도 operator intent가 필요하다.
2. **2개 NG 행은 성공으로 보정하지 않았다.** `train_NG_die_pad_026_ng`와
   `val_NG_die_pad_198_ng`는 locator candidate가 없어 packet을 만들지 않았다.
   따라서 intake 명령의 결과는 `PARTIAL`/exit 1이며, 이는 데이터 손실이 아니라
   fail-closed 결과다.
3. **수용된 120행의 retained candidate count는 모두 1이다.** 현재
   `ScoreMargin` metric은 `NUM_MATCH=2`에서 second candidate가 없으면 best score와
   0의 차이로 계산된다. 따라서 숫자상 margin gate를 통과해도 실제 경쟁 후보와의
   margin을 관찰했다는 뜻은 아니다. qualification 전에는 second-candidate
   provenance 또는 missing-second 상태를 별도 검증해야 한다.
4. **Blob 결과는 판정이 아니다.** 수용된 행의 `ResultCount`가 0, 1, 2, 3, 4로
   나타났고 44행은 Blob 단계에서 runtime success가 아니었다. 이는 fixed ROI와
   replay 계약을 검증한 것이며 OK/NG defect truth를 증명하지 않는다.
5. **operator review/physical ground truth가 없다.** 현재 자료는 synthetic 및
   compatibility 목적이고, 독립 작업자의 overlay review와 현장 label audit가
   아직 없다.
6. **Recipe/Run History packet linkage는 아직 없다.** 현재는 tool smoke와 D:
   evidence artifact 수준이며, Recipe Manager에서 packet을 자동 보관·조회하는
   범위는 열지 않았다.

## 8. 다음 개발 우선순위와 승인 조건

1. Die Pad 1의 120 accepted packet과 2 rejected row를 작업자가 overlay 기준으로
   검토하고, 실제 사용할 Train/Validation/Held-out frozen subset과 label 의미를
   승인한다.
   `Recommended model: gpt-5.6-luna | Reasoning effort: low`
2. `NUM_MATCH=2`의 두 번째 후보가 실제로 보존되는 native case와 missing-second
   case를 분리해 metric/packet provenance를 확인한다. 그 결과에 따라 true
   ambiguity gate를 유지할지, missing-second를 별도 상태로 둘지 결정한다.
   `Recommended model: gpt-5.6-terra | Reasoning effort: medium`
3. operator가 다른 family의 구체적 검사 의도를 지정한 뒤에만 Card Crosspoint,
   Pin/Gap, multishape 중 하나를 새 skill 후보로 추가한다. family별 template,
   ROI, drawing, metric, failure contract를 먼저 정의한다.
   `Recommended model: gpt-5.6-sol | Reasoning effort: high`
4. 위 조건이 모두 충족될 때만 held-out qualification과 Recipe/Run History 연계를
   별도 승인한다. provider integration, per-image tuning, autonomous correction은
   계속 제외한다.
   `Recommended model: gpt-5.6-sol | Reasoning effort: high`

## 9. 재현 명령과 종료 기록

```powershell
dotnet build "tools\LocatorRelativeBlobSkillSmoke\LocatorRelativeBlobSkillSmoke.csproj" -c Debug -p:Platform="Any CPU" --no-restore

dotnet "tools\LocatorRelativeBlobSkillSmoke\bin\Any CPU\Debug\net8.0-windows7.0\LocatorRelativeBlobSkillSmoke.dll" `
  --locator-relative-blob-corpus-pilot `
  "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-e-die-pad-20260827" `
  "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-corpus-pilot-20260827-die-pad1-final"
```

```text
Status: Complete
Scope: Die Pad 1 bounded external corpus intake, packet creation, deterministic compile, and replay evidence
Acceptance criteria: 122-row integrity checked; 120 packets/replays matched; 2 unsafe/no-candidate rows preserved as reject; no processing exception
Verification: tool build passed with 0 errors; current-run intake produced summary/intake/freeze manifest; current overlays were opened and inspected
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-corpus-pilot-20260827-die-pad1-final
Boundary / next dependency: Qualification is not complete; frozen operator-reviewed labels, second-candidate provenance, and held-out approval remain required
```
