# Edge Detection Shape Count LLM Baseline Review

Updated: 2026-07-16 KST

## Decision

`EdgeDetection(Canny) -> Morphology(Close) -> Contour` completed one bounded manual GPT XML-authoring round. The manually transferred first XML is a constrained direct success; no correction loop was needed or manufactured.

## Verified Contract

- Step 1 reads `Main`, uses Canny with `40/120` thresholds, aperture `3`, and `UseL2Gradient=true`, then creates `EdgeDetection_Edge`.
- Step 2 reads `EdgeDetection_Edge`, uses `Morphology(Close)` with a `3x3` Rect kernel and one iteration, then creates `EdgeDetection_EdgeJoin`.
- Step 3 reads `EdgeDetection_EdgeJoin`, restricts Contour to `CvROI=90,100,410,95`, and counts only external joined contours with area `500..5000`.
- Acceptance uses `ResultCount=4..4` on Step 3. The route is sequential: `Main -> EdgeDetection_Edge -> EdgeDetection_EdgeJoin -> EdgeDetection_Shape_Preview`.
- There is no branch input, external template/image dependency, or output layer that overwrites an input layer.

## Current-Build Evidence

1. The existing public baseline `docs/samples/public/Public_EdgeDetection_Shapes.pipeline.xml` validated/imported successfully with three Steps, zero errors, zero warnings, and no external dependencies. `ImageRun: SKIPPED` confirms import did not execute the inspection.
2. Explicit latest-EXE nominal execution passed at `ResultCount=4` in `38.89 ms`; channel transitions were `3 -> 1 -> 1`.
3. Explicit latest-EXE missing-shape execution produced the expected product NG at `ResultCount=2 < 4` in `31.084 ms`.
4. The mechanically derived packet reference XML validated/imported with the same three-Step route, then passed nominal `ResultCount=4` in `29.477 ms` and produced expected missing-shape NG `ResultCount=2 < 4` in `26.241 ms`.
5. Both Good/NG commands used `--expect-run-success` and returned exit code 0 because the expected inspection result was declared explicitly.

The full solution build passed with zero warnings and zero errors before these actual-EXE replays. Current evidence is under `artifacts/p66_edge_detection_baseline_20260716`; the reference XML SHA-256 is `9BD5F06FED2E701616FB08F6C1246968278F7E0C8D2C116FE9A4DD8F82A0D01C`.

## Manual GPT Packet

`docs/evidence/llm/prompt-packets/edge_detection_shapes` contains:

- `README.md`
- `COPY_THIS_TO_GPT.txt`
- `PASTE_VALIDATION_NG_BACK_TO_GPT.txt`
- `EdgeDetection_Shapes_Synthetic_OK.png`
- `EdgeDetection_Shapes_Synthetic_Missing_NG.png`

The two packet images are byte-identical copies of registered public synthetic assets:

- `EdgeDetection_Shapes_Synthetic_OK.png`: `7CC641CC7D560DD5C392FDF1D14BFACDEF76EDD864DFEE975E049A705DDEEF3A`.
- `EdgeDetection_Shapes_Synthetic_Missing_NG.png`: `ACFAEC265F44F99E04F462103940FD2673630F08665337FB52CE8D148D6FC40B`.

The user sends both PNGs and the complete `COPY_THIS_TO_GPT.txt` content in one new GPT conversation. The first response must be preserved unchanged. Use the correction template only after a real current-build validation or nominal/negative run failure.

## Actual GPT Direct-Success Evidence

- The user supplied a first XML response through the P66 GPT packet workflow. Exact model/version, API evidence, and a full provider-chat export were not supplied and remain unknown.
- The raw XML and its copied packet prompt are preserved under `artifacts/llm_transcripts/raw/20260716_edge_detection_shapes_gpt_round1`. The raw manifest records provenance limits, immutable hashes, report hashes, and result-image hashes; it is not a public `docs/evidence` package.
- XML-only boundary verification passed. The user-supplied XML matched the mechanically verified P66 reference across 53 structured fields with zero differences.
- After a fresh zero-warning/zero-error solution build, latest `bin/Debug/OpenVisionLab.exe` validated and imported the response with three Steps, zero errors, zero warnings, no external dependencies, and `ImageRun: SKIPPED`.
- Explicit nominal execution passed at `ResultCount=4` in `34.661 ms`. Explicit Missing-NG execution produced the expected product NG at `ResultCount=2 < 4` in `33.613 ms`. Each smoke returned exit code 0 only through its declared expected outcome.
- This is one constrained direct-success result with zero correction rounds. Do not send the correction template or fabricate a failed correction round.
- P68's ignored six-file sanitized candidate was approved for Dev-worktree inclusion and replayed as P69. The public package is `docs/evidence/llm/20260716_edge_detection_shapes_gpt_direct_success`; its companion decision record is `docs/OPENVISIONLAB_LLM_EDGE_DETECTION_PUBLICATION_REVIEW_20260716.md`. The raw candidate remains ignored and continues to exclude raw reports and local evidence locations.

## Limits

- This is a tightly constrained three-Step authoring task, not a demonstration of independent inspection-design capability.
- The public synthetic pair does not prove robustness against lighting variation, blur, occlusion, or real production variation.
- EdgeDetection is a preprocessing stage here. The final quality decision is the downstream Contour count, not `EdgePointCount` alone.
- No auto-run, layer mutation, tool-parameter UI change, equipment integration, or product-scope expansion is part of this packet.
