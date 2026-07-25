# OpenVisionLab 프로젝트 브리프

Updated: 2026-07-03

## 한 줄 소개

OpenVisionLab은 초보자도 룰베이스 비전 검사를 직접 만들고 검증할 수 있도록 만든 OpenCvSharp4 기반 WPF 비전 워크벤치입니다.

## 만든 이유

룰베이스 비전 검사는 알고리즘 하나를 잘 쓰는 것만으로 끝나지 않습니다.
실제 작업에서는 원본 이미지, 전처리 결과, 최종 검출 결과를 계속 비교해야 하고, 결과가 OK인지 NG인지도 metric으로 설명할 수 있어야 합니다.

기존 방식에서는 다음 문제가 자주 생깁니다.

- 지금 Tool이 어떤 이미지를 읽는지 헷갈린다.
- Output이 생기면서 Input이 바뀌어 검사 흐름이 꼬인다.
- Score나 Count는 보이지만 왜 OK/NG인지 설명하기 어렵다.
- 샘플을 바꿔가며 정상/불량 기준을 검증하는 과정이 불편하다.
- UI에서 만든 설정이 batch 검증에서도 같은 결과를 내는지 확인하기 어렵다.

OpenVisionLab은 이 문제를 줄이는 쪽으로 설계했습니다.

## 핵심 컨셉

```text
Layer로 이미지 흐름을 보고
Tool에서 파라미터를 조정하고
Pipeline으로 검사 순서를 만들고
Pipeline Review에서 OK/NG 이유를 확인하고
Sample Catalog로 정상/불량 기준을 반복 검증한다.
```

이 프로젝트의 중심은 “설명 가능한 룰베이스 비전 검사”입니다.
단순히 결과 이미지를 보여주는 것이 아니라, 어떤 metric이 기준을 넘었는지, 어떤 Step에서 문제가 났는지, 다음에 어떤 파라미터를 봐야 하는지를 보여주는 방향으로 만들고 있습니다.

## 주요 기능

- WPF 기반 Main Workspace
- OpenGL image viewer
- zoom / pan / drag / pixel status
- Layer 생성, 삭제, 이미지 로드, docking 비교
- PropertyGrid 기반 Tool 파라미터 편집
- Threshold, Morphology, Filter, Blob, Contour, Matching, FeatureMatching, Line, Mean 등 OpenCvSharp4 기반 Tool
- Tool preset: Basic / Fast / Precise
- Preview / Run 명시 실행
- Pipeline Step 구성
- Recipe XML 저장
- Pipeline Review
- OK/NG metric acceptance
- Sample Catalog
- Good/Bad sample pair
- 한국어/영어 localization 구조
- WPF/MVVM 방향의 ShellHost 리팩토링
- smoke test와 readiness contract 기반 검증

## 기술적으로 신경 쓴 부분

### 1. Input/Output Layer를 명확히 분리

Output Layer가 만들어졌다고 Input Layer가 자동으로 바뀌면 안 됩니다.
검사 흐름이 조금만 꼬여도 사용자는 잘못된 이미지를 기준으로 파라미터를 맞추게 됩니다.

그래서 Tool View와 Pipeline에서 Input/Output을 계속 분리해서 보여주고, Output 선택은 결과를 어디에 쓸지 정하는 행동으로만 다룹니다.

### 2. Preview와 Run/Publish 분리

visibility toggle이나 단순 UI 변경만으로 Preview/Run이 실행되지 않도록 했습니다.
사용자가 명시적으로 실행했을 때만 결과가 만들어지고, Preview 결과와 실제 반영 동작도 분리했습니다.

이 부분은 초보자에게 특히 중요합니다.
화면이 바뀐 이유가 사용자의 실행인지, 단순 표시 변경인지 구분되어야 하기 때문입니다.

### 3. PropertyGrid 기반 Tool 구조

알고리즘 Tool은 모델 property를 중심으로 구성했습니다.

```text
Tool Property Model
  -> PropertyGrid
  -> Preview / Run
  -> VisionToolResult
  -> Pipeline Step XML
```

이 구조는 Tool을 계속 늘려도 UI와 저장 구조를 일관되게 유지하기 위한 선택입니다.
초보자에게는 preset과 결과 해석을 제공하고, 숙련자는 PropertyGrid에서 세부 파라미터를 직접 만질 수 있게 했습니다.

### 4. Pipeline Review

Pipeline Review는 Step별 input/output image, metric, acceptance, log를 한 화면에서 확인하는 검증 화면입니다.

예를 들어 Bad 샘플에서 다음처럼 설명합니다.

