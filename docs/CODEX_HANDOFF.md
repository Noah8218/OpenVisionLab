# CODEX HANDOFF

## 0. 최신 명명 정리 기록

- 2026-06-08: 이미지 파일 로드/텍스처 업로드 유틸을 현재 역할 기준 이름으로 정리했다. 파일은 `Util/CanvasImageLoader.cs`, 클래스명은 `CanvasImageLoader`로 사용한다. 공개 메서드는 `LoadMatFromFile()`과 `UploadMatAsTexture()` 기준이며, 호출부는 `RoiImageCanvasViewModel`, `FormImageCompare`, `FormLayerDisplay`, `VisionTestImageCanvas` 기준으로 갱신했다. 이전 이름 검색 결과는 소스/문서 기준으로 남지 않으며, Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드에 성공했다. 빌드에는 기존 `OpenVisionLab.ImageCanvas` Windows API CA1416 경고가 남아 있다.
- 2026-06-08: `Library/OpenVisionLab.ImageCanvas`에서 툴킷 전용 잔재를 삭제했다. 삭제 대상은 비활성 `Command` 폴더, `Toolkit` 폴더, `View/ToolKitImageCanvasView.xaml(.cs)`, `ViewModel/ToolKitImageCanvasViewModel.cs`다. 활성 뷰어에서 쓰는 `CanvasImageLoader.LoadMatFromFile()`/`UploadMatAsTexture()`는 유지하되, 툴킷 마스크/ODB 저장 전용 메서드와 `EnumToolkitEditMode`/툴킷 드로잉 enum은 제거했다. 또한 `OpenGL` 폴더의 `Gl*` 접두어 클래스/파일을 `OpenGl*` 이름으로 변경했다. 주요 변경은 `GlDrawing` -> `OpenGlDrawing`, `GlRenderer` -> `OpenGlRenderer`, `GlTextureDrawingParam` -> `OpenGlTextureDrawingParam`, `GlDrawTextOptions` -> `OpenGlTextDrawOptions`, `GlFontBitmapEntry` -> `OpenGlFontBitmapEntry`, `GlFontRenderOptions` -> `OpenGlFontRenderOptions`, `GlOverlayMethodsExtension` -> `OpenGlOverlayExtensions`다. 소스 기준 `ToolKit`/`Toolkit`/`EnumToolkitEditMode`/`Gl*` 접두어 타입 검색 결과가 없고, Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드에 성공했다. 빌드에는 기존 `Lib.OpenCV` 미사용 변수 경고와 `OpenVisionLab.ImageCanvas`의 Windows API CA1416 경고가 남아 있다.
- 2026-06-07: Vision Test 주요 검사 폼의 `RJButton` 디자이너 스타일을 런타임 `VisionTestForm.ApplyVisionTestCompactStyle()` 값과 맞췄다. 대상은 `FormVision_Arithmetic`, `Blob`, `Contour`, `EdgeDection`, `FeatureMatching`, `Filter`, `Histogram`, `HSV`, `Line`, `Matching`, `Mean`, `Morphology`, `RotateAndScale`의 디자이너 버튼이며, 배경 `250,252,253`, 글자/아이콘 `35,85,132`, 테두리 `47,111,171`, `Segoe UI 9 Bold`, `BorderRadius = 3`, `BorderSize = 1`, `ControlStyle.Glass`로 통일했다. 보조 팝업인 `FormVision_NewPanel`, `FormVision_Result`는 이번 런타임 스타일 통일 대상에서 제외했다. Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드에 성공했고, `Lib.OpenCV`의 기존 미사용 변수 경고만 남았다.
- 2026-06-07: 나머지 주요 Vision Test 폼 레이아웃도 코드비하인드 런타임 재배치가 아니라 WinForms 디자이너 좌표로 직접 이전했다. `FormVision_Blob`, `Contour`, `FeatureMatching`, `Matching`, `Mean`은 왼쪽 `Input/Output` 2단 뷰어와 오른쪽 파라미터/실행 버튼 구조로 정리했고, `Filter`, `Histogram`, `Morphology`는 왼쪽 뷰어 2단 + 오른쪽 Operations/Kernel/Shapes 설정 구조로 정리했다. `HSV`와 `Rotate / Scale`은 조작 슬라이더를 오른쪽 컬럼에 배치했고, `Line`은 Edge 선택, 파라미터, Details/Edge/Fit Line/Vertical Line 버튼을 오른쪽 작업 컬럼으로 분리했다. `FormVision_NewPanel`과 `FormVision_Result`는 보조 팝업이라 이번 레이아웃 일괄 변경 대상에서 제외했다. Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드에 성공했다.
- 2026-06-07: Vision Test 폼 공통화를 완료했다. `VisionTestForm`에 `InitializeLayerList()`, `AcceptUserImageChange()`, `SelectLayer()`, `CreateDestinationLayer()`, `PublishResult()`, `InitializeSingleInputViewers()`를 추가했고, 실제 UI 컨트롤 타입인 `RJComboBox` 기준으로 레이어 선택/사용자 이미지 변경/결과 표시 흐름을 중앙화했다. 단일 입력 폼은 공통 초기화 헬퍼를 사용하고, `FormVision_Arithmetic`은 2개 입력 + 1개 결과 뷰어 구조를 별도 초기화하되 같은 공통 헬퍼를 사용한다.
- 2026-06-07: Vision Test 결과 표시 흐름을 정리했다. 각 검사 폼이 직접 `SetLayerImage()`와 `eventUpdateDisplay`를 호출하던 경로를 `PublishResult()`로 모았고, `VisionTestImageCanvas`는 `DisplayImage`, `DisplayBitmap`, `UserImageChanged`를 새 이름으로 제공한다. 기존 호출부 호환을 위해 `Image`, `Bitmap`, `ImageChanged`는 alias로 남겨두었다.
- 2026-06-07: ImageCanvas 텍스처 수명 정책과 CA1416 Windows 경고 정리를 확인했다. `VisionTestImageCanvas.Dispose()`는 `ClearTextureStateOnly()`로 transient 뷰어의 managed 상태만 비우고, 실제 표시 소유 뷰어는 기존 `ClearTexture()`로 OpenGL texture id까지 삭제한다. `OpenVisionLab.ImageCanvas.csproj`는 `net8.0-windows`와 `SupportedOSPlatformVersion` 설정을 사용하며, Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드에서 CA1416 경고 없이 성공했다.
- 2026-06-07: Vision Test 폼 WinForms 디자이너 로드 시 `System.Windows.Controls.WpfPropertyGrid` 원본 DLL을 찾지 못해 디자이너가 열리지 않는 문제를 수정했다. 원인은 일부 `FormVision_*Designer.cs`에 사용되지 않는 `hostedComponent* = new System.Windows.Controls.WpfPropertyGrid.PropertyGrid()` 잔재가 남아 디자이너가 원본 WPF PropertyGrid를 직접 로드하던 것이었다. 해당 디자이너 잔재를 제거했고, `VisionTestForm` 생성자에서는 Visual Studio/DesignToolsServer 디자인 타임을 감지하면 WPF PropertyGrid host 생성을 건너뛰도록 했다. `wpg` 필드는 `IPropertyGridView`로 낮추고, 런타임에서만 구체 `WpfPropertyGridBridge` 기반 PropertyGrid를 생성한다.
- 2026-06-07: Vision Test 폼 표시 속도 개선을 적용했다. `VisionTestForm` 생성자에서 WPF PropertyGrid를 즉시 생성하지 않고, `AttachPropertyGrid()`가 호출되는 `Blob`, `Contour`, `FeatureMatching`, `Line`, `Matching`, `Mean` 계열에서만 lazy 생성/부착한다. `InitializeSingleInputViewers()`와 `FormVision_Arithmetic`은 초기 이미지 복사, OpenCV Mat 변환, OpenGL 텍스처 업로드, `ZoomToFit()`을 `BeginInvoke`로 지연해 폼 shell이 먼저 뜨도록 했다. `VisionTestImageCanvas` 내부의 실제 `ImageCanvasControl`도 이미지/줌이 처음 필요할 때 생성하도록 lazy화했고, `[PERF]` 로그로 생성자 shell, PropertyGrid 생성, viewer event init, 초기 이미지 로드, GL viewer 생성, texture upload 시간을 남긴다.
- 2026-06-07: 추가 리팩토링 1~6번을 진행했다. `VisionTestImageCanvas`의 구식 호환 alias(`Image`, `Bitmap`, `ImageChanged`)를 제거하고 `DisplayImage`, `DisplayBitmap`, `UserImageChanged`만 남겼다. `VisionTestForm`에는 단일 입력 폼용 공통 헬퍼(`InitializeSingleInputLayerList`, `AcceptSourceImageChange`, `AcceptDestinationImageChange`, `SelectSourceLayer`, `SelectDestinationLayer`, `CreateSingleInputDestinationLayer`)를 추가했고, 단일 입력 `FormVision_*` 폼들의 반복 본문을 해당 헬퍼 호출로 정리했다. `PropertyGrid.Abstractions.IPropertyGridView`에 이벤트를 추가해 `CWpgEvent`는 WPF `PropertyGrid` 구체 타입 대신 `IPropertyGridView`/`PropertyGridPropertyValueChangedEventArgs`를 사용한다. 커스텀 editor 정의 때문에 `CPropertyGridEditor`의 WPF editor 타입 의존성은 유지했다.
- 2026-06-07: 사용하지 않는 옛 `Library/Controls.Viewer2D` 폴더를 삭제하고, `OpenVisionLab.csproj`의 해당 폴더 제외 항목도 제거했다. 현재 실사용 뷰어 프로젝트는 `Library/OpenVisionLab.ImageCanvas`다. 삭제 후 `Controls.Viewer2D`/`ToolKit2DViewer`/`Editable2DViewer` 검색 결과는 없고, Debug / Any CPU 별도 출력 폴더 빌드에 성공했다.
- 2026-06-07: 추가 리팩토링 후속으로 `CViewer`를 순수 레이어 ROI 상태 컴포넌트로 축소했다. ImageBox 시절 컨텍스트 메뉴/FontAwesome 메뉴/10ms 타이머가 있던 `CViewer.Designer.cs`와 `CViewer.resx`를 삭제했고, `CViewer.cs`에서는 빈 `InitializeComponent()` 호출, 빈 타이머 핸들러, 사용하지 않는 `displayTitle` 저장을 제거했다. `VisionTestForm`에는 `ApplyVisionTestCompactStyle()`을 추가해 Vision Test 폼의 콤보/버튼/아이콘 크기를 런타임에서 compact하게 보정한다. FontAwesome은 `RJControls`/메인 프레임 계층에 아직 남아 있으나, `CViewer`에서는 제거됐다.
- 2026-06-07: `FormVision_*` 폼이 Euresys 계열 예제/라이브러리 UI를 참고해 작성된 것으로 보인다는 사용자 우려를 검토했다. 기능적 아이디어, 알고리즘명, 일반적인 Source/Destination/Parameter 구성 자체보다, 특정 제품 UI의 전체 배치, 색상, 아이콘, 문구, 버튼 순서, 파라미터 노출 방식이 비슷하게 남아 있으면 저작권/트레이드드레스 분쟁 여지가 커질 수 있다. 향후 안전성을 높이려면 Vision Test 폼을 OpenVisionLab 고유 레이아웃으로 재설계하고, 문구/색상/아이콘/워크플로우를 차별화하는 방향을 권장한다.
- 2026-06-07: 위 UI 유사성 우려를 낮추기 위한 1차 변경을 적용했다. `VisionTestForm.ApplyVisionTestCompactStyle()`에서 Vision Test 폼 공통 배경/버튼/콤보 스타일을 OpenVisionLab 계열의 gray-blue 톤으로 보정한다. 단, Vision Test 폼들은 절대좌표 디자이너 배치가 많으므로 공통 스타일러가 `RJButton`/`RJMenuIcon`의 위치, 크기, 높이, 아이콘, 텍스트 정렬은 변경하지 않도록 제한했다. 사용자 노출 문구는 `Input Layer`/`Output Layer`, `Run`/`Details`, `FormVision_Arithmetic`의 `Input A/B`로 정리했다. 새 레이어 생성 툴팁도 `Create Output Layer`로 변경했고, `Source Image`/`Destination Image`/`EXCUTE`/`RESULT`/`Create New Layer` 검색 결과가 남지 않도록 공통 스타일러 조건문도 컨트롤 이름 기준으로 좁혔다. Debug / Any CPU 별도 출력 폴더 빌드에 성공했다.
- 2026-06-07: Vision Test 폼 표시 직후 `RJButton.IconSize = 0` 때문에 `ArgumentException(Parameter is not valid)`이 발생하던 문제를 수정했다. FontAwesome/RJButton 계열은 0 크기 아이콘을 허용하지 않으므로, 아이콘은 `IconChar.None`으로 숨기고 `IconSize`는 최소 유효값인 `1`로 유지한다. `IconSize = 0` 잔여 검색 결과는 없고, Debug / Any CPU 별도 출력 폴더 빌드에 성공했다.
- 2026-06-07: Euresys 예제/라이브러리 UI와 전체 배치가 닮아 보일 수 있다는 우려 때문에 Vision Test 폼 레이아웃 재설계를 진행 중이다. 처음에는 `VisionTestForm.ApplyVisionTestWorkflowLayout()` 런타임 재배치로 빠르게 검증했지만, WinForms 뷰는 디자이너에서 관리해야 한다는 방향에 맞춰 해당 런타임 레이아웃 호출과 메서드 블록을 제거했다. 우선 `FormVision_EdgeDection.Designer.cs`와 `FormVision_Arithmetic.Designer.cs`에 새 좌표/크기를 직접 반영했다. `Edge Detection`은 왼쪽 `Input/Output` 2단 뷰어와 오른쪽 `Operations/Tab/Run` 구조이고, `Arithmetic`은 상단 `Input A/Input B/Output` 3뷰어 스트립과 하단 `Input B/Operation`, `Contrast`, `Shift`, 전체 폭 `Run` 버튼 구조다. Debug / Any CPU 별도 출력 폴더 빌드에 성공했다.
- 2026-06-07: 레이아웃 패널 표시용 `FormLayerDisplay`를 Cyotek `ImageBox`/`CViewer` 직접 호스팅에서 `ImageCanvasControl` 직접 호스팅으로 전환했다. `FormLayerDisplay`는 `SetImage()`, `GetCurrentImage()`, `RefreshViewer()`, `ZoomToFit()`, `AcceptImageChanged()`를 제공하고, 이미지 텍스처는 폼 표시 이후 지연 로드한다. 상태바는 `ImageCanvasControl.PixelColor`, `PixelPos`, `GrayValue`, `ZoomScale` 기준으로 갱신한다.
- 2026-06-07: `FormLayerDisplay`를 GL 뷰어로 전환하면서 빠졌던 우클릭 이미지 컨텍스트 메뉴를 복구했다. `ImageCanvasControl` 우클릭 시 `RoiImageCanvasView`와 같은 `Load Image`/`Save Image` 항목이 아이콘과 함께 뜨며, 로드된 이미지는 `FormLayerDisplay`와 `ImageSpace`에 함께 동기화된다. WinForms 호스트용 메뉴 생성은 `CanvasContextMenuFactory`로 분리했다.
- 2026-06-07: `FormLayerDisplay`의 Layer 이미지 로드 시 8bpp/Indexed/stride 이미지가 OpenGL 텍스처에서 깨질 수 있어, 텍스처 업로드 전 `Format24bppRgb` 비트맵으로 정규화하도록 수정했다. 하단의 기존 흰색 `R,G,B/X,Y/GV/ZOOM` RJLabel 상태바는 숨기고, OpenGL ROI 뷰어와 같은 어두운 `Pos/Gv/Color` 스타일 상태바로 교체했다. Layer 컨텍스트 메뉴 아이콘도 Edit 메뉴와 같은 검정색으로 맞췄다.
- 2026-06-07: `0. UI/6) Vision Test` 계열 ImageBox 전환 인벤토리를 시작했다. 대부분은 `ibSource` + `ibDestination` 단일 입력/단일 결과 패턴이고, `FormVision_Arithmetic`은 `ibSource1`/`ibSource2`/`ibDestination` 3개 뷰어라 후순위로 분류했다. 이후 `FormVision_Arithmetic`과 Vision Test 범위 밖의 `FormMeasure`까지 전환 완료했다.
- 2026-06-07: Vision Test용 WinForms GL 래퍼 `VisionTestImageCanvas`를 추가했다. 기존 폼 코드가 기대하는 `Image`, `ImageChanged`, `MouseClick`, `ZoomToFit()`, `Invalidate()` 표면을 유지하면서 내부는 `ImageCanvasControl`로 렌더링한다. 우클릭 메뉴는 `CanvasContextMenuFactory`를 사용하고, 이미지 업로드 전 `Format24bppRgb`로 정규화한다.
- 2026-06-07: 단일 입력/단일 결과 Vision Test 폼의 `ibSource`/`ibDestination`을 Cyotek `ImageBox`에서 `VisionTestImageCanvas`로 교체했다. 대상은 `FormVision_Blob`, `Contour`, `EdgeDection`, `FeatureMatching`, `Filter`, `Histogram`, `HSV`, `Line`, `Matching`, `Mean`, `Morphology`, `RotateAndScale`다. 각 폼의 `source_1.LoadImageBox()`/`destination.LoadImageBox()` 연결은 제거했고, 기존 레이어 선택/이미지 변경/실행 로직은 같은 `Image` API를 통해 유지한다. Debug / Any CPU 별도 출력 폴더 빌드에 성공했다.
- 2026-06-07: 프로젝트 전체에서 Cyotek `ImageBox` 의존성을 제거했다. `FormVision_Arithmetic`과 `FormMeasure`도 `VisionTestImageCanvas`로 전환했고, `CViewer`는 ImageBox 호스팅 컴포넌트가 아니라 ROI 상태/DisplayManager 연결만 남긴 경량 컴포넌트로 축소했다. `DrawObject`의 `ImageBox` 타입 인자와 불필요한 `using Cyotek.Windows.Forms`를 제거했으며, `OpenVisionLab.csproj`의 `Cyotek.Windows.Forms.ImageBox` 참조와 `dll/Cyotek.Windows.Forms.ImageBox.dll` 파일을 삭제했다. 소스/XAML/프로젝트 파일 기준 `rg "ImageBox"` 결과가 없고, Debug / Any CPU 별도 출력 폴더 빌드에 성공했다.
- 2026-06-07: Vision Test 폼(`FormVision_Contour` 등)을 띄운 상태에서 검사 후 뒤쪽 메인 레이어 뷰어를 드래그하면 텍스처 위치가 원래대로 돌아가던 문제를 수정했다. 원인은 타이머가 `ibSource/ibDestination.Image`를 주기적으로 재할당하고, `VisionTestImageCanvas.Image` setter가 그 표시 갱신까지 `ImageChanged`로 다시 메인 레이어에 써서 `FormLayerDisplay.SetImage()`/`ZoomToFit()`이 반복 호출되던 것이다. 이제 `Image` setter는 조용한 표시 갱신만 수행하고, 우클릭 이미지 로드처럼 사용자가 뷰어 안에서 이미지를 바꾼 경우에만 `ImageChanged`를 발생시킨다. 같은 레이어 Bitmap 참조가 반복 전달되면 텍스처 재업로드도 건너뛴다.
- 2026-06-07: Vision Test 폼을 닫은 뒤 메인 레이어의 결과 이미지가 하얗게 남는 문제를 완화했다. `VisionTestImageCanvas.Dispose()`에서 transient 검사 폼의 GL 텍스처를 명시 삭제하지 않도록 변경해, SharpGL 컨텍스트/texture id 공유 상황에서 메인 레이어 텍스처가 함께 삭제될 가능성을 제거했다. 또한 `FormLayerDisplay.SetImage()`는 외부 Result Bitmap 참조를 그대로 잡지 않고 24bpp 복사본을 소유하며, `DisplayImageSyncService`/`DisplayLayerPresenter`도 이 복사본을 `ImageSpace`에 저장하도록 정리했다. Debug / Any CPU 별도 출력 폴더 빌드에 성공했다.
- 2026-06-07: ImageCanvas 텍스처 수명 정책을 명시했다. `ImageCanvasControl.ClearTexture(bool deleteOpenGlTextures = true)`로 기본 호출은 기존처럼 OpenGL texture id까지 삭제하고, transient Vision Test 뷰어 dispose는 `ClearTexture(false)`를 호출해 컨트롤의 managed texture 상태만 비운다. 메인 레이어/이미지 비교처럼 자신이 실제 표시 소유자인 GL 뷰어는 계속 기본 `ClearTexture()`를 사용한다.
- 2026-06-07: Vision Test 검사 폼의 ImageBox 시절 타이머 잔재를 제거했다. `FormVision_Blob`, `Contour`, `FeatureMatching`, `EdgeDection`, `Histogram`, `Filter`, `Arithmetic`, `HSV`, `Line`, `Matching`, `Mean`, `Morphology`, `RotateAndScale`에서 `timer1` 생성/필드/이벤트/`timer1_Tick()`/constructor start 로직과 `.resx` tray metadata를 제거했다. 레이어 목록/이미지 갱신은 폼 로드, 콤보 선택 변경, 새 패널 버튼, 검사 실행 시점에만 일어난다. `VisionTestForm`의 unused `panelCount`, `BindLayerToViewer()`, `RefreshViewerRoi()` helper도 삭제했다. `FormVision_NewPanel`과 `FormVision_Result`의 타이머는 검사 뷰어 동기화용이 아니므로 유지했다.
- 2026-06-07: `DisplayLayerPresenter`와 `DisplayImageSyncService`가 `FormLayerDisplay.ibSource` 또는 `viewer._Ib`에 직접 접근하던 코드를 제거하고, `FormLayerDisplay`의 공개 메서드를 통해 이미지 갱신/Refresh/Fit/동기화를 수행하도록 변경했다. 이후 `FormMeasure`와 `0. UI/6) Vision Test` 계열까지 ImageCanvas 전환을 완료했다.
- 2026-06-07: `FormImageCompare` 종료 시 `ImageCanvasControl.ClearTexture()`가 문자열 texture key(`CompareImage1`)를 `uint.Parse()` 하면서 `FormatException`이 발생하던 문제를 수정했다. 이제 `_textureAreas`의 key가 아니라 `OpenGlTextureDrawingParam` 안의 실제 OpenGL texture id들을 수집해 삭제하고, 컨트롤이 dispose 중이면 내부 상태만 정리한다.
- 2026-06-07: `ImageCanvasControl.cs`와 당시 텍스처 파라미터 파일에 깨져 있던 한글 주석을 정상 한글 문장으로 다시 정리하고, Visual Studio에서 다시 깨져 보이지 않도록 두 파일을 UTF-8 BOM으로 저장했다.
- 2026-06-07: `FormImageCompare` 하단 상태바를 28px 높이의 compact dark bar로 정리하고, `Load Images` 버튼이 전체 폭을 차지하지 않도록 우측 소형 버튼으로 변경했다. 마우스가 한쪽 GL 뷰어의 이미지 위에 있을 때 같은 이미지 좌표를 양쪽 뷰어에 노란 십자/박스로 표시하고, 하단에는 LEFT/RIGHT 좌표와 GV를 동시에 표시한다. 메인 WinForms 프로젝트가 `SharpGL`을 직접 참조하지 않도록 실제 마커 렌더링은 `ImageCanvasControl.DrawImagePointMarker()`로 옮겼다.
- 2026-06-07: `FormImageEditView`에서 OpenGL ROI 뷰어 내부 툴바를 우선 사용하도록 상단 WinForms 레거시 툴 패널(`splitContainer2.Panel1`)을 접고 관련 버튼을 숨겼다.
- 2026-06-07: `FormImageEditView`의 레이아웃/코드비하인드에서 `ibSource`, `ibTrainImage`, `groupBox1`, `timer1`, `KtemViewer`, `ImageGrey`, `UseImageCanvas` fallback, 숨겨진 FontAwesome/RJButton 레거시 ROI 버튼을 제거했다. 이 폼은 이제 `ElementHost`의 `RoiImageCanvasView`와 `PatternMatchPreviewView`를 활성 경로로 사용한다.
- 2026-06-07: `FormImageCompare`를 Cyotek `ImageBox` 두 개 구조에서 `OpenVisionLab.ImageCanvas.Rendering.ImageCanvasControl` 두 개를 직접 호스팅하는 GL 비교 뷰어로 전환했다. 이미지 로드, 좌표/RGB/GV 표시, 더블클릭 Fit, 마우스 휠 줌, 좌우 뷰 상태 동기화를 유지하고 GDI 타이머/직접 그리기/FontAwesome 버튼 의존성을 제거했다.
- 2026-06-07: 메인 우상단 옵션 드롭다운에 `Tools > Image Compare` 메뉴를 추가했다. 메뉴 클릭 시 `FormImageCompare`를 non-modal 창으로 띄워 GL 비교 뷰어를 바로 테스트할 수 있다.
- 2026-06-07: `FormImageCompare`를 열자마자 `ImageCanvasControl.OnDraw()`에서 `Draw` 이벤트 구독자가 없어 `NullReferenceException`이 발생하던 문제를 수정했다. `ImageCanvasControl`은 `Draw?.Invoke(...)`로 안전하게 호출하고, 비교 뷰어는 각 GL 컨트롤에 `DrawContent()`를 구독해 기본 텍스처 렌더링을 수행한다.
- 2026-06-07: `ImageCanvasControl`에 비교 뷰어용 `CanvasViewState`, `CaptureViewState()`, `ApplyViewState()`, `ZoomAt()` API를 추가했다. 좌우 GL 뷰어의 `_zoom`/offset 상태를 안전하게 복제하기 위한 최소 공개 API다.
- 2026-06-07: `FormImageEditView` 오른쪽 상태 패널의 `Position/GV` 값 표시를 어두운 배경/밝은 글자색으로 바꾸고, `RoiImageCanvasView.xaml` 내부 하단 상태바(`RobotPos`, `GrayValue`, `PixelColor`)도 밝은 글자색 스타일로 정리했다.
- 2026-06-07: `RoiImageCanvasView.xaml` 내부 세로 툴바는 ROI 티칭과 측정 버튼만 남겼다. ROI 아이콘은 점선 사각 선택 아이콘, 측정 아이콘은 수평 눈금자 아이콘으로 교체했고 `RoiImageCanvasViewModel`에서 두 모드가 서로 배타적으로 토글되도록 정리했다.
- 2026-06-07: `FormImageEditView`의 원본 이미지 기준을 `SourceBitmap`/`ImageSource`로 분리했다. TRAIN preview와 mean/matrix 계산도 `ibSource.Image` 대신 원본 이미지 기준으로 동작하도록 1차 정리했고, 이후 레거시 ImageBox fallback은 제거했다.
- 2026-06-07: `FormImageEditView` 오른쪽 패널은 ROI/MULTI_ROI에서는 접고, TRAIN/패턴 템플릿 선택 모드에서만 표시하도록 정리했다. 중복 `Position/GV` 표시와 `PropertyGrid`는 런타임에서 숨기고, 패턴 preview는 새 WinForms 컨트롤 `PatternMatchPreviewView`가 담당하도록 분리했다.
- 2026-06-07: TRAIN/ROI 선택 확정 UI를 복구했다. 상단 레거시 툴 패널이 숨겨져도 `FormImageEditView` 하단에 `Use Pattern`/`OK`와 `Cancel` 버튼이 항상 표시되고, 기존 Enter/Escape 흐름과 같은 `DialogResult.OK/Cancel` 경로를 사용한다.
- 2026-06-07: ImageCanvas ROI crop 좌표 변환을 좌하단 OpenGL 좌표계에서 좌상단 Bitmap/OpenCV 좌표계로 보정했다. `FormImageEditView.ToImageRectangle()`은 `SourceBitmap.Height - rect.Top` 기준으로 crop Y를 계산하고, `RoiImageCanvasViewModel.AddInitialRoi()`도 기존 이미지 좌표 ROI를 OpenGL 좌표로 올릴 때 같은 기준으로 변환한다.
- 2026-06-07: 패턴 매칭 이미지가 이미 등록된 상태에서 TRAIN 편집창을 다시 열면 기존 패턴 파일이 우측 `Pattern Preview`에 표시되도록 했다. `MatchEditor`/`WpgMatchEditor`가 현재 저장된 패턴 경로를 `FormImageEditView.LoadPatternPreviewImage()`로 전달하며, 새 ROI를 선택하지 않고 OK를 누른 경우 기존 패턴 경로를 유지한다.
- 2026-06-07: 패턴 등록 시 ROI 위치도 함께 복원되도록 했다. 새 패턴 저장 시 `.bmp` 옆에 같은 이름의 `.roi` 메타 파일을 저장하고, 재편집 시 이 ROI를 TRAIN 창의 초기 ROI로 넘긴다. 기존처럼 `.roi`가 없는 패턴은 `Cv2.MatchTemplate()`로 원본 이미지에서 템플릿 위치를 찾아 ROI를 복원한다.
- 2026-06-07: 패턴 ROI 메타 파일 포맷을 보강했다. `.roi`에는 `Format`, `Version`, `X/Y/Width/Height`, `SourceWidth/SourceHeight`를 저장하며, 저장 ROI는 원본 Mat 경계로 클램프한다. `.roi`가 없는 기존 패턴의 fallback `MatchTemplate` 결과는 최소 점수(`0.5`) 이상일 때만 ROI로 사용한다.
- 2026-06-07: `FormImageEditView`의 활성 OpenGL 경로에서 `CViewer`/ImageBox 초기화 의존성을 제거했다. `LegacyViewer`, `ImageGrey`, `UseImageCanvas` 분기, `ibSource_MouseMove()` fallback도 함께 제거했다.
- 2026-06-07: ImageCanvas 좌표/GV 상태를 WinForms 쪽에서 받을 수 있도록 `FormImageEditView.ImageCanvasStatusChanged` 이벤트를 추가했다. `RoiImageCanvasViewModel.PropertyChanged`의 `RobotPos`, `GrayValue`, `PixelColor` 갱신을 받아 이벤트로 발행하며, 오른쪽 숨김 라벨은 내부 상태 동기화만 유지한다.
- 2026-06-07: `Editable` 명칭을 활성 ImageCanvas 소스에서 제거했다. `RoiImageCanvasView`, `RoiImageCanvasViewModel`, `RoiInteraction` 폴더와 `RoiInteractionMouseDown/Move/Up/KeyDown/Cursor` helper 이름을 사용한다.
- 2026-06-07: ROI 편집 핸들 렌더링 함수는 `OpenGlDrawing.DrawRoiEditHandles()`로 정리했다.
- 2026-06-07: `FormImageEditView`는 `RoiImageCanvasView`/`RoiImageCanvasViewModel`과 `RoiAdded`, `RoiMouseUp`, `RoiEditingCompleted`, `RoiChangedEventArgs.RoiRect` 기준으로 연결한다.
- 2026-06-07: `EnumViewMode`, `TKMouseEventArgs`, `FontBitmapEntry` 등 compatibility 잔여 타입도 `CanvasInteractionMode`, `CanvasMouseEventArgs`, `OpenGlFontBitmapEntry` 등으로 분리/변경했다.
- 2026-06-07: `Library/OpenVisionLab.ImageCanvas`와 `FormImageEditView.cs` 기준으로 `Editable`, `NX`, `TX`, `Diagram` 계열 이전 명칭 검색 결과가 없도록 정리했다.

