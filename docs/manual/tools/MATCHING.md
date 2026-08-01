# Matching: 회색조 Template 위치 찾기

## 목적과 준비

밝기 패턴이 비교적 일정한 한 물체의 위치, 점수, 각도, scale을 찾을 때
사용합니다. 반복 패턴에서는 검색 ROI와 unique 결과를 더 엄격히 검토합니다.

## 순서

1. 같은 물리 특징으로 유지되는 Template 영역을 정합니다.
2. 왼쪽 `매칭`을 엽니다.
3. 입력과 `Matching_Preview`를 확인합니다.
4. Template ROI를 등록하고 `Template Ready`를 확인합니다.
5. Search ROI를 지정합니다.
6. 최소 Score와 필요한 Count를 설정합니다.
7. 처음에는 angle/scale search를 끄고 고정 자세에서 확인합니다.
8. 실제 변화가 있을 때만 angle/scale 범위를 좁게 켭니다.
9. `미리보기 실행`을 누릅니다.
10. box와 중심이 실제 Template 물체 위에 있는지 확인합니다.
11. `ScoreMax`, Count, angle, scale, 대안 margin을 확인합니다.
12. 잘못된 반복 패턴이 통과하지 않는지 Bad에서 확인 후 저장합니다.

## 실패 확인 순서

Template 파일/ROI -> 입력 Layer -> Search ROI -> 최소 Score -> Count -> 대비 ->
angle -> scale 순서로 확인합니다. 높은 Score만으로 물리 특징의 동일성을
보증하지 않습니다.
