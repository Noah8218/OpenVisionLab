# OpenVisionLab OVL-19 Shell Recipe basic lifecycle view — 2026-09-09

Status: **Complete for the single Shell XAML basic lifecycle view slice**.

## Scope

The Recipe Manager name editor and basic Create/Duplicate/Rename/Delete strip
was inline in `OpenVisionShellHostView.xaml`. That markup is now owned by the
presentation-only `OpenVisionRecipeBasicLifecycleView` UserControl. The
existing `OpenVisionShellHostRecipeCommandSurface` remains the state and
command owner; the ViewModel, Recipe XML, Preview/Run, Layer/ImageSpace, and
PropertyGrid contracts were not changed.

```text
OpenVisionShellHostView
  -> OpenVisionRecipeBasicLifecycleView
  -> existing RecipeCommands bindings
  -> OpenVisionShellHostRecipeCommandSurface command/state owner
```

The Shell keeps the advanced-review `ElementName` trigger in its own namescope
and applies the collapsed/visible state to the child view. The child view has
no reference to that toggle and its code-behind only calls `InitializeComponent`.
Brushes that were previously resolved from the Shell resource scope use
`DynamicResource`, so the nested UserControl can load before the parent resource
lookup is complete.

## Changed files

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Views/OpenVisionRecipeBasicLifecycleView.xaml`
  - Moved the existing name editor, CRUD command strip, validation text,
    status text, bindings, and AutomationIds without changing their contracts.
- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Views/OpenVisionRecipeBasicLifecycleView.xaml.cs`
  - Minimal WPF initialization-only code-behind.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.xaml`
  - Replaced the former inline `Grid` with the named UserControl and retained
    the Shell-owned advanced-review visibility style.
- `tools/VisionRecipeRunnerSmoke/ShellRecipeBasicLifecycleViewContract.cs`
  - Window-free structural contract for composition, bindings, AutomationIds,
    namescope ownership, and code-behind limits.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - Added `--shell-recipe-basic-lifecycle-view-contract` dispatch.

## Verification

- OpenVisionLab Debug build: **0 warnings / 0 errors**. Output:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl19-runtime-after-20260909\app3`.
- VisionRecipeRunnerSmoke Debug build: **0 warnings / 0 errors**.
- VisionRecipeRunnerSmoke Release build: **0 warnings / 0 errors**.
- OpenVisionLab Release build: **0 warnings / 0 errors**.
- New structural contract: **7/7 passed** in Debug and Release. Evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl19-after-build-20260909\contract-debug-run2`
  and
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl19-after-build-20260909\contract-release`.
- `recipe-manager-tabs` desktop smoke reached the existing Recipe Manager
  summary assertions, including `HostRecipeManagerCommandStrip` and
  `HostRecipeNameEditor`, and captured the basic lifecycle strip after the
  resource and visibility fixes. Evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl19-runtime-after-20260909\recipe-manager-tabs-run3`.
- The same broad smoke process exited with code 0 but reported `FAIL` at
  `OpenVisionLabDirectSmokeRunner.cs:4802` because no comparable benchmark
  baseline existed and the current sample set differed. The baseline run
  reported the identical failure; this is not attributed to the View slice.
- A separate `recipe-pipeline-roundtrip` probe reported `FAIL` because the
  existing Pipeline Review window did not open (`Window=False,
  ManagerOpen=False`, `OpenVisionLabDirectSmokeRunner.cs:3461`). It exercises a
  different route and remains an existing risk; it is not used as OVL-19 pass
  evidence.
- Monitor topology for the desktop run was one logical monitor:
  `\\.\DISPLAY2`, 1920x1080 bounds, 1920x1032 working area. The required
  one-monitor path was used.
- Refactor audit: `CSharpFiles=784`, `XamlFiles=59`,
  `PartialDeclarations=108`, `Projects=27`, `Cycles=0`,
  `ShellRunHistoryStorageCalls=0`. Evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl19-after-build-20260909\refactor-audit`.
- Readiness check was run and retained its pre-existing unrelated failures
  (Run History presenter token, Learn topic tokens, `PIXELPERMM`, and panel
  token checks); the WPF shell migration and source ownership checks passed.
  Evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl19-after-build-20260909\readiness-contract.txt`.

This evidence covers normal summary rendering and the named automation targets.
Hover, pressed, focus, disabled, popup, alternate themes/layouts, and 125/150/
175/200% DPI were not exercised in this source slice and remain unverified.

## Junior readability assessment

**PASS for this slice.** A maintainer can start at the Shell composition, open
one clearly named `Recipe/Views` file for the presentation markup, and follow
all state and commands through the existing `RecipeCommands` owner. There is no
workflow, file I/O, Window creation, or duplicate ViewModel in the new View.

Do not let another model or agent recreate this UserControl, move the same CRUD
bindings again, or split the same Shell strip without a newly reproduced defect,
a changed explicit contract, or a proven responsibility conflict.

## Boundary and next priority

This slice does not complete the entire Shell XAML or generic PropertyGrid
adapter roadmap. The remaining advanced Recipe Manager presentation is still
inline and must be selected by an independent runtime target before extraction.

Next priority: `WpfPropertyGridAdapter` generic internals, after confirming the
existing `PropertyGridToolPolicy` boundary and selecting one independently
testable adapter responsibility.

Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.
