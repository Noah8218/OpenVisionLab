# Line: 엣지·길이·거리·교차점

## 목적부터 선택

Line Tool을 연 뒤 가장 먼저 `Purpose`를 선택합니다.

- Edge: 한 ROI의 피팅 line과 edge 지지 확인
- Measure: Line A/B 사이 거리
- Intersection: Line A/B 피팅 line의 교차점

## 순서

1. 측정할 두 물리 경계와 방향을 정합니다.
2. 왼쪽 `라인`을 엽니다.
3. `Purpose`를 선택합니다.
4. 입력/출력 Layer를 확인합니다.
5. Line A를 선택하고 ROI, scan direction, polarity, contrast를 설정합니다.
6. Measure/Intersection이면 Line B도 별도로 설정합니다.
7. Scan interval과 필요 시 scan angle을 설정합니다.
8. mm가 필요할 때만 검증된 `mm per pixel` 값을 적용합니다.
9. `미리보기 실행`을 누릅니다.
10. edge point와 피팅 line이 실제 경계 위에 있는지 확인합니다.
11. Measure는 `DistancePxAvg/Range`, Intersection은 point와 cross 상태를 봅니다.
12. 서로 다른 방향/극성 샘플에서도 같은 경계를 선택할 때 저장합니다.

## 실패 확인 순서

Purpose -> Line A/B ROI -> scan direction -> polarity -> contrast -> interval ->
angle -> scale 순서로 확인합니다. 평균 거리만 맞고 line이 다른 구조 위에 있으면
잘못된 측정입니다.
