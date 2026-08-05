# OpenVisionLab Responsive Shell Scale

Date: 2026-08-05 KST
Source baseline: Dev `8f047d8e`

## Result

OpenVisionLab now enlarges the complete shell when the available logical window
area is larger than the 1600 x 900 reference layout. The same scale is applied
to the title bar, navigation, toolbar, workspace, result guidance, log panel,
and status bar.

The scale is calculated from the smaller width or height ratio and is clamped
from 1.0 to 1.5. This keeps 1600 x 900 and smaller supported layouts unchanged,
uses an intermediate scale on a 1920 x 1032 work area, and reaches 1.5 at
2560 x 1392. WPF continues to own Windows DPI conversion; the application does
not replace or double-apply the operating-system display scale.

## Verification

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`
  passed with zero warnings and zero errors.
- Screenshot smoke passed at 1600 x 900 with scale 1.0.
- Screenshot smoke passed at 2560 x 1392 with scale 1.5.
- Maximized screenshot smoke passed at 1920 x 1032 on the non-primary leftmost
  monitor. Its monitor lookup now validates the actual hosting monitor instead
  of incorrectly comparing with the primary monitor.
- Compact tool-rail screenshot smoke passed at 1600 x 900.
- `OpenVisionReadinessCheck` passed all 13 contracts.
- The current built EXE was opened, moved to the leftmost monitor, maximized,
  and inspected without running Preview/Run or changing any layer or route.
  The window rectangle was `-1920,365` with size `1920 x 1032`, matching the
  leftmost monitor work area.

## Evidence

- Before, 2560 x 1392:
  `D:\OpenVisionLab-TestData\OpenVisionLab\ui_scale_20260805\before\wpf_shell_host_window_large_workspace.png`
- After, 2560 x 1392:
  `D:\OpenVisionLab-TestData\OpenVisionLab\ui_scale_20260805\after\wpf_shell_host_window_large_workspace.png`
- After, 1600 x 900 reference layout:
  `D:\OpenVisionLab-TestData\OpenVisionLab\ui_scale_20260805\after\wpf_shell_host_window_chrome.png`
- After, 1920 x 1032 maximized layout:
  `D:\OpenVisionLab-TestData\OpenVisionLab\ui_scale_20260805\after_maximized\wpf_shell_host_window_maximized.png`
- After, compact tool rail:
  `D:\OpenVisionLab-TestData\OpenVisionLab\ui_scale_20260805\after_compact\wpf_shell_host_tool_rail_compact.png`

## Closure

```text
Status: Complete
Scope: Responsive whole-shell scaling for large logical work areas and monitor-aware maximized screenshot validation
Acceptance criteria: 1600 x 900 remains 1.0; 1920 x 1032 scales proportionally; 2560 x 1392 reaches 1.5; title bar and shell remain visible; compact layout remains valid
Verification: Debug build 0 warnings/0 errors; reference, large, maximized, and compact screenshot smoke passed; readiness 13/13 passed; current EXE leftmost-monitor smoke passed
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab\ui_scale_20260805 and this report
Boundary / next dependency: This proves the tested 1600 x 900, 1920 x 1032, and 2560 x 1392 layouts; final comfort on a different physical monitor still depends on operator review
```
