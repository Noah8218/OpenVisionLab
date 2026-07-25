# OpenVisionLab GPT Filter Denoise Direct-Success Evidence Publication Review

Review date: 2026-07-16 KST

## Decision

- Privacy/public-asset decision: `GO`
- Dev repository decision: `ADDED TO THE DEV WORKTREE`
- Approval basis: user approved the P57 Filter evidence inclusion step on 2026-07-16 KST
- Package: `docs/evidence/llm/20260716_filter_denoise_gpt_direct_success`
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

The first response followed the supplied three-Step `Filter -> Threshold -> Contour` contract and passed current-build validation plus Good/Missing-NG execution. The prompt supplied the exact tool family, parameters, route, and acceptance gate. This proves contract adherence for one task, not independent algorithm selection from an ambiguous image request or broad provider/model reliability.

## Included Package

| File | SHA-256 |
| --- | --- |
| `README.md` | `BD009782DCACE317B250868706237775C560CBF6B7AA1BEE4693F1568CCAA6B3` |
| `privacy_review.md` | `7A722D5836A40B0EB10962F18AD19EFEE139278F1050E9A44AD0BC4C13D962B8` |
| `prompt.md` | `654BC48DA14717CA76CBA15A5EBBC800CD60C2C2CCFCD5E3964E55F58D07B6C1` |
| `response.xml` | `4E684E9171EAC77D656CC851C9292C211BCB5A8AB8848E2D95F54833A8D6855E` |
| `nominal_result.png` | `F19B25B7E02B7B98737ADB68AE86C38907E90B3C52856A51327E6C3967FE8D6C` |
| `negative_result.png` | `BFCD2853FDF9E29FBA663E83C7762CBF16D5DA9866EB610A507EBF7C95E19DC2` |

The prompt, response, and two result images are byte-identical to the audited raw evidence. `README.md` and `privacy_review.md` document the explicit Dev-worktree inclusion decision and tracked-path replay.

## Privacy And Metadata Review

- Package file count: 6
- Full package text scan: 0 disallowed local-path, contact, URL, credential-marker, or private/legacy asset-hint matches
- Result PNG size: 572x420 each
- PNG chunks: `IHDR`, `IDAT`, `IEND` only
- Text/EXIF metadata chunks: 0

Excluded content includes raw manifests, validation/run reports, local EXE paths, runtime recipe paths, generated runtime recipe identifiers, stack traces, session data, and Recipe Manager captures.

## Public Asset Provenance

| Role | Repository path | SHA-256 |
| --- | --- | --- |
| Nominal OK | `docs/samples/public/Filter_Denoise_Synthetic_OK.png` | `2F415394EE5F38275790406FB38F58C93E7DCC2764BE806A1098A9C67FC333EA` |
| Missing target NG | `docs/samples/public/Filter_Denoise_Synthetic_Missing_NG.png` | `514585BB54689B32F5F0FD4C5784418F20BC2C1B2E4CB394A3D2AE826A7672ED` |

Both assets are OpenVisionLab project-authored public synthetic files registered by `docs/samples/public/OpenVisionLab.PublicSampleManifest.csv`.

## Current-Build Tracked-Path Replay

- Build: PASS, 0 warnings, 0 errors
- EXE: `bin/Debug/OpenVisionLab.exe`
- EXE timestamp: `2026-07-16 17:39:27 +09:00`
- EXE SHA-256: `53E9223D5343DEA9EC16E997ED32C14F0EE56E5878C6246E62BFA88DCD0B0F85`
- Evidence: `artifacts/p58_filter_denoise_tracked_package_20260716`
- Validation/import report SHA-256: `C90679C8AC48112CD36ED9370CDDFCE0625BFCB31AC4BC63C983EA3DA826BD87`
- Nominal report SHA-256: `E4CF7EA54BEA2E7AE1F29B85D61DF6588AD38CC5979D42BFB4646F5DF40063E8`
- Negative report SHA-256: `70C72048256CEBAD90A01DED4F4A694789C45F27E7E4BECB66C9091821FDCADF`

Observed results:

- Validation/import: PASS; 3 Steps; 0 errors; 0 warnings; no external dependencies; `ImageRun: SKIPPED`.
- Nominal: PASS; `ResultCount=4`; source channels `3 -> 3 -> 1`; 33.009 ms.
- Missing-target negative: expected product NG; `ResultCount=2 < 4`; source channels `3 -> 3 -> 1`; 29.16 ms.
- Replayed nominal and negative PNGs are byte-identical to the included package.
- Validation/import and image runs remained separate explicit actions.

## Remaining Limits

- Exact GPT model/version is unknown, so model-to-model comparison is not possible.
- This is one highly constrained direct-success example, not a correction loop.
- The package does not show reliability across real noise, focus variation, exposure drift, occlusion, or production variation.
- The `ResultCount=4` gate is sufficient only for this declared public synthetic target-count task; it does not prove geometry, position, color, or metrology conformance.
- Inclusion does not change the OpenVisionLab product boundary or add automatic execution, camera, lighting, PLC, I/O, account, or deployment behavior.
