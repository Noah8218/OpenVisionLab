# Locator-relative Blob Intent Skill v1

상태: **구현된 계약/초안 생성기 + 증거 패킷 검증기 (pilot)**
Skill ID: `locator-relative-blob-v1`
범위 재개: 2026-08-27, 사용자가 LLM maintenance mode를 명시적으로 다시 열고 구현을 승인함

## 1. 목적

이 스킬은 이동·회전·배율 변화가 있는 이미지에서 작업자가 검토한 locator를 기준으로 동일한 기준좌표 Blob ROI를 검사하는 좁은 v1 경로다. LLM은 이미지를 직접 판단하거나 좌표를 발명하지 않는다. LLM이 선택할 수 있는 것은 해시가 검증된 증거 패킷 안의 `CandidateId`뿐이며, 좌표와 ROI는 작업자가 검토한 Plan과 결정적 컴파일러가 소유한다.

제품의 기본 동작은 여전히 rule-based OpenVisionLab workbench이며, LLM은 선택적인 XML authoring aid다. 이 스킬을 사용하지 않아도 Tool View, Pipeline, Preview/Run, Recipe, Review를 사용할 수 있어야 한다.

## 2. 고정된 실행 그래프

```text
Main image
  -> Matching (NUM_MATCH=2, ambiguity gate)
  -> Matching (NUM_MATCH=1, publish LocatorFrame pose)
  -> RotateScale (USE_FIXTURE_FRAME=true, NormalizeImage)
  -> Threshold (DeviceAligned -> AlignedInspectionBinary)
  -> Blob (fixed reference-coordinate ROI -> LocatorRelativeBlob)
```

각 Step은 활성화 상태, 이름, ToolType, 입력/출력 레이어, 파라미터 키/값, acceptance 상태까지 계획과 일치해야 한다. `Matching` fixture producer와 `RotateScale` consumer는 같은 `Main` source를 사용하고, normalization Step에는 ROI/mask를 사용하지 않는다. Blob ROI는 `LocatorFrame` 기준의 하나의 axis-aligned `x,y,w,h`다. v1은 `USE_MULTI_ROI=true`, mask, per-image ROI, perspective/homography, physical calibration을 허용하지 않는다.

## 3. 작업자 입력

| 입력 | 계약 |
| --- | --- |
| locator template | 실제로 존재하는 cropped template 파일; XML의 `TemplatePath`와 `PATTERN_PATH`가 같은 파일을 가리킴 |
| search ROI | reference image 안의 하나의 `x,y,w,h` |
| reference pose | `x,y,angle,scale,imageWidth,imageHeight`; 중심은 reference image 안에 있고 scale/dimensions는 양수 |
| fixed Blob ROI | reference image 안의 하나의 `x,y,w,h`; 실행 중 이동시키지 않음 |
| locator gates | `SCORE_MIN`, `ScoreMargin`, angle min/max, scale-ratio min/max, minimum valid-pixel ratio |
| Blob parameters | `Threshold` 0..255, positive `MIN_AREA..MAX_AREA` |
| ResultCount | Min/Max가 같은 경우에만 작업자 소유 exact gate로 materialize; 다르면 measurement-only |

LLM은 위 값이 비어 있을 때 값을 추정하지 않는다. 현재 Guided Setup은 기존 Hybrid locator 입력과 Blob 입력을 재사용하며, 새 템플릿을 선택하면 두 설정 패널을 함께 표시한다.

## 4. 판단과 증거 경계

- `NO EVIDENCE, NO COORDINATE`: 증거 패킷이 없으면 `TryCompile`이 실패한다.
- 후보 ID는 packet 안에서 unique해야 하며, selected ID는 accepted 후보 하나와 정확히 일치해야 한다.
- `ScoreMargin`은 실제로 보존된 locator 후보가 2개 이상일 때만 관찰된 경쟁 증거로 취급한다. 후보가 1개뿐이면 숫자 metric이 있어도 missing-second 상태로 보고 packet export/validation을 거부한다.
- 후보의 frame은 `LocatorFrame`이어야 하고, source/template/preview overlay의 SHA-256이 현재 파일과 같아야 한다.
- accepted 후보가 둘 이상이면 ambiguity로 fail closed 한다.
- source/template/overlay 파일 누락, unknown ID, wrong frame, duplicate native index, non-finite geometry, hash mismatch는 모두 실패한다.
- Pipeline 초안 검증은 fixed plan과 일치하는지 확인하지만, packet-aware compile과 실제 explicit Run은 별도 단계다. 검증 보고서는 packet/overlay가 없으면 `WAIT`를 표시한다.
- threshold/area/ResultCount gate는 operator-owned 값이다. LLM이 image별로 바꾸거나 tolerance를 자동 학습하지 않는다.

