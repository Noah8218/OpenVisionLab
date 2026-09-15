# OpenVisionLab WPF runtime environment matrix — 2026-09-11

## Status

Complete for the current-environment recording boundary. This slice did not
change product source, display settings, Recipe/XML contracts, or the
Dev/Original boundary. It records which alternate runtime rows can be executed
on this workstation without waiting for user or hardware changes.

## Baseline

| Item | Value |
| --- | --- |
| Repository | `C:\Git\2D\Dev` |
| Branch | `codex/public-sample-ux-docs` |
| Commit | `0a77e60e444b12757565ee977216a994446b53a6` |
| Target framework | `net8.0-windows7.0` |
| Product version | `2.2.0-dev.2` |
| OS | Windows 11 Pro `10.0.26100`, build `26100` |
| Evidence | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\wpf-runtime-environment-20260911` |

## Recorded environment

The probe used the current Windows monitor topology and `shcore.dll`
`GetDpiForMonitor`; it did not change display settings or start another product
run.

| Monitor | Bounds | Working area | DPI |
| --- | --- | --- | --- |
| `\\.\DISPLAY1` primary | `{X=0,Y=0,Width=2560,Height=1440}` | `{X=0,Y=0,Width=2560,Height=1392}` | 96 DPI / 100% |
| `\\.\DISPLAY2` non-primary left | `{X=-1920,Y=365,Width=1920,Height=1080}` | `{X=-1920,Y=365,Width=1920,Height=1032}` | 96 DPI / 100% |

The supported verification rows are 100%, 125%, 150%, 175%, and 200%. Only
100% is available in the current session; 125%, 150%, 175%, and 200% remain
unverified. One-monitor and headless conditions are not available in this
session. The current Windows user has light system/app theme settings, but the
inspected product Shell path has no runtime alternate-theme switch; the global
`OpenVisionLabWpfTheme.xaml` imports the MaterialDesign Light dictionary and
the product chrome uses its existing semantic brushes.

This is an environment limitation, not a reason to invent a theme provider or
to change the product theme contract. Alternate rows require a supported test
machine, a controlled Windows display-setting change, or a headless visual test
environment. No such external prerequisite is available in this run, so the
rows are recorded as `미검증` instead of being simulated.

## Existing runtime evidence boundary

The already completed 96% two-monitor physical qualification remains the
authoritative executed runtime result:

`docs/reports/OPENVISIONLAB_WPF_RUNTIME_QUALIFICATION_20260908.md`

It contains 17 PASS rows in both Debug and Release, dynamic selection of the
smaller left monitor, native dialog placement, Threshold/Pipeline navigation,
maximize/restore, and deterministic shutdown. This slice does not repeat that
run. The new evidence only records the current matrix and its missing rows.

## Junior developer assessment

**PASS for this slice.** A contributor can now distinguish an executed 100%
two-monitor qualification from environment rows that were not run. The next
runtime action is explicit and does not require reading old logs to discover
whether 125/150/175/200% or alternate themes were actually tested.

## Completion evidence

- `environment-matrix.json`: OS, branch, commit, target framework, product
  version, monitor bounds, working areas, DPI values, theme registry values,
  available rows, and missing rows.
- `environment-matrix.txt`: compact machine-readable result.
- `TestDocumentationIndex.ps1`: `DocumentationIndex=PASS IndexedPaths=290
  Routes=16 RootRedirects=102` after registering this report.
- No product source or tracked fixture changed.
- No user input, hardware, display-setting mutation, commit, push, release, or
  deployment was performed.

Boundary: `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요` for the
unavailable DPI/theme/topology rows and all camera/SDK/GPU and long-run native
operation conditions.
