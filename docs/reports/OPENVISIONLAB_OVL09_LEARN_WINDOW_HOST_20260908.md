# OpenVisionLab OVL-09 Learn Window/Shell host boundary — 2026-09-08

## Status

Complete for one independently verifiable Learn Window host boundary. This
slice does not commit, push, modify `Original`, release, deploy, reset, clean,
or stage the worktree.

## Scope and ownership

Before this slice, `OpenVisionShellHostCommandController` owned workspace image
commands and the unrelated Learn Window instance: creation, reuse, owner
assignment, Learn callback injection, topic routing, and `Closed` cleanup. That
made the Shell command controller the owner of both workspace state and the
Learn surface lifetime.

`OpenVisionShellHostLearnWindowController` now owns the Learn Window lifetime
and Learn entry routing. It receives three explicit callbacks:

- Shell owner window provider;
- existing `PromptAndOpenRunnableSample` workflow for Learn practice samples;
- existing `SelectToolMenu` workflow for opening a related Tool View.

The existing `OpenVisionLearnTopicCatalog`, `OpenVisionLearnWindow`, and all
topic-specific Learn View/Presenter owners remain unchanged. Recipe/XML,
explicit Preview/Run, Layer/ImageSpace, and workspace sample loading policy are
preserved.

## Call path

```text
Shell Chrome command
  -> OpenVisionShellHostLearnWindowController
  -> OpenVisionLearnWindow
  -> OpenVisionLearnTopicCatalog / existing topic View or Presenter
  -> explicit Shell sample or Tool callback when the operator clicks it
```

Pipeline Review's selected-tool Learn entry now uses the same owner. Reopening
an existing Learn window selects the requested topic and activates that window;
the `Closed` handler removes its subscription and clears the reference so the
next request creates a fresh window.

## Changed files

- `src/OpenVisionLab/UI/Menu/Wpf/Shell/Commands/OpenVisionShellHostLearnWindowController.cs`
  — new concrete Learn Window lifetime and routing owner.
- `src/OpenVisionLab/UI/Menu/Wpf/Shell/Commands/OpenVisionShellHostCommandController.cs`
  — removed Learn Window state, topic routing, and lifecycle code; workspace
  sample/image responsibilities remain here.
- `src/OpenVisionLab/UI/Menu/Wpf/Shell/Chrome/OpenVisionShellHostChromeCommandSurface.cs`
  — routes Learn, Tool Learn, and Tool Samples commands through the new owner.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.xaml.cs`
  — composes the new owner with existing Shell callbacks.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.Interactions.cs`
  — routes Pipeline Review's selected-tool Learn entry through the new owner.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl09-learn-host-20260908`

- `LEARN_HOST_CONTRACT=PASS`: D: WPF contract checked Tool menu routing,
  tool-type routing, existing-window reuse, sample path callback, no implicit
  Tool action, `Closed` cleanup/fresh-window creation, and unknown-tool no-op.
- `LEARN_HOST_STRUCTURE_PROOF=True`: old CommandController Learn coupling is
  absent; the new owner contains all four routes and deterministic event
  unsubscription; Chrome and Shell composition use the new owner.
- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj --configuration Debug
  --no-restore -p:Platform=x64`: 0 warnings / 0 errors.
- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj --configuration Release
  --no-restore -p:Platform=x64`: 0 warnings / 0 errors.
- `dotnet build tools/ImageCanvasExternalConsumerSmoke/ImageCanvasExternalConsumerSmoke.csproj
  --configuration Release --no-restore`: 0 warnings / 0 errors.
- `RunUiPrecheck.ps1` Debug and Release for
  `wpf_shell_host_learn_entry,wpf_simple_preprocess_tool_learn_button`:
  both returned `OK 2 / WARN 0 / NG 0`; fresh PNG/report/summary files are in
  the corresponding D: `ui-precheck-debug` and `ui-precheck-release` folders.
- `Invoke-RefactorAudit.ps1 -Verify`: `REFACTOR_AUDIT=PASS`,
  `CSharpFiles=771`, `CSharpLines=268667`, `CSharpBytes=12234466`,
  `XamlFiles=58`, `PartialDeclarations=106`, `TypeDeclarations=1318`,
  `ProjectCycles=0`, `ShellStorageCalls=0`, `ViewModelUiIoFiles=1`.
- `git diff --check` for the five changed source files: exit 0. Git reports
  the repository's existing LF/CRLF normalization warning for these files.

## Junior developer assessment

**PASS for this boundary.** A new contributor can now start at the Shell
Chrome command, find one named Learn Window owner, and follow the selected
topic or explicit sample/Tool callback without searching the workspace image
controller for Learn lifetime state. The remaining large composition point is
inside `OpenVisionLearnWindow.xaml.cs`: common topic selection, panel visibility,
and document/practice presentation still need a separate evidence-backed
boundary. The physical desktop focus/hover/pressed/selected/disabled/popup,
theme/layout, DPI, monitor, and resize matrix remains unverified; this slice
has source, contract, build, and focused offscreen UI evidence.

## No-reopen rule and next slice

Do not reopen this Learn host owner, completed topic View/Presenter owners, or
completed ImageCanvas owners without a reproduced current defect, changed
explicit requirement, or demonstrated responsibility conflict. The next single
slice is **OVL-09 Learn Window topic-selection/presentation composition**.
