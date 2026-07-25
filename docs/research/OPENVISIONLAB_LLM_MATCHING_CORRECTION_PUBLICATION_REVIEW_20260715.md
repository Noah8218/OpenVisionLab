# OpenVisionLab GPT Matching Correction-Loop Publication Review

Date: 2026-07-15

Candidate: `artifacts/llm_transcripts/sanitized/20260715_matching_die_pad_gpt_correction_loop`

Included package: `docs/evidence/llm/20260715_matching_die_pad_gpt_correction_loop`

## Decision

**GO / ADDED TO THE DEV WORKTREE**

The sanitized candidate is technically reproducible, uses only registered OpenVisionLab synthetic assets, and contains no detected local drive path, user-home path, credential, email, URL, attachment path, private asset, or customer data. After the conditional decision was presented, the user explicitly instructed Codex to include the sanitized copy under `docs` on 2026-07-15. The seven-file minimum package was added to the Dev worktree.

The package has not been staged, committed, pushed, or copied to the Original repository. Dev-worktree inclusion does not mean that it is remotely published.

## Evidence Classification

- Provider: GPT through one user-operated ChatGPT conversation
- Exact model/version: unknown
- Transfer: manual
- API evidence: no
- Round 1: XML/schema/routing valid, dependency validation NG, Import blocked
- Round 2: exactly two parameter values corrected, validation/import PASS
- Correction rounds: 1
- Explicit execution: nominal PASS and no-target expected NG

Round 1 failed because the initial OpenVisionLab packet gave GPT the wrong relative-path root for the current Debug host. GPT followed that instruction. The evidence therefore proves a report-driven host-contract correction loop; it must not be described as GPT independently diagnosing an undocumented path convention.

## Audit Results

| Item | Result |
| --- | --- |
| Included package file count | 7 |
| Round 1 prompt | Byte-identical to raw: `B7FFEF69...8725D` |
| Round 1 response | Byte-identical to raw: `479D5D08...ED2FA` |
| Round 2 response | Byte-identical to raw: `E608A864...66B79` |
| Result images | Byte-identical to raw and current-build replay |
| Round 2 prompt sanitization | Four raw workspace-root occurrences replaced with `<REPO_ROOT>`; reverse substitution reconstructs raw byte-for-byte |
| Publishable text scan | No local drive/user-home path, credential, email, URL, AppData/Codex attachment path, private/legacy asset reference, or customer data detected |
| Response structure | Both parse as one-Step `Matching` pipelines; exactly `TemplatePath` and `PATTERN_PATH` differ |
| PNG metadata | Both 572x420; only `IHDR`, `IDAT`, and `IEND`; no text chunks |
| Input provenance | Nominal, negative, and template are registered OpenVisionLab-generated public synthetic assets |
| Current-build replay | Round 1 expected dependency NG; round 2 import PASS; nominal PASS; no-target expected NG |
| Runtime cleanup | No OpenVisionLab process or `Smoke_LlmDraft_*` recipe remains |

## Included Package Integrity

| File | SHA-256 |
| --- | --- |
| `README.md` | `6C24B58D5342C9D341307551859B305E5B052280192A9973C27A9F22CA7B1924` |
| `prompt_round1.md` | `B7FFEF690F57BCCAF49A631D7B4B49B472B0F3D1B23FA110001059115558725D` |
| `response_round1.xml` | `479D5D08C54DB2BCD13192261F03BFAF3C13F24D4B9673E4279DBF8D7FCED2FA` |
| `prompt_round2.md` | `F65783B60BE7EE4E469BBA9C8C02E1ADB60E8870DDE5E5967C308632769F84AC` |
| `response_round2.xml` | `E608A864A646CED3B5E62B48BDA05575E56AD2A33875F5BFE9407A5C7A966B79` |
| `nominal_result.png` | `9301827BB6A760AAF53564DD8D3222233D97BB6144C13130835A977B5DC4789B` |
| `negative_result.png` | `1A9A3CA224139FF5AF59645A021BE939F96E86CFB50927D10A2EC07A1C8FE665` |

