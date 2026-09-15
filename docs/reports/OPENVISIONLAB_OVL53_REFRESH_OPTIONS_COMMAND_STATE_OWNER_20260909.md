# OpenVisionLab OVL-53 — RefreshOptions composite command-state owner

Status: Complete for one independently verifiable Shell command-state projection
boundary. The broader Shell validation and Step Edit refactoring program remains
active.

## Problem and bounded change

The `RefreshOptions` composite path selected the current Recipe and then refreshed
Pipeline and Validation Set options. Each nested refresh called
`RefreshCommandState`, and `RefreshOptions` called it again after the composite
work. The shared command-state bindings were therefore projected three times for
one user-visible refresh. The same path also changed selected summary and recent
run state, whose setters directly notified a subset of those bindings.

The existing owners remain intact. `RefreshOptions` now passes
`refreshCommandState: false` through `SetSelectedRecipeName` to the two nested
refresh methods. Those methods continue to update their existing mutable state and
binding properties, while their direct callers keep the default `true` behavior.
During the composite guard, the overlapping setter notifications are deferred as
well. `RefreshOptions` performs the single final `RefreshCommandState` projection.

No new interface, service, wrapper, partial, public binding name, Recipe/XML
contract, layer rule, Preview/Run action, or UI feature was added.

## Owner and call path

- Current and intended composite owner: `OpenVisionShellHostRecipeCommandSurface.Handlers.cs`.
- Mutable Recipe selection owner: `SetSelectedRecipeName` in the same file.
- Mutable Pipeline option owner: `RefreshPipelineOptions` in the same file.
- Mutable Validation Set selection/document owners: existing
  `OpenVisionRecipeValidationSetSelectionOwner` and
  `OpenVisionRecipeValidationSetDocumentOwner`, called by
  `OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs`.
- Shared command-state notifier: `RefreshCommandState` in
  `OpenVisionShellHostRecipeCommandSurface.Handlers.cs`.
- Actual path:

  ```text
  RefreshOptions
    -> SetSelectedRecipeName(current, refreshCommandState: false)
    -> RefreshPipelineOptions(..., false)
    -> RefreshValidationSetOptions(refreshCommandState: false)
    -> RecipeOptions / filter projection
    -> RefreshCommandState (outer, once)
  ```

The existing `RefreshPipelineOptions(...)` and
`RefreshValidationSetOptions(...)` callers outside this composite path do not pass
the optional flag and therefore retain their previous immediate command-state
projection.

## Observable and developer contracts

- Existing WPF binding names remain unchanged.
- Pipeline and Validation Set XML/document persistence remain owned by their
  existing storage/document owners.
- Preview and Run remain explicit actions; refreshing options does not execute a
  pipeline.
- The shortest reading order is: `RefreshOptions` -> `SetSelectedRecipeName` ->
  `RefreshPipelineOptions` / `RefreshValidationSetOptions` -> `RefreshCommandState`
  -> the command-state properties in `OpenVisionShellHostRecipeCommandSurface.cs`.
  One repository search for `RefreshOptions` reaches the composite entry and its
  nested policy calls.
- `tools/VisionRecipeRunnerSmoke/RefreshOptionsCommandStateContract.cs` records
  the mutable-state and notification contract on the D: evidence root.

## Verification

The focused contract prepares an isolated Recipe/Validation Set fixture, subscribes
to the existing `PropertyChanged` surface, invokes `RefreshOptions`, and requires
each of the ten shared command-state properties to be notified exactly once.

Recorded checks for this slice:

- Runner Debug build (`dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform=x64 --no-restore`): 0 warnings, 0 errors.
- Runner Release build (`dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Release -p:Platform=x64 --no-restore`): 0 warnings, 0 errors.
- `--refresh-options-command-state-contract`: Debug
  `CONTRACT|refresh-options-command-state|passed=1|failed=0` at
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl53-refresh-options-command-state-contract-debug-20260909-run3\refresh-options-command-state-contract.txt`;
  Release `passed=1|failed=0` at
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl53-refresh-options-command-state-contract-release-20260909-run2\refresh-options-command-state-contract.txt`.
- Existing OVL-50, OVL-51, and OVL-52 Release regressions each returned
  `passed=1|failed=0`: `ovl50-validation-set-evidence-notification-regression-20260909-run2`,
  `ovl51-validation-set-projection-error-regression-20260909-run2`, and
  `ovl52-validation-set-success-projection-regression-20260909-run2` under the
  same D: evidence root.
- The OVL-52 reflection contract now invokes the private refresh method with its
  explicit `refreshCommandState: true` policy parameter so the direct success
  branch remains covered after the composite seam was added; public bindings and
  runtime contracts are unchanged.
- Embedded OpenVisionLab x64 Debug and Release builds: 0 warnings, 0 errors.
- `OpenVisionReadinessCheck`: passed; `Invoke-RefactorAudit.ps1 -Verify`:
  `REFACTOR_AUDIT=PASS|CSharpFiles=837|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`;
  `TestDocumentationIndex.ps1`:
  `DocumentationIndex=PASS IndexedPaths=272 Routes=13 RootRedirects=102`;
  JSON parse/reference check: passed; `git diff --check`: passed.

Runtime Shell EXE rendering, binding visuals, themes/layouts, DPI/monitor,
popup/dialog bounds, and full Preview/Run UI were not exercised by this source and
contract slice. Therefore the source conclusion is:

`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`

## Boundary lock

OVL-53 closes this composite command-state boundary. Do not move, split, merge,
rename, or re-abstract it merely for file length or readability. Reopen only for a
new requirement, reproducible defect, failed completion criterion, or changed
dependency boundary, with focused proof that the existing owner cannot satisfy the
change. Other models, agents, and automations must not regenerate or duplicate this
slice without that recorded reason.

The Dev worktree remains dirty by design. This slice does not stage, commit, push,
or modify `C:\Git\2D\Original`, and it does not create or reactivate schedules.
