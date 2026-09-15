# OpenVisionLab ImageCanvas external consumer contract

Date: 2026-08-27
Status: validated prototype in the Dev repository
Contract: `IMAGE-CANVAS-EXTERNAL-CONSUMER-CONTRACT`

## 1. 목적

별도 `.NET 8 WPF` 소비자가 OpenVisionLab 애플리케이션 프로젝트를 참조하지 않고 기존 `OpenVisionLab.ImageCanvas`를 다음 최소 흐름으로 사용할 수 있는지를 검증했다. 또한 3D Result의 역투영 좌표를 같은 외부 Viewer에 읽기 전용 marker로 반영하는 교차 모달 확장을 검증했다.

1. 소비자가 소유한 이미지 또는 소비자에게서 이전받은 이미지를 Viewer에 업로드한다.
2. 소스 이미지 좌표계의 사각형 결과를 Viewer overlay로 표시한다.
3. 한 번에 하나의 결과를 선택하고 기존 CanvasRect editing visual과 굵은 선으로 하이라이트한다.
4. 선택 결과의 accepted/rejected 상태와 reject reason을 이벤트로 받는다.
5. Fit 후 외부 DTO로 Viewer 상태를 캡처하고 다시 적용한다.
6. 세션을 명시적으로 해제하고 반복 생성·해제한다.
7. 3D Result `coordinate-projection-result.json`을 검증하고 `3D->2D` 유효 점을 기존 Viewer overlay renderer로 표시한다.

이 작업은 파이프라인, 레이어, routing, Preview/Run을 외부 계약으로 공개하는 작업이 아니다. 외부 소비자는 Viewer와 표시 증거만 소유하며, 실행은 계속 OpenVisionLab Pipeline의 명시적 계약으로 남는다.

## 2. 현재 구현

### 공개 facade

파일: `src/Libraries/OpenVisionLab.ImageCanvas/External/ImageCanvasExternalConsumerSession.cs`

| 계약 | 동작 |
| --- | --- |
| `ImageCanvasImageOwnership.Borrow` | 업로드가 끝난 뒤에도 caller가 원본을 소유한다. |
| `ImageCanvasImageOwnership.Clone` | facade가 private copy를 만들어 업로드하고 copy만 해제한다. |
| `ImageCanvasImageOwnership.TakeOwnership` | 호출이 끝나기 전에 facade가 전달된 Bitmap을 해제한다. |
| `ImageCanvasSourceRectangle` | source-image top-left 좌표. `Right`/`Bottom`은 exclusive. |
| `ImageCanvasExternalOverlay` | id, geometry, accepted/rejected, reject reason, 색상, 선 두께를 소비자 metadata로 보관한다. |
| `AddOverlay` | source 좌표를 기존 CanvasRect 하단 원점 좌표로 변환하고 표시한다. |
| `SelectOverlay` | 단일 ID를 선택하고 `IsEditing`과 굵은 선으로 기존 renderer를 하이라이트한다. |
| `SelectionChanged` | 선택 ID와 결과 metadata를 소비자에게 전달한다. |
| `SetOverlayVisible`/`RemoveOverlay` | facade가 만든 overlay만 변경한다. 모르는 ID 제거는 `false` no-op이다. |
| `CaptureViewState`/`ApplyViewState` | 내부 `CanvasViewState`를 공개하지 않고 zoom/pan DTO로 round-trip한다. 유한값과 양수 zoom만 허용한다. |
| `Dispose` | WPF UI dispatcher에서만 호출하며 idempotent하다. 기존 ViewModel이 ImageCanvasControl과 native 자원을 해제한다. |

facade는 기존 `RoiImageCanvasViewModel`과 `ImageCanvasControl`을 재사용하지만 외부 host는 facade가 소유하는 작은 WPF `Grid`/`WindowsFormsHost`로 구성했다. 기존 `RoiImageCanvasView`의 shared layout 계약은 변경하지 않았다. 외부 `ContentControl` 아래에서 기존 view의 `WindowsFormsHost.ActualWidth`가 0으로 유지되는 재현 문제를 facade 내부 host로 격리한 결정이다.

### 별도 소비자 샘플

파일: `tools/ImageCanvasExternalConsumerSmoke/`

