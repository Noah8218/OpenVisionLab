# Changelog

OpenVisionLab의 사용자-visible 변경과 release evidence를 짧게 추적합니다.
정식 release gate는 `docs/OPENVISIONLAB_RELEASE_VERSION_POLICY.md`를 따릅니다.

## Unreleased

## 2.2.0-dev.5 - 2026-09-17

### Added

- Added fail-closed 2D runtime qualification for checkout commit and entry
  assembly length/SHA-256 before cross-repository process launch.
- Extended the separate-EXE integration smoke report with profile, source/recipe
  identity, source dimensions, and screenshot evidence.

### Changed

- Recorded PL-0107 evidence and the remaining external qualification boundaries
  in the live handoff and issue ledger.

## 2.2.0-dev.4 - 2026-09-16

### Added

- Promoted the verified 2D C# consumer, retry/fault-injection, input identity,
  and output-allocation evidence paths into the next Dev candidate.
- Recorded the remaining external and product-policy blockers in the durable
  handoff and issue ledger.

### Changed

- Aligned application, shell caption, integration contracts, offline manuals,
  README, and generated manual manifests to `2.2.0-dev.4`.

This Dev candidate is not a stable release, tag, deployment, or commercial-GA
claim. Its exact promotion gates remain separate.

## 2.2.0-dev.3 - 2026-09-16

### Added

- Evidence-constrained locator teaching, operator review-decision gating, and a
  correlated 2D handoff path were added without making LLM support a runtime
  prerequisite.
- External 2D image-buffer/projection consumer samples and deterministic
  cross-process evidence paths were added for bounded integration review.

### Changed

- Pipeline Review, Recipe switching, N-image verification, docking, Tool
  interaction, and workspace restoration were hardened for clearer and more
  stable operator workflows.
- Pipeline storage/recovery and original/effective execution provenance now
  fail closed and preserve reusable run evidence.

### Fixed

- Image/SDK Tool ownership, bitmap row copying, OpenGL cleanup, viewer
  coordinates, and review reopen/startup performance were corrected within the
  recorded Dev verification scopes.
- Unused or forbidden external DLLs were removed and retained dependency NOTICE
  coverage is enforced by the candidate gate.

These Dev changes are not a stable release, publication, deployment, or
commercial-GA claim. Their exact scope and remaining gates are in the current
handoff and dated reports.

## 2.2.0-dev.2 - 2026-09-10

### Changed

- 정량 folder audit를 기준으로 `OpenVisionLab.Docking.Controls`의 계약,
  모델, document, workspace, guide, layer-docking, converter, WPF View 파일을
  책임별 하위 폴더로 정리했습니다.
- namespace, public docking contract, XAML binding/resource URI, Recipe/XML,
  explicit Preview/Run 동작은 유지했습니다. smoke harness와 `Common`은 경로
  계약을 확인한 뒤 후속 검토 대상으로 남겼습니다.

### Verification

- C# 844개, XAML 60개, partial 110개, project cycle 0의 source audit와
  root-class 후보 3개→2개 inventory를 실행했습니다.
- `OpenVisionLab.sln`과 `VisionRecipeRunnerSmoke` Debug/Release 빌드가
  warnings 0/errors 0으로 통과했고, readiness와 ImageCompare/namespace
  focused contract가 양 구성에서 통과했습니다. desktop theme/DPI/장시간
  runtime qualification은 별도입니다.

## 2.2.0-dev.1 - 2026-09-09

### Highlights

- Dev 검증 기준을 `2.2.0-dev.1`로 정렬하고 애플리케이션·오프라인 매뉴얼·
  TCP integration identity가 같은 후보 버전을 사용하도록 했습니다.
- ImageCanvas, Image Compare, Recipe 실행·검증, Shell, PropertyGrid, Learn,
  Pipeline Review, and namespace ownership boundaries를 기존 Recipe/XML 및
  명시적 Preview/Run 계약 안에서 연결했습니다.

### Verification

- Clean Dev snapshot에서 `OpenVisionLab.sln` Debug/Release 빌드와 readiness,
  focused structural contracts를 실행합니다.
- 이 후보는 stable release, tag, deployment, installer 또는 commercial-GA를
  의미하지 않습니다.

## 2.1.0-rc.1 - 2026-08-05

### Highlights

- OpenVisionLab을 PropertyGrid teaching, Pipeline composition, explicit
  Preview/Run, result review, N-sample validation, and saved Recipe 흐름의
  rule-based vision workbench로 정리했습니다.
- 한국어와 영어 UI에 맞춰 선택되는 해시 검증 오프라인 가이드와 별도
  Learn 화면을 제공합니다.
- Recipe Manager 요약/고급 검토, Pipeline Review, Validation Set, Run
  History, 공개 샘플과 결과 증거 흐름을 연결했습니다.
- 큰 모니터와 서로 다른 작업 영역에서 전체 셸을 함께 확대하는 반응형
  배율을 적용했습니다.

### Changed

- 기존 Library-Noah 연결을 manifest-verified OpenVisionLab Vision SDK
  `3.0.0`으로 전환했습니다.
- SDK에서 제거된 WPF bitmap converter는 애플리케이션 소유 호환 경계로
  유지하고, detected-point Affine metadata는 SDK의 strict parameter gate
  전에 애플리케이션에서 처리합니다.
- Visual Studio 2022 17.8+의 .NET desktop development workload와 호환되는
  .NET SDK 선택·소스 빌드 경로를 제공합니다.

### Current Direction

- OpenVisionLab은 OpenCvSharp4 기반 deterministic rule-based vision recipe
  workbench입니다.
- LLM XML authoring은 선택적인 maintenance-mode 보조 기능이며 정상 사용의
  전제 조건이 아닙니다.
- 카메라, 조명, PLC/I/O, MES, equipment integration은 현재 범위 밖입니다.

### Distribution

- Windows 10/11 x64용 portable, framework-dependent ZIP입니다.
- Microsoft .NET 8 Desktop Runtime x64가 필요합니다.
- 이 후보는 unsigned pre-release이며 installer, automatic update/rollback,
  uninstall, SBOM/legal approval, multi-PC qualification 또는 commercial GA를
  의미하지 않습니다.

### Release Gate

- 태그 전 새 원본 clone에서 다음 전체 게이트를 실제 EXE launch 포함으로
  통과해야 합니다.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\VerifyReleaseCandidate.ps1
```
