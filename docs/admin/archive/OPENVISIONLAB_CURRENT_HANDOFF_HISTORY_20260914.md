# OpenVisionLab Current Project Handoff

Updated: 2026-09-14 KST

This is the compact live-status source for a new OpenVisionLab task. Read
`AGENTS.md`, `docs/README.md`, and `docs/LLM_DOCUMENT_INDEX.json` first, then
load only the route matching the task. The former 2,133-line cumulative handoff
is preserved at
`docs/admin/archive/OPENVISIONLAB_CURRENT_HANDOFF_HISTORY_20260830.md`; it is
historical evidence, not current-priority authority.

## Current user-requested Partial cycle — 2026-09-14

Status: `PL-0056 RESOLVED`; the user requested that Partial refactoring continue
on a recurring cycle. The single active heartbeat is
`openvisionlab-2d-partial`, configured for a ten-minute cadence and attached to
the current 2D thread. It is the only executor for this continuation; no second
automation was created.

The first resumed slice rechecked the next retained boundary,
`ImageCompareWindow.xaml.cs`, and moved its pure display-coordinate mapping
policy to the existing `ImageCompareViewModel.TryMapDisplayedPoint`. The Window
still owns required XAML namescope, `OpenFileDialog`, pointer/window chrome
events, public `LoadImages`, and ViewModel lifetime. `ImageCompareViewModel`
owns the numeric scale/letterbox/bounds policy; `UpdatePixelStatus` remains the
mutable status writer, and `ImageCompareImageResource`/
`ImageCompareSlotViewModel` remain the image resource owners. No new service,
interface, wrapper, or Partial was added.

Call path:
`ImageCompareWindow.SlotImage_MouseMove -> ImageCompareViewModel.TryMapDisplayedPoint
-> ImageCompareViewModel.UpdatePixelStatus -> ImageCompareSlotViewModel.Bitmap`.

Focused evidence is recorded in
[`OPENVISIONLAB_IMAGE_COMPARE_POINT_MAPPING_BOUNDARY_20260914.md`](../../reports/OPENVISIONLAB_IMAGE_COMPARE_POINT_MAPPING_BOUNDARY_20260914.md),
`.proofline/issues/PL-0032.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\image-compare-point-mapping-20260914`.
Debug/Release focused contracts passed 4/4 and Smoke builds passed with zero
warnings/errors. Remaining generated/XAML/designer/native/cohesive Partial
declarations must continue to be reopened only with a concrete owner conflict;
the goal is not a mechanical Partial count of zero. Full WPF visual/theme/DPI/
monitor/input and hardware/native runtime remain unverified.

The second resumed slice is `PL-0033`.
`ImageCanvasDirectoryPolicy` now owns image-name extraction and default PNG
save-name sanitization. `RoiImageCanvasViewModel` delegates both paths and no
longer imports or calls `System.IO` path APIs; its shared Mat/ROI/OpenGL/input/
timer state, XAML binding facade, and resource lifetime remain with the existing
owner because no independent state/lifetime/test seam was proven. The call path
and reading order are recorded in
[`OPENVISIONLAB_ROI_IMAGE_CANVAS_PATH_POLICY_BOUNDARY_20260914.md`](../../reports/OPENVISIONLAB_ROI_IMAGE_CANVAS_PATH_POLICY_BOUNDARY_20260914.md),
`.proofline/issues/PL-0033.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\roi-image-canvas-path-policy-20260914`.
The focused contract passed 6/6 in Debug and Release, the existing ROI boundary
contract passed 6/6 in both, and solution/Smoke builds passed with zero
warnings/errors. Full WPF visual/theme/DPI/monitor/input and hardware/native
runtime remain unverified.

The third resumed slice is `PL-0034`.
`OpenVisionLearnWindow.xaml.cs` no longer calls
`OpenVisionWorkspaceLearnDocumentService` directly. It now emits an explicit
`Action<string>` callback, and the Shell/Tool/Threshold Learn composition
controllers wire the existing document service before showing the Window. The
Window still owns XAML namescope, topic/control projection, animation timers,
threshold/practice/tool contracts, and lifetime; no new service/interface/
Partial was added. The owner map and call path are recorded in
[`OPENVISIONLAB_LEARN_WINDOW_DOCUMENT_BOUNDARY_20260914.md`](../../reports/OPENVISIONLAB_LEARN_WINDOW_DOCUMENT_BOUNDARY_20260914.md),
`.proofline/issues/PL-0034.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\learn-window-document-boundary-20260914`.
The focused source contract passed 6/6 in Debug and Release; the foundation
Learn and Shell Learn-entry WPF smoke targets passed on the detected single
monitor and produced fresh PNG evidence. External browser launch and full
topic/theme/DPI/input/native matrices remain unverified.

The fourth resumed slice is `PL-0035`.
`VisionToolSignalInspectorView.xaml.cs` still owns current evidence, plot/marker
presentation, XAML namescope, and the native `SaveFileDialog`, but it no longer
calls `VisionToolSignalEvidenceExporter` directly. Its UI and test export paths
invoke an explicit `Action<VisionToolSignalEvidence, string>` seam; the existing
Threshold, Simple Preprocess, and Line Tool composition owners wire the existing
stateless TSV exporter. No new service/interface/wrapper/Partial was added.

The owner map and call path are recorded in
[`OPENVISIONLAB_SIGNAL_INSPECTOR_EXPORT_BOUNDARY_20260914.md`](../../reports/OPENVISIONLAB_SIGNAL_INSPECTOR_EXPORT_BOUNDARY_20260914.md),
`.proofline/issues/PL-0035.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\signal-inspector-export-boundary-20260914`.
The focused source contract passed 6/6 in Debug and Release; the Tool
composition WPF smoke targets passed on the detected single monitor and
produced fresh PNG evidence. Native SaveFileDialog click/file association,
full theme/DPI/input matrices, and hardware/native runtime remain unverified.

The fifth resumed slice is `PL-0036`.
`MorphologyToolWpfView.xaml.cs` was rechecked as an unprotected Tool View
Partial and retained without production changes. The View already acts as a
thin XAML/composition adapter; `MorphologyToolPresenter` owns the property
facade, `MorphologyToolViewModel` owns mutable parameters/settings persistence,
`VisionToolMorphologyInteractionController` owns operation/shape interaction,
`VisionToolKernelSizeController` owns kernel input, and the existing
single-input base owns preview/lifetime release. No independent state/lifetime/
test seam justified a new split.

The owner map and no-change proof are recorded in
[`OPENVISIONLAB_MORPHOLOGY_TOOL_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_MORPHOLOGY_TOOL_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0036.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\morphology-tool-partial-retention-20260914`.
The focused source contract passed 6/6 in Debug and Release; the Morphology
layout and standalone Tool WPF smoke targets passed on the detected single
monitor. Full theme/DPI/input and camera/SDK/GPU/native runtime remain
unverified.

The sixth resumed slice is `PL-0037`.
`EdgeBasedMatchingToolWpfView.xaml.cs` and `AutoMPointTeachingPanel.xaml.cs`
were rechecked as the Edge Based Matching/Auto MPoint XAML Partial family and
retained without production changes. The Edge View remains the XAML/composition
adapter; the Panel remains presentation/localization only; the existing
`AutoMPointTeachingController` owns teaching state, event/dialog/algorithm
workflow; `AutoMPointHtmlReportExporter` owns report file output; and the
matching controller/factory/composition/base own property/preview/creation/
lifetime. No independent state/lifetime/test seam justified a new split.

The owner map and no-change proof are recorded in
[`OPENVISIONLAB_EDGE_BASED_MATCHING_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_EDGE_BASED_MATCHING_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0037.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\edge-based-mpoint-partial-retention-20260914`.
The focused source contract passed 8/8 in Debug and Release; the full Edge Based
Matching and Auto MPoint WPF smoke targets passed on the detected single monitor.
Native dialog click/file association and full theme/DPI/input/hardware/native
runtime remain unverified.

The seventh resumed slice is `PL-0038`.
`FeatureMatchingToolWpfView.xaml.cs` and its XAML were rechecked through the
factory/composition/ViewModel/shared matching runtime/base lifetime path and
retained without production changes. The View remains a thin XAML/composition
adapter; `FeatureMatchingToolViewModel` owns mutable property/template policy;
the shared matching controller/runtime own PropertyGrid, preview, result-review,
preset, event, language, and disposal state; and factory/base own creation and
release. No independent state/lifetime/test seam justified a new split.

The owner map and no-change proof are recorded in
[`OPENVISIONLAB_FEATURE_MATCHING_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_FEATURE_MATCHING_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0038.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\feature-matching-partial-retention-20260914`.
The focused source contract passed 9/9 in Debug and Release; the Feature
Matching WPF smoke target passed on the detected single monitor. Native dialog
click/file association and full theme/DPI/input/hardware/native runtime remain
unverified.

The eighth resumed slice is `PL-0039`.
`MatchingToolWpfView.xaml.cs` previously held the concrete sample template-path
resolution and the full common OpenCV/Matching property copy callback. The View
now keeps only the existing controller orchestration and delegates those policies
to the existing `VisionPipelineMatchingPropertyAdapter`. Template-path-before-
property-copy ordering, `AUTO_PREVIEW=false`, all field values, and defensive
ROI list copies are preserved. The ViewModel still owns mutable Matching state;
the shared matching runtime/controller owns PropertyGrid, preview, result-review,
event, and disposal state; factory/composition/base still own creation/release.

The owner map and refactor proof are recorded in
[`OPENVISIONLAB_MATCHING_TOOL_PARTIAL_BOUNDARY_20260914.md`](../../reports/OPENVISIONLAB_MATCHING_TOOL_PARTIAL_BOUNDARY_20260914.md),
`.proofline/issues/PL-0039.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\matching-tool-partial-boundary-20260914`.
The focused source contract passed 11/11 in Debug and Release; the Matching
WPF smoke and Recipe Fixture PropertyGrid sample-step smoke targets passed with
fresh screenshots and detected-monitor window probes. Full theme/DPI/input,
native dialog, camera/SDK/GPU, and long-running native runtime remain
unverified.

The ninth resumed slice is `PL-0040`.
`ThresholdToolWpfView.xaml.cs` previously coordinated the complete Threshold
teaching-suggestion workflow in its XAML Partial. The View now routes
Analyze/Use/Undo and evidence availability through the WPF-free
`ThresholdToolSuggestionController`. The existing
`VisionToolThresholdSuggestionSession` remains the suggestion/stale/Undo policy
owner, and `VisionToolThresholdInteractionController` remains the mutable
parameter and debounced Preview writer. XAML names, automation IDs, binding and
test facades, Learn flow, creation, and base disposal are unchanged.

The owner map and refactor proof are recorded in
[`OPENVISIONLAB_THRESHOLD_TOOL_PARTIAL_BOUNDARY_20260914.md`](../../reports/OPENVISIONLAB_THRESHOLD_TOOL_PARTIAL_BOUNDARY_20260914.md),
`.proofline/issues/PL-0040.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\threshold-suggestion-controller-20260914`.
The source contract passed 7/7 in Debug and Release; VisionRecipeRunnerSmoke
builds passed with zero warnings/errors; Threshold Tool and CVR-07 suggestion
WPF smoke passed with fresh screenshots and detected-monitor window probes.
Full theme/DPI/input, native dialog, camera/SDK/GPU, and long-running native
runtime remain unverified.

The tenth resumed slice is `PL-0041`.
`LineToolWpfView.xaml.cs` previously owned the complete common OpenCV/Line field
copy body inside `ApplySampleLinePair`. The View now delegates both Line A/B
projections to the existing `VisionPipelineLinePropertyAdapter`, which already
owns Line pipeline property creation and conversion. The View keeps the existing
sample callback order, purpose selection, presenter persistence seam, PropertyGrid
refresh, ROI overlay, summary, result cleanup, bindings, test facade, creation,
and base lifetime. PL-0027 persistence ownership was not reopened.

The owner map and refactor proof are recorded in
[`OPENVISIONLAB_LINE_TOOL_PARTIAL_BOUNDARY_20260914.md`](../../reports/OPENVISIONLAB_LINE_TOOL_PARTIAL_BOUNDARY_20260914.md),
`.proofline/issues/PL-0041.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\line-tool-partial-boundary-20260914`.
The source contract passed 4/4 in Debug and Release; VisionRecipeRunnerSmoke
builds passed with zero warnings/errors; Line Tool, Line Measure, Line
Intersection, and Recipe Line Pair WPF smoke passed with fresh screenshots and
detected-monitor window probes. Full theme/DPI/input, native dialog,
camera/SDK/GPU, and long-running native runtime remain unverified.

The eleventh resumed slice is `PL-0042`.
`SimplePreprocessToolWpfView.xaml.cs` was rechecked as an unprotected Tool View
Partial and retained without production changes. The View remains the required
XAML/composition adapter; `SimplePreprocessParameterController` owns dynamic
editor state, `SimplePreprocessTextPresenter` owns text projection, the existing
parameter-change/debounced-preview owners handle change scheduling, the property
factory and preview executor own property/algorithm/result policy, the document
factory owns settings persistence and document composition, and the base View
owns the final controller release. No independent state/lifetime/test seam
justified a new split.

The owner map and no-change proof are recorded in
[`OPENVISIONLAB_SIMPLE_PREPROCESS_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_SIMPLE_PREPROCESS_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0042.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\simple-preprocess-partial-retention-20260914`.
The source contract passed 9/9 in Debug and Release; VisionRecipeRunnerSmoke
builds passed with zero warnings/errors; Rotate/Scale, Simple Preprocess result
review, and Learn-button WPF smoke passed with fresh screenshots; and sequential
dynamic monitor/window probes passed for Rotate/Scale and result review. Full
theme/DPI/input, native dialog, camera/SDK/GPU, and long-running native runtime
remain unverified.

The next scheduled candidate is `MatchingLearnView.xaml.cs`, an as-yet-
unprotected Learn View Partial. Completed Image Compare, ROI Image Canvas, Learn document,
Signal Inspector, Morphology, Edge Based Matching/Auto MPoint, Feature Matching,
Matching, Threshold, and Line boundaries remain closed.

The twelfth resumed slice is `PL-0043`.
`AffineTransformToolWpfView.xaml.cs` was rechecked as an unprotected Tool View
Partial and retained without production changes. The View remains the required
XAML/controller adapter; `AffineTransformToolViewModel` owns mutable property and
summary state, the shared generic property controller owns PropertyGrid/layer/
preview/test interaction, `AffineTransformResultReviewPresenter` owns result
explanation, the existing preview executor owns algorithm/overlay execution, and
factory/composition/registry/base own persistence, creation, registration, and
release. No independent state/lifetime/test seam justified a new split.

The owner map and no-change proof are recorded in
[`OPENVISIONLAB_AFFINE_TRANSFORM_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_AFFINE_TRANSFORM_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0043.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\affine-transform-partial-retention-20260914`.
The source contract passed 9/9 in Debug and Release; VisionRecipeRunnerSmoke
builds passed with zero warnings/errors; Affine Transform WPF smoke passed with a
fresh screenshot; and the sequential dynamic monitor/window probe passed. Full
theme/DPI/input, native dialog, camera/SDK/GPU, and long-running native runtime
remain unverified.

The next scheduled candidate is another as-yet-unprotected Tool View/Learn View
Partial family. Completed Image Compare, ROI Image Canvas, Learn document,
Signal Inspector, Morphology, Edge Based Matching/Auto MPoint, Feature Matching,
Matching, Threshold, Line, Simple Preprocess, Affine Transform, Arithmetic, and
Filter boundaries remain closed.

The thirteenth resumed slice is `PL-0044`.
`ArithmeticToolWpfView.xaml.cs` was rechecked as the next unprotected Tool View
Partial and retained without production changes. The View remains the required
XAML/double-input adapter; `ArithmeticToolInteractionController` owns operation/
source/constant/offset editor state, event lifetime and settings projection,
`ArithmeticToolTextPresenter` owns localization and summary projection,
`ArithmeticToolPreviewController` owns debounced Preview/Offset scheduling, the
shared double-input controller/ViewModel/binder own layer/command/preview runtime,
and factory/document/registry/base own settings/routing, creation and release.
No direct persistence/dialog/file-I/O/OpenCvSharp/algorithm coupling or independent
state/lifetime/test seam justified a new split.

The owner map and no-change proof are recorded in
[`OPENVISIONLAB_ARITHMETIC_TOOL_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_ARITHMETIC_TOOL_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0044.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\arithmetic-tool-partial-retention-20260914`.
The source contract passed 9/9 in Debug and Release; VisionRecipeRunnerSmoke
builds passed with zero warnings/errors; Arithmetic Learn-button and layer
selection WPF smoke targets passed with fresh screenshots; and the sequential
dynamic monitor/window probe passed. Full theme/DPI/input, native dialog,
camera/SDK/GPU, and long-running native runtime remain unverified.

The fourteenth resumed slice is `PL-0045`.
`FilterToolWpfView.xaml.cs` was rechecked as the next unprotected Tool View
Partial and retained without production changes. The View remains the required
XAML/custom-tool adapter; `VisionToolFilterInteractionController` owns Filter
selection and mode panels, `VisionToolKernelSizeController` owns kernel input,
lock and presets, `FilterToolTextPresenter` and the parameter-guide binder own
presentation/help, `FilterToolViewModel`/presenter own mutable settings and
summary, shared single-input controller/runtime/base own layer/preview/result/
language lifetime, and composition/factory/document/registry own settings,
creation, routing, registration, and release. No direct persistence/dialog/
file-I/O/OpenCvSharp/algorithm coupling or independent state/lifetime/test seam
justified a new split.

The owner map and no-change proof are recorded in
[`OPENVISIONLAB_FILTER_TOOL_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_FILTER_TOOL_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0045.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\filter-tool-partial-retention-20260914`.
The source contract passed 9/9 in Debug and Release; VisionRecipeRunnerSmoke
builds passed with zero warnings/errors; focused Filter/Morphology WPF smoke and
the sequential dynamic monitor/window probe passed. Full theme/DPI/input,
native dialog, camera/SDK/GPU, and long-running native runtime remain unverified.

The fifteenth resumed slice is `PL-0046`.
`BlobToolWpfView.xaml.cs` was rechecked as the next unprotected Tool View Partial
and retained without production changes. The View remains the required
XAML/property-grid adapter; `BlobToolViewModel`/composition own mutable Blob
settings, normalization, summary, and snapshots, the generic property-grid
controller/runtime/presenter own binding, layer/preview/persistence/debounce and
release, area-verification and threshold-teaching presenters own teaching/review
policy, and factory/document/preview/overlay/registry/pipeline/base own
creation, algorithm/result routing, registration, step creation, and lifetime.
No direct persistence/dialog/file-I/O/OpenCV/Blob algorithm coupling or
independent state/lifetime/test seam justified a new split.

The owner map and no-change proof are recorded in
[`OPENVISIONLAB_BLOB_TOOL_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_BLOB_TOOL_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0046.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\blob-tool-partial-retention-20260914`.
The source contract passed 11/11 in Debug and Release; VisionRecipeRunnerSmoke
builds passed with zero warnings/errors; focused Blob shell/Learn WPF smoke and
the dynamic monitor/window probe passed. Full theme/DPI/input, native dialog,
camera/SDK/GPU, and long-running native runtime remain unverified.

The sixteenth resumed slice is `PL-0047`.
`ContourToolWpfView.xaml.cs` was rechecked as the next unprotected Tool View
Partial and retained without production changes. The View remains the required
XAML/property-grid adapter; `ContourToolViewModel`/composition own mutable
Contour settings, range/epsilon/thickness normalization, summary, and snapshots,
the generic property-grid controller/runtime/presenter own binding,
layer/preview/persistence/debounce and release, area-verification and
threshold-teaching presenters own teaching/review policy, and
factory/document/preview/overlay/registry/pipeline/base own creation,
algorithm/result routing, registration, step creation, and lifetime. No direct
persistence/dialog/file-I/O/OpenCV/Contour algorithm coupling or independent
state/lifetime/test seam justified a new split.

The owner map and no-change proof are recorded in
[`OPENVISIONLAB_CONTOUR_TOOL_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_CONTOUR_TOOL_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0047.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\contour-tool-partial-retention-20260914`.
The source contract passed 11/11 in Debug and Release; VisionRecipeRunnerSmoke
builds passed with zero warnings/errors; focused Contour shell/Learn WPF smoke
and the dynamic monitor/window probe passed. Full theme/DPI/input, native
dialog, camera/SDK/GPU, and long-running native runtime remain unverified.

The seventeenth resumed slice is `PL-0048`.
`BinaryLearnView.xaml.cs` was rechecked as the next unprotected Learn View Partial
and retained without production changes. The View remains the required
XAML/presentation adapter; `BinaryLearnPresenter` and
`OpenVisionLearnBinarySimulationModel` own fixed samples, simulation results,
stage decisions, formulas, explanations, and related-tool guidance,
`OpenVisionLearnWindow` owns topic selection and related-tool callback injection,
and the View owns only cell painting, topic visibility, callback forwarding, and
three DispatcherTimer attach/detach lifetimes. No direct
file-I/O/dialog/OpenCV/tool-creation/persistence coupling or independent
state/lifetime/test seam justified a new split.

The owner map and no-change proof are recorded in
[`OPENVISIONLAB_BINARY_LEARN_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_BINARY_LEARN_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0048.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\binary-learn-partial-retention-20260914`.
The source contract passed 9/9 in Debug and Release; VisionRecipeRunnerSmoke
builds passed with zero warnings/errors; focused Binary/Line Learn WPF smoke and
the dynamic monitor/window probe passed. Full theme/DPI/input, native related
tool click/file association, camera/SDK/GPU, and long-running native runtime
remain unverified.

The next scheduled candidate is another as-yet-unprotected Tool View/Learn View
Partial family. Completed Image Compare, ROI Image Canvas, Learn document,
Signal Inspector, Morphology, Edge Based Matching/Auto MPoint, Feature Matching,
Matching, Threshold, Line, Simple Preprocess, Affine Transform, Arithmetic,
Filter, Blob, Contour, Binary Learn, Foundation Learn, Grayscale Learn,
Layer/Recipe Learn, Geometry Learn, Metrics Acceptance Learn, and Verification
Guide, Double-input Shell, and the prior Matching Learn extraction boundaries
remain closed.

The twenty-fourth resumed slice is `PL-0055`.
`VisionToolParameterGuideView.xaml.cs` was rechecked as the next Tool View
Partial. The View remains the required XAML/content projection adapter and
related-property callback surface; `VisionToolParameterGuidePresenter` owns
selected object/property state and navigation, `VisionToolParameterGuideCatalog`
owns definitions/localization/applicability/fallback policy, the binder/host own
PropertyGrid events and presenter lifetime, and the sidecar/single-input shell
own floating-window lifetime, visibility and composition. No direct file I/O,
dialog, OpenCV, Recipe execution, persistence, algorithm, timer or independent
business-state seam justified a View split.

The slice did correct a concrete catalog boundary defect. When the global
PropertyGrid type-description provider hides an inactive dependent public
property, `VisionToolParameterGuideCatalog.GetPropertyDescriptor` now falls back
to a public-instance reflection descriptor after the normal `TypeDescriptor`
lookup. Attributes remain available to the existing localization/value path, and
PropertyGrid visibility policy does not move into the View. The owner map and
proof are recorded in
[`OPENVISIONLAB_PARAMETER_GUIDE_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_PARAMETER_GUIDE_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0055.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\parameter-guide-partial-retention-20260914`.
The source contract passed 14/14 in Debug and Release; `p257`, `p259`, and
`p260` parameter-guide WPF prechecks passed in both configurations; and dynamic
monitor/window probes passed after moving the window to the selected smaller-left
monitor. Pipeline smoke builds completed with zero errors; Debug has two existing
MSB3270 warnings and Release has those two plus the existing `CS8600` at
`Program.cs:10181`. Full theme/DPI/input, native Tool/file association,
camera/SDK/GPU, actual inspection execution/persistence, and long-running native
runtime remain unverified.

The next scheduled candidate is another as-yet-unprotected Partial selected by
the active automation after static de-duplication. Completed Parameter Guide,
Double-input Shell, the earlier Matching Learn extraction, and all prior
boundaries remain closed and must not be split without a new requirement,
reproducible defect, failed criterion, or changed dependency/lifetime boundary.

The twenty-fifth resumed slice is `PL-0056`.
The scheduled `MatchingLearnView.xaml.cs` candidate was first checked against
the existing `OPENVISIONLAB_LEARN_MATCHING_VIEW_20260908` completion record and
was skipped as duplicate work. The next unprotected boundary,
`VisionToolDoubleInputCustomToolShell.xaml.cs`, was then traced through
`ArithmeticToolWpfView`, the shared double-input base/controller/runtime, the
dock-mode helper and Learn Window controller. The Shell remains a required
XAML/DP/visual facade and docked-density presentation adapter; existing runtime,
controller, base View, concrete View and factory/document owners retain command,
mutable layer/preview state, policy and release lifetime. No new ViewModel,
service, wrapper or Partial was justified.

`VisionToolDoubleInputCustomToolShellPartialBoundaryContract` passed 12/12 in
Debug and Release. `wpf_arithmetic_tool_learn_button` and
`wpf_layer_selection_arithmetic_tool` passed in both configurations, and the
dynamic monitor/window probe moved the actual smoke window to the selected
smaller-left monitor with one intersecting window and a passing screenshot
contract. The owner map, reading order, and evidence are recorded in
[`OPENVISIONLAB_DOUBLE_INPUT_SHELL_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_DOUBLE_INPUT_SHELL_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0056.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\double-input-shell-partial-retention-20260914`.
Full Tool consumer/theme/DPI/input, native SDK/GPU/camera, actual inspection
execution/persistence and long-running native runtime remain unverified.

The PL-0056 de-dup check leaves no unprotected manual Partial candidate. The
current RefactorAudit reports 54 compiled Partial declarations; the resolved
PL-0028 owner map covers every retained row with owner/reason, caller path,
mutable-state writer, lifetime/release owner and binding/public/test contract,
and the current PL-0032–PL-0056 slices add focused proof for reopened concrete
boundaries. Remaining generated/XAML/designer/native/cohesive/test boundaries
must stay closed unless a new requirement, reproducible defect, failed
criterion, or changed dependency/lifetime boundary appears. The active
automation may therefore be removed after this completion gate; a future
explicit user request can create a new bounded cycle.

The twenty-third resumed slice is `PL-0054`.
`VisionToolVerificationGuideView.xaml.cs` was rechecked as the next Tool View
Partial and retained without production changes. The View remains the required
dependency-property/XAML presentation adapter for Header, State, Criteria,
NextAction, StateBrush, automation IDs, tooltips/trimming, and compact-density
row/padding projection. `VisionToolAreaVerificationGuidePresenter` and
`VisionToolMatchingVerificationGuidePresenter` remain the criteria/result/
status/next-action policy owners; `VisionToolSingleInputPropertyToolShell`
remains the ToolContent/visibility/density composition owner; representative
Blob, Contour, Edge Based Matching and shared Matching runtime consumers remain
unchanged. No direct file-I/O/dialog/OpenCV/Recipe execution/persistence,
async work, independent business state, or lifetime/test seam justified a new
ViewModel, service, interface, forwarding Partial, or production split.

The owner map and no-change proof are recorded in
[`OPENVISIONLAB_VERIFICATION_GUIDE_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_VERIFICATION_GUIDE_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0054.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\verification-guide-partial-retention-20260914`.
The source contract passed 12/12 in Debug and Release; VisionRecipeRunnerSmoke
Debug built with zero errors and two existing MSB3270 architecture warnings,
Release built with zero warnings/errors; shared Blob/Contour/Matching WPF
prechecks passed in both configurations; and Debug/Release dynamic
monitor/window probes moved the smoke window to the selected smaller-left
monitor and produced a valid target PNG without an error artifact. Full
theme/DPI/input, native Tool/file association, camera/SDK/GPU, actual
inspection execution/persistence, and long-running native runtime remain
unverified. Readiness, RefactorAudit, DocumentationIndex, LLM index parse and
`git diff --check` are recorded after this slice.

The twenty-second resumed slice is `PL-0053`.
`MetricsAcceptanceLearnView.xaml.cs` was rechecked as the next Learn View Partial
and retained without production changes. The View remains the required
XAML/presentation adapter; `MetricsAcceptanceLearnPresenter` owns fixed samples,
average/range/maximum statistics, average/outlier gate decisions, animation
stages, formulas and status text; `OpenVisionLearnWindow` owns topic visibility,
refresh, public facade and child lifetime; and the View owns only WPF sample-cell,
brush/text projection, Play/Step/Reset interaction and the 520ms
DispatcherTimer attach/detach lifetime. No direct file-I/O/dialog/OpenCV/
persistence/real Recipe execution/tool-creation coupling or independent
state/lifetime/test seam justified a new split.

The owner map and no-change proof are recorded in
[`OPENVISIONLAB_METRICS_ACCEPTANCE_LEARN_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_METRICS_ACCEPTANCE_LEARN_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0053.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\metrics-acceptance-learn-partial-retention-20260914`.
The source contract passed 12/12 in Debug and Release; the existing Metrics
Acceptance presenter contract passed 3/3 in Debug and Release; VisionRecipeRunnerSmoke
Release built with zero warnings/errors and Debug built with zero errors plus two
existing MSB3270 architecture warnings; focused Metrics Acceptance WPF contract
and View smoke passed in Debug and Release; and Debug/Release dynamic
monitor/window probes passed after placing the window on the selected
smaller-left monitor. Full theme/DPI/input, native Tool/file association,
camera/SDK/GPU, actual Recipe execution/persistence, and long-running native
runtime remain unverified. OpenVisionReadinessCheck Debug/Release, RefactorAudit,
DocumentationIndex, LLM index JSON parse, and `git diff --check` also passed.

The twenty-first resumed slice is `PL-0052`.
`GeometryLearnView.xaml.cs` was rechecked as the next Learn View Partial and
retained without production changes. The View remains the required
XAML/presentation adapter; `GeometryLearnPresenter` owns Angle/Scale state,
Rotate→Scale→ROI review stages, semantic roles, formula/status and Tool hint
policy; `OpenVisionLearnWindow` owns topic selection, guide refresh, callback
composition, public facade and child lifetime; and the View owns only WPF
transform/brush/text projection, related-tool forwarding, Play/Step/Reset
interaction, and the 520ms DispatcherTimer attach/detach lifetime. No direct
file-I/O/dialog/OpenCV/persistence/real Recipe execution/tool-creation coupling
or independent state/lifetime/test seam justified a new split.

The owner map and no-change proof are recorded in
[`OPENVISIONLAB_GEOMETRY_LEARN_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_GEOMETRY_LEARN_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0052.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\geometry-learn-partial-retention-20260914`.
The source contract passed 12/12 in Debug and Release; the existing Geometry
presenter contract passed 4/4 in Debug and Release; VisionRecipeRunnerSmoke
builds passed with zero warnings/errors; focused Geometry WPF contract and View
smoke passed in Debug and Release; and Debug/Release dynamic monitor/window
probes passed after placing the window on the selected smaller-left monitor.
Full theme/DPI/input, native Tool/file association, camera/SDK/GPU, actual
Rotate/Scale·Affine Recipe execution/persistence, and long-running native runtime
remain unverified.

The twentieth resumed slice is `PL-0051`.
`LayerRecipeLearnView.xaml.cs` was rechecked as the next Learn View Partial and
retained without production changes. The View remains the required
XAML/presentation adapter; `LayerRecipeLearnPresenter` owns fixed Layer/Step
routes, selected/animation state, formula/meaning/status and route decisions;
`OpenVisionLearnWindow` owns topic visibility, refresh, public facade and child
lifetime; and the View owns only WPF cells/brush/text projection, Play/Step/Reset
interaction, and the 520ms DispatcherTimer attach/detach lifetime. No direct
file-I/O/dialog/OpenCV/persistence/real Recipe execution/tool-creation coupling
or independent state/lifetime/test seam justified a new split.

The owner map and no-change proof are recorded in
[`OPENVISIONLAB_LAYER_RECIPE_LEARN_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_LAYER_RECIPE_LEARN_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0051.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\layer-recipe-learn-partial-retention-20260914`.
The source contract passed 12/12 in Debug and Release; the existing Layer/Recipe
presenter contract passed 4/4 in Debug and Release; VisionRecipeRunnerSmoke
builds passed with zero warnings/errors; focused Layer/Recipe WPF contract and
View smoke passed in Debug and Release; and Debug/Release dynamic monitor/window
probes passed after placing the window on the selected smaller-left monitor.
Full theme/DPI/input, native Tool/file association, camera/SDK/GPU, actual Recipe
execution/persistence, and long-running native runtime remain unverified.

The nineteenth resumed slice is `PL-0050`.
`GrayscaleLearnView.xaml.cs` was rechecked as the next Learn View Partial and
retained without production changes. The View remains the required
XAML/presentation adapter; `GrayscaleLearnPresenter` and
`OpenVisionLearnBasicGrayscaleSimulationModel` own Threshold/Brightness/
Arithmetic/Filtering evaluation, lesson state, formulas, status, and sample
primitives; `OpenVisionLearnWindow` owns topic/action composition, Apply/Close
forwarding, public facade, and child lifetime; and the View owns only WPF
cells/marker/text projection, related-tool callback, explicit result events, and
four DispatcherTimer attach/detach lifetimes. No direct
file-I/O/dialog/OpenCV/tool-creation/persistence coupling or independent
state/lifetime/test seam justified a new split.

The owner map and no-change proof are recorded in
[`OPENVISIONLAB_GRAYSCALE_LEARN_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_GRAYSCALE_LEARN_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0050.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\grayscale-learn-partial-retention-20260914`.
The source contract passed 11/11 in Debug and Release; VisionRecipeRunnerSmoke
builds passed with zero warnings/errors; focused Grayscale Learn WPF contract
and View smoke passed in Debug and Release; and the dynamic monitor/window probe
passed after placing the window on the selected smaller-left monitor. Full
theme/DPI/input, native related-tool click/file association, camera/SDK/GPU, and
long-running native runtime remain unverified.

The eighteenth resumed slice is `PL-0049`.
`FoundationLearnView.xaml.cs` was rechecked as the next Learn View Partial and
retained without production changes. The View remains the required
XAML/presentation adapter; `FoundationLearnPresenter` owns Point/ROI and
Mat-channel lesson stages, role/visibility decisions, fixed guidance and Tool
location text; `OpenVisionLearnWindow` owns topic/action composition and the
public facade; and the View owns only WPF cell/marker/brush projection, related
callback forwarding, and two DispatcherTimer attach/detach lifetimes. No direct
file-I/O/dialog/OpenCV/tool-creation/persistence coupling or independent
state/lifetime/test seam justified a new split.

The owner map and no-change proof are recorded in
[`OPENVISIONLAB_FOUNDATION_LEARN_PARTIAL_RETENTION_20260914.md`](../../reports/OPENVISIONLAB_FOUNDATION_LEARN_PARTIAL_RETENTION_20260914.md),
`.proofline/issues/PL-0049.json`, and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\foundation-learn-partial-retention-20260914`.
The source contract passed 9/9 in Debug and Release; VisionRecipeRunnerSmoke
builds passed with zero warnings/errors; focused Foundation Learn WPF contract
and View smoke passed in Debug and Release; and the dynamic monitor/window probe
passed after placing the window on the selected smaller-left monitor. Full
theme/DPI/input, native related-tool click/file association, camera/SDK/GPU, and
long-running native runtime remain unverified.

## Current Prioritized Refactor Schedule — 2026-09-10

Status: R16 VERIFIED; R17 VERIFIED (no safe extraction); R18 VERIFIED; R19 VERIFIED (no safe physical move); R20 VERIFIED; R21 VERIFIED; R22 VERIFIED; R23 VERIFIED; R24 VERIFIED; R25 VERIFIED (no-change); R26 VERIFIED (Sample Picker member navigation); R27 VERIFIED (WPF environment matrix recording); R28 VERIFIED (Pipeline Review focused recheck, no source change); R29 VERIFIED (release precheck, commit/tag boundary pending); R30 VERIFIED (ImageCanvas debug-output cleanup) for the explicit junior discoverability structure
program; R15 VERIFIED for the Common responsibility-folder cleanup; R1~R14
source/build/focused-contract slices, the clean distribution
launch/migration contract, the public sample learn/pair contract follow-up, the
sequential sample-review async deadlock correction, and the Pipeline Review
smoke ownership/UI-contract follow-up remain verified. Full WPF UI
qualification remains unverified.
The current execution plan is
`docs/reports/OPENVISIONLAB_PRIORITIZED_REFACTOR_SCHEDULE_20260910.md`.

The heartbeat `openvisionlab-2d` was configured at five-minute intervals for
the R16~R25 source-only discoverability program. It was deleted after the
verified R25 no-change boundary because no independent source boundary remained.
The completed runs skipped user-input or hardware-dependent steps instead of
waiting indefinitely, recorded unverified runtime boundaries, and did not
commit, push, create tags/releases, deploy, or modify `C:\\Git\\2D\\Original`.

## Current WPF View MVVM and portability audit — 2026-09-13

Status: `SOURCE AUDIT COMPLETE`; no production code was changed in this audit.
The full report is
[`OPENVISIONLAB_WPF_VIEW_MVVM_PORTABILITY_AUDIT_20260913.md`](../../reports/OPENVISIONLAB_WPF_VIEW_MVVM_PORTABILITY_AUDIT_20260913.md).

The source inventory covers 60 XAML files (56 visual Views and 4 resource
dictionaries), 30 ViewModel files, 59 partial declarations, 27 projects, and 34
project references. The project is a deliberate hybrid of MVVM, concrete
Presenter/Controller owners, PropertyGrid/custom-control contracts, and WPF
lifecycle adapters; it is not a pure View -> ViewModel structure. Single-file
View relocation is therefore generally not a supported portability unit. The
report records the required feature bundle and first-reading route for each
View family.

The static gate passed with `Invoke-RefactorAudit.ps1 -Verify`: 822 C# files,
zero project cycles, zero Shell Run History storage calls, and the existing
`RoiImageCanvasViewModel` UI/IO signal explicitly reported. Three P1 source
boundaries remain candidates for a separate minimum-change implementation:
Recipe run evidence file/Bitmap decoding in the View, Line Tool persistence
called from the View, and the ImageCanvas ViewModel's WPF/native/path coupling.
The completed Shell and Pipeline Review owners remain closed; file length alone
does not reopen them.

First-contributor code order remains:
`AGENTS.md -> docs/README.md -> OpenVisionLab.sln/csproj -> Program.Main ->
OpenVisionLabApplication.Run -> OpenVisionShellHostWindow ->
OpenVisionShellHostView -> feature owner`. Runtime WPF theme/DPI/monitor/input,
camera/SDK/GPU, and long-running native qualification remain
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Current P1 View boundary refactor — 2026-09-13

Status: `PL-0027 M1 VERIFIED; M2 VERIFIED; M3 VERIFIED (no-change proof); M4 VERIFIED; M5 VERIFIED`;
The final owner-map and audit are complete. The single
five-minute heartbeat `openvisionlab-2d-1-5` was reused and activated for this
new bounded issue; no duplicate automation was created.

M1 moved file-backed `System.Drawing.Bitmap` decoding out of
`OpenVisionRecipeRunEvidenceViewerView.xaml.cs` into the existing concrete
`OpenVisionBitmapImagePreviewFactory.LoadBitmap(path, role)`. The View keeps
evidence selection, status/error projection, XAML event wiring, and viewer
lifecycle. `OpenVisionLayerViewerView` remains the clone/Dispose owner after the
local decoded Bitmap is passed to `SetLayer`. Existing role-specific error text,
read-only evidence behavior, and Preview/Run non-mutation were preserved.

The M1 call path is:
`OpenVisionShellHostView -> OpenVisionRecipeRunEvidenceViewerController.Open ->
OpenVisionRecipeRunEvidenceViewerView.TrySetEvidence/TrySetDrawing ->
OpenVisionBitmapImagePreviewFactory.LoadBitmap -> OpenVisionLayerViewerView.SetLayer`.
The Evidence View no longer contains `FileStream` or `Image.FromStream`.

M1 evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\p1-view-boundaries-20260913\\m1-recipe-evidence`.
The focused `RecipeRunEvidenceImageBoundaryContract` passed 4/4 and the
`VisionRecipeRunnerSmoke` Debug build passed with zero warnings and errors.
Full WPF visual/theme/DPI/monitor/input, camera/SDK/GPU, and long-running native
qualification remain `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

M2 moved Line Tool property persistence out of
`LineToolWpfView.xaml.cs`. The existing `OpenVisionNativeCustomToolFactory`
now injects the existing `OpenVisionNativeToolPropertySessionStore.Save`
operation into `LineToolPresenter`; the View keeps only PropertyGrid/preset/sample
change wiring and calls `presenter.PersistProperties()`. The Line A/B storage keys,
Recipe context, failure event, and existing change triggers are unchanged.

The M2 call path is:
`OpenVisionNativeCustomToolFactory.CreateLine -> LineToolPresenter(viewModel,
persistProperties) -> LineToolWpfView property/preset/sample change ->
OpenVisionNativeToolPropertySessionStore.Save(Line(L)_1 / Line(R)_1)`.
The View no longer references the storage type or a `PersistLineProperties`
policy method.

M2 evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\p1-view-boundaries-20260913\m2-line-persistence`.
The focused `LineToolPersistenceBoundaryContract` passed 3/3 and the
`VisionRecipeRunnerSmoke` Debug build passed with zero warnings and errors.
M3 inspected `RoiImageCanvasViewModel` callers and mutable/native ownership. The
existing input controllers, dialog/context hosts, directory policy, image loader,
and image saver already provide the independent policy owners. The ViewModel remains
the single concrete facade for ROI/mode/snapshot state, current Mat clone,
ImageCanvasControl, reshape timer, and native Dispose. No additional independent
state/lifetime/test seam was proven, so no production split was made and no-change
proof was recorded rather than adding a wrapper or partial.

M3 evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\p1-view-boundaries-20260913\\m3-imagecanvas`.
The focused `RoiImageCanvasBoundaryContract` passed 6/6 and the
`VisionRecipeRunnerSmoke` Debug build passed with zero warnings and errors.

M4 reopened the 59 declarations as a responsibility review after the user
clarified that a protected Partial classification is not sufficient structural
proof. The row-level owner/call-path/lifetime disposition is recorded in
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\p1-view-boundaries-20260913\\m4-partial-audit\\partial-structure-review.csv`
and the durable report is
`docs/reports/OPENVISIONLAB_PARTIAL_RESPONSIBILITY_REVIEW_20260913.md`.
The concrete conflict found in this pass was file-backed pattern decoding in
`OpenGlTemplateEditorWindow.xaml.cs` and `RoiEditorWindow.xaml.cs`; both now
use the existing `OpenVisionBitmapImagePreviewFactory.LoadBitmap` owner. The
XAML partials remain because the Windows still own ROI control state, visual
events, preview assignment, and source Bitmap release. The focused image-boundary
contract passed 3/3 and the Debug build passed with zero warnings/errors.

M5 ran the final owner-map/navigation review, `Invoke-RefactorAudit.ps1 -Verify`,
`TestDocumentationIndex.ps1`, `git diff --check`, Solution Debug/Release builds,
VisionRecipeRunnerSmoke Debug/Release builds, and all four focused contracts in
both configurations. The final phase summary is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\p1-view-boundaries-20260913\\m5-final\\phase-summary.txt`.
PL-0027 is resolved in the issue ledger. WPF visual/theme/DPI/monitor/input,
GPU/camera/SDK, and long-running native shutdown remain source-only/unverified.

The durable issue and remaining work are recorded in
`.proofline/issues/PL-0027.json` and
[`P1 View boundary refactor report`](../../reports/OPENVISIONLAB_P1_VIEW_BOUNDARIES_20260913.md).
The retained 59 Partial declarations were not mechanically merged or deleted;
each retained row has a current owner and a reason tied to its XAML, native,
generated, test, or cohesive visual contract.

## Current Partial structural elimination — 2026-09-13

Status: `PL-0028 RESOLVED / M4 VERIFIED`; this is a new bounded follow-up to the resolved
`PL-0027`. The user requested that the 59 Partial declarations be structurally
removed where they hide real responsibilities. The new plan is
`docs/reports/OPENVISIONLAB_PARTIAL_STRUCTURAL_ELIMINATION_PLAN_20260913.md` and
the issue ledger is `.proofline/issues/PL-0028.json`.

M1 and M2 are `VERIFIED`. The M3 boundary reviews are `VERIFIED`: the source
reached 53 compiled Partial declarations during dead-feature cleanup, then the
required Sample Picker child constructor restored the current scan to 54; no
literal smoke-contract matches remain. The first real owner
move extracted Color/HSV lesson animation state, HSV range evaluation, mask
decision, and operator-facing guide text from
`OpenVisionLearnWindow.xaml.cs` into the concrete
`ColorHsvLearnPresenter`. The Window retains XAML event wiring,
`DispatcherTimer`, brushes, and visual control projection. The focused
`ColorHsvLearnPresenterContract` passed 5/5 and the VisionRecipeRunnerSmoke
Debug build passed with 0 warnings/errors. The
`OpenVisionRecipeBasicLifecycleView.xaml.cs` file was then deleted because it
contained only `InitializeComponent()`; XAML/Shell/RecipeCommands contracts
remain unchanged. The Shell lifecycle contract passed 7/7 in both Debug and
Release, Solution and Smoke Release/Debug builds had 0 warnings/errors, and
the refactor audit passed with `PartialDeclarations=58`. The next
`OpenVisionRecipeValidationSuiteView.xaml.cs` file was then deleted because it
also contained only `InitializeComponent()`; its XAML/Shell/RecipeCommands
contracts remain unchanged. The validation-suite contract passed 4/4 in both
Debug and Release, Solution Debug/Release builds had 0 warnings/errors, and
the refactor audit passed with `PartialDeclarations=57`. The third
`OpenVisionWorkspaceSamplePickerView.xaml.cs` file was then deleted because it
also contained only `InitializeComponent()`; its Window composition, Shell
workflow caller, and ViewModel bindings remain unchanged. The source contract
passed five checks, Readiness Debug/Release contracts passed, Solution
Debug/Release builds had 0 warnings/errors, and the refactor audit passed with
`PartialDeclarations=56`.

`OpenVisionShellHostWindow.xaml.cs` was then reviewed and retained. It is the
concrete WPF composition/lifetime owner for Shell creation, HWND hook
registration/removal, responsive Window scaling, smoke projections, and
Shell disposal. The focused source contract passed 7/7 and the OpenVisionLab
Debug build passed with 0 warnings/errors; no independent owner or safe
replacement boundary was proven.

`OpenVisionPendingToolView.xaml.cs` was then reviewed and retained as a narrow
WPF adapter. It injects the existing `OpenVisionPendingToolViewModel` into the
XAML DataContext; the ViewModel owns language/state notifications and the
DocumentController owns disposal on close or tool switch. The focused source
contract passed 8/8, OpenVisionLab and Readiness Debug builds had 0
warnings/errors, and no independent non-UI replacement owner was proven.

`OpenVisionLayerDockWorkspaceView.xaml.cs` was then reviewed and retained as
the concrete AvalonDock visual adapter. Its DependencyProperties, native
DockingManager event bridge, guide/geometry methods, visual snapshots, and
header diagnostics are consumed by existing composition/controllers. The
focused source contract passed 9/9 and the Docking.Controls Debug build had 0
warnings/errors; moving it would add a wrapper without an independent owner.

`OpenVisionLayerDockingGuideOverlayView.xaml.cs` was then reviewed and retained
as the concrete XAML visual adapter for docking guide zones. It owns the
generated namescope, active-zone visibility/brush state, guide-zone count, and
pane margin projection; `OpenVisionLayerDockWorkspaceView` is the existing
composition caller. The focused source contract passed 9/9 and the
Docking.Controls Debug/Release builds had 0 warnings/errors. It contains no
Recipe, persistence, inspection, or native lifetime policy, so extracting a
wrapper would only hide the same visual state.

`ImageCanvasControl.cs` and `ImageCanvasControl.designer.cs` were then reviewed
and retained as one SharpGL/WinForms native rendering boundary. The designer
Partial creates the `OpenGLControl` and owns generated disposal wiring, while
the main Partial owns OpenGL rendering, input event subscriptions, P/Invoke,
texture/overlay state, and `ReleaseOpenGlControl`. `RoiImageCanvasViewModel`
and the external consumer session call the same ImageCanvas and Dispose path.
The focused source contract passed 10/10 and ImageCanvas Debug/Release builds
had 0 warnings/errors; splitting it without a separate native lifetime owner
would risk the existing cleanup order.

The excluded duplicate `AddRoiArrayView.xaml.cs` and `.xaml` pair was then
removed. The ImageCanvas project excluded both files, while the compiled
`Compatibility/AddRoiArrayViewCompatibility.cs` already provided the same
`OpenVisionLab.ImageCanvas.Views.AddRoiArrayView` type, bindings, and
RequestClose/DialogResult path used by `RoiInteractionMouseUp`. The focused
source contract passed 10/10, the existing ROI boundary contract passed 6/6,
ImageCanvas Debug/Release and VisionRecipeRunnerSmoke Debug builds had 0
warnings/errors, and the audit count decreased to `PartialDeclarations=55`.

The excluded `AutoAlignTeachingView.xaml.cs`, `.xaml`, and
`AutoAlignTeachingViewModel.cs` were then removed. All three had no repository
caller or compiled replacement path, and the ImageCanvas project explicitly
excluded them. The focused source contract passed 8/8, the existing ROI
boundary contract passed 6/6, ImageCanvas Debug/Release and
VisionRecipeRunnerSmoke Debug builds had 0 warnings/errors, and the audit count
decreased to `PartialDeclarations=54`. `EnumAutoAlignDirection` was left
unchanged because it is outside this dead-feature removal boundary.

The excluded `AutoWarpageTeachingView.xaml.cs`, `.xaml`, and
`AutoWarpageTeachingViewModel.cs` were then removed. They also had no repository
caller or compiled replacement path, and their stale ImageCanvas project
exclusions were removed. The focused source contract passed 8/8, the existing
ROI boundary contract passed 6/6, ImageCanvas Debug/Release and
VisionRecipeRunnerSmoke Debug builds had 0 warnings/errors, and the audit count
decreased to `PartialDeclarations=53`.

`RoiImageCanvasView.xaml.cs` was then reviewed and retained as a concrete
WPF/WindowsFormsHost lifecycle adapter. It owns the XAML namescope, chrome
DependencyProperties, DataContext attach/detach, dialog/context-menu host wiring,
WPF key event forwarding, pending DispatcherOperation cancellation, and
view-owned native host cleanup. `RoiImageCanvasViewModel` remains the concrete
owner of ROI/mode/snapshot state, current Mat, ImageCanvasControl, refresh timer,
and ViewModel Dispose; the View intentionally does not dispose its DataContext.
The source contract passed 10/10, the existing ROI boundary contract passed 6/6,
ImageCanvas Debug/Release and VisionRecipeRunnerSmoke Debug builds had 0
warnings/errors, and RefactorAudit remained `PartialDeclarations=53` with zero
project cycles and zero Shell storage calls. Evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m14-roi-image-canvas-view-retention`.

`LogPanelView.xaml.cs` was then reviewed and retained as a cohesive WPF adapter.
It creates and attaches the existing `LogPanelViewModel`, projects compact layout
and `LogList.ScrollIntoView`, and removes its event subscriptions before releasing
the ViewModel on `Unloaded`. The View does not own log-file I/O, filtering,
commands, or the display buffer: those remain with `LogPanelViewModel`,
`LogPanelFileAccess`, and `RuntimeLogStream`. The source contract passed 10/10,
Logging.Controls Debug/Release builds had 0 warnings/errors, and the
PipelineViewerScreenshotSmoke Debug build had 0 errors with the existing
CS8600 warning at `Program.cs:10181`. Evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m15-log-panel-view-retention`.

`src/OpenVisionLab/Properties/Settings.Designer.cs` was then reviewed and
retained as generated Settings code. `SettingsSingleFileGenerator` produces the
`ApplicationSettingsBase` Partial, synchronized `Default` instance, and 70
UserScopedSetting/DefaultSettingValue properties from `Settings.settings`.
There is no current source `Settings.Default` caller, but removing or manually
replacing this generated file would be an unapproved settings migration, so the
framework boundary remains intact. The source contract passed 10/10 and
OpenVisionLab Debug/Release builds had 0 warnings/errors. Evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m16-settings-designer-retention`.

`OpenVisionTcpIntegrationWindow.xaml.cs` was then reviewed and retained as the
concrete WPF Window adapter. It forwards the PasswordBox key, applies the
controller-owned close guard, and unsubscribes Closing/Closed/PasswordChanged
before notifying `OpenVisionTcpIntegrationController.OnWindowClosed`. The
controller remains the owner of TCP exchange, JSON settings, validation,
commands, cancellation, callbacks, window reopen, and async disposal; the Window
does not dispose it because the Shell-owned controller may reopen the window.
The source contract passed 10/10, the TCP controller disposal contract passed,
and OpenVisionLab/VisionRecipeRunnerSmoke Debug/Release relevant builds had 0
warnings/errors. Evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m17-tcp-integration-window-retention`.

`VisionToolNImageVerificationWindow.xaml.cs` was then reviewed and retained as
the concrete WPF Window adapter for the N-image verification surface. It keeps
the XAML namescope, source/drawing `OpenVisionZoomableImageController` instances,
selected-image reset projection, and deterministic Closed cleanup. The existing
`VisionToolNImageVerificationController` remains the owner of image selection,
file dialogs, verification execution, cancellation, result/export/promotion
state, and internal resource disposal. `OpenVisionNativeToolDocument` constructs
the controller and window, assigns the owner, and calls `ShowDialog`; no separate
workflow owner is hidden in the View. The source contract passed 10/10, the
`wpf_tool_n_image_verification_window` and
`wpf_tool_n_image_entry_side_effect_contract` smoke targets passed with fresh
screenshots, and OpenVisionLab/PipelineViewerScreenshotSmoke Debug/Release
builds had 0 warnings/errors. Evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m18-n-image-verification-window-retention`.

`OpenVisionShellHostView.xaml.cs` was then reviewed and retained as the single
XAML Shell composition root and concrete visual/lifetime adapter. Its constructor
connects the existing session, recipe, layer, tool-window, workspace, preview,
refresh, presenter, command-surface, and test-surface owners in dependency order.
The View keeps WPF DependencyProperties, binding projection, UI navigation/event
handlers, tracked callback cleanup, and the Dispose entry point; mutable Recipe,
Layer, Tool, Workspace, Pipeline, and session state remains in existing concrete
owners. The source contains no direct file I/O, Pipeline execution/storage,
OpenCvSharp, network, or algorithm policy that can be extracted safely without
reopening another owner. `OpenVisionShellHostWindow` creates and owns the View and
calls `Dispose` on close. The source contract passed 12/12, OpenVisionReadinessCheck
passed, the three focused Shell WPF smoke targets passed with fresh screenshots,
and OpenVisionLab/PipelineViewerScreenshotSmoke/Readiness builds had 0
warnings/errors. Evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m19-shell-host-view-retention`.

`OpenVisionRecipePendingEditDialog.xaml.cs` was then reviewed and retained as
the required XAML Window partial, but its independent non-WPF display projection
was extracted to the concrete
`OpenVisionRecipePendingEditDialogViewModel.cs` file. The Window now keeps only
`InitializeComponent`, DataContext wiring, modal `Decision`/`DialogResult`,
Escape and button event handling. `RecipeDialogAdapter` remains the owner of
Window creation, owner assignment, `ShowDialog`, and result conversion, while
`OpenVisionRecipePendingEditTransitionController` and `RecipeCommandSurface`
remain the owners of apply/discard/cancel workflow state. The source contract
passed 10/10 and the focused `wpf_recipe_pending_edit_dialog` smoke passed with
a fresh screenshot. OpenVisionLab Debug/Release and PipelineViewerScreenshotSmoke
Debug builds completed with 0 warnings/errors; RefactorAudit remains at
`PartialDeclarations=53`. Evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m20-pending-edit-dialog-viewmodel-extraction`.

`OpenVisionRecipeRunEvidenceViewerView.xaml.cs` was then rechecked as the next
retained XAML boundary. The previous P1 extraction is effective: file-backed
`Bitmap` decoding is owned by the existing concrete
`OpenVisionBitmapImagePreviewFactory.LoadBitmap`, while the View owns only
evidence selection, status/error projection, and the two child viewer lifetimes.
`OpenVisionLayerViewerView` clones incoming images and releases its prior owned
Bitmap; `OpenVisionLayerViewerWindowRegistry` disposes hosted content when the
floating window closes, and the controller disposes a failed initial View.
No independent ViewModel or wrapper boundary remains. The source contract passed
10/10 and `RecipeRunEvidenceImageBoundaryContract` passed 4/4. The drawing-
evidence WPF smoke was attempted but returned NG before opening the target because
`OPENVISIONLAB_VALIDATION_DATASET_ROOT` was not configured; that dataset-dependent
runtime path remains unverified. Evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m21-evidence-viewer-retention`.

`OpenVisionLayerViewerView.xaml.cs` was then rechecked as the shared image View
boundary for docked layers, floating layers, tool previews, and run-evidence
images. The View clones incoming Bitmap data into `ownedLayerImage`, releases the
previous owned image, and owns the canvas presenter, fallback zoom controller,
ROI canvas, compact chrome, language/Loaded subscriptions, pending refresh, and
Dispose sequence. `OpenVisionBitmapCanvasPresenter` uses the View-owned image for
OpenGL upload/save and drops its reference on Dispose; it is not a second Bitmap
disposer. Dock workspace and floating-window registries dispose hosted View
content on close. The source contract passed 12/12, the
`wpf_shell_host_layer_popout` smoke passed with a fresh screenshot, and
OpenVisionLab/PipelineViewerScreenshotSmoke Debug/Release checks completed with
0 warnings/errors. No independent ViewModel or wrapper owner was proven. Evidence
is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m22-layer-viewer-retention`.

`OpenVisionShellPreviewView.xaml.cs` was then rechecked as the standalone
`wpf_shell_preview` XAML/DataContext adapter. Its only manual behavior is creating
the existing `OpenVisionShellPreviewViewModel` and disposing that DataContext on
`Unloaded`. Navigation groups, selected tool/layer state, readiness policy,
localization, commands, and the language event subscription remain owned by the
ViewModel; production `OpenVisionShellHostView` also owns that same concrete
ViewModel directly. The View has no image/native/file/inspection policy, so no
independent replacement owner or wrapper was proven. The source contract passed
10/10, `wpf_shell_preview` passed with a fresh screenshot, and OpenVisionLab/
PipelineViewerScreenshotSmoke Debug/Release checks completed with 0 warnings/errors.
Evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m23-shell-preview-retention`.

`OpenVisionPipelineReviewView.xaml.cs` was then rechecked as the next retained
Pipeline Review XAML adapter. `OpenVisionPipelineReviewDocument` creates and
owns the View and existing `OpenVisionPipelineReviewExecutionController`; the
ViewModel owns bindable review/readiness/preview projection, the existing
`OpenVisionPipelineReviewLayoutController` owns compact/details/step-flow layout
state, the execution controller owns run identity, cancellation, revision-gated
callbacks and cached output images, and
`OpenVisionPipelineReviewImageResourceOwner` owns View-side Bitmap/diagnostic
resources. The remaining selection/highlight and event methods write WPF
controls through the required XAML namescope, so no independent concrete owner
or safe partial extraction was proven. The source contract passed 13/13,
`wpf_shell_host_pipeline_review` passed with a fresh screenshot, and
OpenVisionLab Debug/Release plus PipelineViewerScreenshotSmoke Debug builds
completed with 0 warnings/errors. Evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m24-pipeline-review-retention`.

`OpenVisionFloatingToolWindow.xaml.cs` was then rechecked as the required WPF
floating Window adapter. The Window owns only XAML namescope, title/icon/content
host projection, WinForms/OpenGL airspace activation, `DockRequested` event
bridging and symmetric local event cleanup. `OpenVisionFloatingToolWindowHost`
owns create/reuse, placement persistence, owner assignment, close and dock
routing; `OpenVisionLayerViewerWindowRegistry` and feature controllers own the
hosted content's close/Dispose boundary. No file, Recipe, inspection, image or
pipeline policy is present in the Window, so no independent concrete owner or
safe partial extraction was proven. The source contract passed 14/14,
`wpf_tool_window_dock_float_cycle` passed with a fresh screenshot, and
OpenVisionLab Debug/Release plus PipelineViewerScreenshotSmoke Debug builds
completed with 0 warnings/errors. Evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m25-floating-tool-window-retention`.

`OpenVisionStartupLoadingWindow.xaml.cs` was then rechecked as the required
startup Window adapter. It owns only XAML title/detail localization projection,
the render-pump `ShowReady` call, the `Complete` close gate, and `OnClosing`
protection against premature user close. `OpenVisionLabApplication.Run` owns
creation, display, Shell-preparation completion, and dispatcher shutdown;
`OpenVisionLabDirectSmokeRunner` owns the embedded
`startup-loading-feedback` scenario. The Window has no Recipe, Pipeline,
inspection, file, native-resource, or independent DataContext policy, so no
concrete owner extraction was proven. Source contract 12/12 and the focused
startup-loading smoke passed with Korean/English copy, close-gate assertions,
fresh screenshots, and monitor intersection evidence. Embedded OpenVisionLab
Debug/Release and PipelineViewerScreenshotSmoke Debug builds completed with
zero errors (the existing PipelineViewer nullable warning remains outside this
slice). Evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m26-startup-loading-window-retention`.

M3 remains in progress. The next action is to inspect
`src/OpenVisionLab/UI/Menu/Wpf/Windows/OpenVisionWindowTitleBar.xaml.cs` after
confirming its caller, mutable-state writer, lifetime/release owner, and
public/XAML/test contract. Reopen it only if a reproducible responsibility
conflict or changed dependency boundary is found.

`OpenVisionWindowTitleBar.xaml.cs` was then rechecked as the shared WPF chrome
UserControl. It owns only the XAML namescope, title/icon and Dock visibility
projection, localized tooltip/accessibility text, Window drag/minimize/
maximize/close framework actions, the `DockRequested` bridge, and symmetric
Loaded/Unloaded/language cleanup. `OpenVisionShellHostWindow`,
`OpenVisionFloatingToolWindow`, and `OpenVisionWorkspaceSamplePickerWindow`
inject title/icon and own their parent Window/content lifetime. No Recipe,
Pipeline, inspection, file, native-resource or independent DataContext policy
remains in the UserControl. Source contract 14/14 passed; the focused
`wpf_shell_host_window_chrome` and `wpf_shell_host_window_maximized` smoke
targets passed with fresh screenshots, automation IDs and work-area checks.
OpenVisionLab Debug/Release and PipelineViewerScreenshotSmoke Debug builds
completed with zero errors (the existing nullable warning remains in the smoke
project). Evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m27-window-title-bar-retention`.

M3 was closed after the Image Compare retained-boundary review. The final M4 audit
rechecked
`src/OpenVisionLab/UI/Popup/Wpf/ImageCompareWindow.xaml.cs`
after confirming its caller, mutable-state writer, lifetime/release owner, and
public/XAML/test contract. Reopen it only if a reproducible responsibility
conflict or changed dependency boundary is found.

`OpenVisionWorkspaceSamplePickerWindow.xaml.cs` was then rechecked as a modal
XAML adapter. The Window injects the existing
`OpenVisionWorkspaceSamplePickerViewModel`, owns owner/`ShowDialog()` mapping,
title-bar projection, Cancel/`DialogResult`, the HWND work-area hook, and
symmetric event/hook cleanup. The ViewModel remains the owner of catalog
filtering, selection eligibility, Learn-document policy, and sample state. The
old Window click handler that directly called `OpenLearnDocumentForSelection()`
was replaced by the ViewModel's `OpenLearnAndSelectCommand`; the ViewModel emits
the explicit `SelectionAccepted` result contract and the Window only converts it
to `DialogResult`. The child `OpenVisionWorkspaceSamplePickerView.xaml.cs` was
restored after the deleted constructor caused a reproducible empty UserControl
in smoke; this is a required `InitializeComponent()` contract, not a name-only
partial. Source contract 14/14, `wpf_shell_host_workspace_sample_picker` and
`wpf_shell_host_workspace_sample_picker_maximized` passed with fresh screenshots,
automation IDs, and work-area checks. OpenVisionLab Debug and
PipelineViewerScreenshotSmoke Debug builds completed (the existing smoke
nullable warning remains). After generated-output cleanup, the OpenVisionLab
Release build rerun completed with 0 warnings/errors. The original evidence was
at `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m28-workspace-sample-picker-window-retention`.

`ImageCompareWindow.xaml.cs`도 재확인했다. standalone
`OpenVisionLab.ImageCompare` EXE와 `PipelineViewerScreenshotSmoke`의
`wpf_image_compare`가 caller이며, Window는 required XAML partial, public
`LoadImages` facade, `OpenFileDialog`, pointer/window chrome 이벤트와
ViewModel Dispose만 소유한다. 선택 디렉터리 저장/복원은
`ImageCompareDirectoryPolicy`, decoded `Bitmap`/frozen `BitmapSource` 교체
수명은 `ImageCompareImageResource`와 `ImageCompareSlotViewModel`이 소유한다.
독립 state/lifetime/test owner로 이동할 책임은 입증되지 않아 새 wrapper,
ViewModel, partial을 만들지 않았다. DirectoryPolicy/Resource contract,
standalone Debug build, `wpf_image_compare` UI smoke가 통과했다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m29-image-compare-retention`에 있다.
Smoke는 off-screen capture이므로 monitor-visible EXE와 전체 theme/DPI/input
matrix는 미검증이다. PL-0028 M3 경계 처리는 종료했고, M4 최종 감사에서
`Invoke-RefactorAudit -Verify`(Partial 54, literal match 0, cycle 0), Solution
Debug/Release, VisionRecipeRunnerSmoke Debug/Release, Readiness Debug/Release,
focused contracts, five-target WPF smoke, DocumentationIndex, issue-ledger,
`git diff --check`를 다시 통과했다. 전체 실행 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m4-final-audit\phase-summary.txt`에 있다.
파일 크기만으로 Tool/Learn/ROI editor partial을 다시 나누지 않는다.

2026-09-13 storage cleanup removed the D: raw verification tree for this
sequence and the current Dev generated `bin`/`obj`/`.vs`/`build`/`dist`/`tmp`/
`temp` outputs plus `D:\OpenVisionLab_Data\Dev\artifacts`. Source, docs,
samples, and user data were preserved. The recorded results remain in this
handoff and the issue ledger; raw screenshots, logs, and build outputs are no
longer retained.

Each row is reopened against its current caller, mutable-state
writer, lifetime/release owner, and binding/public/test contract. A manual
Partial with an independent concrete owner will be moved one verified boundary
at a time. XAML/generated, designer/native, cohesive visual, and test-host
declarations will remain only when their compile, binding, lifetime, or test
contract requires them and the reason is recorded.

The `openvisionlab-2d` ten-minute heartbeat registered by PL-0029 was the single
executor for the PL-0028/PL-0029 continuation. After the M4 audit and the final
original-request recheck it is stopped; no duplicate or replacement automation
is allowed.

## Current project readiness and document-based refactor — 2026-09-13

Status: `PL-0029 RESOLVED`; the integrated contract is
`docs/reports/OPENVISIONLAB_PROJECT_READINESS_AND_REFACTOR_CONTRACT_20260913.md`
and the issue is `.proofline/issues/PL-0029.json`.

The first-contributor audit consolidates the repository document inventory,
Visual Studio/solution start route, mixed MVVM and WPF/native lifetime owners,
DLL dependency and redistribution boundaries, and document/code cleanup
candidates. The document is indexed through the
`project_readiness_refactor` route. R1 repaired the visible Korean Recipe,
Pipeline, file-dialog, validation-set, and delete-confirmation strings in
`RecipeDialogAdapter` without changing delegate signatures, owner resolution,
or dialog result semantics. `RecipeDialogLocalizationContract` passed 5/5 and
the VisionRecipeRunnerSmoke Debug build passed with zero warnings/errors.

R2 was source/contract rechecked and intentionally left unchanged: the current
`AppPathService` DEBUG behavior, runtime-data-root contract, and
`BuildCleanRuntime.ps1` manifest agree. A new AppPath change requires a
reproducible stale-state fresh-F5 defect. PL-0028 remains the sole owner of
Partial-boundary work; this issue did not duplicate its ImageCompareWindow review.
The final original-request recheck is complete and the `openvisionlab-2d`
heartbeat has been stopped; no replacement schedule, commit, push, tag, release,
or deployment is authorized.

The remaining runtime/UI, DPI/theme/input, hardware, GPU/SDK, long-running
native shutdown, and third-party DLL redistribution checks remain explicitly
unverified until their required environment or rights evidence exists.

## Current Sample Picker image boundary — 2026-09-13

Status: `PL-0030 RESOLVED`. This bounded follow-up was requested after the
documented audit to continue refactoring without reopening completed owners.
`OpenVisionWorkspaceSamplePickerViewModel` no longer owns direct preview-file
existence checks or `BitmapImage`/OnLoad/decode-width construction. Its existing
`SelectedImageSource` binding facade now calls the existing
`OpenVisionBitmapImagePreviewFactory.TryCreateFromPath(path, 420)` owner. Missing
and corrupt paths still return `null`; valid results remain OnLoad/frozen.

The call path is:
`OpenVisionWorkspaceSamplePickerWindow.xaml` binding ->
`OpenVisionWorkspaceSamplePickerViewModel.SelectedImageSource` ->
`LoadImageSource` -> `OpenVisionBitmapImagePreviewFactory`.
No new interface, manager, wrapper, registry, project, or Partial was added.
`WorkspaceSamplePickerImageBoundaryContract` passed 6/6 in Debug and Release;
Solution/Smoke Debug and Release builds passed with 0 warnings/errors;
RefactorAudit, Readiness Debug/Release, DocumentationIndex, issue-ledger, and
targeted diff checks passed. Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\sample-picker-image-boundary-20260913\phase-summary.txt`.

There is no active 2D automation. The `openvisionlab-2d` schedule remains
deleted; this slice was executed once on demand. Full WPF theme/DPI/input/monitor,
camera/GPU/SDK, long-running native shutdown, and DLL redistribution rights remain
unverified.

## Current N-image verification image boundary — 2026-09-13

Status: `PL-0031 RESOLVED`. This bounded continuation moved only the duplicate
file-backed preview decode from `VisionToolNImageVerificationController` to the
existing `OpenVisionBitmapImagePreviewFactory`. The controller still owns image
selection, verification state, `SelectedRow`, result projection, and the
`SelectedSourceImage`/`SelectedDrawingImage` binding properties.

The call path is:
`VisionToolNImageVerificationWindow.xaml` binding ->
`VisionToolNImageVerificationController.LoadSelectedEvidence` ->
`LoadBitmap(path)` facade ->
`OpenVisionBitmapImagePreviewFactory.TryCreateFromPath(path, 0)`.
The controller no longer constructs `FileStream`/`BitmapImage` or owns the
`OnLoad`/`Freeze` decode policy. Binding names and `BitmapImage` types, missing or
corrupt path `null`, and valid frozen preview behavior remain unchanged. No new
interface, manager, wrapper, registry, project, or Partial was added.

The focused `ToolNImageVerificationImageBoundaryContract` passed 6/6 in Debug and
Release. Solution/Smoke Debug and Release builds passed with 0 warnings/errors;
RefactorAudit, Readiness Debug/Release, DocumentationIndex, issue-ledger, and
targeted diff checks passed. Evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\tool-n-image-verification-image-boundary-20260913\phase-summary.txt`.
Full WPF visual/theme/DPI/
monitor/input, hardware/SDK/GPU, long-running native shutdown, and third-party DLL
redistribution rights remain unverified.

## Current View owner refactor — 2026-09-12

Status: `PL-0021 M1 VERIFIED; M2 VERIFIED; M3 VERIFIED; M4 VERIFIED / RESOLVED`;
the five-minute heartbeat `openvisionlab-2d-1-5` was paused after the final audit
closed the issue. User input, hardware, and long-running runtime checks were not
a reason to wait; unavailable checks are recorded as unverified.

M1 moved the independently owned Pipeline Review layout state and visual policy
from `OpenVisionPipelineReviewView.xaml.cs` into the concrete
`Views/OpenVisionPipelineReviewLayoutController.cs`. The controller owns
compact/details/step-flow state, row sizing, visibility, toggle icons, and
localized tooltips. The View remains the XAML event adapter, public View event
owner, and subscription/unsubscription owner. XAML names, bindings, pipeline
execution, image ownership, and shutdown contracts were preserved.

The shortest route is
`OpenVisionShellHostView -> ShowPipelineReview ->
OpenVisionPipelineReviewDocument -> OpenVisionPipelineReviewView ->
OpenVisionPipelineReviewLayoutController`. M1 evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\view-owner-refactor-20260912\\m1-layout-owner`
(`phase-summary.txt`, focused screenshot smoke, contract checks, source audit).
The Debug solution build passed with zero warnings and errors; the focused
Pipeline Review screenshot smoke passed with layout/text/internal errors zero.
Full WPF theme/DPI/monitor matrix, camera/SDK, and long-running native behavior
remain `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

M2 moved the five disposable Pipeline Review image/diagnostic resources from
`OpenVisionPipelineReviewView.xaml.cs` into the concrete
`Views/OpenVisionPipelineReviewImageResourceOwner.cs`. Incoming `Bitmap`s are
still cloned, previous resources are disposed before replacement, and the View
calls `Dispose` after event unsubscription. The ViewModel BitmapImage contract,
render services, selection policy, and public/XAML surface are unchanged.
M2 evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\view-owner-refactor-20260912\\m2-image-lifetime`
(`phase-summary.txt`, focused screenshot, readiness, contract, and source audit).

M3 moved the independent Recipe Manager panel drag state and pointer-capture
cleanup from `OpenVisionShellHostView.xaml.cs` into
`Shell/Recipe/OpenVisionShellHostRecipePanelDragController.cs`. The Shell View
keeps only XAML event adapters and registers controller disposal with the
existing lifecycle owner. The panel offset clamp and test callback contract are
preserved. M3 evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\view-owner-refactor-20260912\\m3-shell-ui`
(`recipe-manager-smoke.txt`, `window-chrome-smoke.txt`, readiness, refactor
audit, and source owner check). Runtime theme/DPI/monitor/input matrix,
camera/SDK, and long-running native behavior remain unverified.

M4 final audit and first-contributor assessment are recorded in
`docs/reports/OPENVISIONLAB_VIEW_OWNER_REFACTOR_20260912.md` and
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\view-owner-refactor-20260912\\m4-final-audit`.
The final source inventory records 819 C# files, 60 XAML files, 59 protected
partial declarations, 34 project references, and zero project cycles. Debug and
Release solution builds, readiness, UI contracts, screenshot runner contract,
focused Pipeline Review/Recipe Manager screenshots, documentation index,
refactor audit, and diff checks passed. Remaining WPF theme/DPI/monitor/input,
camera/SDK/GPU, and long-running native qualification is explicitly unverified.
Do not split by file length alone or reopen the completed layout, image-resource,
or drag owners.

## Current RecipeCommandSurface cleanup — 2026-09-12

Status: `PL-0022 M1 VERIFIED; M2 VERIFIED; M3 VERIFIED; M4 VERIFIED / RESOLVED`;
the five-minute heartbeat `openvisionlab-2d-1-5` was reused for this slice and is
paused after the final audit. The scheduled work did not wait for optional user
input, hardware, or unavailable UI matrices.

The current owner remains the concrete
`src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/RecipeCommandSurface.cs`
(`OpenVisionShellHostRecipeCommandSurface`). The active route is
`OpenVisionShellHostView` constructor -> `RecipeCommandSurface` constructor ->
direct `RelayCommand` assignments -> `RecipeCommands` XAML bindings -> existing
Recipe/Review/Validation/Pipeline/Layer owners. The command surface writes its
binding selections, projections, and command state; the existing execution,
step-edit, validation, persistence, and review owners retain their own mutable
state. The Shell/session owns release and shutdown; the surface has no
independent `IDisposable` or native lifetime. XAML property/command names,
callbacks, Preview/Run routing, cancellation, and shutdown contracts were not
renamed or rerouted.

M2 removed the unreachable `#region Command creation` and its ten
`Initialize*Commands` helpers (153 lines). The constructor's direct command
wiring was already the live path, so the cleanup removed dead code without
creating a manager, interface, provider, wrapper, or manual partial. The file is
now 9,923 lines with 11 responsibility regions. The shortest code-reading route
is the Shell constructor, then the RecipeCommandSurface constructor and its
binding properties, followed by the existing execution/review/validation owner
and its focused contract.

The first-contributor assessment is improved because the only command creation
path is now visible at the constructor and dead helper names no longer suggest a
second lifecycle. The surface is still broad and owns many XAML projections; a
future split requires a new independently owned state, test seam, or lifetime,
not file length or another partial. Recipe already has explicit
`CommandSurface`, `Context`, `IntentSkills`, `Models`, `PropertyGrid`,
`Qualification`, `Review`, `Validation`, and `Views` folders, so no new folder was
introduced.

Evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\recipe-command-surface-refactor-20260912`
(`m1-audit`, `m2-remove-dead-command-region`, `m3-contracts`, and
`m4-final-audit`). The Debug solution build passed with zero warnings/errors;
the copied-runtime Recipe execution contract passed 12/12; focused validation
evidence, step-preview, Shell Recipe lifecycle, and RefreshOptions contracts
passed 4/4, 5/5, 7/7, and 1/1. Readiness, refactor audit, documentation index,
and diff checks passed. Full WPF visual theme/DPI/monitor/input, modal file
interaction, camera/SDK/GPU, and long-running native shutdown remain
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

The next Recipe priority is contract-focused feature work or a newly reproduced
independent owner boundary; do not reopen this completed cleanup without a new
requirement, reproducible defect, failed criterion, or dependency boundary.

## Current Recipe validation orchestration refactor — 2026-09-12

Status: `PL-0023 M1 VERIFIED; M2 VERIFIED; M3 VERIFIED; M4 VERIFIED / RESOLVED`;
the existing five-minute heartbeat `openvisionlab-2d-1-5` was reused for this
bounded slice and is paused after the final audit. The scheduled work did not
wait for optional user input, hardware, or unavailable WPF runtime matrices.

The current owner map is intentionally small. `RecipeCommandSurface` remains the
owner of validation-suite selection, `CanExecute`, XAML command/property
bindings, status/result projection, and the Shell callback boundary. The
existing `OpenVisionRecipeExecutionSessionViewModel` now owns the complete
validation-suite scope dispatch and continues to own running/stop state, status,
command-state notifications, batch-save events, and release through the existing
Shell/session lifetime. Its dispatch reuses the existing selected-sample,
Good/Bad pair, Catalog, and Local validation-set workflow owners.

The shortest reading route is:

```text
OpenVisionShellHostView constructor
  -> RecipeCommandSurface constructor and RunValidationSuiteCommand binding
  -> RecipeCommandSurface.RunValidationSuiteAsync (CanExecute + selected snapshots)
  -> OpenVisionRecipeExecutionSessionViewModel.RunValidationSuiteAsync (scope dispatch)
  -> selected/pair/catalog/local execution owner
  -> existing status, BatchRunSaved, CommandStateChanged, and Recipe/Review projections
```

Before PL-0023 the scope branch and two forwarding wrappers were in the
CommandSurface. They were removed without changing XAML names, selection or
`CanExecute` policy, Preview/Run explicitness, status/cancellation behavior,
Recipe/XML and Layer routing, or shutdown ownership. No new class, interface,
manager, provider, factory, wrapper, or manual partial was introduced.

Focused evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\recipe-validation-orchestration-refactor-20260912`.
The solution Debug and x64 Smoke Debug builds passed with zero warnings/errors;
the copied-runtime dispatch contract passed 13/13, validation evidence 4/4,
step preview 5/5, Shell Recipe lifecycle 7/7, and RefreshOptions command-state
1/1. The exact commands and output files are recorded under `m3-focused` and
`m2-dispatch-copied-rerun-7`.

For a first contributor, the validation path now has one visible policy owner
for scope dispatch and one visible binding owner, so a scope change no longer
requires tracing a second forwarding branch in the Surface. The Surface and
execution session are still broad existing owners; a future split needs a new
independent state, test, lifetime, defect, or dependency boundary. The Recipe
folders and protected framework/XAML partials remain unchanged.

Remaining limits are explicit: `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.
WPF theme/DPI/monitor/input, camera/SDK/GPU, and long-running native shutdown
were not exercised. The next separate priority is a newly reproduced owner
boundary or contract-focused feature; do not reopen PL-0023 without new
evidence.

## Current RecipeCommandSurface Clipboard boundary refactor — 2026-09-12

Status: `PL-0024 M1 VERIFIED; M2 VERIFIED; M3 VERIFIED; M4 VERIFIED / RESOLVED`.
This is a new bounded continuation of the RecipeCommandSurface work; PL-0022
and PL-0023 remain closed. The existing five-minute heartbeat was not duplicated
for this source change, and optional user input or unavailable runtime matrices
did not block the implementation.

The external WPF Clipboard boundary is now explicit. `RecipeCommandSurface`
still owns Copy/Paste commands, `CanExecute`, text composition, status/error
projection, and `LlmXmlDraftText`. `OpenVisionShellHostView` owns the actual
`System.Windows.Clipboard` calls and supplies three narrow callbacks:
`copyTextToClipboard`, `clipboardContainsText`, and `readClipboardText`.

The shortest reading route is:

```text
OpenVisionShellHostView RecipeCommands construction
  -> RecipeCommandSurface callback fields/constructor
  -> Copy/Paste command handlers
  -> Shell Clipboard callback
  -> existing status or XML draft binding
```

No interface, manager, provider, factory, wrapper, or manual partial was added.
The binding names `CopyOperatorHandoffReportCommand`,
`CopySelectedRecentBatchRunReviewCommand`, `CopyLlmPromptCommand`,
`CopyLlmReviewBundleCommand`, and `PasteLlmXmlDraftFromClipboardCommand` remain
unchanged. Callback failure continues to use the existing localized status
messages. The `Binding Properties` region now contains a clearly marked nested
`Commands` navigation region; this is navigation-only and does not change
initialization order.

Evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\recipe-command-surface-clipboard-boundary-20260912`.
The copied-runtime Clipboard boundary contract passed 3/3. RefreshOptions,
Validation Set success/error, and Shell Recipe lifecycle contracts passed 1/1,
1/1, 1/1, and 7/7. Solution and Smoke Debug builds passed with zero
warnings/errors. Actual desktop Clipboard, WPF visual/theme/DPI/monitor/input,
camera/SDK/GPU, and long-running native shutdown remain
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

The remaining broad Surface regions are binding projections and orchestration
that already depend on protected XAML names or existing concrete owners. They
should be reopened only for a new independently owned state/lifetime/test
boundary, a reproducible defect, or a changed dependency contract.

## Current RecipeCommandSurface Dispatcher boundary refactor — 2026-09-12

Status: `PL-0025 M1 VERIFIED; M2 VERIFIED; M3 VERIFIED; M4 VERIFIED / RESOLVED`.
This is a separate, evidence-backed continuation after PL-0024. The existing
five-minute heartbeat remains the single automation record and is paused after
this slice; optional user input and unavailable runtime matrices did not block
the source refactor.

`RecipeCommandSurface` no longer owns direct WPF dispatcher scheduling. The
existing `OpenVisionShellHostView` composition owner supplies two concrete
callbacks: `yieldToUi` preserves the Background-priority yield used during
recipe selection/creation, and `flushUi` preserves the Render-priority flush
used when entering the switching state. The Surface still owns recipe
selection/creation state, command state, status projection, and all existing
XAML names. The callbacks do not own recipe state or disposable resources.

The shortest reading route is:

```text
OpenVisionShellHostView RecipeCommands construction
  -> RecipeCommandSurface constructor callback tail
  -> SelectedRecipeName setter / CreateRecipeCommand
  -> SelectRecipeAsync / CreateAndSwitchRecipeAsync
  -> yieldToUi / flushUi callback
  -> existing RecipeWorkspaceService, switchRecipe, refresh, and status path
```

No scheduler interface, manager, factory, provider, wrapper, or manual partial
was added. The two parameters were appended as optional constructor arguments,
so existing contract callers remain source-compatible. A direct
`System.Windows.Threading.Dispatcher` and `Application.Current.Dispatcher`
search in the Surface is clear; the concrete WPF implementation remains in the
Shell composition owner.

Evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\recipe-command-surface-dispatch-boundary-20260912`.
The copied-runtime scheduler contract passed 2/2; the Clipboard regression
passed 3/3; RefreshOptions passed 1/1; Validation Set success/error projection
passed 1/1 each. Solution and Smoke Debug builds passed with zero warnings and
errors. Actual WPF desktop thread contention, theme/DPI/monitor/input,
camera/SDK/GPU, and long-running native shutdown remain
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

The Surface `BitmapSource` overlay binding and recipe/file policy remain in
their current owners; PL-0026 moved the file decode itself to the existing
preview factory. Reopen this owner only for a new independent state/lifetime/
test boundary, a reproducible defect, or a changed dependency contract; do not
split by file length alone.

## Current RecipeCommandSurface image-file boundary refactor — 2026-09-12

Status: `PL-0026 M1 VERIFIED; M2 VERIFIED; M3 VERIFIED; M4 VERIFIED / RESOLVED`.
This slice followed the PL-0025 dispatcher boundary after the final audit found
four repeated WPF file-image decode sites. The existing five-minute heartbeat
remains the single automation record; it is paused after this verified slice.

The existing `OpenVisionBitmapImagePreviewFactory` now owns two cohesive
operations: `CreateFromPath` loads a frozen `BitmapSource` with `OnLoad` for the
locator overlay, and `ReadPixelSize` reads the dimensions used by the Pin Gap
ROI suggestion, Pin Array Gap validation, and Locator intent validation context.
`RecipeCommandSurface` keeps the `LocatorEvidenceOverlayImage` XAML property,
recipe/intent validation state, and localized status/error projection. It no
longer constructs `BitmapImage` or `BitmapFrame` directly.

The shortest reading route is:

```text
RecipeCommandSurface locator/intent method
  -> OpenVisionBitmapImagePreviewFactory.CreateFromPath / ReadPixelSize
  -> existing validation or status projection
  -> LocatorEvidenceOverlayImage / ROI / intent binding
```

No new image service, interface, manager, provider, wrapper, folder, or partial
was added. The overlay file-not-found exception and the dimension decode
failure paths remain mapped by the existing Surface callers. The factory freezes
the `OnLoad` result, so a returned preview does not retain an open file stream.

Evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\recipe-command-surface-image-boundary-20260912`.
The copied-runtime image contract passed 4/4; dispatcher 2/2; Clipboard 3/3;
RefreshOptions 1/1; Validation Set success/error 1/1 each. Solution and Smoke
Debug builds passed with zero warnings and errors. Documentation, readiness,
refactor, issue-ledger, and diff checks are recorded under `m4-final-audit`.
Actual WPF visual/theme/DPI/monitor/input, camera/SDK/GPU, and long-running
native shutdown remain `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

Remaining direct `File.Exists`, `Path`, XML, and recipe/pipeline storage calls
were not merged into the image boundary because they have multiple existing
owners and no single replacement boundary was proven in this audit. Reopen the
Surface only for a new independent state/lifetime/test boundary, a reproducible
defect, a failed criterion, or a changed dependency contract.

## Current Partial Structure Program — 2026-09-11

Status: `PL-0013 COMPLETE`; P1 inventory `VERIFIED`; P2 owner proof `VERIFIED`;
prior P3/P4/P5 evidence is retained as history, and the reopened manual partial
families are now closed by D4/D5.

The user explicitly reopened a new boundary to structure all existing partial
types. The execution plan is
`docs/reports/OPENVISIONLAB_PARTIAL_STRUCTURE_PLAN_20260911.md` and the durable
ledger is `.proofline/issues/PL-0013.json`. The single five-minute heartbeat
`openvisionlab-2d-partial` was closed after D5 evidence was recorded; no recurring
executor remains for this completed boundary.

The first source-only inventory found 108 actual partial declarations in `src`
and `tools`; the reopened implementation pass then consolidated the safe manual
families. The current declaration-only recheck finds 59 protected compiled partial
declarations and two smoke-contract string matches. The latest source audit
baseline is `CSharpFiles=815`, `XamlFiles=60`, `ProjectCycles=0`, and
`ShellStorageCalls=0`; the partial count is being corrected to report declarations
and text matches separately.

The latest source and verification evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\partial-structure-d5-20260911`
(`protected-partial-inventory.csv`, `protected-partial-owner-map.csv`,
`summary.txt`, build/readiness/audit logs, and stale-path search output). The
earlier evidence remains at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\partial-structure-p1-20260911`
(`partial-inventory.csv`, `partial-groups.csv`, `partial-owner-map.csv`,
`non-declaration-matches.csv`, `summary.txt`). P2 rechecked the cohesive
families and found zero independently owned extraction boundaries, then the user
reopened the requirement and approved source consolidation of same-owner manual
partials. The proof is
at `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\partial-structure-p2-20260911`
and the durable report is
`docs/reports/OPENVISIONLAB_PARTIAL_STRUCTURE_PROOF_20260911.md`.
`OpenVisionDockWorkspaceController`, `OpenVisionShellHostDockedLayerOrchestrator`,
`RoiImageCanvasViewModel`, docking gesture, Pipeline Review event parts, Shell
interactions, and the two Tool Shell layout parts now live in their concrete
owner files. Recipe CommandSurface is physically grouped under
`UI/Menu/Wpf/Recipe/CommandSurface` with one concrete
`RecipeCommandSurface.cs` facade organized by responsibility regions. The
historical P3/P4 evidence protected OpenGL rendering composition, Recipe shared
state, test hooks, XAML, designer, and smoke contracts. The new completion design
is `docs/reports/OPENVISIONLAB_PARTIAL_STRUCTURE_COMPLETION_DESIGN_20260911.md`.
Its completed slices are D0 inventory correction, D1 audit false-positive
separation, D2 Shell TestSurface extraction, and D3 OpenGL concrete renderer
ownership. D2 moved the manual
`OpenVisionShellHostView.TestHooks.cs` behavior into
`src/OpenVisionLab/UI/Menu/Wpf/Shell/Support/OpenVisionShellHostViewTestSurface.cs`
and kept the existing `*ForTest` names through root compatibility forwarders.
D2 Debug build, ReadinessCheck, runtime stability smoke, and structural audit
passed. The D2 audit reported
`PartialDeclarations=77;PartialTextMatches=2;ProjectCycles=0;ShellStorageCalls=0`.
Evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\partial-structure-d2-20260911`.
The D3 evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\partial-structure-d3-20260911`;
the public API design inventory is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\partial-structure-d3-design-20260911`.
The latest audit reports
`PartialDeclarations=59;PartialTextMatches=2;ProjectCycles=0;ShellStorageCalls=0`.
D4 consolidated the Recipe CommandSurface partial family into
`src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/RecipeCommandSurface.cs`
and updated the readiness source-family check. Its design evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\partial-structure-d4-design-20260911`;
its build, contract, body-hash, and audit evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\partial-structure-d4-20260911`.
Solution/Smoke Debug and Release builds, ReadinessCheck, Recipe focused contracts,
source-body comparison, protected inventory, and refactor audit passed. D5
protected-boundary closure is complete. Runtime UI/theme/DPI/monitor,
camera/SDK, and long-running native qualification remain source-only/unverified;
they are not a reason to wait for user input in this source slice.

## Current Junior Discoverability Structure Program — 2026-09-11

Status: R16 `VERIFIED`; R17 `VERIFIED`; R18 `VERIFIED`; R19 `VERIFIED`; R20 `VERIFIED`; R21 `VERIFIED`; R22 `VERIFIED`; R23 `VERIFIED`; R24 `VERIFIED`; R25 `VERIFIED` (no-change); R26 `VERIFIED`; R27 `VERIFIED`; R28 `VERIFIED`; R29 `VERIFIED` (precheck boundary pending); R30 `VERIFIED`.

R21 completed a contract-safe internal rename from
`OpenVisionShellHostDockedLayerWorkspaceComposition` to
`ShellDockedLayerWorkspaceComposition`. R22 completed the same contract-safe
rename for `OpenVisionShellHostDockingTestFacade` to `ShellDockingTestFacade`.
R23 completed the same contract-safe rename for
`OpenVisionShellHostLayerTestFacade` and its callback holder to the
`ShellLayerTestFacade` names. R24 completed the same contract-safe rename for
`OpenVisionShellHostToolTestFacade` and its callback holder to the
`ShellToolTestFacade` names. R25 inspected
`OpenVisionShellHostToolWindowLifecycleController`; its name reflects the real
Shell/ToolWindow/Lifecycle responsibility and no contract-safe shortening was
proven, so R25 records no-change evidence. No new source boundary remains in
this program; the heartbeat was deleted after the evidence was recorded.

R21 evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-structure-r21-audit`.
The existing call path remains `OpenVisionShellHostView constructor ->
OpenVisionDockedLayerWorkspaceRuntimeFactory.CreateComposition ->
ShellDockedLayerWorkspaceComposition -> Attach/CreateTestFacade`; mutable state
and release remain owned by the workspace runtime/orchestrator and the Shell
dispose path. Debug/Release builds both completed with zero warnings and zero
errors; runtime WPF/UI and hardware boundaries remain unverified.

R22 evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-structure-r22-audit`.
The call path remains `OpenVisionShellHostView constructor ->
ShellDockedLayerWorkspaceComposition.CreateTestFacade -> ShellDockingTestFacade`;
the facade forwards existing diagnostics/actions and owns no mutable state or
disposable resource. Debug/Release builds both completed with zero warnings and
zero errors; runtime WPF/UI and hardware boundaries remain unverified.

R23 evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-structure-r23-audit`.
The call path remains `OpenVisionShellHostView constructor -> ShellLayerTestFacade
-> OpenVisionShellHostView.TestHooks`; mutable state and disposal remain owned by
the existing presenters/controllers and Shell View. Debug/Release builds both
completed with zero warnings and zero errors; runtime WPF/UI and hardware
boundaries remain unverified.

R24 evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-structure-r24-audit`.
The call path remains `OpenVisionShellHostView constructor -> ShellToolTestFacade
-> OpenVisionShellHostView.TestHooks`; mutable state and disposal remain owned by
the existing ViewModel/presenters/controllers/documents and Shell View. Debug/
Release builds both completed with zero warnings and zero errors; runtime WPF/UI
and hardware boundaries remain unverified.

R25 evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-structure-r25-audit`.
The lifecycle controller remains the concrete owner used by Shell View,
ToolWindow controller, Chrome commands, Recipe, Session, and ShellToolTestFacade.
No code changed; the previous R24 Debug/Release build remains the last changed
code gate. Runtime WPF/UI and hardware boundaries remain unverified.

## Current whole-repository junior review — 2026-09-11

The current-source review is recorded in
`docs/reports/OPENVISIONLAB_JUNIOR_WHOLE_REPOSITORY_REVIEW_20260911.md` with
inventory evidence at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-whole-repo-review-20260911`.
The assessment is: concrete module and ownership boundaries are established, but
first-time contributor cognitive load remains medium-high at Shell composition,
Recipe CommandSurface, and Pipeline Review presentation state. The report records
the complete `Program.Main -> Application.Run -> ShellWindow -> ShellView` route,
the `Image -> Layer -> Tool -> Inspection -> Pipeline -> Recipe -> Result ->
Review` reading order, and the reasons not to reopen completed owners merely for
file length or class-name preference.

The current inventory is 816 C# files, 60 XAML files, 27 projects, 34 project
references, zero project cycles, 59 protected compiled partial declarations, and
two smoke-contract literal matches. These are navigation signals, not defect
counts. The source review found no new independently owned production boundary
that justifies another Shell/Recipe split in this slice. ViewModel WPF/image/path
signals, async-void adapters, cleanup catches, Pipeline drain behavior, and
alternate WPF runtime rows remain explicitly classified for later focused work.
The follow-up Sample Picker navigation slice below added only member regions and
was verified separately; it did not move production behavior or add a new owner.

The new requirement is to make Shell ownership and the first code-reading path
obvious to a first-time contributor. The current owner remains
`OpenVisionShellHostView` as the composition layer. Existing concrete owners
remain responsible for mutable state and release: Session, Layer, Workspace,
Tooling, Recipe, Documents, and Pipeline Review. The first call path is
`Program.Main -> OpenVisionLabApplication.Run -> OpenVisionShellHostWindow ->
OpenVisionShellHostView`; the Pipeline Review path is
`ShowSelectedTool -> ShowPipelineReview -> document restore/create ->
ShowDockedDocumentWorkspace -> OpenVisionPipelineReviewDocument`.

R16 records this creation/ownership/call-path map and classifies manual partials
as generated/framework composition, event/test hooks, or responsibility files.
The inventory found 59 Shell composition fields and 37 partial files in the WPF
area; the full refactor audit remains at 847 C# files, 60 XAML files, 110 partial
declarations, zero project cycles, and zero Shell storage calls. It preserves XAML
dependency properties, public/test facade contracts, type names, namespaces,
serialization, and Dispose order. R17 now audits one concrete boundary in the
`OpenVisionShellHostRecipeCommandSurface` partial family. The audit found ten
partial files with cross-cutting binding-facing state and callbacks; existing
LLM draft, validation-set, pipeline exchange, run-history, and qualified-snapshot
owners are already called, but no independent snapshot/result lifecycle boundary
with a focused test seam was proven. R17 therefore made no production extraction.
R18 completed one contract-safe internal rename:
`OpenVisionShellHostLayerListRefreshResult` became `LayerListRefreshResult` in
the Shell/Layers source-only call path; no XAML/public/reflection/serialization
contract changed. R19 audited
remaining mixed folders, and R20 completed the final source/focused-smoke
discoverability pass. No new interface, factory, manager, forwarding wrapper,
or manual partial is admitted solely to shorten a file or class name.

R16 evidence is in
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-structure-r16-audit`
(`shell-composition-fields.csv`, `shell-partial-inventory.csv`, `summary.txt`).
`TestDocumentationIndex.ps1`, `Invoke-RefactorAudit.ps1 -Verify`, and solution
Debug/Release builds completed with zero warnings and zero errors. R17 evidence
is in `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-structure-r17-audit`
(`recipe-command-partial-coupling.csv`, `summary.txt`). R18 old-name absence,
Debug/Release build, readiness, refactor audit, documentation index, and diff
checks also passed. R19 evidence is in
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-structure-r19-folder-audit`
(`root-file-inventory.csv`, `large-folder-inventory.csv`, `summary.txt`); no
additional move met the dependency and ownership gate. R20 completed the source route
and focused Shell smoke proof; Runtime UI
themes, DPI, monitor, pointer, keyboard, camera/SDK, and long-run native behavior remain
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요` until separately run.

R15 is the latest completed independent source slice. R14 completed the latest source
level Shell/Pipeline Review boundary audit. The current source
invokes the distribution child check through `pwsh` (commit `19d7ab0b`), and R1
passes in both `-SkipLaunch` and no-`-SkipLaunch` clean candidate evidence. The
installed Release EXE exit-code and monitor placement contract is also verified.
The R6 DisplayManager lifetime correction is verified by a sequential Shell
Window run plus the representative default/performance suites; if the scope
reopens, the next separate boundary is full WPF UI state/theme/DPI/monitor/long-
run qualification.
The P0 `Application.Run` exit-code propagation, DisplayManager lifetime, and
sample-review async deadlock corrections, and the P1 Pipeline drain-state,
Threshold suggestion policy, and Pipeline Review smoke contract slices are
complete for their source/build/focused-contract scopes. Narrowly
justified P2 naming/member-layout
cleanup was audited and left unchanged because no new defect or independent
boundary justified reopening a completed owner. R10 named the existing Pipeline
Review branch and recorded its owner map without adding a new owner. R11 moves
two reproduced code-behind state/I/O paths to existing concrete owners. R12 moves
the reproduced readiness policy to the existing Shell ViewModel owner. R13 moves
the direct Recipe save callback to the existing Recipe controller. R14 rechecked
remaining Shell/Pipeline Review code-behind and found only UI-specific state,
event, image, and callback adapters; it did not add a split. These slices do not
reopen the Shell composition boundary. Remaining checks cover the full WPF UI
matrix and hosted CI rerun.

The current implementation baseline is branch
`codex/public-sample-ux-docs`, HEAD
`0a77e60e444b12757565ee977216a994446b53a6`, target framework
`net8.0-windows7.0`, product version `2.2.0-dev.2`. The pasted analysis used an
older/different baseline; current source and current evidence remain
authoritative. Existing folder organization, the R15 Common responsibility
folders, MVVM/partial audit, ImageSpace ownership, XML atomic save, AppPath
containment, and BackgroundLoopWorker owners remain closed unless a new
reproducible defect or changed boundary is recorded. R15 changes only physical
paths under `src/OpenVisionLab/Common` and preserves namespace, type, Recipe/XML,
XAML, and SDK contracts.

## Current Sample Picker member navigation follow-up — 2026-09-11

Status: `R26 VERIFIED` for the behavior-neutral source navigation and focused
runtime contract scope.

The existing owner remains
`OpenVisionWorkspaceSamplePickerViewModel`. Its call path is
`OpenVisionWorkspaceSamplePickerWindow.TrySelectSample -> view model ->
SamplesView/commands/pair-selection state`; Learn document file and process work
continues through the existing
`OpenVisionWorkspaceLearnDocumentService.OpenDocument`. No Interface, Factory,
Manager, new partial, or duplicate file owner was introduced.

The only production-source change was adding responsibility-oriented regions to
`src/OpenVisionLab/UI/Menu/Wpf/Workspace/Samples/OpenVisionWorkspaceSamplePickerViewModel.cs`:
Fields, Constructors, Properties, Catalog Selection and Filtering, Commands,
Pair Selection and Filtering, Presentation Helpers, Image Preview, and
Localization Helpers. Method bodies, binding/public names, mutable-state owner,
and call order are unchanged. The normalized logic SHA-256 remains
`b3bf3110e029044a9e341ec1f938a8aae9ac63ee7099264d248e6394c53d3528`, and the
region balance is zero.

Verification evidence is under
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-whole-repo-review-20260911`:

- Solution Debug/Release builds: 0 warnings, 0 errors.
- `wpf_shell_host_workspace_sample_picker` visible UI smoke: `OK`, 2249 ms,
  1040x742 capture; monitor topology recorded in `sample-picker-ui`.
- `OpenVisionReadinessCheck` Debug/Release: passed.
- `Invoke-RefactorAudit.ps1 -Verify`: passed with 816 C# files, 60 XAML files,
  59 protected compiled partial declarations, project cycle 0, and Shell storage
  calls 0.
- `TestDocumentationIndex.ps1`: passed with `IndexedPaths=289`, `Routes=16`,
  `RootRedirects=102`.

The current owner is closed against further name-only or file-length changes.
Reopen it only for a reproducible file/image lifetime defect, an independent
state/lifetime boundary, or a changed binding contract. Full WPF theme, DPI,
monitor/input matrix, camera/SDK/GPU, and long-run native shutdown remain
unverified and do not block independent source work.

## Current WPF runtime environment matrix — 2026-09-11

Status: `R27 VERIFIED` for environment recording; alternate runtime rows remain
explicitly unverified.

The new report is
`docs/reports/OPENVISIONLAB_WPF_RUNTIME_ENVIRONMENT_MATRIX_20260911.md` and the
machine-readable evidence is under
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\wpf-runtime-environment-20260911`.
The current Windows 11 session is 96 DPI (100%) on both monitors:
`DISPLAY1` primary `2560x1440` and `DISPLAY2` left `1920x1080`. The supported
125/150/175/200% rows are unavailable in this session. One-monitor/headless
conditions are also unavailable. The inspected Shell path exposes no runtime
alternate-theme switch; its global theme imports MaterialDesign Light and uses
the existing semantic product brushes.

The existing 96% two-monitor physical qualification remains the authoritative
executed runtime evidence and was not repeated. This R27 slice only records
available and missing rows, makes no display-setting or source change, and does
not wait for a user, hardware, or external test machine. Full WPF state/theme/
DPI/input, camera/SDK/GPU, and long-run native behavior remain
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.
`TestDocumentationIndex.ps1` passed with `IndexedPaths=293`, `Routes=16`, and
`RootRedirects=102` after the new report was registered.

## Current Pipeline Review execution recheck — 2026-09-11

Status: `R28 VERIFIED` for the current-source execution, image-lifetime,
stale-callback, and document-revision contract boundary. No production source,
XAML, binding, public name, Recipe/XML contract, or Dev/Original boundary
changed in this slice.

The concrete execution owner remains
`src/OpenVisionLab/UI/Menu/Wpf/PipelineReview/Execution/OpenVisionPipelineReviewExecutionController.cs`.
The shortest call path is
`OpenVisionPipelineReviewDocument -> OpenVisionPipelineReviewExecutionController.RunAsync ->
VisionPipelineExecutionService.RunPreparedAsync -> OnStepExecutionUpdated/CompleteRun ->
Pipeline Review projection`. The controller owns run identity/generation,
`IsRunning`, cancellation, step summaries, review image cache, callback
atomicity, and run-result disposal. The View remains a display and UI-lifecycle
owner. No new wrapper, interface, manager, or partial was justified.

Evidence is under
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\pipeline-review-recheck-20260911`.
`VisionRecipeRunnerSmoke` Debug and Release builds both completed with 0
warnings and 0 errors. In each configuration the layer-image-owner,
cache-lifetime, stale-callback, execution, and document-revision contracts all
returned exit code 0 (10/10 runs). Reset/Close cache retirement, generation
guards, duplicate-run prevention, timeout/cancel versus worker drain, late
result disposal, async disposal, and revision rejection were not regressed.

No current Pipeline Review defect was reproduced. The owner is closed against
name-only, file-length, or partial changes. Full WPF alternate theme, Wide/
Compact, 125/150/175/200% DPI, pointer/keyboard, camera/SDK/GPU, permanent
native hang, and long-run shutdown remain
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`. Detailed evidence is
`docs/reports/OPENVISIONLAB_PIPELINE_REVIEW_RECHECK_20260911.md`.

## Current release precheck — 2026-09-11

Status: `R29 VERIFIED` for the current Dev source-build and platform precheck;
the exact clean commit, commit/push, tag, release, and deployment boundaries
remain pending.

`tools/VerifySourceBuild.ps1` passed locked restore, Debug/Release solution
build, readiness, vendored DLL checks, and expected executables. The Release/
Any CPU `RunVisionPlatformPrecheck.ps1 -SkipUi` passed with 33 public catalog
rows runnable and OK, 0 NG rows, 0 sample failures, and all listed non-UI/WPF
shell/tutorial gates `OK`. The evidence is under
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\pipeline-review-recheck-20260911\\release-precheck`.

`NewOpenVisionReleaseEvidence.ps1` recorded `ReleaseGateOk=True` and
`TagReady=False` because the worktree contained 155 changed files at the
precheck timestamp from the
approved refactor/documentation batch. The canonical clean-clone
`VerifyReleaseCandidate.ps1` gate was therefore not claimed for this state.
No version was changed from `2.2.0-dev.2`, and no commit, push, tag, release
publication, deployment, or `C:\\Git\\2D\\Original` mutation was performed.
The checkout is `codex/public-sample-ux-docs`; `origin/main` is
`e551f307d4150d414cb24f7a8de23dece9b9c23c`, while the current HEAD is
`0a77e60e444b12757565ee977216a994446b53a6` with merge base
`1528d3b869ce67f439ac28fc2b8565cde58c28b2`. A main promotion requires an
explicit merge/rebase or separate promotion worktree; no history choice was
guessed.
Detailed evidence is
`docs/reports/OPENVISIONLAB_RELEASE_PRECHECK_20260911.md`.

The next action is an exact diff/ownership review before any local commit or
branch push. A release tag or publication remains a separate authorization
stage.

Next priorities:

1. Release precheck and clean-candidate review, without commit/push until
   separately authorized | Recommended model: `gpt-5.6-luna` | Reasoning effort:
   `high`.
2. Alternate WPF DPI/theme/topology qualification when a supported environment
   is available | Recommended model: `gpt-5.5` | Reasoning effort: `medium`.

## Current ImageCanvas debug-output cleanup — 2026-09-11

Status: `R30 VERIFIED` for the bounded production debug-output cleanup.

`src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs`
no longer writes the two `LoadMatFromFile`/`LoadImage` elapsed-time lines to
stdout, and the unused `System.Diagnostics` import was removed. The existing
call path and ownership remain
`ImageCanvas command -> RoiImageCanvasViewModel.OpenLoadImage -> ImageDialogHost
-> CanvasImageLoader -> LoadImage -> ImageCanvasDirectoryPolicy`; the Mat is
still released by the existing `using` scope. No new logger, service, partial,
or wrapper was added.

Solution Debug and Release builds both completed with 0 warnings and 0 errors;
Release ReadinessCheck passed; the active production `Console.Write` scan under
`src` returned zero. The concurrent Machine Studio test was not touched. No
product or smoke process remained after the verification. Evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\pipeline-review-recheck-20260911\\debug-output-cleanup`.
Detailed report:
`docs/reports/OPENVISIONLAB_IMAGECANVAS_DEBUG_OUTPUT_CLEANUP_20260911.md`.

## Current ImageCanvas timer-disposal follow-up — 2026-09-12

Status: `PL-0019 RESOLVED` for the reproduced timer callback lifetime defect.

The concrete owner remains
`src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs`.
`_dataTimer_Elapsed` now snapshots `_refreshTimer`, returns when the timer is
already cleared, and keeps the existing `ObjectDisposedException`/
`InvalidOperationException` boundary for a callback that races with disposal.
The call path and ownership remain
`RoiImageCanvasView -> RoiImageCanvasViewModel -> ImageCanvasControl timer`;
`RoiImageCanvasViewModel.Dispose` still stops, unsubscribes, disposes, and clears
the timer before releasing the canvas. No new service, interface, partial, or
wrapper was added.

The pre-fix reflection probe reproduced a `NullReferenceException` when the
callback ran with a null timer. The post-fix probe returned `OK`. ImageCanvas
Debug, solution Debug/Release, Debug/Release ReadinessCheck, and the
representative `wpf_shell_host_workspace_image_load` smoke all passed. Evidence
is under
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\roi-image-canvas-timer-race-20260912`.
This proves the null-timer contract and representative workspace path; it does
not prove the full WPF theme/DPI/monitor matrix, camera/SDK/GPU behavior, or
long-running native shutdown.

## Current five-area junior burden recheck — 2026-09-12

Status: `PL-0020 M1 VERIFIED`; the heartbeat continues with M2 RecipeCommandSurface.

The first recheck after PL-0019 confirms the existing Shell boundary. The actual
creation path is `Program.Main -> OpenVisionLabApplication.Run ->
OpenVisionShellHostWindow -> OpenVisionShellHostView` and the View constructor
still owns visual composition and event registration. `OpenVisionShellHostWindow`
owns the hosted View and calls `Dispose` on close; `OpenVisionShellHostSessionController`
owns session cancellation, reverse-order event cleanup, workspace/tool release, and
view-model disposal; application bootstrap owns the process-scoped DisplayManager
release after `Application.Run` returns. Existing Recipe, Layer, Document,
ToolWindow, Workspace, and Pipeline Review owners continue to own their mutable
state and contracts.

No Shell source change was justified. A file-length-only extraction would create a
mechanical partial or forwarding manager and would reopen the completed PL-0018 /
PL-0014 owner without a new defect, requirement, failed criterion, or dependency
boundary. The current PL-0019 ImageCanvas timer fix is outside this Shell call path.

The focused `Invoke-RefactorAudit.ps1 -Verify` passed with
`CSharpFiles=816`, `XamlFiles=60`, `PartialDeclarations=59`,
`PartialTextMatches=2`, `ViewModelUiIoFiles=1`, `ProjectCycles=0`, and
`ShellStorageCalls=0`; static creation/disposal search and `git diff --check` also
passed. Existing Shell build/readiness evidence was reused because this slice made
no source change. Evidence:
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-burden-recheck-20260912\\shell-recheck\\phase-summary.txt`.

Full WPF visual states, themes, DPI/monitor placement, camera/SDK/GPU behavior,
native permanent hangs, and long-run shutdown remain unverified.
## Current RecipeCommandSurface burden recheck — 2026-09-12

Status: `PL-0020 M2 VERIFIED`; the heartbeat continues with M3 Pipeline Review.

`src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/RecipeCommandSurface.cs`
remains one concrete binding-facing Recipe command/state owner (10,076 lines). Its
constructor receives the existing Shell callbacks and creates the existing step-edit,
execution, pipeline, validation-set, review, workspace, and qualified-snapshot
owners. `OpenVisionRecipeStepEditSessionViewModel` owns edit/dirty state;
`OpenVisionRecipeExecutionSessionViewModel` owns run flags, stop state, summaries,
async execution events, and child execution owners. The command surface owns the
XAML-facing selections, options, status/draft projections, command state, and
PropertyChanged ordering.

The shortest route is
`OpenVisionShellHostView constructor -> RecipeCommandSurface ->
OpenVisionShellHostView.RecipeCommands binding -> Recipe XAML -> existing
Recipe/Review/Validation/Pipeline/Layer owners`. The surface is not `IDisposable`
and has no independent native lifetime; the Shell session remains the release
boundary. The current survey found 12 named regions, 148 command initializers,
22 `File.Exists` readiness checks, one direct `File.ReadAllText` input adapter,
15 existing `VisionPipelineStorage` contract calls, and 663 `RecipeCommands` XAML
references. No new persistence owner, independent lifetime, or test seam was found.

No source change was justified. Splitting this file by region or adding a forwarding
manager would duplicate shared binding state and callback contracts. Existing
PL-0018/PL-0013 owners remain canonical. `Invoke-RefactorAudit.ps1 -Verify` passed
with `CSharpFiles=816`, `XamlFiles=60`, `PartialDeclarations=59`,
`PartialTextMatches=2`, `ViewModelUiIoFiles=1`, `ProjectCycles=0`, and
`ShellStorageCalls=0`; targeted static ownership search and `git diff --check` also
passed. Existing Recipe/Review build and focused contract evidence was reused; no
new build is claimed because this slice made no source change. Evidence:
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-burden-recheck-20260912\\recipe-command-surface-recheck\\phase-summary.txt`.

Full WPF binding/theme/DPI/monitor/input matrix, actual modal dialog/file interaction,
camera/SDK/GPU behavior, and long-run native shutdown remain unverified.
## Current Pipeline Review burden recheck — 2026-09-12

Status: `PL-0020 M3 VERIFIED`; the heartbeat continues with M4 smoke/product boundary.

The current Pipeline Review path remains explicitly divided among concrete owners:
`OpenVisionPipelineReviewDocument` owns composition, selected state, revision and
subscriptions; `OpenVisionPipelineReviewExecutionController` owns run identity,
cancellation, active revisions, summaries, cache and result-image retirement;
`OpenVisionPipelineReviewDocumentRevisionGate` rejects stale projections;
`OpenVisionPipelineReviewLayerImageOwner` returns short-lived snapshots;
`OpenVisionPipelineReviewViewModel` owns binding projections; and
`OpenVisionPipelineReviewView.xaml.cs` owns control events, display-only images and
unload cleanup.

The shortest route is
`OpenVisionShellHostToolWindowController.ShowPipelineReview ->
OpenVisionPipelineReviewDocument -> RunReviewAsync -> RevisionGate.BeginRun ->
ExecutionController.RunAsync -> VisionPipelineExecutionService.RunPreparedAsync ->
Document projection -> ViewModel/View`. Duplicate runs, cancel/reset, timeout drain,
revision stamps and stale callbacks remain guarded by the existing contracts. Bitmap
and Mat ownership remains scoped: source snapshots are disposed after context copy,
cache replacement disposes previous images, ViewModel BitmapImage projections use
`OnLoad`/`Freeze`, and View display buffers are disposed on `Unloaded`.

No source change was justified. Current Pipeline Review files have no working-tree
diff, and splitting the View or Document by file size would reopen the completed
PL-0017/R28 boundaries without a new defect or independent lifetime. The View and
ViewModel contain no direct Pipeline storage, execution service, or file I/O signal.
`Invoke-RefactorAudit.ps1 -Verify` passed with `CSharpFiles=816`, `XamlFiles=60`,
`PartialDeclarations=59`, `PartialTextMatches=2`, `ViewModelUiIoFiles=1`,
`ProjectCycles=0`, and `ShellStorageCalls=0`; targeted source search and
`git diff --check` passed. Existing R28 build and focused contract evidence was
reused; no new build or runtime PASS is claimed for this no-change slice. Evidence:
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-burden-recheck-20260912\\pipeline-review-recheck\\phase-summary.txt`.

Actual alternate themes, Wide/Compact layouts, DPI/monitor/input matrix,
camera/SDK/GPU execution, permanent native hangs, and long-run shutdown remain
source review complete / runtime UI verification required.
## Current Smoke/product boundary recheck — 2026-09-12

Status: `PL-0020 M4 VERIFIED`; the heartbeat continues with M5 final MVVM/partial/folder/module audit.

The current verification files are not product owners. Product execution starts at
`src/OpenVisionLab/Program.cs` and `OpenVisionLabApplication.Run`; headless contract
checks run from `tools/VisionRecipeRunnerSmoke/Program.cs`; WPF desktop capture runs
from `tools/PipelineViewerScreenshotSmoke/Program.cs` through
`ScreenshotSmokeTargetRunner`; and the EXE `--smoke` adapter is
`tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs`.

`OpenVisionLabEnableEmbeddedSmokeRunner` defaults to `false`. The product project
defines `OPENVISIONLAB_EMBEDDED_SMOKE` and links the Direct runner/shared smoke
primitives only when that property is explicitly enabled. Standalone smoke projects
reference the product project as test clients. The product retains mutable state and
shutdown ownership; smoke runners own fixtures, desktop windows, assertions, and D:
evidence. The 60-second route is recorded in `docs/README.md`, with detailed maps in
`docs/admin/CODEBASE_STRUCTURE.md`.

No source or smoke implementation change was justified. Moving the large runners or
splitting their programs would change the conditional compile, existing `--smoke`
compatibility path, target catalog, or desktop lifetime contracts without a new defect
or dependency boundary. `TestDocumentationIndex.ps1` passed with
`IndexedPaths=294`, `Routes=16`, `RootRedirects=102`; `Invoke-RefactorAudit.ps1 -Verify`
passed with `CSharpFiles=816`, `XamlFiles=60`, `PartialDeclarations=59`,
`PartialTextMatches=2`, `ViewModelUiIoFiles=1`, `ProjectCycles=0`, and
`ShellStorageCalls=0`; static project-reference/conditional-compile search and
`git diff --check` passed. Evidence:
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-burden-recheck-20260912\\smoke-boundary-recheck\\phase-summary.txt`.

Actual desktop screenshot execution, themes/layouts/DPI/monitor matrix,
camera/SDK/GPU/device paths, native long-run execution, and desktop shutdown remain
unverified.
## Current Shell composition navigation follow-up — 2026-09-11

Status: `PL-0014 VERIFIED` for source, build, and focused contract scope.

The current Shell composition owner remains
`src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.xaml.cs`. Its constructor
now has five named phase markers in the existing assignment order: runtime/shared
owners; XAML and visual presenters; Workspace and Recipe owners; command/session/
layer/test adapters; and event/release callbacks. This is a navigation aid only.
The `readonly` field ownership, XAML bindings, callbacks, event subscription order,
and `Dispose` order are unchanged. A helper-method extraction was rejected after
the compiler showed that it would move `readonly` assignments outside the
constructor; no wrapper or new abstraction was retained.

The first code-reading route is
`Program.Main -> OpenVisionLabApplication.Run -> OpenVisionShellHostWindow ->
OpenVisionShellHostView` constructor, then the five markers, followed by the
existing concrete owners under `Shell/Session`, `Shell/Layers`, `Shell/Workspace`,
`Shell/Tooling`, `Recipe`, `Shell/Documents`, and Pipeline Review. Mutable state,
Recipe commands, persistence, and shutdown ownership remain with those existing
owners. No new partial or duplicate owner was introduced.

Evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\pl0014-shell-composition-navigation-20260911`
(`phase-map.txt`, `constructor-token-contract.txt`, focused contract outputs, and
audit output). OpenVisionLab solution Debug and Release builds completed with zero
warnings and zero errors. The Shell Recipe lifecycle contract passed in both
configurations (`7/7`), `OpenVisionReadinessCheck` passed in both configurations,
`Invoke-RefactorAudit.ps1 -Verify` passed, `TestDocumentationIndex.ps1` passed,
and `git diff --check` returned exit code `0`.

The independent junior-readability review concludes that the initialization order
is now searchable without another abstraction layer, while the large constructor
and controller tracing remain a known navigation cost. Runtime WPF themes,
Wide/Compact layouts, DPI, monitor placement, keyboard/pointer behavior,
camera/SDK, native permanent hangs, and long-run shutdown remain unverified:
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Current Shell View member navigation follow-up — 2026-09-11

Status: `PL-0015 VERIFIED` for source, build, and focused contract scope.

The remaining Shell View burden was reviewed against the existing code-behind
boundary. The remaining methods are WPF control lifecycle, Recipe/Pipeline
navigation, dialog/file-picker adapters, localization/layout projection, or
test-compatibility forwarding. No new independent domain or persistence owner
was proven. The safe slice therefore structured the existing
`OpenVisionShellHostView.xaml.cs` with responsibility-oriented regions without
moving code or adding a type.

The reading order is now `Constants and Dependency Properties -> Fields ->
Constructors -> Public View Contract -> Lifetime`, followed by the named
`Interactions` regions for Recipe Property Grid, Recipe Manager Navigation,
Pipeline Review Navigation, Recipe Panel Drag, Runtime and Localization, Sample
and Layer Projection, Shell Log Presentation,
Localization Helpers, and Test Contract Helpers. The final compatibility surface
is explicitly grouped as `Surface and Test Hooks`, `Test Properties`, and
`Test Actions`. Existing smoke callers and public test method names remain
unchanged.

Evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\pl0015-shell-view-member-navigation-20260911`.
Region-only normalized token hashes are identical. Sequential solution Debug and
Release builds and VisionRecipeRunnerSmoke Debug and Release builds completed with
zero warnings and zero errors. The Shell Recipe lifecycle contract passed `7/7`
in both configurations; `OpenVisionReadinessCheck`, `Invoke-RefactorAudit.ps1
-Verify`, `TestDocumentationIndex.ps1`, and `git diff --check` passed.

The independent junior-readability review recommends accepting this as a safe P2
navigation improvement. The class and constructor remain large, and Controller/
Presenter/Document/Workspace tracing still follows the existing owner route.
Regions are a navigation aid, not a claim that the View became a small class;
another region-only expansion is not planned without a new independent boundary.
Full WPF theme/layout/DPI/monitor/input, camera/SDK/GPU, native-hang, and long-run
shutdown behavior remain unverified:
`소스 코드 및 focused contract 검토 완료 / 실제 Runtime UI·하드웨어 검증 필요`.

## Current Shell Recipe dialog boundary follow-up — 2026-09-11

Status: `PL-0016 VERIFIED` for source, build, and focused contract scope.

The Recipe confirmation, file/folder picker, and qualified-evidence folder
operations no longer live in the Shell View. They are owned by the concrete
`RecipeDialogAdapter` at
`src/OpenVisionLab/UI/Menu/Wpf/Shell/Recipe/RecipeDialogAdapter.cs`.
The adapter is created by `OpenVisionShellHostView`, passed as method-group
callbacks to `OpenVisionShellHostRecipeCommandSurface`, and returns the same
path/boolean results as before. The existing Shell test properties route through
`OpenVisionShellHostViewTestSurface` into the adapter's two test delegates.

The moved boundary owns only WPF modal/file/process side effects. Recipe state,
workflow policy, pending-edit queue, persistence, event subscriptions, and
resource release remain with the existing command surface, Shell test surface,
and Shell lifetime/session owners. At the PL-0016 checkpoint,
`DecidePendingRecipeEdit` remained in the View as a separate pending-edit/test
contract; the PL-0017 section below records its subsequent move into the same
adapter.
No interface, factory, manager, partial, or public callback rename was added.

Evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\pl0016-dialog-adapter-20260911`.
The dialog body token contract is hash-identical before/after, direct dialog I/O
is absent from the View, callbacks and test hooks route through the adapter,
Solution and VisionRecipeRunnerSmoke Debug/Release builds completed with zero
warnings/errors, Shell lifecycle passed `7/7` in both configurations,
ReadinessCheck passed in both configurations, RefactorAudit and DocumentationIndex
passed, and `git diff --check` returned exit code `0`.

The independent junior review accepts this as a real navigation and ownership
improvement. Actual dialog/file-picker interaction, external evidence-folder
launch, localization rendering, WPF theme/layout/DPI/monitor/input, camera/SDK/
GPU, native permanent hangs, and long-run shutdown remain unverified:
`소스 코드 및 focused contract 검토 완료 / 실제 Runtime UI·하드웨어 검증 필요`.

## Current Shell pending-edit dialog boundary follow-up — 2026-09-11

Status: `PL-0017 VERIFIED` for source, build, and focused contract scope.

The remaining pending Recipe edit decision no longer lives in the Shell View.
`RecipeDialogAdapter` now owns both the existing test decision queue and the
modal `OpenVisionRecipePendingEditDialog` decision. The call path is
`OpenVisionShellHostView -> RecipeDialogAdapter.DecidePendingEdit ->
OpenVisionShellHostRecipeCommandSurface transition -> adapter queue or modal
dialog -> decision`. The public Shell test method keeps its existing name and
routes through `OpenVisionShellHostViewTestSurface` into the adapter queue.

The adapter owns the pending-edit queue and WPF modal side effect. Mutable
Recipe state, persistence, and workflow policy remain with the existing Recipe
command surface. The Shell View and session owners retain the existing event,
timer, image, and shutdown lifetime responsibilities; the adapter owns no
disposable resource. `OpenVisionRecipePendingEditRequest/Decision`, callback
signatures, queue order, owner-window lookup, default decision, and modal
semantics were preserved. No interface, factory, manager, or partial was
added.

Shortest reading order: search `QueuePendingRecipeEditDecisionForTest`, read
the Shell compatibility forwarder, follow
`OpenVisionShellHostViewTestSurface`, then read
`RecipeDialogAdapter.QueuePendingRecipeEditDecisionForTest` and
`RecipeDialogAdapter.DecidePendingEdit`; follow the constructor method-group
registration to the existing Recipe command surface for the runtime call.

Evidence is at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\pl0017-pending-edit-dialog-20260911`.
The normalized pending-edit method token contract is hash-identical before and
after, the View no longer contains the queue or dialog construction, and the
adapter/test forwarding searches pass. Solution Debug/Release builds and the
focused app/smoke builds completed with zero warnings/errors; Shell lifecycle
and readiness checks passed in both configurations; RefactorAudit,
DocumentationIndex, and `git diff --check` passed.

The independent junior review accepts this as a real remaining Shell boundary
closure. Actual modal interaction, owner-window behavior under every WPF theme,
localization rendering, DPI/monitor/input, camera/SDK/GPU, native permanent
hangs, and long-run shutdown remain unverified:
`소스 코드 및 focused contract 검토 완료 / 실제 Runtime UI·하드웨어 검증 필요`.

## Current Application Exit-Code Follow-up — 2026-09-10

Status: VERIFIED for source/build/desktop contract.

`src/OpenVisionLab/App/Bootstrap/OpenVisionLabApplication.cs` now captures the
return value of `application.Run(shellWindow)` and returns it after the existing
shutdown log. `src/OpenVisionLab/Program.cs` already returns
`OpenVisionLabApplication.Run(args)`, so the call path is now
`Program.Main -> OpenVisionLabApplication.Run -> Application.Run -> Program.Main`
without changing startup loading, TCP smoke `Shutdown(exitCode)`, or dispose
ordering. The mutable exit-code owner remains WPF `Application`; the bootstrap
only transports the value.

The Debug and Release OpenVisionLab project builds completed with zero warnings
and zero errors. The source contract at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\application-exit-code-20260910`
confirmed capture, return, and Program forwarding. The desktop contract at
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\application-exit-code-20260910-rerun`
placed both test windows on the dynamically selected smaller left monitor
`\\\\.\\DISPLAY2`; normal `CloseMainWindow` returned `0`, and the intentional
invalid TCP smoke input returned `1` through `Application.Shutdown(1)`.

The P0 clean Release distribution gate (`R1`) passed in a temporary validation
branch assembled from the current dirty Dev source, including the no-`-SkipLaunch`
launch path. Debug/Release builds, 33 public sample rows, metadata and archive
checks, two launch cycles, immutable installation snapshot, data-root creation,
and legacy migration completed in 207.643 seconds. Validation commit
`4c13f145f7557b49693ee09f85da281ba3cdd6b0` was temporary and was removed with
the worktree. Evidence was copied to
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\release-candidate-clean-no-skip-20260910`;
the framework-dependent archive hash is
`CF94D370035D9C70661AE3B15BF746A27664320C48FC8C637E14B017BA83E8A9`.
The installed EXE desktop contract at the same evidence boundary selected the
smaller left `\\.\DISPLAY2`, placed both windows inside its working area, and
verified normal `0` and intentional failure `1` exit codes. No Dev commit, push,
tag, release, deployment, or Original mutation was performed.

## Current Pipeline Completion Follow-up — 2026-09-10

Status: VERIFIED for the existing execution contract; permanently blocked
native-call recovery remains outside this slice.

`VisionPipelineExecutionService.WaitForStepCompletionStatusAsync` remains the
owner of timeout/cancel status and worker drain. The existing call path is
`VisionPipelineExecutionService.Execute -> WaitForStepCompletionStatusAsync ->
VisionPipelineStepExecutionController`, with late result-image disposal after
the worker task drains. The focused
`--pipeline-review-execution-contract` smoke passed for timeout/cancel
distinction, late result and exception observation, plan-failure recovery,
generation invalidation, duplicate-run rejection, and async disposal. Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-completion-status-20260910-final\pipeline-review-execution-contract.txt`.

The mutable execution-status owner and native-result lifetime remain unchanged;
no new interface, worker, or process boundary was introduced. A native function
that never returns still requires a separately approved process-isolation design.

## Current Threshold Suggestion Follow-up — 2026-09-10

Status: VERIFIED for source/build/focused-contract scope and the targeted
`cvr07_threshold_suggestion` UI smoke; the full WPF runtime UI matrix remains
unverified.

`VisionToolThresholdSuggestionSession` now owns the mutable suggestion and Undo
state. The call path is
`ThresholdToolWpfView button event -> VisionToolThresholdSuggestionSession ->
VisionToolThresholdSuggestionAnalyzer`; a successful apply continues through
the existing `VisionToolThresholdInteractionController -> ThresholdToolPresenter
-> ThresholdToolViewModel` path. The View still owns WPF evidence display,
button/panel state, marker rendering, timers, and control event lifetime. XAML
event names, `CreateProperty`, and existing presenter/view-model contracts are
unchanged.

The focused `--threshold-suggestion-session-contract` smoke passed 7/7 checks,
including the View boundary, accepted analysis, apply snapshot, AlreadyCurrent,
stale evidence rejection, Clear-preserved Undo, Undo consumption, and non-Basic
guard. The existing `cvr07_threshold_suggestion` target initially failed in the
current checkout and a clean baseline worktree because it checked a panel inside
the collapsed `ThresholdSignalInspectorOverlay` without opening that overlay.
Other Threshold targets already use the explicit `OpenSignalInspectorForTest()`
path, so `tools/PipelineViewerScreenshotSmoke/Program.cs` now performs that
existing UI action and pumps four dispatcher cycles before the final visibility
assertion. The production View and assertion requirement were preserved. The
target then completed `OK` in 6.6 seconds and verified the overlay, suggestion
panel, Analyze/Use/Undo buttons, and signal plot; evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\threshold-ui-smoke-20260910-fixed`.
The existing `wpf_threshold_to_blob_detection_e2e` smoke had the same NG on
the current checkout and baseline HEAD because its assertion required an
explicit Preview even though the existing controller schedules a debounced
Preview for slider changes. The test now guarantees a changed slider value and
checks exactly one auto-preview without changing product behavior. The target,
`e2e`, `route`, `property-grid-auto-preview`, and `preprocess-auto-preview`
suites all completed `OK`; evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-e2e-20260910-fixed`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-route-20260910`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-property-grid-20260910`,
and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-preprocess-20260910`.
The changed smoke project Debug/Release builds both exited 0. Debug retained
the existing MSB3270 Any CPU/x64 reference warning and CS8600 nullable warning;
Release retained CS8600 at `Program.cs:10181`. These warnings remain outside
this slice.
The source/build conclusion is
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요` for the remaining
themes, DPI, monitor, keyboard/mouse matrix, and long-run WPF behavior. Initial
failure evidence remains at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\threshold-ui-smoke-20260910-fresh`
and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\threshold-ui-smoke-20260910-baseline`.

## Current Representative WPF Smoke Follow-up — 2026-09-10

Status: VERIFIED for the existing Debug quiet-offscreen representative smoke
coverage; the full WPF state/theme/DPI matrix remains unverified.

The default UI target group completed 22/22 `OK`, covering Shell preview,
workspace empty/open/image/output, tool input and save, native tool, Threshold,
Pipeline Review, Blob, Contour, Line, Matching, Feature Matching, pending tool,
ROI editor, Image Compare, Log Panel, and localization catalog. The existing
`perf` suite completed 4/4 `OK` for tool open, Pipeline Review entry, fast click,
and first heavy tool open. Evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-default-fixed-20260910`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-perf-fixed-20260910`

These runs use the current Korean UI and quiet offscreen rendering. They do not
prove alternate themes, Wide/Compact layout, 100/125/150/175/200% DPI, physical
pointer/keyboard states, multi-monitor desktop placement, camera/SDK behavior,
or long-run native lifetime. The remaining conclusion is
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Current DisplayManager Lifetime Follow-up — 2026-09-10

Status: VERIFIED for the reproduced cross-window resource-lifetime defect;
full WPF qualification remains unverified.

The initial same-process target sequence disposed the process-scoped
`DisplayManagerService.Default` from `OpenVisionShellHostView.ReleaseDisplayManager`.
The next `OpenVisionShellHostWindow` then reached
`OpenVisionShellHostLayerRefreshController.RefreshRows ->
DisplayManagerImageExtensions.GetLayerImage -> ImageSpaceService.GetImage` and
failed with `ObjectDisposedException`. The mutable resource owner is
`DisplayManagerService`; the process owner is `OpenVisionLabApplication`, while
an injected per-session manager remains owned by the supplied runtime context
and is still released by the View.

The View now skips disposal only for the shared default instance. The Bootstrap
disposes that instance after `Application.Run` returns and after the WPF
dispatcher shutdown path completes. `OpenVisionBitmapCanvasPresenter` and the
workspace preview controller also stop stale Dispatcher callbacks from touching
a disposed `ImageViewer`. No interface, wrapper, or new manager was added, and
the Window/XAML/public binding contract is unchanged.

Focused runtime evidence:

- Sequential `wpf_shell_preview`, `wpf_shell_host_window_chrome`, and
  `wpf_shell_host_window_maximized`: all `OK` at
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-preview-window-max-fixed-20260910`.
- Sequential `wpf_shell_host_workspace_empty`,
  `wpf_shell_host_learn_entry`, and `wpf_shell_host_tool_search`: all `OK` at
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-lifetime-race-fixed-20260910`.
- Representative default target group: 22/22 `OK` at
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-default-fixed-20260910`.
- Performance suite: 4/4 `OK` at
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-perf-fixed-20260910`.

The bounded `--all` run reached existing sample/learn targets and then stopped
when a later UI path required user selection. It no longer reproduced the
disposed `ImageSpaceService` failure before that stop. The stale
`OpenVisionBitmapCanvasPresenter.RefreshCanvas` path was then rechecked in a
focused sequence and passed; the formerly separate sample fixture/learn-pair
contract failures are recorded and verified in the R7 section below. The
remaining full UI boundary is
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Current Public Sample Learn/Pair Contract Follow-up — 2026-09-10

Status: VERIFIED for the bounded public sample picker and pair-coverage
contracts; the full WPF qualification matrix remains unverified.

The reproduced Geometry Learn-path defect was in
`OpenVisionWorkspaceLearnDocumentService.ResolveDocumentFileName`. The
`Public_Fixture_Normalize_RelativeRoi_Good` pipeline contains both
`RotateScale` and `Threshold`, so sample-text precedence returned
`LEARN_THRESHOLD.md` even when the operator selected the `geometry` Learn path.
The mutable selection owner remains
`OpenVisionWorkspaceSamplePickerViewModel.SelectedLearnPathOption`; the call
path is `SamplePickerViewModel -> OpenVisionWorkspaceLearnDocumentService ->
ResolveDocumentFileName`. The service now gives the explicit Geometry leaf
path precedence while preserving product-sample and other tool-specific
resolution behavior. No XAML, binding, catalog, pipeline, or public sample
asset contract changed.

The Fixture picker smoke had treated the complete search result as one pair,
although the same `Fixture` search legitimately exposes both the translation
Fixture pair and the NormalizeImage relative-ROI pair. The contract now
checks the selected `Public_Fixture_Pad` PairGroup contains exactly one
runnable Good and one runnable Bad row and that both rows are rendered. The
pair-coverage smoke now validates only complete Good/Bad groups, so the
Good-only `Public_AffineTransform` benchmark remains a reference sample rather
than a false incomplete pair. Good/Bad metric comparison accepts the existing
`DistanceMm`/`DistancePx` metric families across `Avg`, `Range`, and `Max`
suffixes, matching the Line learn contract; exact names remain required for
other metric families.

The changed owners and shortest reading order are:

```text
OpenVisionWorkspaceSamplePickerViewModel.SelectedLearnPathOption
  -> OpenVisionWorkspaceLearnDocumentService.ResolveDocumentFileName
PipelineViewerScreenshotSmoke.Program
  -> CaptureShellHostWorkspaceSampleFixturePicker
  -> CaptureShellHostWorkspaceSamplePairCoverage
  -> ValidateSamplePairGroup
```

The focused Debug UI smoke completed all three targets as `OK`:
`wpf_shell_host_workspace_sample_learn_paths`,
`wpf_shell_host_workspace_sample_fixture_picker`, and
`wpf_shell_host_workspace_sample_pair_coverage`. Evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-sample-contracts-final-20260910`.
The same three targets completed `OK` in the Release smoke run at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-sample-contracts-release-20260910`.
Solution Debug/Release builds completed with zero warnings and zero errors;
the changed smoke project Debug/Release builds completed with zero errors and
one pre-existing nullable warning at `Program.cs:10181`. Readiness,
`Invoke-RefactorAudit.ps1 -Verify`, `TestDocumentationIndex.ps1`, and
`git diff --check` also passed. The full WPF theme/layout/DPI/monitor,
physical input, camera/SDK, and long-run boundaries remain
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Current Sequential Sample-Review Async Follow-up — 2026-09-10

Status: VERIFIED for the reproduced sequential sample-review deadlock and the
bounded Debug smoke regression; the full WPF qualification matrix and native
long-run behavior remain unverified.

The reproduced failure occurred when the existing synchronous UI capture path
called `VisionPipelineSampleCheckService.RunSampleCheckSafe`, which waits for
`VisionRecipeRunner.RunAsync` while the UI Dispatcher is blocked. A fast first
sample could complete inline, but a second sample whose first step yielded
captured that Dispatcher and remained pending. The current and intended owner
are both the existing pipeline execution owners:
`VisionRecipeRunner` prepares the recipe run and
`VisionPipelineExecutionService` owns step execution, timeout state, and late
result draining. No new abstraction or worker boundary was introduced.

The verified call path is:

```text
CaptureShellHostWorkspaceSampleOpen
  -> shell disposal
  -> CaptureShellHostWorkspaceSamplePipelineReviewMetrics/NgMetrics
  -> VisionPipelineSampleCheckService.RunSampleCheckSafe
  -> VisionRecipeRunner.RunAsync
  -> VisionPipelineExecutionService.RunPreparedAsync
  -> RunPreparedStepsAsync
  -> WaitForStepCompletionStatusAsync
```

The mutable run state remains owned by `VisionPipelineRunResult` and its
`VisionPipelineContext`; the existing sample-review binding and persisted
report contracts are unchanged. The execution methods now use
`ConfigureAwait(false)` at the prepared-run, step-completion, and late tool
result boundaries, so continuations do not require the blocked Dispatcher.
The shortest reading order is `VisionPipelineSampleCheckService.cs` →
`VisionRecipeRunner.cs` → `VisionPipelineExecutionService.cs` → the two
`PipelineViewerScreenshotSmoke.Program` capture methods.

The sequential Debug smoke
`wpf_shell_host_workspace_sample_open,wpf_shell_host_workspace_sample_pipeline_review_metrics`
completed both targets `OK` at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r8-sample-open-metrics-20260910`.
The corresponding NG-metrics sequence also completed both targets `OK` at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r8-sample-open-ng-metrics-20260910`.
Solution Debug and Release builds completed with zero warnings and zero
errors. `VisionRecipeRunnerSmoke` Debug and Release pipeline-review execution
contracts also passed at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-completion-status-r8-20260910`
and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-completion-status-r8-release-20260910`.
The focused UI evidence uses the existing quiet offscreen path; it
does not verify every theme, DPI, monitor, physical input, camera/SDK, or
long-run native state. The remaining boundary is
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Current Pipeline Review Smoke Ownership Follow-up — 2026-09-10

Status: VERIFIED for the bounded Pipeline Review smoke ownership and Details
visibility contracts; the full WPF qualification matrix remains unverified.

The reproduced failures were in smoke assumptions. The current production
owner opens Pipeline Review through
`OpenVisionShellHostToolWindowController.ShowSelectedTool` and
`ShowDockedDocumentWorkspace`, so the review content is a docked document root,
not an `OpenVisionFloatingToolWindow`. The Details region also starts collapsed
under the existing `btnReviewDetailsToggle`. The test now uses the existing
`GetActiveToolVisualRoot` path and explicitly opens that Details region before
reading its tabs and diagnostic AutomationIds. For the Feature `ScoreMax`
case, a missing localization key now falls back to the existing
`VisionPipelineKnownMetrics` display name; no production binding or metric
contract changed.

The current and intended test owner is the existing
`tools/PipelineViewerScreenshotSmoke/Program.cs` target harness. The mutable
Pipeline Review state remains owned by the existing document/ViewModel; the
smoke change adds no wrapper, service, or UI abstraction. The verified call
path is:

```text
PipelineViewerScreenshotSmoke target
  -> OpenWorkspaceSamplePipelineReviewForSmoke
  -> WorkspaceCommandSurface.OpenPipelineReview
  -> OpenVisionShellHostToolWindowController.ShowSelectedTool
  -> ShowDockedDocumentWorkspace
  -> GetActiveToolVisualRoot / btnReviewDetailsToggle
```

The shortest reading order is
`OpenVisionShellHostWorkspaceCommandSurface.cs` →
`OpenVisionShellHostToolWindowController.cs` →
`PipelineViewerScreenshotSmoke.Program` target and helper methods. Existing
Pipeline Review document, XAML, binding, and public test-hook contracts are
unchanged.

The focused Debug targets all completed `OK`: Fixture review, Normalize Fixture
review, Fixture teach, CVR-06 matcher, Edge NG diagnostics, and Feature/Line/
Blob/BentPin/Film NG metrics. Evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-fixture-review-20260910-rerun`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-normalize-fixture-review-20260910`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-fixture-teach-20260910`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-cvr06-20260910`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-edge-ng-20260910-rerun`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-feature-ng-20260910-rerun`,
and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-ng-metrics-final-20260910`.
The changed smoke project Debug build exited 0 with two pre-existing warnings;
Release exited 0 with one pre-existing nullable warning. The remaining
runtime boundary is
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Current Shell/Pipeline Review Navigation Follow-up — 2026-09-10

Status: VERIFIED for the focused source-path naming slice, Debug/Release build,
and representative Pipeline Review smoke; existing Pipeline Review behavior
and owner contracts remain the authority.

The junior-navigation concern was concrete: the Pipeline branch of
`OpenVisionShellHostToolWindowController.ShowSelectedTool` contained document
restore/create, timing, docking, and completion work inline. A developer had to
read the whole method before seeing that Pipeline Review enters the docked
document workspace. The existing owners were already correct, so the smallest
change is a named private call path, not a new service or composition layer.

The changed path is:

```text
ShowSelectedTool(Pipeline)
  -> ShowPipelineReview
  -> TryRestorePipelineReview / CreatePipelineReviewDocument
  -> ShowDockedDocumentWorkspace
  -> OpenVisionPipelineReviewDocument.View
```

`OpenVisionPipelineReviewView.btnReviewDetailsToggle` remains the owner of the
collapsed/expanded Details presentation state. Execution generation,
cancellation, stale callback rejection, and image lifetime remain with the
existing Pipeline Review document/execution owners. Shared default
`DisplayManager` shutdown remains owned by `OpenVisionLabApplication`; this
slice does not change Dispose order.

The durable owner map and shortest reading order are recorded in
`docs/reports/OPENVISIONLAB_JUNIOR_NAVIGATION_AND_SHELL_REVIEW_20260910.md` and
the Shell section of `docs/admin/CODEBASE_STRUCTURE.md`. OpenVisionLab Debug and
Release builds completed with zero warnings and zero errors. The existing
`wpf_shell_host_pipeline_review`, `wpf_shell_host_pipeline_review_ng`, and
`wpf_shell_host_workspace_sample_pipeline_review_metrics` targets completed
`OK`; evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r10-pipeline-review-20260910`.
`TestDocumentationIndex.ps1` and `Invoke-RefactorAudit.ps1 -Verify` also passed.
Alternate theme/layout/DPI/monitor, physical keyboard/pointer, camera/SDK,
permanent native hang, and long-run runtime remain
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Current Shell MVVM State Ownership Follow-up — 2026-09-10

Status: VERIFIED for the bounded View code-behind state/I/O ownership move,
Debug/Release app builds, readiness, and focused Workspace/Recipe smoke.

`OpenVisionShellHostView.xaml.cs (Interactions region)` contained two concrete persistence
paths in addition to WPF event wiring: `SwitchRuntimeRecipe` changed
`GlobalState.Recipe/System` and called `SaveConfig`, while
`RememberWorkspaceImagePath` validated and saved the last workspace image path.
The existing owners are now explicit in source:

```text
RecipeCommandSurface
  -> OpenVisionShellHostRecipeController.SwitchRuntimeRecipe
  -> GlobalState.Recipe/System.SaveConfig

Workspace/Layer command
  -> OpenVisionShellHostWorkspaceImageController.RememberWorkspaceImagePath
  -> System.LastWorkspaceImagePath/System.SaveConfig
```

The View still provides the same callbacks and keeps dialog/window/control
lifecycle code. No XAML binding, public command name, SaveConfig condition, or
image-load behavior was changed. The owner/call-path record is
`docs/reports/OPENVISIONLAB_MVVM_SHELL_STATE_OWNERSHIP_20260910.md` and the
source ownership checks are in `tools/OpenVisionReadinessCheck/Program.cs`.
Focused Debug/Release app builds, readiness, and workspace/Recipe smoke are
complete. `TestDocumentationIndex.ps1` reports `IndexedPaths=281`,
`Invoke-RefactorAudit.ps1 -Verify` reports
`CSharpFiles=847|XamlFiles=60|PartialDeclarations=110|ProjectCycles=0|ShellStorageCalls=0`,
and `git diff --check` is clean. Evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r11-shell-state-20260910`.
The source/runtime boundary for other Views remains a separate audit; full
WPF theme/layout/DPI/monitor/keyboard and long-run native behavior are still
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Current Shell Readiness MVVM Follow-up — 2026-09-10

Status: VERIFIED for the bounded readiness-policy owner move, Debug/Release app
build, source readiness contract, and focused Workspace/Recipe/tool-search smoke.

`OpenVisionShellHostView.xaml.cs (Interactions region)`의 `RefreshToolReadiness`가 Arithmetic
설정 로드, `VisionPipelineArithmeticStep.RequiresInputLayerB` 계산, Main/보조
Layer 이미지 집계를 직접 수행하던 경계를 확인했다. 이 정책은 기존
`OpenVisionShellPreviewViewModel`에 `RefreshToolReadiness(IDisplayManager,
VisionToolRepository)`로 이동했고, View는 기존 Recipe/Layer/Native settings
변경 이벤트에서 해당 owner를 호출한다.

```text
Recipe/Layer/Native settings event
  -> OpenVisionShellHostView.Interactions.RefreshToolReadiness
  -> OpenVisionShellPreviewViewModel.RefreshToolReadiness
  -> SetToolReadiness -> ApplyToolReadiness
```

기존 `SetToolReadiness` 상태, Arithmetic Input B 안내, Layer readiness,
Preview/Run 명시 동작, XAML binding은 유지했다. 새 service/interface/factory/
manager/wrapper/partial은 추가하지 않았다. OpenVisionLab Debug/Release
build는 경고 0·오류 0, `OpenVisionReadinessCheck`는 PASS, 기존
`wpf_shell_host_workspace_image_load`, `wpf_shell_host_recipe_context_switch`,
`wpf_shell_host_tool_search`는 모두 `OK`였다. Smoke project Debug build는
오류 0, 기존 CS8600 warning 1개였다. `TestDocumentationIndex.ps1`는
`IndexedPaths=282`, `Routes=16`, `RootRedirects=102`로 PASS했고,
`Invoke-RefactorAudit.ps1 -Verify`는
`CSharpFiles=847|XamlFiles=60|PartialDeclarations=110|ProjectCycles=0|ShellStorageCalls=0`로
PASS했다. 변경 기록은
`docs/reports/OPENVISIONLAB_MVVM_SHELL_READINESS_20260910.md`이며 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r12-tool-readiness-20260910`에 있다.
`TestDocumentationIndex.ps1`, `Invoke-RefactorAudit.ps1 -Verify`, 전체 WPF
theme/layout/DPI/monitor/keyboard와 long-run native behavior는 별도 경계로
남아 있다.

## Current Shell Recipe Save Callback Follow-up — 2026-09-10

Status: VERIFIED for the bounded Recipe persistence callback owner move,
Debug/Release app build, readiness contract, and Recipe context/change-safety
smoke.

Shell composition이 `() => runtimeContext.Global.Recipe.SaveTools()`를 직접
Recipe command surface에 전달하던 경계를 확인했다. 기존
`OpenVisionShellHostRecipeController`에 `SaveRuntimeRecipeTools`를 추가하고
동일한 `Func<bool>` callback을 그 메서드로 연결했다.

```text
Recipe command surface
  -> OpenVisionShellHostRecipeController.SaveRuntimeRecipeTools
  -> runtimeContext.Global.Recipe.SaveTools
  -> RecipeRuntimeStorage.Save
```

Recipe XML/rollback/save result, command binding, View constructor의 다른
composition 순서는 변경하지 않았다. OpenVisionLab Debug/Release build는
경고 0·오류 0, `OpenVisionReadinessCheck`는 PASS,
`wpf_shell_host_recipe_context_switch`와 `wpf_shell_host_recipe_change_safety`
는 모두 `OK`였다. `TestDocumentationIndex.ps1`와
`Invoke-RefactorAudit.ps1 -Verify`도 PASS였다. 변경 기록은
`docs/reports/OPENVISIONLAB_MVVM_SHELL_RECIPE_SAVE_20260910.md`이며 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r13-recipe-save-owner-20260910`에 있다.
전체 WPF theme/layout/DPI/monitor/keyboard와 long-run native behavior는
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`로 남아 있다.

## Current Shell/Pipeline Review Code-Behind Boundary Audit — 2026-09-10

Status: VERIFIED as a source-level boundary audit. No additional code split was
justified after R10–R13.

`OpenVisionShellHostView`와 `OpenVisionPipelineReviewView`의 production View
직접 호출을 다시 분류했다. 남은 코드는 WPF dialog/file picker,
MessageBox confirmation, external evidence folder open, control layout,
event lifetime, display-only Bitmap snapshot/disposal, hit-test/localization,
그리고 기존 Document/Controller callback forwarding이다. 다음 업무·저장
신호는 production View에서 발견되지 않았다.

- `VisionPipelineStorage` / `VisionPipelineExecutionService`
- `Recipe.SaveTools` / `System.SaveConfig`
- `OpenVisionNativeToolSettingsStore.Load` / `RequiresInputLayerB`

기존 manual partial은 XAML/event/test-hook와 responsibility별 command/document
파일로 분류되며, R14에서는 새 partial·wrapper·rename을 추가하지 않았다.
증거 요약과 signal 파일은
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-r14-codebehind-audit-20260910`에
있고 상세 기록은
`docs/reports/OPENVISIONLAB_SHELL_PIPELINE_CODEBEHIND_BOUNDARY_AUDIT_20260910.md`에
있다. R13 Debug/Release build, readiness, Recipe smoke, documentation index,
refactor audit, `git diff --check`를 유지 확인했다. 전체 WPF theme/layout/DPI/
monitor/keyboard와 long-run native behavior는
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`로 남아 있다.

## Current Common Folder Reorganization — 2026-09-10

Status: VERIFIED for the physical source-folder move and Debug/Release/source
contracts.

`src/OpenVisionLab/Common`의 23개 파일을 책임별 physical folder로 정리했다.
`Runtime`, `Imaging`, `PropertyGrid`, `Recipe`, `Persistence`, `Results`,
`Events`, `MessageDialogs`, `Account` 폴더에는 이미 호출 경로와 역할이
분명한 타입만 이동했고, `AppCommon`, `CCommon`, `DEFINE`는 여러 기능이
공유하는 legacy surface라 Common 루트에 남겼다. namespace, public type name,
Recipe/XML, XAML, SDK contract는 바꾸지 않았다.

주요 owner 경로는 `Common/Runtime/AppPathService.cs`,
`Common/Imaging/BitmapImageConverter.cs`,
`Common/PropertyGrid/PropertyGridToolPolicy.cs`,
`Common/Persistence/SerializeHelper.cs`다. SDK default compile glob을 유지해
project reference와 build item은 추가하지 않았고, readiness source contract의
AppPath 경로만 새 physical path로 갱신했다.

전후 목록과 path scan은
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-r15-common-folder-audit-20260910`에
있으며 상세 기록은
`docs/reports/OPENVISIONLAB_COMMON_FOLDER_REORGANIZATION_20260910.md`다.
OpenVisionLab Debug/Release build는 각각 경고 0·오류 0, Readiness
Debug/Release, documentation index, refactor audit, `git diff --check`도
통과했다. R15 완료 후에도 전체 WPF theme/layout/DPI/monitor/keyboard와 long-run native
behavior는 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`로 남긴다.

## Current Runtime Path Boundary Follow-up — 2026-09-10

Status: Complete for the bounded `AppPathService` runtime path containment
correction.

기존 `src/OpenVisionLab/Common/Runtime/AppPathService.cs`를 런타임 경로의 단일
소유자로 유지하면서 `Combine`, `CombineInstallation`,
`EnsureDirectory`, `GetCaptureFilePath`, `GetTestConfigPath`가 공통
`CombineUnderRoot` 경계를 사용하도록 보정했다. `.`/`..` 세그먼트와
canonical root escape는 `InvalidOperationException`으로 실패하고, 정상
하위 경로·데이터 루트 초기화/마이그레이션·설치 fallback 계약은 유지된다.
`ResolveExistingDataOrInstallationPath`의 package-relative `..` fallback과
외부 legacy public property 선언은 별도 계약이므로 변경하지 않았다.

새 smoke 명령 `--app-path-boundary-contract`의 Debug/Release가 각각
8/8 통과했고, solution Debug/Release, Readiness, 정량 refactor audit도
최종 소스 기준으로 통과했다. 상세 owner/call path/보존 계약은
`docs/reports/OPENVISIONLAB_APPPATH_BOUNDARY_REVIEW_20260910.md`에 있고,
실행 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\apppath-boundary-contract-20260910`에
있다.

WPF metadata parameter portable 경계는 소스 기준으로 재확인했으나 실제
host 분리 요구나 재현 결함이 없어 추가 추출하지 않는다. 실제 WPF
theme/layout/DPI, camera/SDK, 장시간 native resource 운전은 환경 기반
후속 검증이며, 이 handoff에서 완료로 표시하지 않는다.

## Current Reliability Follow-up — 2026-09-10

Status: Complete for the bounded BackgroundLoopWorker error-observability follow-up.

`src/OpenVisionLab/Common/Runtime/BackgroundLoopWorker.cs`의 public 계약과
Start/Stop/Dispose/cancellation 동작은 유지했다. 기존 빈 exception catch는
worker 이름과 전체 예외를 `OVLog` System/Error로 기록하도록 수정했다. 내부
caller가 없는 public type은 외부 binary/reflection 계약을 확인하기 전 삭제하지
않는다. source contract와 D: console runtime smoke에서 의도한 worker failure와
`BackgroundLoopWorker 'background-loop-contract' failed` 로그를 확인했고,
solution·Runner Debug/Release build와 Readiness도 통과했다.

증거: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\background-loop-worker-20260910`.
`AppPathService` 경계 보정은 위의 2026-09-10 완료 기록으로 닫혔다. WPF
metadata parameter의 portable 경계는 host 분리 요구나 재현 결함이 없는 현재
상태에서 추가 구조 변경을 하지 않는다.

## Current Repository Root Layout Follow-up — 2026-09-10

Status: Complete for the bounded root-visibility cleanup.

루트 폴더를 실제 참조와 junction target 기준으로 분류했다. `src`, `tools`,
`docs`, `scripts`, `dll`, `third_party`와 저장소 제어 폴더는 책임 소유자이므로
이동하지 않았다. `bin`, `obj`, `artifacts`, `dist`, `tmp`, `.vs`, `.codex`,
`.codex-temp`, `Sample`은 모두 저장소 밖 로컬/생성 데이터 junction임을
확인했고, 기존 논리 경로를 유지한 채 NTFS `Hidden` 속성만 적용했다. 따라서
빌드·스모크·Recipe/SDK 경로 계약은 바뀌지 않는다.

`tools/Move-OpenVisionLabLocalData.ps1`가 새 junction 생성과 복원 때도 같은
표시 정책을 유지하도록 보강했고, 이미 외부 D: root 아래의 별도 custom target을
가리키는 `TwoDIntegrationTcpSmoke` mount는 정상적인 기존 external mount로
인식하도록 했다. 상세 owner/call path와 root reading order는
`docs/admin/OPENVISIONLAB_ROOT_LAYOUT_20260910.md`에 있다.

증거: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\root-organization-20260910`.
`Move-OpenVisionLabLocalData.ps1 -WhatIf`는 `LocalDataMove=PASS`로 76개 후보를
확인했고, root mount 9개의 Junction target과 Hidden 상태를 재확인했다.
실제 Explorer 화면은 사용자별 Hidden items 설정에 좌우되므로 이 작업은 NTFS
속성과 경로 보존까지를 검증하며, target 자체의 별도 D: 데이터 migration은
수행하지 않았다.

## Current Folder Organization Follow-up — 2026-09-10

Status: Complete for the bounded `OpenVisionLab.Docking.Controls` physical
folder reorganization; versioned as `2.2.0-dev.2` with the current refactor
change set.

The full source inventory found 27 projects, 844 C# files, 60 XAML files, 110
partial declarations, 34 project references, and no project cycle. The
`OpenVisionLab.Docking.Controls` root contained 49 C# files across contracts,
document state, workspace lifecycle, guides, layer docking, converters, and
Views. Those files now live under `Contracts`, `Models`, `Documents`,
`Workspace`, `Guides`, `LayerDocking`, `Converters`, and `Views`. Namespaces,
public contracts, XAML bindings, and the AvalonDock package boundary are
unchanged; the workspace resource pack URI is the only path reference updated.

The two standalone smoke roots and `src/OpenVisionLab/Common` remain classified
follow-up candidates. Their callers and historical path contracts require a
separate dependency proof before any physical move. Details, owner map, and
before/after inventory are in
`docs/reports/OPENVISIONLAB_FOLDER_ORGANIZATION_AUDIT_20260910.md`.

Static audit/inventory, solution Debug/Release, readiness Debug/Release, and
focused ImageCompare/namespace contracts all pass for this change. The external
ImageCompare link project now explicitly includes the moved directory-policy
source. Desktop theme/DPI and long-run docking/native lifetime remain
unverified; this is recorded as `소스 코드 기준 검토 완료 / 실제 Runtime UI
검증 필요`.

## Current MVVM/Partial/Module Review — 2026-09-10

Status: Complete for the bounded whole-project source audit and two concrete
external-I/O ownership corrections.

최종 정적 감사는 C# 844개/277,236줄, XAML 60개/24,646줄, raw partial 110개,
ViewModel direct WPF UI/Dialog/`Process.Start` 0개, ViewModel direct IO signal
6개, project 27개/ProjectReference 34개/순환 0개로 통과했다. 1,000줄 이상
파일 28개와 3,000줄 이상 파일 8개는 investigation signal로 기록했으며,
파일 크기만으로 완료 owner를 다시 나누지 않았다.

`ImageCompareWindow.xaml.cs`의 최근 디렉터리 메모리·설정 저장/복원과
`LogPanelViewModel`의 최신 로그 파일/폴더 셸 접근을 각각 concrete
`ImageCompareDirectoryPolicy`와 `LogPanelFileAccess`로 이동했다. Window는
파일 선택·pointer/window lifecycle, LogPanel ViewModel은 binding/filter/timer
상태만 소유한다. 기존 ImageCompare resource owner와 LogPanel command/binding은
재사용했다. 새 interface/manager/factory chain은 추가하지 않았다.

partial 110개는 XAML/generated 56, settings 1, docking 19, ImageCanvas 3,
OpenGL 9, Shell 10, Pipeline Review 2, 기타 WPF composition 8, test source
string 2로 분류했다. 생성 코드와 cohesive composition은 유지했고 새 partial은
추가하지 않았다. 상세 call path와 유지/미검증 경계는
`docs/reports/OPENVISIONLAB_FULL_REFACTOR_REVIEW_20260910.md`와
`docs/admin/CODEBASE_STRUCTURE.md` sections 9.46–9.47에 있다.

증거: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\image-compare-directory-policy-20260910`.
solution Debug/Release, Logging.Controls Debug, Image Compare directory/resource
contracts, `log_panel_contract_check`, `logging_buffer_contract`, 대표
`wpf_image_compare` smoke와 final refactor audit를 실행했다. 모든 supported
theme/DPI/long-run field runtime은 아직 검증하지 않았으므로
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요` 경계를 유지한다.

`AppPathService` 경계 보정은 `OPENVISIONLAB_APPPATH_BOUNDARY_REVIEW_20260910.md`에서
완료했다. WPF metadata parameter portable 경계는 host 분리 요구 또는 재현
결함이 생길 때만 다시 연다. Recommended model: `gpt-5.6-terra` | Reasoning
effort: `high`.

## Current Reliability Review — 2026-09-09

Status: Complete (current-source survey, four reliability boundaries, and the residual Shell validation/Step Edit audit).

현재 사용자 요청에 따라 전체 project/source inventory와 주요 Image -> Layer ->
Tool -> Inspection -> Pipeline -> Recipe -> Result -> Review 호출 경로를 조사했다.
입력은 branch `codex/public-sample-ux-docs`, HEAD
`65e2fd8b77989172d68d0aad81d489df94baf474`, `net8.0-windows7.0`, 기존 dirty 314개다.
기존 OVL-01~53 owner는 보존했다.

- Recipe 저장 실패가 성공으로 전달되는 P0를 실제 파일 잠금으로 재현했다.
  `RecipeRuntimeStorage.Save`와 `RecipeState.SaveTools`가 기존 bool을 전달하고
  Tool 실패 시 Data 저장을 중단한다. Debug/Release 새 계약 각 8/8 통과.
- Pipeline callback 예외 후 반환되지 못한 Mat 결과의 해제 누락을 재현했다.
  기존 `VisionPipelineExecutionService`가 반환 전 결과 수명을 소유하고 Step
  입력은 using으로 해제한다. Debug/Release 새 계약 각 7/7 통과.
- Recipe load 중후반 XML 오류를 재현했다. Tool set은 임시 repository에서
  성공한 경우에만 교체하고, 실패 시 이전 Recipe identity/Data/collection/event를
  보존하며 실패한 임시 Matching 계열 템플릿 Mat을 정리한다. Debug/Release
  `recipe-load-recovery` 각 8/8 통과.
- 후반 Tool XML 교체 실패를 재현했다. `VisionToolStorage`가 기존 VISION 파일
  snapshot/rollback을 수행해 앞서 저장된 XML을 원래 bytes로 복구한다.
  Debug/Release `recipe-multi-file-save-recovery` 각 7/7 통과.
- Shell validation document/selection/evidence 및 Step preview/load/apply/session
  owner를 다시 감사했다. 기존 owner를 재분리하지 않았고, Recipe switch callback
  실패가 성공 메시지로 보이지 않도록 Shell guard만 추가했다. 상세:
  `docs/reports/OPENVISIONLAB_SHELL_VALIDATION_STEP_EDIT_AUDIT_20260909.md`.
- 기존 Recipe 경로, Step Edit apply/restore, Review execution/stale callback,
  ImageSpace snapshot 계약을 두 configuration에서 통과했다. 앱/runner 최종
  x64 Debug/Release build는 경고 0/오류 0, Readiness도 통과했다.

상세 수정 이유·전후 동작·소유권·위험·실행 증거와 기능 상태 분류는
`docs/reports/OPENVISIONLAB_ARCHITECTURE_RELIABILITY_REVIEW_20260909.md`에 있다.
코드 읽기 경로는 기존 README Start Here -> `CODEBASE_STRUCTURE.md` 1.1이다.
증거는 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\architecture-reliability-20260909`에 있다.
실제 desktop EXE/UI theme/DPI/장시간 운전은 이번에 실행하지 않았다.
최종 구조 감사는 C# 841개/partial 110개/참조 순환 0으로 통과했고, 문서 색인
274 paths/14 routes/102 redirects, `git diff --check`, 기존 tracked 변경의
대상 밖 보존 비교도 통과했다. 이 완료 범위는 전체 리팩토링 프로그램의 종료를
뜻하지 않으며 아래 두 기존 수정은 새 결함 없이 재분리하지 않는다.

다음 프로젝트 우선순위는 실제 제공되는 DPI/운영 이미지/운전 시간 조건에서의
WPF UI와 장시간 native resource 검증이다. 이 환경이 없으면 실행을 완료로
표시하지 않는다. 그 다음은 `AppPathService`/기본 runtime static 및 WPF
metadata parameter의 portable 경계 재평가이며, 실제 host 분리 요구 또는
재현 결함이 있을 때만 진행한다. 두 항목 모두
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.
후속 항목의 PL-0013 등록은 기존 PL-0010 schema 오류로 거부되었으며 보고서의
TODO 표에만 등록했다. 기존 이슈 원장, 자동화, Original, commit/push는 변경하지 않았다.

아래의 이전 slice 완료 기록은 유지하며 현재 우선순위는 이 절을 먼저 따른다.

## Current Refactoring Program Audit — 2026-09-08

Status: Complete (current-code survey, priority order, junior-readability
assessment, completion gates, and terminating automation contract).

The current Dev worktree contains 789 C# files / 270,361 lines, 59 XAML files /
24,633 lines, 108 partial declarations, 27 projects and 34 project references
with no detected project cycle. The Shell CommandSurface spans 10 files /
10,146 lines. The largest files are concentrated in smoke tools and shared
composition points, so file length remains an investigation signal rather than
an extraction rule.

The code is **partially modular for a junior developer**: Recipe/Pipeline
owners, presenters, and topic-specific Learn modules are navigable, while the
Shell root/Handlers and Learn Window remain large shared composition points.
Learn Window lifetime and topic presentation decisions are explicit owners. The
representative physical WPF runtime slice is complete for the current two-monitor
96% DPI workstation in both Debug and Release; alternate DPI/theme and topology
rows remain environment-bound.
`RoiImageCanvasViewModel.Commands.cs` no longer owns Directory traversal or
shared last-directory state; that policy now has a named ImageCanvas owner.
ContextMenu, OpenFileDialog, SaveFileDialog, WinForms keyboard, WPF
PreviewKeyDown/KeyUp, and ImageCanvas mouse policy now belong to explicit
owners. Other ViewModel IO signals remain in the quantitative audit. Existing
OVL-01/02/03/04/05/06a/07/08/10 and completed Learn/runtime owners remain closed
unless new evidence reopens them.

The fresh module-audit recheck ordered the work as: baseline and closed-owner
protection, Recipe execution owners, residual Shell CommandSurface
validation/evidence and step edit flows, Shell XAML vertical slices, generic
PropertyGrid adapter internals, compatibility-safe namespace/project
boundaries, and only then responsibility-based smoke-runner cleanup.
ImageCompare and all four Recipe execution paths are now closed by OVL-12
through OVL-18. This supersedes the former alternate-DPI-only next
implementation pointer; alternate DPI/theme/topology remains an
environment-bound validation item unless a new runtime failure appears.
The detailed table, junior assessment, and no-duplicate completion gates are in
`docs/reports/OPENVISIONLAB_CODEBASE_MODULE_AUDIT_20260908.md`.

Completion means every P0–P3 item has an explicit owner/call path, focused
Debug/Release proof, preserved Recipe/XML and Preview/Run contracts, required
runtime evidence for changed UI, no duplicate owner or scope TODO, and updated
Handoff/index records. At one independently verifiable slice per authorized
execution, the remaining structural roadmap is expected to take several more
executions; the gates, not the clock, determine completion. P0 and the OVL-11
Mat/save-dialog/ContextMenu/WinForms/WPF keyboard/mouse owners remain closed.
Any future automation must stop itself on completion or an external blocker.
Selective Dev commits and pushes are authorized for completed slices. Original
promotion is a separate versioned change set and remains blocked until its
source commit, version progression, and changed-file allowlist are explicit.
Product version `2.1.0`, tags, release publication, and deployment remain
unchanged.

This paragraph belongs to the historical 2026-09-08 audit slice. Its former
automation names are not current state. The active automation and its stop
conditions are recorded only in the `Current Prioritized Refactor Schedule —
2026-09-10` section at the top of this handoff.

## Current Slice — OVL-53 RefreshOptions composite command-state projection

Status: Complete for one independently verifiable Shell composite refresh
boundary; the broader Shell validation and Step Edit refactoring program remains
active.

OVL-52 이후 호출 경로를 다시 추적한 결과,
`RefreshOptions -> SetSelectedRecipeName -> RefreshPipelineOptions /
RefreshValidationSetOptions -> outer RefreshCommandState`에서 하위 갱신들이
command-state를 먼저 알리고 outer 갱신이 다시 알리는 중복을 확인했습니다.
또한 `SelectedRecipeSummary`와 `SelectedRecentBatchRunOption` setter가 같은
composite 안에서 `RecipeGuidedNextActionText`, `CorrectedOutputRerunText`,
`CorrectedOutputRerunToolTipText`, `QualifiedSnapshotPreflightText`를 직접
알려 outer command-state projection과 겹쳤습니다.

현재 owner와 상태 경계는 다음과 같습니다.

- `RefreshOptions`가 composite entry와 최종 command-state projection을
  소유합니다.
- `SetSelectedRecipeName`이 Recipe 선택 변경과 nested refresh 정책 전달을
  소유합니다.
- `RefreshPipelineOptions`와 `RefreshValidationSetOptions`가 기존
  Pipeline/Validation Set 상태와 binding projection을 소유하며,
  `refreshCommandState`가 false인 composite 호출에서는 알림만 defer합니다.
- `SelectedRecipeSummary`와 `SelectedRecentBatchRunOption` setter는 기존
  mutable state projection을 계속 소유하되 composite 중복 binding만
  `isRefreshingOptions`로 defer합니다.
- `RefreshCommandState`가 10개 shared command-state binding의 최종 notifier입니다.

호출과 상태 흐름은 다음과 같습니다.

```text
RefreshOptions
  -> SetSelectedRecipeName(current, refreshCommandState: false)
  -> RefreshPipelineOptions(..., false)
  -> RefreshValidationSetOptions(refreshCommandState: false)
  -> RecipeOptions / filter 및 기존 state projection
  -> RefreshCommandState (outer, once)
```

기존 직접 호출자는 optional 기본값 `true`를 사용하므로 독립 Pipeline/
Validation Set 갱신의 즉시 command-state 동작은 유지됩니다. binding 이름,
Recipe/Pipeline/Validation Set XML, Preview/Run 명시 실행과 layer 규칙은
변경하지 않았습니다.

읽기 순서: `Recipe/CommandSurface/Handlers.cs`의
`RefreshOptions` -> `SetSelectedRecipeName` -> `RefreshPipelineOptions`와
`Recipe/CommandSurface/ValidationSets.cs`의
`RefreshValidationSetOptions` -> 같은 Handlers 파일의
`RefreshCommandState` -> `RefreshOptionsCommandStateContract.cs` -> OVL-53
report와 `CODEBASE_STRUCTURE.md` section 9.45. 한 번의 검색은
`RefreshOptions`입니다.

집중 계약은 격리된 D: data root에서 Validation Set fixture를 준비하고
`RefreshOptions`를 호출한 뒤 shared command-state 10개 property가 각각
정확히 한 번 알림되는지 확인합니다. Debug와 Release 모두
`CONTRACT|refresh-options-command-state|passed=1|failed=0`입니다.
Debug 증거:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl53-refresh-options-command-state-contract-debug-20260909-run3\refresh-options-command-state-contract.txt`,
Release 증거:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl53-refresh-options-command-state-contract-release-20260909-run2\refresh-options-command-state-contract.txt`.

실제 검증 결과: Runner와 embedded OpenVisionLab x64 Debug/Release 빌드 각
0 warnings/0 errors, OVL-50/51/52 Release 회귀 계약 각각
`passed=1|failed=0`, `OpenVisionReadinessCheck` 통과,
`REFACTOR_AUDIT=PASS|CSharpFiles=837|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`,
`DocumentationIndex=PASS IndexedPaths=272 Routes=13 RootRedirects=102`, JSON
parse/reference와 `git diff --check` 통과를 확인했습니다.

이번 source/contract slice는 실제 Shell EXE refresh, binding 렌더링,
theme/layout, DPI/monitor, popup 및 전체 Preview/Run UI를 실행하지 않았습니다.
따라서 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`입니다.
Developer discoverability self-assessment: **PASS for this boundary**.

OVL-53은 OVL-52의 내부 owner를 다시 나누지 않고 composite caller의 실제
중복만 닫은 경계입니다. 새 재현 결함·명시적 binding/Recipe 계약 변경·
dependency 충돌 없이는 다른 모델·agent·automation이 OVL-50/51/52/53 owner를
재생성·이동·재분할·복제하지 않습니다.

작업 트리는 기존 dirty Dev 상태이며 이번 실행에서도 stage/commit/push하지
않고 Original을 변경하지 않았습니다. 예약 실행은 활성화하지 않았습니다.

OVL-53 next priority: Shell residual validation/Step Edit 책임 감사입니다.
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-52 Validation Set success projection

Status: Complete for one independently verifiable success-state projection fix;
the broader Shell validation and Step Edit refactoring program remains active.

재감사에서 `RefreshValidationSetOptions` 성공 branch가
`ValidationSuiteSummaryText`를 직접 알린 뒤 `RefreshCommandState()`가 같은
알림을 다시 보내는 중복을 확인했습니다. 직접 알림을 제거하고
`RefreshCommandState`를 shared command-state owner로 유지했습니다. 새
owner·interface·wrapper·partial·public API는 추가하지 않았습니다.

현재 owner와 상태 경계는 다음과 같습니다.

- `OpenVisionRecipeValidationSetDocumentOwner`와 기존 storage가 Validation
  Set document/XML 및 `StorageReady`를 소유합니다.
- `OpenVisionRecipeValidationSetSelectionOwner`가 선택·image-row mutable
  state를 소유합니다.
- `Recipe/CommandSurface/ValidationSets.cs`가 성공 상태의
  options/selection/rows/summary/evidence projection을 소유합니다.
- `Recipe/CommandSurface/Handlers.cs`의
  `RefreshCommandState`가 `ValidationSuiteSummaryText`의 단일 notifier입니다.

호출 경로는 `RefreshValidationSetOptions` ->
`validationSetDocumentOwner.TryLoad` -> `validationSetSelectionOwner.Refresh`
-> options/selection/split/rows 알림 -> Variant/identity/evidence projection
-> `ValidationSetSelectionSummaryText` 알림 -> `RefreshCommandState`의
`ValidationSuiteSummaryText` 1회 알림입니다. 기존 binding 이름, error branch,
Validation Set document/XML, Recipe/XML, Preview/Run 명시 실행 계약은
유지됩니다.

읽기 순서: `Recipe/CommandSurface/ValidationSets.cs`의
`RefreshValidationSetOptions` ->
`Recipe/CommandSurface/Handlers.cs`의
`RefreshCommandState` -> Shell의 `ValidationSetSelectionSummaryText`/
`ValidationSuiteSummaryText` properties ->
`tools/VisionRecipeRunnerSmoke/ValidationSetSuccessProjectionContract.cs` ->
`tools/OpenVisionReadinessCheck/Program.cs` -> OVL-52 report와
`CODEBASE_STRUCTURE.md` section 9.44. 한 번의 검색은
`RefreshValidationSetOptions`입니다.

집중 계약은 격리된 D: data root에서 유효한 Validation Set을 저장한 뒤
기존 success branch를 호출하고 selection summary, suite summary, 네 evidence
binding이 각각 한 번만 알림되는지 확인합니다. Debug/Release 모두
`CONTRACT|validation-set-success-projection|passed=1|failed=0`입니다.
Debug 증거:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl52-validation-set-success-projection-contract-debug-20260909-run2\validation-set-success-projection-contract.txt`, Release 증거:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl52-validation-set-success-projection-contract-release-20260909-run1\validation-set-success-projection-contract.txt`.

실제 검증 결과: Runner Debug/Release 빌드 0 warnings/0 errors,
OVL-50 evidence 및 OVL-51 error 회귀 Release 계약 각각
`passed=1|failed=0`, embedded OpenVisionLab x64 Debug/Release 빌드 0
warnings/0 errors, `OpenVisionReadinessCheck` 통과,
`REFACTOR_AUDIT=PASS|CSharpFiles=836|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`을 확인했습니다. 문서 인덱스와 JSON parse,
`git diff --check`도 최종 문서 갱신 후 통과했습니다. 문서 인덱스는
`DocumentationIndex=PASS IndexedPaths=271 Routes=13 RootRedirects=102`입니다.

이번 source/contract slice는 실제 Shell EXE refresh, binding 렌더링,
theme/layout, DPI/monitor, popup 및 전체 Preview/Run UI를 실행하지 않았습니다.
따라서 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`입니다.
Developer discoverability self-assessment: **PASS for this boundary**.

OVL-52와 OVL-17/46/49/50/51 owners는 새 재현 결함·명시적 binding/Recipe
계약 변경·dependency 충돌 없이는 다른 모델·agent·automation이 재생성·이동·
재분할·복제하지 않습니다. 별도 `RefreshOptions` composite caller 감사는
새 경계로 기록하며 이번 owner를 다시 열지 않습니다.

작업 트리는 기존 dirty Dev 상태이며 이번 실행에서도 stage/commit/push하지
않고 Original을 변경하지 않습니다. 예약 실행은 활성화하지 않습니다.

OVL-52 next priority: `RefreshOptions` -> `SetSelectedRecipeName` ->
`RefreshPipelineOptions`/`RefreshValidationSetOptions` -> outer
`RefreshCommandState` composite call path의 중복 command-state projection을
감사합니다. 그 다음 미완료 경계는 Shell residual validation/Step Edit 책임
감사입니다. Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-51 Validation Set projection error branch

Status: Complete for one independently verifiable error-state projection fix;
the broader Shell validation and Step Edit refactoring program remains active.

재감사에서 `RefreshValidationSetOptions`가 잘못된
`validation-sets.xml`을 읽을 때 선택·행 상태는 비우지만
`ValidationSetSelectionSummaryText` 알림을 누락하는 결함을 확인했습니다.
이제 오류 branch가 `ValidationSetSelectionSummaryText`를 한 번 알린 뒤
기존 status/identity/evidence/command-state projection을 계속 수행합니다.
새 owner·interface·wrapper·partial은 추가하지 않았습니다.

현재 owner와 상태 경계는 다음과 같습니다.

- `OpenVisionRecipeValidationSetDocumentOwner`와 기존 storage가
  Validation Set document/XML 및 `StorageReady`를 소유합니다.
- `OpenVisionRecipeValidationSetSelectionOwner`가 오류 시 선택·image-row
  mutable state를 비웁니다.
- `Recipe/CommandSurface/ValidationSets.cs`가 오류 상태의
  WPF `PropertyChanged` 순서를 소유합니다.
- 기존 `OpenVisionRecipeValidationEvidenceOwner`와 presenter는 evidence 및
  사용자 문구 정책을 계속 소유합니다.

호출 경로는 `RefreshValidationSetOptions` ->
`validationSetDocumentOwner.TryLoad` -> (failure)
`validationSetSelectionOwner.Clear` -> options/selection/rows/summary/Variant
알림 -> status/identity/evidence/command-state projection입니다. 기존
binding 이름, 성공 refresh, Validation Set document/XML, Recipe/XML,
Preview/Run 명시 실행 계약은 유지됩니다.

읽기 순서: `Recipe/CommandSurface/ValidationSets.cs`의
`RefreshValidationSetOptions` ->
`Recipe/Validation/OpenVisionRecipeValidationSetDocumentOwner.cs` ->
`OpenVisionRecipeValidationSetSelectionOwner.cs` -> Shell의
`ValidationSetSelectionSummaryText` property ->
`tools/VisionRecipeRunnerSmoke/ValidationSetProjectionErrorContract.cs` ->
`tools/OpenVisionReadinessCheck/Program.cs` -> OVL-51 report와
`CODEBASE_STRUCTURE.md` section 9.43. 한 번의 검색은
`RefreshValidationSetOptions`입니다.

집중 계약은 격리된 D: data root에서 유효한 Validation Set을 저장한 뒤 XML을
malformed content로 바꾸고 `RefreshOptions`를 호출합니다. 오류 branch가
요약 binding을 정확히 한 번 알리고 옵션을 비우며 기존 localized read-error
문구를 노출하는지 확인했습니다. Debug/Release 모두
`CONTRACT|validation-set-projection-error|passed=1|failed=0`입니다.
Debug 증거:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl51-validation-set-projection-error-contract-debug-20260909-run1\validation-set-projection-error-contract.txt`, Release 증거:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl51-validation-set-projection-error-contract-release-20260909-run1\validation-set-projection-error-contract.txt`.

실제 검증 결과: Runner x64 Debug/Release 빌드 0 warnings/0 errors 및
OVL-50 valid-selection Release 계약 `passed=1|failed=0`, embedded
OpenVisionLab x64 Debug/Release 빌드 0 warnings/0 errors,
`OpenVisionReadinessCheck` 통과,
`REFACTOR_AUDIT=PASS|CSharpFiles=835|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`, 문서 인덱스
`DocumentationIndex=PASS IndexedPaths=270 Routes=13 RootRedirects=102`, JSON
parse 통과, `git diff --check` exit 0을 확인했습니다.

이번 source/contract slice는 실제 Shell EXE malformed-file recovery,
binding 렌더링, theme/layout, DPI/monitor, popup 및 전체 Preview/Run UI를
실행하지 않았습니다. 따라서
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`입니다.
Developer discoverability self-assessment: **PASS for this boundary**.

OVL-51과 OVL-17/46/49/50 owners는 새 재현 결함·명시적 binding/Recipe 계약
변경·dependency 충돌 없이는 다른 모델·agent·automation이 재생성·이동·
재분할·복제하지 않습니다. 파일 길이, 모델 선호, 반복 요청은 재개 사유가
아닙니다.

작업 트리는 기존 dirty Dev 상태이며 이번 실행에서도 stage/commit/push하지
않았고 Original을 변경하지 않았습니다. 예약 실행은 활성화하지 않았습니다.

OVL-51 next priority: `RefreshValidationSetOptions` 성공 branch의 선택·
summary·evidence·command-state 알림 순서를 재감사합니다. 새 seam·결함·계약이
없으면 현재 Shell application projection을 유지하고, 그 다음 미완료 경계는
Shell residual validation/Step Edit 책임 감사입니다. Recommended model:
`gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-50 Validation Set evidence `PropertyChanged` projection

Status: Complete for one independently verifiable duplicate-notification fix;
the broader Shell validation and Step Edit refactoring program remains active.

재감사에서 Validation Set 선택 변경 시
`RefreshValidationSetImageRows()`와 `SelectedValidationSetOption` setter가
`ValidationSetExpectedText`, `ValidationSetAcceptanceText`,
`ValidationSetCalibrationText`, `ValidationSetNextActionText`를 각각 다시
알리는 중복 경로를 확인했습니다. image-row helper는 이제 행과 pending
Variant 상태만 갱신하고, 선택 setter가 네 evidence binding을 한 번만
알립니다. 새 정책 owner·interface·wrapper·partial은 추가하지 않았습니다.

현재 owner와 상태 경계는 다음과 같습니다.

- `OpenVisionRecipeValidationSetDocumentOwner`와 기존 storage가
  Validation Set document/XML을 소유합니다.
- `OpenVisionRecipeValidationSetSelectionOwner`가 선택 및 image-row mutable
  state를 씁니다.
- `OpenVisionRecipeValidationEvidenceOwner`가 WPF와 분리된
  acceptance/calibration 정책 결과를 만듭니다.
- `OpenVisionShellHostRecipeCommandSurface`가 선택 setter에서 WPF
  `PropertyChanged` 순서와 command-state를 투영합니다.

호출 경로는 `SelectedValidationSetOption setter` ->
`validationSetSelectionOwner.SelectSet` -> `RefreshValidationSetImageRows` ->
`RefreshImageRows`/pending Variant projection -> row/selection summary/evidence
알림 -> `RefreshCommandState`입니다. 기존 binding 이름, Validation Set
document/XML, Recipe/XML, Preview/Run 명시 실행 계약은 유지됩니다.

읽기 순서: `Recipe/CommandSurface/RecipeCommandSurface.cs`의 선택 setter ->
`Recipe/CommandSurface/ValidationSets.cs`의 image-row helper
및 document flow -> `Recipe/Validation/OpenVisionRecipeValidationSetSelectionOwner.cs`
-> `OpenVisionRecipeValidationEvidenceOwner.cs` ->
`tools/VisionRecipeRunnerSmoke/ValidationSetEvidenceNotificationContract.cs` ->
`tools/OpenVisionReadinessCheck/Program.cs` -> OVL-50 report와
`CODEBASE_STRUCTURE.md` section 9.42. 한 번의 검색은
`NotifyValidationSetEvidenceChanged`입니다.

집중 계약은 격리된 D: data root에 두 세트를 저장하고 기존 Shell command
surface를 구성한 뒤 다른 세트를 선택하여 네 evidence binding이 각각 한 번
발생하는지 확인합니다. 결과는
`passed=1|failed=0`입니다. Debug 증거:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl50-validation-set-evidence-notification-contract-debug-20260909-run2\validation-set-evidence-notification-contract.txt`, Release 증거:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl50-validation-set-evidence-notification-contract-release-20260909-run1\validation-set-evidence-notification-contract.txt`.
정적 readiness gate는 helper에 evidence 알림이 없고 선택 setter가 단일
projection owner임을 확인합니다.

실제 검증 결과: Runner x64 Debug/Release 빌드 0 warnings/0 errors 및 위
계약 Debug/Release `passed=1|failed=0`, embedded OpenVisionLab x64
Debug/Release 빌드 0 warnings/0 errors, `OpenVisionReadinessCheck` 통과,
`REFACTOR_AUDIT=PASS|CSharpFiles=834|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`, 문서 인덱스
`DocumentationIndex=PASS IndexedPaths=269 Routes=13 RootRedirects=102`, JSON
parse 통과, `git diff --check` exit 0을 확인했습니다.

이번 source/contract slice는 실제 Shell EXE의 선택 변경, ComboBox
transient-null, binding, theme/layout, DPI/monitor, popup 및 전체
Preview/Run UI를 실행하지 않았습니다. 따라서
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`입니다.
Developer discoverability self-assessment: **PASS for this boundary**.

OVL-50과 OVL-17/46/49 owners는 새 재현 결함·명시적 binding/Recipe 계약
변경·dependency 충돌 없이는 다른 모델·agent·automation이 재생성·이동·
재분할·복제하지 않습니다. 파일 길이, 모델 선호, 반복 요청은 재개 사유가
아닙니다.

작업 트리는 기존 dirty Dev 상태이며 이번 실행에서도 stage/commit/push하지
않았고 Original을 변경하지 않았습니다. 예약 실행은 활성화하지 않았습니다.

OVL-50 next priority: `RefreshValidationSetOptions`의 전체 projection/error
branch를 재감사하여 별도 owner가 필요한 새 seam 또는 재현 결함이 있는지
확인합니다. 새 seam·결함·계약이 없으면 현재 Shell application projection을
유지하고, 그 다음 미완료 경계는 Shell residual validation/Step Edit 책임
감사입니다. Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-49 PinArrayGap selection-restoration owner

Status: Complete for one independently verifiable PinArrayGap selection
restoration boundary; the overall refactoring program remains active.

재감사에서 `RefreshValidationSetOptions`가
`OpenVisionRecipePinArrayGapValidationRecordStorage.TryLoad`를 직접 호출해
OVL-46 identity owner를 우회하는 잔여 결합을 확인했습니다. 기존
`OpenVisionRecipePinArrayGapValidationIdentityOwner.TryGetFrozenSelectionNames`
가 record를 읽고 Train/Validation/Test 이름만 반환하도록 이동했습니다.
Shell은 그 결과를 기존 `OpenVisionRecipeValidationSetSelectionOwner`와
`PropertyChanged`/evidence/command-state projection에 전달합니다.

Validation Set document/XML persistence, selection mutable state, PinArrayGap
status, Recipe/XML schema, and explicit Preview/Run routing remain unchanged.
No new interface, wrapper, partial, callback, timer, or dispose owner was
added. The Shell ValidationSets file no longer calls
`OpenVisionRecipePinArrayGapValidationRecordStorage.TryLoad` directly.

호출 경로: `RefreshValidationSetOptions` ->
`ValidationSetDocumentOwner.TryLoad` ->
`pinArrayGapValidationIdentityOwner.TryGetFrozenSelectionNames` ->
`validationSetSelectionOwner.Refresh` -> 기존 Shell binding/evidence/command
projection입니다. Mutable state write owner는 document owner와 selection owner,
WPF-facing state는 Shell이 계속 소유합니다.

읽기 순서: `Recipe/CommandSurface/ValidationSets.cs`의
`RefreshValidationSetOptions` ->
`Recipe/Validation/OpenVisionRecipePinArrayGapValidationIdentityOwner.cs`의
`TryGetFrozenSelectionNames` -> record storage -> selection owner ->
`tools/VisionRecipeRunnerSmoke/PinArrayGapValidationIdentityOwnerContract.cs`
-> `tools/OpenVisionReadinessCheck/Program.cs` -> OVL-49 report와
`CODEBASE_STRUCTURE.md` section 9.41. 한 번의 검색은
`TryGetFrozenSelectionNames`입니다.

Runner Debug/Release 계약은 각각 `passed=5|failed=0`, Runner와 embedded
OpenVisionLab x64 Debug/Release 빌드는 0 warnings/0 errors,
`OpenVisionReadinessCheck`와 `REFACTOR_AUDIT=PASS`를 확인했습니다. 증거:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl49-pinarraygap-validation-identity-owner-contract-debug-20260909-run1`,
`...release-20260909-run1`, `...ovl49-refactor-audit-20260909-run1`.

이번 source/contract slice는 실제 Shell EXE의 Validation Set 선택 변경,
ComboBox transient-null, binding, theme/layout, DPI/monitor, popup 및 전체
Preview/Run UI를 실행하지 않았습니다. 따라서
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`입니다.
Developer discoverability self-assessment: **PASS for this boundary**.

OVL-49와 OVL-46 identity owner는 새 결함·명시적 계약 변경·입증된
dependency 충돌 없이는 다른 모델·agent·automation이 재생성·이동·재분할·
복제하지 않습니다. 파일 길이, 모델 선호, 반복 요청은 재개 사유가
아닙니다.

작업 트리는 기존 dirty Dev 상태이며 이번 실행에서도 stage/commit/push하지
않았고 Original을 변경하지 않았습니다. 예약 실행은 활성화하지 않았습니다.

OVL-49 next priority: Shell `PropertyChanged`와 Validation Set evidence
projection을 별도 owner로 이동할 독립 request/result seam이 있는지 검토하고,
새 seam·결함·계약이 없으면 현재 Shell application projection을 유지합니다.
그 다음 미완료 경계는 Shell residual validation/step-edit 책임 감사입니다.
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-48 Validation Set save-failure status owner

Status: Complete for one independently verifiable Validation Set save-failure
status boundary; the overall refactoring program remains active.

The save-failure audit confirmed that
`Recipe/CommandSurface/ValidationSets.cs` must retain
`RefreshValidationSetOptions` because it owns selection restoration,
`PropertyChanged`, PinArrayGap identity, evidence notifications, command state,
and the Validation Set binding. No separate recovery wrapper was introduced.
The direct `operation + " ERROR: " + error` message composition now belongs to
the existing `OpenVisionRecipeValidationSetPresenter.BuildSaveErrorStatus`.

`validationSetDocumentOwner.TrySave` and the existing storage/XML persistence
remain unchanged. Create/Delete/Add/Variant/Repair/Remove commands still call
the same save helper, refresh the disk-backed state on failure, and keep
Recipe/XML, Validation Set schema, and explicit Preview/Run routing unchanged.
No new interface, partial, callback, timer, or dispose owner was added.

`ValidationSetStatusPresenterContract` passed `2/2` in both x64 Debug and
Release, including Korean and English save-error wording. Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl48-validation-set-status-presenter-contract-debug-20260909-run1`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl48-validation-set-status-presenter-contract-release-20260909-run1`.
Runner and embedded OpenVisionLab x64 Debug/Release builds were all 0 warnings/
0 errors. `OpenVisionReadinessCheck` and
`REFACTOR_AUDIT=PASS|CSharpFiles=833|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0` passed; audit evidence is
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl48-refactor-audit-20260909-run1`.
Final documentation index was `DocumentationIndex=PASS IndexedPaths=267
Routes=13 RootRedirects=102`; JSON parse and `git diff --check` also passed.

이번 source/contract slice는 실제 Shell EXE 저장 실패 유도, 폴더 선택,
binding, theme/layout, DPI/monitor와 전체 Validation Set Preview/Run UI를
실행하지 않았습니다. 따라서 `소스 코드 기준 검토 완료 / 실제 Runtime UI
검증 필요`이며, 해당 UI 회귀는 남은 위험입니다.

읽기 순서: `Recipe/CommandSurface/ValidationSets.cs`의
`TrySaveValidationSetDocument`/`RefreshValidationSetOptions` ->
`Recipe/Review/OpenVisionRecipeValidationSetPresenter.cs`의
`BuildSaveErrorStatus` -> `Recipe/Validation/OpenVisionRecipeValidationSetDocumentOwner.cs`
-> `Recipe/Validation/OpenVisionRecipeValidationSetStorage.cs` ->
`tools/VisionRecipeRunnerSmoke/ValidationSetStatusPresenterContract.cs` ->
`tools/OpenVisionReadinessCheck/Program.cs` -> OVL-48 report와
`CODEBASE_STRUCTURE.md` section 9.40.

Developer discoverability self-assessment: **PASS for this boundary**. 저장
문구, persistence, mutable document, refresh/binding을 각각 한 번의 검색으로
확인할 수 있고, 별도 recovery owner가 불필요한 이유도 기록되어 있습니다.

이 save-status owner와 기존 OVL-47 presenter owner는 새 결함·명시적 문구/
계약 변경·입증된 dependency 충돌 없이는 다른 모델·agent·automation이
재생성·이동·재분할·복제하지 않습니다. 파일 길이, 모델 선호, 반복 요청은
재개 사유가 아닙니다.

작업 트리는 기존 dirty Dev 상태이며 이번 실행에서도 stage/commit/push하지
않았고 Original을 변경하지 않았습니다. 예약 실행은 활성화하지 않았습니다.

OVL-48 next priority: `RefreshValidationSetOptions`의 선택/evidence/
`PropertyChanged` projection에 독립 request/result 경계가 있는지 재감사하고,
새 결함·계약·독립 테스트 seam이 없으면 그대로 유지합니다.
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-47 Validation Set status presenter owner

Status: Complete for one independently verifiable Validation Set
folder-registration status boundary; the overall refactoring program remains
active.

`Recipe/CommandSurface/ValidationSets.cs`가 직접 조합하던
폴더 등록 오류, 빈 폴더, 이미지 추가/갱신/건너뜀 수량 문구를 기존
`OpenVisionRecipeValidationSetPresenter`의 세 builder로 이동했습니다.
`OpenVisionRecipeValidationSetStorage.TryGetTopLevelImagePaths`가 파일 수집을,
`OpenVisionRecipeValidationSetDocumentOwner.TryAddImages`가 document mutable
변경을, Shell의 `TrySaveValidationSetDocument`와 `RefreshValidationSetOptions`가
저장/refresh 순서를 계속 소유합니다. Recipe/XML, Validation Set schema,
binding, 명시적 Preview/Run routing은 변경하지 않았습니다.

실제 call path는 `AddValidationSetFolder` -> storage top-level image
enumeration -> document owner `TryAddImages` -> Shell save/refresh ->
`OpenVisionRecipeValidationSetPresenter.BuildImageRegistrationStatus` ->
`ValidationSuiteStatusText` binding입니다. 오류와 빈 폴더도 presenter 전용
builder를 사용합니다. presenter는 WPF-free 순수 문자열 정책이고, 새 mutable
state, callback, timer, dispose owner를 만들지 않았습니다.

`ValidationSetStatusPresenterContract`는 한국어/영어 세 문구 family를 Debug와
Release에서 각각 `passed=2|failed=0`으로 통과했습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl47-validation-set-status-presenter-contract-debug-20260909-run2`,
`...ovl47-validation-set-status-presenter-contract-release-20260909-run2`입니다.
Runner와 embedded OpenVisionLab x64 Debug/Release 빌드는 모두 0 warnings/0
errors였고, `OpenVisionReadinessCheck`와
`REFACTOR_AUDIT=PASS|CSharpFiles=833|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`도 통과했습니다.
문서 인덱스는 `DocumentationIndex=PASS IndexedPaths=266 Routes=13 RootRedirects=102`,
JSON parse와 `git diff --check`도 통과했습니다.

이번 source/contract slice는 실제 Shell EXE의 폴더 선택 click, binding,
theme/layout, DPI/monitor, popup 및 전체 Validation Set Preview/Run UI를
재실행하지 않았습니다. 따라서 `소스 코드 기준 검토 완료 / 실제 Runtime UI
검증 필요`이며, 해당 UI 회귀는 남은 위험입니다.

읽기 순서: `Recipe/CommandSurface/ValidationSets.cs`의
`AddValidationSetFolder`/`AddValidationSetImages`/`TrySaveValidationSetDocument`
-> `Recipe/Review/OpenVisionRecipeValidationSetPresenter.cs`의 세 builder ->
`Recipe/Validation/OpenVisionRecipeValidationSetStorage.cs` ->
`Recipe/Validation/OpenVisionRecipeValidationSetDocumentOwner.cs` ->
`tools/VisionRecipeRunnerSmoke/ValidationSetStatusPresenterContract.cs` ->
`tools/OpenVisionReadinessCheck/Program.cs` -> OVL-47 report와
`CODEBASE_STRUCTURE.md` section 9.39.

Junior developer self-assessment: **PASS for this boundary**. 상태 문구,
파일 수집, document mutation, 저장/refresh와 binding owner가 이름으로
분리되어 한 번의 검색으로 추적됩니다.

이 owner는 새 결함·명시적 문구/계약 변경·입증된 dependency 충돌 없이는 다른
모델·agent·automation이 재생성·이동·재분할·복제하지 않습니다. 파일 길이,
모델 선호, 반복 요청은 재개 사유가 아닙니다.

작업 트리는 기존 dirty Dev 상태이며 이번 실행에서도 stage/commit/push하지
않았고 Original을 변경하지 않았습니다. 예약 실행은 활성화하지 않았습니다.

OVL-47 next priority: `TrySaveValidationSetDocument` 실패 후
`RefreshValidationSetOptions`와 `ValidationSuiteStatusText` 상태 전이의
mutable-state boundary를 재감사하고, 독립 owner가 실제로 필요한 경우에만
다음 한 slice로 분리합니다. 기존 OVL-01/02/03/04/05/06a/07/08/10/12-47
완료 owner는 새 근거 없이 닫힌 상태를 유지합니다.
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-46 Shell PinArrayGap validation identity owner

Status: Complete for one independently verifiable Shell validation identity
boundary; the overall refactoring program remains active.

`Recipe/CommandSurface/Handlers.cs`가 직접 수행하던
PinArrayGap 2단계 검증 기준의 선택 Pipeline XML 원문 읽기와 record
Freeze/Evaluate orchestration을 `OpenVisionRecipePinArrayGapValidationIdentityOwner`
로 이동했습니다. 기존
`OpenVisionRecipePinArrayGapValidationRecordStorage`가 XML·row·split hash와
record persistence를 계속 소유합니다. Shell의 Train/Validation/Test 선택,
`CanExecute`, localized `PHASE 2` 상태, frozen flag, 명시적 Validation Set
실행 routing과 Recipe/XML·Preview/Run 계약은 유지했습니다.

실제 call path는 `FreezePinArrayGapValidationIdentity` 또는
`RefreshPinArrayGapValidationIdentityState` ->
`pinArrayGapValidationIdentityOwner.Freeze/Evaluate` -> raw Pipeline XML
read -> existing record storage -> Shell status projection입니다. mutable
selection/status와 lifetime은 Shell이 계속 보유하고, owner는 WPF-free
호출별 result만 반환합니다.

`PinArrayGapValidationIdentityOwnerContract=PASS` 4/4가 x64 Debug와 Release
에서 통과했습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl46-pinarraygap-validation-identity-owner-contract-debug-20260909-run1`,
`...ovl46-pinarraygap-validation-identity-owner-contract-release-20260909-run1`입니다.
검증은 Freeze persistence, unchanged match, changed-image stale detection,
missing Pipeline XML failure와 저장 XML 불변을 확인했습니다.

VisionRecipeRunnerSmoke와 embedded OpenVisionLab x64 Debug/Release 빌드는
모두 0 warnings/0 errors였고, `OpenVisionReadinessCheck`도 통과했습니다.
최신 감사는
`REFACTOR_AUDIT=PASS|CSharpFiles=832|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`이며, 문서 인덱스도 `DocumentationIndex=PASS IndexedPaths=265 Routes=13 RootRedirects=102`로 통과했습니다.
이번 source/contract slice에서는 실제 PinArrayGap panel click/binding,
theme/layout/DPI/monitor와 전체 Validation Set Preview/Run UI를 실행하지
않았습니다.

읽기 순서: `Recipe/CommandSurface/Handlers.cs`의
`FreezePinArrayGapValidationIdentity`/`RefreshPinArrayGapValidationIdentityState`
-> `Recipe/Validation/OpenVisionRecipePinArrayGapValidationIdentityOwner.cs`
-> `OpenVisionRecipePinArrayGapValidationRecordStorage.cs`
-> `tools/VisionRecipeRunnerSmoke/PinArrayGapValidationIdentityOwnerContract.cs`
-> OVL-46 보고서와 `CODEBASE_STRUCTURE.md` section 9.38.

이 owner는 새 PinArrayGap identity 결함·계약 변경·입증된 dependency 충돌
없이는 다른 모델·agent·automation이 재생성·이동·재분할·복제하지 않습니다.
파일 길이, 모델 선호, 반복 요청은 재개 사유가 아닙니다.

작업 트리는 기존 dirty Dev 상태이며 파일을 stage/commit/push하지 않았고
Original을 변경하지 않았습니다. 예약 실행은 활성화하지 않았습니다.

OVL-46 next priority: Validation Set 이미지 폴더 등록 경로에서
`AddValidationSetFolder`의 파일 수집·document mutation·save/status projection
중 하나의 mutable-state boundary만 선택합니다.
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-45 Shell LLM draft review baseline owner

Status: Complete for one independently verifiable Shell LLM XML draft-review
boundary; the overall refactoring program remains active.

`OpenVisionShellHostRecipeCommandSurface`가 직접 수행하던 LLM XML 초안
import/diff 검토용 active Pipeline XML 조회를
`OpenVisionRecipeLlmDraftReviewOwner`로 이동했습니다. owner는 Recipe 이름과
draft를 받아 active Pipeline을 읽고 기존
`OpenVisionRecipePipelineComparisonPresenter`의 두 읽기 전용 결과를
반환합니다. CommandSurface의 `LlmXmlDraft*` binding state, validation
report/dependency rows, Import readiness, 명시적 Import/Preview/Run 계약은
그대로 유지됩니다. Recipe XML 저장/활성화와 locator 승인 gate도 변경하지
않았습니다.

실제 call path는 `LlmXmlDraftWorkflow.ValidateLlmXmlDraftText` 또는 Import
경로 -> `llmDraftReviewOwner.Build` -> active Pipeline storage read ->
comparison presenter -> 기존 CommandSurface projection입니다. mutable state는
CommandSurface가 보유하고, owner는 호출당 결과만 반환하는 무상태 객체라
dispose/callback/lifetime 변경이 없습니다. `BuildPipelineVariantComparisonReport`
의 별도 variant 비교 경로는 유지했습니다.

`LlmDraftReviewOwnerContract=PASS` 3/3이 x64 Debug와 Release에서 통과했습니다.
증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl45-llm-draft-review-owner-contract-debug-20260909-run2`,
`...ovl45-llm-draft-review-owner-contract-release-20260909-run1`입니다.
`OpenVisionReadinessCheck`도 pass하여 former active-Pipeline helper가
CommandSurface에 남지 않고 owner delegation이 존재함을 확인했습니다.
VisionRecipeRunnerSmoke Debug/Release와 embedded OpenVisionLab
Debug/Release(`OpenVisionLabEnableEmbeddedSmokeRunner=true`) 빌드는 모두
0 warnings/0 errors였습니다. `REFACTOR_AUDIT=PASS`는
`CSharpFiles=830|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`,
documentation index는 `IndexedPaths=264`, `Routes=13`, `RootRedirects=102`이며
JSON parse와 `git diff --check`도 통과했습니다(기존 LF/CRLF normalization
notice만 출력). 이번 source/contract slice에서는 실제 LLM tab desktop UI,
Clipboard, alternate theme/DPI, locator Packet UI, Import/Preview/Run runtime을
재실행하지 않았습니다.

읽기 순서: `Recipe/CommandSurface/LlmXmlDraftWorkflow.cs`
-> `Recipe/Review/OpenVisionRecipeLlmDraftReviewOwner.cs` ->
`Recipe/Review/OpenVisionRecipePipelineComparisonPresenter.cs` ->
`tools/VisionRecipeRunnerSmoke/LlmDraftReviewOwnerContract.cs` -> OVL-45 보고서와
`CODEBASE_STRUCTURE.md` section 9.37.

이 owner는 새 draft-review 결함, 검토 계약 변경, 또는 입증된 dependency
충돌 없이는 다른 모델·agent·automation이 재생성·이동·재분할·복제하지
않습니다. 파일 길이, 모델 선호, 반복 요청은 재개 사유가 아닙니다.

작업 트리는 기존 dirty Dev 상태이며 파일을 stage/commit/push하지 않았고
Original을 변경하지 않았습니다. 예약 실행은 활성화하지 않았습니다.

OVL-45 next priority: 남은 Shell validation/evidence 또는 step-edit 중
mutable-state boundary가 입증된 단일 call path 하나만 선택합니다. OVL-01/02/
03/04/05/06a/07/08/10/12-45 completed owners remain closed.
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-44 Smoke mouse input owner

Status: Complete for one independently verifiable Direct Smoke low-level
mouse-input boundary; the overall refactoring program remains active.

`OpenVisionLabDirectSmokeRunner`의 저수준 `SetCursorPos`/`mouse_event`,
left-button 전환, screen-pixel 반올림, 두 개의 background drag thread
생성·pump·timeout·Join·예외 변환을 `SmokeMouseInput` concrete owner로
이동했습니다. Direct runner는 WPF 좌표 계산, Dispatcher pump, docking
문서/layer/route 상태와 시나리오 판정을 계속 소유합니다. 기존 mouse flag,
delay, thread name, 8초 timeout, 오류 문구, `SetCursorPos` 오류 텍스트와
Recipe/XML·Preview/Run 계약은 유지했습니다.

`SMOKE_MOUSE_INPUT_CONTRACT=PASS` 10/10 in x64 Debug and Release.
PipelineViewerScreenshotSmoke Debug/Release와 embedded OpenVisionLab
Debug/Release(`OpenVisionLabEnableEmbeddedSmokeRunner=true`)가 모두 0
errors로 빌드되었습니다. 기존 monitor placement 9/9, task waiter 8/8,
command-line 10/10 회귀도 Debug/Release에서 통과했습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl44-smoke-mouse-input-contract-debug-20260909-run3`,
`...\ovl44-smoke-mouse-input-contract-release-20260909-run2`입니다.
`REFACTOR_AUDIT=PASS`는
`CSharpFiles=828|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`,
documentation index는 `IndexedPaths=263`, `Routes=13`, `RootRedirects=102`입니다.
실제 desktop cursor/drag, monitor/DPI/theme matrix와 full docking runtime은
이번 source/contract slice에서 실행하지 않았습니다.

읽기 순서: Direct의 `SmokeMouseInput.` 호출 ->
`tools/PipelineViewerScreenshotSmoke/SmokeMouseInput.cs` ->
`SmokeMouseInputContract.cs` -> embedded conditional Link ->
`docs/reports/OPENVISIONLAB_OVL44_SMOKE_MOUSE_INPUT_OWNER_20260909.md` 및
`CODEBASE_STRUCTURE.md` section 9.36.

이 owner는 새 입력 결함, gesture/timeout 계약 변경 또는 입증된 dependency
충돌 없이는 다른 모델·agent·automation이 재생성·이동·재분할·복제하지
않습니다. 파일 길이, 모델 선호, 반복 요청은 재개 사유가 아닙니다.

작업 트리는 기존 dirty Dev 상태이며 파일을 stage/commit/push하지 않았고
Original을 변경하지 않았습니다. 예약 실행은 활성화하지 않았습니다.

OVL-44 next priority: fresh residual Direct Smoke responsibility audit, then
return to the recorded Shell/Recipe roadmap only when one call path and
mutable-state boundary are proven. OVL-01/02/03/04/05/06a/07/08/10/12-44
completed owners remain closed. Recommended model: `gpt-6-astra` | Reasoning
effort: `high`.

## Current Slice — OVL-43 Smoke Window monitor placement owner

Status: Complete for one independently verifiable Direct Smoke monitor
placement boundary; the overall refactoring program remains active.

`OpenVisionLabDirectSmokeRunner`의 20개 시나리오 호출이 직접 소유하던
Win32 monitor enumeration, leftmost selection, centered `SetWindowPos`,
post-move intersection check, native rectangle structs, and exact evidence
format are now owned by `SmokeWindowMonitorPlacement`. The Direct runner keeps
the existing `PlaceWindowOnLeftmostMonitor` wrapper, scenario names, Window
lifetime, Dispatcher timing, error text, and monitor evidence contract.

`SMOKE_WINDOW_MONITOR_PLACEMENT_CONTRACT=PASS` 9/9 in x64 Debug and Release.
Pipeline Smoke Debug/Release builds completed with 0 errors and the
pre-existing nullable `CS8600` warning only. Embedded OpenVisionLab
Debug/Release builds with `OpenVisionLabEnableEmbeddedSmokeRunner=true`
completed with 0 warnings/0 errors. `REFACTOR_AUDIT=PASS` is
`CSharpFiles=826|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`;
documentation index is `DocumentationIndex=PASS IndexedPaths=262 Routes=13
RootRedirects=102`. The owner has deterministic geometry/intersection checks
and is conditionally linked for embedded Direct Smoke. Physical EXE monitor
placement, theme, DPI, input, and desktop rendering remain unverified in this
source/contract slice.

Detailed report:
`docs/reports/OPENVISIONLAB_OVL43_SMOKE_WINDOW_MONITOR_PLACEMENT_OWNER_20260909.md`.
Owner map: `docs/admin/CODEBASE_STRUCTURE.md` section 9.35. Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl43-smoke-window-monitor-placement-contract-debug-20260909-run1` and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl43-smoke-window-monitor-placement-contract-release-20260909-run1`.

Developer discoverability assessment: **PASS for this boundary**. Search the
existing Direct wrapper, follow the single `SmokeWindowMonitorPlacement` owner,
then read its contract and conditional app link. Another model, agent, or
automation must not recreate, rename, re-split, or move this owner without a
new monitor-placement defect, changed selection/placement contract, or
demonstrated dependency conflict.

The worktree remains an existing dirty Dev checkout; no files were staged and
no push or Original mutation was performed.

OVL-44 completed the former residual low-level mouse-input pointer. Follow the
current OVL-44 next priority; do not recreate the completed mouse, monitor,
screenshot, docking, fixture, clipboard, or task-wait owners. The completed
OVL-43 owner remains closed. Recommended model: `gpt-6-astra` | Reasoning
effort: `high`.

## Current Slice — OVL-42 Smoke Task waiter owner

Status: Complete for one independently verifiable shared Smoke task-wait
boundary; the overall refactoring program remains active.

Direct와 Pipeline Smoke에 각각 있던 pump 기반 `WaitForTaskWithPump` loop를
WPF-free `SmokeTaskWaiter` owner로 통합했습니다. Direct wrapper는 required
task/null 예외, 20초 timeout, 기존 문구를 유지하고 Pipeline wrapper는
optional task/null no-op, 호출별 timeout, 기존 문구를 유지합니다. 두 caller의
`Pump(4)` dispatcher callback과 task 생성·수명은 caller에 남아 있습니다.

`SMOKE_TASK_WAITER_CONTRACT=PASS` 8/8 in x64 Debug and Release. OVL-41
clipboard 7/7, OVL-40 Direct screenshot 6/6, OVL-22 command-line 10/10도
양 구성에서 통과했습니다. Pipeline Smoke Debug/Release는 0 errors이며
기존 nullable `CS8600` warning만 남았고, embedded OpenVisionLab
Debug/Release는 0 warnings/0 errors입니다. `REFACTOR_AUDIT=PASS`는
`CSharpFiles=824|XamlFiles=60|PartialDeclarations=110|ProjectCycles=0|ShellStorageCalls=0`,
documentation index는 `IndexedPaths=261`, `Routes=13`, `RootRedirects=102`입니다.
상세 보고서는
`docs/reports/OPENVISIONLAB_OVL42_SMOKE_TASK_WAITER_OWNER_20260909.md`,
owner map은 `docs/admin/CODEBASE_STRUCTURE.md` section 9.34입니다.

개발자 discoverability assessment: **PASS for this boundary**. 읽기 순서는
Direct/Pipeline wrapper -> `SmokeTaskWaiter` -> contract -> embedded csproj
link이며, shared wait loop의 searchable owner는 하나입니다. 새 task-wait
결함·timeout 계약 변경·입증된 dependency 충돌 없이는 다른 모델, agent,
automation이 이 owner를 복제·이동·재분할하지 않습니다.

실제 Dev는 기존 dirty worktree이며 이 실행에서 staged 파일·push는
없었습니다. Original은 untouched입니다. contract는 in-process task와
caller pump만 검증했으므로 physical desktop UI, monitor, theme, DPI,
Clipboard runtime은 미검증입니다.

OVL-43 completed the former residual Direct monitor-placement pointer. Follow
the current OVL-43 next priority; do not recreate the completed monitor,
screenshot, docking, fixture, clipboard, or task-wait owners.
The completed OVL-42 owner remains closed. Recommended model: `gpt-6-astra` |
Reasoning effort: `high`.

## Current Slice — OVL-41 Smoke clipboard retry owner

Status: Complete for one independently verifiable shared Smoke clipboard
retry boundary; the overall refactoring program remains active.

Direct and Pipeline Smoke had separate copies of the same COM clipboard retry
loop. `SmokeClipboardRetry.Run<T>` now owns the target HRESULT filter,
40-attempt limit, delay, pump callback, and final exception propagation. The
existing caller wrappers still own `System.Windows.Clipboard` access and pass
their existing `Pump(4)` callback, so UI-thread affinity and scenario timing
remain at the call sites. The owner is WPF-free and conditionally linked into
the app for embedded Direct Smoke.

`SMOKE_CLIPBOARD_RETRY_CONTRACT=PASS` 7/7 in x64 Debug and Release. Pipeline
Smoke Debug/Release builds completed with 0 errors and the pre-existing
`CS8600` warning only; embedded OpenVisionLab Debug/Release builds completed
with 0 warnings and 0 errors. OVL-22 command-line contract passed 10/10 and
OVL-40 screenshot owner contract passed 6/6 in both configurations. The
post-slice audit passed with
`CSharpFiles=822|XamlFiles=60|PartialDeclarations=110|ProjectCycles=0|ShellStorageCalls=0`;
documentation index passed with `IndexedPaths=260`, `Routes=13`, and
`RootRedirects=102`; JSON parse also passed.
Evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl41-smoke-clipboard-retry-contract-20260909`.
The detailed report is
`docs/reports/OPENVISIONLAB_OVL41_SMOKE_CLIPBOARD_RETRY_OWNER_20260909.md`;
the owner map is section 9.33 of `docs/admin/CODEBASE_STRUCTURE.md`.

Developer discoverability assessment: **PASS for this boundary**. The shortest
route is existing scenario Clipboard wrapper -> `SmokeClipboardRetry` ->
caller-owned Clipboard action. Another model, agent, or automation must not
recreate, rename, re-split, or move this owner without a new clipboard defect,
changed retry contract, or demonstrated dependency conflict.

Actual Dev remains an existing dirty worktree with no staged files; Original is
untouched and no push was performed.

Next priority: perform a fresh residual Direct Smoke responsibility audit and
select one owner only when its call path and mutable-state boundary are proven;
completed OVL-01/02/03/04/05/06a/07/08/10/12-41 owners stay closed.
Recommended model: `gpt-6-astra` | Reasoning effort: `high`.

## Current Slice — OVL-40 Direct Smoke Screenshot PNG owner

Status: Complete for one independently verifiable Direct Smoke screenshot
output boundary; the overall refactoring program remains active.

The three PNG implementations in `OpenVisionLabDirectSmokeRunner` are now
delegated to the existing canonical `ScreenshotPngWriter` owner:
`WriteDpiAwareWindowPng`, `WriteWindowScreenPng`, and
`WriteWindowsScreenPng`. Direct keeps the existing private wrapper names,
window activation, visibility filtering, layout update, and pump order, so
scenario call sites and output names remain unchanged. The app links the
existing writer only when `OpenVisionLabEnableEmbeddedSmokeRunner` is true.

This is a changed dependency boundary for the completed OVL-25 owner, not a
second writer or a file-size split. Recipe/XML, explicit Preview/Run,
Layer/ImageSpace, public bindings, and product UI behavior remain unchanged.

`DIRECT_SMOKE_SCREENSHOT_WRITER_CONTRACT=PASS` 6/6 in x64 Debug and Release;
existing PNG writer contract 7/7, capture lifecycle contract 10/10, and
OVL-22 command-line contract 10/10 also passed in both configurations.
Pipeline Smoke built with 0 errors and the existing nullable `CS8600` warning;
embedded OpenVisionLab built with 0 warnings and 0 errors in Debug/Release.
The post-change audit returned
`REFACTOR_AUDIT=PASS|CSharpFiles=820|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
The detailed report is
`docs/reports/OPENVISIONLAB_OVL40_DIRECT_SMOKE_SCREENSHOT_WRITER_20260909.md`;
the owner map is section 9.32 of `docs/admin/CODEBASE_STRUCTURE.md`.

Junior developer self-assessment: **PASS for this boundary**. The shortest
route is Direct scenario -> existing screenshot wrapper ->
`ScreenshotPngWriter` -> PNG file. No second screenshot owner should be
introduced. Another model, agent, or scheduled run must not recreate, rename,
re-split, or move this owner without a new output defect, changed Direct
capture contract, or demonstrated responsibility/dependency conflict.

The isolated checkpoint `408469b7` is local and has not been pushed. Actual
Dev HEAD remains `65e2fd8b`; this slice is present as unstaged changes in the
pre-existing dirty worktree and no files are staged. Original remains
untouched. Actual Dev evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl40-actual-validation-20260909`:
Direct 6/6, existing PNG 7/7, capture lifecycle 10/10, OVL-22 10/10,
embedded and Pipeline Debug/Release builds, `REFACTOR_AUDIT=PASS`, and
`DocumentationIndex=PASS IndexedPaths=259 Routes=13 RootRedirects=102`.

OVL-42 completed the former residual task-wait pointer. Follow the current
OVL-42 next priority; do not recreate the completed screenshot, docking,
fixture, clipboard, or task-wait owners.

## Current Slice — OVL-39 Smoke docking state files owner

Status: Complete for one independently verifiable persisted UI-state/file
lifecycle boundary; the overall refactoring program remains active.

The duplicated `LayerDocking.layers` / `LayerDocking.layout` backup, restore,
clear, and evidence-copy implementations are now owned by the WPF-free
`SmokeDockingStateFiles` concrete module. `PipelineViewerScreenshotSmoke.Program`
calls `RunWithBackup` directly. `OpenVisionLabDirectSmokeRunner` keeps its
existing private wrapper names for call-site compatibility, but each wrapper
now delegates to the same owner. The app project links that source only when
`OpenVisionLabEnableEmbeddedSmokeRunner` is true.

The owner keeps no mutable state. It receives the already-resolved UI config
directory, snapshots only the two stable files per invocation, returns the
callback result unchanged, rethrows callback exceptions after best-effort
restore, deletes files that were absent before the callback, and preserves the
existing clear/copy behavior. No WPF, Shell, Recipe, Layer, Preview/Run, or
public binding contract changed.

`SmokeDockingStateFilesContract` passed 9/9 in x64 Debug and Release. The
OVL-22 command-line target contract passed 10/10 in both configurations. The
Pipeline smoke builds completed with 0 errors and the existing nullable
`CS8600` warning only; the embedded app Debug and Release builds completed
with 0 warnings and 0 errors. Actual Dev revalidation also passed the
docking-state contract 9/9 in both configurations, the OVL-22 command-line
contract 10/10 in both configurations, `Invoke-RefactorAudit.ps1 -Verify`,
and `TestDocumentationIndex.ps1` (`IndexedPaths=258`, `Routes=13`,
`RootRedirects=102`). The durable owner map is section 9.31 of
`docs/admin/CODEBASE_STRUCTURE.md`; the detailed report is
`docs/reports/OPENVISIONLAB_OVL39_SMOKE_DOCKING_STATE_FILES_20260909.md`.

Junior developer self-assessment: **PASS for this boundary**. The shortest
route is `Program` target -> existing Direct wrapper (when applicable) ->
`SmokeDockingStateFiles` -> contract. The WPF callback and window ownership
remain at the scenario, so a reader does not need to inspect UI lifecycle code
to understand file cleanup.

Dev checkpoint `7a804859` (`refactor(smoke): centralize docking state file
lifecycle`) is local to the isolated Dev worktree and has not been pushed.
Original remains untouched. Do not let another model, agent, or scheduled run
recreate, rename, re-split, or move this owner without a newly reproduced
defect, changed explicit contract, or demonstrated responsibility/dependency
conflict.

OVL-40 completed the former residual screenshot-output pointer. Follow the
current OVL-40 next priority; do not recreate the completed docking or PNG
owners.

## Current Slice — OVL-38 Smoke fixture resources owner

Status: Complete for one independently verifiable WPF-free fixture/resource
boundary; the overall refactoring program remains active.

The 14 synthetic Bitmap, template, representative-image, streaming-hash, and
best-effort temporary-file helpers were moved from
`tools/PipelineViewerScreenshotSmoke/Program.cs` to the internal concrete
`SmokeFixtureResources` owner. `Program` keeps target composition, WPF capture
timing, and OpenGL diagnostics; persisted docking file cleanup now belongs to
`SmokeDockingStateFiles`. A static import preserves every existing call name,
while the owner keeps no mutable fields;
callers retain Bitmap disposal and temporary-file cleanup in their existing
scopes.

The WPF-free `SmokeFixtureResourcesContract` proves the owner import, old
helper removal, dependency boundary, expected fixture dimensions, stable and
variation-sensitive hashes, template crops, workspace image output, and
AutoMPoint representative-file count. The implementation token hash after the
visibility-only `private` to `internal` change is
`71962931406069898943373989cbcf87986c47d8bf2db75f64305e8b9132af04`.

x64 Debug and Release builds completed with 0 errors and the existing nullable
`CS8600` warning only. The new contract passed 9/9 in both configurations, and
the OVL-22 command-line contract passed 10/10 in both configurations.
`Invoke-RefactorAudit.ps1 -Verify` passed with `CSharpFiles=817`,
`XamlFiles=60`, `PartialDeclarations=110`, `ViewModelUiIoFiles=1`,
`ProjectCycles=0`, and `ShellStorageCalls=0`. The durable owner map is section
9.30 of `docs/admin/CODEBASE_STRUCTURE.md`; the report is
`docs/reports/OPENVISIONLAB_OVL38_SMOKE_FIXTURE_RESOURCES_20260909.md`.

Junior developer self-assessment: **PASS for this boundary**. A developer can
follow `Program` target composition -> `SmokeFixtureResources` -> existing
caller disposal/cleanup -> contract without reading WPF lifecycle code.

Dev checkpoint `4d8d4c07` (`refactor(smoke): extract fixture resources`) is
local to the isolated Dev worktree and has not been pushed. Original remains
untouched. Do not let another model, agent, or scheduled run recreate, rename,
re-split, or move this owner without a newly reproduced fixture defect, changed
explicit contract, or demonstrated responsibility/dependency conflict.

The former `WithDockingStateFileBackup` next pointer was completed by OVL-39
above. Follow the current OVL-39 next priority; do not recreate this boundary.

## Current Slice — OVL-37 Template image extraction namespace boundary

Status: Complete for one independently verifiable compatibility-safe
namespace/project boundary; the overall refactoring program remains active.

`TemplateImageExtraction` moved from the broad `OpenVisionLab` namespace under
`src/OpenVisionLab/Common` to the existing `OpenVisionLab.Property` boundary at
`src/OpenVisionLab/Property/TemplateImageExtraction.cs`. The owner remains
internal and static. `PropertyGridImageEditorService`,
`OpenGlTemplateEditorWindow`, and the two existing template smoke callers now
import the responsibility namespace explicitly. The implementation tokens are
unchanged apart from the namespace declaration, and Recipe/XML, explicit
Preview/Run, Layer/ImageSpace, public API, reflection, serializer, and XAML
contracts remain unchanged.

The namespace boundary contract passed 12/12 in Debug and Release. The app,
LocatorRelativeBlobSkillSmoke, PipelineViewerScreenshotSmoke, and
VisionRecipeRunnerSmoke builds passed in Debug and Release with zero warnings
and errors. `Invoke-RefactorAudit.ps1 -Verify` passed with
`CSharpFiles=815`, `XamlFiles=60`, `PartialDeclarations=110`,
`ViewModelUiIoFiles=1`, `ProjectCycles=0`, and `ShellStorageCalls=0`.

Dev commit `13d3a0557def81ef7e0b037969993e9bb85f5739` was pushed to
`origin/codex/public-sample-ux-docs` and the remote SHA matches. Original
`main` remains at `604c7fb6fc5247198afd666b3e3f37241ac069ba`; the Dev commit is
not in Original and the repositories have no directly usable common commit
object for ordinary fast-forward/cherry-pick preparation. Original's
pre-existing `Temp.txt` remains untouched. No Original commit or push was
performed because its current `2.2.0-dev`/`2.1.0` version-source mismatch and
missing exact promotion source/version/allowlist still block a policy-compliant
versioned change set.

Do not let another model, agent, or scheduled run recreate, rename, re-split,
or move this owner without a newly reproduced defect, changed explicit
contract, or demonstrated responsibility/dependency conflict. Do not perform a
bulk namespace rename.

Detailed report:
`docs/reports/OPENVISIONLAB_OVL37_TEMPLATE_IMAGE_EXTRACTION_NAMESPACE_20260909.md`.

Next priority: inspect one responsibility-based smoke-runner cleanup candidate
after confirming that it does not overlap dirty user work | Recommended model:
`gpt-5.4-mini` | Reasoning effort: `medium`.

## Current Slice — OVL-36 PropertyGrid generic metadata adapter

Status: Complete for one independently verifiable generic PropertyGrid metadata
responsibility boundary; the overall refactoring program remains active.

`WpfPropertyGridAdapter.cs` no longer declares the generic category/property
comparers, dynamic type-description provider/descriptor, localized descriptor,
and localization helper. Those seven concrete types now live in
`src/Libraries/WpfPropertyGridBridge/PropertyGridMetadataAdapters.cs`.
`PropertyGrid` remains the lifecycle/state owner for the vendor control,
selected object, hidden-property registry, progressive viewport, navigation,
editor registration, and event forwarding. `PropertyGridToolPolicy` and the
OVL-20 value-change subscription owner were reused and were not reopened.
Recipe/XML, explicit Preview/Run, Layer/ImageSpace, and public PropertyGrid
contracts remain unchanged.

The metadata token contract is unchanged:
`fcd108d34b7a6a24204e3f9dc6ac60378206d7e7e6a09c49faad482713d56c0f` with 10,271
non-whitespace characters. `PropertyGridMetadataAdapterContract` passed 7/7
in Debug and Release, and the existing OVL-20 subscription contract passed 8/8
in Debug and Release. Both app and both smoke-runner configurations built with
zero errors; the PipelineViewerScreenshotSmoke build retains its one
pre-existing nullable warning at `Program.cs:10139`.

`Invoke-RefactorAudit.ps1 -Verify` passed with
`CSharpFiles=815`, `XamlFiles=60`, `PartialDeclarations=110`,
`ViewModelUiIoFiles=1`, `ProjectCycles=0`, and `ShellStorageCalls=0`.
The focused `wpf_property_grid_matching_combo` EXE target passed in Debug and
Release. The Release run dynamically selected the smaller left monitor
`\\.\DISPLAY2` (bounds `-1920,365,1920x1080`, working area
`-1920,365,1920x1032`) and observed window rectangle
`-1900,385,-1140,905`. The screenshot is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl36-ui-property-grid-release2-20260909\wpf_property_grid_matching_combo.png`.
Alternate themes, Wide/Compact layouts, and 100/125/150/175/200% DPI rows
remain environment-bound and unverified.

Junior developer self-assessment: **PASS for this boundary**. The generic
metadata path is now one named file, while mutable grid state stays in
`PropertyGrid`, making the call path easy to trace without adding an interface,
factory, wrapper, message bus, or partial type.

Do not let another model, agent, or scheduled run recreate, rename, re-split, or
move this metadata owner, `PropertyGridToolPolicy`, or the OVL-20 subscription
owner without a newly reproduced defect, changed explicit contract, or proven
responsibility/dependency conflict. Do not split `WpfPropertyGridAdapter` by
file size alone. The detailed report is
`docs/reports/OPENVISIONLAB_OVL36_PROPERTYGRID_METADATA_ADAPTER_20260909.md`.

Selective Dev checkpoint and Original promotion remain separate. The Dev slice
was implemented in `[2.1.0]` commit `638c8a36146a10152e8de475f35b214c9caa819d`,
with push evidence recorded in `22d216fa10686a96f6d397b2267cbb13147ed3da`.
A follow-up docs-only checkpoint `b1827bb5` is the current tip of
`origin/codex/public-sample-ux-docs`; the remote SHA matches. The
pre-existing dirty worktree remains preserved. Original `main` is currently
`2.2.0-dev` and lacks the Dev OVL history; an exact promotion source, version
progression, and changed-file allowlist must be established before modifying
or pushing Original. The versioned-push preparation and blocker record is kept
in this report.

Next priority: inspect one compatibility-safe residual namespace/project
boundary candidate, then continue the responsibility-based smoke-runner cleanup
only if that candidate has no safe migration path | Recommended model:
`gpt-6-astra` | Reasoning effort: `high`.

## Current Slice — OVL-35 Shell Recipe Validation Suite view boundary

Status: Complete for one independently verifiable Shell XAML presentation
boundary; the overall refactoring program remains active.

The Shell `PipelineRunHistory` tab now composes
`OpenVisionRecipeValidationSuiteView`. The extracted View owns the Validation
Suite heading, scope controls, explicit Run/Stop presentation, evidence board,
local validation-set editor, image rows, and summary. `OpenVisionShellHostView`
no longer owns that 30-AutomationId block. The existing `RecipeCommands`
bindings, inherited Shell `DataContext`, explicit Preview/Run commands,
Recipe/XML, Layer/ImageSpace, PropertyGrid, and validation state remain in
existing owners. The new code-behind is presentation-only; a local built-in
`BooleanToVisibilityConverter` preserves the former Shell resource scope.

`RecipeValidationSuiteViewContract` passed 4/4 in Debug and Release with all
30 Validation AutomationIds covered. The
whitespace-stripped moved block hash is unchanged (`c121eab1b491f1595528941b420bfb4c45cbf2e78a8c72d66812ce7410214fbe`, 15,538
non-whitespace characters), both XAML files parse, and focused OVL-31/32/33/34
regression contracts passed in both configurations. App builds passed with
zero warnings/errors; smoke builds passed with zero errors and the existing
single `CS8600` warning.

The post-change audit passed with `CSharpFiles=813`, `XamlFiles=60`,
`PartialDeclarations=110`, `ViewModelUiIoFiles=1`, `ProjectCycles=0`, and
`ShellStorageCalls=0`.

The focused EXE target
`wpf_shell_host_recipe_local_validation_dataset` passed in Debug and Release.
Fresh captures are under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl35-runtime-validation-suite-view-debug5-20260909`
and `ovl35-runtime-validation-suite-view-release-20260909`. The one reported
monitor was `\\.\DISPLAY2` (bounds `1920x1080`, working area `1920x1032`);
observed `1600x900` windows intersected it. Alternate themes, Wide/Compact
layouts, and 100/125/150/175/200% DPI rows remain unverified.

Junior developer self-assessment: **PASS for this boundary**. The call path is
`Shell -> PipelineRunHistory TabItem -> OpenVisionRecipeValidationSuiteView ->
inherited DataContext -> RecipeCommands`; command/state policy stays in the
existing CommandSurface.

Do not let another model, agent, or scheduled run recreate, rename, re-split,
or move command/state policy into this View without a newly reproduced UI
defect, changed explicit contract, or demonstrated responsibility/dependency
conflict. Do not split it by file size alone. The detailed report is
`docs/reports/OPENVISIONLAB_OVL35_RECIPE_VALIDATION_SUITE_VIEW_20260909.md`.

Selective Dev checkpoints `[2.1.0]` `890ad48b` and `0830cdb4` were pushed to
`origin/codex/public-sample-ux-docs`; existing dirty changes were preserved.
`Original`, tags, release publication, and deployment remain unchanged.

Next priority: inspect the generic internals of `WpfPropertyGridAdapter` while
reusing the completed `PropertyGridToolPolicy` and OVL-20 subscription owner;
reopen them only with current defect, explicit contract change, or demonstrated
responsibility conflict | Recommended model: `gpt-5.6-terra` | Reasoning effort:
`high`.

## Current Slice — OVL-34 Validation dataset selected-run drawing evidence owner

Status: Complete for one independently verifiable selected-run drawing-evidence
UI composition boundary; the overall refactoring program remains active.

`ValidationDatasetDrawingEvidence` now owns persisted selected-Run evidence
resolution, SHA-256 source verification, the two-row PinArrayGap drawing
contract, synthetic executed-failure drawing persistence coverage, selector
probing, saved-report artifact copying, floating evidence-window lookup, and
Layer/Preview/route/active-layer side-effect guards. `Program` retains only the
`openDrawingEvidence` decision and composes the existing Recipe configuration,
source path, artifact directory, and UI pump. Recipe/XML, explicit Preview/Run,
Layer/ImageSpace, PropertyGrid, and product UI contracts are unchanged.

The focused `ValidationDatasetDrawingEvidenceContract` passed 7/7 in Debug and
Release. OVL-22 through OVL-33 regression contracts passed in both
configurations. OVL-32/33 contract readers changed only their end marker to the
next stable method after the moved helper; covered behavior is unchanged.
`PipelineViewerScreenshotSmoke` Debug/Release builds completed with zero errors
and retained the existing single `CS8600` nullable warning. The post-change
audit passed with `CSharpFiles=811`, `XamlFiles=59`, `PartialDeclarations=108`,
`ProjectCycles=0`, and `ShellStorageCalls=0`.

The focused EXE target
`wpf_shell_host_recipe_local_validation_drawing_evidence` passed in Debug and
Release on the single reported monitor `\\.\DISPLAY2` (bounds `1920x1080`,
working area `1920x1032`). Fresh captures are under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl34-runtime-drawing-evidence-debug2-20260909`
and `ovl34-runtime-drawing-evidence-release2-20260909`; both are `1420x760` and
show source/drawing images, selector, status, and unchanged read-only workspace
execution state. Alternate themes/layouts and 100/125/150/175/200% DPI rows
remain unverified.

Junior developer self-assessment: **PASS for this boundary**. The call path is
`local validation target -> Program -> ValidationDatasetDrawingEvidence ->
existing Shell RecipeCommands/OpenVisionRecipeRunEvidence/viewer`; the owner
has one explicit evidence responsibility and no new interface/factory/wrapper.

Do not let another model, agent, or scheduled run recreate, rename, or
re-split this drawing-evidence owner, or move OVL-27/30/31/32/33 behavior into
it, without a newly reproduced defect, changed explicit smoke contract, or
demonstrated responsibility/dependency conflict. Do not split it by file size
alone. The detailed report is
`docs/reports/OPENVISIONLAB_OVL34_VALIDATION_DATASET_DRAWING_EVIDENCE_OWNER_20260909.md`.

Selective Dev checkpoint `[2.1.0]`
`fc0b2b8d29a94eac737d8fa9ac316390559cacfd` was pushed to
`origin/codex/public-sample-ux-docs`; local and remote SHA matched. Existing
dirty changes were preserved. `Original`, tags, release publication, and
deployment remain unchanged.

Next priority: inspect one Shell XAML vertical slice with a concrete
presentation owner and focused runtime proof | Recommended model:
`gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-32 Validation dataset execution progress owner

Status: Complete for one independently verifiable local validation dataset
execution progress boundary; the overall refactoring program remains active.

`ValidationDatasetExecutionProgress` now owns the progress-file initialization,
two-second status checkpoints, UI pump callback, saved Run History completion
poll, and existing ten-minute deadline used after `RunValidationSuiteCommand`.
The owner is WPF-free and receives command/status/time behavior through explicit
callbacks. `Program` retains validation-set creation and image registration,
summary loading, artifact output, Run History review, and drawing evidence.
Recipe/XML, Preview/Run, Layer/ImageSpace, PropertyGrid, and product UI
contracts are unchanged.

The focused `ValidationDatasetExecutionProgressContract` passed 7/7 in Debug
and Release. It proves the target call path, discoverable contract usage,
removal of the old loop from the target method, explicit callback ownership,
WPF/Shell independence, early completion, and deadline-bounded progress
retention. OVL-22 through OVL-31 contracts also passed in both configurations.
PipelineViewerScreenshotSmoke Debug/Release builds completed with zero errors
and retained the existing single nullable warning in `Program.cs`.

Evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl32-validation-dataset-execution-progress-contract-debug3-20260909`,
`ovl32-validation-dataset-execution-progress-contract-release3-20260909`, and
the `ovl32-regression-*` directories. The detailed report is
`docs/reports/OPENVISIONLAB_OVL32_VALIDATION_DATASET_EXECUTION_PROGRESS_OWNER_20260909.md`.

Do not let another model, agent, or scheduled run recreate, rename, or
re-split this execution-progress owner, or move OVL-27/30 artifact output or
OVL-31 configuration, without a newly reproduced execution-lifetime defect,
changed explicit smoke contract, or demonstrated responsibility/dependency
conflict. Do not split summary/UI review by file size alone.

Selective Dev checkpoint `[2.1.0]` `bc64b6bd`
(`bc64b6bdfad004bf6940abaa4195dfb0956a2f63`) was pushed to
`origin/codex/public-sample-ux-docs`; local and remote SHA match. Existing
dirty changes were preserved. `Original`, tags, release publication, and
deployment remain unchanged.

Next priority: inspect one remaining validation dataset summary/UI composition
boundary and complete only one independently verifiable owner |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-33 Validation dataset review-queue evidence owner

Status: Complete for one independently verifiable local validation dataset
review-queue evidence boundary; the overall refactoring program remains active.

`ValidationDatasetReviewQueueEvidence` now owns the persisted review-queue
Pitch metric/SHA identity checks, `ShowRecentBatchReviewQueueOnly` state
transition, Preview/Layer/route side-effect guards, review-queue panel
visibility, saved summary copy, and `review_queue_contract.txt` projection.
`Program` keeps target composition and the existing visual-tree lookup, while
OVL-30 artifact output and OVL-32 execution progress remain separate. No
Recipe/XML, Preview/Run, Layer/ImageSpace, PropertyGrid, or product UI
contract changed.

The focused `ValidationDatasetReviewQueueEvidenceContract` passed 6/6 in
Debug and Release, including exact persisted identity/count/invariant output.
OVL-22 through OVL-32 contracts also passed in both configurations.
PipelineViewerScreenshotSmoke Debug/Release builds completed with zero errors
and retained the existing single nullable warning; the refactor audit passed
with `CSharpFiles=809`, `XamlFiles=59`, `PartialDeclarations=108`,
`ProjectCycles=0`, and `ShellStorageCalls=0`.

Evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl33-validation-dataset-review-queue-evidence-contract-debug-20260909`,
`ovl33-validation-dataset-review-queue-evidence-contract-release-20260909`,
and the `ovl33-regression-*` directories. The detailed report is
`docs/reports/OPENVISIONLAB_OVL33_VALIDATION_DATASET_REVIEW_QUEUE_EVIDENCE_OWNER_20260909.md`.

Do not let another model, agent, or scheduled run recreate, rename, or
re-split this review-queue owner, or move OVL-27/30 artifact output or
OVL-31/32 owners, without a newly reproduced review-queue defect, changed
explicit smoke contract, or demonstrated responsibility/dependency conflict.
Do not split drawing evidence by file size alone.

Selective Dev checkpoint `[2.1.0]` `cb41ce0a`
(`cb41ce0aa6c79f36b6b138019a1a2e07a5964ccc`) was pushed to
`origin/codex/public-sample-ux-docs`; local and remote SHA match. Existing
dirty changes were preserved. `Original`, tags, release publication, and
deployment remain unchanged.

Next priority: inspect one remaining validation dataset selected-run
drawing-evidence UI composition boundary and complete only one independently
verifiable owner | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-20 PropertyGrid value-change subscription owner

Status: Complete for the single generic `WpfPropertyGridAdapter` event
subscription lifetime boundary.

`PropertyGridPropertyValueChangeSubscription` now owns the WPG
`ValueChanged`/`PropertyChanged` handlers and last-value snapshot. The adapter
detaches before both selected-object replacement paths, attaches the current
property collection afterward, and keeps the existing `RaisePropertyValueChanged`
projection. PropertyGrid attributes, selected-object binding, PropertyGridToolPolicy,
Recipe/XML, Preview/Run, and Layer/ImageSpace contracts are unchanged.

The focused structural contract passed 8/8 in Debug and Release. Bridge and
VisionRecipeRunnerSmoke Debug/Release builds passed with zero warnings and
errors. The matching PropertyGrid Combo/range-editor desktop smoke passed in
Debug and Release; the captured window intersected the dynamically recorded
one-monitor topology. The current capture matched the closest reproducible
OVL-08 post-policy capture at all 24,700 sampled pixels.

Evidence is recorded under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl20-property-grid-contract-20260909`
and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl20-property-grid-runtime-20260909`.
The detailed report is
`docs/reports/OPENVISIONLAB_OVL20_PROPERTYGRID_VALUE_CHANGE_SUBSCRIPTION_20260909.md`.

Do not let another model or agent recreate this owner, move the same handlers,
or split the adapter without a newly reproduced lifetime defect, a changed
explicit contract, or a proven responsibility conflict. The existing
PropertyGridToolPolicy extraction is closed and must not be reworked here.
Alternate themes/layouts, hover/pressed/focus/disabled/read-only/validation
states, and 100/125/150/175/200% DPI remain unverified environment-bound rows.

Next priority: compatibility-safe namespace/project boundary review |
Recommended model: `gpt-6-astra` | Reasoning effort: `high`.

## Current Slice — OVL-21 compatibility-safe namespace/project boundary

Status: Complete for one internal namespace migration with an unchanged public
Recipe XML contract.

The namespace/project inventory selected `ParameterPropertyStorage` as the
smallest safe candidate. It is `internal static`, has one production caller,
and has no XAML, reflection, serializer-root, or external assembly signal. The
storage owner now lives in `OpenVisionLab.Property`, matching its
`src/OpenVisionLab/Property/` responsibility folder. The public
`OpenVisionLab.ParameterProperty` type remains in its legacy namespace with
`CPropertyParam`, and the Recipe XML compatibility map remains unchanged.

The focused `NamespaceProjectBoundaryContract` passed 8/8 in Debug and Release.
`VisionRecipeRunnerSmoke` and `RecipeXmlCompatibilityCheck` Debug/Release builds
passed with zero warnings and errors; the XML check passed for 13 roots in both
configurations. The refactor audit passed with 787 C# files, 59 XAML files, 108
partial declarations, 27 projects, zero project cycles, and zero Shell Run
History storage calls. Evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl21-namespace-project-boundary-debug-run2-20260909`,
`ovl21-namespace-project-boundary-release-run2-20260909`,
`ovl21-recipe-xml-debug-20260909`,
`ovl21-recipe-xml-release-20260909`, and
`ovl21-refactor-audit-20260909`. The detailed report is
`docs/reports/OPENVISIONLAB_OVL21_NAMESPACE_PROJECT_BOUNDARY_20260909.md`.
Dev checkpoint: `5ea0a9b6f6012f397403a2b722a784b8350c905e` was pushed to `origin/codex/public-sample-ux-docs`; local and remote SHA match.

Do not let another model or agent repeat this migration, move the public
property types, or split the application project without a new defect, changed
explicit contract, compatibility inventory, and focused build proof. The
remaining root-namespace public types and project boundary stay separate work.

Next priority: responsibility-based smoke-runner cleanup after product
boundaries are closed | Recommended model: `gpt-5.4-mini` | Reasoning effort:
`medium`.

## Current Slice — OVL-31 Validation dataset configuration owner

Status: Complete for one independently verifiable smoke-runner configuration
boundary.

`ValidationDatasetSmokeConfiguration` now owns the local validation dataset
target's environment input, OK/NG folder resolution, per-role limit, default
Matching baseline paths, pipeline-name/suite/boundary defaults, and baseline XML
substitution. `Program` calls `LoadFromEnvironment` and retains Recipe storage,
explicit validation-set create/add/run, progress, summary loading, artifact
output, Run History review, and drawing-evidence UI checks. The configuration
owner is WPF-free and does not retain state across targets.

`ValidationDatasetSmokeConfigurationContract` passed 7/7 in Debug and Release.
It verifies delegation and old preparation removal, nested `all_images` folder
selection, caller-supplied pipeline preservation, maximum-per-role clamping,
default baseline XML/path substitution, and fail-closed missing-dataset input.
OVL-22 through OVL-30 contracts reran successfully in both configurations.
PipelineViewerScreenshotSmoke Debug and Release builds had zero errors and
retained only the existing `CS8600` warning at `Program.cs:10395`. The
post-slice refactor audit passed with `CSharpFiles=805`, `XamlFiles=59`,
`PartialDeclarations=108`, `ProjectCycles=0`, and `ShellStorageCalls=0`.

Evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl31-validation-dataset-configuration-contract-debug-20260909`,
`ovl31-validation-dataset-configuration-contract-release-20260909`, the
`ovl31-regression-*` directories, and
`ovl31-refactor-audit-20260909`. The detailed report is
`docs/reports/OPENVISIONLAB_OVL31_VALIDATION_DATASET_CONFIGURATION_OWNER_20260909.md`.

Do not let another model, agent, or scheduled run recreate, rename, or re-split
this configuration owner, or move OVL-27/30 artifact output, without a newly
reproduced configuration defect, changed explicit smoke contract, or proven
responsibility conflict. Target-specific validation execution and summary/UI
composition remain separate and must not be split by file size.

Dev checkpoint: selective `[2.1.0]` Dev commit `fd759948`
(`fd759948f21d6593202206dab6c5d2bba3ef4eaf`) was pushed to
`origin/codex/public-sample-ux-docs`. The pre-existing dirty worktree was
preserved. Original, tags, release publication, and deployment remain
unchanged.
The detailed OVL-31 report was recorded in follow-up Dev commit `f12aa50a`
(`f12aa50a3643afd617d160943d844431729784aa`) on the same branch.

Next priority: inspect one remaining validation dataset execution or summary/UI
owner | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-30 Validation dataset summary artifact owner

Status: Complete for one independently verifiable validation dataset summary
artifact boundary.

The existing `ValidationDatasetArtifactWriter` now owns the remaining local
validation dataset outputs: `pipeline.xml`, `batch_summary.json`,
`audit_summary.json`, and the four expected/actual judgment counts. Its unified
method preserves the former write order by composing the existing CSV and
misclassification evidence writers between the batch and audit outputs.
`Program` retains dataset environment resolution, Recipe setup, validation-set
execution, progress pumping, and Run History/UI review assertions.

`ValidationDatasetArtifactWriterContract` passed 9/9 in Debug and Release,
including exact XML persistence, batch JSON identity, all four audit judgments,
and source ownership/removal checks. OVL-22 through OVL-29 contracts reran
successfully in both configurations. PipelineViewerScreenshotSmoke Debug and
Release builds had zero errors and retained only the existing `CS8600` warning
at `Program.cs:10470`. The post-slice refactor audit passed with
`CSharpFiles=803`, `XamlFiles=59`, `PartialDeclarations=108`,
`ProjectCycles=0`, and `ShellStorageCalls=0`.

Evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl30-validation-dataset-artifact-writer-contract-20260909-debug2`,
`ovl30-validation-dataset-artifact-writer-contract-20260909-release2`, the
`ovl30b-regression-*` directories, and `ovl30b-refactor-audit-20260909`. The
detailed report is
`docs/reports/OPENVISIONLAB_OVL30_VALIDATION_DATASET_SUMMARY_ARTIFACT_OWNER_20260909.md`.

Do not let another model, agent, or scheduled run recreate or re-split the
OVL-27/30 artifact owner without a newly reproduced artifact-schema defect,
changed explicit contract, or proven responsibility conflict. Dataset
execution, progress, and UI review remain separate responsibilities.

Dev checkpoint: selective `[2.1.0]` Dev commit `bd200129`
(`bd200129879916a8fd23cf4c8a5064694ec15b2b`) was pushed to
`origin/codex/public-sample-ux-docs`. The pre-existing dirty worktree was
preserved. Original, tags, release publication, and deployment remain
unchanged.

Next priority: inspect one remaining smoke fixture execution or summary/UI
owner | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-29 Smoke Recipe workspace cleanup owner

Status: Complete for one independently verifiable smoke-runner workspace
cleanup policy boundary.

`SmokeRecipeWorkspaceCleanup` now owns the former
`CleanupTransientRecipeWorkspaces` policy from
`tools/PipelineViewerScreenshotSmoke/Program.cs`. Its pure selector preserves
the existing case-insensitive keep set, always protects `Default`, and selects
only `Smoke_`/`Recipe_` names. `DeleteTransient` keeps
`RecipeWorkspaceService` as the existing lookup/deletion boundary. `Program`
retains target ordering and the one optional keep-name call while delegating all
21 cleanup calls; the old private helper is removed.

`SmokeRecipeWorkspaceCleanupContract` passed 6/6 in Debug and Release. It
checks delegation, old-helper removal, WPF-free ownership, keep/Default
protection, prefix filtering, ordering, and unrelated-name exclusion. The
previous OVL-22 through OVL-28 contracts reran successfully in both
configurations. PipelineViewerScreenshotSmoke Debug and Release builds had zero
errors and retained only the existing `CS8600` warning at `Program.cs:10497`.
The post-slice refactor audit passed with `CSharpFiles=803`, `XamlFiles=59`,
`PartialDeclarations=108`, `ProjectCycles=0`, and `ShellStorageCalls=0`.

Evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl29-smoke-recipe-workspace-cleanup-contract-20260909-run2`,
`ovl29-smoke-recipe-workspace-cleanup-contract-20260909-release`, the
`ovl29-regression-*` directories, and `ovl29-refactor-audit-20260909`. The
detailed report is
`docs/reports/OPENVISIONLAB_OVL29_SMOKE_RECIPE_WORKSPACE_CLEANUP_OWNER_20260909.md`.

Do not let another model, agent, or scheduled run recreate or re-split this
owner without a newly reproduced workspace-lifecycle defect, changed explicit
smoke contract, or proven responsibility conflict. OVL-28's Recipe fixture
owner and the target-specific fixture/execution/summary/UI composition remain
separate and must not be split by file size.

Dev checkpoint: selective `[2.1.0]` Dev commit
`e81f903e` (`e81f903e106488ac834b59eb300f2d626139ccc8`) was pushed to
`origin/codex/public-sample-ux-docs`. The pre-existing dirty worktree was
preserved. A one-line evidence-index correction followed as commit `8c28d171`
(`8c28d1715901c35f15132be651e3b5708c7f970b`) and was pushed to the same Dev
branch. Original, tags, release publication, and deployment remain unchanged.

Next priority: inspect one remaining smoke fixture execution or summary/UI
owner | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-28 Recipe context fixture owner

Status: Complete for one independently verifiable deterministic smoke-fixture construction boundary.

`RecipeContextFixture` now owns the repeated `CreateRecipeContextSmokePipeline`
construction previously held by `PipelineViewerScreenshotSmoke/Program.cs`. Its
single `CreatePipeline` operation preserves the named Threshold steps, `Main` first
input, linked preview inputs/outputs, and zero/negative count behavior. `Program`
keeps fixture selection, specialized parameter edits, Recipe XML persistence,
Preview/Run, Shell creation, and UI assertions. Recipe/XML, Preview/Run,
Layer/ImageSpace, PropertyGrid, and product UI behavior are unchanged.

`RecipeContextFixtureContract` passed 6/6 in Debug and Release. It checks the
26-call-site delegation, old helper removal, WPF-free owner, linked layer sequence,
empty-count behavior, and Recipe XML round-trip. OVL-27/26/25/24/23/22 contracts
reran successfully in both configurations. Debug and Release builds had zero errors
and retained only the existing `CS8600` warning at `Program.cs:10512`. The post-slice
refactor audit passed with `CSharpFiles=801`, `XamlFiles=59`, `PartialDeclarations=108`,
`ProjectCycles=0`, and `ShellStorageCalls=0`.

Evidence is under `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl28-recipe-context-fixture-debug-rerun-20260909`, `ovl28-recipe-context-fixture-release-contract-20260909`, and `ovl28-refactor-audit-20260909`. The detailed report is `docs/reports/OPENVISIONLAB_OVL28_RECIPE_CONTEXT_FIXTURE_OWNER_20260909.md`.

Do not let another model, agent, or scheduled run recreate or re-split this owner
without a newly reproduced fixture-shape/XML defect, changed smoke contract, or
proven responsibility conflict. Remaining fixture execution, summary, and UI
orchestration helpers are separate and must not be split by file size.

Dev checkpoint: selective `[2.1.0]` Dev commit `86441033`
(`86441033886a61a04b4720b6de43625e8436f78e`) was pushed to
`origin/codex/public-sample-ux-docs` after documentation-index verification. The
pre-existing dirty worktree was preserved. Original, tags, release publication, and
deployment remain unchanged.

Next priority: inspect the remaining smoke fixture execution or summary/UI groups for one concrete state owner | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-27 validation dataset artifact writer owner

Status: Complete for one independently verifiable validation dataset reporting boundary.

`ValidationDatasetArtifactWriter` now owns the local validation dataset target's
`misclassification_table.csv` projection and misclassification evidence output:
report metric projection, report-relative drawing lookup, original/drawing/run-report
copying, false-accept/false-reject manifest rows, filename sanitization, CSV escaping,
and evidence README output. `Program` keeps dataset configuration, recipe/pipeline
fixture creation, validation-suite execution/progress, summary/audit JSON, UI review
queue checks, and `IsDatasetJudgment` as the composition boundary. Recipe/XML,
Preview/Run, Layer/ImageSpace, PropertyGrid, and product UI behavior are unchanged.

`ValidationDatasetArtifactWriterContract` passed 7/7 in Debug and Release using a
synthetic persisted Run Report, metric CSV output, copied source/drawing/report files,
and false-reject manifest evidence. OVL-26/25/24/23/22 contracts reran successfully
in both configurations. Debug and Release builds had zero errors and retained only
the existing `CS8600` warning at `Program.cs:10523`. The post-slice refactor audit
passed with `CSharpFiles=799`, `XamlFiles=59`, `PartialDeclarations=108`,
`ProjectCycles=0`, and `ShellStorageCalls=0`.

Evidence is under `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl27-validation-dataset-artifact-writer-contract-rerun-20260909`, `ovl27-validation-dataset-artifact-writer-release-contract-20260909`, and `ovl27-refactor-audit-20260909`. The detailed report is `docs/reports/OPENVISIONLAB_OVL27_VALIDATION_DATASET_ARTIFACT_WRITER_20260909.md`.

Do not let another model, agent, or scheduled run recreate or re-split this owner
without a newly reproduced artifact/report defect, changed output contract, or
proven responsibility conflict. Remaining fixture, execution, summary JSON, and UI
review helpers are separate and must not be split by file size.

Dev checkpoint: selective `[2.1.0]` Dev commit `5140f233`
(`5140f23372c2edb9d20b0d2bf40dc5160412487c`) was pushed to
`origin/codex/public-sample-ux-docs` after documentation-index verification. The
pre-existing dirty worktree was preserved. Original, tags, release publication, and
deployment remain unchanged.

Next priority: inspect the remaining smoke fixture/execution or summary groups for one concrete state owner | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-26 screenshot capture lifecycle owner

Status: Complete for one independently verifiable temporary-window capture lifecycle boundary.

`ScreenshotCaptureLifecycle` now owns the existing `CaptureWindowWithContent`,
`CaptureStandaloneWindow`, and `CaptureElement` workflow implementation. It creates,
shows, activates, selects, captures, and closes temporary/supplied Windows; preserves
pump and verification ordering; forwards OpenGL diagnostics through an explicit
callback; disposes disposable content in the existing `finally` order; and returns the
existing `CaptureResult`. `Program` keeps the target catalog, shared `Pump`, diagnostic
implementation, visual assertions, fixture creation, and capture options as the
composition boundary. `ScreenshotPngWriter` remains the rendering/output owner.
Recipe/XML, Preview/Run, Layer/ImageSpace, PropertyGrid, and product UI behavior are
unchanged.

`ScreenshotCaptureLifecycleContract` passed 10/10 in Debug and Release. It exercises
real WPF temporary and standalone Windows, verification and pump order, PNG output,
diagnostic roots, disposable content, deterministic close, and direct element capture.
OVL-25/24/23/22 contracts reran successfully in both configurations. Debug and Release
builds had zero errors and retained only the existing `CS8600` warning at
`Program.cs:10724`. The post-slice refactor audit passed with `CSharpFiles=797`,
`XamlFiles=59`, `PartialDeclarations=108`, `ProjectCycles=0`, and
`ShellStorageCalls=0`.

Evidence is under `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl26-screenshot-capture-lifecycle-contract-retry-20260909`, `ovl26-screenshot-capture-lifecycle-release-contract-20260909`, and `ovl26-refactor-audit-20260909`. The detailed report is `docs/reports/OPENVISIONLAB_OVL26_SCREENSHOT_CAPTURE_LIFECYCLE_20260909.md`.

Do not let another model, agent, or scheduled run recreate or re-split this owner
without a newly reproduced cleanup/lifecycle defect, changed capture contract, or
proven responsibility conflict. Remaining fixture and reporting groups are separate
and must not be split by file size.

Dev checkpoint: selective `[2.1.0]` Dev commit `0dfe5661`
(`0dfe56614ee0c1efbf2cf85dc652d992d9f650f0`) was pushed to
`origin/codex/public-sample-ux-docs` after documentation index verification. The
pre-existing dirty worktree was preserved. Original, tags, release publication, and
deployment remain unchanged.

Next priority: inspect the remaining smoke fixture/reporting groups for one concrete state owner | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-25 screenshot PNG writer owner

Status: Complete for one independently verifiable WPF screenshot output boundary.

`ScreenshotPngWriter` now owns the three existing PNG output operations: screen capture through `PointToScreen`/`CopyFromScreen`, arranged `FrameworkElement` rendering through `RenderTargetBitmap`, and visible-element rendering. `Program` and the existing Learn smoke modules call this owner, while window creation/show/activate/close, floating-tool selection, dispatcher pumping, verification callbacks, OpenGL diagnostics, fixture bitmap creation, and capture timing remain with their existing owners. Recipe/XML, Preview/Run, Layer/ImageSpace, PropertyGrid, and product UI behavior are unchanged.

The owner has no dependency on `Program`, OpenVision application modules, or product state. `ScreenshotPngWriterContract` passed 7/7 in Debug and Release using real WPF `FrameworkElement` renders and concrete `48x32` PNG outputs. OVL-24 bitmap evidence, OVL-23 Learn document policy, and OVL-22 target-runner contracts reran successfully in both configurations. Debug and Release builds had zero errors and retained only the existing `CS8600` warning at `Program.cs:10718`. The post-slice refactor audit passed with `CSharpFiles=795`, `XamlFiles=59`, `PartialDeclarations=108`, `ProjectCycles=0`, and `ShellStorageCalls=0`.

Evidence is under `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl25-screenshot-png-writer-contract-20260909`, `ovl25-screenshot-png-writer-release-contract-20260909`, and `ovl25-refactor-audit-20260909`. The detailed report is `docs/reports/OPENVISIONLAB_OVL25_SCREENSHOT_PNG_WRITER_20260909.md`.

Do not let another model, agent, or scheduled run recreate or re-split this owner without a newly reproduced capture-format defect, changed rendering contract, or proven responsibility conflict. The remaining window lifecycle and fixture/reporting groups are separate; do not split them by file size.

Dev checkpoint: selective `[2.1.0]` Dev commit `e89d6977f77aa5f567ec79d142a148bcc4846add` was pushed to `origin/codex/public-sample-ux-docs`; the index was clean at the checkpoint and the pre-existing dirty worktree was preserved. Original, tags, release publication, and deployment remain unchanged.

Next priority: inspect `CaptureWindowWithContent`, `CaptureStandaloneWindow`, and `CaptureElement` for one explicit temporary-window state/cleanup owner | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-24 screenshot bitmap assertion owner

Status: Complete for one independently verifiable bitmap evidence boundary in `PipelineViewerScreenshotSmoke`.

`ScreenshotBitmapAssertions` now owns the eight existing bitmap presence, diagnostic-save, image-difference, preview-overlay, source-background, binary-like, grayscale, and expected-color assertions. `Program` keeps WPF window lifecycle, RenderTargetBitmap/CopyFromScreen capture, OpenGL diagnostics, fixture bitmap creation, and the decision of when a Preview/Run image is captured. The extracted owner has no WPF/OpenVision application dependency; Recipe/XML, Preview/Run, Layer/ImageSpace, and PropertyGrid behavior remain unchanged.

`ScreenshotBitmapAssertionsContract` passed 8/8 in Debug and Release without creating a WPF window. It verifies the owner call path, removal of the old methods, preserved sampling/threshold/error behavior, invalid and identical image rejection, changed/overlay/source-background/binary/grayscale/color acceptance, and diagnostic PNG output. OVL-23 Learn policy reruns passed 7/7 and OVL-22 target-runner reruns passed 10/10 in Debug and Release. Debug and Release builds had zero errors and retained only the existing `CS8600` warning at `Program.cs:10712`.

The post-slice `Invoke-RefactorAudit.ps1 -Verify` passed with `CSharpFiles=793`, `XamlFiles=59`, `PartialDeclarations=108`, `ProjectCycles=0`, and `ShellStorageCalls=0`. Evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl24-screenshot-bitmap-assertions-contract-20260909`,
`ovl24-screenshot-bitmap-assertions-release-contract-20260909`, and
`ovl24-refactor-audit-20260909`. The detailed report is
`docs/reports/OPENVISIONLAB_OVL24_SCREENSHOT_BITMAP_ASSERTIONS_20260909.md`.

Do not let another model, agent, or scheduled run recreate or re-split this owner without a newly reproduced bitmap-evidence defect, changed threshold contract, or proven responsibility conflict. Remaining WPF capture lifecycle and fixture groups are separate and must not be split by line count alone.

Dev checkpoint: this slice is prepared for a selective `[2.1.0]` Dev commit and push after documentation index checks. Original, tags, release publication, and deployment remain unchanged.

Next priority: inspect the WPF capture lifecycle helpers for one explicit state/cleanup owner, or record an audit if the dependencies cannot be passed without a speculative wrapper | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.
## Current Slice — OVL-23 Learn document copy policy owner

Status: Complete for one independently verifiable Learn documentation and visible-copy policy boundary.

`LearnDocumentationCopyPolicy` now owns the forbidden internal-engineering phrase list, case-insensitive matching, document-file checks, visible-copy checks, and the existing context-bearing error messages. `PipelineViewerScreenshotSmoke/Program.cs` remains responsible for WPF window creation, topic/document resolution, and visual-tree traversal; its `CollectVisibleLearnCopy` helper supplies strings to the policy. Recipe/XML, Preview/Run, Layer/ImageSpace, PropertyGrid, and capture target behavior are unchanged.

`LearnDocumentationCopyPolicyContract` passed 7/7 in Debug and Release without creating a WPF window. It verifies the new owner and Program call path, removal of the old phrase list/assertion methods, clean and forbidden document behavior, visible-copy behavior, and ordinal-ignore-case matching. The existing OVL-22 target-runner contract was rerun and passed 10/10 in Debug and Release. Debug and Release builds had zero errors and retained only the pre-existing `CS8600` warning at `Program.cs:10706`. The post-slice `Invoke-RefactorAudit.ps1 -Verify` passed with `CSharpFiles=791`, `XamlFiles=59`, `PartialDeclarations=108`, `ProjectCycles=0`, and `ShellStorageCalls=0`; `TestDocumentationIndex.ps1` also passed with `IndexedPaths=242` and `Routes=13`.

Evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl23-learn-document-copy-policy-contract-20260909`,
`ovl23-learn-document-copy-policy-release-contract-20260909`,
`ovl22-screenshot-smoke-target-runner-contract-20260909-rerun`, and
`ovl22-screenshot-smoke-target-runner-release-contract-20260909-rerun`. The detailed report is
`docs/reports/OPENVISIONLAB_OVL23_LEARN_DOCUMENT_COPY_POLICY_20260909.md`.

Do not let another model, agent, or scheduled run recreate or re-split this policy without a newly reproduced content-policy defect, changed explicit contract, or proven responsibility conflict. The related readiness-tool phrase checks remain a separate executable boundary and were intentionally not merged. Do not split `Program.cs` by line count or move its WPF traversal into this policy without a concrete ownership need.

Dev checkpoint: this slice is prepared for a selective `[2.1.0]` Dev commit and push after final audit/index checks. Original, tags, release publication, and deployment remain unchanged.

Next priority: inspect the remaining `PipelineViewerScreenshotSmoke/Program.cs` fixture/capture/reporting groups and select at most one independent owner | Recommended model: `gpt-5.4-mini` | Reasoning effort: `medium`.
## Current Slice — OVL-22 smoke runner target owner

Status: Complete for one command-line target execution boundary in
`PipelineViewerScreenshotSmoke`.

The command-line target selection, suite expansion, sorted catalog output, capture
result formatting, exception evidence, and exit-status policy now live in
`tools/PipelineViewerScreenshotSmoke/ScreenshotSmokeTargetRunner.cs`. `Program`
passes the existing target and suite catalogs and remains the owner of every WPF
capture delegate. The target catalog, `--all`, `--target`, `--suite`, `--list`,
manual-language parsing, Recipe/XML, Preview/Run, Layer/ImageSpace, and
PropertyGrid behavior remain unchanged.

`ScreenshotSmokeTargetRunnerContract` is window-free and passed 10/10 in both
Debug and Release. It verifies the new owner and Program call path plus success,
unknown-target, exception/error-file, trim, suite-order/deduplication, and sorted
catalog behavior. The focused report is
`docs/reports/OPENVISIONLAB_OVL22_SMOKE_RUNNER_TARGET_OWNER_20260909.md`.
Evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl22-screenshot-smoke-target-runner-contract-20260909`,
`ovl22-screenshot-smoke-target-runner-release-contract-20260909`, and
`ovl22-screenshot-smoke-list-debug-20260909.txt`. Debug and Release builds had
zero errors and retained only the pre-existing `CS8600` warning at
`Program.cs:10726`.

The post-slice `Invoke-RefactorAudit.ps1 -Verify` also passed with
`CSharpFiles=789`, `XamlFiles=59`, `PartialDeclarations=108`,
`ProjectCycles=0`, and `ShellStorageCalls=0`; evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl22-refactor-audit-20260909`.

Dev checkpoint: `a80a37b0f5a9b5e14e061718078022ed862a4472` was pushed to
`origin/codex/public-sample-ux-docs`; local and remote SHA match.

Do not let another model, agent, or scheduled run recreate or re-split this
runner without a newly reproduced command contract, lifecycle defect, or proven
responsibility conflict. Do not split the remaining WPF capture groups by line
count alone. A future smoke slice must identify one independent fixture,
capture, or reporting owner and prove it with a focused contract; if no boundary
is demonstrated, record the audit without changing code.

Next priority: inspect the remaining `PipelineViewerScreenshotSmoke/Program.cs`
fixture/capture/reporting groups and select at most one independent owner |
Recommended model: `gpt-5.4-mini` | Reasoning effort: `medium`.
## Current Slice — OVL-19 Shell Recipe basic lifecycle view

Status: Complete for the single Shell XAML basic lifecycle view boundary.

The Recipe Manager name editor and basic Create/Duplicate/Rename/Delete strip
now lives in `Recipe/Views/OpenVisionRecipeBasicLifecycleView.xaml`. The Shell
keeps the advanced-review namescope trigger, while the existing
`OpenVisionShellHostRecipeCommandSurface` remains the state and command owner.
Bindings, AutomationIds, Recipe/XML, Preview/Run, Layer/ImageSpace, and
PropertyGrid contracts are unchanged.

The focused structural contract passed 7/7 in Debug and Release. OpenVisionLab
and VisionRecipeRunnerSmoke Debug/Release builds passed with zero warnings and
errors. The `recipe-manager-tabs` desktop run reached the existing summary
AutomationId assertions and captured the basic strip after the nested-resource
fix. Its broad result retained the pre-existing benchmark-baseline failure at
`OpenVisionLabDirectSmokeRunner.cs:4802`; a separate Pipeline roundtrip probe
also retained the existing Pipeline Review-open failure at line 3461. Neither
failure is attributed to this presentation-only slice.

Evidence and scope limits are recorded in
`docs/reports/OPENVISIONLAB_OVL19_SHELL_RECIPE_BASIC_LIFECYCLE_VIEW_20260909.md`.

Do not let another model or agent recreate this UserControl, move the same CRUD
bindings again, or split the same Shell strip without a newly reproduced defect,
a changed explicit contract, or a proven responsibility conflict. Alternate
themes, layouts, hover/pressed/focus/disabled states, popups, and 125/150/175/
200% DPI remain unverified environment-bound rows.

Next priority: `WpfPropertyGridAdapter` generic internals |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-17 Shell validation evidence owner

Status: Complete for the single Shell Validation Set acceptance/calibration
evidence projection boundary.

`Recipe/CommandSurface/Handlers.cs` no longer loads the
selected Pipeline XML or formats acceptance and `PIXELPERMM` calibration
policy. `OpenVisionRecipeValidationEvidenceOwner` owns that read/policy path
and returns an immutable result; the Shell keeps the existing binding names,
status channels, and PropertyChanged notifications. Preview, Run, Layer, and
Recipe/XML contracts are unchanged.

The focused owner contract passed 4/4 in Debug and Release. OpenVisionLab and
VisionRecipeRunnerSmoke Debug/Release builds passed with zero warnings and
errors. Evidence is recorded under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\shell-validation-evidence-owner-20260909-run2`
and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\shell-validation-evidence-owner-20260909-release`.

Report:
`docs/reports/OPENVISIONLAB_OVL17_SHELL_VALIDATION_EVIDENCE_OWNER_20260909.md`.

Do not let another model or agent repeat OVL-17, recreate its contract, or
split this acceptance/calibration path again without a newly reproduced
defect, a changed explicit contract, or a proven responsibility conflict.
OVL-18 below closes the separate preview navigation portion of Shell Step
Edit; residual tool-menu mapping and orchestration remain separate. OVL-19
above closes one independent Shell XAML vertical slice; the remaining advanced
Recipe Manager markup requires its own runtime target before any extraction.

## Current Slice — OVL-18 Shell Step preview navigation owner

Status: Complete for the single preview-list matching, adjacent navigation,
and Step identity boundary.

`OpenVisionRecipeStepPreviewNavigationOwner` owns the former Shell
`StepMatches`, numeric/reference normalization, previous/next list lookup, and
same-Step comparison rules. The Shell passes its current preview list and
selection through thin facades; the existing Step Edit loader, apply owner,
dirty transition, and tool launch flow remain unchanged.

The focused owner contract passed 5/5 in Debug and Release. Existing Step Edit
loader, apply-owner, and apply-projection contracts passed in Debug. OpenVisionLab
and VisionRecipeRunnerSmoke Debug/Release builds passed with zero warnings and
errors. Evidence is recorded under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\step-preview-navigation-owner-debug-20260909-run2`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\step-preview-navigation-owner-release-20260909-run2`,
and the three `ovl18-regression-step-edit-*` directories.

Report:
`docs/reports/OPENVISIONLAB_OVL18_SHELL_STEP_PREVIEW_NAVIGATION_OWNER_20260909.md`.

Do not let another model or agent repeat OVL-18, recreate its contract, or
split the same preview matching/navigation policy without a newly reproduced
defect, a changed explicit contract, or a proven responsibility conflict.
The next independent slice is the generic `WpfPropertyGridAdapter` internals
boundary after the existing `PropertyGridToolPolicy` owner is confirmed.

Next priority: `WpfPropertyGridAdapter` generic internals |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-16 Local validation-set execution owner

Status: Complete for the single Local validation-set input snapshot and
execution boundary.

The existing `OpenVisionRecipeValidationSetRunner` now also owns Local set
name/notes and image-record snapshot creation plus Pipeline path resolution
through `CreateRunRequest`. Its existing `RunAsync` remains the owner of XML
loading, sequential sample checks, expected-outcome mapping, stop/partial-save
behavior, and batch persistence. The session VM retains running flags,
localized status projection, and command-state/batch-save events.

The copied-runtime Recipe execution contract passed 12/12 in both Debug and
Release, including Local metadata and expected outcomes, one-image partial
stop, missing-XML recovery for all workflows, pair/Catalog regression, and
byte-for-byte Recipe XML preservation. OpenVisionLab and VisionRecipeRunnerSmoke
Debug/Release builds passed with zero warnings and errors. Static ownership
proof, the module audit, documentation index, JSON parse, and `git diff
--check` passed.
Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-local-validation-owner-20260908`.

Dev checkpoint: commit `0f3ad509744cceb3394cba100801c6dfb1dc6ef6` was pushed to
`origin/codex/public-sample-ux-docs` (`https://github.com/Noah8218/OpenVisionLab_Dev.git`).
The commit contains only the OVL-12~16 owner source/report allowlist; the
pre-existing dirty files remain unstaged and uncommitted.

Report: `docs/reports/OPENVISIONLAB_OVL16_RECIPE_LOCAL_VALIDATION_EXECUTION_OWNER_20260908.md`.
No XAML, Recipe/XML schema, Preview/Run command contract, Layer/ImageSpace
route, or PropertyGrid policy changed. Alternate theme and DPI UI rows remain
environment-bound and unverified for this source-only slice.

Do not let another model or agent repeat OVL-12 through OVL-16, recreate their
contracts, or split the same paths again without a newly reproduced defect,
changed contract, or proven responsibility conflict. The next independent
slice is residual Shell CommandSurface validation/evidence or Step Edit.

Next priority: Shell CommandSurface validation/evidence or Step Edit ownership |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-15 Catalog benchmark execution owner

Status: Complete for the single Product catalog benchmark execution, progress,
and batch-storage boundary.

The current audit found that the Recipe session still filtered and sorted
Product samples, read Pipeline XML, executed the catalog, projected 10/N
progress, converted results, and saved the Catalog batch.
`OpenVisionRecipeCatalogExecutionOwner` now owns those operations. The session
VM retains empty-catalog handling, running flags, localized status/error
projection, `LatestCatalogBenchmarkSummary`, and command-state/batch-save
events. The owner has no WPF, Window, Shell, or session dependency.

The copied-runtime Recipe execution contract passed 12/12 in both Debug and
Release, including Catalog group/name ordering, progress at 10/12 and 12/12,
report paths, missing-XML recovery, pair/local regression, and byte-for-byte
Recipe XML preservation. OpenVisionLab and VisionRecipeRunnerSmoke
Debug/Release builds passed with zero warnings and errors. Static ownership
proof, the module audit, documentation index, and `git diff --check` passed.
Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-catalog-execution-owner-20260908`.

Report: `docs/reports/OPENVISIONLAB_OVL15_RECIPE_CATALOG_EXECUTION_OWNER_20260908.md`.
No XAML, Recipe/XML schema, Preview/Run command contract, Layer/ImageSpace
route, or PropertyGrid policy changed. Alternate theme and DPI UI rows remain
environment-bound and unverified for this source-only slice.

Do not let another model or agent repeat OVL-12 through OVL-15, recreate their
contracts, or split the same paths again without a newly reproduced defect,
changed contract, or proven responsibility conflict. Local-set execution was
subsequently closed by OVL-16; Shell CommandSurface validation/evidence or
Step Edit is the next separate slice.

Next priority: Shell CommandSurface validation/evidence or Step Edit ownership |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-14 Good/Bad pair execution owner

Status: Complete for the single Good/Bad pair execution and batch-storage
boundary.

The current audit found that the Recipe session still read Pipeline XML,
executed each Good/Bad sample, converted results, and saved the `GoodBadPair`
batch. `OpenVisionRecipePairExecutionOwner` now owns those operations and the
existing pair sample ordering. The session VM retains running flags, localized
status/error projection, `LatestPairRunSummary`, and command-state/batch-save
events. The owner has no WPF, Window, Shell, or session dependency.

The copied-runtime Recipe execution contract passed 12/12 in both Debug and
Release, including Good-before-Bad ordering, expected rejection handling,
distinct run reports, Catalog/local regression, missing-XML recovery, partial
stop, and byte-for-byte Recipe XML preservation. OpenVisionLab and
VisionRecipeRunnerSmoke Debug/Release builds passed with zero warnings and
errors. Static ownership proof, the module audit, documentation index, and
`git diff --check` passed. Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-pair-execution-owner-20260908`.

Report: `docs/reports/OPENVISIONLAB_OVL14_RECIPE_PAIR_EXECUTION_OWNER_20260908.md`.
No XAML, Recipe/XML schema, Preview/Run command contract, Layer/ImageSpace
route, or PropertyGrid policy changed. Alternate theme and DPI UI rows remain
environment-bound and unverified for this source-only slice.

Do not let another model or agent repeat OVL-12, OVL-13, or this pair owner,
recreate their contracts, or split the same paths again without a newly
reproduced defect, changed contract, or proven responsibility conflict. Catalog
and Local paths were subsequently closed by OVL-15 and OVL-16; Shell
CommandSurface validation/evidence or Step Edit is the next separate slice.

Next priority: Shell CommandSurface validation/evidence or Step Edit ownership |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-13 selected-sample execution owner

Status: Complete for the single selected-sample execution boundary.

The earlier Recipe session extraction left a concrete residual: the session VM
still read Pipeline XML, invoked the selected-sample service, and saved the
selected-sample suite batch. `OpenVisionRecipeSelectedSampleExecutionOwner`
now owns those two operation paths. The session VM retains running flags,
localized status/error projection, binding summaries, command-state events, and
the unchanged public async methods. The owner has no WPF, Window, Shell, or
session dependency.

The copied-runtime Recipe execution contract passed 12/12 in both Debug and
Release, including selected-sample check/suite, pair/catalog/local regression,
missing-XML recovery, partial stop, and byte-for-byte Recipe XML preservation.
OpenVisionLab and VisionRecipeRunnerSmoke Debug/Release builds passed with zero
warnings and errors. Static ownership search, the module audit, documentation
index, and `git diff --check` passed. Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-selected-sample-owner-20260908`.

Report: `docs/reports/OPENVISIONLAB_OVL13_RECIPE_SELECTED_SAMPLE_EXECUTION_OWNER_20260908.md`.
No XAML, Recipe/XML schema, Preview/Run command contract, Layer/ImageSpace
route, or PropertyGrid policy changed. Alternate theme and DPI UI rows remain
environment-bound and unverified for this source-only slice.

Do not let another model or agent repeat this owner, recreate its contract, or
split the selected-sample path again without a newly reproduced defect, changed
contract, or proven responsibility conflict. The pair, Catalog, and Local paths
were subsequently closed by OVL-14, OVL-15, and OVL-16; Shell CommandSurface
validation/evidence or Step Edit is the next separate slice.

Next priority: Shell CommandSurface validation/evidence or Step Edit ownership |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-12 ImageCompare resource owner

Status: Complete for the single Image Compare decoded-image resource lifetime
boundary.

`ImageCompareImageResource` now owns file decoding, the `System.Drawing.Bitmap`,
the frozen WPF `BitmapSource`, format metadata, and idempotent disposal.
`ImageCompareSlotViewModel` keeps the existing public `Bitmap`, `Source`, size,
load, and dispose facade while owning only binding state and resource
projection. Replacement disposes the previous resource first, and invalid
paths still leave the slot empty.

The focused VisionRecipeRunnerSmoke Release build and resource contract passed.
The existing PipelineViewerScreenshotSmoke Release build passed with one
existing nullable warning and its `wpf_image_compare` target returned `OK`.
Dynamic monitor evidence selected the smaller left `DISPLAY2` work area
(`-1920,365–0,1397`) and verified the actual Image Compare window rectangle
intersected it. The post-change module audit, static ownership search, and
`git diff --check` passed.

Report: `docs/reports/OPENVISIONLAB_OVL12_IMAGECOMPARE_RESOURCE_OWNER_20260908.md`.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\image-compare-refactor-20260908`.
The full alternate-theme and 125/150/175/200% DPI matrix remains
environment-bound and unverified for this source/resource-only slice. The
selected-sample portion of the Recipe execution pointer was subsequently
closed by OVL-13 above.

Do not let another model or agent repeat this slice, recreate its contract, or
split the resource owner again without a newly reproduced defect, changed
contract, or proven responsibility conflict. Keep one independently verifiable
slice per heartbeat and preserve the dirty worktree.

The Recipe execution-session pointer has now been split into selected-sample,
pair, Catalog, and Local validation-set owners above. The next remaining
execution slice is Shell CommandSurface validation/evidence or Step Edit |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — 2026-09-08 full module-audit recheck

Status: Complete (read-only source inventory, manual hotspot review, junior-
readability assessment, and ordered refactor plan).

The fresh `Invoke-RefactorAudit.ps1 -Verify` result is
`REFACTOR_AUDIT=PASS|CSharpFiles=773|XamlFiles=58|PartialDeclarations=106|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
Additional D: evidence records 1,321 type declarations, 9,804 method-like
declarations, 13 multi-file partial types, namespace/path distribution, project
inventory, and manually classified ViewModel IO. No source behavior changed in
this slice; only the audit report and continuation records were updated.

The current junior-readability verdict is **partially modular**. Core/Recipe,
ImageCanvas owners, and Learn topic policy are navigable. Good/Bad, Catalog, and
local-set execution paths, Shell CommandSurface/XAML, and the flat app
namespace/project remain the next structural boundaries; ImageCompare and the
selected-sample execution owner are closed under the OVL-12/OVL-13 records above.

Do not let another model or agent repeat this audit, recreate this report, or
split a completed owner without a newly reproduced defect, changed contract, or
proven responsibility conflict. Keep one independently verifiable slice per
heartbeat and preserve the current dirty worktree.

## Current Slice — OVL-06b quantitative audit instrumentation

Status: Complete (one independently verifiable read-only instrumentation slice).

`tools/RefactorAudit/Invoke-RefactorAudit.ps1` now owns the repeatable scan for
source size, partial/type declarations, large files, ViewModel UI/IO signals,
Shell Run History storage calls, and ProjectReference cycles. `-Verify` passed
with 763 C# files, 58 XAML files, 106 partial declarations, two ViewModel
UI/dialog files, eight direct IO files, zero Shell storage calls, and zero
project cycles. A C: output path was rejected with exit code 1 without creating
the path. The product, Recipe/XML contracts, and completed owners were not
changed.

Report: `docs/reports/OPENVISIONLAB_OVL06B_QUANTITATIVE_AUDIT_20260908.md`.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl06b-quantitative-audit-20260908`.
Next priority: OVL-11 ImageCanvas ViewModel UI/IO boundary | Recommended model:
`gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-11 ImageCanvas save owner

Status: Complete (one independently verifiable ImageCanvas persistence boundary).

`CanvasImageSaver` now owns extension fallback, directory creation, save callback
dispatch, empty-Mat rejection, and `Cv2.ImWrite`. The existing public
`RoiImageCanvasViewModel.SaveCurrentImage(string)` remains the compatibility
facade; current Mat and callback lifetime remain ViewModel-owned. ImageCanvas
Debug/Release builds, the existing external consumer Release build, and the
existing `wpf_imagecanvas_owned_mat_load` target passed. The focused target
returned `OK 1 / WARN 0 / NG 0` in both configurations, and the post-change
quantitative audit returned PASS.

Report: `docs/reports/OPENVISIONLAB_OVL11_IMAGECANVAS_SAVE_OWNER_20260908.md`.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-image-save-owner-20260908`.
The next slice must move `OpenFileDialog` or `ContextMenu` ownership to an
explicit View/dialog host; do not reopen this completed save owner or add a
partial-only split.
Next priority: OVL-11 dialog/ContextMenu boundary | Recommended model:
`gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-11 SaveFileDialog host

Status: Complete for the single SaveFileDialog ownership boundary.

`RoiImageCanvasDialogHost` now owns the existing SaveFileDialog options, modal
re-entry guard, and path/null result conversion. `RoiImageCanvasView` connects
the host while its ViewModel is attached and clears it on detach/dispose.
`RoiImageCanvasViewModel.Commands.cs` retains filename/initial-directory policy,
the existing `SaveCurrentImage` call, and last-directory update without a
concrete SaveFileDialog reference.

ImageCanvas and OpenVisionLab app x64 Debug/Release builds, the external consumer
Release build, and the focused ImageCanvas/Shell prechecks passed in both
configurations. The changed source was also included in a post-change
quantitative audit (`766` C# files, `268,339` lines, `106` partial declarations,
project cycles `0`).

Report: `docs/reports/OPENVISIONLAB_OVL11_IMAGECANVAS_SAVE_DIALOG_HOST_20260908.md`.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-save-dialog-host-20260908`.
The full Windows SaveFileDialog visual/theme/DPI interaction matrix remains
unverified. The next slice is OpenFileDialog ownership; ContextMenu and input
events remain later independent boundaries. Do not reopen the completed Mat or
SaveFileDialog owner and do not add a partial-only split.
Next priority: OVL-11 OpenFileDialog host ownership |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-11 OpenFileDialog host

Status: Complete for the single OpenFileDialog ownership boundary.

`RoiImageCanvasDialogHost` now owns the existing OpenFileDialog filter,
initial-directory handoff, modal re-entry guard, and path/null result
conversion. `RoiImageCanvasView` connects the shared ImageDialogHost while its
ViewModel is attached and clears it on detach/dispose. The ViewModel retains
Mat loading, logging, last-directory state, and the public command contract.

ImageCanvas Debug/Release, OpenVisionLab app x64 Debug/Release, and the
external consumer Release builds passed with zero warnings and zero errors.
The focused ImageCanvas/Shell prechecks returned `OK 2` in both configurations;
the post-change audit recorded `766` C# files, `268,358` lines, `106` partial
declarations, and project cycles `0`.

Report: `docs/reports/OPENVISIONLAB_OVL11_IMAGECANVAS_OPEN_DIALOG_HOST_20260908.md`.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-open-dialog-host-20260908`.
The full Windows OpenFileDialog visual/theme/layout/DPI matrix remains
unverified. The next slice is ContextMenu/input ownership; do not reopen the
completed Mat or Open/Save dialog owners and do not add a partial-only split.

Junior readability check for this slice: **PASS**. A new contributor can follow
`LoadImageCommand -> OpenLoadImage -> ImageDialogHost.ShowOpenImageDialog ->
CanvasImageLoader.LoadMatFromFile -> LoadImage` and see that ViewModel state
stays in the ViewModel while modal UI stays in the host. The remaining
ContextMenu/input coupling is explicitly listed as the next boundary.

Next priority: OVL-11 keyboard/mouse input ownership |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-11 ImageCanvas ContextMenu host

Status: Complete for the single ContextMenu ownership boundary.

`IImageCanvasContextMenuHost` is the narrow internal contract. The existing
`RoiImageCanvasView` implements it and owns `MainGrid.ContextMenu` and its
`IsOpen` operation. `RoiImageCanvasViewModel` retains the right-click mode-reset
policy and invokes `ContextMenuHost.OpenContextMenu()` without a concrete WPF
ContextMenu reference. The existing XAML `ContextMenu`, `MenuItems` binding,
menu item commands/parameters, icons, and DataContext wiring are unchanged.

The View assigns `ContextMenuHost` during attach and clears it during DataContext
detach and `Dispose`, alongside the existing `ImageDialogHost` lifecycle. The
right-click path remains `ImageCanvasControl -> OnMouseRightClick ->
ExecuteRightClickCommand -> ContextMenuHost.OpenContextMenu ->
RoiImageCanvasView.MainGrid.ContextMenu.IsOpen`; active Measure/Teaching/
AddRoiArray modes are still cleared before opening the menu.

ImageCanvas Debug/Release, the external consumer Release build, and
OpenVisionLab x64 Debug/Release builds passed with zero warnings and zero
errors. The focused ImageCanvas/Shell precheck returned `OK 2 / WARN 0 / NG 0`
in both configurations. The post-change audit recorded 767 C# files,
268,374 C# lines, 12,221,410 C# bytes, one direct ViewModel UI/dialog signal,
and zero project cycles. Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-context-menu-host-20260908`.

Junior readability check for this slice: **PASS**. A new contributor can follow
`right-click -> mode policy -> ContextMenuHost -> View.MainGrid.ContextMenu`
and can find the visual menu and `MenuItems` binding in the existing XAML. The
remaining Directory coupling was visible as the next owner boundary; the mouse
owner is recorded in the later current slice rather than hidden in another
partial or wrapper.

`소스 코드 기준 검토 완료 / 실제 Runtime ContextMenu UI 검증 필요`. Full
keyboard/mouse/focus/pressed/selected/disabled/popup/theme/layout/DPI and
monitor interaction remains unverified. Do not reopen the completed Mat,
OpenFileDialog, SaveFileDialog, or ContextMenu owners without a new defect,
changed requirement, or proven responsibility conflict. The mouse owner is
recorded in the later current slice; the next single slice is OVL-09.

Report: `docs/reports/OPENVISIONLAB_OVL11_IMAGECANVAS_CONTEXT_MENU_HOST_20260908.md`.
Report: `docs/reports/OPENVISIONLAB_REFACTOR_PROGRAM_AUDIT_20260908.md`.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-audit-20260908`.

## Current Slice — OVL-11 ImageCanvas WinForms keyboard input owner

Status: Complete for the single WinForms keyboard input ownership boundary.

RoiImageCanvasKeyboardInputController now owns the existing
ImageCanvasControl.KeyDown subscription and the Ctrl+Z/Ctrl+Y/Ctrl+Shift+Z,
Delete, Ctrl+C, and Ctrl+V policy. It reuses RoiInteractionKeyDown for
rectangle copy/paste and calls explicit ViewModel callbacks for selected/copy ROI
state, snapshot publication, overlay callbacks, and Undo/Redo. The ViewModel no
longer contains a WinForms key handler or subscribes to KeyDown/KeyUp.
ImageCanvasControl public key events and OpenGL forwarding are unchanged.

Controller disposal runs before the ViewModel releases or disposes
ImageCanvasControl; repeated disposal removes the same handler only once.
WPF PreviewKeyDown/KeyUp policy is now owned by the separate WPF keyboard
controller; mouse input is recorded in the later dedicated owner slice.

The temporary D: keyboard contract harness passed
KEYBOARD_INPUT_CONTRACT=PASS for Ctrl+Z, Ctrl+Y, Ctrl+Shift+Z, Delete, Ctrl+C,
and post-dispose unsubscription. ImageCanvas Debug/Release, external consumer
Release, and OpenVisionLab x64 Debug/Release builds passed with zero warnings and
zero errors. The focused ImageCanvas/Shell UI precheck returned OK for both
targets in Debug and Release. The post-change audit returned
CSharpFiles=768, CSharpLines=268447, CSharpBytes=12224124,
PartialDeclarations=106, DirectUiOrDialogFiles=1, and ProjectCycles=0.

Junior readability check for this slice: PASS. The path
ImageCanvasControl.KeyDown -> RoiImageCanvasKeyboardInputController ->
ViewModel callbacks makes event policy, mutable ROI state, and WinForms
forwarding distinct. The full desktop keyboard focus/pressed/disabled/
theme/layout/DPI/monitor matrix was not executed, so this is source and focused
contract evidence only.

Do not reopen the completed Mat, OpenFileDialog, SaveFileDialog, ContextMenu, or
WinForms keyboard owners without a new defect, changed requirement, or proven
responsibility conflict. The WPF PreviewKeyDown/KeyUp slice named here is
completed in the following section; the mouse owner is recorded below and the
current next slice is OVL-09.

Report: docs/reports/OPENVISIONLAB_OVL11_IMAGECANVAS_KEYBOARD_INPUT_20260908.md.
Evidence:
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-keyboard-input-20260908.

Historical next priority (completed below): OVL-11 WPF PreviewKeyDown/KeyUp input ownership | Recommended model: gpt-5.6-terra | Reasoning effort: high.

## Current Slice — OVL-11 ImageCanvas WPF keyboard input owner

Status: Complete for the single WPF PreviewKeyDown/KeyUp ownership boundary.

`RoiImageCanvasWpfKeyboardInputController` now owns the WPF
`Keyboard.Modifiers` and key policy that previously lived in
`RoiImageCanvasViewModel.Commands.cs`. It preserves the Control guard, Delete
callback/`Handled` behavior, F2/Enter no-op, and Ctrl+C/V/S KeyUp no-op. The
existing public `PreviewKeyDownCommand` and `KeyUpCommand` remain the command
facade used by the View.

`RoiImageCanvasView` still owns PreviewKeyDown/KeyUp subscription,
forwarding, and Dispose unsubscription. The ViewModel owns ROI state and passes
`RemoveSelectedOverlay` as an explicit callback; the WPF controller owns no
View event or mutable ROI state. WinForms keyboard, mouse, dialogs, Mat, and
Recipe/XML boundaries remain separate.

The temporary D: WPF keyboard contract harness returned
`WPF_KEYBOARD_CONTRACT=PASS` for Delete callback/Handled and F2/KeyUp no-op.
ImageCanvas Debug/Release, external consumer Release, and OpenVisionLab x64
Debug/Release builds passed with zero warnings and zero errors. The focused
ImageCanvas/Shell UI precheck returned `OK` for both targets in Debug and
Release. The post-change audit returned `CSharpFiles=769`,
`CSharpLines=268466`, `CSharpBytes=12224956`, `PartialDeclarations=106`,
`DirectUiOrDialogFiles=1`, and `ProjectCycles=0`.

Junior readability check for this slice: **PASS**. A new contributor can follow
`RoiImageCanvasView event -> public command -> WPF keyboard controller ->
ViewModel.RemoveSelectedOverlay`, while View event lifetime, ViewModel state,
and WinForms keyboard policy stay in distinct owners. The physical WPF keyboard
focus/pressed/disabled/theme/layout/DPI/monitor matrix remains unverified.

Do not reopen the completed Mat, OpenFileDialog, SaveFileDialog, ContextMenu,
WinForms keyboard, or WPF keyboard owners without a new defect, changed
requirement, or proven responsibility conflict. The following mouse-input
slice is recorded below; the current next priority is OVL-09.

Report: docs/reports/OPENVISIONLAB_OVL11_IMAGECANVAS_WPF_KEYBOARD_INPUT_20260908.md.
Evidence:
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-wpf-keyboard-input-20260908.

Next priority: OVL-09 Learn Window/Shell residual composition | Recommended model: gpt-5.6-terra | Reasoning effort: high.

## Current Slice — OVL-11 ImageCanvas mouse input owner

Status: Complete for the single ImageCanvas mouse-input ownership boundary.

`RoiImageCanvasMouseInputController` now owns the seven
`ImageCanvasControl` mouse subscriptions and the existing ROI mouse policy:
left draw/edit/move/measure, right-click mode reset/context-menu dispatch,
middle-button pan, wheel zoom, mouse-leave pan reset, cursor selection, and
drawing-timer restart. The controller reuses the existing
`RoiInteractionMouseDown`/`MouseMove`/`MouseUp`/`Cursor` helpers and calls the
ViewModel through explicit state accessors and callbacks.

`RoiImageCanvasViewModel.InitEvent()` and `ReleaseEvents()` now retain only
Load, Resized, and Draw. The ViewModel still owns ROI/measurement state,
snapshot publication, ContextMenu policy, public commands/events, and all
Recipe/XML-compatible behavior. `Dispose()` releases the mouse controller
before disposing `ImageCanvasControl`; repeated controller disposal is safe.
Focus, Drag & Drop, DPI, Window, animation, and rendering remain in the View
and ImageCanvasControl boundary.

The D: mouse contract harness returned `MOUSE_INPUT_CONTRACT=PASS` after
checking all seven subscriptions, release to the original invocation counts,
and idempotent second disposal. ImageCanvas Debug/Release, external consumer
Release, and OpenVisionLab x64 Debug/Release builds passed with zero warnings
and zero errors. The focused ImageCanvas/Shell UI precheck returned `OK 2 /
WARN 0 / NG 0` in both configurations. The post-change audit returned
`REFACTOR_AUDIT=PASS`, `CSharpFiles=770`, `CSharpLines=268637`,
`CSharpBytes=12232911`, `PartialDeclarations=106`, `ViewModelUiIoFiles=1`,
`ProjectCycles=0`, and `ShellStorageCalls=0`.

Junior developer self-assessment for this slice: **PASS**. A new contributor
can follow `ImageCanvasControl mouse event ->
RoiImageCanvasMouseInputController -> existing ROI helper -> explicit
ViewModel state/callback` without searching a large ViewModel for button and
mode policy. The remaining ViewModel `Directory` policy and Learn/Shell shared
composition are visible as separate next boundaries; completed Mat, dialog,
ContextMenu, WinForms keyboard, and WPF keyboard owners are not reopened.

The physical desktop mouse interaction matrix (focus, hover, pressed,
selected, disabled, popup, theme/layout, supported DPI, monitor placement and
drag/pan/measure input) remains unverified. This is source, contract, build,
and focused-precheck evidence only.

Report: `docs/reports/OPENVISIONLAB_OVL11_IMAGECANVAS_MOUSE_INPUT_20260908.md`.
Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-mouse-input-20260908`.

The next single slice is **OVL-09 Learn Window/Shell residual composition** |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-09 Learn Window/Shell host boundary

Status: Complete for the single Learn Window host/lifecycle boundary.

`OpenVisionShellHostLearnWindowController` now owns Learn Window creation,
reuse, owner assignment, callback injection, topic routing, and `Closed`
unsubscription. `OpenVisionShellHostCommandController` keeps the existing
workspace image/sample workflow and is connected through explicit callbacks.
Chrome Learn, Tool Learn, Tool Samples, and Pipeline Review's selected-tool
Learn entry all use the new owner. Existing topic Views/Presenters,
`OpenVisionLearnTopicCatalog`, Recipe/XML, explicit Preview/Run, and
Layer/ImageSpace behavior are unchanged.

The D: WPF Learn host contract returned `LEARN_HOST_CONTRACT=PASS` for topic
routing, existing-window reuse, sample path callback, no implicit Tool action,
`Closed` cleanup/fresh-window creation, and unknown-tool no-op. Structural proof
returned `LEARN_HOST_STRUCTURE_PROOF=True`. OpenVisionLab x64 Debug/Release and
external consumer Release builds passed with zero warnings and zero errors.
The focused Learn/Shell UI precheck returned `OK 2 / WARN 0 / NG 0` in both
configurations. The latest quantitative audit returned
`CSharpFiles=771`, `CSharpLines=268667`, `CSharpBytes=12234466`,
`PartialDeclarations=106`, `ProjectCycles=0`, and `ShellStorageCalls=0`.

Junior developer self-assessment for this slice: **PASS**. A contributor can
follow Shell Chrome or Pipeline Review -> one named Learn Window owner -> topic
catalog/window -> explicit sample or Tool callback. The remaining shared
composition is inside `OpenVisionLearnWindow.xaml.cs` (topic selection, panel
visibility, and document/practice presentation). Physical desktop focus,
pressed, theme/layout, DPI, monitor, and resize qualification remains open;
the current slice has source, contract, build, and focused offscreen UI evidence.

Report: `docs/reports/OPENVISIONLAB_OVL09_LEARN_WINDOW_HOST_20260908.md`.
Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl09-learn-host-20260908`.

The next single slice is **OVL-09 Learn Window topic-selection/presentation composition** |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-09 Learn Window topic-selection/presentation composition

Status: Complete for one independently verifiable topic presentation boundary.

`OpenVisionLearnTopicPresentationPolicy` now owns normalized topic ->
title/subtitle/practice text/panel state/practice-expander state/guide refresh
intent. `OpenVisionLearnWindow.xaml.cs` remains the View owner and applies this
immutable state to existing topic Views and Presenters. Recipe/XML, explicit
Preview/Run, Layer/ImageSpace, SDK, and topic catalog contracts are unchanged.

The D: pure policy contract returned `TOPIC_POLICY_CONTRACT=PASS` and the
all-topic WPF contract returned `TOPIC_WINDOW_CONTRACT=PASS`. OpenVisionLab
Debug/Release and external consumer Release builds passed with 0 warnings and
0 errors. Representative Learn Brightness, Threshold, and Matching UI smoke
passed; Debug/Release Threshold prechecks returned `OK 1 / WARN 0 / NG 0`.
The quantitative audit returned `REFACTOR_AUDIT=PASS` with 772 C# files,
268,718 C# lines, 12,237,951 C# bytes, 58 XAML files, 106 partial
declarations, zero project cycles, and zero Shell storage calls.

Junior developer self-assessment: **PASS for this boundary**. The entry path
is `TopicList_SelectionChanged -> policy.Resolve -> Window applies state ->
existing topic View/Presenter guide owner`. The full physical desktop
focus/hover/pressed/selected/disabled/theme/layout/DPI/monitor/resize matrix
is not run. Existing broader Learn smoke preconditions for curriculum copy,
Layer/Recipe routing safety, Metrics/Acceptance outlier guidance, and Color/HSV
practice guidance remain recorded as unrelated baseline failures; tests were
not weakened.

Report: `docs/reports/OPENVISIONLAB_OVL09_LEARN_TOPIC_PRESENTATION_20260908.md`.
Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl09-topic-composition-20260908`.

The next single slice is **OVL-11 ImageCanvas ViewModel Directory policy owner** (completed below) |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — OVL-11 ImageCanvas Directory policy owner

Status: Complete for one independently verifiable ImageCanvas Directory-policy
boundary.

`ImageCanvasDirectoryPolicy` now owns the shared last-image directory state,
`Sample`/`Samples`/`samples` upward search, and the existing initial-directory
fallback order. `RoiImageCanvasViewModel.Commands.cs` keeps command sequencing,
filename sanitization, dialog-host calls, Mat loading/saving, and explicit
success-only path memory through the policy. The app-level
`OpenVisionImageDirectoryResolver` was not changed because it belongs to a
separate application-layer caller contract.

The D: directory policy cases returned `DIRECTORY_POLICY_CONTRACT=PASS` for
sample search, remembered-directory priority, invalid-path fallback, and
application-base fallback. ImageCanvas Debug/Release, OpenVisionLab x64
Debug/Release, and external consumer Release builds passed with 0 warnings and
0 errors. Baseline ImageCanvas UI precheck passed; after-change Debug/Release
prechecks passed `wpf_imagecanvas_owned_mat_load` and
`wpf_shell_host_tool_input_image_load_save` with `OK 2 / WARN 0 / NG 0`.
Structure proof returned `PolicyStructureProof=True`. The latest audit returned
`REFACTOR_AUDIT=PASS` with 773 C# files, 268,742 C# lines, 12,238,522 C#
bytes, 58 XAML files, 106 partial declarations, zero project cycles, and zero
Shell storage calls.

Junior developer self-assessment: **PASS for this boundary**. The path is
`LoadImageCommand/SaveImageCommand -> ImageCanvasDirectoryPolicy ->
ImageDialogHost -> existing image owner`. Directory traversal and shared path
state now have one named owner; the ViewModel's remaining filename sanitization
is a small command-local policy. The full physical desktop matrix remains
unverified.

Report: `docs/reports/OPENVISIONLAB_OVL11_IMAGECANVAS_DIRECTORY_POLICY_20260908.md`.
Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-directory-policy-20260908`.

The next single slice was **representative WPF runtime qualification** (completed below) |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Current Slice — representative WPF runtime qualification

Status: Complete for the independently verifiable representative 96% DPI
physical desktop slice.

The existing OpenVisionLab Debug and Release binaries were exercised on the
current Windows desktop. The harness dynamically selected the smaller non-primary
left monitor `\\.\DISPLAY2` (bounds `-1920,365,1920x1080`, working area
`-1920,365,1920x1032`), placed the normal window at `-1900,385,1600x900`, and
recorded `GetDpiForWindow=96`. Each configuration returned 17 `PASS` rows with
`FAIL=0`, `WARN=0`; Threshold and Pipeline selected-tool readback, Learn topic
selection, native image-dialog placement/acceptance, loaded workspace status,
maximize/restore, and shutdown all passed. The application exit code was 0 and
no process remained.

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-wpf-runtime-20260908`.
The authoritative files are `debug\\qualification-results.json`,
`release\\qualification-results.json`, `debug\\01-initial.png`,
`debug\\05-learn-window.png`, `debug\\06-learn-topic-selection.png`,
`debug\\07-image-dialog-screen.png`, `debug\\07-image-loaded.png`,
`debug\\08-threshold-tool.png`, `debug\\09-pipeline-view.png`,
`debug\\10-maximized.png`, and the matching Release captures. The run contract,
parser result, script hash, and no-duplicate rule are in `work-contract.md` and
`run-metadata.txt`. Detailed scope and evidence are in
`docs/reports/OPENVISIONLAB_WPF_RUNTIME_QUALIFICATION_20260908.md`.

No product source or tracked sample changed in this slice. The preceding
Directory-policy evidence supplies the Debug/Release source-build proof; this
slice reused those existing binaries.

Junior developer self-assessment: **PASS for this slice**. The evidence path is
`monitor -> app handle -> AutomationId -> physical action -> visible/result
assertion -> shutdown`, and the native dialog helper is isolated in the
D-only harness. The current 96% two-monitor row is closed. DPI 125/150/175/200,
alternate themes, one-monitor, and headless rows are still unverified and are
the only runtime continuation unless a new failure or changed contract appears.

Completed ImageCanvas, Learn, Recipe/Pipeline, and this 96% runtime owner must
not be re-executed, re-split, or re-commented by another model or agent without
a new failing result, changed contract, or an available unverified row.

The next single slice is **alternate DPI/theme/topology runtime coverage or
explicit environment recording** | Recommended model: `gpt-5.5` | Reasoning
effort: `medium`.

## Previous Structural Refactoring Closure — 2026-09-08 (Run History orchestration)

Status: Complete (one independently verifiable Run History persistence and
comparison orchestration slice).

`OpenVisionRecipeRunHistoryOrchestrationOwner` now owns persisted batch-run
inventory lookup, recent/baseline option selection, summary loading, baseline
resolution, comparison-row projection, and default comparison-row selection.
`OpenVisionShellHostRecipeCommandSurface` keeps the existing binding properties,
selection setters, `PropertyChanged` order, command state, and localized
presentation. The existing Run History presenter, Recipe execution/Validation
owners, Recipe/XML contract, and explicit Preview/Run behavior were reused and
not repartitioned.

Evidence: the focused Run History orchestration contract passed 4/4 in Debug
and Release from the isolated D: evidence root. App and Runner x64 Debug and
Release builds passed with 0 warnings and 0 errors. Static structure proof and
`git diff --check` passed. No View/XAML changed, so runtime UI state/theme/DPI
qualification remains unverified for this source-only boundary. Current dirty
work remains on branch `codex/public-sample-ux-docs` / HEAD
`d875559577c85984d54900df973a6fb35fb20146`.

Report: `docs/reports/OPENVISIONLAB_OVL07_RECIPE_RUN_HISTORY_ORCHESTRATION_20260908.md`.
Navigation: `CODEBASE_STRUCTURE` 9.3. Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-run-history-orchestration-20260908`.

The next ordered priority at that closure was OVL-06b full quantitative audit
instrumentation | Recommended model: `gpt-5.6-luna` | Reasoning effort: `low`.
That historical stop boundary is now superseded by the completed current slice
above; OVL-07 completed owners remain closed unless a current defect, changed
explicit requirement, or responsibility conflict is reproduced.

## Previous Structural Refactoring Closure — 2026-09-08 (Pipeline Review Document)

Status: Complete (full-code survey and Pipeline Review Document result
projection revision guard slice).

The current full-code survey measured 761 C# files / 268,055 lines, 58 XAML
files / 24,624 lines, 106 partial declarations, 28 C# types at or above 1,000
lines, and eight at or above 3,000 lines. The survey started at 759 C# files /
267,740 lines before the selected contract and gate files were added. Ten closed or partially closed OVL
candidate boundaries were reconciled with current source; the attached
`OpenVisionLab_Implementation_Tasks_604c7fb.md` remains historical evidence, not
current authority. The product remains an RC/pre-production deterministic
OpenCvSharp4 rule-based vision recipe workbench. The survey and ordered backlog
are in `docs/reports/OPENVISIONLAB_REFACTOR_SURVEY_20260908.md`.

The selected single refactoring slice is the Pipeline Review Document result
projection revision guard. `OpenVisionPipelineReviewDocumentRevisionGate` now
owns document input/recipe/run revisions and disposal invalidation. Document
completion, superseded, exception, StepUpdated and finally projections reject
stale revisions before touching the current View; the existing execution
controller remains the owner of execution generation, cancellation, summaries,
review-cache lifetime and callback atomicity. Recipe/XML, explicit Preview/Run,
Layer routing, ImageSpace ownership and existing completed owners were not
repartitioned.

Evidence: Debug and Release app/Runner builds passed with 0 warnings and 0
errors. The new document revision contract and existing stale-callback/execution
contracts passed in both configurations. Release normal and NG Pipeline Review
WPF targets passed with `OK|check=OK`; the dynamic monitor wrapper recorded the
window on the selected smaller-left monitor. Current dirty work is preserved at
branch `codex/public-sample-ux-docs` / HEAD
`d875559577c85984d54900df973a6fb35fb20146`.

The focused WPF run used the current two-monitor topology at the reported
system scale (96 DPI). Other themes/layouts, 125–200% DPI, pressed/focus/
keyboard matrix and production EXE qualification remain unverified.

Completed Native Tool, TCP identity, runtime log, ImageSpace/Layer, execution
lifetime, PropertyGrid, metric and scoped Learn owners remain closed. Reopening
requires a reproduced current defect, changed explicit requirement or
responsibility conflict. The History/Validation candidate named in this
historical section was subsequently completed or superseded by the current
program audit above.

At that historical closure no next automatic task was authorized and the
15-minute `openvisionlab-2d` heartbeat was `PAUSED`. The current explicit user
request supersedes only that automation state; the active heartbeat still cannot
perform Original edit, staging, commit, push, merge, release or deployment.

Report: `docs/reports/OPENVISIONLAB_OVL07_PIPELINE_REVIEW_DOCUMENT_REVISION_GUARD_20260908.md`.
Survey: `docs/reports/OPENVISIONLAB_REFACTOR_SURVEY_20260908.md`.
Navigation: `CODEBASE_STRUCTURE` 9.2. Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-survey-20260908`.

## Current Product Identity

- OpenVisionLab is an OpenCvSharp4 deterministic rule-based vision recipe
  workbench.
- The normal workflow is sample image -> PropertyGrid teaching -> Pipeline
  composition -> explicit Preview/Run -> drawing/metric/layer comparison ->
  N-sample validation -> saved Recipe.
- LLM/XML authoring and Codex skills are optional maintenance and teaching aids.
  They are not production detectors and are not required to operate the product.
- Camera, lighting, PLC/I/O, account, MES, deployment control, and equipment
  integration remain outside scope unless the user explicitly changes direction.

## Evidence-Based Maturity

- The bounded workbench workflow is broadly connected across Tool Views,
  Pipeline, Pipeline Review, Recipe Manager, Validation Sets, Run History,
  public samples, drawings, and saved reports.
- Source/build, public-sample, storage/recovery/provenance, resource-lifetime,
  focused WPF, and portable RC evidence is broad enough for an RC/pre-production
  assessment within the recorded environments.
- This is not commercial GA. Installer/signing/update evidence, multi-PC and
  hardware qualification, calibrated metrology, field robustness, and current
  full theme/layout/DPI/monitor coverage are not proven.

## Current Repository State

- Canonical Dev root: physical directory `C:\Git\2D\Dev`.
- Branch/HEAD: `codex/public-sample-ux-docs` at
  `d875559577c85984d54900df973a6fb35fb20146`; the benchmark freeze preserved
  this dirty-worktree HEAD and no benchmark process changed it.
  `origin/codex/public-sample-ux-docs`.
- The 2026-08-30 same-volume flattening from
  `C:\Git\2D\Dev\OpenVisionLab_Dev` preserved the repository directory file ID,
  HEAD, branch, upstream, origin, exact dirty status, 1,995 managed/untracked
  files, and aggregate SHA-256. The old nested path and temporary path are
  absent. Evidence:
  `docs/reports/OPENVISIONLAB_DEV_REPOSITORY_PATH_MIGRATION_20260830.md`.
- `C:\Git\2D\Original` was not touched. Commit, push, tag, release publication,
  and deployment were not performed.
- The Dev worktree remains intentionally dirty while its existing product-code,
  smoke, skill-governance, release-draft, and documentation slices are reviewed.
  Do not discard, stage, commit, or combine them without a separate decision.

## 2026-09-08 Rule-Based Skill Static Development

Status: Complete
Follow-up 6 uses unchanged general 1.0.2 and XML candidate 0.1.17. Two fresh
producer contexts and two fresh XML contexts preserve a shared Threshold/two
Blob graph and a dark-seal pixel LineDistance plan through actual file authoring.
Both exact XML files passed the existing headless compatibility checker and
handoff validator. Branch/ROI/area, internal threshold-off, twelve edge parameters,
SourceFrame and absent calibration/acceptance were checked independently.

Priorities 1–5 retain their completed evidence. All six selected scopes are closed.
No skill/source patch was required. Optional typed USE_THRESHOLD owner
admission is not established; the retained shared-mask plan keeps its exact
caller policy in existing observation/purpose/review fields. Matching stays 0.2.1.
The checker used identified existing Dev DLLs, not a fresh product rebuild.

Full benchmarking stays deferred by the user and needs a separately coordinated
interval/admission decision. The candidate remains inactive and unqualified.
No product UI/Preview/Run, Original, frozen benchmark, commit/push or deployment
work is part of this scope. Do not reopen these completed static cases by default.

Worklist: `docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_STATIC_DEVELOPMENT_20260908.md`.
Evidence: `docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_STATIC_INTEGRATION_20260908.md`.

## 2026-09-08 Recipe XML Handoff Candidate 0.1.18

Status: Complete — standalone and contract-bound artifact finalization wording
was separated without changing validator semantics. A normal file-backed
handoff uses the base validator plus its retained XML/report/validator output;
artifact manifest, benchmark/attempt/case identity and second bundle audit are
required only when the caller supplies an artifact contract. Fifty-two focused
tests, Skill Creator, registry and documentation-index checks passed.

The candidate remains explicit-only, inactive and unqualified. No product
execution, XML import, benchmark admission, artifact contract creation, Original,
commit/push or deployment work is part of this correction.

Worklist: `docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_STATIC_DEVELOPMENT_20260908.md`.
Evidence: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_18_STANDALONE_FINALIZATION_20260908.md`.

## 2026-09-08 Recipe XML Handoff Candidate 0.1.19

Status: Complete — orphan bundle identity options now fail closed. Candidate
0.1.18's standalone mode documented no benchmark/attempt/case identity, but
`attempt_id` or `case_id` alone were silently ignored. Candidate 0.1.19 treats
those values as bundle options and returns `E_ARTIFACT_OPTIONS` unless the
complete bundle option group is supplied. Standalone and complete bundle paths
remain valid.

Fifty-three focused tests, Skill Creator, registry and documentation-index
checks passed. The candidate remains explicit-only, inactive and unqualified;
no benchmark, product execution, Import/Preview/Run, activation or qualification
was performed.

Worklist: `docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_STATIC_DEVELOPMENT_20260908.md`.
Evidence: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_19_ORPHAN_BUNDLE_IDENTITY_20260908.md`.

## 2026-09-08 Recipe XML Handoff Candidate 0.1.20

Status: Complete — contract-bound bundle audits now require both
`attempt_id` and `case_id`. Candidate 0.1.19 rejected orphan identity options,
but a bundle could still omit either value. Candidate 0.1.20 fails closed before
artifact validation when the identity pair is incomplete and keeps supplied
values cross-checked against the retained manifest.

Fifty-three focused tests, Skill Creator, registry and documentation-index
checks passed. Standalone validation and complete clear/red bundle paths remain
valid. The candidate remains explicit-only, inactive and unqualified; no
benchmark, product execution, activation or qualification was performed.

Worklist: `docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_STATIC_DEVELOPMENT_20260908.md`.
Evidence: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_20_BUNDLE_IDENTITY_REQUIRED_20260908.md`.

## 2026-09-08 Recipe XML Handoff 0.1.20 Admission Readiness Preflight

Status: Complete for a read-only admission-readiness review; benchmark
admission remains `BLOCKED`. Candidate `0.1.20` is internally aligned and its
53-test, Skill Creator, registry, documentation-index and source-manifest
checks pass. The retained strict corpus is still frozen for `0.1.13`, so a new
benchmark identity and freeze are required before evaluating `0.1.20`.

The final D-drive snapshot observed no active `dotnet`/MSBuild process, but a dirty
Dev worktree with 237 status lines remains. A point-in-time idle snapshot cannot
prove that no concurrent write will occur during a full Author audit. The
last retry remains immutable incomplete evidence (4/112 Authors, audit
violation, no Reviewers, preservation PASS), and its backend PASS is historical
rather than a fresh admission probe. No new identity, Author/Reviewer run,
product process, or token spend was started by this preflight.

Required before a future admission: reserve the operator-coordinated quiet
interval, freeze a new `0.1.20` identity, run fresh backend probes, and record a
separate admission decision. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_20_ADMISSION_READINESS_20260908.md`.
Snapshot:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-admission-readiness-20260908`.

## 2026-09-08 Recipe XML Handoff 0.1.20 New Identity and Freeze Preflight

Status: Complete for candidate identity, freeze and static preflight; benchmark
admission remains `NOT_GRANTED`. A new D-drive identity,
`openvisionlab-rule-based-round1-v0_1_20-strict-corpus-20260908`, was copied
from the preserved `0.1.13` source root and rebound to the current `0.1.20`
candidate snapshots. Its final freeze SHA-256 is
`91CB69791282DA6083F6A3303590425AEA7FAD9AE28C8766DA26FEFF154F1394`.

The freeze was recorded before preparing exactly 112 attempt metadata records.
The target contains no Author outcomes, Reviewer outputs, scorer results or
product-run artifacts. The final gate passed all 20 checks with zero failures;
the source root remained immutable at 461 files. Static direct/projected
validation, compatibility, 53 focused candidate tests, Skill Creator checks,
harness/runner regressions and the target harness self-test all passed.

This preflight did not run Authors, Reviewers, a product EXE, Import,
Preview/Run, or a fresh backend probe. The point-in-time tasklist snapshots had
no matching `dotnet` process, but an exclusive operator-coordinated quiet
interval is still not established. Runtime parity is `NOT_MEASURED` and the
candidate remains inactive and unqualified. A separate admission decision is
required before any token spend. Report:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_20_NEW_IDENTITY_PREFLIGHT_20260908.md`.
Evidence manifest:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-round1-v020-preflight-20260908\identity-preflight.json`.

## 2026-09-08 Recipe XML Handoff 0.1.20 Fresh Backend Prerequisite Probe

Status: Complete for the fresh minimal backend prerequisite; benchmark
admission remains `NOT_GRANTED`. The exact planned Author configuration
(`gpt-5.6-luna` / `medium`) and Reviewer configuration (`gpt-5.6-sol` / `high`)
each returned exit `0` with exactly `HEALTHCHECK_OK`, no event errors and no
observed tool calls under Codex CLI `0.153.4`.

The probe used sequential isolated ephemeral D-drive roots and did not execute
the product, benchmark harness, Authors, Reviewers or repository operations.
The before/after `tasklist` snapshots each still contained 11 `dotnet.exe`
processes, so the operator-coordinated quiet interval for a full Author audit
remains `NOT_ESTABLISHED`. The frozen identity's freeze-time
`backendHealth: NOT_TESTED` field and final-gate/freeze SHA remain unchanged;
this probe is prerequisite evidence only and does not grant admission.

Report:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_20_BACKEND_ADMISSION_PREFLIGHT_20260908.md`.
Machine-readable evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\codex-backend-health-20260908-v020\backend-admission.json`.

## 2026-09-08 Rule-Based Skill G1 Admission Preflight Recheck

Status: Complete for a read-only evidence recheck; G1 admission remains
`BLOCKED`. The repaired current tasklist snapshot contains `dotnet.exe=0` and
`MSBuild.exe=0`, while the retained post-static snapshot contains `23/11`.
The current observation does not establish an operator-coordinated quiet
interval for the complete Author audit, and no separate admission decision was
found in the frozen `0.1.20` target. The target final gate remains `PASS`
(`20/20`, zero failures), with `authorOutcomes=0`, `reviewerOutcomes=0`,
`qualification=false` and `runtimeParity=NOT_MEASURED`.

No benchmark or product process was started, and no Author/Reviewer tokens were
spent. The machine-readable recheck packet is
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-g1-admission-preflight-20260908\g1-admission-preflight.json`.
The report is
`docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_G1_ADMISSION_PREFLIGHT_RECHECK_20260908.md`.
The next checkpoint is an operator-recorded quiet interval and separate
admission decision before G2.

## 2026-09-08 Rule-Based Skill Prompt Alignment Recheck

Status: Complete for a static text-coverage recheck of the supplied 20-section
rule-based vision prompt against teaching skill `1.0.2` and its current
conditional references. All numbered sections `1..20` were found in order and
the coverage terms for image evidence, Tool/Pipeline/ROI/frame selection,
parameters, judgment, robustness, FP/FN, ownership, diagnostics, tests and
the ten-part output were present. This does not establish image robustness,
benchmark quality, runtime parity or field qualification.

Report:
`docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_PROMPT_ALIGNMENT_RECHECK_20260908.md`.
Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-prompt-alignment-recheck-20260908\prompt-alignment-recheck.json`.

## 2026-09-08 Rule-Based Skill Autonomous 10-Minute Plan

The recurring skill-development order is now fixed as three phases: (1) static
contract and prompt alignment, (2) admission readiness with collision-safe
execution, and (3) admitted qualification followed only by failure-derived
correction. Each heartbeat must check active tests/processes and repository
writes, perform at most one bounded action, and report the phase result and
evidence. Phase 1 is `Complete`; Phase 2 is `Blocked` on the operator quiet
interval and separate admission record; Phase 3 is `Not started`.

Plan:
`docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_AUTONOMOUS_10MIN_DEVELOPMENT_PLAN_20260908.md`.
Automation: `OpenVisionLab 2D Rule-Based Skill 10분 개발` was deleted on
2026-09-09 at the user's request; no 10-minute heartbeat remains.

## 2026-09-08 Rule-Based Skill Midterm Evaluation and Development Goals

The midterm assessment is complete as a planning record. Static contract and
governance evidence is strong: general teaching `1.0.2` and Matching `0.2.1`
are active, XML candidate `0.1.20` has focused/static PASS, the new identity
has a 20/20 final gate, and both planned backend configurations passed a fresh
minimal probe. This does not qualify the candidate.

The remaining skill-specific path is ordered evidence closure: establish the
operator-coordinated quiet interval and separate admission decision, run the
existing 112-Author/16-Reviewer contract, make only failure-derived candidate
corrections under a new freeze, and consider promotion only after all existing
Round 1 gates pass. Runtime parity, product qualification, activation, release
and deployment remain separate scopes.

Canonical plan:
`docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_MIDTERM_EVALUATION_AND_GOALS_20260908.md`.

Latest all-goal audit: G0 `PASS`; G1 `BLOCKED`; G2/G4 `NOT_STARTED`; G3
`CONDITIONAL`; G5 `NOT_MEASURED`. Report:
`docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_GOALS_VERIFICATION_20260908.md`.

## 2026-09-08 Rule-Based XML Retry 2

Status: Blocked on an operator-coordinated quiet interval for the Author audit.
The user requested a retry. A new identity preserved all old evidence, passed
45 focused tests, 12 audit fixtures, two backend probes, 11 independent input
equivalence checks and 11 admission checks, then started 112 fresh Authors.
After the prior product task closed, a newly requested Metrics/Acceptance task
started and launched its UI smoke during audit. The retry stopped with four
completed Authors, 108 missing retained, audit `VIOLATION` and final score
`INCOMPLETE`; Reviewers were not admitted. Original and frozen inputs are intact.
The candidate remains explicit-only, inactive and unqualified.

Both interrupted identities remain immutable and their results are not pooled.
A transient idle check has failed twice. The user has been asked to choose a
skill-only evaluation interval after current product work, or defer evaluation.
Do not start another identity or spend Author/Reviewer tokens until that
coordination decision is available. This does not stop separately authorized
product work or change its schedule. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_13_RETRY2_RESULT_20260908.md`.

## 2026-09-08 Rule-Based XML Candidate Round 1 Execution

Status: Blocked on a shared-workspace quiet interval for a valid new evaluation.
The admitted 0.1.13 run stopped after 4/112 completed Authors when another task
launched the Layer/Recipe UI smoke and changed Dev. All four outputs and 108
missing records remain in the fixed denominator. The audit is `VIOLATION`,
final scorer `INCOMPLETE` (exit 2), and Reviewers were not started (0/16).
Original was unchanged; frozen inputs, execution scripts and 112 metadata hashes
remain intact. Current backend health probes passed before launch.

The candidate remains explicit-only, inactive and unqualified. This is no
quality verdict; the prior 0.1.11 full quality FAIL remains historical evidence.
See `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_13_ROUND1_RESULT_20260908.md`.
The stopped identity is immutable. A new identity and freeze/preflight are
required once no concurrent repository writes or product/runner launches will
occur throughout Author audit. A transient idle snapshot did not ensure that
condition. Do not spend further Author/Reviewer tokens until it is available.
This skill request does not authorize unrelated product work or schedule changes.

## 2026-09-08 Rule-Based XML Candidate Corpus Preflight

Status: Complete for 0.1.13 corpus repair/freeze/static preflight only.
The new D-drive identity is
`openvisionlab-rule-based-round1-v0_1_13-strict-corpus-20260908`.
Seven corrupted prompts were restored from original UTF-8 sources; shared-mask,
capability-gap and per-image request ambiguity was resolved under the public
correction contract. All 32 public/private red inputs were reconciled, and the
three skill bundles plus static validator/runtime were snapshotted on D:.
The old 0.1.11 root's 1,800 files remain byte-identical.

Forty-five focused tests, direct/projection handoff validation, static compatibility
(13 XML roots / 1 recipe), harness self-test, 12 audit-classifier fixtures and
20 final integrity gates pass. All 112 prepared metadata records bind to final
freeze SHA-256 `17A025D198AD978BB0161EBCC16579763B68BF8DEE8633D260A7D366B34D6AAB`.
No Author/Reviewer execution or candidate activation occurred. The prior quality
FAIL and 900-second timeout remain evidence; backend health is untested.
See `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_13_STRICT_CORPUS_PREFLIGHT_20260908.md`.

This preflight-only checkpoint predates the admitted execution above. Its
no-run/backend-untested statements describe that checkpoint, not live status.
The product structure scope at the top of this handoff remains separate.

## 2026-09-08 Rule-Based Prompt Alignment

The user supplied a 20-section inspection-design prompt and requested skill
continuation. The existing general teaching skill is now `1.0.1`; it routes
inspection design to a new image-analysis, parameter-evidence, validation,
FP/FN, implementation-lifetime and diagnostic reference. Its v1 envelope and
existing owner/dispatch permissions remain unchanged. XML handoff candidate
`0.1.13` adds only compatibility for reviewed upstream `1.0.0` and `1.0.1`;
unreviewed versions and existing semantic violations still fail closed.

Status: Complete for this prompt-alignment and compatibility checkpoint.
Quick validation, registry checks, 29 focused candidate tests, two independent
design/routing probes and documentation index validation pass. The source mapping and complete current-task evidence belong in
`docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_PROMPT_ALIGNMENT_20260908.md`.
This checkpoint does not rerun or qualify the prior failed Round 1 benchmark.
The candidate remains explicit-only, inactive and unqualified. Existing product
work, including the OVL-07 result projection revision guard, remains separate.

## Current Verification Snapshot

- `git diff --check`: PASS; only working-copy LF/CRLF notices were emitted.
- Debug solution build: 0 warnings, 0 errors.
- `VisionRecipeRunnerSmoke` Release build: 0 warnings, 0 errors.
Readiness: 13/13 contracts PASS; documentation index: 164 indexed paths,
  13 routes, and 102 root redirects PASS.
- N-image contract: six tools x 30 images PASS. The focused WPF N-image target
  separately classifies a missing Matching template as `ERROR`, not ungated
  success.
- Focused WPF targets for exact public Matching sample opening and N-image
  verification passed on dynamically selected `\\.\DISPLAY2`; observed windows
  stayed inside `-1920,365..0,1397`. Evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\priority-review-20260830\wpf-focused\run-2`.
- The exact sample-open target also passed with process working directory on D:,
  outside the repository, proving catalog template dependency resolution does
  not rely on the process CWD. Evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\priority-review-20260830\wpf-nonrepo-cwd\evidence`.
- A two-image same-basename batch produced two SHA-suffixed run directories and
  two retained EdgeBasedMatching overlays. Evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\priority-review-20260830\batch-mini\evidence`.
- Matching skill `0.2.1` quick validation, packet/registry regression,
  template-registration regression, registry validation, and an independent
  machine-readable behavioral-forward evaluation passed at the new Dev root.
- Recipe XML handoff candidate `0.1.1` passed three installed-skill quick
  validations, six exact handoff-contract regressions, registry and Matching
  regression checks, three independent forward cases, and current
  `RecipeXmlCompatibilityCheck` for one generated measurement-only XML. The
  result is retained as the pre-benchmark baseline.
- The current candidate-only Recipe XML handoff `0.1.12` correction passed its
  28-test focused suite, UTF-8 Skill Creator validation, Python compilation,
  registry/index JSON checks, and documentation-index validation. It remains
  explicit-only, inactive, and unqualified; no benchmark rerun was performed.
- Historical frozen Rule-Based Skill Research Round 1 evidence for candidate
  `0.1.1` remains complete with 112/112 author attempts, 16/16 independent
  reviews, and audit `PASS`, but its final benchmark result was `FAIL`:
  structure 86/112, critical red fail-closed 19/32, reviewer pass 9/16,
  unsupported/invented review findings 70, product actions 0, and session
  consistency 65/80. It remains preserved as the v0.1.1 baseline and does not
  activate that candidate. Evidence:
  `docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_ROUND1_RESULT_20260901.md`.
- Recipe XML handoff correction candidate `0.1.2` remains the prior focused
  correction baseline: its 10-case exact/semantic suite, Skill Creator
  validation, corrected S3/S4 replays, and safe-static-baseline check passed.
  It is preserved as historical input to v0.1.3 and remains inactive. Evidence:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_2_FOCUSED_VALIDATION_20260901.md`.
- Candidate `0.1.3` was then implemented with closed-world authoring rules for
  exact Tool-plan/frame/ROI copying, optional-field omission, neutral evidence
  language, and stricter status/packet/baseline preservation. Its Skill Creator
  validation and 10-case focused regression suite passed. Evidence:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_3_FOCUSED_VALIDATION_20260901.md`.
- The user-authorized frozen Round 1 rerun for candidate `0.1.3` completed under
  benchmark `openvisionlab-rule-based-round1-v013-rerun2-20260901`: 112/112
  authors, audit `PASS`, 16/16 reviews, and final scorer `FAIL`. Structure was
  88/112, critical red fail-closed 17/32, reviewer pass 12/16,
  unsupported/invented findings 68, product actions 0, and session consistency
  72/80. A first v0.1.3 rerun identity was invalidated by external Dev drift;
  rerun2 is the valid authoritative result. Evidence:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_3_ROUND1_RESULT_20260901.md`.
- Installed correction candidate `0.1.4` now has the transient decision-ledger
  and ownership-preflight rules, evidence-gated upstream `PASS`, validator and
  retained-artifact hard stops, opaque baseline preservation, and exact public
  reason vocabulary. Skill Creator validation and 12-case focused regression
  checks pass; the bounded positive/blocked forward probe also passes. It
  remains inactive and outside normal dispatch. Its fresh frozen Round 1
  benchmark completed with final scorer `FAIL`: structure 84/112, unsupported/
  invented findings 58, critical red fail-closed 16/32, clear reviewer pass
  12/16, product actions 0, and session consistency 74/80. Evidence:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_FOCUSED_VALIDATION_20260901.md`.
  The full benchmark result, hashes, audit PASS, and reviewer coverage are
  recorded in
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_ROUND1_RESULT_20260902.md`.
- The evidence-backed failure classification and minimum C1–C6 correction
  scope are recorded in
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_TRIAGE_20260902.md`.
- Candidate-only implementation of that scope is now installed as
  `openvisionlab-recipe-xml-handoff 0.1.5`: the finalization/projection gate,
  owner matrix, opaque-baseline boundary, public routing rules, S4 frame lock,
  catalog parameter allowlist, and 15-case focused regression are in place.
  The independent positive/blocked forward probe passes; the candidate remains
  explicit-only, inactive, and outside normal dispatch. Evidence:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_FOCUSED_VALIDATION_20260902.md`.
- The fresh frozen Round 1 benchmark for candidate `0.1.5` is complete under
  `openvisionlab-rule-based-round1-v015-20260902` with author wrapper exit `0`,
  audit `PASS`, `16/16` independent reviews, and final scorer `FAIL`:
  structure `73/112`, unsupported/invented findings `69`, critical red
  fail-closed `20/32`, clear reviewer pass `10/16`, product actions `0`, and
  session consistency `78/80`. A frozen Matching packet/hash and clear
  artifact-contract contradiction was exposed; the candidate remains
  explicit-only, inactive, and unqualified. Evidence:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_ROUND1_RESULT_20260902.md`.
- Candidate-only `0.1.6` focused validation and the `0.1.7` generic-ROI/status
  correction both passed their bounded checks, but neither candidate is
  activated. Evidence:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_FOCUSED_VALIDATION_20260903.md`,
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_FOCUSED_VALIDATION_20260903.md`.
- The isolated Round 1 runner/scorer correction now pins the author process
  `cwd` and retry scan root, scopes intentional red-team hash mismatches to
  case-declared path/hash tuples, and includes scan root in the validator cache
  key. Its 15 harness tests, 1 runner test, and Python compilation pass. This
  corrects the proven protocol defect without changing the frozen corpus or
  installed candidate. Evidence:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_ROUND1_RUNNER_SCORER_CORRECTION_20260903.md`.
- A read-only contract/corpus reconciliation preflight confirms the legacy
  safe-static baseline incompatibility and the three hidden-key reason-alias
  mismatches, while direct S3-02/S4-01 evidence does not reproduce the earlier
  alleged projection/ratio blockers. No frozen corpus was changed. Evidence:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_CONTRACT_CORPUS_RECONCILIATION_PREFLIGHT_20260903.md`.
- An actual embedded 2D EXE registered the locked synthetic die-pad source and
  template, retained zero results before explicit Preview, then returned three
  intended repeated-pad candidates with max score `86.198` and visible
  correspondence/overlay evidence. The registration manifest and visual review
  pass, but this is registration-preview evidence rather than Matching
  qualification, numeric margin proof, or Takt compliance. Evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\nightly-20260831\matching-teaching-v1\actual-exe`.
- A fresh managed-Dev Machine producer to 2D consumer process completed schema
  `2.0` transaction `8ac02470-8912-4d78-8dee-856be63106f0` as `Pass`; the
  producer now derives project id/schema from the supplied Machine project
  instead of a smoke-only constant. Evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab-CrossRepo\nightly-20260831\machine-2d-result\2d-cross-repo-20260831-011415-84bd8854c52a4a85b432686f274775eb`.
- The full WPF theme/state/Wide/Compact/100-200% DPI/performance matrix was not
  executed and is not claimed by this snapshot.

## Current Decision And Priority

The latest admitted Round 1 execution is candidate `0.1.11` under the immutable
identity `openvisionlab-rule-based-round1-v011-strict-corpus-20260904`. Authors
completed `112/112`, the external audit was `PASS`, and independent Reviewers
completed `16/16`; final scoring is `FAIL`: structure `108/112`, unsupported or
invented findings `16`, critical red fail-closed `29/32`, clear Reviewer task
pass `14/16`, product actions `0`, and session consistency `77/80`. The
authoritative result is
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_ROUND1_RESULT_20260904.md`.
The evidence triage and candidate-only `0.1.12` correction were completed
on 2026-09-04. The 2026-09-08 compatibility checkpoint above advances the
installed candidate to `0.1.13`; the following evidence describes `0.1.12`:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_TRIAGE_20260904.md`,
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_12_FOCUSED_VALIDATION_20260904.md`.
The remaining XML candidate gate is a separately approved new strict-corpus
repair/freeze and Round 1 admission decision for the exact reviewed candidate
version; no new run has been started. | Recommended
model: gpt-5.6-terra | Reasoning effort: high.

That triage is now complete. It separates two benchmark-corpus defects from
candidate-owned residual behavior: the v0.1.5 Matching packet contains stale
nested evidence hashes, and the safe-static-baseline wrapper disagrees with
its nested handoff pointer paths. The conditional clear-artifact conflict must
be repaired in a new benchmark identity; the v0.1.5 root remains immutable.
Gate A is now complete in the separately frozen
`openvisionlab-rule-based-round1-v015-corpusfix-20260903` identity. Matching
registration/lock/packet hashes, all linked case hashes, and a local
safe-static-baseline bundle are internally consistent; `load_contract`, harness
self-test, recursive case-reference checks, and a candidate baseline
compatibility probe passed. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_CORPUS_REFREEZE_20260903.md`.
The original v0.1.5 root remains immutable; the installed candidate was not
changed by Gate A.

The separately scoped candidate-only v0.1.6 bundle is now complete for its
focused checkpoint: executable artifact finalization, trust-boundary
projection, public routing/baseline precedence, and expanded regression are
implemented and independently forward-checked. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_FOCUSED_VALIDATION_20260903.md`.
The fresh v0.1.6 Round 1 admission attempt then executed 112/112 Authors with
author wrapper exit `0` and external audit `PASS`, but its mechanical
pre-review score is `INCOMPLETE`: structure `95/112`, clear expected answer
`65/80`, critical red fail-closed `19/32`, session consistency `70/80`,
unsupported/invented findings `0`, and product actions `0`. Reviewer execution
was guard-blocked before any context (`0/16`) because audit event 72 used the
benchmark root as the static-validator scan root and the scorer therefore
rejected audit coverage. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_ROUND1_RESULT_20260903.md`.
The resulting evidence-triage checkpoint separates benchmark/scorer and
frozen-corpus compatibility defects from candidate-owned status, baseline,
reason-routing, and authority-lock defects, and identifies the minimum
correction boundary for a new identity. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_TRIAGE_20260903.md`.
The candidate-only v0.1.7 correction is now complete: generic ROI authority
locks match any enabled consumer with the explicit ROI pair, and the inline,
measurement-only, immutable-baseline, repository-contract, locator-frame, and
upstream-graph precedence guidance is explicit. The focused suite is 21/21,
the independent positive/blocked forward probe is PASS, and no frozen
benchmark or product action was performed. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_FOCUSED_VALIDATION_20260903.md`.
The candidate remains explicit-only, outside normal dispatch, inactive, and
unqualified. The isolated runner/scorer correction is complete and recorded
in `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_ROUND1_RUNNER_SCORER_CORRECTION_20260903.md`;
it does not rewrite the frozen root or admit a benchmark. The reconciliation
preflight reclassified S3-02/S4-01 as not-confirmed mechanical blockers. The
generic public reason-code projection decision is now recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_REASON_PROJECTION_DECISION_20260903.md`,
and the new strict-baseline identity/preflight is complete under
`openvisionlab-rule-based-round1-v017-strictbaseline-20260903`; its evidence is
in `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_STRICT_BASELINE_PREFLIGHT_20260903.md`.
The separately admitted execution then completed Authors `112/112` with
author-wrapper exit `0` and audit `PASS`, but the mechanical scorer returned
`INCOMPLETE` (structure `103/112`, clear expected-answer `76/80`, critical
red fail-closed `25/32`, session consistency `76/80`). The Reviewer runner
was guard-blocked before spawning, so Reviewer evidence is `0/16`. The full
result is recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_ROUND1_RESULT_20260903.md`.
The candidate remains explicit-only, outside normal dispatch, inactive, and
unqualified. A separately scoped v0.1.8 candidate correction is now installed
and focused-validated: the immutable-baseline tuple is projected first,
file-backed XML with validator PASS/no acceptance remains MEASURE_ONLY even
when upstream is PROPOSED, and the multi-block red routing order is explicit.
Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_FOCUSED_VALIDATION_20260903.md`.
The new D-drive identity
`openvisionlab-rule-based-round1-v018-preflight-20260903-r1` is frozen and
preflighted with 112 final-hash-bound metadata records, direct and projected
baseline PASS, static compatibility PASS, and no Authors/Reviewers artifacts.
Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_STRICT_BASELINE_PREFLIGHT_20260903.md`.
The separately admitted Authors execution then recorded `112/112` attempts and
closed the external audit `PASS`, but only 40 attempts produced valid author
evidence. The remaining 72 attempts (all S3/S4 clear attempts and RT01–RT32)
ended with Codex backend HTTP 503/404 connection errors. The mechanical scorer
is therefore `INCOMPLETE` (structure `40/112`, clear expected answer `40/80`,
critical red `0/32`); the Reviewer phase was not launched. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_ROUND1_RESULT_20260903.md`.
The candidate remains explicit-only, outside normal dispatch, inactive, and
unqualified. Any future rerun requires backend availability and a new frozen
benchmark identity; this root is not to be repaired in place. |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

On 2026-09-04, the current Codex binary passed a one-shot D-drive-only health
check, so the previously blocked execution prerequisite was available. The
recovered run was admitted only under the new identity
`openvisionlab-rule-based-round1-v018-backend-recovery-20260904-r2`; the prior
incomplete root remains immutable. Evidence:
`docs/reports/OPENVISIONLAB_CODEX_BACKEND_HEALTH_20260904.md` and
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_BACKEND_RECOVERY_ROUND1_RESULT_20260904.md`.

That v0.1.8 identity completed Authors `112/112`, audit `PASS`, and Reviews
`16/16`, but final scoring is `FAIL`: structure `108/112`, critical red
fail-closed `28/32`, reviewer pass `11/16`, unsupported/invented findings `34`,
product actions `0`, and session consistency `79/80`. Candidate-only v0.1.10
then made the four public-reason precedence rules explicit and passed 25 focused
tests, Skill Creator validation, and Python compilation. A new immutable
v0.1.10 identity is now fully preflighted: contract/self-test, direct and
projected baseline, static compatibility, harness/runner checks, prepare, and
112/112 metadata freeze-hash verification all pass. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_FOCUSED_VALIDATION_20260904.md`
and
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_STRICT_BASELINE_PREFLIGHT_20260904.md`.
The separately admitted v0.1.10 Round 1 execution is now complete under that
immutable identity: Authors `112/112`, audit `PASS`, Reviews `16/16`, and final
scorer `FAIL` with structure `100/112`, critical red fail-closed `20/32`, clear
Reviewer task pass `14/16`, unsupported/invented findings `20`, product actions
`0`, and session consistency `79/80`. The candidate remains explicit-only,
outside normal dispatch, inactive, and unqualified. The authoritative result is
recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_ROUND1_RESULT_20260904.md`.

The next skill priority is a separately scoped evidence-triage and correction
decision for the v0.1.10 clear artifact/Reviewer failures, red reason/status
precedence misses, independent input-frame validation failures, and the 20
unsupported/invented findings. A new benchmark identity is required after any
candidate-only correction; this failed root remains immutable. |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

On 2026-09-04, that triage is complete for the v0.1.10 identity. The 12 red
misses split into candidate status/projection residuals, public-contract gaps
for specialized reason precedence, and a confirmed legacy safe-static
baseline/validator compatibility defect (`SourcePixelFrame[572x420]` on a
`Main` stage). The 20 unsupported/invented findings are grouped into owner
provenance, pixel-only/calibration, graph/projection, and S3-03 semantic-family
overreach; `S2-01` and `S3-03` Reviewer failures are explained by the same
groups. No installed candidate resource or frozen root changed. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_TRIAGE_20260904.md`.

The immediate next skill priority is owner approval of the public reason and
NormalizeImage-value contract plus a new strict-corpus repair specification;
only then may a candidate-only correction be edited and focused-tested. The
v0.1.10 candidate remains explicit-only, inactive, outside normal dispatch, and
unqualified. | Recommended model: gpt-5.6-terra | Reasoning effort: high.

The public correction contract and replacement-corpus repair specification are
now prepared as drafts for operator review:
`docs/roadmap/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_CORRECTION_CONTRACT_20260904.md`
and
`docs/roadmap/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_CORPUS_REPAIR_SPEC_20260904.md`.
They define pattern-based reason precedence, strict `Main -> SourceFrame`
baseline requirements, explicit NormalizeImage value ownership, lossless case
intent, and the unchanged Round 1 gates. No candidate resource, benchmark root,
or product state was changed; publication, candidate correction, new freeze,
and benchmark admission require separate approval. | Recommended model:
`gpt-5.6-terra` | Reasoning effort: `high`.

Candidate activation, normal dispatch, Round 2, product execution, and
qualification remain separate approvals. The v0.1.4 benchmark and v0.1.3
comparison evidence remain immutable roots.

## Candidate-only v0.1.11 Correction — 2026-09-04

The operator authorized the candidate-only continuation using the conservative
NormalizeImage rule: an unowned `FIXTURE_MIN_VALID_PIXEL_RATIO` returns `WAIT`;
the guide's `0.25` example is not a default. Installed
`openvisionlab-recipe-xml-handoff` is now candidate version `0.1.11`. The patch
adds integrity-first hash/schema routing, typed ROI and affine-correspondence
waits, upstream-WAIT propagation, pixel-only calibration boundaries,
image-versus-policy ownership, intent/projection isolation, and strict
`Main -> SourceFrame` baseline compatibility. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_FOCUSED_VALIDATION_20260904.md`.

The focused suite passed `27` tests; Python compilation and UTF-8 Skill Creator
validation passed. The candidate remains explicit-only, outside normal
dispatch, inactive, and unqualified. The v0.1.10 failed Round 1 root remains
immutable; the public correction/corpus documents remain Draft, and no new
benchmark identity, freeze, activation, runtime execution, qualification,
Original-repository change, commit, push, release, or deployment was made.

The immediate candidate/corpus priority is now complete: the new strict
SourceFrame-compatible identity was frozen and preflighted with explicit
NormalizeImage ratio ownership. Remaining skill work is a separate operator
approval for benchmark admission; the public correction contract remains Draft.
Remaining product priority and commercial boundaries are unchanged. |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## Candidate-only v0.1.11 Strict Corpus Preflight — 2026-09-04

The replacement corpus is now frozen under the new D-drive-only identity
`openvisionlab-rule-based-round1-v011-strict-corpus-20260904` at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v011-strict-corpus-20260904`.
The prior v0.1.10 failure root remains immutable; its freeze-record hash was
rechecked unchanged. The replacement repairs the retained baseline to
`Main -> SourceFrame`, makes S1/S3 intents lossless and pixel/edge scoped, and
records one explicit S4 operator lock:
`FIXTURE_MIN_VALID_PIXEL_RATIO=0.25` owned by `OPERATOR_LOCK`.

Contract load, harness self-test, direct and baseline-wrapper projection
validation, static XML compatibility, the 15-case harness regression suite, the
one-case author-runner regression, five-file Python compilation, `prepare`, and
the exact 112/112 metadata freeze gate all passed. No Author, Reviewer, audit,
score, product, or Original-repository action was performed. Full evidence and
frozen hashes are recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_STRICT_CORPUS_PREFLIGHT_20260904.md`.
Copied legacy runner packet templates were quarantined in the D-drive `temp`
area; the active review-packet directory is empty until a Reviewer phase is
explicitly admitted.

At that preflight checkpoint the candidate was `0.1.11`, `EXPLICIT_ONLY`,
outside normal dispatch, inactive, and unqualified. The next skill priority at
that checkpoint was a separate operator decision to admit this immutable identity
to Round 1; admission must preserve
the existing 112-attempt, red fail-closed, reviewer, session-consistency, and
zero-side-effect gates. | Recommended model: `gpt-5.6-terra` | Reasoning effort:
`high`.

## Candidate-only v0.1.12 Failure-Derived Correction — 2026-09-04

The v0.1.11 Round 1 triage is recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_TRIAGE_20260904.md`.
It separates the single S2 timeout (backend variance) from candidate-owned
provenance, shared-mask, prior-mask, intent-isolation, owner/evidence, and
public reason-precedence residuals. The installed candidate is now
`openvisionlab-recipe-xml-handoff 0.1.12` with the smallest correction boundary:
explicit `OPERATOR_SELECTED` provenance, `OPERATOR_LOCK` preservation for
NormalizeImage ratio, one-Threshold shared-mask fan-out, explicit
`USE_THRESHOLD=false`, dark-band parameter isolation, platform/capability and
per-image precedence, and neutral immutable-baseline explanations. Evidence:
`docs/roadmap/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_CORRECTION_CONTRACT_20260904.md`.

The candidate-only focused suite passed `28` tests; UTF-8 Skill Creator
validation and Python compilation passed. The candidate remains
`EXPLICIT_ONLY`, outside normal dispatch, inactive, and unqualified. The
v0.1.11 frozen root, scorer, Authors, Reviewers, product/runtime, Original
checkout, and repository history were not changed. A new strict corpus, freeze,
Authors/Reviewers run, scoring, activation, or promotion has not been performed.

The remaining skill priority is separate strict-corpus repair/freeze and Round
1 admission approval after the `0.1.12` candidate is reviewed. | Recommended
model: `gpt-5.6-terra` | Reasoning effort: `high`.

No autonomous product implementation is admitted from the present evidence.
The remaining product work items are activation-gated in this order:

1. PCB Matching Takt qualification — prerequisite: declare measurement scope
   (`MATCHING_STEP`, `PIPELINE`, or `END_TO_END`), positive budget and unit,
   p95 and hard maximum, warm-up/repeat policy, plus held-out/operator physical
   correspondence review. Current state is `WAIT_TAKT_BUDGET`; A0 is the
   coverage candidate and A2 is the typical-step-time candidate, but neither is
   a product default. The operator reported that no such timing contract can be
   supplied, so the current measurements are characterization only and the same
   fields must not be requested again unless a real requirement owner provides
   them. | Recommended model: none until the prerequisite exists | Reasoning
   effort: none until the prerequisite exists.
2. `locator-relative-blob-v1` qualification — prerequisite: an operator must
   approve or replace the native physical locator and define downstream ROI and
   tolerance before frozen Train/Validation/Held-out work. The synthetic UI
   approval is plumbing evidence only. | Recommended model: none until the
   prerequisite exists | Reasoning effort: none until the prerequisite exists.
3. `CVR-00` novice validation — prerequisite: three independent first-time
   participants and their unedited observations. Agent-operated recordings are
   not participant evidence. | Recommended model: none until the prerequisite
   exists | Reasoning effort: none until the prerequisite exists.
4. Clean RC2/publication work — prerequisite: a separately reviewed clean
   candidate and explicit release-stage authorization. `PL-0012` candidate
   review and `PL-0011` publication/deployment remain separate boundaries. |
   Recommended model: none until the prerequisite and authorization exist |
   Reasoning effort: none until the prerequisite exists.

The deferred full WPF state/theme/Wide/Compact/DPI/performance gate belongs to
the next admitted visible UI slice. Source inspection or the prior functional
smoke does not close that gate.

## OVL-06a Process-Recovery Regression — 2026-09-07

The user-authorized refactoring continuation added a process-boundary regression
harness for the existing PL-0009 Pipeline lifecycle journal. The Release
`VisionRecipeRunnerSmoke` contract stopped a child process after each durable
rename/delete journal stage and reopened the same D: data root in the parent:
`11/11` cases recovered, `11/11` child processes were killed, and no journal,
backup, or atomic temporary artifact remained. The apphost and
`dotnet <assembly>.dll` launch paths both passed. Existing same-process recovery
and storage-path contracts also passed. Evidence and scope are recorded in
`docs/reports/OPENVISIONLAB_OVL06_BEHAVIOR_REGRESSION_PROCESS_RECOVERY_20260907.md`.

This closes OVL-06a only. OVL-06b (full quantitative audit) remains separate;
at that checkpoint the next implementation priority was OVL-07 Validation
responsibility ownership, after its current facade/presenter/execution state
owner was rechecked. The first OVL-07 execution slice is recorded below. |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## OVL-08 PropertyGrid 정책 분리 — 2026-09-07

The user-authorized PropertyGrid slice moved the application-specific child-row
classification out of `WpfPropertyGridBridge`. The previous bridge-owned
`THRESHOLD`/`CANNY` name list and Affine reflection checks now live in
`OpenVisionLab.Common.PropertyGridToolPolicy`; the bridge receives the generic
`PropertyGridDisplayOptions.ChildParameterPredicate` callback through the
existing abstraction. `VisionToolPropertyGridHost` and the direct Matching
smoke wire the application policy. Existing `IsBrowsable`, RangeEditor
companion descriptors, editor registration, ordering, and keyboard commit
contracts were left in place.

Debug x64 builds of the abstraction, bridge, application, and screenshot smoke
passed with no errors. Matching child-row policy plus ComboBox/RangeEditor
layout passed, and Matching, Affine, Contour, EdgeBasedMatching, Blob, and Line
PropertyGrid consumers passed on the current single monitor. Evidence and the
bounded scope are recorded in
`docs/reports/OPENVISIONLAB_OVL08_PROPERTYGRID_POLICY_20260907.md` and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl08-propertygrid-policy-20260907`.
The full supported theme/layout/DPI matrix remains a separate UI gate. This
slice is complete; the next implementation priority is the remaining OVL-07
Validation responsibility work. | Recommended model: `gpt-5.6-terra` | Reasoning effort:
`high`.

## OVL-07 Validation 실행 책임 1차 분리 — 2026-09-07

The user-authorized refactoring continuation extracted the Local Validation Set
execution loop from `Recipe/CommandSurface/Handlers.cs` into
`OpenVisionRecipeValidationSetRunner`. The new concrete owner performs pipeline
XML loading, frozen identity validation, ordered image execution, result mapping,
expected-outcome judgment, partial-stop detection, and summary persistence.
`OpenVisionRecipeValidationRunSupport` owns the moved sample/result conversion
helpers. The Shell remains the binding/command façade and retains selection,
execution-session lifecycle, status projection, history refresh, and error
translation. `OpenVisionRecipeExecutionSessionViewModel` remains the single
owner of running/stop state. The runner has no Shell or WPF dependency and does
not create a mutable Validation Set copy.

Debug and Release x64 app/smoke builds passed. The existing
`wpf_shell_host_recipe_local_validation_set` Debug and Release targets passed
the Set CRUD/repair/variant/full-run/partial-save/re-enable workflow. Static
structure checks and `git diff --check` passed. Evidence and the bounded proof
are recorded in
`docs/reports/OPENVISIONLAB_OVL07_VALIDATION_RESPONSIBILITY_20260907.md` and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-validation-responsibility-20260907`.
The full supported theme/layout/DPI/multi-monitor UI matrix remains unverified.
This closes the first OVL-07 execution slice; Validation Set document/selection/
CRUD and the remaining suite orchestration remain for a later slice. | Recommended
model: `gpt-5.6-terra` | Reasoning effort: `high`.

## OVL-07 Validation Set 문서 owner 분리 — 2026-09-07

The next single refactoring slice moved Validation Set document state,
persistence, set creation/deletion, image add/update/repair/remove, and catalog
pair import behind the concrete `OpenVisionRecipeValidationSetDocumentOwner`.
The Shell no longer owns the mutable document field or calls the document
Storage mutation APIs directly. It remains the binding/command facade and keeps
selection display, image-row projection, status translation, and the explicit
Preview/Run contract. Existing XML schema, storage path, normalization,
identity-lock, and Variant validation remain delegated to the existing Storage
owner. Folder selection still only enumerates candidate paths in the Shell input
boundary.

The new owner has no Window/WPF dependency. The Window-free
`--validation-set-document-owner-contract` passed create/save/reload, duplicate
rejection, image add/projection, image removal, and set deletion in both Debug
and Release. Debug and Release app/smoke builds passed; the existing
`wpf_shell_host_recipe_local_validation_set` Debug and Release targets passed
with `check=OK`, `size=1600x900`. Static structure proof, documentation index,
and `git diff --check` passed. Evidence and the bounded report are recorded in
`docs/reports/OPENVISIONLAB_OVL07_VALIDATION_DOCUMENT_OWNER_20260907.md` and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-validation-document-owner-20260907`.
The full supported theme/layout/DPI/multi-monitor UI matrix remains unverified;
Original, commit, push, merge, and deployment were not touched.

다음 남은 우선순위: OVL-07 Validation Set selection/projection 상태 owner 분리 |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## OVL-07 Validation Set selection/projection owner 분리 — 2026-09-07

The next single refactoring slice moved Validation Set option selection,
PinArrayGap Train/Validation/Test selection, image-row projection, and selected
image-row state from the Shell backing fields to the concrete
OpenVisionRecipeValidationSetSelectionOwner. The Shell remains the binding and
command façade and keeps OnPropertyChanged, status, command re-evaluation, and
Variant input projection. The document owner still owns XML state and mutation;
the existing Validation Set runner still owns execution.

The selection owner has no Window/WPF/Shell dependency. It preserves valid
selection when WPF TwoWay ComboBox/list bindings report a transient null while
ItemsSource is refreshed; empty lists still clear selection explicitly. The
window-free document/selection contract passed in Debug and Release. The
existing wpf_shell_host_recipe_local_validation_set Debug and Release smoke
passed with check=OK, layout=0, text=0, internal=0, and size=1600x900.
App, runner, and smoke builds passed with zero warnings/errors. Static structure,
git diff --check, and documentation index checks passed. Evidence and the scoped
proof are recorded in
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-validation-selection-owner-20260907
and
docs/reports/OPENVISIONLAB_OVL07_VALIDATION_SELECTION_OWNER_20260907.md.
The full theme/layout/DPI/multi-monitor UI matrix remains unverified; Original,
commit, push, merge, and deployment were not touched.

다음 남은 우선순위: OVL-07 Step Edit 책임 경계 분리 |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-07 Step Edit selected-Step loader 분리 — 2026-09-07

The next single refactoring slice moved selected Step pipeline XML loading,
selected-Step re-resolution, and PropertyGrid edit-object projection behind the
Window-free concrete `OpenVisionRecipeStepEditLoader`. The Shell now adapts the
loader result into the existing `OpenVisionRecipeStepEditSessionViewModel`.
The Shell still owns the edit session, pending PropertyGrid commit, XML apply,
round-trip validation, failed-apply restore, status/notification updates, and
explicit Preview/Run behavior. Recipe/XML schema and path, selected-Step
identity fallback, PropertyGrid mapping, and Layer routing were preserved.

The window-free `--step-edit-loader-contract` passed XML Step resolution,
`BlobProperty` projection, stale-index Name/ToolType/OutputLayer fallback,
active-pipeline fallback, and missing-selection guard in Debug and Release.
The existing `wpf_shell_host_pipeline_step_edit_handoff` and
`wpf_shell_host_fixture_step_edit_apply_rerun` passed in Debug and Release with
`check=OK`, `layout=0`, `text=0`, `internal=0`, and `size=1600x900`. The smoke
harness was aligned with the current docked Pipeline Review by expanding its
details toggle before Step Edit; no product UI behavior was changed. App and
runner builds passed with zero warnings/errors; the screenshot smoke build has
one existing nullable conversion warning and zero errors. Static structure,
`git diff --check`, and documentation-index results are recorded with the
focused evidence. The full supported theme/layout/DPI/multi-monitor UI matrix
remains unverified. Original, commit, push, merge, and deployment were not
touched.

Evidence and the bounded report are recorded in
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-step-edit-loader-20260907`
and
`docs/reports/OPENVISIONLAB_OVL07_STEP_EDIT_LOADER_20260907.md`.

다음 남은 우선순위: OVL-07 Step Edit apply persistence/validation/rollback owner 분리 |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-07 Step Edit apply persistence/validation/rollback owner 분리 — 2026-09-07

The next single refactoring slice moved the selected Step XML apply transaction
from the Shell/View path into the Window-free concrete
`OpenVisionRecipeStepEditApplyOwner`. The owner loads the existing XML backup,
applies the existing `VisionPipelineStepPropertyMapper` mapping, saves the
Pipeline, validates XML round-trip, and restores the previous persisted state
after save or validation failure. The Shell remains responsible for the
PropertyGrid commit callback boundary, `OpenVisionRecipeStepEditSessionViewModel`,
status/notification projection, Pipeline Review refresh, corrected-output review,
and explicit Preview/Run behavior. Recipe/XML paths and schema were preserved.

The product View no longer creates or passes Step Edit save/validation delegates;
the Shell constructor keeps the optional hooks only for internal compatibility.
Existing smoke fault hooks now forward through View → Shell → apply owner. The
owner has no Window/WPF/Shell dependency, and the session ViewModel remains one
non-partial concrete state owner.

The window-free `--step-edit-apply-owner-contract` passed normal apply/save/
round-trip, injected save failure with restore, injected round-trip failure with
restore, and unsupported-property no-write checks in Debug and Release. Debug
and Release app/runner builds passed with 0 warnings/0 errors. Screenshot smoke
Debug passed with 0 warnings/0 errors; Release retains the existing nullable
conversion warning at `Program.cs:10722` and has 0 errors. The latest two WPF
Step Edit targets passed in both configurations with `check=OK`,
`layout=0|text=0|internal=0`, `size=1600x900`, and exit 0. The dynamically
selected exact two-monitor test display was the smaller left `\\.\DISPLAY2`,
with actual window rectangle verification recorded in the evidence directory.
Static structure, `git diff --check`, and documentation-index validation are
recorded with the report.

Evidence and the bounded report are recorded in
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-step-edit-apply-owner-20260907`
and
`docs/reports/OPENVISIONLAB_OVL07_STEP_EDIT_APPLY_OWNER_20260907.md`.
The full theme/layout/DPI 100/125/150/175/200% and multi-monitor matrix remains
unverified; Original, commit, push, merge, and deployment were not touched.

다음 남은 우선순위: OVL-07 Step Edit apply 결과/상태 projection owner 분리 |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-07 Step Edit apply 결과/상태 projection owner 분리 — 2026-09-07

The next single refactoring slice moved the apply-result presentation policy out
of `Recipe/CommandSurface/Handlers.cs` into the Window-free
concrete `OpenVisionRecipeStepEditApplyProjectionOwner`. The owner maps success
and failure results to the existing localized Step Edit status, optional Shell
status, and corrected-output review text. The Shell still owns the PropertyGrid
commit boundary, XML transaction call, edit-session dirty/clean state, pipeline
refresh/selection, and explicit Preview/Run sequencing.

The owner has no WPF/View/Shell dependency and reuses the existing text and
Pipeline Step Review presenter. The handler no longer contains the old
round-trip restore status branch, successful apply string construction, or
direct corrected-output presenter call; it only applies the projection to the
existing session/status state. Recipe/XML schema/path, Layer routing, status
binding names, and Korean/English messages remain compatible.

The window-free `--step-edit-apply-projection-contract` passed in Debug and
Release for success, save failure, restored/failed round-trip status, and
unsupported-property channel preservation. The preceding apply-owner contract
also passed in both configurations. Debug/Release app and runner builds passed
with 0 warnings/0 errors. Screenshot smoke builds passed with the existing
nullable conversion warning at `Program.cs:10722` and 0 errors. The two existing
Step Edit WPF targets passed in both configurations with `check=OK`,
`layout=0|text=0|internal=0`, `size=1600x900`, and exit 0. The exact two-monitor
run selected the smaller left `\\.\DISPLAY2` and verified the actual window
rectangle. Static structure, `git diff --check`, and documentation-index
validation are recorded with the report.

Evidence and the bounded report are recorded in
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-step-edit-apply-projection-20260907`
and
`docs/reports/OPENVISIONLAB_OVL07_STEP_EDIT_APPLY_PROJECTION_20260907.md`.
The full theme/layout/DPI 100/125/150/175/200% and multi-monitor matrix remains
unverified; Original, commit, push, merge, and deployment were not touched.

다음 남은 우선순위: OVL-07 Recipe Manager lifecycle/CRUD 책임 owner 분리 |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-07 Recipe Manager lifecycle 정책 owner 분리 — 2026-09-07

The next single refactoring slice moved Recipe Manager create/duplicate/rename/
delete admission policy out of `Recipe/CommandSurface/RecipeWorkspace.cs`
into the existing Window-free concrete `OpenVisionRecipeWorkspaceUseCase`.
The use case now owns name validity, source membership, rename conflict, and
last-Recipe delete guards. Existing workspace mutations, unique-name behavior,
Recipe/XML paths, and the Shell's pending-edit, switch/refresh, status, and
explicit Preview/Run flow remain unchanged.

The new `--recipe-workspace-policy-contract` passed in Debug and Release for
blank/valid/invalid create names, duplicate source/name policy, rename identity
and conflict policy, and last-Recipe protection. Debug/Release app and Runner
builds passed with 0 warnings/0 errors. ScreenshotSmoke was rebuilt in both
configurations with the existing `Program.cs:10722` CS8600 warning and 0 errors.
The Recipe Manager CRUD WPF target passed before the change as a Release
baseline and after the change in Debug/Release with `check=OK`,
`layout=0|text=0|internal=0`, `size=1600x900`, exit 0. Dynamic monitor detection
reported one monitor, `\\.\DISPLAY2` (work area `0,0,1920x1032`), and verified
window rectangle `160,66..1760,966`. Static owner proof, documentation-index,
and scoped diff checks are recorded with the report.

Evidence and the bounded report are recorded in
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-recipe-manager-lifecycle-20260907`
and
`docs/reports/OPENVISIONLAB_OVL07_RECIPE_MANAGER_LIFECYCLE_POLICY_20260907.md`.
The full Recipe Manager lifecycle result/status projection, Pipeline CRUD,
validation/Step Edit, image/Layer lifetime, and full theme/layout/DPI/
multi-monitor matrix remain separate work. Original, commit, push, merge, and
deployment were not touched.

다음 남은 우선순위: OVL-07 Recipe Manager lifecycle 결과/status projection owner 분리 |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-07 Recipe Manager lifecycle 결과/status projection owner 분리 — 2026-09-07

The next single refactoring slice moved Create, Duplicate, Rename, and Delete
success/failure status projection out of
`Recipe/CommandSurface/RecipeWorkspace.cs` into the
Window-free concrete `OpenVisionRecipeWorkspaceLifecycleProjectionOwner`.
The owner returns a result/status DTO; it does not own WPF state, storage
mutation, pending-edit transitions, Recipe switching, or refresh ordering.
Delete status keeps the deleted-name snapshot while the use-case fallback name
remains the projected result name. Create failure remains silent, and the
existing localized Duplicate/Rename/Delete failure text is preserved.

Debug/Release app and `VisionRecipeRunnerSmoke` builds passed with 0 errors.
The new `--recipe-workspace-lifecycle-projection-contract` passed in both
configurations. ScreenshotSmoke rebuilds retained the existing CS8600 warning
at `Program.cs:10722` (1 warning, 0 errors). Monitor-aware Recipe Manager CRUD
smoke passed in Debug and Release with `check=OK`, `layout=0|text=0|internal=0`,
size `1600x900`, actual window `160,66--1760,966`, on the single reported
`\\.\DISPLAY2` (`0,0,1920x1080`, work area `0,0,1920x1032`). Static structure
proof, scoped diff check, and documentation index validation passed. Evidence and
the bounded report are recorded in
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-recipe-manager-projection-20260907`
and
`docs/reports/OPENVISIONLAB_OVL07_RECIPE_MANAGER_LIFECYCLE_PROJECTION_20260907.md`.
The first Release UI attempt hit the existing clipboard XML-paste transient
`CLIPBRD_E_CANT_OPEN (0x800401D0)`; after confirming no process remained, a new
data/evidence root retry passed the same target.
The full theme/layout/DPI/multi-monitor matrix, broader solution readiness,
Original, commit, push, merge, and deployment remain unverified or untouched.

다음 남은 우선순위: OVL-07 Recipe Manager summary/library projection owner 분리 |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-07 Recipe Manager summary/library projection owner 분리 — 2026-09-07

The next single refactoring slice moved selected Recipe summary assembly and
Recipe library count formatting out of
`OpenVisionShellHostRecipeCommandSurface` into the Window-free concrete
`OpenVisionRecipeManagerSummaryProjectionOwner`. The owner now builds the
summary DTO, localized detail text, stored XML validation report, Pipeline
preview Step list, and empty/full/filtered library count text from explicit
request snapshots. It does not own WPF state, storage reads, Recipe/Pipeline
mutation, selection, PropertyChanged, Layer card UI callbacks, or explicit
Preview/Run execution.

The Shell still reads Pipeline names, active Pipeline, stored XML and Recipe
timestamp, preserves the selected-Recipe persistence failure substitution, and
keeps `SelectedRecipeSummary` setter side effects and Recipe option
notifications. The old inline summary/detail/report/preview construction and
library count branch were removed. The new summary projection contract passed
in Debug and Release; existing workspace policy and lifecycle projection
contracts also passed in both configurations. App and Runner builds passed with
0 warnings and 0 errors. ScreenshotSmoke rebuilds retained the existing CS8600
warning at `Program.cs:10722` (1 warning, 0 errors). Monitor-aware Recipe
Manager smoke passed in Debug and Release with `check=OK`,
`layout=0|text=0|internal=0`, size `1600x900`, actual window
`160,66--1760,966`, on the single reported `\\.\DISPLAY2`
(`0,0,1920x1080`, work area `0,0,1920x1032`). The Release PNG and static proof
are recorded under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-recipe-manager-summary-20260907`;
the bounded report is
`docs/reports/OPENVISIONLAB_OVL07_RECIPE_MANAGER_SUMMARY_PROJECTION_20260907.md`.
The full theme/layout/DPI/multi-monitor matrix, broader solution readiness,
Original, commit, push, merge, and deployment remain unverified or untouched.

다음 남은 우선순위: OVL-07 Recipe Manager Pipeline option projection owner 분리 |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-07 Recipe Manager Pipeline option projection owner 분리 — 2026-09-08

The next single refactoring slice moved Recipe Manager Pipeline option
creation, active-first/name ordering, preferred/previous/active selection
fallback, filtering, and Pipeline list count formatting out of the Shell into
the Window-free concrete `OpenVisionRecipePipelineOptionProjectionOwner`.
The owner consumes an explicit inventory/selection snapshot and reuses the
existing `OpenVisionRecipePipelineOption.Create` XML/status projection. It
does not own WPF state, persistence recovery, mutation, pending-edit guards,
PropertyChanged, or Preview/Run execution.

The Shell still reads Pipeline names and active state, performs the empty-
inventory recovery load and persistence-state fallback, assigns
`PipelineOptions`/`FilteredPipelineOptions`, keeps `PipelineFilterText`
binding, and runs `PipelineEditName`, summary, validation identity, recent-run,
and command refresh side effects. The new
`--recipe-manager-pipeline-option-projection-contract` passed in Debug and
Release against an isolated D: data root; summary, workspace policy, and
lifecycle projection regression contracts also passed in both configurations.
Debug/Release app and Runner builds passed with 0 warnings and 0 errors.
The solution Debug/Any CPU build and `OpenVisionReadinessCheck` also passed;
the readiness assertion was updated to recognize the already-completed summary
projection owner delegation instead of requiring a removed direct builder call.
ScreenshotSmoke rebuilds retained the existing CS8600 warning at
`Program.cs:10722` (1 warning, 0 errors). Monitor-aware Recipe Manager smoke
passed in Debug and Release with `check=OK`, `layout=0|text=0|internal=0`, size
`1600x900`, actual window `160,66--1760,966`, on the single reported
`\\.\DISPLAY2` (`0,0,1920x1080`, work area `0,0,1920x1032`). Evidence and the
bounded report are recorded in
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-recipe-manager-pipeline-option-20260908`
and
`docs/reports/OPENVISIONLAB_OVL07_RECIPE_MANAGER_PIPELINE_OPTION_PROJECTION_20260908.md`.
The full theme/layout/DPI/multi-monitor matrix, broader solution readiness,
Original, commit, push, merge, and deployment remain unverified or untouched.

다음 남은 우선순위: OVL-07 Pipeline lifecycle result/status projection owner 분리 |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-07 Pipeline lifecycle result/status projection owner 분리 — 2026-09-08

The next single refactoring slice moved Recipe Manager Pipeline lifecycle
result-to-status projection out of
`Recipe/CommandSurface/PipelineLifecycle.cs` into the
Window-free concrete `OpenVisionRecipePipelineLifecycleProjectionOwner`.
Activate and DuplicateFromSample keep their localized status formats, while
Duplicate/Rename/Delete preserve the existing storage detail text and resulting
Pipeline/fallback names.

The Shell still owns command guards, delete confirmation, pending-edit
transitions, selected Pipeline state, active-state refresh, status binding, and
explicit Preview/Run flow. `OpenVisionRecipePipelineLifecycleUseCase` remains
the only owner of Pipeline storage mutation, active pointer, lifecycle fallback,
and unique-name selection. Recipe/XML and Layer routing contracts are unchanged.

The new `--recipe-pipeline-lifecycle-projection-contract` passed from x64 Debug
and Release runner builds. Existing workspace policy, workspace lifecycle,
summary, and Pipeline option projection contracts also passed in both
configurations. App/Runner Debug and Release builds, solution Debug/Any CPU,
and OpenVisionReadinessCheck passed. Recipe Manager
`wpf_shell_host_recipe_language_controls` smoke passed in Debug and Release
with `check=OK`, `layout=0|text=0|internal=0`, size `1600x900`, and exit 0.
Monitor-aware reruns selected the single `\\.\DISPLAY2` and placed the actual
window rect `160,66--1760,966` inside its work area. The existing ScreenshotSmoke
CS8600 warning remains one warning with zero errors.

Evidence and the bounded report are recorded in
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-lifecycle-projection-20260908`
and
`docs/reports/OPENVISIONLAB_OVL07_PIPELINE_LIFECYCLE_PROJECTION_20260908.md`.
The full theme/layout/DPI/multi-monitor matrix, Pipeline exchange/review,
validation/Step Edit, image/Layer lifetime, Original, commit, push, merge, and
deployment remain separate or untouched.

다음 남은 우선순위: OVL-07 Pipeline exchange/review result projection owner 분리 |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-07 Pipeline exchange/review result projection owner 분리 — 2026-09-08

The next single refactoring slice moved Pipeline XML import/export and
review-bundle export result-to-status projection out of
`Recipe/CommandSurface/PipelineExchange.cs` into the
Window-free concrete `OpenVisionRecipePipelineExchangeProjectionOwner`.
Import preserves the resulting Pipeline name, while XML export and review
bundle export preserve the destination filename. Existing storage detail and
review-bundle failure prefix text remain unchanged.

The Shell still owns file-dialog results, selected-recipe guards, pending Step
edit transitions, selected review references, status binding, refresh order,
and review-bundle dry-run routing. `OpenVisionRecipePipelineExchangeUseCase`
remains the only owner of XML load/save, unique-name selection, active pointer,
bundle construction, and serialization. Recipe/XML, Layer routing, and
explicit Preview/Run behavior are unchanged.

The new `--recipe-pipeline-exchange-projection-contract` passed from x64 Debug
and Release runner builds. Existing workspace policy/lifecycle, summary,
Pipeline option, and Pipeline lifecycle projection contracts passed in both
configurations. App/Runner/ScreenshotSmoke Debug and Release builds,
solution Debug/Any CPU, and `OpenVisionReadinessCheck` passed. The current
`wpf_shell_host_recipe_review_bundle_import` smoke passed in Debug and Release
with `check=OK`, `layout=0|text=0|internal=0`, size `1600x900`; it exercised
review-bundle export setup, tamper rejection, relocation dry-run, and explicit
no-import/Preview/Run behavior. The standalone
`wpf_shell_host_recipe_review_bundle` target still fails its old summary-view
precondition because it expects the Advanced export button before toggling
Advanced review; this is retained as a separate test-maintenance risk.

Static structure proof, documentation index, scoped diff, and new-file
whitespace checks passed. Evidence and the bounded report are recorded in
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-exchange-projection-20260908`
and
`docs/reports/OPENVISIONLAB_OVL07_PIPELINE_EXCHANGE_PROJECTION_20260908.md`.
The full theme/layout/DPI/multi-monitor matrix, review-bundle dry-run state
projection, Pipeline Review execution/result state, validation/Step Edit,
image/Layer lifetime, Original, commit, push, merge, and deployment remain
separate or untouched.

다음 남은 우선순위: OVL-07 Review bundle dry-run result/status projection owner 분리 |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-07 Review bundle dry-run result/status projection owner 분리 — 2026-09-08

The next single refactoring slice moved the three Review bundle dry-run
result-to-status projections out of
Recipe/CommandSurface/LlmXmlDraftWorkflow.cs into the
Window-free concrete OpenVisionRecipeReviewBundleDryRunProjectionOwner.
Inspector failure keeps Succeeded=false; an integrity-success bundle keeps
Succeeded=true whether XML/dependency review is ready or NG, preserving the
existing LoadReviewBundleForDryRun return contract and localized status text.

The Shell/LLM workflow still owns Inspector invocation, integrity/path reports,
loaded inspection state, XML validation, placeholder updates, review-tab
navigation, and the no-import/Preview/Run dry-run guard. The Inspector remains
the owner of bundle integrity/schema/dependency/relocation checks. Recipe/XML,
Layer routing, and explicit Preview/Run behavior are unchanged.

The new recipe-review-bundle-dry-run-projection contract passed in x64 Debug and
Release. Existing workspace, Recipe Manager, Pipeline lifecycle, and Pipeline
exchange projection contracts passed in both configurations. App, Runner, and
ScreenshotSmoke builds passed; the ScreenshotSmoke project retained its
existing CS8600 warning at Program.cs:10722. Solution Debug/Any CPU and
OpenVisionReadinessCheck passed. The current
wpf_shell_host_recipe_review_bundle_import smoke passed in Debug and Release
with check=OK, layout=0|text=0|internal=0, size 1600x900. The single reported
monitor was \\.\DISPLAY2 with bounds 0,0,1920x1080 and work area
0,0,1920x1032; no target process remained after capture.

A first Debug compile exposed CS0136 from two local variables named projection;
the names were separated, the failed logs were preserved, and all later
verification passed. Static structure proof and the bounded evidence are in
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-review-bundle-dry-run-projection-20260908
and
docs/reports/OPENVISIONLAB_OVL07_REVIEW_BUNDLE_DRY_RUN_PROJECTION_20260908.md.
The full theme/layout/DPI/multi-monitor matrix, Pipeline Review execution/result,
validation/Step Edit, image/Layer lifetime, Original, commit, push, merge, and
deployment remain separate or untouched.

다음 남은 우선순위: OVL-07 Pipeline Review result/status projection owner 분리 |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-07 Pipeline Review result/status projection owner 분리 — 2026-09-08

The next single refactoring slice moved Pipeline Review run-level execution
state and overall progress text out of
`OpenVisionPipelineReviewDocument` into the Window-free concrete
`OpenVisionPipelineReviewResultStatusProjectionOwner`. It preserves the
localized lifecycle, validation, start, superseded, failure, completion,
reference-change, run-required, and draining states, together with
OK/NG/WAIT/OFF progress counts and running/stopping prefixes.

The Document still owns dispatcher/View mutations, command guards, validation,
recipe/input revisions, execution-controller calls, and stale-result
suppression. `OpenVisionPipelineReviewExecutionController` remains the owner
of generation/cancellation/cache behavior. Existing
`OpenVisionPipelineReviewResultPresenter` still owns selected-Step result
summary/detail and run-log formatting; `SelectStep` and explicit Preview/Run
behavior are unchanged. Recipe/XML and Layer routing contracts are unchanged.

The new `--pipeline-review-result-status-projection-contract` passed in Debug
and Release and exercised every status plus empty/not-run/count/running/draining
progress branch. Existing Pipeline Review execution, Recipe Manager summary/
Pipeline option, Pipeline lifecycle/exchange, and review-bundle dry-run
projection contracts passed in both configurations. App and Runner Debug/
Release builds, solution Debug/Any CPU, and `OpenVisionReadinessCheck` passed.

`PipelineViewerScreenshotSmoke` Debug/Release builds completed with the
existing `Program.cs:10722` CS8600 warning (1 warning, 0 errors). The normal
and acceptance-NG `wpf_shell_host_pipeline_review` targets passed in Debug and
Release with `check=OK`, `layout=0|text=0|internal=0`, `1600x900`; the current
single monitor was `\\.\DISPLAY2`, bounds `0,0,1920x1080`, working area
`0,0,1920x1032`, and no target process remained. Fresh captures, build logs,
static ownership proof, and the bounded report are recorded in
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-result-status-projection-20260908`
and
`docs/reports/OPENVISIONLAB_OVL07_PIPELINE_REVIEW_RESULT_STATUS_PROJECTION_20260908.md`.
The full theme/layout/DPI/multi-monitor matrix and unrelated validation,
Step Edit, image/Layer lifetime, Original, commit, push, merge, and deployment
remain separate or untouched.

다음 남은 우선순위: OVL-07 Pipeline Review guide/result-detail projection owner 분리 |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-07 Pipeline Review guide/result-detail projection owner 분리 — 2026-09-08

The next single refactoring slice moved selected-Step Pipeline Review guide,
result summary/detail, run-log, pair action/metric composition out of
`OpenVisionPipelineReviewDocument` into the Window-free concrete
`OpenVisionPipelineReviewGuideResultProjectionOwner`. The owner delegates to
the existing Guide and Result presenters, so their localized text and formatting
policy remain the single lower-level source.

The Document still owns selected index/mode, validation, execution-controller
calls, dispatcher/View mutation, Bitmap input lifetime, fixture/designer/scale
updates, and stale-result/lifecycle behavior. Validation-error and running guide
creation now use the same owner. Recipe/XML, Layer routing, and explicit
Preview/Run contracts are unchanged; the owner does not retain or dispose images.

The new `--pipeline-review-guide-result-projection-contract` passed in Debug and
Release across missing-result, OK, tool-NG, acceptance-NG, validation-error,
running, pair action/metric, and run-log cases. Existing Pipeline Review
execution and run-level result/status contracts passed in Debug/Release. Recipe
pipeline exchange, Recipe Manager summary, workspace lifecycle, and review-bundle
dry-run projection contracts passed in Debug. App/Runner Debug/Release builds,
solution Debug/Any CPU, and `OpenVisionReadinessCheck` passed.

`PipelineViewerScreenshotSmoke` Debug/Release builds completed with the existing
`Program.cs:10722` CS8600 warning (1 warning, 0 errors). Normal and acceptance-NG
Pipeline Review targets passed in Debug/Release with `check=OK`,
`layout=0|text=0|internal=0`, `1600x900`. The monitor probe observed the actual
window at `26,26,1600x900` inside single `\\.\DISPLAY2` bounds
`0,0,1920x1080`; no target EXE process remained. Evidence and the bounded report
are recorded in
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-guide-result-projection-20260908`
and
`docs/reports/OPENVISIONLAB_OVL07_PIPELINE_REVIEW_GUIDE_RESULT_PROJECTION_20260908.md`.
The full theme/layout/DPI/multi-monitor and interaction matrix, selected-Step
domain evidence, validation/Step Edit, image/Layer lifetime, Original, commit,
push, merge, and deployment remain separate or untouched.

다음 남은 우선순위: OVL-07 Pipeline Review selected-Step domain evidence projection owner 분리 |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-07 Pipeline Review selected-Step domain evidence projection owner 분리 — 2026-09-08

The next single refactoring slice moved selected-Step Pipeline Review evidence
family policy and same-run summary data projection for object, instance,
geometry, circle, and EdgeBased matcher evidence out of
`OpenVisionPipelineReviewDocument.SelectStep` into the Window-free concrete
`OpenVisionPipelineReviewDomainEvidenceProjectionOwner`. Existing Tool suffix,
space/underscore normalization and `VisionPipelineMultiMatchMeanService`
recognition are preserved, and the projection keeps the original summary
collection references.

The Document still owns selected index/mode, validation, execution-controller
calls, dispatcher/View mutation, input/output image lifetime, and stale-result
suppression. The View still owns grid selection, rendering, image clone/dispose,
circle residual evidence, and matcher presenter usage. View setter order,
fixture/designer/scale refresh, Recipe/XML and Layer routing, and explicit
Preview/Run behavior are unchanged. The readiness source check now recognizes
same-run matcher evidence through the new owner boundary.

The new `--pipeline-review-domain-evidence-projection-contract` passed in Debug
and Release and checked all five family policies, unsupported-tool fail-closed
behavior, and collection identity. Existing Pipeline Review guide/result,
result/status, execution and Recipe exchange/review-bundle contracts passed in
Release. App, Runner, solution and readiness builds passed with 0 warnings and
0 errors. A concurrent Release app build once hit the known WPF generated-file
`CS2001` race; the failed log is retained and a sequential rebuild passed.

`PipelineViewerScreenshotSmoke` Debug/Release builds retained the existing
`Program.cs:10722` CS8600 warning (1 warning, 0 errors). Normal and acceptance-
NG Pipeline Review targets passed in Debug/Release with `check=OK`,
`layout=0|text=0|internal=0`, `1600x900`; fresh dynamic monitor evidence records
the single `\\.\DISPLAY2` (`0,0,1920x1080`, work area `0,0,1920x1032`). Evidence
and the bounded report are recorded in
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-domain-evidence-projection-20260908`
and
`docs/reports/OPENVISIONLAB_OVL07_PIPELINE_REVIEW_DOMAIN_EVIDENCE_PROJECTION_20260908.md`.
The full theme/layout/DPI/multi-monitor and interaction matrix, image/Layer
lifetime, validation/Step Edit, Original, commit, push, merge, and deployment
remain separate or untouched.

다음 남은 우선순위: OVL-07 Pipeline Review image/Layer lifetime owner boundary |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-07 Pipeline Review image/Layer lifetime owner boundary — 2026-09-08

The next single refactoring slice moved Pipeline Review layer-image acquisition
out of the Document's borrowed-image resolvers into the Window-free concrete
`OpenVisionPipelineReviewLayerImageOwner`. Display previews now use the existing
lease-backed `GetLayerImageSnapshot` path, reject the existing placeholder, and
fall back to a cloned review-cache image. Step output keeps the prior
review-cache-first preference. Every synchronous Document, scale-calibration,
and fixture-projection consumer now scopes the returned `Bitmap` with `using`.

The execution controller remains the owner of review-run summaries and cached
output images. Its Reset/Close generation, cancellation, stale-result
suppression, Recipe/XML exchange, Layer routing, and explicit Preview/Run
contracts remain unchanged. The View/ViewModel still clone into WPF or owned
state and dispose through their existing lifecycle.

The new `--pipeline-review-layer-image-owner-contract` passed in Debug and
Release, covering display/cache preference, placeholder fallback, missing-layer
fail-closed behavior, and snapshot validity after Layer replacement/removal.
App and Runner Debug/Release builds passed sequentially with 0 warnings and 0
errors. Existing ImageSpace snapshot, Pipeline Review execution, and Recipe
exchange contracts, the solution build, and readiness check passed. Screenshot
Smoke builds had 0 errors and retained the existing `Program.cs:10722` CS8600
warning. Debug/Release normal and acceptance-NG Pipeline Review UI smoke all
passed with `check=OK`, `layout=0|text=0|internal=0`, `1600x900`; fresh monitor
evidence records one `\\.\DISPLAY2` at `0,0,1920x1080` with working area
`0,0,1920x1032`. Static ownership proof and all evidence are recorded in
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-layer-image-owner-20260908`
and
`docs/reports/OPENVISIONLAB_OVL07_PIPELINE_REVIEW_LAYER_IMAGE_OWNER_20260908.md`.

The full theme/layout/DPI/multi-monitor and interaction matrix, review-cache
retirement/Reset-Close ordering, Original, commit, push, merge, and deployment
remain separate or untouched.

다음 남은 우선순위: OVL-07 Pipeline Review review-cache retirement/Reset-Close owner boundary |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-07 Pipeline Review review-cache retirement / Reset-Close owner boundary — 2026-09-08

The next single refactoring slice kept
`OpenVisionPipelineReviewExecutionController` as the owner of Review summaries
and cached output images while closing the borrowed-`Bitmap` lifetime gap.
`AcquireCachedOutputSnapshot` now clones under `executionSync`; replacement and
Reset/Close cache retirement dispose under the same boundary. Summary dictionary
reads/writes also use that lifecycle lock so clearing cannot overlap enumeration.
The raw `ResolveCachedOutput` path was removed.

`OpenVisionPipelineReviewLayerImageOwner` now consumes the synchronized snapshot
provider and closes the provider clone after its own clone. The Document, View,
Recipe/XML, Layer routing, explicit Preview/Run, execution generation,
cancellation, and stale queued-callback behavior remain unchanged.

The new `--pipeline-review-cache-lifetime-contract` passed in Debug and Release,
covering replacement, Reset retirement, Close retirement, and continued use of
caller-owned snapshots. The existing layer-image owner contract also passed in
Debug and Release. App and Runner Debug/Release sequential builds passed with
0 warnings and 0 errors; the solution and readiness checks passed with 13/13
contracts. Existing Pipeline Review execution, ImageSpace snapshot, and Recipe
exchange contracts passed in Release. ScreenshotSmoke Release built with 0
warnings and 0 errors. Release normal and
acceptance-NG Review smoke both passed with `check=OK`,
`layout=0|text=0|internal=0`, `1600x900`; dynamic monitor evidence records one
`\\.\DISPLAY2` at `0,0,1920x1080` with work area `0,0,1920x1032`. Static proof,
documentation-index validation, and all evidence are recorded in
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-cache-retirement-20260908`
and
`docs/reports/OPENVISIONLAB_OVL07_PIPELINE_REVIEW_CACHE_RETIREMENT_20260908.md`.

The full theme/layout/DPI/multi-monitor and interaction matrix, an atomic race
between Reset and an already-entered UI callback, Original, commit, push,
merge, and deployment remain separate or untouched.

다음 남은 우선순위: OVL-07 Pipeline Review stale callback atomic application boundary |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-07 Pipeline Review stale callback atomic application boundary — 2026-09-08

The next single refactoring slice closed the narrow race between Pipeline
Review UI callback stamp validation and its observable summary/cache/event
application. `OnStepExecutionUpdated` now validates the execution stamp and
applies the summary, review-cache update, and `StepUpdated` notification under
one `executionSync` boundary. Completion validation and `CompleteRun` use the
same boundary, so Reset/Close either waits for an entered callback or
invalidates callbacks before they enter.

The execution controller remains the owner of generation, cancellation,
summaries, and review-cache state. Recipe/XML exchange, Layer routing, explicit
Preview/Run, image snapshot ownership, cache retirement, and queued stale
callback rejection remain unchanged. No new interface, wrapper, message bus,
or UI owner was added.

The new `--pipeline-review-stale-callback-contract` passed in Debug and
Release. It holds `StepUpdated`, starts Reset concurrently, verifies Reset
waits for the entered callback boundary, then repeats the probe for Close, and
verifies final summary/cache state is empty in both cases. Existing Pipeline
Review execution, cache-lifetime, and layer-image owner contracts passed in
Release. App/solution/Runner builds passed with 0
warnings and 0 errors where recorded; readiness passed 13/13. Release normal
and acceptance-NG Pipeline Review UI smoke passed with
`check=OK`, `layout=0|text=0|internal=0`, `1600x900`; dynamic monitor evidence
records one `\\.\DISPLAY2` at `0,0,1920x1080` with working area
`0,0,1920x1032`. Static proof, documentation-index validation, and all fresh
evidence are recorded in
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-stale-callback-20260908`
and
`docs/reports/OPENVISIONLAB_OVL07_PIPELINE_REVIEW_STALE_CALLBACK_20260908.md`.

The full theme/layout/DPI/multi-monitor and interaction matrix, Document-level
result projection revision guarding, Original, commit, push, merge, and
deployment remain separate or untouched.

다음 남은 우선순위: OVL-07 Recipe CommandSurface History/Validation orchestration 경계 |
Recommended model: gpt-5.6-terra | Reasoning effort: high.

## Current Matching Skill State

- `openvisionlab-rule-based-teaching` owns multi-stage intent, datum, ROI/frame,
  Tool-chain, ordering, and cross-stage evidence composition.
- `openvisionlab-matching-teaching` owns Matching/EdgeBasedMatching registration,
  correspondence, finite global selection, detection-power, Takt study, and the
  matching evidence packet. Version `0.2.1` is lifecycle state `active` after
  explicit operator approval and synchronizes the machine-readable packet,
  registration example, and registry validator. The `0.2.0` undeclared-Takt
  forward result remains its behavioral baseline. Active skill status does not
  qualify a Matching candidate or production Recipe.
- Machine-readable ownership and permissions are in
  `docs/contracts/openvisionlab/OPENVISIONLAB_RULE_BASED_SKILL_REGISTRY.json`.
- Do not create Blob/Contour/Line specialist skills until a separately admitted
  repeated responsibility and evidence contract exists.

## Current Rule-Based Skill Development Gate

- The reusable skill-development work contract is
  `docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_DEVELOPMENT_WORK_CONTRACT_20260831.md`.
- The approved three-round research architecture is
  `docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_RESEARCH_PLAN_20260831.md`.
- The 2026-08-31 admission review still found no new algorithm-family
  specialist. After the user authorized documentation and development start,
  `openvisionlab-recipe-xml-handoff 0.1.1` was admitted as one bounded
  non-algorithm `candidate` that serializes an already reviewed general
  teaching envelope to XML and a validation handoff.
- The candidate is explicit-only, omitted from normal registry dispatch, and
  prohibited from raw-image reinterpretation, Import, Preview/Run, Recipe
  mutation, and qualification. Version `0.1.2` preserved the v1 handoff schema
  and added exact status precedence, authority/path/hash propagation,
  operator-lock serialization, specialist frame/parameter and NormalizeImage
  gates, immutable-baseline preservation, and artifact identity/order checks.
  Version `0.1.3` added closed-world evidence discipline and exact Tool-plan,
  frame, ROI, optional-field, and gate-claim constraints. Version `0.1.4`
  adds a transient canonical decision ledger, upstream ownership preflight,
  evidence-gated `PASS`, retained-validator/artifact hard stops, opaque
  baseline preservation, and exact public reason vocabulary. Version `0.1.6`
  adds executable clear/red artifact finalization, one-XML/static-report and
  first-validator binding, blocked `PENDING/UNKNOWN` reference projection,
  inline `PROPOSED` precedence, baseline pointer byte checks, and public
  reason-alias rejection. Version `0.1.7` adds generic ROI authority-lock
  ownership matching and explicit status/reason precedence guidance. Version
  `0.1.8` adds baseline-first status/reason projection, non-blocking upstream
  PROPOSED handling for passed file-backed measurement-only XML, and
  fail-closed multi-constraint routing. Version `0.1.9` adds failure-derived
  evidence-language, frame/layer, graph-exclusion, owner-provenance,
  Contour/projection, per-input observation, final-state, and public-contract
  conflict locks plus matching validator guards. Version `0.1.10` adds the
  explicit datum/calibration, algorithm-gap, unsupported-semantic,
  per-image-override versus acceptance-mutation, and upstream-stage-fail versus
  graph-review precedence locks. The two existing active skills remain
  unchanged.
- The frozen Round 1 benchmark for candidate `0.1.3` is complete and `FAIL`;
  only product-action and session-consistency gates passed. The v0.1.4 and
  v0.1.5 correction benchmarks are also complete and `FAIL`; their results are
  recorded in their respective Round 1 reports. The v0.1.5 run improved red
  fail-closed and session consistency but regressed structure, unsupported or
  invented findings, and reviewer pass. All candidates remain inactive and
  outside normal dispatch. Round 2 is not admitted.
- The v0.1.5 failure triage is recorded in
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_TRIAGE_20260902.md`.
  It defined a mandatory new-corpus re-freeze gate before any benchmark rerun
  and a separate candidate-only v0.1.6 correction proposal. Gate A is complete
  in the new corpus identity and is recorded in
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_CORPUS_REFREEZE_20260903.md`.
  Gate B is complete for its focused candidate-only checkpoint in
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_FOCUSED_VALIDATION_20260903.md`.
  The fresh v0.1.6 Round 1 admission attempt is recorded separately as
  incomplete (Authors/audit complete; Reviewer guard-blocked) in
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_ROUND1_RESULT_20260903.md`;
  no activation, normal dispatch, product action, or qualification is implied.
- Candidate-only `0.1.7` focused correction and forward validation are recorded
  in `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_FOCUSED_VALIDATION_20260903.md`;
  this does not admit a new benchmark or resolve the frozen contract/scorer
  prerequisites.
- Candidate-only `0.1.8` focused correction and regression evidence are recorded
  in `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_FOCUSED_VALIDATION_20260903.md`.
  The candidate remains explicit-only, outside normal dispatch, inactive, and
  unqualified.
- Candidate-only `0.1.9` correction and focused validation are recorded in
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_9_FOCUSED_VALIDATION_20260904.md`.
  It passes 24 focused tests, Skill Creator validation, and Python compilation;
  the candidate remains explicit-only, outside normal dispatch, inactive, and
  unqualified.
- Candidate-only `0.1.10` public-reason precedence correction and focused
  validation are recorded in
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_FOCUSED_VALIDATION_20260904.md`.
  It passes 25 focused tests, Skill Creator validation, and Python compilation;
  the candidate remains explicit-only, outside normal dispatch, inactive, and
  unqualified.
- The separately admitted v0.1.10 Round 1 execution completed 112 Authors,
  audit `PASS`, and 16 independent Reviews, but final scoring is `FAIL`:
  structure `100/112`, critical red fail-closed `20/32`, clear Reviewer tasks
  `14/16`, unsupported/invented findings `20`, product actions `0`, and session
  consistency `79/80`. This remains historical failure evidence; the root is
  immutable and no activation or qualification is implied.
  Evidence:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_ROUND1_RESULT_20260904.md`.
- The isolated runner/scorer correction is recorded in
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_ROUND1_RUNNER_SCORER_CORRECTION_20260903.md`.
  It fixes attempt-root pinning, exact declared red-team mismatch handling,
  and scan-root-aware caching in a D-drive fixture only; the frozen v0.1.6
  result remains `INCOMPLETE` and no new benchmark identity is admitted.
- The contract/corpus reconciliation preflight is recorded in
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_CONTRACT_CORPUS_RECONCILIATION_PREFLIGHT_20260903.md`.
  It explicitly declines to rewrite S3-02 or S4-01 without new semantic
  evidence. The follow-up generic reason projection and strict-baseline
  identity are complete in
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_REASON_PROJECTION_DECISION_20260903.md`
  and
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_STRICT_BASELINE_PREFLIGHT_20260903.md`.
- The new strict-baseline corpus is D-drive-only at
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v017-strictbaseline-20260903`.
  It has 16 clear + 32 red cases, 112 planned attempt metadata records,
  17 exact local baseline evidence files, freeze/contract/case checks with no
  issues, direct and wrapper baseline-validator `PASS`, and static XML
  compatibility `PASS`. The preflight itself did not execute Authors or
  Reviewers; the later admitted execution and its incomplete result are
  recorded in
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_ROUND1_RESULT_20260903.md`.
- The v0.1.8 strict-baseline preflight is a separate D-drive identity at
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v018-preflight-20260903-r1`.
  It has 16 clear + 32 red cases, a final freeze record written before
  `prepare`, 112/112 metadata freeze-hash matches, preserved v0.1.7 baseline
  evidence, static compatibility PASS, and zero author/reviewer artifacts
  before execution. The complete preflight is recorded in
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_STRICT_BASELINE_PREFLIGHT_20260903.md`.
  Its incomplete backend-blocked result is historical. The recovered v0.1.8
  execution used
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v018-backend-recovery-20260904-r2`:
  it completed 112 Authors, audit PASS, 16 Reviews, and a final scorer FAIL.
  The authoritative result is recorded in
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_BACKEND_RECOVERY_ROUND1_RESULT_20260904.md`.
- The v0.1.10 strict-baseline preflight is a separate D-drive identity at
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v010-contract-recovery-20260904`.
  It has the final freeze before `prepare`, preserved v0.1.7 baseline,
  direct/projected validator PASS, static compatibility PASS, harness/runner
  and compilation PASS, 112/112 metadata freeze-hash matches, and zero
  non-metadata attempt files before execution. The admitted execution then
  completed Authors `112/112`, audit `PASS`, and Reviews `16/16`; final scoring
  is `FAIL` (structure `100/112`, critical red `20/32`, unsupported/invented
  findings `20`, clear Reviewer tasks `14/16`, product actions `0`, session
   consistency `79/80`). Evidence:
   `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_STRICT_BASELINE_PREFLIGHT_20260904.md`
   and
   `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_ROUND1_RESULT_20260904.md`.
- The candidate-only v0.1.11 strict corpus is frozen and preflighted at
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v011-strict-corpus-20260904`.
  It has strict `Main -> SourceFrame` baseline compatibility, explicit S4
  `FIXTURE_MIN_VALID_PIXEL_RATIO=0.25` ownership, contract/self-test/direct and
  projected baseline/static validation PASS, harness/runner/compile PASS,
  `prepare` PASS, and exact 112/112 metadata freeze-hash matches with no
  attempt artifacts. Authors, Reviewers, audit, scoring, activation, and
  product execution were not run. Evidence:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_STRICT_CORPUS_PREFLIGHT_20260904.md`.
- The separately admitted v0.1.11 Round 1 execution completed Authors `112/112`
  with audit `PASS`, independent Reviews `16/16`, and final scorer `FAIL`:
  structure `108/112`, unsupported/invented findings `16`, critical red
  fail-closed `29/32`, clear Reviewer tasks `14/16`, product actions `0`, and
  session consistency `77/80`. The candidate remains explicit-only, inactive,
  and unqualified. Evidence:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_ROUND1_RESULT_20260904.md`.
- Candidate creation and active promotion remain separate explicit approvals.

## Commercial Lessons To Retain

- Keep the shortest operator path visible: teach once, explicitly run, inspect
  drawings/metrics, validate a bounded set, and save the Recipe.
- Preserve clear accepted/rejected reasons, click-to-highlight evidence,
  Recipe management, and deterministic replay.
- Do not use commercial-product comparisons as permission to add hardware,
  account, cloud, or equipment-integration scope.

## Current Evidence

- Repository flattening:
  `docs/reports/OPENVISIONLAB_DEV_REPOSITORY_PATH_MIGRATION_20260830.md`.
- Full document audit, priority decision, and dirty-slice verification:
  `docs/reports/OPENVISIONLAB_PROJECT_DOCUMENT_AUDIT_AND_PRIORITY_20260830.md`.
- Matching skill responsibility split:
  `docs/reports/OPENVISIONLAB_MATCHING_SKILL_SPECIALIZATION_20260830.md`.
- Matching undeclared-Takt behavioral-forward validation:
  `docs/reports/OPENVISIONLAB_MATCHING_SKILL_UNDECLARED_TAKT_FORWARD_VALIDATION_20260830.md`.
- Matching `0.2.1` machine-readable contract synchronization:
  `docs/reports/OPENVISIONLAB_MATCHING_SKILL_CONTRACT_SYNC_20260830.md`.
- Rule-Based Skill Research Round 1~3 design:
  `docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_RESEARCH_PLAN_20260831.md`.
- Recipe XML handoff candidate implementation and focused evidence:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_CANDIDATE_20260831.md`.
- Recipe XML handoff `0.1.2` focused correction and validation:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_2_FOCUSED_VALIDATION_20260901.md`.
- Recipe XML handoff `0.1.2` frozen Round 1 rerun result:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_2_ROUND1_RESULT_20260901.md`.
- Recipe XML handoff `0.1.3` focused correction and validation:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_3_FOCUSED_VALIDATION_20260901.md`.
- Recipe XML handoff `0.1.3` frozen Round 1 rerun result:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_3_ROUND1_RESULT_20260901.md`.
- Recipe XML handoff `0.1.3` failure triage:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_3_TRIAGE_20260901.md`.
- Recipe XML handoff `0.1.4` frozen Round 1 benchmark result:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_ROUND1_RESULT_20260902.md`.
- Recipe XML handoff `0.1.4` failure triage and minimum correction scope:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_TRIAGE_20260902.md`.
- Recipe XML handoff `0.1.4` correction contract and focused evidence:
  `docs/roadmap/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_CORRECTION_CONTRACT_20260901.md`.
- Recipe XML handoff `0.1.4` focused validation and bounded forward probe:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_FOCUSED_VALIDATION_20260901.md`.
- Recipe XML handoff `0.1.5` C1–C6 focused validation and independent forward
  probe:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_FOCUSED_VALIDATION_20260902.md`.
- Recipe XML handoff `0.1.5` frozen Round 1 benchmark result:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_ROUND1_RESULT_20260902.md`.
- Recipe XML handoff `0.1.5` failure triage and correction decision:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_TRIAGE_20260902.md`.
- Recipe XML handoff `0.1.5` Gate A corpus re-freeze:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_CORPUS_REFREEZE_20260903.md`.
- Recipe XML handoff `0.1.6` candidate-only C7–C10 focused validation:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_FOCUSED_VALIDATION_20260903.md`.
- Recipe XML handoff `0.1.6` Round 1 Authors/audit execution and Reviewer
  guard-blocked pre-review result:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_ROUND1_RESULT_20260903.md`.
- Recipe XML handoff `0.1.6` evidence-triage and correction boundary:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_TRIAGE_20260903.md`.
- Recipe XML handoff `0.1.7` candidate-only generic-ROI/status-precedence
  correction and focused validation:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_FOCUSED_VALIDATION_20260903.md`.
- Round 1 runner/scorer correction checkpoint (isolated D-drive fixture):
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_ROUND1_RUNNER_SCORER_CORRECTION_20260903.md`.
- Contract/corpus reconciliation preflight (read-only):
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_CONTRACT_CORPUS_RECONCILIATION_PREFLIGHT_20260903.md`.
- Generic public reason-code projection decision for the new corpus:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_REASON_PROJECTION_DECISION_20260903.md`.
- v0.1.7 strict-baseline corpus identity and preflight:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_STRICT_BASELINE_PREFLIGHT_20260903.md`.
- v0.1.7 strict-baseline Round 1 result (Authors/audit complete, scorer
  incomplete, Reviewer guard-blocked):
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_ROUND1_RESULT_20260903.md`.
- v0.1.8 strict-baseline Round 1 result (Authors recorded 112 attempts,
  external audit PASS, scorer incomplete from 72 Codex backend failures, and
  Reviewer not launched):
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_ROUND1_RESULT_20260903.md`.
- v0.1.8 backend-recovery Round 1 final result (112 Authors, audit PASS, 16
  Reviews, final scorer FAIL):
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_BACKEND_RECOVERY_ROUND1_RESULT_20260904.md`.
- Recipe XML handoff v0.1.9 candidate-only failure-derived correction and
  focused validation:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_9_FOCUSED_VALIDATION_20260904.md`.
- Recipe XML handoff v0.1.10 candidate-only public-reason precedence correction
  and focused validation:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_FOCUSED_VALIDATION_20260904.md`.
- Recipe XML handoff v0.1.11 strict SourceFrame corpus identity and candidate-only
  preflight:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_STRICT_CORPUS_PREFLIGHT_20260904.md`.
- Recipe XML handoff v0.1.11 Round 1 execution result (Authors, audit, 16
  Reviews, final scorer `FAIL`):
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_ROUND1_RESULT_20260904.md`.
- Recipe XML handoff v0.1.10 strict-baseline corpus identity and preflight:
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_STRICT_BASELINE_PREFLIGHT_20260904.md`.
- Recipe XML handoff v0.1.10 Round 1 execution result (112 Authors, audit PASS,
  16 Reviews, final scorer FAIL):
  `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_ROUND1_RESULT_20260904.md`.
- Codex backend availability check used before the recovered v0.1.8 identity
  (current binary, D-drive-only ephemeral health probe, exit `0`):
  `docs/reports/OPENVISIONLAB_CODEX_BACKEND_HEALTH_20260904.md`.
- Rule-Based Skill frozen Round 1 final result:
  `docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_ROUND1_RESULT_20260901.md`.
- PCB Matching extent/Takt study:
  `docs/reports/OPENVISIONLAB_MATCHING_TAKT_SKILL_SESSION_20260830.md`.
- Locator operator-decision and UI boundary:
  `docs/reports/OPENVISIONLAB_LOCATOR_RELATIVE_BLOB_REVIEW_DECISION_UI_WIRING_20260829.md`.
- Matching held-out/performance boundary:
  `docs/reports/OPENVISIONLAB_MATCHING_SCALE_HELDOUT_PERFORMANCE_20260829.md`.
- Last grouped Dev checkpoint:
  `docs/reports/OPENVISIONLAB_DEV_VERSIONED_PUSH_CHECKPOINT_20260828.md`.

## Non-Regression Owners

- Work rules and product boundary: `AGENTS.md`.
- Stable UI/runtime behavior:
  `docs/contracts/openvisionlab/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`.
- Product/view ownership:
  `docs/roadmap/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`.
- Task reading routes: `docs/LLM_DOCUMENT_INDEX.json`.
- Release rules:
  `docs/contracts/openvisionlab/OPENVISIONLAB_RELEASE_VERSION_POLICY.md`.

## Restart Checklist

0. Check the latest user-directed stop boundary above. Without a new explicit
   user request, do not restart implementation, commentary edits, documentation
   updates or verification merely because an older entry lists a next task.
1. Work in `C:\Git\2D\Dev` and run `git status --short` plus
   `git log --oneline -5`.
2. Read `AGENTS.md`, `docs/README.md`, and this file.
3. Load only the matching `routes[].read` list from
   `docs/LLM_DOCUMENT_INDEX.json`.
4. State product identity, evidence-based maturity, immediate and remaining
   priority, commercial lessons, and excluded scope before changing anything.
5. Do not touch `C:\Git\2D\Original`, commit, push, tag, publish, or deploy
   unless the user explicitly authorizes that exact stage.

## Current final MVVM/Partial/folder/module recheck — 2026-09-12

Status: `PL-0020 M5 VERIFIED`; all five requested boundaries are complete for the source/audit scope.

The source-wide audit reports 816 C# files, 60 XAML files, 59 actual partial
 declarations, 27 projects, 34 project references, and zero project cycles. The
 two `partial class` text matches are smoke contract text, not declarations. The
 partial declarations are generated/framework composition or cohesive existing
 ToolView/Learn/Shell/View types; no manual business partial sprawl was found that
 can be removed without changing ownership or binding contracts.

The dense folders are responsibility-cohesive: Recipe/Review (34 C# files),
VisionTest/Wpf/Learn (27), and Recipe/IntentSkills (21). No physical folder move
met the ownership/dependency threshold. `OpenVisionShellHostView` and the
10,076-line `RecipeCommandSurface` remain concrete canonical owners because their
state, binding contracts, and callback lifetimes are shared. The known
`RoiImageCanvasViewModel` UI/native/path coupling remains explicit technical debt;
M5 found no new defect or changed boundary that justifies reopening PL-0019.

The shortest developer route is `AGENTS.md -> docs/README.md -> Program.Main ->
OpenVisionLabApplication.Run -> OpenVisionShellHostWindow ->
OpenVisionShellHostView -> Core/Pipeline/Execution -> RecipeCommandSurface child
owners -> Pipeline Review Document/ExecutionController/LayerImageOwner -> focused
contract -> smoke target`.

M5 checks: `TestDocumentationIndex.ps1` PASS (`IndexedPaths=294`, `Routes=16`,
`RootRedirects=102`), `Invoke-RefactorAudit.ps1 -Verify` PASS
(`CSharpFiles=816`, `XamlFiles=60`, `PartialDeclarations=59`,
`PartialTextMatches=2`, `ViewModelUiIoFiles=1`, `ProjectCycles=0`,
`ShellStorageCalls=0`), solution Debug build PASS (0 warnings, 0 errors), solution
Release build PASS (0 warnings, 0 errors), and `git diff --check` PASS. Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\junior-burden-recheck-20260912\final-audit\phase-summary.txt`.

This source/audit completion does not prove runtime WPF visual states, alternate
 themes, Wide/Compact layouts, DPI/monitor/input matrices, camera/SDK/GPU/device
 behavior, native permanent-hang recovery, or long-running shutdown. Those remain
 explicitly unverified and require the corresponding runtime or hardware path.

Next separate priority: only reopen an owner for a new reproducible defect,
changed requirement, failed criterion, or dependency-boundary change; otherwise
move to the next user-facing feature from the current product handoff.
