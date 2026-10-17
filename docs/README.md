# OpenVisionLab 문서 시작점

Updated: 2026-10-13 KST

이 파일은 사용자 문서와 소스 탐색을 연결하는 단일 문서 진입점입니다.

## Start Here

1. [기여 규칙](../AGENTS.md)을 확인합니다.
2. [CODEBASE_STRUCTURE.md](admin/CODEBASE_STRUCTURE.md)의 Start Here에서 실행 프로젝트와 기능 owner를 찾습니다.
3. [안정 계약](contracts/openvisionlab/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md)의 General Rules와 변경 경로의 관련 절을 읽습니다.

기계 판독 경로는 [LLM_DOCUMENT_INDEX.json](LLM_DOCUMENT_INDEX.json)입니다.
실제 동작과 의존성은 소스·프로젝트·스키마가 원본이며 문서 지도는 검색 진입점입니다.

## Visual Studio에서 열고 실행하기

다른 PC에서는 GitHub Desktop으로 `https://github.com/Noah8218/OpenVisionLab.git`의
`main`을 짧은 경로에 클론합니다. Visual Studio의 `.NET 데스크톱 개발` 워크로드와
.NET 8 SDK/Desktop Runtime을 준비하고 개발 터미널의 `git --version`도 확인합니다.
환경 검증은 `tools/VerifySourceBuild.ps1`, 설치 조건은 루트 `README.md`가 안내합니다.
저장소 안의 DLL·연동 NuGet 패키지·잠금 파일을 함께 사용하며 다른 프로젝트 소스는 필요하지 않습니다.
패키지를 바꿀 때는 일반 restore로 잠금 파일을 갱신한 뒤 `--locked-mode`로 검증합니다.

사용 중인 Recipe·이미지·개인 설정은 소스와 별도로 보존합니다. 필요한 데이터는 `OPENVISIONLAB_DATA_ROOT` 및 외부 이미지 경로를 확인해 이관합니다.

| 항목 | 경로 또는 값 |
| --- | --- |
| 열 솔루션 | `OpenVisionLab.sln` |
| 시작 프로젝트 | `src/OpenVisionLab/OpenVisionLab.csproj` |
| 진입점 | `src/OpenVisionLab/Program.cs` |
| 부트스트랩 | `src/OpenVisionLab/App/Bootstrap/OpenVisionLabApplication.cs` |
| 지원 플랫폼 | Windows x64 (`Any CPU` 구성도 앱은 x64로 빌드) |
| 기본 실행 파일 | `bin/Debug/OpenVisionLab.exe` |

```powershell
dotnet restore OpenVisionLab.sln
dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU" --no-restore
& .\bin\Debug\OpenVisionLab.exe
```

위 명령은 소스 개발 시작 경로입니다. UI 검사는 변경을 포함한 빌드 identity를 기록하고 기존 사용자 데이터를 격리합니다.
프로젝트·출력·의존성은 솔루션과 `.csproj`가 원본이며, 탐색은
[`CODEBASE_STRUCTURE.md`](admin/CODEBASE_STRUCTURE.md)의 `Start Here`에서 시작합니다.

## 제품 코드와 검증 코드 구분

책임별 코드 탐색과 검증 진입점은 [owner 지도](admin/CODEBASE_STRUCTURE.md) 한 곳에서 찾습니다.
검증 도구의 `Program.cs`는 제품 정책 소유자가 아닙니다. 제품 동작은 `src/`의
기존 concrete owner에서 수정하고, 가장 가까운 contract와 필요한 WPF smoke로
확인합니다. 생성 로그·스크린샷·임시 데이터는 소스와 사용자 데이터에서 분리한 검사 출력 폴더에 둡니다.

## 작업별 경로

| 작업 | 기본 읽기 다음에 필요한 범위만 찾을 문서 |
| --- | --- |
| UI/UX 또는 WPF 상태 | [안정 계약](contracts/openvisionlab/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md) → [UI smoke](runbooks/UI_SCREENSHOT_SMOKE.md) → 해당 View/owner |
| Recipe/Pipeline/Tool | 안정 계약의 편집·실행·라우팅·저장 절 → owner 지도의 Recipe/Pipeline 읽기 순서 |
| ImageCanvas/ROI/OpenGL | owner 지도의 ImageCanvas 경로 → [외부 consumer 계약](contracts/openvisionlab/OPENVISIONLAB_IMAGECANVAS_EXTERNAL_CONSUMER_MANIFEST.json) |
| 외부 DLL/재배포 | [외부 참조 정책](contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_REFERENCE_POLICY.md) → `dll/OpenVisionLab-Vision-SDK/sdk-manifest.json` |
| 사용자 매뉴얼/Learn | [매뉴얼](manual/README.md) → [Learn](learn/README.md) |

세부 경로는 `LLM_DOCUMENT_INDEX.json`의 같은 목적 route를 따릅니다.

## DLL 사용 경계

- 제품 EXE는 저장소의 manifest 검증 외부 DLL과 프로젝트 라이브러리를 함께 사용합니다.
- `OpenVisionLab.ImageCanvas.dll`도 단일 DLL만 복사하는 계약이 아닙니다. 최소한 그
  DLL의 선언된 managed/native dependency closure와 .NET 8 Windows Desktop runtime이
  함께 필요합니다.
- 실제 파일 목록·해시·허용 정책은
  `docs/contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json`과
  `dll/OpenVisionLab-Vision-SDK/sdk-manifest.json`이 권위입니다.
- 검증 없이 “DLL 하나만 분리 가능” 또는 “재사용 가능”이라고 문서화하지 않습니다.

## 문서 유지 규칙

1. owner와 읽기 순서는 `admin/CODEBASE_STRUCTURE.md` 한 곳에서 찾습니다.
2. 코드·설정·스키마가 원본인 값은 해당 경로와 검증 명령을 연결합니다.
3. 동작 계약과 사용자 안내는 변경된 소스와 함께 갱신합니다.
4. 공개 링크는 보존된 canonical 문서로 연결하고 호환 redirect의 목적지를 확인합니다.
5. 테스트 출력과 개인 데이터를 문서 원천 및 공개 샘플에 섞지 않습니다.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/TestDocumentationIndex.ps1
rg -n "<검색어>" docs/contracts src tools
```
