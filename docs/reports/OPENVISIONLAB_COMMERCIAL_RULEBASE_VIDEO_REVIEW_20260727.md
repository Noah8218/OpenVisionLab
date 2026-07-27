# OpenVisionLab Commercial Rule-Based Vision Video Review

- Date: 2026-07-27 KST
- Workspace: `C:\Git\OpenVisionLab_Dev`
- Repository baseline: `9d7fa796ed94d90e50d840607b441a2954278947`
- Media evidence: `artifacts\commercial_rulebase_video_review_20260727`

## 1. Executive conclusion

OpenVisionLab의 현재 정체성은 분명하다.

> OpenVisionLab은 OpenCvSharp4 기반의 **결정론적 룰베이스 비전 레시피 워크벤치**다. 작업자는 샘플 이미지, 검사 의도, ROI, 허용 범위와 증거를 가르치고, PropertyGrid에서 도구를 설정하며, 명시적으로 Preview/Run을 실행하고, 레이어·도형·수치·N-sample 결과를 비교한 후 검증된 레시피를 저장한다.

LLM은 유지보수 모드의 선택적 XML 작성 보조 수단이다. 카메라, 조명, PLC, I/O, 계정, 배포, 규제 준수 플랫폼은 제품 범위가 아니다.

16개 영상과 현재 프로젝트를 비교한 핵심 결론은 다음과 같다.

1. OpenVisionLab은 이미 단순 도구 모음 단계를 지났다. PropertyGrid teaching, 명시적 실행, 파이프라인, 레이어 비교, Fixture/상대 ROI, 객체별 결과, 기하 측정, N-image 검증, Validation Set, Run History, Qualified Recipe Snapshot이 하나의 검증 흐름으로 연결되어 있다.
2. 현재 강점은 상용 제품의 “많은 알고리즘”이 아니라 **근거를 잃지 않는 작업 흐름**이다. 특히 예상 OK/NG와 실제 Pipeline 결과, 판단 정오, 실행 오류를 구분하고 해시·도형·리뷰 큐를 보존하는 부분은 튜토리얼 영상에서 보인 단순 데모보다 엄격하다.
3. 상용 제품에서 가장 배울 가치가 큰 미비점은 **파라미터를 왜 그렇게 정했는지 보여 주는 대화형 신호/분포 진단**이다. Cognex의 edge-response chart, HALCON의 gray/feature histogram, metrology measure-point 시각화가 같은 방향을 가리킨다.
4. 그러나 최신 프로젝트 감사 결과에는 현재 재현된 운영자 차단 문제가 없다. 따라서 새로운 기능 개발보다 먼저 독립 초보 사용자 평가를 수행하고, 동일 전환에서 반복 실패하거나 실제 튜닝 작업이 현재 UI에서 막히는 증거가 있을 때만 후보를 활성화해야 한다.
5. MERLIC의 범용 easyTouch, HALCON의 스크립트 언어/디버거, Cognex의 ML 분류, 카메라 캘리브레이션, 계정·감사 추적은 현재 정체성에 맞지 않거나 아직 요구 근거가 없다. 영상에 존재한다는 이유만으로 복제하면 제품이 흐려진다.

## 2. 분석 범위와 방법

### 2.1 프로젝트 기준선

- `AGENTS.md`와 현재 handoff/contract 문서를 먼저 확인했다.
- `docs` 아래 Markdown 303개를 전체 경로, 제목, 상태, 갱신일, 키워드 기준으로 인덱싱했다.
- 문서 권위 순서는 `AGENTS.md` → stable contracts/policies → product target/current handoff → current audits → chronological evidence/history 순으로 적용했다.
- 경로 이동 안내 문서는 canonical 위치를 따라가 실제 문서를 확인했다.
- 현재 도구 카탈로그는 2026-07-23 기준 canonical 24개 도구 family와 45개 허용 ToolType 이름/alias를 기록한다.
- 다음 최신 문서를 현재 판단의 주요 근거로 사용했다.
  - `docs\reports\OPENVISIONLAB_FIRST_TIME_OPERATOR_JOURNEY_AUDIT_20260727.md`
  - `docs\reports\OPENVISIONLAB_RULE_BASED_UI_GAP_AUDIT_20260723.md`
  - `docs\contracts\openvisionlab\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`
  - `docs\contracts\openvisionlab\OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md`
  - `docs\contracts\openvisionlab\OPENVISIONLAB_LLM_TOOL_CATALOG.json`
  - `docs\contracts\openvisionlab\OPENVISIONLAB_PUBLIC_SAMPLE_ASSET_POLICY.md`
  - `docs\contracts\openvisionlab\OPENVISIONLAB_EXTERNAL_REFERENCE_POLICY.md`
  - `docs\contracts\openvisionlab\OPENVISIONLAB_RELEASE_VERSION_POLICY.md`

과거 문서의 준비도 백분율은 역사적 또는 특정 범위 추정치로만 취급했다. 현재 문서는 하나의 전체 완성률을 선언하지 않는다.

### 2.2 영상 근거

- 16개 MP4, 총 1시간 24분 32초를 검토했다.
- 각 영상에서 시간 분산 16프레임, 총 256개 대표 프레임을 추출해 화면 상태를 확인했다.
- 제공 자막이 있는 01, 02, 03, 05, 07, 08, 09, 10은 자막과 화면을 함께 검토했다.
- 자막이 없는 04, 06, 11~16은 로컬 `faster-whisper base.en` 전사를 탐색 보조로 사용하고 화면으로 보정했다.
- 자동 전사는 고유명사와 연산자 이름을 오인할 수 있으므로, 세부 판단은 화면·워크플로·현재 소스 계약과 교차 확인했다.
- 영상 파일 SHA-256, duration, storyboard, 전사 경로는 `artifacts\commercial_rulebase_video_review_20260727\README.md`에 기록했다.

### 2.3 비교 원칙

각 영상에 대해 다음을 분리했다.

1. 영상이 실제로 보여 준 작업 흐름
2. OpenVisionLab에 이미 있는 대응 기능
3. 상용 제품에서 배울 설계 원리
4. 현재 제품 범위에 맞는 실제 미비점
5. 채택, 조건부 검토, 보류, 범위 외 결정

