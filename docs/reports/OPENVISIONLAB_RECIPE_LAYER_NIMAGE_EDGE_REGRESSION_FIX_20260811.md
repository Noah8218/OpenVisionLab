# OpenVisionLab Recipe, Layer, N-image, And Edge Regression Fix

Date: 2026-08-11 KST

Repository: `C:\Git\OpenVisionLab_Dev`

Source baseline: `e396df85`

## Scope

This batch closes the reported Recipe, layer image-load, N-image verification,
and EdgeBasedMatching UI regressions without changing Preview/Run, layer
creation, active-route, or pipeline execution contracts.

## Completed Behavior

1. A layer-list right-click selects the clicked row before its context command
   runs. Loading an image therefore writes to that exact layer; `Main` remains
   unchanged when another layer was clicked.
2. Recipe create/switch exposes a visible switching state and retains the
   requested Recipe/Pipeline context.
3. Recipe save is available from the shell toolbar and `Ctrl+S`. A pending
   PropertyGrid edit is committed before the Recipe save request.
4. Workspace, Tool View, layer, and N-image file/folder dialogs reuse the most
   recently used image directory when it is still valid.
5. EdgeBasedMatching separates its guide from Auto MPoint teaching controls.
6. The N-image window uses valid Korean/English labels instead of damaged text.
7. Added N-image files appear as selectable `대기`/`READY` rows immediately;
   source evidence is visible before explicit sequential Run.
8. N-image results distinguish execution `ERROR`, acceptance `NG`, judged
   `OK`, and ungated `RUN OK`. The selected NG row shows its failed Step,
   reason/message, metric evidence, source hash, and retained drawing.
9. N-image window labels, column headers, action names, status explanations,
   and HTML report structure switch with the current language.
10. All N-image actions changed in this batch have localized tooltips and
    accessible names.
11. EdgeBasedMatching starts with the operator-facing parameter set. Advanced,
    scale, and dependent search parameters are revealed only by their owning
    option.
12. Changing compact/advanced visibility does not auto-run Preview and keeps
    the PropertyGrid hierarchy intact.

A failure-path regression was also fixed: an XML save/round-trip failure no
longer refreshes the selected Step while its dirty editor is retained. That
refresh previously reopened a second pending-edit dialog and could make a
Recipe switch look unresponsive.

## Verification

- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU" --no-restore`
  - passed, 0 warnings, 0 errors.
- `dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"`
  - passed all 13 readiness contracts.
- Current-source focused WPF captures passed:
  - `wpf_shell_host_layer_management_commands`
  - `wpf_shell_host_recipe_manager_summary`
  - `wpf_shell_host_recipe_change_safety`
  - `persisted_workspace_preferences_contract`
  - `wpf_shell_host_edge_based_matching_tool`
  - `wpf_tool_n_image_verification_window`
  - `localization_catalog_contract_check`
- `VisionRecipeRunnerSmoke --tool-n-image-verification-contract`
  - passed for Threshold, Blob, Line, Matching, EdgeBasedMatching, and
    AffineTransform with 30 images per Tool.
  - also passed the acceptance-gated NG/reason contract and Korean/English HTML
    report checks.
- `git diff --check`
  - passed before the completion record was written; rerun at handoff.

## Evidence

- Current WPF captures:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\artifacts\recipe_layer_nimage_edge_20260811\final_current`
- Current Recipe Manager and failed-save/round-trip transition captures:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\artifacts\recipe_layer_nimage_edge_20260811\final_current_4`
- Current clicked-layer, EdgeBasedMatching, and NG-focused N-image captures:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\artifacts\recipe_layer_nimage_edge_20260811\final_current_3`
- The focused screenshot targets delete their generated 12-hex-suffix Recipe
  workspaces after capture.
- Six-Tool N-image contract:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\artifacts\recipe_layer_nimage_edge_20260811\n_image_contract_final_current_2`
- Closest available before evidence is the two user-provided screenshots copied
  to `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\artifacts\recipe_layer_nimage_edge_20260811\baseline_user_reported`.
  They are not fresh current-build before captures because implementation had
  already started before the baseline was preserved.

## Completion Record

```text
Status: Complete
Scope: Exact clicked-layer image load, Recipe switching/save/path persistence, EdgeBasedMatching layout/parameter hierarchy, and N-image list/judgment/localization/evidence behavior in Dev
Acceptance criteria: 12 reported behaviors -> passed by focused current-source UI/contract checks; no automatic Preview/Run/layer/routing side effects -> passed
Verification: Debug solution build 0 warnings/errors; readiness 13/13; seven focused WPF targets passed; six Tool families x 30 N-image contract plus gated NG passed
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\artifacts\recipe_layer_nimage_edge_20260811 and this report
Boundary / next dependency: Implementation is pushed as Dev dc08dde5f42a and original a6bbf277dea4; PR and Release publication were not requested
```

## Repository Promotion

- Dev implementation commit:
  `dc08dde5f42ab9264a4696e08c920fe338015bbf`
- Original implementation commit:
  `a6bbf277dea44e56e773ccf2c8cf954a8de8e131`
- Both implementation commits have stable patch-id
  `a6ccfdcf57ed767807cc85f5aaaa0c866839048d`; all 27 resulting file blobs
  matched before push.
- The original repository independently passed the Debug build, readiness
  13/13, six-Tool x 30-image N-image contract, and seven focused WPF targets.
- Remote branch heads were read back after push and matched both local
  implementation commits.
