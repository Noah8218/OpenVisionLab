# OpenVisionLab Recipe XML Handoff 0.1.4 — Round 1 Result

Date: 2026-09-02 KST
Repository: C:\Git\2D\Dev
Benchmark: openvisionlab-rule-based-round1-v014-20260901
Evidence root: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v014-20260901
Candidate: openvisionlab-recipe-xml-handoff 0.1.4 (candidate, explicit-only, outside normal dispatch)

## Outcome

The frozen static Round 1 evaluation completed with scorer status FAIL and exit
code 1. The run is complete negative evidence: the candidate is not promotable,
not activated, and remains unqualified. This result does not authorize normal
dispatch, XML Import, Preview/Run, Recipe or layer/routing mutation, product
qualification, Original-repository work, commit, push, release, or deployment.

## Project identity and scope

OpenVisionLab is an OpenCvSharp4 deterministic rule-based vision Recipe
workbench. Its normal operator path is sample -> PropertyGrid teaching ->
Pipeline composition -> explicit Preview/Run -> drawing/metric/layer review ->
N-sample validation -> saved Recipe. The product evidence supports an
RC/pre-production assessment in the recorded environments, not commercial GA.
Camera/lighting, PLC/I/O, MES, account/cloud control, deployment, calibrated
field metrology, and human-superiority claims were outside this benchmark.

The complete textual-document intake remains recorded at
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\document-audit-20260831-phase1\document-read-manifest.json:
734 text documents, 21,456,285 bytes, 151,733 lines, and zero read/parse
errors. Media were inventoried rather than visually replayed for that intake.

## Frozen execution

| Item | Frozen execution |
| --- | --- |
| Clear authoring | 16 cases x 5 fresh contexts = 80 attempts |
| Critical red-team | 32 cases x 1 fresh context = 32 attempts |
| Author model | gpt-5.6-luna, reasoning medium, timeout 900 seconds |
| Independent review | 16 fresh blinded contexts; one clear case and two red cases per reviewer |
| Product actions | Product EXE, Import, Preview, Run, Recipe/layer/routing mutation forbidden |
| Runtime/human claims | Runtime parity not measured; human comparison not evaluated |

## Integrity and coverage

- Authors: 112/112 recorded and COMPLETED; timeout 0; model error 0; missing
  run record 0.
- Author audit: run
  20260901T131651857Z-34d94be8, result PASS; observed related starts 80/80,
  process violations 0, forbidden action markers 0, monitor errors 0, and
  repository mutations 0. Dev and Original fingerprints were unchanged:
  Dev 86AF6E67F784F7D26E3FCCB2A6704B6662C7CAE8D8B50877123DCEF42D5FC92C at
  both ends; Original 46EA0A5F0CBEAC1DC6660C0393EA70AEFA926FD2C020F96DB161EB89A94B9B83
  at both ends.
- Reviewers: 16/16 task reviews, exact 112-attempt coverage, and reviewer
  wrapper exit 0. Independent clear-reviewer task pass was 12/16.
- Harness self-test: PASS. No product EXE or repository mutation was performed.

## Final gates

| Gate | Required | Observed | Result |
| --- | ---: | ---: | --- |
| Structure/contract | 112/112 | 84/112 (75.00%) | FAIL |
| Unsupported or invented findings | 0 | 58 | FAIL |
| Observed product actions | 0 | 0 | PASS |
| Critical red-team fail-closed | 32/32 | 16/32 (50.00%) | FAIL |
| Independent clear reviewer | >=13/16 | 12/16 | FAIL |
| Session consistency | >=80% | 74/80 (92.50%) | PASS |

The final scorer recorded no fatal or incomplete-evidence issue. The four
failed clear reviewer tasks were S1-04, S3-01, S4-01, and S4-04.

## Failure analysis

The observed failures cluster into four correction targets:

1. Structure and canonical projection: 28 attempts did not produce a fully
   valid contract-bound artifact set. S1-04 and S3-01 modal selections became
   INVALID, while S4-03 consistency fell to 3/5.
2. Evidence ownership: 58 independent reviewer findings identified invented or
   unsupported frame names (for example LocatorFrame/BinaryMaskFrame),
   unowned ROI/parameter claims, semantic image claims, false runtime/validator
   assertions, and retained-baseline pointer/version drift.