## 3. 현재 OpenVisionLab 기준선

### 3.1 현재 제품 흐름

```text
샘플/운영자 이미지
  -> Learn 또는 직접 Tool View teaching
  -> PropertyGrid 파라미터와 ROI
  -> Recipe/Pipeline 구성
  -> 명시적 Preview 또는 Run Review
  -> 레이어, 도형, 수치, 객체 결과 비교
  -> N-image 또는 Validation Set
  -> Run History와 결정론적 리뷰 큐
  -> 검증된 Qualified Recipe Snapshot
```

### 3.2 이미 잘 연결된 영역

- 이미지 중심 작업 공간, zoom/pan/pixel status, named layer와 명시적 input/output routing
- PropertyGrid 기반 Blob, Contour, Line, Matching, EdgeBasedMatching, FeatureMatching 등
- Threshold, Morphology, Filter, Edge, Mean, HSV, Arithmetic, ReferenceDifference, OverlayMerge
- LineDistance, PinArrayGap, CircleGauge, GeometryMeasure
- RotateScale, 3-point AffineTransform, typed Point 기반 동적 Affine source
- Matching Fixture reference teach, NormalizeImage, downstream relative ROI review
- Blob/Contour 객체 행과 drawing 양방향 선택, width/height reject reason
- explicit Run Review, step input/output, NG reason, metrics, elapsed time
- N-image Tool View 검증, source/result/report 보존, hash, review queue
- recipe-local labelled Validation Set, 예상/실제/판단/오류 분리
- immutable Qualified Recipe Snapshot, fingerprint/inventory 검증, working copy, supersede/revoke

### 3.3 현재 성숙도 표현

단일 백분율보다 다음 표현이 정확하다.

| 영역 | 현재 평가 | 근거와 한계 |
| --- | --- | --- |
| 제품 정체성/범위 통제 | 강함 | 룰베이스 teaching과 증거 검토가 중심이며 LLM·하드웨어 범위를 명시적으로 제한 |
| 단일 도구 teaching | 강함 | PropertyGrid, ROI, 명시적 Preview, result drawing, parameter persistence |
| recipe/pipeline review | 강함 | route, step evidence, NG, fixture, geometry, object result가 연결됨 |
| 로컬 검증/증거 보존 | 매우 강함 | labelled set, explicit outcomes, hash, drawings, queue, immutable snapshot |
| matching/fixture 알고리즘 | 중상 | angle/scale/unique/hybrid/affine 기능은 넓지만 증거는 선택된 synthetic/public/local corpus에 한정 |
| metrology | 중간 | pixel geometry와 2-point uniform scale은 있으나 lens calibration·distortion·certified metrology는 없음 |
| parameter tuning explainability | 중간 | overlay와 metrics는 강하지만 histogram/edge-response/measure-signal chart가 통합되어 있지 않음 |
| 초보자 자립성 | 미확정 | current-source 여정 감사 통과, 독립 초보 참여자 연구는 아직 없음 |
| 산업 현장/필드 qualification | 미입증 | acquisition variation, certified calibration, deployment, field robustness 범위를 주장하지 않음 |

현재 상태는 **가이드가 있는 샘플 기반 룰베이스 작업에는 사용 가능한 검증 워크벤치**다. 독립 초보 자립, 계측 인증, 현장 견고성, 운영 배포까지 완료된 산업 플랫폼은 아니다.

## 4. 영상별 상세 분석

## 4.1 01 Cognex VisionPro QuickBuild Demo

### 영상에서 확인한 흐름

- QuickBuild의 그래픽 작업 공간에서 도구를 순서대로 배치하고 출력과 입력을 연결한다.
- PMAlign에서 PatMax 계열 locator를 사용해 공통 부품 특징을 train한다.
- pattern ROI와 origin을 지정하고, 회전 허용 범위, 점수, coarse/fine feature를 조정한다.
- fixture 도구가 PMAlign pose를 받아 downstream 검사가 동일 좌표계에서 작동하도록 한다.
- cavity ROI를 Classify 도구에 가르치고 여러 이미지와 여러 cavity 위치를 학습 표본으로 추가한다.
- double/mix/missing 등의 클래스를 지정하고 잘못된 예측을 수동으로 교정한다.
- classifier를 네 위치에 복제하고 graphic label을 연결해 각 cavity 결과를 표시한다.
- 더 큰 테스트 이미지 세트로 결과를 확인한다.

### OpenVisionLab과 비교

- 그래픽 step flow, Matching → Fixture → downstream ROI, 명시적 결과 검토는 Pipeline Review와 P212/P219 계열에 대응한다.
- OpenVisionLab은 pipeline route와 저장된 ROI, source/normalized drawing, metrics, hash evidence를 더 엄격히 보존한다.
- 영상의 classifier는 학습 기반 기능이며 현재 OpenVisionLab의 결정론적 룰베이스 정체성과 다르다.
- OpenVisionLab에는 네 classifier 복제 같은 dataflow 편의보다 recipe step과 layer route가 중심이다.

### 판단

- **채택할 원리:** locator pose가 downstream 검사 좌표를 일관되게 지배하고, 각 단계가 선택 시 동일 image/result/parameter context를 보여 주는 것.
- **채택하지 않을 범위:** 일반 ML classifier와 label-training 환경.
- **현재 미비점:** 다수 downstream ROI가 하나의 typed fixture transform을 직접 소비하는 범용성은 현재 bounded chain보다 약하다. 실제 다중 ROI 작업이 P212/P219로 표현되지 않을 때만 개발 후보로 본다.

## 4.2 02 Cognex VisionPro PMAlign Tool

### 영상에서 확인한 흐름

- gray template가 아니라 edge/shape feature로 위치와 회전을 찾는다.
- PatMax는 정확도와 세부 feature, PatQuick은 속도와 변화 허용, PatFlex는 곡면·비평면·유연한 부품을 강조한다.
- pattern ROI와 origin을 teach하며 실제 Good 이미지가 없으면 synthetic shape로도 pattern을 만들 수 있다.
- polarity ignore, elasticity, coarse/fine feature grain을 조정한다.
- runtime에서 result count, accept score, clutter, angle, uniform/non-uniform scale, multi-result overlap을 설정한다.
- search ROI로 오검출과 실행 시간을 줄인다.
- coarse/fine model edge와 match overlay를 화면에 표시하고 fixture로 연결한다.

