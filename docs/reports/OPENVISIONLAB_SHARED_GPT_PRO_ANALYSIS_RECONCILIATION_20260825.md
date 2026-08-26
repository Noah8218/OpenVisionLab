# OpenVisionLab Shared GPT Pro Analysis Reconciliation

Date: 2026-08-25 KST

Repository: `C:\Git\OpenVisionLab_Dev`

Current Dev branch at review: `codex/public-sample-ux-docs`

Current Dev HEAD at review: `827a22e92eba`
Shared analysis: <https://chatgpt.com/share/6a8d010b-b510-83ee-876d-3ad1c35714c8>

## Status

This document completes the requested analysis reconciliation, backlog
registration, prioritization, version boundary, and next-chat handoff. It does
not claim that the newly registered code defects are fixed.

The shared analysis reviewed public `main` at its stated `8a029827...`
baseline. The local Dev workspace is newer and contains a material uncommitted
reliability bundle. Therefore the shared analysis is an external review input,
not current source authority. Every accepted item below was checked against the
current Dev source or current project evidence before registration.

## Work Contract

### Concrete outcome

- classify every material shared-analysis proposal as accepted, already
  complete, partially accepted, deferred, or rejected;
- register source-confirmed work with testable acceptance criteria;
- give a Luna-first execution order;
- preserve OpenVisionLab's current product identity and exclusions;
- define the exact `2.1.0` / `v2.1.0-rc.2` commit, push, tag, and release
  boundaries;
- make the next task restartable from canonical documents and the issue ledger.

### Included

- current-source inspection;
- documentation and `.proofline` issue records;
- priority and version planning.

### Excluded from this slice

- runtime code changes;
- deleting tracked binaries;
- changing the product version;
- committing, pushing, tagging, publishing, or deploying;
- changing the original repository;
- installer, signing, update, self-contained packaging, camera, lighting,
  PLC/I/O, MES, provider/LLM expansion, or commercial-GA claims.

## Product Context That Must Be Preserved

- OpenVisionLab is an OpenCvSharp4 rule-based vision recipe workbench.
- Its normal operator route is sample image -> PropertyGrid teaching ->
  Pipeline composition -> explicit Preview/Run -> drawing/metric/layer review
  -> N-sample validation -> saved Recipe.
- LLM XML authoring remains optional maintenance-mode assistance.
- Current maturity is a bounded, evidence-backed Release Candidate. It is not
  installer/signing/update, multi-PC, hardware, calibrated-metrology, field,
  or commercial-GA qualification.
- Commercial products are useful references for guided configuration, visible
  intermediate/result/failure evidence, compact recipe lifecycle, and
  reproducible validation. Their camera/lighting/controller/account/MES breadth
  is not an OpenVisionLab target.

## Reconciliation Decisions

