# OVL-11 ImageCanvas ContextMenu host refactoring — 2026-09-08

Status: Complete (one independently verifiable ContextMenu ownership boundary).

## Scope

This execution moved only the concrete WPF ContextMenu reference and the
`IsOpen` operation out of `RoiImageCanvasViewModel`. The existing XAML menu,
`MenuItems` binding, menu item commands and parameters, right-click mode reset
policy, public ViewModel command contract, Recipe/XML behavior, Preview/Run
behavior, Layer/ImageSpace ownership, Mat lifetime, and external consumer
contract were preserved. Keyboard/mouse event implementation and the full
interactive ContextMenu matrix are a separate next slice.

## Ownership before and after

Before this slice, `RoiImageCanvasViewModel.ContextMenu` stored a concrete
`System.Windows.Controls.ContextMenu`. `ExecuteRightClickCommand` checked that
property and set `ContextMenu.IsOpen`, while the View assigned the XAML menu to
the ViewModel during attachment.

After this slice, the internal
`OpenVisionLab.ImageCanvas.Dialogs.IImageCanvasContextMenuHost` contract exposes
only `OpenContextMenu()`. `RoiImageCanvasView` implements that contract and
owns the existing `MainGrid.ContextMenu` instance and `IsOpen` operation.
`RoiImageCanvasViewModel` owns the right-click mode policy and calls
`ContextMenuHost.OpenContextMenu()` without referencing the concrete WPF type.

The View attaches `ContextMenuHost` together with `ImageDialogHost`, clears both
on DataContext detach and `Dispose`, and keeps the existing
`MainGrid.ContextMenu.DataContext = viewModel` wiring. The XAML
`Grid.ContextMenu`, `ItemsSource="{Binding MenuItems}"`, item template,
commands, icons, children, and placement-target parameter are unchanged.

## Call path and state ownership

```text
ImageCanvasControl right-click
  -> RoiImageCanvasViewModel.OnMouseRightClick
  -> ExecuteRightClickCommand
  -> mode reset when Measure/Teaching/AddRoiArray is active
  -> ContextMenuHost.OpenContextMenu
  -> RoiImageCanvasView.MainGrid.ContextMenu.IsOpen
```

The ViewModel remains the owner of mode state and command policy. The WPF View
is the owner of menu instance, DataContext hookup, open operation, and View
lifecycle wiring. Menu item commands continue to target the existing ViewModel
state. No new global provider, message bus, factory, or partial-only split was
introduced.

## Acceptance evidence

- `structure-proof.txt` passes the narrow contract, View ownership, ViewModel
  decoupling, attach/detach/dispose cleanup, unchanged XAML bindings, and
  right-click call path checks.
- `context-menu-before.txt` and `context-menu-ownership-after.txt` preserve the
  before/after search evidence. The ViewModel has no concrete ContextMenu
  property or `.IsOpen` call after the change.
- `lifecycle-after.txt` proves attach, detach, and dispose host assignment and
  clearing; `context-menu-call-path-after.txt` proves the command route remains
  in use.
- `refactor-proof-plan.md` records current owner, intended owner, dependency
  direction, state ownership, and the focused proof boundary.

## Verification performed

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-context-menu-host-20260908`

- ImageCanvas Debug build: passed, 0 warnings, 0 errors
  (`imagecanvas-build-Debug.log`).
- ImageCanvas Release build: passed, 0 warnings, 0 errors
  (`imagecanvas-build-Release.log`).
- Existing external ImageCanvas consumer Release build: passed, 0 warnings,
  0 errors (`imagecanvas-external-consumer-build-Release.log`).
- OpenVisionLab application x64 Debug build: passed, 0 warnings, 0 errors
  (`openvisionlab-build-Debug-x64.log`).
- OpenVisionLab application x64 Release build: passed, 0 warnings, 0 errors
  (`openvisionlab-build-Release-x64.log`).
- Existing focused ImageCanvas/Shell UI precheck: Debug and Release both
  `OK 2 / WARN 0 / NG 0`; rendered targets were
  `wpf_imagecanvas_owned_mat_load` and
  `wpf_shell_host_tool_input_image_load_save`
  (`ui-precheck-debug\ui_precheck_summary.json`,
  `ui-precheck-release\ui_precheck_summary.json`). These are source/precheck
  evidence for the existing paths; the full interactive ContextMenu runtime
  matrix was not run.
- Post-change quantitative audit: `REFACTOR_AUDIT=PASS`, 767 C# files,
  268,374 C# lines, 12,221,410 C# bytes, 58 XAML files, 106 partial
  declarations, one direct ViewModel UI/dialog signal, eight direct IO files,
  and zero project cycles (`quantitative-audit-after-context-menu-host\source-survey-summary.txt`).

## Junior readability self-evaluation

Result: **PASS for this slice**. A junior contributor can follow one visible
route from `ImageCanvasControl` right-click to
`ExecuteRightClickCommand`, see the mode-reset decision, then open the narrow
`ContextMenuHost` contract and find its concrete WPF implementation in
`RoiImageCanvasView.xaml.cs`. The XAML menu and its `MenuItems` source remain at
the View, so state policy and visual ownership are visible at their respective
boundaries. The remaining WinForms input and `Directory` coupling is explicit
and is not disguised as completed by this report.

## Boundary and next slice

`소스 코드 기준 검토 완료 / 실제 Runtime ContextMenu UI 검증 필요`.
Full keyboard/mouse/focus/pressed/selected/disabled/popup/theme/layout/DPI and
monitor interaction for ContextMenu remains unverified. The next single
implementation slice is **OVL-11 keyboard/mouse input ownership**. It must
reuse the existing event contract, preserve focus/drag/drop/rendering behavior,
and not reopen the completed Mat, OpenFileDialog, SaveFileDialog, or ContextMenu
owners without a new defect, changed requirement, or proven responsibility
conflict.

No commit, push, Original-repository mutation, release, or deployment was
performed.