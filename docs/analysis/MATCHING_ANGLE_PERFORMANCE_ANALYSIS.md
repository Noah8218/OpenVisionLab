# Matching 각도 검사 성능 분석

Updated: 2026-06-26

## 목적

이 문서는 OpenVisionLab의 이미지 매칭 각도 검사 속도를 개선하기 위해 `Lib.noah` 소스 기준으로 현재 알고리즘 흐름과 개선 후보를 정리한다.

Matching PropertyGrid UX는 2026-06-26 사용자 검증이 완료된 안정 영역이다. 아래 문서에 보호 항목으로 기록되어 있다.

- `docs/VISION_TOOL_PROPERTY_GRID_POLICY.md`
- `docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`
- `docs/OPENVISIONLAB_COMPLETED_TRACKER.md`

성능 개선 작업을 하더라도 Matching PropertyGrid의 검증 완료 UX는 새 회귀 증거 또는 명시적 재설계 요청 없이는 변경하지 않는다.

## 분석 기준

소스 기준 분석 대상:

```text
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs
```

참고 비교 대상:

```text
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\CVMatching.cs
```

`CVMatching`은 `[Obsolete("Legacy compatibility API. Use MatchingTool and MatchingResult for new OpenVisionLab code.", false)]`로 표시된 구형 호환 API다. OpenVisionLab의 현재 WPF preview/pipeline 경로는 `Lib.OpenCV.Tool.MatchingTool`을 사용한다.

## 현재 Matching 실행 흐름

`MatchingTool.Run()`은 ROI 구성에 따라 아래 중 하나로 실행된다.

- `ImagePyramidsSingleRun()`
  `C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:449`
- `ImagePyramidsMultiRun()`
  `C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:414`

두 경로 모두 source/template을 전처리한 뒤 `RunMatchingForSource()`를 호출한다.

```text
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:437
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:468
```

`RunMatchingForSource()`의 핵심 흐름:

1. source ROI를 crop하거나 clone한다.
2. template을 `ResizeByMagnification()`으로 축소/확대한다.
3. `NUM_MATCH`만큼 후보를 반복 탐색한다.
4. 각 반복에서 source를 다시 `ResizeByMagnification()`한다.
5. `FindBestMatchingCandidate()`로 최적 후보를 찾는다.
6. `TryRefineMatchingResult()`로 선택 후보를 refinement한다.

관련 위치:

```text
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:479
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:486
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:487
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:496
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:498
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:505
```

## 각도 검사 동작

각도 검사는 `FindBestMatchingCandidate()`에서 수행된다.

```text
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:517
```

현재 구조:

- 0도 template 검사를 `Task.Run()`으로 수행한다.
- `USE_FIND_ANGLE=true`이면 양수 각도와 음수 각도를 각각 별도 task로 수행한다.
- 양수/음수 각도 내부는 `Parallel.ForEach()`로 병렬 수행한다.
- 각 non-zero 후보 각도마다 `Rotate(imageTpl, angle, ...)`로 template을 회전한다.
- 회전된 template으로 `FindTemplate()`을 호출한다.
- `FindTemplate()`은 `Cv2.MatchTemplate()`을 호출한다.

관련 위치:

```text
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:522
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:527
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:531
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:533
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:535
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:542
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:544
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:546
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:222
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:226
```

각도 후보 생성:

```text
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:559
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:568
```

대략적인 후보 수:

```text
1 + floor(FIND_ANGLE_MAX / FIND_ANGLE) + floor(abs(FIND_ANGLE_MIN) / FIND_ANGLE)
```

예:

```text
FIND_ANGLE_MIN = -10
FIND_ANGLE_MAX = 180
FIND_ANGLE = 0.1
=> 약 1901개 각도 후보
```

## 핵심 병목

가장 비싼 단위는 다음 조합이다.

```text
후보 각도별 template 회전 + Cv2.MatchTemplate()
```

이 비용은 아래 조건으로 다시 커진다.

- `NUM_MATCH`가 1보다 크면 suppression 후 재검색한다.
- `USE_MULTI_ROI`이면 ROI 수만큼 반복된다.
- `MAGNIFIATION`에 따라 source/template 작업 크기가 달라진다.
- `TryRefineMatchingResult()`가 선택 후보 주변에서 다시 `Rotate()`와 `Cv2.MatchTemplate()`을 수행한다.

