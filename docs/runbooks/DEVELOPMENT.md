# Developing and checking OpenVisionLab

[English README](../../README.md) · [한국어 README](../../README.ko.md) · [Build and run](BUILD.md)

## Start Here: Choose the Project

Open `OpenVisionLab.sln` for normal development. Set **OpenVisionLab** as the
startup project and use `Debug` / `Any CPU`; the product starts through
`src/OpenVisionLab/Program.cs`, then
`src/OpenVisionLab/App/Bootstrap/OpenVisionLabApplication.cs`, and finally
`OpenVisionShellHostWindow`. Do not choose a verification tool as the F5
startup project.

The solution contains the application and shared libraries. Verification
projects under `tools/` run independently; open or run their `.csproj` files
when you need a specific check.

| Work you are doing | Project or folder to open | First code to read |
| --- | --- | --- |
| Run or change the product workflow, Shell, Recipe, or Pipeline | `src/OpenVisionLab/OpenVisionLab.csproj` | `Program.cs` -> `App/Bootstrap/OpenVisionLabApplication.cs` -> `UI/Menu/Wpf/` or `Core/Recipe/` / `Core/Pipeline/` |
| Change image layers, leases, or layer display state | `src/Libraries/OpenVisionLab.ImageSpace.Core/` and `src/Libraries/OpenVisionLab.Display.Core/` | `ImageSpaceService.cs` and the display owner listed in `docs/admin/CODEBASE_STRUCTURE.md` |
| Change image canvas, ROI input, or canvas persistence | `src/Libraries/OpenVisionLab.ImageCanvas/` | `View/`, `ViewModel/`, and `RoiInteraction/` |
| Change shared PropertyGrid contracts or WPF adaptation | `src/Libraries/PropertyGrid.Abstractions/` or `src/Libraries/WpfPropertyGridBridge/` | `PropertyGridContracts.cs` -> `WpfPropertyGridAdapter.cs` |
| Change shared pipeline, docking, logging, or localization controls | the matching project under `src/Libraries/` | its `View/`, `ViewModel/`, or `Services/` owner |
| Run a structural or feature check | the matching project under `tools/` | `tools/OpenVisionReadinessCheck/`, `tools/VisionRecipeRunnerSmoke/`, or `tools/PipelineViewerScreenshotSmoke/` |

To find the code for a feature, follow
`AGENTS.md` -> `docs/README.md` ->
`docs/admin/CODEBASE_STRUCTURE.md` -> the stable contract for the feature ->
the implementation named in that document. Start with these PowerShell commands:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --project "tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj" -c Debug -- "$PWD"
```

The solution and project files define the dependency graph. The application
is a `WinExe`; its `Any CPU` configurations target x64. ImageCompare is a
separate executable referenced by the application. The other referenced
`src/Libraries` projects supply shared libraries. Verification executables
have their own entry points and must not replace the application startup project.

## Tool Architecture

Algorithm Tools use a PropertyGrid-based configuration model:

```text
Tool Property Model
  -> PropertyGrid SelectedObject
  -> Preview / Run
  -> VisionToolResult
  -> Metrics / Overlays / Logs
  -> Pipeline Step XML
```

The same property model supplies the tool settings and the values saved in
recipe XML. Start with a Basic, Fast, or Precise preset, then adjust individual
parameters in the PropertyGrid.

The existing property models, execution owners and XML contracts remain the
authority when changing a Tool. Use the source owner map linked above rather
than introducing a second navigation map.

## Inspection workflow review

Review changes against the full inspection task:

```text
Load image
  -> Confirm input Layer
  -> Select a Tool
  -> Configure parameters in the PropertyGrid
  -> Preview / Run
  -> Inspect the output Layer
  -> Add a Pipeline Step
  -> Review measurements and OK/NG status
  -> Save the Recipe XML
  -> Validate with Good and Bad samples
