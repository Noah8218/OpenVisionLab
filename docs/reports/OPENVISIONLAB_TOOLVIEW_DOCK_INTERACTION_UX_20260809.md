# OpenVisionLab Tool View Dock And Interaction UX

Date: 2026-08-09 KST
Status: Complete in Dev

## Scope

- Keep native algorithm Tool Views in the right inspector.
- Give Pipeline Review a central document workspace instead of compressing it
  into the right inspector.
- Preserve the P291 same-context Pipeline Review suspension and reopen path.
- Make the no-image workspace guidance usable in Compact layout.
- Give common Tool View, dock-header, and floating-window buttons visible
  keyboard-focus, hover, and pressed states with localized accessible names.
- Include `AffineTransform` in the all-native Tool View open and layer-routing
  regression set.

Camera, PLC, equipment integration, new algorithms, Preview/Run behavior, and
Pipeline routing semantics were not changed.

## Result

- `OpenVisionDockedDocumentWorkspaceController` now owns centrally docked
  Pipeline Review content. The existing right inspector remains the owner for
  native algorithm Tool Views.
- Pipeline Review can float, dock centrally, return to the owning Recipe,
  reopen into the central workspace, and close without creating a second
  Pipeline editor.
- The Compact no-image guide uses a responsive 2 x 2 step flow and 2 x 2
  action layout; all four steps, all four actions, and the operator hint are
  visible at the tested 1280 x 760 window size.
- Common Tool View, dock-header, and title-bar buttons use the established
  teal/dark theme for normal, focus, hover, pressed, and disabled states.
  Pressed controls move by one device-independent pixel; keyboard focus uses
  the existing semantic focus brush.
- Tool actions and Pipeline Review navigation/run actions expose their current
  Korean or English label as an automation name.

## Ownership Proof

- New owner:
  `src/OpenVisionLab/UI/Menu/Wpf/Shell/Documents/OpenVisionDockedDocumentWorkspaceController.cs`.
- Pipeline selection and lifetime routing:
  `OpenVisionShellHostToolWindowController` and
  `OpenVisionShellHostToolWindowLifecycleController`.
- Native Tool View docking remains in
  `OpenVisionDockedToolInspectorController`; it was not replaced by the
  document workspace.
- The focused dock-cycle regression proves the former owner does not retain
  Pipeline Review: after Matching is right-docked, selecting Pipeline hides
  the right inspector and shows one central document; return suspends that
  document, and reopening restores it centrally.

## Current-Build Verification

Built EXE:
`C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp
`2026-08-09T19:14:21.4369420+09:00`.

All actual-EXE visual runs used the dynamically selected leftmost monitor
`\\.\DISPLAY2`, bounds `Left=-1920, Top=365, Width=1920, Height=1080`.

### Commands

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target "wpf_tool_open_perf,wpf_layer_selection_all_native_tools,wpf_tool_window_dock_float_cycle" "D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\toolview-ux-fix-20260809\current-smoke"
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1
git diff --check
```

Results:

- Debug build: zero warnings, zero errors.
- `wpf_tool_open_perf`: pass; 16 prewarmed native tools, cold selection
  65-192 ms and warm selection 63-121 ms in this run.
- `wpf_layer_selection_all_native_tools`: pass; all 16 native tools including
  `AffineTransform`, layout/text/internal issue counts all zero.
- `wpf_tool_window_dock_float_cycle`: pass; layout/text/internal issue counts
  all zero. Matching -> central Pipeline -> float -> central dock -> Return to
  Recipe -> reopen central preserved Preview/Run state, layer count, active
  layer, and routing.
- Readiness: 13/13 contracts passed.
- Vendored external references: passed.
- Public sample assets: 33 catalog rows, 229 manifest assets, 17 Pipelines,
  passed.
- Patch hygiene: passed.

### Actual-EXE Matrix

| Case | Status | Invoke | Ready | Required controls |
| --- | --- | ---: | ---: | --- |
| English Matching | OK, responsive | 66 ms | 1605 ms | 3/3 visible and named in Wide/Compact |
| English Pipeline | OK, responsive | 31 ms | 1008 ms | 2/2 visible and named in Wide/Compact |
| Korean Matching | OK, responsive | 40 ms | 2179 ms | 3/3 visible and named in Wide/Compact |
| Korean Pipeline | OK, responsive | 40 ms | 1059 ms | 2/2 visible and named in Wide/Compact |

The `Ready` value includes actual process launch, first document activation,
and UI Automation discovery; the focused in-process performance smoke above is
the comparable Tool View selection measurement.

### Button-State Matrix

All audited controls were found, enabled, keyboard-focusable, and visually
changed for focus, hover, and press:

| Control | Accessible name | Focus | Hover | Pressed |
| --- | --- | ---: | ---: | ---: |
| Learn | `Learn Matching` | 41.11% | 72.63% | 78.01% |
| Add Pipeline | `파이프라인에 추가·저장` | 16.88% | 84.23% | 17.92% |
| Run Preview | `미리보기 실행` | 12.27% | 86.44% | 87.07% |
| Native dock close | `툴 닫기` | 23.55% | 14.53% | 14.53% |
| Pipeline document close | `툴 닫기` | 23.55% | 14.53% | 14.53% |

Percentages are changed-pixel ratios against each control's normal-state
capture, not a visual-quality score.

### Recipe Manager Interaction Follow-Up

- The top Recipe Manager pencil toggle now uses the shared Recipe Manager
  normal, hover, pressed, checked, keyboard-focus, and disabled states.
- The advanced-review toggle uses the same states. Recipe Manager action
  buttons move their content by one pixel when pressed and expose a themed
  keyboard-focus border; Recipe Manager tabs now expose a hover state.
- In the current actual EXE, the top Recipe Manager toggle changed zero pixels
  before the fix and 884 pixels after the fix when hovered. The advanced-review
  toggle changed 3,921 pixels and the close button changed 644 pixels.
- Korean and English Recipe Manager summary/advanced-review/Pipeline Review
  round-trip smokes passed. The smoke now checks the P292 central document
  workspace instead of the removed right-inspector ownership assumption.

## Evidence

- Before actual EXE:
  `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\toolview-ux-fix-20260809\before`.
- Current actual EXE, Korean/English and Wide/Compact:
  `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\toolview-ux-fix-20260809\current`.
- Current button states:
  `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\toolview-ux-fix-20260809\current-button-states`.
- Current focused smoke and timing:
  `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\toolview-ux-fix-20260809\current-smoke`.
- Recipe Manager before/after hover and focused Korean/English smoke:
  `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\recipe-manager-hover-20260809`.

## Closure Record

```text
Status: Complete
Scope: Dev Tool View speed/docking ownership, Compact layout, and Tool View/Recipe Manager interaction-state correction
Acceptance criteria: native Tool Views retain the right inspector; Pipeline Review uses the central document workspace; Korean/English Wide/Compact controls remain visible; Tool View and Recipe Manager focus/hover/pressed states are themed; reopen causes no automatic execution, layer, or routing mutation
Verification: Debug build 0 warnings/errors; Tool View and Korean/English Recipe Manager focused UI smokes passed; current actual-EXE 4/4 matrix passed; current Tool View button-state 5/5 matrix passed; Recipe Manager actual-EXE hover changed 0 -> 884 pixels and representative panel controls changed on hover; readiness 13/13; external references and public assets passed; git diff --check passed
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\toolview-ux-fix-20260809; D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\recipe-manager-hover-20260809; and this report
Boundary / next dependency: verification was performed in Dev; promotion to the original repository must preserve this reviewed patch. A new product task requires a named operator blocker or a verified current-build regression.
```
