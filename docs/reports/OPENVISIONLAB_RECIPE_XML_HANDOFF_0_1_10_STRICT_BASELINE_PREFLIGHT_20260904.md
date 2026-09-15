# OpenVisionLab Recipe XML Handoff 0.1.10 — Strict Baseline Preflight

Date: 2026-09-04 KST  
Repository: `C:\Git\2D\Dev`  
Status: **Complete for the new frozen identity and preflight; Authors/Reviewers admission remains separate**

## Identity

The immutable v0.1.8 backend-recovery result remains preserved at its original
root. This preflight uses a separate D-drive identity:

```text
Benchmark ID: openvisionlab-rule-based-round1-v010-contract-recovery-20260904
Root: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v010-contract-recovery-20260904
Candidate: openvisionlab-recipe-xml-handoff 0.1.10
Lifecycle: candidate / EXPLICIT_ONLY / inactive / unqualified
Corpus: 16 clear cases x 5 attempts + 32 red-team cases x 1 attempt = 112
```

No file in the prior `v018-backend-recovery-20260904-r2` root was repaired or
resumed.

## Freeze-before-prepare gate

The new identity was finalized in this order:

1. Copy the frozen corpus and harness into the new identity; change only the
   new benchmark ID, candidate version, and approved public precedence rules.
2. Reconcile the RT12/RT17/RT23/RT30 public expectations and the RT30 fixture
   so an actual upstream `FAIL` is represented as `FAIL` with no fabricated
   downstream pass.
3. Refresh clear-case protocol hashes after the final candidate protocol,
   regenerate `public-manifest.json`, update the hidden manifest reference,
   and write `frozen/private/freeze-record.json` with the literal frozen time
   `2026-09-03T21:51:59.736Z`.
4. Validate the freeze and all 16 clear + 32 red case references, then run the
   harness self-test.
5. Validate the historical v0.1.7 baseline directly with its installed
   snapshot validator and validate a v0.1.10 projection against the protected
   baseline tuple.
6. Run static XML compatibility, harness/runner regressions, and Python
   compilation.
7. Run `prepare` once and verify every metadata record against the final freeze
   hash. No author artifact or review file was created.

## Machine evidence

Evidence root:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v010-contract-recovery-20260904`

| Evidence | Result |
| --- | --- |
| `preflight/contract-validation.json` | `PASS`; freeze issues `[]`, contract issues `[]`; 16 clear + 32 red; expected attempts `112` |
| `harness/self-test-output/self-test-result.json` and `preflight/harness-self-test.txt` | `PASS` |
| `preflight/baseline-direct-validator-output.json` | retained v0.1.7 handoff `PASS`, snapshot validator, exit `0` |
| `preflight/baseline-projection-validator-output.json` | v0.1.10 projection against protected baseline `PASS`, errors `[]`, exit `0` |
| `preflight/baseline-static-validator-output.txt` | `RecipeXmlCompatibilityCheck` `PASS`, 13 XML roots and 1 recipe XML, exit `0` |
| `preflight/harness-tests.txt` / `runner-tests.txt` | 15 + 1 tests `OK`, both exit `0` |
| `preflight/benchmark-pycompile.txt` | 5 benchmark Python files compiled, exit `0` |
| `preflight/prepare.txt` | `Prepared 112 immutable attempt metadata records.`, exit `0` |
| `preflight/metadata-freeze-gate.json` | `PASS`; 112 records = 80 `CLEAR` + 32 `RED_TEAM`; all freeze/case/protocol/contract/index hashes match; 0 non-metadata attempt files |
| `preflight/final-status.json` | Consolidated final status: contract/self-test/baseline/unit/docs checks all exit `0`; `overallPreflightStatus: PASS`; Authors/Reviewers `NOT_RUN` |

Final frozen identity hashes:

| Artifact | SHA-256 |
| --- | --- |
| `frozen/public/public-manifest.json` | `00B579FE3D30AFB82CC2CC60753B6F59FA0CA9021ADB52A7CBD2E70BF4895DDE` |
| `frozen/private/hidden-answer-key.json` | `7C4D1889FA400BED2C59D976E106BDAF99E3264FA6D016DDE0AF29A5076EC332` |
| `frozen/private/red-team-design.json` | `8AB9E53841565F8D18C9F52B82C52A6882AA13C90AA89D3CB17B81AE40A09CB4` |
| `frozen/private/freeze-record.json` | `06B66F9C349FD615FB8F9108BF28E0C44D32F3717DBFCACD6825F7308F1C74BD` |
| `frozen/public/author-protocol.json` | `8EB5F75DD87A6DC0A0C6D8873EA96F8A8E22C9052017C8758106B781CB17AB96` |
| `frozen/public/red-author-protocol.json` | `C195E3D4601DC419115B61078856557F7E92132E7D2093606D5AA0F2FE32F300` |
| `frozen/public/artifact-contract.json` | `D27E429F5D67C5B191BDEA579048C3FD7AA598FCF63675D10A5FA5CC0E69FA6D` |
| `frozen/public/dependencies/static-validator/runtime-manifest.json` | `BA03E12D165D1303425A69E8626C13F70FDCE0AC399E51A0DE88D541A2393DE6` |
| `frozen/public/dependencies/fixture-locator/matching-packet.json` | `F4B22E7804D7B3886523B1FB09FDDCBAFCDF4FACB0D021A7328504B58499D2B4` |
| `frozen/public/red-fixtures/upstream-stage-fail.json` | `6808B2612EA13187FBCA37C78C173DFB703959D1F552503E290A699A866071BE` |

The retained safe-static baseline remains `MEASURE_ONLY`, `qualification=false`,
and is validated both directly as v0.1.7 and through the v0.1.10 projection.

## Closure record

Status: **Complete**  
Scope: v0.1.10 frozen corpus identity, contract/hash reconciliation,
baseline/static preflight, deterministic harness checks, and metadata
preparation.  
Acceptance criteria: new identity, final freeze before `prepare`, valid case
and transitive references, preserved baseline, direct/projected validator
`PASS`, static compatibility `PASS`, harness/runner tests and compilation
`PASS`, and final freeze hash on all 112 metadata records — **met**.  
Verification: D-drive preflight JSON/transcripts and hashes listed above.  
Evidence: the evidence root and report paths above.  
Boundary / next dependency: no Authors or Reviewers were executed; benchmark
qualification, candidate activation, normal dispatch, product execution,
runtime parity, human comparison, Original mutation, commit/push, release, and
deployment remain separate approvals.

Next priority: admit or decline the v0.1.10 Authors (112) and Reviewer (16)
Round 1 run using this immutable identity.  
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.
