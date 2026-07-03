# OpenVisionLab Self Evaluation - 2026-07-03

## Final update: 2026-07-03 17:00 KST

The Product sample catalog/native runner conflict recorded later in this document is resolved.

- Root cause: the original repo source DLL was updated to `dll\OpenCVSharp\OpenCvSharpExtern.dll` hash `C9E02A255DD83C9B06CA56EC6F435F15B53A863435238FCC5D8B9082B035F249`, but `VisionRecipeRunnerSmoke` output still held the older `E5EC2B92397D18AEFDBE555502D9AB622B6BC2CD3ED136C2155E7B95CAB57295` native DLL because `CopyToOutputDirectory=PreserveNewest` skipped the older-timestamp source file.
- Fix: native `OpenCvSharpExtern.dll` is copied from `dll\OpenCVSharp\`, and runtime output copies use `CopyToOutputDirectory=Always`.
- Dev evidence: `artifacts\product_catalog_after_shared_native_runtime_20260703_1640\sample_catalog_summary.json` reported `GateStatus=OK`, `RunnableRows=168`, `OKRows=168`, `NGRows=0`.
- Original repo evidence: `artifacts\original_product_catalog_after_always_native_copy_20260703_1658\sample_catalog_summary.json` reported `GateStatus=OK`, `RunnableRows=168`, `OKRows=168`, `NGRows=0`.
- Quality audit evidence: Dev and original both reported `ProductSampleQualityAudit=PASS | PairRecords=84 OK=84 Review=0 Critical=0`.

Current Product sample benchmark assessment is restored to `4.0 / 5`: the catalog is repeatable, public-safe, and guarded against stale native runner output. The next priority moves back to Pipeline/Recipe operator review UX and remaining Tool View code-behind reduction.

이 문서는 OpenVisionLab의 현재 제품 상태를 공식 경쟁 제품 자료와 비교해 자체 평가한 결과다.
평가 기준은 절대적인 산업용 장비 플랫폼 경쟁력이 아니라, OpenVisionLab의 목표인 OpenCvSharp4 기반 rule-based vision workbench 적합도다.

## 1. 평가 범위

OpenVisionLab은 이미지 기반 룰베이스 알고리즘 검증, 학습, 레시피 구성 도구다.

따라서 아래 항목은 목표 범위에 포함한다.

- 이미지 레이어 기반 입력/출력 흐름
- Threshold, Blob, Contour, Line/Length, Matching, EdgeBasedMatching, FeatureMatching 계열 rule-based tool
- PropertyGrid 기반 파라미터 teaching
- Preview와 Pipeline/Recipe 확정 분리
- OK/NG 결과, metric, overlay, 실패 이유 설명
- Good/Bad sample pair 기반 검증 루프

아래 항목은 현재 제품 목표가 아니다.

- 카메라, 조명, PLC, I/O, 산업용 통신 통합 플랫폼
- 현장 설비 runtime/HMI 전체 패키지
- HALCON/VisionPro급 상용 알고리즘 라이브러리 전체 대체
- 딥러닝 학습/배포 제품군

## 2. 확인한 내부 근거

이번 평가에서 직접 확인한 내부 근거는 다음과 같다.

- 제품 정체성: `docs/OPENVISIONLAB_PRODUCT_IDENTITY_AND_ROADMAP.md`
- 안정 계약: `docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`
- 기존 경쟁 UX 검토: `docs/OPENVISIONLAB_UX_COMPETITOR_REVIEW.md`
- 기존 경쟁 우선순위 검토: `docs/OPENVISIONLAB_COMPETITOR_PRIORITY_REVIEW_20260701.md`
- public sample 정책: `docs/OPENVISIONLAB_PUBLIC_SAMPLE_ASSET_POLICY.md`
- Product catalog 실행:
  - Command: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\self_evaluation_product_catalog_20260703`
  - Result: `GateStatus=OK`, `RunnableRows=168`, `RequiredRows=84`, `ExpectedFailureRows=84`, `OKRows=168`, `NGRows=0`
  - Report: `artifacts\self_evaluation_product_catalog_20260703\sample_catalog_report.md`
