# OpenVisionLab OVL-11 ImageCanvas Directory policy owner — 2026-09-08

## Status

Complete for one independently verifiable ImageCanvas Directory-policy
boundary. This slice did not reset, clean, stage, commit, push, touch the
Original checkout, release, or deploy.

## Scope and ownership

Before this slice, `RoiImageCanvasViewModel.Commands.cs` owned the shared
`lastImageDirectory` field, the initial-directory fallback order, upward
`Sample`/`Samples`/`samples` search, and `Directory.Exists` checks used by both
Open and Save image commands.

`ImageCanvasDirectoryPolicy` now owns that state and policy in the
ImageCanvas library. The ViewModel keeps command orchestration, filename
sanitization, dialog-host calls, Mat loading/saving, timing/logging, and the
existing public command contracts. The app-level
`OpenVisionImageDirectoryResolver` and its callers were not changed because
that owner belongs to the application layer and has a different caller
contract.

The fallback order is unchanged: remembered image directory, upward sample
search, application base directory, Pictures, then Desktop. A selected path
is remembered only after the existing Open/Save operation succeeds.

Recipe/XML, explicit Preview/Run, Layer/ImageSpace, SDK, Mat ownership, and
external consumer contracts remain unchanged.

## Call path

`LoadImageCommand`/`SaveImageCommand` ->
`ImageCanvasDirectoryPolicy.ResolveInitialDirectory` ->
`ImageDialogHost.ShowOpenImageDialog` or `ShowSaveImageDialog`.

After a successful load/save, the selected path flows to
`ImageCanvasDirectoryPolicy.RememberImagePath`; the existing
`CanvasImageLoader`, `LoadImage`, `SaveCurrentImage`, and dialog host remain in
place.

## Changed files

- `src/Libraries/OpenVisionLab.ImageCanvas/Util/ImageCanvasDirectoryPolicy.cs`
  - New concrete owner for shared last-directory state and fallback policy.
- `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.Commands.cs`
  - Removed inline Directory traversal/state and routed Open/Save through the
    policy.
- `src/Libraries/OpenVisionLab.ImageCanvas/Properties/AssemblyInfo.cs`
  - Grants the existing D-drive contract assembly access to the internal policy
    without making it a public consumer API.
- `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`
- `docs/admin/CODEBASE_STRUCTURE.md`
- `docs/admin/OPENVISIONLAB_DOCUMENTATION_MAP.md`
- `docs/LLM_DOCUMENT_INDEX.json`
- `docs/reports/OPENVISIONLAB_REFACTOR_PROGRAM_AUDIT_20260908.md`

## Verification evidence

Evidence root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-directory-policy-20260908`.

- `directory-policy-contract\run-output\sample-priority.txt`,
  `remembered-priority.txt`, `invalid-path-fallback.txt`, and
  `base-fallback.txt` each returned `DIRECTORY_POLICY_CONTRACT=PASS`. The
  cases cover sample search, remembered-directory priority, whitespace and
  invalid-path fallback, and application-base fallback.
- `structure-proof.txt` returned `PolicyStructureProof=True`: the ViewModel
  has no Directory traversal or old directory helper, the policy owns
  `DirectoryInfo`/`Directory.Exists`, and the policy has no WPF references.
- ImageCanvas Debug/Release builds passed with 0 warnings and 0 errors.
- OpenVisionLab x64 Debug/Release and ImageCanvas external consumer Release
  builds passed with 0 warnings and 0 errors.
- Baseline `ui-before-imagecanvas` passed
  `wpf_imagecanvas_owned_mat_load` (`OK 1 / WARN 0 / NG 0`).
- `ui-after-debug` and `ui-after-release` both passed
  `wpf_imagecanvas_owned_mat_load` and
  `wpf_shell_host_tool_input_image_load_save` (`OK 2 / WARN 0 / NG 0`).
- `quantitative-audit-after-directory-policy` returned
  `REFACTOR_AUDIT=PASS|CSharpFiles=773|XamlFiles=58|PartialDeclarations=106|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
  Its source survey recorded 268,742 C# lines and 12,238,522 C# bytes.

## UI qualification boundary

No XAML or shared style changed. Fresh before/after ImageCanvas and shell
input PNGs are present under the evidence root. The full physical desktop
matrix (focus, hover, pressed, selected, disabled, popup, themes,
Wide/Compact layouts, 100/125/150/175/200% DPI, monitor placement, resize and
close order) was not run; those states remain unverified.

## Junior developer assessment

**PASS for this boundary.** A contributor can follow
`LoadImageCommand`/`SaveImageCommand -> ImageCanvasDirectoryPolicy ->
ImageDialogHost -> existing load/save owner`. Directory traversal and shared
last-path state have one named owner, while the ViewModel still visibly owns
command sequencing and image operations. Filename sanitization remains a
small command-local presentation policy. The app-level directory resolver is
separate by project-layer ownership and was not duplicated or reopened.

## No-reopen rule and next slice

Completed ImageCanvas Mat, dialog-host, ContextMenu, keyboard, mouse, Learn
host, Learn topic policy, and app-level directory resolver owners are not to be
split again without a reproduced defect, changed requirement, or proven
responsibility conflict.

The next independently verifiable slice is representative WPF runtime
qualification for the completed boundaries.

Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.
