# OpenVisionLab P274 Runtime Data Root v1

Updated: 2026-07-30 KST

## Decision

P274 is `Complete` for separating immutable Release installation files from
writable operator data. Release uses `%LOCALAPPDATA%\OpenVisionLab` by default
or an administrator-supplied absolute `OPENVISIONLAB_DATA_ROOT` outside the
installation directory.

This closes the writable-install-folder limitation recorded by P273. It does
not claim an installer, signed binary, update service, commercial GA, or
multi-user/network-share qualification.

## Scope

- define installation-root and writable-data-root ownership;
- migrate former portable-layout data without overwrite or deletion;
- redirect Recipe, configuration, qualification, log, capture, cache, and
  reusable UI state;
- preserve relative Recipe template portability;
- block unsafe Release roots instead of silently falling back;
- prove copied-package launch, second-launch restoration, installation
  inventory immutability, focused persistence regressions, and clean-clone
  archive reproducibility.

The authoritative behavior contract is
`docs\contracts\openvisionlab\OPENVISIONLAB_RUNTIME_DATA_ROOT_V1_CONTRACT.md`.

## Defects Reproduced During Implementation

The work found and corrected two defects that static path replacement alone
would not have exposed:

- log4net assembly auto-configuration created zero-byte log files beside the
  packaged EXE before the external data root was resolved;
- Recipe workspace folders could still be created through the old external
  `AppUtil` path, causing an isolated PropertyGrid/Recipe persistence
  regression to fail.

The final implementation resolves the data root before application logging,
rebases every file appender, and makes `RecipeWorkspaceService` create all
owned folders beneath the resolved root.

## Runtime Ownership

| Owner | Content |
| --- | --- |
| Installation root | EXE, DLLs, runtime configuration, license/notice, deployment instructions, packaged read-only assets |
| Data root | `CONFIG`, `RECIPE`, `QUALIFIED_RECIPE`, `Log`, `CAPTURE`, `TEST`, `Image`, `CACHE`, `SYSTEM.xml`, and legacy root `VISION.xml` |

Release root precedence:

1. absolute external `OPENVISIONLAB_DATA_ROOT`;
2. `%LOCALAPPDATA%\OpenVisionLab`.

Relative or installation-contained overrides fail closed. Debug retains its
existing build-output default unless a task-specific absolute override is
supplied.

## Migration And Restoration

The one-time migration copies only missing legacy files from the executable
folder, never overwrites an existing target, never deletes the source, reports
conflicts, and writes `data-root-migration-v1.txt`. A failed required copy
writes `.incomplete` evidence and blocks startup. A second equivalent launch
restores the same data scope without repeating a completed migration.

Older flat `%LOCALAPPDATA%\OpenVisionLab` UI/log/cache files are also copied to
their v1 owners with a separate `data-layout-migration-v1.txt` record.

## Acceptance Evidence

| IDs | Evidence | Result |
| --- | --- | --- |
| D1-D4 | Release root-selection and fail-closed rules in `AppPathService`; readiness contract check | PASS |
| D5-D7, D9-D12 | Copied Release install with seeded CONFIG/RECIPE/QUALIFIED data, a pre-existing conflict, two launches, migration report, log verification, and full before/after install inventory | PASS |
| D8 | Required-copy failure path, `.incomplete` report, and startup block retained by source/readiness contract | PASS; fault injection not performed against operator data |
| D10-D11, D14 | Direct `runtime-data-root-contract` smoke: exact roots, relative template runtime/review resolution, zero Preview/Run, zero layer creation, no route change | PASS |
| D13 | Recipe Pipeline round trip, persistence feedback, PropertyGrid load feedback, and settings persistence feedback with isolated task roots | PASS |

Focused runtime evidence:

- `artifacts\p274_runtime_data_root_final_20260730`
- `artifacts\p274_runtime_data_root_final_20260730_data`
- `artifacts\p274_runtime_data_root_contract_20260730\report.txt`
- `artifacts\p274_regressions_r3_20260730`
- `artifacts\p274_regressions_r4_20260730`

