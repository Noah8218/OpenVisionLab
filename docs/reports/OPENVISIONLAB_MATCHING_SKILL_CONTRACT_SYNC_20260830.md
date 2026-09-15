# OpenVisionLab Matching Skill v0.2.1 Contract Sync

Date: 2026-08-30 KST  
Repository: `C:\Git\2D\Dev`  
Status: **Complete — machine-readable contracts synchronized and independently exercised**

## Approval and scope

The operator explicitly approved the proposed
`openvisionlab-matching-teaching v0.2.1` contract synchronization after its
name, version, exact changes, checks, and Dev/Original boundary were presented.

- Candidate: `openvisionlab-matching-teaching machine-readable contract sync`
- Trigger: one-time manual update; no schedule or automatic evolution
- Inputs: the installed Matching skill, its registered resources, the Dev skill
  registry, current handoff, and documentation index
- Writes: the five changed skill resources, the Dev registry, this report,
  current handoff, documentation map, and existing `llm_xml_maintenance` route
- Checks: skill quick validation, packet/registry regression, registration
  manifest regression, registry validation, independent behavioral-forward
  evaluation, documentation index validation, and scoped diff validation
- Stop condition: every approved check passes; otherwise keep the change
  incomplete and report the exact failure
- Report format: this dated completion record
- Risk controls: no Preview/Run, Recipe/XML or product source mutation, runtime
  candidate selection, network update, Original work, commit, push, release, or
  deployment

## Root cause and correction

The active registry named Matching skill version `0.2.0`, while the
machine-readable evidence-packet example still emitted `skillVersion: 0.1.0`.
The minimum registration-manifest example also omitted four top-level fields
that its validator requires: `referenceSource`, `referenceSourceSha256`,
`selectedRoi`, and `coordinateFrame`.

The approved minimal correction:

- advances the active skill version to `0.2.1`;
- emits `skillVersion: 0.2.1` in the Matching packet contract;
- adds the four required top-level manifest fields and keeps them equal to the
  nested reference-lock and registration values;
- makes the existing registry validator fail when the packet declares zero,
  multiple, or a different `skillVersion`;
- adds one standard-library regression script for the match, mismatch,
  missing, and duplicate cases; and
- keeps the evidence schema ID, skill routing, permissions, registration
  validator, product code, and runtime behavior unchanged.

## Independent behavioral-forward evaluation

An independent agent received a fresh technician request for a machine-readable
`REGISTRATION_PREVIEW` packet and a separate registration manifest. It was not
given the suspected defect or intended response and was prohibited from file
writes and product execution.

Result: **PASS**.

- packet `skillVersion` was `0.2.1`;
- the separate manifest contained `referenceSource`,
  `referenceSourceSha256`, `selectedRoi`, and `coordinateFrame`;
- unverified files, hashes, template reproduction, and visual correspondence
  remained visible as `WAIT`/`NOT_REVIEWED` with `qualification=false`; and
- no Preview/Run, Recipe/XML mutation, file write, or other side effect occurred.

This proves the changed machine-readable contract path only. It does not prove
physical correspondence, execution, held-out performance, production
qualification, release, or deployment.

## Verification

Commands executed with test `TEMP` and `TMP` routed to
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\matching-skill-v0.2.1\temp`:

```text
python -B C:\Users\USER\.codex\skills\.system\skill-creator\scripts\quick_validate.py C:\Users\USER\.codex\skills\openvisionlab-matching-teaching
python -B C:\Users\USER\.codex\skills\openvisionlab-matching-teaching\scripts\test_skill_registry.py
python -B C:\Users\USER\.codex\skills\openvisionlab-matching-teaching\scripts\test_template_registration_manifest.py
python -B C:\Users\USER\.codex\skills\openvisionlab-matching-teaching\scripts\validate_skill_registry.py C:\Git\2D\Dev\docs\contracts\openvisionlab\OPENVISIONLAB_RULE_BASED_SKILL_REGISTRY.json C:\Users\USER\.codex\skills C:\Git\2D\Dev --json
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestDocumentationIndex.ps1
git diff --check -- docs\LLM_DOCUMENT_INDEX.json docs\admin\OPENVISIONLAB_CURRENT_HANDOFF.md docs\admin\OPENVISIONLAB_DOCUMENTATION_MAP.md docs\contracts\openvisionlab\OPENVISIONLAB_RULE_BASED_SKILL_REGISTRY.json docs\reports\OPENVISIONLAB_MATCHING_SKILL_CONTRACT_SYNC_20260830.md
```

Results:

- Skill quick validation: `Skill is valid!`
- Packet/registry regression: `PASS` for matching, mismatch, missing, and
  duplicate-version cases
- Minimum registration-manifest example alignment and manifest regression:
  `PASS`
- Registry validation: `PASS`, `errors=[]`
- Independent behavioral-forward evaluation: `PASS`
- Documentation index: `PASS`, 109 indexed paths, 12 routes, 102 root redirects
- Scoped `git diff --check`: exit code 0; only existing LF-to-CRLF working-copy
  warnings were emitted

## Artifact SHA-256

```text
3D7643F9EF3CB90DFB751DF166C6BB3428E8160AAA4E3DC5FBC402C5BC7841E6  openvisionlab-matching-teaching/SKILL.md
89D141A4F17E936642592A30407B6B2DAD7286A759F1541A0956188320677803  openvisionlab-matching-teaching/references/matching-evidence-packet-contract.md
E502F2056E32E589AEC31FA7E987C8BB356A418088B09359ECB3ACA8C51D5C9F  openvisionlab-matching-teaching/references/template-registration-contract.md
EF2F651A2976B38F2A7D7A56DF1F34AF1A629C03A0B14293D6867748ABF62590  openvisionlab-matching-teaching/scripts/validate_skill_registry.py
E95887ED324F01E4604EF87A7FEFC5385D737C8F4370EEE4941E61DF6C89EA07  openvisionlab-matching-teaching/scripts/test_skill_registry.py
5BE91A99A4F4F9E4692C79A645658ECD2907A04158D37A623040F7337B33A359  docs/contracts/openvisionlab/OPENVISIONLAB_RULE_BASED_SKILL_REGISTRY.json
```

## Closure record

Status: **Complete**  
Scope: active Matching skill `0.2.1` machine-readable packet, registration
example, and registry consistency guard only  
Acceptance criteria: packet/registry version agreement -> PASS; minimum
manifest required fields -> PASS; stale/missing/duplicate packet version fails
closed -> PASS; independent forward response -> PASS  
Verification: focused skill, regression, registry, behavioral, documentation,
and diff checks listed above  
Evidence: this report, installed skill resources, and Dev registry  
Boundary / next dependency: PCB Takt acceptance remains waiting for an actual
production scope/budget/measurement contract; physical, held-out, production,
release, and deployment qualification remain separate
