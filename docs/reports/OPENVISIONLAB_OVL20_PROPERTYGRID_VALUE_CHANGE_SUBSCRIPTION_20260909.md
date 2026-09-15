# OpenVisionLab OVL-20 PropertyGrid value-change subscription owner — 2026-09-09

Status: **Complete for the single WpfPropertyGridAdapter property-value change
subscription lifetime slice**.

## Scope

`WpfPropertyGridAdapter` previously owned the `PropertyItem.ValueChanged` and
`INotifyPropertyChanged.PropertyChanged` subscriptions, their handler
dictionaries, and the last-value snapshot used to suppress duplicate
notifications. The adapter still owns the public PropertyGrid bridge and
projects changes through `RaisePropertyValueChanged`, but the subscription
lifetime and snapshot state now have one concrete owner:
`PropertyGridPropertyValueChangeSubscription`.

```text
PropertyGrid.SelectedObject / RefreshSelectedObject
  -> PropertyGridPropertyValueChangeSubscription.Detach()
  -> adapter assigns/rebuilds WPG PropertyItems
  -> PropertyGridPropertyValueChangeSubscription.Attach(Properties)
  -> WPG PropertyItem events
  -> existing adapter RaisePropertyValueChanged projection
```

`Attach` owns the two source event subscriptions and takes the initial value
snapshot. `Detach` removes both subscriptions and clears every registry before
the selected object is replaced. The existing `PropertyValue` equality guard,
old/new value projection, and range-editor value read remain behaviorally the
same. The public bridge, PropertyGrid attributes, extern alias, selected-object
binding, and the previously completed `PropertyGridToolPolicy` boundary were
left intact.

## Changed files

- `src/Libraries/WpfPropertyGridBridge/PropertyGridPropertyValueChangeSubscription.cs`
  - New internal concrete owner for WPG event handlers and last-value snapshots.
  - Provides `Attach`, `Detach`, and the shared safe `PropertyItem` value read.
- `src/Libraries/WpfPropertyGridBridge/WpfPropertyGridAdapter.cs`
  - Composes the owner, detaches before both selected-object replacement paths,
    and attaches the current WPG property collection afterward.
  - No longer stores the old handler dictionaries or registration methods.
- `tools/VisionRecipeRunnerSmoke/PropertyGridPropertyValueChangeSubscriptionContract.cs`
  - Window-free structural contract covering ownership, event wiring,
    equality behavior, composition, call counts, and stale-owner removal.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - Adds `--property-grid-value-change-subscription-contract` dispatch.

No Recipe/XML, Preview/Run, Layer/ImageSpace, Shell command, or visual-theme
policy was changed in this slice. The owner extraction does not add disposal or
change the existing selected-object replacement order.

## Verification

- `WpfPropertyGridBridge` Debug build: **0 warnings / 0 errors**.
- `WpfPropertyGridBridge` Release build: **0 warnings / 0 errors**.
- `VisionRecipeRunnerSmoke` Debug build: **0 warnings / 0 errors**.
- `VisionRecipeRunnerSmoke` Release build: **0 warnings / 0 errors**.
- New structural contract: **8/8 passed** in Debug and Release. Evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl20-property-grid-contract-20260909\`
  (`contract-debug.log`, `contract-release.log`, and per-run contract text).
- `PipelineViewerScreenshotSmoke` Debug and Release builds completed with
  **0 errors** and one pre-existing nullable warning at
  `tools/PipelineViewerScreenshotSmoke/Program.cs:10792`.
- `wpf_property_grid_matching_combo` desktop smoke: **OK** in Debug and
  Release. It exercised the Matching PropertyGrid combo/template/popup,
  range editor, and child-row policy and captured:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl20-property-grid-runtime-20260909\debug\wpf_property_grid_matching_combo.png`
  and the corresponding `release` path.
- The current Release capture is 760x520 and has zero changed sampled pixels
  against the closest reproducible OVL-08 post-policy capture (24,700 samples,
  delta threshold 30). This is a historical comparison, not a rebuilt
  pre-OVL-20 baseline; visual evidence is therefore limited to the current
  runtime and the recorded comparison.
- Dynamic monitor evidence recorded one logical monitor
  `\\.\DISPLAY2`, bounds 1920x1080, working area 1920x1032. The Debug window
  rectangle was `104,104-864,624` and the Release rectangle was
  `156,156-916,676`; both intersected the selected monitor. Evidence is in
  `monitor-topology-debug.txt`, `monitor-topology-release.txt`, and the two
  `window-evidence-*.txt` files under the runtime directory.
- The current refactor audit passed with `CSharpFiles=786`, `XamlFiles=59`,
  `PartialDeclarations=108`, `Projects=27`, `Cycles=0`, and
  `ShellStorageCalls=0`. Evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl20-property-grid-contract-20260909\refactor-audit\`.
- `git diff --check` passed for the selected source/report set before the
  selective commit; unrelated dirty worktree changes remain preserved.

The runtime exercise covered the normal Matching grid, selected child row,
combo popup/template, and range editor. Hover, pressed, keyboard focus,
disabled/read-only, validation-error, alternate themes/layouts, and 100/125/
150/175/200% DPI were not exercised. **소스 코드 기준 검토 완료 / 실제
Runtime UI 검증 필요** for those remaining states.

## Junior readability assessment

**PASS for this slice.** A maintainer can follow one named owner from the two
selected-object replacement call sites to `Attach`/`Detach`, then to the WPG
events and the existing adapter projection. The owner has no Window, Recipe,
file-I/O, or policy dependency, and the adapter no longer mixes subscription
registries with its generic rendering and navigation code.

Do not let another model or agent recreate this owner, move the same event
handlers again, or split the adapter on file size alone without a newly
reproduced lifetime defect, a changed explicit contract, or a proven
responsibility conflict. The existing `PropertyGridToolPolicy` extraction is
already closed and must not be reworked as part of this slice.

## Boundary and next priority

This slice does not complete the full PropertyGrid adapter or the remaining
refactoring program. Public WPG compatibility, generic visual/navigation code,
and the compatibility-safe namespace/project boundary remain separate work.

Next priority: compatibility-safe namespace/project boundary review and one
independently testable migration candidate.

Recommended model: `gpt-6-astra` | Reasoning effort: `high`.

## Completion record

```text
Status: Complete
Scope: WpfPropertyGridAdapter property-value change subscription lifetime owner
Acceptance: concrete owner + attach/detach registries + adapter composition + 8/8 contract + Debug/Release build + focused WPF smoke
Verification: bridge/runner Debug and Release builds; structural contract 8/8; wpf_property_grid_matching_combo Debug/Release; monitor and visual comparison evidence
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl20-property-grid-contract-20260909 and ovl20-property-grid-runtime-20260909
Boundary: alternate WPF states/themes/layouts/DPI and the remaining adapter/namespace roadmap are not proven by this slice
```
