# OpenVisionLab Release and Version Policy

Updated: 2026-07-19

이 문서는 OpenVisionLab, vendored Library-Noah DLL, WPG PropertyGrid DLL, ImageCompare standalone 산출물을 어떤 기준으로 릴리즈할지 정의합니다.

## Repository Roles

| Unit | Role | Release Unit |
| --- | --- | --- |
| `OpenVisionLab` | WPF UI, Pipeline, Recipe Runner, Tool View, Tutorial, Sample Catalog | application tag + build artifact |
| `dll\OpenCVSharp` | shared OpenCVSharp native runtime, including OpenCvSharpExtern.dll | vendored native runtime |
| `dll\Library-Noah` | Lib.Common/Lib.OpenCV/Lib.OpenCV.Blob 및 OpenCvSharp 런타임 | vendored DLL set |
| `dll\System.Windows.Controls.WpfPropertyGrid.dll` | WPF PropertyGrid runtime | prepared DLL |
| `OpenVisionLab.ImageCompare` | Image Compare standalone tool | standalone publish artifact |

## Version Order

1. Library-Noah 변경이 필요한 경우 별도 저장소에서 먼저 확정한다.
2. 확정된 DLL을 `dll\Library-Noah\`에 복사한다.
3. WPG-CUSTOM 변경이 필요한 경우 별도 저장소에서 DLL/XML을 생성해 `dll\`에 복사한다.
4. OpenVisionLab에서 빌드, readiness gate, platform precheck를 실행한다.
5. 필요한 경우 ImageCompare standalone을 publish한다.

## Required Release Evidence

릴리즈 설명에는 최소 항목을 남깁니다.

- OpenVisionLab commit/tag.
- `dll\OpenCVSharp\OpenCvSharpExtern.dll` update status and source.
- `dll\Library-Noah` DLL 갱신 여부와 출처.
- WPG PropertyGrid DLL 갱신 여부와 출처.
- `tools\TestExternalReferences.ps1` 결과.
- `platform_precheck_summary.json` 경로 또는 첨부.
- Sample Catalog summary: runnable rows, OK rows, NG rows, uncovered folders.
- ImageCompare standalone artifact 경로.

## Runner/DLL Release Checklist

- Build the application: `dotnet build OpenVisionLab.sln -c Debug -p:Platform=x64`.
- Run the vendored DLL check: `powershell -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1`.
- Run the readiness gate: `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj`.
- Run the non-UI platform precheck: `powershell -ExecutionPolicy Bypass -File tools\RunVisionPlatformPrecheck.ps1 -SkipUi`.
- Run the sample catalog after any catalog/pipeline change: `powershell -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -OutputDir <evidence-dir>`.
- Generate the release evidence JSON: `powershell -ExecutionPolicy Bypass -File tools\NewOpenVisionReleaseEvidence.ps1 -OutputDir <evidence-dir>`.

## Commit Discipline

- OpenVisionLab 코드 변경과 외부 DLL 갱신은 가능하면 분리한다.
- DLL 갱신 커밋에는 변경된 DLL 이름과 출처를 적는다.
- README/Tutorial/Sample Catalog 같은 문서 변경은 기능 변경과 섞지 않는 것을 우선한다.
- 일반 빌드가 외부 소스 경로에 의존하지 않는지 확인한다.

## Release Gate

아래 중 하나라도 실패하면 release candidate로 올리지 않습니다.

- Build gate 실패.
- Vendored DLL check 실패.
- History Contract 실패.
- Localization Catalog Contract 실패.
- Tool Result Contract 실패.
- Sample Catalog required row 실패.
- OpenVision Readiness Contract 실패.
- Vision Recipe Runner API Contract 실패.
- Tutorial Portable Contract 실패.
- Image Compare Coordinate Contract 실패.

## Generated Artifact Policy

- `dist\` is a generated publish-output root and should not be treated as source.
- ImageCompare standalone packages are regenerated with `scripts\Publish-ImageCompare.ps1`.
- Normal clone/build validation must use committed source files and vendored DLLs under `dll\`, not tracked publish outputs.
- `dll\Library-Noah\OpenCvSharpExtern.dll` must stay removed. The native OpenCVSharp runtime is shared from `dll\OpenCVSharp\OpenCvSharpExtern.dll`.

## Clean Runtime Output Contract (2026-07-19)

- Dev EXE evidence is built with `powershell -NoProfile -ExecutionPolicy Bypass -File tools\BuildCleanRuntime.ps1 -Mode Dev`. The command creates a new timestamped runtime under `artifacts\openvisionlab_clean_runtime_<timestamp>`.
- The release package is published with `powershell -NoProfile -ExecutionPolicy Bypass -File tools\BuildCleanRuntime.ps1 -Mode Release` to `dist\OpenVisionLab` using the `Release` configuration.
- Both modes reject an existing output directory. Release mode accepts only `dist\OpenVisionLab`; this prevents a stale package from being silently reused or overwritten.
- `bin\Debug` remains a retained local recipe workspace. It is not a clean-runtime evidence path or a release package and is not deleted, moved, or migrated automatically.
- P134/P137 verify the template-dependency part of the output contract: Import copies an operator-accessible template into the recipe and stores an installation-root-relative `RECIPE\...\Template\...` reference; Matching, EdgeBasedMatching, and FeatureMatching resolve that path from the running installation root. A freshly published `dist\OpenVisionLab` package was copied to another root and replayed successfully. This does not establish installer, signing, update, or production-deployment qualification.

## Current Policy Decision

- OpenVisionLab은 `Library-Noah`와 `WPG-CUSTOM` 소스 프로젝트를 직접 참조하지 않는다.
- 다른 PC 검증과 GitHub clone/build는 저장소 내부 DLL만 사용한다.
- 별도 소스 저장소는 DLL을 갱신해야 할 때만 필요하다.
