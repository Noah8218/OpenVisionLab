# CODEX HANDOFF

## 0. 최신 명명 정리 기록

- 2026-06-09: 로깅 DLL/로그 뷰의 옛 냄새가 나는 타입/메서드/파일명을 OpenVisionLab 기준으로 정리했다. `ConfigureRollingFileAppender`/`ConfigureCleanUpLogFile`/`GetLogDirectoryPath`는 `ApplyFilePolicy`/`ApplyRetentionPolicy`/`GetLogDirectory`로 바꿨고, `LiveLogAppender`/`LiveLogBufferReader`는 `RuntimeLogSink`/`RuntimeLogStream`, `LogEntry`는 `LogLine`, `LogViewerViewModel`은 `LogPanelViewModel`, `LogControlView`는 `LogPanelView`, `LogLevelToBrushConverter`는 `LogToneBrushConverter`, `ObservableObject`/`RelayCommand`는 `BindableObject`/`UiCommand`로 바꿨다. 파일명과 `OpenVisionLab.Logging.csproj` 명시 Compile 항목도 새 이름에 맞췄다. 소스 기준 옛 이름 검색 결과는 없고, `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했다.
- 2026-06-09: 도킹 로그 뷰가 기존 회사 로그 콘솔 UI와 너무 유사해 보여, 배치를 OpenVisionLab 전용 compact event list로 다시 바꿨다. 상단은 검색 입력, `Follow`, `Reset`, `Logs`, 그리고 `Stream/Area/Signal` 필터를 모두 포함하며 하단 컨트롤 영역은 제거했다. 콤보의 전체 선택 표시도 `All` 대신 `Any`를 사용한다. 로그 영역은 더 이상 검은 콘솔에 원문 한 줄을 그대로 뿌리지 않고, 각 항목을 `시간`, `레벨/타입`, `Source`, `Message`로 나눈 행 형태로 표시한다. 파일 로그의 `[시간] [Info]...` 형식도 파싱되도록 `LogLine` 정규식을 보정했고, `DisplayTime`으로 좌측 시간 칸은 `HH:mm:ss.fff`만 표시한다. `LogToneBrushConverter` 컬러는 왼쪽 accent bar와 메시지/레벨 색상에 유지한다. `LogPanelViewModel`에는 `SearchText` 필터를 추가했고, 초기 파일 로드는 현재 프로세스 시작 이후 로그만 읽도록 제한해 과거 실행 기록이 반복 노출되지 않게 했다. `OpenVisionLab.sln` Debug / Any CPU 빌드와 파서 스모크 테스트에 성공했다.
- 2026-06-09: 도킹 로그 뷰를 사용자가 제공했던 기존 로그 콘솔 형태에 가깝게 재수정했다. GridView 테이블/헤더가 아니라 검은 로그 영역에 `RawText` 라인을 표시하고, `LogLevelToBrushConverter`로 `Grab/Vision/Inspect/Network/Error` 계열 컬러를 유지한다. 상단 컨트롤은 스크롤이 생기지 않도록 `WrapPanel` 기반의 `All`, `Type`, `Level`, `Clear`로 정리했고, 하단에는 `Auto Scroll`, `Open LogFolder`만 둔다. 뷰가 열린 뒤 발생하는 live 로그뿐 아니라 열릴 때 최신 `*ALL.log` 파일을 `FileShare.ReadWrite`로 읽어 초기 로그를 채우도록 했다. `LiveLogAppender`는 `_log` 이름만 받던 제한을 제거해 카테고리별 logger도 받으며, `CLog`의 all/category 이중 기록은 짧은 시간 중복 제거로 걸러낸다. `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했다.
- 2026-06-09: 도킹 로그 뷰 UI를 좌측 레일/상단 통계/하단 Selected Log 상세 영역이 없는 compact 구조로 바꿨다. `Show all`, `Category`, `Level`, `Auto scroll`, `Clear`, `Open Folder`는 상단 한 줄 툴바로 올렸고, 남은 영역은 로그 테이블이 모두 사용한다. `LogViewerViewModel`에서도 더 이상 쓰지 않는 `SelectedLog`, `TotalLogsCount`, `FilteredLogsCount`, `LastLogLevel`, `LastLogTimestamp` 상태를 제거했다. `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했다.
- 2026-06-09: 로그 뷰를 팝업 창이 아니라 Threshold와 같은 좌측 `DockLeftAutoHide` 도킹 탭으로 붙였다. `FormLogViewer`는 `DockContent`로 전환했고, `FormTeachingVision`이 `VISION_DOCK_FORM.LOG`를 생성해 Threshold 옆 auto-hide 탭으로 등록한다. 메인 메뉴 `Tools > Log Viewer`는 새 창을 띄우지 않고 도킹된 로그 탭을 활성화한다. `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했다.
- 2026-06-09: 로그가 실제로 남지 않을 수 있는 상태를 보정했다. `SystemState`에 XML 직렬화되는 `LogConfig`를 추가했고, 기본값은 로그 저장 경로 `AppDomain.CurrentDomain.BaseDirectory\Log`, 파일 최대 크기 `100MB`, 최대 백업 파일 개수 `10000`, 보관 기간 `180일`이다. `Program.Main()`에서 앱 시작 직후 `Global.System.ApplyLogConfig()`를 호출하고, 시작/종료/전역 예외 정도만 `CLog.Write(...)`로 남긴다. 업무 함수 진입/성공/성능 측정용 의례성 로그와 로그만 남기는 try-catch 껍데기는 계속 제거된 상태다. 메인 프로젝트는 `OpenVisionLab.Logging`을 다시 직접 참조한다. `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했고, 임시 콘솔 스모크 테스트에서 `Main.log`와 `Config.log` 파일 생성 및 메시지 기록을 확인했다.
- 2026-06-09: 함수 진입/성공/성능 측정용으로 남기던 의례성 로그를 제거했다. 메인/UI/공통/비전툴/RJControls 쪽 활성 코드 기준 업무성 `CLog.Write(...)` 호출은 제거됐고, 로그 뷰/로깅 DLL 내부 타입만 남도록 정리했다. `catch { CLog.Write(...); }`, `catch { CLog.Write(...); return false; }`처럼 예외를 처리하지 않고 로그만 남기던 try-catch 껍데기를 제거했다. 상태 복구가 있는 catch는 복구 코드만 남기고 로그 출력은 제거했다. `Lib.Common`, `Lib.OpenCV`, `Lib.OpenCV.Blob`의 직접 `OpenVisionLab.Logging` 참조도 제거했다. `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했다.
- 2026-06-09: 로그 호출부가 너무 길어지는 문제를 줄이기 위해 `OpenVisionLogger`를 `CLog`로 축약했다. 파일은 `Library/OpenVisionLab.Logging/CLog.cs`, 공개 진입점은 `CLog.Write(...)` 기준이며, 메인/UI/공통/Lib.OpenCV/RJControls 호출부도 모두 `CLog.Write(...)`로 치환했다. `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했다.
- 2026-06-09: 기존 메인단/공통/비전툴 로그 호출을 `AppLog.*`에서 `OpenVisionLab.Logging` DLL 직접 호출로 전환했다. 호환 어댑터였던 `Library/Lib.Common/AppLog.cs`를 삭제했고, `Lib.Common`, `Lib.OpenCV`, `Lib.OpenCV.Blob`, 메인 프로젝트가 `OpenVisionLab.Logging` 프로젝트를 직접 참조한다. 메인 프로젝트의 `log4net.config` 직접 Content 복사 항목은 제거하고, `OpenVisionLab.Logging` 프로젝트의 content copy 전파에 맡겼다. 활성 소스 기준 `AppLog` 호출/클래스와 구 `Log4netView`/`CustomMemoryAppender` 잔재는 없다. `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했다.
- 2026-06-08: `LogControlView` 뷰 파일은 유지한 채 UI 내부만 변경하는 기준으로 다시 정리했다. `OpenVisionLab.Logging.Controls`의 이전 XAML generated 캐시를 제거하고 restore/build로 `LogControlView.g.cs`만 재생성되는 것을 확인했다. 로그 뷰를 여러 번 열 때 root appender가 누적되지 않도록 `LiveLogBufferReader`에 `IDisposable`을 구현하고, `LogViewerViewModel.Dispose()`에서 appender를 제거하도록 보강했다. `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했다.
- 2026-06-08: 외부 로그 DLL 통합분의 이름을 프로젝트 네이밍에 맞춰 `OpenVisionLab.Logging`과 `OpenVisionLab.Logging.Controls`로 변경했다. 코어 진입점은 `CLog`, 실시간 UI 버퍼는 `LiveLogAppender`/`LiveLogBufferReader`, WinForms 팝업은 `FormLogViewer` 기준으로 정리했다. WPF 뷰 자체는 `LogControlView`로 유지해 기존 뷰를 없애지 않고 내부 UI만 수정하는 방향으로 맞췄다. 이전 이름의 프로젝트 폴더 캐시와 Debug 출력 DLL/PDB 산출물도 제거했고, `OpenVisionLab.sln`에는 새 프로젝트명으로 다시 등록했다.
- 2026-06-08: 로그 뷰 UI를 기존 DLL 제공 화면 형태에서 OpenVisionLab 운영용 로그 콘솔 형태로 재구성했다. `LogEntry` 모델을 추가해 로그 문자열을 시간/분류/레벨/호출 위치/메시지로 파싱하고, 화면은 좌측 필터 레일, 상단 Total/Visible/Last Level/Last Time 집계, 중앙 GridView 로그 테이블, 하단 선택 로그 상세 영역으로 변경했다. 메인 앱의 `Tools > Log Viewer` 메뉴는 `ElementHost`로 `OpenVisionLab.Logging.Controls.View.LogControlView`를 표시한다.
- 2026-06-08: 루트 이미지 자산을 정리했다. 실제 실행 코드에서 `Properties.Resources` 이미지 리소스를 사용하지 않고, 남은 참조가 주석/프로젝트 리소스 등록뿐임을 확인한 뒤 `Image`, `Images`, `Resources` 폴더를 삭제했다. 함께 `Properties/Resources.resx`, `Properties/Resources.Designer.cs`와 `OpenVisionLab.csproj`의 삭제된 `Images` 개별 Include 항목도 제거했다. `RJBaseForm`/`RJImageColorOverlay`에 남아 있던 이미지 리소스 주석과 빈 이벤트 핸들러 껍데기도 정리했다. 현재 소스 트리의 이미지 파일은 애플리케이션 아이콘 `lens.ico`만 남는다. `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했다.
- 2026-06-08: 한글 주석이 Visual Studio에서 깨져 보이는 문제를 보정했다. `GlobalState.cs`의 깨진 한글 주석을 정상 문장으로 복구했고, `GlobalState.cs`와 `SystemState.cs`를 UTF-8 BOM으로 저장했다. 재발 방지를 위해 루트 `.editorconfig`를 추가해 C# 파일의 `charset = utf-8-bom`, `end_of_line = crlf`를 명시했다. 인코딩 복구 후 `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했다.
- 2026-06-08: `SystemState`를 추가 정리했다. 사용처가 없는 `PauseWait`, mode/ui/notice/style 이벤트, `ResultState`/`Result`, `PROCDURE`, `StyleIndex`, `IF_Handle`, 빈 `Close()` 경로를 제거했고, `FormMainFrame`의 `IF_Handle` 대입도 삭제했다. `SystemState` 내부는 `Events`/`Enums`/`Fields`/`Properties`/`Initialization`/`Config` region으로 재배치했으며, 실제 인증 구현 없이 남아 있던 `License`/`USE_LICENSE`/`NO_LICENSE` 흐름과 `SYSTEM.xml`의 `License` 저장 항목도 제거했다. 더 이상 참조되지 않는 `AccountManager` 클래스와 XML 검증 항목도 삭제했다. `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했고, XML 검증은 12개 XML root와 실제 레시피 XML 9개 파일 기준 통과했다.
- 2026-06-08: VisionTool 테스트/리팩토링 범위에서 불필요한 장비식 저장 경로 헬퍼를 제거했다. `AppCommon.SaveLotIDPath()`, `GetPathOK()`, `GetPathOK_Ori()`, `GetPath_Crop()`, `GetPath_Screen()`, `GetPathOK_Insp()`, `GetPathNG()`, `GetPathNG_Ori()`, `GetPathNG_Insp()`와 내부 `GetImagePath()`를 삭제했고, 호출부가 없는 `AppPathService.GetDatedDirectory()`/`EnsureTrailingSeparator()`도 제거했다. `AppPathService`는 현재 `CAPTURE`, `RECIPE`, `TEST` 및 파일 경로 조합 역할만 남는다. `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했고, XML 검증은 13개 XML root와 실제 레시피 XML 9개 파일 기준 통과했다.
- 2026-06-08: 안정화 리팩토링 후속을 진행했다. `AppPathService`를 추가해 앱 기준 경로, `CAPTURE`, `TEST`, `RECIPE`, 날짜별 `LOT`/`Image` 저장 경로 생성을 공통화했고, `AppCommon`의 날짜별 이미지/LOT 경로 생성 중복과 `FormMainFrame.Actions`, `RecipeModel`, Vision Test의 `TEST` XML 경로 조립을 해당 서비스로 전환했다. `VisionToolRepository`의 주요 컬렉션은 public field에서 get-only property로 바꿔 외부 교체를 막고, `PropertyVision`은 internal setter로 제한했다. `SystemState`의 public event field는 C# `event`로 전환했고, `PauseWait`는 get-only property로 정리했다. `RecipeXmlCompatibilityCheck`의 실제 레시피 XML 스캔은 파일명 패턴(`Blob_*.xml`, `Line(...).xml`, `VisionPara.xml`, `VISION.xml` 등)까지 반영해 후보 타입을 더 엄격히 고른다. `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했고, XML 검증은 13개 XML root와 실제 레시피 XML 9개 파일 기준 통과했다. 남은 직접 `Application.StartupPath`는 `AppPathService` 자체와 일부 UI 파일 다이얼로그/라이브러리 유틸 경로 쪽이다.
- 2026-06-08: 레시피/비전툴 후속 리팩토링 1~5번을 진행했다. `VisionToolStorage`를 추가해 `VisionToolRepository`가 직접 갖고 있던 비전 property 생성/로드/저장 루프를 분리했고, 기존 public 리스트 표면은 유지해 호출부 영향을 줄였다. `RecipeRuntimeStorage`를 추가해 `RecipeState`의 데이터/비전툴 로드 저장 책임을 분리했으며, 오타였던 `EventChagedRecipe`는 호환 alias로 남기고 새 `EventChangedRecipe`를 사용하도록 했다. `SystemState`는 생성자 내부 디렉터리 생성/설정 로드를 `Initialize()`와 `EnsureRuntimeDirectories()`로 분리했고, 시스템 XML 경로는 `RecipeWorkspaceService.GetSystemConfigPath()`를 사용한다. `RecipeXmlCompatibilityCheck`는 기본 sample 검증 외에 실제 `RECIPE` 폴더 XML을 root명 기준으로 역직렬화하는 스캔 모드를 추가했다. `ParameterProperty.UseUpToBottom`, `LineGaugeProperty.PRJ_POLARITY` alias를 추가해 기존 XML/인터페이스 오타명은 유지하면서 새 코드에서 쓸 이름을 마련했다. `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했고, XML 검증은 13개 XML root와 `bin\Debug\RECIPE`의 실제 XML 9개 파일 기준 통과했다.
- 2026-06-08: 후속 리팩토링으로 설정 경로/직렬화 검증을 한 단계 더 정리했다. `RecipeWorkspaceService` 내부 경로 반환은 `Path.Combine` 기반으로 맞췄고, `ParameterProperty`의 `ROIs`, `SpecAreas`, `SpecDistance` public field를 동일 XML 이름의 public property로 변경했다. OpenCV 설정 property 계열에는 `IOpenCvConfigurableProperty<T>` 계약을 추가해 `LoadConfig()`/`LoadTestConfig()` 지원 타입을 명시했다. `RecipeXmlCompatibilityCheck`는 sample XML 역직렬화와 주요 필드값 검증에 더해, 역직렬화된 객체를 임시 XML 파일로 저장한 뒤 다시 로드해 같은 값이 유지되는지 확인한다. `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했고, 저장-재로드까지 포함한 XML 호환성 검증도 13개 XML root 기준 통과했다.
- 2026-06-08: XML/설정 경로 리팩토링 1~5번을 진행했다. `RecipeWorkspaceService`에 `GetRecipeFilePath()`, `GetVisionConfigPath()`, `GetAccountConfigPath()`를 추가해 레시피/비전/계정 XML 경로 생성을 공통화했고, `AccountManager`, `ImageViewProperty`, `SpecProperty`, `VisionProperty`, `OpenCvPropertyBase`의 직접 `Application.StartupPath` 문자열 조립을 제거했다. `ParameterPropertyStorage`를 추가해 `ParameterProperty`가 직접 맡던 자기 XML 및 하위 OpenCV property 저장/로드 책임을 분리했다. `OpenCvPropertyBase`에는 로드 후처리 콜백 overload를 추가해 `MatchingProperty`/`FeatureMatchingProperty`의 템플릿 이미지 재로드 흐름을 공통 로드 경로에 붙였다. `RecipeXmlCompatibilityCheck`는 역직렬화 성공뿐 아니라 주요 필드값(`BlobProperty.MIN_AREA`, `VisionProperty.Alpha/Beta`, `AccountManager.Accounts.Count`, `ImageViewProperty.ROWS/COLUMNS` 등)까지 검증하도록 강화했다. 일부 XML 전환 후 남은 using도 정리했다. `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했고, 강화된 `RecipeXmlCompatibilityCheck`도 13개 XML root 기준 통과했다.
- 2026-06-08: XML 직렬화 전환 후속 정리를 진행했다. 호출부가 사라진 `SerializeHelper.ToString<T>()`, `ToByte<T>()`, `Deserialize<T>()`와 관련 `XmlTextReader/XmlTextWriter` 기반 레거시 코드를 제거해 `SerializeHelper`를 XML 파일 로드/저장 전용 헬퍼로 축소했다. `OpenCvPropertyBase`는 VISION 설정 XML 경로 생성 중복을 `GetConfigPath()`로 분리했다. `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했고, `RecipeXmlCompatibilityCheck`는 현재 대상 기준 13개 XML root 역직렬화 검증을 통과했다.
- 2026-06-08: XML 로드/저장 후속 리팩토링 1~5번을 이어서 진행했다. `SerializeHelper`에 `TryLoadFromXmlFile<T>()`, `LoadOrCreateXmlFile<T>()`, `SaveXmlFile<T>()`를 추가해 공통 `XmlSerializer` 기반 로드/저장 경로를 마련했고, 파일이 없을 때만 기본 XML을 생성하며 기존 파일 역직렬화 실패 시에는 덮어쓰지 않도록 했다. 현재 소스에 남은 `ImageViewProperty`, `ParameterProperty`, `SpecProperty`, `RecipeDataStorage`, `AccountManager`, `SystemState`, OpenCV property 계열(`BlobProperty`, `ContourProperty`, `FeatureMatchingProperty`, `LineGaugeProperty`, `MatchingProperty`, `MeanProperty`, `VisionProperty`)의 XML 파일 로드/저장을 새 공통 API로 전환했고, 기존 `SerializeHelper.FromXmlFile/ToXmlFile`은 호출부가 없어 제거했다. `SystemState`는 생성자 재진입을 피하기 위해 `SystemStateConfig` DTO를 사용한다. 사용하지 않아 삭제된 `SocketProperty`/`LotProperty`는 호환성 체크 대상에서 제외했다. `tools/RecipeXmlCompatibilityCheck`는 현재 대상 기준 13개 XML root/sample 역직렬화를 확인한다. `OpenVisionLab.sln` Debug / Any CPU 빌드에 성공했고, `dotnet run --project tools\RecipeXmlCompatibilityCheck\RecipeXmlCompatibilityCheck.csproj --no-restore -- C:\Git\OpenVisionLab_Dev\bin\Debug` 검증도 통과했다.
- 2026-06-08: 후속 리팩토링 1~5번을 진행했다. `FormTeachingVision` 디자이너에서 활성 UI에 붙지 않는 `rjButton1/2/3`, `timerStatus`, `timeragin`, 빈 `label2` 잔재와 미연결 핸들러를 제거했다. Vision Test의 `FormVision_EdgeDection`은 파일/클래스/디자이너/리소스까지 `FormVision_EdgeDetection`으로 변경했고, `cIVT_CVContour`, `btnMinimizar_Click` 등 일부 잔여 레거시 이름을 정리했다. `FormMainFrame`에서는 동작 없는 옵션 메뉴(Profile/Terms/Help/Logout/Exit), 미사용 Device 드롭다운, 빈 이벤트 핸들러를 제거하고 옵션 메뉴를 실제 동작하는 `Tools > Image Compare`, `Settings` 중심으로 줄였다. `tools/RecipeXmlCompatibilityCheck` 콘솔 도구를 추가해 기존 XML 루트명 10개(`CData`, `CPropertyBlob` 등)의 역직렬화 호환성을 검증할 수 있게 했다. 메인 프로젝트가 `tools` 소스를 기본 compile에 포함하지 않도록 `OpenVisionLab.csproj`에 제외 규칙을 추가했다. 일반 Debug / Any CPU 빌드에 성공했고, `dotnet run --project tools\RecipeXmlCompatibilityCheck\RecipeXmlCompatibilityCheck.csproj -- bin\Debug` 검증도 통과했다.
- 2026-06-08: `FormMainFrame` 후속 리팩토링을 진행했다. 캡처/종료/툴 메뉴/드라이브 상태 타이머 핸들러를 `FormMainFrame.Actions.cs` partial로 분리했고, 메인 파일은 초기화/메뉴 전환 중심으로 줄였다. 화면 캡처는 `Bitmap`/`Graphics`를 `using`으로 정리하고, 우클릭 캡처 메뉴는 중복 `ItemClicked` 구독이 쌓이지 않도록 보정했다. `btnAuthoriztionName`, `btnAuthoriztion`, `btnCerrar`, `btnMinimizar`, `rjButton2`는 `btnAuthorizationName`, `btnAuthorizationIcon`, `btnClose`, `btnMinimize`, `btnScreenCapture`로 바꿨다. `IOpenCVPropertyLineGuage`는 `IOpenCvPropertyLineGauge`로 정리했고, `DrawCVLineGuage`/`SetCPropertyLineGuage` helper는 `DrawLineGauge`/`SetLineGaugePropertyVisibility`로 변경했다. 일반 Debug / Any CPU 빌드에 성공했다.
- 2026-06-08: 일반 Debug 빌드에서 `OpenVisionLab.FormMainFrame.resources` 중복 오류가 발생하던 문제를 수정했다. 원인은 리소스가 필요 없는 partial 파일 `FormMainFrame.Window.cs` 옆에 `FormMainFrame.Window.resx`가 생겨 `FormMainFrame.resx`와 같은 리소스 이름으로 확인된 것이다. `FormMainFrame.Window.resx`를 삭제했고, `OpenVisionLab.sln` Debug / Any CPU 일반 빌드가 `bin\Debug\OpenVisionLab.dll` 출력까지 성공한다.
- 2026-06-08: C 접두어 타입명을 대규모로 정리했다. 주요 변경은 `GlobalState`, `RecipeState`, `DataState`, `AppLog`, `AppUtil`, `AppCommon`, `LayerViewerState`, `PropertyGridEventBinder`, `InspectionAlgorithm`, Vision property 계열(`BlobProperty`/`ContourProperty`/`LineGaugeProperty` 등), OpenCV tool/result 계열(`BlobTool`/`LineGaugeTool`/`BlobResult` 등)이다. `CImageConverter`는 단순 `ImageConverter`가 `System.Drawing.ImageConverter`와 충돌해 `BitmapImageConverter`로 정리했다. 기존 레시피/XML 호환을 위해 직렬화 대상 모델에는 기존 XML 루트명(`[XmlRoot("CPropertyBlob")]`, `[XmlRoot("CData")]` 등)을 유지했고, `ParameterProperty`의 기존 connector XML element 이름도 `[XmlElement]`로 보존했다. Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드에 성공했고, 활성 소스 기준 C 접두어 타입 선언은 남지 않는다.
- 2026-06-08: 메인 프레임 이름을 현재 역할에 맞게 `FormMainFrame`으로 변경했다. `.cs`, `.Designer.cs`, `.resx` 파일명과 생성자/참조(`Program.cs`, `AppCommon.GetMessageBoxOwner()` 등)를 함께 정리했고, 창 크기/위치/타이틀바/상태바 배치 책임은 `FormMainFrame.Window.cs` partial 파일로 분리했다. 시스템 상태 클래스는 `SystemState`로 변경하고 내부 enum도 `RunMode`, `MenuKind`, `ResultState`로 정리했다. 단, `DEFINE.RESULT`는 전역 정의 enum이라 이번 변경 대상에서 제외하고 기존 이름을 유지했다. Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드에 성공했으며, 활성 소스 기준 예전 메인 프레임/시스템 상태 타입명 참조는 남지 않는다.
- 2026-06-08: Vision Test 디자이너 안정성을 위해 `VisionTestForm`의 런타임 상태 노출을 줄였다. `source_1`, `source_2`, `destination`은 `protected readonly`로, `source1_Index`, `source2_Index`, `destination_Index`, `eventUpdateDisplay`, `host`, `wpg`는 `protected`로 낮춰 외부/public 노출과 디자이너 직렬화 가능성을 줄였다. 안전한 명명 정리로 `UpdatePixelProperty()`, `GetRotateToTexture()`, `ReshapeNonRefresh()`를 추가하고 기존 오타 메서드(`UpdatePiexelProperty`, `GetRoateToTexture`, `ReshapeNonRefrsh`)는 호환 wrapper로 유지했다. `FormVision_HSV`의 `sourc1` 지역 변수는 `sourceMat`으로 정리하고 Mat 수명을 `using`으로 보강했다. Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드에 성공했고, `bin`/`obj` 기준 `Cyotek`/`ImageBox` 검색 결과도 없다.
- 2026-06-08: Vision Test WinForms 디자이너가 `Cyotek.Windows.Forms.ImageBox`를 찾지 못해 열리지 않던 문제를 정리했다. 활성 소스/리소스/프로젝트에는 Cyotek 참조가 없었지만, 오래된 `bin`/`obj` 산출물의 `.deps.json`, FileList, DLL, AssemblyReference cache에 Cyotek 흔적이 남아 Visual Studio 디자이너가 옛 참조를 물고 있었다. `LayerViewerState`는 디자인타임에 노출되지 않도록 `[ToolboxItem(false)]`, `[DesignTimeVisible(false)]`를 추가했고, `VisionTestForm`의 `source_1`, `source_2`, `destination` 런타임 상태 필드는 `[Browsable(false)]`와 `[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]`로 디자이너 직렬화 대상에서 제외했다. 오래된 Cyotek 산출물/cache를 삭제한 뒤 Debug / Any CPU 빌드에 성공했으며, 소스와 `bin`/`obj` 기준 `Cyotek`/`ImageBox` 검색 결과가 없다.
- 2026-06-08: 후속 리팩토링 1~5번을 진행했다. Vision Test 공통 베이스(`VisionTestForm`)에 `CreateSingleInputResult()`를 추가해 단일 입력 검사 폼의 Mat 준비/ROI 적용/overlay 결과 생성 흐름을 공통화했고, `FormVision_EdgeDetection`, `FormVision_Filter`, `FormVision_Histogram`, `FormVision_Morphology` 실행부의 반복 코드를 줄였다. `RoiImageCanvasViewModel`은 partial class로 전환하고 명령/파일 로드(`RoiImageCanvasViewModel.Commands.cs`)와 refresh timer(`RoiImageCanvasViewModel.Refresh.cs`)를 분리했다. `ImageCanvasControl`은 줌/뷰 상태/좌표 변환 책임을 `ImageCanvasControl.ViewState.cs`로 분리했다. `FormThreshold`는 현재 레이어 Mat 준비, ROI 여부 처리, 결과 Bitmap 생성 흐름을 `CreateLayerOperationResult()`로 공통화했다. 프로젝트 참조는 `Lib.Common`, `Lib.OpenCV`의 `System.Drawing.Common`을 `8.0.27`로 통일했다. Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드에 성공했다.
- 2026-06-08: 프로그램 시작 시 메인 창이 숨김/비활성처럼 보일 수 있는 문제를 보정했다. Threshold 도킹 창은 의도된 UI대로 왼쪽 `DockLeftAutoHide` 탭 상태를 유지한다. 스플래시 종료 직후 `FormMainFrame`이 최소화/비표시/화면 밖 Bounds 상태로 남지 않도록 `EnsureStartupWindowVisible()` 보정 흐름만 추가했다.
- 2026-06-08: 뷰어 하단 상태바의 좌표/색상 표시를 사용자 관점으로 정리했다. 기존 `Pos`/`Img` 약어는 의미가 불명확해 `좌표(좌하)`와 `이미지좌표(좌상)`으로 바꿨다. `좌표(좌하)`는 기존 OpenGL 좌하단 원점 좌표이고, `이미지좌표(좌상)`은 Bitmap/OpenCV에서 사용하는 좌상단 원점 이미지 좌표다. `FormLayerDisplay`, `RoiImageCanvasView`, `FormImageCompare` 하단 상태바에 반영했고, 색상은 `RGB(...)` 숫자만 보여주지 않고 작은 색상 사각형 swatch도 같이 표시하도록 했다. Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드에 성공했으며, 기존 `Lib.OpenCV` 미사용 변수 경고만 남아 있다.
- 2026-06-08: 추가 리팩토링 후속과 좌표 표시 개선을 적용했다. `OpenGlDrawing`에서 기본 도형(`OpenGlShapeDrawing.cs`), pen drawing(`OpenGlPenDrawing.cs`), 측정 렌더링(`OpenGlMeasurementDrawing.cs`), overlay display list compile(`OpenGlOverlayCompiler.cs`)을 추가 partial 파일로 분리했다. `OpenVisionLab.ImageCanvas` 어셈블리에 Windows 전용 플랫폼 특성을 추가해 Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드의 CA1416 Windows API 경고를 정리했다. 또한 `ImageCanvasControl.ImagePixelPos`를 추가해 기존 좌하단 OpenGL 좌표(`PixelPos`)와 별도로 좌상단 이미지 좌표를 계산한다. `FormLayerDisplay` 하단 상태바와 `RoiImageCanvasView` 상태바는 기존 `Pos` 좌하단 좌표와 새 `Img` 좌상단 좌표를 함께 표시하며, `FormImageCompare`의 상태 문구도 좌상단 이미지 좌표임을 `Img`로 명확히 표시한다. Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드에 성공했다.
- 2026-06-08: 추가 리팩토링 1~5번을 적용했다. `OpenGlDrawing`은 partial class로 전환하고 텍스트 렌더링(`OpenGlTextDrawing.cs`), 텍스처 렌더링(`OpenGlTextureDrawing.cs`), 색 변환(`OpenGlColorConversion.cs`), overlay 텍스트/텍스처(`OpenGlOverlayTextDrawing.cs`) 파일로 책임을 분리했다. `ImageCanvasControl`에는 `ImageCanvasTextureStore`를 추가해 texture id 수집/중복 제거/삭제를 한곳에서 처리하도록 정리했다. Vision Test 단일 입력 폼 일부(`Blob`, `Contour`, `FeatureMatching`, `Line`, `Matching`, `Mean`)는 `CreateRunSourceMat()` helper로 입력 Mat/결과 Bitmap 준비 흐름을 공통화했다. 소스가 사라지고 산출물만 남아 있던 `Library/Controls.Viewer2D` 폴더는 삭제했다. 깨진 README를 현재 .NET 8/OpenGL 전환 상태에 맞게 다시 작성하고, OpenGL 렌더링 파일의 깨진 주석을 제거했다. Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드에 성공했다.
- 2026-06-08: PC2 실기 확인 기준으로 `FormImageEditView`의 핵심 ROI 동작 1차 검증을 완료 처리했다. 확인된 항목은 ROI 신규 생성/기존 ROI 선택/꼭지점 리사이즈/내부 드래그 이동, 단일 ROI 모드의 기존 ROI 교체, `MULTI_ROI` 누적 및 `SelectedRegions` 갱신, 빈 영역 새 ROI 드로잉 진입 시 이전 리사이즈 상태 해제, OpenGL display list 잔상 여부다. 이후 작업은 남은 삭제 키 흐름, Teaching/MetroFrame 레이아웃 확인, Vision Test 실제 폼 흐름 확인, 레거시 폴더/참조 정리와 추가 리팩토링 후보 검토로 이어간다.
- 2026-06-08: 이미지 파일 로드/텍스처 업로드 유틸을 현재 역할 기준 이름으로 정리했다. 파일은 `Util/CanvasImageLoader.cs`, 클래스명은 `CanvasImageLoader`로 사용한다. 공개 메서드는 `LoadMatFromFile()`과 `UploadMatAsTexture()` 기준이며, 호출부는 `RoiImageCanvasViewModel`, `FormImageCompare`, `FormLayerDisplay`, `VisionTestImageCanvas` 기준으로 갱신했다. 이전 이름 검색 결과는 소스/문서 기준으로 남지 않으며, Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드에 성공했다. 빌드에는 기존 `OpenVisionLab.ImageCanvas` Windows API CA1416 경고가 남아 있다.
- 2026-06-08: `Library/OpenVisionLab.ImageCanvas`에서 툴킷 전용 잔재를 삭제했다. 삭제 대상은 비활성 `Command` 폴더, `Toolkit` 폴더, `View/ToolKitImageCanvasView.xaml(.cs)`, `ViewModel/ToolKitImageCanvasViewModel.cs`다. 활성 뷰어에서 쓰는 `CanvasImageLoader.LoadMatFromFile()`/`UploadMatAsTexture()`는 유지하되, 툴킷 마스크/ODB 저장 전용 메서드와 `EnumToolkitEditMode`/툴킷 드로잉 enum은 제거했다. 또한 `OpenGL` 폴더의 `Gl*` 접두어 클래스/파일을 `OpenGl*` 이름으로 변경했다. 주요 변경은 `GlDrawing` -> `OpenGlDrawing`, `GlRenderer` -> `OpenGlRenderer`, `GlTextureDrawingParam` -> `OpenGlTextureDrawingParam`, `GlDrawTextOptions` -> `OpenGlTextDrawOptions`, `GlFontBitmapEntry` -> `OpenGlFontBitmapEntry`, `GlFontRenderOptions` -> `OpenGlFontRenderOptions`, `GlOverlayMethodsExtension` -> `OpenGlOverlayExtensions`다. 소스 기준 `ToolKit`/`Toolkit`/`EnumToolkitEditMode`/`Gl*` 접두어 타입 검색 결과가 없고, Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드에 성공했다. 빌드에는 기존 `Lib.OpenCV` 미사용 변수 경고와 `OpenVisionLab.ImageCanvas`의 Windows API CA1416 경고가 남아 있다.
- 2026-06-07: Vision Test 주요 검사 폼의 `RJButton` 디자이너 스타일을 런타임 `VisionTestForm.ApplyVisionTestCompactStyle()` 값과 맞췄다. 대상은 `FormVision_Arithmetic`, `Blob`, `Contour`, `EdgeDetection`, `FeatureMatching`, `Filter`, `Histogram`, `HSV`, `Line`, `Matching`, `Mean`, `Morphology`, `RotateAndScale`의 디자이너 버튼이며, 배경 `250,252,253`, 글자/아이콘 `35,85,132`, 테두리 `47,111,171`, `Segoe UI 9 Bold`, `BorderRadius = 3`, `BorderSize = 1`, `ControlStyle.Glass`로 통일했다. 보조 팝업인 `FormVision_NewPanel`, `FormVision_Result`는 이번 런타임 스타일 통일 대상에서 제외했다. Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드에 성공했고, `Lib.OpenCV`의 기존 미사용 변수 경고만 남았다.
- 2026-06-07: 나머지 주요 Vision Test 폼 레이아웃도 코드비하인드 런타임 재배치가 아니라 WinForms 디자이너 좌표로 직접 이전했다. `FormVision_Blob`, `Contour`, `FeatureMatching`, `Matching`, `Mean`은 왼쪽 `Input/Output` 2단 뷰어와 오른쪽 파라미터/실행 버튼 구조로 정리했고, `Filter`, `Histogram`, `Morphology`는 왼쪽 뷰어 2단 + 오른쪽 Operations/Kernel/Shapes 설정 구조로 정리했다. `HSV`와 `Rotate / Scale`은 조작 슬라이더를 오른쪽 컬럼에 배치했고, `Line`은 Edge 선택, 파라미터, Details/Edge/Fit Line/Vertical Line 버튼을 오른쪽 작업 컬럼으로 분리했다. `FormVision_NewPanel`과 `FormVision_Result`는 보조 팝업이라 이번 레이아웃 일괄 변경 대상에서 제외했다. Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드에 성공했다.
- 2026-06-07: Vision Test 폼 공통화를 완료했다. `VisionTestForm`에 `InitializeLayerList()`, `AcceptUserImageChange()`, `SelectLayer()`, `CreateDestinationLayer()`, `PublishResult()`, `InitializeSingleInputViewers()`를 추가했고, 실제 UI 컨트롤 타입인 `RJComboBox` 기준으로 레이어 선택/사용자 이미지 변경/결과 표시 흐름을 중앙화했다. 단일 입력 폼은 공통 초기화 헬퍼를 사용하고, `FormVision_Arithmetic`은 2개 입력 + 1개 결과 뷰어 구조를 별도 초기화하되 같은 공통 헬퍼를 사용한다.
- 2026-06-07: Vision Test 결과 표시 흐름을 정리했다. 각 검사 폼이 직접 `SetLayerImage()`와 `eventUpdateDisplay`를 호출하던 경로를 `PublishResult()`로 모았고, `VisionTestImageCanvas`는 `DisplayImage`, `DisplayBitmap`, `UserImageChanged`를 새 이름으로 제공한다. 기존 호출부 호환을 위해 `Image`, `Bitmap`, `ImageChanged`는 alias로 남겨두었다.
- 2026-06-07: ImageCanvas 텍스처 수명 정책과 CA1416 Windows 경고 정리를 확인했다. `VisionTestImageCanvas.Dispose()`는 `ClearTextureStateOnly()`로 transient 뷰어의 managed 상태만 비우고, 실제 표시 소유 뷰어는 기존 `ClearTexture()`로 OpenGL texture id까지 삭제한다. `OpenVisionLab.ImageCanvas.csproj`는 `net8.0-windows`와 `SupportedOSPlatformVersion` 설정을 사용하며, Debug / Any CPU 별도 출력 폴더(`bin\CodexVerify`) 빌드에서 CA1416 경고 없이 성공했다.
- 2026-06-07: Vision Test 폼 WinForms 디자이너 로드 시 `System.Windows.Controls.WpfPropertyGrid` 원본 DLL을 찾지 못해 디자이너가 열리지 않는 문제를 수정했다. 원인은 일부 `FormVision_*Designer.cs`에 사용되지 않는 `hostedComponent* = new System.Windows.Controls.WpfPropertyGrid.PropertyGrid()` 잔재가 남아 디자이너가 원본 WPF PropertyGrid를 직접 로드하던 것이었다. 해당 디자이너 잔재를 제거했고, `VisionTestForm` 생성자에서는 Visual Studio/DesignToolsServer 디자인 타임을 감지하면 WPF PropertyGrid host 생성을 건너뛰도록 했다. `wpg` 필드는 `IPropertyGridView`로 낮추고, 런타임에서만 구체 `WpfPropertyGridBridge` 기반 PropertyGrid를 생성한다.
- 2026-06-07: Vision Test 폼 표시 속도 개선을 적용했다. `VisionTestForm` 생성자에서 WPF PropertyGrid를 즉시 생성하지 않고, `AttachPropertyGrid()`가 호출되는 `Blob`, `Contour`, `FeatureMatching`, `Line`, `Matching`, `Mean` 계열에서만 lazy 생성/부착한다. `InitializeSingleInputViewers()`와 `FormVision_Arithmetic`은 초기 이미지 복사, OpenCV Mat 변환, OpenGL 텍스처 업로드, `ZoomToFit()`을 `BeginInvoke`로 지연해 폼 shell이 먼저 뜨도록 했다. `VisionTestImageCanvas` 내부의 실제 `ImageCanvasControl`도 이미지/줌이 처음 필요할 때 생성하도록 lazy화했고, `[PERF]` 로그로 생성자 shell, PropertyGrid 생성, viewer event init, 초기 이미지 로드, GL viewer 생성, texture upload 시간을 남긴다.
- 2026-06-07: 추가 리팩토링 1~6번을 진행했다. `VisionTestImageCanvas`의 구식 호환 alias(`Image`, `Bitmap`, `ImageChanged`)를 제거하고 `DisplayImage`, `DisplayBitmap`, `UserImageChanged`만 남겼다. `VisionTestForm`에는 단일 입력 폼용 공통 헬퍼(`InitializeSingleInputLayerList`, `AcceptSourceImageChange`, `AcceptDestinationImageChange`, `SelectSourceLayer`, `SelectDestinationLayer`, `CreateSingleInputDestinationLayer`)를 추가했고, 단일 입력 `FormVision_*` 폼들의 반복 본문을 해당 헬퍼 호출로 정리했다. `PropertyGrid.Abstractions.IPropertyGridView`에 이벤트를 추가해 `PropertyGridEventBinder`는 WPF `PropertyGrid` 구체 타입 대신 `IPropertyGridView`/`PropertyGridPropertyValueChangedEventArgs`를 사용한다. 커스텀 editor 정의 때문에 `PropertyGridEditorFactory`의 WPF editor 타입 의존성은 유지했다.
- 2026-06-07: 사용하지 않는 옛 `Library/Controls.Viewer2D` 폴더를 삭제하고, `OpenVisionLab.csproj`의 해당 폴더 제외 항목도 제거했다. 현재 실사용 뷰어 프로젝트는 `Library/OpenVisionLab.ImageCanvas`다. 삭제 후 `Controls.Viewer2D`/`ToolKit2DViewer`/`Editable2DViewer` 검색 결과는 없고, Debug / Any CPU 별도 출력 폴더 빌드에 성공했다.
- 2026-06-07: 추가 리팩토링 후속으로 `LayerViewerState`를 순수 레이어 ROI 상태 컴포넌트로 축소했다. ImageBox 시절 컨텍스트 메뉴/FontAwesome 메뉴/10ms 타이머가 있던 `LayerViewerState.Designer.cs`와 `LayerViewerState.resx`를 삭제했고, `LayerViewerState.cs`에서는 빈 `InitializeComponent()` 호출, 빈 타이머 핸들러, 사용하지 않는 `displayTitle` 저장을 제거했다. `VisionTestForm`에는 `ApplyVisionTestCompactStyle()`을 추가해 Vision Test 폼의 콤보/버튼/아이콘 크기를 런타임에서 compact하게 보정한다. FontAwesome은 `RJControls`/메인 프레임 계층에 아직 남아 있으나, `LayerViewerState`에서는 제거됐다.
- 2026-06-07: `FormVision_*` 폼이 Euresys 계열 예제/라이브러리 UI를 참고해 작성된 것으로 보인다는 사용자 우려를 검토했다. 기능적 아이디어, 알고리즘명, 일반적인 Source/Destination/Parameter 구성 자체보다, 특정 제품 UI의 전체 배치, 색상, 아이콘, 문구, 버튼 순서, 파라미터 노출 방식이 비슷하게 남아 있으면 저작권/트레이드드레스 분쟁 여지가 커질 수 있다. 향후 안전성을 높이려면 Vision Test 폼을 OpenVisionLab 고유 레이아웃으로 재설계하고, 문구/색상/아이콘/워크플로우를 차별화하는 방향을 권장한다.
- 2026-06-07: 위 UI 유사성 우려를 낮추기 위한 1차 변경을 적용했다. `VisionTestForm.ApplyVisionTestCompactStyle()`에서 Vision Test 폼 공통 배경/버튼/콤보 스타일을 OpenVisionLab 계열의 gray-blue 톤으로 보정한다. 단, Vision Test 폼들은 절대좌표 디자이너 배치가 많으므로 공통 스타일러가 `RJButton`/`RJMenuIcon`의 위치, 크기, 높이, 아이콘, 텍스트 정렬은 변경하지 않도록 제한했다. 사용자 노출 문구는 `Input Layer`/`Output Layer`, `Run`/`Details`, `FormVision_Arithmetic`의 `Input A/B`로 정리했다. 새 레이어 생성 툴팁도 `Create Output Layer`로 변경했고, `Source Image`/`Destination Image`/`EXCUTE`/`RESULT`/`Create New Layer` 검색 결과가 남지 않도록 공통 스타일러 조건문도 컨트롤 이름 기준으로 좁혔다. Debug / Any CPU 별도 출력 폴더 빌드에 성공했다.
- 2026-06-07: Vision Test 폼 표시 직후 `RJButton.IconSize = 0` 때문에 `ArgumentException(Parameter is not valid)`이 발생하던 문제를 수정했다. FontAwesome/RJButton 계열은 0 크기 아이콘을 허용하지 않으므로, 아이콘은 `IconChar.None`으로 숨기고 `IconSize`는 최소 유효값인 `1`로 유지한다. `IconSize = 0` 잔여 검색 결과는 없고, Debug / Any CPU 별도 출력 폴더 빌드에 성공했다.
- 2026-06-07: Euresys 예제/라이브러리 UI와 전체 배치가 닮아 보일 수 있다는 우려 때문에 Vision Test 폼 레이아웃 재설계를 진행 중이다. 처음에는 `VisionTestForm.ApplyVisionTestWorkflowLayout()` 런타임 재배치로 빠르게 검증했지만, WinForms 뷰는 디자이너에서 관리해야 한다는 방향에 맞춰 해당 런타임 레이아웃 호출과 메서드 블록을 제거했다. 우선 `FormVision_EdgeDetection.Designer.cs`와 `FormVision_Arithmetic.Designer.cs`에 새 좌표/크기를 직접 반영했다. `Edge Detection`은 왼쪽 `Input/Output` 2단 뷰어와 오른쪽 `Operations/Tab/Run` 구조이고, `Arithmetic`은 상단 `Input A/Input B/Output` 3뷰어 스트립과 하단 `Input B/Operation`, `Contrast`, `Shift`, 전체 폭 `Run` 버튼 구조다. Debug / Any CPU 별도 출력 폴더 빌드에 성공했다.
- 2026-06-07: 레이아웃 패널 표시용 `FormLayerDisplay`를 Cyotek `ImageBox`/`LayerViewerState` 직접 호스팅에서 `ImageCanvasControl` 직접 호스팅으로 전환했다. `FormLayerDisplay`는 `SetImage()`, `GetCurrentImage()`, `RefreshViewer()`, `ZoomToFit()`, `AcceptImageChanged()`를 제공하고, 이미지 텍스처는 폼 표시 이후 지연 로드한다. 상태바는 `ImageCanvasControl.PixelColor`, `PixelPos`, `GrayValue`, `ZoomScale` 기준으로 갱신한다.
- 2026-06-07: `FormLayerDisplay`를 GL 뷰어로 전환하면서 빠졌던 우클릭 이미지 컨텍스트 메뉴를 복구했다. `ImageCanvasControl` 우클릭 시 `RoiImageCanvasView`와 같은 `Load Image`/`Save Image` 항목이 아이콘과 함께 뜨며, 로드된 이미지는 `FormLayerDisplay`와 `ImageSpace`에 함께 동기화된다. WinForms 호스트용 메뉴 생성은 `CanvasContextMenuFactory`로 분리했다.
- 2026-06-07: `FormLayerDisplay`의 Layer 이미지 로드 시 8bpp/Indexed/stride 이미지가 OpenGL 텍스처에서 깨질 수 있어, 텍스처 업로드 전 `Format24bppRgb` 비트맵으로 정규화하도록 수정했다. 하단의 기존 흰색 `R,G,B/X,Y/GV/ZOOM` RJLabel 상태바는 숨기고, OpenGL ROI 뷰어와 같은 어두운 `Pos/Gv/Color` 스타일 상태바로 교체했다. Layer 컨텍스트 메뉴 아이콘도 Edit 메뉴와 같은 검정색으로 맞췄다.
- 2026-06-07: `0. UI/6) Vision Test` 계열 ImageBox 전환 인벤토리를 시작했다. 대부분은 `ibSource` + `ibDestination` 단일 입력/단일 결과 패턴이고, `FormVision_Arithmetic`은 `ibSource1`/`ibSource2`/`ibDestination` 3개 뷰어라 후순위로 분류했다. 이후 `FormVision_Arithmetic`과 Vision Test 범위 밖의 `FormMeasure`까지 전환 완료했다.
- 2026-06-07: Vision Test용 WinForms GL 래퍼 `VisionTestImageCanvas`를 추가했다. 기존 폼 코드가 기대하는 `Image`, `ImageChanged`, `MouseClick`, `ZoomToFit()`, `Invalidate()` 표면을 유지하면서 내부는 `ImageCanvasControl`로 렌더링한다. 우클릭 메뉴는 `CanvasContextMenuFactory`를 사용하고, 이미지 업로드 전 `Format24bppRgb`로 정규화한다.
- 2026-06-07: 단일 입력/단일 결과 Vision Test 폼의 `ibSource`/`ibDestination`을 Cyotek `ImageBox`에서 `VisionTestImageCanvas`로 교체했다. 대상은 `FormVision_Blob`, `Contour`, `EdgeDetection`, `FeatureMatching`, `Filter`, `Histogram`, `HSV`, `Line`, `Matching`, `Mean`, `Morphology`, `RotateAndScale`다. 각 폼의 `source_1.LoadImageBox()`/`destination.LoadImageBox()` 연결은 제거했고, 기존 레이어 선택/이미지 변경/실행 로직은 같은 `Image` API를 통해 유지한다. Debug / Any CPU 별도 출력 폴더 빌드에 성공했다.
- 2026-06-07: 프로젝트 전체에서 Cyotek `ImageBox` 의존성을 제거했다. `FormVision_Arithmetic`과 `FormMeasure`도 `VisionTestImageCanvas`로 전환했고, `LayerViewerState`는 ImageBox 호스팅 컴포넌트가 아니라 ROI 상태/DisplayManager 연결만 남긴 경량 컴포넌트로 축소했다. `DrawObject`의 `ImageBox` 타입 인자와 불필요한 `using Cyotek.Windows.Forms`를 제거했으며, `OpenVisionLab.csproj`의 `Cyotek.Windows.Forms.ImageBox` 참조와 `dll/Cyotek.Windows.Forms.ImageBox.dll` 파일을 삭제했다. 소스/XAML/프로젝트 파일 기준 `rg "ImageBox"` 결과가 없고, Debug / Any CPU 별도 출력 폴더 빌드에 성공했다.
- 2026-06-07: Vision Test 폼(`FormVision_Contour` 등)을 띄운 상태에서 검사 후 뒤쪽 메인 레이어 뷰어를 드래그하면 텍스처 위치가 원래대로 돌아가던 문제를 수정했다. 원인은 타이머가 `ibSource/ibDestination.Image`를 주기적으로 재할당하고, `VisionTestImageCanvas.Image` setter가 그 표시 갱신까지 `ImageChanged`로 다시 메인 레이어에 써서 `FormLayerDisplay.SetImage()`/`ZoomToFit()`이 반복 호출되던 것이다. 이제 `Image` setter는 조용한 표시 갱신만 수행하고, 우클릭 이미지 로드처럼 사용자가 뷰어 안에서 이미지를 바꾼 경우에만 `ImageChanged`를 발생시킨다. 같은 레이어 Bitmap 참조가 반복 전달되면 텍스처 재업로드도 건너뛴다.
- 2026-06-07: Vision Test 폼을 닫은 뒤 메인 레이어의 결과 이미지가 하얗게 남는 문제를 완화했다. `VisionTestImageCanvas.Dispose()`에서 transient 검사 폼의 GL 텍스처를 명시 삭제하지 않도록 변경해, SharpGL 컨텍스트/texture id 공유 상황에서 메인 레이어 텍스처가 함께 삭제될 가능성을 제거했다. 또한 `FormLayerDisplay.SetImage()`는 외부 Result Bitmap 참조를 그대로 잡지 않고 24bpp 복사본을 소유하며, `DisplayImageSyncService`/`DisplayLayerPresenter`도 이 복사본을 `ImageSpace`에 저장하도록 정리했다. Debug / Any CPU 별도 출력 폴더 빌드에 성공했다.
- 2026-06-07: ImageCanvas 텍스처 수명 정책을 명시했다. `ImageCanvasControl.ClearTexture(bool deleteOpenGlTextures = true)`로 기본 호출은 기존처럼 OpenGL texture id까지 삭제하고, transient Vision Test 뷰어 dispose는 `ClearTexture(false)`를 호출해 컨트롤의 managed texture 상태만 비운다. 메인 레이어/이미지 비교처럼 자신이 실제 표시 소유자인 GL 뷰어는 계속 기본 `ClearTexture()`를 사용한다.
- 2026-06-07: Vision Test 검사 폼의 ImageBox 시절 타이머 잔재를 제거했다. `FormVision_Blob`, `Contour`, `FeatureMatching`, `EdgeDetection`, `Histogram`, `Filter`, `Arithmetic`, `HSV`, `Line`, `Matching`, `Mean`, `Morphology`, `RotateAndScale`에서 `timer1` 생성/필드/이벤트/`timer1_Tick()`/constructor start 로직과 `.resx` tray metadata를 제거했다. 레이어 목록/이미지 갱신은 폼 로드, 콤보 선택 변경, 새 패널 버튼, 검사 실행 시점에만 일어난다. `VisionTestForm`의 unused `panelCount`, `BindLayerToViewer()`, `RefreshViewerRoi()` helper도 삭제했다. `FormVision_NewPanel`과 `FormVision_Result`의 타이머는 검사 뷰어 동기화용이 아니므로 유지했다.
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
- 2026-06-07: `FormImageEditView`의 활성 OpenGL 경로에서 `LayerViewerState`/ImageBox 초기화 의존성을 제거했다. `LegacyViewer`, `ImageGrey`, `UseImageCanvas` 분기, `ibSource_MouseMove()` fallback도 함께 제거했다.
- 2026-06-07: ImageCanvas 좌표/GV 상태를 WinForms 쪽에서 받을 수 있도록 `FormImageEditView.ImageCanvasStatusChanged` 이벤트를 추가했다. `RoiImageCanvasViewModel.PropertyChanged`의 `RobotPos`, `GrayValue`, `PixelColor` 갱신을 받아 이벤트로 발행하며, 오른쪽 숨김 라벨은 내부 상태 동기화만 유지한다.
- 2026-06-07: `Editable` 명칭을 활성 ImageCanvas 소스에서 제거했다. `RoiImageCanvasView`, `RoiImageCanvasViewModel`, `RoiInteraction` 폴더와 `RoiInteractionMouseDown/Move/Up/KeyDown/Cursor` helper 이름을 사용한다.
- 2026-06-07: ROI 편집 핸들 렌더링 함수는 `OpenGlDrawing.DrawRoiEditHandles()`로 정리했다.
- 2026-06-07: `FormImageEditView`는 `RoiImageCanvasView`/`RoiImageCanvasViewModel`과 `RoiAdded`, `RoiMouseUp`, `RoiEditingCompleted`, `RoiChangedEventArgs.RoiRect` 기준으로 연결한다.
- 2026-06-07: `EnumViewMode`, `TKMouseEventArgs`, `FontBitmapEntry` 등 compatibility 잔여 타입도 `CanvasInteractionMode`, `CanvasMouseEventArgs`, `OpenGlFontBitmapEntry` 등으로 분리/변경했다.
- 2026-06-07: `Library/OpenVisionLab.ImageCanvas`와 `FormImageEditView.cs` 기준으로 `Editable`, `NX`, `TX`, `Diagram` 계열 이전 명칭 검색 결과가 없도록 정리했다.

