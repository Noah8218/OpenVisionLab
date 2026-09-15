# OVL-11 ImageCanvas OpenFileDialog host — 2026-09-08

Status: Complete for the single OpenFileDialog ownership boundary.

## Scope

This slice moved only the ImageCanvas ViewModel's `OpenFileDialog` construction,
existing options, modal result conversion, and re-entry protection into the
existing dialog host. It preserved Recipe/XML compatibility, explicit
Preview/Run behavior, image/layer ownership, and the public ImageCanvas
consumer contract.

`ContextMenu`, keyboard/mouse input, directory-policy implementation, Mat
loading and disposal, `CanvasImageSaver`, SaveFileDialog policy, other
application OpenFileDialog owners, and the full Windows modal UI matrix remain
outside this slice.

## Current owner -> intended owner

Before, `RoiImageCanvasViewModel.Commands.cs::OpenLoadImage` constructed
`Microsoft.Win32.OpenFileDialog`, configured its filter and initial directory,
called `ShowDialog`, and read `FileName`.

After, `Dialogs.IImageCanvasDialogHost.ShowOpenImageDialog` is the narrow
path-or-null contract. `Views.RoiImageCanvasDialogHost` owns the WPF dialog
configuration, a shared transient modal guard for Open and Save, and the
selected-path/null conversion. `RoiImageCanvasView` assigns the host only while
its ViewModel is attached and clears it during detach/dispose.

The ViewModel property is named `ImageDialogHost` because the same host now
serves both image-open and image-save commands. It remains internal; no public
consumer signature changed.

## Call path and state flow

Before:

`LoadImageCommand -> OpenLoadImage -> new OpenFileDialog -> ShowDialog ->
CanvasImageLoader.LoadMatFromFile -> LoadImage -> lastImageDirectory update`

After:

`LoadImageCommand -> OpenLoadImage -> ImageDialogHost.ShowOpenImageDialog ->
CanvasImageLoader.LoadMatFromFile -> LoadImage -> lastImageDirectory update`

The host returns only a path or null. The ViewModel retains filename and
initial-directory policy, load timing/logging, the `using (Mat)` scope,
`LoadImage`, current image state, and last-directory state. Cancel or a blocked
re-entry still returns without loading or updating the last directory.

## Acceptance evidence

- The existing OpenFileDialog filter and `InitialDirectory` value are unchanged.
- `RoiImageCanvasViewModel.Commands.cs` contains no `OpenFileDialog` construction
  or `ShowDialog` call.
- `RoiImageCanvasDialogHost` is the only ImageCanvas owner of OpenFileDialog
  construction and modal result conversion.
- One transient guard covers both Open and Save dialogs and resets in `finally`.
- View attach, detach, and dispose assign or clear `ImageDialogHost`
  deterministically.
- Existing SaveImageCommand and SaveCurrentImage facade paths remain intact.
- Existing public commands and external consumer APIs remain unchanged.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-open-dialog-host-20260908`

- ImageCanvas Debug build: PASS, zero warnings and zero errors
  (`imagecanvas-build-Debug.log`).
- ImageCanvas Release build: PASS, zero warnings and zero errors
  (`imagecanvas-build-Release.log`).
- `ImageCanvasExternalConsumerSmoke` Release build: PASS, zero warnings and
  zero errors (`imagecanvas-external-consumer-build-Release.log`).
- OpenVisionLab app x64 Debug/Release builds: PASS, zero warnings and zero
  errors (`openvisionlab-build-Debug-x64.log`, `openvisionlab-build-Release-x64.log`).
- `RunUiPrecheck.ps1` targets `wpf_imagecanvas_owned_mat_load` and
  `wpf_shell_host_tool_input_image_load_save`: both `OK` in Debug and Release
  (`ui-precheck-debug`, `ui-precheck-release`).
- Structural proof: PASS for owner, call path, lifecycle wiring, shared guard,
  and preserved Save path (`structure-proof.txt`, `open-dialog-ownership-after.txt`,
  `lifecycle-after.txt`, `load-call-path-after.txt`).
- Post-change quantitative audit: PASS; 766 C# files, 268,358 lines,
  12,221,049 bytes, 58 XAML files, 106 partial declarations, 0 project cycles,
  and 0 Shell Run History storage calls
  (`quantitative-audit-after-open-dialog-host`).
- Documentation index, JSON parse, and targeted `git diff --check`: PASS.

## Junior developer readability check

**PASS for this slice.** A new contributor can start at `LoadImageCommand`,
follow `OpenLoadImage`, see the single `ImageDialogHost` contract, and then
continue through the unchanged loader and `LoadImage` state flow. The host file
contains only modal UI policy; the ViewModel file contains image state and load
policy. The remaining ContextMenu/input coupling is named as the next boundary
rather than hidden in another partial.

## Boundary and next work

The full interactive Windows OpenFileDialog theme, layout, popup bounds, DPI,
keyboard, and mouse matrix was not run. This is **소스 코드 기준 검토 완료 /
실제 Runtime OpenFileDialog UI 검증 필요**.

The next independently verifiable slice is ContextMenu/input ownership. Do not
reopen the completed Mat, OpenFileDialog, or SaveFileDialog owners and do not
add a partial-only split without new defect or responsibility-conflict
evidence.