Raw round 2 prompt hash: `B38AE2918337ED75CBAEE3592D2B59AC7672CE0BA56BAB5634DEF366783200CC`. The sanitized prompt intentionally has a different hash; the exact transformation is disclosed in the included README. The candidate README hash was `1F08A0E0...9A88`; the included README changes only publication status, package terminology, tracked replay path, and this review link.

## Public Replay Assets

| Role | Path | SHA-256 |
| --- | --- | --- |
| Nominal | `docs/samples/public/Matching_DiePad_Synthetic_OK.png` | `EF12511D298E9EB8BEE90AF40E75A075C992CB4B776A315C00E1C985812A68BF` |
| Negative | `docs/samples/public/Matching_DiePad_Synthetic_NoTarget_NG.png` | `C01E8770A33767A05B26F8B686C6AB0843D5A492C7FC6FA133F8198767EE2649` |
| Template | `docs/samples/public/templates/Matching_DiePad_Synthetic_Template.png` | `FE8B97997DF34A05B106AFD35FD495F26973F1896888ECE7273E8D9E69BA5144` |

The public manifest attributes all three to OpenVisionLab and `tools/GenerateOpenVisionSyntheticSamples.ps1`.

## Candidate Current-Build Replay

Evidence: `artifacts/p29_matching_correction_publication_review_20260715`

- Full solution build: PASS, 0 warnings, 0 errors.
- Round 1: dependency paths detected 2, missing 2, Import disabled, image run skipped.
- Round 2: dependencies detected 2, copied 2, missing 0, Import completed, image run skipped.
- Nominal: PASS, `ResultCount=3`, `ScoreMin=80.059`, `ScoreMax=93.074`, `ScoreAvg=86.444`, 30.848 ms.
- Negative: expected product NG, `ResultCount=0 < 3`, 23.598 ms.
- Replayed result PNG hashes match the candidate exactly.

## Included Package Replay

Evidence: `artifacts/p30_matching_correction_tracked_package_20260715`

- Round 1: dependency paths detected 2, missing 2, Import disabled, image run skipped.
- Round 2: dependencies detected 2, copied 2, missing 0, Import completed, image run skipped.
- Nominal: PASS, `ResultCount=3`, `ScoreMin=80.059`, `ScoreMax=93.074`, `ScoreAvg=86.444`, 41.523 ms.
- Negative: expected product NG, `ResultCount=0 < 3`, 29.561 ms.
- Both replayed result PNG hashes match the included package exactly.

## Post-Inclusion Repository Verification

- Full solution build: PASS, 0 warnings, 0 errors.
- Readiness contract: PASS.
- External-reference policy: PASS.
- Public-sample policy: PASS (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`).
- Package audit: exactly 7 files; all expected hashes match; no trailing whitespace in publishable text.
- Privacy scan: no drive/user-home path, email, credential value, URL, AppData/Codex path, attachment filename, or private asset path.
- Runtime cleanup: 0 OpenVisionLab processes and 0 `Smoke_LlmDraft_*` recipe directories.
- `git diff --check`: PASS with line-ending conversion warnings only.

## Included Package

The approved Dev-worktree package contains exactly these files:

```text
docs/evidence/llm/20260715_matching_die_pad_gpt_correction_loop/
  README.md
  prompt_round1.md
  response_round1.xml
  prompt_round2.md
  response_round2.xml
  nominal_result.png
  negative_result.png
```

Raw manifests, reports, screenshots, stack traces, runtime recipe paths, and local session data remain excluded. Readiness, external-reference, public-sample, package-content, privacy, hash, and `git diff --check` checks passed after inclusion. Do not commit, push, or touch the Original repository without a separate explicit request.

## Policy Basis

- `docs/OPENVISIONLAB_PUBLIC_SAMPLE_ASSET_POLICY.md`
- `docs/OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md`
- `docs/OPENVISIONLAB_LLM_TRANSCRIPT_PUBLICATION_REVIEW_20260715.md`
- Repository `LICENSE`

The earlier direct-success publication review records the external publication-policy references used by this project. This document applies the same project gate and adds no new legal conclusion.
