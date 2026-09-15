# OpenVisionLab 2D partial owner proof

Updated: 2026-09-11 KST

This file contains the P2 baseline and the reopened P3/P4 structural proof for
[`OPENVISIONLAB_PARTIAL_STRUCTURE_PLAN_20260911.md`](OPENVISIONLAB_PARTIAL_STRUCTURE_PLAN_20260911.md).
The goal is to structure partial types only when a concrete owner can take the
responsibility with its own state, dependency direction, lifetime, and observable
contract. A file count reduction by itself is not a successful refactor.

## P2 baseline result

The P1 inventory found 108 actual partial declarations (110 raw tokens, including
two smoke-contract strings), across 65 partial types. P2 rechecked the cohesive
families that could plausibly be mistaken for independent modules. No new
production owner is justified by the current code. The generated/XAML and
same-lifetime partials remain deliberately unchanged.

The original source evidence is stored at:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-p2-20260911\p2-owner-proof.md`

## Reopened P3/P4 result

The user explicitly reopened the requirement to structure every existing partial.
The implementation therefore applied the smallest source changes that reduce
manual file sprawl without inventing a new owner:

- `OpenVisionShellHostDockedLayerOrchestrator`: four manual partial files were
  merged into the concrete root; the existing composition remains the state and
  lifetime owner.
- `OpenVisionDockWorkspaceController`: eight manual partial files were merged
  into the concrete root; all AvalonDock mutation and cleanup remain in the same
  controller.
- `OpenVisionLayerDockingGestureController`, `RoiImageCanvasViewModel`,
  `OpenVisionPipelineReviewDocument`, `OpenVisionPipelineReviewView`, and Shell
  View interactions were merged into their existing concrete/XAML owners.
- Single/Double Input Tool Shell layout controllers were merged into the XAML
  code-behind roots as private concrete owners; no public binding changed.
- Recipe CommandSurface files were moved from the flat Wpf directory into
  `Recipe/CommandSurface` and renamed by responsibility. The type name,
  namespace, binding contract, callbacks, and shared mutable state were kept.

The post-change audit reports 80 partial declarations across `src` and `tools`
(76 in product source and 4 in smoke contracts), down from the 110-token baseline.
The latest inventory and focused evidence are in:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-p3-20260911`

`partial-inventory-after.csv` and `partial-owner-map-after.csv` contain one
entry per current declaration with category, caller, mutable-state writer,
lifetime owner, and contract impact.

This is a real owner consolidation and folder organization change; it is not a
claim that every partial must be eliminated.

## Decisions by family

| Family | Decision | Owner and proof |
| --- | --- | --- |
| `OpenVisionShellHostRecipeCommandSurface` | Concrete facade in `Recipe/CommandSurface/RecipeCommandSurface.cs` | The facade owns binding-facing state, command invalidation, recipe selection, callbacks, and Shell lifetime. Existing concrete Recipe owners retain independent workflow state/results; the former 9-file manual partial family is gone. |
| `OpenVisionDockWorkspaceController` | Consolidated to one concrete root | Documents, layout, move, native, normalization, state, and cleanup still mutate the same AvalonDock `DockingManager`/`primaryPane` tree. The root now exposes those responsibilities in named regions without changing the native lifetime. |
| `OpenVisionPipelineReviewDocument` | Consolidated to existing root | Event handlers still use the document's pipeline, view, execution controller, image owner, validation state, selected indexes, and revision gate. The root still owns unsubscribe and release. |
| `OpenVisionShellHostDockedLayerOrchestrator` | Consolidated to existing root | All behavior still forwards through the one composition field and event subscription; the root now contains the related regions. |
| `OpenVisionShellHostView` | XAML root retained; D2 test surface extracted | XAML construction and UI event handling remain in the generated View root. The established smoke/test surface now has the concrete `OpenVisionShellHostViewTestSurface` owner with explicit bindings; root compatibility forwarding preserves the existing public/internal contract without moving Shell lifetime ownership. |
| `RoiImageCanvasViewModel` | Consolidated to existing root | Commands, image/ROI state, timers, OpenGL control, input controllers, and `Dispose` still share Mat/native ownership and event cleanup. |
| `ImageCanvasControl` (3 files) | Keep by framework contract | The designer file is generated. Core and view-state files share the SharpGL context and the control's native release path. |
| `OpenVisionLayerDockingGestureController` | Consolidated to existing root | Source resolution and drag state still share the native manager and callbacks; no resolver wrapper was added. |
| `OpenVisionLayerDockWorkspaceView` | Consolidated manual parts; XAML root retained | XAML, visual, guide, and event parts still mutate the same WPF controls and overlay resources. The remaining `partial` is the required XAML type. |
| `OpenGlDrawing` | D3 concrete renderer owners with public façade | The old static files shared a public call surface and context assumptions. D3 moved their implementation into concrete responsibility owners and retained `OpenGlDrawing` as the compatibility façade; the caller still owns context and resource release. |
| `VisionToolSingleInputPropertyToolShell` / `VisionToolDoubleInputCustomToolShell` | Keep | Their nested layout controllers are already concrete cohesive owners that directly mutate the shell controls and share the shell lifetime. |

