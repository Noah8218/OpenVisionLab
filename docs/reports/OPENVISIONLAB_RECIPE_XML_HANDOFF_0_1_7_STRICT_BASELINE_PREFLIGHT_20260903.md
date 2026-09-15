# OpenVisionLab Recipe XML Handoff 0.1.7 — Strict Baseline Preflight

Date: 2026-09-03 KST  
Repository: `C:\Git\2D\Dev`  
Status: **Complete for the new corpus identity and preflight; benchmark admission and execution remain separate decisions**

## Scope and boundary

This checkpoint creates a new D-drive-only corpus identity for the installed
candidate `openvisionlab-recipe-xml-handoff 0.1.7` after the reason-code
projection decision. It preserves the historical v0.1.6 root, does not change
the installed skill or product, and does not run Authors, Reviewers, a product
EXE, Import, Preview/Run, Recipe/layer/routing mutation, activation, release,
or deployment.

The new identity is:

```text
Benchmark ID: openvisionlab-rule-based-round1-v017-strictbaseline-20260903
Root: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v017-strictbaseline-20260903
Candidate: openvisionlab-recipe-xml-handoff 0.1.7
Lifecycle: candidate / EXPLICIT_ONLY / inactive / unqualified
```

The public reason projection is recorded separately in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_REASON_PROJECTION_DECISION_20260903.md`.
The public artifact contract uses only the generic v1 vocabulary; the three
historical `AUTO_REJECTED_*` values are projected by evidence pattern and are
not public aliases.

## Corpus changes

- The immutable v0.1.6 `frozen` tree was copied to the new identity; the old
  root was not edited.
- The safe-static baseline is now the local
  `frozen/public/red-fixtures/safe-static-baseline-bundle/positive-v0.1.7`
  bundle. Its 17 evidence entries are exact local `{path, sha256}` pairs.
- Unexecuted baseline stages use the exact tuple
  `packetSchemaId=PENDING`, `packetPath=PENDING`, `packetSha256=UNKNOWN`.
- The nested handoff, upstream envelope, wrapper, static-validator output,
  public manifest, hidden answer key, freeze record, and case references are
  internally linked and hash-checked.
- The copied author/reviewer protocols and corrected runner/scorer harness are
  pinned to the new benchmark ID and candidate version. `prepare` creates 80
  clear + 32 red-team = 112 immutable attempt metadata records only; no author
  artifacts were produced.

## Machine-readable evidence

The complete preflight record is:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v017-strictbaseline-20260903\preflight\strictbaseline-preflight.json`

Its SHA-256 is
`194BE1C5E27FF19D27A83D5ABC6B07AC4B68642FDE9592F39597F1F607C0B775`.
The baseline validator result is retained at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v017-strictbaseline-20260903\preflight\baseline-validator-result.json`
and reports `status=PASS`,
`skillVersion=0.1.7`, and an empty error list.

Key frozen hashes:

| Artifact | SHA-256 |
| --- | --- |
| `frozen/public/public-manifest.json` | `8BE8841F2989958C0EA6503BF001B264F755A053E26C13386622ABF3A35D1CE4` |
| `frozen/private/hidden-answer-key.json` | `063782C13FE05228D592969E565FAE6A90A1460E1011D237D5F5E6CA01614E07` |
| `frozen/private/freeze-record.json` | `304C09CE10A361D9549DD9199FBA28F32E4C99BD1A208DADE3F548709D9FDEF2` |
| `frozen/public/red-fixtures/safe-static-baseline.json` | `540845F4ED9CAD45EF9F629E69E22AF9501CF6ECB4DD4E42D07718BA79D38E9F` |
| local `positive-v0.1.7/handoff.json` | `41CD7941073C5D1B7FA121C3D7F0B2ED9F1C96536D1456C9AF23573D0D029F9E` |
| local `positive-v0.1.7/upstream-envelope.json` | `5EDB936D32BDEF05D69BA0FF8FA96E3B880644900A7B35C0611DA6B7386A52D8` |
| local `particle-count-measure-only.pipeline.xml` | `722AAF92DAC8B812A88E564C5F19496DC8FA7EA589A10B0855EADCC861B7C2DC` |

## Acceptance and verification

| Criterion | Evidence and result |
| --- | --- |
| New identity is separate from the historical root | D-drive root exists; old v0.1.6 root remains immutable; no C:\Git\2D\Original mutation |
| Generic reason projection is applied without public aliases | `reason-projection.json`, public artifact contract, private expected reasons; RT22/RT31/RT32 project to `PER_IMAGE_OVERRIDE_FORBIDDEN`, `UPSTREAM_GRAPH_REVIEW_REQUIRED`, `PER_IMAGE_OVERRIDE_FORBIDDEN` |
| Freeze, contract, and recursive case references are valid | `validate_freeze_record`, `load_contract`, and all 16 clear + 32 red case references: `freezeIssues=[]`, `contractIssues=[]`, `caseIssues=[]` |
| Baseline handoff is accepted directly and against the wrapper | `validate_recipe_xml_handoff.py --json` and `--baseline-handoff`: both exit `0`, `status=PASS`, errors `[]` |
| Static XML compatibility remains valid | `RecipeXmlCompatibilityCheck.exe <assembly-dir> <scan-root>`: 13 XML roots and 1 recipe XML, exit `0` |
| Corrected protocol remains regression-safe | 15 harness tests `OK`, 1 runner test `OK`, Python compilation exit `0`, harness `self-test` `PASS` |
| Attempt setup is deterministic but not an execution claim | harness `prepare`: 112 immutable metadata records; no Authors/Reviewers run |

## Closure record

Status: **Complete**  
Scope: reason-projection application, v0.1.7 strict-baseline corpus identity,
transitive hash freeze, and read-only preflight.  
Acceptance criteria: new ID, generic public reasons, exact local baseline
evidence, valid freeze/contract/case references, direct/baseline validator
PASS, static compatibility PASS, and corrected harness checks — **met**.  
Verification: D-drive preflight JSON, validator outputs, static compatibility
output, 15+1 protocol tests, compilation, self-test, and prepare evidence.  
Evidence: the D-drive root above and this report.  
Boundary / next dependency: this does not admit or execute the `112 + 16`
benchmark, promote or activate the candidate, qualify a Recipe, change the
product, or authorize Original-repository, commit, push, release, or
deployment work. A separate benchmark admission decision is required.
