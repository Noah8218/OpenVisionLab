# Recipe XML Handoff 0.1.15 — Stage Route Validation

Date: 2026-09-08 KST

Status: Complete — 43 focused tests, 24 independent contract cases, Skill
Creator, registry and documentation-index checks passed.

## Scope and contract

The user's continuation request advances worklist priority 3: stage-to-Tool
layer/frame binding. Input is installed candidate 0.1.14, whose earlier JSON
and upstream-state work remains closed. The installed candidate advances to
0.1.15; teaching 1.0.1, Matching 0.2.1 and the v1 schemas are unchanged.
Dev source HEAD is `d875559577c85984d54900df973a6fb35fb20146`, with existing
unrelated worktree changes.

General teaching SKILL step 6 requires layer/frame/owner information for each
Step; its evidence contract requires downstream consumers to preserve the
upstream frame. Neither establishes a one-stage-per-Tool or positional-array
contract. The XML handoff contract already forbids Tool/layer/frame drift.
The existing XML parser requires non-Main input layers to have earlier
producers. `contract-review.md` retains the independent contract analysis.

The validator now checks the complete primary input order and resolves each
stage by its input/output layer path. Every Tool needs stage coverage. Groups,
interleaved branches, consistent overlaps and reordered stage declarations
remain valid. Unconnected/ambiguous paths, uncovered Tools and contradictory
frames fail closed. Frame-preserving Tools carry frame equality across stage
boundaries; evidence and specialist-owner checks use the actual covered Tools.

No frame name is inferred solely from `FIXTURE_FRAME_NAME`. Arbitrary transform
semantics and secondary-input policy remain with their existing owners. The
immutable-baseline path retains its semantic-preservation and strict JSON
parsing boundary. No public reason code or schema field was added.

## Observed behavior

| Retained probe | 0.1.14 | 0.1.15 |
| --- | --- | --- |
| Valid measurement control | Accepted | Accepted |
| Unequal stage count with wrong Main frame | Incorrectly accepted | Rejected |
| Stage output absent from Tool graph | Incorrectly accepted | Rejected |
| Stage input disconnected from its output | Incorrectly accepted | Rejected |
| Tool plan with no stage coverage | Incorrectly accepted | Rejected |
| Shared layer declared with conflicting frames | Incorrectly accepted | Rejected |
| Grouped path with an interleaved branch | Accepted | Accepted |
| Reordered declarations of a valid transform/measurement pair | Incorrectly rejected | Accepted |

Independent review additionally reproduced forward references, a two-Tool
cycle, missing input and a non-Main self-loop passing an INLINE/PROPOSED
handoff with separate per-Tool stages. The complete Tool-order check now runs
before stage traversal, so these errors cannot wait until XML-file validation.
The additional focused regression passed after the correction.

## Changed files and verification

Installed skill root:
`C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff`.

- `scripts/validate_recipe_xml_handoff.py`: `_validate_stage_routes` replaces
  the equal-count positional check within the existing upstream validator.
- `scripts/test_recipe_xml_handoff.py`: eight regression methods plus version
  alignment; existing baseline, frame, JSON, status and packet cases retained.
- `SKILL.md` and `references/recipe-xml-handoff-contract.md`: version and stage
  path/coverage/frame rules. Invocation policy is unchanged.

Physical evidence and test TEMP/TMP root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-stage-mapping-20260908`.

Commands actually run from `C:\Git\2D\Dev`:

```powershell
$evidenceRoot = 'D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-stage-mapping-20260908'
$env:TEMP = "$evidenceRoot\temp"
$env:TMP = $env:TEMP
$env:OPENVISIONLAB_TEST_ROOT = "$evidenceRoot\tests"
$env:PYTHONDONTWRITEBYTECODE = '1'
python -X utf8 -B "$evidenceRoot\reproduce.py" --phase before
python -X utf8 -B "$evidenceRoot\reproduce.py" --phase after
python -X utf8 -B 'C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\scripts\test_recipe_xml_handoff.py' -v
python -B "$evidenceRoot\independent\review_stage_mapping.py" --label final
python -X utf8 -B 'C:\Users\USER\.codex\skills\.system\skill-creator\scripts\quick_validate.py' 'C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff'
python -X utf8 -B 'C:\Users\USER\.codex\skills\openvisionlab-matching-teaching\scripts\validate_skill_registry.py' 'C:\Git\2D\Dev\docs\contracts\openvisionlab\OPENVISIONLAB_RULE_BASED_SKILL_REGISTRY.json' 'C:\Users\USER\.codex\skills' 'C:\Git\2D\Dev' --json
powershell -NoProfile -ExecutionPolicy Bypass -File 'C:\Git\2D\Dev\tools\TestDocumentationIndex.ps1' -RepoRoot 'C:\Git\2D\Dev'
```

Evidence: `source-manifest.json`, `before/candidate/`,
`reproduction-before.json`, `reproduction-after.json`, `focused-tests.log`
(initial 42 tests), `focused-tests-final.log` (43 tests),
`independent/early-inline-route-results.json`, `contract-review.md`, and
`work-contract.json`. The tests are static Python checks using retained
fixtures; no product runtime was launched.

Final independent evidence is `independent/independent-review.md` and
`independent/final/independent-results.json`: 24/24 expected acceptance or
rejection outcomes passed, including preservation of fixture hashes before
and after validation. A reviewer-created specialist fixture initially omitted
its packet-required normalization path; that fixture was corrected, with the
initial result retained. This was not a validator regression. The final
reviewed validator SHA-256 is
`79ceb543971aecd82a82ff1ae5d62bfebbe4ddec6d05ba8d3be3d8254969e4f4`.

Skill Creator and registry validation passed. Documentation index passed with
196 indexed paths, 13 routes and 102 root redirects. `skill-format.log`,
`registry-validation.log` and `documentation-index.log` retain those results.
`final-evidence.json` and `task-changes.diff` record source identity, scoped
changes, preserved policy/schema and the completion checks. These checks do
not establish an exclusive or immutable full Dev workspace.

## Acceptance and remaining boundary

All five scoped acceptance criteria passed: failures were reproduced; primary
routes and frame constraints are independent of stage count/position; positive
cases remain accepted; focused and independent checks passed; registry,
worklist and navigation are aligned. The work contract links each criterion
to its evidence.

The next skill scope is OPERATOR_SELECTED evidence ownership, still queued in
`docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_STATIC_DEVELOPMENT_20260908.md`.
Recommended model: `gpt-5.6-terra` | Reasoning effort: `medium`.

Full benchmarking remains deferred by the user. This work does not change old
frozen evidence, activate the candidate, qualify inspection accuracy or
runtime UI, launch product EXEs, alter product source, touch Original, or
authorize commit/push/release. The existing ledger registration limitation
remains recorded in the worklist; no unrelated ledger repair was attempted.
