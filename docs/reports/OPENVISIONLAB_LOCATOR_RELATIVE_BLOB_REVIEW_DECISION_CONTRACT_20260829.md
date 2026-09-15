# OpenVisionLab Locator-relative Blob Review Decision Contract

Date: 2026-08-29 KST
Repository: `C:\Git\OpenVisionLab_Dev`
Status: **Complete — bounded review-decision contract and pending handoff; physical approval remains open**

## 1. Scope

The native candidate diagnostic already retained a hash-verified packet and
candidate-review drawings, but its result still stopped at
`REVIEW_ONLY / NOT_APPROVED`. This slice adds the smallest durable seam for
recording a later operator decision without selecting a physical candidate or
changing the current Recipe, Preview, or Run behavior.

Included:

- a file-backed `locator-relative-blob-review-decision-v1` contract;
- packet SHA-256, source/template/overlay hashes, and a deterministic reviewed
  Plan fingerprint;
- the selected retained `CandidateId`, visual-correspondence state, reviewer,
  review time, notes, fixed inspection ROI, threshold, area range, and optional
  exact ResultCount;
- fail-closed stale checks against the current packet and Plan;
- automatic creation of a `PENDING / NOT_REVIEWED` template beside a successful
  native candidate packet.

Excluded:

- approving IC Frame4 or any other physical/native datum;
- changing the current fixed Blob ROI, threshold, area range, or count;
- automatic candidate selection, per-image tuning, XML import, Preview, Run,
  Recipe promotion, qualification, release, or deployment;
- WPF UI changes and performance testing.

## 2. Contract behavior

The contract is implemented by
`src/OpenVisionLab/UI/Menu/Wpf/Recipe/IntentSkills/OpenVisionRecipeLocatorRelativeBlobReviewDecision.cs`.

| State | Meaning | Required operator evidence |
| --- | --- | --- |
| `PENDING` | Review handoff exists; no approval has been claimed. | None yet; visual state must remain `NOT_REVIEWED`. |
| `APPROVED` | An explicit review approved the packet's numerically accepted candidate and current downstream definition. | `PASS` visual correspondence, reviewer, UTC time, notes, matching packet/Plan hashes, and current ROI/parameters. |
| `REJECTED` | The reviewed packet or candidate is not accepted. | Reviewer, UTC time, and reason. |
| `REPLACEMENT_REQUESTED` | The physical datum or evidence is insufficient and a new packet is required. | Reviewer, UTC time, and reason. |

An approved record must still match all of the following at validation time:

1. the exact evidence packet file and its current SHA-256;
2. source image, locator template, and current-run overlay hashes;
3. the packet's retained, numerically accepted `CandidateId`;
4. the current reviewed Plan fingerprint and its fixed inspection ROI,
   threshold, area range, and optional ResultCount.

Changing any of those inputs makes the decision stale. A lower-ranked retained
candidate cannot be silently promoted by editing the decision; it requires a
new packet whose runtime accepted candidate is explicit. `APPROVED` is an
operator review state, not Train/Validation/Held-out qualification or release
approval.

## 3. Runtime handoff

When the native candidate diagnostic exports a valid v1.1 packet, it now writes:

```text
<evidence-root>\review-decision.template.json
```

The generated file is deliberately:

```text
decision=PENDING
visualCorrespondence=NOT_REVIEWED
reviewer=""
reviewedUtc=""
```

The diagnostic continues to publish `operator_approval=NOT_APPROVED` and
`qualification=false`. The existing Guided Setup `Packet 로드` and
`검증·Compile` path is unchanged: it still reviews the packet and prepares an
XML draft only. Wiring this decision file into a user-facing approval action
and making Recipe promotion consume `APPROVED` is a separate UI/workflow task;
this slice does not claim that action exists.

## 4. Verification

### Contract smoke

```powershell
dotnet build "tools\LocatorRelativeBlobSkillSmoke\LocatorRelativeBlobSkillSmoke.csproj" `
  -c Debug -p:Platform="Any CPU" --no-restore `
  -p:OutputPath="D:\OpenVisionLab-TestData\OpenVisionLab_Dev\build\locator-relative-blob-review-decision\"

dotnet "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\build\locator-relative-blob-review-decision\LocatorRelativeBlobSkillSmoke.dll"
```

Result: build completed with 0 errors and 10 pre-existing nullable-analysis
warnings in the smoke harness; contract smoke returned
`LocatorRelativeBlobSkillSmoke: PASS`. The smoke covered pending creation,
save/load, packet/Plan matching, a synthetic contract-only explicit approval,
and stale Plan/packet hash rejection.

### Native handoff smoke

The existing `01-ok` native candidate source/template and fixed
`368,368,96,96` inspection ROI were rerun through the real diagnostic:

```text
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-external-native-ic-frame4-review-decision-r4
```

Observed result:

- native candidates: 2;
- score margin: `16.001`;
- ambiguity gate: pass;
- packet export: pass;
- packet reload/compile/replay: pass;
- review decision template: written;
- decision: `PENDING`;
- visual correspondence: `NOT_REVIEWED`;
- operator approval: `NOT_APPROVED`;
- qualification: `false`.

No EXE, WPF runtime, performance, PC restart, original repository, commit,
push, release, or deployment operation was performed.

## 5. Completion record

Status: Complete
Scope: File-backed review-decision contract and pending native-packet handoff.
Acceptance criteria: explicit state model, hash/Plan stale rejection,
pending-only diagnostic output, and no automatic physical approval — all pass.
Verification: Dev build, contract smoke, native candidate handoff smoke, and
JSON field inspection.
Evidence: the D-drive build directory and
`locator-relative-blob-external-native-ic-frame4-review-decision-r4` listed
above.
Boundary / next dependency: an operator must still review the drawings and
provide an explicit decision plus downstream inspection definition; UI wiring,
full frozen Train/Validation/Held-out qualification, and Recipe/default
promotion remain open.
