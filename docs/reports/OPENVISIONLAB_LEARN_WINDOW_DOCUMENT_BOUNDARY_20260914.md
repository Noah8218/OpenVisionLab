# Learn Window document-action boundary

Date: 2026-09-14 (KST)  
Issue: `PL-0034`  
Scope: one scheduled Tool/Learn Partial refactor slice in `C:\Git\2D\Dev`

## Result

`OpenVisionLearnWindow.xaml.cs` no longer calls the concrete
`OpenVisionWorkspaceLearnDocumentService` directly. The required XAML Window
Partial emits the existing-style explicit `Action<string>` callback through
`SetOpenLearnDocumentAction`; the three known production composition owners
wire that callback to the existing document service before showing the Window.

No new service, interface, manager, event bus, wrapper, or Partial was added.
The Window still owns XAML namescope, topic selection, control projection,
animation timers, and its existing public threshold/practice/tool contracts.

## Owner and call-path proof

| Concern | Previous owner/path | Current owner/path | Mutable state/lifetime |
| --- | --- | --- | --- |
| Open current Learn document | `OpenVisionLearnWindow.OpenLearnDocsButton_Click` called `OpenVisionWorkspaceLearnDocumentService.OpenLearnDocumentFile` | Window resolves the selected filename and invokes `openLearnDocumentAction`; Shell/Tool composition wires the existing service | `OpenVisionLearnWindow` keeps topic/control state; service owns document resolution, HTML cache, and process launch |
| Open foundation document | Window called the concrete service with `LEARN_OPENCVSHARP_FOUNDATIONS.md` | Window invokes the same callback with the fixed filename; composition owns the concrete service binding | No change to filename or service behavior |
| Topic filename/practice path | Window delegates to `OpenVisionLearnTopicCatalog.Resolve` | unchanged | Catalog remains pure metadata owner |
| Learn Window lifetime | Shell/Tool/Threshold controllers create, own, and release the Window | unchanged; each controller now also wires the document callback | Existing `Closed` and threshold event cleanup remain unchanged |

The resulting call paths are:

```text
OpenVisionShellHostLearnWindowController
  -> OpenVisionLearnWindow.SetOpenLearnDocumentAction(
       OpenVisionWorkspaceLearnDocumentService.OpenLearnDocumentFile)
  -> OpenVisionLearnWindow.OpenLearnDocsButton_Click
  -> callback(filename)
  -> OpenVisionWorkspaceLearnDocumentService

VisionToolLearnWindowController / ThresholdToolLearnWindowController
  -> same explicit callback wiring
```

The Window's XAML binding/public contract is unchanged: the existing
automation IDs, `ApplyThresholdRequested`, `SetOpenPracticeSamplesAction`,
`SetOpenRelatedToolAction`, topic selection, and visual child names remain.

## Shortest code-reading route

1. `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/OpenVisionLearnWindow.xaml.cs`
   — XAML event adapter, topic/control projection, and callback boundary.
2. `src/OpenVisionLab/UI/Menu/Wpf/Shell/Commands/OpenVisionShellHostLearnWindowController.cs`
   — Shell composition and Learn Window lifetime.
3. `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/VisionToolLearnWindowController.cs`
   and `ThresholdToolLearnWindowController.cs` — Tool entry points and cleanup.
4. `src/OpenVisionLab/UI/Menu/Wpf/Workspace/Samples/OpenVisionWorkspaceLearnDocumentService.cs`
   — document path, HTML cache, and process-launch owner.
5. `tools/VisionRecipeRunnerSmoke/LearnWindowDocumentBoundaryContract.cs`
   — source-level ownership and call-path proof.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\learn-window-document-boundary-20260914`

- `LearnWindowDocumentBoundaryContract` — Debug and Release passed 6/6.
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj` — Debug and Release passed with 0 warnings/errors.
- `dotnet build OpenVisionLab.sln` — Debug and Release passed with 0 warnings/errors.
- `PipelineViewerScreenshotSmoke` x64 Debug build — 0 errors; three pre-existing warnings (`CS8600` and two `MSB3270` architecture warnings) remain.
- WPF smoke on the detected single monitor `\\.\DISPLAY2` (1920x1080, working area 1920x1032): `wpf_openvision_learn_foundation_contract` and `wpf_shell_host_learn_entry` both passed and produced fresh PNG evidence.
- The focused screenshot showed the Learn Window document/foundation/practice action row, topic list, guide content, and scrollable lesson panel rendered without clipping in the exercised state.
- `OpenVisionReadinessCheck`, `Invoke-RefactorAudit.ps1 -Verify`, `TestDocumentationIndex.ps1`, issue-ledger validation, and `git diff --check` are required final gates for this slice and are recorded in the phase summary.

## Boundary and remaining risk

The source and focused WPF smoke prove the callback wiring and exercised visual
state. They do not prove a real external browser/HTML launch, every Learn topic,
all keyboard/mouse states, every theme/DPI/monitor matrix, or long-running
native/hardware behavior. Those remain
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

All known production constructors wire the callback. Direct Window construction
used by visual smoke hosts intentionally exercises presentation without opening
an external document; no production caller outside the three controllers was
found by source search.
