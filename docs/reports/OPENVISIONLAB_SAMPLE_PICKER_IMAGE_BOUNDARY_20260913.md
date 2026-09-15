# OpenVisionLab Sample Picker 이미지 미리보기 경계 리팩터링

작성일: 2026-09-13 KST  
작업 원장: `PL-0030`  
대상: `C:\Git\2D\Dev`

## 사용자 목표와 범위

리팩터링을 계속하되 완료된 owner를 파일 크기만으로 다시 나누지 않는다.
이번에는 Sample Picker의 이미지 미리보기 경계를 한 개의 독립 책임으로
고정하고, 기존 concrete image factory를 재사용한다. 새로운 interface,
manager, wrapper, registry, Partial은 만들지 않는다.

## Refactor proof plan

### 현재 구조

- 현재 책임 owner: `OpenVisionWorkspaceSamplePickerViewModel.LoadImageSource`
- 현재 호출 경로: `OpenVisionWorkspaceSamplePickerWindow.xaml`의
  `SelectedImageSource` binding -> `SelectedSample` 변경 -> ViewModel의
  `LoadImageSource(SelectedSample.ImageFullPath)`
- 현재 dependency direction: Sample Picker ViewModel이 `File.Exists`와 WPF
  `BitmapImage` 생성/캐시/DecodePixelWidth 정책을 직접 소유
- 현재 mutable state writer: `OpenVisionWorkspaceSamplePickerViewModel`의
  `selectedSample`; image stream/state는 매 getter 호출의 local `BitmapImage`
- 현재 lifetime owner: ViewModel의 static loader가 `BitmapCacheOption.OnLoad`
  후 `Freeze()`를 수행

### 의도한 구조

- 새 책임 owner: 기존 `OpenVisionBitmapImagePreviewFactory`
- 새 호출 경로: `SelectedImageSource` binding -> ViewModel facade ->
  `OpenVisionBitmapImagePreviewFactory.TryCreateFromPath(path, 420)`
- 새 dependency direction: ViewModel은 이미지 경로를 전달하고, 기존 Viewer
  factory가 파일 존재 확인·OnLoad decode·freeze를 담당
- 새 state/data owner: 선택 상태는 ViewModel에 유지하고, 반환된 frozen
  `BitmapSource`의 stream-independent lifetime은 factory 결과가 보장

### 구조적 완료 조건

1. Sample Picker ViewModel에서 `File.Exists`, `BitmapImage` 생성, `OnLoad`,
   `DecodePixelWidth` 직접 호출이 사라진다.
2. 기존 factory가 missing/corrupt path를 null로 처리하는 Preview 전용
   entry point를 제공하고, 기존 throwing `CreateFromPath` 계약은 유지한다.
3. `SelectedImageSource`의 binding 이름/타입과 420px preview 동작이 유지된다.
4. 호출 경로와 old-coupling absence를 focused source contract가 증명한다.

### Proof checks

- Search: Sample Picker ViewModel old decode tokens absence; factory new path.
- Dependency: no new project, interface, wrapper, or Partial; existing factory
  remains the only decode owner for this path.
- Call path: XAML binding -> ViewModel facade -> existing factory.
- Test/build: `--workspace-sample-picker-image-boundary-contract`,
  `VisionRecipeRunnerSmoke` Debug build, documentation index, ledger validation,
  and targeted `git diff --check`.
- Runtime boundary: actual full WPF theme/DPI/input/monitor matrix and hardware
  are not claimed by this source/focused contract slice.

## Implementation result

### 구조 변경 확인

- Before: `OpenVisionWorkspaceSamplePickerViewModel.LoadImageSource`가 직접
  `File.Exists`, `BitmapImage.BeginInit`, `BitmapCacheOption.OnLoad`,
  `DecodePixelWidth=420`, `UriSource`, `Freeze`를 수행했다.
- After: ViewModel은 기존
  `OpenVisionBitmapImagePreviewFactory.TryCreateFromPath(path, 420)`를 호출하고,
  factory가 경로 존재 확인·OnLoad decode·420px 선택·freeze를 담당한다.
- 기존 throwing `CreateFromPath(path)` 계약은 유지하고, Sample Picker 전용
  `TryCreateFromPath`만 missing/corrupt path를 `null`로 반환한다.

### 호출 경로와 상태 흐름

`OpenVisionWorkspaceSamplePickerWindow.xaml`의 `SelectedImageSource` binding
-> `OpenVisionWorkspaceSamplePickerViewModel.SelectedImageSource`
-> `LoadImageSource(SelectedSample.ImageFullPath)`
-> `OpenVisionBitmapImagePreviewFactory.TryCreateFromPath(path, 420)`
-> frozen `BitmapSource` 반환.

선택 상태 writer는 계속 ViewModel의 `selectedSample`이고, 파일 stream과
decode lifetime은 factory의 OnLoad/frozen 결과가 소유한다. binding 이름·반환
타입·missing/corrupt `null` 동작은 변경하지 않았다.

### 실제 검증

- `WorkspaceSamplePickerImageBoundaryContract`: 6/6 PASS.
- `VisionRecipeRunnerSmoke` Debug build: 경고 0·오류 0.
- Release contract/build도 6/6 및 경고 0·오류 0으로 통과했다.
- Solution Debug/Release, `Invoke-RefactorAudit.ps1 -Verify`,
  `OpenVisionReadinessCheck` Debug/Release, `TestDocumentationIndex.ps1`,
  원장 검증, 대상 `git diff --check`도 통과했다.
- 전체 실행 요약: `D:/OpenVisionLab-TestData/OpenVisionLab_Dev/sample-picker-image-boundary-20260913/phase-summary.txt`
- Contract 증거: `D:/OpenVisionLab-TestData/OpenVisionLab_Dev/sample-picker-image-boundary-20260913/workspace-sample-picker-image-boundary-contract.txt`

Full WPF theme/DPI/input/monitor matrix, hardware/GPU/SDK, and long-running native
shutdown remain unverified for this source/focused contract slice.
