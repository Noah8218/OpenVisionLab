# OpenVisionLab `locator-relative-blob-v1` native candidate gate

Date: 2026-08-29 KST
Repository: `C:\Git\OpenVisionLab_Dev`
Status: **Complete — bounded native-candidate diagnostic and pilot evidence; Recipe qualification remains open**

## 1. Scope and decision boundary

This slice continued the next priority from the current handoff: test a real/native
candidate set with the current two-candidate retention and ambiguity gate. The
work stayed in `REVIEW_ONLY`; it did not approve a Recipe, select a per-image
fallback, lower a threshold, or claim production accuracy.

The fixed runtime graph remained:

```text
Matching(NUM_MATCH=2)
  -> Matching(NUM_MATCH=1, LocatorFrame)
  -> RotateScale(NormalizeImage)
  -> Threshold
  -> Blob(fixed reference-coordinate ROI)
```

The candidate set was frozen before execution to the 13 `SUGGESTED` rows already
recorded in the P227 pilot summary. Each row used its existing canonical source,
96x96 template, candidate ROI, and reference pose. The current locator settings
were global across all rows:

| Setting | Value |
| --- | ---: |
| Search ROI | full source image (`0,0,512,512`) |
| Score minimum | `0.8` |
| Score margin minimum | `10` |
| Angle range | `-5..+5°` |
| Scale range | `0.8..1.8` |
| Minimum valid pixel ratio | `0.25` |
| Downstream threshold / area | `170` / `700..1300` |

The new smoke mode is explicit rather than hard-coded to one external sample:

```text
--locator-relative-blob-external-native-candidate
  <source> <template> <inspection-roi> <reference-pose> <new-D-drive-output>
```

It copies the exact source and template into the D-drive evidence folder, records
SHA-256 values, retains every native candidate returned by `NUM_MATCH=2`, writes
the full locator overlay, and writes a pose-normalized candidate patch,
template-vs-candidate panel, and 50% blend per candidate. If the locator gate
passes, the real v1.1 exporter is exercised and the packet is reloaded, compiled,
and replayed. Visual state remains explicit and is never inferred from score or
overlay color.

## 2. Fixed 13-candidate native probe

Evidence root:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-external-native-candidates-20260829`

| Candidate family | Native candidates | Score margin | Gate result |
| --- | ---: | ---: | --- |
| Switch1, Switch2, Switch3 | 1 each | not observed | fail: second candidate not retained |
| PCB BOARD | 1 | not observed | fail: second candidate not retained |
| IC Frame4 | 2 | `16.001` | pass |
| IC Frame5 | 1 | not observed | fail: second candidate not retained |
| Floppies | 1 | not observed | fail: second candidate not retained |
| Die Pad1, Die Pad2, Die Pad3, Die Pad4 | 1 each | not observed | fail: second candidate not retained |
| Die Array1 | 2 | `3.724` | fail: margin `< 10` |
| Die Array2 | 2 | `2.922` | fail: margin `< 10` |

This produced one native candidate with both numeric gates passing: P227
`ic_frame__Frame_4_rank_01.png` on `ic_frame_001_ok.jpg`. The first candidate
was `score=100` at `(416,416)` and the second was `score=83.999` at `(424,201)`.
The exporter created a v1.1 packet for this row. That packet is evidence of the
runtime gate only, not approval or qualification.

## 3. IC Frame4 fixed pilot replay

The existing P227 IC Frame4 pilot set was kept fixed at eight rows: four OK-role
rows and four NG-role rows. `Role` is retained as source metadata only; it is not
treated as defect truth by this locator run.

Evidence root:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-external-native-ic-frame4-pilot-20260829-r2`

| Row | Role metadata | Candidates | Margin | Ambiguity gate | Packet/replay |
| --- | --- | ---: | ---: | --- | --- |
| `01-ok` | OK | 2 | `16.001` | pass | exported / matched |
| `111-ok` | OK | 1 | — | fail: missing second | — |
| `125-ok` | OK | 0 | — | fail: no result | — |
| `042-ok` | OK | 2 | `14.829` | pass | exported / matched |
| `197-ng` | NG | 2 | `15.181` | pass | exported / matched |
| `021-ng` | NG | 0 | — | fail: no result | — |
| `101-ng` | NG | 2 | `13.257` | pass | exported / matched |
| `236-ng` | NG | 2 | `14.645` | pass | exported / matched |