The focused copied-install replay retained 92 installation files with zero
added, removed, or modified files, created no installation-root `Log`, copied
the expected legacy data, preserved the seeded `data-root-wins` conflict
target, wrote migration evidence, and passed the second launch.

## Clean-Clone Release Verification

Verified implementation commit:
`823d2d8acb87a269b79c602d29316e0908081ab0`

| Check | Result |
| --- | --- |
| Clean clone A | `C:\Git\OpenVisionLab_Production_DataRoot_RC_20260730` |
| Clean clone B | `C:\Git\OpenVisionLab_Production_DataRoot_Repro_20260730` |
| Debug build | PASS, 0 warnings, 0 errors |
| Release build | PASS, 0 warnings, 0 errors |
| Readiness | PASS, 13/13 |
| External references | PASS |
| Public asset policy | PASS |
| Public sample execution | PASS, 33/33 |
| Release payload | PASS, 75 files |
| Copied-package launch and second launch | PASS |
| Clone A ZIP SHA-256 | `807747DB316FE115E48728DF930F224F7CFB289CD597BDD0F5774B253CC123BD` |
| Clone B ZIP SHA-256 | `807747DB316FE115E48728DF930F224F7CFB289CD597BDD0F5774B253CC123BD` |
| Archive reproducibility | PASS, exact ZIP hash match |

Primary Release evidence:

- `C:\Git\OpenVisionLab_Production_DataRoot_RC_20260730\artifacts\production_release_candidate_823d2d8\release_candidate_summary.json`
- `C:\Git\OpenVisionLab_Production_DataRoot_RC_20260730\artifacts\release_launch_smoke_20260730_224128`
- `C:\Git\OpenVisionLab_Production_DataRoot_RC_20260730\dist\OpenVisionLab-win-x64-framework-dependent.zip`
- `C:\Git\OpenVisionLab_Production_DataRoot_Repro_20260730\dist\OpenVisionLab-win-x64-framework-dependent.zip`

## Next Production Priorities

1. Approve the distribution model, publisher identity, signing certificate,
   update channel, and machine/per-user installation policy.
   Prerequisite: the named business/deployment inputs above.
   Recommended model: none until prerequisites; `gpt-5.6-sol` afterward |
   Reasoning effort: none until prerequisites; `high` afterward.
2. Implement and verify installer, signed payload, update/rollback, uninstall,
   data retention, and migration recovery against the approved model.
   Recommended model: `gpt-5.6-sol` | Reasoning effort: `high`.
3. Generate and review SBOM/dependency/license evidence, then add an operator
   support bundle and bounded startup/run performance criteria.
   Recommended model: `gpt-5.6-terra` | Reasoning effort: `medium`.

## Completion Record

Status: Complete

Scope: Release installation/data-root separation, copy-only legacy migration,
path redirection, persistence compatibility, installation immutability, and
reproducible clean-clone package verification.

Acceptance criteria: root ownership and unsafe-root rejection -> pass;
migration/conflict/second-launch behavior -> pass; data-root Recipe,
configuration, qualification, log, and relative-template behavior -> pass;
installation inventory immutability -> pass; persistence/side-effect
regressions -> pass; two-clone byte-identical archive -> pass.

Verification: Debug and Release builds; readiness 13/13; direct
`runtime-data-root-contract`; four focused persistence regressions;
`tools\VerifyReleaseCandidate.ps1` in clone A; and
`tools\TestReleaseDistribution.ps1 -SkipLaunch` in clone B all passed.

Evidence: the repository and clean-clone artifact paths, report, launch
evidence, package paths, source commit, and exact ZIP SHA-256 listed above.

Boundary / next dependency: The package remains unsigned and
framework-dependent. Installer, signing, update/rollback, uninstall, SBOM and
legal approval, support policy, performance qualification, and field
acceptance remain outside P274. Distribution and publisher/signing decisions
are the next external prerequisites.