| Shared-analysis item | Current Dev finding | Decision | Durable owner |
| --- | --- | --- | --- |
| Audit bundled DLLs and licenses before the next RC | Confirmed gap. The current `dll` tree contains 25 files totaling 93,581,810 bytes. The existing policy/gate verifies the required SDK/OpenCvSharp/PropertyGrid core and a short forbidden list, but it does not classify every tracked binary or bind every retained file to redistribution evidence. | Accept as a release-gate audit. Do not delete a file until direct/dynamic use, publish need, and license evidence are classified. | `PL-0005` |
| Fix `ImageSpace` Bitmap lifetime and viewer ownership | Already completed in current Dev through reference-counted image owners/leases, central/docked/popout disposal, and the five-cycle 4512 lifetime gate. | Do not reopen without changed evidence. | Resolved `PL-0004`; current handoff and 2026-08-23 reliability report |
| Remove Emgu overlap | Already completed in current Dev. The three Emgu DLLs were removed and Canvas loading is OpenCvSharp-owned. | No new work. | Current 2026-08-23 reliability report |
| Fix 8-bpp/stride conversion | Confirmed and broader than the shared example. `BitmapImageConverter` can write a source-stride-sized buffer into a smaller destination row for odd-width 8-bpp single-channel output, assumes positive stride, copies destination-step bytes in other branches, and does not consistently dispose allocation on conversion failure. | Accept as the first code-safety change after baseline/audit. | `PL-0006` |
| Enforce recipe/pipeline path boundaries | Confirmed. Several path builders accept raw recipe/pipeline segments; `SanitizePathSegment` only replaces invalid filename characters and leaves `.`/`..`, reserved names, and trailing-dot/space cases; final containment is not checked on all create/write paths. Validation logic is duplicated. | Accept. Centralize one segment policy and require final `GetFullPath` containment before mutation while preserving valid legacy names. | `PL-0007` |
| Keep original Pipeline immutable and record effective execution identity | Confirmed. `VisionPipelineNormalizer` mutates `InputLayer` and preprocessing parameters. Recipe execution normalizes before invoking the execution service, which normalizes again. The run report keeps one snapshot identity but not original/effective hashes and normalization changes as a complete provenance contract. | Accept. Clone once, normalize once, execute the effective plan, and preserve/report both identities. | `PL-0008` |
| Add broad unit-test infrastructure | The repository uses purpose-built executable smoke/contract tools and has no general xUnit/NUnit/MSTest suite. A new framework is not itself an acceptance criterion. | Partially accept. Add the smallest deterministic reproducer to the current owning smoke/check target for each issue. Reconsider a shared test project only after repeated duplication or poor isolation is measured. | Criteria inside `PL-0006` through `PL-0010` |
| Add a runtime fingerprint to every report | The need is valid but overlaps immutable original/effective execution provenance. | Merge into `PL-0008`: schema version, original/effective hashes, normalization changes, app version, and SDK identity. Avoid a second parallel identity format. | `PL-0008` |
| Remove duplicate Blob/Contour execution used to collect rejected objects | Confirmed. `VisionPipelineObjectResults.TryCaptureUnfiltered` creates and executes a relaxed audit tool after the normal execution and silently falls back to accepted-only evidence on failure. | Accept as measured P1 work. First identify an SDK-compatible one-pass evidence path; do not weaken result parity to remove a call. | `PL-0010` |
| Make Pipeline rename/delete and active-pointer persistence atomic | Confirmed. Rename saves the new file, deletes the old file, then updates the active pointer; delete removes the file before updating the pointer; the pointer uses direct `WriteAllText`. | Accept after the shared path-boundary policy, with failure-injection and recovery evidence. | `PL-0009`, following `PL-0007` |
| Run a WPF memory soak | The current Dev work already completed bounded ImageSpace/viewer and OpenGL allocation/lifetime gates. | Do not repeat unchanged evidence. Admit a new soak only for a changed owner, reproduced regression, or a new bounded UI slice. | Resolved `PL-0004` and OpenGL completion report |
| Recruit three first-time users | Still the valid external `CVR-00` prerequisite. Agent recordings cannot satisfy it. | Keep deferred. It is not a general blocker for the accepted source-hardening work, but it blocks novice/commercial-usability claims. | `CVR-00` |
| Publish `v2.1.0-rc.2` | Reasonable target only after the accepted hardening gates pass. The current source product version remains `2.1.0`; the existing public candidate is `v2.1.0-rc.1`. | Register as a coordination issue, not as current release authorization. | `PL-0011` |
| Add GitHub description/topics | Low-value external metadata mutation compared with confirmed correctness/release gates. | Defer until a separately authorized repository-publication task. | No active issue |
| Expand x86/AnyCPU support or add more platforms | No reproduced current support defect establishes this scope. | Reject for this train. Preserve the current documented Windows x64 release boundary. | None |

## Source-Confirmed Defect Notes

### `PL-0006` Bitmap conversion safety

- `src/OpenVisionLab/Common/BitmapImageConverter.cs:150-181` uses the source
  stride as the managed buffer length in the non-contiguous 8-bpp path, then
  copies that whole buffer into a destination row whose step can be only the
  pixel width.
- Negative `BitmapData.Stride` is treated as a positive allocation and pointer
  increment in several branches.
- The 24/32-bpp non-contiguous paths copy `dstStep` bytes instead of the exact
  visible row byte count in some branches.
- Allocating `ToMat`/`ToBitmap` overloads need deterministic disposal when the
  conversion throws.
- The current narrow indexed-color smoke does not cover odd-width one-channel
  conversion, negative stride, destination submatrices, or guard corruption.

### `PL-0007` storage boundary

- `RecipeWorkspaceService.GetRecipeDirectory`, `GetVisionDirectory`, and
  `CombineRecipePath` compose raw recipe names.
- several run/sample/batch helpers sanitize only one or two leaf names and
  call `Directory.CreateDirectory` without checking final root containment;
