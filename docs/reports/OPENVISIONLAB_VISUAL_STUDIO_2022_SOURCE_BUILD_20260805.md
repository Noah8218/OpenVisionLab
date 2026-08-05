# OpenVisionLab SDK And Visual Studio 2022 Compatibility

Date: 2026-08-05 KST

Status: Complete

## Scope

Replace the exact `.NET SDK 8.0.421` source-build requirement with the smallest
verified range that covers Visual Studio 2022. OpenVisionLab still targets
`net8.0-windows7.0`; this change does not add .NET 7, SDK 10, or cross-platform
support.

The verified policy is:

```json
{
  "sdk": {
    "version": "8.0.100",
    "allowPrerelease": false,
    "rollForward": "major"
  }
}
```

Microsoft's `global.json` contract defines `major` as preferring the requested
major/minor and rolling to a higher major only when no compatible requested
version exists. This lets Visual Studio 2022 versions 17.8 through 17.14 use
their bundled SDK 8.x or 9.x. Visual Studio builds that target `net8.0` require
Visual Studio 2022 17.8 or later.

Official references:

- [global.json overview](https://learn.microsoft.com/en-us/dotnet/core/tools/global-json)
- [.NET 8 SDK version requirements](https://learn.microsoft.com/en-us/dotnet/core/compatibility/sdk/8.0/version-requirements)
- [.NET SDK, MSBuild, and Visual Studio versioning](https://learn.microsoft.com/en-us/dotnet/core/porting/versioning-sdk-msbuild-vs)

## Changed Behavior

- `global.json` accepts SDK `8.0.100+` in the `8.x` line and SDK `9.x`.
- `tools/VerifySourceBuild.ps1` records the SDK actually selected, the minimum
  SDK, maximum SDK major, roll-forward policy, and required .NET 8 runtimes
  instead of requiring an exact installation.
- The clean Windows Sandbox helper installs the minimum version declared by
  `global.json` instead of duplicating `8.0.421` in the helper.
- README and release-policy documentation now state the same supported range.

## Acceptance Criteria And Evidence

| Criterion | Result | Evidence |
| --- | --- | --- |
| Minimum SDK `8.0.100` is selected and completes locked restore, Debug, Release, readiness, and vendored-reference checks | PASS | `artifacts/sdk_compat_8_0_100_vs_policy_20260805/source_build_summary.json` |
| Reported-user SDK `8.0.300` completes the same source-build gate | PASS | `artifacts/sdk_compat_8_0_300_vs_policy_20260805/source_build_summary.json` |
| A new D-drive Git checkout snapshot with no prior build outputs completes the gate with `8.0.300` | PASS | `D:/OpenVisionLab-TestData/OpenVisionLab/sdk-compat/fresh-clone-8.0.300-20260805/artifacts/fresh_clone_sdk_8_0_300/source_build_summary.json` |
| A new D-drive checkout reproducing Visual Studio 2022 17.14's SDK 9 and .NET 8 runtime combination completes the gate | PASS with `9.0.316` and .NET 8.0.29 runtimes | `D:/OpenVisionLab-TestData/OpenVisionLab/sdk-compat/fresh-clone-vs2022-sdk9-r2-20260805/artifacts/fresh_clone_sdk_9_0_316/source_build_summary.json` |
| Installed Visual Studio 2022 has the `.NET desktop development` workload and builds the solution through its MSBuild | PASS with Visual Studio 2022 17.14.37 / MSBuild 17.14.51 | `D:/OpenVisionLab-TestData/OpenVisionLab/sdk-compat/vs2022-workload-build-20260805/msbuild-debug.log` |
| Later installed major SDKs are not selected | PASS | With 9.0.316 and 10.0.302 installed, repository `dotnet --version` selected 8.0.423 |
| Public sample asset policy remains valid | PASS | 33 catalog rows, 229 manifest assets, 17 pipelines |

The final SDK 8.0.100, 8.0.300, and 9.0.316 source-build runs produced zero
Debug and Release build warnings and zero errors. Repository readiness passed
13/13 in each run. The Visual Studio 2022 MSBuild run also produced zero
warnings and zero errors.

The first SDK 9-only test built Debug and Release successfully but failed when
the `net8.0` readiness executable started because that portable SDK contained
only 9.x runtimes. Adding the .NET 8 Core and Desktop runtimes reproduced the
actual Visual Studio workload and the fresh-checkout retry passed. The README
therefore keeps the runtime requirement explicit for standalone SDK 9 users.

The logical `artifacts` path is a junction whose physical target is
`D:\OpenVisionLab_Data\Dev\artifacts`. Portable SDKs and their temporary files
were installed under
`D:\OpenVisionLab-TestData\OpenVisionLab\sdk-compat`.

Summary SHA-256 values:

- `8.0.100`: `3B8429EC936A4D2768404CBFB5A8DEF0F6BD740FD8C6A056CE56486F62772D87`
- `8.0.300`: `D8F7385058802273EA32837609241BDE822BA863C7A9D9241F2E7F70E3C4609F`
- clean-checkout `8.0.300`: `404D5A80713571397BCBD85D7676917468247D93F28B9E5DF652B985E3984C04`
- clean-checkout `9.0.316`: `A0794C99983C0C0697F3D3185DA2F63E04865A47D21B98A6D1CF65923A5C05BA`

## Commands Run

For each isolated SDK root, with `DOTNET_ROOT`, `PATH`, `TEMP`, and `TMP`
pointing to its D-drive location:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\VerifySourceBuild.ps1 -OutputDir <SDK-specific-artifact-directory>
```

Additional checks:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1
git diff --check
```

Both changed PowerShell files also passed parser validation.

## Boundary / Next Dependency

- This proves command-line source-build compatibility for SDK 8.0.100,
  8.0.300, and 9.0.316, plus a Visual Studio 2022 17.14 workload/MSBuild build.
  It does not automate the interactive Clone/F5 clicks on the external PC.
  That PC still needs Visual Studio 2022 17.8 or later with the `.NET desktop
  development` workload.
- The separate non-UTF-8/CP949 source defect was resolved and passed the local
  GitHub-equivalent clean-clone gate in P287. See
  `docs/reports/OPENVISIONLAB_GITHUB_CI_UTF8_SOURCE_REPAIR_20260805.md`.
- The SDK policy and P287 encoding repair were published as Dev commit
  `28bd8501f659169d02d6c5ccf951419b9feea53b` and exact-ported to original
  commit `a17cfe6bdb48f2e583cc7e9d46fc7afd4dd4bca4`. Both hosted Actions runs
  passed.
