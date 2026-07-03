# OpenVisionLab Scenario Validation

Updated: 2026-06-16

이 문서는 OpenVisionLab을 실제 사용자 흐름으로 검증하기 위한 체크리스트입니다. 목표는 기능 존재 여부가 아니라 사용자가 검사 Recipe를 만들고, 검증하고, 저장하고, 다시 실행할 수 있는지 확인하는 것입니다.

## 1. Main Workspace

목표: 사용자가 현재 어떤 이미지를 보고 있고, 어떤 레이어가 검사 입력인지 즉시 이해해야 합니다.

검증 절차:

1. 프로그램을 실행한다.
2. Main 레이어에 샘플 이미지를 로드한다.
3. 새 레이어를 하나 만든다.
4. 레이어 콤보박스에서 Main과 새 레이어를 전환한다.
5. 오른쪽 레이어/결과 패널을 확인한다.
6. 하단 로그를 Summary/Detail 상태로 확인한다.

기대 결과:

- 상단 상태바에 활성 레이어, 입력 기준, 실행 흐름, 작업 상태가 표시된다.
- 오른쪽 레이어/결과 목록에 레이어 이름, 역할, 이미지 크기, 현재 표시 상태가 보인다.
- 이미지가 없을 때는 빈 화면이 아니라 명확한 empty state가 보인다.
- 로그는 기본 상태에서 너무 많은 내부 정보를 노출하지 않는다.

대표 검증:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 -Targets main_workspace
```

## 2. Tool Form Flow

목표: 각 Tool Form은 독립 테스트도 가능하고 Pipeline Step으로도 변환 가능해야 합니다.

검증 절차:

1. Main에 이미지를 로드한다.
2. Threshold Form을 연다.
3. Input Source를 Main으로 둔다.
4. Basic Threshold, Range Threshold, Adaptive Threshold를 각각 조정한다.
5. Output Result 레이어에 결과가 반영되는지 확인한다.
6. `Add Pipeline Step`으로 Step을 추가한다.
7. Contour, Blob, Line, Rotate/Scale 등 주요 Tool에서도 동일하게 입력/출력 레이어를 확인한다.

기대 결과:

- Tool Form에서 입력 레이어와 출력 레이어가 명확히 보인다.
- Preview는 지정된 Output Layer에만 반영된다.
- TrackBar 조정 중 깜빡임과 느린 반응이 사용을 방해하지 않아야 한다.
- Tool 결과는 ErrorCode, ResultStatus, Message, Metrics, Overlay를 반환한다.

대표 검증:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 -Targets threshold_form,tool_contour_form,tool_blob_form,tool_line_form,tool_rotate_scale_form
```

## 3. Pipeline Authoring

목표: 사용자가 Step을 추가할 때 기본은 이전 Step의 Output을 다음 Step의 Input으로 연결해야 합니다. Main으로 되돌아가는 Branch는 의도적이어야 합니다.

검증 절차:

1. Pipeline Form을 연다.
2. Threshold Step을 추가한다.
3. Morphology Step을 추가한다.
4. Contour Step을 추가한다.
5. 각 Step의 Input/Output pill을 확인한다.
6. 의도적으로 Contour Input을 Main으로 바꾸고 Branch 표시를 확인한다.
7. `Use Previous Output as Input`으로 다시 체인 연결한다.

기대 결과:

- 정상 체인은 `PREV OUT`으로 보인다.
- 첫 Step은 `SOURCE IMG`로 보인다.
- 의도적 Branch는 `BRANCH IN`으로 보이며 review 대상이 된다.
- Input pill을 클릭하면 Input image preview, Output pill을 클릭하면 Output image preview로 전환된다.
- Preview가 없으면 `Run Preview required`가 표시된다.