## 0-1. ImageBox -> ImageCanvas 전환 인벤토리

2026-06-07 기준 `FormImageEditView`의 OpenGL 전환은 활성 ROI 편집/패턴 등록 경로에서 ImageBox/CViewer fallback을 제거하는 단계까지 진행했다.

현재 `FormImageEditView`에서 이미 ImageCanvas가 담당하는 항목:

- 이미지 표시 주 경로: `InitializeImageCanvas()`에서 `RoiImageCanvasView`를 `ElementHost`에 붙인다.
- 이미지 로드: `PendingImageCanvasImage`를 `Shown` 이후 `RoiImageCanvasViewModel.LoadImage(mat, "Source")`로 전달한다.
- 기존 ROI 로드: 생성자에서 받은 `Rectangle`/`List<Rect>`를 `PendingImageCanvasRois`에 저장하고 `AddInitialRoi()`로 OpenGL overlay에 추가한다.
- ROI 결과: `RoiAdded`, `RoiMouseUp`, `RoiEditingCompleted` 이벤트로 `ImageCanvasSelectedRegion`, `ImageCanvasSelectedRegions`를 갱신한다.
- 단일/다중 ROI 반환: `btnCut_Click()`에서 단일 ROI는 `ImageCanvasSelectedRegion`, 다중 ROI는 `ImageCanvasSelectedRegions`를 우선 사용한다.

