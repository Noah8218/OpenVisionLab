# OpenVisionLab Release And Version Policy

Updated: 2026-07-03

This document defines how OpenVisionLab source, prepared DLLs, sample catalogs, and generated release artifacts should move into a public release branch.

## Repository Roles

| Unit | Role | Release Unit |
| --- | --- | --- |
| `OpenVisionLab` | WPF shell, MVVM tool views, recipe runner, sample review UX | application tag + build artifact |
| `dll\Library-Noah` | prepared managed vision/runtime DLLs | vendored DLL set |
| `dll\OpenCVSharp` | shared OpenCVSharp native runtime | native runtime DLL set |
| `dll\System.Windows.Controls.WpfPropertyGrid.dll` | WPF PropertyGrid runtime | prepared DLL |
| `docs\samples\public` | redistributable synthetic samples | public sample asset set |

## Version Order

1. Keep application source changes separate from DLL updates when practical.
2. If Library-Noah managed DLLs change, update `dll\Library-Noah\` and document the source of the DLLs.
3. If OpenCVSharp native runtime changes, update `dll\OpenCVSharp\`; do not add another copy under `dll\Library-Noah\`.
4. If a new large native runtime is needed, decide NuGet, Git LFS, or release-artifact handling before committing it.
5. Run build, readiness, external reference, and focused smoke checks.
6. Keep public samples under `docs\samples\public\` or another clearly licensed public path.

## Required Release Evidence

A release or handoff PR should include:

- OpenVisionLab commit or tag.
- DLL update summary, if any.
- External reference check result.
- Readiness check result.
- Product/public sample catalog check result when samples or pipelines changed.
- Focused WPF/EXE smoke evidence when UI paths changed.

## Runner/DLL Release Checklist

```powershell
dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"
powershell -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab"
```

Run product sample catalog checks after catalog, pipeline, or synthetic sample changes. Run focused WPF screenshot/EXE smoke checks after shell, docking, viewer, or sample-review UI changes.

## Public Handoff Rules

- Do not merge development history that once contained vendor sample binaries into the public repository.
- Prefer squash, cherry-pick, or final-tree import when moving from a development workspace to public GitHub.
- Do not track root `Sample/`, generated captures, `artifacts/`, `.codex/`, `tmp/`, `bin/`, or `obj/`.
- README/tutorial captures must be regenerated from the current build and must not reuse old screenshots.
- Public documentation must not contain private workflow notes, portfolio-only wording, or local-only vendor sample paths.

## Release Gate

Do not promote a release candidate if any of these fail:

- Build.
- External reference check.
- Readiness check.
- Public sample asset/catalog check for touched sample areas.
- Localization or PropertyGrid contract after UI changes.
- Focused WPF/EXE smoke for touched UI paths.
