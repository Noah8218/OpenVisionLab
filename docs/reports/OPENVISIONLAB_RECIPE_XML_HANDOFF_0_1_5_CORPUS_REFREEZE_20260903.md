# OpenVisionLab Recipe XML Handoff 0.1.5 Corpus Re-freeze

Date: 2026-09-03 KST
Repository: `C:\Git\2D\Dev`
Status: **Complete — Gate A corpus re-freeze only; candidate correction and benchmark rerun remain pending**

## Scope

This record closes the corpus-repair gate identified by the v0.1.5 Round 1
failure triage. It creates a new benchmark identity without changing the
original v0.1.5 root, the installed candidate, the product, or the Original
repository.

- Benchmark: `openvisionlab-rule-based-round1-v015-corpusfix-20260903`
- Physical root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v015-corpusfix-20260903`
- Candidate under evaluation: `openvisionlab-recipe-xml-handoff` `0.1.5`
- Corpus class: `EXPOSED_SELECTION_REGRESSION_ONLY`
- Frozen execution shape: 16 clear cases x 5 attempts plus 32 red-team cases =
  112 expected attempts

The new root copied public contracts, private answer/design/protocol inputs,
harness/runner source, and the audit monitor source. Prior attempts, reviews,
scores, audit runs, and the old freeze record were not copied. The new audit
state is `NOT_STARTED`.

## Repairs applied

### Matching dependency graph

The copied path identity changed the bytes, so all transitive references were
recomputed and propagated:

| Artifact | SHA-256 |
| --- | --- |
| `registration-manifest.json` | `89C8956B688515A86429D0FC2927F8EAC3D2D2066DA09362198991EF67031191` |
| `operator-lock.json` | `49794E06B6E01C335F3CB714306C5D4C27C5B888CF5DB19EA88D12D0A65997FC` |
| `matching-packet.json` | `DE8D748F1E1E5EFFB8F1184186EE6D1670537B08A11E77F65A4B6956916DD112` |

The operator lock now points to the actual registration-manifest hash. The
Matching packet points to the actual registration and operator-lock hashes;
S4-01..S4-04 and RT04 point to the actual packet hash. Red fixture hashes that
changed because the new root was embedded in their JSON (RT03 and RT06) were
also recalculated before the public cases were regenerated.

### Safe static baseline

The wrapper now points to one local, self-contained baseline bundle rather than
the prior external positive-v0.1.2 directory. The bundle contains the handoff,
XML, validation report, upstream envelope, and the referenced pixel-analysis
evidence. The wrapper and nested handoff use one canonical local pointer set:

| Pointer | SHA-256 |
| --- | --- |
| `handoff.json` | `9C8F5422E0FF8D59F27EDA1AA3D4E4EA1E80FA175FC889E43D2AAE37E2BB12C5` |
| `particle-count-measure-only.pipeline.xml` | `722AAF92DAC8B812A88E564C5F19496DC8FA7EA589A10B0855EADCC861B7C2DC` |
| `xml-validator-output.txt` | `57B2AC5F03F9D47E38C0BE3C1529BB465B1FCFD0ADD682C4EE882189EAD9CCD5` |
| `upstream-envelope.json` | `C1402B371D821B96BFBFFD3167B2BC6F6599EF03E1EEBCBDD1B58825390F2DAB` |
| `safe-static-baseline.json` wrapper | `B83496F50598EBE046D4F06963FB83A41EEBF75C2A7914B2DED71E96EA474906` |

The nested handoff and wrapper paths/hashes are equal for all three shared
fields (XML, validation report, and upstream). A synthetic v0.1.5 handoff
validated with the unchanged candidate validator and this wrapper as its
baseline passed with no errors.

## Recomputed frozen identity

| Frozen artifact | SHA-256 |
| --- | --- |
| `artifact-contract.json` | `86E0924941DE0535A461D889D17EB17BE571F2658AD1AC2E6CB0C8039E86A093` |
| `author-protocol.json` | `4E9CA0E9EB6A4B674F8942DEB24AC58A8BB655E76B4B804EE904D38958E2E143` |
| `red-author-protocol.json` | `89E202EC73F9D77BFDFEC902CE79D6A58A41602D9BB2691DE6EC5F0F67A5FCDA` |
| `red-team-design.json` | `1DF2AA6450CBFE973DB283D864FCF4EB11B8EA8CA4524B2434182DEE333C7CFD` |
| `public-manifest.json` | `444ACD064B302A771E3FF10DB9095B492D83833470C24EB2FBCD25812B47D1AA` |
| `hidden-answer-key.json` | `3714E778C3205E06D26DAB13E5211BFBCA7DF22A9E2C22DB21F451F5EFDE5DE8` |
| `freeze-record.json` | `4967D81DCCC4E041A00055DA123712FD5D5DA2DDAD1626E17D6EF2808C9AF8DA` |

## Verification evidence

- `build-public-manifest.ps1` produced 16 clear and 32 red case entries.
- `build-freeze-record.ps1` produced a 20-artifact freeze record for the new
  identity.
- Harness `prepare`: **PASS**, 112 immutable attempt metadata records created.
- Harness `self-test`: **PASS**; frozen contract, 112-attempt count, XML
  semantics, blocked/complete result collection, and run-record checks all
  passed.
- `validate_freeze_record`, `load_contract`, and recursive case-file reference
  checks: **PASS** (`freezeIssues=[]`, `contractIssues=[]`, `caseIssues=[]`).
- Direct Matching transitive-reference check: **PASS** (`issues=[]`).
- Safe-baseline wrapper file/hash and nested-pointer check: **PASS** (four
  files present, all declared hashes match, no pointer drift).
- Candidate baseline compatibility probe: **PASS**; unchanged
  `validate_recipe_xml_handoff.py` returned `status=PASS`, `errors=[]`.
- Static compatibility validator against the local baseline bundle: **PASS**,
  exit code `0` (`13 XML roots`, `1 recipe XML`).
- Spot checks preserved the old v0.1.5 Matching packet and baseline wrapper
  hashes, and preserved the candidate SKILL/validator hashes. No old benchmark
  literals remain in the new root.

Reusable evidence is stored at:

```text
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v015-corpusfix-20260903\gate-a-evidence\corpus-integrity.json
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v015-corpusfix-20260903\gate-a-evidence\baseline-validator-result.json
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v015-corpusfix-20260903\gate-a-evidence\static-validator-output.txt
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v015-corpusfix-20260903\harness\self-test-output\self-test-result.json
```

## Boundary and next gate

This is a corpus-only completion. No author attempts, reviewer tasks, product
EXE, Import, Preview, Run, Recipe mutation, layer/routing mutation, candidate
activation, or v0.1.6 installation occurred. The original
`openvisionlab-rule-based-round1-v015-20260902` root remains the immutable
historical result and still records `FAIL`.

The next separately scoped priority is Gate B: candidate-only v0.1.6 work for
C7-C10 (finalization, trust-boundary projection, public routing/baseline
precedence, and focused regression). A new benchmark run remains blocked until
that candidate change is explicitly approved and verified.
