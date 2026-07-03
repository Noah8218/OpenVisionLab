# Vision Pipeline Runtime and DLL Plan

OpenVisionLab의 장기 목표는 UI에서 검증한 Rule-based Vision Recipe를 실제 설비 프로그램에서 그대로 실행하는 것이다. 이를 위해 UI Workbench와 Runtime을 분리하는 방향을 잡는다.

## 1. 목표 구조

```text
OpenVisionLab UI
  - Image load/view
  - ROI and parameter tuning
  - Pipeline edit
  - Sample and history validation

Recipe XML
  - VisionPipeline
  - Parameters
  - Acceptance criteria

Runtime / DLL
  - Load Recipe XML
  - Load input image or Mat
  - Execute Pipeline
  - Return result image, metrics, OK/NG, logs
```

## 2. Runtime API 후보

초기 DLL API는 단순해야 한다.

```csharp
public sealed class VisionRecipeRunner
{
    public VisionRecipeRunResult Run(string recipeXmlPath, Mat sourceImage);
}
```

현재 OpenVisionLab 앱 내부에는 1차 초안으로 `VisionRecipeRunner`가 추가되어 있다. 이 Runner는 `VisionPipeline` XML 또는 객체와 `Mat` 입력을 받아 기존 Pipeline 실행 서비스를 호출하고, UI 의존이 없는 `VisionRecipeRunResult`를 돌려준다.

결과 모델 후보:

```csharp
public sealed class VisionRecipeRunResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public Mat ResultImage { get; set; }
    public List<VisionStepRunResult> Steps { get; set; }
}
```

## 3. 분리 기준

Runtime에 들어가면 안 되는 것:

- WinForms Form
- PropertyGrid
- DockPanel
- MessageBox
- 사용자 조작 UI

Runtime에 들어가야 하는 것:

- `VisionPipeline`
- `VisionPipelineStep`
- Tool factory
- Tool parameter binding
- Pipeline execution
- Result summary
- Acceptance evaluation

## 4. OpenVisionLab에서 먼저 해야 할 일

1. 모든 Tool이 `IVisionTool.Execute(Mat)` 경로로 실행되게 한다.
2. 모든 Tool의 Property -> Step 변환을 `VisionPipelineStepBuilder`로 모은다.
3. Step -> Tool 변환을 UI와 Runtime이 공유 가능한 factory로 이동한다.
4. UI 의존 로그/MessageBox를 Runtime에서 제거한다.
5. Recipe XML 호환성 체크를 CI 또는 릴리즈 전 점검으로 사용한다.

## 5. LLM 연동과의 관계

LLM은 Runtime을 직접 실행하지 않는다. LLM은 Recipe 초안을 만든다.

```text
User image + user guide
  -> LLM suggests VisionPipeline XML
  -> OpenVisionLab loads XML
  -> User tunes parameters
  -> Runtime executes approved Recipe
```

이 흐름이면 LLM은 아이디어와 초기값을 제공하고, 최종 검증 책임은 OpenVisionLab의 실제 실행 결과가 갖는다.
