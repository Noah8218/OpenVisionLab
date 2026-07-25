# Dark-band Gap Measurement Intent Skill

Updated: 2026-07-22
Status: Phase 1 contract frozen; P189 small-split evidence complete; P190 all-500 audit closed as `Keep with documented limits`, not full Phase 2 semantic completion.

## Operator intent

Measure the vertical pixel thickness between the upper and lower edges of one long dark band inside one operator-reviewed coarse ROI.

This skill does not locate an object. It does not use Matching, a locator, template teaching, `NormalizeImage`, Blob, or Contour. The operator owns the physical target and ROI; OpenVisionLab owns deterministic edge-pair selection and evidence.

## Required input

| Input | Rule |
|---|---|
| Coarse ROI | Exactly one `x,y,w,h` rectangle containing the intended long dark band. |
| Measurement | Upper-to-lower edge distance. |
| Units | Pixel only. `PIXELPERMM=0`. |
| Tolerance | Not supplied in v1. The starter is measurement-only. |

## Locked tool family and starter

- Exactly one enabled `LineDistance` Step.
- `InputLayer=Main`; output must be a distinct layer.
- `USE_ROI=true` and `USE_GAP_EDGE_PAIR=true`.
- The upper edge is a supported near-horizontal candidate. The lower edge must be fitted from the nearest sustained bright transition after the dark core below that upper edge; an arbitrary farther Hough edge is not an eligible lower boundary.
- No acceptance fields may be enabled until the operator supplies tolerance evidence.
- P187 starter parameters remain fixed during the initial 500-image replay.

```xml
<VisionPipeline>
  <Name>Dark_Band_Gap_Measurement</Name>
  <Steps>
    <Step>
      <Name>Detect Dark Band Gap Edges</Name>
      <ToolType>LineDistance</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Main</InputLayer>
      <OutputLayer>GapMeasured</OutputLayer>
      <UseAcceptance>false</UseAcceptance>
      <ExpectedSuccess>true</ExpectedSuccess>
      <Parameters>
        <Parameter><Key>Name</Key><Value>DarkBandGapEdgePair</Value></Parameter>
        <Parameter><Key>PIXELPERMM</Key><Value>0</Value></Parameter>
        <Parameter><Key>USE_ROI</Key><Value>true</Value></Parameter>
        <Parameter><Key>CvROI</Key><Value>100,80,530,230</Value></Parameter>
        <Parameter><Key>USE_GAP_EDGE_PAIR</Key><Value>true</Value></Parameter>
        <Parameter><Key>CANNY_LOW</Key><Value>10</Value></Parameter>
        <Parameter><Key>CANNY_HIGH</Key><Value>45</Value></Parameter>
        <Parameter><Key>GAP_MIN_PX</Key><Value>12</Value></Parameter>
        <Parameter><Key>GAP_MAX_PX</Key><Value>60</Value></Parameter>
        <Parameter><Key>GAP_MAX_ANGLE_DEG</Key><Value>8</Value></Parameter>
        <Parameter><Key>GAP_MAX_PARALLEL_DELTA_DEG</Key><Value>4</Value></Parameter>
        <Parameter><Key>GAP_MIN_SUPPORT_RATIO</Key><Value>0.26</Value></Parameter>
        <Parameter><Key>GAP_MIN_DARK_CONTRAST</Key><Value>8</Value></Parameter>
        <Parameter><Key>GAP_MIN_DARK_COVERAGE_RATIO</Key><Value>0.25</Value></Parameter>
        <Parameter><Key>GAP_MIN_SCORE_MARGIN</Key><Value>0.05</Value></Parameter>
      </Parameters>
    </Step>
  </Steps>
</VisionPipeline>
```

The example ROI is the reviewed P187 top-right coarse ROI. Another inspection requires the operator to replace it with one reviewed ROI; an LLM must not invent coordinates.

## Required runtime evidence

The inspection is not reviewable from a PASS count alone. Every run must retain:

- green coarse ROI;
- thin yellow candidate lines;
- thick blue selected upper edge;
- thick magenta selected lower edge;
- five red Gap sample lines;
- PASS/REJECT text and pixel measurement or named rejection reason;
- source image/hash, XML identity, and result image/hash.

