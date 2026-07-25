# OpenVisionLab 자체 평가 - 2026-07-03

## 결론

OpenVisionLab은 상용 장비 플랫폼과 정면 경쟁하는 제품이 아니라, OpenCvSharp4 기반 rule-based vision workbench로 보는 것이 맞다. 현재 강점은 카메라/PLC/I/O 통합이 아니라, 이미지 파일 기반으로 Threshold, Blob, Contour, Matching, EdgeBasedMatching, FeatureMatching, LineDistance 같은 룰 기반 도구를 학습하고, 레이어와 Pipeline/Recipe로 검증하며, Good/Bad 샘플 쌍으로 결과와 실패 이유를 반복 확인하는 데 있다.

상용 suite 전체와 비교하면 범위와 완성도는 아직 낮다. 하지만 "공개 가능한 이미지 기반 룰베이스 알고리즘 검증/학습/레시피 구성 도구"라는 목표 안에서는 이미 쓸 수 있는 수준까지 올라왔다. 현재 완성도는 제품 목표 기준 4.0/5, 상용 산업용 통합 플랫폼 기준 2.0/5로 평가한다.

## 이번 평가 근거

- Dev 작업 위치: `C:\Git\OpenVisionLab_Dev`
- 원본 반영 위치: `C:\Git\OpenVisionLab`
- 최신 원본 안정 커밋:
  - `811c2b2 Update handoff after final catalog check`
  - `4278e43 Trim unused filter morphology usings`
  - `da392e8 Update handoff after pipeline copy polish`
  - `5f76663 Shorten pipeline review next-action copy`
  - `853da22 Update handoff after review smoke restore`
  - `0a2e026 Restore filter morphology layout smoke`
  - `567fefc Share kernel preset click handling`
  - `6ca54d3 Add public sample review smoke runner`
  - `5f753d1 Stabilize product sample review and native runtime`
- Product catalog 실행:
  - Command: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_quality_followup_dev_20260703_01 -SkipRunnerBuild`
  - Result: `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`
  - Artifact issues: `0`, metadata issues: `0`
  - Report: `artifacts\product_catalog_quality_followup_dev_20260703_01\sample_catalog_report.md`
  - Summary: `artifacts\product_catalog_quality_followup_dev_20260703_01\sample_catalog_summary.json`
  - Current Dev public catalog confirmation: `PublicSampleAssetCheck=PASS | CatalogRows=184 ManifestAssets=214 Pipelines=87`
- Product sample quality audit:
  - Command: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -SummaryPath artifacts\product_catalog_quality_followup_dev_20260703_01\sample_catalog_summary.json`
  - Result: `ProductSampleQualityAudit=PASS | PairRecords=84 OK=84 Review=0 Critical=0`
  - Report: `artifacts\product_sample_quality_audit\product_sample_quality_audit.md`
- UI evidence:
  - Product focus before: `artifacts\mainview_product_flow_before_20260703_1735\wpf_shell_host_workspace_sample_product_focus_open.png`
  - Product focus after: `artifacts\mainview_product_flow_after_20260703_1745\wpf_shell_host_workspace_sample_product_focus_open.png`
  - Pipeline Review before: `artifacts\mainview_product_review_before_20260703_1739\wpf_shell_host_workspace_product_sample_review.png`
  - Pipeline Review after: `artifacts\pipeline_review_final_next_after_20260703_1746\wpf_shell_host_workspace_product_sample_review.png`
  - Original after: `C:\Git\OpenVisionLab\artifacts\original_pipeline_review_final_next_after_20260703_1748\wpf_shell_host_workspace_product_sample_review.png`
- Follow-up stabilization after the initial self-evaluation:
  - Sample review smoke runner passed after later Pipeline Review copy and Filter/Morphology smoke fixes.
  - Pipeline Review top-card action text was shortened so the summary card no longer carries the long detailed guide string.
  - Filter/Morphology kernel preset clicks now use the shared `VisionToolKernelSizeController` path and the restored guard clicks both preset paths.
  - The self-evaluation conclusion is unchanged: keep the rule-based, OpenCvSharp4, PropertyGrid-centered workbench identity instead of expanding toward a hardware integration platform.
- 2026-07-04 follow-up:
  - Pipeline Review now has a manual `NG Step` button that selects the first failed review step after explicit Run Review.
  - The first-issue navigation smoke clicks the real button, verifies the first NG Threshold step is selected, and confirms Native Preview/Run count does not increase.
  - Line and Arithmetic preview scheduling moved into dedicated preview controllers; Threshold removed a duplicate schedule wrapper; SimplePreprocess setting restore now belongs to the parameter controller.
  - Product field-style samples are represented as 16 renamed Explore rows, with current full catalog gate `RunnableRows=184`, `OKRows=184`, `NGRows=0`.

## 외부 비교 근거

이번 비교는 공식 또는 공식에 가까운 자료만 사용했다.

