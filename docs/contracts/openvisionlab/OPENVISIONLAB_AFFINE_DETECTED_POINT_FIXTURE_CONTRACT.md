# OpenVisionLab Affine Detected-Point Fixture Contract

## Purpose

This contract extends the fixed numeric three-point `AffineTransform` from P218
without changing the OpenVisionLab Vision SDK algorithm:

```text
three earlier deterministic Point results
    -> current-image source points
    -> OpenVisionLab Vision SDK AffineTransform
    -> reference-coordinate output layer
    -> unchanged downstream ROI/inspection
```

Typical producers are three separately constrained `Matching` Steps. Each
successful one publishes its single detected `Center` as a typed `Point`.
Line endpoints/midpoints, CircleGauge centers, and GeometryMeasure Point outputs
can be used by the same contract.

## Ownership

- OpenVisionLab Vision SDK remains authoritative for the affine point/output/gate checks,
  `GetAffineTransform`, `WarpAffine`, output coverage, matrix/decomposition
  metrics, stable error codes, and destination/frame drawings.
- OpenVisionLab owns the same-run typed-Point references, source Step/result
  acceptance checks, coordinate-frame checks, PropertyGrid/XML persistence,
  runtime source-coordinate injection, and source-point provenance metrics.
- The original fixed `SourcePoint*X/Y` mode remains the default. Missing
  `USE_DETECTED_SOURCE_POINTS` therefore preserves all P218 XML behavior.

## Pipeline/XML Contract

An `AffineTransform` Step enables runtime Point binding with:

```xml
<Parameter>
  <Key>USE_DETECTED_SOURCE_POINTS</Key>
  <Value>true</Value>
</Parameter>
<Parameter>
  <Key>SOURCE_POINT_1_FEATURE</Key>
  <Value>LocateTopLeft/Center</Value>
</Parameter>
<Parameter>
  <Key>SOURCE_POINT_2_FEATURE</Key>
  <Value>LocateTopRight/Center</Value>
</Parameter>
<Parameter>
  <Key>SOURCE_POINT_3_FEATURE</Key>
  <Value>LocateBottomLeft/Center</Value>
</Parameter>
```

The three fixed destination coordinates remain the taught reference frame:

```xml
<Parameter><Key>DestinationPoint1X</Key><Value>60.5</Value></Parameter>
<Parameter><Key>DestinationPoint1Y</Key><Value>50.5</Value></Parameter>
<Parameter><Key>DestinationPoint2X</Key><Value>300.5</Value></Parameter>
<Parameter><Key>DestinationPoint2Y</Key><Value>50.5</Value></Parameter>
<Parameter><Key>DestinationPoint3X</Key><Value>60.5</Value></Parameter>
<Parameter><Key>DestinationPoint3Y</Key><Value>230.5</Value></Parameter>
```

The order is a correspondence contract: source 1 maps to destination 1, and so
on. OpenVisionLab does not guess, reorder, or select these correspondences.

## Declared Typed Point Outputs

| Producer tool | Available Point features |
|---|---|
| `Matching` / `TemplateMatching` | `Center` when exactly one usable result exists |
| `EdgeBasedMatching` aliases | `Center` when exactly one usable result exists |
| `Line` / `LineGauge` | `Start`, `End`, `Midpoint` |
| `CircleGauge` | `Center` |
| `GeometryMeasure` | `Intersection`, or `MeasureStart` / `MeasureEnd` where the selected mode produces them |

The selected-Step PropertyGrid lists only earlier enabled Point producers that
use the same input coordinate layer as the Affine Step.

## Fail-Closed Rules

Definition validation or the explicit Run fails when any of these is true:

- fewer than three references are supplied;
- a reference is not `StepName/FeatureName`;
- two references are the same;
- the source Step is missing, later, disabled, or ambiguous;
- the declared output is not a typed `Point`;
- the source Step failed execution or its acceptance gate;
- a configured producer did not publish exactly one selected Point this run;
- a Point is non-finite, outside the image, or belongs to a different
  input-layer name or image size;
- the resolved source triangle or taught destination triangle is degenerate or
  below its configured area gate;
- output dimensions, sampling, or valid-pixel coverage fail the existing P218
  OpenVisionLab Vision SDK gates.

No fixed-point fallback occurs after detected-point mode is enabled.

## Operator Workflow

1. Teach and gate three stable, physically distinct locator/geometry Steps on
   the same unannotated source layer.
2. Teach the corresponding three destination points in the reviewed reference
   coordinate system.
3. In Recipe Manager, load the Affine Step PropertyGrid, enable
   `Use detected Point features`, and select the three ordered Point features.
4. Keep the downstream inspection ROI fixed on the Affine output layer.
5. Run explicitly, then review all three locator drawings, the Affine
   destination triangle/frame, its matrix/coverage metrics, and the downstream
   ROI drawing/result.
6. Save the recipe only after those drawings prove the intended physical
   correspondence.

Property edits, selection, XML apply, output-layer creation, and visibility
changes do not invoke Preview or Run.

## Runtime Evidence

P219 executes this actual six-Step representative path:

```text
Matching x3 -> AffineTransform -> Threshold -> fixed-reference-ROI Blob
```

- all three Matching Steps publish their runtime `Center` Point;
- Affine publishes `AffineDetectedSourcePointCount=3` and the six resolved
  `AffineSourcePoint*X/Y` values;
- the recovered matrix equals the independently calculated three-point matrix;
- the saved downstream `CvROI=170,120,70,60` is unchanged;
- Blob accepts exactly one normalized target;
- duplicate source references are rejected by definition validation and again
  fail closed when execution is invoked directly.

Evidence:

- `artifacts\p219_dynamic_affine_fixture_20260723\runtime`
- `artifacts\p219_dynamic_affine_fixture_20260723\before`
- `artifacts\p219_dynamic_affine_fixture_20260723\after`

Focused commands:

```powershell
dotnet build "tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj" -c Debug -p:Platform="Any CPU"
dotnet "tools\VisionRecipeRunnerSmoke\bin\Any CPU\Debug\net8.0-windows7.0\VisionRecipeRunnerSmoke.dll" --affine-detected-points-contract "artifacts\p219_dynamic_affine_fixture_20260723\runtime"
dotnet build "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU"
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target p219_affine_point_binding_property_grid "artifacts\p219_dynamic_affine_fixture_20260723\after"
```

## Boundary

This is deterministic same-run correspondence wiring, not automatic
correspondence, one-shot feature selection, per-image ROI movement, homography,
perspective correction, lens calibration, certified metrology, unseen-data
robustness, or field qualification. Three good locators remain a
recipe/application responsibility. LLM development remains frozen.

Status: Complete

Scope: typed Point x3 to the existing OpenVisionLab Vision SDK Affine transform, followed by
one unchanged fixed reference-coordinate inspection ROI.

Acceptance criteria: XML/PropertyGrid persistence, actual Matching centers,
matrix equivalence, downstream fixed-ROI result, drawings, provenance metrics,
legacy fixed-mode preservation, and duplicate-reference fail-closed behavior
all pass.

Verification: P219 detected-point runtime/UI contracts, P218 fixed-source Affine
regression, P213 geometry review/PropertyGrid regressions, the readiness contract,
vendored-DLL and public-sample policy checks, JSON catalog parsing, `git diff
--check`, and the final current-source zero-warning solution build all passed.

Evidence: `artifacts\p219_dynamic_affine_fixture_20260723`.

Boundary / next dependency: qualification of a real inspection requires an
operator-selected physical three-point fixture, taught destination frame,
downstream ROI/tolerance, and representative samples.