- `SanitizePathSegment` replaces only `Path.GetInvalidFileNameChars()` and does
  not reject traversal segments or Windows reserved/ambiguous names;
- related validation/sanitization is repeated in storage, lifecycle, UI, and
  LLM-draft paths.

### `PL-0008` execution provenance

- `VisionPipelineNormalizer.NormalizeLikelySequentialLink` assigns
  `step.InputLayer`;
- chained inspection normalization changes `USE_THRESHOLD`,
  `USE_ADAPTIVE_THRESHOLD`, and `USE_BITWISENOT`;
- `VisionRecipeRunner.RunAsync` and `VisionPipelineExecutionService.RunAsync`
  both call `NormalizeForRun` on the supplied model;
- current run-report storage does not preserve a complete original/effective
  identity and transformation record.

### `PL-0009` persistence atomicity

- `VisionPipelineStorage.TryRenamePipeline` saves the target, deletes the
  source, then updates the active name;
- `TryDeletePipeline` deletes the selected file before updating the active
  name;
- `SaveActivePipelineName` calls `File.WriteAllText` directly.

### `PL-0010` duplicate evidence execution

- `VisionPipelineObjectResults.TryCaptureUnfiltered` creates a second relaxed
  Blob/Contour tool and executes the same input again;
- exceptions return an empty list, causing accepted-only fallback and making
  audit completeness depend on a hidden second run.

## Prioritized Development List

The user requested Luna for continuation. The least expensive available Luna
configuration that still matches each risk is listed below.

0. Establish one verified Dev baseline from the existing reliability bundle and repair or explicitly separate the stale focused-smoke assertions in `PL-0003` before mixing in new source work | Recommended model: gpt-5.6-luna | Reasoning effort: medium
1. `PL-0005` classify every tracked external binary and its redistribution evidence; this is read-mostly but can block any RC2 publication | Recommended model: gpt-5.6-luna | Reasoning effort: high
2. `PL-0006` make `BitmapImageConverter` row copies, signed stride handling, submatrix behavior, exceptions, and failure disposal memory-safe | Recommended model: gpt-5.6-luna | Reasoning effort: high
3. `PL-0007` establish one recipe/pipeline path-segment and final-containment policy, including legacy compatibility and mutation tests | Recommended model: gpt-5.6-luna | Reasoning effort: high
4. `PL-0008` clone and normalize a Pipeline once, preserve the original model, and persist original/effective execution provenance | Recommended model: gpt-5.6-luna | Reasoning effort: high
5. `PL-0009` make Pipeline rename/delete plus active-pointer changes atomic or recoverable, after the path policy is stable | Recommended model: gpt-5.6-luna | Reasoning effort: medium
6. `PL-0010` measure and replace the second Blob/Contour audit execution with one-pass evidence only if SDK/result parity can be proven | Recommended model: gpt-5.6-luna | Reasoning effort: high
7. `PL-0011` run the exact clean RC2 candidate gate and prepare release evidence; mutation remains separately authorized | Recommended model: gpt-5.6-luna | Reasoning effort: high
8. Keep `CVR-00` deferred until three independent first-time participants and their unedited observations exist | Recommended model: none until participants exist | Reasoning effort: none until participants exist

The immediate project priority is baseline separation followed by `PL-0005`.
The first source correction is `PL-0006`. The remaining pre-existing project
priority is still the externally blocked `CVR-00`; the hardening train does not
convert agent-operated testing into novice evidence.

## Version, Commit, Push, Tag, And Release Plan

### Version truth

- Product version sources currently say `2.1.0`:
  `src/OpenVisionLab/OpenVisionLab.csproj` and
  `src/OpenVisionLab/Core/State/GlobalState.cs`.
- The currently published candidate is `v2.1.0-rc.1` from the original
  repository, as recorded in
  `docs/reports/OPENVISIONLAB_2_1_0_RC1_PUBLICATION_20260805.md`.
- Normal development commits do not increment the product version.
- The next approved candidate identity is `v2.1.0-rc.2`, not `2.1.1` and not a
  new version per issue.

### Commit boundaries

Each accepted issue should land as a focused, independently verified commit
after the current reliability bundle has a known baseline. Suggested subjects:

