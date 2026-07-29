# OpenVisionLab Overlay Rendering v1 Contract

Updated: 2026-07-29 KST  
Queue item: CVR-20  
Status: Complete

## Purpose

Make dense `OverlayMerge` review evidence readable without changing the
inspection result. Rendering remains owned by the existing `OverlayMerge`
Step; this contract adds no new algorithm family and no visualization script
surface.

## Operator Workflow

1. Select an existing `OverlayMerge` Step in Recipe Manager.
2. Load its parameters once.
3. Review all source and display settings in the same PropertyGrid.
4. Choose a bounded palette, label mode, line width, point size, label
   background, and label margin.
5. Select **Apply to XML** to persist the setup with the recipe.
6. Reopening the same Step restores the saved values visibly.
7. Select **Display defaults** to restore the backward-compatible defaults,
   then select **Apply to XML** to save that reset.
8. Preview or Run remains a separate explicit operator action.

Changing, applying, restoring, or resetting these properties must not Preview,
Run, create/delete/select a layer, or change input/output routing.

## Persisted Parameters

| XML parameter | Allowed value | Missing-key behavior |
| --- | --- | --- |
| `RenderPreset` | `LegacyDefault`, `HighContrast`, `ColorBlindSafe` | `LegacyDefault` |
| `LabelMode` | `None`, `Name`, `NameWithCoordinates` | Derive from legacy `DrawLabels`: `true` -> `Name`; otherwise `None` |
| `LineWidth` | integer `1..8` image pixels | `2` |
| `PointSize` | integer `1..12` image pixels | `4` |
| `LabelBackground` | Boolean | `false` |
| `LabelMargin` | integer `0..12` image pixels | `0` |

Existing `SourceLayers`, `SourceSteps`, `BurnIn`, `AllowEmpty`, `MaxPoints`,
and unrepresented parameters remain intact. PropertyGrid save also writes
legacy `DrawLabels` from `LabelMode` for older readers.

`NameWithCoordinates` appends the overlay evidence center in source image
pixel coordinates. It is not a calibrated/world-coordinate value.

## Display-only Invariant

For the same input, source Steps, and source overlays, changing any v1
rendering parameter may change only burned-in output pixels. It must preserve:

- tool success/failure;
- `ResultCount`, `MergeOverlayCount`, and `MergeSourceCount`;
- returned overlay kind, geometry, label, and count;
- Step acceptance and Pipeline outcome.

Invalid enum, Boolean, or bounded integer values fail semantic validation.

## Evidence Retention

Saved Pipeline XML, Pipeline snapshots, and Run Report Step parameters retain
the exact rendering setup used for the evidence. This records how an image was
rendered without treating style as inspection truth.

## Acceptance Checklist

- [x] Missing new keys reproduce the legacy `DrawLabels=true` output pixel for
  pixel.
- [x] Bounded presets and coordinate labels visibly change only rendered
  pixels.
- [x] Metrics, returned overlays, and acceptance remain identical across
  presets.
- [x] PropertyGrid save/reload/reopen restores the recipe-scoped setup.
- [x] **Display defaults** restores and persists the legacy-compatible
  defaults.
- [x] Apply/reset/reopen causes zero Preview/Run, layer, active-layer, or route
  changes.
- [x] Run Report and Pipeline snapshot retain the rendering parameters.
- [x] Out-of-range values fail closed.

## Boundaries

This is bounded image-pixel presentation control. It does not add arbitrary
colors, arbitrary drawing code, scripting, calibrated labels, annotation
editing, inspection-gate changes, field qualification, or a general
visualization platform.

