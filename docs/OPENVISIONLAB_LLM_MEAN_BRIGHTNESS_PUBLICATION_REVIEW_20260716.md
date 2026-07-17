# OpenVisionLab GPT Mean Brightness Direct-Success Evidence Publication Review

Review date: 2026-07-16 KST

## Decision

- Privacy/public-asset decision: `GO`
- Dev repository decision: `ADDED TO THE DEV WORKTREE`
- Approval basis: explicit user inclusion instruction on 2026-07-16 KST
- Package: `docs/evidence/llm/20260716_mean_brightness_gpt_direct_success`
- Commit/push status: not performed as part of this inclusion step
- Original repository: not touched

## Evidence Classification

- Provider: GPT through a user-operated ChatGPT conversation
- Exact model/version: not provided
- Transfer method: manual copy
- API evidence: not supplied
- Evidence type: constrained direct success
- Correction rounds: 0
- Full conversation export: not captured

The first response followed the supplied one-Step `Mean` contract and passed current-build validation plus Good/Dark-NG execution. The prompt supplied the exact tool, parameters, and acceptance gate. This proves contract adherence for one task, not independent algorithm selection from an ambiguous image request or broad provider/model reliability.

## Included Package

| File | SHA-256 |
| --- | --- |
| `README.md` | `48156BD85A58E49A2A48A81221C80553BA8881117C58B532F58F192B8C8F279E` |
| `privacy_review.md` | `3C6DD952BB0BC4C442B5E15881658ED1780E7A60BC0B4A5F9B33616ABA01D665` |
| `prompt.md` | `87CF9840D3D7F90E94D5D263544D06CB923D1E95C60EDF2ACF99331BF5DD65CC` |
| `response.xml` | `19C2EA46F624CF6ED7E24A733078AACE8FA4756940EF5805FD733CE3C723E191` |
| `nominal_result.png` | `A7978A4A9B432710CDA6DBC42AEE0C35B439A787FECF1C8FC45DFE5A6519B380` |
| `negative_result.png` | `2EB25E357176B6999BC37479B97FF1B63624BFF50922B0F30E60B3ACA19D3C47` |

The prompt, response, and two result images are byte-identical to the audited raw evidence. `README.md` and `privacy_review.md` document the explicit Dev-worktree inclusion decision and tracked-path replay.

## Privacy And Metadata Review

- Package file count: 6
- Absolute Windows path matches: 0
- User-home path matches: 0
- Application-data path matches: 0
- Codex attachment path matches: 0
- Email matches: 0
- URL matches: 0
- Credential-token matches in the prompt/XML payload: 0
- Result PNG size: 572x420 each
- PNG chunks: `IHDR`, `IDAT`, `IEND` only
- Text/EXIF metadata chunks: 0

Excluded content includes raw reports, stack traces, local EXE paths, runtime recipe paths, generated runtime recipe identifiers, session data, and Recipe Manager captures.

## Public Asset Provenance

| Role | Repository path | SHA-256 |
| --- | --- | --- |
| Nominal OK | `docs/samples/public/Mean_Brightness_Synthetic_OK.png` | `4C8EB56BA73703719865AA62446944DC4DC991410BC7DDB746133577A4ED4FE9` |
| Dark NG | `docs/samples/public/Mean_Brightness_Synthetic_Dark_NG.png` | `28DF874ABE9B4CD8B0E5460210499CD167A00C69E455D470DA960FCCA88BD4D9` |

Both assets are OpenVisionLab project-authored public synthetic files registered by `docs/samples/public/OpenVisionLab.PublicSampleManifest.csv`.

## Current-Build Tracked-Path Replay

- Build: PASS, 0 warnings, 0 errors
- EXE: `bin/Debug/OpenVisionLab.exe`
- EXE timestamp: `2026-07-16 15:00:01 +09:00`
- EXE SHA-256: `53E9223D5343DEA9EC16E997ED32C14F0EE56E5878C6246E62BFA88DCD0B0F85`
- Evidence: `artifacts/p52_mean_tracked_package_20260716`
- Validation/import report SHA-256: `FA79E2550A8280B311FC2DEDE281888E55E14E16F28AEC112755656EBE188AC7`
- Nominal report SHA-256: `E21D93316303B3A05D040A5C12FD82B48B4BA961D538493A4F0100578BF866EB`
- Negative report SHA-256: `910B81A453B01728EF2BB29FE3F987C6527A900DFA8A981E414A82FA7FBFA426`

Observed results:

- Validation/import: PASS; 1 Step; 0 errors; 0 warnings; no external dependencies; `ImageRun: SKIPPED`.
- Nominal: PASS; `MeanValueAvg=201.5`; source channels 3; `6.079 ms`.
- Dark negative: expected product NG; `MeanValueAvg=117.5 < 185`; source channels 3; `5.316 ms`.
- Replayed nominal and negative PNGs are byte-identical to the included package.
- Validation/import and image runs remained separate explicit actions.

## Remaining Limits

- Exact GPT model/version is unknown, so model-to-model comparison is not possible.
- This is one highly constrained direct-success example, not a correction loop.
- The package does not show reliability across exposure drift, lighting variation, blur, occlusion, or real production variation.
- `MeanValueAvg` is sufficient only for this declared full-image brightness task; it does not prove object count, position, geometry, or color conformance.
- Inclusion does not change the OpenVisionLab product boundary or add automatic execution, camera, lighting, PLC, I/O, account, or deployment behavior.
