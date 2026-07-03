# OpenVisionLab UX Competitor Review Refresh

Updated: 2026-07-01

이 문서는 OpenVisionLab의 현재 UX 상태를 공식 타사 자료와 비교해 자체 평가하고, 다음 구현 우선순위를 고정하기 위한 작업 문서입니다. 범위는 메인 워크벤치, 도킹된 Tool View 검증 흐름, 초보자 학습 흐름, Pipeline/Recipe 검토 UX입니다.

## 확인한 공식 자료

| 제품 | 공식 자료 | 이번 판단에 사용한 관찰점 |
| --- | --- | --- |
| Cognex In-Sight EasyBuilder | https://docs.cognex.com/is_611/web/EN/ise/Content/EasyBuilder/AppSteps.htm | 검사 앱을 Set Up Image, Locate Part, Inspect Part, Inputs/Outputs 같은 단계로 이해시키는 구조 |
| MVTec MERLIC | https://www.mvtec.com/products/merlic | no-code 산업용 머신비전, 이미지 중심 도구 구성, 프로세스 통합 방향 |
| MVTec HDevelop | https://www.mvtec.com/products/halcon/work-with-halcon/hdevelop | Graphics Window, Variable Window, profiler/debugger처럼 이미지와 진단 정보를 분리해 확인하는 개발 환경 |
| NI Vision Builder AI | https://www.ni.com/en/shop/electronic-test-instrumentation/application-software-for-electronic-test-and-instrumentation-category/what-is-vision-builder-for-automated-inspection.html | menu-driven 검사 구성, 결과 확인, benchmark 기반 검증 방향 |
| Aurora Vision Studio | https://docs.adaptive-vision.com/5.6/studio/introduction/HowToLearn.html | 예제, 튜토리얼, 필터 문서로 학습 경로를 제품 안팎에서 연결하는 방식 |
| KEYENCE XG-X | https://www.keyence.com/products/vision/vision-sys/xg-x/ | flowchart, debug, simulation을 통해 검사 흐름을 확인하는 방향 |

## 타사 UX에서 공통으로 보이는 패턴

1. 검사 흐름은 도구 목록이 아니라 단계로 설명된다.
   - 초보자에게는 `이미지 준비 -> 위치/ROI/템플릿 지정 -> 검사 -> 결과 확인 -> 입출력/저장` 같은 순서가 먼저 보인다.
   - OpenVisionLab의 왼쪽 Tool List는 유지하되, 중앙/오른쪽 화면에서는 현재 단계와 다음 행동을 계속 보여줘야 한다.

2. 이미지는 항상 주 작업면이다.
   - 도구 설정, 결과, 로그, Pipeline은 이미지를 가리지 않는 보조 영역이어야 한다.
   - 도킹된 Tool View는 이미지 검증을 방해하지 않는 폭과 정보 밀도가 중요하다.

3. 결과는 숫자만으로 끝나지 않는다.
   - Count, Score, Box, Angle, Tact 같은 지표는 필요하지만, 초보자는 `OK/NG`, `왜 실패했는지`, `다음에 무엇을 조정할지`를 함께 봐야 한다.

4. 예제와 템플릿은 별도 문서가 아니라 제품 내 진입점이어야 한다.
   - 샘플은 “구경”이 아니라 실제 이미지, Pipeline, 기대 지표, 다음 행동을 연결해야 한다.

5. 고급 디버깅은 초보자 기본 화면과 분리되어야 한다.
   - HDevelop/KEYENCE 계열처럼 디버깅, 변수, 프로파일러, flow review는 강력해야 하지만 기본 화면을 과밀하게 만들면 안 된다.

## OpenVisionLab 현재 자체 평가

평가는 2026-06-30까지의 복구 문서, 안정 계약, smoke 결과, 최근 도킹/메인 워크벤치 작업 상태를 기준으로 한다.

| 영역 | 현재 평가 | 근거 | 부족한 점 |
| --- | --- | --- | --- |
| 제품 정체성 | 좋음 | Layer 기반 rule-based vision workbench 방향이 명확하고, PropertyGrid 기반 도구 확장성이 유지된다. | 초보자용 설명 계층이 아직 모든 도구에 공통 적용되지 않았다. |
| 도킹 레이어 UX | 안정화 구간 | Visual Studio식 상단 탭/문서형 도킹, global/pane-local 분기, native floating preview suppression까지 최근 회귀가 많이 줄었다. | 앞으로는 새 기능보다 regression gate 유지가 우선이다. |
| Matching 도킹 Tool View | 좋음 | compact verification guide, result guidance, PropertyGrid 보존, manual Preview 계약이 정리되었다. | Matching에서 검증된 패턴을 Contour/Blob/Line으로 일반화해야 한다. |
| 메인 시작 UX | 보통 이상 | no-image beginner card, sample picker, sample workflow strip, explicit sample actions가 추가되었다. | “처음 무엇을 배워야 하는지”는 개선됐지만, Tool View와 Pipeline Review까지 하나의 학습 루프로 이어지는 힘은 아직 약하다. |
| Pipeline/Recipe 검토 UX | 부족 | 샘플 Pipeline을 열고 첫 단계로 이동하는 명시적 경로는 생겼다. | step별 input/output/result/expected metric을 한 화면에서 검증하는 경험은 아직 부족하다. |
| 회귀 검증 체계 | 좋음 | screenshot smoke와 actual EXE smoke가 주요 UX 회귀를 잡기 시작했다. | 도구별 guide 확장 시 before/after 이미지 비교와 “자동 실행 없음” 검증을 반드시 같이 추가해야 한다. |

## 제품 방향 결정

