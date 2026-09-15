# Locator-relative Blob second-candidate evidence gate

Date: 2026-08-29 KST
Repository: `C:\Git\OpenVisionLab_Dev`
Scope: `locator-relative-blob-v1` evidence export, packet validation, and bounded corpus intake

## Decision

`ScoreMargin` is treated as observed competition evidence only when the runtime
locator retains at least two `VisionPipelineMatchResultEvidence` candidates.
A numeric `ScoreMargin` from the generic Matching metric is not sufficient when
the second candidate was not retained.

Because this is a compatibility-tightening change for file-backed evidence, the
packet schema is explicitly revised to `locator-relative-blob-evidence-v1.1`.
The `locator-relative-blob-v1` skill ID and deterministic execution graph remain
unchanged. There is no automatic migration for the older one-candidate packet.

The change is deliberately bounded to the locator evidence contract:

- exporter rejects a successful `NUM_MATCH=2` run with fewer than two retained
  candidates;
- evidence packet v1.1 save/load/validation rejects packets with fewer than two
  candidates and keeps the existing exactly-one-accepted rule;
- corpus intake records the explicit state `ObservedSecondCandidate`,
  `MissingSecondCandidate`, or `Unavailable`;
- the numeric CSV margin is recorded only for `ObservedSecondCandidate` rows;
- global Matching metric enrichment and unrelated Matching consumers are unchanged.

## Verification

### Build

Command:

```powershell
dotnet build "tools\LocatorRelativeBlobSkillSmoke\LocatorRelativeBlobSkillSmoke.csproj" -c Debug -p:Platform="Any CPU" --no-restore
```

Result: passed with 0 errors; the final run reported 8 nullable-analysis
warnings in the smoke harness.

### Contract smoke

Command:

```powershell
dotnet "tools\LocatorRelativeBlobSkillSmoke\bin\Any CPU\Debug\net8.0-windows7.0\LocatorRelativeBlobSkillSmoke.dll"
```

Result: `LocatorRelativeBlobSkillSmoke: PASS`.

The smoke covers:

- a packet with two retained candidates, exactly one accepted candidate,
  save/load, and deterministic compile;
- a one-candidate packet rejected with `second candidate is missing`;
- unknown candidate ID, source hash mismatch, wrong coordinate frame,
  multiple accepted candidates, tampered fixed ROI, and compile-without-evidence
  fail-closed paths.

Evidence:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-v1.1-contract-20260829`

### Positive native two-candidate probe

The existing public locator template was copied into a second position in the
public synthetic source. The second instance was deliberately degraded so that
the native matcher had two distinct, still-detectable scores. This is a
provenance probe, not a qualification dataset or a per-image production
tuning change. At the user's direction, the same explicitly chosen public
fixture and fixed ROI were rerun as a current smoke check.

Command:

```powershell
dotnet "tools\LocatorRelativeBlobSkillSmoke\bin\Any CPU\Debug\net8.0-windows7.0\LocatorRelativeBlobSkillSmoke.dll" `
  --locator-relative-blob-dual-candidate-probe `
  "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-dual-candidate-probe-20260829-v5"
```

Result: `LocatorRelativeBlobDualCandidateProbe: PASS`.

The current native run retained two candidates with scores `100` and `82.205`,
producing an observed `ScoreMargin` of `17.795` against the plan minimum `10`.
The v1.1 packet contained two candidates with exactly one accepted candidate;
packet reload, CandidateId-only compile, and compiled replay all passed with the
same runtime outcome.

Evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-dual-candidate-probe-20260829-v5\dual-candidate-source.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-dual-candidate-probe-20260829-v5\locator-runtime-overlay.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-dual-candidate-probe-20260829-v5\evidence.packet.json`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-dual-candidate-probe-20260829-v5\probe-summary.txt`

### External Die Array candidate provenance probe

The historical P227 `Die Array / Die1.tif` candidate was used as an explicit
diagnostic input. Its prior review already stopped this candidate because the
96x96 window follows a repeated grid rather than a unique physical locator.
The current native Matching runtime was run against the preserved
`die_array_003_ok.jpg` source with `NUM_MATCH=2`, the same full-image search,
and the fixed `144,128,96,96` review ROI.

Command:

```powershell
dotnet "tools\LocatorRelativeBlobSkillSmoke\bin\Any CPU\Debug\net8.0-windows7.0\LocatorRelativeBlobSkillSmoke.dll" `
  --locator-relative-blob-external-die-array-provenance `
  "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-external-die-array-provenance-20260829-r2"
