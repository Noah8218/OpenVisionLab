# OpenVisionLab OVL-08 PropertyGrid 정책 분리

작성일: 2026-09-07 KST

## 의미

PropertyGrid 정책 분리는 공용 WPF bridge가 `Threshold`, `Canny`, `Affine` 같은
OpenVisionLab 업무 속성명을 직접 알고 표시 규칙을 결정하지 않게 하는 작업이다.
툴별 정책은 앱이 소유하고, bridge는 `PropertyGrid.Abstractions`로 전달받은 일반
판정 결과를 행의 시각 표현에만 적용한다.

예를 들어 `FIND_ANGLE`은 Matching 정책에서 조건부 하위 행으로 분류된다. 이전에는
같은 분류 목록과 Affine 반사 규칙이 bridge 내부에 중복되어 있었다. 이제는
`PropertyGridToolPolicy.IsChildParameterProperty`가 그 판정을 만들고,
`PropertyGridDisplayOptions.ChildParameterPredicate`로 전달한다. bridge는 선택된
객체와 속성명을 predicate에 넘긴 뒤 true일 때만 child-row accent/indent를 적용한다.

## 변경 범위

- `src/OpenVisionLab/Common/PropertyGridToolPolicy.cs`
  - 기존 bridge의 child 속성 46개와 Affine 조건을 앱 정책 owner로 이동했다.
  - 기존 문자열 비교와 `ShowAdvancedSettings`/`UseDetectedSourcePoints` 존재 조건을
    유지했다.
- `src/Libraries/PropertyGrid.Abstractions/PropertyGridContracts.cs`
  - `PropertyGridDisplayOptions.ChildParameterPredicate`를 추가했다.
  - WPF 타입이나 OpenVisionLab Tool 타입을 abstraction에 추가하지 않았다.
- `src/Libraries/WpfPropertyGridBridge/WpfPropertyGridAdapter.cs`
  - 업무 속성명 목록과 Affine 반사 규칙을 제거했다.
  - 전달받은 predicate 결과만 `NormalizeChildParameterRow`의 시각 처리에 사용한다.
  - `IsBrowsable`, RangeEditor companion descriptor 유지, editor 등록, 정렬, 키보드
    commit 흐름은 변경하지 않았다.
- `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/PropertyGrid/VisionToolPropertyGridHost.cs`
  - 앱 Tool host가 `PropertyGridToolPolicy`를 ToolForm 옵션에 연결한다.
- `tools/PipelineViewerScreenshotSmoke/Program.cs`
  - 직접 생성하는 Matching PropertyGrid smoke에도 동일 정책을 연결했다.
  - Matching 정책 분류와 child-row accent를 검사한다.

## 구조 증거

이전 호출 경로는 `WpfPropertyGridAdapter` 내부의 정적 속성명 목록/반사 검사에서
행 스타일을 결정했다. 현재 호출 경로는 다음과 같다.

`VisionToolPropertyGridHost`
→ `PropertyGridToolPolicy.IsChildParameterProperty`
→ `PropertyGridDisplayOptions.ChildParameterPredicate`
→ `WpfPropertyGridAdapter.IsChildParameterProperty`
→ `NormalizeChildParameterRow`

공용 bridge 프로젝트는 계속 `PropertyGrid.Abstractions`와 Localization만 참조하며
`OpenVisionLab` 앱 프로젝트를 참조하지 않는다. bridge 파일에는 `THRESHOLD_TYPES`,
`CANNY_LOW`, `ShowAdvancedSettings`, `UseDetectedSourcePoints` 업무 정책 문자열이
남아 있지 않다. 이전 bridge 목록과 새 앱 목록의 46개 분류 값은 동일하다.

## Refactor proof

