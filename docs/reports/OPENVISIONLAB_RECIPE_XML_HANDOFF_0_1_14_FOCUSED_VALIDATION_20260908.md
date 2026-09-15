# Recipe XML Handoff 0.1.14 Focused Validation

Date: 2026-09-08 KST

Status: Complete — the bounded 0.1.14 correction passed 35 focused tests,
13 independent probes, Skill Creator, registry and documentation-index checks.

## Completed implementation scope

Input: installed candidate `0.1.13`, general teaching `1.0.1`, Matching `0.2.1`;
Dev HEAD `d875559577c85984d54900df973a6fb35fb20146` with pre-existing worktree
changes. The user's latest decision deferred full benchmarking in favor of
other skill development. This correction advances only the XML candidate to
`0.1.14`, preserving the v1 schema and explicit-only invocation.

The validator now reads root and linked JSON through the same standard-library
decoder. It rejects duplicate object keys, NaN/Infinity and overflow-to-infinity
numbers. Invalid JSON/UTF-8 produces structured `E_PARSE` or `E_JSON_PARSE`
diagnostics. A matching file hash cannot bypass content parsing.

After reading an actual upstream envelope, a nonblocked handoff is rejected
when the envelope is WAIT/REJECTED or a stage/gate is WAIT/FAIL. The existing
immutable-baseline preservation boundary and public reason precedence remain
unchanged. NOT_REVIEWED alone does not invalidate a static measurement plan.

Changed installed files under
`C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff`:

- `scripts/validate_recipe_xml_handoff.py`: common decoder and retained-state gate.
- `scripts/test_recipe_xml_handoff.py`: six focused regression methods and version alignment.
- `SKILL.md`: version, input gates and exact static/runtime boundary.
- `references/recipe-xml-handoff-contract.md`: corresponding input/state contract.

No dependency, public schema field or public decision-reason code was added.
The existing `agents/openai.yaml` invocation policy is preserved.

## Observed before/after behavior

| Probe | 0.1.13 retained result | 0.1.14 current result |
| --- | --- | --- |
| Valid measurement control | CLI exit 0 | CLI exit 0 |
| Duplicate product action (`run: true` followed by `run: false`) | CLI exit 0 | CLI exit 1, structured E_PARSE |
| NaN in unknowns | CLI exit 0 | CLI exit 1, structured E_PARSE |
| Numeric overflow `1e999` | CLI exit 0 | CLI exit 1, structured E_PARSE |
| Malformed UTF-8 | CLI exit 1 with uncaught traceback | CLI exit 1, structured E_PARSE, no traceback |
| Retained envelope WAIT | No validation errors | E_UPSTREAM_BLOCKING_STATE |
| Retained stage FAIL | No validation errors | E_UPSTREAM_BLOCKING_STATE |
| Retained gate WAIT | No validation errors | E_UPSTREAM_BLOCKING_STATE |

The state probes recalculate valid upstream hashes. They test actual retained
content rather than relying only on the summary's status. The regression suite
also covers the corresponding blocked handoffs, PROPOSED drafts, stage WAIT,
gate FAIL, envelope REJECTED, nested duplicate keys, Infinity/-Infinity,
positive/negative numeric overflow, valid finite values, BOM input and
hash-valid duplicated upstream authority.

## Verification

All generated fixtures, copies, logs and temporary files are on D:. Test-process
TEMP/TMP and OPENVISIONLAB_TEST_ROOT are routed under the evidence root.

Commands actually run (PowerShell, from `C:\Git\2D\Dev`):

```powershell
$evidenceRoot = 'D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-static-hardening-20260908'
$env:TEMP = "$evidenceRoot\temp"
$env:TMP = $env:TEMP
$env:OPENVISIONLAB_TEST_ROOT = "$evidenceRoot\tests"
$env:PYTHONDONTWRITEBYTECODE = '1'
python -X utf8 -B "$evidenceRoot\reproduce_before.py"
python -X utf8 -B 'C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\scripts\test_recipe_xml_handoff.py' -v
python -X utf8 -B "$evidenceRoot\reproduce_after.py"
python -X utf8 -B "$evidenceRoot\independent_probe_after.py"
python -X utf8 -B 'C:\Users\USER\.codex\skills\.system\skill-creator\scripts\quick_validate.py' 'C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff'
python -X utf8 -B 'C:\Users\USER\.codex\skills\openvisionlab-matching-teaching\scripts\validate_skill_registry.py' 'C:\Git\2D\Dev\docs\contracts\openvisionlab\OPENVISIONLAB_RULE_BASED_SKILL_REGISTRY.json' 'C:\Users\USER\.codex\skills' 'C:\Git\2D\Dev' --json
powershell -NoProfile -ExecutionPolicy Bypass -File 'C:\Git\2D\Dev\tools\TestDocumentationIndex.ps1' -RepoRoot 'C:\Git\2D\Dev'
```

Results: before reproduction retained; **35 tests passed**; after reproduction
rejects all seven selected invalid cases and preserves the positive control.
The 35 tests include existing reviewed-upstream, draft/measurement, immutable
baseline and public-reason regressions. They do not establish a new full
behavioral benchmark score.

Independent review found that the baseline-preservation path skipped upstream
JSON parsing. That gap was corrected and covered by a new regression before
closure. The independent final 13-case probe verifies the three reported
baseline parsing failures, a legacy baseline positive control, escaped
duplicate keys, finite-number boundaries, upstream blocking states, authority
duplicates, catalog overflow and malformed specialist JSON evidence. Original
review evidence remains retained. All actual JSON-loading paths use the strict
reader; this does not recursively reinterpret arbitrary opaque file references.

Skill Creator and registry validation passed. Documentation index passed with
194 indexed paths, 13 routes and 102 root redirects.
The final comparison detected a concurrent addition of
`OPENVISIONLAB_LEARN_BINARY_LINE_BATCH_20260908.md` to the separate structure
route. That addition was retained; this task changed only the skill-governance
route. No exclusive-workspace or full-repository immutability claim is made.

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-static-hardening-20260908`.

- `before/candidate/` and `before-candidate-manifest.json`: exact source snapshot.
- `reproduction-before.json` / `reproduction-after.json`: actual CLI output and state probes.
- `focused-tests.log`: initial 34-test checkpoint.
- `focused-tests-final.log`: final 35-test result after the independent finding was fixed.
- `independent-review.md` / `independent-review-after.md`: finding and closure.
- `independent-probe-results-after.json`: 13 independent cases passed.
- `skill-format.log`, `registry-validation.log`, `documentation-index.log`: structural checks.
- `final-evidence.json`: source hashes, before/after comparisons and closure gates.
- `work-contract.json`: bounded acceptance criteria and exclusions.

## Boundary and follow-on work

Worklist:
`docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_STATIC_DEVELOPMENT_20260908.md`.
Stage-to-Tool layer/frame binding and operator-selection evidence remain queued
and are excluded from this correction's completion criteria. A stage/Tool
one-to-one invariant has not been established and must not be invented.
Automatic registration of those queued items was prevented by existing
PL-0010/PL-0011 ledger validation errors; no new ID was issued and `.proofline`
was not changed. The worklist retains them; `ledger-registration-note.md`
records the exact CLI errors. This registration limitation does not undo the
implemented and verified priorities 1 and 2.

No product EXE, product build, UI smoke, full benchmark, Author/Reviewer run,
candidate activation, runtime qualification, Original mutation, commit or push
was performed in this batch. The two 0.1.13 interrupted identities remain
historical evidence; their latest report remains the registry benchmark link.
The candidate remains inactive and unqualified for production use.