- 별도 `WinExe` `.NET 8 WPF` 프로젝트다.
- `OpenVisionLab` 애플리케이션 프로젝트를 `ProjectReference`하지 않는다.
- 왼쪽은 Viewer, 오른쪽은 overlay 선택 목록과 상태/뷰 상태 명령이다.
- accepted 후보와 rejected 후보를 동시에 그리고 rejected 후보를 선택해 노란색 하이라이트와 `WidthBelowMinimum` 사유를 표시한다.
- `--projection-smoke` 경로에서는 3D Result를 150ms dispatcher coalescing watcher로 관찰하고, `3D->2D` 유효 점을 `cross-modal-3d-to-2d:` marker로 교체 표시한다.
- 역투영 marker는 `DeepSkyBlue` 윤곽과 결과 상태 metadata를 사용하며, Preview/Run, acknowledgement, layer mutation은 호출하지 않는다.
- 샘플의 `--smoke <outputDir>` 경로는 UI를 자동 실행하고 결과를 D 드라이브에 보존한다.
- smoke에서 사용하는 모니터는 Windows topology를 런타임에 읽는다. 모니터가 정확히 2개일 때 면적이 작은 모니터를 선택하고, 동률이면 왼쪽 모니터를 선택한다. 실제 HWND 사각형은 물리 장치 좌표로 다시 배치하고 검증한다.

## 3. 의존성/소유권 경계

기계 판독용 목록은 [`OPENVISIONLAB_IMAGECANVAS_EXTERNAL_CONSUMER_MANIFEST.json`](../contracts/openvisionlab/OPENVISIONLAB_IMAGECANVAS_EXTERNAL_CONSUMER_MANIFEST.json)에 있다.

샘플 빌드 output에서 관찰된 런타임은 다음과 같다.

- `OpenVisionLab.ImageCanvas` — 직접 project reference
- `OpenVisionLab.Localization` — ImageCanvas의 전이 project reference
- `OpenCvSharp.dll` — SDK managed runtime
- `SharpGL.dll`, `SharpGL.WinForms.dll` — ImageCanvas native/OpenGL host
- `OpenCvSharpExtern.dll` — 단일 shared native runtime

샘플에는 `OpenVisionLab` 앱 project reference, NuGet package reference, SDK algorithm DLL reference가 없다. ImageCanvas의 SharpGL/OpenCV native lifetime과 texture/overlay 내부 상태는 library가 소유한다. 소비자는 descriptor와 선택 상태만 소유한다.

### 2D Result 소비와 marker ownership

파일: `src/Libraries/OpenVisionLab.ImageCanvas/External/ImageCanvasExternalProjectionResult.cs`, `tools/ImageCanvasExternalConsumerSmoke/MainWindow.xaml.cs`

- `ImageCanvasExternalProjectionResultReader`는 schema `1.0`, projection/transaction ID, 이미지·grid 차원, 유한 좌표를 검증한 뒤에만 Result를 소비한다.
- 화면은 `<exchange>\transactions`의 `coordinate-projection-result.json` 변경을 감시하고 150ms dispatcher timer로 알림을 합친다. 최신 `RecordedAtUtc` 결과만 적용한다.
- `GetVisibleThreeDToTwoD`가 `Valid`이며 이미지 경계 안에 있는 점만 반환하고, `ImageCanvasExternalConsumerSession.ReplaceProjectionMarkers`가 기존 projection marker를 제거·교체한다.
- 3D Result가 authoritative source이며 화면은 rendered marker ID, count, status만 소유한다. 읽기 과정은 pipeline 실행, layer 변경, route 변경, acknowledgement를 발생시키지 않는다.

## 4. 검증 결과

