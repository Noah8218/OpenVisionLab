# Changelog

User-facing changes to OpenVisionLab. Product and dependency versioning follows
[the version policy](docs/OPENVISIONLAB_RELEASE_VERSION_POLICY.md).

## 2.2.0-dev.10 - 2026-10-18

- Rewrites the English and Korean README for clearer inspection, setup and build instructions while preserving the detailed workflow, tables, commands and media.

## 2.2.0-dev.9 - 2026-10-17

- Aligns the two offline manual manifests, generated Guide headers and checksum manifest with the application version.
- Pins generated Guide HTML to CRLF and checks manual/application version agreement during readiness validation.

## 2.2.0-dev.8 - 2026-10-17

- Reduces redundant updates during image selection, tool-editor activation and log filtering.
- Improves inline tool editing, recipe transitions and inspection-result review.
- Adds a full Korean README and refreshes the screenshots and workflow recordings.

## 2.2.0-dev.7 - 2026-10-06

- Improves Recipe Manager selection and explicit work opening, preserving edits
  through cancellation and save failures.
- Modernizes shared controls and nine UI areas, and improves ROI dialog layout,
  long Tool errors, and input/output layer names.
- Fixes output-layer target synchronization and updates the main workspace,
  native Tool forms, Recipe persistence, Pipeline execution and LLM/XML review.

## 2.2.0-dev.6 - 2026-09-27

- Includes the recipe workspace sources and integration alpha.4 packages.
- Adds NuGet lock files for repeatable dependency restores.
- Clarifies the Visual Studio workload, desktop SDK and Git prerequisites for source builds.

## 2.2.0-dev.5 - 2026-09-17

### Added

- Added fail-closed 2D runtime qualification for checkout commit and entry
  assembly length/SHA-256 before cross-repository process launch.
- Extended the separate-EXE integration smoke report with profile, source/recipe
  identity, source dimensions, and screenshot evidence.

## 2.2.0-dev.4 - 2026-09-16

### Added

- Added typed 2D C# consumer outcomes, retry/fault-injection, input identity,
  and output-allocation checks.

### Changed

- Aligned application, shell caption, integration contracts, offline manuals,
  README, and generated manual manifests to `2.2.0-dev.4`.


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
  affected workflows.
- Unused or forbidden external DLLs were removed and retained dependency NOTICE
  coverage is enforced by the candidate gate.


## 2.2.0-dev.2 - 2026-09-10

### Changed

- 정량 folder audit를 기준으로 `OpenVisionLab.Docking.Controls`의 계약,
  모델, document, workspace, guide, layer-docking, converter, WPF View 파일을
  책임별 하위 폴더로 정리했습니다.
- namespace, public docking contract, XAML binding/resource URI, Recipe/XML,
  explicit Preview/Run 동작은 유지했습니다. smoke harness와 `Common`은 경로
  계약을 확인한 뒤 후속 검토 대상으로 남겼습니다.

## 2.2.0-dev.1 - 2026-09-09

### Highlights

- Dev 검증 기준을 `2.2.0-dev.1`로 정렬하고 애플리케이션·오프라인 매뉴얼·
  TCP integration identity가 같은 후보 버전을 사용하도록 했습니다.
- ImageCanvas, Image Compare, Recipe 실행·검증, Shell, PropertyGrid, Learn,
  Pipeline Review, and namespace ownership boundaries를 기존 Recipe/XML 및
  명시적 Preview/Run 계약 안에서 연결했습니다.

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

### Product Direction

- OpenVisionLab은 OpenCvSharp4 기반 deterministic rule-based vision recipe
  workbench입니다.
- LLM XML authoring은 선택적인 maintenance-mode 보조 기능이며 정상 사용의
  전제 조건이 아닙니다.