## 5. Evidence packet v1.1

JSON schema identifier는 `locator-relative-blob-evidence-v1.1`, plan schema는 `locator-relative-blob-plan-v1`이다. skill ID와 실행 그래프는 `locator-relative-blob-v1`로 유지하며, v1.1은 두 번째 retained candidate provenance를 명시적으로 요구하는 packet 계약 revision이다. 필수 top-level 필드는 다음과 같다.

```json
{
  "schemaVersion": "locator-relative-blob-evidence-v1.1",
  "planSchemaVersion": "locator-relative-blob-plan-v1",
  "skillId": "locator-relative-blob-v1",
  "sourceImagePath": "...",
  "sourceImageSha256": "64 hex chars",
  "locatorTemplatePath": "...",
  "locatorTemplateSha256": "64 hex chars",
  "previewOverlayPath": "...",
  "previewOverlaySha256": "64 hex chars",
  "sourceImageWidth": 572,
  "sourceImageHeight": 420,
  "coordinateFrame": "LocatorFrame",
  "producer": "...",
  "producerVersion": "...",
  "selectedCandidateId": "locator-0",
  "candidates": [
    {
      "candidateId": "locator-0",
      "nativeIndex": 0,
      "accepted": true,
      "centerX": 120,
      "centerY": 100,
      "angle": 0,
      "scale": 1,
      "boundsX": 100,
      "boundsY": 80,
      "boundsWidth": 40,
      "boundsHeight": 40,
      "score": 0.95,
      "scoreMargin": 12,
      "coordinateFrame": "LocatorFrame",
      "overlayPath": "...",
      "overlaySha256": "64 hex chars"
    },
    {
      "candidateId": "locator-1",
      "nativeIndex": 1,
      "accepted": false,
      "centerX": 300,
      "centerY": 200,
      "angle": 0,
      "scale": 1,
      "boundsX": 280,
      "boundsY": 180,
      "boundsWidth": 40,
      "boundsHeight": 40,
      "score": 0.83,
      "scoreMargin": 12,
      "coordinateFrame": "LocatorFrame",
      "overlayPath": "...",
      "overlaySha256": "64 hex chars"
    }
  ]
}
```

`OpenVisionRecipeLocatorRelativeBlobEvidencePacket.TrySave/TryLoad/TryValidate`가 이 구조와 파일 해시를 확인한다. packet에는 최소 2개의 retained locator candidate가 있어야 하며, 그중 정확히 하나만 accepted여야 한다. packet은 실행 자체를 수행하지 않으며, `TryCompile`은 유효한 packet과 reviewed Plan을 받아 동일한 결정적 5-Step Pipeline을 생성한다.

### 5.1 현재 런타임 export 경계

`OpenVisionRecipeLocatorRelativeBlobEvidenceExporter.TryExport`는 이미 명시적으로 실행된
`VisionPipelineRunResult`에서 `NUM_MATCH=2` locator Step의 보존된
`VisionPipelineMatchResultEvidence`만 읽는다. exporter가 새 실행을 시작하거나 좌표를
추정하지 않는다. 현재 source `Mat`와 public path/template의 SHA-256을 packet에 기록하고,
동일 실행 결과를 `VisionPipelineRunReportImageRenderer`로 그린 locator overlay를 저장한 뒤
동일 실행에서 2개 이상의 후보가 보존된 경우에만 packet을 다시 로드·검증할 수 있게 한다.
후보가 1개뿐이면 runtime `ScoreMargin`을 실제 경쟁 margin으로 승격하지 않고
`second candidate is missing`으로 export를 fail closed 한다. 이후 `TryCompile`과 별도의
명시적 `Run` 호출이 필요하며, packet 생성 성공 자체는 qualification이나 operator
acceptance가 아니다.

### 5.2 현재 operator review 흐름

Recipe Manager의 Guided Setup에서 `Locator-relative Blob (Evidence constrained)`를
선택하면 `Evidence Packet` 검토 패널이 함께 표시된다.

1. `Packet 로드`로 `.packet.json`을 선택한다.
2. 화면은 packet 전체의 source/template/overlay SHA-256, `LocatorFrame`, 선택된
   `CandidateId`, 보존된 후보 목록을 검증한 뒤 review text와 current-run overlay를
   표시한다. 누락·변조·unknown ID·ambiguity는 로드 실패로 남는다.