`FormImageEditView`에서 제거 완료한 ImageBox/CViewer 항목:

- `KtemViewer.LoadImageBox(ibSource)`와 `Train.LoadImageBox(ibTrainImage)` fallback 호출을 제거했다.
- Designer에서 `ibSource`, `ibTrainImage`, `groupBox1`, `timer1`, 숨겨진 레거시 ROI 버튼들을 제거했다.
- `ibSource.Image`, `ImageGrey`, `ibSource_MouseMove()`, `PointToImage`, `ImageBoxSelectionMode`, `SelectionRegion` 기반 경로를 제거했다.
- `GetSelectedRoi()`/`GetAnalysisRoi()`는 ImageCanvas ROI 결과만 사용한다.
- TRAIN preview는 `PatternMatchPreviewView`가 담당한다.

다음 구현 우선순위:

1. 완료: `FormImageEditView`의 원본 이미지 저장소를 `ibSource.Image`에서 `SourceBitmap`/`ImageSource` 기준으로 분리했다.
2. 완료: `KtemViewer.LoadImageBox(ibSource)`와 `Train.LoadImageBox(ibTrainImage)` 호출을 제거했다.
3. 완료: ImageCanvas의 좌표/GV 상태를 WinForms 쪽으로 전달하는 `ImageCanvasStatusChanged` 이벤트를 추가했다.
4. 완료: Train preview는 ImageCanvas ROI 이벤트와 원본 이미지 crop으로 갱신하고, 표시 책임을 `PatternMatchPreviewView`로 분리했다.
5. 완료: 레거시 `SelectionRegion` 기반 버튼/타이머/상단 툴 버튼 로직을 제거했다.
6. 완료: Designer에서 `ibSource`/`ibTrainImage`/`groupBox1`을 제거했다.

