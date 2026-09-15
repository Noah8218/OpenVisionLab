# OpenVisionLab Matching Skill Specialization

Date: 2026-08-30 KST  
Repository: `C:\Git\OpenVisionLab_Dev`  
Status: **Complete — responsibility split and declared validation passed**

> Path note: the repository was moved later on 2026-08-30, without content
> change, to `C:\Git\2D\Dev`. Commands below preserve their original execution
> context; use the new root for reruns. See
> `docs/reports/OPENVISIONLAB_DEV_REPOSITORY_PATH_MIGRATION_20260830.md`.

## Scope

This change applies the bounded skill-organization lessons reviewed from
`https://www.youtube.com/watch?v=0qySk1fcf6k` to the personal OpenVisionLab
Codex skills. The source video's team simulation is treated as a design prompt,
not quantitative product evidence.

The approved scope is:

- retain `openvisionlab-rule-based-teaching` as the multi-stage composition and
  common evidence owner;
- create `openvisionlab-matching-teaching` as the specialist owner for Matching
  registration, physical correspondence, finite global selection,
  detection-power, Takt study, and manifest validation;
- add a repo-local machine-readable registry with owner, version, lifecycle,
  dependencies, permissions, compatibility authority, and evaluation checks;
- add a deterministic registry validator;
- validate only after all of the above edits are complete.

Blob and Contour/Line skills, product algorithms, WPF UI, Recipe/Pipeline
runtime behavior, original-repository promotion, release, and deployment are
outside this change.

## Responsibility change

### Before

`openvisionlab-rule-based-teaching` owned both generic rule-based composition
and all Matching-specific references/scripts. Its general machine-readable
packet also contained template registration, correspondence, automatic
selection, and Takt fields.

### After

- `openvisionlab-rule-based-teaching`
  - owns intent, datum, supported Tool-chain selection, ROI/frame composition,
    stage ordering, and the generic stage-packet envelope;
  - routes primary Matching work to `$openvisionlab-matching-teaching`;
  - links specialist packets by path/hash rather than duplicating their fields.
- `openvisionlab-matching-teaching`
  - owns Matching/EdgeBasedMatching registration, template context selection,
    physical correspondence, global candidate selection, detection-power and
    Takt studies, the Matching packet, and registration validators;
  - publishes locator pose/frame/transform evidence for downstream composition;
  - does not choose Blob, Contour, or Line policy.

The intended downstream direction is:

```text
Matching packet
  -> reviewed normalization / LocatorFrame
  -> fixed reference-coordinate ROI
  -> Threshold/Morphology
  -> Blob
  -> Contour/Line
```

Downstream stages may consume the locator frame but may not reteach the
Matching template, move its reference ROI per image, or hide an upstream
`WAIT`/`FAIL` state.

## Changed artifacts before validation

- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\SKILL.md`
- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\agents\openai.yaml`
- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\references\detection-point-contract.md`
- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\references\evidence-packet-contract.md`
- `C:\Users\USER\.codex\skills\openvisionlab-matching-teaching\SKILL.md`
- `C:\Users\USER\.codex\skills\openvisionlab-matching-teaching\agents\openai.yaml`
- Matching specialist `references\` and `scripts\`
- `docs\contracts\openvisionlab\OPENVISIONLAB_RULE_BASED_SKILL_REGISTRY.json`
- `docs\LLM_DOCUMENT_INDEX.json`
- this report

Refactor-plan evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\matching-skill-governance-20260830\refactor-proof-plan.md`

## Structural proof

- The general skill now contains only its entrypoint, UI metadata,
  `detection-point-contract.md`, and the generic composition envelope.
- Its `SKILL.md` has no route to a Matching-specific registration, visual
  correspondence, automatic selection, detection-power, Takt, or manifest
  validator resource.
- The specialist skill owns seven Matching references and three deterministic
  scripts, including the registry and registration validators.
- The registry validator resolved both unique skill IDs, every listed resource,
  dispatch target, owner role, SemVer, lifecycle state, dependency/composition
  target, repository-authority path, and fail-closed permission.
