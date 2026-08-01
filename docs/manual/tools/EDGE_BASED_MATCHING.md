# Edge Based Matching: 윤곽 Template 위치 찾기

## 목적과 준비

밝기보다 물체 윤곽이 안정적인 경우 edge model로 위치를 찾습니다. Template의
물리적 동일성을 먼저 확인해야 하며 반복 윤곽은 ambiguity가 생길 수 있습니다.

## 순서

1. 고유한 윤곽을 포함한 Template ROI를 정합니다.
2. 왼쪽 `엣지 기반 매칭`을 엽니다.
3. 입력/출력 Layer를 확인합니다.
4. Template을 등록합니다.
5. `Edge Model`에서 Canny와 최소 gradient, 최대 point를 확인합니다.
6. Search ROI, 최소 Score, Count를 설정합니다.
7. 고정 자세에서 먼저 Preview합니다.
8. 필요할 때만 angle/scale, coarse search, position refine, hybrid verify를 켭니다.
9. `미리보기 실행`을 누릅니다.
10. edge box와 model point가 실제 윤곽 위에 있는지 확인합니다.
11. Score, angle, scale, ambiguity/alternative 결과를 확인합니다.
12. 반복 구조와 no-target에서 fail-closed인지 확인 후 저장합니다.

## 실패 확인 순서

Template ROI -> Canny/gradient -> Search ROI -> Score/Count -> 반복 윤곽 ->
angle/scale -> refine/hybrid 순서로 확인합니다. 속도 옵션은 정확도 기본값이
아니며 명시적으로 선택해야 합니다.
