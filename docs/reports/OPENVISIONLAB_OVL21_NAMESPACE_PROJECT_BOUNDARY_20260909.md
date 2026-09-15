# OpenVisionLab OVL-21 namespace/project boundary — 2026-09-09

Status: **Complete for one compatibility-safe internal namespace migration**.

## Scope

The namespace/project audit identified `src/OpenVisionLab/Property/ParameterPropertyStorage.cs` as the smallest safe migration candidate. The type is `internal static`, has exactly one production caller (`ParameterProperty`), and is not a XAML type, public API, reflection target, or serializer root. The migration moved only this storage owner from the application root namespace to the namespace that matches its responsibility folder:

```text
OpenVisionLab.ParameterProperty (public XML model; compatibility owner)
  -> OpenVisionLab.Property.ParameterPropertyStorage (internal persistence owner)
  -> RecipeWorkspaceService / SerializeHelper / child property SaveConfig/LoadConfig
```

`ParameterProperty` remains in `OpenVisionLab` and keeps `[XmlRoot("CPropertyParam")]`. The existing Recipe XML compatibility map still resolves `OpenVisionLab.ParameterProperty`; no public type, XAML `x:Class`, reflection string, project reference, Recipe/XML shape, Preview/Run, Layer/ImageSpace, or PropertyGrid behavior was changed.

## Ownership proof

| Boundary | Before | After |
| --- | --- | --- |
| Public Recipe property model | `OpenVisionLab.ParameterProperty` | unchanged; owns public shape and XML root |
| Parameter child persistence | root-namespace helper in the `Property` folder | `OpenVisionLab.Property.ParameterPropertyStorage`; owns Load/Save and child config persistence |
| State owner | `ParameterProperty` instance and its child properties | unchanged |
| Dependency direction | model -> same-namespace helper | model (`OpenVisionLab`) -> explicit `OpenVisionLab.Property` internal owner -> existing storage/child property APIs |
| Observable contract | `LoadConfig`/`SaveConfig`, public type name, `CPropertyParam` root | unchanged |

The former owner no longer declares the storage type in `OpenVisionLab`; the only production references are the moved owner and the public model. This is a concrete namespace boundary, not a partial-file or name-only split.

## Changed files

- `src/OpenVisionLab/Property/ParameterPropertyStorage.cs`
  - Namespace changed to `OpenVisionLab.Property`; type remains internal static.
- `src/OpenVisionLab/Property/ParameterProperty.cs`
  - Adds the explicit namespace import while retaining the public legacy namespace and XML root.
- `tools/VisionRecipeRunnerSmoke/NamespaceProjectBoundaryContract.cs`
  - New window-free contract covering namespace ownership, access level, caller count, XAML/reflection/serializer exclusion, and XML compatibility mapping.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - Adds `--namespace-project-boundary-contract` dispatch.

No unrelated source files were staged; existing dirty worktree changes were preserved. Handoff, structure, map, README, and index navigation records were updated locally and are outside this selective code checkpoint.

## Verification

- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Debug --nologo`: **0 warnings / 0 errors**.
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Release --nologo`: **0 warnings / 0 errors**.
- `--namespace-project-boundary-contract`: **8/8 passed** in Debug and Release. Evidence:
  - `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl21-namespace-project-boundary-debug-run2-20260909\`
  - `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl21-namespace-project-boundary-release-run2-20260909\`
- `RecipeXmlCompatibilityCheck` Debug and Release builds: **0 warnings / 0 errors**; runtime check passed for **13 XML roots** in each configuration. Temporary XML files were routed to D:.
- `Invoke-RefactorAudit.ps1 -Verify`: **PASS**; `CSharpFiles=787`, `XamlFiles=59`, `PartialDeclarations=108`, `Projects=27`, `ProjectCycles=0`, `ShellStorageCalls=0`. Evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl21-refactor-audit-20260909\`.
- `git diff --check` passed for the migrated source files. The existing unrelated dirty changes remain in place.

This is a code-only namespace change; no WPF visual/runtime state changed, so the UI runtime gate is outside this slice.

## Junior readability assessment

**PASS for this slice.** A junior maintainer can locate the public XML model in `Property/ParameterProperty.cs`, follow its explicit `OpenVisionLab.Property` dependency, and find persistence policy in one concrete owner. The migration leaves legacy public naming in place and makes the folder/namespace relationship visible without introducing an interface, wrapper, factory, or project cycle.

## Compatibility and no-duplicate rule

Do not move the remaining public property classes or rename the root `OpenVisionLab` namespace without a separate inventory of XAML `x:Class`, reflection, serialization, public callers, and staged build evidence. Do not recreate or re-split `ParameterPropertyStorage` unless a new defect, explicit contract change, or proven responsibility conflict appears. Another model or agent must not repeat this migration or add documentation-only follow-up work for the same boundary.

## Boundary and next priority

This slice does not complete the full namespace/project migration. Other root-namespace public types remain compatibility-sensitive, and the 171k-line app project remains an investigation hotspot. No project split was attempted because the candidate has no independent assembly dependency and the current project graph already has zero cycles.

Next priority: responsibility-based smoke-runner cleanup after all product boundaries are closed.

Recommended model: `gpt-5.4-mini` | Reasoning effort: `medium`.

## Completion record

```text
Status: Complete
Scope: Move internal ParameterPropertyStorage to OpenVisionLab.Property
Acceptance: explicit namespace boundary + one production caller + public XML type/root preserved + no XAML/reflection/serializer signal + 8/8 contract + Debug/Release build + 13-root XML compatibility check
Verification: VisionRecipeRunnerSmoke Debug/Release; NamespaceProjectBoundaryContract 8/8; RecipeXmlCompatibilityCheck Debug/Release; RefactorAudit -Verify; git diff --check
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl21-namespace-project-boundary-debug-run2-20260909; ovl21-namespace-project-boundary-release-run2-20260909; ovl21-recipe-xml-debug-20260909; ovl21-recipe-xml-release-20260909; ovl21-refactor-audit-20260909
Boundary: remaining public root namespace and project-level extraction require separate compatibility inventory; UI states were not changed
```