## 1. 현재 작업 목표

OpenVisionLab 프로젝트를 기존 WinForms/GDI 중심 구조에서 점진적으로 정리하고, 이미지 표시/ROI 편집 영역을 `OpenVisionLab.ImageCanvas` 기반 OpenGL 뷰어로 전환하는 작업을 진행 중이다.

현재 가장 중요한 목표는 `FormImageEditView`에서 OpenGL 기반 `RoiImageCanvasView`를 안정적으로 사용하도록 만드는 것이다. 특히 ROI 생성, 기존 ROI 선택, 이동, 리사이즈, 단일 ROI 교체, 다중 ROI 누적 흐름이 깨지지 않아야 한다.

최근 작업의 초점은 다음이다.

- `FormImageEditView`에서 기존 ImageBox 대신 `ElementHost`로 WPF/OpenGL 이미지 캔버스를 붙임
- 기존 ROI가 있으면 뷰 로드시 OpenGL 다이어그램으로 표시
- ROI 클릭 시 기존 다이어그램을 찾아 선택/이동/리사이즈
- 빈 영역 클릭 시 기존 선택 해제 후 새 ROI 드로잉
- 단일 ROI 모드에서는 새 ROI 생성 시 기존 ROI 제거
- 다중 ROI 모드에서는 ROI 누적
- 삭제/교체 시 OpenGL display list 잔상 방지

