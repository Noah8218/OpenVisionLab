# Filter: 노이즈 제거와 선명화

## 목적과 준비

Threshold, Edge, Matching 전에 작은 노이즈를 줄이거나 경계를 보강할 때
사용합니다. 원본을 보존하고 별도 출력 Layer를 만듭니다.

## 순서

1. 제거할 것이 점 노이즈인지 밝기 흔들림인지 확인합니다.
2. 왼쪽 `필터`를 엽니다.
3. 입력 Layer와 `Filter_Preview` 출력을 확인합니다.
4. Filter 종류를 선택합니다.
5. Kernel 크기를 작은 홀수 값부터 시작합니다.
6. Gaussian/Bilateral이면 Sigma 관련 값을 확인합니다.
7. 필요한 경우 ROI를 설정합니다.
8. `미리보기 실행`을 누릅니다.
9. 원본과 결과를 비교해 노이즈와 실제 경계의 변화를 봅니다.
10. downstream Threshold/Edge 결과가 좋아졌을 때만 저장합니다.

## 선택 기준

- Gaussian: 부드러운 랜덤 노이즈
- Median: 점 형태 노이즈
- Bilateral: 경계를 보존하며 평활화
- Sharpen: 약한 경계 보강. 노이즈도 강해질 수 있음

## 실패 확인 순서

Filter 종류 -> Kernel -> Sigma -> 원본 대비 순서로 확인합니다. 대상 경계가
사라지거나 Blob이 합쳐지면 Kernel을 줄입니다.