### OpenVisionLab과 비교

- EdgeBasedMatching은 score, count, Canny, template point, angle, coarse-to-fine, scale, search step, position/subpixel refine, greediness, pyramid proposal, hybrid verify, unique-result margin을 이미 제공한다.
- Auto MPoint는 후보를 분석하되 자동 선택하지 않고 operator review 후 명시적으로 적용한다.
- OpenVisionLab의 장점은 ambiguity를 단일 best score로 숨기지 않고 `Success/NoMatch/Ambiguous`와 대안 margin을 보존하는 것이다.
- Cognex 영상에 보이는 explicit polarity modes, elasticity/deformation, X/Y non-uniform scale, overlap control은 동일 수준으로 제공하지 않는다.

### 판단

- **잘된 부분:** 현재 OpenVisionLab matcher는 룰베이스 학습·ambiguity·결과 증거 면에서 이미 충분히 깊다.
- **미비점:** trained edge model, pyramid level, 후보별 score/margin과 search cost를 한 화면에서 설명하는 진단 시각화가 약하다.
- **조건부 항목:** polarity/deformation을 실제 corpus 실패가 증명하기 전에는 추가하지 않는다. 단순 parameter parity는 개발 근거가 아니다.

## 4.3 03 HALCON HDevelop GUI and Navigation

### 영상에서 확인한 흐름

- Program, Graphics, Variables, Operator 창을 분리한다.
- Run, Step, Stop, Reset, Step Over, breakpoint, line activate/deactivate를 제공한다.
- Variables 창에서 iconic image/region/contour와 control number/string을 구분한다.
- 변수를 더블 클릭해 graphics window에 표시하거나 지운다.
- Operator 검색과 parameter dialog가 input/output, 설명, 도움말을 보여 준다.

### OpenVisionLab과 비교

- OpenVisionLab은 프로그래밍 IDE가 아니라 recipe workbench이므로 source line breakpoint나 general variable debugger는 필요하지 않다.
- 대신 Pipeline Review가 selected Step의 route, input/output preview, metrics, elapsed, drawing을 보여 준다.
- typed geometry result와 object row도 이미 결과 종류를 구분한다.

### 판단

- **채택할 원리:** 선택한 단계, 변수/결과, 이미지 overlay가 항상 같은 context를 가리키고, 도움말이 그 context에서 열리는 것.
- **채택하지 않을 범위:** HALCON식 범용 스크립트 언어와 디버거.
- **평가:** 현재 selected-Step coherence는 강하지만, parameter가 어떤 pixel signal에 반응했는지를 보여 주는 진단이 더 필요하다.

## 4.4 04 HALCON HDevelop Variables

### 영상에서 확인한 흐름

- image read, ROI/domain 축소, threshold, connected region, object count, contour를 순서대로 만든다.
- iconic variable은 image, domain, region, connected-component array, XLD contour array를 담는다.
- control variable은 number, string, handle, tuple을 담는다.
- operator가 object array와 tuple을 직접 받아 불필요한 loop를 줄인다.
- iconic object index와 control tuple index 규칙 차이를 설명한다.

### OpenVisionLab과 비교

- OpenVisionLab은 general iconic/control variable 언어를 제공하지 않는다.
- 대신 named layers, Step metrics, stable object rows, typed Point/Segment/Circle feature가 제한된 타입 계약을 제공한다.
- 이 제한은 recipe validation과 fail-closed behavior를 단순하게 유지하는 장점이 있다.

### 판단

- **잘된 부분:** 결과 타입과 source Step identity가 검증되어 arbitrary tuple보다 recipe evidence에 적합하다.
- **보류:** general tuple/expression/data language. 현재 요구 없이 추가하면 제품이 HDevelop clone으로 흐른다.
- **배울 점:** object-array 결과를 더 잘 탐색하는 방향은 P211 Object Results Inspector에서 이미 구현됐다.

## 4.5 05 HALCON HDevelop Visualization

### 영상에서 확인한 흐름

- 저대비 영상에서 LUT를 바꾸고 square-root LUT로 어두운 영역을 강조한다.
- GUI 조작을 code로 기록하고 aspect ratio를 유지하는 window 생성 코드를 삽입한다.
- message text, font, line break를 정의한다.
- color, fill/margin, line width를 조정한다.
- image coordinate와 window coordinate를 구분해 image-bound label 위치를 보정한다.

### OpenVisionLab과 비교

- OpenVisionLab은 zoom/pan/aspect 유지, ROI overlay, object/geometry drawing, current-run evidence를 보존한다.
- drawing style은 도구 계약에 의해 제한되어 결과 의미를 안정적으로 유지한다.
- 사용자가 임의 visualization script를 만드는 기능은 없다.

### 판단

- **채택할 원리:** display-only 변경과 algorithm execution을 분리하고 coordinate space를 명확히 하는 것.
- **잘된 부분:** OpenVisionLab은 visibility toggle, overlay, layer action이 Preview/Run을 유발하지 않는 계약이 더 엄격하다.
- **보류:** 범용 visualization scripting과 임의 styling. workflow blocker가 없는 장식적 기능이다.

## 4.6 06 MVTec MERLIC easyTouch

### 영상에서 확인한 흐름

- cursor가 image feature 위를 움직일 때 task에 적합한 geometry를 제안한다.
- green은 유일하고 충분한 edge, red는 약하거나 반복적인 edge를 의미한다.
- click으로 alignment pattern을 선택하고 image sequence를 넘기며 확인한다.
- Measure Circle은 contour를 따라 측정 ROI를 자동 제안하고 EasyTouch Plus로 여러 ROI를 추가한다.
- alignment data가 이미지 변화에 따라 measurement ROI를 이동시킨다.
- OCR 예제에서는 text 영역을 자동 제안한다.

### OpenVisionLab과 비교

