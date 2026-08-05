# Edge-Based Unique Match V1 Contract

Status: P224 implementation complete; P225 first real-image fixed candidate rejected

## Purpose

The existing OpenVisionLab Vision SDK edge matcher can be configured to fail closed when
more than one spatially distinct plausible match remains. This prevents an
external `NUM_MATCH=1` request from hiding a repeated-pattern ambiguity.

This is an optional runtime acceptance contract. It does not change legacy XML
and it does not qualify a template for production.

## Parameters

```xml
<Parameter><Key>NUM_MATCH</Key><Value>1</Value></Parameter>
<Parameter><Key>USE_MULTI_ROI</Key><Value>false</Value></Parameter>
<Parameter><Key>USE_UNIQUE_MATCH_VALIDATION</Key><Value>true</Value></Parameter>
<Parameter><Key>UNIQUE_MATCH_MIN_SCORE_MARGIN</Key><Value>0.03</Value></Parameter>
```

- `USE_UNIQUE_MATCH_VALIDATION` defaults to `false` when absent.
- `UNIQUE_MATCH_MIN_SCORE_MARGIN` defaults to `0.03` and must be finite in
  `0..1`.
- Enabled mode requires external `NUM_MATCH=1` and one search region
  (`USE_MULTI_ROI=false`).
- Internal candidate retention is at least Top 8 and is independent of the
  external one-result contract.

## Runtime Decision

The selected candidate must first meet the existing `SCORE_MIN`. An eligible
alternative is another candidate that also meets `SCORE_MIN` and whose center is
at least `max(8 px, 0.35 * min(template width, template height))` from the
selected center. It becomes plausible when the selected-minus-alternative score
margin is below `UNIQUE_MATCH_MIN_SCORE_MARGIN`.

- `NoMatch`: no selected candidate meets the existing score gate. No
  `MatchingResult` is returned; error code is `MatchingNoResult`.
- `Success`: no plausible alternative exists. Exactly one `MatchingResult` is
  returned. A weaker eligible alternative may still be reported through the
  strongest-alternative score and passing margin.
- `Ambiguous`: at least one plausible alternative exists. No `MatchingResult` is
  returned; error code is
  `MatchingAmbiguous`.

When hybrid verification is enabled, the unique decision uses the finite hybrid
selection score. Otherwise it uses the existing edge score. The ambiguity reason
records best score, strongest alternative score, actual/required margin,
plausible-alternative count, and the matching options.

## Evidence Contract

`VisionToolResult.Metrics` publishes normalized values:

- `UniqueMatch.Enabled`
- `UniqueMatch.State` (`0 Disabled`, `1 NoMatch`, `2 Success`, `3 Ambiguous`)
- `UniqueMatch.MinimumInternalTopK`
- `UniqueMatch.PlausibleAlternativeCount`
- `UniqueMatch.SelectedScore`
- `UniqueMatch.StrongestAlternativeScore`
- `UniqueMatch.ScoreMargin`
- `UniqueMatch.MinimumScoreMargin`
- `UniqueMatch.DistanceThresholdPx`

A successful `MatchingResult` also retains `EdgeScore`, optional `ImageScore`,
`FinalScore`, and `ScoreMargin`. Result scores and its margin use the existing
percentage-point presentation; pipeline metrics remain normalized `0..1`.
Legacy-disabled results keep `ScoreMargin=NaN` rather than inventing uniqueness
evidence.

OpenVisionLab exposes the two parameters in the existing Edge Based Matching
PropertyGrid and XML/Pipeline mapper. Edits do not auto-run Preview. Recipe
validation fails closed for invalid one-result/one-search-region combinations,
and Pipeline diagnostics preserve the exact OpenVisionLab Vision SDK ambiguity reason.

## Bounded Evidence

The current synthetic matrix contains four cases:

1. legacy repeated pattern: succeeds with one legacy result and uniqueness
   disabled;
2. distinct pattern with unique mode: succeeds with one result;
3. repeated pattern with unique mode: fails `MatchingAmbiguous`, returns zero
   results, and records the failed margin;
4. absent pattern: fails `MatchingNoResult`, returns zero results, and records
   `NoMatch`.

Historical predecessor-library evidence:
`artifacts\p224_unique_match_runtime_20260724\library_noah`

The predecessor-library Release output, the former OpenVisionLab vendored copy, and that historical Debug
output are assembly `2.1.0.0`, file `2.8.0.0`, SHA-256
`000C75A7D0E796E166DF6F24C95F264FC001927881B1ED7DE7BAE31913099F6D`.

OpenVisionLab current-source UI evidence:

- before:
  `artifacts\p224_unique_match_runtime_20260724\before\wpf_shell_host_edge_based_matching_auto_mpoint.png`
- after:
  `artifacts\p224_unique_match_runtime_20260724\after_current\wpf_shell_host_edge_based_matching_auto_mpoint.png`

## P225 Fixed-ROI Evidence

The exact P220/P221 card `R` anchor was replayed on 12 hash-frozen rows with a
reviewed search ROI, angle `-8..8°`, scale `0.9..1.1`, score `0.45`, unique
margin `0.03`, and prior-center error gate `<=5 px`. Unique mode returned
`0/12` correct accepts: two baseline mismatches, two ambiguity rejects, and
eight no-match rejects. On the original broad ROI, both legacy and unique modes
retained two wrong accepted centers.

Drawing review confirmed a high-score/high-margin selection on `T` rather than
the intended `R`. This proves the intended boundary: uniqueness can reject a
competing candidate, but cannot establish semantic or physical-feature
identity. The candidate decision is `Reject`; no gate or ROI was tuned after
observing results.

The OpenVisionLab Pipeline path now also preserves existing EdgeBased
scale/refinement fields and exports `Center` for exactly one successful result,
so a future qualified candidate can feed Affine/relative-ROI consumers.

Evidence: `artifacts\p225_edge_unique_card_r_matrix_20260724`.

## Boundary

This contract proves fail-closed state separation, backward-compatible XML,
PropertyGrid exposure, and bounded synthetic runtime behavior. It does not prove
that the template or search ROI is physically correct, that the `0.03` default is
appropriate for a production line, or that pose accuracy, false-accept rate,
repeatability, lighting variation, and field robustness meet an operator
requirement.

Do not add joint least-squares refinement, adaptive pattern sizing, ODB/CAD,
Homography, multi-anchor grouping, or a larger image campaign from this result.
P225 has supplied and rejected one approved anchor/ROI matrix. A second matrix is
blocked until the operator reviews and approves a different Auto MPoint
suggestion as one durable physical feature across the representative images.
