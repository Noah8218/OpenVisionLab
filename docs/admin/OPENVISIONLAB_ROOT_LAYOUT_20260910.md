# OpenVisionLab 2D Root Layout

- Date: 2026-09-10 KST
- Repository: `C:\Git\2D\Dev`
- Branch: `codex/public-sample-ux-docs`
- Commit observed at review: `65e2fd8b77989172d68d0aad81d489df94baf474`
- Target framework: `net8.0-windows7.0`
- Scope: make the repository root easier to scan without changing source ownership or stable tool paths.

## What belongs at the root

The root contains the solution and repository policy files, plus the following
responsibility owners:

| Entry | Responsibility | Keep visible |
| --- | --- | --- |
| `src/` | Application and internal library source | Yes |
| `tools/` | Smoke runners, contract checks, and utility projects | Yes |
| `docs/` | Product contracts, runbooks, architecture, and evidence routing | Yes |
| `scripts/` | Release and publishing scripts | Yes |
| `dll/` | Vendored SDK and native runtime dependencies | Yes |
| `third_party/` | External package/source material retained by the repository | Yes |
| `.github/` | Repository automation metadata | Yes when maintaining workflows |
| `.proofline/` | Local completion/evidence state | Yes when reviewing task evidence |

The source tree was not moved for visual sorting. Folder placement continues to
signal runtime responsibility, as required by `AGENTS.md`.

## Local and generated mounts

The following root entries are junctions, not second copies of the source tree:

| Entry | Physical target | Purpose | Explorer state |
| --- | --- | --- | --- |
| `.codex/`, `.codex-temp/` | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\...` | Codex probes and temporary smoke data | Hidden |
| `.vs/` | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\.vs` | Visual Studio cache | Hidden |
| `bin/`, `obj/` | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\...` | Build output and intermediate files | Hidden |
| `dist/` | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\dist` | Release/publish output | Hidden |
| `tmp/` | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\tmp` | Local temporary output | Hidden |
| `Sample/` | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\Sample` | Local/vendor sample material | Hidden |
| `artifacts/` | `D:\OpenVisionLab_Data\Dev\artifacts` | Existing historical evidence mount | Hidden |

The logical names remain unchanged because `Directory.Build.props`, smoke
scripts, runbooks, and existing operator evidence use paths such as
`bin\Debug`, `tools\...\bin`, and `artifacts\...`. Hiding the mounts removes
generated data from the normal Explorer view while keeping those contracts
valid for builds and tools. Use **View → Show → Hidden items** when inspecting
the mounts directly.

`artifacts/` still points at the pre-existing `D:\OpenVisionLab_Data\Dev`
location. It was not copied or deleted in this cleanup because that location
contains historical evidence and the canonical D: test root already contains
newer, non-overlapping artifacts. A separate verified data migration is needed
before changing that target.

## Ownership and call path

1. `Directory.Build.props` defines the repository root, vendored DLL root, and
   default exclusions for `bin`, `obj`, and `artifacts`.
2. `src/OpenVisionLab/OpenVisionLab.csproj` maps build configurations to the
   stable root-relative `bin` output path.
3. `tools/Move-OpenVisionLabLocalData.ps1` owns externalization and restoration
   of ignored local/build directories. Newly created mounts and existing root
   mounts are marked hidden in externalized mode and made visible again during
   a restore.
4. The build, smoke runners, and documentation continue to resolve those
   logical root-relative paths; no ViewModel, inspection, Recipe, or SDK call
   path was changed by this layout cleanup.

Mutable state remains owned by the external target directory. The repository
only owns the junction entry and the path contract. No WPF binding or public
Recipe/XML contract is affected.

## Verification and boundary

- Root inventory, link targets, attributes, and the before/after worktree
  snapshots are under
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\root-organization-20260910`.
- `Move-OpenVisionLabLocalData.ps1 -WhatIf` completed with
  `LocalDataMove=PASS`, `Candidates=76`, and no attribute changes.
- The existing root mounts retain `Junction` type and their original targets
  after the visibility update.
- The root cleanup does not prove desktop Explorer rendering on every Windows
  personalization setting; it changes only NTFS directory attributes and path
  ownership. Build/readiness checks are required after any subsequent change to
  the path targets themselves.

## Recommended reading order

`OpenVisionLab.sln` → `Directory.Build.props` → `src/OpenVisionLab/OpenVisionLab.csproj` → `src/OpenVisionLab/Program.cs` → `src/OpenVisionLab/App/Bootstrap/OpenVisionLabApplication.cs` → `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.xaml` → `docs/admin/CODEBASE_STRUCTURE.md` → this document.

