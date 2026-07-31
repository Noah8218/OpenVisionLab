# OpenVisionLab RotateScale Geometry GPT Packet Baseline Review

Review date: 2026-07-16 KST

## Goal

Prove one public-safe geometry contract before requesting external XML: resize the full source image to 50 percent with `RotateScale`, then accept the nominal source only when the output width is exactly 286 pixels.

This is a geometry-output-size check. It does not claim object detection, physical calibration, template registration, or broad rotation/scale robustness.

## Verified Public Contract

Reference XML: `docs/samples/public/Public_Geometry_RotateScale.pipeline.xml`

| Field | Locked value |
| --- | --- |
| Tool sequence | one `RotateScale` Step |
| Route | `Main -> Geometry_ResizeHalf_Result` |
| Angle | `0` |
| ScaleXPercent / ScaleYPercent | `50` / `50` |
| Interpolation / BorderType | `Linear` / `Constant` |
| Acceptance metric | `ResultImageWidth` |
| Acceptance range | `286..286` |
| Max elapsed | `300` ms |

Public inputs:

| Role | Path | SHA-256 |
| --- | --- | --- |
| Nominal | `docs/samples/public/Geometry_RotateScale_Synthetic_OK.png` | `B6850A6110D15D86BFD5214F04C1FF1FD99193C8A74C04062355F1872E037F2D` |
| Wide negative | `docs/samples/public/Geometry_RotateScale_Synthetic_Wide_NG.png` | `40A28B9D5767C6975E5AF88C13F518540DF15A686FA2D16259D6FAE61AA47CFD` |

The public reference XML SHA-256 is `BB4C685981FF35F213ABE0D06A483E290B5EADC38ACC2D67315EA088A6D3C2F8`.

## Current-Build Baseline

- Build: PASS, 0 warnings, 0 errors.
- EXE: `bin/Debug/OpenVisionLab.exe`.
- EXE SHA-256: `53E9223D5343DEA9EC16E997ED32C14F0EE56E5878C6246E62BFA88DCD0B0F85`.
- Evidence: `artifacts/p70_rotate_scale_baseline_20260716`.
- Validation/import: PASS, 1 Step, 0 errors, 0 warnings, no external dependencies, and `ImageRun: SKIPPED`.
- Nominal: PASS; source 572x420 -> result 286x210; `ResultImageWidth=286`; 3.428 ms.
- Wide negative: expected product NG; source 640x420 -> result 320x210; `ResultImageWidth=320 > 286`; 3.18 ms.

The negative smoke returns exit code 0 only because `--expect-run-success false` declares the expected product rejection. Validation/import and image execution remain separate explicit actions.

## Validation-Review Clarity Fix

The first P70 import was valid but displayed a misleading generic warning because the LLM review treated only parameter-key patterns as judgement evidence. A valid `UseAcceptance` metric/range is also an explicit judgement criterion.

`OpenVisionShellHostRecipeCommandSurface.HasJudgementParameter` now recognizes an enabled acceptance metric with a configured minimum or maximum range. The final current-EXE import reports `Inspection.Evidence: OK - explicit judgement criteria are present.` The change does not alter XML execution, Preview/Run, layer creation, active layer selection, or input/output routing.

## Packet

`docs/evidence/llm/prompt-packets/rotate_scale_geometry` contains the self-contained prompt, correction template, and byte-identical public Good/Wide-NG images. Its response contract is deliberately constrained to the verified one-Step XML so that a returned first response can be structurally compared to the public reference and replayed without manual cleanup.

## Limits

- No real GPT/Gemini/Claude response exists for this packet yet.
- Do not claim a correction loop, provider reliability, or independent geometry-algorithm selection from this baseline.
- The exact 286-width gate is valid only for the declared 572x420 synthetic nominal source and 50 percent resize requirement.
- Do not broaden this into automatic image load, Preview/Run, layer selection, camera, calibration, PLC, I/O, account, or deployment behavior.