관련 위치:

```text
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:489
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:491
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:595
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:608
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:611
```

현재 구현은 이미 `Task.Run()`과 `Parallel.ForEach()`를 사용한다. 따라서 단순히 외부 병렬화를 더 추가하는 것보다 angle candidate 수와 `MatchTemplate()` 호출 횟수를 줄이는 쪽이 우선이다.

## 2026-06-26 측정 결과

측정 조건:

- Benchmark tool: `.codex/MatchingAngleBenchmark`
- Source images: `C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch\Die Pad 1.bmp` ~ `Die Pad 4.bmp`
- Template: `C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch\Die Pad Model 1.bmp`
- `MATCH_MODE=CCoeffNormed`
- `SCORE_MIN=0.60`
- `MAGNIFIATION=1.0`
- `NUM_MATCH=1`
- ROI/Threshold/Canny off
- Direct call path: `Lib.OpenCV.Tool.MatchingTool.Execute(Mat source)`

이번 실행 기준 결과:

| Case | Candidates | Time range |
| --- | ---: | ---: |
| Angle off | 1 | 16.5 ~ 61.4 ms |
| -10..10 / step 1 | 21 | 98.7 ~ 174.4 ms |
| -10..10 / step 0.1 | 201 | 542.7 ~ 761.4 ms |
| -10..180 / step 1 | 191 | 480.0 ~ 578.4 ms |
| -10..180 / step 0.5 | 381 | 975.5 ~ 1228.0 ms |
| -10..180 / step 0.1 | 1901 | 4153.8 ~ 6246.4 ms |

관찰:

- 실행시간은 후보 수 증가와 거의 비례한다.
- 같은 200개 안팎 후보라도 이미지별 매칭/후처리 상태에 따라 시간이 달라진다.
- `-10..180 / step 0.1`은 Die Pad 샘플 기준 4~6초대로, operator가 자동 preview로 실행하면 멈춘 것으로 느낄 수 있다.
- 1차 대응으로 UI에 후보 수와 slow warning을 표시하는 것은 합리적이다.

## 소스 기준으로 보이는 개선 포인트

### 1. 각도 후보 수를 UI에 표시

알고리즘을 바꾸기 전에 비용을 먼저 보이게 한다.

```text
Angle candidates: N
Wide/fine angle search
```

각도 범위와 간격이 과도한 후보 수를 만들 때 non-blocking warning을 표시한다. 특히 `AUTO_PREVIEW=true`일 때는 사용자가 오래 걸릴 수 있음을 알 수 있어야 한다.

이 항목은 결과 알고리즘을 바꾸지 않으므로 가장 안전하다.

2026-06-26 적용:

- `MatchingToolViewModel.Summary`에 `Candidates N` 표시를 추가했다.
- 후보 수가 500 이상이면 `Slow`, 1500 이상이면 `Very slow`를 표시한다.
- 계산식은 `Lib.noah`의 `CreatePositiveSearchAngles()` / `CreateNegativeSearchAngles()` 반복 방식과 맞춘다.
- 검증: `dotnet build .\src\OpenVisionLab\OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false` 통과.
- 검증: `tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_matching_tool" -FailOnWarn -OutputDir "artifacts\ui_precheck_matching_angle_candidates_20260626" -WpgCustomBuildEnabled false -TimeoutSeconds 420` 통과.

### 2. Coarse-to-fine 각도 탐색

전체 범위를 `FIND_ANGLE` fine step으로 모두 검사하지 않고 2단계로 나눈다.

권장 흐름:

1. Coarse pass
   - 전체 각도 범위를 2도 또는 5도 같은 큰 step으로 검사한다.
   - 가능하면 downscale source/ROI에서 먼저 검사한다.
   - 상위 K개 후보 각도만 남긴다.
2. Fine pass
   - 상위 후보 주변만 검사한다.
   - 예: `bestAngle +/- coarseStep`
   - 이 구간에서만 사용자의 `FIND_ANGLE` fine step을 적용한다.
3. Final pass
   - 최종 후보에 대해 기존 `TryRefineMatchingResult()` 흐름을 사용한다.

넓은 각도 범위에서 가장 효과가 큰 개선 후보로 판단한다.

2026-06-26 적용:

- `Lib.noah` `MatchingTool`에 coarse-to-fine 각도 탐색을 옵션으로 추가했다.
- 옵션은 기본값 `false`다. 기존 recipe와 기존 매칭 결과 흐름은 사용자가 켜기 전까지 바뀌지 않는다.
- 추가된 property:
  - `USE_COARSE_TO_FINE_ANGLE_SEARCH`
  - `COARSE_ANGLE_STEP`
  - `COARSE_ANGLE_TOP_K`
- OpenVisionLab Matching PropertyGrid에는 `Use angle search`가 켜진 경우에만 `Coarse angle search`가 보인다.
- `Coarse angle search`가 켜진 경우에만 `Coarse angle step`, `Coarse top K`가 하위 옵션으로 보인다.
- Matching summary는 option off일 때 `Candidates N`, option on일 때 `Coarse step N xK / Candidates ~effective/full`을 표시한다.
- Coarse tooltip은 operator가 언제 켜야 하는지 알 수 있게 "넓은 각도 범위를 큰 간격으로 먼저 찾고, 상위 후보 주변만 Angle step으로 다시 검사한다"는 의미를 설명한다.

검증 측정:

| Case | Candidates | Time range | Result |
| --- | ---: | ---: | --- |
| -10..180 / step 0.1 exhaustive | 1901 | 4258.3 ~ 5782.1 ms | Die Pad 1~4 OK |
| -10..180 / step 0.1, coarse step 5, top K 3 | 1901 full / estimated 342 | 481.7 ~ 781.0 ms | Die Pad 1~4 OK, best score/angle 동일 |

검증 명령:

```powershell
dotnet run --project .\.codex\MatchingAngleBenchmark\MatchingAngleBenchmark.csproj -c Release
dotnet build C:\Git\Library-Noah\Lib.Common.sln -c Release -p:Platform=x64 -m:1 -nr:false
dotnet build .\src\OpenVisionLab\OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_matching_tool" -FailOnWarn -OutputDir "artifacts\ui_precheck_matching_coarse_angle_20260626_diag" -WpgCustomBuildEnabled false -TimeoutSeconds 420
```

### 3. 회전된 template cache

`FindBestMatchingCandidate()`는 각 후보 각도마다 `Rotate(imageTpl, angle, ...)`를 수행한다.

반복 preview/run에서 template과 전처리 설정이 같다면 회전 template을 cache할 수 있다.

권장 cache key:

```text
PATTERN_PATH
template file timestamp
USE_THRESHOLD / THRESHOLD / THRESHOLD_TYPES
USE_ADAPTIVE_THRESHOLD / ADAPTIVE_THRESHOLD / BlockSize / Weight
USE_CANNY / CANNY_LOW / CANNY_HIGH
MAGNIFIATION
USE_PADDING_COLOR_WHITE
angle
```

단, memory가 늘 수 있으므로 LRU 방식 또는 template/preprocess 설정 변경 시 cache clear가 필요하다.

2026-06-26 적용:

- `MatchingTool`에 static bounded LRU rotated-template cache를 추가했다.
- cache key는 prepared/resized template의 content hash, width, height, type, padding mode, angle로 구성한다.
- cache는 stored `Mat`을 소유하고, 호출자는 read-only lease로 사용한다. 사용 중인 entry는 eviction 대상에서 건너뛰어 병렬 매칭 중 dispose되지 않게 한다.
- cache budget은 128 MB, 최대 4096 entry다.
- 작은 템플릿에서는 회전보다 hash/lock 비용이 더 커질 수 있으므로 32 KB 미만 prepared template은 cache를 사용하지 않고 기존 `Rotate()` 경로를 유지한다.
- EasyMatch `Die Pad Model 1.bmp`는 64x74 템플릿이라 cache threshold 아래다. 따라서 해당 샘플에서는 coarse-to-fine이 주 성능 개선 수단이고, rotated-template cache는 더 큰 템플릿/반복 preview/run을 위한 내부 최적화다.

큰 이미지 기반 추가 측정:

| Scenario | Template | Source | Candidates | Repeat 1 | Repeat 2~5 | Result |
| --- | --- | --- | ---: | ---: | ---: | --- |
| LargeTemplateRepeat | `DieModel.tif` 135x282 | `Die1.tif` 500x512 | 201 | 565.1 ms | 450.8 ~ 520.5 ms | Score 96.161, Angle 0.2 유지 |
| LargeCropRepeat | `Frame 4.bmp` crop 300x300 | `Frame 4.bmp` 768x576 | 201 | 1405.7 ms | 1458.7 ~ 1787.8 ms | Score 100 유지, 동점 후보로 Angle 변동 |