3. `검증·Compile`은 packet의 `CandidateId`와 현재 Guided Setup Plan을 대조해 같은
   결정적 5-Step XML 초안만 준비한다. 좌표를 읽어 쓰거나 LLM 응답을 적용하지 않는다.
4. XML `검증`, `가져오기`, 기존 `명시적 Run`은 각각 별도 작업이다. Packet 로드/Compile은
   Preview/Run, 레이어 생성·삭제·선택, active layer 변경, Pipeline routing 변경을
   수행하지 않는다.

Packet review 상태는 recipe나 workspace에 자동 저장하지 않는 일회성 검토 상태다. 현재
Guided Setup 값 또는 XML이 바뀌면 Compile 상태는 `STALE`로 내려가며, 같은 packet을
현재 설정에 다시 대조해야 한다.

### 5.3 review decision handoff

native candidate packet을 사람이 검토할 수 있도록
`locator-relative-blob-review-decision-v1` 파일 계약도 제공한다. 구현은
`OpenVisionRecipeLocatorRelativeBlobReviewDecision`이며, 성공한 native
candidate diagnostic은 packet 옆에 `review-decision.template.json`을
생성한다. 이 파일은 항상 다음 상태로 시작한다.

```text
decision=PENDING
visualCorrespondence=NOT_REVIEWED
reviewer=""
reviewedUtc=""
```

결정 파일은 packet 파일 자체의 SHA-256, source/template/overlay SHA-256,
현재 Plan의 결정적 fingerprint, packet의 retained `CandidateId`, fixed
inspection ROI, Threshold, Blob area 범위, optional exact ResultCount를 함께
보존한다. `TryValidateAgainst`는 packet 또는 Plan이 바뀌면 stale로
fail-closed 한다. `APPROVED`를 기록하려면 packet의 수치상 accepted candidate,
`PASS` visual correspondence, reviewer/UTC/note, 현재 ROI/parameter 정의가
모두 일치해야 한다. `REJECTED`와 `REPLACEMENT_REQUESTED`는 승인 상태가
아니며 사유를 남기는 종결 상태다.

이 결정 파일은 현재 WPF `Packet 로드`/`검증·Compile` 경로에 자동 적용되지
않는다. 현재 Compile은 여전히 XML 초안만 준비한다. 따라서 이 단계는
operator 입력을 자동으로 승인하지 않으며, Recipe promotion이
`APPROVED`만 소비하도록 하는 UI/workflow 연결은 별도 작업이다. 결정 파일의
`APPROVED`도 Train/Validation/Held-out qualification이나 release 승인을
뜻하지 않는다.

## 6. 초안/검증 상태

1. `MISSING`: template, ROI, pose, gate 또는 Blob 값이 없음/잘못됨.
2. `LOCATION GATED / MEASURE READY`: locator/normalization/Blob 구조가 준비됨.
3. `MEASURE ONLY / NOT JUDGED`: exact ResultCount gate가 없음.
4. `WAIT - evidence`: current hash-verified packet과 current-run overlay가 컴파일/Run 전에 필요함.
5. `JUDGED`: Min=Max인 작업자 소유 exact ResultCount gate가 Plan에 포함됨.
6. `QUALIFIED`: 이 문서의 범위를 넘어 Train/Validation/Held-out split과 중복 제거, 대표 Good/Bad/변환 overlay, operator review가 모두 통과한 경우에만 별도 보고서에서 사용한다.

## 7. 금지된 우회

- 전체 이미지를 Blob ROI로 사용해 locator를 우회하지 않는다.
- 후보의 숫자 좌표를 LLM 응답에서 직접 읽지 않는다.
- 샘플마다 ROI, threshold, area, count gate를 변경하지 않는다.
- missing/weak/ambiguous locator를 성공으로 간주하지 않는다.
- 실행 횟수나 ResultCount만으로 물리적 의미/정확성을 주장하지 않는다.
- provider API, consumer-web/browser automation, local VLM, autonomous correction loop를 제품 종속성으로 추가하지 않는다.

