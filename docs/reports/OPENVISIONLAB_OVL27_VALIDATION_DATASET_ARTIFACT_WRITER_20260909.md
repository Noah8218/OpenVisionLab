# OpenVisionLab OVL-27 — Validation dataset artifact writer owner

- **Status:** Complete for one independently verifiable validation dataset reporting boundary.
- **Scope:** `tools/PipelineViewerScreenshotSmoke/Program.cs` no longer owns the CSV and misclassification evidence implementation used by the local validation dataset target. `ValidationDatasetArtifactWriter` now owns metric projection into `misclassification_table.csv`, persisted report/artifact resolution, original/drawing/run-report copying, false-accept/false-reject manifest creation, filename sanitization, and CSV/evidence README output.
- **Intentional boundary:** `Program` still owns dataset configuration, recipe/pipeline fixture creation, validation-suite execution and progress, summary/audit JSON, UI review-queue checks, and `IsDatasetJudgment`. `VisionPipelineRunReportStorage` remains the product report persistence owner. Recipe/XML, Preview/Run, Layer/ImageSpace, PropertyGrid, and product UI behavior are unchanged.
- **Dependency direction:** `Program` delegates two output calls to the concrete writer. The writer depends only on validation result/report models, report storage, and filesystem/CSV formatting primitives; it does not reference `Program`, WPF, or Shell view types.
- **State/data owner:** The caller owns the validation result list and artifact root. The writer owns only per-call output directories, copied artifact names, manifest rows, and CSV/evidence serialization; it retains no state after the call.
- **Observable contract:** Existing CSV columns, metric names and invariant formatting, false-accept/false-reject selection, report-relative overlay/result resolution, copied artifact names, manifest fields, README text, and output locations are preserved.
- **Duplicate-work rule:** The old reporting methods and their private helpers were found only in `Program`; all calls now route through the single concrete writer. Do not recreate or re-split this owner without a newly reproduced artifact/report defect, changed output contract, or proven responsibility conflict. Do not split the remaining fixture, execution, summary JSON, or UI review helpers by file size.

## Focused proof

`ValidationDatasetArtifactWriterContract` verifies:

1. The two dataset artifact outputs delegate from `Program` to the writer and the old methods/helpers are absent from `Program`.
2. The writer owns report loading, report-relative drawing resolution, artifact copying, CSV output, and misclassification evidence output.
3. A synthetic persisted report with metrics produces the existing CSV judgment/metric fields and source path.
4. A synthetic false reject copies the original image, selected drawing, and run report and records the manifest/README identity.

Evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl27-validation-dataset-artifact-writer-contract-rerun-20260909\validation-dataset-artifact-writer-contract.txt`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl27-validation-dataset-artifact-writer-release-contract-20260909\validation-dataset-artifact-writer-contract.txt`

## Verification

- Debug build: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform='Any CPU' -m:1 -nr:false --nologo` — 0 errors; the existing `CS8600` warning remains at `Program.cs:10523`.
- Debug focused contract: `--validation-dataset-artifact-writer-contract` — `PASS|checks=7`.
- Debug OVL-26/25/24/23/22 regression contracts — `10/10`, `7/7`, `8/8`, `7/7`, and `10/10` PASS.
- Release build: same command with `-c Release` — 0 errors; the existing `CS8600` warning remains at `Program.cs:10523`.
- Release focused contract and OVL-26/25/24/23/22 regressions — `7/7`, `10/10`, `7/7`, `8/8`, `7/7`, and `10/10` PASS.
- Refactor audit: `REFACTOR_AUDIT=PASS|CSharpFiles=799|XamlFiles=59|PartialDeclarations=108|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
- Documentation index: `DocumentationIndex=PASS IndexedPaths=245 Routes=13 RootRedirects=102` before registering this report; the index is updated with this report and must be rerun before the selective commit.
- WPF product theme/DPI runtime matrix was not run; this slice changes the window-free smoke reporting owner and does not claim product UI qualification.

## Junior readability review

**PASS for this boundary.** A junior developer can follow the dataset flow from `Program` into one named writer, then read the two public operations and their small private helpers without entering the WPF shell workflow. The writer's dependencies and non-owning caller responsibilities are documented above.

## Next slice

Inspect the remaining `PipelineViewerScreenshotSmoke` fixture/execution or summary/reporting groups for one independently testable owner. Do not repeat OVL-12 through OVL-27 or split `Program.cs` by file size.
