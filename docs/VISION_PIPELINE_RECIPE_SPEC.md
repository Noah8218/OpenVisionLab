# Vision Pipeline Recipe Spec

이 문서는 OpenVisionLab Pipeline XML을 사람이 직접 읽고, LLM이 생성하고, OpenVisionLab이 실행할 수 있게 만들기 위한 기준이다.

## 1. 방향

Pipeline Recipe는 OpenVisionLab의 핵심 교환 포맷이다.

- UI에서 만든 검사 흐름을 저장한다.
- 다른 PC에서 같은 검사를 재현한다.
- LLM이 이미지 분석 가이드를 받아 Recipe 초안을 생성할 수 있다.
- 나중에 DLL/Runner가 UI 없이 같은 Recipe를 실행한다.

## 2. XML Root

현재 기본 Root는 `VisionPipeline`이다.

```xml
<VisionPipeline>
  <Name>Inspection</Name>
  <Steps>
    <Step>
      <Name>Threshold1</Name>
      <ToolType>Threshold</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Main</InputLayer>
      <OutputLayer>Binary</OutputLayer>
      <Parameters>
        <Parameter>
          <Key>Threshold</Key>
          <Value>127</Value>
        </Parameter>
      </Parameters>
    </Step>
  </Steps>
</VisionPipeline>
```

## 3. Step 필드

| Field | Required | Description |
| --- | --- | --- |
| `Name` | Yes | 사용자가 볼 Step 이름 |
| `ToolType` | Yes | 실행할 Tool 종류 |
| `Enabled` | No | false면 Step을 건너뜀 |
| `InputLayer` | Yes | 이전 Step 또는 이미지 로드로 존재하는 레이어 |
| `OutputLayer` | Yes | Step 결과를 저장할 레이어 |
| `Parameters` | No | Tool별 Property 값 |

## 4. Parameter 규칙

Parameter는 문자열 Key/Value로 저장한다.

- 숫자: `InvariantCulture` 형식 사용
- Boolean: `true` 또는 `false`
- Enum: C# enum 이름 사용
- ROI: `x,y,width,height`
- ROI List: `x,y,width,height;x,y,width,height`

예:

```xml
<Parameter>
  <Key>CvROI</Key>
  <Value>10,20,640,480</Value>
</Parameter>
```

## 5. Acceptance 규칙

Step마다 OK/NG 기준이 필요하면 Acceptance 필드를 사용한다.

```xml
<UseAcceptance>true</UseAcceptance>
<ExpectedSuccess>true</ExpectedSuccess>
<MaxElapsedMilliseconds>150</MaxElapsedMilliseconds>
<AcceptanceMetricName>ResultCount</AcceptanceMetricName>
<UseAcceptanceMetricMinimum>true</UseAcceptanceMetricMinimum>
<AcceptanceMetricMinimum>1</AcceptanceMetricMinimum>
<UseAcceptanceMetricMaximum>true</UseAcceptanceMetricMaximum>
<AcceptanceMetricMaximum>10</AcceptanceMetricMaximum>
```

Acceptance는 Recipe가 단순 영상처리 체인이 아니라 검증 기준까지 가진 검사 Recipe가 되게 만든다.

## 6. LLM 생성 Recipe 기준

LLM이 Recipe를 만들 때는 아래 순서를 지킨다.

1. 이미지에서 검출 목표를 요약한다.
2. 필요한 Step 후보를 `Threshold -> Morphology -> Contour`처럼 제안한다.
3. 각 Step의 Input/Output Layer를 명확히 연결한다.
4. 파라미터는 보수적인 초기값으로 넣는다.
5. 검출 성공 기준이 있으면 Acceptance를 넣는다.
6. 모르는 값은 과감하게 추정하지 말고 조정 후보로 남긴다.

## 7. 현재 지원 ToolType

현재 OpenVisionLab Runtime 기준 주요 ToolType:

- `Threshold`
- `Morphology`
- `Filter`
- `EdgeDetection`
- `Blob`
- `Contour`
- `LineGauge`
- `Matching`
- `Mean`
- `FeatureMatching`

Alias로 `Edge`, `Line`, `TemplateMatching`, `Feature`, `Sift`도 허용된다.