- The specialist publishes `openvisionlab-matching-evidence-v1`; the general
  skill publishes `openvisionlab-rule-based-teaching-envelope-v1` and links the
  specialist packet instead of duplicating Matching fields.

Key SHA-256 values after the split:

```text
7A6154209E693A4F94A8AC13686B9ABFEC4004B21CB825F75CCB27CE4DA716B8  openvisionlab-rule-based-teaching/SKILL.md
A749B40EA16B25F70303C5BF499B8BDA39187E298A5FD8F0C82D500618931AF4  openvisionlab-rule-based-teaching/references/evidence-packet-contract.md
009CAA74006D9072287925DAC0D8C1B9093093E9E67FDF8439D0B8EC4623E899  openvisionlab-matching-teaching/SKILL.md
226FBB7C517C3CF03A71386E35A3E18A2FD86EF0D7E86710F2EB7ED2CC313104  openvisionlab-matching-teaching/references/matching-evidence-packet-contract.md
7B11EA3E47E745DE2BCB90ADAA1E8D103CDB2A5DDBD460E8C7A5264E97960F2D  openvisionlab-matching-teaching/scripts/validate_skill_registry.py
D5FB7D7DC8664CCB8814E08915207F7A596EF877B93CCBB21EC21AE90ECCE6E7  OPENVISIONLAB_RULE_BASED_SKILL_REGISTRY.json
```

## Verification

All semantic Skill and registry edits were complete before the first validator
was run. Test-process `TEMP`/`TMP` paths were routed under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\matching-skill-governance-20260830`.

```text
python C:\Users\USER\.codex\skills\.system\skill-creator\scripts\quick_validate.py C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching
=> PASS: Skill is valid!

python C:\Users\USER\.codex\skills\.system\skill-creator\scripts\quick_validate.py C:\Users\USER\.codex\skills\openvisionlab-matching-teaching
=> PASS: Skill is valid!

python C:\Users\USER\.codex\skills\openvisionlab-matching-teaching\scripts\validate_skill_registry.py C:\Git\OpenVisionLab_Dev\docs\contracts\openvisionlab\OPENVISIONLAB_RULE_BASED_SKILL_REGISTRY.json C:\Users\USER\.codex\skills C:\Git\OpenVisionLab_Dev --json
=> PASS, errors=[]

python C:\Users\USER\.codex\skills\openvisionlab-matching-teaching\scripts\test_template_registration_manifest.py
=> PASS: template registration manifest regression cases

powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestDocumentationIndex.ps1
=> PASS: IndexedPaths=107, Routes=12, RootRedirects=102

dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- C:\Git\OpenVisionLab_Dev
=> PASS: all 13 readiness contracts OK

git diff --check -- docs\LLM_DOCUMENT_INDEX.json docs\contracts\openvisionlab\OPENVISIONLAB_RULE_BASED_SKILL_REGISTRY.json docs\reports\OPENVISIONLAB_MATCHING_SKILL_SPECIALIZATION_20260830.md
=> PASS; only the existing non-blocking LF-to-CRLF working-copy warning was emitted
```

## Boundary

- `openvisionlab-matching-teaching` remains lifecycle state `candidate` until a
  fresh real Matching request supplies independent behavioral-forward evidence;
  structural validation does not prove every future model invocation.
- No Blob or Contour/Line skill was created. Their input contract starts only
  after the Matching packet is frozen and reviewed.
- No OpenVisionLab runtime, WPF UI, Recipe/Pipeline behavior, external provider,
  original repository, push, release, or deployment was changed.
- Existing unrelated dirty product-code files in the Dev worktree were
  preserved and not edited by this task.

## Closure record

Status: **Complete**  
Scope: general teaching orchestrator plus specialized Matching Skill, versioned packet boundary, repo-local registry, and deterministic validators  
Acceptance criteria: responsibility moved -> PASS; old source ownership removed -> PASS; registry/resources/permissions/dependencies -> PASS; registration regression -> PASS; documentation/readiness -> PASS  
Verification: exact commands and results are listed above  
Evidence: this report and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\matching-skill-governance-20260830`  
Boundary / next dependency: fresh Matching behavioral use before promoting the specialist from `candidate`; Blob then Contour/Line remain later separately admitted skills
