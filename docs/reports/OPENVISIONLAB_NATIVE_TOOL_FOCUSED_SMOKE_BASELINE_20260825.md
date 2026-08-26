# OpenVisionLab Native Tool Focused Smoke Baseline

Date: 2026-08-25 KST  
Repository: `C:\Git\OpenVisionLab_Dev`  
Issue: `PL-0003`  
Status: Complete in Dev

## Scope

Stabilize the three named focused WPF smoke assertions that were stale after
the Native Tool lifetime verification. This is test-contract maintenance only;
no product UI or Tool runtime behavior was changed.

## Root causes and change

- Range Threshold asserted `Lower`, `Upper`, and `Drag Lower/Upper` through
  `TextBlock` traversal, although the marker and guidance are drawn by the
  custom signal plot. The smoke now pumps the opened overlay and checks the
  actual localized `VisionToolSignalInspectorView` text surface: Korean title,
  current-evidence badge, SHA-256 provenance, and the `Gray population` legend.
  Existing marker count/value, evidence identity, overlay/cue, marker commit,
  and Preview side-effect assertions remain in place.
- Basic Threshold no longer requires the three-second transient evidence cue
  to be visible at the assertion instant. It still requires the inspector to
  be closed while current evidence is retained; explicit open/close and later
  cue dismissal checks remain unchanged.
- EdgeBasedMatching Auto MPoint now restores `ShowAdvancedSettings` before
  checking the generated PropertyGrid rows, verifies the current Korean labels
  `고유 매칭 요구` and `최소 고유 점수 여유`, and confirms that this inspection
  does not trigger Preview. The analysis/application/report no-side-effect
  assertions for Preview, layers, active layer, and routing remain unchanged.

Changed file: `tools/PipelineViewerScreenshotSmoke/Program.cs`

## Acceptance criteria

| Criterion | Result | Current evidence |
| --- | --- | --- |
| C1. Range target verifies localized signal-inspector evidence without weakening marker/value/Preview checks. | PASS | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_threshold_iso1\ui_precheck_summary.json`, `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_threshold_iso2\ui_precheck_summary.json`, and combined `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_r3\ui_precheck_summary.json` |
| C2. Edge Auto MPoint verifies current generated PropertyGrid labels and no Preview/layer/active-layer/routing side effect. | PASS | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_edge_r2\ui_precheck_summary.json` and combined `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_r3\ui_precheck_summary.json` |
| C3. Basic Threshold passes repeatedly isolated and combined without transient-cue lifetime dependence. | PASS | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_basic_iso1\ui_precheck_summary.json`, `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_basic_iso2\ui_precheck_summary.json`, and combined `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_r3\ui_precheck_summary.json` |

## Verification

Commands run from `C:\Git\OpenVisionLab_Dev`:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` — passed
  with 0 warnings and 0 errors after the smoke-contract edit.
- `dotnet build "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU" --no-restore` — passed with 0 warnings and 0 errors after the edit.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 -SkipSolutionBuild -SkipRestore -Targets "wpf_shell_host_threshold_basic_tool,wpf_shell_host_threshold_tool,wpf_shell_host_edge_based_matching_auto_mpoint" -OutputDir "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_r2" -WpgCustomBuildEnabled false -FailOnWarn -TimeoutSeconds 420` — passed, 3/3 OK.
- The same combined command passed again in `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_r3` — 3/3 OK.
- The Basic Threshold target passed twice in isolated folders `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_basic_iso1` and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_basic_iso2`.
- The Range Threshold target passed twice in isolated folders `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_threshold_iso1` and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_threshold_iso2`.
- The EdgeBasedMatching Auto MPoint target passed in isolated folder `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_edge_r2`.
- `node "C:\Users\USER\.codex\skills\proofline-issue-ledger\scripts\issue-ledger.js" validate ".proofline\issues\PL-0003.json" --root ".proofline\issues"` — `PL-0003: valid v2`.

## Runtime evidence and boundary

Current-source WPF captures from the latest combined run are:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_r3\wpf_shell_host_threshold_basic_tool.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_r3\wpf_shell_host_threshold_tool.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0003_current_20260825_r3\wpf_shell_host_edge_based_matching_auto_mpoint.png`

The smoke uses the current-source offscreen WPF path; it is not an actual
`OpenVisionLab.exe` theme/DPI/monitor qualification. It proves only the named
focused smoke workflows and their assertions. The original repository was not
touched, and no commit, push, release, or deployment was authorized.

The durable completion record is `.proofline/issues/PL-0003.json`.
