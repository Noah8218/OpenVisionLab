# OpenVisionLab 문서 시작점

Updated: 2026-09-15 KST

이 파일은 사람과 도구가 함께 쓰는 단일 문서 진입점입니다. 전체 문서를 시간순으로
읽지 말고 아래 공통 4개 문서와 작업별 경로만 읽습니다.

## Start Here

1. [`AGENTS.md`](../AGENTS.md) — 저장소 규칙, 제품 범위, 변경·검증 계약
2. [`OPENVISIONLAB_CURRENT_HANDOFF.md`](admin/OPENVISIONLAB_CURRENT_HANDOFF.md) — 현재 상태와 실제 다음 우선순위
3. [`OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`](roadmap/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md) — 제품 정체성과 화면 책임
4. [`OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`](contracts/openvisionlab/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md) — 회귀시키면 안 되는 동작

선행개발 전 전체 점검 결과와 처음 접하는 개발자를 위한 판독 순서는
[`OPENVISIONLAB_PREDEVELOPMENT_PRIORITIES_1_TO_5_20260915.md`](reports/OPENVISIONLAB_PREDEVELOPMENT_PRIORITIES_1_TO_5_20260915.md)에
정리되어 있습니다.

기계 판독 경로는 [`LLM_DOCUMENT_INDEX.json`](LLM_DOCUMENT_INDEX.json), 상세 문서
등록부는 [`OPENVISIONLAB_DOCUMENTATION_MAP.md`](admin/OPENVISIONLAB_DOCUMENTATION_MAP.md)입니다.

## Visual Studio에서 열고 실행하기

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

솔루션에는 제품 앱 1개, 제품 라이브러리 12개, 검증 도구 3개가 있습니다. 정확한
프로젝트 역할과 `ProjectReference` 방향은
[`CODEBASE_STRUCTURE.md`](admin/CODEBASE_STRUCTURE.md)의 `Start Here` 표를 사용합니다.

## 제품 코드와 검증 코드 구분

| 목적 | 먼저 읽을 위치 |
| --- | --- |
| 제품 시작과 Shell 조합 | `Program.cs` → `OpenVisionLabApplication.cs` → `OpenVisionShellHostView.xaml(.cs)` |
| Recipe/Pipeline 실행 | `RecipeCommandSurface.cs` → `OpenVisionPipelineReviewDocument.cs` → `VisionPipelineExecutionService.cs` |
| Tool 편집/Preview | `UI/VisionTest/Wpf/ToolViews` → 해당 Presenter/Controller → Pipeline Tool |
| 화면 없는 계약 검증 | `tools/VisionRecipeRunnerSmoke`의 해당 `*Contract.cs` |
| 실제 WPF 화면 검증 | `tools/PipelineViewerScreenshotSmoke`와 `tools/RunUiPrecheck.ps1` |
| 제품 EXE smoke | `tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs` |

검증 도구의 큰 `Program.cs`는 제품 정책 소유자가 아닙니다. 제품 동작은 `src/`의
기존 concrete owner에서 수정하고, 가장 가까운 contract와 필요한 WPF smoke로
확인합니다. 생성 로그·스크린샷·임시 데이터는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev`에 둡니다.

## 작업별 경로

| 작업 | 공통 4개 다음에 읽을 문서 |
| --- | --- |
| UI/UX 또는 WPF 상태 | `WPF_UI_UX_RULES.md`(전역) → `docs/runbooks/UI_SCREENSHOT_SMOKE.md` |
| Recipe/Pipeline/Tool | `docs/reports/OPENVISIONLAB_RECIPE_EXECUTION_STRUCTURE_20260908.md` |
| ImageCanvas/ROI/OpenGL | `docs/reports/OPENVISIONLAB_WPF_VIEW_MVVM_PORTABILITY_AUDIT_20260913.md` → 관련 ImageCanvas owner 보고서 |
| 외부 DLL/재배포 | `docs/contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_REFERENCE_POLICY.md` → `dll/OpenVisionLab-Vision-SDK/sdk-manifest.json` |
| 사용자 매뉴얼/Learn | `docs/manual/README.md` → `docs/learn/README.md` |
| 상용 도구 비교 | `docs/roadmap/OPENVISIONLAB_COMMERCIAL_VIDEO_DEVELOPMENT_BACKLOG_20260727.md` |
| 과거 P 번호/결정 | `docs/reports/OPENVISIONLAB_COMPLETED_TRACKER.md` → `docs/admin/archive`에서 검색 |

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

1. 현재 상태는 `OPENVISIONLAB_CURRENT_HANDOFF.md` 한 곳에만 기록합니다.
2. 구조 owner와 읽기 순서는 `CODEBASE_STRUCTURE.md` 한 곳에만 기록합니다.
3. 완료 과정·명령 원문·스크린샷 목록은 `docs/reports` 또는 D 드라이브 증거에 둡니다.
4. 누적 chronology는 `docs/admin/archive`에 보존하고 활성 문서에 다시 붙이지 않습니다.
5. 새 보고서는 실제로 재사용할 결정·증거가 있을 때만 만들고, 색인에는 필요한 route에만 넣습니다.
6. 루트의 호환 redirect는 외부 링크 조사 없이 삭제하지 않습니다.

검증:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestDocumentationIndex.ps1
rg -n "<검색어>" AGENTS.md docs
rg --files docs | rg "<파일명 일부>"
```

2026-09-14 이전의 비대해진 진입점 원문은
[`DOCUMENTATION_ENTRYPOINT_HISTORY_20260914.md`](admin/archive/DOCUMENTATION_ENTRYPOINT_HISTORY_20260914.md)에
보존했습니다.
