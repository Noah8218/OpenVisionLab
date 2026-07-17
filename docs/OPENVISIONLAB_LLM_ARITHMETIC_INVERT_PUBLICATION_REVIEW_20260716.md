# OpenVisionLab GPT Arithmetic Invert Direct-Success Evidence Publication Review

Review date: 2026-07-16 KST

## Decision

- Privacy/public-asset decision: `GO`
- Dev repository decision: `ADDED TO THE DEV WORKTREE`
- Approval basis: user approved the P64 Arithmetic evidence inclusion step on 2026-07-16 KST
- Package: `docs/evidence/llm/20260716_arithmetic_invert_gpt_direct_success`
- Commit/push status: not performed as part of this inclusion step
- Original repository: not touched

## Evidence Classification

- Provider identity: `GPT`, as identified by the user
- Exact model/version: not provided
- Transfer method: manual copy
- API evidence: not supplied
- Evidence type: constrained direct success
- Correction rounds: 0
- Full provider-chat export: not captured

The first response followed the supplied two-Step `Arithmetic(Bitwise_NOT) -> Mean` contract and passed current-build validation plus nominal/Bright-NG execution. The prompt supplied the exact tool family, parameters, route, and acceptance gate. This proves contract adherence for one task, not independent algorithm selection from an ambiguous image request or broad provider/model reliability.

## Included Package

| File | SHA-256 |
| --- | --- |
| `README.md` | `1A5A2DC3CFF35C4DCA81BC1AC29A76B2A564791FF1948957FE651307434E72B6` |
| `privacy_review.md` | `B03D9CABFFE1864320C2D49F82A761A70C79C0BE7E40FFB10846B0E0DC4A2A37` |
| `prompt.md` | `74D53D1904D097C189A2E09656E882C4A9FB3C1D62926029867A3719B4D8CD67` |
| `response.xml` | `B1C215FCCE1AC5C696A48515D1FFB48681A3F50F34556D763760AC7807D07A1B` |
| `nominal_result.png` | `D6E9822DA94A919C924D5518DA49220A5E32BE1DCD4E38B62721568A3E0DE243` |
| `negative_result.png` | `6F05A34049A11B4056EB0E22A1B27D5A8E62E6CCD9CEF4AAE0476617B2DE318E` |

The prompt, response, and two result images are byte-identical to the audited raw evidence. `README.md` and `privacy_review.md` document the explicit Dev-worktree inclusion decision and tracked-path replay.

## Privacy And Metadata Review

- Package file count: 6
- Full package text scan: 0 disallowed local-path, contact, URL, credential-marker, or private/legacy asset-hint matches
- Result PNG size: 572x420 each
- PNG chunk types: `IHDR`, `IDAT`, `IEND` only
- Text/EXIF metadata chunks: 0

Excluded content includes raw manifests, validation/run reports, local EXE paths, runtime recipe paths, generated runtime recipe identifiers, stack traces, session data, and Recipe Manager captures.

## Public Asset Provenance

| Role | Repository path | SHA-256 |
| --- | --- | --- |
| Nominal OK | `docs/samples/public/Arithmetic_Invert_Synthetic_OK.png` | `D53BC22EB96EC87CAAC4CA46A7F52DB9B4FC04AD3E6C561B759557DCB126207A` |
| Bright-input NG | `docs/samples/public/Arithmetic_Invert_Synthetic_Bright_NG.png` | `72514BA2524E0998676ECE85A18D6A6D7C91900E4FA3B1A951F62293725C086E` |

Both assets are OpenVisionLab project-authored public synthetic files registered by `docs/samples/public/OpenVisionLab.PublicSampleManifest.csv`.

## Current-Build Tracked-Path Replay

- Build: PASS, 0 warnings, 0 errors
- EXE: `bin/Debug/OpenVisionLab.exe`
- EXE timestamp: `2026-07-16 17:39:27 +09:00`
- EXE SHA-256: `53E9223D5343DEA9EC16E997ED32C14F0EE56E5878C6246E62BFA88DCD0B0F85`
- Evidence: `artifacts/p65_arithmetic_invert_tracked_package_20260716`
- Validation/import report SHA-256: `85D62C8F83241FD649A6CE22A0C1E5C7082BF79810A15B3DFEEB268E72A24967`
- Nominal report SHA-256: `3D998D2013F9E66D787EB6E5586BDEC9D0916E9CE0D350BA68EFABD023C1C2EA`
- Bright-NG report SHA-256: `AF596CD31DC445A176C3505EB04AD19BA64D1FD7B89DB0AD0E68DA35D6DD9A49`

Observed results:

- Validation/import: PASS; 2 Steps; 0 errors; 0 warnings; no external dependencies; `ImageRun: SKIPPED`.
- Nominal: PASS; `MeanValueAvg=208`; 15.558 ms.
- Bright-input negative: expected product NG; `MeanValueAvg=76.7 < 190`; 5.929 ms.
- Replayed nominal and Bright-NG PNGs are byte-identical to the included package.
- Validation/import and image runs remained separate explicit actions.

## Remaining Limits

- Exact GPT model/version is unknown, so model-to-model comparison is not possible.
- This is one highly constrained direct-success example, not a correction loop.
- The package does not show reliability across real noise, focus variation, exposure drift, occlusion, or production variation.
- The `MeanValueAvg=190..230` gate is sufficient only for this declared public synthetic brightness inversion task; it does not prove geometry, position, color, or metrology conformance.
- Inclusion does not change the OpenVisionLab product boundary or add automatic execution, camera, lighting, PLC, I/O, account, or deployment behavior.
