# Learn Matching Simulation Model Refactor Proof (2026-07-26)

## Status

Complete.

## Scope

- Move the deterministic Matching and Feature Matching lesson data and
  evaluation calculations out of `OpenVisionLearnWindow.xaml.cs`.
- Keep the WPF view responsible for controls, timers, animation progress,
  localized explanation text, and rendering.
- Preserve the exact search/template samples, candidate positions, template
  equality score, best-candidate selection, score threshold, feature points,
  descriptor scores, Good Match threshold, required-count gate, and RANSAC
  illustration.

## Excluded

- No XAML, user-visible product text, timer cadence, topic order, Tool View
  handoff, algorithm-tool runtime, Preview/Run, layer, or routing change.
- No interface, factory, dependency injection container, generic matching
  framework, or partial class was added.
- This is a deterministic Learn simulation boundary, not a replacement for
  production Matching, EdgeBasedMatching, or FeatureMatching algorithms.

## Structural Changes Confirmed

- Before:
  `OpenVisionLearnWindow` owned the fixed matching/feature samples, template
  candidate score calculation, best-result selection, descriptor Good Match
  classification, count gate, and pass/fail decisions.
- After:
  `OpenVisionLearnMatchingSimulationModel` owns the scenario data and returns
  typed template/feature evaluation results without a WPF dependency.
  `OpenVisionLearnWindow` passes slider values and paints those results.
- The view fell from 4,834 to 4,801 lines. The reduction is a consequence of
  responsibility movement, not the acceptance criterion.
- Readiness now checks the new owner, both view-to-model call paths, and the
  absence of the old view-owned calculation/data fields.

## Call Path

- Old:
  WPF event/timer -> view-owned sample and score evaluation -> view rendering.
- New:
  WPF event/timer -> `OpenVisionLearnMatchingSimulationModel` evaluation ->
  view rendering.
- State/data owner:
  animation/timer/control state remains in the view; matching lesson samples
  and deterministic evaluation results belong to the model.

## Acceptance Criteria

1. The model owns Matching samples, candidate scoring, best selection, and
   threshold result.
2. The model owns Feature Matching points/scores, Good Match classification,
   required-count normalization, and result.
3. The view no longer contains the moved fields or
   `CalculateTemplateScore`.
4. Current-source Matching and Feature Matching Learn targets pass before and
   after with pixel-identical output.
5. Debug build, readiness, and patch hygiene pass.

## Checks Run

- The pre-change UI attempt exposed three stale smoke assertions that expected
  retired English status text and an initial `ResultCount` detail that the
  current Feature Matching panel does not show. The smoke contract was aligned
  to the current Korean UI and its actual initial `GoodMatches` evidence before
  the production move.
- Before current-source UI baseline:
  `wpf_openvision_learn_matching` and
  `wpf_openvision_learn_feature_matching` passed under
  `artifacts/refactor_learn_matching_simulation_model_20260726/before`.
- After current-source UI:
  the same two targets passed under
  `artifacts/refactor_learn_matching_simulation_model_20260726/after`.
- Matching before/after PNG SHA-256:
  `DF701E6E2BFF37D1489FCA664E79FB7B6EAFECE582E0BFB1E8DFC8BEB8FF16E1`.
- Feature Matching before/after PNG SHA-256:
  `55AA6330D29A4404C8FE4C4F6A8A62E6D0F0AF560E58AF7F001A6C45BE4459D6`.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`:
  passed with 0 warnings and 0 errors.
- `dotnet run --project
  "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj"
  -c Debug --no-build`: passed.
- `git diff --check`: passed.

## Evidence

- `UI/VisionTest/Wpf/Learn/OpenVisionLearnMatchingSimulationModel.cs`
- `artifacts/refactor_learn_matching_simulation_model_20260726/before`
- `artifacts/refactor_learn_matching_simulation_model_20260726/after`

## Boundary / Next Dependency

This proves the calculation/data owner and call path changed for the two
matching lessons while their UI remained pixel-identical. It does not make the
full Learn window MVVM-complete. The next structural audit should examine
whether the Line/LineDistance lesson calculations form one cohesive non-WPF
simulation owner. Do not split event handlers, timers, or rendering into
partial files merely to reduce line count.
