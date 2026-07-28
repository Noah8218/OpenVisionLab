# OpenVisionLab CVR-08 Multi-ROI Fixture Completion

Date: 2026-07-28 KST
Backlog item: `CVR-08`
Status: Complete

## Decision And Scope

The user explicitly delegated the bounded task choice with `알아서 해주세요`.
The selected task extends the existing public synthetic fixture workflow:

- one existing `Matching` producer publishes `PartFrame`;
- one existing `RotateScale/NormalizeImage` consumer creates the
  `572x420` reference-coordinate image;
- ROI A verifies the circular datum;
- ROI B verifies pad presence;
- Pipeline Review presents and selects both downstream ROI consumers.

This is a P212 review/workflow extension. It adds no fixture transform,
matching, segmentation, measurement, or acceptance algorithm.

## Frozen Inputs And Contract

| Item | Value |
| --- | --- |
| Good image | `docs/samples/public/Fixture_Pad_Synthetic_Shifted_OK.png` |
| Good SHA-256 | `31F3DDDDFA560EA3322C73380E8489C465EEF4D1DB2455435527A20233CA0138` |
| Bad image | `docs/samples/public/Fixture_Pad_Synthetic_Shifted_Missing_NG.png` |
| Bad SHA-256 | `A04142352E95FAA50251A5E2F617E0DA9333F28845AC96C2CE386FCA6348D2FC` |
| Template | `docs/samples/public/templates/Fixture_Locator_Synthetic_Template.png` |
| Template SHA-256 | `754D1D6DF4A933C81ED5B9DE41482DD3AF4B235B09E32BBD7E5942908D04ADC0` |
| Pipeline | `docs/samples/public/Public_Matching_NormalizeImage_RelativeRoi.pipeline.xml` |
| Pipeline SHA-256 | `C64F13A3B1C135F56598D0107CCC23ED891B26900E040EEE8B1CE419D6C85E39` |
| Reference pose | center `(120,100)`, angle `0`, scale `1` |
| Reviewed sample pose | center `(200,155)`, angle `0`, scale `1` |
| ROI A | circular datum, `210,240,55,55` |
| ROI A tool/gate | existing `Blob`, area `350..600`, `ResultCount=1` |
| ROI B | pad presence, `320,180,60,50` |
| ROI B tool/gate | existing `Blob`, area `700..1300`, `ResultCount=1` |

The Good image retains both objects. The controlled Bad image removes only the
pad, leaving the locator and circular datum unchanged.

## Implemented Behavior

- Fixture-chain resolution retains every enabled downstream Step that is
  reachable through declared layer routing and owns one valid `CvROI`.
- Each consumer row exposes Step number/name, ToolType, immutable
  reference-coordinate ROI, route, current-run state, and a deterministic
  SHA-256 evidence identity.
- The source preview draws every transformed ROI polygon; the normalized
  preview draws every unchanged reference rectangle.
- The selected row is visually distinguished and becomes the target of the
  existing `measurement ROI edit` handoff.
- Selecting the Fixture tab or a consumer row does not execute Preview/Run,
  change layer count or active layer, alter routes, or edit the recipe.
- Existing reference teach, producer edit, measurement edit, and explicit Run
  Review remain the only mutation/execution entry points.
- Existing user localization catalogs are migrated from the old single-ROI
  relationship/source/normalized formats to the multi-consumer formats.

## Good And Controlled Bad Replay

The focused current-build smoke executes both catalog rows before opening the
review UI:

- Good: Steps 1-5 are OK; datum `ResultCount=1`; pad `ResultCount=1`.
- Bad: Matching, NormalizeImage, Threshold, and datum remain OK; pad returns
  the expected `BlobNoResult` failure with `ResultCount=0`.
- Both use the same template, reference pose, transform settings, two ROIs, and
  gates.

Machine-readable replay evidence:
`artifacts/cvr08_multi_roi_fixture_20260728/final/fixture_multi_roi_sample_check.tsv`.

## UI Evidence

- Before current-source capture:
  `artifacts/cvr08_multi_roi_fixture_20260728/before/wpf_shell_host_workspace_sample_normalize_fixture_review.png`
- Final current-build capture:
  `artifacts/cvr08_multi_roi_fixture_20260728/final/wpf_shell_host_workspace_sample_normalize_fixture_review.png`

The final capture must show both consumer rows and the selected pad ROI in the
relationship and preview labels. The focused smoke also checks both 64-character
identities before and after row selection.

## Verification

Final commands and outcomes:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet build "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug
dotnet run --no-build --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_normalize_fixture_review artifacts\cvr08_multi_roi_fixture_20260728\final
dotnet run --no-build --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_workspace_sample_fixture_review artifacts\cvr08_multi_roi_fixture_20260728\regression
dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug
```

- Full solution build: PASS, 0 warnings / 0 errors.
- Screenshot-runner build: PASS, 0 warnings / 0 errors.
- Multi-ROI focused smoke: PASS, `check=OK`, `layout=0`, `text=0`,
  `internal=0`, `1500x880`.
- Legacy translation-only Fixture smoke: PASS, `check=OK`, `layout=0`,
  `text=0`, `internal=0`, `1500x880`.
- Readiness: PASS for all reported contract groups.

## Boundary

This proves one public synthetic translation-only workflow and its multi-ROI
review contract. It does not prove:

- angle/scale extremes, unseen data, lighting/focus/part variation, or
  production locator robustness;
- calibrated measurement, metrology accuracy, field qualification, or
  commercial parity;
- multiple fixture instances, homography, automatic locator selection,
  per-image ROI mutation, straight-edge fixture production, or sub-recipe
  fan-out.

## Durable Completion Record

```text
Status: Complete
Scope: Existing P212 Matching/NormalizeImage fixture review extended to two public synthetic downstream ROI consumers with stable identity, selection, drawings, edit handoff, and controlled Good/Bad replay.
Acceptance criteria: two physical ROI intents and frozen gates -> pass; every reachable consumer retained -> pass; stable identities and selected highlighting/edit target -> pass; Good passes both and Bad preserves datum while pad fails -> pass; no tab/row-selection Preview/Run, layer, active-layer, route, or recipe mutation -> pass.
Verification: full solution and screenshot-runner builds passed with 0 warnings/errors; focused multi-ROI and legacy Fixture WPF smokes passed; readiness passed.
Evidence: artifacts/cvr08_multi_roi_fixture_20260728 and docs/reports/OPENVISIONLAB_CVR08_MULTI_ROI_FIXTURE_20260728.md
Boundary / next dependency: CVR-08 does not qualify a production fixture. CVR-00 still requires three real novice participants; CVR-09 remains conditional on a named two-datum-edge task.
```
