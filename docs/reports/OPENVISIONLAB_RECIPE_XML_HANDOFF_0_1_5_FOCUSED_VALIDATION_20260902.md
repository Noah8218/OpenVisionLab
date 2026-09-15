# OpenVisionLab Recipe XML Handoff 0.1.5 — Focused Validation

Date: 2026-09-02 KST  
Repository: `C:\Git\2D\Dev`  
Installed candidate: `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff`  
Parent evidence: `OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_TRIAGE_20260902.md`  
Status: **Complete for the candidate-only C1–C6 checkpoint; candidate remains inactive**

## Scope and lifecycle boundary

The installed candidate is now `openvisionlab-recipe-xml-handoff 0.1.5`. This
checkpoint implements the minimum C1–C6 correction scope from the v0.1.4
failure triage without changing the public
`openvisionlab-recipe-xml-handoff-v1` shape, adding an algorithm family, or
touching product source. Registry status remains `candidate`; invocation is
explicit-only and outside normal dispatch; `qualification` and every product
action remain false. No Import, Preview/Run, Recipe mutation, release,
deployment, Original-repository change, commit, or push was performed.

The v0.1.4 full Round 1 benchmark remains the authoritative `FAIL` result. No
new 112-author/16-reviewer benchmark was admitted or run by this checkpoint.

## Implemented C1–C6 correction

- **C1 — finalization gate:** `SKILL.md` and the reference contract now require
  one frozen decision tuple and a blocking finalization/projection preflight
  before completion prose, XML, handoff, manifest, or decision evidence. A
  missing XML, invalid artifact order, hash/evidence failure, or retained
  first-validator result other than `PASS` cannot be described as a completed
  file-backed handoff. The existing artifact-bundle hard stop remains covered.
- **C2 — owner matrix/projection:** the candidate explicitly separates case /
  authority ownership (ROI, thresholds, area, acceptance, downstream locks),
  reviewed upstream graph ownership, specialist packet ownership, repository
  Tool/parameter ownership, and validator evidence ownership. Cross-owner
  repair, semantic image labels, invented frames, and helper stages remain
  forbidden.
- **C3 — opaque baseline:** the existing validator baseline comparison remains
  field- and pointer-preserving; the candidate instructions now place it inside
  the same frozen projection gate and prohibit substituting another baseline.
- **C4 — public routing:** blocked decisions are routed from the supplied
  request/evidence pattern, never a case identifier or hidden mapping, while
  preserving the exact public reason vocabulary and state precedence.
- **C5 — S4 lock:** Matching/NormalizeImage rules now require the retained
  specialist frame (`PartFrame` remains `PartFrame`; no generic `LocatorFrame`),
  keep downstream policy case/upstream-owned, and enforce catalog
  `commonParameters` plus the guide-defined shared branch/ROI pairs. The
  validator rejects an unknown Tool parameter and a specialist stage/packet
  frame mismatch.
- **C6 — focused matrix:** the standard-library suite grew from 12 to 15 tests,
  adding missing-XML finalization, specialist stage-frame drift, and
  `RotateScale` catalog-allowlist coverage while preserving baseline,
  evidence-gate, status, packet, artifact, and public-reason regressions.

`agents/openai.yaml` was not changed; explicit-only invocation is preserved.

## Resource identity

| Resource | Bytes | SHA-256 |
| --- | ---: | --- |
| `SKILL.md` | 19,492 | `4E87570693EA5903D75059E19260D8750A8D30EA9146B3170B9173888647E42E` |
| `references/recipe-xml-handoff-contract.md` | 11,905 | `714F9B3AE20FE8652117DFFE3AD34CCBC28D6B6B2385FECA638904159F757A57` |
| `scripts/validate_recipe_xml_handoff.py` | 59,488 | `9994A4C8042689B2A68130DFE66FCF35C94DD19326FF8ADB9B24277A8B58BC81` |
| `scripts/test_recipe_xml_handoff.py` | 31,814 | `C68186018FA95C9EED279F81F951F406F7FC40C0034191B25FC3414BC5B2C01F` |
| `agents/openai.yaml` | 350 | `E66F0729CD38076507B72A8FD8772DAD925796C6FBA5C01B7E10DFF8783B4C9A` |
| `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v015-forward\forward-probe-v015.json` | 1,384 | `5D4CCB71E212AF1FC08DBD0A85F0DF4C77FF13DC5DAD2C417FD1ED2980F69F46` |

## Verification evidence

| Check | Command/result |
| --- | --- |
| Skill Creator validation | `python -B C:\Users\USER\.codex\skills\.system\skill-creator\scripts\quick_validate.py C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff` → `Skill is valid!` |
| Focused regression | `python -B C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\scripts\test_recipe_xml_handoff.py` → 15 tests, `OK` |
| Python compilation | `python -B -m py_compile ...validate_recipe_xml_handoff.py ...test_recipe_xml_handoff.py` → exit `0` |
| Independent forward probe | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v015-forward\forward-probe-v015.json` → fresh positive `MEASURE_ONLY/FILE/PASS` and missing-upstream `WAIT/NOT_EMITTED/NOT_RUN`, both with no product action |

The forward probe constructs fresh handoff inputs independently of the
regression fixture and does not claim the deferred benchmark or human
superiority. The benchmark and activation boundaries remain unchanged.

## Governance synchronization

After the installed resources and focused tests changed, the candidate
registry, current handoff, research plan, work contract, documentation map,
and LLM document index were synchronized to this report. Registry status and
dispatch are unchanged: the candidate remains explicit-only, outside normal
dispatch, inactive, and unqualified. The v0.1.4 benchmark and triage reports
remain immutable historical evidence.

## Closure record

Status: **Complete**  
Scope: candidate-only v0.1.5 implementation of C1–C6, focused regression,
independent positive/blocked forward validation, and Dev governance sync.  
Acceptance criteria: candidate resources implement the approved C1–C6 scope
without a new public schema, Tool family, hidden case table, or product
action; missing/failed finalization, owner/frame projection, catalog parameter
drift, baseline preservation, status/reason vocabulary, and forward no-action
boundaries have evidence-backed checks.  
Verification: Skill Creator validation, 15 standard-library regression tests,
Python compilation, independent forward probe, registry validation/regression,
documentation-index validation, JSON parse, scoped `git diff --check`, and
final resource hash review pass in the final command batch.  
Evidence: this report and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v015-forward\forward-probe-v015.json`.  
Boundary / next dependency: no fresh full benchmark, candidate activation,
normal dispatch, product qualification, release, deployment, or
Original-repository work is claimed. A new benchmark requires separate
operator admission and a new identity.