최신 current-source run artifact:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\imagecanvas-external-consumer-20260827-r29`

실행 명령:

```powershell
dotnet build tools\ImageCanvasExternalConsumerSmoke\ImageCanvasExternalConsumerSmoke.csproj -c Release --no-restore --nologo /nodeReuse:false
dotnet tools\ImageCanvasExternalConsumerSmoke\bin\Release\net8.0-windows7.0\ImageCanvasExternalConsumerSmoke.dll --smoke D:\OpenVisionLab-TestData\OpenVisionLab_Dev\imagecanvas-external-consumer-20260827-r29
```

`contract-smoke.txt` 결과:

- 이미지: `720x480`
- overlay: 2개
- ownership: `Borrow`, `Clone`, `TakeOwnership` 모두 실행했으며 Borrow/Clone caller Bitmap이 호출 후 계속 사용 가능함을 확인했다.
- `SelectedOverlay=candidate-rejected`
- `ViewStateRoundTrip=PASS`
- `CreateDisposeCycles=100`
- `WindowInsideTestMonitor=PASS`
- 모니터: `\\.\DISPLAY2`, bounds `{X=-2400,Y=456,Width=2400,Height=1350}`
- 물리 창 bounds: `-2360,496,1550,950`
- DPI scale: `1.25x1.25` (현재 125% 모니터 실행; 100/150/175/200%는 미실행)
- resource delta: handles `-5`, working set `+17211392` bytes
- native screen color assertion: accepted green, rejected red, selected yellow 모두 통과
- 최종: `Result=PASS`

보존한 증거:

- `contract-smoke.txt` — 계약 assertion, ownership, view-state, monitor, resource 기록
- `source.png` — 소비자 입력 이미지
- `external-consumer-selected-rejected.png` — 실제 HWND 화면 캡처. image, accepted/rejected rectangles, rejected selection/highlight, reject reason을 포함한다.

현재-build 화면 증거:

![ImageCanvas external consumer selected rejected](D:\OpenVisionLab-TestData\OpenVisionLab_Dev\imagecanvas-external-consumer-20260827-r29\external-consumer-selected-rejected.png)

기존 ImageCanvas 회귀 경로도 같은 current-source build에서 재검증했다.

```powershell
dotnet tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll --target wpf_imagecanvas_owned_mat_load D:\OpenVisionLab-TestData\OpenVisionLab_Dev\imagecanvas-existing-20260827-r3
dotnet tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll --target wpf_opengl_native_readback D:\OpenVisionLab-TestData\OpenVisionLab_Dev\imagecanvas-native-readback-20260827-r2
```

두 target 모두 `OK|check=OK|exit=0`이었다. 첫 target은 loader Mat을 dispose한 뒤 texture/save가 유지되는지, 두 번째 target은 native OpenGL readback와 색상/좌표 증거가 유지되는지를 확인한다. 외부 facade smoke와 별도 회귀 계약으로 기록했다.

교차 모달 화면 smoke도 current Release build에서 통과했다.

```powershell
dotnet tools\RunCrossModalProjectionSmoke.ps1
```

최종 cross-process 증거:

`D:\OpenVisionLab-TestData\OpenVisionLab-CrossRepo\projection\cross-modal-projection-20260827-163714-c93911d218f94f7e9b4586a7bda7dcae`

- 화면 consumer는 3D Result 발행 전에 `ProjectionScreenReady=PASS`로 준비되었다.
- `ProjectionRefreshCount=1`, `ProjectionMarkerCount=5`, `Result=PASS`.
- 화면은 `\\.\DISPLAY2`에서 선택되었고 `WindowInsideTestMonitor=PASS`, DPI는 `1.25x1.25`였다.
- `PipelineExecution=NotInvoked`, `LayerMutation=NotInvoked`로 읽기 전용 경계를 확인했다.
- 실제 화면 캡처: `...\2d-screen-consumer\cross-modal-2d-screen.png`.

![2D reverse-projected markers](D:\OpenVisionLab-TestData\OpenVisionLab-CrossRepo\projection\cross-modal-projection-20260827-163714-c93911d218f94f7e9b4586a7bda7dcae\2d-screen-consumer\cross-modal-2d-screen.png)

솔루션 수준 확인도 통과했다.

- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU" --no-restore` — 경고 0, 오류 0
- `OpenVisionReadinessCheck` — contract passed
- `TestExternalReferences.ps1` — vendored DLL check passed
- `TestPublicSampleAssets.ps1` — `PublicSampleAssetCheck=PASS`
- `dotnet sln OpenVisionLab.sln list` — `ImageCanvasExternalConsumerSmoke` 등록 확인

## 5. 범위 밖/다음 단계

이번 단계에서 의도적으로 공개하지 않은 항목:

- freehand canvas hit-test로 native pointer 위치에서 자동 선택
- multi-selection
- rectangle 이외의 line/contour/polygon/rotated-shape 공개 계약
- selection에 따른 Pipeline/Layer/Route 변경
- Preview/Run 또는 Run History 연결
- OpenVisionLab 본 애플리케이션 Pipeline Review에 역투영 marker를 직접 연결하는 작업
- calibration, lens distortion, camera pose, physical metrology
- `net48`, Linux/Avalonia, single-DLL redistribution, NuGet publication
- full renderer rewrite

다음 공개 계약을 추가하려면 먼저 실제 소비자 요구를 이름 붙여야 한다. 특히 pointer hit-test나 multi-selection은 기존 Canvas 좌표 변환, z-order, 숨김 overlay, group 경계의 acceptance criteria와 별도 runtime smoke가 필요하다. 현재 단계에서는 programmatic ID selection과 읽기 전용 3D 역투영 marker가 외부 ImageCanvas 경계에서 검증되었다.

## 6. 변경 경계

- 변경: ImageCanvas 외부 facade, projection Result reader/marker 경로, 별도 소비자 샘플, 솔루션 등록, 계약 manifest, 본 보고서
- 변경하지 않음: 원본 repository, release/tag/push/deployment, 기존 shared `RoiImageCanvasView` layout
- 이 문서는 release나 배포 승인이 아니다. 공개 release, 설치, rollout, push는 별도 사용자 승인과 release gate가 필요하다.