- KEYENCE XG-X 공식 페이지: XG-X는 카메라 라인업과 컨트롤러를 포함하는 customizable vision system이고, XG-X VisionEditor는 flowchart programming, UI creation, debugging, simulation을 제공한다고 설명한다.
  - https://www.keyence.com/products/vision/vision-sys/xg-x/
  - https://www.keyence.com/support/user/xg/video/vision-editor.jsp
- MVTec HALCON 공식 페이지: HALCON은 2D/3D machine vision toolbox이며, image acquisition부터 deep learning까지 넓은 toolset, GPU/parallelization, calibration 등을 강조한다.
  - https://www.mvtec.com/products/halcon
- MVTec HALCON/MERLIC 공식 소개: HALCON은 2,000개 이상 operator와 HDevelop IDE를, MERLIC은 no-code image-centered UI와 integrated tool library를 강조한다.
  - https://www.mvtec.com/imageprocessingsoftware
- NI Vision Builder AI 공식 페이지: Vision Builder AI는 no-programming 환경에서 camera configuration, image analysis, inspection result, automation hardware interface, benchmark를 제공한다고 설명한다.
  - https://www.ni.com/en/shop/electronic-test-instrumentation/application-software-for-electronic-test-and-instrumentation-category/what-is-vision-builder-for-automated-inspection.html
- Cognex VisionPro 공식 URL은 접근 시 403으로 본문 확인이 제한되었다. 검색 결과 수준에서는 rule-based programming, hybrid smart tools, AI capability를 강조한다는 정도만 참고한다.
  - https://www.cognex.com/en/products/machine-vision-software/visionpro-software

## 타사 대비 평가

| 비교 대상 | 타사 강점 | OpenVisionLab 판단 |
| --- | --- | --- |
| KEYENCE XG-X | 컨트롤러, 카메라, flowchart programming, simulation, 현장 지원까지 포함 | 이 방향으로 확장하면 제품 정체성이 흐려진다. 대신 flowchart/debug 관점은 Pipeline Review UX에만 참고한다. |
| MVTec HALCON | 방대한 operator, 2D/3D, calibration, deep learning, 배포/운영 지향 | 알고리즘 breadth로 경쟁하면 불리하다. OpenVisionLab은 작은 범위에서 레이어/metric/overlay를 설명 가능한 학습 워크벤치로 유지해야 한다. |
| MVTec MERLIC | no-code, image-centered UI, runtime operation까지 한 패키지 | image-centered guide는 참고 가치가 크다. 그러나 PropertyGrid 기반 tool 구조를 wizard-only UI로 바꾸면 안 된다. |
| NI Vision Builder AI | 메뉴 기반 inspection authoring, camera/hardware integration, benchmark | result/failure explanation과 benchmark 관점은 배울 부분이다. hardware integration은 현재 제품 범위 밖이다. |
| Cognex VisionPro | rule-based, smart tools, AI, 제조 task coverage | 상용 tool breadth와 제조 적용 범위는 따라가기 어렵다. OpenVisionLab은 OpenCvSharp 기반 투명한 recipe 학습/검증으로 차별화해야 한다. |

## 현재 강점

1. 제품 정체성이 분명하다.
   - 이미지 기반 rule-based vision workbench이다.
   - camera/lighting/PLC/I/O 통합 플랫폼이 아니라는 선이 명확하다.

2. 학습 가능한 Tool 구조를 유지한다.
   - PropertyGrid 기반 파라미터 UI가 유지된다.
   - 사용자는 tool별 파라미터가 결과에 미치는 영향을 직접 볼 수 있다.

3. Preview와 Run/Pipeline 검증의 의미가 분리되어 있다.
   - output layer 생성이 input layer를 자동 변경하지 않는 계약을 지킨다.
   - visibility toggle, layer create/delete/load-image가 자동 Run을 유발하지 않는 방향이 맞다.

4. 결과 설명이 실제 runner와 연결되어 있다.
   - Product sample 184 row가 전부 실행 검증된다.
   - Bad 샘플 84 row는 expected-failure로 관리되어 NG 이유와 metric 범위를 확인할 수 있다.
   - 예: Matching 실패는 template/input/ROI/score threshold 튜닝 안내를 낸다. Blob/Contour/Line 실패는 ResultCount, DistanceMmAvg 같은 측정값과 target 범위를 같이 낸다.

5. 공개 안전성이 개선되었다.
   - public sample과 product sample은 synthetic asset 중심이다.
   - root `Sample` 의존 smoke를 public-safe sample로 교체했다.
   - public asset 정책과 readiness check가 샘플 역류를 막는다.

## 현재 미비사항

1. 첫 사용자 흐름은 아직 밀도가 높다.
   - Product sample picker와 MainView sample strip은 개선되었지만, 처음 쓰는 사용자가 "지금 무엇을 검증 중인지"를 즉시 이해하려면 더 많은 in-app affordance가 필요하다.
   - 특히 Sample Picker -> Open Sample -> Pipeline Review -> Bad counterpart 비교 흐름이 하나의 guided review처럼 이어져야 한다.

