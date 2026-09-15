# OpenVisionLab `locator-relative-blob-v1` review-decision UI wiring

Date: 2026-08-29 KST
Repository: `C:\Git\OpenVisionLab_Dev`
Status: **Incomplete — implementation and functional smoke passed; the deferred WPF completion gate remains open**

## 1. Scope

This slice wires the existing file-backed
`locator-relative-blob-review-decision-v1` contract into the Recipe Manager
Guided Setup surface. It keeps the candidate, ROI, and downstream definition
explicitly operator-owned.

Included:

- load a hash- and Plan-bound review-decision JSON after a verified Evidence
  Packet is loaded;
- show visual correspondence, reviewer, notes, decision state, and the
  current candidate/integrity review in Guided Setup;
- record `APPROVED`, `REJECTED`, or `REPLACEMENT_REQUESTED` only after the
  operator supplies visual correspondence and review metadata;
- save a decision beside a template without overwriting the pending template,
  reload the persisted record, and revalidate it;
- require a current persisted `APPROVED` decision before locator Recipe import;
  packet hash, source/template/overlay hashes, CandidateId, Plan fingerprint,
  ROI, threshold, area, and count are rechecked before promotion;
- keep approval separate from qualification, Preview, Run, layer mutation, and
  routing.

Excluded:

- selecting or approving the physical/native locator datum;
- inventing a candidate or downstream ROI/tolerance;
- Train/Validation/Held-out qualification;
- performance testing, release, deployment, commit, push, or PC restart.

## 2. Implementation boundary

The existing `OpenVisionShellHostRecipeCommandSurface` remains the owner of
Guided Setup interaction orchestration. The file-backed decision class remains
the contract/validation owner. The WPF view only binds the explicit inputs and
commands.

The import path is fail-closed in two layers:

1. `CanImportLlmXmlDraft` keeps the button disabled unless the in-memory state
   has a loaded approved decision.
2. `ImportLlmXmlDraft` reloads the decision from disk and validates it against
   the current packet and Plan immediately before any dependency copy or Recipe
   mutation. A stale, missing, pending, rejected, or replacement-requested
   record is blocked.

Loading or recording a decision does not execute Preview/Run and does not
qualify the locator.

## 3. Verification

### Source and contract checks

- `LocatorRelativeBlobSkillSmoke` build: 0 errors, 10 existing nullable
  warnings; contract smoke returned `LocatorRelativeBlobSkillSmoke: PASS`.
- `OpenVisionReadinessCheck`: all checks `OK`; readiness contract passed.
- `PipelineViewerScreenshotSmoke` build: 0 errors, 1 existing nullable warning.
- `git diff --check`: no whitespace error; Git reported only the repository's
  existing LF/CRLF conversion warnings.

### Functional WPF smoke

The target was run from the freshly built
`PipelineViewerScreenshotSmoke.dll` with a bounded wait. The monitor topology
was detected dynamically: two independent monitors were present, and the
smaller left work area was selected.

Observed placement and result:

```text
SelectedMonitor=-1920,365,0,1397
WindowRect=-1900,385,-300,1285
ExitCode=0
wpf_shell_host_recipe_locator_relative_blob_guided_setup=OK|check=OK|elapsed=1961ms|colors=64|flat=0%|layout=0|text=0|internal=0|size=1600x900
```

The smoke verified that:

- the locator Guided Setup controls and AutomationIds are present;
- the latest v1.1 Evidence Packet loads and compiles without Preview/Run or
  layer-count changes;
- the pending review template keeps Recipe import disabled;
- a synthetic UI-only `APPROVED` record can be persisted and enables the
  current import command gate;
- reloading the unchanged pending template disables import again.

The synthetic record is explicitly not an operator approval. It is only a
plumbing assertion and is stored in the D-drive test root.

## 4. Evidence

- Screenshot and smoke output:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-smoke-locator-review-20260829-r4\wpf_shell_host_recipe_locator_relative_blob_guided_setup.png`
- Synthetic approval and restored pending template:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-smoke-locator-review-20260829-r4\review-decision.json`
  and
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-smoke-locator-review-20260829-r4\review-decision.template.json`
- Build output:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\build\locator-review-ui-smoke-r2\`
- Native pending packet used by the smoke:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-external-native-ic-frame4-review-decision-r4\`

## 5. Completion record

Status: Incomplete
Scope: Guided Setup review-decision UI wiring and fail-closed locator Recipe
promotion gate.
Acceptance criteria: explicit load/record controls, persisted reload, current
packet/Plan validation, pending-before-approval, synthetic approved-path
enablement, and restored pending-path blocking — all passed.
Verification: Dev builds, contract/readiness checks, dynamically placed WPF
functional smoke, and screenshot inspection.
Evidence: the D-drive paths listed above.
Boundary / next dependency: no actual operator candidate/ROI decision has been
made; full WPF state/theme/layout/DPI coverage and the project-mandated UI
performance smoke were intentionally not run because the user deferred
performance testing. Therefore this slice is not a full UI-gate completion or
locator qualification.
