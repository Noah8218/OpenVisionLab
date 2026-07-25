# OpenVisionLab Repository Reorganization & MVVM Refactor Design (2026-07-24)

## 0) 목적

- `0/1/2/3` 같은 접두어 폴더를 제거한 상태를 기준으로 정리/안정화
- docs/config/md 산재를 제거해 탐색성과 소유권 경계를 안정화
- MVVM 적용을 “한 번에 큰 리팩토링”이 아니라, 동작 보존 기반의 작은 슬라이스로 진행

## 1) 현재 상태 점검(현재 소스 기준)

- 루트 번호형 폴더 이름은 소스에서 제거됨(활성 경로 기준)
- `docs/`는 카테고리 하위 폴더로 재배치 완료
  - `docs/admin`, `docs/contracts`, `docs/analysis`, `docs/reports`, `docs/research`, `docs/roadmap`, `docs/learn`, `docs/runbooks`, `docs/assets`, `docs/evidence`
- 루트 docs는 안내 스텁 중심으로 축소됨(원본 본문은 하위 폴더)
- 구조상 동작 영향이 큰 고우선 후보는 UI/쉘/런타임 오케스트레이션 파일에 집중

## 2) 구조 원칙(강제 규칙)

1. Preview/Run/레이어 변경은 기존 동작 유지(명시 액션)
2. 알고리즘 추가/정정 없이 코드 경계만 정리
3. 하위 호환성 유지: 기존 XML/기존 설정 기본값은 유지
4. 실패 없이 컴파일 유지가 우선(동작 리팩토링보다 먼저 구조 안정화)
5. 변경 단위는 하나의 책임으로 제한: `View`, `ViewModel`, `Coordinator/Service`, `Converter` 경계 분리

## 3) 우선순위(단계)

### P0 — 실행 경로 분리 (현재 제일 우선)
- `tools/OpenVisionLabDirectSmokeRunner.cs`는 앱 런타임 본문에서 분리 대상
- 정상 실행 경로는 제품 부팅만 담당, smoke는 별도 빌드 플래그/도구 실행체로 이동
- 이유: 유지보수성 및 코드 정합성 확보, 의도치 않은 런타임 영향 감소

### P1 — 핵심 쉘/리시피 오케스트레이션 분리
- `UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs`
- `UI/Menu/Wpf/OpenVisionShellHostView.xaml.cs`
- 목표: 명령 라우팅(실행/전환), 상태(필드), UI 트리거를 분리
- 방식: `Command/State/Formatting`로 최소 2~3개 partial 또는 보조 클래스로 나누기

### P2 — 리뷰/도큐먼트 경계 정리
- `UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs`
- `UI/Menu/Wpf/OpenVisionPipelineReviewView.xaml.cs`
- 목표: 문서/실행 상태 vs UI 표시 상태 분리

### P3 — 도메인 매퍼 정리
- `UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelineStepPropertyMapper.cs`
- 목표: Tool family별 Adapter 분할(Line/Blob/Matching/Threshold/Geometry 등)

### P4 — Learn 윈도우 분리
- `UI/VisionTest/Wpf/Learn/OpenVisionLearnWindow.xaml.cs`
- 목표: 애니메이션/콘텐츠/네비게이션 상태를 ViewModel + 서비스로 분리

## 4) 현재 파일별 리팩토링 후보(우선 처리 리스트)

- `UI/VisionTest/Wpf/Learn/OpenVisionLearnWindow.xaml.cs`
- `UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs`
- `UI/Menu/Wpf/OpenVisionShellHostView.xaml.cs`
- `UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelineStepPropertyMapper.cs`
- `UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs`

## 5) 적용 원칙 및 중단 기준

- 단일 슬라이스당 “기능 동작 변화 없음”으로 보류 검증
- 각 슬라이스 완료 후 최소 확인:
  - `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"`
  - 대상 뷰/기능 smoke smoke-like command path를 통한 재실행
  - git diff 크기 증가가 책임 경계보다 과도하지 않음 (UI 행위 유지)
- 동작 차이 발견 시 이전 commit으로 되돌리고 단위를 더 작게 쪼갬

## 6) 문서 이동 규칙(완료/유지)

- 번호형 파일 이동 흔적은 스텁으로만 남김(루트 탐색용)
- 완전 본문은 하위 폴더에서만 유지
- 중요한 계약/근거 문서는 `docs/contracts/*` 또는 `docs/reports/*`, 운영/운영전술은 `docs/admin/*`, 튜토리얼은 `docs/learn/*`