- Product sample quality audit:
  - Command: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1`
  - Result: `ProductSampleQualityAudit=PASS | PairRecords=84 OK=84 Review=0 Critical=0`
  - Report: `artifacts\product_sample_quality_audit\product_sample_quality_audit.md`
- 추가 확인 충돌:
  - 원본 repo `C:\Git\OpenVisionLab`에 선별 patch/import 반영 후 current runner로 `OpenVisionLab.ProductSampleCatalog.csv`를 다시 실행했을 때 `GateStatus=NG`가 재현됐다.
  - 실패는 주로 Contour/EdgeBasedMatching/FeatureMatching 계열에서 native `AccessViolationException` 또는 `Exit=-1073741819`로 나타났다.
  - 같은 summary를 대상으로 한 `AuditProductSampleQuality.ps1 -SummaryPath artifacts\original_product_catalog_20260703_1605\sample_catalog_summary.json`는 `PairRecords=84 OK=84 Review=0 Critical=0`로 통과했지만, catalog gate 자체가 NG이므로 release-ready PASS 근거로 보지 않는다.
  - 따라서 Product sample benchmark는 "Dev 검증 PASS / 원본 current runner NG" 충돌 상태이며, 원인 확인 전까지 publish-stable로 간주하지 않는다.

## 3. 확인한 외부 근거

공식 또는 공식 문서에 가까운 자료를 우선 확인했다.

- MVTec MERLIC: https://www.mvtec.com/products/merlic
- MVTec HALCON: https://www.mvtec.com/products/halcon
- MVTec HDevelop: https://www.mvtec.com/products/halcon/features-tools/development-tools-programming/hdevelop
- Cognex In-Sight EasyBuilder development environment: https://docs.cognex.com/is_621/web/EN/ise/Content/GettingStarted/DevEnvironment.htm
- Cognex EasyBuilder GUI: https://docs.cognex.com/is_580/web/EN/ise/Content/EasyBuilder/GraphicalUserInterface.htm
- Cognex In-Sight EasyBuilder I/O: https://docs.cognex.com/isvs_2610/web/EN/InSight_EZ/Content/Topics/EZB/ezb-ui-outputs-step.htm
- NI Vision Builder for Automated Inspection: https://www.ni.com/en/shop/electronic-test-instrumentation/application-software-for-electronic-test-and-instrumentation-category/what-is-vision-builder-for-automated-inspection.html
- Zebra Aurora Vision Studio: https://www.zebra.com/us/en/products/oem/software/aurora-vision-studio.html
- Aurora Vision Studio documentation: https://docs.adaptive-vision.com/5.6/studio/introduction/ProductOverview.html
- KEYENCE XG-X: https://www.keyence.com/products/vision/vision-sys/xg-x/
- OpenCV About: https://opencv.org/about/

## 4. 경쟁 제품에서 확인한 패턴

| 제품 | 공식 자료에서 확인한 방향 | OpenVisionLab에 주는 의미 |
| --- | --- | --- |
| MVTec HALCON / HDevelop | 폭넓은 2D/3D machine vision toolset, interactive 개발, code export, runtime 연계 | 알고리즘 범위와 산업 적용성은 비교 대상이 아니다. 대신 OpenVisionLab은 더 작은 범위에서 학습 가능한 recipe/metric/overlay 흐름을 명확히 해야 한다. |
| MVTec MERLIC | no-code, image-centered interface, 개발부터 runtime operation까지 all-in-one, deep learning과 rule-based 조합 | 우리도 image-centered 검증 흐름은 배워야 한다. 다만 camera/acquisition/industrial integration은 의도적으로 범위 밖에 둔다. |
| Cognex In-Sight EasyBuilder | image-centric GUI, application steps, interactive regions, result table, help와 I/O monitoring | 초보자에게는 tool list보다 `이미지 -> 영역/툴 -> 기준 -> 결과 -> 조치` 흐름이 먼저 보여야 한다. |
| NI Vision Builder AI | no-programming, camera configuration, hundreds of algorithms/inspection steps, benchmark, automation hardware interface | benchmark/pass-fail 설명은 배울 지점이다. 자동화 hardware 연계는 OpenVisionLab의 제품 정체성과 맞지 않는다. |
| Zebra Aurora Vision Studio | dataflow 기반 visual programming, ready-made filters, custom HMI | recipe/pipeline을 시각적으로 검토하는 UX는 강화할 가치가 있다. 하지만 PropertyGrid tool을 dataflow-only builder로 바꾸면 안 된다. |
| KEYENCE XG-X | flowchart programming, UI creation, debugging, simulation | operator review/debug loop는 참고 가치가 있다. 장비 controller 제품군과 같은 방향으로 확장하지 않는다. |
| OpenCV | open-source computer vision library, 2500개 이상 알고리즘 | OpenVisionLab의 강점은 OpenCV 계열 알고리즘을 WPF workbench, layer, recipe, sample review로 묶는 데 있다. |

## 5. 현재 OpenVisionLab 강점

### 5.1 제품 정체성이 선명하다

OpenVisionLab은 상용 설비 플랫폼을 따라가려는 제품이 아니라, 이미지와 rule-based recipe를 검증하는 workbench다.
이 정체성은 경쟁 제품 대비 약점이 아니라 범위 통제다.

유지해야 할 핵심은 다음과 같다.

- PropertyGrid 기반 algorithm tool
- 명시적 input/output layer 선택
- Preview와 Pipeline/Recipe 확정 분리
- output layer 생성이 input route를 자동 변경하지 않는 계약
- boolean visibility toggle만으로 Preview/Run이 실행되지 않는 계약
- viewer zoom/pan/drag, ROI overlay, template editor, layer compare/docking

### 5.2 학습 가능한 rule-based workflow가 있다

상용 제품은 빠른 production deployment를 강조한다.
OpenVisionLab은 반대로 사용자가 rule-based vision의 이유를 볼 수 있다는 점이 강점이다.

특히 아래 흐름은 계속 살려야 한다.

- input layer와 output layer를 직접 확인
- PropertyGrid에서 실제 알고리즘 파라미터를 조정
- Preview 결과를 overlay와 metric으로 확인
- Pipeline/Recipe에는 검증된 단계만 반영

### 5.3 sample-backed result explanation이 강해지고 있다

Product catalog는 현재 84개 Good/Bad pair, 168개 runnable row를 통과했다.
Bad row는 단순 실패가 아니라 expected-failure로 관리되고, NG 이유와 metric range를 확인할 수 있다.

이 부분은 OpenVisionLab의 핵심 차별점이 될 수 있다.
상용 장비형 제품이 operator deployment를 강조한다면, OpenVisionLab은 sample, metric, recipe, failure explanation을 묶어 학습과 검증을 설명하는 쪽에 강점이 있다.

### 5.4 공개 저장소 안전성이 고려되어 있다

제품 샘플은 project-authored synthetic asset 중심이며, vendor SDK sample을 public path에 다시 들여오지 않는 정책이 있다.
GitHub 공개를 전제로 한 workbench라면 이 점은 기술 기능만큼 중요하다.

## 6. 현재 약점과 위험

### 6.1 알고리즘 범위는 상용 suite와 비교할 수 없다

HALCON, VisionPro, Aurora Vision Studio 계열은 긴 기간 축적된 상용 toolset, 3D, calibration, OCR, barcode, deep learning, deployment 기능을 가진다.
OpenVisionLab은 이 영역에서 직접 경쟁하면 안 된다.

대신 제한된 toolset의 설명력과 sample-backed repeatability를 높이는 편이 맞다.

### 6.2 초보자 흐름은 아직 더 명확해야 한다

MainView와 Product sample review는 개선되고 있지만, 첫 사용자가 다음 질문에 즉시 답할 수 있어야 한다.

- 지금 무엇을 열었는가?
- 이 sample은 Good 기준인가, Bad 기준인가?
- 어떤 tool/pipeline이 어떤 metric으로 판단하는가?
- Preview 결과가 OK/NG인 이유는 무엇인가?
- 다음 조치는 sample review, parameter 조정, pipeline 추가 중 무엇인가?

### 6.3 Pipeline/Recipe operator review는 계속 강화할 축이다

Product sample은 준비되어 있고 catalog gate도 통과한다.
주요 약점은 operator가 recipe step을 훑어볼 때 input/output, branch reason, expected metric, actual metric을 빠르게 비교하는 UX다.
이번 Dev 루프에서 Review Habit 중복 문구, Good/Bad pair metric 판정, Recipe 저장 범위 표시를 일부 보강했지만,
step별 suggested fix와 expected/actual metric 비교 밀도는 계속 개선할 가치가 있다.

경쟁 제품들은 result table, flowchart, visual dataflow, application step 구조로 이 문제를 푼다.
OpenVisionLab도 flowchart를 무리하게 도입하기보다 현재 layer/pipeline 구조 위에 step review와 failure explanation을 더 분명하게 올리는 것이 적합하다.

### 6.4 Tool View code-behind와 공통 runtime 정리가 계속 필요하다

PropertyGrid 구조는 유지해야 하지만, tool별 View code-behind에 반복되는 presenter/controller/runtime wiring은 장기 유지보수 위험이다.
이번 Dev 루프에서 Line과 Arithmetic의 shell/runtime/event wiring은 공통 controller로 이동했고 readiness 계약으로 회귀를 막았다.
남은 과제는 기능별 interaction/presenter 로직을 과하게 추상화하지 않으면서, 새 tool 추가 시 반복되는 작은 패턴을 계속 줄이는 것이다.

## 7. 자체 점수

점수는 시장 전체 제품력 점수가 아니라 OpenVisionLab 목표 적합도 기준이다.

| 항목 | 점수 | 판단 |
| --- | ---: | --- |
| 제품 정체성 | 4.5 / 5 | rule-based vision workbench 방향이 명확하다. 장비 플랫폼으로 확장하지 않는 결정이 맞다. |
| PropertyGrid 기반 tool 구조 | 4.2 / 5 | 기존 tool 계약이 잘 정리되어 있고 Line/Arithmetic의 shell wiring도 controller로 이동했다. 반복 interaction 로직 축소는 남아 있다. |
| 이미지/layer 중심 검증 | 4.0 / 5 | viewer, layer compare, ROI/template, preview/output 분리가 강점이다. |
| 초보자 in-app guide | 3.5 / 5 | Product sample의 검증 순서와 Recipe 범위 표시가 보강됐다. 첫 사용 흐름은 계속 실제 캡처로 점검해야 한다. |
| 결과/실패 설명 | 4.2 / 5 | OK/NG, metric, suggested fix가 sample runner와 UI에 들어가고 있고 Pair Metric의 기준 안/밖 판정이 강화됐다. |
| Product sample benchmark | 3.5 / 5 | Dev에서는 168 row gate와 84 pair audit가 강점이었지만, 원본 current runner에서 native crash가 재현됐다. 원인 확인 전까지 release gate로 과장하면 안 된다. |
| 알고리즘 breadth | 2.5 / 5 | 상용 suite 대비 넓지 않다. 현재 목표상 치명적 약점은 아니지만 과장하면 안 된다. |
| 산업 runtime/integration | 1.5 / 5 | 낮지만 의도된 비목표다. camera/PLC/I/O 플랫폼으로 포지셔닝하면 안 된다. |
| 유지보수 구조 | 3.6 / 5 | 안정 계약, smoke, Tool View controller ownership readiness가 강점이다. 남은 리스크는 기능별 interaction 코드의 점진적 정리다. |

## 8. 유지할 장점

OpenVisionLab의 방향은 다음 장점을 계속 사용하는 쪽이어야 한다.

1. PropertyGrid를 버리지 않는다.
   - 초보자 guide는 PropertyGrid 위아래의 설명 레이어로 제공한다.
   - hand-coded parameter panel로 대체하지 않는다.

2. Preview와 Pipeline/Recipe 확정을 계속 분리한다.
   - sample open, layer create/delete/load, visibility toggle은 실행 행위가 아니다.
   - operator가 명시적으로 Preview/Run/Add Pipeline을 눌렀을 때만 결과가 바뀌어야 한다.

3. layer route를 명시적으로 유지한다.
   - output layer 생성이 input layer를 바꾸면 학습/검증 도구의 신뢰가 깨진다.

4. Good/Bad pair와 expected metric을 제품 핵심 기능으로 끌고 간다.
   - 단순 sample gallery가 아니라 benchmark entry point로 보여야 한다.

5. 공개 가능한 synthetic sample 자산을 강점으로 사용한다.
   - public repo에 안전하고 repeatable한 자료를 둔다는 점은 프로젝트 신뢰도를 높인다.

## 9. 가져오지 말아야 할 방향

경쟁 제품을 참고하더라도 아래 방향은 피해야 한다.

- camera acquisition, trigger, PLC, discrete I/O, industrial communication을 핵심 제품 방향으로 끌어들이기
- PropertyGrid tool을 wizard-only UI로 대체하기
- sample open 또는 layer 조작만으로 자동 Preview/Run을 실행하기
- beginner mode라는 이유로 metric, layer route, recipe step detail을 숨기기
- 상용 suite처럼 보이기 위해 검증되지 않은 tool category를 늘리기
- vendor SDK sample 이미지를 public sample로 재도입하기

## 10. 다음 우선순위

현재 상태 기준 우선순위는 다음 순서가 적합하다.

1. Product sample catalog/native runner gate 안정화
   - 원본 current runner에서 재현된 Contour/EdgeBasedMatching/FeatureMatching native crash를 먼저 분리한다.
   - stale build output, native DLL 선택, runner copy path, Contour input Mat lifetime을 확인한다.
   - catalog gate가 다시 `GateStatus=OK`가 되기 전에는 Product sample benchmark를 publish-ready로 홍보하지 않는다.

2. Pipeline/Recipe operator review UX 추가 보강
   - step별 input/output, expected/actual metric, failure reason, suggested fix를 더 빠르게 비교하게 만든다.
   - Product sample review와 연결해 Good/Bad pair를 여는 순간 operator가 무엇을 검증하는지 알게 한다.
   - 이번 루프에서 보강한 Pair Metric 판정을 기반으로, 실패 시 어떤 parameter를 조정할지 더 직접적으로 보여준다.

3. MainView/Product sample review 실제 사용자 흐름 재점검
   - 새 sample affordance가 sample picker, pipeline review, image workspace 사이에서 끊기지 않는지 실제 EXE 캡처로 다시 확인한다.

4. in-app guide와 result/failure explanation 고도화
   - 안내 문구를 늘리는 것이 아니라 현재 상태에서 다음 행동을 더 정확히 보여준다.

5. Product sample catalog 품질 감사와 샘플 보강
   - 84 pair 품질 audit는 PASS지만, catalog runner gate 충돌이 먼저 해결되어야 한다.
   - 추가 보강은 반복 가능한 metric margin이 있는 경우에만 한다.

6. Tool View code-behind 축소와 공통 runtime/controller/template 정리
   - PropertyGrid 구조를 유지하면서 반복 wiring을 줄인다.
   - Line/Arithmetic controller 분리 이후에는 직접 runtime wiring 재도입을 막고, 새 반복 패턴이 확인될 때만 작게 추출한다.

## 11. 결론

OpenVisionLab은 HALCON, MERLIC, Cognex, NI, Zebra, KEYENCE 계열 제품처럼 설비 구축과 runtime deployment를 포괄하는 상용 플랫폼으로 평가하면 약하다.
그러나 OpenCvSharp4 기반 rule-based vision workbench로 평가하면 제품 정체성, layer/preview separation, PropertyGrid teaching, sample-backed result explanation에서 분명한 장점이 있다.

따라서 다음 개선은 기능 범위를 넓히는 쪽보다, 현재 장점을 사용자가 더 빨리 이해하고 검증할 수 있게 만드는 방향이 맞다.
특히 Product sample benchmark와 Pipeline/Recipe review를 하나의 학습 루프로 연결하는 것이 현재 가장 큰 효율을 낼 작업이다.
