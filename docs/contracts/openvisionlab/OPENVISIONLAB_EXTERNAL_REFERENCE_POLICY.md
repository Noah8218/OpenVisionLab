# OpenVisionLab 외부 DLL 참조 정책

Updated: 2026-08-05

OpenVisionLab은 GitHub 저장소 하나만 복제해 빌드할 수 있어야 합니다. 별도 SDK 소스 경로나 개발자 PC의 절대 경로를 `ProjectReference` 또는 `HintPath`로 사용하지 않습니다.

## 필수 DLL 구조

```text
dll\
  OpenVisionLab-Vision-SDK\
    OpenVisionLab.Core.dll
    OpenVisionLab.Vision2D.dll
    OpenVisionLab.Vision2D.Blob.dll
    OpenCvSharp.dll
    OpenCvSharp.Blob.dll
    sdk-manifest.json
  OpenCVSharp\
    OpenCvSharpExtern.dll
  System.Windows.Controls.WpfPropertyGrid.dll
  System.Windows.Controls.WpfPropertyGrid.xml
```

`Directory.Build.props`의 공통 경로는 다음과 같습니다.

- `OpenVisionLabDllRoot`: `dll\`
- `OpenVisionLabVisionSdkDllRoot`: `dll\OpenVisionLab-Vision-SDK\`
- `OpenVisionLabOpenCvSharpDllRoot`: `dll\OpenCVSharp\`

## SDK 소유권

- 알고리즘, Property 모델, Pipeline 런타임과 결과 모델은 OpenVisionLab Vision SDK 3.0이 소유합니다.
- OpenVisionLab은 SDK 3.0의 `OpenVisionLab.Core`, `OpenVisionLab.Vision2D`, `OpenVisionLab.Vision2D.Blob` 네임스페이스를 사용합니다.
- `System.Drawing.Bitmap`과 `OpenCvSharp.Mat` 사이의 UI 변환은 SDK가 아니라 OpenVisionLab의 `Common\BitmapImageConverter.cs`가 소유합니다.
- 구형 `Lib.Common.dll`, `Lib.OpenCV.dll`, `Lib.OpenCV.Blob.dll`, `OpenCvSharp.Extensions.dll`, `dll\Library-Noah` 폴더를 다시 추가하지 않습니다.
- 네이티브 `OpenCvSharpExtern.dll`은 `dll\OpenCVSharp\`에서 한 번만 공유합니다.

## SDK DLL 갱신 절차

1. `OpenVisionLab-Vision-SDK` 저장소의 확정 커밋에서 Release 빌드, 전체 스모크, 패키지 소비자 스모크를 통과시킵니다.
2. `OpenVisionLab.Core.dll`, `OpenVisionLab.Vision2D.dll`, `OpenVisionLab.Vision2D.Blob.dll`, `OpenCvSharp.dll`, `OpenCvSharp.Blob.dll`을 `dll\OpenVisionLab-Vision-SDK\`에 복사합니다.
3. `sdk-manifest.json`에 SDK 버전, 원격 저장소, 커밋, 파일 길이와 SHA-256을 기록합니다.
4. `OpenCvSharpExtern.dll`이 SDK 빌드본과 동일한지 SHA-256으로 확인합니다. 동일하면 공용 파일을 유지하고, 다르면 SDK와 함께 검증한 공용 파일로 갱신합니다.
5. 아래 검증을 실행합니다.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- <repository-root>
```

DLL 이름만 바꾸고 완료로 처리하지 않습니다. Threshold/Edge 같은 실제 OpenCV 실행, Pipeline XML 왕복, Preview/Run, 레이어 생성·삭제·입력 선택, Recipe 검증과 clean runtime 배포 검사를 함께 통과해야 합니다.

## WPG PropertyGrid

PropertyGrid는 준비된 다음 파일을 사용합니다.

- `dll\System.Windows.Controls.WpfPropertyGrid.dll`
- `dll\System.Windows.Controls.WpfPropertyGrid.xml`

WPG-CUSTOM을 변경한 경우 별도 저장소에서 빌드한 DLL/XML만 위 위치에 갱신하고 PropertyGrid bridge와 UI 상태 검사를 다시 실행합니다.

## 커밋 기록

SDK DLL 갱신 커밋에는 다음을 남깁니다.

- SDK 버전과 정확한 source commit
- 변경된 DLL과 `sdk-manifest.json`
- `OpenCvSharpExtern.dll` 갱신 여부
- 실제 실행한 빌드·스모크·외부 참조 검사의 결과