## 0-1. ImageBox -> ImageCanvas 전환 인벤토리

2026-06-07 기준 `FormImageEditView`의 OpenGL 전환은 활성 ROI 편집/패턴 등록 경로에서 ImageBox/LayerViewerState fallback을 제거하는 단계까지 진행했다.

현재 `FormImageEditView`에서 이미 ImageCanvas가 담당하는 항목:

- 이미지 표시 주 경로: `InitializeImageCanvas()`에서 `RoiImageCanvasView`를 `ElementHost`에 붙인다.
- 이미지 로드: `PendingImageCanvasImage`를 `Shown` 이후 `RoiImageCanvasViewModel.LoadImage(mat, "Source")`로 전달한다.
- 기존 ROI 로드: 생성자에서 받은 `Rectangle`/`List<Rect>`를 `PendingImageCanvasRois`에 저장하고 `AddInitialRoi()`로 OpenGL overlay에 추가한다.
- ROI 결과: `RoiAdded`, `RoiMouseUp`, `RoiEditingCompleted` 이벤트로 `ImageCanvasSelectedRegion`, `ImageCanvasSelectedRegions`를 갱신한다.
- 단일/다중 ROI 반환: `btnCut_Click()`에서 단일 ROI는 `ImageCanvasSelectedRegion`, 다중 ROI는 `ImageCanvasSelectedRegions`를 우선 사용한다.

