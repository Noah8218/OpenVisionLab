# OpenVisionLab Recipe XML Handoff 0.1.8 — Strict Baseline Preflight

Date: 2026-09-03 KST  
Repository: `C:\Git\2D\Dev`  
Status: **Complete for the new corpus identity and preflight; Authors/Reviewers admission remains separate**

## Identity

The v0.1.7 failed run remains immutable. This preflight uses a new D-drive-only
identity with the corrected candidate:

```text
Benchmark ID: openvisionlab-rule-based-round1-v018-preflight-20260903-r1
Root: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v018-preflight-20260903-r1
Candidate: openvisionlab-recipe-xml-handoff 0.1.8
Lifecycle: candidate / EXPLICIT_ONLY / inactive / unqualified
```

The previous v0.1.7 result is still `INCOMPLETE` because its metadata captured
the freeze hash before the final freeze rewrite and its Reviewer phase was
guard-blocked. This preflight does not rewrite that root and does not claim that
result was repaired retroactively.

## Freeze-before-prepare gate

The new identity was finalized in this order:

1. Copy the frozen corpus into the new root and update only the new identity.
2. Refresh clear-case protocol hashes and reconcile transitive JSON references.
3. Regenerate `public-manifest.json`, update the hidden manifest reference, and
   write the final `freeze-record.json` once before attempt preparation.
4. Validate the freeze and all 16 clear + 32 red case references.
5. Validate the historical v0.1.7 baseline directly and validate a v0.1.8
   projection against the same protected baseline tuple.
6. Run static XML compatibility and harness/runner regressions.
7. Run `prepare` once and hard-check all 112 metadata records against the final
   freeze hash. No author artifact or review file was created.

## Machine evidence

The complete preflight record is:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v018-preflight-20260903-r1\preflight\strictbaseline-preflight.json`

Its SHA-256 is `390565D466D81077AAAFBBCDD9B834966C411B27A5BFE7205785760494AB2725`.

| Evidence | Result |
| --- | --- |
| `preflight/contract-validation.json` | freeze `PASS`, contract `PASS`, 16 clear + 32 red cases |
| `preflight/baseline-direct-validator-result.json` | historical v0.1.7 handoff `PASS` |
| `preflight/baseline-validator-result.json` | v0.1.8 projection with baseline wrapper `PASS`, errors `[]` |
| `preflight/baseline-static-validator-output.txt` | `RecipeXmlCompatibilityCheck` exit `0`; 13 XML roots, 1 recipe XML |
| `preflight/harness-self-test.txt` | `PASS` |
| `preflight/harness-tests.txt` / `runner-tests.txt` | 15 + 1 tests `OK` |
| `preflight/metadata-freeze-gate.json` | 112/112 metadata hashes match; 80 clear + 32 red; 0 non-metadata attempt files |

Frozen identity hashes:

| Artifact | SHA-256 |
| --- | --- |
| `frozen/public/public-manifest.json` | `DBC2FB39B4DD26DC35BC313E76DD9931C5F5DE3E483AD8AF477013164C8F5A3B` |
| `frozen/private/hidden-answer-key.json` | `DF66681E3B7511BD36497F07F050A273FDC4ECD116AD9550E721EB925151C3A8` |
| `frozen/private/freeze-record.json` | `CBEDBF8491CAAD956A7F41DBBB4FE237CA5935B7EAD079ED5C6953A059CFED70` |
| `frozen/public/author-protocol.json` | `8D585A3BEF3EA1FE71044268D7C883C5D27466F433381521CFC30CE01508AFBA` |
| `frozen/public/red-author-protocol.json` | `A8271C9FC5BA9BFD46BEEB93FD80F9FBC87FC94B7F1280889693DDFAFC84866E` |
| `frozen/public/artifact-contract.json` | `7E12990FB1DF5F7CB0872131F042058C48D194D887990482E1406099D563FB64` |

The retained baseline remains `MEASURE_ONLY`, `qualification=false`, with the
same upstream/XML/validation pointers and 17 evidence files under
`frozen/public/red-fixtures/safe-static-baseline-bundle/positive-v0.1.7`.

## Closure record

Status: **Complete**  
Scope: v0.1.8 strict-baseline corpus identity, transitive hash freeze,
baseline/static preflight, and deterministic metadata preparation.  
Acceptance criteria: new ID, valid freeze/contract/case references, preserved
v0.1.7 baseline, direct and projected validator `PASS`, static compatibility
`PASS`, and final freeze hash on all 112 metadata records — **met**.  
Verification: D-drive preflight JSON and transcripts, 15 harness tests, 1 runner
test, compilation, self-test, and `prepare`; no Authors/Reviewers execution.  
Evidence: the D-drive root and files listed above.  
Boundary / next dependency: the 112 + 16 benchmark is not admitted or run;
candidate activation, product execution, qualification, Original mutation,
commit/push, release, and deployment remain separate approvals.

Next priority: separately admit or decline the v0.1.8 Authors/Reviewers run.  
Recommended model: `gpt-5.6-terra`  | Reasoning effort: `high`.
