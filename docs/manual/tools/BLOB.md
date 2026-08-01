# Blob: 연결 영역 개수·면적·크기

## 목적과 준비

Threshold로 분리된 연결 영역을 세고 면적, 중심, axis-aligned bounds로
필터링할 때 사용합니다. 외곽 형상 자체가 중요하면 Contour를 검토합니다.

## 순서

1. 셀 대상과 제외 대상을 정합니다.
2. 왼쪽 `블랍`을 엽니다.
3. 입력 Layer와 `Blob_Preview`를 확인합니다.
4. 내부 Threshold를 쓸지 이전 Threshold Layer를 쓸지 정합니다.
5. Threshold 방향과 값을 설정합니다.
6. ROI를 설정합니다.
7. Min/Max Area를 설정합니다.
8. 필요할 때 Width/Height 범위를 설정합니다.
9. `미리보기 실행`을 누릅니다.
10. accepted/rejected object box, 중심, 이유를 확인합니다.
11. `ResultCount`, Area, Bounds Metric이 드로잉과 같은지 봅니다.
12. 맞으면 저장하고 Good/Bad를 같은 Pipeline으로 실행합니다.

## 실패 확인 순서

입력/Threshold 극성 -> ROI -> Morphology -> Area -> Width/Height -> 연결성 순서로
확인합니다. 개수가 맞아도 잘못된 물체를 세면 완료가 아닙니다.
