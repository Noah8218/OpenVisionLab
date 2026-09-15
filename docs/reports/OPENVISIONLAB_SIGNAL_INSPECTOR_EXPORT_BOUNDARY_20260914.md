# Signal Inspector TSV export boundary

Date: 2026-09-14 (KST)  
Issue: `PL-0035`  
Scope: one scheduled Tool Partial refactor slice in `C:\Git\2D\Dev`

## Result

`VisionToolSignalInspectorView.xaml.cs` no longer owns the concrete
`VisionToolSignalEvidenceExporter` file-I/O call. The required XAML Partial keeps
the `SaveFileDialog` and the selected-path presentation flow, then invokes an
explicit `Action<VisionToolSignalEvidence, string>` configured by each existing
Tool composition owner. The existing exporter remains the TSV/path-validation
owner.

No new service, interface, manager, event bus, wrapper, or Partial was added.
The current evidence state, marker event, plot surface, localization, XAML
namescope, and Tool view lifetime remain unchanged.

## Owner and call-path proof

| Concern | Previous owner/path | Current owner/path | Mutable state/lifetime |
| --- | --- | --- | --- |
| Current signal evidence | `VisionToolSignalInspectorView.evidence` set by `ShowEvidence` and cleared by `ClearEvidence` | unchanged | Inspector remains the single mutable evidence writer; parent Tool View owns its XAML child lifetime and cleanup |
| TSV file output | Inspector called `VisionToolSignalEvidenceExporter.ExportTsv` directly from `ExportButton_Click` and `ExportForTest` | Inspector resolves the selected path and invokes `exportAction`; `ThresholdToolWpfView`, `SimplePreprocessToolWpfView`, and `LineToolWpfView` wire the existing exporter | `VisionToolSignalEvidenceExporter` remains stateless file/path/UTF-8 output owner |
| Save dialog | Inspector created and owned `SaveFileDialog` | unchanged; this remains View-specific presentation plumbing | Window owner is still `Window.GetWindow(this)`; no dialog lifetime moved |
| Marker/plot behavior | Inspector owns plot callbacks and raises `MarkerValueChangeRequested` to the parent Tool View | unchanged | `VisionToolSignalPlotSurface` remains the visual interaction/resource owner |

The resulting paths are:

```text
ThresholdToolWpfView / SimplePreprocessToolWpfView / LineToolWpfView
  -> XAML VisionToolSignalInspectorView
  -> SetExportAction(VisionToolSignalEvidenceExporter.ExportTsv)
  -> ExportButton_Click
  -> SaveFileDialog.ShowDialog(Window.GetWindow(this))
  -> ExportEvidence(selectedPath)
  -> existing VisionToolSignalEvidenceExporter.ExportTsv(evidence, path)
```

The deterministic smoke/test path uses the same final seam:

```text
Tool test facade -> signalInspector.ExportForTest(path)
  -> ExportEvidence(path)
  -> configured exporter action
```

## Contract and reading order

The XAML binding/public contract is unchanged: `VisionToolSignalInspectorView`
automation IDs, `MarkerValueChangeRequested`, `ShowEvidence`, `ClearEvidence`,
marker/navigation test facades, and the three parent Tool View facades remain.

Shortest code-reading order:

1. `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/SignalInspection/VisionToolSignalInspectorView.xaml.cs`
   — evidence presentation, SaveFileDialog, and explicit export seam.
2. `src/OpenVisionLab/UI/VisionTest/Wpf/ToolViews/ThresholdToolWpfView.xaml.cs`,
   `SimplePreprocessToolWpfView.xaml.cs`, and `LineToolWpfView.xaml.cs`
   — XAML composition and callback wiring.
3. `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/SignalInspection/VisionToolSignalEvidenceExporter.cs`
   — TSV metadata, path, and file output owner.
4. `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Preview/OpenVisionNativeToolPreviewExecutor.cs`
   and the Threshold/Simple Preprocess/Line preview factories — evidence producers.
5. `tools/VisionRecipeRunnerSmoke/SignalInspectorExportBoundaryContract.cs`
   — source ownership and call-path proof.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\signal-inspector-export-boundary-20260914`

- `SignalInspectorExportBoundaryContract` — Debug and Release passed 6/6.
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj` — Debug and Release passed with 0 warnings/errors.
- `dotnet build OpenVisionLab.sln` — Debug and Release passed with 0 warnings/errors.
- `PipelineViewerScreenshotSmoke` x64 Debug build — 0 errors; three pre-existing warnings (`CS8600` and two `MSB3270` architecture warnings) remain.
- WPF smoke on detected single monitor `\\.\DISPLAY2` (1920x1080, working area 1920x1032): `wpf_threshold_signal_good_bad_replay`, `wpf_line_signal_profile`, `wpf_shell_host_threshold_tool`, and `wpf_shell_host_line_tool` passed and produced fresh PNG evidence.
- `OpenVisionReadinessCheck`, `Invoke-RefactorAudit.ps1 -Verify`, `TestDocumentationIndex.ps1`, LLM index JSON parse, and `git diff --check` passed.
- The threshold/line screenshots exercised the existing Tool composition and visual surface without clipping in the captured states; the export dialog was not driven interactively.

## Boundary and remaining risk

Source and WPF smoke prove that all known production XAML composition owners
configure the seam and that the Tool views still load/render. They do not prove
clicking the native Save dialog, external file association, every theme/DPI/
keyboard/mouse state, or long-running native/hardware behavior. Those remain
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

Direct visual-smoke construction of `VisionToolSignalInspectorView` remains
presentation-only and intentionally does not configure an export action unless
the host supplies one; known production XAML parents all configure the existing
exporter before use.
