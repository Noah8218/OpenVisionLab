# Morphology로 binary 결과 정리하기

Updated: 2026-07-13

Morphology는 Threshold 이후의 흰 영역을 줄이거나 키워서 작은 노이즈, 구멍, 끊어진 부분을 정리하는 전처리입니다. Erode는 흰 영역을 줄이고, Dilate는 흰 영역을 키우며, Open은 작은 흰 노이즈 제거, Close는 끊어진 영역 연결에 자주 사용합니다.

## 사용할 sample

| 역할 | SampleName | 기준 |
| --- | --- | --- |
| Good | `Public_Morphology_Cleanup_Good` | `ResultCount=4` |
| Bad | `Public_Morphology_Cleanup_Missing_Bad` | `ResultCount=2` |

이 샘플들은 Threshold -> Morphology -> Contour 흐름을 사용합니다. Morphology는 최종 검출 도구가 아니라 Contour가 안정적으로 영역을 세도록 만드는 정리 단계입니다.

## 보는 순서

1. Threshold 결과 Layer에서 대상과 배경이 먼저 분리됐는지 확인합니다.
2. Morphology Tool을 열고 입력 Layer가 Threshold 결과인지 확인합니다.
3. Operation을 Erode, Dilate, Open, Close로 바꿔 결과 차이를 봅니다.
4. Kernel 크기를 3x3, 5x5, 7x7 순서로 올리며 대상이 사라지거나 붙는 지점을 확인합니다.
5. Preview를 직접 실행해 Morphology 결과 Layer를 확인합니다.
6. 다음 Contour/Blob 단계에서 count, area, box metric이 안정되는지 확인합니다.

## Learn에서 Tool View 열기

- `Morphology Tool 열기`는 기존 Morphology Tool View를 선택합니다.
- 공통 입력/출력 레이어와 `Operation`, Kernel `Width/Height`, `Shape`를 확인합니다.
- Kernel 크기는 `3 x 3`, `5 x 5`, `7 x 7` 프리셋으로 시작하고, Shape는 `Rect`, `Ellipse`, `Cross` 중 대상 구조에 맞게 비교합니다.
- Operation, Kernel 크기, Shape를 확인한 뒤 Preview에서 연산 전후의 형상 변화를 비교합니다.

## 실패 원인

| 증상 | 먼저 볼 것 |
| --- | --- |
| 작은 노이즈가 많음 | Open 또는 Erode, Kernel 크기 |
| 대상 내부 구멍이 큼 | Close 또는 Dilate |
| 대상들이 서로 붙음 | Dilate/Close가 과함 |
| 대상이 사라짐 | Erode/Open 또는 Kernel 크기가 과함 |

## 완료 기준

- Morphology 결과 Layer가 다음 Blob/Contour Step 입력으로 연결됩니다.
- Good sample에서는 count/area가 기대 범위 안에 들어옵니다.
- Bad sample에서는 같은 pipeline이 metric으로 NG를 설명합니다.
- Morphology 옵션을 바꾼 뒤 Preview에서 노이즈, 구멍, 연결 상태 변화를 확인합니다.
After changing Morphology parameters, run Preview and confirm that noise is removed without deleting or merging the target.

## Beginner path handoff

- Previous topic: Threshold. Use Morphology after the image is binary and the target/background polarity is already correct.
- This topic goal: make connected regions cleaner before Blob or Contour counts area, bounds, or shape.
- Practice Samples path: `preprocess`.
- Public sample pair: `Public_Morphology_Cleanup_Good` / `Public_Morphology_Cleanup_Missing_Bad`.
- Practice action: run Morphology Preview, then run Blob or Contour and compare the metric.
- Next topic: move to Blob for connected-region count/area, or Contour when boundary shape and bounds matter.
- Compare the before/after output layers because Morphology can remove noise but can also delete or merge the target.
