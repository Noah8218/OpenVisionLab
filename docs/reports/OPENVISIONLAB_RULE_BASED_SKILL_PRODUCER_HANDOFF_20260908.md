# General Teaching to XML Handoff — Producer Compatibility

Date: 2026-09-08 KST
Status: Complete — producer transfer acceptance and verification passed.
Scope: general teaching 1.0.1 -> 1.0.2 and XML candidate 0.1.16 -> 0.1.17.

## Requirement and resulting behavior

The previous static worklist closed four consumer-validation scopes. A new
continuation request exposed a producer guidance gap: general teaching did not
describe the caller-owned selection binding required by the new 0.1.16 consumer
profile. Its generic envelope example could pair OPERATOR_SELECTED with empty
evidence. This follow-up aligns the existing owners without adding a specialist.

General teaching now conditionally links the canonical XML selection profile
only for explicitly requested XML transfer preparation. It retains exact
operator statements/evidence and records the external authority path/hash in
the companion report. Missing bindings produce a teaching WAIT artifact that
retains actual selected provenance and names the caller-owned dependency.
The skill cannot manufacture authority or evade the gate by relabeling a
selection. This WAIT artifact is not a validated XML input. Ordinary design
does not require an XML authority manifest or invoke the candidate.

The v1 example now uses UNKNOWN with an explicitly unverified statement; its
field structure is unchanged. The candidate accepts reviewed producer 1.0.2
alongside 1.0.0/1.0.1. Future versions remain rejected. All selection, frame,
stage, baseline, schema, reason, product-action and invocation gates remain.

Example: `Use the full image as the inspection region.` with its exact supplied
request evidence and matching authority row remains ready for explicit XML
review. The same request with operatorLocks=[] stays WAIT with OPERATOR_SELECTED
unchanged. A valid file hash alone cannot supply the missing selection binding.

## Verification

- Before: 0.1.16 rejects the proposed producer 1.0.2 version even with a valid
  selection binding. The missing-binding control also reports its binding error.
- After: eight version/binding combinations pass expected outcomes; only reviewed
  versions with valid selection authority are accepted.
- Focused candidate regression: 51 tests pass, including the updated version
  test's binding and per-image-override checks for all three reviewed versions.
- Both Skill Creator format checks pass. Independent source review found no
  required correction; the consumer source equals its snapshot after reversing
  only the declared version constant, allowlist and diagnostic text changes.
- Two fresh independent producer contexts each received the real request, the
  general skill entrypoint and immutable synthetic inputs, without expected
  answers or the change hypothesis. Their outputs retain the supplied plan,
  exact selected observation, external authority and qualification=false. The
  supplied case remains READY_FOR_OPERATOR_REVIEW; missing binding remains WAIT.
  Actual responses and the parent's separate verdict are retained.
- Registry validation passed; documentation index passed with 199 indexed paths, 13 routes and 102 root redirects.

Source changes: general SKILL.md and references/evidence-packet-contract.md;
candidate SKILL.md, references/recipe-xml-handoff-contract.md,
scripts/validate_recipe_xml_handoff.py and scripts/test_recipe_xml_handoff.py.
No new source module or dependency was added. Registry changes are limited to
the two reviewed versions and evaluation metadata; Matching, dispatch and
invocation policies remain.

Evidence root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-producer-handoff-20260908`. Inputs, test outputs and TEMP/TMP are physically on D:.
Source identity: before/skills, source-manifest.json and input-manifest.json.
Checks: compatibility-before.json, compatibility-after.json, focused-tests.log,
general-skill-format.log, candidate-skill-format.log, independent-change-review.md,
forward/, forward-verdict.json, registry-validation.log, documentation-index.log,
work-contract.json, final-evidence.json and task-changes.diff.

Commands run from `C:\Git\2D\Dev`:

```powershell
python -X utf8 -B 'D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-producer-handoff-20260908\prepare.py'
python -X utf8 -B 'C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\scripts\test_recipe_xml_handoff.py' -v
python -X utf8 -B 'C:\Users\USER\.codex\skills\.system\skill-creator\scripts\quick_validate.py' 'C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching'
python -X utf8 -B 'C:\Users\USER\.codex\skills\.system\skill-creator\scripts\quick_validate.py' 'C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff'
python -X utf8 -B 'D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-producer-handoff-20260908\verify.py' --phase compatibility
```

Final metadata and evidence commands are retained in completion-commands.json.
Acceptance: conditional routing, provenance/authority preservation, narrow
version compatibility and required checks are the four scoped criteria in the
work contract. Each criterion links its actual evidence there.

Boundary / next dependency: supplied-authority consistency and synthetic
authoring evidence only, not authentication, prose truth, XML execution or
inspection qualification. Full benchmarking remains deferred and the candidate
inactive/unqualified. Product build/UI/EXE, broad regression, Original, frozen
benchmarks, commit/push/release and new specialist admission were not performed.