대표 검증:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 -Targets pipeline_form,pipeline_form_branch,pipeline_add_step_form,pipeline_add_step_branch_form
```

## 4. Preview And Publish

목표: Run Preview는 Pipeline Form 내부 검증이고, Publish Result는 Main Workspace를 명시적으로 갱신하는 동작이어야 합니다.

검증 절차:

1. Pipeline Form에서 샘플 Pipeline을 로드한다.
2. Run Preview를 실행한다.
3. Main Workspace가 자동으로 덮어써지지 않는지 확인한다.
4. Preview에서 Summary / Input / Output / Overlay를 전환한다.
5. Publish Result를 누른다.
6. Main Workspace에 결과 레이어가 생성되거나 갱신되는지 확인한다.

기대 결과:

- Preview 완료 전 Main Workspace는 변경되지 않는다.
- Preview 결과는 Pipeline Form 안에 남아 있다.
- Publish Result 이후에만 Main Workspace 레이어가 갱신된다.
- Publish All이 켜진 경우 모든 Output Layer가 명시적으로 반영된다.

대표 검증:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 -Targets pipeline_form_run_preview,pipeline_sample_open_preview,pipeline_sample_llm_open_preview
```

## 5. Save, Load, Restart

목표: Recipe는 XML로 저장/로드되어도 Step, Input/Output, Parameters, Acceptance가 유지되어야 합니다.

검증 절차:

1. Pipeline을 만든다.
2. Save Project를 실행한다.
3. Pipeline Form을 닫고 다시 연다.
4. Load Saved 또는 Import XML을 실행한다.
5. Step 수와 Input/Output 레이어를 확인한다.
6. Run Preview를 다시 실행한다.

기대 결과:

- Step 순서가 유지된다.
- 파라미터가 유지된다.
- Input/Output 레이어가 유지된다.
- 저장된 Preview 이미지가 없어도 Pipeline 자체는 사라지지 않는다.
- 샘플 이미지 복원 여부는 로그로 명확히 안내된다.

대표 검증:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunVisionPlatformPrecheck.ps1 -SkipUi
```

## 6. Sample Catalog

목표: 샘플 이미지는 OpenVisionLab의 학습/검증 기준입니다. 사용자는 샘플 이미지, 추천 Pipeline, 기대 결과를 한 번에 열 수 있어야 합니다.

검증 절차:

1. Pipeline Samples를 연다.
2. Recipe Catalog에서 Required Sample을 선택한다.
3. Open + Preview를 실행한다.
4. Expected Metric과 Actual Metric을 비교한다.
5. Result image, Overlay, Final layer를 확인한다.

기대 결과:

- Required sample은 모두 OK여야 한다.
- Failed sample은 실패 Step, ErrorCode, DiagnosticHint, SuggestedFix를 제공해야 한다.
- ResultCount, Area, BoundsWidth, LineLength 같은 metric이 기대 범위와 비교된다.

대표 검증:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1
```

## 7. External Runner

목표: UI에서 만든 XML은 UI 없이도 실행 가능해야 합니다.

검증 절차:

1. 검증된 Pipeline XML을 준비한다.
2. VisionRecipeRunnerSmoke를 실행한다.
3. OutcomeText, SummaryText, StepSummaryText, FinalMetricsText를 확인한다.
4. 실패 Recipe도 실행하여 FirstFailedSummaryText와 SuggestedFix를 확인한다.

기대 결과:

- OK Recipe는 final layer, metrics, overlays를 반환한다.
- NG Recipe는 첫 실패 Step과 수정 가이드를 반환한다.
- UI에 의존하지 않고 실행된다.

대표 검증:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunVisionPlatformPrecheck.ps1 -SkipUi
```

## Completion Rule

위 시나리오가 모두 안정적으로 통과하면 OpenVisionLab은 내부 개발/검증 플랫폼 기준으로 95% 이상으로 본다.

남은 5%는 다음 성격의 작업입니다.

- 실제 작업자 반복 사용 피드백
- Tool별 더 많은 정상/불량 paired sample
- WPG 공통 Editor 최종 통합
- AI Recipe의 interactive parameter edit
- 배포/패키징/외부 참조 문서화