- OpenVisionLab은 wizard-only 제품이 아니라 실제 작업 가능한 rule-based vision workbench로 유지한다.
- 초보자 친화성은 PropertyGrid를 제거해서 얻지 않는다. PropertyGrid 위/아래에 guide, result explanation, next action presenter를 얹어서 얻는다.
- Preview와 Pipeline Add는 계속 분리한다. Boolean visibility toggle, sample open, guide 표시만으로 Preview/Run이 실행되면 안 된다.
- Output layer 생성은 input route를 자동 변경하지 않는다.
- 도킹 레이아웃은 새 기능 확장보다 안정 계약과 smoke 유지가 우선이다.
- 샘플은 “열기”에서 끝나지 않고 `샘플 선택 -> 이미지/파이프라인 준비 -> 첫 단계 확인 -> Preview -> 결과 해석 -> Pipeline Review`로 이어져야 한다.

## 다음 구현 우선순위

### P1. Contour 도킹 Tool View teaching guide 확장

다음 구현은 Matching에서 검증한 `VisionToolVerificationGuideView` 계열을 Contour에 확장하는 것이 가장 합리적이다.

이유:

- 초보자 샘플 흐름에서 Threshold/Morphology/Contour는 이해하기 쉬운 첫 검사 루프다.
- Contour는 Count, Area, Bounding Box/Outline 같은 검증 지표가 명확하다.
- Matching보다 단순해서 공통 guide 모델을 안정적으로 일반화하기 좋다.
- PropertyGrid 기반 구조를 유지하면서 guide/result presenter만 추가할 수 있다.

구현 기준:

- Contour Tool View는 PropertyGrid를 유지한다.
- guide는 display-only다. 표시되거나 접혀도 Preview/Run/Add Pipeline을 실행하지 않는다.
- guide는 최소한 다음 정보를 보여준다.
  - 현재 검증 단계: 입력 이미지 확인, threshold/range 확인, contour result 확인, Pipeline 추가 여부
  - 합격 기준: contour count, area range, draw mode, ROI 적용 상태
  - 다음 행동: Preview 실행, area/threshold 조정, Pipeline 추가
- 결과 설명은 `OK/NG`, count, area range 대비 결과, 실패 시 조정 후보를 포함한다.
- 도킹 inspector 공간을 과도하게 차지하지 않는다.

필요 smoke:

- before: 기존 `wpf_shell_host_contour_tool` 캡처
- after: 새 `wpf_shell_host_contour_tool_docked_verification` 또는 동등 target
- 검증 항목:
  - guide가 보인다.
  - PropertyGrid가 계속 보인다.
  - Preview/Run/Add Pipeline 버튼이 명시적 액션으로 남아 있다.
  - guide 표시만으로 output layer, input route, Preview 실행이 바뀌지 않는다.
  - before/after 이미지를 작업 완료 보고에 함께 첨부한다.

2026-07-01 진행 상태:

- P1은 1차 구현 완료.
- `Contour` Tool View에 compact verification guide와 result guidance를 추가했다.
- PropertyGrid, 명시적 Preview/Run/Add Pipeline, input/output route 분리 계약은 유지했다.
- 다음 UX 구현 우선순위는 P2의 Blob guide 확장이다.

### P2. Blob/Line/EdgeBasedMatching으로 guide 확장

Contour에서 공통 모델이 안정되면 Blob과 Line에 확장한다. EdgeBasedMatching은 Matching 계열이지만 고급 옵션이 많으므로, Contour/Blob/Line의 단순 guide 계약을 먼저 고정한 뒤 적용한다.

2026-07-01 진행 상태:

- Blob은 1차 구현 완료.
- Blob/Contour는 `VisionToolAreaVerificationGuidePresenter`와 `VisionToolAreaVerificationCriteriaText`를 공유한다.
- 다음 구현 우선순위는 Line guide 확장이다.

### P3. Pipeline Review를 샘플 학습 루프의 중심으로 격상

샘플을 연 뒤 Pipeline Review가 다음을 보여줘야 한다.

- step list와 현재 선택 단계
- step별 input/output layer
- expected metric과 actual metric
- Preview와 Pipeline 확정의 차이
- 실패 시 다음 조정 후보

중요: Pipeline Review 진입이나 step 선택은 Preview/Run을 자동 실행하면 안 된다.

### P4. Tool List scanability 개선

왼쪽 Tool List는 유지하되 초보자용 탐색을 개선한다.

- 최근 사용 도구
- 샘플에서 다음으로 사용할 도구 강조
- 검색 또는 즐겨찾기
- 카테고리 밀도 정리

이 작업은 알고리즘 도구 구조를 건드리지 않고 ShellHost/ViewModel 계층에서 진행해야 한다.

## UI 변경 작업 체크리스트

모든 UX 변경은 아래 형식으로 남긴다.

```text
UI 비교

- 이전:
  - artifact path:
  - 검증 target:
  - 문제:

- 이후:
  - artifact path:
  - 검증 target:
  - 개선:

- 회귀 확인:
  - PropertyGrid 유지:
  - Preview/Run 자동 실행 없음:
  - Output 생성이 Input 자동 변경하지 않음:
  - zoom/pan/drag/ROI/template/docking 유지:
```

## 다음 작업 시작 문장

다음 Codex 작업은 아래 문장으로 시작하면 된다.

```text
Contour 도킹 Tool View에 Matching에서 검증한 compact verification guide/result explanation 패턴을 확장한다.
PropertyGrid 기반 구조는 유지하고, guide는 display-only로 둔다.
before/after screenshot smoke를 생성하고, guide 표시만으로 Preview/Run/output/input route가 바뀌지 않는지 검증한다.
```