## 2. 지금까지 결정한 구조

`Controls.Viewer2D` 계열 코드는 프로젝트 성격에 맞게 `OpenVisionLab.ImageCanvas`로 이름을 바꾸는 방향으로 진행했다.

현재 구조 판단은 다음과 같다.

- 이미지 표시/ROI 편집/OpenGL 렌더링은 `Library/OpenVisionLab.ImageCanvas` 프로젝트에서 관리한다.
- 이미지 데이터/공간 관리 성격은 `OpenVisionLab.ImageSpace.Core`로 분리하는 방향을 유지한다.
- Display 관련 공용 기능은 `OpenVisionLab.Display.Core`로 분리하는 방향을 유지한다.
- WinForms 화면에서는 WPF `RoiImageCanvasView`를 `ElementHost`로 호스팅한다.
- `FormImageEditView`는 기존 GDI ImageBox fallback을 제거하고 OpenGL `RoiImageCanvasView`를 사용한다.
- `DiagramControl`은 없는 상태로 개발해야 한다. 호환에 필요한 타입은 `OpenVisionLab.ImageCanvas` 내부 compatibility 코드로 처리한다.
- `CanvasImageLoader.LoadMatFromFile()` 쪽에서 EMGU를 사용하는 흐름이 있으므로 EMGU 관련 DLL/참조는 제거하면 안 된다.
- `FormTeachingVision`의 하단 컨트롤 문제는 `FormTeachingVision` 내부 패널/디자이너 구조로 해결하는 방향을 택했다. 코드비하인드 위치 보정보다 `InitializeComponent`/Designer 구조 우선이다.
- `FormMetroFrame`은 `TeachingVision`과 `pnStatusBar`가 서로 겹치지 않도록 메인 영역과 상태바를 분리하는 방향으로 정리했다.
- System/Property dock 창은 메인에서 일단 보이지 않도록 처리하는 방향으로 결정했다.

