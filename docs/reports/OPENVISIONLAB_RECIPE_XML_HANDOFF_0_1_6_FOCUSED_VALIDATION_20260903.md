# OpenVisionLab Recipe XML Handoff 0.1.6 — Focused Validation

Date: 2026-09-03 KST  
Repository: `C:\Git\2D\Dev`  
Installed candidate: `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff`  
Parent evidence: `OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_TRIAGE_20260902.md` and
`OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_CORPUS_REFREEZE_20260903.md`  
Status: **Complete for the candidate-only Gate B C7–C10 focused checkpoint; candidate remains inactive**

## Scope and lifecycle boundary

The installed candidate is now `openvisionlab-recipe-xml-handoff 0.1.6`. This
checkpoint implements only the approved candidate-side C7–C10 correction after
the clean Gate A corpus identity. The public
`openvisionlab-recipe-xml-handoff-v1` shape, Tool catalog, explicit-only
invocation policy, registry status `candidate`, dispatch exclusion, and
`qualification: false` boundary are unchanged.

No new Tool family, public schema, hidden case table, image interpretation,
product EXE, Import, Preview/Run, Recipe/layer/routing mutation, activation,
Round 2, fresh 112-author/16-reviewer benchmark, Original-repository change,
commit, push, release, or deployment was performed.

## Implemented C7–C10 correction

- **C7 — executable bundle finalizer:** the validator now requires the
  supplied contract's exact artifact set with unique ordinal entries and
  attempt-local paths. A clear bundle binds `handoff.xmlArtifact` to one and
  only one `candidate.pipeline.xml`, binds `validation.reportPath` to
  `static-compatibility.txt`, verifies the manifest-listed first validator's
  current version/exact handoff/`PASS`/empty errors/no manifest phase, and
  rejects a manifest that is older than a listed file or a later extra XML.
  Blocked bundles reject leftover local XML.
- **C8 — trust-boundary projection:** evidence entries must be exact
  `{path, sha256}` pairs; blocked unverifiable references must collapse to
  `PENDING/UNKNOWN`. Upstream stage/tool/gate owner fields are non-empty and
  use the reviewed owner vocabulary. Inline, unhashed upstream content keeps
  `PROPOSED` precedence and is not rejected solely for lacking a file hash.
- **C9 — public routing and baseline precedence:** supplied baseline wrapper
  pointers are re-read and hash-verified before field-preservation checks.
  Decision evidence must use the public reason vocabulary; historical
  `AUTO_REJECTED_*` aliases are rejected even when a supplied benchmark
  contract lists them. Existing status, product-action, and protected baseline
  fields remain opaque.
- **C10 — focused regression:** the standard-library suite now has 20 cases,
  retaining the v0.1.5 C1–C6 coverage and adding clear bundle/XML/report
  binding, first-validator identity, manifest-last mutation, blocked pending
  references, inline precedence, owner/evidence shape, baseline pointer bytes,
  and public-reason alias routing.

## Resource identity

| Resource | Bytes | SHA-256 |
| --- | ---: | --- |
| `SKILL.md` | 21,773 | `95E937FF0D7A60CCAB4BC858744D1720A3EB38D894E42F24789A79A713AA1E1C` |
| `references/recipe-xml-handoff-contract.md` | 12,857 | `86E9F8CD08D448BAC6228A187A8E03C894B0D3ED600E263E04AF3A98F1B00E00` |
| `scripts/validate_recipe_xml_handoff.py` | 68,586 | `ECAB14961C9AFB71A5F15DC894DAE1FF46D3ADDCF36BBC52A17503625A945FF3` |
| `scripts/test_recipe_xml_handoff.py` | 42,308 | `34F742D18859E541B6BE2BF8579725DB9892DF38E6C42F6D1A3F4BC41D3C5527` |
| `agents/openai.yaml` (unchanged) | 350 | `E66F0729CD38076507B72A8FD8772DAD925796C6FBA5C01B7E10DFF8783B4C9A` |
| `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v016-forward-20260903\forward-probe-v016.json` | 1,130 | `08FAAC723BF845DE88E61E495EA28A0F6B2FF264BD2952C45B1DACD93D04E7DE` |

## Verification evidence

| Check | Command/result |
| --- | --- |
| Skill Creator | `python -X utf8 -B C:\Users\USER\.codex\skills\.system\skill-creator\scripts\quick_validate.py C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff` → `Skill is valid!` |
| Focused regression | `python -B C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\scripts\test_recipe_xml_handoff.py` with `OPENVISIONLAB_TEST_ROOT=D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v016-tests-20260903` → 20 tests, `OK`, exit `0`; transcript: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v016-forward-20260903\gate-b-evidence\focused-regression.txt` |
| Python compilation | `python -B -m py_compile ...validate_recipe_xml_handoff.py ...test_recipe_xml_handoff.py` → exit `0`; transcript: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v016-forward-20260903\gate-b-evidence\py-compile.txt` |
| Independent forward probe | `forward-probe-v016.json` records fresh positive `MEASURE_ONLY/FILE/PASS` and blocked `WAIT/NOT_EMITTED/NOT_RUN`; both validator CLI results are `PASS`, skill version `0.1.6`, and no product action was performed. |
| Resource hashes | Captured in `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v016-forward-20260903\gate-b-evidence\resource-hashes.json`. |

The first unqualified Skill Creator invocation used the Windows default
cp949 codec and stopped while reading the UTF-8 skill text. The same official
validator was rerun with `python -X utf8` and passed; no source workaround or
encoding mutation was added.

## Governance synchronization

The candidate registry, current handoff, rule-based skill work contract,
research plan, documentation map, and LLM document index now point to this
v0.1.6 focused evidence. Candidate lifecycle remains explicit-only,
outside normal dispatch, inactive, and unqualified. Gate A's repaired corpus
identity remains separate and immutable; its full author/reviewer benchmark
has not been run.

## Closure record

Status: **Complete**  
Scope: candidate-only v0.1.6 C7–C10 implementation, 20-case focused
regression, UTF-8 Skill Creator validation, Python compilation, independent
positive/blocked forward probe, and Dev governance synchronization.  
Acceptance criteria: the candidate finalizer binds the required clear/red
artifact sets and first validator, blocks unverifiable references, preserves
inline/baseline/status/reason precedence, and has focused evidence without a
new schema, Tool family, hidden mapping, product action, or activation.  
Verification: 20 standard-library tests, `quick_validate.py`, `py_compile`,
independent D-drive forward outputs, candidate resource hash review, and
registry/documentation checks recorded in this report.  
Evidence: this report and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v016-forward-20260903\forward-probe-v016.json`.  
Boundary / next dependency: a fresh 112-author/16-reviewer benchmark against
`openvisionlab-rule-based-round1-v015-corpusfix-20260903` requires separate
operator admission; activation, normal dispatch, product execution,
qualification, release, deployment, and Original-repository work remain
unapproved.