- Auto MPoint가 edge pattern 후보를 score와 reject reason으로 제안하고, `Analyze candidates`와 `Use this pattern`을 분리한다.
- CircleGauge는 operator-reviewed annular ROI, polarity, contrast, support, residual을 사용한다.
- Fixture/relative ROI가 downstream ROI 관계를 표시한다.
- 범용 easyTouch와 OCR은 제공하지 않는다.

### 판단

- **잘된 부분:** 후보를 자동 적용하지 않고 operator review와 explicit apply를 요구하는 현재 설계가 제품 정체성에 맞다.
- **미비점:** Circle/Line/Threshold에 대한 task-specific visual teaching assistant가 없지만, 일반화하기 전에 실제 반복 실패가 필요하다.
- **보류:** generic easyTouch와 OCR. 현재 문서가 명시적으로 proactive expansion을 중단했다.

## 4.7 07 MVTec MERLIC Alignment

### 영상에서 확인한 흐름

- matching 기반 alignment와 straight-border 기반 alignment를 구분한다.
- easyTouch의 green/red가 edge uniqueness를 알려 주지만, 반사처럼 물리적으로 불안정한 feature는 operator가 배제해야 한다.
- image sequence에서 no match와 low score를 확인하고 score/rotation range를 조정한다.
- EasyTouch Plus로 두 번째 ROI를 더해 robustness를 높인다.
- Align Image가 alignment data를 받아 normalized image를 만든다.
- 반복 패턴이 강한 sheet에서는 긴 수평·수직 straight border 두 개로 alignment data를 만든다.
- downstream ROI가 alignment data를 직접 받아 전체 이미지를 warp하지 않고 움직일 수도 있다.

### OpenVisionLab과 비교

- Matching fixture, NormalizeImage, fixed reference ROI, typed Point 기반 Affine가 대응한다.
- current fixture designer는 template/search ROI, reference/current pose, score/margin, valid pixels, source polygon, normalized rectangle을 보여 준다.
- P219는 세 개의 앞선 typed Point로 Affine source를 연결한다.
- generic “alignment data consumer” abstraction은 제한된 chain보다 덜 일반적이다.

### 판단

- **잘된 부분:** OpenVisionLab은 pose가 맞다는 것과 downstream physical ROI가 맞다는 것을 별도로 검토한다.
- **조건부 미비점:** 동일 fixture를 다수 검사 Step이 image warp 없이 소비해야 하는 실제 recipe가 현재 방식으로 표현되지 않을 때 generic typed fixture consumer를 검토한다.
- **보류:** 두 번째 locator나 multi-anchor를 자동으로 추가하는 기능.

## 4.8 08 MVTec MERLIC Calibrated Measurement

### 영상에서 확인한 흐름

- wide-angle lens의 barrel distortion이 측정 오차를 만든다는 문제에서 시작한다.
- calibration plate를 자동 검출하고 plate type, camera pose, calibration error를 확인한다.
- calibration error를 0.1 px 미만으로 관리하는 예를 보여 준다.
- Rectify Image가 calibration data를 받아 distortion을 제거한다.
- row spacing을 millimeter로 측정하고 표준편차를 계산해 tolerance를 평가한다.

### OpenVisionLab과 비교

- OpenVisionLab은 same-run 두 Point와 image hash를 이용한 2-point uniform mm-per-pixel teaching을 제공한다.
- LineDistance와 GeometryMeasure가 적용된 scale로 mm metrics를 낸다.
- lens distortion, camera intrinsic/extrinsic calibration, calibration plate, uncertainty budget은 없다.

### 판단

- **현재 한계:** scalar mm/px는 image-plane uniform scale일 뿐 camera calibration이 아니다.
- **범위 외:** lens calibration, distortion correction, certified metrology는 현재 제품 방향에 포함하지 않는다.
- **향후 게이트:** 사용자가 제품 범위를 명시적으로 계측 플랫폼으로 확장하고 calibration hardware/data/acceptance contract를 제공할 때만 별도 프로젝트로 검토한다.

## 4.9 09 Cognex In-Sight 25.1 Charts

### 영상에서 확인한 흐름

- development-only chart로 tool threshold와 반복 성능을 시각화한다.
- edge response chart가 dark-to-light positive peak와 light-to-dark negative valley를 보여 준다.
- histogram chart가 ROI gray-level별 pixel count를 보여 준다.
- compatible tool에서 `Show chart`를 선택하면 chart가 해당 function과 연결된다.
- 여러 chart tab을 유지하며 function selection과 chart selection이 양방향으로 동기화된다.
- chart를 pan/zoom해 threshold 근거를 확인한다.

### OpenVisionLab과 비교

- Learn에는 histogram bar가 있고 Histogram Tool View 이름도 존재하지만, 현재 runtime은 CLAHE/equalize/normalize와 mean/contrast summary 중심이다.
- current-source 검색에서 selected Step/ROI에 연결된 gray histogram 또는 1D edge-response chart contract는 확인되지 않았다.
- LineGauge는 scan line, edge point, fitted line, contrast/polarity를 drawing으로 보여 주지만 signal peak 자체를 plot하지 않는다.

### 판단

- **가장 유력한 제품 gap:** retained Preview/Run evidence에서 선택 ROI/scan line의 intensity distribution과 edge response를 보여 주는 `Tool Signal Inspector`.
- **왜 중요한가:** threshold, polarity, contrast, edge position을 “결과가 나왔다”가 아니라 “어떤 signal 때문에 이 값이 맞는가”로 설명한다.
- **안전 조건:** chart 열기/선택/zoom은 재실행하지 않는다. parameter 변경도 자동 Run을 만들지 않는다. 새 실행은 기존 explicit Preview/Run 규칙을 따른다.

## 4.10 10 Cognex In-Sight 25.1 Validation

### 영상에서 확인한 흐름

- Job validation과 system validation을 분리한다.
- Good/Bad와 다양한 조건의 validation image를 descriptive name으로 등록한다.
- named result variable을 선택하고 예상 값을 지정한다.
- 여러 part style을 variant로 관리한다.
- explicit play로 validation set을 실행하고 pass/fail을 확인한다.
- system validation은 변경 감지, audit trail, user role, SSO를 다룬다.
- 규제 준수는 사용자의 책임이라고 명시한다.

### OpenVisionLab과 비교