## 3. 수정한 파일 목록

현재 스레드에서 주요하게 수정하거나 생성한 파일은 다음과 같다. PC2에서는 GitHub Desktop의 Changes 탭에서 실제 diff를 한 번 더 확인하는 것이 좋다.

- `OpenVisionLab.sln`
- `OpenVisionLab.csproj`
- `0. UI/0) MENU/FormMetroFrame.cs`
- `0. UI/0) MENU/FormMetroFrame.Designer.cs`
- `0. UI/0) MENU/FormMetroFrame.resx`
- `0. UI/0) MENU/FormTeachingVision.cs`
- `0. UI/0) MENU/FormTeachingVision.Designer.cs`
- `0. UI/2) POPUP/FormImageEditView.cs`
- `0. UI/2) POPUP/FormImageEditView.Designer.cs`
- `0. UI/2) POPUP/FormImageCompare.cs`
- `0. UI/2) POPUP/FormImageCompare.Designer.cs`
- `0. UI/2) POPUP/FormMessageBox.cs`
- `0. UI/2) POPUP/FormMessageBox.Designer.cs`
- `Library/OpenVisionLab.ImageCanvas/OpenVisionLab.ImageCanvas.csproj`
- `Library/OpenVisionLab.ImageCanvas/View/RoiImageCanvasView.xaml`
- `Library/OpenVisionLab.ImageCanvas/View/RoiImageCanvasView.xaml.cs`
- `Library/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs`
- `Library/OpenVisionLab.ImageCanvas/RoiInteraction/RoiInteractionMouseDown.cs`
- `Library/OpenVisionLab.ImageCanvas/RoiInteraction/RoiInteractionMouseMove.cs`
- `Library/OpenVisionLab.ImageCanvas/RoiInteraction/RoiInteractionMouseUp.cs`
- `Library/OpenVisionLab.ImageCanvas/Compatibility/MvvmCompatibility.cs`
- `Library/OpenVisionLab.ImageCanvas/OpenGL/OpenGlDrawing.cs`
- `Library/OpenVisionLab.ImageCanvas/OpenGL/OpenGlRenderer.cs`
- `Library/OpenVisionLab.ImageCanvas/OpenGL/OpenGlTextureDrawingParam.cs`
- `Library/OpenVisionLab.ImageCanvas/Canvas/CanvasViewState.cs`
- `Library/OpenVisionLab.ImageCanvas/Engine/ImageCanvasControl.cs`
- `Library/OpenVisionLab.ImageSpace.Core/*`
- `Library/OpenVisionLab.Display.Core/*`
- `docs/CODEX_HANDOFF.md`

