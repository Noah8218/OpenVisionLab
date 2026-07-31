# Morphology Cleanup LLM Baseline Review

Updated: 2026-07-16 KST

## Decision

`Threshold -> Morphology -> Contour` is ready for one bounded manual GPT XML-authoring round. This is prompt and baseline readiness only; no external GPT response or correction loop exists yet.

## Verified Contract

- Step 1 creates `Morphology_Binary` from `Main` with `Threshold=130`, `MaxValue=255`, and `ThresholdType=Binary`.
- Step 2 reads that output and uses `Morphology` `Open` with a `Rect` 5x5 kernel and one iteration to create `Morphology_Clean`.
- Step 3 reads `Morphology_Clean`, disables its internal threshold/ROI/draw-image options, and counts external `ApproxSimple` contours from 20 through 5000 pixels.
- Acceptance uses `ResultCount` exactly `4` through `4` on Step 3.
- The route is sequential: `Main -> Morphology_Binary -> Morphology_Clean -> Morphology_Cleanup_Preview`. There is no branch input and no output layer overwrites an input layer.

## Current-Build Evidence

1. The existing public baseline pipeline `docs/samples/public/Public_Morphology_Cleanup.pipeline.xml` validated/imported successfully with three Steps, zero errors, zero warnings, and no external dependencies.
2. Explicit latest-EXE nominal execution passed at `ResultCount=4`, with Step channel transitions `3 -> 1 -> 1`, four overlays, and `60.533 ms` total elapsed time.
3. Explicit latest-EXE missing-target execution produced the expected product NG at `ResultCount=2 < 4`, with the same Step channel transitions, two overlays, and `39.122 ms` total elapsed time.
4. Both baseline commands used `--expect-run-success` and returned exit code 0 because the expected inspection result was declared explicitly.

The latest full solution build passed with zero warnings and zero errors before these replays. The latest-EXE evidence is under `artifacts/p53_morphology_baseline_20260716`.

## Manual GPT Packet

`docs/evidence/llm/prompt-packets/morphology_cleanup` contains:

- `README.md`
- `COPY_THIS_TO_GPT.txt`
- `PASTE_VALIDATION_NG_BACK_TO_GPT.txt`
- `Morphology_Cleanup_Synthetic_OK.png`
- `Morphology_Cleanup_Synthetic_Missing_NG.png`

The two packet images are byte-identical copies of registered public synthetic assets:

- `Morphology_Cleanup_Synthetic_OK.png`: `58CA44380245210C9A4131606D419F44779D2AB3E55582DFB85CE3B1D3FAEB32`.
- `Morphology_Cleanup_Synthetic_Missing_NG.png`: `B5C47606E7A3CD27070A7E1223606354CFC628AE2EB9555574CFB694D5FC663C`.

The user sends the two PNGs and the complete `COPY_THIS_TO_GPT.txt` content in one new GPT conversation. The first response must be preserved unchanged. Use the correction template only after a real current-build validation or nominal/negative run failure.

## Limits

- This is a tightly constrained three-Step authoring task, not a demonstration of independent inspection-design capability.
- The public synthetic pair does not prove robustness against lighting variation, blur, occlusion, or real production variation.
- No auto-run, layer mutation, tool-parameter UI change, equipment integration, or product-scope expansion is part of this packet.
