# OpenVisionLab GPT HSV Color-Patch Direct-Success Evidence Publication Review

Review date: 2026-07-16 KST

## Decision

- Privacy/public-asset decision: `GO`
- Dev repository decision: `ADDED TO THE DEV WORKTREE`
- Approval basis: explicit user inclusion instruction on 2026-07-16 KST
- Package: `docs/evidence/llm/20260716_hsv_color_patch_gpt_direct_success`
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

The first response followed the supplied one-Step `HSV` contract and passed current-build validation plus Good/Missing-patch execution. The prompt supplied the exact tool, parameters, circular red hue range, and acceptance gate. This proves contract adherence for one task, not independent algorithm selection from an ambiguous image request or broad provider/model reliability.

## Included Package

| File | SHA-256 |
| --- | --- |
| `README.md` | `778EA127ED8157B02AA8B6C851A10C0CCF57D7DACDECD0853D70AD762D9293BF` |
| `privacy_review.md` | `F34505B7318DAED8847D7853A96F94CB9D817B51FCDF989AE8BD7A21994A8BBE` |
| `prompt.md` | `B77AF4DE5B6BD37DB19DF6A2ABD4520D59A81FF312C4D71C15CCEA20127B7ACA` |
| `response.xml` | `1B85CA1DAEE5849AF2E059EC36186B37A3F7F43F096075C5727B282702E88587` |
| `nominal_result.png` | `C3D021DF99B680BE888597BE096A2BC13F3605E382B21850BC1A9B16C3ABB5A1` |
| `negative_result.png` | `06C6D742BF4FBAC55577E5CB47378158544E9ACBACF902EACAC244D738239C34` |

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
| Nominal OK | `docs/samples/public/HSV_ColorPatch_Synthetic_OK.png` | `FC12CB41A7B6D662BDDB91F89BE508702A4D2A23A3020CA64DCEE3E5A3C49FBD` |
| Missing-patch NG | `docs/samples/public/HSV_ColorPatch_Synthetic_Missing_NG.png` | `2DDC1DACEC1F6BB1109A4F53B4164B438DB99A762E1DA1A1C9421CA75F4C3854` |

Both assets are OpenVisionLab project-authored public synthetic files registered by `docs/samples/public/OpenVisionLab.PublicSampleManifest.csv`.

## Current-Build Tracked-Path Replay

- Build: PASS, 0 warnings, 0 errors
- EXE: `bin/Debug/OpenVisionLab.exe`
- EXE timestamp: `2026-07-16 15:00:01 +09:00`
- EXE SHA-256: `53E9223D5343DEA9EC16E997ED32C14F0EE56E5878C6246E62BFA88DCD0B0F85`
- Evidence: `artifacts/p49_hsv_tracked_package_20260716`
- Validation/import report SHA-256: `FA3553002C604CD532A959A1B56A5131E7B71AD9C57F6C60592D8F3AFC92EEFF`
- Nominal report SHA-256: `867678BCED29E08852CD2C0FA9DDF24330A4359FCFBF93C798FAFBF98FAF91F4`
- Negative report SHA-256: `9769C23138786E78758E7B6A0B02250DAB3AEAA58D4F2882F8AF222B272FC9F7`

Observed results:

- Validation/import: PASS; 1 Step; 0 errors; 0 warnings; no external dependencies; `ImageRun: SKIPPED`.
- Nominal: PASS; `MaskPixelCount=14000`; `MaskPixelRatio=0.058`; source channels 3; `7.176 ms`.
- Missing-patch negative: expected product NG; `MaskPixelCount=3500`; `MaskPixelRatio=0.015 < 0.05`; source channels 3; `6.904 ms`.
- Replayed nominal and negative PNGs are byte-identical to the included package.
- Validation/import and image runs remained separate explicit actions.

## Remaining Limits

- Exact GPT model/version is unknown, so model-to-model comparison is not possible.
- This is one highly constrained direct-success example, not a correction loop.
- The package does not show reliability across lighting, hue drift, saturation variation, blur, occlusion, or real production variation.
- `MaskPixelRatio` is sufficient only for this declared full-image color-coverage task; it does not prove object count, position, or shape conformance.
- Inclusion does not change the OpenVisionLab product boundary or add automatic execution, camera, lighting, PLC, I/O, account, or deployment behavior.
