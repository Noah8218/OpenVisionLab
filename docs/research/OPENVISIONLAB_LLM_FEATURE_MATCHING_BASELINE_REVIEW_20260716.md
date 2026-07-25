# OpenVisionLab FeatureMatching LLM Baseline Review

Date: 2026-07-16

Decision: `GO FOR A SELF-CONTAINED MANUAL GPT PACKET`

This review establishes the OpenVisionLab rule-based baseline before asking an external LLM to author FeatureMatching XML. It is not an LLM transcript and does not claim that GPT generated or validated the inspection.

## Product boundary

- OpenVisionLab remains an LLM-assisted OpenCvSharp4 rule-based vision recipe workbench.
- The tested inspection uses a PropertyGrid-backed `FeatureMatching` tool.
- Preview and Run remain explicit user actions.
- No camera, lighting, PLC, I/O, account, deployment, automatic layer switch, or automatic execution behavior is involved.

## Public synthetic evidence

| Role | Path | SHA-256 |
| --- | --- | --- |
| Nominal | `docs/samples/public/Feature_Card_Synthetic_OK.png` | `AA0A0092FFA40A22CCD1C44AF66BF59FC198BD89A39E649918FDD090E9B63C26` |
| Wrong-card negative | `docs/samples/public/Feature_Card_Synthetic_Wrong_NG.png` | `3791EC6701F4426A3DCADAE588F5B0BFAA784990326BFB8C07E66C33CAA3703F` |
| Template | `docs/samples/public/templates/Feature_Card_Synthetic_Template.png` | `75D535A584969862B8B1CE600EE7DCB88E5AC04BAD5C4BAF92EC89C3F1D0A1CD` |
| Reference pipeline | `docs/samples/public/Public_Feature_Card.pipeline.xml` | `7F80FE7EAFD281AEC96D4FD3BE086881F83D8464DDED3B74A6627FFE3A530AA2` |

All three images are registered OpenVisionLab-generated synthetic assets in `docs/samples/public/OpenVisionLab.PublicSampleManifest.csv`.

## Verified contract

- Tool sequence: exactly one `FeatureMatching` Step.
- Route: `Main -> Feature_Preview`.
- Template parameters: `TemplatePath` and `PATTERN_PATH` reference the same project-authored template.
- `SCORE_MIN=0.85` is the normalized Lowe descriptor-ratio input.
- `RANSAC_REPROJ_THRESHOLD=4` is the geometric reprojection tolerance in pixels.
- Full-image inspection: `USE_ROI=false`.
- Acceptance: `ScoreMax` from `80` through `100`.
- Maximum Step time: `3000 ms`.

`ResultCount` is not sufficient for this task. The wrong-card negative consistently returns one weak geometric hypothesis, but its `ScoreMax=26.7` is correctly rejected by the 80-point acceptance gate.

## Current-build evidence

Build command:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
```

Result: PASS, 0 warnings, 0 errors.

Current EXE:

- Path: `bin/Debug/OpenVisionLab.exe`
- Timestamp: `2026-07-16 09:39:33 +09:00`
- SHA-256: `53E9223D5343DEA9EC16E997ED32C14F0EE56E5878C6246E62BFA88DCD0B0F85`

The current EXE ran each sample five times sequentially with `llm-xml-image-run`. Evidence is under `artifacts/p35_feature_matching_sequential_20260716`.

| Sample | Runs | Expected outcome | Observed outcome | ResultCount | ScoreMax | Time range | Unique result hashes |
| --- | ---: | --- | --- | ---: | ---: | --- | ---: |
| Nominal | 5 | PASS | 5 PASS | 1 | 96.7 | 210.595-288.316 ms | 1 |
| Wrong-card negative | 5 | inspection NG | 5 expected FAIL | 1 | 26.7 | 261.463-286.012 ms | 1 |

Result-image hashes were stable across all five sequential runs:

- Nominal: `6FD550656825FB95E2133F6532885FEC37BF131B85A70361EC3ED780B84F4914`
- Wrong-card negative: `FF663DD3AF10A846FC146507DDA3F05A600F13EE8B6A19119F3C8BAC0D4EC57B`

### Dependency-path replay condition

The packet uses the verified application-startup-relative template path `..\..\docs\samples\public\templates\Feature_Card_Synthetic_Template.png`.

- Recipe Manager validation/import resolved both template parameters to the registered public template, copied both dependencies into the generated recipe, enabled import, and reported `ImageRun: SKIPPED`.
- The raw XML image-run harness failed to load that relative path when it was deliberately launched with the repository root as its working directory. This is not the normal application-startup path.
- Re-running the same unchanged XML with `bin/Debug` as the process working directory passed the nominal image at `ScoreMax=96.7` and produced the expected wrong-card NG at `ScoreMax=26.7`.

Therefore raw-XML replay commands must either run from the EXE startup directory or execute the dependency-rewritten pipeline imported by Recipe Manager. The failed repository-root harness attempt remains under `artifacts/p35_feature_matching_packet_20260716/good` and `bad`; the valid startup-directory replay is under `good_startup_cwd` and `bad_startup_cwd`.

## Decision and next gate

The public FeatureMatching baseline is deterministic enough for a bounded external GPT XML-authoring test. The self-contained packet belongs under `llm_prompt_packets/feature_matching_card` and must include byte-identical copies of the nominal, negative, and template images.

The next gate is external evidence:

1. Send the three packet images and the complete `COPY_THIS_TO_GPT.txt` content in one new GPT conversation.
2. Preserve GPT's first response unchanged.
3. Validate/import it in Recipe Manager without running automatically.
4. Run the nominal and negative images explicitly.
5. Use the correction prompt only if the unchanged response produces a real validation or behavior failure.

Do not fabricate a failed first response or correction round.