The remaining single-file partials are XAML, designer, Settings, or other
framework-required contracts and are retained by the P1 classification.

## Call-path and ownership check

The verified navigation remains:

```text
Program.Main
 -> OpenVisionLabApplication.Run
 -> OpenVisionShellHostWindow
 -> OpenVisionShellHostView
 -> concrete controller/presenter/document/workspace owner
```

For consolidated families, the existing concrete/XAML owner remains the mutable-
state writer and lifetime/release owner; call sites continue to instantiate the
same type. For retained families, the same ownership proof still applies. Static
searches found no stale deleted partial path in the readiness or smoke tools.

## Verification

- P1 inventory and owner map: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-p1-20260911`
- P2 owner proof: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-p2-20260911`
- `dotnet build OpenVisionLab.sln --configuration Debug --no-restore`: passed, 0 warnings/errors
- `dotnet build OpenVisionLab.sln --configuration Release --no-restore`: passed, 0 warnings/errors
- Readiness tool: passed all contracts after updating the moved/merged paths
- Focused smoke contracts: Pipeline Review document revision, Recipe run-history
  orchestration, and Shell Recipe basic-lifecycle view all passed
- `Invoke-RefactorAudit.ps1 -Verify`: `PASS`, 822 C# files, 60 XAML files,
  80 partial declarations, 0 project cycles, 0 Shell storage calls
- Static source search for deleted partial paths and current call paths: passed

`TestDocumentationIndex.ps1` and `git diff --check` remain final P5 gates for the
combined worktree and are recorded separately in the current evidence directory.

This slice did not claim WPF runtime theme/layout/DPI/monitor, camera/SDK, or
long-running native shutdown verification.

## Protected boundaries after P4

`OpenGlDrawing` remains nine responsibility-named files in the OpenGL folder
because all methods share one static rendering context and public call surface;
turning them into forwarding classes would obscure the state rather than clarify
ownership. Recipe CommandSurface remains a partial family for the same reason:
binding-facing state, callbacks, and execution-session state are shared. XAML,
designer, Settings, test-hook, and smoke-contract partials remain by framework or
test contract. These are explicit retained decisions, not unreviewed leftovers.

## Next boundary

P3/P4 source consolidation is complete for the reopened requirement. Reopen a
protected family only for a new requirement, a reproducible defect, a failed
completion criterion, or a changed dependency/lifetime boundary. Do not add
forwarding classes or interfaces merely to reduce the remaining partial count.

## Completion design reopened by the user — 2026-09-11

The user has now explicitly required the remaining manual partial families to be
structured through to completion and asked for the design before implementation.
The previous P5 closure therefore no longer satisfies the current requirement.
It remains valid historical evidence for the source state it covered, but the
current PL-0013 status is **DESIGN IN PROGRESS**.

The current declaration-only recheck found 78 compiled partial declarations and
two smoke-contract string matches. The strings are not declarations and will be
reported separately by the next audit. The remaining manual families are:

- `OpenGlDrawing` — nine rendering files with a public static call surface.
- `OpenVisionShellHostRecipeCommandSurface` — nine binding/workflow files with
  shared state and existing concrete projection owners.
- `OpenVisionShellHostView.TestHooks` — one public/internal test contract file
  that reaches existing Shell test facades and callbacks.

