# OpenVisionLab Vision SDK 3.0 Migration

Date: 2026-08-05 KST
OpenVisionLab Dev baseline: `6698a83a212641beb6e3c12066ddf41c7d1593a1`
SDK source: `C:\Git\OpenVisionLab-Vision-SDK`
SDK source commit: `ba0055b713e0bf434b9d0a7fd3f4b0e445c1f982`
SDK version: `3.0.0`

## Result

OpenVisionLab now consumes the renamed OpenVisionLab Vision SDK instead of the
former Library-Noah assemblies. The repository remains buildable from one
checkout because the exact Release DLL set and its provenance manifest are
tracked under `dll\OpenVisionLab-Vision-SDK`.

The application namespace mapping is:

| Former owner | Current SDK owner |
| --- | --- |
| `Lib.Common` | `OpenVisionLab.Core` |
| `Lib.Line` | `OpenVisionLab.Core.Geometry2D` |
| `Lib.OpenCV` | `OpenVisionLab.Vision2D` |
| `Lib.OpenCV.Pipeline` | `OpenVisionLab.Vision2D.Pipeline` |
| `Lib.OpenCV.Property` | `OpenVisionLab.Vision2D.Property` |
| `Lib.OpenCV.Result` | `OpenVisionLab.Vision2D.Result` |
| `Lib.OpenCV.Tool` | `OpenVisionLab.Vision2D.Tool` |
| `Lib.OpenCV.Blob` | `OpenVisionLab.Vision2D.Blob` |

The former `dll\Library-Noah` directory and duplicate managed OpenCvSharp DLLs
under `dll\OpenCVSharp` are removed. The remaining native runtime is the one
manifest-verified `dll\OpenCVSharp\OpenCvSharpExtern.dll`.

## Application-Owned Compatibility Boundary

SDK 3.0 intentionally does not carry the WPF bitmap converter. OpenVisionLab
therefore owns `src\OpenVisionLab\Common\BitmapImageConverter.cs`. It is an
exact behavior port of the last SDK source version before the UI converter was
removed (`f7eeeb3b0bee938424be951143e8c717256ac10e`,
`src/OpenVisionLab.Core/Converter/BitmapConvert.cs`). Normalized source
comparison was 492 lines with zero differing lines after the namespace and
class-name change. An independent old/new harness passed nine conversion and
failure cases.

SDK 3.0 also rejects unknown Tool property keys. OpenVisionLab keeps that
fail-closed rule. The app-owned detected-point Affine binding now consumes and
removes only its four feature-binding keys and `ALLOW_BRANCH_INPUT` from the
runtime clone after resolving the three typed Points, then passes the six
numeric source coordinates to the SDK. The saved Recipe/XML Step remains
unchanged. No general unknown-parameter fallback was added.

## Vendored SDK Identity

`dll\OpenVisionLab-Vision-SDK\sdk-manifest.json` records the SDK repository,
version, exact source commit, build configuration, lengths, and SHA-256 values
for all five managed DLLs. It also records the shared native runtime identity.
`tools\TestExternalReferences.ps1` verifies that manifest, every file hash and
length, the native runtime, WPG-CUSTOM, and removal of all predecessor names.

## Verification

### SDK repository

- Locked restore and Release build passed with zero warnings and zero errors.
- `OpenVisionLab.Inspection.Smoke` passed 142/142.
- Pack produced exactly five NuGet packages; every package contained its README
  and `lib/netstandard2.0/<package>.xml` documentation.
- A package-only 2D properties/tools/Blob, 3D, surface-match, and mesh consumer
  passed with an isolated empty NuGet cache.
- The first package-consumer attempt found a stale machine-global cache entry
  with the same `3.0.0` version. Isolated restore proved the produced packages;
  consumers replacing a same-version local build must clear or isolate that
  cache.

### OpenVisionLab source and contracts

- `tools\VerifySourceBuild.ps1` passed locked restore, Debug and Release builds
  with zero warnings/errors, readiness 13/13, and Debug/Release external DLL
  verification. Selected SDK: `8.0.423`.
- `tools\TestPublicSampleAssets.ps1` passed: 33 catalog rows, 229 manifest
  assets, and 17 pipelines.
