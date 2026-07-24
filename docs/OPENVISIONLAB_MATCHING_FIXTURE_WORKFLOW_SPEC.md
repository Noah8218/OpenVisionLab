# OpenVisionLab Matching Fixture Workflow Spec

Status: V1 translation-only consumer runtime and public Good/Bad operator workflow implemented on 2026-07-14. P179 added Matching pose/scale Pipeline/XML round trip and evidence metrics on 2026-07-21. P181 later implemented the separate V2 `NormalizeImage` consumer; this document remains the V1 translation contract.

> For angle/scale compensation, use the implemented and bounded V2 contract in `OPENVISIONLAB_MATCHING_SIMILARITY_FIXTURE_V2_SPEC.md` and the `Matching Fixture NormalizeImage` section of `OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md`. V1 below still means X/Y translation of one downstream ROI; it must not be described as rotation/scale compensation.

## Purpose

OpenVisionLab needs one verified fixture workflow before it grows a general coordinate-frame abstraction. The first workflow uses one `Matching` result to move one downstream `CvROI` when the same part translates in the source image.

This is an offline recipe-workbench feature. It does not add camera setup, motion control, PLC/I/O, deployment, or an automatic inspection loop.

## Existing Source Contract

- `Lib.OpenCV.Result.MatchingResult` exposes `Center`, `Angle`, `Scale`, `Bounding`, and `Score`.
- `VisionToolResult` exposes metrics and overlays. Matching rectangle overlays carry bounds, center, and angle. Because the external overlay contract has no scale field, the app derives uniform scale from runtime bounds versus the loaded template and snaps it to the configured Matching scale-search grid.
- `VisionPipelineStep` has no fixture property, but its parameter dictionary preserves explicit extension parameters in XML.
- Pipeline ROI values are image-coordinate `Rect` values. Before this change, every step executed the saved `CvROI` directly.
- `VisionRecipeRunner` already records overlay centers and angles in step summaries.

The v1 implementation therefore extends the app-owned pipeline execution layer. It does not modify the external `Lib.OpenCV.dll` contract.

## V1 Operator Workflow

1. Load a representative reference image.
2. Configure a `Matching` step with `NUM_MATCH=1` and run Preview explicitly.
3. Review the match score, center, angle, and scale.
4. Mark the Matching step as fixture frame `PartFrame` and store the reviewed reference X, Y, angle, and scale.
5. Configure one downstream ROI inspection step on the same source layer.
6. Opt that step into `PartFrame` and keep `ALLOW_BRANCH_INPUT=true` because it intentionally reads the original source layer.
7. Run Preview or Run explicitly.
8. Review `FixtureOffsetX`, `FixtureOffsetY`, `FixtureAngleDelta`, `FixtureScale`, `FixtureScaleRatio`, and the effective ROI metrics together with the inspection result.

Creating or selecting a fixture frame must not run a tool, create a layer, or change input routing.

## V1 Parameters

Matching producer:

| Parameter | Required | Meaning |
| --- | --- | --- |
| `USE_AS_FIXTURE_FRAME` | Yes | Publishes the successful Matching pose as a frame. |
| `FIXTURE_FRAME_NAME` | Yes | Case-insensitive frame identifier. |
| `FIXTURE_REFERENCE_X` | Yes | Reviewed match center X on the reference image. |
| `FIXTURE_REFERENCE_Y` | Yes | Reviewed match center Y on the reference image. |
| `FIXTURE_REFERENCE_ANGLE` | Yes | Reviewed match angle on the reference image. |
| `FIXTURE_REFERENCE_SCALE` | Yes | Positive reviewed uniform scale on the reference image. Normally `1` for the original template size. |
| `FIXTURE_MAX_ANGLE_DELTA` | No | Maximum angle change accepted by translation-only v1. Default is 2 degrees. |

Downstream consumer:

| Parameter | Required | Meaning |
| --- | --- | --- |
| `USE_FIXTURE_FRAME` | Yes | Applies a previously published frame. |
| `FIXTURE_FRAME_NAME` | Yes | Earlier frame to consume. |
| `USE_ROI` | Yes | Must be true. |
| `CvROI` | Yes | Saved reference-image ROI. This value is never rewritten during execution. |
| `ALLOW_BRANCH_INPUT` | Recommended | Must be true for the intentional return to the fixture source layer. |

V1 supports one axis-aligned `CvROI`. It rejects multi-ROI and masking rather than applying a partial transform.

## XML Shape

