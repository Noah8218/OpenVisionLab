# OpenVisionLab Vision Tool Contract

OpenVisionLab의 Vision Tool은 UI에서 파라미터를 조정하는 기능이면서, 동시에 Pipeline과 DLL/Runner에서 그대로 실행할 수 있어야 한다. 새 Tool은 아래 계약을 만족해야 플랫폼 안에서 같은 방식으로 검증, 저장, 재실행된다.

## 1. Tool의 책임

Tool은 하나의 입력 이미지와 파라미터를 받아 하나의 결과를 만든다.

- 입력: `Mat source`
- 출력: `VisionToolResult`
- 실행 진입점: `IVisionTool.Execute(Mat source)`
- 결과 이미지: `VisionToolResult.ResultImage`
- 실행 결과: `Success`, `Message`, `Elapsed`, `Exception`

UI Form은 Tool 내부 알고리즘을 직접 구현하지 않고, 가능한 한 Tool을 호출하는 얇은 화면이어야 한다.

## 2. Property의 책임

Property는 사용자가 조정할 수 있는 검사 조건을 담는다.

- UI PropertyGrid에 표시될 값
- XML/Pipeline에 저장될 값
- Tool 실행 시 다시 복원 가능한 값

Property 값은 Pipeline으로 넘어갈 때 `VisionPipelineStep.Parameters`의 문자열 Dictionary로 저장된다. 숫자는 `InvariantCulture` 기준 문자열을 사용한다.

## 3. Pipeline Step 계약

`VisionPipelineStep`은 최소한 아래 정보를 가져야 한다.

- `Name`: 사용자가 이해할 수 있는 Step 이름
- `ToolType`: `Threshold`, `Morphology`, `Contour` 같은 Tool 식별자
- `Enabled`: 실행 여부
- `InputLayer`: 입력 레이어 이름
- `OutputLayer`: 출력 레이어 이름
- `Parameters`: Tool Property를 복원할 수 있는 Key/Value 목록

추가로 검증 기준이 필요한 경우 아래 Acceptance 항목을 사용한다.

- `UseAcceptance`
- `ExpectedSuccess`
- `MaxElapsedMilliseconds`
- `RequiredMessageText`
- `AcceptanceMetricName`
- `AcceptanceMetricMinimum`
- `AcceptanceMetricMaximum`

## 4. 새 Tool 추가 체크리스트

새 Tool을 추가할 때는 다음 위치를 함께 확인한다.

1. Tool property와 Tool 실행 클래스 추가
2. UI Form에서 `RunVisionStep(...)` 사용
3. UI Form에서 `PublishResult(...)`로 결과 레이어 반영
4. `VisionPipelineStepBuilder`에 Property -> Step 변환 추가
5. `VisionPipelineAppToolFactory` 또는 Lib factory에 Step -> Tool 변환 추가
6. `VisionPipelineStepParameterSchema`에 주요 파라미터 타입 추가
7. `VisionPipelineValidator`의 지원 ToolType 목록 추가
8. 필요하면 `VisionPipelineKnownMetrics`에 결과 Metric 정의 추가
9. Recipe XML 호환성 체크 또는 샘플 Recipe에 최소 1개 예제 추가

## 5. 현재 우선 표준화 대상

이미 공통 실행 흐름에 들어온 주요 폼:

- `Arithmetic`
- `Blob`
- `Contour`
- `EdgeDetection`
- `FeatureMatching`
- `Filter`
- `Histogram`
- `HSV`
- `Line`
- `Matching`
- `Mean`
- `Morphology`
- `RotateAndScale`

다음 개발에서는 이 폼들이 동일하게 Input/Output Layer, Result Publish, Log, Pipeline Step 변환을 제공하는지 점검한다.

## 6. 실패 처리 기준

Tool은 실패를 숨기지 않는다.

- 입력 이미지가 없으면 실행 실패로 처리한다.
- 파라미터가 잘못되면 실패 메시지를 남긴다.
- Pipeline에서는 실패한 Step 이후를 기본적으로 중단한다.
- UI에서는 실패 메시지와 로그가 같이 남아야 한다.

이 기준이 있어야 OpenVisionLab에서 검증한 Recipe를 실제 설비 Runtime으로 옮겼을 때 동작이 달라지지 않는다.
