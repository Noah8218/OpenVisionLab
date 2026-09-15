# OpenVisionLab Recipe XML Handoff 0.1.2 — Round 1 Result

Date: 2026-09-01 KST  
Repository: `C:\Git\2D\Dev`  
Benchmark: `openvisionlab-rule-based-round1-v012-rerun3-20260901`  
Candidate: `openvisionlab-recipe-xml-handoff 0.1.2` (`candidate`, explicit-only,
outside normal dispatch)

## Outcome

The frozen static Round 1 evaluation completed with scorer status **FAIL** and
exit code `1`. The candidate remains inactive and unqualified. This result does
not authorize activation, normal dispatch, XML Import/Preview/Run, Recipe or
layer/routing mutation, product qualification, Original-repository work,
release, or deployment.

## Project intake and analysis

The prior full textual-document intake is retained at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\document-audit-20260831-phase1\document-read-manifest.json`
(SHA-256 `9B6CBAF537D26A3E2E09653A3F6154A9F8A5FD7ACB51831410E5F3883EDA6718`):
734 text documents, 21,456,285 bytes, 151,733 lines, and zero read/parse
errors. 408 media files were inventoried, not visually replayed for this
document-reading result.

The reconciled product identity is an OpenCvSharp4 deterministic rule-based
vision Recipe workbench. Its normal operator path is sample -> PropertyGrid
teaching -> Pipeline -> explicit Preview/Run -> drawing/metric/layer review ->
N-sample validation -> saved Recipe. Current evidence supports an RC/pre-
production assessment in the recorded environments, not commercial GA. Camera,
lighting, PLC/I/O, MES, account/cloud control, deployment, calibrated field
metrology, and human-superiority claims remain outside this benchmark.

## Frozen scope

| Item | Frozen execution |
| --- | --- |
| Clear authoring | 16 tasks × 5 fresh contexts = 80 attempts |
| Critical red-team | 32 tasks × 1 fresh context = 32 attempts |
| Author model | `gpt-5.6-luna`, reasoning `medium`, timeout 900 s |
| Independent review | 16 fresh blinded contexts; each reviewed one clear set and two red attempts |
| Product actions | Product EXE, Import, Preview, Run, Recipe/layer/routing mutation forbidden |
| Runtime/human claims | Runtime parity not measured; human comparison not evaluated |

## Integrity and setup note

- Authors: 112/112 `COMPLETED`; timeout 0; model error 0; missing run record 0.
- Audit: run `20260901T013904507Z-3336f5bb`, result `PASS`; 73 allowed related
  process starts, 0 process violations, 0 forbidden action markers, 0 monitor
  errors, and 0 repository mutations. Dev and Original fingerprints were
  unchanged across the author interval.
- Reviews: 16/16, exact 112-attempt coverage, and reviewer task pass `10/16`.
- The declared safe-baseline external directory
  `...\rule-based-skill-phase1-20260831\positive-v0.1.2` was absent when the
  author run started. Before scoring, the four declared files were restored
  from the byte-identical `positive-v0.1.1` baseline (hashes preserved). No
  frozen benchmark file or Git repository was changed. The author outputs were
  not re-run after this path repair; baseline-preservation failures therefore
  carry this setup caveat and should not be treated as a pristine path-presence
  replay.

## Final gates

| Gate | Required | Observed | Result |
| --- | ---: | ---: | --- |
| Structure/contract | 112/112 | 90/112 (80.36%) | FAIL |
| Unsupported or invented findings | 0 | 64 | FAIL |
| Observed product actions | 0 | 0 | PASS |
| Critical red-team fail-closed | 32/32 | 19/32 (59.38%) | FAIL |
| Independent clear reviewer | ≥13/16 | 10/16 | FAIL |
| Session consistency | ≥80% | 72/80 (90.00%) | PASS |

The final scorer recorded no fatal or incomplete-evidence issue. The dominant
failure classes were exact status/state drift, missing or non-canonical artifact
sets, tool-signature and frame/parameter mismatches, six safe-baseline
preservation drifts, and red-team reason/status mismatches. Reviewer failures
were concentrated in no-invention and frame/parameter judgments for S1-01,
S1-03, S1-04, S3-01, S3-03, and S4-04.

## Evidence identity

| Evidence | SHA-256 |
| --- | --- |
| `frozen/public/public-manifest.json` | `EA14B57343CA4D30675CFAC6E71B08E588C9392BB7584FF436729CC5F3A994DE` |
| `frozen/private/freeze-record.json` | `E763ADBDC68570634F214715626D881A07A588C0042BA42CBBFF9D931BD73693` |
| `score/round1-summary.json` | `8BF5925CCBD2522343C95BD6EA2B6399D8684B953A911CBCE0FFD5C34CFEE848` |
| `score/round1-summary.md` | `07FC983C0ABFC38316CAC35DE3F0A620E4432894F57BBC94C23E99975CF9370B` |
| `reviews/reviewer-evidence.json` | `71CD95EF4099CE53C0F92C40474CA3F3963F4CFA9760034B18612E53216F293F` |
| `audit/runs/20260901T013904507Z-3336f5bb/summary.json` | `15C6DE4A707166472B954C76F444EF06678C0D58E4759B6D96F9B76A45B43409` |
| `runner/wrapper-status.json` | `0258AB4DE6A584C943CA452653B955C39C975077267ED5C10C8C11A8C5A3A4F6` |
| `runner/reviewer-wrapper-status.json` | `5F51C949F30F25C791F69EBBE4D5BF4BC2E87F4CD49AEACC99B211C6833E9502` |

## Closure record

Status: **Complete**  
Scope: frozen static Round 1 authoring benchmark execution and result recording
for candidate `0.1.2` only.  
Acceptance criteria: author coverage 112/112 -> PASS; audit closure -> PASS;
review coverage 16/16 and 112 attempts -> PASS; final scorer -> FAIL with all
gate results recorded above.  
Verification: harness/runner tests and self-test passed before execution;
author runner exit `0`; reviewer runner exit `0`; final scorer exit `1`.  
Evidence: the D-drive benchmark root above, this report, and the linked score,
review, audit, and frozen artifacts.  
Boundary / next dependency: focused 0.1.2 validation remains PASS, but the full
benchmark does not promote the candidate. A new correction/version and new
benchmark identity require a separate decision; Round 2, runtime parity,
qualification, human comparison, product work, Original-repository work,
release, and deployment remain outside scope.
