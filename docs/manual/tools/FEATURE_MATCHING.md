# Feature Matching: 특징점 기하로 위치 찾기

## 목적과 준비

회색조 질감과 고유한 특징점이 있는 대상의 위치를 Ratio/RANSAC 기하로 찾을
때 사용합니다. 단색이거나 반복 무늬인 대상에는 적합하지 않을 수 있습니다.

## 순서

1. 서로 구분되는 texture를 포함한 Template을 정합니다.
2. 왼쪽 `특징 매칭`을 엽니다.
3. 입력/출력 Layer를 확인합니다.
4. 특징 Template을 등록합니다.
5. detector/descriptor 기본 설정에서 시작합니다.
6. Ratio와 RANSAC 허용 오차를 확인합니다.
7. 필요한 검색 ROI를 설정합니다.
8. `미리보기 실행`을 누릅니다.
9. 연결 특징점과 inlier가 대상의 같은 부분을 잇는지 확인합니다.
10. 결과 box와 기하가 찌그러지거나 뒤집히지 않았는지 봅니다.
11. score/inlier 결과와 reject 이유를 확인합니다.
12. blur, 조명, no-target 샘플에서 같은 설정을 검증 후 저장합니다.

## 실패 확인 순서

Template texture -> 입력 blur -> ROI -> Ratio -> RANSAC -> 반복 무늬 순서로
확인합니다. 연결선이 많아도 물리적으로 잘못된 기하면 실패입니다.
