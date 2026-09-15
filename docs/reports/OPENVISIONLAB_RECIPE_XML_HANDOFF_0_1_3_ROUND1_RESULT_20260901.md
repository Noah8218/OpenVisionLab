# OpenVisionLab Recipe XML Handoff 0.1.3 — Round 1 Result

Date: 2026-09-01 KST  
Repository: `C:\Git\2D\Dev`  
Benchmark: `openvisionlab-rule-based-round1-v013-rerun2-20260901`  
Candidate: `openvisionlab-recipe-xml-handoff 0.1.3` (`candidate`, explicit-only,
outside normal dispatch)

## Outcome

The frozen static Round 1 evaluation completed with scorer status **FAIL** and
exit code `1`. The v0.1.3 candidate was actually implemented and fully
evaluated, but it is not promotable. It remains inactive and unqualified. This
result does not authorize activation, normal dispatch, XML Import/Preview/Run,
Recipe or layer/routing mutation, product qualification, Original-repository
work, release, or deployment.

## Project intake and analysis

The prior full textual-document intake is retained at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\document-audit-20260831-phase1\document-read-manifest.json`
(SHA-256 `9B6CBAF537D26A3E2E09653A3F6154A9F8A5FD7ACB51831410E5F3883EDA6718`):
734 text documents, 21,456,285 bytes, 151,733 lines, and zero read/parse
errors. 408 media files were inventoried, not visually replayed for that
document-reading result.

The reconciled product identity is an OpenCvSharp4 deterministic rule-based
vision Recipe workbench. Its normal operator path is sample -> PropertyGrid
teaching -> Pipeline -> explicit Preview/Run -> drawing/metric/layer review ->
N-sample validation -> saved Recipe. Current evidence supports an RC/pre-
production assessment in the recorded environments, not commercial GA.
Camera, lighting, PLC/I/O, MES, account/cloud control, deployment, calibrated
field metrology, and human-superiority claims remain outside this benchmark.

## Frozen scope

| Item | Frozen execution |
| --- | --- |
| Clear authoring | 16 tasks x 5 fresh contexts = 80 attempts |
| Critical red-team | 32 tasks x 1 fresh context = 32 attempts |
| Author model | `gpt-5.6-luna`, reasoning `medium`, timeout 900 s |
| Independent review | 16 fresh blinded contexts; each reviewed one clear set and two red attempts |
| Product actions | Product EXE, Import, Preview, Run, Recipe/layer/routing mutation forbidden |
| Runtime/human claims | Runtime parity not measured; human comparison not evaluated |

## Integrity and execution notes

- Authors: 112/112 `COMPLETED`; timeout 0; model error 0; missing run record 0.
- The first v0.1.3 rerun identity was invalidated after four attempts because
  an external Dev file drifted. That root is preserved as invalid evidence and
  was not scored. The valid rerun2 identity above was rebuilt with fresh
  attempt/review outputs and is the sole authoritative result here.
- Audit: run `20260901T061507586Z-6b642f29`, result `PASS`; 73 allowed related
  process starts, 0 process violations, 0 forbidden action markers, 0 monitor
  errors, and 0 repository mutations. Dev and Original fingerprints were
  unchanged across the author interval (`BFC8D96311E262F34A8540DD19EA9F9FAF0E86B5FEDB454A6EF32316CAA581AD`
  and `46EA0A5F0CBEAC1DC6660C0393EA70AEFA926FD2C020F96DB161EB89A94B9B83`,
  respectively, at both ends).
- Reviews: 16/16 with exact 112-attempt coverage; independent clear-reviewer
  task pass was 12/16. S3-04 initially timed out; it was retried with the same
  frozen inputs and an extended in-memory timeout, then the standard wrapper
  rerun completed the remaining S4 reviews. The final reviewer evidence file
  contains all 16 task reviews.
- The frozen public manifest supplied the safe-static-baseline fixture. No
  product or repository repair was performed during this valid rerun; the
  scorer nevertheless recorded baseline-preservation and pointer-drift
  findings in affected red attempts.

## Final gates

| Gate | Required | Observed | Result |
| --- | ---: | ---: | --- |
| Structure/contract | 112/112 | 88/112 (78.57%) | FAIL |
| Unsupported or invented findings | 0 | 68 | FAIL |
| Observed product actions | 0 | 0 | PASS |
| Critical red-team fail-closed | 32/32 | 17/32 (53.13%) | FAIL |
| Independent clear reviewer | >=13/16 | 12/16 | FAIL |
| Session consistency | >=80% | 72/80 (90.00%) | PASS |

The final scorer recorded no fatal or incomplete-evidence issue. The main
failure classes were:

1. visual or semantic facts were invented from the input (for example, red
   regions were claimed where the assigned image showed blue blocks);
2. artifact status and reason-code drift (`PROPOSED`, `WAIT`, `REJECTED`, and
   `MEASURE_ONLY`) remained in both clear and red attempts;
3. frame/layer names, helper stages, Tool substitutions, and unowned optional
   parameters were invented despite the closed-world rule;
4. unexecuted runtime/measurement gates were marked `PASS`, or required XML
   was claimed while absent;
5. S3 ROI ownership, projection direction, contour geometry, and gap geometry
   still drifted from the reviewed packet; and
6. S4 normalized-frame ROI/branch/parameter/status claims were not always
   supported by the supplied Matching evidence.

The four clear reviewer task failures were `S1-02`, `S1-04`, `S3-01`, and
`S3-04`. Reviewer judgments for the other 12 clear tasks passed their
tool-family, physical-target, frame/ROI, parameter, and no-invention gates.

## Comparison with v0.1.2

The correction improved the independent reviewer task result from 10/16 to
12/16, while the final benchmark still regressed or remained below gate on
structure (90/112 -> 88/112), critical red fail-closed (19/32 -> 17/32), and
unsupported/invented findings (64 -> 68). Product actions stayed at 0 and
session consistency stayed at 72/80. This is an observed benchmark comparison,
not a claim that the model or corpus is generally better or worse outside this
frozen run.

## Evidence identity

| Evidence | SHA-256 |
| --- | --- |
| `frozen/public/public-manifest.json` | `58ACD410F13B1A57BA11F62959027D183B37D420372913C5A8346D2B0DD159EE` |
| `frozen/private/freeze-record.json` | `F555E5165222AE532A074194A2537ECB49445B1577DEBEB5245A7B8A5AE10521` |
| `score/round1-summary.json` | `3F56D80C2F0113872683CDE6CCFA289501E7F20480D6488FD2858C8E24F03F1A` |
| `score/round1-summary.md` | `9E9F422F4BDC15572E9062C6A9E48D9A73A084A6CCCF43EC76AB39DCC1F810D8` |
| `reviews/reviewer-evidence.json` | `FACB49D014CF9CC3C87F1FC292E9B9CDD321A190BC86D5B2EF2BADC53C9876E6` |
| `audit/runs/20260901T061507586Z-6b642f29/summary.json` | `83594DB0D45E02B14AE8B71A2BD72B26085940C42AA3B2813CA1106132A7F9B0A` |
| `runner/wrapper-status.json` | `9AACDFFD08CB08E8695C767698CBE5F51CE06AC653359CB0115076F9BDBD1B83` |
| `runner/reviewer-wrapper-status.json` | `8130FA635549195FCAEC507D59B535E29C0774A230E63349277531B03224758A` |

## Closure record

Status: **Complete**  
Scope: frozen static Round 1 authoring benchmark execution and result recording
for candidate `0.1.3` only.  
Acceptance criteria: author coverage 112/112 -> PASS; audit closure -> PASS;
review coverage 16/16 and 112 attempts -> PASS; final scorer -> FAIL with all
gate results recorded above.  
Verification: focused candidate tests and Skill Creator validation passed;
author runner exit `0`; reviewer wrapper final exit `0`; final scorer exit `1`.
Evidence: the D-drive benchmark root above, this report, the focused v0.1.3
report, and the linked score, review, audit, and frozen artifacts.  
Boundary / next dependency: the candidate remains explicit-only, outside normal
dispatch, inactive, and unqualified. A new correction/version and new
benchmark identity require a separate operator decision; Round 2, runtime
parity, qualification, human comparison, product work, Original-repository
work, release, and deployment remain outside scope.