`FormImageEditView`에서 제거 완료한 ImageBox/LayerViewerState 항목:

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
- `FormMainFrame`은 `TeachingVision`과 `pnStatusBar`가 서로 겹치지 않도록 메인 영역과 상태바를 분리하는 방향으로 정리했다.
- System/Property dock 창은 메인에서 일단 보이지 않도록 처리하는 방향으로 결정했다.

## 3. 수정한 파일 목록

현재 스레드에서 주요하게 수정하거나 생성한 파일은 다음과 같다. PC2에서는 GitHub Desktop의 Changes 탭에서 실제 diff를 한 번 더 확인하는 것이 좋다.

- `OpenVisionLab.sln`
- `OpenVisionLab.csproj`
- `0. UI/0) MENU/FormMainFrame.cs`
- `0. UI/0) MENU/FormMainFrame.Designer.cs`
- `0. UI/0) MENU/FormMainFrame.resx`
- `0. UI/0) MENU/FormMainFrame.Window.cs`
- `1. Core/SystemState.cs`
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

- `FormMainFrame`의 창 크기/위치 문제, 더블클릭 축소/복원, 전체 크기 버튼, 상단 버튼 정렬 문제를 정리했다.
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

- 완료: `FormImageEditView`에서 실제로 ROI 신규 생성, 기존 ROI 선택, 꼭지점 리사이즈, 내부 드래그 이동이 모두 정상인지 확인했다.
- 완료: 단일 ROI 모드에서 새 ROI를 그리면 기존 ROI가 화면/내부 선택값에서 모두 교체되는지 확인했다.
- 완료: `MULTI_ROI` 모드에서 ROI가 누적되고 `SelectedRegions`가 정상 갱신되는지 확인했다.
- 완료: 기존 ROI를 클릭하지 못한 빈 영역에서 새 ROI를 그릴 때 이전 ROI 리사이즈 상태로 남지 않는지 확인했다.
- 완료: OpenGL display list 잔상이 더 남지 않는지 확인했다.
- ROI 삭제 키(`Delete`) 흐름이 실제 UI 요구에 맞는지 확인한다.
- `FormTeachingVision` 상단 컨트롤, 상태바, 버전 표시, Tack Time 표시가 화면 해상도별로 잘 보이는지 확인한다.
- `FormMainFrame` 다중 모니터 환경에서 실행 위치/더블클릭 최대화/복원 동작을 다시 확인한다.
- `FormMainFrame` 옵션 메뉴에서 `Tools > Image Compare`, `Settings`만 표시되고 각 메뉴가 정상 동작하는지 확인한다.
- `FormTeachingVision` 상단 메뉴/카메라/레이어/Panel Source Image/New Layer/Tack Time 표시가 정상인지 확인한다.
- `FormVision_EdgeDetection` 메뉴 실행, 디자이너 열기, Canny/Sobel/Scharr/Laplacian 실행 흐름을 확인한다.
- 기존 recipe XML이 있는 PC에서 `dotnet run --project tools\RecipeXmlCompatibilityCheck\RecipeXmlCompatibilityCheck.csproj -- bin\Debug`를 실행해 XML 루트 호환성을 확인한다.
- GitHub Desktop 기준 변경 파일이 너무 넓으면 커밋 단위를 나눌지 검토한다.

