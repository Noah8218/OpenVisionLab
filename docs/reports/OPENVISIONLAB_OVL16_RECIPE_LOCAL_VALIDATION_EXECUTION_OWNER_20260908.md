# OpenVisionLab OVL-16 Local Validation-set Execution Owner

Updated: 2026-09-08 KST
Status: Complete for the single Local validation-set input snapshot and
execution boundary.

## Why this slice was selected

After OVL-12 through OVL-15, the Recipe session still copied Local
validation-set image records and resolved the Pipeline path before invoking
`OpenVisionRecipeValidationSetRunner`. The runner already owned the XML read,
ordered execution, expected-outcome mapping, stop/partial-save behavior, and
batch persistence. Moving the remaining input preparation into that existing
runner closes the boundary without adding another wrapper or changing the
Recipe contract.

## Structural change

`OpenVisionRecipeValidationSetRunner.CreateRunRequest` now owns:

- the Local set name and notes snapshot;
- the existing image-record copy, including variant and expected-metric fields;
- the existing `RecipeWorkspaceService` Pipeline path resolution;
- construction of the request consumed by the existing runner loop.

`OpenVisionRecipeExecutionSessionViewModel` retains the public async entry
point, running/stop flags, localized status projection, and command-state and
batch-save events. It now asks the existing runner for one request and passes
that request to the existing execution method.

```text
CommandSurface
  -> OpenVisionRecipeExecutionSessionViewModel
  -> OpenVisionRecipeValidationSetRunner.CreateRunRequest
       (Local input snapshot + Pipeline path)
  -> OpenVisionRecipeValidationSetRunner.RunAsync
       (XML/engine call + expected outcome + stop/partial save + batch storage)
  -> session status/command projection
```

The selected-sample, Good/Bad pair, and Catalog owners remain unchanged and
are not re-split. Recipe XML, explicit Preview/Run, Layer/ImageSpace routing,
report paths, and Local expected outcomes remain unchanged.

## Changed files

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeExecutionSessionViewModel.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetRunner.cs`
- `docs/admin/CODEBASE_STRUCTURE.md`
- `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`
- `docs/admin/OPENVISIONLAB_DOCUMENTATION_MAP.md`
- `docs/LLM_DOCUMENT_INDEX.json`
- `docs/README.md`
- `docs/reports/OPENVISIONLAB_OVL16_RECIPE_LOCAL_VALIDATION_EXECUTION_OWNER_20260908.md`

## Verification evidence

All generated logs and copied runtimes are under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-local-validation-owner-20260908`.

- OpenVisionLab Release x64 build: PASS, 0 warnings/0 errors
  (`app-release-build.log`).
- VisionRecipeRunnerSmoke Release x64 build: PASS, 0 warnings/0 errors
  (`runner-release-build.log`).
- OpenVisionLab Debug x64 build: PASS, 0 warnings/0 errors
  (`app-debug-build.log`).
- VisionRecipeRunnerSmoke Debug x64 build: PASS, 0 warnings/0 errors
  (`runner-debug-build.log`).
- Copied-runtime Recipe execution contract: PASS 12/12 in Release and Debug.
  The contract covers Local metadata and expected outcomes, one-image partial
  stop, report persistence, missing-XML recovery for all workflows, pair and
  Catalog regression, and byte-for-byte Recipe XML preservation. Reports are
  `recipe-execution-session-contract.txt` and
  `debug/recipe-execution-session-contract.txt`.
- Static ownership proof: `structure-proof.txt`. The session no longer
  resolves the Pipeline path, copies Local image records, constructs the run
  request, reads XML, or saves the Local batch. Those signals resolve in the
  named runner, which has no WPF, Shell, or session coupling.
- `Invoke-RefactorAudit.ps1 -Verify`: PASS,
  `CSharpFiles=778`, `XamlFiles=58`, `PartialDeclarations=106`,
  `ViewModelUiIoFiles=1`, `ProjectCycles=0`, `ShellStorageCalls=0`.
- Documentation index validation, JSON parsing, and `git diff --check`:
  PASS. The existing LF-to-CRLF notices remain the only diff-check notices.

## Junior readability assessment

Pass for this boundary. A new contributor can follow the Local command into the
session state projection, then one concrete runner for input snapshot,
Pipeline path, execution, stop handling, and persistence. The session no
longer mixes Local input preparation with UI state and the execution loop.

## Completion record

```text
Status: Complete
Scope: Local validation-set input snapshot, Pipeline path resolution, execution, partial-save, and batch-storage ownership
Acceptance criteria:
  - Existing ValidationSetRunner owns the remaining Local input preparation: PASS
  - Local metadata, expected outcomes, stop/partial-save, reports, and notifications remain compatible: PASS
  - Missing-XML recovery and byte-for-byte Recipe XML preservation remain compatible: PASS (12/12 Debug and Release)
  - OVL-12 through OVL-15 completed owners are not duplicated or re-split: PASS
  - Focused Debug/Release builds and structure checks pass: PASS
Verification: app/runner Debug and Release builds, copied-runtime 12-case contract in both configurations, static ownership proof, module audit, documentation index, JSON parse, git diff check
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-local-validation-owner-20260908
Boundary / next dependency: Shell CommandSurface validation/evidence or step-edit call path is the next independent slice; do not reopen OVL-12 through OVL-16 without a newly reproduced defect, changed contract, or proven responsibility conflict.
```

## Next priority

Residual validation/evidence or Step Edit responsibility in
`OpenVisionShellHostRecipeCommandSurface`.

Recommended model: `gpt-5.6-terra`
Reasoning effort: `high`