```text
NG 원인: Bounds Width Max
측정값: 26
목표: 0 - 18
해석: 검출은 되었지만 정상 shaft 폭보다 커서 NG
```

이런 방식으로 결과 이미지만 보는 것이 아니라, 왜 NG인지 설명할 수 있게 했습니다.

### 5. Good/Bad Sample Catalog

Sample Catalog는 public synthetic 샘플과 product-domain synthetic 샘플을 분리해 관리합니다.
각 샘플은 이미지, baseline pipeline, expected metric, Good/Bad pair 기준을 함께 갖고 있으며, 같은 pipeline으로 정상/불량 차이를 반복 검증하는 데 사용합니다.

대표 예시:

- Blob sparse density: `ResultCount` 기준 NG
- Bent pin shaft: `BoundsWidthMax` 기준 NG
- Film dark spot: `AreaMax` 기준 NG
- LineGauge tilted pin: `LineAngleAvg` 기준 NG
- Feature low score: `ScoreMax` 기준 NG
- Mean brightness drift: `MeanValueAvg` 기준 NG

Bad 샘플을 무조건 실패로 처리하지 않고, 안정적인 metric gate가 있는 경우에만 controlled NG로 승격합니다.
나머지는 비교용 Bad로 남겨두어 정상/불량 차이를 학습하는 데 사용합니다.

## 현재 완성도와 남은 과제

현재 핵심 구조는 잡혀 있습니다.

- 이미지 로드와 Layer workspace
- Tool별 preview/run
- Pipeline 구성
- Recipe XML 저장
- Pipeline Review
- Sample Catalog와 Good/Bad pair
- WPF Shell / Docking 구조
- 주요 smoke 검증

남은 과제는 기능이 없는 상태라기보다, 제품 완성도를 더 높이는 방향입니다.

- 더 많은 실제 검사 Good/Bad pair 확보
- Tool별 결과 해석 문구 고도화
- 샘플 중심 Learn Mode 확장
- Recipe 전환 UX 마무리
- ShellHost code-behind 축소와 MVVM 경계 정리
- 배포/버전 정책 정리

## 대표 시각 자료

튜토리얼과 프로젝트 소개에서는 단순 전체 화면보다 번호가 표시된 캡처를 쓰는 편이 좋습니다.
보는 사람이 화면을 훑으면서 “어디를 눌러야 하는지”와 “결과를 어디서 확인하는지”를 바로 따라갈 수 있기 때문입니다.

단, 대표 이미지는 반드시 현재 빌드의 `OpenVisionLab.exe`에서 다시 캡처한 이미지를 사용합니다.
예전 캡처를 그대로 가져오면 실제 UI와 설명이 어긋나므로, 이미지를 교체하기 전 [문서 캡처 가이드](OPENVISIONLAB_DOCUMENTATION_CAPTURE_GUIDE.md)의 절차를 먼저 실행합니다.

- Main Workspace: `docs/assets/tutorial/annotated/main_workspace_callouts.png`
- Layer Docking: `docs/assets/tutorial/annotated/layer_docking_callouts.png`
- Tool View: `docs/assets/tutorial/annotated/tool_matching_form_callouts.png`
- Pipeline Review: `docs/assets/tutorial/annotated/pipeline_matching_review_callouts.png`
- Learn Mode: `docs/learn/README.md`, `docs/learn/LEARN_MATCHING.md`, `docs/learn/LEARN_BLOB.md`, `docs/learn/LEARN_LINE.md`

## 강조할 문장

OpenVisionLab은 룰베이스 비전 검사를 초보자도 따라갈 수 있게 만들기 위한 WPF/OpenCvSharp4 기반 워크벤치입니다.
이미지를 Layer로 나누어 비교하고, Tool 파라미터를 PropertyGrid로 조정하며, Pipeline Review에서 OK/NG를 metric으로 설명합니다.
특히 Good/Bad 샘플 pair를 통해 단순한 데모가 아니라 재현 가능한 검사 기준을 만드는 데 집중했습니다.

## 개발 기준

OpenVisionLab에서 중요한 기준은, 비전 프로그램의 난이도가 알고리즘 자체보다 “결과를 어떻게 믿게 만들 것인가”에서 많이 나온다는 점입니다.

Threshold, Blob, Matching 같은 Tool은 각각 따로 만들 수 있습니다.
하지만 실제 제품처럼 쓰려면 input/output layer, recipe 저장, sample 검증, OK/NG 설명, log, undo 가능한 작업 흐름, 사용자가 실수하지 않는 UI가 같이 필요합니다.

OpenVisionLab은 이 부분을 하나씩 제품 구조로 묶어가는 프로젝트입니다.