## 4. 핵심 코드 변경 내용

`FormImageEditView` 관련:

- `RoiImageCanvasViewModel`과 `RoiImageCanvasView`를 생성해서 `ElementHost`에 붙였다.
- 이미지 로드는 `PendingImageCanvasImage`로 보관했다가 `Shown` 이후 `LoadImage(mat, "Source")`로 OpenGL 텍스처에 올린다.
- 생성자에서 전달받은 `Rectangle ROI` 또는 `List<Rect>`는 `PendingImageCanvasRois`에 저장 후 이미지 로드 뒤 `AddInitialRoi()`로 OpenGL 다이어그램에 추가한다.
- `DiagramAdded`, `DiagramMouseUp`, `DiagramEditingCompleted` 이벤트를 받아 `ImageCanvasSelectedRegion`, `ImageCanvasSelectedRegions`를 갱신한다.
- `ROI`, `TRAIN`, `MULTI_ROI` 모드에서 ImageCanvas 드로잉 모드를 사용한다.
- `MULTI_ROI`가 아닌 단일 ROI 모드에서는 `ReplaceExistingRoiOnDraw = true`로 설정한다.
- Group 이름, ROI 번호, Group bounds는 `FormImageEditView`에서는 숨기도록 했다.

ROI 편집 관련:

- 기존 `_activeRoiRect` 하나가 선택된 실제 ROI와 새로 그리는 임시 ROI 역할을 동시에 하던 구조를 분리했다.
- `RoiImageCanvasViewModel`에 `_selectedRect`와 `_drawingRect`를 둔다.
- 기존 ROI를 클릭하면 `_selectedRect`로 잡고 `Edit` 또는 `Move` 모드로 처리한다.
- 빈 영역을 클릭하면 기존 선택을 해제하고 `_drawingRect`를 새 드로잉 임시 객체로 사용한다.
- MouseMove에서 `Drawing`은 `_drawingRect`, `Edit/Move`는 `_selectedRect`를 조작한다.
- MouseUp에서 새 ROI가 생성되면 `_drawingRect`를 다이어그램에 추가하고 `_selectedRect`로 승격한 뒤 `_drawingRect`를 초기화한다.
- ROI edge/handle hit-test는 실제 ROI 변 범위 안에서만 잡히도록 수정했다. 예전에는 같은 X/Y 라인에 있으면 멀리 떨어진 곳도 ROI 변으로 오판할 수 있었다.

OpenGL display list 관련:

- 다이어그램 삭제 시 `DisplayListId`를 `gl.DeleteLists()`로 해제한다.
- `ExtendedRectangle`이 null일 때 NullReference가 나지 않도록 명시적 null 체크로 수정했다.
- `ClearDiagram()` 호출 시에도 전체 다이어그램 display list를 먼저 해제하도록 했다.
- `DiagramManager.RemoveDiagramByUniqueId()`가 삭제 성공 후에도 `false`를 반환하던 문제를 `true` 반환으로 수정했다.

UI/프레임 관련:

- `FormMetroFrame`의 창 크기/위치 문제, 더블클릭 축소/복원, 전체 크기 버튼, 상단 버튼 정렬 문제를 정리했다.
- 다중 모니터 환경에서는 더블클릭 복원 시 현재 폼이 위치한 화면 기준으로 커지도록 수정하는 방향을 택했다.
- `FormTeachingVision`에서 하단 메뉴/컨트롤이 `pnStatusBar`에 가려지던 문제를 패널 분리 방향으로 정리했다.
- Teaching 하단 컨트롤 일부는 상단으로 이동했고, `Image Processing`, `Algorithm`, `Camera`, `Main`, `Panel Source Image`, `Tack Time` 흐름을 유지해야 한다.
- `FormInit`는 기본 윈도우 폼 테두리가 보이지 않도록 UI를 현재 앱 스타일에 맞추는 방향으로 수정했다.
- MessageBox는 앱 중앙에 뜨도록 조정하는 방향으로 수정했다.

리팩토링 관련:

- 오래된 `CThreadStatus`는 제거하고 새로운 런타임/상태 관리 방식으로 가는 방향으로 결정했다.
- Matrox, Encoder 등 현재 VisionTestTool 목적과 맞지 않는 과거 장비 잔재는 제거하는 방향으로 정리했다.
- `MetroModernUI`는 제거 방향, `ImageListView`는 사용처가 없으면 제거 방향으로 결정했다.
- `log4net`은 업데이트 후보로 남겨도 된다.

## 5. 아직 남은 작업

PC2에서 우선 확인할 작업:

- `FormImageEditView`에서 실제로 ROI 신규 생성, 기존 ROI 선택, 꼭지점 리사이즈, 내부 드래그 이동이 모두 정상인지 확인한다.
- 단일 ROI 모드에서 새 ROI를 그리면 기존 ROI가 화면/내부 선택값에서 모두 교체되는지 확인한다.
- `MULTI_ROI` 모드에서 ROI가 누적되고 `SelectedRegions`가 정상 갱신되는지 확인한다.
- 기존 ROI를 클릭하지 못한 빈 영역에서 새 ROI를 그릴 때 이전 ROI 리사이즈 상태로 남지 않는지 확인한다.
- OpenGL display list 잔상이 더 남는지 확인한다.
- ROI 삭제 키(`Delete`) 흐름이 실제 UI 요구에 맞는지 확인한다.
- `FormTeachingVision` 상단 컨트롤, 상태바, 버전 표시, Tack Time 표시가 화면 해상도별로 잘 보이는지 확인한다.
- `FormMetroFrame` 다중 모니터 환경에서 실행 위치/더블클릭 최대화/복원 동작을 다시 확인한다.
- GitHub Desktop 기준 변경 파일이 너무 넓으면 커밋 단위를 나눌지 검토한다.

다음 리팩토링 후보:

