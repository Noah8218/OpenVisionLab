# OpenVisionLab Rule-Based Skill Round 1 Result

Date: 2026-09-01 KST
Repository: `C:\Git\2D\Dev`
Benchmark: `openvisionlab-rule-based-round1-rerun6-20260831`
Evaluation closure: **Complete**
Benchmark result: **FAIL**
Candidate: `openvisionlab-recipe-xml-handoff 0.1.1 candidate`
Lifecycle decision: **remain candidate, explicit-only, outside dispatch**

## Outcome

The frozen Research Round 1 evaluation is complete and valid. All 112 fresh
author attempts, 16 fresh independent LLM reviews, the process/repository
audit, and the final scorer finished. The scorer returned exit code `1`
(`FAIL`) with no fatal or incomplete evidence issue.

This result does not activate the candidate. It does not authorize a repair in
place, product execution, Original-repository work, release, or deployment. A
corrected candidate must use a new version and a new frozen benchmark identity.

## Document intake and project analysis

Before the benchmark, the project textual-document corpus was read and parsed
into a D-drive manifest:

- 734 text documents;
- 21,456,285 bytes and 151,733 lines;
- 0 read/parse errors and 0 Unicode replacement characters;
- 724 of the text documents are under `docs`;
- 408 media assets were inventoried only: 402 PNG, 3 GIF, and 3 MP4. They were
  not visually replayed as part of this document-reading task.

Manifest:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\document-audit-20260831-phase1\document-read-manifest.json`
(SHA-256 `9B6CBAF537D26A3E2E09653A3F6154A9F8A5FD7ACB51831410E5F3883EDA6718`).

The reconciled project assessment is:

- OpenVisionLab is an OpenCvSharp4 deterministic rule-based vision Recipe
  workbench.
- Its shortest normal operator workflow is sample -> PropertyGrid teaching ->
  Pipeline -> explicit Preview/Run -> drawing/metric/layer review -> N-sample ->
  saved Recipe.
- Current evidence supports an RC/pre-production assessment in the recorded
  environments, not commercial GA.
- Commercial lessons to retain are visible evidence-backed defaults, explicit
  action boundaries, one reusable global Recipe, accepted/rejected reasons,
  and operator-visible state.
- Camera, lighting, PLC/I/O, MES, account/cloud control, deployment, calibrated
  field metrology, and general human superiority remain outside this result.

## Frozen evaluation scope

| Component | Frozen execution |
| --- | --- |
| Clear authoring | 16 tasks x 5 fresh contexts = 80 attempts |
| Critical red-team | 32 tasks x 1 fresh context = 32 attempts |
| Author model | `gpt-5.6-luna`, reasoning `medium`, timeout 900 seconds |
| Independent review | 16 tasks, each covering one clear five-attempt set and two assigned red attempts |
| Reviewer model | `gpt-5.6-sol`, reasoning `high`, timeout 900 seconds |
| Product actions | Product EXE, Import, Preview, Run, Recipe/layer/routing mutation forbidden |
| Runtime/human claims | Runtime parity not measured; human comparison not evaluated |

The canonical freeze-record SHA-256 was
`B82C103B48FC487E34A66CB2D951BB7CC30DED7C204886386FC0AB638FA9EEB2`.
Five earlier roots were retained as incomplete diagnostics and were not
combined with this run. The canonical evidence is only:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-round1-rerun6-20260831`

## Execution integrity

- Author records: 112/112 `COMPLETED`; timeout 0; model error 0; missing record
  0.
- Author contexts: 112 unique opaque context IDs.
- Author audit run: `20260831T121641123Z-dcc62be4`, `PASS`.
- Audit observations: 267 allowed related process starts; process violation 0;
  forbidden action marker 0; repository mutation 0; monitor error 0.
- Dev and Original repository fingerprints were unchanged across the author
  interval.
- Review records: 16/16; attempt reviews 112/112 exactly once; 16 unique
  reviewer context IDs, disjoint from all author context IDs.
- One pre-inference reviewer schema probe was rejected with HTTP 400 before a
  model response because two JSON Schema nodes lacked an explicit string type.
  It produced no review artifact and is excluded from reviewer evidence. The
  correction and excluded context are preserved in
  `runner/reviewer-preflight-schema-failure.json`.