The concrete completion design is recorded in
[`OPENVISIONLAB_PARTIAL_STRUCTURE_COMPLETION_DESIGN_20260911.md`](OPENVISIONLAB_PARTIAL_STRUCTURE_COMPLETION_DESIGN_20260911.md).
It defines D0 contract inventory, D1 audit false-positive separation, D2 test
surface extraction, D3 OpenGL renderer ownership, D4 Recipe workflow ownership,
and D5 protected-boundary closure. Source implementation has not started in this
design slice. The target is 59 protected framework/XAML declarations, zero
manual partial families, and zero partial text false positives in the final
audit report.

## D0/D1 verification update — 2026-09-11

D0/D1 corrected the audit boundary without changing product behavior. The audit
now matches partial declarations only on declaration lines and emits literal
smoke-contract matches separately. The verified result is
`PartialDeclarations=78;PartialTextMatches=2;ProjectCycles=0;ShellStorageCalls=0`.
The corrected declaration and owner maps contain 78 rows and are stored under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d1-20260911`.
The next proof boundary is D2, which must move the Shell TestHooks behavior to a
concrete test-surface owner before the OpenGL and Recipe families are reopened.

## D2 verification update — 2026-09-11

D2 removed the manual `OpenVisionShellHostView.TestHooks.cs` declaration. The
test behavior now has a concrete owner at
`src/OpenVisionLab/UI/Menu/Wpf/Shell/Support/OpenVisionShellHostViewTestSurface.cs`.
The owner receives explicit bindings to the existing mutable-state and lifetime
owners; it does not clone Shell state or introduce a service/factory/interface
layer. `OpenVisionShellHostView.xaml.cs` retains only the root compatibility
forwarders needed by the existing public/internal `*ForTest` contract and the
surface's construction/lifetime wiring.

The actual call path is:

```text
OpenVisionShellHostView constructor
 -> OpenVisionShellHostViewTestSurface(bindings)
 -> existing Shell/Layer/Tool/Recipe owners
 -> existing test contract callers
```

D2 evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d2-20260911`:

- Debug and Release solution builds: PASS, 0 warnings / 0 errors.
- `OpenVisionReadinessCheck`: PASS.
- `VisionRecipeRunnerSmoke --runtime-stability-contract`: PASS.
- Refactor audit: PASS with `PartialDeclarations=77`,
  `PartialTextMatches=2`, `ProjectCycles=0`, `ShellStorageCalls=0`.

The two text matches remain intentional smoke contract strings. Actual desktop WPF
visual states were not run in this bounded slice and remain unverified. D3 is the
next proof boundary: split OpenGL rendering responsibilities
behind the existing public façade while preserving the OpenGL context and display
list lifetime owner.

## D3 verification update — 2026-09-11

D3 completed the OpenGL boundary designed in the completion document. The nine
partial files were replaced by the following concrete owners:

```text
OpenGlDrawing (public façade)
 -> OpenGlColorConverter
 -> OpenGlTextureRenderer
 -> OpenGlShapeRenderer
 -> OpenGlPenRenderer
 -> OpenGlTextRenderer
 -> OpenGlMeasurementRenderer
 -> OpenGlOverlayRenderer
 -> OpenGlOverlayTextRenderer
 -> OpenGlDrawingState
```

The façade preserves all 83 original public static method signatures, the public
`ZoomFactor` field, and the internal `FontGlyphCount` reflection contract. A
source comparison reports original 83 / façade 83 with zero missing or unexpected
signatures. The only cross-file private helper discovered during the build was
`ConvertDotInfoToPoints`; it now belongs to `OpenGlPenRenderer`, which is the
actual caller owner.

The state and release proof is explicit: `ImageCanvasControl` remains the
OpenGL context and `ZoomFactor` writer, `OpenGlTextRenderer` receives the existing
caller-owned font list, `OpenGlOverlayRenderer` updates `CanvasShape.DisplayListId`,
and `OpenGlOverlayExtensions` retains display-list release. No renderer stores a
context or introduces a new Bitmap/Mat/texture lifetime.

D3 evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d3-20260911` and
the design inventory is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d3-design-20260911`.
ImageCanvas and solution Debug/Release builds, VisionRecipeRunnerSmoke
Debug/Release builds, runtime stability contracts, ReadinessCheck Debug/Release,
signature comparison, and refactor audit all passed. The audit now reports
`PartialDeclarations=68;PartialTextMatches=2;ProjectCycles=0;ShellStorageCalls=0`.
GPU-backed visual rendering and monitor/DPI coverage were not run and remain
unverified. D4 is the next proof boundary for Recipe CommandSurface ownership.