- recipe-local Validation Set은 image별 Expected OK/NG, notes, actual OK/NG, judgment correctness, execution error를 분리한다.
- Run History는 batch analytics, baseline comparison, per-Step report, deterministic review queue를 제공한다.
- Qualified Snapshot은 Pipeline, dependencies, source hash, report, drawing, runtime fingerprint를 immutable archive로 보존한다.
- account, SSO, role, regulatory audit trail은 없다.

### 판단

- **잘된 부분:** 로컬 recipe qualification evidence의 내용 무결성과 결과 의미 구분은 매우 강하다.
- **범위 외:** 계정·권한·전자서명·21 CFR system validation.
- **남은 실제 gap:** 독립 사용자가 이 흐름을 올바르게 수행할 수 있는지에 대한 참여자 evidence.

## 4.11 11 MVTec MERLIC Application in Five Minutes

### 영상에서 확인한 흐름

- missing fuse inspection을 task로 정의한다.
- image sequence source와 Execute Once로 각 단계를 확인한다.
- task-oriented tool library에서 `Check Presence with Gray Features`를 선택하고 image가 자동 연결된다.
- Good fuse ROI와 bad empty-socket ROI를 train한다.
- search ROI를 정의하고 image sequence에서 accepted/confidence를 확인한다.
- `Region Accepted` tuple의 합과 길이로 missing count를 계산한다.
- online help와 optional frontend 단계로 이어진다.

### OpenVisionLab과 비교

- sample catalog, Learn path, Guided Setup, PropertyGrid, Blob/ReferenceDifference/Matching, metrics acceptance가 task workflow를 제공한다.
- N-image Tool View와 Validation Set이 image sequence 검증을 담당한다.
- generic tuple expression engine은 없지만 현재 검사에서는 named metric acceptance가 같은 판단을 더 제한적으로 수행한다.

### 판단

- **채택할 원리:** task-oriented entry, representative Good/Bad teaching, 즉시 여러 sample 확인, 다음 행동 안내.
- **보류:** generic expression engine. arbitrary expression은 validation과 안전성을 복잡하게 하며 현재 named metrics로 해결되는 작업이 많다.
- **평가:** OpenVisionLab은 설정 시간이 더 길 수 있지만 증거와 저장 계약은 더 강하다.

## 4.12 12 HALCON Shape-Based Matching Introduction

### 영상에서 확인한 흐름

- model ROI/domain을 만들고 shape model을 생성한다.
- find shape model이 row, column, angle, score를 반환한다.
- result contour를 변환해 image에 overlay한다.
- contour feature 기반이라 조명 변화에 비교적 강하다고 설명한다.
- angle range, result count, minimum score가 정확도와 성능에 미치는 영향을 보여 준다.
- 모델 handle을 정리한다.

### OpenVisionLab과 비교

- EdgeBasedMatching이 ROI/template registration, angle, result count, score, drawings을 지원한다.
- unique validation은 내부 Top-K를 유지하면서 외부에 정확히 Success/NoMatch/Ambiguous를 제공한다.
- result metrics와 report에 candidate diagnostics를 보존한다.

### 판단

- **잘된 부분:** beginner matching contract는 이미 구현되어 있으며 ambiguity 처리 면에서 더 엄격하다.
- **미비점:** model contour와 search candidate가 parameter 변경에 따라 어떻게 달라지는지 한눈에 설명하는 진단 뷰.
- **우선순위:** 별도 matcher 재작성보다 공통 signal/model diagnostic이 먼저다.

## 4.13 13 HALCON Shape Matching Advanced Parameters

### 영상에서 확인한 흐름

- image pyramid `NumLevels`가 속도와 검출 가능 feature 크기를 조절한다.
- blur/shape variation에 대한 허용 범위를 조정한다.
- threshold와 connected region으로 search domain을 줄여 오검출과 비용을 낮춘다.
- polarity를 use, ignore global, ignore local 등으로 설정한다.
- greediness와 scaled shape matching을 설명한다.
- 검색 ROI에서는 model center가 들어오는 조건을 강조한다.

### OpenVisionLab과 비교

- coarse-to-fine/pyramid proposal, angle/scale search, greediness, subpixel/position refine, hybrid verify, search ROI가 이미 있다.
- 현재 source에는 여러 scale/pyramid 실험의 통과·실패 evidence와 “자동 기본값으로 승격하지 말 것” 계약이 있다.
- explicit polarity/deformation modes와 pyramid/model-level visualization은 상대적으로 약하다.

### 판단

- **잘된 부분:** parameter를 무작정 늘리지 않고 opt-in과 evidence gate를 유지한다.
- **유력한 개선:** 기존 내부 model/candidate diagnostics를 operator가 이해할 수 있는 review surface로 노출.
- **조건부 알고리즘 확장:** 동일 source/ROI에서 polarity 또는 deformation 때문에 실패한다는 N-sample 증거가 있을 때만 작은 계약으로 추가한다.

## 4.14 14 HALCON 2D Metrology Measurement

### 영상에서 확인한 흐름

- circle 같은 geometric primitive의 정밀 측정을 metrology model handle에 추가한다.
- image size를 설정하고 rough center/radius를 가르친다.
- threshold와 area center로 object의 대략 위치를 찾은 뒤 metrology model을 align한다.
- apply 후 center와 radius를 받고 result contour를 표시한다.
- `Measure Length 1/2`로 sampling rectangle의 크기를 조정한다.
- 각 rectangle에서 찾은 edge point를 표시하고 이 점으로 final circle을 fit한다.
- camera parameters와 plane pose를 주면 pixel 결과를 world meter/mm로 변환한다.

### OpenVisionLab과 비교

- CircleGauge는 annular ROI, radius range, scan count, polarity, contrast, support ratio, fit residual을 사용한다.
- drawing은 ROI, sample/edge support, fitted circle을 보존한다.
- GeometryMeasure가 typed geometry를 조합하며 scale이 있으면 mm metrics를 제공한다.
- full camera model과 calibrated measuring plane은 없다.

### 판단

- **잘된 부분:** pixel-only circle/geometry 계약과 fail-closed source validation은 현재 방향에 적합하다.
- **미비점:** radial sampling별 intensity/edge response, inlier/outlier, residual distribution을 대화형으로 보는 진단.
- **범위 외:** camera/plane calibrated world metrology.

