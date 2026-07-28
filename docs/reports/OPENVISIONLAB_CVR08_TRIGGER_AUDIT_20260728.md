# OpenVisionLab CVR-08 Activation Audit

Date: 2026-07-28 KST
Backlog item: `CVR-08`
Status: Blocked

> Historical state notice: this document records the audit-time decision. The
> user subsequently delegated the bounded task choice on 2026-07-28, and CVR-08
> was completed with the public synthetic circular-datum plus pad-presence
> workflow. Current authority:
> `docs/reports/OPENVISIONLAB_CVR08_MULTI_ROI_FIXTURE_20260728.md`.

## Decision

Do not implement the generic typed fixture-transform consumer yet.

The current evidence does not contain one named operator inspection in which a
qualified locator must drive at least two downstream ROI/measurement Steps and
the existing P212 NormalizeImage or P219 Affine path cannot express the task
safely.

This is an activation-gate result, not a failure of the existing fixture
runtime.

## Current-Source Findings

### Existing NormalizeImage execution is already reusable

- `VisionPipelineFixtureFrameService` validates one named finite fixture frame
  and a full-image `NormalizeImage` consumer.
- `VisionPipelineNormalizeImageTool` produces a reference-sized output layer.
- Normal Pipeline routing can send that normalized layer, or layers derived
  from it, to multiple later Steps without rewriting any saved `CvROI`.
- Therefore another runtime transform engine is not justified merely to add a
  second fixed reference-coordinate ROI.

### P212 review is intentionally single-ROI

- `OpenVisionPipelineReviewFixturePresenter.TryResolveFixtureChain` walks the
  layers reachable after the selected NormalizeImage Step.
- It returns on the first enabled downstream Step with one valid `CvROI`.
- The state retains one `MeasurementIndex`, one source polygon, and one
  normalized reference rectangle.
- This is a real presentation limit, but no current operator task proves that
  the limit blocks recipe teaching or result review.

### Available retained evidence does not satisfy the combined trigger

1. The public
   `Public_Matching_NormalizeImage_RelativeRoi.pipeline.xml` contains one
   Matching producer, one NormalizeImage Step, and one downstream Blob
   `CvROI=320,180,60,50`.
2. A repository scan found no second tracked `.pipeline.xml` NormalizeImage
   workflow and no tracked NormalizeImage Pipeline with two downstream
   `CvROI` keys.
3. P235 preserves a qualified-with-limits hash-locked locator expected-success
   set:
   - Step SHA-256
     `7CEAEC5D50259ED1337AB912F0F0A63C673F4A74E692DCDEA01BAA14FC25658F`;
   - 24/24 locator-expected-success rows.
4. The exact P235 promoted Pipeline is one EdgeBasedMatching Step. It has no
   `USE_AS_FIXTURE_FRAME`, fixture-frame name, taught reference pose/image
   dimensions, NormalizeImage consumer, downstream ROI, or inspection
   acceptance contract.
5. P219 proves typed Point x3 to Affine wiring, but no current evidence names
   two downstream inspection ROIs that it cannot represent safely.

The project therefore has individual pieces of the trigger, but not the one
combined operator workflow required by CVR-08.

## Required Activation Packet

Provide one packet with all fields below before implementation:

```text
Task name:
Qualified locator Pipeline / Validation Set:
Reference image and exact source hash:
Allowed translation / angle / scale range:

ROI A:
  physical inspection intent:
  reference-coordinate rectangle:
  existing ToolType:
  required metrics and OK/NG gate:

ROI B:
  physical inspection intent:
  reference-coordinate rectangle:
  existing ToolType:
  required metrics and OK/NG gate:

Representative Good images:
Representative Bad images:
Why P212 NormalizeImage is insufficient:
Why P219 Affine is insufficient:
Required source/normalized drawings:
```

The two ROIs must describe real physical inspection regions. Two arbitrary
rectangles added only to exercise the UI do not activate the feature.

## Minimum Future Slice After Activation

If the packet satisfies the gate, prefer extending the existing P212 review
owner instead of adding another transform runtime:

1. retain every reachable downstream ROI consumer with stable Step identity;
2. show each immutable reference rectangle and transformed source polygon;
3. retain producer, transform, source layer, output layer, and consumer
   provenance per row;
4. use explicit selection/edit actions and existing explicit Run Review;
5. fail closed on ambiguous frame identity, cross-frame layers, invalid
   transforms, missing ROI, or duplicate consumer identity;
6. prove at least one Good and one controlled Bad replay using the same frozen
   locator, transform, ROIs, and gates.

Do not add multi-instance fan-out, homography, automatic locator selection,
per-image recipe mutation, camera calibration, or equipment integration.

## Verification Performed

- Rebuilt current priority from `AGENTS.md`, the current handoff, stable
  contracts, product target, and the canonical CVR backlog.
- Inspected current `VisionPipelineFixtureFrameService`,
  `VisionPipelineNormalizeImageTool`, and
  `OpenVisionPipelineReviewFixturePresenter`.
- Inspected the exact public NormalizeImage Pipeline.
- Scanned tracked `.pipeline.xml` files for NormalizeImage and downstream
  `CvROI` occurrence.
- Inspected the exact P235 promoted Pipeline and completion record.

No product code, Pipeline, sample, locator configuration, ROI, gate, layer, or
route was changed by this audit.

## Durable Completion Record

```text
Status: Blocked
Scope: CVR-08 activation audit only; no fixture runtime or UI implementation.
Acceptance criteria: current producer/consumer path identified -> pass; multiple-ROI evidence searched -> pass; qualified-locator compatibility checked -> pass; named two-ROI operator task found -> fail.
Verification: current-source and durable-evidence inspection listed above; readiness and git diff checks are recorded in the handoff close-out.
Evidence: docs/reports/OPENVISIONLAB_CVR08_TRIGGER_AUDIT_20260728.md
Boundary / next dependency: One completed activation packet naming a qualified locator, two physical downstream inspection ROIs, existing tools/metrics/gates, representative Good/Bad evidence, and the concrete P212/P219 insufficiency.
```
