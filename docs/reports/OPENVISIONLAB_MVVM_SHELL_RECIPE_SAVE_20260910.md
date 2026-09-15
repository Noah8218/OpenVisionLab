# OpenVisionLab 2D Shell Recipe 저장 callback 소유권 보정

Updated: 2026-09-10 KST

Status: VERIFIED for the bounded Recipe persistence callback owner move,
Debug/Release app build, readiness contract, and Recipe context/change-safety
smoke. Full WPF theme/DPI/input/long-run qualification remains unverified.

## Problem

Shell composition code passed a lambda that directly called
`runtimeContext.Global.Recipe.SaveTools()` into the Recipe command surface. The
lambda had no policy of its own, but it made the View constructor the visible
caller of Recipe persistence and required a new developer to cross from View
composition into `GlobalState` before finding the Recipe owner.

## Current → intended owner and call path

| 항목 | 변경 전 | 변경 후 |
| --- | --- | --- |
| Recipe tool save callback | `OpenVisionShellHostView.xaml.cs` lambda | `OpenVisionShellHostRecipeController.SaveRuntimeRecipeTools` |
| Persistence implementation | `GlobalState.Recipe.SaveTools()` | same existing `RecipeState.SaveTools()` call through the Recipe controller |
| View responsibility | direct Recipe persistence call in composition | pass the existing concrete controller method |

호출 경로는 다음과 같습니다.

```text
Recipe command surface
  -> OpenVisionShellHostRecipeController.SaveRuntimeRecipeTools
  -> runtimeContext.Global.Recipe.SaveTools
  -> RecipeRuntimeStorage.Save
```

The existing `Func<bool> saveRecipe` command-surface contract, Recipe XML
format, save result, and failure projection are unchanged. No interface,
wrapper, manager, factory, or partial was added.

## Ownership and lifetime

- `OpenVisionShellHostRecipeController` already owns the Shell Recipe runtime
  context and Recipe switch lifecycle, so it is the concrete owner for this
  callback as well.
- `RecipeState`/`RecipeRuntimeStorage` continue to own tool persistence and
  rollback behavior.
- The View remains the composition root for WPF controls and callback wiring;
  it does not own the Recipe save result or storage side effects.

## Verification

- OpenVisionLab Debug build: 0 warning / 0 error
- OpenVisionLab Release build: 0 warning / 0 error
- `OpenVisionReadinessCheck`: PASS, including source checks that reject direct
  `Global.Recipe.SaveTools()` from Shell composition
- focused UI smoke: `wpf_shell_host_recipe_context_switch=OK`,
  `wpf_shell_host_recipe_change_safety=OK`
- evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r13-recipe-save-owner-20260910`
- `TestDocumentationIndex.ps1`: PASS, `IndexedPaths=282`, `Routes=16`,
  `RootRedirects=102`
- `Invoke-RefactorAudit.ps1 -Verify`: PASS,
  `CSharpFiles=847|XamlFiles=60|PartialDeclarations=110|ProjectCycles=0|ShellStorageCalls=0`
- `git diff --check`: no whitespace errors; only existing LF-to-CRLF
  normalization notices were reported

## Self-evaluation

The Shell Recipe path now has one named concrete owner for selection and tool
save callbacks. The View constructor still creates and wires many existing
collaborators because it is the WPF composition root; this change does not
pretend that a shorter constructor alone would reduce ownership complexity.