- Current responsibility owner: bridge의 정적 child 속성 목록과 Affine 반사 검사.
- Current call path: generated row → bridge 내부 `IsChildParameterProperty`.
- Current dependency direction: 공용 bridge가 앱 Tool 개념을 속성명/반사 규칙으로 암묵적으로 소유.
- Current state/data owner: 선택된 Property 객체는 앱이 소유하지만 bridge가 분류 상태를 별도로 결정.
- Intended responsibility owner: `OpenVisionLab.Common.PropertyGridToolPolicy`.
- Intended call path: host → app predicate → abstraction option → bridge row renderer.
- Intended dependency direction: 앱 → abstraction → bridge; bridge → 앱 참조 없음.
- Intended state/data owner: Property 모델과 앱 정책이 소유하고 bridge는 렌더링 결과만 소비.
- Structural proof: 업무 목록·Affine 반사 규칙 삭제, callback 연결, old/new 46개 값 동등성
  검사(`policy-parity.log`) 및 대표 소비 화면 smoke 통과.

## 검증

- `dotnet build src/Libraries/WpfPropertyGridBridge/WpfPropertyGridBridge.csproj -c Debug -p:Platform=x64 --no-restore -m:1 -nr:false`
  - 0 warnings / 0 errors.
- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Debug -p:Platform=x64 --no-restore -m:1 -nr:false`
  - 0 warnings / 0 errors.
- `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform=x64 --no-restore -m:1 -nr:false`
  - 0 errors. 기존 dirty smoke 코드의 `Program.cs:10717` nullable warning 1개가
    남아 있으며 이번 정책 변경 라인에서 발생한 warning은 아니다.
- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Release -p:Platform=x64 --no-restore -m:1 -nr:false`
  및 `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Release -p:Platform=x64 --no-restore -m:1 -nr:false`
  - Release 앱/runner 빌드 완료. 앱은 0 warnings / 0 errors이며 smoke runner는
    기존 dirty 코드의 nullable warning 1개와 함께 0 errors다.
- `wpf_property_grid_matching_combo`
  - Matching child 정책 분류(`FIND_ANGLE=true`, `SCORE_MIN=false`), child-row
    accent, ComboBox popup/template, 단일 선택 텍스트, RangeEditor Min/Max 폭을
    검사하고 PASS했다.
- Release runner의 `wpf_property_grid_matching_combo`도 같은 정책·편집기 검사를
  PASS했다.
- `wpf_shell_host_matching_tool`, `wpf_shell_host_affine_transform_tool`,
  `wpf_shell_host_contour_tool`, `wpf_shell_host_edge_based_matching_tool`,
  `wpf_shell_host_blob_tool`, `wpf_shell_host_line_measure_tool`
  - 현재 빌드의 대표 PropertyGrid 소비 화면이 모두 PASS했다.
- 정적 확인: bridge의 업무 정책 목록 제거, 앱 정책 callback 연결, 이전 46개
  분류 값 보존을 확인했다.
- 모든 smoke 증거는 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl08-propertygrid-policy-20260907`에 저장했다.
  현재 Windows는 독립 모니터 1개(`\\.\DISPLAY1`, bounds `0,0,1920x1080`,
  working area `0,0,1920x1032`)로 보고되어 창 배치 규칙에 따라 화면을 변경하지
  않았다.

## 범위 경계

이번 완료는 child-row 정책의 책임 이동과 대표 화면 회귀에 한정한다. PropertyGrid의
전체 속성 표시·순서·숨김 정책은 기존 앱 binder와 모델 계약을 보존했다. Dark/Compact
조합, 125/150/175/200% DPI, 전체 지원 테마·레이아웃 조합, 장시간/다중 모니터 UI
검증은 이번 실행에서 하지 않았으며 별도 WPF 상태 게이트로 남긴다. Recipe XML,
Preview/Run, output layer, ROI, persistence 동작을 이 변경이 새로 증명하지는 않는다.

Status: Complete
Scope: 공용 WPF PropertyGrid bridge의 업무 속성명 기반 child-row 정책을 앱 정책 callback으로 분리.
Acceptance criteria: bridge의 Tool 정책 제거, 앱→abstraction→bridge 전달 경로, 기존 46개 분류 보존, 대표 PropertyGrid 소비 화면 통과.
Verification: 위 Debug/Release x64 빌드, 정적 구조 확인, Matching direct PropertyGrid와 Matching·Affine·Contour·EdgeBasedMatching·Blob·Line WPF consumer smoke.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl08-propertygrid-policy-20260907`.
Boundary / next dependency: 전체 테마·레이아웃·DPI 행렬은 미검증이며 별도 UI 게이트다. `C:\Git\2D\Original`은 변경하지 않았다.
