# OpenVisionLab Phase 1-2 Stabilization Plan

이 문서는 OpenVisionLab을 내부 개발/검증 플랫폼으로 정리하기 위한 1~2단계 기준이다.

목표는 새 알고리즘을 늘리는 것이 아니라, 이미 있는 이미지/레이어/ROI/알고리즘 테스트 흐름을 같은 방식으로 검증하고 반복 사용할 수 있게 만드는 것이다.

## Phase 1. 현재 구조 안정화

### 목표

어떤 Vision Test 화면을 열어도 사용자가 같은 흐름으로 작업할 수 있어야 한다.

기준 흐름:

1. 입력 레이어를 선택한다.
2. 필요하면 ROI를 지정한다.
3. 파라미터를 조정한다.
4. 출력 레이어를 선택하거나 새로 만든다.
5. Run을 실행한다.
6. 결과 이미지와 Tack Time을 확인한다.
7. 결과 레이어를 다음 알고리즘의 입력으로 사용한다.

### 우선 점검 항목

- 입력 레이어 콤보가 현재 Display Layer 목록과 동기화되는가.
- 출력 레이어 콤보가 현재 Display Layer 목록과 동기화되는가.
- `Create Output Layer` 버튼으로 만든 레이어가 즉시 출력 후보에 들어오는가.
- 입력 이미지가 없을 때 실행 실패 메시지가 사용자에게 보이는가.
- 잘못된 파라미터 값이 들어왔을 때 앱이 죽지 않고 실패 메시지가 보이는가.
- 결과 이미지는 출력 레이어와 폼 내부 출력 뷰어에 동시에 반영되는가.
- `Tack Time` 형식이 알고리즘별로 흔들리지 않는가.
- ROI가 있는 레이어를 입력으로 쓸 때 ROI 영역만 처리되는가.
- ROI가 없는 레이어를 입력으로 쓸 때 전체 이미지가 처리되는가.

### 현재 1차 적용 기준

다음 폼은 공통 실행 실패 처리와 Tack Time 포맷을 우선 적용한다.

- `FormVision_Filter`
- `FormVision_Morphology`
- `FormVision_Blob`
- `FormVision_Contour`
- `FormVision_EdgeDetection`
- `FormVision_Line`
- `FormVision_Matching`

이 폼들은 대표 시나리오에 직접 포함되거나, 이후 Pipeline Runtime으로 이전할 때 우선순위가 높은 검사 기능이다.

### Tool 분리 기준

검사 계열 `BlobTool`, `ContourTool`, `LineGaugeTool`, `MatchingTool`처럼 전처리 계열도 UI 밖에서 실행 가능한 Tool로 분리한다.

1차 기준으로 `MorphologyTool`, `FilterTool`, `ThresholdTool`, `EdgeDetectionTool`을 추가했다.

- Property: `MorphologyToolProperty`
- Interface: `IOpenCVPropertyMorphology`
- Tool: `MorphologyTool`
- UI 연결: `FormVision_Morphology`는 직접 `Cv2.MorphologyEx`를 호출하지 않고 `MorphologyTool`을 호출한다.
- Property: `FilterToolProperty`
- Interface: `IOpenCVPropertyFilter`
- Tool: `FilterTool`
- UI 연결: `FormVision_Filter`는 직접 `Cv2.Blur`, `Cv2.GaussianBlur`, `Cv2.MedianBlur`, `Cv2.BoxFilter`, `Cv2.BilateralFilter`를 호출하지 않고 `FilterTool`을 호출한다.
- Property: `ThresholdToolProperty`
- Interface: `IOpenCVPropertyThreshold`
- Tool: `ThresholdTool`
- UI 연결: `FormThreshold`는 직접 `Cv2.Threshold`, `Cv2.InRange`, `Cv2.AdaptiveThreshold`를 호출하지 않고 `ThresholdTool`을 호출한다.
- Property: `EdgeDetectionToolProperty`
- Interface: `IOpenCVPropertyEdgeDetection`
- Tool: `EdgeDetectionTool`
- UI 연결: `FormVision_EdgeDetection`은 직접 `Cv2.Canny`, `Cv2.Sobel`, `Cv2.Scharr`, `Cv2.Laplacian`을 호출하지 않고 `EdgeDetectionTool`을 호출한다.

