# OpenVisionLab LLM `locator-relative-blob-v1` Phase E 기록

날짜: 2026-08-27 KST
상태: **구현 범위 완료 / qualification은 불완전**

## 요청과 판단

사용자가 GPT Pro 분석 문서를 다시 읽고 LLM 설계를 반영해 진행하도록 명시적으로 승인했다. 이는 저장소의 “LLM/provider validation은 명시적 재개 전까지 닫힘” 규칙을 이 특정 skill에 한해 다시 연 것이다. 다른 provider, 브라우저 자동화, 반복 dataset tuning, camera/PLC/I/O 범위는 열지 않았다.

제품 정체성은 변하지 않는다. OpenVisionLab은 OpenCvSharp4 rule-based recipe workbench이고, LLM은 optional XML authoring aid다. 핵심 흐름은 operator-reviewed inputs -> Pipeline -> explicit Preview/Run -> result/drawing review이며, LLM은 좌표 검출기가 아니다.

## GPT 분석에서 반영한 설계

분석 문서의 Phase E 요구를 다음처럼 코드 계약으로 내렸다.

```text
deterministic candidate evidence
  -> hash-verified evidence packet
  -> CandidateId-only selection
  -> typed Plan validation
  -> deterministic 5-step compiler
  -> explicit Run boundary
  -> later frozen Good/Bad/transform/held-out review
```

`locator-relative-blob-v1`의 첫 대상은 기존 Hybrid locator 경로와 public `Matching -> NormalizeImage -> Threshold -> fixed reference-coordinate Blob ROI` 샘플을 결합한다. 새 알고리즘이나 per-image coordinate inference는 구현하지 않았다.

## 구현된 범위

1. `OpenVisionRecipeLocatorRelativeBlobIntentSkill`
   - strict typed Plan parser/validator
   - reference-bounded search/inspection ROI
   - score/margin/angle/scale/valid-pixel gate
   - Threshold 0..255 및 positive Blob area validation
   - optional exact ResultCount gate; otherwise measurement-only
   - deterministic `Matching(2) -> Matching(1 fixture) -> RotateScale NormalizeImage -> Threshold -> Blob` compiler
   - compiled pipeline exact step/route/parameter/acceptance replay validator
2. `OpenVisionRecipeLocatorRelativeBlobEvidencePacket`
   - schema/plan/skill/producer metadata
   - source/template/overlay file SHA-256 verification
   - unique CandidateId/native index and LocatorFrame checks
   - unknown selected ID, wrong frame, stale hash, duplicate/ambiguous accepted candidates fail closed
   - JSON save/load round trip
   - packet-aware compile boundary
3. Existing LLM maintenance path
   - Guided Setup catalog option: `Locator-relative Blob (Evidence constrained)`
   - prompt contract includes `NO EVIDENCE, NO COORDINATE`, CandidateId-only, fixed ROI, explicit Run
   - generic Blob branch is evaluated after the exact new template branch
   - LLM XML draft builder and strict intent validator understand the new five-step graph
   - existing Hybrid locator and Blob input panels are shown together for this template
   - Guided Setup now loads a hash-verified Evidence Packet, shows candidate/integrity review and the current-run overlay, and exposes a separate no-auto-run Compile action
   - Compile prepares the deterministic XML draft; XML validation, Import, and the existing explicit Run remain separate operator actions
4. Documentation
   - new reusable contract document
   - machine-readable route/index and current handoff entries are updated separately

## 파일럿 corpus 경계

현재 public bounded teaching fixture는 다음 두 샘플과 하나의 locator template다.

- `docs/samples/public/Fixture_Pad_Synthetic_Shifted_OK.png`
- `docs/samples/public/Fixture_Pad_Synthetic_Shifted_Missing_NG.png`
- `docs/samples/public/templates/Fixture_Locator_Synthetic_Template.png`

이 두 샘플은 구조·경로·실행 예시를 확인하는 pilot이다. 두 장만으로 held-out 일반성, 변환 안정성, 생산 qualification, 또는 물리적 결함 의미를 주장하지 않는다.

## 검증 결과

