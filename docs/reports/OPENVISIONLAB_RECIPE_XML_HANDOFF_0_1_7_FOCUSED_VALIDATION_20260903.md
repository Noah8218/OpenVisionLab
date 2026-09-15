# OpenVisionLab Recipe XML Handoff 0.1.7 — Focused Validation

Date: 2026-09-03 KST  
Repository: `C:\Git\2D\Dev`  
Installed candidate: `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff`  
Parent evidence: `OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_TRIAGE_20260903.md`  
Status: **Complete for the candidate-only 0.1.7 focused correction; candidate remains inactive**

## Scope and lifecycle boundary

The installed candidate is now `openvisionlab-recipe-xml-handoff 0.1.7`.
This checkpoint implements only the observed candidate-owned authority-lock
and projection-guidance corrections. The v1 handoff schema, explicit-only
invocation policy, registry status `candidate`, normal-dispatch exclusion, and
`qualification: false` boundary are unchanged.

No frozen benchmark/corpus, hidden answer key, Tool family, product EXE,
Import, Preview/Run, Recipe/layer/routing mutation, activation, full
`112 + 16` admission run, Original-repository change, commit, push, release,
or deployment was performed.

## Implemented 0.1.7 correction

- **Generic ROI authority ownership:** the validator no longer assumes that a
  generic `operatorLocks[].kind == "ROI"` belongs to `LineDistance`. It now
  matches the exact `USE_ROI=true` and `CvROI` pair on any enabled consumer;
  the edge-sampling, binary, object-area, and normalized-frame lock branches
  retain their Tool/frame-specific candidate sets.
- **Status and reason guidance:** `SKILL.md` and the machine-readable contract
  now make the observed precedence explicit: inline/unhashed upstream remains
  `PROPOSED`; passed file-backed XML without Good/Bad fields is
  `MEASURE_ONLY`; an immutable baseline keeps its original protected status;
  missing repository contracts, locator output frame, upstream graph review,
  and direct layer/routing mutation use their specific public reason codes.
  Historical `AUTO_REJECTED_*` aliases remain forbidden; the unresolved
  artifact-contract vocabulary conflict is not changed here.
- **Focused regression:** the standard-library suite adds a non-`LineDistance`
  (`Mean`) generic-ROI case while retaining all 0.1.6 checks, for 21 tests.

## Resource identity

| Resource | Bytes | SHA-256 |
| --- | ---: | --- |
| `SKILL.md` | 23,373 | `A36ADFEE3CCB7D0A8107C2FB2F5E8FB7DBDAF803FEB80B4D80CCC6F73F824A12` |
| `references/recipe-xml-handoff-contract.md` | 13,619 | `D9F30951EEDA7D73A676359D7791588CCAD246422AC5BC244A094EA1E406014B` |
| `scripts/validate_recipe_xml_handoff.py` | 68,738 | `8EFA2BC6A32AF343341BDE71249BE1E8880BE19E2FFC33B40F2E0B96191C3A3A` |
| `scripts/test_recipe_xml_handoff.py` | 43,044 | `329FE06ADFB2361B39A29D2182D9AE3B244C83B6A1B91568BAD0426102B1B004` |
| `agents/openai.yaml` (unchanged) | 350 | `E66F0729CD38076507B72A8FD8772DAD925796C6FBA5C01B7E10DFF8783B4C9A` |
| `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v017-forward-20260903\forward-probe-v017.json` | 2,391 | `7B1BAF7F974AA488930E37EB79639EAFCDB64D9FC5183A3EE2FFD9940CD4A0C3` |

The frozen v0.1.6 benchmark root and its recorded candidate hashes were not
rewritten. The installed candidate remains explicit-only and is not activated.

## Verification evidence

| Check | Command/result |
| --- | --- |
| Skill Creator | `python -X utf8 -B C:\Users\USER\.codex\skills\.system\skill-creator\scripts\quick_validate.py C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff` → `Skill is valid!`, exit `0`; transcript: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v017-forward-20260903\gate-evidence\skill-quick-validate.txt` |
| Focused regression | `OPENVISIONLAB_TEST_ROOT=D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v017-focused-20260903 python -X utf8 -B C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\scripts\test_recipe_xml_handoff.py -v` → 21 tests, `OK`, exit `0`; transcript SHA-256 `E54F46A1EB2108C47674CCA9A713B530DC5CF0E1DB14782E90C525491E968FD6`. |
| Python compilation | `python -X utf8 -B -m py_compile ...validate_recipe_xml_handoff.py ...test_recipe_xml_handoff.py` → exit `0`; transcript SHA-256 `E55665FB131904B27D1B192672301B76724BFD7FF2FB528284A1895DC6B18BA7`. |
| Independent forward probe | `forward-probe-v017.json` records fresh positive `MEASURE_ONLY/FILE/PASS` and blocked `WAIT/NOT_EMITTED/NOT_RUN`; both direct validator and CLI results are `PASS`, skill version `0.1.7`, and every `productActions` flag is `false`. |
| Forward CLI evidence | Positive validator output SHA-256 `EF07A65778771237538387DD6D57FBB65AE7CF951B40AE9605B1052D57221F76`; blocked output SHA-256 `0DF19B8C098E4430D7ABED75E5311A3886AD6B118371A174EAEA6E118122F967`. |
| Skill registry | `validate_skill_registry.py ... --json` → `PASS`; `test_skill_registry.py` → `PASS`; transcripts: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v017-forward-20260903\gate-evidence\skill-registry-validation.json`, `skill-registry-regression.txt`. |
| Documentation index / JSON | `tools\TestDocumentationIndex.ps1` → `DocumentationIndex=PASS` (`131` indexed paths, `13` routes, `102` root redirects); both changed JSON files parse as UTF-8 JSON. |
| Diff hygiene | `git diff --check` on the changed Dev documentation set → exit `0` (Git emitted only existing LF/CRLF normalization warnings). |

The first Skill Creator invocation on this Windows host used the default
`cp949` codec and stopped on the UTF-8 skill text. The same official validator
was rerun with `python -X utf8` and passed; no source encoding workaround was
added.

## Governance and admission boundary

The candidate registry, current handoff, research plan, work contract,
documentation map, and LLM document index are updated to point to this report
and version `0.1.7`. The v0.1.6 full Round 1 result remains independently
recorded as `INCOMPLETE` (audit scan-root violation, scorer fatal chain, and
reviewer guard block). This focused result is not a new benchmark admission
and does not resolve the frozen alias vocabulary, legacy safe-baseline shape,
S3-02 projection lock, S4-01 prerequisite, or author-runner/scorer defects.

## Closure record

Status: **Complete**  
Scope: candidate-only 0.1.7 generic-ROI validator correction, status/reason
precedence guidance, 21-case regression, UTF-8 Skill Creator validation,
Python compilation, independent positive/blocked forward validation, and Dev
governance synchronization.  
Acceptance criteria: the generic ROI lock accepts an explicitly matching
non-`LineDistance` consumer; existing strict status, baseline, evidence, and
public-reason guards remain green; no product or frozen-benchmark mutation —
**met**.  
Verification: 21 standard-library tests, `quick_validate.py`, `py_compile`,
independent D-drive forward outputs, CLI validator outputs, and resource hash
review.  
Evidence: this report and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v017-forward-20260903\forward-probe-v017.json`.  
Boundary / next dependency: reconcile the frozen corpus/contract and runner
scan-root issues before considering a new benchmark identity; candidate
activation, normal dispatch, product execution, qualification, release,
deployment, and Original-repository work remain unapproved.