```xml
<Step>
  <Name>01 Locate Part</Name>
  <ToolType>Matching</ToolType>
  <Enabled>true</Enabled>
  <InputLayer>Main</InputLayer>
  <OutputLayer>FixtureMatch</OutputLayer>
  <Parameters>
    <Parameter><Key>TemplatePath</Key><Value>templates\part_mark.png</Value></Parameter>
    <Parameter><Key>NUM_MATCH</Key><Value>1</Value></Parameter>
    <Parameter><Key>USE_AS_FIXTURE_FRAME</Key><Value>true</Value></Parameter>
    <Parameter><Key>FIXTURE_FRAME_NAME</Key><Value>PartFrame</Value></Parameter>
    <Parameter><Key>FIXTURE_REFERENCE_X</Key><Value>60</Value></Parameter>
    <Parameter><Key>FIXTURE_REFERENCE_Y</Key><Value>70</Value></Parameter>
    <Parameter><Key>FIXTURE_REFERENCE_ANGLE</Key><Value>0</Value></Parameter>
    <Parameter><Key>FIXTURE_REFERENCE_SCALE</Key><Value>1</Value></Parameter>
    <Parameter><Key>FIXTURE_MAX_ANGLE_DELTA</Key><Value>1</Value></Parameter>
  </Parameters>
</Step>
<Step>
  <Name>02 Inspect Part ROI</Name>
  <ToolType>Blob</ToolType>
  <Enabled>true</Enabled>
  <InputLayer>Main</InputLayer>
  <OutputLayer>PartBlob</OutputLayer>
  <Parameters>
    <Parameter><Key>USE_ROI</Key><Value>true</Value></Parameter>
    <Parameter><Key>CvROI</Key><Value>170,80,50,50</Value></Parameter>
    <Parameter><Key>ALLOW_BRANCH_INPUT</Key><Value>true</Value></Parameter>
    <Parameter><Key>USE_FIXTURE_FRAME</Key><Value>true</Value></Parameter>
    <Parameter><Key>FIXTURE_FRAME_NAME</Key><Value>PartFrame</Value></Parameter>
  </Parameters>
</Step>
```

## Runtime Contract

For the single Matching result:

```text
offsetX = currentCenterX - referenceCenterX
offsetY = currentCenterY - referenceCenterY
angleDelta = normalize(currentAngle - referenceAngle)
scaleRatio = currentScale / referenceScale

effectiveRoi.X = savedRoi.X + round(offsetX)
effectiveRoi.Y = savedRoi.Y + round(offsetY)
effectiveRoi.Width = savedRoi.Width
effectiveRoi.Height = savedRoi.Height
```

The executor clones the step for the current run, replaces only the cloned `CvROI`, and sends that clone to the existing tool factory. The original pipeline step and serialized XML remain unchanged.

The producer and consumer must read the same source layer in v1. This avoids pretending that geometry remains valid through an arbitrary preprocessing or resampling layer.

## Metrics

- `FixtureCenterX`
- `FixtureCenterY`
- `FixtureAngle`
- `FixtureScale`
- `FixtureOffsetX`
- `FixtureOffsetY`
- `FixtureAngleDelta`
- `FixtureScaleRatio`
- `FixtureEffectiveRoiX`
- `FixtureEffectiveRoiY`

These are execution evidence. They do not change the normal tool acceptance metric unless the recipe explicitly selects one.

## Failure Rules

The pipeline stops with a structured step failure when:

- the producer is not `Matching`;
- `NUM_MATCH` is not exactly 1;
- the frame name or numeric center/angle reference pose is missing, or reference scale is not positive;
- the Matching result has no usable rectangle pose overlay;
- enabled scale search cannot resolve a positive uniform scale from the template and runtime overlay bounds;
- a frame name is published twice;
- a consumer references a missing or later frame;
- producer and consumer source layers differ;
- the angle delta exceeds the configured translation-only limit;
- the consumer has no valid `CvROI`;
- the consumer enables multi-ROI or masking;
- the translated ROI falls outside the input image.

No failure path changes input routing, saved ROI values, or Preview/Run behavior.

## PropertyGrid UI Contract

Recipe Manager exposes only the parameters already proven by the runtime. The first consumer is `Blob`; broader consumer coverage must follow a real sample need rather than adding the fields to every tool speculatively.

```text
Matching step
  Fixture output
    [ ] Publish fixture frame
    Frame name              PartFrame
    Reference center X      60
    Reference center Y      70
    Reference angle         0 deg
    Reference scale         1
    Max angle delta         1 deg

Blob/Contour/Line step
  Fixture input
    [ ] Use fixture frame
    Frame name              PartFrame
    Saved ROI               170,80,50,50
```

Pipeline Review exposes `Save as reference` only after an explicit successful Review produces one valid Matching fixture pose. The command copies `FixtureCenterX`, `FixtureCenterY`, `FixtureAngle`, and `FixtureScale` into the four reference fields, saves the active pipeline, invalidates the old review result, and requires another explicit Review. It does not launch Preview/Run itself.

