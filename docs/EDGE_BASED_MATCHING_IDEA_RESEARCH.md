# Edge Based Matching Idea Research

## 목적

엣지 기반 매칭을 이미지 매칭보다 빠르고 안정적으로 만들기 위한 후보 아이디어를 조사한 문서입니다. 이 문서는 바로 구현 지시가 아니라, 어떤 아이디어가 OpenVisionLab 구조에 맞는지와 구현 전에 무엇을 측정해야 하는지를 정리합니다.

## 현재 OpenVisionLab 상태

- 현재 엣지 기반 매칭은 템플릿에서 엣지/방향 정보를 만들고, 회전 모델 캐시와 coarse-to-fine 각도 탐색을 사용합니다.
- 최근 개선으로 이미지 매칭 제안을 같이 쓰는 hybrid candidate 경로와 `Model.*`, `Candidate.*` 진단 메트릭이 추가되었습니다.
- `artifacts/edge_based_candidate_diagnostics_20260627` 기준 측정:
  - `EdgeHybrid`: 50/50 성공, 평균 124.935 ms, 중앙값 114.588 ms
  - `EdgeOptimized`: 43/50 성공, 평균 86.718 ms
  - `ImageCoarse`: 50/50 성공, 평균 135.467 ms
  - `ImageExhaustive`: 50/50 성공, 평균 139.637 ms
  - fast path 18건, fallback 32건
  - image proposal selected 24건, fallback selected 27건
  - fallback에서 평균 약 38개 후보 검증
- 결론: 지금 상태에서 fallback을 제거하거나 이미지 매칭 제안만 신뢰하면 검출률을 잃을 가능성이 큽니다.

## 외부 알고리즘 조사 요약

| 계열 | 핵심 아이디어 | OpenVisionLab 적용 판단 |
| --- | --- | --- |
| HALCON shape-based matching | 모델 피라미드, 자동 각도 step, contrast/polarity, MinScore/Greediness, 모델 검사 | 1순위로 참고할 만합니다. 특히 피라미드 레벨별 모델 품질 검사와 greedy 후보 축소가 적합합니다. |
| Cognex PatMax RedLine | coarse filter 후 fine filter, coarse accept threshold, angle/scale tolerance, timeout, show/debug mode | coarse 후보를 유지/탈락시키는 별도 threshold와 timeout 옵션이 유용합니다. 단, coarse에서 정답을 버리면 fine에서 복구가 안 됩니다. |
| Adaptive Vision edge-based matching | 템플릿/이미지 피라미드, 위치/방향 후보, edge pixel gradient direction | 현재 구조와 가장 가깝습니다. 실제 피라미드 후보 유지 방식으로 발전시키기 좋습니다. |
| Euresys EasyFind | feature point 기반, 모델 feature 표시, don't-care 영역, occlusion/blur/illumination 대응 | don't-care/mask와 모델 feature 표시 UX는 제품화 가치가 높습니다. |
| OpenCV Generalized Hough | x/y/scale/rotation 검출 가능 | 프로토타입 비교용으로는 가능하지만 고해상도에서 느릴 수 있어 기본 엔진 대체로는 부적합합니다. |
| Chamfer / Directional Chamfer | distance transform으로 edge template 거리 계산, 방향 차이 포함 | 별도 연구 브랜치 가치가 있습니다. 현재 스코어링과 구조가 달라 바로 섞으면 리스크가 큽니다. |
| Dominant Orientation Templates / LINE2D 계열 | 국소 dominant orientation, bit coding, branch-and-bound | 장기적으로 가장 빠른 계열 중 하나지만 구현 난도가 큽니다. 현재 엔진 안정화 뒤 별도 모듈로 검토해야 합니다. |

## 적용 후보

### 1. 실제 edge-model pyramid 후보 유지

가장 우선순위가 높습니다.

- 현재는 coarse angle/step과 hybrid proposal이 있지만, HALCON/Adaptive Vision 방식처럼 모델 레벨별로 후보를 점진적으로 줄이는 구조는 아직 약합니다.
- 먼저 검색 동작을 바꾸지 말고, 피라미드 레벨별 edge point 수, 분포, 후보 유지율을 진단 메트릭으로 기록해야 합니다.
- 기대 효과: fallback 후보 38개 수준을 줄일 가능성이 큽니다.
- 위험: coarse 단계에서 정답 후보를 버리면 검출률이 바로 떨어집니다.

### 2. coarse accept threshold 분리

Cognex PatMax RedLine처럼 최종 `MinScore`와 coarse 단계 통과 threshold를 분리하는 방식입니다.