Required metrics:

- measurement: `DistancePxAvg`, `DistancePxMin`, `DistancePxMax`, `DistancePxRange`;
- stages: `GapCandidateLineCount`, `GapOverlapPairCount`, `GapSeparationPairCount`, `GapParallelPairCount`, `GapContrastPairCount`;
- selection: `GapSelectedAngleDeltaDeg`, `GapSelectedSupportRatio`, `GapDarkContrast`, `GapDarkCoverageRatio`, `GapBandMeanGray`, `GapScoreMargin`.

## Failure table

| Failure | Meaning | Operator action |
|---|---|---|
| No candidate lines | No supported near-horizontal edges in the ROI. | Review ROI and Canny evidence. |
| No nearest lower boundary | The upper edge has no sufficiently supported first sustained bright transition after its dark core. | Review whether the ROI contains one visible dark band; do not connect a farther structure. |
| No overlap/separation pair | The traced lower boundary does not share enough length or expected thickness with the upper edge. | Review physical target and expected thickness range. |
| No parallel pair | Edge angle difference is too large. | Review whether the selected region contains one band. |
| No contrast pair | The between-edge region is not sufficiently dark. | Review polarity, illumination, and ROI. |
| Ambiguous pair | A second distinct pair is too competitive. | Narrow the operator ROI; do not guess. |

## Three-phase completion gate

1. Starter XML: one reviewed ROI creates valid/importable measurement-only XML and rejects Matching, changed ROI, or an unapproved acceptance gate.
2. N-sample evidence: unchanged XML runs on the frozen corpus and retains drawings, metrics, error rows, and representative/extreme contact sheets.
3. Correction loop: a genuine failed LLM draft is corrected using working evidence, frozen, then replayed on previously unused held-out data.

Large-corpus execution follows `OPENVISIONLAB_SCALABLE_SKILL_VALIDATION_PROTOCOL.md`: execute every row, select the review queue deterministically from all failures, stage/measurement extremes, declared strata, and a hash-seeded random audit sample, then open every queued current-run drawing. Do not ask the operator to review or tune all images. Permit at most two bounded correction cycles before closing this skill as `Keep`, `Keep with documented limits`, `Hybrid candidate`, or `Reject`.

P187 is the historical first implementation, but its canonical drawing was invalidated when the selected magenta line connected a farther lower structure. P189 corrects that defect without changing the starter XML and supplies the current canonical plus ten-row Phase 2 pilot evidence under `artifacts\p189_gap_lower_edge_correction_20260722`. It does not supply all-500 robustness, tolerance truth, calibration, other directions, or Phase 3 correction evidence.

P190 executes the unchanged starter on all 500 `device_top_right` rows and reviews a frozen deterministic 128-row drawing queue. The baseline measured 448 and failed closed on 52, but several successful rows selected a lower secondary structure instead of the intended upper band. One bounded support-ratio correction produced 329 measurements and 171 fail-closed outcomes but retained the same wrong-pass class on long secondary structures. Therefore the skill remains usable only when the operator ROI contains exactly one complete intended long band and no competing long band. Do not claim general raw-coordinate robustness, and do not continue numeric tuning without a changed operating assumption.

## Review checklist

- [ ] Exactly one operator-reviewed coarse ROI.
- [ ] Exactly one enabled `LineDistance` Step.
- [ ] `USE_GAP_EDGE_PAIR=true`, `PIXELPERMM=0`.
- [ ] No Matching/locator/normalization/template dependency.
- [ ] No product acceptance gate without operator tolerance.
- [ ] The magenta lower line follows the nearest sustained lower boundary of the same dark core, not a farther edge.
- [ ] Candidate and selected-edge drawings retained.
- [ ] Stage, support, dark-coverage, and ambiguity metrics retained.
- [ ] Same XML used across the declared validation split.
- [ ] Full corpus accounted for while human review is limited to the frozen deterministic review queue.
- [ ] Correction count is 0..2 and no image-specific ROI or parameter tuning occurred.
- [ ] Final `Keep` / limited / hybrid / reject decision and honest evidence boundary recorded.
