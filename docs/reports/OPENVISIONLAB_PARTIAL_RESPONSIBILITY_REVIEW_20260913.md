# OpenVisionLab 2D Partial Responsibility Review

Updated: 2026-09-13 KST  
Scope: current `C:\Git\2D\Dev` source and the 59 compiled `partial` declarations

## Decision

The 59 declarations were reviewed as responsibility boundaries. They were not
mechanically merged or deleted. A `partial` declaration remains when it is a
generated/XAML/designer contract, a native control composition boundary, a
test-only host, or a cohesive View whose remaining methods only mutate its own
visual tree. A declaration is reopened only when a concrete responsibility can
move to an existing owner with an explicit call path and lifetime.

The first proven responsibility conflict in this review was file-backed pattern
image decoding in the two ROI editor windows. Both Views now use the existing
`OpenVisionBitmapImagePreviewFactory.LoadBitmap` owner. The Views still own ROI
interaction state, control events, preview assignment, and source image release.

## Evidence-based inventory

The source inventory contains 59 compiled declarations and two literal smoke
contract matches. The row-level review, including caller, mutable-state writer,
lifetime/release owner, binding/public contract, and disposition, is stored at:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\p1-view-boundaries-20260913\m4-partial-audit\partial-structure-review.csv`

The latest static audit reports:

```text
REFACTOR_AUDIT=PASS|CSharpFiles=826|XamlFiles=60|PartialDeclarations=59|PartialTextMatches=2|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0
```

The inventory is grouped as follows:

| Group | Count | Decision |
| --- | ---: | --- |
| Generated, designer, or framework contracts | 27 | Retain. The generator, XAML loader, WinForms designer, or Settings infrastructure owns the contract. |
| Small XAML control contracts with no non-UI signal | 14 | Retain. No independent state or lifetime owner is present. |
| Learn topic Views with existing topic presenters | 9 | Retain the XAML partial. The presenter owns lesson state/decisions; the View owns controls and timer visualization. |
| Shell composition root | 1 | Retain `OpenVisionShellHostView`. It creates and releases collaborators; splitting by line count would obscure ownership. |
| Pipeline Review View | 1 | Retain. Result projection, selection, and WPF controls are already bounded by the ViewModel, Document, image-resource owner, render presenters, and layout controller. |
| ROI/template editor Views | 2 | Retain the XAML partial after moving file-backed pattern decode to the existing image factory. |
| ImageCanvas native host adapter | 1 | Retain. The View attaches/detaches the native child and dialog/context hosts; `RoiImageCanvasViewModel` owns native image/timer lifetime. |
| Layer viewer View | 1 | Retain. Existing presenter/factory and the View preserve Bitmap clone/dispose ownership. |
| Recipe evidence View | 1 | Retain after moving file-backed decode to the existing image factory. |
| Line Tool View | 1 | Retain after moving property persistence to `LineToolPresenter` and the existing composition factory. |
| Smoke-only WPF host | 1 | Retain as test infrastructure; it is outside product ownership. |

## Reopened responsibility slice

### ROI/template image decode

Before the change, both `OpenGlTemplateEditorWindow.xaml.cs` and
`RoiEditorWindow.xaml.cs` directly checked a path and constructed
`System.Drawing.Bitmap`. That made the XAML code-behind responsible for file
access, native decode error behavior, and the visual state update.

The call path is now:

```text
PropertyGridImageEditorService
  -> OpenGlTemplateEditorWindow / RoiEditorWindow
  -> OpenVisionBitmapImagePreviewFactory.LoadBitmap(path, "pattern")
  -> View assigns a frozen preview and keeps its existing ROI state
```

`OpenVisionBitmapImagePreviewFactory` remains the stream/Bitmap owner. The
temporary `Bitmap` is disposed by the existing `using` scope in each View after
the preview has been copied to a WPF image source. No public constructor,
XAML name, dialog result, ROI coordinate contract, or template extraction
contract changed.

Focused evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\p1-view-boundaries-20260913\m4-template-editor-image\source-owner-check.txt`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\p1-view-boundaries-20260913\m4-template-editor-image\template-editor-image-boundary-contract.txt`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\p1-view-boundaries-20260913\m4-template-editor-image\phase-summary.txt`

## Why the remaining declarations were not extracted

`ImageCanvasControl.cs` and its designer file share the SharpGL context and
native release order; moving one set of fields would split a single native
lifetime. `OpenVisionShellHostView.xaml.cs` is the composition root, and the
existing Recipe, Workspace, Pipeline, Tool, Review, and lifecycle owners are
already created and disposed there. `OpenVisionPipelineReviewView.xaml.cs`
retains only the result-to-control projection after its image resources, layout,
rendering, ViewModel, and Document owners were inspected. Learn Views already
delegate lesson decisions to topic presenters. The remaining small XAML files
contain only namescope, binding, control-event, or visual lifecycle code.

These are retained decisions, not an assertion that every file is easy to read.
Reopening any retained owner requires a new reproducible defect, failed
completion criterion, or changed dependency/lifetime boundary. A future change
must identify the intended owner, mutable-state writer, release owner, public or
binding contract, and focused check before editing.

## Verification boundary

- Source inventory and structural disposition: completed for all 59 declarations.
- Refactor audit: passed with 59 declarations and zero project cycles.
- `VisionRecipeRunnerSmoke` Debug build: passed with 0 warnings and 0 errors.
- ROI/template image boundary contract: passed 3/3.
- WPF visual theme/layout/DPI/monitor/input, GPU rendering, camera/SDK, and
  long-running native shutdown: not run in this source/build slice.

