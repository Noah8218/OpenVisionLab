# OpenVisionLab DLL Reference Policy

Updated: 2026-06-21

OpenVisionLab은 GitHub에서 이 저장소 하나만 받아도 빌드할 수 있도록 외부 소스 프로젝트를 직접 참조하지 않습니다. `Library-Noah`와 `WPG-CUSTOM`은 개발 편의를 위한 별도 소스 저장소일 수 있지만, OpenVisionLab 프로젝트 파일은 준비된 DLL만 참조합니다.

## 1. Required DLL Layout

필수 런타임 DLL은 저장소 내부 `dll` 폴더에 포함합니다.

```text
dll\
  System.Windows.Controls.WpfPropertyGrid.dll
  System.Windows.Controls.WpfPropertyGrid.xml
  Library-Noah\
    Lib.Common.dll
    Lib.OpenCV.dll
    Lib.OpenCV.Blob.dll
    OpenCvSharp.dll
    OpenCvSharp.Blob.dll
    OpenCvSharp.Extensions.dll
  OpenCVSharp\
    OpenCvSharpExtern.dll
```

`Directory.Build.props`는 공통 DLL 경로를 제공합니다.

- `OpenVisionLabDllRoot`: `dll\`
- `OpenVisionLabLibraryNoahDllRoot`: `dll\Library-Noah\`
- `OpenVisionLabOpenCvSharpDllRoot`: `dll\OpenCVSharp\`

## 2. Build Policy

기본 빌드는 외부 소스 루트 없이 수행되어야 합니다.

```powershell
dotnet build OpenVisionLab.sln -c Debug -p:Platform=x64
```

사전 점검은 저장소 내부 DLL 누락 여부를 확인합니다.

```powershell
powershell -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
```

`RunVisionPlatformPrecheck.ps1`도 같은 DLL 점검을 빌드 전에 실행합니다.

## 3. Library-Noah Policy

OpenVisionLab은 다음 Library-Noah 산출물을 DLL로 참조합니다.

- `Lib.Common.dll`
- `Lib.OpenCV.dll`
- `Lib.OpenCV.Blob.dll`
- `OpenCvSharp*.dll`

Library-Noah 소스를 수정해야 할 때의 절차:

1. 별도 Library-Noah 저장소에서 수정하고 단독 빌드를 통과시킨다.
2. 위 DLL을 `dll\Library-Noah\`에 복사한다.
3. OpenVisionLab 빌드와 플랫폼 프리체크를 통과시킨다.
4. 변경 사유와 DLL 갱신 사실을 OpenVisionLab 커밋/PR 설명에 남긴다.

## 4. WPG-CUSTOM Policy

OpenVisionLab은 WPG-CUSTOM 소스 프로젝트를 솔루션에 포함하지 않습니다. 프로퍼티그리드는 다음 준비된 DLL을 사용합니다.

- `dll\System.Windows.Controls.WpfPropertyGrid.dll`
- `dll\System.Windows.Controls.WpfPropertyGrid.xml`

WPG-CUSTOM 소스를 수정해야 할 때의 절차:

1. 별도 WPG-CUSTOM 저장소에서 수정하고 DLL을 생성한다.
2. 생성된 DLL/XML을 OpenVisionLab `dll\` 폴더에 복사한다.
3. PropertyGrid bridge 빌드와 UI/계약 프리체크를 통과시킨다.

## 5. Commit Policy

OpenVisionLab 커밋에는 다음을 명확히 남깁니다.

- OpenVisionLab 코드 변경 내용
- `dll\Library-Noah` 갱신 여부
- WPG PropertyGrid DLL 갱신 여부
- 실행한 검증 명령과 결과

외부 소스 저장소의 태그나 커밋은 DLL을 갱신한 경우에만 기록합니다. 일반 빌드 사용자는 `Library-Noah` 또는 `WPG-CUSTOM` 소스 클론이 필요하지 않습니다.

## 6. Risk

남은 관리 포인트:

- DLL이 누락되면 빌드가 실패하므로 `tools\TestExternalReferences.ps1`를 먼저 실행한다.
- DLL만 교체해도 공개 API가 바뀌면 OpenVisionLab 컴파일/런타임이 깨질 수 있으므로 플랫폼 프리체크를 필수로 수행한다.
- OpenCvSharp native runtime (`OpenCvSharpExtern.dll`) is shared from `dll\OpenCVSharp\`.
- Do not add `dll\Library-Noah\OpenCvSharpExtern.dll` again.
