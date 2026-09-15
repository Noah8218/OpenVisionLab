# OpenVisionLab OVL-13 Selected Sample Execution Owner

Updated: 2026-09-08 KST
Status: Complete for the single selected-sample execution boundary.

## Why this boundary was reopened

The earlier Recipe execution record correctly moved the five execution entry
points out of the Shell CommandSurface and into
`OpenVisionRecipeExecutionSessionViewModel`. A fresh audit still found that
the session VM directly read pipeline XML, invoked the selected-sample service,
and saved the selected-sample batch. That is a concrete responsibility conflict
with the audit target of a session VM that owns running state and result
projection. This slice resolves only the selected-sample check and selected
sample suite paths; pair, catalog, and local-set paths remain in the session for
their own later slices.

## Structural change

Before this slice, the session VM owned binding state, status projection,
selected-sample XML loading, sample execution, and selected-sample batch
storage.

`OpenVisionRecipeSelectedSampleExecutionOwner` now owns the two selected-sample
operation paths:

- resolving and reading the existing Pipeline XML;
- invoking the existing safe sample-check service;
- creating the selected-sample run report and batch summary for the suite path.

The session VM still owns running flags, `PropertyChanging`/
`PropertyChanged` notifications, summary projection, localized status/error
translation, command-state events, and the unchanged public async methods.
The owner has no WPF, Window, Shell, or session dependency.

Call path:

```text
CommandSurface command
  -> OpenVisionRecipeExecutionSessionViewModel
  -> OpenVisionRecipeSelectedSampleExecutionOwner
  -> existing VisionPipelineSampleCheckService / BatchRunSummaryStorage
  -> session result and binding projection
```

The session still contains the corresponding XML/execution/storage calls for
Good/Bad pair, Catalog, and Local Validation Set. Those residual paths are
intentionally outside this one-slice boundary and are not claimed as complete.

## Changed files

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeSelectedSampleExecutionOwner.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeExecutionSessionViewModel.cs`
- `docs/admin/CODEBASE_STRUCTURE.md`
- `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`
- `docs/admin/OPENVISIONLAB_DOCUMENTATION_MAP.md`
- `docs/LLM_DOCUMENT_INDEX.json`
- `docs/README.md`
- `docs/reports/OPENVISIONLAB_OVL13_RECIPE_SELECTED_SAMPLE_EXECUTION_OWNER_20260908.md`

## Verification evidence

Generated artifacts and logs are on D::
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-selected-sample-owner-20260908`.

- OpenVisionLab Release x64 build — passed, 0 warnings and 0 errors. Log:
  `app-release-build.log`.
- VisionRecipeRunnerSmoke Release x64 build — passed, 0 warnings and 0 errors.
  Log: `runner-release-build.log`.
- OpenVisionLab Debug x64 build — passed, 0 warnings and 0 errors. Log:
  `app-debug-build.log`.
- VisionRecipeRunnerSmoke Debug x64 build — passed, 0 warnings and 0 errors.
  Log: `runner-debug-build.log`.
- Copied-runtime Recipe execution contract in Release — passed 12/12,
  including selected-sample check, selected suite persistence, pair/catalog/
  local regression paths, missing-XML recovery, partial stop, and byte-for-byte
  Recipe XML preservation. Evidence: `recipe-execution-session-contract.txt`.
- Copied-runtime Recipe execution contract in Debug — passed 12/12. Evidence:
  `debug/recipe-execution-session-contract.txt`.
- Static ownership search — passed. The session's selected-sample methods call
  the named owner; `File.ReadAllText`, selected-sample service execution, and
  selected-sample batch save now resolve in the owner. The owner has no WPF or
  Shell reference. Evidence is recorded in the task command output and source
  paths above.
- `Invoke-RefactorAudit.ps1 -Verify` — passed:
  `CSharpFiles=776`, `XamlFiles=58`, `PartialDeclarations=106`,
  `ViewModelUiIoFiles=1`, `ProjectCycles=0`, `ShellStorageCalls=0`. Evidence:
  `module-audit`.
- `TestDocumentationIndex.ps1` — passed, `IndexedPaths=231`, `Routes=13`,
  `RootRedirects=102`.
- `git diff --check` — exit 0. Git reported existing LF-to-CRLF conversion
  warnings for the dirty worktree; no whitespace error was returned.

No XAML, View, Recipe/XML schema, Preview/Run command contract, Layer/ImageSpace
route, or PropertyGrid policy changed. A physical UI screenshot was not needed
for this source-only operation-owner change; the existing UI visual matrix
remains a separate qualification boundary.

## Junior readability assessment

Pass for this boundary. The selected-sample call path now has one obvious
operation owner, while the session VM visibly contains state and projection
logic. A new contributor can inspect the command, session, selected-sample
owner, existing engine/storage, and result projection in that order without
following file I/O through unrelated binding members.

## Completion record

```text
Status: Complete
Scope: Selected-sample check and selected-sample suite execution/storage owner
Acceptance criteria:
  - selected-sample XML/engine/storage responsibility moved to a concrete owner: PASS
  - session public API and binding/state projection preserved: PASS
  - pair/catalog/local paths left unchanged for later slices: PASS
  - Recipe XML and Preview/Run contracts preserved: PASS (12/12 Debug and Release contract)
  - focused Debug/Release builds and structural checks pass: PASS
Verification: app/runner Debug and Release builds, copied-runtime 12-case contract in both configurations, static ownership search, module audit, documentation index, diff check
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-selected-sample-owner-20260908
Boundary / next dependency: Good/Bad pair execution and batch storage is the next independent Recipe-session slice; do not repeat this owner without a newly reproduced defect, changed contract, or proven responsibility conflict.
```

## Next priority

Good/Bad pair execution and batch-storage ownership from
`OpenVisionRecipeExecutionSessionViewModel`, preserving pair ordering, expected
rejection semantics, report paths, status notifications, and Recipe/XML bytes.

Recommended model: `gpt-5.6-terra`
Reasoning effort: `high`
