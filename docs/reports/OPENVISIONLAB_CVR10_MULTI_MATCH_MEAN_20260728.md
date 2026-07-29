# CVR-10 MultiMatchMean v1 Implementation Report

Updated: 2026-07-28 KST

## Outcome

CVR-10 is complete for one bounded deterministic fan-out:

```text
Matching with four retained results
  -> stable row-major I01..I04
  -> per-result NormalizeImage
  -> one fixed reference-coordinate Mean ROI
  -> individual rows/drawings
  -> all-required or minimum-pass aggregate acceptance
```

The user's 2026-07-28 instruction to continue the next priority explicitly
activated this bounded row. The implementation does not introduce a generic
graph engine or arbitrary nested sub-recipe.

## Implemented Product Behavior

- Added canonical `MultiMatchMean` and alias `MultiFixtureMean`.
- Retained native multi-result Matching/EdgeBasedMatching evidence in the
  current run.
- Added deterministic row-major instance ordering and stable `I01..Ixx` IDs.
- Added instance-count, pairwise overlap, angle, scale, valid-pixel, and Mean
  fail-closed gates.
- Reused the existing `NormalizeImage` and `Mean` owners once per instance.
- Continued inspecting remaining instances after an individual rejection.
- Added all-required and minimum-pass aggregate modes with exact
  `InstanceAggregatePassed=1` Pipeline acceptance.
- Added PropertyGrid source/reference/ROI/gate editing and XML round trip.
- Added Pipeline Review `Instance Results`, row-to-drawing highlight, and
  current-run accepted/rejected ROI evidence.
- Added direct/recipe Run Report instance persistence and reload.

Property edits and review selection do not Preview/Run, create or select a
layer, or change routing.

## Fixed Runtime Matrix

The synthetic image contains four identical durable locator patterns in two
rows. Every locator has the same relative inspection pad. The bad case changes
only the third pad from Mean 225 to Mean 65.

| Case | Individual result | Aggregate | Pipeline |
| --- | --- | --- | --- |
| all good | 4 OK / 0 NG | all required -> pass | OK |
| one bad, require all | 3 OK / 1 NG | all required -> reject | NG |
| one bad, allow three | 3 OK / 1 NG | minimum 3 -> pass | OK |
| maximum count 3 | source count 4 | fail closed before fan-out | NG |
| overlapping fabricated same-frame source pair | IoU above 0.10 | fail closed before fan-out | NG |

All executed four-instance cases retained row-major `I01,I02,I03,I04`,
transformed ROI geometry, score, center, Mean, valid-pixel ratio, state, and
exact reject reason. The saved Run Report reloaded the same four ordered rows.

## Visual Review

The actual runtime NG drawing is:

- source:
  `artifacts\cvr10_multi_match_mean_20260728_r6\cases\one_bad_require_all_source.png`;
- result:
  `artifacts\cvr10_multi_match_mean_20260728_r6\cases\one_bad_require_all_result.png`.

I01, I02, and I04 retain green relative ROIs at Mean 225. I03 retains the red
relative ROI at Mean 65. The drawing therefore proves the numeric 3/4 result is
attached to the intended physical synthetic pad, rather than only to a report
row.

Current-source Pipeline Review after evidence:

- `artifacts\cvr10_multi_match_mean_20260728_r6\ui_selected_ng\cvr10_multi_match_mean_review.png`.

The view shows the two-Step branch, NG aggregate, four-instance summary,
selected rejected ROI highlight, and Instance Results table. This is a
current-source WPF view capture, not a claim of physical-part EXE
qualification.

A true before capture does not exist because neither the ToolType nor the
Instance Results tab existed before this implementation. The nearest historical
baseline is the completed CVR-09 Pipeline Review/PropertyGrid evidence; it is
not relabelled as a CVR-10 before image.

## Verification

Commands actually run:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet build tools\OpenVisionFixtureSmoke\OpenVisionFixtureSmoke.csproj -c Debug
dotnet run --no-build --project tools\OpenVisionFixtureSmoke\OpenVisionFixtureSmoke.csproj -c Debug -- --cvr10-multi-match-mean artifacts\cvr10_multi_match_mean_20260728_r6
dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target cvr10_multi_match_mean_review artifacts\cvr10_multi_match_mean_20260728_r6\ui_selected_ng
dotnet run --no-build --project tools\OpenVisionFixtureSmoke\OpenVisionFixtureSmoke.csproj -c Debug -- --cvr09-line-fixture artifacts\cvr10_regression_cvr09_line_fixture_20260728
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- C:\Git\OpenVisionLab_Dev
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1
```

Observed results:

- solution build: zero warnings, zero errors;
- runtime matrix: all five intended contracts passed;
- PropertyGrid round trip: passed;
- Pipeline XML round trip: passed;
- saved Run Report reload: four stable rows passed;
- current-source UI target: `check=OK`, `layout=0`, `text=0`,
  `internal=0`, `1400x1150`.
- CVR-09 LineFixture regression: 8/8 plus both fail-closed checks passed;
- readiness, external-reference, and public-sample contracts passed.

Evidence files:

- `artifacts\cvr10_multi_match_mean_20260728_r6\report.txt`;
- `artifacts\cvr10_multi_match_mean_20260728_r6\runtime_matrix.tsv`;
- `artifacts\cvr10_multi_match_mean_20260728_r6\saved_run_report.xml`;
- `artifacts\cvr10_multi_match_mean_20260728_r6\cases`;
- `artifacts\cvr10_multi_match_mean_20260728_r6\ui_selected_ng`.

## Limits

This evidence proves only a fixed four-instance synthetic translation matrix
with one Mean sub-inspection. It does not prove:

- arbitrary nested sub-recipes or general graph fan-out;
- cross-image physical identity tracking;
- rotation/scale production robustness;
- calibrated measurement;
- unseen-data or field robustness;
- performance or evidence usability at the 64-instance ceiling;
- another per-instance algorithm family.

CVR-14 remains conditional because the v1 overlap gate rejects source evidence;
it does not change the matcher's candidate suppression algorithm.

## Durable Completion Record

Status: Complete

Scope: CVR-10 bounded `MultiMatchMean` v1 implementation, current-run
multi-result wiring, individual/aggregate review, persistence, and synthetic
integration.

Acceptance criteria: stable ordering/IDs -> pass; count/overlap -> pass;
individual rows/drawings -> pass; partial/all aggregate semantics -> pass;
Run Report round trip -> pass; no automatic Preview/Run -> pass.

Verification: commands and observed results are recorded above.

Evidence: `artifacts\cvr10_multi_match_mean_20260728_r6`.

Boundary / next dependency: CVR-00 still requires three independent novice
participants. CVR-09 physical qualification and any additional CVR-10
per-instance family require named operator/data packets. CVR-11 remains
conditional on a labelled polarity-reversal blocker.