다음 리팩토링 후보:

- PC2에서 Vision Test 각 검사 폼(`Blob`, `Contour`, `Filter`, `Histogram`, `Arithmetic` 등)을 실제로 열어 레이어 콤보 선택, 새 패널 생성, 실행 결과 표시, 메인 레이어 동기화가 정상인지 확인한다.
- `VisionTestImageCanvas`의 호환 alias(`Image`, `Bitmap`, `ImageChanged`)는 모든 호출부가 `DisplayImage`, `DisplayBitmap`, `UserImageChanged`로 안정화된 뒤 제거할지 결정한다.
- `FormVision_*` 디자이너 파일에 남은 큰 고정 좌표/절대 배치를 정리해 Vision Test 폼 UI를 더 compact하게 만든다.
- 완료: 비활성/레거시 `Library/Controls.Viewer2D` 폴더는 산출물만 남은 상태로 확인 후 삭제했다.

2026-06-08 추가 리팩토링 검토 메모:

- 완료: `OpenGlDrawing.cs`가 텍스트, 기본 도형, pen/measurement, texture drawing, overlay display list compile까지 모두 가진 큰 정적 클래스로 남아 있어, 우선 텍스트/텍스처/색 변환/overlay 텍스트 책임을 partial 파일로 분리했다. 기본 도형, measurement, overlay compiler 추가 분리는 후속 후보로 남아 있다.
- 완료: `ImageCanvasControl.cs`의 texture id 수집/삭제 중복을 `ImageCanvasTextureStore`로 분리했다. WinForms 이벤트 bridge, 좌표 변환, 뷰 상태, overlay, 픽셀/GV 조회 분리는 후속 후보로 남아 있다.
- 일부 완료: Vision Test 단일 입력 폼 중 반복 패턴이 명확한 `Blob`, `Contour`, `FeatureMatching`, `Line`, `Matching`, `Mean`은 `CreateRunSourceMat()` helper로 입력 Mat/결과 Bitmap 준비를 공통화했다. `Filter`, `Histogram`, `HSV`, `Morphology`, `RotateAndScale`, `Arithmetic`은 처리 패턴 차이가 있어 후속으로 별도 확인한다.
- 완료: `Library/Controls.Viewer2D` 폴더는 현재 `.vs`, `bin`, `obj`, `.csproj.user` 산출물만 남아 있고 활성 소스/솔루션 참조가 보이지 않아 삭제했다.
- 완료: `README.md`와 OpenGL 렌더링 파일 일부의 깨진 한글 주석을 정리했다.
- 주의: `VisionTestImageCanvas`의 예전 호환 alias는 현재 활성 소스 검색상 이미 `DisplayImage`/`DisplayBitmap`/`UserImageChanged` 중심으로 정리된 상태다. 제거 후보를 다시 잡을 때는 `FormMeasure`의 `SelectionRegion`, `PointToImage`, `ShowPixelGrid` 호환 표면까지 같이 확인해야 한다.