## 8. 현재 구현 위치

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/IntentSkills/OpenVisionRecipeLocatorRelativeBlobIntentSkill.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/IntentSkills/OpenVisionRecipeLocatorRelativeBlobEvidencePacket.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/IntentSkills/OpenVisionRecipeLocatorRelativeBlobReviewDecision.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/IntentSkills/OpenVisionRecipeLocatorRelativeBlobEvidenceExporter.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/IntentSkills/OpenVisionRecipeLlmPromptBuilder.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/IntentSkills/OpenVisionRecipeLlmTemplateDraftBuilder.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeLlmDraftValidationService.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeLlmDraftValidationRules.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface*.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.xaml`

## 9. 파일럿 한계와 다음 승인 조건

현재 public Matching -> NormalizeImage -> Threshold -> fixed reference-ROI Blob 경로는 Good/미검출 Bad 두 샘플의 bounded teaching pilot로만 사용할 수 있다. 이것은 최종 일반성이나 현장 qualification을 의미하지 않는다.

현재 런타임 pilot은 다음 evidence를 각 샘플별로 함께 보존한다.

- source image와 SHA-256
- 실제 Matching 후보와 `CandidateId`
- 현재 실행에서 렌더링한 locator overlay와 SHA-256
- 고정 ROI와 최종 Blob 결과를 표시한 annotated overlay
- evidence packet JSON
- 원래 실행과 packet 컴파일 후 explicit replay의 outcome/metrics 요약

다음 단계에는 사용자가 별도로 지정한 frozen corpus와 held-out split이 필요하다. 각 샘플의 source/XML/packet/overlay/hash/metrics를 묶고, Good 한 건·경계/shifted 한 건·실패 한 건의 현재-run drawing을 보존해야 한다. held-out 결과가 고정되기 전에는 skill을 `QUALIFIED` 또는 자동 판단기로 승격하지 않는다.

### 9.1 외부 샘플 corpus intake (2026-08-27 pilot)

`E:\라벨테스트`는 33개 dataset family, 153,988개 파일, 98,619개 raster image로
구성되어 있다. 현재 skill에 직접 연결한 범위는 그중
`EasyMatch_Die_Pad_500(1)\EasyMatch_Die_Pad_500`의 `source_file=Die Pad 1.bmp`
122행(train 82 / val 27 / test 13, role label OK 62 / NG 60)뿐이다. 이 subset은
README가 밝히는 synthetic labeling/tool/pipeline compatibility test이므로 실제
불량 판정 성능의 근거가 아니다.

재현 명령은 다음과 같다.

```text
dotnet "tools\\LocatorRelativeBlobSkillSmoke\\bin\\Any CPU\\Debug\\net8.0-windows7.0\\LocatorRelativeBlobSkillSmoke.dll" `
  --locator-relative-blob-corpus-pilot <native-batch-evidence> <new-D-drive-output>
```

intake는 native batch의 source hash/overlay를 먼저 확인한 뒤 각 행에 대해 현재
5-Step pipeline을 실행하고, `NUM_MATCH=2` locator 결과가 명시적 gate를 통과한
경우에만 current overlay와 source/template/overlay hash를 가진 packet을 만든다.
그 packet을 reload/compile하고, LLM 없이 생성된 XML을 다시 explicit replay하여
outcome을 비교한다. `RoleLabelOnly`는 CSV/manifest에만 보존하며 Blob
`ResultCount`나 runtime success로 OK/NG 의미를 대신하지 않는다.

기존 2026-08-27 pilot 결과는 122행 중 120 packet/replay 수용, 2행(locator 후보
없음) reject, 오류 0건이었다. 수용된 120행의 Blob `ResultCount`는 0~4로 분포하므로
이 결과는 고정 ROI와 replay 경계를 확인한 historical pilot evidence이지 결함 분류
gate가 아니다. 또한 수용된 행은 모두 보존 후보가 1개였고, 당시 metric 정의에서
second candidate가 없으면 `ScoreMargin`은 best-score와 0의 차이로 계산되었다.
따라서 당시 packet은 “두 번째 후보와의 실제 경쟁 margin이 관찰된” 증거와 동일하지
않다. 현재 계약은 이 경계를 반영해 후보 2개 미만의 packet을 validation/export에서
거부하고, corpus intake는 `MissingSecondCandidate` 상태로 별도 기록한다.

2026-08-29 계약 스모크는 v1.1 2개 후보 packet의 save/load/compile과 1개 후보로
축약한 packet의 fail-closed validation을 확인했다. 기존 v1 2026-08-27 artifact는
자동 변환하지 않고 historical evidence로 보존되며, 현재 validator의 관찰된 margin
증거로 재사용하지 않는다.

같은 날짜의 bounded native probe는 기존 public synthetic source에 두 번째
template instance를 추가해 실제 Matching 결과에 후보 2개(score `100`, `82.205`)와
`ScoreMargin=17.795`를 만들었고, v1.1 packet export/load/compile/replay까지 통과했다.
이 probe는 native provenance 경계를 검증하지만, 외부 corpus qualification이나
operator 승인된 Train/Validation/Held-out subset을 의미하지 않는다.

