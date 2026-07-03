# Native WPF Tool Extension Guide

This guide is the first stop when adding an OpenVisionLab tool.
The goal is to reduce repeated wiring without hiding tool-specific behavior.

## Choose One Lane

Use one of these lanes before adding a row to `OpenVisionNativeToolRegistry`.

| Lane | Use When | Main Entry Points |
| --- | --- | --- |
| PropertyGrid tool | The tool is an inspection/algorithm tool whose parameters should come from a property model. | `OpenVisionNativePropertyGridToolFactory`, `OpenVisionNativePropertyGridToolDocumentBuilder` |
| Custom UI tool | The tool needs a hand-built WPF parameter panel, but still creates one property object for preview/pipeline. | `OpenVisionNativeCustomToolFactory`, `ISingleInputPropertyVisionToolWpfView<TProperty>` |
| SimplePreprocess tool | The tool can share `SimplePreprocessToolWpfView` and only differs by parameter configuration, preview, and step creation. | `OpenVisionNativeSimplePreprocessDocumentFactory`, `OpenVisionNativeSimplePreprocessViewConfigurator`, `OpenVisionNativeSimplePreprocessPropertyFactory`, `OpenVisionNativeSimplePreprocessPreviewExecutor` |

Do not merge all lanes into one generic factory. A new tool should make its lane obvious.

## Shared Wiring

`OpenVisionNativeSingleInputToolDocumentBuilder` owns only the final single-input document wiring:

- selected WPF view
- preview delegate
- pipeline step delegate
- default output layer
- single-channel normalization flag

Keep tool-specific decisions in the lane factory. That makes the code easier to read and keeps special cases visible.

## State And Layer Contract

Every new native WPF tool must keep these contracts before it is considered complete:

1. PropertyGrid inspection tools must use repository-owned recipe property objects when available. Do not create fresh default property objects for an existing recipe tool path.
2. Custom WPF tools must persist teaching state through `OpenVisionNativeToolSettingsStore.CreateConfigName(toolName)` and recipe `*_ToolState.xml` files.
3. ROI, template, mask, and last parameter values are teaching state. They must survive tool close/reopen and must be visible again when the input image is loaded.
4. Input layer selection, output layer selection, preview status, run status, and Add Pipeline wiring should come from the shared runtime/controller path, not from copied view code-behind.
5. Output layer creation is explicit. Opening a tool, selecting ROI, loading input, or changing parameters must not automatically create/select/overwrite output layers unless that command is the direct purpose of the action.

Use `docs\VISION_TOOL_PROPERTY_GRID_POLICY.md` and `docs\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md` as the source of truth for these rules.

## PropertyGrid Tool Checklist

1. Add or reuse a property model in the OpenCV tool layer.
2. Add a ViewModel contract in `VisionToolViewModelContracts` if the tool needs summary, normalization, template status, or test hooks.
3. Add the ViewModel implementation under `0. UI/6) Vision Test/ViewModels`.
4. Add a factory method in `VisionToolCompositionService`.
5. Add or reuse a WPF view that preserves the PropertyGrid editor.
6. Add preview execution in `OpenVisionNativeToolPreviewExecutor`.
7. Add a creation method in `OpenVisionNativePropertyGridToolFactory`.
8. Add one row in `OpenVisionNativeToolRegistry`.
9. Add or update UI smoke coverage in `PipelineViewerScreenshotSmoke`.

The PropertyGrid editor must remain model-driven. Adding public properties and attributes to the property model should create the parameter UI.

## Custom UI Tool Checklist

1. Add a ViewModel contract and implementation for parameter state and normalization.
2. Add a Presenter if the view needs binding-friendly properties or commands.
3. Implement `ISingleInputPropertyVisionToolWpfView<TProperty>` on the WPF view.
4. Use `VisionToolSingleInputCustomToolRuntime` for layer selectors, previews, status, run, and pipeline buttons.
5. Add a creation method in `OpenVisionNativeCustomToolFactory`.
6. Use `CreateSinglePropertyToolDocument` unless the tool has special step/preview semantics.
7. Add one row in `OpenVisionNativeToolRegistry`.
8. Add or update UI smoke coverage.

Line is intentionally special because it has Edge, Measure, and Intersection paths plus paired properties.

## SimplePreprocess Tool Checklist

1. Add parameter UI setup in `OpenVisionNativeSimplePreprocessViewConfigurator`.
2. Add property mapping in `OpenVisionNativeSimplePreprocessPropertyFactory` if the tool creates a property model.
3. Add preview execution in `OpenVisionNativeSimplePreprocessPreviewExecutor`.
4. Add a descriptor in `OpenVisionNativeSimplePreprocessDocumentFactory`.
5. Add one row in `OpenVisionNativeToolRegistry`.
6. Add or update UI smoke coverage.

## Verification

Run at least:

```powershell
dotnet build .\OpenVisionLab.sln -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
```

Run the static UI contract check after adding or moving a native tool:

```powershell
dotnet run --project .\tools\VisionUiContractCheck\VisionUiContractCheck.csproj -c Debug -p:Platform=x64 -- .\bin\x64\Debug
```

This check must report `NativeToolNavigationContract=OK` and `NativeToolPrewarmContract=OK`. These gates catch missing `OpenVisionNativeToolRegistry` rows, shell navigation mismatches, duplicated menu entries, and prewarm policy drift.

For tool creation, layer routing, and warm-open performance:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Configuration Debug -Platform x64 -Targets "wpf_layer_selection_all_native_tools,wpf_tool_open_perf" -WpgCustomBuildEnabled false
```

Add focused smoke targets for tool-specific behavior, especially result review, template registration, ROI editing, or pipeline metadata.

Before marking the tool-addition task complete, update:

1. `docs\OPENVISIONLAB_COMPLETED_TRACKER.md` if the tool behavior is stable enough to freeze.
2. `docs\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md` if the tool introduces a user-visible rule that future LLM work must not casually change.
3. `docs\VISION_TOOL_PROPERTY_GRID_POLICY.md` if the tool adds PropertyGrid editor/order/visibility rules.

Before calling a tool UX fix complete, run one actual EXE pass as an operator would:

1. Launch `bin\x64\Debug\OpenVisionLab.exe`.
2. Confirm startup is workspace-first: no tool window is opened until the operator selects one.
3. Load an image from the visible workspace image-load button.
4. Select a tool, run a preview, and confirm the input image, output image, right-side layer/result list, and selected-layer preview all update.
5. Open one PropertyGrid-based algorithm tool, such as Blob, and confirm the parameter grid, layer selectors, and preview button are visible and clickable.

Keep this pass focused on the changed area. Do not expand it into a full regression suite unless the change touches shared shell, layer routing, or tool-window creation.