판단:

- 큰 템플릿에서는 cache warm 이후 일부 개선이 있다.
- 다만 EasyMatch 기준 병목은 회전 템플릿 생성보다 `Cv2.MatchTemplate()` 반복 호출 비중이 더 크다.
- 따라서 rotated-template cache는 반복 preview/run에서 보조 최적화로 유지하고, 넓은 각도 범위의 주 성능 개선은 coarse-to-fine 옵션으로 본다.
- `Frame 4.bmp` crop 테스트는 여러 각도에서 Score 100 동점 후보가 나와 angle이 변동된다. 이는 cache 검증용으로는 score/result 유지 확인까지만 사용하고, angle 안정성 검증 샘플로 쓰지 않는다.

검증:

- `dotnet build C:\Git\Library-Noah\Lib.Common.sln -c Release -p:Platform=x64 -m:1 -nr:false` 통과.
- `dotnet build .\src\OpenVisionLab\OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false` 통과.
- `.codex\MatchingAngleBenchmark`에서 EasyMatch Die Pad 1~4 및 큰 템플릿 반복 측정 확인.
- Matching UI smoke: `artifacts\ui_precheck_matching_rotated_template_cache_20260626_final` OK.

## Operator 성능 기준표

2026-06-26 기준 `C:\Git\OpenVisionLab_Dev\bin\Debug\EasyMatch` 샘플로 측정했다. 이 표는 operator에게 권장할 coarse 옵션의 기준값을 잡기 위한 baseline이며, 절대 시간은 PC 상태와 OpenCV runtime 상태에 따라 달라질 수 있다.

공통 조건:

- 매칭 방식: `CCoeffNormed`
- Score min: `0.60`
- Match count: `1`
- Threshold/Canny/ROI: off
- Wide angle: `-10..180`, `Angle step=0.1`
- Coarse 비교 조건: `Coarse angle step=5`, `Coarse top K=3`

| Sample | Template | Source | Search | Full candidates | Estimated candidates | Time | Score / Angle | 판단 |
| --- | --- | --- | --- | ---: | ---: | ---: | --- | --- |
| DiePadSmall | `Die Pad Model 1.bmp` 64x74 | `Die Pad 1.bmp` 640x484 | exhaustive | 1901 | 1901 | 5003.0 ms | 99.074 / 0 | 기존 정밀 탐색 |
| DiePadSmall | same | same | coarse | 1901 | 342 | 870.1 ms | 99.074 / 0 | 동일 결과, 약 5.7배 빠름 |
| DieModelLargeTemplate | `DieModel.tif` 135x282 | `Die1.tif` 500x512 | exhaustive | 1901 | 1901 | 4263.0 ms | 96.161 / 0.2 | 큰 template 파일 기준 |
| DieModelLargeTemplate | same | same | coarse | 1901 | 342 | 531.2 ms | 96.161 / 0.2 | 동일 결과, 약 8.0배 빠름 |
| Frame4LargeCrop | `Frame 4.bmp` crop 300x300 | `Frame 4.bmp` 768x576 | exhaustive | 1901 | 1901 | 14349.5 ms | 100 / 174.7 | crop template이 크면 10초 이상 |
| Frame4LargeCrop | same | same | coarse | 1901 | 342 | 2428.2 ms | 100 / 82.1 | 빠르지만 동점 후보로 angle 변동, 정확도 기준 샘플 아님 |
| BoardLargeCrop | `BOARD.JPG` crop 260x180 | `BOARD.JPG` 772x480 | exhaustive | 1901 | 1901 | 8474.7 ms | 100 / 0 | 큰 source/crop 기준 |
| BoardLargeCrop | same | same | coarse | 1901 | 342 | 1306.4 ms | 100 / 0 | 동일 결과, 약 6.5배 빠름 |

판단:

- Wide angle에서 `Angle step=0.1`을 그대로 exhaustive로 쓰면 작은 template도 수 초가 걸린다.
- 실제 template이 커지면 `Cv2.MatchTemplate()` 반복 비용이 지배적이라 10초 이상까지 올라간다.
- Coarse 옵션은 기본 off로 유지하되, `-10..180`처럼 넓은 각도 범위와 `0.1`처럼 작은 step을 같이 쓰는 recipe에서는 operator에게 우선 권장할 수 있다.
- `Frame4LargeCrop`처럼 source 내부 crop을 그대로 template으로 쓴 샘플은 여러 각도에서 score 100 동점이 나올 수 있다. 이 경우 성능 측정에는 유효하지만 angle 정확도 검증에는 적합하지 않다.

### 4. 같은 source resize 반복 제거

`RunMatchingForSource()`는 `while` 루프 안에서 매번 `ResizeByMagnification(imageSrc)`를 호출한다.

```text
C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool\MatchingTool.cs:496
```

`NUM_MATCH`가 2 이상이면 같은 source를 반복 resize하는 비용이 생긴다. suppression 때문에 source 자체가 변경되는 구조라 완전히 재사용하기는 어렵지만, 다음 중 하나를 검토할 수 있다.

- suppression을 original scale과 resized scale 양쪽에 일관되게 적용해 resized source를 유지한다.
- `NUM_MATCH=1`일 때는 resize 결과를 명확히 1회만 생성한다.

이 항목은 결과 차이를 만들 수 있어 테스트가 필요하다.

### 5. ROI-first teaching 유도

넓은 각도 검사는 ROI가 없으면 `MatchTemplate()` 대상 영역이 커져서 비용이 급증한다.

안전한 UX:

- 후보 각도 수가 높고 ROI가 꺼져 있으면 경고를 보여준다.
- ROI를 자동으로 강제하지 않는다.

### 6. Manual preview 기본값 유지

현재 검증된 Matching UX는 `AUTO_PREVIEW=false`가 기본이다. 이 정책을 유지한다.

각도 범위/간격 변경만으로 자동 실행되면 사용자는 멈춘 것으로 느낄 수 있다. 넓은 각도 검사는 operator가 명시적으로 preview를 누를 때 실행되는 흐름이 안전하다.

### 7. Early exit는 옵션으로만 제공

`NUM_MATCH=1`이고 강한 점수 기준을 넘는 후보가 나오면 빠르게 종료하는 fast mode를 만들 수 있다.

다만 뒤쪽 각도에서 더 좋은 후보가 나올 수 있으므로 기본 동작으로 넣으면 안 된다. 별도 옵션으로만 검토한다.

### 8. Edge/pose 사전 추정

일부 부품은 template matching 전에 대략적인 각도를 빠르게 추정할 수 있다.

- edge 방향성 추정
- contour/line 기반 orientation 추정
- 추정 각도 주변에서만 template matching 수행

다만 대칭 형상이나 texture가 약한 부품에서는 오판 가능성이 있으므로, 현재 Matching에 암묵적으로 섞지 말고 별도 옵션으로 둔다.

### 9. GPU/OpenCL은 후순위

`Cv2.MatchTemplate()` 자체를 GPU/OpenCL로 가속할 수 있는지 검토할 수 있다. 하지만 OpenCV 빌드 옵션, 배포 환경, OpenCvSharp runtime 제약이 있으므로 coarse-to-fine과 cache 이후에도 부족할 때 후순위로 검토한다.

여기서 CL은 OpenCL을 의미한다. OpenCL은 NVIDIA/CUDA 전용이 아니라 CPU, GPU, 내장 GPU 같은 여러 장치에서 쓸 수 있도록 만든 범용 병렬 연산 API다. OpenCV는 일부 연산에서 `UMat` 기반 T-API를 통해 OpenCL backend를 사용할 수 있다.

주의점:

- OpenCL은 "켜면 무조건 빨라지는 옵션"이 아니다.
- OpenCV runtime이 OpenCL을 포함해서 빌드되어 있어야 한다.
- 드라이버와 실제 장치가 안정적으로 잡혀야 한다.
- `Mat` CPU 데이터를 매번 GPU/OpenCL 장치로 보내고 다시 가져오면 전송 비용 때문에 이득이 사라질 수 있다.
- 각도 탐색처럼 회전 template을 많이 만들고 `MatchTemplate()`을 반복하는 구조에서는 source/template을 장치 메모리에 오래 유지할 수 있을 때만 효과가 커진다.
- OpenVisionLab 현재 경로는 OpenCvSharp `Mat` CPU path가 기준이다. OpenCL 검토는 CPU fallback을 반드시 유지한 별도 옵션/실험 경로로 해야 한다.