The PropertyGrid mapper preserves all producer/consumer keys through load and XML apply-back. Disabling the fixture option removes its fixture keys instead of adding disabled boilerplate to unrelated recipes. Loading, searching, or applying these fields does not trigger Preview/Run or rewrite `CvROI`, `InputLayer`, or `OutputLayer`.

## Proof Evidence

`tools/OpenVisionFixtureSmoke` generates a reference image and a second image translated by `(70,40)` pixels.

Required assertions:

- reference image with fixture: OK;
- translated image without fixture: downstream Blob NG;
- translated image with fixture: OK;
- effective ROI changes from `(170,80)` to `(240,120)`;
- saved `CvROI` remains `170,80,50,50` after both runs.

Current artifact folder:

`artifacts/fixture_translation_smoke_current`

P179 pose/scale round-trip and drawing evidence:

- `artifacts/p179_matching_pose_scale_pipeline_roundtrip_20260721`;
- current relative-template XML snapshots and reports preserve `USE_FIND_SCALE`, its range/step, `FIXTURE_REFERENCE_SCALE`, and the published `FixtureScale`/`FixtureScaleRatio` metrics;
- representative current-run overlays show the same approved P178 object at scale/angle pairs `0.90/+3 deg`, `0.95/-2.5 deg`, and `1.15/+2.5 deg`;
- this evidence does not change the v1 consumer equation: only X/Y translation is applied.

Recipe Manager PropertyGrid evidence:

- target: `wpf_shell_host_recipe_fixture_properties`;
- before: `artifacts/fixture_property_grid_roundtrip_20260714/before`;
- after: `artifacts/fixture_property_grid_roundtrip_20260714/after`;
- assertions: producer/consumer descriptor values, XML parameter round trip, unchanged ROI/routing, and unchanged Preview/Run count.

Public operator-workflow evidence:

- catalog pair: `Public_Fixture_Pad_Good` / `Public_Fixture_Pad_Missing_Bad`;
- pipeline: `docs/samples/public/Public_Matching_FixturePad.pipeline.xml`;
- translated Good: Matching center `(200,155)`, offset `(80,55)`, effective ROI `(400,235)`, Blob result 1, final OK;
- translated Bad: the same Matching pose succeeds, then Blob fails with `BlobNoResult` because the pad is missing;
- translated Good without Fixture: Blob fails against the unchanged saved ROI `320,180,60,50`;
- public catalog gate: 30/30 runnable rows OK under `artifacts/public_fixture_sample_20260714/catalog_current`;
- current-source before/after Pipeline Review: `artifacts/public_fixture_sample_20260714/before_fixture_metrics` and `artifacts/public_fixture_sample_20260714/after_final`;
- latest EXE evidence: `artifacts/public_fixture_sample_20260714/latest_exe/public_fixture_pipeline_review_current_exe.png` and `report.txt`.

Pipeline Review displays the selected consumer's runtime proof as `Fixture Delta X,Y | ROI X,Y`. This is read-only evidence; it does not teach a pose, rewrite the saved ROI, or run Preview automatically.

Reference-pose teach evidence:

- target: `wpf_shell_host_workspace_sample_fixture_teach`;
- before/after: `artifacts/fixture_pose_teach_20260714/before` and `artifacts/fixture_pose_teach_20260714/after`;
- explicit copy: reference `(120,100,0)` becomes reviewed pose `(200,155,0)`;
- unchanged: all consumer parameters, `CvROI`, step routes, workspace layer selection/count, and native Preview count;
- post-save: prior review evidence is cleared and the operator is told to run Review again;
- latest Debug EXE regression smoke: `artifacts/fixture_pose_teach_20260714/direct_exe/report.txt` reports `Result: PASS`.

## Deliberately Deferred

- rotation compensation of the image or ROI geometry;
- scale compensation;
- multi-ROI and mask transformation;
- fixture chaining;
- EdgeBasedMatching and FeatureMatching producers;
- a generic affine coordinate-frame graph;
- automatic teach, Preview, or Run.

Rotation support should align an image/layer or transform the real tool geometry. Expanding an axis-aligned rectangle around rotated corners would inspect extra pixels and is not an acceptable metrology shortcut.

## Next Evidence Gate

1. Use the actual chosen reference image in the latest EXE, explicitly run Review, save the reviewed Matching pose, and run Review again.
2. Confirm whether the operator understands that consumer ROIs remain unchanged and may need explicit reteaching when the reference image changes.
3. Add an ROI-reteaching guide or command only if that real workflow exposes confusion. It must remain explicit and must not silently rewrite consumer ROIs.
4. Rotation, scale, multi-ROI, masks, chaining, and a generic frame graph remain deferred until a real failing sample proves the need.
