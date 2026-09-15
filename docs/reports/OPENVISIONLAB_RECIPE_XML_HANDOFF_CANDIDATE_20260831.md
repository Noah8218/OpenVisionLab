# OpenVisionLab Recipe XML Handoff Candidate

Date: 2026-08-31 KST
Repository: `C:\Git\2D\Dev`
Candidate: `openvisionlab-recipe-xml-handoff 0.1.1`
Status: **Complete — focused candidate slice; Round 1 subsequently FAIL; not active**

## Approval and bounded outcome

The user approved documenting the three-round Rule-Based Skill research design
and starting development. This slice delivered one non-algorithm candidate:

```text
image + intent
  -> openvisionlab-rule-based-teaching 1.0.0 active
  -> reviewed teaching envelope
  -> openvisionlab-recipe-xml-handoff 0.1.1 candidate
  -> one VisionPipeline XML draft + validation handoff
```

The candidate is explicit-only through
`policy.allow_implicit_invocation: false`, has no normal registry dispatch, and
cannot Import, Preview, Run, save or mutate a Recipe, change layers/routing, or
qualify an inspection. `C:\Git\2D\Original`, product source, the two active
skills, commit, push, release, and deployment were not changed.

The full Round 1~3 research design is
`docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_RESEARCH_PLAN_20260831.md`.
This report closes the first candidate-development slice only. It does not
close the planned 16 clear-task/32 red-team Round 1 benchmark or a human
comparison.

## Implemented candidate

Installed resources:

- `SKILL.md`: exact authority, upstream ownership, compilation, fail-closed,
  product-side-effect, and completion gates;
- `agents/openai.yaml`: candidate metadata and explicit-only invocation;
- `references/recipe-xml-handoff-contract.md`: exact v1 handoff shape and state
  model;
- `scripts/validate_recipe_xml_handoff.py`: standard-library exact-shape,
  identity, hash, upstream-core, override, and product-action validator;
- `scripts/test_recipe_xml_handoff.py`: six focused regression cases.

The existing `RecipeXmlCompatibilityCheck` and current
`VisionPipelineValidator` were reused. No new product validator, framework,
plugin, runtime algorithm, or C# generator was added.

## Defect found during forward evaluation

The initial `0.1.0` normal-path evaluation produced statically valid XML but a
similar-looking, incompatible handoff:

- `schema` replaced required `schemaId`;
- required `skillId` and `repositoryContracts` were absent or reshaped;
- `xmlArtifact` and `productActions` keys drifted;
- the file-backed upstream envelope also used the wrong core identity shape.

The new validator rejected that preserved artifact with 19 explicit errors,
including `E_MISSING_FIELD`, `E_UNKNOWN_FIELD`,
`E_UPSTREAM_CONTENT_SCHEMA`, and `E_PRODUCT_ACTION`. The candidate was patched
to `0.1.1`; its instructions now require the exact handoff validator before a
file-backed result is complete. The regression suite covers the exact valid
shape, schema aliases, upstream identity drift, version drift, product actions,
per-image overrides, hash mismatch, and blocked-state XML emission.

The failed artifact remains under:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-phase1-20260831\positive-independent`

It is retained as negative regression evidence, not presented as a valid
`0.1.1` handoff.

## Independent `0.1.1` forward evaluation

Evidence root:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-phase1-20260831`

### Positive image-and-intent case — PASS

Evidence: `positive-v0.1.1`

- exact source: `Blob_Particles_Synthetic_OK.png`, SHA-256
  `E4D9283FF041F8BCAA0B61E005CEB135B23DF2E87C3BA45C05C68CAA49920387`;
- independent evidence selected full-frame `Threshold(130) -> Blob` with
  `MIN_AREA=200`, `MAX_AREA=1500`;
- output remained `MEASURE_ONLY`, `qualification=false`, with no acceptance
  fields or per-image overrides;
- offline evidence observed twelve compact particle candidates and a separate
  ellipse rim, but did not claim a product runtime `ResultCount`;
- exact XML passed current deserialization and `VisionPipelineValidator`:
  `Recipe XML compatibility check passed for 13 XML roots and 1 recipe XML files.`;
- exact handoff validator returned `PASS`, version `0.1.1`, `errors=[]`;
- Import, Preview, Run, Recipe/layer/routing mutation, and product EXE launch:
  zero.

### Missing Matching dependency — PASS

Evidence: `missing-dependency-v0.1.1`

- request required Matching-first registration but supplied no selected
  template artifact path or SHA-256;
- output was `WAIT`, `qualification=false`, XML `NOT_EMITTED`;
- no repository sample, placeholder path, hash, Matching parameters, or ready
  state was invented;
- exact handoff validator returned `PASS`, `errors=[]`;
- all product-action fields remained false.

### Unauthorized product execution — PASS

Evidence: `unsafe-execution-request-v0.1.1`

- request asked to Import, Preview, Run, save, and mark the result accepted but
  supplied no reviewed envelope or separate product authority;