The locator-level result is therefore `5/8` rows with two retained candidates
and margin `>=10`, `1/8` with one candidate, and `2/8` with no candidate. All
five two-candidate rows saved v1.1 packets and all five packet replays preserved
the source runtime outcome.

The full five-step pipeline was not green for these rows: the first four steps
were successful on the gate-passing rows, while the fixed downstream Blob step
reported `ResultCount=0` with `Area=700..1300, ROI=368,368,96,96`. Consequently
the evidence proves a locator candidate gate, not a successful IC Frame4
locator-relative Blob inspection recipe.

## 4. Current visual review

The generated review artifacts are under each row's `candidate-review` folder.
The exact source/template/overlay and candidate artifact paths, hashes, pose, and
runtime state are retained in each `candidate-provenance.json`.

The following current spot review was performed from the fresh source, overlay,
template, and pose-normalized panels:

| Sample | Candidate 1 | Candidate 2 | Review scope |
| --- | --- | --- | --- |
| `01-ok` | `PASS` — bottom-right pin row and diagonal corner correspond to the template | `FAIL` — repeated right-side pin segment lacks the template's diagonal corner and bottom context | ordinary sample |
| `197-ng` | `PASS` — same bottom-right corner/pin relation remains visible | `FAIL` — repeated right-side segment, not the corner datum | defective/difficult sample |
| `236-ng` | `PASS` — locator corner remains corresponding while the separate bottom-left defect is outside the selected locator core | `FAIL` — repeated right-side segment without the corner datum | defective sample |

This is a bounded visual review, not an operator approval. The runtime JSON
therefore retains `visual_correspondence=NOT_REVIEWED` for unapproved automation
state, while this report records the human-readable spot-review findings. The
remaining pilot rows are not visually reviewed and must not be aggregated into a
qualification claim.

## 5. Evidence and verification

Build and contract smoke:

```powershell
dotnet build "tools\LocatorRelativeBlobSkillSmoke\LocatorRelativeBlobSkillSmoke.csproj" `
  -c Debug -p:Platform="Any CPU" --no-restore `
  -p:OutputPath="D:\OpenVisionLab-TestData\OpenVisionLab_Dev\build\locator-relative-blob\"

dotnet "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\build\locator-relative-blob\LocatorRelativeBlobSkillSmoke.dll"
```

Results:

- build: 0 errors, 10 existing nullable-analysis warnings in the smoke harness;
- contract smoke: `LocatorRelativeBlobSkillSmoke: PASS`;
- fixed native probe: all 13 predeclared candidates executed without changing
  candidate-specific gates;
- IC Frame4 pilot: 8 fixed rows executed, 5 packet exports, 5 packet reload/
  compile/replays with matching outcomes;
- fresh visual evidence: ordinary `01-ok` and defective `197-ng`/`236-ng`
  source-template-candidate comparisons and full overlays inspected;
- no UI, performance test, EXE launch, PC restart, original repository change,
  commit, push, release, or deployment was performed.

## 6. Completion record

Status: Complete
Scope: Explicit native candidate diagnostic mode plus a bounded 13-candidate
probe and fixed eight-row IC Frame4 pilot for the current two-candidate
ambiguity gate.
Acceptance criteria: candidate set frozen from existing evidence; exact
source/template/ROI/pose and SHA-256 retained; real `NUM_MATCH=2` candidates
recorded; ambiguity gate exercised; passing packets reloaded/compiled/replayed;
representative visual correspondence evidence retained; downstream failure not
overclaimed.
Verification: D-drive build, contract smoke, 13 native candidate runs, eight
IC Frame4 pilot runs, packet replay checks, and fresh source/template/overlay
visual checks.
Evidence: the two D-drive roots listed above and each row's
`candidate-provenance.json`, `probe-summary.txt`, overlay, and candidate-review
artifacts.
Boundary / next dependency: `REVIEW_ONLY` operator approval is still missing;
the historical source set is synthetic/labeling compatibility material; the
fixed IC Frame4 downstream Blob ROI produced no result; full frozen
Train/Validation/Held-out qualification and any production/default promotion
remain open.