3. Fail-closed red-team routing: only 16/32 red cases matched the frozen
   expected status and reason contract. Several used generic or wrong reason
   codes instead of the case-specific required code, and four validator
   cases retained invalid upstream evidence.
4. Reviewer-level semantic/parameter review: S4-01 failed frame/ROI review and
   S4-04 failed parameter review even though their selected Tool signatures
   were otherwise present.

These are benchmark observations from exposed frozen cases, not a claim about
unseen tasks or human performance.

## Comparison with v0.1.3

| Metric | v0.1.3 | v0.1.4 | Delta |
| --- | ---: | ---: | ---: |
| Structure pass | 88/112 (78.57%) | 84/112 (75.00%) | -4 attempts, -3.57 pp |
| Unsupported/invented | 68 | 58 | -10 |
| Red fail-closed | 17/32 (53.13%) | 16/32 (50.00%) | -1, -3.13 pp |
| Clear reviewer pass | 12/16 | 12/16 | unchanged |
| Product actions | 0 | 0 | unchanged |
| Session consistency | 72/80 (90.00%) | 74/80 (92.50%) | +2 attempts, +2.50 pp |

The v0.1.4 patch reduced unsupported/invented findings and improved session
consistency, but it did not meet any of the four failed promotion gates.

## Evidence identity

| Evidence | SHA-256 |
| --- | --- |
| frozen/public/public-manifest.json | 48387207ED183D32C8D06D9494924513CAD70FA7F67837D6E0BF3ABE1D64B044 |
| frozen/private/freeze-record.json | AE5BB6ACEC5820C89ED64FBECE8C2006D8368503C50631AA343D4053667BEDB8 |
| frozen/public/artifact-contract.json | F8F9D5D8DC9856CF111D20DEE8C17070300EA328A6CB67F57F301C8E36EFC963 |
| frozen/private/reviewer-protocol.json | 743FC36710E1F61838F0C2457509F6ADE92B10FCBA48D3831A32862642084BC9 |
| score/checkpoint-112-pre-review/round1-summary.json | 720EA1120ADE2DE05A8E492D70EE465C3328803629F4AB76D0DB037097EA2853 |
| score/round1-summary.json | 05C1D5D3559233E74EE06B66809BFB89B9E9D9C6D2B1706864E5FE32FAFF99B9 |
| score/round1-summary.md | 50FFAC47F4857F2DFD4BC1DAE2801E2A1DFF8F22E9091F0249BDFD83916A71BB |
| reviews/reviewer-evidence.json | E8E3C5B10B285F727F1F7F5D1787FC6AFDB7BC8388973FB7A4F20926C31CD35A |
| audit/runs/20260901T131651857Z-34d94be8/summary.json | 0DC6596236AAA7E9A012AF1A3E75F1FA076075631F3C191FA62035DCAFA9C6DA |
| runner/wrapper-status.json | 9209F1D4A1FAFE68DFEA9529C72C00378A7C2A35CEEEFDA61CCAC010FB8D0EED |
| runner/reviewer-wrapper-status.json | 40BBD431E89EE6A56D0CED987A26F8B8947CB6314F7D9E73974FFB3B44CBE567 |
| harness/self-test-output/self-test-result.json | DCC32F1FF31380B76DC09EA2F0E40B1E539BCB124AB62BFF69287598AA510180 |
| docs/contracts/openvisionlab/OPENVISIONLAB_RULE_BASED_SKILL_REGISTRY.json | E6A7D30D4264406D2D4588C4287FAA8C28DA8C3CAC055EBAA5898EC8BF3EFC91 |

## Closure record

Status: Complete
Scope: frozen v0.1.4 Round 1 authoring/reviewer benchmark execution and
evidence-backed result recording.
Acceptance criteria: author coverage 112/112, audit closure PASS, review
coverage 16/16 with 112 attempts, final scorer execution, gate metrics, hashes,
and lifecycle boundary recorded.
Verification: author wrapper exit 0; author audit PASS; reviewer wrapper exit 0;
harness self-test PASS; final scorer exit 1 with all gates recorded.
Evidence: this report and the D-drive benchmark root above, including score,
review, audit, freeze, and runner artifacts.
Boundary / next dependency: the candidate remains explicit-only, inactive,
outside normal dispatch, and unqualified. The next priority is a separate,
minimal v0.1.4 failure-triage/correction decision; no new benchmark, activation,
Round 2, product runtime, Original work, release, or deployment is implied.