같은 날짜에 기존 P227 `Die Array / Die1.tif` 후보를 현재 native Matching의
외부 provenance diagnostic으로 재실행했다. 보존된 `die_array_003_ok.jpg`와
96x96 template에 `NUM_MATCH=2`를 적용한 결과 후보 2개(score `100` at
`192,176`, `96.276` at `56,464`)와 실제 `ScoreMargin=3.724`가 관찰되었다.
현재 margin 요구치 `10`을 통과하지 못해 `acceptance=false`로 fail closed 되었고,
반복 격자 후보라는 이전 시각 검토 경계 때문에 v1.1 packet은 의도적으로
export하지 않았다. 같은 runtime result를 실제
`OpenVisionRecipeLocatorRelativeBlobEvidenceExporter.TryExport`에 전달한
결과도 `false`와 `The runtime locator did not pass its explicit success and
ambiguity gates.`를 반환했고, packet/ exporter overlay path를 반환하지 않았으며
`evidence.packet.json`을 생성하지 않았다. `candidate-provenance.json`,
`probe-summary.txt`, current native overlay는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-external-die-array-provenance-20260829-r2`
에 보존한다. 이는 실제 두 번째 후보 보존과 ambiguity 차단 및 exporter
fail-closed 경계를 확인하는 negative evidence이며, locator 승인·margin
완화·qualification을 의미하지 않는다.

상세 intake manifest와 현재-run overlay는 다음에 보존한다.

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-corpus-inventory-20260827\inventory.json`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-corpus-pilot-20260827-die-pad1-final\summary.json`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-corpus-pilot-20260827-die-pad1-final\intake.csv`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-corpus-pilot-20260827-die-pad1-final\corpus-freeze-manifest.tsv`

이 intake는 pilot 범위만 완료하며 `PilotOnly=true`, `Qualification=false`를
유지한다. 다른 dataset family, multi-ROI/mask, per-image ROI, physical ground
truth, Recipe/Run History 자동 연결은 사용자 승인과 별도 계약 없이는 열지 않는다.

### 9.2 native 후보 ambiguity gate diagnostic (2026-08-29)

실제 외부 후보를 한 개에 하드코딩하지 않고 재현하려면 다음 명시적 진단
모드를 사용한다.

```text
dotnet "<built-smoke-dll>" \
  --locator-relative-blob-external-native-candidate \
  <source> <template> <inspection-roi> <reference-pose> <new-D-drive-output>
```

이 모드는 `REVIEW_ONLY`이며 입력된 source/template를 evidence 폴더에 복사하고,
full-source search ROI와 고정된 locator gate를 적용한다. `NUM_MATCH=2`의
실제 보존 후보, score/margin, pose, overlay, source hash/template hash를
기록하고 후보마다 pose-normalized patch, template 비교 panel, 50% blend를
보존한다. 두 후보와 margin `10`을 모두 통과한 경우에만 실제 v1.1 exporter를
호출하고 packet reload/compile/replay를 확인한다. 이 모드의 `PASS`는 진단
실행 완료를 뜻하며, operator 승인이나 qualification을 뜻하지 않는다.

2026-08-29 P227에 이미 기록된 13개 `SUGGESTED` 후보를 같은 조건으로
실행했다. IC Frame4만 canonical row에서 두 후보(`100`, `83.999`)와
`ScoreMargin=16.001`을 통과했다. 고정 IC Frame4 pilot 8행에서는 5행이
두 후보와 margin gate를 통과하고 packet/replay까지 일치했으며, 1행은
second candidate 미보존, 2행은 no-result였다. 다만 고정된
`Blob(ROI=368,368,96,96, Area=700..1300)`는 gate 통과 행에서도
`ResultCount=0`이어서 전체 locator-relative Blob graph 성공은 아니다.

현재 source/template/overlay 및 row별 provenance는 다음 report와 D-drive
evidence에 보존한다.

- `docs/reports/OPENVISIONLAB_LLM_LOCATOR_RELATIVE_BLOB_NATIVE_CANDIDATE_GATE_20260829.md`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-external-native-candidates-20260829`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-external-native-ic-frame4-pilot-20260829-r2`

IC Frame4의 top candidate는 현재 visual spot review에서 우측 하단 pin row와
대각 corner가 template과 대응하지만, second candidate는 반복 pin segment만
포함한다. 이는 `REVIEW_ONLY`의 시각 검토 기록이며 operator 승인으로
대체하지 않는다. 따라서 `Qualification=false`를 유지하고, 승인 이후에만
frozen Train/Validation/Held-out 범위와 downstream inspection ROI를 별도로
확정한다.