## D4 proof — Recipe CommandSurface concrete facade (2026-09-11)

D4 removed the remaining manual Recipe partial family after recording its design in
`OPENVISIONLAB_PARTIAL_STRUCTURE_COMPLETION_DESIGN_20260911.md`. The former 9
source declarations now compile as one concrete
`OpenVisionShellHostRecipeCommandSurface` in
`src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/RecipeCommandSurface.cs`.
Responsibility regions preserve navigation without creating another type layer.

The owner map is explicit:

```text
OpenVisionShellHostView constructor
 -> RecipeCommandSurface concrete facade
 -> XAML binding / RelayCommand
 -> existing Workspace, Pipeline, Validation, Qualification, Review and RunHistory owners
 -> facade notification/status projection
```

The facade remains the mutable binding-state writer (`SetProperty`,
`OnPropertyChanged`, selection fields, execution-session event handlers). Existing
concrete owners remain the file/Recipe/Pipeline/policy writers. The Shell View still
creates and owns the facade and closes the existing event/callback lifetime. No
public type, XAML binding, command name, callback parameter, Recipe XML, Preview/Run
contract, or Dispose order changed.

The deleted physical files were saved before integration in
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d4-design-20260911`.
`source-body-contract.csv` compares each of the eight former partial bodies and the
existing root core by whitespace/region-normalized SHA-256; every row is `match=True`.
The current implementation evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d4-20260911`.

D4 focused verification:

- Solution Debug/Release and VisionRecipeRunnerSmoke Debug/Release builds: PASS,
  0 warnings/errors.
- ReadinessCheck Debug/Release: PASS after updating its source-family reader for
  the consolidated path.
- Recipe contracts: Debug 24/24 PASS, including the 12-case execution-session
  contract in a copied D: runtime; Release 23/23 PASS.
- Refactor audit: `CSharpFiles=815`, `XamlFiles=60`,
  `PartialDeclarations=59`, `PartialTextMatches=2`, `ProjectCycles=0`,
  `ShellStorageCalls=0`.
- Documentation index and `git diff --check`: PASS; only pre-existing line-ending
  conversion warnings were printed by Git.
- `src`/`tools` search found no Recipe partial declaration or stale deleted Recipe
  partial path.

The actual WPF visual tree, themes/layouts/DPI/monitor, GPU rendering,
camera/SDK, and long-running shutdown remain unverified in this source/build slice.
D5 must now freeze the 59 protected framework declarations and final closure
record; it must not reopen the completed Recipe owner without a new defect or
changed boundary.

## D5 final closure — protected partial owner map (2026-09-11)

The final source inventory contains 59 compiled partial declarations and two
literal smoke-contract matches. The 59 declarations are exactly 56 WPF/XAML
code-behind or external smoke XAML contracts, two ImageCanvas native/designer
contracts, and one generated Settings contract. No manual workflow partial remains.
The row-level owner map records file, project, namespace, caller, mutable-state
writer, lifetime/release owner, and binding/public/test contract at:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d5-20260911\protected-partial-inventory.csv`

The final call path remains direct and searchable:

```text
Program.Main -> OpenVisionLabApplication.Run -> OpenVisionShellHostWindow
 -> OpenVisionShellHostView -> concrete feature owner / Recipe facade / XAML View
 -> Layer / Tool / Pipeline / Result / Review owner
```

The final static and build evidence is recorded in the same D5 directory and the D4
implementation directory. RefactorAudit passed with
`CSharpFiles=815;XamlFiles=60;PartialDeclarations=59;PartialTextMatches=2;
ProjectCycles=0;ShellStorageCalls=0`. ReadinessCheck passed in Debug and Release;
Solution and VisionRecipeRunnerSmoke Debug/Release builds passed with zero warnings
and errors; Recipe contracts passed 24/24 in Debug (including the copied D: runtime
execution session) and 23/23 in Release. DocumentationIndex passed and
`git diff --check` found no whitespace errors. Searches found no manual partial or
stale Recipe/OpenGL/TestHooks source/tool path.

PL-0013 is structurally complete. The retained 59 partials are protected framework,
generated, designer, or smoke contracts with explicit reasons; they are not pending
manual cleanup. WPF visual states/themes/layouts/DPI/monitor, GPU rendering,
camera/SDK, and long-running native shutdown remain unverified source/runtime
boundaries.
