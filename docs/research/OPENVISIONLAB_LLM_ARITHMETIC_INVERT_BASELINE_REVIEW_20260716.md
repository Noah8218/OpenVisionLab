# Arithmetic Invert LLM Baseline Review

Updated: 2026-07-16 KST

## Decision

`Arithmetic(Bitwise_NOT) -> Mean` completed one bounded, user-identified GPT XML-authoring round. The manually transferred first XML is a constrained direct success; no correction loop was needed or manufactured.

## Verified Contract

- Step 1 reads `Main`, uses `ArithmeticMode=Operation`, `ArithmeticOperation=Bitwise_NOT`, and `UseConstantInput=false`, then creates `Arithmetic_Invert_Result`.
- `Bitwise_NOT` is the supported unary Arithmetic operation for this task; it must not declare `InputLayerB`.
- Step 2 reads `Arithmetic_Invert_Result`, uses full-image `MEAN_TYPES=Mean`, keeps its internal threshold/adaptive/invert/ROI/multi-ROI options disabled, and creates `Arithmetic_Invert_Mean`.
- Acceptance uses `MeanValueAvg` from `190` through `230` on Step 2.
- The route is sequential: `Main -> Arithmetic_Invert_Result -> Arithmetic_Invert_Mean`. There is no branch input and no output layer overwrites an input layer.

## Current-Build Evidence

1. The existing public baseline pipeline `docs/samples/public/Public_Arithmetic_Invert.pipeline.xml` validated/imported successfully with two Steps, zero errors, zero warnings, and no external dependencies. `ImageRun: SKIPPED` confirms import did not execute the inspection.
2. Explicit latest-EXE nominal execution passed at `MeanValueAvg=208` within the `190..230` gate in `7.424 ms`.
3. Explicit latest-EXE bright-input execution produced the expected product NG at `MeanValueAvg=76.7 < 190` in `6.811 ms`.
4. Both image commands used `--expect-run-success` and returned exit code 0 because the expected inspection result was declared explicitly.

The full solution build passed with zero warnings and zero errors before these actual-EXE replays. Current evidence is under `artifacts/p62_arithmetic_invert_baseline_20260716`.

## Manual GPT Packet

`docs/evidence/llm/prompt-packets/arithmetic_invert` contains:

- `README.md`
- `COPY_THIS_TO_GPT.txt`
- `PASTE_VALIDATION_NG_BACK_TO_GPT.txt`
- `Arithmetic_Invert_Synthetic_OK.png`
- `Arithmetic_Invert_Synthetic_Bright_NG.png`

The two packet images are byte-identical copies of registered public synthetic assets:

- `Arithmetic_Invert_Synthetic_OK.png`: `D53BC22EB96EC87CAAC4CA46A7F52DB9B4FC04AD3E6C561B759557DCB126207A`.
- `Arithmetic_Invert_Synthetic_Bright_NG.png`: `72514BA2524E0998676ECE85A18D6A6D7C91900E4FA3B1A951F62293725C086E`.

The user sends both PNGs and the complete `COPY_THIS_TO_GPT.txt` content in one new GPT conversation. The first response must be preserved unchanged. Use the correction template only after a real current-build validation or nominal/negative run failure.

## Actual GPT Direct-Success Evidence

- The user supplied a first XML response identified as GPT through a manual paste. Exact model/version, API evidence, and a full provider-chat export were not supplied and remain unknown.
- The raw XML and its copied packet prompt are preserved under `artifacts/llm_transcripts/raw/20260716_arithmetic_invert_gpt_round1`. The raw manifest records provenance limits, immutable hashes, report hashes, and result-image hashes; it is not a public `docs/evidence` package.
- XML-only boundary verification passed. The user-supplied XML matched the mechanically verified P62 reference across 34 structured fields with zero differences.
- After a fresh zero-warning/zero-error solution build, latest `bin/Debug/OpenVisionLab.exe` validated and imported the response with two Steps, zero errors, zero warnings, no external dependencies, and `ImageRun: SKIPPED`.
- Explicit nominal execution passed at `MeanValueAvg=208` in `16.243 ms`. Explicit Bright-NG execution produced the expected product NG at `MeanValueAvg=76.7 < 190` in `10.07 ms`. Each smoke returned exit code 0 only through its declared expected outcome.
- This is one constrained direct-success result with zero correction rounds. Do not send the correction template or fabricate a failed correction round.
- The ignored P64 candidate excluded raw reports and local evidence locations. After explicit user approval, its audited six-file public counterpart was added to `docs/evidence/llm/20260716_arithmetic_invert_gpt_direct_success`; the tracked-path replay is recorded in `docs/OPENVISIONLAB_LLM_ARITHMETIC_INVERT_PUBLICATION_REVIEW_20260716.md`.

## Limits

- This is a tightly constrained two-Step authoring task, not a demonstration of independent inspection-design capability.
- The public synthetic pair does not prove robustness against lighting variation, blur, occlusion, or real production variation.
- No auto-run, layer mutation, tool-parameter UI change, equipment integration, or product-scope expansion is part of this packet.
