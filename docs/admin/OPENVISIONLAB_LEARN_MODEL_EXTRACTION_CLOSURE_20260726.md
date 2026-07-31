# Learn Model Extraction Closure (2026-07-26)

## Status

Complete.

## Scope

- Audit the remaining Threshold, Metrics Acceptance, Layer Recipe, Geometry
  Transform, and Color/HSV calculations in `OpenVisionLearnWindow`.
- Move Threshold into an existing cohesive owner when justified.
- Record why the other topic-local logic should remain in the view.
- Close proactive Learn extraction rather than creating a generic model or
  partial files for line count.

## Structural Decision

### Threshold: moved

Threshold uses the same fixed grayscale samples, byte clamping, and per-pixel
transformation responsibility already owned by
`OpenVisionLearnBasicGrayscaleSimulationModel`.

- Old path:
  WPF control -> view-owned sample/threshold calculation -> view rendering.
- New path:
  WPF control -> `EvaluateThreshold` -> view rendering.
- The view no longer owns `sampleValues`.
- No new file, interface, factory, partial, or dependency direction was added.

### Metrics Acceptance: retained

This is one five-value presentation scenario. Its average/range/maximum are
used only to drive the same animation and outlier coloring. A separate model
would be a thin one-topic wrapper with no demonstrated second consumer.

### Layer Recipe: retained

The arrays are presentation content for selecting and highlighting four
educational routing rows. The logic mutates WPF cell colors and slider state;
it is not pipeline execution or domain recipe state.

### Geometry Transform: retained

The only pure values are two display-size multiplications. The rest directly
drives WPF rotate/scale transforms and animation state. A result record would
add indirection without moving a meaningful domain responsibility.

### Color/HSV: retained

The lesson combines slider normalization, fixed channel examples, WPF
`Color`/brush rendering, and step-specific channel highlighting. Splitting a
few booleans while leaving WPF color conversion and painting in the view would
not create a complete owner. Reassess only if a second non-WPF consumer or a
real color-simulation test need appears.

## Acceptance Criteria

1. Threshold samples and Binary/BinaryInv result calculation belong to the
   existing basic grayscale model.
2. The view no longer owns Threshold sample data.
3. Remaining non-extracted logic has a concrete owner/size/dependency reason.
4. No generic grab-bag model or file-size partial is introduced.
5. Current Threshold UI, Debug build, readiness, and patch hygiene pass.

## Verification

- `wpf_threshold_tool_guide` passed before and after under
  `artifacts/refactor_learn_threshold_model_closure_20260726`.
- Before/after PNG hashes differed. Direct comparison showed identical layout,
  text, values, controls, and state; only the accordion disclosure glyph
  rendered differently. The semantic smoke passed in both captures.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`:
  0 warnings, 0 errors.
- OpenVision readiness: passed.
- Structural search: the view contains the `EvaluateThreshold` call and no
  `private readonly int[] sampleValues`.
- `git diff --check`: passed.

## Evidence

- `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/OpenVisionLearnBasicGrayscaleSimulationModel.cs`
- `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/OpenVisionLearnWindow.xaml.cs`
- `artifacts/refactor_learn_threshold_model_closure_20260726/before`
- `artifacts/refactor_learn_threshold_model_closure_20260726/after`

## Boundary / Next Dependency

Proactive Learn calculation extraction is closed. Reopen only for a concrete
maintenance change, a verified regression, a second non-WPF consumer, or a
demonstrated calculation owner that is currently mixed into WPF. Do not
continue extracting Metrics, Layer Recipe, Geometry, Color/HSV, event
handlers, timers, or rendering merely to reduce `OpenVisionLearnWindow` line
count.