1. `Audit third-party binary boundary (PL-0005)`
2. `Make bitmap conversion stride-safe (PL-0006)`
3. `Enforce recipe storage path boundaries (PL-0007)`
4. `Preserve pipeline execution provenance (PL-0008)`
5. `Make pipeline persistence recoverable (PL-0009)`
6. `Capture object evidence in one execution (PL-0010)`
7. `Prepare OpenVisionLab 2.1.0 RC2 evidence (PL-0011)`

Do not force an artificial commit if an intermediate state cannot build or
preserve behavior. Conversely, do not combine unrelated issue work merely to
reduce commit count.

### Authorization boundaries

The following remain separate decisions even when the preceding one passes:

1. edit and verify in Dev;
2. commit the focused Dev change;
3. push the Dev branch;
4. promote a reviewed commit to the original repository;
5. commit/push the original branch;
6. create annotated tag `v2.1.0-rc.2` on the exact approved original commit;
7. push that exact tag;
8. create a draft release with exact artifacts and checksums;
9. publish the release.

The repository rule requires an explicit `PUSH` request before any push. This
document and the user's desire to work version-by-version do not silently
authorize a push, tag, release, or original-repository mutation. If a published
RC2 later needs correction, create `v2.1.0-rc.3`; never move or overwrite the
published RC2 tag or bytes.

## RC2 Entry And Exit Gates

### Entry

- current Dev reliability changes are separated into a known, verified
  baseline;
- `PL-0003` is resolved or proven unrelated and explicitly separated;
- exact candidate scope is frozen;
- no source files change after the final build begins.

### Exit

- `PL-0005`, `PL-0006`, `PL-0007`, and `PL-0008` are resolved with reusable
  evidence;
- `PL-0009` and `PL-0010` each have an explicit include/defer decision backed
  by their criteria and risk;
- Debug and Release builds, readiness 13/13, external-reference checks, public
  sample checks, and all focused issue checks pass from the exact candidate;
- the copied clean runtime launches and uses the intended dependency set;
- version sources, exact commit, proposed tag, artifacts, and SHA-256 hashes
  agree;
- original-repository promotion, tag, draft release, and publication each
  receive their own authorization.

## Next-Chat Luna Restart

Paste or invoke this request in the next task:

```text
Work only in C:\Git\OpenVisionLab_Dev. Load the repository instructions and
mandatory Ponytail/Proofline modes. Read docs/LLM_DOCUMENT_INDEX.json using the
start_or_continue route, then read
docs/reports/OPENVISIONLAB_SHARED_GPT_PRO_ANALYSIS_RECONCILIATION_20260825.md
and .proofline/issues/PL-0005.json. First inspect the existing dirty reliability
bundle and PL-0003; establish or request the exact safe commit boundary before
mixing new code. Then execute PL-0005 only, using gpt-5.6-luna with high
reasoning. Do not delete an unclassified binary. Do not change the product
version, original repository, tag, release, or deployment. Do not commit unless
the completed scope and verification form one coherent boundary. Do not push
unless I explicitly say PUSH.
```

After `PL-0005` closes, continue with `PL-0006` rather than selecting a new
feature from the older backlog.

## Durable Closure Record

Status: Complete

Scope: Shared GPT Pro analysis reconciliation, current-source triage, seven
durable issue records (`PL-0005` through `PL-0011`), Luna-first priorities,
version boundaries, and next-chat handoff only.

Acceptance criteria: shared source inspected -> pass; current Dev evidence used
to accept/reject each material proposal -> pass; already completed work excluded
from reopening -> pass; testable backlog and execution order recorded -> pass;
`2.1.0`/RC2 commit-push-tag-release boundaries recorded -> pass; next-task route
recorded -> pass.

Verification: issue-ledger validation -> 11 valid issues with only the two
expected legacy-Markdown warnings; `tools\TestDocumentationIndex.ps1` ->
`DocumentationIndex=PASS IndexedPaths=68 Routes=12 RootRedirects=101`;
`git diff --check` -> exit 0 with line-ending conversion warnings only; explicit
new report/issue trailing-whitespace scan -> pass.

Evidence: this report, `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`,
`docs/LLM_DOCUMENT_INDEX.json`, and `.proofline/issues/PL-0005.json` through
`.proofline/issues/PL-0011.json`.

Boundary / next dependency: no runtime fix, binary deletion, commit, push, tag,
release, deployment, or original-repository mutation is claimed. The next
engineering dependency is a safe verified baseline for the current dirty Dev
worktree, followed by `PL-0005` and `PL-0006`.
