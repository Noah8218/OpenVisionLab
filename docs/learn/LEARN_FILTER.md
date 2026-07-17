# Filter로 노이즈와 경계 준비하기

Updated: 2026-07-07

Filter는 검출 도구가 보기 쉬운 입력 레이어를 만들기 위한 전처리입니다. Blur는 작은 노이즈를 줄이고, MedianBlur는 점 잡음을 줄이며, BilateralFilter는 경계를 비교적 유지하면서 표면 노이즈를 낮춥니다.

## 사용할 sample

| 역할 | SampleName | 기준 |
| --- | --- | --- |
| Good | `Public_Filter_Denoise_Good` | `ResultCount=4` |
| Bad | `Public_Filter_Denoise_Missing_Bad` | `ResultCount=2` |

이 샘플은 `SurfaceDefect_EdgeContour.pipeline.xml`을 사용합니다. 흐름은 Filter -> EdgeDetection -> Morphology -> Contour이며, Filter는 결함 주변 경계가 다음 단계에서 안정적으로 보이도록 만드는 준비 단계입니다.

## 보는 순서

1. Filter Tool을 열고 입력 Layer가 `Main`인지 확인합니다.
2. 결과 Layer 이름을 원본과 다르게 둡니다.
3. Blur, MedianBlur, BilateralFilter 중 어떤 방식이 노이즈를 줄이는지 봅니다.
4. Kernel 크기를 너무 크게 올리면 결함 경계도 사라질 수 있습니다.
5. Preview를 직접 실행해 Filter 결과 Layer를 확인합니다.
6. 다음 Edge/Contour 단계에서 `ResultCount`와 `AreaMax`가 어떻게 바뀌는지 확인합니다.

## Learn에서 Tool View 열기

- `Filter Tool 열기`는 기존 Filter Tool View를 선택합니다.
- 공통 입력/출력 레이어와 `Filter Type`, `Border Type`, Kernel `Width/Height`를 확인합니다.
- Median 계열은 `Median Kernel`, Bilateral은 `Diameter`, `Sigma Color`, `Sigma Space`를 추가로 확인합니다.
- Filter Type, Kernel, Border Type을 확인한 뒤 Preview에서 입력과 출력 영상을 비교합니다.

## 실패 원인

| 증상 | 먼저 볼 것 |
| --- | --- |
| 결함이 같이 사라짐 | Kernel 크기가 너무 큼 |
| 노이즈가 여전히 많음 | Filter 종류, Kernel 크기, Edge threshold |
| 경계가 번짐 | Blur 대신 Median/Bilateral 검토 |
| 다음 단계 count가 흔들림 | Filter 결과 Layer가 다음 Step 입력인지 확인 |

## 완료 기준

- Filter만으로 OK/NG를 판단하지 않습니다.
- Filter 결과가 다음 Edge, Blob, Contour 단계의 입력으로 쓰이는지 확인합니다.
- Filter 옵션을 바꾼 뒤 Preview에서 노이즈 제거와 경계 보존 결과를 확인합니다.
After changing a Filter parameter, run Preview and compare noise removal, edge preservation, and the downstream result.

## Beginner path handoff

- Previous topic: Threshold. Use Filter only when raw pixels are too noisy for a stable binary split or edge map.
- This topic goal: compare whether blur/median/bilateral preprocessing makes the next Threshold, Edge, Blob, or Contour metric more stable.
- Practice Samples path: `preprocess`.
- Public sample pair: `Public_Filter_Denoise_Good` / `Public_Filter_Denoise_Missing_Bad`.
- Practice action: run Filter Preview, then run the downstream tool and compare its final OK/NG metric.
- Next topic: move to Morphology when the image is already binary and the problem is small holes, specks, or broken connected regions.
