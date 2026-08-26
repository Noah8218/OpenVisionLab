# OpenVisionLab P256 Four-Step Route Clarity Completion

Updated: 2026-08-24 KST  
State: Complete

## Scope

Current Dev-source verification and the smallest shared cleanup for the
bounded operator route:

`Filter -> Threshold -> Morphology -> Blob -> restart -> explicit Run Review`

The route preserves explicit Preview/Run, saved layer routing, active-layer
behavior, Pipeline Review, and the existing Tool View/Recipe Manager flow. No
renderer, GPU framework, algorithm, concurrency, camera/PLC/I/O, or broad UI
redesign was added.

## Baseline and root cause

The first current-source walkthrough reached all four Tool View previews and
save actions, but Pipeline Review contained only `Threshold`, `Morphology`,
and `Blob_1`; `Filter` was missing. The saved P256 pipeline pointed
`Threshold` at `Filter_Preview`, while the `Filter` step had been written to
the `Default` recipe. The baseline review frame and timeline are retained
under the evidence root.

The shared root cause was in
`src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Documents/OpenVisionNativeToolDocumentCache.cs`:
an existing cached native Tool document always received
`ApplyRecipeContext(recipeContext)`. Background prewarm uses the legacy
overload with a null context, so it could reset an active document to
`Default` while the operator was teaching a different recipe.

The fix applies a recipe context only when explicit Tool activation supplies
one. The legacy background-prewarm path now leaves an already active document
unchanged; a newly created document still receives its creation context.

## Acceptance criteria

- AC1: Pass. The pre-fix evidence records four Tool View save actions followed
  by a Pipeline Review lookup failure for `01 Filter`. The saved comparison
  XML has three Steps and the Default-recipe comparison retains Filter.
- AC2: Pass. The cache fix is included in the current application assembly;
  the focused existing recipe-context switch smoke returned `OK` after the
  fix.
- AC3: Pass. The current actual EXE walkthrough records all four routes before
  restart in order:
  `Main -> Filter_Preview`,
  `Filter_Preview -> Threshold_Preview`,
  `Threshold_Preview -> Morphology_Preview`, and
  `Morphology_Preview -> Blob_Preview`.
- AC4: Pass. After application restart, recipe restoration, and image reload,
  the same four ordered routes are recorded as restored without running the
  Pipeline. The captured review state shows `Pipeline / 4 steps` and all
  Steps in `WAIT` before the explicit run.
- AC5: Pass. The only Run Review click occurs after restored route review and
  completes with `OK / 24.3 ms`; the final state records `OK 4 / NG 0 /
  WAIT 0`, a `572x420` result, and Blob candidate evidence (`12` candidates /
  `12` inspection objects). The final capture visibly shows the four green
  Step results and the Blob output drawing.
- AC6: Pass. The actual EXE used the latest current-source Debug build and the
  dynamic two-monitor rule selected `\\.\DISPLAY2`, bounds
  `-1920,365,1920,1080`, with no fallback. The recorded window bounds
  intersect that selected monitor.
- AC7: Pass. The route XML, sample image, source/build hashes, timeline,
  video, baseline comparison, and current route/run captures are retained in
  the D-drive evidence folder.

## Source and evidence identity

- Repository: `C:\Git\OpenVisionLab_Dev` only; unrelated dirty changes were
  preserved; no original-repository, commit, push, release, or deployment
  action was performed.
- Source baseline: Dev `HEAD 827a22e9`; the cache fix is an uncommitted
  current-worktree change included in the build below.
- EXE SHA-256:
  `C3EC8B7685D7E92EFDF96B24AFFE18760D23FDEA8B5CDD29BA6179815BCC0BD1`.
- Application assembly SHA-256:
  `03E6D193CFA8E7B089EB2CECAB7BDC662FA5CAC9DB4CA9AB058D5EDD88A70341`.
- Fixture:
  `docs/samples/public/Blob_Particles_Synthetic_OK.png`, SHA-256
  `E4D9283FF041F8BCAA0B61E005CEB135B23DF2E87C3BA45C05C68CAA49920387`.
- Current captured pipeline XML SHA-256:
  `258D2C1080C09DC57C3DF543DE0B0AAF7FE1D2384294C329F19EF85725F982C0`.

## Verification

Commands and checks actually run for this slice:

```text
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" --nologo -> 0 warnings, 0 errors
PowerShell parser for tools/OperatorWalkthroughCapture/Record-OperatorWalkthrough.ps1 -> PASS
dotnet run --no-build --project tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_context_switch <D-drive output> -> OK
powershell -NoProfile -ExecutionPolicy Bypass -File tools/OperatorWalkthroughCapture/Record-OperatorWalkthrough.ps1 -Scenario novice-four-step-route-clarity -OutputDirectory <D-drive output> -RuntimeDirectory bin/Debug -> Status=Complete
Persisted Pipeline.xml parse -> 4 Steps and the four expected routes
dotnet run --project tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev" -> passed
powershell -NoProfile -ExecutionPolicy Bypass -File tools/TestExternalReferences.ps1 -> passed
powershell -NoProfile -ExecutionPolicy Bypass -File tools/TestPublicSampleAssets.ps1 -> PASS (33 catalog rows, 229 assets, 17 pipelines)
powershell -NoProfile -ExecutionPolicy Bypass -File tools/TestDocumentationIndex.ps1 -> PASS (66 indexed paths, 12 routes, 101 redirects)
git diff --check -> no whitespace errors (only existing Git line-ending warnings)
```

The current actual-EXE run is the authoritative P256 acceptance run. Earlier
UIA/file-dialog attempts are retained only as harness diagnostics and are not
used as product-failure evidence.

## Reconfirmation policy

This P256 acceptance is frozen for the evidence identity above. Do not rerun
the actual-EXE walkthrough or focused smoke merely to reconfirm it when all of
the following remain unchanged:

- the effective Dev source and the cache-fix behavior;
- the EXE and application-assembly SHA-256 values;
- the fixture path and SHA-256, saved recipe/pipeline identity and XML hash;
- the P256 route/Preview/Run contract and the selected monitor bounds; and
- the retained evidence files remain readable and internally consistent.

Use this report and its D-drive evidence as the confirmation for that same
condition. Reopen verification only when the source or effective build
changes, the P256 contract or acceptance criteria changes, the fixture/recipe
identity changes, the monitor/runtime/driver conditions materially change, a
test-harness or measurement change invalidates the prior evidence, or an
actual regression is reproduced.

## Boundary / next dependency

This proves the bounded current-source route and its reproduced cached-context
regression fix. It does not prove CVR-00 novice evidence, every theme/layout/
DPI/monitor combination, every native driver, arbitrary-duration stability,
multi-PC qualification, camera/PLC/I/O, release, deployment, or the original
repository. CVR-00 remains deferred until three independent first-time
participants provide unedited observations. No new feature is admitted from
this completed P256 chain without a named operator task or a verified current
regression.
