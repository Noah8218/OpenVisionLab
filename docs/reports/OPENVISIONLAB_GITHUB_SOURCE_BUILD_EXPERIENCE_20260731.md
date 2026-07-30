# OpenVisionLab P275 GitHub Source Build Experience

Updated: 2026-07-31 KST

## Decision

P275 is `Complete` for the user-confirmed goal: a Windows user can obtain the
repository source and verify that it restores and builds without entering the
commercial installer/signing/update track.

The supported newcomer path is now one command:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\VerifySourceBuild.ps1
```

This is intentionally smaller than the Release Candidate gate. Installer,
code signing, update/rollback, uninstall, SBOM/legal review, support SLA, and
commercial GA are not active priorities for this source-build goal.

## Scope

- document an exact fresh-clone command sequence in the root README;
- require the exact .NET SDK pinned by `global.json`;
- provide one zero-option source-build verifier;
- check locked restore, Debug, Release, readiness, vendored DLL completeness,
  and expected executables;
- retain the existing GitHub Actions clean Windows checkout as the stricter
  repository gate;
- add an optional Windows Sandbox replay without making Sandbox a build
  prerequisite or changing Windows features automatically.

Public sample execution, package creation, UI launch, installer behavior, and
commercial deployment qualification are deliberately outside the lightweight
source-build command. The existing Release Candidate gate remains available
when those broader checks are specifically needed.

## User Workflow

```powershell
git clone https://github.com/Noah8218/OpenVisionLab.git
cd OpenVisionLab
powershell -NoProfile -ExecutionPolicy Bypass -File tools\VerifySourceBuild.ps1
.\bin\Debug\OpenVisionLab.exe
```

Requirements are Windows 10/11 x64, Git, PowerShell, Internet access for the
first NuGet restore, and exact .NET SDK `8.0.421`. Visual Studio is optional.
If the SDK is missing or differs, the verifier stops with the required version
and the official .NET download location instead of allowing compiler
roll-forward.

## Lightweight Verification Contract

`tools\VerifySourceBuild.ps1` performs:

1. Windows and repository-layout checks;
2. exact SDK availability check from `global.json`;
3. `dotnet restore OpenVisionLab.sln --locked-mode`;
4. Debug solution build;
5. Release solution build;
6. readiness contract check;
7. vendored OpenCV/Library-Noah/WPF PropertyGrid DLL check;
8. expected Debug and Release EXE existence check;
9. task-local JSON summary under `artifacts`.

It does not run Preview/Run, open the UI, change Layers or routes, create an
installer, or execute the full public sample catalog.

## Optional Windows Sandbox Contract

`tools\WindowsSandbox\InvokeSourceBuildSandbox.ps1`:

- refuses to start while another Sandbox instance is open;
- archives the current committed source instead of exposing the full working
  directory;
- maps only a new task-specific artifact directory as writable;
- enables network only for the official SDK and NuGet downloads;
- installs exact SDK `8.0.421` inside the disposable environment;
- copies in the source-build verifier and executes the same checks;
- writes progress, transcript, result, and build summary back to the artifact
  directory;
- closes only the Sandbox instance created for the verification.

Windows Sandbox remains optional. It is unavailable on Windows Home and
requires supported virtualization and the Windows optional feature. The tool
does not enable the feature, change BIOS/Hyper-V settings, or replace the
GitHub Actions clean-checkout evidence.

## Reproduced Sandbox Defect And Correction

The first actual Sandbox run reached the installed SDK but failed before build
because the stock Windows Sandbox PowerShell Archive module was missing its
localized `ArchiveResources.psd1`, so `Expand-Archive` could not load.

The runner now uses the built-in .NET
`System.IO.Compression.ZipFile.ExtractToDirectory` API and has no dependency on
that PowerShell module. A later start immediately after the first Sandbox
shutdown also exposed an instance-start race; the launcher now refuses an
already-open Sandbox and closes its own exact instance after result creation.

These were verification-harness defects, not OpenVisionLab source-build
failures.

## Verification

### Host one-command path

Evidence:
`artifacts\p275_source_build_local_20260731\source_build_summary.json`

| Check | Result |
| --- | --- |
| Source commit | `9ab13ced8cd8becd9ffc081a71d1c9ab7d6c7c2b` |
| SDK | `8.0.421` |
| Locked restore | PASS |
| Debug build | PASS, 0 warnings, 0 errors |
| Release build | PASS, 0 warnings, 0 errors |
| Readiness | PASS, 13/13 |
| Vendored references | PASS |
| Duration | 102.734 seconds |

### Final Windows Sandbox path

Evidence:
`artifacts\p275_windows_sandbox_actual_r4_20260731`

| Check | Result |
| --- | --- |
| Sandbox OS | Windows 10 clean disposable environment |
| Source snapshot | commit `9ab13ced8cd8becd9ffc081a71d1c9ab7d6c7c2b` |
| SDK installation | PASS, exact `8.0.421` |
| Source extraction | PASS |
| Locked restore | PASS |
| Debug build | PASS |
| Release build | PASS |
| Readiness | PASS |
| Vendored references | PASS |
| Build-verifier duration | 102.907 seconds |
| Total Sandbox bootstrap duration | 255.187 seconds |
| Sandbox process after result | none |

The final progress sequence was:
`BootstrapStarted -> DotnetInstallerDownloaded -> DotnetSdkInstalled ->
SourceExtracted -> SourceBuildStarted -> SourceBuildPassed`.

## Maintainer Guidance

- Keep `tools\VerifySourceBuild.ps1` short and independent from installer or
  release packaging policy.
- Treat `.github\workflows\ci.yml` as a stricter clean-checkout superset.
- Run the Sandbox path when SDK, vendored dependency, solution structure,
  bootstrap, or clean-machine assumptions change; it need not run for every
  documentation edit.
- Do not reopen installer/signing/SBOM work unless the user changes the goal
  from source-build availability to distributable commercial installation.

## Completion Record

Status: Complete

Scope: Fresh-clone README path, one-command source-build verification, exact
SDK diagnostics, and optional disposable Windows Sandbox replay.

Acceptance criteria: exact newcomer commands -> documented; locked restore and
Debug/Release build -> pass; readiness and vendored dependencies -> pass;
expected EXEs -> present; actual Windows Sandbox replay -> pass; Sandbox
cleanup -> pass; installer/signing work -> excluded.

Verification: `tools\VerifySourceBuild.ps1` passed locally; the final Windows
Sandbox replay passed all the same checks after correcting the Sandbox-only
archive-module dependency; PowerShell parsing and patch hygiene passed.

Evidence: the host and Sandbox artifact directories and JSON summaries listed
above.

Boundary / next dependency: This proves the committed source snapshot builds
in the tested Windows environments. It does not prove every user network,
future SDK availability, UI operation, inspection semantics, installer,
signing, updates, or commercial deployment. No further Productionization work
is active without a new explicit user request.