## 4.15 15 HALCON Working with Regions

### 영상에서 확인한 흐름

- gray histogram assistant에서 lower/upper threshold bar를 움직이며 segmentation 결과를 즉시 본다.
- touching bean/pea 때문에 connected components가 실패하는 장면을 확인한다.
- dilation, erosion, closing, opening의 geometry 효과를 시각화한다.
- erosion으로 붙은 object를 분리하고 connection 후 같은 radius dilation으로 shape를 복원한다.
- region feature reference에서 circularity를 선택한다.
- feature histogram에서 두 object group의 분포를 보고 threshold를 정한다.
- `Select Shape`으로 pea를 고르고 region difference로 bean을 만든다.

### OpenVisionLab과 비교

- Threshold, Morphology, Blob/Contour, area 및 width/height filter, Object Results가 core workflow를 제공한다.
- current source의 per-object filter는 area와 axis-aligned width/height까지이며 circularity/aspect/holes/gray descriptors는 제공하지 않는다.
- rule-based UI gap audit는 이 feature들을 이름만 보고 추가하지 말고 실제 operator task가 있을 때만 선택하도록 결정했다.
- region set algebra도 현재 선택된 범위가 아니다.

### 판단

- **가장 중요한 교훈:** histogram은 단순 장식이 아니라 threshold와 feature gate를 선택하는 증거 도구다.
- **미비점:** distribution chart가 없어 object population을 보고 gate를 정하기 어렵다.
- **보류:** circularity/aspect/holes/region algebra. 먼저 signal/distribution inspector가 기존 area/width/height에서도 operator 문제를 해결하는지 확인한다.

## 4.16 16 HALCON Shape Matching: Align ROI and Images

### 영상에서 확인한 흐름

- score tuple 길이로 match count를 계산하고 match마다 score를 image 좌표에 표시한다.
- original model ROI center/angle과 match row/column/angle 사이의 rigid transform을 만든다.
- affine transform으로 ROI를 각 match 위치로 이동한다.
- multi-match에서는 tuple 전체를 한 번에 전달할 수 없어 match별 loop를 사용한다.
- 반대 transform을 image에 적용해 모든 object가 reference orientation으로 나타나게 한다.

### OpenVisionLab과 비교

- result count, score, drawings, fixture pose, NormalizeImage가 같은 기본 목적을 담당한다.
- translation-only fixture, similarity normalization, three-point affine, typed Point source가 단계별로 분리되어 있다.
- current contract는 unsupported multi-ROI/multi-result/cross-frame cases를 fail closed한다.

### 판단

- **잘된 부분:** transform 방향과 reference/current pose, unchanged saved ROI를 명시적으로 검증한다.
- **조건부 gap:** 한 match가 여러 downstream ROI를 움직이거나 multi-instance 각각에 동일 recipe를 적용하는 범용 consumer flow는 아직 제한적이다.
- **보류:** multi-instance loop/automatic fan-out. 결과 identity, output naming, evidence explosion, acceptance 의미가 먼저 정의되어야 한다.

## 5. 상용 제품에서 배울 공통 설계 패턴

| 공통 패턴 | 영상 | OpenVisionLab 상태 | 결정 |
| --- | --- | --- | --- |
| 이미지가 중심이고 선택한 도구/단계/결과가 동기화 | 01, 03, 05, 09 | 강함 | 유지 |
| locator pose가 downstream ROI를 지배 | 01, 07, 14, 16 | bounded support 있음 | 실제 다중 소비자 blocker 때 확장 |
| Good/Bad와 image sequence로 즉시 확인 | 01, 07, 10, 11 | 강함 | 유지, novice evidence 추가 |
| histogram/edge response로 threshold 근거 제공 | 09, 14, 15 | 약함 | 가장 유력한 조건부 개발 후보 |
| model edge/pyramid/candidate 진단 | 02, 12, 13 | metric은 있으나 operator surface가 약함 | signal inspector와 통합 검토 |
| task-specific suggestion, explicit accept | 06, 11 | Auto MPoint에서 bounded support | generic easyTouch는 보류 |
| object population feature distribution | 15 | object rows는 있음, chart 없음 | distribution inspector부터 |
| calibration data로 world measurement | 08, 14 | uniform scale만 있음 | 현 제품 범위 외 |
| account/audit/regulatory system validation | 10 | 없음 | 현 제품 범위 외 |
| scripting/debugger/general tuple language | 03, 04, 16 | 없음 | product identity와 불일치 |

## 6. OpenVisionLab이 잘하는 부분

### 6.1 실행과 편집의 분리

상용 데모는 parameter를 만지는 즉시 결과가 바뀌는 경우가 많다. OpenVisionLab은 다음 행위를 실행과 분리한다.

- boolean visibility toggle
- layer create/delete/load/rename
- output layer creation
- Tool rail search
- Pipeline Review tab/step selection
- Fixture/Geometry/Object review selection
- recipe parameter edit
- Qualified Snapshot review

이 계약은 “화면을 봤다”와 “새 결과를 실행했다”를 혼동하지 않게 한다.

### 6.2 결과 의미와 실행 성공의 분리

- tool execution success
- metric acceptance OK/NG
- expected OK/NG
- judgment correct/incorrect
- execution error

를 분리한다. 이는 검증 결과를 과장하지 않게 하는 핵심 강점이다.

### 6.3 evidence provenance

- exact Pipeline/Step XML
- source/result SHA-256
- dependency hash
- current-run drawing
- per-Step report
- deterministic review queue
- runtime fingerprint
- immutable qualified archive

가 연결되어 있어, 결과 이미지만 남기는 데모보다 재현성과 감사 가능성이 높다.

### 6.4 알고리즘보다 workflow contract를 우선

matching, line, circle, affine, object filter를 각각 구현한 데 그치지 않고, PropertyGrid round-trip, recipe mapping, explicit Run, drawing, metrics, report, validation, history까지 연결한다. 이 방향은 유지해야 한다.

## 7. 현재 미비한 부분

### 7.1 파라미터 튜닝 근거의 분포/신호 시각화

