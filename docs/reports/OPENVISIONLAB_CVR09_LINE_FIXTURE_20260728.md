# CVR-09 LineFixture v1 Implementation Report

Updated: 2026-07-28 KST

## Outcome

The bounded `LineFixture` v1 implementation is complete. Two earlier accepted
`LineGauge/Segment` results now publish one gated pixel fixture that the
existing `NormalizeImage` and relative-ROI workflow can consume.

This development was explicitly activated by the user's 2026-07-28
continuation after the previous handoff had kept CVR-09 conditional. The user
accepted a bounded synthetic implementation slice. That decision authorizes
the reusable contract and integration evidence; it does not replace the
separate real-part qualification prerequisite.

## Implemented Scope

- Added canonical `LineFixture` and alias `DualEdgeFixture`.
- Reused exact typed `Segment` outputs from two earlier accepted Line Steps.
- Added support, fit-residual, included-angle, intersection-extension, source
  identity, coordinate-frame, reference-pose, and in-image gates.
- Published the intersection as fixture origin and typed `Origin/Point`.
- Converted image-coordinate Line angle into the existing positive
  counter-clockwise Fixture convention.
- Connected the producer to the existing Fixture frame service and
  `NormalizeImage` consumer without adding another transform engine.
- Added known metrics, validator/schema/normalizer support, XML persistence,
  selected-Step PropertyGrid editing, typed Segment dropdowns, and
  Line-specific Pipeline Review quality text.
- Preserved explicit Run and zero PropertyGrid Preview/Run, layer, and route
  side effects.

The complete reusable contract is:

- `docs/contracts/openvisionlab/OPENVISIONLAB_LINE_FIXTURE_V1_CONTRACT.md`.

## Actual Runtime Matrix

The frozen generated part contains a reviewed dark rectangular outer datum,
one bright downstream pad, and multiple internal horizontal/vertical rails
that could act as distractors outside the taught Line search bands.

| Case | Origin | Angle | Included | Support A/B | Residual A/B px | Valid pixels | Fixed ROI mean |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: |
| reference | 89,69 | 0 | 90 | 50/37 | 0/0 | 0.995 | 192.2 |
| shift right/down | 113,81 | 0 | 90 | 50/37 | 0/0 | 0.923 | 192.2 |
| shift left/down | 71,84 | 0 | 90 | 50/36 | 0/0 | 0.923 | 192.2 |
| shift right/up | 101,55 | 0 | 90 | 50/36 | 0/0 | 0.936 | 192.2 |
| rotate +3 deg | 95.410,75.704 | 2.919 | 89.995 | 50/37 | 0.941/1.331 | 0.961 | 188.2 |
| rotate -3 deg | 81.788,77.280 | -3.081 | 89.898 | 50/36 | 1.098/1.013 | 0.930 | 188.2 |
| rails +2 deg | 105.086,63.150 | 2.109 | 89.818 | 50/37 | 0.977/1.062 | 0.928 | 188.1 |
| rails -2 deg | 73.316,59.006 | -2.109 | 89.594 | 50/36 | 1.344/0.980 | 0.941 | 185.4 |

Result: `8/8` complete Pipeline runs. A valid definition whose maximum
included angle was below the measured physical pair rejected at runtime with
the exact reason. Duplicate Datum A/B typed identity rejected during
definition validation.

## Defect Found During Verification

The first rotation replay exposed a real coordinate-convention defect:
`LineGauge` Segment angle uses image coordinates with Y increasing downward,
while Fixture/`NormalizeImage` uses OpenCV's positive counter-clockwise angle.
Passing the Segment angle through unchanged rotated the normalized image in the
wrong direction and moved the fixed ROI off the bright pad.

`LineFixture` now converts the sign at the ownership boundary and draws its
axes in the matching screen direction. The same frozen `+/-3 deg` cases then
returned the fixed ROI to the pad.

## UI Evidence

The selected-Step PropertyGrid exposes the two exact earlier Line Segments and
the frame/gate contract. Apply/save/reload preserved source identities and
triggered no Preview/Run.

Fresh current-source capture:

- `artifacts/cvr09_line_fixture_20260728_r11/ui/cvr09_line_fixture_property_grid.png`.

Current-run algorithm drawings:

- `artifacts/cvr09_line_fixture_20260728_r11/cases/reference_fixture_overlay.png`;
- `artifacts/cvr09_line_fixture_20260728_r11/cases/rotate_positive_fixture_overlay.png`;
- equivalent Datum A, Datum B, and Fixture overlays for all eight cases.

A true before screenshot was not captured before implementation began. The
closest reproducible baseline is the completed CVR-08/P212 Matching fixture
designer evidence in `artifacts/cvr08_multi_roi_fixture_20260728`. It has a
Matching producer and no dual-Line producer editor. It is labelled as the
baseline, not presented as a true before capture.

## Verification

Commands actually run:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet build "tools\OpenVisionFixtureSmoke\OpenVisionFixtureSmoke.csproj" -c Debug
dotnet run --no-build --project "tools\OpenVisionFixtureSmoke\OpenVisionFixtureSmoke.csproj" -c Debug -- --cvr09-line-fixture "C:\Git\OpenVisionLab_Dev\artifacts\cvr09_line_fixture_20260728_r11"
dotnet build "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug
dotnet run --no-build --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target cvr09_line_fixture_property_grid "C:\Git\OpenVisionLab_Dev\artifacts\cvr09_line_fixture_20260728_r11\ui"
git diff --check
```

Observed results:

- solution build: zero warnings, zero errors;
- focused fixture build: zero warnings, zero errors;
- runtime: 8/8 pass;
- included-angle fail-closed: pass;
- duplicate-source fail-closed: pass;
- Pipeline XML round trip: pass;
- PropertyGrid round trip: pass;
- UI target: `check=OK`, `layout=0`, `text=0`, `internal=0`,
  `1600x900`.

## Boundaries

This result proves a reusable synthetic pixel-space two-datum integration.
It does not prove a named physical datum, polarity/lighting variation, scale,
perspective, calibration, certified metrology, unseen-data robustness,
production variation, or field qualification.

Do not mark physical-task CVR-09 qualification complete until the operator
packet and reviewed N-sample evidence in the contract exist.

## Completion Record

```text
Status: Complete
Scope: Bounded LineFixture v1 implementation and synthetic integration only.
Acceptance criteria: Existing Line Segments -> gated fixture -> existing NormalizeImage -> fixed reference ROI; 8/8 translation/rotation/distractor replay; exact fail-closed reasons; PropertyGrid/XML persistence; no automatic Preview/Run/layer/route changes.
Verification: Commands and observed results above.
Evidence: artifacts/cvr09_line_fixture_20260728_r11
Boundary / next dependency: Real-part CVR-09 qualification is blocked on a named part, certified datum identities, representative images, pose/polarity limits, downstream intent, and reviewed N-sample evidence.
```