현재 턴에서 실제 실행한 검증은 다음과 같다.

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" --no-restore` — pass, 0 warning / 0 error.
- `dotnet run --project tools\LocatorRelativeBlobSkillSmoke\LocatorRelativeBlobSkillSmoke.csproj -c Debug --no-restore` — pass.
  - positive Plan parse, five-step order, measurement-only behavior
  - packet JSON save/load and SHA-256 round trip
  - evidence-aware compile
  - unknown CandidateId, source hash mismatch, wrong coordinate frame, ambiguous accepted candidates, tampered fixed ROI, and compile-without-evidence negative cases
  - artifact root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-v1-contract-20260827`
- `dotnet run --project tools\LocatorRelativeBlobSkillSmoke\LocatorRelativeBlobSkillSmoke.csproj -c Debug --no-restore -- --locator-relative-blob-runtime-pilot D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-runtime-pilot-20260827-final` — pass.
  - 현재 빌드의 실제 `VisionPipelineExecutionService`로 public Good와 Missing-Bad를 각각 실행
  - Good: locator `Matching` success/ambiguity acceptance, `Blob ResultCount=1`, runtime success
  - Missing-Bad: locator는 통과하지만 최종 Blob `ResultCount=0`, explicit runtime failure
  - 각 실행에서 보존된 Matching 후보를 current-run locator overlay로 렌더링하고, source/template/overlay SHA-256이 포함된 evidence packet을 export/load
  - packet의 `CandidateId`만 사용해 deterministic pipeline을 compile한 뒤 별도 explicit replay를 수행했으며 원 실행과 outcome이 일치
  - 각 sample 폴더에 실행 source copy, `pipeline.xml`, current locator overlay, fixed-ROI/Blob annotated overlay, result image, packet, metrics summary를 함께 보존
  - artifact root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-runtime-pilot-20260827-final`
  - freeze manifest는 `PilotOnly=true`, `Qualification=false`로 기록
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunUiScreenshotSmoke.ps1 -Targets wpf_shell_host_recipe_locator_relative_blob_guided_setup -OutputDir D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-ui-20260827-packet` — pass.
  - current source/build screenshot rendered the Evidence Packet panel, non-empty packet path, verified candidate review, overlay, and Compile status
  - screenshot check reported `check=OK`, `layout=0`, `text=0`, `internal=0`, `size=1600x900`
  - no Preview/Run or layer mutation occurred in the packet load/compile path

## 부족한 점과 수용한 리스크

- Evidence Packet은 현재 Guided Setup에서 선택·해시검증·candidate/overlay 검토·deterministic compile까지 가능하다. 다만 packet과 Recipe/Run History를 자동으로 연결하거나 packet Compile과 Import/Run을 하나의 복합 명령으로 합치지는 않았다.
- public corpus는 Good/미검출 Bad 두 장뿐이다. 회전·배율·경계·배경-only·template mismatch·중복 split·held-out 집합은 아직 qualification evidence가 아니다.
- 이번 턴의 WPF 검증은 현재 source/build의 1600x900 quiet screenshot과 control/layout/text checks까지 수행했다. actual-EXE theme/Wide/Compact/DPI/resize/mouse/keyboard matrix는 다시 실행하지 않았다.
- 이번 runtime pilot은 두 public teaching fixture에서의 현재 실행 drawing/metrics evidence를 제공하지만, 두 샘플만으로 알고리즘 일반성·현장 정확성·물리적 결함 의미를 증명하지 않는다.

따라서 결론은 “설계가 코드 계약과 수동 증거 패킷 경계까지 구현되었다”이지 “LLM skill이 현장 검증 완료되었다”가 아니다.

## 다음 단계 승인 조건

qualification을 계속하려면 사용자가 exact frozen corpus(Train/Validation/Held-out), sample labels, 그리고 현재-run overlay/metric 보존 범위를 지정해야 한다. 그 전에는 provider integration, per-image tuning, tolerance 자동화, release/deploy를 진행하지 않는다.

## 외부 샘플 corpus intake pilot (2026-08-27 추가)

사용자가 제공한 `E:\라벨테스트`를 재귀 inventory한 결과 33개 top-level dataset
directory, 153,988개 파일, 98,619개 raster image가 확인되었다. 전체 inventory는
다음에 보존한다.

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-corpus-inventory-20260827\inventory.json`

현재 v1 계약에 직접 연결한 대상은 `EasyMatch_Die_Pad_500(1)\EasyMatch_Die_Pad_500`
내 `source_file=Die Pad 1.bmp` 122행이다(train 82 / val 27 / test 13, role label
OK 62 / NG 60). 기존 `matching-die-pad-batch` native evidence를 입력으로 재사용하고,
새 `--locator-relative-blob-corpus-pilot` 모드가 source hash/512×512 decode,
5-Step runtime, current locator overlay, packet reload/compile, compiled replay를
수행한다. 모든 output은 D:에 기록하며 기존 E: 데이터와 원본 repository를 변경하지
않는다.

현재 실행 결과:

- 122행 중 120 packet 생성 및 120 compiled replay outcome 일치
- `train_NG_die_pad_026_ng`, `val_NG_die_pad_198_ng`는 locator 후보 없음으로
  packet 생성 없이 reject
- 처리 중 예외 0건; locator/Blob current overlay 각각 120개
- Blob `ResultCount`는 accepted rows에서 0~4로 분포하며, `RoleLabelOnly`와
  defect truth를 대신하지 않음
- `PilotOnly=true`, `Qualification=false`

상세 결과는 다음에 있다.

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-corpus-pilot-20260827-die-pad1-final\summary.json`

이 intake는 skill을 실제 external corpus에 연결하는 재현 경로를 추가했지만,
qualification은 진전시키지 않았다. 수용된 120행의 retained candidate count가
모두 1이고, 현재 `ScoreMargin` 정의는 second candidate가 없을 때 best-score와
0의 차이를 사용한다. 따라서 실제 경쟁 후보 margin과 동일하다고 해석하지 않으며,
다음 승인 조건에 second-candidate provenance/missing-second 상태 검증을 추가했다.
다른 dataset family, multi-ROI/mask, per-image ROI, physical ground truth,
Recipe/Run History 자동 연결은 별도 operator intent와 계약 없이는 열지 않는다.
