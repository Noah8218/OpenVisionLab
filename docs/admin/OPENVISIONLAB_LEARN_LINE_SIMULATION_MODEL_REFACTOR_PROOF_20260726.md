# Learn Line Simulation Model Refactor Proof (2026-07-26)

## Status

Complete.

## Scope

- Move the deterministic Edge/Line and LineDistance lesson data and
  calculations out of `OpenVisionLearnWindow.xaml.cs`.
- Keep the WPF view responsible for controls, timers, animation progress,
  localized explanation text, and rendering.
- Preserve the exact 5x5 gray-value sample, horizontal gradient rule, edge
  threshold normalization, vertical run selection, left/right edge samples,
  pixel distances, average/range, fixed `0.006 mm/px` lesson conversion, and
  range gate.

## Excluded

- No XAML, user-visible product text, timer cadence, topic order, Tool View
  handoff, production Line/LineDistance algorithm, Preview/Run, layer, or
  routing change.
- No interface, factory, dependency injection container, generic metrology
  framework, or partial class was added.
- This is a deterministic Learn simulation boundary, not calibration,
  certified metrology, or production-algorithm evidence.

## Structural Changes Confirmed

- Before:
  `OpenVisionLearnWindow` owned the Edge/Line and LineDistance samples,
  duplicated gradient/run evaluation, duplicated distance statistics, mm
  conversion, and pass/fail decision.
- After:
  `OpenVisionLearnLineSimulationModel` owns both line-family lesson scenarios
  and returns typed `EdgeLineEvaluation` and `LineDistanceEvaluation` values
  without a WPF dependency. The view passes slider values and paints the
  returned results.
- The view fell from 4,801 to 4,718 lines. The reduction is a consequence of
  responsibility movement, not the acceptance criterion.
- Readiness checks the new owner, both view-to-model call paths, and removal of
  the old view-owned sample fields.

## Call Path

- Old:
  WPF event/timer -> duplicated view-owned line calculation -> view rendering.
- New:
  WPF event/timer -> `OpenVisionLearnLineSimulationModel` evaluation -> view
  rendering.
- State/data owner:
  animation/timer/control state remains in the view; deterministic line lesson
  samples and evaluation results belong to the model.

## Acceptance Criteria

1. The model owns Edge/Line threshold normalization, gradients, edge flags,
   best column, and best run.
2. The model owns LineDistance samples, pixel statistics, lesson mm metrics,
   and range decision.
3. The view no longer owns the moved arrays or duplicated calculations.
4. Current-source Edge/Line and LineDistance UI targets preserve the exact
   numeric lesson contracts and visible workflow.
5. Debug build, readiness, structural search, and patch hygiene pass.

## Checks Run

- Before current-source UI baseline:
  `wpf_openvision_learn_edge_line` and
  `wpf_openvision_learn_line_distance` passed under
  `artifacts/refactor_learn_line_simulation_model_20260726/before`.
- The first post-move build found one remaining reference to the old local
  `strengths` variable. It was changed to `evaluation.Strengths`; the final
  build passed with 0 warnings and 0 errors.
- The UI smoke now fixes exact calculation outputs:
  Edge threshold `85`, `LineRun = 5 px`, `DistancePxAvg=4.2`,
  `DistancePxRange=1`, `DistanceMmAvg=0.025`,
  `DistanceMmRange=0.006`, and `DistanceMmMax=0.030`.
- After current-source UI:
  both targets passed under
  `artifacts/refactor_learn_line_simulation_model_20260726/after_contract`.
- Before/after PNG hashes differed. An unchanged repeated Edge/Line capture
  also produced a different hash, while repeated LineDistance after captures
  were identical. Direct review of all before/after images found the same
  layout, text, values, controls, and visible state; semantic and exact-value
  smoke checks passed.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`:
  final pass with 0 warnings and 0 errors.
- `dotnet run --project
  "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj"
  -c Debug --no-build`: passed.
- Structural search found no old Edge/Line or LineDistance sample field or
  duplicated calculation in the view.
- `git diff --check`: passed.

## Evidence

- `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/OpenVisionLearnLineSimulationModel.cs`
- `artifacts/refactor_learn_line_simulation_model_20260726/before`
- `artifacts/refactor_learn_line_simulation_model_20260726/after`
- `artifacts/refactor_learn_line_simulation_model_20260726/after_repeat`
- `artifacts/refactor_learn_line_simulation_model_20260726/after_contract`

## Boundary / Next Dependency

This proves the calculation/data owner and call path changed for the two
line-family lessons. It does not make the full Learn window MVVM-complete.
The next structural audit may assess whether Brightness, Arithmetic, and
Filtering form one cohesive basic grayscale simulation owner. Do not extract
Metrics Acceptance, Color/HSV, event handlers, timers, or rendering merely for
file size or symmetry.
