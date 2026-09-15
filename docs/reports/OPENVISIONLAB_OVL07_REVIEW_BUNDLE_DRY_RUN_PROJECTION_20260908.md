# OpenVisionLab OVL-07 Review bundle dry-run result/status projection owner 분리 — 2026-09-08

## 의미와 범위

이번 실행은 review bundle dry-run의 결과를 Shell StatusText와 반환값으로
투영하는 책임을 OpenVisionRecipeReviewBundleDryRunProjectionOwner로
분리했다. owner는 무결성 검사 성공 여부와 XML 검토 가능 여부만 받아
기존 한국어/영어 상태 문구와 성공값을 계산한다.

LoadReviewBundleForDryRun의 Inspector 호출, 무결성/경로 보고서와
placeholder 갱신, loaded inspection 보관, XML validation, review 화면
전환은 기존 LLM workflow에 남겼다. dry-run은 계속 review 화면에만 XML을
로드하며 Import/Preview/Run을 실행하지 않는다. Inspector가 실패할 때만
기존 반환값 false를 유지하고, bundle 무결성은 성공했지만 XML/의존성
검토가 NG인 경우 기존처럼 true를 유지한다. Recipe/XML schema, Layer
routing, explicit Preview/Run 계약은 변경하지 않았다.

## 구조 변화

| 항목 | 이전 | 현재 |
| --- | --- | --- |
| Inspector 실패 status/return | LLM workflow inline formatting | OpenVisionRecipeReviewBundleDryRunProjectionOwner.Project(false, false) |
| Inspector 성공 + XML 검토 가능 status | LLM workflow inline formatting | Project(true, true) |
| Inspector 성공 + XML/의존성 NG status | LLM workflow inline formatting | Project(true, false) |
| Inspector/state/validation/review navigation | LoadReviewBundleForDryRun | 변경 없음 |
| XML/storage/bundle mutation | 기존 Inspector/UseCase 경로 | 변경 없음 |

호출 경로는 다음과 같다.

Shell command guard → LoadReviewBundleForDryRun → ReviewBundleInspector /
ValidateLlmXmlDraftText → DryRunProjectionOwner.Project → Shell StatusText /
review tab

owner는 Window, UserControl, WPF type, Inspector, storage service나
mutable Shell state를 가지지 않는다. Projection은 Succeeded와
StatusText만 반환한다.

## 변경 파일

- src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeReviewBundleDryRunProjectionOwner.cs
  - dry-run 세 결과를 localized status와 기존 반환 의미로 투영하는
    Window-free concrete owner/projection을 추가했다.
- src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs
  - owner를 Shell dependency로 보유한다.
- src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.LlmXmlDraftWorkflow.cs
  - inline dry-run 상태 문구를 제거하고 Inspector/state/validation/review
    전환 뒤 owner를 호출한다.
- tools/VisionRecipeRunnerSmoke/Program.cs
  - --recipe-review-bundle-dry-run-projection-contract 무창 계약과
    usage를 추가했다.
- docs/reports/OPENVISIONLAB_OVL07_REVIEW_BUNDLE_DRY_RUN_PROJECTION_20260908.md,
  docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md,
  docs/LLM_DOCUMENT_INDEX.json
  - 완료 증거와 다음 단일 우선순위를 기록했다.

## 구조 증거

- owner는 internal sealed concrete non-partial이며 WPF/Inspector/
  validation/storage 참조가 없다.
- workflow에는 Project 호출이 Inspector 실패/성공 두 경로에 각각 하나씩
  있고, 성공 경로는 xmlReady를 전달한다.
- 기존 Inspector 검사, ClearLoadedReviewBundleContext, XML/path report,
  LlmXmlDraftText = inspection.PipelineXml, loaded inspection state,
  ValidateLlmXmlDraftText(false), openLlmXmlReview 호출은 workflow에
  그대로 남아 있다.
- Projection contract는 실패는 Succeeded=false, 무결성 성공/XML 준비는
  Succeeded=true, 무결성 성공/XML NG도 Succeeded=true라는 기존
  return contract와 세 localized status를 고정한다.
- 정적 결과:
  D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-review-bundle-dry-run-projection-20260908\static-review-bundle-dry-run-projection-check.log

## 검증

- x64 Debug/Release OpenVisionLab app과 VisionRecipeRunnerSmoke
  빌드는 경고 0개·오류 0개로 통과했다.
- x64 Debug/Release PipelineViewerScreenshotSmoke 빌드는 오류 0개로
  통과했다. 기존 Program.cs:10722 CS8600 경고 1개는 양 구성에서
  그대로 발생했다.
