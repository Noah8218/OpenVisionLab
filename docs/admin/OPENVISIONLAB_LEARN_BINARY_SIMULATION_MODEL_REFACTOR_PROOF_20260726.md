# Learn Binary Simulation Model Refactor Proof (2026-07-26)

## Status

Complete.

## Scope

- Move the deterministic Morphology, Blob, and Contour sample calculations
  out of `OpenVisionLearnWindow.xaml.cs`.
- Keep the WPF view responsible for topic selection, timers, controls,
  animation progress, localized explanation text, and rendering.
- Preserve the exact fixed sample values, 3x3 morphology boundary behavior,
  8-neighbor connected components, area filtering, four-neighbor contour
  extraction, bounding-box drawing, and visible Learn workflow.

## Excluded

- No XAML, user-visible text, timer cadence, topic order, Tool View handoff,
  public sample, algorithm-tool runtime, Preview/Run, layer, or routing change.
- No interface, factory, dependency injection container, generic image
  framework, or partial class was added.

## Structural Changes Confirmed

- Before:
  `OpenVisionLearnWindow` directly owned erosion, dilation, connected-component
  flood fill, contour extraction, bounds calculation, and bound-edge tests.
- After:
  `OpenVisionLearnBinarySimulationModel` owns those pure calculations without
  a WPF dependency. `OpenVisionLearnWindow` supplies the fixed sample/mode and
  paints the returned arrays.
- Evidence:
  the view contains model call sites but no `FloodFillBlob`, `Erode`, or
  `FindContourPixels` implementation; readiness enforces both sides.

## Call Path

- Old:
  WPF event/timer -> view-owned calculation -> view rendering.
- New:
  WPF event/timer -> `OpenVisionLearnBinarySimulationModel` -> view rendering.
- State/data owner:
  animation/timer/control state remains in the view; deterministic binary
  transformation results are produced by the model.

## Acceptance Criteria

1. The model owns morphology, connected-component labeling, contour pixels,
   bounds, and bounding-edge calculations.
2. The view no longer contains the moved implementations.
3. Current-source Morphology, Blob, and Contour Learn targets pass before and
   after with no Preview/Run, layer, or routing change.
4. Debug build, readiness, and patch hygiene pass.

## Checks Run

- Before current-source UI baseline:
  `wpf_openvision_learn_morphology`,
  `wpf_openvision_learn_blob`, and
  `wpf_openvision_learn_contour` passed under
  `artifacts/refactor_learn_binary_simulation_model_20260726/before`.
- After current-source UI:
  the same three targets passed under
  `artifacts/refactor_learn_binary_simulation_model_20260726/after`.
- Morphology and Blob before/after PNG SHA-256 values matched exactly.
- Contour PNG hashes differed, but a second unchanged after capture also
  differed. The varying pixels were limited to the animated practice accordion
  region; both semantic smoke assertions passed and direct visual review found
  no layout or content regression.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`:
  passed with 0 warnings and 0 errors.
- `dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`:
  initially failed only because the strict Learn owner-file allowlist did not
  include the new model; the allowlist and structural ownership assertions
  were updated, and the rerun passed.
- `git diff --check`: passed.

## Evidence

- `UI/VisionTest/Wpf/Learn/OpenVisionLearnBinarySimulationModel.cs`
- `artifacts/refactor_learn_binary_simulation_model_20260726/before`
- `artifacts/refactor_learn_binary_simulation_model_20260726/after`

## Boundary / Next Dependency

This proves a real calculation-owner and call-path change for the three binary
learning simulations. It does not make the full Learn window MVVM-complete.
Other topic calculations should move only as separately cohesive,
test-covered simulation responsibilities; do not split event handlers or
timers into partial files merely to reduce line count.
