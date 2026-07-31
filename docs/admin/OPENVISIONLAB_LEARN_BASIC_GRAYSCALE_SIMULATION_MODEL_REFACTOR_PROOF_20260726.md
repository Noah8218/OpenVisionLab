# Learn Basic Grayscale Simulation Model Refactor Proof (2026-07-26)

## Status

Complete.

## Scope

- Move deterministic Brightness, Arithmetic, and Filtering lesson data and
  calculations out of `OpenVisionLearnWindow.xaml.cs`.
- Keep the WPF view responsible for controls, timers, animation progress,
  localized explanation text, and rendering.
- Preserve the exact brightness offset/clamp/histogram/average behavior, five
  arithmetic modes, and Mean/Median/Sharpen sample calculations.

## Excluded

- No XAML, user-visible product text, topic order, Tool View handoff,
  production algorithm, Preview/Run, layer, or routing change.
- No interface, factory, dependency injection container, generic image
  framework, or partial class was added.
- The view's existing `ClampToByte` remains because Threshold and HSV/color
  rendering still use it; this refactor does not create a shared utility only
  for symmetry.

## Structural Changes Confirmed

- Before:
  `OpenVisionLearnWindow` owned all three fixed sample sets, brightness result
  and histogram calculation, arithmetic pixel operations, and filter
  center-result calculation.
- After:
  `OpenVisionLearnBasicGrayscaleSimulationModel` owns those samples and
  returns typed Brightness, Arithmetic, and Filter evaluations without a WPF
  dependency. The view passes selected values/modes and paints the results.
- The view fell from 4,718 to 4,706 lines. The line count is not the acceptance
  criterion; the owner and call path change are.
- Readiness checks the model, all three call paths, and removal of the old
  view-owned fields and arithmetic calculation.

## Call Path

- Old:
  WPF event/timer -> view-owned grayscale calculation -> view rendering.
- New:
  WPF event/timer -> `OpenVisionLearnBasicGrayscaleSimulationModel` -> view
  rendering.
- State/data owner:
  timer/control/selection state remains in the view; fixed lesson samples and
  deterministic results belong to the model.

## Acceptance Criteria

1. The model owns Brightness results, histogram bins, and source/result
   averages.
2. The model owns Add, Subtract, AbsDiff, Bitwise AND, and Bitwise OR results.
3. The model owns Mean, Median, and Sharpen filter results plus sorted/sum
   evidence used by the lesson.
4. The view no longer owns the moved arrays or arithmetic calculation.
5. Current-source UI output is preserved and exact numeric contracts pass.
6. Debug build, readiness, structural search, and patch hygiene pass.

## Checks Run

- Initial pre-change Brightness and Arithmetic captures exposed stale smoke
  assertions that depended on damaged/retired initial copy and an old
  `Preview/Run` animation status. Assertions were aligned to the current
  stable parameter title, practice copy, and OutputLayer step before the
  production move.
- Before baseline:
  `wpf_openvision_learn_brightness`,
  `wpf_openvision_learn_arithmetic`, and
  `wpf_openvision_learn_filtering` passed under
  `artifacts/refactor_learn_basic_grayscale_model_20260726/before`.
- The Brightness smoke fixes exact average movement `116 -> 83` and direction
  `왼쪽`; Arithmetic already checks all five first-pixel expressions; Filtering
  checks Mean sum `677 / 9`, Median output `59`, and Sharpen expression.
- After current-source UI:
  all three targets passed under
  `artifacts/refactor_learn_basic_grayscale_model_20260726/after_contract`.
- Every before/after PNG pair has the same SHA-256:
  Brightness
  `7810B58FCE02C3CA08DFEE2684B1FA611975F8495180C8872BAF6759C328DCFC`,
  Arithmetic
  `9AF2944B5308E16902E3AF9AC9D3A5EBA399527F3139B0DA3DECF3507040EB95`,
  and Filtering
  `ECABD6818C8955E454BAB2EEF9B08C299B518B74D313AB194CFC01F3FC5E597D`.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`:
  passed with 0 warnings and 0 errors.
- `dotnet run --project
  "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj"
  -c Debug --no-build`: passed.
- Structural search found no moved sample field or
  `CalculateArithmeticResult` in the view.
- `git diff --check`: passed.

## Evidence

- `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/OpenVisionLearnBasicGrayscaleSimulationModel.cs`
- `artifacts/refactor_learn_basic_grayscale_model_20260726/before`
- `artifacts/refactor_learn_basic_grayscale_model_20260726/after_contract`

## Boundary / Next Dependency

This proves one cohesive basic grayscale calculation owner. It does not make
the full Learn window MVVM-complete. Remaining calculations are small,
topic-specific Threshold, Metrics Acceptance, Layer Recipe, Geometry
Transform, and Color/HSV logic. Perform one closure audit before another move;
do not create a generic grab-bag model or split event handlers, timers, and
rendering merely to reduce file size.

## Threshold Closure Addendum

The final closure audit found that Threshold is not a separate model boundary:
it is the same fixed grayscale sample/clamp/pixel-transform responsibility.
`OpenVisionLearnBasicGrayscaleSimulationModel` now also owns Threshold samples
and Binary/BinaryInv evaluation. The current Threshold UI smoke, Debug build,
readiness, structural search, and visual comparison passed. Detailed closure
and the retained-topic decisions are recorded in
`OPENVISIONLAB_LEARN_MODEL_EXTRACTION_CLOSURE_20260726.md`.
