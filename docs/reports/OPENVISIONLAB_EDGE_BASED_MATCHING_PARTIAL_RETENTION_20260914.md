# OpenVisionLab Edge Based Matching / Auto MPoint Partial Retention

Updated: 2026-09-14 KST  
Issue: `PL-0037`  
Status: `Complete` — source-only no-change structural audit; no production source change

## Scope and decision

This slice reviewed the `EdgeBasedMatchingToolWpfView.xaml.cs` and
`AutoMPointTeachingPanel.xaml.cs` XAML `partial` family from the caller through
the Auto MPoint teaching workflow, report export, binding/test facade, and
disposal path. The required `partial` declarations remain. They are generated
XAML composition boundaries, not a hidden business-state split.

No production split was justified. `EdgeBasedMatchingToolWpfView` only composes
the existing single-input controller, verification guide, and Auto MPoint panel;
`AutoMPointTeachingPanel` only exposes named controls and localization lifetime;
and `AutoMPointTeachingController` already owns the teaching workflow's mutable
state, event wiring, dialog selection, algorithm invocation, and disposal. The
existing `AutoMPointHtmlReportExporter` remains the concrete report file-I/O
owner. Adding another ViewModel, wrapper, interface, or Partial would duplicate
state or hide the existing owner rather than create an independent state,
lifetime, or test seam.

## Owner map and call path

| Concern | Current owner | Intended owner | Evidence |
| --- | --- | --- | --- |
| XAML namescope and tool-content composition | `EdgeBasedMatchingToolWpfView` | same View Partial | `InitializeComponent`, `Grid.SetRow`, guide/panel composition |
| Property/preview facade | `VisionToolSingleInputMatchingToolController<EdgeBasedMatchingProperty>` | same existing controller | `AttachPropertyToolController`, `CreateProperty`, preview delegates |
| Visible Auto MPoint controls/localization | `AutoMPointTeachingPanel` | same XAML View Partial | named controls, automation IDs, `VisionToolLanguageChangeController` |
| Candidate/source/representative-image state | `AutoMPointTeachingController` | same concrete controller | `sourceBitmap`, `representativeImagePaths`, revisions and analysis definitions |
| Teaching event workflow and lifetime | `AutoMPointTeachingController` | same concrete controller | button/list subscriptions and deterministic `Dispose` unsubscriptions |
| Report file output | `AutoMPointHtmlReportExporter` | same existing exporter | `TryExport`, temporary HTML write, atomic move |
| Tool construction | `OpenVisionNativePropertyGridToolFactory` + `VisionToolCompositionService` | same composition owners | factory -> ViewModel -> presenter -> View |
| Base tool release | `VisionToolSingleInputPropertyToolViewBase` | same base lifetime owner | `DisposeToolResources()` then controller disposal |

The shortest reading order is:

```text
OpenVisionNativePropertyGridToolFactory.CreateEdgeBasedMatching
  -> VisionToolCompositionService.CreateEdgeBasedMatchingToolViewModel
  -> EdgeBasedMatchingToolWpfView
  -> AutoMPointTeachingPanel + AutoMPointTeachingController
  -> AutoMPointHtmlReportExporter / matching controller / base lifetime
```

The runtime path for teaching is:

```text
Shell selects Edge Based Matching
  -> EdgeBasedMatchingToolWpfView composes guide + AutoMPointTeachingPanel
  -> AutoMPointTeachingController handles Analyze/Use Pattern/Report
  -> existing matching controller updates property/preview
  -> AutoMPointHtmlReportExporter writes the selected report
```

## Contract and boundary findings

- The Edge Based Matching View Partial has no `SaveFileDialog`, `OpenFileDialog`,
  `System.IO`, `AutoMPointTool` construction, report exporter call, or template
  image save call. Its mutable state is limited to composition collaborators and
  guide/panel visibility.
- The Auto MPoint Panel Partial has no algorithm, candidate, file, or persistence
  state. Its XAML automation IDs and internal control facade are preserved.
- The controller is intentionally a responsibility-oriented WPF workflow
  adapter, not a ViewModel. It owns the mutable teaching session and couples it
  to the named panel controls; moving only a subset would create a second state
  owner. A future dialog-policy requirement would be a separate, explicit seam
  task and is not implied by the XAML `partial` declaration.
- Public/internal test facades remain delegated: report and representative-image
  calls go through the controller, while property and preview calls go through
  the existing matching controller.

## Verification

Focused source contract:

```text
EDGE_BASED_MATCHING_PARTIAL_BOUNDARY_CONTRACT=PASS|checks=8
```

The result passed in both Debug and Release. The contract checks the composition
owner, absence of direct View file/algorithm coupling, panel binding contract,
controller state/event ownership, test facade delegation, factory/base lifetime,
and existing report exporter ownership.

Builds and runtime smoke:

- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj --configuration Debug --no-restore --nologo` — 0 warnings, 0 errors.
- Same command with `--configuration Release` — 0 warnings, 0 errors.
- `PipelineViewerScreenshotSmoke` x64 Debug build — 0 errors, 2 pre-existing `MSB3270` MSIL/AMD64 warnings.
- Dynamic monitor detection before EXE launch: one monitor, `\\.\DISPLAY2`, bounds `1920x1080`, working area `1920x1032`; the reported single screen was used unchanged.
- A fresh smoke process window probe recorded one visible top-level window at
  `Left=208,Top=208,Right=1808,Bottom=1108` intersecting `\\.\DISPLAY2`
  (`IntersectingWindowCount=1`). Geometry evidence is
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\edge-based-mpoint-partial-retention-20260914\monitor-geometry-3\monitor-window-geometry.json`.
- `wpf_shell_host_edge_based_matching_tool` — `OK`, `1600x900` PNG.
- `wpf_shell_host_edge_based_matching_auto_mpoint` — `OK`, `430x330` PNG.

Fresh visual evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\edge-based-mpoint-partial-retention-20260914\wpf-smoke`.
The full Edge Based Matching shell, Auto MPoint expander, parameter grid,
verification guide, result review, and action row rendered without the smoke
layout/text/overlap assertions failing.

Repository gates also passed: `OpenVisionReadinessCheck` Debug/Release (13/13
each), `Invoke-RefactorAudit.ps1 -Verify`
(`CSharpFiles=831|XamlFiles=57|PartialDeclarations=54|PartialTextMatches=2|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`),
`TestDocumentationIndex.ps1`
(`IndexedPaths=306|Routes=17|RootRedirects=102`), PL-0037 JSON parsing, and
PL-0037 schema/closure validation (`valid v2`), and `git diff --check`
(existing LF/CRLF normalization notices only). The complete
phase record is `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\edge-based-mpoint-partial-retention-20260914\phase-summary.txt`.

Native file dialog click/file association, all themes, DPI 125/150/175/200%,
alternate monitor topology, camera/SDK/GPU, and long-running native runtime
remain unverified. Source conclusion: `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Completion record

```text
Status: Complete
Scope: Edge Based Matching / Auto MPoint XAML Partial owner audit; no production split
Acceptance criteria: owner/call path/state/lifetime/public contract documented; source contract 8/8 Debug+Release; focused WPF smoke passed
Verification: VisionRecipeRunnerSmoke Debug/Release build+contract; PipelineViewerScreenshotSmoke x64 Debug build and two Edge Based Matching targets
Evidence: this report; .proofline/issues/PL-0037.json; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\edge-based-mpoint-partial-retention-20260914
Boundary / next dependency: broader WPF/theme/DPI/input/native/hardware qualification remains unverified; reopen only for a new requirement, defect, failed criterion, or changed dependency boundary
```