- `OpenVisionFixtureSmoke` passed translation, fail-closed frame/ROI/duplicate
  cases, public Good/Bad, NormalizeImage coverage, and fixed RotateScale
  compatibility.
- `VisionRecipeRunnerSmoke --affine-transform-contract` passed.
- `VisionRecipeRunnerSmoke --affine-detected-points-contract` passed after the
  app/SDK parameter-boundary correction.
- `VisionRecipeRunnerSmoke --edge-global-polarity-contract` passed 20/20.
- `RecipeXmlCompatibilityCheck` passed all 13 XML roots against an empty clean
  recipe scan root. Two local runtime-created empty pipelines under
  `bin\Debug\RECIPE` remain invalid recipes by design; they are not tracked or
  included in the clean runtime and were not used as SDK compatibility input.
- `QualifiedRecipeSnapshotSmoke` passed its initial and revision snapshot IDs.
- Focused current-source UI smoke passed 4/4 with zero warnings: Affine Tool
  View, detected-point Affine PropertyGrid, Threshold Tool View, and Blob Tool
  View.
- A Release-configuration Dev clean runtime was built with the SDK provenance
  in `clean_runtime_manifest.json`. Its actual EXE opened, remained responsive,
  and closed normally. It was placed at `-310,426` with captured size
  `1290x900`, intersecting the selected leftmost monitor `DISPLAY2`
  (`-1920,365,1920x1080`).
- Release-evidence generation found all eight prepared manifest/DLL files and
  reported `ReleaseGateOk=true`. `TagReady=false` is expected because this
  reviewed change set is not committed yet.

## Evidence

- Task root:
  `D:\OpenVisionLab-TestData\OpenVisionLab\sdk3_migration_20260805`
- SDK checkout and standalone results:
  `D:\OpenVisionLab-TestData\OpenVisionLab\sdk3_migration_20260805\sdk`
- Converter equivalence harness:
  `D:\OpenVisionLab-TestData\OpenVisionLab\sdk3_migration_20260805\converter-equivalence`
- Source-build summary:
  `artifacts\sdk3_source_build_20260805_01\source_build_summary.json`
- Clean runtime:
  `artifacts\sdk3_clean_runtime_20260805_01`
- Affine and polarity evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab\sdk3_migration_20260805\vision-recipe-contracts`
- UI smoke:
  `D:\OpenVisionLab-TestData\OpenVisionLab\sdk3_migration_20260805\ui-smoke`
- Qualified snapshot:
  `D:\OpenVisionLab-TestData\OpenVisionLab\sdk3_migration_20260805\qualified-recipe-snapshot-2`
- Release-evidence probe:
  `D:\OpenVisionLab-TestData\OpenVisionLab\sdk3_migration_20260805\release-evidence-probe`

## Closure

```text
Status: Complete
Scope: Dev repository migration from Library-Noah assemblies/namespaces to manifest-verified OpenVisionLab Vision SDK 3.0, including application compatibility boundaries, tools, documentation, build/runtime packaging, and focused functional/UI regressions
Acceptance criteria: SDK standalone build/smoke/package consumer pass; no active predecessor assembly reference remains; Debug/Release source build passes; external hashes pass; Affine fixed/detected-point and edge polarity contracts pass; XML/snapshot/public sample/fixture/UI checks pass; clean runtime EXE launches from the D-backed artifact root
Verification: SDK Release 0 warnings/0 errors and 142/142; package consumer pass with isolated cache; OpenVisionLab locked restore plus Debug/Release 0 warnings/0 errors; readiness 13/13; public assets pass; fixture pass; Affine fixed and detected-point pass; edge polarity 20/20; XML 13 roots; snapshot pass; UI 4/4; clean runtime launch/respond/close pass
Evidence: docs/reports/OPENVISIONLAB_VISION_SDK_3_MIGRATION_20260805.md and D:\OpenVisionLab-TestData\OpenVisionLab\sdk3_migration_20260805
Boundary / next dependency: C:\Git\OpenVisionLab original was not changed. Dev changes are not staged, committed, pushed, tagged, or promoted; final Release tag readiness requires an intentional clean commit. Package consumers replacing another local 3.0.0 build must clear or isolate the NuGet cache.
```