```

Result: `LocatorRelativeBlobExternalDieArrayProvenance: PASS` for the bounded
provenance criterion. The native run retained two candidates: score `100` at
`192,176` and score `96.276` at `56,464`, yielding a real
`ScoreMargin=3.724`. The locator tool itself succeeded, but the ambiguity gate
correctly returned `acceptance=false` because the required margin is `10`.
The overall pipeline result is therefore `runtime_success=false`; this is the
expected fail-closed outcome, not a processing exception.

The same runtime result was then passed to the actual
`OpenVisionRecipeLocatorRelativeBlobEvidenceExporter.TryExport` path. It
returned `false` with `The runtime locator did not pass its explicit success and
ambiguity gates.`, returned no packet or exporter overlay path, and left
`evidence.packet.json` absent. This proves the external negative case is
fail-closed at the real exporter boundary, not merely skipped by the probe.

The probe does not export a v1.1 evidence packet because this is a historically
rejected repeated-grid candidate and its visual correspondence is `NOT_REVIEWED`
for the current run. The source, template, candidate JSON, native overlay, and
exporter refusal fields are retained at:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-external-die-array-provenance-20260829-r2`

This is the first current external sample showing real second-candidate
retention, but it is negative ambiguity evidence, not an approved locator,
qualification corpus, or a reason to lower the margin threshold.

### External corpus boundary run

Command:

```powershell
dotnet "tools\LocatorRelativeBlobSkillSmoke\bin\Any CPU\Debug\net8.0-windows7.0\LocatorRelativeBlobSkillSmoke.dll" `
  --locator-relative-blob-corpus-pilot `
  "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-e-die-pad-20260827" `
  "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-corpus-pilot-20260829-second-candidate-gate-final2"
```

Result: `LocatorRelativeBlobCorpusPilot: PARTIAL`, exit code `1` by design for
the current input. The new evidence summary reports:

| Metric | Result |
| --- | ---: |
| input rows | 122 |
| packets | 0 |
| compiled replays | 0 |
| observed second candidate | 0 |
| missing second candidate | 120 |
| candidate evidence unavailable | 2 |
| processing errors | 0 |

Evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-corpus-pilot-20260829-second-candidate-gate-final2\summary.json`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-corpus-pilot-20260829-second-candidate-gate-final2\intake.csv`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-corpus-pilot-20260829-second-candidate-gate-final2\report.txt`

The exit code is not a processing failure. It records that the current corpus
has no positive two-candidate packet rows under the corrected contract.

## Boundary and next prerequisite

The previous 2026-08-27 run accepted 120 one-candidate packets because the
then-current path allowed a missing second candidate to surface as a numeric
margin. Those v1 files remain historical pilot evidence without automatic
migration; they are not reclassified as observed-margin evidence.

This slice does not qualify the skill, infer OK/NG truth, add other dataset
families, connect Recipe/Run History, change the main Pipeline Review, run UI
matrices, measure performance, release, publish, deploy, or restart the PC.
The positive synthetic native probe is now complete, and the external Die Array
probe confirms real second-candidate retention while correctly failing both the
ambiguity gate and packet export. The next prerequisite is an operator-approved
real/native corpus case whose locator retains two candidates and also passes the
ambiguity gate, followed by a frozen Train/Validation/Held-out review scope.

## Completion record

Status: Complete
Scope: Fail-closed second-candidate provenance and external ambiguity refusal for locator-relative Blob evidence
Acceptance criteria: exporter and packet reject missing-second evidence; actual external exporter refuses an ambiguity failure without creating a packet; intake records explicit state; smoke/build pass
Verification: focused tool build, contract smoke, positive native two-candidate probe, 122-row corpus boundary run, and external Die Array exporter-refusal probe
Evidence: D-drive paths listed above
Boundary / next dependency: the Die Array external row supplies two candidates but fails the ambiguity gate and remains unapproved; a passing approved real/native row and frozen qualification splits are still missing