2. Pipeline/Recipe operator review는 아직 상용 flowchart/debug 도구만큼 즉시 읽히지 않는다.
   - step별 input/output, branch reason, expected metric, actual metric, failed step fix가 이미 존재하지만 정보 위치가 분산되어 있다.
   - 최종 OK 안내는 개선했지만, NG 단계에서는 "어느 파라미터부터 볼지"를 더 직접적으로 보여줄 여지가 있다.

3. Tool View code-behind 축소는 계속 필요하다.
   - Line/Arithmetic 계열에서 공통 controller 추출이 시작되었지만 반복 wiring이 아직 남아 있다.
   - 단, 과한 추상화로 PropertyGrid 학습 구조를 흐리면 안 된다. 반복 이벤트 wiring과 runtime 생성만 줄이는 방향이 맞다.

4. 알고리즘 breadth는 상용 suite 대비 낮다.
   - OCR, barcode, calibration, 3D, deep learning, deployment runtime은 아직 핵심 범위가 아니다.
   - 현재 목표 안에서는 약점이지만, 상용 플랫폼처럼 보이려고 무리하게 넓히면 제품의 강점이 흐려진다.

5. Product sample은 양보다 "리뷰 가능한 현장감"이 더 중요해졌다.
   - 현재 84 Good/Bad pair는 PASS이고 추가 샘플을 무조건 늘릴 필요는 없다.
   - 다음 보강은 새 row 추가보다, 대표 샘플의 설명/비교/실패 원인 affordance를 높이는 쪽이 우선이다.

## 점수

| 항목 | 점수 | 판단 |
| --- | ---: | --- |
| 제품 방향성 | 4.5 / 5 | rule-based workbench 방향은 명확하다. 장비 플랫폼으로 확장하지 않는 결정이 맞다. |
| 현재 사용 가능성 | 4.0 / 5 | 샘플 실행, Pipeline Review, result/failure explanation이 실제로 동작한다. |
| Product sample catalog | 4.3 / 5 | 184 row gate, 84 pair audit가 통과한다. 추가 샘플보다 UX 연결이 더 중요하다. |
| in-app guide | 3.8 / 5 | MainView와 Pipeline Review가 개선되었고 NG Step affordance가 추가되었다. 첫 사용자 guided flow는 더 다듬어야 한다. |
| 결과/실패 설명 | 4.3 / 5 | metric, target, suggested fix가 runner와 UI에 연결되어 있고 첫 NG 단계 이동이 가능하다. |
| Tool View 유지보수성 | 3.7 / 5 | preview/review/parameter 책임이 controller로 더 이동했지만 일부 facade와 반복 code-behind가 아직 남아 있다. |
| 상용 알고리즘 범위 | 2.3 / 5 | HALCON/VisionPro류 breadth와 비교 대상이 아니다. 현재 목표에서는 치명적 결함으로 보지 않는다. |
| 산업 장비 통합성 | 1.5 / 5 | 일부러 하지 않는 영역이다. 이 점수를 올리려 하면 제품 방향이 잘못된다. |

## 지켜야 할 방향

- PropertyGrid 기반 tool 구조는 유지한다.
- image layer, input/output route, metric, overlay를 감추지 않는다.
- Preview, Run, Pipeline/Recipe 검증을 자동으로 섞지 않는다.
- Good/Bad pair sample은 단순 gallery가 아니라 검증 루프의 핵심 기능으로 다룬다.
- public-safe synthetic sample을 강점으로 유지한다.
- 타사 flowchart/debug UX는 참고하되, 카메라/PLC/통합 runtime 플랫폼으로 확장하지 않는다.

## 다음 우선순위

1. Pipeline/Recipe operator review UX 보강
   - NG 단계에서 failed metric, expected/actual, suggested fix, 관련 파라미터 위치를 더 가깝게 보여준다.
   - Product sample pair의 Bad counterpart를 여는 affordance를 step review와 더 자연스럽게 연결한다.

2. MainView/Product sample review 흐름 재점검
   - current EXE 캡처 기준으로 Sample Picker -> sample open -> Pipeline Review -> pair comparison 흐름을 한 번 더 걸어 본다.
   - UI/UX를 바꾸면 반드시 새 before/after 캡처를 남긴다.

3. Tool View code-behind 축소
   - 이미 추출된 공통 controller 패턴을 기준으로 반복 wiring만 줄인다.
   - PropertyGrid 구조와 tool별 학습 가능성은 유지한다.

4. Product sample catalog 추가 여부 판단
   - 현재는 샘플 추가보다 대표 샘플의 설명, 실패 이유, pair review affordance 강화가 우선이다.
   - 새 샘플은 metric margin이 분명하고 공개 안전성이 명확할 때만 추가한다.
