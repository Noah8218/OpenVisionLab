# OpenVisionLab Image Compare 좌표 매핑 책임 경계

## 범위

- 감사일: 2026-09-14 KST
- 저장소: `C:\Git\2D\Dev`
- 대상 Partial: `src/OpenVisionLab/UI/Popup/Wpf/ImageCompareWindow.xaml.cs`
- 관련 owner: `src/OpenVisionLab/UI/Popup/Wpf/ViewModels/ImageCompareViewModel.cs`
- 테스트: `tools/VisionRecipeRunnerSmoke/ImageComparePointMappingContract.cs`
- 범위 밖: 실제 사용자 데이터, 전체 WPF theme/DPI/monitor/input 행렬, 카메라·SDK·GPU,
  장시간 native 실행, `C:\Git\2D\Original`

이번 slice는 Partial 선언을 삭제하지 않았다. `ImageCompareWindow`는 XAML
`InitializeComponent`, `OpenFileDialog`, Window chrome, WPF pointer 이벤트와
`ImageCompareViewModel` 수명을 함께 소유하는 required Window boundary다. 대신
화면 크기와 포인터를 이미지 픽셀로 변환하는 순수 계산 정책을 Window에서 기존
ViewModel owner로 이동했다.

## 구조 변경

### 이전 owner

`ImageCompareWindow.SlotImage_MouseMove`가 `Image.ActualWidth`/`ActualHeight`,
`BitmapSource.PixelWidth`/`PixelHeight`, letterbox offset, scale, bounds clamp를
모두 계산했다. 이 코드는 XAML namescope나 `Image` 이벤트 구독 자체가 아니라
입력 좌표를 domain pixel 좌표로 변환하는 정책이었다.

### 현재 owner

`ImageCompareViewModel.TryMapDisplayedPoint`가 pixel 크기와 host 크기 및 포인터
값만 입력으로 받는 순수 정책을 소유한다. View는 WPF `Image`에서 측정값과
포인터를 읽어 이 메서드에 전달하고, 결과가 유효할 때 기존
`UpdatePixelStatus` 호출을 유지한다. 새 service/interface/wrapper/partial은
추가하지 않았다.

### 호출·상태·수명 경로

```text
ImageCompareWindow.SlotImage_MouseMove
  -> ImageCompareViewModel.TryMapDisplayedPoint(pixel/host/point snapshot)
  -> ImageCompareViewModel.UpdatePixelStatus(slot, x, y)
  -> ImageCompareSlotViewModel.Bitmap.GetPixel / bindable status projection
```

| 항목 | 현재 owner | 근거 |
| --- | --- | --- |
| caller | `ImageCompareWindow`의 `SlotImage_MouseMove` | WPF event가 측정값과 포인터를 전달 |
| 계산 정책 | `ImageCompareViewModel.TryMapDisplayedPoint` | WPF 객체 없이 scale/offset/bounds 계산 |
| mutable status writer | `ImageCompareViewModel.UpdatePixelStatus` | XY/RGB/GV/Delta/Swatch를 한 곳에서 기록 |
| 이미지 resource lifetime | `ImageCompareImageResource` → `ImageCompareSlotViewModel` | Bitmap/BitmapSource 생성·교체·Dispose |
| Window lifetime | `ImageCompareWindow.Dispose` | `Closed` 해제와 ViewModel Dispose |
| binding/public contract | `Slots`, `Source`, `Bitmap`, `StatusText`, `XyText`, `ColorText`, `DeltaText`, `SwatchBrush` | 기존 XAML/standalone `LoadImages` 경로 유지 |
| matching test | `ImageComparePointMappingContract` | center, letterbox, edge clamp, invalid dimensions |

## 검증

증거 루트:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\image-compare-point-mapping-20260914`

- `VisionRecipeRunnerSmoke` Debug build: 0 warnings / 0 errors
- `--image-compare-point-mapping-contract` Debug: 4/4 PASS
- `VisionRecipeRunnerSmoke` Release build: 0 warnings / 0 errors
- `--image-compare-point-mapping-contract` Release: 4/4 PASS
- `git diff --check` 대상 파일: PASS
- 정적 owner 확인: Window의 기존 `TryMapImagePoint` 제거, ViewModel 위임 및 순수
  `TryMapDisplayedPoint` 확인

## 판정

`ImageCompareWindow` Partial은 required XAML/Window adapter로 유지한다. 이번
slice에서 이동 가능한 수동 정책은 기존 ViewModel로 이동되어 Window가 더 이상
좌표 변환 알고리즘을 소유하지 않는다. Partial 선언 수 자체를 0으로 만드는
것은 목표가 아니며, 생성/XAML/framework/native/cohesive 경계는 독립 state·lifetime·
test owner가 증명될 때만 다시 연다.

실제 WPF 화면의 monitor-visible EXE, theme, DPI, keyboard/mouse full matrix와
OpenGL/GPU/native 장시간 수명은 이 slice에서 실행하지 않았다.

```text
Status: Complete
Scope: ImageCompareWindow의 표시 좌표 매핑 정책을 ImageCompareViewModel로 이동
Acceptance criteria:
  - Window가 WPF 측정값만 전달하고 계산 정책을 직접 소유하지 않음: 충족
  - 기존 pixel status/binding/resource/lifetime 계약 유지: 충족
  - focused Debug/Release contract 및 build 통과: 충족
Verification: ImageComparePointMappingContract Debug/Release 4/4, Smoke build Debug/Release 0 warning/error, git diff --check
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\image-compare-point-mapping-20260914
Boundary / next dependency: 전체 WPF visual/theme/DPI/monitor/input 및 hardware/native runtime 미검증
```
