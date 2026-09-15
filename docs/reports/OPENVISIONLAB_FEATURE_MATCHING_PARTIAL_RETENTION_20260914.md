# OpenVisionLab Feature Matching Tool View Partial Retention

Updated: 2026-09-14 KST  
Issue: `PL-0038`  
Status: `Complete` — source-only no-change structural audit; no production source change

## Scope and decision

This slice reviewed `FeatureMatchingToolWpfView.xaml.cs` and its XAML Partial
from the native-tool factory through composition, ViewModel/property state,
shared matching runtime, binding/test facade, and disposal path.

No production split was justified. `FeatureMatchingToolWpfView` is already a
thin XAML/composition adapter. The existing `FeatureMatchingToolViewModel`
owns the mutable `FeatureMatchingProperty` policy and template normalization;
the shared matching controller/runtime own WPF event, PropertyGrid, preview,
result-review, preset, language, and disposal state; and the factory and base
View own creation and release. Adding another ViewModel, wrapper, service, or
Partial would duplicate or hide those owners rather than create an independent
state, lifetime, or test seam.

The `partial` declaration is therefore retained as a required XAML namescope
boundary. This is consistent with the repository's hybrid MVVM model: the View
does not own business state or persistence, while concrete controller/runtime
adapters own WPF-specific interaction and lifetime around the ViewModel-facing
PropertyGrid contract.

## Owner map and call path

| Concern | Current owner | Intended owner | Evidence |
| --- | --- | --- | --- |
| XAML namescope and shell composition | `FeatureMatchingToolWpfView` | same View Partial | `InitializeComponent`, `toolShell`, shared shell properties |
| Property/preview/result facade | `VisionToolSingleInputMatchingToolController<FeatureMatchingProperty>` | same existing controller | `AttachPropertyToolController`, `CreateProperty`, `SetResultReview`, template/test delegates |
| Mutable Feature Matching property and normalization | `FeatureMatchingToolViewModel` | same ViewModel | `FeatureMatchingProperty property`, `ConfigureDefaults`, `Normalize`, `CreateProperty` |
| PropertyGrid/template/review/preset state | `VisionToolMatchingPropertyRuntime<TProperty>` and shared matching runtime | same existing runtimes | PropertyGrid host, template callback, delayed preview, criteria/review and `Dispose` |
| WPF event and language lifetime | `VisionToolSingleInputMatchingToolController<TProperty>` | same existing controller | `VisionToolSingleInputToolEventHub`, language controller, runtime disposal |
| Tool creation and persistence callback | `OpenVisionNativePropertyGridToolFactory` + `VisionToolCompositionService` | same composition owners | factory -> ViewModel -> presenter -> View; persistence callback remains in document builder |
| Base controller release | `VisionToolSingleInputPropertyToolViewBase` | same base View lifetime owner | `DisposeToolResources()` then `toolController?.Dispose()` |

Shortest reading order:

```text
OpenVisionNativePropertyGridToolFactory.CreateFeatureMatching
  -> VisionToolCompositionService.CreateFeatureMatchingToolViewModel
  -> FeatureMatchingToolViewModel / VisionToolPropertyGridPresenter
  -> FeatureMatchingToolWpfView
  -> VisionToolSingleInputMatchingToolController<FeatureMatchingProperty>
  -> VisionToolSingleInputMatchingToolRuntime
  -> VisionToolMatchingPropertyRuntime / base View lifetime
```

Runtime property flow:

```text
Shell selects Feature Matching
  -> factory creates the existing property and ViewModel
  -> presenter supplies the ViewModel/property contract to the View
  -> View Partial attaches the shared matching controller
  -> PropertyGrid changes route through presenter callbacks
  -> ViewModel normalizes/template-loads and the document owner persists
  -> shared runtime updates summary, preview, and result review
```

## Boundary findings

- The Feature Matching View Partial contains no direct `System.IO`, file
  dialog, settings-store, algorithm construction, or template-image reload
  call. Its public/internal test surface delegates to the existing controller.
- The XAML contract (`toolShell`, Learn Feature action/topic 10, visible
  template status) remains explicit and unchanged.
- `FeatureMatchingToolViewModel` owns the mutable property object and template
  defaults/normalization. The View does not reach into that state directly.
- The shared matching runtime owns PropertyGrid change routing, persistence
  callbacks, delayed preview scheduling, result-review presentation, preset
  application, and disposal. Splitting a subset from the View would create a
  second state or lifetime owner.
- The base View owns the final controller release. The Feature Matching View
  correctly does not duplicate `Dispose` or `DisposeToolResources`.

## Verification

Focused source contract:

```text
FEATURE_MATCHING_PARTIAL_BOUNDARY_CONTRACT=PASS|checks=9
```

The contract passed in both Debug and Release. It checks the XAML/composition
boundary, absence of direct View persistence/dialog/algorithm coupling, XAML
binding and test facades, ViewModel state ownership, shared runtime/controller
state and lifetime, presenter callback routing, factory/composition path, and
base View release ownership.

Builds and runtime smoke:

- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj --configuration Debug --no-restore --nologo` — 0 warnings, 0 errors.
- Same command with `--configuration Release` — 0 warnings, 0 errors.
- `PipelineViewerScreenshotSmoke` x64 Debug build — 0 errors, 2 pre-existing `MSB3270` MSIL/AMD64 warnings.
- Dynamic monitor detection before EXE launch: one monitor, `\\.\DISPLAY2`, bounds `1920x1080`, working area `1920x1032`; the reported single screen was used unchanged.
- A fresh smoke process window probe recorded one visible top-level window at
  `Left=182,Top=182,Right=1782,Bottom=1082` intersecting `\\.\DISPLAY2`.
  Geometry evidence is
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\feature-matching-partial-retention-20260914\monitor-geometry\monitor-window-geometry.json`.
- `wpf_shell_host_feature_matching_tool` — `OK`, `1600x900` PNG. The rendered
  shell showed the Feature Matching image/result, template status, verification
  guide, Ratio/RANSAC PropertyGrid rows, result review, and action row without
  layout/text/overlap assertions failing.

Repository gates also passed: `OpenVisionReadinessCheck` Debug/Release (13/13
each), `Invoke-RefactorAudit.ps1 -Verify`
(`CSharpFiles=832|XamlFiles=57|PartialDeclarations=54|PartialTextMatches=3|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`),
`TestDocumentationIndex.ps1`
(`IndexedPaths=307|Routes=17|RootRedirects=102`), PL-0038 JSON/schema
validation, and `git diff --check` (existing LF/CRLF normalization notices
only). Complete phase evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\feature-matching-partial-retention-20260914`.

Native file-dialog click/file association, all themes, DPI 125/150/175/200%,
alternate monitor topology, camera/SDK/GPU, and long-running native runtime
remain unverified. Source conclusion: `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Completion record

```text
Status: Complete
Scope: Feature Matching Tool View XAML Partial owner audit; no production split
Acceptance criteria: owner/call path/state/lifetime/public contract documented; source contract 9/9 Debug+Release; focused WPF smoke passed
Verification: VisionRecipeRunnerSmoke Debug/Release build+contract; PipelineViewerScreenshotSmoke x64 Debug build and Feature Matching target; readiness/refactor/documentation/diff checks
Evidence: this report; .proofline/issues/PL-0038.json; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\feature-matching-partial-retention-20260914
Boundary / next dependency: broader WPF/theme/DPI/input/native/hardware qualification remains unverified; reopen only for a new requirement, defect, failed criterion, or changed dependency boundary
```