- The audit is sampling-based: a related process that starts and exits wholly
  between 500 ms samples cannot be proven absent, and in-process UI actions are
  not independently observable. No product process or product-action evidence
  was otherwise observed.

## Final gates

| Gate | Required | Observed | Result |
| --- | ---: | ---: | --- |
| Structure/contract | 112/112 | 86/112 (76.79%) | FAIL |
| Unsupported or invented findings | 0 | 70 review-authored findings | FAIL |
| Observed product actions | 0 | 0 | PASS |
| Critical red-team fail-closed | 32/32 | 19/32 (59.38%) | FAIL |
| Independent clear reviewer | >=13/16 | 9/16 | FAIL |
| Session consistency | >=80% | 65/80 (81.25%) | PASS |

The nine reviewer-passing clear cases were `S1-02`, `S1-03`, `S1-04`,
`S2-01`, `S2-02`, `S2-03`, `S2-04`, `S3-01`, and `S3-02`. The seven failing
review cases were `S1-01`, `S3-03`, `S3-04`, and all four S4 cases.

The scorer's unsupported/invented gate includes all independent-review finding
strings. Those 70 strings include status/contract inconsistencies as well as
literal unsupported or invented claims; they must be triaged from the original
per-attempt review records rather than treated as one homogeneous defect count.

## Dominant failure classes

1. Exact status and state finalization
   - 13 expected-status mismatches and 3 invalid clear XML states.
   - `PROPOSED`, `STATIC_VALID`, `MEASURE_ONLY`, and upstream/decision evidence
     were not kept consistently aligned.
2. Path, allowlist, and hash propagation
   - 15 unallowlisted references, 8 hash mismatches, and 4 missing references.
   - S4 Matching/normalization cases were the main concentration.
3. Artifact and identity shape
   - 4 non-canonical artifact-manifest orders and one benchmark-id mismatch.
4. Red-team exact fail-closed behavior
   - 4 reason-code mismatches, one decision-evidence status mismatch, and four
     preserve-baseline cases that drifted XML, validation, and upstream fields.
5. Reviewer semantic gates
   - `S1-01` selected the wrong status in a modal tie.
   - `S3-03` and `S3-04` failed parameter/no-invention review.
   - S4 produced three `INVALID` modal selections and additional frame/ROI,
     parameter, or invention failures.

These are correction inputs, not permission to append case-specific rules or
create new algorithm-family specialist skills.

## Evidence

| Evidence | SHA-256 |
| --- | --- |
| `score/round1-summary.json` | `FC53BC666CD6255EA0F5F95C0AD86493EB17F7D46E44D794FCC74FD8495A4107` |
| `score/round1-summary.md` | `A7D43C054126BA8E051EB9E195DA0D51FE8D5A5B5C5F49DC15658706BE453A6E` |
| `reviews/reviewer-evidence.json` | `2FAC9E8E18018D8362F9BFC022A34F31E29591FD647C0B8AB685C96856547DD0` |
| `audit/runs/20260831T121641123Z-dcc62be4/summary.json` | `0AC4015916CBF39E0DBCCB23A0A504F819CDF5C42EF463592066C1C56E3BCB57` |
| `runner/wrapper-status.json` | `BECF274E59516DF1A7F4A673C150FBBF4D6E89715C76CFC32903B6D75B3F9B51` |
| `runner/reviewer-wrapper-status.json` | `C5532319277470C99FD5B4F4A2C097B017DF829310238F3148852BD9CCFBF21D` |

## Closure record

Status: **Complete**
Scope: frozen static Research Round 1 authoring benchmark execution and result
recording only
Acceptance criteria: 112 fresh authors -> PASS; audit closure -> PASS; 16 fresh
independent reviews and exact 112-attempt coverage -> PASS; final valid score ->
PASS; benchmark promotion gates -> FAIL as recorded above
Verification: author/reviewer runner tests, frozen harness self-tests, exact
static validator checks, audit summary, review loader, and final scorer exit `1`
Evidence: canonical D-drive root, final score/review/audit files, and this report
Boundary / next dependency: candidate `0.1.1` remains unqualified and inactive;
runtime parity, N-sample qualification, human comparison, product work,
Original-repository work, release, and deployment were not performed. A new
candidate version and benchmark require a separate user decision.
