# OpenVisionLab 공개 문서 / Public Documentation

Updated: 2026-10-26 KST

사용자 안내, 공개 소스 빌드와 제품 인터페이스 참조의 시작점입니다.

## Source Build

[빌드·설치 안내](runbooks/BUILD.md)에 따라 공개 저장소를 짧은 로컬 경로에
클론하고 `OpenVisionLab.sln`을 엽니다. 시작 프로젝트는
`src/OpenVisionLab/OpenVisionLab.csproj`, 기본 구성은 `Debug` / `Any CPU`이며
Windows x64 WPF 앱을 생성합니다. Visual Studio 2022의 .NET 데스크톱 개발
워크로드 또는 지원되는 .NET 8/9 SDK와 .NET 8 Desktop Runtime이 필요합니다.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/VerifySourceBuild.ps1
```

Git은 빌드 식별에 필요합니다. 배포 DLL과 잠긴 NuGet 참조를 함께 사용하며
다른 저장소의 제품 소스는 필요하지 않습니다. 기존 Recipe·이미지·설정은
소스 빌드로 복사되지 않습니다. 필요한 데이터는 `OPENVISIONLAB_DATA_ROOT` 또는
명시적인 이미지 경로로 연결합니다.

## User Guides And Interfaces

| 목적 | 안내 |
| --- | --- |
| 설치·실행 | [English README](../README.md), [한국어 README](../README.ko.md), [빌드 안내](runbooks/BUILD.md) |
| 한국어·영어 오프라인 Guide | [사용자 매뉴얼](manual/README.md) |
| Tool 개념과 실습 | [Learn](learn/README.md) |
| 공개 예제 이미지·Pipeline | [공개 샘플](samples/public/README.md), [제품 분야별 샘플](samples/public/product/README.md) |
| XML Recipe 작성 | [XML 작성 안내](contracts/openvisionlab/OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md), [Pipeline XML 계약](contracts/vision/VISION_PIPELINE_LLM_RECIPE_CONTRACT.md) |
| Image Compare | [별도 실행 안내](runbooks/IMAGE_COMPARE_STANDALONE.md) |
| DLL 출처·재배포 | [외부 참조 정책](contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_REFERENCE_POLICY.md), [DLL 매니페스트](contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json), [NOTICE](../NOTICE) |

매뉴얼과 Learn의 캡처는 사용자 학습에 필요한 제품 자료입니다. 내부 개발 기록,
회귀 테스트, 화면 검수 도구와 검증 로그는 Dev에서 관리합니다.
문서 경로 색인은 [LLM_DOCUMENT_INDEX.json](LLM_DOCUMENT_INDEX.json)입니다.

## Public Build And Distribution Verification

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/VerifyReleaseCandidate.ps1 -SkipLaunch
powershell -NoProfile -ExecutionPolicy Bypass -File tools/TestDocumentationIndex.ps1
```

전체 게이트는 깨끗한 Git 상태에서 잠금 복원, Debug/Release 빌드, 공개 입력과
두 언어 Guide의 준비 검사, DLL·NOTICE·공개 자산 정책, 공개 카탈로그 실행,
깨끗한 Release publish와 배포 패키지 검증을 수행합니다. `-SkipLaunch`는 CI처럼
GUI 실행 검증을 별도로 수행할 때만 사용합니다. 이 명령은 GitHub Release를
게시하거나 제품을 설치하지 않습니다.

`OpenVisionLab.ImageCanvas.dll` 하나만으로 독립 실행할 수 있다고 가정하지 마세요.
필요한 managed/native 의존성과 .NET Desktop Runtime의 전체 범위는 외부 참조
정책과 실제 프로젝트 참조를 확인합니다. LICENSE·NOTICE와 DLL 해시를 보존합니다.