- PC2에서 Vision Test 각 검사 폼(`Blob`, `Contour`, `Filter`, `Histogram`, `Arithmetic` 등)을 실제로 열어 레이어 콤보 선택, 새 패널 생성, 실행 결과 표시, 메인 레이어 동기화가 정상인지 확인한다.
- `VisionTestImageCanvas`의 호환 alias(`Image`, `Bitmap`, `ImageChanged`)는 모든 호출부가 `DisplayImage`, `DisplayBitmap`, `UserImageChanged`로 안정화된 뒤 제거할지 결정한다.
- `FormVision_*` 디자이너 파일에 남은 큰 고정 좌표/절대 배치를 정리해 Vision Test 폼 UI를 더 compact하게 만든다.
- 현재 비활성/레거시로 보이는 `Library/Controls.Viewer2D`의 FontAwesome 참조와 이전 뷰어 코드가 실제 프로젝트 참조에서 완전히 제외돼 있는지 별도 범위로 확인한다.

## 6. 주의해야 할 점

- EMGU 관련 참조/DLL은 제거하지 말 것. `CanvasImageLoader.LoadMatFromFile()` 흐름에서 필요하다.
- `DiagramControl`은 현재 없으므로 추가 전제로 작업하지 말 것.
- `Library/Controls.Viewer2D` 옛 폴더는 삭제가 완전히 끝난 상태가 아닐 수 있다. 현재 실사용 프로젝트는 `Library/OpenVisionLab.ImageCanvas`다.
- `ToolKitImageCanvasViewModel`은 삭제됐다. ROI 편집 경로는 `RoiImageCanvasViewModel` 기준으로만 확인할 것.
- `FormTeachingVision` 레이아웃은 코드비하인드에서 위치를 억지로 보정하는 방식보다 Designer/InitializeComponent 구조를 우선해야 한다.
- `FormMetroFrame`의 `pnStatusBar`가 Teaching 화면을 덮지 않도록 메인 영역과 상태바 영역을 분리해야 한다.
- 다중 모니터 이슈는 `Screen.FromControl(this)` 또는 현재 폼 위치 기준 화면을 사용해야 한다. 최초 실행 위치만 기준으로 삼으면 안 된다.
- OpenGL 다이어그램은 객체를 한 번 그려 display list로 관리하는 구조다. 삭제/교체 시 display list 해제를 빼먹으면 잔상이 남을 수 있다.
- ROI hit-test는 꼭지점/변/내부 순서가 중요하다. 변 hit-test는 반드시 실제 변 범위 안인지 검사해야 한다.
- `FormImageEditView`의 ImageCanvas ROI 이벤트는 OpenGL 좌하단 좌표를 반환한다. Bitmap/OpenCV crop 또는 `SelectedRegion`에 넣을 때는 `SourceBitmap.Height - rect.Top`으로 좌상단 이미지 좌표계에 맞춰야 한다. 반대로 기존 이미지 좌표 ROI를 `AddInitialRoi()`로 캔버스에 올릴 때는 `canvasTop = imageHeight - roi.Top`, `canvasBottom = imageHeight - roi.Bottom` 기준을 유지해야 한다.
- 패턴 매칭 편집 값은 ROI가 아니라 저장된 템플릿 이미지 파일 경로 문자열이다. 재편집 시 현재 문자열 경로를 preview에 로드해야 하며, ROI를 새로 선택하지 않은 OK는 기존 문자열 값을 유지해야 한다.
- 패턴 매칭 ROI 위치는 `PropertyGridImageEditorService.SaveTemplateImage()`가 템플릿 `.bmp`와 함께 저장하는 `.roi` 메타 파일을 우선 사용한다. 메타 파일이 없는 기존 패턴은 `LoadTemplateRoi()`에서 템플릿 매칭으로 위치를 추정하므로, 원본 이미지가 바뀌었거나 같은 패턴이 여러 군데 있으면 첫 번째 최고 점수 위치로 복원될 수 있다. 현재 fallback은 `CCoeffNormed` 최고 점수가 `0.5` 미만이면 ROI 복원을 포기한다.
- `FormImageEditView`에서 ImageCanvas의 좌표/GV가 필요하면 오른쪽 `Position/GV` 라벨을 다시 표시하지 말고 `ImageCanvasStatusChanged` 이벤트를 구독하는 쪽으로 연결한다.
- `OpenVisionLab.sln` 빌드는 샌드박스에서 Windows SDK 경로 접근 권한 문제로 실패할 수 있다. Codex에서는 MSBuild 실행 시 권한 상승이 필요할 수 있다.
- 현재 코드는 Debug / Any CPU 기준 별도 출력 폴더(`bin\CodexVerify`) 빌드에 성공했다. 기본 `bin\Debug` 빌드는 실행 중인 Visual Studio/OpenVisionLab 프로세스가 `OpenVisionLab.ImageCanvas.dll/pdb`를 잠그면 복사 단계에서 실패할 수 있다. 이번 검증 빌드에서는 CA1416 Windows 전용 API 경고가 출력되지 않았다.

빌드 명령:

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "OpenVisionLab.sln" /t:Build /p:Configuration=Debug /p:Platform="Any CPU" /p:RestorePackages=false /p:OutDir="C:\Git\OpenVisionLab_Dev\bin\CodexVerify\" /v:minimal
```

## 7. PC2에서 이어서 작업할 때 Codex에게 줄 시작 프롬프트

```text
이 저장소는 C:\Git\OpenVisionLab 입니다.

먼저 docs/CODEX_HANDOFF.md를 읽고, 현재 진행 중인 OpenVisionLab.ImageCanvas / FormImageEditView ROI 편집 리팩토링 맥락을 파악해주세요.

현재 목표는 FormImageEditView에서 OpenGL 기반 RoiImageCanvasView를 안정화하는 것입니다.

특히 아래 동작을 실제 코드 기준으로 점검해주세요.

1. 기존 ROI가 있는 상태에서 뷰 로드시 ROI가 표시되는지
2. 기존 ROI 내부를 클릭하면 이동되는지
3. 꼭지점/변을 클릭하면 리사이즈되는지
4. ROI가 없는 빈 영역을 클릭하면 기존 선택이 해제되고 새 ROI 드로잉으로 들어가는지
5. 단일 ROI 모드에서는 새 ROI 생성 시 기존 ROI가 제거되는지
6. MULTI_ROI 모드에서는 ROI가 누적되는지
7. 삭제/교체 후 OpenGL display list 잔상이 남지 않는지

주의사항:
- EMGU 관련 DLL/참조는 제거하지 마세요.
- DiagramControl은 없습니다. 추가 전제로 작업하지 마세요.
- ToolKit 뷰/뷰모델은 삭제됐습니다. ROI 편집은 RoiImageCanvasViewModel 기준으로 확인하세요.
- FormTeachingVision/FormMetroFrame 레이아웃은 코드비하인드 위치 보정보다 Designer 구조를 우선해서 봐주세요.

수정 전에는 관련 파일을 먼저 읽고, 필요한 변경만 좁게 적용한 뒤 OpenVisionLab.sln Debug / Any CPU 빌드까지 확인해주세요.
```
