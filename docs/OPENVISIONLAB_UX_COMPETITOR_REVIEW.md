# OpenVisionLab UX Competitor Review

Updated: 2026-06-30

이 문서는 OpenVisionLab의 현재 메인 워크벤치와 툴 도킹 검증 UX를, 주요 rule-based / industrial vision 제품의 공식 자료와 비교해 개선 방향을 정리한 문서입니다.

## 1. 검토 질문

이번 검토의 질문은 두 가지입니다.

- 툴별로 오른쪽 또는 옆에 Tool View를 도킹한 상태에서 이미지를 보며 검증하는 UX가 충분히 좋은가?
- OpenVisionLab의 전체 메인뷰가 초보자도 rule-based vision을 쉽게 익힐 수 있는 구조인가?

## 2. 참고한 공식 자료

- [Cognex In-Sight Explorer Development Environments](https://docs.cognex.com/is_621/web/EN/ise/Content/GettingStarted/DevEnvironment.htm)
- [MVTec MERLIC](https://www.mvtec.com/products/merlic)
- [MVTec HDevelop](https://www.mvtec.com/products/halcon/work-with-halcon/hdevelop)
- [NI Vision Builder for Automated Inspection](https://www.ni.com/en/shop/electronic-test-instrumentation/application-software-for-electronic-test-and-instrumentation-category/what-is-vision-builder-for-automated-inspection.html)
- [Aurora Vision Studio Learning Documentation](https://docs.adaptive-vision.com/5.6/studio/introduction/HowToLearn.html)
- [KEYENCE XG-X Series](https://www.keyence.com/products/vision/vision-sys/xg-x/)

## 3. 경쟁 제품에서 확인한 공통 UX 패턴

| 제품 | 확인한 UX 패턴 | OpenVisionLab에 가져올 원칙 |
| --- | --- | --- |
| Cognex In-Sight EasyBuilder | 이미지 중심 인터페이스, 단계형 application steps, 이미지 위 interactive region, 즉시 feedback, results table, help/IO monitoring | 초보자용 흐름은 Tool List가 아니라 `이미지 로드 -> 위치/ROI/Template -> 검사 -> 결과 -> 출력` 단계로 읽혀야 한다. |
| MVTec MERLIC | no-code, image-centered operating concept, standardized tool 조합, 이미지 위에서 parameter와 processing step을 확인하는 구조 | PropertyGrid는 유지하되, 현재 보고 있는 이미지와 파라미터 변경의 관계를 화면에서 바로 설명해야 한다. |
| NI Vision Builder AI | menu-driven camera/image processing/inspection step 구성, 결과 생성, benchmark로 요구 조건 검증 | 툴 실행 결과는 단순 숫자 나열이 아니라 `요구 조건 충족 여부`, `실패 이유`, `재검증 방법`을 보여줘야 한다. |
| Aurora Vision Studio | drag-and-drop visual programming, examples/tutorials/filter theory learning material | 초보자용 sample/tutorial은 별도 문서가 아니라 실제 Tool View와 연결되는 학습 흐름이어야 한다. |
| KEYENCE XG-X | flowchart style programming, 빠른 개발, UI creation, debugging, simulation | Pipeline/Recipe는 최종적으로 단계별 image/result/debug를 빠르게 왕복 확인할 수 있어야 한다. |
| MVTec HDevelop | interactive execution, graphics window, program window, variable inspection, profiler | 고급 사용자를 위한 diagnostic/profiler 영역은 필요하지만, 초보자 기본 화면에 모두 노출하면 안 된다. |

## 4. 현재 OpenVisionLab 자체평가

### 강점

- 중앙 workspace가 이미지 레이어와 도킹 비교를 중심으로 구성되어 제품 정체성과 맞습니다.
- 오른쪽 Tool View 도킹 상태에서 `입력 레이어`, `출력 레이어`, `Template`, `PropertyGrid`, `Result`, `Pipeline 추가`, `Preview 실행`이 한 화면에 존재합니다.
- Matching 예시 기준으로 score, count, center, box, angle, tact 같은 vision 검증 지표가 이미 노출됩니다.
- PropertyGrid 기반 구조 덕분에 새 알고리즘 속성을 추가해도 UI가 자동 생성되는 확장성이 있습니다.
- 로그, 상태 배너, layer status가 있어 실행 결과 추적의 기반이 있습니다.

### 약점

- 초보자 기준으로 다음 행동이 명확하지 않습니다. 화면에는 기능이 많지만 `먼저 무엇을 해야 하는지`가 시각적으로 정렬되어 있지 않습니다.
- PropertyGrid가 전문가에게는 효율적이지만, 초보자에게는 어떤 파라미터가 핵심이고 어떤 파라미터가 고급 옵션인지 구분이 약합니다.
- 결과 영역이 숫자 중심입니다. `왜 OK인지`, `왜 NG인지`, `어떤 값을 조정해야 하는지`가 결과 카드에서 즉시 읽히지 않습니다.
- 왼쪽 Tool List, 중앙 Layer Workspace, 오른쪽 Tool View, 하단 Log가 모두 강하게 보이기 때문에 화면 밀도가 높습니다.
- 하단 Log는 중요하지만 기본 상태에서는 이미지 검증 공간을 많이 차지합니다. 오류가 없을 때는 접힌 요약 상태가 더 적합합니다.
- `Preview`, `Output Layer`, `Pipeline 추가`의 관계는 안정 계약으로는 분리되어 있지만, 초보자 화면에서는 그 차이가 더 명확하게 표현되어야 합니다.

## 5. 제품 방향 결정

### 유지할 방향

- OpenVisionLab은 landing page나 단순 wizard가 아니라 실제 작업 가능한 rule-based vision workbench로 유지합니다.
- 알고리즘 Tool View는 PropertyGrid 기반을 유지합니다.
- 중앙은 이미지와 레이어 비교가 주역이어야 합니다.
- 오른쪽 도킹 Tool View는 툴 검증의 주 작업면으로 유지합니다.
- Preview와 Pipeline 확정은 계속 분리합니다.
- Output 레이어 생성이 Input 레이어를 자동 변경하지 않는 안정 계약은 유지합니다.

### 보강할 방향

- PropertyGrid 위에 초보자용 `검증 흐름 레이어`를 얹습니다. PropertyGrid를 없애지 않고, 툴별 teaching 순서와 결과 해석을 보조합니다.
- Tool View는 `입력/출력`, `가르치기`, `핵심 파라미터`, `고급 파라미터`, `결과 해석`, `다음 행동` 순서로 읽혀야 합니다.
- 결과 영역은 숫자뿐 아니라 판정 문장과 실패 이유를 표시해야 합니다.
- Main View는 첫 실행/이미지 없음 상태에서 `샘플 열기`, `이미지 열기`, `최근 Recipe`, `가이드 시작`을 중앙에 명확히 보여야 합니다.
- 하단 Log는 기본적으로 요약/접힘 상태를 우선하고, 실패/경고가 있을 때 확장 유도합니다.

## 6. 툴 도킹 검증 UX 목표

오른쪽에 Tool View를 도킹하고 이미지를 보면서 검증하는 화면은 다음 구조가 목표입니다.

```text
좌/중앙: 이미지 레이어 workspace
  - Main / Preview / Output 비교
  - ROI, template, result overlay
  - zoom, pan, drag, GV/status 유지

오른쪽: 도킹 Tool View
  1. Tool header
     - 툴 이름, 현재 상태, dock/float/close
  2. Input / Output cards
     - 입력 레이어와 출력 레이어를 명확히 분리
     - 출력 생성 버튼은 출력 레이어 영역에 배치
  3. Teach card
     - ROI/template/status
     - 현재 sample/template이 준비됐는지 표시
  4. Beginner parameters
     - 초보자가 먼저 조정할 핵심 파라미터 3~5개
  5. Advanced PropertyGrid
     - 기존 PropertyGrid 전체 설정
     - 고급 항목은 접힘 그룹으로 시작 가능
  6. Result explanation
     - OK/NG, count/score/box/angle/tact
     - threshold 대비 actual value
     - 실패 시 다음 조정 후보
  7. Sticky actions
     - Preview 실행
     - Pipeline 추가
```

이 구조는 Matching을 첫 구현 대상으로 삼는 것이 적합합니다. Matching은 template, ROI, angle/scale, score, result overlay, pipeline 확정까지 거의 모든 핵심 UX 요소를 포함하기 때문입니다.

## 7. 메인뷰 UX 목표

메인뷰는 초보자에게 다음 질문의 답이 즉시 보여야 합니다.

- 지금 이미지를 열었는가?
- 어떤 툴을 선택했는가?
- 이 툴은 어떤 입력 레이어를 보고 있는가?
- Preview는 됐는가, Pipeline에 확정됐는가?
- 결과가 OK/NG인 이유는 무엇인가?
- 다음에 누를 버튼은 무엇인가?

권장 개선 방향은 다음과 같습니다.

- 이미지가 없을 때 중앙 workspace에 `이미지 열기`, `샘플 열기`, `최근 Recipe`, `튜토리얼 시작` 선택지를 표시합니다.
- 왼쪽 Tool List는 category 유지 + 검색 + 즐겨찾기 + 최근 사용을 추가합니다.
- 툴 선택 시 오른쪽 Tool View가 `대기`, `준비 필요`, `Preview OK`, `Pipeline 추가 가능` 같은 상태를 명확히 표시합니다.
- 하단 Log는 기본 접힘 요약으로 두고, 실패/경고/상세 디버그가 필요할 때 확장합니다.
- 상단 상태 배너는 실행 결과만 간결히 표시하고, 상세 결과는 Tool View의 Result explanation에 둡니다.
- `가이드` 탭은 별도 문서 링크보다 현재 선택한 툴/상태에 맞는 contextual guide로 발전시키는 것이 좋습니다.

## 8. 구현 우선순위

### P0. UX 방향 문서화

- 이 문서를 기준으로 Tool View와 Main View의 개선 방향을 고정합니다.
- 안정 계약과 충돌하는 변경은 하지 않습니다.

### P1. Matching 도킹 Tool View UX 개선

- Matching Tool View를 첫 기준 화면으로 삼습니다.
- PropertyGrid는 유지합니다.
- PropertyGrid 위/아래에 `검증 흐름`, `결과 해석`, `다음 행동`을 추가합니다.
- 결과 카드에는 최소한 다음을 표시합니다.
  - 판정: OK/NG/대기
  - 이유: score/count/threshold 비교
  - 위치: center/box/angle/scale
  - 시간: tact
  - 다음 행동: Preview 재실행, threshold 조정, Pipeline 추가

2026-06-30 첫 구현:

- Matching/EdgeMatching 계열 result review presenter가 Preview OK/NG, Criteria, reason, next action guidance를 표시합니다.
- 도킹 inspector에서는 결과 칩을 숨기고 compact summary/guidance 텍스트만 보여 PropertyGrid 편집 공간을 보존합니다.
- Floating tool에서는 기존 result chips를 유지하면서 Decision/Criteria chips를 추가합니다.
- 새 smoke target `wpf_shell_host_matching_tool_docked_verification`이 Matching 도킹 Tool View, result guidance, PropertyGrid 최소 편집 공간을 검증합니다.

2026-06-30 2차 구현:

- Matching 계열 Tool View에 재사용 가능한 compact verification guide를 추가했습니다.
- guide는 PropertyGrid 위에서 `검증 흐름`, `Preview OK/NG`, `합격 기준`, `다음` 행동을 표시합니다.
- guide는 표시 전용이며 Preview/Run, 레이어 라우팅, PropertyGrid 모델 구조를 변경하지 않습니다.
- `wpf_shell_host_matching_tool`과 `wpf_shell_host_matching_tool_docked_verification` smoke가 guide 문구와 도킹 편집 공간을 함께 검증합니다.

### P2. Main View empty/start workflow 개선

- 이미지 없음 상태에서 workspace가 비어 보이지 않게 합니다.
- `샘플 열기`를 초보자 기본 경로로 올립니다.
- 이미지가 로드되면 중앙 workspace와 오른쪽 tool state가 자연스럽게 연결되어야 합니다.

### P3. Tool 공통 teaching guide 구조

- Matching에서 검증한 구조를 Blob, Contour, Line, EdgeBasedMatching으로 확장합니다.
- View code-behind에 직접 로직을 넣지 말고 presenter/viewmodel/controller로 분리합니다.
- 툴별 문구와 핵심 파라미터 목록은 공통 모델 또는 metadata로 제공하는 방향이 좋습니다.

### P4. Pipeline/Recipe review UX

- Pipeline step별 input/output/result/metric을 한 화면에서 검토할 수 있게 합니다.
- Flowchart 또는 step timeline 형태의 review UX를 검토합니다.
- `Preview`와 `Publish/Pipeline 추가`의 차이를 계속 명확히 유지합니다.

## 9. UX 수용 체크리스트

툴 도킹 검증 UX 변경은 아래 질문에 통과해야 합니다.

- 이미지를 보면서 파라미터를 조정할 수 있는가?
- Tool View가 이미지를 과도하게 가리지 않는가?
- Input layer와 Output layer가 명확히 분리되어 있는가?
- Output layer 생성이 Input layer를 자동 변경하지 않는가?
- Boolean visibility toggle만으로 Preview/Run이 실행되지 않는가?
- Preview와 Pipeline 추가의 차이가 화면상으로 명확한가?
- 결과가 OK/NG인 이유를 숫자와 문장으로 확인할 수 있는가?
- 초보자가 다음 행동을 찾기 위해 로그나 문서를 뒤지지 않아도 되는가?
- PropertyGrid 기반 확장성이 유지되는가?
- Viewer zoom/pan/drag, ROI overlay, template editor, layer docking이 유지되는가?

## 10. 검증 방향

문서 이후 실제 구현이 들어가면 최소한 다음 smoke가 필요합니다.

- `wpf_shell_host_matching_tool_docked_verification`
  - 이미지가 보이는 상태에서 Matching Tool View가 도킹되어 있어야 합니다.
  - 입력/출력 레이어, template status, PropertyGrid, result explanation, Preview, Pipeline 추가가 보이는지 확인합니다.
- `wpf_shell_host_main_empty_start_workflow`
  - 이미지 없음 상태에서 이미지/샘플/최근 recipe 진입점이 보이는지 확인합니다.
- `wpf_shell_host_tool_result_explanation`
  - Preview 후 OK/NG 이유와 핵심 metric이 결과 카드에 표시되는지 확인합니다.

실제 EXE UX 확인도 필요합니다. 화면 개선은 빌드 성공만으로 완료 판단하지 않습니다.