다음 후보는 `HSVTool`, `HistogramTool`, `RotateAndScaleTool` 중 실제 사용 빈도가 높은 순서로 잡는다.

### 공통 Tool 실행 계약

Pipeline Runtime과 DLL Runtime으로 넘어가기 위해 Tool 실행 결과를 표준화한다.

- Interface: `IVisionTool`
- Result: `VisionToolResult`
- Base: `OpenCvAlgorithmBase`가 `IVisionTool.Execute(Mat source)`를 기본 구현한다.

현재 `OpenCvAlgorithmBase`를 상속하는 Tool은 기존 `Run()` 호출도 유지하면서, Runtime에서는 `Execute(Mat)`로 실행할 수 있다.

표준 결과에는 다음 값을 담는다.

- `Success`
- `Message`
- `ResultImage`
- `Elapsed`
- `Exception`

### Pipeline 준비 모델

수동 레이어 체인을 저장/재실행 가능한 데이터로 바꾸기 위한 최소 모델을 추가한다.

- `VisionPipeline`
- `VisionPipelineStep`
- `VisionPipelineContext`
- `VisionPipelineStepResult`
- `VisionPipelineRunResult`
- `VisionPipelineRuntime`

초기 Pipeline Step은 다음 정보를 가진다.

- `Name`
- `ToolType`
- `InputLayer`
- `OutputLayer`
- `Parameters`

`VisionPipelineRuntime`은 Tool 생성과 파라미터 바인딩을 아직 직접 책임지지 않는다. 현재는 `Func<VisionPipelineStep, IVisionTool>` factory를 외부에서 받아 실행한다. 이렇게 하면 UI, 테스트 코드, DLL Runtime에서 각자 다른 방식으로 Tool을 만들 수 있다.

기본 Tool factory도 추가했다.

- `VisionPipelineToolFactory`
- `VisionPipelineRuntime()` 기본 생성자는 `VisionPipelineToolFactory.Create`를 사용한다.
- `VisionPipelineAppToolFactory`

현재 기본 factory가 지원하는 `ToolType`:

- `Threshold` 또는 `ThresholdTool`
- `Morphology` 또는 `MorphologyTool`
- `Filter` 또는 `FilterTool`
- `EdgeDetection`, `Edge`, 또는 `EdgeDetectionTool`

앱 레벨 factory가 추가 지원하는 `ToolType`:

- `Blob` 또는 `BlobTool`
- `Contour` 또는 `ContourTool`
- `Line`, `LineGauge`, 또는 `LineGaugeTool`
- `Matching`, `TemplateMatching`, 또는 `MatchingTool`

`BlobTool`은 `Lib.OpenCV.Blob` 프로젝트에 있어 `Lib.OpenCV` 기본 factory에서 직접 참조하면 순환 참조가 생긴다. 따라서 검사 Tool 확장은 메인 앱 쪽 `VisionPipelineAppToolFactory`에서 담당한다. UI 또는 앱 Runtime에서 검사 Tool까지 실행하려면 `new VisionPipelineRuntime(VisionPipelineAppToolFactory.Create)` 형태로 생성한다.

Pipeline Step 예시:

```text
Name: Binary threshold
ToolType: Threshold
InputLayer: Main
OutputLayer: Threshold
Parameters:
  Mode = Threshold
  Threshold = 120
  MaxValue = 255
  ThresholdType = Binary
```

```text
Name: Noise close
ToolType: Morphology
InputLayer: Threshold
OutputLayer: Morphology_Result
Parameters:
  Shape = Rect
  Operator = Close
  KernelWidth = 3
  KernelHeight = 3
  Iterations = 1
```

```text
Name: Edge
ToolType: EdgeDetection
InputLayer: Filtered
OutputLayer: Edge
Parameters:
  EdgeType = Canny
  CannyThresholdLow = 100
  CannyThresholdHigh = 200
  CannyApertureSize = 3
  UseL2Gradient = true
```

다음 Pipeline 구현 후보:

1. `Threshold -> Morphology -> Blob`
2. `Filter -> EdgeDetection -> Line`

다음 세부 작업:

- `VisionPipelineStep.Parameters`를 실제 Tool property로 변환하는 binder 추가
- Pipeline 결과에서 Blob/Contour/Line/Matching의 측정 리스트를 표준 결과 모델로 노출
- UI에서 현재 레이어 체인을 `VisionPipeline`으로 저장하는 기능 추가
- `Run Selected Step`, `Run All` 연결

