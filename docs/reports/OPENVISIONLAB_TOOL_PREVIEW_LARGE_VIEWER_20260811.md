# Tool Preview Large Viewer

Date: 2026-08-11 KST
Repository: `C:\Git\OpenVisionLab_Dev`

## Outcome

Tool View input/output images can be inspected in a reusable large viewer by
double-clicking a non-empty preview. The implementation reuses the existing
`OpenVisionLayerViewerView` and themed floating window; it does not introduce a
second viewer framework.

## Operator Contract

- Single-input Tools expose Input and Output. Arithmetic exposes Input A,
  Input B, and Output.
- Double-click keeps the inline fit-reset behavior and opens the exact routed
  layer in one 960x720 resizable viewer.
- Repeated role selection reuses the same viewer. Explicit Preview refreshes
  an open Output viewer.
- The viewer closes when the owning Tool changes or closes.
- The title is localized live and shows Tool, role, and layer.
- Opening or switching the viewer does not execute Preview/Run, create/select
  layers, change the active layer, or mutate routes.

## Current-Build Evidence

Physical artifact root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\artifacts\tool_preview_popout_20260811`

Before:
`before\OpenVisionLab_LineSignal_ParametersRetained.png`

After:

- `after\OpenVisionLab_ToolPreview_Input_Large.png`
- `after\OpenVisionLab_ToolPreview_Output_Large.png`
- `after\OpenVisionLab_ToolPreview_Docked_Output_Large.png`
- `after\OpenVisionLab_ToolPreview_Arithmetic_InputB_Large.png`
- `after\ToolPreview_Output_Edge.png`
- `after\ToolPreview_Output_Measure.png`
- `after\report.txt`

The actual EXE report records `PASS` for Input/Output window reuse, explicit
Preview refresh, floating/docked access, Arithmetic Input A/B/Output, live
Korean/English localization, and zero execution/layer/route side effects. It
also records `\\.\DISPLAY2`, its dynamic bounds, and intersecting shell, Tool,
and viewer rectangles.

Visual inspection found and corrected one interim mismatch where the custom
title bar retained Input A while the viewer content had switched to Input B.
The final screenshots show matching titles, themed chrome, visible dimensions
and status, and no clipped or overlapping controls.

## Verification

Commands executed against the current Dev source:

```powershell
dotnet build "src\OpenVisionLab\OpenVisionLab.csproj" -c Debug -p:Platform="Any CPU" -p:OpenVisionLabEnableEmbeddedSmokeRunner=true

Start-Process "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe" `
  -ArgumentList @('--smoke','tool-preview-popout','--output', `
  'D:\OpenVisionLab-TestData\OpenVisionLab_Dev\artifacts\tool_preview_popout_20260811\after') `
  -Wait

powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunUiScreenshotSmoke.ps1 `
  -Configuration Debug -Platform "Any CPU" `
  -OutputDir "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\artifacts\tool_preview_popout_20260811\after\focused-ui-smoke" `
  -Targets "wpf_shell_host_line_measure_tool,wpf_layer_selection_arithmetic_tool,wpf_tool_window_dock_float_cycle,localization_catalog_contract_check" `
  -TimeoutSeconds 180

dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"

dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj `
  -c Debug -- "C:\Git\OpenVisionLab_Dev"
```

Results available at this checkpoint:

- Embedded actual-EXE build: 0 warnings, 0 errors.
- Actual EXE `tool-preview-popout`: PASS.
- Four focused current-source UI targets: PASS; `layout=0`, `text=0`,
  `internal=0` for every target.
- Full Debug solution build: 0 warnings, 0 errors.
- Readiness contract: 13/13 PASS.

## Original Repository Verification

The reviewed Dev changes were applied to `C:\Git\OpenVisionLab` without an
approved deviation. All 29 changed file contents matched after EOL
normalization. The original repository independently passed:

- Embedded actual-EXE build: 0 warnings, 0 errors.
- Actual EXE `tool-preview-popout`: PASS for floating/docked Line,
  Arithmetic Input A/B/Output, explicit refresh, Korean/English switching,
  monitor placement, and zero Preview/layer/route side effects.
- Full Debug solution build: 0 warnings, 0 errors.
- Readiness contract: 13/13 PASS.

Original actual-EXE evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\tool_preview_popout_port_20260811`.

The reviewed 29-file batch is pushed in Dev `5134e43c` and original
`32bc70c`.

## Boundary

This proves the shared current Tool Preview path with actual-EXE Line and
Arithmetic coverage plus focused docking/localization checks. It does not
change algorithm results, recipe persistence, or pipeline execution.
