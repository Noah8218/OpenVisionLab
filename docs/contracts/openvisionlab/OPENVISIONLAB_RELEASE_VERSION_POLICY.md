# OpenVisionLab Release and Version Policy

Updated: 2026-08-05

이 문서는 OpenVisionLab과 외부 런타임의 버전·출처·호환성 검증 기준을 정의합니다.

## Repository Roles

| Unit | Role | Release Unit |
| --- | --- | --- |
| `OpenVisionLab` | WPF UI, Pipeline, Recipe Runner, Tool View, Tutorial, Sample Catalog | application version |
| `dll\OpenCVSharp` | shared OpenCVSharp native runtime, including OpenCvSharpExtern.dll | vendored native runtime |
| `dll\OpenVisionLab-Vision-SDK` | OpenVisionLab.Core/OpenVisionLab.Vision2D/OpenVisionLab.Vision2D.Blob 및 OpenCvSharp 관리 DLL | manifest-verified vendored SDK 3.0 set |
| `dll\System.Windows.Controls.WpfPropertyGrid.dll` | WPF PropertyGrid runtime | prepared DLL |
| `OpenVisionLab.ImageCompare` | Image Compare standalone tool | standalone executable |

## Canonical Product Version

`src/OpenVisionLab/OpenVisionLab.csproj` owns `<Version>`. Shell captions,
application metadata and the generated integration runtime manifest derive the
product identity from that property. A `-dev.N` suffix denotes a development
prerelease; do not present it as a stable-version identity.

`AssemblyVersion` and `FileVersion` are separate properties. The current project
keeps both at `2.2.0.0`; do not infer the product prerelease suffix from assembly
binding identity. Changing assembly identity requires checking binary consumers.
Do not change dependency package versions or vendored DLL hashes merely to match
the application version. SDK provenance is recorded by
`dll/OpenVisionLab-Vision-SDK/sdk-manifest.json`; integration package identities
and exact versions come from project references, lock files and the local feed.

## Version Order

1. OpenVisionLab Vision SDK 변경이 필요한 경우 SDK 저장소에서 Release 빌드, 전체 스모크와 패키지 소비자 스모크를 먼저 통과시킨다.
2. 확정된 DLL을 `dll\OpenVisionLab-Vision-SDK\`에 복사하고 `sdk-manifest.json`의 source commit, 파일 길이와 SHA-256을 갱신한다.
3. WPG-CUSTOM 변경이 필요한 경우 별도 저장소에서 DLL/XML을 생성해 `dll\`에 복사한다.
4. OpenVisionLab에서 빌드, readiness gate, platform precheck를 실행한다.

## Runtime Provenance

OpenVisionLab source identity, SDK version/commit, vendored DLL length/SHA-256 and WPG provenance must remain traceable through their manifests.

## Source/DLL Checks

- Build the application: `dotnet build OpenVisionLab.sln -c Debug -p:Platform=x64`.
- Run the vendored DLL check: `powershell -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1`.
- Run the readiness gate: `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj`.
- Run the public sample catalog after any catalog/pipeline change: `powershell -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.PublicSampleCatalog.csv -OutputDir <evidence-dir>`.

The legacy `docs\samples\OpenVisionLab.SampleCatalog.csv` remains available for
explicit local-only historical checks, but its ignored `Sample\...` inputs are
not a release prerequisite and must not be used as public sample evidence.

## Commit Discipline

- OpenVisionLab 코드 변경과 외부 DLL 갱신은 가능하면 분리한다.
- DLL 갱신 커밋에는 변경된 DLL 이름과 출처를 적는다.
- README/Tutorial/Sample Catalog 같은 문서 변경은 기능 변경과 섞지 않는 것을 우선한다.
- 일반 빌드가 외부 소스 경로에 의존하지 않는지 확인한다.

## Compatibility Gate

Preserve build, vendored DLL, History, Localization Catalog, Tool Result, public
Sample Catalog, OpenVision Readiness, Vision Recipe Runner API, portable Tutorial
and Image Compare coordinate contracts when changing dependencies.

Normal clone/build uses source and vendored DLLs under `dll/`, not generated
outputs. `dll/Library-Noah` remains absent; share the native runtime from
`dll/OpenCVSharp/OpenCvSharpExtern.dll` and record its hash in
`dll/OpenVisionLab-Vision-SDK/sdk-manifest.json`.

Runtime data-root behavior is specified in
[the stable data-root contract](OPENVISIONLAB_RUNTIME_DATA_ROOT_V1_CONTRACT.md).