- 현재 최종 score 기준만 operator에게 보이기 때문에, coarse에서 얼마나 느슨하게 후보를 남길지 표현하기 어렵습니다.
- `CoarseAcceptScore` 같은 옵션은 유용하지만 기본값은 보수적으로 둬야 합니다.
- 적용 전 `Candidate.CoarseRetained`, `Candidate.CoarseRejected`, `Candidate.TrueTargetSurvived` 같은 진단이 필요합니다.

### 3. 모델 검사/표시 UX

상용 툴은 모델 학습 후 실제로 어떤 feature가 모델에 들어갔는지 보여주는 기능이 강합니다.

- 템플릿 등록 후 edge points, 방향 분포, 피라미드 레벨별 모델 이미지를 표시하면 operator가 왜 느린지/왜 못 찾는지 이해하기 쉽습니다.
- 성능 개선 자체보다 디버깅/티칭 품질을 높이는 항목입니다.

### 4. don't-care / mask 지원

Euresys 계열에서 중요한 기능입니다.

- 반복적으로 변하는 영역을 모델 score에서 제외하면 오검출과 score 흔들림을 줄일 수 있습니다.
- 기존 ROI/마스킹 UX와 충돌하지 않게 템플릿 학습용 mask와 실행 ROI를 분리해야 합니다.

### 5. scale search

이미지 피라미드/모델 피라미드가 안정화된 뒤 진행해야 합니다.

- scale까지 넣으면 angle x scale x position 후보가 증가합니다.
- 먼저 angle만 안정화한 뒤 scale tolerance를 제한적으로 추가하는 순서가 안전합니다.

### 6. Directional Chamfer / Distance Transform 프로토타입

정식 엔진 교체가 아니라 `.codex` 하위 실험 프로젝트로 비교해야 합니다.

- 장점: edge 위치 거리 기반이라 약간의 위치 흔들림과 edge 누락에 강할 수 있습니다.
- 단점: 현재 gradient dot-product scoring과 달라 score 해석, threshold, 속도 특성이 모두 바뀝니다.

### 7. OpenCV Generalized Hough 비교

OpenCV에 구현이 있어 비교 실험은 쉽습니다.

- 고해상도/복잡한 이미지에서 느릴 수 있고 parameter tuning이 까다롭습니다.
- 기본 엔진이 아니라 benchmark baseline으로 쓰는 것이 맞습니다.

### 8. Dominant orientation bit-coded template

장기 후보입니다.

- LINEMOD/LINE2D 계열처럼 orientation을 bitset으로 압축하고 branch-and-bound를 쓰면 매우 빨라질 수 있습니다.
- 다만 OpenVisionLab 엔진 안에 바로 넣기에는 변경 폭이 큽니다.

## 구현 전 실험 순서

1. Candidate survival audit
   - 큰 샘플 10종과 회전 synthetic 이미지에서 정답 위치가 coarse 단계 후보로 살아남는지 기록합니다.
   - 검색 결과를 바꾸지 않고 shadow metric만 추가합니다.
   - 2026-06-27 완료: `artifacts/edge_based_candidate_survival_audit_20260627` 기준 1/2 scale은 위치 50/50, pose 49/50 생존했습니다. 1/4 scale은 위치 40/50, pose 36/50으로 기본 후보 축소 단계로 쓰기에는 아직 위험합니다.

2. Pyramid model inspection
   - 레벨별 edge point 수, 이미지 크기, 방향 분포, 사분면 분포를 기록합니다.
   - 최상위 레벨에서 edge가 너무 적으면 해당 레벨은 사용하지 않아야 합니다.

3. Coarse threshold sweep
   - 동일 후보 로그를 기준으로 coarse accept threshold를 여러 값으로 바꿔 통과 후보 수와 정답 탈락률을 계산합니다.
   - 운영 기본값은 정답 탈락률 0을 기준으로 잡아야 합니다.

4. Mask/don't-care simulation
   - 템플릿 내 변동 영역을 제외했을 때 score 안정성과 오검출 변화를 비교합니다.

5. Chamfer/GHT prototype
   - production 코드가 아니라 `.codex` 실험 프로젝트에서만 속도/검출률을 비교합니다.

## 다음 개발 우선순위 제안

1. 검색 결과를 바꾸지 않는 pyramid/candidate 진단 메트릭 추가
2. 1/2 scale position proposal을 옵션/실험 경로로 구현하고, 원본 해상도 fallback은 유지
3. 큰 이미지/큰 템플릿 포함 10종 샘플 benchmark 기준표 작성
4. coarse accept threshold sweep 리포트 생성
5. 결과가 좋을 때만 실제 edge-model pyramid 후보 축소 적용
6. 이후 don't-care/mask와 scale search를 순차 적용

