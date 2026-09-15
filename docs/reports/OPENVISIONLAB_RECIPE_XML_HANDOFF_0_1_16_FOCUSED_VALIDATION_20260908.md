# Recipe XML Handoff 0.1.16 — Operator Selection Evidence

Date: 2026-09-08 KST
Status: Complete — all five scoped acceptance criteria and verification gates passed.
Scope: worklist priority 4, installed XML handoff candidate only.
Source: candidate 0.1.15 snapshot; Dev HEAD d875559577c85984d54900df973a6fb35fb20146.

## Behavior and ownership

Fresh OPERATOR_SELECTED observations now require a nonblank statement,
nonempty evidence and exactly one matching OBSERVATION_SELECTION row in a
caller-supplied authority manifest's operatorLocks. The row has exactly
kind/owner/statement/evidence; accepted owners are OPERATOR and OPERATOR_LOCK.
The statement is exact. Evidence path/hash pairs must agree, ignoring order
and using existing path/hash normalization while retaining duplicate counts.
Missing, malformed, mismatched or ambiguous bindings fail closed. Existing
file existence, hash and allowlist checks remain. Malformed operatorLocks
collections and non-string lock kinds return structured errors through the
upstream owner and XML parameter-lock dispatch paths.

The authority remains caller-owned: the handoff consumes it without creating,
approving or rewriting a selection. An image or allowlisted file alone cannot
establish selection ownership. Example: an observation stating `Use the full
image as the inspection region.` needs that exact statement and its retained
request-file evidence in the supplied row. The candidate reference contains
the exact JSON example and compatibility rules.

This intentionally narrows acceptance of fresh informal/unbound claims.
OBSERVED, INFERRED and UNKNOWN retain their existing rules. Immutable-baseline
preservation keeps protected historical semantics and strict JSON parsing.
General envelope v1, public reason vocabulary, existing parameter lock kinds
and explicit-only invocation remain. Static PASS establishes supplied binding
and byte consistency; it does not authenticate human approval, interpret the
truth of arbitrary prose, or qualify inspection accuracy/runtime behavior.

## Reproduction and verification

Eight retained before/after cases include five unsupported claims accepted by
0.1.15 and rejected by 0.1.16: empty evidence, image-only evidence, wrong owner,
wrong statement and mismatched evidence. The explicit-binding and OBSERVED
controls remain accepted. The initial unrelated-lock fixture already failed
because its ROI lock was incomplete; it is retained transparently and is not
counted as a reproduced bypass. Independent probes cover valid unrelated locks.

51 focused regression tests passed, including eight new operator-selection
methods. 40 independent probes passed; final source identity and per-case API/CLI
results are retained in `independent/`. Positive focused tests also verify that
validation does not rewrite the authority, upstream or XML files. Contract review identified no
conflict with the existing authority owner. Independent review also caught
the stale 0.1.15 introductory paragraph, which was corrected to describe 0.1.16.
The initial independent run passed 38/40 cases; array/object lock kinds caused
TypeError in existing lock dispatch. String guards at both dispatch owners and
focused malformed-kind cases correct that failure. The original failing results
are retained alongside the final 40/40 passing run. A valid unrelated
ROI lock independently reproduced the old provenance bypass and now fails with
E_OPERATOR_SELECTION_BINDING; it does not rely on the incomplete initial fixture.
Skill Creator and registry validation passed. Documentation index passed with
197 indexed paths initially. After the concurrent product-document addition,
the final check passed with 198 indexed paths, 13 routes and 102 root redirects.
Both logs are retained.

Commands actually run from `C:\Git\2D\Dev` (TEMP/TMP and all generated test
outputs use `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-operator-evidence-20260908`):

```powershell
python -X utf8 -B 'D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-operator-evidence-20260908\prepare.py'
python -X utf8 -B 'D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-operator-evidence-20260908\reproduce.py' --phase before
python -X utf8 -B 'C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\scripts\test_recipe_xml_handoff.py' -v
python -X utf8 -B 'D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-operator-evidence-20260908\reproduce.py' --phase after
python -X utf8 -B 'C:\Users\USER\.codex\skills\.system\skill-creator\scripts\quick_validate.py' 'C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff'
```

## Changed files and evidence

Installed skill: `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff`.
Changed: SKILL.md, references/recipe-xml-handoff-contract.md,
scripts/validate_recipe_xml_handoff.py and scripts/test_recipe_xml_handoff.py.
The validator owns the observation guard within the existing upstream parser;
no new module, dependency or product component was introduced.

Dev documentation: this report, the static worklist, skill registry, document
index, current handoff skill section and existing skill development contract.
Evidence root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-operator-evidence-20260908`. Retained files include source-manifest.json,
before/candidate, reproduction-before.json, reproduction-after.json,
focused-tests.log, focused-tests-final.log, contract-review.md, independent/,
skill-format.log, skill-format-final.log and
work-contract.json. Final closure records use final-evidence.json and
task-changes.diff; they bind the completed scope to exact source hashes.

Boundary / next dependency: full benchmarking remains deferred by the user;
the candidate stays inactive and unqualified. No product EXE/UI or broad
repository regression was run. Product source, Original, frozen benchmarks,
commit/push/release and unrelated ledger repair are outside this scope.

## Final verification record

Final validator SHA-256:
`ccd1f3c83949b889ae93ae321118856f2e498806a2e7d7bd2188e36508d6129d`.
The 51-test suite was repeated after the malformed-kind correction; final
evidence is focused-tests-final.log. The independent final run is recorded in
independent/independent-review.md and independent/independent-results.json.

Additional commands actually run:

```powershell
python -u -X utf8 -B 'D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-operator-evidence-20260908\independent\probe.py'
python -X utf8 -B 'D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-operator-evidence-20260908\update_docs.py' --independent-cases 40
python -X utf8 -B 'C:\Users\USER\.codex\skills\openvisionlab-matching-teaching\scripts\validate_skill_registry.py' 'C:\Git\2D\Dev\docs\contracts\openvisionlab\OPENVISIONLAB_RULE_BASED_SKILL_REGISTRY.json' 'C:\Users\USER\.codex\skills' 'C:\Git\2D\Dev' --json
powershell -NoProfile -ExecutionPolicy Bypass -File 'C:\Git\2D\Dev\tools\TestDocumentationIndex.ps1' -RepoRoot 'C:\Git\2D\Dev'
python -X utf8 -B 'D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-operator-evidence-20260908\finalize.py'
```

All four selected static priorities are closed. This does not reopen their
completed scopes or resume the separately deferred full qualification gate.

## Shared-document boundary

The first closure audit detected a concurrent Learn Grayscale report added to
the index and a replacement of the Latest Structural Refactoring Closure
section in the shared handoff. Those product updates were preserved. They are
recorded separately in concurrent-document-updates.json and excluded from the
skill-only task-changes.diff; this report does not claim that product work.
The final index rerun is documentation-index-final.log. The initial scope-audit
failure is retained in initial-closure-evidence.json; no candidate source or
test behavior changed for this documentation reconciliation.

Final audit command: `python -X utf8 -B 'D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-operator-evidence-20260908\finalize.py' --audit-only`.