## 6. 주의해야 할 점

- EMGU 관련 참조/DLL은 제거하지 말 것. `CanvasImageLoader.LoadMatFromFile()` 흐름에서 필요하다.
- `DiagramControl`은 현재 없으므로 추가 전제로 작업하지 말 것.
- `Library/Controls.Viewer2D` 옛 폴더는 삭제됐다. 현재 실사용 프로젝트는 `Library/OpenVisionLab.ImageCanvas`다.
- `ToolKitImageCanvasViewModel`은 삭제됐다. ROI 편집 경로는 `RoiImageCanvasViewModel` 기준으로만 확인할 것.
- `FormTeachingVision` 레이아웃은 코드비하인드에서 위치를 억지로 보정하는 방식보다 Designer/InitializeComponent 구조를 우선해야 한다.
- `FormMainFrame`의 `pnStatusBar`가 Teaching 화면을 덮지 않도록 메인 영역과 상태바 영역을 분리해야 한다.
- 다중 모니터 이슈는 `Screen.FromControl(this)` 또는 현재 폼 위치 기준 화면을 사용해야 한다. 최초 실행 위치만 기준으로 삼으면 안 된다.
- OpenGL 다이어그램은 객체를 한 번 그려 display list로 관리하는 구조다. 삭제/교체 시 display list 해제를 빼먹으면 잔상이 남을 수 있다.
- ROI hit-test는 꼭지점/변/내부 순서가 중요하다. 변 hit-test는 반드시 실제 변 범위 안인지 검사해야 한다.
- `FormImageEditView`의 ImageCanvas ROI 이벤트는 OpenGL 좌하단 좌표를 반환한다. Bitmap/OpenCV crop 또는 `SelectedRegion`에 넣을 때는 `SourceBitmap.Height - rect.Top`으로 좌상단 이미지 좌표계에 맞춰야 한다. 반대로 기존 이미지 좌표 ROI를 `AddInitialRoi()`로 캔버스에 올릴 때는 `canvasTop = imageHeight - roi.Top`, `canvasBottom = imageHeight - roi.Bottom` 기준을 유지해야 한다.
- 뷰어 상태바의 `좌표(좌하)`는 기존 OpenGL 좌하단 원점 좌표이고, `이미지좌표(좌상)`은 Bitmap/OpenCV 기준 좌상단 원점 이미지 좌표다. crop/OpenCV 입력이나 사용자에게 익숙한 이미지 좌표가 필요하면 `이미지좌표(좌상)`을 기준으로 본다.
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
- FormTeachingVision/FormMainFrame 레이아웃은 코드비하인드 위치 보정보다 Designer 구조를 우선해서 봐주세요.

수정 전에는 관련 파일을 먼저 읽고, 필요한 변경만 좁게 적용한 뒤 OpenVisionLab.sln Debug / Any CPU 빌드까지 확인해주세요.
```