현재 overlay는 “어디서 무엇을 찾았는가”에는 강하지만, 다음 질문에는 상대적으로 약하다.

- threshold 128이 왜 110이나 145보다 적절한가?
- polarity와 contrast가 어느 edge peak를 선택했는가?
- circle의 어느 radial sample이 inlier/outlier인가?
- object area/width/height population이 gate 양쪽에서 어떻게 나뉘는가?
- matcher pyramid/model/candidate가 어떤 feature를 사용하고 어떤 alternative와 경쟁했는가?

이는 09, 13, 14, 15 영상이 반복해서 보여 준 공통 gap이다.

### 7.2 독립 초보 사용성 증거

현재 source audit는 Sample → Learn/teach → Recipe → Run Review → Validation Set → Qualified Snapshot 흐름이 연결되었음을 확인했다. 그러나 실제 초보 참여자가 도움 없이 완료하는지는 아직 외부 증거가 없다.

### 7.3 generic fixture consumer와 multi-instance

현재 bounded fixture/NormalizeImage/Affine workflow는 강하다. 하지만 한 pose를 여러 downstream ROI가 직접 소비하거나 여러 match instance에 동일 sub-recipe를 적용하는 일반 모델은 없다. 이는 기능 목록의 빈칸이 아니라 identity/evidence/acceptance가 복잡한 새 실행 모델이다. 실제 blocker 없이는 시작하지 않는다.

### 7.4 field and calibration qualification

현재 증거는 synthetic/public/local selected corpus와 UI/runtime smoke가 중심이다. acquisition drift, lens distortion, certified reference, long-term repeatability, production environment, installer/deployment를 증명하지 않는다.

## 8. 개발 우선순위

아래에서 0번만 현재 활성 우선순위다. 1번 이후는 명시한 trigger가 충족될 때만 활성화한다.

이 절은 최초 비교 보고서의 요약이다. 모든 영상 파생 후보를 보존한
`CVR-00`~`CVR-20` canonical queue, 상세 activation gate, acceptance
boundary, model/reasoning recommendation, 제외 목록은
`docs\OPENVISIONLAB_COMMERCIAL_VIDEO_DEVELOPMENT_BACKLOG_20260727.md`를
따른다. 새 대화는 이 canonical backlog를 읽고, 이 보고서의 짧은 목록을
전체 backlog로 오해하지 않아야 한다.

### 0. 독립 초보 사용자 여정 평가

- 목표: 최소 3명의 OpenVisionLab 비경험자가 기존 protocol로 sample 선택부터 Qualified Snapshot까지 수행한다.
- acceptance:
  - task completion과 도움 요청 지점을 기록한다.
  - 잘못된 자동 실행, route 변경, evidence 오해가 없는지 확인한다.
  - 첫 3명 중 2명 이상이 같은 전환/mental model에서 실패하면 하나의 bounded UX correction 후보로 승격한다.
- prerequisite: 독립 참여자와 관찰 기록.
- Recommended model: 없음. 참여자 evidence가 생기기 전에는 model token을 쓰지 않는다.
- Reasoning effort: 없음.

### 1. Tool Signal Inspector

- 상태: **가장 유력한 조건부 구현 후보**, 아직 active feature 아님.
- trigger:
  - 실제 Threshold/Line/Circle/Object 작업에서 현재 overlay/metrics만으로 gate를 설명하지 못하는 current-source 재현, 또는
  - 초보 3명 중 2명 이상이 같은 signal-to-parameter 연결을 이해하지 못함.
- 최소 범위:
  - retained source와 selected ROI/scan line에서 gray histogram 또는 1D intensity/edge response를 표시한다.
  - selected Step/PropertyGrid와 양방향 context를 유지한다.
  - threshold/contrast/polarity marker를 chart에 표시한다.
  - chart open/select/pan/zoom은 Preview/Run을 실행하지 않는다.
  - parameter edit은 기존 manual/auto-preview 계약을 그대로 따른다.
  - export 시 source hash, ROI/line coordinates, Step hash를 기록한다.
- 1차 적용 순서: Threshold histogram → Line edge response → Circle radial response → Blob/Contour object metric distribution.
- 제외: 자동 threshold 추천, 자동 gate 변경, arbitrary plotting framework.
- Recommended model: `gpt-5.6-sol`
- Reasoning effort: `high`

### 2. Generic typed fixture consumer

- 상태: 조건부.
- trigger: 하나의 qualified locator pose를 두 개 이상의 downstream ROI/measurement Step이 소비해야 하는 named operator task가 현재 P212 NormalizeImage 또는 P219 Affine로 안전하게 표현되지 않음.
- 최소 범위:
  - one accepted fixture frame identity
  - transform provenance와 same-frame validation
  - saved reference ROI는 불변
  - source polygon과 reference rectangle drawing
  - multi-consumer는 지원하되 multi-instance fan-out은 제외
  - explicit Run만 transform을 적용
- 제외: auto locator selection, homography, arbitrary graph engine, multi-instance sub-recipe.
- Recommended model: `gpt-5.6-sol`
- Reasoning effort: `high`

### 3. Matcher diagnostic surface

- 상태: Tool Signal Inspector와 겹치므로 별도 구현보다 1번에 흡수하는 것이 우선.
- trigger: matcher의 existing metrics/drawings로 operator가 wrong feature, pyramid loss, ambiguity를 판별하지 못하는 current-source N-sample 사례.
- 최소 범위:
  - trained edge model/coarse-fine level
  - selected candidate와 strongest alternative
  - score/margin/search ROI/scale-angle state
  - existing diagnostic metrics의 설명
  - read-only retained-run evidence
- 제외: score 자동 조정, default 변경, per-image tuning.
- Recommended model: `gpt-5.6-sol`
- Reasoning effort: `high`

### 4. Matcher polarity 또는 bounded deformation

- 상태: 낮은 조건부 우선순위.
- trigger: 동일 physical feature와 frozen ROI에서 현재 matcher가 polarity reversal 또는 bounded shape deformation 때문에 실패하고, 기존 threshold/Canny/angle/scale/hybrid/search ROI로 해결되지 않는 labelled N-sample 증거.
- acceptance:
  - missing XML key는 legacy behavior 유지
  - opt-in
  - explicit PropertyGrid mapping
  - current-run model/candidate drawing
  - Good/NG/ambiguity matrix와 held-out replay