- 새 --recipe-review-bundle-dry-run-projection-contract는 Debug/Release
  모두 PASS했다. 실패/성공/XML NG의 반환값과 localized status를 직접
  확인했다.
- 기존 workspace policy/lifecycle, Recipe Manager summary/Pipeline option,
  Pipeline lifecycle, Pipeline exchange projection 계약은 Debug/Release
  모두 PASS했다.
- OpenVisionLab.sln Debug/Any CPU build와 OpenVisionReadinessCheck
  Debug는 경고 0개·오류 0개 및 PASS였다.
- PipelineViewerScreenshotSmoke의
  wpf_shell_host_recipe_review_bundle_import는 Debug/Release 모두
  check=OK, layout=0|text=0|internal=0, 1600x900으로 통과했다.
  실제 review-bundle import 경로의 tamper rejection, relocation 후보,
  dry-run 검증과 Import/Preview/Run 무실행을 확인했다. Release 캡처:
  D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-review-bundle-dry-run-projection-20260908\smoke-Release-wpf_shell_host_recipe_review_bundle_import\wpf_shell_host_recipe_review_bundle_import.png
- 첫 Debug 빌드에서는 같은 메서드 범위의 지역 변수 이름 충돌
  CS0136이 발생했다. failureProjection/reviewProjection으로 이름을
  분리한 뒤 동일 빌드와 후속 검증을 다시 실행했고, 실패 로그는
  build-app-debug-initial-cs0136.log,
  build-runner-debug-initial-cs0136.log로 보존했다.
- 현재 모니터 토폴로지는 Windows가 보고한 단일 모니터
  \\.\DISPLAY2, bounds 0,0,1920,1080, working area
  0,0,1920,1032였다. 해당 smoke 프로세스는 종료되었고 잔류 target
  process는 없다.
- 문서 index JSON 검증과 TestDocumentationIndex.ps1, static proof, scoped
  git diff --check, 새 파일 trailing-whitespace 검사는 모두 PASS했다.
  git diff --check의 LF→CRLF 경고는 기존 작업 트리의 line-ending
  정책 알림이며 whitespace 오류는 없었다. 결과는
  D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-review-bundle-dry-run-projection-20260908\scoped-diff-check.log에 보존했다.
- 기존 dirty worktree와 C:\Git\2D\Original은 보존했고
  commit/push/merge/tag/release/deployment는 수행하지 않았다.

증거 root:
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-review-bundle-dry-run-projection-20260908

## 범위 경계

이번 완료는 dry-run 결과/status projection owner 분리에 한정한다.
Review bundle Inspector의 검사 정책, XML validation 자체, Pipeline Review
실행/result state, validation/Step Edit, 이미지·Layer lifetime, 전체
theme/layout/DPI(100/125/150/175/200%)와 다중 모니터 matrix는 별도
작업이다. 이번 캡처는 단일 모니터의 Release/Debug 대표 화면만 증명한다.

## Refactor proof

- Current owner: OpenVisionShellHostRecipeCommandSurface.LlmXmlDraftWorkflow.cs
  의 LoadReviewBundleForDryRun 결과 분기와 localized status formatting.
- Intended owner: OpenVisionRecipeReviewBundleDryRunProjectionOwner
  concrete module.
- Dependency direction: Shell LLM workflow → existing Inspector/XML
  validation → projection owner → Shell-bound status/return projection.
- State owner: bundle path, inspection reports, loaded inspection,
  XML validation, review-tab navigation과 Import/Preview/Run guard는
  workflow/Shell; owner는 입력 bool과 immutable-like projection만 가진다.
- Observable contract: review-only XML load, existing failure/success
  return semantics, localized status, Recipe/XML, Layer routing, explicit
  Preview/Run과 existing bindings를 유지한다.

Status: Complete
Scope: OVL-07 Review bundle dry-run result/status projection owner 분리.
Acceptance criteria: 세 dry-run 결과의 inline status projection 제거,
Window-free concrete owner 호출, Inspector/state/validation/review-only
side effects 보존, Debug/Release focused contract와 import smoke 통과.
Verification: x64 Debug/Release app·Runner·ScreenshotSmoke build, focused
projection contract, 기존 regression contracts, solution/readiness,
review-bundle import smoke, static proof, documentation index, scoped diff
check.
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-review-bundle-dry-run-projection-20260908.
Boundary / next dependency: Pipeline Review 실행/result status projection은
별도 범위이며, 다음 단일 작업은 OVL-07 Pipeline Review result/status
projection owner 분리다.