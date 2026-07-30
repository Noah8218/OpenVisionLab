# OpenVisionLab Runtime Data Root v1 Contract

Updated: 2026-07-30 KST

## Purpose

Release installation files must be immutable. Operators must be able to save
Recipes, settings, logs, captures, qualification evidence, and reusable UI
state without installing OpenVisionLab into an operator-writable executable
folder.

This contract defines path ownership and one-time adoption of data from the
former portable layout. It does not define an installer, account model,
multi-user database, network share protocol, cloud sync, or equipment data
platform.

## Root Selection

Release precedence:

1. If `OPENVISIONLAB_DATA_ROOT` contains an absolute path outside the
   installation directory, use that path.
2. Otherwise use `%LOCALAPPDATA%\OpenVisionLab`.
3. If the selected root cannot be created or safely migrated, stop startup
   with a visible Korean/English error. Never fall back to the installation
   directory or an empty temporary Recipe workspace.

Debug builds retain their build-output directory by default so developer
workspaces and existing Debug evidence remain isolated. Debug smoke tests may
set `OPENVISIONLAB_DATA_ROOT` to a task-specific absolute path.

The resolved root is published to the current process as
`OPENVISIONLAB_DATA_ROOT_RESOLVED`. Logging receives
`OPENVISIONLAB_LOG_ROOT`. These resolved variables are runtime plumbing, not
operator configuration inputs.

## Owned Writable Data

The data root owns:

- `CONFIG`: localization, account/device settings, UI placement, recent Tool,
  Image Compare state, and PropertyGrid/native Tool settings;
- `RECIPE`: Recipe XML, Pipeline XML, templates, validation sets, run history,
  drawings, batch evidence, and saved sample copies;
- `QUALIFIED_RECIPE`: qualified snapshot payload and lifecycle evidence;
- `Log`: application logs, dumps, and diagnostic logs;
- `CAPTURE`, `Image`, and `TEST`: captures, generated inspection images, and
  retained test/runtime configuration;
- `CACHE`: regenerable Learn HTML and future bounded caches;
- `SYSTEM.xml` and legacy root `VISION.xml` when present.

The installation root owns executable content only: EXE, DLLs, runtime config,
licenses/notices, deployment instructions, and packaged read-only assets.

## Legacy Portable Migration

When Release first uses a data root different from the installation root:

1. scan the named legacy writable directories and root configuration files;
2. copy missing files to the same relative location under the data root;
3. never overwrite an existing target;
4. retain both source and target when content conflicts;
5. never delete or move the source;
6. write `data-root-migration-v1.txt` with source, target, copied rows,
   same-content rows, conflicts, counts, timestamps, and status;
7. write `.incomplete` evidence and stop startup if any required copy fails;
8. do not repeat a completed migration on later launches.

Existing `%LOCALAPPDATA%\OpenVisionLab` files from the older flat layout are
copied, without overwrite or deletion, into their v1 owners:

- `recent-native-tool.txt` -> `CONFIG\UI`;
- `image_compare_last_directory.txt` -> `CONFIG`;
- `Logs\property-grid-editors.log` -> `Log\Diagnostics`;
- `LearnHtml` -> `CACHE\LearnHtml`.

Their durable record is `data-layout-migration-v1.txt`.

## Relative Dependency Compatibility

Newly imported Recipe template paths remain portable `RECIPE\...` values, but
their base is now the data root.

Resolution order for a relative template or pattern is:

1. selected data root;
2. installation root, for read-only packaged compatibility;
3. existing current-working-directory fallback used by repository catalog
   execution.

Absolute operator-selected paths remain absolute. Qualification runtime DLL
fingerprints continue to resolve from the installation root.

## Log Contract

log4net automatic assembly initialization is disabled. The application resolves
the data root first, then configures every file appender against the selected
`Log` directory. A legacy log path inside the installation root is rebased to
the data-root default. An explicit absolute log path outside the installation
root remains supported.

No zero-byte bootstrap logs, dumps, or diagnostic logs may be created in the
installation directory.

## Operator Workflow

Normal Release use requires no setup:

1. install or extract immutable application files;
2. launch OpenVisionLab;
3. the per-user data root is selected automatically;
4. legacy portable data is copied once if present;
5. the operator continues with the restored Recipe and settings;
6. migration details remain inspectable in the data root.

An administrator who requires a controlled shared or redirected writable
folder sets `OPENVISIONLAB_DATA_ROOT` before launch. The path remains visible
in the application log and deployment manifest. Changing that deployment
value selects a different data scope; OpenVisionLab must not silently merge
unrelated roots.

## Acceptance Matrix

| ID | Case | Required result |
| --- | --- | --- |
| D1 | Release without override | `%LOCALAPPDATA%\OpenVisionLab` selected |
| D2 | Absolute external override | exact path selected |
| D3 | Relative override | startup blocked |
| D4 | Release override inside installation | startup blocked |
| D5 | First launch with legacy data | missing files copied, source retained |
| D6 | Existing identical target | target retained, reported as same |
| D7 | Existing different target | target wins, conflict reported |
| D8 | Migration copy failure | `.incomplete` report and startup blocked |
| D9 | Second equivalent launch | same root/state restored, migration not repeated |
| D10 | Recipe/CONFIG/Qualified/Log writes | all occur below selected data root |
| D11 | Template relative round trip | data-root `RECIPE\...` resolves exactly |
| D12 | Installation inventory | no added, removed, or modified files |
| D13 | Property/Recipe persistence regressions | pass with task-specific data root |
| D14 | Restore side effects | zero automatic Preview/Run/layer/routing changes |

## Release Verification

The clean Release package gate must:

- seed a copied installation with representative legacy CONFIG, RECIPE, and
  QUALIFIED_RECIPE files;
- seed one conflicting target in an external data root;
- hash the full installation inventory;
- launch with `OPENVISIONLAB_DATA_ROOT` set to the external root;
- verify copied files, conflict retention, migration report, SYSTEM config,
  and data-root logs;
- verify the installation inventory and hashes are unchanged;
- launch a second time and verify selected state is retained.

Canonical command:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\VerifyReleaseCandidate.ps1
```

## Boundaries

- `%LOCALAPPDATA%` is per Windows user. Shared-station or service-account
  deployment requires an administrator-selected absolute data root and an
  explicit permissions/backup policy.
- The migration is copy-only and intentionally leaves the old portable data
  for rollback/manual archive. Installer cleanup must not delete it
  implicitly.
- This contract does not encrypt Recipes, define access control, or qualify
  network-share concurrency.
- This contract is a prerequisite for `Program Files` installation, not proof
  of installer, signing, update/rollback, uninstall, SBOM/legal approval, or
  commercial GA.
