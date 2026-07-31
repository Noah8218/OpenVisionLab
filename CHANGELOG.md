# Changelog

OpenVisionLab의 사용자-visible 변경과 release evidence를 짧게 추적합니다.
정식 release gate는 `docs/OPENVISIONLAB_RELEASE_VERSION_POLICY.md`를 따릅니다.

## Unreleased

### Added

- 루트 `README.md`에 1분 요약, 설치/실행, 샘플 데이터, build/smoke command, CI, release note, roadmap, known limitations 섹션을 정리했습니다.
- GitHub Actions 최소 CI workflow를 추가했습니다.
- Recipe-local Validation Suite / Result Archive 설계 문서를 추가했습니다.

### Current Direction

- OpenVisionLab은 LLM-assisted OpenCvSharp4 rule-based vision recipe workbench입니다.
- LLM 기능은 one-shot 자동 영상처리가 아니라 operator intent와 샘플 evidence를 바탕으로 XML recipe 작성/검증/수정 루프를 돕는 방향입니다.
- 카메라, 조명, PLC, I/O, account, deployment 플랫폼 확장은 현재 범위 밖입니다.

### Release Gate

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`
- `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"`
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1`
- src/OpenVisionLab/UI/UX 변경 시 current-build/current-source before/after screenshot smoke evidence
