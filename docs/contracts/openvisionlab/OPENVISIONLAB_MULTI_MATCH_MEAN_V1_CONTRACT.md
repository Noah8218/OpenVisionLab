# OpenVisionLab MultiMatchMean v1 Contract

Updated: 2026-07-28 KST

## Status And Scope

`MultiMatchMean` is the bounded CVR-10 multi-instance consumer. It takes the
retained results of one earlier accepted multi-result `Matching` or
`EdgeBasedMatching` Step, assigns deterministic instance identities, normalizes
each pose into one taught reference frame, and runs the same fixed reference
ROI `Mean` inspection for every instance.

This is not a generic loop, nested Pipeline, arbitrary sub-recipe graph, or
automatic feature-selection engine. Adding another per-instance inspection
family requires its own named operator task and contract.

Canonical ToolType:

- `MultiMatchMean`

Compatibility alias:

- `MultiFixtureMean`

## Operator Workflow

```text
teach one durable matching template and request NUM_MATCH >= 2
  -> Run Matching explicitly
  -> configure one reference pose and one fixed reference-coordinate Mean ROI
  -> configure count, overlap, pose, Mean, and aggregate gates
  -> Run Review explicitly
  -> inspect I01..Ixx rows and their source-image ROI drawings
  -> save recipe and Run History evidence
```

Property edits, table selection, and drawing selection do not invoke Preview or
Run and do not change layers or routes.

## Source Contract

The source must be:

- one exact earlier enabled Step name;
- `Matching`, `TemplateMatching`, `EdgeBasedMatching`,
  `EdgeBasedTemplateMatching`, or `EdgeTemplateMatching`;
- successful and accepted in the same explicit run;
- configured with `NUM_MATCH >= 2`;
- executed on the same input coordinate layer and image dimensions as the
  consumer;
- composed only of finite results with a positive bounding width, height, and
  scale.

Missing, later, duplicate-name, failed, rejected, wrong-family, cross-frame,
empty, or invalid sources fail closed.

## Stable Instance Identity

v1 assigns `I01`, `I02`, and so on after row-major ordering:

1. matches whose centers are within `ROW_TOLERANCE_PX` belong to one row;
2. rows are ordered top to bottom;
3. matches in each row are ordered left to right.

The stable ID is a same-run geometric review identity. It is not a serialized
physical serial number and must not be used as cross-image part tracking.

`MIN_INSTANCES`, `MAX_INSTANCES`, and `MAX_OVERLAP_RATIO` fail closed before
fan-out. Overlap uses pairwise axis-aligned source match bounding-box IoU.
`MAX_INSTANCES` is capped at 64 to bound execution and evidence size.

## Per-Instance Inspection

For each ordered result, runtime:

1. calculates angle delta and scale ratio from the taught reference pose;
2. applies the existing `NormalizeImage` Fixture transform to the original
   consumer input;
3. runs the existing `Mean` tool once with `USE_ROI=true` at `RELATIVE_ROI`;
4. records score, source center/angle/scale, transformed source-image ROI,
   normalized valid-pixel ratio, Mean, accepted state, and exact reject reason;
5. continues with the remaining instances after an individual failure.

The fixed sub-inspection uses:

- `MIN_MEAN`;
- `MAX_MEAN`;
- `MAX_ANGLE_DELTA`;
- `MIN_SCALE_RATIO`;
- `MAX_SCALE_RATIO`;
- `MIN_VALID_PIXEL_RATIO`.

Every current-run source image retains a green accepted or red rejected
transformed ROI with its stable ID, state, and finite Mean value.

## Aggregate Acceptance

Per-instance results are combined by one explicit mode:

- `REQUIRE_ALL=true`: every retained instance must pass;
- `REQUIRE_ALL=false`: at least `MIN_PASS_COUNT` instances must pass.

The tool publishes `InstanceAggregatePassed` as `1` or `0`. A valid Pipeline
definition must enable acceptance on this metric with exact minimum and maximum
`1`. The tool can therefore retain all individual rows and drawings while the
Pipeline reports NG through deterministic acceptance.

Published metrics:

- `InstanceCount`;
- `InstancePassCount`;
- `InstanceFailCount`;
- `InstanceAggregatePassed`;
- `InstanceMeanMin`, `InstanceMeanMax`, `InstanceMeanAvg`;
- `InstanceScoreMin`, `InstanceScoreMax`;
- `InstanceValidPixelRatioMin`;
- the existing image and ROI bounds metrics where applicable.

## PropertyGrid, Review, And Persistence

- Selected-Step PropertyGrid exposes the exact earlier compatible source,
  reference pose/image size, reference ROI, count/ordering/overlap gates,
  Mean/pose gates, and aggregate mode.
- Apply/save/reload preserves all parameters and the exact aggregate acceptance
  gate without Preview/Run.
- Pipeline Review exposes an `Instance Results` table. Selecting a row
  highlights the same transformed ROI without rerunning the Pipeline.
- direct and recipe Run Reports persist all stable rows and their reject
  reasons. XML reload must preserve row order and IDs.

## Compatibility And Boundaries

- Existing Matching, EdgeBasedMatching, Fixture, NormalizeImage, Mean, report,
  and object-result behavior is unchanged when `MultiMatchMean` is absent.
- Missing v1 keys use the documented PropertyGrid/runtime defaults, but source,
  reference pose, and `RELATIVE_ROI` remain required for a meaningful run.
- v1 supports one fixed `Mean` sub-inspection only.
- v1 does not prove rotation/scale robustness, calibrated measurement,
  arbitrary nested tools, cross-image tracking, production variation,
  unseen-data robustness, performance at 64 instances, or field qualification.

## Verification Authority

Focused commands:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --no-build --project tools\OpenVisionFixtureSmoke\OpenVisionFixtureSmoke.csproj -c Debug -- --cvr10-multi-match-mean artifacts\cvr10_multi_match_mean_20260728_r6
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target cvr10_multi_match_mean_review artifacts\cvr10_multi_match_mean_20260728_r6\ui
```

Evidence:

- `artifacts\cvr10_multi_match_mean_20260728_r6`;
- `docs\reports\OPENVISIONLAB_CVR10_MULTI_MATCH_MEAN_20260728.md`.

## Durable Completion Record

Status: Complete

Scope: Bounded Matching multi-result to per-instance NormalizeImage and fixed
reference-ROI Mean fan-out, stable row-major IDs, individual/aggregate review,
PropertyGrid/XML, and saved Run Report persistence.

Acceptance criteria: stable ordering/IDs passed; count and overlap fail-closed
gates passed; per-instance results/drawings passed; all-required and
minimum-pass aggregate semantics passed; report round trip passed; no automatic
Preview/Run path was added.

Verification: Debug solution build passed with zero warnings/errors; focused
runtime matrix and current-source Pipeline Review capture passed.

Evidence: `artifacts\cvr10_multi_match_mean_20260728_r6`.

Boundary / next dependency: another per-instance tool family or physical
qualification requires a separately named operator task, representative data,
and explicit acceptance/error limits.