```

The workflow connects Layer, Tool, Pipeline, Recipe, Pipeline Review and Sample
Catalog. Review these questions when changing it:

| Review focus | What to check |
| --- | --- |
| Source and intermediate images | The operator can identify which image is displayed |
| Step connections | A Tool's output reaches the intended downstream input |
| Decision evidence | Score, Count and Area values can be read with the OK/NG reason |
| Recipe persistence | Saved Recipe behavior agrees with the taught Preview settings |
| Repeated checks | The same Recipe can be checked on Good and Bad samples |

These are review criteria, not a list of confirmed current defects.

## Pipeline Review

Pipeline Review shows each step in execution order, along with its retained
result. Select a step to check:

- the input and output Layer for the selected Step;
- connections and branches between Steps;
- the result image and detection drawings;
- measurements such as `ResultCount`, `AreaMax`, `ScoreMax`, and
  `LineAngleAvg`;
- the configured acceptance range and measured value;
- the reason for an OK or NG result;
- the parameters that may need adjustment.

![OpenVisionLab Pipeline Review](../assets/demo/readme-workflows/pipeline-results.png)

## Validation

Run the relevant build and readiness checks from the repository root:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --project "tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj" -c Debug -- "$PWD"
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.PublicSampleCatalog.csv -OutputDir artifacts\public_sample_catalog
```

Choose focused checks for the affected owner. The full sample catalog is
relevant when the changed execution or sample contract requires it.

Run a current-source WPF view check:

```powershell
dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\smoke\recipe_language_controls
```

The former README's actual-EXE example uses an embedded smoke runner.
That runner is disabled in normal builds; enable its existing project property
only in an isolated verification build:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" -p:OpenVisionLabEnableEmbeddedSmokeRunner=true
.\bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs --output artifacts\smoke\recipe_manager_tabs
```

The opt-in is defined in the [application project](../../src/OpenVisionLab/OpenVisionLab.csproj)
and checked by [Program.cs](../../src/OpenVisionLab/Program.cs).
This invocation is a development smoke example; it does not replace the
[fresh-runtime procedure](BUILD.md#run-a-source-build).
Do not treat a normal build's ignored `--smoke` argument as a passed test.

When changing the UI or a workflow, run the affected steps and capture the
results from that build. An older screenshot does not verify the change.
Distinguish source/view checks, actual-EXE interaction and algorithm validation.

## Continuous Integration

[The GitHub Actions workflow](../../.github/workflows/ci.yml) is the authority
for CI commands. It runs on Windows and calls
[`tools/VerifyReleaseCandidate.ps1`](../../tools/VerifyReleaseCandidate.ps1)
with `-SkipLaunch`.

Its checks include Debug/Release solution builds, readiness contracts,
external references, retained NOTICE coverage, public-sample asset policy and
sample execution, and the clean package/distribution contract.
A manual workflow run also uploads the candidate artifacts.
This describes the workflow; it is not a claim that a current run passed.

Run WPF screenshot checks and actual-EXE UI checks locally when a change
affects the displayed controls or workflow. CI's `-SkipLaunch` does not verify
those interactions.

## Releases

- Change history: [CHANGELOG.md](../../CHANGELOG.md)
- Release and version policy:
  [docs/OPENVISIONLAB_RELEASE_VERSION_POLICY.md](../OPENVISIONLAB_RELEASE_VERSION_POLICY.md)

The version in [`src/OpenVisionLab/OpenVisionLab.csproj`](../../src/OpenVisionLab/OpenVisionLab.csproj)
is the source for the version shown in the README and changelog.
Keep the source version separate from the version of an already-published ZIP.
A source build or CI artifact is not release publication or deployment.

## Reviewing a change

Changes are reviewed against the inspection workflow and its existing contracts:

- [Inspection workflows](../manual/README.md)
- [Tool and Recipe contracts](../OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md)
- [Public examples](../samples/public/README.md)
- [Source ownership](../admin/CODEBASE_STRUCTURE.md)

For a Tool change, include the input images, parameters, output Layer and
acceptance metrics. For a workflow change, include a reproducible operator task
and evidence from the affected controls. Validate a Recipe with images from
the intended inspection; public samples establish only their own cases.

## Further documentation

[Tutorial](../learn/OPENVISIONLAB_TUTORIAL.md) · [Learn Mode](../learn/README.md) ·
[Documentation index](../README.md) ·
[Public sample catalog](../samples/OpenVisionLab.PublicSampleCatalog.csv) ·
[Product sample catalog](../samples/OpenVisionLab.ProductSampleCatalog.csv)
