# OpenVisionLab Recipe XML Handoff — Round 1 Runner/Scorer Correction

Date: 2026-09-03 KST  
Repository: `C:\Git\2D\Dev`  
Parent evidence: `OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_TRIAGE_20260903.md`  
Status: **Complete for the isolated runner/scorer correction checkpoint; no new benchmark identity admitted**

## Scope and boundary

This checkpoint corrects two proven benchmark-protocol defects without
rewriting the frozen v0.1.6 corpus, hidden answer key, reason vocabulary, or
candidate skill resources. The work was performed only in the isolated test
copy
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-runner-scorer-fix-20260903`.
The frozen source root
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v016-corpusfix-auditfix2-20260903`
was read for diagnosis and was not modified. No product EXE, Import,
Preview/Run, Recipe/layer/routing mutation, activation, full `112 + 16`
benchmark, Original-repository change, commit, push, release, or deployment
was performed.

## Proven defects and correction

The v0.1.6 audit record contains one invalid static-validator invocation
(event index `72`): the benchmark root was supplied as the validator scan root
instead of the assigned author-attempt root. The scorer already has a strict
`AUDIT_ALLOWED_PROCESS_SCAN_ROOT_INVALID` guard; this correction keeps that
fail-closed rule and fixes the author path that produced the invalid event.

- **Author runner path pinning** (`runner/run_authors.py`): the public protocol
  now supplies the validator executable and assembly directory, states that
  the process working directory and every retry must use the exact assigned
  attempt root, forbids the benchmark/parent/sibling/repository roots and
  relative aliases, and shows the canonical three-argument invocation. The
  Codex subprocess also sets `cwd` to the assigned attempt root while retaining
  the existing `-C` checkout pin.
- **Declared red-team hash mismatch** (`harness/round1_harness.py`): a
  `RED_TEAM` case may declare an intentional mismatch by exact absolute path,
  provided hash, and actual hash. Only that case-declared tuple is exempted
  from `REFERENCED_FILE_HASH_MISMATCH`; clear cases and arbitrary hashes remain
  strict. This preserves the negative-test meaning without making the scorer
  accept an untrusted mismatch.
- **Static-validator cache isolation** (`harness/round1_harness.py`): the
  cache key now includes the normalized scan-root path in addition to the XML,
  executable, and assembly identities, preventing a result from one attempt
  root from being reused for another.
- **Focused harness coverage** (`harness/test_round1_harness.py` and
  `runner/test_run_authors.py`): tests cover exact mismatch scoping, cache
  separation by scan root, rejection of an unassigned scan root, and runner
  prompt/`cwd` pinning. `probe_rt03.py` compares the retained RT03 behavior
  before and after the scorer correction; `probe_audit_scan_root.py` records
  the retained invalid audit event without changing its source.

## Acceptance criteria and evidence

| Criterion | Result and evidence |
| --- | --- |
| The runner pins process `cwd`, prompt instructions, and retries to the assigned attempt root | **PASS** — runner regression test; `evidence/runner-regression.txt` |
| A benchmark-root or otherwise unassigned scan root remains fail-closed | **PASS** — existing scorer guard test and retained-event diagnosis; `evidence/audit-scan-root-diagnosis.json` |
| RT03's intentional mismatch is accepted only for its declared path/provided hash | **PASS** — old result has `REFERENCED_FILE_HASH_MISMATCH`; fixed result is `REJECTED`, `contractPass=true`, `answerPass=true`, with no issues; `evidence/rt03-comparison.json` |
| Static-validator results cannot cross scan-root boundaries through the cache | **PASS** — dedicated harness regression |
| No frozen source root was rewritten | **PASS** — all edits and generated evidence are under the isolated D-drive copy |
| The correction is syntactically and behaviorally regression-safe | **PASS** — 15 harness tests, 1 runner test, and Python compilation all exit `0` |

## Verification

| Check | Command/result |
| --- | --- |
| Harness regression | `python -X utf8 -B -m unittest discover -s "<fixture>\harness" -p 'test_*.py' -v` → 15 tests, `OK`, exit `0`; transcript SHA-256 `CF81E8EE6A5793AD27F2B638C987244DE5465674FE99DC59CA1524AD213D0ADE` |
| Runner regression | `python -X utf8 -B -m unittest discover -s "<fixture>\runner" -p 'test_*.py' -v` → 1 test, `OK`, exit `0`; transcript SHA-256 `274938B1D4326117A27BC5760993C80BFF3A0D2D008DD532132DFDFADD10A9EA` |
| Python compilation | `python -X utf8 -B -m py_compile` on the changed runner/harness modules and probes → exit `0` |
| RT03 comparison probe | `evidence/rt03-comparison.json` → fixed `status=REJECTED`, `contractPass=true`, `answerPass=true`, `issues=[]`; declared mismatch key count `1` |
| Retained audit diagnosis | `evidence/audit-scan-root-diagnosis.json` → exactly one invalid validator event, index `72`; command line contains the frozen benchmark root; `benchmarkRootWasRejected=true` |
| Artifact identity | `evidence/artifact-hashes.json` records byte lengths and SHA-256 for every changed fixture script, regression transcript, and probe output |

## Lifecycle and next dependency

This is a protocol/harness correction, not a new skill version and not a
benchmark admission. The installed `openvisionlab-recipe-xml-handoff 0.1.7`
candidate remains explicit-only, outside normal dispatch, inactive, and
unqualified. The frozen v0.1.6 result remains `INCOMPLETE`; no Reviewer result
can be inferred from this isolated regression.

Before a new benchmark identity can be created, the project still needs an
explicit contract/corpus reconciliation for the frozen artifact-contract
vocabulary and legacy safe-baseline shape. The follow-up preflight did not
reproduce S3-02 projection or S4-01 ratio as mechanical blockers; those cases
must not be rewritten without new semantic evidence. After the owner decision,
the corrected runner/scorer copy must be promoted into the separately reviewed
benchmark setup and rerun under a new identity. Candidate activation, normal
dispatch, Round 2, product execution, qualification, release, deployment, and
Original-repository work remain separate approvals.

## Closure record

Status: **Complete**  
Scope: isolated Round 1 author-runner scan-root pinning, strict scorer handling
for case-declared red-team hash mismatches, scan-root-aware validator caching,
and focused regression evidence.  
Acceptance criteria: all six criteria above — **met**.  
Verification: 15 harness tests, 1 runner test, Python compilation, RT03
before/after probe, retained audit scan-root diagnosis, and artifact hash
manifest — all recorded under the D-drive fixture.  
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-runner-scorer-fix-20260903\evidence`.  
Boundary / next dependency: this does not prove a new `112 + 16` benchmark,
candidate qualification, activation, or product behavior; contract/corpus
reconciliation and a separately authorized new benchmark identity remain
required.
