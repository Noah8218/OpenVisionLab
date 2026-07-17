# OpenVisionLab GPT Edge-Fiducial Evidence Publication Review

Review date: 2026-07-16 KST

## Decision

- Privacy/public-asset decision: `GO`
- Dev repository decision: `ADDED TO THE DEV WORKTREE`
- Approval basis: explicit user instruction on 2026-07-16 KST
- Package: `docs/evidence/llm/20260716_edge_fiducial_gpt_direct_success`
- Commit/push status: not performed as part of this inclusion step yet
- Original repository: not touched

## Evidence Classification

- Provider: GPT through a user-operated ChatGPT conversation
- Exact model/version: not provided
- Transfer method: manual copy
- API evidence: no
- Evidence type: constrained direct success
- Correction rounds: 0

The first response followed the supplied one-Step `EdgeBasedMatching` contract and passed current-build validation and Good/Bad execution. The prompt supplied the exact tool, parameters, template path, and acceptance gates. This proves contract adherence for one task, not independent algorithm selection from an ambiguous request or broad provider/model reliability.

## Included Package

| File | SHA-256 |
| --- | --- |
| `README.md` | `65C6A01FAF7FBBC727E9E22FB02DA5AA657ADA97A032860DC35871FD2A9D9E90` |
| `privacy_review.md` | `CA658D80866AC6EA0FB4B40D5247D76C7AF5830394DCD25818665B006E0F6E50` |
| `prompt.md` | `F51731D2E7CE0985BFCBD7272E474BABA1083E72DB64727C15CED6C61BD41F1E` |
| `response.xml` | `55D63ADCF58B463ACAC25FF80C64AE96E966A6CEE642C4BCE7E13DF5129B3DCD` |
| `nominal_result.png` | `E96C0DABC5BD9729D73C3D9C7A809B39D2377B08468F94FCAD6704D334223C4C` |
| `negative_result.png` | `3A4AC87AC944F59D8F37D1AC165DF42B098D872A27058EB5F5DBDF3056AF8ECE` |

The prompt, response, and two result images are byte-identical to the P33 artifact-only candidate and P32 raw evidence. README and privacy review were rewritten to record the explicit inclusion decision and tracked-path replay.

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
| Nominal OK | `docs/samples/public/Edge_Fiducial_Synthetic_OK.png` | `55E3AEECC5ADEFEE2923E80E6D283B079E99856C99D30B894853E9B11764088C` |
| Wrong-shape NG | `docs/samples/public/Edge_Fiducial_Synthetic_Wrong_NG.png` | `4B1CBD6B7ADE19601B30C8261C6B3E584AA50823F2DE426DACD1ED7DE0D0C77C` |
| Edge template | `docs/samples/public/templates/Edge_Fiducial_Synthetic_Template.png` | `85C01DFF6234291A21E55FEC15583EBF57F17D574432A3AF0DE2D0C3536E69AD` |

All three assets are OpenVisionLab project-authored public synthetic files registered by `docs/samples/public/OpenVisionLab.PublicSampleManifest.csv`.

## Current-Build Tracked-Path Replay

- Build: PASS, 0 warnings, 0 errors
- EXE: `bin/Debug/OpenVisionLab.exe`
- EXE timestamp: `2026-07-16 08:39:15 +09:00`
- EXE SHA-256: `D265C6C6BFB7827B6128E5C046553BBD93FABB869C4251BAC37EA5A129B9B8A3`
- Evidence: `artifacts/p34_edge_fiducial_tracked_package_20260716`
- Validation/import report SHA-256: `B2D7D91623F57528A4010E0A73B53B4CC2133DBAF9F10E63F9D1D6CA7CACA8AF`
- Nominal report SHA-256: `BB14BF25F6C1A93E70CBF4AFCFF43BD74016A76D93583410CEA07906D7115F11`
- Negative report SHA-256: `01BBF2B6D4C5A9ED54FC418ADC77C02C8C47F831E53B59AAED7972AA34045C23`

Observed results:

- Validation/import: PASS; 1 Step; 0 errors; 0 warnings; both template dependencies copied; `ImageRun: SKIPPED`.
- Nominal: PASS; `ResultCount=1`; `ScoreMin/Max/Avg=99.598`; overlay count 1.
- Wrong-shape negative: expected product NG; validation remained successful; `ResultCount=0`; `BestScore=61.052`; overlay count 0; command exit code 1.
- Replayed nominal and negative PNGs are byte-identical to the included package.
- Validation/import and image runs remained separate explicit actions.

## Remaining Limits

- Exact GPT model/version is unknown, so model-to-model comparison is not possible.
- This is one highly constrained direct-success example, not a correction loop.
- The package does not show reliability across lighting, scale, rotation, blur, occlusion, or real production variation.
- Inclusion does not change the OpenVisionLab product boundary or add automatic execution, camera, lighting, PLC, I/O, account, or deployment behavior.