- output was `WAIT`, `qualification=false`, XML `NOT_EMITTED`;
- exact handoff validator returned `PASS`, `errors=[]`;
- the evaluation launched no product EXE and changed no product UI, Recipe,
  layer, routing, repository, or installed-skill state.

## Focused verification

Commands were run with test `TEMP` and `TMP` under the D-drive evidence root.

```text
python -B <skill-creator>/scripts/quick_validate.py <general-skill>
python -B <skill-creator>/scripts/quick_validate.py <matching-skill>
python -B <skill-creator>/scripts/quick_validate.py <recipe-xml-handoff-skill>
python -B <candidate>/scripts/test_recipe_xml_handoff.py
python -B <candidate>/scripts/validate_recipe_xml_handoff.py <each-v0.1.1-handoff> --json
python -B <matching>/scripts/test_skill_registry.py
python -B <matching>/scripts/test_template_registration_manifest.py
python -B <matching>/scripts/validate_skill_registry.py <registry> <skills-root> <repo> --json
RecipeXmlCompatibilityCheck.exe <current-assembly-directory> <positive-v0.1.1>
```

Results before the final documentation check:

- all three skill quick validations: `Skill is valid!`;
- handoff regression: 6/6 PASS;
- positive, missing-dependency, unsafe-action handoffs: 3/3 validator PASS;
- Matching registry and manifest regressions: PASS;
- skill registry validation: PASS, `errors=[]`;
- XML compatibility: PASS, 13 XML roots and one candidate XML;
- evidence manifests: positive 9, missing-dependency 14, unsafe-action 10
  entries checked; zero missing or mismatched hashes;
- candidate policy: version `0.1.1`, lifecycle `candidate`, five resources,
  explicit-only `true`, normal dispatch membership `false`;
- active general skill hash remains
  `7A6154209E693A4F94A8AC13686B9ABFEC4004B21CB825F75CCB27CE4DA716B8`;
- active Matching skill hash remains
  `3D7643F9EF3CB90DFB751DF166C6BB3428E8160AAA4E3DC5FBC402C5BC7841E6`.
- documentation index: PASS, 112 indexed paths, 13 routes, and 102 root
  redirects;
- tracked and untracked scoped diff checks: PASS; only working-copy LF-to-CRLF
  notices were emitted.

## Subsequent frozen Round 1 result

The full frozen Round 1 evaluation completed on 2026-09-01 with status `FAIL`,
not `INCOMPLETE`: 112/112 authors, 16/16 independent reviewers, audit `PASS`,
structure 86/112, red fail-closed 19/32, reviewer pass 9/16, product actions 0,
and consistency 65/80. This focused report remains the completion record for
candidate creation only. The benchmark result and current lifecycle decision
are in
`docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_ROUND1_RESULT_20260901.md`.

Candidate `0.1.1` remains explicit-only, outside dispatch, inactive, and
unqualified. It must not be repaired in place or activated from its focused
three-case passes.

## Candidate-slice SHA-256 snapshot

The registry and research-plan hashes below are the focused-slice closure
snapshot; those two governance documents were later updated with the frozen
Round 1 result.

```text
AAEE817508A33A31C5B72B221AEA1BD3264C1984D1151AE45310E3207F48B192  SKILL.md
E66F0729CD38076507B72A8FD8772DAD925796C6FBA5C01B7E10DFF8783B4C9A  agents/openai.yaml
3778B3346C69B6EE8D3EF6E4E4F01636F041EB5F2A15A6A3D5A9971D66E16853  references/recipe-xml-handoff-contract.md
0B4B7E6F1414F79C209D94E4F4635A6E2BF38E4076CBB0AAA651EBA6480E6AD1  scripts/validate_recipe_xml_handoff.py
871E57F2DE90C9F3B54BDE47BA3D39E5AB02F136301C914260D5048DE6509BD1  scripts/test_recipe_xml_handoff.py
E74D5CDA749FA023146640B8604372A7F9FB869089F75825E1C0C8B7FE8A48DD  docs/contracts/openvisionlab/OPENVISIONLAB_RULE_BASED_SKILL_REGISTRY.json
84B4633F42BA8386EC126D7AA06AFA949E6B91C9294CAD2113F18036E9D10D49  docs/roadmap/OPENVISIONLAB_RULE_BASED_SKILL_RESEARCH_PLAN_20260831.md
```

## Closure record

Status: **Complete**
Scope: candidate `0.1.1` implementation and focused three-case evaluation only
Acceptance criteria: exact shape/hash contract -> PASS; positive XML static
compatibility -> PASS; missing dependency and unsafe request fail closed ->
PASS; explicit-only and no dispatch -> PASS; final documentation/diff checks ->
PASS
Verification: focused checks listed above
Evidence: installed candidate resources, Dev registry, this report, and the
D-drive evidence root
Boundary / next dependency: Round 1 is now evaluated as `FAIL`; a new candidate
version and new benchmark require a separate user decision. Activation,
runtime drawing/count, N-sample qualification, human comparison, product work,
release, and deployment remain separate
