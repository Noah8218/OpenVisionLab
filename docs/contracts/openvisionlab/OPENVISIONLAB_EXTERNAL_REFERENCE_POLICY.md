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

## 외부 바이너리 인벤토리와 allowlist

- 전체 DLL 인벤토리와 현재 물리 파일의 길이·SHA-256·참조·복사·재배포 상태는
  docs/contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json이
  소유합니다.
- tools/TestExternalReferences.ps1은 추적된 DLL과 dll 아래 실제 DLL이 모두
  manifest에 등록되어 있는지, 등록된 현재 파일의 길이·SHA-256이 일치하는지,
  금지 파일이 재도입되지 않았는지를 검사합니다.
- manifest의 blocked 항목은 저장소에 남아 있을 수 있는 분류 보류 자산일 뿐
  release 허가가 아닙니다. 라이선스, 정확한 출처, 또는 재배포 notice 근거가
  없으면 release package에 포함하지 않습니다.
- 새 DLL을 추가할 때는 먼저 소유자·직접/동적 참조·build/publish 경로·
  라이선스/notice 근거·release 정책을 manifest에 기록합니다. 등록되지 않은
  새 DLL은 외부 참조 gate에서 실패해야 합니다.
- manifest는 재현 가능한 파일 식별자이며 법률 자문을 대체하지 않습니다.

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

사용자는 `WPG-CUSTOM` DLL/XML을 직접 제작한 것으로 명시했다. 따라서 이
파일들은 현재 PropertyGrid 런타임 계약과 SHA-256 gate는 유지하되, PL-0005의
제3자 라이선스/재배포 증거 blocker로 분류하지 않는다. 이 분류는
`OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json`의
`user-created-owner-declaration` 상태가 소유한다.

WPG-CUSTOM을 변경한 경우 별도 저장소에서 빌드한 DLL/XML만 위 위치에 갱신하고 PropertyGrid bridge와 UI 상태 검사를 다시 실행합니다.

## 커밋 기록

SDK DLL 갱신 커밋에는 다음을 남깁니다.

- SDK 버전과 정확한 source commit
- 변경된 DLL과 `sdk-manifest.json`
- `OpenCvSharpExtern.dll` 갱신 여부
- 실제 실행한 빌드·스모크·외부 참조 검사의 결과
