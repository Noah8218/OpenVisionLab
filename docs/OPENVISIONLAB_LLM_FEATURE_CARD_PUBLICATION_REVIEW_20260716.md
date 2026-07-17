# OpenVisionLab GPT Feature Card Direct-Success Evidence Publication Review

Review date: 2026-07-16 KST

## Decision

- Privacy/public-asset decision: `GO`
- Dev repository decision: `ADDED TO THE DEV WORKTREE`
- Approval basis: explicit user continuation instruction on 2026-07-16 KST
- Package: `docs/evidence/llm/20260716_feature_card_gpt_direct_success`
- Commit/push status: not performed as part of this inclusion step
- Original repository: not touched

## Evidence Classification

- Provider: GPT through a user-operated ChatGPT conversation
- Exact model/version: not provided
- Transfer method: manual copy
- API evidence: no
- Evidence type: constrained direct success
- Correction rounds: 0

The first response followed the supplied one-Step `FeatureMatching` contract and passed current-build validation plus Good/Wrong execution. The prompt supplied the exact tool, parameters, template path, and acceptance gate. This proves contract adherence for one task, not independent algorithm selection from an ambiguous request or broad provider/model reliability.

## Included Package

| File | SHA-256 |
| --- | --- |
| `README.md` | `68D13992F0106F120F26B0185E14B97F3E50D52AB284BE9145642F554EE4D5E0` |
| `privacy_review.md` | `2CA546C5223B137D0C636F9E5C2CE1C6426F93EAAE320A166AE09F85B986A160` |
| `prompt.md` | `B9F32A8C0CBD00C1569C3CC9049ABFF00C928B8F47C6BEFFD5EC64D60CD7BBFE` |
| `response.xml` | `F06E9259E8FE48363142AFFDBB047EC191FCF90B91A19B56CDE7797757E4E245` |
| `nominal_result.png` | `6FD550656825FB95E2133F6532885FEC37BF131B85A70361EC3ED780B84F4914` |
| `negative_result.png` | `FF663DD3AF10A846FC146507DDA3F05A600F13EE8B6A19119F3C8BAC0D4EC57B` |

The prompt, response, and two result images are byte-identical to the P37 sanitized candidate and P36 raw evidence. `README.md` and `privacy_review.md` were rewritten for the explicit Dev-worktree inclusion decision and the tracked-path replay.

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
| Nominal OK | `docs/samples/public/Feature_Card_Synthetic_OK.png` | `AA0A0092FFA40A22CCD1C44AF66BF59FC198BD89A39E649918FDD090E9B63C26` |
| Wrong-card NG | `docs/samples/public/Feature_Card_Synthetic_Wrong_NG.png` | `3791EC6701F4426A3DCADAE588F5B0BFAA784990326BFB8C07E66C33CAA3703F` |
| Feature template | `docs/samples/public/templates/Feature_Card_Synthetic_Template.png` | `75D535A584969862B8B1CE600EE7DCB88E5AC04BAD5C4BAF92EC89C3F1D0A1CD` |

All three assets are OpenVisionLab project-authored public synthetic files registered by `docs/samples/public/OpenVisionLab.PublicSampleManifest.csv`.

## Current-Build Tracked-Path Replay

- Build: PASS, 0 warnings, 0 errors
- EXE: `bin/Debug/OpenVisionLab.exe`
- EXE timestamp: `2026-07-16 09:39:33 +09:00`
- EXE SHA-256: `53E9223D5343DEA9EC16E997ED32C14F0EE56E5878C6246E62BFA88DCD0B0F85`
- Evidence: `artifacts/p38_feature_card_tracked_package_20260716`
- Validation/import report SHA-256: `529F790EA64C9B7838DD6D8456603F27B591FA95B06B3B468CD58CB047D40E95`
- Nominal report SHA-256: `26709CEB901FADEE1C30EF91994E8836508B412CC929B126282B885FB1F7C23D`
- Negative report SHA-256: `490DE61C242B8BC02748EDD628D16E0547DDBE632391A36EDEB566975AF1E73D`

Observed results:

- Validation/import: PASS; 1 Step; 0 errors; 0 warnings; both template dependencies copied; `ImageRun: SKIPPED`.
- Nominal: PASS; `ResultCount=1`; `ScoreMin/Max/Avg=96.7`; overlay count 1; `355.149 ms`.
- Wrong-card negative: expected product NG; validation remained successful; `ResultCount=1`; `ScoreMax=26.7 < 80`; overlay count 1; `287.081 ms`; command exit code 1.
- Replayed nominal and negative PNGs are byte-identical to the included package.
- Validation/import and image runs remained separate explicit actions.

## Remaining Limits

- Exact GPT model/version is unknown, so model-to-model comparison is not possible.
- This is one highly constrained direct-success example, not a correction loop.
- The package does not show reliability across lighting, scale, rotation, blur, occlusion, or real production variation.
- `ResultCount=1` alone is insufficient for this task because the wrong-card sample also returns one candidate; the `ScoreMax` acceptance gate is essential.
- Inclusion does not change the OpenVisionLab product boundary or add automatic execution, camera, lighting, PLC, I/O, account, or deployment behavior.
