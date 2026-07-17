# OpenVisionLab Commercial Vision UI Reference Review

Date: 2026-07-15

## Purpose And Boundary

This review compares public, official UI evidence from commercial machine-vision authoring products with OpenVisionLab's current workflow.

OpenVisionLab remains an OpenCvSharp4 rule-based vision recipe workbench. The useful comparison target is authoring and review clarity: tool selection, ordered execution, selected-step parameters, image evidence, metrics, and timing. Camera setup, lighting, PLC/I/O, controller deployment, accounts, and production HMI design remain outside the product scope.

The captured images are research evidence only. They must not be copied into OpenVisionLab public samples, Learn content, release assets, or product UI.

## Official Capture Inventory

| Product | Official source | Local review capture | What the capture shows |
| --- | --- | --- | --- |
| MVTec MERLIC Creator | <https://www.mvtec.com/doc/merlic/5.7/manual/en-us/Content/Creator/User_interface_creator/user_interface_creator.html> | `artifacts/commercial_vision_ui_reference_20260715/01_MVTec_MERLIC_Creator_Official.png` | Tool Flow, Tool Library, selected Tool Workspace, image, parameters/results, status |
| Cognex VisionPro QuickBuild | <https://docs.cognex.com/vpromx_1000/web/en/visionpro/Content/Topics/users-guide/quickbuild/adding-vision-tools.htm> | `artifacts/commercial_vision_ui_reference_20260715/02_Cognex_VisionPro_QuickBuild_Official.png` | ToolBlock inputs/outputs and explicit tool-to-tool data links |
| Zebra Aurora Vision Studio | <https://docs.adaptive-vision.com/current/studio/getting_started/MainWindowOverview.html> | `artifacts/commercial_vision_ui_reference_20260715/03_Zebra_Aurora_Vision_Studio_Official.png` | Toolbox, program editor, properties, image previews, hints, results, execution status/time, complexity level |
| NI Vision Builder for Automated Inspection | <https://knowledge.ni.com/KnowledgeArticleDetails?id=kA00Z0000019O9nSAE&l=en-US> | `artifacts/commercial_vision_ui_reference_20260715/04_NI_Vision_Builder_AI_Official.png` | Inspection sequence and task-grouped step palette |
| KEYENCE XG VisionEditor | <https://www.keyence.com/support/user/xg/video/vision-editor.jsp> | `artifacts/commercial_vision_ui_reference_20260715/05_KEYENCE_XG_X_VisionEditor_Official.jpg` | Flowchart, selected unit, error/check list, image window, unit property and result areas |

## Product-Level Findings

### MVTec MERLIC Creator

Useful pattern:

- The complete tool flow remains visible while the selected tool gets a larger working area.
- Connections, image evidence, parameters/results, and the most recent processing time are associated with the selected tool.

OpenVisionLab implication:

- Preserve Pipeline as the ordered inspection-flow owner and keep the selected Step visually anchored while reviewing its image, PropertyGrid parameters, metrics, and timing.
- Do not move Pipeline responsibilities back into Recipe Manager.

Do not copy:

- MERLIC runtime, frontend, image-acquisition, communication, and deployment surfaces.

### Cognex VisionPro QuickBuild

Useful pattern:

- ToolBlock makes input image, tool outputs, fixture transforms, and downstream dependencies explicit.
- The compact tree is useful for tracing data without requiring every property to be visible at once.

OpenVisionLab implication:

- Keep layer routing and selected-Step producer/consumer evidence readable.
- Extend branch/output comparison only when a real recipe exceeds the existing selected-Step map; do not replace the ordered Pipeline with an unrestricted node graph.

Do not copy:

- Automatic/electric execution behavior, acquisition jobs, scripting surfaces, or application-host integration.

### Zebra Aurora Vision Studio

Useful pattern:

- Toolbox, program, properties, image previews, hints, numerical results, execution status, and time are reachable from one coherent workspace.
- Complexity level and Minimal/Compact/Full display modes provide progressive disclosure instead of showing every option to every user.

OpenVisionLab implication:

- The existing Recipe Manager summary plus explicit Advanced review follows the right direction; do not add another mode switch.
- Pipeline Review should be the selected-Step workspace where image/output, parameters, result meaning, and timing converge.
- Contextual Learn or parameter hints are more valuable than adding another long guide panel.

Do not copy:

- Hardware acquisition catalogs, arbitrary HMI composition, or continuous auto-execution.

### NI Vision Builder AI

Useful pattern:

- The current inspection sequence and the palette of additional steps are separated clearly.
- Tools are grouped by operator task rather than exposed as one undifferentiated list.

OpenVisionLab implication:

- Keep the collapsed Tool rail actionable through icons and retain task-oriented categories.
- A Step insertion action should preserve the operator's current Pipeline context and remain explicit.

Do not copy:

- Inspection-interface/HMI editing, LabVIEW integration, device acquisition, or deployment workflow.

### KEYENCE XG VisionEditor

Useful pattern:

- Flowchart, selected unit properties, image, results, and an error/check list share the same working layout.
- The selected unit is highlighted in the flow, which reduces ambiguity about which parameters and results are being viewed.

OpenVisionLab implication:

- Keep selected-Step identity and status synchronized across Step Flow, PropertyGrid, viewer/output comparison, and result text.
- Surface validation/preflight issues near the Pipeline flow only when they identify a concrete Step and correction action.

Do not copy:

- Controller workspaces, transfer, SD-card/program management, user-interface builder, communications, or equipment settings.

## Cross-Product Pattern

The common commercial pattern is not a large feature count. It is one stable authoring loop:

1. Choose or select a processing Step.
2. See where its input comes from and where its output goes.
3. Edit parameters in a predictable property surface.
4. Run explicitly and inspect image, metrics, status, and elapsed time together.
5. Correct the failed Step without losing pipeline context.

OpenVisionLab already has most of the structural pieces: ordered Pipeline, PropertyGrid tools, layer routing, explicit Preview/Run, selected-Step review, Good/Bad evidence, branch/output comparison, and novice summary versus Advanced review. The remaining value is integration and evidence quality, not another top-level workspace.

## Recommended OpenVisionLab Priority

### P1. Per-Step Bottleneck Evidence In Existing Run History - Completed

The sample-suite path saves and links a structured Step report through `RunReportPath`. The existing Run History now provides a compact Step timing aggregate with:

- Step index and name;
- sample coverage;
- average, p95, and maximum elapsed time;
- incompatible or missing-report reason instead of partial numbers.

The implementation is read-only and rejects incomplete or incompatible linked-report coverage instead of mixing partial statistics. It adds no telemetry service, database, background execution, or automatic run.

### P2. Selected-Step Workspace Coherence Audit - Completed

The real three-Step `Public_Matching_FixturePad` review now verifies one selected Step identity, Blob tool, branch route, input/output evidence, Fixture parameters, result metrics, and elapsed time. Duplicate Step ordinals were removed and the existing summary card was widened; Recipe Manager remains library/summary and Pipeline remains sequence/execution owner.

### P3. Contextual Learn Link

When a selected tool or parameter has a Learn topic, offer one compact contextual entry point. Opening Learn must not change parameters, create layers, alter routing, or run Preview/Run. Apply-to-tool remains a separate explicit action.

### Deferred Until Evidence Exists

- Pipeline search, grouping, or minimap: only after a real long recipe proves navigation friction.
- Broader branch/output graph: only after a real recipe exceeds current producer/consumer coverage.
- Additional Recipe Manager panels: only after current-build evidence proves a missing recipe-library responsibility.

## Review Checklist For Future Commercial References

- Is the source an official product page, manual, support article, or training page?
- Does the capture show the actual authoring/review UI rather than marketing artwork?
- Which operator task becomes clearer?
- Does OpenVisionLab already implement the pattern?
- Can the idea fit PropertyGrid tools and explicit Preview/Run?
- Does it avoid camera, lighting, PLC/I/O, account, deployment, and HMI scope?
- Is a real OpenVisionLab workflow available to prove the gap before implementation?
