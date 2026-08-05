# Changelog

OpenVisionLab의 사용자-visible 변경과 release evidence를 짧게 추적합니다.
정식 release gate는 `docs/OPENVISIONLAB_RELEASE_VERSION_POLICY.md`를 따릅니다.

## Unreleased

- No user-visible changes after the `2.1.0-rc.1` candidate.

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
