# OpenVisionLab External Reference Policy

Updated: 2026-06-16

OpenVisionLab은 현재 두 개의 외부 소스 루트를 참조합니다.

- `Library-Noah`
- `WPG-CUSTOM`

이 문서는 외부 참조를 dev 환경에서 안정적으로 관리하고, 다른 PC에서 빌드할 때 발생할 수 있는 경로 문제를 줄이기 위한 기준입니다.

## 1. Recommended Folder Layout

권장 구조:

```text
C:\Git\
  ├── OpenVisionLab_Dev\
  ├── Library-Noah\
  └── WPG-CUSTOM\
```

기본 MSBuild 경로는 OpenVisionLab 기준 상위 폴더의 형제 디렉토리를 가정합니다.

```text
$(MSBuildProjectDirectory)\..\Library-Noah
$(MSBuildProjectDirectory)\..\WPG-CUSTOM
```

## 2. Library-Noah Policy

Library-Noah는 OpenVisionLab이 사용하는 공통/비전 알고리즘 라이브러리의 외부 소스 루트입니다.

대상 프로젝트:

- `Lib.Common`
- `Lib.OpenCV`
- `Lib.OpenCV.Blob`

정책:

1. OpenVisionLab 내부 `Library\Lib.*`를 직접 장기 운영하지 않는다.
2. 수정이 필요하면 먼저 Library-Noah에 반영한다.
3. Library-Noah 단독 빌드를 확인한다.
4. OpenVisionLab은 확인된 Library-Noah 버전을 참조한다.
5. `Lib.OpenCV.Blob`의 CVBlob DLL 버전은 유지한다. 버전 업그레이드는 별도 결정 없이는 하지 않는다.
6. 알고리즘 리팩토링은 동작/속도 호환성을 우선한다.

검증 기준:

- Library-Noah 단독 Build OK
- OpenVisionLab Build OK
- Sample Catalog Required rows OK
- Algorithm Contract OK
- Tool Result Contract OK

## 3. WPG-CUSTOM Policy

WPG-CUSTOM은 WPF PropertyGrid 커스텀 소스입니다.

목표:

- Tool Form과 Pipeline PropertyGrid가 동일한 editor 철학을 공유한다.
- Threshold, Range, ComboBox, Layer 선택, TrackBar editor를 공통화한다.
- OpenVisionLab 쪽 코드는 adapter/bridge 중심으로 유지한다.

정책:

1. WPG-CUSTOM 원본 소스 수정은 별도 변경으로 관리한다.
2. OpenVisionLab에는 bridge/adapter만 둔다.
3. 공통 editor 동작은 WPG-CUSTOM 또는 WpfPropertyGridBridge에서 일관되게 노출한다.
4. Pipeline과 Tool Form이 같은 editor behavior를 쓰도록 smoke contract를 둔다.
5. 디자이너에서 열리는 WinForms Form은 가능한 한 designer-friendly하게 유지한다.

검증 기준:

- `pipeline_property_grid_contract_check`
- `threshold_form`
- 주요 Tool Form smoke
- WPG editor row hide/visibility contract

## 4. Local Override

외부 라이브러리 경로가 기본 구조와 다르면 MSBuild 속성으로 지정합니다.

```powershell
dotnet build OpenVisionLab.csproj `
  -p:LibraryNoahSourceRoot="D:\Work\Library-Noah" `
  -p:WpgCustomSourceRoot="D:\Work\WPG-CUSTOM"
```

WPG 소스 빌드를 건너뛰고 준비된 DLL을 사용하는 경우:

```powershell
dotnet build OpenVisionLab.csproj -p:WpgCustomBuildEnabled=false
```

## 5. Commit Policy

OpenVisionLab commit에는 다음을 명확히 남깁니다.

- OpenVisionLab 코드 변경
- Library-Noah 변경 필요 여부
- WPG-CUSTOM 변경 필요 여부
- 사용한 외부 참조 버전 또는 commit
- 검증한 precheck report 경로

예:

```text
Title: Stabilize pipeline UI and external recipe validation

Body:
- Clarified Main/Pipeline/Threshold UI contracts and quiet UI precheck.
- Verified with RunUiPrecheck and RunVisionPlatformPrecheck.
- Requires Library-Noah at <commit/tag> and WPG-CUSTOM at <commit/tag>.
```

## 6. Release/Package Direction

내부 개발 단계:

- Source reference를 유지한다.
- `C:\Git\OpenVisionLab_Dev`, `C:\Git\Library-Noah`, `C:\Git\WPG-CUSTOM` 구조를 권장한다.

사내 배포 단계:

- Library-Noah와 WPG-CUSTOM을 tag 또는 package 기준으로 고정한다.
- OpenVisionLab은 고정된 버전을 참조한다.
- README에 필요한 외부 루트를 명시한다.

외부 공개 단계:

- 외부 참조가 없는 clone/build 경험을 제공해야 한다.
- 가능한 경우 NuGet 또는 binary package로 고정한다.
- 샘플 이미지와 sample catalog는 최소 세트를 포함한다.

## 7. Risk

현재 남은 리스크:

- 다른 PC에서 `Library-Noah` 또는 `WPG-CUSTOM` 경로가 없으면 빌드 실패 가능.
- WPG editor 구현이 원본 소스와 bridge 사이에 나뉘어 있어 변경 책임 경계가 흐릴 수 있음.
- CVBlob DLL은 버전 고정이 필요하므로 자동 업데이트 대상이 아님.

관리 방법:

- README와 이 문서에 경로/속성 override를 유지한다.
- precheck report에 외부 참조 상태를 남긴다.
- WPG editor 변경은 `pipeline_property_grid_contract_check`를 필수로 돌린다.