- 제외: Cognex parameter parity 자체, PatFlex 수준의 일반 elastic matching 주장.
- Recommended model: `gpt-5.6-sol`
- Reasoning effort: `high`

## 9. 개발하지 않을 항목

| 항목 | 결정 | 이유 |
| --- | --- | --- |
| 일반 ML classifier/training UI | 제외 | 룰베이스 정체성과 다르고 현재 operator task 없음 |
| HDevelop script IDE/debugger | 제외 | recipe workbench가 범용 vision language IDE로 변질 |
| general tuple/expression language | 보류 | named metric acceptance로 현재 작업을 더 안전하게 표현 |
| generic easyTouch | 보류 | Auto MPoint의 bounded explicit-accept 방식이 현재 방향에 적합 |
| circularity/aspect/holes/gray feature 전체 | 보류 | 실제 object task/blocker 없이 descriptor 폭만 넓히지 않음 |
| region set algebra 전체 | 보류 | current blocker 없음 |
| OCR/barcode | 보류 | named task 없음 |
| camera/lens calibration | 범위 외 | 현재는 uniform image-plane scale만 소유 |
| PLC/I/O/camera/lighting | 범위 외 | product identity 위반 |
| user/role/SSO/regulatory audit | 범위 외 | 로컬 recipe workbench 범위를 넘음 |
| homography/multi-instance graph | 보류 | identity, evidence, acceptance contract가 먼저 필요 |
| LLM provider/skill expansion | 동결 | P196 maintenance-mode 계약 |

## 10. 권장 제품 방향

### 10.1 유지해야 할 정체성

“이미지와 프롬프트를 주면 자동으로 검사 프로그램을 완성하는 도구”가 아니다.

“작업자가 검사 의도와 물리적 ROI/공차/표본을 가르치면, 룰베이스 도구가 결정론적으로 실행되고, 결과 도형·수치·표본 이력을 근거로 레시피를 검증하는 워크벤치”다.

### 10.2 상용 제품에서 선택적으로 모방할 것

- image-first workspace
- selected tool/step/result context coherence
- task-oriented entry and progressive disclosure
- fixture relationship visibility
- histogram/edge-response/metrology sampling diagnostics
- Good/Bad image sequence review
- “왜 실패했고 무엇을 확인할지”에 대한 직접 안내

### 10.3 모방하지 않을 플랫폼 범위

- camera acquisition and lighting controller
- PLC/I/O/MES
- user/account/SSO/regulatory platform
- installer/fleet/deployment system
- general-purpose scripting IDE
- arbitrary AI classifier/LLM automation platform

## 11. 결론

OpenVisionLab은 상용 제품보다 도구 family와 field qualification 폭은 좁지만, 현재 선택한 룰베이스 workflow의 **명시적 실행, 결과 의미 분리, evidence provenance, recipe qualification**은 이미 강하다.

다음 개발 방향은 알고리즘을 더 모으는 것이 아니다.

1. 독립 초보 사용자 evidence로 현재 workflow의 실제 막힘을 찾는다.
2. 막힘이 signal-to-parameter 이해에 있다면 Tool Signal Inspector를 가장 먼저 구현한다.
3. fixture나 matcher 확장은 실제 named task가 현재 bounded workflow를 깨뜨릴 때만 수행한다.
4. calibration, hardware, compliance, general scripting, generic AI 기능은 제품 범위와 분리한다.

이 순서를 지키면 OpenVisionLab은 상용 제품의 표면을 흉내 내는 프로그램이 아니라, “가르친 룰과 증거가 재현되는 검사 recipe workbench”라는 고유 정체성을 강화할 수 있다.

## 12. Completion record

```text
Status: Complete
Scope: 프로젝트 문서/계약 기준선 재구성, 16개 제공 영상의 화면·자막/로컬 전사 분석, 현재 소스와 기능 비교, 제품 평가, 조건부 개발 우선순위 도출
```

Acceptance criteria:

- `AGENTS.md`와 현재 canonical product/contract/status 문서를 기준으로 제품 정체성을 재구성함: Pass
- `docs` Markdown 303개를 인덱싱하고 current authority 문서를 상세 검토함: Pass
- MP4 16개를 모두 검토하고 영상별 관찰/비교/결정을 기록함: Pass
- 제공 자막 8개와 자막 없는 영상 8개의 로컬 전사 및 256개 대표 프레임을 근거로 사용함: Pass
- 현재 기능과 미비점, 범위 외 항목을 분리함: Pass
- 각 개발 우선순위에 trigger와 Recommended model/Reasoning effort를 기록함: Pass

Verification:

- media count/duration/resolution 확인
- MP4 16개 SHA-256 기록
- storyboard 16개와 local transcript 8개 존재 확인
- current-source targeted search로 Tool/Validation/Qualified Snapshot 존재와 gray histogram/edge-response chart 부재 확인
- repository baseline `9d7fa796ed94d90e50d840607b441a2954278947` 기록
- Markdown 구조와 local evidence path 점검

Evidence:

- `docs\reports\OPENVISIONLAB_COMMERCIAL_RULEBASE_VIDEO_REVIEW_20260727.md`
- `artifacts\commercial_rulebase_video_review_20260727\README.md`
- `artifacts\commercial_rulebase_video_review_20260727\01_storyboard.jpg` through `16_storyboard.jpg`
- `artifacts\commercial_rulebase_video_review_20260727\04_whisper_base_en.txt`
- `artifacts\commercial_rulebase_video_review_20260727\06_whisper_base_en.txt`
- `artifacts\commercial_rulebase_video_review_20260727\11_whisper_base_en.txt` through `16_whisper_base_en.txt`

Boundary / next dependency: 이 보고서는 제공 영상과 현재 Dev repository의 비교 분석이다. 상용 제품의 최신 전체 사양, OpenVisionLab의 독립 초보 자립성, certified metrology, unseen/field robustness, production deployment를 증명하지 않는다. 다음 활성 prerequisite는 독립 초보 참여자 최소 3명의 관찰 기록이다.