## Phase 2. 대표 검증 시나리오 선정

대표 시나리오는 OpenVisionLab의 방향성을 보여주는 데 가장 중요하다. 각 시나리오는 "OpenCV 개념 이해", "룰베이스 검증", "레이어 기반 체인"을 동시에 보여줘야 한다.

### Scenario A. Threshold -> Morphology -> Blob

목적:

- 이진화와 노이즈 제거 후 객체를 검출하는 가장 기본적인 룰베이스 검사 흐름을 보여준다.

권장 흐름:

1. Main 레이어에 원본 이미지를 로드한다.
2. Threshold 패널에서 이진화 결과를 `Threshold` 레이어로 만든다.
3. Morphology 화면에서 입력을 `Threshold`로 선택한다.
4. 출력 레이어를 새로 만들고 `Morphology_Blob_Input`처럼 이름을 지정한다.
5. Erode/Dilate/Open/Close 중 목적에 맞는 연산을 적용한다.
6. Blob 화면에서 입력을 Morphology 결과 레이어로 선택한다.
7. Blob 파라미터의 ROI, 최소/최대 면적, 색상 조건을 조정한다.
8. Run 후 검출 Count, Bounding Box, OK/NG 판단 가능성을 확인한다.

완료 기준:

- 같은 이미지에서 같은 파라미터로 반복 실행하면 같은 Blob 결과가 나온다.
- 중간 결과 레이어를 바꿔도 사용자가 현재 입력/출력을 혼동하지 않는다.
- 결과 이미지가 최종 출력 레이어에 남아 다음 단계에서 재사용 가능하다.

### Scenario B. Filter -> Edge Detection -> Line

목적:

- 노이즈를 줄인 뒤 에지/라인을 검출하여 길이, 위치, 교차점 같은 측정 검사 흐름을 보여준다.

권장 흐름:

1. Main 레이어에 원본 이미지를 로드한다.
2. Filter 화면에서 Gaussian/Median 등 노이즈 제거 필터를 적용한다.
3. 출력 레이어를 `Filtered`로 만든다.
4. Edge Detection 화면에서 입력을 `Filtered`로 선택한다.
5. Canny/Sobel/Scharr/Laplacian 중 목적에 맞는 에지를 만든다.
6. 출력 레이어를 `Edge`로 만든다.
7. Line 화면에서 입력을 `Edge` 또는 `Filtered`로 선택한다.
8. Line Gauge 파라미터를 조정하고 Run 또는 Edge Run을 실행한다.

완료 기준:

- Filter 결과가 Edge Detection 입력으로 자연스럽게 이어진다.
- Line 결과가 출력 레이어에 오버레이로 표시된다.
- 파라미터 오류나 입력 이미지 없음이 앱 크래시로 이어지지 않는다.

### Scenario C. Template Matching -> Result

목적:

- OpenCV 기반 패턴 매칭으로 위치/각도/스코어를 찾는 검출 흐름을 보여준다.

권장 흐름:

1. Main 레이어에 검사 이미지를 로드한다.
2. Matching 속성에서 Template 이미지를 지정한다.
3. ROI가 필요하면 검색 영역을 제한한다.
4. Score, angle, count 조건을 조정한다.
5. Run 후 매칭 위치, 각도, Score 표시를 확인한다.
6. Details 창에서 결과 리스트를 확인한다.

완료 기준:

- 템플릿 이미지가 없거나 잘못된 경우 실패 메시지가 보인다.
- 매칭 결과 Bounding Box와 Score가 결과 이미지에 표시된다.
- 파라미터 저장 후 다시 열어도 동일 조건으로 재검증할 수 있다.

## 다음 단계로 넘길 기준

Phase 1~2가 끝났다고 판단하려면 아래 조건을 만족해야 한다.

- 대표 시나리오 3개가 수동으로 반복 실행 가능하다.
- 각 시나리오의 중간 결과가 레이어로 남는다.
- 주요 실행 실패가 사용자 메시지로 처리된다.
- Tack Time 표시가 주요 폼에서 같은 형식이다.
- 코드상 알고리즘 실행 흐름이 `VisionTestForm` 공통 메서드에 최대한 모인다.

이 기준을 만족한 뒤 Phase 3에서 `IVisionStep`, `VisionPipeline`, `Run All` 구조로 넘어간다.
