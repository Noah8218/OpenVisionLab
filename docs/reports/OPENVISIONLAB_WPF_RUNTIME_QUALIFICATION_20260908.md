# OpenVisionLab representative WPF runtime qualification — 2026-09-08

## Status

Complete for the independently verifiable representative 96% DPI desktop slice. This run did not change product source, Recipe/XML contracts, Preview/Run semantics, Layer/ImageSpace ownership, tracked samples, or the Dev/Original boundary. The evidence harness and all generated artifacts remain on `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-wpf-runtime-20260908`.

## Scope and acceptance

The existing `OpenVisionLab.exe` Debug and Release binaries were exercised through one physical desktop workflow. The acceptance gate was 17 result rows with zero `FAIL` and zero `WARN` in each configuration, followed by a deterministic process shutdown. The harness enumerated monitors instead of assuming a display: this workstation selected the smaller non-primary monitor on the left, `\\.\DISPLAY2` (bounds `-1920,365,1920x1080`, working area `-1920,365,1920x1032`). The normal window was placed at `-1900,385,1600x900`; `GetDpiForWindow` reported 96 (100%). Maximize covered the selected working area.

## Exercised behavior

- startup, required AutomationId contract, normal placement, and initial screen capture;
- keyboard focus and search value readback (`THRESHOLD`);
- physical compact tool rail toggle;
- language popup open and close;
- Learn Window open, topic selection, and close;
- native `#32770` image dialog discovery and target-monitor placement, keyboard filename entry, Open acceptance, and loaded workspace status `도킹 레이어 1개` using `Sample\\Contour.jpg`;
- maximize/restore;
- Threshold navigation with selected-tool readback `스레시홀드 (선택됨)`;
- Pipeline navigation with selected-tool readback `파이프라인 (선택됨)`;
- top-window inventory and application shutdown.

## Verification evidence

Evidence root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-wpf-runtime-20260908`.

- `debug\\qualification-results.json`: 17 PASS, 0 FAIL, 0 WARN; application exit code 0.
- `release\\qualification-results.json`: 17 PASS, 0 FAIL, 0 WARN; application exit code 0.
- `debug\\01-initial.png` and `release\\01-initial.png`: app shell is visible on the selected monitor, with the empty-workspace first-use panel.
- `debug\\05-learn-window.png`: Learn Window and curriculum are visible and readable; `debug\\06-learn-topic-selection.png` records a second topic selection.
- `debug\\07-image-dialog-screen.png`: native image dialog is visible on the selected monitor before acceptance.
- `debug\\07-image-loaded.png`: the loaded sample and `도킹 레이어 1개` status are visible.
- `debug\\08-threshold-tool.png`: Threshold tool window and input preview are visible.
- `debug\\09-pipeline-view.png`: Pipeline review and input layer are visible.
- `debug\\10-maximized.png`: shell fills the selected working area.
- `work-contract.md` and `run-metadata.txt`: monitor rule, commands, aggregate counts, parser result, and no-duplicate continuation rule.

The harness parser returned `PARSER=PASS` and its SHA-256 is recorded in `run-metadata.txt`. No `OpenVisionLab` process remained after either run. Source build evidence remains the preceding Directory-policy slice: ImageCanvas and OpenVisionLab Debug/Release builds passed with zero warnings and zero errors; this runtime slice intentionally used those existing binaries and did not rebuild source.

## Boundary and junior readability

The runtime evidence proves the exercised 96% Debug/Release path on the current two-monitor workstation. It does not prove WPF behavior at 125%, 150%, 175%, or 200% DPI, alternate themes, one-monitor topology, or headless fallback; those rows were not run and remain explicit environment coverage gaps.

Junior developer assessment: **PASS for this slice**. The harness reads as `monitor selection -> app handle -> AutomationId contract -> physical action -> visible/result assertion -> shutdown`. Native dialog handling is isolated in the evidence-only `RuntimeQualificationDialogNative` helper, while the product source remains unchanged. Completed runtime evidence must not be repeated or repartitioned without a new failing result, changed contract, or an available unverified matrix row.

## Next priority

1. Expand WPF qualification to an available alternate DPI/theme or document the environment prerequisite | Recommended model: `gpt-5.5` | Reasoning effort: `medium`
2. Re-run the quantitative audit and close-out documentation only after the alternate matrix row is available | Recommended model: `gpt-5.4-mini` | Reasoning effort: `low`

No code owner from the completed ImageCanvas, Learn, Recipe/Pipeline, or 96% runtime slices should be split again without a demonstrated defect or responsibility conflict.