검토 순서:

1. 현재 배포 OpenCV runtime에서 `Cv2.Ocl.HaveOpenCL()` / `Cv2.Ocl.UseOpenCL()` 상태를 확인한다.
2. `UMat` 기반 prototype으로 preprocess, rotate, matchTemplate의 실제 지원 여부를 확인한다.
3. EasyMatch 큰 template/source baseline과 동일 조건으로 CPU path 대비 시간을 비교한다.
4. 빠르더라도 결과 score, angle, box가 CPU 기준과 허용 오차 안에 들어오는지 확인한다.
5. 효과가 있는 장치에서만 operator 옵션 또는 내부 자동 선택으로 검토한다.

2026-06-26 OpenCL probe 결과:

Probe:

```powershell
dotnet run --project .\.codex\OpenClProbe\OpenClProbe.csproj -c Release
```

Runtime:

| Item | Result |
| --- | --- |
| OpenCvSharp assembly | `OpenCvSharp, Version=1.0.0.0` |
| `Cv2.UseOptimized()` | `True` |
| OpenCV threads | `12` |
| OpenCV build OpenCL | `YES (NVD3D11)` |
| CUDA modules | unavailable |
| OpenCvSharp `UMat` type | not exposed |
| OpenCvSharp `Ocl` type | not exposed |

CPU `Mat` 경로 단일 `Cv2.MatchTemplate()` 기준:

| Sample | Source | Template | Repeat | Min | Avg | Max |
| --- | --- | --- | ---: | ---: | ---: | ---: |
| Frame4Crop300 | 768x576 | 300x300 | 20 | 31.045 ms | 39.493 ms | 81.538 ms |
| BoardCrop260x180 | 772x480 | 260x180 | 20 | 13.926 ms | 20.202 ms | 26.333 ms |
| DieModel | 500x512 | 135x282 | 20 | 8.800 ms | 9.707 ms | 10.750 ms |

판단:

- 네이티브 OpenCV 빌드는 OpenCL을 포함하지만, 현재 OpenCvSharp 래퍼에서 `UMat`/`Ocl`을 직접 사용할 수 없다.
- 현재 OpenVisionLab/Lib.noah 매칭 경로는 `Mat` 기반 CPU path로 봐야 한다.
- CUDA 모듈도 현재 runtime에서는 사용할 수 없다.
- 단일 `MatchTemplate()`은 수십 ms 수준이지만 각도 탐색에서 후보가 1901개로 늘면 이 비용이 누적되어 수 초~10초 이상으로 커진다.
- 따라서 현재 배포 구조에서 즉시 효과가 큰 방법은 OpenCL보다 angle candidate 수를 줄이는 coarse-to-fine이다.
- OpenCL을 실제 기능으로 검토하려면 OpenCvSharp 버전/배포 DLL 교체, `UMat` 접근 가능 여부, 또는 별도 native bridge를 먼저 검토해야 한다.

## 권장 진행 순서

1. Matching summary/status에 각도 후보 수와 warning을 표시한다.
2. EasyMatch 샘플 기준으로 `ExecuteMatchingPreview()` 실행시간을 측정하는 작은 benchmark/smoke를 추가한다.
3. `MatchingTool.cs`에 coarse-to-fine 탐색을 옵션으로 추가했다. 기본 off를 유지하고 operator가 넓은 각도 범위에서 필요할 때 켠다.
4. 반복 실행이 여전히 느리면 bounded rotated-template cache를 추가한다.
5. 그래도 부족하면 pose 사전 추정 또는 GPU/OpenCL을 검토한다.

## 건드리면 안 되는 안정 계약

- Matching PropertyGrid 검증 완료 UX
- manual preview 기본값
- `AUTO_PREVIEW` opt-in 동작
- `USE_COARSE_TO_FINE_ANGLE_SEARCH` 기본 off 및 명시적 opt-in 동작
- `FIND_ANGLE_MIN` / `FIND_ANGLE_MAX` 단일 RangeEditor 행
- XML/execution용 `FIND_ANGLE_MAX` descriptor/model property
- template editor image 표시, ROI 이동/크기 조절, template status 표시
- result review의 Count, Score, Center, Box, Angle, Tact 표시