## 아직 하지 말아야 할 것

- fallback 제거 금지
- image proposal만으로 최종 후보 결정 금지
- fast path threshold를 근거 없이 낮추는 것 금지
- OpenCV Generalized Hough를 기본 엔진으로 교체 금지
- GPU/OpenCL을 CPU 구조 안정화 전에 먼저 적용 금지

## 참고 자료

- HALCON Solution Guide II-B Matching: https://www.mvtec.com/fileadmin/Redaktion/mvtec.com/products/halcon/documentation/solution_guide/solution_guide_ii_b_matching.pdf
- HALCON `create_shape_model`: https://www.mvtec.com/doc/halcon/12/en/create_shape_model.html
- HALCON `find_shape_model`: https://www.mvtec.com/doc/halcon/12/en/find_shape_model.html
- Cognex PatMax RedLine Theory: https://docs.cognex.com/is_630/web/EN/ise/Content/Reference/PatMaxRedLineTheoryOfOperation.htm
- Cognex FindPatMaxRedLine: https://docs.cognex.com/is-usp_2410/web/EN/InSight_Sheet/Content/Topics/Spreadsheet/VisionTools/FindPatMaxRedLine.htm
- Adaptive Vision Template Matching: https://docs.adaptive-vision.com/5.1/studio/machine_vision_guide/TemplateMatching.html
- Euresys EasyFind: https://www.euresys.com/en/products/software/easyfind/
- Euresys Learning Process: https://documentation.euresys.com/products/open_evision/open_evision_2_5/en-us/Content/03_Using_Open_eVision/C4_EasyFind_-_Matching_Geometric_Patterns/Learning_Process.htm
- Euresys Don't Care Areas: https://documentation.euresys.com/Products/OPEN_EVISION/OPEN_EVISION/en-us/Content/05_Resources/01_Tutorials/06_EasyFind/Improving_the_Score_of_Found_Instances_by_Using_Don_t_Care_Areas.htm
- OpenCV Generalized Hough tutorial: https://docs.opencv.org/4.x/da/ddc/tutorial_generalized_hough_ballard_guil.html
- Fast Directional Chamfer Matching: https://www.ece.rice.edu/~av21/Documents/pre2011/Fast%20Directional%20Chamfer%20Matching.pdf
- Dominant Orientation Templates for Real-Time Detection: https://vincentlepetit.github.io/files/papers/comp_hinterstoisser_cvpr10.pdf
- Distance Transform Templates for Object Detection and Pose Estimation: https://www.stefan-hinterstoisser.com/papers/holzerst2009distancetemplates.pdf

## 2026-06-27 Update: 1/2 Scale Position Proposal

- `artifacts/edge_based_pyramid_position_proposal_20260627` 기준으로 1/2 scale position proposal 옵션을 구현하고 검증했습니다.
- 권장 설정은 `Pyramid proposal top N = 6`, `Pyramid proposal min score = 0.70`입니다.
- 옵션은 기본 off입니다. 켜졌을 때만 1/2 scale proposal을 사용하고, proposal이 약하면 full-resolution fallback으로 돌아갑니다.
- 권장 top6 결과:
  - `EdgeHybrid:pyramid`: 50/50, 평균 54.268 ms
  - `EdgeOptimized:pyramid`: 50/50, 평균 49.290 ms
- top4는 `EdgeOptimized`에서 49/50으로 실패 이력이 있으므로 사용하지 않습니다.
- 다음 개선 후보는 더 많은 실제 큰 이미지/큰 템플릿 기준표 작성과 proposal acceptance threshold sweep입니다.

## 2026-06-27 Update: Large Template Benchmark

- `artifacts/edge_based_large_template_benchmark_20260627` 기준으로 full-resolution EasyMatch 이미지와 큰 템플릿 벤치마크를 추가했습니다.
- Large mode 실행 인자: `large`, `--large`, `fullres`, `--fullres`
- 결과:
  - `EdgeHybrid baseline`: 50/50, 평균 123.907 ms
  - `EdgeHybrid + Pyramid proposal`: 50/50, 평균 90.791 ms
  - `EdgeOptimized baseline`: 46/50, 평균 118.213 ms
  - `EdgeOptimized + Pyramid proposal`: 46/50, 평균 114.916 ms
- 결론:
  - 큰 템플릿에서는 `EdgeHybrid + Pyramid proposal`이 현재 추천 경로입니다.
  - `EdgeOptimized` 단독은 반복 패턴인 `FloppiesLarge`에서 실패하므로 생산 기본값으로 추천하지 않습니다.
  - 다음 후보는 proposal acceptance threshold sweep과 반복 패턴용 score/coverage/ambiguity 진단입니다.
